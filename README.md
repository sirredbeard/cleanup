# 🧹 Windows Cleanup Tool

A comprehensive C# .NET console application that performs extensive Windows system cleanup and privacy trace removal.

## 🎯 What Gets Cleared

### 💻 System Files & Caches
- ✓ Windows temporary files (`%TEMP%`, `C:\Windows\Temp`)
- ✓ Hibernation file (`hiberfil.sys`)
- ✓ Page file cleared on shutdown/restart (configured via registry)
- ✓ Thumbnail cache (image previews)
- ✓ Icon cache (`IconCache.db`)
- ✓ Font cache (`FNTCACHE.DAT`)
- ✓ Prefetch files (program execution tracking)
- ✓ Windows Defender scan history and cache

### 👤 User Activity & Recent History
- ✓ Recent documents folder
- ✓ Jump lists (taskbar recent items)
- ✓ Quick Access history
- ✓ Start Menu recent items and recommended section (Windows 11)
- ✓ PowerShell command history (system-wide)
- ✓ Command Prompt Run dialog history
- ✓ SSH command history in `.ssh` folder (keys preserved)
- ✓ Git Bash history (`~/.bash_history`, `~/.bash_logout`)
- ✓ WSL shell histories (bash, zsh, fish) across all distributions
- ✓ File Explorer typed paths
- ✓ File Explorer address bar history
- ✓ Recent file dialogs (ComDlg32 MRU)
- ✓ GitHub Copilot CLI session state, cache, logs, and telemetry
- ✓ VS Code / VS Code Insiders Copilot cache and workspace storage
- ✓ Windows Copilot+ (AI features) cache, logs, and telemetry
- ✓ Windows AI Platform services and ML model cache
- ✓ Copilot Runtime and DirectML cache
- ✓ Windows Studio Effects, Live Captions, Voice Access cache
- ✓ Windows Recall feature cache (if enabled)
- ✓ Edge Copilot sidebar cache and Bing Chat integration
- ✓ NPU (Neural Processing Unit) related cache
- ✓ Scoop package manager cache (downloaded installers)
- ✓ WinGet logs and diagnostic files

### 🌐 Browser Data (Auto-detected)
**Supported Browsers:**
- Microsoft Edge (Stable, Dev, Beta, Canary/Insider)
- Google Chrome
- Mozilla Firefox
- Brave
- Opera (Regular & Opera GX)
- Vivaldi

**Clears:**
- Cache files (main cache, code cache, GPU cache)
- Service worker cache
- Browsing history
- Cookies
- Web data (form history, search history)
- Local storage
- Session storage

### 🪟 Windows Features & Services
- ✓ Clipboard history (Windows 10/11)
- ✓ Timeline/Activity history (cross-device tracking)
- ✓ Windows Search history and index data
- ✓ Cortana data and cache
- ✓ Windows Spotlight lock screen images
- ✓ Windows notification history and database
- ✓ Windows Tips and Help history
- ✓ Windows Sandbox logs, caches, and container traces
- ✓ Windows Firewall logs and pfirewall.log files

### 🛠️ Development Tools & Caches
- ✓ Node.js/npm cache and node-gyp logs
- ✓ NuGet package cache (v3-cache)
- ✓ Cargo (Rust) registry cache
- ✓ Go module cache
- ✓ Composer (PHP) cache
- ✓ Jupyter notebook checkpoints (.ipynb_checkpoints)
- ✓ kubectl cache
- ✓ Azure CLI logs, commands, and telemetry
- ✓ AWS CLI cache
- ✓ Postman logs and cache
- ✓ DBeaver workspace logs

### 📦 Virtualization & Containers
- ✓ Hyper-V logs and event traces (ETL files)
- ✓ Hyper-V VM logs (not checkpoints)
- ✓ Windows Sandbox container logs
- ✓ Docker logs and diagnostics

### 🎮 Graphics & Drivers
- ✓ NVIDIA shader cache (DXCache, GLCache) and driver logs
- ✓ AMD shader cache (DxCache, VkCache) and driver logs
- ✓ Intel shader cache and driver logs
- ✓ Qualcomm Snapdragon (Adreno GPU) shader cache and logs
- ✓ DirectX Shader Cache (D3DSCache)

### 🌍 Network & Connectivity
- ✓ Network share MRU (mapped drives history)
- ✓ Network location history
- ✓ Remote Desktop connection history (server list)
- ✓ VPN connection logs
- ✓ Cloudflare WARP logs, cache, crash reports, and diagnostics (if installed)
- ✓ DNS cache (flushed)

