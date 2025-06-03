# Chapter 6: Authentication & Authorization (JWT)

Welcome back to the HotelManagementAPI tutorial!

In the previous chapter, [Chapter 5: Database Access (Dapper)](05_database_access__dapper__.md), we learned how to use Dapper to talk to our database – fetching information like room details or user profiles, and saving new data. Now that we can access and manage our hotel's data, a critical question arises: how do we ensure that only the *right* people can access sensitive information or perform actions they are allowed to do?

We need a system for **security**. In the world of APIs, this involves two main concepts:

1.  **Authentication:** Verifying *who* a user is. It's the process of confirming your identity.
2.  **Authorization:** Determining *what* an authenticated user is allowed to do or access. It's about permissions.

In our `HotelManagementAPI`, we use **JWT (JSON Web Tokens)** as a key part of our Authentication and Authorization system.

## The Hotel Security Desk Analogy

Imagine our HotelManagementAPI is a real physical hotel.

*   **Authentication is like checking in at the front desk.** When you arrive, you provide proof of identity (like a reservation number and ID). The receptionist verifies who you are. Once confirmed, they give you a **key card**. This key card is your temporary digital ID for the duration of your stay.
*   **Authorization is like using that key card to open doors or access services.** Your basic key card might let you into your room and the lobby (public endpoints). But to enter the staff-only areas (administrator endpoints) or access special services (employee-only actions), you need a different key card or your existing one needs special permissions encoded on it. The system checks your key card each time you try to access a restricted area.

In our API:

*   Logging in successfully is **Authentication**.
*   The **JWT (JSON Web Token)** you receive is your "digital key card".
*   The **claims** inside the JWT (like your user ID and role) are the permissions encoded on the card.
*   Checking if your token allows you to access a specific API endpoint or perform an action is **Authorization**.

## Authentication: Getting Your Digital ID Card (JWT)

The first step is proving who you are. This typically happens at a login endpoint.

In our project, the public `TatCaTruyCapController` handles login requests because anyone can log in.

Let's look at a simplified version of the `DangNhap` (Login) method in `Controllers/TatCaTruyCapController.cs`:

```csharp
// Inside TatCaTruyCapController.cs (simplified)
[HttpPost("dangnhap")] // Endpoint for login via POST
public async Task<ActionResult<string>> DangNhap([FromBody] LoginDTO login) // Takes login info from request body
{
    // 1. Use Dapper to find the user by username (from Chapter 5)
    const string query = "SELECT * FROM NguoiDung WHERE TenTaiKhoan = @TenTaiKhoan";
    var nguoiDung = await _db.QueryFirstOrDefaultAsync<NguoiDungDTO>(query, new { login.TenTaiKhoan });

    // If user not found
    if (nguoiDung == null)
        return Unauthorized(new { Message = "Tên tài khoản hoặc mật khẩu không đúng 1." }); // HTTP 401

    // 2. Verify the password (using BCrypt library for secure hashing)
    bool isValid = BCrypt.Net.BCrypt.Verify(login.MatKhau, nguoiDung.MatKhau);
    if (!isValid)
        return Unauthorized(new { Message = "Tên tài khoản hoặc mật khẩu không đúng 2." }); // HTTP 401

    // 3. Password is correct! Now generate the JWT token (the "key card").
    var secretKey = _config["Jwt:SecretKey"]; // Get secret key from configuration (Chapter 8)
    var issuer = _config["Jwt:Issuer"];       // Get issuer from configuration (Chapter 8)
    var audience = _config["Jwt:Audience"];   // Get audience from configuration (Chapter 8)

    // Need the Model object for token generation, not DTO
     var nguoiDungModel = new HotelManagementAPI.Models.NguoiDung
            {
                MaNguoiDung = nguoiDung.MaNguoiDung,
                Vaitro = nguoiDung.Vaitro, // Crucially, include the Role
                Email = nguoiDung.Email,
                // ... other properties as needed for claims
            };

    // Call a helper method to generate the token
    var token = JwtHelper.GenerateJwtToken(nguoiDungModel, secretKey, issuer, audience);

    // 4. Return the token to the client
    return Ok(new { Token = token }); // HTTP 200 OK with token in the response body
}
```

