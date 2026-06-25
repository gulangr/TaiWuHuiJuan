using System;
using System.Collections.Generic;
using FrameWork;
using UnityEngine;

// Token: 0x0200038A RID: 906
public class UI_LegendaryBookFallen : UIBase
{
	// Token: 0x060035C4 RID: 13764 RVA: 0x001B0710 File Offset: 0x001AE910
	public override void OnInit(ArgumentBox argsBox)
	{
		List<int> fallenList;
		argsBox.Get<List<int>>("Fallen", out fallenList);
		CharacterTable table = base.CGet<CharacterTable>("CharacterTable");
		table.Init(fallenList, null, null, null, null, null, null, null, -1, null, true, null);
	}

	// Token: 0x060035C5 RID: 13765 RVA: 0x001B0754 File Offset: 0x001AE954
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
