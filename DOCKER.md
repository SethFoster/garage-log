# Docker Setup for Garage Log

This guide explains how to run Garage Log using Docker.

## Prerequisites

- Docker Desktop for Windows
- Git (optional, for version control)

## Quick Start

### Option 1: Using the batch files (Easiest)

1. Double-click `start.bat` to build and start the app
2. Access the app at http://localhost:3000
3. Double-click `stop.bat` when you're done

### Option 2: Using Docker Compose directly

```bash
# Start the application
docker-compose up --build

# Stop the application (Ctrl+C, then)
docker-compose down
```

## What's Running?

- **Frontend (Next.js)**: http://localhost:3000
- **Backend (ASP.NET)**: http://localhost:5084
- **Database**: SQLite in a Docker volume (persists between restarts)

## Common Commands

```bash
# Start in background (detached mode)
docker-compose up -d

# View logs
docker-compose logs -f

# Stop containers
docker-compose down

# Stop and remove volumes (⚠️ deletes database)
docker-compose down -v

# Rebuild after code changes
docker-compose up --build

# Access backend container shell
docker-compose exec backend bash

# Access frontend container shell
docker-compose exec frontend sh
```

## Development Workflow

### Making Changes

1. Edit your code locally
2. Stop the containers: `docker-compose down`
3. Rebuild and restart: `docker-compose up --build`

### Database

- The database is stored in a Docker volume named `garage-log_backend-data`
- Data persists between container restarts
- To reset the database: `docker-compose down -v` (⚠️ this deletes all data)

### Running Locally Without Docker

You can still run the app locally as before:

```bash
# Terminal 1 - Backend
cd backend
dotnet run

# Terminal 2 - Frontend
cd frontend
npm run dev
```

## Troubleshooting

**Port already in use:**
- Make sure to stop your local dev servers (ports 3000 and 5084)
- Or change the ports in `docker-compose.yml`

**Changes not appearing:**
- Make sure to rebuild: `docker-compose up --build`
- Clear Docker cache: `docker-compose build --no-cache`

**Database issues:**
- Check volume: `docker volume ls`
- Reset database: `docker-compose down -v` then `docker-compose up`

**Can't connect to API:**
- Check both containers are running: `docker-compose ps`
- Check logs: `docker-compose logs backend`
