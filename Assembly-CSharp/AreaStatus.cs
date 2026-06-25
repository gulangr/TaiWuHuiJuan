using System;
using System.Runtime.CompilerServices;
using GameData.Domains.Map;
using TMPro;
using UnityEngine;

// Token: 0x020003BF RID: 959
public class AreaStatus : Refers
{
	// Token: 0x06003A18 RID: 14872 RVA: 0x001D95B7 File Offset: 0x001D77B7
	private void Awake()
	{
		this._pyramidLayout = base.GetComponent<PyramidLayout>();
		base.CGet<TextMeshProUGUI>("LegendaryCount").color = Colors.Instance["brokenarea"];
	}

	// Token: 0x06003A19 RID: 14873 RVA: 0x001D95E8 File Offset: 0x001D77E8
	public void SetStatus(AreaDisplayData displayData)
	{
		base.CGet<GameObject>("AdventureBack").SetActive(displayData.AdventureCount > 0);
		base.CGet<GameObject>("InfectedBack").SetActive(displayData.InfectedCount > 0);
		base.CGet<GameObject>("LegendaryBack").SetActive(displayData.LegendaryCount > 0);
		base.CGet<GameObject>("PurpleBamboo").SetActive(displayData.AnyPurpleBamboo);
		ValueTuple<string, string> adventureStyle = this.GetAdventureStyle(displayData);
		base.CGet<CImage>("AdventureIcon").SetSprite(adventureStyle.Item1, false, null);
		base.CGet<TextMeshProUGUI>("AdventureCount").color = Colors.Instance[adventureStyle.Item2];
		base.CGet<TextMeshProUGUI>("AdventureCount").text = displayData.AdventureCount.ToString();
		base.CGet<TextMeshProUGUI>("InfectedCount").text = displayData.InfectedCount.ToString();
		base.CGet<TextMeshProUGUI>("LegendaryCount").text = displayData.LegendaryCount.ToString();
		this._pyramidLayout.UpdateChild();
	}

	// Token: 0x06003A1A RID: 14874 RVA: 0x001D9710 File Offset: 0x001D7910
	[return: TupleElementNames(new string[]
	{
		"sprite",
		"color"
	})]
	private ValueTuple<string, string> GetAdventureStyle(AreaDisplayData displayData)
	{
		string color = "normaladventure";
		bool flag = displayData.AdventureCount == 0;
		ValueTuple<string, string> result;
		if (flag)
		{
			result = new ValueTuple<string, string>(string.Empty, color);
		}
		else
		{
			string sprite = "largemap_part_1_xinxin_10";
			result = new ValueTuple<string, string>(sprite, color);
		}
		return result;
	}

	// Token: 0x040029E6 RID: 10726
	private const string AdventureSpriteNormal = "largemap_part_1_xinxin_10";

	// Token: 0x040029E7 RID: 10727
	private const string AdventureSpriteSect = "largemap_part_1_xinxin_11";

	// Token: 0x040029E8 RID: 10728
	private const string AdventureSpriteMain = "largemap_part_1_xinxin_12";

	// Token: 0x040029E9 RID: 10729
	private PyramidLayout _pyramidLayout;
}
