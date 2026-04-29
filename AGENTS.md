# AGENTS.md — Quick Context for OpenCode Sessions

## Project Structure — 4-Layer Clean Architecture
- **Domain/** — Entities, Enums, Value objects, Domain events, Common abstractions (interfaces). Pure business rules, zero infrastructure.
- **Application/** — Use cases, DTOs, interfaces for persistence/services, mapping (AutoMapper), responses, validators. Orchestrates domain; no HTTP concerns.
- **Persistence/** — EF Core DbContext, repositories, migrations, security (JWT), identity, tenant provider. Implements Application contracts.
- **TrendyApi/** (Presentation) — ASP.NET Core entry point: Program.cs, service registration, config (appsettings*.json). No controllers implemented.

## Critical Gaps (Ramp-up Notes)
- **No Controllers**: Controllers layer not present — endpoints must be added (Minimal APIs or MVC controllers). Currently Program.cs maps Controllers but none exist.
- **Incomplete Auth**: Register implemented as availability check only; Login not implemented; no token issuance flow wired; no refresh tokens or password hashing wired through DI (PasswordMaker exists but not used in flows).
- **No API surface**: Add controllers/minimal APIs and authorization policies before exposing endpoints.

## Key Commands
- dotnet build — build solution
- dotnet ef migrations add <name> -s TrendyApi/TrendyApi.csproj -p Persistence/Persistence.csproj — add migration
- dotnet ef database update -s TrendyApi/TrendyApi.csproj — apply migrations
- dotnet run --project TrendyApi/TrendyApi.csproj — run API

## Database / Config Notes
- Provider: SQL Server. Default dev connection in AppDbContext.OnConfiguring (sqlserver:1433, TaskMana!1). Override via connection string "ConnectionString" in appsettings.json.
- Persistence registers AddDbContext with connection string from config (PersistenceServiceRegisteration.cs).
- AppDbContext has multi-tenant query filters (CompanyId) and audit entity save hooks. Migrations assembly = Persistence.
- appsettings.json minimal (Logging). Add Jwt (Key,Issuer,Audience,ExpiresInMinutes) and ConnectionString for local dev.

## Multi-tenancy
- TenantProvider reads claim "companyId" from HttpContext (IHttpContextAccessor). Used by AppDbContext query filter (x.CompanyId == _tenantProvider.CompanyId).
- Entities that support tenancy carry CompanyId and have query filters applied. Ensure every tenant-scoped entity includes CompanyId and index.

## Security Practices
- JWT tokens: HmacSha256, claims include sub, email, role, companyId. Token lifetime from Jwt:ExpiresInMinutes.
- Store secrets securely (user-secrets or env vars) — do not commit keys to repo.
- Password handling: PasswordMaker exists; ensure registration/login uses it (not currently wired).
- Enforce HTTPS in production (UseHttpsRedirection present).
- Validate all inputs; use existing validators (FluentValidation not present — use manual validation or add library).

## Config Files of Interest
- TrendyApi/appsettings.json — global settings; add "ConnectionString" and "Jwt".
- TrendyApi/appsettings.Development.json — dev logging only.
- TrendyApi/Program.cs — service registration and middleware pipeline (AddControllers, AddOpenApi).
- TrendyApi/ApplicationServiceRegisteration.cs — application-level DI.
- Persistence/PersistenceServiceRegisteration.cs — persistence/security DI.
- Persistence/AppDbContext.cs — EF config, multi-tenant filters, audit save hooks.
- Persistence/Security/JwtTokenGenerator.cs — token creation.
- Persistence/Identity/TenantProvider.cs — tenant resolution from claims.

## Quick Start (OpenCode)
1. Check appsettings.json for ConnectionString and Jwt config before running.
2. Add controllers or minimal endpoints under TrendyApi/ (or a separate Presentation layer).
3. Implement Login flow and wire password hashing (PasswordMaker).
4. Add authorization policies and enforce tenant isolation in new endpoints.
5. Run migrations when model changes (dotnet ef migrations add/update).
6. Never commit secrets — use user-secrets or environment variables.