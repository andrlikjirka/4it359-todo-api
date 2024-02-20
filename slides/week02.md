---
marp: true
style: |
  .columns { display: grid; grid-template-columns: repeat(2, minmax(0, 1fr)); gap: 1rem; padding: 0; margin:0;}
---

# Web application development on .NET platform
### C# language features

<script type="module">
  import mermaid from 'https://cdn.jsdelivr.net/npm/mermaid@10/dist/mermaid.esm.min.mjs';
  mermaid.initialize({ startOnLoad: true });
</script>

---

# what will we talk about

<div class="columns"><div class="columns-left">

- properties
- value types and reference types
- extension methods
- nullables
- delegates and expressions
- generic constraints

</div><div class="columns-right">

- tuples and deconstruction
- ref and out parameters
- attributes
- records
- async / await
- IEnumerable
- LINQ

</div></div>

---

# Properties

<div class="columns"><div class="columns-left">

- autoproperty
- backing field
- read-only / writeonly
- access modifiers
- validation
- lazy loading
- computed properties
- better encapsulation
- reflection friendly

</div><div class="columns-right">

```csharp
public class Person
{
    public bool Selected { get; set; }
    protected DateTimeOffset Birth { get; }
    public int NicknamesCount {get; private set;}

    public Person(DateTime birth) => Birth = birth;

    private string? _nick;
    public string Nick
    {
        get => _nick ?? "world";
        set
        {
            if (value != _nick)
            {
                _nick = value;
                _greeting = null;
                NicknamesCount++;
            }
        }
    }

    private string? _greeting;
    public string Greeting
    {
        get => _greeting ?? "Hello";
        set
        {
            _greeting = string.IsNullOrEmpty(value)
                ? throw new ArgumentException("Name cannot be null or empty.")
                : value;
            _greetingPhrase = null;
        }
    }

    private string? _greetingPhrase;
    public string GreetingPhrase => _greetingPhrase ??= $"{_greeting} {Nick}!";

    public int Age => DateTime.Now.Year - Birth.Year;
}
```

---

<div class="columns"><div class="columns-left">

# value types
- stored on stack
- default value if unassigned
- copied by value
- need to be boxed
- live in function frame
- Nullable&lt;T> or T?

```
public struct MyValue { }
```


</div><div class="columns-right">

# reference types
- stored on a heap
- null if unassigned
- copied by reference
- don't need to be boxed
- accessible till used

```
public class Myclass { }
```

</div></div>

---

# Extension methods

<div class="columns"><div class="columns-left">

- static method in a static class
- first parameter with `this` keyword
- same syntax as a direct member call
  - compiler rewrites it actually
- can only access class's public API
- piggy tailing namespace

</div><div class="columns-right">

```csharp
public static class StringExtensions
{
    public static string ToTitleCase(this string str)
    {
        if (string.IsNullOrEmpty(str))
        {
          return str;
        }

        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str.ToLower());
    }
}

string title = "hello world".ToTitleCase();
```

</div></div>

---

# Nullable feature

<div class="columns"><div class="columns-left">

- disabled
  - default
  - references are nullable
- enabled
  - references are non-nullable
  - exception `MyType?`
  - suppress check `myVar!`


</div><div class="columns-right">

```
// in .csproj file
<Nullable>enable</Nullable>

// in .cs files
#nullable disable
#nullable enable
```

```
public static string Coallesce(string? value, string defaultValue)

var result = Coallesce(myValue, myDefault!)
```

</div></div>

---

# Coallescing values

<div class="columns"><div class="columns-left">

- fallback if a variable is null
  - using `??` operator
  ```csharp
  var result = greeting ?? "hello";
  ```

<br/>

- fallback variable's member access
  - using `?.` operator
  ```csharp
  var length = greeting?.Length ?? 0;
  ```

</div><div class="columns-right">

- assign if a variable is null
  - using `??=` operator
  ```csharp
  greeting ??= "hello";
  ```

</div></div>

---

# delegates

<div class="columns"><div class="columns-left">

- functors | function pointers
- allow using methods as values
- can be multicasted
  - using += operator

</div><div class="columns-right">

