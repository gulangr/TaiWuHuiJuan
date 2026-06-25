using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using FrameWork;
using GameData.Domains.Character;
using GameData.Domains.Item.Display;
using GameData.Domains.Taiwu;
using GameData.GameDataBridge;
using GameData.Serializer;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020002FB RID: 763
public class UI_WugKingMultiplySelect : UIBase
{
	// Token: 0x170004DC RID: 1244
	// (get) Token: 0x06002CB2 RID: 11442 RVA: 0x001602AD File Offset: 0x0015E4AD
	private int TaiwuCharId
	{
		get
		{
			return SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
		}
	}

	// Token: 0x06002CB3 RID: 11443 RVA: 0x001602BC File Offset: 0x0015E4BC
	public override void OnInit(ArgumentBox argsBox)
	{
		List<ItemDisplayData> selectedItems;
		argsBox.Get<List<ItemDisplayData>>("SelectedItems", out selectedItems);
		argsBox.Get<Action<List<ItemDisplayData>>>("OnConfirm", out this._onConfirm);
		argsBox.Get("TargetCount", out this._targetCount);
		this._multiplyItemScrollView = base.GetComponent<MultiplyItemScrollView>();
		Dictionary<ItemSourceType, List<ItemDisplayData>> itemDict = new Dictionary<ItemSourceType, List<ItemDisplayData>>
		{
			{
				ItemSourceType.Inventory,
				this._inventoryItems
			}
		};
		this._multiplyItemScrollView.Init(this.TaiwuCharId, itemDict, null, null, new Action<List<ValueTuple<ItemDisplayData, int>>>(this.OnContentChange), true);
		bool flag = selectedItems != null && selectedItems.Count > 0;
		if (flag)
		{
			foreach (ItemDisplayData item in selectedItems)
			{
				this._multiplyItemScrollView.SelectedMultiplyItemDict.Add(item, item.Amount);
			}
		}
		this.NeedDataListenerId = true;
		this._inventoryItems.Clear();
		UIElement element = this.Element;
		element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(this.OnListenerIdReady));
		UIElement element2 = this.Element;
		element2.OnShowed = (Action)Delegate.Combine(element2.OnShowed, new Action(delegate()
		{
			ConchShipCursor.Instance.SetSelectCountActive(true);
			ConchShipCursor.Instance.SetSelectCountMax(this._targetCount, null);
		}));
		this.InitRefers();
		this.OnContentChange(null);
	}

	// Token: 0x06002CB4 RID: 11444 RVA: 0x0016041C File Offset: 0x0015E61C
	private void OnListenerIdReady()
	{
		CharacterDomainMethod.Call.GetAllInventoryItems(this.Element.GameDataListenerId, this.TaiwuCharId);
	}

	// Token: 0x06002CB5 RID: 11445 RVA: 0x00160438 File Offset: 0x0015E638
	private void Awake()
	{
		this.InitRefers();
		this._itemScrollView.Init();
		this._itemScrollView.SetItemList(ref this._inventoryItems, true, "UI_CombatSkillSpecialBreakMultiplySelect", this._itemScrollView.SortAndFilter.IsDetailView, new Action<ItemDisplayData, ItemView>(this.OnRenderItem));
		this._itemScrollView.InfinityScroll.AddOnScrollEvent(new Action(this.OnScroll));
		this._itemScrollView.SetCharId(this.TaiwuCharId);
	}

	// Token: 0x06002CB6 RID: 11446 RVA: 0x001604BC File Offset: 0x0015E6BC
	private void OnScroll()
	{
		this._multiplyItemScrollView.HideGradeLimitTip();
	}

	// Token: 0x06002CB7 RID: 11447 RVA: 0x001604CC File Offset: 0x0015E6CC
	public override void OnNotifyGameData(List<NotificationWrapper> notifications)
	{
		foreach (NotificationWrapper wrapper in notifications)
		{
			Notification notification = wrapper.Notification;
			byte type = notification.Type;
			byte b = type;
			if (b == 1)
			{
				bool flag = notification.DomainId == 4;
				if (flag)
				{
					bool flag2 = notification.MethodId == 27;
					if (flag2)
					{
						this._inventoryItems.Clear();
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._inventoryItems);
						this._multiplyItemScrollView.EnterKongsangSpecialInteractMultiplyMode();
						this.Element.ShowAfterRefresh();
					}
				}
			}
		}
	}

	// Token: 0x06002CB8 RID: 11448 RVA: 0x00160598 File Offset: 0x0015E798
	private void OnDisable()
	{
		this._multiplyItemScrollView.ExitMultiplyMode();
		this.OnContentChange(null);
		ConchShipCursor.Instance.SetSelectCountActive(false);
	}

	// Token: 0x06002CB9 RID: 11449 RVA: 0x001605BC File Offset: 0x0015E7BC
	protected override void OnClick(Transform btn)
	{
		string name = btn.name;
		string a = name;
		if (!(a == "ConfirmButton"))
		{
			if (a == "CancelButton")
			{
				this.QuickHide();
			}
		}
		else
		{
			this._selectedItems.ForEach(delegate(ItemDisplayData x)
			{
				x.Amount = this._multiplyItemScrollView.SelectedMultiplyItemDict[x];
			});
			this._onConfirm(this._selectedItems);
			this.QuickHide();
		}
	}

	// Token: 0x06002CBA RID: 11450 RVA: 0x0016062C File Offset: 0x0015E82C
	private void UpdateCursorSelectCountDisplay()
	{
		int count = (from v in this._selectedItems
		select this._multiplyItemScrollView.SelectedMultiplyItemDict[v]).Sum();
		ConchShipCursor.Instance.SetSelectCountCur(count, (count >= this._targetCount) ? "brightblue" : "brightred");
	}

	// Token: 0x06002CBB RID: 11451 RVA: 0x00160678 File Offset: 0x0015E878
	private void OnRenderItem(ItemDisplayData itemData, ItemView itemView)
	{
		this._multiplyItemScrollView.OnRenderItemMultiply(itemData, itemView);
	}

	// Token: 0x06002CBC RID: 11452 RVA: 0x0016068C File Offset: 0x0015E88C
	private void OnContentChange([TupleElementNames(new string[]
	{
		"data",
		"count"
	})] List<ValueTuple<ItemDisplayData, int>> changeList = null)
	{
		bool flag = this._isUpdatingSelectByCode || changeList == null || changeList.Count == 0;
		if (flag)
		{
			this.OnContentChangeInner();
		}
		else
		{
			ValueTuple<ItemDisplayData, int> valueTuple = changeList[0];
			ItemDisplayData data = valueTuple.Item1;
			int count = valueTuple.Item2;
			int selectedCount;
			this._multiplyItemScrollView.SelectedMultiplyItemDict.TryGetValue(data, out selectedCount);
			bool flag2 = selectedCount == 0;
			if (flag2)
			{
				this.OnContentChangeInner();
			}
			else
			{
				bool needDialog = data.UsingType != ItemDisplayData.ItemUsingType.Invalid;
				bool flag3 = !needDialog;
				if (flag3)
				{
					this.OnContentChangeInner();
				}
				else
				{
					string confirmTip = data.GetUsingOperationConfirmTip(ItemDisplayData.ItemUsingOperationType.Give);
					this.ShowDialogAndSelectItem(confirmTip, data);
				}
			}
		}
	}

	// Token: 0x06002CBD RID: 11453 RVA: 0x00160734 File Offset: 0x0015E934
	private void ShowDialogAndSelectItem(string content, ItemDisplayData itemData)
	{
		DialogCmd dialogCmd = new DialogCmd
		{
			Type = 1,
			Title = LocalStringManager.Get(LanguageKey.LK_Common_Attention),
			Content = content,
			Yes = delegate()
			{
				this.OnContentChangeInner();
			},
			No = delegate()
			{
				this.UnSelectItem(itemData);
			}
		};
		UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", dialogCmd));
		UIManager.Instance.MaskUI(UIElement.Dialog);
	}

	// Token: 0x06002CBE RID: 11454 RVA: 0x001607CC File Offset: 0x0015E9CC
	private void OnContentChangeInner()
	{
		this._selectedItems.Clear();
		foreach (KeyValuePair<ItemDisplayData, int> pair in this._multiplyItemScrollView.SelectedMultiplyItemDict)
		{
			ItemDisplayData itemData = pair.Key;
			int selectedCount = pair.Value;
			Inventory itemKeys = itemData.GetOperationInventoryFromPool(selectedCount, true);
			ItemDisplayData.ReturnInventoryToPool(itemKeys);
			this._selectedItems.Add(itemData);
		}
		this.RefreshSelectedItems();
		this.RefreshConfirmButton();
		bool flag = this._selectedItems.Count == 0;
		if (flag)
		{
			this._multiplyItemScrollView.HideGradeLimitTip();
		}
		this._selectedItemScrollView.SetActive(this._selectedItems.Count > 0);
		this.UpdateCursorSelectCountDisplay();
	}

	// Token: 0x06002CBF RID: 11455 RVA: 0x001608B0 File Offset: 0x0015EAB0
	private void RefreshConfirmButton()
	{
		TooltipInvoker tipDisplayer = this._confirmButton.transform.Find("ClickRect").GetComponent<TooltipInvoker>();
		bool isEmpty = this._selectedItems.Count == 0;
		this._confirmButton.interactable = !isEmpty;
		tipDisplayer.enabled = isEmpty;
		bool flag = isEmpty;
		if (flag)
		{
			TooltipInvoker tooltipInvoker = tipDisplayer;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
			}
			tipDisplayer.RuntimeParam.Set("arg0", LocalStringManager.Get(LanguageKey.LK_CombatSkill_SpecialBreak_ItemSelect_Tip2));
		}
	}

	// Token: 0x06002CC0 RID: 11456 RVA: 0x0016093C File Offset: 0x0015EB3C
	private void RefreshSelectedItems()
	{
		for (int i = 0; i < this._selectedItems.Count; i++)
		{
			ItemDisplayData itemData = this._selectedItems[i];
			bool flag = i > this._selectedItemLayout.transform.childCount - 1;
			ItemView item;
			if (flag)
			{
				item = Object.Instantiate<ItemView>(this._selectedItemView, this._selectedItemLayout.transform);
			}
			else
			{
				item = this._selectedItemLayout.transform.GetChild(i).GetComponent<ItemView>();
			}
			item.gameObject.SetActive(true);
			int selectedCount;
			this._multiplyItemScrollView.SelectedMultiplyItemDict.TryGetValue(itemData, out selectedCount);
			item.SetData(itemData, false, selectedCount, false, true, null, false, true);
			item.SetClickEvent(delegate
			{
				this.UnSelectItem(itemData);
			});
			int index = this._multiplyItemScrollView.GetSelectedOrder(itemData);
			item.SetSelectedOrder(true, index);
			item.CGet<GameObject>("Order").SetActive(false);
			item.SetAmountPos(true);
		}
		for (int j = this._selectedItems.Count; j < this._selectedItemLayout.transform.childCount; j++)
		{
			this._selectedItemLayout.transform.GetChild(j).gameObject.SetActive(false);
		}
		this.CalculateSelectedItemLayoutHeight();
	}

	// Token: 0x06002CC1 RID: 11457 RVA: 0x00160AB8 File Offset: 0x0015ECB8
	private void UnSelectItem(ItemDisplayData itemData)
	{
		this._isUpdatingSelectByCode = true;
		List<ValueTuple<ItemDisplayData, int>> selectList = new List<ValueTuple<ItemDisplayData, int>>();
		for (int i = 0; i < this._multiplyItemScrollView.SelectedMultiplyItemOrderedList.Count; i++)
		{
			ItemDisplayData itemDataInList = this._multiplyItemScrollView.SelectedMultiplyItemOrderedList[i];
			bool flag = itemDataInList != itemData;
			if (flag)
			{
				int count = this._multiplyItemScrollView.SelectedMultiplyItemDict[itemDataInList];
				selectList.Add(new ValueTuple<ItemDisplayData, int>(itemDataInList, count));
			}
		}
		this._multiplyItemScrollView.SelectItem(selectList);
		this._isUpdatingSelectByCode = false;
	}

	// Token: 0x06002CC2 RID: 11458 RVA: 0x00160B4C File Offset: 0x0015ED4C
	private void CalculateSelectedItemLayoutHeight()
	{
		float ySpace = this._selectedItemLayout.spacing.y;
		float itemHeight = this._selectedItemView.GetComponent<RectTransform>().rect.height;
		int countPerLine = this._selectedItemLayout.constraintCount;
		int visibleChildCount = 0;
		for (int i = 0; i < this._selectedItemLayout.transform.childCount; i++)
		{
			bool activeSelf = this._selectedItemLayout.transform.GetChild(i).gameObject.activeSelf;
			if (activeSelf)
			{
				visibleChildCount++;
			}
		}
		int rowCount = Mathf.CeilToInt((float)visibleChildCount / (float)countPerLine);
		float height = (float)rowCount * itemHeight + (float)(rowCount - 1) * ySpace;
		this._selectedItemLayout.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
	}

	// Token: 0x06002CC3 RID: 11459 RVA: 0x00160C14 File Offset: 0x0015EE14
	private void InitRefers()
	{
		this._itemScrollView = base.CGet<ItemScrollView>("ItemScrollView");
		this._selectedItemLayout = base.CGet<GridLayoutGroup>("SelectedItemLayout");
		this._selectedItemView = base.CGet<ItemView>("SelectedItemView");
		this._confirmButton = base.CGet<CButtonObsolete>("ConfirmButton");
		this._bottomTips = base.CGet<TextMeshProUGUI>("BottomTips");
		this._selectedItemScrollView = base.CGet<GameObject>("SelectedItemScrollView");
	}

	// Token: 0x04002065 RID: 8293
	private List<ItemDisplayData> _inventoryItems = new List<ItemDisplayData>();

	// Token: 0x04002066 RID: 8294
	private readonly List<ItemDisplayData> _selectedItems = new List<ItemDisplayData>();

	// Token: 0x04002067 RID: 8295
	private MultiplyItemScrollView _multiplyItemScrollView;

	// Token: 0x04002068 RID: 8296
	private Action<List<ItemDisplayData>> _onConfirm;

	// Token: 0x04002069 RID: 8297
	private int _targetCount = 0;

	// Token: 0x0400206A RID: 8298
	private bool _isUpdatingSelectByCode = false;

	// Token: 0x0400206B RID: 8299
	private ItemScrollView _itemScrollView;

	// Token: 0x0400206C RID: 8300
	private GridLayoutGroup _selectedItemLayout;

	// Token: 0x0400206D RID: 8301
	private ItemView _selectedItemView;

	// Token: 0x0400206E RID: 8302
	private CButtonObsolete _confirmButton;

	// Token: 0x0400206F RID: 8303
	private TextMeshProUGUI _bottomTips;

	// Token: 0x04002070 RID: 8304
	private GameObject _selectedItemScrollView;
}
