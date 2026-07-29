# BVTL-X Project Rules for Antigravity

## Project Context
- ASP.NET MVC 5 C# solution at `d:\Projects\BVTL-X` (`WebApp.sln`).
- Targeting .NET Framework 4.7.2, Entity Framework 6, SQL Server.
- Main projects: `WebApp` (UI/Reports), `SyncApp` (Data Sync), `Data`, `Model`, `Common`.
- IGNORE `WebBVTLAPI` project entirely.

## Coordination with Claude Code
- This project is developed using both Antigravity (Gemini) and Claude Code.
- Shared context and tracking are stored under `docs/` and `.agents/` directories.
- Always read `docs/session-log.md` when resuming work.
- Always update `docs/session-log.md` before ending a session with:
  - What was accomplished
  - What needs to be done next
  - Files modified
  - Any open questions or risks
- Avoid parallel file edits on the same branch.

## Division of Responsibilities
- **Antigravity**: Focuses on research, code analysis, writing implementation plans, review, and documentation.
- **Claude Code**: Focuses on code execution, multi-file code generation, refactoring, building, and running test commands.

## Coding Style & Standards
- Language: Write technical plans, documentation, and session logs in Vietnamese. Write code, API endpoints, comments, and variable names in English.
- Report Controller standard: Follow `BaoCaoThangController.cs` for report structure. Keep logic clean.
- Security: Parametrize all SQL queries to prevent SQL Injection. Do not expose connection strings or plain-text passwords.
