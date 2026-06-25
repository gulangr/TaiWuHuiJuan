using System;
using TMPro;
using UnityEngine;

// Token: 0x020003BE RID: 958
public class AreaName : Refers
{
	// Token: 0x06003A14 RID: 14868 RVA: 0x001D9502 File Offset: 0x001D7702
	private void Awake()
	{
		this._defaultColor = base.CGet<TextMeshProUGUI>("Name").color;
		this._awaken = true;
	}

	// Token: 0x06003A15 RID: 14869 RVA: 0x001D9524 File Offset: 0x001D7724
	public void SetBroken(bool isBroken)
	{
		bool flag = !this._awaken;
		if (flag)
		{
			this.Awake();
		}
		base.CGet<TextMeshProUGUI>("Name").color = (isBroken ? Colors.Instance["brokenarea"] : this._defaultColor);
		base.CGet<CImage>("NameBack").SetSprite(isBroken ? "largemap_part_1_nameposui" : "largemap_part_1_name", false, null);
	}

	// Token: 0x06003A16 RID: 14870 RVA: 0x001D9593 File Offset: 0x001D7793
	public void ShowFiveLoongIcon(bool show)
	{
		base.CGet<GameObject>("FiveLoongIcon").SetActive(show);
	}

	// Token: 0x040029E2 RID: 10722
	private const string NormalAreaSprite = "largemap_part_1_name";

	// Token: 0x040029E3 RID: 10723
	private const string BrokenAreaSprite = "largemap_part_1_nameposui";

	// Token: 0x040029E4 RID: 10724
	private Color _defaultColor;

	// Token: 0x040029E5 RID: 10725
	private bool _awaken = false;
}
