---
marp: true
style: |
  .columns { display: grid; grid-template-columns: repeat(2, minmax(0, 1fr)); gap: 1rem; padding: 0; margin:0;}
---

# Web application development on .NET platform
### Observability

<script type="module">
  import mermaid from 'https://cdn.jsdelivr.net/npm/mermaid@10/dist/mermaid.esm.min.mjs';
  mermaid.initialize({ startOnLoad: true });
</script>

---

# What is observability

<div class="columns"><div class="columns-left">

- system property
- can we reason abou the system?
- logs
- traces
- metrics

</div><div class="columns-right">

<img src="authentication.webp" />

</div></div>

---

# logs

<div class="columns"><div class="columns-left">

- most customizable
- usually stringified
- can be parsed
  - data lake
  - data warehouse
  - map reduce

</div><div class="columns-right">

```text
Configuring middleware: AuthenticationMiddleware initialized at [timestamp]
Responding with 404 Not Found for URL '/api/products/999
Executed query 'SELECT * FROM products WHERE category = 'books'' in 85ms
```

</div></div>

---

# enhanced logs

<div class="columns"><div class="columns-left">

- common information
  - timestamp
  - instance ID
  - trace ID

</div><div class="columns-right">

```text
2024-04-21T12:34:56.789Z|232553|fh234jfd23|
  Configuring middleware: AuthenticationMiddleware initialized at [timestamp]
2024-04-21T12:34:56.789Z|232553|fh234jfd23|
  Responding with 404 Not Found for URL '/api/products/999
2024-04-21T12:34:56.789Z|232553|fh234jfd23|
  Executed query 'SELECT * FROM products WHERE category = 'books'' in 85ms

232553|fh234jfd23
2024-04-21T12:34:56.789Z|
  Configuring middleware: AuthenticationMiddleware initialized at [timestamp]
2024-04-21T12:34:56.789Z|
  Responding with 404 Not Found for URL '/api/products/999
2024-04-21T12:34:56.789Z|
  Executed query 'SELECT * FROM products WHERE category = 'books'' in 85ms
```

</div></div>

---

# data classification

<div class="columns"><div class="columns-left">
  
  - public information
    - generally known information
      - published content
      - shared data
    - no risks when leaked
    - no special measures

</div><div class="columns-right">

<img src="auth-props.webp" />

</div></div>

---

# data classification

<div class="columns"><div class="columns-left">
  
  - operational information (OI)
    - information created by us about user
      - pseudo-identifiers
      - unlinkable times, numbers...
    - risks when leaked
      - identity misuse
      - reputation damage
    - measures
      - multiple eyes access

</div><div class="columns-right">

<img src="auth-props.webp" />

</div></div>

---

# data classification

<div class="columns"><div class="columns-left">
  
  - Personal / Organizational identifiable information (PII, OII)
    - identify a person on its own or in context
      - birth number
      - phone number
      - address
    - risks when leaked
      - user can be linked
    - measures
      - encrypted or hashed
      - user initiated access

</div><div class="columns-right">

<img src="auth-props.webp" />

</div></div>

---

# data classification

<div class="columns"><div class="columns-left">

- customer data
  - information created by customer
    - order history
    - chat messages
    - configurations
    - bussines plans
  - risks when leaked
    - personal details misuse
  - measures
    - encrypted (ideally by BYOC)
    - locality and temporarity
    - user assisted access

</div><div class="columns-right">

<img src="auth-props.webp" />

</div></div>

---

# tracing

<div class="columns"><div class="columns-left">

- traces calls through multiple services
  - performance evaluation
  - bottleneck search
  - hotspot and SPoF identification
- Open Telemetry format
  - version
  - trace id
  - span id
  - flags

</div><div class="columns-right">

```text
00-4bf92f3577b34da6a3ce929d0e0e4736-00f067aa0ba902b7-01
```

- 00
  - version prefix
- 4bf92f3577b34da6a3ce929d0e0e4736
  - trace-id (128-bit)
- 00f067aa0ba902b7
  - span-id (64-bit)
- 01
  - trace-flags (sampled)

</div></div>

---

# tracing

<img src="traces_spans.png" />

---

# metrics

<div class="columns"><div class="columns-left">

- measurements indicating a behaviour
- purpose
  - performance audit
  - hotspot identification
  - monitoring and alerting
  - debugging and trouble-shooting

</div><div class="columns-right">

- multiple parts
  - identifier
  - timestamp
  - value
  - dimensions
    - name/value pairs

- cardinality
  - unique dimension values
  - high cardinality is bad
    - storage, performance, interpretation...

</div></div>

---

<div class="columns"><div class="columns-left">

# metric types

- counter
  - counts occurences of events
  - increasing, dicreasing
  - monotonic, non-monotonic
- gauge
  - reports a measurement at a time
  - CPU usage
- histogram
  - captures distribution of values

</div><div class="columns-right">

# metric processing

- creation
- client aggregation
- export
- store
- system aggregation
- presentation

</div></div>

---

# Ilogger

