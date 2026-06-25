using System;
using FrameWork;
using GameData.Domains.Combat;
using UICommon.Character;

// Token: 0x02000282 RID: 642
public class MouseTipCombatInjuryChange : MouseTipBase
{
	// Token: 0x06002957 RID: 10583 RVA: 0x00134E20 File Offset: 0x00133020
	public override bool CanShowWithArgumentBox(ArgumentBox argumentBox)
	{
		CombatResultDisplayData combatResultDisplayData;
		return argumentBox.Get<CombatResultDisplayData>("DisplayData", out combatResultDisplayData);
	}

	// Token: 0x06002958 RID: 10584 RVA: 0x00134E3F File Offset: 0x0013303F
	protected override void Init(ArgumentBox argsBox)
	{
		this.Refresh(argsBox);
	}

	// Token: 0x06002959 RID: 10585 RVA: 0x00134E4C File Offset: 0x0013304C
	public override void Refresh(ArgumentBox argsBox)
	{
		argsBox.Get<CombatResultDisplayData>("DisplayData", out this._displayData);
		base.CGet<CharacterTabInjury>("Before").Refresh(this._displayData, false);
		base.CGet<CharacterTabInjury>("After").Refresh(this._displayData, true);
	}

	// Token: 0x04001E05 RID: 7685
	private CombatResultDisplayData _displayData;
}
