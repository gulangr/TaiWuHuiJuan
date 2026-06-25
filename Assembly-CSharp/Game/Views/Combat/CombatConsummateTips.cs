using System;
using System.Text;
using FrameWork;
using GameData.Domains.Combat;
using TMPro;
using UnityEngine;

namespace Game.Views.Combat
{
	// Token: 0x02000AF8 RID: 2808
	public class CombatConsummateTips : MonoBehaviour
	{
		// Token: 0x06008A4C RID: 35404 RVA: 0x00400B77 File Offset: 0x003FED77
		private static string GetConsummateIcon(sbyte consummateLevel)
		{
			return CommonUtils.GetConsummateLevelShowDataLegacy(consummateLevel).Item1;
		}

		// Token: 0x06008A4D RID: 35405 RVA: 0x00400B84 File Offset: 0x003FED84
		public void Set(sbyte selfConsummateLevel, sbyte enemyConsummateLevel)
		{
			int reduce = (int)CFormula.CalcConsummateChangeDamagePercent(selfConsummateLevel, enemyConsummateLevel);
			int increase = (int)CFormula.CalcConsummateChangeDamagePercent(enemyConsummateLevel, selfConsummateLevel);
			bool showTips = enemyConsummateLevel > selfConsummateLevel && reduce < 0;
			base.gameObject.SetActive(showTips);
			bool flag = !showTips;
			if (!flag)
			{
				this.selfConsummateLevelIcon.SetSprite(CombatConsummateTips.GetConsummateIcon(selfConsummateLevel), false, null);
				this.selfConsummateLevelText.SetText(selfConsummateLevel.ToString(), true);
				this.enemyConsummateLevelIcon.SetSprite(CombatConsummateTips.GetConsummateIcon(enemyConsummateLevel), false, null);
				this.enemyConsummateLevelText.SetText(enemyConsummateLevel.ToString(), true);
				StringBuilder sb = new StringBuilder();
				sb.Append(LocalStringManager.GetFormat(LanguageKey.LK_Combat_ConsummateLevelTips2, Math.Abs(reduce)));
				bool flag2 = increase != 0;
				if (flag2)
				{
					sb.Append("\n \n");
					sb.Append(LocalStringManager.GetFormat(LanguageKey.LK_Combat_ConsummateLevelTips3, increase));
				}
				TooltipInvoker mouseTip = this.consummateLevelMouseTip;
				TooltipInvoker tooltipInvoker = mouseTip;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = new ArgumentBox();
				}
				mouseTip.RuntimeParam.Set("arg0", LocalStringManager.Get(LanguageKey.LK_Combat_ConsummateLevelTitle));
				mouseTip.RuntimeParam.Set("arg1", sb.ToString());
			}
		}

		// Token: 0x04006A0F RID: 27151
		[SerializeField]
		private CImage selfConsummateLevelIcon;

		// Token: 0x04006A10 RID: 27152
		[SerializeField]
		private TextMeshProUGUI selfConsummateLevelText;

		// Token: 0x04006A11 RID: 27153
		[SerializeField]
		private CImage enemyConsummateLevelIcon;

		// Token: 0x04006A12 RID: 27154
		[SerializeField]
		private TextMeshProUGUI enemyConsummateLevelText;

		// Token: 0x04006A13 RID: 27155
		[SerializeField]
		private TooltipInvoker consummateLevelMouseTip;
	}
}
