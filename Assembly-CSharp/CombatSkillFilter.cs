using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Config;
using GameData.Domains.CombatSkill;
using TMPro;
using UnityEngine;

// Token: 0x020001E0 RID: 480
public class CombatSkillFilter : Refers
{
	// Token: 0x14000014 RID: 20
	// (add) Token: 0x06001F92 RID: 8082 RVA: 0x000E621C File Offset: 0x000E441C
	// (remove) Token: 0x06001F93 RID: 8083 RVA: 0x000E6254 File Offset: 0x000E4454
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action OnOutputChanged;

	// Token: 0x17000337 RID: 823
	// (get) Token: 0x06001F94 RID: 8084 RVA: 0x000E6289 File Offset: 0x000E4489
	public IReadOnlyList<IFilterableCombatSkill> OutputCombatSkills
	{
		get
		{
			return this._outputCombatSkills;
		}
	}

	// Token: 0x06001F95 RID: 8085 RVA: 0x000E6291 File Offset: 0x000E4491
	private void Awake()
	{
		this.Init();
	}

	// Token: 0x06001F96 RID: 8086 RVA: 0x000E629C File Offset: 0x000E449C
	public void Init()
	{
		bool componentInitialized = this._componentInitialized;
		if (!componentInitialized)
		{
			this._typeFilterTogGroup = base.CGet<CToggleGroupObsolete>("TypeFilter");
			this._typeFilterTogGroup.InitPreOnToggle(-1);
			this._typeFilterTogGroup.OnActiveToggleChange = new Action<CToggleObsolete, CToggleObsolete>(this.OnTypeFilterTogChange);
			this._sectFilterTogGroup = base.CGet<CToggleGroupObsolete>("SectFilter");
			this._sectFilterTogGroup.InitPreOnToggle(-1);
			this._sectFilterTogGroup.OnActiveToggleChange = new Action<CToggleObsolete, CToggleObsolete>(this.OnSectFilterTogChange);
			for (int i = 0; i < 16; i++)
			{
				CToggleObsolete tog = this._sectFilterTogGroup.Get(i + 1);
				tog.GetComponentInChildren<TextMeshProUGUI>().text = LocalStringManager.Get(string.Format("LK_CombatSkill_Filter_Sect{0}", i));
			}
			this._componentInitialized = true;
		}
	}

	// Token: 0x06001F97 RID: 8087 RVA: 0x000E6370 File Offset: 0x000E4570
	public void SetData(IEnumerable<IFilterableCombatSkill> originalCombatSkills, bool reset = false, Action onOutputChanged = null)
	{
		this.SetOriginalData(originalCombatSkills);
		if (reset)
		{
			this.OnOutputChanged = onOutputChanged;
			this.ResetFilters();
		}
		this.UpdateOutput();
	}

	// Token: 0x06001F98 RID: 8088 RVA: 0x000E63A4 File Offset: 0x000E45A4
	public IReadOnlyList<IFilterableCombatSkill> GetOriginalData()
	{
		return this._filterableCombatSkills;
	}

	// Token: 0x06001F99 RID: 8089 RVA: 0x000E63BC File Offset: 0x000E45BC
	private void SetOriginalData(IEnumerable<IFilterableCombatSkill> originalCombatSkills)
	{
		this._filterableCombatSkills.Clear();
		bool flag = originalCombatSkills != null;
		if (flag)
		{
			this._filterableCombatSkills.AddRange(originalCombatSkills);
		}
		for (int i = 0; i < CombatSkillTypeTogGroup.ToggleConfig.Count; i++)
		{
			CombatSkillTypeTogGroup.ToggleInfo toggleInfo = CombatSkillTypeTogGroup.ToggleConfig[i];
			int learnedCount = this._filterableCombatSkills.FindAll(delegate(IFilterableCombatSkill data)
			{
				CombatSkillItem skillConfig = CombatSkill.Instance[data.TemplateId];
				return skillConfig.Type == toggleInfo.CombatSkillType && skillConfig.EquipType == toggleInfo.EquipType;
			}).Count;
			this._typeFilterTogGroup.Get(i).GetComponent<Refers>().CGet<TextMeshProUGUI>("BookCount").text = learnedCount.ToString();
		}
	}

