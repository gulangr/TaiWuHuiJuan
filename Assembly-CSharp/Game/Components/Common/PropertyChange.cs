using System;
using TMPro;
using UnityEngine;

namespace Game.Components.Common
{
	// Token: 0x02000F95 RID: 3989
	public class PropertyChange : MonoBehaviour
	{
		// Token: 0x0600B772 RID: 46962 RVA: 0x005398E0 File Offset: 0x00537AE0
		public void Set(string icon, string title, string iconCurrent, string valueCurrent, string iconPreview, string valuePreview)
		{
			this.imageIcon.SetSprite(icon, false, null);
			this.textTitle.SetText(title, true);
			this.imageIconCurrent.SetSprite(iconCurrent, false, null);
			this.imageIconCurrent.gameObject.SetActive(!iconCurrent.IsNullOrEmpty());
			this.textValueCurrent.SetText(valueCurrent, true);
			this.imageIconPreview.SetSprite(iconPreview, false, null);
			this.imageIconPreview.gameObject.SetActive(!iconPreview.IsNullOrEmpty());
			this.textValuePreview.SetText(valuePreview, true);
		}

		// Token: 0x04008E72 RID: 36466
		[SerializeField]
		private CImage imageIcon;

		// Token: 0x04008E73 RID: 36467
		[SerializeField]
		private TextMeshProUGUI textTitle;

		// Token: 0x04008E74 RID: 36468
		[SerializeField]
		private CImage imageIconCurrent;

		// Token: 0x04008E75 RID: 36469
		[SerializeField]
		private TextMeshProUGUI textValueCurrent;

		// Token: 0x04008E76 RID: 36470
		[SerializeField]
		private CImage imageIconPreview;

		// Token: 0x04008E77 RID: 36471
		[SerializeField]
		private TextMeshProUGUI textValuePreview;
	}
}
