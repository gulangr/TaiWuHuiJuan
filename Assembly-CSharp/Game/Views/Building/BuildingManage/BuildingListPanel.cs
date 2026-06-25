using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using Game.Components.Common;
using Game.Views.Make;
using GameData.Domains.Building;
using GameData.Domains.Map;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.Building.BuildingManage
{
	// Token: 0x02000BF1 RID: 3057
	public class BuildingListPanel : MonoBehaviour
	{
		// Token: 0x17001067 RID: 4199
		// (get) Token: 0x06009B56 RID: 39766 RVA: 0x0048BAE4 File Offset: 0x00489CE4
		private BuildingListPanel.TogKey CurTogKey
		{
			get
			{
				return (BuildingListPanel.TogKey)this.toggleGroupTab.GetActiveIndex();
			}
		}

		// Token: 0x17001068 RID: 4200
		// (get) Token: 0x06009B57 RID: 39767 RVA: 0x0048BAF1 File Offset: 0x00489CF1
		private List<BuildingBlockData> CurBuildingList
		{
			get
			{
				return (this._currentDisplayMode == BuildingListPanel.EPanelDisplayMode.Manage) ? this._normalBuildingDict[this.CurTogKey] : this._craftBuildingDict[this.CurTogKey];
			}
		}

		// Token: 0x06009B58 RID: 39768 RVA: 0x0048BB20 File Offset: 0x00489D20
		public void Init(MapBlockData mapBlockData, BuildingAreaData areaData, bool isTaiwuVillage, IAsyncMethodRequestHandler requestHandle)
		{
			this._currentDisplayMode = BuildingListPanel.EPanelDisplayMode.Manage;
			this._hasInit = false;
			this._mapBlockData = mapBlockData;
			this._location = this._mapBlockData.GetLocation();
			this._areaData = areaData;
			this._isTaiwuVillage = isTaiwuVillage;
			this._requestHandler = requestHandle;
			bool hasInit = this._hasInit;
			if (!hasInit)
			{
				this.toggleGroupTab.AddAllChildToggles();
				this.toggleGroupTab.Init(-1);
				ToggleGroupHotkeyController.Set(UIElement.BuildingManage, this.toggleGroupTab, 1, null);
				for (BuildingListPanel.TogKey togKey = BuildingListPanel.TogKey.Shop; togKey <= BuildingListPanel.TogKey.Resource; togKey++)
				{
					CToggle tog = this.toggleGroupTab.Get(togKey.ToInt());
					if (!true)
					{
					}
					string text;
					switch (togKey)
					{
					case BuildingListPanel.TogKey.Shop:
						text = LanguageKey.LK_Building_Shop.Tr();
						break;
					case BuildingListPanel.TogKey.Make:
						text = LanguageKey.LK_Make_Item.Tr();
						break;
					case BuildingListPanel.TogKey.Resource:
						text = LanguageKey.LK_Resource.Tr();
						break;
					default:
						if (!true)
						{
						}
						<PrivateImplementationDetails>.ThrowSwitchExpressionException(togKey);
						break;
					}
					if (!true)
					{
					}
					string togName = text;
					tog.GetComponentInChildren<TextMeshProUGUI>().SetText(togName, true);
				}
				this.toggleGroupTab.OnActiveIndexChange += this.OnTabActiveIndexChange;
				this.scroll.OnItemRender += this.OnItemRender;
				this.scroll.AddOnScrollEvent(new Action(this.OnScroll));
				this._scrollRect = this.scroll.Scroll;
				this._scrollCellWidth = this.scroll.srcPrefab.GetComponent<RectTransform>().rect.size.x;
				this.inputFieldSearch.onEndEdit.RemoveAllListeners();
				this.inputFieldSearch.onEndEdit.AddListener(new UnityAction<string>(this.OnEndEdit));
				this.inputFieldSearch.SetTextWithoutNotify(string.Empty);
				this.btnLeftArrow.ClearAndAddListener(delegate
				{
					this.OnClickBtnArrow(false);
				});
				this.btnRightArrow.ClearAndAddListener(delegate
				{
					this.OnClickBtnArrow(true);
				});
				this.btnLeftArrow.gameObject.SetActive(false);
				this.btnRightArrow.gameObject.SetActive(false);
			}
		}

		// Token: 0x06009B59 RID: 39769 RVA: 0x0048BD58 File Offset: 0x00489F58
		public void Refresh(List<BuildingBlockData> blockList, BuildingBlockData curBlockData, BuildingExceptionData buildingExceptionData)
		{
			this._blockList = blockList;
			this._buildingExceptionData = buildingExceptionData;
			foreach (KeyValuePair<BuildingListPanel.TogKey, List<BuildingBlockData>> keyValuePair in this._normalBuildingDict)
			{
				BuildingListPanel.TogKey togKey2;
				List<BuildingBlockData> list3;
				keyValuePair.Deconstruct(out togKey2, out list3);
				List<BuildingBlockData> list = list3;
				list.Clear();
			}
			foreach (KeyValuePair<BuildingListPanel.TogKey, List<BuildingBlockData>> keyValuePair in this._craftBuildingDict)
			{
				BuildingListPanel.TogKey togKey2;
				List<BuildingBlockData> list3;
				keyValuePair.Deconstruct(out togKey2, out list3);
				List<BuildingBlockData> list2 = list3;
				list2.Clear();
			}
			short i = 0;
			while ((int)i < blockList.Count)
			{
				BuildingBlockData blockData = blockList[(int)i];
				bool flag = blockData == null || blockData.TemplateId <= 0;
				if (!flag)
				{
					BuildingBlockItem configData = BuildingBlock.Instance[blockData.TemplateId];
					bool canMakeItem = configData.CanMakeItem;
					if (canMakeItem)
					{
						this._normalBuildingDict[BuildingListPanel.TogKey.Make].Add(blockData);
						bool artisanOrderAvailable = configData.ArtisanOrderAvailable;
						if (artisanOrderAvailable)
						{
							this._craftBuildingDict[BuildingListPanel.TogKey.Make].Add(blockData);
						}
					}
					else
					{
						bool isShop = configData.IsShop;
						if (isShop)
						{
							this._normalBuildingDict[BuildingListPanel.TogKey.Shop].Add(blockData);
							short templateId = configData.TemplateId;
							bool flag2 = templateId == 127 || templateId == 128;
							if (flag2)
							{
								this._craftBuildingDict[BuildingListPanel.TogKey.Shop].Add(blockData);
							}
						}
						else
						{
							bool flag3 = BuildingBlockData.IsResource(configData.Type);
							if (flag3)
							{
								this._normalBuildingDict[BuildingListPanel.TogKey.Resource].Add(blockData);
							}
						}
					}
				}
				i += 1;
			}
			for (BuildingListPanel.TogKey togKey = BuildingListPanel.TogKey.Shop; togKey <= BuildingListPanel.TogKey.Resource; togKey++)
			{
				CToggle tog = this.toggleGroupTab.Get(togKey.ToInt());
				bool hasCount = this._normalBuildingDict[togKey].Count > 0;
				if (!true)
				{
				}
				bool flag4;
				switch (togKey)
				{
				case BuildingListPanel.TogKey.Shop:
					flag4 = this._isTaiwuVillage;
					break;
				case BuildingListPanel.TogKey.Make:
					flag4 = true;
					break;
				case BuildingListPanel.TogKey.Resource:
					flag4 = this._isTaiwuVillage;
					break;
				default:
					throw new ArgumentOutOfRangeException();
				}
				if (!true)
				{
				}
				bool interactable = flag4;
				tog.interactable = (interactable && hasCount);
			}
			this.SelectBuilding(curBlockData, true);
		}

		// Token: 0x06009B5A RID: 39770 RVA: 0x0048C000 File Offset: 0x0048A200
		private void OnTabActiveIndexChange(int newIndex, int oldIndex)
		{
			this.RefreshScroll();
		}

		// Token: 0x06009B5B RID: 39771 RVA: 0x0048C00C File Offset: 0x0048A20C
		private void RefreshScroll()
		{
			this._sameBuildingDict.Clear();
			this._sameBuildingTemplateList.Clear();
			this._tempBuildingList.Clear();
			List<BuildingBlockData> originList = this.CurBuildingList;
			bool flag = this.inputFieldSearch.text.IsNullOrEmpty();
			if (flag)
			{
				this._tempBuildingList.AddRange(originList);
			}
			else
			{
				foreach (BuildingBlockData blockData in originList)
				{
					BuildingBlockKey blockKey = new BuildingBlockKey(this._location.AreaId, this._location.BlockId, blockData.BlockIndex);
					string buildingName = ViewBuildingManage.GetBuildingName(blockKey, blockData.TemplateId, this._mapBlockData.TemplateId, true, false);
					bool flag2 = buildingName.Contains(this.inputFieldSearch.text.Trim());
					if (flag2)
					{
						this._tempBuildingList.Add(blockData);
					}
				}
			}
			bool flag3 = this.CurTogKey == BuildingListPanel.TogKey.Make;
			if (flag3)
			{
				this._tempBuildingList.Sort(delegate(BuildingBlockData a, BuildingBlockData b)
				{
					TemplatedContainer.ButtonOrderType aOrder = TemplatedContainer.GetButtonOrderType(a.ConfigData.RequireLifeSkillType);
					TemplatedContainer.ButtonOrderType bOrder = TemplatedContainer.GetButtonOrderType(b.ConfigData.RequireLifeSkillType);
					return aOrder.CompareTo(bOrder);
				});
			}
			foreach (BuildingBlockData blockData2 in this._tempBuildingList)
			{
				List<BuildingBlockData> sameList;
				bool flag4 = !this._sameBuildingDict.TryGetValue((int)blockData2.TemplateId, out sameList);
				if (flag4)
				{
					sameList = new List<BuildingBlockData>();
					this._sameBuildingDict[(int)blockData2.TemplateId] = sameList;
				}
				sameList.Add(blockData2);
				bool flag5 = !this._sameBuildingTemplateList.Contains((int)blockData2.TemplateId);
				if (flag5)
				{
					this._sameBuildingTemplateList.Add((int)blockData2.TemplateId);
				}
			}
			this.scroll.SetDataCount(this._sameBuildingTemplateList.Count);
			int index = this._sameBuildingTemplateList.IndexOf(this._curTemplateId);
			this.scroll.ScrollTo(index, 0.3f);
			this._maxScrollContentPosX = (this._scrollRect.Content.rect.width - this._scrollRect.Viewport.rect.width) * -1f;
			this.OnScroll();
		}

		// Token: 0x06009B5C RID: 39772 RVA: 0x0048C294 File Offset: 0x0048A494
		private void OnItemRender(int index, GameObject obj)
		{
			int templateId = this._sameBuildingTemplateList[index];
			List<BuildingBlockData> sameList = this._sameBuildingDict[templateId];
			bool isSelected = this._curTemplateId == templateId;
			BuildingListItem item = obj.GetComponent<BuildingListItem>();
			int sameIndex = sameList.FindIndex((BuildingBlockData b) => b.BlockIndex == this._curBlockData.BlockIndex);
			bool flag = sameIndex < 0;
			if (flag)
			{
				sameIndex = 0;
			}
			item.Refresh(this.CurTogKey, this._mapBlockData, sameList, sameIndex, isSelected, new Action<BuildingBlockData>(this.SelectBuilding), this._buildingExceptionData, () => this._requestHandler);
		}

		// Token: 0x06009B5D RID: 39773 RVA: 0x0048C324 File Offset: 0x0048A524
		private BuildingBlockKey GetBuildingBlockKey(BuildingBlockData blockData)
		{
			return new BuildingBlockKey(this._location.AreaId, this._location.BlockId, blockData.BlockIndex);
		}

		// Token: 0x06009B5E RID: 39774 RVA: 0x0048C347 File Offset: 0x0048A547
		private void SelectBuilding(BuildingBlockData blockData)
		{
			this.SelectBuilding(blockData, false);
		}

		// Token: 0x06009B5F RID: 39775 RVA: 0x0048C354 File Offset: 0x0048A554
		private void SelectBuilding(BuildingBlockData blockData, bool isRefresh)
		{
			this._curBlockData = blockData;
			BuildingBlockItem config = BuildingBlock.Instance[blockData.TemplateId];
			BuildingListPanel.TogKey targetTogKey = config.CanMakeItem ? BuildingListPanel.TogKey.Make : (config.IsShop ? BuildingListPanel.TogKey.Shop : (BuildingBlockData.IsResource(config.Type) ? BuildingListPanel.TogKey.Resource : BuildingListPanel.TogKey.Invalid));
			bool flag = targetTogKey == BuildingListPanel.TogKey.Invalid;
			if (flag)
			{
				this._curTemplateId = -1;
				for (BuildingListPanel.TogKey togKey = BuildingListPanel.TogKey.Shop; togKey <= BuildingListPanel.TogKey.Resource; togKey++)
				{
					CToggle tog = this.toggleGroupTab.Get(togKey.ToInt());
					bool interactable = tog.interactable;
					if (interactable)
					{
						this.toggleGroupTab.Set(togKey.ToInt(), true);
						return;
					}
				}
				this.scroll.SetDataCount(0);
			}
			else
			{
				this.toggleGroupTab.Set(targetTogKey.ToInt(), isRefresh);
				int index = this._sameBuildingTemplateList.IndexOf((int)blockData.TemplateId);
				bool flag2 = index < 0;
				if (flag2)
				{
					this._curTemplateId = -1;
				}
				else
				{
					this.scroll.ScrollTo(index, 0.3f);
					this._curTemplateId = (int)blockData.TemplateId;
					List<BuildingBlockData> list = this._sameBuildingDict[(int)blockData.TemplateId];
					int targetBuildingIndex = list.FindIndex((BuildingBlockData b) => b.BlockIndex == blockData.BlockIndex);
					BuildingBlockData buildingBlockData = list[targetBuildingIndex];
					BuildingBlockKey buildingBlockKey = this.GetBuildingBlockKey(buildingBlockData);
					bool flag3 = !isRefresh;
					if (flag3)
					{
						this.UpdateBuildingManage(buildingBlockData);
					}
					bool flag4 = this.CurTogKey == BuildingListPanel.TogKey.Make && UIManager.Instance.IsFocusElement(UIElement.Make) && buildingBlockData.OperationType != 0;
					if (flag4)
					{
						ArgumentBox argsBox = UI_Make.GetMakeBuildingInfo(buildingBlockData, buildingBlockKey);
						GEvent.OnEvent(UiEvents.SwitchBuildingMake, argsBox);
					}
					else
					{
						bool flag5 = this._currentDisplayMode == BuildingListPanel.EPanelDisplayMode.CraftsmanPanel && UIManager.Instance.IsFocusElement(UIElement.Make) && buildingBlockData.OperationType != 0;
						if (flag5)
						{
							ArgumentBox argsBox2 = UI_Make.GetMakeBuildingInfo(buildingBlockData, buildingBlockKey);
							GEvent.OnEvent(UiEvents.SwitchBuildingMake, argsBox2);
						}
						else
						{
							bool flag6 = this.CurTogKey == BuildingListPanel.TogKey.Make && this._isOpenMakePage && UIManager.Instance.IsFocusElement(UIElement.BuildingManage) && buildingBlockData.OperationType != 0;
							if (flag6)
							{
								ArgumentBox argumentBox = ViewMake.GetMakeBuildingInfo(blockData, buildingBlockKey, MakeTogKey.Invalid);
								argumentBox.SetObject("Tab", MakeTogKey.Make);
								UIElement.Make.SetOnInitArgs(argumentBox);
								UIManager.Instance.ShowUI(UIElement.Make, true);
							}
							else
							{
								bool flag7 = UIManager.Instance.IsFocusElement(UIElement.Make);
								if (flag7)
								{
									UIManager.Instance.HideUI(UIElement.Make);
									bool flag8 = buildingBlockData.OperationType == 0;
									if (flag8)
									{
										this._isOpenMakePage = true;
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06009B60 RID: 39776 RVA: 0x0048C654 File Offset: 0x0048A854
		private void UpdateBuildingManage(BuildingBlockData blockData)
		{
			this.scroll.ReRender();
			short blockIndex = blockData.BlockIndex;
			bool flag = UIManager.Instance.IsElementActive(UIElement.BuildingManage);
			if (flag)
			{
				List<BuildingBlockData> neighborList = new List<BuildingBlockData>();
				List<int> neighborDistanceList = new List<int>();
				this.GetNeighborBlocks(blockIndex, ref neighborList, 1, 2, neighborDistanceList);
				ArgumentBox argsBox = EasyPool.Get<ArgumentBox>();
				argsBox.Set("BlockTemplateId", blockData.TemplateId);
				argsBox.SetObject("BuildingBlockData", blockData);
				GEvent.OnEvent(UiEvents.SwitchBuildingManage, argsBox);
			}
		}

		// Token: 0x06009B61 RID: 39777 RVA: 0x0048C6DA File Offset: 0x0048A8DA
		private void GetNeighborBlocks(short blockIndex, ref List<BuildingBlockData> neighborBlockList, sbyte blockWidth = 1, int range = 2, List<int> neighborDistanceList = null)
		{
			CommonUtils.GetNeighborBlocks(this._areaData, this._blockList, blockIndex, ref neighborBlockList, blockWidth, range, neighborDistanceList);
		}

		// Token: 0x06009B62 RID: 39778 RVA: 0x0048C6F8 File Offset: 0x0048A8F8
		private void Update()
		{
			int curIndex = this._sameBuildingTemplateList.IndexOf(this._curTemplateId);
			bool flag = TabSwitchCommandKit.PrevTabLevel1.Check(UIElement.BuildingManage, false, false, false, true, false) && curIndex > 0;
			if (flag)
			{
				this.SelectBuilding(this._sameBuildingDict[this._sameBuildingTemplateList[curIndex - 1]][0], false);
			}
			else
			{
				bool flag2 = TabSwitchCommandKit.NextTabLevel1.Check(UIElement.BuildingManage, false, false, false, true, false) && curIndex >= 0 && curIndex < this._sameBuildingTemplateList.Count - 1;
				if (flag2)
				{
					this.SelectBuilding(this._sameBuildingDict[this._sameBuildingTemplateList[curIndex + 1]][0], false);
				}
			}
		}

		// Token: 0x06009B63 RID: 39779 RVA: 0x0048C7BE File Offset: 0x0048A9BE
		private void OnEndEdit(string value)
		{
			CommonUtils.FixToShowAbleString(ref value, this.inputFieldSearch.textComponent.font);
			this.inputFieldSearch.SetTextWithoutNotify(value);
			this.RefreshScroll();
		}

		// Token: 0x06009B64 RID: 39780 RVA: 0x0048C7F0 File Offset: 0x0048A9F0
		private void OnScroll()
		{
			bool flag = this._scrollRect.Content.rect.width < this._scrollRect.Viewport.rect.width;
			if (flag)
			{
				this.btnLeftArrow.gameObject.SetActive(false);
				this.btnRightArrow.gameObject.SetActive(false);
			}
			else
			{
				bool isLeft = this._scrollRect.Content.anchoredPosition.x < 0f;
				bool isRight = this._scrollRect.Content.anchoredPosition.x > this._maxScrollContentPosX;
				this.btnLeftArrow.gameObject.SetActive(isLeft);
				this.btnRightArrow.gameObject.SetActive(isRight);
			}
		}

		// Token: 0x06009B65 RID: 39781 RVA: 0x0048C8BC File Offset: 0x0048AABC
		private void OnClickBtnArrow(bool isRight)
		{
			float orgPos = this._scrollRect.Content.anchoredPosition.x;
			float offset = (this._scrollCellWidth + this.scroll.gap.x) * (float)this.scroll.PageCount;
			float endPos = isRight ? (orgPos - offset) : (orgPos + offset);
			endPos = Mathf.Clamp(endPos, this._maxScrollContentPosX, 0f);
			this._scrollRect.ScrollTo(new Vector2(endPos, 0f), 0.3f);
		}

		// Token: 0x0400783E RID: 30782
		[SerializeField]
		private CToggleGroup toggleGroupTab;

		// Token: 0x0400783F RID: 30783
		[SerializeField]
		private InfinityScroll scroll;

		// Token: 0x04007840 RID: 30784
		[SerializeField]
		private TMP_InputField inputFieldSearch;

		// Token: 0x04007841 RID: 30785
		[SerializeField]
		private CButton btnLeftArrow;

		// Token: 0x04007842 RID: 30786
		[SerializeField]
		private CButton btnRightArrow;

		// Token: 0x04007843 RID: 30787
		private bool _hasInit;

		// Token: 0x04007844 RID: 30788
		private List<BuildingBlockData> _blockList;

		// Token: 0x04007845 RID: 30789
		private readonly Dictionary<BuildingListPanel.TogKey, List<BuildingBlockData>> _normalBuildingDict = new Dictionary<BuildingListPanel.TogKey, List<BuildingBlockData>>
		{
			{
				BuildingListPanel.TogKey.Shop,
				new List<BuildingBlockData>()
			},
			{
				BuildingListPanel.TogKey.Make,
				new List<BuildingBlockData>()
			},
			{
				BuildingListPanel.TogKey.Resource,
				new List<BuildingBlockData>()
			}
		};

		// Token: 0x04007846 RID: 30790
		private readonly Dictionary<BuildingListPanel.TogKey, List<BuildingBlockData>> _craftBuildingDict = new Dictionary<BuildingListPanel.TogKey, List<BuildingBlockData>>
		{
			{
				BuildingListPanel.TogKey.Shop,
				new List<BuildingBlockData>()
			},
			{
				BuildingListPanel.TogKey.Make,
				new List<BuildingBlockData>()
			},
			{
				BuildingListPanel.TogKey.Resource,
				new List<BuildingBlockData>()
			}
		};

		// Token: 0x04007847 RID: 30791
		private int _curTemplateId = -1;

		// Token: 0x04007848 RID: 30792
		private readonly List<BuildingBlockData> _tempBuildingList = new List<BuildingBlockData>();

		// Token: 0x04007849 RID: 30793
		private readonly Dictionary<int, List<BuildingBlockData>> _sameBuildingDict = new Dictionary<int, List<BuildingBlockData>>();

		// Token: 0x0400784A RID: 30794
		private readonly List<int> _sameBuildingTemplateList = new List<int>();

		// Token: 0x0400784B RID: 30795
		private BuildingListPanel.EPanelDisplayMode _currentDisplayMode;

		// Token: 0x0400784C RID: 30796
		private bool _isOpenMakePage;

		// Token: 0x0400784D RID: 30797
		private Location _location;

		// Token: 0x0400784E RID: 30798
		private BuildingAreaData _areaData;

		// Token: 0x0400784F RID: 30799
		private BuildingBlockData _curBlockData;

		// Token: 0x04007850 RID: 30800
		private MapBlockData _mapBlockData;

		// Token: 0x04007851 RID: 30801
		private bool _isTaiwuVillage;

		// Token: 0x04007852 RID: 30802
		private CScrollRect _scrollRect;

		// Token: 0x04007853 RID: 30803
		private float _maxScrollContentPosX;

		// Token: 0x04007854 RID: 30804
		private float _scrollCellWidth;

		// Token: 0x04007855 RID: 30805
		private BuildingExceptionData _buildingExceptionData;

		// Token: 0x04007856 RID: 30806
		private IAsyncMethodRequestHandler _requestHandler;

		// Token: 0x020022FC RID: 8956
		public enum TogKey
		{
			// Token: 0x0400DD55 RID: 56661
			Invalid = -1,
			// Token: 0x0400DD56 RID: 56662
			Shop,
			// Token: 0x0400DD57 RID: 56663
			Make,
			// Token: 0x0400DD58 RID: 56664
			Resource
		}

		// Token: 0x020022FD RID: 8957
		private enum EPanelDisplayMode
		{
			// Token: 0x0400DD5A RID: 56666
			None,
			// Token: 0x0400DD5B RID: 56667
			Manage,
			// Token: 0x0400DD5C RID: 56668
			CraftsmanPanel
		}
	}
}
