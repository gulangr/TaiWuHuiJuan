using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using DG.Tweening;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Character;
using Game.Components.ListStyleGeneralScroll;
using Game.Components.ListStyleGeneralScroll.Item;
using Game.Components.SortAndFilter.Item;
using Game.Components.SortAndFilter.Item.Apply;
using Game.Views.CharacterMenu;
using Game.Views.Item;
using Game.Views.Select;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Information;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Taiwu;
using GameData.Domains.Taiwu.ExchangeSystem;
using GameDataExtensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.Exchange
{
	// Token: 0x02000A31 RID: 2609
	public abstract class ViewExchangeBase : UIBase
	{
		// Token: 0x17000DEC RID: 3564
		// (get) Token: 0x06007FF2 RID: 32754
		public abstract Exchange Exchange { get; }

		// Token: 0x17000DED RID: 3565
		// (get) Token: 0x06007FF3 RID: 32755
		public abstract SecretInformationDisplayPackage SecretInformationDisplayPackage { get; }

		// Token: 0x17000DEE RID: 3566
		// (get) Token: 0x06007FF4 RID: 32756
		public abstract CharacterDisplayData TargetDisplayData { get; }

		// Token: 0x17000DEF RID: 3567
		// (get) Token: 0x06007FF5 RID: 32757
		public abstract CharacterDisplayData TaiwuDisplayData { get; }

		// Token: 0x06007FF6 RID: 32758
		public abstract void Submit();

		// Token: 0x06007FF7 RID: 32759
		protected abstract void RequestData();

		// Token: 0x06007FF8 RID: 32760
		protected abstract void RefreshValues();

		// Token: 0x06007FF9 RID: 32761
		public abstract IReadOnlyList<ITradeableContent> GetTargetTradeableList(int index);

		// Token: 0x06007FFA RID: 32762
		public abstract IReadOnlyList<ITradeableContent> GetSelfTradeableList(int index);

		// Token: 0x17000DF0 RID: 3568
		// (get) Token: 0x06007FFB RID: 32763
		protected abstract string SortSaveKeyPrefix { get; }

		// Token: 0x17000DF1 RID: 3569
		// (get) Token: 0x06007FFC RID: 32764 RVA: 0x003B8F1D File Offset: 0x003B711D
		public virtual bool CanConfirmExchange
		{
			get
			{
				return this.Exchange.CanConfirmExchange;
			}
		}

		// Token: 0x06007FFD RID: 32765 RVA: 0x003B8F2A File Offset: 0x003B712A
		public virtual IReadOnlyList<ITradeableContent> GetTargetTradeableList()
		{
			CToggleGroup targetPage = this.exchangeContainer.targetPage;
			return this.GetTargetTradeableList((targetPage != null) ? targetPage.GetActiveIndex() : -1);
		}

		// Token: 0x06007FFE RID: 32766 RVA: 0x003B8F49 File Offset: 0x003B7149
		public virtual IReadOnlyList<ITradeableContent> GetSelfTradeableList()
		{
			CToggleGroup currPage = this.exchangeContainer.currPage;
			return this.GetSelfTradeableList((currPage != null) ? currPage.GetActiveIndex() : -1);
		}

		// Token: 0x06007FFF RID: 32767 RVA: 0x003B8F68 File Offset: 0x003B7168
		protected virtual void OnClickItem(ItemListScroll scroll, ITradeableContent itemData, RowItemLine rowItemLine, Action<RowItemLine, int> action)
		{
			Action <>9__3;
			scroll.HandleClickItem(itemData, rowItemLine, delegate(RowItemLine x)
			{
				bool flag = itemData.Amount == 1;
				if (flag)
				{
					TipType type = rowItemLine.TipDisplayer.Type;
					action(x, 1);
					this.Refresh();
					YieldHelper instance = SingletonObject.getInstance<YieldHelper>();
					uint frame = 1U;
					Action job;
					if ((job = <>9__3) == null)
					{
						job = (<>9__3 = delegate()
						{
							rowItemLine.TipDisplayer.Refresh(true, -1);
						});
					}
					instance.DelayFrameDo(frame, job);
				}
				else
				{
					scroll.SetItemToSelectCountMode(x, delegate(int count)
					{
						action(x, count);
						this.Refresh();
					}, delegate
					{
					}, 0, 0, 1, null, false, null, false);
				}
			});
		}

		// Token: 0x17000DF2 RID: 3570
		// (get) Token: 0x06008000 RID: 32768 RVA: 0x003B8FC4 File Offset: 0x003B71C4
		protected virtual ItemDisplayData.ItemUsingOperationType ItemUsingOperationType
		{
			get
			{
				return ItemDisplayData.ItemUsingOperationType.Default;
			}
		}

		// Token: 0x06008001 RID: 32769 RVA: 0x003B8FC8 File Offset: 0x003B71C8
		protected virtual void OnClickSelfItem(ITradeableContent itemData, RowItemLine rowItemLine)
		{
			ItemDisplayData displayData = itemData as ItemDisplayData;
			bool flag = displayData != null && displayData.UsingType != ItemDisplayData.ItemUsingType.Invalid;
			if (flag)
			{
				Action<RowItemLine, int> <>9__3;
				DialogCmd cmd = new DialogCmd
				{
					Title = LanguageKey.LK_Common_Attention.Tr(),
					Content = displayData.GetUsingOperationConfirmTip(this.ItemUsingOperationType),
					Type = 1,
					Yes = delegate()
					{
						ViewExchangeBase <>4__this = this;
						ItemListScroll selfItemList = this.exchangeContainer.selfItemList;
						ITradeableContent itemData2 = itemData;
						RowItemLine rowItemLine2 = rowItemLine;
						Action<RowItemLine, int> action;
						if ((action = <>9__3) == null)
						{
							action = (<>9__3 = delegate(RowItemLine _, int count)
							{
								this.Exchange.SelectTaiwuItem(itemData, count);
							});
						}
						<>4__this.OnClickItem(selfItemList, itemData2, rowItemLine2, action);
					},
					No = delegate()
					{
						rowItemLine.SetSelected(false);
					}
				};
				UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
				UIManager.Instance.MaskUI(UIElement.Dialog);
			}
			else
			{
				this.OnClickItem(this.exchangeContainer.selfItemList, itemData, rowItemLine, delegate(RowItemLine _, int count)
				{
					this.Exchange.SelectTaiwuItem(itemData, count);
				});
			}
		}

		// Token: 0x06008002 RID: 32770 RVA: 0x003B90C4 File Offset: 0x003B72C4
		protected virtual void OnClickTargetItem(ITradeableContent itemData, RowItemLine rowItemLine)
		{
			this.OnClickItem(this.exchangeContainer.targetItemList, itemData, rowItemLine, delegate(RowItemLine _, int count)
			{
				this.Exchange.SelectTargetItem(itemData, count);
			});
		}

		// Token: 0x06008003 RID: 32771 RVA: 0x003B910C File Offset: 0x003B730C
		protected virtual void OnClickSelfExchangedItem(ITradeableContent itemData, RowItemLine rowItemLine)
		{
			this.OnClickItem(this.exchangeContainer.selfExchangeList, itemData, rowItemLine, delegate(RowItemLine _, int count)
			{
				this.Exchange.CancelTaiwuItem(itemData, count);
			});
		}

		// Token: 0x06008004 RID: 32772 RVA: 0x003B9154 File Offset: 0x003B7354
		protected virtual void OnClickTargetExchangedItem(ITradeableContent itemData, RowItemLine rowItemLine)
		{
			this.OnClickItem(this.exchangeContainer.targetExchangeList, itemData, rowItemLine, delegate(RowItemLine _, int count)
			{
				this.Exchange.CancelTargetItem(itemData, count);
			});
		}

		// Token: 0x06008005 RID: 32773 RVA: 0x003B919A File Offset: 0x003B739A
		public virtual void Reset()
		{
			this.Exchange.ExchangeItemList.Clear();
			this.RequestData();
		}

		// Token: 0x06008006 RID: 32774 RVA: 0x003B91B8 File Offset: 0x003B73B8
		public virtual void ResetSelf()
		{
			foreach (ITradeableContent item in this.Exchange.TaiwuContentList)
			{
				this.Exchange.CancelTaiwuItem(item, item.Amount);
			}
			this.Refresh();
		}

		// Token: 0x06008007 RID: 32775 RVA: 0x003B9228 File Offset: 0x003B7428
		public virtual void ResetTarget()
		{
			foreach (ITradeableContent item in this.Exchange.TargetContentList)
			{
				this.Exchange.CancelTargetItem(item, item.Amount);
			}
			this.Refresh();
		}

		// Token: 0x06008008 RID: 32776 RVA: 0x003B9298 File Offset: 0x003B7498
		protected virtual IEnumerable<ITradeableContent> PutAllFilter(ItemListScroll scroll, IEnumerable<ITradeableContent> itemList, IEnumerable<ITradeableContent> contentList, CharacterDisplayData displayData)
		{
			Func<ITradeableContent, bool> filter = scroll.SortAndFilterController.GenerateFilter();
			return this.Filter(itemList, contentList, displayData).Where(delegate(ITradeableContent item)
			{
				string text;
				return filter(item) && !this.CheckItemIsLocked(item, displayData, out text) && item.ItemSourceType != 0 && item.UsingType == ItemDisplayData.ItemUsingType.Invalid;
			});
		}

		// Token: 0x06008009 RID: 32777 RVA: 0x003B92F0 File Offset: 0x003B74F0
		protected virtual void PutSelfAll()
		{
			foreach (ITradeableContent item in this.PutAllFilter(this.exchangeContainer.selfItemList, this.GetSelfTradeableList(), this.Exchange.TaiwuContentList, this.TaiwuDisplayData))
			{
				this.Exchange.SelectTaiwuItem(item, item.Amount);
			}
			this.Refresh();
		}

		// Token: 0x0600800A RID: 32778 RVA: 0x003B9378 File Offset: 0x003B7578
		protected virtual void PutTargetAll()
		{
			foreach (ITradeableContent item in this.PutAllFilter(this.exchangeContainer.targetItemList, this.GetTargetTradeableList(), this.Exchange.TargetContentList, this.TargetDisplayData))
			{
				this.Exchange.SelectTargetItem(item, item.Amount);
			}
			this.Refresh();
		}

		// Token: 0x0600800B RID: 32779 RVA: 0x003B9400 File Offset: 0x003B7600
		protected virtual void SelfMultiplyOperate()
		{
			int activeIndex = this.exchangeContainer.currPage.GetActiveIndex();
			if (!true)
			{
			}
			EItemSourceToggleKey eitemSourceToggleKey;
			switch (activeIndex)
			{
			case 0:
				eitemSourceToggleKey = EItemSourceToggleKey.Inventory;
				break;
			case 1:
				eitemSourceToggleKey = EItemSourceToggleKey.Warehouse;
				break;
			case 2:
				eitemSourceToggleKey = EItemSourceToggleKey.Treasury;
				break;
			case 3:
				eitemSourceToggleKey = EItemSourceToggleKey.Stock;
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			if (!true)
			{
			}
			EItemSourceToggleKey initTogKey = eitemSourceToggleKey;
			ArgumentBox args = EasyPool.Get<ArgumentBox>().SetObject("InitTogKey", initTogKey);
			UIElement.ItemMultiplyOperation.SetOnInitArgs(args);
			UIManager.Instance.ShowUI(UIElement.ItemMultiplyOperation, true);
		}

		// Token: 0x0600800C RID: 32780 RVA: 0x003B948C File Offset: 0x003B768C
		protected virtual void TargetMultiplyOperate()
		{
			int activeIndex = this.exchangeContainer.targetPage.GetActiveIndex();
			if (!true)
			{
			}
			EItemSourceToggleKey eitemSourceToggleKey;
			switch (activeIndex)
			{
			case 0:
				eitemSourceToggleKey = EItemSourceToggleKey.Inventory;
				break;
			case 1:
				eitemSourceToggleKey = EItemSourceToggleKey.Warehouse;
				break;
			case 2:
				eitemSourceToggleKey = EItemSourceToggleKey.Treasury;
				break;
			case 3:
				eitemSourceToggleKey = EItemSourceToggleKey.Stock;
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			if (!true)
			{
			}
			EItemSourceToggleKey initTogKey = eitemSourceToggleKey;
			ArgumentBox args = EasyPool.Get<ArgumentBox>().SetObject("InitTogKey", initTogKey);
			UIElement.ItemMultiplyOperation.SetOnInitArgs(args);
			UIManager.Instance.ShowUI(UIElement.ItemMultiplyOperation, true);
		}

		// Token: 0x0600800D RID: 32781 RVA: 0x003B9518 File Offset: 0x003B7718
		public virtual void Balance()
		{
			this.Exchange.Balance((from d in this.exchangeContainer.selfItemList.FilteredData
			where d.IsResource
			select (ItemDisplayData)d).ToList<ItemDisplayData>());
			this.Refresh();
		}

		// Token: 0x0600800E RID: 32782 RVA: 0x003B9598 File Offset: 0x003B7798
		public void ClearTaiwuItems()
		{
			foreach (ITradeableContent content in this.Exchange.TaiwuContentList)
			{
				this.Exchange.ChangeItem(content, Math.Abs(content.Amount));
			}
			this.Refresh();
		}

		// Token: 0x0600800F RID: 32783 RVA: 0x003B960C File Offset: 0x003B780C
		public void ClearTargetItems()
		{
			foreach (ITradeableContent content in this.Exchange.TargetContentList)
			{
				this.Exchange.ChangeItem(content, -Math.Abs(content.Amount));
			}
			this.Refresh();
		}

		// Token: 0x06008010 RID: 32784 RVA: 0x003B9680 File Offset: 0x003B7880
		public virtual void SetDebt()
		{
			int init = this.Exchange.AdvantageSummary.DebtUsed;
			UIElement setSelectCount = UIElement.SetSelectCount;
			ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>().Set("MinCount", 0).Set("MaxCount", this.Exchange.AdvantageSummary.DebtMax).Set("InitCount", init).Set("LimitCount", this.Exchange.AdvantageSummary.DebtMax).Set("LimitTip", string.Empty).Set("ChangeValue", GlobalConfig.Instance.ExchangeDebtUnit).Set("NoCancelOnHide", true).SetObject("FollowOffset", this.exchangeContainer.debtFollowOffset).SetObject("OnValueChanged", new Action<int>(this.SetDebtImpl)).SetObject("OnCancelSetCount", new Action(delegate
			{
			}));
			string key = "ItemRectTrans";
			CButton debt = this.exchangeContainer.debt;
			setSelectCount.SetOnInitArgs(argumentBox.SetObject(key, ((debt != null) ? debt.transform : null) as RectTransform).Set("ZeroValid", true));
			UIManager.Instance.ShowUI(UIElement.SetSelectCount, true);
		}

		// Token: 0x06008011 RID: 32785 RVA: 0x003B97C3 File Offset: 0x003B79C3
		public virtual void SetDebtImpl(int value)
		{
			this.Exchange.SetDebtUsed(value);
			this.Refresh();
		}

		// Token: 0x06008012 RID: 32786 RVA: 0x003B97DC File Offset: 0x003B79DC
		public virtual void AddSecretOrPopSelections()
		{
			bool flag = this.Exchange.AdvantageSummary.SecretId == -1;
			if (flag)
			{
				this.<AddSecretOrPopSelections>g__ShowSelectSecret|43_1();
			}
			else
			{
				List<ViewPopupMenu.BtnData> btnList = new List<ViewPopupMenu.BtnData>
				{
					new ViewPopupMenu.BtnData(LanguageKey.LK_Exchange_Secret_Remove.Tr(), true, EItemMenuDisplayOrder.Discard, delegate()
					{
						this.<AddSecretOrPopSelections>g__SetSecretImpl|43_2(null);
						ViewExchangeBase.<AddSecretOrPopSelections>g__OnCancel|43_0();
					}, null, null, false),
					new ViewPopupMenu.BtnData(LanguageKey.LK_Exchange_Secret_Relpace.Tr(), true, EItemMenuDisplayOrder.Info, new Action(this.<AddSecretOrPopSelections>g__ShowSelectSecret|43_1), null, null, false)
				};
				RectTransform itemRectTrans = this.exchangeContainer.secret.transform as RectTransform;
				Vector3 itemScreenPos = UIManager.Instance.UiCamera.WorldToScreenPoint(itemRectTrans.position);
				Vector3 mouseScreenPos = Input.mousePosition;
				itemScreenPos.x = mouseScreenPos.x;
				UIElement.PopupMenu.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("BtnInfo", btnList).SetObject("ScreenPos", itemScreenPos).SetObject("ItemSize", itemRectTrans.rect.size).SetObject("OnCancel", new Action(ViewExchangeBase.<AddSecretOrPopSelections>g__OnCancel|43_0)));
				UIManager.Instance.ShowUI(UIElement.PopupMenu, true);
			}
		}

		// Token: 0x06008013 RID: 32787 RVA: 0x003B9918 File Offset: 0x003B7B18
		public void RefreshSecret()
		{
			bool flag = !this.exchangeContainer.secretImg;
			if (!flag)
			{
				Image secretImg = this.exchangeContainer.secretImg;
				ExchangeAdvantage advantageSummary = this.Exchange.AdvantageSummary;
				Sprite sprite;
				if (!(((advantageSummary != null) ? new int?(advantageSummary.SecretId) : null) >= 0))
				{
					ExchangeAdvantage advantageSummary2 = this.Exchange.AdvantageSummary;
					if (!(((advantageSummary2 != null) ? new int?(advantageSummary2.ApprovingCharId) : null) >= 0))
					{
						sprite = this.exchangeContainer.secretIsNone;
						goto IL_B0;
					}
				}
				sprite = this.exchangeContainer.secretIsSome;
				IL_B0:
				secretImg.sprite = sprite;
			}
		}

		// Token: 0x06008014 RID: 32788 RVA: 0x003B99DC File Offset: 0x003B7BDC
		protected virtual bool CheckItemCanTransfer(ITradeableContent itemData, CharacterDisplayData cd)
		{
			bool isDetachable = itemData.UsingType != ItemDisplayData.ItemUsingType.Equiped || ItemTemplateHelper.IsDetachable(itemData.Key.ItemType, itemData.Key.TemplateId);
			bool isTransferable = itemData.CharacterId != -1 || (ItemTemplateHelper.IsTransferable(itemData.Key.ItemType, itemData.Key.TemplateId) && isDetachable);
			bool miscResourceCanExchange = ItemTemplateHelper.MiscResourceCanExchange(itemData.Key.ItemType, itemData.Key.TemplateId);
			bool canTransfer = isTransferable || miscResourceCanExchange;
			return canTransfer && !ItemDisplayDataHelper.IsItemLockedByTask(itemData) && !itemData.IsLocked;
		}

		// Token: 0x06008015 RID: 32789 RVA: 0x003B9A7C File Offset: 0x003B7C7C
		protected virtual bool CheckItemIsLocked(ITradeableContent itemData, CharacterDisplayData cd, out string reason)
		{
			reason = "";
			bool isBaby = cd != null && AgeGroup.GetAgeGroup(cd.PhysiologicalAge) == 0;
			bool flag = isBaby;
			return flag || !this.CheckItemCanTransfer(itemData, cd);
		}

		// Token: 0x06008016 RID: 32790 RVA: 0x003B9AC0 File Offset: 0x003B7CC0
		protected virtual void SetItemLockedStatus(ITradeableContent itemData, CharacterDisplayData cd, RowItemLine rowItemLine, RowItemMain main)
		{
			bool isLocked = itemData.IsLocked;
			if (isLocked)
			{
				rowItemLine.SetInteractable(false, true);
				rowItemLine.SetDisabled(false);
			}
			else
			{
				string reason;
				bool interactable = this.CheckItemIsLocked(itemData, cd, out reason);
				rowItemLine.SetInteractable(!interactable, true);
				rowItemLine.SetDisabled(interactable);
				bool flag = string.IsNullOrWhiteSpace(reason);
				if (flag)
				{
					main.HideInteractionState();
				}
				else
				{
					main.SetInteractionStateLockText(reason);
				}
			}
		}

		// Token: 0x06008017 RID: 32791 RVA: 0x003B9B28 File Offset: 0x003B7D28
		protected virtual void OnTargetItemRender(ITradeableContent itemData, RowItemLine rowItemLine)
		{
			this.OnItemRender(itemData, rowItemLine, this.TargetDisplayData);
		}

		// Token: 0x06008018 RID: 32792 RVA: 0x003B9B39 File Offset: 0x003B7D39
		protected virtual void OnSelfItemRender(ITradeableContent itemData, RowItemLine rowItemLine)
		{
			this.OnItemRender(itemData, rowItemLine, this.TaiwuDisplayData);
		}

		// Token: 0x06008019 RID: 32793 RVA: 0x003B9B4A File Offset: 0x003B7D4A
		protected virtual void OnExchangingSelfItemRender(ITradeableContent itemData, RowItemLine rowItemLine)
		{
			this.OnItemRender(itemData, rowItemLine, this.TaiwuDisplayData);
		}

		// Token: 0x0600801A RID: 32794 RVA: 0x003B9B5B File Offset: 0x003B7D5B
		protected virtual void OnExchangingTargetItemRender(ITradeableContent itemData, RowItemLine rowItemLine)
		{
			this.OnItemRender(itemData, rowItemLine, this.TargetDisplayData);
		}

		// Token: 0x0600801B RID: 32795 RVA: 0x003B9B6C File Offset: 0x003B7D6C
		protected virtual RowItemMain OnItemRender(ITradeableContent itemData, RowItemLine rowItemLine, CharacterDisplayData displayData)
		{
			RowItemMain rowItemMain = rowItemLine.GetComponentInChildren<RowItemMain>();
			rowItemMain.SetData(itemData);
			rowItemLine.Set(rowItemMain, true);
			this.SetItemLockedStatus(itemData, displayData, rowItemLine, rowItemMain);
			rowItemLine.SetSelected(false);
			bool flag = !itemData.IsResource;
			if (flag)
			{
				RowItemLine.SetMouseTipDisplayer(true, itemData, rowItemLine.TipDisplayer);
			}
			else
			{
				bool flag2 = displayData != null;
				if (flag2)
				{
					bool isTaiwu = displayData.CharacterId == SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
					RowItemLine.SetResourceTip(itemData, rowItemLine.TipDisplayer, NameCenter.GetMonasticTitleOrDisplayName(displayData, isTaiwu), true, isTaiwu);
				}
			}
			rowItemMain.SetCoreData(this.ExchangeValueType, (int)itemData.Value);
			return rowItemMain;
		}

		// Token: 0x0600801C RID: 32796 RVA: 0x003B9C10 File Offset: 0x003B7E10
		protected virtual void AwakeBack()
		{
			this.exchangeContainer.exchangeBack.SetInvalid();
			this.exchangeContainer.exchangeFront.SetInvalid();
		}

		// Token: 0x17000DF3 RID: 3571
		// (get) Token: 0x0600801D RID: 32797 RVA: 0x003B9C35 File Offset: 0x003B7E35
		protected virtual ESortAndFilterControllerType ControllerType
		{
			get
			{
				return ESortAndFilterControllerType.Item;
			}
		}

		// Token: 0x17000DF4 RID: 3572
		// (get) Token: 0x0600801E RID: 32798 RVA: 0x003B9C38 File Offset: 0x003B7E38
		protected virtual ItemListScroll.EColumnType ColumnType
		{
			get
			{
				return ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Amount | ItemListScroll.EColumnType.Type | ItemListScroll.EColumnType.Weight | ItemListScroll.EColumnType.Value | ItemListScroll.EColumnType.Durability;
			}
		}

		// Token: 0x0600801F RID: 32799 RVA: 0x003B9C40 File Offset: 0x003B7E40
		protected virtual void AwakeScroll()
		{
			this.exchangeContainer.selfItemList.SetTableHeadSortEnabled(false);
			this.exchangeContainer.targetItemList.SetTableHeadSortEnabled(false);
			this.exchangeContainer.selfItemList.Init(this.SortSaveKeyPrefix + "Self", this.ControllerType, true, new Action<ITradeableContent, RowItemLine>(this.OnSelfItemRender), new Action<ITradeableContent, RowItemLine>(this.OnClickSelfItem), this.ColumnType, null, null, null);
			this.exchangeContainer.selfExchangeList.Init(this.SortSaveKeyPrefix + "Tmp", this.ControllerType, true, new Action<ITradeableContent, RowItemLine>(this.OnExchangingSelfItemRender), new Action<ITradeableContent, RowItemLine>(this.OnClickSelfExchangedItem), this.ColumnType, null, null, null);
			this.exchangeContainer.targetItemList.Init(this.SortSaveKeyPrefix + "Target", this.ControllerType, true, new Action<ITradeableContent, RowItemLine>(this.OnTargetItemRender), new Action<ITradeableContent, RowItemLine>(this.OnClickTargetItem), this.ColumnType, null, null, null);
			this.exchangeContainer.targetExchangeList.Init(this.SortSaveKeyPrefix + "TargetTmp", this.ControllerType, true, new Action<ITradeableContent, RowItemLine>(this.OnExchangingTargetItemRender), new Action<ITradeableContent, RowItemLine>(this.OnClickTargetExchangedItem), this.ColumnType, null, null, null);
			this.exchangeContainer.targetItemList.OnSortAndFilterChangedCallback = new Action(this.OnTargetSortAndFilterChangedCallback);
			this.exchangeContainer.selfItemList.OnSortAndFilterChangedCallback = new Action(this.OnSelfSortAndFilterChangedCallback);
			this.exchangeContainer.selfItemList.SetItemList(new List<ITradeableContent>());
			this.exchangeContainer.selfExchangeList.SetItemList(new List<ITradeableContent>());
			this.exchangeContainer.targetItemList.SetItemList(new List<ITradeableContent>());
			this.exchangeContainer.targetExchangeList.SetItemList(new List<ITradeableContent>());
		}

		// Token: 0x06008020 RID: 32800 RVA: 0x003B9E30 File Offset: 0x003B8030
		protected virtual void AwakePager()
		{
			bool flag = this.exchangeContainer.currPage;
			if (flag)
			{
				this.exchangeContainer.currPage.Init(-1);
				this.exchangeContainer.currPage.OnActiveIndexChange += delegate(int newTog, int _)
				{
					this.exchangeContainer.selfItemList.InfiniteScroll.Refresh(0);
					this.exchangeContainer.SetSelfFilterEnabled(newTog != 5);
					this.RefreshSelfItems(newTog);
					this.RefreshSelfColumnDefinitions(newTog);
					this.RefreshPutButtons();
					this.RefreshValues();
					this.exchangeContainer.selfItemList.InfiniteScroll.ScrollTo(0, 0.3f);
				};
			}
			bool flag2 = this.exchangeContainer.targetPage;
			if (flag2)
			{
				this.exchangeContainer.targetPage.Init(-1);
				this.exchangeContainer.targetPage.OnActiveIndexChange += delegate(int newTog, int _)
				{
					this.exchangeContainer.targetItemList.InfiniteScroll.Refresh(0);
					this.RefreshTargetItems(newTog);
					this.RefreshTargetColumnDefinitions(newTog);
					this.RefreshPutButtons();
					this.exchangeContainer.targetItemList.InfiniteScroll.ScrollTo(0, 0.3f);
				};
			}
		}

		// Token: 0x06008021 RID: 32801 RVA: 0x003B9EC8 File Offset: 0x003B80C8
		public virtual void SetTitle(TMP_Text title, int newTog)
		{
			switch (newTog)
			{
			case 0:
				title.text = LanguageKey.LK_SelectItem_Group_Inventory.Tr();
				break;
			case 1:
				title.text = LanguageKey.LK_SelectItem_Group_Warehouse.Tr();
				break;
			case 2:
				title.text = LanguageKey.LK_SelectItem_Group_Treasury.Tr();
				break;
			case 3:
				title.text = LanguageKey.LK_Exchange_Stock.Tr();
				break;
			}
		}

		// Token: 0x06008022 RID: 32802 RVA: 0x003B9F40 File Offset: 0x003B8140
		protected virtual void AwakeButtons()
		{
			bool flag = this.exchangeContainer.confirm;
			if (flag)
			{
				this.exchangeContainer.confirm.onClick.ResetListener(new Action(this.Submit));
			}
			bool flag2 = this.exchangeContainer.reset;
			if (flag2)
			{
				this.exchangeContainer.reset.onClick.ResetListener(new Action(this.Reset));
			}
			bool flag3 = this.exchangeContainer.resetSelf;
			if (flag3)
			{
				this.exchangeContainer.resetSelf.onClick.ResetListener(new Action(this.ResetSelf));
			}
			bool flag4 = this.exchangeContainer.resetTarget;
			if (flag4)
			{
				this.exchangeContainer.resetTarget.onClick.ResetListener(new Action(this.ResetTarget));
			}
			bool flag5 = this.exchangeContainer.balance;
			if (flag5)
			{
				this.exchangeContainer.balance.onClick.ResetListener(new Action(this.Balance));
			}
			bool flag6 = this.exchangeContainer.secret;
			if (flag6)
			{
				this.exchangeContainer.secret.onClick.ResetListener(new Action(this.AddSecretOrPopSelections));
			}
			bool flag7 = this.exchangeContainer.putSelfAll;
			if (flag7)
			{
				this.exchangeContainer.putSelfAll.onClick.ResetListener(new Action(this.PutSelfAll));
			}
			bool flag8 = this.exchangeContainer.putTargetAll;
			if (flag8)
			{
				this.exchangeContainer.putTargetAll.onClick.ResetListener(new Action(this.PutTargetAll));
			}
			bool flag9 = this.exchangeContainer.selfMultiplyOperate;
			if (flag9)
			{
				this.exchangeContainer.selfMultiplyOperate.onClick.ResetListener(new Action(this.SelfMultiplyOperate));
			}
			bool flag10 = this.exchangeContainer.targetMultiplyOperate;
			if (flag10)
			{
				this.exchangeContainer.targetMultiplyOperate.onClick.ResetListener(new Action(this.TargetMultiplyOperate));
			}
			bool flag11 = this.exchangeContainer.hide;
			if (flag11)
			{
				this.exchangeContainer.hide.onClick.ResetListener(new Action(this.QuickHide));
			}
			bool flag12 = this.exchangeContainer.debt;
			if (flag12)
			{
				this.exchangeContainer.debt.onClick.ResetListener(new Action(this.SetDebt));
			}
		}

		// Token: 0x06008023 RID: 32803 RVA: 0x003BA1F2 File Offset: 0x003B83F2
		protected virtual void Awake()
		{
			this.AwakeBack();
			this.AwakeScroll();
			this.AwakePager();
			this.AwakeButtons();
			this.AwakeAvatarBtn();
		}

		// Token: 0x06008024 RID: 32804 RVA: 0x003BA218 File Offset: 0x003B8418
		protected virtual void AwakeAvatarBtn()
		{
			bool flag = this.exchangeContainer.avatarBtn;
			if (flag)
			{
				this.exchangeContainer.avatarBtn.interactable = false;
			}
		}

		// Token: 0x06008025 RID: 32805 RVA: 0x003BA24C File Offset: 0x003B844C
		protected void ShowCharacterMenu(CharacterDisplayData data)
		{
			this.ShowCharacterMenu((data != null) ? data.CharacterId : -1);
		}

		// Token: 0x06008026 RID: 32806 RVA: 0x003BA264 File Offset: 0x003B8464
		protected void ShowCharacterMenu(int id)
		{
			bool flag = id == -1;
			if (!flag)
			{
				UIElement.CharacterMenu.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("CharacterId", id).Set("PreviousView", 8));
				UIManager.Instance.ShowUI(UIElement.CharacterMenu, true);
			}
		}

		// Token: 0x06008027 RID: 32807 RVA: 0x003BA2B4 File Offset: 0x003B84B4
		protected List<ITradeableContent> Filter(IEnumerable<ITradeableContent> orig, IEnumerable<ITradeableContent> contentList, CharacterDisplayData displayData)
		{
			Dictionary<ValueTuple<ItemKey, int, sbyte>, ITradeableContent> dict = contentList.ToDictionary((ITradeableContent x) => new ValueTuple<ItemKey, int, sbyte>(x.RealKey, x.CharacterId, x.ItemSourceType), (ITradeableContent x) => x);
			return (((orig != null) ? orig.Select(delegate(ITradeableContent x)
			{
				ITradeableContent exchanged;
				ITradeableContent result;
				if (!dict.TryGetValue(new ValueTuple<ItemKey, int, sbyte>(x.RealKey, x.CharacterId, x.ItemSourceType), out exchanged))
				{
					result = x;
				}
				else
				{
					int y = x.Amount - exchanged.Amount;
					result = ((y > 0) ? x.Clone(y) : null);
				}
				return result;
			}).Where(delegate(ITradeableContent item)
			{
				string text;
				return item != null && (this.ShowNonInteractableGoods || !this.CheckItemIsLocked(item, displayData, out text));
			}) : null) ?? Enumerable.Empty<ITradeableContent>()).ToList<ITradeableContent>();
		}

		// Token: 0x06008028 RID: 32808 RVA: 0x003BA35C File Offset: 0x003B855C
		public override void OnInit(ArgumentBox argsBox)
		{
			this.ExtraScrollToTopAction = 3;
			Sequence bubbleAnim = this.BubbleAnim;
			if (bubbleAnim != null)
			{
				bubbleAnim.Kill(false);
			}
			bool flag = this.exchangeContainer.bubble;
			if (flag)
			{
				this.exchangeContainer.bubble.gameObject.SetActive(false);
			}
			if (argsBox != null)
			{
				argsBox.Get("CharacterId", out this.TargetId);
			}
			bool flag2 = this.exchangeContainer.confirm;
			if (flag2)
			{
				this.exchangeContainer.confirm.interactable = false;
			}
			bool taiwuFunctionAvailable = SingletonObject.getInstance<FunctionLockManager>().IsFunctionUnlock(10);
			this.exchangeContainer.currPage.Get(0).gameObject.SetActive(true);
			this.exchangeContainer.currPage.Get(1).gameObject.SetActive(true);
			this.exchangeContainer.currPage.Get(2).gameObject.SetActive(taiwuFunctionAvailable);
			this.exchangeContainer.currPage.Get(3).gameObject.SetActive(taiwuFunctionAvailable);
			this.exchangeContainer.currPage.Get(4).gameObject.SetActive(false);
			this.RefreshTargetColumnDefinitions(-1);
			this.NeedDataListenerId = true;
			UIElement element = this.Element;
			element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(this.RequestData));
			this.ResetDialog();
		}

		// Token: 0x06008029 RID: 32809 RVA: 0x003BA4C8 File Offset: 0x003B86C8
		public virtual void ShowBubble(string sayingThings, BubbleLevel level = BubbleLevel.Middle, float duration = 5f, float showingAnimDuration = 1f)
		{
			bool flag = level < this._currentLevel;
			if (!flag)
			{
				Sequence bubbleAnim = this.BubbleAnim;
				if (bubbleAnim != null)
				{
					bubbleAnim.Kill(false);
				}
				this.exchangeContainer.bubbleText.text = sayingThings;
				this.exchangeContainer.bubble.alpha = 0f;
				this.exchangeContainer.bubble.gameObject.SetActive(true);
				this.BubbleAnim = DOTween.Sequence().Append(this.exchangeContainer.bubble.DOFade(1f, showingAnimDuration)).AppendInterval(duration).Append(this.exchangeContainer.bubble.DOFade(0f, showingAnimDuration)).OnComplete(delegate
				{
					this._currentLevel = BubbleLevel.Low;
				}).Play<Sequence>();
			}
		}

		// Token: 0x0600802A RID: 32810 RVA: 0x003BA599 File Offset: 0x003B8799
		protected virtual void ResetDialog()
		{
			this._currentLevel = BubbleLevel.Low;
		}

		// Token: 0x0600802B RID: 32811 RVA: 0x003BA5A3 File Offset: 0x003B87A3
		public virtual void PrepareCustomRowTemplateContainersForCharacter(RowItem rowTemplate)
		{
			this.PrepareCustomRowTemplateContainers(rowTemplate, false);
		}

		// Token: 0x0600802C RID: 32812 RVA: 0x003BA5B0 File Offset: 0x003B87B0
		public virtual void PrepareCustomRowTemplateContainers(RowItem rowTemplate, bool isItemMode)
		{
			Transform containerRoot = rowTemplate.ContainerRoot;
			Object.Instantiate<RowCellContainer>(this.exchangeContainer.itemIconAndNameCellContainer, containerRoot).gameObject.SetActive(true);
			Object.Instantiate<RowCellContainer>(this.exchangeContainer.singleTextCellContainer, containerRoot).gameObject.SetActive(true);
			Object.Instantiate<RowCellContainer>(this.exchangeContainer.singleTextCellContainer, containerRoot).gameObject.SetActive(true);
			if (isItemMode)
			{
				Object.Instantiate<RowCellContainer>(this.exchangeContainer.singleTextCellContainer, containerRoot).gameObject.SetActive(true);
			}
			Object.Instantiate<RowCellContainer>(this.exchangeContainer.singleTextCellContainer, containerRoot).gameObject.SetActive(true);
		}

		// Token: 0x0600802D RID: 32813 RVA: 0x003BA65B File Offset: 0x003B885B
		public virtual IEnumerable<ColumnDefinition> GenerateColumnDefinitions(ItemListScroll scroll, bool isItemMode)
		{
			LayoutOption option = default(LayoutOption);
			ColumnDefinition<ITradeableContent, ITradeableContent> colName = scroll.ColumnIconAndName(option, false);
			bool flag = !isItemMode;
			if (flag)
			{
				colName.TableHeadLabel = (() => LanguageKey.LK_Char_Name.Tr());
			}
			yield return colName;
			LayoutOption numOption = new LayoutOption(20f, 1f, 122f, 1);
			if (isItemMode)
			{
				yield return scroll.ColumnAmount(numOption, true);
				yield return scroll.ColumnSubType(numOption, true);
				yield return scroll.ColumnWeight(numOption, true);
			}
			else
			{
				yield return scroll.ColumnOrganizationName(option, false);
				yield return scroll.ColumnCharacterGrade(option, false);
			}
			yield return scroll.ColumnValue(numOption, true, LanguageKey.LK_ItemValue);
			yield break;
		}

		// Token: 0x0600802E RID: 32814 RVA: 0x003BA67C File Offset: 0x003B887C
		protected virtual void SetTargetCharacterDisplayData(CharacterDisplayData displayData, LanguageKey targetTitle = LanguageKey.Invalid)
		{
			bool flag = this.exchangeContainer.btnOpenCharMenu;
			if (flag)
			{
				this.exchangeContainer.btnOpenCharMenu.gameObject.SetActive(displayData != null && displayData.CreatingType == 1);
			}
			bool flag2 = this.exchangeContainer.grade;
			if (flag2)
			{
				this.exchangeContainer.grade.Set(CommonUtils.GetOrganizationGradeString(displayData.OrgInfo, displayData.Gender, displayData.PhysiologicalAge, -1), (int)displayData.OrgInfo.Grade);
			}
			sbyte? b = (displayData != null) ? new sbyte?(displayData.BehaviorType) : null;
			sbyte type;
			bool flag3;
			if (b != null)
			{
				type = b.GetValueOrDefault();
				if (type >= 0)
				{
					flag3 = (type < 5);
					goto IL_BA;
				}
			}
			flag3 = false;
			IL_BA:
			bool flag4 = flag3;
			if (flag4)
			{
				Game.Components.Character.BehaviorType behavior = this.exchangeContainer.behavior;
				if (behavior != null)
				{
					behavior.Set(type);
				}
			}
			bool flag5 = this.exchangeContainer.favor;
			if (flag5)
			{
				this.exchangeContainer.favor.Set(displayData, true);
			}
			bool flag6 = this.exchangeContainer.alertness;
			if (flag6)
			{
				this.exchangeContainer.alertness.Set(displayData);
			}
			this.exchangeContainer.nameTarget.text = NameCenter.GetMonasticTitleOrDisplayName(displayData, false);
			bool flag7 = targetTitle != LanguageKey.Invalid;
			if (flag7)
			{
				this.exchangeContainer.titleTarget.text = targetTitle.TrFormat(NameCenter.GetMonasticTitleOrDisplayName(displayData, false));
			}
			this.exchangeContainer.avatar.Refresh(displayData, true);
			this.exchangeContainer.avatar.gameObject.SetActive(true);
		}

		// Token: 0x0600802F RID: 32815 RVA: 0x003BA820 File Offset: 0x003B8A20
		public virtual void SetExchangeData()
		{
			this.exchangeContainer.targetItemList.Exchange = (this.exchangeContainer.selfItemList.Exchange = (this.exchangeContainer.targetExchangeList.Exchange = (this.exchangeContainer.selfExchangeList.Exchange = this.Exchange)));
		}

		// Token: 0x06008030 RID: 32816 RVA: 0x003BA87C File Offset: 0x003B8A7C
		public virtual void SetWarehouseCanInteract(bool canInteract)
		{
			this.exchangeContainer.SetCurrPageInteractable(canInteract, 1);
			this.exchangeContainer.SetCurrPageInteractable(canInteract, 2);
			this.exchangeContainer.SetCurrPageInteractable(canInteract, 3);
			bool flag;
			if (!canInteract)
			{
				int activeIndex = this.exchangeContainer.currPage.GetActiveIndex();
				flag = (activeIndex == 1 || activeIndex == 2);
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			if (flag2)
			{
				this.exchangeContainer.currPage.DeSelectWithoutNotify();
				this.exchangeContainer.currPage.Set(0, false);
			}
		}

		// Token: 0x06008031 RID: 32817 RVA: 0x003BA906 File Offset: 0x003B8B06
		protected virtual void Refresh()
		{
			this.RefreshAdvantageSummary();
			this.RefreshSecret();
			this.RefreshSelfItems();
			this.RefreshTargetItems();
			this.RefreshButtons();
			this.RefreshValues();
			this.RefreshScroll();
		}

		// Token: 0x06008032 RID: 32818 RVA: 0x003BA93C File Offset: 0x003B8B3C
		protected virtual void RefreshAdvantageSummary()
		{
			bool enabled = this.Exchange.AdvantageSummary.Enabled;
			if (enabled)
			{
				bool flag = this.exchangeContainer.debt;
				if (flag)
				{
					this.exchangeContainer.debt.gameObject.SetActive(true);
					this.RefreshDebt();
				}
				this.exchangeContainer.exchangeTaskPanel.Set(this.Exchange.AdvantageSummary);
				this.exchangeContainer.exchangeAdvantageBar.Set(this.Exchange.TotalValueWithAdvantage, (long)GlobalConfig.Instance.ExchangeBarRangeP1);
			}
			else
			{
				bool flag2 = this.exchangeContainer.debt;
				if (flag2)
				{
					this.exchangeContainer.debt.gameObject.SetActive(false);
				}
				this.exchangeContainer.exchangeTaskPanel.gameObject.SetActive(false);
				this.exchangeContainer.exchangeAdvantageBar.gameObject.SetActive(false);
			}
		}

		// Token: 0x06008033 RID: 32819 RVA: 0x003BAA34 File Offset: 0x003B8C34
		protected virtual void RefreshDebt()
		{
			bool flag = this.exchangeContainer.debtIsNegative;
			if (flag)
			{
				this.exchangeContainer.debtIsNegative.SetStyleEffect(this.Exchange.AdvantageSummary.DebtMax <= 0, false);
			}
			bool flag2 = this.exchangeContainer.debtIsNegativeTips;
			if (flag2)
			{
				this.exchangeContainer.debtIsNegativeTips.enabled = (this.Exchange.AdvantageSummary.DebtMax <= 0);
			}
			bool flag3 = this.exchangeContainer.debtText;
			if (flag3)
			{
				this.exchangeContainer.debtText.text = ((this.Exchange.AdvantageSummary.DebtUsed > 0) ? LanguageKey.LK_Exchange_Debt_Used.TrFormat(this.Exchange.AdvantageSummary.DebtUsed) : LanguageKey.LK_Exchange_Debt.Tr());
			}
			bool flag4 = this.exchangeContainer.debtImg;
			if (flag4)
			{
				this.exchangeContainer.debtImg.sprite = ((this.Exchange.AdvantageSummary.DebtUsed > 0) ? this.exchangeContainer.secretIsSome : this.exchangeContainer.secretIsNone);
			}
			bool flag5 = this.exchangeContainer.debt;
			if (flag5)
			{
				this.exchangeContainer.debt.interactable = (this.Exchange.AdvantageSummary.DebtMax > 0);
			}
		}

		// Token: 0x17000DF5 RID: 3573
		// (get) Token: 0x06008034 RID: 32820 RVA: 0x003BABA6 File Offset: 0x003B8DA6
		protected virtual LanguageKey ExchangeNotFair
		{
			get
			{
				return LanguageKey.LK_Exchange_Not_Fair;
			}
		}

		// Token: 0x17000DF6 RID: 3574
		// (get) Token: 0x06008035 RID: 32821 RVA: 0x003BABAD File Offset: 0x003B8DAD
		protected virtual LanguageKey ExchangeEmpty
		{
			get
			{
				return LanguageKey.LK_Exchange_Plan_Is_Empty;
			}
		}

		// Token: 0x06008036 RID: 32822 RVA: 0x003BABB4 File Offset: 0x003B8DB4
		protected virtual void RefreshButtons()
		{
			bool flag = this.exchangeContainer.balance;
			if (flag)
			{
				List<ItemDisplayData> itemList = (from x in this.GetSelfTradeableList(0)
				where x != null
				select x as ItemDisplayData).ToList<ItemDisplayData>();
				LanguageKey tipContent;
				this.exchangeContainer.balance.interactable = this.Exchange.CanBalance(itemList, out tipContent);
				TooltipInvoker tip = this.exchangeContainer.balance.gameObject.GetOrAddComponent<TooltipInvoker>();
				tip.enabled = !this.exchangeContainer.balance.interactable;
				bool enabled = tip.enabled;
				if (enabled)
				{
					tip.Type = TipType.SingleDesc;
					tip.PresetParam = new string[]
					{
						tipContent.Tr().SetColor("brightred")
					};
				}
			}
			bool flag2 = this.exchangeContainer.confirmDisplayer.enabled = !(this.exchangeContainer.confirm.interactable = this.CanConfirmExchange);
			if (flag2)
			{
				this.exchangeContainer.confirmDisplayer.PresetParam[0] = ((this.Exchange.ExchangeItemList.Count > 0) ? this.ExchangeNotFair : this.ExchangeEmpty).Tr();
			}
			this.RefreshPutButtons();
			bool flag3 = this.exchangeContainer.reset;
			if (flag3)
			{
				this.exchangeContainer.reset.interactable = (this.Exchange.ExchangeItemList.Count > 0 || this.Exchange.AdvantageSummary.SecretId != -1 || this.Exchange.AdvantageSummary.ApprovingCharId != -1 || this.Exchange.AdvantageSummary.DebtUsed != 0);
			}
			bool flag4 = this.exchangeContainer.resetSelf;
			if (flag4)
			{
				this.exchangeContainer.resetSelf.interactable = this.Exchange.ExchangeItemList.Any((ExchangeItem x) => x.Count < 0);
			}
			bool flag5 = this.exchangeContainer.resetTarget;
			if (flag5)
			{
				this.exchangeContainer.resetTarget.interactable = this.Exchange.ExchangeItemList.Any((ExchangeItem x) => x.Count > 0);
			}
			int index = this.exchangeContainer.targetPage ? this.exchangeContainer.targetPage.GetActiveIndex() : -1;
			bool flag6 = this.exchangeContainer.selfMultiplyOperate;
			if (flag6)
			{
				this.exchangeContainer.selfMultiplyOperate.gameObject.SetActive(index != 4);
			}
			bool flag7 = this.exchangeContainer.targetMultiplyOperate;
			if (flag7)
			{
				this.exchangeContainer.targetMultiplyOperate.gameObject.SetActive(index != 4);
			}
		}

		// Token: 0x06008037 RID: 32823 RVA: 0x003BAEE0 File Offset: 0x003B90E0
		protected virtual void RefreshPutButtons()
		{
			bool flag = this.exchangeContainer.putSelfAll;
			if (flag)
			{
				this.exchangeContainer.putSelfAll.interactable = this.PutAllFilter(this.exchangeContainer.selfItemList, this.GetSelfTradeableList(), this.Exchange.TaiwuContentList, this.TaiwuDisplayData).Any((ITradeableContent _) => true);
			}
			bool flag2 = this.exchangeContainer.putTargetAll;
			if (flag2)
			{
				this.exchangeContainer.putTargetAll.interactable = this.PutAllFilter(this.exchangeContainer.targetItemList, this.GetTargetTradeableList(), this.Exchange.TargetContentList, this.TargetDisplayData).Any((ITradeableContent _) => true);
			}
		}

		// Token: 0x06008038 RID: 32824 RVA: 0x003BAFD0 File Offset: 0x003B91D0
		protected virtual void RefreshTargetItems(int index)
		{
			this.exchangeContainer.targetItemList.Exchange = (this.exchangeContainer.targetExchangeList.Exchange = this.Exchange);
			this.exchangeContainer.targetItemList.SetItemList(this.Filter(this.GetTargetTradeableList(index), this.Exchange.TargetContentList, this.TargetDisplayData).Select(delegate(ITradeableContent x)
			{
				bool flag = ItemTemplateHelper.IsMiscResource(x.Key.ItemType, x.Key.TemplateId);
				if (flag)
				{
					x.Value = this.Exchange.CalcBaseValue(x) * (long)x.Amount;
				}
				return x;
			}).ToArray<ITradeableContent>());
			this.exchangeContainer.targetExchangeList.SetItemList(this.Exchange.TargetContentList);
			this.exchangeContainer.targetItemList.InfiniteScroll.ReRender();
			this.exchangeContainer.targetExchangeList.InfiniteScroll.ReRender();
		}

		// Token: 0x06008039 RID: 32825 RVA: 0x003BB094 File Offset: 0x003B9294
		protected virtual void RefreshTargetColumnDefinitions(int index)
		{
			this.exchangeContainer.targetItemList.SetColumnTypeFlags(this.ColumnType);
			this.exchangeContainer.targetExchangeList.SetColumnTypeFlags(this.ColumnType);
		}

		// Token: 0x0600803A RID: 32826 RVA: 0x003BB0C5 File Offset: 0x003B92C5
		protected virtual void RefreshSelfItems()
		{
			CToggleGroup currPage = this.exchangeContainer.currPage;
			this.RefreshSelfItems((currPage != null) ? currPage.GetActiveIndex() : -1);
		}

		// Token: 0x0600803B RID: 32827 RVA: 0x003BB0E5 File Offset: 0x003B92E5
		protected virtual void RefreshTargetItems()
		{
			CToggleGroup targetPage = this.exchangeContainer.targetPage;
			this.RefreshTargetItems((targetPage != null) ? targetPage.GetActiveIndex() : -1);
		}

		// Token: 0x0600803C RID: 32828 RVA: 0x003BB108 File Offset: 0x003B9308
		protected virtual void RefreshSelfItems(int index)
		{
			this.exchangeContainer.selfItemList.Exchange = (this.exchangeContainer.selfExchangeList.Exchange = this.Exchange);
			Exchange exchange = this.Exchange;
			if (!true)
			{
			}
			ItemSourceType itemSourceType;
			switch (index)
			{
			case 0:
				itemSourceType = ItemSourceType.Inventory;
				break;
			case 1:
				itemSourceType = ItemSourceType.Warehouse;
				break;
			case 2:
				itemSourceType = ItemSourceType.Treasury;
				break;
			default:
				itemSourceType = ItemSourceType.Inventory;
				break;
			}
			if (!true)
			{
			}
			exchange.SetItemSource((sbyte)itemSourceType);
			this.exchangeContainer.selfItemList.SetItemList(this.Filter(this.GetSelfTradeableList(index), this.Exchange.TaiwuContentList, this.TaiwuDisplayData).Select(delegate(ITradeableContent x)
			{
				bool flag = ItemTemplateHelper.IsMiscResource(x.Key.ItemType, x.Key.TemplateId);
				if (flag)
				{
					x.Value = this.Exchange.CalcBaseValue(x) * (long)x.Amount;
				}
				return x;
			}).ToArray<ITradeableContent>());
			this.exchangeContainer.selfExchangeList.SetItemList(this.Exchange.TaiwuContentList);
			this.exchangeContainer.selfItemList.InfiniteScroll.ReRender();
			this.exchangeContainer.selfExchangeList.InfiniteScroll.ReRender();
		}

		// Token: 0x0600803D RID: 32829 RVA: 0x003BB208 File Offset: 0x003B9408
		protected virtual void RefreshSelfColumnDefinitions(int index)
		{
			bool flag = index == 0 || index == 2 || index == 1 || index == 3 || index == 4;
			if (flag)
			{
				this.exchangeContainer.selfItemList.SetColumnTypeFlags(this.ColumnType);
				this.exchangeContainer.selfExchangeList.SetColumnTypeFlags(this.ColumnType);
			}
			else
			{
				this.exchangeContainer.selfItemList.SetColumnDefinitions(this.GenerateColumnDefinitions(this.exchangeContainer.selfItemList, false), new Action<RowItem>(this.PrepareCustomRowTemplateContainersForCharacter));
				this.exchangeContainer.selfExchangeList.SetColumnDefinitions(this.GenerateColumnDefinitions(this.exchangeContainer.selfExchangeList, false), new Action<RowItem>(this.PrepareCustomRowTemplateContainersForCharacter));
			}
		}

		// Token: 0x0600803E RID: 32830 RVA: 0x003BB2C4 File Offset: 0x003B94C4
		public virtual void RefreshScroll()
		{
			switch (this.ExtraScrollToTopAction)
			{
			case 1:
			{
				this.exchangeContainer.selfItemList.InfiniteScroll.Scroll.ScrollTo(Vector2.zero, 0.3f);
				bool flag = this.exchangeContainer.selfExchangeList;
				if (flag)
				{
					this.exchangeContainer.selfExchangeList.InfiniteScroll.Scroll.ScrollTo(Vector2.zero, 0.3f);
				}
				break;
			}
			case 2:
			{
				this.exchangeContainer.targetItemList.InfiniteScroll.Scroll.ScrollTo(Vector2.zero, 0.3f);
				bool flag2 = this.exchangeContainer.targetExchangeList;
				if (flag2)
				{
					this.exchangeContainer.targetExchangeList.InfiniteScroll.Scroll.ScrollTo(Vector2.zero, 0.3f);
				}
				break;
			}
			case 3:
			{
				this.exchangeContainer.selfItemList.InfiniteScroll.Scroll.ScrollTo(Vector2.zero, 0.3f);
				this.exchangeContainer.targetItemList.InfiniteScroll.Scroll.ScrollTo(Vector2.zero, 0.3f);
				bool flag3 = this.exchangeContainer.selfExchangeList;
				if (flag3)
				{
					this.exchangeContainer.selfExchangeList.InfiniteScroll.Scroll.ScrollTo(Vector2.zero, 0.3f);
				}
				bool flag4 = this.exchangeContainer.targetExchangeList;
				if (flag4)
				{
					this.exchangeContainer.targetExchangeList.InfiniteScroll.Scroll.ScrollTo(Vector2.zero, 0.3f);
				}
				break;
			}
			}
			this.ExtraScrollToTopAction = 0;
		}

		// Token: 0x0600803F RID: 32831 RVA: 0x003BB484 File Offset: 0x003B9684
		public static string GetPreviewString(int init, int preview, double scale = 1.0)
		{
			return ((preview < 0) ? string.Format("{0}", (double)init * scale).SetColor("red") : string.Format("{0}", (double)init * scale)) + ViewExchangeBase.ShowDiffValue(preview - init, scale, true);
		}

		// Token: 0x06008040 RID: 32832 RVA: 0x003BB4D8 File Offset: 0x003B96D8
		public static string GetAuthorityString(int init, int preview)
		{
			return LanguageKey.LK_Exchange_Authority_Value.TrFormat((preview < 0) ? string.Format("{0}", init).SetColor("red") : string.Format("{0}", init), ViewExchangeBase.ShowDiffValue(preview - init, 1.0, true));
		}

		// Token: 0x06008041 RID: 32833 RVA: 0x003BB531 File Offset: 0x003B9731
		public static string GetWeightString(int init, int max, int preview, int maxPreview, LanguageKey lk = LanguageKey.LK_Exchange_Weight_Value)
		{
			return ViewExchangeBase.GetNewWeightDiffString(init, max, preview, maxPreview, lk, 0.01);
		}

		// Token: 0x06008042 RID: 32834 RVA: 0x003BB548 File Offset: 0x003B9748
		public static string GetNewWeightDiffString(int init, int max, int preview, int maxPreview, LanguageKey key, double scale)
		{
			return key.TrFormat(new object[]
			{
				string.Format("{0}", (double)preview * scale).SetColor(CommonUtils.GetLoadWeightValueColor(preview, maxPreview)),
				"",
				(max == maxPreview) ? string.Format("{0}", (double)max * scale) : string.Format("{0}", (double)max * scale).SetColor((maxPreview > max) ? "brightblue" : "brightred"),
				""
			});
		}

		// Token: 0x06008043 RID: 32835 RVA: 0x003BB5DB File Offset: 0x003B97DB
		public static string GetWarehouseWeightString(int init)
		{
			return ViewExchangeBase.GetDiffString(init, init * 2 + 100, init, init * 2 + 100, LanguageKey.LK_Exchange_Weight_Warehouse_Value, 0.01);
		}

		// Token: 0x06008044 RID: 32836 RVA: 0x003BB600 File Offset: 0x003B9800
		public static string GetDiffString(int init, int max, int preview, int maxPreview, LanguageKey key, double scale)
		{
			return key.TrFormat(new object[]
			{
				string.Format("{0}", (double)init * scale).SetColor(CommonUtils.GetLoadWeightValueColor(preview, maxPreview)),
				ViewExchangeBase.ShowDiffValue(preview - init, scale, true),
				(double)max * scale,
				ViewExchangeBase.ShowDiffValue(maxPreview - max, scale, true)
			});
		}

		// Token: 0x06008045 RID: 32837 RVA: 0x003BB668 File Offset: 0x003B9868
		public static string ShowDiffValue(int value, double scale, bool isPositive = true)
		{
			return (value == 0) ? "" : ((value > 0) ? string.Format("+{0}", (double)value * scale) : string.Format("-{0}", (double)(-(double)value) * scale)).SetColor((value > 0 ^ isPositive) ? "darkred" : "lightblue");
		}

		// Token: 0x17000DF7 RID: 3575
		// (get) Token: 0x06008046 RID: 32838 RVA: 0x003BB6C8 File Offset: 0x003B98C8
		protected virtual bool ShouldConfirmHide
		{
			get
			{
				bool result;
				if (this.ConfirmOnHideKey != LanguageKey.Invalid)
				{
					Exchange exchange = this.Exchange;
					List<ExchangeItem> list = (exchange != null) ? exchange.ExchangeItemList : null;
					result = (list != null && list.Count > 0);
				}
				else
				{
					result = false;
				}
				return result;
			}
		}

		// Token: 0x17000DF8 RID: 3576
		// (get) Token: 0x06008047 RID: 32839 RVA: 0x003BB703 File Offset: 0x003B9903
		protected virtual string ConfirmOnHideDesc
		{
			get
			{
				return "";
			}
		}

		// Token: 0x06008048 RID: 32840 RVA: 0x003BB70A File Offset: 0x003B990A
		public virtual void QuickHideImpl()
		{
			base.QuickHide();
		}

		// Token: 0x06008049 RID: 32841 RVA: 0x003BB714 File Offset: 0x003B9914
		public override void QuickHide()
		{
			bool shouldConfirmHide = this.ShouldConfirmHide;
			if (shouldConfirmHide)
			{
				UIElement dialog = UIElement.Dialog;
				ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>();
				string key = "Cmd";
				DialogCmd dialogCmd = new DialogCmd();
				dialogCmd.Type = 1;
				dialogCmd.Title = this.ConfirmOnHideKey.Tr();
				dialogCmd.Content = this.ConfirmOnHideDesc;
				dialogCmd.Yes = new Action(this.QuickHideImpl);
				dialogCmd.No = delegate()
				{
				};
				dialog.SetOnInitArgs(argumentBox.SetObject(key, dialogCmd));
				UIManager.Instance.MaskUI(UIElement.Dialog);
			}
			else
			{
				this.QuickHideImpl();
			}
		}

		// Token: 0x0600804A RID: 32842 RVA: 0x003BB7C5 File Offset: 0x003B99C5
		private void OnTargetSortAndFilterChangedCallback()
		{
			this.OnSortAndFilterChangedCallback(this.exchangeContainer.targetItemList);
			this.RefreshPutButtons();
		}

		// Token: 0x0600804B RID: 32843 RVA: 0x003BB7E1 File Offset: 0x003B99E1
		private void OnSelfSortAndFilterChangedCallback()
		{
			this.OnSortAndFilterChangedCallback(this.exchangeContainer.selfItemList);
			this.RefreshPutButtons();
		}

		// Token: 0x0600804C RID: 32844 RVA: 0x003BB800 File Offset: 0x003B9A00
		private void OnSortAndFilterChangedCallback(ItemListScroll itemListScroll)
		{
			ItemSortAndFilterController itemSortAndFilterController = itemListScroll.SortAndFilterController as ItemSortAndFilterController;
			bool flag = itemSortAndFilterController == null;
			if (!flag)
			{
				bool flag2 = !(this is ViewWarehouse);
				if (!flag2)
				{
					ESelectItemFilterType currentFilterType = itemSortAndFilterController.GetCurrentFilterType();
					if (!true)
					{
					}
					ItemListScroll.EColumnType ecolumnType;
					switch (currentFilterType)
					{
					case ESelectItemFilterType.Food:
						ecolumnType = (ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Amount | ItemListScroll.EColumnType.Type | ItemListScroll.EColumnType.Weight | ItemListScroll.EColumnType.Value);
						break;
					case ESelectItemFilterType.Medicine:
						ecolumnType = (ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Amount | ItemListScroll.EColumnType.Type | ItemListScroll.EColumnType.Weight | ItemListScroll.EColumnType.Value);
						break;
					case ESelectItemFilterType.Equipment:
						ecolumnType = (ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Type | ItemListScroll.EColumnType.Weight | ItemListScroll.EColumnType.Value | ItemListScroll.EColumnType.Durability);
						break;
					case ESelectItemFilterType.EquipmentWeapon:
						ecolumnType = (ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Type | ItemListScroll.EColumnType.Weight | ItemListScroll.EColumnType.Value | ItemListScroll.EColumnType.Durability);
						break;
					case ESelectItemFilterType.EquipmentArmor:
						ecolumnType = (ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Type | ItemListScroll.EColumnType.Weight | ItemListScroll.EColumnType.Value | ItemListScroll.EColumnType.Durability);
						break;
					case ESelectItemFilterType.EquipmentAccessory:
						ecolumnType = (ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Type | ItemListScroll.EColumnType.Weight | ItemListScroll.EColumnType.Value | ItemListScroll.EColumnType.Durability);
						break;
					case ESelectItemFilterType.EquipmentClothing:
						ecolumnType = (ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Type | ItemListScroll.EColumnType.Weight | ItemListScroll.EColumnType.Value | ItemListScroll.EColumnType.Durability);
						break;
					case ESelectItemFilterType.EquipmentCarrier:
						ecolumnType = (ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Type | ItemListScroll.EColumnType.Weight | ItemListScroll.EColumnType.Value | ItemListScroll.EColumnType.Durability);
						break;
					case ESelectItemFilterType.Book:
						ecolumnType = (ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Type | ItemListScroll.EColumnType.Weight | ItemListScroll.EColumnType.Value | ItemListScroll.EColumnType.Durability);
						break;
					case ESelectItemFilterType.Tool:
						ecolumnType = (ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Type | ItemListScroll.EColumnType.Weight | ItemListScroll.EColumnType.Value | ItemListScroll.EColumnType.Durability);
						break;
					case ESelectItemFilterType.Material:
						ecolumnType = (ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Amount | ItemListScroll.EColumnType.Type | ItemListScroll.EColumnType.Weight | ItemListScroll.EColumnType.Value);
						break;
					case ESelectItemFilterType.Misc:
						ecolumnType = (ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Amount | ItemListScroll.EColumnType.Type | ItemListScroll.EColumnType.Weight | ItemListScroll.EColumnType.Value);
						break;
					case ESelectItemFilterType.MiscCricket:
						ecolumnType = (ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.CricketAge | ItemListScroll.EColumnType.CricketDurability | ItemListScroll.EColumnType.CricketWin | ItemListScroll.EColumnType.CricketLose);
						break;
					default:
						ecolumnType = (ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Amount | ItemListScroll.EColumnType.Type | ItemListScroll.EColumnType.Weight | ItemListScroll.EColumnType.Value);
						break;
					}
					if (!true)
					{
					}
					ItemListScroll.EColumnType columnType = ecolumnType;
					bool flag3 = itemListScroll.ColumnTypeFlags != columnType;
					if (flag3)
					{
						itemListScroll.SetColumnTypeFlags(columnType);
					}
				}
			}
		}

		// Token: 0x0600804D RID: 32845 RVA: 0x003BB910 File Offset: 0x003B9B10
		protected int GetInitSelectCount(ITradeableContent itemData, bool isSelected)
		{
			bool flag = itemData.IsResource && itemData.OwnerCharId == SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			int result;
			if (flag)
			{
				bool flag2 = this.Exchange.TotalValueWithAdvantage == 0L;
				if (flag2)
				{
					result = (isSelected ? itemData.Amount : 1);
				}
				else
				{
					result = this.Exchange.GetBalanceResourceAmount(itemData as ItemDisplayData, isSelected);
				}
			}
			else
			{
				result = 0;
			}
			return result;
		}

		// Token: 0x06008050 RID: 32848 RVA: 0x003BB9B9 File Offset: 0x003B9BB9
		[CompilerGenerated]
		internal static void <AddSecretOrPopSelections>g__OnCancel|43_0()
		{
		}

		// Token: 0x06008051 RID: 32849 RVA: 0x003BB9BC File Offset: 0x003B9BBC
		[CompilerGenerated]
		private void <AddSecretOrPopSelections>g__ShowSelectSecret|43_1()
		{
			ArgumentBox argsBox = EasyPool.Get<ArgumentBox>().SetObject("secretInformation", this.SecretInformationDisplayPackage).SetObject("ExchangeData", this.Exchange.AdvantageSummary).SetObject("callback", new Action<SecretInformationDisplayData>(this.<AddSecretOrPopSelections>g__SetSecretImpl|43_2));
			UIElement.SelectInformationForShopping.SetOnInitArgs(argsBox);
			UIManager.Instance.MaskUI(UIElement.SelectInformationForShopping);
		}

		// Token: 0x06008052 RID: 32850 RVA: 0x003BBA27 File Offset: 0x003B9C27
		[CompilerGenerated]
		private void <AddSecretOrPopSelections>g__SetSecretImpl|43_2(SecretInformationDisplayData selectedSecret)
		{
			this.Exchange.SetSecret(selectedSecret);
			ViewExchangeBase.<AddSecretOrPopSelections>g__OnCancel|43_0();
			this.Refresh();
		}

		// Token: 0x04006223 RID: 25123
		[SerializeField]
		protected ExchangeContainer exchangeContainer;

		// Token: 0x04006224 RID: 25124
		protected Sequence BubbleAnim;

		// Token: 0x04006225 RID: 25125
		protected int TargetId;

		// Token: 0x04006226 RID: 25126
		protected int ExchangeValueType = -1;

		// Token: 0x04006227 RID: 25127
		protected bool ShowNonInteractableGoods = false;

		// Token: 0x04006228 RID: 25128
		private BubbleLevel _currentLevel = BubbleLevel.Low;

		// Token: 0x04006229 RID: 25129
		protected int ExtraScrollToTopAction = 0;

		// Token: 0x0400622A RID: 25130
		protected LanguageKey ConfirmOnHideKey = LanguageKey.Invalid;

		// Token: 0x02001FC2 RID: 8130
		public static class ToggleDefine
		{
			// Token: 0x0400CE85 RID: 52869
			public const int Inventory = 0;

			// Token: 0x0400CE86 RID: 52870
			public const int Warehouse = 1;

			// Token: 0x0400CE87 RID: 52871
			public const int Treasury = 2;

			// Token: 0x0400CE88 RID: 52872
			public const int Stock = 3;

			// Token: 0x0400CE89 RID: 52873
			public const int Trough = 4;

			// Token: 0x0400CE8A RID: 52874
			public const int Kidnapped = 5;
		}
	}
}
