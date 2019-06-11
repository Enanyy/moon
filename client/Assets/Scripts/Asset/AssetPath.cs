using System;
using System.Collections.Generic;
/// <summary>
/// Auto gen Code.
/// </summary>
public static class AssetID
{
//ASSET_ID_REPLACE_START
	public const uint R_CONFIG_M_AIRENHUOJIANPAO = 10001;
	public const uint R_CONFIG_YINGXIONG_KULOUQISHI = 10002;
	public const uint R_CONFIG_NVJIANGJUN = 10003;
	public const uint R_CONFIG_PAOSHOU = 10004;
	public const uint R_CONFIG_TROLLGIANT = 10006;
	public const uint R_MODEL_YINGXIONG_KULOUQISHI = 20001;
	public const uint R_MODEL_M_AIRENHUOJIANPAO = 20002;
	public const uint R_MODEL_NVJIANGJUN = 20003;
	public const uint R_MODEL_PAOSHOU = 20004;
	public const uint R_MODEL_TROLLGIANT = 20006;
	public const uint R_SPELL_FX_FX_KULOUFASHI_ATTACK_HIT = 30001;
	public const uint R_SPELL_FX_FX_KULOUFASHI_ATTACK_XULI = 30002;
	public const uint R_SPELL_FX_FX_NVJIANGJUN_ATTACK_01 = 30003;
	public const uint R_SPELL_FX_FX_NVJIANGJUN_ATTACK_02 = 30004;
	public const uint R_SPELL_FX_FX_NVJIANGJUN_ATTACK_03 = 30005;
	public const uint R_SPELL_FX_FX_PAO01_ATTACK = 30006;
	public const uint R_SPELL_FX_FX_PAO01_HIT = 30007;
	public const uint R_SPELL_FX_FX_PAO_PANDAN = 30008;
	public const uint R_SPELL_FX_FX_XIAOPAOBING_HIT = 30009;
	public const uint R_SPELL_FX_FX_XIAOPAOBING_PAODAN = 30010;
	public const uint R_SPELL_FX_FX_SIWANG = 30011;
	public const uint R_MATERIAL_DIBAN = 40001;
	public const uint R_MATERIAL_LINE = 40002;
	public const uint R_MATERIAL_ARROW = 40003;
	public const uint R_MATERIAL_TILE = 40004;
	public const uint R_SHADER_DIMAIN = 50001;
	public const uint R_TEXTURE_FLOOR = 60001;
//ASSET_ID_REPLACE_END
}

public static class AssetPath
{
    static readonly Dictionary<uint, string> mAssetPath = new Dictionary<uint, string> {
//ASSET_PATH_REPLACE_START
	{ AssetID.R_CONFIG_M_AIRENHUOJIANPAO,"r/config/m_airenhuojianpao1.txt" },
	{ AssetID.R_CONFIG_YINGXIONG_KULOUQISHI,"r/config/yingxiong_kulouqishi.txt" },
	{ AssetID.R_CONFIG_NVJIANGJUN,"r/config/nvjiangjun.txt" },
	{ AssetID.R_CONFIG_PAOSHOU,"r/config/paoshou.txt" },
	{ AssetID.R_CONFIG_TROLLGIANT,"r/config/trollgiant.txt" },
	{ AssetID.R_MODEL_YINGXIONG_KULOUQISHI,"r/model/yingxiong_kulouqishi.prefab" },
	{ AssetID.R_MODEL_M_AIRENHUOJIANPAO,"r/model/m_airenhuojianpao.prefab" },
	{ AssetID.R_MODEL_NVJIANGJUN,"r/model/nvjiangjun.prefab" },
	{ AssetID.R_MODEL_PAOSHOU,"r/model/paoshou.prefab" },
	{ AssetID.R_MODEL_TROLLGIANT,"r/model/trollgiant.prefab" },
	{ AssetID.R_SPELL_FX_FX_KULOUFASHI_ATTACK_HIT,"r/spell_fx/fx_kuloufashi_attack_hit.prefab" },
	{ AssetID.R_SPELL_FX_FX_KULOUFASHI_ATTACK_XULI,"r/spell_fx/fx_kuloufashi_attack_xuli.prefab" },
	{ AssetID.R_SPELL_FX_FX_NVJIANGJUN_ATTACK_01,"r/spell_fx/fx_nvjiangjun_attack_01.prefab" },
	{ AssetID.R_SPELL_FX_FX_NVJIANGJUN_ATTACK_02,"r/spell_fx/fx_nvjiangjun_attack_02.prefab" },
	{ AssetID.R_SPELL_FX_FX_NVJIANGJUN_ATTACK_03,"r/spell_fx/fx_nvjiangjun_attack_03.prefab" },
	{ AssetID.R_SPELL_FX_FX_PAO01_ATTACK,"r/spell_fx/fx_pao01_attack.prefab" },
	{ AssetID.R_SPELL_FX_FX_PAO01_HIT,"r/spell_fx/fx_pao01_hit.prefab" },
	{ AssetID.R_SPELL_FX_FX_PAO_PANDAN,"r/spell_fx/fx_pao_pandan.prefab" },
	{ AssetID.R_SPELL_FX_FX_XIAOPAOBING_HIT,"r/spell_fx/fx_xiaopaobing_hit.prefab" },
	{ AssetID.R_SPELL_FX_FX_XIAOPAOBING_PAODAN,"r/spell_fx/fx_xiaopaobing_paodan.prefab" },
	{ AssetID.R_SPELL_FX_FX_SIWANG,"r/spell_fx/fx_siwang.prefab" },
	{ AssetID.R_MATERIAL_DIBAN,"r/material/diban.mat" },
	{ AssetID.R_MATERIAL_LINE,"r/material/line.mat" },
	{ AssetID.R_MATERIAL_ARROW,"r/material/arrow.mat" },
	{ AssetID.R_MATERIAL_TILE,"r/material/tile.mat" },
	{ AssetID.R_SHADER_DIMAIN,"r/shader/dimain.shader" },
	{ AssetID.R_TEXTURE_FLOOR,"r/texture/floor.png" },
//ASSET_PATH_REPLACE_END
    };

    public static Dictionary<uint, string> GetAllAssets()
    {
        return mAssetPath;
    }

    public static string Get(uint assetID)
    {
        if(mAssetPath.ContainsKey(assetID))
        {
            string path = mAssetPath[assetID];
            path = path.Substring(0, path.LastIndexOf('.'));
            return path;
        }
        return "";
    }
}

