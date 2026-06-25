using System;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips
{
	// Token: 0x02000882 RID: 2178
	public class TooltipCombatSkillBodyStrongItem : MonoBehaviour
	{
		// Token: 0x060068DF RID: 26847 RVA: 0x00303144 File Offset: 0x00301344
		public void Set(string title, string icon1Sprite, string value1, bool showValue1, string icon2Sprite, string value2, bool showValue2, bool isActive = true)
		{
			this.titleText.text = title;
			this.item1Go.SetActive(showValue1);
			if (showValue1)
			{
				bool flag = !string.IsNullOrEmpty(icon1Sprite);
				if (flag)
				{
					this.icon1.SetSprite(icon1Sprite, false, null);
				}
				this.value1Text.text = value1;
			}
			this.item2Go.SetActive(showValue2);
			if (showValue2)
			{
				bool flag2 = !string.IsNullOrEmpty(icon2Sprite);
				if (flag2)
				{
					this.icon2.SetSprite(icon2Sprite, false, null);
				}
				this.value2Text.text = value2;
			}
			this.dividerGo.SetActive(showValue1 && showValue2);
			bool flag3 = !isActive;
			if (flag3)
			{
				this.colorStyleRoot.SetColor(Colors.Instance["grey"], null);
			}
			else
			{
				this.value1Text.color = Colors.Instance["outterinjury"];
				this.value2Text.color = Colors.Instance["innerinjury"];
				this.icon1.SetColor(Color.white);
				this.icon2.SetColor(Color.white);
			}
		}

		// Token: 0x04004AF0 RID: 19184
		[SerializeField]
		private TextMeshProUGUI titleText;

		// Token: 0x04004AF1 RID: 19185
		[SerializeField]
		private CImage icon1;

		// Token: 0x04004AF2 RID: 19186
		[SerializeField]
		private GameObject item1Go;

		// Token: 0x04004AF3 RID: 19187
		[SerializeField]
		private TextMeshProUGUI value1Text;

		// Token: 0x04004AF4 RID: 19188
		[SerializeField]
		private CImage icon2;

		// Token: 0x04004AF5 RID: 19189
		[SerializeField]
		private GameObject item2Go;

		// Token: 0x04004AF6 RID: 19190
		[SerializeField]
		private TextMeshProUGUI value2Text;

		// Token: 0x04004AF7 RID: 19191
		[SerializeField]
		private GameObject dividerGo;

		// Token: 0x04004AF8 RID: 19192
		[SerializeField]
		private ColorStyleRoot colorStyleRoot;
	}
}
