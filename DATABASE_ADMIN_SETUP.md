# Database Queries for Admin User Setup

This file contains SQL queries to manually create and manage admin users in the MySQL database.

---

## Prerequisites

1. Ensure MySQL is running
2. Connect to the database using MySQL Workbench, command line, or any MySQL client
3. Select the `blogapp` database

```sql
USE blogapp;
```

---

## Option 1: Quick Admin Creation (Recommended)

This creates an admin user with default credentials. **Change the password after first login!**

```sql
-- Step 1: Insert the admin user
INSERT INTO Users (Username, Email, PasswordHash, FirstName, LastName, IsActive, CreatedAt)
VALUES (
    'admin',
    'admin@blogapp.com',
    '$2a$11$3kVZxQYJ5LH.YzKQvF5lVe8F5f7nYZxF6KwH3d8RYnG5kQZYH5fKu',  -- Password: Admin@123
    'System',
    'Administrator',
    1,
    UTC_TIMESTAMP()
);

-- Step 2: Get the newly created user ID
SET @adminUserId = LAST_INSERT_ID();

-- Step 3: Assign Admin role (Role ID 2 = Admin)
INSERT INTO UserRoles (UserId, RoleId, AssignedAt)
VALUES (@adminUserId, 2, UTC_TIMESTAMP());

-- Step 4: Also assign User role (Role ID 1 = User) for full access
INSERT INTO UserRoles (UserId, RoleId, AssignedAt)
VALUES (@adminUserId, 1, UTC_TIMESTAMP());

-- Step 5: Verify the admin user was created
SELECT 
    u.Id,
    u.Username,
    u.Email,
    u.FirstName,
    u.LastName,
    GROUP_CONCAT(r.Name) as Roles
FROM Users u
LEFT JOIN UserRoles ur ON u.Id = ur.UserId
LEFT JOIN Roles r ON ur.RoleId = r.RoleId
WHERE u.Email = 'admin@blogapp.com'
GROUP BY u.Id;
```

**Default Credentials:**
- Email: `admin@blogapp.com`
- Password: `Admin@123`

---

## Option 2: Create Admin with Custom Credentials

If you want to use different credentials, you'll need to generate a BCrypt hash for your password.

### Using Online BCrypt Generator:
1. Go to https://bcrypt-generator.com/
2. Enter your desired password
3. Use 11 rounds (default in the app)
4. Copy the generated hash

### Then run:
```sql
-- Replace with your custom values
INSERT INTO Users (Username, Email, PasswordHash, FirstName, LastName, IsActive, CreatedAt)
VALUES (
    'your_username',           -- Change this
    'your_email@example.com',  -- Change this
    'YOUR_BCRYPT_HASH_HERE',   -- Paste BCrypt hash here
    'Your',                    -- Change this
    'Name',                    -- Change this
    1,
    UTC_TIMESTAMP()
);

SET @adminUserId = LAST_INSERT_ID();

-- Assign Admin role
INSERT INTO UserRoles (UserId, RoleId, AssignedAt)
VALUES (@adminUserId, 2, UTC_TIMESTAMP());

-- Assign User role
INSERT INTO UserRoles (UserId, RoleId, AssignedAt)
VALUES (@adminUserId, 1, UTC_TIMESTAMP());
```

---

## Option 3: Promote Existing User to Admin

If you already have a user account and want to make it an admin:

```sql
-- Find the user ID by email
SELECT Id, Username, Email FROM Users WHERE Email = 'user@example.com';

-- Assign Admin role (replace USER_ID with actual ID)
INSERT INTO UserRoles (UserId, RoleId, AssignedAt)
VALUES (USER_ID, 2, UTC_TIMESTAMP());

-- Example: Make user with ID 5 an admin
INSERT INTO UserRoles (UserId, RoleId, AssignedAt)
VALUES (5, 2, UTC_TIMESTAMP());
```

---

## Verification Queries

### Check if admin user exists:
```sql
SELECT 
    u.Id,
    u.Username,
    u.Email,
    u.IsActive,
    GROUP_CONCAT(r.Name) as Roles
FROM Users u
LEFT JOIN UserRoles ur ON u.Id = ur.UserId
LEFT JOIN Roles r ON ur.RoleId = r.RoleId
WHERE u.Email = 'admin@blogapp.com'
GROUP BY u.Id;
```

### List all admin users:
```sql
SELECT 
    u.Id,
    u.Username,
    u.Email,
    u.CreatedAt
FROM Users u
INNER JOIN UserRoles ur ON u.Id = ur.UserId
INNER JOIN Roles r ON ur.RoleId = r.RoleId
WHERE r.Name = 'Admin';
```

### View all users with their roles:
```sql
SELECT 
    u.Id,
    u.Username,
    u.Email,
    u.IsActive,
    GROUP_CONCAT(r.Name ORDER BY r.Name) as Roles,
    u.CreatedAt
FROM Users u
LEFT JOIN UserRoles ur ON u.Id = ur.UserId
LEFT JOIN Roles r ON ur.RoleId = r.RoleId
GROUP BY u.Id, u.Username, u.Email, u.IsActive, u.CreatedAt
ORDER BY u.CreatedAt DESC;
```

