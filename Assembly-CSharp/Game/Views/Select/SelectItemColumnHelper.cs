using System;
using System.Collections;
using System.Collections.Generic;
using Config;
using Game.Components.ListStyleGeneralScroll;
using Game.Components.ListStyleGeneralScroll.CellContent;
using GameData.Domains.Character.AvatarSystem;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Utilities;
using GameDataExtensions;
using UnityEngine;

namespace Game.Views.Select
{
	// Token: 0x020007A3 RID: 1955
	public static class SelectItemColumnHelper
	{
		// Token: 0x06005E67 RID: 24167 RVA: 0x002B5CDC File Offset: 0x002B3EDC
		public static IEnumerable<ColumnDefinition> GetColumnDefinitions(ESelectItemFilterType filterType)
		{
			yield return SelectItemColumnHelper.CreateIconAndNameColumn();
			switch (filterType)
			{
			case ESelectItemFilterType.All:
				yield return SelectItemColumnHelper.CreateAmountColumn();
				yield return SelectItemColumnHelper.CreateTypeColumn();
				yield return SelectItemColumnHelper.CreateValueColumn();
				yield return SelectItemColumnHelper.CreateWeightColumn();
				yield return SelectItemColumnHelper.CreateDurabilityColumn();
				break;
			case ESelectItemFilterType.Food:
			case ESelectItemFilterType.Medicine:
				yield return SelectItemColumnHelper.CreateAmountColumn();
				yield return SelectItemColumnHelper.CreateTypeColumn();
				yield return SelectItemColumnHelper.CreateValueColumn();
				yield return SelectItemColumnHelper.CreateWeightColumn();
				break;
			case ESelectItemFilterType.Equipment:
				yield return SelectItemColumnHelper.CreateValueColumn();
				yield return SelectItemColumnHelper.CreateWeightColumn();
				yield return SelectItemColumnHelper.CreateDurabilityColumn();
				yield return SelectItemColumnHelper.CreatePowerColumn();
				break;
			case ESelectItemFilterType.EquipmentWeapon:
				yield return SelectItemColumnHelper.CreateValueColumn();
				yield return SelectItemColumnHelper.CreateWeightColumn();
				yield return SelectItemColumnHelper.CreateDurabilityColumn();
				yield return SelectItemColumnHelper.CreatePenetrationColumn();
				yield return SelectItemColumnHelper.CreateToughnessColumn();
				break;
			case ESelectItemFilterType.EquipmentArmor:
				yield return SelectItemColumnHelper.CreateValueColumn();
				yield return SelectItemColumnHelper.CreateWeightColumn();
				yield return SelectItemColumnHelper.CreateDurabilityColumn();
				yield return SelectItemColumnHelper.CreateArmorBreakColumn();
				yield return SelectItemColumnHelper.CreateToughnessColumn();
				break;
			case ESelectItemFilterType.EquipmentAccessory:
				yield return SelectItemColumnHelper.CreateTypeColumn();
				yield return SelectItemColumnHelper.CreateValueColumn();
				yield return SelectItemColumnHelper.CreateWeightColumn();
				yield return SelectItemColumnHelper.CreateAccessoryEffectColumn();
				break;
			case ESelectItemFilterType.EquipmentClothing:
				yield return SelectItemColumnHelper.CreateTypeColumn();
				yield return SelectItemColumnHelper.CreateValueColumn();
				yield return SelectItemColumnHelper.CreateWeightColumn();
				yield return SelectItemColumnHelper.CreateCharmColumn();
				break;
			case ESelectItemFilterType.EquipmentCarrier:
				yield return SelectItemColumnHelper.CreateValueColumn();
				yield return SelectItemColumnHelper.CreateWeightColumn();
				yield return SelectItemColumnHelper.CreateDurabilityColumn();
				yield return SelectItemColumnHelper.CreateTravelTimeReductionColumn();
				yield return SelectItemColumnHelper.CreateInventoryBonusColumn();
				break;
			case ESelectItemFilterType.Book:
				yield return SelectItemColumnHelper.CreateValueColumn();
				yield return SelectItemColumnHelper.CreateWeightColumn();
				yield return SelectItemColumnHelper.CreateDurabilityColumn();
				yield return SelectItemColumnHelper.CreateBookReadingInfoColumn();
				break;
			case ESelectItemFilterType.Tool:
				yield return SelectItemColumnHelper.CreateValueColumn();
				yield return SelectItemColumnHelper.CreateWeightColumn();
				yield return SelectItemColumnHelper.CreateDurabilityColumn();
				yield return SelectItemColumnHelper.CreateToolEffectColumn();
				break;
			case ESelectItemFilterType.Material:
				yield return SelectItemColumnHelper.CreateAmountColumn();
				yield return SelectItemColumnHelper.CreateValueColumn();
				yield return SelectItemColumnHelper.CreateWeightColumn();
				yield return SelectItemColumnHelper.CreateRefiningEffectColumn();
				break;
			case ESelectItemFilterType.Misc:
				yield return SelectItemColumnHelper.CreateAmountColumn();
				yield return SelectItemColumnHelper.CreateTypeColumn();
				yield return SelectItemColumnHelper.CreateValueColumn();
				yield return SelectItemColumnHelper.CreateWeightColumn();
				break;
			case ESelectItemFilterType.MiscCricket:
				yield return SelectItemColumnHelper.CreateCricketAgeColumn();
				yield return SelectItemColumnHelper.CreateCricketDurabilityColumn();
				yield return SelectItemColumnHelper.CreateCricketWinsColumn();
				yield return SelectItemColumnHelper.CreateCricketLossesColumn();
				break;
			default:
				yield return SelectItemColumnHelper.CreateAmountColumn();
				yield return SelectItemColumnHelper.CreateTypeColumn();
				yield return SelectItemColumnHelper.CreateValueColumn();
				yield return SelectItemColumnHelper.CreateWeightColumn();
				break;
			}
			yield break;
		}

		// Token: 0x06005E68 RID: 24168 RVA: 0x002B5CEC File Offset: 0x002B3EEC
		public static IEnumerable<ColumnDefinition> GetColumnDefinitionsByFlags(ESelectItemColumnType flags)
		{
			foreach (object obj in Enum.GetValues(typeof(ESelectItemColumnType)))
			{
				ESelectItemColumnType flag = (ESelectItemColumnType)obj;
				bool flag2 = flag == ESelectItemColumnType.None || (flags & flag) == ESelectItemColumnType.None;
				if (!flag2)
				{
					ColumnDefinition column = SelectItemColumnHelper.CreateColumnByType(flag);
					bool flag3 = column != null;
					if (flag3)
					{
						yield return column;
					}
					column = null;
				}
			}
			IEnumerator enumerator = null;
			yield break;
			yield break;
		}

