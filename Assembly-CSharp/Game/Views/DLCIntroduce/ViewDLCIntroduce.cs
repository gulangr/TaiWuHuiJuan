using System;
using FrameWork;
using UnityEngine;

namespace Game.Views.DLCIntroduce
{
	// Token: 0x02000A98 RID: 2712
	public class ViewDLCIntroduce : UIBase
	{
		// Token: 0x060084AC RID: 33964 RVA: 0x003DB41C File Offset: 0x003D961C
		public override void OnInit(ArgumentBox argsBox)
		{
			int selectedIndex;
			argsBox.Get("SelectedIndex", out selectedIndex);
			this.helper.Set(selectedIndex, false);
			this.Element.ShowAfterRefresh();
		}

		// Token: 0x060084AD RID: 33965 RVA: 0x003DB454 File Offset: 0x003D9654
		protected override void OnClick(Transform btn)
		{
			string name = btn.name;
			string text = name;
			string a = text;
			if (a == "ButtonCloseView")
			{
				this.QuickHide();
			}
		}

		// Token: 0x040065BD RID: 26045
		[SerializeField]
		private DLCIntroduceHelper helper;
	}
}
