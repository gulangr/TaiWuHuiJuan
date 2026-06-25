using System;
using Config;
using FrameWork;
using GameData.Utilities;
using TMPro;
using UnityEngine;

// Token: 0x020002DC RID: 732
public class MouseTipTaiwuVillageStele : MouseTipBase
{
	// Token: 0x06002B82 RID: 11138 RVA: 0x0015355B File Offset: 0x0015175B
	protected override void Init(ArgumentBox argsBox)
	{
		argsBox.Get("OrgTemplateId", out this._orgTemplateId);
		argsBox.Get("Unlocked", out this._unlocked);
	}

	// Token: 0x06002B83 RID: 11139 RVA: 0x00153584 File Offset: 0x00151784
	protected override void OnEnable()
	{
		base.OnEnable();
		sbyte orgTemplateId = this._orgTemplateId;
		bool flag = orgTemplateId < 1 || orgTemplateId > 15;
		if (flag)
		{
			SingletonObject.getInstance<TooltipManager>().HideTips(TipType.TaiwuVillageStele, true);
			AdaptableLog.Warning("MouseTipTaiwuVillageStele is hidden due to missing parameters.", false);
		}
		else
		{
			OrganizationItem config = Organization.Instance[this._orgTemplateId];
			this.title.text = LocalStringManager.GetFormat(LanguageKey.LK_Building_ExpandTaiwuVillage_MouseTip_Title, config.Name);
			this.desc.text = config.TaiwuVillageSteleDesc.ColorReplace();
			this.status.text = LocalStringManager.GetFormat(this.GetPreferStatusTemplate(), config.Name).SetColor(this._unlocked ? "brightblue" : "brightred");
			bool unlocked = this._unlocked;
			if (!unlocked)
			{
				TextMeshProUGUI textMeshProUGUI = this.status;
				textMeshProUGUI.text = textMeshProUGUI.text + "\n" + (string.IsNullOrEmpty(config.VowSpecialHint) ? LanguageKey.LK_Building_ExpandTaiwuVillage_MouseTip_AuthorityPercent.TrFormat(config.Name, config.SectMainStory.Name) : config.VowSpecialHint).SetColor("pinkyellow");
			}
		}
	}

	// Token: 0x06002B84 RID: 11140 RVA: 0x001536B8 File Offset: 0x001518B8
	private LanguageKey GetPreferStatusTemplate()
	{
		WorldMapModel mapModel = SingletonObject.getInstance<WorldMapModel>();
		short taiwuVillageAreaId = mapModel.GetTaiwuVillageAreaId();
		sbyte taiwuVillageStateId = mapModel.Areas[(int)taiwuVillageAreaId].GetConfig().StateID;
		bool sectInTaiwuVillage = MapState.Instance[taiwuVillageStateId].SectID == this._orgTemplateId;
		bool flag = sectInTaiwuVillage;
		LanguageKey result;
		if (flag)
		{
			result = LanguageKey.LK_Building_ExpandTaiwuVillage_MouseTip_Default;
		}
		else
		{
			result = (this._unlocked ? LanguageKey.LK_Building_ExpandTaiwuVillage_MouseTip_Unlocked : LanguageKey.LK_Building_ExpandTaiwuVillage_MouseTip_Locked);
		}
		return result;
	}

	// Token: 0x04001FC0 RID: 8128
	[SerializeField]
	private TextMeshProUGUI title;

	// Token: 0x04001FC1 RID: 8129
	[SerializeField]
	private TextMeshProUGUI desc;

	// Token: 0x04001FC2 RID: 8130
	[SerializeField]
	private TextMeshProUGUI status;

	// Token: 0x04001FC3 RID: 8131
	private sbyte _orgTemplateId;

	// Token: 0x04001FC4 RID: 8132
	private bool _unlocked;
}