---

## Managing Admin Users

### Remove admin role from user:
```sql
-- Replace USER_ID with actual ID
DELETE FROM UserRoles 
WHERE UserId = USER_ID AND RoleId = 2;

-- Example: Remove admin role from user with ID 5
DELETE FROM UserRoles 
WHERE UserId = 5 AND RoleId = 2;
```

### Deactivate admin user (without deleting):
```sql
UPDATE Users 
SET IsActive = 0 
WHERE Email = 'admin@blogapp.com';
```

### Reactivate admin user:
```sql
UPDATE Users 
SET IsActive = 1 
WHERE Email = 'admin@blogapp.com';
```

### Change admin email:
```sql
UPDATE Users 
SET Email = 'newemail@example.com' 
WHERE Email = 'admin@blogapp.com';
```

### Delete admin user completely (cascade will remove roles):
```sql
-- BE CAREFUL! This permanently deletes the user
DELETE FROM Users WHERE Email = 'admin@blogapp.com';
```

---

## Troubleshooting

### Check if Roles table is populated:
```sql
SELECT * FROM Roles;
```

Expected output:
```
Id | Name  | Description    | CreatedAt
1  | User  | Regular user   | ...
2  | Admin | Administrator  | ...
```

If empty, run:
```sql
INSERT INTO Roles (Id, Name, Description, CreatedAt) VALUES
(1, 'User', 'Regular user', UTC_TIMESTAMP()),
(2, 'Admin', 'Administrator', UTC_TIMESTAMP());
```

### Check table structure:
```sql
DESCRIBE Users;
DESCRIBE Roles;
DESCRIBE UserRoles;
```

### View all relationships:
```sql
SELECT 
    ur.UserId,
    u.Username,
    ur.RoleId,
    r.Name as RoleName,
    ur.AssignedAt
FROM UserRoles ur
JOIN Users u ON ur.UserId = u.Id
JOIN Roles r ON ur.RoleId = r.RoleId;
```

---

## Password Hash Reference

The password hash in Option 1 is generated using BCrypt with 11 rounds:
- **Password**: `Admin@123`
- **Hash**: `$2a$11$3kVZxQYJ5LH.YzKQvF5lVe8F5f7nYZxF6KwH3d8RYnG5kQZYH5fKu`

**Important Security Notes:**
1. Always change default passwords in production
2. Use strong passwords with:
   - At least 8 characters
   - Uppercase and lowercase letters
   - Numbers
   - Special characters
3. Never share admin credentials
4. Use different passwords for different environments (dev, staging, production)

---

## Alternative: Using .NET Application Seeding

The application automatically seeds an admin user on startup if it doesn't exist.

**Automatic Seeding Details:**
- **Location**: `src/BlogApplication.Infrastructure/Data/DbInitializer.cs`
- **Triggered**: On application startup in `Program.cs`
- **Default Credentials**: Same as Option 1 above

If you prefer automatic seeding:
1. Simply run the application: `dotnet run`
2. The `DbInitializer.SeedAdminUser` method will create the admin automatically
3. No SQL queries needed!

To modify the seeded admin user:
1. Edit `DbInitializer.cs`
2. Change the `Username`, `Email`, or `Password` values
3. Restart the application

---

## Connection to Database

### Using MySQL Command Line:
```bash
mysql -u root -p
USE blogapp;
# Then run queries from above
```

### Using MySQL Workbench:
1. Open MySQL Workbench
2. Connect to your server
3. Select `blogapp` schema
4. Open a new SQL tab
5. Paste and execute queries

### Using Docker (if running MySQL in container):
```bash
# Connect to MySQL container
docker exec -it mysql-db-blogapp mysql -uroot -p

# Enter password: root
# Then: USE blogapp;
```

---

## Quick Reference

| Task | Query |
|------|-------|
| Create admin | Option 1 queries above |
| Check admin exists | `SELECT * FROM Users WHERE Email = 'admin@blogapp.com'` |
| List all admins | See "List all admin users" section |
| Make user admin | See "Promote Existing User" section |
| Remove admin role | `DELETE FROM UserRoles WHERE UserId = X AND RoleId = 2` |
| Change password | Generate new BCrypt hash, then `UPDATE Users SET PasswordHash = 'hash' WHERE Id = X` |

---

## Notes

- The application uses **MySQL** for user/role data
- The application uses **MongoDB** for blog posts
- Role IDs are fixed: 1 = User, 2 = Admin
- UserRoles is a junction table (many-to-many relationship)
- A user can have multiple roles
- The application checks roles via JWT token claims
- Admin users have access to:
  - `/admin` route
  - `/admin/categories` route
  - User management endpoints
  - Category management
  - All regular user features

---

For more information, see:
- `LEARNING_GUIDE.md` - Complete project learning guide
- `src/BlogApplication.Infrastructure/Data/DbInitializer.cs` - Automatic admin seeding
- `src/BlogApplication.Infrastructure/Services/AuthService.cs` - Authentication logic
