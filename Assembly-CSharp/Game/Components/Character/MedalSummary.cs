using System;
using System.Collections.Generic;
using FrameWork;
using GameData.Domains.Character.Display;
using TMPro;
using UnityEngine;

namespace Game.Components.Character
{
	// Token: 0x02000F37 RID: 3895
	public class MedalSummary : MonoBehaviour
	{
		// Token: 0x0600B321 RID: 45857 RVA: 0x00518DD3 File Offset: 0x00516FD3
		public void Set(CharacterDisplayData displayData)
		{
			this.Set(displayData.AttackMedal, displayData.DefenceMedal, displayData.WisdomMedal);
		}

		// Token: 0x0600B322 RID: 45858 RVA: 0x00518DF0 File Offset: 0x00516FF0
		public void Set(int attackMedal, int defenceMedal, int wisdomMedal)
		{
			bool showDisplay = this.attackMedalIcon != null && this.attackMedalValue != null && this.defenceMedalIcon != null && this.defenceMedalValue != null && this.wisdomMedalIcon != null && this.wisdomMedalValue != null && this.attackMedalTip != null && this.defenceMedalTip != null && this.wisdomMedalTip != null;
			base.gameObject.SetActive(showDisplay);
			bool flag = !showDisplay;
			if (!flag)
			{
				this.SetMedalDisplay(this.attackMedalIcon, this.attackMedalValue, attackMedal, MedalSummary.AttackMedalIconConfig, 0);
				this.SetMedalDisplay(this.defenceMedalIcon, this.defenceMedalValue, defenceMedal, MedalSummary.DefenceMedalIconConfig, 1);
				this.SetMedalDisplay(this.wisdomMedalIcon, this.wisdomMedalValue, wisdomMedal, MedalSummary.WisdomMedalIconConfig, 2);
				this.SetTip(this.attackMedalTip, 0);
				this.SetTip(this.defenceMedalTip, 1);
				this.SetTip(this.wisdomMedalTip, 2);
			}
		}

		// Token: 0x0600B323 RID: 45859 RVA: 0x00518F10 File Offset: 0x00517110
		public void SetValue(int attackCurr, int attackTotal, int defenceCurr, int defenceTotal, int wisdomCurr, int wisdomTotal)
		{
			this.attackMedalValue.text = string.Format("{0}/{1}", attackCurr, Mathf.Abs(attackTotal));
			this.defenceMedalValue.text = string.Format("{0}/{1}", defenceCurr, Mathf.Abs(defenceTotal));
			this.wisdomMedalValue.text = string.Format("{0}/{1}", wisdomCurr, Mathf.Abs(wisdomTotal));
		}

		// Token: 0x0600B324 RID: 45860 RVA: 0x00518F96 File Offset: 0x00517196
		public void SetEmpty()
		{
			base.gameObject.SetActive(false);
		}

		// Token: 0x0600B325 RID: 45861 RVA: 0x00518FA8 File Offset: 0x005171A8
		private void SetMedalDisplay(CImage iconImage, TextMeshProUGUI valueText, int medalValue, Dictionary<int, string> iconConfig, int medalType)
		{
			bool flag = iconImage == null || valueText == null || iconConfig == null;
			if (!flag)
			{
				bool flag2 = medalValue > 0;
				string iconNumber;
				if (flag2)
				{
					iconNumber = iconConfig[1];
				}
				else
				{
					bool flag3 = medalValue < 0;
					if (flag3)
					{
						iconNumber = iconConfig[-1];
					}
					else
					{
						iconNumber = iconConfig[0];
					}
				}
				string iconName = "ui9_icon_strategy_big_" + iconNumber;
				iconImage.SetSprite(iconName, false, null);
				iconImage.SetNativeSize();
				valueText.text = string.Format(" x{0}", Mathf.Abs(medalValue));
			}
		}

		// Token: 0x0600B326 RID: 45862 RVA: 0x00519048 File Offset: 0x00517248
		private void SetTip(TooltipInvoker tip, sbyte medalType)
		{
			tip.Type = TipType.FeatureMedal;
			if (tip.RuntimeParam == null)
			{
				tip.RuntimeParam = new ArgumentBox();
			}
			tip.RuntimeParam.Set("MouseTipMedalType", medalType);
		}

		// Token: 0x04008B23 RID: 35619
		[SerializeField]
		private CImage attackMedalIcon;

		// Token: 0x04008B24 RID: 35620
		[SerializeField]
		private TextMeshProUGUI attackMedalValue;

		// Token: 0x04008B25 RID: 35621
		[SerializeField]
		private CImage defenceMedalIcon;

		// Token: 0x04008B26 RID: 35622
		[SerializeField]
		private TextMeshProUGUI defenceMedalValue;

		// Token: 0x04008B27 RID: 35623
		[SerializeField]
		private CImage wisdomMedalIcon;

		// Token: 0x04008B28 RID: 35624
		[SerializeField]
		private TextMeshProUGUI wisdomMedalValue;

		// Token: 0x04008B29 RID: 35625
		[SerializeField]
		private TooltipInvoker attackMedalTip;

		// Token: 0x04008B2A RID: 35626
		[SerializeField]
		private TooltipInvoker defenceMedalTip;

		// Token: 0x04008B2B RID: 35627
		[SerializeField]
		private TooltipInvoker wisdomMedalTip;

		// Token: 0x04008B2C RID: 35628
		public static readonly Dictionary<int, string> AttackMedalIconConfig = new Dictionary<int, string>
		{
			{
				1,
				"0_2"
			},
			{
				-1,
				"0_1"
			},
			{
				0,
				"0_0"
			}
		};

		// Token: 0x04008B2D RID: 35629
		public static readonly Dictionary<int, string> DefenceMedalIconConfig = new Dictionary<int, string>
		{
			{
				1,
				"1_2"
			},
			{
				-1,
				"1_1"
			},
			{
				0,
				"1_0"
			}
		};

		// Token: 0x04008B2E RID: 35630
		public static readonly Dictionary<int, string> WisdomMedalIconConfig = new Dictionary<int, string>
		{
			{
				1,
				"3_2"
			},
			{
				-1,
				"3_1"
			},
			{
				0,
				"3_0"
			}
		};
	}
}
