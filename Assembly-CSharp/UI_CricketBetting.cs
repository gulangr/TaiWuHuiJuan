using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using Game.Components.Item;
using Game.Components.ListStyleGeneralScroll.Item;
using Game.Components.SortAndFilter.Item.Apply;
using Game.Views.Migrate;
using GameData.Domains.Character.Display;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.TaiwuEvent;
using GameData.Domains.TaiwuEvent.DisplayEvent;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UICommon.Character.Elements;
using UnityEngine;

// Token: 0x020001F8 RID: 504
public class UI_CricketBetting : UIBase
{
	// Token: 0x1700034E RID: 846
	// (get) Token: 0x060020AB RID: 8363 RVA: 0x000EDF92 File Offset: 0x000EC192
	private int TaiwuCharId
	{
		get
		{
			return SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
		}
	}

	// Token: 0x060020AC RID: 8364 RVA: 0x000EDF9E File Offset: 0x000EC19E
	public override void OnInit(ArgumentBox argsBox)
	{
		this._cricketBettingData = null;
		argsBox.Get<EventCricketBettingData>("CricketBettingData", out this._cricketBettingData);
		argsBox.Get("TaiwuName", out this._taiwuName);
	}

	// Token: 0x060020AD RID: 8365 RVA: 0x000EDFCC File Offset: 0x000EC1CC
	public void Awake()
	{
		this.wagerTypeTogGroup.OnActiveIndexChange += this.OnChipTogChanged;
		this.wagerTypeTogGroup.Init(0);
		this.characterTogGroup.OnActiveIndexChange += this.OnChipCharacterClicked;
		this.characterScrollView.OnItemRender += this.OnRenderCharList;
		this.itemListScroll.Init("CricketBettingItems", ESortAndFilterControllerType.Item, true, new Action<ITradeableContent, RowItemLine>(this.OnRenderItemList), null, ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Amount | ItemListScroll.EColumnType.Type | ItemListScroll.EColumnType.Weight | ItemListScroll.EColumnType.Value | ItemListScroll.EColumnType.Durability, null, null, null);
		this.confirmBtn.ClearAndAddListener(new Action(this.OnConfirmBtnClicked));
		this.cancelBtn.ClearAndAddListener(new Action(this.OnCancelBtnClicked));
		this.rewardGroup.OnActiveIndexChange += this.OnRewardBtnClicked;
	}

	// Token: 0x060020AE RID: 8366 RVA: 0x000EE0A0 File Offset: 0x000EC2A0
	private void OnDestroy()
	{
		this.wagerTypeTogGroup.OnActiveIndexChange -= this.OnChipTogChanged;
		this.characterTogGroup.OnActiveIndexChange -= this.OnChipCharacterClicked;
		this.characterScrollView.OnItemRender -= this.OnRenderCharList;
		this.rewardGroup.OnActiveIndexChange -= this.OnRewardBtnClicked;
	}

	// Token: 0x060020AF RID: 8367 RVA: 0x000EE110 File Offset: 0x000EC310
	private void OnEnable()
	{
		this.wagerTypeTogGroup.Set(0, false);
		this.characterScrollView.UpdateData(0);
		InfinityScroll infinityScroll = this.characterScrollView;
		List<CharacterDisplayData> betCharacters = this._cricketBettingData.BetCharacters;
		infinityScroll.UpdateData(((betCharacters != null) ? betCharacters.Count : 0) + 1);
		this.itemListScroll.SetItemList(this._cricketBettingData.BetItems);
		this.confirmBtn.interactable = false;
		for (int index = 0; index < this._cricketBettingData.BetRewards.Count; index++)
		{
			this.UpdateRewardList(index);
		}
		this.targetWager.SetData(Wager.Invalid);
		this.UpdateWager();
		SingletonObject.getInstance<YieldHelper>().DelayFrameDo(1U, delegate
		{
			this.verticalScrollbar.value = 0f;
		});
	}

	// Token: 0x060020B0 RID: 8368 RVA: 0x000EE1DC File Offset: 0x000EC3DC
	private void OnDisable()
	{
		foreach (GameObject obj in this._rewardObjects)
		{
			Object.Destroy(obj);
		}
		this.ResetCharList();
		this._selectedToggle = -1;
		this._selectedReward = -1;
		this._selectedRewardMinValue = long.MaxValue;
		this._selectedWager = Wager.Invalid;
		this._selectedChipItem = null;
		this._selectingChipItem = null;
		this.characterTogGroup.Clear();
		this.rewardGroup.Clear();
	}

