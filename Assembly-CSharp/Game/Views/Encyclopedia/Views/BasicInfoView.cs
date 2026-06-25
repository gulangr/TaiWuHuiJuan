using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using DG.Tweening;
using FrameWork;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using Game.Components.Common;
using Game.Views.Encyclopedia.Elements;
using Game.Views.Encyclopedia.Event;
using Game.Views.Encyclopedia.Save;
using Game.Views.Encyclopedia.SyntaxTree;
using Game.Views.Encyclopedia.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.Encyclopedia.Views
{
	// Token: 0x02000A67 RID: 2663
	internal class BasicInfoView : View
	{
		// Token: 0x17000E63 RID: 3683
		// (get) Token: 0x060082D5 RID: 33493 RVA: 0x003CEDED File Offset: 0x003CCFED
		private CScrollRect CurScroll
		{
			get
			{
				return BasicInfoView.IsShowSearchResult ? this.searchScrollRect : this.normalScrollRect;
			}
		}

		// Token: 0x17000E64 RID: 3684
		// (get) Token: 0x060082D6 RID: 33494 RVA: 0x003CEE04 File Offset: 0x003CD004
		private RectTransform levelTwoContainer
		{
			get
			{
				return this.normalScrollRect.Content;
			}
		}

		// Token: 0x17000E65 RID: 3685
		// (get) Token: 0x060082D7 RID: 33495 RVA: 0x003CEE11 File Offset: 0x003CD011
		public BasicInfoView.BasicInfoViewState ViewState
		{
			get
			{
				return this.viewState;
			}
		}

		// Token: 0x060082D8 RID: 33496 RVA: 0x003CEE1C File Offset: 0x003CD01C
		private bool GetLevelTwoNodeFolded(string dataId)
		{
			bool flag = Save.SaveData == null;
			if (flag)
			{
				Save.LoadInfo();
			}
			return Save.SaveData.FoldedLevelTwoNodeSet.Contains(dataId);
		}

		// Token: 0x060082D9 RID: 33497 RVA: 0x003CEE50 File Offset: 0x003CD050
		private void ChangeLevelTwoNodeFoldedStatus(string dataId, bool isJump = false)
		{
			bool flag = Save.SaveData == null;
			if (flag)
			{
				Save.LoadInfo();
			}
			HashSet<string> set = Save.SaveData.FoldedLevelTwoNodeSet;
			bool flag2 = !set.Remove(dataId) && !isJump;
			if (flag2)
			{
				set.Add(dataId);
			}
			Save.SaveInfo();
		}

		// Token: 0x060082DA RID: 33498 RVA: 0x003CEEA0 File Offset: 0x003CD0A0
		private bool GetLevelThreeNodeFolded(string dataId)
		{
			bool flag = Save.SaveData == null;
			if (flag)
			{
				Save.LoadInfo();
			}
			return !Save.SaveData.DroppedLevelThreeNodeSet.Contains(dataId);
		}

		// Token: 0x060082DB RID: 33499 RVA: 0x003CEED8 File Offset: 0x003CD0D8
		internal void ChangeLevelThreeNodeFoldedStatus(string dataId, bool isJump = false)
		{
			bool flag = Save.SaveData == null;
			if (flag)
			{
				Save.LoadInfo();
			}
			HashSet<string> set = Save.SaveData.DroppedLevelThreeNodeSet;
			bool flag2 = !set.Add(dataId) && !isJump;
			if (flag2)
			{
				set.Remove(dataId);
			}
			Save.SaveInfo();
		}

		// Token: 0x060082DC RID: 33500 RVA: 0x003CEF28 File Offset: 0x003CD128
		private void Init()
		{
			bool flag = !this.NeedInit || SingletonBehaviour<ElementFactory>.Instance == null;
			if (!flag)
			{
				this.viewState = BasicInfoView.BasicInfoViewState.Normal;
				this.maxLevelOneNum = EncyclopediaDataManager.Instance.GetAllLevelOneData().Count;
				this.levelOneElements = new List<TitleElement>(this.maxLevelOneNum);
				this.levelTwoElements = new List<TitleElement>(this.maxLevelOneNum);
				this.levelThreeElementsDict = new Dictionary<int, TitleElement>(this.maxLevelOneNum);
				this.levelFourElementsDict = new Dictionary<int, TitleElement>(this.maxLevelOneNum);
				this.favoriteSelected.SetActive(false);
				BasicInfoView.Instance = this;
				this.InitLevelOneElements();
				this.InitLabels();
				this.InitFavorites();
				this.InitHistory();
				this.InitSearch();
				this.SwitchToNormal();
				this.currentLevelOneIndex = -1;
				this.ChangeLevelOneTitle(0);
				this.OnClickLevelThreeTitle(this.levelThreeElementsDict.Values.First<TitleElement>());
				this.NeedInit = false;
			}
		}

		// Token: 0x060082DD RID: 33501 RVA: 0x003CF01C File Offset: 0x003CD21C
		public override void InitButtonEvents()
		{
			this.favoriteButton.onClick.AddListener(new UnityAction(this.OnFavoriteButtonClick));
			this.historyButton.onClick.AddListener(new UnityAction(this.OnHistoryButtonClick));
		}

		// Token: 0x060082DE RID: 33502 RVA: 0x003CF059 File Offset: 0x003CD259
		private void Awake()
		{
			this.Init();
		}

		// Token: 0x060082DF RID: 33503 RVA: 0x003CF063 File Offset: 0x003CD263
		private void OnDestroy()
		{
			BasicInfoView.Instance = null;
		}

		// Token: 0x060082E0 RID: 33504 RVA: 0x003CF06C File Offset: 0x003CD26C
		private void OnEnable()
		{
			EventManager.Instance.AddListener(1, this);
			EventManager.Instance.AddListener(2, this);
			EventManager.Instance.AddListener(5, this);
			EventManager.Instance.AddListener(6, this);
			bool flag = !this.JumpKey.IsNullOrEmpty();
			if (flag)
			{
				this.JumpTo(this.JumpKey);
			}
			else
			{
				this.scrollRect.ScrollTo(this.scrollRect.GetComponent<RectTransform>().anchoredPosition, 0f);
			}
		}

		// Token: 0x060082E1 RID: 33505 RVA: 0x003CF0F0 File Offset: 0x003CD2F0
		private void OnDisable()
		{
			EventManager.Instance.RemoveListener(1, this);
			EventManager.Instance.RemoveListener(2, this);
			EventManager.Instance.RemoveListener(5, this);
			EventManager.Instance.RemoveListener(6, this);
		}

		// Token: 0x060082E2 RID: 33506 RVA: 0x003CF127 File Offset: 0x003CD327
		protected override void OnShow()
		{
			base.OnShow();
			this.Init();
		}

		// Token: 0x060082E3 RID: 33507 RVA: 0x003CF138 File Offset: 0x003CD338
		protected override void OnHandleEvent(int eventId, IEventArgs args)
		{
			switch (eventId)
			{
			case 1:
			{
				string key = args.GetValue<string>();
				this.OnLinkElementClick(key);
				break;
			}
			case 2:
			{
				FavoriteTypeChangedEventArgs favoriteArgs = args.GetValue<FavoriteTypeChangedEventArgs>();
				this.OnFavoriteChanged(favoriteArgs);
				break;
			}
			case 5:
			{
				PinTitleEventArgs pinArgs = args.GetValue<PinTitleEventArgs>();
				bool toPin = pinArgs.ToPin;
				if (toPin)
				{
					this.PinTab(pinArgs.DataId);
				}
				else
				{
					this.UnpinTab(pinArgs.DataId);
				}
				break;
			}
			case 6:
			{
				OpenPinTitleEventArgs openPinPageArgs = args.GetValue<OpenPinTitleEventArgs>();
				this.OpenPinPage(openPinPageArgs.DataId);
				break;
			}
			}
		}

		// Token: 0x060082E4 RID: 33508 RVA: 0x003CF1D9 File Offset: 0x003CD3D9
		private void OnClickLevelOneTitle(TitleElement title)
		{
			this.SwitchToNormal();
			this.ChangeLevelOneTitle(title.Index);
		}

		// Token: 0x060082E5 RID: 33509 RVA: 0x003CF1F0 File Offset: 0x003CD3F0
		private void OnClickLevelTwoTitle(TitleElement title)
		{
			this.OnClickLevelTwoTitle(title, false);
		}

		// Token: 0x060082E6 RID: 33510 RVA: 0x003CF1FC File Offset: 0x003CD3FC
		private void OnClickLevelTwoTitle(TitleElement title, bool isJump)
		{
			TitleElement titleElement = this.currentLevelFourTitle;
			if (titleElement != null)
			{
				titleElement.RefreshShowStatus();
			}
			this.CancelSelectTitle();
			this.SelectTitle(title, true);
			bool flag = this.viewState == BasicInfoView.BasicInfoViewState.Normal;
			if (flag)
			{
				this.ChangeLevelTwoNodeFoldedStatus(title.NodeData.ConfigItem.Key, isJump);
			}
			List<int> chidrenId = title.NodeData.Children;
			bool flag2 = chidrenId == null || chidrenId.Count <= 0;
			if (!flag2)
			{
				if (isJump)
				{
					bool flag3 = !title.IsShowChildren();
					if (flag3)
					{
						title.SetDropDown();
					}
				}
				this.RefreshLevelThreeTitle(title);
			}
		}

		// Token: 0x060082E7 RID: 33511 RVA: 0x003CF29A File Offset: 0x003CD49A
		private void OnClickLevelThreeTitle(TitleElement title)
		{
			this.OnClickLevelThreeTitle(title, false, true);
		}

		// Token: 0x060082E8 RID: 33512 RVA: 0x003CF2A8 File Offset: 0x003CD4A8
		private void OnClickLevelThreeTitle(TitleElement title, bool isJump, bool addHistory = true)
		{
			TitleElement titleElement = this.currentLevelFourTitle;
			if (titleElement != null)
			{
				titleElement.RefreshShowStatus();
			}
			bool needChangeTempData = title.NodeData.TempShowLevel == EEncyclopediaContentLevel.None && title.NodeData.TempTipLevel > EEncyclopediaContentLevel.None;
			bool flag = needChangeTempData;
			if (flag)
			{
				title.NodeData.TempShowLevel = title.NodeData.TempTipLevel;
				title.RefreshShowStatus();
			}
			bool needChangeTempDataForChild = false;
			bool flag2 = title.NodeData.Children != null;
			if (flag2)
			{
				foreach (int childId in title.NodeData.Children)
				{
					NodeData childNode = EncyclopediaDataManager.Instance.GetNodeData(childId);
					bool flag3 = childNode.TempShowLevel == EEncyclopediaContentLevel.None && childNode.TempTipLevel > EEncyclopediaContentLevel.None;
					if (flag3)
					{
						needChangeTempDataForChild = true;
						break;
					}
				}
			}
			bool flag4 = needChangeTempDataForChild;
			if (flag4)
			{
				bool flag5 = title.NodeData.Children != null;
				if (flag5)
				{
					foreach (int childId2 in title.NodeData.Children)
					{
						NodeData childNode2 = EncyclopediaDataManager.Instance.GetNodeData(childId2);
						childNode2.TempShowLevel = childNode2.TempTipLevel;
					}
				}
			}
			if (addHistory)
			{
				this.AddHistory(NodeLayerType.Three, title.LinkId, true);
				EncyclopediaDataManager.Instance.AddTabHistory(title.LinkId);
			}
			this.AddLabelItem(title.LinkId);
			this.CancelSelectTitle();
			this.SelectTitle(title, true);
			this.RefreshPageDetail(title.NodeData, null);
			this.pageDetailElement.ScrollToLastPos();
			bool flag6 = needChangeTempData;
			if (flag6)
			{
				title.NodeData.TempShowLevel = EEncyclopediaContentLevel.None;
			}
			bool flag7 = needChangeTempDataForChild;
			if (flag7)
			{
				bool flag8 = title.NodeData != null;
				if (flag8)
				{
					foreach (int childId3 in title.NodeData.Children)
					{
						NodeData childNode3 = EncyclopediaDataManager.Instance.GetNodeData(childId3);
						childNode3.TempShowLevel = EEncyclopediaContentLevel.None;
					}
				}
			}
			List<int> chidrenId = title.NodeData.Children;
			bool flag9 = chidrenId == null || chidrenId.Count <= 0;
			if (!flag9)
			{
				if (isJump)
				{
					bool flag10 = !title.IsShowChildren();
					if (flag10)
					{
						title.SetDropDown();
					}
				}
				this.RefreshLevelFourTitle(title);
				this.pageDetailElement.OnUpdateScrollValue(0f);
			}
		}

		// Token: 0x060082E9 RID: 33513 RVA: 0x003CF56C File Offset: 0x003CD76C
		private void OnClickLevelFourTitle(TitleElement titleElement)
		{
			TitleElement titleElement2 = this.currentLevelFourTitle;
			NodeData nodeData;
			if (titleElement2 == null)
			{
				nodeData = null;
			}
			else
			{
				TitleElement parentElement = titleElement2.ParentElement;
				nodeData = ((parentElement != null) ? parentElement.NodeData : null);
			}
			NodeData lastParent = nodeData;
			NodeData curParent = titleElement.NodeData.Parent;
			bool flag = lastParent != null && lastParent != curParent;
			if (flag)
			{
				TitleElement titleElement3 = this.currentLevelFourTitle;
				if (titleElement3 != null)
				{
					titleElement3.RefreshShowStatus();
				}
			}
			bool needChangeTempData = titleElement.NodeData.TempShowLevel == EEncyclopediaContentLevel.None && titleElement.NodeData.TempTipLevel > EEncyclopediaContentLevel.None;
			bool flag2 = needChangeTempData;
			if (flag2)
			{
				titleElement.NodeData.TempShowLevel = (titleElement.NodeData.Parent.TempShowLevel = titleElement.NodeData.TempTipLevel);
				titleElement.RefreshShowStatus();
			}
			TitleElement parent = titleElement.ParentElement;
			this.AddLabelItem(parent.LinkId);
			this.CancelSelectTitle();
			this.SelectTitle(titleElement, true);
			NodeData target = needChangeTempData ? titleElement.NodeData : null;
			this.RefreshPageDetail(parent.NodeData, target);
			this.pageDetailElement.JumpToContent(titleElement.NodeData.Id, null);
			bool flag3 = needChangeTempData;
			if (flag3)
			{
				titleElement.NodeData.TempShowLevel = (titleElement.NodeData.Parent.TempShowLevel = EEncyclopediaContentLevel.None);
			}
		}

		// Token: 0x060082EA RID: 33514 RVA: 0x003CF6A8 File Offset: 0x003CD8A8
		private void CancelSelectTitle()
		{
			TitleElement titleElement = this.currentLevelOneTitle;
			if (titleElement != null)
			{
				titleElement.Refresh(TitleShowMode.Normal);
			}
			TitleElement titleElement2 = this.currentLevelTwoTitle;
			if (titleElement2 != null)
			{
				titleElement2.Refresh(TitleShowMode.Normal);
			}
			TitleElement titleElement3 = this.currentLevelThreeTitle;
			if (titleElement3 != null)
			{
				titleElement3.Refresh(TitleShowMode.Normal);
			}
			TitleElement titleElement4 = this.currentLevelFourTitle;
			if (titleElement4 != null)
			{
				titleElement4.Refresh(TitleShowMode.Normal);
			}
		}

		// Token: 0x060082EB RID: 33515 RVA: 0x003CF704 File Offset: 0x003CD904
		public void UpdateSelectedTitle(int id)
		{
			TitleElement titleElement;
			if ((titleElement = this.GetLevelFourTitleElementById(id)) == null && (titleElement = this.GetLevelThreeTitleElementById(id)) == null)
			{
				titleElement = (this.GetLevelTwoTitleElementById(id) ?? this.GetLevelOneTitleElementById(id));
			}
			TitleElement title = titleElement;
			bool flag = title == null;
			if (!flag)
			{
				bool flag2 = this.viewState == BasicInfoView.BasicInfoViewState.History && this.GetLevelFourTitleElementById(id);
				if (!flag2)
				{
					this.CancelSelectTitle();
					this.SelectTitle(title, true);
				}
			}
		}

		// Token: 0x060082EC RID: 33516 RVA: 0x003CF778 File Offset: 0x003CD978
		private void SelectTitle(TitleElement titleElement, bool updateCurrentTitleReference = true)
		{
			bool flag = titleElement == null;
			if (!flag)
			{
				switch (titleElement.NodeData.NodeLayerType)
				{
				case NodeLayerType.One:
					this.currentLevelOneTitle = titleElement;
					if (updateCurrentTitleReference)
					{
						this.currentLevelTwoTitle = null;
						this.currentLevelThreeTitle = null;
						this.currentLevelFourTitle = null;
					}
					break;
				case NodeLayerType.Two:
					this.currentLevelTwoTitle = titleElement;
					if (updateCurrentTitleReference)
					{
						this.currentLevelThreeTitle = null;
						this.currentLevelFourTitle = null;
					}
					break;
				case NodeLayerType.Three:
					this.currentLevelThreeTitle = titleElement;
					if (updateCurrentTitleReference)
					{
						this.currentLevelFourTitle = null;
					}
					break;
				case NodeLayerType.Four:
					this.currentLevelFourTitle = titleElement;
					break;
				}
				bool flag2 = titleElement.NodeData.NodeLayerType != NodeLayerType.One || this.viewState == BasicInfoView.BasicInfoViewState.Normal;
				if (flag2)
				{
					titleElement.Refresh(TitleShowMode.Highlight);
				}
				this.SelectTitle(titleElement.ParentElement, false);
			}
		}

		// Token: 0x060082ED RID: 33517 RVA: 0x003CF854 File Offset: 0x003CDA54
		private void OnLinkElementClick(string key)
		{
			this.SwitchToNormal();
			NodeData data = EncyclopediaDataManager.Instance.GetNodeData(key);
			bool flag = data == null;
			if (!flag)
			{
				this.JumpTo(key);
			}
		}

		// Token: 0x060082EE RID: 33518 RVA: 0x003CF88A File Offset: 0x003CDA8A
		private void OnIndexerButtonClick()
		{
			this.SwitchToNormal();
		}

		// Token: 0x060082EF RID: 33519 RVA: 0x003CF894 File Offset: 0x003CDA94
		private void OnFavoriteButtonClick()
		{
			bool flag = this.viewState != BasicInfoView.BasicInfoViewState.Favorite;
			if (flag)
			{
				this.SwitchToFavorite();
			}
			else
			{
				this.OnIndexerButtonClick();
			}
			this.RefreshLabelItemsSelectStates();
		}

		// Token: 0x060082F0 RID: 33520 RVA: 0x003CF8D0 File Offset: 0x003CDAD0
		private void OnHistoryButtonClick()
		{
			bool flag = this.viewState != BasicInfoView.BasicInfoViewState.History;
			if (flag)
			{
				this.SwitchToHistory();
			}
			else
			{
				this.OnIndexerButtonClick();
			}
			this.RefreshLabelItemsSelectStates();
		}

		// Token: 0x060082F1 RID: 33521 RVA: 0x003CF90C File Offset: 0x003CDB0C
		private void OnFavoriteChanged(FavoriteTypeChangedEventArgs args)
		{
			bool toFavorite = args.ToFavorite;
			if (toFavorite)
			{
				this.AddFavorite(args.DataId);
			}
			else
			{
				this.RemoveFavorite(args.DataId);
			}
			this.RefreshFavoriteIcon(args.DataId);
		}

		// Token: 0x060082F2 RID: 33522 RVA: 0x003CF954 File Offset: 0x003CDB54
		private void RefreshFavoriteIcon(int dataId)
		{
			NodeData nodeData = EncyclopediaDataManager.Instance.GetNodeData(dataId);
			bool flag = nodeData.NodeLayerType == NodeLayerType.Three;
			if (flag)
			{
				TitleElement titleElement = this.GetLevelThreeTitleElementById(dataId);
				bool flag2 = titleElement != null;
				if (flag2)
				{
					titleElement.RefreshFavoriteIcon();
					titleElement.ParentElement.RefreshFavoriteIcon();
				}
				else
				{
					TitleElement parentElement = this.levelTwoElements.SingleOrDefault((TitleElement t) => t.NodeData.Id == nodeData.ParentId);
					if (parentElement != null)
					{
						parentElement.RefreshFavoriteIcon();
					}
				}
			}
			else
			{
				bool flag3 = nodeData.NodeLayerType == NodeLayerType.Four;
				if (flag3)
				{
					TitleElement titleElement2;
					bool flag4 = this.levelFourElementsDict.TryGetValue(dataId, out titleElement2);
					if (flag4)
					{
						titleElement2.RefreshFavoriteIcon();
						titleElement2.ParentElement.RefreshFavoriteIcon();
					}
					else
					{
						TitleElement parentElement2 = this.levelThreeElementsDict.Values.SingleOrDefault((TitleElement t) => t.NodeData.Id == nodeData.ParentId);
						if (parentElement2 != null)
						{
							parentElement2.RefreshFavoriteIcon();
						}
					}
				}
			}
			BasicInfoView.FavoriteDisplayInfo favorItem;
			bool flag5 = this._favoriteDisplayInfos.TryGetValue(new ValueTuple<NodeLayerType, int>(nodeData.NodeLayerType, dataId), out favorItem);
			if (flag5)
			{
				favorItem.TitleUIElement.RefreshFavoriteIcon();
				favorItem.TitleUIElement.ParentElement.RefreshFavoriteIcon();
			}
			this.RefreshHistoryFavoriteIconSingle(dataId);
			bool flag6 = this.pageDetailElement != null && this.pageDetailElement.NodeData != null && this.pageDetailElement.NodeData.Id == dataId;
			if (flag6)
			{
				this.pageDetailElement.RefreshFavoriteIcon();
			}
			this.UpdateAllFavoritesItem();
		}

		// Token: 0x060082F3 RID: 33523 RVA: 0x003CFAEC File Offset: 0x003CDCEC
		private void InitLevelOneElements()
		{
			List<NodeData> nodeDataList = EncyclopediaDataManager.Instance.GetAllLevelOneData();
			for (int i = 0; i < this.maxLevelOneNum; i++)
			{
				NodeData nodeData = nodeDataList[i];
				TitleElement levelOne = (i < this.levelOneContainer.childCount) ? this.levelOneContainer.GetChild(i).GetComponent<TitleElement>() : SingletonBehaviour<ElementFactory>.Instance.CreateLevelOneTitleItem(this.levelOneContainer);
				levelOne.Init(nodeData, null);
				levelOne.Index = i;
				levelOne.SetIcon(string.Format("ui9_tab_encyclopedia_level_{0}_{1}", i, 0), string.Format("ui9_tab_encyclopedia_level_{0}_{1}", i, 1), string.Format("ui9_tab_encyclopedia_level_{0}_{1}", i, 3), string.Format("ui9_tab_encyclopedia_level_{0}_{1}", i, 4));
				levelOne.SetLine(i < this.maxLevelOneNum - 1);
				levelOne.OnClickSelfEvent = new Action<TitleElement>(this.OnClickLevelOneTitle);
				levelOne.gameObject.SetActive(true);
				this.levelOneElements.Add(levelOne);
			}
		}

		// Token: 0x060082F4 RID: 33524 RVA: 0x003CFC0C File Offset: 0x003CDE0C
		private void ChangeLevelOneTitle(int index)
		{
			TitleElement titleElement = this.currentLevelFourTitle;
			if (titleElement != null)
			{
				titleElement.RefreshShowStatus();
			}
			TitleElement title = this.levelOneElements[index];
			bool hasChange = this.currentLevelOneIndex != index;
			this.currentLevelOneIndex = index;
			this.CancelSelectTitle();
			this.SelectTitle(title, true);
			bool flag = hasChange;
			if (flag)
			{
				this.RefreshLevelTwoTitle(true);
			}
			this.RefreshFavorItemsSelectStates();
			this.RefreshHistoryItemsSelectStates();
			this._indexButtonIndex = this.currentLevelOneIndex;
			this.RefreshLabelItemsSelectStates();
		}

		// Token: 0x060082F5 RID: 33525 RVA: 0x003CFC8C File Offset: 0x003CDE8C
		public void RefreshLevelTwoTitle(bool clearCurrentSelections = true)
		{
			bool flag = this.ViewState == BasicInfoView.BasicInfoViewState.Search;
			if (!flag)
			{
				List<int> childIds = this.currentLevelOneTitle.NodeData.Children;
				this.levelTwoElements.Clear();
				for (int i = 0; i < this.levelTwoContainer.childCount; i++)
				{
					this.levelTwoContainer.GetChild(i).gameObject.SetActive(false);
				}
				for (int j = 0; j < childIds.Count; j++)
				{
					int childId = childIds[j];
					TitleElement title = SingletonBehaviour<ElementFactory>.Instance.GetOrCreateLevelTwoTitle(this.levelTwoContainer, j);
					NodeData nodeData = EncyclopediaDataManager.Instance.GetNodeData(childId);
					this.InitLevelTwoTitle(title, nodeData, j, new Action<TitleElement>(this.OnClickLevelTwoTitle));
					title.ParentElement = this.currentLevelOneTitle;
					this.RefreshLevelThreeTitle(title);
					bool flag2 = this.viewState == BasicInfoView.BasicInfoViewState.Normal && !this.GetLevelTwoNodeFolded(nodeData.ConfigItem.Key);
					if (flag2)
					{
						title.SetDropDown();
					}
					this.levelTwoElements.Add(title);
					title.RefreshButtonExtend();
				}
				if (clearCurrentSelections)
				{
					TitleElement titleElement = this.currentLevelTwoTitle;
					if (titleElement != null)
					{
						titleElement.Refresh(TitleShowMode.Normal);
					}
					this.currentLevelTwoTitle = null;
					TitleElement titleElement2 = this.currentLevelThreeTitle;
					if (titleElement2 != null)
					{
						titleElement2.Refresh(TitleShowMode.Normal);
					}
					this.currentLevelThreeTitle = null;
					TitleElement titleElement3 = this.currentLevelFourTitle;
					if (titleElement3 != null)
					{
						titleElement3.Refresh(TitleShowMode.Normal);
					}
					this.currentLevelFourTitle = null;
				}
				else
				{
					TitleElement titleElement4 = this.currentLevelTwoTitle;
					if (titleElement4 != null)
					{
						titleElement4.Refresh(TitleShowMode.Highlight);
					}
					TitleElement titleElement5 = this.currentLevelThreeTitle;
					if (titleElement5 != null)
					{
						titleElement5.Refresh(TitleShowMode.Highlight);
					}
					TitleElement titleElement6 = this.currentLevelFourTitle;
					if (titleElement6 != null)
					{
						titleElement6.Refresh(TitleShowMode.Highlight);
					}
				}
			}
		}

		// Token: 0x060082F6 RID: 33526 RVA: 0x003CFE58 File Offset: 0x003CE058
		private void RefreshLevelThreeTitle(TitleElement title)
		{
			bool flag = title.NodeData.Children != null;
			if (flag)
			{
				for (int i = 0; i < title.NodeData.Children.Count; i++)
				{
					int childId = title.NodeData.Children[i];
					NodeData nodeData = EncyclopediaDataManager.Instance.GetNodeData(childId);
					TitleElement levelThreeTitle = this.CreateAndInitLevelThreeTitle(title, i, nodeData, new Action<TitleElement>(this.OnClickLevelThreeTitle));
					levelThreeTitle.ParentElement = title;
					this.levelThreeElementsDict[childId] = levelThreeTitle;
					this.RefreshLevelFourTitle(levelThreeTitle);
					bool flag2 = this.viewState == BasicInfoView.BasicInfoViewState.Normal && !this.GetLevelThreeNodeFolded(nodeData.ConfigItem.Key);
					if (flag2)
					{
						levelThreeTitle.SetDropDown();
					}
				}
			}
			List<int> children = title.NodeData.Children;
			int count = (children != null) ? children.Count : 0;
			for (int j = count; j < title.ChildrenContainer.childCount; j++)
			{
				title.ChildrenContainer.GetChild(j).gameObject.SetActive(false);
			}
			title.RefreshButtonExtend();
		}

		// Token: 0x060082F7 RID: 33527 RVA: 0x003CFF8C File Offset: 0x003CE18C
		private void RefreshLevelFourTitle(TitleElement title)
		{
			int levelFourCount = 0;
			bool flag = title.NodeData.Children != null;
			if (flag)
			{
				for (int i = 0; i < title.NodeData.Children.Count; i++)
				{
					int childId = title.NodeData.Children[i];
					NodeData nodeData = EncyclopediaDataManager.Instance.GetNodeData(childId);
					bool flag2 = nodeData.NodeLayerType != NodeLayerType.Four;
					if (!flag2)
					{
						TitleElement levelFourTitle = this.CreateAndInitLevelFourTitle(title, levelFourCount++, nodeData, new Action<TitleElement>(this.OnClickLevelFourTitle));
						levelFourTitle.ParentElement = title;
						this.levelFourElementsDict[childId] = levelFourTitle;
					}
				}
			}
			for (int j = levelFourCount; j < title.ChildrenContainer.childCount; j++)
			{
				title.ChildrenContainer.GetChild(j).gameObject.SetActive(false);
			}
			title.RefreshButtonExtend();
		}

		// Token: 0x060082F8 RID: 33528 RVA: 0x003D0084 File Offset: 0x003CE284
		private TitleElement GetLevelOneTitleElementById(int id)
		{
			foreach (TitleElement titleElement in this.levelOneElements)
			{
				bool flag = titleElement.NodeData.Id == id;
				if (flag)
				{
					return titleElement;
				}
			}
			return null;
		}

		// Token: 0x060082F9 RID: 33529 RVA: 0x003D00F4 File Offset: 0x003CE2F4
		private TitleElement GetLevelTwoTitleElementById(int id)
		{
			foreach (TitleElement titleElement in this.levelTwoElements)
			{
				bool flag = titleElement.NodeData.Id == id;
				if (flag)
				{
					return titleElement;
				}
			}
			return null;
		}

		// Token: 0x060082FA RID: 33530 RVA: 0x003D0164 File Offset: 0x003CE364
		private TitleElement GetLevelThreeTitleElementById(int id)
		{
			return this.levelThreeElementsDict.GetValueOrDefault(id);
		}

		// Token: 0x060082FB RID: 33531 RVA: 0x003D0184 File Offset: 0x003CE384
		private TitleElement GetLevelFourTitleElementById(int id)
		{
			return this.levelFourElementsDict.GetValueOrDefault(id);
		}

		// Token: 0x060082FC RID: 33532 RVA: 0x003D01A4 File Offset: 0x003CE3A4
		private void JumpToLevelFour(int id)
		{
			NodeData fourData = EncyclopediaDataManager.Instance.GetNodeData(id);
			this.JumpToLevelThree(fourData.ParentId);
			TitleElement fourTitleElement = this.GetLevelFourTitleElementById(id);
			bool flag = fourTitleElement;
			if (flag)
			{
				this.OnClickLevelFourTitle(fourTitleElement);
				this.CurScroll.ScrollTo(fourTitleElement.RectTransform, 0f);
			}
		}

		// Token: 0x060082FD RID: 33533 RVA: 0x003D0200 File Offset: 0x003CE400
		private void JumpToLevelThree(int id)
		{
			NodeData threeData = EncyclopediaDataManager.Instance.GetNodeData(id);
			this.AddHistory(NodeLayerType.Three, id, true);
			EncyclopediaDataManager.Instance.AddTabHistory(id);
			this.JumpToLevelTwo(threeData.ParentId);
			TitleElement threeTitleElement = this.GetLevelThreeTitleElementById(id);
			bool flag = threeTitleElement;
			if (flag)
			{
				this.OnClickLevelThreeTitle(threeTitleElement, true, true);
				this.CurScroll.ScrollTo(threeTitleElement.RectTransform, 0f);
			}
		}

		// Token: 0x060082FE RID: 33534 RVA: 0x003D0274 File Offset: 0x003CE474
		private void JumpToLevelTwo(int id)
		{
			bool flag = this.ViewState == BasicInfoView.BasicInfoViewState.Search;
			if (!flag)
			{
				NodeData twoData = EncyclopediaDataManager.Instance.GetNodeData(id);
				this.JumpToLevelOne(twoData.ParentId);
				TitleElement twoTitleElement = this.GetLevelTwoTitleElementById(id);
				bool flag2 = twoTitleElement;
				if (flag2)
				{
					this.OnClickLevelTwoTitle(twoTitleElement, true);
				}
			}
		}

		// Token: 0x060082FF RID: 33535 RVA: 0x003D02C8 File Offset: 0x003CE4C8
		private void JumpToLevelOne(int id)
		{
			TitleElement oneTitleElement = this.GetLevelOneTitleElementById(id);
			bool flag = oneTitleElement;
			if (flag)
			{
				this.ChangeLevelOneTitle(oneTitleElement.Index);
			}
		}

		// Token: 0x06008300 RID: 33536 RVA: 0x003D02F8 File Offset: 0x003CE4F8
		public void JumpToFavorite(int id)
		{
			NodeData nodeData = EncyclopediaDataManager.Instance.GetNodeData(id);
			bool flag = nodeData == null;
			if (!flag)
			{
				switch (nodeData.NodeLayerType)
				{
				case NodeLayerType.Two:
					this.JumpToLevelTwoFavor(id);
					this.pageDetailElement.ScrollToLastPos();
					break;
				case NodeLayerType.Three:
					this.JumpToLevelThreeFavor(id);
					this.pageDetailElement.ScrollToLastPos();
					break;
				case NodeLayerType.Four:
					this.JumpToLevelThreeFavor(nodeData.ParentId);
					this.pageDetailElement.JumpToContent(id, null);
					break;
				case NodeLayerType.Content:
				{
					NodeData fourNode = EncyclopediaDataManager.Instance.GetNodeData(nodeData.ParentId);
					this.JumpToLevelThreeFavor(fourNode.ParentId);
					this.pageDetailElement.JumpToContent(id, null);
					break;
				}
				}
			}
		}

		// Token: 0x06008301 RID: 33537 RVA: 0x003D03C0 File Offset: 0x003CE5C0
		public void JumpTo(int id, SearchIndex index = null)
		{
			NodeData nodeData = EncyclopediaDataManager.Instance.GetNodeData(id);
			bool flag = nodeData == null;
			if (!flag)
			{
				switch (nodeData.NodeLayerType)
				{
				case NodeLayerType.One:
					this.JumpToLevelOne(id);
					break;
				case NodeLayerType.Two:
					this.JumpToLevelTwo(id);
					break;
				case NodeLayerType.Three:
					this.JumpToLevelThree(id);
					break;
				case NodeLayerType.Four:
					this.JumpToLevelFour(id);
					break;
				case NodeLayerType.Content:
					nodeData.IsCollapse = false;
					this.JumpTo(nodeData.ParentId, null);
					this.pageDetailElement.JumpToContent(id, index);
					break;
				default:
					throw new ArgumentOutOfRangeException();
				}
			}
		}

		// Token: 0x06008302 RID: 33538 RVA: 0x003D045C File Offset: 0x003CE65C
		public void JumpTo(string key)
		{
			NodeData nodeData = EncyclopediaDataManager.Instance.GetNodeData(key);
			bool flag = nodeData != null;
			if (flag)
			{
				this.JumpTo(nodeData.Id, null);
			}
			else
			{
				this.CalcSearchResult(key, true);
				bool flag2 = this._normalSearchResult.SelfSearchResultList.Count == 0;
				if (flag2)
				{
					this.CalcSearchResult(key, false);
				}
				foreach (SearchResultItem resultItem in this._normalSearchResult.SelfSearchResultList)
				{
					this.SetTempShowLevel(resultItem.NodeData, EEncyclopediaContentLevel.LowMidHigh);
				}
				this.JumpToSearchResult();
			}
		}

		// Token: 0x06008303 RID: 33539 RVA: 0x003D0518 File Offset: 0x003CE718
		private void SetTempShowLevel(NodeData nodeData, EEncyclopediaContentLevel level)
		{
			bool flag = nodeData == null;
			if (!flag)
			{
				nodeData.TempTipLevel = level;
				nodeData.TempShowLevel = level;
				this.SetTempShowLevel(nodeData.Parent, level);
			}
		}

		// Token: 0x06008304 RID: 33540 RVA: 0x003D054E File Offset: 0x003CE74E
		private void SwitchToNormal()
		{
			this.SwitchTo(BasicInfoView.BasicInfoViewState.Normal);
		}

		// Token: 0x06008305 RID: 33541 RVA: 0x003D0558 File Offset: 0x003CE758
		private void SwitchToFavorite()
		{
			this.SwitchTo(BasicInfoView.BasicInfoViewState.Favorite);
		}

		// Token: 0x06008306 RID: 33542 RVA: 0x003D0562 File Offset: 0x003CE762
		private void SwitchToHistory()
		{
			this.SwitchTo(BasicInfoView.BasicInfoViewState.History);
		}

		// Token: 0x06008307 RID: 33543 RVA: 0x003D056C File Offset: 0x003CE76C
		private void SwitchToSearch()
		{
			this.SwitchTo(BasicInfoView.BasicInfoViewState.Search);
		}

		// Token: 0x06008308 RID: 33544 RVA: 0x003D0578 File Offset: 0x003CE778
		private void SwitchTo(BasicInfoView.BasicInfoViewState state)
		{
			bool flag = this.viewState == state;
			if (!flag)
			{
				this.previousState = this.viewState;
				this.viewState = state;
				this.RefreshState();
				Action<BasicInfoView.BasicInfoViewState, BasicInfoView.BasicInfoViewState> onViewStateChange = this._onViewStateChange;
				if (onViewStateChange != null)
				{
					onViewStateChange(this.previousState, this.viewState);
				}
				switch (this.viewState)
				{
				case BasicInfoView.BasicInfoViewState.Normal:
					break;
				case BasicInfoView.BasicInfoViewState.History:
					this.UpdateAllHistoryItem();
					break;
				case BasicInfoView.BasicInfoViewState.Favorite:
					this.UpdateAllFavoritesItem();
					break;
				case BasicInfoView.BasicInfoViewState.Search:
					break;
				default:
					throw new ArgumentOutOfRangeException();
				}
				bool flag2 = this.previousState == BasicInfoView.BasicInfoViewState.Search && this.viewState != BasicInfoView.BasicInfoViewState.Search;
				if (flag2)
				{
					this.ClearSearch();
				}
				bool flag3 = this.previousState != BasicInfoView.BasicInfoViewState.Normal;
				if (flag3)
				{
					this.pageDetailElement.OnUpdateScrollValue(0f);
				}
				GameObject lastObj = this.GetView(this.previousState);
				GameObject curObj = this.GetView(this.viewState);
				bool flag4 = lastObj;
				if (flag4)
				{
					this.ExecuteViewAnim(lastObj, curObj, false);
				}
			}
		}

		// Token: 0x06008309 RID: 33545 RVA: 0x003D068C File Offset: 0x003CE88C
		private GameObject GetView(BasicInfoView.BasicInfoViewState state)
		{
			if (!true)
			{
			}
			GameObject result;
			switch (state)
			{
			case BasicInfoView.BasicInfoViewState.Normal:
				result = this.indexerGameObject;
				break;
			case BasicInfoView.BasicInfoViewState.History:
				result = this.historyGameObject;
				break;
			case BasicInfoView.BasicInfoViewState.Favorite:
				result = this.favoriteGameObject;
				break;
			case BasicInfoView.BasicInfoViewState.Search:
				result = this.searchGameObject;
				break;
			default:
				result = null;
				break;
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x0600830A RID: 33546 RVA: 0x003D06E4 File Offset: 0x003CE8E4
		private void ExecuteViewAnim(GameObject outGo, GameObject inGo, bool isRightMove)
		{
			Sequence seq = this._seq;
			if (seq != null)
			{
				seq.Kill(true);
			}
			CanvasGroup outCg = outGo.GetOrAddComponent<CanvasGroup>();
			RectTransform outRectTs = outGo.GetComponent<RectTransform>();
			Vector2 outStartPos = outRectTs.anchoredPosition;
			Vector2 outEndPos = outStartPos + Vector2.right * (isRightMove ? outRectTs.rect.width : (-1f * outRectTs.rect.width));
			CanvasGroup inCg = inGo.GetOrAddComponent<CanvasGroup>();
			RectTransform inRectTs = inGo.GetComponent<RectTransform>();
			Vector2 inEndPos = inRectTs.anchoredPosition;
			Vector2 inStartPos = inEndPos + Vector2.right * (isRightMove ? (-1f * inRectTs.rect.width) : inRectTs.rect.width);
			this._seq = ToggleGroupAnimUtility.SecondToggleSubChangeAnim(new SecondToggleSubAnimInfo
			{
				Cg = outCg,
				RectTs = outRectTs,
				StartAnchorPos = outStartPos,
				EndAnchorPos = outEndPos,
				MoveDuration = 0.13f,
				StartAlpha = 1f,
				EndAlpha = 0f,
				FadeDuration = 0.13f
			}, new SecondToggleSubAnimInfo
			{
				Cg = inCg,
				RectTs = inRectTs,
				StartAnchorPos = inStartPos,
				EndAnchorPos = inEndPos,
				MoveDuration = 0.13f,
				StartAlpha = 0f,
				EndAlpha = 1f,
				FadeDuration = 0.13f
			}, delegate
			{
				inRectTs.anchoredPosition = inEndPos;
				outRectTs.anchoredPosition = outStartPos;
				this._seq = null;
			});
			this._seq.Restart(true, -1f);
		}

		// Token: 0x0600830B RID: 33547 RVA: 0x003D08C8 File Offset: 0x003CEAC8
		private void SwitchToPreviousState()
		{
			switch (this.previousState)
			{
			case BasicInfoView.BasicInfoViewState.Normal:
				this.SwitchToNormal();
				this.ChangeLevelOneTitle(this._indexButtonIndex);
				break;
			case BasicInfoView.BasicInfoViewState.History:
				this.SwitchToHistory();
				break;
			case BasicInfoView.BasicInfoViewState.Favorite:
				this.SwitchToFavorite();
				break;
			case BasicInfoView.BasicInfoViewState.Search:
				this.SwitchToSearch();
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		// Token: 0x0600830C RID: 33548 RVA: 0x003D0930 File Offset: 0x003CEB30
		private void RefreshState()
		{
			bool isNormal = this.viewState == BasicInfoView.BasicInfoViewState.Normal;
			bool isFavorite = this.viewState == BasicInfoView.BasicInfoViewState.Favorite;
			bool isHistory = this.viewState == BasicInfoView.BasicInfoViewState.History;
			bool isSearch = this.viewState == BasicInfoView.BasicInfoViewState.Search;
			this.indexerGameObject.SetActive(isNormal);
			this.favoriteGameObject.SetActive(isFavorite);
			this.favoriteSelected.SetActive(isFavorite);
			this.historyGameObject.SetActive(isHistory);
			this.historySelected.SetActive(isHistory);
			this.searchGameObject.SetActive(isSearch);
			bool flag = this.currentLevelOneTitle;
			if (flag)
			{
				TitleShowMode mode = isNormal ? TitleShowMode.Highlight : TitleShowMode.Normal;
				this.currentLevelOneTitle.Refresh(mode);
			}
		}

		// Token: 0x0600830D RID: 33549 RVA: 0x003D09E0 File Offset: 0x003CEBE0
		private void RefreshPageDetail(NodeData nodeData, NodeData target = null)
		{
			bool flag;
			if (!this.NeedInit)
			{
				NodeData nodeData2 = this.pageDetailElement.NodeData;
				int? num = (nodeData2 != null) ? new int?(nodeData2.Id) : null;
				int id = nodeData.Id;
				flag = !(num.GetValueOrDefault() == id & num != null);
			}
			else
			{
				flag = true;
			}
			bool flag2 = flag;
			if (flag2)
			{
				this.pageDetailElement.Init(nodeData, target);
			}
			this.pageDetailElement.RefreshSearchResultHighlight(this.Searcher, this.SearchTitleOnly);
			this.RefreshLabelItemsSelectStates();
			this.RefreshHistoryItemsSelectStates();
			this.RefreshFavorItemsSelectStates(nodeData.NodeLayerType, nodeData.Id);
			this.RefreshHistoryItemsSelectStates(nodeData.NodeLayerType, nodeData.Id);
		}

		// Token: 0x0600830E RID: 33550 RVA: 0x003D0A9A File Offset: 0x003CEC9A
		public void RefreshPageDetail()
		{
			PageDetailElement pageDetailElement = this.pageDetailElement;
			if (pageDetailElement != null)
			{
				pageDetailElement.RefreshSearchResultHighlight(this.Searcher, this.SearchTitleOnly);
			}
		}

		// Token: 0x0600830F RID: 33551 RVA: 0x003D0ABB File Offset: 0x003CECBB
		private void InitLevelTwoTitle(TitleElement levelTwoTitle, NodeData nodeData, int i, Action<TitleElement> clickAction)
		{
			levelTwoTitle.Init(nodeData, null);
			levelTwoTitle.Index = i;
			levelTwoTitle.OnClickSelfEvent = clickAction;
		}

		// Token: 0x06008310 RID: 33552 RVA: 0x003D0AD8 File Offset: 0x003CECD8
		private TitleElement CreateAndInitLevelThreeTitle(TitleElement parent, int index, NodeData nodeData, Action<TitleElement> clickAction)
		{
			TitleElement levelThreeTitle = SingletonBehaviour<ElementFactory>.Instance.GetOrCreateLevelThreeTitle(parent.ChildrenContainer, index);
			levelThreeTitle.Init(nodeData, null);
			levelThreeTitle.Index = index;
			levelThreeTitle.OnClickSelfEvent = clickAction;
			levelThreeTitle.ParentElement = parent;
			return levelThreeTitle;
		}

		// Token: 0x06008311 RID: 33553 RVA: 0x003D0B20 File Offset: 0x003CED20
		private TitleElement CreateAndInitLevelFourTitle(TitleElement parent, int index, NodeData nodeData, Action<TitleElement> clickAction)
		{
			TitleElement titleElement = SingletonBehaviour<ElementFactory>.Instance.GetOrCreateLevelFourTitle(parent.ChildrenContainer, index);
			titleElement.Init(nodeData, null);
			titleElement.Index = index;
			titleElement.OnClickSelfEvent = clickAction;
			titleElement.ParentElement = parent;
			return titleElement;
		}

		// Token: 0x06008312 RID: 33554 RVA: 0x003D0B68 File Offset: 0x003CED68
		private void AddFavorite(int dataId)
		{
			NodeData nodeData = EncyclopediaDataManager.Instance.GetNodeData(dataId);
			Save.AddFavoritesInfos(nodeData.ConfigItem.Key);
			NodeLayerType titleLevel = nodeData.NodeLayerType;
			ValueTuple<NodeLayerType, int> tempKey = new ValueTuple<NodeLayerType, int>(titleLevel, dataId);
			bool flag = this._toRemoveFavoriteCache.Contains(tempKey);
			if (flag)
			{
				this._toRemoveFavoriteCache.Remove(tempKey);
			}
			this.AddFavorDisplay(titleLevel, dataId);
			bool flag2 = titleLevel == NodeLayerType.Three;
			if (flag2)
			{
				List<int> children = nodeData.Children;
				bool flag3 = children != null && children.Count > 0;
				if (flag3)
				{
					foreach (int childId in nodeData.Children)
					{
						NodeData childData = EncyclopediaDataManager.Instance.GetNodeData(childId);
						bool flag4 = childData.NodeLayerType == NodeLayerType.Four;
						if (flag4)
						{
							this.AddFavorDisplay(childData.NodeLayerType, childData.Id);
						}
					}
				}
			}
		}

		// Token: 0x06008313 RID: 33555 RVA: 0x003D0C74 File Offset: 0x003CEE74
		private void RemoveFavorite(int dataId)
		{
			NodeData nodeData = EncyclopediaDataManager.Instance.GetNodeData(dataId);
			bool flag = nodeData == null;
			if (!flag)
			{
				NodeLayerType titleLayer = nodeData.NodeLayerType;
				bool flag2 = !this._favoriteDisplayInfos.ContainsKey(new ValueTuple<NodeLayerType, int>(titleLayer, dataId));
				if (!flag2)
				{
					BasicInfoView.FavoriteDisplayInfo displayInfo = this._favoriteDisplayInfos[new ValueTuple<NodeLayerType, int>(titleLayer, dataId)];
					EncyclopediaContentItem configItem = displayInfo.DisplayNodeData.ConfigItem;
					Save.RemoveFavoritesInfos(configItem.Key);
					bool flag3 = this.viewState == BasicInfoView.BasicInfoViewState.Favorite;
					if (flag3)
					{
						this._toRemoveFavoriteCache.Add(new ValueTuple<NodeLayerType, int>(titleLayer, dataId));
					}
					else
					{
						this.RemoveFavorDisplay(titleLayer, dataId);
					}
				}
			}
		}

		// Token: 0x06008314 RID: 33556 RVA: 0x003D0D20 File Offset: 0x003CEF20
		private void RemoveFavorDisplay(NodeLayerType titleLayer, int dataId)
		{
			bool flag = titleLayer != NodeLayerType.One && titleLayer != NodeLayerType.Two && titleLayer != NodeLayerType.Three && titleLayer != NodeLayerType.Four;
			if (!flag)
			{
				BasicInfoView.FavoriteDisplayInfo displayInfo = this._favoriteDisplayInfos[new ValueTuple<NodeLayerType, int>(titleLayer, dataId)];
				this._favoriteDisplayInfos.Remove(new ValueTuple<NodeLayerType, int>(titleLayer, dataId));
				displayInfo.TitleUIElement.Release(true);
				NodeData displayNodeData = displayInfo.DisplayNodeData;
				bool flag2 = displayNodeData.NodeLayerType == NodeLayerType.Four;
				BasicInfoView.FavoriteDisplayInfo parentDisplayInfo;
				if (flag2)
				{
					parentDisplayInfo = this._favoriteDisplayInfos[new ValueTuple<NodeLayerType, int>(NodeLayerType.Three, displayNodeData.ParentId)];
				}
				else
				{
					bool flag3 = displayNodeData.NodeLayerType == NodeLayerType.Three;
					if (flag3)
					{
						parentDisplayInfo = this._favoriteDisplayInfos[new ValueTuple<NodeLayerType, int>(NodeLayerType.One, displayNodeData.LevelOneRoot.Id)];
					}
					else
					{
						bool flag4 = displayNodeData.NodeLayerType == NodeLayerType.Two;
						if (!flag4)
						{
							return;
						}
						parentDisplayInfo = this._favoriteDisplayInfos[new ValueTuple<NodeLayerType, int>(NodeLayerType.One, displayNodeData.ParentId)];
					}
				}
				parentDisplayInfo.DisplayNodeData.Children.Remove(displayInfo.DisplayNodeData.Id);
				bool flag5 = parentDisplayInfo.DisplayNodeData.Children.Count == 0;
				if (flag5)
				{
					this.RemoveFavorDisplay(parentDisplayInfo.DisplayNodeData.NodeLayerType, parentDisplayInfo.DisplayNodeData.Id);
				}
			}
		}

		// Token: 0x06008315 RID: 33557 RVA: 0x003D0E64 File Offset: 0x003CF064
		private void AddFavorDisplay(NodeLayerType titleLevel, int dataId)
		{
			bool flag = titleLevel != NodeLayerType.One && titleLevel != NodeLayerType.Three;
			if (!flag)
			{
				BasicInfoView.FavoriteDisplayInfo displayInfo;
				bool flag2 = !this._favoriteDisplayInfos.TryGetValue(new ValueTuple<NodeLayerType, int>(titleLevel, dataId), out displayInfo);
				if (flag2)
				{
					displayInfo = new BasicInfoView.FavoriteDisplayInfo
					{
						DisplayNodeData = this.GetNewNodeDataForFavor(EncyclopediaDataManager.Instance.GetNodeData(dataId))
					};
					this._favoriteDisplayInfos[new ValueTuple<NodeLayerType, int>(titleLevel, dataId)] = displayInfo;
				}
				bool flag3 = displayInfo.DisplayNodeData.NodeLayerType == NodeLayerType.One;
				if (flag3)
				{
					displayInfo.TitleUIElement = this.CreateFavorOneTitle(displayInfo.DisplayNodeData);
				}
				else
				{
					bool flag4 = displayInfo.DisplayNodeData.NodeLayerType == NodeLayerType.Two;
					if (flag4)
					{
						bool flag5 = !this._favoriteDisplayInfos.ContainsKey(new ValueTuple<NodeLayerType, int>(NodeLayerType.One, displayInfo.DisplayNodeData.ParentId));
						if (flag5)
						{
							this.AddFavorDisplay(NodeLayerType.One, displayInfo.DisplayNodeData.ParentId);
						}
						BasicInfoView.FavoriteDisplayInfo parentInfo = this._favoriteDisplayInfos[new ValueTuple<NodeLayerType, int>(NodeLayerType.One, displayInfo.DisplayNodeData.ParentId)];
						bool flag6 = !parentInfo.DisplayNodeData.Children.Contains(dataId);
						if (flag6)
						{
							parentInfo.DisplayNodeData.Children.Add(dataId);
							displayInfo.TitleUIElement = this.CreateFavorTwoTitle(displayInfo.DisplayNodeData, parentInfo.TitleUIElement);
						}
						this.CheckDisplayEnable(parentInfo);
					}
					else
					{
						bool flag7 = displayInfo.DisplayNodeData.NodeLayerType == NodeLayerType.Three;
						if (flag7)
						{
							bool flag8 = !this._favoriteDisplayInfos.ContainsKey(new ValueTuple<NodeLayerType, int>(NodeLayerType.One, displayInfo.DisplayNodeData.LevelOneRoot.Id));
							if (flag8)
							{
								this.AddFavorDisplay(NodeLayerType.One, displayInfo.DisplayNodeData.LevelOneRoot.Id);
							}
							BasicInfoView.FavoriteDisplayInfo parentInfo2 = this._favoriteDisplayInfos[new ValueTuple<NodeLayerType, int>(NodeLayerType.One, displayInfo.DisplayNodeData.LevelOneRoot.Id)];
							bool flag9 = !parentInfo2.DisplayNodeData.Children.Contains(dataId);
							if (flag9)
							{
								parentInfo2.DisplayNodeData.Children.Add(dataId);
								displayInfo.TitleUIElement = this.CreateFavorThreeTitle(displayInfo.DisplayNodeData, parentInfo2.TitleUIElement);
							}
							this.CheckDisplayEnable(parentInfo2);
						}
						else
						{
							bool flag10 = displayInfo.DisplayNodeData.NodeLayerType == NodeLayerType.Four;
							if (flag10)
							{
								bool flag11 = !this._favoriteDisplayInfos.ContainsKey(new ValueTuple<NodeLayerType, int>(NodeLayerType.Three, displayInfo.DisplayNodeData.ParentId));
								if (flag11)
								{
									this.AddFavorDisplay(NodeLayerType.Three, displayInfo.DisplayNodeData.ParentId);
								}
								BasicInfoView.FavoriteDisplayInfo parentInfo3 = this._favoriteDisplayInfos[new ValueTuple<NodeLayerType, int>(NodeLayerType.Three, displayInfo.DisplayNodeData.ParentId)];
								bool flag12 = !parentInfo3.DisplayNodeData.Children.Contains(dataId);
								if (flag12)
								{
									parentInfo3.DisplayNodeData.Children.Add(dataId);
									displayInfo.TitleUIElement = this.CreateFavorFourTitle(displayInfo.DisplayNodeData, parentInfo3.TitleUIElement);
								}
								this.CheckDisplayEnable(parentInfo3);
							}
						}
					}
				}
			}
		}

		// Token: 0x06008316 RID: 33558 RVA: 0x003D1160 File Offset: 0x003CF360
		private void CheckDisplayEnable(BasicInfoView.FavoriteDisplayInfo displayInfo)
		{
			bool flag = displayInfo.DisplayNodeData.Children.Count > 0;
			if (flag)
			{
				displayInfo.TitleUIElement.gameObject.SetActive(true);
			}
			bool flag2 = displayInfo.DisplayNodeData.NodeLayerType == NodeLayerType.Two;
			if (flag2)
			{
				displayInfo.TitleUIElement.ParentElement.gameObject.SetActive(true);
			}
		}

		// Token: 0x06008317 RID: 33559 RVA: 0x003D11C4 File Offset: 0x003CF3C4
		private void InitFavorites()
		{
			Debug.Log(SingletonObject.getInstance<GlobalSettings>().FavoritEncyclopediaData);
			this.InitFavorDisplay();
			this._onViewStateChange = (Action<BasicInfoView.BasicInfoViewState, BasicInfoView.BasicInfoViewState>)Delegate.Combine(this._onViewStateChange, new Action<BasicInfoView.BasicInfoViewState, BasicInfoView.BasicInfoViewState>(this.FavoriteOnChangeViewState));
		}

		// Token: 0x06008318 RID: 33560 RVA: 0x003D1200 File Offset: 0x003CF400
		private void FavoriteOnChangeViewState(BasicInfoView.BasicInfoViewState previous, BasicInfoView.BasicInfoViewState newState)
		{
			bool flag = previous == BasicInfoView.BasicInfoViewState.Favorite;
			if (flag)
			{
				this.ApplyRemoveFavorite();
			}
		}

		// Token: 0x06008319 RID: 33561 RVA: 0x003D1220 File Offset: 0x003CF420
		private void ApplyRemoveFavorite()
		{
			foreach (ValueTuple<NodeLayerType, int> item in this._toRemoveFavoriteCache)
			{
				this.RemoveFavorDisplay(item.Item1, item.Item2);
			}
			this._toRemoveFavoriteCache.Clear();
		}

		// Token: 0x0600831A RID: 33562 RVA: 0x003D1290 File Offset: 0x003CF490
		private void InitFavorDisplay()
		{
			foreach (string item in Save.SaveData.FavoritesInfos)
			{
				NodeData nodeData = EncyclopediaDataManager.Instance.GetNodeData(item);
				bool flag = nodeData == null;
				if (!flag)
				{
					bool flag2 = nodeData.NodeLayerType == NodeLayerType.Three;
					if (flag2)
					{
						this.AddFavorDisplay(nodeData.NodeLayerType, nodeData.Id);
						List<int> children = nodeData.Children;
						bool flag3 = children != null && children.Count > 0;
						if (flag3)
						{
							foreach (int childId in nodeData.Children)
							{
								NodeData childData = EncyclopediaDataManager.Instance.GetNodeData(childId);
								bool flag4 = childData.NodeLayerType == NodeLayerType.Four;
								if (flag4)
								{
									this.AddFavorDisplay(childData.NodeLayerType, childData.Id);
								}
							}
						}
					}
					else
					{
						bool flag5 = nodeData.NodeLayerType == NodeLayerType.Two;
						if (flag5)
						{
						}
					}
				}
			}
		}

		// Token: 0x0600831B RID: 33563 RVA: 0x003D13D4 File Offset: 0x003CF5D4
		private TitleElement CreateFavorOneTitle(NodeData nodeData)
		{
			TitleElement levelOne = SingletonBehaviour<ElementFactory>.Instance.CreateFavoritesItemLevelOne(this.favoritesContainer);
			levelOne.Init(nodeData, null);
			levelOne.OnClickSelfEvent = new Action<TitleElement>(this.OnClickFavorLevelOneTitle);
			levelOne.gameObject.SetActive(nodeData.Children.Count > 0);
			return levelOne;
		}

		// Token: 0x0600831C RID: 33564 RVA: 0x003D1430 File Offset: 0x003CF630
		private TitleElement CreateFavorTwoTitle(NodeData nodeData, TitleElement parent)
		{
			return this.CreateAndInitFavorLevelTwoTitle(parent, nodeData, new Action<TitleElement>(this.OnClickFavoritesLevelTwoTitle));
		}

		// Token: 0x0600831D RID: 33565 RVA: 0x003D1458 File Offset: 0x003CF658
		private TitleElement CreateFavorThreeTitle(NodeData nodeData, TitleElement parent)
		{
			return this.CreateAndInitLevelThreeTitleFavor(parent, nodeData, new Action<TitleElement>(this.OnClickFavoritesLevelThreeTitle));
		}

		// Token: 0x0600831E RID: 33566 RVA: 0x003D1480 File Offset: 0x003CF680
		private TitleElement CreateFavorFourTitle(NodeData nodeData, TitleElement parent)
		{
			return this.CreateAndInitLevelFourTitleFavor(parent, nodeData, new Action<TitleElement>(this.OnClickFavoritesLevelFourTitle));
		}

		// Token: 0x0600831F RID: 33567 RVA: 0x003D14A8 File Offset: 0x003CF6A8
		private TitleElement CreateAndInitLevelFourTitleFavor(TitleElement parent, NodeData nodeData, Action<TitleElement> clickAction)
		{
			TitleElement levelThreeTitle = SingletonBehaviour<ElementFactory>.Instance.CreateLevelFourTitleItem(parent.ChildrenContainer);
			levelThreeTitle.Init(nodeData, null);
			levelThreeTitle.OnClickSelfEvent = clickAction;
			levelThreeTitle.ParentElement = parent;
			return levelThreeTitle;
		}

		// Token: 0x06008320 RID: 33568 RVA: 0x003D14E4 File Offset: 0x003CF6E4
		private TitleElement CreateAndInitLevelThreeTitleFavor(TitleElement parent, NodeData nodeData, Action<TitleElement> clickAction)
		{
			TitleElement levelThreeTitle = SingletonBehaviour<ElementFactory>.Instance.CreateLevelThreeTitleItem(parent.ChildrenContainer);
			levelThreeTitle.Init(nodeData, null);
			levelThreeTitle.OnClickSelfEvent = clickAction;
			levelThreeTitle.ParentElement = parent;
			return levelThreeTitle;
		}

		// Token: 0x06008321 RID: 33569 RVA: 0x003D1520 File Offset: 0x003CF720
		private NodeData GetNewNodeDataForFavor(NodeData originalNodeData)
		{
			return new NodeData
			{
				Id = originalNodeData.Id,
				ParentId = originalNodeData.ParentId,
				NodeContentType = originalNodeData.NodeContentType,
				NodeLayerType = originalNodeData.NodeLayerType,
				Children = new List<int>(),
				TemplateId = originalNodeData.TemplateId,
				Title = originalNodeData.Title,
				DefaultDisplayChild = true
			};
		}

		// Token: 0x06008322 RID: 33570 RVA: 0x003D1594 File Offset: 0x003CF794
		private void UpdateAllFavoritesItem()
		{
			foreach (KeyValuePair<ValueTuple<NodeLayerType, int>, BasicInfoView.FavoriteDisplayInfo> item in this._favoriteDisplayInfos)
			{
				bool flag = item.Value.DisplayNodeData.NodeLayerType == NodeLayerType.Three;
				if (!flag)
				{
					item.Value.TitleUIElement.Refresh(TitleShowMode.Down);
					bool flag2 = item.Value.DisplayNodeData.NodeLayerType == NodeLayerType.Two;
					if (flag2)
					{
						item.Value.TitleUIElement.RefreshFavoriteIcon();
					}
				}
			}
		}

		// Token: 0x06008323 RID: 33571 RVA: 0x003D1640 File Offset: 0x003CF840
		private void OnClickFavorLevelOneTitle(TitleElement title)
		{
			List<int> chidrenId = title.NodeData.Children;
			bool flag = chidrenId == null || chidrenId.Count <= 0;
			if (!flag)
			{
				title.SetDropDown();
			}
		}

		// Token: 0x06008324 RID: 33572 RVA: 0x003D167C File Offset: 0x003CF87C
		private TitleElement CreateAndInitFavorLevelTwoTitle(TitleElement parent, NodeData nodeData, Action<TitleElement> clickAction)
		{
			TitleElement levelTwoTitle = SingletonBehaviour<ElementFactory>.Instance.CreateLevelTwoTitleItem(parent.ChildrenContainer);
			levelTwoTitle.Init(nodeData, null);
			levelTwoTitle.OnClickSelfEvent = clickAction;
			levelTwoTitle.ParentElement = parent;
			return levelTwoTitle;
		}

		// Token: 0x06008325 RID: 33573 RVA: 0x003D16B8 File Offset: 0x003CF8B8
		private void OnClickFavoritesLevelTwoTitle(TitleElement title)
		{
			bool flag = this.currentFavorLevelTwoTitle != title && this.currentFavorLevelTwoTitle != null;
			if (flag)
			{
				this.currentFavorLevelTwoTitle.Refresh(TitleShowMode.Normal);
			}
			this.currentFavorLevelTwoIndex = title.Index;
			this.currentFavorLevelTwoTitle = title;
			List<int> chidrenId = title.NodeData.Children;
			bool flag2 = chidrenId == null || chidrenId.Count <= 0;
			if (!flag2)
			{
				title.SetDropDown();
			}
		}

		// Token: 0x06008326 RID: 33574 RVA: 0x003D1734 File Offset: 0x003CF934
		private void OnClickFavoritesLevelThreeTitle(TitleElement titleElement)
		{
			this.AddLabelItem(titleElement.LinkId);
			NodeData realData = EncyclopediaDataManager.Instance.GetNodeData(titleElement.LinkIdName);
			bool flag = this.currentLevelThreeTitle == null || this.currentLevelThreeTitle != titleElement;
			if (flag)
			{
				bool flag2 = this.currentLevelThreeTitle != null;
				if (flag2)
				{
					this.currentLevelThreeTitle.Refresh(TitleShowMode.Normal);
				}
				this.currentLevelThreeTitle = titleElement;
				titleElement.Refresh(TitleShowMode.Highlight);
				this.RefreshPageDetail(realData, null);
				this.pageDetailElement.ScrollToLastPos();
			}
			bool flag3 = titleElement.IsShowChildren();
			if (flag3)
			{
				titleElement.SetDropDown();
			}
		}

		// Token: 0x06008327 RID: 33575 RVA: 0x003D17D8 File Offset: 0x003CF9D8
		private void OnClickFavoritesLevelFourTitle(TitleElement titleElement)
		{
			TitleElement parent = titleElement.ParentElement;
			this.AddLabelItem(parent.LinkId);
			bool flag = this.currentLevelFourTitle == null || this.currentLevelFourTitle != titleElement;
			if (flag)
			{
				TitleElement titleElement2 = this.currentLevelFourTitle;
				if (titleElement2 != null)
				{
					titleElement2.Refresh(TitleShowMode.Normal);
				}
				this.currentLevelFourTitle = titleElement;
				titleElement.Refresh(TitleShowMode.Highlight);
				this.RefreshPageDetail(parent.NodeData, null);
				this.pageDetailElement.JumpToContent(titleElement.NodeData.Id, null);
			}
		}

		// Token: 0x06008328 RID: 33576 RVA: 0x003D1868 File Offset: 0x003CFA68
		private void RefreshFavorItemsSelectStates(NodeLayerType layerType, int dataId)
		{
			foreach (KeyValuePair<ValueTuple<NodeLayerType, int>, BasicInfoView.FavoriteDisplayInfo> item in this._favoriteDisplayInfos)
			{
				bool titleShow = layerType == item.Key.Item1 && dataId == item.Key.Item2;
				item.Value.TitleUIElement.Refresh(titleShow ? TitleShowMode.Highlight : TitleShowMode.Normal);
			}
		}

		// Token: 0x06008329 RID: 33577 RVA: 0x003D18F4 File Offset: 0x003CFAF4
		private void RefreshFavorItemsSelectStates()
		{
			foreach (KeyValuePair<ValueTuple<NodeLayerType, int>, BasicInfoView.FavoriteDisplayInfo> item in this._favoriteDisplayInfos)
			{
				item.Value.TitleUIElement.Refresh(TitleShowMode.Normal);
			}
		}

		// Token: 0x0600832A RID: 33578 RVA: 0x003D1958 File Offset: 0x003CFB58
		private void JumpToLevelThreeFavor(int id)
		{
			NodeData threeData = EncyclopediaDataManager.Instance.GetNodeData(id);
			this.JumpToLevelTwoFavor(threeData.ParentId);
			BasicInfoView.FavoriteDisplayInfo titleElement;
			bool flag = this._favoriteDisplayInfos.TryGetValue(new ValueTuple<NodeLayerType, int>(NodeLayerType.Three, id), out titleElement);
			if (flag)
			{
				this.OnClickFavoritesLevelThreeTitle(titleElement.TitleUIElement);
			}
		}

		// Token: 0x0600832B RID: 33579 RVA: 0x003D19A8 File Offset: 0x003CFBA8
		private void JumpToLevelTwoFavor(int id)
		{
			BasicInfoView.FavoriteDisplayInfo twoTitleElement;
			bool flag = this._favoriteDisplayInfos.TryGetValue(new ValueTuple<NodeLayerType, int>(NodeLayerType.Two, id), out twoTitleElement);
			if (flag)
			{
				this.OnClickFavoritesLevelTwoTitle(twoTitleElement.TitleUIElement);
			}
		}

		// Token: 0x0600832C RID: 33580 RVA: 0x003D19E0 File Offset: 0x003CFBE0
		private void AddHistory(NodeLayerType titleLevel, int dataId, bool refreshDisplay = true)
		{
			bool flag = titleLevel != NodeLayerType.Three;
			if (!flag)
			{
				NodeData nodeData = EncyclopediaDataManager.Instance.GetNodeData(dataId);
				bool flag2 = nodeData == null;
				if (!flag2)
				{
					EncyclopediaDataManager.Instance.AddHistory(nodeData);
					if (refreshDisplay)
					{
						this.RefreshHistoryDisplay();
					}
				}
			}
		}

		// Token: 0x0600832D RID: 33581 RVA: 0x003D1A2B File Offset: 0x003CFC2B
		private void InitHistory()
		{
			this.historyScroll.OnItemRender -= this.OnItemRender;
			this.historyScroll.OnItemRender += this.OnItemRender;
		}

		// Token: 0x0600832E RID: 33582 RVA: 0x003D1A60 File Offset: 0x003CFC60
		private void OnItemRender(int index, GameObject refers)
		{
			TitleElement levelThreeTitle = refers.GetComponent<TitleElement>();
			NodeData nodeData = this._historyCache[index];
			levelThreeTitle.Init(nodeData, null);
			levelThreeTitle.OnClickSelfEvent = new Action<TitleElement>(this.OnClickHistoryLevelThreeTitle);
			levelThreeTitle.ParentElement = null;
		}

		// Token: 0x0600832F RID: 33583 RVA: 0x003D1AA4 File Offset: 0x003CFCA4
		private void RefreshHistoryDisplay()
		{
			this.historyScroll.SetDataCount(this._historyCache.Count);
		}

		// Token: 0x06008330 RID: 33584 RVA: 0x003D1ABE File Offset: 0x003CFCBE
		private void UpdateAllHistoryItem()
		{
			this._historyCache = EncyclopediaDataManager.Instance.GetHistoryDatas();
			this.RefreshHistoryDisplay();
		}

		// Token: 0x06008331 RID: 33585 RVA: 0x003D1AD8 File Offset: 0x003CFCD8
		private void OnClickHistoryLevelThreeTitle(TitleElement titleElement)
		{
			this.AddLabelItem(titleElement.LinkId);
			NodeData realData = EncyclopediaDataManager.Instance.GetNodeData(titleElement.LinkIdName);
			bool flag = this.currentLevelThreeTitle == null || this.currentLevelThreeTitle != titleElement;
			if (flag)
			{
				bool flag2 = this.currentLevelThreeTitle != null;
				if (flag2)
				{
					this.currentLevelThreeTitle.Refresh(TitleShowMode.Normal);
				}
				this.currentLevelThreeTitle = titleElement;
				titleElement.Refresh(TitleShowMode.Highlight);
				this.RefreshPageDetail(realData, null);
				this.pageDetailElement.ScrollToLastPos();
			}
			bool flag3 = titleElement.IsShowChildren();
			if (flag3)
			{
				titleElement.SetDropDown();
			}
			this.AddHistory(NodeLayerType.Three, titleElement.LinkId, false);
		}

		// Token: 0x06008332 RID: 33586 RVA: 0x003D1B8C File Offset: 0x003CFD8C
		private void RefreshHistoryItemsSelectStates(NodeLayerType layerType, int dataId)
		{
			for (int i = 0; i < this._historyCache.Count; i++)
			{
				bool titleShow = layerType == NodeLayerType.Three && dataId == this._historyCache[i].Id;
				GameObject refers = this.historyScroll.GetActiveCell(i);
				bool flag = refers != null;
				if (flag)
				{
					refers.GetComponent<TitleElement>().Refresh(titleShow ? TitleShowMode.Highlight : TitleShowMode.Normal);
				}
			}
		}

		// Token: 0x06008333 RID: 33587 RVA: 0x003D1C04 File Offset: 0x003CFE04
		private void RefreshHistoryItemsSelectStates()
		{
			for (int i = 0; i < this._historyCache.Count; i++)
			{
				GameObject refers = this.historyScroll.GetActiveCell(i);
				bool flag = refers != null && this.CurrentSelectedDataId >= 0;
				if (flag)
				{
					TitleElement title = refers.GetComponent<TitleElement>();
					title.Refresh((this.CurrentSelectedDataId == title.LinkId) ? TitleShowMode.Highlight : TitleShowMode.Normal);
				}
			}
		}

		// Token: 0x06008334 RID: 33588 RVA: 0x003D1C7C File Offset: 0x003CFE7C
		private void RefreshHistoryFavoriteIconSingle(int dataId)
		{
			for (int i = 0; i < this._historyCache.Count; i++)
			{
				bool flag = this._historyCache[i].Id == dataId;
				if (flag)
				{
					GameObject refers = this.historyScroll.GetActiveCell(i);
					bool flag2 = refers != null;
					if (flag2)
					{
						TitleElement titleElement = refers.GetComponent<TitleElement>();
						titleElement.RefreshFavoriteIcon();
					}
					break;
				}
			}
		}

		// Token: 0x17000E66 RID: 3686
		// (get) Token: 0x06008335 RID: 33589 RVA: 0x003D1CED File Offset: 0x003CFEED
		private int CurrentSelectedDataId
		{
			get
			{
				NodeData nodeData = this.pageDetailElement.NodeData;
				return (nodeData != null) ? nodeData.Id : -1;
			}
		}

		// Token: 0x06008336 RID: 33590 RVA: 0x003D1D06 File Offset: 0x003CFF06
		private void InitLabels()
		{
			this._labelInfos = new List<int>();
			this.UpdateAllLabelItemDisplay();
		}

		// Token: 0x06008337 RID: 33591 RVA: 0x003D1D1C File Offset: 0x003CFF1C
		private void UpdateAllLabelItemDisplay()
		{
			this.clearHistoryButton.interactable = true;
			this.clearHistoryButton.gameObject.SetActive(this._labelInfos.Count > 1);
			this.pageDetailElement.UpdateTitleTips();
			int tempIndex = 0;
			for (int i = this._labelInfos.Count - 1; i >= 0; i--)
			{
				this.UpdateLabelItem(tempIndex, this._labelInfos[i]);
				tempIndex++;
			}
			for (int j = tempIndex; j < this.scrollRect.Content.childCount; j++)
			{
				GameObject child = this.scrollRect.Content.GetChild(j).gameObject;
				bool activeSelf = child.activeSelf;
				if (activeSelf)
				{
					child.SetActive(false);
				}
			}
			this.UpdatePinnedScroll();
		}

		// Token: 0x06008338 RID: 33592 RVA: 0x003D1DF8 File Offset: 0x003CFFF8
		private void UpdateLabelItem(int index, int dataId)
		{
			bool labelSelected = !EncyclopediaDataManager.Instance.IsHighlightPinned && this.CurrentSelectedDataId == dataId;
			LabelItem labelItem = SingletonBehaviour<ElementFactory>.Instance.GetOrCreateLabelItem(index, this.scrollRect.Content);
			labelItem.Init(dataId, labelSelected, new Action<int>(this.OnClickLabelItem), new Action<int>(this.OnClickDeleteLabel));
		}

		// Token: 0x06008339 RID: 33593 RVA: 0x003D1E58 File Offset: 0x003D0058
		private void OnClickLabelItem(int dataId)
		{
			NodeData nodeData = EncyclopediaDataManager.Instance.GetNodeData(dataId);
			EncyclopediaDataManager.Instance.IsHighlightPinned = false;
			bool flag = this.viewState == BasicInfoView.BasicInfoViewState.Normal;
			if (flag)
			{
				this.JumpTo(nodeData.Id, null);
			}
			else
			{
				TitleElement threeTitleElement = this.GetLevelThreeTitleElementById(nodeData.Id);
				bool levelOneKeyValid = this.currentLevelOneTitle != null && nodeData.LevelOneRoot.Key.Equals(this.currentLevelOneTitle.LinkIdName);
				bool flag2 = threeTitleElement != null && (this.currentLevelOneIndex == nodeData.LevelOneRoot.Id || levelOneKeyValid);
				if (flag2)
				{
					this.CancelSelectTitle();
					this.SelectTitle(threeTitleElement, true);
				}
				this.pageDetailElement.Init(nodeData, null);
				this.pageDetailElement.ScrollToLastPos();
			}
			this.RefreshLabelItemsSelectStates();
			this.RefreshFavorItemsSelectStates(nodeData.NodeLayerType, nodeData.Id);
			this.RefreshHistoryItemsSelectStates(nodeData.NodeLayerType, nodeData.Id);
		}

		// Token: 0x0600833A RID: 33594 RVA: 0x003D1F5B File Offset: 0x003D015B
		private void RefreshLabelItemsSelectStates()
		{
			this.UpdateAllLabelItemDisplay();
		}

		// Token: 0x0600833B RID: 33595 RVA: 0x003D1F65 File Offset: 0x003D0165
		private void OnClickDeleteLabel(int dataId)
		{
			this.RemoveLabelItem(dataId);
		}

		// Token: 0x0600833C RID: 33596 RVA: 0x003D1F70 File Offset: 0x003D0170
		public void AddLabelItem(string key)
		{
			NodeData data = EncyclopediaDataManager.Instance.GetNodeData(key);
			bool flag = data != null;
			if (flag)
			{
				this.AddLabelItem(data.Id);
			}
		}

		// Token: 0x0600833D RID: 33597 RVA: 0x003D1FA0 File Offset: 0x003D01A0
		public void AddLabelItem(int dataId)
		{
			int index = this._labelInfos.IndexOf(dataId);
			bool flag = index >= 0;
			if (flag)
			{
				EncyclopediaDataManager.Instance.IsHighlightPinned = false;
			}
			else
			{
				this._labelInfos.Add(dataId);
				SingletonObject.getInstance<YieldHelper>().DelayFrameDo(2U, delegate
				{
					this.scrollRect.ScrollBar.value = 0f;
				});
			}
			this.UpdateAllLabelItemDisplay();
		}

		// Token: 0x0600833E RID: 33598 RVA: 0x003D2004 File Offset: 0x003D0204
		internal void RemoveLabelItem(int dataId)
		{
			bool flag = !this._labelInfos.Contains(dataId);
			if (!flag)
			{
				int previousIndex = this._labelInfos.IndexOf(dataId);
				this._labelInfos.Remove(dataId);
				bool flag2 = this._labelInfos.Count == 0;
				if (flag2)
				{
					this.CheckDefaultLabelAndDisplay();
				}
				else
				{
					bool flag3 = this.CurrentSelectedDataId == dataId;
					if (flag3)
					{
						bool flag4 = previousIndex < this._labelInfos.Count;
						int nextSelectedIndex;
						if (flag4)
						{
							nextSelectedIndex = previousIndex;
						}
						else
						{
							nextSelectedIndex = this._labelInfos.Count - 1;
						}
						bool flag5 = nextSelectedIndex >= 0 && nextSelectedIndex < this._labelInfos.Count;
						if (flag5)
						{
							this.OnClickLabelItem(this._labelInfos[nextSelectedIndex]);
						}
						else
						{
							this.ShowMainPage();
						}
					}
					this.UpdateAllLabelItemDisplay();
					EncyclopediaDataManager.Instance.DetailPageScrollPosDict.Remove(dataId);
				}
			}
		}

		// Token: 0x0600833F RID: 33599 RVA: 0x003D20F7 File Offset: 0x003D02F7
		public void ClearTempLabel()
		{
			this._labelInfos.Clear();
			this.CheckDefaultLabelAndDisplay();
		}

		// Token: 0x06008340 RID: 33600 RVA: 0x003D2110 File Offset: 0x003D0310
		private void CheckDefaultLabelAndDisplay()
		{
			bool flag = this.viewState == BasicInfoView.BasicInfoViewState.Search && this._normalSearchResult.SelfSearchResultList.Count > 0;
			if (flag)
			{
				this.JumpToFirstSearchResult();
			}
			else
			{
				bool flag2 = this._labelInfos.Count > 0;
				if (flag2)
				{
					List<int> labelInfos = this._labelInfos;
					this.SetLevelThreePageDisplaySimple(labelInfos[labelInfos.Count - 1]);
					EncyclopediaDataManager.Instance.IsHighlightPinned = false;
				}
				else
				{
					bool flag3 = EncyclopediaDataManager.Instance.CurrentPinnedDataId >= 0 && EncyclopediaDataManager.Instance.PinnedNodeList.Contains(EncyclopediaDataManager.Instance.CurrentPinnedDataId);
					if (flag3)
					{
						this.OpenPinPage(EncyclopediaDataManager.Instance.CurrentPinnedDataId);
					}
					else
					{
						bool flag4 = EncyclopediaDataManager.Instance.PinnedNodeList.Count > 0;
						if (flag4)
						{
							EncyclopediaDataManager.Instance.CurrentPinnedDataId = EncyclopediaDataManager.Instance.PinnedNodeList[0];
							this.OpenPinPage(EncyclopediaDataManager.Instance.CurrentPinnedDataId);
						}
						else
						{
							this.ShowMainPage();
						}
					}
				}
			}
			this.UpdateAllLabelItemDisplay();
		}

		// Token: 0x06008341 RID: 33601 RVA: 0x003D2224 File Offset: 0x003D0424
		public bool PinTab(int dataId)
		{
			bool flag = EncyclopediaDataManager.Instance.PinnedNodeList.Add(dataId);
			bool result;
			if (flag)
			{
				EncyclopediaDataManager.Instance.CurrentPinnedDataId = dataId;
				this.UpdatePinnedScroll();
				this.pageDetailElement.UpdateTitleTips();
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x06008342 RID: 33602 RVA: 0x003D2270 File Offset: 0x003D0470
		public bool UnpinTab(int dataId)
		{
			int previousIndex = EncyclopediaDataManager.Instance.PinnedNodeList.IndexOf(dataId);
			bool flag = !EncyclopediaDataManager.Instance.PinnedNodeList.Remove(dataId);
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = EncyclopediaDataManager.Instance.CurrentPinnedDataId == dataId;
				if (flag2)
				{
					bool flag3 = EncyclopediaDataManager.Instance.PinnedNodeList.Count == 0;
					if (flag3)
					{
						EncyclopediaDataManager.Instance.CurrentPinnedDataId = -1;
					}
					else
					{
						bool flag4 = EncyclopediaDataManager.Instance.PinnedNodeList.Count > previousIndex;
						if (flag4)
						{
							EncyclopediaDataManager.Instance.CurrentPinnedDataId = EncyclopediaDataManager.Instance.PinnedNodeList[previousIndex];
						}
						else
						{
							EncyclopediaDataManager instance = EncyclopediaDataManager.Instance;
							OrderedHashSet<int> pinnedNodeList = EncyclopediaDataManager.Instance.PinnedNodeList;
							instance.CurrentPinnedDataId = pinnedNodeList[pinnedNodeList.Count - 1];
						}
					}
					bool isHighlightPinned = EncyclopediaDataManager.Instance.IsHighlightPinned;
					if (isHighlightPinned)
					{
						bool flag5 = EncyclopediaDataManager.Instance.CurrentPinnedDataId != -1;
						if (flag5)
						{
							NodeData nodeData = EncyclopediaDataManager.Instance.GetNodeData(EncyclopediaDataManager.Instance.CurrentPinnedDataId);
							TitleElement titleElement = this.GetLevelThreeTitleElementById(nodeData.Id);
							this.RefreshPageDetail(nodeData, null);
							this.UpdateAllLabelItemDisplay();
						}
						else
						{
							this.CheckDefaultLabelAndDisplay();
						}
					}
				}
				this.UpdatePinnedScroll();
				this.pageDetailElement.UpdateTitleTips();
				result = true;
			}
			return result;
		}

		// Token: 0x06008343 RID: 33603 RVA: 0x003D23C7 File Offset: 0x003D05C7
		private void UpdatePinnedScroll()
		{
			this.pinnedLabelItem.UpdateDisplay();
		}

		// Token: 0x06008344 RID: 33604 RVA: 0x003D23D8 File Offset: 0x003D05D8
		private void OpenPinPage(int dataId)
		{
			EncyclopediaDataManager.Instance.CurrentPinnedDataId = dataId;
			EncyclopediaDataManager.Instance.IsHighlightPinned = true;
			this.SetLevelThreePageDisplaySimple(dataId);
			this.UpdateAllLabelItemDisplay();
			bool flag = this.viewState == BasicInfoView.BasicInfoViewState.Normal;
			if (flag)
			{
				this.JumpTo(dataId, null);
			}
		}

		// Token: 0x06008345 RID: 33605 RVA: 0x003D2424 File Offset: 0x003D0624
		private void SetLevelThreePageDisplaySimple(int dataId)
		{
			NodeData nodeData = EncyclopediaDataManager.Instance.GetNodeData(dataId);
			TitleElement titleElement = this.GetLevelThreeTitleElementById(nodeData.Id);
			this.RefreshPageDetail(nodeData, null);
		}

		// Token: 0x06008346 RID: 33606 RVA: 0x003D2454 File Offset: 0x003D0654
		private void ShowMainPage()
		{
			int mainPageId = this.GetMainPageId();
			this.JumpTo(mainPageId, null);
		}

		// Token: 0x06008347 RID: 33607 RVA: 0x003D2474 File Offset: 0x003D0674
		private int GetMainPageId()
		{
			return 3;
		}

		// Token: 0x17000E67 RID: 3687
		// (get) Token: 0x06008348 RID: 33608 RVA: 0x003D2488 File Offset: 0x003D0688
		internal static string SearchValue
		{
			get
			{
				string result;
				if (!BasicInfoView.IsShowSearchResult || BasicInfoView.Instance.viewState != BasicInfoView.BasicInfoViewState.Search)
				{
					result = string.Empty;
				}
				else
				{
					BasicInfoView instance = BasicInfoView.Instance;
					string src;
					if (instance == null)
					{
						src = null;
					}
					else
					{
						TMP_InputField tmp_InputField = instance.searchInputField;
						src = ((tmp_InputField != null) ? tmp_InputField.text : null);
					}
					result = (Utility.GetValidInputString(src) ?? string.Empty);
				}
				return result;
			}
		}

		// Token: 0x17000E68 RID: 3688
		// (get) Token: 0x06008349 RID: 33609 RVA: 0x003D24DC File Offset: 0x003D06DC
		internal OptimizedHtmlPatternMatcher Searcher
		{
			get
			{
				OptimizedHtmlPatternMatcher searcher = this._searcher;
				return (((searcher != null) ? searcher.Pattern : null) == BasicInfoView.SearchValue) ? this._searcher : ((BasicInfoView.SearchValue != string.Empty) ? (this._searcher = new OptimizedHtmlPatternMatcher(BasicInfoView.SearchValue)) : null);
			}
		}

		// Token: 0x17000E69 RID: 3689
		// (get) Token: 0x0600834A RID: 33610 RVA: 0x003D2536 File Offset: 0x003D0736
		private bool SearchTitleOnly
		{
			get
			{
				return this.toggleGroupMode.GetActiveIndex() == 0;
			}
		}

		// Token: 0x17000E6A RID: 3690
		// (get) Token: 0x0600834B RID: 33611 RVA: 0x003D2546 File Offset: 0x003D0746
		internal SearchResult CurSearchResult
		{
			get
			{
				return this._normalSearchResult;
			}
		}

		// Token: 0x17000E6B RID: 3691
		// (get) Token: 0x0600834C RID: 33612 RVA: 0x003D2550 File Offset: 0x003D0750
		public static bool IsShowSearchResult
		{
			get
			{
				BasicInfoView instance = BasicInfoView.Instance;
				return ((instance != null) ? new BasicInfoView.BasicInfoViewState?(instance.viewState) : null) == BasicInfoView.BasicInfoViewState.Search;
			}
		}

		// Token: 0x0600834D RID: 33613 RVA: 0x003D2594 File Offset: 0x003D0794
		private void InitSearch()
		{
			BasicInfoView.CurSearchResultId = -1;
			this._curSearchResultIndex = 0;
			this.searchInputField.onEndEdit.RemoveAllListeners();
			this.searchInputField.onEndEdit.AddListener(new UnityAction<string>(this.OnSearchValueChanged));
			this.searchInputField.onValueChanged.RemoveAllListeners();
			this.searchInputField.onValueChanged.AddListener(new UnityAction<string>(this.OnSearchInputChanged));
			this.searchInputField.onSelect.RemoveAllListeners();
			this.searchInputField.onSelect.AddListener(new UnityAction<string>(this.OnSearchInputSelect));
			this.searchInputField.SetTextWithoutNotify(string.Empty);
			this.buttonLastSearch.ClearAndAddListener(new Action(this.JumpToLastSearchResult));
			this.buttonNextSearch.ClearAndAddListener(new Action(this.JumpToNextSearchResult));
			this.buttonClearSearch.ClearAndAddListener(new Action(this.ClearSearch));
			this.RefreshJumpButton();
			this.toggleGroupMode.OnActiveIndexChange -= this.OnClickSearchTypeButton;
			this.toggleGroupMode.Init(1);
			this.toggleGroupMode.OnActiveIndexChange += this.OnClickSearchTypeButton;
			this.RefreshSearchLayout();
			this.RefreshSearchMode();
		}

		// Token: 0x0600834E RID: 33614 RVA: 0x003D26E4 File Offset: 0x003D08E4
		private void OnClickSearchTypeButton(int newIndex, int oldIndex)
		{
			this.OnSearchValueChanged(Utility.GetValidInputString(this.searchInputField.text));
			this.RefreshSearchMode();
		}

		// Token: 0x0600834F RID: 33615 RVA: 0x003D2708 File Offset: 0x003D0908
		internal void OnSearchValueChanged(string value)
		{
			bool flag = BasicInfoView.Counter > 0;
			if (!flag)
			{
				value = Utility.GetValidInputString(value);
				this.RefreshSearchLayout();
				bool hasContent = !value.IsNullOrEmpty();
				bool flag2 = !hasContent;
				if (flag2)
				{
					this.ClearSearchResult();
					bool flag3 = this.viewState == BasicInfoView.BasicInfoViewState.Search;
					if (flag3)
					{
						this.SwitchToPreviousState();
					}
					this.RefreshLevelTwoTitle(true);
					BasicInfoView.CurSearchResultIndex = new SearchIndex(-1, -1, -1);
					this.pageDetailElement.RefreshSearchResultHighlight(null, false);
					this.RefreshJumpButton();
				}
				else
				{
					this.CalcSearchResult(value, this.SearchTitleOnly);
					this.SwitchToSearch();
					this.RefreshSearchResult(this._normalSearchResult);
					bool activeInHierarchy = base.gameObject.activeInHierarchy;
					if (activeInHierarchy)
					{
						base.StartCoroutine(this.DelayHideSuggestions);
					}
					else
					{
						this.suggestions.gameObject.SetActive(false);
					}
				}
			}
		}

		// Token: 0x17000E6C RID: 3692
		// (get) Token: 0x06008350 RID: 33616 RVA: 0x003D27E4 File Offset: 0x003D09E4
		private IEnumerator DelayHideSuggestions
		{
			get
			{
				yield return null;
				this.suggestions.gameObject.SetActive(false);
				yield break;
			}
		}

		// Token: 0x06008351 RID: 33617 RVA: 0x003D2804 File Offset: 0x003D0A04
		private void ClearSearchResult()
		{
			this._curSearchResultIndex = 0;
			foreach (SearchResultItem resultItem in this._normalSearchResult.SelfSearchResultList)
			{
				resultItem.NodeData.TempShowLevel = (resultItem.NodeData.TempTipLevel = EEncyclopediaContentLevel.None);
			}
			this._normalSearchResult.Clear();
		}

		// Token: 0x06008352 RID: 33618 RVA: 0x003D2888 File Offset: 0x003D0A88
		private void CalcSearchResult(string value, bool searchTitleOnly)
		{
			this.ClearSearchResult();
			bool flag = value.IsNullOrEmpty();
			if (!flag)
			{
				IReadOnlyList<NodeData> list = EncyclopediaDataManager.Instance.GetAllNodeDataList();
				foreach (NodeData nodeData in list)
				{
					foreach (SearchIndex result in nodeData.Search(value, searchTitleOnly, false, false))
					{
						SearchResultItem resultItem = new SearchResultItem(nodeData, result);
						this._normalSearchResult.SearchResultList.Add(resultItem);
						this.AddNode(nodeData, this._normalSearchResult.IncludeParentSearchResultList);
						bool flag2 = !this._normalSearchResult.SelfSearchResultList.Contains(resultItem);
						if (flag2)
						{
							this._normalSearchResult.SelfSearchResultList.Add(resultItem);
						}
					}
				}
			}
		}

		// Token: 0x06008353 RID: 33619 RVA: 0x003D2998 File Offset: 0x003D0B98
		private void RefreshJumpButton()
		{
			int count = this._normalSearchResult.SelfSearchResultList.Count;
			this.buttonLastSearch.interactable = (this.buttonNextSearch.interactable = (count > 1));
		}

		// Token: 0x06008354 RID: 33620 RVA: 0x003D29D8 File Offset: 0x003D0BD8
		private void AddNode(NodeData nodeData, List<NodeData> list)
		{
			NodeData parent = EncyclopediaDataManager.Instance.GetNodeData(nodeData.ParentId);
			bool flag = parent != null;
			if (flag)
			{
				this.AddNode(parent, list);
			}
			bool flag2 = !list.Contains(nodeData);
			if (flag2)
			{
				list.Add(nodeData);
			}
		}

		// Token: 0x06008355 RID: 33621 RVA: 0x003D2A20 File Offset: 0x003D0C20
		internal void RefreshSearchResult(SearchResult searchResult)
		{
			this.RefreshJumpButton();
			List<NodeData> levelOneNodeList = EncyclopediaDataManager.Instance.GetAllLevelOneData();
			int[] searchCount = new int[levelOneNodeList.Count];
			foreach (SearchResultItem resultItem in searchResult.SearchResultList)
			{
				int index = levelOneNodeList.IndexOf(resultItem.NodeData.LevelOneRoot);
				searchCount[index]++;
			}
			for (int i = 0; i < this.searchScrollRect.Content.childCount; i++)
			{
				this.searchScrollRect.Content.GetChild(i).gameObject.SetActive(false);
			}
			List<NodeData> levelOneList = (from n in searchResult.IncludeParentSearchResultList
			where n.NodeLayerType == NodeLayerType.One
			select n).ToList<NodeData>();
			for (int j = 0; j < levelOneList.Count; j++)
			{
				NodeData levelOneData = levelOneList[j];
				TitleElement levelOneTitle = SingletonBehaviour<ElementFactory>.Instance.GetOrCreateLevelOneTitleForSearch(this.searchScrollRect.Content, j);
				this.InitLevelTwoTitle(levelOneTitle, levelOneData, j, delegate(TitleElement t)
				{
					t.SetDropDown();
				});
				for (int k = 0; k < levelOneTitle.ChildrenContainer.childCount; k++)
				{
					levelOneTitle.ChildrenContainer.GetChild(k).gameObject.SetActive(false);
				}
				bool flag = !levelOneTitle.IsShowChildren();
				if (flag)
				{
					levelOneTitle.SetDropDown();
				}
				List<NodeData> levelTwoList = (from n in searchResult.IncludeParentSearchResultList
				where n.ParentId == levelOneData.Id && n.NodeLayerType == NodeLayerType.Two
				select n).ToList<NodeData>();
				int childIndex = 0;
				for (int l = 0; l < levelTwoList.Count; l++)
				{
					NodeData levelTwoData = levelTwoList[l];
					this.RefreshSearchResultForLevelThreeAndFour(searchResult, levelTwoData, levelOneTitle, ref childIndex);
				}
				int index2 = levelOneNodeList.IndexOf(levelOneData.LevelOneRoot);
				levelOneTitle.RefreshSearchCount(searchCount.CheckIndex(index2) ? searchCount[index2] : 0);
				levelOneTitle.RefreshButtonExtend();
			}
			this.pageDetailElement.RefreshSearchResultHighlight(this.Searcher, this.SearchTitleOnly);
			this.currentLevelTwoTitle = null;
			this.currentLevelThreeTitle = null;
			this.JumpToCurSearchResult();
		}

		// Token: 0x06008356 RID: 33622 RVA: 0x003D2CB8 File Offset: 0x003D0EB8
		private void RefreshSearchResultForLevelThreeAndFour(SearchResult searchResult, NodeData parentNode, TitleElement container, ref int childIndex)
		{
			List<int> children = parentNode.Children;
			bool flag = children == null || children.Count <= 0;
			if (!flag)
			{
				string searchText = Utility.GetValidInputString(this.searchInputField.text);
				for (int i = 0; i < parentNode.Children.Count; i++)
				{
					int childId = parentNode.Children[i];
					NodeData childNodeData = EncyclopediaDataManager.Instance.GetNodeData(childId);
					bool flag2 = parentNode.NodeLayerType == NodeLayerType.Three && childNodeData.NodeLayerType != NodeLayerType.Four;
					if (!flag2)
					{
						bool show = searchResult.IncludeParentSearchResultList.Contains(childNodeData) || (childNodeData.NodeLayerType == NodeLayerType.Four && searchResult.IncludeParentSearchResultList.Contains(parentNode));
						bool flag3 = show;
						if (flag3)
						{
							bool flag4 = parentNode.NodeLayerType == NodeLayerType.Two;
							if (flag4)
							{
								EEncyclopediaContentLevel tempLevel = EEncyclopediaContentLevel.None;
								bool flag5 = !this.SearchTitleOnly && !searchText.IsNullOrEmpty() && childNodeData.IsSearchOverLevel(false, true);
								if (flag5)
								{
									tempLevel = childNodeData.Level;
								}
								childNodeData.TempTipLevel = tempLevel;
								int num = childIndex;
								childIndex = num + 1;
								TitleElement childTitle = this.CreateAndInitLevelThreeTitle(container, num, childNodeData, new Action<TitleElement>(this.OnClickLevelThreeTitle));
								this.levelThreeElementsDict[childId] = childTitle;
								int levelFourIndex = 0;
								this.RefreshSearchResultForLevelThreeAndFour(searchResult, childTitle.NodeData, childTitle, ref levelFourIndex);
								bool flag6 = !childTitle.IsShowChildren();
								if (flag6)
								{
									childTitle.SetDropDown();
								}
							}
							else
							{
								bool flag7 = parentNode.NodeLayerType == NodeLayerType.Three;
								if (flag7)
								{
									EEncyclopediaContentLevel tempLevel2 = EEncyclopediaContentLevel.None;
									bool flag8 = !this.SearchTitleOnly && !searchText.IsNullOrEmpty() && childNodeData.IsSearchOverLevel(true, false);
									if (flag8)
									{
										tempLevel2 = childNodeData.Level;
									}
									childNodeData.TempTipLevel = tempLevel2;
									int num = childIndex;
									childIndex = num + 1;
									TitleElement childTitle2 = this.CreateAndInitLevelFourTitle(container, num, childNodeData, new Action<TitleElement>(this.OnClickLevelFourTitle));
									this.levelFourElementsDict[childId] = childTitle2;
								}
							}
						}
					}
				}
				for (int j = childIndex; j < container.ChildrenContainer.childCount; j++)
				{
					container.ChildrenContainer.GetChild(j).gameObject.SetActive(false);
				}
				container.RefreshButtonExtend();
			}
		}

		// Token: 0x06008357 RID: 33623 RVA: 0x003D2F08 File Offset: 0x003D1108
		private void JumpToCurSearchResult()
		{
			this.JumpToSearchResult(this._curSearchResultIndex);
		}

		// Token: 0x06008358 RID: 33624 RVA: 0x003D2F18 File Offset: 0x003D1118
		public void JumpToNextSearchResult()
		{
			bool flag = this.viewState != BasicInfoView.BasicInfoViewState.Search;
			if (!flag)
			{
				this.JumpToSearchResult(this._curSearchResultIndex + 1);
			}
		}

		// Token: 0x06008359 RID: 33625 RVA: 0x003D2F48 File Offset: 0x003D1148
		public void JumpToLastSearchResult()
		{
			bool flag = this.viewState != BasicInfoView.BasicInfoViewState.Search;
			if (!flag)
			{
				this.JumpToSearchResult(this._curSearchResultIndex - 1);
			}
		}

		// Token: 0x0600835A RID: 33626 RVA: 0x003D2F78 File Offset: 0x003D1178
		public void JumpToFirstSearchResult()
		{
			bool flag = this.viewState != BasicInfoView.BasicInfoViewState.Search;
			if (!flag)
			{
				this.JumpToSearchResult(0);
			}
		}

		// Token: 0x0600835B RID: 33627 RVA: 0x003D2FA0 File Offset: 0x003D11A0
		private void JumpToSearchResult(int index)
		{
			bool flag = this.viewState != BasicInfoView.BasicInfoViewState.Search;
			if (flag)
			{
				this.SwitchToSearch();
			}
			List<SearchResultItem> levelResultList = this._normalSearchResult.SelfSearchResultList;
			bool flag2 = levelResultList.Count == 0;
			if (flag2)
			{
				this.RefreshSearchResultState();
			}
			else
			{
				bool flag3 = index >= levelResultList.Count;
				if (flag3)
				{
					index = 0;
				}
				else
				{
					bool flag4 = index < 0;
					if (flag4)
					{
						index = levelResultList.Count - 1;
					}
				}
				this._curSearchResultIndex = Math.Clamp(index, 0, levelResultList.Count - 1);
				bool flag5 = levelResultList.CheckIndex(this._curSearchResultIndex);
				if (flag5)
				{
					SearchResultItem resultItem = levelResultList[this._curSearchResultIndex];
					BasicInfoView.CurSearchResultId = resultItem.NodeData.Id;
					BasicInfoView.CurSearchResultIndex = resultItem.SearchIndex;
					this.JumpTo(BasicInfoView.CurSearchResultId, BasicInfoView.CurSearchResultIndex);
				}
				this.RefreshSearchResultState();
			}
		}

		// Token: 0x0600835C RID: 33628 RVA: 0x003D3080 File Offset: 0x003D1280
		private void JumpToSearchResult()
		{
			List<SearchResultItem> levelResultList = this._normalSearchResult.SelfSearchResultList;
			bool flag = levelResultList.Count == 0;
			if (flag)
			{
				this.RefreshSearchResultState();
			}
			else
			{
				this._curSearchResultIndex = 0;
				bool flag2 = levelResultList.CheckIndex(this._curSearchResultIndex);
				if (flag2)
				{
					SearchResultItem resultItem = levelResultList[this._curSearchResultIndex];
					BasicInfoView.CurSearchResultId = resultItem.NodeData.Id;
					BasicInfoView.CurSearchResultIndex = resultItem.SearchIndex;
					this.JumpTo(BasicInfoView.CurSearchResultId, BasicInfoView.CurSearchResultIndex);
				}
			}
		}

		// Token: 0x0600835D RID: 33629 RVA: 0x003D3103 File Offset: 0x003D1303
		private void ClearSearch()
		{
			this.searchInputField.SetTextWithoutNotify(string.Empty);
			this.RefreshSearchLayout();
		}

		// Token: 0x0600835E RID: 33630 RVA: 0x003D311E File Offset: 0x003D131E
		private void RefreshSearchMode()
		{
			this.buttonLastSearch.gameObject.SetActive(true);
			this.buttonNextSearch.gameObject.SetActive(true);
			this.RefreshSearchResultState();
		}

		// Token: 0x0600835F RID: 33631 RVA: 0x003D314C File Offset: 0x003D134C
		private void RefreshSearchResultState()
		{
			int count = this._normalSearchResult.SelfSearchResultList.Count;
			int curIndex = Math.Clamp(this._curSearchResultIndex + 1, 0, count);
			this.textSearchResultState.text = LanguageKey.LK_Encyclopedia_Search_Result_All.TrFormat(curIndex, count);
		}

		// Token: 0x06008360 RID: 33632 RVA: 0x003D31A0 File Offset: 0x003D13A0
		private void RefreshSearchLayout()
		{
			bool hasContent = !Utility.GetValidInputString(this.searchInputField.text).IsNullOrEmpty();
			this.searchResultLayout.gameObject.SetActive(hasContent);
			this.buttonClearSearch.gameObject.SetActive(hasContent);
		}

		// Token: 0x06008361 RID: 33633 RVA: 0x003D31EB File Offset: 0x003D13EB
		private void OnSearchInputSelect(string value)
		{
			this.RefreshSearchAssociate();
		}

		// Token: 0x06008362 RID: 33634 RVA: 0x003D31F5 File Offset: 0x003D13F5
		private void OnSearchInputChanged(string value)
		{
			this.RefreshSearchAssociate();
		}

		// Token: 0x06008363 RID: 33635 RVA: 0x003D3200 File Offset: 0x003D1400
		private void RefreshSearchAssociate()
		{
			string inputText = Utility.GetValidInputString(this.searchInputField.text);
			HashSet<string> dup = EasyPool.Get<HashSet<string>>();
			IEncyclopediaSearchableContent[] related = (from x in (from x in EncyclopediaReference.DataArray.AsEnumerable<IEncyclopediaSearchableContent>().Concat(EncyclopediaContent.DataArray.AsEnumerable<IEncyclopediaSearchableContent>())
			where x.Contains(inputText)
			select x).OrderBy(delegate(IEncyclopediaSearchableContent x)
			{
				bool item = !x.StartsWith(inputText);
				int length = x.Length;
				string content = x.Content;
				if (!true)
				{
				}
				EncyclopediaReferenceItem encyclopediaReferenceItem = x as EncyclopediaReferenceItem;
				int item2;
				if (encyclopediaReferenceItem != null)
				{
					EEncyclopediaReferenceInsertType insertType = encyclopediaReferenceItem.InsertType;
					if (insertType == EEncyclopediaReferenceInsertType.HyperLink)
					{
						string[] desc = encyclopediaReferenceItem.Desc;
						if (desc != null)
						{
							int num = desc.Length;
							if (num > 1)
							{
								item2 = 0;
								goto IL_6C;
							}
						}
					}
				}
				else if (x is EncyclopediaContentItem)
				{
					item2 = 1;
					goto IL_6C;
				}
				item2 = 2;
				IL_6C:
				if (!true)
				{
				}
				return new ValueTuple<bool, int, string, int>(item, length, content, item2);
			})
			where dup.Add(x.Content)
			select x).Take(10).ToArray<IEncyclopediaSearchableContent>();
			EasyPool.Free<HashSet<string>>(dup);
			bool flag = related.Length == 0;
			if (flag)
			{
				this.suggestions.gameObject.SetActive(false);
			}
			else
			{
				this.suggestions.gameObject.SetActive(true);
				this.suggestions.Rebuild<EncyclopediaTextLinkHelper>(related.Length, delegate(EncyclopediaTextLinkHelper t, int i)
				{
					t.Set(related[i], this.searchInputField);
				});
			}
		}

		// Token: 0x0400645A RID: 25690
		[SerializeField]
		private CButton clearHistoryButton;

		// Token: 0x0400645B RID: 25691
		[SerializeField]
		private CButton favoriteButton;

		// Token: 0x0400645C RID: 25692
		[SerializeField]
		private GameObject favoriteSelected;

		// Token: 0x0400645D RID: 25693
		[SerializeField]
		private CButton historyButton;

		// Token: 0x0400645E RID: 25694
		[SerializeField]
		private GameObject historySelected;

		// Token: 0x0400645F RID: 25695
		[SerializeField]
		private GameObject indexerGameObject;

		// Token: 0x04006460 RID: 25696
		[SerializeField]
		private GameObject historyGameObject;

		// Token: 0x04006461 RID: 25697
		[SerializeField]
		private GameObject favoriteGameObject;

		// Token: 0x04006462 RID: 25698
		[SerializeField]
		private GameObject searchGameObject;

		// Token: 0x04006463 RID: 25699
		[SerializeField]
		private Transform levelOneContainer;

		// Token: 0x04006464 RID: 25700
		[SerializeField]
		internal PageDetailElement pageDetailElement;

		// Token: 0x04006465 RID: 25701
		[SerializeField]
		private CScrollRect normalScrollRect;

		// Token: 0x04006466 RID: 25702
		[SerializeField]
		private CScrollRect searchScrollRect;

		// Token: 0x04006467 RID: 25703
		private List<TitleElement> levelOneElements;

		// Token: 0x04006468 RID: 25704
		private List<TitleElement> levelTwoElements;

		// Token: 0x04006469 RID: 25705
		private Dictionary<int, TitleElement> levelThreeElementsDict;

		// Token: 0x0400646A RID: 25706
		private Dictionary<int, TitleElement> levelFourElementsDict;

		// Token: 0x0400646B RID: 25707
		private int _indexButtonIndex = -1;

		// Token: 0x0400646C RID: 25708
		private int maxLevelOneNum;

		// Token: 0x0400646D RID: 25709
		private int currentLevelOneIndex = -1;

		// Token: 0x0400646E RID: 25710
		internal TitleElement currentLevelOneTitle;

		// Token: 0x0400646F RID: 25711
		private TitleElement currentLevelTwoTitle;

		// Token: 0x04006470 RID: 25712
		private TitleElement currentLevelThreeTitle;

		// Token: 0x04006471 RID: 25713
		private TitleElement currentLevelFourTitle;

		// Token: 0x04006472 RID: 25714
		private BasicInfoView.BasicInfoViewState viewState;

		// Token: 0x04006473 RID: 25715
		private BasicInfoView.BasicInfoViewState previousState;

		// Token: 0x04006474 RID: 25716
		private Action<BasicInfoView.BasicInfoViewState, BasicInfoView.BasicInfoViewState> _onViewStateChange;

		// Token: 0x04006475 RID: 25717
		[NonSerialized]
		public bool NeedInit = true;

		// Token: 0x04006476 RID: 25718
		[NonSerialized]
		public string JumpKey;

		// Token: 0x04006477 RID: 25719
		internal static BasicInfoView Instance;

		// Token: 0x04006478 RID: 25720
		private bool _isLinkElementClick = false;

		// Token: 0x04006479 RID: 25721
		private Sequence _seq;

		// Token: 0x0400647A RID: 25722
		[SerializeField]
		private RectTransform favoritesContainer;

		// Token: 0x0400647B RID: 25723
		private Dictionary<ValueTuple<NodeLayerType, int>, BasicInfoView.FavoriteDisplayInfo> _favoriteDisplayInfos = new Dictionary<ValueTuple<NodeLayerType, int>, BasicInfoView.FavoriteDisplayInfo>();

		// Token: 0x0400647C RID: 25724
		private TitleElement currentFavorLevelTwoTitle;

		// Token: 0x0400647D RID: 25725
		private TitleElement currentFavorLevelThreeTitle;

		// Token: 0x0400647E RID: 25726
		private int currentFavorLevelTwoIndex;

		// Token: 0x0400647F RID: 25727
		private int currentFavorLevelThreeIndex;

		// Token: 0x04006480 RID: 25728
		private int currentFavoritesLevelTwoIndex = 0;

		// Token: 0x04006481 RID: 25729
		[TupleElementNames(new string[]
		{
			"titleLayer",
			"dataId"
		})]
		private HashSet<ValueTuple<NodeLayerType, int>> _toRemoveFavoriteCache = new HashSet<ValueTuple<NodeLayerType, int>>();

		// Token: 0x04006482 RID: 25730
		[SerializeField]
		private InfinityScroll historyScroll;

		// Token: 0x04006483 RID: 25731
		private List<NodeData> _historyCache = new List<NodeData>();

		// Token: 0x04006484 RID: 25732
		[SerializeField]
		private CScrollRect scrollRect;

		// Token: 0x04006485 RID: 25733
		[SerializeField]
		internal LabelItemFixed pinnedLabelItem;

		// Token: 0x04006486 RID: 25734
		private List<int> _labelInfos;

		// Token: 0x04006487 RID: 25735
		[SerializeField]
		private TMP_InputField searchInputField;

		// Token: 0x04006488 RID: 25736
		[SerializeField]
		private GameObject searchResultLayout;

		// Token: 0x04006489 RID: 25737
		[SerializeField]
		private CToggleGroup toggleGroupMode;

		// Token: 0x0400648A RID: 25738
		[SerializeField]
		private CButton buttonLastSearch;

		// Token: 0x0400648B RID: 25739
		[SerializeField]
		private CButton buttonNextSearch;

		// Token: 0x0400648C RID: 25740
		[SerializeField]
		private CButton buttonClearSearch;

		// Token: 0x0400648D RID: 25741
		[SerializeField]
		private TextMeshProUGUI textSearchResultState;

		// Token: 0x0400648E RID: 25742
		[SerializeField]
		private TemplatedContainerAssemblyNew suggestions;

		// Token: 0x0400648F RID: 25743
		private OptimizedHtmlPatternMatcher _searcher;

		// Token: 0x04006490 RID: 25744
		private readonly SearchResult _normalSearchResult = new SearchResult();

		// Token: 0x04006491 RID: 25745
		private int _curSearchResultIndex;

		// Token: 0x04006492 RID: 25746
		internal static readonly object Locker = new object();

		// Token: 0x04006493 RID: 25747
		internal static int Counter = 0;

		// Token: 0x04006494 RID: 25748
		public static int CurSearchResultId = -1;

		// Token: 0x04006495 RID: 25749
		public static SearchIndex CurSearchResultIndex = new SearchIndex(-1, -1, -1);

		// Token: 0x0200200F RID: 8207
		internal enum BasicInfoViewState
		{
			// Token: 0x0400CFD8 RID: 53208
			None,
			// Token: 0x0400CFD9 RID: 53209
			Normal,
			// Token: 0x0400CFDA RID: 53210
			History,
			// Token: 0x0400CFDB RID: 53211
			Favorite,
			// Token: 0x0400CFDC RID: 53212
			Search
		}

		// Token: 0x02002010 RID: 8208
		private class FavoriteDisplayInfo
		{
			// Token: 0x0400CFDD RID: 53213
			public TitleElement TitleUIElement;

			// Token: 0x0400CFDE RID: 53214
			public NodeData DisplayNodeData;
		}

		// Token: 0x02002011 RID: 8209
		private class HistoryDisplayInfo
		{
			// Token: 0x0400CFDF RID: 53215
			public TitleElement TitleUIElement;

			// Token: 0x0400CFE0 RID: 53216
			public NodeData DisplayNodeData;
		}
	}
}
