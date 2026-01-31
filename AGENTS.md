# 🤖 Agent Guidelines for Windows Cleanup Tool Repository

This document outlines the standards, procedures, and practices for maintaining this repository. Follow these guidelines when making changes.

## 📋 Table of Contents
- [Version Control](#version-control)
- [Build Process](#build-process)
- [Release Process](#release-process)
- [Code Standards](#code-standards)
- [Testing Procedures](#testing-procedures)
- [Documentation Standards](#documentation-standards)
- [Git Workflow](#git-workflow)

---

## 🔄 Version Control

### Versioning Scheme
- Use semantic versioning: `v0.0.X` (currently in v0.0.x phase)
- Increment version for each release with significant changes
- Version format: `v0.0.5` (not `0.0.5`)

### Commit Message Format
```
v0.0.X - Brief description

Detailed changes:
- Feature 1
- Feature 2
- Breaking change (if any)
```

**Example:**
```
v0.0.5 - Major privacy and cleanup expansion

New Features:
- System Restore Points deletion (requires admin, irreversible)
- Shadow Copies / Previous Versions deletion (requires admin, irreversible)
- Adobe applications support (20+ apps)
- JetBrains IDEs support (13+ tools)
```

### Commit Squashing
- **ALWAYS** squash documentation/minor commits into the main version commit
- Each version should have ONE commit (plus the tag)
- Use `git reset --soft HEAD~1` then `git commit --amend --no-edit` to squash

**Example workflow:**
```powershell
# After making a minor docs change
git reset --soft HEAD~1
git commit --amend --no-edit
git push -f origin master
```

---

## 🏗️ Build Process

### Build Requirements
- .NET 10 SDK
- Windows OS
- Native AOT compilation enabled

### Building Both Architectures
**ALWAYS build BOTH ARM64 and x64 for releases:**

```powershell
# ARM64 build
cd WindowsCleanup
dotnet publish -c Release -r win-arm64 --self-contained

# x64 build
dotnet publish -c Release -r win-x64 --self-contained

# Copy binaries to root
cd ..
Copy-Item "WindowsCleanup\bin\Release\net10.0\win-arm64\publish\WindowsCleanup.exe" "WindowsCleanup-arm64.exe" -Force
Copy-Item "WindowsCleanup\bin\Release\net10.0\win-x64\publish\WindowsCleanup.exe" "WindowsCleanup-x64.exe" -Force
```

### Binary Specifications
- **ARM64**: ~2.6 MB (for Windows on ARM devices)
- **x64**: ~2.6 MB (for Intel/AMD systems)
- Single-file, self-contained executables
- No .NET runtime required
- Native AOT compiled

### Binary Management
- ✅ Binaries are built for each release
- ❌ Binaries are NOT tracked in git (excluded in .gitignore)
- ✅ Binaries are uploaded to GitHub releases only

---

## 🚀 Release Process

### Pre-Release Checklist
1. ✅ All code changes committed
2. ✅ README.md updated with new features and current statistics (no version-specific annotations)
3. ✅ Build both ARM64 and x64 binaries
4. ✅ Test binaries with `--dry-run` mode
5. ✅ Squash commits if needed
6. ✅ Create/update version tag

### Creating a Release

#### Step 1: Squash commits and tag
```powershell
cd D:\cleanup

# Squash if needed (see Version Control section)

# Delete old tag if updating
git tag -d v0.0.X
git push origin :refs/tags/v0.0.X

# Create new tag
git tag -a v0.0.X -m "Version 0.0.X - Brief description"
git push -f origin master
git push origin v0.0.X
```

#### Step 2: Build binaries
```powershell
# Build both architectures (see Build Process section)
```

#### Step 3: Create GitHub release with gh CLI
```powershell
# Delete existing release if updating
gh release delete v0.0.X --yes --repo sirredbeard/cleanup

# Create release with inline notes and binaries
gh release create v0.0.X `
    --title "v0.0.X - Title" `
    --notes "COMPREHENSIVE_RELEASE_NOTES_HERE" `
    --repo sirredbeard/cleanup `
    "WindowsCleanup-arm64.exe#WindowsCleanup-arm64.exe (ARM64 - 2.66 MB)" `
    "WindowsCleanup-x64.exe#WindowsCleanup-x64.exe (x64 - 2.62 MB)"
```

### Release Notes Format

**CRITICAL:** Release notes are included ONLY in GitHub releases, NOT as a separate file in the repository.

**Release notes structure:**
```markdown
# 🎉 Windows Cleanup Tool v0.0.X - Brief Title

## 🔴 CRITICAL NEW FEATURES (if applicable)
- Highlight breaking changes
- Highlight irreversible operations

## 🎨 New Application Support
- List new applications/tools added

## 🌐 Browser Support Additions
- List new browser channels

## 🗂️ Expanded Registry Traces
- List new registry trace categories

## 🎨 UI/UX Improvements
- List interface improvements

## 📊 Statistics
- Total lines of code
- Applications supported
- Browser variants
- Registry categories
- Binary size

## 🔧 Technical Details
- Architectures
- Build technology

## 📦 What's Cleaned (Summary)
- Categorized lists

## 🛡️ What's Preserved
- What authentication/data is kept

## ⚠️ Breaking Changes (if any)
- List breaking changes from previous version

## 💡 Usage Recommendations
- First time users
- Regular users

## 📥 Installation
- Download links
- Usage examples

## ⚠️ Disclaimer
- Standard disclaimer about data loss
```

---

## 💻 Code Standards

### Code Style
- C# with .NET 10
- Native AOT compatible code only
- Try-catch blocks around all file operations
- Continue on error philosophy (don't crash, skip and continue)

### Emoji Usage
**Consistently use emoji throughout the application:**

```csharp
// Headers
Console.WriteLine("  🧹✨ Windows Cleanup Tool 💻");

// Section indicators
Console.WriteLine("🔹 Clearing browser data...");

// Success messages
Console.WriteLine("  ✅ Cleared browser data");

// Errors
Console.WriteLine("  ❌ Failed: {ex.Message}");

// Warnings
Console.WriteLine("  ⚠️ Warning: Not running as administrator");

// Info
Console.WriteLine("  🔵 Dry-run mode: No files will be deleted");

// User prompts
Console.WriteLine("💭 Would you like to restart as administrator? (Y/N)");
```

**Standard emoji mapping:**
- `🔹` - Section headers (replaced `→`)
- `✅` - Success (replaced `✓`)
- `❌` - Error (replaced `✗`)
- `⚠️` - Warning (replaced `⚠`)
- `🔵` - Info messages
- `💭` - User questions

### Authentication Preservation Rules

**CRITICAL: NEVER delete these folders/files:**
```csharp
// Browser authentication
- "Local Storage"
- "Session Storage"
- "IndexedDB"
- "databases"
- "Cookies" folder (only clear cookie FILES with try-catch)

// Application authentication
- Skip folders/files containing credentials
- Preserve license keys
- Preserve SSH keys
```

**Generic app scanner safeguards:**
```csharp
// Skip these patterns
if (folderName.Equals("Local Storage", StringComparison.OrdinalIgnoreCase) ||
    folderName.Equals("Session Storage", StringComparison.OrdinalIgnoreCase) ||
    folderName.Equals("IndexedDB", StringComparison.OrdinalIgnoreCase) ||
    folderName.Equals("databases", StringComparison.OrdinalIgnoreCase))
{
    continue; // SKIP - authentication data
}
```

### Dry-Run Consistency

**All delete operations MUST support dry-run:**

```csharp
if (_dryRun)
{
    LogDryRun($"Would delete: {filePath}");
}
else
{
    File.Delete(filePath);
}
```

**Console output format:**
```csharp
Console.WriteLine($"  ✅ {(_dryRun ? "Would clear" : "Cleared")} {count} items");
```

**Log format:**
```csharp
LogDryRun($"Would delete file: {path}");
LogDryRun($"Would delete directory: {path} ({FormatBytes(size)})");
```

---

## 🧪 Testing Procedures

### Pre-Release Testing

**ALWAYS test with dry-run before releasing:**

```powershell
cd WindowsCleanup\bin\Release\net10.0\win-arm64\publish

# Run dry-run test
.\WindowsCleanup.exe --dry-run

# Review the generated log file
# Look for:
# - New features are logged
# - No crashes/errors
# - "Would delete:" messages (not "Cleaned" in dry-run)
```

### Testing Checklist
- ✅ Dry-run completes without crashes
- ✅ Log file is generated with timestamp
- ✅ New features appear in log output
- ✅ Emoji display correctly in console
- ✅ Admin prompt appears when not elevated
- ✅ All delete messages say "Would delete:" in dry-run mode

---

## 📚 Documentation Standards

### README.md Updates

**ALWAYS update README.md with each new version:**

1. **Update feature lists** - Add new applications, browsers, registry traces
2. **Update statistics** - Lines of code, app count, browser count, binary sizes
3. **Update "What Gets Cleared"** - Categorize new cleanup targets
4. **Update "What's NOT Cleared"** - If authentication preserved
5. **Use consistent emoji** - Match the emoji used in code
6. **Update line counts** - Reflect actual Program.cs line count in project structure section
7. **Update binary sizes** - Reflect actual compiled sizes in build instructions

**IMPORTANT: Do NOT mention specific version numbers in README.md**
- README.md should be version-agnostic, describing current features
- Do NOT add "New in v0.0.X" or "v0.0.X -" annotations
- Version history belongs in GitHub releases only
- Keep README focused on what the tool does now, not when features were added

**README.md sections to update:**
```markdown
## 🎯 What Gets Cleared
### 💻 System Files & Caches
### 👤 User Activity & Recent History
### 🌐 Browser Data (Auto-detected)
### 🛠️ Development Tools & Caches
### 🎨 Creative & Productivity Apps
### 📦 Virtualization & Containers
### 🎮 Graphics & Drivers
### 🌍 Network & Connectivity
### 📱 Application Caches & Logs
### 📊 System Logs & Diagnostics
### 🔄 Windows Update & Maintenance
### 🗂️ Registry Traces
### 🧹 Comprehensive Disk Cleanup
```

**Emoji usage in README:**
- ✅ Use checkmarks for feature lists
- ❌ Use X marks for excluded features
- 🎯 Use category emoji (🌐, 💻, 📱, etc.)
- Keep emoji consistent with console output

### Project Structure in README

**Keep this minimal (no build artifacts):**
```markdown
## 📁 Project Structure

```
D:\cleanup\
├── README.md                       # This file
├── LICENSE                         # MIT License
└── WindowsCleanup\
    ├── Program.cs                 # Main application code (~12,000 lines)
    └── WindowsCleanup.csproj      # Project file with AOT config
```
```

---

## 🔀 Git Workflow

### Branch Strategy
- **master branch**: Production-ready code
- Tags mark releases (v0.0.1, v0.0.2, etc.)
- No separate dev branches (small project)

### Force Push Guidelines

**When to force push:**
- ✅ Squashing commits for cleaner history
- ✅ Updating tags after squashing
- ⚠️ ONLY on personal/single-maintainer repos
- ❌ NEVER on collaborative projects

**Force push commands:**
```powershell
git push -f origin master       # After squashing commits
git push -f origin v0.0.X       # After updating tag
```

### Tag Management

**Updating a tag (after squashing commits):**
```powershell
# Delete local tag
git tag -d v0.0.X

# Delete remote tag
git push origin :refs/tags/v0.0.X

# Create new tag
git tag -a v0.0.X -m "Version 0.0.X - Description"

# Push new tag
git push origin v0.0.X
```

### Commit Squashing Workflow

**Standard workflow for squashing minor commits:**

```powershell
# Example: Squashing a docs-only commit into v0.0.5 commit

# 1. Check current commits
git log --oneline -5

# 2. Soft reset to before minor commit
git reset --soft HEAD~1

# 3. Amend previous commit (keeps all changes)
git commit --amend --no-edit

# 4. Update tag
git tag -d v0.0.5
git tag -a v0.0.5 -m "Version 0.0.5 - Major privacy and cleanup expansion"

# 5. Force push everything
git push -f origin master
git push -f origin v0.0.5
```

---

## 🎯 Quick Reference

### Complete Release Workflow

```powershell
# 1. Make code changes and commit
git add -A
git commit -m "v0.0.X - Description"

# 2. Update README.md if needed
git add README.md
git commit --amend --no-edit  # Squash into previous commit

# 3. Create/update tag
git tag -a v0.0.X -m "Version 0.0.X - Description"
git push -f origin master
git push origin v0.0.X

# 4. Build binaries
cd WindowsCleanup
dotnet publish -c Release -r win-arm64 --self-contained
dotnet publish -c Release -r win-x64 --self-contained
cd ..
Copy-Item "WindowsCleanup\bin\Release\net10.0\win-arm64\publish\WindowsCleanup.exe" "WindowsCleanup-arm64.exe" -Force
Copy-Item "WindowsCleanup\bin\Release\net10.0\win-x64\publish\WindowsCleanup.exe" "WindowsCleanup-x64.exe" -Force

# 5. Test binaries
.\WindowsCleanup-arm64.exe --dry-run

# 6. Create GitHub release with gh CLI
gh release create v0.0.X `
    --title "v0.0.X - Title" `
    --notes "FULL_RELEASE_NOTES" `
    --repo sirredbeard/cleanup `
    "WindowsCleanup-arm64.exe#WindowsCleanup-arm64.exe (ARM64 - 2.66 MB)" `
    "WindowsCleanup-x64.exe#WindowsCleanup-x64.exe (x64 - 2.62 MB)"
```

### File Structure

```
D:\cleanup\
├── .git\                          # Git repository
├── .gitignore                     # Excludes binaries and dry-run logs
├── .vscode\                       # VS Code settings
├── AGENTS.md                      # This file
├── LICENSE                        # MIT License
├── README.md                      # User-facing documentation
├── WindowsCleanup\
│   ├── Program.cs                # Main application (~12,000 lines)
│   ├── WindowsCleanup.csproj     # Project file
│   └── bin\                      # Build output (not in git)
├── WindowsCleanup-arm64.exe      # Built for releases (not in git)
└── WindowsCleanup-x64.exe        # Built for releases (not in git)
```

---

## 📝 Important Notes

### DO's ✅
- Always build both ARM64 and x64 binaries
- Always squash minor commits into version commits
- Always test with `--dry-run` before releasing
- Always update README.md with new features
- Always use emoji consistently in code and docs
- Always preserve authentication data (cookies, tokens, keys)
- Always include comprehensive release notes in GitHub releases
- Always use `gh` CLI for creating releases

### DON'Ts ❌
- Don't track binaries in git
- Don't create separate release notes files in repo
- Don't commit without testing
- Don't delete authentication folders (Local Storage, IndexedDB, etc.)
- Don't use ASCII boxes/arrows (use emoji instead)
- Don't release without both architectures
- Don't push tags without squashing commits first
- Don't include build artifacts in README project structure

---

## 🔗 Resources

- **GitHub CLI**: `winget install --id GitHub.cli`
- **.NET 10 SDK**: https://dotnet.microsoft.com/download
- **Repository**: https://github.com/sirredbeard/cleanup
- **Releases**: https://github.com/sirredbeard/cleanup/releases

---

**Last Updated:** 2026-01-31 (v0.0.5)
