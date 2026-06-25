using System;
using TMPro;
using UnityEngine;

namespace Game.Views.UsingMedicine
{
	// Token: 0x020007D2 RID: 2002
	public class EatingItemPropertyCell : MonoBehaviour
	{
		// Token: 0x060061CF RID: 25039 RVA: 0x002CE1A0 File Offset: 0x002CC3A0
		public void Set(string iconName, string cellName, string cellValue)
		{
			this.imageIcon.SetSprite(iconName, false, null);
			this.textName.SetText(cellName, true);
			this.textValue.SetText(cellValue, true);
		}

		// Token: 0x040043E3 RID: 17379
		[SerializeField]
		private CImage imageIcon;

		// Token: 0x040043E4 RID: 17380
		[SerializeField]
		private TextMeshProUGUI textName;

		// Token: 0x040043E5 RID: 17381
		[SerializeField]
		private TextMeshProUGUI textValue;
	}
}