This method:
*   Receives login details (`TenTaiKhoan`, `MatKhau`) as a `LoginDTO` ([Chapter 1](01_data_transfer_objects__dtos__.md)).
*   Uses Dapper ([Chapter 5](05_database_access__dapper__.md)) to find the user in the database.
*   Securely verifies the provided password against the stored hash.
*   If login is successful, it calls `JwtHelper.GenerateJwtToken` to create the token.
*   Returns an HTTP 200 OK response containing the generated token. If login fails, it returns an HTTP 401 Unauthorized response.

### Generating the JWT

The `JwtHelper.GenerateJwtToken` method (found in `Helper/HelperJWT.cs`) is where the token is actually created. It takes the user's data and uses it to embed **claims** within the token. Claims are pieces of information about the user (who they are, what permissions they have).

Here's a simplified look at the token generation:

```csharp
// Inside Helper/JwtHelper.cs (simplified)
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using HotelManagementAPI.Models; // Need the Model type

public static class JwtHelper
{
    public static string GenerateJwtToken(NguoiDung nguoiDung, string secretKey, string issuer, string audience)
    {
        // Define the claims (information about the user) to include in the token
        var claims = new[]
        {
            // 'sub' (Subject): Usually the user's unique ID
            new Claim(JwtRegisteredClaimNames.Sub, nguoiDung.MaNguoiDung.ToString()),
            // 'Vaitro' (Role): The user's role is crucial for authorization
            new Claim("Vaitro", nguoiDung.Vaitro),
            // 'email': User's email
            new Claim(JwtRegisteredClaimNames.Email, nguoiDung.Email),
            // 'jti' (JWT ID): A unique ID for this specific token instance
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            // You could add other claims here if needed (e.g., username, display name)
        };

        // Create the signing key using the secret key (must be kept secret!)
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        // Create signing credentials
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Create the token object
        var token = new JwtSecurityToken(
            issuer: issuer,         // Who issued the token
            audience: audience,     // Who the token is for
            claims: claims,         // The claims we defined
            expires: DateTime.Now.AddHours(1), // Token expiration time (e.g., 1 hour)
            signingCredentials: creds // How the token is signed
        );

        // Serialize the token object into a string (the JWT)
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
```

Key takeaways here:
*   The token is created using `JwtSecurityToken`.
*   It includes **claims** like `sub` (user ID) and importantly, `Vaitro` (Role). These claims will be used later for authorization.
*   It includes an expiration time (`expires`). Tokens are temporary!
*   It is **signed** using a `secretKey`. This secret key is crucial because it allows the API to verify that the token hasn't been tampered with after it was issued. **This key must be kept secret on the server side!**
*   The method returns the token as a compact string, which is what the client receives.

### What the Client Does with the Token

Once the client (e.g., a web browser or mobile app) receives the JWT from the `/api/TatCaTruyCap/dangnhap` endpoint, it should store it securely (e.g., in memory, a secure cookie, or local storage).

For all subsequent requests to protected endpoints (endpoints that require the user to be logged in), the client needs to include this token in the request's **Authorization header**, typically in the format `Bearer [your_token_string]`.

Example HTTP Header:

```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

This is how the API receives the "digital key card" back with each request.

## Behind the Scenes: Verifying the Token (Authentication Middleware)

When the API receives a request with a JWT in the `Authorization: Bearer` header, ASP.NET Core's built-in **Authentication Middleware** steps in *before* the request even reaches your controller action method.

This middleware is configured in `Program.cs`:

```csharp
// File: Program.cs (simplified)
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
// ... other using statements ...

// ... code before builder.Build() ...

// Cấu hình JWT
builder.Services.AddAuthentication("Bearer") // 1. Add authentication services, default scheme is "Bearer"
    .AddJwtBearer(options => // 2. Configure the JWT Bearer handler
    {
        options.TokenValidationParameters = new TokenValidationParameters // 3. Define how to validate the token
        {
            ValidateIssuer = true, // Check if the issuer is valid
            ValidateAudience = true, // Check if the audience is valid
            ValidateLifetime = true, // Check if the token has expired
            ValidateIssuerSigningKey = true, // Check if the signature is valid using the secret key

            // These must match the values used when generating the token
            ValidIssuer = "your-issuer",
            ValidAudience = "your-audience",
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("your_super_secret_key_1234567890") // **MUST MATCH THE KEY USED FOR SIGNING**
            ),

            // Map standard JWT claims to .NET identity claims
            NameClaimType = "sub",       // Use 'sub' claim as the user's Name (Identifier)
            RoleClaimType = "Vaitro"     // Use 'Vaitro' claim as the user's Role
        };

        // Optional: Add events for custom handling (e.g., checking if token is missing)
         options.Events = new JwtBearerEvents { /* ... */ };
    });

