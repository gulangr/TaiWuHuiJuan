using System;
using TMPro;
using UnityEngine;

namespace Game.Views.Villager
{
	// Token: 0x02000746 RID: 1862
	public class VillagerWorkProperty : MonoBehaviour
	{
		// Token: 0x06005A37 RID: 23095 RVA: 0x0029DED0 File Offset: 0x0029C0D0
		public void Set(string titleText, string valueText, bool showDelim)
		{
			this.title.text = titleText;
			this.value.text = valueText;
			this.delim.gameObject.SetActive(showDelim);
		}

		// Token: 0x04003E23 RID: 15907
		[SerializeField]
		private TMP_Text title;

		// Token: 0x04003E24 RID: 15908
		[SerializeField]
		private TMP_Text value;

		// Token: 0x04003E25 RID: 15909
		[SerializeField]
		private CImage delim;
	}
}
