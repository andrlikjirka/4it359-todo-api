---
marp: true
style: |
  .columns { display: grid; grid-template-columns: repeat(2, minmax(0, 1fr)); gap: 1rem; padding: 0; margin:0;}
---

# Web application development on .NET platform
### Authentication and authorization

<script type="module">
  import mermaid from 'https://cdn.jsdelivr.net/npm/mermaid@10/dist/mermaid.esm.min.mjs';
  mermaid.initialize({ startOnLoad: true });
</script>

---

# Authentication

<div class="columns"><div class="columns-left">

- who user is
- user is a set of properties
- user needs to prove

</div><div class="columns-right">

<img src="authentication.webp" />

</div></div>

---

# Provable properties

<div class="columns"><div class="columns-left">

- things user can prove about himself
- something he knows
  - password, personal information
- something he has
  - tokens, chip cards, certificates
- something he is
  - biometrics like fingerprints
- where he is
  - IP address, ISP
  

</div><div class="columns-right">

<img src="auth-props.webp" />

- a reference
    - identity providers

</div></div>

---

# Authentication protocols

<div class="columns"><div class="columns-left">

- Basic
- Digest
- HMAC
- Token
- OAuth
- OpenID Connect
- SAML
- SAS Token
- PoP token

</div><div class="columns-right">

<img src="auth-methods.webp" />

</div></div>

---

# Basic authentication

<div class="columns"><div class="columns-left">

- uses Authorization header
- format: `Basic <payload>`
- payload `name:password` in base64
- needs HTTPS 

</div><div class="columns-right">

```bash
GET /resource HTTP/1.1
Host: example.com
Authorization: Basic QWxpY2U6U2VjcmV0UGFzc3dvcmQxMjM=
```

</div></div>

---

# Digest authentication

<div class="columns"><div class="columns-left">

- tries to overcome basic auth issues
- challenge flow auth
  - no plain-text password
- needs additional calls 

</div><div class="columns-right">

```bash
HTTP/1.1 401 Unauthorized
WWW-Authenticate: Digest realm="example.com",
  qop="auth",nonce="dcd98b7102dd2f0e8b11d0f600bfb0c093",
  opaque="5ccc069c403ebaf9f0171e9517f40e41"
```

```bash
GET /resource HTTP/1.1
Host: example.com
Authorization: Digest username="Alice",
  realm="example.com",nonce="dcd98b7102dd2f0e8b11d0f600bfb0c093",
  uri="/resource",response="6629fae49393a05397450978507c4ef1",
  opaque="5ccc069c403ebaf9f0171e9517f40e41",qop=auth,nc=00000001,
  cnonce="0a4f113b"
```

</div></div>

---

# HMAC authentication

<div class="columns"><div class="columns-left">

- Hash-Based Message Authentication Code
- signature covers request parts
  - body
  - date
  - path
- prevents sending plain-text password
- doesn't need additional calls
  - needs additional CPU cycles
- prevents replay attacks, body tampering...

</div><div class="columns-right">

```bash
POST /api/update HTTP/1.1
Host: example.com
Content-Type: application/json
Date: Tue, 10 Nov 2020 08:00:00 GMT
Authorization: HMAC Date,
  Content-Length,3yxb9caBdq+uv3hL4Bg3xgTTXQzP6+8Q0Y1kbbY1uQ8=
Content-Length: 31

{"name":"John Doe","age":30}
```

</div></div>

---

# Bearer authentication

<div class="columns"><div class="columns-left">

- a family of token-based flows
- typically uses JWT token

</div><div class="columns-right">

```bash
GET /resource HTTP/1.1
Host: example.com
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.
  eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.
  SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c
```

</div></div>

---

# JWT token

<div class="columns"><div class="columns-left">

- tampering resistant token format
- header & payload
  - JSON
  - base64 encoded
- signature
  - header and payload
  - using cypher or certificate

</div><div class="columns-right">

```bash
eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9
  .eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ
  .SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c
```

