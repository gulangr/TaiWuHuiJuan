using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.ListStyleGeneralScroll;
using Game.Components.ListStyleGeneralScroll.CellContent;
using Game.Components.ListStyleGeneralScroll.Item;
using Game.Components.SortAndFilter;
using Game.Components.SortAndFilter.Book;
using Game.Views.CharacterMenu;
using Game.Views.Select;
using GameData.Common;
using GameData.Domains.CombatSkill;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.TaiwuEvent;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UISkillBreakPlate;
using UnityEngine;

namespace Game.Views.SectInteract.Wudang
{
	// Token: 0x020009E0 RID: 2528
	public class ViewWudangModifyBook : UIBase
	{
		// Token: 0x17000DA4 RID: 3492
		// (get) Token: 0x06007BED RID: 31725 RVA: 0x00399C3B File Offset: 0x00397E3B
		private int TaiwuId
		{
			get
			{
				return SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			}
		}

		// Token: 0x17000DA5 RID: 3493
		// (get) Token: 0x06007BEE RID: 31726 RVA: 0x00399C48 File Offset: 0x00397E48
		private int ExpCost
		{
			get
			{
				int expCost = 0;
				bool flag = !this._selected.Equals(ItemKey.Invalid);
				if (flag)
				{
					expCost = ((this._originPage[0] != this.outlinePageToggleGroup.GetActiveIndex()) ? this._data[this._selected].OutlinePageCostExp : 0);
					for (int i = 1; i < 6; i++)
					{
						bool flag2 = this.otherPageToggleGroup.Get(i - 1).isOn != (this._originPage[i] == 0);
						if (flag2)
						{
							expCost += this._data[this._selected].NormalPageCostExp;
						}
					}
				}
				return expCost;
			}
		}

		// Token: 0x06007BEF RID: 31727 RVA: 0x00399CFC File Offset: 0x00397EFC
		public override void OnInit(ArgumentBox argsBox)
		{
			ModifyBookSortAndFilterController sortAndFilterController = this._sortAndFilterController;
			this._currentFilterType = ((sortAndFilterController != null) ? sortAndFilterController.GetCurrentFilterType() : ESelectItemFilterType.All);
			this._needRebuildColumns = true;
		}

		// Token: 0x06007BF0 RID: 31728 RVA: 0x00399D1E File Offset: 0x00397F1E
		private void Awake()
		{
			this.InitSortAndFilter();
			this.InitScroll();
			this.InitBreakPanel();
			this.InitBtn();
			this._inited = true;
		}

		// Token: 0x06007BF1 RID: 31729 RVA: 0x00399D44 File Offset: 0x00397F44
		private void OnEnable()
		{
			this._selected = ItemKey.Invalid;
			this.UpdatePanel();
			this.RequestData();
		}

		// Token: 0x06007BF2 RID: 31730 RVA: 0x00399D60 File Offset: 0x00397F60
		public override void InitMonitorFieldIds()
		{
			this.MonitorFields = new List<UIBase.MonitorDataField>
			{
				new UIBase.MonitorDataField(4, 0, (ulong)((long)this.TaiwuId), new uint[]
				{
					66U
				})
			};
		}

		// Token: 0x06007BF3 RID: 31731 RVA: 0x00399D9C File Offset: 0x00397F9C
		public override void OnNotifyGameData(List<NotificationWrapper> notifications)
		{
			foreach (NotificationWrapper wrapper in notifications)
			{
				Notification notification = wrapper.Notification;
				bool flag = notification.Type == 0;
				if (flag)
				{
					DataUid uid = notification.Uid;
					bool flag2 = uid.DomainId == 4 && uid.DataId == 0 && uid.SubId1 == 66U;
					if (flag2)
					{
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._exp);
						this.UpdateInfo();
					}
				}
			}
		}

		// Token: 0x06007BF4 RID: 31732 RVA: 0x00399E50 File Offset: 0x00398050
		public override void QuickHide()
		{
			base.QuickHide();
			this.TriggerListenerBookModified(ItemKey.Invalid);
		}

		// Token: 0x06007BF5 RID: 31733 RVA: 0x00399E66 File Offset: 0x00398066
		private void TriggerListenerBookModified(ItemKey itemKey)
		{
			TaiwuEventDomainMethod.Call.SetListenerEventActionItemKeyArg("BookModified", "ModifiedBookItemKey", itemKey);
			TaiwuEventDomainMethod.Call.TriggerListener("BookModified", true);
		}

		// Token: 0x06007BF6 RID: 31734 RVA: 0x00399E86 File Offset: 0x00398086
		private void RequestData()
		{
			this._data.Clear();
			ItemDomainMethod.AsyncCall.GetTaiwuInventoryCombatSkillBooks(null, delegate(int offset, RawDataPool dataPool)
			{
				List<SkillBookModifyDisplayData> data = new List<SkillBookModifyDisplayData>();
				Serializer.Deserialize(dataPool, offset, ref data);
				foreach (SkillBookModifyDisplayData bookData in data)
				{
					this._data[bookData.ItemDisplayData.RealKey] = bookData;
				}
				this.RefreshList();
				this.Element.ShowAfterRefresh();
			});
		}

