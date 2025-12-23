# Blog Application
## Comprehensive Technical Presentation

---

## Slide 1: Title Slide
# **Blog Application**
### A Modern Full-Stack Blogging Platform

**Built With:**
- ASP.NET Core Web API
- Clean Architecture
- CQRS + MediatR Pattern
- MySQL + MongoDB Hybrid Database

**GitHub:** https://github.com/golamyusuf/Blog-App

---

## Slide 2: Project Overview

### **What is This Application?**
A full-featured blog platform that enables users to:
- ? Create, read, update, and delete blog posts
- ? User authentication & authorization (JWT)
- ? Category-based organization
- ? Media attachments support
- ? Search and filtering capabilities
- ? Role-based access control (Admin/User)

### **Key Features**
- Paginated blog listings
- Draft & published post management
- View count tracking
- Tag-based categorization
- Rich media support (images, videos)

---

## Slide 3: Architecture Overview

### **Clean Architecture Pattern**

```
???????????????????????????????????????????????????????
?                    API Layer                        ?
?  (Controllers, Middleware, Program.cs)              ?
?  - HTTP Request/Response                            ?
?  - Authentication/Authorization                     ?
?  - Swagger Documentation                            ?
???????????????????????????????????????????????????????
                 ?
???????????????????????????????????????????????????????
?               Application Layer                     ?
?  (Commands, Queries, Handlers, DTOs, Validators)    ?
?  - Business Logic                                   ?
?  - CQRS Implementation                              ?
?  - MediatR Pattern                                  ?
?  - FluentValidation                                 ?
???????????????????????????????????????????????????????
                 ?
???????????????????????????????????????????????????????
?                Domain Layer                         ?
?  (Entities, Interfaces)                             ?
?  - Core Business Entities                           ?
?  - Domain Interfaces                                ?
?  - No External Dependencies                         ?
???????????????????????????????????????????????????????
                 ?
???????????????????????????????????????????????????????
?             Infrastructure Layer                    ?
?  (Data, Repositories, Services, Migrations)         ?
?  - Database Contexts (MySQL + MongoDB)              ?
?  - Repository Implementations                       ?
?  - External Service Integrations                    ?
???????????????????????????????????????????????????????
```

---

## Slide 4: Technology Stack

### **Backend Technologies**
| Technology | Purpose | Version |
|------------|---------|---------|
| **ASP.NET Core** | Web API Framework | .NET 9.0/10.0 |
| **Entity Framework Core** | ORM for MySQL | Latest |
| **MongoDB Driver** | NoSQL Document Storage | Latest |
| **MediatR** | CQRS Implementation | Latest |
| **FluentValidation** | Input Validation | Latest |
| **JWT Bearer** | Authentication | Latest |
| **Swagger/OpenAPI** | API Documentation | Latest |

### **Database Architecture**
- **MySQL**: User accounts, roles, categories (relational data)
- **MongoDB**: Blog posts, comments (document storage)
- **Hybrid Approach**: Best of both worlds!

---

## Slide 5: Database Schema

### **MySQL Schema (Relational)**

```sql
-- Users Table
Users
??? Id (PK)
??? Username (Unique)
??? Email (Unique)
??? PasswordHash
??? FirstName
??? LastName
??? ProfileImageUrl
??? IsActive
??? LastLoginAt
??? CreatedAt
??? UpdatedAt

-- Roles Table
Roles
??? Id (PK)
??? Name (Admin/User)
??? Description
??? CreatedAt
??? UpdatedAt

-- UserRoles (Many-to-Many)
UserRoles
??? UserId (FK ? Users)
??? RoleId (FK ? Roles)
??? AssignedAt

-- Categories Table
Categories
??? Id (PK)
??? Name
??? Description
??? Slug (URL-friendly)
??? CreatedByUserId (FK ? Users)
??? IsActive
??? CreatedAt
??? UpdatedAt
```

---

## Slide 6: MongoDB Schema

### **Blog Collection (Document-Oriented)**

```javascript
{
  "_id": ObjectId("507f1f77bcf86cd799439011"),
  "userId": 123,
  "username": "john_doe",              // Denormalized
  "categoryId": 5,
  "categoryName": "Technology",         // Denormalized
  "title": "Understanding CQRS Pattern",
  "content": "Full blog post content...",
  "summary": "Brief overview of CQRS",
  "tags": ["cqrs", "architecture", "design-patterns"],
  "mediaItems": [
    {
      "url": "https://example.com/image1.jpg",
      "type": "image",
      "caption": "Architecture Diagram",
      "order": 1
    }
  ],
  "viewCount": 542,
  "isPublished": true,
  "createdAt": ISODate("2024-01-15T10:30:00Z"),
  "updatedAt": ISODate("2024-01-16T14:22:00Z"),
  "publishedAt": ISODate("2024-01-15T12:00:00Z")
}
```

**Design Decision**: Denormalization for read performance!

---

## Slide 7: CQRS Pattern Implementation

### **Command Query Responsibility Segregation**

