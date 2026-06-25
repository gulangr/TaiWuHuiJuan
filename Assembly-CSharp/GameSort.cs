using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using CommonSortAndFilterLegacy;
using Config;
using Game.Components.SortAndFilter;
using GameData.Serializer;
using UnityEngine;

// Token: 0x0200012E RID: 302
public class GameSort : ISingletonInit, IDisposable, GameData.Serializer.ICommonObjectSerializationAware
{
	// Token: 0x06000D36 RID: 3382 RVA: 0x00057AE0 File Offset: 0x00055CE0
	public void Dispose()
	{
		this._itemSortConfigMap.Clear();
		this._itemSortConfigMap = null;
		this._combatSkillSortConfigMap.Clear();
		this._combatSkillSortConfigMap = null;
		this._characterSortConfigMap.Clear();
		this._characterSortConfigMap = null;
		this._residentSortConfigMap.Clear();
		this._residentSortConfigMap = null;
		this._infectSortConfigMap.Clear();
		this._infectSortConfigMap = null;
		this._itemGradeFilterSettingsMap.Clear();
		this._itemGradeFilterSettingsMap = null;
		this._simpleSortConfigMap.Clear();
		this._simpleSortConfigMap = null;
		this._filterCustomOrderDataMap.Clear();
		this._filterCustomOrderDataMap = null;
		this._commonSortConfigMap.Clear();
		this._commonSortConfigMap = null;
		this._newSortConfigMap.Clear();
		this._newSortConfigMap = null;
	}

	// Token: 0x06000D37 RID: 3383 RVA: 0x00057BAC File Offset: 0x00055DAC
	public GameSort()
	{
		this.Reset();
	}

	// Token: 0x06000D38 RID: 3384 RVA: 0x00057BBD File Offset: 0x00055DBD
	public void Init()
	{
		this.Reset();
		this.Reload();
	}

	// Token: 0x06000D39 RID: 3385 RVA: 0x00057BD0 File Offset: 0x00055DD0
	private void Reset()
	{
		this._itemSortConfigMap = new Dictionary<string, ItemSortFilterSetting>();
		this._combatSkillSortConfigMap = new Dictionary<string, CombatSkillSortFilterSetting>();
		this._characterSortConfigMap = new Dictionary<string, CharacterSortFilterSetting>();
		this._residentSortConfigMap = new Dictionary<string, ResidentSortFilterSetting>();
		this._infectSortConfigMap = new Dictionary<string, InfectSortFilterSetting>();
		this._itemGradeFilterSettingsMap = new Dictionary<string, ItemGradeFilterSetting>();
		this._simpleSortConfigMap = new Dictionary<string, List<SimpleAbstractSort.Sort>>();
		this._filterCustomOrderDataMap = new Dictionary<string, FilterCustomOrderData>();
		this._commonSortConfigMap = new Dictionary<string, CommonSortAndFilterLegacy.SortStateData>();
		this._newSortConfigMap = new Dictionary<string, Game.Components.SortAndFilter.SortStateData>();
	}

	// Token: 0x06000D3A RID: 3386 RVA: 0x00057C4C File Offset: 0x00055E4C
	public bool SkipMember(MemberInfo member, bool deserializing)
	{
		return true;
	}

