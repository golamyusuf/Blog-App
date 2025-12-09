# Quick Start Guide

Get the Blog Application up and running in 5 minutes!

## Step 1: Start Docker Containers

```powershell
# Navigate to project directory
cd "D:\Personal_Project\Blog application"

# Start databases
docker-compose up -d

# Verify containers are running
docker ps
```

You should see both `mysql-db-new` and `mongo-dev` containers running.

## Step 2: Configure the Application

### Update Backend Settings

Edit `src/BlogApplication.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=blogapp;User=root;Password=root123;"
  },
  "JwtSettings": {
    "Secret": "ThisIsAVerySecretKeyWith32CharactersForJWT123456",
    "Issuer": "BlogApplication",
    "Audience": "BlogApplicationUsers",
    "ExpirationInMinutes": 1440
  }
}
```

**Note:** Use strong passwords in production!

## Step 3: Run Database Migrations

```powershell
cd src/BlogApplication.API
dotnet ef database update
```

If migrations don't exist:
```powershell
dotnet ef migrations add InitialCreate --project ../BlogApplication.Infrastructure
dotnet ef database update
```

## Step 4: Start the Backend

```powershell
# In src/BlogApplication.API directory
dotnet run
```

The API will start at:
- HTTP: http://localhost:5000
- HTTPS: https://localhost:5001
- Swagger: https://localhost:5001/swagger

## Step 5: Install Frontend Dependencies

```powershell
# Open new terminal
cd client
npm install
```

## Step 6: Start the Frontend

```powershell
# In client directory
npm run dev
```

The application will open at: http://localhost:3000

## Step 7: Create Your First Account

1. Go to http://localhost:3000
2. Click "Register"
3. Fill in your details:
   - Username: yourname
   - Email: your@email.com
   - Password: Password123 (must have uppercase, lowercase, and number)
4. Click "Register"
5. Login with your credentials

## Step 8: Create Your First Blog

1. After logging in, click "Create Blog"
2. Fill in:
   - Title: "My First Blog Post"
   - Content: Write at least 50 characters
   - Tags: technology, tutorial
3. Optionally add images/videos by URL
4. Check "Publish immediately"
5. Click "Create Blog"

## Creating an Admin Account

### Method 1: Direct Database Update

```sql
-- Connect to MySQL
docker exec -it mysql-db-new mysql -uroot -proot123

USE blogapp;

-- Find your user ID
SELECT * FROM Users WHERE Email = 'your@email.com';

-- Assign Admin role (assuming User ID is 1)
INSERT INTO UserRoles (UserId, RoleId, AssignedAt) 
VALUES (1, 2, NOW());

-- Verify
SELECT u.Username, r.Name 
FROM Users u
JOIN UserRoles ur ON u.Id = ur.UserId
JOIN Roles r ON ur.RoleId = r.RoleId
WHERE u.Id = 1;
```

### Method 2: API Call (for development)

1. Register a new user via API
2. Update the database as shown above
3. Login again to get a new token with admin privileges

## Testing the API

### Using Swagger

1. Go to https://localhost:5001/swagger
2. Click "Authorize"
3. Enter: `Bearer YOUR_JWT_TOKEN`
4. Test any endpoint

### Using curl

```bash
# Register
curl -X POST http://localhost:5000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "username": "testuser",
    "email": "test@example.com",
    "password": "Password123"
  }'

# Login
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "password": "Password123"
  }'

# Get blogs
curl http://localhost:5000/api/blogs
```

## Troubleshooting

### Port Already in Use

**Frontend (port 3000):**
```powershell
# Kill process on port 3000
Stop-Process -Id (Get-NetTCPConnection -LocalPort 3000).OwningProcess -Force
```

**Backend (port 5000/5001):**
```powershell
# Change port in launchSettings.json
# src/BlogApplication.API/Properties/launchSettings.json
```

### Docker Containers Not Starting

```powershell
# Stop all containers
docker-compose down

# Remove volumes (WARNING: deletes data)
docker-compose down -v

# Start fresh
docker-compose up -d
```

### Database Connection Error

1. Check if MySQL container is running: `docker ps`
2. Test connection:
```powershell
docker exec -it mysql-db-new mysql -uroot -proot123
```
3. Verify password in connection string matches Docker password

### Migration Errors

```powershell
# Drop and recreate database
docker exec -it mysql-db-new mysql -uroot -proot123 -e "DROP DATABASE IF EXISTS blogapp; CREATE DATABASE blogapp;"

# Rerun migrations
cd src/BlogApplication.API
dotnet ef database update
```

## Common Commands

### Docker

```powershell
# Start containers
docker-compose up -d

# Stop containers
docker-compose down

# View logs
docker logs mysql-db-new
docker logs mongo-dev

# Enter MySQL shell
docker exec -it mysql-db-new mysql -uroot -proot123

# Enter MongoDB shell
docker exec -it mongo-dev mongosh
```

### Backend

```powershell
# Restore packages
dotnet restore

# Build
dotnet build

# Run
dotnet run

# Run tests
dotnet test

# Create migration
dotnet ef migrations add MigrationName --project ../BlogApplication.Infrastructure

# Update database
dotnet ef database update
```

### Frontend

```powershell
# Install dependencies
npm install

# Run development server
npm run dev

# Build for production
npm run build

# Preview production build
npm run preview
```

## Default Credentials

**Database:**
- MySQL Root Password: `root123` (change in docker-compose.yml)
- MongoDB: No authentication (add in production)

**Application:**
- Create your own accounts via registration

## Next Steps

1. ✅ Create an admin account
2. ✅ Test creating, editing, and deleting blogs
3. ✅ Test admin features (deleting other users' posts)
4. ✅ Explore the API documentation: https://localhost:5001/swagger
5. ✅ Read the full README.md for architecture details

## Production Checklist

Before deploying to production:

- [ ] Change all default passwords
- [ ] Use strong JWT secret (at least 32 characters)
- [ ] Enable HTTPS only
- [ ] Configure CORS for specific origins
- [ ] Set up proper database backups
- [ ] Add rate limiting
- [ ] Configure logging and monitoring
- [ ] Use environment variables for secrets
- [ ] Enable MongoDB authentication
- [ ] Set up CDN for media files
- [ ] Add API versioning
- [ ] Implement caching
- [ ] Add email verification
- [ ] Set up CI/CD pipeline

## Support

If you encounter issues:
1. Check the troubleshooting section above
2. Review logs in terminal/console
3. Check Docker container logs
4. Refer to README.md for detailed documentation
5. Review API_DOCUMENTATION.md for API details

Happy blogging! 🎉
