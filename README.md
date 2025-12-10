# TalentoPlus

**Human Resources Management System**

A full-stack enterprise application for managing employees, departments, and HR operations. Built with .NET 8, PostgreSQL, and modern web technologies.

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-4169E1?logo=postgresql)](https://www.postgresql.org/)
[![Docker](https://img.shields.io/badge/Docker-Ready-2496ED?logo=docker)](https://www.docker.com/)
[![License](https://img.shields.io/badge/License-MIT-green)](LICENSE)

---

**Language / Idioma:**
- [English](#english)
- [Español](#español)

---

# English

## Table of Contents

1. [Overview](#overview)
2. [Architecture](#architecture)
3. [Features](#features)
4. [Tech Stack](#tech-stack)
5. [Getting Started](#getting-started)
   - [Prerequisites](#prerequisites)
   - [Running with Docker](#running-with-docker)
   - [Running Locally](#running-locally)
6. [Configuration](#configuration)
7. [API Documentation](#api-documentation)
8. [Authentication Flow](#authentication-flow)
9. [Testing](#testing)
10. [Project Structure](#project-structure)

---

## Overview

TalentoPlus is a comprehensive human resources management solution consisting of two main components:

- **Web Portal (MVC)**: Administrative interface for HR managers to manage employees, departments, and generate reports.
- **REST API**: Secure endpoint for employees to access their personal information, download their resume, and update contact details.

---

## Architecture

The project follows **Clean Architecture** principles with clear separation of concerns:

```
TalentoPlus/
├── TalentoPlus.Core/           # Domain layer: Entities, Enums, Interfaces, DTOs
├── TalentoPlus.Infrastructure/ # Data layer: DbContext, Repositories, External Services
├── TalentoPlus.Web/            # Presentation layer: MVC Admin Portal
├── TalentoPlus.API/            # Presentation layer: REST API for employees
└── TalentoPlus.Tests/          # Test layer: Unit and Integration tests
```

### Design Patterns Used

- **Repository Pattern**: Abstracts data access logic
- **Dependency Injection**: All services are injected via constructor
- **DTO Pattern**: Separates domain entities from API contracts
- **Unit of Work**: Implicit through EF Core's DbContext

---

## Features

### Web Portal (Admin)

| Feature | Description |
|---------|-------------|
| Employee Management | Full CRUD operations with validation |
| Department Management | Create and manage organizational units |
| Excel Import | Bulk employee upload from spreadsheet files |
| PDF Generation | Professional resume generation for each employee |
| AI Assistant | Natural language queries about employee data |
| Role-based Access | Admin-only access with ASP.NET Core Identity |
| Dashboard | Statistics and charts for HR metrics |

### REST API (Employees)

| Feature | Description |
|---------|-------------|
| Self-service Portal | Employees can view their own information |
| PDF Download | Download personal resume in PDF format |
| Contact Update | Update phone number and address |
| JWT Authentication | Secure token-based authentication |

---

## Tech Stack

| Layer | Technology |
|-------|------------|
| Framework | .NET 8 |
| Database | PostgreSQL 16 |
| ORM | Entity Framework Core 8 |
| Web Authentication | ASP.NET Core Identity + Cookies |
| API Authentication | JWT Bearer Tokens |
| PDF Generation | QuestPDF |
| Excel Processing | EPPlus |
| Email Service | MailKit |
| AI Integration | OpenAI GPT-4o-mini |
| Containerization | Docker + Docker Compose |

---

## Getting Started

### Prerequisites

**For Docker deployment:**
- Docker Engine 20.10+
- Docker Compose 2.0+

**For local development:**
- .NET 8 SDK
- PostgreSQL 16
- Git

### Running with Docker

This is the recommended method for quick setup.

1. **Clone the repository**

```bash
git clone https://github.com/Brahiamriwi/TalentoPlus.git
cd TalentoPlus
```

2. **Create environment file**

```bash
cp .env.example .env
```

3. **Edit the `.env` file with your values**

```env
# Database
POSTGRES_USER=postgres
POSTGRES_PASSWORD=your_secure_password
POSTGRES_DB=TalentoPlusDb

# OpenAI (for AI assistant)
OPENAI_API_KEY=sk-your-api-key

# JWT (for API authentication)
JWT_KEY=your-secret-key-minimum-32-characters

# SMTP (for email notifications)
SMTP_HOST=smtp.gmail.com
SMTP_PORT=587
SMTP_USER=your-email@gmail.com
SMTP_PASSWORD=your-app-password
```

4. **Build and start containers**

```bash
docker-compose up --build -d
```

5. **Verify services are running**

```bash
docker-compose ps
```

6. **Access the applications**

| Service | URL |
|---------|-----|
| Web Portal | http://localhost:5001 |
| API (Swagger) | http://localhost:5002/swagger |

7. **View logs**

```bash
docker-compose logs -f
```

8. **Stop services**

```bash
docker-compose down
```

### Running Locally

For development without Docker.

1. **Clone the repository**

```bash
git clone https://github.com/Brahiamriwi/TalentoPlus.git
cd TalentoPlus
```

2. **Create PostgreSQL database**

```sql
CREATE DATABASE "TalentoPlusDb";
```

3. **Configure application settings**

Create `appsettings.Development.json` in both `TalentoPlus.Web` and `TalentoPlus.API` folders:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=TalentoPlusDb;Username=postgres;Password=your_password"
  },
  "OpenAI": {
    "ApiKey": "sk-your-api-key"
  },
  "Jwt": {
    "Key": "your-secret-key-minimum-32-characters",
    "Issuer": "TalentoPlusAPI",
    "Audience": "TalentoPlusUsers"
  },
  "Smtp": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "User": "your-email@gmail.com",
    "Password": "your-app-password"
  }
}
```

4. **Apply database migrations**

```bash
dotnet ef database update --project TalentoPlus.Infrastructure --startup-project TalentoPlus.Web
```

5. **Run the Web Portal**

```bash
cd TalentoPlus.Web
dotnet run
```

Access at: http://localhost:5076

6. **Run the API (in a separate terminal)**

```bash
cd TalentoPlus.API
dotnet run
```

Access Swagger at: http://localhost:5226/swagger

---

## Configuration

### Environment Variables

| Variable | Description | Required |
|----------|-------------|----------|
| `POSTGRES_USER` | PostgreSQL username | Yes |
| `POSTGRES_PASSWORD` | PostgreSQL password | Yes |
| `POSTGRES_DB` | Database name | Yes |
| `OPENAI_API_KEY` | OpenAI API key for AI assistant | Yes |
| `JWT_KEY` | Secret key for JWT tokens (min. 32 chars) | Yes |
| `JWT_ISSUER` | JWT token issuer | No |
| `JWT_AUDIENCE` | JWT token audience | No |
| `SMTP_HOST` | SMTP server hostname | Yes |
| `SMTP_PORT` | SMTP server port | Yes |
| `SMTP_USER` | SMTP username/email | Yes |
| `SMTP_PASSWORD` | SMTP password or app password | Yes |

### Default Admin Credentials

On first run, the system creates an administrator account:

| Field | Value |
|-------|-------|
| Email | admin@talentoplus.com |
| Password | Admin123* |
| Role | Administrator |

**Important:** Change the admin password immediately in production environments.

---

## API Documentation

### Public Endpoints (No authentication required)

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/departamentos` | List all departments |
| GET | `/api/departamentos/{id}` | Get department by ID |
| POST | `/api/auth/register` | Request account credentials |
| POST | `/api/auth/login` | Authenticate and receive JWT token |

### Protected Endpoints (JWT required)

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/empleado/me` | Get authenticated employee's information |
| GET | `/api/empleado/me/pdf` | Download resume as PDF |
| PUT | `/api/empleado/me/contact` | Update address and phone number |

### Request/Response Examples

**Register (Request credentials)**

```http
POST /api/auth/register
Content-Type: application/json

{
  "document": "1234567890"
}
```

Response:
```json
{
  "message": "Account created. Credentials sent to j***@example.com"
}
```

**Login**

```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "john@example.com",
  "password": "received_password"
}
```

Response:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiration": "2025-12-10T12:00:00Z",
  "email": "john@example.com",
  "fullName": "John Doe"
}
```

**Get Employee Information (Protected)**

```http
GET /api/empleado/me
Authorization: Bearer {token}
```

Response:
```json
{
  "id": 1,
  "document": "1234567890",
  "firstName": "John",
  "lastName": "Doe",
  "email": "john@example.com",
  "position": "Software Developer",
  "department": "Technology",
  "status": "Active"
}
```

---

## Authentication Flow

The API implements a secure authentication flow where passwords are never chosen by users:

1. **Admin creates employee** in the Web Portal with their personal information
2. **Employee requests access** by calling `/api/auth/register` with their document number
3. **System validates** the employee exists in the database
4. **System generates** a secure random password
5. **System sends** the password to the employee's registered email
6. **Employee logs in** using their email and the received password

This flow prevents account theft since credentials are only sent to the email registered by HR.

```
┌─────────────┐     ┌─────────────┐     ┌─────────────┐
│   Admin     │     │   System    │     │  Employee   │
└──────┬──────┘     └──────┬──────┘     └──────┬──────┘
       │                   │                   │
       │ Create employee   │                   │
       │──────────────────>│                   │
       │                   │                   │
       │                   │   POST /register  │
       │                   │<──────────────────│
       │                   │                   │
       │                   │ Send password     │
       │                   │ via email         │
       │                   │──────────────────>│
       │                   │                   │
       │                   │   POST /login     │
       │                   │<──────────────────│
       │                   │                   │
       │                   │   JWT Token       │
       │                   │──────────────────>│
       │                   │                   │
```

---

## Testing

The project includes unit and integration tests.

**Run all tests:**

```bash
dotnet test
```

**Run with coverage:**

```bash
dotnet test --collect:"XPlat Code Coverage"
```

**Test categories:**

| Category | Description |
|----------|-------------|
| Unit Tests | Repository and service logic tests |
| Integration Tests | Controller endpoint tests |

---

## Project Structure

```
TalentoPlus/
│
├── TalentoPlus.Core/
│   ├── DTOs/                    # Data Transfer Objects
│   ├── Entities/                # Domain entities (Employee, Department)
│   ├── Enums/                   # Enumerations (EmployeeStatus, EducationLevel)
│   └── Interfaces/              # Repository and service contracts
│
├── TalentoPlus.Infrastructure/
│   ├── Data/
│   │   └── ApplicationDbContext.cs
│   ├── Repositories/            # Repository implementations
│   └── Services/                # External service implementations
│       ├── EmailService.cs
│       ├── PdfService.cs
│       ├── ExcelService.cs
│       └── OpenAIService.cs
│
├── TalentoPlus.Web/
│   ├── Controllers/             # MVC Controllers
│   ├── Views/                   # Razor views
│   ├── Models/                  # View models
│   └── wwwroot/                 # Static files (CSS, JS)
│
├── TalentoPlus.API/
│   └── Controllers/             # API Controllers
│       ├── AuthController.cs
│       ├── EmpleadoController.cs
│       └── DepartamentosController.cs
│
├── TalentoPlus.Tests/
│   ├── UnitTests/
│   └── IntegrationTests/
│
├── docker-compose.yml
├── .env.example
└── README.md
```

---

# Español

## Tabla de Contenidos

1. [Descripción General](#descripción-general)
2. [Arquitectura](#arquitectura-1)
3. [Funcionalidades](#funcionalidades)
4. [Stack Tecnológico](#stack-tecnológico)
5. [Inicio Rápido](#inicio-rápido)
   - [Prerrequisitos](#prerrequisitos-1)
   - [Ejecución con Docker](#ejecución-con-docker)
   - [Ejecución Local](#ejecución-local)
6. [Configuración](#configuración)
7. [Documentación de la API](#documentación-de-la-api)
8. [Flujo de Autenticación](#flujo-de-autenticación)
9. [Pruebas](#pruebas)
10. [Estructura del Proyecto](#estructura-del-proyecto)

---

## Descripción General

TalentoPlus es una solución integral para la gestión de recursos humanos compuesta por dos componentes principales:

- **Portal Web (MVC)**: Interfaz administrativa para que los gestores de RRHH administren empleados, departamentos y generen reportes.
- **API REST**: Endpoint seguro para que los empleados accedan a su información personal, descarguen su hoja de vida y actualicen sus datos de contacto.

---

## Arquitectura

El proyecto sigue los principios de **Clean Architecture** con clara separación de responsabilidades:

```
TalentoPlus/
├── TalentoPlus.Core/           # Capa de dominio: Entidades, Enums, Interfaces, DTOs
├── TalentoPlus.Infrastructure/ # Capa de datos: DbContext, Repositorios, Servicios externos
├── TalentoPlus.Web/            # Capa de presentación: Portal Admin MVC
├── TalentoPlus.API/            # Capa de presentación: API REST para empleados
└── TalentoPlus.Tests/          # Capa de pruebas: Tests unitarios e integración
```

### Patrones de Diseño Utilizados

- **Repository Pattern**: Abstrae la lógica de acceso a datos
- **Dependency Injection**: Todos los servicios se inyectan vía constructor
- **DTO Pattern**: Separa las entidades de dominio de los contratos de API
- **Unit of Work**: Implícito a través del DbContext de EF Core

---

## Funcionalidades

### Portal Web (Administrador)

| Funcionalidad | Descripción |
|---------------|-------------|
| Gestión de Empleados | Operaciones CRUD completas con validación |
| Gestión de Departamentos | Crear y administrar unidades organizacionales |
| Importación Excel | Carga masiva de empleados desde archivos de hoja de cálculo |
| Generación de PDF | Generación de hojas de vida profesionales para cada empleado |
| Asistente IA | Consultas en lenguaje natural sobre datos de empleados |
| Control de Acceso | Acceso solo para administradores con ASP.NET Core Identity |
| Dashboard | Estadísticas y gráficos de métricas de RRHH |

### API REST (Empleados)

| Funcionalidad | Descripción |
|---------------|-------------|
| Portal de Autoservicio | Los empleados pueden ver su propia información |
| Descarga de PDF | Descargar hoja de vida personal en formato PDF |
| Actualización de Contacto | Actualizar número de teléfono y dirección |
| Autenticación JWT | Autenticación segura basada en tokens |

---

## Stack Tecnológico

| Capa | Tecnología |
|------|------------|
| Framework | .NET 8 |
| Base de Datos | PostgreSQL 16 |
| ORM | Entity Framework Core 8 |
| Autenticación Web | ASP.NET Core Identity + Cookies |
| Autenticación API | JWT Bearer Tokens |
| Generación de PDF | QuestPDF |
| Procesamiento Excel | EPPlus |
| Servicio de Email | MailKit |
| Integración IA | OpenAI GPT-4o-mini |
| Contenedores | Docker + Docker Compose |

---

## Inicio Rápido

### Prerrequisitos

**Para despliegue con Docker:**
- Docker Engine 20.10+
- Docker Compose 2.0+

**Para desarrollo local:**
- .NET 8 SDK
- PostgreSQL 16
- Git

### Ejecución con Docker

Este es el método recomendado para una configuración rápida.

1. **Clonar el repositorio**

```bash
git clone https://github.com/Brahiamriwi/TalentoPlus.git
cd TalentoPlus
```

2. **Crear archivo de entorno**

```bash
cp .env.example .env
```

3. **Editar el archivo `.env` con tus valores**

```env
# Base de datos
POSTGRES_USER=postgres
POSTGRES_PASSWORD=tu_contraseña_segura
POSTGRES_DB=TalentoPlusDb

# OpenAI (para el asistente IA)
OPENAI_API_KEY=sk-tu-api-key

# JWT (para autenticación de la API)
JWT_KEY=tu-clave-secreta-minimo-32-caracteres

# SMTP (para notificaciones por email)
SMTP_HOST=smtp.gmail.com
SMTP_PORT=587
SMTP_USER=tu-email@gmail.com
SMTP_PASSWORD=tu-contraseña-de-aplicacion
```

4. **Construir e iniciar contenedores**

```bash
docker-compose up --build -d
```

5. **Verificar que los servicios estén corriendo**

```bash
docker-compose ps
```

6. **Acceder a las aplicaciones**

| Servicio | URL |
|----------|-----|
| Portal Web | http://localhost:5001 |
| API (Swagger) | http://localhost:5002/swagger |

7. **Ver logs**

```bash
docker-compose logs -f
```

8. **Detener servicios**

```bash
docker-compose down
```

### Ejecución Local

Para desarrollo sin Docker.

1. **Clonar el repositorio**

```bash
git clone https://github.com/Brahiamriwi/TalentoPlus.git
cd TalentoPlus
```

2. **Crear base de datos PostgreSQL**

```sql
CREATE DATABASE "TalentoPlusDb";
```

3. **Configurar ajustes de la aplicación**

Crear `appsettings.Development.json` en las carpetas `TalentoPlus.Web` y `TalentoPlus.API`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=TalentoPlusDb;Username=postgres;Password=tu_contraseña"
  },
  "OpenAI": {
    "ApiKey": "sk-tu-api-key"
  },
  "Jwt": {
    "Key": "tu-clave-secreta-minimo-32-caracteres",
    "Issuer": "TalentoPlusAPI",
    "Audience": "TalentoPlusUsers"
  },
  "Smtp": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "User": "tu-email@gmail.com",
    "Password": "tu-contraseña-de-aplicacion"
  }
}
```

4. **Aplicar migraciones de base de datos**

```bash
dotnet ef database update --project TalentoPlus.Infrastructure --startup-project TalentoPlus.Web
```

5. **Ejecutar el Portal Web**

```bash
cd TalentoPlus.Web
dotnet run
```

Acceder en: http://localhost:5076

6. **Ejecutar la API (en una terminal separada)**

```bash
cd TalentoPlus.API
dotnet run
```

Acceder a Swagger en: http://localhost:5226/swagger

---

## Configuración

### Variables de Entorno

| Variable | Descripción | Requerida |
|----------|-------------|-----------|
| `POSTGRES_USER` | Usuario de PostgreSQL | Sí |
| `POSTGRES_PASSWORD` | Contraseña de PostgreSQL | Sí |
| `POSTGRES_DB` | Nombre de la base de datos | Sí |
| `OPENAI_API_KEY` | API key de OpenAI para el asistente IA | Sí |
| `JWT_KEY` | Clave secreta para tokens JWT (mín. 32 caracteres) | Sí |
| `JWT_ISSUER` | Emisor del token JWT | No |
| `JWT_AUDIENCE` | Audiencia del token JWT | No |
| `SMTP_HOST` | Hostname del servidor SMTP | Sí |
| `SMTP_PORT` | Puerto del servidor SMTP | Sí |
| `SMTP_USER` | Usuario/email SMTP | Sí |
| `SMTP_PASSWORD` | Contraseña SMTP o contraseña de aplicación | Sí |

### Credenciales de Administrador por Defecto

En la primera ejecución, el sistema crea una cuenta de administrador:

| Campo | Valor |
|-------|-------|
| Email | admin@talentoplus.com |
| Contraseña | Admin123* |
| Rol | Administrator |

**Importante:** Cambiar la contraseña del administrador inmediatamente en entornos de producción.

---

## Documentación de la API

### Endpoints Públicos (Sin autenticación requerida)

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/departamentos` | Listar todos los departamentos |
| GET | `/api/departamentos/{id}` | Obtener departamento por ID |
| POST | `/api/auth/register` | Solicitar credenciales de cuenta |
| POST | `/api/auth/login` | Autenticarse y recibir token JWT |

### Endpoints Protegidos (Requieren JWT)

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/empleado/me` | Obtener información del empleado autenticado |
| GET | `/api/empleado/me/pdf` | Descargar hoja de vida en PDF |
| PUT | `/api/empleado/me/contact` | Actualizar dirección y teléfono |

### Ejemplos de Peticiones/Respuestas

**Registro (Solicitar credenciales)**

```http
POST /api/auth/register
Content-Type: application/json

{
  "document": "1234567890"
}
```

Respuesta:
```json
{
  "message": "Cuenta creada. Credenciales enviadas a j***@example.com"
}
```

**Login**

```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "juan@example.com",
  "password": "contraseña_recibida"
}
```

Respuesta:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiration": "2025-12-10T12:00:00Z",
  "email": "juan@example.com",
  "fullName": "Juan Pérez"
}
```

**Obtener Información del Empleado (Protegido)**

```http
GET /api/empleado/me
Authorization: Bearer {token}
```

Respuesta:
```json
{
  "id": 1,
  "document": "1234567890",
  "firstName": "Juan",
  "lastName": "Pérez",
  "email": "juan@example.com",
  "position": "Desarrollador de Software",
  "department": "Tecnología",
  "status": "Activo"
}
```

---

## Flujo de Autenticación

La API implementa un flujo de autenticación seguro donde las contraseñas nunca son elegidas por los usuarios:

1. **El administrador crea el empleado** en el Portal Web con su información personal
2. **El empleado solicita acceso** llamando a `/api/auth/register` con su número de documento
3. **El sistema valida** que el empleado existe en la base de datos
4. **El sistema genera** una contraseña aleatoria segura
5. **El sistema envía** la contraseña al email registrado del empleado
6. **El empleado inicia sesión** usando su email y la contraseña recibida

Este flujo previene el robo de cuentas ya que las credenciales solo se envían al email registrado por RRHH.

```
┌─────────────┐     ┌─────────────┐     ┌─────────────┐
│   Admin     │     │   Sistema   │     │  Empleado   │
└──────┬──────┘     └──────┬──────┘     └──────┬──────┘
       │                   │                   │
       │ Crear empleado    │                   │
       │──────────────────>│                   │
       │                   │                   │
       │                   │   POST /register  │
       │                   │<──────────────────│
       │                   │                   │
       │                   │ Enviar contraseña │
       │                   │ por email         │
       │                   │──────────────────>│
       │                   │                   │
       │                   │   POST /login     │
       │                   │<──────────────────│
       │                   │                   │
       │                   │   Token JWT       │
       │                   │──────────────────>│
       │                   │                   │
```

---

## Pruebas

El proyecto incluye pruebas unitarias y de integración.

**Ejecutar todas las pruebas:**

```bash
dotnet test
```

**Ejecutar con cobertura:**

```bash
dotnet test --collect:"XPlat Code Coverage"
```

**Categorías de pruebas:**

| Categoría | Descripción |
|-----------|-------------|
| Unit Tests | Pruebas de lógica de repositorios y servicios |
| Integration Tests | Pruebas de endpoints de controladores |

---

## Estructura del Proyecto

```
TalentoPlus/
│
├── TalentoPlus.Core/
│   ├── DTOs/                    # Objetos de Transferencia de Datos
│   ├── Entities/                # Entidades de dominio (Employee, Department)
│   ├── Enums/                   # Enumeraciones (EmployeeStatus, EducationLevel)
│   └── Interfaces/              # Contratos de repositorios y servicios
│
├── TalentoPlus.Infrastructure/
│   ├── Data/
│   │   └── ApplicationDbContext.cs
│   ├── Repositories/            # Implementaciones de repositorios
│   └── Services/                # Implementaciones de servicios externos
│       ├── EmailService.cs
│       ├── PdfService.cs
│       ├── ExcelService.cs
│       └── OpenAIService.cs
│
├── TalentoPlus.Web/
│   ├── Controllers/             # Controladores MVC
│   ├── Views/                   # Vistas Razor
│   ├── Models/                  # Modelos de vista
│   └── wwwroot/                 # Archivos estáticos (CSS, JS)
│
├── TalentoPlus.API/
│   └── Controllers/             # Controladores de API
│       ├── AuthController.cs
│       ├── EmpleadoController.cs
│       └── DepartamentosController.cs
│
├── TalentoPlus.Tests/
│   ├── UnitTests/
│   └── IntegrationTests/
│
├── docker-compose.yml
├── .env.example
└── README.md
```

---

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

---

## Author

**Brahiam Riwi**

- GitHub: [@Brahiamriwi](https://github.com/Brahiamriwi)
