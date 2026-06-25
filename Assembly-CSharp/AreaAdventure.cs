using System;
using Config;
using TMPro;
using UnityEngine;

// Token: 0x020003BB RID: 955
public class AreaAdventure : Refers
{
	// Token: 0x06003A0C RID: 14860 RVA: 0x001D90D0 File Offset: 0x001D72D0
	public void Refresh(sbyte adventureType, int adventureCount, int level)
	{
		if (!true)
		{
		}
		string text;
		if (adventureType != 1)
		{
			if (adventureType != 2)
			{
				if (adventureType != 15)
				{
					text = "AnimCommon";
				}
				else
				{
					text = "AnimSword";
				}
			}
			else
			{
				text = "AnimSect";
			}
		}
		else
		{
			text = "AnimMain";
		}
		if (!true)
		{
		}
		string activatedAnimName = text;
		foreach (GameObject anim in this.CGetIter<GameObject>("Anim"))
		{
			anim.SetActive(anim.name == activatedAnimName);
		}
		AdventureTypeItem config = AdventureType.Instance[adventureType];
		string color = config.ColorName.IsNullOrEmpty() ? "normaladventure" : config.ColorName;
		base.CGet<TextMeshProUGUI>("Text").text = LocalStringManager.GetFormat(LanguageKey.LK_MapAreaMousetip_Adventure, config.DisplayName, adventureCount).SetColor(color);
		RectTransform levelLayout = base.CGet<RectTransform>("LevelLayout");
		bool needShowLevel = adventureType == 4 || adventureType == 5;
		levelLayout.gameObject.SetActive(needShowLevel);
		bool flag = needShowLevel;
		if (flag)
		{
			for (int i = 0; i < levelLayout.childCount; i++)
			{
				Transform item = levelLayout.GetChild(levelLayout.childCount - 1 - i);
				CImage image = item.Find("Fill").GetComponent<CImage>();
				image.gameObject.SetActive(level > i);
				bool flag2 = level > i;
				if (flag2)
				{
					string spriteName = (adventureType == 4) ? ((i == 0) ? "largemap_part_1_qingshi_0" : "largemap_part_1_qingshi_1") : ((i == 0) ? "largemap_part_1_qingshi_3" : "largemap_part_1_qingshi_4");
					image.SetSprite(spriteName, false, null);
				}
			}
		}
	}

	// Token: 0x040029D8 RID: 10712
	private const string DefaultColor = "normaladventure";

	// Token: 0x040029D9 RID: 10713
	private const string RighteousStrongholdLevelOneSprite = "largemap_part_1_qingshi_3";

	// Token: 0x040029DA RID: 10714
	private const string RighteousStrongholdLevelOverOneSprite = "largemap_part_1_qingshi_4";

	// Token: 0x040029DB RID: 10715
	private const string HereticStrongholdLevelOneSprite = "largemap_part_1_qingshi_0";

	// Token: 0x040029DC RID: 10716
	private const string HereticStrongholdLevelOverOneSprite = "largemap_part_1_qingshi_1";
}
