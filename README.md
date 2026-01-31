# 🧹✨ Windows Cleanup Tool

A comprehensive C# .NET console application that performs extensive Windows system cleanup and privacy trace removal.

## 🎯 What Gets Cleared

### 💻 System Files & Caches
- ✅ Windows temporary files (`%TEMP%`, `C:\Windows\Temp`)
- ✅ Hibernation file (`hiberfil.sys`)
- ✅ Page file cleared on shutdown/restart (configured via registry)
- ✅ Thumbnail cache (image previews)
- ✅ Icon cache (`IconCache.db`)
- ✅ Font cache (`FNTCACHE.DAT`)
- ✅ Prefetch files (program execution tracking)
- ✅ Windows Defender scan history and cache
- ✅ System Restore Points (⚠️ requires admin, irreversible)
- ✅ Shadow Copies / Previous Versions (⚠️ requires admin, irreversible)

### 👤 User Activity & Recent History
- ✅ Recent documents folder
- ✅ Jump lists (taskbar recent items)
- ✅ Quick Access history
- ✅ Start Menu recent items and recommended section (Windows 11)
- ✅ PowerShell command history (system-wide)
- ✅ Command Prompt Run dialog history
- ✅ SSH command history in `.ssh` folder (keys preserved)
- ✅ Git Bash history (`~/.bash_history`, `~/.bash_logout`)
- ✅ WSL shell histories (bash, zsh, fish) across all distributions
- ✅ File Explorer typed paths
- ✅ File Explorer address bar history
- ✅ Recent file dialogs (ComDlg32 MRU)
- ✅ GitHub Copilot CLI session state, cache, logs, and telemetry
- ✅ VS Code / VS Code Insiders Copilot cache and workspace storage
- ✅ Windows Copilot+ (AI features) cache, logs, and telemetry
- ✅ Windows AI Platform services and ML model cache
- ✅ Copilot Runtime and DirectML cache
- ✅ Windows Studio Effects, Live Captions, Voice Access cache
- ✅ Windows Recall feature cache (if enabled)
- ✅ Edge Copilot sidebar cache and Bing Chat integration
- ✅ NPU (Neural Processing Unit) related cache
- ✅ Scoop package manager cache (downloaded installers)
- ✅ WinGet logs and diagnostic files

### 📁 Documents Folder Cleanup
- ✅ PowerShell folder
- ✅ Visual Studio project folders
- ✅ Custom Office Templates folder

**Note:** Only these specific folders are removed from Documents. All other folders and user data remain completely untouched.

### 🌐 Browser Data (Auto-detected)
**Supported Browsers:**
- 🔷 Microsoft Edge (Stable, Dev, Beta, Canary/Insider)
- 🔵 Google Chrome (Stable, Beta, Dev, Canary)
- 🦁 Brave (Stable, Beta, Dev, Nightly)
- 🎭 Opera (Regular, GX, Beta, Developer)
- 🎨 Vivaldi (Stable, Snapshot)
- 🦊 Mozilla Firefox (Stable, Beta, Developer Edition, Nightly)
- 🌐 Internet Explorer (Legacy cache, cookies, temp files)

**Clears:**
- Cache files (main cache, code cache, GPU cache)
- Service worker cache
- Browsing history
- Download history
- Cookies
- Web data (form history, search history)
- Local storage
- Session storage

### 🪟 Windows Features & Services
- ✅ Clipboard history (Windows 10/11)
- ✅ Timeline/Activity history (cross-device tracking)
- ✅ Windows Search history and index data
- ✅ Cortana data and cache
- ✅ Windows Spotlight lock screen images
- ✅ Windows notification history and database
- ✅ Windows Tips and Help history
- ✅ Windows Sandbox logs, caches, and container traces
- ✅ Windows Firewall logs and pfirewall.log files

### 🛠️ Development Tools & Caches
- ✅ Node.js/npm cache and node-gyp logs
- ✅ NuGet package cache (v3-cache)
- ✅ Cargo (Rust) registry cache and target folders
- ✅ Go module cache
- ✅ Composer (PHP) cache
- ✅ Python pip/pytest/mypy/tox cache
- ✅ Ruby Gem and Bundler cache
- ✅ Perl CPAN cache
- ✅ Nim nimble cache
- ✅ Java Maven/Gradle/Ivy cache
- ✅ Jupyter notebook checkpoints (.ipynb_checkpoints)
- ✅ kubectl cache
- ✅ Azure CLI logs, commands, and telemetry
- ✅ AWS CLI cache
- ✅ Postman logs and cache
- ✅ DBeaver workspace logs

