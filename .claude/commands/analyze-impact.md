# Custom Command: Analyze Impact of a Code Change

This command analyzes the impact of modifying code in a specific class, table, or method of the BVTL-X project.

## Instructions
1. Prompt the user for:
   - What file, class, method, or database table they intend to modify.
   - What the goal of the change is.
2. Search the codebase for references to the specified element using grep or search tools.
3. Identify dependencies across projects:
   - WebApp Controllers ↔ Data Access (`Data/Admin/` or `Data/InterfaceDA/`)
   - Data Access ↔ Models (`Model/`)
   - SyncApp Jobs ↔ Data Access / API Sync Service
4. Map the impact zone:
   - List all files that must be modified.
   - List any potential regression risks (e.g. broken reports, broken sync jobs).
5. Output a structured summary:
   - File Impact List
   - Dependency Warnings
   - Estimated Risk Level (Low / Medium / High)
   - Suggested verification steps
6. Write these findings to `docs/session-log.md` under a new session heading.