		// Token: 0x06005E69 RID: 24169 RVA: 0x002B5CFC File Offset: 0x002B3EFC
		public static ColumnDefinition CreateColumnByType(ESelectItemColumnType columnType)
		{
			if (!true)
			{
			}
			ColumnDefinition result;
			if (columnType <= ESelectItemColumnType.ToolEffect)
			{
				if (columnType <= ESelectItemColumnType.ArmorBreak)
				{
					if (columnType <= ESelectItemColumnType.Weight)
					{
						switch (columnType)
						{
						case ESelectItemColumnType.IconAndName:
							result = SelectItemColumnHelper.CreateIconAndNameColumn();
							goto IL_308;
						case ESelectItemColumnType.Amount:
							result = SelectItemColumnHelper.CreateAmountColumn();
							goto IL_308;
						case ESelectItemColumnType.IconAndName | ESelectItemColumnType.Amount:
							break;
						case ESelectItemColumnType.Type:
							result = SelectItemColumnHelper.CreateTypeColumn();
							goto IL_308;
						default:
							if (columnType == ESelectItemColumnType.Value)
							{
								result = SelectItemColumnHelper.CreateValueColumn();
								goto IL_308;
							}
							if (columnType == ESelectItemColumnType.Weight)
							{
								result = SelectItemColumnHelper.CreateWeightColumn();
								goto IL_308;
							}
							break;
						}
					}
					else if (columnType <= ESelectItemColumnType.Power)
					{
						if (columnType == ESelectItemColumnType.Durability)
						{
							result = SelectItemColumnHelper.CreateDurabilityColumn();
							goto IL_308;
						}
						if (columnType == ESelectItemColumnType.Power)
						{
							result = SelectItemColumnHelper.CreatePowerColumn();
							goto IL_308;
						}
					}
					else
					{
						if (columnType == ESelectItemColumnType.Penetration)
						{
							result = SelectItemColumnHelper.CreatePenetrationColumn();
							goto IL_308;
						}
						if (columnType == ESelectItemColumnType.ArmorBreak)
						{
							result = SelectItemColumnHelper.CreateArmorBreakColumn();
							goto IL_308;
						}
					}
				}
				else if (columnType <= ESelectItemColumnType.Charm)
				{
					if (columnType == ESelectItemColumnType.Toughness)
					{
						result = SelectItemColumnHelper.CreateToughnessColumn();
						goto IL_308;
					}
					if (columnType == ESelectItemColumnType.AccessoryEffect)
					{
						result = SelectItemColumnHelper.CreateAccessoryEffectColumn();
						goto IL_308;
					}
					if (columnType == ESelectItemColumnType.Charm)
					{
						result = SelectItemColumnHelper.CreateCharmColumn();
						goto IL_308;
					}
				}
				else if (columnType <= ESelectItemColumnType.InventoryBonus)
				{
					if (columnType == ESelectItemColumnType.TravelTimeReduction)
					{
						result = SelectItemColumnHelper.CreateTravelTimeReductionColumn();
						goto IL_308;
					}
					if (columnType == ESelectItemColumnType.InventoryBonus)
					{
						result = SelectItemColumnHelper.CreateInventoryBonusColumn();
						goto IL_308;
					}
				}
				else
				{
					if (columnType == ESelectItemColumnType.BookReadingInfo)
					{
						result = SelectItemColumnHelper.CreateBookReadingInfoColumn();
						goto IL_308;
					}
					if (columnType == ESelectItemColumnType.ToolEffect)
					{
						result = SelectItemColumnHelper.CreateToolEffectColumn();
						goto IL_308;
					}
				}
			}
			else if (columnType <= ESelectItemColumnType.CricketSp)
			{
				if (columnType <= ESelectItemColumnType.CricketDurability)
				{
					if (columnType == ESelectItemColumnType.RefiningEffect)
					{
						result = SelectItemColumnHelper.CreateRefiningEffectColumn();
						goto IL_308;
					}
					if (columnType == ESelectItemColumnType.CricketAge)
					{
						result = SelectItemColumnHelper.CreateCricketAgeColumn();
						goto IL_308;
					}
					if (columnType == ESelectItemColumnType.CricketDurability)
					{
						result = SelectItemColumnHelper.CreateCricketDurabilityColumn();
						goto IL_308;
					}
				}
				else if (columnType <= ESelectItemColumnType.CricketLosses)
				{
					if (columnType == ESelectItemColumnType.CricketWins)
					{
						result = SelectItemColumnHelper.CreateCricketWinsColumn();
						goto IL_308;
					}
					if (columnType == ESelectItemColumnType.CricketLosses)
					{
						result = SelectItemColumnHelper.CreateCricketLossesColumn();
						goto IL_308;
					}
				}
				else
				{
					if (columnType == ESelectItemColumnType.CricketHp)
					{
						result = SelectItemColumnHelper.CreateCricketHpColumn();
						goto IL_308;
					}
					if (columnType == ESelectItemColumnType.CricketSp)
					{
						result = SelectItemColumnHelper.CreateCricketSpColumn();
						goto IL_308;
					}
				}
			}
			else if (columnType <= ESelectItemColumnType.CricketBite)
			{
				if (columnType == ESelectItemColumnType.CricketVigor)
				{
					result = SelectItemColumnHelper.CreateCricketVigorColumn();
					goto IL_308;
				}
				if (columnType == ESelectItemColumnType.CricketStrength)
				{
					result = SelectItemColumnHelper.CreateCricketStrengthColumn();
					goto IL_308;
				}
				if (columnType == ESelectItemColumnType.CricketBite)
				{
					result = SelectItemColumnHelper.CreateCricketBiteColumn();
					goto IL_308;
				}
			}
			else if (columnType <= ESelectItemColumnType.CombatSkillType)
			{
				if (columnType == ESelectItemColumnType.EscapeRate)
				{
					result = SelectItemColumnHelper.CreateEscapeRateColumn();
					goto IL_308;
				}
				if (columnType == ESelectItemColumnType.CombatSkillType)
				{
					result = SelectItemColumnHelper.CreateCombatSkillTypeColumn();
					goto IL_308;
				}
			}
			else
			{
				if (columnType == ESelectItemColumnType.DishDurability)
				{
					result = SelectItemColumnHelper.CreateDishDurabilityColumn();
					goto IL_308;
				}
				if (columnType == ESelectItemColumnType.AmountWithSelected)
				{
					result = SelectItemColumnHelper.CreateAmountWithSelectedColumn();
					goto IL_308;
				}
			}
			result = null;
			IL_308:
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x06005E6A RID: 24170 RVA: 0x002B601C File Offset: 0x002B421C
		public static ColumnDefinition CreateIconAndNameColumn()
		{
			ColumnDefinition<ITradeableContent, ITradeableContent> columnDefinition = new ColumnDefinition<ITradeableContent, ITradeableContent>();
			columnDefinition.LayoutOption = new LayoutOption
			{
				MinWidth = 200f,
				FlexibleWidth = 1f,
				PreferredWidth = 400f,
				Priority = 1
			};
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_Item.Tr());
			columnDefinition.CellDataGenerator = ((ITradeableContent data) => data);
			columnDefinition.SortId = 0;
			return columnDefinition;
		}