	// Token: 0x06000D3B RID: 3387 RVA: 0x00057C4F File Offset: 0x00055E4F
	public IEnumerable<GameData.Serializer.CommonObjectSerializationMember> ExtraMembers(bool deserializing)
	{
		yield return GameData.Serializer.CommonObjectSerializationMember.Make<Dictionary<string, ResidentSortFilterSetting>>("ResidentSortFilter", () => this._residentSortConfigMap, delegate(Dictionary<string, ResidentSortFilterSetting> v)
		{
			this._residentSortConfigMap = v;
		});
		yield return GameData.Serializer.CommonObjectSerializationMember.Make<Dictionary<string, ItemGradeFilterSetting>>("ItemGradeFilter", () => this._itemGradeFilterSettingsMap, delegate(Dictionary<string, ItemGradeFilterSetting> v)
		{
			this._itemGradeFilterSettingsMap = v;
		});
		yield return GameData.Serializer.CommonObjectSerializationMember.Make<Dictionary<string, CharacterSortFilterSetting>>("CharacterSortFilter", () => this._characterSortConfigMap, delegate(Dictionary<string, CharacterSortFilterSetting> v)
		{
			this._characterSortConfigMap = v;
		});
		yield return GameData.Serializer.CommonObjectSerializationMember.Make<Dictionary<string, List<SimpleAbstractSort.Sort>>>("SimpleAbstractSort", () => this._simpleSortConfigMap, delegate(Dictionary<string, List<SimpleAbstractSort.Sort>> v)
		{
			this._simpleSortConfigMap = v;
		});
		yield return GameData.Serializer.CommonObjectSerializationMember.Make<Dictionary<string, ItemSortFilterSetting>>("ItemSortFilter", () => this._itemSortConfigMap, delegate(Dictionary<string, ItemSortFilterSetting> v)
		{
			this._itemSortConfigMap = v;
		});
		yield return GameData.Serializer.CommonObjectSerializationMember.Make<Dictionary<string, CombatSkillSortFilterSetting>>("CombatSkillSortFilter", () => this._combatSkillSortConfigMap, delegate(Dictionary<string, CombatSkillSortFilterSetting> v)
		{
			this._combatSkillSortConfigMap = v;
		});
		yield return GameData.Serializer.CommonObjectSerializationMember.Make<Dictionary<string, InfectSortFilterSetting>>("InfectSortFilter", () => this._infectSortConfigMap, delegate(Dictionary<string, InfectSortFilterSetting> v)
		{
			this._infectSortConfigMap = v;
		});
		yield return GameData.Serializer.CommonObjectSerializationMember.Make<Dictionary<string, FilterCustomOrderData>>("FilterCustomOrder", () => this._filterCustomOrderDataMap, delegate(Dictionary<string, FilterCustomOrderData> v)
		{
			this._filterCustomOrderDataMap = (v ?? new Dictionary<string, FilterCustomOrderData>());
		});
		yield return GameData.Serializer.CommonObjectSerializationMember.Make<Dictionary<string, CommonSortAndFilterLegacy.SortStateData>>("CommonSort", () => this._commonSortConfigMap, delegate(Dictionary<string, CommonSortAndFilterLegacy.SortStateData> v)
		{
			this._commonSortConfigMap = (v ?? new Dictionary<string, CommonSortAndFilterLegacy.SortStateData>());
		});
		yield return GameData.Serializer.CommonObjectSerializationMember.Make<Dictionary<string, Game.Components.SortAndFilter.SortStateData>>("NewSort", () => this._newSortConfigMap, delegate(Dictionary<string, Game.Components.SortAndFilter.SortStateData> v)
		{
			this._newSortConfigMap = (v ?? new Dictionary<string, Game.Components.SortAndFilter.SortStateData>());
		});
		yield break;
	}

	// Token: 0x06000D3C RID: 3388 RVA: 0x00057C68 File Offset: 0x00055E68
	private void Reload()
	{
		string saveFilePath = this.EnsureSaveFilePath();
		bool flag = !File.Exists(saveFilePath) || SingletonObject.getInstance<TutorialChapterModel>().InGuiding;
		if (!flag)
		{
			try
			{
				GameData.Serializer.CommonObjectSerializer.RestoreObject<GameSort>(File.ReadAllText(saveFilePath), this, GameData.Serializer.CommonObjectSerializer.MarshalFormat.LuaWithReturnPrefix);
			}
			catch (Exception ex)
			{
				string message = string.Format("{0}", ex);
				bool flag2 = message.Length > 128;
				if (flag2)
				{
					Debug.Log(message);
					message = message.Substring(0, 128) + "...";
				}
				PredefinedLog.DefValue.ConfigurationParseFailed.Log(saveFilePath, message);
				this.Reset();
			}
		}
	}

	// Token: 0x06000D3D RID: 3389 RVA: 0x00057D14 File Offset: 0x00055F14
	public void Save()
	{
		string content;
		GameData.Serializer.CommonObjectSerializer.Serialize<GameSort>(this, out content, GameData.Serializer.CommonObjectSerializer.MarshalFormat.LuaWithReturnPrefix);
		File.WriteAllText(this.EnsureSaveFilePath(), content, Encoding.UTF8);
	}

	// Token: 0x06000D3E RID: 3390 RVA: 0x00057D40 File Offset: 0x00055F40
	private string EnsureSaveFilePath()
	{
		string archiveDir = GameApp.GetArchiveDirPath();
		bool flag = !Directory.Exists(archiveDir);
		if (flag)
		{
			Directory.CreateDirectory(archiveDir);
		}
		return Path.Combine(archiveDir, "SortConfig.lua");
	}

