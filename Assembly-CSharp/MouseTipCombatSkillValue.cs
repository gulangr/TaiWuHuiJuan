using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using GameData.Domains.Character;
using TMPro;

// Token: 0x02000289 RID: 649
public class MouseTipCombatSkillValue : MouseTipBase
{
	// Token: 0x1700048C RID: 1164
	// (get) Token: 0x060029C0 RID: 10688 RVA: 0x0013C735 File Offset: 0x0013A935
	protected override bool CanStick
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060029C1 RID: 10689 RVA: 0x0013C738 File Offset: 0x0013A938
	protected override void Init(ArgumentBox argsBox)
	{
		argsBox.Get("charName", out this._charName);
		argsBox.Get<CombatSkillShorts>("values", out this._combatSkillShorts);
		UIElement element = this.Element;
		element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(this.UpdateSkillInfo));
	}

	// Token: 0x060029C2 RID: 10690 RVA: 0x0013C791 File Offset: 0x0013A991
	private void Awake()
	{
		this._combatSkillRefers = base.CGetList<Refers>("CombatSkill");
	}

	// Token: 0x060029C3 RID: 10691 RVA: 0x0013C7A8 File Offset: 0x0013A9A8
	private unsafe void UpdateSkillInfo()
	{
		base.CGet<TextMeshProUGUI>("TitleName").SetText(LocalStringManager.GetFormat(LanguageKey.LK_Char_CombatSkill_Qualification, this._charName), true);
		for (int i = 0; i < 14; i++)
		{
			CombatSkillTypeItem config = CombatSkillType.Instance[i];
			this._combatSkillRefers[i].CGet<CImage>("SkillIcon").SetSprite(string.Format("sp_18_iconwuxuezhanshi_{0}", i), false, null);
			this._combatSkillRefers[i].CGet<TextMeshProUGUI>("SkillName").SetText(config.Name, true);
			this._combatSkillRefers[i].CGet<TextMeshProUGUI>("SkillValue").SetText((*(ref this._combatSkillShorts.Items.FixedElementField + (IntPtr)i * 2)).SetValueColor(), true);
		}
	}

	// Token: 0x04001E59 RID: 7769
	private string _charName;

	// Token: 0x04001E5A RID: 7770
	private CombatSkillShorts _combatSkillShorts;

	// Token: 0x04001E5B RID: 7771
	private List<Refers> _combatSkillRefers;
}
