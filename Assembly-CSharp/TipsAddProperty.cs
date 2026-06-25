using System;
using Config;
using GameData.Utilities;
using TMPro;

// Token: 0x020002E6 RID: 742
public class TipsAddProperty : Refers
{
	// Token: 0x06002BDB RID: 11227 RVA: 0x0015725C File Offset: 0x0015545C
	public void Set(short propertyId, int value)
	{
		Tester.Assert(propertyId >= 0 && (int)propertyId < CharacterPropertyReferenced.Instance.Count, "");
		CharacterPropertyReferencedItem config = CharacterPropertyReferenced.Instance[propertyId];
		CharacterPropertyDisplayItem display = CharacterPropertyDisplay.Instance[config.DisplayType];
		this.SetData(propertyId, value, false, display.IsPercent, false, !display.IsPercent, true, false);
	}

	// Token: 0x06002BDC RID: 11228 RVA: 0x001572C4 File Offset: 0x001554C4
	public void SetData(short type, int value, bool isRecover = false, bool percent = false, bool isInverse = false, bool showAddMark = true, bool bigIcon = false, bool basedOnPercent = false)
	{
		bool flag = (int)type < CharacterPropertyReferenced.Instance.Count;
		string propertyName;
		string icon;
		if (flag)
		{
			CharacterPropertyDisplayItem configData = CharacterPropertyDisplay.Instance[CharacterPropertyReferenced.Instance[type].DisplayType];
			propertyName = configData.Name;
			if (bigIcon)
			{
				icon = configData.TipsBigIcon;
			}
			else
			{
				icon = configData.TipsIcon;
			}
		}
		else
		{
			CombatSkillPropertyItem configData2 = CombatSkillProperty.Instance[(int)type - CharacterPropertyReferenced.Instance.Count];
			propertyName = configData2.Name;
			icon = configData2.TipsIcon;
		}
		string addMarkStr = showAddMark ? "+" : "";
		string percentStr = percent ? "%" : "";
		bool isBuff = (!isInverse && value >= 0) || (isInverse && value < 0);
		int valueStr = basedOnPercent ? (100 + value) : value;
		string showValueStr = (value >= 0) ? string.Format("{0}{1}", addMarkStr, valueStr) : string.Format("{0}", valueStr);
		base.CGet<CImage>("PropertyIcon").SetSprite(icon, false, null);
		base.CGet<TextMeshProUGUI>("PropertyName").text = TipsAddProperty.FormatPropertyName(propertyName, isRecover);
		base.CGet<TextMeshProUGUI>("AddValue").text = (isBuff ? (showValueStr + percentStr) : "");
		base.CGet<TextMeshProUGUI>("ReduceValue").text = (isBuff ? "" : (showValueStr + percentStr));
	}

	// Token: 0x06002BDD RID: 11229 RVA: 0x00157438 File Offset: 0x00155638
	public void SetData(string icon, string propertyName, int value, bool isRecover = false, bool percent = false, bool isOpposite = false, bool showAddMark = true, bool hideIconIfEmpty = false)
	{
		string addMarkStr = showAddMark ? "+" : "";
		string percentStr = percent ? "%" : "";
		bool isBuff = (!isOpposite && value >= 0) || (isOpposite && value < 0);
		string showValueStr = (value >= 0) ? string.Format("{0}{1}", addMarkStr, value) : string.Format("{0}", value);
		base.CGet<CImage>("PropertyIcon").gameObject.SetActive(!hideIconIfEmpty || !string.IsNullOrEmpty(icon));
		base.CGet<CImage>("PropertyIcon").SetSprite(icon, false, null);
		base.CGet<TextMeshProUGUI>("PropertyName").text = TipsAddProperty.FormatPropertyName(propertyName, isRecover);
		base.CGet<TextMeshProUGUI>("AddValue").text = (isBuff ? (showValueStr + percentStr) : "");
		base.CGet<TextMeshProUGUI>("ReduceValue").text = (isBuff ? "" : (showValueStr + percentStr));
	}

	// Token: 0x06002BDE RID: 11230 RVA: 0x00157540 File Offset: 0x00155740
	public void SetData(short type, string valueStr, bool isRecover = false)
	{
		bool flag = (int)type < CharacterPropertyReferenced.Instance.Count;
		string propertyName;
		string icon;
		if (flag)
		{
			CharacterPropertyDisplayItem configData = CharacterPropertyDisplay.Instance[CharacterPropertyReferenced.Instance[type].DisplayType];
			propertyName = configData.Name;
			icon = configData.TipsIcon;
		}
		else
		{
			CombatSkillPropertyItem configData2 = CombatSkillProperty.Instance[(int)type - CharacterPropertyReferenced.Instance.Count];
			propertyName = configData2.Name;
			icon = configData2.TipsIcon;
		}
		base.CGet<CImage>("PropertyIcon").SetSprite(icon, false, null);
		base.CGet<TextMeshProUGUI>("PropertyName").text = TipsAddProperty.FormatPropertyName(propertyName, isRecover);
		base.CGet<TextMeshProUGUI>("AddValue").text = valueStr;
		base.CGet<TextMeshProUGUI>("ReduceValue").text = "";
	}

	// Token: 0x06002BDF RID: 11231 RVA: 0x0015760A File Offset: 0x0015580A
	private static string FormatPropertyName(string propertyName, bool isRecover)
	{
		return isRecover ? LocalStringManager.GetFormat(LanguageKey.LK_TipsAddProperty_PropertyRecover, propertyName) : propertyName;
	}
}