```csharp
public delegate int Addition(int x, int y);

public class Foo
{
  public int Add(int a, int b) => Math.Abs(a) + Math.Abs(b);
}

public class Bar
{
  private Addition _addition;

  public Bar(Addition addition)
  {
      _addition = addition;
  }

  public int Add(int a, int? b) => _addition(a, b ?? 0);
}
```

</div></div>

---

# Lambda expressions

- better delegates
- used heavily in LINQ and event handlers

<div class="columns"><div class="columns-left">

## Action&lt;T, ...>

- returns void

```
Action<string> greet = name =>
{
    var greeting = $"Hello, {name}!";
    Console.WriteLine(greeting);
};

greet("Alice");
```

</div><div class="columns-right">

## Func&lt;T, ...>

- returns the last generic type

```
var factor = 5;
Func<int, int, int> add = (x, y) => x + y + factor;

int result = add(5, 3); // Outputs: 13
```

</div></div>

---

# generic constraints

- using `where` keyword 
  ```csharp
  public class MyGenericClass<T> where T : IComparable<T> { }
  ```

<br/>

- T must be a reference type with parameterless constructor
  ```csharp
  public T GetValue<T>() where T : class, new() { }
  ```

<br/>

- T must be a value type
  ```csharp
  public class ValueGeneric<T> where T : struct { }
  ```

---

# value tuples and deconstruction

<div class="columns"><div class="columns-left">

- sequence of elements with different data types
- named tuples
- deconstruction
- live on stack

</div><div class="columns-right">

```csharp
public (string, int) GetPersonInfo()
{
    return ("Alice", 30); // Returns a tuple with a string and an int
}

var personInfo = GetPersonInfo();
Console.WriteLine($"Name: {personInfo.Item1}, Age: {personInfo.Item2}");

(string Name, int Age) GetPersonInfo()
{
    return ("Alice", 30);
}

var personInfo = GetPersonInfo();
Console.WriteLine($"Name: {personInfo.Name}, Age: {personInfo.Age}");

var (name, age) = GetPersonInfo(); // Deconstructs the returned tuple
Console.WriteLine($"Name: {name}, Age: {age}");

string name;
int age;
(name, age) = GetPersonInfo(); // Updates name and age
```

</div></div>

---

# ref parameters

<div class="columns"><div class="columns-left">

- passed by stack reference


</div><div class="columns-right">

```csharp
void Increment(ref int number)
{
    number += 1;
}

int myNumber = 5;
Increment(ref myNumber);
Console.WriteLine(myNumber); // Outputs: 6
```

</div></div>

---

# out parameters

<div class="columns"><div class="columns-left">

- passed by stack reference
- callee supposed to assign the reference
- used for initialization

</div><div class="columns-right">

```csharp
bool TryParse(string input, out int result)
{
    return int.TryParse(input, out result);
}

if (TryParse("123", out var number))
{
    Console.WriteLine(number); // Outputs: 123
}
else
{
    Console.WriteLine("Failed to parse.");
}
```

</div></div>

---

# attributes

<div class="columns"><div class="columns-left">

- code metadata
- used by reflection
- counterpart to Java annotations
- declarative
- resolved very early


</div><div class="columns-right">

```csharp
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method,
  AllowMultiple = false, Inherited = true)]
public class InfoAttribute : Attribute
{
    public string Name { get; }
    public string Description { get; set; }

    public InfoAttribute(string name) => Name = name;
}

[Info("Master", Description = "This class does something.")]
public class MyClass { }


var attributes = typeof(MyClass).GetCustomAttributes(typeof(InfoAttribute), false);
if (attributes.Length > 0)
{
    var infoAttr = (InfoAttribute)attributes[0];
    Console.WriteLine(infoAttr.Description);
}
```

</div></div>

---

# records

<div class="columns"><div class="columns-left">

- simple immutable reference types
- value based equality semantics
- great for DTOs and DDD
- `init` keyword accessors
  - properties can only be set during initialization
- with expressions
- can inherit from another record
- deconstruction



</div><div class="columns-right">

