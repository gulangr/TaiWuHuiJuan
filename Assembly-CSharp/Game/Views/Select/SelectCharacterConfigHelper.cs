using System;
using System.Collections.Generic;
using System.Linq;
using FrameWork;
using Game.Components.ListStyleGeneralScroll;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.TaiwuEvent;
using GameData.Serializer;
using GameData.Utilities;

namespace Game.Views.Select
{
	// Token: 0x02000793 RID: 1939
	public static class SelectCharacterConfigHelper
	{
		// Token: 0x06005E1C RID: 24092 RVA: 0x002B4BD8 File Offset: 0x002B2DD8
		public static string GetName(this ESelectCharacterSubPage subpage)
		{
			string result;
			switch (subpage)
			{
			case ESelectCharacterSubPage.State:
				result = LanguageKey.LK_Team_Tog_State.Tr();
				break;
			case ESelectCharacterSubPage.Property:
				result = LanguageKey.LK_Team_Tog_Property.Tr();
				break;
			case ESelectCharacterSubPage.Property2:
				result = LanguageKey.LK_Team_Tog_Property_Hit.Tr();
				break;
			case ESelectCharacterSubPage.LifeSkill:
				result = LanguageKey.LK_Team_Tog_LifeSkill.Tr();
				break;
			case ESelectCharacterSubPage.CombatSkill:
				result = LanguageKey.LK_Team_Tog_CombatSkill.Tr();
				break;
			case ESelectCharacterSubPage.Personality:
				result = LanguageKey.LK_Team_Tog_Personality.Tr();
				break;
			case ESelectCharacterSubPage.Item:
				result = LanguageKey.LK_Team_Tog_Item.Tr();
				break;
			case ESelectCharacterSubPage.Command:
				result = LanguageKey.LK_Team_Tog_Command.Tr();
				break;
			case ESelectCharacterSubPage.Villager:
				result = LanguageKey.LK_Villager.Tr();
				break;
			case ESelectCharacterSubPage.DarkAah:
				result = LanguageKey.LK_KongsanSpecialInteract_DarkAah.Tr();
				break;
			case ESelectCharacterSubPage.YuanshanInfection:
				result = LanguageKey.LK_SelectCharacter_CustomTab_YuanshanTransferInfection.Tr();
				break;
			case ESelectCharacterSubPage.ApproveRate:
				result = LanguageKey.LK_SelectCharacter_CustomTab_Support.Tr();
				break;
			case ESelectCharacterSubPage.Grave:
				result = LanguageKey.LK_Map_Block_CharList_Grave.Tr();
				break;
			case ESelectCharacterSubPage.AssignRolePeasant:
			case ESelectCharacterSubPage.AssignRoleCraftsman:
			case ESelectCharacterSubPage.AssignRoleDoctor:
			case ESelectCharacterSubPage.AssignRoleMerchant:
			case ESelectCharacterSubPage.AssignRoleLiterati:
			case ESelectCharacterSubPage.AssignRoleSwordTombKeeper:
			case ESelectCharacterSubPage.AssignRoleEnvoy:
				result = LanguageKey.LK_Building_AssignRole.Tr();
				break;
			case ESelectCharacterSubPage.TasterUltimateSkill:
				result = LanguageKey.LK_Reading.Tr();
				break;
			case ESelectCharacterSubPage.SamsaraPlatform:
				result = LanguageKey.LK_CharacterAttribute_Tog_Samsara.Tr();
				break;
			case ESelectCharacterSubPage.Baihua:
				result = LanguageKey.LK_Sect_Name_Short_3.Tr();
				break;
			default:
				result = string.Empty;
				break;
			}
			return result;
		}