```
????????????????????????????????????????????????????????
?                    WRITE SIDE                        ?
?                    (Commands)                        ?
????????????????????????????????????????????????????????
?  • CreateBlogCommand                                 ?
?  • UpdateBlogCommand                                 ?
?  • DeleteBlogCommand                                 ?
?  • RegisterCommand                                   ?
?  • LoginCommand                                      ?
?                                                      ?
?  ? Modify database state                            ?
?  ? Return Result<T> with success/failure            ?
????????????????????????????????????????????????????????

????????????????????????????????????????????????????????
?                    READ SIDE                         ?
?                    (Queries)                         ?
????????????????????????????????????????????????????????
?  • GetBlogsQuery                                     ?
?  • GetBlogByIdQuery                                  ?
?  • SearchBlogsQuery                                  ?
?                                                      ?
?  ? Retrieve data only                               ?
?  ? Optimized for performance                        ?
?  ? Return Result<T> with data                       ?
????????????????????????????????????????????????????????
```

**Benefits:**
- Separation of concerns
- Optimized read/write operations
- Better scalability
- Easier testing

---

## Slide 8: MediatR Request Flow

### **Example: Creating a Blog Post**

```csharp
// 1. CONTROLLER (API Layer)
[HttpPost]
public async Task<IActionResult> CreateBlog([FromBody] CreateBlogDto request)
{
    var command = new CreateBlogCommand
    {
        Title = request.Title,
        Content = request.Content,
        // ... map properties
    };

    var result = await _mediator.Send(command);  // ? The Magic!

    if (!result.IsSuccess)
        return BadRequest(new { message = result.ErrorMessage });

    return CreatedAtAction(nameof(GetBlog), 
        new { id = result.Data!.Id }, 
        result.Data);
}
```

```csharp
// 2. COMMAND (Application Layer)
public class CreateBlogCommand : IRequest<Result<BlogDto>>
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
    // ... other properties
}
```

---

## Slide 9: MediatR Request Flow (Continued)

```csharp
// 3. HANDLER (Application Layer)
public class CreateBlogCommandHandler 
    : IRequestHandler<CreateBlogCommand, Result<BlogDto>>
{
    private readonly IBlogRepository _blogRepository;
    private readonly ICurrentUserService _currentUserService;
    
    public async Task<Result<BlogDto>> Handle(
        CreateBlogCommand request, 
        CancellationToken cancellationToken)
    {
        // Validation
        var validator = new CreateBlogCommandValidator();
        var validationResult = await validator.ValidateAsync(request);
        
        if (!validationResult.IsValid)
            return Result<BlogDto>.Failure("Validation failed");

        // Business Logic
        var blog = new Blog
        {
            UserId = _currentUserService.UserId,
            Title = request.Title,
            Content = request.Content,
            CreatedAt = DateTime.UtcNow
        };

        // Save to MongoDB
        await _blogRepository.AddAsync(blog);
        
        return Result<BlogDto>.Success(MapToDto(blog));
    }
}
```

---

## Slide 10: How MediatR Routes Requests

### **Type-Based Matching System**

```csharp
// Step 1: Registration (Program.cs)
builder.Services.AddMediatR(cfg => 
    cfg.RegisterServicesFromAssembly(
        typeof(LoginCommand).Assembly));

// This scans and creates mappings:
// CreateBlogCommand ? CreateBlogCommandHandler
// GetBlogsQuery ? GetBlogsQueryHandler
// RegisterCommand ? RegisterCommandHandler
```

### **Runtime Resolution**

```
Controller calls:
    _mediator.Send(new CreateBlogCommand())
        ?
MediatR examines type:
    typeof(CreateBlogCommand)
        ?
Looks up handler:
    IRequestHandler<CreateBlogCommand, Result<BlogDto>>
        ?
DI Container resolves:
    CreateBlogCommandHandler (with dependencies)
        ?
Invokes method:
    handler.Handle(command, cancellationToken)
        ?
Returns:
    Result<BlogDto>
```

---

## Slide 11: Authentication & Authorization

### **JWT Token-Based Authentication**

```csharp
// Program.cs Configuration
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});
```

### **Usage in Controllers**

```csharp
[Authorize]  // ? Requires authentication
[HttpPost]
public async Task<IActionResult> CreateBlog(...)

[Authorize(Roles = "Admin")]  // ? Requires Admin role
[HttpDelete("blogs/{id}")]
public async Task<IActionResult> DeleteBlog(string id)
```

---

## Slide 12: API Endpoints Overview

### **Authentication Endpoints**
| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/api/auth/register` | Register new user | ? |
| POST | `/api/auth/login` | Login & get JWT token | ? |

### **Blog Endpoints**
| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/blogs` | Get paginated blogs | ? |
| GET | `/api/blogs/{id}` | Get single blog | ? |
| GET | `/api/blogs/my-blogs` | Get user's blogs | ? |
| POST | `/api/blogs` | Create new blog | ? |
| PUT | `/api/blogs/{id}` | Update blog | ? |
| DELETE | `/api/blogs/{id}` | Delete blog | ? |

