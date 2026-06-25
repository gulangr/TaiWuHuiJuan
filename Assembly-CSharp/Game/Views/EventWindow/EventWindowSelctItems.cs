using System;
using System.Collections.Generic;
using System.Linq;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.ListStyleGeneralScroll.Item;
using Game.Components.SortAndFilter.Item.Apply;
using Game.Views.Select;
using GameData.Domains.Character;
using GameData.Domains.Extra;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Taiwu;
using GameData.Domains.TaiwuEvent.DisplayEvent;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.EventWindow
{
	// Token: 0x02000A41 RID: 2625
	public class EventWindowSelctItems : MonoBehaviour
	{
		// Token: 0x17000E31 RID: 3633
		// (get) Token: 0x0600818B RID: 33163 RVA: 0x003C584B File Offset: 0x003C3A4B
		public IReadOnlyDictionary<ITradeableContent, int> DisplayDataDic
		{
			get
			{
				return this._displayDataDic;
			}
		}

		// Token: 0x17000E32 RID: 3634
		// (get) Token: 0x0600818C RID: 33164 RVA: 0x003C5853 File Offset: 0x003C3A53
		private int _currentSelectedAmount
		{
			get
			{
				return this._displayDataDic.Values.Sum();
			}
		}

		// Token: 0x17000E33 RID: 3635
		// (get) Token: 0x0600818D RID: 33165 RVA: 0x003C5865 File Offset: 0x003C3A65
		private int _currentSelectedRowAmount
		{
			get
			{
				return this._displayDataDic.Count<KeyValuePair<ITradeableContent, int>>();
			}
		}

		// Token: 0x0600818E RID: 33166 RVA: 0x003C5874 File Offset: 0x003C3A74
		public Dictionary<ItemKey, int> GetSelectedItemDict()
		{
			Dictionary<ItemKey, int> result = new Dictionary<ItemKey, int>();
			foreach (KeyValuePair<ITradeableContent, int> pair in this._displayDataDic)
			{
				int selectedAmount = pair.Value;
				bool flag = selectedAmount <= 0;
				if (!flag)
				{
					ItemDisplayData itemDisplayData = pair.Key as ItemDisplayData;
					bool flag2 = itemDisplayData == null;
					if (flag2)
					{
						result[pair.Key.Key] = selectedAmount;
					}
					else
					{
						bool isResource = itemDisplayData.IsResource;
						if (isResource)
						{
							int count;
							bool flag3 = result.TryGetValue(itemDisplayData.Key, out count);
							if (flag3)
							{
								result[itemDisplayData.Key] = count + selectedAmount;
							}
							else
							{
								result[itemDisplayData.Key] = selectedAmount;
							}
						}
						else
						{
							int operationAmount = Mathf.Min(selectedAmount, itemDisplayData.Amount);
							bool flag4 = operationAmount <= 0;
							if (!flag4)
							{
								Inventory inventory = itemDisplayData.GetOperationInventoryFromPool(operationAmount, true);
								foreach (KeyValuePair<ItemKey, int> keyValuePair in inventory.Items)
								{
									ItemKey itemKey;
									int num;
									keyValuePair.Deconstruct(out itemKey, out num);
									ItemKey item = itemKey;
									int curCount = num;
									int count2;
									bool flag5 = result.TryGetValue(item, out count2);
									if (flag5)
									{
										result[item] = count2 + curCount;
									}
									else
									{
										result[item] = curCount;
									}
								}
								ItemDisplayData.ReturnInventoryToPool(inventory);
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x0600818F RID: 33167 RVA: 0x003C5A4C File Offset: 0x003C3C4C
		private void Awake()
		{
			this.Init();
		}

		// Token: 0x06008190 RID: 33168 RVA: 0x003C5A58 File Offset: 0x003C3C58
		private void OpenSelectItemsWindow()
		{
			bool flag = this._maxAmount > 1;
			SelectItemConfig config;
			if (flag)
			{
				config = SelectItemConfig.CreateMultipleSelectConfig(new SelectItemRules
				{
					OnlyFromInventory = true
				}, delegate(List<SelectedItemData> itemSelected)
				{
					this.OnSelectItems(itemSelected);
				}, "", 0, -1, null);
			}
			else
			{
				config = SelectItemConfig.CreateSingleSelectConfig(new SelectItemRules
				{
					OnlyFromInventory = true
				}, delegate(List<SelectedItemData> itemSelected)
				{
					this.OnSelectItems(itemSelected);
				}, "", null);
			}
			config.ShowProfessionPreview = this._showProfessionPreview;
			config.OperationMode = ESelectItemOperationMode.NoPreSelect;
			config.SelectItemMode = (this._selectItemData.SingleRowMode ? ESelectItemMode.RowSelect : ESelectItemMode.ItemSelect);
			config.MaxSelectCount = ((this._maxAmount >= 99) ? 0 : this._maxAmount);
			config.MinSelectCount = this._minAmount;
			config.ExternalItems = this._candidates;
			config.SortKey = "EventWindowSelectItem";
			config.CustomTextSetter = new Action<IReadOnlyList<SelectedItemData>, TextMeshProUGUI, GameObject>(this.OnSelectionChange);
			config.InitialSelectedItems = new List<SelectedItemData>();
			config.OperationMode = (this._selectItemData.SlotMode ? ESelectItemOperationMode.Slot : ESelectItemOperationMode.NoPreSelect);
			config.HideSourceToggles = this._selectItemData.HideSourceToggle;
			config.CheckSameByReferenceOnly = this._selectItemData.CheckSameByReferenceOnly;
			config.ResourceMaxValue = this._selectItemData.ResourceMaxValue;
			bool flag2 = config.ResourceMaxValue > 0;
			if (flag2)
			{
				config.SelectItemMode = ESelectItemMode.RowSelect;
			}
			bool flag3 = this._selectItemData.ItemTitleKey >= 0;
			if (flag3)
			{
				LanguageKey titleKey = (LanguageKey)this._selectItemData.ItemTitleKey;
				config.Title = titleKey.Tr();
			}
			bool flag4 = this._selectItemData.ItemSelectedTitleKey >= 0;
			if (flag4)
			{
				LanguageKey titleKey2 = (LanguageKey)this._selectItemData.ItemSelectedTitleKey;
				config.SelectedTitle = titleKey2.Tr();
			}
			bool flag5 = this._selectItemData.ItemSelectedToggleKey >= 0;
			if (flag5)
			{
				LanguageKey titleKey3 = (LanguageKey)this._selectItemData.ItemSelectedToggleKey;
				config.SelectedToggleKey = titleKey3;
			}
			foreach (KeyValuePair<ITradeableContent, int> item in this._displayDataDic)
			{
				config.InitialSelectedItems.Add(new SelectedItemData(item.Key, item.Value));
			}
			config.CanSelectLockedItem = (this._selectItemData.ItemOperationType == ItemOperationType.EItemOperationType.UpgradeAnimal.ToSbyte());
			config.VisibleMainFilterToggles = this.GetVisibleMainFilterToggles();
			bool flag6 = this._selectItemData.ItemOperationType == ItemOperationType.EItemOperationType.HostileInteraction.ToSbyte();
			if (flag6)
			{
				config.CustomTextTips = LanguageKey.LK_Select_Item_SumAlertFactor_Tips.Tr();
			}
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.Set("DisplayBg", true);
			argBox.SetObject("SelectItemConfig", config);
			UIElement.SelectItem.SetOnInitArgs(argBox);
			UIManager.Instance.ShowUI(UIElement.SelectItem, true);
		}

		// Token: 0x06008191 RID: 33169 RVA: 0x003C5D5C File Offset: 0x003C3F5C
		private List<int> GetVisibleMainFilterToggles()
		{
			EventSelectItemData selectItemData = this._selectItemData;
			List<int> source = (selectItemData != null) ? selectItemData.VisibleItemFilterTypes : null;
			bool flag = source == null || source.Count == 0;
			List<int> result;
			if (flag)
			{
				result = null;
			}
			else
			{
				result = (from index in source
				where index >= 0 && index < 7
				select index).Distinct<int>().ToList<int>();
			}
			return result;
		}

		// Token: 0x06008192 RID: 33170 RVA: 0x003C5DC8 File Offset: 0x003C3FC8
		private void OnSelectionChange(IReadOnlyList<SelectedItemData> list, TextMeshProUGUI targetLabel, GameObject targetLabelArea)
		{
			targetLabelArea.SetActive(false);
			ItemKey itemKey = (list == null || list.Count == 0) ? ItemKey.Invalid : list[0].ItemData.Key;
			bool flag = this._selectItemData.ItemOperationType == ItemOperationType.EItemOperationType.FixItem.ToSbyte();
			if (flag)
			{
				bool flag2 = itemKey.IsValid();
				if (flag2)
				{
					ItemDomainMethod.AsyncCall.GetRepairItemNeedResourceCount(null, itemKey, delegate(int offset, RawDataPool dataPool)
					{
						int cost2 = 0;
						Serializer.Deserialize(dataPool, offset, ref cost2);
						int ownAmount2 = SingletonObject.getInstance<BuildingModel>().GetResourceCount(ItemSourceType.Resources, 6);
						string ownStr2 = this.GetOwnedString(ownAmount2, cost2);
						targetLabel.SetText(LocalStringManager.GetFormat(LanguageKey.LK_Select_Item_MoneyCost, ownStr2, CommonUtils.GetDisplayStringForNum(cost2, 100000)), true);
						targetLabelArea.SetActive(true);
					});
				}
			}
			else
			{
				bool flag3 = this._selectItemData.ItemOperationType == ItemOperationType.EItemOperationType.ExchangeTools.ToSbyte();
				if (flag3)
				{
					bool flag4 = itemKey.IsValid();
					if (flag4)
					{
						sbyte grade = ItemTemplateHelper.GetGrade(itemKey.ItemType, itemKey.TemplateId);
						int cost = GameData.Domains.Extra.SharedMethods.GetExchangeToolSpiritualDebtCost(grade);
						int ownAmount = SingletonObject.getInstance<WorldMapModel>().GetCurrAreaSpiritualDebt();
						string ownStr = this.GetOwnedString(ownAmount, cost);
						targetLabel.SetText(LocalStringManager.GetFormat(LanguageKey.LK_Select_Item_DebtCost, ownStr, CommonUtils.GetDisplayStringForNum(cost, 10000)), true);
						targetLabelArea.SetActive(true);
					}
				}
				else
				{
					bool flag5 = this._selectItemData.ItemOperationType == ItemOperationType.EItemOperationType.HostileInteraction.ToSbyte();
					if (flag5)
					{
						int alertFactor = 0;
						foreach (SelectedItemData item in list)
						{
							alertFactor += item.ItemData.AlertFactor;
						}
						targetLabel.SetText(LocalStringManager.GetFormat(LanguageKey.LK_Select_Item_SumAlertFactor, alertFactor), true);
						targetLabelArea.SetActive(true);
					}
				}
			}
		}

		// Token: 0x06008193 RID: 33171 RVA: 0x003C5FA4 File Offset: 0x003C41A4
		private string GetOwnedString(int ownAmount, int cost)
		{
			string resultStr = CommonUtils.GetDisplayStringForNum(cost, 10000);
			bool flag = ownAmount >= cost;
			if (flag)
			{
				resultStr = ownAmount.ToString().SetColor("brightblue");
			}
			else
			{
				resultStr = ownAmount.ToString().SetColor("brightred");
			}
			return resultStr;
		}

		// Token: 0x06008194 RID: 33172 RVA: 0x003C5FF8 File Offset: 0x003C41F8
		private void OnSelectItems(List<SelectedItemData> selectedItems)
		{
			this._displayDataDic.Clear();
			this.SelectedKeyList.Clear();
			foreach (SelectedItemData item in selectedItems)
			{
				this._displayDataDic[item.ItemData] = item.SelectedAmount;
				this.SelectedKeyList.Add(new ItemKey
				{
					ItemType = item.ItemData.Key.ItemType,
					TemplateId = item.ItemData.Key.TemplateId,
					Id = item.ItemData.Key.Id,
					ModificationState = item.ItemData.Key.ModificationState
				});
			}
			bool flag = this.SelectedKeyList.Count > 0;
			if (flag)
			{
				this.OpenInfoPage();
				this.RefreshDisplay();
			}
			GEvent.OnEvent(UiEvents.OnEventWindowSelectItems, null);
		}

		// Token: 0x06008195 RID: 33173 RVA: 0x003C6120 File Offset: 0x003C4320
		private void OpenInfoPage()
		{
			this.selectPage.gameObject.SetActive(false);
			this.itemListPage.gameObject.SetActive(true);
			this.RefreshButtons();
		}

		// Token: 0x06008196 RID: 33174 RVA: 0x003C6150 File Offset: 0x003C4350
		private void RefreshDisplay()
		{
			List<ITradeableContent> displayData = new List<ITradeableContent>();
			foreach (KeyValuePair<ITradeableContent, int> item in this._displayDataDic)
			{
				ITradeableContent tempItem = item.Key.Clone(-1);
				tempItem.Amount = item.Value;
				displayData.Add(tempItem);
			}
			bool flag = displayData.Count == 0 || this._maxAmount > this._currentSelectedAmount;
			if (flag)
			{
				this.addSelectItem.gameObject.SetActive(true);
			}
			else
			{
				this.addSelectItem.gameObject.SetActive(false);
			}
			this.itemList.SetItemList(displayData);
		}

		// Token: 0x06008197 RID: 33175 RVA: 0x003C6224 File Offset: 0x003C4424
		private void Init()
		{
			bool inited = this._inited;
			if (!inited)
			{
				ItemListScroll.EColumnType columnType = ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Amount | ItemListScroll.EColumnType.Type | ItemListScroll.EColumnType.Weight | ItemListScroll.EColumnType.CustomButton;
				this.itemList.Init("EventWindowSelectItems", ESortAndFilterControllerType.Item, true, new Action<ITradeableContent, RowItemLine>(this.OnRenderItem), new Action<ITradeableContent, RowItemLine>(this.OnClickItem), columnType, null, null, null);
				this.btnSelectItem.ClearAndAddListener(new Action(this.OpenSelectItemsWindow));
				this.btnDeselectAll.ClearAndAddListener(new Action(this.OpenSelectPage));
				this.btnAddSelectItem.ClearAndAddListener(new Action(this.OpenSelectItemsWindow));
				this._inited = true;
			}
		}

		// Token: 0x06008198 RID: 33176 RVA: 0x003C62C8 File Offset: 0x003C44C8
		private void OpenSelectPage()
		{
			this._displayDataDic.Clear();
			this.SelectedKeyList.Clear();
			this.selectPage.gameObject.SetActive(true);
			this.itemListPage.gameObject.SetActive(false);
			this.RefreshButtons();
			GEvent.OnEvent(UiEvents.OnEventWindowSelectItems, null);
		}

		// Token: 0x06008199 RID: 33177 RVA: 0x003C632C File Offset: 0x003C452C
		private void RefreshButtons()
		{
			this.btnDeselectAll.gameObject.SetActive(this._displayDataDic.Count > 0);
			bool hideMaxAmount = this._selectItemData != null && this._selectItemData.FilterWithOrOperate;
			bool flag = this.SelectedKeyList != null;
			if (flag)
			{
				bool flag2 = this._maxAmount <= 0 || this._maxAmount >= 99 || hideMaxAmount;
				if (flag2)
				{
					this.txtValue.text = string.Format("{0}", this._currentSelectedAmount);
				}
				else
				{
					bool flag3 = this._maxAmount > 0 && this._selectItemData != null && this._selectItemData.SingleRowMode;
					if (flag3)
					{
						this.txtValue.text = string.Format("{0}/{1}", this._currentSelectedRowAmount, this._maxAmount);
					}
					else
					{
						bool flag4 = this._minAmount > 0;
						if (flag4)
						{
							this.txtValue.text = string.Format("{0}/{1}", this._currentSelectedAmount, this._minAmount);
						}
						else
						{
							this.txtValue.text = string.Format("{0}/{1}", this._currentSelectedAmount, this._maxAmount);
						}
					}
				}
			}
			else
			{
				this.txtValue.text = string.Empty;
			}
			bool flag5 = this._confirmButton == null;
			if (!flag5)
			{
				bool reachMinAmount = this._selectItemData != null && this._selectItemData.MinSelectAmount > 0 && this.SelectedKeyList.Count >= this._selectItemData.MinSelectAmount;
				this._confirmButton.interactable = (this.SelectedKeyList != null && this.SelectedKeyList.Count > 0 && this._selectItemData != null && (this._selectItemData.IsAvailableSelectResult(this.SelectedKeyList) || this._maxAmount == this._currentSelectedAmount || reachMinAmount));
				Action refreshConfirmButtonTips = this._refreshConfirmButtonTips;
				if (refreshConfirmButtonTips != null)
				{
					refreshConfirmButtonTips();
				}
			}
		}

		// Token: 0x0600819A RID: 33178 RVA: 0x003C6554 File Offset: 0x003C4754
		public void Refresh(EventSelectItemData selectItemData, bool showProfessionPreview, CButton confirmButton, Action refreshConfirmButtonTips, bool autoSelect)
		{
			this._showProfessionPreview = showProfessionPreview;
			this._selectItemData = selectItemData;
			this._confirmButton = confirmButton;
			this._refreshConfirmButtonTips = refreshConfirmButtonTips;
			bool flag = selectItemData.CanSelectItemList == null;
			if (flag)
			{
				selectItemData.CanSelectItemList = new List<ITradeableContent>();
			}
			this._isMultiItemSelect = (selectItemData.FilterList.Count > 1);
			this._candidates = selectItemData.CanSelectItemList;
			this._maxAmount = selectItemData.FilterList.Count;
			this._minAmount = selectItemData.MinSelectAmount;
			this.OpenSelectPage();
			if (autoSelect)
			{
				this.OpenSelectItemsWindow();
			}
		}

		// Token: 0x0600819B RID: 33179 RVA: 0x003C65EA File Offset: 0x003C47EA
		public void OnClickItem(ITradeableContent itemData, RowItemLine rowItemLine)
		{
		}

		// Token: 0x0600819C RID: 33180 RVA: 0x003C65F0 File Offset: 0x003C47F0
		private void RemoveSingle(ITradeableContent itemData)
		{
			foreach (KeyValuePair<ITradeableContent, int> item in this._displayDataDic)
			{
				bool flag = item.Key.Key.TemplateEquals(itemData.Key);
				if (flag)
				{
					this._displayDataDic.Remove(item.Key);
					break;
				}
			}
			this.SelectedKeyList.Remove(itemData.Key);
		}

		// Token: 0x0600819D RID: 33181 RVA: 0x003C6688 File Offset: 0x003C4888
		public void OnRenderItem(ITradeableContent itemData, RowItemLine rowItemLine)
		{
			RowItemMain rowItemMain = rowItemLine.GetComponentInChildren<RowItemMain>();
			rowItemLine.Set(rowItemMain, false);
			Refers lastCellRefers = rowItemLine.ContainerRoot.GetChild(rowItemLine.ContainerRoot.childCount - 1).GetComponent<Refers>();
			rowItemLine.SetInteractable(false, true);
			CButton removeButton = lastCellRefers.CGet<CButton>("BtnRemove");
			removeButton.ClearAndAddListener(delegate
			{
				this.RemoveSingle(itemData);
				bool flag = this._displayDataDic.Count <= 0;
				if (flag)
				{
					this.OpenSelectPage();
				}
				else
				{
					this.RefreshDisplay();
				}
				this.RefreshButtons();
			});
		}

		// Token: 0x04006309 RID: 25353
		[Header("pages")]
		[SerializeField]
		private GameObject selectPage;

		// Token: 0x0400630A RID: 25354
		[SerializeField]
		private GameObject itemListPage;

		// Token: 0x0400630B RID: 25355
		[SerializeField]
		private TextMeshProUGUI txtValue;

		// Token: 0x0400630C RID: 25356
		[Header("selectpage")]
		[SerializeField]
		private CButton btnSelectItem;

		// Token: 0x0400630D RID: 25357
		[Header("infomations")]
		[SerializeField]
		private ItemListScroll itemList;

		// Token: 0x0400630E RID: 25358
		[SerializeField]
		private CButton btnDeselectAll;

		// Token: 0x0400630F RID: 25359
		[SerializeField]
		private Refers addSelectItem;

		// Token: 0x04006310 RID: 25360
		[SerializeField]
		private CButton btnAddSelectItem;

		// Token: 0x04006311 RID: 25361
		private CButton _confirmButton;

		// Token: 0x04006312 RID: 25362
		private Action _refreshConfirmButtonTips;

		// Token: 0x04006313 RID: 25363
		private bool _inited = false;

		// Token: 0x04006314 RID: 25364
		private Dictionary<ITradeableContent, int> _displayDataDic = new Dictionary<ITradeableContent, int>();

		// Token: 0x04006315 RID: 25365
		public readonly List<ItemKey> SelectedKeyList = new List<ItemKey>();

		// Token: 0x04006316 RID: 25366
		private EventSelectItemData _selectItemData;

		// Token: 0x04006317 RID: 25367
		private bool _showProfessionPreview = false;

		// Token: 0x04006318 RID: 25368
		private bool _isMultiItemSelect;

		// Token: 0x04006319 RID: 25369
		private List<ITradeableContent> _candidates;

		// Token: 0x0400631A RID: 25370
		private int _maxAmount;

		// Token: 0x0400631B RID: 25371
		private int _minAmount;
	}
}
