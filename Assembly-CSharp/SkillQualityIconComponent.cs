using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001C7 RID: 455
public class SkillQualityIconComponent : Refers
{
	// Token: 0x170002E5 RID: 741
	// (get) Token: 0x06001C18 RID: 7192 RVA: 0x000C2242 File Offset: 0x000C0442
	private List<CImage> _qualityIcons
	{
		get
		{
			return base.CGetList<CImage>("QualityIcon");
		}
	}

	// Token: 0x06001C19 RID: 7193 RVA: 0x000C224F File Offset: 0x000C044F
	private string GetSpriteName(int index)
	{
		return string.Format("{0}{1}", "ui_charactermenu_14_icon_combatskill_", index);
	}

	// Token: 0x06001C1A RID: 7194 RVA: 0x000C2268 File Offset: 0x000C0468
	public void SetLevel(int attainmentLevel)
	{
		attainmentLevel = Mathf.Clamp(attainmentLevel, 0, this._qualityIcons.Count);
		string spriteName = string.Format("{0}{1}", "ui_charactermenu_14_icon_combatskill_", this._qualityIcons.Count - attainmentLevel);
		for (int i = 0; i < this._qualityIcons.Count; i++)
		{
			this._qualityIcons[i].SetSprite((i < attainmentLevel) ? spriteName : this.greySprite, false, null);
		}
	}

	// Token: 0x040015EA RID: 5610
	private const string spritePrefix = "ui_charactermenu_14_icon_combatskill_";

	// Token: 0x040015EB RID: 5611
	private string greySprite = "ui_charactermenu_14_icon_combatskill_4";
}