// ... other services ...

var app = builder.Build();

// ... middleware setup before UseAuthorization() ...

// 4. Add the Authentication Middleware to the request pipeline
app.UseAuthentication();
// 5. Add custom middleware for path-based authorization and token blacklist (more below)
app.UseMiddleware<RoleMiddleware>();
// 6. Add the Authorization Middleware (checks [Authorize] attributes)
app.UseAuthorization();

// ... rest of middleware and app setup ...
```

Explanation:
1.  `AddAuthentication("Bearer")`: Registers the authentication service. "Bearer" is the standard scheme name for JWTs sent in the `Authorization: Bearer` header.
2.  `AddJwtBearer(options => ...)`: Configures the specific handler for JWT tokens.
3.  `TokenValidationParameters`: This is the core of verification. It tells the handler how to validate the token. It *must* match the secret key, issuer, and audience used during token generation. `ValidateLifetime = true` is crucial to automatically reject expired tokens. `NameClaimType` and `RoleClaimType` map the custom `sub` and `Vaitro` claims from our token to the standard `ClaimTypes.NameIdentifier` and `ClaimTypes.Role` that ASP.NET Core's security system understands.
4.  `app.UseAuthentication()`: Adds the middleware to the request processing pipeline. When a request arrives with a token, this middleware will try to validate it based on the configured `TokenValidationParameters`. If valid, it creates a `ClaimsPrincipal` object representing the authenticated user and attaches it to the current `HttpContext.User`.
5.  `app.UseMiddleware<RoleMiddleware>()`: Adds custom middleware for extra authorization checks (explained below).
6.  `app.UseAuthorization()`: Adds the middleware that checks for `[Authorize]` attributes on controllers and actions.

**Authentication Flow:**

```mermaid
sequenceDiagram
    participant Client as Browser/App
    participant ApiFramework as ASP.NET Core Framework
    participant JwtMiddleware as Authentication Middleware
    participant JwtHelper as JwtHelper
    participant ClaimsPrincipal as ClaimsPrincipal Object

    Client->>ApiFramework: HTTP Request with<br/>'Authorization: Bearer [token]' header
    ApiFramework->>JwtMiddleware: Process request<br/>Note over JwtMiddleware: Extract token from header
    JwtMiddleware->>JwtHelper: Validate token signature,<br/>issuer, audience, lifetime
    JwtHelper-->>JwtMiddleware: Validation Result (Success/Fail)
    alt Validation Success
        JwtMiddleware->>ClaimsPrincipal: Create ClaimsPrincipal<br/>with claims from token (e.g., 'sub', 'Vaitro')
        ClaimsPrincipal-->>ApiFramework: Attach ClaimsPrincipal<br/>to HttpContext.User
        ApiFramework->>ApiFramework: Continue processing request<br/>(Next: Custom Middleware, Authorization Middleware)
    else Validation Fail
        JwtMiddleware-->>ApiFramework: Stop processing<br/>Set Status Code 401 Unauthorized
        ApiFramework-->>Client: Send HTTP 401 Unauthorized Response
    end
```

If the token validation succeeds, the authenticated user's identity (represented by the `ClaimsPrincipal` object with its claims) becomes available in `HttpContext.User` for the rest of the request pipeline. If validation fails (wrong signature, expired, invalid issuer/audience), the request is stopped immediately with a 401 Unauthorized response.

## Authorization: What Are You Allowed To Do?

Once the user is authenticated (the framework knows *who* they are based on the valid token and its claims), the next step is **Authorization** – checking *what* they are allowed to do.

Authorization in ASP.NET Core is often handled using the `[Authorize]` attribute on controllers or individual action methods.

### Basic `[Authorize]`

Placing `[Authorize]` on a controller or method means that **only authenticated users** can access it. The framework simply checks if `HttpContext.User` has an identity associated with it (meaning the Authentication middleware successfully validated a token).

Example from `Controllers/XacThucController.cs`:

```csharp
// Inside XacThucController.cs
[ApiController]
[Authorize] // This controller requires an authenticated user
[Route("api/xacthuc")]
public class XacThucController : ControllerBase
{
    // ... constructor and methods ...