```json
{
  "alg": "HS256",
  "typ": "JWT"
}
```

```json
{
  "sub": "1234567890",
  "name": "John Doe",
  "admin": true,
  "iat": 1516239022
}
```

</div></div>

---

# OAuth authentication

<div class="columns"><div class="columns-left">

- delegated authentication
- multiple flows
- focuses more on authorization
- uses access tokens
  - usually JWT token
- Microsoft, Google, Facebook...
- OpenID Connect
  - extends OAuth with ID tokens


</div><div class="columns-right">

```bash
POST /token HTTP/1.1
Host: authorization-server.com
Content-Type: application/x-www-form-urlencoded

grant_type=authorization_code&
  code=AUTH_CODE_RECEIVED_FROM_AUTHORIZATION&
  redirect_uri=https%3A%2F%2Fclient%2Eexample%2Ecom%2Fcb&
  client_id=CLIENT_ID&
  client_secret=CLIENT_SECRET
```

</div></div>

---

<div class="columns"><div class="columns-left" style="margin-top:10%">

# Identity

- HttpContext.User
    - ClaimsPrincipal
- identity
    - what user is
    - visitor, admin, owner
- claim
    - a truth about its identity

</div><div class="columns-right mermaid">
    classDiagram
        IPrincipal --|> IIdentity : has
        ClaimsIdentity --|> Claim : has
        ClaimsIdentity --|> IIdentity: implements
        ClaimsPrincipal --|> ClaimsIdentity : has
        ClaimsPrincipal --|> IPrincipal : implements
        HtpContext --|> ClaimsPrincipal: has
        class IIdentity {
            +Name: string?
            +AuthenticationType: string?
            IsAuthenticated: bool
        }
        class IPrincipal {
            +Identity: IIdentity?
            +IsInRole(role: string) bool
        }
        class ClaimsPrincipal{
            +Identity: IIdentity
            +Identities: IEnumerable&ltClaimsIdentity>
            +IsInRole(role: string) bool
        }
        class ClaimsIdentity {
            +Name: string?
            +AuthenticationType: string?
            IsAuthenticated: bool
        }
        class Claim {
            +Value: string
            +Type: string
            +Issuer: string
            +ValueType: string
        }
</div></div>

---

# Claim

<div class="columns"><div class="columns-left">

- a true statement
- key-name pair
  - claim type
  - claim value
- formalized names
  - namespace + name

</div><div class="columns-right">

```csharp
internal const string ClaimTypeNamespace =
  "http://schemas.microsoft.com/ws/2008/06/identity/claims";
 
public const string AuthenticationInstant =
  ClaimTypeNamespace + "/authenticationinstant";

public const string AuthenticationMethod =
  ClaimTypeNamespace + "/authenticationmethod";

public const string CookiePath =
  ClaimTypeNamespace + "/cookiepath";

public const string Dsa =
  ClaimTypeNamespace + "/dsa";

public const string Expiration =
  ClaimTypeNamespace + "/expiration";

public const string Expired =
  ClaimTypeNamespace + "/expired";

public const string GroupSid =
  ClaimTypeNamespace + "/groupsid";

public const string IsPersistent =
  ClaimTypeNamespace + "/ispersistent";

public const string Role =
  ClaimTypeNamespace + "/role";

public const string SerialNumber =
  ClaimTypeNamespace + "/serialnumber";

public const string UserData =
  ClaimTypeNamespace + "/userdata";

public const string Version =
  ClaimTypeNamespace + "/version";
```

</div></div>

---

<div class="columns"><div class="columns-left">

# AddAuthentication

- registers relevant types in DI
- configures authentication schemes
- we add JWT bearer auth
  - built-in
  - extremely customizable

</div><div class="columns-right">

```csharp
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme =
            JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new()
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("Security:TokenSecret"))),
            IssuerSigningKeyResolver = (_, _, _, parameters) => new[] { parameters.IssuerSigningKey },
            ValidateIssuer = false,
            ValidateAudience = false,
        };
    });
```