		// Token: 0x06007BF7 RID: 31735 RVA: 0x00399EA8 File Offset: 0x003980A8
		private void InitScroll()
		{
			this.scroll.OnRowClicked += this.OnClickItem;
			bool flag = this.rowTemplate != null;
			if (flag)
			{
				this._selectedRowTemplate = Object.Instantiate<RowItem>(this.rowTemplate, this.rowTemplate.transform.parent);
				this._selectedRowTemplate.gameObject.SetActive(false);
			}
			this.PrepareRowTemplateContainers();
			this.scroll.SetRowTemplate(this.rowTemplate);
			this.scroll.Init<ITradeableContent>(this.GenerateColumnDefinitions(), true, new Action<int, GameObject>(this.OnRenderItem), null);
			this.scroll.SetSortController(this._sortAndFilterController);
			this.cardScroll.OnRowClicked += this.OnClickItem;
			this.cardScroll.Init<ItemDisplayData>(this.GenerateColumnDefinitions(), true, new Action<int, GameObject>(this.OnCardItemRender), null);
			this.cardScroll.SetSortController(this._sortAndFilterController);
			this.switchToggleGroup.Init(-1);
			this.switchToggleGroup.OnActiveIndexChange += this.SwitchCardMode;
		}

		// Token: 0x06007BF8 RID: 31736 RVA: 0x00399FD0 File Offset: 0x003981D0
		private void InitSortAndFilter()
		{
			this._sortAndFilterController = new ModifyBookSortAndFilterController(this.sortAndFilter);
			this._sortAndFilterController.Init(new Action(this.OnSortAndFilterChanged), "SelectItem");
			this._sortAndFilterController.OnFilterTypeChanged += this.OnFilterTypeChanged;
		}

		// Token: 0x06007BF9 RID: 31737 RVA: 0x0039A024 File Offset: 0x00398224
		private void OnSortAndFilterChanged()
		{
			this.RefreshList();
		}

		// Token: 0x06007BFA RID: 31738 RVA: 0x0039A030 File Offset: 0x00398230
		private void OnFilterTypeChanged(ESelectItemFilterType newFilterType)
		{
			bool flag = this._currentFilterType != newFilterType;
			if (flag)
			{
				this._currentFilterType = newFilterType;
				this._needRebuildColumns = true;
			}
		}

		// Token: 0x06007BFB RID: 31739 RVA: 0x0039A060 File Offset: 0x00398260
		private void OnRenderItem(int index, GameObject obj)
		{
			TooltipInvoker tips = obj.GetComponent<RowItem>().TipDisplayer;
			ITradeableContent data = this._filteredData[index];
			tips.enabled = true;
			tips.Type = TooltipManager.ItemTypeToTipType[data.Key.ItemType];
			tips.NeedRefresh = true;
			tips.RuntimeParam = new ArgumentBox().SetObject("ItemData", data.Clone(-1));
			tips.RuntimeParam.Set("ShowPageInfo", data.Key.ItemType == 10);
			tips.RuntimeParam.Set("TemplateDataOnly", true);
			tips.RuntimeParam.Set("CharId", data.OwnerCharId);
		}

		// Token: 0x06007BFC RID: 31740 RVA: 0x0039A118 File Offset: 0x00398318
		private void OnClickItem(int index, RowItem rowItem)
		{
			ITradeableContent itemData = this._filteredData.GetOrDefault(index);
			bool flag = itemData == null;
			if (!flag)
			{
				this._selected = (itemData.RealKey.Equals(this._selected) ? ItemKey.Invalid : itemData.RealKey);
				this.scroll.SetSelectedRow(index);
				this.UpdatePanel();
				this.UpdateInfo();
				this.RefreshList();
			}
		}

		// Token: 0x06007BFD RID: 31741 RVA: 0x0039A188 File Offset: 0x00398388
		private void OnOutlinePageToggleChange(int togNew, int togOld)
		{
			bool flag = togNew >= 0;
			if (flag)
			{
				PracticeSlice slice = this.outlinePageToggleGroup.Get(togNew).GetComponent<PracticeSlice>();
				bool flag2 = slice;
				if (flag2)
				{
					slice.SetSelected(true);
				}
			}
			bool flag3 = togOld >= 0;
			if (flag3)
			{
				PracticeSlice sliceOld = this.outlinePageToggleGroup.Get(togOld).GetComponent<PracticeSlice>();
				bool flag4 = sliceOld;
				if (flag4)
				{
					sliceOld.SetSelected(false);
				}
			}
			this.UpdateInfo();
			this.RefreshList();
		}

		// Token: 0x06007BFE RID: 31742 RVA: 0x0039A20C File Offset: 0x0039840C
		private void OnOtherPageToggleChange(int togNew, int togOld)
		{
			int index = (togNew >= 0) ? togNew : togOld;
			bool flag = index < 0;
			if (!flag)
			{
				bool isOn = this.otherPageToggleGroup.Get(index).isOn;
				this.otherPageToggleGroup.Get(index).GetComponent<PracticeSlice>().SetSelected(isOn);
				bool flag2 = isOn;
				if (flag2)
				{
					this.otherPageToggleGroup.DeSelectWithoutNotify((int)this._reverseOtherPageDict[index]);
					this.otherPageToggleGroup.Get((int)this._reverseOtherPageDict[index]).GetComponent<PracticeSlice>().SetSelected(false);
				}
				this.UpdateInfo();
				this.RefreshList();
			}
		}