	// Token: 0x06000D3F RID: 3391 RVA: 0x00057D78 File Offset: 0x00055F78
	public ItemSortFilterSetting GetItemSortConfig(string tag)
	{
		bool flag = string.IsNullOrEmpty(tag);
		ItemSortFilterSetting result;
		if (flag)
		{
			result = new ItemSortFilterSetting();
		}
		else
		{
			ItemSortFilterSetting setting;
			bool flag2 = !this._itemSortConfigMap.TryGetValue(tag, out setting);
			if (flag2)
			{
				setting = new ItemSortFilterSetting();
				this._itemSortConfigMap.Add(tag, setting);
			}
			result = setting;
		}
		return result;
	}

	// Token: 0x06000D40 RID: 3392 RVA: 0x00057DC8 File Offset: 0x00055FC8
	public void SetItemSortConfig(string tag, ItemSortFilterSetting setting)
	{
		this._itemSortConfigMap[tag] = setting;
		this.Save();
	}

	// Token: 0x06000D41 RID: 3393 RVA: 0x00057DE0 File Offset: 0x00055FE0
	public CombatSkillSortFilterSetting GetCombatSkillSortConfig(string tag)
	{
		bool flag = string.IsNullOrEmpty(tag);
		CombatSkillSortFilterSetting result;
		if (flag)
		{
			result = new CombatSkillSortFilterSetting();
		}
		else
		{
			CombatSkillSortFilterSetting setting;
			bool flag2 = !this._combatSkillSortConfigMap.TryGetValue(tag, out setting);
			if (flag2)
			{
				setting = new CombatSkillSortFilterSetting();
				this._combatSkillSortConfigMap.Add(tag, setting);
			}
			result = setting;
		}
		return result;
	}

	// Token: 0x06000D42 RID: 3394 RVA: 0x00057E30 File Offset: 0x00056030
	public void SetCombatSkillSortConfig(string tag, CombatSkillSortFilterSetting setting)
	{
		this._combatSkillSortConfigMap[tag] = setting;
		this.Save();
	}

	// Token: 0x06000D43 RID: 3395 RVA: 0x00057E48 File Offset: 0x00056048
	public CharacterSortFilterSetting GetCharacterSortConfig(string tag)
	{
		bool flag = string.IsNullOrEmpty(tag);
		CharacterSortFilterSetting result;
		if (flag)
		{
			result = new CharacterSortFilterSetting();
		}
		else
		{
			CharacterSortFilterSetting setting;
			bool flag2 = !this._characterSortConfigMap.TryGetValue(tag, out setting);
			if (flag2)
			{
				setting = new CharacterSortFilterSetting();
				this._characterSortConfigMap.Add(tag, setting);
			}
			result = setting;
		}
		return result;
	}

	// Token: 0x06000D44 RID: 3396 RVA: 0x00057E98 File Offset: 0x00056098
	public void SetCharacterSortConfig(string tag, CharacterSortFilterSetting setting)
	{
		this._characterSortConfigMap[tag] = setting;
		this.Save();
	}

	// Token: 0x06000D45 RID: 3397 RVA: 0x00057EB0 File Offset: 0x000560B0
	public InfectSortFilterSetting GetInfectSortConfig(string tag)
	{
		bool flag = string.IsNullOrEmpty(tag);
		InfectSortFilterSetting result;
		if (flag)
		{
			result = new InfectSortFilterSetting();
		}
		else
		{
			InfectSortFilterSetting setting;
			bool flag2 = !this._infectSortConfigMap.TryGetValue(tag, out setting);
			if (flag2)
			{
				setting = new InfectSortFilterSetting();
				this._infectSortConfigMap.Add(tag, setting);
			}
			result = setting;
		}
		return result;
	}

	// Token: 0x06000D46 RID: 3398 RVA: 0x00057F00 File Offset: 0x00056100
	public void SetInfectSortConfig(string tag, InfectSortFilterSetting setting)
	{
		this._infectSortConfigMap[tag] = setting;
		this.Save();
	}

	// Token: 0x06000D47 RID: 3399 RVA: 0x00057F18 File Offset: 0x00056118
	public void SetResidentSortConfig(string tag, ResidentSortFilterSetting setting)
	{
		this._residentSortConfigMap[tag] = setting;
		this.Save();
	}

