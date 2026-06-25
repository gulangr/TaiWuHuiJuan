using System;
using FrameWork.UISystem.UIElements;
using UnityEngine;

namespace Game.Views.VillagerRoleView
{
	// Token: 0x02000736 RID: 1846
	public class SlideBookIndexerComponent : MonoBehaviour
	{
		// Token: 0x060058B3 RID: 22707 RVA: 0x0029287D File Offset: 0x00290A7D
		public void SetInteractable(bool isOpen)
		{
			this._unlocked = isOpen;
			this.RefreshInteractable();
		}

		// Token: 0x060058B4 RID: 22708 RVA: 0x0029288E File Offset: 0x00290A8E
		public void SetEnableIndexerInteract(bool enableInteract)
		{
			this._interactable = enableInteract;
			this.RefreshInteractable();
		}

		// Token: 0x060058B5 RID: 22709 RVA: 0x002928A0 File Offset: 0x00290AA0
		private void RefreshInteractable()
		{
			this.PageIndexBtn.interactable = (this._interactable && this._unlocked);
			this.Normal.SetActive(this._unlocked);
			this.Disable.SetActive(!this._unlocked);
			bool flag = !this._interactable;
			if (flag)
			{
				this.HoverObj.SetActive(false);
			}
		}

		// Token: 0x060058B6 RID: 22710 RVA: 0x0029290E File Offset: 0x00290B0E
		public void SetButtonAction(Action actionOnClick)
		{
			this.PageIndexBtn.ClearAndAddListener(actionOnClick);
		}

		// Token: 0x04003D1F RID: 15647
		public CButton PageIndexBtn;

		// Token: 0x04003D20 RID: 15648
		public PointerTrigger PageIndexDescTrigger;

		// Token: 0x04003D21 RID: 15649
		public GameObject HoverObj;

		// Token: 0x04003D22 RID: 15650
		public GameObject Normal;

		// Token: 0x04003D23 RID: 15651
		public GameObject Disable;

		// Token: 0x04003D24 RID: 15652
		private bool _unlocked = true;

		// Token: 0x04003D25 RID: 15653
		private bool _interactable = true;
	}
}
