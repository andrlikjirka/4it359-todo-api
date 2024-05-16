---
marp: true
style: |
  .columns { display: grid; grid-template-columns: repeat(2, minmax(0, 1fr)); gap: 1rem; padding: 0; margin:0;}
---

# Web application development on .NET platform
### Configuration, routing, health checks

<script type="module">
  import mermaid from 'https://cdn.jsdelivr.net/npm/mermaid@10/dist/mermaid.esm.min.mjs';
  mermaid.initialize({ startOnLoad: true });
</script>

---

# Configuration

<div class="columns"><div class="columns-left">

- runtime parameters
  - deployment specific
  - system synchronization
  - disaster recovery
  - security
  - server identification

</div><div class="columns-right">

<img src="config-illustration.webp" />

</div></div>

---

# Configuration in ASP.NET

<div class="columns"><div class="columns-left">

- POCO class
  - declare properties

- sources
  - provide raw values
  - file, remote server, environment variables

- consumers
  - IOptions
  - IOptionsSnapshot
  - IOptionsMonitor

</div><div class="columns-right">

```csharp
public class TelemetryOptions
{
    public string Region { get; set; }
    public string Location { get; set; }
    public string ServerUrl { get; set; }
    public string ApiKey { get; set; }
}

// program.cs
builder.Services.Configure<TelemetryOptions>(
  builder.Configuration.GetSection("Telemetry"));
```

</div></div>

---

# Configuration sources

<div class="columns"><div class="columns-left">

- provides raw configuration values
- latter overrides former
- customization
  - IConfigurationSource 
  - IConfigurationProvider


</div><div class="columns-right">

```csharp

var builder = WebApplication.CreateBuilder(args);

// JSON file provider
builder.Configuration.AddJsonFile(
  $"appsettings.{builder.Environment.EnvironmentName}.json", true);

// environmental variables provider
// TelemetryOptions__Region = "West Europe"
// => {"TelemetryOptions": {"Region": "West Europe"}}
builder.Configuration.AddEnvironmentVariables()

// --Logging:LogLevel=Warning
builder.Configuration.AddCommandLine(args)

// dotnet user-secrets set "Movies:ServiceApiKey" "12345"
builder.Configuration.AddUserSecrets<Program>()


builder.Configuration.AddAzureKeyVault(
    new Uri($"https://{builder.Configuration["KeyVaultName"]}.vault.azure.net/"),
    new DefaultAzureCredential(),
    new AzureKeyVaultConfigurationOptions
    {
        Manager = /*your manager needs to know what secrets to use*/
    });

```

</div></div>

---

# Configuration sources

<div class="columns"><div class="columns-left">

- provides raw configuration values
- latter overrides former
- customization
  - IConfigurationSource 
  - IConfigurationProvider


</div><div class="columns-right">

```json
{
  "Region" : "North Europe",
  "Location" : "Netherlands",
  "ServerUrl": "https://localhost:57589/metrics",
  "DefaultUser": 42
}
```

```json
{
  "ServerUrl": "https://dl.contoso.com/metrics",
  "ApiKey": "fhiogherwjgtziuegt",
  "DefaultUser : null
}
```

</div></div>

---
# Configuration setup

<div class="columns"><div class="columns-left">

- part of services registration
- renew on configuration source change
- direct binding

</div><div class="columns-right">

```csharp

// just adds an instance
services.Configure<TelemetryOptions>();

// programatic configuration
services.Configure<TelemetryOptions>(options => options.Region = "West Europe");

// adds instance and maps to configuration
services.Configure<TelemetryOptions>(
  builder.Configuration.GetSection("Telemetry"));

// adds instance and maps to top level configuration
services.Configure<TelemetryOptions>(builder.Configuration);

// direct binding
var telemetryOptions = new TelemetryOptions();
builder.Configuration.Bind(telemetryOptions);      //  <--- This
builder.Services.AddSingleton(telemetryOptions);

// a bit low level approach
// you get a builder back with several advanced options
services.AddOptions<TelemetryOptions>()
  .Configure<LoggingOptions>((tOptions, lOptions => 
    tOptions.Location = lOptions.Location);
  .Bind(Configuration.GetSection("Telemetry"))

// named options
services.Configure<TelemetryOptions>(
  "prime", options => options.Server = new Uri("https://prime.com/otel"));
services.Configure<TelemetryOptions>(
  "backup", options => options.Server = new Uri("https://backup.com/otel"));
```

