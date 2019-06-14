using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

public class Asset
{
    public string name;
    public string path;
}

public class Assets
{
    public Dictionary<string, Asset> assets = new Dictionary<string, Asset>();


    public static Assets FromXml(string xml)
    {
        XmlDocument doc = new XmlDocument();

        doc.LoadXml(xml);

        XmlElement root = doc.DocumentElement;

        Assets assets = new Assets();

        for(int i = 0; i < root.ChildNodes.Count; ++i)
        {
            XmlElement child = root.ChildNodes[i] as XmlElement;
            Asset asset = new Asset();
            asset.name = child.GetAttribute("name");
            asset.path = child.GetAttribute("path");

            assets.assets.Add(asset.name, asset);
        }


        return assets;
    }

    public static string ToXml(Assets assets)
    {
        XmlDocument doc = new XmlDocument();
        XmlDeclaration dec = doc.CreateXmlDeclaration("1.0", "utf-8", "yes");
        doc.InsertBefore(dec, doc.DocumentElement);


        XmlElement root = doc.CreateElement("Assets");
        doc.AppendChild(root);
        var it = assets.assets.GetEnumerator();
        while(it.MoveNext())
        {
            XmlElement node = doc.CreateElement("asset");

            root.AppendChild(node);
   
            XmlAttribute name = doc.CreateAttribute("name");
            name.Value = it.Current.Value.name;

            node.Attributes.Append(name);

            XmlAttribute path = doc.CreateAttribute("path");
            path.Value = it.Current.Value.path;

            node.Attributes.Append(path);
        }

        MemoryStream ms = new MemoryStream();
        XmlTextWriter xw = new XmlTextWriter(ms, System.Text.Encoding.UTF8);
        xw.Formatting = Formatting.Indented;
        doc.Save(xw);

        ms = (MemoryStream)xw.BaseStream;
        byte[] bytes = ms.ToArray();
        string xml = System.Text.Encoding.UTF8.GetString(bytes, 3, bytes.Length - 3);

        return xml;
    }

}

/// <summary>
/// Auto gen Code.
/// </summary>
public static class AssetID
{
//ASSET_ID_REPLACE_START
	public const uint R_CONFIG_AIRENBUBING = 10001;
	public const uint R_CONFIG_AIRENBUBING01 = 10002;
	public const uint R_CONFIG_DAFU = 10003;
	public const uint R_CONFIG_M_AIRENHUOJIANPAO = 10004;
	public const uint R_CONFIG_M_AIRENZHANSHI = 10005;
	public const uint R_CONFIG_NVJIANGJUN = 10006;
	public const uint R_CONFIG_PAOSHOU = 10007;
	public const uint R_CONFIG_TROLLGIANT = 10008;
	public const uint R_CONFIG_YINGXIONG_KULOUQISHI = 10009;
	public const uint R_DATABASE_DATA = 20001;
	public const uint R_MATERIAL_ARROW = 30001;
	public const uint R_MATERIAL_DIBAN = 30002;
	public const uint R_MATERIAL_LINE = 30003;
	public const uint R_MATERIAL_TILE = 30004;
	public const uint R_MODEL_AIRENBUBING = 40001;
	public const uint R_MODEL_AIRENBUBING01 = 40002;
	public const uint R_MODEL_DAFU = 40003;
	public const uint R_MODEL_M_AIRENHUOJIANPAO = 40004;
	public const uint R_MODEL_M_AIRENZHANSHI = 40005;
	public const uint R_MODEL_NVJIANGJUN = 40006;
	public const uint R_MODEL_PAOSHOU = 40007;
	public const uint R_MODEL_TROLLGIANT = 40008;
	public const uint R_MODEL_YINGXIONG_KULOUQISHI = 40009;
	public const uint R_SHADER_DIMAIN = 50001;
	public const uint R_SPELL_FX_FX_KULOUFASHI_ATTACK_HIT = 60001;
	public const uint R_SPELL_FX_FX_KULOUFASHI_ATTACK_XULI = 60002;
	public const uint R_SPELL_FX_FX_NVJIANGJUN_ATTACK_01 = 60003;
	public const uint R_SPELL_FX_FX_NVJIANGJUN_ATTACK_02 = 60004;
	public const uint R_SPELL_FX_FX_NVJIANGJUN_ATTACK_03 = 60005;
	public const uint R_SPELL_FX_FX_PAO01_ATTACK = 60006;
	public const uint R_SPELL_FX_FX_PAO01_HIT = 60007;
	public const uint R_SPELL_FX_FX_PAO_PANDAN = 60008;
	public const uint R_SPELL_FX_FX_SIWANG = 60009;
	public const uint R_SPELL_FX_FX_XIAOPAOBING_HIT = 60010;
	public const uint R_SPELL_FX_FX_XIAOPAOBING_PAODAN = 60011;
	public const uint R_TEXTURE_FLOOR = 70001;
//ASSET_ID_REPLACE_END
}