### 📱 Application Caches & Logs
- ✓ Microsoft Teams (cache, blob storage, GPU cache, databases, temp files)
- ✓ OneDrive logs, cache, telemetry, and setup logs (sync files NOT affected)
- ✓ Windows Store cache
- ✓ Microsoft Office all applications (Word, Excel, PowerPoint, Outlook, OneNote, Access, Publisher, Teams) - logs, temp files, caches
- ✓ Windows Photos app (cache, temp files, logs, history)
- ✓ Windows Media Player (cache, logs, album art, recent playlists)
- ✓ Windows Store app packages cache
- ✓ Windows Camera app (cache, temp state, face analysis)
- ✓ Windows Snipping Tool / Screen Sketch (cache, temp state)
- ✓ Windows Terminal (all variants: stable, preview, canary) - tab backups, temp state, cache
- ✓ Windows Backup (logs, event traces)
- ✓ 🎵 Spotify (cache, persistent cache, data storage)
- ✓ 💬 Slack (cache, code cache, GPU cache, service worker cache, logs, enhanced)
- ✓ 📹 Zoom (logs, VDI cache)
- ✓ ✈️ Telegram Desktop (emoji cache, user data cache, logs)
- ✓ 📱 WhatsApp Desktop (cache, temp state)
- ✓ 🔐 1Password (logs, cache - credentials NOT affected)
- ✓ 📝 Notion (cache, code cache, GPU cache, logs)
- ✓ ✅ Todoist (cache, code cache, GPU cache)
- ✓ 📊 Asana (cache, code cache, GPU cache)
- ✓ 🐙 GitHub Desktop (logs, cache, GPU cache, crash reports, service worker, enhanced)
- ✓ 🎮 Steam (logs, HTML cache, dumps, appcache, overlay cache, all install locations, enhanced)
- ✓ 🎬 VLC Media Player (logs, crash dumps, media library cache, album art cache)
- ✓ 🐳 Docker (logs, diagnostics)
- ✓ 🐍 Python pip cache
- ✓ 🦕 Deno cache (gen, deps)
- ✓ 🔒 Proton VPN (logs, diagnostic logs, cache, crash reports, WireGuard logs, temp files, connection history)
- ✓ 🖱️ PowerToys (logs, caches, temp files, crash dumps for all modules)
- ✓ 🖱️ Logitech Options/Logi Tune (logs, caches, crash reports, temp files)
- ✓ Postman (logs, cache, GPU cache, code cache)
- ✓ DBeaver (workspace logs, connection logs)
- ✓ 💾 CrystalDiskMark (logs, temp files)
- ✓ 🔊 Dolby Access & Dolby Atmos (cache, temp state)
- ✓ 🔧 CMake (logs, cache)
- ✓ 💻 Lenovo Commercial Vantage (logs, cache, temp state)
- ✓ 🎥 Clipchamp (cache, temp state)
- ✓ 🗜️ NanaZip & NanaZip Preview (cache, temp state)
- ✓ 📁 7-Zip (logs, temp files)
- ✓ ⬇️ Aria2 download manager (logs)
- ✓ 👆 Synaptics Fingerprint Reader (logs, cache)
- ✓ **Prism Cache**: ARM to x86/x64 emulation cache (for ARM64 Windows devices)
- ✓ **Program Files scanner**: Automatically scans all apps in Program Files and Program Files (x86) for logs/caches/temp folders
- ✓ **All detected applications**: Automatically scans AppData and clears cache/logs/temp/history folders for all installed applications
- ✓ **Comprehensive installed app scanner**: Detects ALL installed applications from:
  - Windows Registry (Uninstall keys)
  - Microsoft Store apps (WindowsApps)
  - Scoop package manager
  - WinGet package manager
  - Chocolatey package manager
  - AppData folders
  - Then clears logs, caches, temp files, and crash dumps for ALL detected apps

### 📊 System Logs & Diagnostics
- ✓ Crash dumps (`%LOCALAPPDATA%\CrashDumps`, `C:\Windows\Minidump`)
- ✓ Windows Error Reporting files
- ✓ Windows Event Logs (Application, System, Security - requires admin)
- ✓ Windows telemetry and diagnostic data
- ✓ Live kernel reports
- ✓ Memory dump files
- ✓ Windows Installer logs (CBS logs)
- ✓ Windows Panther setup logs
- ✓ Windows Performance Diagnostics (WER)
- ✓ GameDVR/Xbox Game Bar clips metadata
- ✓ Windows Defender scan history (service logs only)
- ✓ Microsoft Edge update cache
- ✓ BITS (Background Intelligent Transfer Service) database
- ✓ Windows Performance Recorder traces (ETL files - ALL logs)
- ✓ Windows Application Compatibility cache
- ✓ System Resource Usage Monitor (SRUM) backups
- ✓ Windows Notification history database
- ✓ Windows Installer rollback files (ALL patches)
- ✓ WinSxS component store backups
- ✓ SetupAPI device installation logs (ALL logs)
- ✓ DISM logs
- ✓ Windows Terminal tab backups and temp state
- ✓ Windows Insider Program diagnostic logs
- ✓ Windows Compatibility Telemetry (DiagTrack ETL files - ALL logs)
- ✓ DirectX installation logs
- ✓ Windows Activation logs
- ✓ Windows Performance Analyzer traces (ALL logs)
- ✓ Reliability Monitor state data (ALL logs)
- ✓ COM+ event dump files
- ✓ IIS logs (if installed - ALL logs)
- ✓ SQL Server error logs (if installed - ALL logs)

