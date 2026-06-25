using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using EasyButtons;
using FrameWork;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using Game.Components.SortAndFilter;
using Game.Components.SortAndFilter.CombatSkill;
using Game.Views.Migrate;
using GameData.Domains.CombatSkill;
using GameData.Domains.TaiwuEvent;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.Select
{
	// Token: 0x020007B2 RID: 1970
	public class ViewSelectSkill : UIBase
	{
		// Token: 0x17000BAF RID: 2991
		// (get) Token: 0x06005FEB RID: 24555 RVA: 0x002C0770 File Offset: 0x002BE970
		public bool IsTaiwu
		{
			get
			{
				return this._charId == SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			}
		}

		// Token: 0x17000BB0 RID: 2992
		// (get) Token: 0x06005FEC RID: 24556 RVA: 0x002C0784 File Offset: 0x002BE984
		// (set) Token: 0x06005FED RID: 24557 RVA: 0x002C078C File Offset: 0x002BE98C
		public CombatSkillSortAndFilterController SortAndFilter { get; private set; }

		// Token: 0x17000BB1 RID: 2993
		// (get) Token: 0x06005FEE RID: 24558 RVA: 0x002C0795 File Offset: 0x002BE995
		private int Height
		{
			get
			{
				return this._selectCount ? 948 : 1086;
			}
		}

		// Token: 0x17000BB2 RID: 2994
		// (get) Token: 0x06005FEF RID: 24559 RVA: 0x002C07AB File Offset: 0x002BE9AB
		// (set) Token: 0x06005FF0 RID: 24560 RVA: 0x002C07B3 File Offset: 0x002BE9B3
		private int CurCount
		{
			get
			{
				return this._curCount;
			}
			set
			{
				this._curCount = value;
			}
		}

		// Token: 0x06005FF1 RID: 24561 RVA: 0x002C07BD File Offset: 0x002BE9BD
		[Button("武学筛选按钮")]
		public void Set(int value)
		{
			this.SortAndFilter.SetDropdownOption(EFilterLine.First.ToInt(), 0, value);
			this.OnSkillListChanged();
		}

		// Token: 0x06005FF2 RID: 24562 RVA: 0x002C07E0 File Offset: 0x002BE9E0
		public override void OnInit(ArgumentBox argsBox)
		{
			this._isNeedDefaultSelectCombatSkill = true;
			this.GetArgs(argsBox);
			this.titleText.SetText(LocalStringManager.Get(this._selectCount ? LanguageKey.LK_PracticeCombatSkill_Title : LanguageKey.LK_Skill_Select), true);
			this.confirmBtn.interactable = false;
			this.RefreshAdvanceDaysInfo();
			this.NeedDataListenerId = true;
			this.NeedWaitData = true;
			UIElement element = this.Element;
			element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(this.OnListenerIdReady));
			this.raycasterMask.SetActive(true);
		}

		// Token: 0x06005FF3 RID: 24563 RVA: 0x002C0879 File Offset: 0x002BEA79
		private void OnListenerIdReady()
		{
			this.RequestData();
		}

		// Token: 0x06005FF4 RID: 24564 RVA: 0x002C0883 File Offset: 0x002BEA83
		public void RequestData()
		{
			CombatSkillDomainMethod.AsyncCall.GetCombatSkillDisplayData(this, this._charId, this._combatSkillIdList, delegate(int offset, RawDataPool pool)
			{
				Serializer.Deserialize(pool, offset, ref this._combatSkillDisplayDataList);
				this._combatSkillDataDict.Clear();
				if (this._combatSkillDisplayDataList == null)
				{
					this._combatSkillDisplayDataList = new List<CombatSkillDisplayData>();
				}
				foreach (CombatSkillDisplayData data in this._combatSkillDisplayDataList)
				{
					this._combatSkillDataDict.Add(data.TemplateId, data);
				}
				bool flag = this._combatSkillEquipType != -1;
				if (flag)
				{
					this.SortAndFilter.SetDropdownOption(EFilterLine.First.ToInt(), 0, (int)this._combatSkillEquipType);
				}
				this.OnSkillListChanged();
				this.RefreshConfirmButton();
				this.Element.ShowAfterRefresh();
				this.raycasterMask.SetActive(false);
			});
		}

		// Token: 0x06005FF5 RID: 24565 RVA: 0x002C08A8 File Offset: 0x002BEAA8
		private void GetArgs(ArgumentBox argsBox)
		{
			argsBox.Get("CharId", out this._charId);
			argsBox.Get<Action<sbyte, short>>("Callback", out this._onSelected);
			argsBox.Get<Action<sbyte, short, int>>("Callback2", out this._onSelected2);
			argsBox.Get<Action<CombatSkillDisplayData>>("CallbackCombatSkill", out this._onCombatSkillSelected);
			argsBox.Get("ShowCombatSkill", out this._selectCombatSkill);
			argsBox.Get<DialogCmd>("ConfirmDialog", out this._confirmDialogCmd);
			argsBox.Get("IsShowNeiLiFinish", out this.isShowNeiLiFinish);
			argsBox.Get("ExitByAction", out this._exitByAction);
			argsBox.Get("IsNeedDefaultSelectCombatSkill", out this._isNeedDefaultSelectCombatSkill);
			argsBox.Get("PracticeCombatSkillCostActionPoint", out this._practiceCombatSkillCostActionPoint);
			argsBox.Get("ShowSelectCount", out this._selectCount);
			argsBox.Get("AtSettlement", out this._atSettlement);
			argsBox.Get("IsTaiwuVillageBuilding", out this._isTaiwuVillageBuilding);
			argsBox.Get("CurrLocationOrganizationTemplateId", out this._currLocationOrganizationTemplateId);
			bool flag = !this._selectCombatSkill;
			if (flag)
			{
				throw new Exception("ShowCombatSkill is false is not allowed");
			}
			argsBox.Get<List<short>>("CombatSkillIdList", out this._combatSkillIdList);
			argsBox.Get<List<short>>("UnselectableCombatSkillList", out this._unselectableCombatSkillList);
			argsBox.Get("ShowNone", out this._addEmptyItem);
			bool flag2 = !argsBox.Get("PrevCombatSkillId", out this._prevSelectedCombatSkillId);
			if (flag2)
			{
				this._prevSelectedCombatSkillId = -1;
			}
			this._currSelectedCombatSkillId = (this._addEmptyItem ? this._prevSelectedCombatSkillId : -1);
			bool flag3 = !argsBox.Get("CombatSkillType", out this._combatSkillEquipType);
			if (flag3)
			{
				this._combatSkillEquipType = -1;
			}
		}

		// Token: 0x06005FF6 RID: 24566 RVA: 0x002C0A58 File Offset: 0x002BEC58
		private void Awake()
		{
			this.SortAndFilter = new CombatSkillSortAndFilterController(this.commonSortAndFilter, true, EFilterType.Common);
			this.SortAndFilter.Init(new Action(this.OnSkillListChanged), "SelectSkillCombatSkillSortAndFilter");
			this.combatSkillScroll.OnItemRender += this.OnItemRender;
		}

		// Token: 0x06005FF7 RID: 24567 RVA: 0x002C0AAF File Offset: 0x002BECAF
		private void OnEnable()
		{
			GEvent.Add(UiEvents.TopUiChanged, new GEvent.Callback(this.OnTopUiChanged));
		}

		// Token: 0x06005FF8 RID: 24568 RVA: 0x002C0ACA File Offset: 0x002BECCA
		private void OnTopUiChanged(ArgumentBox argBox)
		{
			this.RequestData();
		}

		// Token: 0x06005FF9 RID: 24569 RVA: 0x002C0AD4 File Offset: 0x002BECD4
		private void OnDisable()
		{
			GEvent.Remove(UiEvents.TopUiChanged, new GEvent.Callback(this.OnTopUiChanged));
		}

		// Token: 0x06005FFA RID: 24570 RVA: 0x002C0AF0 File Offset: 0x002BECF0
		private void OnItemRender(int index, GameObject obj)
		{
			CombatSkillDisplayData combatSkillDisplayData = this._filteredCombatSkillList[index];
			SelectSkillItem selectSkillItem = obj.GetComponent<SelectSkillItem>();
			selectSkillItem.Set(index, combatSkillDisplayData, this._currSelectedCombatSkillId == combatSkillDisplayData.TemplateId);
		}

		// Token: 0x06005FFB RID: 24571 RVA: 0x002C0B2C File Offset: 0x002BED2C
		private void OnSkillListChanged()
		{
			List<CombatSkillDisplayData> sourceList = this._combatSkillDisplayDataList ?? new List<CombatSkillDisplayData>();
			CombatSkillSortAndFilterController sortAndFilter = this.SortAndFilter;
			Func<IFilterableCombatSkill, bool> func;
			if ((func = ((sortAndFilter != null) ? sortAndFilter.GenerateFilter() : null)) == null && (func = ViewSelectSkill.<>c.<>9__61_0) == null)
			{
				func = (ViewSelectSkill.<>c.<>9__61_0 = ((IFilterableCombatSkill _) => true));
			}
			Func<IFilterableCombatSkill, bool> filter = func;
			CombatSkillSortAndFilterController sortAndFilter2 = this.SortAndFilter;
			Comparison<IFilterableCombatSkill> comparer = (sortAndFilter2 != null) ? sortAndFilter2.GenerateComparer(sourceList) : null;
			this._filteredCombatSkillList.Clear();
			foreach (CombatSkillDisplayData item in sourceList)
			{
				bool flag = filter(item);
				if (flag)
				{
					this._filteredCombatSkillList.Add(item);
				}
			}
			bool flag2 = comparer != null;
			if (flag2)
			{
				this._filteredCombatSkillList.Sort(comparer);
			}
			bool flag3 = this._isNeedDefaultSelectCombatSkill && this._currSelectedCombatSkillId < 0 && this._filteredCombatSkillList.Count > 0;
			if (flag3)
			{
				this._isNeedDefaultSelectCombatSkill = false;
				this._currSelectedCombatSkillId = this._filteredCombatSkillList[0].TemplateId;
			}
			bool flag4 = this.combatSkillScroll != null;
			if (flag4)
			{
				this.combatSkillScroll.SetDataCount(this._filteredCombatSkillList.Count);
			}
			this.noContent.gameObject.SetActive(this._filteredCombatSkillList.Count == 0);
			CombatSkillDisplayData currSelectSkillDisplayData = null;
			for (int i = 0; i < this._filteredCombatSkillList.Count; i++)
			{
				bool flag5 = this._filteredCombatSkillList[i].TemplateId == this._currSelectedCombatSkillId;
				if (flag5)
				{
					currSelectSkillDisplayData = this._filteredCombatSkillList[i];
				}
			}
			bool flag6 = currSelectSkillDisplayData != null;
			if (flag6)
			{
				this.selectSkillInfo.Refresh(currSelectSkillDisplayData.TemplateId, currSelectSkillDisplayData.CharId, false, false);
			}
			else
			{
				this.selectSkillInfo.Clear();
			}
			CombatSkillSortAndFilterController sortAndFilter3 = this.SortAndFilter;
			if (sortAndFilter3 != null)
			{
				sortAndFilter3.AfterFilter(sourceList);
			}
		}

		// Token: 0x06005FFC RID: 24572 RVA: 0x002C0D40 File Offset: 0x002BEF40
		protected override void OnClick(Transform btn)
		{
			string btnName = btn.name;
			bool flag = btnName == "ConfirmBtn";
			if (flag)
			{
				bool exitByAction = this._exitByAction;
				if (exitByAction)
				{
					sbyte groupType = 1;
					Action<sbyte, short> onSelected = this._onSelected;
					if (onSelected != null)
					{
						onSelected(groupType, this._currSelectedCombatSkillId);
					}
					Action<sbyte, short, int> onSelected2 = this._onSelected2;
					if (onSelected2 != null)
					{
						onSelected2(groupType, this._currSelectedCombatSkillId, this._curCount);
					}
					Action<CombatSkillDisplayData> onCombatSkillSelected = this._onCombatSkillSelected;
					if (onCombatSkillSelected != null)
					{
						onCombatSkillSelected(this._combatSkillDataDict[this._currSelectedCombatSkillId]);
					}
					UIManager.Instance.HideUI(this.Element);
				}
				else
				{
					bool flag2 = this._confirmDialogCmd != null;
					if (flag2)
					{
						this._confirmDialogCmd.Yes = new Action(this.<OnClick>g__SelectAction|62_0);
						UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", this._confirmDialogCmd));
						UIManager.Instance.MaskUI(UIElement.Dialog);
					}
					else
					{
						this.<OnClick>g__SelectAction|62_0();
					}
				}
			}
			else
			{
				bool flag3 = btnName == "ButtonCloseView";
				if (flag3)
				{
					UIManager.Instance.HideUI(this.Element);
					Action<sbyte, short> onSelected3 = this._onSelected;
					if (onSelected3 != null)
					{
						onSelected3(-1, -2);
					}
					Action<sbyte, short, int> onSelected4 = this._onSelected2;
					if (onSelected4 != null)
					{
						onSelected4(-1, -2, -1);
					}
					this.CancelHideTrigger();
				}
			}
		}

		// Token: 0x06005FFD RID: 24573 RVA: 0x002C0EA3 File Offset: 0x002BF0A3
		private void CancelHideTrigger()
		{
			TaiwuEventDomainMethod.Call.SetListenerEventActionIntArg("FinishSelectCombatSkill", "SelectCombatSkillTemplateId", -1);
			TaiwuEventDomainMethod.Call.TriggerListener("FinishSelectCombatSkill", false);
		}

		// Token: 0x06005FFE RID: 24574 RVA: 0x002C0EC3 File Offset: 0x002BF0C3
		public override void QuickHide()
		{
			AudioManager.Instance.PlaySound("ui_default_cancel", false, false);
			this.CancelHideTrigger();
			base.QuickHide();
		}

		// Token: 0x06005FFF RID: 24575 RVA: 0x002C0EE8 File Offset: 0x002BF0E8
		private void RefreshConfirmButton()
		{
			bool skillSelected = this._currSelectedCombatSkillId != -1 || this._addEmptyItem;
			this.confirmBtn.interactable = (this._selectCount ? (skillSelected && this._curCount > 0 && this._atSettlement) : skillSelected);
			TooltipInvoker tip = this.confirmBtn.GetComponent<TooltipInvoker>();
			tip.enabled = !skillSelected;
		}

		// Token: 0x06006000 RID: 24576 RVA: 0x002C0F4C File Offset: 0x002BF14C
		internal void OnSkillItemClicked(int index, CombatSkillDisplayData combatSkillDisplayData)
		{
			short templateId = combatSkillDisplayData.TemplateId;
			bool flag = templateId == this._currSelectedCombatSkillId;
			if (flag)
			{
				this._currSelectedCombatSkillId = -1;
			}
			else
			{
				this._prevSelectedCombatSkillId = this._currSelectedCombatSkillId;
				this._currSelectedCombatSkillId = templateId;
			}
			this.RefreshConfirmButton();
			this.OnSkillListChanged();
		}

		// Token: 0x06006001 RID: 24577 RVA: 0x002C0F9C File Offset: 0x002BF19C
		public void RefreshAdvanceDaysInfo()
		{
			ViewSelectSkill.<>c__DisplayClass67_0 CS$<>8__locals1 = new ViewSelectSkill.<>c__DisplayClass67_0();
			CS$<>8__locals1.<>4__this = this;
			this.selectCountHolder.gameObject.SetActive(this._selectCount);
			this.combatSkillScrollHolder.sizeDelta = new Vector2(this.combatSkillScrollHolder.sizeDelta.x, (float)this.Height);
			bool flag = !this._selectCount;
			if (!flag)
			{
				CS$<>8__locals1.leftDays = SingletonObject.getInstance<TimeManager>().GetRemainingActionPointConvertToDays();
				this.slider.maxValue = (float)(CS$<>8__locals1.leftDays / (this._practiceCombatSkillCostActionPoint / 10));
				this.slider.value = (this.slider.minValue = 0f);
				int targetCount = Math.Max((int)this.slider.maxValue - 1, 0);
				CommonUtils.PrepareEnoughChildren(this.lineHolder, this.lineHolder.GetChild(0).gameObject, targetCount, null);
				for (int i = 0; i < targetCount; i++)
				{
					Transform line = this.lineHolder.GetChild(i);
					((RectTransform)line).anchoredPosition = new Vector2(this.lineHolder.sizeDelta.x / this.slider.maxValue * (float)(i + 1), 0f);
				}
				CS$<>8__locals1.<RefreshAdvanceDaysInfo>g__UpdateActionPointText|3();
				this.slider.onValueChanged.AddListener(delegate(float value)
				{
					base.<RefreshAdvanceDaysInfo>g__UpdateActionPointText|3();
				});
				this.lessBtn.onClick.ResetListener(delegate()
				{
					CS$<>8__locals1.<>4__this.slider.value = Mathf.Max(0f, CS$<>8__locals1.<>4__this.slider.value - 1f);
				});
				this.moreBtn.onClick.ResetListener(delegate()
				{
					CS$<>8__locals1.<>4__this.slider.value = Mathf.Min(CS$<>8__locals1.<>4__this.slider.maxValue, CS$<>8__locals1.<>4__this.slider.value + 1f);
				});
			}
		}

		// Token: 0x06006004 RID: 24580 RVA: 0x002C1288 File Offset: 0x002BF488
		[CompilerGenerated]
		private void <OnClick>g__SelectAction|62_0()
		{
			sbyte groupType = 1;
			TaiwuEventDomainMethod.Call.SetListenerEventActionIntArg("FinishSelectCombatSkill", "SelectCombatSkillTemplateId", (int)this._currSelectedCombatSkillId);
			TaiwuEventDomainMethod.Call.TriggerListener("FinishSelectCombatSkill", true);
			UIManager.Instance.HideUI(this.Element);
			Action<sbyte, short> onSelected = this._onSelected;
			if (onSelected != null)
			{
				onSelected(groupType, this._currSelectedCombatSkillId);
			}
			Action<sbyte, short, int> onSelected2 = this._onSelected2;
			if (onSelected2 != null)
			{
				onSelected2(groupType, this._currSelectedCombatSkillId, this._curCount);
			}
		}

		// Token: 0x04004265 RID: 16997
		[SerializeField]
		private InfinityScroll combatSkillScroll;

		// Token: 0x04004266 RID: 16998
		[SerializeField]
		private SortAndFilter commonSortAndFilter;

		// Token: 0x04004267 RID: 16999
		[SerializeField]
		private CButton confirmBtn;

		// Token: 0x04004268 RID: 17000
		[SerializeField]
		private RectTransform combatSkillScrollHolder;

		// Token: 0x04004269 RID: 17001
		[SerializeField]
		private RectTransform selectCountHolder;

		// Token: 0x0400426A RID: 17002
		[SerializeField]
		private CSlider slider;

		// Token: 0x0400426B RID: 17003
		[SerializeField]
		private CButton lessBtn;

		// Token: 0x0400426C RID: 17004
		[SerializeField]
		private CButton moreBtn;

		// Token: 0x0400426D RID: 17005
		[SerializeField]
		private TextMeshProUGUI currentCountLabel;

		// Token: 0x0400426E RID: 17006
		[SerializeField]
		private TextMeshProUGUI currentTimeLabel;

		// Token: 0x0400426F RID: 17007
		[SerializeField]
		private TextMeshProUGUI limitTimeLabel;

		// Token: 0x04004270 RID: 17008
		[SerializeField]
		private GameObject noContent;

		// Token: 0x04004271 RID: 17009
		[SerializeField]
		private RectTransform lineHolder;

		// Token: 0x04004272 RID: 17010
		[SerializeField]
		private SelectSkillInfo selectSkillInfo;

		// Token: 0x04004273 RID: 17011
		[SerializeField]
		private GameObject raycasterMask;

		// Token: 0x04004274 RID: 17012
		[SerializeField]
		private TextMeshProUGUI titleText;

		// Token: 0x04004275 RID: 17013
		private int _charId;

		// Token: 0x04004276 RID: 17014
		private Action<sbyte, short> _onSelected;

		// Token: 0x04004277 RID: 17015
		private Action<sbyte, short, int> _onSelected2;

		// Token: 0x04004278 RID: 17016
		private Action<CombatSkillDisplayData> _onCombatSkillSelected;

		// Token: 0x04004279 RID: 17017
		private bool _selectCombatSkill;

		// Token: 0x0400427A RID: 17018
		private bool isShowNeiLiFinish = false;

		// Token: 0x0400427B RID: 17019
		private DialogCmd _confirmDialogCmd;

		// Token: 0x0400427C RID: 17020
		private List<short> _combatSkillIdList = new List<short>();

		// Token: 0x0400427D RID: 17021
		private List<short> _unselectableCombatSkillList = new List<short>();

		// Token: 0x0400427E RID: 17022
		private readonly List<CombatSkillDisplayData> _filteredCombatSkillList = new List<CombatSkillDisplayData>();

		// Token: 0x0400427F RID: 17023
		private short _prevSelectedCombatSkillId;

		// Token: 0x04004280 RID: 17024
		private short _currSelectedCombatSkillId;

		// Token: 0x04004281 RID: 17025
		private readonly Dictionary<short, CombatSkillDisplayData> _combatSkillDataDict = new Dictionary<short, CombatSkillDisplayData>();

		// Token: 0x04004282 RID: 17026
		private List<CombatSkillDisplayData> _combatSkillDisplayDataList;

		// Token: 0x04004283 RID: 17027
		private bool _addEmptyItem;

		// Token: 0x04004284 RID: 17028
		private bool _isNeedDefaultSelectCombatSkill;

		// Token: 0x04004285 RID: 17029
		private bool _exitByAction = false;

		// Token: 0x04004286 RID: 17030
		private sbyte _combatSkillEquipType;

		// Token: 0x04004287 RID: 17031
		private bool _selectCount;

		// Token: 0x04004288 RID: 17032
		private bool _atSettlement;

		// Token: 0x04004289 RID: 17033
		private bool _isTaiwuVillageBuilding;

		// Token: 0x0400428A RID: 17034
		private int _practiceCombatSkillCostActionPoint;

		// Token: 0x0400428B RID: 17035
		private short _currLocationOrganizationTemplateId;

		// Token: 0x0400428D RID: 17037
		private int _curCount;
	}
}