### **Admin Endpoints**
| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| DELETE | `/api/admin/blogs/{id}` | Admin delete blog | ? Admin |

---

## Slide 13: Request/Response Examples

### **Creating a Blog Post**

**Request:**
```http
POST /api/blogs HTTP/1.1
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
Content-Type: application/json

{
  "categoryId": 5,
  "title": "Getting Started with Clean Architecture",
  "content": "Clean architecture is a software design...",
  "summary": "An introduction to clean architecture principles",
  "tags": ["architecture", "clean-code", "design-patterns"],
  "mediaItems": [
    {
      "url": "https://example.com/diagram.png",
      "type": "image",
      "caption": "Clean Architecture Diagram",
      "order": 1
    }
  ],
  "isPublished": true
}
```

**Response:**
```json
{
  "id": "507f1f77bcf86cd799439011",
  "userId": 123,
  "username": "john_doe",
  "categoryId": 5,
  "categoryName": "Architecture",
  "title": "Getting Started with Clean Architecture",
  "content": "Clean architecture is a software design...",
  "summary": "An introduction to clean architecture principles",
  "tags": ["architecture", "clean-code", "design-patterns"],
  "mediaItems": [
    {
      "url": "https://example.com/diagram.png",
      "type": "image",
      "caption": "Clean Architecture Diagram",
      "order": 1
    }
  ],
  "viewCount": 0,
  "isPublished": true,
  "createdAt": "2024-01-15T10:30:00Z",
  "updatedAt": null,
  "publishedAt": "2024-01-15T10:30:00Z"
}
```

---

## Slide 14: Validation with FluentValidation

### **Input Validation Example**

```csharp
public class CreateBlogCommandValidator : AbstractValidator<CreateBlogCommand>
{
    public CreateBlogCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Content is required")
            .MinimumLength(100).WithMessage("Content must be at least 100 characters");

        RuleFor(x => x.Summary)
            .MaximumLength(500).WithMessage("Summary must not exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.Summary));

        RuleFor(x => x.Tags)
            .Must(tags => tags.Count <= 10)
            .WithMessage("Maximum 10 tags allowed");
    }
}
```

**Registration in Program.cs:**
```csharp
builder.Services.AddValidatorsFromAssembly(
    typeof(LoginCommandValidator).Assembly);
```

---

## Slide 15: Repository Pattern

### **Interface Definition (Domain Layer)**

```csharp
public interface IBlogRepository
{
    Task<Blog> GetByIdAsync(string id);
    Task<IEnumerable<Blog>> GetAllAsync(int pageNumber, int pageSize);
    Task<IEnumerable<Blog>> GetPublishedAsync(int pageNumber, int pageSize);
    Task<IEnumerable<Blog>> GetByUserIdAsync(int userId, int pageNumber, int pageSize);
    Task<IEnumerable<Blog>> SearchAsync(string searchTerm, int pageNumber, int pageSize);
    Task AddAsync(Blog blog);
    Task UpdateAsync(Blog blog);
    Task DeleteAsync(string id);
    Task<long> GetTotalCountAsync();
}
```

### **Implementation (Infrastructure Layer)**

```csharp
public class BlogRepository : IBlogRepository
{
    private readonly IMongoCollection<Blog> _blogs;

    public BlogRepository(MongoDbContext context)
    {
        _blogs = context.Blogs;
    }

    public async Task<Blog> GetByIdAsync(string id)
    {
        return await _blogs.Find(b => b.Id == id).FirstOrDefaultAsync();
    }

    public async Task AddAsync(Blog blog)
    {
        await _blogs.InsertOneAsync(blog);
    }

    // ... other implementations
}
```

---

## Slide 16: Dependency Injection Setup

### **Service Registration (Program.cs)**

```csharp
// Database Contexts
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySQL(connectionString));
builder.Services.AddSingleton<MongoDbContext>();

// Repositories (Scoped - per HTTP request)
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IBlogRepository, BlogRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// MediatR (scans assembly and auto-registers handlers)
builder.Services.AddMediatR(cfg => 
    cfg.RegisterServicesFromAssembly(typeof(LoginCommand).Assembly));

// FluentValidation (auto-registers validators)
builder.Services.AddValidatorsFromAssembly(
    typeof(LoginCommandValidator).Assembly);
```

---

## Slide 17: Middleware & Error Handling

### **Custom Exception Handling Middleware**

```csharp
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        var response = new
        {
            message = "An error occurred while processing your request",
            details = exception.Message
        };

        return context.Response.WriteAsJsonAsync(response);
    }
}
```

**Registration:**
```csharp
app.UseMiddleware<ExceptionHandlingMiddleware>();
```

---

## Slide 18: CORS Configuration

### **Cross-Origin Resource Sharing Setup**

```csharp
// Program.cs
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins(
                "http://localhost:3000",    // React dev server
                "http://localhost:5173")    // Vite dev server
              .AllowAnyMethod()              // GET, POST, PUT, DELETE, etc.
              .AllowAnyHeader()              // Authorization, Content-Type, etc.
              .AllowCredentials();           // Allow cookies/auth headers
    });
});

// Middleware pipeline
app.UseCors("AllowAll");
```

