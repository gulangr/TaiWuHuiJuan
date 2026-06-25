using System;
using GameData.Domains.Character;
using TMPro;
using UnityEngine;

namespace Game.Views.Combat
{
	// Token: 0x02000B0D RID: 2829
	public class CombatRangeText : MonoBehaviour
	{
		// Token: 0x06008B17 RID: 35607 RVA: 0x00405AE0 File Offset: 0x00403CE0
		public static OuterAndInnerShorts AttackRangeClamp(OuterAndInnerShorts attackRange)
		{
			return new OuterAndInnerShorts
			{
				Outer = Math.Max(attackRange.Outer, 20),
				Inner = Math.Min(attackRange.Inner, 120)
			};
		}

		// Token: 0x06008B18 RID: 35608 RVA: 0x00405B24 File Offset: 0x00403D24
		private static string GetArrowSprite(CombatRangeText.EType type, bool inRange)
		{
			bool flag = !inRange;
			string result;
			if (flag)
			{
				result = "combat_jiaodi_juli_1_3";
			}
			else
			{
				if (!true)
				{
				}
				string text;
				switch (type)
				{
				case CombatRangeText.EType.Self:
					text = "combat_jiaodi_juli_1_1";
					break;
				case CombatRangeText.EType.Enemy:
					text = "combat_jiaodi_juli_1_0";
					break;
				case CombatRangeText.EType.Preview:
					text = "combat_jiaodi_juli_1_2";
					break;
				default:
					throw new ArgumentOutOfRangeException("type", type, null);
				}
				if (!true)
				{
				}
				result = text;
			}
			return result;
		}

		// Token: 0x06008B19 RID: 35609 RVA: 0x00405B90 File Offset: 0x00403D90
		private static string GetLineSprite(CombatRangeText.EType type, bool inRange)
		{
			bool flag = !inRange;
			string result;
			if (flag)
			{
				result = "combat_jiaodi_juli_0_3";
			}
			else
			{
				if (!true)
				{
				}
				string text;
				switch (type)
				{
				case CombatRangeText.EType.Self:
					text = "combat_jiaodi_juli_0_1";
					break;
				case CombatRangeText.EType.Enemy:
					text = "combat_jiaodi_juli_0_0";
					break;
				case CombatRangeText.EType.Preview:
					text = "combat_jiaodi_juli_0_2";
					break;
				default:
					throw new ArgumentOutOfRangeException("type", type, null);
				}
				if (!true)
				{
				}
				result = text;
			}
			return result;
		}

		// Token: 0x06008B1A RID: 35610 RVA: 0x00405BFC File Offset: 0x00403DFC
		public void Refresh(CombatRangeText.EType type, OuterAndInnerShorts attackRange, bool inRange)
		{
			string arrowSprite = CombatRangeText.GetArrowSprite(type, inRange);
			string lineSprite = CombatRangeText.GetLineSprite(type, inRange);
			this.arrowLeft.SetSprite(arrowSprite, false, null);
			this.arrowRight.SetSprite(arrowSprite, false, null);
			this.connectLine.SetSprite(lineSprite, false, null);
			short min = attackRange.Outer;
			short max = attackRange.Inner;
			string minStr = (min < 20) ? LocalStringManager.Get(LanguageKey.LK_Infinity) : ((float)min / 10f).ToString("F1");
			string maxStr = (max > 120) ? LocalStringManager.Get(LanguageKey.LK_Infinity) : ((float)max / 10f).ToString("F1");
			this.textLeft.text = ((type == CombatRangeText.EType.Enemy) ? maxStr : minStr);
			this.textRight.text = ((type == CombatRangeText.EType.Enemy) ? minStr : maxStr);
		}

		// Token: 0x04006AB4 RID: 27316
		[SerializeField]
		private CImage connectLine;

		// Token: 0x04006AB5 RID: 27317
		[SerializeField]
		private CImage arrowLeft;

		// Token: 0x04006AB6 RID: 27318
		[SerializeField]
		private CImage arrowRight;

		// Token: 0x04006AB7 RID: 27319
		[SerializeField]
		private TextMeshProUGUI textLeft;

		// Token: 0x04006AB8 RID: 27320
		[SerializeField]
		private TextMeshProUGUI textRight;

		// Token: 0x020020D0 RID: 8400
		public enum EType
		{
			// Token: 0x0400D252 RID: 53842
			Self,
			// Token: 0x0400D253 RID: 53843
			Enemy,
			// Token: 0x0400D254 RID: 53844
			Preview
		}
	}
}