	// Token: 0x060020B1 RID: 8369 RVA: 0x000EE288 File Offset: 0x000EC488
	private void Update()
	{
		bool flag = CommonCommandKit.Space.Check(this.Element, false, false, false, true, false);
		if (flag)
		{
			bool interactable = this.confirmBtn.interactable;
			if (interactable)
			{
				this.confirmBtn.onClick.Invoke();
				this.confirmBtn.interactable = false;
			}
		}
	}

	// Token: 0x060020B2 RID: 8370 RVA: 0x000EE2E0 File Offset: 0x000EC4E0
	public override void QuickHide()
	{
		this.OnCancelBtnClicked();
	}

	// Token: 0x060020B3 RID: 8371 RVA: 0x000EE2EC File Offset: 0x000EC4EC
	private void OnChipTogChanged(int togNew, int togOld)
	{
		bool flag = togNew == -1;
		if (!flag)
		{
			bool flag2 = togNew == 0;
			if (flag2)
			{
				this.itemSelectHolder.SetActive(true);
				this.characterSelectHolder.SetActive(false);
			}
			else
			{
				this.itemSelectHolder.SetActive(false);
				this.characterSelectHolder.SetActive(true);
			}
			this._selectedToggle = togNew;
		}
	}

	// Token: 0x060020B4 RID: 8372 RVA: 0x000EE350 File Offset: 0x000EC550
	private void OnChipItemConfirmed(int count)
	{
		bool flag = count > 0;
		if (flag)
		{
			this.UpdateChipItem(count);
		}
		else
		{
			this.OnChipItemCanceled();
		}
	}

	// Token: 0x060020B5 RID: 8373 RVA: 0x000EE377 File Offset: 0x000EC577
	private void OnChipItemCanceled()
	{
		this._selectingChipItem = null;
	}

	// Token: 0x060020B6 RID: 8374 RVA: 0x000EE384 File Offset: 0x000EC584
	private static string GetUsingOperationConfirmTip(ITradeableContent displayData)
	{
		ItemDisplayData itemDisplayData = displayData as ItemDisplayData;
		bool flag = itemDisplayData != null;
		string result;
		if (flag)
		{
			result = itemDisplayData.GetUsingOperationConfirmTip(ItemDisplayData.ItemUsingOperationType.Bet);
		}
		else
		{
			string usingTypeName = displayData.UsingType.ToString();
			string itemTypeName = (displayData.Key.ItemType == 10) ? LocalStringManager.Get(LanguageKey.LK_ItemType_10) : LocalStringManager.Get(LanguageKey.LK_Item);
			string operationName = LocalStringManager.Get(LanguageKey.LK_ItemUsingOperationType_Bet);
			string content = LocalStringManager.GetFormat(LanguageKey.LK_ItemUsing_ConfirmTip, usingTypeName, itemTypeName, operationName).ColorReplace();
			result = content;
		}
		return result;
	}

	// Token: 0x060020B7 RID: 8375 RVA: 0x000EE414 File Offset: 0x000EC614
	private void OnChipItemClicked(ITradeableContent content, RowItemLine rowItemLine)
	{
		UI_CricketBetting.<>c__DisplayClass39_0 CS$<>8__locals1 = new UI_CricketBetting.<>c__DisplayClass39_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.content = content;
		CS$<>8__locals1.rowItemLine = rowItemLine;
		bool flag = CS$<>8__locals1.content.UsingType != ItemDisplayData.ItemUsingType.Invalid;
		if (flag)
		{
			DialogCmd cmd = new DialogCmd
			{
				Title = LocalStringManager.Get(LanguageKey.LK_Common_Attention),
				Content = UI_CricketBetting.GetUsingOperationConfirmTip(CS$<>8__locals1.content),
				Type = 1,
				Yes = delegate()
				{
					CS$<>8__locals1.<>4__this.itemListScroll.HandleClickItem(CS$<>8__locals1.content, CS$<>8__locals1.rowItemLine, new Action<RowItemLine>(base.<OnChipItemClicked>g__Action|0));
				}
			};
			UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
			UIManager.Instance.MaskUI(UIElement.Dialog);
		}
		else
		{
			this.itemListScroll.HandleClickItem(CS$<>8__locals1.content, CS$<>8__locals1.rowItemLine, new Action<RowItemLine>(CS$<>8__locals1.<OnChipItemClicked>g__Action|0));
		}
	}

