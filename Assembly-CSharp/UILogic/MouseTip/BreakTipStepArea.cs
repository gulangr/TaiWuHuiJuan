using System;
using TMPro;
using UnityEngine;

namespace UILogic.MouseTip
{
	// Token: 0x020006B0 RID: 1712
	public class BreakTipStepArea : MonoBehaviour
	{
		// Token: 0x06005005 RID: 20485 RVA: 0x00256A7C File Offset: 0x00254C7C
		public void RefreshStep(int leftNormalStep, int leftMadStep, int costStep, bool isNormalStepExhausted)
		{
			bool normalStepEnough = leftNormalStep >= costStep;
			bool madStepEnough = leftMadStep >= costStep;
			this.normalStepLine.GetComponent<DisableStyleRoot>().SetStyleEffect(isNormalStepExhausted, false);
			this.goneMadStepLine.GetComponent<DisableStyleRoot>().SetStyleEffect(!isNormalStepExhausted, false);
			this.normalStep.text = LocalStringManager.GetFormat(LanguageKey.LK_Skill_Break_Cell_Tip_NormalStep_Line, leftNormalStep.ToString().SetColor(normalStepEnough ? "brightblue" : "brightred"), costStep).ColorReplace();
			this.goneMadStep.text = LocalStringManager.GetFormat(LanguageKey.LK_Skill_Break_Cell_Tip_GoneMadStep_Line, leftMadStep.ToString().SetColor(madStepEnough ? "brightblue" : "brightred"), costStep).ColorReplace();
		}

		// Token: 0x06005006 RID: 20486 RVA: 0x00256B40 File Offset: 0x00254D40
		public void RefreshExp(int currentExp, int needExp)
		{
			bool expEnough = currentExp >= needExp;
			string currentExpStr = currentExp.ToString().SetColor(expEnough ? "brightblue" : "brightred");
			this.expCheck.text = LocalStringManager.GetFormat(LanguageKey.LK_Skill_Break_Bonus_Cell_ExpCost, currentExpStr, needExp).ColorReplace();
		}

		// Token: 0x04003723 RID: 14115
		[SerializeField]
		private GameObject normalStepLine;

		// Token: 0x04003724 RID: 14116
		[SerializeField]
		private GameObject goneMadStepLine;

		// Token: 0x04003725 RID: 14117
		[SerializeField]
		private TextMeshProUGUI normalStep;

		// Token: 0x04003726 RID: 14118
		[SerializeField]
		private TextMeshProUGUI goneMadStep;

		// Token: 0x04003727 RID: 14119
		[SerializeField]
		private TextMeshProUGUI expCheck;
	}
}
