using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Character;
using Game.Components.Common;
using Game.Components.ListStyleGeneralScroll;
using Game.Components.ListStyleGeneralScroll.CellContent;
using Game.Components.SortAndFilter;
using Game.Components.SortAndFilter.Team;
using GameData.Domains.Character;
using GameData.Domains.Character.Creation;
using GameData.Domains.Character.Display;
using GameData.GameDataBridge;
using GameData.Serializer;
using TMPro;
using UnityEngine;

namespace Game.Views.CharacterMenu.Team
{
	// Token: 0x02000BB2 RID: 2994
	public class ViewCharacterMenuTeam : UI_CharacterMenuSubPageBase
	{
		// Token: 0x17001030 RID: 4144
		// (get) Token: 0x06009696 RID: 38550 RVA: 0x00464042 File Offset: 0x00462242
		public override LanguageKey TitleKey
		{
			get
			{
				return LanguageKey.LK_CharacterMenu_Title_Team;
			}
		}

		// Token: 0x06009697 RID: 38551 RVA: 0x0046404C File Offset: 0x0046224C
		public override bool CheckState(ECharacterSubToggleBase curSubTogglePage, ECharacterSubPage curSubPage)
		{
			return curSubPage == ECharacterSubPage.Team;
		}

		// Token: 0x06009698 RID: 38552 RVA: 0x00464062 File Offset: 0x00462262
		private void Awake()
		{
			this.InitSubPageToggles();
			this.RefreshToggleText(null);
			GEvent.Add(UiEvents.OnLanguageChange, new GEvent.Callback(this.RefreshToggleText));
			this.InitSortAndFilter();
			this.PreCreateAllRowTemplates();
		}

		// Token: 0x06009699 RID: 38553 RVA: 0x0046409E File Offset: 0x0046229E
		private void OnDestroy()
		{
			GEvent.Remove(UiEvents.OnLanguageChange, new GEvent.Callback(this.RefreshToggleText));
		}

		// Token: 0x0600969A RID: 38554 RVA: 0x004640BD File Offset: 0x004622BD
		private new void OnDisable()
		{
			this._dataList.Clear();
			this._originalCharIdOrder.Clear();
			TabSortStateManager<TeamSubPage, GroupCharDisplayData> tabSortStateManager = this._tabSortStateManager;
			if (tabSortStateManager != null)
			{
				tabSortStateManager.ClearAll();
			}
		}

		// Token: 0x0600969B RID: 38555 RVA: 0x004640EA File Offset: 0x004622EA
		public override void OnInit(ArgumentBox argsBox)
		{
			this.NeedDataListenerId = true;
			UIElement element = this.Element;
			element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(this.OnListenerIdReady));
		}

		// Token: 0x0600969C RID: 38556 RVA: 0x0046411B File Offset: 0x0046231B
		private void OnListenerIdReady()
		{
			this.localLoadingAnim.SetLoadingState(true);
			this.RequestData();
		}

		// Token: 0x0600969D RID: 38557 RVA: 0x00464132 File Offset: 0x00462332
		public override void OnSubpageVisible()
		{
			base.OnSubpageVisible();
			this.localLoadingAnim.SetLoadingState(true);
			this.RequestData();
		}

		// Token: 0x0600969E RID: 38558 RVA: 0x00464150 File Offset: 0x00462350
		public override void OnSubpageInVisible()
		{
			base.OnSubpageInVisible();
			this._dataList.Clear();
			this._originalCharIdOrder.Clear();
			this._filteredDataList.Clear();
		}

		// Token: 0x0600969F RID: 38559 RVA: 0x0046417E File Offset: 0x0046237E
		public override void OnCurrentCharacterChange(int prevCharacterId)
		{
			this.RefreshListData();
		}