	// Token: 0x060020B8 RID: 8376 RVA: 0x000EE4E8 File Offset: 0x000EC6E8
	private void OnChipCharacterClicked(int togNew, int togOld)
	{
		bool flag = togNew == -1;
		if (!flag)
		{
			bool flag2 = this._selectedToggle == 1;
			if (flag2)
			{
				this._selectedChipItem = this._noneItem;
			}
			CToggle togObj = this.characterTogGroup.Get(togNew);
			togObj.GetComponent<SelectableCharacter>().hoverLight.SetActive(false);
			this._selectedWager = ((togNew == 0) ? Wager.Invalid : Wager.CreateChar(this.GetCharIdByIndex(togNew)));
			this.UpdateWager();
		}
	}

	// Token: 0x060020B9 RID: 8377 RVA: 0x000EE55C File Offset: 0x000EC75C
	private void OnRewardBtnClicked(int togNew, int togOld)
	{
		bool flag = togNew == -1;
		if (!flag)
		{
			CricketWagerData reward = this._cricketBettingData.BetRewards[togNew];
			this._selectedReward = togNew;
			this._selectedRewardMinValue = reward.MinWagerValue;
			this.targetWager.SetData(reward.Wager);
			List<CToggle> togList = this.characterTogGroup.GetAll();
			for (int i = 0; i < togList.Count; i++)
			{
				bool flag2 = i == 0;
				if (!flag2)
				{
					CToggle toggle = togList[i];
					long value = this._cricketBettingData.BetCharacterValueMap[this.GetCharIdByIndex(i)];
					toggle.GetComponent<CricketBettingCharacterSelecting>().value.text = value.ToString().SetColor((value >= this._selectedRewardMinValue) ? "A7988A" : "9D3A20");
				}
			}
			this.itemListScroll.ReRender();
			this.UpdateWager();
		}
	}

	// Token: 0x060020BA RID: 8378 RVA: 0x000EE648 File Offset: 0x000EC848
	private void OnConfirmBtnClicked()
	{
		TaiwuEventDomainMethod.Call.SetCricketBettingResult(true, this._selectedWager, this._selectedReward);
		UIManager.Instance.HideUI(this.Element);
	}

	// Token: 0x060020BB RID: 8379 RVA: 0x000EE66F File Offset: 0x000EC86F
	private void OnCancelBtnClicked()
	{
		TaiwuEventDomainMethod.Call.SetCricketBettingResult(false, Wager.Invalid, this._selectedReward);
		UIManager.Instance.HideUI(this.Element);
	}

	// Token: 0x060020BC RID: 8380 RVA: 0x000EE698 File Offset: 0x000EC898
	private Wager GetWagerByItemDisplayData(ITradeableContent data, int count = 1)
	{
		ItemKey key = data.Key;
		return (data.Key == ItemKey.Invalid) ? Wager.Invalid : (data.IsResource ? Wager.CreateResource(ItemTemplateHelper.GetMiscResourceType(key.ItemType, key.TemplateId), count) : Wager.CreateItem(key, count));
	}

	// Token: 0x060020BD RID: 8381 RVA: 0x000EE6F2 File Offset: 0x000EC8F2
	private int GetMinAmountByMinValue()
	{
		return (int)Math.Min(2147483647.0, Math.Ceiling((double)((float)this._selectedRewardMinValue / (float)this.GetWagerValue(false))));
	}

	// Token: 0x060020BE RID: 8382 RVA: 0x000EE71C File Offset: 0x000EC91C
	private long GetWagerValue(bool isSelected)
	{
		RowItemLine item = isSelected ? this._selectedChipItem : this._selectingChipItem;
		Wager wager = isSelected ? this._selectedWager : ((item == null) ? Wager.Invalid : this.GetWagerByItemDisplayData(item.Data, 1));
		switch (wager.Type)
		{
		case -1:
			return 0L;
		case 1:
			return wager.CalcWagerValue((item == null) ? 0 : ((int)item.Data.Value), 0, 0, 0, -1, 0);
		case 2:
			return (wager.CharId >= 0) ? this._cricketBettingData.BetCharacterValueMap[wager.CharId] : 0L;
		}
		return wager.CalcWagerValue(0, 0, 0, 0, -1, 0);
	}