<div class="columns"><div class="columns-left">

- general interface
- throughout the whole dotnet
- generic version
- lot of extensions

</div><div class="columns-right">

```csharp
public interface ILogger
{
    void Log<TState>(
      LogLevel logLevel,
      EventId eventId,
      TState state,
      Exception? exception,
      Func<TState, Exception?, string> formatter);

    bool IsEnabled(LogLevel logLevel);

    IDisposable? BeginScope<TState>(TState state) where TState : notnull;
}

public interface ILogger<out TCategoryName> : ILogger
{
}

void LogTrace(string message);
void LogTrace(Exception exception, string message, params object[] args);
void LogDebug(string message);
void LogDebug(Exception exception, string message, params object[] args);
void LogInformation(string message);
void LogInformation(Exception exception, string message, params object[] args);
void LogWarning(string message);
void LogWarning(Exception exception, string message, params object[] args);
void LogError(string message);
void LogError(Exception exception, string message, params object[] args);
void LogCritical(string message);
void LogCritical(Exception exception, string message, params object[] args);
```

</div></div>

---

# log levels

<div class="columns"><div class="columns-left">
  
- critical
  - need to stop immediately
- error
  - investigate and fix
- warning
  - shouldn't happen but not severe
- information
  - audit information
- debug
  - root cause identification
- trace
  - everything

</div><div class="columns-right">

```json
{
    "Logging": {
        "LogLevel": {
            "Default": "Information",
            "Microsoft": "Warning",
            "Microsoft.Hosting.Lifetime": "Information"
        }
    }
}
```

</div></div>

---

# Registration

<div class="columns"><div class="columns-left">

- adds logging mechanism
- adds outputs
- filtering
- advanced configuration

</div><div class="columns-right">

```csharp
builder.Logging.ClearProviders();
builder.Logging.AddDebug();
builder.Logging.AddSimpleConsole(options => options.IncludeScopes = true);

/************************************************************************/

builder.ConfigureLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole(options =>
    {
        options.LogToStandardErrorThreshold = LogLevel.Error;
    });
    logging.AddDebug();
    logging.AddFilter("Microsoft", LogLevel.Warning);
    logging.AddFilter("System", LogLevel.Error);
});

```

</div></div>

---

# logging

<div class="columns"><div class="columns-left">

- use extension methods
  - interface is primarily for frameworks
- don't use string operations
  - use built-in formatter instead

</div><div class="columns-right">

```csharp

public class OrderService
{
    private readonly ILogger<OrderService> _logger;

    public OrderService(ILogger<OrderService> logger, ITransaction transaction)
    {
        _logger = logger;
    }

    public void GetProduct(int id)
    {
      try
      {
          var order = transaction.Start(id);
          _logger.LogInformation("Getting product with ID: {ProductId}", id);
          return order;
      }
      catch (Exception ex)
      {
          _logger.LogError(ex, "Failed to get product with ID: {ProductId}", id);
      }
    }
}
```

</div></div>

---

<div class="columns"><div class="columns-left">

# logging scopes

- creates logging context
- attached to all log instances
- use short scopes
  - performance ramifications
- use consistent key names
  - avoid obfuscating your logs

</div><div class="columns-right">

```csharp
using(_logger.BeginScope(
  new Dictionary<string, object> { { "TransactionId", order.Id } }))
{
    _logger.LogInformation("Starting transaction");
    _transaction.Start()
    _logger.LogInformation("Transaction completed");
}
```

```json
{
  "Timestamp": "2024-04-21T12:34:56.789Z",
  "Level": "Information",
  "MessageTemplate": "Starting transaction",
  "Properties": { "TransactionId": "12345" }
},
{
  "Timestamp": "2024-04-21T12:34:56.888Z",
  "Level": "Information",
  "MessageTemplate": "Writing data",
  "Properties": { "TransactionId": "12345" }
},
{
  "Timestamp": "2024-04-21T12:34:58.789Z",
  "Level": "Information",
  "MessageTemplate": "Transaction completed",
  "Properties": { "TransactionId": "12345" }
}
```

</div></div>

---

# Open Telemetry

<div class="columns"><div class="columns-left">

- consorcium-based logging standard
- define interfaces and concepts
  - same for all providers
  - same for all languages

</div><div class="columns-right">

```bash
dotnet add package OpenTelemetry.Exporter.Console
dotnet add package OpenTelemetry.Extensions.Hosting
dotnet add package OpenTelemetry.Instrumentation.AspNetCore
```

```csharp
builder.Services.AddOpenTelemetry()
  .ConfigureResource(resource => resource
      .AddService(serviceName: "TodoApp"))
  .WithTracing(tracing => tracing
      .AddAspNetCoreInstrumentation()
      .AddConsoleExporter());
```

</div></div>

---

# Exercise

- add logging to ItemsController
  - think about multiple levels of logging
  - ensure we can turn it off though app configuration
- add OpenTelemetry request instrumentation
  - export to console
---

# Observability questions

- price
- performance
- legistation

---
# Thank you!
