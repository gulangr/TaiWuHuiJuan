using System;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000D9A RID: 3482
	public static class ItemFilterCommon
	{
		// Token: 0x0600A903 RID: 43267 RVA: 0x004E25D0 File Offset: 0x004E07D0
		public static bool IsItemMatchItemSubType(ITradeableContent data, short targetItemSubType)
		{
			short itemSubType = ItemTemplateHelper.GetItemSubType(data.Key.ItemType, data.Key.TemplateId);
			return itemSubType == targetItemSubType;
		}

		// Token: 0x0600A904 RID: 43268 RVA: 0x004E2604 File Offset: 0x004E0804
		public static bool IsEquip(sbyte itemType)
		{
			return itemType == 0 || itemType == 1 || itemType == 2 || itemType == 3 || itemType == 4;
		}

		// Token: 0x0600A905 RID: 43269 RVA: 0x004E262D File Offset: 0x004E082D
		public static bool IsFood(sbyte itemType)
		{
			return itemType == 7 || itemType == 9;
		}

		// Token: 0x0600A906 RID: 43270 RVA: 0x004E263B File Offset: 0x004E083B
		public static bool IsMedicine(sbyte itemType)
		{
			return itemType == 8;
		}

		// Token: 0x0600A907 RID: 43271 RVA: 0x004E2641 File Offset: 0x004E0841
		public static bool IsSkillBook(sbyte itemType)
		{
			return itemType == 10;
		}

		// Token: 0x0600A908 RID: 43272 RVA: 0x004E2648 File Offset: 0x004E0848
		public static bool IsMaterial(sbyte itemType)
		{
			return itemType == 5;
		}

		// Token: 0x0600A909 RID: 43273 RVA: 0x004E264E File Offset: 0x004E084E
		public static bool IsCraftTool(sbyte itemType)
		{
			return itemType == 6;
		}

		// Token: 0x0600A90A RID: 43274 RVA: 0x004E2654 File Offset: 0x004E0854
		public static bool IsMisc(sbyte itemType)
		{
			return itemType == 12;
		}

		// Token: 0x0600A90B RID: 43275 RVA: 0x004E265B File Offset: 0x004E085B
		public static bool IsCricket(sbyte itemType)
		{
			return itemType == 11;
		}

		// Token: 0x0600A90C RID: 43276 RVA: 0x004E2664 File Offset: 0x004E0864
		public static bool IsMiscOrCricket(sbyte itemType)
		{
			return ItemFilterCommon.IsMisc(itemType) || ItemFilterCommon.IsCricket(itemType);
		}
	}
}
