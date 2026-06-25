using System;
using TMPro;
using UnityEngine;

namespace Game.Components.Character.LifeRecord
{
	// Token: 0x02000F5F RID: 3935
	public class RecordTitle : MonoBehaviour
	{
		// Token: 0x0600B410 RID: 46096 RVA: 0x0051E8C4 File Offset: 0x0051CAC4
		public virtual void Set(string text)
		{
			this.date.text = text;
			bool flag = !base.gameObject.activeSelf;
			if (flag)
			{
				base.gameObject.SetActive(true);
			}
		}

		// Token: 0x04008C0C RID: 35852
		[SerializeField]
		protected TMP_Text date;
	}
}
