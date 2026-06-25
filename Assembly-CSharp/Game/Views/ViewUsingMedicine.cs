using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Character;
using Game.Components.ListStyleGeneralScroll;
using Game.Components.ListStyleGeneralScroll.Item;
using Game.Components.SortAndFilter.Item;
using Game.Components.SortAndFilter.Item.Apply;
using Game.Views.CharacterMenu;
using Game.Views.Exchange;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Global;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.GameDataBridge;
using GameData.Serializer;
using TMPro;
using UnityEngine;

namespace Game.Views
{
	// Token: 0x0200071D RID: 1821
	public class ViewUsingMedicine : UIBase
	{
		// Token: 0x17000A7A RID: 2682
		// (get) Token: 0x060056B2 RID: 22194 RVA: 0x002836E0 File Offset: 0x002818E0
		private int MaxLoad
		{
			get
			{
				return this._usingMedicineDisplayData.MaxLoad;
			}
		}

		// Token: 0x17000A7B RID: 2683
		// (get) Token: 0x060056B3 RID: 22195 RVA: 0x002836ED File Offset: 0x002818ED
		private int CurLoad
		{
			get
			{
				return this._usingMedicineDisplayData.CurLoad;
			}
		}

		// Token: 0x17000A7C RID: 2684
		// (get) Token: 0x060056B4 RID: 22196 RVA: 0x002836FA File Offset: 0x002818FA
		private bool NeedAutoUseMedicine
		{
			get
			{
				return this._usingMedicineDisplayData.NeedAutoUseMedicine;
			}
		}

		// Token: 0x17000A7D RID: 2685
		// (get) Token: 0x060056B5 RID: 22197 RVA: 0x00283708 File Offset: 0x00281908
		private bool HasAutoUseMedicine
		{
			get
			{
				Inventory autoUseMedicineInventory = this._usingMedicineDisplayData.AutoUseMedicineInventory;
				bool result;
				if (autoUseMedicineInventory == null)
				{
					result = false;
				}
				else
				{
					Dictionary<ItemKey, int> items = autoUseMedicineInventory.Items;
					int? num = (items != null) ? new int?(items.Count) : null;
					int num2 = 0;
					result = (num.GetValueOrDefault() > num2 & num != null);
				}
				return result;
			}
		}

		// Token: 0x17000A7E RID: 2686
		// (get) Token: 0x060056B6 RID: 22198 RVA: 0x0028375A File Offset: 0x0028195A
		private Inventory AutoUseMedicineInventory
		{
			get
			{
				return this._usingMedicineDisplayData.AutoUseMedicineInventory;
			}
		}

		// Token: 0x17000A7F RID: 2687
		// (get) Token: 0x060056B7 RID: 22199 RVA: 0x00283767 File Offset: 0x00281967
		public ViewCharacterMenu CharacterMenu
		{
			get
			{
				return UIElement.CharacterMenu.UiBaseAs<ViewCharacterMenu>();
			}
		}

		// Token: 0x060056B8 RID: 22200 RVA: 0x00283774 File Offset: 0x00281974
		public override void OnInit(ArgumentBox argsBox)
		{
			base.GetComponent<CanvasGroup>().alpha = 0f;
			short type;
			bool flag = argsBox != null && argsBox.Get("UsingMedicineItemType", out type);
			if (flag)
			{
				this._usingMedicineItemType = type;
			}
			else
			{
				this._usingMedicineItemType = UsingMedicineItemType.Invalid;
			}
			ItemKey selectedItemKey;
			bool flag2 = argsBox != null && argsBox.Get<ItemKey>("SelectedItemKey", out selectedItemKey);
			if (flag2)
			{
				this._selectedItemKey = selectedItemKey;
			}
			else
			{
				this._selectedItemKey = ItemKey.Invalid;
			}
			bool flag3 = argsBox == null || !argsBox.Get("CurrentCharacterId", out this._charId);
			if (flag3)
			{
				this._charId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			}
			this._canOpenEatDetail = (UIElement.CharacterMenu.IsShowing || UIElement.EatDetail.IsShowing);
			this.injury.SetAreaInteract(this._canOpenEatDetail ? new Action(this.OnClickEatArea) : null);
			this.injury.SetEatAreaInteractable(this._canOpenEatDetail);
			this.NeedDataListenerId = true;
			UIElement element = this.Element;
			element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(this.OnListenerIdReady));
		}

