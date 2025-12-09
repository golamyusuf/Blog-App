# API Documentation

## Base URL

```
http://localhost:5000/api
https://localhost:5001/api
```

## Authentication

All authenticated endpoints require a Bearer token in the Authorization header:

```
Authorization: Bearer <your_jwt_token>
```

---

## Auth Endpoints

### Register User

**POST** `/auth/register`

Register a new user account.

**Request Body:**
```json
{
  "username": "johndoe",
  "email": "john@example.com",
  "password": "Password123",
  "firstName": "John",
  "lastName": "Doe"
}
```

**Validation Rules:**
- Username: minimum 3 characters, maximum 50 characters
- Email: valid email format
- Password: minimum 6 characters, must contain uppercase, lowercase, and number

**Response:** `200 OK`
```json
{
  "id": 1,
  "username": "johndoe",
  "email": "john@example.com",
  "firstName": "John",
  "lastName": "Doe",
  "roles": ["User"],
  "createdAt": "2024-01-01T00:00:00Z"
}
```

---

### Login

**POST** `/auth/login`

Authenticate and receive JWT token.

**Request Body:**
```json
{
  "email": "john@example.com",
  "password": "Password123"
}
```

**Response:** `200 OK`
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": 1,
    "username": "johndoe",
    "email": "john@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "roles": ["User"],
    "createdAt": "2024-01-01T00:00:00Z"
  }
}
```

---

## Blog Endpoints

### Get All Blogs

**GET** `/blogs?pageNumber=1&pageSize=10&searchTerm=tech`

Get paginated list of published blogs.

**Query Parameters:**
- `pageNumber` (optional, default: 1)
- `pageSize` (optional, default: 10)
- `searchTerm` (optional) - Search in title, content, and tags
- `userId` (optional) - Filter by user ID

**Response:** `200 OK`
```json
{
  "blogs": [
    {
      "id": "507f1f77bcf86cd799439011",
      "userId": 1,
      "username": "johndoe",
      "title": "Introduction to Clean Architecture",
      "content": "Full blog content here...",
      "summary": "A brief introduction to Clean Architecture principles",
      "tags": ["architecture", "dotnet", "clean-code"],
      "mediaItems": [
        {
          "url": "https://example.com/image.jpg",
          "type": "Image",
          "caption": "Architecture diagram",
          "order": 0
        }
      ],
      "viewCount": 150,
      "isPublished": true,
      "createdAt": "2024-01-01T00:00:00Z",
      "publishedAt": "2024-01-01T00:00:00Z"
    }
  ],
  "totalCount": 25,
  "pageNumber": 1,
  "pageSize": 10,
  "totalPages": 3
}
```

---

### Get Blog by ID

**GET** `/blogs/{id}`

Get a single blog post. Increments view count.

**Response:** `200 OK`
```json
{
  "id": "507f1f77bcf86cd799439011",
  "userId": 1,
  "username": "johndoe",
  "title": "Introduction to Clean Architecture",
  "content": "Full blog content here...",
  "summary": "A brief introduction",
  "tags": ["architecture", "dotnet"],
  "mediaItems": [],
  "viewCount": 151,
  "isPublished": true,
  "createdAt": "2024-01-01T00:00:00Z",
  "updatedAt": "2024-01-02T00:00:00Z",
  "publishedAt": "2024-01-01T00:00:00Z"
}
```

---

### Get My Blogs

**GET** `/blogs/my-blogs?pageNumber=1&pageSize=10`

🔒 **Requires Authentication**

Get current user's blogs (including drafts).

**Response:** Same as Get All Blogs

---

### Create Blog

**POST** `/blogs`

🔒 **Requires Authentication**

Create a new blog post.

**Request Body:**
```json
{
  "title": "My New Blog Post",
  "content": "This is the content of my blog post with at least 50 characters...",
  "summary": "Brief summary",
  "tags": ["technology", "programming"],
  "mediaItems": [
    {
      "url": "https://example.com/image.jpg",
      "type": "Image",
      "caption": "Optional caption",
      "order": 0
    }
  ],
  "isPublished": true
}
```

**Validation Rules:**
- Title: required, maximum 200 characters
- Content: required, minimum 50 characters
- Summary: optional, maximum 500 characters
- Tags: maximum 10 tags
- MediaItems: optional array

**Response:** `201 Created`
```json
{
  "id": "507f1f77bcf86cd799439011",
  "userId": 1,
  "username": "johndoe",
  "title": "My New Blog Post",
  ...
}
```

---

### Update Blog

**PUT** `/blogs/{id}`

🔒 **Requires Authentication** (Owner only)

Update an existing blog post.

**Request Body:** Same as Create Blog

**Response:** `200 OK` - Returns updated blog

---

### Delete Blog

**DELETE** `/blogs/{id}`

🔒 **Requires Authentication** (Owner or Admin)

Delete a blog post.

**Response:** `204 No Content`

---

## Admin Endpoints

### Delete Blog (Admin)

**DELETE** `/admin/blogs/{id}`

🔒 **Requires Admin Role**

Delete any blog post (for content moderation).

**Response:** `204 No Content`

---

## Error Responses

### 400 Bad Request
```json
{
  "message": "Validation error",
  "errors": [
    "Title is required",
    "Content must be at least 50 characters"
  ]
}
```

### 401 Unauthorized
```json
{
  "error": "Unauthorized",
  "statusCode": 401
}
```

### 404 Not Found
```json
{
  "error": "Blog not found",
  "statusCode": 404
}
```

### 500 Internal Server Error
```json
{
  "error": "An error occurred while processing your request",
  "statusCode": 500
}
```

---

## Rate Limiting

Currently no rate limiting is implemented. Consider implementing in production.

## Pagination

All list endpoints support pagination:
- Default page size: 10
- Maximum page size: 100
- Page numbers start at 1

## Media Types

Supported media types in blog posts:
- **Image**: JPG, PNG, GIF, WebP
- **Video**: MP4, WebM

Media files should be hosted externally (e.g., CDN, cloud storage) and only URLs are stored.

## Status Codes

- `200 OK` - Successful GET/PUT request
- `201 Created` - Successful POST request
- `204 No Content` - Successful DELETE request
- `400 Bad Request` - Validation error or bad input
- `401 Unauthorized` - Missing or invalid token
- `403 Forbidden` - Insufficient permissions
- `404 Not Found` - Resource not found
- `500 Internal Server Error` - Server error
