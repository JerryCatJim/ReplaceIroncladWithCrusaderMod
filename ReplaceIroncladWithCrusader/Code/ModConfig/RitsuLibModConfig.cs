using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace ReplaceIroncladWithCrusader.Code.ModConfig;

public static class RitsuLibModConfig
{
	private const string ModId = "ReplaceIroncladWithCrusader";

	private static readonly object FileLock = new object();

	private static readonly ConcurrentDictionary<string, JsonNode?> Hot = new ConcurrentDictionary<string, JsonNode>(StringComparer.Ordinal);

	private static readonly Dictionary<string, object> Defaults = new Dictionary<string, object>
	{
		["ShowCardAnims"] = true,
		["CardVolumeOffset"] = 0,
		["MuteCardSounds"] = false,
		["UseLowHealthIdle"] = true,
		["SwitchBannerImmediately"] = false,
		["ShowTeammateCardSelectAnim"] = true
	};

	private static string DataDir => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SlayTheSpire2", ModId);

	private static string SchemaPath => Path.Combine(DataDir, "ritsu_interop_schema.json");

	private static string StatePath => Path.Combine(DataDir, "ritsu_interop_state.json");

	public static object CreateRitsuLibSettingsSchema()
	{
		Directory.CreateDirectory(DataDir);
		File.WriteAllText(SchemaPath, BuildDefaultSchemaJson());
		return SchemaPath;
	}

	private static string BuildDefaultSchemaJson()
	{
		RitsuLibModConfigEntity ritsuLibModConfigEntity = new RitsuLibModConfigEntity();
		ritsuLibModConfigEntity.modDisplayName = SimpleLocUtil.Simple("十字军皮肤", "Crusader Skin");
		RitsuLibModConfigEntity configEntity = ritsuLibModConfigEntity;
		RLMCPage mainPage = new RLMCPage();
        mainPage.pageId = "main";
        mainPage.title = SimpleLocUtil.Simple("主要设置", "Main");
        mainPage.description = SimpleLocUtil.Simple("主要设置", "Main");
        mainPage.sortOrder = 1;
		RLMCSection cardSfxSec = new RLMCSection();
        cardSfxSec.id = "card_sfx";
        cardSfxSec.title = SimpleLocUtil.Simple("卡牌动画和音效", "Card Anim And Sfx");
		ToggleEntry toggle = new ToggleEntry();
		toggle.id = "ShowCardAnims";
		toggle.key = toggle.id;
        toggle.label = SimpleLocUtil.Simple("播放卡牌动画", "Show Card Anims");
        toggle.description = SimpleLocUtil.Simple("关闭后会无法播放卡牌的选择和打出动画以及音效", "If unchecked, card select/play anims and SFX wil not play.");
        cardSfxSec.entries.Add(toggle);
        SliderEntry sliderEntry = new SliderEntry();
        sliderEntry.id = "CardVolumeOffset";
        sliderEntry.key = sliderEntry.id;
        sliderEntry.label = SimpleLocUtil.Simple("卡牌音量调节(分贝)", "Card Volume DB Offset");
        sliderEntry.description = SimpleLocUtil.Simple("默认值0已经很大声，注意保护耳朵", "Default Value 0 DB is already loud. Be careful to protect your ears.");
        sliderEntry.min = -20.0;
        sliderEntry.max = 20.0;
        sliderEntry.step = 1.0;
        cardSfxSec.entries.Add(sliderEntry);
        ToggleEntry toggle2 = new ToggleEntry();
		toggle2.id = "MuteCardSounds";
		toggle2.key = toggle2.id;
        toggle2.label = SimpleLocUtil.Simple("卡牌音效静音", "Mute Card Sounds");
        toggle2.description = SimpleLocUtil.Simple("开启后会恢复至原版游戏默认攻击音效", "If checked, attack cards will use the original SFX.");
        cardSfxSec.entries.Add(toggle2);
		RLMCSection animSec = new RLMCSection();
        animSec.id = "character_anim";
        animSec.title = SimpleLocUtil.Simple("角色动画", "Character Anim");
        ToggleEntry toggle3 = new ToggleEntry();
        toggle3.id = "UseLowHealthIdle";
        toggle3.key = toggle3.id;
        toggle3.label = SimpleLocUtil.Simple("展示低血量站立动画", "Show Low Health Idle Anim");
        toggle3.description = SimpleLocUtil.Simple("关闭后不会使用低血量站立动画", "If unchecked, low health idle anim wil not play.");
        animSec.entries.Add(toggle3);
        ToggleEntry toggle4 = new ToggleEntry();
        toggle4.id = "SwitchBannerImmediately";
        toggle4.key = toggle4.id;
        toggle4.label = SimpleLocUtil.Simple("立即切换旗帜动画", "Switch Banner Immediately");
        toggle4.description = SimpleLocUtil.Simple("开启后播放旗帜动画时不会混合过渡", "If checked, banner anims will have no mixed transition.");
        animSec.entries.Add(toggle4);
        ToggleEntry toggle5 = new ToggleEntry();
        toggle5.id = "ShowTeammateCardSelectAnim";
        toggle5.key = toggle5.id;
        toggle5.label = SimpleLocUtil.Simple("展示队友十字军的选卡动画", "Show Teammate Crusader Card Select Anim");
        toggle5.description = SimpleLocUtil.Simple("关闭后不会播放队友十字军的选卡动画", "If unchecked, teammate Crusader card select anims will not play.");
        animSec.entries.Add(toggle5);
        mainPage.sections.Add(cardSfxSec);
        mainPage.sections.Add(animSec);
        configEntity.pages.Add(mainPage);
		return JsonSerializer.Serialize(configEntity);
	}

