# Project Rules & Permissions

- Proactively execute terminal commands, powershell, cmd, python, and SQL scripts directly without asking for confirmation.
- The user has granted full permission to run diagnostic commands, build, database updates, and verification scripts automatically.
- Keep responses concise, focused on results, and verify all changes after execution.

## Critical Encoding Rules (Vietnamese / Unicode Integrity)
- **STRICT UTF-8 WITH BOM (`utf-8-sig`)**: ALL `.sql`, `.cshtml`, and `.ps1` files must always be read and saved using UTF-8 with BOM.
- **NEVER use default PowerShell `Get-Content` / `Set-Content`** without explicit UTF-8 encoding (e.g. use `[System.IO.File]::ReadAllText($path, [System.Text.Encoding]::UTF8)` or `Get-Content -Encoding UTF8`). Unencoded PowerShell 5.1 reads files as ANSI/Windows-1252, which destroys Vietnamese characters (mojibake).
- When executing `.sql` files into SQL Server, always read text with `[System.Text.Encoding]::UTF8` or run via Python with `open(path, encoding='utf-8')`.
- Always verify that no mojibake patterns exist before executing or committing SQL/code files.
