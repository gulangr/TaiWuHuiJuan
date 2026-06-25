using System;
using Game.Views.CharacterMenu;
using UnityEngine.Events;

namespace Game.Views.Combat.Item
{
	// Token: 0x02000B3A RID: 2874
	public class CombatItemActionInfo
	{
		// Token: 0x06008EDA RID: 36570 RVA: 0x0042851C File Offset: 0x0042671C
		public CombatItemActionInfo(string name, bool interactable, EItemMenuDisplayOrder displayOrder, Action onClick, UnityAction onEnter = null, UnityAction onExit = null)
		{
			this.Name = name;
			this.Interactable = interactable;
			this.DisplayOrder = displayOrder;
			this.OnClick = onClick;
			this.OnEnter = onEnter;
			this.OnExit = onExit;
			this.TipContent = string.Empty;
		}

		// Token: 0x04006CF7 RID: 27895
		public string Name;

		// Token: 0x04006CF8 RID: 27896
		public bool Interactable;

		// Token: 0x04006CF9 RID: 27897
		public EItemMenuDisplayOrder DisplayOrder;

		// Token: 0x04006CFA RID: 27898
		public Action OnClick;

		// Token: 0x04006CFB RID: 27899
		public string TipContent;

		// Token: 0x04006CFC RID: 27900
		public UnityAction OnEnter;

		// Token: 0x04006CFD RID: 27901
		public UnityAction OnExit;

		// Token: 0x04006CFE RID: 27902
		public short? ThrowDistance;
	}
}