public static class AssetPath
{
    static readonly Dictionary<uint, string> mAssetPath = new Dictionary<uint, string> {
//ASSET_PATH_REPLACE_START
	{ AssetID.R_CONFIG_AIRENBUBING,"r/config/airenbubing.txt" },
	{ AssetID.R_CONFIG_AIRENBUBING01,"r/config/airenbubing01.txt" },
	{ AssetID.R_CONFIG_DAFU,"r/config/dafu.txt" },
	{ AssetID.R_CONFIG_M_AIRENHUOJIANPAO,"r/config/m_airenhuojianpao.txt" },
	{ AssetID.R_CONFIG_M_AIRENZHANSHI,"r/config/m_airenzhanshi.txt" },
	{ AssetID.R_CONFIG_NVJIANGJUN,"r/config/nvjiangjun.txt" },
	{ AssetID.R_CONFIG_PAOSHOU,"r/config/paoshou.txt" },
	{ AssetID.R_CONFIG_TROLLGIANT,"r/config/trollgiant.txt" },
	{ AssetID.R_CONFIG_YINGXIONG_KULOUQISHI,"r/config/yingxiong_kulouqishi.txt" },
	{ AssetID.R_DATABASE_DATA,"r/database/data.bytes" },
	{ AssetID.R_MATERIAL_ARROW,"r/material/arrow.mat" },
	{ AssetID.R_MATERIAL_DIBAN,"r/material/diban.mat" },
	{ AssetID.R_MATERIAL_LINE,"r/material/line.mat" },
	{ AssetID.R_MATERIAL_TILE,"r/material/tile.mat" },
	{ AssetID.R_MODEL_AIRENBUBING,"r/model/airenbubing.prefab" },
	{ AssetID.R_MODEL_AIRENBUBING01,"r/model/airenbubing01.prefab" },
	{ AssetID.R_MODEL_DAFU,"r/model/dafu.prefab" },
	{ AssetID.R_MODEL_M_AIRENHUOJIANPAO,"r/model/m_airenhuojianpao.prefab" },
	{ AssetID.R_MODEL_M_AIRENZHANSHI,"r/model/m_airenzhanshi.prefab" },
	{ AssetID.R_MODEL_NVJIANGJUN,"r/model/nvjiangjun.prefab" },
	{ AssetID.R_MODEL_PAOSHOU,"r/model/paoshou.prefab" },
	{ AssetID.R_MODEL_TROLLGIANT,"r/model/trollgiant.prefab" },
	{ AssetID.R_MODEL_YINGXIONG_KULOUQISHI,"r/model/yingxiong_kulouqishi.prefab" },
	{ AssetID.R_SHADER_DIMAIN,"r/shader/dimain.shader" },
	{ AssetID.R_SPELL_FX_FX_KULOUFASHI_ATTACK_HIT,"r/spell_fx/fx_kuloufashi_attack_hit.prefab" },
	{ AssetID.R_SPELL_FX_FX_KULOUFASHI_ATTACK_XULI,"r/spell_fx/fx_kuloufashi_attack_xuli.prefab" },
	{ AssetID.R_SPELL_FX_FX_NVJIANGJUN_ATTACK_01,"r/spell_fx/fx_nvjiangjun_attack_01.prefab" },
	{ AssetID.R_SPELL_FX_FX_NVJIANGJUN_ATTACK_02,"r/spell_fx/fx_nvjiangjun_attack_02.prefab" },
	{ AssetID.R_SPELL_FX_FX_NVJIANGJUN_ATTACK_03,"r/spell_fx/fx_nvjiangjun_attack_03.prefab" },
	{ AssetID.R_SPELL_FX_FX_PAO01_ATTACK,"r/spell_fx/fx_pao01_attack.prefab" },
	{ AssetID.R_SPELL_FX_FX_PAO01_HIT,"r/spell_fx/fx_pao01_hit.prefab" },
	{ AssetID.R_SPELL_FX_FX_PAO_PANDAN,"r/spell_fx/fx_pao_pandan.prefab" },
	{ AssetID.R_SPELL_FX_FX_SIWANG,"r/spell_fx/fx_siwang.prefab" },
	{ AssetID.R_SPELL_FX_FX_XIAOPAOBING_HIT,"r/spell_fx/fx_xiaopaobing_hit.prefab" },
	{ AssetID.R_SPELL_FX_FX_XIAOPAOBING_PAODAN,"r/spell_fx/fx_xiaopaobing_paodan.prefab" },
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