		// Token: 0x06005E6B RID: 24171 RVA: 0x002B60C0 File Offset: 0x002B42C0
		public static ColumnDefinition CreateAmountColumn()
		{
			ColumnDefinition<ITradeableContent, string> columnDefinition = new ColumnDefinition<ITradeableContent, string>();
			columnDefinition.LayoutOption = new LayoutOption
			{
				MinWidth = 60f,
				FlexibleWidth = 1f,
				PreferredWidth = 80f,
				Priority = 1
			};
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_Count.Tr());
			columnDefinition.CellDataGenerator = ((ITradeableContent data) => data.Amount.ToString());
			columnDefinition.SortId = 17;
			return columnDefinition;
		}

		// Token: 0x06005E6C RID: 24172 RVA: 0x002B6164 File Offset: 0x002B4364
		public static ColumnDefinition CreateAmountWithSelectedColumn()
		{
			ColumnDefinition<ITradeableContent, string> columnDefinition = new ColumnDefinition<ITradeableContent, string>();
			columnDefinition.LayoutOption = new LayoutOption
			{
				MinWidth = 60f,
				FlexibleWidth = 1f,
				PreferredWidth = 80f,
				Priority = 1
			};
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_Count.Tr());
			columnDefinition.CellDataGenerator = ((ITradeableContent data) => data.Amount.ToString());
			columnDefinition.SortId = 17;
			return columnDefinition;
		}

		// Token: 0x06005E6D RID: 24173 RVA: 0x002B6208 File Offset: 0x002B4408
		public static ColumnDefinition CreateTypeColumn()
		{
			ColumnDefinition<ITradeableContent, string> columnDefinition = new ColumnDefinition<ITradeableContent, string>();
			columnDefinition.LayoutOption = new LayoutOption
			{
				MinWidth = 80f,
				FlexibleWidth = 1f,
				PreferredWidth = 120f,
				Priority = 1
			};
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_Type.Tr());
			columnDefinition.CellDataGenerator = new Func<ITradeableContent, string>(SelectItemColumnHelper.GetItemTypeString);
			columnDefinition.SortId = 56;
			return columnDefinition;
		}

		// Token: 0x06005E6E RID: 24174 RVA: 0x002B629C File Offset: 0x002B449C
		public static ColumnDefinition CreateValueColumn()
		{
			ColumnDefinition<ITradeableContent, string> columnDefinition = new ColumnDefinition<ITradeableContent, string>();
			columnDefinition.LayoutOption = new LayoutOption
			{
				MinWidth = 60f,
				FlexibleWidth = 1f,
				PreferredWidth = 100f,
				Priority = 1
			};
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_ItemValue.Tr());
			columnDefinition.CellDataGenerator = ((ITradeableContent data) => (data.GetContentType() == 4) ? "-" : data.Value.ToString());
			columnDefinition.SortId = 5;
			return columnDefinition;
		}

		// Token: 0x06005E6F RID: 24175 RVA: 0x002B6340 File Offset: 0x002B4540
		public static ColumnDefinition CreateWeightColumn()
		{
			ColumnDefinition<ITradeableContent, string> columnDefinition = new ColumnDefinition<ITradeableContent, string>();
			columnDefinition.LayoutOption = new LayoutOption
			{
				MinWidth = 60f,
				FlexibleWidth = 1f,
				PreferredWidth = 100f,
				Priority = 1
			};
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_Weight.Tr());
			columnDefinition.CellDataGenerator = ((ITradeableContent data) => (data.GetContentType() > 0 || data.IsSpecialInteract) ? "-" : NumberFormatUtils.FormatItemWeight(data.Weight));
			columnDefinition.SortId = 6;
			return columnDefinition;
		}

		// Token: 0x06005E70 RID: 24176 RVA: 0x002B63E4 File Offset: 0x002B45E4
		public static ColumnDefinition CreateDurabilityColumn()
		{
			ColumnDefinition<ITradeableContent, string> columnDefinition = new ColumnDefinition<ITradeableContent, string>();
			columnDefinition.LayoutOption = new LayoutOption
			{
				MinWidth = 60f,
				FlexibleWidth = 1f,
				PreferredWidth = 100f,
				Priority = 1
			};
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_Durability.Tr());
			columnDefinition.CellDataGenerator = delegate(ITradeableContent data)
			{
				bool flag = data.GetContentType() > 0 || data.IsSpecialInteract || data.MaxDurability == 0;
				string result;
				if (flag)
				{
					result = "-";
				}
				else
				{
					string color = (data.Durability * 2 < data.MaxDurability) ? "brightred" : "brightyellow";
					string durabilityStr = data.Durability.ToString().SetColor(color);
					result = string.Format("{0}/{1}", durabilityStr, data.MaxDurability);
				}
				return result;
			};
			columnDefinition.SortId = 18;
			return columnDefinition;
		}

		// Token: 0x06005E71 RID: 24177 RVA: 0x002B6488 File Offset: 0x002B4688
		public static ColumnDefinition CreatePowerColumn()
		{
			ColumnDefinition<ITradeableContent, string> columnDefinition = new ColumnDefinition<ITradeableContent, string>();
			columnDefinition.LayoutOption = new LayoutOption
			{
				MinWidth = 60f,
				FlexibleWidth = 1f,
				PreferredWidth = 100f,
				Priority = 1
			};
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_SelectItem_Column_Power.Tr());
			columnDefinition.CellDataGenerator = delegate(ITradeableContent data)
			{
				string result;
				if (data.GetContentType() <= 0)
				{
					ItemDisplayData itemData = data as ItemDisplayData;
					result = ((itemData != null && itemData.ShouldShowPower()) ? string.Format("{0}%", data.PowerInfo.Power) : (data.PowerInfo.AnyValue ? string.Format("{0}%", data.PowerInfo.Power) : "-"));
				}
				else
				{
					result = "-";
				}
				return result;
			};
			columnDefinition.SortId = -1;
			return columnDefinition;
		}

		// Token: 0x06005E72 RID: 24178 RVA: 0x002B652C File Offset: 0x002B472C
		public static ColumnDefinition CreatePenetrationColumn()
		{
			ColumnDefinition<ITradeableContent, string> columnDefinition = new ColumnDefinition<ITradeableContent, string>();
			columnDefinition.LayoutOption = new LayoutOption
			{
				MinWidth = 60f,
				FlexibleWidth = 1f,
				PreferredWidth = 100f,
				Priority = 1
			};
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_SelectItem_Column_Penetration.Tr());
			columnDefinition.CellDataGenerator = ((ITradeableContent data) => string.Format("{0:f1}", (float)data.EquipmentAttack / 100f));
			columnDefinition.SortId = -1;
			return columnDefinition;
		}

		// Token: 0x06005E73 RID: 24179 RVA: 0x002B65D0 File Offset: 0x002B47D0
		public static ColumnDefinition CreateArmorBreakColumn()
		{
			ColumnDefinition<ITradeableContent, string> columnDefinition = new ColumnDefinition<ITradeableContent, string>();
			columnDefinition.LayoutOption = new LayoutOption
			{
				MinWidth = 60f,
				FlexibleWidth = 1f,
				PreferredWidth = 100f,
				Priority = 1
			};
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_SelectItem_Column_ArmorBreak.Tr());
			columnDefinition.CellDataGenerator = ((ITradeableContent data) => string.Format("{0:f2}", (float)data.EquipmentAttack / 100f));
			columnDefinition.SortId = -1;
			return columnDefinition;
		}

		// Token: 0x06005E74 RID: 24180 RVA: 0x002B6674 File Offset: 0x002B4874
		public static ColumnDefinition CreateToughnessColumn()
		{
			ColumnDefinition<ITradeableContent, string> columnDefinition = new ColumnDefinition<ITradeableContent, string>();
			columnDefinition.LayoutOption = new LayoutOption
			{
				MinWidth = 60f,
				FlexibleWidth = 1f,
				PreferredWidth = 100f,
				Priority = 1
			};
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_SelectItem_Column_Toughness.Tr());
			columnDefinition.CellDataGenerator = ((ITradeableContent data) => string.Format("{0:f2}", (float)data.EquipmentDefense / 100f));
			columnDefinition.SortId = -1;
			return columnDefinition;
		}

		// Token: 0x06005E75 RID: 24181 RVA: 0x002B6718 File Offset: 0x002B4918
		public static ColumnDefinition CreateAccessoryEffectColumn()
		{
			ColumnDefinition<ITradeableContent, string> columnDefinition = new ColumnDefinition<ITradeableContent, string>();
			columnDefinition.LayoutOption = new LayoutOption
			{
				MinWidth = 80f,
				FlexibleWidth = 1f,
				PreferredWidth = 170f,
				Priority = 1
			};
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_SelectItem_Column_AccessoryEffect.Tr());
			columnDefinition.CellDataGenerator = new Func<ITradeableContent, string>(SelectItemColumnHelper.GetAccessoryEffectString);
			columnDefinition.SortId = -1;
			return columnDefinition;
		}

		// Token: 0x06005E76 RID: 24182 RVA: 0x002B67AC File Offset: 0x002B49AC
		public static ColumnDefinition CreateCharmColumn()
		{
			ColumnDefinition<ITradeableContent, string> columnDefinition = new ColumnDefinition<ITradeableContent, string>();
			columnDefinition.LayoutOption = new LayoutOption
			{
				MinWidth = 60f,
				FlexibleWidth = 1f,
				PreferredWidth = 100f,
				Priority = 1
			};
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_Main_SummaryInfo_Charm.Tr());
			columnDefinition.CellDataGenerator = new Func<ITradeableContent, string>(SelectItemColumnHelper.GetClothingCharmString);
			columnDefinition.SortId = -1;
			return columnDefinition;
		}

		// Token: 0x06005E77 RID: 24183 RVA: 0x002B6840 File Offset: 0x002B4A40
		public static ColumnDefinition CreateTravelTimeReductionColumn()
		{
			ColumnDefinition<ITradeableContent, string> columnDefinition = new ColumnDefinition<ITradeableContent, string>();
			columnDefinition.LayoutOption = new LayoutOption
			{
				MinWidth = 60f,
				FlexibleWidth = 1f,
				PreferredWidth = 100f,
				Priority = 1
			};
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_SelectItem_Column_TravelTimeReduction.Tr());
			columnDefinition.CellDataGenerator = ((ITradeableContent data) => (data.TravelTimeReduction > 0) ? string.Format("-{0}%", data.TravelTimeReduction) : "-");
			columnDefinition.SortId = -1;
			return columnDefinition;
		}

		// Token: 0x06005E78 RID: 24184 RVA: 0x002B68E4 File Offset: 0x002B4AE4
		public static ColumnDefinition CreateInventoryBonusColumn()
		{
			ColumnDefinition<ITradeableContent, string> columnDefinition = new ColumnDefinition<ITradeableContent, string>();
			columnDefinition.LayoutOption = new LayoutOption
			{
				MinWidth = 60f,
				FlexibleWidth = 1f,
				PreferredWidth = 100f,
				Priority = 1
			};
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_SelectItem_Column_InventoryBonus.Tr());
			columnDefinition.CellDataGenerator = ((ITradeableContent data) => (data.MaxInventoryLoadBonus > 0) ? string.Format("+{0:f1}", (float)data.MaxInventoryLoadBonus / 100f) : "-");
			columnDefinition.SortId = -1;
			return columnDefinition;
		}

		// Token: 0x06005E79 RID: 24185 RVA: 0x002B6988 File Offset: 0x002B4B88
		public static ColumnDefinition CreateBookReadingInfoColumn()
		{
			ColumnDefinition<ITradeableContent, BookPageInfoData> columnDefinition = new ColumnDefinition<ITradeableContent, BookPageInfoData>();
			columnDefinition.LayoutOption = new LayoutOption
			{
				MinWidth = 300f,
				FlexibleWidth = 1f,
				PreferredWidth = 400f,
				Priority = 1
			};
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_SelectItem_Column_BookReadingInfo.Tr());
			columnDefinition.CellDataGenerator = ((ITradeableContent data) => new BookPageInfoData(data));
			columnDefinition.SortId = -1;
			return columnDefinition;
		}

		// Token: 0x06005E7A RID: 24186 RVA: 0x002B6A2C File Offset: 0x002B4C2C
		public static ColumnDefinition CreateToolEffectColumn()
		{
			ColumnDefinition<ITradeableContent, string> columnDefinition = new ColumnDefinition<ITradeableContent, string>();
			columnDefinition.LayoutOption = new LayoutOption
			{
				MinWidth = 80f,
				FlexibleWidth = 1f,
				PreferredWidth = 150f,
				Priority = 1
			};
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_SelectItem_Column_ToolEffect.Tr());
			columnDefinition.CellDataGenerator = new Func<ITradeableContent, string>(SelectItemColumnHelper.GetToolEffectString);
			columnDefinition.SortId = -1;
			return columnDefinition;
		}

		// Token: 0x06005E7B RID: 24187 RVA: 0x002B6AC0 File Offset: 0x002B4CC0
		public static ColumnDefinition CreateRefiningEffectColumn()
		{
			ColumnDefinition<ITradeableContent, string> columnDefinition = new ColumnDefinition<ITradeableContent, string>();
			columnDefinition.LayoutOption = new LayoutOption
			{
				MinWidth = 300f,
				FlexibleWidth = 1f,
				PreferredWidth = 500f,
				Priority = 1
			};
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_SelectItem_Column_RefiningEffect.Tr());
			columnDefinition.CellDataGenerator = new Func<ITradeableContent, string>(SelectItemColumnHelper.GetRefiningEffectString);
			columnDefinition.SortId = -1;
			return columnDefinition;
		}

		// Token: 0x06005E7C RID: 24188 RVA: 0x002B6B54 File Offset: 0x002B4D54
		public static ColumnDefinition CreateCricketAgeColumn()
		{
			ColumnDefinition<ITradeableContent, string> columnDefinition = new ColumnDefinition<ITradeableContent, string>();
			columnDefinition.LayoutOption = new LayoutOption
			{
				MinWidth = 60f,
				FlexibleWidth = 1f,
				PreferredWidth = 100f,
				Priority = 1
			};
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_SelectItem_Column_CricketAge.Tr());
			columnDefinition.CellDataGenerator = ((ITradeableContent data) => (data.CricketData != null) ? string.Format("{0}/{1}", data.CricketData.AgeStr, data.CricketData.MaxAge) : "-");
			columnDefinition.SortId = 43;
			return columnDefinition;
		}

		// Token: 0x06005E7D RID: 24189 RVA: 0x002B6BF8 File Offset: 0x002B4DF8
		public static ColumnDefinition CreateCricketDurabilityColumn()
		{
			ColumnDefinition<ITradeableContent, string> columnDefinition = new ColumnDefinition<ITradeableContent, string>();
			columnDefinition.LayoutOption = new LayoutOption
			{
				MinWidth = 60f,
				FlexibleWidth = 1f,
				PreferredWidth = 100f,
				Priority = 1
			};
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_Durability.Tr());
			columnDefinition.CellDataGenerator = ((ITradeableContent data) => string.Format("{0}/{1}", data.Durability, data.MaxDurability));
			columnDefinition.SortId = 42;
			return columnDefinition;
		}

		// Token: 0x06005E7E RID: 24190 RVA: 0x002B6C9C File Offset: 0x002B4E9C
		public static ColumnDefinition CreateCricketWinsColumn()
		{
			ColumnDefinition<ITradeableContent, string> columnDefinition = new ColumnDefinition<ITradeableContent, string>();
			columnDefinition.LayoutOption = new LayoutOption
			{
				MinWidth = 60f,
				FlexibleWidth = 1f,
				PreferredWidth = 80f,
				Priority = 1
			};
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_SelectItem_Column_CricketWins.Tr());
			columnDefinition.CellDataGenerator = delegate(ITradeableContent data)
			{
				CricketData cricketData = data.CricketData;
				return ((cricketData != null) ? cricketData.WinsCount.ToString() : null) ?? "-";
			};
			columnDefinition.SortId = 44;
			return columnDefinition;
		}

		// Token: 0x06005E7F RID: 24191 RVA: 0x002B6D40 File Offset: 0x002B4F40
		public static ColumnDefinition CreateCricketLossesColumn()
		{
			ColumnDefinition<ITradeableContent, string> columnDefinition = new ColumnDefinition<ITradeableContent, string>();
			columnDefinition.LayoutOption = new LayoutOption
			{
				MinWidth = 60f,
				FlexibleWidth = 1f,
				PreferredWidth = 80f,
				Priority = 1
			};
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_SelectItem_Column_CricketLosses.Tr());
			columnDefinition.CellDataGenerator = delegate(ITradeableContent data)
			{
				CricketData cricketData = data.CricketData;
				return ((cricketData != null) ? cricketData.LossesCount.ToString() : null) ?? "-";
			};
			columnDefinition.SortId = 45;
			return columnDefinition;
		}

		// Token: 0x06005E80 RID: 24192 RVA: 0x002B6DE4 File Offset: 0x002B4FE4
		public static ColumnDefinition CreateCricketHpColumn()
		{
			return SelectItemColumnHelper.CreateCricketPropertyColumn(() => LanguageKey.LK_Cricket_Property_Hp.Tr(), new Func<ITradeableContent, int?>(SelectItemColumnHelper.GetCricketHp), 46);
		}

		// Token: 0x06005E81 RID: 24193 RVA: 0x002B6E28 File Offset: 0x002B5028
		public static ColumnDefinition CreateCricketSpColumn()
		{
			return SelectItemColumnHelper.CreateCricketPropertyColumn(() => LanguageKey.LK_Cricket_Property_Sp.Tr(), new Func<ITradeableContent, int?>(SelectItemColumnHelper.GetCricketSp), 47);
		}

		// Token: 0x06005E82 RID: 24194 RVA: 0x002B6E6C File Offset: 0x002B506C
		public static ColumnDefinition CreateCricketVigorColumn()
		{
			return SelectItemColumnHelper.CreateCricketPropertyColumn(() => LanguageKey.LK_Cricket_Property_Vigor.Tr(), new Func<ITradeableContent, int?>(SelectItemColumnHelper.GetCricketVigor), 48);
		}

		// Token: 0x06005E83 RID: 24195 RVA: 0x002B6EB0 File Offset: 0x002B50B0
		public static ColumnDefinition CreateCricketStrengthColumn()
		{
			return SelectItemColumnHelper.CreateCricketPropertyColumn(() => LanguageKey.LK_Cricket_Property_Strength.Tr(), new Func<ITradeableContent, int?>(SelectItemColumnHelper.GetCricketStrength), 49);
		}

		// Token: 0x06005E84 RID: 24196 RVA: 0x002B6EF4 File Offset: 0x002B50F4
		public static ColumnDefinition CreateCricketBiteColumn()
		{
			return SelectItemColumnHelper.CreateCricketPropertyColumn(() => LanguageKey.LK_Cricket_Property_Bite.Tr(), new Func<ITradeableContent, int?>(SelectItemColumnHelper.GetCricketBite), 50);
		}

		// Token: 0x06005E85 RID: 24197 RVA: 0x002B6F38 File Offset: 0x002B5138
		private static ColumnDefinition CreateCricketPropertyColumn(Func<string> headerGetter, Func<ITradeableContent, int?> valueGetter, short sortId)
		{
			return new ColumnDefinition<ITradeableContent, string>
			{
				LayoutOption = new LayoutOption
				{
					MinWidth = 60f,
					FlexibleWidth = 1f,
					PreferredWidth = 80f,
					Priority = 1
				},
				TableHeadLabel = headerGetter,
				CellDataGenerator = delegate(ITradeableContent data)
				{
					int? num;
					return ((valueGetter(data) != null) ? num.GetValueOrDefault().ToString() : null) ?? "-";
				},
				SortId = sortId
			};
		}

		// Token: 0x06005E86 RID: 24198 RVA: 0x002B6FB8 File Offset: 0x002B51B8
		private static int? GetCricketHp(ITradeableContent data)
		{
			return SelectItemColumnHelper.GetCricketPropertyValue(data, (CricketPartsItem colorConfig, CricketPartsItem partConfig, bool hasPart) => (int)(colorConfig.HP + (hasPart ? partConfig.HP : 0)), 0);
		}

		// Token: 0x06005E87 RID: 24199 RVA: 0x002B6FF0 File Offset: 0x002B51F0
		private static int? GetCricketSp(ITradeableContent data)
		{
			return SelectItemColumnHelper.GetCricketPropertyValue(data, (CricketPartsItem colorConfig, CricketPartsItem partConfig, bool hasPart) => (int)(colorConfig.SP + (hasPart ? partConfig.SP : 0)), 1);
		}

		// Token: 0x06005E88 RID: 24200 RVA: 0x002B7028 File Offset: 0x002B5228
		private static int? GetCricketVigor(ITradeableContent data)
		{
			return SelectItemColumnHelper.GetCricketPropertyValue(data, (CricketPartsItem colorConfig, CricketPartsItem partConfig, bool hasPart) => (int)(colorConfig.Vigor + (hasPart ? partConfig.Vigor : 0)), 2);
		}

		// Token: 0x06005E89 RID: 24201 RVA: 0x002B7060 File Offset: 0x002B5260
		private static int? GetCricketStrength(ITradeableContent data)
		{
			return SelectItemColumnHelper.GetCricketPropertyValue(data, (CricketPartsItem colorConfig, CricketPartsItem partConfig, bool hasPart) => (int)(colorConfig.Strength + (hasPart ? partConfig.Strength : 0)), 3);
		}

		// Token: 0x06005E8A RID: 24202 RVA: 0x002B7098 File Offset: 0x002B5298
		private static int? GetCricketBite(ITradeableContent data)
		{
			return SelectItemColumnHelper.GetCricketPropertyValue(data, (CricketPartsItem colorConfig, CricketPartsItem partConfig, bool hasPart) => (int)(colorConfig.Bite + (hasPart ? partConfig.Bite : 0)), 4);
		}

		// Token: 0x06005E8B RID: 24203 RVA: 0x002B70D0 File Offset: 0x002B52D0
		private static int? GetCricketPropertyValue(ITradeableContent data, Func<CricketPartsItem, CricketPartsItem, bool, int> maxValueGetter, int injuryIndex)
		{
			sbyte? b = (data != null) ? new sbyte?(data.Key.ItemType) : null;
			int? num = (b != null) ? new int?((int)b.GetValueOrDefault()) : null;
			int num2 = 11;
			bool flag = !(num.GetValueOrDefault() == num2 & num != null) || data.CricketData == null;
			int? result;
			if (flag)
			{
				result = null;
			}
			else
			{
				CricketPartsItem colorConfig = CricketParts.Instance[data.CricketColorId];
				CricketPartsItem partConfig = CricketParts.Instance[data.CricketPartId];
				bool hasPart = data.CricketPartId > 0;
				int maxValue = maxValueGetter(colorConfig, partConfig, hasPart);
				result = new int?(Mathf.Max(maxValue - (int)data.CricketData.Injuries[injuryIndex], 0));
			}
			return result;
		}

		// Token: 0x06005E8C RID: 24204 RVA: 0x002B71B4 File Offset: 0x002B53B4
		public static ColumnDefinition CreateEscapeRateColumn()
		{
			ColumnDefinition<ITradeableContent, string> columnDefinition = new ColumnDefinition<ITradeableContent, string>();
			columnDefinition.LayoutOption = new LayoutOption
			{
				MinWidth = 60f,
				FlexibleWidth = 1f,
				PreferredWidth = 100f,
				Priority = 1
			};
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_SelectItem_Title_Kidnap_EscapeRate.Tr());
			columnDefinition.CellDataGenerator = delegate(ITradeableContent data)
			{
				IItemConfig config = data.Key.GetConfig();
				MiscItem misc = config as MiscItem;
				return (misc != null) ? string.Format("-{0}%", misc.ReduceEscapeRate) : "-0%";
			};
			columnDefinition.SortId = -1;
			return columnDefinition;
		}

		// Token: 0x06005E8D RID: 24205 RVA: 0x002B7258 File Offset: 0x002B5458
		public static ColumnDefinition CreateCombatSkillTypeColumn()
		{
			ColumnDefinition<ITradeableContent, string> columnDefinition = new ColumnDefinition<ITradeableContent, string>();
			columnDefinition.LayoutOption = new LayoutOption
			{
				MinWidth = 80f,
				FlexibleWidth = 1f,
				PreferredWidth = 100f,
				Priority = 1
			};
			columnDefinition.TableHeadLabel = (() => SortItem.Instance[124].Names[0]);
			columnDefinition.CellDataGenerator = new Func<ITradeableContent, string>(SelectItemColumnHelper.GetCombatSkillTypeString);
			columnDefinition.SortId = 124;
			return columnDefinition;
		}

		// Token: 0x06005E8E RID: 24206 RVA: 0x002B72EC File Offset: 0x002B54EC
		public static ColumnDefinition CreateDishDurabilityColumn()
		{
			ColumnDefinition<ITradeableContent, string> columnDefinition = new ColumnDefinition<ITradeableContent, string>();
			columnDefinition.LayoutOption = new LayoutOption
			{
				MinWidth = 60f,
				FlexibleWidth = 1f,
				PreferredWidth = 100f,
				Priority = 1
			};
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_Building_Entertain_RemainCount_Title.Tr());
			columnDefinition.CellDataGenerator = ((ITradeableContent data) => string.Format("{0}/{1}", data.SpecialArg, GlobalConfig.Instance.FeastDurability));
			columnDefinition.SortId = 204;
			return columnDefinition;
		}

		// Token: 0x06005E8F RID: 24207 RVA: 0x002B7394 File Offset: 0x002B5594
		private static string GetItemTypeString(ITradeableContent data)
		{
			bool flag = data.GetContentType() == 1;
			string result;
			if (flag)
			{
				result = LanguageKey.LK_Exchange_Prisoner.Tr();
			}
			else
			{
				bool flag2 = data.GetContentType() > 0;
				if (flag2)
				{
					result = "-";
				}
				else
				{
					ItemKey itemKey = data.RealKey;
					sbyte itemType = itemKey.ItemType;
					short templateId = itemKey.TemplateId;
					string resourceTypeName = string.Empty;
					sbyte resourceType = -1;
					switch (itemType)
					{
					case 0:
						resourceType = Weapon.Instance[templateId].ResourceType;
						break;
					case 1:
						resourceType = Armor.Instance[templateId].ResourceType;
						break;
					case 2:
						resourceType = Accessory.Instance[templateId].ResourceType;
						break;
					}
					bool flag3 = resourceType >= 0;
					if (flag3)
					{
						resourceTypeName = ResourceType.Instance[resourceType].Name;
					}
					short subType = ItemTemplateHelper.GetItemSubType(itemType, templateId);
					string subTypeName = LocalStringManager.Get(string.Format("LK_ItemSubType_{0}", subType));
					result = (string.IsNullOrEmpty(resourceTypeName) ? subTypeName : (resourceTypeName + subTypeName));
				}
			}
			return result;
		}

		// Token: 0x06005E90 RID: 24208 RVA: 0x002B74B0 File Offset: 0x002B56B0
		private static string GetAccessoryEffectString(ITradeableContent data)
		{
			bool flag = data.RealKey.ItemType != 2;
			string result;
			if (flag)
			{
				result = "-";
			}
			else
			{
				AccessoryItem config = Accessory.Instance[data.RealKey.TemplateId];
				List<string> parts = new List<string>(3);
				SelectItemColumnHelper.AddAccessoryBonus(parts, (int)config.Strength, 0);
				SelectItemColumnHelper.AddAccessoryBonus(parts, (int)config.Dexterity, 1);
				SelectItemColumnHelper.AddAccessoryBonus(parts, (int)config.Concentration, 2);
				SelectItemColumnHelper.AddAccessoryBonus(parts, (int)config.Vitality, 3);
				SelectItemColumnHelper.AddAccessoryBonus(parts, (int)config.Energy, 4);
				SelectItemColumnHelper.AddAccessoryBonus(parts, (int)config.Intelligence, 5);
				bool flag2 = parts.Count > 0;
				if (flag2)
				{
					result = string.Join(" ", parts);
				}
				else
				{
					SelectItemColumnHelper.AddAccessoryBonus(parts, (int)config.HitRateStrength, 6);
					SelectItemColumnHelper.AddAccessoryBonus(parts, (int)config.HitRateTechnique, 7);
					SelectItemColumnHelper.AddAccessoryBonus(parts, (int)config.HitRateSpeed, 8);
					SelectItemColumnHelper.AddAccessoryBonus(parts, (int)config.HitRateMind, 9);
					SelectItemColumnHelper.AddAccessoryBonus(parts, (int)config.PenetrateOfOuter, 10);
					SelectItemColumnHelper.AddAccessoryBonus(parts, (int)config.PenetrateOfInner, 11);
					SelectItemColumnHelper.AddAccessoryBonus(parts, (int)config.AvoidRateStrength, 12);
					SelectItemColumnHelper.AddAccessoryBonus(parts, (int)config.AvoidRateTechnique, 13);
					SelectItemColumnHelper.AddAccessoryBonus(parts, (int)config.AvoidRateSpeed, 14);
					SelectItemColumnHelper.AddAccessoryBonus(parts, (int)config.AvoidRateMind, 15);
					SelectItemColumnHelper.AddAccessoryBonus(parts, (int)config.PenetrateResistOfOuter, 16);
					SelectItemColumnHelper.AddAccessoryBonus(parts, (int)config.PenetrateResistOfInner, 17);
					bool flag3 = parts.Count > 0;
					if (flag3)
					{
						result = string.Join(" ", parts);
					}
					else
					{
						SelectItemColumnHelper.AddAccessoryBonus(parts, (int)config.RecoveryOfStance, 18);
						SelectItemColumnHelper.AddAccessoryBonus(parts, (int)config.RecoveryOfBreath, 19);
						SelectItemColumnHelper.AddAccessoryBonus(parts, (int)config.MoveSpeed, 20);
						SelectItemColumnHelper.AddAccessoryBonus(parts, (int)config.RecoveryOfFlaw, 21);
						SelectItemColumnHelper.AddAccessoryBonus(parts, (int)config.CastSpeed, 22);
						SelectItemColumnHelper.AddAccessoryBonus(parts, (int)config.RecoveryOfBlockedAcupoint, 23);
						SelectItemColumnHelper.AddAccessoryBonus(parts, (int)config.WeaponSwitchSpeed, 24);
						SelectItemColumnHelper.AddAccessoryBonus(parts, (int)config.AttackSpeed, 25);
						SelectItemColumnHelper.AddAccessoryBonus(parts, (int)config.InnerRatio, 26);
						SelectItemColumnHelper.AddAccessoryBonus(parts, (int)config.RecoveryOfQiDisorder, 27);
						bool flag4 = parts.Count > 0;
						if (flag4)
						{
							result = string.Join(" ", parts);
						}
						else
						{
							SelectItemColumnHelper.AddAccessoryBonus(parts, (int)config.ResistOfHotPoison, 28);
							SelectItemColumnHelper.AddAccessoryBonus(parts, (int)config.ResistOfGloomyPoison, 29);
							SelectItemColumnHelper.AddAccessoryBonus(parts, (int)config.ResistOfColdPoison, 30);
							SelectItemColumnHelper.AddAccessoryBonus(parts, (int)config.ResistOfRedPoison, 31);
							SelectItemColumnHelper.AddAccessoryBonus(parts, (int)config.ResistOfRottenPoison, 32);
							SelectItemColumnHelper.AddAccessoryBonus(parts, (int)config.ResistOfIllusoryPoison, 33);
							bool flag5 = parts.Count > 0;
							if (flag5)
							{
								result = string.Join(" ", parts);
							}
							else
							{
								bool flag6 = config.BonusCombatSkillSect >= 0;
								if (flag6)
								{
									string sectName = Organization.Instance[config.BonusCombatSkillSect].Name;
									result = string.Format("{0}+{1}%", sectName, GlobalConfig.Instance.SectAccessoryBonusCombatSkillPower);
								}
								else
								{
									result = "-";
								}
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06005E91 RID: 24209 RVA: 0x002B77C0 File Offset: 0x002B59C0
		private static void AddAccessoryBonus(List<string> parts, int value, int propertyTypeIndex)
		{
			bool flag = value <= 0;
			if (!flag)
			{
				bool flag2 = propertyTypeIndex >= CharacterPropertyReferenced.Instance.Count;
				if (!flag2)
				{
					short displayType = CharacterPropertyReferenced.Instance[propertyTypeIndex].DisplayType;
					string propName = CharacterPropertyDisplay.Instance[displayType].Name;
					parts.Add(string.Format("{0}+{1}", propName, value));
				}
			}
		}

		// Token: 0x06005E92 RID: 24210 RVA: 0x002B782C File Offset: 0x002B5A2C
		private static string GetClothingCharmString(ITradeableContent data)
		{
			bool flag = data.RealKey.ItemType != 3;
			string result;
			if (flag)
			{
				result = "-";
			}
			else
			{
				ClothingItem config = Clothing.Instance[data.RealKey.TemplateId];
				AvatarManager avatarManager = SingletonObject.getInstance<AvatarManager>();
				short maleCharm = avatarManager.GetAsset(3, EAvatarElementsType.Cloth, new short[]
				{
					config.DisplayId
				}).Config.ElemCharm;
				short femaleCharm = avatarManager.GetAsset(4, EAvatarElementsType.Cloth, new short[]
				{
					config.DisplayId
				}).Config.ElemCharm;
				short charm = Math.Max(maleCharm, femaleCharm);
				result = ((charm >= 0) ? string.Format("+{0}", charm) : charm.ToString());
			}
			return result;
		}

		// Token: 0x06005E93 RID: 24211 RVA: 0x002B78EC File Offset: 0x002B5AEC
		private static string GetToolEffectString(ITradeableContent data)
		{
			bool flag = data.RealKey.ItemType != 6;
			string result;
			if (flag)
			{
				result = "-";
			}
			else
			{
				CraftToolItem config = CraftTool.Instance[data.RealKey.TemplateId];
				bool flag2 = config.RequiredLifeSkillTypes == null || config.RequiredLifeSkillTypes.Count == 0;
				if (flag2)
				{
					result = "-";
				}
				else
				{
					short attainment = UI_Make.GetToolAttainment(data.RealKey.TemplateId, -1);
					bool flag3 = attainment == 0;
					if (flag3)
					{
						result = LanguageKey.LK_SelectItem_ItemCell_ToolEffect_None.Tr();
					}
					else
					{
						sbyte skillType = config.RequiredLifeSkillTypes[0];
						string skillTypeName = LocalStringManager.Get(string.Format("LK_LifeSkillType_{0}", skillType));
						result = string.Format("{0}{1}：+{2}", skillTypeName, LanguageKey.LK_Attainment.Tr(), attainment);
					}
				}
			}
			return result;
		}

		// Token: 0x06005E94 RID: 24212 RVA: 0x002B79CC File Offset: 0x002B5BCC
		private static string GetRefiningEffectString(ITradeableContent data)
		{
			bool flag = data.RealKey.ItemType != 5;
			string result;
			if (flag)
			{
				result = "-";
			}
			else
			{
				MaterialItem config = Config.Material.Instance[data.RealKey.TemplateId];
				bool flag2 = config.RefiningEffect < 0;
				if (flag2)
				{
					result = "-";
				}
				else
				{
					RefiningEffectItem refiningEffectConfig = RefiningEffect.Instance[config.RefiningEffect];
					List<string> parts = new List<string>(3);
					ERefiningEffectWeaponType weaponType = refiningEffectConfig.WeaponType;
					sbyte weaponValue = refiningEffectConfig.WeaponBonusValues[(int)config.Grade];
					bool flag3 = weaponValue != 0;
					if (flag3)
					{
						string weaponName = LocalStringManager.Get(TipsRefiningEffect.RefiningPropertyNameKey[0][(int)weaponType]);
						string weaponTypeName = LanguageKey.LK_ItemTips_RefineEffect_Weapon.Tr();
						parts.Add(string.Format("{0}{1}{2}{3}%", new object[]
						{
							weaponTypeName,
							weaponName,
							(weaponValue >= 0) ? "+" : "",
							weaponValue
						}));
					}
					ERefiningEffectArmorType armorType = refiningEffectConfig.ArmorType;
					sbyte armorValue = refiningEffectConfig.ArmorBonusValues[(int)config.Grade];
					bool flag4 = armorValue != 0;
					if (flag4)
					{
						string armorName = LocalStringManager.Get(TipsRefiningEffect.RefiningPropertyNameKey[1][(int)armorType]);
						string armorTypeName = LanguageKey.LK_ItemTips_RefineEffect_Armor.Tr();
						parts.Add(string.Format("{0}{1}{2}{3}%", new object[]
						{
							armorTypeName,
							armorName,
							(armorValue >= 0) ? "+" : "",
							armorValue
						}));
					}
					ERefiningEffectAccessoryType accessoryType = refiningEffectConfig.AccessoryType;
					sbyte accessoryValue = refiningEffectConfig.AccessoryBonusValues[(int)config.Grade];
					bool flag5 = accessoryValue != 0;
					if (flag5)
					{
						string accessoryName = LocalStringManager.Get(TipsRefiningEffect.RefiningPropertyNameKey[2][(int)accessoryType]);
						string accessoryTypeName = LanguageKey.LK_ItemTips_RefineEffect_Accessory.Tr();
						parts.Add(string.Format("{0}{1}{2}{3}", new object[]
						{
							accessoryTypeName,
							accessoryName,
							(accessoryValue >= 0) ? "+" : "",
							accessoryValue
						}));
					}
					result = ((parts.Count > 0) ? string.Join(" ", parts) : "-");
				}
			}
			return result;
		}

		// Token: 0x06005E95 RID: 24213 RVA: 0x002B7BE8 File Offset: 0x002B5DE8
		private static string GetCombatSkillTypeString(ITradeableContent data)
		{
			bool flag = data.RealKey.ItemType != 12;
			string result;
			if (flag)
			{
				result = "-";
			}
			else
			{
				short templateId = data.RealKey.TemplateId;
				int combatSkillType = (int)(templateId - 240);
				bool flag2 = combatSkillType < 0 || combatSkillType > 13;
				if (flag2)
				{
					result = "-";
				}
				else
				{
					result = LocalStringManager.Get(string.Format("LK_CombatSkillType_{0}", combatSkillType));
				}
			}
			return result;
		}

		// Token: 0x04004174 RID: 16756
		private const float MinWidthSmall = 60f;

		// Token: 0x04004175 RID: 16757
		private const float MinWidthMedium = 80f;

		// Token: 0x04004176 RID: 16758
		private const float PreferredWidthSmall = 80f;

		// Token: 0x04004177 RID: 16759
		private const float PreferredWidthMedium = 100f;

		// Token: 0x04004178 RID: 16760
		private const float PreferredWidthLarge = 120f;

		// Token: 0x04004179 RID: 16761
		public const string EmptyBlockContent = "-";
	}
}