	// Token: 0x060020BF RID: 8383 RVA: 0x000EE7F0 File Offset: 0x000EC9F0
	private int GetCharIdByIndex(int index)
	{
		return (index > 0) ? this._cricketBettingData.BetCharacters[index - 1].CharacterId : -1;
	}

	// Token: 0x060020C0 RID: 8384 RVA: 0x000EE821 File Offset: 0x000ECA21
	private void UpdateChipItem(int count)
	{
		this.ResetCharList();
		this._selectedChipItem = this._selectingChipItem;
		this._selectedWager = this.GetWagerByItemDisplayData(this._selectedChipItem.Data, count);
		this._selectingChipItem = null;
		this.UpdateWager();
	}

	// Token: 0x060020C1 RID: 8385 RVA: 0x000EE860 File Offset: 0x000ECA60
	private void UpdateRewardList(int index)
	{
		GameObject obj = Object.Instantiate<GameObject>(this.rewardTemplate, this.rewardGroup.transform);
		CToggle toggle = obj.GetComponent<CToggle>();
		CricketBettingRewardTemplate refers = obj.GetComponent<CricketBettingRewardTemplate>();
		PointerTrigger trigger = obj.GetComponent<PointerTrigger>();
		CricketWagerData reward = this._cricketBettingData.BetRewards[index];
		switch (reward.Wager.Type)
		{
		case 0:
		{
			short templateId = Convert.ToInt16((int)reward.Wager.WagerResourceType);
			ItemKey itemKey = new ItemKey(12, 0, templateId, 0);
			ItemDisplayData itemData = new ItemDisplayData
			{
				Key = itemKey,
				Amount = reward.Wager.Count
			};
			ItemBack rewardItemObj = refers.rewardItem;
			rewardItemObj.Set(itemData, false);
			rewardItemObj.GetComponent<CButton>().ClearAndAddListener(delegate
			{
				this.rewardGroup.Set(index, false);
			});
			rewardItemObj.gameObject.SetActive(true);
			refers.rewardName.text = ResourceType.Instance[reward.Wager.WagerResourceType].Name;
			break;
		}
		case 1:
		{
			Action <>9__2;
			ItemDomainMethod.AsyncCall.GetItemDisplayData(null, reward.Wager.ItemKey, this.TaiwuCharId, delegate(int offset, RawDataPool dataPool)
			{
				ItemDisplayData itemDisplayData = null;
				Serializer.Deserialize(dataPool, offset, ref itemDisplayData);
				ItemBack rewardObj = refers.rewardItem;
				rewardObj.Set(itemDisplayData, false);
				CButton component2 = rewardObj.GetComponent<CButton>();
				Action action2;
				if ((action2 = <>9__2) == null)
				{
					action2 = (<>9__2 = delegate()
					{
						this.rewardGroup.Set(index, false);
					});
				}
				component2.ClearAndAddListener(action2);
				rewardObj.gameObject.SetActive(true);
				refers.rewardName.text = LocalStringManager.Get(string.Format("LK_ItemType_{0}", reward.Wager.ItemKey.ItemType));
			});
			break;
		}
		case 3:
			refers.count.text = reward.Wager.Count.ToString();
			refers.countBack.SetActive(reward.Wager.Count > 1);
			refers.rewardExp.SetActive(true);
			refers.rewardName.text = LocalStringManager.Get(LanguageKey.LK_Exp);
			break;
		}
		Action <>9__3;
		for (int i = 0; i < 3; i++)
		{
			CardItem cricketObj = refers.cricketList[i];
			bool visible = reward.IsShowCricket(i);
			RowItemMain rowItemMain = cricketObj.GetComponent<RowItemMain>();
			rowItemMain.SetData(reward.Crickets[i]);
			cricketObj.Set(rowItemMain, true);
			CButton component = cricketObj.GetComponent<CButton>();
			Action action;
			if ((action = <>9__3) == null)
			{
				action = (<>9__3 = delegate()
				{
					this.rewardGroup.Set(index, false);
				});
			}
			component.ClearAndAddListener(action);
			cricketObj.CricketView.skeletonGraphic.enabled = visible;
			cricketObj.CricketView.GetComponent<TooltipInvoker>().enabled = visible;
			cricketObj.GetComponent<TooltipInvoker>().enabled = visible;
			cricketObj.gameObject.SetActive(true);
			refers.coverList[i].SetActive(!visible);
			trigger.EnterEvent.AddListener(delegate()
			{
				cricketObj.CricketView.Sing(true, true, true, -1f, null, 0.5f);
			});
			trigger.ExitEvent.AddListener(delegate()
			{
				bool flag2 = cricketObj.CricketView == null;
				if (!flag2)
				{
					cricketObj.CricketView.StopSing(0.2f);
				}
			});
		}
		obj.SetActive(true);
		this._rewardObjects.Add(obj);
		this.rewardGroup.Add(toggle);
		bool flag = index == 0;
		if (flag)
		{
			this.rewardGroup.Set(index, true);
		}
	}