	// Token: 0x06000D48 RID: 3400 RVA: 0x00057F30 File Offset: 0x00056130
	public ResidentSortFilterSetting GetResidentSortConfig(string tag)
	{
		bool flag = string.IsNullOrEmpty(tag);
		ResidentSortFilterSetting result;
		if (flag)
		{
			result = new ResidentSortFilterSetting();
		}
		else
		{
			ResidentSortFilterSetting setting;
			bool flag2 = !this._residentSortConfigMap.TryGetValue(tag, out setting);
			if (flag2)
			{
				setting = new ResidentSortFilterSetting();
				this._residentSortConfigMap.Add(tag, setting);
			}
			result = setting;
		}
		return result;
	}

	// Token: 0x06000D49 RID: 3401 RVA: 0x00057F80 File Offset: 0x00056180
	public ItemGradeFilterSetting GetItemGradeFilterSetting()
	{
		bool flag = string.IsNullOrEmpty("Default");
		ItemGradeFilterSetting result;
		if (flag)
		{
			result = new ItemGradeFilterSetting();
		}
		else
		{
			ItemGradeFilterSetting setting;
			bool flag2 = !this._itemGradeFilterSettingsMap.TryGetValue("Default", out setting);
			if (flag2)
			{
				setting = new ItemGradeFilterSetting();
				this._itemGradeFilterSettingsMap.Add("Default", setting);
			}
			result = setting;
		}
		return result;
	}

	// Token: 0x06000D4A RID: 3402 RVA: 0x00057FDC File Offset: 0x000561DC
	public void SetItemGradeFilterSetting(ItemGradeFilterSetting setting)
	{
		this._itemGradeFilterSettingsMap["Default"] = setting;
		this.Save();
	}

	// Token: 0x06000D4B RID: 3403 RVA: 0x00057FF8 File Offset: 0x000561F8
	public void SetSimpleAbstractSortConfig(string key, List<SimpleAbstractSort.Sort> sorts)
	{
		this._simpleSortConfigMap[key] = sorts.ToList<SimpleAbstractSort.Sort>();
		this.Save();
	}

	// Token: 0x06000D4C RID: 3404 RVA: 0x00058018 File Offset: 0x00056218
	public List<SimpleAbstractSort.Sort> GetSimpleAbstractSortConfig(string key)
	{
		List<SimpleAbstractSort.Sort> result = new List<SimpleAbstractSort.Sort>();
		List<SimpleAbstractSort.Sort> sortArray;
		bool flag = this._simpleSortConfigMap.TryGetValue(key, out sortArray);
		List<SimpleAbstractSort.Sort> result2;
		if (flag)
		{
			result.AddRange(sortArray);
			result2 = result;
		}
		else
		{
			result2 = null;
		}
		return result2;
	}

	// Token: 0x06000D4D RID: 3405 RVA: 0x00058050 File Offset: 0x00056250
	public void SetCommonSortSortConfig(string key, CommonSortAndFilterLegacy.SortStateData sortStateData)
	{
		bool flag = sortStateData == null;
		if (flag)
		{
			this._commonSortConfigMap.Remove(key);
		}
		else
		{
			this._commonSortConfigMap[key] = sortStateData;
		}
		this.Save();
	}

	// Token: 0x06000D4E RID: 3406 RVA: 0x00058090 File Offset: 0x00056290
	public CommonSortAndFilterLegacy.SortStateData GetCommonSortSortConfig(string key)
	{
		CommonSortAndFilterLegacy.SortStateData sortState;
		bool flag = this._commonSortConfigMap.TryGetValue(key, out sortState);
		CommonSortAndFilterLegacy.SortStateData result;
		if (flag)
		{
			result = sortState;
		}
		else
		{
			result = null;
		}
		return result;
	}

	// Token: 0x06000D4F RID: 3407 RVA: 0x000580BC File Offset: 0x000562BC
	public FilterCustomOrderData LoadFilterCustomOrderData(string key)
	{
		FilterCustomOrderData data;
		bool flag = this._filterCustomOrderDataMap.TryGetValue(key, out data);
		FilterCustomOrderData result;
		if (flag)
		{
			result = data;
		}
		else
		{
			result = new FilterCustomOrderData
			{
				LineCustomOrder = new Dictionary<int, IFilterLineCustomOrderData>()
			};
		}
		return result;
	}

