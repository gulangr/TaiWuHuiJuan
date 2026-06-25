using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Game.Views.MonthNotify
{
	// Token: 0x020008BD RID: 2237
	public class MonthNotifyNeiliAllocationType : MonoBehaviour
	{
		// Token: 0x06006AA9 RID: 27305 RVA: 0x00313E9C File Offset: 0x0031209C
		public void Set(int prev, int curr)
		{
			int delta = (curr - prev) / 100;
			if (!true)
			{
			}
			string text;
			if (delta <= 0)
			{
				if (delta >= 0)
				{
					text = "";
				}
				else
				{
					text = string.Format("({0})", delta).SetColor("brightred");
				}
			}
			else
			{
				text = string.Format("(+{0})", delta).SetColor("brightblue");
			}
			if (!true)
			{
			}
			string deltaString = text;
			List<int> milestone = LoopingCommonUtils.GenerateNeiliAllocationProgressMinestones(0, curr);
			int num;
			if (milestone.Count <= 0)
			{
				num = curr;
			}
			else
			{
				List<int> list = milestone;
				num = curr - list[list.Count - 1];
			}
			int progressInStage = num;
			int extraNeiliAllocation = LoopingCommonUtils.CalcExtraNeiliAllocationFromProgress(curr);
			bool flag = extraNeiliAllocation >= (int)GlobalConfig.Instance.MaxExtraNeiliAllocation;
			if (flag)
			{
				progressInStage = 0;
			}
			int progressStageLength = 100 * ((int)GlobalConfig.Instance.ExtraNeiliAllocationFromProgressRatio + extraNeiliAllocation * (int)GlobalConfig.Instance.ExtraNeiliAllocationFromProgressRatioGrowth);
			this.valueLabel.text = string.Format("+{0}/{1}", extraNeiliAllocation, GlobalConfig.Instance.MaxExtraNeiliAllocation);
			this.progress.fillAmount = (float)progressInStage / (float)progressStageLength;
			this.progressLabel.text = string.Format("{0}/{1}{2}", progressInStage / 100, progressStageLength / 100, deltaString);
		}

		// Token: 0x04004D13 RID: 19731
		[SerializeField]
		protected TextMeshProUGUI valueLabel;

		// Token: 0x04004D14 RID: 19732
		[SerializeField]
		protected TextMeshProUGUI progressLabel;

		// Token: 0x04004D15 RID: 19733
		[SerializeField]
		protected CImage progress;
	}
}
