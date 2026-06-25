using System;
using TMPro;
using UnityEngine;

namespace Game.Views.DLCIntroduce
{
	// Token: 0x02000A94 RID: 2708
	public class DlCIntroduceDescItem : MonoBehaviour
	{
		// Token: 0x0600848C RID: 33932 RVA: 0x003DA5B0 File Offset: 0x003D87B0
		public void Set(string title, string content)
		{
			this.title.text = title;
			this.content.text = content;
		}

		// Token: 0x0400659E RID: 26014
		[SerializeField]
		private TextMeshProUGUI title;

		// Token: 0x0400659F RID: 26015
		[SerializeField]
		private TextMeshProUGUI content;
	}
}
