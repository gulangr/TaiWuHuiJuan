using System;
using FrameWork;
using FrameWork.UISystem.UIElements;
using UnityEngine;

namespace Game.Views.SectInteract
{
	// Token: 0x020009B3 RID: 2483
	public class QuickSectInteractTutorial : MonoBehaviour
	{
		// Token: 0x06007850 RID: 30800 RVA: 0x0037FBB6 File Offset: 0x0037DDB6
		private void Awake()
		{
			base.GetComponent<CButton>().ClearAndAddListener(new Action(this.OpenSectTutorial));
		}

		// Token: 0x06007851 RID: 30801 RVA: 0x0037FBD1 File Offset: 0x0037DDD1
		private void OpenSectTutorial()
		{
			UIElement.SectInteractTutorial.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("SectTutorialId", this.sectId));
			UIManager.Instance.ShowUI(UIElement.SectInteractTutorial, true);
		}

		// Token: 0x04005AF5 RID: 23285
		public int sectId;
	}
}