	// Token: 0x060020C2 RID: 8386 RVA: 0x000EEC00 File Offset: 0x000ECE00
	private void UpdateWager()
	{
		bool flag = this._selectedRewardMinValue > this.GetWagerValue(true);
		if (flag)
		{
			this.confirmBtn.interactable = false;
			this.disableText.SetActive(true);
			this.disableIcon.SetActive(true);
		}
		else
		{
			this.confirmBtn.interactable = true;
			this.disableText.SetActive(false);
			this.disableIcon.SetActive(false);
		}
		this.selfWager.SetData(this._selectedWager);
	}

	// Token: 0x060020C3 RID: 8387 RVA: 0x000EEC88 File Offset: 0x000ECE88
	private void ResetCharList()
	{
		CToggle tog = this.characterTogGroup.Get(0);
		bool flag = tog != null;
		if (flag)
		{
			this.characterTogGroup.Set(0, false);
		}
	}

	// Token: 0x060020C4 RID: 8388 RVA: 0x000EECBC File Offset: 0x000ECEBC
	private void OnRenderItemList(ITradeableContent content, RowItemLine rowItemLine)
	{
		ItemDisplayData itemData = (ItemDisplayData)content;
		RowItemMain roleItemMain = rowItemLine.GetComponentInChildren<RowItemMain>();
		roleItemMain.SetData(itemData);
		rowItemLine.Set(roleItemMain, !itemData.Key.Equals(ItemKey.Invalid));
		bool canSelect = itemData.Key.Equals(ItemKey.Invalid) || itemData.Value >= this._selectedRewardMinValue || (itemData.IsResource && this.GetWagerByItemDisplayData(itemData, itemData.Amount).CalcWagerValue(0, 0, 0, 0, -1, 0) >= this._selectedRewardMinValue);
		bool flag = canSelect;
		if (flag)
		{
			rowItemLine.SetLocked(false);
			rowItemLine.GetComponent<CButton>().interactable = true;
			rowItemLine.SetClickEvent(delegate
			{
				this.OnChipItemClicked(content, rowItemLine);
			});
		}
		else
		{
			rowItemLine.SetLocked(true);
			rowItemLine.GetComponent<CButton>().interactable = false;
		}
		bool isResource = itemData.IsResource;
		if (isResource)
		{
			RowItemLine.SetResourceTip(itemData, rowItemLine.TipDisplayer, this._taiwuName, true, false);
			rowItemLine.GetComponent<TooltipInvoker>().enabled = true;
		}
		else
		{
			bool flag2 = itemData.Key.Equals(ItemKey.Invalid);
			if (flag2)
			{
				rowItemLine.GetComponent<TooltipInvoker>().enabled = false;
			}
			else
			{
				rowItemLine.GetComponent<TooltipInvoker>().enabled = true;
			}
		}
		bool flag3 = itemData.Key == ItemKey.Invalid;
		if (flag3)
		{
			this._noneItem = rowItemLine;
		}
	}

