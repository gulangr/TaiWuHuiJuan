using System;
using FrameWork.UISystem.UIElements;
using UnityEngine;

namespace Game.Views.NewGame
{
	// Token: 0x02000818 RID: 2072
	public class NewGameToggleHelper : MonoBehaviour
	{
		// Token: 0x060065A7 RID: 26023 RVA: 0x002E6FD0 File Offset: 0x002E51D0
		private void Awake()
		{
			bool flag = this.toggle == null;
			if (flag)
			{
				this.toggle = base.GetComponent<CToggle>();
			}
		}

		// Token: 0x060065A8 RID: 26024 RVA: 0x002E6FFC File Offset: 0x002E51FC
		public void SetLocked(bool locked)
		{
			bool flag = this.toggle != null;
			if (flag)
			{
				this.toggle.interactable = !locked;
			}
			bool flag2 = this.lockOverlay != null;
			if (flag2)
			{
				this.lockOverlay.SetActive(locked);
			}
		}

		// Token: 0x17000C44 RID: 3140
		// (get) Token: 0x060065A9 RID: 26025 RVA: 0x002E7047 File Offset: 0x002E5247
		public bool IsLocked
		{
			get
			{
				return this.toggle != null && !this.toggle.interactable;
			}
		}

		// Token: 0x040046E1 RID: 18145
		[SerializeField]
		private CToggle toggle;

		// Token: 0x040046E2 RID: 18146
		[SerializeField]
		private GameObject lockOverlay;
	}
}
