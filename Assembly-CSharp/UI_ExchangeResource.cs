using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using CharacterDataMonitor;
using Config;
using FrameWork;
using Game.Components.Avatar;
using GameData.Common;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Character.Relation;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UICommon.Character;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x0200037E RID: 894
public class UI_ExchangeResource : UIBase
{
	// Token: 0x060034AC RID: 13484 RVA: 0x001A4368 File Offset: 0x001A2568
	public override void OnInit(ArgumentBox argsBox)
	{
		this._taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
		argsBox.Get("NpcCharId", out this._npcCharId);
		argsBox.Get("WorthLimited", out this._worthLimited);
		this.InitNpc();
		this.ShowDebtButtonTips();
		this._taiwuResources.Items.FixedElementField = -1;
		this._npcResources.Items.FixedElementField = -1;
		this._maxWorthCanBeLentToTaiwu = -1L;
		base.CGet<Refers>("DebtChange").gameObject.SetActive(false);
		UIElement element = this.Element;
		element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(this.OnListenerIdReady));
	}

	// Token: 0x060034AD RID: 13485 RVA: 0x001A4422 File Offset: 0x001A2622
	private void Awake()
	{
		this._confirmBtn = base.CGet<CButtonObsolete>("Confirm");
	}

	// Token: 0x060034AE RID: 13486 RVA: 0x001A4438 File Offset: 0x001A2638
	public override void InitMonitorFieldIds()
	{
		this.MonitorFields.Add(new UIBase.MonitorDataField(5, 31, ulong.MaxValue, null));
		this.MonitorFields.Add(new UIBase.MonitorDataField(4, 0, (ulong)this._taiwuCharId, new uint[]
		{
			34U
		}));
		this.MonitorFields.Add(new UIBase.MonitorDataField(4, 0, (ulong)this._npcCharId, new uint[]
		{
			34U
		}));
	}

	// Token: 0x060034AF RID: 13487 RVA: 0x001A44A8 File Offset: 0x001A26A8
	private void OnListenerIdReady()
	{
		List<int> charIdList = new List<int>
		{
			this._taiwuCharId,
			this._npcCharId
		};
		CharacterDomainMethod.Call.GetCharacterDisplayDataList(this.Element.GameDataListenerId, charIdList);
		CharacterDomainMethod.Call.GetMaxWorthCanBeLentToTaiwu(this.Element.GameDataListenerId, this._npcCharId);
	}

	// Token: 0x060034B0 RID: 13488 RVA: 0x001A4500 File Offset: 0x001A2700
	public override void OnNotifyGameData(List<NotificationWrapper> notifications)
	{
		foreach (NotificationWrapper wrapper in notifications)
		{
			Notification notification = wrapper.Notification;
			byte type = notification.Type;
			byte b = type;
			if (b != 0)
			{
				if (b == 1)
				{
					bool flag = notification.DomainId == 4;
					if (flag)
					{
						bool flag2 = notification.MethodId == 48;
						if (flag2)
						{
							List<CharacterDisplayData> dataList = new List<CharacterDisplayData>();
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref dataList);
							CharacterDisplayData taiwuData = dataList[0];
							base.CGet<Game.Components.Avatar.Avatar>("TaiwuAvatar").Refresh(taiwuData, true);
							base.CGet<TextMeshProUGUI>("TaiwuName").text = NameCenter.GetCharMonasticTitleOrNameByDisplayData(taiwuData, true, false);
							CharacterDisplayData npcData = dataList[1];
							base.CGet<Game.Components.Avatar.Avatar>("NpcAvatar").Refresh(npcData, true);
							base.CGet<TextMeshProUGUI>("NpcName").text = NameCenter.GetCharMonasticTitleOrNameByDisplayData(npcData, false, false);
						}
						else
						{
							bool flag3 = notification.MethodId == 45;
							if (flag3)
							{
								Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._maxWorthCanBeLentToTaiwu);
								bool flag4 = this._taiwuResources.Items.FixedElementField >= 0 && this._npcResources.Items.FixedElementField >= 0;
								if (flag4)
								{
									this.InitResourceList();
								}
								this.Element.ShowAfterRefresh();
							}
							else
							{
								bool flag5 = notification.MethodId == 46;
								if (flag5)
								{
									bool canTransfer = false;
									Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref canTransfer);
									this.UpdateConfirmBtn(canTransfer);
								}
							}
						}
					}
				}
			}
			else
			{
				DataUid uid = notification.Uid;
				bool flag6 = uid.DomainId == 4 && uid.DataId == 0 && uid.SubId1 == 34U;
				if (flag6)
				{
					bool flag7 = (int)uid.SubId0 == this._taiwuCharId;
					if (flag7)
					{
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._taiwuResources);
					}
					else
					{
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._npcResources);
					}
					bool flag8 = this._taiwuResources.Items.FixedElementField >= 0 && this._npcResources.Items.FixedElementField >= 0 && this._maxWorthCanBeLentToTaiwu >= 0L;
					if (flag8)
					{
						this.InitResourceList();
					}
				}
				else
				{
					bool flag9 = uid.DomainId == 5 && uid.DataId == 31;
					if (flag9)
					{
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._groupCharacterSet);
						base.CGet<GameObject>("DisableMask").gameObject.SetActive(!this._groupCharacterSet.Contains(this._npcCharId));
					}
				}
			}
		}
	}

	// Token: 0x060034B1 RID: 13489 RVA: 0x001A4808 File Offset: 0x001A2A08
	protected override void OnClick(Transform btn)
	{
		bool flag = btn.name == "Confirm";
		if (flag)
		{
			this.ConfirmClick();
		}
		else
		{
			bool flag2 = btn.name == "Cancel";
			if (flag2)
			{
				this.QuickHide();
			}
			else
			{
				bool flag3 = btn.name == "ButtonReset";
				if (flag3)
				{
					this.OnClickButtonReset();
				}
				else
				{
					bool flag4 = btn.name == "ButtonBalance";
					if (flag4)
					{
						this.OnClickButtonBalance();
					}
				}
			}
		}
	}

	// Token: 0x060034B2 RID: 13490 RVA: 0x001A4890 File Offset: 0x001A2A90
	private void ConfirmClick()
	{
		bool flag = this._npcExchangeResources.IsNonZero();
		if (flag)
		{
			CharacterDomainMethod.Call.TransferResourcesWithDebt(this._npcCharId, this._taiwuCharId, this._npcExchangeResources, this._worthLimited);
		}
		bool flag2 = this._taiwuExchangeResources.IsNonZero();
		if (flag2)
		{
			CharacterDomainMethod.Call.TransferResourcesWithDebt(this._taiwuCharId, this._npcCharId, this._taiwuExchangeResources, this._worthLimited);
		}
		SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<BasicInfoMonitor>(this._npcCharId, false).Refresh();
		SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<ResourceMonitor>(this._taiwuCharId, false).Refresh();
		SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<ResourceMonitor>(this._npcCharId, false).Refresh();
		this.QuickHide();
		GEvent.OnEvent(UiEvents.ExchangeResource, null);
	}

	// Token: 0x060034B3 RID: 13491 RVA: 0x001A4958 File Offset: 0x001A2B58
	private void InitNpc()
	{
		Refers npcInfo = base.CGet<Refers>("NpcInfo");
		CImage npcFavorIcon = npcInfo.CGet<CImage>("FavorIcon");
		TextMeshProUGUI npcFavorValue = npcInfo.CGet<TextMeshProUGUI>("FavorValue");
		this._npcFavorabilityHandler = new CharacterFavorability(npcFavorIcon, npcFavorValue, null, null, null);
		this._npcFavorabilityHandler.CharacterId = this._npcCharId;
		BasicInfoMonitor monitor = this._npcFavorabilityHandler.GetMonitor<BasicInfoMonitor>();
		sbyte favorabilityType = FavorabilityType.GetFavorabilityType(monitor.FavorabilityToTaiwu);
		string fillSpriteName = this.GetFavorFillSpriteNameByFavorType(favorabilityType);
		CImage favorProgressFill = npcInfo.CGet<CImage>("FavorProgress");
		favorProgressFill.SetSpriteOnly(fillSpriteName, false, null);
		ValueTuple<short, short> favorabilityRange = FavorabilityType.GetFavorabilityRange(monitor.FavorabilityToTaiwu);
		short min = favorabilityRange.Item1;
		short max = favorabilityRange.Item2;
		float progress = (float)(monitor.FavorabilityToTaiwu - min) / (float)(max - min);
		favorProgressFill.fillAmount = progress;
	}

	// Token: 0x060034B4 RID: 13492 RVA: 0x001A4A20 File Offset: 0x001A2C20
	private void InitResourceList()
	{
		for (sbyte type = 0; type < 7; type += 1)
		{
			this.InitResource(type, this._taiwuResources.Get((int)type), this._taiwuCurResourceArray);
			this.InitResource(type, this._npcResources.Get((int)type), this._npcCurResourceArray);
			this.InitResource(type, 0, this._taiwuExchangeResourceArray);
			this.InitResource(type, 0, this._npcExchangeResourceArray);
			this._taiwuCurResourceItemArray[(int)type] = base.CGet<GameObject>("TaiwuCurResourcesLayout").transform.GetChild((int)type).GetComponent<Refers>();
			this._npcCurResourceItemArray[(int)type] = base.CGet<GameObject>("NpcCurResourcesLayout").transform.GetChild((int)type).GetComponent<Refers>();
			this._taiwuExchangeResourceItemArray[(int)type] = base.CGet<GameObject>("TaiwuExchangeResourcesLayout").transform.GetChild((int)type).GetComponent<Refers>();
			this._npcExchangeResourceItemArray[(int)type] = base.CGet<GameObject>("NpcExchangeResourcesLayout").transform.GetChild((int)type).GetComponent<Refers>();
			this.RefreshCurResource(type, true);
			this.RefreshCurResource(type, false);
			this.RefreshExchangeResource(type, true);
			this.RefreshExchangeResource(type, false);
		}
		this.RefreshResult();
		this.Element.ShowAfterRefresh();
	}

	// Token: 0x060034B5 RID: 13493 RVA: 0x001A4B5C File Offset: 0x001A2D5C
	private void InitResource(sbyte type, int amount, ItemDisplayData[] dataList)
	{
		short templateId = Convert.ToInt16((int)type);
		ItemKey itemKey = new ItemKey(12, 0, templateId, 0);
		ItemDisplayData itemData = new ItemDisplayData
		{
			Key = itemKey,
			Amount = amount
		};
		dataList[(int)type] = itemData;
	}

	// Token: 0x060034B6 RID: 13494 RVA: 0x001A4B98 File Offset: 0x001A2D98
	private void RefreshCurResource(sbyte type, bool isTaiwu)
	{
		ResourceTypeItem resourceConfig = Config.ResourceType.Instance[type];
		ItemDisplayData[] curResourceArray = isTaiwu ? this._taiwuCurResourceArray : this._npcCurResourceArray;
		ItemDisplayData curResourceData = curResourceArray[(int)type];
		Refers resourceItem = isTaiwu ? this._taiwuCurResourceItemArray[(int)type] : this._npcCurResourceItemArray[(int)type];
		ItemView itemView = resourceItem.CGet<ItemView>("ItemView");
		itemView.SetData(curResourceData, false, -1, false, true, null, false, true);
		itemView.SetInteractable(false);
		itemView.SetMask(false);
		itemView.SetCount(false, false, 0);
		resourceItem.CGet<TextMeshProUGUI>("Name").text = resourceConfig.Name;
		resourceItem.CGet<CImage>("Icon").SetSprite(resourceConfig.Icon, false, null);
		resourceItem.CGet<TextMeshProUGUI>("Amount").text = CommonUtils.GetDisplayStringForNum(curResourceData.Amount, 100000);
		CButtonObsolete button = resourceItem.CGet<CButtonObsolete>("Button");
		button.interactable = (curResourceData.Amount > 0);
		button.ClearAndAddListener(delegate
		{
			int limitSelectCount = curResourceData.Amount;
			short changeUnit = GlobalConfig.UnitsOfResourceTransfer[(int)type];
			bool isTaiwu2 = isTaiwu;
			int initCount;
			if (isTaiwu2)
			{
				int lackAmount = (this._remainWorthCanBeLentToTaiwu < 0L) ? Debts.WorthToResourceAmount((short)type, Math.Abs(this._remainWorthCanBeLentToTaiwu), true) : 0;
				initCount = Math.Min(lackAmount, limitSelectCount);
			}
			else
			{
				int remainCount = Debts.WorthToResourceAmount((short)type, Math.Max(0L, this._remainWorthCanBeLentToTaiwu), false);
				initCount = Math.Min(remainCount, limitSelectCount);
			}
			this.SelectCount(button, curResourceData.Amount, new Action<int>(base.<RefreshCurResource>g__OnConfirm|1), new Action(base.<RefreshCurResource>g__OnCancel|2), (int)changeUnit, initCount, limitSelectCount, null);
		});
	}

	// Token: 0x060034B7 RID: 13495 RVA: 0x001A4CF4 File Offset: 0x001A2EF4
	private void RefreshExchangeResource(sbyte type, bool isTaiwu)
	{
		ResourceTypeItem resourceConfig = Config.ResourceType.Instance[type];
		ItemDisplayData[] exchangeResourceArray = isTaiwu ? this._taiwuExchangeResourceArray : this._npcExchangeResourceArray;
		ItemDisplayData exchangeResourceData = exchangeResourceArray[(int)type];
		Refers resourceItem = isTaiwu ? this._taiwuExchangeResourceItemArray[(int)type] : this._npcExchangeResourceItemArray[(int)type];
		bool showItem = exchangeResourceData.Amount > 0;
		resourceItem.gameObject.SetActive(showItem);
		bool flag = !showItem;
		if (!flag)
		{
			ItemView itemView = resourceItem.CGet<ItemView>("ItemView");
			itemView.SetData(exchangeResourceData, false, -1, false, true, null, false, true);
			itemView.SetInteractable(false);
			itemView.SetMask(false);
			itemView.SetCount(false, false, 0);
			resourceItem.CGet<TextMeshProUGUI>("Name").text = resourceConfig.Name;
			resourceItem.CGet<CImage>("Icon").SetSprite(resourceConfig.Icon, false, null);
			resourceItem.CGet<TextMeshProUGUI>("Amount").text = CommonUtils.GetDisplayStringForNum(exchangeResourceData.Amount, 100000);
			CButtonObsolete button = resourceItem.CGet<CButtonObsolete>("Button");
			button.ClearAndAddListener(delegate
			{
				int limitSelectCount = exchangeResourceData.Amount;
				short changeUnit = GlobalConfig.UnitsOfResourceTransfer[(int)type];
				int initCount = 0;
				bool flag2 = !isTaiwu;
				if (flag2)
				{
					int overAmount = (this._remainWorthCanBeLentToTaiwu < 0L) ? Debts.WorthToResourceAmount((short)type, Math.Abs(this._remainWorthCanBeLentToTaiwu), true) : 0;
					initCount = overAmount;
				}
				this.SelectCount(button, exchangeResourceData.Amount, new Action<int>(base.<RefreshExchangeResource>g__OnConfirm|1), new Action(base.<RefreshExchangeResource>g__OnCancel|2), (int)changeUnit, initCount, limitSelectCount, null);
			});
		}
	}

	// Token: 0x060034B8 RID: 13496 RVA: 0x001A4E64 File Offset: 0x001A3064
	private void SelectCount(CButtonObsolete button, int amount, Action<int> onConfirm, Action onCancel, int changeUnit, int initSelectCount, int limitCount, string limitTip = null)
	{
		GameObject focusMask = base.CGet<GameObject>("FocusMask");
		focusMask.gameObject.SetActive(true);
		RectTransform itemRectTrans = button.GetComponent<RectTransform>();
		Transform originParent = itemRectTrans.parent;
		int originChildIndex = itemRectTrans.GetSiblingIndex();
		LayoutGroup layoutGroup = originParent.GetComponent<LayoutGroup>();
		layoutGroup.enabled = false;
		itemRectTrans.SetParent(focusMask.transform);
		int maxCount = (limitCount > 0) ? Mathf.Min(limitCount, amount) : amount;
		initSelectCount = Mathf.Clamp(initSelectCount, 0, maxCount);
		ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
		argBox.Set("MinCount", 0);
		argBox.Set("MaxCount", maxCount);
		argBox.Set("InitCount", initSelectCount);
		argBox.Set("LimitCount", limitCount);
		bool flag = limitCount >= amount;
		if (flag)
		{
			limitTip = string.Empty;
		}
		argBox.Set("LimitTip", limitTip);
		argBox.Set("ChangeValue", changeUnit);
		argBox.SetObject("FollowOffset", Vector2.zero);
		argBox.SetObject("OnConfirmSetCount", onConfirm);
		argBox.SetObject("OnCancelSetCount", onCancel);
		argBox.SetObject("ItemRectTrans", itemRectTrans);
		argBox.Set("ZeroValid", true);
		UIElement.SetSelectCount.SetOnInitArgs(argBox);
		UIManager.Instance.ShowUI(UIElement.SetSelectCount, true);
		UIElement setSelectCount = UIElement.SetSelectCount;
		setSelectCount.OnShowed = (Action)Delegate.Combine(setSelectCount.OnShowed, new Action(delegate()
		{
			button.ClearAndAddListener(delegate
			{
				GEvent.OnEvent(UiEvents.OnConfirmSetSelectCount, null);
			});
		}));
		UIElement setSelectCount2 = UIElement.SetSelectCount;
		setSelectCount2.OnHide = (Action)Delegate.Combine(setSelectCount2.OnHide, new Action(delegate()
		{
			focusMask.gameObject.SetActive(false);
			itemRectTrans.SetParent(originParent);
			itemRectTrans.SetSiblingIndex(originChildIndex);
			layoutGroup.enabled = true;
		}));
	}

	// Token: 0x060034B9 RID: 13497 RVA: 0x001A504C File Offset: 0x001A324C
	private void UpdateDebtChange()
	{
		ResourceInts exchangeResources = default(ResourceInts);
		exchangeResources.Add(ref this._taiwuExchangeResources);
		exchangeResources = exchangeResources.Subtract(ref this._npcExchangeResources);
		CharacterDomainMethod.AsyncCall.CheckDebtChange(this, this._npcCharId, exchangeResources, delegate(int offset, RawDataPool dataPool)
		{
			ValueTuple<sbyte, short> debtChange = default(ValueTuple<sbyte, short>);
			Serializer.Deserialize(dataPool, offset, ref debtChange);
			Refers debtChangeRefers = base.CGet<Refers>("DebtChange");
			debtChangeRefers.CGet<CImage>("Arrow").SetSprite(string.Format("popup_debt_bianhua_{0}", Mathf.Min(3, (int)debtChange.Item1)), false, null);
			debtChangeRefers.CGet<CImage>("DebtChangeImage").SetSprite(string.Format("popup_debt_zi_{0}", debtChange.Item1), false, null);
			debtChangeRefers.gameObject.SetActive(this._taiwuExchangeResources.IsNonZero() || this._npcExchangeResources.IsNonZero());
		});
	}

	// Token: 0x060034BA RID: 13498 RVA: 0x001A5098 File Offset: 0x001A3298
	private void RefreshResult()
	{
		long debtWorth = 0L;
		this._taiwuExchangeResources.Initialize();
		this._npcExchangeResources.Initialize();
		for (sbyte type = 0; type < 7; type += 1)
		{
			this._taiwuExchangeResources.Add(type, this._taiwuExchangeResourceArray[(int)type].Amount);
			this._npcExchangeResources.Add(type, this._npcExchangeResourceArray[(int)type].Amount);
			debtWorth += Debts.ResourceAmountToWorth((short)type, this._taiwuExchangeResourceArray[(int)type].Amount);
			debtWorth -= Debts.ResourceAmountToWorth((short)type, this._npcExchangeResourceArray[(int)type].Amount);
		}
		base.CGet<CImage>("TakeMark").SetSprite(this.GetMarkSpriteName(debtWorth, false), false, null);
		base.CGet<CImage>("GiveMark").SetSprite(this.GetMarkSpriteName(debtWorth, true), false, null);
		this._remainWorthCanBeLentToTaiwu = this._maxWorthCanBeLentToTaiwu + debtWorth;
		bool worthIsMeet = this._worthLimited && this._remainWorthCanBeLentToTaiwu >= 0L;
		bool worthPass = worthIsMeet || !this._worthLimited;
		bool hasExchange = this._taiwuExchangeResources.IsNonZero() || this._npcExchangeResources.IsNonZero();
		this.UpdateConfirmBtn(worthPass && hasExchange);
		long taiwuResourceWorth = Debts.ResourceAmountToWorth(ref this._taiwuExchangeResources);
		long npcResourceWorth = Debts.ResourceAmountToWorth(ref this._npcExchangeResources);
		bool canBalance = taiwuResourceWorth != npcResourceWorth;
		base.CGet<CButtonObsolete>("ButtonBalance").interactable = canBalance;
		this.UpdateDebtChange();
		TextMeshProUGUI limitTip = base.CGet<TextMeshProUGUI>("LimitTip");
		ResourceTypeItem moneyConfig = Config.ResourceType.Instance[6];
		int moneyCount = Debts.WorthToResourceAmount(6, Math.Max(0L, this._remainWorthCanBeLentToTaiwu), false);
		string moneyCountStr = CommonUtils.GetDisplayStringForNum(moneyCount, 100000);
		string moneyStr = (moneyConfig.Name + LocalStringManager.Get(LanguageKey.LK_Colon_Symbol) + moneyCountStr).SetColor("pinkyellow");
		limitTip.text = LocalStringManager.GetFormat(LanguageKey.LK_Exchange_Resource_Limit, moneyConfig.Icon, moneyStr);
		limitTip.GetComponent<TMPTextSpriteHelper>().Parse();
		base.CGet<CButtonObsolete>("ButtonReset").interactable = hasExchange;
		TooltipInvoker tip = this._confirmBtn.GetComponentInChildren<TooltipInvoker>();
		tip.enabled = (hasExchange && !worthIsMeet);
		string[] presetParam = tip.PresetParam;
		bool flag = presetParam == null || presetParam.Length < 1;
		if (flag)
		{
			tip.PresetParam = new string[1];
		}
		tip.PresetParam[0] = LocalStringManager.Get(LanguageKey.LK_Exchange_Resource_OverLimit).SetColor("brightred");
	}

	// Token: 0x060034BB RID: 13499 RVA: 0x001A531C File Offset: 0x001A351C
	private string GetMarkSpriteName(long worth, bool isPositive)
	{
		int type = isPositive ? 1 : 0;
		long index = 0L;
		bool flag = (isPositive && worth > 0L) || (!isPositive && worth < 0L);
		if (flag)
		{
			index = Math.Clamp(Math.Abs(worth) / 2000L + 1L, 1L, 5L);
		}
		return string.Format("popup_exchange_arrow_{0}_{1}", type, index);
	}

	// Token: 0x060034BC RID: 13500 RVA: 0x001A5383 File Offset: 0x001A3583
	private void UpdateConfirmBtn(bool canTransfer)
	{
		this._confirmBtn.interactable = canTransfer;
	}

	// Token: 0x060034BD RID: 13501 RVA: 0x001A5393 File Offset: 0x001A3593
	private void ShowDebtButtonTips()
	{
	}

	// Token: 0x060034BE RID: 13502 RVA: 0x001A5396 File Offset: 0x001A3596
	private void OnClickButtonReset()
	{
		this.InitResourceList();
	}

	// Token: 0x060034BF RID: 13503 RVA: 0x001A53A0 File Offset: 0x001A35A0
	private void OnClickButtonBalance()
	{
		long taiwuResourceWorth = Debts.ResourceAmountToWorth(ref this._taiwuExchangeResources);
		long npcResourceWorth = Debts.ResourceAmountToWorth(ref this._npcExchangeResources);
		bool flag = taiwuResourceWorth == npcResourceWorth;
		if (!flag)
		{
			ItemDisplayData[] lowAddDataArray = new ItemDisplayData[this._taiwuCurResourceArray.Length];
			ItemDisplayData[] higSubtractDataArray = new ItemDisplayData[this._taiwuCurResourceArray.Length];
			ItemDisplayData[] lowCurResourceArray = (taiwuResourceWorth > npcResourceWorth) ? this._npcCurResourceArray : this._taiwuCurResourceArray;
			ItemDisplayData[] highCurResourceArray = (taiwuResourceWorth > npcResourceWorth) ? this._taiwuCurResourceArray : this._npcCurResourceArray;
			ItemDisplayData[] lowExchangeResourceArray = (taiwuResourceWorth > npcResourceWorth) ? this._npcExchangeResourceArray : this._taiwuExchangeResourceArray;
			ItemDisplayData[] highExchangeResourceArray = (taiwuResourceWorth > npcResourceWorth) ? this._taiwuExchangeResourceArray : this._npcExchangeResourceArray;
			List<ItemDisplayData> sortedLowResourceList = new List<ItemDisplayData>();
			sortedLowResourceList.AddRange(lowCurResourceArray);
			sortedLowResourceList = sortedLowResourceList.OrderByDescending(delegate(ItemDisplayData data)
			{
				sbyte type2 = ItemTemplateHelper.GetMiscResourceType(data.Key.ItemType, data.Key.TemplateId);
				return Debts.ResourceAmountToWorth((short)type2, data.Amount);
			}).ToList<ItemDisplayData>();
			List<ItemDisplayData> sortedHighResourceList = new List<ItemDisplayData>();
			sortedHighResourceList.AddRange(highExchangeResourceArray);
			sortedHighResourceList = sortedHighResourceList.OrderByDescending(delegate(ItemDisplayData data)
			{
				sbyte type2 = ItemTemplateHelper.GetMiscResourceType(data.Key.ItemType, data.Key.TemplateId);
				return Debts.ResourceAmountToWorth((short)type2, data.Amount);
			}).ToList<ItemDisplayData>();
			UI_ExchangeResource.<>c__DisplayClass39_0 CS$<>8__locals1;
			CS$<>8__locals1.needWorth = Math.Abs(taiwuResourceWorth - npcResourceWorth);
			UI_ExchangeResource.<OnClickButtonBalance>g__Calc|39_2(sortedLowResourceList, lowAddDataArray, ref CS$<>8__locals1);
			bool flag2 = CS$<>8__locals1.needWorth > 0L;
			if (flag2)
			{
				UI_ExchangeResource.<OnClickButtonBalance>g__Calc|39_2(sortedHighResourceList, higSubtractDataArray, ref CS$<>8__locals1);
			}
			for (sbyte type = 0; type < 7; type += 1)
			{
				ItemDisplayData addData = lowAddDataArray[(int)type];
				bool flag3 = addData != null;
				if (flag3)
				{
					lowCurResourceArray[(int)type].Amount -= addData.Amount;
					lowExchangeResourceArray[(int)type].Amount += addData.Amount;
				}
				ItemDisplayData subtractData = higSubtractDataArray[(int)type];
				bool flag4 = subtractData != null;
				if (flag4)
				{
					higSubtractDataArray[(int)type].Amount -= subtractData.Amount;
					highCurResourceArray[(int)type].Amount += subtractData.Amount;
				}
				this.RefreshCurResource(type, true);
				this.RefreshCurResource(type, false);
				this.RefreshExchangeResource(type, true);
				this.RefreshExchangeResource(type, false);
			}
			this.RefreshResult();
		}
	}

	// Token: 0x060034C0 RID: 13504 RVA: 0x001A55D0 File Offset: 0x001A37D0
	private string GetFavorFillSpriteNameByFavorType(sbyte favorType)
	{
		if (!true)
		{
		}
		string result;
		switch (favorType)
		{
		case -6:
			result = "popup_exchange_progress_8";
			break;
		case -5:
			result = "popup_exchange_progress_8";
			break;
		case -4:
			result = "popup_exchange_progress_8";
			break;
		case -3:
			result = "popup_exchange_progress_7";
			break;
		case -2:
			result = "popup_exchange_progress_7";
			break;
		case -1:
			result = "popup_exchange_progress_7";
			break;
		case 0:
			result = "popup_exchange_progress_6";
			break;
		case 1:
			result = "popup_exchange_progress_5";
			break;
		case 2:
			result = "popup_exchange_progress_4";
			break;
		case 3:
			result = "popup_exchange_progress_3";
			break;
		case 4:
			result = "popup_exchange_progress_2";
			break;
		case 5:
			result = "popup_exchange_progress_1";
			break;
		case 6:
			result = "popup_exchange_progress_0";
			break;
		default:
			result = "popup_exchange_progress__6";
			break;
		}
		if (!true)
		{
		}
		return result;
	}

	// Token: 0x060034C1 RID: 13505 RVA: 0x001A569C File Offset: 0x001A389C
	private void Update()
	{
		bool flag = CommonCommandKit.Space.Check(this.Element, false, false, false, true, false) && EventSystem.current.currentSelectedGameObject == null && this._confirmBtn != null && this._confirmBtn.interactable;
		if (flag)
		{
			this.ConfirmClick();
		}
	}

	// Token: 0x060034C4 RID: 13508 RVA: 0x001A581C File Offset: 0x001A3A1C
	[CompilerGenerated]
	internal static void <OnClickButtonBalance>g__Calc|39_2(List<ItemDisplayData> list, ItemDisplayData[] operationArray, ref UI_ExchangeResource.<>c__DisplayClass39_0 A_2)
	{
		foreach (ItemDisplayData data in list)
		{
			sbyte type = ItemTemplateHelper.GetMiscResourceType(data.Key.ItemType, data.Key.TemplateId);
			long worth = Debts.ResourceAmountToWorth((short)type, data.Amount);
			int needAmount = Debts.WorthToResourceAmount((short)type, A_2.needWorth, false);
			ItemDisplayData tempData = operationArray[(int)type];
			bool flag = tempData == null;
			if (flag)
			{
				tempData = data.Clone(0);
				operationArray[(int)type] = tempData;
			}
			bool flag2 = worth >= A_2.needWorth;
			if (flag2)
			{
				A_2.needWorth -= A_2.needWorth;
				tempData.Amount = needAmount;
			}
			else
			{
				A_2.needWorth -= worth;
				tempData.Amount = data.Amount;
			}
			bool flag3 = A_2.needWorth <= 0L;
			if (flag3)
			{
				break;
			}
		}
	}

	// Token: 0x0400263E RID: 9790
	private int _taiwuCharId;

	// Token: 0x0400263F RID: 9791
	private int _npcCharId;

	// Token: 0x04002640 RID: 9792
	private ResourceInts _taiwuResources;

	// Token: 0x04002641 RID: 9793
	private ResourceInts _npcResources;

	// Token: 0x04002642 RID: 9794
	private CharacterFavorability _npcFavorabilityHandler;

	// Token: 0x04002643 RID: 9795
	private long _maxWorthCanBeLentToTaiwu;

	// Token: 0x04002644 RID: 9796
	private long _remainWorthCanBeLentToTaiwu;

	// Token: 0x04002645 RID: 9797
	private bool _worthLimited;

	// Token: 0x04002646 RID: 9798
	private ResourceInts _taiwuExchangeResources;

	// Token: 0x04002647 RID: 9799
	private ResourceInts _npcExchangeResources;

	// Token: 0x04002648 RID: 9800
	private CharacterSet _groupCharacterSet;

	// Token: 0x04002649 RID: 9801
	private CButtonObsolete _confirmBtn;

	// Token: 0x0400264A RID: 9802
	private readonly ItemDisplayData[] _taiwuCurResourceArray = new ItemDisplayData[7];

	// Token: 0x0400264B RID: 9803
	private readonly ItemDisplayData[] _npcCurResourceArray = new ItemDisplayData[7];

	// Token: 0x0400264C RID: 9804
	private readonly ItemDisplayData[] _taiwuExchangeResourceArray = new ItemDisplayData[7];

	// Token: 0x0400264D RID: 9805
	private readonly ItemDisplayData[] _npcExchangeResourceArray = new ItemDisplayData[7];

	// Token: 0x0400264E RID: 9806
	private readonly Refers[] _taiwuCurResourceItemArray = new Refers[7];

	// Token: 0x0400264F RID: 9807
	private readonly Refers[] _npcCurResourceItemArray = new Refers[7];

	// Token: 0x04002650 RID: 9808
	private readonly Refers[] _taiwuExchangeResourceItemArray = new Refers[7];

	// Token: 0x04002651 RID: 9809
	private readonly Refers[] _npcExchangeResourceItemArray = new Refers[7];
}