### 🎨 Creative & Productivity Apps
- ✅ Adobe applications (Photoshop, Illustrator, Premiere Pro, After Effects, Acrobat, Lightroom, Creative Cloud, and 15+ more)
- ✅ JetBrains IDEs (IntelliJ IDEA, PyCharm, WebStorm, Rider, CLion, GoLand, RustRover, PhpStorm, RubyMine, DataGrip, Android Studio, Fleet, AppCode)
- ✅ Visual Studio (cache, temp files, logs)
- ✅ VS Code (cache, logs, crash dumps)
- ✅ GitHub Copilot (CLI, VS Code extension)
- ✅ Claude Desktop / Claude Code (cache, logs, crash dumps)

### 📦 Virtualization & Containers
- ✅ Hyper-V logs and event traces (ETL files)
- ✅ Hyper-V VM logs (not checkpoints)
- ✅ Windows Sandbox container logs
- ✅ Docker logs and diagnostics
- ✅ VirtualBox logs and dumps
- ✅ VMware logs, dumps, and cache

### 🎮 Graphics & Drivers
- ✅ NVIDIA shader cache (DXCache, GLCache) and driver logs
- ✅ AMD shader cache (DxCache, VkCache) and driver logs
- ✅ Intel shader cache and driver logs
- ✅ Qualcomm Snapdragon (Adreno GPU) shader cache and logs
- ✅ DirectX Shader Cache (D3DSCache)

### 🌍 Network & Connectivity
- ✅ Network share MRU (mapped drives history)
- ✅ Network location history
- ✅ Remote Desktop connection history (server list)
- ✅ VPN connection logs
- ✅ Cloudflare WARP logs, cache, crash reports, and diagnostics
- ✅ DNS cache (flushed)

### 📱 Application Caches & Logs
**Microsoft Apps:**
- ✅ Microsoft Teams (cache, blob storage, GPU cache, databases, temp files)
- ✅ OneDrive logs, cache, telemetry, and setup logs (sync files NOT affected)
- ✅ Windows Store cache
- ✅ Microsoft Office all applications (Word, Excel, PowerPoint, Outlook, OneNote, Access, Publisher, Teams) - logs, temp files, caches
- ✅ Windows Photos app (cache, temp files, logs, history)
- ✅ Windows Media Player (cache, logs, album art, recent playlists)
- ✅ Windows Camera app (cache, temp state, face analysis)
- ✅ Windows Snipping Tool / Screen Sketch (cache, temp state)
- ✅ Windows Terminal (all variants: stable, preview, canary) - tab backups, temp state, cache
- ✅ Windows Backup (logs, event traces)

**Communication & Collaboration:**
- ✅ 🎵 Spotify (cache, persistent cache, data storage)
- ✅ 💬 Slack (cache, code cache, GPU cache, service worker cache, logs)
- ✅ 📹 Zoom (logs, VDI cache)
- ✅ ✈️ Telegram Desktop (emoji cache, user data cache, logs)
- ✅ 📱 WhatsApp Desktop (cache, temp state)
- ✅ 💬 Discord (cache, GPU cache, logs - auth preserved)
- ✅ 💬 Signal Desktop (cache, logs - auth preserved)

**Productivity & Tools:**
- ✅ 📝 Notion (cache, code cache, GPU cache, logs)
- ✅ ✅ Todoist (cache, code cache, GPU cache)
- ✅ 📊 Asana (cache, code cache, GPU cache)
- ✅ 🔐 1Password (logs, cache - credentials NOT affected)
- ✅ 🐙 GitHub Desktop (logs, cache, GPU cache, crash reports, service worker)
- ✅ 🖱️ PowerToys (logs, caches, temp files, crash dumps for all modules)
- ✅ 🖱️ Logitech Options/Logi Tune (logs, caches, crash reports, temp files)

**Media & Entertainment:**
- ✅ 🎮 Steam (logs, HTML cache, dumps, appcache, overlay cache, all install locations)
- ✅ 🎬 VLC Media Player (logs, crash dumps, media library cache, album art cache)
- ✅ 🍎 iTunes (cache, logs, temp files)
- ✅ 🎵 Apple Music (cache, logs)
- ✅ 📺 Apple TV (cache, logs)
- ✅ 📱 Apple Devices (cache, logs)