		// Token: 0x06005E1D RID: 24093 RVA: 0x002B4D50 File Offset: 0x002B2F50
		public static void ShowSelctGraveCharacter(List<int> list, SelectCharacterCallback OnSelectGrave, Action cancelCallback = null)
		{
			CharacterDomainMethod.AsyncCall.GetGraveDisplayDataListForSelection(null, list, delegate(int offset, RawDataPool pool)
			{
				List<GraveCharDisplayDataForSelection> dataList = new List<GraveCharDisplayDataForSelection>();
				Serializer.Deserialize(pool, offset, ref dataList);
				CommonSelectCharacterConfig config = CommonSelectCharacterConfig.CreateBasicFilterConfig(ESelectCharacterSubPage.Grave);
				config.InteractionMode = ESelectCharacterInteractionMode.Instant;
				config.SelectionMode = ESelectCharacterSelectionMode.Single;
				config.RefreshDeadAsAlive = true;
				config.CustomAvatar = ViewSelectCharacter.CreateAvatarWithNameColumn(true, delegate(TooltipInvoker displayer, int i)
				{
					displayer.Type = TipType.CharacterComplete;
					TooltipInvoker displayer2 = displayer;
					ArgumentBox argumentBox;
					if ((argumentBox = displayer2.RuntimeParam) == null)
					{
						argumentBox = (displayer2.RuntimeParam = EasyPool.Get<ArgumentBox>());
					}
					argumentBox.Set("CharId", i);
					CharacterDomainMethod.AsyncCall.GetDeadCharacterDisplayDataForTooltip(null, i, delegate(int offset, RawDataPool pool)
					{
						CharacterDisplayDataForTooltip characterDisplayDataForTooltip = new CharacterDisplayDataForTooltip();
						Serializer.Deserialize(pool, offset, ref characterDisplayDataForTooltip);
						displayer.RuntimeParam.SetObject("Data", characterDisplayDataForTooltip);
						displayer.enabled = true;
					});
				}, null);
				config.CustomColumnGenerator = new Dictionary<ESelectCharacterSubPage, Func<IEnumerable<ColumnDefinition>>>();
				config.CustomColumnGenerator[ESelectCharacterSubPage.Grave] = new Func<IEnumerable<ColumnDefinition>>(SelectCharacterConfigHelper.CustomGraveColumn);
				UIElement.SelectChar.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("SelectCharacterConfig", config).SetObject("SelectCharacterDataList", dataList).SetObject("SelectCharacterCallback", OnSelectGrave).SetObject("SelectCharacterCancelCallback", cancelCallback));
				UIManager.Instance.MaskUI(UIElement.SelectChar);
			});
		}

