using System;
using System.Collections.Generic;
using GameData.Serializer;

// Token: 0x0200034F RID: 847
public class InfectSortFilterSetting : GameData.Serializer.ICommonObjectSerializationAware
{
	// Token: 0x06003165 RID: 12645 RVA: 0x001856C0 File Offset: 0x001838C0
	public InfectSortFilterSetting()
	{
		for (InfectSortAndFilter.FilterType type = InfectSortAndFilter.FilterType.All; type < InfectSortAndFilter.FilterType.Count; type++)
		{
			this.FilterTypes.Add(type);
		}
	}

	// Token: 0x06003166 RID: 12646 RVA: 0x00185714 File Offset: 0x00183914
	public bool DeserializingUnknownField(string name, out GameData.Serializer.CommonObjectSerializationMember proc)
	{
		int idx;
		bool flag = int.TryParse(name, out idx);
		bool result;
		if (flag)
		{
			proc = GameData.Serializer.CommonObjectSerializationMember.Make<InfectSortAndFilter.FilterType>(name, () => this.FilterTypes.CheckIndex(idx) ? this.FilterTypes[idx] : InfectSortAndFilter.FilterType.Teammate, delegate(InfectSortAndFilter.FilterType v)
			{
				while (this.FilterTypes.Count <= idx)
				{
					this.FilterTypes.Add(InfectSortAndFilter.FilterType.Teammate);
				}
				this.FilterTypes[idx] = v;
			});
			result = true;
		}
		else
		{
			proc = default(GameData.Serializer.CommonObjectSerializationMember);
			result = false;
		}
		return result;
	}

	// Token: 0x04002431 RID: 9265
	public readonly List<InfectSortAndFilter.FilterType> FilterTypes = new List<InfectSortAndFilter.FilterType>();

	// Token: 0x04002432 RID: 9266
	public readonly List<InfectSortAndFilter.SortType> SortTypes = new List<InfectSortAndFilter.SortType>();

	// Token: 0x04002433 RID: 9267
	public readonly List<bool> IsDescSort = new List<bool>();
}
