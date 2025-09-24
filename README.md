# Project Dotaboost

A clean-architecture full-stack web application for creating and managing rating boosting orders in online games.  
Supports order tracking, booster and customer roles, admin management, and secure authentication/authorization.

---

## Navigation

* [Features](#features)  
* [Tech Stack](#tech-stack)  
* [Getting Started](#getting-started)  
* [Frontend Demo](#frontend-demo)  
* [Project Structure](#project-structure)  

---

## Features

* Secure authentication & role-based authorization  
* Booster application workflow  
* Order management system based on game batches  
* Fully dockerized backend and frontend


---

## Tech Stack

* **Backend:** ASP.NET 9, Entity Framework, Identity, MediatR  
* **Database:** PostgreSQL 16  
* **Frontend:** Vue 3  
* **Containerization:** Docker, Docker Compose  

---

## Getting Started

### Environment Setup

1. Copy the example `.env` file in root directory:

```bash
cp backend/src/WebApi/.env.Example backend/src/WebApi/.env
```

2. Update the `.env` files with your environment-specific variables (database credentials, JWT secret, etc.).

---

### Running with Docker

Build and start both backend and frontend containers:

```bash
docker compose up --build
```

The app should be accessible at `http://localhost:8080` (frontend) and backend API at `http://localhost:5001` (default port).

---


## Frontend Demo
<img width="2527" height="1291" alt="screen1" src="https://github.com/user-attachments/assets/6cf7bfac-c9f9-4eb9-8748-21d255435520" />
<img width="2538" height="1290" alt="screen3" src="https://github.com/user-attachments/assets/66743907-2f5e-43c0-b853-28513dc092dc" />
<img width="2516" height="1238" alt="screen4" src="https://github.com/user-attachments/assets/6e87004a-5aa2-4c1c-8e76-a6426d956135" />


---

## Project Structure



```
root/
├─ backend/               # ASP.NET backend
├─ frontend/              # Vue frontend
├─ docker-compose.yml     # Docker compose setup
├─ .env.example           # Backend environment variables example
```

---