</div></div>

---

# IOptions

<div class="columns"><div class="columns-left">

- configuration POCO wrapper
- can be constructor injected
- doesn't reflect configuration changes

</div><div class="columns-right">

```csharp

public interface IOptions<out TOptions> where TOptions : class
{
    TOptions Value { get; }
}

public class TelemetryService
{
    private readonly TelemetryOptions _options;

    public string Region => _options.Region;

    public SomeService(IOptions<TelemetryOptions> options)
    {
        _options = options.Value;
    }
}

```

</div></div>

---

<div class="columns"><div class="columns-left">

# IOptionsSnapshot

- configuration POCO wrapper
- can be constructor injected
  - injects fresh values
  - not for singletons
- offers named options

</div><div class="columns-right">

```csharp
public interface IOptionsSnapshot<out TOptions> : IOptions<TOptions>
    where TOptions : class
{
    TOptions Get(string? name);
}

public async Task Invoke(HttpContext context)
{
    var options = context.RequestServices
      .GetRequiredservice<IOptionsSnapshot<TelemetryOptions>>();
    var region = options.Get("prime").Region;
    var location = options.Value.Location;
}
```

</div></div>

---

<div class="columns"><div class="columns-left">

# IOptionsMonitor

- configuration POCO wrapper
- can be constructor injected
- reflects configuration changes
- offers named options
- offers onChange callback

</div><div class="columns-right">

```csharp
public interface IOptionsMonitor<out TOptions>
{
    TOptions CurrentValue { get; }

    TOptions Get(string? name);

    IDisposable? OnChange(Action<TOptions, string?> listener);
}

public class TelemetryService
{
    private readonly IOptionsMonitor<TelemetryOptions> _options;
    public string Region => _options.CurrentValue.Region;
    public string GetLocation(string tier) => _options.Get(tier).Location;

    public SomeService(IOptionsMonitor<TelemetryOptions> options)
    {
        _options = options;
        options.OnChange += (newValue, name) => RestartLogger(newValue);
    }
}
```

</div></div>

---

# PostConfigure

<div class="columns"><div class="columns-left">

- after all Configure methods called
- the last word
  - final overrides
  - forcing defaults
- syncronization with other services
- conditional configuration

</div><div class="columns-right">

```csharp
builder.Services.PostConfigure<MessagingOptions>(options =>
{
    options.RetryCount = Math.Max(5, options.RetryCount);
});

public class TelemetryOptionsPostConfigure : IPostConfigureOptions<TelemetryOptions>
{
    private IOptionsMonitor<LoggingOptions> _loggingOptions;

    public MyServiceSettingsPostConfigure(IOptionsMonitor<LoggingOptions> options)
    {
        _telemetryOptions = options;
    }
    
    
    public void PostConfigure(string name, TelemetryOptions options)
    {
        options.Location = _loggingOptions.Location;
    }
}

services.Configure<TelemetryOptions>(
    Configuration.GetSection("Telemetry"));
  services
  .AddSingleton<IPostConfigureOptions<TelemetryOptions>,
  TelemetryOptionsPostConfigure>();
```

</div></div>

---

<div class="columns"><div class="columns-left">

# OptionsValidation

- validates configuration
  - on startup
  - on first usage
- data annotations
- custom logic
- may cause issues on reconfiguration

</div><div class="columns-right">

```csharp

public class MyServiceOptions
{
    [Required]
    [Url]
    public string ServiceEndpoint { get; set; }

    [Range(1, 60)]
    public int TimeoutInSeconds { get; set; } = 30;
}

public void ConfigureServices(IServiceCollection services)
{
    services.AddOptions<MyServiceOptions>()
            .Bind(Configuration.GetSection("MyServiceOptions"))
            .ValidateDataAnnotations();
}

public class MyServiceOptionsValidation : IValidateOptions<MyServiceOptions>
{
    public ValidateOptionsResult Validate(string name, MyServiceOptions options)
    {
        var failures = new List<string>();

        if (string.IsNullOrEmpty(options.ServiceEndpoint))
        {
            failures.Add($"{nameof(options.ServiceEndpoint)} is required.");
        }

        if (failures.Count > 0)
        {
            return ValidateOptionsResult.Fail(failures);
        }

        return ValidateOptionsResult.Success;
    }
}

public void ConfigureServices(IServiceCollection services)
{
    services.AddOptions<MyServiceOptions>()
            .Bind(Configuration.GetSection("MyServiceOptions"))
            .Validate<MyServiceOptionsValidation>();
}

services.AddOptions<MyServiceOptions>()
        .Bind(Configuration.GetSection("MyServiceOptions"))
        .ValidateDataAnnotations()
        .ValidateOnStart();

```