### **Why This Matters**
- Enables frontend applications to consume the API
- Supports multiple development environments
- Allows authenticated requests with JWT tokens
- Production-ready with specific origins (not "*")

---

## Slide 19: Swagger/OpenAPI Integration

### **API Documentation Configuration**

```csharp
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Blog Application API", 
        Version = "v1" 
    });
    
    // JWT Authentication in Swagger UI
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
```

**Access at:** `https://localhost:5001/swagger`

---

## Slide 20: Key Design Patterns Used

### **1. CQRS (Command Query Responsibility Segregation)**
- Separate read and write operations
- Optimized for different concerns

### **2. Mediator Pattern (via MediatR)**
- Decouples controllers from handlers
- Single responsibility principle
- Easy to test and maintain

### **3. Repository Pattern**
- Abstracts data access logic
- Easily swap implementations
- Testable with mocks

### **4. Unit of Work Pattern**
- Transaction management
- Coordinates multiple repositories
- Ensures data consistency

### **5. Dependency Injection**
- Loose coupling
- Testability
- Lifecycle management

### **6. Result Pattern**
- Explicit success/failure handling
- No exceptions for flow control
- Type-safe error handling

---

## Slide 21: Project Structure

```
BlogApplication/
?
??? BlogApplication.API/              # Presentation Layer
?   ??? Controllers/
?   ?   ??? AuthController.cs
?   ?   ??? BlogsController.cs
?   ?   ??? AdminController.cs
?   ??? Middleware/
?   ?   ??? ExceptionHandlingMiddleware.cs
?   ??? Program.cs                    # Application entry point
?
??? BlogApplication.Application/      # Application Layer
?   ??? Features/
?   ?   ??? Auth/
?   ?   ?   ??? Commands/            # Login, Register
?   ?   ?   ??? Validators/
?   ?   ?   ??? Handlers/
?   ?   ??? Blogs/
?   ?       ??? Commands/            # Create, Update, Delete
?   ?       ??? Queries/             # Get, Search
?   ?       ??? Handlers/
?   ??? DTOs/                        # Data Transfer Objects
?   ??? Common/                      # Result<T> wrapper
?   ??? Interfaces/
?
??? BlogApplication.Domain/           # Domain Layer
?   ??? Entities/
?   ?   ??? User.cs
?   ?   ??? Role.cs
?   ?   ??? Blog.cs
?   ?   ??? Category.cs
?   ?   ??? BaseEntity.cs
?   ??? Interfaces/                  # Repository interfaces
?
??? BlogApplication.Infrastructure/   # Infrastructure Layer
    ??? Data/
    ?   ??? ApplicationDbContext.cs  # MySQL EF Core
    ?   ??? MongoDbContext.cs        # MongoDB
    ??? Repositories/
    ?   ??? UserRepository.cs
    ?   ??? BlogRepository.cs
    ?   ??? CategoryRepository.cs
    ??? Services/
    ?   ??? AuthService.cs
    ?   ??? PasswordHasher.cs
    ??? Configuration/
    ?   ??? JwtSettings.cs
    ?   ??? MongoDbSettings.cs
    ??? Migrations/                  # EF Core migrations
```

---

## Slide 22: Security Features

### **Password Security**
```csharp
public interface IPasswordHasher
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string hash);
}

// Implementation uses BCrypt or PBKDF2
```

### **JWT Token Generation**
```csharp
public class AuthService : IAuthService
{
    public string GenerateJwtToken(User user, List<string> roles)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email)
        };

        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(_jwtSettings.ExpirationHours),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
```

### **Protection Features**
- ? Hashed passwords (never stored in plain text)
- ? JWT token expiration
- ? Role-based authorization
- ? HTTPS enforcement
- ? CORS restrictions

---

## Slide 23: Database Migration & Seeding

### **Entity Framework Migrations**

```csharp
// Create migration
dotnet ef migrations add InitialCreate

// Apply to database
dotnet ef database update
```

### **Auto-Migration & Seeding on Startup**

```csharp
// Program.cs
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider
        .GetRequiredService<ApplicationDbContext>();
    
    // Apply pending migrations
    dbContext.Database.Migrate();
    
    // Seed default admin user
    await DbInitializer.SeedAdminUser(dbContext);
}
```

### **DbInitializer Example**
```csharp
public static class DbInitializer
{
    public static async Task SeedAdminUser(ApplicationDbContext context)
    {
        if (!await context.Users.AnyAsync())
        {
            var adminRole = new Role { Name = "Admin", Description = "Administrator" };
            var userRole = new Role { Name = "User", Description = "Regular User" };
            
            await context.Roles.AddRangeAsync(adminRole, userRole);
            
            var admin = new User
            {
                Username = "admin",
                Email = "admin@blog.com",
                PasswordHash = HashPassword("Admin@123"),
                IsActive = true
            };
            
            await context.Users.AddAsync(admin);
            await context.SaveChangesAsync();
        }
    }
}
```