**Other Apps:**
- ✅ 🐳 Docker (logs, diagnostics)
- ✅ 🐍 Python pip cache
- ✅ 🦕 Deno cache (gen, deps)
- ✅ 🔒 Proton VPN (logs, diagnostic logs, cache, crash reports, WireGuard logs, temp files, connection history)
- ✅ 💾 CrystalDiskMark (logs, temp files)
- ✅ 🔊 Dolby Access & Dolby Atmos (cache, temp state)
- ✅ 🔧 CMake (logs, cache)
- ✅ 💻 Lenovo Commercial Vantage (logs, cache, temp state)
- ✅ 🎥 Clipchamp (cache, temp state)
- ✅ 🗜️ NanaZip & NanaZip Preview (cache, temp state)
- ✅ 📁 7-Zip (logs, temp files)
- ✅ ⬇️ Aria2 download manager (logs)
- ✅ 👆 Synaptics Fingerprint Reader (logs, cache)
- ✅ 🤖 Ollama (AI model runner - logs and cache)
- ✅ 📹 yt-dlp / youtube-dl (cache and history)
- ✅ 🎬 ffmpeg (temp files and logs)

**Smart App Detection:**
- ✅ **Program Files scanner**: Automatically scans all apps in Program Files and Program Files (x86) for logs/caches/temp folders
- ✅ **All detected applications**: Automatically scans AppData and clears cache/logs/temp/history folders for all installed applications
- ✅ **Comprehensive installed app scanner**: Detects ALL installed applications from:
  - Windows Registry (Uninstall keys)
  - Microsoft Store apps (WindowsApps)
  - Running Windows Services
  - Scoop package manager
  - WinGet package manager
  - Chocolatey package manager
  - AppData folders
  - Then clears logs, caches, temp files, and crash dumps for ALL detected apps

### 📊 System Logs & Diagnostics
- ✅ Crash dumps (`%LOCALAPPDATA%\CrashDumps`, `C:\Windows\Minidump`)
- ✅ Windows Error Reporting files
- ✅ Windows Event Logs (Application, System, Security - requires admin)
- ✅ Windows telemetry and diagnostic data
- ✅ Live kernel reports
- ✅ Memory dump files (MEMORY.DMP)
- ✅ Windows Installer logs (CBS logs)
- ✅ Windows Panther setup logs
- ✅ Windows Performance Diagnostics (WER)
- ✅ GameDVR/Xbox Game Bar clips metadata
- ✅ Windows Defender scan history (service logs only)
- ✅ Microsoft Edge update cache
- ✅ BITS (Background Intelligent Transfer Service) database
- ✅ Windows Performance Recorder traces (ETL files - ALL logs)
- ✅ Windows Application Compatibility cache (AmCache)
- ✅ System Resource Usage Monitor (SRUM) backups
- ✅ Windows Notification history database
- ✅ Windows Installer rollback files (ALL patches)
- ✅ WinSxS component store backups
- ✅ SetupAPI device installation logs (ALL logs)
- ✅ DISM logs
- ✅ Windows Terminal tab backups and temp state
- ✅ Windows Insider Program diagnostic logs
- ✅ Windows Compatibility Telemetry (DiagTrack ETL files - ALL logs)
- ✅ DirectX installation logs
- ✅ Windows Activation logs
- ✅ Windows Performance Analyzer traces (ALL logs)
- ✅ Reliability Monitor state data (ALL logs)
- ✅ COM+ event dump files
- ✅ IIS logs (if installed - ALL logs)
- ✅ SQL Server error logs (if installed - ALL logs)
- ✅ RecentFileCache.bcf (file access tracking)
- ✅ Search logs (Windows Search service logs)
- ✅ Print Spooler logs and cache
- ✅ Task Scheduler logs
- ✅ Windows Feedback Hub logs and diagnostic data
- ✅ Windows upgrade folders ($WINDOWS.~BT, $WINDOWS.~WS, Windows.old)

### 🔄 Windows Update & Maintenance
- ✅ Windows Update download cache
- ✅ Delivery Optimization cache (P2P update files, NetworkService profile)
- ✅ Windows.old folder (previous Windows installation)
- ✅ Windows Installer cache (orphaned installers)
- ✅ Windows upgrade installation files (all variants)