    // This method can only be accessed by someone with a valid token
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        // Access user information from the token claims via User object
        var maNguoiDung = User.FindFirstValue("sub"); // Use the 'sub' claim (User ID)
        // ... logic to get user profile using maNguoiDung ...
        return Ok(...);
    }
}
```

Any request to `/api/xacthuc/profile` *without* a valid JWT will be blocked by the `UseAuthorization()` middleware with a 401 Unauthorized response. Requests *with* a valid token will proceed to the `GetProfile` method, where you can then access the user's details from the token via the `User` property (a `ClaimsPrincipal`).

### Role-Based Authorization (`[Authorize(Roles = "RoleName")]`)

A more specific type of authorization is based on **Roles**. In our application, users have a `Vaitro` (Role) like "KhachHang", "NhanVien", or "QuanTriVien". We embedded this role as a claim in the JWT.

You can restrict access to controllers or methods based on these roles using `[Authorize(Roles = "RoleName")]`.

Example from `Controllers/QuanTriVien/QuanTriVienPhongController.cs`:

```csharp
// Inside QuanTriVienPhongController.cs
[ApiController]
[Route("api/quantrivien/phong")]
[Authorize(Roles = "QuanTriVien")] // Only users with the "QuanTriVien" role can access ANY endpoint in this controller
public class QuanTriVienPhongController : ControllerBase
{
    // ... constructor ...

    /// <summary>
    /// Thêm phòng mới.
    /// </summary>
    [HttpPost] // This method requires the "QuanTriVien" role (because it's on the controller)
    public async Task<IActionResult> ThemPhong([FromBody] PhongDetailsDTO phongDetailsDTO)
    {
        // ... logic to add a room ...
        return Ok(...);
    }

    /// <summary>
    /// Sửa thông tin phòng.
    /// </summary>
    [HttpPut("{maPhong}")] // This method also requires the "QuanTriVien" role
    public async Task<IActionResult> CapNhatPhong(string maPhong, [FromBody] PhongDetailsDTO phongDetailsDTO)
    {
       // ... logic to update a room ...
       return Ok(...);
    }
}
```

Now, only users whose JWT contains a `Vaitro` claim with the value "QuanTriVien" will be allowed to access any endpoint starting with `/api/quantrivien/phong`. If an authenticated user with the role "KhachHang" tries to access `/api/quantrivien/phong`, the `UseAuthorization()` middleware will stop the request with a 403 Forbidden response.

You can also apply `[Authorize(Roles = "...")]` to individual action methods if different methods within the same controller require different roles.

Example from `Controllers/NhanVienController.cs`:

```csharp
// Inside NhanVienController.cs
[ApiController]
[Route("api/nhanvien")]
[Authorize(Roles = "NhanVien")] // Default for this controller is "NhanVien" role
public class NhanVienController : ControllerBase
{
    // ... methods requiring "NhanVien" role ...

    /// <summary>
    /// Thêm bài viết mới (nhân viên).
    /// </summary>
    [HttpPost("baiviet")]
    public async Task<IActionResult> ThemBaiViet([FromForm] NhanVienThemBaiVietDTO dto, IFormFile? file)
    {
        // This method requires the "NhanVien" role
        // ... logic ...
    }

    /// <summary>
    /// Cập nhật thông tin phòng.
    /// </summary>
    /// <param name="maPhong">Mã phòng cần cập nhật</param>
    /// <param name="phongDetailsDTO">Thông tin phòng mới</param>
    [HttpPut("{maPhong}")]
    // NOTE: Even though the controller default is "NhanVien",
    // specific methods might have different or combined requirements.
    // In this project's code, this method is under NhanVienController,
    // implying it's intended for NhanVien. If QuanTriVien also needs access,
    // you might change the controller attribute or apply attribute here:
    // [Authorize(Roles = "NhanVien,QuanTriVien")]
    public async Task<IActionResult> CapNhatPhong(string maPhong, [FromBody] PhongDetailsDTO phongDetailsDTO)
    {
        // This method requires the "NhanVien" role
        // ... logic ...
    }
}
```
Here, the `[Authorize(Roles = "NhanVien")]` on the controller applies to all action methods within `NhanVienController` by default. A user needs the "NhanVien" role to access endpoints like `/api/nhanvien/baiviet` (POST) or `/api/nhanvien/phong/{maPhong}` (PUT).

### Allowing Anonymous Access (`[AllowAnonymous]`)

Some endpoints, like the login endpoint itself or viewing public information (like available rooms), should not require authentication at all. You mark these with the `[AllowAnonymous]` attribute.

Example from `Controllers/TatCaTruyCapController.cs`:

```csharp
// Inside TatCaTruyCapController.cs
[ApiController]
[AllowAnonymous] // This controller allows ANYONE to access its endpoints
[Route("api/TatCaTruyCap")]
public class TatCaTruyCapController : ControllerBase
{
    // ... constructor ...

