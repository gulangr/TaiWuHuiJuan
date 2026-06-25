using System;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x02000512 RID: 1298
	public static class ItemFilterCommon
	{
		// Token: 0x060042FF RID: 17151 RVA: 0x00205C2C File Offset: 0x00203E2C
		public static bool IsItemMatchItemSubType(ItemDisplayData data, short targetItemSubType)
		{
			short itemSubType = ItemTemplateHelper.GetItemSubType(data.Key.ItemType, data.Key.TemplateId);
			return itemSubType == targetItemSubType;
		}

		// Token: 0x06004300 RID: 17152 RVA: 0x00205C60 File Offset: 0x00203E60
		public static bool IsEquip(sbyte itemType)
		{
			return itemType == 0 || itemType == 1 || itemType == 2 || itemType == 3 || itemType == 4;
		}

		// Token: 0x06004301 RID: 17153 RVA: 0x00205C89 File Offset: 0x00203E89
		public static bool IsFood(sbyte itemType)
		{
			return itemType == 7 || itemType == 9;
		}

		// Token: 0x06004302 RID: 17154 RVA: 0x00205C97 File Offset: 0x00203E97
		public static bool IsMedicine(sbyte itemType)
		{
			return itemType == 8;
		}

		// Token: 0x06004303 RID: 17155 RVA: 0x00205C9D File Offset: 0x00203E9D
		public static bool IsSkillBook(sbyte itemType)
		{
			return itemType == 10;
		}

		// Token: 0x06004304 RID: 17156 RVA: 0x00205CA4 File Offset: 0x00203EA4
		public static bool IsMaterial(sbyte itemType)
		{
			return itemType == 5;
		}

		// Token: 0x06004305 RID: 17157 RVA: 0x00205CAA File Offset: 0x00203EAA
		public static bool IsCraftTool(sbyte itemType)
		{
			return itemType == 6;
		}

		// Token: 0x06004306 RID: 17158 RVA: 0x00205CB0 File Offset: 0x00203EB0
		public static bool IsMisc(sbyte itemType)
		{
			return itemType == 12;
		}

		// Token: 0x06004307 RID: 17159 RVA: 0x00205CB7 File Offset: 0x00203EB7
		public static bool IsCricket(sbyte itemType)
		{
			return itemType == 11;
		}

		// Token: 0x06004308 RID: 17160 RVA: 0x00205CC0 File Offset: 0x00203EC0
		public static bool IsMiscOrCricket(sbyte itemType)
		{
			return ItemFilterCommon.IsMisc(itemType) || ItemFilterCommon.IsCricket(itemType);
		}
	}
}
