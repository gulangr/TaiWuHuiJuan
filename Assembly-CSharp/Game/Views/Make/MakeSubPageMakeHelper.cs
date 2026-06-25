using System;
using System.Collections.Generic;
using Config;
using GameData.Domains.Item.Display;

namespace Game.Views.Make
{
	// Token: 0x02000955 RID: 2389
	public static class MakeSubPageMakeHelper
	{
		// Token: 0x06007167 RID: 29031 RVA: 0x0034A7A0 File Offset: 0x003489A0
		public static bool CheckIsRandomMake(ITradeableContent data)
		{
			bool flag = data == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool isInvalid = data.Key.ItemType == -1;
				bool hasTempId = data.Key.TemplateId >= 0;
				bool flag2 = isInvalid && hasTempId;
				result = flag2;
			}
			return result;
		}

		// Token: 0x06007168 RID: 29032 RVA: 0x0034A7F0 File Offset: 0x003489F0
		public static string GetRandomMakeIcon(short tempId)
		{
			if (tempId <= 300)
			{
				if (tempId <= 103)
				{
					switch (tempId)
					{
					case 0:
						return "ui9_icon_maketype_needlebox";
					case 1:
						return "ui9_icon_maketype_doubledaggers";
					case 2:
						return "ui9_icon_maketype_hidden";
					case 3:
						return "ui9_icon_maketype_flute";
					case 4:
						return "ui9_icon_maketype_gloves";
					case 5:
						return "ui9_icon_maketype_pestle";
					case 6:
						return "ui9_icon_maketype_whisk";
					case 7:
						return "ui9_icon_maketype_whip";
					case 8:
						return "ui9_icon_maketype_sword";
					case 9:
						return "ui9_icon_maketype_blade";
					case 10:
						return "ui9_icon_maketype_polearm";
					case 11:
						return "ui9_icon_maketype_zither";
					case 12:
						return "ui9_icon_maketype_mechanicweapon";
					case 13:
						return "ui9_icon_maketype_magicsymbol";
					case 14:
						return "ui9_icon_maketype_poisonweaponcream";
					case 15:
						return "ui9_icon_maketype_poisonweaponsand";
					default:
						switch (tempId)
						{
						case 100:
							return "ui9_icon_maketype_helm";
						case 101:
							return "ui9_icon_maketype_torsoarmor";
						case 102:
							return "ui9_icon_maketype_bracers";
						case 103:
							return "ui9_icon_maketype_boots";
						}
						break;
					}
				}
				else
				{
					if (tempId == 200)
					{
						return "ui9_icon_maketype_accessory";
					}
					if (tempId == 201)
					{
						return "ui9_icon_maketype_pocket";
					}
					if (tempId == 300)
					{
						return "ui9_icon_maketype_clothing";
					}
				}
			}
			else if (tempId <= 701)
			{
				if (tempId == 400)
				{
					return "ui9_icon_maketype_carrier";
				}
				if (tempId - 700 > 1)
				{
				}
			}
			else
			{
				if (tempId == 800)
				{
					return "ui9_icon_maketype_medicine";
				}
				if (tempId == 801)
				{
					return "ui9_icon_maketype_poison";
				}
				if (tempId == 1206)
				{
					return "ui9_icon_maketype_rope";
				}
			}
			return "ui9_icon_maketype_all";
		}

		// Token: 0x06007169 RID: 29033 RVA: 0x0034A9FC File Offset: 0x00348BFC
		public static string GetRandomMakeTypeName(short tempId)
		{
			if (tempId <= 300)
			{
				if (tempId <= 103)
				{
					switch (tempId)
					{
					case 0:
						return LanguageKey.LK_ItemSubType_0.Tr();
					case 1:
						return LanguageKey.LK_ItemSubType_1.Tr();
					case 2:
						return LanguageKey.LK_ItemSubType_2.Tr();
					case 3:
						return LanguageKey.LK_ItemSubType_3.Tr();
					case 4:
						return LanguageKey.LK_ItemSubType_4.Tr();
					case 5:
						return LanguageKey.LK_ItemSubType_5.Tr();
					case 6:
						return LanguageKey.LK_ItemSubType_6.Tr();
					case 7:
						return LanguageKey.LK_ItemSubType_7.Tr();
					case 8:
						return LanguageKey.LK_ItemSubType_8.Tr();
					case 9:
						return LanguageKey.LK_ItemSubType_9.Tr();
					case 10:
						return LanguageKey.LK_ItemSubType_10.Tr();
					case 11:
						return LanguageKey.LK_ItemSubType_11.Tr();
					case 12:
						return LanguageKey.LK_ItemSubType_12.Tr();
					case 13:
						return LanguageKey.LK_ItemSubType_13.Tr();
					case 14:
						return LanguageKey.LK_ItemSubType_14.Tr();
					case 15:
						return LanguageKey.LK_ItemSubType_15.Tr();
					default:
						switch (tempId)
						{
						case 100:
							return LanguageKey.LK_ItemSubType_100.Tr();
						case 101:
							return LanguageKey.LK_ItemSubType_101.Tr();
						case 102:
							return LanguageKey.LK_ItemSubType_102.Tr();
						case 103:
							return LanguageKey.LK_ItemSubType_103.Tr();
						}
						break;
					}
				}
				else
				{
					if (tempId == 200)
					{
						return LanguageKey.LK_ItemSubType_200.Tr();
					}
					if (tempId == 201)
					{
						return LanguageKey.LK_ItemSubType_201.Tr();
					}
					if (tempId == 300)
					{
						return LanguageKey.LK_ItemSubType_300.Tr();
					}
				}
			}
			else if (tempId <= 701)
			{
				if (tempId == 400)
				{
					return LanguageKey.LK_ItemSubType_400.Tr();
				}
				if (tempId == 700)
				{
					return LanguageKey.LK_ItemSubType_700.Tr();
				}
				if (tempId == 701)
				{
					return LanguageKey.LK_ItemSubType_701.Tr();
				}
			}
			else
			{
				if (tempId == 800)
				{
					return LanguageKey.LK_ItemSubType_800.Tr();
				}
				if (tempId == 801)
				{
					return LanguageKey.LK_ItemSubType_801.Tr();
				}
				if (tempId == 1206)
				{
					return LanguageKey.LK_ItemSubType_1206.Tr();
				}
			}
			return "";
		}

		// Token: 0x0600716A RID: 29034 RVA: 0x0034ACC8 File Offset: 0x00348EC8
		public static bool CheckCanMakeTargetRandomType(short itemSubType, ItemDisplayData materialData)
		{
			bool result = false;
			List<short> craftableItemTypes = Material.Instance[materialData.RealKey.TemplateId].CraftableItemTypes;
			foreach (short id in craftableItemTypes)
			{
				MakeItemTypeItem item = MakeItemType.Instance[id];
				bool flag = item.ItemSubType != itemSubType;
				if (!flag)
				{
					result = true;
					break;
				}
			}
			return result;
		}
	}
}
