using System;
using TMPro;
using UnityEngine;

namespace Game.Views.SectInteract.Zhujian
{
	// Token: 0x020009C3 RID: 2499
	public class GearMateLifeSkillBookChapter : MonoBehaviour
	{
		// Token: 0x0600793D RID: 31037 RVA: 0x00385E38 File Offset: 0x00384038
		public void Refresh(int index, int last, int cur)
		{
			this.textName.text = LocalStringManager.Get(string.Format("LK_Book_Page_Index_{0}", index)).SetColor("grey");
			this.curProgress.text = string.Format("{0}%", cur).SetColor("FiveElementType_Xuanyin");
			this.lastProgress.text = string.Format("{0}%", last).SetColor("PersonalityType_Calm");
		}

		// Token: 0x04005BEA RID: 23530
		[SerializeField]
		private TextMeshProUGUI textName;

		// Token: 0x04005BEB RID: 23531
		[SerializeField]
		private TextMeshProUGUI lastProgress;

		// Token: 0x04005BEC RID: 23532
		[SerializeField]
		private TextMeshProUGUI curProgress;
	}
}
