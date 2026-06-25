using System;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips
{
	// Token: 0x02000834 RID: 2100
	public class LegacyItemWorldPercent : MonoBehaviour
	{
		// Token: 0x0600669F RID: 26271 RVA: 0x002EC9FB File Offset: 0x002EABFB
		public void Set(int percentage, string iconName)
		{
			this.pointValue.text = string.Format("+{0}%", percentage);
			this.icon.SetSprite(iconName, false, null);
		}

		// Token: 0x040047D7 RID: 18391
		[SerializeField]
		private TMP_Text pointValue;

		// Token: 0x040047D8 RID: 18392
		[SerializeField]
		private CImage icon;
	}
}
