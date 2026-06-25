using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using Game.Components.Character;
using Game.Components.Common;
using Game.Components.ListStyleGeneralScroll;
using Game.Components.SortAndFilter;
using Game.Components.SortAndFilter.Relationship;
using Game.Views.CharacterMenu.Relationship;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Extra;
using GameData.Domains.Global;
using GameData.Domains.Map;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.CharacterMenu
{
	// Token: 0x02000BAC RID: 2988
	public class ViewCharacterMenuRelationship : UI_CharacterMenuSubPageBase
	{
		// Token: 0x1700102C RID: 4140
		// (get) Token: 0x06009634 RID: 38452 RVA: 0x00461551 File Offset: 0x0045F751
		private bool CurrentSelectDreamBack
		{
			get
			{
				return this._isCurrentTaiwuOverwrittenByDreamBack && this.toggleGroupPeriod.gameObject.activeSelf && this.toggleGroupPeriod.GetActiveIndex() == 1;
			}
		}

		// Token: 0x1700102D RID: 4141
		// (get) Token: 0x06009635 RID: 38453 RVA: 0x0046157E File Offset: 0x0045F77E
		public override LanguageKey TitleKey
		{
			get
			{
				return LanguageKey.LK_CharacterMenu_Title_RelationShip_Title;
			}
		}

		// Token: 0x06009636 RID: 38454 RVA: 0x00461588 File Offset: 0x0045F788
		public override bool CheckState(ECharacterSubToggleBase curSubTogglePage, ECharacterSubPage curSubPage)
		{
			return curSubTogglePage == ECharacterSubToggleBase.RelationshipBase && curSubPage == ECharacterSubPage.Relationship;
		}

		// Token: 0x06009637 RID: 38455 RVA: 0x004615A8 File Offset: 0x0045F7A8
		public override void OnInit(ArgumentBox argsBox)
		{
			this._isInitDreamBack = false;
			this.toggleGroupPeriod.SetWithoutNotify(0);
			this.NeedDataListenerId = true;
			UIElement element = this.Element;
			element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(delegate()
			{
				this.localLoadingAnim.SetLoadingState(true);
				ExtraDomainMethod.AsyncCall.IsCurrentTaiwuOverwrittenByDreamBack(this, delegate(int offset, RawDataPool dataPool)
				{
					Serializer.Deserialize(dataPool, offset, ref this._isCurrentTaiwuOverwrittenByDreamBack);
					this._isInitDreamBack = true;
					this.RequestData();
				});
			}));
		}

		// Token: 0x06009638 RID: 38456 RVA: 0x004615F8 File Offset: 0x0045F7F8
		private void RequestData()
		{
			bool flag = !this._isInitDreamBack;
			if (!flag)
			{
				int selfCharacterId = base.CharacterMenu.CurCharacterId;
				bool flag2 = selfCharacterId < 0;
				if (!flag2)
				{
					base.StopAllCoroutines();
					this._potentialSuccessorCharId = -1;
					this._potentialSuccessorData = null;
					this._selfDisplayData = null;
					this._hasOrganizationMemberPotentialSuccessor = false;
					this.SetCharacterSuccessorVisible(false);
					this.localLoadingAnim.SetLoadingState(true);
					this.ChangeViewShow(this._isCurrentTaiwuOverwrittenByDreamBack && base.CharacterMenu.CurrentCharacterIsTaiwu);
					this.dropdownModeSwitch.onValueChanged.RemoveAllListeners();
					AsyncMethodCallbackDelegate <>9__1;
					AsyncMethodCallbackDelegate <>9__2;
					CharacterDomainMethod.AsyncCall.GetCharacterDisplayDataList(this, new List<int>
					{
						selfCharacterId
					}, delegate(int offset, RawDataPool dataPool)
					{
						List<CharacterDisplayData> displays = null;
						Serializer.Deserialize(dataPool, offset, ref displays);
						CharacterDisplayData charData = displays[0];
						this._selfDisplayData = charData;
						this._hasOrganizationMemberPotentialSuccessor = CommonUtils.ShouldDisplayOrganizationMemberPotentialSuccessor(charData);
						this._potentialSuccessorCharId = (this._hasOrganizationMemberPotentialSuccessor ? charData.OrganizationMemberPotentialSuccessor.CharacterId : -1);
						this.graphViewSelf.Set(charData, this.CharacterMenu.IsTaiwu(charData.CharacterId), false);
						this.SetCharacterTips(this.graphViewSelf.gameObject.GetOrAddComponent<TooltipInvoker>(), charData.CharacterId);
						bool flag3 = this.IsCurrentSelectDreamBack();
						if (flag3)
						{
							IAsyncMethodRequestHandler <>4__this = this;
							AsyncMethodCallbackDelegate callback;
							if ((callback = <>9__1) == null)
							{
								callback = (<>9__1 = delegate(int offset2, RawDataPool dataPool2)
								{
									this.StartCoroutine(this.Prepare(offset2, dataPool2));
								});
							}
							ExtraDomainMethod.AsyncCall.GetDreamBackTaiwuRelatedCharactersForRelations(<>4__this, callback);
						}
						else
						{
							IAsyncMethodRequestHandler <>4__this2 = this;
							int selfCharacterId = selfCharacterId;
							AsyncMethodCallbackDelegate callback2;
							if ((callback2 = <>9__2) == null)
							{
								callback2 = (<>9__2 = delegate(int offset2, RawDataPool dataPool2)
								{
									this.StartCoroutine(this.Prepare(offset2, dataPool2));
								});
							}
							CharacterDomainMethod.AsyncCall.GetRelatedCharactersForRelations(<>4__this2, selfCharacterId, callback2);
						}
					});
				}
			}
		}

		// Token: 0x06009639 RID: 38457 RVA: 0x004616D3 File Offset: 0x0045F8D3
		public override void OnCurrentCharacterChange(int prevCharacterId)
		{
			this.RequestData();
		}

		// Token: 0x0600963A RID: 38458 RVA: 0x004616E0 File Offset: 0x0045F8E0
		private void Awake()
		{
			this.InitRelationshipToggleGroup();
			this.dropdownModeSwitch.ClearOptions();
			this.dropdownModeSwitch.AddOptions(new List<string>
			{
				LanguageKey.LK_ViewMode_RelationGraph.Tr(),
				LanguageKey.LK_ViewMode_List.Tr()
			});
			this.dropdownModeSwitch.value = 0;
			this.RefreshGroupTitles();
			PoolManager.SetSrcObjectWithTurnOff("ViewCharacterMenuRelationshipgraphViewTemplateCharacter", this.graphViewTemplateCharacter);
			this.InitSortAndFilter();
			this.localLoadingAnim.SetLoadingState(true);
			GEvent.Add(UiEvents.OnLanguageChange, new GEvent.Callback(this.OnLanguageChange));
		}

		// Token: 0x0600963B RID: 38459 RVA: 0x00461789 File Offset: 0x0045F989
		private void OnEnable()
		{
			GlobalDomainMethod.Call.InvokeGuidingTrigger(123);
		}

		// Token: 0x0600963C RID: 38460 RVA: 0x00461794 File Offset: 0x0045F994
		private void InitRelationshipToggleGroup()
		{
			this.toggleGroupRelationshipType.Init(-1);
			ToggleGroupHotkeyController.Set(this.Element, this.toggleGroupRelationshipType, 2, null);
			this.toggleGroupRelationshipType.OnActiveIndexChange += this.OnToggleGroupRelationshipTypeChange;
			this.toggleGroupPeriod.Init(-1);
			ToggleGroupHotkeyController.Set(this.Element, this.toggleGroupPeriod, 1, null);
			this.toggleGroupPeriod.OnActiveIndexChange += this.OnToggleGroupPeriodChange;
			this.toggleGroupPeriod.gameObject.SetActive(false);
		}

		// Token: 0x0600963D RID: 38461 RVA: 0x00461826 File Offset: 0x0045FA26
		private void OnToggleGroupRelationshipTypeChange(int _, int __)
		{
			this._selectedGroup = (ViewCharacterMenuRelationship.EGroup)(this.toggleGroupRelationshipType.GetActiveIndex() - 1);
			this.OnSortOrFilterChanged();
			this.UpdateGraph();
		}

		// Token: 0x0600963E RID: 38462 RVA: 0x0046184A File Offset: 0x0045FA4A
		private void OnToggleGroupPeriodChange(int _, int __)
		{
			this.RequestData();
		}

		// Token: 0x0600963F RID: 38463 RVA: 0x00461854 File Offset: 0x0045FA54
		private void InitSortAndFilter()
		{
			bool flag = this.sortAndFilter == null;
			if (!flag)
			{
				this._sortAndFilterController = new RelationshipSortAndFilterController(this.sortAndFilter, new Func<int, bool>(base.CharacterMenu.IsTaiwu), new Func<int, bool>(base.CharacterMenu.IsTaiwuSpecialTeammate));
				this._sortAndFilterController.Init(new Action(this.OnSortOrFilterChanged), "RelationshipSort");
				bool flag2 = this.listViewStyleGeneralScroll != null;
				if (flag2)
				{
					this.listViewStyleGeneralScroll.SetSortController(this._sortAndFilterController);
				}
				this._tabSortStateManager = new TabSortStateManager<ViewCharacterMenuRelationship.EGroup, CharacterDisplayDataForRelations>(this._sortAndFilterController);
			}
		}

		// Token: 0x06009640 RID: 38464 RVA: 0x004618FC File Offset: 0x0045FAFC
		private void OnSortOrFilterChanged()
		{
			bool flag = this._selectedGroup == ViewCharacterMenuRelationship.EGroup.Max;
			if (!flag)
			{
				bool flag2 = this.dropdownModeSwitch.value != 1;
				if (!flag2)
				{
					CharacterDisplayDataForGeneralScrollListUtils.SubPage listSubPage = CharacterDisplayDataForGeneralScrollListUtils.SubPage.State;
					this.RefreshList(this._selectedGroup, listSubPage);
				}
			}
		}

		// Token: 0x06009641 RID: 38465 RVA: 0x00461944 File Offset: 0x0045FB44
		private void RefreshList(ViewCharacterMenuRelationship.EGroup group, CharacterDisplayDataForGeneralScrollListUtils.SubPage listSubPage)
		{
			bool flag = group == ViewCharacterMenuRelationship.EGroup.Max || this._relatedChars == null;
			if (!flag)
			{
				IEnumerable<CharacterDisplayDataForRelations> source;
				if (group != ViewCharacterMenuRelationship.EGroup.All)
				{
					IEnumerable<CharacterDisplayDataForRelations> dataListForList = this.GetDataListForList(group);
					source = dataListForList;
				}
				else
				{
					source = this._relatedChars.Values.SelectMany((List<CharacterDisplayDataForRelations> pair) => pair);
				}
				this.RefreshListData(ViewCharacterMenuRelationship.ValidRelatedChar(source), listSubPage, delegate(int charId)
				{
					CharacterDisplayDataForRelations data = this._relatedChars.Values.SelectMany((List<CharacterDisplayDataForRelations> pair) => pair).First((CharacterDisplayDataForRelations d) => d.CharacterId == charId);
					this.OnCharacterClicked(data);
				}, new Action<TooltipInvoker, int>(this.SetCharacterTips));
			}
		}

		// Token: 0x06009642 RID: 38466 RVA: 0x004619CC File Offset: 0x0045FBCC
		private List<CharacterDisplayDataForRelations> GetDataListForList(ViewCharacterMenuRelationship.EGroup group)
		{
			List<CharacterDisplayDataForRelations> res = new List<CharacterDisplayDataForRelations>();
			List<CharacterDisplayDataForRelations> groupList;
			bool flag = this._relatedChars.TryGetValue((int)group, out groupList);
			if (flag)
			{
				res.AddRange(groupList);
			}
			bool flag2 = this._relatedChars.TryGetValue((int)(-(int)group), out groupList);
			if (flag2)
			{
				res.AddRange(groupList);
			}
			return res;
		}

		// Token: 0x06009643 RID: 38467 RVA: 0x00461A1C File Offset: 0x0045FC1C
		private void OnCharacterClicked(CharacterDisplayDataForRelations charData)
		{
			bool flag = charData.LifeState == 1 && charData.Location.IsValid();
			if (flag)
			{
				ViewLifeRecords.Show(charData.CharacterId);
			}
			else
			{
				bool flag2 = ViewCharacterMenuGenealogy.CanJumpCharacter(base.CharacterMenu, charData, this.IsCurrentSelectDreamBack());
				if (flag2)
				{
					string fullName = NameCenter.GetMonasticTitleOrDisplayName(ref charData.Main.NameData, base.CharacterMenu.IsTaiwu(charData.CharacterId), false);
					string relationText = this.GetRelationText(charData.CharacterId);
					ViewCharacterMenuGenealogy.JumpCharacterCallback(base.CharacterMenu, charData, fullName, relationText);
				}
			}
		}

		// Token: 0x06009644 RID: 38468 RVA: 0x00461AAC File Offset: 0x0045FCAC
		private string GetRelationText(int charId)
		{
			bool flag = this._relatedChars == null;
			string result;
			if (flag)
			{
				result = null;
			}
			else
			{
				Func<CharacterDisplayDataForRelations, bool> <>9__0;
				foreach (KeyValuePair<int, List<CharacterDisplayDataForRelations>> kv in this._relatedChars)
				{
					ViewCharacterMenuRelationship.EGroup group = (ViewCharacterMenuRelationship.EGroup)Math.Abs(kv.Key);
					bool flag2 = group == ViewCharacterMenuRelationship.EGroup.All || group == ViewCharacterMenuRelationship.EGroup.Max;
					if (!flag2)
					{
						IEnumerable<CharacterDisplayDataForRelations> value = kv.Value;
						Func<CharacterDisplayDataForRelations, bool> predicate;
						if ((predicate = <>9__0) == null)
						{
							predicate = (<>9__0 = ((CharacterDisplayDataForRelations c) => c.CharacterId == charId));
						}
						bool flag3 = value.Any(predicate);
						if (flag3)
						{
							bool flag4 = group == ViewCharacterMenuRelationship.EGroup.Faction;
							if (flag4)
							{
								return LocalStringManager.Get(LanguageKey.LK_Faction);
							}
							return LocalStringManager.Get(string.Format("LK_RelationShip_{0}", group));
						}
					}
				}
				result = null;
			}
			return result;
		}

		// Token: 0x06009645 RID: 38469 RVA: 0x00461BB0 File Offset: 0x0045FDB0
		private new void OnDisable()
		{
			this._played = false;
			TabSortStateManager<ViewCharacterMenuRelationship.EGroup, CharacterDisplayDataForRelations> tabSortStateManager = this._tabSortStateManager;
			if (tabSortStateManager != null)
			{
				tabSortStateManager.ClearAll();
			}
		}

		// Token: 0x06009646 RID: 38470 RVA: 0x00461BCC File Offset: 0x0045FDCC
		private void OnDestroy()
		{
			GEvent.Remove(UiEvents.OnLanguageChange, new GEvent.Callback(this.OnLanguageChange));
			PoolManager.RemoveData("ViewCharacterMenuRelationshipgraphViewTemplateCharacter");
		}

		// Token: 0x06009647 RID: 38471 RVA: 0x00461BF8 File Offset: 0x0045FDF8
		private void RefreshListData(IEnumerable<CharacterDisplayDataForRelations> dataList, CharacterDisplayDataForGeneralScrollListUtils.SubPage selectedSubPage, Action<int> onCharacterClicked, Action<TooltipInvoker, int> mouseTipModifier)
		{
			this.<RefreshListData>g__PrepareRowTemplateContainers|57_4(selectedSubPage);
			this.listViewStyleGeneralScroll.ClearInfinityScrollCache();
			this.listViewStyleGeneralScroll.Init<CharacterDisplayDataForRelations>(CharacterDisplayDataForGeneralScrollListUtils.GenerateColumnDefinitions<CharacterDisplayDataForRelations>(selectedSubPage, (CharacterDisplayDataForRelations data) => data.Main, onCharacterClicked, mouseTipModifier), true, null, null);
			List<CharacterDisplayDataForRelations> dataItems = (from data in dataList
			group data by data.CharacterId into @group
			select @group.First<CharacterDisplayDataForRelations>()).ToList<CharacterDisplayDataForRelations>();
			RelationshipSortAndFilterController sortAndFilterController = this._sortAndFilterController;
			Func<CharacterDisplayDataForRelations, bool> func;
			if ((func = ((sortAndFilterController != null) ? sortAndFilterController.GenerateFilter() : null)) == null && (func = ViewCharacterMenuRelationship.<>c.<>9__57_3) == null)
			{
				func = (ViewCharacterMenuRelationship.<>c.<>9__57_3 = ((CharacterDisplayDataForRelations _) => true));
			}
			Func<CharacterDisplayDataForRelations, bool> filter = func;
			RelationshipSortAndFilterController sortAndFilterController2 = this._sortAndFilterController;
			Comparison<CharacterDisplayDataForRelations> comparer = (sortAndFilterController2 != null) ? sortAndFilterController2.GenerateComparer(dataItems) : null;
			bool flag;
			if (comparer != null)
			{
				TabSortStateManager<ViewCharacterMenuRelationship.EGroup, CharacterDisplayDataForRelations> tabSortStateManager = this._tabSortStateManager;
				flag = (tabSortStateManager != null && tabSortStateManager.ShouldSort());
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			if (flag2)
			{
				dataItems.Sort(comparer);
			}
			this.listViewStyleGeneralScroll.SetData<CharacterDisplayDataForRelations>(dataItems.Where(filter), -1);
		}

		// Token: 0x06009648 RID: 38472 RVA: 0x00461D28 File Offset: 0x0045FF28
		private void OnLanguageChange(ArgumentBox argumentBox)
		{
			this.OnLanguageChange(LocalStringManager.CurLanguageType);
			this.RefreshGroupTitles();
			bool hasOrganizationMemberPotentialSuccessor = this._hasOrganizationMemberPotentialSuccessor;
			if (hasOrganizationMemberPotentialSuccessor)
			{
				this.RefreshCharacterSuccessor();
			}
		}

		// Token: 0x06009649 RID: 38473 RVA: 0x00461D5C File Offset: 0x0045FF5C
		private void RefreshGroupTitles()
		{
			for (int i = 0; i < this.graphViewGroupConfiguration.Length; i++)
			{
				RelationshipGroup item = (i < this.graphViewTemplateGroupContainer.childCount) ? this.graphViewTemplateGroupContainer.GetChild(i).GetComponent<RelationshipGroup>() : Object.Instantiate<GameObject>(this.graphViewTemplateGroup.gameObject, this.graphViewTemplateGroupContainer).GetComponent<RelationshipGroup>();
				item.SetIconAndName(this.graphViewGroupConfiguration[i]);
			}
		}

		// Token: 0x0600964A RID: 38474 RVA: 0x00461DD3 File Offset: 0x0045FFD3
		private void UpdatePagesVisible()
		{
			this.UpdatePagesVisible(this.dropdownModeSwitch.value);
		}

		// Token: 0x0600964B RID: 38475 RVA: 0x00461DE8 File Offset: 0x0045FFE8
		private void UpdatePagesVisible(int idx)
		{
			bool isActiveAndEnabled = this.loadingAnimation.isActiveAndEnabled;
			if (isActiveAndEnabled)
			{
				idx = -1;
			}
			for (int i = 0; i < this.rootViewModes.Length; i++)
			{
				GameObject pageRoot = this.rootViewModes[i].gameObject;
				pageRoot.SetActive(i == idx);
				pageRoot.GetOrAddComponent<RectMask2D>().enabled = true;
			}
		}

		// Token: 0x0600964C RID: 38476 RVA: 0x00461E48 File Offset: 0x00460048
		private bool IsCurrentSelectDreamBack()
		{
			return this.CurrentSelectDreamBack && base.CharacterMenu.CurrentCharacterIsTaiwu;
		}

		// Token: 0x0600964D RID: 38477 RVA: 0x00461E70 File Offset: 0x00460070
		private void ChangeViewShow(bool isDreamBack)
		{
			this.toggleGroupPeriod.gameObject.SetActive(isDreamBack);
		}

		// Token: 0x0600964E RID: 38478 RVA: 0x00461E85 File Offset: 0x00460085
		protected override void SetLoadingState(bool loading)
		{
			base.SetLoadingState(loading);
			this.UpdatePagesVisible();
		}

		// Token: 0x0600964F RID: 38479 RVA: 0x00461E97 File Offset: 0x00460097
		private IEnumerator Prepare(int offset, RawDataPool dataPool)
		{
			ViewCharacterMenuRelationship.<>c__DisplayClass67_0 CS$<>8__locals1 = new ViewCharacterMenuRelationship.<>c__DisplayClass67_0();
			CS$<>8__locals1.<>4__this = this;
			RelatedCharactersForRelations relatedCharactersForRelations = null;
			Serializer.Deserialize(dataPool, offset, ref relatedCharactersForRelations);
			CS$<>8__locals1.relatedChars = new Dictionary<int, List<CharacterDisplayDataForRelations>>();
			this._selectedGroup = ViewCharacterMenuRelationship.EGroup.Max;
			CharacterSet[] groupLists = new CharacterSet[]
			{
				relatedCharactersForRelations.Parents,
				relatedCharactersForRelations.SwornBrothersAndSisters,
				relatedCharactersForRelations.HusbandsAndWives,
				relatedCharactersForRelations.Children,
				relatedCharactersForRelations.FactionMembers,
				relatedCharactersForRelations.Enemies,
				relatedCharactersForRelations.Friends,
				relatedCharactersForRelations.Adored,
				relatedCharactersForRelations.Mentors,
				relatedCharactersForRelations.BrothersAndSisters,
				relatedCharactersForRelations.RelatedAdored,
				relatedCharactersForRelations.RelatedEnemies
			};
			int factionLeaderId = relatedCharactersForRelations.FactionLeaderId;
			CS$<>8__locals1.lastIndex = -1;
			int num;
			for (int i = 0; i < groupLists.Length; i = num + 1)
			{
				bool flag = groupLists[i].GetCount() > 0;
				if (flag)
				{
					CS$<>8__locals1.lastIndex = i;
				}
				num = i;
			}
			bool flag2 = CS$<>8__locals1.lastIndex >= 0;
			if (flag2)
			{
				for (int j = 0; j < groupLists.Length; j = num + 1)
				{
					ViewCharacterMenuRelationship.<>c__DisplayClass67_1 CS$<>8__locals2 = new ViewCharacterMenuRelationship.<>c__DisplayClass67_1();
					CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
					CS$<>8__locals2.group = j;
					List<int> list = new List<int>();
					list.AddRange(groupLists[j].GetCollection());
					bool flag3 = CS$<>8__locals2.group == 4 && factionLeaderId >= 0;
					if (flag3)
					{
						list.Add(factionLeaderId);
					}
					CS$<>8__locals2.idx = CS$<>8__locals2.group;
					bool flag4 = CS$<>8__locals2.group == groupLists.Length - 1;
					if (flag4)
					{
						CS$<>8__locals2.idx = -5;
					}
					else
					{
						bool flag5 = CS$<>8__locals2.group == groupLists.Length - 2;
						if (flag5)
						{
							CS$<>8__locals2.idx = -7;
						}
					}
					bool flag6 = list.Count > 0;
					if (flag6)
					{
						bool flag7 = this.IsCurrentSelectDreamBack();
						if (flag7)
						{
							ExtraDomainMethod.AsyncCall.GetCharacterDisplayDataListForDreamBackRelations(this, list, delegate(int offset2, RawDataPool dataPool2)
							{
								List<CharacterDisplayDataForRelations> ret = new List<CharacterDisplayDataForRelations>();
								Serializer.Deserialize(dataPool2, offset2, ref ret);
								CS$<>8__locals2.CS$<>8__locals1.relatedChars[CS$<>8__locals2.idx] = ret;
								bool flag8 = CS$<>8__locals2.group == CS$<>8__locals2.CS$<>8__locals1.lastIndex;
								if (flag8)
								{
									CS$<>8__locals2.CS$<>8__locals1.<>4__this.Refresh(CS$<>8__locals2.CS$<>8__locals1.relatedChars);
								}
							});
						}
						else
						{
							CharacterDomainMethod.AsyncCall.GetCharacterDisplayDataListForRelationsWithRelationType(this, base.CharacterMenu.CurCharacterId, list, delegate(int offset2, RawDataPool dataPool2)
							{
								List<CharacterDisplayDataForRelations> ret = new List<CharacterDisplayDataForRelations>();
								Serializer.Deserialize(dataPool2, offset2, ref ret);
								CS$<>8__locals2.CS$<>8__locals1.relatedChars[CS$<>8__locals2.idx] = ret;
								bool flag8 = CS$<>8__locals2.group == CS$<>8__locals2.CS$<>8__locals1.lastIndex;
								if (flag8)
								{
									CS$<>8__locals2.CS$<>8__locals1.<>4__this.Refresh(CS$<>8__locals2.CS$<>8__locals1.relatedChars);
								}
							});
						}
					}
					yield return new WaitForEndOfFrame();
					CS$<>8__locals2 = null;
					list = null;
					num = j;
				}
			}
			else
			{
				this.Refresh(CS$<>8__locals1.relatedChars);
			}
			yield break;
		}

		// Token: 0x06009650 RID: 38480 RVA: 0x00461EB4 File Offset: 0x004600B4
		private IEnumerator RefreshProcess(IDictionary<int, List<CharacterDisplayDataForRelations>> relatedChars)
		{
			ViewCharacterMenuRelationship.<>c__DisplayClass69_0 CS$<>8__locals1 = new ViewCharacterMenuRelationship.<>c__DisplayClass69_0();
			CS$<>8__locals1.<>4__this = this;
			this._relatedChars = relatedChars;
			CS$<>8__locals1.tG = this.toggleGroupRelationshipType.gameObject.GetOrAddComponent<CToggleGroup>();
			this.localLoadingAnim.SetLoadingState(true);
			CS$<>8__locals1.lastIdx = null;
			this.dropdownModeSwitch.onValueChanged.ResetListener(delegate(int idx)
			{
				base.<RefreshProcess>g__UpdateView|2(idx, CS$<>8__locals1.<>4__this._selectedGroup);
			});
			foreach (GameObject obj in this._pooledCharObjects)
			{
				Transform tr = obj.transform;
				tr.SetParent(null);
				Object.Destroy(obj);
				tr = null;
				obj = null;
			}
			HashSet<GameObject>.Enumerator enumerator = default(HashSet<GameObject>.Enumerator);
			this._pooledCharObjects.Clear();
			yield return new WaitForEndOfFrame();
			Rect contentRange = default(Rect);
			int num;
			for (int i = 0; i < this.graphViewGroupConfiguration.Length; i = num + 1)
			{
				ViewCharacterMenuRelationship.<>c__DisplayClass69_1 CS$<>8__locals2 = new ViewCharacterMenuRelationship.<>c__DisplayClass69_1();
				CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
				ViewCharacterMenuRelationship.EGroup group = this.graphViewGroupConfiguration[i].group;
				int dataIndex = (int)group;
				RelationshipGroup item = this.graphViewTemplateGroup.parent.GetChild(i).GetComponent<RelationshipGroup>();
				Refers refers = item.GetComponent<Refers>();
				CS$<>8__locals2.groupReverseSource = null;
				CS$<>8__locals2.groupListFull = new List<CharacterDisplayDataForRelations>();
				HashSet<int> ids = new HashSet<int>();
				bool flag = group == ViewCharacterMenuRelationship.EGroup.Adored || group == ViewCharacterMenuRelationship.EGroup.Enemy;
				if (flag)
				{
					CS$<>8__locals2.groupReverseSource = new List<CharacterDisplayDataForRelations>();
					List<CharacterDisplayDataForRelations> dataListReverse;
					bool flag2 = relatedChars.TryGetValue((int)(-(int)group), out dataListReverse);
					if (flag2)
					{
						foreach (CharacterDisplayDataForRelations data in dataListReverse)
						{
							bool flag3 = data.Main != null && data.Main.CreatingType != 3;
							if (flag3)
							{
								CS$<>8__locals2.groupReverseSource.Add(data);
								ids.Add(data.CharacterId);
							}
							data = null;
						}
						List<CharacterDisplayDataForRelations>.Enumerator enumerator2 = default(List<CharacterDisplayDataForRelations>.Enumerator);
					}
					CS$<>8__locals2.groupListFull.AddRange(CS$<>8__locals2.groupReverseSource);
					dataListReverse = null;
				}
				bool flag4 = relatedChars.TryGetValue(dataIndex, out CS$<>8__locals2.dataList);
				if (flag4)
				{
					foreach (CharacterDisplayDataForRelations data2 in CS$<>8__locals2.dataList)
					{
						bool flag5 = !ids.Contains(data2.CharacterId) && data2.Main != null && data2.Main.CreatingType != 3;
						if (flag5)
						{
							CS$<>8__locals2.groupListFull.Add(data2);
							ids.Add(data2.CharacterId);
						}
						data2 = null;
					}
					List<CharacterDisplayDataForRelations>.Enumerator enumerator3 = default(List<CharacterDisplayDataForRelations>.Enumerator);
				}
				else
				{
					CS$<>8__locals2.dataList = null;
				}
				CS$<>8__locals2.groupStyle = this.graphViewGroupStyle[0];
				foreach (ViewCharacterMenuRelationship.GraphViewGroupStyle style in this.graphViewGroupStyle)
				{
					CS$<>8__locals2.groupStyle = style;
					bool flag6 = CS$<>8__locals2.groupListFull.Count <= style.MemberCapacity;
					if (flag6)
					{
						break;
					}
					style = default(ViewCharacterMenuRelationship.GraphViewGroupStyle);
				}
				ViewCharacterMenuRelationship.GraphViewGroupStyle[] array = null;
				ValueTuple<Vector3, Vector2> valueTuple = item.SetBack(CS$<>8__locals2.groupStyle.scale, 360f / (float)this.graphViewGroupConfiguration.Length, i);
				Vector3 localPosition = valueTuple.Item1;
				Vector2 area = valueTuple.Item2;
				yield return new WaitForEndOfFrame();
				CS$<>8__locals2.leftIsSelf = (localPosition.x > 0f);
				item.SetLink(-localPosition);
				yield return new WaitForEndOfFrame();
				CS$<>8__locals2.groupListFiltered = new List<CharacterDisplayDataForRelations>();
				CS$<>8__locals2.groupListCurrent = new List<CharacterDisplayDataForRelations>();
				CS$<>8__locals2.pageMemberCapacity = CS$<>8__locals2.groupStyle.MemberCapacity;
				CS$<>8__locals2.pageCount = (int)Mathf.Ceil(1f * (float)CS$<>8__locals2.groupListFull.Count / (float)CS$<>8__locals2.pageMemberCapacity);
				bool flag7 = refers.CTryGet<RectTransform>("Root", out CS$<>8__locals2.groupRoot);
				if (flag7)
				{
					int indexToCircle = 0;
					int indexInCircle = 0;
					Vector2 childSize = this.graphViewTemplateCharacter.GetComponent<RectTransform>().rect.size;
					for (int j = 0; j < CS$<>8__locals2.pageMemberCapacity; j = num + 1)
					{
						Refers child = PoolManager.GetObject("ViewCharacterMenuRelationshipgraphViewTemplateCharacter").GetComponent<Refers>();
						this._pooledCharObjects.Add(child.gameObject);
						RectTransform childRect = child.RectTransform;
						childRect.SetParent(CS$<>8__locals2.groupRoot);
						int capacityInCircle = CS$<>8__locals2.groupStyle.memberCountPerCircle[indexToCircle];
						float angleDelta = 360f / (float)capacityInCircle;
						float angle = (90f + (float)indexInCircle * angleDelta) * 0.017453292f;
						int circleInner = CS$<>8__locals2.groupStyle.memberCountPerCircle.Length - 1 - indexToCircle;
						Vector2 radius = new Vector2(area.x / 2f - childSize.x / 2f - (childSize.x + CS$<>8__locals2.groupStyle.circlePadding) * (float)circleInner, area.y / 2f - childSize.y / 2f - (childSize.y + CS$<>8__locals2.groupStyle.circlePadding) * (float)circleInner);
						childRect.localScale = Vector3.one;
						childRect.anchoredPosition3D = ((radius.x < 0f || radius.y < 0f) ? Vector3.zero : ViewCharacterMenuRelationship.<RefreshProcess>g__ProtectedVector3|69_7(Mathf.Cos(angle) * radius.x, Mathf.Sin(angle) * radius.y));
						yield return new WaitForEndOfFrame();
						indexInCircle++;
						bool flag8 = indexInCircle >= capacityInCircle;
						if (flag8)
						{
							indexInCircle = 0;
							indexToCircle++;
						}
						num = j;
					}
					childSize = default(Vector2);
				}
				bool flag9 = refers.CTryGet<Refers>("Detail", out CS$<>8__locals2.detail);
				if (flag9)
				{
					Transform transform = CS$<>8__locals2.detail.transform;
					RectTransform detailRect = transform as RectTransform;
					CImage iconFrame;
					bool flag10 = detailRect != null && CS$<>8__locals2.detail.CTryGet<CImage>("IconFrame", out iconFrame);
					if (flag10)
					{
						bool centeredDetail = CS$<>8__locals2.groupStyle.centeredDetail;
						if (centeredDetail)
						{
							Vector2 center = Vector2.one * 0.5f;
							detailRect.SetAnchor(center, center);
							center = default(Vector2);
						}
						else
						{
							Vector2 bottom = new Vector2(0.5f, 0f);
							detailRect.SetAnchor(bottom, bottom);
							bottom = default(Vector2);
						}
						detailRect.anchoredPosition = Vector2.zero;
					}
					Refers operations;
					bool flag11 = CS$<>8__locals2.detail.CTryGet<Refers>("Operations", out operations);
					if (flag11)
					{
						CDropdown opFilter = operations.CGet<CDropdown>("Alive");
						opFilter.gameObject.SetActive(CS$<>8__locals2.pageCount > 1);
						opFilter.ClearOptions();
						opFilter.AddOptions(new List<string>
						{
							LanguageKey.LK_RelationShip_All.Tr(),
							LanguageKey.LK_RelationShipGroupFilterMode_Alive.Tr(),
							LanguageKey.LK_RelationShipGroupFilterMode_Dead.Tr()
						});
						opFilter.onValueChanged.ResetListener(new Action<int>(CS$<>8__locals2.<RefreshProcess>g__UpdateFilter|3));
						opFilter = null;
					}
					detailRect = null;
					iconFrame = null;
					operations = null;
				}
				CS$<>8__locals2.<RefreshProcess>g__UpdateFilter|3(0);
				yield return new WaitForEndOfFrame();
				RectTransform viewport = this.graphViewScrollRect.viewport;
				bool flag12 = viewport != null;
				if (flag12)
				{
					MouseWheelScale wheelScale = this.graphViewScrollRect.content.GetComponent<MouseWheelScale>();
					float scale = 1f / wheelScale.Min.x;
					float ratio = viewport.rect.width / viewport.rect.height;
					float bdrX = viewport.rect.width * scale;
					float bdrY = viewport.rect.height * scale;
					float extX = (ratio > 0f) ? ratio : scale;
					float extY = (ratio < 0f) ? (scale / ratio) : scale;
					contentRange.xMin = Mathf.Min(contentRange.xMin, localPosition.x - area.x * extX);
					contentRange.xMax = Mathf.Max(contentRange.xMax, localPosition.x + area.x * extX);
					contentRange.yMin = Mathf.Min(contentRange.yMin, localPosition.y - area.y * extY);
					contentRange.yMax = Mathf.Max(contentRange.yMax, localPosition.y + area.y * extY);
					contentRange.xMin = Mathf.Min(contentRange.xMin, -bdrX);
					contentRange.xMax = Mathf.Max(contentRange.xMax, bdrX);
					contentRange.yMin = Mathf.Min(contentRange.yMin, -bdrY);
					contentRange.yMax = Mathf.Max(contentRange.yMax, bdrY);
					wheelScale = null;
				}
				num = i;
			}
			RectTransform content = this.graphViewScrollRect.content;
			bool flag13 = content != null;
			if (flag13)
			{
				content.sizeDelta = contentRange.size;
				content.localScale = Vector3.one;
				content.localPosition = Vector3.zero;
				this.graphViewScrollRect.normalizedPosition = Vector2.one * 0.5f;
				yield return new WaitForEndOfFrame();
				MouseWheelScale wheelScale2 = content.GetComponent<MouseWheelScale>();
				yield return new WaitForEndOfFrame();
				wheelScale2.Reset();
				wheelScale2.ScaleProcess(0f);
				yield return new WaitForEndOfFrame();
				wheelScale2 = null;
			}
			yield return new WaitForEndOfFrame();
			this.RefreshList(ViewCharacterMenuRelationship.EGroup.All, CharacterDisplayDataForGeneralScrollListUtils.SubPage.State);
			CS$<>8__locals1.<RefreshProcess>g__ResetTg|1();
			yield return new WaitForEndOfFrame();
			int showModeIdx = this.dropdownModeSwitch.value;
			for (int k = 0; k < this.rootViewModes.Length; k = num + 1)
			{
				GameObject pageRoot = this.rootViewModes[k].gameObject;
				pageRoot.SetActive(k == showModeIdx);
				pageRoot.GetOrAddComponent<RectMask2D>().enabled = true;
				pageRoot = null;
				num = k;
			}
			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();
			this.localLoadingAnim.SetLoadingState(false);
			this.PlayAnimIn();
			yield return new WaitForEndOfFrame();
			bool starting = this._starting;
			if (starting)
			{
				this._starting = false;
				RectTransform viewList = this.rootViewModes[1];
				viewList.gameObject.SetActive(true);
				viewList.localScale = Vector3.zero;
				yield return new WaitForEndOfFrame();
				this.RefreshList(ViewCharacterMenuRelationship.EGroup.All, CharacterDisplayDataForGeneralScrollListUtils.SubPage.State);
				yield return new WaitForEndOfFrame();
				viewList.gameObject.SetActive(false);
				viewList.localScale = Vector3.one;
				viewList = null;
			}
			yield break;
		}

		// Token: 0x06009651 RID: 38481 RVA: 0x00461ECC File Offset: 0x004600CC
		private void UpdateGraph()
		{
			bool flag = this.dropdownModeSwitch.value != 0;
			if (!flag)
			{
				RectTransform contentRoot;
				bool flag2;
				if (this.graphViewScrollRect.isActiveAndEnabled)
				{
					contentRoot = this.graphViewScrollRect.content;
					flag2 = (contentRoot != null);
				}
				else
				{
					flag2 = false;
				}
				bool flag3 = flag2;
				if (flag3)
				{
					contentRoot.DOKill(false);
					contentRoot.SetPivot(new Vector2(0.5f, 0.5f));
					MouseWheelScale wheelScale = contentRoot.GetComponent<MouseWheelScale>();
					bool flag4 = this._selectedGroup == ViewCharacterMenuRelationship.EGroup.All;
					if (flag4)
					{
						contentRoot.DOAnchorPos(Vector2.zero, this.focusCameraDuration, false);
						contentRoot.DOScale(wheelScale.Min, this.focusCameraDuration).OnComplete(delegate
						{
							wheelScale.Reset();
							wheelScale.ScaleProcess(0f);
						});
					}
					else
					{
						for (int i = 0; i < this.graphViewGroupConfiguration.Length; i++)
						{
							ViewCharacterMenuRelationship.GraphViewGroupConfiguration config = this.graphViewGroupConfiguration[i];
							bool flag5 = config.group == this._selectedGroup;
							if (flag5)
							{
								RectTransform refersRect = this.graphViewTemplateGroup.parent.GetChild(i).GetComponent<RectTransform>();
								Vector2 groupPos = refersRect.anchoredPosition;
								Vector3 groupScale = wheelScale.Min + (wheelScale.Max - wheelScale.Min) * 0.5f;
								contentRoot.DOAnchorPos(new Vector2(-groupPos.x * groupScale.x, -groupPos.y * groupScale.y), this.focusCameraDuration, false).OnComplete(delegate
								{
									contentRoot.DOScale(groupScale, this.focusCameraDuration);
								});
								break;
							}
						}
					}
				}
			}
		}

		// Token: 0x06009652 RID: 38482 RVA: 0x0046211C File Offset: 0x0046031C
		private void SetCharacterTips(TooltipInvoker tips, int charId)
		{
			tips.triggerByChildRaycast = true;
			tips.Type = TipType.Character;
			tips.LeftUpOffsetPos = new Vector2(-10f, 0f);
			tips.LeftDownOffsetPos = new Vector2(-10f, 0f);
			if (tips.RuntimeParam == null)
			{
				tips.RuntimeParam = EasyPool.Get<ArgumentBox>();
			}
			tips.RuntimeParam.Set("charId", charId).Set("locationShow", true).Set("isDreamBack", this.IsCurrentSelectDreamBack());
		}

		// Token: 0x06009653 RID: 38483 RVA: 0x004621A8 File Offset: 0x004603A8
		private void Refresh(IDictionary<int, List<CharacterDisplayDataForRelations>> relatedChars)
		{
			bool flag = this._refreshCoroutine != null;
			if (flag)
			{
				base.StopCoroutine(this._refreshCoroutine);
			}
			this._refreshCoroutine = base.StartCoroutine(this.RefreshWithCharacterSuccessor(relatedChars));
			this.Element.ShowAfterRefresh();
		}

		// Token: 0x06009654 RID: 38484 RVA: 0x004621EF File Offset: 0x004603EF
		private IEnumerator RefreshWithCharacterSuccessor(IDictionary<int, List<CharacterDisplayDataForRelations>> relatedChars)
		{
			this._potentialSuccessorData = null;
			bool flag = this._potentialSuccessorCharId >= 0;
			if (flag)
			{
				ViewCharacterMenuRelationship.<>c__DisplayClass74_0 CS$<>8__locals1 = new ViewCharacterMenuRelationship.<>c__DisplayClass74_0();
				CS$<>8__locals1.<>4__this = this;
				CS$<>8__locals1.fetched = false;
				CharacterDomainMethod.AsyncCall.GetCharacterDisplayDataListForRelationsWithRelationType(this, base.CharacterMenu.CurCharacterId, new List<int>
				{
					this._potentialSuccessorCharId
				}, delegate(int offset, RawDataPool dataPool)
				{
					List<CharacterDisplayDataForRelations> ret = null;
					Serializer.Deserialize(dataPool, offset, ref ret);
					bool flag2 = ret != null && ret.Count > 0;
					if (flag2)
					{
						CS$<>8__locals1.<>4__this._potentialSuccessorData = ret[0];
					}
					CS$<>8__locals1.fetched = true;
				});
				while (!CS$<>8__locals1.fetched)
				{
					yield return null;
				}
				CS$<>8__locals1 = null;
			}
			this.RefreshCharacterSuccessor();
			yield return this.RefreshProcess(relatedChars);
			yield break;
		}

		// Token: 0x06009655 RID: 38485 RVA: 0x00462208 File Offset: 0x00460408
		private void RefreshCharacterSuccessor()
		{
			bool flag = this.characterSuccessor == null;
			if (!flag)
			{
				bool flag2 = !this._hasOrganizationMemberPotentialSuccessor || this._selfDisplayData == null;
				if (flag2)
				{
					this.SetCharacterSuccessorVisible(false);
				}
				else
				{
					bool hasKnownSuccessor = this._potentialSuccessorCharId >= 0;
					this.SetCharacterSuccessorVisible(true);
					this.SetCharacterSuccessorTitleTip(hasKnownSuccessor);
					bool flag3 = hasKnownSuccessor && this._potentialSuccessorData != null;
					if (flag3)
					{
						this.SetCharacterSuccessorMemberVisible(true);
						this.BindRelationMember(this.characterSuccessor, this._potentialSuccessorData, true);
					}
					else
					{
						this.SetCharacterSuccessorMemberVisible(false);
					}
				}
			}
		}

		// Token: 0x06009656 RID: 38486 RVA: 0x004622A4 File Offset: 0x004604A4
		private void SetCharacterSuccessorTitleTip(bool hasKnownSuccessor)
		{
			TooltipInvoker tips;
			bool flag = !this.characterSuccessor.CTryGet<TooltipInvoker>("TitleBg", out tips);
			if (!flag)
			{
				string text;
				bool flag2 = !CommonUtils.TryFormatOrganizationMemberPotentialSuccessorRelationTitleTip(this._selfDisplayData, hasKnownSuccessor, out text);
				if (flag2)
				{
					tips.enabled = false;
				}
				else
				{
					tips.Type = TipType.SingleDesc;
					tips.IsLanguageKey = false;
					tips.triggerByChildRaycast = true;
					tips.enabled = true;
					tips.PresetParam = new string[]
					{
						text.ColorReplace()
					};
				}
			}
		}

		// Token: 0x06009657 RID: 38487 RVA: 0x00462320 File Offset: 0x00460520
		private void SetCharacterSuccessorMemberVisible(bool visible)
		{
			Game.Components.Avatar.Avatar avatar;
			bool flag = this.characterSuccessor.CTryGet<Game.Components.Avatar.Avatar>("Avatar", out avatar);
			if (flag)
			{
				avatar.gameObject.SetActive(visible);
			}
			TextMeshProUGUI name;
			bool flag2 = this.characterSuccessor.CTryGet<TextMeshProUGUI>("Name", out name);
			if (flag2)
			{
				name.gameObject.SetActive(visible);
			}
			CButton button;
			bool flag3 = this.characterSuccessor.CTryGet<CButton>("Button", out button);
			if (flag3)
			{
				button.gameObject.SetActive(visible);
			}
			CButton jump;
			bool flag4 = this.characterSuccessor.CTryGet<CButton>("Jump", out jump);
			if (flag4)
			{
				jump.gameObject.SetActive(visible);
			}
			RectTransform typeIconFrame;
			bool flag5 = this.characterSuccessor.CTryGet<RectTransform>("TypeIconFrame", out typeIconFrame);
			if (flag5)
			{
				typeIconFrame.gameObject.SetActive(visible);
			}
			Refers direction;
			bool flag6 = this.characterSuccessor.CTryGet<Refers>("Direction", out direction);
			if (flag6)
			{
				direction.gameObject.SetActive(visible);
			}
			Transform line = this.characterSuccessor.transform.Find("Line");
			bool flag7 = line != null;
			if (flag7)
			{
				line.gameObject.SetActive(visible);
			}
		}

		// Token: 0x06009658 RID: 38488 RVA: 0x00462444 File Offset: 0x00460644
		private void SetCharacterSuccessorVisible(bool visible)
		{
			bool flag = this.characterSuccessor != null;
			if (flag)
			{
				this.characterSuccessor.gameObject.SetActive(visible);
			}
		}

		// Token: 0x06009659 RID: 38489 RVA: 0x00462474 File Offset: 0x00460674
		private void BindRelationMember(Refers member, CharacterDisplayDataForRelations data, bool hideRelationTypeIcon = false)
		{
			bool dead;
			ViewCharacterMenuGenealogy.UpdateAvatar(data, member.CGet<Game.Components.Avatar.Avatar>("Avatar"), out dead);
			RectTransform memberRect = member.transform as RectTransform;
			bool flag = memberRect != null;
			if (flag)
			{
				memberRect.localPosition = memberRect.localPosition.SetZ(0f);
			}
			member.CGet<TextMeshProUGUI>("Name").text = NameCenter.GetMonasticTitleOrDisplayName(ref data.Main.NameData, base.CharacterMenu.IsTaiwu(data.CharacterId), false);
			CButton memberButton;
			bool flag2 = member.CTryGet<CButton>("Button", out memberButton);
			if (flag2)
			{
				memberButton.onClick.ResetListener(delegate()
				{
					this.OnCharacterClicked(data);
				});
				this.SetCharacterTips(memberButton.gameObject.GetOrAddComponent<TooltipInvoker>(), data.CharacterId);
			}
			ViewCharacterMenuRelationship.SetToLocationButton(base.CharacterMenu, data.Location, data.CharacterId, dead, this.IsCurrentSelectDreamBack(), member.CGet<CButton>("Jump"));
			RectTransform typeIconFrame;
			bool flag3 = hideRelationTypeIcon && member.CTryGet<RectTransform>("TypeIconFrame", out typeIconFrame);
			if (flag3)
			{
				typeIconFrame.gameObject.SetActive(false);
			}
			Refers direction;
			bool flag4 = member.CTryGet<Refers>("Direction", out direction);
			if (flag4)
			{
				direction.gameObject.SetActive(false);
			}
		}

		// Token: 0x0600965A RID: 38490 RVA: 0x004625E0 File Offset: 0x004607E0
		private static void SetRelationTypeTips(CImage typeIcon, ViewCharacterMenuRelationship.RelationType type)
		{
			TooltipInvoker tips = typeIcon.gameObject.GetOrAddComponent<TooltipInvoker>();
			tips.Type = TipType.SingleDesc;
			tips.triggerByChildRaycast = true;
			tips.enabled = true;
			switch (type)
			{
			case ViewCharacterMenuRelationship.RelationType.Blood:
				tips.PresetParam = new string[]
				{
					LanguageKey.LK_MouseTip_CloseRelation_0.Tr()
				};
				return;
			case ViewCharacterMenuRelationship.RelationType.Step:
				tips.PresetParam = new string[]
				{
					LanguageKey.LK_MouseTip_CloseRelation_2.Tr()
				};
				return;
			case ViewCharacterMenuRelationship.RelationType.Adoptive:
				tips.PresetParam = new string[]
				{
					LanguageKey.LK_MouseTip_CloseRelation_1.Tr()
				};
				return;
			case ViewCharacterMenuRelationship.RelationType.Spouse:
				tips.PresetParam = new string[]
				{
					LanguageKey.LK_MouseTip_CloseRelation_3.Tr()
				};
				return;
			}
			tips.enabled = false;
		}

		// Token: 0x0600965B RID: 38491 RVA: 0x004626A8 File Offset: 0x004608A8
		private static void SetToLocationButton(ViewCharacterMenu characterMenu, Location charLocation, int charId, bool isDead, bool isDreamBack, CButton toLocationButton)
		{
			TooltipInvoker toLocationTips = toLocationButton.gameObject.GetOrAddComponent<TooltipInvoker>();
			WorldMapModel mapModel = SingletonObject.getInstance<WorldMapModel>();
			toLocationTips.Type = TipType.Simple;
			TooltipInvoker tooltipInvoker = toLocationTips;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
			}
			toLocationTips.RuntimeParam.Set("arg0", LocalStringManager.Get(LanguageKey.LK_CharacterLocationFind_Tips_Title));
			bool flag = UIElement.EventWindow.Exist || UIElement.Combat.Exist || UIElement.CombatBegin.Exist || UIElement.LifeSkillCombatOld.Exist || UIElement.Debate.Exist || UIElement.LifeSkillCombatBegin.Exist || UIElement.LifeSkillCombatPrepare.Exist || UIElement.BuildingArea.Exist || UIElement.CricketCombat.Exist || SingletonObject.getInstance<AdventureRemakeModel>().AdventureTaiwu.InAdventure || SingletonObject.getInstance<AdventureRemakeModel>().AdventureMajorEventTaiwu.InAdventure;
			bool isUsable;
			if (flag)
			{
				isUsable = false;
				toLocationTips.RuntimeParam.Set("arg1", LocalStringManager.Get(LanguageKey.LK_CharacterLocationFind_Tips_Disabled));
			}
			else
			{
				isUsable = (charLocation.IsValid() && mapModel.GetStateId(charLocation.AreaId) == mapModel.CurrentStateId && !isDreamBack);
				bool isUsable2 = isUsable;
				if (isUsable2)
				{
					toLocationTips.RuntimeParam.Set("arg1", LocalStringManager.Get(LanguageKey.LK_CharacterLocationFind_Tips_Available));
				}
				else
				{
					bool flag2 = SingletonObject.getInstance<CharacterMonitorModel>().IsTaiwuTeamCharacter(charId);
					if (flag2)
					{
						toLocationTips.RuntimeParam.Set("arg1", LocalStringManager.Get(LanguageKey.LK_CharacterLocationFind_Tips_Available));
						isUsable = true;
					}
					else
					{
						bool flag3 = !charLocation.IsValid();
						if (flag3)
						{
							toLocationTips.RuntimeParam.Set("arg1", LocalStringManager.Get(LanguageKey.LK_CharacterLocationFind_Tips_Unknown));
						}
						else
						{
							bool flag4 = mapModel.GetStateId(charLocation.AreaId) != mapModel.CurrentStateId;
							if (flag4)
							{
								toLocationTips.RuntimeParam.Set("arg1", LocalStringManager.Get(LanguageKey.LK_CharacterLocationFind_Tips_NotAvailable));
							}
							else
							{
								bool flag5 = mapModel.Areas.CheckIndex((int)charLocation.AreaId) && !mapModel.Areas[(int)charLocation.AreaId].StationUnlocked;
								if (flag5)
								{
									toLocationTips.RuntimeParam.Set("arg1", LocalStringManager.Get(LanguageKey.LK_CharacterLocationFind_Tips_NeedToUnlock));
								}
								else
								{
									toLocationTips.RuntimeParam.Set("arg1", LocalStringManager.Get(LanguageKey.LK_CharacterLocationFind_Tips_Unknown));
								}
							}
						}
					}
				}
			}
			bool flag6 = SingletonObject.getInstance<DisplayTriggerModel>().LegacyPassingState > 0;
			if (flag6)
			{
				isUsable = false;
			}
			bool inAdventure = SingletonObject.getInstance<AdventureRemakeModel>().AdventureTaiwu.InAdventure;
			if (inAdventure)
			{
				isUsable = false;
			}
			bool traveling = WorldMapModel.Traveling;
			if (traveling)
			{
				isUsable = false;
			}
			toLocationButton.interactable = isUsable;
			toLocationButton.ClearAndAddListener(delegate
			{
				bool flag7 = !isUsable;
				if (!flag7)
				{
					bool activeSelf = characterMenu.StackView.gameObject.activeSelf;
					if (activeSelf)
					{
						characterMenu.StackView.ResetAndSetNotActive(characterMenu);
					}
					UIManager uiManager = UIManager.Instance;
					bool flag8 = uiManager.IsFocusElement(UIElement.StateMainWorld);
					if (flag8)
					{
						uiManager.HideUI(UIElement.CharacterMenu);
					}
					else
					{
						uiManager.HideAll();
						uiManager.ChangeToUI(UIElement.StateMainWorld);
					}
					bool flag9 = SingletonObject.getInstance<CharacterMonitorModel>().IsTaiwuTeamCharacter(charId);
					if (flag9)
					{
						charLocation = SingletonObject.getInstance<WorldMapModel>().CurrentLocation;
					}
					Location location = isDreamBack ? Location.Invalid : charLocation;
					SingletonObject.getInstance<WorldMapModel>().JumpToTemporaryMark(location, isDead ? 3 : 0);
				}
			});
			toLocationTips.Refresh(false, -1);
		}

		// Token: 0x0600965C RID: 38492 RVA: 0x004629F0 File Offset: 0x00460BF0
		private static IEnumerable<CharacterDisplayDataForRelations> ValidRelatedChar(IEnumerable<CharacterDisplayDataForRelations> source)
		{
			return from data in source
			where data.Main != null
			where data.Main.CreatingType != 3
			select data;
		}

		// Token: 0x0600965D RID: 38493 RVA: 0x00462A48 File Offset: 0x00460C48
		private void PlayAnimIn()
		{
			bool played = this._played;
			if (!played)
			{
				this._played = true;
				this.graphViewSelf.PlayAnim();
				for (int i = 0; i < this.graphViewTemplateGroupContainer.childCount; i++)
				{
					this.graphViewTemplateGroupContainer.GetChild(i).GetComponent<RelationshipGroup>().PlayAnim();
				}
			}
		}

		// Token: 0x06009662 RID: 38498 RVA: 0x00462B80 File Offset: 0x00460D80
		[CompilerGenerated]
		private void <RefreshListData>g__PrepareRowTemplateContainers|57_4(CharacterDisplayDataForGeneralScrollListUtils.SubPage subPage)
		{
			Transform containerRoot = this.listViewRowTemplate.ContainerRoot;
			for (int i = containerRoot.childCount - 1; i >= 0; i--)
			{
				Transform child = containerRoot.GetChild(i);
				bool flag = child.GetComponent<RowCellContainer>() != null;
				if (flag)
				{
					Object.Destroy(child.gameObject);
				}
			}
			foreach (RowCellContainer containerTemplate in this.<RefreshListData>g__GetCellContainerTemplates|57_5(subPage))
			{
				RowCellContainer container = Object.Instantiate<RowCellContainer>(containerTemplate, containerRoot);
				container.gameObject.SetActive(true);
			}
			this.listViewRowTemplate.ResetSibling();
		}

		// Token: 0x06009663 RID: 38499 RVA: 0x00462C48 File Offset: 0x00460E48
		[CompilerGenerated]
		private IEnumerable<RowCellContainer> <RefreshListData>g__GetCellContainerTemplates|57_5(CharacterDisplayDataForGeneralScrollListUtils.SubPage subPage)
		{
			yield return this.listViewAvatarAndNameCellContainer;
			if (subPage != CharacterDisplayDataForGeneralScrollListUtils.SubPage.Command)
			{
				int columnCount = ViewCharacterMenuRelationship.<RefreshListData>g__GetColumnCount|57_6(subPage);
				int num;
				for (int i = 0; i < columnCount; i = num + 1)
				{
					yield return this.listViewSingleTextCellContainer;
					num = i;
				}
			}
			else
			{
				int num;
				for (int j = 0; j < 6; j = num + 1)
				{
					yield return this.listViewIconAndTextCellContainer;
					num = j;
				}
			}
			yield break;
		}

		// Token: 0x06009664 RID: 38500 RVA: 0x00462C70 File Offset: 0x00460E70
		[CompilerGenerated]
		internal static int <RefreshListData>g__GetColumnCount|57_6(CharacterDisplayDataForGeneralScrollListUtils.SubPage subPage)
		{
			if (!true)
			{
			}
			int result;
			switch (subPage)
			{
			case CharacterDisplayDataForGeneralScrollListUtils.SubPage.State:
				result = 10;
				break;
			case CharacterDisplayDataForGeneralScrollListUtils.SubPage.Property:
				result = 10;
				break;
			case CharacterDisplayDataForGeneralScrollListUtils.SubPage.Property2:
				result = 9;
				break;
			case CharacterDisplayDataForGeneralScrollListUtils.SubPage.LifeSkill:
				result = 17;
				break;
			case CharacterDisplayDataForGeneralScrollListUtils.SubPage.CombatSkill:
				result = 15;
				break;
			case CharacterDisplayDataForGeneralScrollListUtils.SubPage.Personality:
				result = 7;
				break;
			case CharacterDisplayDataForGeneralScrollListUtils.SubPage.Item:
				result = 10;
				break;
			case CharacterDisplayDataForGeneralScrollListUtils.SubPage.Command:
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

		// Token: 0x06009665 RID: 38501 RVA: 0x00462CE0 File Offset: 0x00460EE0
		[CompilerGenerated]
		internal static Vector3 <RefreshProcess>g__ProtectedVector3|69_7(float x, float y)
		{
			Vector3 result = new Vector3(float.IsNaN(x) ? 0f : x, float.IsNaN(y) ? 0f : y);
			return result;
		}

		// Token: 0x06009666 RID: 38502 RVA: 0x00462D1A File Offset: 0x00460F1A
		[CompilerGenerated]
		internal static bool <RefreshProcess>g__IsAlive|69_5(CharacterDisplayDataForRelations charData)
		{
			return charData.Main.CreatingType != 0 && charData.LifeState == 0;
		}

		// Token: 0x06009667 RID: 38503 RVA: 0x00462D38 File Offset: 0x00460F38
		[CompilerGenerated]
		internal static bool <RefreshProcess>g__IsDead|69_6(CharacterDisplayDataForRelations charData)
		{
			sbyte lifeState = charData.LifeState;
			return lifeState == 1 || lifeState == 2;
		}

		// Token: 0x04007345 RID: 29509
		[SerializeField]
		private CToggleGroup toggleGroupRelationshipType;

		// Token: 0x04007346 RID: 29510
		[SerializeField]
		private CDropdown dropdownModeSwitch;

		// Token: 0x04007347 RID: 29511
		[SerializeField]
		private RectTransform[] rootViewModes;

		// Token: 0x04007348 RID: 29512
		[SerializeField]
		private float focusCameraDuration;

		// Token: 0x04007349 RID: 29513
		[SerializeField]
		private Transform graphViewTemplateGroupContainer;

		// Token: 0x0400734A RID: 29514
		[SerializeField]
		private RectTransform graphViewTemplateGroup;

		// Token: 0x0400734B RID: 29515
		[SerializeField]
		private GameObject graphViewTemplateCharacter;

		// Token: 0x0400734C RID: 29516
		[SerializeField]
		private CharacterCircle graphViewSelf;

		// Token: 0x0400734D RID: 29517
		[SerializeField]
		private Refers characterSuccessor;

		// Token: 0x0400734E RID: 29518
		[SerializeField]
		private ScrollRect graphViewScrollRect;

		// Token: 0x0400734F RID: 29519
		private int _potentialSuccessorCharId = -1;

		// Token: 0x04007350 RID: 29520
		private CharacterDisplayDataForRelations _potentialSuccessorData;

		// Token: 0x04007351 RID: 29521
		private CharacterDisplayData _selfDisplayData;

		// Token: 0x04007352 RID: 29522
		private bool _hasOrganizationMemberPotentialSuccessor;

		// Token: 0x04007353 RID: 29523
		[SerializeField]
		private ViewCharacterMenuRelationship.GraphViewGroupConfiguration[] graphViewGroupConfiguration;

		// Token: 0x04007354 RID: 29524
		[SerializeField]
		private ViewCharacterMenuRelationship.GraphViewGroupStyle[] graphViewGroupStyle;

		// Token: 0x04007355 RID: 29525
		[SerializeField]
		private ViewCharacterMenuRelationship.GraphViewRelationTypeConfiguration[] graphViewRelationTypeConfiguration;

		// Token: 0x04007356 RID: 29526
		[SerializeField]
		private RowItem listViewRowTemplate;

		// Token: 0x04007357 RID: 29527
		[SerializeField]
		private RowCellContainer listViewSingleTextCellContainer;

		// Token: 0x04007358 RID: 29528
		[SerializeField]
		private RowCellContainer listViewAvatarAndNameCellContainer;

		// Token: 0x04007359 RID: 29529
		[SerializeField]
		private RowCellContainer listViewIconAndTextCellContainer;

		// Token: 0x0400735A RID: 29530
		[SerializeField]
		private ListStyleGeneralScroll listViewStyleGeneralScroll;

		// Token: 0x0400735B RID: 29531
		[SerializeField]
		private SortAndFilterLegacy sortAndFilter;

		// Token: 0x0400735C RID: 29532
		[SerializeField]
		private CToggleGroup toggleGroupPeriod;

		// Token: 0x0400735D RID: 29533
		[SerializeField]
		private CharacterMenuLocalLoadingAnim localLoadingAnim;

		// Token: 0x0400735E RID: 29534
		private RelationshipSortAndFilterController _sortAndFilterController;

		// Token: 0x0400735F RID: 29535
		private TabSortStateManager<ViewCharacterMenuRelationship.EGroup, CharacterDisplayDataForRelations> _tabSortStateManager;

		// Token: 0x04007360 RID: 29536
		private IDictionary<int, List<CharacterDisplayDataForRelations>> _relatedChars;

		// Token: 0x04007361 RID: 29537
		private bool _isInitDreamBack;

		// Token: 0x04007362 RID: 29538
		private bool _isCurrentTaiwuOverwrittenByDreamBack;

		// Token: 0x04007363 RID: 29539
		private ViewCharacterMenuRelationship.EGroup _selectedGroup = ViewCharacterMenuRelationship.EGroup.Max;

		// Token: 0x04007364 RID: 29540
		private const string CharTemplatePrefabKey = "ViewCharacterMenuRelationshipgraphViewTemplateCharacter";

		// Token: 0x04007365 RID: 29541
		private HashSet<GameObject> _pooledCharObjects = new HashSet<GameObject>();

		// Token: 0x04007366 RID: 29542
		private bool _starting = true;

		// Token: 0x04007367 RID: 29543
		private Coroutine _refreshCoroutine;

		// Token: 0x04007368 RID: 29544
		private bool _played = false;

		// Token: 0x02002230 RID: 8752
		[Serializable]
		public struct GraphViewGroupConfiguration
		{
			// Token: 0x0400D864 RID: 55396
			[SerializeField]
			public ViewCharacterMenuRelationship.EGroup group;

			// Token: 0x0400D865 RID: 55397
			[SerializeField]
			public Sprite icon;

			// Token: 0x0400D866 RID: 55398
			[SerializeField]
			public Texture2D back;
		}

		// Token: 0x02002231 RID: 8753
		[Serializable]
		public struct GraphViewGroupStyle
		{
			// Token: 0x170019E6 RID: 6630
			// (get) Token: 0x0600FD78 RID: 64888 RVA: 0x0063EB4F File Offset: 0x0063CD4F
			public int MemberCapacity
			{
				get
				{
					int[] array = this.memberCountPerCircle;
					return (array != null) ? array.Sum() : 0;
				}
			}

			// Token: 0x0400D867 RID: 55399
			[SerializeField]
			public int[] memberCountPerCircle;

			// Token: 0x0400D868 RID: 55400
			[SerializeField]
			public float circlePadding;

			// Token: 0x0400D869 RID: 55401
			[SerializeField]
			public float scale;

			// Token: 0x0400D86A RID: 55402
			[SerializeField]
			public bool centeredDetail;

			// Token: 0x0400D86B RID: 55403
			[SerializeField]
			public bool showMemberCount;
		}

		// Token: 0x02002232 RID: 8754
		[Serializable]
		public struct GraphViewRelationTypeConfiguration
		{
			// Token: 0x0400D86C RID: 55404
			[SerializeField]
			internal ViewCharacterMenuRelationship.RelationType type;

			// Token: 0x0400D86D RID: 55405
			[SerializeField]
			internal Sprite icon;

			// Token: 0x0400D86E RID: 55406
			[SerializeField]
			internal Sprite back;

			// Token: 0x0400D86F RID: 55407
			[SerializeField]
			internal bool hidden;
		}

		// Token: 0x02002233 RID: 8755
		public enum EGroup
		{
			// Token: 0x0400D871 RID: 55409
			All = -1,
			// Token: 0x0400D872 RID: 55410
			Parent,
			// Token: 0x0400D873 RID: 55411
			SwornBro,
			// Token: 0x0400D874 RID: 55412
			Wife,
			// Token: 0x0400D875 RID: 55413
			Child,
			// Token: 0x0400D876 RID: 55414
			Faction,
			// Token: 0x0400D877 RID: 55415
			Enemy,
			// Token: 0x0400D878 RID: 55416
			Friend,
			// Token: 0x0400D879 RID: 55417
			Adored,
			// Token: 0x0400D87A RID: 55418
			Mentor,
			// Token: 0x0400D87B RID: 55419
			Bro,
			// Token: 0x0400D87C RID: 55420
			Max
		}

		// Token: 0x02002234 RID: 8756
		public enum RelationType
		{
			// Token: 0x0400D87E RID: 55422
			Unknown,
			// Token: 0x0400D87F RID: 55423
			Blood,
			// Token: 0x0400D880 RID: 55424
			Step,
			// Token: 0x0400D881 RID: 55425
			Adoptive,
			// Token: 0x0400D882 RID: 55426
			Spouse
		}
	}
}
