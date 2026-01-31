using System.Diagnostics;
using System.Security.Principal;
using System.ServiceProcess;
using Microsoft.Win32;

class WindowsCleanupTool
{
    private static bool _dryRun = false;
    private static StreamWriter? _dryRunLog = null;

    static void Main(string[] args)
    {
        // Set console encoding to UTF-8 for proper emoji display
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.InputEncoding = System.Text.Encoding.UTF8;
        
        // Check for dry-run mode
        bool foundDryRun = false;
        foreach (var arg in args)
        {
            if (arg.Equals("--dry-run", StringComparison.OrdinalIgnoreCase) ||
                arg.Equals("/dry-run", StringComparison.OrdinalIgnoreCase) ||
                arg.Equals("-dry-run", StringComparison.OrdinalIgnoreCase))
            {
                foundDryRun = true;
                break;
            }
        }

        if (foundDryRun)
        {
            _dryRun = true;
            var timestamp = DateTime.Now.ToString("yyyyMMdd-HHmmss");
            var logPath = Path.Combine(Environment.CurrentDirectory, $"dry-run-{timestamp}.txt");
            _dryRunLog = new StreamWriter(logPath, false);
            
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine();
            Console.WriteLine("  🧹✨ Windows Cleanup Tool - DRY RUN MODE 🔍");
            Console.WriteLine();
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"  🔵 Dry-run mode: No files will be deleted. Logging to: {logPath}");
            Console.ResetColor();
            Console.WriteLine();
            
            LogDryRun($"=== Windows Cleanup Tool - Dry Run ===");
            LogDryRun($"Date: {DateTime.Now}");
            LogDryRun($"User: {Environment.UserName}");
            LogDryRun($"Computer: {Environment.MachineName}");
            LogDryRun($"OS: {Environment.OSVersion}");
            LogDryRun("");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine();
            Console.WriteLine("  🧹✨ Windows Cleanup Tool 💻");
            Console.WriteLine();
            Console.ResetColor();
            Console.WriteLine();
        }

        if (!IsRunAsAdministrator())
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("⚠️ Warning: Not running as administrator. Some cleanup tasks may fail.");
            Console.ResetColor();
            Console.WriteLine("\n💭 Would you like to restart as administrator? (Y/N)");
            
            var key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.Y)
            {
                try
                {
                    // Restart with admin privileges
                    var startInfo = new ProcessStartInfo
                    {
                        UseShellExecute = true,
                        WorkingDirectory = Environment.CurrentDirectory,
                        FileName = Process.GetCurrentProcess().MainModule?.FileName ?? "WindowsCleanup.exe",
                        Verb = "runas" // Request elevation
                    };
                    
                    // Pass through the arguments (including --dry-run)
                    if (args.Length > 0)
                    {
                        startInfo.Arguments = string.Join(" ", args);
                    }
                    
                    Console.WriteLine("\n🔄 Requesting administrator privileges...");
                    Process.Start(startInfo);
                    
                    // Exit this non-admin instance
                    return;
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\n✗ Failed to elevate: {ex.Message}");
                    Console.WriteLine("Continuing without administrator privileges...");
                    Console.ResetColor();
                    Console.WriteLine();
                }
            }
            else
            {
                Console.WriteLine("\nContinuing without administrator privileges...");
                Console.WriteLine();
            }
        }

        try
        {
            ClearTempFiles();
            ClearPageAndHibernateFiles();
            ClearRecentFiles();
            ClearStartMenuRecent();
            ClearThumbnailsAndCaches();
            ClearBrowserData();
            ClearPowerShellHistory();
            ClearCommandPromptHistory();
            ClearWSLHistory();
            ClearCopilotHistory();
            ClearVSCodeCopilotCache();
            ClearCopilotPlusCache();
            ClearVisualStudioCache();
            ClearCloudflareWarp();
            ClearWindowsFeatures();
            ClearNetworkHistory();
            ClearApplicationCaches();
            ClearOutlookWebView();
            ClearEmbeddedWebViews();
            ClearAllDetectedAppData();
            ClearSpecificInstalledApps();
            ClearPowerToys();
            ClearPrismCache();
            ClearDevelopmentToolCaches();
            ClearProgrammingLanguageCaches();
            ClearHyperVLogs();
            ClearVirtualBoxData();
            ClearVMwareData();
            ClearDeliveryOptimizationCache();
            ClearEventLogs();
            ClearGraphicsDriverCaches();
            ClearWindowsSubsystems();
            ClearDetectedInstalledApps();
            ClearAdditionalWindowsTraces();
            ClearDeepSystemCaches();
            ClearWindowsUpgradeFolders();
            ClearProgramFilesAppData();
            ClearSystemLogs();
            // ClearWindowsUpdateCache(); // Removed - leave for Windows Disk Cleanup tool (can hang)
            ClearUserDocumentsFolders();
            ClearWindowsFeedback();
            ClearOllama();
            ClearYtDlp();
            ClearFfmpeg();
            ClearRegistryTraces();
            ClearAdditionalRegistryTraces();
            ClearExpandedRegistryTraces();
            ClearDeepSystemTraces();
            ClearSystemRestoreAndShadowCopies();
            FlushDnsCache();
            RunDiskCleanup();

            Console.ForegroundColor = ConsoleColor.Green;
            if (_dryRun)
            {
                Console.WriteLine("\n✓ Dry-run completed successfully! Check the log file for details.");
            }
            else
            {
                Console.WriteLine("\n✓ Cleanup completed successfully!");
            }
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n✗ Error during cleanup: {ex.Message}");
            Console.ResetColor();
            LogDryRun($"ERROR: {ex.Message}");
        }
        finally
        {
            if (_dryRunLog != null)
            {
                LogDryRun("");
                LogDryRun("=== End of Dry Run ===");
                _dryRunLog.Close();
                _dryRunLog.Dispose();
            }
        }

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }

    static void LogDryRun(string message)
    {
        if (_dryRun && _dryRunLog != null)
        {
            _dryRunLog.WriteLine(message);
            _dryRunLog.Flush();
        }
    }

    static bool IsRunAsAdministrator()
    {
        var identity = WindowsIdentity.GetCurrent();
        var principal = new WindowsPrincipal(identity);
        return principal.IsInRole(WindowsBuiltInRole.Administrator);
    }

    static void ClearTempFiles()
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("🔹 Clearing temporary files...");
        Console.ResetColor();

        var tempPaths = new[]
        {
            Path.GetTempPath(),
            Environment.GetEnvironmentVariable("TEMP"),
            @"C:\Windows\Temp"
        };

        int deletedCount = 0;
        long freedSpace = 0;

        LogDryRun("\n=== TEMP FILES ===");

        foreach (var tempPath in tempPaths.Where(p => p != null && Directory.Exists(p)).Distinct())
        {
            try
            {
                var files = Directory.GetFiles(tempPath, "*.*", SearchOption.AllDirectories);
                foreach (var file in files)
                {
                    try
                    {
                        var fileInfo = new FileInfo(file);
                        long size = fileInfo.Length;
                        
                        if (_dryRun)
                        {
                            LogDryRun($"Would delete file: {file} ({FormatBytes(size)})");
                        }
                        else
                        {
                            fileInfo.Delete();
                        }
                        deletedCount++;
                        freedSpace += size;
                    }
                    catch { /* Skip files in use */ }
                }

                var dirs = Directory.GetDirectories(tempPath);
                foreach (var dir in dirs)
                {
                    try
                    {
                        if (_dryRun)
                        {
                            LogDryRun($"Would delete directory: {dir}");
                        }
                        else
                        {
                            Directory.Delete(dir, true);
                        }
                    }
                    catch { /* Skip folders in use */ }
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine($"  ⚠️ Could not fully clean: {tempPath} - {ex.Message}");
                Console.ResetColor();
            }
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"  ✅ {(_dryRun ? "Would delete" : "Deleted")} {deletedCount} temp files ({FormatBytes(freedSpace)} freed)");
        Console.ResetColor();
    }

    static void ClearPageAndHibernateFiles()
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("\n🔹 Clearing page and hibernate files...");
        Console.ResetColor();

        LogDryRun("\n=== PAGE AND HIBERNATE FILES ===");

        try
        {
            // Disable hibernation (removes hiberfil.sys)
            if (_dryRun)
            {
                LogDryRun("Would run: powercfg /hibernate off");
            }
            else
            {
                RunCommand("powercfg", "/hibernate off");
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"  ✅ Hibernation file {(_dryRun ? "would be" : "")} deleted");
            Console.ResetColor();

            // Re-enable hibernation (recreates clean hiberfil.sys)
            if (_dryRun)
            {
                LogDryRun("Would run: powercfg /hibernate on");
            }
            else
            {
                RunCommand("powercfg", "/hibernate on");
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"  ✅ Hibernation {(_dryRun ? "would be" : "")} re-enabled (clean file recreated)");
            Console.ResetColor();

            // Enable pagefile clearing on shutdown
            try
            {
                if (_dryRun)
                {
                    LogDryRun("Would set registry: HKLM\\SYSTEM\\CurrentControlSet\\Control\\Session Manager\\Memory Management\\ClearPageFileAtShutdown = 1");
                }
                else
                {
                    using var key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management", true);
                    if (key != null)
                    {
                        key.SetValue("ClearPageFileAtShutdown", 1, RegistryValueKind.DWord);
                    }
                }
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"  ✅ Pagefile {(_dryRun ? "would be configured to clear" : "will be cleared")} on next shutdown/restart");
                Console.ResetColor();
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("  ⚠️ Could not enable pagefile clearing (requires admin)");
                Console.ResetColor();
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  ❌ Failed: {ex.Message}");
            Console.ResetColor();
        }
    }

    static void ClearRecentFiles()
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("\n🔹 Clearing recent files and jump lists...");
        Console.ResetColor();

        int clearedCount = 0;

        try
        {
            // Clear Recent folder
            var recentPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Microsoft", "Windows", "Recent");
            if (Directory.Exists(recentPath))
            {
                var files = Directory.GetFiles(recentPath, "*.*", SearchOption.AllDirectories);
                foreach (var file in files)
                {
                    try
                    {
                        File.Delete(file);
                        clearedCount++;
                    }
                    catch { }
                }
            }

            // Clear Jump Lists
            var jumpListPaths = new[]
            {
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "Microsoft", "Windows", "Recent", "AutomaticDestinations"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "Microsoft", "Windows", "Recent", "CustomDestinations")
            };

            foreach (var path in jumpListPaths)
            {
                if (Directory.Exists(path))
                {
                    var files = Directory.GetFiles(path);
                    foreach (var file in files)
                    {
                        try
                        {
                            File.Delete(file);
                            clearedCount++;
                        }
                        catch { }
                    }
                }
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"  ✅ {(_dryRun ? "Would clear" : "Cleared")} {clearedCount} recent files and jump lists");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  ❌ Failed: {ex.Message}");
            Console.ResetColor();
        }
    }

    static void ClearStartMenuRecent()
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("\n🔹 Clearing Start Menu recent and recommended items...");
        Console.ResetColor();

        LogDryRun("\n=== START MENU RECENT AND RECOMMENDED ===");

        int clearedCount = 0;

        try
        {
            // Clear Start Menu recent items registry
            var startMenuKeys = new[]
            {
                @"Software\Microsoft\Windows\CurrentVersion\Explorer\RecentDocs",
                @"Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts"
            };

            foreach (var keyPath in startMenuKeys)
            {
                try
                {
                    using var key = Registry.CurrentUser.OpenSubKey(keyPath, true);
                    if (key != null)
                    {
                        if (keyPath.EndsWith("RecentDocs"))
                        {
                            // Clear main RecentDocs key
                            var valueNames = key.GetValueNames();
                            foreach (var valueName in valueNames)
                            {
                                if (valueName != "MRUListEx")
                                {
                                    try
                                    {
                                        if (_dryRun)
                                        {
                                            LogDryRun($"Would delete registry value: {keyPath}\\{valueName}");
                                        }
                                        else
                                        {
                                            key.DeleteValue(valueName);
                                        }
                                        clearedCount++;
                                    }
                                    catch { }
                                }
                            }

                            // Clear subkeys for each file extension
                            var subKeyNames = key.GetSubKeyNames();
                            foreach (var subKeyName in subKeyNames)
                            {
                                try
                                {
                                    if (_dryRun)
                                    {
                                        LogDryRun($"Would delete registry key: {keyPath}\\{subKeyName}");
                                    }
                                    else
                                    {
                                        key.DeleteSubKeyTree(subKeyName, false);
                                    }
                                    clearedCount++;
                                }
                                catch { }
                            }
                        }
                    }
                }
                catch { }
            }

            // Clear Windows 11 "Recommended" section registry
            var recommendedKeys = new[]
            {
                @"Software\Microsoft\Windows\CurrentVersion\Start\RecommendedFiles",
                @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced\Start_TrackDocs"
            };

            foreach (var keyPath in recommendedKeys)
            {
                try
                {
                    using var key = Registry.CurrentUser.OpenSubKey(keyPath, true);
                    if (key != null)
                    {
                        var valueNames = key.GetValueNames();
                        foreach (var valueName in valueNames)
                        {
                            try
                            {
                                if (_dryRun)
                                {
                                    LogDryRun($"Would delete registry value: {keyPath}\\{valueName}");
                                }
                                else
                                {
                                    key.DeleteValue(valueName);
                                }
                                clearedCount++;
                            }
                            catch { }
                        }
                    }
                }
                catch { }
            }

            // Disable "Show recommendations" in Start (Windows 11)
            try
            {
                var startKey = @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced";
                using var key = Registry.CurrentUser.OpenSubKey(startKey, true);
                if (key != null)
                {
                    if (_dryRun)
                    {
                        LogDryRun($"Would set registry: {startKey}\\Start_IrisRecommendations = 0");
                    }
                    else
                    {
                        key.SetValue("Start_IrisRecommendations", 0, RegistryValueKind.DWord);
                    }
                    clearedCount++;
                }
            }
            catch { }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"  ✅ {(_dryRun ? "Would clear" : "Cleared")} {clearedCount} Start Menu recent and recommended items");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  ❌ Failed: {ex.Message}");
            Console.ResetColor();
        }
    }

    static void ClearThumbnailsAndCaches()
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("\n🔹 Clearing thumbnail and icon caches...");
        Console.ResetColor();

        int clearedCount = 0;
        long freedSpace = 0;

        try
        {
            // Clear thumbnail cache
            var thumbCachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Microsoft", "Windows", "Explorer");
            
            if (Directory.Exists(thumbCachePath))
            {
                var thumbFiles = Directory.GetFiles(thumbCachePath, "thumbcache_*.db");
                foreach (var file in thumbFiles)
                {
                    try
                    {
                        var fileInfo = new FileInfo(file);
                        freedSpace += fileInfo.Length;
                        fileInfo.Delete();
                        clearedCount++;
                    }
                    catch { }
                }
            }

            // Clear icon cache
            var iconCachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "IconCache.db");
            if (File.Exists(iconCachePath))
            {
                try
                {
                    var fileInfo = new FileInfo(iconCachePath);
                    freedSpace += fileInfo.Length;
                    fileInfo.Delete();
                    clearedCount++;
                }
                catch { }
            }

            // Clear font cache
            var fontCachePath = @"C:\Windows\System32\FNTCACHE.DAT";
            if (File.Exists(fontCachePath))
            {
                try
                {
                    var fileInfo = new FileInfo(fontCachePath);
                    freedSpace += fileInfo.Length;
                    fileInfo.Delete();
                    clearedCount++;
                }
                catch { }
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"  ✅ {(_dryRun ? "Would clear" : "Cleared")} {clearedCount} cache files ({FormatBytes(freedSpace)} {(_dryRun ? "would be freed" : "freed")})");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  ❌ Failed: {ex.Message}");
            Console.ResetColor();
        }
    }

    static void ClearBrowserData()
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("\n🔹 Clearing browser data...");
        Console.ResetColor();

        var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        // Track detected browsers
        var detectedBrowsers = new List<string>();

        // Browser profiles (Chromium-based and Firefox)
        var browserPaths = new Dictionary<string, string[]>
        {
            // Edge browsers - will scan all profiles including web apps
            { "Edge", new[] { Path.Combine(localAppData, "Microsoft", "Edge", "User Data") } },
            { "Edge Dev", new[] { Path.Combine(localAppData, "Microsoft", "Edge Dev", "User Data") } },
            { "Edge Beta", new[] { Path.Combine(localAppData, "Microsoft", "Edge Beta", "User Data") } },
            { "Edge Canary", new[] { Path.Combine(localAppData, "Microsoft", "Edge SxS", "User Data") } },
            
            // Chrome - all channels
            { "Chrome", new[] { Path.Combine(localAppData, "Google", "Chrome", "User Data") } },
            { "Chrome Beta", new[] { Path.Combine(localAppData, "Google", "Chrome Beta", "User Data") } },
            { "Chrome Dev", new[] { Path.Combine(localAppData, "Google", "Chrome Dev", "User Data") } },
            { "Chrome Canary", new[] { Path.Combine(localAppData, "Google", "Chrome SxS", "User Data") } },
            
            // Brave - all channels
            { "Brave", new[] { Path.Combine(localAppData, "BraveSoftware", "Brave-Browser", "User Data") } },
            { "Brave Beta", new[] { Path.Combine(localAppData, "BraveSoftware", "Brave-Browser-Beta", "User Data") } },
            { "Brave Dev", new[] { Path.Combine(localAppData, "BraveSoftware", "Brave-Browser-Dev", "User Data") } },
            { "Brave Nightly", new[] { Path.Combine(localAppData, "BraveSoftware", "Brave-Browser-Nightly", "User Data") } },
            
            // Opera - all channels
            { "Opera", new[] { Path.Combine(appData, "Opera Software", "Opera Stable") } },
            { "Opera Beta", new[] { Path.Combine(appData, "Opera Software", "Opera Beta") } },
            { "Opera Developer", new[] { Path.Combine(appData, "Opera Software", "Opera Developer") } },
            { "Opera GX", new[] { Path.Combine(appData, "Opera Software", "Opera GX Stable") } },
            
            // Vivaldi - all channels
            { "Vivaldi", new[] { Path.Combine(localAppData, "Vivaldi", "User Data") } },
            { "Vivaldi Snapshot", new[] { Path.Combine(localAppData, "Vivaldi Snapshot", "User Data") } },
            
            // Firefox - all channels
            { "Firefox", new[] { Path.Combine(appData, "Mozilla", "Firefox", "Profiles") } },
            { "Firefox Beta", new[] { Path.Combine(appData, "Mozilla", "Firefox Beta", "Profiles") } },
            { "Firefox Developer Edition", new[] { Path.Combine(appData, "Mozilla", "Firefox Developer Edition", "Profiles") } },
            { "Firefox Nightly", new[] { Path.Combine(appData, "Mozilla", "Firefox Nightly", "Profiles") } }
        };

        var chromiumCacheSubfolders = new[] { "Cache", "Code Cache", "GPUCache", "Service Worker", "Storage" };
        var chromiumDataFiles = new[] { "History", "History-journal", "Cookies", "Cookies-journal", 
            "Web Data", "Web Data-journal", "QuotaManager", "QuotaManager-journal" };

        bool foundAny = false;

        // Clear Chromium-based browsers (all profiles including web apps)
        foreach (var browser in browserPaths)
        {
            foreach (var userDataPath in browser.Value)
            {
                if (!Directory.Exists(userDataPath))
                    continue;

                foundAny = true;
                detectedBrowsers.Add(browser.Key);
                Console.WriteLine($"  Cleaning {browser.Key}...");

                try
                {
                    // For Chromium browsers, find all profiles (Default, Profile 1, Profile 2, etc.)
                    var profiles = new List<string>();
                    
                    // Check if this is a User Data directory structure
                    var defaultProfile = Path.Combine(userDataPath, "Default");
                    if (Directory.Exists(defaultProfile))
                    {
                        // Chromium-style User Data directory
                        profiles.Add(defaultProfile);
                        
                        // Add numbered profiles (Profile 1, Profile 2, etc.)
                        var numberedProfiles = Directory.GetDirectories(userDataPath, "Profile *");
                        profiles.AddRange(numberedProfiles);
                    }
                    else
                    {
                        // Opera or other non-standard structure
                        profiles.Add(userDataPath);
                    }

                    foreach (var profilePath in profiles)
                    {
                        var profileName = Path.GetFileName(profilePath);
                        if (profiles.Count > 1)
                        {
                            Console.WriteLine($"    Cleaning profile: {profileName}");
                        }

                        // Clear cache folders
                        foreach (var folder in chromiumCacheSubfolders)
                        {
                            var folderPath = Path.Combine(profilePath, folder);
                            if (Directory.Exists(folderPath))
                            {
                                try
                                {
                                    if (_dryRun)
                                    {
                                        LogDryRun($"Would delete directory: {folderPath}");
                                    }
                                    else
                                    {
                                        Directory.Delete(folderPath, true);
                                        Directory.CreateDirectory(folderPath);
                                    }
                                }
                                catch { }
                            }
                        }

                        // Delete data files (including download history)
                        foreach (var file in chromiumDataFiles)
                        {
                            var filePath = Path.Combine(profilePath, file);
                            if (File.Exists(filePath))
                            {
                                try
                                {
                                    if (_dryRun)
                                    {
                                        LogDryRun($"Would delete file: {filePath}");
                                    }
                                    else
                                    {
                                        File.Delete(filePath);
                                    }
                                }
                                catch { }
                            }
                        }

                        // Explicitly clear download history (History file contains download records)
                        var downloadFiles = new[] { "History Provider Cache", "Download Service" };
                        foreach (var file in downloadFiles)
                        {
                            var filePath = Path.Combine(profilePath, file);
                            if (File.Exists(filePath))
                            {
                                try
                                {
                                    if (_dryRun)
                                    {
                                        LogDryRun($"Would delete file: {filePath}");
                                    }
                                    else
                                    {
                                        File.Delete(filePath);
                                    }
                                }
                                catch { }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine($"  ⚠️ Warning: {ex.Message}");
                    Console.ResetColor();
                }
            }
        }

        // Clear Firefox
        var firefoxPath = Path.Combine(appData, "Mozilla", "Firefox", "Profiles");
        if (Directory.Exists(firefoxPath))
        {
            foundAny = true;
            detectedBrowsers.Add("Firefox");
            Console.WriteLine("  Cleaning Firefox...");
            
            var profiles = Directory.GetDirectories(firefoxPath);
            foreach (var profile in profiles)
            {
                try
                {
                    // Clear Firefox cache
                    var cacheFolders = new[] { "cache2", "cache", "thumbnails", "OfflineCache", "storage" };
                    foreach (var folder in cacheFolders)
                    {
                        var folderPath = Path.Combine(profile, folder);
                        if (Directory.Exists(folderPath))
                        {
                            try
                            {
                                if (_dryRun)
                                {
                                    LogDryRun($"Would delete directory: {folderPath}");
                                }
                                else
                                {
                                    Directory.Delete(folderPath, true);
                                }
                            }
                            catch { }
                        }
                    }

                    // Clear Firefox data files (includes download history in places.sqlite)
                    var firefoxFiles = new[] { "places.sqlite", "places.sqlite-wal", "places.sqlite-shm",
                        "cookies.sqlite", "cookies.sqlite-wal", "cookies.sqlite-shm",
                        "formhistory.sqlite", "webappsstore.sqlite", "downloads.sqlite" };
                    
                    foreach (var file in firefoxFiles)
                    {
                        var filePath = Path.Combine(profile, file);
                        if (File.Exists(filePath))
                        {
                            try
                            {
                                if (_dryRun)
                                {
                                    LogDryRun($"Would delete file: {filePath}");
                                }
                                else
                                {
                                    File.Delete(filePath);
                                }
                            }
                            catch { }
                        }
                    }
                }
                catch { }
            }
        }

        if (foundAny)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"\n  📋 Detected browsers: {string.Join(", ", detectedBrowsers.Distinct())}");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"  ✅ {(_dryRun ? "Would clear" : "Cleared")} browser data (including download history)");
            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("  ⚠️ No supported browsers detected");
            Console.ResetColor();
        }

        // Clear legacy Internet Explorer cache
        ClearInternetExplorerCache();
    }

    static void ClearInternetExplorerCache()
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("\n🔹 Clearing legacy Internet Explorer cache...");
        Console.ResetColor();

        long freedSpace = 0;
        int clearedCount = 0;

        try
        {
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var localLowAppData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData", "LocalLow");
            
            // IE cache locations
            var ieCachePaths = new[]
            {
                Path.Combine(localAppData, "Microsoft", "Windows", "INetCache"),
                Path.Combine(localAppData, "Microsoft", "Windows", "INetCookies"),
                Path.Combine(localAppData, "Microsoft", "Windows", "IECompatCache"),
                Path.Combine(localAppData, "Microsoft", "Windows", "IECompatUaCache"),
                Path.Combine(localAppData, "Microsoft", "Windows", "IEDownloadHistory"),
                Path.Combine(localAppData, "Microsoft", "Windows", "Temporary Internet Files"),
                Path.Combine(localLowAppData, "Microsoft", "Internet Explorer", "DOMStore"),
                Path.Combine(localLowAppData, "Microsoft", "Internet Explorer", "Services"),
                Path.Combine(localAppData, "Microsoft", "Internet Explorer", "Recovery"),
                Path.Combine(localAppData, "Microsoft", "Internet Explorer", "imagestore")
            };

            foreach (var cachePath in ieCachePaths)
            {
                if (Directory.Exists(cachePath))
                {
                    try
                    {
                        var size = GetDirectorySize(cachePath);
                        if (_dryRun)
                        {
                            LogDryRun($"Would delete directory: {cachePath} ({FormatBytes(size)})");
                        }
                        else
                        {
                            Directory.Delete(cachePath, true);
                        }
                        freedSpace += size;
                        clearedCount++;
                    }
                    catch { }
                }
            }

            // Clear IE Flash cache
            var flashCachePath = Path.Combine(localAppData, "Microsoft", "Windows", "FlashPlayerCache");
            if (Directory.Exists(flashCachePath))
            {
                try
                {
                    var size = GetDirectorySize(flashCachePath);
                    if (_dryRun)
                    {
                        LogDryRun($"Would delete directory: {flashCachePath} ({FormatBytes(size)})");
                    }
                    else
                    {
                        Directory.Delete(flashCachePath, true);
                    }
                    freedSpace += size;
                    clearedCount++;
                }
                catch { }
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"  ✅ {(_dryRun ? "Would clear" : "Cleared")} {clearedCount} IE cache items ({FormatBytes(freedSpace)} freed)");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  ❌ Failed: {ex.Message}");
            Console.ResetColor();
        }
    }

    static void ClearPowerShellHistory()
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("\n🔹 Clearing PowerShell history...");
        Console.ResetColor();

        int clearedCount = 0;

        try
        {
            // Clear current user's PowerShell history
            var historyPaths = new[]
            {
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "Microsoft", "Windows", "PowerShell", "PSReadLine", "ConsoleHost_history.txt"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "Microsoft", "Windows", "PowerShell", "PSReadLine", "Visual Studio Code Host_history.txt")
            };

            foreach (var historyPath in historyPaths)
            {
                if (File.Exists(historyPath))
                {
                    try
                    {
                        File.Delete(historyPath);
                        clearedCount++;
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"  ✅ {(_dryRun ? "Would clear" : "Cleared")}: {Path.GetFileName(historyPath)}");
                        Console.ResetColor();
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.WriteLine($"  ⚠️ Could not delete {Path.GetFileName(historyPath)}: {ex.Message}");
                        Console.ResetColor();
                    }
                }
            }

            // Try to clear for all users (requires admin)
            if (IsRunAsAdministrator())
            {
                var usersPath = @"C:\Users";
                if (Directory.Exists(usersPath))
                {
                    foreach (var userDir in Directory.GetDirectories(usersPath))
                    {
                        var userName = Path.GetFileName(userDir);
                        if (userName == "Public" || userName == "Default" || userName == "Default User")
                            continue;

                        var userHistoryPath = Path.Combine(userDir, "AppData", "Roaming", "Microsoft",
                            "Windows", "PowerShell", "PSReadLine", "ConsoleHost_history.txt");

                        if (File.Exists(userHistoryPath))
                        {
                            try
                            {
                                File.Delete(userHistoryPath);
                                clearedCount++;
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine($"  ✅ {(_dryRun ? "Would clear" : "Cleared")} PowerShell history for user: {userName}");
                                Console.ResetColor();
                            }
                            catch { /* Skip if access denied */ }
                        }
                    }
                }
            }

            if (clearedCount > 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"  ✅ {(_dryRun ? "Would clear" : "Cleared")} {clearedCount} PowerShell history file(s)");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("  ⚠️ No PowerShell history files found");
                Console.ResetColor();
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  ❌ Failed: {ex.Message}");
            Console.ResetColor();
        }
    }

    static void ClearCommandPromptHistory()
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("\n🔹 Clearing Command Prompt history...");
        Console.ResetColor();

        try
        {
            // Clear Run dialog history (MRU - Most Recently Used)
            var registryPaths = new[]
            {
                @"Software\Microsoft\Windows\CurrentVersion\Explorer\RunMRU",
                @"Software\Microsoft\Windows\CurrentVersion\Explorer\TypedPaths"
            };

            int clearedCount = 0;

            foreach (var regPath in registryPaths)
            {
                try
                {
                    using var key = Registry.CurrentUser.OpenSubKey(regPath, true);
                    if (key != null)
                    {
                        var valueNames = key.GetValueNames();
                        foreach (var valueName in valueNames)
                        {
                            if (valueName != "MRUList" && valueName != "") // Keep the MRUList structure
                            {
                                try
                                {
                                    if (_dryRun)
                                    {
                                        LogDryRun($"Would delete registry value: {regPath}\\{valueName}");
                                    }
                                    else
                                    {
                                        key.DeleteValue(valueName);
                                    }
                                    clearedCount++;
                                }
                                catch { }
                            }
                        }
                        
                        // Reset MRUList to empty
                        if (key.GetValue("MRUList") != null && !_dryRun)
                        {
                            key.SetValue("MRUList", "");
                        }
                    }
                }
                catch { /* Skip if key doesn't exist */ }
            }

            // Clear SSH history (not keys)
            var sshPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".ssh");
            if (Directory.Exists(sshPath))
            {
                try
                {
                    var sshHistoryFiles = new[] { ".bash_history", ".zsh_history", ".history" };
                    foreach (var histFile in sshHistoryFiles)
                    {
                        var fullPath = Path.Combine(sshPath, histFile);
                        if (File.Exists(fullPath))
                        {
                            try
                            {
                                if (_dryRun)
                                {
                                    LogDryRun($"Would delete file: {fullPath}");
                                }
                                else
                                {
                                    File.Delete(fullPath);
                                }
                                clearedCount++;
                            }
                            catch { }
                        }
                    }
                }
                catch { }
            }

            // Clear Git Bash history (Windows Git installation)
            var gitBashPaths = new[]
            {
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".bash_history"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".bash_logout"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".bash_profile"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".minttyrc"),
            };

            foreach (var histFile in gitBashPaths)
            {
                // Only clear history files, not config files
                if (histFile.Contains("_history") || histFile.Contains("_logout"))
                {
                    if (File.Exists(histFile))
                    {
                        try
                        {
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete file: {histFile}");
                            }
                            else
                            {
                                File.Delete(histFile);
                            }
                            clearedCount++;
                        }
                        catch { }
                    }
                }
            }

            // Clear Scoop cache and logs (not packages)
            var scoopPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "scoop");
            if (Directory.Exists(scoopPath))
            {
                var scoopCachePath = Path.Combine(scoopPath, "cache");
                if (Directory.Exists(scoopCachePath))
                {
                    try
                    {
                        var files = Directory.GetFiles(scoopCachePath, "*.*", SearchOption.AllDirectories);
                        foreach (var file in files)
                        {
                            try
                            {
                                if (_dryRun)
                                {
                                    LogDryRun($"Would delete file: {file}");
                                }
                                else
                                {
                                    File.Delete(file);
                                }
                                clearedCount++;
                            }
                            catch { }
                        }
                    }
                    catch { }
                }

                // Clear scoop logs
                var scoopLogsPath = Path.Combine(scoopPath, "apps", "*", "current", "logs");
                try
                {
                    var logDirs = Directory.GetDirectories(Path.Combine(scoopPath, "apps"), "*", SearchOption.AllDirectories)
                        .Where(d => d.EndsWith("logs", StringComparison.OrdinalIgnoreCase));

                    foreach (var logDir in logDirs)
                    {
                        try
                        {
                            var files = Directory.GetFiles(logDir, "*.log", SearchOption.AllDirectories);
                            foreach (var file in files)
                            {
                                try
                                {
                                    if (_dryRun)
                                    {
                                        LogDryRun($"Would delete file: {file}");
                                    }
                                    else
                                    {
                                        File.Delete(file);
                                    }
                                    clearedCount++;
                                }
                                catch { }
                            }
                        }
                        catch { }
                    }
                }
                catch { }
            }

            // Clear WinGet logs and cache (not packages)
            var wingetPaths = new[]
            {
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Microsoft", "WinGet", "State"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Packages", "Microsoft.DesktopAppInstaller_8wekyb3d8bbwe", "LocalState", "DiagOutputDir"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Packages", "Microsoft.DesktopAppInstaller_8wekyb3d8bbwe", "TempState"),
            };

            foreach (var wingetPath in wingetPaths)
            {
                if (Directory.Exists(wingetPath))
                {
                    try
                    {
                        var files = Directory.GetFiles(wingetPath, "*.*", SearchOption.AllDirectories)
                            .Where(f => f.EndsWith(".log", StringComparison.OrdinalIgnoreCase) || 
                                       f.EndsWith(".etl", StringComparison.OrdinalIgnoreCase) ||
                                       f.Contains("diagnostic", StringComparison.OrdinalIgnoreCase));

                        foreach (var file in files)
                        {
                            try
                            {
                                if (_dryRun)
                                {
                                    LogDryRun($"Would delete file: {file}");
                                }
                                else
                                {
                                    File.Delete(file);
                                }
                                clearedCount++;
                            }
                            catch { }
                        }
                    }
                    catch { }
                }
            }

            // Clear command prompt doskey history (session-based, so we just note it)
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"  ✅ {(_dryRun ? "Would clear" : "Cleared")} Run dialog and command history ({clearedCount} entries)");
            Console.WriteLine("  ℹ Note: Active Command Prompt session history will clear on window close");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  ❌ Failed: {ex.Message}");
            Console.ResetColor();
        }
    }

    static void ClearWSLHistory()
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("\n🔹 Clearing WSL logs, caches, and shell history...");
        Console.ResetColor();

        LogDryRun("\n=== WSL CLEANUP ===");

        int clearedCount = 0;
        long freedSpace = 0;

        try
        {
            // Check if WSL is installed
            var wslCheckInfo = new ProcessStartInfo
            {
                FileName = "wsl",
                Arguments = "--list --quiet",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            List<string> distros = new List<string>();

            try
            {
                using var wslCheck = Process.Start(wslCheckInfo);
                if (wslCheck != null)
                {
                    string output = wslCheck.StandardOutput.ReadToEnd();
                    wslCheck.WaitForExit();

                    if (wslCheck.ExitCode == 0 && !string.IsNullOrWhiteSpace(output))
                    {
                        // Parse distro names
                        var lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (var line in lines)
                        {
                            var distroName = line.Trim().Replace("\0", ""); // Remove null characters
                            if (!string.IsNullOrWhiteSpace(distroName))
                            {
                                distros.Add(distroName);
                            }
                        }
                    }
                }
            }
            catch
            {
                // WSL not available
            }

            if (distros.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("  ⚠️ WSL not detected or no distributions installed");
                Console.ResetColor();
                return;
            }

            Console.WriteLine($"  Found {distros.Count} WSL distribution(s)");

            foreach (var distro in distros)
            {
                Console.WriteLine($"  Cleaning {distro}...");

                // Get list of all users in the distro (run as root)
                var getUsersCmd = new ProcessStartInfo
                {
                    FileName = "wsl",
                    Arguments = $"-d {distro} --user root bash -c \"cut -d: -f1,6 /etc/passwd | grep -E '/home/'\"",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                List<(string username, string homedir)> users = new List<(string, string)>();
                
                try
                {
                    using var getUsersProcess = Process.Start(getUsersCmd);
                    if (getUsersProcess != null)
                    {
                        var usersOutput = getUsersProcess.StandardOutput.ReadToEnd();
                        getUsersProcess.WaitForExit();
                        
                        if (getUsersProcess.ExitCode == 0 && !string.IsNullOrWhiteSpace(usersOutput))
                        {
                            var userLines = usersOutput.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (var userLine in userLines)
                            {
                                var parts = userLine.Split(':');
                                if (parts.Length == 2)
                                {
                                    users.Add((parts[0].Trim(), parts[1].Trim()));
                                }
                            }
                        }
                    }
                }
                catch { }

                if (users.Count == 0)
                {
                    // Fallback to default user
                    users.Add(("default", "~"));
                }
                
                // ALWAYS include root user (home directory is /root, not /home/root)
                users.Add(("root", "/root"));

                Console.WriteLine($"    Found {users.Count} user(s) in {distro} (including root)");

                // Clean for each user
                foreach (var (username, homedir) in users)
                {
                    // Shell history files to delete
                    var historyFiles = new[]
                    {
                        $"{homedir}/.bash_history",
                        $"{homedir}/.zsh_history",
                        $"{homedir}/.zsh_sessions",
                        $"{homedir}/.local/share/fish/fish_history",
                        $"{homedir}/.history",
                        $"{homedir}/.sh_history",
                        $"{homedir}/.lesshst",
                        $"{homedir}/.mysql_history",
                        $"{homedir}/.psql_history",
                        $"{homedir}/.sqlite_history",
                        $"{homedir}/.python_history",
                        $"{homedir}/.node_repl_history",
                        $"{homedir}/.irb_history"
                    };

                    foreach (var historyFile in historyFiles)
                    {
                        try
                        {
                            var deleteCmd = new ProcessStartInfo
                            {
                                FileName = "wsl",
                                Arguments = $"-d {distro} --user root bash -c \"rm -f {historyFile} 2>/dev/null || true\"",
                                UseShellExecute = false,
                                CreateNoWindow = true,
                                RedirectStandardOutput = true,
                                RedirectStandardError = true
                            };

                            if (_dryRun)
                            {
                                LogDryRun($"Would delete: {distro}:{username}:{historyFile}");
                                clearedCount++;
                            }
                            else
                            {
                                using var deleteProcess = Process.Start(deleteCmd);
                                if (deleteProcess != null)
                                {
                                    deleteProcess.WaitForExit();
                                    if (deleteProcess.ExitCode == 0)
                                    {
                                        clearedCount++;
                                    }
                                }
                            }
                        }
                        catch { /* Skip files that don't exist or can't be deleted */ }
                    }

                    // Clear user-specific cache, tmp, and log directories
                    var userDirsToClean = new[]
                    {
                        $"{homedir}/.cache",
                        $"{homedir}/.tmp",
                        $"{homedir}/tmp",
                        $"{homedir}/.local/share/Trash",
                        $"{homedir}/.thumbnails",
                        
                        // Desktop environment caches
                        $"{homedir}/.cache/gnome-software",
                        $"{homedir}/.cache/gnome-shell",
                        $"{homedir}/.cache/kde4",
                        $"{homedir}/.cache/plasmashell",
                        $"{homedir}/.cache/kioexec",
                        $"{homedir}/.cache/ksycoca5",
                        $"{homedir}/.cache/gstreamer-1.0",
                        $"{homedir}/.cache/thumbnails",
                        $"{homedir}/.cache/fontconfig",
                        $"{homedir}/.cache/mesa_shader_cache",
                        
                        // GTK/Qt caches
                        $"{homedir}/.cache/gtk-3.0",
                        $"{homedir}/.cache/gtk-4.0",
                        $"{homedir}/.cache/qt",
                        
                        // Browser caches (if running browsers in WSL)
                        $"{homedir}/.cache/mozilla",
                        $"{homedir}/.cache/chromium",
                        $"{homedir}/.cache/google-chrome",
                        
                        // Config app logs
                        $"{homedir}/.config/*/logs",
                        $"{homedir}/.config/*/cache",
                        $"{homedir}/.local/share/xorg",
                        
                        // Development tool caches
                        $"{homedir}/.npm/_logs",
                        $"{homedir}/.npm/_cacache",
                        $"{homedir}/.yarn/cache",
                        $"{homedir}/.pnpm-store",
                        $"{homedir}/.cargo/registry/cache",
                        $"{homedir}/.cargo/git/checkouts",
                        $"{homedir}/.rustup/downloads",
                        $"{homedir}/.m2/repository",
                        $"{homedir}/.gradle/caches",
                        $"{homedir}/.gradle/daemon",
                        $"{homedir}/.gradle/wrapper",
                        $"{homedir}/.sbt/boot",
                        $"{homedir}/.sbt/staging",
                        $"{homedir}/.ivy2/cache",
                        $"{homedir}/.nuget/packages",
                        $"{homedir}/.cache/pip",
                        $"{homedir}/.cache/go-build",
                        $"{homedir}/.cache/composer",
                        $"{homedir}/.composer/cache",
                        $"{homedir}/.gem/ruby",
                        $"{homedir}/.bundle/cache",
                        
                        // IDE caches
                        $"{homedir}/.vscode/extensions/*/logs",
                        $"{homedir}/.vscode-server/data/logs",
                        $"{homedir}/.IntelliJIdea*/system/log",
                        $"{homedir}/.PyCharm*/system/log",
                        
                        // Docker/Container caches
                        $"{homedir}/.docker/buildx/cache",
                        $"{homedir}/.docker/scan/cache",
                        
                        // Network/DNS caches
                        $"{homedir}/.cache/dconf",
                        $"{homedir}/.wget-hsts",
                        $"{homedir}/.curlrc"
                    };

                    foreach (var dirPath in userDirsToClean)
                    {
                        try
                        {
                            // Get size first (for display)
                            var sizeCmd = new ProcessStartInfo
                            {
                                FileName = "wsl",
                                Arguments = $"-d {distro} --user root bash -c \"du -sb {dirPath} 2>/dev/null | cut -f1 || echo 0\"",
                                UseShellExecute = false,
                                CreateNoWindow = true,
                                RedirectStandardOutput = true,
                                RedirectStandardError = true
                            };

                            long dirSize = 0;
                            try
                            {
                                using var sizeProcess = Process.Start(sizeCmd);
                                if (sizeProcess != null)
                                {
                                    var sizeOutput = sizeProcess.StandardOutput.ReadToEnd().Trim();
                                    sizeProcess.WaitForExit();
                                    long.TryParse(sizeOutput, out dirSize);
                                }
                            }
                            catch { }

                            if (dirSize > 0)
                            {
                                var deleteCmd = new ProcessStartInfo
                                {
                                    FileName = "wsl",
                                    Arguments = $"-d {distro} --user root bash -c \"rm -rf {dirPath} 2>/dev/null || true\"",
                                    UseShellExecute = false,
                                    CreateNoWindow = true,
                                    RedirectStandardOutput = true,
                                    RedirectStandardError = true
                                };

                                if (_dryRun)
                                {
                                    LogDryRun($"Would clean: {distro}:{username}:{dirPath} ({FormatBytes(dirSize)})");
                                    clearedCount++;
                                }
                                else
                                {
                                    using var deleteProcess = Process.Start(deleteCmd);
                                    if (deleteProcess != null)
                                    {
                                        deleteProcess.WaitForExit();
                                        clearedCount++;
                                    }
                                }
                                freedSpace += dirSize;
                            }
                        }
                        catch { /* Skip directories that don't exist or can't be accessed */ }
                    }
                }

                // Clear system-wide directories (run as root)
                var systemDirsToClean = new[]
                {
                    // Standard temp and log directories
                    "/tmp",
                    "/var/tmp",
                    "/var/log",
                    
                    // Package manager caches
                    "/var/cache/apt/archives",
                    "/var/cache/apt/pkgcache.bin",
                    "/var/cache/apt/srcpkgcache.bin",
                    "/var/cache/yum",
                    "/var/cache/dnf",
                    "/var/cache/pacman/pkg",
                    "/var/cache/zypp",
                    "/var/cache/apk",
                    
                    // Systemd/journald logs
                    "/var/log/journal",
                    "/run/log/journal",
                    
                    // System logs
                    "/var/log/syslog",
                    "/var/log/messages",
                    "/var/log/kern.log",
                    "/var/log/auth.log",
                    "/var/log/daemon.log",
                    "/var/log/user.log",
                    "/var/log/debug",
                    "/var/log/lastlog",
                    "/var/log/wtmp",
                    "/var/log/btmp",
                    "/var/log/faillog",
                    
                    // Networking logs and caches
                    "/var/log/mail.log",
                    "/var/log/mail.err",
                    "/var/log/apache2",
                    "/var/log/nginx",
                    "/var/log/httpd",
                    "/var/cache/bind",
                    "/var/cache/nscd",
                    
                    // Desktop environment caches
                    "/var/cache/fontconfig",
                    "/var/cache/man",
                    "/var/cache/thumbnails",
                    
                    // Application logs
                    "/var/log/cups",
                    "/var/log/mysql",
                    "/var/log/postgresql",
                    "/var/log/mongodb",
                    "/var/log/redis",
                    "/var/log/docker",
                    "/var/log/containerd",
                    
                    // Crash reports
                    "/var/crash",
                    "/var/lib/systemd/coredump",
                    
                    // Old rotated logs
                    "/var/log/*.gz",
                    "/var/log/*.1",
                    "/var/log/*.old"
                };

                foreach (var dirPath in systemDirsToClean)
                {
                    try
                    {
                        // Get size first (for display)
                        var sizeCmd = new ProcessStartInfo
                        {
                            FileName = "wsl",
                            Arguments = $"-d {distro} --user root bash -c \"du -sb {dirPath} 2>/dev/null | cut -f1 || echo 0\"",
                            UseShellExecute = false,
                            CreateNoWindow = true,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true
                        };

                        long dirSize = 0;
                        try
                        {
                            using var sizeProcess = Process.Start(sizeCmd);
                            if (sizeProcess != null)
                            {
                                var sizeOutput = sizeProcess.StandardOutput.ReadToEnd().Trim();
                                sizeProcess.WaitForExit();
                                long.TryParse(sizeOutput, out dirSize);
                            }
                        }
                        catch { }

                        if (dirSize > 0)
                        {
                            // For /var/log and /tmp, only remove contents, not the directory itself
                            string cleanCommand;
                            if (dirPath == "/var/log" || dirPath == "/tmp" || dirPath == "/var/tmp")
                            {
                                cleanCommand = $"-d {distro} --user root bash -c \"find {dirPath} -type f -delete 2>/dev/null || true\"";
                            }
                            else
                            {
                                cleanCommand = $"-d {distro} --user root bash -c \"rm -rf {dirPath}/* 2>/dev/null || true\"";
                            }

                            var deleteCmd = new ProcessStartInfo
                            {
                                FileName = "wsl",
                                Arguments = cleanCommand,
                                UseShellExecute = false,
                                CreateNoWindow = true,
                                RedirectStandardOutput = true,
                                RedirectStandardError = true
                            };

                            if (_dryRun)
                            {
                                LogDryRun($"Would clean: {distro}:system:{dirPath} ({FormatBytes(dirSize)})");
                                clearedCount++;
                            }
                            else
                            {
                                using var deleteProcess = Process.Start(deleteCmd);
                                if (deleteProcess != null)
                                {
                                    deleteProcess.WaitForExit();
                                    clearedCount++;
                                }
                            }
                            freedSpace += dirSize;
                        }
                    }
                    catch { /* Skip directories that don't exist or can't be accessed */ }
                }

                // Clear package manager caches (run as root)
                var packageCacheCommands = new[]
                {
                    ("apt", "apt-get clean 2>/dev/null || true"),
                    ("apt-autoremove", "apt-get autoremove -y 2>/dev/null || true"),
                    ("yum", "yum clean all 2>/dev/null || true"),
                    ("dnf", "dnf clean all 2>/dev/null || true"),
                    ("zypper", "zypper clean --all 2>/dev/null || true"),
                    ("pacman", "pacman -Scc --noconfirm 2>/dev/null || true"),
                    ("apk", "apk cache clean 2>/dev/null || true")
                };

                foreach (var (manager, cleanCmd) in packageCacheCommands)
                {
                    try
                    {
                        var cmd = new ProcessStartInfo
                        {
                            FileName = "wsl",
                            Arguments = $"-d {distro} --user root bash -c \"{cleanCmd}\"",
                            UseShellExecute = false,
                            CreateNoWindow = true,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true
                        };

                        if (_dryRun)
                        {
                            LogDryRun($"Would clean: {distro} {manager} cache");
                        }
                        else
                        {
                            using var process = Process.Start(cmd);
                            if (process != null)
                            {
                                process.WaitForExit(10000); // 10 second timeout
                            }
                        }
                    }
                    catch { }
                }
                
                // Clear systemd journal logs (if systemd is present)
                try
                {
                    var journalCleanCmd = new ProcessStartInfo
                    {
                        FileName = "wsl",
                        Arguments = $"-d {distro} --user root bash -c \"journalctl --vacuum-time=1d 2>/dev/null || true\"",
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    };
                    
                    if (_dryRun)
                    {
                        LogDryRun($"Would clean: {distro} systemd journal logs");
                    }
                    else
                    {
                        using var process = Process.Start(journalCleanCmd);
                        if (process != null)
                        {
                            process.WaitForExit(5000);
                        }
                    }
                }
                catch { }
                
                // Clear thumbnail caches (desktop environments)
                try
                {
                    var thumbCmd = new ProcessStartInfo
                    {
                        FileName = "wsl",
                        Arguments = $"-d {distro} --user root bash -c \"rm -rf /home/*/.cache/thumbnails/* 2>/dev/null || true\"",
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    };
                    
                    if (!_dryRun)
                    {
                        using var process = Process.Start(thumbCmd);
                        if (process != null)
                        {
                            process.WaitForExit(5000);
                        }
                    }
                }
                catch { }
                
                // Clear old log files (*.gz, *.1, *.old)
                try
                {
                    var oldLogsCmd = new ProcessStartInfo
                    {
                        FileName = "wsl",
                        Arguments = $"-d {distro} --user root bash -c \"find /var/log -type f \\( -name '*.gz' -o -name '*.1' -o -name '*.old' \\) -delete 2>/dev/null || true\"",
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    };
                    
                    if (!_dryRun)
                    {
                        using var process = Process.Start(oldLogsCmd);
                        if (process != null)
                        {
                            process.WaitForExit(5000);
                        }
                    }
                }
                catch { }
            }

            if (clearedCount > 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"  ✅ {(_dryRun ? "Would clean" : "Cleaned")} {clearedCount} WSL items across {distros.Count} distribution(s) ({FormatBytes(freedSpace)} {(_dryRun ? "would be freed" : "freed")})");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("  ⚠️ No WSL items found to clean");
                Console.ResetColor();
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  ❌ Failed: {ex.Message}");
            Console.ResetColor();
        }
    }

    static void ClearCopilotHistory()
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("\n🔹 Clearing GitHub Copilot CLI logs, cache, and sessions...");
        Console.ResetColor();

        try
        {
            var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var copilotPath = Path.Combine(userProfile, ".copilot");

            if (!Directory.Exists(copilotPath))
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("  ⚠️ No GitHub Copilot CLI directory found");
                Console.ResetColor();
                return;
            }

            long freedSpace = 0;
            int deletedFiles = 0;
            int deletedDirs = 0;

            // Comprehensive list of directories to clear in .copilot
            var copilotDirsToClean = new[]
            {
                "session-state",  // Session state and checkpoints
                "cache",          // Cache files
                "logs",           // Log files
                "telemetry",      // Telemetry data
                "tmp",            // Temporary files
                "temp",           // Temporary files
                "checkpoints"     // Checkpoints
            };

            foreach (var dirName in copilotDirsToClean)
            {
                var dirPath = Path.Combine(copilotPath, dirName);
                if (Directory.Exists(dirPath))
                {
                    try
                    {
                        var files = Directory.GetFiles(dirPath, "*.*", SearchOption.AllDirectories);
                        foreach (var file in files)
                        {
                            try
                            {
                                var fileInfo = new FileInfo(file);
                                freedSpace += fileInfo.Length;
                                
                                if (_dryRun)
                                {
                                    LogDryRun($"Would delete: {file}");
                                }
                                else
                                {
                                    fileInfo.Delete();
                                }
                                deletedFiles++;
                            }
                            catch { /* Skip files in use */ }
                        }

                        // Remove empty directories
                        if (!_dryRun)
                        {
                            try
                            {
                                var dirs = Directory.GetDirectories(dirPath);
                                foreach (var dir in dirs)
                                {
                                    try
                                    {
                                        Directory.Delete(dir, true);
                                        deletedDirs++;
                                    }
                                    catch { /* Skip if not empty */ }
                                }
                            }
                            catch { }
                        }

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"  ✅ {(_dryRun ? "Would clear" : "Cleared")} {dirName}");
                        Console.ResetColor();
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.WriteLine($"  ⚠️ Could not fully clear {dirName}: {ex.Message}");
                        Console.ResetColor();
                    }
                }
            }

            // Clear individual log files in .copilot root
            try
            {
                var logFiles = Directory.GetFiles(copilotPath, "*.log", SearchOption.TopDirectoryOnly);
                var tmpFiles = Directory.GetFiles(copilotPath, "*.tmp", SearchOption.TopDirectoryOnly);
                
                foreach (var file in logFiles.Concat(tmpFiles))
                {
                    try
                    {
                        var fileInfo = new FileInfo(file);
                        freedSpace += fileInfo.Length;
                        
                        if (_dryRun)
                        {
                            LogDryRun($"Would delete: {file}");
                        }
                        else
                        {
                            fileInfo.Delete();
                        }
                        deletedFiles++;
                    }
                    catch { }
                }
            }
            catch { }

            // Also check LocalAppData for GitHub Copilot cache (if exists)
            var localCopilotPaths = new[]
            {
                Path.Combine(localAppData, "github-copilot"),
                Path.Combine(localAppData, "GitHub", "Copilot")
            };

            foreach (var localPath in localCopilotPaths)
            {
                if (Directory.Exists(localPath))
                {
                    var localDirs = new[] { "cache", "logs", "tmp" };
                    foreach (var dirName in localDirs)
                    {
                        var dirPath = Path.Combine(localPath, dirName);
                        if (Directory.Exists(dirPath))
                        {
                            try
                            {
                                var size = GetDirectorySize(dirPath);
                                
                                if (_dryRun)
                                {
                                    LogDryRun($"Would delete: {dirPath} ({FormatBytes(size)})");
                                }
                                else
                                {
                                    Directory.Delete(dirPath, true);
                                }
                                
                                freedSpace += size;
                                deletedDirs++;
                                
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine($"  ✅ {(_dryRun ? "Would clear" : "Cleared")} {localPath}\\{dirName}");
                                Console.ResetColor();
                            }
                            catch { }
                        }
                    }
                }
            }

            if (deletedFiles > 0 || deletedDirs > 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"  ✅ {(_dryRun ? "Would delete" : "Deleted")} {deletedFiles} files and {deletedDirs} directories ({FormatBytes(freedSpace)} {(_dryRun ? "would be freed" : "freed")})");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("  ⚠️ No GitHub Copilot CLI cache/log files found to clear");
                Console.ResetColor();
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  ❌ Failed: {ex.Message}");
            Console.ResetColor();
        }
    }

    static void ClearVSCodeCopilotCache()
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("\n🔹 Clearing VS Code Copilot cache...");
        Console.ResetColor();

        long freedSpace = 0;
        int clearedCount = 0;
        bool foundAny = false;

        try
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            
            // VS Code Copilot cache locations
            var vscodeVariants = new[]
            {
                ("VS Code", Path.Combine(appData, "Code")),
                ("VS Code Insiders", Path.Combine(appData, "Code - Insiders"))
            };

            foreach (var (name, basePath) in vscodeVariants)
            {
                if (!Directory.Exists(basePath))
                    continue;

                foundAny = true;
                Console.WriteLine($"  Cleaning {name} Copilot cache...");

                // Clear Copilot extension cache
                var copilotPaths = new[]
                {
                    Path.Combine(basePath, "User", "globalStorage", "github.copilot"),
                    Path.Combine(basePath, "User", "globalStorage", "github.copilot-chat"),
                    Path.Combine(basePath, "User", "workspaceStorage") // Copilot workspace cache
                };

                foreach (var copilotPath in copilotPaths)
                {
                    if (Directory.Exists(copilotPath))
                    {
                        try
                        {
                            // For workspaceStorage, only clear Copilot-related folders
                            if (copilotPath.EndsWith("workspaceStorage"))
                            {
                                var workspaceFolders = Directory.GetDirectories(copilotPath);
                                foreach (var workspaceFolder in workspaceFolders)
                                {
                                    try
                                    {
                                        var copilotFile = Path.Combine(workspaceFolder, "copilot");
                                        if (File.Exists(copilotFile) || Directory.Exists(copilotFile))
                                        {
                                            var size = GetDirectorySize(workspaceFolder);
                                            if (!_dryRun)
                                            {
                                                Directory.Delete(workspaceFolder, true);
                                            }
                                            freedSpace += size;
                                            clearedCount++;
                                            if (_dryRun)
                                            {
                                                LogDryRun($"Would delete: {workspaceFolder} ({FormatBytes(size)})");
                                            }
                                        }
                                    }
                                    catch { }
                                }
                            }
                            else
                            {
                                var size = GetDirectorySize(copilotPath);
                                if (!_dryRun)
                                {
                                    Directory.Delete(copilotPath, true);
                                }
                                freedSpace += size;
                                clearedCount++;
                                if (_dryRun)
                                {
                                    LogDryRun($"Would delete: {copilotPath} ({FormatBytes(size)})");
                                }
                            }
                        }
                        catch { }
                    }
                }

                // Clear Copilot logs
                var logsPath = Path.Combine(basePath, "logs");
                if (Directory.Exists(logsPath))
                {
                    try
                    {
                        var copilotLogs = Directory.GetFiles(logsPath, "*copilot*", SearchOption.AllDirectories);
                        foreach (var log in copilotLogs)
                        {
                            try
                            {
                                var fileInfo = new FileInfo(log);
                                freedSpace += fileInfo.Length;
                                if (!_dryRun)
                                {
                                    fileInfo.Delete();
                                }
                                clearedCount++;
                                if (_dryRun)
                                {
                                    LogDryRun($"Would delete: {log} ({FormatBytes(fileInfo.Length)})");
                                }
                            }
                            catch { }
                        }
                    }
                    catch { }
                }
            }

            if (foundAny)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                if (_dryRun)
                {
                    Console.WriteLine($"  ✅ Would clear {clearedCount} VS Code Copilot cache items ({FormatBytes(freedSpace)} freed)");
                }
                else
                {
                    Console.WriteLine($"  ✅ {(_dryRun ? "Would clear" : "Cleared")} {clearedCount} VS Code Copilot cache items ({FormatBytes(freedSpace)} {(_dryRun ? "would be freed" : "freed")})");
                }
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("  ⚠️ VS Code installations not detected");
                Console.ResetColor();
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  ❌ Failed: {ex.Message}");
            Console.ResetColor();
        }
    }

    static void ClearCopilotPlusCache()
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("\n🔹 Clearing Windows Copilot+ (AI features) cache and logs...");
        Console.ResetColor();

        long freedSpace = 0;
        int clearedCount = 0;
        bool foundAny = false;

        try
        {
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var programData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            // Windows Copilot+ / AI Platform cache locations
            var copilotPlusPaths = new[]
            {
                // Windows Copilot app cache (Windows 11 Copilot sidebar)
                Path.Combine(localAppData, "Packages", "Microsoft.Windows.Ai.Copilot_cw5n1h2txyewy"),
                Path.Combine(localAppData, "Packages", "MicrosoftWindows.Client.WebExperience_cw5n1h2txyewy"), // Widget/Copilot container
                
                // AI Platform services
                Path.Combine(localAppData, "Microsoft", "Windows", "AIService"),
                Path.Combine(localAppData, "Microsoft", "Windows", "AI"),
                Path.Combine(programData, "Microsoft", "Windows", "AIService"),
                
                // Copilot Runtime / AI models cache
                Path.Combine(localAppData, "Microsoft", "CopilotRuntime"),
                Path.Combine(programData, "Microsoft", "CopilotRuntime"),
                Path.Combine(localAppData, "Microsoft", "AIModels"),
                Path.Combine(programData, "Microsoft", "AIModels"),
                
                // Windows ML / DirectML cache
                Path.Combine(localAppData, "Microsoft", "Windows", "MachineLearning"),
                Path.Combine(programData, "Microsoft", "Windows", "MachineLearning"),
                
                // Copilot+ specific features
                Path.Combine(localAppData, "Microsoft", "Windows", "CopilotPlus"),
                Path.Combine(programData, "Microsoft", "Windows", "CopilotPlus"),
                
                // AI-powered features cache
                Path.Combine(localAppData, "Microsoft", "Windows", "StudioEffects"), // Camera effects
                Path.Combine(localAppData, "Microsoft", "Windows", "LiveCaptions"),
                Path.Combine(localAppData, "Microsoft", "Windows", "VoiceAccess"),
                Path.Combine(localAppData, "Microsoft", "Windows", "Recall"), // Windows Recall feature
                
                // Bing Chat / Edge Copilot integration
                Path.Combine(localAppData, "Microsoft", "BingChat"),
                Path.Combine(localAppData, "Microsoft", "EdgeCopilot"),
                
                // Neural Processing Unit (NPU) related
                Path.Combine(localAppData, "Microsoft", "NPU"),
                Path.Combine(programData, "Microsoft", "NPU"),
            };

            foreach (var basePath in copilotPlusPaths)
            {
                if (!Directory.Exists(basePath))
                    continue;

                foundAny = true;
                var folderName = Path.GetFileName(basePath);
                Console.WriteLine($"  Cleaning {folderName}...");

                // Clear cache subdirectories
                var cacheSubdirs = new[] { "Cache", "Logs", "Temp", "LocalCache", "AC", "Telemetry", "CrashReports" };
                foreach (var subdir in cacheSubdirs)
                {
                    var cachePath = Path.Combine(basePath, subdir);
                    if (Directory.Exists(cachePath))
                    {
                        try
                        {
                            var files = Directory.GetFiles(cachePath, "*.*", SearchOption.AllDirectories);
                            foreach (var file in files)
                            {
                                try
                                {
                                    if (_dryRun)
                                    {
                                        var fileInfo = new FileInfo(file);
                                        freedSpace += fileInfo.Length;
                                        LogDryRun($"Would delete: {file} ({FormatBytes(fileInfo.Length)})");
                                        clearedCount++;
                                    }
                                    else
                                    {
                                        var fileInfo = new FileInfo(file);
                                        freedSpace += fileInfo.Length;
                                        fileInfo.Delete();
                                        clearedCount++;
                                    }
                                }
                                catch { /* Skip files in use */ }
                            }

                            if (!_dryRun)
                            {
                                // Try to remove empty directories
                                try { Directory.Delete(cachePath, true); } catch { /* Keep if not empty */ }
                            }
                        }
                        catch { /* Skip if inaccessible */ }
                    }
                }

                // Clear LocalState cache files (but preserve settings)
                var localStatePath = Path.Combine(basePath, "LocalState");
                if (Directory.Exists(localStatePath))
                {
                    try
                    {
                        var cacheFiles = Directory.GetFiles(localStatePath, "*.cache", SearchOption.AllDirectories)
                            .Concat(Directory.GetFiles(localStatePath, "*.tmp", SearchOption.AllDirectories))
                            .Concat(Directory.GetFiles(localStatePath, "*.log", SearchOption.AllDirectories));

                        foreach (var file in cacheFiles)
                        {
                            try
                            {
                                if (_dryRun)
                                {
                                    var fileInfo = new FileInfo(file);
                                    freedSpace += fileInfo.Length;
                                    LogDryRun($"Would delete: {file} ({FormatBytes(fileInfo.Length)})");
                                    clearedCount++;
                                }
                                else
                                {
                                    var fileInfo = new FileInfo(file);
                                    freedSpace += fileInfo.Length;
                                    fileInfo.Delete();
                                    clearedCount++;
                                }
                            }
                            catch { /* Skip files in use */ }
                        }
                    }
                    catch { /* Skip if inaccessible */ }
                }
            }

            // Clear Microsoft Edge Copilot sidebar cache (separate from main Edge cache)
            var edgeCopilotPaths = new[]
            {
                Path.Combine(localAppData, "Microsoft", "Edge", "User Data", "Default", "Service Worker", "CacheStorage"),
                Path.Combine(localAppData, "Microsoft", "Edge", "User Data", "Default", "Copilot"),
                Path.Combine(localAppData, "Microsoft", "Edge Beta", "User Data", "Default", "Copilot"),
                Path.Combine(localAppData, "Microsoft", "Edge Dev", "User Data", "Default", "Copilot"),
                Path.Combine(localAppData, "Microsoft", "Edge SxS", "User Data", "Default", "Copilot"),
            };

            foreach (var path in edgeCopilotPaths)
            {
                if (Directory.Exists(path))
                {
                    foundAny = true;
                    try
                    {
                        var files = Directory.GetFiles(path, "*copilot*", SearchOption.AllDirectories)
                            .Concat(Directory.GetFiles(path, "*bing*", SearchOption.AllDirectories))
                            .Concat(Directory.GetFiles(path, "*ai*", SearchOption.AllDirectories));

                        foreach (var file in files)
                        {
                            try
                            {
                                if (_dryRun)
                                {
                                    var fileInfo = new FileInfo(file);
                                    freedSpace += fileInfo.Length;
                                    LogDryRun($"Would delete: {file} ({FormatBytes(fileInfo.Length)})");
                                    clearedCount++;
                                }
                                else
                                {
                                    var fileInfo = new FileInfo(file);
                                    freedSpace += fileInfo.Length;
                                    fileInfo.Delete();
                                    clearedCount++;
                                }
                            }
                            catch { /* Skip files in use */ }
                        }
                    }
                    catch { /* Skip if inaccessible */ }
                }
            }

            if (foundAny && clearedCount > 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                if (_dryRun)
                {
                    Console.WriteLine($"  ✅ Would clear {clearedCount} Copilot+ cache/log items ({FormatBytes(freedSpace)} would be freed)");
                }
                else
                {
                    Console.WriteLine($"  ✅ {(_dryRun ? "Would clear" : "Cleared")} {clearedCount} Copilot+ cache/log items ({FormatBytes(freedSpace)} {(_dryRun ? "would be freed" : "freed")})");
                }
                Console.ResetColor();
            }
            else if (!foundAny)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("  ⚠️ No Windows Copilot+ features detected on this system");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("  ⚠️ No cached files found to clean");
                Console.ResetColor();
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  ❌ Failed: {ex.Message}");
            Console.ResetColor();
        }
    }

    static void ClearVisualStudioCache()
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("\n🔹 Clearing Visual Studio cache and logs...");
        Console.ResetColor();

        long freedSpace = 0;
        int clearedCount = 0;

        try
        {
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var tempPath = Path.GetTempPath();

            // Visual Studio versions to check (2025, 2022, 2019, 2017, etc.)
            var vsVersions = new[] { "2025", "2022", "2019", "2017", "2015", "2013" };
            var vsEditions = new[] { "Enterprise", "Professional", "Community", "Preview", "BuildTools" };

            foreach (var version in vsVersions)
            {
                foreach (var edition in vsEditions)
                {
                    var vsName = $"Visual Studio {version} {edition}";
                    
                    // Check if this VS version exists
                    var vsAppDataPath = Path.Combine(localAppData, "Microsoft", "VisualStudio");
                    if (!Directory.Exists(vsAppDataPath))
                        continue;

                    // Clear Component Model Cache (MEF cache)
                    var componentModelCache = Path.Combine(localAppData, "Microsoft", "VisualStudio", $"*{version}*", "ComponentModelCache");
                    ClearDirectoryPattern(componentModelCache, ref clearedCount, ref freedSpace);

                    // Clear Designer cache
                    var designerCache = Path.Combine(localAppData, "Microsoft", "VisualStudio", $"*{version}*", "Designer", "ShadowCache");
                    ClearDirectoryPattern(designerCache, ref clearedCount, ref freedSpace);

                    // Clear Code Analysis cache
                    var codeAnalysisCache = Path.Combine(localAppData, "Microsoft", "VisualStudio", $"*{version}*", "Cache");
                    ClearDirectoryPattern(codeAnalysisCache, ref clearedCount, ref freedSpace);

                    // Clear Telemetry data
                    var telemetryPath = Path.Combine(localAppData, "Microsoft", "VisualStudio", $"*{version}*", "Telemetry");
                    ClearDirectoryPattern(telemetryPath, ref clearedCount, ref freedSpace);

                    // Clear crash dumps and diagnostics
                    var diagnosticsPath = Path.Combine(localAppData, "Microsoft", "VisualStudio", $"*{version}*", "CrashDumps");
                    ClearDirectoryPattern(diagnosticsPath, ref clearedCount, ref freedSpace);

                    var logsPath = Path.Combine(localAppData, "Microsoft", "VisualStudio", $"*{version}*", "Logs");
                    ClearDirectoryPattern(logsPath, ref clearedCount, ref freedSpace);

                    // Clear temp files
                    var vsTempPath = Path.Combine(localAppData, "Temp", $"VS*{version}*");
                    ClearDirectoryPattern(vsTempPath, ref clearedCount, ref freedSpace);
                }
            }

            // Clear .NET temporary ASP.NET files (all versions)
            var aspNetTempPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), 
                "Microsoft.NET", "Framework*", "v*", "Temporary ASP.NET Files");
            ClearDirectoryPattern(aspNetTempPath, ref clearedCount, ref freedSpace);

            // Clear MSBuild logs
            var msbuildLogPath = Path.Combine(localAppData, "Microsoft", "MSBuild", "*", "Logs");
            ClearDirectoryPattern(msbuildLogPath, ref clearedCount, ref freedSpace);

            // Clear Test Results (common locations)
            var testResultsPaths = new[]
            {
                Path.Combine(localAppData, "Microsoft", "VisualStudio", "*", "TestResults"),
                Path.Combine(tempPath, "TestResults")
            };
            foreach (var path in testResultsPaths)
            {
                ClearDirectoryPattern(path, ref clearedCount, ref freedSpace);
            }

            // Clear Visual Studio Code coverage data
            var coveragePath = Path.Combine(localAppData, "Microsoft", "VisualStudio", "*", "Coverage");
            ClearDirectoryPattern(coveragePath, ref clearedCount, ref freedSpace);

            // Clear IntelliTrace logs
            var intelliTracePath = Path.Combine(localAppData, "Microsoft", "VisualStudio", "*", "IntelliTrace");
            ClearDirectoryPattern(intelliTracePath, ref clearedCount, ref freedSpace);

            // Clear Visual Studio extensions temp files
            var extensionsTempPath = Path.Combine(localAppData, "Microsoft", "VisualStudio", "*", "Extensions", ".temp");
            ClearDirectoryPattern(extensionsTempPath, ref clearedCount, ref freedSpace);

            // Clear NuGet HTTP cache (but preserve packages cache as it's shared)
            var nugetHttpCache = Path.Combine(localAppData, "NuGet", "v3-cache");
            if (Directory.Exists(nugetHttpCache))
            {
                try
                {
                    var size = GetDirectorySize(nugetHttpCache);
                    if (!_dryRun)
                    {
                        Directory.Delete(nugetHttpCache, true);
                    }
                    freedSpace += size;
                    clearedCount++;
                    if (_dryRun)
                    {
                        LogDryRun($"Would delete: {nugetHttpCache} ({FormatBytes(size)})");
                    }
                }
                catch { }
            }

            // Clear Roslyn compiler cache
            var roslynCache = Path.Combine(localAppData, "Microsoft", "VisualStudio", "*", "Roslyn", "Cache");
            ClearDirectoryPattern(roslynCache, ref clearedCount, ref freedSpace);

            // Clear Xamarin logs and cache
            var xamarinLogs = Path.Combine(localAppData, "Xamarin", "Logs");
            if (Directory.Exists(xamarinLogs))
            {
                try
                {
                    var size = GetDirectorySize(xamarinLogs);
                    if (!_dryRun)
                    {
                        Directory.Delete(xamarinLogs, true);
                    }
                    freedSpace += size;
                    clearedCount++;
                    if (_dryRun)
                    {
                        LogDryRun($"Would delete: {xamarinLogs} ({FormatBytes(size)})");
                    }
                }
                catch { }
            }

            // Clear Visual Studio Installer logs
            var vsInstallerLogs = Path.Combine(tempPath, "VSInstallerLogs");
            if (Directory.Exists(vsInstallerLogs))
            {
                try
                {
                    var size = GetDirectorySize(vsInstallerLogs);
                    if (!_dryRun)
                    {
                        Directory.Delete(vsInstallerLogs, true);
                    }
                    freedSpace += size;
                    clearedCount++;
                    if (_dryRun)
                    {
                        LogDryRun($"Would delete: {vsInstallerLogs} ({FormatBytes(size)})");
                    }
                }
                catch { }
            }

            // Clear dd_setup logs (Visual Studio setup)
            var setupLogs = Directory.GetFiles(tempPath, "dd_setup_*.log", SearchOption.TopDirectoryOnly);
            foreach (var log in setupLogs)
            {
                try
                {
                    var fi = new FileInfo(log);
                    freedSpace += fi.Length;
                    if (!_dryRun)
                    {
                        fi.Delete();
                    }
                    clearedCount++;
                    if (_dryRun)
                    {
                        LogDryRun($"Would delete: {log} ({FormatBytes(fi.Length)})");
                    }
                }
                catch { }
            }

            Console.ForegroundColor = ConsoleColor.Green;
            if (_dryRun)
            {
                Console.WriteLine($"  ✅ Would clear {clearedCount} Visual Studio cache/log items ({FormatBytes(freedSpace)} freed)");
            }
            else
            {
                Console.WriteLine($"  ✅ {(_dryRun ? "Would clear" : "Cleared")} {clearedCount} Visual Studio cache/log items ({FormatBytes(freedSpace)} {(_dryRun ? "would be freed" : "freed")})");
            }
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  ❌ Failed: {ex.Message}");
            Console.ResetColor();
        }
    }

    // Helper function to clear directories matching a wildcard pattern
    static void ClearDirectoryPattern(string pattern, ref int clearedCount, ref long freedSpace)
    {
        try
        {
            var dirPath = Path.GetDirectoryName(pattern);
            var searchPattern = Path.GetFileName(pattern);
            
            if (dirPath == null || !Directory.Exists(Path.GetDirectoryName(dirPath)))
                return;

            // Handle wildcards in directory path
            var parentDir = Path.GetDirectoryName(dirPath);
            var dirPattern = Path.GetFileName(dirPath);
            
            if (parentDir != null && Directory.Exists(parentDir) && dirPattern != null && dirPattern.Contains("*"))
            {
                // Get matching directories
                var matchingDirs = Directory.GetDirectories(parentDir, dirPattern, SearchOption.TopDirectoryOnly);
                foreach (var matchingDir in matchingDirs)
                {
                    var fullPath = Path.Combine(matchingDir, searchPattern);
                    if (Directory.Exists(fullPath))
                    {
                        try
                        {
                            var size = GetDirectorySize(fullPath);
                            if (!_dryRun)
                            {
                                Directory.Delete(fullPath, true);
                            }
                            freedSpace += size;
                            clearedCount++;
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete: {fullPath} ({FormatBytes(size)})");
                            }
                        }
                        catch { }
                    }
                }
            }
            else if (Directory.Exists(dirPath))
            {
                // Direct path without wildcards
                if (Directory.Exists(pattern))
                {
                    var size = GetDirectorySize(pattern);
                    if (!_dryRun)
                    {
                        Directory.Delete(pattern, true);
                    }
                    freedSpace += size;
                    clearedCount++;
                    if (_dryRun)
                    {
                        LogDryRun($"Would delete: {pattern} ({FormatBytes(size)})");
                    }
                }
            }
        }
        catch { }
    }

    static void ClearCloudflareWarp()
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("\n🔹 Clearing Cloudflare WARP data...");
        Console.ResetColor();

        long freedSpace = 0;
        int clearedCount = 0;

        try
        {
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var programData = Environment.GetEnvironmentVariable("ProgramData") ?? @"C:\ProgramData";

            var warpPaths = new[]
            {
                Path.Combine(localAppData, "Cloudflare"),
                Path.Combine(programData, "Cloudflare")
            };

            bool foundWarp = false;

            foreach (var warpBasePath in warpPaths)
            {
                if (!Directory.Exists(warpBasePath))
                    continue;

                foundWarp = true;

                // Clear logs
                var logsPath = Path.Combine(warpBasePath, "logs");
                if (Directory.Exists(logsPath))
                {
                    try
                    {
                        var size = GetDirectorySize(logsPath);
                        Directory.Delete(logsPath, true);
                        Directory.CreateDirectory(logsPath);
                        freedSpace += size;
                        clearedCount++;
                        Console.WriteLine($"  ✅ {(_dryRun ? "Would clear" : "Cleared")} WARP logs");
                    }
                    catch { }
                }

                // Clear cache
                var cachePath = Path.Combine(warpBasePath, "cache");
                if (Directory.Exists(cachePath))
                {
                    try
                    {
                        var size = GetDirectorySize(cachePath);
                        Directory.Delete(cachePath, true);
                        Directory.CreateDirectory(cachePath);
                        freedSpace += size;
                        clearedCount++;
                        Console.WriteLine($"  ✅ {(_dryRun ? "Would clear" : "Cleared")} WARP cache");
                    }
                    catch { }
                }

                // Clear crash reports
                var crashPath = Path.Combine(warpBasePath, "crashes");
                if (Directory.Exists(crashPath))
                {
                    try
                    {
                        var size = GetDirectorySize(crashPath);
                        Directory.Delete(crashPath, true);
                        freedSpace += size;
                        clearedCount++;
                        Console.WriteLine($"  ✅ {(_dryRun ? "Would clear" : "Cleared")} WARP crash reports");
                    }
                    catch { }
                }

                // Clear diagnostics
                var diagPath = Path.Combine(warpBasePath, "diagnostics");
                if (Directory.Exists(diagPath))
                {
                    try
                    {
                        var size = GetDirectorySize(diagPath);
                        Directory.Delete(diagPath, true);
                        freedSpace += size;
                        clearedCount++;
                        Console.WriteLine($"  ✅ {(_dryRun ? "Would clear" : "Cleared")} WARP diagnostics");
                    }
                    catch { }
                }
            }

            if (foundWarp && clearedCount > 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"  ✅ {(_dryRun ? "Would clear" : "Cleared")} {clearedCount} WARP data items ({FormatBytes(freedSpace)} {(_dryRun ? "would be freed" : "freed")})");
                Console.ResetColor();
            }
            else if (!foundWarp)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("  ⚠️ Cloudflare WARP not detected");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("  ⚠️ No WARP data to clear");
                Console.ResetColor();
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  ❌ Failed: {ex.Message}");
            Console.ResetColor();
        }
    }

    static void ClearWindowsFeatures()
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("\n🔹 Clearing Windows features data...");
        Console.ResetColor();

        int clearedCount = 0;

        try
        {
            // Clear clipboard history (Windows 10/11)
            try
            {
                RunCommand("powershell", "-Command \"Remove-Item -Path 'HKCU:\\Software\\Microsoft\\Clipboard' -Recurse -Force -ErrorAction SilentlyContinue\"");
                clearedCount++;
            }
            catch { }

            // Clear Activity History / Timeline
            var activityPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "ConnectedDevicesPlatform");
            if (Directory.Exists(activityPath))
            {
                try
                {
                    Directory.Delete(activityPath, true);
                    clearedCount++;
                }
                catch { }
            }

            // Clear Windows Search history
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Search", true);
                if (key != null)
                {
                    var valuesToDelete = new[] { "RecentApps", "RecentDocs" };
                    foreach (var value in valuesToDelete)
                    {
                        try
                        {
                            if (!_dryRun)
                            {
                                key.DeleteSubKeyTree(value, false);
                            }
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete registry: HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\Search\\{value}");
                            }
                        }
                        catch { }
                    }
                    clearedCount++;
                }
            }
            catch { }

            // Clear Cortana data
            var cortanaPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Packages", "Microsoft.Windows.Cortana_cw5n1h2txyewy");
            if (Directory.Exists(cortanaPath))
            {
                try
                {
                    var localStatePath = Path.Combine(cortanaPath, "LocalState");
                    if (Directory.Exists(localStatePath))
                    {
                        if (!_dryRun)
                        {
                            Directory.Delete(localStatePath, true);
                        }
                        clearedCount++;
                        if (_dryRun)
                        {
                            var size = GetDirectorySize(localStatePath);
                            LogDryRun($"Would delete: {localStatePath} ({FormatBytes(size)})");
                        }
                    }
                }
                catch { }
            }

            // Clear Windows Spotlight cache
            var spotlightPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Packages", "Microsoft.Windows.ContentDeliveryManager_cw5n1h2txyewy", "LocalState", "Assets");
            if (Directory.Exists(spotlightPath))
            {
                try
                {
                    if (!_dryRun)
                    {
                        Directory.Delete(spotlightPath, true);
                    }
                    clearedCount++;
                    if (_dryRun)
                    {
                        var size = GetDirectorySize(spotlightPath);
                        LogDryRun($"Would delete: {spotlightPath} ({FormatBytes(size)})");
                    }
                }
                catch { }
            }

            // Clear notification history
            var notificationPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Microsoft", "Windows", "Notifications");
            if (Directory.Exists(notificationPath))
            {
                try
                {
                    var files = Directory.GetFiles(notificationPath, "*.db*");
                    foreach (var file in files)
                    {
                        try
                        {
                            if (!_dryRun)
                            {
                                File.Delete(file);
                            }
                            if (_dryRun)
                            {
                                var fi = new FileInfo(file);
                                LogDryRun($"Would delete: {file} ({FormatBytes(fi.Length)})");
                            }
                        }
                        catch { }
                    }
                    clearedCount++;
                }
                catch { }
            }

            Console.ForegroundColor = ConsoleColor.Green;
            if (_dryRun)
            {
                Console.WriteLine($"  ✅ Would clear {clearedCount} Windows feature data items");
            }
            else
            {
                Console.WriteLine($"  ✅ {(_dryRun ? "Would clear" : "Cleared")} {clearedCount} Windows feature data items");
            }
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  ❌ Failed: {ex.Message}");
            Console.ResetColor();
        }
    }

    static void ClearNetworkHistory()
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("\n🔹 Clearing network history...");
        Console.ResetColor();

        int clearedCount = 0;

        try
        {
            // Clear network share MRU
            var networkMRUKeys = new[]
            {
                @"Software\Microsoft\Windows\CurrentVersion\Explorer\Map Network Drive MRU",
                @"Software\Microsoft\Windows\CurrentVersion\Explorer\ComDlg32\LastVisitedPidlMRU",
                @"Software\Microsoft\Windows\CurrentVersion\Explorer\ComDlg32\OpenSavePidlMRU"
            };

            foreach (var keyPath in networkMRUKeys)
            {
                try
                {
                    using var key = Registry.CurrentUser.OpenSubKey(keyPath, true);
                    if (key != null)
                    {
                        var valueNames = key.GetValueNames();
                        foreach (var valueName in valueNames)
                        {
                            if (valueName != "MRUList" && valueName != "")
                            {
                                try
                                {
                                    if (!_dryRun)
                                    {
                                        key.DeleteValue(valueName);
                                    }
                                    clearedCount++;
                                    if (_dryRun)
                                    {
                                        LogDryRun($"Would delete registry value: {keyPath}\\{valueName}");
                                    }
                                }
                                catch { }
                            }
                        }
                        
                        if (key.GetValue("MRUList") != null)
                        {
                            if (!_dryRun)
                            {
                                key.SetValue("MRUList", "");
                            }
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete registry value: {keyPath}\\MRUList");
                            }
                        }
                    }
                }
                catch { }
            }

            // Clear Remote Desktop Connection history
            try
            {
                using var rdpKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Terminal Server Client", true);
                if (rdpKey != null)
                {
                    try
                    {
                        if (!_dryRun)
                        {
                            rdpKey.DeleteSubKeyTree("Default", false);
                            rdpKey.DeleteSubKeyTree("Servers", false);
                        }
                        clearedCount++;
                        if (_dryRun)
                        {
                            LogDryRun($"Would delete registry keys: Terminal Server Client\\Default and Terminal Server Client\\Servers");
                        }
                    }
                    catch { }
                }
            }
            catch { }

            Console.ForegroundColor = ConsoleColor.Green;
            if (_dryRun)
            {
                Console.WriteLine($"  ✅ Would clear {clearedCount} network history entries");
            }
            else
            {
                Console.WriteLine($"  ✅ {(_dryRun ? "Would clear" : "Cleared")} {clearedCount} network history entries");
            }
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  ❌ Failed: {ex.Message}");
            Console.ResetColor();
        }
    }

    static void ClearApplicationCaches()
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("\n🔹 Clearing application caches...");
        Console.ResetColor();

        long freedSpace = 0;
        int clearedCount = 0;

        try
        {
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var programData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

            // Clear Microsoft Teams cache
            var teamsPath = Path.Combine(localAppData, "Microsoft", "Teams");
            if (Directory.Exists(teamsPath))
            {
                // Removed IndexedDB and Local Storage - contain auth/settings
                var teamsCacheFolders = new[] { "Cache", "blob_storage", "databases", "GPUcache", "tmp" };
                foreach (var folder in teamsCacheFolders)
                {
                    var folderPath = Path.Combine(teamsPath, folder);
                    if (Directory.Exists(folderPath))
                    {
                        try
                        {
                            var size = GetDirectorySize(folderPath);
                            Directory.Delete(folderPath, true);
                            freedSpace += size;
                            clearedCount++;
                        }
                        catch { }
                    }
                }
            }

            // Clear OneDrive cache (not sync files, just cache)
            var oneDriveBasePaths = new[]
            {
                Path.Combine(localAppData, "Microsoft", "OneDrive"),
                Path.Combine(programData, "Microsoft OneDrive")
            };

            foreach (var oneDriveBasePath in oneDriveBasePaths)
            {
                if (Directory.Exists(oneDriveBasePath))
                {
                    var oneDrivePaths = new[]
                    {
                        Path.Combine(oneDriveBasePath, "logs"),
                        Path.Combine(oneDriveBasePath, "setup", "logs"),
                        Path.Combine(oneDriveBasePath, "settings", "logs"),
                        Path.Combine(oneDriveBasePath, "cache"),
                        Path.Combine(oneDriveBasePath, "OfficeFileCache"),
                        Path.Combine(oneDriveBasePath, "StandaloneUpdater", "OneDriveSetup.exe.log"),
                        Path.Combine(oneDriveBasePath, "ObfuscatedTelemetryLogs"),
                        Path.Combine(oneDriveBasePath, "TelemetryCache"),
                    };

                    foreach (var oneDrivePath in oneDrivePaths)
                    {
                        if (File.Exists(oneDrivePath))
                        {
                            try
                            {
                                var fileInfo = new FileInfo(oneDrivePath);
                                freedSpace += fileInfo.Length;
                                Directory.Delete(oneDrivePath, true);
                                clearedCount++;
                            }
                            catch { }
                        }
                        else if (Directory.Exists(oneDrivePath))
                        {
                            try
                            {
                                var size = GetDirectorySize(oneDrivePath);
                                Directory.Delete(oneDrivePath, true);
                                freedSpace += size;
                                clearedCount++;
                            }
                            catch { }
                        }
                    }
                }
            }

            // Clear Windows Store cache
            var storeCachePath = Path.Combine(localAppData, "Packages", "Microsoft.WindowsStore_8wekyb3d8bbwe", "LocalCache");
            if (Directory.Exists(storeCachePath))
            {
                try
                {
                    var size = GetDirectorySize(storeCachePath);
                    Directory.Delete(storeCachePath, true);
                    freedSpace += size;
                    clearedCount++;
                }
                catch { }
            }

            // Clear Office cache
            var officeCachePath = Path.Combine(localAppData, "Microsoft", "Office");
            if (Directory.Exists(officeCachePath))
            {
                var officeCacheFolders = new[] { "16.0\\OfficeFileCache", "15.0\\OfficeFileCache" };
                foreach (var folder in officeCacheFolders)
                {
                    var folderPath = Path.Combine(officeCachePath, folder);
                    if (Directory.Exists(folderPath))
                    {
                        try
                        {
                            var files = Directory.GetFiles(folderPath, "*.*");
                            foreach (var file in files)
                            {
                                try
                                {
                                    var fileInfo = new FileInfo(file);
                                    freedSpace += fileInfo.Length;
                                    
                                    if (_dryRun)
                                    {
                                        LogDryRun($"Would delete file: {file} ({FormatBytes(fileInfo.Length)})");
                                    }
                                    else
                                    {
                                        fileInfo.Delete();
                                    }
                                    clearedCount++;
                                }
                                catch { }
                            }
                        }
                        catch { }
                    }
                }
            }

            // Clear Windows Photos app
            var photosAppPaths = new[]
            {
                Path.Combine(localAppData, "Packages", "Microsoft.Windows.Photos_8wekyb3d8bbwe", "LocalCache"),
                Path.Combine(localAppData, "Packages", "Microsoft.Windows.Photos_8wekyb3d8bbwe", "TempState"),
                Path.Combine(localAppData, "Packages", "Microsoft.Windows.Photos_8wekyb3d8bbwe", "AC", "Temp")
            };

            foreach (var photosPath in photosAppPaths)
            {
                if (Directory.Exists(photosPath))
                {
                    try
                    {
                        var size = GetDirectorySize(photosPath);
                        
                        if (_dryRun)
                        {
                            LogDryRun($"Would delete directory: {photosPath} ({FormatBytes(size)})");
                        }
                        else
                        {
                            Directory.Delete(photosPath, true);
                        }
                        freedSpace += size;
                        clearedCount++;
                    }
                    catch { }
                }
            }

            // Clear Windows Photos logs
            var photosLogsPath = Path.Combine(localAppData, "Packages", "Microsoft.Windows.Photos_8wekyb3d8bbwe", "LocalState", "photosAppLogs");
            if (Directory.Exists(photosLogsPath))
            {
                try
                {
                    var files = Directory.GetFiles(photosLogsPath, "*.*");
                    foreach (var file in files)
                    {
                        try
                        {
                            var fileInfo = new FileInfo(file);
                            freedSpace += fileInfo.Length;
                            
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete file: {file}");
                            }
                            else
                            {
                                fileInfo.Delete();
                            }
                            clearedCount++;
                        }
                        catch { }
                    }
                }
                catch { }
            }

            // Clear Windows Media Player history and cache
            var wmpPaths = new[]
            {
                Path.Combine(localAppData, "Microsoft", "Media Player"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Microsoft", "Windows Media")
            };

            foreach (var wmpBasePath in wmpPaths)
            {
                if (Directory.Exists(wmpBasePath))
                {
                    try
                    {
                        // Clear cache and temp files
                        var cacheFiles = Directory.GetFiles(wmpBasePath, "*.wmdb", SearchOption.AllDirectories)
                            .Concat(Directory.GetFiles(wmpBasePath, "*.tmp", SearchOption.AllDirectories))
                            .Concat(Directory.GetFiles(wmpBasePath, "CurrentDatabase*", SearchOption.AllDirectories));

                        foreach (var file in cacheFiles)
                        {
                            try
                            {
                                var fileInfo = new FileInfo(file);
                                freedSpace += fileInfo.Length;
                                
                                if (_dryRun)
                                {
                                    LogDryRun($"Would delete file: {file}");
                                }
                                else
                                {
                                    fileInfo.Delete();
                                }
                                clearedCount++;
                            }
                            catch { }
                        }

                        // Clear album art cache
                        var albumArtPath = Path.Combine(wmpBasePath, "Art Cache");
                        if (Directory.Exists(albumArtPath))
                        {
                            try
                            {
                                var size = GetDirectorySize(albumArtPath);
                                
                                if (_dryRun)
                                {
                                    LogDryRun($"Would delete directory: {albumArtPath} ({FormatBytes(size)})");
                                }
                                else
                                {
                                    Directory.Delete(albumArtPath, true);
                                }
                                freedSpace += size;
                                clearedCount++;
                            }
                            catch { }
                        }
                    }
                    catch { }
                }
            }

            // Clear Windows Media Player recent playlist
            try
            {
                var wmpRecentKey = @"Software\Microsoft\MediaPlayer\Player\RecentFileList";
                using var key = Registry.CurrentUser.OpenSubKey(wmpRecentKey, true);
                if (key != null)
                {
                    var valueNames = key.GetValueNames();
                    foreach (var valueName in valueNames)
                    {
                        try
                        {
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete registry value: {wmpRecentKey}\\{valueName}");
                            }
                            else
                            {
                                key.DeleteValue(valueName);
                            }
                            clearedCount++;
                        }
                        catch { }
                    }
                }
            }
            catch { }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"  ✅ {(_dryRun ? "Would clear" : "Cleared")} {clearedCount} application cache items ({FormatBytes(freedSpace)} freed)");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  ❌ Failed: {ex.Message}");
            Console.ResetColor();
        }
    }

    static void ClearOutlookWebView()
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("\n🔹 Clearing Outlook WebView caches...");
        Console.ResetColor();

        long freedSpace = 0;
        int clearedCount = 0;

        try
        {
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            LogDryRun("\n=== OUTLOOK WEBVIEW ===");

            // Outlook uses Edge WebView2 for rendering
            var outlookWebViewPaths = new[]
            {
                Path.Combine(localAppData, @"Microsoft\Olk\EBWebView"),
                Path.Combine(localAppData, @"Microsoft\Outlook\WebView"),
                Path.Combine(localAppData, @"Microsoft\Office\16.0\WebView")
            };

            foreach (var basePath in outlookWebViewPaths)
            {
                if (Directory.Exists(basePath))
                {
                    try
                    {
                        // Find all profiles (Default, Profile 1, etc.)
                        var profiles = new List<string>();
                        
                        var defaultProfile = Path.Combine(basePath, "Default");
                        if (Directory.Exists(defaultProfile))
                        {
                            profiles.Add(defaultProfile);
                            var numberedProfiles = Directory.GetDirectories(basePath, "Profile *");
                            profiles.AddRange(numberedProfiles);
                        }
                        else
                        {
                            profiles.Add(basePath);
                        }

                        foreach (var profilePath in profiles)
                        {
                            // Clear cache folders (preserve authentication)
                            var cacheFolders = new[] { "Cache", "Code Cache", "GPUCache", "Service Worker", "DawnCache", "CacheStorage" };
                            
                            foreach (var folder in cacheFolders)
                            {
                                var folderPath = Path.Combine(profilePath, folder);
                                if (Directory.Exists(folderPath))
                                {
                                    try
                                    {
                                        var size = GetDirectorySize(folderPath);
                                        freedSpace += size;
                                        
                                        if (_dryRun)
                                        {
                                            LogDryRun($"Would delete: {folderPath} ({FormatBytes(size)})");
                                        }
                                        else
                                        {
                                            Directory.Delete(folderPath, true);
                                            Directory.CreateDirectory(folderPath); // Recreate empty
                                        }
                                        clearedCount++;
                                    }
                                    catch { }
                                }
                            }

                            // Clear logs
                            var logsPath = Path.Combine(profilePath, "Logs");
                            if (Directory.Exists(logsPath))
                            {
                                try
                                {
                                    var size = GetDirectorySize(logsPath);
                                    freedSpace += size;
                                    
                                    if (_dryRun)
                                    {
                                        LogDryRun($"Would delete: {logsPath} ({FormatBytes(size)})");
                                    }
                                    else
                                    {
                                        Directory.Delete(logsPath, true);
                                    }
                                    clearedCount++;
                                }
                                catch { }
                            }
                        }
                    }
                    catch { }
                }
            }

            if (clearedCount > 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"  ✅ {(_dryRun ? "Would clear" : "Cleared")} {clearedCount} Outlook WebView cache items ({FormatBytes(freedSpace)} {(_dryRun ? "would be freed" : "freed")})");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("  ⚠️ No Outlook WebView caches found");
                Console.ResetColor();
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  ❌ Failed: {ex.Message}");
            Console.ResetColor();
        }
    }

    static void ClearEmbeddedWebViews()
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("\n🔹 Clearing embedded WebView caches (Teams, Edge WebView2, etc.)...");
        Console.ResetColor();

        long freedSpace = 0;
        int clearedCount = 0;

        try
        {
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            LogDryRun("\n=== EMBEDDED WEBVIEWS ===");

            // Microsoft Teams WebView
            var teamsWebViewPath = Path.Combine(localAppData, @"Microsoft\Teams\EBWebView");
            if (Directory.Exists(teamsWebViewPath))
            {
                try
                {
                    var size = GetDirectorySize(teamsWebViewPath);
                    
                    // Clear cache folders but preserve authentication
                    var cacheFolders = new[] { "Default\\Cache", "Default\\Code Cache", "Default\\GPUCache", 
                        "Default\\DawnCache", "Default\\CacheStorage", "Default\\Service Worker" };
                    
                    foreach (var folder in cacheFolders)
                    {
                        var folderPath = Path.Combine(teamsWebViewPath, folder);
                        if (Directory.Exists(folderPath))
                        {
                            try
                            {
                                var folderSize = GetDirectorySize(folderPath);
                                freedSpace += folderSize;
                                
                                if (_dryRun)
                                {
                                    LogDryRun($"Would delete: {folderPath} ({FormatBytes(folderSize)})");
                                }
                                else
                                {
                                    Directory.Delete(folderPath, true);
                                    Directory.CreateDirectory(folderPath);
                                }
                                clearedCount++;
                            }
                            catch { }
                        }
                    }
                }
                catch { }
            }

            // Edge WebView2 Runtime cache
            var edgeWebView2Path = Path.Combine(localAppData, @"Microsoft\EdgeWebView");
            if (Directory.Exists(edgeWebView2Path))
            {
                try
                {
                    // Clear logs and temp files
                    var files = Directory.GetFiles(edgeWebView2Path, "*.log", SearchOption.AllDirectories)
                        .Concat(Directory.GetFiles(edgeWebView2Path, "*.tmp", SearchOption.AllDirectories));
                    
                    foreach (var file in files)
                    {
                        try
                        {
                            var fi = new FileInfo(file);
                            freedSpace += fi.Length;
                            
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete: {file} ({FormatBytes(fi.Length)})");
                            }
                            else
                            {
                                fi.Delete();
                            }
                            clearedCount++;
                        }
                        catch { }
                    }
                }
                catch { }
            }

            // Generic WebView2 search (other apps using WebView2)
            try
            {
                if (Directory.Exists(localAppData))
                {
                    var webViewDirs = Directory.GetDirectories(localAppData, "EBWebView", SearchOption.AllDirectories)
                        .Concat(Directory.GetDirectories(localAppData, "WebView2", SearchOption.AllDirectories))
                        .Where(d => !d.Contains("Outlook") && !d.Contains("Teams")); // Already handled above
                    
                    foreach (var webViewDir in webViewDirs)
                    {
                        try
                        {
                            // Look for cache subdirectories
                            var cacheDirs = new[] { "Cache", "Code Cache", "GPUCache", "DawnCache", "CacheStorage" };
                            
                            foreach (var cacheDir in cacheDirs)
                            {
                                var cachePath = Path.Combine(webViewDir, "Default", cacheDir);
                                if (Directory.Exists(cachePath))
                                {
                                    try
                                    {
                                        var size = GetDirectorySize(cachePath);
                                        freedSpace += size;
                                        
                                        if (_dryRun)
                                        {
                                            LogDryRun($"Would delete: {cachePath} ({FormatBytes(size)})");
                                        }
                                        else
                                        {
                                            Directory.Delete(cachePath, true);
                                            Directory.CreateDirectory(cachePath);
                                        }
                                        clearedCount++;
                                    }
                                    catch { }
                                }
                            }
                        }
                        catch { }
                    }
                }
            }
            catch { }

            // Office WebView caches (Word, Excel, PowerPoint online/embedded)
            var officeWebViewPaths = new[]
            {
                Path.Combine(localAppData, @"Microsoft\Office\16.0\Wef"),
                Path.Combine(localAppData, @"Microsoft\Office\16.0\WebView"),
                Path.Combine(localAppData, @"Microsoft\Office\WebView")
            };

            foreach (var path in officeWebViewPaths)
            {
                if (Directory.Exists(path))
                {
                    try
                    {
                        var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories)
                            .Where(f => f.EndsWith(".log", StringComparison.OrdinalIgnoreCase) ||
                                       f.EndsWith(".tmp", StringComparison.OrdinalIgnoreCase) ||
                                       f.Contains("Cache"));
                        
                        foreach (var file in files)
                        {
                            try
                            {
                                var fi = new FileInfo(file);
                                freedSpace += fi.Length;
                                
                                if (_dryRun)
                                {
                                    LogDryRun($"Would delete: {file} ({FormatBytes(fi.Length)})");
                                }
                                else
                                {
                                    fi.Delete();
                                }
                                clearedCount++;
                            }
                            catch { }
                        }
                    }
                    catch { }
                }
            }

            if (clearedCount > 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"  ✅ {(_dryRun ? "Would clear" : "Cleared")} {clearedCount} embedded WebView cache items ({FormatBytes(freedSpace)} {(_dryRun ? "would be freed" : "freed")})");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("  ⚠️ No embedded WebView caches found");
                Console.ResetColor();
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  ❌ Failed: {ex.Message}");
            Console.ResetColor();
        }
    }

    static void ClearAllDetectedAppData()
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("\n🔹 Clearing detected application logs/cache/history...");
        Console.ResetColor();

        long freedSpace = 0;
        int clearedCount = 0;

        try
        {
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            // Common app data patterns to clear (cache, logs, temp, history)
            var appFoldersToScan = new[]
            {
                localAppData,
                appData,
                Path.Combine(localAppData, "Packages")
            };

            var patternsToDelete = new[] { 
                "cache", "caches", "logs", "log", "tmp", "temp", "temps", "temporary",
                "history", "diagnostic", "diagnostics", "crashpad", "crash", "dump", "dumps",
                "telemetry", "analytics", "blob_storage", "gpucache", "gpu cache"
            };

            foreach (var baseFolder in appFoldersToScan)
            {
                if (!Directory.Exists(baseFolder)) continue;

                try
                {
                    var subdirs = Directory.GetDirectories(baseFolder);
                    foreach (var appFolder in subdirs)
                    {
                        try
                        {
                            var appName = Path.GetFileName(appFolder);
                            
                            // Skip system and Windows folders
                            if (appName.StartsWith("Microsoft.Windows", StringComparison.OrdinalIgnoreCase) ||
                                appName.Equals("Microsoft", StringComparison.OrdinalIgnoreCase) ||
                                appName.Equals("Packages", StringComparison.OrdinalIgnoreCase))
                                continue;

                            // Skip symlinks and junction points that might cause access issues
                            try
                            {
                                var dirInfo = new DirectoryInfo(appFolder);
                                if (dirInfo.Attributes.HasFlag(FileAttributes.ReparsePoint))
                                    continue;
                            }
                            catch { continue; }

                            // Look for cache/log/temp folders within each app folder
                            if (Directory.Exists(appFolder))
                            {
                                var appSubDirs = Directory.GetDirectories(appFolder, "*", SearchOption.AllDirectories);
                                foreach (var subDir in appSubDirs)
                                {
                                    var folderName = Path.GetFileName(subDir).ToLowerInvariant();
                                    
                                    // Skip authentication and sensitive data folders
                                    var authFolders = new[] { "local storage", "session storage", "indexeddb", "databases", "cookies" };
                                    if (authFolders.Any(auth => folderName.Contains(auth)))
                                        continue;
                                    
                                    if (patternsToDelete.Any(pattern => folderName.Contains(pattern)))
                                    {
                                        try
                                        {
                                            var size = GetDirectorySize(subDir);
                                            
                                            if (_dryRun)
                                            {
                                                LogDryRun($"Would delete directory: {subDir} ({FormatBytes(size)})");
                                            }
                                            else
                                            {
                                                Directory.Delete(subDir, true);
                                            }
                                            freedSpace += size;
                                            clearedCount++;
                                        }
                                        catch { /* Skip if in use or access denied */ }
                                    }
                                }

                                // Also clear individual log/cache files in app root
                                try
                                {
                                    var files = Directory.GetFiles(appFolder, "*.log", SearchOption.TopDirectoryOnly)
                                        .Concat(Directory.GetFiles(appFolder, "*.tmp", SearchOption.TopDirectoryOnly))
                                        .Concat(Directory.GetFiles(appFolder, "*.cache", SearchOption.TopDirectoryOnly));

                                    foreach (var file in files)
                                    {
                                        try
                                        {
                                            var fileInfo = new FileInfo(file);
                                            freedSpace += fileInfo.Length;

                                            if (_dryRun)
                                            {
                                                LogDryRun($"Would delete file: {file}");
                                            }
                                            else
                                            {
                                                fileInfo.Delete();
                                            }
                                            clearedCount++;
                                        }
                                        catch { }
                                    }
                                }
                                catch { }
                            }
                        }
                        catch { /* Skip problematic app folders */ }
                    }
                }
                catch { /* Skip if can't enumerate */ }
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"  ✅ {(_dryRun ? "Would clear" : "Cleared")} {clearedCount} app data items ({FormatBytes(freedSpace)} freed)");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  ❌ Failed: {ex.Message}");
            Console.ResetColor();
        }
    }

    static void ClearWindowsSandbox()
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("\n🔹 Clearing Windows Sandbox data...");
        Console.ResetColor();

        long freedSpace = 0;
        int clearedCount = 0;

        try
        {
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            
            // Windows Sandbox container data
            var sandboxPaths = new[]
            {
                Path.Combine(localAppData, "Packages", "Microsoft.Windows.WindowsSandbox_8wekyb3d8bbwe", "LocalCache"),
                Path.Combine(localAppData, "Packages", "Microsoft.Windows.WindowsSandbox_8wekyb3d8bbwe", "TempState"),
                Path.Combine(localAppData, "Packages", "Microsoft.Windows.WindowsSandbox_8wekyb3d8bbwe", "AC", "Temp"),
                Path.Combine(localAppData, "Packages", "Microsoft.Windows.WindowsSandbox_8wekyb3d8bbwe", "LocalState", "Logs"),
                Path.Combine(localAppData, "Packages", "MicrosoftWindows.WindowsSandbox_cw5n1h2txyewy", "LocalCache"),
                Path.Combine(localAppData, "Packages", "MicrosoftWindows.WindowsSandbox_cw5n1h2txyewy", "TempState"),
                Path.Combine(localAppData, "Packages", "MicrosoftWindows.WindowsSandbox_cw5n1h2txyewy", "AC", "Temp"),
                Path.Combine(localAppData, "Packages", "MicrosoftWindows.WindowsSandbox_cw5n1h2txyewy", "LocalState", "Logs")
            };

            foreach (var sandboxPath in sandboxPaths)
            {
                if (Directory.Exists(sandboxPath))
                {
                    try
                    {
                        var size = GetDirectorySize(sandboxPath);
                        
                        if (_dryRun)
                        {
                            LogDryRun($"Would delete directory: {sandboxPath} ({FormatBytes(size)})");
                        }
                        else
                        {
                            Directory.Delete(sandboxPath, true);
                        }
                        freedSpace += size;
                        clearedCount++;
                    }
                    catch { }
                }
            }

            // Clear Windows Sandbox VHD files (stored containers)
            var sandboxVhdPaths = new[]
            {
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Packages", "Microsoft.Windows.WindowsSandbox_8wekyb3d8bbwe", "LocalState"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Packages", "MicrosoftWindows.WindowsSandbox_cw5n1h2txyewy", "LocalState")
            };

            foreach (var vhdPath in sandboxVhdPaths)
            {
                if (Directory.Exists(vhdPath))
                {
                    try
                    {
                        var vhdFiles = Directory.GetFiles(vhdPath, "*.vhdx", SearchOption.TopDirectoryOnly)
                            .Concat(Directory.GetFiles(vhdPath, "*.vhd", SearchOption.TopDirectoryOnly));

                        foreach (var vhdFile in vhdFiles)
                        {
                            try
                            {
                                var fileInfo = new FileInfo(vhdFile);
                                freedSpace += fileInfo.Length;

                                if (_dryRun)
                                {
                                    LogDryRun($"Would delete file: {vhdFile} ({FormatBytes(fileInfo.Length)})");
                                }
                                else
                                {
                                    fileInfo.Delete();
                                }
                                clearedCount++;
                            }
                            catch { }
                        }
                    }
                    catch { }
                }
            }

            if (clearedCount > 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"  ✅ {(_dryRun ? "Would clear" : "Cleared")} {clearedCount} Windows Sandbox items ({FormatBytes(freedSpace)} freed)");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("  ⚠️ No Windows Sandbox data to clear");
                Console.ResetColor();
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  ❌ Failed: {ex.Message}");
            Console.ResetColor();
        }
    }

    static void ClearSpecificInstalledApps()
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("\n🔹 Clearing specific installed application data...");
        Console.ResetColor();

        LogDryRun("\n=== SPECIFIC INSTALLED APPS ===");

        long freedSpace = 0;
        int clearedCount = 0;

        try
        {
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var programData = Environment.GetEnvironmentVariable("ProgramData") ?? @"C:\ProgramData";

            // 🎵 SPOTIFY - Comprehensive cleanup
            Console.WriteLine("  Cleaning Spotify...");
            var spotifyBasePath = Path.Combine(appData, "Spotify");
            if (Directory.Exists(spotifyBasePath))
            {
                var spotifyPaths = new[]
                {
                    Path.Combine(spotifyBasePath, "PersistentCache"),
                    Path.Combine(spotifyBasePath, "Data"),
                    Path.Combine(spotifyBasePath, "Browser"),
                    Path.Combine(spotifyBasePath, "Storage"),
                    Path.Combine(localAppData, "Spotify", "Data"),
                    Path.Combine(localAppData, "Spotify", "Storage")
                };

                foreach (var spotifyPath in spotifyPaths)
                {
                    if (Directory.Exists(spotifyPath))
                    {
                        try
                        {
                            var size = GetDirectorySize(spotifyPath);
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete directory: {spotifyPath} ({FormatBytes(size)})");
                            }
                            else
                            {
                                Directory.Delete(spotifyPath, true);
                            }
                            freedSpace += size;
                            clearedCount++;
                        }
                        catch { }
                    }
                }

                // Clear Spotify log files and temp files
                try
                {
                    var logFiles = Directory.GetFiles(spotifyBasePath, "*.log", SearchOption.AllDirectories);
                    var tmpFiles = Directory.GetFiles(spotifyBasePath, "*.tmp", SearchOption.AllDirectories);
                    
                    foreach (var file in logFiles.Concat(tmpFiles))
                    {
                        try
                        {
                            var fileInfo = new FileInfo(file);
                            freedSpace += fileInfo.Length;
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete file: {file}");
                            }
                            else
                            {
                                fileInfo.Delete();
                            }
                            clearedCount++;
                        }
                        catch { }
                    }
                }
                catch { }
            }

            // 💬 SLACK - Comprehensive cleanup
            Console.WriteLine("  Cleaning Slack...");
            var slackBasePath = Path.Combine(appData, "Slack");
            if (Directory.Exists(slackBasePath))
            {
                // Removed IndexedDB, Local Storage, Session Storage - contain auth tokens
                var slackPaths = new[]
                {
                    Path.Combine(slackBasePath, "Cache"),
                    Path.Combine(slackBasePath, "Code Cache"),
                    Path.Combine(slackBasePath, "GPUCache"),
                    Path.Combine(slackBasePath, "Service Worker", "CacheStorage"),
                    Path.Combine(slackBasePath, "Service Worker", "ScriptCache"),
                    Path.Combine(slackBasePath, "logs"),
                    Path.Combine(slackBasePath, "crashDumps")
                };

                foreach (var slackPath in slackPaths)
                {
                    if (Directory.Exists(slackPath))
                    {
                        try
                        {
                            var size = GetDirectorySize(slackPath);
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete directory: {slackPath} ({FormatBytes(size)})");
                            }
                            else
                            {
                                Directory.Delete(slackPath, true);
                            }
                            freedSpace += size;
                            clearedCount++;
                        }
                        catch { }
                    }
                }

                // Clear Slack temp and log files
                try
                {
                    var tempFiles = Directory.GetFiles(slackBasePath, "*.tmp", SearchOption.AllDirectories);
                    var logFiles = Directory.GetFiles(slackBasePath, "*.log*", SearchOption.TopDirectoryOnly);
                    
                    foreach (var file in tempFiles.Concat(logFiles))
                    {
                        try
                        {
                            var fileInfo = new FileInfo(file);
                            freedSpace += fileInfo.Length;
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete file: {file}");
                            }
                            else
                            {
                                fileInfo.Delete();
                            }
                            clearedCount++;
                        }
                        catch { }
                    }
                }
                catch { }
            }

            // 📹 ZOOM - Comprehensive cleanup
            Console.WriteLine("  Cleaning Zoom...");
            var zoomBasePath = Path.Combine(appData, "Zoom");
            if (Directory.Exists(zoomBasePath))
            {
                var zoomPaths = new[]
                {
                    Path.Combine(zoomBasePath, "logs"),
                    Path.Combine(zoomBasePath, "data", "VDI"),
                    Path.Combine(zoomBasePath, "data", "cache"),
                    Path.Combine(zoomBasePath, "CachedApps")
                };

                foreach (var zoomPath in zoomPaths)
                {
                    if (Directory.Exists(zoomPath))
                    {
                        try
                        {
                            var size = GetDirectorySize(zoomPath);
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete directory: {zoomPath} ({FormatBytes(size)})");
                            }
                            else
                            {
                                Directory.Delete(zoomPath, true);
                            }
                            freedSpace += size;
                            clearedCount++;
                        }
                        catch { }
                    }
                }

                // Clear Zoom temp and log files
                try
                {
                    var tempFiles = Directory.GetFiles(zoomBasePath, "*.tmp", SearchOption.AllDirectories);
                    var logFiles = Directory.GetFiles(zoomBasePath, "*.log", SearchOption.AllDirectories);
                    
                    foreach (var file in tempFiles.Concat(logFiles))
                    {
                        try
                        {
                            var fileInfo = new FileInfo(file);
                            freedSpace += fileInfo.Length;
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete file: {file}");
                            }
                            else
                            {
                                fileInfo.Delete();
                            }
                            clearedCount++;
                        }
                        catch { }
                    }
                }
                catch { }
            }

            // 🖱️ LOGITECH OPTIONS / LOGI TUNE - Comprehensive cleanup
            Console.WriteLine("  Cleaning Logitech Options/Logi Tune...");
            var logiPaths = new[]
            {
                Path.Combine(appData, "LogiOptionsPlus"),
                Path.Combine(appData, "Logitech", "LogiOptions"),
                Path.Combine(localAppData, "Logitech", "LogiOptions"),
                Path.Combine(localAppData, "LogiOptionsPlus"),
                Path.Combine(appData, "Logi", "LogiTune"),
                Path.Combine(localAppData, "Logi", "LogiTune"),
                Path.Combine(programData, "Logishrd"),
                Path.Combine(programData, "Logitech"),
            };

            foreach (var logiBasePath in logiPaths)
            {
                if (Directory.Exists(logiBasePath))
                {
                    // Clear logs
                    var logiLogPaths = new[]
                    {
                        Path.Combine(logiBasePath, "Logs"),
                        Path.Combine(logiBasePath, "logs"),
                        Path.Combine(logiBasePath, "crash_reports"),
                        Path.Combine(logiBasePath, "CrashReports"),
                    };

                    foreach (var logPath in logiLogPaths)
                    {
                        if (Directory.Exists(logPath))
                        {
                            try
                            {
                                var size = GetDirectorySize(logPath);
                                if (_dryRun)
                                {
                                    LogDryRun($"Would delete directory: {logPath} ({FormatBytes(size)})");
                                }
                                else
                                {
                                    Directory.Delete(logPath, true);
                                }
                                freedSpace += size;
                                clearedCount++;
                            }
                            catch { }
                        }
                    }

                    // Clear cache directories
                    var logiCachePaths = new[]
                    {
                        Path.Combine(logiBasePath, "Cache"),
                        Path.Combine(logiBasePath, "cache"),
                        Path.Combine(logiBasePath, "Code Cache"),
                        Path.Combine(logiBasePath, "GPUCache"),
                        Path.Combine(logiBasePath, "Temp"),
                        Path.Combine(logiBasePath, "temp"),
                    };

                    foreach (var cachePath in logiCachePaths)
                    {
                        if (Directory.Exists(cachePath))
                        {
                            try
                            {
                                var size = GetDirectorySize(cachePath);
                                if (_dryRun)
                                {
                                    LogDryRun($"Would delete directory: {cachePath} ({FormatBytes(size)})");
                                }
                                else
                                {
                                    Directory.Delete(cachePath, true);
                                }
                                freedSpace += size;
                                clearedCount++;
                            }
                            catch { }
                        }
                    }

                    // Clear temp and log files
                    try
                    {
                        var tempFiles = Directory.GetFiles(logiBasePath, "*.tmp", SearchOption.AllDirectories);
                        var logFiles = Directory.GetFiles(logiBasePath, "*.log", SearchOption.AllDirectories);
                        var dmpFiles = Directory.GetFiles(logiBasePath, "*.dmp", SearchOption.AllDirectories);
                        
                        foreach (var file in tempFiles.Concat(logFiles).Concat(dmpFiles))
                        {
                            try
                            {
                                var fileInfo = new FileInfo(file);
                                freedSpace += fileInfo.Length;
                                if (_dryRun)
                                {
                                    LogDryRun($"Would delete file: {file}");
                                }
                                else
                                {
                                    fileInfo.Delete();
                                }
                                clearedCount++;
                            }
                            catch { }
                        }
                    }
                    catch { }
                }
            }

            // ✈️ TELEGRAM DESKTOP - Comprehensive cleanup
            Console.WriteLine("  Cleaning Telegram Desktop...");
            var telegramBasePath = Path.Combine(appData, "Telegram Desktop");
            if (Directory.Exists(telegramBasePath))
            {
                var telegramPaths = new[]
                {
                    Path.Combine(telegramBasePath, "tdata", "emoji"),
                    Path.Combine(telegramBasePath, "tdata", "user_data"),
                    Path.Combine(telegramBasePath, "tdata", "working"),
                    Path.Combine(telegramBasePath, "log.txt"),
                    Path.Combine(telegramBasePath, "log_start.txt"),
                    Path.Combine(telegramBasePath, "log_finish.txt")
                };

                foreach (var telegramPath in telegramPaths)
                {
                    if (File.Exists(telegramPath))
                    {
                        try
                        {
                            var fileInfo = new FileInfo(telegramPath);
                            freedSpace += fileInfo.Length;
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete file: {telegramPath}");
                            }
                            else
                            {
                                fileInfo.Delete();
                            }
                            clearedCount++;
                        }
                        catch { }
                    }
                    else if (Directory.Exists(telegramPath))
                    {
                        try
                        {
                            var size = GetDirectorySize(telegramPath);
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete directory: {telegramPath} ({FormatBytes(size)})");
                            }
                            else
                            {
                                Directory.Delete(telegramPath, true);
                            }
                            freedSpace += size;
                            clearedCount++;
                        }
                        catch { }
                    }
                }

                // Clear Telegram temp files
                try
                {
                    var tempFiles = Directory.GetFiles(telegramBasePath, "*.tmp", SearchOption.AllDirectories);
                    foreach (var file in tempFiles)
                    {
                        try
                        {
                            var fileInfo = new FileInfo(file);
                            freedSpace += fileInfo.Length;
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete file: {file}");
                            }
                            else
                            {
                                fileInfo.Delete();
                            }
                            clearedCount++;
                        }
                        catch { }
                    }
                }
                catch { }
            }

            // 📱 WHATSAPP DESKTOP - Comprehensive cleanup
            Console.WriteLine("  Cleaning WhatsApp Desktop...");
            var whatsappPackagePath = Path.Combine(localAppData, "Packages", "5319275A.WhatsAppDesktop_cv1g1gvanyjgm");
            if (Directory.Exists(whatsappPackagePath))
            {
                var whatsappPaths = new[]
                {
                    Path.Combine(whatsappPackagePath, "LocalCache"),
                    Path.Combine(whatsappPackagePath, "TempState"),
                    Path.Combine(whatsappPackagePath, "AC", "Temp"),
                    Path.Combine(whatsappPackagePath, "LocalState", "logs")
                };

                foreach (var whatsappPath in whatsappPaths)
                {
                    if (Directory.Exists(whatsappPath))
                    {
                        try
                        {
                            var size = GetDirectorySize(whatsappPath);
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete directory: {whatsappPath} ({FormatBytes(size)})");
                            }
                            else
                            {
                                Directory.Delete(whatsappPath, true);
                            }
                            freedSpace += size;
                            clearedCount++;
                        }
                        catch { }
                    }
                }
            }

            // 🔐 1PASSWORD - Comprehensive cleanup (SAFE - no vault deletion)
            Console.WriteLine("  Cleaning 1Password...");
            var onePasswordBasePath = Path.Combine(localAppData, "1Password");
            if (Directory.Exists(onePasswordBasePath))
            {
                var onePasswordPaths = new[]
                {
                    Path.Combine(onePasswordBasePath, "logs"),
                    Path.Combine(onePasswordBasePath, "cache"),
                    Path.Combine(onePasswordBasePath, "Crashpad"),
                    Path.Combine(onePasswordBasePath, "GPUCache"),
                    Path.Combine(onePasswordBasePath, "Code Cache")
                };

                foreach (var onePasswordPath in onePasswordPaths)
                {
                    if (Directory.Exists(onePasswordPath))
                    {
                        try
                        {
                            var size = GetDirectorySize(onePasswordPath);
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete directory: {onePasswordPath} ({FormatBytes(size)})");
                            }
                            else
                            {
                                Directory.Delete(onePasswordPath, true);
                            }
                            freedSpace += size;
                            clearedCount++;
                        }
                        catch { }
                    }
                }

                // Clear 1Password temp and log files (NOT vault files)
                try
                {
                    var tempFiles = Directory.GetFiles(onePasswordBasePath, "*.tmp", SearchOption.AllDirectories);
                    var logFiles = Directory.GetFiles(onePasswordBasePath, "*.log", SearchOption.TopDirectoryOnly);
                    
                    foreach (var file in tempFiles.Concat(logFiles))
                    {
                        try
                        {
                            var fileInfo = new FileInfo(file);
                            freedSpace += fileInfo.Length;
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete file: {file}");
                            }
                            else
                            {
                                fileInfo.Delete();
                            }
                            clearedCount++;
                        }
                        catch { }
                    }
                }
                catch { }
            }

            // 📝 NOTION - Comprehensive cleanup
            Console.WriteLine("  Cleaning Notion...");
            var notionBasePath = Path.Combine(appData, "Notion");
            if (Directory.Exists(notionBasePath))
            {
                // Removed IndexedDB, Local Storage, Session Storage - contain auth/data
                var notionPaths = new[]
                {
                    Path.Combine(notionBasePath, "Cache"),
                    Path.Combine(notionBasePath, "Code Cache"),
                    Path.Combine(notionBasePath, "GPUCache"),
                    Path.Combine(notionBasePath, "logs"),
                    Path.Combine(notionBasePath, "Crashpad"),
                    Path.Combine(notionBasePath, "Service Worker")
                };

                foreach (var notionPath in notionPaths)
                {
                    if (Directory.Exists(notionPath))
                    {
                        try
                        {
                            var size = GetDirectorySize(notionPath);
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete directory: {notionPath} ({FormatBytes(size)})");
                            }
                            else
                            {
                                Directory.Delete(notionPath, true);
                            }
                            freedSpace += size;
                            clearedCount++;
                        }
                        catch { }
                    }
                }

                // Clear Notion temp files
                try
                {
                    var tempFiles = Directory.GetFiles(notionBasePath, "*.tmp", SearchOption.AllDirectories);
                    foreach (var file in tempFiles)
                    {
                        try
                        {
                            var fileInfo = new FileInfo(file);
                            freedSpace += fileInfo.Length;
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete file: {file}");
                            }
                            else
                            {
                                fileInfo.Delete();
                            }
                            clearedCount++;
                        }
                        catch { }
                    }
                }
                catch { }
            }

            // ✅ TODOIST - Comprehensive cleanup
            Console.WriteLine("  Cleaning Todoist...");
            var todoistBasePath = Path.Combine(appData, "Todoist");
            if (Directory.Exists(todoistBasePath))
            {
                // Removed IndexedDB and Local Storage - contain auth tokens
                var todoistPaths = new[]
                {
                    Path.Combine(todoistBasePath, "Cache"),
                    Path.Combine(todoistBasePath, "Code Cache"),
                    Path.Combine(todoistBasePath, "GPUCache"),
                    Path.Combine(todoistBasePath, "logs"),
                    Path.Combine(todoistBasePath, "Crashpad"),
                    Path.Combine(todoistBasePath, "Service Worker")
                };

                foreach (var todoistPath in todoistPaths)
                {
                    if (Directory.Exists(todoistPath))
                    {
                        try
                        {
                            var size = GetDirectorySize(todoistPath);
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete directory: {todoistPath} ({FormatBytes(size)})");
                            }
                            else
                            {
                                Directory.Delete(todoistPath, true);
                            }
                            freedSpace += size;
                            clearedCount++;
                        }
                        catch { }
                    }
                }

                // Clear Todoist temp files
                try
                {
                    var tempFiles = Directory.GetFiles(todoistBasePath, "*.tmp", SearchOption.AllDirectories);
                    foreach (var file in tempFiles)
                    {
                        try
                        {
                            var fileInfo = new FileInfo(file);
                            freedSpace += fileInfo.Length;
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete file: {file}");
                            }
                            else
                            {
                                fileInfo.Delete();
                            }
                            clearedCount++;
                        }
                        catch { }
                    }
                }
                catch { }
            }

            // 📊 ASANA - Comprehensive cleanup
            Console.WriteLine("  Cleaning Asana...");
            var asanaBasePath = Path.Combine(appData, "Asana");
            if (Directory.Exists(asanaBasePath))
            {
                // Removed IndexedDB and Local Storage - contain auth tokens
                var asanaPaths = new[]
                {
                    Path.Combine(asanaBasePath, "Cache"),
                    Path.Combine(asanaBasePath, "Code Cache"),
                    Path.Combine(asanaBasePath, "GPUCache"),
                    Path.Combine(asanaBasePath, "logs"),
                    Path.Combine(asanaBasePath, "Crashpad"),
                    Path.Combine(asanaBasePath, "Service Worker")
                };

                foreach (var asanaPath in asanaPaths)
                {
                    if (Directory.Exists(asanaPath))
                    {
                        try
                        {
                            var size = GetDirectorySize(asanaPath);
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete directory: {asanaPath} ({FormatBytes(size)})");
                            }
                            else
                            {
                                Directory.Delete(asanaPath, true);
                            }
                            freedSpace += size;
                            clearedCount++;
                        }
                        catch { }
                    }
                }

                // Clear Asana temp files
                try
                {
                    var tempFiles = Directory.GetFiles(asanaBasePath, "*.tmp", SearchOption.AllDirectories);
                    foreach (var file in tempFiles)
                    {
                        try
                        {
                            var fileInfo = new FileInfo(file);
                            freedSpace += fileInfo.Length;
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete file: {file}");
                            }
                            else
                            {
                                fileInfo.Delete();
                            }
                            clearedCount++;
                        }
                        catch { }
                    }
                }
                catch { }
            }

            // 🐙 GITHUB DESKTOP - Comprehensive cleanup (preserves authentication)
            Console.WriteLine("  Cleaning GitHub Desktop...");
            var githubDesktopBasePaths = new[]
            {
                Path.Combine(appData, "GitHub Desktop"),
                Path.Combine(localAppData, "GitHubDesktop")
            };

            foreach (var githubDesktopBasePath in githubDesktopBasePaths)
            {
                if (Directory.Exists(githubDesktopBasePath))
                {
                    var githubDesktopPaths = new[]
                    {
                        Path.Combine(githubDesktopBasePath, "logs"),
                        Path.Combine(githubDesktopBasePath, "Cache"),
                        Path.Combine(githubDesktopBasePath, "Code Cache"),
                        Path.Combine(githubDesktopBasePath, "GPUCache"),
                        Path.Combine(githubDesktopBasePath, "Crashpad"),
                        Path.Combine(githubDesktopBasePath, "CrashReports"),
                        // Removed: IndexedDB, Session Storage, Local Storage, shared_proto_db (contain auth tokens)
                        Path.Combine(githubDesktopBasePath, "Service Worker"),
                        Path.Combine(githubDesktopBasePath, "blob_storage")
                    };

                    foreach (var githubPath in githubDesktopPaths)
                    {
                        if (Directory.Exists(githubPath))
                        {
                            try
                            {
                                var size = GetDirectorySize(githubPath);
                                if (_dryRun)
                                {
                                    LogDryRun($"Would delete directory: {githubPath} ({FormatBytes(size)})");
                                }
                                else
                                {
                                    Directory.Delete(githubPath, true);
                                }
                                freedSpace += size;
                                clearedCount++;
                            }
                            catch { }
                        }
                    }

                    // Clear GitHub Desktop temp and log files
                    try
                    {
                        var tempFiles = Directory.GetFiles(githubDesktopBasePath, "*.tmp", SearchOption.AllDirectories);
                        var logFiles = Directory.GetFiles(githubDesktopBasePath, "*.log", SearchOption.AllDirectories);
                        var dmpFiles = Directory.GetFiles(githubDesktopBasePath, "*.dmp", SearchOption.AllDirectories);
                        
                        foreach (var file in tempFiles.Concat(logFiles).Concat(dmpFiles))
                        {
                            try
                            {
                                var fileInfo = new FileInfo(file);
                                freedSpace += fileInfo.Length;
                                if (_dryRun)
                                {
                                    LogDryRun($"Would delete file: {file}");
                                }
                                else
                                {
                                    fileInfo.Delete();
                                }
                                clearedCount++;
                            }
                            catch { }
                        }
                    }
                    catch { }
                }
            }

            // 💬 SIGNAL DESKTOP - Comprehensive cleanup (preserves authentication)
            Console.WriteLine("  Cleaning Signal Desktop...");
            var signalBasePaths = new[]
            {
                Path.Combine(appData, "Signal"),
                Path.Combine(localAppData, "Programs", "signal-desktop")
            };

            foreach (var signalBasePath in signalBasePaths)
            {
                if (Directory.Exists(signalBasePath))
                {
                    // Removed: sql database (contains messages/auth), config.json (contains auth)
                    var signalPaths = new[]
                    {
                        Path.Combine(signalBasePath, "logs"),
                        Path.Combine(signalBasePath, "Cache"),
                        Path.Combine(signalBasePath, "Code Cache"),
                        Path.Combine(signalBasePath, "GPUCache"),
                        Path.Combine(signalBasePath, "Service Worker"),
                        Path.Combine(signalBasePath, "blob_storage"),
                        Path.Combine(signalBasePath, "Crashpad"),
                        Path.Combine(signalBasePath, "crash reports")
                    };

                    foreach (var signalPath in signalPaths)
                    {
                        if (Directory.Exists(signalPath))
                        {
                            try
                            {
                                var size = GetDirectorySize(signalPath);
                                if (_dryRun)
                                {
                                    LogDryRun($"Would delete directory: {signalPath} ({FormatBytes(size)})");
                                }
                                else
                                {
                                    Directory.Delete(signalPath, true);
                                }
                                freedSpace += size;
                                clearedCount++;
                            }
                            catch { }
                        }
                    }

                    // Clear Signal log files
                    try
                    {
                        var logFiles = Directory.GetFiles(signalBasePath, "*.log", SearchOption.TopDirectoryOnly);
                        foreach (var file in logFiles)
                        {
                            try
                            {
                                var fileInfo = new FileInfo(file);
                                freedSpace += fileInfo.Length;
                                if (_dryRun)
                                {
                                    LogDryRun($"Would delete file: {file}");
                                }
                                else
                                {
                                    fileInfo.Delete();
                                }
                                clearedCount++;
                            }
                            catch { }
                        }
                    }
                    catch { }
                }
            }

            // 💬 DISCORD - Comprehensive cleanup (preserves authentication)
            Console.WriteLine("  Cleaning Discord...");
            var discordBasePaths = new[]
            {
                Path.Combine(appData, "discord"),
                Path.Combine(appData, "discordcanary"),
                Path.Combine(appData, "discordptb")
            };

            foreach (var discordBasePath in discordBasePaths)
            {
                if (Directory.Exists(discordBasePath))
                {
                    // Removed: Local Storage (contains auth tokens)
                    var discordPaths = new[]
                    {
                        Path.Combine(discordBasePath, "Cache"),
                        Path.Combine(discordBasePath, "Code Cache"),
                        Path.Combine(discordBasePath, "GPUCache"),
                        Path.Combine(discordBasePath, "Service Worker"),
                        Path.Combine(discordBasePath, "blob_storage"),
                        Path.Combine(discordBasePath, "logs"),
                        Path.Combine(discordBasePath, "Crashpad"),
                        Path.Combine(discordBasePath, "crash reports")
                    };

                    foreach (var discordPath in discordPaths)
                    {
                        if (Directory.Exists(discordPath))
                        {
                            try
                            {
                                var size = GetDirectorySize(discordPath);
                                if (_dryRun)
                                {
                                    LogDryRun($"Would delete directory: {discordPath} ({FormatBytes(size)})");
                                }
                                else
                                {
                                    Directory.Delete(discordPath, true);
                                }
                                freedSpace += size;
                                clearedCount++;
                            }
                            catch { }
                        }
                    }
                }
            }

            // 🎮 STEAM - Comprehensive cleanup
            Console.WriteLine("  Cleaning Steam...");
            var steamBasePaths = new[]
            {
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Steam"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Steam"),
                @"C:\Program Files (x86)\Steam",
                @"C:\Steam",
                @"D:\Steam",
                @"E:\Steam"
            };

            foreach (var steamBasePath in steamBasePaths)
            {
                if (Directory.Exists(steamBasePath))
                {
                    var steamPaths = new[]
                    {
                        Path.Combine(steamBasePath, "logs"),
                        Path.Combine(steamBasePath, "appcache", "httpcache"),
                        Path.Combine(steamBasePath, "dumps"),
                        Path.Combine(steamBasePath, "crashhandler64.dmp"),
                        Path.Combine(steamBasePath, "config", "htmlcache"),
                        Path.Combine(steamBasePath, "config", "overlayhtmlcache"),
                        Path.Combine(steamBasePath, "steamui", "logs"),
                        Path.Combine(steamBasePath, "userdata", "crash_dumps"),
                    };

                    foreach (var steamPath in steamPaths)
                    {
                        if (File.Exists(steamPath))
                        {
                            try
                            {
                                var fileInfo = new FileInfo(steamPath);
                                freedSpace += fileInfo.Length;
                                if (_dryRun)
                                {
                                    LogDryRun($"Would delete file: {steamPath}");
                                }
                                else
                                {
                                    fileInfo.Delete();
                                }
                                clearedCount++;
                            }
                            catch { }
                        }
                        else if (Directory.Exists(steamPath))
                        {
                            try
                            {
                                var size = GetDirectorySize(steamPath);
                                if (_dryRun)
                                {
                                    LogDryRun($"Would delete directory: {steamPath} ({FormatBytes(size)})");
                                }
                                else
                                {
                                    Directory.Delete(steamPath, true);
                                }
                                freedSpace += size;
                                clearedCount++;
                            }
                            catch { }
                        }
                    }

                    // Clear old log files
                    try
                    {
                        var logFiles = Directory.GetFiles(steamBasePath, "*.log", SearchOption.TopDirectoryOnly);
                        var dmpFiles = Directory.GetFiles(steamBasePath, "*.dmp", SearchOption.TopDirectoryOnly);
                        var mdmpFiles = Directory.GetFiles(steamBasePath, "*.mdmp", SearchOption.TopDirectoryOnly);
                        
                        foreach (var file in logFiles.Concat(dmpFiles).Concat(mdmpFiles))
                        {
                            try
                            {
                                var fileInfo = new FileInfo(file);
                                freedSpace += fileInfo.Length;
                                if (_dryRun)
                                {
                                    LogDryRun($"Would delete file: {file}");
                                }
                                else
                                {
                                    fileInfo.Delete();
                                }
                                clearedCount++;
                            }
                            catch { }
                        }
                    }
                    catch { }
                }
            }

            // Clear Steam AppData cache
            var steamLocalAppData = Path.Combine(localAppData, "Steam");
            if (Directory.Exists(steamLocalAppData))
            {
                var steamAppDataPaths = new[]
                {
                    Path.Combine(steamLocalAppData, "htmlcache"),
                    Path.Combine(steamLocalAppData, "logs")
                };

                foreach (var path in steamAppDataPaths)
                {
                    if (Directory.Exists(path))
                    {
                        try
                        {
                            var size = GetDirectorySize(path);
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete directory: {path} ({FormatBytes(size)})");
                            }
                            else
                            {
                                Directory.Delete(path, true);
                            }
                            freedSpace += size;
                            clearedCount++;
                        }
                        catch { }
                    }
                }
            }

            // 🐳 DOCKER - Comprehensive cleanup (logs, cache, unused images)
            Console.WriteLine("  Cleaning Docker...");
            var dockerBasePath = Path.Combine(localAppData, "Docker");
            if (Directory.Exists(dockerBasePath))
            {
                var dockerPaths = new[]
                {
                    Path.Combine(dockerBasePath, "log"),
                    Path.Combine(dockerBasePath, "wsl", "diagnostics"),
                    Path.Combine(dockerBasePath, "wsl", "docker-desktop", "tmp"),
                    Path.Combine(dockerBasePath, "wsl", "docker-desktop-data", "tmp"),
                    Path.Combine(appData, "Docker", "log.txt")
                };

                foreach (var dockerPath in dockerPaths)
                {
                    if (File.Exists(dockerPath))
                    {
                        try
                        {
                            var fileInfo = new FileInfo(dockerPath);
                            freedSpace += fileInfo.Length;
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete file: {dockerPath}");
                            }
                            else
                            {
                                fileInfo.Delete();
                            }
                            clearedCount++;
                        }
                        catch { }
                    }
                    else if (Directory.Exists(dockerPath))
                    {
                        try
                        {
                            var size = GetDirectorySize(dockerPath);
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete directory: {dockerPath} ({FormatBytes(size)})");
                            }
                            else
                            {
                                Directory.Delete(dockerPath, true);
                            }
                            freedSpace += size;
                            clearedCount++;
                        }
                        catch { }
                    }
                }

                // Clear Docker temp and log files
                try
                {
                    var logFiles = Directory.GetFiles(dockerBasePath, "*.log", SearchOption.AllDirectories);
                    foreach (var file in logFiles)
                    {
                        try
                        {
                            var fileInfo = new FileInfo(file);
                            freedSpace += fileInfo.Length;
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete file: {file}");
                            }
                            else
                            {
                                fileInfo.Delete();
                            }
                            clearedCount++;
                        }
                        catch { }
                    }
                }
                catch { }

                // Run Docker cleanup commands if Docker is running
                try
                {
                    var dockerInfo = new ProcessStartInfo
                    {
                        FileName = "docker",
                        Arguments = "info",
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    };

                    bool dockerRunning = false;
                    try
                    {
                        using var checkProcess = Process.Start(dockerInfo);
                        if (checkProcess != null)
                        {
                            checkProcess.WaitForExit(5000);
                            dockerRunning = checkProcess.ExitCode == 0;
                        }
                    }
                    catch { }

                    if (dockerRunning)
                    {
                        Console.WriteLine("    Docker is running - cleaning build cache and unused images...");

                        // Clean Docker build cache
                        var cleanCommands = new[]
                        {
                            ("system prune -f", "Prune stopped containers and networks"),
                            ("builder prune -f", "Prune build cache"),
                            ("image prune -f", "Prune dangling images")
                        };

                        foreach (var (args, description) in cleanCommands)
                        {
                            try
                            {
                                var cmd = new ProcessStartInfo
                                {
                                    FileName = "docker",
                                    Arguments = args,
                                    UseShellExecute = false,
                                    CreateNoWindow = true,
                                    RedirectStandardOutput = true,
                                    RedirectStandardError = true
                                };

                                if (_dryRun)
                                {
                                    LogDryRun($"Would run: docker {args} ({description})");
                                }
                                else
                                {
                                    using var process = Process.Start(cmd);
                                    if (process != null)
                                    {
                                        process.WaitForExit(30000); // 30 second timeout
                                        if (process.ExitCode == 0)
                                        {
                                            Console.WriteLine($"    ✅ {description}");
                                        }
                                    }
                                }
                            }
                            catch { }
                        }
                    }
                }
                catch { }
            }

            // 🎬 VLC MEDIA PLAYER - Comprehensive cleanup
            Console.WriteLine("  Cleaning VLC Media Player...");
            var vlcBasePaths = new[]
            {
                Path.Combine(appData, "vlc"),
                Path.Combine(localAppData, "VLC")
            };

            foreach (var vlcBasePath in vlcBasePaths)
            {
                if (Directory.Exists(vlcBasePath))
                {
                    var vlcPaths = new[]
                    {
                        Path.Combine(vlcBasePath, "logs"),
                        Path.Combine(vlcBasePath, "crash-dumps"),
                        Path.Combine(vlcBasePath, "ml.xspf"), // Media library cache
                        Path.Combine(vlcBasePath, "art"), // Album art cache
                        Path.Combine(vlcBasePath, "skins2", "cache")
                    };

                    foreach (var vlcPath in vlcPaths)
                    {
                        if (File.Exists(vlcPath))
                        {
                            try
                            {
                                var fileInfo = new FileInfo(vlcPath);
                                freedSpace += fileInfo.Length;
                                if (_dryRun)
                                {
                                    LogDryRun($"Would delete file: {vlcPath}");
                                }
                                else
                                {
                                    fileInfo.Delete();
                                }
                                clearedCount++;
                            }
                            catch { }
                        }
                        else if (Directory.Exists(vlcPath))
                        {
                            try
                            {
                                var size = GetDirectorySize(vlcPath);
                                if (_dryRun)
                                {
                                    LogDryRun($"Would delete directory: {vlcPath} ({FormatBytes(size)})");
                                }
                                else
                                {
                                    Directory.Delete(vlcPath, true);
                                }
                                freedSpace += size;
                                clearedCount++;
                            }
                            catch { }
                        }
                    }

                    // Clear VLC log and dump files
                    try
                    {
                        var logFiles = Directory.GetFiles(vlcBasePath, "*.log", SearchOption.AllDirectories);
                        var dmpFiles = Directory.GetFiles(vlcBasePath, "*.dmp", SearchOption.AllDirectories);
                        
                        foreach (var file in logFiles.Concat(dmpFiles))
                        {
                            try
                            {
                                var fileInfo = new FileInfo(file);
                                freedSpace += fileInfo.Length;
                                if (_dryRun)
                                {
                                    LogDryRun($"Would delete file: {file}");
                                }
                                else
                                {
                                    fileInfo.Delete();
                                }
                                clearedCount++;
                            }
                            catch { }
                        }
                    }
                    catch { }
                }
            }

            // 📦 WINDOWS SANDBOX - Comprehensive cleanup
            Console.WriteLine("  Cleaning Windows Sandbox...");
            var sandboxPaths = new[]
            {
                Path.Combine(localAppData, "Packages", "Microsoft.Windows.ContentDeliveryManager_cw5n1h2txyewy", "LocalState", "Assets"),
                Path.Combine(programData, "Microsoft", "Windows", "Containers"),
                @"C:\ProgramData\Microsoft\Windows\Containers\BaseImages",
                @"C:\ProgramData\Microsoft\Windows\Containers\Logs",
                @"C:\Windows\Logs\WindowsSandbox"
            };

            foreach (var sandboxPath in sandboxPaths)
            {
                if (Directory.Exists(sandboxPath))
                {
                    try
                    {
                        var size = GetDirectorySize(sandboxPath);
                        if (_dryRun)
                        {
                            LogDryRun($"Would delete directory: {sandboxPath} ({FormatBytes(size)})");
                        }
                        else
                        {
                            // Only delete log/cache files, not base images
                            if (sandboxPath.Contains("Logs", StringComparison.OrdinalIgnoreCase) ||
                                sandboxPath.Contains("Assets", StringComparison.OrdinalIgnoreCase))
                            {
                                Directory.Delete(sandboxPath, true);
                            }
                            else
                            {
                                // Clear only .log and .etl files from container directories
                                var files = Directory.GetFiles(sandboxPath, "*.log", SearchOption.AllDirectories)
                                    .Concat(Directory.GetFiles(sandboxPath, "*.etl", SearchOption.AllDirectories));
                                
                                foreach (var file in files)
                                {
                                    try { File.Delete(file); } catch { }
                                }
                            }
                        }
                        freedSpace += size;
                        clearedCount++;
                    }
                    catch { }
                }
            }

            // 🔥 WINDOWS FIREWALL - Logs and traces
            Console.WriteLine("  Cleaning Windows Firewall logs...");
            var firewallLogPaths = new[]
            {
                @"C:\Windows\System32\LogFiles\Firewall\pfirewall.log",
                @"C:\Windows\System32\LogFiles\Firewall\pfirewall.log.old",
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "LogFiles", "Firewall")
            };

            foreach (var firewallPath in firewallLogPaths)
            {
                if (File.Exists(firewallPath))
                {
                    try
                    {
                        var fileInfo = new FileInfo(firewallPath);
                        freedSpace += fileInfo.Length;
                        if (_dryRun)
                        {
                            LogDryRun($"Would delete file: {firewallPath} ({FormatBytes(fileInfo.Length)})");
                        }
                        else
                        {
                            fileInfo.Delete();
                        }
                        clearedCount++;
                    }
                    catch { }
                }
                else if (Directory.Exists(firewallPath))
                {
                    try
                    {
                        var files = Directory.GetFiles(firewallPath, "*.log", SearchOption.AllDirectories);
                        foreach (var file in files)
                        {
                            try
                            {
                                var fileInfo = new FileInfo(file);
                                freedSpace += fileInfo.Length;
                                if (_dryRun)
                                {
                                    LogDryRun($"Would delete file: {file} ({FormatBytes(fileInfo.Length)})");
                                }
                                else
                                {
                                    fileInfo.Delete();
                                }
                                clearedCount++;
                            }
                            catch { }
                        }
                    }
                    catch { }
                }
            }

            // 💾 CRYSTALDISKMARK - Comprehensive cleanup
            Console.WriteLine("  Cleaning CrystalDiskMark...");
            var crystalDiskMarkPaths = new[]
            {
                Path.Combine(appData, "CrystalDiskMark"),
                Path.Combine(localAppData, "CrystalDiskMark")
            };

            foreach (var cdmPath in crystalDiskMarkPaths)
            {
                if (Directory.Exists(cdmPath))
                {
                    try
                    {
                        var logFiles = Directory.GetFiles(cdmPath, "*.log", SearchOption.AllDirectories);
                        var tmpFiles = Directory.GetFiles(cdmPath, "*.tmp", SearchOption.AllDirectories);
                        
                        foreach (var file in logFiles.Concat(tmpFiles))
                        {
                            try
                            {
                                var fileInfo = new FileInfo(file);
                                freedSpace += fileInfo.Length;
                                if (_dryRun)
                                {
                                    LogDryRun($"Would delete file: {file}");
                                }
                                else
                                {
                                    fileInfo.Delete();
                                }
                                clearedCount++;
                            }
                            catch { }
                        }
                    }
                    catch { }
                }
            }

            // 🔊 DOLBY ACCESS - Comprehensive cleanup
            Console.WriteLine("  Cleaning Dolby Access...");
            var dolbyPaths = new[]
            {
                Path.Combine(localAppData, "Packages", "DolbyLaboratories.DolbyAccess_rz1tebttyb220"),
                Path.Combine(localAppData, "Packages", "DolbyLaboratories.DolbyAtmos_rz1tebttyb220")
            };

            foreach (var dolbyPath in dolbyPaths)
            {
                if (Directory.Exists(dolbyPath))
                {
                    var dolbyCachePaths = new[]
                    {
                        Path.Combine(dolbyPath, "AC"),
                        Path.Combine(dolbyPath, "LocalCache"),
                        Path.Combine(dolbyPath, "TempState")
                    };

                    foreach (var cachePath in dolbyCachePaths)
                    {
                        if (Directory.Exists(cachePath))
                        {
                            try
                            {
                                var size = GetDirectorySize(cachePath);
                                if (_dryRun)
                                {
                                    LogDryRun($"Would delete directory: {cachePath} ({FormatBytes(size)})");
                                }
                                else
                                {
                                    Directory.Delete(cachePath, true);
                                }
                                freedSpace += size;
                                clearedCount++;
                            }
                            catch { }
                        }
                    }
                }
            }

            // 🔧 CMAKE - Cache and logs
            Console.WriteLine("  Cleaning CMake...");
            var cmakeCachePaths = new[]
            {
                Path.Combine(userProfile, ".cmake"),
                Path.Combine(localAppData, "CMake")
            };

            foreach (var cmakePath in cmakeCachePaths)
            {
                if (Directory.Exists(cmakePath))
                {
                    try
                    {
                        var logFiles = Directory.GetFiles(cmakePath, "*.log", SearchOption.AllDirectories);
                        foreach (var file in logFiles)
                        {
                            try
                            {
                                var fileInfo = new FileInfo(file);
                                freedSpace += fileInfo.Length;
                                if (_dryRun)
                                {
                                    LogDryRun($"Would delete file: {file}");
                                }
                                else
                                {
                                    fileInfo.Delete();
                                }
                                clearedCount++;
                            }
                            catch { }
                        }
                    }
                    catch { }
                }
            }

            // 💻 LENOVO COMMERCIAL VANTAGE - Comprehensive cleanup
            Console.WriteLine("  Cleaning Lenovo Commercial Vantage...");
            var lenovoPaths = new[]
            {
                Path.Combine(localAppData, "Packages", "E046963F.LenovoCompanion_k1h2ywk1493x8"),
                Path.Combine(localAppData, "Packages", "E046963F.LenovoSettingsforEnterprise_k1h2ywk1493x8"),
                Path.Combine(programData, "Lenovo"),
                Path.Combine(localAppData, "Lenovo")
            };

            foreach (var lenovoPath in lenovoPaths)
            {
                if (Directory.Exists(lenovoPath))
                {
                    var lenovoCachePaths = new[]
                    {
                        Path.Combine(lenovoPath, "Logs"),
                        Path.Combine(lenovoPath, "logs"),
                        Path.Combine(lenovoPath, "Cache"),
                        Path.Combine(lenovoPath, "cache"),
                        Path.Combine(lenovoPath, "AC"),
                        Path.Combine(lenovoPath, "TempState"),
                        Path.Combine(lenovoPath, "LocalCache")
                    };

                    foreach (var cachePath in lenovoCachePaths)
                    {
                        if (Directory.Exists(cachePath))
                        {
                            try
                            {
                                var size = GetDirectorySize(cachePath);
                                if (_dryRun)
                                {
                                    LogDryRun($"Would delete directory: {cachePath} ({FormatBytes(size)})");
                                }
                                else
                                {
                                    Directory.Delete(cachePath, true);
                                }
                                freedSpace += size;
                                clearedCount++;
                            }
                            catch { }
                        }
                    }
                }
            }

            // 🎥 CLIPCHAMP - Comprehensive cleanup
            Console.WriteLine("  Cleaning Clipchamp...");
            var clipchampPaths = new[]
            {
                Path.Combine(localAppData, "Packages", "Clipchamp.Clipchamp_yxz26nhyzhsrt"),
                Path.Combine(localAppData, "Packages", "Microsoft.Clipchamp_8wekyb3d8bbwe")
            };

            foreach (var clipchampPath in clipchampPaths)
            {
                if (Directory.Exists(clipchampPath))
                {
                    var clipchampCachePaths = new[]
                    {
                        Path.Combine(clipchampPath, "AC"),
                        Path.Combine(clipchampPath, "LocalCache"),
                        Path.Combine(clipchampPath, "TempState")
                    };

                    foreach (var cachePath in clipchampCachePaths)
                    {
                        if (Directory.Exists(cachePath))
                        {
                            try
                            {
                                var size = GetDirectorySize(cachePath);
                                if (_dryRun)
                                {
                                    LogDryRun($"Would delete directory: {cachePath} ({FormatBytes(size)})");
                                }
                                else
                                {
                                    Directory.Delete(cachePath, true);
                                }
                                freedSpace += size;
                                clearedCount++;
                            }
                            catch { }
                        }
                    }
                }
            }

            // 🗜️ NANAZIP - Comprehensive cleanup
            Console.WriteLine("  Cleaning NanaZip...");
            var nanazipPaths = new[]
            {
                Path.Combine(localAppData, "Packages", "40174MouriNaruto.NanaZip_gnj4mf6z9tkrc"),
                Path.Combine(localAppData, "Packages", "40174MouriNaruto.NanaZipPreview_gnj4mf6z9tkrc")
            };

            foreach (var nanazipPath in nanazipPaths)
            {
                if (Directory.Exists(nanazipPath))
                {
                    var nanazipCachePaths = new[]
                    {
                        Path.Combine(nanazipPath, "AC"),
                        Path.Combine(nanazipPath, "LocalCache"),
                        Path.Combine(nanazipPath, "TempState")
                    };

                    foreach (var cachePath in nanazipCachePaths)
                    {
                        if (Directory.Exists(cachePath))
                        {
                            try
                            {
                                var size = GetDirectorySize(cachePath);
                                if (_dryRun)
                                {
                                    LogDryRun($"Would delete directory: {cachePath} ({FormatBytes(size)})");
                                }
                                else
                                {
                                    Directory.Delete(cachePath, true);
                                }
                                freedSpace += size;
                                clearedCount++;
                            }
                            catch { }
                        }
                    }
                }
            }

            // 📁 7-ZIP - Logs and temp
            Console.WriteLine("  Cleaning 7-Zip...");
            var sevenZipPaths = new[]
            {
                Path.Combine(appData, "7-Zip"),
                Path.Combine(localAppData, "7-Zip")
            };

            foreach (var sevenZipPath in sevenZipPaths)
            {
                if (Directory.Exists(sevenZipPath))
                {
                    try
                    {
                        var files = Directory.GetFiles(sevenZipPath, "*.log", SearchOption.AllDirectories)
                            .Concat(Directory.GetFiles(sevenZipPath, "*.tmp", SearchOption.AllDirectories));

                        foreach (var file in files)
                        {
                            try
                            {
                                var fileInfo = new FileInfo(file);
                                freedSpace += fileInfo.Length;
                                if (_dryRun)
                                {
                                    LogDryRun($"Would delete file: {file}");
                                }
                                else
                                {
                                    fileInfo.Delete();
                                }
                                clearedCount++;
                            }
                            catch { }
                        }
                    }
                    catch { }
                }
            }

            // ⬇️ ARIA2 - Download manager logs
            Console.WriteLine("  Cleaning Aria2...");
            var aria2Paths = new[]
            {
                Path.Combine(userProfile, ".aria2"),
                Path.Combine(appData, "aria2"),
                Path.Combine(localAppData, "aria2")
            };

            foreach (var aria2Path in aria2Paths)
            {
                if (Directory.Exists(aria2Path))
                {
                    try
                    {
                        var logFiles = Directory.GetFiles(aria2Path, "*.log", SearchOption.AllDirectories);
                        foreach (var file in logFiles)
                        {
                            try
                            {
                                var fileInfo = new FileInfo(file);
                                freedSpace += fileInfo.Length;
                                if (_dryRun)
                                {
                                    LogDryRun($"Would delete file: {file}");
                                }
                                else
                                {
                                    fileInfo.Delete();
                                }
                                clearedCount++;
                            }
                            catch { }
                        }
                    }
                    catch { }
                }
            }

            // 👆 SYNAPTICS FINGERPRINT - Logs and cache
            Console.WriteLine("  Cleaning Synaptics Fingerprint Reader...");
            var synapticsPaths = new[]
            {
                Path.Combine(programData, "Synaptics"),
                Path.Combine(localAppData, "Synaptics")
            };

            foreach (var synapticsPath in synapticsPaths)
            {
                if (Directory.Exists(synapticsPath))
                {
                    var synapticsCachePaths = new[]
                    {
                        Path.Combine(synapticsPath, "Logs"),
                        Path.Combine(synapticsPath, "logs"),
                        Path.Combine(synapticsPath, "Cache")
                    };

                    foreach (var cachePath in synapticsCachePaths)
                    {
                        if (Directory.Exists(cachePath))
                        {
                            try
                            {
                                var size = GetDirectorySize(cachePath);
                                if (_dryRun)
                                {
                                    LogDryRun($"Would delete directory: {cachePath} ({FormatBytes(size)})");
                                }
                                else
                                {
                                    Directory.Delete(cachePath, true);
                                }
                                freedSpace += size;
                                clearedCount++;
                            }
                            catch { }
                        }
                    }
                }
            }

            // 💻 WINDOWS TERMINAL - All variants
            Console.WriteLine("  Cleaning Windows Terminal...");
            var terminalPaths = new[]
            {
                Path.Combine(localAppData, "Packages", "Microsoft.WindowsTerminal_8wekyb3d8bbwe"),
                Path.Combine(localAppData, "Packages", "Microsoft.WindowsTerminalPreview_8wekyb3d8bbwe"),
                Path.Combine(localAppData, "Packages", "Microsoft.WindowsTerminalCanary_8wekyb3d8bbwe")
            };

            foreach (var terminalPath in terminalPaths)
            {
                if (Directory.Exists(terminalPath))
                {
                    var terminalCachePaths = new[]
                    {
                        Path.Combine(terminalPath, "LocalState", "TabBackup"),
                        Path.Combine(terminalPath, "TempState"),
                        Path.Combine(terminalPath, "AC"),
                        Path.Combine(terminalPath, "LocalCache")
                    };

                    foreach (var cachePath in terminalCachePaths)
                    {
                        if (Directory.Exists(cachePath))
                        {
                            try
                            {
                                var size = GetDirectorySize(cachePath);
                                if (_dryRun)
                                {
                                    LogDryRun($"Would delete directory: {cachePath} ({FormatBytes(size)})");
                                }
                                else
                                {
                                    Directory.Delete(cachePath, true);
                                }
                                freedSpace += size;
                                clearedCount++;
                            }
                            catch { }
                        }
                    }
                }
            }

            // 💾 WINDOWS BACKUP - Logs
            Console.WriteLine("  Cleaning Windows Backup...");
            var windowsBackupLogPaths = new[]
            {
                @"C:\Windows\Logs\WindowsBackup",
                Path.Combine(localAppData, "Microsoft", "Windows", "Backup")
            };

            foreach (var backupPath in windowsBackupLogPaths)
            {
                if (Directory.Exists(backupPath))
                {
                    try
                    {
                        var logFiles = Directory.GetFiles(backupPath, "*.log", SearchOption.AllDirectories)
                            .Concat(Directory.GetFiles(backupPath, "*.etl", SearchOption.AllDirectories));

                        foreach (var file in logFiles)
                        {
                            try
                            {
                                var fileInfo = new FileInfo(file);
                                freedSpace += fileInfo.Length;
                                if (_dryRun)
                                {
                                    LogDryRun($"Would delete file: {file}");
                                }
                                else
                                {
                                    fileInfo.Delete();
                                }
                                clearedCount++;
                            }
                            catch { }
                        }
                    }
                    catch { }
                }
            }

            // ✂️ SNIPPING TOOL - Cache and logs
            Console.WriteLine("  Cleaning Snipping Tool...");
            
            // Only clean ScreenSketch package cache folders (NOT the CBS system package!)
            var screenSketchPath = Path.Combine(localAppData, "Packages", "Microsoft.ScreenSketch_8wekyb3d8bbwe");
            if (Directory.Exists(screenSketchPath))
            {
                var snipCachePaths = new[]
                {
                    Path.Combine(screenSketchPath, "AC"),
                    Path.Combine(screenSketchPath, "LocalCache"),
                    Path.Combine(screenSketchPath, "TempState")
                };

                foreach (var cachePath in snipCachePaths)
                {
                    if (Directory.Exists(cachePath))
                    {
                        try
                        {
                            var size = GetDirectorySize(cachePath);
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete directory: {cachePath} ({FormatBytes(size)})");
                            }
                            else
                            {
                                Directory.Delete(cachePath, true);
                            }
                            freedSpace += size;
                            clearedCount++;
                        }
                        catch { }
                    }
                }
            }
            
            // DO NOT TOUCH MicrosoftWindows.Client.CBS package - it's critical for Windows Shell and keyboard shortcuts!

            // 📷 CAMERA - Windows Camera app
            Console.WriteLine("  Cleaning Windows Camera...");
            
            // Clean Camera app package
            var cameraPackagePath = Path.Combine(localAppData, "Packages", "Microsoft.WindowsCamera_8wekyb3d8bbwe");
            if (Directory.Exists(cameraPackagePath))
            {
                var cameraCachePaths = new[]
                {
                    Path.Combine(cameraPackagePath, "AC"),
                    Path.Combine(cameraPackagePath, "LocalCache"),
                    Path.Combine(cameraPackagePath, "TempState")
                };

                foreach (var cachePath in cameraCachePaths)
                {
                    if (Directory.Exists(cachePath))
                    {
                        try
                        {
                            var size = GetDirectorySize(cachePath);
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete directory: {cachePath} ({FormatBytes(size)})");
                            }
                            else
                            {
                                Directory.Delete(cachePath, true);
                            }
                            freedSpace += size;
                            clearedCount++;
                        }
                        catch { }
                    }
                }
            }
            
            // Clean Face Analysis cache (specific subfolder only)
            var faceAnalysisPath = Path.Combine(localAppData, "Packages", "MicrosoftWindows.Client.CBS_cw5n1h2txyewy", "LocalState", "Microsoft.Media.FaceAnalysis");
            if (Directory.Exists(faceAnalysisPath))
            {
                try
                {
                    var size = GetDirectorySize(faceAnalysisPath);
                    if (_dryRun)
                    {
                        LogDryRun($"Would delete directory: {faceAnalysisPath} ({FormatBytes(size)})");
                    }
                    else
                    {
                        Directory.Delete(faceAnalysisPath, true);
                    }
                    freedSpace += size;
                    clearedCount++;
                }
                catch { }
            }

            // 📄 MICROSOFT OFFICE - All applications comprehensive cleanup
            Console.WriteLine("  Cleaning Microsoft Office applications...");
            var officeAppPaths = new[]
            {
                Path.Combine(localAppData, "Microsoft", "Office"),
                Path.Combine(localAppData, "Microsoft", "Word"),
                Path.Combine(localAppData, "Microsoft", "Excel"),
                Path.Combine(localAppData, "Microsoft", "PowerPoint"),
                Path.Combine(localAppData, "Microsoft", "Outlook"),
                Path.Combine(localAppData, "Microsoft", "OneNote"),
                Path.Combine(localAppData, "Microsoft", "Access"),
                Path.Combine(localAppData, "Microsoft", "Publisher"),
                Path.Combine(localAppData, "Microsoft", "Teams"),
                Path.Combine(appData, "Microsoft", "Templates")
            };

            foreach (var officePath in officeAppPaths)
            {
                if (Directory.Exists(officePath))
                {
                    try
                    {
                        // Clear cache and temp files
                        var logFiles = Directory.GetFiles(officePath, "*.log", SearchOption.AllDirectories);
                        var tmpFiles = Directory.GetFiles(officePath, "*.tmp", SearchOption.AllDirectories);
                        
                        foreach (var file in logFiles.Concat(tmpFiles))
                        {
                            try
                            {
                                var fileInfo = new FileInfo(file);
                                freedSpace += fileInfo.Length;
                                if (_dryRun)
                                {
                                    LogDryRun($"Would delete file: {file}");
                                }
                                else
                                {
                                    fileInfo.Delete();
                                }
                                clearedCount++;
                            }
                            catch { }
                        }
                    }
                    catch { }
                }
            }

            // 🐍 PYTHON PIP - Cache cleanup
            Console.WriteLine("  Cleaning Python pip cache...");
            var pipCachePath = Path.Combine(localAppData, "pip", "cache");
            if (Directory.Exists(pipCachePath))
            {
                try
                {
                    var size = GetDirectorySize(pipCachePath);
                    if (_dryRun)
                    {
                        LogDryRun($"Would delete directory: {pipCachePath} ({FormatBytes(size)})");
                    }
                    else
                    {
                        Directory.Delete(pipCachePath, true);
                    }
                    freedSpace += size;
                    clearedCount++;
                }
                catch { }
            }

            // 🦕 DENO - Cache cleanup
            Console.WriteLine("  Cleaning Deno cache...");
            var denoCachePath = Path.Combine(localAppData, "deno");
            if (Directory.Exists(denoCachePath))
            {
                var denoCacheFolders = new[] { "gen", "deps", "npm" };
                foreach (var folder in denoCacheFolders)
                {
                    var folderPath = Path.Combine(denoCachePath, folder);
                    if (Directory.Exists(folderPath))
                    {
                        try
                        {
                            var size = GetDirectorySize(folderPath);
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete directory: {folderPath} ({FormatBytes(size)})");
                            }
                            else
                            {
                                Directory.Delete(folderPath, true);
                            }
                            freedSpace += size;
                            clearedCount++;
                        }
                        catch { }
                    }
                }
            }

            // 🔒 PROTON VPN - Comprehensive cleanup
            Console.WriteLine("  Cleaning Proton VPN...");
            var protonVpnBasePath = Path.Combine(localAppData, "ProtonVPN");
            if (Directory.Exists(protonVpnBasePath))
            {
                var protonVpnPaths = new[]
                {
                    Path.Combine(protonVpnBasePath, "Logs"),
                    Path.Combine(protonVpnBasePath, "DiagnosticLogs"),
                    Path.Combine(protonVpnBasePath, "Cache"),
                    Path.Combine(protonVpnBasePath, "CrashReports"),
                    Path.Combine(protonVpnBasePath, "wireguard")
                };

                foreach (var protonPath in protonVpnPaths)
                {
                    if (Directory.Exists(protonPath))
                    {
                        try
                        {
                            var size = GetDirectorySize(protonPath);
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete directory: {protonPath} ({FormatBytes(size)})");
                            }
                            else
                            {
                                Directory.Delete(protonPath, true);
                            }
                            freedSpace += size;
                            clearedCount++;
                        }
                        catch { }
                    }
                }

                // Clear Proton VPN log and temp files
                try
                {
                    var logFiles = Directory.GetFiles(protonVpnBasePath, "*.log", SearchOption.TopDirectoryOnly);
                    var tempFiles = Directory.GetFiles(protonVpnBasePath, "*.tmp", SearchOption.AllDirectories);
                    
                    foreach (var file in logFiles.Concat(tempFiles))
                    {
                        try
                        {
                            var fileInfo = new FileInfo(file);
                            freedSpace += fileInfo.Length;
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete file: {file}");
                            }
                            else
                            {
                                fileInfo.Delete();
                            }
                            clearedCount++;
                        }
                        catch { }
                    }
                }
                catch { }
            }

            // Clear Proton VPN AppData Roaming location
            var protonVpnRoamingPath = Path.Combine(appData, "ProtonVPN");
            if (Directory.Exists(protonVpnRoamingPath))
            {
                var roamingCleanupPaths = new[]
                {
                    Path.Combine(protonVpnRoamingPath, "Logs"),
                    Path.Combine(protonVpnRoamingPath, "DiagnosticLogs"),
                    Path.Combine(protonVpnRoamingPath, "Cache")
                };

                foreach (var roamingPath in roamingCleanupPaths)
                {
                    if (Directory.Exists(roamingPath))
                    {
                        try
                        {
                            var size = GetDirectorySize(roamingPath);
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete directory: {roamingPath} ({FormatBytes(size)})");
                            }
                            else
                            {
                                Directory.Delete(roamingPath, true);
                            }
                            freedSpace += size;
                            clearedCount++;
                        }
                        catch { }
                    }
                }
            }

            // Clear Proton VPN registry traces
            try
            {
                var protonVpnRegistryKeys = new[]
                {
                    @"Software\ProtonVPN\RecentConnections",
                    @"Software\ProtonVPN\ConnectionHistory"
                };

                foreach (var regPath in protonVpnRegistryKeys)
                {
                    try
                    {
                        using var key = Registry.CurrentUser.OpenSubKey(regPath, true);
                        if (key != null)
                        {
                            var valueNames = key.GetValueNames();
                            foreach (var valueName in valueNames)
                            {
                                try
                                {
                                    if (_dryRun)
                                    {
                                        LogDryRun($"Would delete registry value: {regPath}\\{valueName}");
                                    }
                                    else
                                    {
                                        key.DeleteValue(valueName);
                                    }
                                    clearedCount++;
                                }
                                catch { }
                            }
                        }
                    }
                    catch { }
                }
            }
            catch { }

            // 🤖 CLAUDE CODE (Anthropic) - Comprehensive cleanup
            Console.WriteLine("  Cleaning Claude Code...");
            var claudePaths = new[]
            {
                Path.Combine(appData, "Claude"),
                Path.Combine(localAppData, "Claude"),
                Path.Combine(appData, "claude-desktop"),
                Path.Combine(localAppData, "claude-desktop")
            };

            foreach (var claudeBasePath in claudePaths)
            {
                if (Directory.Exists(claudeBasePath))
                {
                    var claudeCleanupPaths = new[]
                    {
                        Path.Combine(claudeBasePath, "Cache"),
                        Path.Combine(claudeBasePath, "Code Cache"),
                        Path.Combine(claudeBasePath, "GPUCache"),
                        Path.Combine(claudeBasePath, "logs"),
                        Path.Combine(claudeBasePath, "crashDumps"),
                        Path.Combine(claudeBasePath, "Crashpad")
                    };

                    foreach (var path in claudeCleanupPaths)
                    {
                        if (Directory.Exists(path))
                        {
                            try
                            {
                                var size = GetDirectorySize(path);
                                if (_dryRun)
                                {
                                    LogDryRun($"Would delete directory: {path} ({FormatBytes(size)})");
                                }
                                else
                                {
                                    Directory.Delete(path, true);
                                }
                                freedSpace += size;
                                clearedCount++;
                            }
                            catch { }
                        }
                    }

                    // Clean log files
                    try
                    {
                        var logFiles = Directory.GetFiles(claudeBasePath, "*.log", SearchOption.AllDirectories);
                        foreach (var file in logFiles)
                        {
                            try
                            {
                                var fi = new FileInfo(file);
                                freedSpace += fi.Length;
                                if (_dryRun)
                                {
                                    LogDryRun($"Would delete file: {file}");
                                }
                                else
                                {
                                    fi.Delete();
                                }
                                clearedCount++;
                            }
                            catch { }
                        }
                    }
                    catch { }
                }
            }

            // 🍎 ITUNES - Comprehensive cleanup
            Console.WriteLine("  Cleaning iTunes...");
            var itunesPaths = new[]
            {
                Path.Combine(appData, "Apple Computer", "iTunes"),
                Path.Combine(localAppData, "Apple Computer", "iTunes"),
                Path.Combine(programData, "Apple Computer", "iTunes")
            };

            foreach (var itunesBasePath in itunesPaths)
            {
                if (Directory.Exists(itunesBasePath))
                {
                    var itunesCleanupPaths = new[]
                    {
                        Path.Combine(itunesBasePath, "Cache"),
                        Path.Combine(itunesBasePath, "Logs")
                    };

                    foreach (var path in itunesCleanupPaths)
                    {
                        if (Directory.Exists(path))
                        {
                            try
                            {
                                var size = GetDirectorySize(path);
                                if (_dryRun)
                                {
                                    LogDryRun($"Would delete directory: {path} ({FormatBytes(size)})");
                                }
                                else
                                {
                                    Directory.Delete(path, true);
                                }
                                freedSpace += size;
                                clearedCount++;
                            }
                            catch { }
                        }
                    }

                    // Clean temp files and logs
                    try
                    {
                        var files = Directory.GetFiles(itunesBasePath, "*.tmp", SearchOption.AllDirectories)
                            .Concat(Directory.GetFiles(itunesBasePath, "*.log", SearchOption.AllDirectories));
                        
                        foreach (var file in files)
                        {
                            try
                            {
                                var fi = new FileInfo(file);
                                freedSpace += fi.Length;
                                if (_dryRun)
                                {
                                    LogDryRun($"Would delete file: {file}");
                                }
                                else
                                {
                                    fi.Delete();
                                }
                                clearedCount++;
                            }
                            catch { }
                        }
                    }
                    catch { }
                }
            }

            // 🎵 APPLE MUSIC - Comprehensive cleanup
            Console.WriteLine("  Cleaning Apple Music...");
            var appleMusicPaths = new[]
            {
                Path.Combine(localAppData, "Apple Music"),
                Path.Combine(appData, "Apple Music"),
                Path.Combine(localAppData, "Packages", "AppleInc.AppleMusic*")
            };

            foreach (var musicBasePath in appleMusicPaths)
            {
                if (musicBasePath.Contains("*"))
                {
                    var parentDir = Path.GetDirectoryName(musicBasePath);
                    if (Directory.Exists(parentDir))
                    {
                        var dirs = Directory.GetDirectories(parentDir, Path.GetFileName(musicBasePath));
                        foreach (var dir in dirs)
                        {
                            var cacheLocations = new[] { "LocalCache", "TempState", "AC\\Temp" };
                            foreach (var cacheDir in cacheLocations)
                            {
                                var cachePath = Path.Combine(dir, cacheDir);
                                if (Directory.Exists(cachePath))
                                {
                                    try
                                    {
                                        var size = GetDirectorySize(cachePath);
                                        if (_dryRun)
                                        {
                                            LogDryRun($"Would delete directory: {cachePath} ({FormatBytes(size)})");
                                        }
                                        else
                                        {
                                            Directory.Delete(cachePath, true);
                                        }
                                        freedSpace += size;
                                        clearedCount++;
                                    }
                                    catch { }
                                }
                            }
                        }
                    }
                }
                else if (Directory.Exists(musicBasePath))
                {
                    var musicCleanupPaths = new[]
                    {
                        Path.Combine(musicBasePath, "Cache"),
                        Path.Combine(musicBasePath, "Logs")
                    };

                    foreach (var path in musicCleanupPaths)
                    {
                        if (Directory.Exists(path))
                        {
                            try
                            {
                                var size = GetDirectorySize(path);
                                if (_dryRun)
                                {
                                    LogDryRun($"Would delete directory: {path} ({FormatBytes(size)})");
                                }
                                else
                                {
                                    Directory.Delete(path, true);
                                }
                                freedSpace += size;
                                clearedCount++;
                            }
                            catch { }
                        }
                    }
                }
            }

            // 🍎 APPLE TV, APPLE DEVICES - Comprehensive cleanup
            Console.WriteLine("  Cleaning other Apple apps...");
            var otherApplePaths = new[]
            {
                Path.Combine(localAppData, "Apple TV"),
                Path.Combine(appData, "Apple TV"),
                Path.Combine(localAppData, "Apple Devices"),
                Path.Combine(appData, "Apple Devices"),
                Path.Combine(localAppData, "Apple Inc"),
                Path.Combine(programData, "Apple", "Logs")
            };

            foreach (var appleBasePath in otherApplePaths)
            {
                if (Directory.Exists(appleBasePath))
                {
                    // Clean caches and logs
                    var cleanupSubdirs = new[] { "Cache", "Logs", "cache", "logs" };
                    foreach (var subdir in cleanupSubdirs)
                    {
                        var path = Path.Combine(appleBasePath, subdir);
                        if (Directory.Exists(path))
                        {
                            try
                            {
                                var size = GetDirectorySize(path);
                                if (_dryRun)
                                {
                                    LogDryRun($"Would delete directory: {path} ({FormatBytes(size)})");
                                }
                                else
                                {
                                    Directory.Delete(path, true);
                                }
                                freedSpace += size;
                                clearedCount++;
                            }
                            catch { }
                        }
                    }

                    // Clean log files
                    try
                    {
                        var logFiles = Directory.GetFiles(appleBasePath, "*.log", SearchOption.AllDirectories);
                        foreach (var file in logFiles)
                        {
                            try
                            {
                                var fi = new FileInfo(file);
                                freedSpace += fi.Length;
                                if (_dryRun)
                                {
                                    LogDryRun($"Would delete file: {file}");
                                }
                                else
                                {
                                    fi.Delete();
                                }
                                clearedCount++;
                            }
                            catch { }
                        }
                    }
                    catch { }
                }
            }

            // 🎨 ADOBE APPLICATIONS - Comprehensive cleanup
            Console.WriteLine("  Cleaning Adobe applications...");
            var adobeBasePaths = new[]
            {
                Path.Combine(localAppData, "Adobe"),
                Path.Combine(appData, "Adobe"),
                Path.Combine(programData, "Adobe")
            };

            foreach (var adobeBasePath in adobeBasePaths)
            {
                if (Directory.Exists(adobeBasePath))
                {
                    // Clean common Adobe app folders
                    var adobeApps = new[] 
                    { 
                        "Photoshop", "Illustrator", "InDesign", "Premiere Pro", "After Effects",
                        "Acrobat", "Acrobat DC", "Adobe XD", "Lightroom", "Media Encoder",
                        "Animate", "Audition", "Bridge", "Character Animator", "Dimension",
                        "Dreamweaver", "Fresco", "Prelude", "Substance 3D Designer",
                        "Substance 3D Painter", "Substance 3D Sampler", "Substance 3D Stager"
                    };

                    foreach (var adobeApp in adobeApps)
                    {
                        var appPath = Path.Combine(adobeBasePath, adobeApp);
                        if (Directory.Exists(appPath))
                        {
                            // Clean logs, cache, temp files (preserve prefs, licenses, workspaces)
                            var cleanupFolders = new[] { "Logs", "Cache", "Caches", "Temp", "logs", "cache" };
                            foreach (var folder in cleanupFolders)
                            {
                                var folderPath = Path.Combine(appPath, folder);
                                if (Directory.Exists(folderPath))
                                {
                                    try
                                    {
                                        var size = GetDirectorySize(folderPath);
                                        if (_dryRun)
                                        {
                                            LogDryRun($"Would delete directory: {folderPath} ({FormatBytes(size)})");
                                        }
                                        else
                                        {
                                            Directory.Delete(folderPath, true);
                                        }
                                        freedSpace += size;
                                        clearedCount++;
                                    }
                                    catch { }
                                }
                            }

                            // Clean log files
                            try
                            {
                                var logFiles = Directory.GetFiles(appPath, "*.log", SearchOption.AllDirectories);
                                foreach (var file in logFiles)
                                {
                                    try
                                    {
                                        var fi = new FileInfo(file);
                                        freedSpace += fi.Length;
                                        if (_dryRun)
                                        {
                                            LogDryRun($"Would delete file: {file}");
                                        }
                                        else
                                        {
                                            fi.Delete();
                                        }
                                        clearedCount++;
                                    }
                                    catch { }
                                }
                            }
                            catch { }
                        }
                    }

                    // Adobe Creative Cloud specific cleanup
                    var creativeClouds = new[] { "Creative Cloud", "Adobe Creative Cloud", "ACC" };
                    foreach (var ccName in creativeClouds)
                    {
                        var ccPath = Path.Combine(adobeBasePath, ccName);
                        if (Directory.Exists(ccPath))
                        {
                            var ccCleanupPaths = new[]
                            {
                                Path.Combine(ccPath, "Logs"),
                                Path.Combine(ccPath, "Cache"),
                                Path.Combine(ccPath, "logs")
                            };

                            foreach (var path in ccCleanupPaths)
                            {
                                if (Directory.Exists(path))
                                {
                                    try
                                    {
                                        var size = GetDirectorySize(path);
                                        if (_dryRun)
                                        {
                                            LogDryRun($"Would delete directory: {path} ({FormatBytes(size)})");
                                        }
                                        else
                                        {
                                            Directory.Delete(path, true);
                                        }
                                        freedSpace += size;
                                        clearedCount++;
                                    }
                                    catch { }
                                }
                            }
                        }
                    }
                }
            }

            // Adobe temp files in system temp
            try
            {
                var tempPath = Path.GetTempPath();
                if (Directory.Exists(tempPath))
                {
                    var adobeTempDirs = Directory.GetDirectories(tempPath, "Adobe*", SearchOption.TopDirectoryOnly);
                    foreach (var dir in adobeTempDirs)
                    {
                        try
                        {
                            var size = GetDirectorySize(dir);
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete directory: {dir} ({FormatBytes(size)})");
                            }
                            else
                            {
                                Directory.Delete(dir, true);
                            }
                            freedSpace += size;
                            clearedCount++;
                        }
                        catch { }
                    }
                }
            }
            catch { }

            // 💻 JETBRAINS IDEs - Comprehensive cleanup
            Console.WriteLine("  Cleaning JetBrains IDEs...");
            
            // JetBrains stores configs in user home
            var jetBrainsBasePaths = new[]
            {
                Path.Combine(userProfile, ".IntelliJIdea*"),
                Path.Combine(userProfile, ".PyCharm*"),
                Path.Combine(userProfile, ".WebStorm*"),
                Path.Combine(userProfile, ".PhpStorm*"),
                Path.Combine(userProfile, ".RubyMine*"),
                Path.Combine(userProfile, ".CLion*"),
                Path.Combine(userProfile, ".GoLand*"),
                Path.Combine(userProfile, ".Rider*"),
                Path.Combine(userProfile, ".DataGrip*"),
                Path.Combine(userProfile, ".RustRover*"),
                Path.Combine(userProfile, ".AndroidStudio*"),
                Path.Combine(userProfile, ".Fleet*"),
                Path.Combine(userProfile, ".AppCode*"),
                Path.Combine(localAppData, "JetBrains"),
                Path.Combine(appData, "JetBrains")
            };

            foreach (var pattern in jetBrainsBasePaths)
            {
                if (pattern.Contains("*"))
                {
                    var parentDir = Path.GetDirectoryName(pattern);
                    if (Directory.Exists(parentDir))
                    {
                        var matchPattern = Path.GetFileName(pattern);
                        var dirs = Directory.GetDirectories(parentDir, matchPattern);
                        
                        foreach (var ideDir in dirs)
                        {
                            if (Directory.Exists(ideDir))
                            {
                                // Clean logs, cache, temp (preserve config, licenses, plugins settings)
                                var cleanupSubdirs = new[] { "system/log", "system/caches", "system/tmp", "logs" };
                                
                                foreach (var subdir in cleanupSubdirs)
                                {
                                    var cleanupPath = Path.Combine(ideDir, subdir);
                                    if (Directory.Exists(cleanupPath))
                                    {
                                        try
                                        {
                                            var size = GetDirectorySize(cleanupPath);
                                            if (_dryRun)
                                            {
                                                LogDryRun($"Would delete directory: {cleanupPath} ({FormatBytes(size)})");
                                            }
                                            else
                                            {
                                                Directory.Delete(cleanupPath, true);
                                            }
                                            freedSpace += size;
                                            clearedCount++;
                                        }
                                        catch { }
                                    }
                                }

                                // Clean log files in system folder
                                var systemFolder = Path.Combine(ideDir, "system");
                                if (Directory.Exists(systemFolder))
                                {
                                    try
                                    {
                                        var logFiles = Directory.GetFiles(systemFolder, "*.log", SearchOption.AllDirectories);
                                        foreach (var file in logFiles)
                                        {
                                            try
                                            {
                                                var fi = new FileInfo(file);
                                                freedSpace += fi.Length;
                                                if (_dryRun)
                                                {
                                                    LogDryRun($"Would delete file: {file}");
                                                }
                                                else
                                                {
                                                    fi.Delete();
                                                }
                                                clearedCount++;
                                            }
                                            catch { }
                                        }
                                    }
                                    catch { }
                                }
                            }
                        }
                    }
                }
                else if (Directory.Exists(pattern))
                {
                    // JetBrains in AppData
                    var toolboxLogs = Path.Combine(pattern, "Toolbox", "logs");
                    if (Directory.Exists(toolboxLogs))
                    {
                        try
                        {
                            var size = GetDirectorySize(toolboxLogs);
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete directory: {toolboxLogs} ({FormatBytes(size)})");
                            }
                            else
                            {
                                Directory.Delete(toolboxLogs, true);
                            }
                            freedSpace += size;
                            clearedCount++;
                        }
                        catch { }
                    }

                    // Clean IDE logs in JetBrains folder
                    try
                    {
                        var logFiles = Directory.GetFiles(pattern, "*.log", SearchOption.AllDirectories);
                        foreach (var file in logFiles)
                        {
                            try
                            {
                                var fi = new FileInfo(file);
                                freedSpace += fi.Length;
                                if (_dryRun)
                                {
                                    LogDryRun($"Would delete file: {file}");
                                }
                                else
                                {
                                    fi.Delete();
                                }
                                clearedCount++;
                            }
                            catch { }
                        }
                    }
                    catch { }
                }
            }

            // JetBrains caches in system temp
            try
            {
                var tempPath = Path.GetTempPath();
                if (Directory.Exists(tempPath))
                {
                    var jetBrainsTempDirs = Directory.GetDirectories(tempPath, "jetbrains*", SearchOption.TopDirectoryOnly);
                    foreach (var dir in jetBrainsTempDirs)
                    {
                        try
                        {
                            var size = GetDirectorySize(dir);
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete directory: {dir} ({FormatBytes(size)})");
                            }
                            else
                            {
                                Directory.Delete(dir, true);
                            }
                            freedSpace += size;
                            clearedCount++;
                        }
                        catch { }
                    }
                }
            }
            catch { }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"  ✅ {(_dryRun ? "Would clear" : "Cleared")} {clearedCount} app-specific items ({FormatBytes(freedSpace)} freed)");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  ❌ Failed: {ex.Message}");
            Console.ResetColor();
        }
    }

    static void ClearDetectedInstalledApps()
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("\n🔹 Scanning for all installed applications and clearing their logs/caches...");
        Console.ResetColor();

        long freedSpace = 0;
        int clearedCount = 0;
        var detectedApps = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        try
        {
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var programData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            Console.WriteLine("  Scanning installed applications from multiple sources...");

            // 1. Scan Windows Registry for installed applications
            try
            {
                var registryKeys = new[]
                {
                    Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall"),
                    Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall"),
                    Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall")
                };

                foreach (var regKey in registryKeys)
                {
                    if (regKey != null)
                    {
                        foreach (var subKeyName in regKey.GetSubKeyNames())
                        {
                            try
                            {
                                using var subKey = regKey.OpenSubKey(subKeyName);
                                var displayName = subKey?.GetValue("DisplayName")?.ToString();
                                if (!string.IsNullOrEmpty(displayName))
                                {
                                    detectedApps.Add(displayName);
                                }
                            }
                            catch { }
                        }
                        regKey.Close();
                    }
                }
            }
            catch { }

            // 2. Scan Microsoft Store apps (WindowsApps)
            try
            {
                var windowsAppsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "WindowsApps");
                if (Directory.Exists(windowsAppsPath))
                {
                    var storeDirs = Directory.GetDirectories(windowsAppsPath);
                    foreach (var dir in storeDirs)
                    {
                        var appName = Path.GetFileName(dir).Split('_')[0];
                        detectedApps.Add(appName);
                    }
                }
            }
            catch { }

            // 3. Scan Scoop apps
            try
            {
                var scoopPath = Path.Combine(userProfile, "scoop", "apps");
                if (Directory.Exists(scoopPath))
                {
                    var scoopApps = Directory.GetDirectories(scoopPath);
                    foreach (var app in scoopApps)
                    {
                        detectedApps.Add(Path.GetFileName(app));
                    }
                }
            }
            catch { }

            // 4. Scan WinGet-installed apps (check common locations)
            try
            {
                var wingetAppsPath = Path.Combine(localAppData, "Microsoft", "WinGet", "Packages");
                if (Directory.Exists(wingetAppsPath))
                {
                    var wingetApps = Directory.GetDirectories(wingetAppsPath);
                    foreach (var app in wingetApps)
                    {
                        detectedApps.Add(Path.GetFileName(app));
                    }
                }
            }
            catch { }

            // 5. Scan Chocolatey apps
            try
            {
                var chocoPath = @"C:\ProgramData\chocolatey\lib";
                if (Directory.Exists(chocoPath))
                {
                    var chocoApps = Directory.GetDirectories(chocoPath);
                    foreach (var app in chocoApps)
                    {
                        detectedApps.Add(Path.GetFileName(app));
                    }
                }
            }
            catch { }

            // 6. Scan running Windows Services for applications
            try
            {
                var services = System.ServiceProcess.ServiceController.GetServices();
                foreach (var service in services)
                {
                    try
                    {
                        // Extract app name from service name (e.g., "VMwareHostd" -> "VMware")
                        var serviceName = service.ServiceName;
                        
                        // Add the service name itself
                        detectedApps.Add(serviceName);
                        
                        // Try to extract company/product name from service display name
                        var displayName = service.DisplayName;
                        if (!string.IsNullOrEmpty(displayName))
                        {
                            // Split by common delimiters and add significant words
                            var words = displayName.Split(new[] { ' ', '-', '_', '(', ')' }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (var word in words)
                            {
                                // Only add words that look like product names (not generic words)
                                if (word.Length >= 4 && !word.Equals("Service", StringComparison.OrdinalIgnoreCase) && 
                                    !word.Equals("Windows", StringComparison.OrdinalIgnoreCase))
                                {
                                    detectedApps.Add(word);
                                }
                            }
                        }
                    }
                    catch { }
                }
            }
            catch { }

            // 7. Scan AppData folders for installed apps
            try
            {
                if (Directory.Exists(localAppData))
                {
                    var localAppDirs = Directory.GetDirectories(localAppData);
                    foreach (var dir in localAppDirs)
                    {
                        detectedApps.Add(Path.GetFileName(dir));
                    }
                }

                if (Directory.Exists(appData))
                {
                    var appDataDirs = Directory.GetDirectories(appData);
                    foreach (var dir in appDataDirs)
                    {
                        detectedApps.Add(Path.GetFileName(dir));
                    }
                }
            }
            catch { }

            Console.WriteLine($"  Found {detectedApps.Count} unique applications");
            Console.WriteLine("  Cleaning logs, caches, and temp files...");

            // Target folders to clean for each app
            var targetFolders = new[] { "logs", "log", "cache", "caches", "temp", "tmp", "Logs", "Cache", "Temp" };

            // Now clean logs/caches for all detected apps
            foreach (var appName in detectedApps)
            {
                // Clean from LocalAppData
                var localAppPath = Path.Combine(localAppData, appName);
                if (Directory.Exists(localAppPath))
                {
                    foreach (var targetFolder in targetFolders)
                    {
                        var targetPath = Path.Combine(localAppPath, targetFolder);
                        if (Directory.Exists(targetPath))
                        {
                            try
                            {
                                var size = GetDirectorySize(targetPath);
                                if (_dryRun)
                                {
                                    LogDryRun($"Would delete: {targetPath} ({FormatBytes(size)})");
                                }
                                else
                                {
                                    Directory.Delete(targetPath, true);
                                }
                                freedSpace += size;
                                clearedCount++;
                            }
                            catch { }
                        }
                    }

                    // Also clean .log, .tmp, .dmp files directly in app folder
                    try
                    {
                        var logFiles = Directory.GetFiles(localAppPath, "*.log", SearchOption.TopDirectoryOnly)
                            .Concat(Directory.GetFiles(localAppPath, "*.tmp", SearchOption.TopDirectoryOnly))
                            .Concat(Directory.GetFiles(localAppPath, "*.dmp", SearchOption.TopDirectoryOnly));

                        foreach (var file in logFiles)
                        {
                            try
                            {
                                var fi = new FileInfo(file);
                                freedSpace += fi.Length;
                                if (_dryRun)
                                {
                                    LogDryRun($"Would delete: {file}");
                                }
                                else
                                {
                                    fi.Delete();
                                }
                                clearedCount++;
                            }
                            catch { }
                        }
                    }
                    catch { }
                }

                // Clean from AppData (Roaming)
                var appDataPath = Path.Combine(appData, appName);
                if (Directory.Exists(appDataPath))
                {
                    foreach (var targetFolder in targetFolders)
                    {
                        var targetPath = Path.Combine(appDataPath, targetFolder);
                        if (Directory.Exists(targetPath))
                        {
                            try
                            {
                                var size = GetDirectorySize(targetPath);
                                if (_dryRun)
                                {
                                    LogDryRun($"Would delete: {targetPath} ({FormatBytes(size)})");
                                }
                                else
                                {
                                    Directory.Delete(targetPath, true);
                                }
                                freedSpace += size;
                                clearedCount++;
                            }
                            catch { }
                        }
                    }

                    // Also clean .log, .tmp, .dmp files
                    try
                    {
                        var logFiles = Directory.GetFiles(appDataPath, "*.log", SearchOption.TopDirectoryOnly)
                            .Concat(Directory.GetFiles(appDataPath, "*.tmp", SearchOption.TopDirectoryOnly))
                            .Concat(Directory.GetFiles(appDataPath, "*.dmp", SearchOption.TopDirectoryOnly));

                        foreach (var file in logFiles)
                        {
                            try
                            {
                                var fi = new FileInfo(file);
                                freedSpace += fi.Length;
                                if (_dryRun)
                                {
                                    LogDryRun($"Would delete: {file}");
                                }
                                else
                                {
                                    fi.Delete();
                                }
                                clearedCount++;
                            }
                            catch { }
                        }
                    }
                    catch { }
                }

                // Clean from ProgramData
                var programDataPath = Path.Combine(programData, appName);
                if (Directory.Exists(programDataPath))
                {
                    foreach (var targetFolder in targetFolders)
                    {
                        var targetPath = Path.Combine(programDataPath, targetFolder);
                        if (Directory.Exists(targetPath))
                        {
                            try
                            {
                                var size = GetDirectorySize(targetPath);
                                if (_dryRun)
                                {
                                    LogDryRun($"Would delete: {targetPath} ({FormatBytes(size)})");
                                }
                                else
                                {
                                    Directory.Delete(targetPath, true);
                                }
                                freedSpace += size;
                                clearedCount++;
                            }
                            catch { }
                        }
                    }

                    // Also clean .log files
                    try
                    {
                        var logFiles = Directory.GetFiles(programDataPath, "*.log", SearchOption.AllDirectories);
                        foreach (var file in logFiles)
                        {
                            try
                            {
                                var fi = new FileInfo(file);
                                freedSpace += fi.Length;
                                if (_dryRun)
                                {
                                    LogDryRun($"Would delete: {file}");
                                }
                                else
                                {
                                    fi.Delete();
                                }
                                clearedCount++;
                            }
                            catch { }
                        }
                    }
                    catch { }
                }
            }

            if (clearedCount > 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                if (_dryRun)
                {
                    Console.WriteLine($"  ✅ Would clear {clearedCount} items from detected apps ({FormatBytes(freedSpace)} would be freed)");
                }
                else
                {
                    Console.WriteLine($"  ✅ {(_dryRun ? "Would clear" : "Cleared")} {clearedCount} items from detected apps ({FormatBytes(freedSpace)} {(_dryRun ? "would be freed" : "freed")})");
                }
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("  ⚠️ No additional cache/log files found");
                Console.ResetColor();
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  ❌ Failed: {ex.Message}");
            Console.ResetColor();
        }
    }

    static void ClearAdditionalWindowsTraces()
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("\n🔹 Clearing additional Windows traces...");
        Console.ResetColor();

        LogDryRun("\n=== ADDITIONAL WINDOWS TRACES ===");

        long freedSpace = 0;
        int clearedCount = 0;

        try
        {
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            // Windows Installer logs
            var installerLogsPath = @"C:\Windows\Logs\CBS";
            if (Directory.Exists(installerLogsPath))
            {
                try
                {
                    var files = Directory.GetFiles(installerLogsPath, "*.log");
                    foreach (var file in files)
                    {
                        try
                        {
                            var fileInfo = new FileInfo(file);
                            freedSpace += fileInfo.Length;
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete file: {file}");
                            }
                            else
                            {
                                fileInfo.Delete();
                            }
                            clearedCount++;
                        }
                        catch { }
                    }
                }
                catch { }
            }

            // Windows Panther logs (Windows setup)
            var pantherPath = @"C:\Windows\Panther";
            if (Directory.Exists(pantherPath))
            {
                try
                {
                    var logFiles = Directory.GetFiles(pantherPath, "*.log", SearchOption.AllDirectories);
                    foreach (var logFile in logFiles)
                    {
                        try
                        {
                            var fileInfo = new FileInfo(logFile);
                            freedSpace += fileInfo.Length;
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete file: {logFile}");
                            }
                            else
                            {
                                fileInfo.Delete();
                            }
                            clearedCount++;
                        }
                        catch { }
                    }
                }
                catch { }
            }

            // Windows Performance Diagnostics logs
            var perfLogsPath = Path.Combine(localAppData, "Microsoft", "Windows", "WER");
            if (Directory.Exists(perfLogsPath))
            {
                try
                {
                    var size = GetDirectorySize(perfLogsPath);
                    if (_dryRun)
                    {
                        LogDryRun($"Would delete directory: {perfLogsPath} ({FormatBytes(size)})");
                    }
                    else
                    {
                        Directory.Delete(perfLogsPath, true);
                    }
                    freedSpace += size;
                    clearedCount++;
                }
                catch { }
            }

            // File History (Previous Versions) configuration data
            var fileHistoryPath = Path.Combine(localAppData, "Microsoft", "Windows", "FileHistory", "Configuration");
            if (Directory.Exists(fileHistoryPath))
            {
                try
                {
                    var files = Directory.GetFiles(fileHistoryPath, "*.*");
                    foreach (var file in files)
                    {
                        // Don't delete Config1.xml and Config2.xml as they contain settings
                        if (!file.EndsWith("Config1.xml", StringComparison.OrdinalIgnoreCase) &&
                            !file.EndsWith("Config2.xml", StringComparison.OrdinalIgnoreCase))
                        {
                            try
                            {
                                var fileInfo = new FileInfo(file);
                                freedSpace += fileInfo.Length;
                                if (_dryRun)
                                {
                                    LogDryRun($"Would delete file: {file}");
                                }
                                else
                                {
                                    fileInfo.Delete();
                                }
                                clearedCount++;
                            }
                            catch { }
                        }
                    }
                }
                catch { }
            }

            // GameDVR (Xbox Game Bar) clips metadata
            var gameDvrPath = Path.Combine(localAppData, "Packages", "Microsoft.XboxGamingOverlay_8wekyb3d8bbwe", "LocalCache");
            if (Directory.Exists(gameDvrPath))
            {
                try
                {
                    var size = GetDirectorySize(gameDvrPath);
                    if (_dryRun)
                    {
                        LogDryRun($"Would delete directory: {gameDvrPath} ({FormatBytes(size)})");
                    }
                    else
                    {
                        Directory.Delete(gameDvrPath, true);
                    }
                    freedSpace += size;
                    clearedCount++;
                }
                catch { }
            }

            // Windows Defender quarantine history (not actual threats, just history)
            var defenderHistoryPath = @"C:\ProgramData\Microsoft\Windows Defender\Scans\History";
            if (Directory.Exists(defenderHistoryPath))
            {
                try
                {
                    var historyFolder = Path.Combine(defenderHistoryPath, "Service");
                    if (Directory.Exists(historyFolder))
                    {
                        var size = GetDirectorySize(historyFolder);
                        if (_dryRun)
                        {
                            LogDryRun($"Would delete directory: {historyFolder} ({FormatBytes(size)})");
                        }
                        else
                        {
                            Directory.Delete(historyFolder, true);
                        }
                        freedSpace += size;
                        clearedCount++;
                    }
                }
                catch { }
            }

            // Windows Store app updates cache
            var storeUpdateCachePath = Path.Combine(localAppData, "Packages", "Microsoft.WindowsStore_8wekyb3d8bbwe", "AC", "INetCache");
            if (Directory.Exists(storeUpdateCachePath))
            {
                try
                {
                    var size = GetDirectorySize(storeUpdateCachePath);
                    if (_dryRun)
                    {
                        LogDryRun($"Would delete directory: {storeUpdateCachePath} ({FormatBytes(size)})");
                    }
                    else
                    {
                        Directory.Delete(storeUpdateCachePath, true);
                    }
                    freedSpace += size;
                    clearedCount++;
                }
                catch { }
            }

            // Microsoft Edge update cache
            var edgeUpdateCachePath = Path.Combine(localAppData, "Microsoft", "EdgeUpdate", "Download");
            if (Directory.Exists(edgeUpdateCachePath))
            {
                try
                {
                    var size = GetDirectorySize(edgeUpdateCachePath);
                    if (_dryRun)
                    {
                        LogDryRun($"Would delete directory: {edgeUpdateCachePath} ({FormatBytes(size)})");
                    }
                    else
                    {
                        Directory.Delete(edgeUpdateCachePath, true);
                    }
                    freedSpace += size;
                    clearedCount++;
                }
                catch { }
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"  ✅ {(_dryRun ? "Would clear" : "Cleared")} {clearedCount} additional Windows traces ({FormatBytes(freedSpace)} freed)");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  ❌ Failed: {ex.Message}");
            Console.ResetColor();
        }
    }

    static void ClearPowerToys()
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("\n🔹 Clearing PowerToys logs and caches...");
        Console.ResetColor();

        long freedSpace = 0;
        int clearedCount = 0;

        try
        {
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var programData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

            // PowerToys main directories
            var powerToysBasePaths = new[]
            {
                Path.Combine(localAppData, "Microsoft", "PowerToys"),
                Path.Combine(programData, "Microsoft", "PowerToys")
            };

            bool foundAny = false;
            foreach (var basePath in powerToysBasePaths)
            {
                if (!Directory.Exists(basePath))
                    continue;

                foundAny = true;

                // Clear logs for all PowerToys modules
                var logPaths = new[]
                {
                    Path.Combine(basePath, "Logs"),
                    Path.Combine(basePath, "FancyZones", "Logs"),
                    Path.Combine(basePath, "PowerRename", "Logs"),
                    Path.Combine(basePath, "ImageResizer", "Logs"),
                    Path.Combine(basePath, "KeyboardManager", "Logs"),
                    Path.Combine(basePath, "PowerLauncher", "Logs"),
                    Path.Combine(basePath, "ColorPicker", "Logs"),
                    Path.Combine(basePath, "ShortcutGuide", "Logs"),
                    Path.Combine(basePath, "FileLocksmith", "Logs"),
                    Path.Combine(basePath, "Awake", "Logs"),
                    Path.Combine(basePath, "PowerAccent", "Logs"),
                    Path.Combine(basePath, "PowerOCR", "Logs"),
                    Path.Combine(basePath, "MouseUtils", "Logs"),
                    Path.Combine(basePath, "Peek", "Logs"),
                    Path.Combine(basePath, "RegistryPreview", "Logs"),
                    Path.Combine(basePath, "CropAndLock", "Logs"),
                    Path.Combine(basePath, "EnvironmentVariables", "Logs"),
                    Path.Combine(basePath, "Hosts", "Logs"),
                    Path.Combine(basePath, "MeasureTool", "Logs"),
                    Path.Combine(basePath, "PowerToys Run", "Logs"),
                };

                foreach (var logPath in logPaths)
                {
                    if (Directory.Exists(logPath))
                    {
                        try
                        {
                            var files = Directory.GetFiles(logPath, "*.*", SearchOption.AllDirectories);
                            foreach (var file in files)
                            {
                                try
                                {
                                    if (_dryRun)
                                    {
                                        var fileInfo = new FileInfo(file);
                                        freedSpace += fileInfo.Length;
                                        LogDryRun($"Would delete: {file} ({FormatBytes(fileInfo.Length)})");
                                        clearedCount++;
                                    }
                                    else
                                    {
                                        var fileInfo = new FileInfo(file);
                                        freedSpace += fileInfo.Length;
                                        fileInfo.Delete();
                                        clearedCount++;
                                    }
                                }
                                catch { /* Skip files in use */ }
                            }
                        }
                        catch { /* Skip if inaccessible */ }
                    }
                }

                // Clear PowerToys cache directories
                var cachePaths = new[]
                {
                    Path.Combine(basePath, "Cache"),
                    Path.Combine(basePath, "PowerLauncher", "Cache"),
                    Path.Combine(basePath, "PowerToys Run", "Cache"),
                    Path.Combine(basePath, "Peek", "Cache"),
                };

                foreach (var cachePath in cachePaths)
                {
                    if (Directory.Exists(cachePath))
                    {
                        try
                        {
                            var files = Directory.GetFiles(cachePath, "*.*", SearchOption.AllDirectories);
                            foreach (var file in files)
                            {
                                try
                                {
                                    if (_dryRun)
                                    {
                                        var fileInfo = new FileInfo(file);
                                        freedSpace += fileInfo.Length;
                                        LogDryRun($"Would delete: {file} ({FormatBytes(fileInfo.Length)})");
                                        clearedCount++;
                                    }
                                    else
                                    {
                                        var fileInfo = new FileInfo(file);
                                        freedSpace += fileInfo.Length;
                                        fileInfo.Delete();
                                        clearedCount++;
                                    }
                                }
                                catch { /* Skip files in use */ }
                            }

                            if (!_dryRun)
                            {
                                try { Directory.Delete(cachePath, true); } catch { }
                            }
                        }
                        catch { /* Skip if inaccessible */ }
                    }
                }

                // Clear temp files and crash dumps
                var tempPaths = new[]
                {
                    Path.Combine(basePath, "Temp"),
                    Path.Combine(basePath, "CrashDumps"),
                };

                foreach (var tempPath in tempPaths)
                {
                    if (Directory.Exists(tempPath))
                    {
                        try
                        {
                            var files = Directory.GetFiles(tempPath, "*.*", SearchOption.AllDirectories);
                            foreach (var file in files)
                            {
                                try
                                {
                                    if (_dryRun)
                                    {
                                        var fileInfo = new FileInfo(file);
                                        freedSpace += fileInfo.Length;
                                        LogDryRun($"Would delete: {file} ({FormatBytes(fileInfo.Length)})");
                                        clearedCount++;
                                    }
                                    else
                                    {
                                        var fileInfo = new FileInfo(file);
                                        freedSpace += fileInfo.Length;
                                        fileInfo.Delete();
                                        clearedCount++;
                                    }
                                }
                                catch { /* Skip files in use */ }
                            }

                            if (!_dryRun)
                            {
                                try { Directory.Delete(tempPath, true); } catch { }
                            }
                        }
                        catch { /* Skip if inaccessible */ }
                    }
                }

                // Clear settings backups (keep current settings)
                var backupPath = Path.Combine(basePath, "Backup");
                if (Directory.Exists(backupPath))
                {
                    try
                    {
                        var files = Directory.GetFiles(backupPath, "*.*", SearchOption.AllDirectories);
                        foreach (var file in files)
                        {
                            try
                            {
                                // Keep the most recent backup, delete older ones
                                var fileInfo = new FileInfo(file);
                                if ((DateTime.Now - fileInfo.LastWriteTime).TotalDays > 7)
                                {
                                    if (_dryRun)
                                    {
                                        freedSpace += fileInfo.Length;
                                        LogDryRun($"Would delete old backup: {file} ({FormatBytes(fileInfo.Length)})");
                                        clearedCount++;
                                    }
                                    else
                                    {
                                        freedSpace += fileInfo.Length;
                                        fileInfo.Delete();
                                        clearedCount++;
                                    }
                                }
                            }
                            catch { /* Skip files in use */ }
                        }
                    }
                    catch { /* Skip if inaccessible */ }
                }
            }

            if (foundAny && clearedCount > 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                if (_dryRun)
                {
                    Console.WriteLine($"  ✅ Would clear {clearedCount} PowerToys items ({FormatBytes(freedSpace)} would be freed)");
                }
                else
                {
                    Console.WriteLine($"  ✅ {(_dryRun ? "Would clear" : "Cleared")} {clearedCount} PowerToys items ({FormatBytes(freedSpace)} {(_dryRun ? "would be freed" : "freed")})");
                }
                Console.ResetColor();
            }
            else if (!foundAny)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("  ⚠️ PowerToys not detected on this system");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("  ⚠️ No cached files found to clean");
                Console.ResetColor();
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  ❌ Failed: {ex.Message}");
            Console.ResetColor();
        }
    }

    static void ClearPrismCache()
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("\n🔹 Clearing Prism ARM to x86/x64 emulation cache...");
        Console.ResetColor();

        long freedSpace = 0;
        int clearedCount = 0;

        try
        {
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var programData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

            // Prism translation cache locations (ARM64 emulation)
            // XtaCache is the main Prism cache location on ARM64 Windows
            var prismPaths = new[]
            {
                @"C:\Windows\XtaCache", // Primary Prism cache (x86-to-ARM64 translation)
                Path.Combine(localAppData, "Microsoft", "Windows", "Prism"),
                Path.Combine(programData, "Microsoft", "Windows", "Prism"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Prism"),
            };

            bool foundAny = false;
            foreach (var prismPath in prismPaths)
            {
                if (!Directory.Exists(prismPath))
                    continue;

                foundAny = true;
                Console.WriteLine($"  Cleaning Prism cache at {prismPath}...");

                try
                {
                    var files = Directory.GetFiles(prismPath, "*.*", SearchOption.AllDirectories);
                    foreach (var file in files)
                    {
                        try
                        {
                            if (_dryRun)
                            {
                                var fileInfo = new FileInfo(file);
                                freedSpace += fileInfo.Length;
                                LogDryRun($"Would delete: {file} ({FormatBytes(fileInfo.Length)})");
                                clearedCount++;
                            }
                            else
                            {
                                var fileInfo = new FileInfo(file);
                                freedSpace += fileInfo.Length;
                                fileInfo.Delete();
                                clearedCount++;
                            }
                        }
                        catch { /* Skip files in use */ }
                    }

                    if (!_dryRun)
                    {
                        try { Directory.Delete(prismPath, true); } catch { }
                    }
                }
                catch { /* Skip if inaccessible */ }
            }

            if (foundAny && clearedCount > 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                if (_dryRun)
                {
                    Console.WriteLine($"  ✅ Would clear {clearedCount} Prism cache items ({FormatBytes(freedSpace)} would be freed)");
                }
                else
                {
                    Console.WriteLine($"  ✅ {(_dryRun ? "Would clear" : "Cleared")} {clearedCount} Prism cache items ({FormatBytes(freedSpace)} {(_dryRun ? "would be freed" : "freed")})");
                }
                Console.ResetColor();
                Console.WriteLine("  ℹ Note: Prism will rebuild cache on next x86/x64 app launch (may take longer)");
            }
            else if (!foundAny)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("  ⚠️ Prism cache not found");
                Console.WriteLine("  ℹ Note: Prism cache is typically only present if x86/x64 apps have been run on ARM64");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("  ⚠️ No cached files found to clean");
                Console.ResetColor();
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  ❌ Failed: {ex.Message}");
            Console.ResetColor();
        }
    }

    static void ClearDevelopmentToolCaches()
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("\n🔹 Clearing development tool caches and logs...");
        Console.ResetColor();

        long freedSpace = 0;
        int clearedCount = 0;

        try
        {
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            // Node.js / npm cache
            var npmCachePath = Path.Combine(appData, "npm-cache");
            if (Directory.Exists(npmCachePath))
            {
                try
                {
                    var size = GetDirectorySize(npmCachePath);
                    if (_dryRun)
                    {
                        LogDryRun($"Would delete: {npmCachePath} ({FormatBytes(size)})");
                    }
                    else
                    {
                        Directory.Delete(npmCachePath, true);
                    }
                    freedSpace += size;
                    clearedCount++;
                }
                catch { }
            }

            // node-gyp cache
            var nodeGypPath = Path.Combine(userProfile, ".node-gyp");
            if (Directory.Exists(nodeGypPath))
            {
                try
                {
                    var files = Directory.GetFiles(nodeGypPath, "*.log", SearchOption.AllDirectories);
                    foreach (var file in files)
                    {
                        try
                        {
                            var fi = new FileInfo(file);
                            freedSpace += fi.Length;
                            if (!_dryRun) fi.Delete();
                            else LogDryRun($"Would delete: {file}");
                            clearedCount++;
                        }
                        catch { }
                    }
                }
                catch { }
            }

            // NuGet package cache
            var nugetCachePaths = new[]
            {
                Path.Combine(userProfile, ".nuget", "packages"),
                Path.Combine(localAppData, "NuGet", "Cache"),
                Path.Combine(localAppData, "NuGet", "v3-cache")
            };

            foreach (var nugetPath in nugetCachePaths)
            {
                if (Directory.Exists(nugetPath) && nugetPath.Contains("Cache", StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        var size = GetDirectorySize(nugetPath);
                        if (_dryRun)
                        {
                            LogDryRun($"Would delete: {nugetPath} ({FormatBytes(size)})");
                        }
                        else
                        {
                            Directory.Delete(nugetPath, true);
                        }
                        freedSpace += size;
                        clearedCount++;
                    }
                    catch { }
                }
            }

            // Maven cache (.m2)
            var mavenCachePath = Path.Combine(userProfile, ".m2", "repository");
            if (Directory.Exists(mavenCachePath))
            {
                try
                {
                    var size = GetDirectorySize(mavenCachePath);
                    // Only report, don't delete automatically (can be large and important)
                    if (_dryRun)
                    {
                        LogDryRun($"Maven cache found: {mavenCachePath} ({FormatBytes(size)}) - skipping auto-delete");
                    }
                }
                catch { }
            }

            // Gradle cache
            var gradleCachePath = Path.Combine(userProfile, ".gradle", "caches");
            if (Directory.Exists(gradleCachePath))
            {
                try
                {
                    var size = GetDirectorySize(gradleCachePath);
                    if (_dryRun)
                    {
                        LogDryRun($"Gradle cache found: {gradleCachePath} ({FormatBytes(size)}) - skipping auto-delete");
                    }
                }
                catch { }
            }

            // Cargo (Rust) cache
            var cargoCachePath = Path.Combine(userProfile, ".cargo", "registry", "cache");
            if (Directory.Exists(cargoCachePath))
            {
                try
                {
                    var size = GetDirectorySize(cargoCachePath);
                    if (_dryRun)
                    {
                        LogDryRun($"Would delete: {cargoCachePath} ({FormatBytes(size)})");
                    }
                    else
                    {
                        Directory.Delete(cargoCachePath, true);
                    }
                    freedSpace += size;
                    clearedCount++;
                }
                catch { }
            }

            // Go module cache
            var goModCachePath = Path.Combine(userProfile, "go", "pkg", "mod", "cache");
            if (Directory.Exists(goModCachePath))
            {
                try
                {
                    var size = GetDirectorySize(goModCachePath);
                    if (_dryRun)
                    {
                        LogDryRun($"Would delete: {goModCachePath} ({FormatBytes(size)})");
                    }
                    else
                    {
                        Directory.Delete(goModCachePath, true);
                    }
                    freedSpace += size;
                    clearedCount++;
                }
                catch { }
            }

            // Composer (PHP) cache
            var composerCachePath = Path.Combine(localAppData, "Composer", "cache");
            if (Directory.Exists(composerCachePath))
            {
                try
                {
                    var size = GetDirectorySize(composerCachePath);
                    if (_dryRun)
                    {
                        LogDryRun($"Would delete: {composerCachePath} ({FormatBytes(size)})");
                    }
                    else
                    {
                        Directory.Delete(composerCachePath, true);
                    }
                    freedSpace += size;
                    clearedCount++;
                }
                catch { }
            }

            // Jupyter notebook checkpoints
            var jupyterPaths = Directory.GetDirectories(userProfile, ".ipynb_checkpoints", SearchOption.AllDirectories);
            foreach (var checkpointPath in jupyterPaths)
            {
                try
                {
                    var size = GetDirectorySize(checkpointPath);
                    if (_dryRun)
                    {
                        LogDryRun($"Would delete: {checkpointPath} ({FormatBytes(size)})");
                    }
                    else
                    {
                        Directory.Delete(checkpointPath, true);
                    }
                    freedSpace += size;
                    clearedCount++;
                }
                catch { }
            }

            // Anaconda/Miniconda package cache
            var condaCachePath = Path.Combine(userProfile, ".conda", "pkgs");
            if (Directory.Exists(condaCachePath))
            {
                try
                {
                    var size = GetDirectorySize(condaCachePath);
                    if (_dryRun)
                    {
                        LogDryRun($"Conda cache found: {condaCachePath} ({FormatBytes(size)}) - skipping auto-delete");
                    }
                }
                catch { }
            }

            // Git credential cache
            var gitConfigPath = Path.Combine(userProfile, ".gitconfig");
            // Don't delete credentials, just clean up logs

            // Terraform plugin cache
            var terraformPluginPath = Path.Combine(appData, "terraform.d", "plugin-cache");
            if (Directory.Exists(terraformPluginPath))
            {
                try
                {
                    var size = GetDirectorySize(terraformPluginPath);
                    if (_dryRun)
                    {
                        LogDryRun($"Terraform cache found: {terraformPluginPath} ({FormatBytes(size)}) - skipping auto-delete");
                    }
                }
                catch { }
            }

            // kubectl cache
            var kubectlCachePath = Path.Combine(userProfile, ".kube", "cache");
            if (Directory.Exists(kubectlCachePath))
            {
                try
                {
                    var size = GetDirectorySize(kubectlCachePath);
                    if (_dryRun)
                    {
                        LogDryRun($"Would delete: {kubectlCachePath} ({FormatBytes(size)})");
                    }
                    else
                    {
                        Directory.Delete(kubectlCachePath, true);
                    }
                    freedSpace += size;
                    clearedCount++;
                }
                catch { }
            }

            // Azure CLI cache and logs
            var azureCachePaths = new[]
            {
                Path.Combine(userProfile, ".azure", "logs"),
                Path.Combine(userProfile, ".azure", "commands"),
                Path.Combine(userProfile, ".azure", "telemetry")
            };

            foreach (var azurePath in azureCachePaths)
            {
                if (Directory.Exists(azurePath))
                {
                    try
                    {
                        var size = GetDirectorySize(azurePath);
                        if (_dryRun)
                        {
                            LogDryRun($"Would delete: {azurePath} ({FormatBytes(size)})");
                        }
                        else
                        {
                            Directory.Delete(azurePath, true);
                        }
                        freedSpace += size;
                        clearedCount++;
                    }
                    catch { }
                }
            }

            // AWS CLI cache and logs
            var awsCachePath = Path.Combine(userProfile, ".aws", "cli", "cache");
            if (Directory.Exists(awsCachePath))
            {
                try
                {
                    var size = GetDirectorySize(awsCachePath);
                    if (_dryRun)
                    {
                        LogDryRun($"Would delete: {awsCachePath} ({FormatBytes(size)})");
                    }
                    else
                    {
                        Directory.Delete(awsCachePath, true);
                    }
                    freedSpace += size;
                    clearedCount++;
                }
                catch { }
            }

            // Postman logs and cache
            var postmanBasePath = Path.Combine(appData, "Postman");
            if (Directory.Exists(postmanBasePath))
            {
                var postmanPaths = new[]
                {
                    Path.Combine(postmanBasePath, "logs"),
                    Path.Combine(postmanBasePath, "Cache"),
                    Path.Combine(postmanBasePath, "GPUCache"),
                    Path.Combine(postmanBasePath, "Code Cache")
                };

                foreach (var postmanPath in postmanPaths)
                {
                    if (Directory.Exists(postmanPath))
                    {
                        try
                        {
                            var size = GetDirectorySize(postmanPath);
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete: {postmanPath} ({FormatBytes(size)})");
                            }
                            else
                            {
                                Directory.Delete(postmanPath, true);
                            }
                            freedSpace += size;
                            clearedCount++;
                        }
                        catch { }
                    }
                }
            }

            // DBeaver logs and cache
            var dbeaverBasePath = Path.Combine(appData, "DBeaverData");
            if (Directory.Exists(dbeaverBasePath))
            {
                var dbeaverPaths = new[]
                {
                    Path.Combine(dbeaverBasePath, "workspace6", ".metadata", ".log"),
                    Path.Combine(dbeaverBasePath, "logs")
                };

                foreach (var dbeaverPath in dbeaverPaths)
                {
                    if (File.Exists(dbeaverPath))
                    {
                        try
                        {
                            var fi = new FileInfo(dbeaverPath);
                            freedSpace += fi.Length;
                            if (!_dryRun) fi.Delete();
                            else LogDryRun($"Would delete: {dbeaverPath}");
                            clearedCount++;
                        }
                        catch { }
                    }
                    else if (Directory.Exists(dbeaverPath))
                    {
                        try
                        {
                            var size = GetDirectorySize(dbeaverPath);
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete: {dbeaverPath} ({FormatBytes(size)})");
                            }
                            else
                            {
                                Directory.Delete(dbeaverPath, true);
                            }
                            freedSpace += size;
                            clearedCount++;
                        }
                        catch { }
                    }
                }
            }

            if (clearedCount > 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                if (_dryRun)
                {
                    Console.WriteLine($"  ✅ Would clear {clearedCount} dev tool items ({FormatBytes(freedSpace)} would be freed)");
                }
                else
                {
                    Console.WriteLine($"  ✅ {(_dryRun ? "Would clear" : "Cleared")} {clearedCount} dev tool items ({FormatBytes(freedSpace)} {(_dryRun ? "would be freed" : "freed")})");
                }
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("  ⚠️ No dev tool caches found");
                Console.ResetColor();
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  ❌ Failed: {ex.Message}");
            Console.ResetColor();
        }
    }

    static void ClearProgrammingLanguageCaches()
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("\n🔹 Clearing programming language caches (20+ languages)...");
        Console.ResetColor();

        long freedSpace = 0;
        int clearedCount = 0;

        try
        {
            var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            LogDryRun("\n=== PROGRAMMING LANGUAGE CACHES ===");

            // Python caches
            try
            {
                // pip cache
                var pipCachePaths = new[]
                {
                    Path.Combine(localAppData, "pip", "Cache"),
                    Path.Combine(appData, "pip", "cache"),
                    Path.Combine(userProfile, ".cache", "pip")
                };

                foreach (var pipCache in pipCachePaths)
                {
                    if (Directory.Exists(pipCache))
                    {
                        try
                        {
                            var size = GetDirectorySize(pipCache);
                            freedSpace += size;
                            
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete: {pipCache} ({FormatBytes(size)})");
                            }
                            else
                            {
                                Directory.Delete(pipCache, true);
                            }
                            clearedCount++;
                        }
                        catch { }
                    }
                }

                // pytest cache
                var pytestCache = Path.Combine(userProfile, ".pytest_cache");
                if (Directory.Exists(pytestCache))
                {
                    try
                    {
                        var size = GetDirectorySize(pytestCache);
                        freedSpace += size;
                        
                        if (_dryRun)
                        {
                            LogDryRun($"Would delete: {pytestCache} ({FormatBytes(size)})");
                        }
                        else
                        {
                            Directory.Delete(pytestCache, true);
                        }
                        clearedCount++;
                    }
                    catch { }
                }

                // mypy cache
                var mypyCache = Path.Combine(userProfile, ".mypy_cache");
                if (Directory.Exists(mypyCache))
                {
                    try
                    {
                        var size = GetDirectorySize(mypyCache);
                        freedSpace += size;
                        
                        if (_dryRun)
                        {
                            LogDryRun($"Would delete: {mypyCache} ({FormatBytes(size)})");
                        }
                        else
                        {
                            Directory.Delete(mypyCache, true);
                        }
                        clearedCount++;
                    }
                    catch { }
                }

                // tox cache
                var toxCache = Path.Combine(userProfile, ".tox");
                if (Directory.Exists(toxCache))
                {
                    try
                    {
                        var size = GetDirectorySize(toxCache);
                        freedSpace += size;
                        
                        if (_dryRun)
                        {
                            LogDryRun($"Would delete: {toxCache} ({FormatBytes(size)})");
                        }
                        else
                        {
                            Directory.Delete(toxCache, true);
                        }
                        clearedCount++;
                    }
                    catch { }
                }
            }
            catch { }

            // Rust caches
            try
            {
                // Cargo cache (registry, git repos, build artifacts)
                var cargoPaths = new[]
                {
                    Path.Combine(userProfile, ".cargo", "registry", "cache"),
                    Path.Combine(userProfile, ".cargo", "registry", "index"),
                    Path.Combine(userProfile, ".cargo", "git", "checkouts")
                };

                foreach (var cargoPath in cargoPaths)
                {
                    if (Directory.Exists(cargoPath))
                    {
                        try
                        {
                            var size = GetDirectorySize(cargoPath);
                            freedSpace += size;
                            
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete: {cargoPath} ({FormatBytes(size)})");
                            }
                            else
                            {
                                Directory.Delete(cargoPath, true);
                            }
                            clearedCount++;
                        }
                        catch { }
                    }
                }
            }
            catch { }

            // Java caches (Maven, Gradle, Ivy)
            try
            {
                // Maven repository cache
                var mavenCache = Path.Combine(userProfile, ".m2", "repository");
                if (Directory.Exists(mavenCache))
                {
                    try
                    {
                        // Only clear cached artifacts, not installed dependencies
                        var files = Directory.GetFiles(mavenCache, "*.lastUpdated", SearchOption.AllDirectories)
                            .Concat(Directory.GetFiles(mavenCache, "_remote.repositories", SearchOption.AllDirectories))
                            .Concat(Directory.GetFiles(mavenCache, "*.sha*", SearchOption.AllDirectories));

                        foreach (var file in files)
                        {
                            try
                            {
                                var fi = new FileInfo(file);
                                freedSpace += fi.Length;
                                
                                if (_dryRun)
                                {
                                    LogDryRun($"Would delete: {file} ({FormatBytes(fi.Length)})");
                                }
                                else
                                {
                                    fi.Delete();
                                }
                                clearedCount++;
                            }
                            catch { }
                        }
                    }
                    catch { }
                }

                // Gradle caches
                var gradleCachePaths = new[]
                {
                    Path.Combine(userProfile, ".gradle", "caches"),
                    Path.Combine(userProfile, ".gradle", "wrapper", "dists")
                };

                foreach (var gradleCache in gradleCachePaths)
                {
                    if (Directory.Exists(gradleCache))
                    {
                        try
                        {
                            var size = GetDirectorySize(gradleCache);
                            freedSpace += size;
                            
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete: {gradleCache} ({FormatBytes(size)})");
                            }
                            else
                            {
                                Directory.Delete(gradleCache, true);
                            }
                            clearedCount++;
                        }
                        catch { }
                    }
                }

                // Ivy cache
                var ivyCache = Path.Combine(userProfile, ".ivy2", "cache");
                if (Directory.Exists(ivyCache))
                {
                    try
                    {
                        var size = GetDirectorySize(ivyCache);
                        freedSpace += size;
                        
                        if (_dryRun)
                        {
                            LogDryRun($"Would delete: {ivyCache} ({FormatBytes(size)})");
                        }
                        else
                        {
                            Directory.Delete(ivyCache, true);
                        }
                        clearedCount++;
                    }
                    catch { }
                }
            }
            catch { }

            // Ruby caches
            try
            {
                // Gem cache
                var gemCache = Path.Combine(userProfile, ".gem");
                if (Directory.Exists(gemCache))
                {
                    try
                    {
                        var cacheDirs = Directory.GetDirectories(gemCache, "cache", SearchOption.AllDirectories);
                        foreach (var cacheDir in cacheDirs)
                        {
                            try
                            {
                                var size = GetDirectorySize(cacheDir);
                                freedSpace += size;
                                
                                if (_dryRun)
                                {
                                    LogDryRun($"Would delete: {cacheDir} ({FormatBytes(size)})");
                                }
                                else
                                {
                                    Directory.Delete(cacheDir, true);
                                }
                                clearedCount++;
                            }
                            catch { }
                        }
                    }
                    catch { }
                }

                // Bundler cache
                var bundlerCache = Path.Combine(userProfile, ".bundle", "cache");
                if (Directory.Exists(bundlerCache))
                {
                    try
                    {
                        var size = GetDirectorySize(bundlerCache);
                        freedSpace += size;
                        
                        if (_dryRun)
                        {
                            LogDryRun($"Would delete: {bundlerCache} ({FormatBytes(size)})");
                        }
                        else
                        {
                            Directory.Delete(bundlerCache, true);
                        }
                        clearedCount++;
                    }
                    catch { }
                }
            }
            catch { }

            // PHP caches (Composer)
            try
            {
                var composerCachePaths = new[]
                {
                    Path.Combine(appData, "Composer", "cache"),
                    Path.Combine(localAppData, "Composer", "cache"),
                    Path.Combine(userProfile, ".composer", "cache")
                };

                foreach (var composerCache in composerCachePaths)
                {
                    if (Directory.Exists(composerCache))
                    {
                        try
                        {
                            var size = GetDirectorySize(composerCache);
                            freedSpace += size;
                            
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete: {composerCache} ({FormatBytes(size)})");
                            }
                            else
                            {
                                Directory.Delete(composerCache, true);
                            }
                            clearedCount++;
                        }
                        catch { }
                    }
                }
            }
            catch { }

            // Perl caches (CPAN)
            try
            {
                var cpanCachePaths = new[]
                {
                    Path.Combine(userProfile, ".cpan", "build"),
                    Path.Combine(userProfile, ".cpan", "sources")
                };

                foreach (var cpanCache in cpanCachePaths)
                {
                    if (Directory.Exists(cpanCache))
                    {
                        try
                        {
                            var size = GetDirectorySize(cpanCache);
                            freedSpace += size;
                            
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete: {cpanCache} ({FormatBytes(size)})");
                            }
                            else
                            {
                                Directory.Delete(cpanCache, true);
                            }
                            clearedCount++;
                        }
                        catch { }
                    }
                }
            }
            catch { }

            // Nim caches
            try
            {
                var nimCachePaths = new[]
                {
                    Path.Combine(userProfile, ".cache", "nim"),
                    Path.Combine(userProfile, ".nimble", "packages_cache")
                };

                foreach (var nimCache in nimCachePaths)
                {
                    if (Directory.Exists(nimCache))
                    {
                        try
                        {
                            var size = GetDirectorySize(nimCache);
                            freedSpace += size;
                            
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete: {nimCache} ({FormatBytes(size)})");
                            }
                            else
                            {
                                Directory.Delete(nimCache, true);
                            }
                            clearedCount++;
                        }
                        catch { }
                    }
                }
            }
            catch { }

            // Go module cache
            try
            {
                var goModCache = Path.Combine(userProfile, "go", "pkg", "mod", "cache");
                if (Directory.Exists(goModCache))
                {
                    try
                    {
                        var size = GetDirectorySize(goModCache);
                        freedSpace += size;
                        
                        if (_dryRun)
                        {
                            LogDryRun($"Would delete: {goModCache} ({FormatBytes(size)})");
                        }
                        else
                        {
                            Directory.Delete(goModCache, true);
                        }
                        clearedCount++;
                    }
                    catch { }
                }

                // Go build cache
                var goBuildCache = Path.Combine(localAppData, "go-build");
                if (Directory.Exists(goBuildCache))
                {
                    try
                    {
                        var size = GetDirectorySize(goBuildCache);
                        freedSpace += size;
                        
                        if (_dryRun)
                        {
                            LogDryRun($"Would delete: {goBuildCache} ({FormatBytes(size)})");
                        }
                        else
                        {
                            Directory.Delete(goBuildCache, true);
                        }
                        clearedCount++;
                    }
                    catch { }
                }
            }
            catch { }

            // .NET / C# caches
            try
            {
                // NuGet package cache
                var nugetCachePaths = new[]
                {
                    Path.Combine(localAppData, "NuGet", "v3-cache"),
                    Path.Combine(localAppData, "NuGet", "Cache"),
                    Path.Combine(userProfile, ".nuget", "packages", ".tools")
                };

                foreach (var nugetCache in nugetCachePaths)
                {
                    if (Directory.Exists(nugetCache))
                    {
                        try
                        {
                            var size = GetDirectorySize(nugetCache);
                            freedSpace += size;
                            
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete: {nugetCache} ({FormatBytes(size)})");
                            }
                            else
                            {
                                Directory.Delete(nugetCache, true);
                            }
                            clearedCount++;
                        }
                        catch { }
                    }
                }

                // .NET temp / build artifacts
                var dotnetTempPaths = new[]
                {
                    Path.Combine(localAppData, "Temp", ".net"),
                    Path.Combine(Path.GetTempPath(), ".dotnet"),
                    Path.Combine(Path.GetTempPath(), "MSBuild")
                };

                foreach (var tempPath in dotnetTempPaths)
                {
                    if (Directory.Exists(tempPath))
                    {
                        try
                        {
                            var size = GetDirectorySize(tempPath);
                            freedSpace += size;
                            
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete: {tempPath} ({FormatBytes(size)})");
                            }
                            else
                            {
                                Directory.Delete(tempPath, true);
                            }
                            clearedCount++;
                        }
                        catch { }
                    }
                }
            }
            catch { }

            // C / C++ build caches
            try
            {
                // ccache (C/C++ compiler cache)
                var ccachePath = Path.Combine(userProfile, ".ccache");
                if (Directory.Exists(ccachePath))
                {
                    try
                    {
                        var size = GetDirectorySize(ccachePath);
                        freedSpace += size;
                        
                        if (_dryRun)
                        {
                            LogDryRun($"Would delete: {ccachePath} ({FormatBytes(size)})");
                        }
                        else
                        {
                            Directory.Delete(ccachePath, true);
                        }
                        clearedCount++;
                    }
                    catch { }
                }

                // CMake build cache
                var cmakeCachePaths = new[]
                {
                    Path.Combine(userProfile, ".cmake", "packages"),
                    Path.Combine(localAppData, "CMake", "Cache")
                };

                foreach (var cmakeCache in cmakeCachePaths)
                {
                    if (Directory.Exists(cmakeCache))
                    {
                        try
                        {
                            var size = GetDirectorySize(cmakeCache);
                            freedSpace += size;
                            
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete: {cmakeCache} ({FormatBytes(size)})");
                            }
                            else
                            {
                                Directory.Delete(cmakeCache, true);
                            }
                            clearedCount++;
                        }
                        catch { }
                    }
                }

                // vcpkg (C++ package manager)
                var vcpkgPaths = new[]
                {
                    Path.Combine(localAppData, "vcpkg", "archives"),
                    Path.Combine(userProfile, ".vcpkg", "archives")
                };

                foreach (var vcpkgPath in vcpkgPaths)
                {
                    if (Directory.Exists(vcpkgPath))
                    {
                        try
                        {
                            var size = GetDirectorySize(vcpkgPath);
                            freedSpace += size;
                            
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete: {vcpkgPath} ({FormatBytes(size)})");
                            }
                            else
                            {
                                Directory.Delete(vcpkgPath, true);
                            }
                            clearedCount++;
                        }
                        catch { }
                    }
                }

                // Conan (C++ package manager)
                var conanCache = Path.Combine(userProfile, ".conan", "data");
                if (Directory.Exists(conanCache))
                {
                    try
                    {
                        var size = GetDirectorySize(conanCache);
                        freedSpace += size;
                        
                        if (_dryRun)
                        {
                            LogDryRun($"Would delete: {conanCache} ({FormatBytes(size)})");
                        }
                        else
                        {
                            Directory.Delete(conanCache, true);
                        }
                        clearedCount++;
                    }
                    catch { }
                }
            }
            catch { }

            // TypeScript / Node.js additional caches
            try
            {
                // TypeScript compiler cache
                var tscCache = Path.Combine(userProfile, ".tsbuildinfo");
                if (File.Exists(tscCache))
                {
                    try
                    {
                        var fi = new FileInfo(tscCache);
                        freedSpace += fi.Length;
                        
                        if (_dryRun)
                        {
                            LogDryRun($"Would delete: {tscCache}");
                        }
                        else
                        {
                            fi.Delete();
                        }
                        clearedCount++;
                    }
                    catch { }
                }

                // yarn cache
                var yarnCachePaths = new[]
                {
                    Path.Combine(localAppData, "Yarn", "Cache"),
                    Path.Combine(localAppData, "yarn-cache"),
                    Path.Combine(userProfile, ".yarn", "cache")
                };

                foreach (var yarnCache in yarnCachePaths)
                {
                    if (Directory.Exists(yarnCache))
                    {
                        try
                        {
                            var size = GetDirectorySize(yarnCache);
                            freedSpace += size;
                            
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete: {yarnCache} ({FormatBytes(size)})");
                            }
                            else
                            {
                                Directory.Delete(yarnCache, true);
                            }
                            clearedCount++;
                        }
                        catch { }
                    }
                }

                // pnpm cache
                var pnpmCachePaths = new[]
                {
                    Path.Combine(localAppData, "pnpm-cache"),
                    Path.Combine(userProfile, ".pnpm-store")
                };

                foreach (var pnpmCache in pnpmCachePaths)
                {
                    if (Directory.Exists(pnpmCache))
                    {
                        try
                        {
                            var size = GetDirectorySize(pnpmCache);
                            freedSpace += size;
                            
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete: {pnpmCache} ({FormatBytes(size)})");
                            }
                            else
                            {
                                Directory.Delete(pnpmCache, true);
                            }
                            clearedCount++;
                        }
                        catch { }
                    }
                }
            }
            catch { }

            // Kotlin caches
            try
            {
                var kotlinCachePaths = new[]
                {
                    Path.Combine(userProfile, ".kotlin", "cache"),
                    Path.Combine(localAppData, "kotlin")
                };

                foreach (var kotlinCache in kotlinCachePaths)
                {
                    if (Directory.Exists(kotlinCache))
                    {
                        try
                        {
                            var size = GetDirectorySize(kotlinCache);
                            freedSpace += size;
                            
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete: {kotlinCache} ({FormatBytes(size)})");
                            }
                            else
                            {
                                Directory.Delete(kotlinCache, true);
                            }
                            clearedCount++;
                        }
                        catch { }
                    }
                }
            }
            catch { }

            // Scala caches (sbt already covered under Java)
            try
            {
                var scalaCachePaths = new[]
                {
                    Path.Combine(userProfile, ".ivy2", "cache"),
                    Path.Combine(userProfile, ".sbt", "boot", "scala-*", "org.scala-sbt", "sbt", "*", "cache"),
                    Path.Combine(userProfile, ".coursier", "cache")
                };

                foreach (var scalaCache in scalaCachePaths)
                {
                    if (Directory.Exists(scalaCache))
                    {
                        try
                        {
                            var size = GetDirectorySize(scalaCache);
                            freedSpace += size;
                            
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete: {scalaCache} ({FormatBytes(size)})");
                            }
                            else
                            {
                                Directory.Delete(scalaCache, true);
                            }
                            clearedCount++;
                        }
                        catch { }
                    }
                }
            }
            catch { }

            // Swift caches
            try
            {
                var swiftCachePaths = new[]
                {
                    Path.Combine(localAppData, "swiftpm", "cache"),
                    Path.Combine(appData, "swiftpm", "cache"),
                    Path.Combine(userProfile, ".swiftpm", "cache")
                };

                foreach (var swiftCache in swiftCachePaths)
                {
                    if (Directory.Exists(swiftCache))
                    {
                        try
                        {
                            var size = GetDirectorySize(swiftCache);
                            freedSpace += size;
                            
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete: {swiftCache} ({FormatBytes(size)})");
                            }
                            else
                            {
                                Directory.Delete(swiftCache, true);
                            }
                            clearedCount++;
                        }
                        catch { }
                    }
                }
            }
            catch { }

            // R caches
            try
            {
                var rCachePaths = new[]
                {
                    Path.Combine(localAppData, "R", "cache"),
                    Path.Combine(userProfile, ".cache", "R"),
                    Path.Combine(userProfile, ".Rcache")
                };

                foreach (var rCache in rCachePaths)
                {
                    if (Directory.Exists(rCache))
                    {
                        try
                        {
                            var size = GetDirectorySize(rCache);
                            freedSpace += size;
                            
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete: {rCache} ({FormatBytes(size)})");
                            }
                            else
                            {
                                Directory.Delete(rCache, true);
                            }
                            clearedCount++;
                        }
                        catch { }
                    }
                }
            }
            catch { }

            // Julia caches
            try
            {
                var juliaCachePaths = new[]
                {
                    Path.Combine(userProfile, ".julia", "compiled"),
                    Path.Combine(userProfile, ".julia", "scratchspaces"),
                    Path.Combine(userProfile, ".julia", "logs")
                };

                foreach (var juliaCache in juliaCachePaths)
                {
                    if (Directory.Exists(juliaCache))
                    {
                        try
                        {
                            var size = GetDirectorySize(juliaCache);
                            freedSpace += size;
                            
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete: {juliaCache} ({FormatBytes(size)})");
                            }
                            else
                            {
                                Directory.Delete(juliaCache, true);
                            }
                            clearedCount++;
                        }
                        catch { }
                    }
                }
            }
            catch { }

            // Haskell caches (Stack & Cabal)
            try
            {
                var haskellCachePaths = new[]
                {
                    Path.Combine(localAppData, "Programs", "stack", "snapshots"),
                    Path.Combine(appData, "stack", "snapshots"),
                    Path.Combine(appData, "cabal", "packages"),
                    Path.Combine(userProfile, ".stack", "snapshots"),
                    Path.Combine(userProfile, ".cabal", "packages")
                };

                foreach (var haskellCache in haskellCachePaths)
                {
                    if (Directory.Exists(haskellCache))
                    {
                        try
                        {
                            var size = GetDirectorySize(haskellCache);
                            freedSpace += size;
                            
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete: {haskellCache} ({FormatBytes(size)})");
                            }
                            else
                            {
                                Directory.Delete(haskellCache, true);
                            }
                            clearedCount++;
                        }
                        catch { }
                    }
                }
            }
            catch { }

            // Elixir caches (Mix & Hex)
            try
            {
                var elixirCachePaths = new[]
                {
                    Path.Combine(userProfile, ".hex", "cache"),
                    Path.Combine(userProfile, ".mix", "archives")
                };

                foreach (var elixirCache in elixirCachePaths)
                {
                    if (Directory.Exists(elixirCache))
                    {
                        try
                        {
                            var size = GetDirectorySize(elixirCache);
                            freedSpace += size;
                            
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete: {elixirCache} ({FormatBytes(size)})");
                            }
                            else
                            {
                                Directory.Delete(elixirCache, true);
                            }
                            clearedCount++;
                        }
                        catch { }
                    }
                }
            }
            catch { }

            // Lua caches (LuaRocks)
            try
            {
                var luaCachePaths = new[]
                {
                    Path.Combine(appData, "luarocks", "cache"),
                    Path.Combine(userProfile, ".cache", "luarocks")
                };

                foreach (var luaCache in luaCachePaths)
                {
                    if (Directory.Exists(luaCache))
                    {
                        try
                        {
                            var size = GetDirectorySize(luaCache);
                            freedSpace += size;
                            
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete: {luaCache} ({FormatBytes(size)})");
                            }
                            else
                            {
                                Directory.Delete(luaCache, true);
                            }
                            clearedCount++;
                        }
                        catch { }
                    }
                }
            }
            catch { }

            // Dart / Flutter caches
            try
            {
                var dartCachePaths = new[]
                {
                    Path.Combine(localAppData, "Pub", "Cache"),
                    Path.Combine(appData, "Pub", "Cache"),
                    Path.Combine(userProfile, ".pub-cache"),
                    Path.Combine(userProfile, ".dart", "cache"),
                    Path.Combine(userProfile, ".flutter", "cache")
                };

                foreach (var dartCache in dartCachePaths)
                {
                    if (Directory.Exists(dartCache))
                    {
                        try
                        {
                            var size = GetDirectorySize(dartCache);
                            freedSpace += size;
                            
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete: {dartCache} ({FormatBytes(size)})");
                            }
                            else
                            {
                                Directory.Delete(dartCache, true);
                            }
                            clearedCount++;
                        }
                        catch { }
                    }
                }
            }
            catch { }

            // Clojure caches (Leiningen & Clojure CLI)
            try
            {
                var clojureCachePaths = new[]
                {
                    Path.Combine(userProfile, ".lein", "cache"),
                    Path.Combine(userProfile, ".m2", "repository", "org", "clojure"),
                    Path.Combine(userProfile, ".clojure", ".cpcache")
                };

                foreach (var clojureCache in clojureCachePaths)
                {
                    if (Directory.Exists(clojureCache))
                    {
                        try
                        {
                            var size = GetDirectorySize(clojureCache);
                            freedSpace += size;
                            
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete: {clojureCache} ({FormatBytes(size)})");
                            }
                            else
                            {
                                Directory.Delete(clojureCache, true);
                            }
                            clearedCount++;
                        }
                        catch { }
                    }
                }
            }
            catch { }

            // F# caches
            try
            {
                var fsharpCachePaths = new[]
                {
                    Path.Combine(localAppData, "FSI-ASSEMBLY-CACHE"),
                    Path.Combine(appData, ".fsharp", "cache")
                };

                foreach (var fsharpCache in fsharpCachePaths)
                {
                    if (Directory.Exists(fsharpCache))
                    {
                        try
                        {
                            var size = GetDirectorySize(fsharpCache);
                            freedSpace += size;
                            
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete: {fsharpCache} ({FormatBytes(size)})");
                            }
                            else
                            {
                                Directory.Delete(fsharpCache, true);
                            }
                            clearedCount++;
                        }
                        catch { }
                    }
                }
            }
            catch { }

            // Zig caches
            try
            {
                var zigCachePaths = new[]
                {
                    Path.Combine(localAppData, "zig"),
                    Path.Combine(userProfile, ".cache", "zig")
                };

                foreach (var zigCache in zigCachePaths)
                {
                    if (Directory.Exists(zigCache))
                    {
                        try
                        {
                            var size = GetDirectorySize(zigCache);
                            freedSpace += size;
                            
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete: {zigCache} ({FormatBytes(size)})");
                            }
                            else
                            {
                                Directory.Delete(zigCache, true);
                            }
                            clearedCount++;
                        }
                        catch { }
                    }
                }
            }
            catch { }

            // OCaml caches (opam)
            try
            {
                var ocamlCachePaths = new[]
                {
                    Path.Combine(userProfile, ".opam", "download-cache"),
                    Path.Combine(userProfile, ".opam", "log")
                };

                foreach (var ocamlCache in ocamlCachePaths)
                {
                    if (Directory.Exists(ocamlCache))
                    {
                        try
                        {
                            var size = GetDirectorySize(ocamlCache);
                            freedSpace += size;
                            
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete: {ocamlCache} ({FormatBytes(size)})");
                            }
                            else
                            {
                                Directory.Delete(ocamlCache, true);
                            }
                            clearedCount++;
                        }
                        catch { }
                    }
                }
            }
            catch { }

            if (clearedCount > 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"  ✅ {(_dryRun ? "Would clear" : "Cleared")} {clearedCount} programming language cache items ({FormatBytes(freedSpace)} {(_dryRun ? "would be freed" : "freed")})");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("  ⚠️ No programming language caches found");
                Console.ResetColor();
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  ❌ Failed: {ex.Message}");
            Console.ResetColor();
        }
    }

    static void ClearHyperVLogs()
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("\n🔹 Clearing Hyper-V logs and caches...");
        Console.ResetColor();

        long freedSpace = 0;
        int clearedCount = 0;

        try
        {
            var programData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

            // Hyper-V log paths
            var hypervLogPaths = new[]
            {
                @"C:\Windows\System32\LogFiles\Hyper-V",
                Path.Combine(programData, "Microsoft", "Windows", "Hyper-V"),
                @"C:\ProgramData\Microsoft\Windows\Hyper-V\Logs"
            };

            foreach (var logPath in hypervLogPaths)
            {
                if (Directory.Exists(logPath))
                {
                    try
                    {
                        var files = Directory.GetFiles(logPath, "*.log", SearchOption.AllDirectories)
                            .Concat(Directory.GetFiles(logPath, "*.etl", SearchOption.AllDirectories));

                        foreach (var file in files)
                        {
                            try
                            {
                                var fi = new FileInfo(file);
                                freedSpace += fi.Length;
                                if (!_dryRun) fi.Delete();
                                else LogDryRun($"Would delete: {file} ({FormatBytes(fi.Length)})");
                                clearedCount++;
                            }
                            catch { }
                        }
                    }
                    catch { }
                }
            }

            // Hyper-V VM automatic checkpoints logs (not the checkpoints themselves)
            var vmmsLogsPath = @"C:\ProgramData\Microsoft\Windows\Hyper-V\Virtual Machines";
            if (Directory.Exists(vmmsLogsPath))
            {
                try
                {
                    var files = Directory.GetFiles(vmmsLogsPath, "*.log", SearchOption.AllDirectories);
                    foreach (var file in files)
                    {
                        try
                        {
                            var fi = new FileInfo(file);
                            freedSpace += fi.Length;
                            if (!_dryRun) fi.Delete();
                            else LogDryRun($"Would delete: {file} ({FormatBytes(fi.Length)})");
                            clearedCount++;
                        }
                        catch { }
                    }
                }
                catch { }
            }

            if (clearedCount > 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                if (_dryRun)
                {
                    Console.WriteLine($"  ✅ Would clear {clearedCount} Hyper-V log items ({FormatBytes(freedSpace)} would be freed)");
                }
                else
                {
                    Console.WriteLine($"  ✅ {(_dryRun ? "Would clear" : "Cleared")} {clearedCount} Hyper-V log items ({FormatBytes(freedSpace)} {(_dryRun ? "would be freed" : "freed")})");
                }
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("  ⚠️ No Hyper-V logs found (Hyper-V may not be installed)");
                Console.ResetColor();
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  ❌ Failed: {ex.Message}");
            Console.ResetColor();
        }
    }

    static void ClearVirtualBoxData()
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("\n🔹 Clearing VirtualBox logs and caches...");
        Console.ResetColor();

        long freedSpace = 0;
        int clearedCount = 0;

        try
        {
            var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var programData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

            // VirtualBox paths
            var vboxPaths = new List<string>
            {
                Path.Combine(userProfile, ".VirtualBox", "VBoxSVC.log"),
                Path.Combine(userProfile, ".VirtualBox", "VBoxSVC.log.1"),
                Path.Combine(userProfile, ".VirtualBox", "VBoxSVC.log.2"),
                Path.Combine(userProfile, ".VirtualBox", "VBoxSVC.log.3"),
                Path.Combine(userProfile, "VirtualBox VMs")  // Log files in VM directories
            };

            // Add Program Files paths
            var vboxInstallPath = @"C:\Program Files\Oracle\VirtualBox";
            if (Directory.Exists(vboxInstallPath))
            {
                vboxPaths.Add(Path.Combine(vboxInstallPath, "logs"));
            }

            foreach (var path in vboxPaths)
            {
                if (File.Exists(path))
                {
                    try
                    {
                        var fi = new FileInfo(path);
                        freedSpace += fi.Length;
                        if (!_dryRun)
                        {
                            fi.Delete();
                        }
                        else
                        {
                            LogDryRun($"Would delete: {path} ({FormatBytes(fi.Length)})");
                        }
                        clearedCount++;
                    }
                    catch { }
                }
                else if (Directory.Exists(path))
                {
                    try
                    {
                        // Look for .log files in VM directories
                        var logFiles = Directory.GetFiles(path, "*.log", SearchOption.AllDirectories);
                        foreach (var logFile in logFiles)
                        {
                            try
                            {
                                var fi = new FileInfo(logFile);
                                freedSpace += fi.Length;
                                if (!_dryRun)
                                {
                                    fi.Delete();
                                }
                                else
                                {
                                    LogDryRun($"Would delete: {logFile} ({FormatBytes(fi.Length)})");
                                }
                                clearedCount++;
                            }
                            catch { }
                        }
                    }
                    catch { }
                }
            }

            if (clearedCount > 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"  ✅ {(_dryRun ? "Would clear" : "Cleared")} {clearedCount} VirtualBox log items ({FormatBytes(freedSpace)} {(_dryRun ? "would be freed" : "freed")})");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("  ⚠️ No VirtualBox logs found (VirtualBox may not be installed)");
                Console.ResetColor();
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  ❌ Failed: {ex.Message}");
            Console.ResetColor();
        }
    }

    static void ClearVMwareData()
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("\n🔹 Clearing VMware logs and caches...");
        Console.ResetColor();

        long freedSpace = 0;
        int clearedCount = 0;

        try
        {
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var programData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            var temp = Path.GetTempPath();

            // VMware paths
            var vmwarePaths = new[]
            {
                Path.Combine(appData, "VMware"),
                Path.Combine(localAppData, "VMware"),
                Path.Combine(programData, "VMware"),
                Path.Combine(temp, "vmware-*")
            };

            // Specific items to clean (logs, caches, not configs or VMs)
            var patterns = new[] { "*.log", "*.dmp", "vmware-*.log", "*.tmp" };

            foreach (var basePath in vmwarePaths)
            {
                if (basePath.Contains("*"))
                {
                    // Handle wildcard paths (temp directories)
                    var parentDir = Path.GetDirectoryName(basePath);
                    if (Directory.Exists(parentDir))
                    {
                        var pattern = Path.GetFileName(basePath);
                        var dirs = Directory.GetDirectories(parentDir, pattern);
                        foreach (var dir in dirs)
                        {
                            try
                            {
                                var size = GetDirectorySize(dir);
                                freedSpace += size;
                                if (!_dryRun)
                                {
                                    Directory.Delete(dir, true);
                                }
                                else
                                {
                                    LogDryRun($"Would delete: {dir} ({FormatBytes(size)})");
                                }
                                clearedCount++;
                            }
                            catch { }
                        }
                    }
                }
                else if (Directory.Exists(basePath))
                {
                    try
                    {
                        // Clean logs and temp files
                        foreach (var pattern in patterns)
                        {
                            var files = Directory.GetFiles(basePath, pattern, SearchOption.AllDirectories);
                            foreach (var file in files)
                            {
                                try
                                {
                                    var fi = new FileInfo(file);
                                    freedSpace += fi.Length;
                                    if (!_dryRun)
                                    {
                                        fi.Delete();
                                    }
                                    else
                                    {
                                        LogDryRun($"Would delete: {file} ({FormatBytes(fi.Length)})");
                                    }
                                    clearedCount++;
                                }
                                catch { }
                            }
                        }
                    }
                    catch { }
                }
            }

            if (clearedCount > 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"  ✅ {(_dryRun ? "Would clear" : "Cleared")} {clearedCount} VMware log/cache items ({FormatBytes(freedSpace)} {(_dryRun ? "would be freed" : "freed")})");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("  ⚠️ No VMware logs found (VMware may not be installed)");
                Console.ResetColor();
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  ❌ Failed: {ex.Message}");
            Console.ResetColor();
        }
    }

    static void ClearDeliveryOptimizationCache()
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("\n🔹 Clearing Windows Delivery Optimization cache...");
        Console.ResetColor();

        long freedSpace = 0;
        int clearedCount = 0;

        try
        {
            var deliveryOptPath = @"C:\Windows\ServiceProfiles\NetworkService\AppData\Local\Microsoft\Windows\DeliveryOptimization\Cache";

            if (Directory.Exists(deliveryOptPath))
            {
                try
                {
                    var size = GetDirectorySize(deliveryOptPath);
                    freedSpace = size;

                    if (!_dryRun)
                    {
                        // Clear contents but keep directory structure
                        var files = Directory.GetFiles(deliveryOptPath, "*.*", SearchOption.AllDirectories);
                        foreach (var file in files)
                        {
                            try
                            {
                                File.Delete(file);
                                clearedCount++;
                            }
                            catch { }
                        }

                        // Try to remove empty subdirectories
                        try
                        {
                            var dirs = Directory.GetDirectories(deliveryOptPath, "*", SearchOption.AllDirectories)
                                              .OrderByDescending(d => d.Length); // Delete deepest first
                            foreach (var dir in dirs)
                            {
                                try
                                {
                                    if (Directory.GetFileSystemEntries(dir).Length == 0)
                                    {
                                        Directory.Delete(dir);
                                    }
                                }
                                catch { }
                            }
                        }
                        catch { }
                    }
                    else
                    {
                        LogDryRun($"Would delete: {deliveryOptPath} - Delivery Optimization cache ({FormatBytes(size)})");
                        clearedCount = 1;
                    }

                    if (freedSpace > 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"  ✅ {(_dryRun ? "Would clear" : "Cleared")} Delivery Optimization cache ({FormatBytes(freedSpace)} {(_dryRun ? "would be freed" : "freed")})");
                        Console.ResetColor();
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"  ❌ Access denied (requires administrator privileges)");
                    Console.ResetColor();
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"  ❌ Failed: {ex.Message}");
                    Console.ResetColor();
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("  ⚠️ Delivery Optimization cache directory not found");
                Console.ResetColor();
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  ❌ Failed: {ex.Message}");
            Console.ResetColor();
        }
    }

    static void ClearEventLogs()
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("\n🔹 Clearing Windows Event Logs...");
        Console.ResetColor();

        int clearedCount = 0;
        var clearedLogs = new List<string>();

        try
        {
            if (!IsRunAsAdministrator())
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("  ❌ Requires administrator privileges to clear event logs");
                Console.ResetColor();
                return;
            }

            var eventLogs = System.Diagnostics.EventLog.GetEventLogs();

            LogDryRun("\n=== WINDOWS EVENT LOGS ===");

            foreach (var log in eventLogs)
            {
                try
                {
                    var logName = log.Log;
                    var entryCount = log.Entries.Count;

                    if (entryCount > 0)
                    {
                        if (!_dryRun)
                        {
                            log.Clear();
                            clearedLogs.Add(logName);
                        }
                        else
                        {
                            LogDryRun($"Would delete: Event log {logName} ({entryCount} entries)");
                            clearedLogs.Add(logName);
                        }
                        clearedCount++;
                    }
                }
                catch (Exception ex)
                {
                    // Some logs may be protected or in use, skip them
                    LogDryRun($"Could not clear log {log.Log}: {ex.Message}");
                }
            }

            if (clearedCount > 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"  ✅ {(_dryRun ? "Would clear" : "Cleared")} {clearedCount} event logs");
                Console.ResetColor();

                if (clearedLogs.Count > 0 && clearedLogs.Count <= 10)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine($"     Logs: {string.Join(", ", clearedLogs)}");
                    Console.ResetColor();
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("  ⚠️ No event logs to clear");
                Console.ResetColor();
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  ❌ Failed: {ex.Message}");
            Console.ResetColor();
        }
    }

    static void ClearGraphicsDriverCaches()
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("\n🔹 Clearing graphics driver caches and logs...");
        Console.ResetColor();

        long freedSpace = 0;
        int clearedCount = 0;

        try
        {
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var programData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

            // NVIDIA shader cache and logs
            var nvidiaPaths = new[]
            {
                Path.Combine(localAppData, "NVIDIA", "DXCache"),
                Path.Combine(localAppData, "NVIDIA", "GLCache"),
                Path.Combine(localAppData, "NVIDIA Corporation", "NvBackend", "ApplicationOntology", "logs"),
                Path.Combine(programData, "NVIDIA Corporation", "NvBackend", "logs"),
                Path.Combine(programData, "NVIDIA", "DisplayDriver", "*.log"),
                @"C:\Windows\System32\DriverStore\FileRepository\nv*.inf*\LogFiles"
            };

            foreach (var nvidiaPath in nvidiaPaths)
            {
                if (Directory.Exists(nvidiaPath))
                {
                    try
                    {
                        var size = GetDirectorySize(nvidiaPath);
                        if (_dryRun)
                        {
                            LogDryRun($"Would delete: {nvidiaPath} ({FormatBytes(size)})");
                        }
                        else
                        {
                            Directory.Delete(nvidiaPath, true);
                        }
                        freedSpace += size;
                        clearedCount++;
                    }
                    catch { }
                }
            }

            // AMD shader cache and logs
            var amdPaths = new[]
            {
                Path.Combine(localAppData, "AMD", "DxCache"),
                Path.Combine(localAppData, "AMD", "VkCache"),
                Path.Combine(localAppData, "AMD", "CN", "Reports"),
                Path.Combine(programData, "AMD", "PPC", "Logs")
            };

            foreach (var amdPath in amdPaths)
            {
                if (Directory.Exists(amdPath))
                {
                    try
                    {
                        var size = GetDirectorySize(amdPath);
                        if (_dryRun)
                        {
                            LogDryRun($"Would delete: {amdPath} ({FormatBytes(size)})");
                        }
                        else
                        {
                            Directory.Delete(amdPath, true);
                        }
                        freedSpace += size;
                        clearedCount++;
                    }
                    catch { }
                }
            }

            // Intel graphics cache and logs
            var intelPaths = new[]
            {
                Path.Combine(localAppData, "Intel", "ShaderCache"),
                Path.Combine(programData, "Intel", "Logs")
            };

            foreach (var intelPath in intelPaths)
            {
                if (Directory.Exists(intelPath))
                {
                    try
                    {
                        var size = GetDirectorySize(intelPath);
                        if (_dryRun)
                        {
                            LogDryRun($"Would delete: {intelPath} ({FormatBytes(size)})");
                        }
                        else
                        {
                            Directory.Delete(intelPath, true);
                        }
                        freedSpace += size;
                        clearedCount++;
                    }
                    catch { }
                }
            }

            // Qualcomm Snapdragon (Adreno GPU) shader cache and logs
            var snapdragonPaths = new[]
            {
                Path.Combine(localAppData, "Qualcomm", "Adreno", "ShaderCache"),
                Path.Combine(localAppData, "Qualcomm", "Adreno", "Cache"),
                Path.Combine(localAppData, "Qualcomm", "logs"),
                Path.Combine(programData, "Qualcomm", "logs"),
                Path.Combine(programData, "Qualcomm", "Adreno", "logs")
            };

            foreach (var snapdragonPath in snapdragonPaths)
            {
                if (Directory.Exists(snapdragonPath))
                {
                    try
                    {
                        var size = GetDirectorySize(snapdragonPath);
                        if (_dryRun)
                        {
                            LogDryRun($"Would delete: {snapdragonPath} ({FormatBytes(size)})");
                        }
                        else
                        {
                            Directory.Delete(snapdragonPath, true);
                        }
                        freedSpace += size;
                        clearedCount++;
                    }
                    catch { }
                }
            }

            // Snapdragon log files
            var snapdragonLogPaths = new[]
            {
                Path.Combine(localAppData, "Qualcomm"),
                Path.Combine(programData, "Qualcomm")
            };

            foreach (var snapdragonLogPath in snapdragonLogPaths)
            {
                if (Directory.Exists(snapdragonLogPath))
                {
                    try
                    {
                        var logFiles = Directory.GetFiles(snapdragonLogPath, "*.log", SearchOption.AllDirectories);
                        foreach (var file in logFiles)
                        {
                            try
                            {
                                var fileInfo = new FileInfo(file);
                                freedSpace += fileInfo.Length;
                                if (_dryRun)
                                {
                                    LogDryRun($"Would delete file: {file}");
                                }
                                else
                                {
                                    fileInfo.Delete();
                                }
                                clearedCount++;
                            }
                            catch { }
                        }
                    }
                    catch { }
                }
            }

            if (clearedCount > 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                if (_dryRun)
                {
                    Console.WriteLine($"  ✅ Would clear {clearedCount} graphics driver items ({FormatBytes(freedSpace)} would be freed)");
                }
                else
                {
                    Console.WriteLine($"  ✅ {(_dryRun ? "Would clear" : "Cleared")} {clearedCount} graphics driver items ({FormatBytes(freedSpace)} {(_dryRun ? "would be freed" : "freed")})");
                }
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("  ⚠️ No graphics driver caches found");
                Console.ResetColor();
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  ❌ Failed: {ex.Message}");
            Console.ResetColor();
        }
    }

    static void ClearWindowsSubsystems()
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("\n🔹 Clearing Windows subsystem logs and caches...");
        Console.ResetColor();

        long freedSpace = 0;
        int clearedCount = 0;

        try
        {
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var programData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            // Windows Terminal logs and cache
            var terminalPaths = new[]
            {
                Path.Combine(localAppData, "Packages", "Microsoft.WindowsTerminal_8wekyb3d8bbwe", "LocalState", "TabBackup"),
                Path.Combine(localAppData, "Packages", "Microsoft.WindowsTerminal_8wekyb3d8bbwe", "TempState"),
                Path.Combine(localAppData, "Packages", "Microsoft.WindowsTerminalPreview_8wekyb3d8bbwe", "TempState")
            };

            foreach (var terminalPath in terminalPaths)
            {
                if (Directory.Exists(terminalPath))
                {
                    try
                    {
                        var size = GetDirectorySize(terminalPath);
                        if (_dryRun)
                        {
                            LogDryRun($"Would delete: {terminalPath} ({FormatBytes(size)})");
                        }
                        else
                        {
                            Directory.Delete(terminalPath, true);
                        }
                        freedSpace += size;
                        clearedCount++;
                    }
                    catch { }
                }
            }

            // Windows Insider Program logs
            var insiderLogsPath = @"C:\Windows\Logs\WindowsUpdate\InsiderData";
            if (Directory.Exists(insiderLogsPath))
            {
                try
                {
                    var files = Directory.GetFiles(insiderLogsPath, "*.*", SearchOption.AllDirectories);
                    foreach (var file in files)
                    {
                        try
                        {
                            var fi = new FileInfo(file);
                            freedSpace += fi.Length;
                            if (!_dryRun) fi.Delete();
                            else LogDryRun($"Would delete: {file}");
                            clearedCount++;
                        }
                        catch { }
                    }
                }
                catch { }
            }

            // Windows Compatibility Telemetry
            var diagTrackPath = @"C:\ProgramData\Microsoft\Diagnosis\ETLLogs\AutoLogger";
            if (Directory.Exists(diagTrackPath))
            {
                try
                {
                    var files = Directory.GetFiles(diagTrackPath, "*.etl", SearchOption.AllDirectories);
                    foreach (var file in files)
                    {
                        try
                        {
                            var fi = new FileInfo(file);
                            freedSpace += fi.Length;
                            if (!_dryRun) fi.Delete();
                            else LogDryRun($"Would delete: {file}");
                            clearedCount++;
                        }
                        catch { }
                    }
                }
                catch { }
            }

            // DirectX installation logs
            var dxSetupLogPath = @"C:\Windows\DirectX.log";
            if (File.Exists(dxSetupLogPath))
            {
                try
                {
                    var fi = new FileInfo(dxSetupLogPath);
                    freedSpace += fi.Length;
                    if (!_dryRun) fi.Delete();
                    else LogDryRun($"Would delete: {dxSetupLogPath}");
                    clearedCount++;
                }
                catch { }
            }

            // Windows Activation logs
            var slmgrLogsPath = @"C:\Windows\System32\Licensing\TrustedStore";
            if (Directory.Exists(slmgrLogsPath))
            {
                try
                {
                    var files = Directory.GetFiles(slmgrLogsPath, "*.log", SearchOption.AllDirectories);
                    foreach (var file in files)
                    {
                        try
                        {
                            var fi = new FileInfo(file);
                            freedSpace += fi.Length;
                            if (!_dryRun) fi.Delete();
                            else LogDryRun($"Would delete: {file}");
                            clearedCount++;
                        }
                        catch { }
                    }
                }
                catch { }
            }

            // Windows Performance Analyzer/Recorder traces
            var wprTracesPath = @"C:\Windows\System32\WDI";
            if (Directory.Exists(wprTracesPath))
            {
                try
                {
                    var files = Directory.GetFiles(wprTracesPath, "*.etl", SearchOption.AllDirectories);
                    foreach (var file in files)
                    {
                        try
                        {
                            var fi = new FileInfo(file);
                            freedSpace += fi.Length;
                            if (!_dryRun) fi.Delete();
                            else LogDryRun($"Would delete: {file}");
                            clearedCount++;
                        }
                        catch { }
                    }
                }
                catch { }
            }

            // Reliability Monitor logs
            var reliabilityPath = @"C:\ProgramData\Microsoft\RAC\StateData";
            if (Directory.Exists(reliabilityPath))
            {
                try
                {
                    var files = Directory.GetFiles(reliabilityPath, "*.dat", SearchOption.AllDirectories);
                    foreach (var file in files)
                    {
                        try
                        {
                            var fi = new FileInfo(file);
                            freedSpace += fi.Length;
                            if (!_dryRun) fi.Delete();
                            else LogDryRun($"Would delete: {file}");
                            clearedCount++;
                        }
                        catch { }
                    }
                }
                catch { }
            }

            // COM+ event logs
            var comPlusLogsPath = @"C:\Windows\System32\Com\dmp";
            if (Directory.Exists(comPlusLogsPath))
            {
                try
                {
                    var files = Directory.GetFiles(comPlusLogsPath, "*.dmp", SearchOption.AllDirectories);
                    foreach (var file in files)
                    {
                        try
                        {
                            var fi = new FileInfo(file);
                            freedSpace += fi.Length;
                            if (!_dryRun) fi.Delete();
                            else LogDryRun($"Would delete: {file}");
                            clearedCount++;
                        }
                        catch { }
                    }
                }
                catch { }
            }

            // IIS logs (if installed)
            var iisLogsPath = @"C:\inetpub\logs\LogFiles";
            if (Directory.Exists(iisLogsPath))
            {
                try
                {
                    var files = Directory.GetFiles(iisLogsPath, "*.log", SearchOption.AllDirectories);
                    foreach (var file in files)
                    {
                        try
                        {
                            var fi = new FileInfo(file);
                            freedSpace += fi.Length;
                            if (!_dryRun) fi.Delete();
                            else LogDryRun($"Would delete: {file}");
                            clearedCount++;
                        }
                        catch { }
                    }
                }
                catch { }
            }

            // SQL Server error logs (if installed)
            var sqlServerLogPaths = new[]
            {
                @"C:\Program Files\Microsoft SQL Server\MSSQL*\MSSQL\Log\ERRORLOG.*",
                @"C:\Program Files (x86)\Microsoft SQL Server\MSSQL*\MSSQL\Log\ERRORLOG.*"
            };

            foreach (var sqlPattern in sqlServerLogPaths)
            {
                try
                {
                    var parentDir = Path.GetDirectoryName(sqlPattern);
                    if (parentDir != null && Directory.Exists(Path.GetDirectoryName(parentDir)))
                    {
                        var files = Directory.GetFiles(Path.GetDirectoryName(parentDir), Path.GetFileName(sqlPattern), SearchOption.AllDirectories);
                        foreach (var file in files)
                        {
                            try
                            {
                                var fi = new FileInfo(file);
                                freedSpace += fi.Length;
                                if (!_dryRun) fi.Delete();
                                else LogDryRun($"Would delete: {file}");
                                clearedCount++;
                            }
                            catch { }
                        }
                    }
                }
                catch { }
            }

            if (clearedCount > 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                if (_dryRun)
                {
                    Console.WriteLine($"  ✅ Would clear {clearedCount} subsystem items ({FormatBytes(freedSpace)} would be freed)");
                }
                else
                {
                    Console.WriteLine($"  ✅ {(_dryRun ? "Would clear" : "Cleared")} {clearedCount} subsystem items ({FormatBytes(freedSpace)} {(_dryRun ? "would be freed" : "freed")})");
                }
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("  ⚠️ No subsystem logs found");
                Console.ResetColor();
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  ❌ Failed: {ex.Message}");
            Console.ResetColor();
        }
    }

    static void ClearDeepSystemCaches()
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("\n🔹 Clearing deep system caches...");
        Console.ResetColor();

        int clearedCount = 0;
        long freedSpace = 0;

        try
        {
            // Clear ARM64 Prism emulation cache (x86/x64 to ARM translation cache)
            var prismCache = @"C:\Windows\XtaCache";
            if (Directory.Exists(prismCache))
            {
                try
                {
                    var size = GetDirectorySize(prismCache);
                    if (!_dryRun)
                    {
                        // Clear contents but keep the directory
                        foreach (var file in Directory.GetFiles(prismCache, "*.*", SearchOption.AllDirectories))
                        {
                            try { File.Delete(file); } catch { }
                        }
                        foreach (var dir in Directory.GetDirectories(prismCache))
                        {
                            try { Directory.Delete(dir, true); } catch { }
                        }
                    }
                    freedSpace += size;
                    clearedCount++;
                    if (_dryRun)
                    {
                        LogDryRun($"Would delete: {prismCache} - Prism x86-to-ARM cache ({FormatBytes(size)})");
                    }
                }
                catch { }
            }

            // Clear Windows Error Reporting (crash dumps and reports)
            var werPaths = new[]
            {
                @"C:\ProgramData\Microsoft\Windows\WER\ReportQueue",
                @"C:\ProgramData\Microsoft\Windows\WER\ReportArchive",
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CrashDumps")
            };
            foreach (var werPath in werPaths)
            {
                if (Directory.Exists(werPath))
                {
                    try
                    {
                        var files = Directory.GetFiles(werPath, "*.*", SearchOption.AllDirectories);
                        foreach (var file in files)
                        {
                            try
                            {
                                var fi = new FileInfo(file);
                                freedSpace += fi.Length;
                                if (!_dryRun)
                                {
                                    fi.Delete();
                                }
                                if (_dryRun)
                                {
                                    LogDryRun($"Would delete: {file} ({FormatBytes(fi.Length)})");
                                }
                            }
                            catch { }
                        }
                        clearedCount++;
                    }
                    catch { }
                }
            }

            // Clear Windows Minidumps
            var minidumpPath = @"C:\Windows\Minidump";
            if (Directory.Exists(minidumpPath))
            {
                try
                {
                    var files = Directory.GetFiles(minidumpPath, "*.dmp");
                    foreach (var file in files)
                    {
                        try
                        {
                            var fi = new FileInfo(file);
                            freedSpace += fi.Length;
                            if (!_dryRun)
                            {
                                fi.Delete();
                            }
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete: {file} ({FormatBytes(fi.Length)})");
                            }
                        }
                        catch { }
                    }
                    clearedCount++;
                }
                catch { }
            }

            // Clear MEMORY.DMP (main memory dump file - can be very large)
            var memoryDmpPath = @"C:\Windows\MEMORY.DMP";
            if (File.Exists(memoryDmpPath))
            {
                try
                {
                    var fi = new FileInfo(memoryDmpPath);
                    freedSpace += fi.Length;
                    if (_dryRun)
                    {
                        LogDryRun($"Would delete: {memoryDmpPath} ({FormatBytes(fi.Length)})");
                    }
                    else
                    {
                        fi.Delete();
                    }
                    clearedCount++;
                }
                catch { }
            }

            // Clear Live Kernel Reports (more memory dumps)
            var liveKernelPath = @"C:\Windows\LiveKernelReports";
            if (Directory.Exists(liveKernelPath))
            {
                try
                {
                    var files = Directory.GetFiles(liveKernelPath, "*.*", SearchOption.AllDirectories);
                    foreach (var file in files)
                    {
                        try
                        {
                            var fi = new FileInfo(file);
                            freedSpace += fi.Length;
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete: {file} ({FormatBytes(fi.Length)})");
                            }
                            else
                            {
                                fi.Delete();
                            }
                        }
                        catch { }
                    }
                    clearedCount++;
                }
                catch { }
            }

            // Clear Windows Performance Recorder traces (ETL files - ALL logs)
            var perfTracePaths = new[]
            {
                @"C:\Windows\System32\LogFiles\WMI",
                @"C:\ProgramData\Microsoft\Windows\WER\Temp"
            };
            
            foreach (var tracePath in perfTracePaths)
            {
                if (Directory.Exists(tracePath))
                {
                    try
                    {
                        var etlFiles = Directory.GetFiles(tracePath, "*.etl", SearchOption.AllDirectories);
                        foreach (var file in etlFiles)
                        {
                            try
                            {
                                var fi = new FileInfo(file);
                                freedSpace += fi.Length;
                                if (_dryRun)
                                {
                                    LogDryRun($"Would delete: {file} ({FormatBytes(fi.Length)})");
                                }
                                else
                                {
                                    fi.Delete();
                                }
                            }
                            catch { }
                        }
                        clearedCount++;
                    }
                    catch { }
                }
            }

            // Clear DirectX Shader Cache
            var dxCache = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "D3DSCache");
            if (Directory.Exists(dxCache))
            {
                try
                {
                    var size = GetDirectorySize(dxCache);
                    if (!_dryRun)
                    {
                        Directory.Delete(dxCache, true);
                    }
                    freedSpace += size;
                    clearedCount++;
                    if (_dryRun)
                    {
                        LogDryRun($"Would delete: {dxCache} (DirectX Shader Cache) ({FormatBytes(size)})");
                    }
                }
                catch { }
            }

            // Clear Windows Timeline/Activity History
            var activityCache = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "ConnectedDevicesPlatform");
            if (Directory.Exists(activityCache))
            {
                try
                {
                    var dbFiles = Directory.GetFiles(activityCache, "*.db", SearchOption.AllDirectories);
                    foreach (var db in dbFiles)
                    {
                        try
                        {
                            var fi = new FileInfo(db);
                            freedSpace += fi.Length;
                            if (!_dryRun)
                            {
                                fi.Delete();
                            }
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete: {db} (Timeline activity) ({FormatBytes(fi.Length)})");
                            }
                        }
                        catch { }
                    }
                    clearedCount++;
                }
                catch { }
            }

            // Clear CBS (Component-Based Servicing) logs
            var cbsLogs = @"C:\Windows\Logs\CBS";
            if (Directory.Exists(cbsLogs))
            {
                try
                {
                    var logFiles = Directory.GetFiles(cbsLogs, "*.log");
                    foreach (var log in logFiles)
                    {
                        try
                        {
                            // Keep the most recent CBS.log, delete older ones
                            if (!log.EndsWith("CBS.log", StringComparison.OrdinalIgnoreCase))
                            {
                                var fi = new FileInfo(log);
                                freedSpace += fi.Length;
                                if (!_dryRun)
                                {
                                    fi.Delete();
                                }
                                if (_dryRun)
                                {
                                    LogDryRun($"Would delete: {log} ({FormatBytes(fi.Length)})");
                                }
                            }
                        }
                        catch { }
                    }
                    clearedCount++;
                }
                catch { }
            }

            // Clear DISM logs
            var dismLogs = @"C:\Windows\Logs\DISM";
            if (Directory.Exists(dismLogs))
            {
                try
                {
                    var size = GetDirectorySize(dismLogs);
                    if (!_dryRun)
                    {
                        foreach (var file in Directory.GetFiles(dismLogs, "*.log"))
                        {
                            try { File.Delete(file); } catch { }
                        }
                    }
                    freedSpace += size;
                    clearedCount++;
                    if (_dryRun)
                    {
                        LogDryRun($"Would delete: {dismLogs} ({FormatBytes(size)})");
                    }
                }
                catch { }
            }

            // Clear Windows Update logs
            var wuLogs = @"C:\Windows\Logs\WindowsUpdate";
            if (Directory.Exists(wuLogs))
            {
                try
                {
                    var logFiles = Directory.GetFiles(wuLogs, "*.etl");
                    foreach (var log in logFiles)
                    {
                        try
                        {
                            var fi = new FileInfo(log);
                            freedSpace += fi.Length;
                            if (!_dryRun)
                            {
                                fi.Delete();
                            }
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete: {log} ({FormatBytes(fi.Length)})");
                            }
                        }
                        catch { }
                    }
                    clearedCount++;
                }
                catch { }
            }

            // Clear SetupAPI device installation logs
            var setupApiLogs = @"C:\Windows\INF";
            if (Directory.Exists(setupApiLogs))
            {
                try
                {
                    var logFiles = Directory.GetFiles(setupApiLogs, "setupapi*.log");
                    foreach (var log in logFiles)
                    {
                        try
                        {
                            var fi = new FileInfo(log);
                            freedSpace += fi.Length;
                            if (!_dryRun)
                            {
                                fi.Delete();
                            }
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete: {log} (old setup log) ({FormatBytes(fi.Length)})");
                            }
                        }
                        catch { }
                    }
                    clearedCount++;
                }
                catch { }
            }

            // Clear Cryptnet URL Cache (certificate validation cache)
            var cryptnetCache = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Microsoft", "Windows", "INetCache");
            if (Directory.Exists(cryptnetCache))
            {
                try
                {
                    var files = Directory.GetFiles(cryptnetCache, "*.*", SearchOption.AllDirectories);
                    foreach (var file in files)
                    {
                        try
                        {
                            var fi = new FileInfo(file);
                            freedSpace += fi.Length;
                            if (!_dryRun)
                            {
                                fi.Delete();
                            }
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete: {file} ({FormatBytes(fi.Length)})");
                            }
                        }
                        catch { }
                    }
                    clearedCount++;
                }
                catch { }
            }

            // Clear Windows Defender scan history
            var defenderScans = @"C:\ProgramData\Microsoft\Windows Defender\Scans\History";
            if (Directory.Exists(defenderScans))
            {
                try
                {
                    var size = GetDirectorySize(defenderScans);
                    if (!_dryRun)
                    {
                        foreach (var dir in Directory.GetDirectories(defenderScans))
                        {
                            try { Directory.Delete(dir, true); } catch { }
                        }
                    }
                    freedSpace += size;
                    clearedCount++;
                    if (_dryRun)
                    {
                        LogDryRun($"Would delete: {defenderScans} ({FormatBytes(size)})");
                    }
                }
                catch { }
            }

            // Clear Delivery Optimization cache
            var deliveryOptimization = Path.Combine(Environment.GetEnvironmentVariable("WINDIR") ?? "C:\\Windows",
                "SoftwareDistribution", "DeliveryOptimization");
            if (Directory.Exists(deliveryOptimization))
            {
                try
                {
                    var size = GetDirectorySize(deliveryOptimization);
                    if (!_dryRun)
                    {
                        foreach (var file in Directory.GetFiles(deliveryOptimization, "*.*", SearchOption.AllDirectories))
                        {
                            try { File.Delete(file); } catch { }
                        }
                    }
                    freedSpace += size;
                    clearedCount++;
                    if (_dryRun)
                    {
                        LogDryRun($"Would clear: {deliveryOptimization} ({FormatBytes(size)})");
                    }
                }
                catch { }
            }

            // Clear BITS (Background Intelligent Transfer Service) database
            var bitsDb = @"C:\ProgramData\Microsoft\Network\Downloader";
            if (Directory.Exists(bitsDb))
            {
                try
                {
                    var files = Directory.GetFiles(bitsDb, "*.dat", SearchOption.AllDirectories);
                    foreach (var file in files)
                    {
                        try
                        {
                            var fi = new FileInfo(file);
                            freedSpace += fi.Length;
                            if (!_dryRun)
                            {
                                fi.Delete();
                            }
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete: {file} (BITS database) ({FormatBytes(fi.Length)})");
                            }
                        }
                        catch { }
                    }
                    clearedCount++;
                }
                catch { }
            }

            // Clear Windows Performance Recorder traces
            var wprTraces = @"C:\Windows\System32\LogFiles\WMI";
            if (Directory.Exists(wprTraces))
            {
                try
                {
                    var files = Directory.GetFiles(wprTraces, "*.etl", SearchOption.AllDirectories);
                    foreach (var file in files)
                    {
                        try
                        {
                            var fi = new FileInfo(file);
                            freedSpace += fi.Length;
                            if (!_dryRun)
                            {
                                fi.Delete();
                            }
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete: {file} (Performance trace) ({FormatBytes(fi.Length)})");
                            }
                        }
                        catch { }
                    }
                    clearedCount++;
                }
                catch { }
            }

            // Clear Windows Application Compatibility cache
            var appCompatCache = @"C:\Windows\AppCompat\Programs";
            if (Directory.Exists(appCompatCache))
            {
                try
                {
                    var files = Directory.GetFiles(appCompatCache, "*.txt", SearchOption.AllDirectories);
                    foreach (var file in files)
                    {
                        try
                        {
                            var fi = new FileInfo(file);
                            freedSpace += fi.Length;
                            if (!_dryRun)
                            {
                                fi.Delete();
                            }
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete: {file} ({FormatBytes(fi.Length)})");
                            }
                        }
                        catch { }
                    }
                    clearedCount++;
                }
                catch { }
            }

            // Clear System Resource Usage Monitor (SRUM) database backup
            var srumBackup = @"C:\Windows\System32\sru";
            if (Directory.Exists(srumBackup))
            {
                try
                {
                    var files = Directory.GetFiles(srumBackup, "*.bak", SearchOption.TopDirectoryOnly);
                    foreach (var file in files)
                    {
                        try
                        {
                            var fi = new FileInfo(file);
                            freedSpace += fi.Length;
                            if (!_dryRun)
                            {
                                fi.Delete();
                            }
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete: {file} (SRUM backup) ({FormatBytes(fi.Length)})");
                            }
                        }
                        catch { }
                    }
                    clearedCount++;
                }
                catch { }
            }

            // Clear Windows Notification Database
            var notificationDb = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Microsoft", "Windows", "Notifications");
            if (Directory.Exists(notificationDb))
            {
                try
                {
                    var files = Directory.GetFiles(notificationDb, "*.db", SearchOption.AllDirectories);
                    foreach (var file in files)
                    {
                        try
                        {
                            var fi = new FileInfo(file);
                            freedSpace += fi.Length;
                            if (!_dryRun)
                            {
                                fi.Delete();
                            }
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete: {file} (Notification history) ({FormatBytes(fi.Length)})");
                            }
                        }
                        catch { }
                    }
                    clearedCount++;
                }
                catch { }
            }

            // Clear Windows Installer rollback files
            var installerRollback = @"C:\Windows\Installer\$PatchCache$";
            if (Directory.Exists(installerRollback))
            {
                try
                {
                    var size = GetDirectorySize(installerRollback);
                    if (size > 100 * 1024 * 1024) // Only if > 100MB
                    {
                        if (!_dryRun)
                        {
                            foreach (var file in Directory.GetFiles(installerRollback, "*.*", SearchOption.AllDirectories))
                            {
                                try 
                                { 
                                    var fi = new FileInfo(file);
                                    fi.Delete();
                                } 
                                catch { }
                            }
                        }
                        freedSpace += size;
                        clearedCount++;
                        if (_dryRun)
                        {
                            LogDryRun($"Would clear: {installerRollback} (old rollback files) ({FormatBytes(size)})");
                        }
                    }
                }
                catch { }
            }

            // Clear WinSxS backup files (component store backup)
            var winsxsBackup = @"C:\Windows\WinSxS\Backup";
            if (Directory.Exists(winsxsBackup))
            {
                try
                {
                    var size = GetDirectorySize(winsxsBackup);
                    if (!_dryRun)
                    {
                        foreach (var file in Directory.GetFiles(winsxsBackup, "*.*", SearchOption.AllDirectories))
                        {
                            try { File.Delete(file); } catch { }
                        }
                    }
                    freedSpace += size;
                    clearedCount++;
                    if (_dryRun)
                    {
                        LogDryRun($"Would clear: {winsxsBackup} ({FormatBytes(size)})");
                    }
                }
                catch { }
            }

            // Clear Event Viewer logs (Application, System - but not Security)
            try
            {
                var logsToSave = new[] { "Security" }; // Don't clear security logs
                var logs = new[] { "Application", "System", "Setup" };
                
                foreach (var logName in logs)
                {
                    try
                    {
                        if (!_dryRun)
                        {
                            var psi = new ProcessStartInfo
                            {
                                FileName = "wevtutil.exe",
                                Arguments = $"cl {logName}",
                                UseShellExecute = false,
                                CreateNoWindow = true
                            };
                            Process.Start(psi)?.WaitForExit();
                        }
                        if (_dryRun)
                        {
                            LogDryRun($"Would clear Event Log: {logName}");
                        }
                        clearedCount++;
                    }
                    catch { }
                }
            }
            catch { }

            Console.ForegroundColor = ConsoleColor.Green;
            if (_dryRun)
            {
                Console.WriteLine($"  ✅ Would clear {clearedCount} deep system cache items ({FormatBytes(freedSpace)} freed)");
            }
            else
            {
                Console.WriteLine($"  ✅ {(_dryRun ? "Would clear" : "Cleared")} {clearedCount} deep system cache items ({FormatBytes(freedSpace)} {(_dryRun ? "would be freed" : "freed")})");
            }
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  ❌ Failed: {ex.Message}");
            Console.ResetColor();
        }
    }

    static void ClearProgramFilesAppData()
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("\n🔹 Scanning Program Files for application logs and caches...");
        Console.ResetColor();

        long freedSpace = 0;
        int clearedCount = 0;
        int appsScanned = 0;

        try
        {
            // Program Files directories to scan
            var programFilesDirs = new[]
            {
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
            };

            // Common folder names that contain logs/caches/temp files
            var targetFolderNames = new[] 
            { 
                "logs", "log", "cache", "caches", "temp", "tmp", "temporary",
                "crashdumps", "crash_reports", "diagnostics", "debug",
                "telemetry", "analytics", "reports", "dumps"
            };

            // File patterns to clean
            var filePatterns = new[] { "*.log", "*.tmp", "*.temp", "*.dmp", "*.trace", "*.etl" };

            foreach (var programFilesDir in programFilesDirs)
            {
                if (!Directory.Exists(programFilesDir))
                    continue;

                Console.WriteLine($"  Scanning {Path.GetFileName(programFilesDir)}...");

                try
                {
                    var appDirs = Directory.GetDirectories(programFilesDir);
                    foreach (var appDir in appDirs)
                    {
                        try
                        {
                            appsScanned++;
                            var appName = Path.GetFileName(appDir);

                            // Skip system-critical directories
                            if (appName.Equals("Windows Defender", StringComparison.OrdinalIgnoreCase) ||
                                appName.Equals("Windows Defender Advanced Threat Protection", StringComparison.OrdinalIgnoreCase) ||
                                appName.Equals("Windows NT", StringComparison.OrdinalIgnoreCase) ||
                                appName.Equals("WindowsApps", StringComparison.OrdinalIgnoreCase) ||
                                appName.Equals("Common Files", StringComparison.OrdinalIgnoreCase))
                            {
                                continue;
                            }

                            // Scan for target folders
                            var subdirs = Directory.GetDirectories(appDir, "*", SearchOption.AllDirectories);
                            foreach (var subdir in subdirs)
                            {
                                var subdirName = Path.GetFileName(subdir).ToLowerInvariant();
                                
                                // Check if this directory matches our target folder names
                                if (targetFolderNames.Contains(subdirName))
                                {
                                    try
                                    {
                                        var files = Directory.GetFiles(subdir, "*.*", SearchOption.AllDirectories);
                                        if (files.Length > 0)
                                        {
                                            foreach (var file in files)
                                            {
                                                try
                                                {
                                                    var fileInfo = new FileInfo(file);

                                                    if (_dryRun)
                                                    {
                                                        freedSpace += fileInfo.Length;
                                                        LogDryRun($"Would delete: {file} ({FormatBytes(fileInfo.Length)})");
                                                        clearedCount++;
                                                    }
                                                    else
                                                    {
                                                        freedSpace += fileInfo.Length;
                                                        fileInfo.Delete();
                                                        clearedCount++;
                                                    }
                                                }
                                                catch { /* Skip files in use or protected */ }
                                            }
                                        }
                                    }
                                    catch { /* Skip inaccessible directories */ }
                                }
                            }

                            // Also scan for loose log/temp files in the app root
                            foreach (var pattern in filePatterns)
                            {
                                try
                                {
                                    var files = Directory.GetFiles(appDir, pattern, SearchOption.TopDirectoryOnly);
                                    foreach (var file in files)
                                    {
                                        try
                                        {
                                            var fileInfo = new FileInfo(file);

                                            if (_dryRun)
                                            {
                                                freedSpace += fileInfo.Length;
                                                LogDryRun($"Would delete: {file} ({FormatBytes(fileInfo.Length)})");
                                                clearedCount++;
                                            }
                                            else
                                            {
                                                freedSpace += fileInfo.Length;
                                                fileInfo.Delete();
                                                clearedCount++;
                                            }
                                        }
                                        catch { /* Skip files in use */ }
                                    }
                                }
                                catch { /* Skip pattern search errors */ }
                            }
                        }
                        catch { /* Skip apps we can't access */ }
                    }
                }
                catch { /* Skip if directory enumeration fails */ }
            }

            if (clearedCount > 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                if (_dryRun)
                {
                    Console.WriteLine($"  ✅ Would clear {clearedCount} files from {appsScanned} apps ({FormatBytes(freedSpace)} would be freed)");
                }
                else
                {
                    Console.WriteLine($"  ✅ {(_dryRun ? "Would clear" : "Cleared")} {clearedCount} files from {appsScanned} apps ({FormatBytes(freedSpace)} {(_dryRun ? "would be freed" : "freed")})");
                }
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine($"  ⚠️ Scanned {appsScanned} apps, no cleanable files found");
                Console.ResetColor();
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  ❌ Failed: {ex.Message}");
            Console.ResetColor();
        }
    }

    static void ClearSystemLogs()
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("\n🔹 Clearing system logs and diagnostics...");
        Console.ResetColor();

        long freedSpace = 0;
        int clearedCount = 0;

        try
        {
            // Clear Prefetch files
            var prefetchPath = @"C:\Windows\Prefetch";
            if (Directory.Exists(prefetchPath))
            {
                try
                {
                    var files = Directory.GetFiles(prefetchPath, "*.pf");
                    foreach (var file in files)
                    {
                        try
                        {
                            var fileInfo = new FileInfo(file);
                            freedSpace += fileInfo.Length;
                            fileInfo.Delete();
                            clearedCount++;
                        }
                        catch { }
                    }
                }
                catch { }
            }

            // Clear crash dumps
            var crashDumpPaths = new[]
            {
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CrashDumps"),
                @"C:\Windows\Minidump",
                @"C:\Windows\LiveKernelReports"
            };

            foreach (var path in crashDumpPaths)
            {
                if (Directory.Exists(path))
                {
                    try
                    {
                        var files = Directory.GetFiles(path, "*.*");
                        foreach (var file in files)
                        {
                            try
                            {
                                var fileInfo = new FileInfo(file);
                                freedSpace += fileInfo.Length;
                                fileInfo.Delete();
                                clearedCount++;
                            }
                            catch { }
                        }
                    }
                    catch { }
                }
            }

            // Clear Windows Event Logs (requires admin)
            if (IsRunAsAdministrator())
            {
                try
                {
                    var logNames = new[] { "Application", "System", "Security" };
                    foreach (var logName in logNames)
                    {
                        try
                        {
                            RunCommand("wevtutil", $"cl {logName}");
                            clearedCount++;
                        }
                        catch { }
                    }
                }
                catch { }
            }

            // Clear telemetry data
            var telemetryPath = @"C:\ProgramData\Microsoft\Diagnosis";
            if (Directory.Exists(telemetryPath))
            {
                try
                {
                    var size = GetDirectorySize(telemetryPath);
                    Directory.Delete(telemetryPath, true);
                    freedSpace += size;
                    clearedCount++;
                }
                catch { }
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"  ✅ {(_dryRun ? "Would clear" : "Cleared")} {clearedCount} system log items ({FormatBytes(freedSpace)} {(_dryRun ? "would be freed" : "freed")})");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  ❌ Failed: {ex.Message}");
            Console.ResetColor();
        }
    }

    static void ClearWindowsUpdateCache()
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("\n🔹 Clearing Windows Update cache...");
        Console.ResetColor();

        long freedSpace = 0;
        int clearedCount = 0;

        try
        {
            // Clear Windows Update download cache
            var updateCachePath = @"C:\Windows\SoftwareDistribution\Download";
            if (Directory.Exists(updateCachePath))
            {
                try
                {
                    if (_dryRun)
                    {
                        LogDryRun($"Would delete contents of directory: {updateCachePath}");
                        // Skip size calculation in dry-run for protected dirs
                        freedSpace += 0;
                    }
                    else
                    {
                        var size = GetDirectorySize(updateCachePath);
                        Directory.Delete(updateCachePath, true);
                        Directory.CreateDirectory(updateCachePath);
                        freedSpace += size;
                    }
                    clearedCount++;
                }
                catch { }
            }

            // Clear Delivery Optimization cache
            var deliveryOptPath = @"C:\Windows\ServiceProfiles\NetworkService\AppData\Local\Microsoft\Windows\DeliveryOptimization\Cache";
            if (Directory.Exists(deliveryOptPath))
            {
                try
                {
                    if (_dryRun)
                    {
                        LogDryRun($"Would delete directory: {deliveryOptPath}");
                        freedSpace += 0;
                    }
                    else
                    {
                        var size = GetDirectorySize(deliveryOptPath);
                        Directory.Delete(deliveryOptPath, true);
                        freedSpace += size;
                    }
                    clearedCount++;
                }
                catch { }
            }

            // Clear Windows.old folder (if exists)
            var windowsOldPath = @"C:\Windows.old";
            if (Directory.Exists(windowsOldPath))
            {
                try
                {
                    if (_dryRun)
                    {
                        LogDryRun($"Would delete directory: {windowsOldPath}");
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"  ✅ Would remove Windows.old folder");
                        Console.ResetColor();
                    }
                    else
                    {
                        var size = GetDirectorySize(windowsOldPath);
                        Directory.Delete(windowsOldPath, true);
                        freedSpace += size;
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"  ✅ Removed Windows.old folder ({FormatBytes(size)})");
                        Console.ResetColor();
                    }
                    clearedCount++;
                }
                catch
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine("  ⚠️ Could not remove Windows.old (may require admin or Disk Cleanup)");
                    Console.ResetColor();
                }
            }

            if (clearedCount > 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"  ✅ {(_dryRun ? "Would clear" : "Cleared")} {clearedCount} update cache items ({FormatBytes(freedSpace)} freed)");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("  ⚠️ No update cache items to clear");
                Console.ResetColor();
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  ❌ Failed: {ex.Message}");
            Console.ResetColor();
        }
    }

    static void ClearUserDocumentsFolders()
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("\n🔹 Clearing specific folders from user Documents...");
        Console.ResetColor();

        long freedSpace = 0;
        int clearedCount = 0;

        try
        {
            // Folders to delete from Documents directories
            var foldersToDelete = new[]
            {
                "Custom Office Templates",
                "PowerShell",
                "Visual Studio 2015",
                "Visual Studio 2017",
                "Visual Studio 2018",
                "Visual Studio 2019",
                "Visual Studio 2022",
                "Visual Studio 2025"
            };

            // Get all user directories
            var usersPath = @"C:\Users";
            if (Directory.Exists(usersPath))
            {
                foreach (var userDir in Directory.GetDirectories(usersPath))
                {
                    var userName = Path.GetFileName(userDir);
                    
                    // Skip system accounts
                    if (userName == "Public" || userName == "Default" || userName == "Default User" || 
                        userName == "All Users" || userName == "Default.migrated")
                        continue;

                    // Check both regular Documents and OneDrive Documents (personal and business)
                    var documentsPaths = new List<string>
                    {
                        Path.Combine(userDir, "Documents"),
                        Path.Combine(userDir, "OneDrive", "Documents"),
                        Path.Combine(userDir, "OneDrive - Personal", "Documents")
                    };

                    // Also check for OneDrive Business folders (dynamically named by org)
                    try
                    {
                        var oneDriveDirs = Directory.GetDirectories(userDir, "OneDrive - *", SearchOption.TopDirectoryOnly);
                        foreach (var oneDriveDir in oneDriveDirs)
                        {
                            var docPath = Path.Combine(oneDriveDir, "Documents");
                            if (Directory.Exists(docPath))
                            {
                                documentsPaths.Add(docPath);
                            }
                        }
                    }
                    catch { }

                    foreach (var documentsPath in documentsPaths)
                    {
                        if (!Directory.Exists(documentsPath))
                            continue;

                        foreach (var folderName in foldersToDelete)
                        {
                            var folderPath = Path.Combine(documentsPath, folderName);
                            if (Directory.Exists(folderPath))
                            {
                                try
                                {
                                    var size = GetDirectorySize(folderPath);
                                    
                                    if (_dryRun)
                                    {
                                        LogDryRun($"Would delete: {folderPath} ({FormatBytes(size)})");
                                        Console.WriteLine($"  Would delete: {folderPath.Replace(usersPath, "C:\\Users")}");
                                    }
                                    else
                                    {
                                        Directory.Delete(folderPath, true);
                                        Console.WriteLine($"  ✅ Deleted: {folderPath.Replace(usersPath, "C:\\Users")}");
                                    }
                                    
                                    freedSpace += size;
                                    clearedCount++;
                                }
                                catch (Exception ex)
                                {
                                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                                    Console.WriteLine($"  ⚠️ Could not delete {folderPath}: {ex.Message}");
                                    Console.ResetColor();
                                }
                            }
                        }
                    }
                }
            }

            if (clearedCount > 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"  ✅ {(_dryRun ? "Would delete" : "Deleted")} {clearedCount} folders ({FormatBytes(freedSpace)} {(_dryRun ? "would be freed" : "freed")})");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("  ⚠️ No target folders found in user Documents");
                Console.ResetColor();
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  ❌ Failed: {ex.Message}");
            Console.ResetColor();
        }
    }

    static void ClearFfmpeg()
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("\n🔹 Clearing ffmpeg logs and temp files...");
        Console.ResetColor();

        long freedSpace = 0;
        int clearedCount = 0;

        try
        {
            var temp = Path.GetTempPath();
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            LogDryRun("\n=== FFMPEG ===");

            // ffmpeg temporary files (often left behind during conversions)
            try
            {
                if (Directory.Exists(temp))
                {
                    var ffmpegFiles = Directory.GetFiles(temp, "ffmpeg*.*", SearchOption.TopDirectoryOnly)
                        .Concat(Directory.GetFiles(temp, "*.ffmpeg", SearchOption.TopDirectoryOnly))
                        .Concat(Directory.GetFiles(temp, "*.mkv.tmp", SearchOption.TopDirectoryOnly))
                        .Concat(Directory.GetFiles(temp, "*.mp4.tmp", SearchOption.TopDirectoryOnly));

                    foreach (var file in ffmpegFiles)
                    {
                        try
                        {
                            var fi = new FileInfo(file);
                            freedSpace += fi.Length;
                            
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete: {file} ({FormatBytes(fi.Length)})");
                            }
                            else
                            {
                                fi.Delete();
                            }
                            clearedCount++;
                        }
                        catch { }
                    }
                }
            }
            catch { }

            // ffmpeg config/log locations
            var ffmpegPaths = new[]
            {
                Path.Combine(appData, "ffmpeg"),
                Path.Combine(localAppData, "ffmpeg"),
                Path.Combine(userProfile, ".ffmpeg")
            };

            foreach (var path in ffmpegPaths)
            {
                if (Directory.Exists(path))
                {
                    try
                    {
                        // Only clean logs and temp files, not config
                        var files = Directory.GetFiles(path, "*.log", SearchOption.AllDirectories)
                            .Concat(Directory.GetFiles(path, "*.tmp", SearchOption.AllDirectories));

                        foreach (var file in files)
                        {
                            try
                            {
                                var fi = new FileInfo(file);
                                freedSpace += fi.Length;
                                
                                if (_dryRun)
                                {
                                    LogDryRun($"Would delete: {file} ({FormatBytes(fi.Length)})");
                                }
                                else
                                {
                                    fi.Delete();
                                }
                                clearedCount++;
                            }
                            catch { }
                        }
                    }
                    catch { }
                }
            }

            if (clearedCount > 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"  ✅ {(_dryRun ? "Would clear" : "Cleared")} {clearedCount} ffmpeg temp/log items ({FormatBytes(freedSpace)} {(_dryRun ? "would be freed" : "freed")})");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("  ⚠️ No ffmpeg temp files found");
                Console.ResetColor();
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  ❌ Failed: {ex.Message}");
            Console.ResetColor();
        }
    }

    static void ClearRegistryTraces()
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("\n🔹 Clearing registry traces...");
        Console.ResetColor();

        int clearedCount = 0;

        try
        {
            // Clear UserAssist (program usage statistics)
            try
            {
                using var userAssistKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\UserAssist", true);
                if (userAssistKey != null)
                {
                    var subKeys = userAssistKey.GetSubKeyNames();
                    foreach (var subKeyName in subKeys)
                    {
                        try
                        {
                            using var subKey = userAssistKey.OpenSubKey(subKeyName, true);
                            if (subKey != null)
                            {
                                subKey.DeleteSubKeyTree("Count", false);
                                clearedCount++;
                            }
                        }
                        catch { }
                    }
                }
            }
            catch { }

            // Clear ComDlg32 recent file lists
            var comdlgKeys = new[]
            {
                @"Software\Microsoft\Windows\CurrentVersion\Explorer\ComDlg32\LastVisitedPidlMRU",
                @"Software\Microsoft\Windows\CurrentVersion\Explorer\ComDlg32\OpenSavePidlMRU"
            };

            foreach (var keyPath in comdlgKeys)
            {
                try
                {
                    using var key = Registry.CurrentUser.OpenSubKey(keyPath, true);
                    if (key != null)
                    {
                        var valueNames = key.GetValueNames();
                        foreach (var valueName in valueNames)
                        {
                            if (valueName != "MRUList" && valueName != "")
                            {
                                try
                                {
                                    key.DeleteValue(valueName);
                                }
                                catch { }
                            }
                        }
                        
                        if (key.GetValue("MRUList") != null)
                        {
                            key.SetValue("MRUList", "");
                        }
                        clearedCount++;
                    }
                }
                catch { }
            }

            // Clear File Explorer typed paths
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\TypedPaths", true);
                if (key != null)
                {
                    var valueNames = key.GetValueNames();
                    foreach (var valueName in valueNames)
                    {
                        try
                        {
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete registry value: TypedPaths\\{valueName}");
                            }
                            else
                            {
                                key.DeleteValue(valueName);
                            }
                        }
                        catch { }
                    }
                    clearedCount++;
                }
            }
            catch { }

            // Clear WordWheelQuery (Windows Search typed queries)
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\WordWheelQuery", true);
                if (key != null)
                {
                    var valueNames = key.GetValueNames();
                    foreach (var valueName in valueNames)
                    {
                        if (valueName != "MRUListEx" && valueName != "")
                        {
                            try
                            {
                                if (_dryRun)
                                {
                                    LogDryRun($"Would delete registry value: WordWheelQuery\\{valueName}");
                                }
                                else
                                {
                                    key.DeleteValue(valueName);
                                }
                            }
                            catch { }
                        }
                    }
                    clearedCount++;
                }
            }
            catch { }

            // Clear StreamMRU (file streams)
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\StreamMRU", true);
                if (key != null)
                {
                    var valueNames = key.GetValueNames();
                    foreach (var valueName in valueNames)
                    {
                        if (valueName != "MRUListEx" && valueName != "")
                        {
                            try
                            {
                                if (_dryRun)
                                {
                                    LogDryRun($"Would delete registry value: StreamMRU\\{valueName}");
                                }
                                else
                                {
                                    key.DeleteValue(valueName);
                                }
                            }
                            catch { }
                        }
                    }
                    clearedCount++;
                }
            }
            catch { }

            // Clear Find Computer MRU (network computer search)
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\FindComputerMRU", true);
                if (key != null)
                {
                    var valueNames = key.GetValueNames();
                    foreach (var valueName in valueNames)
                    {
                        if (valueName != "MRUList" && valueName != "")
                        {
                            try
                            {
                                if (_dryRun)
                                {
                                    LogDryRun($"Would delete registry value: FindComputerMRU\\{valueName}");
                                }
                                else
                                {
                                    key.DeleteValue(valueName);
                                }
                            }
                            catch { }
                        }
                    }
                    if (key.GetValue("MRUList") != null && !_dryRun)
                    {
                        key.SetValue("MRUList", "");
                    }
                    clearedCount++;
                }
            }
            catch { }

            // Clear Internet Explorer/Edge Typed URLs
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Internet Explorer\TypedURLs", true);
                if (key != null)
                {
                    var valueNames = key.GetValueNames();
                    foreach (var valueName in valueNames)
                    {
                        try
                        {
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete registry value: TypedURLs\\{valueName}");
                            }
                            else
                            {
                                key.DeleteValue(valueName);
                            }
                        }
                        catch { }
                    }
                    clearedCount++;
                }
            }
            catch { }

            // Clear Office Recent Documents (all versions)
            var officeVersions = new[] { "16.0", "15.0", "14.0", "12.0" }; // Office 2016-2021, 2013, 2010, 2007
            var officeApps = new[] { "Word", "Excel", "PowerPoint", "Access" };
            
            foreach (var version in officeVersions)
            {
                foreach (var app in officeApps)
                {
                    try
                    {
                        var officePath = $@"Software\Microsoft\Office\{version}\{app}\File MRU";
                        using var key = Registry.CurrentUser.OpenSubKey(officePath, true);
                        if (key != null)
                        {
                            var subKeyNames = key.GetSubKeyNames();
                            foreach (var subKeyName in subKeyNames)
                            {
                                try
                                {
                                    if (_dryRun)
                                    {
                                        LogDryRun($"Would delete registry key: {officePath}\\{subKeyName}");
                                    }
                                    else
                                    {
                                        key.DeleteSubKeyTree(subKeyName, false);
                                    }
                                }
                                catch { }
                            }
                            
                            var valueNames = key.GetValueNames();
                            foreach (var valueName in valueNames)
                            {
                                try
                                {
                                    if (_dryRun)
                                    {
                                        LogDryRun($"Would delete registry value: {officePath}\\{valueName}");
                                    }
                                    else
                                    {
                                        key.DeleteValue(valueName);
                                    }
                                }
                                catch { }
                            }
                            clearedCount++;
                        }
                    }
                    catch { }

                    // Also clear Office Place MRU (recent folders)
                    try
                    {
                        var placePath = $@"Software\Microsoft\Office\{version}\{app}\Place MRU";
                        using var key = Registry.CurrentUser.OpenSubKey(placePath, true);
                        if (key != null)
                        {
                            var subKeyNames = key.GetSubKeyNames();
                            foreach (var subKeyName in subKeyNames)
                            {
                                try
                                {
                                    if (_dryRun)
                                    {
                                        LogDryRun($"Would delete registry key: {placePath}\\{subKeyName}");
                                    }
                                    else
                                    {
                                        key.DeleteSubKeyTree(subKeyName, false);
                                    }
                                }
                                catch { }
                            }
                            clearedCount++;
                        }
                    }
                    catch { }
                }
            }

            // Clear Notepad recent files
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Notepad", true);
                if (key != null)
                {
                    var valueNames = key.GetValueNames();
                    foreach (var valueName in valueNames)
                    {
                        if (valueName.StartsWith("File", StringComparison.OrdinalIgnoreCase))
                        {
                            try
                            {
                                if (_dryRun)
                                {
                                    LogDryRun($"Would delete registry value: Notepad\\{valueName}");
                                }
                                else
                                {
                                    key.DeleteValue(valueName);
                                }
                            }
                            catch { }
                        }
                    }
                    clearedCount++;
                }
            }
            catch { }

            // Clear Paint recent files
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Applets\Paint\Recent File List", true);
                if (key != null)
                {
                    var valueNames = key.GetValueNames();
                    foreach (var valueName in valueNames)
                    {
                        try
                        {
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete registry value: Paint Recent File List\\{valueName}");
                            }
                            else
                            {
                                key.DeleteValue(valueName);
                            }
                        }
                        catch { }
                    }
                    clearedCount++;
                }
            }
            catch { }

            // Clear IME (Input Method Editor) history for Asian languages
            var imeLanguages = new[] { "CHS", "CHT", "JPN", "KOR" };
            foreach (var lang in imeLanguages)
            {
                try
                {
                    var imePath = $@"Software\Microsoft\InputMethod\Settings\{lang}";
                    using var key = Registry.CurrentUser.OpenSubKey(imePath, true);
                    if (key != null)
                    {
                        // Clear IME history values
                        var valueNames = key.GetValueNames();
                        foreach (var valueName in valueNames)
                        {
                            if (valueName.Contains("History", StringComparison.OrdinalIgnoreCase) ||
                                valueName.Contains("UserPhrase", StringComparison.OrdinalIgnoreCase))
                            {
                                try
                                {
                                    if (_dryRun)
                                    {
                                        LogDryRun($"Would delete registry value: IME {lang}\\{valueName}");
                                    }
                                    else
                                    {
                                        key.DeleteValue(valueName);
                                    }
                                }
                                catch { }
                            }
                        }
                        clearedCount++;
                    }
                }
                catch { }
            }

            // Clear Run Dialog History
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\RunMRU", true);
                if (key != null)
                {
                    var valueNames = key.GetValueNames();
                    foreach (var valueName in valueNames)
                    {
                        if (valueName != "MRUList" && valueName != "")
                        {
                            try
                            {
                                if (_dryRun)
                                {
                                    LogDryRun($"Would delete registry value: RunMRU\\{valueName}");
                                }
                                else
                                {
                                    key.DeleteValue(valueName);
                                }
                            }
                            catch { }
                        }
                    }
                    if (key.GetValue("MRUList") != null && !_dryRun)
                    {
                        key.SetValue("MRUList", "");
                    }
                    clearedCount++;
                }
            }
            catch { }

            // Clear Map Network Drive MRU
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Map Network Drive MRU", true);
                if (key != null)
                {
                    var valueNames = key.GetValueNames();
                    foreach (var valueName in valueNames)
                    {
                        if (valueName != "MRUList" && valueName != "")
                        {
                            try
                            {
                                if (_dryRun)
                                {
                                    LogDryRun($"Would delete registry value: Map Network Drive MRU\\{valueName}");
                                }
                                else
                                {
                                    key.DeleteValue(valueName);
                                }
                            }
                            catch { }
                        }
                    }
                    if (key.GetValue("MRUList") != null && !_dryRun)
                    {
                        key.SetValue("MRUList", "");
                    }
                    clearedCount++;
                }
            }
            catch { }

            // Clear Remote Desktop Connection history
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Terminal Server Client\Default", true);
                if (key != null)
                {
                    var valueNames = key.GetValueNames();
                    foreach (var valueName in valueNames)
                    {
                        try
                        {
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete registry value: RDP Default\\{valueName}");
                            }
                            else
                            {
                                key.DeleteValue(valueName);
                            }
                        }
                        catch { }
                    }
                    clearedCount++;
                }
            }
            catch { }

            // Clear MUICache (tracks executable file display names and paths)
            try
            {
                var muiCachePaths = new[]
                {
                    @"Software\Classes\Local Settings\Software\Microsoft\Windows\Shell\MuiCache",
                    @"Software\Microsoft\Windows\ShellNoRoam\MUICache"
                };

                foreach (var muiPath in muiCachePaths)
                {
                    try
                    {
                        using var key = Registry.CurrentUser.OpenSubKey(muiPath, true);
                        if (key != null)
                        {
                            var valueNames = key.GetValueNames();
                            foreach (var valueName in valueNames)
                            {
                                try
                                {
                                    if (_dryRun)
                                    {
                                        LogDryRun($"Would delete registry value: {muiPath}\\{valueName}");
                                    }
                                    else
                                    {
                                        key.DeleteValue(valueName);
                                    }
                                }
                                catch { }
                            }
                            clearedCount++;
                        }
                    }
                    catch { }
                }
            }
            catch { }

            // Clear RecentDocs (recent documents by file type)
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\RecentDocs", true);
                if (key != null)
                {
                    var subKeyNames = key.GetSubKeyNames();
                    foreach (var subKeyName in subKeyNames)
                    {
                        try
                        {
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete registry key: RecentDocs\\{subKeyName}");
                            }
                            else
                            {
                                key.DeleteSubKeyTree(subKeyName, false);
                            }
                        }
                        catch { }
                    }

                    // Also clear values in the RecentDocs root key
                    var valueNames = key.GetValueNames();
                    foreach (var valueName in valueNames)
                    {
                        if (valueName != "MRUListEx" && valueName != "")
                        {
                            try
                            {
                                if (_dryRun)
                                {
                                    LogDryRun($"Would delete registry value: RecentDocs\\{valueName}");
                                }
                                else
                                {
                                    key.DeleteValue(valueName);
                                }
                            }
                            catch { }
                        }
                    }
                    clearedCount++;
                }
            }
            catch { }

            // Clear RunMRU (Run dialog history)
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\RunMRU", true);
                if (key != null)
                {
                    var valueNames = key.GetValueNames();
                    foreach (var valueName in valueNames)
                    {
                        if (valueName != "MRUList" && valueName != "")
                        {
                            try
                            {
                                if (_dryRun)
                                {
                                    LogDryRun($"Would delete registry value: RunMRU\\{valueName}");
                                }
                                else
                                {
                                    key.DeleteValue(valueName);
                                }
                            }
                            catch { }
                        }
                    }
                    if (key.GetValue("MRUList") != null && !_dryRun)
                    {
                        key.SetValue("MRUList", "");
                    }
                    clearedCount++;
                }
            }
            catch { }

            // Clear Windows Shell Bags (folder view settings and history)
            var shellBagPaths = new[]
            {
                @"Software\Classes\Local Settings\Software\Microsoft\Windows\Shell\BagMRU",
                @"Software\Classes\Local Settings\Software\Microsoft\Windows\Shell\Bags",
                @"Software\Microsoft\Windows\Shell\BagMRU",
                @"Software\Microsoft\Windows\Shell\Bags"
            };

            foreach (var bagPath in shellBagPaths)
            {
                try
                {
                    using var key = Registry.CurrentUser.OpenSubKey(bagPath, true);
                    if (key != null)
                    {
                        var subKeyNames = key.GetSubKeyNames();
                        foreach (var subKeyName in subKeyNames)
                        {
                            try
                            {
                                if (_dryRun)
                                {
                                    LogDryRun($"Would delete registry key: {bagPath}\\{subKeyName}");
                                }
                                else
                                {
                                    key.DeleteSubKeyTree(subKeyName, false);
                                }
                            }
                            catch { }
                        }
                        clearedCount++;
                    }
                }
                catch { }
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"  ✅ {(_dryRun ? "Would clear" : "Cleared")} {clearedCount} registry trace items");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  ❌ Failed: {ex.Message}");
            Console.ResetColor();
        }
    }

    static void ClearAdditionalRegistryTraces()
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("\n🔹 Clearing additional registry user activity traces...");
        Console.ResetColor();

        int clearedCount = 0;

        try
        {
            LogDryRun("\n=== ADDITIONAL REGISTRY TRACES ===");

            // Clear Windows Explorer Recent Folders (for Jump Lists)
            try
            {
                var recentFoldersKey = @"Software\Microsoft\Windows\CurrentVersion\Explorer\RecentDocs";
                using var key = Registry.CurrentUser.OpenSubKey(recentFoldersKey, true);
                if (key != null)
                {
                    // Clear all subkeys (file types)
                    var subKeyNames = key.GetSubKeyNames();
                    foreach (var subKeyName in subKeyNames)
                    {
                        try
                        {
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete registry subkey: {recentFoldersKey}\\{subKeyName}");
                            }
                            else
                            {
                                key.DeleteSubKeyTree(subKeyName, false);
                            }
                        }
                        catch { }
                    }
                    clearedCount++;
                }
            }
            catch { }

            // Clear Taskbar Jump Lists
            try
            {
                var jumpListPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    @"Microsoft\Windows\Recent\AutomaticDestinations");
                
                if (Directory.Exists(jumpListPath))
                {
                    var files = Directory.GetFiles(jumpListPath, "*.automaticDestinations-ms");
                    foreach (var file in files)
                    {
                        try
                        {
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete Jump List: {file}");
                            }
                            else
                            {
                                File.Delete(file);
                            }
                            clearedCount++;
                        }
                        catch { }
                    }
                }

                var customJumpListPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    @"Microsoft\Windows\Recent\CustomDestinations");
                
                if (Directory.Exists(customJumpListPath))
                {
                    var files = Directory.GetFiles(customJumpListPath, "*.customDestinations-ms");
                    foreach (var file in files)
                    {
                        try
                        {
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete Custom Jump List: {file}");
                            }
                            else
                            {
                                File.Delete(file);
                            }
                            clearedCount++;
                        }
                        catch { }
                    }
                }
            }
            catch { }

            // Clear Explorer address bar dropdown (TypedURLs for IE/Edge)
            try
            {
                var typedUrlsKey = @"Software\Microsoft\Internet Explorer\TypedURLs";
                using var key = Registry.CurrentUser.OpenSubKey(typedUrlsKey, true);
                if (key != null)
                {
                    var valueNames = key.GetValueNames();
                    foreach (var valueName in valueNames)
                    {
                        try
                        {
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete registry value: TypedURLs\\{valueName}");
                            }
                            else
                            {
                                key.DeleteValue(valueName);
                            }
                        }
                        catch { }
                    }
                    clearedCount++;
                }
            }
            catch { }

            // Clear Windows Timeline activity history
            try
            {
                var timelineDbPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    @"ConnectedDevicesPlatform\L." + Environment.UserName,
                    "ActivitiesCache.db");
                
                if (File.Exists(timelineDbPath))
                {
                    try
                    {
                        if (_dryRun)
                        {
                            LogDryRun($"Would delete Timeline database: {timelineDbPath}");
                        }
                        else
                        {
                            File.Delete(timelineDbPath);
                        }
                        clearedCount++;
                    }
                    catch { }
                }
            }
            catch { }

            // Clear Cortana search history
            try
            {
                var cortanaKey = @"Software\Microsoft\Windows\CurrentVersion\SearchSettings";
                using var key = Registry.CurrentUser.OpenSubKey(cortanaKey, true);
                if (key != null)
                {
                    try
                    {
                        if (_dryRun)
                        {
                            LogDryRun($"Would clear Cortana search settings");
                        }
                        else
                        {
                            key.SetValue("IsAADCloudSearchEnabled", 0);
                            key.SetValue("IsDeviceSearchHistoryEnabled", 0);
                        }
                        clearedCount++;
                    }
                    catch { }
                }
            }
            catch { }

            // Clear Windows Error Reporting queue (user-level)
            try
            {
                var werQueuePath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    @"Microsoft\Windows\WER\ReportQueue");
                
                if (Directory.Exists(werQueuePath))
                {
                    var dirs = Directory.GetDirectories(werQueuePath);
                    foreach (var dir in dirs)
                    {
                        try
                        {
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete WER queue: {dir}");
                            }
                            else
                            {
                                Directory.Delete(dir, true);
                            }
                            clearedCount++;
                        }
                        catch { }
                    }
                }
            }
            catch { }

            // Clear Start Menu tracking
            try
            {
                var startTrackingKey = @"Software\Microsoft\Windows\CurrentVersion\Explorer\StartPage";
                using var key = Registry.CurrentUser.OpenSubKey(startTrackingKey, true);
                if (key != null)
                {
                    var valueNames = key.GetValueNames();
                    foreach (var valueName in valueNames)
                    {
                        if (valueName.Contains("MostUsed") || valueName.Contains("Suggested"))
                        {
                            try
                            {
                                if (_dryRun)
                                {
                                    LogDryRun($"Would delete registry value: StartPage\\{valueName}");
                                }
                                else
                                {
                                    key.DeleteValue(valueName, false);
                                }
                            }
                            catch { }
                        }
                    }
                    clearedCount++;
                }
            }
            catch { }

            // Clear Windows notifications/action center history
            try
            {
                var notificationsPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    @"Microsoft\Windows\Notifications");
                
                if (Directory.Exists(notificationsPath))
                {
                    var files = Directory.GetFiles(notificationsPath, "*.db*", SearchOption.AllDirectories);
                    foreach (var file in files)
                    {
                        try
                        {
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete notification history: {file}");
                            }
                            else
                            {
                                File.Delete(file);
                            }
                            clearedCount++;
                        }
                        catch { }
                    }
                }
            }
            catch { }

            if (clearedCount > 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"  ✅ {(_dryRun ? "Would clear" : "Cleared")} {clearedCount} additional trace categories");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("  ⚠️ No additional traces found");
                Console.ResetColor();
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  ❌ Failed: {ex.Message}");
            Console.ResetColor();
        }
    }

    static void ClearExpandedRegistryTraces()
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("\n🔹 Clearing expanded registry activity traces...");
        Console.ResetColor();

        int clearedCount = 0;

        try
        {
            LogDryRun("\n=== EXPANDED REGISTRY TRACES ===");

            // Map Network Drive MRU
            try
            {
                var mapDriveMRU = @"Software\Microsoft\Windows\CurrentVersion\Explorer\Map Network Drive MRU";
                using var key = Registry.CurrentUser.OpenSubKey(mapDriveMRU, true);
                if (key != null)
                {
                    var valueNames = key.GetValueNames();
                    foreach (var valueName in valueNames)
                    {
                        if (valueName != "MRUList" && valueName != "")
                        {
                            try
                            {
                                if (_dryRun)
                                {
                                    LogDryRun($"Would delete registry value: {mapDriveMRU}\\{valueName}");
                                }
                                else
                                {
                                    key.DeleteValue(valueName);
                                }
                            }
                            catch { }
                        }
                    }
                    
                    if (key.GetValue("MRUList") != null)
                    {
                        if (!_dryRun)
                        {
                            key.SetValue("MRUList", "");
                        }
                        else
                        {
                            LogDryRun($"Would delete registry value: {mapDriveMRU}\\MRUList");
                        }
                    }
                    clearedCount++;
                }
            }
            catch { }

            // Explorer Feature Usage (tracks which features used)
            try
            {
                var featureUsagePath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\FeatureUsage";
                using var key = Registry.CurrentUser.OpenSubKey(featureUsagePath, true);
                if (key != null)
                {
                    var subKeyNames = key.GetSubKeyNames();
                    foreach (var subKeyName in subKeyNames)
                    {
                        try
                        {
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete registry subkey: {featureUsagePath}\\{subKeyName}");
                            }
                            else
                            {
                                key.DeleteSubKeyTree(subKeyName, false);
                            }
                        }
                        catch { }
                    }
                    clearedCount++;
                }
            }
            catch { }

            // Windows Search Recent Apps
            try
            {
                var searchRecentApps = @"Software\Microsoft\Windows\CurrentVersion\Search\RecentApps";
                using var key = Registry.CurrentUser.OpenSubKey(searchRecentApps, true);
                if (key != null)
                {
                    var subKeyNames = key.GetSubKeyNames();
                    foreach (var subKeyName in subKeyNames)
                    {
                        try
                        {
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete registry subkey: {searchRecentApps}\\{subKeyName}");
                            }
                            else
                            {
                                key.DeleteSubKeyTree(subKeyName, false);
                            }
                        }
                        catch { }
                    }
                    clearedCount++;
                }
            }
            catch { }

            // Program Compatibility Assistant Store
            try
            {
                var pcaStore = @"Software\Microsoft\Windows NT\CurrentVersion\AppCompatFlags\Compatibility Assistant\Store";
                using var key = Registry.CurrentUser.OpenSubKey(pcaStore, true);
                if (key != null)
                {
                    var valueNames = key.GetValueNames();
                    foreach (var valueName in valueNames)
                    {
                        try
                        {
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete registry value: {pcaStore}\\{valueName}");
                            }
                            else
                            {
                                key.DeleteValue(valueName);
                            }
                        }
                        catch { }
                    }
                    clearedCount++;
                }
            }
            catch { }

            // Terminal Server Client (RDP) history
            try
            {
                var rdpDefault = @"Software\Microsoft\Terminal Server Client\Default";
                using var key = Registry.CurrentUser.OpenSubKey(rdpDefault, true);
                if (key != null)
                {
                    var valueNames = key.GetValueNames();
                    foreach (var valueName in valueNames)
                    {
                        try
                        {
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete registry value: {rdpDefault}\\{valueName}");
                            }
                            else
                            {
                                key.DeleteValue(valueName);
                            }
                        }
                        catch { }
                    }
                    clearedCount++;
                }

                // Servers list
                var rdpServers = @"Software\Microsoft\Terminal Server Client\Servers";
                using var serversKey = Registry.CurrentUser.OpenSubKey(rdpServers, true);
                if (serversKey != null)
                {
                    var subKeyNames = serversKey.GetSubKeyNames();
                    foreach (var subKeyName in subKeyNames)
                    {
                        try
                        {
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete registry subkey: {rdpServers}\\{subKeyName}");
                            }
                            else
                            {
                                serversKey.DeleteSubKeyTree(subKeyName, false);
                            }
                        }
                        catch { }
                    }
                    clearedCount++;
                }
            }
            catch { }

            // User File History (UFH/SHC)
            try
            {
                var ufhPath = @"Software\Microsoft\Windows\CurrentVersion\UFH\SHC";
                using var key = Registry.CurrentUser.OpenSubKey(ufhPath, true);
                if (key != null)
                {
                    var valueNames = key.GetValueNames();
                    foreach (var valueName in valueNames)
                    {
                        try
                        {
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete registry value: {ufhPath}\\{valueName}");
                            }
                            else
                            {
                                key.DeleteValue(valueName);
                            }
                        }
                        catch { }
                    }
                    clearedCount++;
                }
            }
            catch { }

            // MUICache in Classes (tracks executable names)
            try
            {
                var muiCachePath = @"Software\Classes\Local Settings\Software\Microsoft\Windows\Shell\MuiCache";
                using var key = Registry.CurrentUser.OpenSubKey(muiCachePath, true);
                if (key != null)
                {
                    var valueNames = key.GetValueNames();
                    foreach (var valueName in valueNames)
                    {
                        try
                        {
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete registry value: {muiCachePath}\\{valueName}");
                            }
                            else
                            {
                                key.DeleteValue(valueName);
                            }
                        }
                        catch { }
                    }
                    clearedCount++;
                }
            }
            catch { }

            // AppKey tracking
            try
            {
                var appKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\AppKey";
                using var key = Registry.CurrentUser.OpenSubKey(appKeyPath, true);
                if (key != null)
                {
                    var subKeyNames = key.GetSubKeyNames();
                    foreach (var subKeyName in subKeyNames)
                    {
                        try
                        {
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete registry subkey: {appKeyPath}\\{subKeyName}");
                            }
                            else
                            {
                                key.DeleteSubKeyTree(subKeyName, false);
                            }
                        }
                        catch { }
                    }
                    clearedCount++;
                }
            }
            catch { }

            if (clearedCount > 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"  ✅ {(_dryRun ? "Would clear" : "Cleared")} {clearedCount} expanded registry trace categories");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("  ⚠️ No expanded registry traces found");
                Console.ResetColor();
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  ❌ Failed: {ex.Message}");
            Console.ResetColor();
        }
    }

    static void ClearWindowsFeedback()
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("\n🔹 Clearing Windows Feedback and Diagnostics data...");
        Console.ResetColor();

        long freedSpace = 0;
        int clearedCount = 0;

        try
        {
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var programData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

            LogDryRun("\n=== WINDOWS FEEDBACK ===");

            // Windows Feedback Hub data
            var feedbackPaths = new[]
            {
                Path.Combine(localAppData, @"Microsoft\Windows\WinX\WinXFeedback"),
                Path.Combine(localAppData, @"Packages\Microsoft.WindowsFeedbackHub_8wekyb3d8bbwe\LocalCache"),
                Path.Combine(localAppData, @"Packages\Microsoft.WindowsFeedbackHub_8wekyb3d8bbwe\TempState"),
                Path.Combine(localAppData, @"Packages\Microsoft.WindowsFeedbackHub_8wekyb3d8bbwe\AC\Temp"),
                Path.Combine(programData, @"Microsoft\Diagnosis\FeedbackHub")
            };

            foreach (var path in feedbackPaths)
            {
                if (Directory.Exists(path))
                {
                    try
                    {
                        var size = GetDirectorySize(path);
                        freedSpace += size;
                        
                        if (_dryRun)
                        {
                            LogDryRun($"Would delete: {path} ({FormatBytes(size)})");
                        }
                        else
                        {
                            Directory.Delete(path, true);
                        }
                        clearedCount++;
                    }
                    catch { }
                }
            }

            // Windows diagnostic data
            var diagnosticPaths = new[]
            {
                Path.Combine(programData, @"Microsoft\Diagnosis"),
                Path.Combine(localAppData, @"Microsoft\Windows\AppDiagnostics"),
                Path.Combine(localAppData, @"Diagnostics")
            };

            foreach (var path in diagnosticPaths)
            {
                if (Directory.Exists(path))
                {
                    try
                    {
                        // Delete .etl, .log files but preserve structure
                        var files = Directory.GetFiles(path, "*.etl", SearchOption.AllDirectories)
                            .Concat(Directory.GetFiles(path, "*.log", SearchOption.AllDirectories))
                            .Concat(Directory.GetFiles(path, "*.cab", SearchOption.AllDirectories));

                        foreach (var file in files)
                        {
                            try
                            {
                                var fi = new FileInfo(file);
                                freedSpace += fi.Length;
                                
                                if (_dryRun)
                                {
                                    LogDryRun($"Would delete: {file} ({FormatBytes(fi.Length)})");
                                }
                                else
                                {
                                    fi.Delete();
                                }
                                clearedCount++;
                            }
                            catch { }
                        }
                    }
                    catch { }
                }
            }

            if (clearedCount > 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"  ✅ {(_dryRun ? "Would clear" : "Cleared")} {clearedCount} feedback/diagnostic items ({FormatBytes(freedSpace)} {(_dryRun ? "would be freed" : "freed")})");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("  ⚠️ No Windows Feedback data found");
                Console.ResetColor();
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  ❌ Failed: {ex.Message}");
            Console.ResetColor();
        }
    }

    static void ClearOllama()
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("\n🔹 Clearing Ollama logs and cache...");
        Console.ResetColor();

        long freedSpace = 0;
        int clearedCount = 0;

        try
        {
            var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            LogDryRun("\n=== OLLAMA ===");

            // Ollama paths (preserve models)
            var ollamaPaths = new List<(string path, string[] patterns)>
            {
                (Path.Combine(userProfile, ".ollama"), new[] { "*.log", "*.tmp" }),
                (Path.Combine(localAppData, "Ollama"), new[] { "*.log", "*.tmp" }),
                (Path.Combine(appData, "Ollama"), new[] { "*.log", "*.tmp" })
            };

            // Clear logs but preserve models
            foreach (var (basePath, patterns) in ollamaPaths)
            {
                if (Directory.Exists(basePath))
                {
                    foreach (var pattern in patterns)
                    {
                        try
                        {
                            var files = Directory.GetFiles(basePath, pattern, SearchOption.AllDirectories);
                            foreach (var file in files)
                            {
                                try
                                {
                                    var fi = new FileInfo(file);
                                    freedSpace += fi.Length;
                                    
                                    if (_dryRun)
                                    {
                                        LogDryRun($"Would delete: {file} ({FormatBytes(fi.Length)})");
                                    }
                                    else
                                    {
                                        fi.Delete();
                                    }
                                    clearedCount++;
                                }
                                catch { }
                            }
                        }
                        catch { }
                    }

                    // Clear logs directory if exists
                    var logsPath = Path.Combine(basePath, "logs");
                    if (Directory.Exists(logsPath))
                    {
                        try
                        {
                            var size = GetDirectorySize(logsPath);
                            freedSpace += size;
                            
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete: {logsPath} ({FormatBytes(size)})");
                            }
                            else
                            {
                                Directory.Delete(logsPath, true);
                            }
                            clearedCount++;
                        }
                        catch { }
                    }
                }
            }

            if (clearedCount > 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"  ✅ {(_dryRun ? "Would clear" : "Cleared")} {clearedCount} Ollama log items ({FormatBytes(freedSpace)} {(_dryRun ? "would be freed" : "freed")})");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("  ⚠️ No Ollama logs found (Ollama may not be installed)");
                Console.ResetColor();
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  ❌ Failed: {ex.Message}");
            Console.ResetColor();
        }
    }

    static void ClearYtDlp()
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("\n🔹 Clearing yt-dlp cache and history...");
        Console.ResetColor();

        long freedSpace = 0;
        int clearedCount = 0;

        try
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            LogDryRun("\n=== YT-DLP ===");

            // yt-dlp cache locations
            var ytDlpPaths = new[]
            {
                Path.Combine(appData, "yt-dlp"),
                Path.Combine(localAppData, "yt-dlp"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".cache", "yt-dlp")
            };

            foreach (var path in ytDlpPaths)
            {
                if (Directory.Exists(path))
                {
                    try
                    {
                        var size = GetDirectorySize(path);
                        freedSpace += size;
                        
                        if (_dryRun)
                        {
                            LogDryRun($"Would delete: {path} ({FormatBytes(size)})");
                        }
                        else
                        {
                            Directory.Delete(path, true);
                        }
                        clearedCount++;
                    }
                    catch { }
                }
            }

            // Also check for youtube-dl cache (older version)
            var youtubeDlPaths = new[]
            {
                Path.Combine(appData, "youtube-dl"),
                Path.Combine(localAppData, "youtube-dl"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".cache", "youtube-dl")
            };

            foreach (var path in youtubeDlPaths)
            {
                if (Directory.Exists(path))
                {
                    try
                    {
                        var size = GetDirectorySize(path);
                        freedSpace += size;
                        
                        if (_dryRun)
                        {
                            LogDryRun($"Would delete: {path} ({FormatBytes(size)})");
                        }
                        else
                        {
                            Directory.Delete(path, true);
                        }
                        clearedCount++;
                    }
                    catch { }
                }
            }

            if (clearedCount > 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"  ✅ {(_dryRun ? "Would clear" : "Cleared")} {clearedCount} yt-dlp cache locations ({FormatBytes(freedSpace)} {(_dryRun ? "would be freed" : "freed")})");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("  ⚠️ No yt-dlp cache found");
                Console.ResetColor();
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  ❌ Failed: {ex.Message}");
            Console.ResetColor();
        }
    }

    static void ClearSystemRestoreAndShadowCopies()
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("\n🔹 Clearing System Restore points and Shadow Copies...");
        Console.ResetColor();

        int clearedCount = 0;

        try
        {
            if (!IsRunAsAdministrator())
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("  ❌ Requires administrator privileges to delete restore points and shadow copies");
                Console.ResetColor();
                return;
            }

            LogDryRun("\n=== SYSTEM RESTORE & SHADOW COPIES ===");

            // Delete all Shadow Copies (Volume Shadow Copy Service snapshots)
            // These contain previous versions of files and can reveal user activity
            try
            {
                Console.WriteLine("  Deleting Volume Shadow Copies...");
                
                if (!_dryRun)
                {
                    var vssProcess = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "vssadmin.exe",
                            Arguments = "delete shadows /all /quiet",
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            CreateNoWindow = true
                        }
                    };
                    
                    vssProcess.Start();
                    vssProcess.WaitForExit(30000); // 30 second timeout
                    
                    if (vssProcess.ExitCode == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("  ✅ Shadow copies deleted");
                        Console.ResetColor();
                        clearedCount++;
                    }
                    else
                    {
                        var error = vssProcess.StandardError.ReadToEnd();
                        if (string.IsNullOrEmpty(error) || error.Contains("No items found"))
                        {
                            Console.ForegroundColor = ConsoleColor.DarkYellow;
                            Console.WriteLine("  ⚠️ No shadow copies found");
                            Console.ResetColor();
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine($"  ⚠️ Shadow copy deletion: {error.Trim()}");
                            Console.ResetColor();
                        }
                    }
                }
                else
                {
                    LogDryRun("Would delete all Volume Shadow Copies (vssadmin delete shadows /all)");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("  ✅ Would delete all shadow copies");
                    Console.ResetColor();
                    clearedCount++;
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"  ❌ Shadow copy deletion failed: {ex.Message}");
                Console.ResetColor();
            }

            // Delete all System Restore Points
            // These contain system state and can include user files
            try
            {
                Console.WriteLine("  Deleting System Restore Points...");
                
                if (!_dryRun)
                {
                    // Use PowerShell to get and delete restore points
                    var psScript = @"
                        try {
                            $restorePoints = Get-ComputerRestorePoint -ErrorAction Stop
                            if ($restorePoints) {
                                $count = $restorePoints.Count
                                # Delete all restore points using WMI
                                $result = (Get-WmiObject -Namespace 'root\default' -Class SystemRestore).Disable($env:SystemDrive)
                                Start-Sleep -Seconds 1
                                (Get-WmiObject -Namespace 'root\default' -Class SystemRestore).Enable($env:SystemDrive)
                                Write-Output ""DELETED:$count""
                            } else {
                                Write-Output ""NONE""
                            }
                        } catch {
                            Write-Output ""ERROR:$($_.Exception.Message)""
                        }
                    ";

                    var psProcess = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "powershell.exe",
                            Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"{psScript}\"",
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            CreateNoWindow = true
                        }
                    };
                    
                    psProcess.Start();
                    var output = psProcess.StandardOutput.ReadToEnd();
                    psProcess.WaitForExit(30000);
                    
                    if (output.Contains("DELETED:"))
                    {
                        var countStr = output.Split(':')[1].Trim();
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"  ✅ Deleted {countStr} restore point(s)");
                        Console.ResetColor();
                        clearedCount++;
                    }
                    else if (output.Contains("NONE"))
                    {
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.WriteLine("  ⚠️ No restore points found");
                        Console.ResetColor();
                    }
                    else if (output.Contains("ERROR:"))
                    {
                        var error = output.Split(':')[1].Trim();
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"  ⚠️ Restore point deletion: {error}");
                        Console.ResetColor();
                    }
                }
                else
                {
                    LogDryRun("Would delete all System Restore Points");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("  ✅ Would delete all restore points");
                    Console.ResetColor();
                    clearedCount++;
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"  ❌ Restore point deletion failed: {ex.Message}");
                Console.ResetColor();
            }

            // Also try to clear Windows Backup shadow copies (if Windows Backup is used)
            try
            {
                var windowsBackupPath = @"C:\Windows\System32\config\systemprofile\AppData\Local\Microsoft\Windows\FileHistory";
                if (Directory.Exists(windowsBackupPath))
                {
                    if (!_dryRun)
                    {
                        var size = GetDirectorySize(windowsBackupPath);
                        Directory.Delete(windowsBackupPath, true);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"  ✅ Cleared File History backup metadata ({FormatBytes(size)})");
                        Console.ResetColor();
                        clearedCount++;
                    }
                    else
                    {
                        LogDryRun($"Would delete: {windowsBackupPath}");
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("  ✅ Would clear File History metadata");
                        Console.ResetColor();
                        clearedCount++;
                    }
                }
            }
            catch { }

            if (clearedCount > 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"  ✅ {(_dryRun ? "Would clear" : "Cleared")} {clearedCount} restore/shadow copy items");
                Console.ResetColor();
                
                if (!_dryRun)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("  ⚠️ Note: System Restore Points and Shadow Copies deleted. Previous file versions are no longer available.");
                    Console.ResetColor();
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("  ⚠️ No restore points or shadow copies to clear");
                Console.ResetColor();
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  ❌ Failed: {ex.Message}");
            Console.ResetColor();
        }
    }

    static void ClearDeepSystemTraces()
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("\n🔹 Clearing deep system activity traces...");
        Console.ResetColor();

        long freedSpace = 0;
        int clearedCount = 0;

        try
        {
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            LogDryRun("\n=== DEEP SYSTEM TRACES ===");

            // Windows Prefetch (tracks program launches)
            var prefetchPath = @"C:\Windows\Prefetch";
            if (Directory.Exists(prefetchPath))
            {
                try
                {
                    var files = Directory.GetFiles(prefetchPath, "*.pf");
                    foreach (var file in files)
                    {
                        try
                        {
                            var fi = new FileInfo(file);
                            freedSpace += fi.Length;
                            
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete: {file} ({FormatBytes(fi.Length)})");
                            }
                            else
                            {
                                fi.Delete();
                            }
                            clearedCount++;
                        }
                        catch { }
                    }
                }
                catch { }
            }

            // RecentFileCache.bcf (recent file access tracking)
            var recentFileCachePath = @"C:\Windows\AppCompat\Programs\RecentFileCache.bcf";
            if (File.Exists(recentFileCachePath))
            {
                try
                {
                    var fi = new FileInfo(recentFileCachePath);
                    freedSpace += fi.Length;
                    
                    if (_dryRun)
                    {
                        LogDryRun($"Would delete: {recentFileCachePath} ({FormatBytes(fi.Length)})");
                    }
                    else
                    {
                        fi.Delete();
                    }
                    clearedCount++;
                }
                catch { }
            }

            // AmCache.hve backups (Application Compatibility cache)
            var amcachePath = @"C:\Windows\AppCompat\Programs";
            if (Directory.Exists(amcachePath))
            {
                try
                {
                    var backups = Directory.GetFiles(amcachePath, "Amcache.hve*.bak");
                    foreach (var backup in backups)
                    {
                        try
                        {
                            var fi = new FileInfo(backup);
                            freedSpace += fi.Length;
                            
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete: {backup} ({FormatBytes(fi.Length)})");
                            }
                            else
                            {
                                fi.Delete();
                            }
                            clearedCount++;
                        }
                        catch { }
                    }
                }
                catch { }
            }

            // IconCache.db (icon cache)
            var iconCacheFiles = new[]
            {
                Path.Combine(localAppData, "IconCache.db"),
                Path.Combine(localAppData, "Microsoft", "Windows", "Explorer", "iconcache_*.db")
            };

            foreach (var pattern in iconCacheFiles)
            {
                if (pattern.Contains("*"))
                {
                    var dir = Path.GetDirectoryName(pattern);
                    if (Directory.Exists(dir))
                    {
                        var files = Directory.GetFiles(dir, Path.GetFileName(pattern));
                        foreach (var file in files)
                        {
                            try
                            {
                                var fi = new FileInfo(file);
                                freedSpace += fi.Length;
                                
                                if (_dryRun)
                                {
                                    LogDryRun($"Would delete: {file} ({FormatBytes(fi.Length)})");
                                }
                                else
                                {
                                    fi.Delete();
                                }
                                clearedCount++;
                            }
                            catch { }
                        }
                    }
                }
                else if (File.Exists(pattern))
                {
                    try
                    {
                        var fi = new FileInfo(pattern);
                        freedSpace += fi.Length;
                        
                        if (_dryRun)
                        {
                            LogDryRun($"Would delete: {pattern} ({FormatBytes(fi.Length)})");
                        }
                        else
                        {
                            fi.Delete();
                        }
                        clearedCount++;
                    }
                    catch { }
                }
            }

            // Thumbcache (thumbnail cache)
            var thumbcachePath = Path.Combine(localAppData, "Microsoft", "Windows", "Explorer");
            if (Directory.Exists(thumbcachePath))
            {
                try
                {
                    var thumbcaches = Directory.GetFiles(thumbcachePath, "thumbcache_*.db");
                    foreach (var thumbcache in thumbcaches)
                    {
                        try
                        {
                            var fi = new FileInfo(thumbcache);
                            freedSpace += fi.Length;
                            
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete: {thumbcache} ({FormatBytes(fi.Length)})");
                            }
                            else
                            {
                                fi.Delete();
                            }
                            clearedCount++;
                        }
                        catch { }
                    }
                }
                catch { }
            }

            // Windows Search database (contains search history and indexed content metadata)
            var searchDbPath = @"C:\ProgramData\Microsoft\Search\Data\Applications\Windows";
            if (Directory.Exists(searchDbPath))
            {
                try
                {
                    // Clear logs, not the main database (would break search)
                    var files = Directory.GetFiles(searchDbPath, "*.log", SearchOption.AllDirectories)
                        .Concat(Directory.GetFiles(searchDbPath, "*.jrs", SearchOption.AllDirectories));
                    
                    foreach (var file in files)
                    {
                        try
                        {
                            var fi = new FileInfo(file);
                            freedSpace += fi.Length;
                            
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete: {file} ({FormatBytes(fi.Length)})");
                            }
                            else
                            {
                                fi.Delete();
                            }
                            clearedCount++;
                        }
                        catch { }
                    }
                }
                catch { }
            }

            // Font Cache
            var fontCachePaths = new[]
            {
                @"C:\Windows\ServiceProfiles\LocalService\AppData\Local\FontCache",
                Path.Combine(localAppData, "Microsoft", "Windows", "Caches")
            };

            foreach (var fontPath in fontCachePaths)
            {
                if (Directory.Exists(fontPath))
                {
                    try
                    {
                        var files = Directory.GetFiles(fontPath, "*.dat", SearchOption.AllDirectories);
                        foreach (var file in files)
                        {
                            try
                            {
                                var fi = new FileInfo(file);
                                freedSpace += fi.Length;
                                
                                if (_dryRun)
                                {
                                    LogDryRun($"Would delete: {file} ({FormatBytes(fi.Length)})");
                                }
                                else
                                {
                                    fi.Delete();
                                }
                                clearedCount++;
                            }
                            catch { }
                        }
                    }
                    catch { }
                }
            }

            // Clipboard History (Windows 10+)
            var clipboardPath = Path.Combine(localAppData, "Microsoft", "Windows", "Clipboard");
            if (Directory.Exists(clipboardPath))
            {
                try
                {
                    var files = Directory.GetFiles(clipboardPath, "*.*", SearchOption.AllDirectories);
                    foreach (var file in files)
                    {
                        try
                        {
                            var fi = new FileInfo(file);
                            freedSpace += fi.Length;
                            
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete: {file} ({FormatBytes(fi.Length)})");
                            }
                            else
                            {
                                fi.Delete();
                            }
                            clearedCount++;
                        }
                        catch { }
                    }
                }
                catch { }
            }

            // RDP (Remote Desktop) connection history
            var rdpHistoryFiles = new[]
            {
                Path.Combine(userProfile, "Documents", "Default.rdp"),
                Path.Combine(localAppData, "Microsoft", "Terminal Server Client", "Cache")
            };

            foreach (var rdpPath in rdpHistoryFiles)
            {
                if (File.Exists(rdpPath))
                {
                    try
                    {
                        var fi = new FileInfo(rdpPath);
                        freedSpace += fi.Length;
                        
                        if (_dryRun)
                        {
                            LogDryRun($"Would delete: {rdpPath} ({FormatBytes(fi.Length)})");
                        }
                        else
                        {
                            fi.Delete();
                        }
                        clearedCount++;
                    }
                    catch { }
                }
                else if (Directory.Exists(rdpPath))
                {
                    try
                    {
                        var size = GetDirectorySize(rdpPath);
                        freedSpace += size;
                        
                        if (_dryRun)
                        {
                            LogDryRun($"Would delete: {rdpPath} ({FormatBytes(size)})");
                        }
                        else
                        {
                            Directory.Delete(rdpPath, true);
                        }
                        clearedCount++;
                    }
                    catch { }
                }
            }

            // Print Spooler logs and cache
            var printSpoolerPaths = new[]
            {
                @"C:\Windows\System32\spool\PRINTERS",
                @"C:\Windows\System32\spool\SERVERS"
            };

            foreach (var spoolPath in printSpoolerPaths)
            {
                if (Directory.Exists(spoolPath))
                {
                    try
                    {
                        var files = Directory.GetFiles(spoolPath, "*.*", SearchOption.AllDirectories);
                        foreach (var file in files)
                        {
                            try
                            {
                                var fi = new FileInfo(file);
                                freedSpace += fi.Length;
                                
                                if (_dryRun)
                                {
                                    LogDryRun($"Would delete: {file} ({FormatBytes(fi.Length)})");
                                }
                                else
                                {
                                    fi.Delete();
                                }
                                clearedCount++;
                            }
                            catch { }
                        }
                    }
                    catch { }
                }
            }

            // Task Scheduler logs
            var taskSchedulerLog = @"C:\Windows\System32\LogFiles\SchedLgU.Txt";
            if (File.Exists(taskSchedulerLog))
            {
                try
                {
                    var fi = new FileInfo(taskSchedulerLog);
                    freedSpace += fi.Length;
                    
                    if (_dryRun)
                    {
                        LogDryRun($"Would delete: {taskSchedulerLog} ({FormatBytes(fi.Length)})");
                    }
                    else
                    {
                        fi.Delete();
                    }
                    clearedCount++;
                }
                catch { }
            }

            // Windows Background Transfer Host (BITS transfers metadata)
            var backgroundTransferPath = Path.Combine(localAppData, "Microsoft", "Windows", "BackgroundTransferHost");
            if (Directory.Exists(backgroundTransferPath))
            {
                try
                {
                    var files = Directory.GetFiles(backgroundTransferPath, "*.log", SearchOption.AllDirectories);
                    foreach (var file in files)
                    {
                        try
                        {
                            var fi = new FileInfo(file);
                            freedSpace += fi.Length;
                            
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete: {file} ({FormatBytes(fi.Length)})");
                            }
                            else
                            {
                                fi.Delete();
                            }
                            clearedCount++;
                        }
                        catch { }
                    }
                }
                catch { }
            }

            // Windows Store cache
            var storeCachePath = Path.Combine(localAppData, "Packages");
            if (Directory.Exists(storeCachePath))
            {
                try
                {
                    var storePackages = Directory.GetDirectories(storeCachePath, "Microsoft.WindowsStore*");
                    foreach (var storePackage in storePackages)
                    {
                        var localCache = Path.Combine(storePackage, "LocalCache");
                        if (Directory.Exists(localCache))
                        {
                            try
                            {
                                var size = GetDirectorySize(localCache);
                                freedSpace += size;
                                
                                if (_dryRun)
                                {
                                    LogDryRun($"Would delete: {localCache} ({FormatBytes(size)})");
                                }
                                else
                                {
                                    Directory.Delete(localCache, true);
                                }
                                clearedCount++;
                            }
                            catch { }
                        }
                    }
                }
                catch { }
            }

            // ShellExperienceHost cache (Start Menu, Action Center)
            try
            {
                var shellPackages = Directory.GetDirectories(storeCachePath, "Microsoft.Windows.ShellExperienceHost*");
                foreach (var shellPackage in shellPackages)
                {
                    var cacheLocations = new[] { "LocalCache", "TempState", "AC\\Temp" };
                    foreach (var cacheDir in cacheLocations)
                    {
                        var cachePath = Path.Combine(shellPackage, cacheDir);
                        if (Directory.Exists(cachePath))
                        {
                            try
                            {
                                var size = GetDirectorySize(cachePath);
                                freedSpace += size;
                                
                                if (_dryRun)
                                {
                                    LogDryRun($"Would delete: {cachePath} ({FormatBytes(size)})");
                                }
                                else
                                {
                                    Directory.Delete(cachePath, true);
                                }
                                clearedCount++;
                            }
                            catch { }
                        }
                    }
                }
            }
            catch { }

            // Windows Firewall logs
            var firewallLogPath = @"C:\Windows\System32\LogFiles\Firewall";
            if (Directory.Exists(firewallLogPath))
            {
                try
                {
                    var files = Directory.GetFiles(firewallLogPath, "*.log", SearchOption.AllDirectories);
                    foreach (var file in files)
                    {
                        try
                        {
                            var fi = new FileInfo(file);
                            freedSpace += fi.Length;
                            
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete: {file} ({FormatBytes(fi.Length)})");
                            }
                            else
                            {
                                fi.Delete();
                            }
                            clearedCount++;
                        }
                        catch { }
                    }
                }
                catch { }
            }

            // Windows Media Player database cache
            var wmpCachePath = Path.Combine(localAppData, "Microsoft", "Media Player");
            if (Directory.Exists(wmpCachePath))
            {
                try
                {
                    var files = Directory.GetFiles(wmpCachePath, "*.wmdb", SearchOption.TopDirectoryOnly);
                    foreach (var file in files)
                    {
                        try
                        {
                            var fi = new FileInfo(file);
                            freedSpace += fi.Length;
                            
                            if (_dryRun)
                            {
                                LogDryRun($"Would delete: {file} ({FormatBytes(fi.Length)})");
                            }
                            else
                            {
                                fi.Delete();
                            }
                            clearedCount++;
                        }
                        catch { }
                    }
                }
                catch { }
            }

            if (clearedCount > 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"  ✅ {(_dryRun ? "Would clear" : "Cleared")} {clearedCount} deep system trace items ({FormatBytes(freedSpace)} {(_dryRun ? "would be freed" : "freed")})");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("  ⚠️ No deep system traces found");
                Console.ResetColor();
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  ❌ Failed: {ex.Message}");
            Console.ResetColor();
        }
    }

    static void ClearWindowsUpgradeFolders()
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("\n🔹 Clearing Windows upgrade temporary folders...");
        Console.ResetColor();

        long freedSpace = 0;
        int clearedCount = 0;

        try
        {
            LogDryRun("\n=== WINDOWS UPGRADE FOLDERS ===");

            // Windows upgrade/installation temporary folders
            var upgradeFolders = new[]
            {
                @"C:\$WINDOWS.~BT",      // Windows 10/11 upgrade files
                @"C:\$WINDOWS.~WS",      // Windows setup files
                @"C:\$Windows.~Q",       // Quality update files
                @"C:\$GetCurrent",       // Windows update installation
                @"C:\$SysReset",         // System reset files
                @"C:\Windows.old",       // Previous Windows installation
                @"C:\Windows\SoftwareDistribution\Download",  // Windows Update downloads
                @"C:\Windows\Temp"       // Windows temp (may have update files)
            };

            foreach (var folder in upgradeFolders)
            {
                if (Directory.Exists(folder))
                {
                    try
                    {
                        // Try to get size, but don't fail if we can't (might be too large or protected)
                        long size = 0;
                        try
                        {
                            size = GetDirectorySizeQuick(folder);
                        }
                        catch { }
                        
                        if (_dryRun)
                        {
                            LogDryRun($"Would delete: {folder} ({FormatBytes(size)})");
                            freedSpace += size;
                            clearedCount++;
                        }
                        else
                        {
                            // These folders require TrustedInstaller permissions
                            Console.WriteLine($"    Attempting to remove {folder}...");
                            var result = TakeOwnershipAndDelete(folder, out string errorMsg);
                            if (result)
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine($"    ✅ Removed {folder} ({FormatBytes(size)})");
                                Console.ResetColor();
                                freedSpace += size;
                                clearedCount++;
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.WriteLine($"    ⚠️ Could not remove {folder}");
                                if (!string.IsNullOrEmpty(errorMsg))
                                {
                                    Console.WriteLine($"       Reason: {errorMsg}");
                                }
                                Console.ResetColor();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (!_dryRun)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"  ❌ Error with {folder}: {ex.Message}");
                            Console.ResetColor();
                        }
                    }
                }
            }

            if (clearedCount > 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"  ✅ {(_dryRun ? "Would clear" : "Cleared")} {clearedCount} Windows upgrade folders ({FormatBytes(freedSpace)} {(_dryRun ? "would be freed" : "freed")})");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("  ⚠️ No Windows upgrade folders found");
                Console.ResetColor();
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  ❌ Failed: {ex.Message}");
            Console.ResetColor();
        }
    }

    static void FlushDnsCache()
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("\n🔹 Flushing DNS cache...");
        Console.ResetColor();

        try
        {
            RunCommand("ipconfig", "/flushdns");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("  ✅ DNS cache flushed successfully");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  ❌ Failed: {ex.Message}");
            Console.ResetColor();
        }
    }

    static void RunDiskCleanup()
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("\n🔹 Running Windows Disk Cleanup...");
        Console.ResetColor();

        try
        {
            // Profile number for our automated cleanup
            const int profileId = 65535;

            // Configure all disk cleanup options via registry
            Console.WriteLine("  Configuring cleanup options...");
            var cleanupOptions = new[]
            {
                "Active Setup Temp Folders",
                "BranchCache",
                "Downloaded Program Files",
                "Internet Cache Files",
                "Offline Pages Files",
                "Old ChkDsk Files",
                "Previous Installations",
                "Recycle Bin",
                "Service Pack Cleanup",
                "Setup Log Files",
                "System error memory dump files",
                "System error minidump files",
                "Temporary Files",
                "Temporary Setup Files",
                "Thumbnail Cache",
                "Update Cleanup",
                "Upgrade Discarded Files",
                "User file versions",
                "Windows Defender",
                "Windows Error Reporting Archive Files",
                "Windows Error Reporting Queue Files",
                "Windows Error Reporting System Archive Files",
                "Windows Error Reporting System Queue Files",
                "Windows ESD installation files",
                "Windows Upgrade Log Files"
            };

            // Set registry keys to enable all cleanup options
            var baseKeyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\VolumeCaches";
            int enabledCount = 0;

            foreach (var option in cleanupOptions)
            {
                try
                {
                    var keyPath = $@"{baseKeyPath}\{option}";
                    using var key = Registry.LocalMachine.OpenSubKey(keyPath, true);
                    if (key != null)
                    {
                        key.SetValue($"StateFlags{profileId:D4}", 2, RegistryValueKind.DWord);
                        enabledCount++;
                    }
                }
                catch
                {
                    // Skip options that don't exist or we can't access
                }
            }

            Console.WriteLine($"  Enabled {enabledCount} cleanup options");

            // Run cleanmgr silently with our configured profile
            Console.WriteLine("  Running cleanup (this may take a few minutes)...");

            var processInfo = new ProcessStartInfo
            {
                FileName = "cleanmgr.exe",
                Arguments = $"/sagerun:{profileId}",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            using var process = Process.Start(processInfo);
            if (process != null)
            {
                // Wait for cleanup to complete (with timeout)
                bool completed = process.WaitForExit(300000); // 5 minute timeout

                if (completed && process.ExitCode == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("  ✅ Disk Cleanup completed successfully");
                    Console.ResetColor();
                }
                else if (!completed)
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine("  ⚠️ Disk Cleanup is taking longer than expected (still running in background)");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine($"  ⚠️ Disk Cleanup completed with code: {process.ExitCode}");
                    Console.ResetColor();
                }
            }
        }
        catch (UnauthorizedAccessException)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("  ❌ Failed: Administrator privileges required to configure Disk Cleanup");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  ❌ Failed: {ex.Message}");
            Console.ResetColor();
        }
    }

    static void RunCommand(string command, string arguments)
    {
        if (_dryRun)
        {
            LogDryRun($"Would run command: {command} {arguments}");
            return;
        }

        var processInfo = new ProcessStartInfo
        {
            FileName = command,
            Arguments = arguments,
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        using var process = Process.Start(processInfo);
        process?.WaitForExit();

        if (process?.ExitCode != 0)
        {
            throw new Exception($"Command '{command} {arguments}' failed with exit code {process?.ExitCode}");
        }
    }

    static string FormatBytes(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        double len = bytes;
        int order = 0;
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len /= 1024;
        }
        return $"{len:0.##} {sizes[order]}";
    }

    static long GetDirectorySize(string path)
    {
        long size = 0;
        try
        {
            var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                try
                {
                    var fileInfo = new FileInfo(file);
                    size += fileInfo.Length;
                }
                catch { }
            }
        }
        catch { }
        return size;
    }

    static bool TakeOwnershipAndDelete(string folderPath, out string errorMessage)
    {
        errorMessage = "";
        string batchFile = null;
        try
        {
            // Try simple deletion first (works for non-protected folders)
            try
            {
                Directory.Delete(folderPath, true);
                if (!Directory.Exists(folderPath))
                {
                    return true;
                }
            }
            catch { }
            
            // For TrustedInstaller-protected folders, use batch file approach
            // CRITICAL: Don't redirect output - write directly to NUL to avoid blocking
            Console.WriteLine($"       Taking ownership (this may take 1-2 minutes for large folders)...");
            
            // Create a batch file to avoid command line escaping issues
            batchFile = Path.Combine(Path.GetTempPath(), $"cleanup_{Guid.NewGuid()}.bat");
            var batchContent = $@"@echo off
takeown /f ""{folderPath}"" /a /r /d y >nul 2>nul
icacls ""{folderPath}"" /grant Administrators:F /t /c /q >nul 2>nul
rd /s /q ""{folderPath}"" >nul 2>nul
exit /b 0
";
            File.WriteAllText(batchFile, batchContent);
            
            var cmdProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = batchFile,
                    UseShellExecute = false,
                    RedirectStandardOutput = false,  // Don't redirect to avoid blocking
                    RedirectStandardError = false,
                    CreateNoWindow = true
                }
            };
            
            cmdProcess.Start();
            
            // Show progress dots while waiting (max 2 minutes)
            int secondsWaited = 0;
            int maxSeconds = 120; // 2 minute timeout
            while (!cmdProcess.HasExited && secondsWaited < maxSeconds)
            {
                System.Threading.Thread.Sleep(5000); // Wait 5 seconds
                secondsWaited += 5;
                Console.Write(".");
                
                // Check if folder is gone (early success detection)
                if (!Directory.Exists(folderPath))
                {
                    break;
                }
            }
            Console.WriteLine(); // New line after dots
            
            if (!cmdProcess.HasExited)
            {
                try { cmdProcess.Kill(); } catch { }
                errorMessage = $"Deletion timed out after {maxSeconds} seconds. Folder may be too large or requires reboot.";
            }
            
            // Clean up batch file
            try { if (batchFile != null && File.Exists(batchFile)) File.Delete(batchFile); } catch { }
            
            // Check if folder was deleted
            if (!Directory.Exists(folderPath))
            {
                return true;
            }
            
            // Final attempt: Try .NET method as last resort
            try
            {
                Directory.Delete(folderPath, true);
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = $"Could not fully remove folder. Try running Disk Cleanup or manually delete on reboot. Error: {ex.Message}";
                return false;
            }
        }
        catch (Exception ex)
        {
            // Clean up batch file
            try { if (batchFile != null && File.Exists(batchFile)) File.Delete(batchFile); } catch { }
            
            errorMessage = ex.Message;
            return false;
        }
    }

    static long GetDirectorySizeQuick(string path)
    {
        // Quick size calculation with timeout to avoid hanging on huge folders
        long size = 0;
        try
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c dir \"{path}\" /s /-c",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
            process.Start();
            
            // Read output with timeout
            var task = System.Threading.Tasks.Task.Run(() => process.StandardOutput.ReadToEnd());
            if (task.Wait(10000)) // 10 second timeout
            {
                var output = task.Result;
                // Parse the total bytes from dir output (last line usually has "X bytes")
                var lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in lines.Reverse())
                {
                    if (line.Contains("bytes") && line.Contains("total"))
                    {
                        var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < parts.Length - 1; i++)
                        {
                            if (parts[i + 1].Contains("bytes"))
                            {
                                var sizeStr = parts[i].Replace(",", "").Replace(".", "");
                                if (long.TryParse(sizeStr, out long parsedSize))
                                {
                                    return parsedSize;
                                }
                            }
                        }
                        break;
                    }
                }
            }
            
            if (!process.HasExited)
            {
                process.Kill();
            }
        }
        catch { }
        
        return size;
    }
}