### 🔄 Windows Update & Maintenance
- ✓ Windows Update download cache
- ✓ Delivery Optimization cache (P2P update files)
- ✓ Windows.old folder (previous Windows installation)
- ✓ Windows Installer cache (orphaned installers)

### 🗂️ Registry Traces
- ✓ UserAssist (program usage statistics and launch count)
- ✓ ComDlg32 MRU lists (recent file dialog history)
- ✓ Run dialog MRU
- ✓ Typed paths registry entries
- ✓ Last visited folders
- ✓ Office recent documents registry entries (Word, Excel, PowerPoint, Access - all versions)
- ✓ WordWheelQuery (Windows Search typed queries)
- ✓ StreamMRU (file streams)
- ✓ Find Computer MRU (network computer searches)
- ✓ Internet Explorer/Edge Typed URLs
- ✓ Notepad recent files
- ✓ Paint recent files
- ✓ IME (Input Method Editor) history for Asian languages
- ✓ Map Network Drive MRU
- ✓ Remote Desktop Connection history (server list)
- ✓ Windows Shell Bags (folder view settings and navigation history)
- ✓ Task Manager column preferences (cache-like data)
- ✓ Windows Update detection history cache
- ✓ File type Open With lists (MRU)

### Comprehensive Disk Cleanup
- ✓ Runs Windows Disk Cleanup non-interactively with 25+ options enabled:
  - Active Setup Temp Folders
  - BranchCache
  - Downloaded Program Files
  - Internet Cache Files
  - Offline Pages Files
  - Old ChkDsk Files
  - Previous Installations
  - Recycle Bin
  - Service Pack Cleanup
  - Setup Log Files
  - System error memory dumps
  - System error minidumps
  - Temporary Files
  - Temporary Setup Files
  - Thumbnail Cache
  - Update Cleanup
  - Upgrade Discarded Files
  - User file versions
  - Windows Defender files
  - Windows Error Reporting files (all types)
  - Windows ESD installation files
  - Windows Upgrade Log Files

## What's NOT Cleared (Intentionally Excluded)

### Security & Credentials
- ❌ Windows Credential Manager (saved passwords)
- ❌ Browser saved passwords
- ❌ SSH keys (`~/.ssh`)
- ❌ Certificate store (personal certificates)
- ❌ BitLocker recovery keys

### Network & Connectivity
- ❌ WiFi network passwords and profiles
- ❌ VPN connection configurations (only logs cleared)

### System & Recovery
- ❌ System Restore Points
- ❌ Shadow Copies (Previous Versions)

### User Data
- ❌ Downloads folder
- ❌ Documents, Pictures, Videos, Music
- ❌ Desktop files
- ❌ OneDrive synced files (only logs/cache cleared)
- ❌ Application settings and configurations
- ❌ Installed programs and features

## Requirements

- .NET 10.0 SDK or later
- Windows OS (Windows 10/11 recommended)
- Administrator privileges (recommended for full functionality)

## Building

### Standard Build
```bash
cd D:\cleanup\WindowsCleanup
dotnet build
```

### Optimized Release Build (AOT Compiled)
For maximum performance, build with AOT (Ahead-of-Time) compilation:
```bash
cd D:\cleanup\WindowsCleanup
dotnet publish -c Release -r win-x64 --self-contained
```

This creates a highly optimized single-file executable (~2.1 MB) at:
`D:\cleanup\WindowsCleanup\bin\Release\net10.0\win-x64\publish\WindowsCleanup.exe`

**AOT Benefits:**
- ⚡ Instant startup (no JIT compilation)
- 📦 Single standalone .exe (no .NET runtime required)
- 🚀 Optimized for speed with full IL trimming
- 💾 Smaller memory footprint

## Running

### Dry-Run Mode (Recommended First Time)
Test what the tool will delete without actually deleting anything:
```bash
WindowsCleanup.exe --dry-run
```

This creates a timestamped log file (e.g., `dry-run-20260130-143052.txt`) listing all files and operations that would be performed.

### Normal Execution

#### Option 1: Using dotnet run (Debug build)
```bash
cd D:\cleanup\WindowsCleanup
dotnet run
```

Or with dry-run:
```bash
dotnet run -- --dry-run
```

