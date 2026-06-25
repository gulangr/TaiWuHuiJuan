using System;
using GameData.Domains.Character;

namespace CommonSortAndFilterLegacy.Character
{
	// Token: 0x0200059C RID: 1436
	public interface ICharacterSortAndFilterData
	{
		// Token: 0x17000883 RID: 2179
		// (get) Token: 0x0600454A RID: 17738
		string Name { get; }

		// Token: 0x17000884 RID: 2180
		// (get) Token: 0x0600454B RID: 17739
		sbyte Gender { get; }

		// Token: 0x17000885 RID: 2181
		// (get) Token: 0x0600454C RID: 17740
		short Age { get; }

		// Token: 0x17000886 RID: 2182
		// (get) Token: 0x0600454D RID: 17741
		sbyte Grade { get; }

		// Token: 0x17000887 RID: 2183
		// (get) Token: 0x0600454E RID: 17742
		sbyte BehaviorType { get; }

		// Token: 0x17000888 RID: 2184
		// (get) Token: 0x0600454F RID: 17743
		ushort RelationToTaiwu { get; }

		// Token: 0x17000889 RID: 2185
		// (get) Token: 0x06004550 RID: 17744
		ushort RelationFromTaiwu { get; }

		// Token: 0x1700088A RID: 2186
		// (get) Token: 0x06004551 RID: 17745
		short Charm { get; }

		// Token: 0x1700088B RID: 2187
		// (get) Token: 0x06004552 RID: 17746
		short Health { get; }

		// Token: 0x1700088C RID: 2188
		// (get) Token: 0x06004553 RID: 17747
		short FavorabilityToTaiwu { get; }

		// Token: 0x1700088D RID: 2189
		// (get) Token: 0x06004554 RID: 17748
		short Happiness { get; }

		// Token: 0x1700088E RID: 2190
		// (get) Token: 0x06004555 RID: 17749
		OrganizationInfo Organization { get; }

		// Token: 0x1700088F RID: 2191
		// (get) Token: 0x06004556 RID: 17750
		bool IsSameFactionWithTaiwu { get; }
	}
}
