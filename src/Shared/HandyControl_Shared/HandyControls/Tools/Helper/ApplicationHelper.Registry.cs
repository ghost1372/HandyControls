using Microsoft.Win32;

namespace HandyControl.Tools;

public static partial class ApplicationHelper
{
    #region Directory

    /// <summary>
    /// Register Context Menu in Directory
    /// Use this method when you want to add a ContextMenu when you right-click on folders
    /// </summary>
    /// <param name="ContextMenuName"></param>
    /// <param name="Command"></param>
    /// <param name="IconPath">Icon Should be in *.ico format</param>
    public static void RegisterContextMenuToDirectory(string ContextMenuName, string Command, string IconPath = null)
    {
        string _DirectoryShell = $@"SOFTWARE\Classes\directory\shell\{ContextMenuName}\command\";
        string _Icon = $@"SOFTWARE\Classes\directory\shell\{ContextMenuName}\";
        RegistryHelper.AddOrUpdateKey("", _DirectoryShell, Command);
        if (!string.IsNullOrEmpty(IconPath))
        {
            RegistryHelper.AddOrUpdateKey("Icon", _Icon, IconPath);
        }
    }

    /// <summary>
    /// UnRegister Context Menu from Directory
    /// Use this method when you want to remove a ContextMenu when you right-click on folders
    /// </summary>
    /// <param name="ContextMenuName"></param>
    public static bool UnRegisterContextMenuFromDirectory(string ContextMenuName)
    {
        string _RemovePath = $@"SOFTWARE\Classes\directory\shell\";
        return RegistryHelper.DeleteSubKeyTree(ContextMenuName, _RemovePath);
    }

    /// <summary>
    /// Register Cascade Context Menu in Directory
    /// Use this method when you want to add a Cascade Menu in ContextMenu when you right-click on folders
    /// This Method need Administrator Access
    /// </summary>
    /// <param name="ContextMenuName"></param>
    /// <param name="CascadeContextMenuName"></param>
    /// <param name="Command"></param>
    /// <param name="IconPath">Icon Should be in *.ico format</param>
    /// <param name="CascadeIconPath">Icon Should be in *.ico format</param>
    public static void RegisterCascadeContextMenuToDirectory(string ContextMenuName, string CascadeContextMenuName, string Command, string IconPath = null, string CascadeIconPath = null)
    {
        string Folder = $@"Directory\Shell\{ContextMenuName}";
        RegisterCascade(Folder, ContextMenuName, CascadeContextMenuName, Command, IconPath, CascadeIconPath);
    }

    /// <summary>
    /// UnRegister Cascade Context Menu from Directory
    /// Use this method when you want to remove a Cascade Menu from ContextMenu when you right-click on folders
    /// This Method need Administrator Access
    /// </summary>
    /// <param name="ContextMenuName"></param>
    /// <param name="CascadeContextMenuName"></param>
    /// <returns></returns>
    public static bool UnRegisterCascadeContextMenuFromDirectory(string ContextMenuName, string CascadeContextMenuName = null)
    {
        var _RemovePath = $@"Directory\Shell";
        return UnRegisterCascade(_RemovePath, ContextMenuName, CascadeContextMenuName);
    }

    #endregion

    #region File
    /// <summary>
    /// Register Context Menu in File
    /// Use this method when you want to add a ContextMenu when you right-click on files
    /// </summary>
    /// <param name="ContextMenuName"></param>
    /// <param name="Command"></param>
    /// <param name="IconPath">Icon Should be in *.ico format</param>
    public static void RegisterContextMenuToFile(string ContextMenuName, string Command, string IconPath = null)
    {
        string _FileShell = $@"SOFTWARE\Classes\*\shell\{ContextMenuName}\command\";
        string _Icon = $@"SOFTWARE\Classes\*\shell\{ContextMenuName}\";

        RegistryHelper.AddOrUpdateKey("", _FileShell, Command);
        if (!string.IsNullOrEmpty(IconPath))
        {
            RegistryHelper.AddOrUpdateKey("Icon", _Icon, IconPath);
        }
    }

    /// <summary>
    /// UnRegister Context Menu from File
    /// Use this method when you want to remove a ContextMenu when you right-click on files
    /// </summary>
    /// <param name="ContextMenuName"></param>
    public static bool UnRegisterContextMenuFromFile(string ContextMenuName)
    {
        var _RemovePath = @"SOFTWARE\Classes\*\shell\";
        return RegistryHelper.DeleteSubKeyTree(ContextMenuName, _RemovePath);
    }

