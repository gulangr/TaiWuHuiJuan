using System;
using GameData.Domains.Taiwu;

// Token: 0x02000367 RID: 871
public static class SkillBreakPlateStepStateExtensions
{
	// Token: 0x06003265 RID: 12901 RVA: 0x0018D87C File Offset: 0x0018BA7C
	public static ESkillBreakStepState GetStepState(this SkillBreakPlate plate)
	{
		bool flag = plate.StepCostedNormal == 0 && plate.StepCostedGoneMad == 0;
		ESkillBreakStepState result;
		if (flag)
		{
			result = ESkillBreakStepState.NoStepCosted;
		}
		else
		{
			bool flag2 = plate.StepCostedNormal < plate.StepNormal / 2;
			if (flag2)
			{
				result = ESkillBreakStepState.CostStepBelowHalf;
			}
			else
			{
				bool flag3 = plate.StepCostedNormal < plate.StepNormal;
				if (flag3)
				{
					result = ESkillBreakStepState.CostStepAboveOrEqualHalf;
				}
				else
				{
					result = ((plate.StepCostedGoneMad > 0) ? ESkillBreakStepState.InGoneMad : ESkillBreakStepState.CostStepEqualMaxNormal);
				}
			}
		}
		return result;
	}

	// Token: 0x06003266 RID: 12902 RVA: 0x0018D8E8 File Offset: 0x0018BAE8
	public static bool GetIsInGoneMad(this SkillBreakPlate plate)
	{
		return plate.GetStepState() == ESkillBreakStepState.InGoneMad;
	}
}
