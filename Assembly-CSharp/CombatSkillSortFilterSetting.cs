using System;
using System.Collections.Generic;
using GameData.Serializer;

// Token: 0x020001E9 RID: 489
public class CombatSkillSortFilterSetting : GameData.Serializer.ICommonObjectSerializationAware
{
	// Token: 0x0600201E RID: 8222 RVA: 0x000EA058 File Offset: 0x000E8258
	public CombatSkillSortFilterSetting() : this(true)
	{
	}

	// Token: 0x0600201F RID: 8223 RVA: 0x000EA064 File Offset: 0x000E8264
	private CombatSkillSortFilterSetting(bool setInitFilterType)
	{
		if (setInitFilterType)
		{
			for (sbyte type = 0; type < 17; type += 1)
			{
				this.SectType.Add(type);
			}
		}
	}

	// Token: 0x06002020 RID: 8224 RVA: 0x000EA0C8 File Offset: 0x000E82C8
	public bool DeserializingUnknownField(string name, out GameData.Serializer.CommonObjectSerializationMember proc)
	{
		bool result;
		if (!(name == "TypeFilter"))
		{
			if (!(name == "SectFilter"))
			{
				if (!(name == "SortType"))
				{
					if (!(name == "SortOrder"))
					{
						proc = default(GameData.Serializer.CommonObjectSerializationMember);
						result = false;
					}
					else
					{
						proc = GameData.Serializer.CommonObjectSerializationMember.MakeListRefill<bool>(name, this.IsDescSort);
						result = true;
					}
				}
				else
				{
					proc = GameData.Serializer.CommonObjectSerializationMember.MakeListRefill<CombatSkillSortAndFilter.SortType>(name, this.SortTypes);
					result = true;
				}
			}
			else
			{
				proc = GameData.Serializer.CommonObjectSerializationMember.MakeListRefill<sbyte>(name, this.SectType);
				result = true;
			}
		}
		else
		{
			proc = GameData.Serializer.CommonObjectSerializationMember.Make<sbyte>(name, () => this.SkillType, delegate(sbyte v)
			{
				this.SkillType = v;
			});
			result = true;
		}
		return result;
	}

	// Token: 0x04001835 RID: 6197
	public sbyte SkillType = 0;

	// Token: 0x04001836 RID: 6198
	public readonly List<sbyte> SectType = new List<sbyte>();

	// Token: 0x04001837 RID: 6199
	public readonly List<CombatSkillSortAndFilter.SortType> SortTypes = new List<CombatSkillSortAndFilter.SortType>();

	// Token: 0x04001838 RID: 6200
	public readonly List<bool> IsDescSort = new List<bool>();

	// Token: 0x04001839 RID: 6201
	private const string CombatSkillTypeFilter = "TypeFilter";

	// Token: 0x0400183A RID: 6202
	private const string CombatSkillSectFilter = "SectFilter";

	// Token: 0x0400183B RID: 6203
	private const string CombatSkillSortType = "SortType";

	// Token: 0x0400183C RID: 6204
	private const string CombatSkillSortOrder = "SortOrder";
}