    /// <summary>
    /// Đăng nhập hệ thống.
    /// </summary>
    [HttpPost("dangnhap")] // This specific method is also implicitly AllowAnonymous because of the controller attribute
    public async Task<ActionResult<string>> DangNhap([FromBody] LoginDTO login) { /* ... */ }

    /// <summary>
    /// Lấy tất cả phòng (chi tiết).
    /// </summary>
    [HttpGet("phong")] // This method is also implicitly AllowAnonymous
    public async Task<ActionResult<IEnumerable<PhongDetailsDTO>>> GetAllPhong() { /* ... */ }

    // ... other public methods like dangky, tiennghi, dichvu ...
}
```
The `[AllowAnonymous]` attribute overrides any `[Authorize]` attribute that might be present on the controller or globally.

### Policy-Based Authorization (`[Authorize(Policy = "PolicyName")]`)

For more complex authorization rules (e.g., requiring multiple claims, checking specific claim values, or custom logic), you can define **Policies**. Policies are configured in `Program.cs` and then referenced by name in the `[Authorize]` attribute.

Example policy configuration in `Program.cs`:

```csharp
// File: Program.cs (simplified)
// ... code before builder.Build() ...

builder.Services.AddAuthorization(options =>
{
    // Define a policy named "QuanTriVienPolicy"
    options.AddPolicy("QuanTriVienPolicy", policy =>
        // This policy requires the user to have a claim
        // with the type "Vaitro" AND the value "QuanTriVien".
        policy.RequireClaim("Vaitro", "QuanTriVien"));

    // Define a policy named "NhanVienPolicy"
    options.AddPolicy("NhanVienPolicy", policy =>
        // This policy requires the "Vaitro" claim to be EITHER "NhanVien" OR "QuanTriVien"
        policy.RequireClaim("Vaitro", "NhanVien", "QuanTriVien"));

    // Define a policy named "KhachHangPolicy"
    options.AddPolicy("KhachHangPolicy", policy =>
        // This policy requires the "Vaitro" claim to be ANY of "KhachHang", "NhanVien", "QuanTriVien"
        policy.RequireClaim("Vaitro", "KhachHang","NhanVien", "QuanTriVien"));
});

// ... rest of setup ...
```

Now you can use these policies with `[Authorize(Policy = "PolicyName")]`:

Example from `Controllers/QuanTriVien/QuanTriVienController.cs`:

```csharp
// Inside QuanTriVienController.cs
[ApiController]
[Route("api/quantrivien")]
[Authorize(Policy = "QuanTriVienPolicy")] // Uses the policy defined in Program.cs
public class QuanTriVienController : ControllerBase
{
    // This requires the user to have Vaitro="QuanTriVien" according to the policy
    [HttpGet("dashboard")]
    public IActionResult GetDashboard()
    {
        return Ok(new { Message = "Đây là dashboard của Quản trị viên." });
    }

    // You can override or add policies/roles on individual methods
    // For example, this method *also* requires the policy (redundant here as it's on the controller)
    // [HttpPost("dangky")]
    // [Authorize(Policy = "QuanTriVienPolicy")]
    // public async Task<ActionResult<NguoiDungDTO>> DangKyQuanTriVien([FromBody] NguoiDungDTO nguoiDung) { ... }
}
```
Using policies makes authorization rules more descriptive and reusable. The `[Authorize(Roles = "...")]` syntax is actually a shortcut for a simple policy that requires a specific role claim. In this project, policies are used on `QuanTriVienController`, `NhanVienController`, and `KhachHangController` for clarity and potentially more complex rules in the future.

### Custom Authorization Middleware (`RoleMiddleware`)

While `[Authorize]` attributes and policies are powerful, they operate at the level of *controllers* and *action methods*. What if you want an extra layer of path-based role checking, perhaps for stricter security or logging?

The `HotelManagementAPI` project includes a custom `RoleMiddleware` (`RoleMiddleware/RoleMiddleware.cs`) which runs *after* authentication but *before* the Authorization middleware and controller execution (as seen in the `Program.cs` middleware order: `UseAuthentication()`, `UseMiddleware<RoleMiddleware>()`, `UseAuthorization()`).

Here's a simplified look at its `InvokeAsync` method:

```csharp
// Inside RoleMiddleware/RoleMiddleware.cs (simplified)
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Security.Claims; // Needed to access claims
using System.Threading.Tasks;

