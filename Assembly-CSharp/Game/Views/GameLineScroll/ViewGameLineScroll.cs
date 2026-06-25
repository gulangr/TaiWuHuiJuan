using System;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Common;
using GameData.Domains.TaiwuEvent;
using UnityEngine;

namespace Game.Views.GameLineScroll
{
	// Token: 0x02000A1F RID: 2591
	public class ViewGameLineScroll : UIBase
	{
		// Token: 0x06007F2F RID: 32559 RVA: 0x003B42C4 File Offset: 0x003B24C4
		public override void OnInit(ArgumentBox argsBox)
		{
			argsBox.Get("index", out this._unlockIndex);
			argsBox.Get("levelIndex", out this._levelIndex);
			bool isUnlockScroll;
			argsBox.Get("isUnlockScroll", out isUnlockScroll);
			int targetScrollIndex;
			argsBox.Get("targetScrollIndex", out targetScrollIndex);
			Refers curPage = base.CGet<Refers>((this._levelIndex == 0) ? "FirstLevel" : "SecondLevel");
			ScrollHelper.SetUIElement(this.Element);
			ScrollHelper.OnInit(curPage, this._unlockIndex, true, -1, isUnlockScroll, targetScrollIndex);
			ToggleGroupHotkeyController.Set(this.Element, curPage.CGet<CToggleGroup>("ScrollTypeToggleGroup"), 0, null);
			GameObject clickMask = curPage.CGet<GameObject>("ClickMask");
			clickMask.GetComponent<CButton>().ClearAndAddListener(delegate
			{
				AudioManager.Instance.StopSound("UI_GameLineScroll_Illustration_Loop");
				AudioManager.Instance.PlaySound("ui_default_cancel", false, false);
				base.QuickHide();
				TaiwuEventDomainMethod.Call.TriggerListener("GameLineScrollShowed", true);
			});
		}

		// Token: 0x06007F30 RID: 32560 RVA: 0x003B4388 File Offset: 0x003B2588
		public override void QuickHide()
		{
			bool flag = ScrollHelper.QuickHide();
			if (flag)
			{
				AudioManager.Instance.PlaySound("ui_default_cancel", false, false);
				base.QuickHide();
				TaiwuEventDomainMethod.Call.TriggerListener("GameLineScrollShowed", true);
			}
		}

		// Token: 0x06007F31 RID: 32561 RVA: 0x003B43C8 File Offset: 0x003B25C8
		protected override void OnClick(Transform btn)
		{
			bool flag = btn.name == "Close";
			if (flag)
			{
				this.QuickHide();
			}
			else
			{
				ScrollHelper.OnClick(btn);
			}
		}

		// Token: 0x06007F32 RID: 32562 RVA: 0x003B43FA File Offset: 0x003B25FA
		public void ShowIllustration()
		{
			ScrollHelper.ShowIllustration(false);
		}

		// Token: 0x06007F33 RID: 32563 RVA: 0x003B4404 File Offset: 0x003B2604
		public void HideIllustration()
		{
			ScrollHelper.HideIllustration(true);
		}

		// Token: 0x04006149 RID: 24905
		private int _unlockIndex;

		// Token: 0x0400614A RID: 24906
		private int _levelIndex;
	}
}
