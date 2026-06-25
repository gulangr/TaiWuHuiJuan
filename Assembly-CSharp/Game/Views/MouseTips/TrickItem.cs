using System;
using Config;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips
{
	// Token: 0x02000892 RID: 2194
	public class TrickItem : MonoBehaviour
	{
		// Token: 0x06006938 RID: 26936 RVA: 0x00305610 File Offset: 0x00303810
		public void Set(int hit, int hitSum, int index, bool isMax)
		{
			this.style.SetStyleEffect(hit == 0, false);
			this.partName.text = BodyPart.Instance[index].Name;
			this.percentage.text = (isMax ? string.Format("{0:f01}%", (float)hit * 100f / (float)hitSum).SetColor("orange") : string.Format("{0:f01}%", (float)hit * 100f / (float)hitSum));
		}

		// Token: 0x04004B66 RID: 19302
		[SerializeField]
		private CImage icon;

		// Token: 0x04004B67 RID: 19303
		[SerializeField]
		private DisableStyleRoot style;

		// Token: 0x04004B68 RID: 19304
		[SerializeField]
		private TMP_Text partName;

		// Token: 0x04004B69 RID: 19305
		[SerializeField]
		private TMP_Text percentage;
	}
}