```csharp
// positional record
public record Person(string FirstName, string LastName);

// non-positional record
public record Person
{
    public string FirstName { get; init; }
    public string LastName { get; init; }
}

var person1 = new Person("John", "Doe");
var person2 = new Person("John", "Doe");
Console.WriteLine(person1 == person2); // Outputs: True

var original = new Person("Jane", "Doe");
var updated = original with { FirstName = "John" };

var person = new Person("John", "Doe");
var (firstName, lastName) = person; // Deconstructs the record
Console.WriteLine($"First: {firstName}, Last: {lastName}");
```

</div></div>

---

# async / await primer

<div class="columns"><div class="columns-left">

- simplifies asynchronous programming

- `async` marks method as asynchronous
  - returns Task or Task&lt;T>
  - boxes context on heap
  - backward compatibility
- `await` marks asynchronous call
  - pospones next line execusion till completed
- interesting state machine behind

</div><div class="columns-right">

```csharp
public async Task<int> GetNumberAsync()
{
    await Task.Delay(1000);
    return 42;
}
```

</div></div>

---

<div class="columns"><div class="columns-left">

# IEnumerable<T>

- producer interface
  - consuming collections
  - consuming coroutines
- returns IEnumerator&lt;T&gt;

  - split by spaces to a string array

</div><div class="columns-right">

```csharp
public interface IEnumerable<out T> : IEnumerable
{
  new IEnumerator<T> GetEnumerator();
}

public interface IEnumerator<out T> : IDisposable, IEnumerator
{
    new T Current { get; }
}

var array = new string[3] {"first", "second", "third"} as IEnumerable<string>

foreach (var item in array)
{
  Console.WriteLine(item);
}

public IEnumerable<string> GetArrayItems()
{
  yield return "first";
  yield return "second";
  yield return "third";
}
```

</div></div>

---

# LINQ (Language Integrated Query)

<div class="columns"><div class="columns-left">

- Filtering
  - Where
- Sorting
  - OrderBy, OrderByDescending
- Projection
  - Select
- Aggregation
  - Sum, Average, Min, Max
- Selection
  - Take, Skip, First, Last, FirstOrDefault, LastOrDefault

</div><div class="columns-right">

```csharp

var numbers = new List<int> { 5, 10, 8, 3, 6, 12};

numbers.Where(n => n % 2 == 0);
// {10, 8, 6, 12}

numbers.OrderBy(f => f);
// {3, 5, 6, 8, 10, 12}

numbers.Select(f => f + 1);
// { 6, 11, 9, 4, 7, 13 }

numbers.Max();
// 12

numbers.Sum();
// 48

numbers.Skip(2).Take(2);
// { 8, 3 }

numbers.First(n => n > 5);
// 10
```

</div></div>

---

# LINQ (Language Integrated Query)

<div class="columns"><div class="columns-left">

- declarative data processing
- consists of extension methods
- uses IEnumerable&lt;T, ...>

</div><div class="columns-right">

```csharp

public static bool Any<TSource>(this IEnumerable<TSource> source)
{
    return
        TryGetNonEnumeratedCount(source, out int count) ? count != 0 :
        WithEnumerator(source);

    static bool WithEnumerator(IEnumerable<TSource> source)
    {
        using IEnumerator<TSource> e = source.GetEnumerator();
        return e.MoveNext();
    }
}

public static bool TryGetNonEnumeratedCount<TSource>(
  this IEnumerable<TSource> source, out int count)
{
    if (source == null)
    {
        ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
    }

    if (source is ICollection<TSource> collectionoft)
    {
        count = collectionoft.Count;
        return true;
    }

    /* skipped */

    if (source is ICollection collection)
    {
        count = collection.Count;
        return true;
    }

    count = 0;
    return false;
}
```

</div></div>

---

# exercise

- pull forked repository `exercises`
- checkout branch `week02`
- create your branch on top of `week02`
- follow `readme.md`
- push and create a pull request
- let others review or find someone to do so

---

# Thank you!

---

# Good ideas and how to use them

- dogmatism
  - dedication to one idea
- eclecticism
  - combining multiple ideas

<br />

- TDD (test driven development)
- BDD (behaviour driven development)
- DDD (domain driven development)
- extreme programming techniques (pair programming)
- SCRAM (agile techniques)
- SOLID (design principles)
