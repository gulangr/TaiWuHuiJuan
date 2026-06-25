using System;
using GameData.Domains.Taiwu.Profession;

namespace Game.Components.SortAndFilter.ProfessionSkill
{
	// Token: 0x02000D0E RID: 3342
	public class ProfessionSkillSortData
	{
		// Token: 0x04008347 RID: 33607
		public int ProfessionId;

		// Token: 0x04008348 RID: 33608
		public ProfessionData ProfessionData;

		// Token: 0x04008349 RID: 33609
		public int SkillIndex;

		// Token: 0x0400834A RID: 33610
		public int SkillId;

		// Token: 0x0400834B RID: 33611
		public int Seniority;

		// Token: 0x0400834C RID: 33612
		public string SkillName;

		// Token: 0x0400834D RID: 33613
		public bool IsUnlocked;

		// Token: 0x0400834E RID: 33614
		public bool IsEquipped;

		// Token: 0x0400834F RID: 33615
		public bool HasEmptyEquipSlotInSameLevel;

		// Token: 0x04008350 RID: 33616
		public bool IsPendingLearn;
	}
}