    /// <summary>
    /// Register Cascade Context Menu in File
    /// Use this method when you want to add a Cascade Menu in ContextMenu when you right-click on files
    /// This Method need Administrator Access
    /// </summary>
    /// <param name="ContextMenuName"></param>
    /// <param name="CascadeContextMenuName"></param>
    /// <param name="Command"></param>
    /// <param name="IconPath">Icon Should be in *.ico format</param>
    /// <param name="CascadeIconPath">Icon Should be in *.ico format</param>
    public static void RegisterCascadeContextMenuToFile(string ContextMenuName, string CascadeContextMenuName, string Command, string IconPath = null, string CascadeIconPath = null)
    {
        string Folder = $@"*\Shell\{ContextMenuName}";
        RegisterCascade(Folder, ContextMenuName, CascadeContextMenuName, Command, IconPath, CascadeIconPath);
    }

    /// <summary>
    /// UnRegister Cascade Context Menu from File
    /// Use this method when you want to remove a Cascade Menu from ContextMenu when you right-click on files
    /// This Method need Administrator Access
    /// </summary>
    /// <param name="ContextMenuName"></param>
    /// <param name="CascadeContextMenuName"></param>
    /// <returns></returns>
    public static bool UnRegisterCascadeContextMenuFromFile(string ContextMenuName, string CascadeContextMenuName = null)
    {
        var _RemovePath = $@"*\Shell";
        return UnRegisterCascade(_RemovePath, ContextMenuName, CascadeContextMenuName);
    }
    #endregion

    #region Background
    /// <summary>
    /// Register Context Menu in Background
    /// Use this method when you want to add a ContextMenu when you right-click on desktop or Explorer background
    /// </summary>
    /// <param name="ContextMenuName"></param>
    /// <param name="Command"></param>
    /// <param name="IconPath">Icon Should be in *.ico format</param>
    public static void RegisterContextMenuToBackground(string ContextMenuName, string Command, string IconPath = null)
    {
        string _DirectoryShell = $@"SOFTWARE\Classes\directory\background\shell\{ContextMenuName}\command\";
        string _Icon = $@"SOFTWARE\Classes\directory\background\shell\{ContextMenuName}\";

        RegistryHelper.AddOrUpdateKey("", _DirectoryShell, Command);
        if (!string.IsNullOrEmpty(IconPath))
        {
            RegistryHelper.AddOrUpdateKey("Icon", _Icon, IconPath);
        }
    }

    /// <summary>
    /// UnRegister Context Menu from Background
    /// Use this method when you want to remove a ContextMenu when you right-click on desktop or Explorer background
    /// </summary>
    /// <param name="ContextMenuName"></param>
    public static bool UnRegisterContextMenuFromBackground(string ContextMenuName)
    {
        string _RemovePath = $@"SOFTWARE\Classes\directory\background\shell\";
        return RegistryHelper.DeleteSubKeyTree(ContextMenuName, _RemovePath);
    }

    /// <summary>
    /// Register Cascade Context Menu in Background
    /// Use this method when you want to add a Cascade Menu in ContextMenu when you right-click on desktop or Explorer background
    /// This Method need Administrator Access
    /// </summary>
    /// <param name="ContextMenuName"></param>
    /// <param name="CascadeContextMenuName"></param>
    /// <param name="Command"></param>
    /// <param name="IconPath">Icon Should be in *.ico format</param>
    /// <param name="CascadeIconPath">Icon Should be in *.ico format</param>
    public static void RegisterCascadeContextMenuToBackground(string ContextMenuName, string CascadeContextMenuName, string Command, string IconPath = null, string CascadeIconPath = null)
    {
        string Folder = $@"Directory\Background\Shell\{ContextMenuName}";
        RegisterCascade(Folder, ContextMenuName, CascadeContextMenuName, Command, IconPath, CascadeIconPath);
    }

    /// <summary>
    /// UnRegister Cascade Context Menu from Background
    /// Use this method when you want to remove a Cascade Menu from ContextMenu when you right-click on desktop or Explorer background
    /// This Method need Administrator Access
    /// </summary>
    /// <param name="ContextMenuName"></param>
    /// <param name="CascadeContextMenuName"></param>
    /// <returns></returns>
    public static bool UnRegisterCascadeContextMenuFromBackground(string ContextMenuName, string CascadeContextMenuName = null)
    {
        var _RemovePath = $@"Directory\Background\Shell";
        return UnRegisterCascade(_RemovePath, ContextMenuName, CascadeContextMenuName);
    }

    #endregion

    #region Drive

