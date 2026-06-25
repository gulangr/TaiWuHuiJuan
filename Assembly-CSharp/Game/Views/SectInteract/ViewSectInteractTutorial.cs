using System;
using FrameWork;
using UnityEngine;

namespace Game.Views.SectInteract
{
	// Token: 0x020009B8 RID: 2488
	public class ViewSectInteractTutorial : UIBase
	{
		// Token: 0x06007898 RID: 30872 RVA: 0x00381ABC File Offset: 0x0037FCBC
		public override void OnInit(ArgumentBox argsBox)
		{
			int index;
			argsBox.Get("SectTutorialId", out index);
			for (int i = 0; i < this.sectRoot.Length; i++)
			{
				this.sectRoot[i].SetActive(i == index);
			}
		}

		// Token: 0x06007899 RID: 30873 RVA: 0x00381B04 File Offset: 0x0037FD04
		protected override void OnClick(Transform btn)
		{
			string btnName = btn.name;
			bool flag = "ButtonCloseView" == btnName;
			if (flag)
			{
				this.QuickHide();
			}
		}

		// Token: 0x04005B44 RID: 23364
		[SerializeField]
		private GameObject[] sectRoot;
	}
}
