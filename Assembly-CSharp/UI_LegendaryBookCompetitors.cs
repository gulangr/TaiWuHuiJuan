using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using GameData.Domains.LegendaryBook;
using UnityEngine;

// Token: 0x02000389 RID: 905
public class UI_LegendaryBookCompetitors : UIBase
{
	// Token: 0x060035C1 RID: 13761 RVA: 0x001B05C4 File Offset: 0x001AE7C4
	public override void OnInit(ArgumentBox argsBox)
	{
		List<int> competitorList;
		argsBox.Get<List<int>>("Competitors", out competitorList);
		Dictionary<int, LegendaryBookCharacterRelatedData> characterData;
		argsBox.Get<Dictionary<int, LegendaryBookCharacterRelatedData>>("Characters", out characterData);
		List<global::CharacterTable.CharacterTableFilterData> filterList = new List<global::CharacterTable.CharacterTableFilterData>();
		for (sbyte i = 0; i < 14; i += 1)
		{
			filterList.Add(new global::CharacterTable.CharacterTableFilterData(LocalStringManager.Get(string.Format("LK_LegendaryBook_{0}", i)), Misc.Instance[240 + (int)i].Icon, new List<int>()));
		}
		foreach (int charId in competitorList)
		{
			filterList[(int)characterData[charId].BookType].FilterCharIds.Add(charId);
		}
		global::CharacterTable table = base.CGet<global::CharacterTable>("CharacterTable");
		table.Init(competitorList, null, filterList, null, null, null, null, null, -1, null, true, null);
	}

	// Token: 0x060035C2 RID: 13762 RVA: 0x001B06D0 File Offset: 0x001AE8D0
	protected override void OnClick(Transform btn)
	{
		string btnName = btn.name;
		bool flag = btnName == "ButtonClose";
		if (flag)
		{
			UIManager.Instance.HideUI(this.Element);
		}
	}
}
