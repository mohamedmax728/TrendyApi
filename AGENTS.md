# AGENTS.md — Quick Context for OpenCode Sessions

## Project Structure — 4-Layer Clean Architecture
- **Domain/** — Entities, Enums, Value objects, Domain events, Common abstractions (interfaces). Pure business rules, zero infrastructure.  
  Entities: `User`, `Role`, `Product`, `ProductCategory`, `Store`, `Vendor`, `AuditEntity` (base).  
  AuditEntity has `CompanyId` (int, NOT NULL) — critical for multi-tenancy.
- **Application/** — Use cases, DTOs, interfaces for persistence/services, mapping (AutoMapper), responses, validators. Orchestrates domain; no HTTP concerns.  
  Contains: `Authentication/` feature, `Common/Abstractions` (IJwtTokenGenerator, ITenantProvider, IUnitOfWork), `Common/Helpers` (PasswordMaker, ValidationRegex), `Common/Responses` (BaseResponse), `Common/Models` (PaginatedResult), `Profiles` (MappingProfile — EMPTY, needs config).
- **Persistence/** — EF Core DbContext, repositories, migrations, security (JWT), identity, tenant provider. Implements Application contracts.  
  `AppDbContext` has tenant query filter on User. `BaseRepository<T>` generic CRUD. `JwtTokenGenerator` creates tokens. `TenantProvider` reads `companyId` from JWT claims.
- **TrendyApi/** (Presentation) — ASP.NET Core entry point: Program.cs, service registration, config (appsettings*.json). **No controllers implemented.**

## Critical Gaps (Ramp-up Notes)  
**NEWBIES: READ THIS FIRST**
- **No Controllers**: Zero API endpoints exist. Program.cs calls `AddControllers()` and `MapControllers()` but no `[ApiController]` classes found. You must create them.
- **Incomplete Auth Flows**:  
  - `RegisterAsync` only checks email/phone uniqueness — does NOT hash password or save user.  
  - `LoginAsync` throws `NotImplementedException`.  
  - JWT middleware NOT configured in Program.cs (`AddAuthentication`, `UseAuthentication` missing).  
  - PasswordMaker exists but is never called during registration.
- **No API Surface**: Cannot make authenticated requests until controllers + auth middleware + policies are added.
- **AutoMapper Empty**: MappingProfile.cs exists but has no `CreateMap<,>()` calls. Projections via `ProjectTo<>` will fail.
- **Tenant ID on Registration**: User entity requires CompanyId but registration flow doesn't set it. What value to use for new users?

## Key Commands
```bash
# Build all projects (Domain, Application, Persistence, API)
dotnet build

# Run API (HTTP :5261, HTTPS :7259)
dotnet run --project TrendyApi/TrendyApi.csproj

# Add EF migration (after adding ConnectionString to appsettings.json)
dotnet ef migrations add <MigrationName> -s TrendyApi/TrendyApi.csproj -p Persistence/Persistence.csproj

# Apply migrations to database
dotnet ef database update -s TrendyApi/TrendyApi.csproj
```

## Database & Connection String  
**Two sources (potential conflict):**
1. `appsettings.json` → `"ConnectionString"` key — used by `PersistenceServiceRegisteration.cs`
2. Hardcoded in `AppDbContext.OnConfiguring` — fallback: `"Server=sqlserver,1433;Database=TaskManagementDb;User Id=sa;Password=TaskMana!1;TrustServerCertificate=True;"`  
**Note**: The persistence service registration uses option (1) if provided. Option (2) is fallback when no config. Add `"ConnectionString"` to `appsettings.json`.

- Provider: SQL Server
- Migrations assembly: `Persistence` (see `PersistenceServiceRegisteration.cs`)
- Tenant isolation: `AppDbContext` applies `x.CompanyId == _tenantProvider.CompanyId` filter on `User` entity. **Indexed**.

## Authentication Flow (What Needs to Be Completed)
```
1. Client POST /auth/register with RegisterRequestDto
   ↓ (MISSING)
   - Validate DTO (FluentValidation works)
   - Check email/phone uniqueness (works in AuthenticationService)
   - ⚠️ Hash password via PasswordMaker.CreatePasswordHash (NOT CALLED)
   - ⚠️ Create User entity (CompanyId = ???)
   - ⚠️ Save via UserRepository.AddAsync (NOT CALLED)
   - ⚠️ Return success

2. Client POST /auth/login with LoginRequestDto
   ↓ (MISSING)
   - ⚠️ Fetch user by email (NOT IMPLEMENTED)
   - ⚠️ Verify password via PasswordMaker.VerifyPassword (NOT CALLED)
   - ⚠️ Generate JWT via JwtTokenGenerator.GenerateToken (exists but not used)
   - ⚠️ Return token

3. Subsequent requests
   ↓ (MISSING)
   - ⚠️ JWT validation middleware not configured
   - ⚠️ No [Authorize] attributes on any endpoint
   - ⚠️ TenantProvider fails if companyId claim missing from token
```

## Multi-tenancy Details  
**How it works:**
- All entities inheriting `AuditEntity` have `CompanyId` column (non-nullable int).
- `TenantProvider` constructor reads `companyId` claim from current HttpContext's User (JWT).  
  Code: `CompanyId = int.Parse(claim!.Value);` — **WILL THROW** if claim missing/null.
- `AppDbContext.ConfigureUsers()` adds global query filter: `x.CompanyId == _tenantProvider.CompanyId`.
- Every query on `User` automatically filtered by current tenant's CompanyId.

**Newbie gotchas:**
- What CompanyId for NEW users during registration? No auth context yet (not logged in, no JWT claim).
- If `companyId` claim missing in JWT, TenantProvider throws on construction → request crashes.
- ALL entities should have CompanyId + query filter for true isolation (check Store, Product, etc.).

## Security Practices — What's There vs What's Missing
| Area | Present | Missing |
|------|---------|---------|
| Password hashing | ✅ HMAC-SHA512 + salt (PasswordMaker) | ❌ Never called in registration flow |
| JWT tokens | ✅ Generator creates token with sub/email/role/companyId | ❌ Auth middleware not configured |
| Token signing | ✅ HmacSha256 with key from config | ❌ No key in appsettings.json (will crash) |
| Email uniqueness | ✅ Unique index in DB | ✅ |
| Account lockout | ❌ | ❌ Brute force possible |
| Refresh tokens | ❌ | ❌ Short-lived JWTs only |
| Password complexity | ⚠️ Min 8 chars only | ❌ No uppercase/number/special requirements |
| Rate limiting | ❌ | ❌ |
| CORS | ❌ | ❌ |
| HTTPS enforcement | ✅ UseHttpsRedirection | ✅ |

## FluentValidation Setup  
- Validator file: `RegisterRequestValidator.cs` exists with rules.
- **NOT wired into pipeline**. To use:
  1. Add: `builder.Services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>()` in Program.cs
  2. Either: Add validation filter/attribute to endpoints OR use automatic validation library.
- Custom rule: `ValidationRegex.EnglishLetters` enforces English-only for FullNameEn.

## Testing Readiness  
**What exists:**
- ✅ Interfaces (IUnitOfWork, IBaseRepository) enable mocking
- ✅ DI configured for all layers
- ✅ DTOs separated from entities

**What's missing:**
- ❌ No test projects (no .Tests.csproj files)
- ❌ No xUnit/NUnit packages installed
- ❌ No integration test setup (no TestServer, no in-memory DB configuration)
- ❌ No test data builders

## Performance Optimizations Present
- ✅ `AsNoTracking()` on all queries in BaseRepository (read-only efficiency)
- ✅ `ProjectTo<TResult>()` for DTO projections (AutoMapper, avoids over-fetching)
- ✅ Indexes: `User.Email` (unique), `User.CompanyId` (tenant filter)
- ✅ Lazy repository initialization (`Lazy<T>` pattern in UnitOfWork)

## Config Files - What They Do
| File | Purpose | What to Add |
|------|---------|-------------|
| `TrendyApi/Program.cs` | API entrypoint, middleware pipeline | `AddAuthentication`, `AddAuthorization`, JWT config, wire auth middleware |
| `TrendyApi/appsettings.json` | App configuration | Add `Jwt` section (Key, Issuer, Audience, ExpiresInMinutes), `ConnectionString` |
| `TrendyApi/appsettings.Development.json` | Dev-only logging config | (Good as-is) |
| `TrendyApi/Properties/launchSettings.json` | Dev server URLs | HTTP :5261, HTTPS :7259 |
| `TrendyApi/ApplicationServiceRegisteration.cs` | App-layer DI | AutoMapper, IAuthenticationService |
| `Persistence/PersistenceServiceRegisteration.cs` | Persistence/security DI | DbContext, IJwtTokenGenerator |
| `Application/Profiles/MappingProfile.cs` | AutoMapper config | **EMPTY** — needs `CreateMap<User, UserDto>()` etc. |
| `Persistence/AppDbContext.OnConfiguring` | DbContext config | Hardcoded SQL Server fallback connection |

## Multi-tenancy Implementation Checklist
- [ ] Add `CompanyId` to new User during registration (what value?)
- [ ] Ensure TenantProvider gracefully handles missing claim (don't use `!` operator)
- [ ] Add CompanyId index on all tenant-scoped tables (User ✅, check others)
- [ ] Verify query filter applies correctly (user can't see other tenant's data)
- [ ] Test multi-tenant scenarios (switch CompanyId in JWT)

## Security Implementation Checklist
- [ ] Add JWT config to appsettings.json (Key, Issuer, Audience, ExpiresInMinutes)
- [ ] Configure auth in Program.cs: `AddAuthentication(JwtBearerDefaults.AuthenticationScheme)`
- [ ] Add `[Authorize]` to protected endpoints
- [ ] Implement password hashing in Register flow (call PasswordMaker.CreatePasswordHash)
- [ ] Implement Login flow (fetch user, verify password, generate token)
- [ ] Add account lockout after N failed attempts
- [ ] Add rate limiting to /auth endpoints
- [ ] Configure CORS for production
- [ ] Move secrets to User Secrets or env vars (never commit to git)

## Common Newbie Mistakes (Learn from Others)
1. **Assuming AuthenticationService.Register creates a user** — it doesn't, only checks uniqueness
2. **Assuming JWT auth works out of box** — middleware not configured in Program.cs
3. **Assuming AutoMapper works** — MappingProfile is empty, will throw on projection
4. **Assuming appsettings has JWT config** — it doesn't, will crash at runtime when JwtTokenGenerator reads it
5. **Forgetting CompanyId on new User** — entity requires it, but registration has no tenant context
6. **Using TenantProvider without auth** — will throw if no JWT claim (no null check)
7. **Forgetting to run migrations** — Db has no schema until `dotnet ef database update`
8. **Assuming Login returns token** — throws NotImplementedException

## Quick Start for New Developers
1. **Check config**: Add `Jwt` section and `ConnectionString` to `appsettings.json`
2. **Set up DB**: `dotnet ef database update -s TrendyApi/TrendyApi.csproj`
3. **Create controllers**: Add `[ApiController]` classes under `TrendyApi/Controllers/` or use minimal APIs in `Program.cs`
4. **Wire auth middleware**: In Program.cs add `AddAuthentication`, `AddAuthorization`, `UseAuthentication`, `UseAuthorization`
5. **Fix registration**: In `AuthenticationService.RegisterAsync`, call `PasswordMaker.CreatePasswordHash`, create `User` entity, save via repository
6. **Implement login**: In `AuthenticationService.LoginAsync`, fetch user, verify password, generate and return JWT
7. **Configure AutoMapper**: Populate `MappingProfile.cs` with `CreateMap<,>()` calls
8. **Run & test**: `dotnet run --project TrendyApi/TrendyApi.csproj`, test endpoints

## Where to Find Key Files
```
Authentication:
  ├─ Service: Application/Features/Authentication/AuthenticationService.cs
  ├─ DTOs: Application/Features/Authentication/Dtos/
  ├─ Validator: Application/Features/Authentication/Dtos/Validators/RegisterRequestValidator.cs
  └─ Interface: Application/Features/Authentication/IAuthenticationService.cs

Security/JWT:
  ├─ Generator: Persistence/Security/JwtTokenGenerator.cs
  ├─ Interface: Application/Common/Abstractions/IJwtTokenGenerator.cs
  └─ Password: Application/Common/Helpers/PasswordMaker.cs

Database:
  ├─ Context: Persistence/AppDbContext.cs
  ├─ Migrations: (none yet - create with dotnet ef)
  └─ Repos: Persistence/Repositories/

Tenant:
  ├─ Provider: Persistence/Identity/TenantProvider.cs
  └─ Interface: Application/Common/Abstractions/ITenantProvider.cs

DI Config:
  ├─ Program.cs: TrendyApi/Program.cs
  ├─ App Services: TrendyApi/ApplicationServiceRegisteration.cs
  └─ Persistence Services: Persistence/PersistenceServiceRegisteration.cs
```

## Questions to Ask Before Starting
- What CompanyId should new users get during registration (before they have a JWT)?
- Do we need email/phone verification flows?
- What password complexity rules are required (beyond 8 chars)?
- How long should JWTs last? (configure ExpiresInMinutes)
- Do we need refresh tokens or is re-login acceptable?
- What CORS origins are allowed in production?
- Any special rate limiting requirements for auth endpoints?

## References in Codebase
- `AGENTS.md` (this file) — repo-specific agent guidance
- `.kilo/plans/1777165384676-calm-eagle.md` — detailed technical analysis
- `README*` — project README if exists
- `appsettings*.json` — runtime configuration