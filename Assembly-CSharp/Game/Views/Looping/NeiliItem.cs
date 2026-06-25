using System;
using System.Collections.Generic;
using Config;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.Looping
{
	// Token: 0x0200097F RID: 2431
	public class NeiliItem : MonoBehaviour
	{
		// Token: 0x060074AF RID: 29871 RVA: 0x00365640 File Offset: 0x00363840
		public void Set(IntList extraNeiliAllocationProgress, byte neiliAllocationType, CombatSkillItem skillConfig, int minDeltaNeiliAllocation, int maxDeltaNeiliAllocation)
		{
			bool flag = extraNeiliAllocationProgress.Items == null;
			if (flag)
			{
				Debug.LogWarning("[NeiliItem] Items is null");
			}
			else
			{
				bool flag2 = (int)neiliAllocationType >= extraNeiliAllocationProgress.Items.Count;
				if (flag2)
				{
					Debug.LogWarning(string.Format("[NeiliItem] Index out of range: {0}", neiliAllocationType));
				}
				else
				{
					int currentProgressValue = extraNeiliAllocationProgress.Items[(int)neiliAllocationType];
					List<int> milestones = LoopingCommonUtils.GenerateNeiliAllocationProgressMinestones(0, currentProgressValue);
					int num;
					if (milestones.Count <= 0)
					{
						num = currentProgressValue;
					}
					else
					{
						int num2 = currentProgressValue;
						List<int> list = milestones;
						num = num2 - list[list.Count - 1];
					}
					int progressInStage = num;
					int extraNeiliAllocation = LoopingCommonUtils.CalcExtraNeiliAllocationFromProgress(currentProgressValue);
					bool flag3 = extraNeiliAllocation >= (int)GlobalConfig.Instance.MaxExtraNeiliAllocation;
					if (flag3)
					{
						progressInStage = 0;
					}
					int progressStageLength = 100 * ((int)GlobalConfig.Instance.ExtraNeiliAllocationFromProgressRatio + extraNeiliAllocation * (int)GlobalConfig.Instance.ExtraNeiliAllocationFromProgressRatioGrowth);
					this.currentValue.text = string.Format("+{0}/{1}", extraNeiliAllocation, GlobalConfig.Instance.MaxExtraNeiliAllocation);
					extraNeiliAllocation = Math.Min(extraNeiliAllocation, (int)GlobalConfig.Instance.MaxExtraNeiliAllocation);
					this.neiliNumbers[0].sprite = this.numbers[extraNeiliAllocation / 10];
					this.neiliNumbers[1].sprite = this.numbers[extraNeiliAllocation % 10];
					this.currentProgress.fillAmount = (float)progressInStage / (float)progressStageLength;
					int basicDelta = (int)((skillConfig == null) ? 0 : skillConfig.ExtraNeiliAllocationProgress[(int)neiliAllocationType]);
					int progressMax = LoopingCommonUtils.GetNeiliAllocationMaxProgress();
					int realMin = Math.Min(basicDelta * 100 + minDeltaNeiliAllocation, progressMax - currentProgressValue);
					int realMax = Math.Min(basicDelta * 100 + maxDeltaNeiliAllocation, progressMax - currentProgressValue);
					string growString = (realMax > realMin) ? string.Format("{0}~{1}", realMin / 100, realMax / 100) : (realMin / 100).ToString();
					string coloredGrowString = (realMax > 0) ? ("<color=#brightblue>" + growString + "</color>+") : "";
					this.progress.text = string.Format("{0}{1}/{2}", coloredGrowString, progressInStage / 100, progressStageLength / 100).ColorReplace();
					float minPreviewFill = (float)(progressInStage + realMin) / (float)progressStageLength;
					float maxPreviewFill = (realMax > realMin) ? ((float)(progressInStage + realMax) / (float)progressStageLength) : minPreviewFill;
					this.maxPreviewProgress.fillAmount = maxPreviewFill;
					this.minPreviewProgress.fillAmount = minPreviewFill;
				}
			}
		}

		// Token: 0x0400573A RID: 22330
		[SerializeField]
		private Sprite[] numbers;

		// Token: 0x0400573B RID: 22331
		[SerializeField]
		private CImage[] neiliNumbers;

		// Token: 0x0400573C RID: 22332
		[SerializeField]
		private TextMeshProUGUI currentValue;

		// Token: 0x0400573D RID: 22333
		[SerializeField]
		private TextMeshProUGUI progress;

		// Token: 0x0400573E RID: 22334
		[SerializeField]
		private CImage currentProgress;

		// Token: 0x0400573F RID: 22335
		[SerializeField]
		private CImage maxPreviewProgress;

		// Token: 0x04005740 RID: 22336
		[SerializeField]
		private CImage minPreviewProgress;
	}
}
