using System;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips
{
	// Token: 0x02000863 RID: 2147
	public class MouseTipFuyuPropertyItem : MonoBehaviour
	{
		// Token: 0x060067BE RID: 26558 RVA: 0x002F6220 File Offset: 0x002F4420
		public void Set(string iconName, string title, int addValue, int addPercent, bool showImmune = false)
		{
			bool flag = this.icon != null;
			if (flag)
			{
				this.icon.SetSprite(iconName, false, null);
			}
			bool flag2 = this.titleText != null;
			if (flag2)
			{
				this.titleText.text = title;
			}
			bool flag3 = this.valueText != null;
			if (flag3)
			{
				if (showImmune)
				{
					this.valueText.alignment = TextAlignmentOptions.Center;
					this.valueText.text = LocalStringManager.Get(LanguageKey.LK_PoisonImmuneSinceBorn).SetColor("grey");
				}
				else
				{
					this.valueText.alignment = TextAlignmentOptions.Right;
					this.valueText.text = MouseTipFuyuPropertyItem.FormatBonusValue(addValue, addPercent);
				}
			}
		}

		// Token: 0x060067BF RID: 26559 RVA: 0x002F62E0 File Offset: 0x002F44E0
		private static string FormatBonusValue(int addValue, int addPercent)
		{
			bool flag = addValue != 0;
			string result;
			if (flag)
			{
				bool flag2 = addValue > 0;
				if (flag2)
				{
					result = "+" + addValue.ToString();
				}
				else
				{
					result = addValue.ToString().SetColor("brightred");
				}
			}
			else
			{
				bool flag3 = addPercent != 0;
				if (flag3)
				{
					bool flag4 = addPercent > 0;
					if (flag4)
					{
						result = "+" + addPercent.ToString() + "%";
					}
					else
					{
						result = (addPercent.ToString() + "%").SetColor("brightred");
					}
				}
				else
				{
					result = "";
				}
			}
			return result;
		}

		// Token: 0x04004949 RID: 18761
		[SerializeField]
		private CImage icon;

		// Token: 0x0400494A RID: 18762
		[SerializeField]
		private TextMeshProUGUI titleText;

		// Token: 0x0400494B RID: 18763
		[SerializeField]
		private TextMeshProUGUI valueText;
	}
}
