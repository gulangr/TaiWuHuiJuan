using System;
using UnityEngine;

// Token: 0x02000241 RID: 577
public interface ILifeSkillCombatSelectable
{
	// Token: 0x060025A8 RID: 9640
	void ShowStrategyTargetMark(bool show, Action<bool> onClick);

	// Token: 0x060025A9 RID: 9641
	void SetSelected(bool isSelected);

	// Token: 0x060025AA RID: 9642
	Transform GetTransform();
}
