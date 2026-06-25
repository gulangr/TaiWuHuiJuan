using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x020002E5 RID: 741
public class TipRequirement : Refers
{
	// Token: 0x06002BD8 RID: 11224 RVA: 0x00157058 File Offset: 0x00155258
	public void RefreshRequirement([TupleElementNames(new string[]
	{
		"type",
		"required",
		"actual"
	})] List<ValueTuple<int, int, int>> requirements, bool isConvertValue = false, string requireValueColorName = "", bool showSpecialBack = false, bool considerCombatSkillProficiencyEffective = false)
	{
		RectTransform requirementHolder = base.CGet<RectTransform>("RequirementHolder");
		TipsRequireProperty requirePropertyTemplate = base.CGet<TipsRequireProperty>("RequireProperty");
		int requirementCount = (requirements != null) ? requirements.Count : 0;
		int needGrayTypeId = TipRequirement.GenerateNeedGrayType(requirements, considerCombatSkillProficiencyEffective);
		bool flag = requirements != null;
		if (flag)
		{
			CommonUtils.PrepareEnoughChildren(requirementHolder.transform, requirePropertyTemplate.gameObject, requirements.Count, null);
			for (int i = 0; i < requirements.Count; i++)
			{
				ValueTuple<int, int, int> requirement = requirements[i];
				TipsRequireProperty requireProperty = requirementHolder.GetChild(i).GetComponent<TipsRequireProperty>();
				requireProperty.transform.localScale = Vector3.one;
				bool needGray = requirement.Item1 == needGrayTypeId || (requirement.Item1 == 135 && requirement.Item2 > requirement.Item3);
				if (isConvertValue)
				{
					requireProperty.SetData((short)requirement.Item1, -1, requirement.Item2, requireValueColorName, needGray);
				}
				else
				{
					requireProperty.SetData((short)requirement.Item1, requirement.Item3, requirement.Item2, requireValueColorName, needGray);
				}
				requireProperty.gameObject.SetActive(true);
			}
		}
		for (int j = requirementCount; j < requirementHolder.childCount; j++)
		{
			requirementHolder.GetChild(j).gameObject.SetActive(false);
		}
		base.CGet<GameObject>("SpecialBack").SetActive(showSpecialBack);
	}

	// Token: 0x06002BD9 RID: 11225 RVA: 0x001571DC File Offset: 0x001553DC
	private static int GenerateNeedGrayType(List<ValueTuple<int, int, int>> requirements, bool enable)
	{
		bool flag = !enable || requirements == null;
		int result;
		if (flag)
		{
			result = -1;
		}
		else
		{
			result = (from t in requirements
			select new ValueTuple<int, float>(t.Item1, (float)t.Item3 / (float)t.Item2) into t
			orderby t.Item2
			select t).First<ValueTuple<int, float>>().Item1;
		}
		return result;
	}
}
