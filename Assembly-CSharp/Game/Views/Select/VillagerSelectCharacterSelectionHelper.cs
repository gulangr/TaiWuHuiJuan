using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork;
using Game.Components.ListStyleGeneralScroll;
using Game.Views.Select.SelectCharacter;
using GameData.Domains.Character;
using GameData.Domains.Character.Creation;
using GameData.Domains.Character.Display;
using GameData.Domains.Map;
using GameData.Serializer;
using GameData.Utilities;

namespace Game.Views.Select
{
	// Token: 0x020007A0 RID: 1952
	public static class VillagerSelectCharacterSelectionHelper
	{
		// Token: 0x06005E5B RID: 24155 RVA: 0x002B5544 File Offset: 0x002B3744
		public static void OpenDefaultSelectChar(List<int> charIds, List<int> selectedCharIds, SelectCharacterCallback callback, ESelectCharacterInteractionMode interactionMode = ESelectCharacterInteractionMode.Instant, ESelectCharacterSelectionMode selectionMode = ESelectCharacterSelectionMode.Single, int taretCount = 1, IAsyncMethodRequestHandler requestHandler = null, HashSet<int> bannedCharacterIds = null, Func<IReadOnlyList<int>, string> textGenerator = null)
		{
			CharacterDomainMethod.AsyncCall.GetCharacterDisplayDataForGeneralScrollListBatch(requestHandler, charIds, delegate(int offset, RawDataPool pool)
			{
				List<CharacterDisplayDataForGeneralScrollList> displayData = new List<CharacterDisplayDataForGeneralScrollList>();
				Serializer.Deserialize(pool, offset, ref displayData);
				List<ISelectCharacterData> dataList = new List<ISelectCharacterData>(displayData.Count);
				HashSet<int> banned = bannedCharacterIds;
				if (banned == null)
				{
					banned = new HashSet<int>();
				}
				foreach (CharacterDisplayDataForGeneralScrollList data in displayData)
				{
					bool flag = data == null;
					if (!flag)
					{
						bool flag2 = AgeGroup.GetAgeGroup(data.PhysiologicalAge) == 0;
						if (flag2)
						{
							banned.Add(data.CharacterId);
						}
						dataList.Add(new BasicSelectCharacterDataAdapter(data));
					}
				}
				CommonSelectCharacterConfig config = CommonSelectCharacterConfig.CreateBasicFilterConfig(ESelectCharacterSubPage.None);
				config.InteractionMode = interactionMode;
				config.SelectionMode = selectionMode;
				config.TargetCount = taretCount;
				config.CustomTextGenerator = textGenerator;
				config.FilterMenuIds = new List<ESelectCharacterFilterMenuId>
				{
					ESelectCharacterFilterMenuId.Gender,
					ESelectCharacterFilterMenuId.BehaviorType,
					ESelectCharacterFilterMenuId.Relation,
					ESelectCharacterFilterMenuId.AdoreRelation,
					ESelectCharacterFilterMenuId.EnemyRelation,
					ESelectCharacterFilterMenuId.Organization,
					ESelectCharacterFilterMenuId.Sect
				};
				config.InitialSelectedCharacterIds = selectedCharIds;
				config.BannedCharacterIds = banned;
				UIElement.SelectChar.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("SelectCharacterConfig", config).SetObject("SelectCharacterDataList", dataList).SetObject("SelectCharacterCallback", callback));
				UIManager.Instance.MaskUI(UIElement.SelectChar);
			});
		}