	// Token: 0x060020C5 RID: 8389 RVA: 0x000EEE84 File Offset: 0x000ED084
	private void OnRenderCharList(int index, GameObject charObj)
	{
		SelectableCharacter charRefers = charObj.GetComponent<SelectableCharacter>();
		bool flag = index == 0;
		if (flag)
		{
			charRefers.none.transform.parent.GetComponent<CImage>().enabled = false;
			charRefers.none.gameObject.SetActive(true);
			charRefers.GetComponent<SelectableCharacter>().CharacterId = -1;
			charRefers.characterName.text = LocalStringManager.Get(LanguageKey.LK_None);
			charRefers.transform.Find("CharacterInfo/Image/Value").GetComponent<TextMeshProUGUI>().text = "0";
			charRefers.avatar.ResetToBlank(false);
			charRefers.transform.Find("MarkLayout/Teammate").gameObject.SetActive(false);
			charRefers.transform.Find("MarkLayout/Prisoner").gameObject.SetActive(false);
		}
		else
		{
			int charId = this.GetCharIdByIndex(index);
			long value = this._cricketBettingData.BetCharacterValueMap[charId];
			bool isTeammate = SingletonObject.getInstance<CharacterMonitorModel>().IsTaiwuTeamCharacter(charId);
			charRefers.none.transform.parent.GetComponent<CImage>().enabled = true;
			charRefers.none.gameObject.SetActive(false);
			charRefers.GetComponent<SelectableCharacter>().CharacterId = charId;
			charRefers.transform.Find("MarkLayout/Teammate").gameObject.SetActive(isTeammate);
			charRefers.transform.Find("MarkLayout/Prisoner").gameObject.SetActive(!isTeammate);
			charRefers.transform.Find("CharacterInfo/Image/Value").GetComponent<TextMeshProUGUI>().text = value.ToString().SetColor((value >= this._selectedRewardMinValue) ? "A7988A" : "9D3A20");
		}
		CToggle toggle = charRefers.GetComponent<CToggle>();
		bool flag2 = this.characterTogGroup.Get(index) == null;
		if (flag2)
		{
			this.characterTogGroup.Add(toggle);
		}
	}

	// Token: 0x04001907 RID: 6407
	[SerializeField]
	private InfinityScroll characterScrollView;

	// Token: 0x04001908 RID: 6408
	[SerializeField]
	private ItemListScroll itemListScroll;

	// Token: 0x04001909 RID: 6409
	[SerializeField]
	private CToggleGroup wagerTypeTogGroup;

	// Token: 0x0400190A RID: 6410
	[SerializeField]
	private GameObject itemSelectHolder;

	// Token: 0x0400190B RID: 6411
	[SerializeField]
	private GameObject characterSelectHolder;

	// Token: 0x0400190C RID: 6412
	[SerializeField]
	private CToggleGroup characterTogGroup;

	// Token: 0x0400190D RID: 6413
	[SerializeField]
	private CButton cancelBtn;

	// Token: 0x0400190E RID: 6414
	[SerializeField]
	private CButton confirmBtn;

	// Token: 0x0400190F RID: 6415
	[SerializeField]
	private GameObject disableText;

	// Token: 0x04001910 RID: 6416
	[SerializeField]
	private GameObject disableIcon;

	// Token: 0x04001911 RID: 6417
	[SerializeField]
	private CricketCombatCricketWagerView selfWager;

	// Token: 0x04001912 RID: 6418
	[SerializeField]
	private CricketCombatCricketWagerView targetWager;

	// Token: 0x04001913 RID: 6419
	[SerializeField]
	private CToggleGroup rewardGroup;

	// Token: 0x04001914 RID: 6420
	[SerializeField]
	private GameObject rewardTemplate;

	// Token: 0x04001915 RID: 6421
	[SerializeField]
	private CScrollbar verticalScrollbar;

	// Token: 0x04001916 RID: 6422
	private string _taiwuName;

	// Token: 0x04001917 RID: 6423
	private EventCricketBettingData _cricketBettingData;

	// Token: 0x04001918 RID: 6424
	private int _selectedToggle = -1;

	// Token: 0x04001919 RID: 6425
	private int _selectedReward = -1;

	// Token: 0x0400191A RID: 6426
	private long _selectedRewardMinValue = long.MaxValue;

	// Token: 0x0400191B RID: 6427
	private Wager _selectedWager = Wager.Invalid;

	// Token: 0x0400191C RID: 6428
	private RowItemLine _selectedChipItem;

	// Token: 0x0400191D RID: 6429
	private RowItemLine _selectingChipItem;

	// Token: 0x0400191E RID: 6430
	private readonly List<GameObject> _rewardObjects = new List<GameObject>();

	// Token: 0x0400191F RID: 6431
	private RowItemLine _noneItem;

	// Token: 0x02001485 RID: 5253
	private class SelectItem
	{
		// Token: 0x0400A17E RID: 41342
		public RowItemLine RowItemLine;

		// Token: 0x0400A17F RID: 41343
		public bool IsValid;
	}
}
