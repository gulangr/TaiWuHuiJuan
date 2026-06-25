using System;
using System.Collections.Generic;
using Config;
using TMPro;
using UnityEngine;

// Token: 0x020001EA RID: 490
public class CombatSkillTypeTogGroup : MonoBehaviour
{
	// Token: 0x06002023 RID: 8227 RVA: 0x000EA198 File Offset: 0x000E8398
	public void Awake()
	{
		CToggleGroupObsolete togGroup = base.GetComponent<CToggleGroupObsolete>();
		Refers togRefers = togGroup.Get(-1).GetComponent<Refers>();
		togRefers.CGet<TextMeshProUGUI>("Label").text = LocalStringManager.Get(LanguageKey.LK_Filter_Type_All);
		for (int i = 0; i < CombatSkillTypeTogGroup.ToggleConfig.Count; i++)
		{
			CombatSkillTypeTogGroup.ToggleInfo toggleInfo = CombatSkillTypeTogGroup.ToggleConfig[i];
			togRefers = togGroup.Get(i).GetComponent<Refers>();
			togRefers.CGet<TextMeshProUGUI>("Label").text = LocalStringManager.Get(toggleInfo.LabelKey);
			CImage iconImage = togRefers.CGet<CImage>("Icon");
			iconImage.SetSprite(toggleInfo.GetIcon(), false, null);
		}
	}

	// Token: 0x06002024 RID: 8228 RVA: 0x000EA248 File Offset: 0x000E8448
	public void SetLearnedCount(IList<short> learnedCombatSkillIds)
	{
		for (int i = 0; i < CombatSkillTypeTogGroup.ToggleConfig.Count; i++)
		{
			int learnedCount = 0;
			foreach (short skillId in learnedCombatSkillIds)
			{
				CombatSkillItem skillConfig = CombatSkill.Instance[skillId];
				bool flag = skillConfig.Type == CombatSkillTypeTogGroup.ToggleConfig[i].CombatSkillType && skillConfig.EquipType == CombatSkillTypeTogGroup.ToggleConfig[i].EquipType;
				if (flag)
				{
					learnedCount++;
				}
			}
			TextMeshProUGUI label = base.GetComponent<CToggleGroupObsolete>().Get(i).GetComponent<Refers>().CGet<TextMeshProUGUI>("BookCount");
			label.text = learnedCount.ToString();
		}
	}

	// Token: 0x06002025 RID: 8229 RVA: 0x000EA330 File Offset: 0x000E8530
	public CombatSkillTypeTogGroup.ToggleInfo? GetSelectedSkillTypeToggleInfo()
	{
		int key = base.GetComponent<CToggleGroupObsolete>().GetActive().Key;
		bool flag = key < 0;
		CombatSkillTypeTogGroup.ToggleInfo? result;
		if (flag)
		{
			result = null;
		}
		else
		{
			result = new CombatSkillTypeTogGroup.ToggleInfo?(CombatSkillTypeTogGroup.ToggleConfig[key]);
		}
		return result;
	}

	// Token: 0x06002026 RID: 8230 RVA: 0x000EA378 File Offset: 0x000E8578
	public CombatSkillTypeTogGroup.ToggleInfo? GetToggleInfoByKey(int key)
	{
		bool flag = key < 0 || key >= CombatSkillTypeTogGroup.ToggleConfig.Count;
		CombatSkillTypeTogGroup.ToggleInfo? result;
		if (flag)
		{
			result = null;
		}
		else
		{
			result = new CombatSkillTypeTogGroup.ToggleInfo?(CombatSkillTypeTogGroup.ToggleConfig[key]);
		}
		return result;
	}

	// Token: 0x0400183D RID: 6205
	public static readonly List<CombatSkillTypeTogGroup.ToggleInfo> ToggleConfig = new List<CombatSkillTypeTogGroup.ToggleInfo>
	{
		new CombatSkillTypeTogGroup.ToggleInfo(0, 0, LanguageKey.LK_CombatSkillType_0),
		new CombatSkillTypeTogGroup.ToggleInfo(1, 2, LanguageKey.LK_CombatSkillType_1),
		new CombatSkillTypeTogGroup.ToggleInfo(2, 3, LanguageKey.LK_CombatSkill_EquipType_3),
		new CombatSkillTypeTogGroup.ToggleInfo(2, 4, LanguageKey.LK_CombatSkill_EquipType_4),
		new CombatSkillTypeTogGroup.ToggleInfo(3, 1, LanguageKey.LK_CombatSkillType_3),
		new CombatSkillTypeTogGroup.ToggleInfo(4, 1, LanguageKey.LK_CombatSkillType_4),
		new CombatSkillTypeTogGroup.ToggleInfo(5, 1, LanguageKey.LK_CombatSkillType_5),
		new CombatSkillTypeTogGroup.ToggleInfo(6, 1, LanguageKey.LK_CombatSkillType_6),
		new CombatSkillTypeTogGroup.ToggleInfo(7, 1, LanguageKey.LK_CombatSkillType_7),
		new CombatSkillTypeTogGroup.ToggleInfo(8, 1, LanguageKey.LK_CombatSkillType_8),
		new CombatSkillTypeTogGroup.ToggleInfo(9, 1, LanguageKey.LK_CombatSkillType_9),
		new CombatSkillTypeTogGroup.ToggleInfo(10, 1, LanguageKey.LK_CombatSkillType_10),
		new CombatSkillTypeTogGroup.ToggleInfo(11, 1, LanguageKey.LK_CombatSkillType_11),
		new CombatSkillTypeTogGroup.ToggleInfo(12, 1, LanguageKey.LK_CombatSkillType_12),
		new CombatSkillTypeTogGroup.ToggleInfo(13, 1, LanguageKey.LK_CombatSkillType_13)
	};

	// Token: 0x0200147D RID: 5245
	public struct ToggleInfo
	{
		// Token: 0x0600CC08 RID: 52232 RVA: 0x00595770 File Offset: 0x00593970
		public ToggleInfo(sbyte combatSkillType, sbyte equipType, LanguageKey labelKey)
		{
			this.CombatSkillType = combatSkillType;
			this.EquipType = equipType;
			this.LabelKey = labelKey;
		}

		// Token: 0x0600CC09 RID: 52233 RVA: 0x00595788 File Offset: 0x00593988
		public string GetIcon()
		{
			return string.Format("sp_icon_wuxue_{0}", this.CombatSkillType);
		}

		// Token: 0x0400A168 RID: 41320
		public readonly sbyte CombatSkillType;

		// Token: 0x0400A169 RID: 41321
		public readonly sbyte EquipType;

		// Token: 0x0400A16A RID: 41322
		public readonly LanguageKey LabelKey;
	}
}
