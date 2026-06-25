using System;
using UnityEngine;

namespace Game.Views.MouseTips
{
	// Token: 0x02000865 RID: 2149
	public class MouseTipFuyuSwordGrip : MonoBehaviour
	{
		// Token: 0x060067C3 RID: 26563 RVA: 0x002F6420 File Offset: 0x002F4620
		public void Set(bool showStoneHouse)
		{
			bool flag = this.savePeopleArea != null;
			if (flag)
			{
				this.savePeopleArea.SetActive(showStoneHouse);
			}
		}

		// Token: 0x0400494C RID: 18764
		[SerializeField]
		private GameObject savePeopleArea;
	}
}
