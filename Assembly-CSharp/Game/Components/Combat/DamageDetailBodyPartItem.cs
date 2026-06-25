using System;
using TMPro;
using UnityEngine;

namespace Game.Components.Combat
{
	// Token: 0x02000F09 RID: 3849
	public class DamageDetailBodyPartItem : MonoBehaviour
	{
		// Token: 0x0600B160 RID: 45408 RVA: 0x0050D021 File Offset: 0x0050B221
		public void Set(string icon, string partName, string probability)
		{
			CImage cimage = this.imageIcon;
			if (cimage != null)
			{
				cimage.SetSprite(icon, false, null);
			}
			this.textPartName.text = partName;
			this.textProbability.text = probability;
		}

		// Token: 0x0600B161 RID: 45409 RVA: 0x0050D054 File Offset: 0x0050B254
		public void Set(Sprite iconSprite, string partName, string probability)
		{
			bool flag = this.imageIcon;
			if (flag)
			{
				this.imageIcon.sprite = iconSprite;
			}
			this.textPartName.text = partName;
			this.textProbability.text = probability;
		}

		// Token: 0x04008977 RID: 35191
		[SerializeField]
		private CImage imageIcon;

		// Token: 0x04008978 RID: 35192
		[SerializeField]
		private TextMeshProUGUI textPartName;

		// Token: 0x04008979 RID: 35193
		[SerializeField]
		private TextMeshProUGUI textProbability;
	}
}
