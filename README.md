# 🚀 TalentoPlus - Employee Management System

<p align="center">
  <a href="#-descripción-en-español">🇪🇸 Español</a> •
  <a href="#-description-in-english">🇺🇸 English</a>
</p>

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-4169E1?logo=postgresql)
![Docker](https://img.shields.io/badge/Docker-Ready-2496ED?logo=docker)
![License](https://img.shields.io/badge/License-MIT-green)

---

# 🇺🇸 Description in English

## 📋 Description

TalentoPlus is a comprehensive human resources management solution that includes:

- **Admin Web Portal**: Complete employee management with role-based authentication
- **REST API**: Employee access to personal information with JWT authentication
- **AI Assistant**: Natural language queries about employee data using OpenAI
- **PDF Generation**: Professional resumes for each employee
- **Excel Import**: Bulk employee upload from Excel files
- **Email Notifications**: Automatic welcome emails for new employees

## 🏗️ Architecture

The project follows a **Clean Architecture** pattern with 5 layers:

```
TalentoPlus/
├── TalentoPlus.Core/           # Entities, Enums, Interfaces, DTOs
├── TalentoPlus.Infrastructure/ # DbContext, Repositories, Services
├── TalentoPlus.Web/            # MVC Admin Portal (Port 5001)
├── TalentoPlus.API/            # REST API for employees (Port 5002)
└── TalentoPlus.Tests/          # Unit & Integration Tests
```

### Tech Stack

| Component | Technology |
|-----------|------------|
| Framework | .NET 8 |
| Database | PostgreSQL 16 |
| ORM | Entity Framework Core 8 |
| Web Auth | ASP.NET Core Identity + Cookies |
| API Auth | JWT Bearer Tokens |
| PDF Generation | QuestPDF |
| Excel Import | EPPlus |
| Email | MailKit |
| AI | OpenAI GPT-4o-mini |
| Containers | Docker + Docker Compose |

## 🐳 Running with Docker (Recommended)

### Prerequisites
- [Docker](https://www.docker.com/get-started) installed
- [Docker Compose](https://docs.docker.com/compose/install/) installed

### Steps

1. **Clone the repository**
   ```bash
   git clone https://github.com/Brahiamriwi/TalentoPlus.git
   cd TalentoPlus
   ```

2. **Configure environment variables**
   ```bash
   cp .env.example .env
   # Edit .env with your actual values
   ```

3. **Build and run**
   ```bash
   docker-compose up --build -d
   ```

4. **Verify services are running**
   ```bash
   docker-compose ps
   ```

5. **Access the applications**
   - 🌐 **Web Portal**: http://localhost:5001
   - 🔌 **API + Swagger**: http://localhost:5002/swagger

6. **View logs**
   ```bash
   docker-compose logs -f
   ```

7. **Stop services**
   ```bash
   docker-compose down
   ```

## 💻 Running without Docker (Local Development)

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL 16](https://www.postgresql.org/download/)

### Steps

1. **Clone the repository**
   ```bash
   git clone https://github.com/Brahiamriwi/TalentoPlus.git
   cd TalentoPlus
   ```

2. **Create PostgreSQL database**
   ```sql
   CREATE DATABASE "TalentoPlusDb";
   ```

3. **Configure `appsettings.Development.json`**
   
   Create file `TalentoPlus.Web/appsettings.Development.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Port=5432;Database=TalentoPlusDb;Username=postgres;Password=your_password"
     },
     "OpenAI": {
       "ApiKey": "sk-your-api-key"
     },
     "Smtp": {
       "Host": "smtp.gmail.com",
       "Port": 587,
       "Username": "your-email@gmail.com",
       "Password": "your-app-password",
       "FromEmail": "your-email@gmail.com",
       "FromName": "TalentoPlus"
     }
   }
   ```

   Create file `TalentoPlus.API/appsettings.Development.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Port=5432;Database=TalentoPlusDb;Username=postgres;Password=your_password"
     },
     "OpenAI": {
       "ApiKey": "sk-your-api-key"
     },
     "Jwt": {
       "Key": "your-jwt-key-minimum-32-characters-secure",
       "Issuer": "TalentoPlusAPI",
       "Audience": "TalentoPlusUsers"
     },
     "Smtp": {
       "Host": "smtp.gmail.com",
       "Port": 587,
       "Username": "your-email@gmail.com",
       "Password": "your-app-password",
       "FromEmail": "your-email@gmail.com",
       "FromName": "TalentoPlus"
     }
   }
   ```

4. **Apply migrations**
   ```bash
   dotnet ef database update --project TalentoPlus.Infrastructure --startup-project TalentoPlus.Web
   ```

5. **Run the Web Portal**
   ```bash
   cd TalentoPlus.Web
   dotnet run
   # Access https://localhost:5001
   ```

6. **Run the API (in another terminal)**
   ```bash
   cd TalentoPlus.API
   dotnet run
   # Access https://localhost:5002/swagger
   ```

7. **Run tests**
   ```bash
   dotnet test
   ```

## ⚙️ Environment Variables

| Variable | Description | Required |
|----------|-------------|----------|
| `POSTGRES_USER` | PostgreSQL username | ✅ |
| `POSTGRES_PASSWORD` | PostgreSQL password | ✅ |
| `POSTGRES_DB` | Database name | ✅ |
| `OPENAI_API_KEY` | OpenAI API Key for AI assistant | ✅ |
| `JWT_KEY` | Secret key for JWT tokens (min. 32 chars) | ✅ |
| `JWT_ISSUER` | JWT token issuer | ❌ |
| `JWT_AUDIENCE` | JWT token audience | ❌ |
| `SMTP_HOST` | SMTP server | ✅ |
| `SMTP_PORT` | SMTP port (587 for TLS) | ✅ |
| `SMTP_USERNAME` | SMTP username | ✅ |
| `SMTP_PASSWORD` | SMTP password | ✅ |
| `SMTP_FROM_EMAIL` | Sender email | ✅ |
| `SMTP_FROM_NAME` | Sender name | ✅ |

## 🔐 Admin Credentials

When the application starts for the first time, an admin user is automatically created:

| Field | Value |
|-------|-------|
| **Email** | `admin@talentoplus.com` |
| **Password** | `Admin123*` |
| **Role** | Administrator |

> ⚠️ **Important**: Change the admin password in production.

## 🔌 API Endpoints

### Public (No authentication)

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/departamentos` | List all departments |
| `GET` | `/api/departamentos/{id}` | Get department by ID |
| `POST` | `/api/auth/register` | Register new employee |
| `POST` | `/api/auth/login` | Login and get JWT token |

### Protected (JWT Required)

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/empleado/me` | Get authenticated employee info |
| `GET` | `/api/empleado/me/pdf` | Download resume as PDF |
| `PUT` | `/api/empleado/me/contact` | Update address/phone |

### API Usage Example

**1. Login to get token:**
```bash
curl -X POST http://localhost:5002/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email": "employee@example.com", "password": "Password123*"}'
```

**2. Use token in protected requests:**
```bash
curl -X GET http://localhost:5002/api/empleado/me \
  -H "Authorization: Bearer {your_jwt_token}"
```

---

# 🇪🇸 Descripción en Español

## 📋 Descripción

TalentoPlus es una solución completa para la gestión de recursos humanos que incluye:

- **Portal Web Administrativo**: Gestión completa de empleados con autenticación basada en roles
- **API REST**: Acceso para empleados a su información personal con autenticación JWT
- **Asistente IA**: Consultas en lenguaje natural sobre datos de empleados usando OpenAI
- **Generación de PDFs**: Hojas de vida profesionales para cada empleado
- **Importación Excel**: Carga masiva de empleados desde archivos Excel
- **Notificaciones Email**: Emails de bienvenida automáticos para nuevos empleados

## 🏗️ Arquitectura

El proyecto sigue una **arquitectura limpia (Clean Architecture)** con 5 capas:

```
TalentoPlus/
├── TalentoPlus.Core/           # Entidades, Enums, Interfaces, DTOs
├── TalentoPlus.Infrastructure/ # DbContext, Repositories, Services
├── TalentoPlus.Web/            # MVC Admin Portal (Puerto 5001)
├── TalentoPlus.API/            # REST API para empleados (Puerto 5002)
└── TalentoPlus.Tests/          # Unit & Integration Tests
```

### Stack Tecnológico

| Componente | Tecnología |
|------------|------------|
| Framework | .NET 8 |
| Base de Datos | PostgreSQL 16 |
| ORM | Entity Framework Core 8 |
| Autenticación Web | ASP.NET Core Identity + Cookies |
| Autenticación API | JWT Bearer Tokens |
| PDF Generation | QuestPDF |
| Excel Import | EPPlus |
| Email | MailKit |
| IA | OpenAI GPT-4o-mini |
| Contenedores | Docker + Docker Compose |

## 🐳 Ejecución con Docker (Recomendado)

### Prerrequisitos
- [Docker](https://www.docker.com/get-started) instalado
- [Docker Compose](https://docs.docker.com/compose/install/) instalado

### Pasos

1. **Clonar el repositorio**
   ```bash
   git clone https://github.com/Brahiamriwi/TalentoPlus.git
   cd TalentoPlus
   ```

2. **Configurar variables de entorno**
   ```bash
   cp .env.example .env
   # Editar .env con tus valores reales
   ```

3. **Construir y ejecutar**
   ```bash
   docker-compose up --build -d
   ```

4. **Verificar que los servicios estén corriendo**
   ```bash
   docker-compose ps
   ```

5. **Acceder a las aplicaciones**
   - 🌐 **Portal Web**: http://localhost:5001
   - 🔌 **API + Swagger**: http://localhost:5002/swagger

6. **Ver logs**
   ```bash
   docker-compose logs -f
   ```

7. **Detener servicios**
   ```bash
   docker-compose down
   ```

## 💻 Ejecución sin Docker (Desarrollo Local)

### Prerrequisitos
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL 16](https://www.postgresql.org/download/)

### Pasos

1. **Clonar el repositorio**
   ```bash
   git clone https://github.com/Brahiamriwi/TalentoPlus.git
   cd TalentoPlus
   ```

2. **Crear la base de datos PostgreSQL**
   ```sql
   CREATE DATABASE "TalentoPlusDb";
   ```

3. **Configurar `appsettings.Development.json`**
   
   Crear archivo `TalentoPlus.Web/appsettings.Development.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Port=5432;Database=TalentoPlusDb;Username=postgres;Password=tu_password"
     },
     "OpenAI": {
       "ApiKey": "sk-tu-api-key"
     },
     "Smtp": {
       "Host": "smtp.gmail.com",
       "Port": 587,
       "Username": "tu-email@gmail.com",
       "Password": "tu-app-password",
       "FromEmail": "tu-email@gmail.com",
       "FromName": "TalentoPlus"
     }
   }
   ```

   Crear archivo `TalentoPlus.API/appsettings.Development.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Port=5432;Database=TalentoPlusDb;Username=postgres;Password=tu_password"
     },
     "OpenAI": {
       "ApiKey": "sk-tu-api-key"
     },
     "Jwt": {
       "Key": "tu-clave-jwt-minimo-32-caracteres-segura",
       "Issuer": "TalentoPlusAPI",
       "Audience": "TalentoPlusUsers"
     },
     "Smtp": {
       "Host": "smtp.gmail.com",
       "Port": 587,
       "Username": "tu-email@gmail.com",
       "Password": "tu-app-password",
       "FromEmail": "tu-email@gmail.com",
       "FromName": "TalentoPlus"
     }
   }
   ```

4. **Aplicar migraciones**
   ```bash
   dotnet ef database update --project TalentoPlus.Infrastructure --startup-project TalentoPlus.Web
   ```

5. **Ejecutar el Portal Web**
   ```bash
   cd TalentoPlus.Web
   dotnet run
   # Acceder a https://localhost:5001
   ```

6. **Ejecutar la API (en otra terminal)**
   ```bash
   cd TalentoPlus.API
   dotnet run
   # Acceder a https://localhost:5002/swagger
   ```

7. **Ejecutar tests**
   ```bash
   dotnet test
   ```

## ⚙️ Variables de Entorno

| Variable | Descripción | Requerida |
|----------|-------------|-----------|
| `POSTGRES_USER` | Usuario de PostgreSQL | ✅ |
| `POSTGRES_PASSWORD` | Contraseña de PostgreSQL | ✅ |
| `POSTGRES_DB` | Nombre de la base de datos | ✅ |
| `OPENAI_API_KEY` | API Key de OpenAI para el asistente IA | ✅ |
| `JWT_KEY` | Clave secreta para tokens JWT (mín. 32 chars) | ✅ |
| `JWT_ISSUER` | Emisor del token JWT | ❌ |
| `JWT_AUDIENCE` | Audiencia del token JWT | ❌ |
| `SMTP_HOST` | Servidor SMTP | ✅ |
| `SMTP_PORT` | Puerto SMTP (587 para TLS) | ✅ |
| `SMTP_USERNAME` | Usuario SMTP | ✅ |
| `SMTP_PASSWORD` | Contraseña SMTP | ✅ |
| `SMTP_FROM_EMAIL` | Email remitente | ✅ |
| `SMTP_FROM_NAME` | Nombre remitente | ✅ |

## 🔐 Credenciales de Administrador

Al iniciar la aplicación por primera vez, se crea automáticamente un usuario administrador:

| Campo | Valor |
|-------|-------|
| **Email** | `admin@talentoplus.com` |
| **Password** | `Admin123*` |
| **Rol** | Administrator |

> ⚠️ **Importante**: Cambiar la contraseña del administrador en producción.

## 🔌 API Endpoints

### Públicos (Sin autenticación)

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| `GET` | `/api/departamentos` | Lista todos los departamentos |
| `GET` | `/api/departamentos/{id}` | Obtiene un departamento por ID |
| `POST` | `/api/auth/register` | Registra nuevo empleado |
| `POST` | `/api/auth/login` | Login y obtiene token JWT |

### Protegidos (Requieren JWT)

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| `GET` | `/api/empleado/me` | Información del empleado autenticado |
| `GET` | `/api/empleado/me/pdf` | Descarga hoja de vida en PDF |
| `PUT` | `/api/empleado/me/contact` | Actualiza dirección/teléfono |

### Ejemplo de uso de la API

**1. Login para obtener token:**
```bash
curl -X POST http://localhost:5002/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email": "empleado@example.com", "password": "Password123*"}'
```

**2. Usar token en peticiones protegidas:**
```bash
curl -X GET http://localhost:5002/api/empleado/me \
  -H "Authorization: Bearer {tu_token_jwt}"
```

## 📊 Funcionalidades del Portal Web

### Dashboard
- Estadísticas generales (total empleados, activos, departamentos)
- Distribución de empleados por departamento
- Asistente IA para consultas en lenguaje natural

### Gestión de Empleados
- CRUD completo de empleados
- Filtros y búsqueda
- Descarga de hoja de vida en PDF
- Cambio de estado (Activo, Inactivo, Vacaciones)

### Importación Excel
- Carga masiva de empleados
- Validación de datos
- Reporte de errores

## 🧪 Tests

El proyecto incluye tests unitarios y de integración:

```bash
# Ejecutar todos los tests
dotnet test

# Ejecutar con detalle
dotnet test --verbosity normal

# Ejecutar con cobertura
dotnet test --collect:"XPlat Code Coverage"
```

## 📝 Licencia

Este proyecto está bajo la Licencia MIT. Ver el archivo [LICENSE](LICENSE) para más detalles.

## 👨‍💻 Autor

Desarrollado por **Brahiam Riwi** como proyecto de aprendizaje para gestión de talento humano.

---

⭐ Si este proyecto te fue útil, ¡dale una estrella en GitHub!
