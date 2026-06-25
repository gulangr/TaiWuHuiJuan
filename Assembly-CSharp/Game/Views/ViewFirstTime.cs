using System;
using FrameWork;
using FrameWork.UISystem.UIElements;
using UnityEngine;

namespace Game.Views
{
	// Token: 0x02000707 RID: 1799
	public class ViewFirstTime : UIBase
	{
		// Token: 0x060054F6 RID: 21750 RVA: 0x0027691D File Offset: 0x00274B1D
		public override void OnInit(ArgumentBox argsBox)
		{
			SingletonObject.getInstance<GlobalSettings>().HaveShowFirstTime = true;
		}

		// Token: 0x060054F7 RID: 21751 RVA: 0x0027692C File Offset: 0x00274B2C
		private void Awake()
		{
			this.toggleGroup.Init(-1);
			this.btnConfirm.ClearAndAddListener(new Action(this.OnClickConfirm));
			this.btnCancel.ClearAndAddListener(new Action(this.OnClose));
		}

		// Token: 0x060054F8 RID: 21752 RVA: 0x0027696C File Offset: 0x00274B6C
		private void OnClickConfirm()
		{
			bool flag = this.toggleGroup.GetActiveIndex() == 1;
			if (flag)
			{
				UIElement newFunctionUnlock = UIElement.NewFunctionUnlock;
				newFunctionUnlock.OnHide = (Action)Delegate.Combine(newFunctionUnlock.OnHide, new Action(this.OnClose));
				UIElement.NewFunctionUnlock.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("FunctionUnlockTemplateId", 25));
				UIManager.Instance.MaskUI(UIElement.NewFunctionUnlock);
			}
			else
			{
				UIManager.Instance.ShowUI(UIElement.TutorialChaptersMenu, true);
				this.QuickHide();
			}
		}

		// Token: 0x060054F9 RID: 21753 RVA: 0x002769FC File Offset: 0x00274BFC
		private void OnClose()
		{
			SingletonObject.getInstance<GlobalSettings>().SkipTutorialChapters = true;
			ViewMainMenu viewMainMenu = (ViewMainMenu)UIElement.MainMenu.UiBase;
			if (viewMainMenu != null)
			{
				viewMainMenu.UpdateRoleListButtonInteractable();
			}
			UIManager.Instance.ChangeToUI(UIElement.MainMenu);
			this.QuickHide();
		}

		// Token: 0x040039DA RID: 14810
		[SerializeField]
		private CToggleGroup toggleGroup;

		// Token: 0x040039DB RID: 14811
		[SerializeField]
		private CButton btnConfirm;

		// Token: 0x040039DC RID: 14812
		[SerializeField]
		private CButton btnCancel;
	}
}