		// Token: 0x060096A0 RID: 38560 RVA: 0x00464188 File Offset: 0x00462388
		protected override void OnNotifyGameDataFiltered(List<NotificationWrapper> notifications)
		{
			foreach (NotificationWrapper wrapper in notifications)
			{
				Notification notification = wrapper.Notification;
				byte type = notification.Type;
				byte b = type;
				if (b == 1)
				{
					bool flag = notification.DomainId == 4 && notification.MethodId == 57;
					if (flag)
					{
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._dataList);
						this.RefreshList();
						this.localLoadingAnim.SetLoadingState(false);
					}
				}
			}
		}

		// Token: 0x060096A1 RID: 38561 RVA: 0x0046423C File Offset: 0x0046243C
		private void InitSubPageToggles()
		{
			this.subPageToggleGroup.Init(-1);
			ToggleGroupHotkeyController.Set(this.Element, this.subPageToggleGroup, 1, null);
			this.subPageToggleGroup.OnActiveIndexChange += delegate(int newIndex, int oldIndex)
			{
				this.OnSubPageChanged((TeamSubPage)newIndex);
			};
		}

		// Token: 0x060096A2 RID: 38562 RVA: 0x00464278 File Offset: 0x00462478
		private void RefreshToggleText(ArgumentBox _ = null)
		{
			List<CToggle> toggles = this.subPageToggleGroup.GetAll();
			for (int i = 0; i < toggles.Count; i++)
			{
				CToggle toggle = toggles[i];
				TextMeshProUGUI label = toggle.transform.Find("Label").GetComponent<TextMeshProUGUI>();
				label.text = ViewCharacterMenuTeam.ToggleGroupNameKeys[i].Tr();
			}
		}

		// Token: 0x060096A3 RID: 38563 RVA: 0x004642E0 File Offset: 0x004624E0
		private void InitSortAndFilter()
		{
			if (this.sortAndFilter == null)
			{
				this.sortAndFilter = base.GetComponentInChildren<SortAndFilter>(true);
			}
			bool flag = this.sortAndFilter == null;
			if (!flag)
			{
				this._sortAndFilterController = new TeamSortAndFilterController(this.sortAndFilter, new Func<int, bool>(this.IsTaiwu));
				this._sortAndFilterController.Init(new Action(this.OnSortOrFilterChanged), "TeamSort");
				bool flag2 = this.listScroll != null;
				if (flag2)
				{
					this.listScroll.SetSortController(this._sortAndFilterController);
				}
				this._tabSortStateManager = new TabSortStateManager<TeamSubPage, GroupCharDisplayData>(this._sortAndFilterController);
			}
		}

		// Token: 0x060096A4 RID: 38564 RVA: 0x00464384 File Offset: 0x00462584
		private void RequestData()
		{
			List<int> charIds = new List<int>();
			IReadOnlyList<CharacterDisplayData> characters = base.CharacterMenu.DisplayCharacters;
			bool flag = characters != null;
			if (flag)
			{
				for (int i = 0; i < characters.Count; i++)
				{
					CharacterDisplayData item = characters[i];
					bool flag2 = item != null;
					if (flag2)
					{
						charIds.Add(item.CharacterId);
					}
				}
			}
			this._originalCharIdOrder = new List<int>(charIds);
			CharacterDomainMethod.Call.GetGroupCharDisplayDataList(this.Element.GameDataListenerId, charIds);
		}

		// Token: 0x060096A5 RID: 38565 RVA: 0x00464407 File Offset: 0x00462607
		private void RefreshList()
		{
			this.RefreshListStructure();
			this.RefreshListData();
		}

		// Token: 0x060096A6 RID: 38566 RVA: 0x00464418 File Offset: 0x00462618
		private void RefreshListStructure()
		{
			IEnumerable<ColumnDefinition> columnDefinitions = this.GenerateColumnDefinitions(this._currentSubPage);
			this.PrepareRowTemplateContainers(this._currentSubPage);
			this.listScroll.ClearInfinityScrollCache();
			this.listScroll.Init<GroupCharDisplayData>(columnDefinitions, true, null, null);
			this.BindCellStyleProvider();
		}

		// Token: 0x060096A7 RID: 38567 RVA: 0x00464464 File Offset: 0x00462664
		private void RefreshListData()
		{
			TeamSortAndFilterController sortAndFilterController = this._sortAndFilterController;
			Func<GroupCharDisplayData, bool> func;
			if ((func = ((sortAndFilterController != null) ? sortAndFilterController.GenerateFilter() : null)) == null && (func = ViewCharacterMenuTeam.<>c.<>9__35_0) == null)
			{
				func = (ViewCharacterMenuTeam.<>c.<>9__35_0 = ((GroupCharDisplayData _) => true));
			}
			Func<GroupCharDisplayData, bool> filter = func;
			TeamSortAndFilterController sortAndFilterController2 = this._sortAndFilterController;
			Comparison<GroupCharDisplayData> comparer = (sortAndFilterController2 != null) ? sortAndFilterController2.GenerateComparer(this._dataList) : null;
			bool flag;
			if (comparer != null)
			{
				TabSortStateManager<TeamSubPage, GroupCharDisplayData> tabSortStateManager = this._tabSortStateManager;
				flag = (tabSortStateManager != null && tabSortStateManager.ShouldSort());
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			if (flag2)
			{
				this._dataList.Sort(comparer);
			}
			else
			{
				this._dataList.Sort(delegate(GroupCharDisplayData a, GroupCharDisplayData b)
				{
					bool aIsTaiwu = this.IsTaiwu(a.CharacterId);
					bool bIsTaiwu = this.IsTaiwu(b.CharacterId);
					bool flag3 = aIsTaiwu != bIsTaiwu;
					int result;
					if (flag3)
					{
						result = (aIsTaiwu ? -1 : 1);
					}
					else
					{
						bool flag4 = a.IsSpecialGroupMember != b.IsSpecialGroupMember;
						if (flag4)
						{
							result = (a.IsSpecialGroupMember ? -1 : 1);
						}
						else
						{
							int ia = this._originalCharIdOrder.IndexOf(a.CharacterId);
							int ib = this._originalCharIdOrder.IndexOf(b.CharacterId);
							result = ((ia >= 0) ? ia : int.MaxValue).CompareTo((ib >= 0) ? ib : int.MaxValue);
						}
					}
					return result;
				});
			}
			this._filteredDataList = this._dataList.Where(filter).ToList<GroupCharDisplayData>();
			this.listScroll.SetData<GroupCharDisplayData>(this._filteredDataList, -1);
			TeamSortAndFilterController sortAndFilterController3 = this._sortAndFilterController;
			if (sortAndFilterController3 != null)
			{
				sortAndFilterController3.NotifyDataChanged(this._dataList);
			}
			TeamSortAndFilterController sortAndFilterController4 = this._sortAndFilterController;
			if (sortAndFilterController4 != null)
			{
				sortAndFilterController4.AfterFilter(this._dataList);
			}
		}

		// Token: 0x060096A8 RID: 38568 RVA: 0x00464564 File Offset: 0x00462764
		private void BindCellStyleProvider()
		{
			bool flag = this.listScroll == null;
			if (!flag)
			{
				this.listScroll.CellStyleProvider = delegate(ListStyleGeneralScroll.CellStyleContext context)
				{
					GroupCharDisplayData rowData = context.RowData as GroupCharDisplayData;
					bool flag2 = rowData == null;
					ListStyleGeneralScroll.CellStyle result;
					if (flag2)
					{
						result = ListStyleGeneralScroll.CellStyle.Default;
					}
					else
					{
						bool flag3 = context.ColumnIndex <= 0;
						if (flag3)
						{
							result = ListStyleGeneralScroll.CellStyle.Default;
						}
						else
						{
							bool flag4 = this._filteredDataList == null || this._filteredDataList.Count <= 1;
							if (flag4)
							{
								result = ListStyleGeneralScroll.CellStyle.Default;
							}
							else
							{
								int value;
								bool flag5 = !ViewCharacterMenuTeam.TryGetComparableValue(this._currentSubPage, context.ColumnIndex, rowData, out value);
								if (flag5)
								{
									result = ListStyleGeneralScroll.CellStyle.Default;
								}
								else
								{
									int maxValue;
									bool flag6 = !ViewCharacterMenuTeam.TryGetMaxComparableValue(this._currentSubPage, context.ColumnIndex, this._filteredDataList, out maxValue);
									if (flag6)
									{
										result = ListStyleGeneralScroll.CellStyle.Default;
									}
									else
									{
										bool flag7 = maxValue == int.MinValue;
										if (flag7)
										{
											result = ListStyleGeneralScroll.CellStyle.Default;
										}
										else
										{
											int minValue;
											bool flag8 = ViewCharacterMenuTeam.TryGetMinComparableValue(this._currentSubPage, context.ColumnIndex, this._filteredDataList, out minValue) && minValue == maxValue;
											if (flag8)
											{
												result = ListStyleGeneralScroll.CellStyle.Default;
											}
											else
											{
												result = new ListStyleGeneralScroll.CellStyle(value >= maxValue);
											}
										}
									}
								}
							}
						}
					}
					return result;
				};
			}
		}

		// Token: 0x060096A9 RID: 38569 RVA: 0x004645A0 File Offset: 0x004627A0
		private unsafe static bool TryGetComparableValue(TeamSubPage subPage, int columnIndex, GroupCharDisplayData data, out int value)
		{
			value = 0;
			int idx = columnIndex - 1;
			bool result;
			switch (subPage)
			{
			case TeamSubPage.State:
			{
				ViewCharacterMenuTeam.EStateColumn col = (ViewCharacterMenuTeam.EStateColumn)idx;
				bool flag = col == ViewCharacterMenuTeam.EStateColumn.Favorability && !data.IsInteractedWithTaiwu;
				if (flag)
				{
					value = int.MinValue;
					result = true;
				}
				else
				{
					bool flag2 = col == ViewCharacterMenuTeam.EStateColumn.Charm && !ViewCharacterMenuTeam.IsCharmComparable(data);
					if (flag2)
					{
						value = int.MinValue;
						result = true;
					}
					else
					{
						bool flag3 = col == ViewCharacterMenuTeam.EStateColumn.QiDisorder;
						if (flag3)
						{
							value = (int)(data.DisorderOfQi / 10);
							result = true;
						}
						else
						{
							if (!true)
							{
							}
							int num;
							switch (col)
							{
							case ViewCharacterMenuTeam.EStateColumn.Age:
								num = (int)data.PhysiologicalAge;
								goto IL_145;
							case ViewCharacterMenuTeam.EStateColumn.Health:
								num = ViewCharacterMenuTeam.CalcHealthValue(data);
								goto IL_145;
							case ViewCharacterMenuTeam.EStateColumn.DefeatMark:
								num = (int)data.DefeatMarkCount;
								goto IL_145;
							case ViewCharacterMenuTeam.EStateColumn.Charm:
								num = (int)data.Charm;
								goto IL_145;
							case ViewCharacterMenuTeam.EStateColumn.Behavior:
								num = (int)data.BehaviorType;
								goto IL_145;
							case ViewCharacterMenuTeam.EStateColumn.Happiness:
								num = (int)data.Happiness;
								goto IL_145;
							case ViewCharacterMenuTeam.EStateColumn.Favorability:
								num = (int)data.FavorabilityToTaiwu;
								goto IL_145;
							case ViewCharacterMenuTeam.EStateColumn.Alertness:
								num = data.Alertness;
								goto IL_145;
							case ViewCharacterMenuTeam.EStateColumn.Samsara:
								num = (int)data.PreexistenceCharCount;
								goto IL_145;
							case ViewCharacterMenuTeam.EStateColumn.Fame:
								num = (int)data.Fame;
								goto IL_145;
							}
							num = 0;
							IL_145:
							if (!true)
							{
							}
							value = num;
							result = (idx >= 0 && idx < 11);
						}
					}
				}
				break;
			}
			case TeamSubPage.Property:
			{
				ViewCharacterMenuTeam.EPropertyColumn col2 = (ViewCharacterMenuTeam.EPropertyColumn)idx;
				bool flag4 = idx >= 0 && idx <= 5;
				if (flag4)
				{
					value = (int)(*data.MaxMainAttributes[idx]);
					result = true;
				}
				else
				{
					if (!true)
					{
					}
					int num;
					switch (col2)
					{
					case ViewCharacterMenuTeam.EPropertyColumn.PenetrateOuter:
						num = data.Penetrations.Outer;
						break;
					case ViewCharacterMenuTeam.EPropertyColumn.PenetrateInner:
						num = data.Penetrations.Inner;
						break;
					case ViewCharacterMenuTeam.EPropertyColumn.ResistOuter:
						num = data.PenetrationResists.Outer;
						break;
					case ViewCharacterMenuTeam.EPropertyColumn.ResistInner:
						num = data.PenetrationResists.Inner;
						break;
					default:
						num = 0;
						break;
					}
					if (!true)
					{
					}
					value = num;
					result = (idx >= 0 && idx < 10);
				}
				break;
			}
			case TeamSubPage.Property2:
			{
				bool flag5 = idx >= 0 && idx <= 3;
				if (flag5)
				{
					value = data.HitValues[idx];
					result = true;
				}
				else
				{
					bool flag6 = idx >= 4 && idx <= 7;
					if (flag6)
					{
						value = data.AvoidValues[idx - 4];
						result = true;
					}
					else
					{
						result = false;
					}
				}
				break;
			}
			case TeamSubPage.LifeSkill:
			{
				ViewCharacterMenuTeam.ELifeSkillColumn col3 = (ViewCharacterMenuTeam.ELifeSkillColumn)idx;
				bool flag7 = idx >= 0 && idx <= 15;
				if (flag7)
				{
					value = (int)(*data.LifeSkillQualifications[idx]);
					result = true;
				}
				else
				{
					bool flag8 = col3 == ViewCharacterMenuTeam.ELifeSkillColumn.Growth;
					if (flag8)
					{
						value = (int)ViewCharacterMenuTeam.GetSkillGrowthAddValue(data.ActualAge, (int)data.LifeSkillGrowthType);
						result = true;
					}
					else
					{
						result = false;
					}
				}
				break;
			}
			case TeamSubPage.CombatSkill:
			{
				ViewCharacterMenuTeam.ECombatSkillColumn col4 = (ViewCharacterMenuTeam.ECombatSkillColumn)idx;
				bool flag9 = idx >= 0 && idx <= 13;
				if (flag9)
				{
					value = (int)(*data.CombatSkillQualifications[idx]);
					result = true;
				}
				else
				{
					bool flag10 = col4 == ViewCharacterMenuTeam.ECombatSkillColumn.Growth;
					if (flag10)
					{
						value = (int)ViewCharacterMenuTeam.GetSkillGrowthAddValue(data.ActualAge, (int)data.CombatSkillGrowthType);
						result = true;
					}
					else
					{
						result = false;
					}
				}
				break;
			}
			case TeamSubPage.Personality:
			{
				bool flag11 = idx >= 0 && idx <= 6;
				if (flag11)
				{
					value = (int)(*data.Personalities[idx]);
					result = true;
				}
				else
				{
					result = false;
				}
				break;
			}
			case TeamSubPage.Item:
			{
				ViewCharacterMenuTeam.EItemColumn col5 = (ViewCharacterMenuTeam.EItemColumn)idx;
				bool flag12 = idx >= 0 && idx <= 7;
				if (flag12)
				{
					value = *data.Resources[idx];
					result = true;
				}
				else
				{
					if (!true)
					{
					}
					int num;
					if (col5 != ViewCharacterMenuTeam.EItemColumn.InventoryLoad)
					{
						if (col5 != ViewCharacterMenuTeam.EItemColumn.KidnapCount)
						{
							num = 0;
						}
						else
						{
							num = (int)data.KidnapCount;
						}
					}
					else
					{
						num = ((data.MaxInventoryLoad > 0) ? ((int)((long)data.CurrInventoryLoad * 10000L / (long)data.MaxInventoryLoad)) : 0);
					}
					if (!true)
					{
					}
					value = num;
					result = (idx >= 0 && idx < 10);
				}
				break;
			}
			case TeamSubPage.Command:
			{
				ViewCharacterMenuTeam.ECommandColumn col6 = (ViewCharacterMenuTeam.ECommandColumn)idx;
				if (!true)
				{
				}
				int num;
				switch (col6)
				{
				case ViewCharacterMenuTeam.ECommandColumn.AttackMedal:
					num = ((data.AttackMedal > 0) ? data.AttackMedal : int.MinValue);
					break;
				case ViewCharacterMenuTeam.ECommandColumn.DefenceMedal:
					num = ((data.DefenceMedal > 0) ? data.DefenceMedal : int.MinValue);
					break;
				case ViewCharacterMenuTeam.ECommandColumn.WisdomMedal:
					num = ((data.WisdomMedal > 0) ? data.WisdomMedal : int.MinValue);
					break;
				case ViewCharacterMenuTeam.ECommandColumn.Command0:
					num = ViewCharacterMenuTeam.GetCommandComparableValue(data, 0);
					break;
				case ViewCharacterMenuTeam.ECommandColumn.Command1:
					num = ViewCharacterMenuTeam.GetCommandComparableValue(data, 1);
					break;
				case ViewCharacterMenuTeam.ECommandColumn.Command2:
					num = ViewCharacterMenuTeam.GetCommandComparableValue(data, 2);
					break;
				default:
					num = 0;
					break;
				}
				if (!true)
				{
				}
				value = num;
				result = (idx >= 0 && idx < 6);
				break;
			}
			default:
				result = false;
				break;
			}
			return result;
		}

		// Token: 0x060096AA RID: 38570 RVA: 0x00464A64 File Offset: 0x00462C64
		private static bool TryGetMaxComparableValue(TeamSubPage subPage, int columnIndex, List<GroupCharDisplayData> list, out int maxValue)
		{
			maxValue = int.MinValue;
			bool found = false;
			foreach (GroupCharDisplayData data in list)
			{
				int value;
				bool flag = !ViewCharacterMenuTeam.TryGetComparableValue(subPage, columnIndex, data, out value);
				if (!flag)
				{
					found = true;
					bool flag2 = value > maxValue;
					if (flag2)
					{
						maxValue = value;
					}
				}
			}
			return found;
		}

		// Token: 0x060096AB RID: 38571 RVA: 0x00464AE8 File Offset: 0x00462CE8
		private static bool TryGetMinComparableValue(TeamSubPage subPage, int columnIndex, List<GroupCharDisplayData> list, out int minValue)
		{
			minValue = int.MaxValue;
			bool found = false;
			foreach (GroupCharDisplayData data in list)
			{
				int value;
				bool flag = !ViewCharacterMenuTeam.TryGetComparableValue(subPage, columnIndex, data, out value);
				if (!flag)
				{
					found = true;
					bool flag2 = value < minValue;
					if (flag2)
					{
						minValue = value;
					}
				}
			}
			return found;
		}

		// Token: 0x060096AC RID: 38572 RVA: 0x00464B6C File Offset: 0x00462D6C
		private IEnumerable<ColumnDefinition> GenerateColumnDefinitions(TeamSubPage subPage)
		{
			yield return this.CreateAvatarWithNameColumn();
			switch (subPage)
			{
			case TeamSubPage.State:
			{
				foreach (ColumnDefinition col in ViewCharacterMenuTeam.GenerateStateColumns())
				{
					yield return col;
					col = null;
				}
				IEnumerator<ColumnDefinition> enumerator = null;
				break;
			}
			case TeamSubPage.Property:
			{
				foreach (ColumnDefinition col2 in ViewCharacterMenuTeam.GeneratePropertyColumns())
				{
					yield return col2;
					col2 = null;
				}
				IEnumerator<ColumnDefinition> enumerator2 = null;
				break;
			}
			case TeamSubPage.Property2:
			{
				foreach (ColumnDefinition col3 in ViewCharacterMenuTeam.GenerateProperty2Columns())
				{
					yield return col3;
					col3 = null;
				}
				IEnumerator<ColumnDefinition> enumerator3 = null;
				break;
			}
			case TeamSubPage.LifeSkill:
			{
				foreach (ColumnDefinition col4 in ViewCharacterMenuTeam.GenerateLifeSkillColumns())
				{
					yield return col4;
					col4 = null;
				}
				IEnumerator<ColumnDefinition> enumerator4 = null;
				break;
			}
			case TeamSubPage.CombatSkill:
			{
				foreach (ColumnDefinition col5 in ViewCharacterMenuTeam.GenerateCombatSkillColumns())
				{
					yield return col5;
					col5 = null;
				}
				IEnumerator<ColumnDefinition> enumerator5 = null;
				break;
			}
			case TeamSubPage.Personality:
			{
				foreach (ColumnDefinition col6 in ViewCharacterMenuTeam.GeneratePersonalityColumns())
				{
					yield return col6;
					col6 = null;
				}
				IEnumerator<ColumnDefinition> enumerator6 = null;
				break;
			}
			case TeamSubPage.Item:
			{
				foreach (ColumnDefinition col7 in ViewCharacterMenuTeam.GenerateItemColumns())
				{
					yield return col7;
					col7 = null;
				}
				IEnumerator<ColumnDefinition> enumerator7 = null;
				break;
			}
			case TeamSubPage.Command:
			{
				foreach (ColumnDefinition col8 in ViewCharacterMenuTeam.GenerateCommandColumns())
				{
					yield return col8;
					col8 = null;
				}
				IEnumerator<ColumnDefinition> enumerator8 = null;
				break;
			}
			}
			yield break;
			yield break;
		}

		// Token: 0x060096AD RID: 38573 RVA: 0x00464B83 File Offset: 0x00462D83
		private IEnumerable<RowCellContainer> GetCellContainerTemplates(TeamSubPage subPage)
		{
			yield return this.avatarAndNameCellContainer;
			if (subPage != TeamSubPage.Command)
			{
				int columnCount = ViewCharacterMenuTeam.GetColumnCount(subPage);
				int num;
				for (int i = 0; i < columnCount; i = num + 1)
				{
					yield return this.singleTextCellContainer;
					num = i;
				}
			}
			else
			{
				int num;
				for (int j = 0; j < 6; j = num + 1)
				{
					yield return this.iconAndTextCellContainer;
					num = j;
				}
			}
			yield break;
		}

		// Token: 0x060096AE RID: 38574 RVA: 0x00464B9C File Offset: 0x00462D9C
		private static int GetColumnCount(TeamSubPage subPage)
		{
			if (!true)
			{
			}
			int result;
			switch (subPage)
			{
			case TeamSubPage.State:
				result = 11;
				break;
			case TeamSubPage.Property:
				result = 10;
				break;
			case TeamSubPage.Property2:
				result = 8;
				break;
			case TeamSubPage.LifeSkill:
				result = 17;
				break;
			case TeamSubPage.CombatSkill:
				result = 15;
				break;
			case TeamSubPage.Personality:
				result = 7;
				break;
			case TeamSubPage.Item:
				result = 10;
				break;
			case TeamSubPage.Command:
				result = 6;
				break;
			default:
				result = 0;
				break;
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x060096AF RID: 38575 RVA: 0x00464C08 File Offset: 0x00462E08
		private void PrepareRowTemplateContainers(TeamSubPage subPage)
		{
			RowItem cachedTemplate;
			bool flag = this._rowTemplateCache.TryGetValue(subPage, out cachedTemplate);
			if (flag)
			{
				this.listScroll.SetRowTemplate(cachedTemplate);
			}
			else
			{
				RowItem newTemplate = this.CreateRowTemplateForSubPage(subPage);
				this._rowTemplateCache[subPage] = newTemplate;
				this.listScroll.SetRowTemplate(newTemplate);
			}
		}

		// Token: 0x060096B0 RID: 38576 RVA: 0x00464C5C File Offset: 0x00462E5C
		private RowItem CreateRowTemplateForSubPage(TeamSubPage subPage)
		{
			RowItem newTemplate = Object.Instantiate<RowItem>(this.rowTemplate, this.rowTemplate.transform.parent);
			newTemplate.gameObject.SetActive(false);
			Transform containerRoot = newTemplate.ContainerRoot;
			foreach (RowCellContainer containerTemplate in this.GetCellContainerTemplates(subPage))
			{
				RowCellContainer container = Object.Instantiate<RowCellContainer>(containerTemplate, containerRoot);
				container.gameObject.SetActive(true);
			}
			newTemplate.ResetSibling();
			return newTemplate;
		}

		// Token: 0x060096B1 RID: 38577 RVA: 0x00464D00 File Offset: 0x00462F00
		private void PreCreateAllRowTemplates()
		{
			this.rowTemplate.gameObject.SetActive(false);
			foreach (object obj in Enum.GetValues(typeof(TeamSubPage)))
			{
				TeamSubPage subPage = (TeamSubPage)obj;
				RowItem template = this.CreateRowTemplateForSubPage(subPage);
				this._rowTemplateCache[subPage] = template;
			}
		}

		// Token: 0x060096B2 RID: 38578 RVA: 0x00464D88 File Offset: 0x00462F88
		private ColumnDefinition CreateAvatarWithNameColumn()
		{
			ColumnDefinition<GroupCharDisplayData, AvatarWithNameCellData> columnDefinition = new ColumnDefinition<GroupCharDisplayData, AvatarWithNameCellData>();
			columnDefinition.LayoutOption = new LayoutOption
			{
				MinWidth = 314f,
				FlexibleWidth = 0f,
				PreferredWidth = 314f,
				Priority = 1
			};
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_Char_Name.Tr());
			columnDefinition.CellDataGenerator = ((GroupCharDisplayData data) => AvatarWithNameCellData.FromGroupCharDisplayData(data, this.IsTaiwu(data.CharacterId), new Action<int>(this.OnCharacterClicked), null, false, false));
			columnDefinition.SortId = 0;
			return columnDefinition;
		}

		// Token: 0x060096B3 RID: 38579 RVA: 0x00464E18 File Offset: 0x00463018
		private static ColumnDefinition CreateTextColumn(Func<string> headerKey, Func<GroupCharDisplayData, string> valueGetter, short sortId = -1, float minWidth = 30f, float preferredWidth = 224f)
		{
			return new ColumnDefinition<GroupCharDisplayData, string>
			{
				LayoutOption = new LayoutOption
				{
					MinWidth = minWidth,
					FlexibleWidth = 1f,
					PreferredWidth = preferredWidth,
					Priority = 1
				},
				TableHeadLabel = headerKey,
				CellDataGenerator = valueGetter,
				SortId = sortId
			};
		}

		// Token: 0x060096B4 RID: 38580 RVA: 0x00464E78 File Offset: 0x00463078
		private static ColumnDefinition CreateIconAndTextColumn(Func<string> headerKey, Func<GroupCharDisplayData, IconAndTextCellData> valueGetter, short sortId = -1, float minWidth = 80f, float preferredWidth = 336f)
		{
			return new ColumnDefinition<GroupCharDisplayData, IconAndTextCellData>
			{
				LayoutOption = new LayoutOption
				{
					MinWidth = minWidth,
					FlexibleWidth = 1f,
					PreferredWidth = preferredWidth,
					Priority = 1
				},
				TableHeadLabel = headerKey,
				CellDataGenerator = valueGetter,
				SortId = sortId
			};
		}

		// Token: 0x060096B5 RID: 38581 RVA: 0x00464ED8 File Offset: 0x004630D8
		private static IEnumerable<ColumnDefinition> GenerateStateColumns()
		{
			yield return ViewCharacterMenuTeam.CreateTextColumn(() => LanguageKey.LK_Char_Age.Tr(), (GroupCharDisplayData data) => ViewCharacterMenuTeam.GetAgeDisplayString(data), 8, 30f, 224f);
			yield return ViewCharacterMenuTeam.CreateTextColumn(() => LanguageKey.LK_Health.Tr(), (GroupCharDisplayData data) => CommonUtils.GetCharacterHealthInfo(data.Health, data.MaxLeftHealth, data.CharacterId).Item1, 10, 30f, 224f);
			yield return ViewCharacterMenuTeam.CreateTextColumn(() => LanguageKey.LK_Injury.Tr(), (GroupCharDisplayData data) => data.DefeatMarkCount.ToString(), 53, 30f, 224f);
			yield return ViewCharacterMenuTeam.CreateTextColumn(() => LanguageKey.LK_Qi_Disorder.Tr(), (GroupCharDisplayData data) => ((int)(data.DisorderOfQi / 10)).ToString(), 55, 30f, 224f);
			yield return ViewCharacterMenuTeam.CreateTextColumn(() => LanguageKey.LK_Main_SummaryInfo_Charm.Tr(), (GroupCharDisplayData data) => ViewCharacterMenuTeam.GetCharmDisplayString(data), 9, 30f, 224f);
			yield return ViewCharacterMenuTeam.CreateTextColumn(() => LanguageKey.LK_Main_SummaryInfo_Behavior.Tr(), (GroupCharDisplayData data) => CommonUtils.GetBehaviorString(data.BehaviorType), 57, 30f, 224f);
			yield return ViewCharacterMenuTeam.CreateTextColumn(() => LanguageKey.LK_Main_SummaryInfo_Happiness.Tr(), (GroupCharDisplayData data) => CommonUtils.GetHappinessString(HappinessType.GetHappinessType(data.Happiness)), 12, 30f, 224f);
			yield return ViewCharacterMenuTeam.CreateTextColumn(() => LanguageKey.LK_Favorability.Tr(), (GroupCharDisplayData data) => ViewCharacterMenuTeam.GetFavorDisplayString(data), 11, 30f, 224f);
			yield return ViewCharacterMenuTeam.CreateTextColumn(() => LanguageKey.LK_Alertness.Tr(), (GroupCharDisplayData data) => CommonUtils.GetAlertnessNameByValue(data.Alertness), 130, 30f, 224f);
			yield return ViewCharacterMenuTeam.CreateTextColumn(() => LanguageKey.LK_Samsara.Tr(), (GroupCharDisplayData data) => data.PreexistenceCharCount.ToString(), 58, 30f, 224f);
			yield return ViewCharacterMenuTeam.CreateTextColumn(() => LanguageKey.LK_Main_SummaryInfo_Fame.Tr(), (GroupCharDisplayData data) => CommonUtils.GetFameString(FameType.GetFameType(data.Fame)), 59, 30f, 224f);
			yield break;
		}

		// Token: 0x060096B6 RID: 38582 RVA: 0x00464EE1 File Offset: 0x004630E1
		private static IEnumerable<ColumnDefinition> GeneratePropertyColumns()
		{
			yield return ViewCharacterMenuTeam.CreateTextColumn(() => LanguageKey.LK_Main_Attribute_Strength.Tr(), (GroupCharDisplayData data) => data.MaxMainAttributes[0].ToString(), 60, 30f, 224f);
			yield return ViewCharacterMenuTeam.CreateTextColumn(() => LanguageKey.LK_Main_Attribute_Dexterity.Tr(), (GroupCharDisplayData data) => data.MaxMainAttributes[1].ToString(), 61, 30f, 224f);
			yield return ViewCharacterMenuTeam.CreateTextColumn(() => LanguageKey.LK_Main_Attribute_Concentration.Tr(), (GroupCharDisplayData data) => data.MaxMainAttributes[2].ToString(), 62, 30f, 224f);
			yield return ViewCharacterMenuTeam.CreateTextColumn(() => LanguageKey.LK_Main_Attribute_Vitality.Tr(), (GroupCharDisplayData data) => data.MaxMainAttributes[3].ToString(), 63, 30f, 224f);
			yield return ViewCharacterMenuTeam.CreateTextColumn(() => LanguageKey.LK_Main_Attribute_Energy.Tr(), (GroupCharDisplayData data) => data.MaxMainAttributes[4].ToString(), 64, 30f, 224f);
			yield return ViewCharacterMenuTeam.CreateTextColumn(() => LanguageKey.LK_Main_Attribute_Intelligence.Tr(), (GroupCharDisplayData data) => data.MaxMainAttributes[5].ToString(), 65, 30f, 224f);
			yield return ViewCharacterMenuTeam.CreateTextColumn(() => LanguageKey.LK_Penetrate_Outer.Tr(), (GroupCharDisplayData data) => data.Penetrations.Outer.ToString(), 22, 30f, 224f);
			yield return ViewCharacterMenuTeam.CreateTextColumn(() => LanguageKey.LK_Penetrate_Inner.Tr(), (GroupCharDisplayData data) => data.Penetrations.Inner.ToString(), 23, 30f, 224f);
			yield return ViewCharacterMenuTeam.CreateTextColumn(() => LanguageKey.LK_Penetrate_Resist_Outer.Tr(), (GroupCharDisplayData data) => data.PenetrationResists.Outer.ToString(), 29, 30f, 224f);
			yield return ViewCharacterMenuTeam.CreateTextColumn(() => LanguageKey.LK_Penetrate_Resist_Inner.Tr(), (GroupCharDisplayData data) => data.PenetrationResists.Inner.ToString(), 30, 30f, 224f);
			yield break;
		}

		// Token: 0x060096B7 RID: 38583 RVA: 0x00464EEA File Offset: 0x004630EA
		private static IEnumerable<ColumnDefinition> GenerateProperty2Columns()
		{
			yield return ViewCharacterMenuTeam.CreateTextColumn(() => LanguageKey.LK_HitType_0.Tr(), (GroupCharDisplayData data) => data.HitValues[0].ToString(), 24, 30f, 224f);
			yield return ViewCharacterMenuTeam.CreateTextColumn(() => LanguageKey.LK_HitType_1.Tr(), (GroupCharDisplayData data) => data.HitValues[1].ToString(), 25, 30f, 224f);
			yield return ViewCharacterMenuTeam.CreateTextColumn(() => LanguageKey.LK_HitType_2.Tr(), (GroupCharDisplayData data) => data.HitValues[2].ToString(), 26, 30f, 224f);
			yield return ViewCharacterMenuTeam.CreateTextColumn(() => LanguageKey.LK_HitType_3.Tr(), (GroupCharDisplayData data) => data.HitValues[3].ToString(), 27, 30f, 224f);
			yield return ViewCharacterMenuTeam.CreateTextColumn(() => LanguageKey.LK_AvoidType_0.Tr(), (GroupCharDisplayData data) => data.AvoidValues[0].ToString(), 33, 30f, 224f);
			yield return ViewCharacterMenuTeam.CreateTextColumn(() => LanguageKey.LK_AvoidType_1.Tr(), (GroupCharDisplayData data) => data.AvoidValues[1].ToString(), 34, 30f, 224f);
			yield return ViewCharacterMenuTeam.CreateTextColumn(() => LanguageKey.LK_AvoidType_2.Tr(), (GroupCharDisplayData data) => data.AvoidValues[2].ToString(), 35, 30f, 224f);
			yield return ViewCharacterMenuTeam.CreateTextColumn(() => LanguageKey.LK_AvoidType_3.Tr(), (GroupCharDisplayData data) => data.AvoidValues[3].ToString(), 36, 30f, 224f);
			yield break;
		}

		// Token: 0x060096B8 RID: 38584 RVA: 0x00464EF3 File Offset: 0x004630F3
		private static IEnumerable<ColumnDefinition> GenerateLifeSkillColumns()
		{
			int num;
			for (int i = 0; i < 16; i = num + 1)
			{
				ViewCharacterMenuTeam.<>c__DisplayClass52_0 CS$<>8__locals1 = new ViewCharacterMenuTeam.<>c__DisplayClass52_0();
				CS$<>8__locals1.index = i;
				yield return ViewCharacterMenuTeam.CreateTextColumn(() => LocalStringManager.Get(string.Format("LK_LifeSkillType_{0}", CS$<>8__locals1.index)), (GroupCharDisplayData data) => data.LifeSkillQualifications[CS$<>8__locals1.index].ToString(), (short)(66 + CS$<>8__locals1.index), 80f, 116f);
				CS$<>8__locals1 = null;
				num = i;
			}
			LocalStringManager.LanguageType curLanguageType = LocalStringManager.CurLanguageType;
			if (!true)
			{
			}
			float num2;
			if (curLanguageType != LocalStringManager.LanguageType.EN)
			{
				num2 = 224f;
			}
			else
			{
				num2 = 300f;
			}
			if (!true)
			{
			}
			float preferredWidth = num2;
			yield return ViewCharacterMenuTeam.CreateTextColumn(() => LanguageKey.LK_Growth.Tr(), (GroupCharDisplayData data) => ViewCharacterMenuTeam.GetSkillGrowthString(data.ActualAge, data.LifeSkillGrowthType), 118, 30f, preferredWidth);
			yield break;
		}

		// Token: 0x060096B9 RID: 38585 RVA: 0x00464EFC File Offset: 0x004630FC
		private static IEnumerable<ColumnDefinition> GenerateCombatSkillColumns()
		{
			int num;
			for (int i = 0; i < 14; i = num + 1)
			{
				ViewCharacterMenuTeam.<>c__DisplayClass53_0 CS$<>8__locals1 = new ViewCharacterMenuTeam.<>c__DisplayClass53_0();
				CS$<>8__locals1.index = i;
				yield return ViewCharacterMenuTeam.CreateTextColumn(() => LocalStringManager.Get(string.Format("LK_CombatSkillType_{0}", CS$<>8__locals1.index)), (GroupCharDisplayData data) => data.CombatSkillQualifications[CS$<>8__locals1.index].ToString(), (short)(82 + CS$<>8__locals1.index), 128f, 128f);
				CS$<>8__locals1 = null;
				num = i;
			}
			yield return ViewCharacterMenuTeam.CreateTextColumn(() => LanguageKey.LK_Growth.Tr(), (GroupCharDisplayData data) => ViewCharacterMenuTeam.GetSkillGrowthString(data.ActualAge, data.CombatSkillGrowthType), 119, 30f, 224f);
			yield break;
		}

		// Token: 0x060096BA RID: 38586 RVA: 0x00464F05 File Offset: 0x00463105
		private static IEnumerable<ColumnDefinition> GeneratePersonalityColumns()
		{
			yield return ViewCharacterMenuTeam.CreateTextColumn(() => LanguageKey.LK_Personality_Calm_Name.Tr(), (GroupCharDisplayData data) => data.Personalities[0].ToString(), 96, 30f, 224f);
			yield return ViewCharacterMenuTeam.CreateTextColumn(() => LanguageKey.LK_Personality_Clever_Name.Tr(), (GroupCharDisplayData data) => data.Personalities[1].ToString(), 97, 30f, 224f);
			yield return ViewCharacterMenuTeam.CreateTextColumn(() => LanguageKey.LK_Personality_Enthusiastic_Name.Tr(), (GroupCharDisplayData data) => data.Personalities[2].ToString(), 98, 30f, 224f);
			yield return ViewCharacterMenuTeam.CreateTextColumn(() => LanguageKey.LK_Personality_Brave_Name.Tr(), (GroupCharDisplayData data) => data.Personalities[3].ToString(), 99, 30f, 224f);
			yield return ViewCharacterMenuTeam.CreateTextColumn(() => LanguageKey.LK_Personality_Firm_Name.Tr(), (GroupCharDisplayData data) => data.Personalities[4].ToString(), 100, 30f, 224f);
			yield return ViewCharacterMenuTeam.CreateTextColumn(() => LanguageKey.LK_Personality_Lucky_Name.Tr(), (GroupCharDisplayData data) => data.Personalities[5].ToString(), 101, 30f, 224f);
			yield return ViewCharacterMenuTeam.CreateTextColumn(() => LanguageKey.LK_Personality_Perceptive_Name.Tr(), (GroupCharDisplayData data) => data.Personalities[6].ToString(), 102, 30f, 224f);
			yield break;
		}

		// Token: 0x060096BB RID: 38587 RVA: 0x00464F0E File Offset: 0x0046310E
		private static IEnumerable<ColumnDefinition> GenerateItemColumns()
		{
			yield return ViewCharacterMenuTeam.CreateTextColumn(() => LanguageKey.LK_Resource_Name_Food.Tr(), (GroupCharDisplayData data) => data.Resources[0].ToString(), 103, 30f, 224f);
			yield return ViewCharacterMenuTeam.CreateTextColumn(() => LanguageKey.LK_Resource_Name_Wood.Tr(), (GroupCharDisplayData data) => data.Resources[1].ToString(), 104, 30f, 224f);
			yield return ViewCharacterMenuTeam.CreateTextColumn(() => LanguageKey.LK_Resource_Name_Metal.Tr(), (GroupCharDisplayData data) => data.Resources[2].ToString(), 105, 30f, 224f);
			yield return ViewCharacterMenuTeam.CreateTextColumn(() => LanguageKey.LK_Resource_Name_Jade.Tr(), (GroupCharDisplayData data) => data.Resources[3].ToString(), 106, 30f, 224f);
			yield return ViewCharacterMenuTeam.CreateTextColumn(() => LanguageKey.LK_Resource_Name_Fabric.Tr(), (GroupCharDisplayData data) => data.Resources[4].ToString(), 107, 30f, 224f);
			yield return ViewCharacterMenuTeam.CreateTextColumn(() => LanguageKey.LK_Resource_Name_Herb.Tr(), (GroupCharDisplayData data) => data.Resources[5].ToString(), 108, 30f, 224f);
			yield return ViewCharacterMenuTeam.CreateTextColumn(() => LanguageKey.LK_Resource_Name_Money.Tr(), (GroupCharDisplayData data) => data.Resources[6].ToString(), 109, 30f, 224f);
			yield return ViewCharacterMenuTeam.CreateTextColumn(() => LanguageKey.LK_Resource_Name_Authority.Tr(), (GroupCharDisplayData data) => data.Resources[7].ToString(), 110, 30f, 224f);
			yield return ViewCharacterMenuTeam.CreateTextColumn(() => LanguageKey.LK_Inventory.Tr(), (GroupCharDisplayData data) => ViewCharacterMenuTeam.GetInventoryLoadString(data.CurrInventoryLoad, data.MaxInventoryLoad), 37, 30f, 224f);
			yield return ViewCharacterMenuTeam.CreateTextColumn(() => LanguageKey.LK_Kidnap.Tr(), (GroupCharDisplayData data) => data.KidnapCount.ToString(), 111, 30f, 224f);
			yield break;
		}

		// Token: 0x060096BC RID: 38588 RVA: 0x00464F17 File Offset: 0x00463117
		private static IEnumerable<ColumnDefinition> GenerateCommandColumns()
		{
			yield return ViewCharacterMenuTeam.CreateIconAndTextColumn(() => LanguageKey.LK_Feature_Attack.Tr(), (GroupCharDisplayData data) => ViewCharacterMenuTeam.CreateMedalCellData(data.AttackMedal, ViewCharacterMenuTeam.EMedalType.Attack), 112, 80f, 336f);
			yield return ViewCharacterMenuTeam.CreateIconAndTextColumn(() => LanguageKey.LK_Feature_Defence.Tr(), (GroupCharDisplayData data) => ViewCharacterMenuTeam.CreateMedalCellData(data.DefenceMedal, ViewCharacterMenuTeam.EMedalType.Defence), 113, 80f, 336f);
			yield return ViewCharacterMenuTeam.CreateIconAndTextColumn(() => LanguageKey.LK_Feature_Wisdom.Tr(), (GroupCharDisplayData data) => ViewCharacterMenuTeam.CreateMedalCellData(data.WisdomMedal, ViewCharacterMenuTeam.EMedalType.Wisdom), 114, 80f, 336f);
			yield return ViewCharacterMenuTeam.CreateIconAndTextColumn(() => LanguageKey.LK_Team_Property_Title_Command_0.Tr(), (GroupCharDisplayData data) => ViewCharacterMenuTeam.CreateCommandCellData(data, 0), 115, 80f, 336f);
			yield return ViewCharacterMenuTeam.CreateIconAndTextColumn(() => LanguageKey.LK_Team_Property_Title_Command_1.Tr(), (GroupCharDisplayData data) => ViewCharacterMenuTeam.CreateCommandCellData(data, 1), 116, 80f, 336f);
			yield return ViewCharacterMenuTeam.CreateIconAndTextColumn(() => LanguageKey.LK_Team_Property_Title_Command_2.Tr(), (GroupCharDisplayData data) => ViewCharacterMenuTeam.CreateCommandCellData(data, 2), 117, 80f, 336f);
			yield break;
		}

		// Token: 0x060096BD RID: 38589 RVA: 0x00464F20 File Offset: 0x00463120
		private static IconAndTextCellData CreateMedalCellData(int medalCount, ViewCharacterMenuTeam.EMedalType medalType)
		{
			bool flag = medalCount == 0;
			IconAndTextCellData result;
			if (flag)
			{
				result = IconAndTextCellData.TextOnly("-");
			}
			else
			{
				string iconName = ViewCharacterMenuTeam.GetMedalIconName(medalCount, medalType);
				string text = string.Format(" x{0}", Mathf.Abs(medalCount));
				result = new IconAndTextCellData(iconName, text, true, false, false, false);
			}
			return result;
		}

		// Token: 0x060096BE RID: 38590 RVA: 0x00464F74 File Offset: 0x00463174
		private static string GetMedalIconName(int medalCount, ViewCharacterMenuTeam.EMedalType medalType)
		{
			int signKey = (medalCount > 0) ? 1 : ((medalCount < 0) ? -1 : 0);
			if (!true)
			{
			}
			string text;
			switch (medalType)
			{
			case ViewCharacterMenuTeam.EMedalType.Attack:
				text = MedalSummary.AttackMedalIconConfig[signKey];
				break;
			case ViewCharacterMenuTeam.EMedalType.Defence:
				text = MedalSummary.DefenceMedalIconConfig[signKey];
				break;
			case ViewCharacterMenuTeam.EMedalType.Wisdom:
				text = MedalSummary.WisdomMedalIconConfig[signKey];
				break;
			default:
				text = string.Empty;
				break;
			}
			if (!true)
			{
			}
			string iconNumber = text;
			return "ui9_icon_strategy_big_" + iconNumber;
		}

		// Token: 0x060096BF RID: 38591 RVA: 0x00464FF4 File Offset: 0x004631F4
		private static IconAndTextCellData CreateCommandCellData(GroupCharDisplayData data, int commandIndex)
		{
			bool flag = data.Command.Items == null || !data.Command.Items.CheckIndex(commandIndex);
			IconAndTextCellData result;
			if (flag)
			{
				result = IconAndTextCellData.TextOnly("-");
			}
			else
			{
				sbyte commandId = data.Command.Items[commandIndex];
				bool flag2 = commandId < 0;
				if (flag2)
				{
					result = IconAndTextCellData.TextOnly("-");
				}
				else
				{
					TeammateCommandItem cmdConfig = Config.TeammateCommand.Instance[commandId];
					result = IconAndTextCellData.TextOnly(cmdConfig.Name);
				}
			}
			return result;
		}

		// Token: 0x060096C0 RID: 38592 RVA: 0x00465080 File Offset: 0x00463280
		private bool IsTaiwu(int charId)
		{
			return charId == SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
		}

		// Token: 0x060096C1 RID: 38593 RVA: 0x004650A0 File Offset: 0x004632A0
		private static string GetCharmDisplayString(GroupCharDisplayData data)
		{
			return CommonUtils.GetCharmLevelText(data.Charm, data.Gender, data.PhysiologicalAge, data.ClothDisplayId, CreatingType.IsFixedPresetType(data.CreatingType), data.FaceVisible);
		}

		// Token: 0x060096C2 RID: 38594 RVA: 0x004650E0 File Offset: 0x004632E0
		private static bool IsCharmComparable(GroupCharDisplayData data)
		{
			bool flag = !data.FaceVisible;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool isFixedCharacter = CreatingType.IsFixedPresetType(data.CreatingType);
				bool flag2 = !isFixedCharacter && data.PhysiologicalAge < 16;
				if (flag2)
				{
					result = false;
				}
				else
				{
					bool flag3 = !isFixedCharacter && data.ClothDisplayId == 0;
					result = !flag3;
				}
			}
			return result;
		}

		// Token: 0x060096C3 RID: 38595 RVA: 0x00465144 File Offset: 0x00463344
		private static int CalcHealthValue(GroupCharDisplayData data)
		{
			EHealthType type = CommonUtils.GetHealthType(data.Health, data.MaxLeftHealth, data.CharacterId);
			long ratio = (data.MaxLeftHealth > 0) ? ((long)data.Health * 10000L / (long)data.MaxLeftHealth) : 0L;
			return (int)(type * (EHealthType)20000 + (int)ratio);
		}

		// Token: 0x060096C4 RID: 38596 RVA: 0x0046519C File Offset: 0x0046339C
		private static string GetFavorDisplayString(GroupCharDisplayData data)
		{
			return CommonUtils.GetFavorStringByInteracted(data.FavorabilityToTaiwu, data.IsInteractedWithTaiwu);
		}

		// Token: 0x060096C5 RID: 38597 RVA: 0x004651C0 File Offset: 0x004633C0
		private static string GetAgeDisplayString(GroupCharDisplayData data)
		{
			bool isNonEvolutionaryType = CreatingType.IsNonEvolutionaryType(data.CreatingType);
			return (Character.Instance[data.CharacterTemplateId].HideAge && isNonEvolutionaryType) ? "-" : data.PhysiologicalAge.ToString();
		}

		// Token: 0x060096C6 RID: 38598 RVA: 0x0046520C File Offset: 0x0046340C
		private static string GetSkillGrowthString(short actualAge, sbyte growthType)
		{
			sbyte addValue = ViewCharacterMenuTeam.GetSkillGrowthAddValue(actualAge, (int)growthType);
			string growthName = (growthType == 0) ? LocalStringManager.Get("LK_Qualification_Growth_Average") : ((growthType == 1) ? LocalStringManager.Get("LK_Qualification_Growth_Precocious") : LocalStringManager.Get("LK_Qualification_Growth_LateBlooming"));
			bool flag = addValue > 0;
			string addValueStr;
			if (flag)
			{
				addValueStr = string.Format("+{0}", addValue).SetColor("lightblue");
			}
			else
			{
				bool flag2 = addValue == 0;
				if (flag2)
				{
					addValueStr = "+0".SetColor("lightgrey");
				}
				else
				{
					addValueStr = string.Format("{0}", addValue).SetColor("red");
				}
			}
			return growthName + addValueStr;
		}

		// Token: 0x060096C7 RID: 38599 RVA: 0x004652B8 File Offset: 0x004634B8
		private static int GetCommandComparableValue(GroupCharDisplayData data, int commandIndex)
		{
			bool flag = data.Command.Items == null || !data.Command.Items.CheckIndex(commandIndex);
			int result;
			if (flag)
			{
				result = int.MinValue;
			}
			else
			{
				sbyte value = data.Command.Items[commandIndex];
				result = ((value < 0) ? int.MinValue : ((int)value));
			}
			return result;
		}

		// Token: 0x060096C8 RID: 38600 RVA: 0x00465318 File Offset: 0x00463518
		private static sbyte GetSkillGrowthAddValue(short actualAge, int growthType)
		{
			AgeEffectItem ageData = AgeEffect.Instance[Math.Min((int)actualAge, AgeEffect.Instance.Count - 1)];
			return (growthType == 0) ? ageData.SkillQualificationAverage : ((growthType == 1) ? ageData.SkillQualificationPrecocious : ageData.SkillQualificationLateBlooming);
		}

		// Token: 0x060096C9 RID: 38601 RVA: 0x00465364 File Offset: 0x00463564
		private static string GetInventoryLoadString(int currLoad, int maxLoad)
		{
			string currLoadStr = ((float)currLoad / 100f).ToString("f1").SetColor(CommonUtils.GetLoadWeightValueColor(currLoad, maxLoad));
			return string.Format("{0}/{1:f1}", currLoadStr, (float)maxLoad / 100f);
		}

		// Token: 0x060096CA RID: 38602 RVA: 0x004653B0 File Offset: 0x004635B0
		private void OnSubPageChanged(TeamSubPage subPage)
		{
			bool flag = this._currentSubPage == subPage;
			if (!flag)
			{
				this._currentSubPage = subPage;
				TeamSortAndFilterController sortAndFilterController = this._sortAndFilterController;
				if (sortAndFilterController != null)
				{
					sortAndFilterController.SetSubPage(subPage);
				}
				TabSortStateManager<TeamSubPage, GroupCharDisplayData> tabSortStateManager = this._tabSortStateManager;
				if (tabSortStateManager != null)
				{
					tabSortStateManager.OnTabChange(subPage);
				}
				this.RefreshList();
			}
		}

		// Token: 0x060096CB RID: 38603 RVA: 0x00465401 File Offset: 0x00463601
		private void OnSortOrFilterChanged()
		{
			this.RefreshListData();
		}

		// Token: 0x060096CC RID: 38604 RVA: 0x0046540B File Offset: 0x0046360B
		private void OnCharacterClicked(int charId)
		{
			base.CharacterMenu.SelectCharacter(charId);
		}

		// Token: 0x04007392 RID: 29586
		private static readonly List<LanguageKey> ToggleGroupNameKeys = new List<LanguageKey>
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

		// Token: 0x04007393 RID: 29587
		private TeamSubPage _currentSubPage = TeamSubPage.State;

		// Token: 0x04007394 RID: 29588
		private List<GroupCharDisplayData> _dataList = new List<GroupCharDisplayData>();

		// Token: 0x04007395 RID: 29589
		private List<int> _originalCharIdOrder = new List<int>();

		// Token: 0x04007396 RID: 29590
		private List<GroupCharDisplayData> _filteredDataList = new List<GroupCharDisplayData>();

		// Token: 0x04007397 RID: 29591
		private TeamSortAndFilterController _sortAndFilterController;

		// Token: 0x04007398 RID: 29592
		private TabSortStateManager<TeamSubPage, GroupCharDisplayData> _tabSortStateManager;

		// Token: 0x04007399 RID: 29593
		private readonly Dictionary<TeamSubPage, RowItem> _rowTemplateCache = new Dictionary<TeamSubPage, RowItem>();

		// Token: 0x0400739A RID: 29594
		[SerializeField]
		private ListStyleGeneralScroll listScroll;

		// Token: 0x0400739B RID: 29595
		[SerializeField]
		private SortAndFilter sortAndFilter;

		// Token: 0x0400739C RID: 29596
		[SerializeField]
		private CToggleGroup subPageToggleGroup;

		// Token: 0x0400739D RID: 29597
		[SerializeField]
		private CharacterMenuLocalLoadingAnim localLoadingAnim;

		// Token: 0x0400739E RID: 29598
		[Header("行模板配置")]
		[SerializeField]
		private RowItem rowTemplate;

		// Token: 0x0400739F RID: 29599
		[SerializeField]
		private RowCellContainer singleTextCellContainer;

		// Token: 0x040073A0 RID: 29600
		[SerializeField]
		private RowCellContainer avatarAndNameCellContainer;

		// Token: 0x040073A1 RID: 29601
		[SerializeField]
		private RowCellContainer iconAndTextCellContainer;

		// Token: 0x02002254 RID: 8788
		private enum EStateColumn
		{
			// Token: 0x0400D939 RID: 55609
			Age,
			// Token: 0x0400D93A RID: 55610
			Health,
			// Token: 0x0400D93B RID: 55611
			DefeatMark,
			// Token: 0x0400D93C RID: 55612
			QiDisorder,
			// Token: 0x0400D93D RID: 55613
			Charm,
			// Token: 0x0400D93E RID: 55614
			Behavior,
			// Token: 0x0400D93F RID: 55615
			Happiness,
			// Token: 0x0400D940 RID: 55616
			Favorability,
			// Token: 0x0400D941 RID: 55617
			Alertness,
			// Token: 0x0400D942 RID: 55618
			Samsara,
			// Token: 0x0400D943 RID: 55619
			Fame,
			// Token: 0x0400D944 RID: 55620
			Count
		}

		// Token: 0x02002255 RID: 8789
		private enum EPropertyColumn
		{
			// Token: 0x0400D946 RID: 55622
			AttrStrength,
			// Token: 0x0400D947 RID: 55623
			AttrDexterity,
			// Token: 0x0400D948 RID: 55624
			AttrConcentration,
			// Token: 0x0400D949 RID: 55625
			AttrVitality,
			// Token: 0x0400D94A RID: 55626
			AttrEnergy,
			// Token: 0x0400D94B RID: 55627
			AttrIntelligence,
			// Token: 0x0400D94C RID: 55628
			PenetrateOuter,
			// Token: 0x0400D94D RID: 55629
			PenetrateInner,
			// Token: 0x0400D94E RID: 55630
			ResistOuter,
			// Token: 0x0400D94F RID: 55631
			ResistInner,
			// Token: 0x0400D950 RID: 55632
			Count
		}

		// Token: 0x02002256 RID: 8790
		private enum EProperty2Column
		{
			// Token: 0x0400D952 RID: 55634
			HitStrength,
			// Token: 0x0400D953 RID: 55635
			HitTechnique,
			// Token: 0x0400D954 RID: 55636
			HitSpeed,
			// Token: 0x0400D955 RID: 55637
			HitMind,
			// Token: 0x0400D956 RID: 55638
			AvoidStrength,
			// Token: 0x0400D957 RID: 55639
			AvoidTechnique,
			// Token: 0x0400D958 RID: 55640
			AvoidSpeed,
			// Token: 0x0400D959 RID: 55641
			AvoidMind,
			// Token: 0x0400D95A RID: 55642
			Count
		}

		// Token: 0x02002257 RID: 8791
		private enum ELifeSkillColumn
		{
			// Token: 0x0400D95C RID: 55644
			Skill0,
			// Token: 0x0400D95D RID: 55645
			Skill1,
			// Token: 0x0400D95E RID: 55646
			Skill2,
			// Token: 0x0400D95F RID: 55647
			Skill3,
			// Token: 0x0400D960 RID: 55648
			Skill4,
			// Token: 0x0400D961 RID: 55649
			Skill5,
			// Token: 0x0400D962 RID: 55650
			Skill6,
			// Token: 0x0400D963 RID: 55651
			Skill7,
			// Token: 0x0400D964 RID: 55652
			Skill8,
			// Token: 0x0400D965 RID: 55653
			Skill9,
			// Token: 0x0400D966 RID: 55654
			Skill10,
			// Token: 0x0400D967 RID: 55655
			Skill11,
			// Token: 0x0400D968 RID: 55656
			Skill12,
			// Token: 0x0400D969 RID: 55657
			Skill13,
			// Token: 0x0400D96A RID: 55658
			Skill14,
			// Token: 0x0400D96B RID: 55659
			Skill15,
			// Token: 0x0400D96C RID: 55660
			Growth,
			// Token: 0x0400D96D RID: 55661
			Count
		}

		// Token: 0x02002258 RID: 8792
		private enum ECombatSkillColumn
		{
			// Token: 0x0400D96F RID: 55663
			Skill0,
			// Token: 0x0400D970 RID: 55664
			Skill1,
			// Token: 0x0400D971 RID: 55665
			Skill2,
			// Token: 0x0400D972 RID: 55666
			Skill3,
			// Token: 0x0400D973 RID: 55667
			Skill4,
			// Token: 0x0400D974 RID: 55668
			Skill5,
			// Token: 0x0400D975 RID: 55669
			Skill6,
			// Token: 0x0400D976 RID: 55670
			Skill7,
			// Token: 0x0400D977 RID: 55671
			Skill8,
			// Token: 0x0400D978 RID: 55672
			Skill9,
			// Token: 0x0400D979 RID: 55673
			Skill10,
			// Token: 0x0400D97A RID: 55674
			Skill11,
			// Token: 0x0400D97B RID: 55675
			Skill12,
			// Token: 0x0400D97C RID: 55676
			Skill13,
			// Token: 0x0400D97D RID: 55677
			Growth,
			// Token: 0x0400D97E RID: 55678
			Count
		}

		// Token: 0x02002259 RID: 8793
		private enum EPersonalityColumn
		{
			// Token: 0x0400D980 RID: 55680
			P0,
			// Token: 0x0400D981 RID: 55681
			P1,
			// Token: 0x0400D982 RID: 55682
			P2,
			// Token: 0x0400D983 RID: 55683
			P3,
			// Token: 0x0400D984 RID: 55684
			P4,
			// Token: 0x0400D985 RID: 55685
			P5,
			// Token: 0x0400D986 RID: 55686
			P6,
			// Token: 0x0400D987 RID: 55687
			Count
		}

		// Token: 0x0200225A RID: 8794
		private enum EItemColumn
		{
			// Token: 0x0400D989 RID: 55689
			Food,
			// Token: 0x0400D98A RID: 55690
			Wood,
			// Token: 0x0400D98B RID: 55691
			Metal,
			// Token: 0x0400D98C RID: 55692
			Jade,
			// Token: 0x0400D98D RID: 55693
			Fabric,
			// Token: 0x0400D98E RID: 55694
			Herb,
			// Token: 0x0400D98F RID: 55695
			Money,
			// Token: 0x0400D990 RID: 55696
			Authority,
			// Token: 0x0400D991 RID: 55697
			InventoryLoad,
			// Token: 0x0400D992 RID: 55698
			KidnapCount,
			// Token: 0x0400D993 RID: 55699
			Count
		}

		// Token: 0x0200225B RID: 8795
		private enum ECommandColumn
		{
			// Token: 0x0400D995 RID: 55701
			AttackMedal,
			// Token: 0x0400D996 RID: 55702
			DefenceMedal,
			// Token: 0x0400D997 RID: 55703
			WisdomMedal,
			// Token: 0x0400D998 RID: 55704
			Command0,
			// Token: 0x0400D999 RID: 55705
			Command1,
			// Token: 0x0400D99A RID: 55706
			Command2,
			// Token: 0x0400D99B RID: 55707
			Count
		}

		// Token: 0x0200225C RID: 8796
		private enum EMedalType
		{
			// Token: 0x0400D99D RID: 55709
			Attack,
			// Token: 0x0400D99E RID: 55710
			Defence,
			// Token: 0x0400D99F RID: 55711
			Wisdom
		}
	}
}
