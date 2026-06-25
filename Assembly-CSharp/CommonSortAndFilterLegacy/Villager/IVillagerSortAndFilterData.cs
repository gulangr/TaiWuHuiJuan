using System;

namespace CommonSortAndFilterLegacy.Villager
{
	// Token: 0x02000464 RID: 1124
	public interface IVillagerSortAndFilterData
	{
		// Token: 0x170006AB RID: 1707
		// (get) Token: 0x06004087 RID: 16519
		string Name { get; }

		// Token: 0x170006AC RID: 1708
		// (get) Token: 0x06004088 RID: 16520
		sbyte Gender { get; }

		// Token: 0x170006AD RID: 1709
		// (get) Token: 0x06004089 RID: 16521
		short Age { get; }

		// Token: 0x170006AE RID: 1710
		// (get) Token: 0x0600408A RID: 16522
		sbyte Grade { get; }

		// Token: 0x170006AF RID: 1711
		// (get) Token: 0x0600408B RID: 16523
		sbyte BehaviorType { get; }

		// Token: 0x170006B0 RID: 1712
		// (get) Token: 0x0600408C RID: 16524
		ushort RelationToTaiwu { get; }

		// Token: 0x170006B1 RID: 1713
		// (get) Token: 0x0600408D RID: 16525
		ushort RelationFromTaiwu { get; }

		// Token: 0x170006B2 RID: 1714
		// (get) Token: 0x0600408E RID: 16526
		short Charm { get; }

		// Token: 0x170006B3 RID: 1715
		// (get) Token: 0x0600408F RID: 16527
		short Health { get; }

		// Token: 0x170006B4 RID: 1716
		// (get) Token: 0x06004090 RID: 16528
		short FavorabilityToTaiwu { get; }

		// Token: 0x170006B5 RID: 1717
		// (get) Token: 0x06004091 RID: 16529
		short Happiness { get; }

		// Token: 0x170006B6 RID: 1718
		// (get) Token: 0x06004092 RID: 16530
		sbyte LeftPotentialCount { get; }

		// Token: 0x06004093 RID: 16531
		bool IsArrangementMatched(EArrangementMenuOption arrangementMenuOption);

		// Token: 0x06004094 RID: 16532
		bool IsWorkStatusMatched(EWorkStatusMenuOption workStatusMenuOption);

		// Token: 0x170006B7 RID: 1719
		// (get) Token: 0x06004095 RID: 16533
		bool IsSameFactionWithTaiwu { get; }
	}
}