	// Token: 0x06000D50 RID: 3408 RVA: 0x000580F9 File Offset: 0x000562F9
	public void SaveFilterCustomOrderData(string key, FilterCustomOrderData data)
	{
		this._filterCustomOrderDataMap[key] = data;
		this.Save();
	}

	// Token: 0x06000D51 RID: 3409 RVA: 0x00058111 File Offset: 0x00056311
	public void ClearFilterCustomOrderData(string key)
	{
		this._filterCustomOrderDataMap.Remove(key);
		this.Save();
	}

	// Token: 0x06000D52 RID: 3410 RVA: 0x00058128 File Offset: 0x00056328
	public void SetNewSortConfig(string key, Game.Components.SortAndFilter.SortStateData sortStateData)
	{
		bool flag = sortStateData == null;
		if (flag)
		{
			this._newSortConfigMap.Remove(key);
		}
		else
		{
			this._newSortConfigMap[key] = sortStateData;
		}
		this.Save();
	}

	// Token: 0x06000D53 RID: 3411 RVA: 0x00058168 File Offset: 0x00056368
	public Game.Components.SortAndFilter.SortStateData GetNewSortConfig(string key)
	{
		Game.Components.SortAndFilter.SortStateData sortState;
		bool flag = this._newSortConfigMap.TryGetValue(key, out sortState);
		Game.Components.SortAndFilter.SortStateData result;
		if (flag)
		{
			result = sortState;
		}
		else
		{
			result = null;
		}
		return result;
	}

	// Token: 0x04000DE2 RID: 3554
	private Dictionary<string, ResidentSortFilterSetting> _residentSortConfigMap;

	// Token: 0x04000DE3 RID: 3555
	private Dictionary<string, ItemGradeFilterSetting> _itemGradeFilterSettingsMap;

	// Token: 0x04000DE4 RID: 3556
	private Dictionary<string, CharacterSortFilterSetting> _characterSortConfigMap;

	// Token: 0x04000DE5 RID: 3557
	private Dictionary<string, List<SimpleAbstractSort.Sort>> _simpleSortConfigMap;

	// Token: 0x04000DE6 RID: 3558
	private Dictionary<string, ItemSortFilterSetting> _itemSortConfigMap;

	// Token: 0x04000DE7 RID: 3559
	private Dictionary<string, CombatSkillSortFilterSetting> _combatSkillSortConfigMap;

	// Token: 0x04000DE8 RID: 3560
	private Dictionary<string, InfectSortFilterSetting> _infectSortConfigMap;

	// Token: 0x04000DE9 RID: 3561
	private Dictionary<string, FilterCustomOrderData> _filterCustomOrderDataMap;

	// Token: 0x04000DEA RID: 3562
	private Dictionary<string, CommonSortAndFilterLegacy.SortStateData> _commonSortConfigMap;

	// Token: 0x04000DEB RID: 3563
	private Dictionary<string, Game.Components.SortAndFilter.SortStateData> _newSortConfigMap;

	// Token: 0x04000DEC RID: 3564
	private const string SortConfigFileName = "SortConfig.lua";

	// Token: 0x04000DED RID: 3565
	private const string ItemSortFilter = "ItemSortFilter";

	// Token: 0x04000DEE RID: 3566
	private const string CharacterSortFilter = "CharacterSortFilter";

	// Token: 0x04000DEF RID: 3567
	private const string ResidentSortFilter = "ResidentSortFilter";

	// Token: 0x04000DF0 RID: 3568
	private const string InfectSortFilter = "InfectSortFilter";

	// Token: 0x04000DF1 RID: 3569
	private const string CombatSkillSortFilter = "CombatSkillSortFilter";

	// Token: 0x04000DF2 RID: 3570
	private const string ItemGradeFilter = "ItemGradeFilter";

	// Token: 0x04000DF3 RID: 3571
	private const string FilterCustomOrderKey = "FilterCustomOrder";

	// Token: 0x04000DF4 RID: 3572
	private const string CommonSortKey = "CommonSort";

	// Token: 0x04000DF5 RID: 3573
	private const string NewSortKey = "NewSort";

	// Token: 0x04000DF6 RID: 3574
	private const string NewFilterCustomOrderKey = "NewFilterCustomOrder";

	// Token: 0x04000DF7 RID: 3575
	private const string SimpleAbstractSortKey = "SimpleAbstractSort";
}
