# NexusComm — Communication Manager

A full-stack **Communication Manager** for scheduling, dispatching, and tracking **Email (SMTP)** and **WhatsApp (WATI)** messages, with automatic retry, background job processing, and complete delivery tracking.

Built as part of an internship project to demonstrate a production-style architecture: layered backend design, JWT authentication, reliable background job scheduling, and a responsive React frontend.

---

## ✨ Features

- **Dual-channel messaging** — send Email via SMTP and WhatsApp via the WATI Business API from a single interface
- **Send Now or Schedule** — dispatch immediately, or schedule for any future date/time
- **Automatic background dispatching** — a Hangfire recurring job polls the database every 15 seconds and dispatches due messages without any user action
- **Retry with exponential backoff** — failed messages automatically retry up to 3 times, with increasing delay between attempts (2 min → 4 min)
- **Complete tracking** — every message records its full lifecycle: created, scheduled, processing, sent/failed timestamps, retry count, and failure reason
- **Attempt history** — every dispatch attempt (success or failure) is stored as a separate, permanent record — nothing is ever overwritten
- **JWT authentication** — secure register/login with hashed passwords via ASP.NET Core Identity
- **Role-based access** — Users see only their own communications; Admins can view all (role assignment is manual, by design, for security)
- **Consistent API responses** — a global exception-handling middleware ensures every error returns a clean, predictable JSON shape
- **Hangfire Dashboard** — visual monitoring of every background job run, at `/hangfire`

---

## 🛠️ Tech Stack

**Backend**
- ASP.NET Core Web API (.NET 10)
- Entity Framework Core + SQL Server
- ASP.NET Core Identity + JWT Bearer Authentication
- Hangfire (background job scheduling, backed by SQL Server storage)
- MailKit (SMTP email delivery)
- WATI API (WhatsApp Business messaging, via `HttpClient`)
- Repository Pattern + Service Layer + Dependency Injection

**Frontend**
- React (Vite)
- React Router
- Axios
- Custom design system (no UI framework dependency)

---

## 🏗️ Architecture

```
React Frontend
      ↓
Controllers   → receive HTTP requests
      ↓
Services      → business rules, validation
      ↓
Repositories  → database access (EF Core)
      ↓
SQL Server
```

**Dispatch flow:**

```
Create Message
      ↓
 Send Now?  ──Yes──→ Dispatcher → SMTP / WATI → Attempt recorded → Sent / Retrying / Failed
      │
      No
      ↓
 Status = Scheduled
      ↓
Hangfire (polls every 15s)
      ↓
 Time reached? → Claim → Dispatcher → SMTP / WATI → Attempt recorded → Sent / Retrying / Failed
```

Failed messages are retried automatically (max 3 attempts, exponential backoff). After the final failed attempt, the message is marked permanently **Failed** with a recorded reason — visible on the Failed Messages page.

---

## 📁 Project Structure

```
NexusComm/
├── Nexuscomm.API/          # ASP.NET Core Web API (single project, organized by folders)
│   ├── Controllers/        # HTTP endpoints
│   ├── Services/           # Business logic (Auth, Communication, Dispatcher, SMTP, WATI)
│   ├── Repositories/       # Data access layer
│   ├── Models/             # EF Core entities
│   ├── DTOs/                # Request/response contracts
│   ├── BackgroundServices/ # Hangfire dispatch job
│   ├── Middleware/         # Global exception handling
│   └── Configurations/     # Strongly-typed settings (SMTP, WATI, JWT, Retry)
└── nexuscomm.client/       # React frontend
    ├── src/pages/          # Login, Register, Dashboard, Create/List/Details pages
    ├── src/components/     # Reusable UI (Sidebar, StatusBadge, ChannelIcon, etc.)
    ├── src/context/        # Auth state
    └── src/api/            # Axios API clients
```

---

## 🚀 Getting Started

### Prerequisites
- .NET 10 SDK
- SQL Server (LocalDB is fine for development)
- Node.js + npm

### Backend Setup
```bash
cd Nexuscomm.API
dotnet restore

# Set secrets (never commit real credentials)
dotnet user-secrets set "JwtSettings:Secret" "your-strong-secret-key"
dotnet user-secrets set "SmtpSettings:Password" "your-smtp-app-password"
dotnet user-secrets set "WatiSettings:ApiToken" "your-wati-token"

# Create the database
dotnet ef database update

dotnet run
```
API runs at `https://localhost:7270` — Swagger docs at `/swagger`, Hangfire dashboard at `/hangfire`.

### Frontend Setup
```bash
cd nexuscomm.client
npm install
npm run dev
```
Frontend runs at `http://localhost:5173`.

---

## 🔑 Configuration

Sensitive values (SMTP password, WATI token, JWT secret) are **never hard-coded** — they're read from `appsettings.json` placeholders and overridden via **.NET User Secrets** in development, or environment variables in production. See `Configurations/` for the strongly-typed settings classes.

---

## 📌 Notes

- WhatsApp delivery requires a WATI account connected to a dedicated business phone number (per Meta's policy, this number cannot be linked to an existing personal WhatsApp account). The integration is fully implemented and error-tested — connecting a live account only requires updating configuration values, no code changes.
- Email delivery has been tested end-to-end with a real Gmail SMTP account and confirmed working.

---

## 📄 License

This project was built for educational/internship purposes.
