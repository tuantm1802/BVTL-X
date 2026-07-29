# BVTL-X Project Instructions

## Quick Reference
- **Stack**: ASP.NET MVC 5.2.7, .NET Framework 4.7.2, Entity Framework 6 (EDMX Database-First), SQL Server
- **Solution File**: `WebApp.sln`
- **Main DB Connection**: `BVTL_REPORTING` (SQL Server)

## Projects Directory Map
1. **WebApp** — Main UI, reports, and admin logic.
   - Controllers: `WebApp/Controllers/`
   - Views: `WebApp/Views/`
   - Reports (RDLC): `WebApp/Report/ReportFile/`
   - Services: `WebApp/Service/`
2. **SyncApp** (SyncBVTL.Push) — Background synchronization application.
   - Jobs: `SyncApp/Jobs/` (Quartz.NET 3.0.7 scheduling)
   - Schedulers: `SyncApp/ScheduleTasks/JobScheduler.cs`
   - Services: `SyncApp/Services/ProcessService.cs`
3. **Data** — Database access layer.
   - concretes: `Data/Admin/` (e.g. `BaoCaoTongHopDA.cs`)
   - interfaces: `Data/InterfaceDA/` (e.g. `IBaoCaoTongHopDA.cs`)
   - API sync concretes: `Data/API/` (e.g. `SyncDataFromApi_SaveToDB.cs`)
4. **Model** — Entities & Extended models.
   - entity models: `Model/Model/`
   - extended models: `Model/ModelExtend/`
5. **Common** — Shared helpers, filters & utilities (`Common/Common/`, `Common/ICommon/`).
6. **Simple** — Base project/library.
- **IGNORE**: `WebBVTLAPI` (redundant draft project).

## Build & Run
- Build using VS 2022 or CLI: `msbuild WebApp.sln`
- WebApp startup: Set `WebApp` as startup project in VS 2022.
- SyncApp startup: Set `SyncApp` as startup project (SyncBVTL.Push).

## Key Architecture & Conventions

### Authentication & Authorization
- User login handles inside `LoginController.cs`. Authenticated users are stored in `Session["USER_SESSION"]` as a `UserLogin` model.
- Authorization uses menu-based RBAC via custom action filter `[HasCredential(ControllerName = "ControllerName")]` defined in `Common/Common/HasCredentialAttribute.cs`. It checks `Session["Menus"]`.
- `BaseController.cs` performs global session validation on `OnActionExecuting`. If empty, checks `UserToken` cookie via `TokenService` (JWT implementation).

### Report Pattern (Excel Export)
All report controllers (e.g., `BaoCaoThangController.cs`) follow this structure:
1. Inherit from `BaseController` and use `[HasCredential(ControllerName = "...")]`
2. `Index()` checks user menu access and loads basic dropdown details.
3. `GetData()` processes AJAX grid requests, pulling data from DB via Data Access layer (`*DA`).
4. `Download()` generates Excel reports dynamically using **ClosedXML**.
5. When creating a new report, follow `BaoCaoThangController.cs` as a template.

### Sync Pattern
1. Quartz.NET scheduler triggers jobs (e.g., `GetDataAPIJob.cs`) in `SyncApp`.
2. Job calls `SyncDataFromApi_SaveToDB.cs` under `Data/API/`.
3. Process calls external REST APIs via `HttpClient`, deserializes JSON, maps entities via `ConvertResultApiToEntity.cs` (a massive manual mapper), and bulk writes to DB via `InsertDataDA.cs` (SqlBulkCopy).
4. Results are logged to folder `Log\` under SyncApp.

## Critical Constraints
- **NO SQL INJECTION**: Never use string concatenation for SQL statements. Use parametrized queries.
- **NO DIRECT INSTANTIATION FOR CORE SERVICES**: Follow the Interface-DA pattern, even if DI container is not yet present.
- **DO NOT TOUCH AUTH**: Never modify authentication/authorization filters without explicit user request.
- **CLEAN CODE**: Avoid inline styling in Razor views; utilize class styles.

## Handoff & Session Log
- Before ending an AI session, update `docs/session-log.md`.
- Read `docs/session-log.md` when starting a session to recover state.
