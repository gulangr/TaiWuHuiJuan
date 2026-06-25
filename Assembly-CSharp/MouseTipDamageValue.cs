using System;
using FrameWork;
using Game.Views.Combat;
using TMPro;

// Token: 0x0200028C RID: 652
public class MouseTipDamageValue : MouseTipBase
{
	// Token: 0x060029D1 RID: 10705 RVA: 0x0013D389 File Offset: 0x0013B589
	protected override void Init(ArgumentBox argsBox)
	{
		this.Refresh(argsBox);
	}

	// Token: 0x060029D2 RID: 10706 RVA: 0x0013D394 File Offset: 0x0013B594
	public override void Refresh(ArgumentBox argsBox)
	{
		base.Refresh(argsBox);
		CombatDamageValueLayoutData damageValue;
		argsBox.Get<CombatDamageValueLayoutData>("DamageValue", out damageValue);
		CombatDamageValueLayoutData damageValueOptional;
		argsBox.Get<CombatDamageValueLayoutData>("DamageValueOptional", out damageValueOptional);
		base.CGet<TextMeshProUGUI>("Title").text = CombatConstants.ParseDamageValueTitle(damageValue.MarkKey);
		base.CGet<CombatDamageValueLayout>("DamageValue").Set(damageValue);
		base.CGet<CombatDamageValueLayout>("DamageValueOptional").Set(damageValueOptional);
		base.CGet<CombatHeavyOrBreakLayout>("HeavyOrBreak").Set(damageValue.HeavyOrBreak, (int)damageValue.MarkKey.BodyPart);
	}
}
