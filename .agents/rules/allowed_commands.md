# Workspace Allowed Commands Rule

The agent is explicitly authorized to execute the following interpreters and commands without restriction:

- **PowerShell**: `powershell`, `pwsh`, `.ps1` scripts, `Get-ChildItem` (`get-children`), `Get-Content`, `Set-Content`, `Test-Path`, `New-Item`, `Add-Type`.
- **Python**: `python`, `python3`, `py`, `pip`.
- **Unity**: `Unity.exe` batchmode builds and verifications.
