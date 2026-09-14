using Godot;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using ReplaceIroncladWithCrusaderMod.Code;
using System.IO;
using System.Linq;

namespace ReplaceIroncladWithCrusader.Code.Config;

public static class CrusaderConfigLoader
{
    // 内部资源路径（打包在 PCK 内）
    private const string InternalConfigPath = "res://CrusaderConfig.json";

    private static Godot.Collections.Dictionary _cachedConfig = null;
    public static Godot.Collections.Dictionary GetCachedConfig()
    {
        if (_cachedConfig == null)
        {
            _cachedConfig = LoadConfig();
        }
        return _cachedConfig;
    }
    /// <summary>
    /// 加载配置文件，优先读取外部路径（游戏 exe 同级目录），
    /// 如果外部不存在则读取内部资源，若都不存在则返回 null。
    /// </summary>
    public static Godot.Collections.Dictionary LoadConfig()
    {
        string? jsonText = GetExternalConfigJson();
        if (jsonText == null)
        {
            //回退到内部资源（res://）
            jsonText = LoadInternalConfig();
            if (jsonText != null)
            {
                GD.Print("Loaded config from internal (res://)");
            }
        }

        if (string.IsNullOrEmpty(jsonText))
        {
            GD.PushError("Config file not found in either external or internal path.");
            return null;
        }

        // 解析 JSON
        var jsonVariant = Json.ParseString(jsonText);
        if (jsonVariant.VariantType == Variant.Type.Nil)
        {
            GD.PushError("Invalid JSON format.");
            return null;
        }

        return jsonVariant.As<Godot.Collections.Dictionary>();
    }

    /// <summary>
    /// 获取外部配置文件路径（位于可执行文件同级目录）
    /// </summary>
    private static string? GetExternalConfigJson()
    {
        string? json = null;
        var mod = ModManager.GetLoadedMods()
        .FirstOrDefault(m => m.manifest?.id == Entry.ModId);

        if (mod != null)
        {
            string jsonPath = Path.Combine(mod.path, "CrusaderConfig.json");
            if (File.Exists(jsonPath))
            {
                json = File.ReadAllText(jsonPath);
            }
        }
        return json;
    }

    /// <summary>
    /// 从内部资源（res://）加载 JSON 文本
    /// </summary>
    private static string LoadInternalConfig()
    {
        if (!ResourceLoader.Exists(InternalConfigPath))
        {
            GD.PushError($"Internal config not found: {InternalConfigPath}");
            return null;
        }

        using var file = Godot.FileAccess.Open(InternalConfigPath, Godot.FileAccess.ModeFlags.Read);
        if (file == null)
            return null;

        return file.GetAsText();
    }

    // 便捷取值方法（可选）
    public static Variant GetValue(string key, Variant defaultValue = default)
    {
        var dict = GetCachedConfig();
        if (dict == null || !dict.ContainsKey(key))
            return defaultValue;
        return dict[key];
    }

    public static string GetValueAsString(string key)
    {
        var dict = GetCachedConfig();
        if (dict == null || !dict.ContainsKey(key))
            return "";
        return dict[key].AsString();
    }
}