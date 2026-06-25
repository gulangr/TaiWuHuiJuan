using System;
using FrameWork.UISystem.UIElements;
using UnityEngine;

namespace Game.Views.NewGame
{
	// Token: 0x02000816 RID: 2070
	public class NewGameSubPageWorldDetailTabItem : MonoBehaviour
	{
		// Token: 0x060065A3 RID: 26019 RVA: 0x002E6F40 File Offset: 0x002E5140
		public void SetInteractable(bool interactable)
		{
			this.toggle.interactable = interactable;
			bool flag = this.lockedObj;
			if (flag)
			{
				this.lockedObj.gameObject.SetActive(!interactable);
			}
		}

		// Token: 0x040046DE RID: 18142
		[SerializeField]
		private CToggle toggle;

		// Token: 0x040046DF RID: 18143
		[SerializeField]
		private GameObject lockedObj;
	}
}
