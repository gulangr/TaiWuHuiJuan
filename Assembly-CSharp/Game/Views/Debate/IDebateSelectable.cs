using System;
using UnityEngine;

namespace Game.Views.Debate
{
	// Token: 0x02000AA6 RID: 2726
	public interface IDebateSelectable
	{
		// Token: 0x060085A4 RID: 34212
		void ShowStrategyTargetMark(bool show, Action<bool> onClick);

		// Token: 0x060085A5 RID: 34213
		void SetSelected(bool isSelected);

		// Token: 0x060085A6 RID: 34214
		Transform GetTransform();
	}
}
