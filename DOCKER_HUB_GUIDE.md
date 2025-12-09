# Docker Hub Deployment Guide

This guide will help you containerize the Blog Application and publish it to Docker Hub.

---

## Table of Contents
1. [Prerequisites](#prerequisites)
2. [Creating Dockerfiles](#creating-dockerfiles)
3. [Building Docker Images](#building-docker-images)
4. [Testing Locally](#testing-locally)
5. [Pushing to Docker Hub](#pushing-to-docker-hub)
6. [Using Your Images](#using-your-images)
7. [CI/CD Integration](#cicd-integration)

---

## Prerequisites

### 1. Install Docker
- Download from https://www.docker.com/products/docker-desktop
- Verify installation:
```bash
docker --version
docker-compose --version
```

### 2. Create Docker Hub Account
- Go to https://hub.docker.com/
- Sign up for free account
- Remember your username

### 3. Login to Docker Hub
```bash
docker login
# Enter your Docker Hub username and password
```

---

## Creating Dockerfiles

### 1. Backend Dockerfile

Create `Dockerfile` in the root directory:

```dockerfile
# Use official .NET SDK image for build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["src/BlogApplication.API/BlogApplication.API.csproj", "src/BlogApplication.API/"]
COPY ["src/BlogApplication.Application/BlogApplication.Application.csproj", "src/BlogApplication.Application/"]
COPY ["src/BlogApplication.Domain/BlogApplication.Domain.csproj", "src/BlogApplication.Domain/"]
COPY ["src/BlogApplication.Infrastructure/BlogApplication.Infrastructure.csproj", "src/BlogApplication.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "src/BlogApplication.API/BlogApplication.API.csproj"

# Copy all source files
COPY . .

# Build the application
WORKDIR "/src/src/BlogApplication.API"
RUN dotnet build "BlogApplication.API.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "BlogApplication.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Create final runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 5205
EXPOSE 443

# Copy published files
COPY --from=publish /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:5205
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "BlogApplication.API.dll"]
```

### 2. Frontend Dockerfile

Create `Dockerfile` in the `client/` directory:

```dockerfile
# Build stage
FROM node:20-alpine AS build
WORKDIR /app

# Copy package files
COPY package*.json ./

# Install dependencies
RUN npm ci

# Copy source code
COPY . .

# Build the application
RUN npm run build

# Production stage with nginx
FROM nginx:alpine
WORKDIR /usr/share/nginx/html

# Remove default nginx content
RUN rm -rf ./*

# Copy built files from build stage
COPY --from=build /app/dist .

# Copy nginx configuration
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
```

### 3. Frontend Nginx Configuration

Create `client/nginx.conf`:

```nginx
server {
    listen 80;
    server_name localhost;
    root /usr/share/nginx/html;
    index index.html;

    location / {
        try_files $uri $uri/ /index.html;
    }

    location /api {
        proxy_pass http://backend:5205;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection 'upgrade';
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
```

### 4. Update docker-compose.yml

Update your existing `docker-compose.yml`:

```yaml
version: '3.8'

services:
  # MySQL Database
  mysql-db:
    image: mysql:8.0
    container_name: mysql-db-blogapp
    environment:
      MYSQL_ROOT_PASSWORD: root
      MYSQL_DATABASE: blogapp
    ports:
      - "3306:3306"
    volumes:
      - mysql-data:/var/lib/mysql
    networks:
      - blogapp-network
    healthcheck:
      test: ["CMD", "mysqladmin", "ping", "-h", "localhost"]
      timeout: 20s
      retries: 10

  # MongoDB Database
  mongodb:
    image: mongo:7.0
    container_name: mongo-dev
    environment:
      MONGO_INITDB_ROOT_USERNAME: root
      MONGO_INITDB_ROOT_PASSWORD: root
    ports:
      - "27017:27017"
    volumes:
      - mongo-data:/data/db
    networks:
      - blogapp-network
    healthcheck:
      test: echo 'db.runCommand("ping").ok' | mongosh localhost:27017/test --quiet
      interval: 10s
      timeout: 10s
      retries: 5

  # Backend API
  backend:
    build:
      context: .
      dockerfile: Dockerfile
    container_name: blogapp-api
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__DefaultConnection=Server=mysql-db;Database=blogapp;User=root;Password=root;
      - MongoDbSettings__ConnectionString=mongodb://root:root@mongodb:27017
      - MongoDbSettings__DatabaseName=BlogAppDb
      - JwtSettings__Secret=YourSuperSecretKeyThatIsAtLeast32CharactersLong!
      - JwtSettings__Issuer=BlogApplication
      - JwtSettings__Audience=BlogApplicationUsers
      - JwtSettings__ExpirationInMinutes=1440
    ports:
      - "5205:5205"
    depends_on:
      mysql-db:
        condition: service_healthy
      mongodb:
        condition: service_healthy
    networks:
      - blogapp-network

  # Frontend
  frontend:
    build:
      context: ./client
      dockerfile: Dockerfile
    container_name: blogapp-frontend
    ports:
      - "3000:80"
    depends_on:
      - backend
    networks:
      - blogapp-network

networks:
  blogapp-network:
    driver: bridge

volumes:
  mysql-data:
  mongo-data:
```

### 5. Create .dockerignore

Create `.dockerignore` in root directory:

```
# .NET
**/bin/
**/obj/
**/out/
*.user
*.userosscache
*.suo
*.cache
*.dll
*.exe
*.pdb
*.tmp

# Node
**/node_modules/
**/dist/
npm-debug.log*
yarn-debug.log*
yarn-error.log*

# IDE
.vscode/
.vs/
.idea/

# Git
.git/
.gitignore

# Documentation
*.md
!README.md

# Docker
**/.dockerignore
**/docker-compose*
**/Dockerfile*

# Other
.env
*.log
```

Create `client/.dockerignore`:

```
node_modules
dist
.env
npm-debug.log
yarn-debug.log
.git
.vscode
.idea
*.md
```

---

## Building Docker Images

### 1. Build Backend Image

```bash
# Build the backend image
docker build -t your-dockerhub-username/blogapp-api:latest -f Dockerfile .

# Example:
docker build -t johndoe/blogapp-api:latest -f Dockerfile .

# Build with specific version tag
docker build -t johndoe/blogapp-api:v1.0.0 -f Dockerfile .
```

### 2. Build Frontend Image

```bash
# Navigate to client directory and build
cd client
docker build -t your-dockerhub-username/blogapp-frontend:latest -f Dockerfile .

# Example:
docker build -t johndoe/blogapp-frontend:latest -f Dockerfile .

# Build with specific version tag
docker build -t johndoe/blogapp-frontend:v1.0.0 -f Dockerfile .
```

### 3. Build All Services with Docker Compose

```bash
# Build all images at once
docker-compose build

# Build specific service
docker-compose build backend
docker-compose build frontend
```

---

## Testing Locally

### 1. Run with Docker Compose

```bash
# Start all services
docker-compose up -d

# Check logs
docker-compose logs -f

# Check specific service logs
docker-compose logs -f backend
docker-compose logs -f frontend

# Check running containers
docker ps
```

### 2. Test the Application

- Frontend: http://localhost:3000
- Backend API: http://localhost:5205
- Swagger: http://localhost:5205/swagger

### 3. Verify Database Connections

```bash
# Connect to MySQL
docker exec -it mysql-db-blogapp mysql -uroot -proot

# Connect to MongoDB
docker exec -it mongo-dev mongosh -u root -p root
```

### 4. Stop Services

```bash
# Stop all services
docker-compose down

# Stop and remove volumes (clean slate)
docker-compose down -v
```

---

## Pushing to Docker Hub

### 1. Tag Your Images (if not already tagged)

```bash
# Tag backend
docker tag blogapp-api:latest your-dockerhub-username/blogapp-api:latest
docker tag blogapp-api:latest your-dockerhub-username/blogapp-api:v1.0.0

# Tag frontend
docker tag blogapp-frontend:latest your-dockerhub-username/blogapp-frontend:latest
docker tag blogapp-frontend:latest your-dockerhub-username/blogapp-frontend:v1.0.0
```

### 2. Push to Docker Hub

```bash
# Push backend images
docker push your-dockerhub-username/blogapp-api:latest
docker push your-dockerhub-username/blogapp-api:v1.0.0

# Push frontend images
docker push your-dockerhub-username/blogapp-frontend:latest
docker push your-dockerhub-username/blogapp-frontend:v1.0.0

# Example:
docker push johndoe/blogapp-api:latest
docker push johndoe/blogapp-api:v1.0.0
docker push johndoe/blogapp-frontend:latest
docker push johndoe/blogapp-frontend:v1.0.0
```

### 3. Create Public Repositories on Docker Hub

1. Go to https://hub.docker.com/
2. Click "Create Repository"
3. Create repositories:
   - `blogapp-api` (Backend)
   - `blogapp-frontend` (Frontend)
4. Set visibility to Public or Private

---

## Using Your Images

### Create docker-compose.production.yml

```yaml
version: '3.8'

services:
  # MySQL Database
  mysql-db:
    image: mysql:8.0
    container_name: mysql-db-blogapp
    environment:
      MYSQL_ROOT_PASSWORD: ${MYSQL_PASSWORD:-root}
      MYSQL_DATABASE: blogapp
    ports:
      - "3306:3306"
    volumes:
      - mysql-data:/var/lib/mysql
    networks:
      - blogapp-network
    restart: unless-stopped

  # MongoDB Database
  mongodb:
    image: mongo:7.0
    container_name: mongo-dev
    environment:
      MONGO_INITDB_ROOT_USERNAME: root
      MONGO_INITDB_ROOT_PASSWORD: ${MONGO_PASSWORD:-root}
    ports:
      - "27017:27017"
    volumes:
      - mongo-data:/data/db
    networks:
      - blogapp-network
    restart: unless-stopped

  # Backend API (from Docker Hub)
  backend:
    image: your-dockerhub-username/blogapp-api:latest
    container_name: blogapp-api
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__DefaultConnection=Server=mysql-db;Database=blogapp;User=root;Password=${MYSQL_PASSWORD:-root};
      - MongoDbSettings__ConnectionString=mongodb://root:${MONGO_PASSWORD:-root}@mongodb:27017
      - MongoDbSettings__DatabaseName=BlogAppDb
      - JwtSettings__Secret=${JWT_SECRET}
      - JwtSettings__Issuer=BlogApplication
      - JwtSettings__Audience=BlogApplicationUsers
      - JwtSettings__ExpirationInMinutes=1440
    ports:
      - "5205:5205"
    depends_on:
      - mysql-db
      - mongodb
    networks:
      - blogapp-network
    restart: unless-stopped

  # Frontend (from Docker Hub)
  frontend:
    image: your-dockerhub-username/blogapp-frontend:latest
    container_name: blogapp-frontend
    ports:
      - "80:80"
    depends_on:
      - backend
    networks:
      - blogapp-network
    restart: unless-stopped

networks:
  blogapp-network:
    driver: bridge

volumes:
  mysql-data:
  mongo-data:
```

### Deploy on Any Server

```bash
# Pull and run from Docker Hub
docker-compose -f docker-compose.production.yml pull
docker-compose -f docker-compose.production.yml up -d
```

---

## CI/CD Integration

### GitHub Actions Workflow

Create `.github/workflows/docker-publish.yml`:

```yaml
name: Build and Push Docker Images

on:
  push:
    branches: [ main ]
    tags: [ 'v*' ]
  pull_request:
    branches: [ main ]

env:
  DOCKERHUB_USERNAME: ${{ secrets.DOCKERHUB_USERNAME }}

jobs:
  build-backend:
    runs-on: ubuntu-latest
    steps:
      - name: Checkout code
        uses: actions/checkout@v3

      - name: Set up Docker Buildx
        uses: docker/setup-buildx-action@v2

      - name: Login to Docker Hub
        uses: docker/login-action@v2
        with:
          username: ${{ secrets.DOCKERHUB_USERNAME }}
          password: ${{ secrets.DOCKERHUB_TOKEN }}

      - name: Extract metadata
        id: meta
        uses: docker/metadata-action@v4
        with:
          images: ${{ secrets.DOCKERHUB_USERNAME }}/blogapp-api
          tags: |
            type=ref,event=branch
            type=ref,event=pr
            type=semver,pattern={{version}}
            type=semver,pattern={{major}}.{{minor}}
            type=raw,value=latest,enable={{is_default_branch}}

      - name: Build and push
        uses: docker/build-push-action@v4
        with:
          context: .
          file: ./Dockerfile
          push: true
          tags: ${{ steps.meta.outputs.tags }}
          labels: ${{ steps.meta.outputs.labels }}
          cache-from: type=registry,ref=${{ secrets.DOCKERHUB_USERNAME }}/blogapp-api:buildcache
          cache-to: type=registry,ref=${{ secrets.DOCKERHUB_USERNAME }}/blogapp-api:buildcache,mode=max

  build-frontend:
    runs-on: ubuntu-latest
    steps:
      - name: Checkout code
        uses: actions/checkout@v3

      - name: Set up Docker Buildx
        uses: docker/setup-buildx-action@v2

      - name: Login to Docker Hub
        uses: docker/login-action@v2
        with:
          username: ${{ secrets.DOCKERHUB_USERNAME }}
          password: ${{ secrets.DOCKERHUB_TOKEN }}

      - name: Extract metadata
        id: meta
        uses: docker/metadata-action@v4
        with:
          images: ${{ secrets.DOCKERHUB_USERNAME }}/blogapp-frontend
          tags: |
            type=ref,event=branch
            type=ref,event=pr
            type=semver,pattern={{version}}
            type=semver,pattern={{major}}.{{minor}}
            type=raw,value=latest,enable={{is_default_branch}}

      - name: Build and push
        uses: docker/build-push-action@v4
        with:
          context: ./client
          file: ./client/Dockerfile
          push: true
          tags: ${{ steps.meta.outputs.tags }}
          labels: ${{ steps.meta.outputs.labels }}
          cache-from: type=registry,ref=${{ secrets.DOCKERHUB_USERNAME }}/blogapp-frontend:buildcache
          cache-to: type=registry,ref=${{ secrets.DOCKERHUB_USERNAME }}/blogapp-frontend:buildcache,mode=max
```

### Setup GitHub Secrets

1. Go to your GitHub repository
2. Settings → Secrets and variables → Actions
3. Add secrets:
   - `DOCKERHUB_USERNAME`: Your Docker Hub username
   - `DOCKERHUB_TOKEN`: Docker Hub access token (create at https://hub.docker.com/settings/security)

---

## Complete Deployment Steps

### Step-by-Step Process:

```bash
# 1. Build images locally
docker build -t yourusername/blogapp-api:latest -f Dockerfile .
cd client
docker build -t yourusername/blogapp-frontend:latest -f Dockerfile .
cd ..

# 2. Test locally
docker-compose up -d
# Test the application
docker-compose down

# 3. Login to Docker Hub
docker login

# 4. Push images
docker push yourusername/blogapp-api:latest
docker push yourusername/blogapp-frontend:latest

# 5. Tag versions (optional)
docker tag yourusername/blogapp-api:latest yourusername/blogapp-api:v1.0.0
docker tag yourusername/blogapp-frontend:latest yourusername/blogapp-frontend:v1.0.0
docker push yourusername/blogapp-api:v1.0.0
docker push yourusername/blogapp-frontend:v1.0.0
```

---

## Production Deployment

### On Any Server with Docker:

```bash
# 1. Install Docker and Docker Compose
curl -fsSL https://get.docker.com -o get-docker.sh
sh get-docker.sh

# 2. Clone your repository (or just copy docker-compose.production.yml)
git clone https://github.com/yourusername/Blog-App.git
cd Blog-App

# 3. Create .env file
cat > .env << EOF
MYSQL_PASSWORD=your-secure-password
MONGO_PASSWORD=your-secure-password
JWT_SECRET=YourSuperSecretKeyThatIsAtLeast32CharactersLong!
EOF

# 4. Pull images and start
docker-compose -f docker-compose.production.yml pull
docker-compose -f docker-compose.production.yml up -d

# 5. Check logs
docker-compose -f docker-compose.production.yml logs -f
```

---

## Useful Docker Commands

```bash
# List all images
docker images

# Remove unused images
docker image prune -a

# View image details
docker inspect yourusername/blogapp-api:latest

# Pull specific version
docker pull yourusername/blogapp-api:v1.0.0

# Check image layers
docker history yourusername/blogapp-api:latest

# Export image to file
docker save -o blogapp-api.tar yourusername/blogapp-api:latest

# Import image from file
docker load -i blogapp-api.tar

# Tag and retag
docker tag oldtag:version newtag:version

# Remove specific image
docker rmi yourusername/blogapp-api:v1.0.0

# Check running containers
docker ps

# Check all containers
docker ps -a

# View container logs
docker logs -f container-name

# Execute command in container
docker exec -it container-name bash
```

---

## Best Practices

### 1. Multi-stage Builds
- Use multi-stage builds to reduce image size
- Separate build and runtime dependencies
- Already implemented in the Dockerfiles above

### 2. Image Tagging Strategy
```bash
# Always use semantic versioning
:latest          # Latest stable version
:v1.0.0          # Specific version
:v1.0            # Minor version
:v1              # Major version
:develop         # Development branch
:feature-xyz     # Feature branches
```

### 3. Security
- Don't include secrets in images
- Use environment variables
- Scan images for vulnerabilities:
```bash
docker scan yourusername/blogapp-api:latest
```

### 4. Optimization
- Minimize layers
- Use .dockerignore
- Order commands from least to most frequently changing
- Use caching effectively

### 5. Documentation
- Add labels to images:
```dockerfile
LABEL maintainer="your-email@example.com"
LABEL version="1.0.0"
LABEL description="Blog Application API"
```

---

## Troubleshooting

### Image Build Fails
```bash
# Clear build cache
docker builder prune -a

# Build with no cache
docker build --no-cache -t yourusername/blogapp-api:latest .
```

### Push Fails (Authentication)
```bash
# Logout and login again
docker logout
docker login
```

### Image Too Large
```bash
# Check image size
docker images

# Analyze layers
docker history yourusername/blogapp-api:latest

# Use dive tool for detailed analysis
docker run --rm -it -v /var/run/docker.sock:/var/run/docker.sock wagoodman/dive yourusername/blogapp-api:latest
```

### Container Won't Start
```bash
# Check logs
docker logs container-name

# Inspect container
docker inspect container-name

# Try running interactively
docker run -it yourusername/blogapp-api:latest sh
```

---

## Next Steps

1. **Add Health Checks** to Dockerfiles
2. **Implement Container Orchestration** (Kubernetes, Docker Swarm)
3. **Set up Monitoring** (Prometheus, Grafana)
4. **Add Logging** (ELK Stack, Fluentd)
5. **Implement CI/CD** pipeline
6. **Set up Reverse Proxy** (Nginx, Traefik)
7. **Add SSL Certificates** (Let's Encrypt)
8. **Configure Auto-scaling**

---

## Resources

- [Docker Documentation](https://docs.docker.com/)
- [Docker Hub](https://hub.docker.com/)
- [Docker Best Practices](https://docs.docker.com/develop/dev-best-practices/)
- [.NET Docker Images](https://hub.docker.com/_/microsoft-dotnet)
- [Node Docker Images](https://hub.docker.com/_/node)

---

## Summary

Your images will be available at:
- Backend: `docker pull yourusername/blogapp-api:latest`
- Frontend: `docker pull yourusername/blogapp-frontend:latest`

Anyone can now deploy your application with a single command:
```bash
docker-compose -f docker-compose.production.yml up -d
```

Good luck with your Docker deployment! 🐳