		// Token: 0x06005E1E RID: 24094 RVA: 0x002B4D86 File Offset: 0x002B2F86
		public static IEnumerable<ColumnDefinition> CustomGraveColumn()
		{
			yield return ViewSelectCharacter.CreateGenericTextColumn<GraveCharDisplayDataForSelection>(() => LanguageKey.LK_SelectGrave_GraveDurability.Tr(), (GraveCharDisplayDataForSelection data) => data.Durability.ToString(), -1, 30f, 90f);
			yield return ViewSelectCharacter.CharacterIdentity;
			ColumnDefinition<ISelectCharacterData, string> columnDefinition = new ColumnDefinition<ISelectCharacterData, string>();
			columnDefinition.LayoutOption = new LayoutOption
			{
				MinWidth = 60f,
				FlexibleWidth = 1f,
				PreferredWidth = 100f,
				Priority = 1
			};
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_Main_SummaryInfo_Organization.Tr());
			columnDefinition.CellDataGenerator = delegate(ISelectCharacterData data)
			{
				CharacterDisplayDataForGeneralScrollList generalData = data.GetGeneralScrollListData();
				string text;
				return CommonUtils.TryGetCharacterSpecialGradeName((int)generalData.CharacterTemplateId, out text) ? "-" : SingletonObject.getInstance<WorldMapModel>().GetSettlementName(generalData.OrgInfo);
			};
			yield return columnDefinition;
			yield return ViewSelectCharacter.BirthDate;
			yield return ViewSelectCharacter.CreateGenericTextColumn<GraveCharDisplayDataForSelection>(() => LanguageKey.LK_Died_Time.Tr(), (GraveCharDisplayDataForSelection data) => TimeManager.GetYearDisplayString(data.DeadAt), -1, 30f, 90f);
			yield break;
		}

		// Token: 0x06005E1F RID: 24095 RVA: 0x002B4D90 File Offset: 0x002B2F90
		public static void ShowSelectCharacterInstantUI(int targetCount, List<int> selectedList, HashSet<int> bannedCharacterIds, SelectCharacterCallback callback, List<int> charIds, bool skilFallbackSort = false, bool refreshDeadAsAlive = false)
		{
			CharacterDomainMethod.AsyncCall.GetCharacterDisplayDataForGeneralScrollListBatch(null, charIds, delegate(int offset, RawDataPool pool)
			{
				List<CharacterDisplayDataForGeneralScrollList> displayData = new List<CharacterDisplayDataForGeneralScrollList>();
				Serializer.Deserialize(pool, offset, ref displayData);
				List<ISelectCharacterData> selectList = (from item in displayData
				select new BasicSelectCharacterDataAdapter(item)).ToList<ISelectCharacterData>();
				CommonSelectCharacterConfig config = CommonSelectCharacterConfig.CreateBasicFilterConfig(ESelectCharacterSubPage.None);
				config.InteractionMode = ESelectCharacterInteractionMode.Instant;
				config.SelectionMode = ((targetCount == 0 || targetCount > 1) ? ESelectCharacterSelectionMode.Multiple : ESelectCharacterSelectionMode.Single);
				config.TargetCount = targetCount;
				config.InitialSelectedCharacterIds = selectedList;
				config.BannedCharacterIds = bannedCharacterIds;
				config.SkipFallbackSort = skilFallbackSort;
				config.RefreshDeadAsAlive = refreshDeadAsAlive;
				config.FilterMenuIds = new List<ESelectCharacterFilterMenuId>
				{
					ESelectCharacterFilterMenuId.Gender,
					ESelectCharacterFilterMenuId.BehaviorType,
					ESelectCharacterFilterMenuId.Relation,
					ESelectCharacterFilterMenuId.Organization,
					ESelectCharacterFilterMenuId.Sect,
					ESelectCharacterFilterMenuId.AdoreRelation,
					ESelectCharacterFilterMenuId.EnemyRelation,
					ESelectCharacterFilterMenuId.WorkStatus,
					ESelectCharacterFilterMenuId.RoleArrangementWork,
					ESelectCharacterFilterMenuId.Identity
				};
				UIElement.SelectChar.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("SelectCharacterConfig", config).SetObject("SelectCharacterDataList", selectList).SetObject("SelectCharacterCallback", callback));
				UIManager.Instance.MaskUI(UIElement.SelectChar);
			});
		}

		// Token: 0x06005E20 RID: 24096 RVA: 0x002B4DE8 File Offset: 0x002B2FE8
		public static void TriggerEvent(bool confirm, bool isMultiSelect, List<int> selectedCharIds)
		{
			bool flag = selectedCharIds.Count == 0;
			if (!flag)
			{
				if (isMultiSelect)
				{
					CharacterSet characterSet = default(CharacterSet);
					foreach (int charId in selectedCharIds)
					{
						characterSet.Add(charId);
					}
					TaiwuEventDomainMethod.Call.SetCharacterSetSelectResult("SelectCharOver", "SelectedCharId", characterSet);
				}
				else
				{
					TaiwuEventDomainMethod.Call.SetListenerEventActionIntArg("SelectCharOver", "SelectedCharId", selectedCharIds.First<int>());
				}
				TaiwuEventDomainMethod.Call.SetListenerEventActionBoolArg("SelectCharOver", "SelectedCharConfirm", confirm);
				TaiwuEventDomainMethod.Call.TriggerListener("SelectCharOver", true);
			}
		}

		// Token: 0x0400410C RID: 16652
		private static readonly List<LanguageKey> BaseSubPageNameKeys = new List<LanguageKey>
		{
			LanguageKey.LK_Team_Tog_State,
			LanguageKey.LK_Team_Tog_Property,
			LanguageKey.LK_Team_Tog_Property_Hit,
			LanguageKey.LK_Team_Tog_LifeSkill,
			LanguageKey.LK_Team_Tog_CombatSkill,
			LanguageKey.LK_Team_Tog_Personality,
			LanguageKey.LK_Team_Tog_Item,
			LanguageKey.LK_Team_Tog_Command
		};
	}
}
