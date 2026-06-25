using System;
using TMPro;
using UnityEngine;

namespace Game.Components.Combat
{
	// Token: 0x02000F06 RID: 3846
	public class CombatDamageDetailResultItem : MonoBehaviour
	{
		// Token: 0x0600B155 RID: 45397 RVA: 0x0050CCA0 File Offset: 0x0050AEA0
		public void Set(string icon, string title, string value)
		{
			bool flag = this.imageIcon != null;
			if (flag)
			{
				this.imageIcon.SetSprite(icon, false, null);
			}
			this.textTitle.text = title;
			this.textValue.text = value;
		}

		// Token: 0x0600B156 RID: 45398 RVA: 0x0050CCE8 File Offset: 0x0050AEE8
		public void Set(Sprite iconSprite, string title, string value)
		{
			bool flag = this.imageIcon;
			if (flag)
			{
				this.imageIcon.sprite = iconSprite;
			}
			this.textTitle.text = title;
			this.textValue.text = value;
		}

		// Token: 0x0600B157 RID: 45399 RVA: 0x0050CD2C File Offset: 0x0050AF2C
		public void Set(string text)
		{
			bool flag = this.imageIcon != null;
			if (flag)
			{
				this.imageIcon.gameObject.SetActive(false);
			}
			bool flag2 = this.textTitle != null;
			if (flag2)
			{
				this.textTitle.gameObject.SetActive(false);
			}
			this.textValue.text = text;
		}

		// Token: 0x0600B158 RID: 45400 RVA: 0x0050CD8C File Offset: 0x0050AF8C
		public void SetValueColor(Color color)
		{
			bool flag = this.textValue != null;
			if (flag)
			{
				this.textValue.color = color;
			}
		}

		// Token: 0x0400896F RID: 35183
		[SerializeField]
		private CImage imageIcon;

		// Token: 0x04008970 RID: 35184
		[SerializeField]
		private TextMeshProUGUI textTitle;

		// Token: 0x04008971 RID: 35185
		[SerializeField]
		private TextMeshProUGUI textValue;
	}
}
