using System;
using FrameWork.UISystem.Components;

namespace UILogic.CharacterMenu.CharacterMenuEquipCombatSkill
{
	// Token: 0x020006B2 RID: 1714
	public class LayoutManager
	{
		// Token: 0x170009C3 RID: 2499
		// (get) Token: 0x06005017 RID: 20503 RVA: 0x002573D4 File Offset: 0x002555D4
		public bool IsTweenToEditingMode
		{
			get
			{
				return this._pageSwitchController.IsAnimatingAToB;
			}
		}

		// Token: 0x170009C4 RID: 2500
		// (get) Token: 0x06005018 RID: 20504 RVA: 0x002573E1 File Offset: 0x002555E1
		public bool IsTweenToNoneEditMode
		{
			get
			{
				return this._pageSwitchController.IsAnimatingAToB;
			}
		}

		// Token: 0x170009C5 RID: 2501
		// (get) Token: 0x06005019 RID: 20505 RVA: 0x002573EE File Offset: 0x002555EE
		public bool IsInTween
		{
			get
			{
				return this._pageSwitchController.IsAnimatingAToB || this._pageSwitchController.IsAnimatingBToA;
			}
		}

		// Token: 0x170009C6 RID: 2502
		// (get) Token: 0x0600501A RID: 20506 RVA: 0x0025740B File Offset: 0x0025560B
		public bool IsEditingMode
		{
			get
			{
				return this._pageSwitchController.IsStateB && !this.IsInTween;
			}
		}

		// Token: 0x0600501B RID: 20507 RVA: 0x00257426 File Offset: 0x00255626
		public LayoutManager(UIHorizontalLayoutSwitcher pageSwitchController)
		{
			this._pageSwitchController = pageSwitchController;
		}

		// Token: 0x0600501C RID: 20508 RVA: 0x00257438 File Offset: 0x00255638
		public void SwitchToEditingMode(bool anim = true, Action onComplete = null, Action<float> onUpdate = null)
		{
			bool flag = this.IsInTween || this.IsEditingMode;
			if (!flag)
			{
				this._pageSwitchController.SwitchToStateB(new bool?(anim), onComplete, onUpdate);
			}
		}

		// Token: 0x0600501D RID: 20509 RVA: 0x00257474 File Offset: 0x00255674
		public void SwitchToNoneEditMode(bool anim = true, Action onComplete = null, Action<float> onUpdate = null)
		{
			bool flag = this.IsInTween || !this.IsEditingMode;
			if (!flag)
			{
				this._pageSwitchController.SwitchToStateA(new bool?(anim), onComplete, onUpdate);
			}
		}

		// Token: 0x0400373E RID: 14142
		private UIHorizontalLayoutSwitcher _pageSwitchController;
	}
}
