using System;
using FrameWork;
using FrameWork.UISystem.UIElements;
using UnityEngine;

namespace Game.Views.SectInteract
{
	// Token: 0x0200099D RID: 2461
	public class ViewTangramDialog : UIBase
	{
		// Token: 0x06007670 RID: 30320 RVA: 0x00373628 File Offset: 0x00371828
		private void Awake()
		{
			this.ToggleGroup.OnActiveIndexChange += this.ToggleChange;
			this.btnConfirm.ClearAndAddListener(new Action(this.OnConfirm));
			this.btnCancel.ClearAndAddListener(new Action(this.OnCancel));
			this.ToggleGroup.Init(0);
		}

		// Token: 0x06007671 RID: 30321 RVA: 0x0037368B File Offset: 0x0037188B
		private void OnCancel()
		{
			Action no = this._cmd.No;
			if (no != null)
			{
				no();
			}
			this.Element.ShowAfterRefresh();
			this.QuickHide();
		}

		// Token: 0x06007672 RID: 30322 RVA: 0x003736B8 File Offset: 0x003718B8
		private void OnConfirm()
		{
			Action<int> yes = this._cmd.Yes;
			if (yes != null)
			{
				yes(this._currentSelectKey);
			}
			this.QuickHide();
		}

		// Token: 0x06007673 RID: 30323 RVA: 0x003736DF File Offset: 0x003718DF
		public override void OnInit(ArgumentBox argsBox)
		{
			if (argsBox != null)
			{
				argsBox.Get<TangramDialogCmd>("Cmd", out this._cmd);
			}
		}

		// Token: 0x06007674 RID: 30324 RVA: 0x003736F9 File Offset: 0x003718F9
		private void ToggleChange(int newToggle, int oldToggle)
		{
			this._currentSelectKey = newToggle;
		}

		// Token: 0x04005958 RID: 22872
		private TangramDialogCmd _cmd;

		// Token: 0x04005959 RID: 22873
		private int _currentSelectKey;

		// Token: 0x0400595A RID: 22874
		[SerializeField]
		private CButton btnConfirm;

		// Token: 0x0400595B RID: 22875
		[SerializeField]
		private CButton btnCancel;

		// Token: 0x0400595C RID: 22876
		public CToggleGroup ToggleGroup;
	}
}
