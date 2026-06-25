using System;
using Config;
using FrameWork;
using TMPro;
using UnityEngine;

// Token: 0x02000285 RID: 645
public class MouseTipCombatSkillBreakout : MouseTipBase
{
	// Token: 0x17000489 RID: 1161
	// (get) Token: 0x0600299B RID: 10651 RVA: 0x0013B111 File Offset: 0x00139311
	protected override bool CanStick
	{
		get
		{
			return true;
		}
	}

	// Token: 0x0600299C RID: 10652 RVA: 0x0013B114 File Offset: 0x00139314
	protected override void Init(ArgumentBox argsBox)
	{
		this.Refresh(argsBox);
	}

	// Token: 0x0600299D RID: 10653 RVA: 0x0013B120 File Offset: 0x00139320
	public override void Refresh(ArgumentBox argsBox)
	{
		argsBox.Get("SelectedFirstPageCount", out this._selectedFirstPageCount);
		argsBox.Get("SelectedOtherPageCount", out this._selectedOtherPageCount);
		argsBox.Get("DirectPageCount", out this._directPageCount);
		argsBox.Get("ReversePageCount", out this._reversePageCount);
		argsBox.Get("SkillId", out this._skillId);
		this.UpdateInfo();
	}

	// Token: 0x0600299E RID: 10654 RVA: 0x0013B190 File Offset: 0x00139390
	private void UpdateInfo()
	{
		CombatSkillItem skillConfig = CombatSkill.Instance.GetItem(this._skillId);
		this.RefreshDirectReverse(skillConfig);
		this.RefreshPageCount();
	}

	// Token: 0x0600299F RID: 10655 RVA: 0x0013B1C0 File Offset: 0x001393C0
	private void RefreshDirectReverse(CombatSkillItem skillConfig)
	{
		bool isDirect = this._directPageCount > this._reversePageCount;
		bool isPageEnough = this._directPageCount + this._reversePageCount == 5;
		this.directEffectLabel.text = "     " + CommonUtils.GetSpecialEffectDesc(skillConfig.DirectEffectID);
		this.reverseEffectLabel.text = "     " + CommonUtils.GetSpecialEffectDesc(skillConfig.ReverseEffectID);
		DisableStyleRoot directDisable = this.directEffectLabel.transform.parent.GetComponent<DisableStyleRoot>();
		DisableStyleRoot reverseDisable = this.reverseEffectLabel.transform.parent.GetComponent<DisableStyleRoot>();
		bool flag = isPageEnough;
		if (flag)
		{
			directDisable.SetStyleEffect(!isDirect, false);
			reverseDisable.SetStyleEffect(isDirect, false);
		}
		else
		{
			directDisable.SetStyleEffect(false, false);
			reverseDisable.SetStyleEffect(false, false);
		}
	}

	// Token: 0x060029A0 RID: 10656 RVA: 0x0013B290 File Offset: 0x00139490
	private void RefreshPageCount()
	{
		string expColor = (this._selectedFirstPageCount == 0) ? "brightred" : "brightblue";
		string expColor2 = (this._selectedOtherPageCount < 5) ? "brightred" : "brightblue";
		this.outlinePageCountLabel.text = LocalStringManager.GetFormat(LanguageKey.LK_Skill_Break_Tip_OutlinePage, this._selectedFirstPageCount.ToString().SetColor(expColor));
		this.otherPageCountLabel.text = LocalStringManager.GetFormat(LanguageKey.LK_Skill_Break_Tip_OtherPage, this._selectedOtherPageCount.ToString().SetColor(expColor2));
	}

	// Token: 0x04001E2F RID: 7727
	private int _selectedFirstPageCount;

	// Token: 0x04001E30 RID: 7728
	private int _selectedOtherPageCount;

	// Token: 0x04001E31 RID: 7729
	private int _directPageCount;

	// Token: 0x04001E32 RID: 7730
	private int _reversePageCount;

	// Token: 0x04001E33 RID: 7731
	private short _skillId;

	// Token: 0x04001E34 RID: 7732
	[SerializeField]
	private TextMeshProUGUI outlinePageCountLabel;

	// Token: 0x04001E35 RID: 7733
	[SerializeField]
	private TextMeshProUGUI otherPageCountLabel;

	// Token: 0x04001E36 RID: 7734
	[SerializeField]
	private TextMeshProUGUI directEffectLabel;

	// Token: 0x04001E37 RID: 7735
	[SerializeField]
	private TextMeshProUGUI reverseEffectLabel;
}