public class RoleMiddleware
{
    private readonly RequestDelegate _next;

    public RoleMiddleware(RequestDelegate next)
    {
        _next = next; // This is the next middleware in the pipeline
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // 1. Skip check if the endpoint allows anonymous access ([AllowAnonymous])
        var endpoint = context.GetEndpoint();
        if (endpoint?.Metadata?.GetMetadata<AllowAnonymousAttribute>() != null)
        {
            await _next(context); // Go to the next middleware immediately
            return;
        }

        // 2. Get the user's role from the ClaimsPrincipal attached by Authentication Middleware
        var userRole = context.User.Claims.FirstOrDefault(c => c.Type == "Vaitro")?.Value;

        // 3. If no role claim found (meaning not authenticated), return 401
        if (string.IsNullOrEmpty(userRole))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Bạn cần đăng nhập để truy cập tài nguyên này.");
            return;
        }

        // 4. Perform path-based role checks
        // Check if path starts with /api/quantrivien AND user is NOT QuanTriVien
        if (context.Request.Path.StartsWithSegments("/api/quantrivien") && userRole != "QuanTriVien")
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("Bạn không có quyền truy cập vào tài nguyên này.");
            return;
        }
        // Check if path starts with /api/nhanvien AND user is NOT NhanVien or QuanTriVien
        else if (context.Request.Path.StartsWithSegments("/api/nhanvien") && userRole != "NhanVien" && userRole != "QuanTriVien")
        {
             context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("Bạn không có quyền truy cập vào tài nguyên này.");
            return;
        }
        // Check if path starts with /api/khachhang AND user is NOT KhachHang, NhanVien, or QuanTriVien
         else if (context.Request.Path.StartsWithSegments("/api/khachhang") && userRole != "KhachHang" && userRole != "NhanVien" && userRole != "QuanTriVien")
        {
             context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("Bạn không có quyền truy cập vào tài nguyên này.");
            return;
        }

        // 5. If checks pass, proceed to the next middleware (Authorization middleware, then controller)
        await _next(context);
    }
}
```

This middleware adds an extra layer of security. Even if an endpoint is marked `[Authorize(Roles = "KhachHang")]`, this middleware will *first* check if the path starts with `/api/khachhang`. If it does, it verifies the `Vaitro` claim is one of the allowed roles ("KhachHang", "NhanVien", or "QuanTriVien"). If this path-based check fails, it returns 403 Forbidden *before* the standard `[Authorize]` attribute check even happens. This can be useful for broad access control based on URL segments.

Note the `ClaimsPrincipalExtensions` class in the same file provides helper methods like `GetUserRole()` and `GetUserSub()` to easily access claims in controllers:

```csharp
// Inside Controllers/XacThucController.cs
// ...
[HttpGet("profile")]
public async Task<IActionResult> GetProfile()
{
    // Using the extension method for clarity
    var maNguoiDung = User.GetUserSub(); // Equivalent to User.FindFirstValue("sub")

    // ... logic ...
}
```

## Putting It All Together: The Request Pipeline with Security

Here's how a request to a protected endpoint flows through the security mechanisms we've discussed:

```mermaid
sequenceDiagram
    participant Client as Browser/App
    participant ApiFramework as ASP.NET Core
    participant AuthMiddleware as Authentication Middleware
    participant CustomMiddleware as RoleMiddleware
    participant AuthzMiddleware as Authorization Middleware
    participant ApiController as API Controller

    Client->>ApiFramework: HTTP Request with 'Authorization: Bearer [token]'
    ApiFramework->>AuthMiddleware: Pass request to Authentication
    AuthMiddleware->>AuthMiddleware: Validate JWT (signature, expiration, etc.)<br/>using config (Program.cs)
    alt Token Invalid
        AuthMiddleware-->>Client: Return 401 Unauthorized<br/>Stop Processing
    else Token Valid
        AuthMiddleware->>ApiFramework: Attach validated user (ClaimsPrincipal) to HttpContext
        ApiFramework->>CustomMiddleware: Pass request to RoleMiddleware
        CustomMiddleware->>CustomMiddleware: Check [AllowAnonymous]<br/>Check Path vs User Role (e.g., /api/quantrivien vs 'Vaitro' claim)
        alt Path/Role Mismatch
            CustomMiddleware-->>Client: Return 403 Forbidden<br/>Stop Processing
        else Path/Role Match or Anonymous
            CustomMiddleware->>AuthzMiddleware: Pass request to Authorization
            AuthzMiddleware->>AuthzMiddleware: Check [Authorize] attributes<br/>on Controller/Action (Roles/Policies)
            alt User Lacks Required Role/Policy
                AuthzMiddleware-->>Client: Return 403 Forbidden<br/>Stop Processing
            else User Has Required Role/Policy
                AuthzMiddleware->>ApiController: Allow request to reach Controller Action Method
                ApiController->>ApiController: Execute business logic<br/>(using DbContext, Dapper, etc.)
                ApiController-->>Client: Return API Response (2xx, 4xx, 5xx)
            end
        end
    end
