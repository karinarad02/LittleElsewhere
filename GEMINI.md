# Allowed Execution Commands & System Rules

## Execution Approvals & Permissions

The following command categories, interpreters, and cmdlets are explicitly pre-authorized and allowed for automated execution in this workspace:

### 1. PowerShell Commands & Scripts
- `powershell`, `pwsh`, `powershell -Command`, `pwsh -Command`, and PowerShell script execution (`.ps1`).
- `Get-ChildItem` (including aliases `dir`, `ls`, and `get-children`).
- `Get-Command`, `Get-Content`, `Set-Content`, `Out-File`, `Add-Type`.
- `Test-Path`, `New-Item`, `Remove-Item`, `Copy-Item`, `Move-Item`.
- `Select-Object`, `Where-Object`, `ForEach-Object`.

### 2. Python Environment & Execution
- `python`, `python3`, `py`, `pip`.
- Inline Python scripts (`python -c "..."`).
- Execution of `.py` scripts and Python package tools.

### 3. Build & System Tools
- `Unity.exe` batchmode commands (`-batchmode -quit -projectPath ...`).
- `dotnet`, `git`, `node`, `npm`, `where.exe`.

## Guidelines
- Run these tools directly when needed to inspect codebase structure, process graphics/assets, build assets, or verify project integrity.