```csharp
[Authorize]
[HttpPost]
[ModelValidation]
public async Task<ActionResult<Item>> Post(CreateItemRequest request)
{
    var item = await _itemRepository.Add(request.ToItem());
    return CreatedAtAction(nameof(Get), new { id = item.Id }, item);
}
```

</div></div>

---

<div class="columns"><div class="columns-left">

# Implementing basic auth

- just for example
- not recommended
- basic auth handler is missing intentionally

</div><div class="columns-right">

```csharp
public class BasicHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IBasicValidator _userRepository;
    
    public BasicHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IBasicValidator userRepository)
        : base(options, logger, encoder, clock)
    { 
        _userRepository = userRepository;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var header = Request.Headers["Authorization"];
        if (header.Count == 0 || !((string)header).StartsWith(
                "Basic ", StringComparison.OrdinalIgnoreCase))
        {
            return AuthenticateResult.NoResult();
        }
        var headerValue = header.ToString()[7..].Trim();
        var credentialBytes = Convert.FromBase64String(headerValue);
        var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':');
        if (credentials.Length != 2)
        {
            return AuthenticateResult.Fail("Invalid Authorization header format.");
        }

        if (!_userRepository.Validate(credentials[0], credentials[1]))
        {
            return AuthenticateResult.Fail("Invalid username or password.");
        }

        var claims = new[] {
            new Claim(ClaimTypes.NameIdentifier, credentials[0]),
            new Claim(ClaimTypes.Name, credentials[1]),
        };

        var principal = new ClaimsPrincipal(
            new ClaimsIdentity(claims, Scheme.Name));
        return AuthenticateResult.Success(
            new AuthenticationTicket(principal, Scheme.Name));
    }
}
```

</div></div>

---

<div class="columns"><div class="columns-left">

# Chain of responsibility

- decouples sender from receivers
    - extensibility
    - testability
    - isolation

</div><div class="columns-right mermaid">

```mermaid
flowchart TD
    subgraph ASPNET ["ASP.NET Authorization"]
        A[("Requirements")]
        B[("Handler 1")]
        C[("Handler 2")]
        D[("AuthorizationResult")]
        A -->|foreach| B
        B -->|Not sure| C
        C -->|success/failure| D
    end
    subgraph  
        client[("Client")]
        h1[("Handler 1")]
        h2[("Handler 2")]
        h3[("Handler 3")]
        h4[("Handler 4")]
        client -->|Sends Request| h1
        h1 -->|Can't handle, pass on| h2
        h2 -->|Can't handle, pass on| h3
        h3 -->|Can handle, pass on| h4
        h4 -->|Can handle | client
    end
```

</div></div>

---

# Setup authorization

<div class="columns"><div class="columns-left">

- AddAuthorization
    - registers services
    - configuration

- UseAuthorization
    - Checks UseAuthorization
    - Adds a middleware

</div><div class="columns-right">

```csharp
var builder = WebApplication.CreateBuilder(args);

/*...*/

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

/*...*/

var app = builder.Build();

/*...*/

app.UseAuthentication();
app.UseAuthorization();
```

</div></div>

---

# AuthorizeAttribute

<div class="columns"><div class="columns-left">

- Default
    - checks if authentication succeeded
- AuthenticationSchemes
    - only considers given schemes
    - same as default
- Roles
    - checks claims principal
- Policy
    - checks policy requirements

</div><div class="columns-right">

