using System;
using System.Collections.Generic;
using GameData.Serializer;

// Token: 0x02000355 RID: 853
public class ResidentSortFilterSetting : GameData.Serializer.ICommonObjectSerializationAware
{
	// Token: 0x060031C9 RID: 12745 RVA: 0x0018989A File Offset: 0x00187A9A
	public ResidentSortFilterSetting() : this(true)
	{
	}

	// Token: 0x060031CA RID: 12746 RVA: 0x001898A8 File Offset: 0x00187AA8
	public ResidentSortFilterSetting(bool setInitFilterType)
	{
		if (setInitFilterType)
		{
			for (ResidentSortAndFilter.ResidentFilterType type = ResidentSortAndFilter.ResidentFilterType.Resident; type < ResidentSortAndFilter.ResidentFilterType.Count; type++)
			{
				this.ResidentFilterType.Add(type);
			}
		}
	}

	// Token: 0x060031CB RID: 12747 RVA: 0x001898EC File Offset: 0x00187AEC
	public bool DeserializingUnknownField(string name, out GameData.Serializer.CommonObjectSerializationMember proc)
	{
		int idx;
		bool flag = int.TryParse(name, out idx) && idx >= 0 && idx < this.ResidentFilterType.Count;
		if (flag)
		{
			proc = GameData.Serializer.CommonObjectSerializationMember.Make<ResidentSortAndFilter.ResidentFilterType>(name, () => this.ResidentFilterType.CheckIndex(idx) ? this.ResidentFilterType[idx] : ResidentSortAndFilter.ResidentFilterType.Invalid, delegate(ResidentSortAndFilter.ResidentFilterType v)
			{
				while (this.ResidentFilterType.Count <= idx)
				{
					this.ResidentFilterType.Add(ResidentSortAndFilter.ResidentFilterType.Invalid);
				}
				this.ResidentFilterType[idx] = v;
			});
		}
		proc = default(GameData.Serializer.CommonObjectSerializationMember);
		return false;
	}

	// Token: 0x0400247E RID: 9342
	public readonly List<ResidentSortAndFilter.ResidentFilterType> ResidentFilterType = new List<ResidentSortAndFilter.ResidentFilterType>();
}
