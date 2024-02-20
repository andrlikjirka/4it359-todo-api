
This week, you’re provided with a data set containing information about cars in JSON format.

There’s a `Car` DTO available in the `Car.cs` file.

In your program, you’ll need to load that data set into memory and then write following queries:

1. List first 20 cars which were manufactured in Japan between years 1970-1980 and pick only Name and Year properties. Year property should be parsed in order to show only a year part of the date.
2. List all 4-cylinder cars ordered from the fastest to the slowest, and for cars that have identical acceleration, order them by their gasoline consumption from the greediest and pick only Name and Year properties.
3. List origins ordered by a sum of all car models manufactured in a given country or origin.

Write each resulting list into console.

## File reading hint

<details>

```csharp
var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
using var file = File.OpenRead("./Data/car-data.json");
var cars = JsonSerializer.Deserialize<List<Car>>(file, options);
```

</details>

## Date parsing hint

<details>

```csharp
var year = DateTime.ParseExact(car.Year, "yyyy-mm-dd", CultureInfo.InvariantCulture).Year;
```

</details>