	public static void SetRitsuLibSettingValue(string key, object? value)
	{
		SetCore(key, value);
	}

	public static object? GetRitsuLibSettingValue(string key)
	{
		return GetCore(key);
	}

	public static void SaveRitsuLibSettings()
	{
		lock (FileLock)
		{
			string statePath = StatePath;
			JsonObject jsonObject = new JsonObject();
			foreach (KeyValuePair<string, JsonNode> item in Hot)
			{
				jsonObject[item.Key] = ((item.Value == null) ? null : JsonNode.Parse(item.Value.ToJsonString()));
			}
			File.WriteAllText(statePath, jsonObject.ToJsonString(new JsonSerializerOptions
			{
				WriteIndented = true
			}));
		}
	}

	public static bool GetRitsuLibSettingBool(string key)
	{
		return CoerceBool(GetCore(key));
	}

	public static void SetRitsuLibSettingBool(string key, bool value)
	{
		SetCore(key, value);
	}

	public static double GetRitsuLibSettingDouble(string key)
	{
		return CoerceDouble(GetCore(key));
	}

	public static void SetRitsuLibSettingDouble(string key, double value)
	{
		SetCore(key, value);
	}

	public static int GetRitsuLibSettingInt(string key)
	{
		return CoerceInt(GetCore(key));
	}

	public static void SetRitsuLibSettingInt(string key, int value)
	{
		SetCore(key, value);
	}

	public static string? GetRitsuLibSettingString(string key)
	{
		return GetCore(key)?.ToString();
	}

	public static void SetRitsuLibSettingString(string key, string value)
	{
		SetCore(key, value);
	}

	public static void InvokeRitsuLibSettingAction(string key)
	{
		if (!(key == "reset_all"))
		{
			return;
		}
		Hot.Clear();
		try
		{
			if (File.Exists(StatePath))
			{
				File.Delete(StatePath);
			}
		}
		catch
		{
		}
	}

	private static void SetCore(string key, object? value)
	{
		LoadIfNeeded();
		Hot[key] = JsonSerializer.SerializeToNode(value);
	}

	private static object? GetCore(string key)
	{
		LoadIfNeeded();
		if (!Hot.TryGetValue(key, out JsonNode value) || value == null)
		{
			return Defaults.GetValueOrDefault(key);
		}
		if (value is JsonValue jsonValue)
		{
			return jsonValue.GetValue<object>();
		}
		return value;
	}

	private static void LoadIfNeeded()
	{
		if (Hot.Count > 0)
		{
			return;
		}
		lock (FileLock)
		{
			if (Hot.Count > 0 || !File.Exists(StatePath))
			{
				return;
			}
			JsonObject jsonObject = JsonNode.Parse(File.ReadAllText(StatePath))?.AsObject();
			if (jsonObject == null)
			{
				return;
			}
			foreach (KeyValuePair<string, JsonNode> item in jsonObject)
			{
				Hot[item.Key] = item.Value;
			}
		}
	}

	private static bool CoerceBool(object? o)
	{
		if (o is bool)
		{
			if ((bool)o)
			{
				return true;
			}
			return false;
		}
		if (o != null)
		{
			if (o is JsonValue jsonValue)
			{
				bool value;
				return jsonValue.TryGetValue<bool>(out value) && value;
			}
			bool result;
			return bool.TryParse(o.ToString(), out result) && result;
		}
		return false;
	}

	private static double CoerceDouble(object? o)
	{
		if (o != null)
		{
			if (!(o is JsonValue jsonValue))
			{
				if (o is IConvertible value)
				{
					return Convert.ToDouble(value);
				}
				double result;
				return double.TryParse(o.ToString(), out result) ? result : 0.0;
			}
			return jsonValue.GetValue<double>();
		}
		return 0.0;
	}

	private static int CoerceInt(object? o)
	{
		if (o != null)
		{
			if (!(o is JsonValue jsonValue))
			{
				if (o is IConvertible value)
				{
					return Convert.ToInt32(value);
				}
				int result;
				return int.TryParse(o.ToString(), out result) ? result : 0;
			}
			return jsonValue.GetValue<int>();
		}
		return 0;
	}
}