#### Option 2: Run the compiled executable
```bash
D:\cleanup\WindowsCleanup\bin\Debug\net10.0\WindowsCleanup.exe
```

#### Option 3: Run the optimized AOT executable
```bash
D:\cleanup\WindowsCleanup\bin\Release\net10.0\win-x64\publish\WindowsCleanup.exe
```

#### Option 4: Run as Administrator (Recommended)
Right-click on `WindowsCleanup.exe` and select "Run as administrator"

### Command-Line Arguments
- `--dry-run` - Preview mode: logs all operations without executing them

## Notes & Warnings

⚠️ **Important Warnings:**
- Running this tool will delete browsing history, cookies, and cached data from all detected browsers
- PowerShell and command history will be permanently deleted
- Recent file lists will be cleared (you won't see recent documents in File Explorer or Start Menu)
- Copilot session history, VS Code Copilot cache, and Windows Copilot+ AI features cache will be removed
- Windows AI/ML model cache and Copilot Runtime data will be cleared
- **ALL logs, caches, and temp files are deleted regardless of age** - no time-based filtering is applied
- Page file will be cleared on next shutdown/restart (slightly slower shutdown time)
- Cloudflare WARP logs and cache will be cleared if installed
- Some cleanup operations require administrator privileges
- Close all browsers and VS Code before running for best results
- The tool is designed to preserve your data (documents, photos, etc.) while removing traces

ℹ️ **Best Practices:**
- **First time users**: Run with `--dry-run` flag to preview what will be deleted
- Run as administrator for maximum effectiveness
- Close all applications before running
- Review the dry-run log file before executing actual cleanup
- The tool shows progress and statistics for each operation
- Some operations may be skipped if files are in use

## Features Explained

### Non-Interactive Disk Cleanup
Unlike the manual Windows Disk Cleanup tool, this application automatically configures all cleanup options via registry and runs the cleanup silently in the background without showing any dialogs.

### Browser Detection
The tool automatically detects installed browsers and clears their data. If a browser is currently running, some cleanup operations may be skipped with a warning message.

### System-Wide PowerShell History
When run as administrator, the tool clears PowerShell history for all users on the system, not just the current user.

### Safe Operation
The tool uses try-catch blocks extensively to skip files/folders that are in use or inaccessible, ensuring it won't crash if something can't be deleted.

## Project Structure

```
D:\cleanup\
├── README.md                    # This file
└── WindowsCleanup\
    ├── Program.cs              # Main application code
    ├── WindowsCleanup.csproj   # Project file
    └── bin\Debug\net10.0\      # Compiled output
        └── WindowsCleanup.exe  # Executable
```

## Privacy & Security

This tool is designed to clean privacy traces and free up disk space. It does NOT:
- Send any data over the network
- Modify system files outside of standard cleanup locations
- Delete user documents or important files
- Require internet access
- Install any services or background processes

All operations are performed locally and the tool exits when complete.

## Performance & Optimization

The release build uses .NET 10 Native AOT (Ahead-of-Time) compilation for optimal performance:

**Optimization Features:**
- ✅ Native AOT compilation - compiles to native machine code
- ✅ Single-file deployment - no external dependencies
- ✅ Full IL trimming - removes unused code
- ✅ Optimized for speed - ILC optimization preference set to Speed
- ✅ Instant startup - no JIT compilation overhead
- ✅ Small binary size (~2.1 MB standalone executable)
- ✅ No .NET runtime required on target machine

**Performance Comparison:**
- Standard build: ~30ms startup time, requires .NET runtime
- AOT build: <5ms startup time, self-contained

The tool is optimized for Windows x64 and takes full advantage of platform-specific optimizations.

## Requirements

- .NET 10.0 SDK or later
- Windows OS
- Administrator privileges (recommended for full functionality)

## Building

```powershell
cd D:\cleanup\WindowsCleanup
dotnet build
```

## Running

### Option 1: Using dotnet run
```powershell
cd D:\cleanup\WindowsCleanup
dotnet run
```

### Option 2: Run the compiled executable
```powershell
D:\cleanup\WindowsCleanup\bin\Debug\net10.0\WindowsCleanup.exe
```

### Option 3: Run as Administrator (Recommended)
Right-click on `WindowsCleanup.exe` and select "Run as administrator"

## Notes

- The application will warn you if not running as administrator
- Some cleanup operations may fail if Edge browser is currently running
- Disk Cleanup will launch as a separate process
- The application shows progress and statistics for each cleanup operation

## Project Structure

```
D:\cleanup\
└── WindowsCleanup\
    ├── Program.cs              # Main application code
    ├── WindowsCleanup.csproj   # Project file
    └── bin\Debug\net10.0\      # Compiled output
        └── WindowsCleanup.exe  # Executable
```