```

This flow shows how Authentication happens first, followed by the custom `RoleMiddleware`, and then the standard Authorization middleware, ensuring multiple layers of checks before your controller code runs.

## Logout: Revoking the Key Card

Since JWTs are self-contained and typically stateless, simply deleting the token on the client side isn't enough to invalidate it on the server side before it expires. If someone steals an active token, they could still use it.

To truly "logout" a user and invalidate their token immediately, the server needs a way to track tokens that are no longer valid. A common approach is using a **Token Blacklist**.

Our project implements a simple token blacklist using an in-memory cache.

Look at `XacThucController.cs`:

```csharp
// Inside XacThucController.cs
private readonly IMemoryCache _cache; // Injected memory cache service

public XacThucController(IMemoryCache cache, /* ... other dependencies */)
{
    _cache = cache;
    // ...
}

/// <summary>
/// Đăng xuất người dùng dựa trên vai trò từ token.
/// </summary>
[HttpPost("logout")]
[Authorize] // Only authenticated users can logout
public IActionResult Logout()
{
    // Get the token string from the Authorization header
    var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

    if (string.IsNullOrEmpty(token))
        return BadRequest(new { Message = "Token không hợp lệ." });

    // Get the token's original expiration time (assuming it was valid)
    // In a real app, you'd decode the token or get claims before adding
    // For simplicity, we'll assume the cache expiration matches token expiration (1 hour)
    var expirationTime = DateTime.UtcNow.AddHours(1); // **Matches token lifetime in JwtHelper/Program.cs**

    // Add the token to the in-memory blacklist cache
    _cache.Set(token, true, expirationTime); // Store the token string with its original expiry

    return Ok(new { Message = "Đăng xuất thành công. Token đã bị thu hồi." });
}
```

When a user hits the `/api/xacthuc/logout` endpoint with their token, the token string is extracted and added to the `IMemoryCache` with an expiration time matching the token's original expiry.

To enforce this blacklist, a custom `TokenBlacklistMiddleware` is added in `Program.cs` *before* `UseAuthentication()`:

```csharp
// File: Program.cs (simplified)
// ...
builder.Services.AddMemoryCache(); // Register IMemoryCache

// ...
var app = builder.Build();

// ... middleware before UseAuthentication() ...

// Add the token blacklist middleware FIRST
app.UseMiddleware<TokenBlacklistMiddleware>();
// Then standard Authentication
app.UseAuthentication();
// Then custom RoleMiddleware
app.UseMiddleware<RoleMiddleware>();
// Then standard Authorization
app.UseAuthorization();

// ...
```

Here's the `TokenBlacklistMiddleware.InvokeAsync`:

```csharp
// Inside RoleMiddleware/RoleMiddleware.cs (simplified)
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory; // Needed for cache
using System.Threading.Tasks;