</div></div>

---

<div class="columns"><div class="columns-left">

# using IConfiguration directly

- avoid if possible
  - use strongly typed IOptions
- when to use
  - at startup
  - runtime based keys
  - locality (take next key, previous etc.)
  - need to bind manually
  - need to register callbacks

</div><div class="columns-right">

```csharp
public interface IConfiguration
{
    string? this[string key] { get; set; }

    IConfigurationSection GetSection(string key);

    IEnumerable<IConfigurationSection> GetChildren();

    IChangeToken GetReloadToken();
}

public class MyService
{
    private readonly IConfiguration _configuration;
    private readonly IOptionsMonitor<SecurityOptions> _securityOptions;

    public MyService(IConfiguration configuration,
      IOptionsMonitor<SecurityOptions> options)
    {
        _configuration = configuration;
        _options = options;
    }

    public void PrintSettings()
    {
        var appKey = _configuration.GetSection("Security:Keys")
          .GetValue<string>(_securityOptions.CurrentValue.KeyName);
        /*...*/
    }
}
```

</div></div>

---

# Routing

<div class="columns"><div class="columns-left">

- maps requests to endpoints
- template driven
- convention based routing
- attribute based routing


</div><div class="columns-right">

```csharp
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});


[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    [HttpGet("{id}")]
    public IActionResult GetProduct(int id) { /* ... */ }
}
```

</div></div>

---

<div class="columns"><div class="columns-left">

# route parameters

- defined by {}
  - {controller}/{action}/{id}
- may be optional
  - {controller}/{action}/{id?}
- default values
  - {controller=Home}/{action=Index}/{id?}

</div><div class="columns-right">

- catch-all parameters
  - "Blog/{*slug}"
- type constraints
  - "{controller=Home}/{action=Index}/{id:int?}"

```csharp
public class BlogController : Controller
{
    public IActionResult Post(int id)
    {
        return View(id);
    }
}
```

</div></div>

---

# Health checks

<div class="columns"><div class="columns-left">

- reports app state
- used by LBs, orchestrators
- 

</div><div class="columns-right">

```csharp
services.AddHealthChecks();

services.AddHealthChecks()
    .AddSqlServer(connectionString: "YourConnectionString");

app.UseEndpoints(endpoints =>
{
    endpoints.MapHealthChecks("/health");
});

```

</div></div>

---

# Custom health check

```csharp
public class ExampleHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
      CancellationToken cancellationToken = new CancellationToken())
    {
        var healthCheckResultHealthy = true;

        if (healthCheckResultHealthy)
        {
            return Task.FromResult(HealthCheckResult.Healthy("A healthy result."));
        }

        return Task.FromResult(HealthCheckResult.Unhealthy("An unhealthy result."));
    }
}
```

```csharp
services.AddHealthChecks()
    .AddCheck<ExampleHealthCheck>("example_health_check");

services.AddHealthChecks()
    .AddCheck<ExampleHealthCheck>("example", tags: new[] { "ready" });

app.UseEndpoints(endpoints =>
{
    endpoints.MapHealthChecks("/health", new HealthCheckOptions
    {
        ResponseWriter = async (context, report) =>
        {
            context.Response.ContentType = "application/json";
            var response = new
            { 
              status = report.Status.ToString(),
              checks = report.Entries
                .Select(x => new { name = x.Key, status = x.Value.Status, exception = x.Value.Exception?.Message ?? "none", duration = x.Value.Duration })
            };
            await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
        }
    });
});
```

---

# Exercise

- make the background service configurable:
  - option to start it or not at all
  - option for sweep interval
  - option for lowest priority to remove
    - validate its range

---

# Should Configuration ever change?

<div class="columns"><div class="columns-left">

## yes

- disaster recovery
- scaling
- logging level
- leader change
- throttling

</div><div class="columns-right">

# no

- runtime changes arent't tested
- servers under load
  - processing customer data
- inconsistent configurations
- human error

</div></div>

---

# Thank you!
