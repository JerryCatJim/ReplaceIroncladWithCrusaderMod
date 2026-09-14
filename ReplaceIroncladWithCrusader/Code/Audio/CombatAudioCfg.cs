namespace ReplaceIroncladWithCrusader.Code.Audio;

public static class CombatAudioCfg
{
    public static string GetCrusaderPath(string AudioName)
    {
        string FileName = "";
        string PathHead = "res://crusader_assets/Sound_Crusader/";
        string FileEnd = ".wav";

        switch (AudioName)
        {
            //CardSelect
            case "CardSelect/Smite":
                FileName = "sfx_hero_cru_smite_antic";
                break;
            case "CardSelect/Stun":
                FileName = "sfx_hero_cru_stun_antic";
                break;
            case "CardSelect/Accus":
            case "CardSelect/Heal":
                FileName = "sfx_hero_cru_heal_antic";
                break;
            case "CardSelect/Insp":
            case "CardSelect/Rally":
            case "CardSelect/Tenacity":
                FileName = "sfx_hero_cru_insp_antic";
                break;
            case "CardSelect/Reap":
                FileName = "sfx_hero_cru_reap_antic";
                break;
            case "CardSelect/Bulwark":
            case "CardSelect/Radiance":
                FileName = "sfx_hero_cru_bulwark_antic";
                break;
            case "CardSelect/Holy":
                FileName = "sfx_hero_cru_holy_antic";
                break;
            case "CardSelect/Mercy":
                FileName = "sfx_hero_cru_mercy_antic";
                break;
            //CardPlay
            case "CardPlay/Smite":
                FileName = "sfx_hero_cru_smite_use";
                break;
            case "CardPlay/Stun":
                FileName = "sfx_hero_cru_stun_use";
                break;
            case "CardPlay/Accus":
                FileName = "sfx_hero_cru_accus_use";
                break;
            case "CardPlay/Insp":
                FileName = "sfx_hero_cru_insp_use";
                break;
            case "CardPlay/Rally":
                FileName = "sfx_hero_cru_rallying_use";
                break;
            case "CardPlay/Reap":
                FileName = "sfx_hero_cru_reap_use";
                break;
            case "CardPlay/Heal":
                FileName = "sfx_hero_cru_heal_use";
                break;
            case "CardPlay/Bulwark":
                FileName = "sfx_hero_cru_bulwark_use";
                break;
            case "CardPlay/Holy":
                FileName = "sfx_hero_cru_holy_use";
                break;
            case "CardPlay/Radiance":
                FileName = "sfx_hero_cru_radiance_use";
                break;
            case "CardPlay/Tenacity":
                FileName = "sfx_hero_cru_tenacity_use";
                break;
            case "CardPlay/Mercy":
                FileName = "sfx_hero_cru_insp_use";  //没有用"sfx_hero_cru_mercy_use"，声音有点不合适
                break;
            //CardPlayRecover
            case "CardPlay/Smite_Recover":
                FileName = "sfx_hero_cru_smite_return";
                break;
            case "CardPlay/Stun_Recover":
                FileName = "sfx_hero_cru_stun_return";
                break;
            case "CardPlay/Accus_Recover":
            case "CardPlay/Heal_Recover":
                FileName = "sfx_hero_cru_heal_return";
                break;
            case "CardPlay/Insp_Recover":
            case "CardPlay/Rally_Recover":
            case "CardPlay/Tenacity_Recover":
                FileName = "sfx_hero_cru_insp_return";
                break;
            case "CardPlay/Reap_Recover":
                FileName = "sfx_hero_cru_bulwark_return";  //没有用"sfx_hero_cru_reap_return"，声音有点对不上
                break;
            case "CardPlay/Bulwark_Recover":
            case "CardPlay/Radiance_Recover":
                FileName = "sfx_hero_cru_bulwark_return";
                break;
            case "CardPlay/Holy_Recover":
                FileName = "sfx_hero_cru_holy_return";
                break;
            case "CardPlay/Mercy_Recover":
                FileName = "sfx_hero_cru_insp_return";  //没有用"sfx_hero_cru_mercy_return"，改了动画，而且声音跟游戏不太搭，因为地牢2里用了mercy后战斗结束了
                break;
            //Do nothing
            default:
                break;
        }
        return FileName == "" ? FileName : PathHead + FileName + FileEnd;
    }

    public static float GetCrusaderVolumeDB(string AudioName)
    {
        float TempDB = -10.0f;
        switch (AudioName)
        {
            //CardSelect
            case "CardSelect/Smite":
            case "CardSelect/Stun":

            case "CardSelect/Accus":
            case "CardSelect/Heal":  //Accus和Heal的Select和Recover资源相同

            case "CardSelect/Insp":
            case "CardSelect/Rally":
            case "CardSelect/Tenacity": //Insp,Rally和Tenacity的Select和Recover资源相同

            case "CardSelect/Reap":

            case "CardSelect/Bulwark":
            case "CardSelect/Radiance": //Bulwark和Radiance的Select和Recover资源相同

            case "CardSelect/Holy":
            case "CardSelect/Mercy":
                TempDB = -10;
                break;
            //CardPlay
            case "CardPlay/Smite":
            case "CardPlay/Stun":
                TempDB = -10;
                break;
            case "CardPlay/Accus":
                TempDB = -10;
                break;
            case "CardPlay/Insp":
                TempDB = -6;
                break;
            case "CardPlay/Rally":
                TempDB = -4;
                break;
            case "CardPlay/Reap":
                TempDB = -10;
                break;
            case "CardPlay/Heal":
                TempDB = -8;
                break;
            case "CardPlay/Bulwark":
                TempDB = -8;
                break;
            case "CardPlay/Holy":
                TempDB = -6;
                break;
            case "CardPlay/Radiance":
                TempDB = -7;
                break;
            case "CardPlay/Tenacity":
                TempDB = -8;
                break;
            case "CardPlay/Mercy":
                TempDB = -6;
                break;
            //CardPlayRecover
            case "CardPlay/Smite_Recover":
            case "CardPlay/Stun_Recover":
                TempDB = -14;
                break;
            case "CardPlay/Accus_Recover":
            case "CardPlay/Heal_Recover":
                TempDB = -4;
                break;
            case "CardPlay/Insp_Recover":
            case "CardPlay/Rally_Recover":
            case "CardPlay/Tenacity_Recover":
            case "CardPlay/Mercy_Recover":
                TempDB = -10;
                break;
            case "CardPlay/Reap_Recover":
                TempDB = -4;
                break;
            case "CardPlay/Bulwark_Recover":
            case "CardPlay/Radiance_Recover":
                TempDB = -4;
                break;
            case "CardPlay/Holy_Recover":
                TempDB = -12;
                break;
            //Do nothing
            default:
                TempDB = -10;
                break;
        }
        return TempDB;
    }
}