public class TokenBlacklistMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IMemoryCache _cache; // Injected memory cache

    public TokenBlacklistMiddleware(RequestDelegate next, IMemoryCache cache)
    {
        _next = next;
        _cache = cache;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Get the token from the header
        var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

        // If a token exists and it's found in the cache (blacklist)
        if (!string.IsNullOrEmpty(token) && _cache.TryGetValue(token, out _))
        {
            // Token is blacklisted, return Unauthorized
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Token đã bị thu hồi. Vui lòng đăng nhập lại.");
            return; // Stop processing the request
        }

        // If token is not blacklisted or no token is present, proceed
        await _next(context);
    }
}
```
This middleware runs very early. It checks if the incoming token is in the blacklist cache. If it is, the request is rejected immediately with a 401 Unauthorized, effectively revoking the token's access before it even gets to the Authentication middleware. Tokens automatically expire from the cache when their blacklisted time is up, cleaning up the list.

## Summary

In this chapter, we tackled the essential concepts of **Authentication** (verifying who you are) and **Authorization** (what you can do) in our `HotelManagementAPI`.

We learned that:
*   **JWT (JSON Web Tokens)** act as digital key cards, issued upon successful login (Authentication).
*   These tokens contain **claims** like user ID (`sub`) and **Role** (`Vaitro`) which determine permissions.
*   The client sends the JWT back with subsequent requests in the `Authorization: Bearer` header.
*   ASP.NET Core's built-in Authentication middleware (`UseAuthentication()`) validates the token's integrity and lifetime based on configuration in `Program.cs`.
*   Authorization middleware (`UseAuthorization()`) uses attributes like `[Authorize]`, `[Authorize(Roles="...")]`, and `[Authorize(Policy="...")]` on controllers/actions to check if the authenticated user's claims (especially Role) grant access.
*   `[AllowAnonymous]` allows public access to specific endpoints.
*   A custom `RoleMiddleware` provides an extra layer of path-based role checking.
*   Token revocation on logout is handled by adding the token to an in-memory blacklist (`TokenBlacklistMiddleware`).

Understanding these concepts is vital for building a secure API that protects its resources.

We've now seen how requests arrive (Controllers), how data is structured (DTOs, Models), how data is validated (FluentValidation), how we talk to the database (Dapper), and how we secure access (Authentication & Authorization with JWT).

In the next chapter, we'll take a closer look at **Middleware** itself, including the `RoleMiddleware` and `TokenBlacklistMiddleware` we just touched upon, and understand how middleware forms a pipeline that processes requests step-by-step.

Ready to dive deeper into the request processing pipeline? Let's go to [Chapter 7: Request Middleware](07_request_middleware_.md)!

---

<sub><sup>Generated by [AI Codebase Knowledge Builder](https://github.com/The-Pocket/Tutorial-Codebase-Knowledge).</sup></sub> <sub><sup>**References**: [[1]](https://github.com/Persinus/HotelManagementAPI/blob/36142c7f0bf93973f50c4408eb5a21a7f994eea2/Controllers/KhachHangController.cs), [[2]](https://github.com/Persinus/HotelManagementAPI/blob/36142c7f0bf93973f50c4408eb5a21a7f994eea2/Controllers/NhanVienController.cs), [[3]](https://github.com/Persinus/HotelManagementAPI/blob/36142c7f0bf93973f50c4408eb5a21a7f994eea2/Controllers/QuanTriVien/QuanTriVienController.cs), [[4]](https://github.com/Persinus/HotelManagementAPI/blob/36142c7f0bf93973f50c4408eb5a21a7f994eea2/Controllers/QuanTriVien/QuanTriVienPhongController.cs), [[5]](https://github.com/Persinus/HotelManagementAPI/blob/36142c7f0bf93973f50c4408eb5a21a7f994eea2/Controllers/QuanTriVien/QuanTriVien_HeThongController.cs), [[6]](https://github.com/Persinus/HotelManagementAPI/blob/36142c7f0bf93973f50c4408eb5a21a7f994eea2/Controllers/QuanTriVien/QuanTriVien_QuanLyDichVuQTVController.cs), [[7]](https://github.com/Persinus/HotelManagementAPI/blob/36142c7f0bf93973f50c4408eb5a21a7f994eea2/Controllers/TatCaTruyCapController.cs), [[8]](https://github.com/Persinus/HotelManagementAPI/blob/36142c7f0bf93973f50c4408eb5a21a7f994eea2/Controllers/XacThucController.cs), [[9]](https://github.com/Persinus/HotelManagementAPI/blob/36142c7f0bf93973f50c4408eb5a21a7f994eea2/Helper/HelperJWT.cs), [[10]](https://github.com/Persinus/HotelManagementAPI/blob/36142c7f0bf93973f50c4408eb5a21a7f994eea2/Program.cs), [[11]](https://github.com/Persinus/HotelManagementAPI/blob/36142c7f0bf93973f50c4408eb5a21a7f994eea2/RoleMiddleware/RoleMiddleware.cs)</sup></sub>