    /// <summary>
    /// Register Context Menu in Drive
    /// Use this method when you want to add a ContextMenu when you right-click on drives
    /// This Method need Administrator Access
    /// </summary>
    /// <param name="ContextMenuName"></param>
    /// <param name="Command"></param>
    /// <param name="IconPath">Icon Should be in *.ico format</param>
    public static void RegisterContextMenuToDrive(string ContextMenuName, string Command, string IconPath = null)
    {
        string _DirectoryShell = $@"Drive\shell\{ContextMenuName}\command\";
        string _Icon = $@"Drive\shell\{ContextMenuName}\";

        RegistryHelper.AddOrUpdateKey("", _DirectoryShell, Command, Registry.ClassesRoot);
        if (!string.IsNullOrEmpty(IconPath))
        {
            RegistryHelper.AddOrUpdateKey("Icon", _Icon, IconPath, Registry.ClassesRoot);
        }
    }

    /// <summary>
    /// UnRegister Context Menu from Drive
    /// Use this method when you want to remove a ContextMenu when you right-click on drives
    /// This Method need Administrator Access
    /// </summary>
    /// <param name="ContextMenuName"></param>
    public static bool UnRegisterContextMenuFromDrive(string ContextMenuName)
    {
        string _RemovePath = $@"Drive\shell\";
        return RegistryHelper.DeleteSubKeyTree(ContextMenuName, _RemovePath, Registry.ClassesRoot);
    }

    /// <summary>
    /// Register Cascade Context Menu in Drive
    /// Use this method when you want to add a Cascade Menu in ContextMenu when you right-click on drives
    /// This Method need Administrator Access
    /// </summary>
    /// <param name="ContextMenuName"></param>
    /// <param name="CascadeContextMenuName"></param>
    /// <param name="Command"></param>
    /// <param name="IconPath">Icon Should be in *.ico format</param>
    /// <param name="CascadeIconPath">Icon Should be in *.ico format</param>
    public static void RegisterCascadeContextMenuToDrive(string ContextMenuName, string CascadeContextMenuName, string Command, string IconPath = null, string CascadeIconPath = null)
    {
        string Folder = $@"Drive\Shell\{ContextMenuName}";
        RegisterCascade(Folder, ContextMenuName, CascadeContextMenuName, Command, IconPath, CascadeIconPath);
    }

    /// <summary>
    /// UnRegister Cascade Context Menu from Drive
    /// Use this method when you want to remove a Cascade Menu from ContextMenu when you right-click on drives
    /// This Method need Administrator Access
    /// </summary>
    /// <param name="ContextMenuName"></param>
    /// <param name="CascadeContextMenuName"></param>
    /// <returns></returns>
    public static bool UnRegisterCascadeContextMenuFromDrive(string ContextMenuName, string CascadeContextMenuName = null)
    {
        var _RemovePath = $@"Drive\Shell";
        return UnRegisterCascade(_RemovePath, ContextMenuName, CascadeContextMenuName);
    }

    #endregion

    private static void RegisterCascade(string Folder, string ContextMenuName, string CascadeContextMenuName, string Command, string IconPath = null, string CascadeIconPath = null)
    {
        RegistryHelper.AddOrUpdateKey(string.Empty, Folder, string.Empty, Registry.ClassesRoot);
        RegistryHelper.AddOrUpdateKey("MUIVerb", Folder, ContextMenuName, Registry.ClassesRoot);
        RegistryHelper.AddOrUpdateKey("subcommands", Folder, string.Empty, Registry.ClassesRoot);
        RegistryHelper.AddOrUpdateKey(string.Empty, $@"{Folder}\shell", string.Empty, Registry.ClassesRoot);
        RegistryHelper.AddOrUpdateKey(string.Empty, $@"{Folder}\shell\{CascadeContextMenuName}", string.Empty, Registry.ClassesRoot);
        RegistryHelper.AddOrUpdateKey(string.Empty, $@"{Folder}\shell\{CascadeContextMenuName}\command", Command, Registry.ClassesRoot);

        if (!string.IsNullOrEmpty(IconPath))
        {
            RegistryHelper.AddOrUpdateKey("Icon", Folder, IconPath, Registry.ClassesRoot);
        }

        if (!string.IsNullOrEmpty(CascadeIconPath))
        {
            RegistryHelper.AddOrUpdateKey("Icon", $@"{Folder}\shell\{CascadeContextMenuName}", CascadeIconPath, Registry.ClassesRoot);
        }
    }

    private static bool UnRegisterCascade(string Folder, string ContextMenuName, string CascadeContextMenuName)
    {
        var _RemovePathCascade = $@"{Folder}\{ContextMenuName}\shell\";

        if (string.IsNullOrEmpty(CascadeContextMenuName))
        {
            return RegistryHelper.DeleteSubKeyTree(ContextMenuName, Folder, Registry.ClassesRoot);
        }
        else
        {
            return RegistryHelper.DeleteSubKeyTree(CascadeContextMenuName, _RemovePathCascade, Registry.ClassesRoot);
        }
    }
}