```csharp
[HttpPost("none")]
public IEnumerable<IdentityResponse> None() => GetPrincipalResponse();

[Authorize]
[AllowAnonymous]
[HttpPost("anonymous")]
public IEnumerable<IdentityResponse> Anonymous() => GetPrincipalResponse();

[Authorize]
[HttpPost("default")]
public IEnumerable<IdentityResponse> Default() => GetPrincipalResponse();

[Authorize(AuthenticationSchemes = "foo,bar")]
[HttpPost("schemes")]
public IEnumerable<IdentityResponse> Schemes() => GetPrincipalResponse();

[Authorize(Roles = "Admin,Owner")]
[HttpPost("roles")]
public IEnumerable<IdentityResponse> Roles() => GetPrincipalResponse();

[Authorize(Policy = "MinimumAge")]
[HttpPost("age")]
public IEnumerable<IdentityResponse> Policy() => GetPrincipalResponse();
```

</div></div>

---

<div class="columns"><div class="columns-left">

# ClaimRequirement

- Keeps claim type
- Keeps claim value
- is handled by `ClaimHandler`

<br />

# ClaimHandler

- checks if user has required claim
    - either fails or succeeds

</div><div class="columns-right">

```csharp
public class ClaimRequirement : IAuthorizationRequirement
{
    public string ClaimType { get; }
    public string ClaimValue { get; }

    public ClaimRequirement(string claimType, string claimValue)
    {
        ClaimType = claimType;
        ClaimValue = claimValue;
    }
}

public class ClaimHandler : AuthorizationHandler<ClaimRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, ClaimRequirement requirement)
    {
        var value = context.User
            .FindFirst(c => c.Type == requirement.ClaimType)?.Value;
        if (value == null || !value.Equals(requirement.ClaimValue,
            StringComparison.OrdinalIgnoreCase))
        {
            context.Fail();
        }
        else
        {
            context.Succeed(requirement);
        }
        
        return Task.CompletedTask;
    }
}
```

</div></div>

---

<div class="columns"><div class="columns-left">

# ClaimPolicyProvider

- creates policies from policy names
- if not claims policy, uses `DefaultAuthorizationPolicyProvider`

</div><div class="columns-right">

```csharp
public class ClaimPolicyProvider : DefaultAuthorizationPolicyProvider
{
    public ClaimPolicyProvider(IOptions<AuthorizationOptions> options)
        : base(options)
    {
    }

    public override async Task<AuthorizationPolicy?> GetPolicyAsync(
        string policyName)
    {
        var isClaimsPolicy = policyName
            .StartsWith(AuthorizeClaimAttribute.PolicyPrefix, StringComparison.Ordinal)
        
        if (!isClaimsPolicy)
        {
            return await base.GetPolicyAsync(policyName);
        }

        var claim = policyName
            .Substring(AuthorizeClaimAttribute.PolicyPrefix.Length).Split("=");

        return new AuthorizationPolicyBuilder()
            .AddRequirements(new ClaimRequirement(claim[0], claim[1]))
            .Build();
    }
}
```

</div></div>

---

<div class="columns"><div class="columns-left">

# AuthorizeClaimAttribute

- gets key and value
- creates a policy name
    - needs to be deterministic

</div><div class="columns-right">

```csharp
public class AuthorizeClaimAttribute : AuthorizeAttribute
{
    public const string PolicyPrefix = "Claims_";

    public AuthorizeClaimAttribute(string claimType, string claimValue)
    {
        Policy = $"{PolicyPrefix}{claimType}={claimValue}";
    }
}
```

</div></div>

---


<div class="columns"><div class="columns-left">

# Usage

- register custom types
- use the new attribute
- use multiple to get "AND"

</div><div class="columns-right">

```csharp
builder.Services.AddSingleton<IAuthorizationHandler, ClaimHandler>();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, ClaimPolicyProvider>();
```

```csharp
[AuthorizeClaim("name", "Jarda")]
[AuthorizeClaim("purpose", "test")]
[HttpPost("claims")]
public IEnumerable<IdentityResponse> Claims() => GetPrincipalResponse();
```

</div></div>

---

# Exercise

- add JWT authentication to our TodoApp
- enforce authentication for all endpoints
- authorize POST, PUT and DELETE operations by "Owner" role

---

# Comfortability vs security

- expirations
- password constrains
- SFA vs 2FA vs MFA

---
# Thank you!