### 🗂️ Registry Traces (User Activity Tracking)
- ✅ UserAssist (program usage statistics and launch count)
- ✅ ComDlg32 MRU lists (recent file dialog history)
- ✅ Run dialog MRU
- ✅ Typed paths registry entries
- ✅ Last visited folders
- ✅ Office recent documents registry entries (Word, Excel, PowerPoint, Access - all versions)
- ✅ WordWheelQuery (Windows Search typed queries)
- ✅ StreamMRU (file streams)
- ✅ Find Computer MRU (network computer searches)
- ✅ Internet Explorer/Edge Typed URLs
- ✅ Notepad recent files
- ✅ Paint recent files
- ✅ IME (Input Method Editor) history for Asian languages
- ✅ Map Network Drive MRU (network path history)
- ✅ Remote Desktop Connection history (server list and MRU)
- ✅ Windows Shell Bags (folder view settings and navigation history)
- ✅ Task Manager column preferences (cache-like data)
- ✅ Windows Update detection history cache
- ✅ File type Open With lists (MRU)
- ✅ Explorer Feature Usage tracking
- ✅ Windows Search Recent Apps
- ✅ Program Compatibility Assistant Store (execution tracking)
- ✅ Terminal Server Client history
- ✅ User File History (UFH/SHC) tracking
- ✅ MUICache in Classes (executable names)
- ✅ AppKey tracking
- ✅ Jump Lists registry entries
- ✅ Timeline/Activity Feed registry data
- ✅ Cortana registry traces
- ✅ Notifications registry data

### 🧹 Comprehensive Disk Cleanup
- ✅ Runs Windows Disk Cleanup non-interactively with 25+ options enabled:
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

## 🛡️ What's NOT Cleared (Intentionally Excluded)

### 🔐 Security & Credentials
- ❌ Windows Credential Manager (saved passwords)
- ❌ Browser saved passwords
- ❌ SSH keys (`~/.ssh`)
- ❌ Certificate store (personal certificates)
- ❌ BitLocker recovery keys
- ❌ Browser cookies (login sessions preserved)
- ❌ Browser Local Storage / Session Storage (auth tokens preserved)
- ❌ Browser IndexedDB (app data preserved)

### 🌐 Network & Connectivity
- ❌ WiFi network passwords and profiles
- ❌ VPN connection configurations (only logs cleared)

### 💾 User Data
- ❌ Downloads folder
- ❌ Documents, Pictures, Videos, Music
- ❌ Desktop files
- ❌ OneDrive synced files (only logs/cache cleared)
- ❌ Application settings and configurations
- ❌ Installed programs and features
- ❌ Browser bookmarks and extensions

## 📋 Requirements

- .NET 10.0 SDK or later (for building)
- Windows 10/11 (ARM64 or x64)
- Administrator privileges (recommended for full functionality)

## 🔨 Building

### Optimized Release Build (Native AOT)
Build standalone executables for ARM64 and x64:

```powershell
# ARM64 build
dotnet publish -c Release -r win-arm64 --self-contained

# x64 build  
dotnet publish -c Release -r win-x64 --self-contained
```

This creates highly optimized executables (~2.7-2.8 MB each):
- `WindowsCleanup\bin\Release\net10.0\win-arm64\publish\WindowsCleanup.exe`
- `WindowsCleanup\bin\Release\net10.0\win-x64\publish\WindowsCleanup.exe`

**Native AOT Benefits:**
- ⚡ Instant startup (no JIT compilation)
- 📦 Single standalone .exe (no .NET runtime required)
- 🚀 Optimized for speed with full IL trimming
- 💾 Smaller memory footprint

## 🚀 Running

### 🔍 Dry-Run Mode (Recommended First Time)
Test what the tool will delete without actually deleting anything:

```powershell
WindowsCleanup.exe --dry-run
```

This creates a timestamped log file (e.g., `dry-run-20260131-120000.txt`) listing all files and operations that would be performed.

### ✅ Normal Execution

#### Option 1: Run as Administrator (Recommended)
Right-click on `WindowsCleanup.exe` and select "Run as administrator"

The tool will automatically prompt to elevate to administrator if needed.

#### Option 2: From PowerShell
```powershell
.\WindowsCleanup.exe
```

### 📝 Command-Line Arguments
- `--dry-run` - Preview mode: logs all operations without executing them

## ⚠️ Important Warnings

