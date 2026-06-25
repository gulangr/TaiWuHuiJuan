using System;
using GameData.Utilities;
using UnityEngine;

// Token: 0x020001F1 RID: 497
public class CombatSkillHitParts : Refers
{
	// Token: 0x06002079 RID: 8313 RVA: 0x000EC784 File Offset: 0x000EA984
	public void SetData(int[] injuryPartAtkRates, bool[] showBacks = null)
	{
		GameObject hitPartsRoot = base.CGet<GameObject>("HitParts");
		CombatSkillCommonTipRefers _commonTipRefers = base.CGet<CombatSkillCommonTipRefers>("CombatSkillCommonTipPrefab");
		GameObject hitPartsHolder = base.CGet<GameObject>("HitPartsHolder");
		for (int x = 0; x < hitPartsHolder.transform.childCount; x++)
		{
			hitPartsHolder.transform.GetChild(x).gameObject.SetActive(false);
		}
		hitPartsRoot.gameObject.SetActive(true);
		bool flag = injuryPartAtkRates.Length < 1;
		if (flag)
		{
			hitPartsRoot.gameObject.SetActive(false);
		}
		else
		{
			int sum = injuryPartAtkRates.Sum();
			for (int i = 0; i < injuryPartAtkRates.Length; i++)
			{
				int configIndex = (int)CommonUtils.ConvertShowIndexToConfigIndex((sbyte)i);
				bool flag2 = i >= hitPartsHolder.transform.childCount;
				CombatSkillCommonTipRefers tip;
				if (flag2)
				{
					tip = Object.Instantiate<CombatSkillCommonTipRefers>(_commonTipRefers, hitPartsHolder.transform);
				}
				else
				{
					tip = hitPartsHolder.transform.GetChild(i).GetComponent<CombatSkillCommonTipRefers>();
				}
				tip.gameObject.SetActive(true);
				tip.gameObject.name = MouseTipConstant.HitPartNames[i, 0];
				string strText = LocalStringManager.Get(MouseTipConstant.HitPartNames[i, 1]).ColorReplace();
				int showInt = (injuryPartAtkRates[configIndex] > 0) ? Math.Max(1, (int)((double)injuryPartAtkRates[configIndex] * 1000.0 / (double)sum + 0.5)) : 0;
				string valueText = (showInt > 0) ? string.Format("{0}.{1}%", showInt / 10, showInt % 10) : LocalStringManager.Get(LanguageKey.LK_MouseTip_DoNotTargetThisBodyPart).ColorReplace();
				bool showBack = false;
				bool flag3 = showBacks != null && configIndex < showBacks.Length;
				if (flag3)
				{
					showBack = showBacks[configIndex];
				}
				tip.SetData(strText, MouseTipConstant.HitPartNames[i, 2], valueText, showBack);
			}
		}
	}
}