---

## Slide 24: Testing Strategy

### **Unit Testing Approach**

```csharp
// Example: Testing CreateBlogCommandHandler
public class CreateBlogCommandHandlerTests
{
    [Fact]
    public async Task Handle_ValidCommand_ReturnsSuccessResult()
    {
        // Arrange
        var mockBlogRepo = new Mock<IBlogRepository>();
        var mockUserService = new Mock<ICurrentUserService>();
        mockUserService.Setup(x => x.UserId).Returns(1);
        
        var handler = new CreateBlogCommandHandler(
            mockBlogRepo.Object, 
            mockUserService.Object);
        
        var command = new CreateBlogCommand
        {
            Title = "Test Blog",
            Content = "Test content with at least 100 characters..."
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        mockBlogRepo.Verify(x => x.AddAsync(It.IsAny<Blog>()), Times.Once);
    }

    [Fact]
    public async Task Handle_EmptyTitle_ReturnsFailure()
    {
        // Test validation scenarios
    }
}
```

### **Integration Testing**
- Test full HTTP request/response cycle
- Use WebApplicationFactory
- Test database interactions
- Verify authentication/authorization

---

## Slide 25: Performance Optimizations

### **1. Denormalization in MongoDB**
```csharp
public class Blog
{
    public int UserId { get; set; }
    public string Username { get; set; }      // ? Denormalized from User
    public int? CategoryId { get; set; }
    public string? CategoryName { get; set; } // ? Denormalized from Category
    
    // Avoids JOIN operations when reading blogs
}
```

### **2. Pagination**
```csharp
public async Task<IEnumerable<Blog>> GetAllAsync(int pageNumber, int pageSize)
{
    return await _blogs
        .Find(_ => true)
        .Skip((pageNumber - 1) * pageSize)
        .Limit(pageSize)
        .ToListAsync();
}
```

### **3. Indexed Fields**
- User Email (unique index)
- User Username (unique index)
- Blog Title (text index for search)
- Blog Tags (multi-key index)

### **4. Async/Await Throughout**
- Non-blocking I/O operations
- Better thread utilization
- Improved scalability

### **5. Scoped Services**
- Per-request lifecycle
- Proper resource disposal
- Memory efficiency

---

## Slide 26: Future Enhancements

### **Planned Features**

**1. Comments System**
- Nested comments on blog posts
- Reply functionality
- Comment moderation

**2. Like/Reaction System**
- Users can like blogs
- Reaction analytics
- Popular post tracking

**3. File Upload Service**
- Azure Blob Storage integration
- Image optimization
- CDN support

**4. Real-time Notifications**
- SignalR integration
- Email notifications
- Push notifications

**5. Caching Layer**
- Redis integration
- Response caching
- Distributed cache

**6. Search Enhancement**
- Elasticsearch integration
- Full-text search
- Faceted search

**7. Analytics Dashboard**
- View statistics
- User engagement metrics
- Content performance

---

## Slide 27: Development Workflow

### **Local Development Setup**

```bash
# 1. Clone repository
git clone https://github.com/golamyusuf/Blog-App
cd Blog-App/src

# 2. Update connection strings in appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "server=localhost;database=BlogApp;user=root;password=****"
  },
  "MongoDbSettings": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "BlogAppMongo"
  }
}

# 3. Install dependencies
dotnet restore

# 4. Apply migrations
dotnet ef database update --project BlogApplication.Infrastructure

# 5. Run application
dotnet run --project BlogApplication.API

# 6. Access Swagger UI
https://localhost:5001/swagger
```

### **Required Software**
- .NET SDK 9.0 or 10.0
- MySQL Server 8.0+
- MongoDB 5.0+
- Visual Studio 2022 / VS Code / Rider

---

## Slide 28: API Best Practices Implemented

### **? RESTful Design**
- Proper HTTP verbs (GET, POST, PUT, DELETE)
- Resource-based URLs
- Meaningful status codes

### **? Versioning Ready**
```csharp
[Route("api/v1/[controller]")]  // Future v2 API support
```

### **? Consistent Response Format**
```csharp
public class Result<T>
{
    public bool IsSuccess { get; set; }
    public T? Data { get; set; }
    public string? ErrorMessage { get; set; }
    public List<string>? Errors { get; set; }
}
```

### **? Proper HTTP Status Codes**
- 200 OK - Successful GET/PUT
- 201 Created - Successful POST
- 204 No Content - Successful DELETE
- 400 Bad Request - Validation errors
- 401 Unauthorized - Missing/invalid token
- 403 Forbidden - Insufficient permissions
- 404 Not Found - Resource doesn't exist
- 500 Internal Server Error - Unexpected errors

### **? Security Headers**
- CORS configured properly
- HTTPS enforced
- JWT token validation

---

## Slide 29: Code Quality Measures

### **1. Clean Code Principles**
- Single Responsibility Principle
- Dependency Inversion
- Interface Segregation
- Don't Repeat Yourself (DRY)