**Critical Notices:**
- 🔴 **System Restore Points will be DELETED** - This is irreversible! You won't be able to restore Windows to a previous state.
- 🔴 **Shadow Copies will be DELETED** - The "Previous Versions" feature will no longer work. You cannot recover old file versions.
- 🟡 Running this tool will delete browsing history, download history, and cached data from all detected browsers
- 🟡 PowerShell and command history will be permanently deleted
- 🟡 Recent file lists will be cleared (you won't see recent documents in File Explorer or Start Menu)
- 🟡 ALL logs, caches, and temp files are deleted regardless of age - no time-based filtering is applied
- 🟡 Page file will be cleared on next shutdown/restart (slightly slower shutdown time)
- 🟡 Some cleanup operations require administrator privileges
- 🟡 Close all browsers, IDEs, and applications before running for best results

**🔐 Authentication & Sessions Preserved:**
- ✅ Browser login sessions preserved (cookies, Local Storage, Session Storage, IndexedDB)
- ✅ Application credentials and auth tokens preserved
- ✅ SSH keys and certificates preserved
- ✅ License keys and activation data preserved

## 💡 Best Practices

- ✅ **First time users**: Run with `--dry-run` flag to preview what will be deleted
- ✅ Run as administrator for maximum effectiveness
- ✅ Close all applications before running
- ✅ Review the dry-run log file before executing actual cleanup
- ✅ The tool shows progress and statistics for each operation
- ✅ Some operations may be skipped if files are in use
- ✅ Create a System Restore Point BEFORE running if you want to preserve that option

## 🎯 Features Explained

### 🤖 Smart Browser Detection
The tool automatically detects installed browsers (all release channels) and clears their data. If a browser is currently running, some cleanup operations may be skipped with a warning message.

### 🔍 Comprehensive App Scanner
Scans multiple sources to detect ALL installed applications:
- Registry (Uninstall keys)
- Microsoft Store apps
- Running Windows Services (extracts app names)
- Package managers (Scoop, WinGet, Chocolatey)
- AppData folders

Then intelligently clears logs, caches, and temp files while preserving authentication data.

### 🔒 Authentication Safety
The tool has built-in safeguards to NEVER delete:
- Browser cookies
- Local Storage / Session Storage
- IndexedDB databases
- Application credential files
- License keys

This ensures you stay logged in to all your applications after cleanup.

### 🎭 System-Wide Cleanup
When run as administrator, the tool clears PowerShell history and other traces for all users on the system, not just the current user.

### 🛡️ Safe Operation
The tool uses try-catch blocks extensively to skip files/folders that are in use or inaccessible, ensuring it won't crash if something can't be deleted.

## 📁 Project Structure

```
D:\cleanup\
├── README.md                       # This file
├── LICENSE                         # MIT License
└── WindowsCleanup\
    ├── Program.cs                 # Main application code (~14,600 lines)
    └── WindowsCleanup.csproj      # Project file with AOT config
```

## 🔒 Privacy & Security

This tool is designed to clean privacy traces and free up disk space. It does NOT:
- 🚫 Send any data over the network
- 🚫 Modify system files outside of standard cleanup locations
- 🚫 Delete user documents or important files
- 🚫 Require internet access
- 🚫 Install any services or background processes
- 🚫 Collect telemetry or analytics

All operations are performed locally and the tool exits when complete.

## ⚡ Performance & Optimization

The release build uses .NET 10 Native AOT (Ahead-of-Time) compilation for optimal performance:

**Optimization Features:**
- ✅ Native AOT compilation - compiles to native machine code
- ✅ Single-file deployment - no external dependencies
- ✅ Full IL trimming - removes unused code
- ✅ Optimized for speed - ILC optimization preference set to Speed
- ✅ Instant startup - no JIT compilation overhead
- ✅ Small binary size (~2.6 MB standalone executable)
- ✅ No .NET runtime required on target machine
- ✅ Supports both ARM64 and x64 architectures

**Performance Comparison:**
- Standard build: ~30ms startup time, requires .NET runtime
- AOT build: <5ms startup time, self-contained

## 📜 License

MIT License - See LICENSE file for details.

## 🤝 Contributing

This is a personal cleanup tool. Feel free to fork and modify for your own needs.

## ⚠️ Disclaimer

**USE AT YOUR OWN RISK.** This tool permanently deletes files including System Restore Points and Shadow Copies. Always run with `--dry-run` first to review what will be deleted. The authors are not responsible for any data loss.

## 🎉 Version History

See [Releases](https://github.com/YOUR_USERNAME/cleanup/releases) for version history and changelog.

---

**Made with 💙 for Windows privacy and performance**
