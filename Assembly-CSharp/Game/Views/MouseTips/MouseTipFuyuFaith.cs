using System;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips
{
	// Token: 0x0200085D RID: 2141
	public class MouseTipFuyuFaith : MonoBehaviour
	{
		// Token: 0x060067AE RID: 26542 RVA: 0x002F5EB0 File Offset: 0x002F40B0
		public void Set(int fuyuFaith, bool showFaith)
		{
			base.gameObject.SetActive(showFaith);
			bool flag = !showFaith;
			if (!flag)
			{
				bool flag2 = this.faithValueText != null;
				if (flag2)
				{
					this.faithValueText.text = fuyuFaith.ToString();
				}
			}
		}

		// Token: 0x04004945 RID: 18757
		[SerializeField]
		private TextMeshProUGUI faithValueText;
	}
}