		// Token: 0x06007BFF RID: 31743 RVA: 0x0039A2A7 File Offset: 0x003984A7
		private void InitBtn()
		{
			this.btnConfirm.ClearAndAddListener(new Action(this.OnClickConfirm));
			this.btnClose.ClearAndAddListener(new Action(this.QuickHide));
		}

		// Token: 0x06007C00 RID: 31744 RVA: 0x0039A2DC File Offset: 0x003984DC
		private void OnClickConfirm()
		{
			CombatSkillItem config = CombatSkill.Instance[this._data[this._selected].TemplateId];
			string bookName = config.Name.SetGradeColor((int)config.Grade);
			UIElement.ConfirmDialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", new ConfirmDialogCmd
			{
				Title = LanguageKey.LK_CombatSkillModifyBook_Title_Outline.Tr(),
				ContentUpper = LanguageKey.LK_CombatSkill_ModifyBookConfirm_Tip.TrFormat(bookName),
				ContentLower = LanguageKey.LK_Building_ConfirmOperate.TrFormat(LanguageKey.LK_CombatSkill_ModifyBook_Change.Tr()),
				ConfirmDialogCost = new List<ConfirmDialogCost>
				{
					new ConfirmDialogCost
					{
						Type = EConfirmDialogCostType.ActionPoint,
						ValueCost = 10,
						ValueHave = SingletonObject.getInstance<TimeManager>().GetRemainingActionPointConvertToDays()
					},
					new ConfirmDialogCost
					{
						Type = EConfirmDialogCostType.Exp,
						ValueCost = this.ExpCost,
						ValueHave = this._exp
					}
				},
				Yes = new Action(this.OnConfirm)
			}));
			UIManager.Instance.MaskUI(UIElement.ConfirmDialog);
		}

		// Token: 0x06007C01 RID: 31745 RVA: 0x0039A40C File Offset: 0x0039860C
		private void OnConfirm()
		{
			sbyte behavior = (sbyte)this.outlinePageToggleGroup.GetActiveIndex();
			List<sbyte> directions = new List<sbyte>();
			for (int i = 0; i < 5; i++)
			{
				directions.Add(this.otherPageToggleGroup.Get(i).isOn ? 0 : 1);
			}
			ItemDomainMethod.AsyncCall.SetCombatSkillBookPage(null, this._selected, behavior, directions, delegate(int offset, RawDataPool pool)
			{
				bool res = false;
				Serializer.Deserialize(pool, offset, ref res);
				bool flag = res;
				if (flag)
				{
					this.TriggerListenerBookModified(this._selected);
					UIManager.Instance.HideUI(this.Element);
				}
				else
				{
					this.TriggerListenerBookModified(ItemKey.Invalid);
				}
			});
		}

		// Token: 0x06007C02 RID: 31746 RVA: 0x0039A478 File Offset: 0x00398678
		private void InitBreakPanel()
		{
			List<CToggle> outlineTogList = this.outlinePageToggleGroup.GetAll();
			sbyte index = 0;
			while ((int)index < outlineTogList.Count)
			{
				CToggle tog = outlineTogList[(int)index];
				PracticeSlice slice = tog.GetComponent<PracticeSlice>();
				slice.AutoRotateNameBg();
				slice.RefreshStyle();
				slice.SetNum(((int)(index + 1)).ToString().SetColor("ffffff"));
				slice.SetNameLabel(string.Format("ui9_text_combat_skill_outline_{0}_cn", index));
				PracticeSkillPlatePageUtils.RefreshOutlinePageTip(slice.GetPageTip(), index, true, false);
				index += 1;
			}
			this.outlinePageToggleGroup.Init(-1);
			this.outlinePageToggleGroup.OnActiveIndexChange += this.OnOutlinePageToggleChange;
			List<CToggle> pageTogList = this.otherPageToggleGroup.GetAll();
			byte index2 = 0;
			while ((int)index2 < pageTogList.Count)
			{
				CToggle tog2 = pageTogList[(int)index2];
				PracticeSlice slice2 = tog2.GetComponent<PracticeSlice>();
				bool isDirect = this.CheckIsDirectByToggleIndex((int)index2);
				byte actualIndex = (byte)this.GetActualIndex((int)index2);
				bool flag = !isDirect;
				if (flag)
				{
					this._reverseOtherPageDict[(int)index2] = actualIndex;
					this._reverseOtherPageDict[(int)actualIndex] = index2;
				}
				slice2.AutoRotateNameBg();
				slice2.RefreshStyle();
				slice2.SetNum(this.GetPageId((int)index2).ToString().SetColor(isDirect ? "81ddff" : "ffb7b7"));
				slice2.SetNameLabel(isDirect ? string.Format("ui9_text_combat_skill_direct_{0}_cn", actualIndex) : string.Format("ui9_text_combat_skill_reverse_{0}_cn", actualIndex));
				index2 += 1;
			}
			this.otherPageToggleGroup.Init();
			this.otherPageToggleGroup.OnActiveIndexChange += this.OnOtherPageToggleChange;
		}

		// Token: 0x06007C03 RID: 31747 RVA: 0x0039A650 File Offset: 0x00398850
		private void UpdatePanel()
		{
			this.RefreshCardMode();
			bool flag = this._selected.Equals(ItemKey.Invalid);
			if (flag)
			{
				this.DeactivatePanel();
			}
			else
			{
				SkillBookModifyDisplayData data = this._data[this._selected];
				sbyte outlinePageType = SkillBookStateHelper.GetOutlinePageType(data.PageTypes);
				bool flag2 = outlinePageType >= 0;
				if (flag2)
				{
					this.outlinePageToggleGroup.Set((int)outlinePageType, false);
				}
				else
				{
					this.outlinePageToggleGroup.DeSelect(false);
				}
				this._originPage[0] = (int)outlinePageType;
				for (byte i = 1; i < 6; i += 1)
				{
					sbyte direction = SkillBookStateHelper.GetNormalPageType(data.PageTypes, i);
					int directIndex = (int)(i - 1);
					int reverseIndex = (int)(i + 5 - 1);
					int index = (direction == 0) ? directIndex : reverseIndex;
					sbyte state = SkillBookStateHelper.GetPageIncompleteState(data.PageIncompleteState, i);
					string icon = string.Format("ui9_icon_item_book_reading_status_{0}", state);
					PracticeSlice directPageSlice = this.otherPageToggleGroup.Get(directIndex).GetComponent<PracticeSlice>();
					PracticeSlice reversePageSlice = this.otherPageToggleGroup.Get(reverseIndex).GetComponent<PracticeSlice>();
					this.directPageStatus.GetChild((int)(i - 1)).GetComponent<CImage>().SetSprite(icon, false, null);
					this.reversePageStatus.GetChild((int)(i - 1)).GetComponent<CImage>().SetSprite(icon, false, null);
					this.otherPageToggleGroup.Select(index, false);
					this._originPage[(int)i] = (int)direction;
					TooltipInvoker directTip = directPageSlice.GetPageTip();
					TooltipInvoker directTipNoPage = directPageSlice.GetNoPageTip();
					TooltipInvoker reverseTip = reversePageSlice.GetPageTip();
					TooltipInvoker reverseTipNoPage = reversePageSlice.GetNoPageTip();
					this.RefreshOtherPageTips((int)(i - 1), directTip, data.TemplateId, true);
					this.RefreshOtherPageTips((int)(i - 1), directTipNoPage, data.TemplateId, true);
					this.RefreshOtherPageTips((int)(i - 1), reverseTip, data.TemplateId, false);
					this.RefreshOtherPageTips((int)(i - 1), reverseTipNoPage, data.TemplateId, false);
				}
				this.ActivatePanel();
				CombatSkillDomainMethod.AsyncCall.GetCombatSkillDisplayDataOnce(null, this.TaiwuId, data.TemplateId, delegate(int offset, RawDataPool pool)
				{
					CombatSkillDisplayData skillData = null;
					Serializer.Deserialize(pool, offset, ref skillData);
					this.centerCombatSkill.Set(skillData);
					this.centerCombatSkill.gameObject.SetActive(true);
				});
				ItemDomainMethod.AsyncCall.GetSkillBookPagesInfo(this, this._selected, delegate(int offset, RawDataPool dataPool)
				{
					SkillBookPageDisplayData displayData = null;
					Serializer.Deserialize(dataPool, offset, ref displayData);
					bool flag3 = displayData.CombatSkillAllReadingProgress == null;
					if (flag3)
					{
						displayData.CombatSkillAllReadingProgress = new sbyte[15];
					}
					for (int j = 0; j < 5; j++)
					{
						byte direct = CombatSkillStateHelper.GetPageInternalIndex(-1, 0, (byte)(j + 1));
						byte reverse = CombatSkillStateHelper.GetPageInternalIndex(-1, 1, (byte)(j + 1));
						this.outlinePageToggleGroup.Get(j).GetComponent<PracticeSlice>().SetReadingProgress((int)displayData.CombatSkillAllReadingProgress[j]);
						this.otherPageToggleGroup.Get(j).GetComponent<PracticeSlice>().SetReadingProgress((int)displayData.CombatSkillAllReadingProgress[(int)direct]);
						this.otherPageToggleGroup.Get(j + 5).GetComponent<PracticeSlice>().SetReadingProgress((int)displayData.CombatSkillAllReadingProgress[(int)reverse]);
					}
				});
			}
		}

		// Token: 0x06007C04 RID: 31748 RVA: 0x0039A874 File Offset: 0x00398A74
		private void UpdateInfo()
		{
			int actionPointCost = this._selected.Equals(ItemKey.Invalid) ? 0 : 10;
			int expCost = this.ExpCost;
			this.exp.text = string.Format("{0}/{1}", this._exp.ToString().SetColor((expCost > this._exp) ? "brightred" : "brightblue"), expCost);
			int remain = SingletonObject.getInstance<TimeManager>().GetRemainingActionPointConvertToDays();
			this.actionPoint.text = string.Format("{0}/{1}", remain.ToString().SetColor((actionPointCost > remain) ? "brightred" : "brightblue"), actionPointCost);
			this.btnConfirm.interactable = (!this._selected.Equals(ItemKey.Invalid) && expCost > 0 && expCost <= this._exp && actionPointCost <= remain);
		}

		// Token: 0x06007C05 RID: 31749 RVA: 0x0039A95C File Offset: 0x00398B5C
		private void ActivatePanel()
		{
			this.skillPanel.SetActive(true);
			this.notSelected.SetActive(false);
			this.pageStatus.SetActive(true);
			for (int i = 0; i < this.outlinePageToggleGroup.Count(); i++)
			{
				PracticeSlice slice = this.outlinePageToggleGroup.Get(i).GetComponent<PracticeSlice>();
				slice.ShowReadingProgress();
				slice.SetNameByActive(true);
				slice.SetInteractable(true);
				slice.SetPageShow(true);
			}
			for (int j = 0; j < this.otherPageToggleGroup.Count(); j++)
			{
				PracticeSlice slice2 = this.otherPageToggleGroup.Get(j).GetComponent<PracticeSlice>();
				slice2.ShowReadingProgress();
				slice2.SetNameByActive(true);
				slice2.SetInteractable(true);
				slice2.SetPageShow(true);
			}
		}

		// Token: 0x06007C06 RID: 31750 RVA: 0x0039AA34 File Offset: 0x00398C34
		private void DeactivatePanel()
		{
			this.skillPanel.SetActive(false);
			this.notSelected.SetActive(true);
			this.pageStatus.SetActive(false);
			this.centerCombatSkill.gameObject.SetActive(false);
			for (int i = 0; i < this.outlinePageToggleGroup.Count(); i++)
			{
				PracticeSlice slice = this.outlinePageToggleGroup.Get(i).GetComponent<PracticeSlice>();
				slice.HideReadingProgress();
				slice.SetNameByActive(false);
				slice.SetInteractable(false);
				slice.SetPageShow(false);
			}
			this.outlinePageToggleGroup.DeSelect(false);
			for (int j = 0; j < this.otherPageToggleGroup.Count(); j++)
			{
				PracticeSlice slice2 = this.otherPageToggleGroup.Get(j).GetComponent<PracticeSlice>();
				slice2.HideReadingProgress();
				slice2.SetNameByActive(false);
				slice2.SetInteractable(false);
				slice2.SetPageShow(false);
			}
			this.otherPageToggleGroup.DeSelectAll(false);
		}

		// Token: 0x06007C07 RID: 31751 RVA: 0x0039AB38 File Offset: 0x00398D38
		private bool CheckIsDirectByToggleIndex(int index)
		{
			return index < 5;
		}

		// Token: 0x06007C08 RID: 31752 RVA: 0x0039AB50 File Offset: 0x00398D50
		private sbyte GetDirection(int index)
		{
			return this.CheckIsDirectByToggleIndex(index) ? 0 : 1;
		}

		// Token: 0x06007C09 RID: 31753 RVA: 0x0039AB70 File Offset: 0x00398D70
		private int GetActualIndex(int index)
		{
			return (int)((byte)(this.CheckIsDirectByToggleIndex(index) ? index : (index - 5)));
		}

		// Token: 0x06007C0A RID: 31754 RVA: 0x0039AB94 File Offset: 0x00398D94
		private byte GetPageId(int index)
		{
			return (byte)(this.GetActualIndex(index) + 1);
		}

		// Token: 0x06007C0B RID: 31755 RVA: 0x0039ABB0 File Offset: 0x00398DB0
		private void RefreshOtherPageTips(int index, TooltipInvoker tip, short skillId, bool isDirect)
		{
			tip.Type = TipType.GeneralLines;
			if (tip.RuntimeParam == null)
			{
				tip.RuntimeParam = new ArgumentBox();
			}
			ViewWudangModifyBook.<>c__DisplayClass75_0 CS$<>8__locals1;
			CS$<>8__locals1.tipParam = tip.RuntimeParam;
			CS$<>8__locals1.lineCount = 0;
			string otherPageName = LocalStringManager.Get(string.Format("LK_CombatSkill_{0}_Page_{1}", isDirect ? "Direct" : "Reverse", index)).SetColor(isDirect ? "81ddff" : "ffb7b7");
			string titleName = LocalStringManager.GetFormat(LanguageKey.LK_Skill_Break_Tip_OtherPage_Title, otherPageName);
			CS$<>8__locals1.tipParam.Set("Title", titleName);
			GeneralLineData effectTitle = new GeneralLineData
			{
				Type = 1,
				Args = new List<string>
				{
					LocalStringManager.Get(LanguageKey.LK_Skill_Break_Tip_OtherPage_EffectTitle)
				}
			};
			ViewWudangModifyBook.<RefreshOtherPageTips>g__AddNode|75_0(effectTitle, ref CS$<>8__locals1);
			int pageIndex = isDirect ? (index + 5) : (index + 5 + 5);
			List<string> descList = EasyPool.Get<List<string>>();
			PageEffectHelper.GenerateNormalPageEffects(skillId, pageIndex - 5, descList);
			foreach (string desc in descList)
			{
				GeneralLineData effectLine = new GeneralLineData
				{
					Type = 5,
					Args = new List<string>
					{
						desc
					},
					ExtraArgs = new List<object>
					{
						24
					}
				};
				ViewWudangModifyBook.<RefreshOtherPageTips>g__AddNode|75_0(effectLine, ref CS$<>8__locals1);
			}
			CS$<>8__locals1.tipParam.Set("LineCount", CS$<>8__locals1.lineCount);
			tip.Refresh(false, -1);
			EasyPool.Free<List<string>>(descList);
		}

		// Token: 0x06007C0C RID: 31756 RVA: 0x0039AD58 File Offset: 0x00398F58
		private IEnumerable<ColumnDefinition> GenerateColumnDefinitions()
		{
			ColumnDefinition<ITradeableContent, ITradeableContent> columnDefinition = new ColumnDefinition<ITradeableContent, ITradeableContent>();
			columnDefinition.LayoutOption = new LayoutOption
			{
				MinWidth = 400f,
				FlexibleWidth = 1f,
				PreferredWidth = 440f,
				Priority = 1
			};
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_Item.Tr());
			columnDefinition.CellDataGenerator = ((ITradeableContent data) => data);
			columnDefinition.SortId = 0;
			yield return columnDefinition;
			ColumnDefinition<ITradeableContent, string> columnDefinition2 = new ColumnDefinition<ITradeableContent, string>();
			columnDefinition2.LayoutOption = new LayoutOption
			{
				MinWidth = 89f,
				FlexibleWidth = 1f,
				PreferredWidth = 100f,
				Priority = 1
			};
			columnDefinition2.TableHeadLabel = (() => LanguageKey.LK_Durability.Tr());
			columnDefinition2.CellDataGenerator = delegate(ITradeableContent data)
			{
				string durabilityStr = data.Durability.ToString().SetColor((data.Durability == data.MaxDurability) ? "f7f7f7" : "d62b35");
				string maxDurabilityStr = ("/" + data.MaxDurability.ToString()).SetColor("b9b6b1");
				return durabilityStr + maxDurabilityStr;
			};
			columnDefinition2.SortId = 18;
			yield return columnDefinition2;
			ColumnDefinition<ITradeableContent, BookPageInfoAndStateData> columnDefinition3 = new ColumnDefinition<ITradeableContent, BookPageInfoAndStateData>();
			columnDefinition3.LayoutOption = new LayoutOption
			{
				MinWidth = 400f,
				FlexibleWidth = 1f,
				PreferredWidth = 480f,
				Priority = 1
			};
			columnDefinition3.TableHeadLabel = (() => LanguageKey.LK_SelectItem_Column_BookReadingInfo.Tr());
			columnDefinition3.CellDataGenerator = ((ITradeableContent data) => new BookPageInfoAndStateData(data, this.GetBookPageStates(data.RealKey)));
			columnDefinition3.SortId = -1;
			yield return columnDefinition3;
			yield break;
		}

		// Token: 0x06007C0D RID: 31757 RVA: 0x0039AD68 File Offset: 0x00398F68
		private IEnumerable<ColumnDefinition> GetCurrentColumnDefinitions()
		{
			return this.GenerateColumnDefinitions();
		}

		// Token: 0x06007C0E RID: 31758 RVA: 0x0039AD70 File Offset: 0x00398F70
		private bool[] GetBookPageStates(ItemKey itemKey)
		{
			bool flag = !itemKey.Equals(this._selected);
			bool[] result;
			if (flag)
			{
				result = this._noneStateBookPages;
			}
			else
			{
				bool[] res = new bool[6];
				res[0] = (this._originPage[0] != this.outlinePageToggleGroup.GetActiveIndex());
				for (int i = 1; i < 6; i++)
				{
					res[i] = (this.otherPageToggleGroup.Get(i - 1).isOn != (this._originPage[i] == 0));
				}
				result = res;
			}
			return result;
		}

		// Token: 0x06007C0F RID: 31759 RVA: 0x0039ADFC File Offset: 0x00398FFC
		private void PrepareRowTemplateContainers()
		{
			Transform containerRoot = this.rowTemplate.ContainerRoot;
			for (int i = containerRoot.childCount - 1; i >= 0; i--)
			{
				Transform child = containerRoot.GetChild(i);
				bool flag = child.GetComponent<RowCellContainer>() != null;
				if (flag)
				{
					Object.Destroy(child.gameObject);
				}
			}
			List<ColumnDefinition> columnDefinitions = this.GetCurrentColumnDefinitions().ToList<ColumnDefinition>();
			foreach (ColumnDefinition columnDef in columnDefinitions)
			{
				bool flag2 = columnDef is ColumnDefinition<ITradeableContent, ITradeableContent>;
				RowCellContainer containerTemplate;
				if (flag2)
				{
					containerTemplate = this.itemIconAndNameCellContainer;
				}
				else
				{
					bool flag3 = columnDef is ColumnDefinition<ITradeableContent, BookPageInfoAndStateData>;
					if (flag3)
					{
						containerTemplate = this.bookPageInfoCellContainer;
					}
					else
					{
						containerTemplate = this.singleTextCellContainer;
					}
				}
				bool flag4 = containerTemplate == null;
				if (flag4)
				{
					Debug.LogError(string.Format("[ViewSelectItem] containerTemplate is null for column: {0}", columnDef.TableHeadLabel));
				}
				else
				{
					RowCellContainer container = Object.Instantiate<RowCellContainer>(containerTemplate, containerRoot);
					container.gameObject.SetActive(true);
				}
			}
		}

		// Token: 0x06007C10 RID: 31760 RVA: 0x0039AF2C File Offset: 0x0039912C
		private bool IsItemSelected(int index, object rowData)
		{
			bool flag = this._selected.Equals(ItemKey.Invalid);
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				ITradeableContent itemData = rowData as ITradeableContent;
				bool flag2 = itemData == null;
				result = (!flag2 && itemData.RealKey.Equals(this._selected));
			}
			return result;
		}

		// Token: 0x06007C11 RID: 31761 RVA: 0x0039AF84 File Offset: 0x00399184
		private void RefreshList()
		{
			bool flag = !this._inited;
			if (!flag)
			{
				bool needRebuild = this._needRebuildColumns;
				bool flag2 = needRebuild;
				if (flag2)
				{
					this._needRebuildColumns = false;
					this.RebuildColumnStructure();
				}
				this.ApplySortAndFilter();
				bool isCardMode = this._isCardMode;
				if (isCardMode)
				{
					this.cardScroll.RowSelectedProvider = new Func<int, object, bool>(this.IsItemSelected);
					this.cardScroll.SetData<ITradeableContent>(this._filteredData, -1);
				}
				else
				{
					this.scroll.RowSelectedProvider = new Func<int, object, bool>(this.IsItemSelected);
					this.scroll.SetData<ITradeableContent>(this._filteredData, -1);
				}
			}
		}

		// Token: 0x06007C12 RID: 31762 RVA: 0x0039B02D File Offset: 0x0039922D
		private void RebuildColumnStructure()
		{
			this.PrepareRowTemplateContainers();
			this.scroll.ClearInfinityScrollCache();
			this.scroll.Init<ITradeableContent>(this.GetCurrentColumnDefinitions(), true, null, null);
			this.scroll.SetSortController(this._sortAndFilterController);
		}

		// Token: 0x06007C13 RID: 31763 RVA: 0x0039B06C File Offset: 0x0039926C
		private void ApplySortAndFilter()
		{
			this._filteredData.Clear();
			bool flag = this._sortAndFilterController == null || this._data == null;
			if (!flag)
			{
				Func<ITradeableContent, bool> filter = this._sortAndFilterController.GenerateFilter();
				List<ITradeableContent> allData = new List<ITradeableContent>();
				foreach (SkillBookModifyDisplayData item in this._data.Values)
				{
					ItemDisplayData displayData = item.ItemDisplayData;
					allData.Add(displayData);
					bool flag2 = filter(displayData);
					if (flag2)
					{
						this._filteredData.Add(displayData);
					}
				}
				this._sortAndFilterController.AfterFilter(allData);
				Comparison<ITradeableContent> comparer = this._sortAndFilterController.GenerateComparer(this._filteredData);
				this._filteredData.Sort(comparer);
			}
		}

		// Token: 0x06007C14 RID: 31764 RVA: 0x0039B15C File Offset: 0x0039935C
		private void SwitchCardMode(int previousIndex, int currentIndex)
		{
			this._isCardMode = (currentIndex == 0);
			this.RefreshCardMode();
			this.RefreshList();
		}

		// Token: 0x06007C15 RID: 31765 RVA: 0x0039B177 File Offset: 0x00399377
		private void RefreshCardMode()
		{
			this.cardScroll.gameObject.SetActive(this._isCardMode);
			this.scroll.gameObject.SetActive(!this._isCardMode);
		}

		// Token: 0x06007C16 RID: 31766 RVA: 0x0039B1AC File Offset: 0x003993AC
		private void OnCardItemRender(int index, GameObject rowObject)
		{
			bool flag = index < 0 || index >= this._filteredData.Count;
			if (!flag)
			{
				ITradeableContent rowData = this._filteredData[index];
				RowItemLine rowItemLine = rowObject.GetComponent<RowItemLine>();
				RowItemMain rowItemMain = rowItemLine.GetComponentInChildren<RowItemMain>();
				rowItemMain.SetData(rowData);
				rowItemLine.Set(rowItemMain, true);
			}
		}

		// Token: 0x06007C1C RID: 31772 RVA: 0x0039B444 File Offset: 0x00399644
		[CompilerGenerated]
		internal static void <RefreshOtherPageTips>g__AddNode|75_0(GeneralLineData lineData, ref ViewWudangModifyBook.<>c__DisplayClass75_0 A_1)
		{
			ArgumentBox tipParam = A_1.tipParam;
			string format = "LineData{0}";
			int num = A_1.lineCount + 1;
			A_1.lineCount = num;
			tipParam.SetObject(string.Format(format, num), lineData);
		}

		// Token: 0x04005E16 RID: 24086
		[Header("排序筛选")]
		[SerializeField]
		private SortAndFilter sortAndFilter;

		// Token: 0x04005E17 RID: 24087
		[SerializeField]
		private ListStyleGeneralScroll scroll;

		// Token: 0x04005E18 RID: 24088
		[Header("行模板配置")]
		[SerializeField]
		private RowItem rowTemplate;

		// Token: 0x04005E19 RID: 24089
		[SerializeField]
		private RowCellContainer singleTextCellContainer;

		// Token: 0x04005E1A RID: 24090
		[SerializeField]
		private RowCellContainer itemIconAndNameCellContainer;

		// Token: 0x04005E1B RID: 24091
		[SerializeField]
		private RowCellContainer bookPageInfoCellContainer;

		// Token: 0x04005E1C RID: 24092
		[SerializeField]
		private CToggleGroup outlinePageToggleGroup;

		// Token: 0x04005E1D RID: 24093
		[SerializeField]
		private CToggleGroupMultiSelect otherPageToggleGroup;

		// Token: 0x04005E1E RID: 24094
		[SerializeField]
		private CharacterMenuCombatSkillItem centerCombatSkill;

		// Token: 0x04005E1F RID: 24095
		[SerializeField]
		private Transform directPageStatus;

		// Token: 0x04005E20 RID: 24096
		[SerializeField]
		private Transform reversePageStatus;

		// Token: 0x04005E21 RID: 24097
		[SerializeField]
		private GameObject pageStatus;

		// Token: 0x04005E22 RID: 24098
		[SerializeField]
		private GameObject skillPanel;

		// Token: 0x04005E23 RID: 24099
		[SerializeField]
		private GameObject notSelected;

		// Token: 0x04005E24 RID: 24100
		[SerializeField]
		private TextMeshProUGUI exp;

		// Token: 0x04005E25 RID: 24101
		[SerializeField]
		private TextMeshProUGUI actionPoint;

		// Token: 0x04005E26 RID: 24102
		[SerializeField]
		private CButton btnConfirm;

		// Token: 0x04005E27 RID: 24103
		[SerializeField]
		private CButton btnClose;

		// Token: 0x04005E28 RID: 24104
		[Header("图标模式相关组件")]
		[SerializeField]
		private CardStyleGeneralScroll cardScroll;

		// Token: 0x04005E29 RID: 24105
		[SerializeField]
		private CToggleGroup switchToggleGroup;

		// Token: 0x04005E2A RID: 24106
		private readonly Dictionary<ItemKey, SkillBookModifyDisplayData> _data = new Dictionary<ItemKey, SkillBookModifyDisplayData>();

		// Token: 0x04005E2B RID: 24107
		private ItemKey _selected = ItemKey.Invalid;

		// Token: 0x04005E2C RID: 24108
		private readonly int[] _originPage = new int[6];

		// Token: 0x04005E2D RID: 24109
		private const string Outline = "ffffff";

		// Token: 0x04005E2E RID: 24110
		private const string Direct = "81ddff";

		// Token: 0x04005E2F RID: 24111
		private const string Reverse = "ffb7b7";

		// Token: 0x04005E30 RID: 24112
		private const float DurabilityWidth = 89f;

		// Token: 0x04005E31 RID: 24113
		private const float PreferredDurabilityWidth = 100f;

		// Token: 0x04005E32 RID: 24114
		private const float NameWidth = 400f;

		// Token: 0x04005E33 RID: 24115
		private const float PreferredNameWidth = 440f;

		// Token: 0x04005E34 RID: 24116
		private const float DescWidth = 400f;

		// Token: 0x04005E35 RID: 24117
		private const float PreferredDescWidth = 480f;

		// Token: 0x04005E36 RID: 24118
		private const int ActionPointCost = 10;

		// Token: 0x04005E37 RID: 24119
		private ModifyBookSortAndFilterController _sortAndFilterController;

		// Token: 0x04005E38 RID: 24120
		private RowItem _selectedRowTemplate;

		// Token: 0x04005E39 RID: 24121
		private ESelectItemFilterType _currentFilterType = ESelectItemFilterType.All;

		// Token: 0x04005E3A RID: 24122
		private bool _needRebuildColumns;

		// Token: 0x04005E3B RID: 24123
		private readonly List<ITradeableContent> _filteredData = new List<ITradeableContent>();

		// Token: 0x04005E3C RID: 24124
		private readonly Dictionary<int, byte> _reverseOtherPageDict = new Dictionary<int, byte>();

		// Token: 0x04005E3D RID: 24125
		private readonly bool[] _noneStateBookPages = new bool[6];

		// Token: 0x04005E3E RID: 24126
		private int _exp = 0;

		// Token: 0x04005E3F RID: 24127
		private bool _inited;

		// Token: 0x04005E40 RID: 24128
		private bool _isCardMode;
	}
}