### **2. Naming Conventions**
```csharp
// Commands: [Entity][Action]Command
CreateBlogCommand, UpdateBlogCommand

// Queries: Get[Entity]Query
GetBlogsQuery, GetBlogByIdQuery

// Handlers: [CommandOrQuery]Handler
CreateBlogCommandHandler, GetBlogsQueryHandler

// DTOs: [Entity]Dto
BlogDto, UserDto, CreateBlogDto
```

### **3. Documentation**
- XML comments on public APIs
- Swagger annotations
- README files
- Inline comments for complex logic

### **4. Error Handling**
- Global exception middleware
- Validation at multiple layers
- Meaningful error messages
- Logging for diagnostics

---

## Slide 30: Architectural Benefits

### **Why This Architecture?**

**? Maintainability**
- Clear separation of concerns
- Easy to locate and modify code
- Changes in one layer don't affect others

**? Testability**
- Dependency injection enables mocking
- Business logic isolated from infrastructure
- Unit tests don't require databases

**? Scalability**
- Stateless API design
- Horizontal scaling ready
- Database optimization strategies

**? Flexibility**
- Easy to swap implementations
- Can change databases without affecting business logic
- Plugin additional features via MediatR behaviors

**? Team Collaboration**
- Different developers can work on different layers
- Clear boundaries reduce merge conflicts
- Consistent patterns across codebase

---

## Slide 31: Learning Outcomes

### **Technologies Mastered**

1. **Clean Architecture**
   - Dependency flow from outer to inner layers
   - Domain-driven design principles

2. **CQRS Pattern**
   - Separating read and write operations
   - Optimizing for different concerns

3. **MediatR**
   - Request/response pattern
   - Pipeline behaviors
   - Handler discovery and routing

4. **Entity Framework Core**
   - Code-first migrations
   - Repository pattern
   - Relationship mapping

5. **MongoDB**
   - Document-oriented design
   - Denormalization strategies
   - NoSQL best practices

6. **JWT Authentication**
   - Token generation and validation
   - Claims-based authorization
   - Role-based access control

7. **ASP.NET Core**
   - Middleware pipeline
   - Dependency injection
   - API development

---

## Slide 32: Common Pitfalls Avoided

### **? Anti-patterns NOT Used**

**1. Anemic Domain Model**
- ? Rich domain entities with behavior (when applicable)

**2. God Objects**
- ? Small, focused classes with single responsibility

**3. Magic Strings**
- ? Constants and enums for role names, claim types

**4. Exception-Based Flow Control**
- ? Result<T> pattern for expected failures

**5. N+1 Query Problem**
- ? Denormalization in MongoDB
- ? Eager loading in EF Core where needed

**6. Tight Coupling**
- ? Dependency injection throughout
- ? Interface-based programming

**7. Hardcoded Configuration**
- ? appsettings.json
- ? Options pattern

---

## Slide 33: Deployment Considerations

### **Production Readiness Checklist**

