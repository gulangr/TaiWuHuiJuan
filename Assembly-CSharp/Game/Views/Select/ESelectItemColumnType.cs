using System;

namespace Game.Views.Select
{
	// Token: 0x020007A1 RID: 1953
	[Flags]
	public enum ESelectItemColumnType
	{
		// Token: 0x04004146 RID: 16710
		None = 0,
		// Token: 0x04004147 RID: 16711
		IconAndName = 1,
		// Token: 0x04004148 RID: 16712
		Amount = 2,
		// Token: 0x04004149 RID: 16713
		Type = 4,
		// Token: 0x0400414A RID: 16714
		Value = 8,
		// Token: 0x0400414B RID: 16715
		Weight = 16,
		// Token: 0x0400414C RID: 16716
		Durability = 32,
		// Token: 0x0400414D RID: 16717
		Power = 64,
		// Token: 0x0400414E RID: 16718
		Penetration = 128,
		// Token: 0x0400414F RID: 16719
		ArmorBreak = 256,
		// Token: 0x04004150 RID: 16720
		Toughness = 512,
		// Token: 0x04004151 RID: 16721
		AccessoryEffect = 1024,
		// Token: 0x04004152 RID: 16722
		Charm = 2048,
		// Token: 0x04004153 RID: 16723
		TravelTimeReduction = 4096,
		// Token: 0x04004154 RID: 16724
		InventoryBonus = 8192,
		// Token: 0x04004155 RID: 16725
		BookReadingInfo = 16384,
		// Token: 0x04004156 RID: 16726
		ToolEffect = 32768,
		// Token: 0x04004157 RID: 16727
		RefiningEffect = 65536,
		// Token: 0x04004158 RID: 16728
		CricketAge = 131072,
		// Token: 0x04004159 RID: 16729
		CricketDurability = 262144,
		// Token: 0x0400415A RID: 16730
		CricketWins = 524288,
		// Token: 0x0400415B RID: 16731
		CricketLosses = 1048576,
		// Token: 0x0400415C RID: 16732
		CricketHp = 2097152,
		// Token: 0x0400415D RID: 16733
		CricketSp = 4194304,
		// Token: 0x0400415E RID: 16734
		CricketVigor = 8388608,
		// Token: 0x0400415F RID: 16735
		CricketStrength = 16777216,
		// Token: 0x04004160 RID: 16736
		CricketBite = 33554432,
		// Token: 0x04004161 RID: 16737
		EscapeRate = 67108864,
		// Token: 0x04004162 RID: 16738
		CombatSkillType = 134217728,
		// Token: 0x04004163 RID: 16739
		DishDurability = 268435456,
		// Token: 0x04004164 RID: 16740
		AmountWithSelected = 536870912
	}
}
