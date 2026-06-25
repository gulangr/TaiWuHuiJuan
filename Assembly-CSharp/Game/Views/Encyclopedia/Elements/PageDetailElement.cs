using System;
using System.Collections.Generic;
using System.Linq;
using FrameWork.Tools;
using FrameWork.UISystem.UIElements;
using Game.Views.Encyclopedia.Event;
using Game.Views.Encyclopedia.Save;
using Game.Views.Encyclopedia.SyntaxTree;
using Game.Views.Encyclopedia.Utilities;
using Game.Views.Encyclopedia.Views;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.Views.Encyclopedia.Elements
{
	// Token: 0x02000A8B RID: 2699
	public class PageDetailElement : Element, ISearch
	{
		// Token: 0x06008428 RID: 33832 RVA: 0x003D7090 File Offset: 0x003D5290
		private void Awake()
		{
			this.scrollRect.OnScrollEnd = delegate()
			{
				this.OnUpdateScrollValue(0.5f);
			};
			this.scrollRect.OnScrollEvent += this.OnScroll;
			this._favoriteTip = this.favoriteIcon.GetComponent<TooltipInvoker>();
		}

		// Token: 0x06008429 RID: 33833 RVA: 0x003D70E0 File Offset: 0x003D52E0
		private void OnEnable()
		{
			base.Init(base.NodeData, null);
			this.favoriteIcon.onValueChanged.AddListener(new UnityAction<int>(this.OnFavoriteTypeChanged));
			this.pinButton.onClick.AddListener(new UnityAction(this.OnPinTitleButtonClicked));
		}

		// Token: 0x0600842A RID: 33834 RVA: 0x003D7136 File Offset: 0x003D5336
		private void OnDisable()
		{
			this.favoriteIcon.onValueChanged.RemoveListener(new UnityAction<int>(this.OnFavoriteTypeChanged));
			this.pinButton.onClick.RemoveListener(new UnityAction(this.OnPinTitleButtonClicked));
		}

		// Token: 0x0600842B RID: 33835 RVA: 0x003D7174 File Offset: 0x003D5374
		internal void RefreshFavoriteIcon()
		{
			bool isDone = Save.SaveData.FavoritesInfos.Contains(base.NodeData.ConfigItem.Key);
			this.favoriteIcon.UpdateState(isDone ? 0 : 1);
			bool flag = this._favoriteTip;
			if (flag)
			{
				LanguageKey languageKey = isDone ? LanguageKey.LK_Encyclopedia_Favorite_Button_Done : LanguageKey.LK_Encyclopedia_Favorite_Button;
				this._favoriteTip.PresetParam = new string[]
				{
					languageKey.Tr()
				};
			}
		}

		// Token: 0x0600842C RID: 33836 RVA: 0x003D71F0 File Offset: 0x003D53F0
		protected override void OnInit()
		{
			this.selectedElement = null;
			this.showInLevelFour.Init(base.NodeData);
			this.ShowEmpty(false);
			bool isSelecting = BasicInfoView.IsShowSearchResult && base.NodeData.Id == BasicInfoView.CurSearchResultId;
			this.title.text = Utility.GetHighlightText(base.NodeData.Title, base.NodeData, null, BasicInfoView.CurSearchResultIndex.SingleTextIndex, isSelecting);
			this.desc.text = string.Empty;
			this.ChildrenElements.Clear();
			this.favoriteIcon.gameObject.SetActive(base.NodeData.NodeLayerType == NodeLayerType.Three);
			this.favoriteIcon.UpdateState(Save.SaveData.FavoritesInfos.Contains(base.NodeData.Key) ? 0 : 1);
			this.ClearCache();
			this.childrenContainer.scrollContainer = this.scrollRect;
			this.CreateChildren(base.NodeData, null);
			this.UpdateTitleTips();
			this.RefreshFavoriteIcon();
			LayoutRebuilder.ForceRebuildLayoutImmediate(this.childrenContainer.self);
		}

		// Token: 0x0600842D RID: 33837 RVA: 0x003D7318 File Offset: 0x003D5518
		private void ClearCache()
		{
			foreach (KeyValuePair<NodeContentType, List<Element>> keyValuePair in this._cachedElementDict)
			{
				NodeContentType nodeContentType;
				List<Element> list2;
				keyValuePair.Deconstruct(out nodeContentType, out list2);
				List<Element> list = list2;
				foreach (Element element in list)
				{
					element.gameObject.SetActive(false);
				}
			}
			this._curElementCountDict.Clear();
		}

		// Token: 0x0600842E RID: 33838 RVA: 0x003D73D0 File Offset: 0x003D55D0
		private void CreateChildren(NodeData nodeData, Element element)
		{
			bool flag = this.resetCurrentNodeDataShowLevel;
			if (flag)
			{
				nodeData.TempShowLevel = EEncyclopediaContentLevel.None;
			}
			bool flag2 = nodeData.Children == null;
			if (!flag2)
			{
				foreach (int id in nodeData.Children)
				{
					NodeData elementData = EncyclopediaDataManager.Instance.GetNodeData(id);
					bool canShow = this._targetNodeData == null || elementData == this._targetNodeData || elementData.Parent == this._targetNodeData;
					bool flag3 = !canShow;
					if (!flag3)
					{
						TitleTextElement levelFourTitle = element as TitleTextElement;
						bool flag4 = levelFourTitle != null;
						if (flag4)
						{
							levelFourTitle.childrenContainer.scrollContainer = this.scrollRect;
						}
						Element elem = this.CreateElement(elementData, element);
						this.CreateChildren(elementData, elem);
					}
				}
			}
		}

		// Token: 0x0600842F RID: 33839 RVA: 0x003D74C8 File Offset: 0x003D56C8
		private Element CreateElement(NodeData elementData, Element element)
		{
			int count;
			this._curElementCountDict.TryGetValue(elementData.NodeContentType, out count);
			List<Element> cachedElementList;
			bool flag = !this._cachedElementDict.TryGetValue(elementData.NodeContentType, out cachedElementList);
			if (flag)
			{
				cachedElementList = new List<Element>();
				this._cachedElementDict[elementData.NodeContentType] = cachedElementList;
			}
			LeveledContainer container = this.childrenContainer;
			TitleTextElement levelFourTitle = element as TitleTextElement;
			bool flag2 = levelFourTitle != null;
			if (flag2)
			{
				container = levelFourTitle.childrenContainer;
			}
			EEncyclopediaContentLevel eencyclopediaContentLevel = elementData.ConfigItem.Level & EEncyclopediaContentLevel.LowMidHigh;
			if (!true)
			{
			}
			RectTransform rectTransform;
			if (elementData.ConfigItem.Layer != EEncyclopediaContentLayer.Four)
			{
				if (eencyclopediaContentLevel <= EEncyclopediaContentLevel.LowMid)
				{
					if (eencyclopediaContentLevel > EEncyclopediaContentLevel.Low)
					{
						rectTransform = container.mid;
					}
					else
					{
						rectTransform = container.low;
					}
				}
				else if (eencyclopediaContentLevel > EEncyclopediaContentLevel.LowMidHigh)
				{
					rectTransform = container.self;
				}
				else
				{
					rectTransform = container.high;
				}
			}
			else
			{
				rectTransform = container.bottom;
			}
			if (!true)
			{
			}
			RectTransform realRect = rectTransform;
			bool hasCache = count < cachedElementList.Count;
			Element child = hasCache ? cachedElementList[count] : SingletonBehaviour<ElementFactory>.Instance.CreateElement(elementData, realRect);
			bool flag3 = !child;
			Element result;
			if (flag3)
			{
				result = null;
			}
			else
			{
				bool flag4 = hasCache;
				if (flag4)
				{
					child.RectTransform.SetParent(realRect, false);
				}
				else
				{
					cachedElementList.Add(child);
				}
				child.Release(false);
				child.Init(elementData, null);
				this.ChildrenElements.Add(child);
				child.transform.SetSiblingIndex(this.ChildrenElements.Count);
				this._curElementCountDict[elementData.NodeContentType] = count + 1;
				result = child;
			}
			return result;
		}

		// Token: 0x06008430 RID: 33840 RVA: 0x003D766C File Offset: 0x003D586C
		public void JumpToContent(int id, SearchIndex searchIndex = null)
		{
			PageDetailElement.<>c__DisplayClass28_0 CS$<>8__locals1 = new PageDetailElement.<>c__DisplayClass28_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.searchIndex = searchIndex;
			this._jumping = true;
			this.selectedElement = null;
			foreach (Element element in this.ChildrenElements)
			{
				bool flag = element.NodeData.Id == id;
				if (flag)
				{
					this.SelectElement(element);
					break;
				}
				TableCollectionElement tableElem = element as TableCollectionElement;
				bool flag2;
				if (tableElem != null)
				{
					List<int> children = element.NodeData.Children;
					flag2 = (children != null && children.Contains(id));
				}
				else
				{
					flag2 = false;
				}
				bool flag3 = flag2;
				if (flag3)
				{
					this.SelectElement(element);
					int index = EncyclopediaReference.Instance[tableElem.NodeData.ConfigItem.Inserts[0]].Params.IndexOf(EncyclopediaReference.Instance[EncyclopediaDataManager.Instance.GetNodeData(id).ConfigItem.Inserts[0]].LinkId);
					tableElem.toggleGroup.Get(index).SetIsOnWithoutNotify(false);
					tableElem.toggleGroup.Set(index, false);
				}
			}
			bool flag4 = this.selectedElement != null;
			if (flag4)
			{
				this.UpdateFixedTableHeader();
				this.scrollRect.ScrollTo(CS$<>8__locals1.<JumpToContent>g__GetScrollToPos|2(), 0f);
				SingletonObject.getInstance<YieldHelper>().DelayFrameDo(1U, delegate
				{
					bool flag6 = CS$<>8__locals1.<>4__this.selectedElement == null;
					if (!flag6)
					{
						CS$<>8__locals1.<>4__this.scrollRect.ScrollTo(base.<JumpToContent>g__GetScrollToPos|2(), 0f);
					}
				});
				bool flag5 = CS$<>8__locals1.searchIndex != null;
				if (flag5)
				{
					NodeData nodeData = this.selectedElement.NodeData.Parent;
					Element titleTextElement = this.ChildrenElements.Find((Element e) => e.NodeData == nodeData);
					if (titleTextElement != null)
					{
						titleTextElement.Init(titleTextElement.NodeData, null);
					}
				}
			}
			else
			{
				this.ScrollToLastPos();
			}
			SingletonObject.getInstance<YieldHelper>().DelayFrameDo(5U, delegate
			{
				CS$<>8__locals1.<>4__this._jumping = false;
			});
		}

		// Token: 0x06008431 RID: 33841 RVA: 0x003D7880 File Offset: 0x003D5A80
		public void SelectElement(Element element)
		{
			this.selectedElement = element;
			NodeData nodeData = element.NodeData;
			bool flag = nodeData.ConfigItem.Layer == EEncyclopediaContentLayer.Content;
			if (flag)
			{
				nodeData = nodeData.Parent;
			}
			bool flag2 = nodeData.ConfigItem.Layer == EEncyclopediaContentLayer.Four;
			if (flag2)
			{
				this.selectedId = nodeData.Id;
			}
			else
			{
				Element element2 = this.ChildrenElements.FirstOrDefault((Element x) => x.NodeData.ConfigItem.Layer != EEncyclopediaContentLayer.Content);
				this.selectedId = ((element2 != null) ? element2.NodeData.Id : -1);
			}
		}

		// Token: 0x06008432 RID: 33842 RVA: 0x003D7918 File Offset: 0x003D5B18
		public void ScrollToLastPos()
		{
			this.selectedId = -1;
			Vector2 pos;
			Vector2 targetPos = EncyclopediaDataManager.Instance.DetailPageScrollPosDict.TryGetValue(base.NodeData.Id, out pos) ? pos : Vector2.zero;
			this.scrollRect.ScrollTo(targetPos, 0f);
		}

		// Token: 0x06008433 RID: 33843 RVA: 0x003D7968 File Offset: 0x003D5B68
		private void OnFavoriteTypeChanged(int isFavorite)
		{
			FavoriteTypeChangedEventArgs args = new FavoriteTypeChangedEventArgs
			{
				ToFavorite = (isFavorite == 0),
				DataId = base.NodeData.Id
			};
			IEventArgs arg = EventArgs<FavoriteTypeChangedEventArgs>.CreateEventArgs(args);
			EventManager.Instance.Dispatch(2, arg);
			this.RefreshFavoriteIcon();
			this._favoriteTip.Refresh(true, -1);
		}

		// Token: 0x06008434 RID: 33844 RVA: 0x003D79C8 File Offset: 0x003D5BC8
		public void OnPinTitleButtonClicked()
		{
			bool pinFlag = EncyclopediaDataManager.Instance.PinnedNodeList.Contains(base.NodeData.Id);
			PinTitleEventArgs args = new PinTitleEventArgs
			{
				DataId = base.NodeData.Id,
				ToPin = !pinFlag
			};
			IEventArgs arg = EventArgs<PinTitleEventArgs>.CreateEventArgs(args);
			EventManager.Instance.Dispatch(5, arg);
			this.UpdateTitleTips();
		}

		// Token: 0x06008435 RID: 33845 RVA: 0x003D7A34 File Offset: 0x003D5C34
		public void UpdateTitleTips()
		{
			bool pinFlag = EncyclopediaDataManager.Instance.PinnedNodeList.Contains(base.NodeData.Id);
			this.titleDisplayer.PresetParam[0] = (pinFlag ? LanguageKey.LK_Encyclopedia_HasPinnedThisTab.Tr() : LanguageKey.LK_Encyclopedia_PinThisTab.Tr());
			TooltipInvoker buttonPinTip = this.pinButton.GetComponent<TooltipInvoker>();
			bool flag = buttonPinTip;
			if (flag)
			{
				buttonPinTip.PresetParam[0] = this.titleDisplayer.PresetParam[0];
			}
			this.titleDisplayer.Refresh(false, -1);
			this.imgPinned.gameObject.SetActive(pinFlag);
		}

		// Token: 0x06008436 RID: 33846 RVA: 0x003D7AD0 File Offset: 0x003D5CD0
		public void RefreshSearchResultHighlight(OptimizedHtmlPatternMatcher value, bool onlyTitle = false)
		{
			bool isSelecting = BasicInfoView.IsShowSearchResult && base.NodeData.Id == BasicInfoView.CurSearchResultId;
			bool flag = this._cachedSearchingValue != ((value != null) ? value.Pattern : null);
			if (flag)
			{
				this._cachedSearchingValue = ((value != null) ? value.Pattern : null);
				this._currHighlightResult = Utility.GetHighlightText(base.NodeData.Title, base.NodeData, value, BasicInfoView.CurSearchResultIndex.SingleTextIndex, false);
			}
			this.title.text = (isSelecting ? Utility.GetHighlightText(base.NodeData.Title, base.NodeData, value, BasicInfoView.CurSearchResultIndex.SingleTextIndex, true) : this._currHighlightResult);
			this.desc.text = string.Empty;
			foreach (Element element in this.ChildrenElements)
			{
				ISearch search = element as ISearch;
				bool flag2 = search != null;
				if (flag2)
				{
					bool flag3 = !onlyTitle || element.NodeData.NodeLayerType != NodeLayerType.Content;
					if (flag3)
					{
						search.RefreshSearchResultHighlight(value, onlyTitle);
					}
				}
			}
		}

		// Token: 0x06008437 RID: 33847 RVA: 0x003D7C18 File Offset: 0x003D5E18
		public void ShowEmpty(bool show)
		{
			this.noContent.SetActive(show);
		}

		// Token: 0x06008438 RID: 33848 RVA: 0x003D7C28 File Offset: 0x003D5E28
		private void OnScroll()
		{
			bool flag = this.scrollRect.Content.anchoredPosition.y < 0.01f;
			if (flag)
			{
				EncyclopediaDataManager.Instance.DetailPageScrollPosDict.Remove(base.NodeData.Id);
			}
			else
			{
				EncyclopediaDataManager.Instance.DetailPageScrollPosDict[base.NodeData.Id] = this.scrollRect.Content.anchoredPosition;
			}
			this.UpdateFixedTableHeader();
			this.OnUpdateScrollValue(0.5f);
		}

		// Token: 0x06008439 RID: 33849 RVA: 0x003D7CB4 File Offset: 0x003D5EB4
		private void UpdateFixedTableHeader()
		{
			bool isSame;
			this.fixedTableHeaderLayout.transform.localScale = ((this.UpdateFixedTableHeader(base.NodeData, out isSame) || isSame) ? Vector3.one : Vector3.zero);
		}

		// Token: 0x0600843A RID: 33850 RVA: 0x003D7CF4 File Offset: 0x003D5EF4
		private bool UpdateFixedTableHeader(NodeData nodeData, out bool isSame)
		{
			isSame = false;
			bool flag = nodeData.Children == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				Rect viewportRect = CommonUtils.RectTransToScreenPos(this.scrollRect.Viewport, UIManager.Instance.UiCamera);
				foreach (int childId in nodeData.Children)
				{
					NodeData childNode = EncyclopediaDataManager.Instance.GetNodeData(childId);
					bool childIsSame;
					bool flag2 = this.UpdateFixedTableHeader(childNode, out childIsSame);
					if (flag2)
					{
						return true;
					}
					isSame = (isSame || childIsSame);
					bool isTable = childNode.NodeContentType == NodeContentType.Table;
					bool isTableCollection = childNode.NodeContentType == NodeContentType.TableCollection;
					bool flag3 = !isTable && !isTableCollection;
					if (!flag3)
					{
						bool flag4 = isTable;
						TableElement element;
						if (flag4)
						{
							List<Element> elements;
							bool flag5 = !this._cachedElementDict.TryGetValue(NodeContentType.Table, out elements);
							if (flag5)
							{
								return false;
							}
							TableElement tableElement = elements.Find((Element e) => e.NodeData == childNode) as TableElement;
							bool flag6 = tableElement == null || !tableElement.gameObject.activeInHierarchy || !tableElement.CellContainer.gameObject.activeInHierarchy;
							if (flag6)
							{
								continue;
							}
							element = tableElement;
						}
						else
						{
							List<Element> elements2;
							bool flag7 = !this._cachedElementDict.TryGetValue(NodeContentType.TableCollection, out elements2);
							if (flag7)
							{
								return false;
							}
							TableCollectionElement tableCollectionElement = elements2.Find((Element e) => e.NodeData == childNode) as TableCollectionElement;
							bool flag8 = tableCollectionElement == null || !tableCollectionElement.gameObject.activeInHierarchy || tableCollectionElement.tableElement == null;
							if (flag8)
							{
								continue;
							}
							element = tableCollectionElement.tableElement;
						}
						Rect elementRect = CommonUtils.RectTransToScreenPos(element.CellContainerRectTrans, UIManager.Instance.UiCamera);
						bool isOverlap = elementRect.Overlaps(viewportRect);
						bool flag9 = !isOverlap || elementRect.yMax < viewportRect.yMax || element.CellContainerRectTrans.rect.height <= this.scrollRect.Viewport.rect.height;
						if (!flag9)
						{
							isSame = (this._fixedTableHeaderElementList.Count > 0 && this._fixedTableHeaderElementList.All((CellElement f) => element.CellElementList.Any((CellElement e) => e.CellData.isColHeader && e.CellData.Equals(f.CellData))));
							bool flag10 = isSame;
							if (!flag10)
							{
								this._fixedTableHeaderElementList.Clear();
								int index = 0;
								float maxHeight = 0f;
								foreach (CellElement cellElement in element.CellElementList)
								{
									bool flag11 = !cellElement.CellData.isColHeader;
									if (!flag11)
									{
										CellElement fixedCellElement = (index < this.fixedTableHeaderLayout.transform.childCount) ? this.fixedTableHeaderLayout.transform.GetChild(index).gameObject.GetComponent<CellElement>() : Object.Instantiate<CellElement>(cellElement, this.fixedTableHeaderLayout.transform);
										maxHeight = Mathf.Max(maxHeight, cellElement.RectTransform.rect.height);
										fixedCellElement.Init(cellElement.CellData, cellElement.Level);
										fixedCellElement.RectTransform.SetSize(cellElement.RectTransform.rect.size);
										fixedCellElement.RectTransform.SetAnchor(cellElement.RectTransform.anchorMin, cellElement.RectTransform.anchorMax);
										fixedCellElement.RectTransform.anchoredPosition = cellElement.RectTransform.anchoredPosition;
										bool flag12 = !fixedCellElement.gameObject.activeSelf;
										if (flag12)
										{
											fixedCellElement.gameObject.SetActive(true);
										}
										this._fixedTableHeaderElementList.Add(fixedCellElement);
										index++;
									}
								}
								for (int i = index; i < this.fixedTableHeaderLayout.transform.childCount; i++)
								{
									this.fixedTableHeaderLayout.transform.GetChild(i).gameObject.SetActive(false);
								}
								Vector2 size = new Vector2(element.CellContainer.RectTransform.rect.width, maxHeight);
								this.fixedTableHeaderLayout.SetSize(size);
								this.fixedTableHeaderLayout.position = element.CellContainerRectTrans.position;
								this.fixedTableHeaderLayout.localPosition = this.fixedTableHeaderLayout.localPosition.SetY(0f);
								this.fixedTableHeaderLayout.transform.localScale = Vector3.one;
								return true;
							}
						}
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x0600843B RID: 33851 RVA: 0x003D8220 File Offset: 0x003D6420
		public void ClearFixedTableHeaderData()
		{
			this._fixedTableHeaderElementList.Clear();
		}

		// Token: 0x0600843C RID: 33852 RVA: 0x003D8230 File Offset: 0x003D6430
		public void OnUpdateScrollValue(float val)
		{
			PageDetailElement.<>c__DisplayClass42_0 CS$<>8__locals1 = new PageDetailElement.<>c__DisplayClass42_0();
			CS$<>8__locals1.<>4__this = this;
			bool flag;
			if (!this._jumping && base.NodeData != null)
			{
				BasicInfoView instance = BasicInfoView.Instance;
				object obj;
				if (instance == null)
				{
					obj = null;
				}
				else
				{
					TitleElement currentLevelOneTitle = instance.currentLevelOneTitle;
					obj = ((currentLevelOneTitle != null) ? currentLevelOneTitle.NodeData : null);
				}
				flag = (obj == null);
			}
			else
			{
				flag = true;
			}
			bool flag2 = flag;
			if (!flag2)
			{
				int id = base.NodeData.LevelOneRoot.Id;
				TitleElement currentLevelOneTitle2 = BasicInfoView.Instance.currentLevelOneTitle;
				int? num = (currentLevelOneTitle2 != null) ? new int?(currentLevelOneTitle2.NodeData.Id) : null;
				bool flag3 = !(id == num.GetValueOrDefault() & num != null);
				if (!flag3)
				{
					List<Element> childrenElements = this.ChildrenElements;
					bool flag4 = childrenElements != null && childrenElements.Count > 0;
					if (flag4)
					{
						bool flag5 = Mathf.Approximately(val, 0f);
						if (flag5)
						{
							this.SelectElement(this.ChildrenElements[0]);
							BasicInfoView.Instance.UpdateSelectedTitle(this.selectedId);
							return;
						}
					}
					CS$<>8__locals1.viewport = this.sightRect.ToWorldRect();
					bool flag6 = this.selectedElement != null;
					if (flag6)
					{
						Element selected = null;
						bool flag7 = this.selectedElement.GetComponent<RectTransform>().ToWorldRect().Overlaps(CS$<>8__locals1.viewport);
						if (flag7)
						{
							selected = this.selectedElement;
						}
						else
						{
							Element selectedElem = (from elem in this.ChildrenElements
							where elem.NodeData.ParentId == CS$<>8__locals1.<>4__this.selectedId
							select elem).FirstOrDefault(new Func<Element, bool>(CS$<>8__locals1.<OnUpdateScrollValue>g__Checker|0));
							bool flag8 = selectedElem != null;
							if (flag8)
							{
								selected = selectedElem;
							}
						}
						bool flag9 = selected != null;
						if (flag9)
						{
							this.SelectElement(selected);
							BasicInfoView.Instance.UpdateSelectedTitle(this.selectedId);
							return;
						}
					}
					Element element = (from elem in this.ChildrenElements
					where elem.NodeData.ConfigItem.Layer != EEncyclopediaContentLayer.Content
					select elem).Concat(from elem in this.ChildrenElements
					where elem.NodeData.ConfigItem.Layer == EEncyclopediaContentLayer.Content
					select elem).FirstOrDefault(new Func<Element, bool>(CS$<>8__locals1.<OnUpdateScrollValue>g__Checker|0));
					bool flag10 = element != null;
					if (flag10)
					{
						this.SelectElement(element);
						BasicInfoView.Instance.UpdateSelectedTitle(this.selectedId);
					}
					else
					{
						this.selectedElement = null;
					}
				}
			}
		}

		// Token: 0x0600843D RID: 33853 RVA: 0x003D8494 File Offset: 0x003D6694
		public int PushSelectedId()
		{
			bool flag = this._previousSelectId == -1;
			int result;
			if (flag)
			{
				result = (this._previousSelectId = this.selectedId);
			}
			else
			{
				result = -1;
			}
			return result;
		}

		// Token: 0x0600843E RID: 33854 RVA: 0x003D84C8 File Offset: 0x003D66C8
		public int PopSelectedId(int previousId)
		{
			bool flag = this._previousSelectId == -1 || previousId != this._previousSelectId;
			int result;
			if (flag)
			{
				result = -1;
			}
			else
			{
				this.JumpToContent(this._previousSelectId, null);
				BasicInfoView.Instance.UpdateSelectedTitle(this.selectedId);
				this._previousSelectId = -1;
				result = this.selectedId;
			}
			return result;
		}

		// Token: 0x04006537 RID: 25911
		[SerializeField]
		private TextMeshProUGUI title;

		// Token: 0x04006538 RID: 25912
		[SerializeField]
		private LeveledContainer childrenContainer;

		// Token: 0x04006539 RID: 25913
		[SerializeField]
		private RectTransform sightRect;

		// Token: 0x0400653A RID: 25914
		[SerializeField]
		private TextMeshProUGUI desc;

		// Token: 0x0400653B RID: 25915
		[SerializeField]
		private CScrollRect scrollRect;

		// Token: 0x0400653C RID: 25916
		[SerializeField]
		private MultiStateToggle favoriteIcon;

		// Token: 0x0400653D RID: 25917
		[SerializeField]
		private ShowInLevelFour showInLevelFour;

		// Token: 0x0400653E RID: 25918
		[SerializeField]
		private GameObject noContent;

		// Token: 0x0400653F RID: 25919
		[SerializeField]
		private CButton pinButton;

		// Token: 0x04006540 RID: 25920
		[SerializeField]
		private GameObject imgPinned;

		// Token: 0x04006541 RID: 25921
		[SerializeField]
		private RectTransform fixedTableHeaderLayout;

		// Token: 0x04006542 RID: 25922
		[SerializeField]
		private TooltipInvoker titleDisplayer;

		// Token: 0x04006543 RID: 25923
		private TooltipInvoker _favoriteTip;

		// Token: 0x04006544 RID: 25924
		[SerializeField]
		[ReadOnly]
		private Element selectedElement;

		// Token: 0x04006545 RID: 25925
		[SerializeField]
		[ReadOnly]
		private int selectedId = -1;

		// Token: 0x04006546 RID: 25926
		internal readonly List<Element> ChildrenElements = new List<Element>();

		// Token: 0x04006547 RID: 25927
		private readonly Dictionary<NodeContentType, List<Element>> _cachedElementDict = new Dictionary<NodeContentType, List<Element>>();

		// Token: 0x04006548 RID: 25928
		private readonly Dictionary<NodeContentType, int> _curElementCountDict = new Dictionary<NodeContentType, int>();

		// Token: 0x04006549 RID: 25929
		private readonly List<CellElement> _fixedTableHeaderElementList = new List<CellElement>();

		// Token: 0x0400654A RID: 25930
		public bool resetCurrentNodeDataShowLevel;

		// Token: 0x0400654B RID: 25931
		private string _currHighlightResult;

		// Token: 0x0400654C RID: 25932
		private bool _jumping = false;

		// Token: 0x0400654D RID: 25933
		private int _previousSelectId = -1;
	}
}