		// Token: 0x060056B9 RID: 22201 RVA: 0x0028389C File Offset: 0x00281A9C
		private void Awake()
		{
			this.itemScroll.Init("ViewUsingMedicine", ESortAndFilterControllerType.UsingMedicine, true, new Action<ITradeableContent, RowItemLine>(this.OnItemRender), new Action<ITradeableContent, RowItemLine>(this.OnClickItem), ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Amount | ItemListScroll.EColumnType.Type | ItemListScroll.EColumnType.Weight | ItemListScroll.EColumnType.Value | ItemListScroll.EColumnType.Durability, null, null, null);
			this.buttonBack.ClearAndAddListener(new Action(this.OnClickButtonBack));
		}

		// Token: 0x060056BA RID: 22202 RVA: 0x002838F4 File Offset: 0x00281AF4
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
						bool flag2 = notification.MethodId == 206;
						if (flag2)
						{
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._usingMedicineDisplayData);
							this.RefreshLoad();
							this.attributeAndInjury.Attribute.Set(this._usingMedicineDisplayData.AttributeDisplayData);
							this.attributeAndInjury.Injury.Set(this._usingMedicineDisplayData.InjuryDisplayData, true);
							this.attributeAndInjury.SwitchToInjury();
							this.injury.ExitSelectInjuryPart();
							this.OnUsingMedicineItemSwitch();
							bool flag3 = !this._selectedItemKey.Equals(ItemKey.Invalid);
							if (flag3)
							{
								ITradeableContent targetData = this.itemScroll.FilteredData.FirstOrDefault((ITradeableContent d) => d.ContainsItemKey(this._selectedItemKey));
								int targetDataIndex = this.itemScroll.FilteredData.IndexOf(targetData);
								this.itemScroll.InfiniteScroll.OnRenderEnd += this.OnRenderEnd;
								this.itemScroll.InfiniteScroll.ScrollTo(targetDataIndex, 0f);
							}
							this.buttonAutoUseMedicine.interactable = (this.NeedAutoUseMedicine && this.HasAutoUseMedicine);
							TooltipInvoker tip = this.buttonAutoUseMedicine.GetComponent<TooltipInvoker>();
							tip.enabled = !this.buttonAutoUseMedicine.interactable;
							bool enabled = tip.enabled;
							if (enabled)
							{
								string content = string.Empty;
								bool flag4 = !this.NeedAutoUseMedicine;
								if (flag4)
								{
									content = LanguageKey.LK_AutoUseMedicine_Tip_NoNeed.Tr().SetColor("brightred");
								}
								else
								{
									bool flag5 = !this.HasAutoUseMedicine;
									if (flag5)
									{
										content = LanguageKey.LK_AutoUseMedicine_Tip_NoMedicine.Tr().SetColor("brightred");
									}
								}
								tip.PresetParam = new string[]
								{
									content
								};
							}
							base.GetComponent<CanvasGroup>().alpha = 1f;
							this.Element.ShowAfterRefresh();
							GEvent.OnEvent(UiEvents.OnShowUsingMedicine, null);
							GlobalDomainMethod.Call.InvokeGuidingTrigger(64);
						}
					}
				}
			}
		}

		// Token: 0x060056BB RID: 22203 RVA: 0x00283B80 File Offset: 0x00281D80
		private void OnRenderEnd()
		{
			bool flag = this._selectedItemKey.Equals(ItemKey.Invalid);
			if (!flag)
			{
				RowItemLine targetView = this.itemScroll.FindActiveItem(this._selectedItemKey, false);
				this._selectedItemKey = ItemKey.Invalid;
				this.EnterSelectInjuryPart(targetView.Data, targetView, 1);
				this.itemScroll.InfiniteScroll.OnRenderEnd -= this.OnRenderEnd;
				base.GetComponent<CanvasGroup>().alpha = 1f;
			}
		}

		// Token: 0x060056BC RID: 22204 RVA: 0x00283C00 File Offset: 0x00281E00
		protected override void OnClick(Transform btn)
		{
			base.OnClick(btn);
			string name = btn.name;
			string a = name;
			if (!(a == "ButtonCloseView"))
			{
				if (a == "ButtonAutoUseMedicine")
				{
					this.OnClickAutoUseMedicine();
				}
			}
			else
			{
				this.QuickHide();
			}
		}

		// Token: 0x060056BD RID: 22205 RVA: 0x00283C50 File Offset: 0x00281E50
		public override void QuickHide()
		{
			bool isSelectInjuryPart = this.injury.IsSelectInjuryPart;
			if (isSelectInjuryPart)
			{
				this.injury.ExitSelectInjuryPart();
			}
			else
			{
				base.QuickHide();
			}
		}

		// Token: 0x060056BE RID: 22206 RVA: 0x00283C84 File Offset: 0x00281E84
		private void OnClickButtonBack()
		{
			bool isSelectInjuryPart = this.injury.IsSelectInjuryPart;
			if (isSelectInjuryPart)
			{
				this.injury.ExitSelectInjuryPart();
				AudioManager.Instance.PlaySound("ui_default_cancel", false, false);
			}
		}

		// Token: 0x060056BF RID: 22207 RVA: 0x00283CC4 File Offset: 0x00281EC4
		private void OnClickEatArea()
		{
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.Set("charId", this._charId);
			UIElement.EatDetail.SetOnInitArgs(argBox);
			UIManager.Instance.ShowUI(UIElement.EatDetail, true);
		}

		// Token: 0x060056C0 RID: 22208 RVA: 0x00283D07 File Offset: 0x00281F07
		private void OnEnable()
		{
			GEvent.Add(UiEvents.UsingMedicineItemSwitch, new GEvent.Callback(this.OnUsingMedicineItemSwitch));
			GEvent.Add(UiEvents.OnShowEatDetail, new GEvent.Callback(this.OnShowEatDetail));
		}

		// Token: 0x060056C1 RID: 22209 RVA: 0x00283D3F File Offset: 0x00281F3F
		private void OnShowEatDetail(ArgumentBox argBox)
		{
			this.QuickHide();
		}

		// Token: 0x060056C2 RID: 22210 RVA: 0x00283D49 File Offset: 0x00281F49
		private void OnDisable()
		{
			GEvent.Remove(UiEvents.UsingMedicineItemSwitch, new GEvent.Callback(this.OnUsingMedicineItemSwitch));
			GEvent.Remove(UiEvents.OnShowEatDetail, new GEvent.Callback(this.OnShowEatDetail));
		}

		// Token: 0x060056C3 RID: 22211 RVA: 0x00283D81 File Offset: 0x00281F81
		private void OnListenerIdReady()
		{
			CharacterDomainMethod.Call.GetCharacterUsingMedicineDisplayData(this.Element.GameDataListenerId, this._charId);
		}

		// Token: 0x060056C4 RID: 22212 RVA: 0x00283D9C File Offset: 0x00281F9C
		private void RefreshLoad()
		{
			this.loadText.text = ViewExchangeBase.GetWeightString(this.CurLoad, this.MaxLoad, this.CurLoad, this.MaxLoad, LanguageKey.LK_Exchange_Weight_Value);
			TooltipInvoker tipDisplayer = this.loadOverflowTips;
			tipDisplayer.enabled = (this.CharacterMenu.IsTaiwuTeam && this.CurLoad > this.MaxLoad);
			bool enabled = tipDisplayer.enabled;
			if (enabled)
			{
				tipDisplayer.Type = TipType.Simple;
				TooltipInvoker tooltipInvoker = tipDisplayer;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
				}
				WorldStateItem worldStateItem = WorldState.Instance[11];
				tipDisplayer.RuntimeParam.Set("arg0", worldStateItem.Name);
				string loadTipContent = LocalStringManager.GetFormat(LanguageKey.LK_Inventory_Overflow_Tips, this._usingMedicineDisplayData.MoveTimeCostPercent - 100).ColorReplace();
				tipDisplayer.RuntimeParam.Set("arg1", loadTipContent);
			}
		}

		// Token: 0x060056C5 RID: 22213 RVA: 0x00283E8C File Offset: 0x0028208C
		private void CallRefreshItems()
		{
			CharacterDomainMethod.Call.GetCharacterUsingMedicineDisplayData(this.Element.GameDataListenerId, this._charId);
			GEvent.OnEvent(UiEvents.OnRefreshCharacterMenuItem, null);
		}

		// Token: 0x060056C6 RID: 22214 RVA: 0x00283EB8 File Offset: 0x002820B8
		private void SetEatItemInfectNotice(bool show, ITradeableContent itemDisplayData = null, int amount = 1)
		{
			bool flag = show && itemDisplayData != null;
			if (flag)
			{
				this.injury.ShowInfectNotice(itemDisplayData, amount);
				this.injury.ShowEatNotice(itemDisplayData, amount);
			}
			else
			{
				this.injury.HideNotice(true, true);
			}
		}

		// Token: 0x060056C7 RID: 22215 RVA: 0x00283F04 File Offset: 0x00282104
		private void OnItemRender(ITradeableContent itemData, RowItemLine rowItemLine)
		{
			RowItemMain rowItemMain = rowItemLine.GetComponentInChildren<RowItemMain>();
			rowItemMain.SetData(itemData);
			rowItemLine.Set(rowItemMain, true);
			string tipContent;
			bool isLock = this.CheckItemIsLocked(itemData, out tipContent);
			bool flag = tipContent.IsNullOrEmpty();
			if (flag)
			{
				rowItemMain.HideInteractionState();
			}
			else
			{
				rowItemMain.SetItemNotCanSelectReason(tipContent.ColorReplace());
			}
			rowItemLine.SetInteractable(!isLock, true);
			rowItemLine.SetDisabled(isLock);
		}

		// Token: 0x060056C8 RID: 22216 RVA: 0x00283F68 File Offset: 0x00282168
		private void OnClickItem(ITradeableContent itemData, RowItemLine rowItemLine)
		{
			bool flag = UIManager.Instance.IsElementActive(UIElement.CharacterMenu);
			if (flag)
			{
				bool flag2 = !this.CharacterMenu.CanOperate || !this.CharacterMenu.IsTaiwuTeam;
				if (flag2)
				{
					return;
				}
			}
			bool interactable = itemData.Interactable;
			if (interactable)
			{
				bool flag3 = CommonUtils.IsRecoverInjuryMainMedicineItem(itemData.RealKey);
				if (flag3)
				{
					this.OnClickEatItem(itemData, rowItemLine);
				}
				else
				{
					this.ShowItemOperateMenu(itemData, rowItemLine);
				}
			}
		}

		// Token: 0x060056C9 RID: 22217 RVA: 0x00283FE0 File Offset: 0x002821E0
		private void ShowItemOperateMenu(ITradeableContent itemData, RowItem itemView)
		{
			bool isShowing = UIElement.CharacterMenu.IsShowing;
			if (isShowing)
			{
				bool flag = !this.CharacterMenu.CanOperate || !this.CharacterMenu.IsTaiwuTeam;
				if (flag)
				{
					return;
				}
			}
			List<ViewPopupMenu.BtnData> btnList = new List<ViewPopupMenu.BtnData>();
			string limitTip;
			bool interactable = !Injury.CheckEatItemIsLocked(this.injury.Data, itemData, (int)UsingMedicineItemType.Invalid, out limitTip);
			bool flag2 = itemData.Key.ItemType == 8;
			if (flag2)
			{
				string btnName = CommonUtils.GetCanEatItemButtonName(itemData.Key);
				ViewPopupMenu.BtnData btnData = new ViewPopupMenu.BtnData(btnName, interactable, EItemMenuDisplayOrder.Eat, delegate()
				{
					this.OnClickEatItem(itemData, itemView);
				}, delegate()
				{
					this.SetEatItemInfectNotice(true, itemData, 1);
				}, delegate()
				{
					this.SetEatItemInfectNotice(false, null, 1);
				}, false);
				btnList.Add(btnData);
				bool flag3 = !interactable;
				if (flag3)
				{
					btnData.SetTip(string.Empty, limitTip);
				}
			}
			else
			{
				bool flag4 = CommonUtils.CanItemEat(itemData.Key.ItemType, itemData.Key.TemplateId, this._charId);
				if (flag4)
				{
					string btnName2 = LocalStringManager.Get(LanguageKey.LK_Eat_Item);
					ViewPopupMenu.BtnData btnData2 = new ViewPopupMenu.BtnData(btnName2, interactable, EItemMenuDisplayOrder.Eat, delegate()
					{
						this.OnClickEatItem(itemData, itemView);
					}, delegate()
					{
						this.SetEatItemInfectNotice(true, itemData, 1);
					}, delegate()
					{
						this.SetEatItemInfectNotice(false, null, 1);
					}, false);
					btnList.Add(btnData2);
					bool flag5 = !interactable;
					if (flag5)
					{
						btnData2.SetTip(string.Empty, limitTip);
					}
				}
			}
			bool flag6 = btnList.Count > 0;
			if (flag6)
			{
				this.itemScroll.SetItemToPopupMenuMode(itemView as RowItemLine, btnList, new Action(this.ExitSelectInjuryPart), false);
			}
		}

		// Token: 0x060056CA RID: 22218 RVA: 0x002841B8 File Offset: 0x002823B8
		private bool CheckItemIsLocked(ITradeableContent itemData, out string tipContent)
		{
			tipContent = string.Empty;
			bool isLocked = itemData.IsLocked;
			bool result;
			if (isLocked)
			{
				result = true;
			}
			else
			{
				bool isShowing = UIElement.CharacterMenu.IsShowing;
				if (isShowing)
				{
					bool flag = !this.CharacterMenu.CanOperate || !this.CharacterMenu.IsTaiwuTeam;
					if (flag)
					{
						return true;
					}
				}
				result = Injury.CheckEatItemIsLocked(this.injury.Data, itemData, (int)this._usingMedicineItemType, out tipContent);
			}
			return result;
		}

		// Token: 0x060056CB RID: 22219 RVA: 0x00284230 File Offset: 0x00282430
		private void OnClickAutoUseMedicine()
		{
			StringBuilder names = EasyPool.Get<StringBuilder>();
			foreach (KeyValuePair<ItemKey, int> keyValuePair in this.AutoUseMedicineInventory.Items)
			{
				ItemKey itemKey2;
				int num;
				keyValuePair.Deconstruct(out itemKey2, out num);
				ItemKey itemKey = itemKey2;
				int amount = num;
				names.Append(string.Format("{0}x{1} ", ItemTemplateHelper.GetName(itemKey.ItemType, itemKey.TemplateId), amount));
			}
			string title = LocalStringManager.Get(LanguageKey.LK_AutoUseMedicine_Title);
			string content = LocalStringManager.GetFormat(LanguageKey.LK_AutoUseMedicine_Content, names.ToString());
			CommonUtils.ShowConfirmDialog(title, content, delegate
			{
				foreach (KeyValuePair<ItemKey, int> keyValuePair2 in this.AutoUseMedicineInventory.Items)
				{
					ItemKey itemKey4;
					int num2;
					keyValuePair2.Deconstruct(out itemKey4, out num2);
					ItemKey itemKey3 = itemKey4;
					int amount2 = num2;
					ItemDisplayData itemData = new ItemDisplayData(itemKey3, amount2);
					EatingUtils.TryRequestAddEatingItems(this._charId, itemData, amount2, null, false, null);
				}
				this.CallRefreshItems();
			}, null, EDialogType.None);
			EasyPool.Free<StringBuilder>(names);
		}

		// Token: 0x060056CC RID: 22220 RVA: 0x00284308 File Offset: 0x00282508
		private void OnClickEatItem(ITradeableContent itemData, RowItem itemView)
		{
			ViewUsingMedicine.<>c__DisplayClass46_0 CS$<>8__locals1 = new ViewUsingMedicine.<>c__DisplayClass46_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.itemData = itemData;
			bool flag = CommonUtils.IsRecoverInjuryMainMedicineItem(CS$<>8__locals1.itemData.RealKey);
			if (flag)
			{
				bool isSelectInjuryPart = this.injury.IsSelectInjuryPart;
				if (isSelectInjuryPart)
				{
					this.injury.ExitSelectInjuryPart();
					AudioManager.Instance.PlaySound("ui_default_cancel", false, false);
				}
				else
				{
					this.EnterSelectInjuryPart(CS$<>8__locals1.itemData, itemView, 1);
				}
			}
			else
			{
				ValueTuple<int, string> valueTuple = Injury.CheckEatSlot(this.injury.Data, CS$<>8__locals1.itemData.Key, CS$<>8__locals1.itemData.Amount);
				int limitCount = valueTuple.Item1;
				string reason = valueTuple.Item2;
				bool flag2 = CS$<>8__locals1.itemData.Amount > 1 && ItemTemplateHelper.CanUseMultiple(CS$<>8__locals1.itemData.Key);
				if (flag2)
				{
					int minCount = 1;
					bool flag3 = ItemTemplateHelper.IsTianJieFuLu(CS$<>8__locals1.itemData.Key.ItemType, CS$<>8__locals1.itemData.Key.TemplateId);
					if (flag3)
					{
						minCount = ItemTemplateHelper.GetTianJieFuLuCountUnit();
					}
					this.itemScroll.SetItemToSelectCountMode(itemView as RowItemLine, delegate(int count)
					{
						bool flag4 = count <= 0;
						if (flag4)
						{
							base.<OnClickEatItem>g__Cancel|1();
						}
						else
						{
							base.<OnClickEatItem>g__Confirm|0(count);
						}
					}, new Action(CS$<>8__locals1.<OnClickEatItem>g__Cancel|1), 0, limitCount, minCount, LocalStringManager.Get(LanguageKey.LK_SelectCount_Limit_Eat), false, delegate(int count)
					{
						CS$<>8__locals1.<>4__this.SetEatItemInfectNotice(true, CS$<>8__locals1.itemData, count);
					}, false);
				}
				else
				{
					CS$<>8__locals1.<OnClickEatItem>g__Confirm|0(1);
				}
			}
		}

		// Token: 0x060056CD RID: 22221 RVA: 0x00284470 File Offset: 0x00282670
		private void EnterSelectInjuryPart(ITradeableContent itemData, RowItem itemView, int count)
		{
			this.itemScroll.HighLightItemView(itemView);
			this.buttonCloseView.gameObject.SetActive(false);
			Action <>9__1;
			this.injury.EnterSelectInjuryPart(itemData, delegate(List<sbyte> targetBodyParts)
			{
				int charId = this._charId;
				ITradeableContent itemData2 = itemData;
				int count2 = count;
				Action onRequest;
				if ((onRequest = <>9__1) == null)
				{
					onRequest = (<>9__1 = delegate()
					{
						this.injury.DelayRefreshOnEatItemSend();
						LifeRecordsController instance = SingletonObject.getInstance<LifeRecordsController>();
						if (instance != null)
						{
							instance.RemoveLatestYearRecordCache();
						}
						this.CallRefreshItems();
					});
				}
				EatingUtils.TryRequestAddEatingItems(charId, itemData2, count2, onRequest, false, targetBodyParts);
			}, new Action(this.ExitSelectInjuryPart));
		}

		// Token: 0x060056CE RID: 22222 RVA: 0x002844E4 File Offset: 0x002826E4
		private void ExitSelectInjuryPart()
		{
			this.buttonCloseView.gameObject.SetActive(true);
			this.OnUsingMedicineItemSwitch();
			this.itemScroll.SetScrollEnable(true);
			this.itemScroll.CancelHighLightItemView();
			this.injury.HideNotice(true, true);
			this.injury.RefreshAllHealBtn(this._charId, false);
		}

		// Token: 0x060056CF RID: 22223 RVA: 0x00284545 File Offset: 0x00282745
		private void OnUsingMedicineItemSwitch(ArgumentBox argbox)
		{
			argbox.Get("UsingMedicineItemType", out this._usingMedicineItemType);
			this.OnUsingMedicineItemSwitch();
		}

		// Token: 0x060056D0 RID: 22224 RVA: 0x00284564 File Offset: 0x00282764
		private void OnUsingMedicineItemSwitch()
		{
			this.injury.SelectPartByUsingMedicineItemType(this._usingMedicineItemType);
			int toggleIndex = -1;
			bool flag = UsingMedicineItemType.IsHurt(this._usingMedicineItemType);
			if (flag)
			{
				sbyte injuryType = (sbyte)this._usingMedicineItemType;
				ValueTuple<sbyte, sbyte> valueTuple = this.injury.Data.Injuries.Get(injuryType);
				sbyte outer = valueTuple.Item1;
				sbyte inner = valueTuple.Item2;
				bool flag2 = outer > 0 || inner == 0;
				if (flag2)
				{
					toggleIndex = EMedicineSubFilterKeys.Outer.ToInt();
				}
				else
				{
					bool flag3 = inner > 0;
					if (flag3)
					{
						toggleIndex = EMedicineSubFilterKeys.Inner.ToInt();
					}
				}
			}
			else
			{
				bool flag4 = UsingMedicineItemType.IsPoison(this._usingMedicineItemType);
				if (flag4)
				{
					toggleIndex = EMedicineSubFilterKeys.Detox.ToInt();
				}
				else
				{
					bool flag5 = UsingMedicineItemType.IsHealth(this._usingMedicineItemType);
					if (flag5)
					{
						toggleIndex = EMedicineSubFilterKeys.Health.ToInt();
					}
					else
					{
						bool flag6 = UsingMedicineItemType.IsDisorderOfQi(this._usingMedicineItemType);
						if (flag6)
						{
							toggleIndex = EMedicineSubFilterKeys.Disorder.ToInt();
						}
					}
				}
			}
			this.itemScroll.OnSortAndFilterChangedCallback = null;
			this.itemScroll.SortAndFilterController.SetToggleIsOn(EFilterLine.MedicineFilter.ToInt(), toggleIndex);
			this.itemScroll.OnSortAndFilterChangedCallback = new Action(this.OnSortAndFilterChangedCallback);
			foreach (ItemDisplayData itemDisplayData in this._usingMedicineDisplayData.ItemList)
			{
				string text;
				itemDisplayData.Interactable = !this.CheckItemIsLocked(itemDisplayData, out text);
			}
			this.itemScroll.SetItemList(this._usingMedicineDisplayData.ItemList);
		}

		// Token: 0x060056D1 RID: 22225 RVA: 0x0028470C File Offset: 0x0028290C
		private void OnSortAndFilterChangedCallback()
		{
			this.injury.UnselectPartByUsingMedicineItemType();
		}

		// Token: 0x04003B38 RID: 15160
		[SerializeField]
		private Injury injury;

		// Token: 0x04003B39 RID: 15161
		[SerializeField]
		private AttributeAndInjuryDynamic attributeAndInjury;

		// Token: 0x04003B3A RID: 15162
		[SerializeField]
		private CButton buttonCloseView;

		// Token: 0x04003B3B RID: 15163
		[SerializeField]
		private CButton buttonBack;

		// Token: 0x04003B3C RID: 15164
		[SerializeField]
		private CButton buttonAutoUseMedicine;

		// Token: 0x04003B3D RID: 15165
		[SerializeField]
		private TextMeshProUGUI loadText;

		// Token: 0x04003B3E RID: 15166
		[SerializeField]
		private TooltipInvoker loadOverflowTips;

		// Token: 0x04003B3F RID: 15167
		[SerializeField]
		private RectTransform focusMask;

		// Token: 0x04003B40 RID: 15168
		[SerializeField]
		private ItemListScroll itemScroll;

		// Token: 0x04003B41 RID: 15169
		private CharacterUsingMedicineDisplayData _usingMedicineDisplayData;

		// Token: 0x04003B42 RID: 15170
		private int _charId;

		// Token: 0x04003B43 RID: 15171
		private short _usingMedicineItemType;

		// Token: 0x04003B44 RID: 15172
		private ItemKey _selectedItemKey;

		// Token: 0x04003B45 RID: 15173
		private bool _canOpenEatDetail;
	}
}
