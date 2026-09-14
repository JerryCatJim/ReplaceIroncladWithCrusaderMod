namespace ReplaceIroncladWithCrusader.Code.ModConfig;

public static class CrusaderSettings
{
    public static bool ShowCardAnims => RitsuLibModConfig.GetRitsuLibSettingBool("ShowCardAnims");
    public static float CardVolumeOffset => (float)RitsuLibModConfig.GetRitsuLibSettingDouble("CardVolumeOffset");
    public static bool MuteCardSounds => RitsuLibModConfig.GetRitsuLibSettingBool("MuteCardSounds");
    public static bool UseLowHealthIdle => RitsuLibModConfig.GetRitsuLibSettingBool("UseLowHealthIdle");
    public static bool SwitchBannerImmediately => RitsuLibModConfig.GetRitsuLibSettingBool("SwitchBannerImmediately");
    public static bool ShowTeammateCardSelectAnim => RitsuLibModConfig.GetRitsuLibSettingBool("ShowTeammateCardSelectAnim");

    public static bool ToggleEnabled(string key)
    {
        return RitsuLibModConfig.GetRitsuLibSettingBool(key);
    }
}