**Infrastructure**
- [ ] MySQL database (Azure Database, AWS RDS)
- [ ] MongoDB cluster (MongoDB Atlas, AWS DocumentDB)
- [ ] Application hosting (Azure App Service, AWS Elastic Beanstalk, Docker)
- [ ] HTTPS certificate (Let's Encrypt, Azure SSL)
- [ ] CDN for static assets

**Configuration**
- [ ] Production connection strings
- [ ] Strong JWT secret key (not in source control)
- [ ] Environment variables
- [ ] Logging configuration (Application Insights, CloudWatch)

**Security**
- [ ] Rate limiting
- [ ] Input sanitization
- [ ] SQL injection protection (EF Core handles this)
- [ ] XSS protection
- [ ] CSRF tokens (if needed)

**Monitoring**
- [ ] Application Performance Monitoring (APM)
- [ ] Error tracking (Sentry, Application Insights)
- [ ] Health check endpoints
- [ ] Database monitoring

**Backup & Recovery**
- [ ] Database backup strategy
- [ ] Disaster recovery plan
- [ ] Data retention policies

---

## Slide 34: Docker Support (Optional Enhancement)

### **Dockerfile Example**

```dockerfile
# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["BlogApplication.API/BlogApplication.API.csproj", "BlogApplication.API/"]
COPY ["BlogApplication.Application/BlogApplication.Application.csproj", "BlogApplication.Application/"]
COPY ["BlogApplication.Domain/BlogApplication.Domain.csproj", "BlogApplication.Domain/"]
COPY ["BlogApplication.Infrastructure/BlogApplication.Infrastructure.csproj", "BlogApplication.Infrastructure/"]

RUN dotnet restore "BlogApplication.API/BlogApplication.API.csproj"
COPY . .

WORKDIR "/src/BlogApplication.API"
RUN dotnet build -c Release -o /app/build
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 80
EXPOSE 443

ENTRYPOINT ["dotnet", "BlogApplication.API.dll"]
```

### **docker-compose.yml**

```yaml
version: '3.8'
services:
  api:
    build: .
    ports:
      - "5000:80"
    environment:
      - ConnectionStrings__DefaultConnection=server=mysql;database=BlogApp;user=root;password=****
      - MongoDbSettings__ConnectionString=mongodb://mongo:27017
    depends_on:
      - mysql
      - mongo

  mysql:
    image: mysql:8.0
    environment:
      MYSQL_ROOT_PASSWORD: ****
      MYSQL_DATABASE: BlogApp

  mongo:
    image: mongo:5.0
    ports:
      - "27017:27017"
```

---

## Slide 35: Performance Metrics

### **Expected Performance Characteristics**

| Operation | Response Time | Throughput |
|-----------|---------------|------------|
| User Login | < 200ms | 500 req/sec |
| Get Blog List (paginated) | < 150ms | 1000 req/sec |
| Get Single Blog | < 100ms | 1500 req/sec |
| Create Blog | < 300ms | 200 req/sec |
| Update Blog | < 250ms | 300 req/sec |
| Delete Blog | < 200ms | 400 req/sec |

### **Optimization Techniques**

**Database Level**
- Indexed queries
- Denormalized data (MongoDB)
- Connection pooling

**Application Level**
- Async/await throughout
- Minimal allocations
- Scoped service lifetimes

**API Level**
- Response caching headers
- Compression middleware
- Pagination (not loading all data)

**Network Level**
- GZIP compression
- CDN for static assets
- HTTP/2 support

---

## Slide 36: Comparison: SQL vs NoSQL

### **Why Hybrid Database Approach?**

| Aspect | MySQL (Relational) | MongoDB (Document) |
|--------|-------------------|-------------------|
| **Use Case** | Users, Roles, Categories | Blog Posts, Comments |
| **Strengths** | ACID transactions, relationships | Flexible schema, fast reads |
| **Data Type** | Structured, normalized | Semi-structured, denormalized |
| **Queries** | Complex JOINs, relationships | Simple lookups, embedded data |
| **Scaling** | Vertical scaling | Horizontal scaling |
| **Schema** | Fixed, migration required | Flexible, dynamic |

### **Design Decision**

```
User Authentication & Authorization ? MySQL
   ? (Strict ACID requirements, relationships)
   
Blog Content & Media ? MongoDB
   ? (Flexible schema, fast reads, large documents)
   
Result: Best of both worlds!
```

**Benefits:**
- ? Relational integrity for critical data (users, roles)
- ? Performance for content-heavy operations (blogs)
- ? Schema flexibility for evolving blog structure
- ? Easy horizontal scaling for blog storage

---

## Slide 37: MediatR Pipeline Behaviors (Advanced)

### **Cross-Cutting Concerns**

```csharp
// Example: Logging Behavior
public class LoggingBehavior<TRequest, TResponse> 
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        
        _logger.LogInformation("Handling {RequestName}", requestName);
        var stopwatch = Stopwatch.StartNew();

        var response = await next();  // Call actual handler

        stopwatch.Stop();
        _logger.LogInformation("Handled {RequestName} in {Elapsed}ms", 
            requestName, stopwatch.ElapsedMilliseconds);

        return response;
    }
}
```

### **Other Possible Behaviors**
- Validation (FluentValidation integration)
- Transaction management
- Caching
- Performance monitoring
- Authorization checks

**Registration:**
```csharp
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
```

---

## Slide 38: Real-World Scenarios Handled

### **Scenario 1: User Creates Draft Blog**
```
1. User authenticates ? JWT token received
2. POST /api/blogs with isPublished: false
3. CreateBlogCommandHandler validates input
4. Blog saved to MongoDB with isPublished: false
5. Blog appears in /api/blogs/my-blogs but NOT in public listing
6. User can edit later
7. User publishes ? isPublished: true, publishedAt: now()
```

### **Scenario 2: Admin Moderates Content**
```
1. Admin role detected via JWT claims
2. Admin views all blogs (including unpublished)
3. DELETE /api/admin/blogs/{id} removes inappropriate content
4. Authorization enforced via [Authorize(Roles = "Admin")]
```

### **Scenario 3: Public User Browses Blogs**
```
1. No authentication required
2. GET /api/blogs?pageNumber=1&pageSize=10&categoryId=5
3. GetBlogsQueryHandler filters publishedOnly: true
4. Returns paginated, public blogs
5. GET /api/blogs/{id} increments view count
```

### **Scenario 4: Search Functionality**
```
1. GET /api/blogs?searchTerm=architecture
2. MongoDB text search on title and content
3. Returns relevant blogs ranked by relevance
```

---

## Slide 39: Code Walkthrough - Complete Request Flow

### **Complete Flow: Creating a Blog Post**

```
???????????????????????????????????????????????????????????
? 1. HTTP REQUEST                                         ?
? POST /api/blogs                                         ?
? Headers: Authorization: Bearer <token>                  ?
? Body: { "title": "...", "content": "..." }             ?
???????????????????????????????????????????????????????????
                     ?
???????????????????????????????????????????????????????????
? 2. MIDDLEWARE PIPELINE                                  ?
? • CORS Middleware                                       ?
? • Authentication Middleware ? Validates JWT             ?
? • Authorization Middleware ? Checks [Authorize]         ?
? • Exception Handling Middleware                         ?
???????????????????????????????????????????????????????????
                     ?
???????????????????????????????????????????????????????????
? 3. CONTROLLER (BlogsController.cs)                      ?
? • Maps CreateBlogDto ? CreateBlogCommand                ?
? • Calls: _mediator.Send(command)                        ?
???????????????????????????????????????????????????????????
                     ?
???????????????????????????????????????????????????????????
? 4. MEDIATR                                              ?
? • Examines: typeof(CreateBlogCommand)                   ?
? • Looks up: IRequestHandler<CreateBlogCommand, Result>  ?
? • Resolves: CreateBlogCommandHandler from DI            ?
???????????????????????????????????????????????????????????
                     ?
???????????????????????????????????????????????????????????
? 5. HANDLER (CreateBlogCommandHandler.cs)                ?
? • Gets current user ID from ICurrentUserService         ?
? • Validates command with FluentValidation               ?
? • Creates Blog entity                                   ?
? • Calls: _blogRepository.AddAsync(blog)                 ?
???????????????????????????????????????????????????????????
                     ?
???????????????????????????????????????????????????????????
? 6. REPOSITORY (BlogRepository.cs)                       ?
? • Calls: _blogs.InsertOneAsync(blog)                    ?
? • MongoDB driver persists document                      ?
???????????????????????????????????????????????????????????
                     ?
???????????????????????????????????????????????????????????
? 7. MONGODB                                              ?
? • Document stored in "blogs" collection                 ?
? • Returns inserted document with _id                    ?
???????????????????????????????????????????????????????????
                     ?
                     ? (Response bubbles back up)
                     ?
???????????????????????????????????????????????????????????
? 8. HTTP RESPONSE                                        ?
? 201 Created                                             ?
? Location: /api/blogs/{newId}                            ?
? Body: { "id": "...", "title": "...", ... }              ?
???????????????????????????????????????????????????????????
```

---

## Slide 40: Summary & Conclusion

### **What We've Built**
A **production-ready** blog application demonstrating:
- ? Clean Architecture principles
- ? CQRS pattern with MediatR
- ? Hybrid database strategy (MySQL + MongoDB)
- ? JWT-based authentication & authorization
- ? RESTful API design
- ? Comprehensive validation
- ? Error handling & logging
- ? API documentation with Swagger

### **Key Takeaways**

1. **Architecture Matters**: Clean separation enables maintainability
2. **Patterns Solve Problems**: CQRS, Repository, Mediator all have purposes
3. **Type Safety**: C# + MediatR provide compile-time guarantees
4. **Security First**: Authentication, authorization, password hashing
5. **Developer Experience**: Swagger, clear errors, consistent patterns

### **Why This Approach?**
- ?? **Scalable**: Can handle growing user base
- ?? **Maintainable**: Easy to modify and extend
- ?? **Testable**: Every layer can be unit tested
- ?? **Team-Friendly**: Clear structure for collaboration
- ?? **Professional**: Industry-standard patterns and practices

---

## Slide 41: Resources & References

### **Technologies Documentation**
- ASP.NET Core: https://docs.microsoft.com/aspnet/core
- Entity Framework Core: https://docs.microsoft.com/ef/core
- MongoDB .NET Driver: https://mongodb.github.io/mongo-csharp-driver
- MediatR: https://github.com/jbogard/MediatR
- FluentValidation: https://fluentvalidation.net

### **Architecture References**
- Clean Architecture (Robert C. Martin)
- Domain-Driven Design (Eric Evans)
- CQRS Pattern: https://martinfowler.com/bliki/CQRS.html

### **GitHub Repository**
?? **https://github.com/golamyusuf/Blog-App**

### **Contact**
- GitHub: @golamyusuf
- Project Repository: Blog-App

---

## Slide 42: Q&A

# Questions?

### Common Questions:

**Q: Why both MySQL and MongoDB?**
A: MySQL for relational integrity (users, roles), MongoDB for flexible content storage and performance.

**Q: Why MediatR instead of direct service calls?**
A: Decoupling, testability, single responsibility, and cross-cutting concerns via pipeline behaviors.

**Q: How do you handle concurrent updates?**
A: Optimistic concurrency with UpdatedAt timestamp checks, Unit of Work for transactions.

**Q: What about caching?**
A: Future enhancement - Redis for distributed caching, response caching middleware.

**Q: How to add new features?**
A: Create new Command/Query, Handler, add DTO, controller endpoint. MediatR auto-registers!

**Q: Production deployment?**
A: Docker containers, Azure App Service, or AWS Elastic Beanstalk with managed databases.

---

## Thank You!

### **Project Highlights**
- ??? Clean Architecture
- ?? CQRS Pattern
- ?? JWT Security
- ?? Hybrid Database
- ?? Production-Ready

**GitHub Repository:**
https://github.com/golamyusuf/Blog-App

---
