using System;
using FrameWork.UISystem.UIElements;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.Building.BuildingAreaQuickActionMenu
{
	// Token: 0x02000C25 RID: 3109
	public abstract class LeftButtonProcessor
	{
		// Token: 0x170010B0 RID: 4272
		// (get) Token: 0x06009DBE RID: 40382 RVA: 0x0049DE70 File Offset: 0x0049C070
		// (set) Token: 0x06009DBF RID: 40383 RVA: 0x0049DE78 File Offset: 0x0049C078
		public GameObject ButtonObject { get; protected set; }

		// Token: 0x06009DC0 RID: 40384 RVA: 0x0049DE84 File Offset: 0x0049C084
		protected LeftButtonProcessor(ViewBuildingQuickActionMenu menu, GameObject buttonObject, LeftButtonType type)
		{
			this._menu = menu;
			this.ButtonObject = buttonObject;
			this._leftButtonType = type;
			this._button = buttonObject.GetComponent<CButton>();
			this._button.ClearAndAddListener(new Action(this.OnClick));
			this._tip = buttonObject.GetComponent<TooltipInvoker>();
			this._tip.Type = TipType.Simple;
			this._pointerTrigger = buttonObject.GetOrAddComponent<PointerTrigger>();
			PointerTrigger pointerTrigger = this._pointerTrigger;
			if (pointerTrigger.EnterEvent == null)
			{
				pointerTrigger.EnterEvent = new UnityEvent();
			}
			this._pointerTrigger.EnterEvent.RemoveAllListeners();
			this._pointerTrigger.EnterEvent.AddListener(new UnityAction(this.OnHoverEnter));
			pointerTrigger = this._pointerTrigger;
			if (pointerTrigger.ExitEvent == null)
			{
				pointerTrigger.ExitEvent = new UnityEvent();
			}
			this._pointerTrigger.ExitEvent.RemoveAllListeners();
			this._pointerTrigger.ExitEvent.AddListener(new UnityAction(this.OnHoverExit));
			string[] spriteNames = ViewBuildingQuickActionMenu.GetTabSpriteNames(type);
			ViewBuildingQuickActionMenu.SetButtonSprites(this._button, spriteNames, true);
		}

		// Token: 0x06009DC1 RID: 40385
		public abstract void PrepareData();

		// Token: 0x06009DC2 RID: 40386 RVA: 0x0049DF9F File Offset: 0x0049C19F
		public void UpdateVisibility()
		{
			this.ButtonObject.SetActive(this.IsVisible());
			LeftLayoutManager leftLayoutManager = this._menu.LeftLayoutManager;
			if (leftLayoutManager != null)
			{
				leftLayoutManager.RequestLayoutUpdate();
			}
		}

		// Token: 0x06009DC3 RID: 40387 RVA: 0x0049DFCB File Offset: 0x0049C1CB
		public void UpdateInteractivity()
		{
			this._button.interactable = this.CanInteract();
		}

		// Token: 0x06009DC4 RID: 40388
		public abstract bool IsVisible();

		// Token: 0x06009DC5 RID: 40389
		public abstract bool CanInteract();

		// Token: 0x06009DC6 RID: 40390 RVA: 0x0049DFE0 File Offset: 0x0049C1E0
		public virtual void OnClick()
		{
		}

		// Token: 0x06009DC7 RID: 40391 RVA: 0x0049DFE3 File Offset: 0x0049C1E3
		public virtual void OnHoverEnter()
		{
		}

		// Token: 0x06009DC8 RID: 40392 RVA: 0x0049DFE6 File Offset: 0x0049C1E6
		public virtual void OnHoverExit()
		{
		}

		// Token: 0x04007A40 RID: 31296
		protected ViewBuildingQuickActionMenu _menu;

		// Token: 0x04007A41 RID: 31297
		protected LeftButtonType _leftButtonType;

		// Token: 0x04007A42 RID: 31298
		protected CButton _button;

		// Token: 0x04007A43 RID: 31299
		protected TooltipInvoker _tip;

		// Token: 0x04007A44 RID: 31300
		protected PointerTrigger _pointerTrigger;
	}
}