	// Token: 0x06001F9A RID: 8090 RVA: 0x000E6464 File Offset: 0x000E4664
	private void ResetFilters()
	{
		bool activeSelf = this._typeFilterTogGroup.gameObject.activeSelf;
		if (activeSelf)
		{
			this._typeFilterTogGroup.Set(-1, true, false);
		}
		bool activeSelf2 = this._sectFilterTogGroup.gameObject.activeSelf;
		if (activeSelf2)
		{
			this._internalInitializing = true;
			this._sectFilterTogGroup.Set(0, true, false);
			this._internalInitializing = false;
		}
	}

	// Token: 0x06001F9B RID: 8091 RVA: 0x000E64CA File Offset: 0x000E46CA
	private void OnTypeFilterTogChange(CToggleObsolete togNew, CToggleObsolete togOld)
	{
		this.UpdateOutput();
	}

	// Token: 0x06001F9C RID: 8092 RVA: 0x000E64D4 File Offset: 0x000E46D4
	private void OnSectFilterTogChange(CToggleObsolete togNew, CToggleObsolete togOld)
	{
		this.UpdateOutput();
	}

	// Token: 0x06001F9D RID: 8093 RVA: 0x000E64E0 File Offset: 0x000E46E0
	private void UpdateOutput()
	{
		bool internalInitializing = this._internalInitializing;
		if (!internalInitializing)
		{
			IEnumerable<IFilterableCombatSkill> filterResult = this._filterableCombatSkills;
			bool flag = this._typeFilterTogGroup.gameObject.activeSelf && this._typeFilterTogGroup.GetActive().Key != -1;
			if (flag)
			{
				int toggleKey = this._typeFilterTogGroup.GetActive().Key;
				bool flag2 = toggleKey >= 0 && toggleKey < CombatSkillTypeTogGroup.ToggleConfig.Count;
				if (flag2)
				{
					CombatSkillTypeTogGroup.ToggleInfo toggleInfo = CombatSkillTypeTogGroup.ToggleConfig[toggleKey];
					filterResult = filterResult.Where(delegate(IFilterableCombatSkill data)
					{
						CombatSkillItem skillConfig = CombatSkill.Instance[data.TemplateId];
						return skillConfig.Type == toggleInfo.CombatSkillType && skillConfig.EquipType == toggleInfo.EquipType;
					});
				}
			}
			else
			{
				filterResult = this._filterableCombatSkills;
			}
			int sectTogKey = this._sectFilterTogGroup.GetActive().Key;
			bool flag3 = this._sectFilterTogGroup.gameObject.activeSelf && sectTogKey != 0;
			if (flag3)
			{
				filterResult = from data in filterResult
				where Mathf.Min((int)data.SectId, 17) == sectTogKey - 1
				select data;
			}
			this._outputCombatSkills.Clear();
			this._outputCombatSkills.AddRange(filterResult);
			this._outputCombatSkills.Sort(new Comparison<IFilterableCombatSkill>(this.ItemCompare));
			Action onOutputChanged = this.OnOutputChanged;
			if (onOutputChanged != null)
			{
				onOutputChanged();
			}
		}
	}

	// Token: 0x06001F9E RID: 8094 RVA: 0x000E6634 File Offset: 0x000E4834
	private int ItemCompare(IFilterableCombatSkill skillL, IFilterableCombatSkill skillR)
	{
		return skillL.TemplateId.CompareTo(skillR.TemplateId);
	}

	// Token: 0x040017BB RID: 6075
	public const sbyte SectFilterTypeCount = 16;

	// Token: 0x040017BC RID: 6076
	public const sbyte OtherSectFilterType = 17;

	// Token: 0x040017BD RID: 6077
	private bool _componentInitialized;

	// Token: 0x040017BF RID: 6079
	private readonly List<IFilterableCombatSkill> _filterableCombatSkills = new List<IFilterableCombatSkill>();

	// Token: 0x040017C0 RID: 6080
	private readonly List<IFilterableCombatSkill> _outputCombatSkills = new List<IFilterableCombatSkill>();

	// Token: 0x040017C1 RID: 6081
	private CToggleGroupObsolete _typeFilterTogGroup;

	// Token: 0x040017C2 RID: 6082
	private CToggleGroupObsolete _sectFilterTogGroup;

	// Token: 0x040017C3 RID: 6083
	private bool _internalInitializing;
}
