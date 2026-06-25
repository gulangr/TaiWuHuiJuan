using System;
using System.Collections.Generic;
using Config;

// Token: 0x02000250 RID: 592
public static class LoopingCommonUtils
{
	// Token: 0x06002739 RID: 10041 RVA: 0x00121544 File Offset: 0x0011F744
	public static int CalcLoopingEventRate(CombatSkillItem skillConfig, List<short> referenceSkillList)
	{
		int rate = (int)GlobalConfig.Instance.BaseLoopingEventProbability;
		int possibleQiArtStrategyCount = 0;
		foreach (short referenceSkillId in referenceSkillList)
		{
			bool flag = referenceSkillId == -1;
			if (!flag)
			{
				CombatSkillItem referenceSkillConfig = CombatSkill.Instance[referenceSkillId];
				rate += (int)referenceSkillConfig.QiArtStrategyGenerateProbability;
				possibleQiArtStrategyCount += referenceSkillConfig.PossibleQiArtStrategyList.Count;
			}
		}
		return (possibleQiArtStrategyCount > 0) ? rate : 0;
	}

	// Token: 0x0600273A RID: 10042 RVA: 0x001215E0 File Offset: 0x0011F7E0
	public static int CalcExtraNeiliAllocationFromProgress(int extraNeiliAllocationProgress)
	{
		int basic = (int)(GlobalConfig.Instance.ExtraNeiliAllocationFromProgressRatio * 100);
		int delta = (int)(GlobalConfig.Instance.ExtraNeiliAllocationFromProgressRatioGrowth * 100);
		int currentStageLength = basic;
		int result = 0;
		for (;;)
		{
			extraNeiliAllocationProgress -= currentStageLength;
			bool flag = extraNeiliAllocationProgress >= 0;
			if (!flag)
			{
				break;
			}
			result++;
			currentStageLength += delta;
		}
		return result;
	}

	// Token: 0x0600273B RID: 10043 RVA: 0x00121640 File Offset: 0x0011F840
	public static int CalcExtraNeiliAllocationFromProgress(int extraNeiliAllocationProgress, int basic, int delta)
	{
		int currentStageLength = basic;
		int result = 0;
		for (;;)
		{
			extraNeiliAllocationProgress -= currentStageLength;
			bool flag = extraNeiliAllocationProgress >= 0;
			if (!flag)
			{
				break;
			}
			result++;
			currentStageLength += delta;
		}
		return result;
	}

	// Token: 0x0600273C RID: 10044 RVA: 0x00121680 File Offset: 0x0011F880
	public static List<int> GenerateNeiliAllocationProgressMinestones(int lowProgress, int highProgress)
	{
		int basic = (int)(GlobalConfig.Instance.ExtraNeiliAllocationFromProgressRatio * 100);
		int delta = (int)(GlobalConfig.Instance.ExtraNeiliAllocationFromProgressRatioGrowth * 100);
		int p0 = 0;
		int d0 = basic;
		List<int> result = new List<int>();
		while (p0 <= highProgress)
		{
			bool flag = p0 > lowProgress;
			if (flag)
			{
				result.Add(p0);
			}
			p0 += d0;
			d0 += delta;
		}
		return result;
	}

	// Token: 0x0600273D RID: 10045 RVA: 0x001216EC File Offset: 0x0011F8EC
	public static int GetNeiliAllocationMaxProgress()
	{
		int basic = (int)(GlobalConfig.Instance.ExtraNeiliAllocationFromProgressRatio * 100);
		int delta = (int)(GlobalConfig.Instance.ExtraNeiliAllocationFromProgressRatioGrowth * 100);
		short i = GlobalConfig.Instance.MaxExtraNeiliAllocation;
		return basic * (int)i + delta * (int)(i * (i - 1)) / 2;
	}

	// Token: 0x0600273E RID: 10046 RVA: 0x00121738 File Offset: 0x0011F938
	public static ValueTuple<int, float> CalculateNeiliProgressInfo(int extraNeiliAllocationProgress, int basic = -1, int delta = -1)
	{
		basic = ((basic == -1) ? ((int)(GlobalConfig.Instance.ExtraNeiliAllocationFromProgressRatio * 100)) : basic);
		delta = ((delta == -1) ? ((int)(GlobalConfig.Instance.ExtraNeiliAllocationFromProgressRatioGrowth * 100)) : delta);
		int completedNeili = LoopingCommonUtils.CalcExtraNeiliAllocationFromProgress(extraNeiliAllocationProgress, basic, delta);
		int completedProgress = basic * completedNeili + delta * completedNeili * (completedNeili - 1) / 2;
		int nextIntervalLength = basic + completedNeili * delta;
		int remainingProgress = extraNeiliAllocationProgress - completedProgress;
		float progressPercentage = (nextIntervalLength > 0) ? ((float)remainingProgress / (float)nextIntervalLength) : 0f;
		return new ValueTuple<int, float>(completedNeili, progressPercentage);
	}

	// Token: 0x0600273F RID: 10047 RVA: 0x001217B4 File Offset: 0x0011F9B4
	public static int GetExtraNeiliAllocationProgressByExtraNeiliAllocation(int extraNeiliAllocation, int basic = -1, int delta = -1)
	{
		basic = ((basic == -1) ? ((int)(GlobalConfig.Instance.ExtraNeiliAllocationFromProgressRatio * 100)) : basic);
		delta = ((delta == -1) ? ((int)(GlobalConfig.Instance.ExtraNeiliAllocationFromProgressRatioGrowth * 100)) : delta);
		return basic * extraNeiliAllocation + delta * (extraNeiliAllocation * (extraNeiliAllocation - 1)) / 2;
	}
}
