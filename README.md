# Blog Application

A full-stack blog application where users can create their own accounts and post their thoughts and stories with images and videos. Built with .NET backend following Clean Architecture principles and React frontend with TypeScript.

## Features

### User Features
- 🔐 **User Authentication**: Secure JWT-based authentication and authorization
- ✍️ **Create & Edit Blogs**: Rich blog creation with support for images and videos
- 📝 **Personal Blog Management**: View, edit, and delete your own blogs
- 🔍 **Search & Browse**: Search and browse published blogs
- 🏷️ **Tags & Categories**: Organize blogs with tags
- 👁️ **View Tracking**: Automatic view count tracking

### Admin Features
- 🛡️ **Content Moderation**: Remove inappropriate or anti-social posts
- 📊 **Dashboard**: View all blogs and manage content

## Technology Stack

### Backend
- **.NET 10.0** - Latest .NET framework
- **Clean Architecture** - Domain-driven design with clear separation of concerns
- **CQRS Pattern** - Using MediatR for command and query separation
- **Entity Framework Core** - ORM for MySQL database
- **MongoDB Driver** - For blog storage
- **JWT Authentication** - Secure token-based authentication
- **FluentValidation** - Input validation
- **xUnit** - Unit testing framework
- **Moq & FluentAssertions** - Testing tools

### Frontend
- **React 18** - Modern UI library
- **TypeScript** - Type-safe JavaScript
- **Vite** - Fast build tool
- **React Router** - Client-side routing
- **Axios** - HTTP client
- **React Toastify** - Notifications
- **CSS3** - Responsive styling

### Databases
- **MySQL** - User and role management (running in Docker as `mysql-db-new`)
- **MongoDB** - Blog posts storage (running in Docker as `mongo-dev`)

## Architecture

### Backend Architecture (Clean Architecture)

```
BlogApplication/
├── src/
│   ├── BlogApplication.Domain/          # Enterprise business rules
│   │   ├── Entities/                    # Domain entities
│   │   ├── Interfaces/                  # Repository interfaces
│   │   └── Exceptions/                  # Domain exceptions
│   │
│   ├── BlogApplication.Application/     # Application business rules
│   │   ├── DTOs/                        # Data Transfer Objects
│   │   ├── Features/                    # CQRS Commands & Queries
│   │   │   ├── Auth/
│   │   │   └── Blogs/
│   │   ├── Interfaces/                  # Application services
│   │   └── Common/                      # Shared application logic
│   │
│   ├── BlogApplication.Infrastructure/  # External concerns
│   │   ├── Data/                        # Database contexts
│   │   ├── Repositories/                # Repository implementations
│   │   ├── Services/                    # Service implementations
│   │   └── Configuration/               # Configuration classes
│   │
│   ├── BlogApplication.API/             # Presentation layer
│   │   ├── Controllers/                 # API endpoints
│   │   └── Middleware/                  # HTTP middleware
│   │
│   └── BlogApplication.Tests/           # Unit tests
│       ├── Application/
│       └── Validators/
```

### Design Patterns Implemented
1. **Repository Pattern** - Data access abstraction
2. **Unit of Work Pattern** - Transaction management
3. **CQRS Pattern** - Command Query Responsibility Segregation
4. **Mediator Pattern** - Decoupled request handling
5. **Dependency Injection** - Loose coupling
6. **Factory Pattern** - Object creation
7. **Strategy Pattern** - Password hashing

## Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download)
- [Node.js 18+](https://nodejs.org/) (for frontend)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [Git](https://git-scm.com/)

## Getting Started

### 1. Clone the Repository

```bash
cd "D:\Personal_Project\Blog application"
```

### 2. Start Docker Containers

Start MySQL and MongoDB containers:

```bash
docker-compose up -d
```

Verify containers are running:

```bash
docker ps
```

You should see `mysql-db-new` and `mongo-dev` containers running.

### 3. Configure Backend

#### Update Connection Strings

Edit `src/BlogApplication.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=blogapp;User=root;Password=YOUR_PASSWORD;"
  },
  "MongoDbSettings": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "blogapp"
  },
  "JwtSettings": {
    "Secret": "YOUR_SECRET_KEY_AT_LEAST_32_CHARACTERS_LONG",
    "Issuer": "BlogApplication",
    "Audience": "BlogApplicationUsers",
    "ExpirationInMinutes": 1440
  }
}
```

**Important**: Replace `YOUR_PASSWORD` and `YOUR_SECRET_KEY_AT_LEAST_32_CHARACTERS_LONG` with your actual values.

### 4. Run Database Migrations

```bash
cd src/BlogApplication.API
dotnet ef migrations add InitialCreate --project ../BlogApplication.Infrastructure
dotnet ef database update
```

### 5. Start Backend API

```bash
cd src/BlogApplication.API
dotnet run
```

The API will start at `https://localhost:5001` (or `http://localhost:5000`)

**Swagger Documentation**: https://localhost:5001/swagger

### 6. Start Frontend

Open a new terminal:

```bash
cd client
npm install
npm run dev
```

The frontend will start at `http://localhost:3000`

### 7. Create Admin User

You can create an admin user in two ways:

**Option 1: Using Swagger**
1. Go to https://localhost:5001/swagger
2. Use POST `/api/auth/register` to create a user
3. Manually update the user's role in MySQL database

**Option 2: Using MySQL**
```sql
USE blogapp;

-- Find the user ID
SELECT * FROM Users WHERE Email = 'admin@example.com';

-- Assign Admin role (role ID 2)
INSERT INTO UserRoles (UserId, RoleId, AssignedAt) 
VALUES (1, 2, NOW());
```

## API Endpoints

### Authentication
- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - Login user

### Blogs
- `GET /api/blogs` - Get all published blogs (public)
- `GET /api/blogs/{id}` - Get blog by ID (public)
- `GET /api/blogs/my-blogs` - Get current user's blogs (authenticated)
- `POST /api/blogs` - Create new blog (authenticated)
- `PUT /api/blogs/{id}` - Update blog (authenticated, owner only)
- `DELETE /api/blogs/{id}` - Delete blog (authenticated, owner or admin)

### Admin
- `DELETE /api/admin/blogs/{id}` - Delete any blog (admin only)

## Running Tests

```bash
cd src/BlogApplication.Tests
dotnet test
```

## Environment Variables

Create a `.env` file in the root directory based on `.env.example`:

```env
MYSQL_ROOT_PASSWORD=YourStrongPassword123!
JWT_SECRET=YourSuperSecretKeyThatIsAtLeast32CharactersLongForMaximumSecurity123!
```

## Security Features

- ✅ Password hashing using BCrypt
- ✅ JWT token authentication
- ✅ Role-based authorization (User, Admin)
- ✅ CORS configuration
- ✅ Input validation using FluentValidation
- ✅ SQL injection prevention through EF Core
- ✅ XSS protection in frontend

## Default Roles

1. **User** - Can create, edit, and delete own blogs
2. **Admin** - Can delete any blog (for content moderation)

## Project Structure

### Backend Layers

**Domain Layer** (`BlogApplication.Domain`)
- Pure business logic and domain models
- No dependencies on other layers
- Contains entities, value objects, domain events, and interfaces

**Application Layer** (`BlogApplication.Application`)
- Application-specific business rules
- Contains DTOs, CQRS handlers, validators
- Depends only on Domain layer

**Infrastructure Layer** (`BlogApplication.Infrastructure`)
- External concerns (databases, file systems, etc.)
- Implements interfaces defined in Domain/Application layers
- Contains EF Core configurations, repositories, services

**Presentation Layer** (`BlogApplication.API`)
- API endpoints
- Middleware
- Startup configuration

### Frontend Structure

```
client/
├── src/
│   ├── api/              # API client functions
│   ├── components/       # Reusable components
│   ├── contexts/         # React contexts (Auth)
│   ├── pages/            # Page components
│   ├── types/            # TypeScript interfaces
│   └── App.tsx           # Main app component
```

## Troubleshooting

### Docker Issues

**Containers won't start:**
```bash
docker-compose down
docker-compose up -d
```

**Check logs:**
```bash
docker logs mysql-db-new
docker logs mongo-dev
```

### Database Connection Issues

**MySQL connection refused:**
- Ensure MySQL container is running: `docker ps`
- Check port 3306 is not used by another service
- Verify password in connection string matches Docker environment

**MongoDB connection issues:**
- Ensure MongoDB container is running
- Check port 27017 is available
- Verify connection string format

### Build Issues

**Backend:**
```bash
cd src
dotnet clean
dotnet restore
dotnet build
```

**Frontend:**
```bash
cd client
rm -rf node_modules package-lock.json
npm install
```

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## License

This project is licensed under the MIT License.

## Support

For issues and questions:
- Create an issue in the repository
- Check existing documentation
- Review error logs in console/terminal

## Acknowledgments

- Built with Clean Architecture principles
- Follows SOLID principles
- Implements industry-standard security practices
- Uses modern web development technologies