		// Token: 0x06005E5C RID: 24156 RVA: 0x002B55A4 File Offset: 0x002B37A4
		public static void OpenDefendTreeSelectChar(List<int> charIds, List<int> selectedCharIds, SelectCharacterCallback callback, ESelectCharacterInteractionMode interactionMode = ESelectCharacterInteractionMode.Instant, ESelectCharacterSelectionMode selectionMode = ESelectCharacterSelectionMode.Single, int taretCount = 1, IAsyncMethodRequestHandler requestHandler = null, HashSet<int> bannedCharacterIds = null, Func<IEnumerable<CharacterDisplayDataForGeneralScrollList>, string> textGenerator = null)
		{
			CharacterDomainMethod.AsyncCall.GetCharacterDisplayDataForGeneralScrollListBatch(requestHandler, charIds, delegate(int offset, RawDataPool pool)
			{
				List<CharacterDisplayDataForGeneralScrollList> displayData = new List<CharacterDisplayDataForGeneralScrollList>();
				Serializer.Deserialize(pool, offset, ref displayData);
				List<ISelectCharacterData> dataList = new List<ISelectCharacterData>(displayData.Count);
				HashSet<int> banned = bannedCharacterIds;
				if (banned == null)
				{
					banned = new HashSet<int>();
				}
				foreach (CharacterDisplayDataForGeneralScrollList data in displayData)
				{
					bool flag = data == null;
					if (!flag)
					{
						bool flag2 = AgeGroup.GetAgeGroup(data.PhysiologicalAge) == 0;
						if (flag2)
						{
							banned.Add(data.CharacterId);
						}
						dataList.Add(new BasicSelectCharacterDataAdapter(data));
					}
				}
				CommonSelectCharacterConfig config = CommonSelectCharacterConfig.CreateBasicFilterConfig(ESelectCharacterSubPage.None);
				config.InteractionMode = interactionMode;
				config.SelectionMode = selectionMode;
				config.TargetCount = taretCount;
				config.CustomTextGenerator = ((IReadOnlyList<int> list) => textGenerator(from t in displayData
				where list.Contains(t.CharacterId)
				select t));
				config.FilterMenuIds = new List<ESelectCharacterFilterMenuId>
				{
					ESelectCharacterFilterMenuId.Gender,
					ESelectCharacterFilterMenuId.BehaviorType,
					ESelectCharacterFilterMenuId.Relation,
					ESelectCharacterFilterMenuId.AdoreRelation,
					ESelectCharacterFilterMenuId.EnemyRelation,
					ESelectCharacterFilterMenuId.Organization,
					ESelectCharacterFilterMenuId.Sect
				};
				config.InitialSelectedCharacterIds = selectedCharIds;
				config.BannedCharacterIds = banned;
				UIElement.SelectChar.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("SelectCharacterConfig", config).SetObject("SelectCharacterDataList", dataList).SetObject("SelectCharacterCallback", callback));
				UIManager.Instance.MaskUI(UIElement.SelectChar);
			});
		}

		// Token: 0x06005E5D RID: 24157 RVA: 0x002B5604 File Offset: 0x002B3804
		public static List<ISelectCharacterData> CreateDataList(List<VillagerSelectCharacterDisplayData> displayDataList)
		{
			List<ISelectCharacterData> dataList = new List<ISelectCharacterData>((displayDataList != null) ? displayDataList.Count : 0);
			bool flag = displayDataList == null;
			List<ISelectCharacterData> result;
			if (flag)
			{
				result = dataList;
			}
			else
			{
				foreach (VillagerSelectCharacterDisplayData data in displayDataList)
				{
					dataList.Add(new VillagerSelectCharacterDataAdapter(data));
				}
				result = dataList;
			}
			return result;
		}

		// Token: 0x06005E5E RID: 24158 RVA: 0x002B5684 File Offset: 0x002B3884
		public static CommonSelectCharacterConfig CreateVillagerSingleSlotConfig(string title, int initialSelectedCharId, ESelectCharacterSubPage additionalSubpage = ESelectCharacterSubPage.Villager)
		{
			CommonSelectCharacterConfig config = CommonSelectCharacterConfig.CreateBasicFilterConfig(additionalSubpage);
			config.Title = title;
			config.InteractionMode = ESelectCharacterInteractionMode.Slot;
			config.SelectionMode = ESelectCharacterSelectionMode.Single;
			config.TargetCount = 1;
			config.FilterMenuIds = new List<ESelectCharacterFilterMenuId>
			{
				ESelectCharacterFilterMenuId.Gender,
				ESelectCharacterFilterMenuId.BehaviorType,
				ESelectCharacterFilterMenuId.Relation,
				ESelectCharacterFilterMenuId.AdoreRelation,
				ESelectCharacterFilterMenuId.EnemyRelation,
				ESelectCharacterFilterMenuId.WorkStatus,
				ESelectCharacterFilterMenuId.RoleArrangementWork,
				ESelectCharacterFilterMenuId.Identity
			};
			CommonSelectCharacterConfig commonSelectCharacterConfig = config;
			object initialSelectedCharacterIds;
			if (initialSelectedCharId < 0)
			{
				initialSelectedCharacterIds = null;
			}
			else
			{
				(initialSelectedCharacterIds = new List<int>()).Add(initialSelectedCharId);
			}
			commonSelectCharacterConfig.InitialSelectedCharacterIds = initialSelectedCharacterIds;
			return config;
		}

		// Token: 0x06005E5F RID: 24159 RVA: 0x002B5728 File Offset: 0x002B3928
		[Obsolete]
		public static SelectCharacterConfig CreateBasicSelectSingleVillagerConfig(string title, int initialSelectedCharId)
		{
			SelectCharacterConfig selectCharacterConfig = new SelectCharacterConfig();
			selectCharacterConfig.Title = title;
			selectCharacterConfig.InteractionMode = ESelectCharacterInteractionMode.Slot;
			selectCharacterConfig.SelectionMode = ESelectCharacterSelectionMode.Single;
			selectCharacterConfig.TargetCount = 1;
			selectCharacterConfig.FilterMenuIds = new List<ESelectCharacterFilterMenuId>
			{
				ESelectCharacterFilterMenuId.Gender,
				ESelectCharacterFilterMenuId.BehaviorType,
				ESelectCharacterFilterMenuId.Relation,
				ESelectCharacterFilterMenuId.AdoreRelation,
				ESelectCharacterFilterMenuId.EnemyRelation,
				ESelectCharacterFilterMenuId.WorkStatus,
				ESelectCharacterFilterMenuId.RoleArrangementWork,
				ESelectCharacterFilterMenuId.Identity
			};
			selectCharacterConfig.CustomSubPageName = LanguageKey.LK_Villager.Tr();
			selectCharacterConfig.CustomColumnGenerator = new Func<IEnumerable<ColumnDefinition>>(VillagerSelectCharacterSelectionHelper.GenerateVillagerColumns);
			SelectCharacterConfig selectCharacterConfig2 = selectCharacterConfig;
			object initialSelectedCharacterIds;
			if (initialSelectedCharId < 0)
			{
				initialSelectedCharacterIds = null;
			}
			else
			{
				(initialSelectedCharacterIds = new List<int>()).Add(initialSelectedCharId);
			}
			selectCharacterConfig2.InitialSelectedCharacterIds = initialSelectedCharacterIds;
			return selectCharacterConfig;
		}

		// Token: 0x06005E60 RID: 24160 RVA: 0x002B57EE File Offset: 0x002B39EE
		public static IEnumerable<ColumnDefinition> GenerateVillagerColumns()
		{
			ColumnDefinition<ISelectCharacterData, string> columnDefinition = new ColumnDefinition<ISelectCharacterData, string>();
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_Char_Age.Tr());
			columnDefinition.CellDataGenerator = ((ISelectCharacterData data) => VillagerSelectCharacterSelectionHelper.GetAgeDisplayString(data));
			columnDefinition.LayoutOption = new LayoutOption
			{
				MinWidth = 60f,
				PreferredWidth = 80f,
				FlexibleWidth = 1f
			};
			columnDefinition.SortId = 8;
			yield return columnDefinition;
			ColumnDefinition<ISelectCharacterData, string> columnDefinition2 = new ColumnDefinition<ISelectCharacterData, string>();
			columnDefinition2.TableHeadLabel = (() => LanguageKey.LK_VillagerRole_Title_Short.Tr());
			columnDefinition2.CellDataGenerator = delegate(ISelectCharacterData data)
			{
				VillagerSelectCharacterDataAdapter villagerData = data as VillagerSelectCharacterDataAdapter;
				bool flag = villagerData != null;
				string result;
				if (flag)
				{
					VillagerSelectCharacterDisplayData raw = villagerData.GetRawData();
					result = CommonUtils.GetIdentityStringWithSpecialCharacterConfig((int)raw.MainData.CharacterTemplateId, raw.OrgInfo, raw.MainData.Gender, raw.MainData.PhysiologicalAge, false);
				}
				else
				{
					result = "";
				}
				return result;
			};
			columnDefinition2.LayoutOption = new LayoutOption
			{
				MinWidth = 60f,
				PreferredWidth = 80f,
				FlexibleWidth = 1f
			};
			yield return columnDefinition2;
			ColumnDefinition<ISelectCharacterData, string> columnDefinition3 = new ColumnDefinition<ISelectCharacterData, string>();
			columnDefinition3.TableHeadLabel = (() => LanguageKey.LK_VillagerInfo_WorkingStatus.Tr());
			columnDefinition3.CellDataGenerator = delegate(ISelectCharacterData data)
			{
				VillagerSelectCharacterDataAdapter villagerData = data as VillagerSelectCharacterDataAdapter;
				bool flag = villagerData != null;
				string result;
				if (flag)
				{
					result = VillagerSelectCharacterSelectionHelper.GetWorkStatusText(villagerData.GetRawData());
				}
				else
				{
					result = "";
				}
				return result;
			};
			columnDefinition3.LayoutOption = new LayoutOption
			{
				MinWidth = 60f,
				PreferredWidth = 100f,
				FlexibleWidth = 1f
			};
			yield return columnDefinition3;
			ColumnDefinition<ISelectCharacterData, string> columnDefinition4 = new ColumnDefinition<ISelectCharacterData, string>();
			columnDefinition4.TableHeadLabel = (() => LanguageKey.LK_VillagerInfo_ManagePlace.Tr());
			columnDefinition4.CellDataGenerator = delegate(ISelectCharacterData data)
			{
				VillagerSelectCharacterDataAdapter villagerData = data as VillagerSelectCharacterDataAdapter;
				bool flag = villagerData != null;
				string result;
				if (flag)
				{
					result = VillagerSelectCharacterSelectionHelper.GetWorkLocationName(villagerData.GetRawData());
				}
				else
				{
					result = "";
				}
				return result;
			};
			columnDefinition4.LayoutOption = new LayoutOption
			{
				MinWidth = 60f,
				PreferredWidth = 100f,
				FlexibleWidth = 1.2f
			};
			yield return columnDefinition4;
			ColumnDefinition<ISelectCharacterData, string> columnDefinition5 = new ColumnDefinition<ISelectCharacterData, string>();
			columnDefinition5.TableHeadLabel = (() => LanguageKey.LK_VillagerInfo_ManagerJob.Tr());
			columnDefinition5.CellDataGenerator = delegate(ISelectCharacterData data)
			{
				VillagerSelectCharacterDataAdapter villagerData = data as VillagerSelectCharacterDataAdapter;
				bool flag = villagerData != null;
				string result;
				if (flag)
				{
					result = VillagerSelectCharacterSelectionHelper.GetWorkPostName(villagerData.GetRawData());
				}
				else
				{
					result = "";
				}
				return result;
			};
			columnDefinition5.LayoutOption = new LayoutOption
			{
				MinWidth = 60f,
				PreferredWidth = 100f,
				FlexibleWidth = 1f
			};
			yield return columnDefinition5;
			ColumnDefinition<ISelectCharacterData, string> columnDefinition6 = new ColumnDefinition<ISelectCharacterData, string>();
			columnDefinition6.TableHeadLabel = (() => LanguageKey.LK_UI_Following_SubTitle_Location.Tr());
			columnDefinition6.CellDataGenerator = delegate(ISelectCharacterData data)
			{
				VillagerSelectCharacterDataAdapter villagerData = data as VillagerSelectCharacterDataAdapter;
				bool flag = villagerData != null;
				string result;
				if (flag)
				{
					result = VillagerSelectCharacterSelectionHelper.GetLocationName(villagerData.GetRawData());
				}
				else
				{
					result = "";
				}
				return result;
			};
			columnDefinition6.LayoutOption = new LayoutOption
			{
				MinWidth = 200f,
				PreferredWidth = 300f,
				FlexibleWidth = 1.5f
			};
			yield return columnDefinition6;
			ColumnDefinition<ISelectCharacterData, string> columnDefinition7 = new ColumnDefinition<ISelectCharacterData, string>();
			columnDefinition7.TableHeadLabel = (() => LanguageKey.LK_VillagerInfo_Potential.Tr());
			columnDefinition7.CellDataGenerator = delegate(ISelectCharacterData data)
			{
				VillagerSelectCharacterDataAdapter villagerData = data as VillagerSelectCharacterDataAdapter;
				bool flag = villagerData != null;
				string result;
				if (flag)
				{
					result = villagerData.GetRawData().LeftPotentialCount.ToString();
				}
				else
				{
					result = "";
				}
				return result;
			};
			columnDefinition7.LayoutOption = new LayoutOption
			{
				MinWidth = 60f,
				PreferredWidth = 80f,
				FlexibleWidth = 0.5f
			};
			columnDefinition7.SortId = 13;
			yield return columnDefinition7;
			ColumnDefinition<ISelectCharacterData, string> columnDefinition8 = new ColumnDefinition<ISelectCharacterData, string>();
			columnDefinition8.TableHeadLabel = (() => LanguageKey.LK_Main_SummaryInfo_Happiness.Tr());
			columnDefinition8.CellDataGenerator = delegate(ISelectCharacterData data)
			{
				CharacterDisplayDataForGeneralScrollList generalScrollListData = data.GetGeneralScrollListData();
				return CommonUtils.GetHappinessString(HappinessType.GetHappinessType((generalScrollListData != null) ? generalScrollListData.Happiness : 0));
			};
			columnDefinition8.LayoutOption = new LayoutOption
			{
				MinWidth = 60f,
				PreferredWidth = 80f,
				FlexibleWidth = 1f
			};
			columnDefinition8.SortId = 12;
			yield return columnDefinition8;
			ColumnDefinition<ISelectCharacterData, string> columnDefinition9 = new ColumnDefinition<ISelectCharacterData, string>();
			columnDefinition9.TableHeadLabel = (() => LanguageKey.LK_Health.Tr());
			columnDefinition9.CellDataGenerator = delegate(ISelectCharacterData data)
			{
				CharacterDisplayDataForGeneralScrollList genData = data.GetGeneralScrollListData();
				return (genData != null) ? VillagerSelectCharacterSelectionHelper.GetHealthDisplayString(genData) : "";
			};
			columnDefinition9.LayoutOption = new LayoutOption
			{
				MinWidth = 60f,
				PreferredWidth = 80f,
				FlexibleWidth = 1f
			};
			columnDefinition9.SortId = 10;
			yield return columnDefinition9;
			yield break;
		}

		// Token: 0x06005E61 RID: 24161 RVA: 0x002B57F8 File Offset: 0x002B39F8
		public static string GetWorkStatusText(VillagerSelectCharacterDisplayData data)
		{
			CharacterTableLocationData locationData = data.LocationData;
			string result;
			switch (data.WorkStatus)
			{
			case 0:
				result = LanguageKey.LK_Villager_WorkStatus_Unemployed.Tr().SetColor("brightblue");
				break;
			case 1:
				result = LanguageKey.LK_Villager_WorkStatus_InTaiwuGroup.Tr().SetColor("brightred");
				break;
			case 2:
			{
				bool flag = data.ArrangementTemplateId >= 0;
				if (flag)
				{
					string workName = VillagerRoleArrangement.Instance[data.ArrangementTemplateId].ShortName;
					string locationName = (data.ArrangementTemplateId == 13 && data.SwordTombId >= 0) ? LocalStringManager.Get(string.Format("LK_SwordTomb_{0}", data.SwordTombId)) : ((locationData.AreaTemplateId >= 0) ? MapArea.Instance[locationData.AreaTemplateId].Name : "");
					result = LanguageKey.LK_Villager_WorkStatus_Working.TrFormat(locationName, workName).SetColor("brightred");
				}
				else
				{
					bool flag2 = data.WorkType == 1;
					if (flag2)
					{
						result = (data.IsBuyOperation ? LanguageKey.LK_VillagerSelection_Crafting.Tr() : LanguageKey.LK_VillagerSelection_Managing.Tr());
					}
					else
					{
						bool flag3 = data.WorkType == 0;
						if (flag3)
						{
							result = (data.IsBuyOperation ? LanguageKey.LK_VillagerSelection_Building.Tr() : LanguageKey.LK_VillagerSelection_Removing.Tr());
						}
						else
						{
							string locationName2 = SingletonObject.getInstance<WorldMapModel>().GetBlockName(locationData.AreaId, locationData.BlockId, locationData.BlockTemplateId, -1);
							string workName2 = LocalStringManager.Get(string.Format("LK_WorkType_{0}", data.WorkType));
							result = LanguageKey.LK_Villager_WorkStatus_Working.TrFormat(locationName2, workName2).SetColor("brightred");
						}
					}
				}
				break;
			}
			case 3:
				result = LanguageKey.LK_Villager_WorkStatus_NotOldEnough.Tr().SetColor("brightred");
				break;
			case 4:
				result = LanguageKey.LK_ResidentState_Infected.Tr().SetColor("brightred");
				break;
			case 5:
				result = LanguageKey.LK_Villager_WorkStatus_ProtectingTaiwuVillage.Tr().SetColor("brightred");
				break;
			default:
				result = "";
				break;
			}
			return result;
		}

		// Token: 0x06005E62 RID: 24162 RVA: 0x002B5A20 File Offset: 0x002B3C20
		public static string GetWorkLocationName(VillagerSelectCharacterDisplayData data)
		{
			bool flag = data.BuildingBlockTemplateId < 0;
			string result;
			if (flag)
			{
				result = "";
			}
			else
			{
				BuildingBlockItem config = BuildingBlock.Instance[data.BuildingBlockTemplateId];
				result = config.Name;
			}
			return result;
		}

		// Token: 0x06005E63 RID: 24163 RVA: 0x002B5A60 File Offset: 0x002B3C60
		public static string GetWorkPostName(VillagerSelectCharacterDisplayData data)
		{
			bool flag = data.BuildingBlockTemplateId < 0;
			string result;
			if (flag)
			{
				result = "";
			}
			else
			{
				BuildingBlockItem config = BuildingBlock.Instance[data.BuildingBlockTemplateId];
				result = (data.IsWorkLeader ? config.LeaderName : config.MemberName);
			}
			return result;
		}

		// Token: 0x06005E64 RID: 24164 RVA: 0x002B5AB0 File Offset: 0x002B3CB0
		public static string GetLocationName(VillagerSelectCharacterDisplayData data)
		{
			CharacterTableLocationData locationData = data.LocationData;
			Location location = new Location(locationData.AreaId, locationData.BlockId);
			bool isCapturedInStoneRoom = locationData.IsCapturedInStoneRoom;
			string locationName;
			if (isCapturedInStoneRoom)
			{
				locationName = LanguageKey.LK_Character_Location_Format_StoneHouse_2.TrFormat(Organization.DefValue.Taiwu.Name);
			}
			else
			{
				bool flag = !location.IsValid();
				if (flag)
				{
					locationName = LanguageKey.LK_Character_Location_Format_Invalid_2.Tr();
				}
				else
				{
					string stateName = (locationData.StateTemplateId >= 0) ? MapState.Instance[locationData.StateTemplateId].Name : "";
					string areaName = (locationData.AreaTemplateId >= 0) ? MapArea.Instance[locationData.AreaTemplateId].Name : "";
					locationName = string.Concat(new string[]
					{
						stateName,
						"-",
						areaName,
						"-",
						SingletonObject.getInstance<WorldMapModel>().GetBlockName(location.AreaId, location.BlockId, locationData.BlockTemplateId, (int)locationData.BlockIndex)
					});
				}
			}
			bool flag2 = locationData.AdventureCoreId > 0;
			string result;
			if (flag2)
			{
				result = LanguageKey.LK_LocationItem_InAdventure.TrFormat(locationName, AdventureRemakeModel.Core.GetAdventureAny(locationData.AdventureCoreId).Name).ColorReplace();
			}
			else
			{
				bool flag3 = locationData.KidnapperId >= 0;
				if (flag3)
				{
					result = LanguageKey.LK_LocationItem_Kidnapped.TrFormat(locationName, "???").ColorReplace();
				}
				else
				{
					result = LanguageKey.LK_LocationItem_Normal.TrFormat(locationName).ColorReplace();
				}
			}
			return result;
		}

		// Token: 0x06005E65 RID: 24165 RVA: 0x002B5C34 File Offset: 0x002B3E34
		private static string GetAgeDisplayString(ISelectCharacterData data)
		{
			CharacterDisplayDataForGeneralScrollList genData = data.GetGeneralScrollListData();
			bool flag = genData == null;
			string result;
			if (flag)
			{
				result = "";
			}
			else
			{
				bool isNonEvolutionaryType = CreatingType.IsNonEvolutionaryType(genData.CreatingType);
				result = ((Character.Instance[genData.CharacterTemplateId].HideAge && isNonEvolutionaryType) ? "-" : genData.PhysiologicalAge.ToString());
			}
			return result;
		}

		// Token: 0x06005E66 RID: 24166 RVA: 0x002B5C9C File Offset: 0x002B3E9C
		private static string GetHealthDisplayString(CharacterDisplayDataForGeneralScrollList data)
		{
			bool hideHealth = data.HideHealth;
			string result;
			if (hideHealth)
			{
				result = "-";
			}
			else
			{
				result = CommonUtils.GetCharacterHealthInfo(data.Health, data.MaxLeftHealth, data.CharacterId).Item1;
			}
			return result;
		}
	}
}
