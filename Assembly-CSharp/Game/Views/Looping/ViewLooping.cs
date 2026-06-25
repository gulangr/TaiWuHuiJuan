using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Config;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using EasyButtons;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.ListStyleGeneralScroll;
using Game.Components.SortAndFilter;
using Game.Components.SortAndFilter.CombatSkill;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.CombatSkill;
using GameData.Domains.Extra;
using GameData.Domains.Global;
using GameData.Domains.Taiwu;
using GameData.Domains.Taiwu.Display;
using GameData.Domains.TaiwuEvent;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.Views.Looping
{
	// Token: 0x02000981 RID: 2433
	public class ViewLooping : UIBase
	{
		// Token: 0x060074BD RID: 29885 RVA: 0x00365D61 File Offset: 0x00363F61
		public static void SetOnShowAction(Action<ViewLooping> action)
		{
			ViewLooping._onShowAction = action;
		}

		// Token: 0x060074BE RID: 29886 RVA: 0x00365D6A File Offset: 0x00363F6A
		public static void JumpToStrategy()
		{
			ViewLooping.SetOnShowAction(delegate(ViewLooping v)
			{
				v.CurrentMode = ViewLooping.ViewLoopingMode.SelectStrategy;
			});
		}

		// Token: 0x060074BF RID: 29887 RVA: 0x00365D94 File Offset: 0x00363F94
		public static void JumpToSelectLoopingSkill(Func<CombatSkillDisplayDataCharacterMenuListItem, bool> customFilter = null)
		{
			ViewLooping.SetOnShowAction(delegate(ViewLooping v)
			{
				v._customFilter = customFilter;
				v.CurrentMode = ViewLooping.ViewLoopingMode.SelectLoopingSkill;
			});
		}

		// Token: 0x060074C0 RID: 29888 RVA: 0x00365DC4 File Offset: 0x00363FC4
		public static void JumpToSelectReferenceSkill(Func<CombatSkillDisplayDataCharacterMenuListItem, bool> customFilter = null)
		{
			ViewLooping.SetOnShowAction(delegate(ViewLooping v)
			{
				v._customFilter = customFilter;
				v.CurrentMode = ViewLooping.ViewLoopingMode.SelectReferenceSkill;
			});
		}

		// Token: 0x060074C1 RID: 29889 RVA: 0x00365DF1 File Offset: 0x00363FF1
		public static void JumpToIncompleteLoopingSkill()
		{
			ViewLooping.JumpToSelectLoopingSkill((CombatSkillDisplayDataCharacterMenuListItem data) => data.ObtainedNeili < data.MaxObtainableNeili);
		}

		// Token: 0x060074C2 RID: 29890 RVA: 0x00365E19 File Offset: 0x00364019
		public static void JumpToGetExtraNeiliSkill()
		{
			ViewLooping.SetOnShowAction(delegate(ViewLooping v)
			{
				int index = v._taiwuExtraNeiliAllocationProgress.Items.FindIndex((int x) => LoopingCommonUtils.CalcExtraNeiliAllocationFromProgress(x) < (int)GlobalConfig.Instance.MaxExtraNeiliAllocation);
				bool flag = index >= 0;
				if (flag)
				{
					ViewLooping.JumpToSelectLoopingSkill(delegate(CombatSkillDisplayDataCharacterMenuListItem data)
					{
						bool result;
						if (CombatSkill.Instance[data.TemplateId].ExtraNeiliAllocationProgress[index] <= 0)
						{
							sbyte[] extraNeiliAllocationProgress = CombatSkill.Instance[data.TemplateId].ExtraNeiliAllocationProgress;
							result = (extraNeiliAllocationProgress[extraNeiliAllocationProgress.Length - 1] > 0);
						}
						else
						{
							result = true;
						}
						return result;
					});
				}
			});
		}

		// Token: 0x060074C3 RID: 29891 RVA: 0x00365E41 File Offset: 0x00364041
		public unsafe static void JumpToFiveElementTransferSkill()
		{
			ViewLooping.SetOnShowAction(delegate(ViewLooping v)
			{
				NeiliProportionOfFiveElements NeiliProportion = UIElement.Looping.UiBaseAs<ViewLooping>()._neiliDisplayData.NeiliProportion;
				int srcFiveElementType = 0;
				sbyte max = -1;
				sbyte max2 = -1;
				int max1Idx = 0;
				int max2Idx = 0;
				for (int i = 0; i < 5; i++)
				{
					bool flag = *(ref NeiliProportion.Items.FixedElementField + i) > max;
					if (flag)
					{
						max2 = max;
						max2Idx = max1Idx;
						max = *(ref NeiliProportion.Items.FixedElementField + i);
						max1Idx = i;
					}
					else
					{
						bool flag2 = *(ref NeiliProportion.Items.FixedElementField + i) > max2;
						if (flag2)
						{
							max2 = *(ref NeiliProportion.Items.FixedElementField + i);
							max2Idx = i;
						}
					}
				}
				srcFiveElementType = max2Idx;
				ViewLooping.JumpToSelectLoopingSkill(delegate(CombatSkillDisplayDataCharacterMenuListItem data)
				{
					sbyte destType = data.FiveElementDestTypeWhileLooping;
					bool flag3 = destType < 0;
					bool result;
					if (flag3)
					{
						result = false;
					}
					else
					{
						sbyte transferType = data.FiveElementTransferTypeWhileLooping;
						if (!true)
						{
						}
						sbyte b;
						switch (transferType)
						{
						case 0:
							b = FiveElementsType.Countered[(int)destType];
							break;
						case 1:
							b = FiveElementsType.Countering[(int)destType];
							break;
						case 2:
							b = FiveElementsType.Produced[(int)destType];
							break;
						default:
							b = FiveElementsType.Producing[(int)destType];
							break;
						}
						if (!true)
						{
						}
						sbyte actualSrcType = b;
						result = ((int)actualSrcType == srcFiveElementType);
					}
					return result;
				});
			});
		}

		// Token: 0x17000D2B RID: 3371
		// (get) Token: 0x060074C4 RID: 29892 RVA: 0x00365E69 File Offset: 0x00364069
		private int TaiwuCharId
		{
			get
			{
				return SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			}
		}

		// Token: 0x17000D2C RID: 3372
		// (get) Token: 0x060074C5 RID: 29893 RVA: 0x00365E75 File Offset: 0x00364075
		private bool IsCombatSkillDisplayDataReady
		{
			get
			{
				return this._combatSkillDisplayDataDict.Count == this._learnedSkillList.Count;
			}
		}

		// Token: 0x17000D2D RID: 3373
		// (get) Token: 0x060074C6 RID: 29894 RVA: 0x00365E8F File Offset: 0x0036408F
		private bool HasEvent
		{
			get
			{
				return this._loopingEventSkillIdList != null && this._loopingEventSkillIdList.Contains(this._loopingNeigong);
			}
		}

		// Token: 0x17000D2E RID: 3374
		// (get) Token: 0x060074C7 RID: 29895 RVA: 0x00365EAD File Offset: 0x003640AD
		public bool HasEmptyStrategySlot
		{
			get
			{
				return this._taiwuQiArtStrategyList.Count((QiArtStrategyDisplayData x) => x.TemplateId >= 0 && x.ExpireTime >= 0) < this.currentStrategyItems.Length;
			}
		}

		// Token: 0x17000D2F RID: 3375
		// (get) Token: 0x060074C8 RID: 29896 RVA: 0x00365EE4 File Offset: 0x003640E4
		// (set) Token: 0x060074C9 RID: 29897 RVA: 0x00365F1E File Offset: 0x0036411E
		private CombatSkillSortAndFilterController SortAndFilterController
		{
			get
			{
				ViewLooping.ViewLoopingMode currentMode = this._currentMode;
				if (!true)
				{
				}
				CombatSkillSortAndFilterController result;
				if (currentMode != ViewLooping.ViewLoopingMode.SelectReferenceSkill)
				{
					result = this._loopingSortAndFilterController;
				}
				else
				{
					result = this._referenceSortAndFilterController;
				}
				if (!true)
				{
				}
				return result;
			}
			set
			{
				this._loopingSortAndFilterController = value;
			}
		}

		// Token: 0x060074CA RID: 29898 RVA: 0x00365F28 File Offset: 0x00364128
		public override void OnInit(ArgumentBox argsBox)
		{
			this._usedStrategies.Clear();
			argsBox.Get<LoopingViewDisplayData>("LoopingViewDisplayData", out this._displayData);
			this.UpdateData();
			this.Refresh();
			this.RequestNeiliData();
			this.RequestNeiliPageData();
			ViewLooping.ViewLoopingMode mode;
			this.CurrentMode = (argsBox.Get<ViewLooping.ViewLoopingMode>("CurrentMode", out mode) ? mode : ViewLooping.ViewLoopingMode.Normal);
			bool flag = ViewLooping._onShowAction != null;
			if (flag)
			{
				Action<ViewLooping> action = ViewLooping._onShowAction;
				ViewLooping._onShowAction = null;
				action(this);
			}
		}

		// Token: 0x060074CB RID: 29899 RVA: 0x00365FAC File Offset: 0x003641AC
		private void RequestData()
		{
			TaiwuDomainMethod.AsyncCall.GetLoopingViewDisplayData(this, delegate(int offset, RawDataPool dataPool)
			{
				Serializer.Deserialize(dataPool, offset, ref this._displayData);
				this.UpdateData();
				this.Refresh();
				this.RequestNeiliData();
			});
		}

		// Token: 0x060074CC RID: 29900 RVA: 0x00365FC4 File Offset: 0x003641C4
		private void RequestNeiliData()
		{
			bool flag = this._displayData.LoopingNeigong < 0;
			if (!flag)
			{
				TaiwuDomainMethod.AsyncCall.RequestTaiwuNeiliProportionDisplayData(null, delegate(int offset, RawDataPool pool)
				{
					this._neiliDisplayData = new TaiwuNeiliProportionDisplayData();
					Serializer.Deserialize(pool, offset, ref this._neiliDisplayData);
					this.RefreshNeiliProportion();
				});
			}
		}

		// Token: 0x060074CD RID: 29901 RVA: 0x00365FF9 File Offset: 0x003641F9
		private void RequestNeiliPageData()
		{
			CharacterDomainMethod.AsyncCall.GetCharacterDisplayDataForNeiliPage(null, this.TaiwuCharId, delegate(int offset, RawDataPool dataPool)
			{
				Serializer.Deserialize(dataPool, offset, ref this._neiliPageData);
				bool flag = this._neiliPageData == null;
				if (!flag)
				{
					this.SetHighlightNode(this._neiliPageData.NeiliType);
				}
			});
		}

		// Token: 0x060074CE RID: 29902 RVA: 0x00366015 File Offset: 0x00364215
		private void Awake()
		{
			this.InitSortAndFilter();
		}

		// Token: 0x060074CF RID: 29903 RVA: 0x00366020 File Offset: 0x00364220
		private void OnEnable()
		{
			GlobalDomainMethod.Call.InvokeGuidingTrigger(77);
			bool flag = this.loopingSkillToggle != null;
			if (flag)
			{
				this.loopingSkillToggle.onValueChanged.RemoveListener(new UnityAction<bool>(this.OnLoopingSkillToggleChanged));
				this.loopingSkillToggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnLoopingSkillToggleChanged));
				this.SyncLoopingSkillToggleFromMode();
			}
		}

		// Token: 0x060074D0 RID: 29904 RVA: 0x0036608C File Offset: 0x0036428C
		private void OnLoopingSkillToggleChanged(bool isOn)
		{
			if (isOn)
			{
				this.CurrentMode = ViewLooping.ViewLoopingMode.SelectLoopingSkill;
			}
			else
			{
				bool flag = this.CurrentMode == ViewLooping.ViewLoopingMode.SelectLoopingSkill;
				if (flag)
				{
					this.CurrentMode = ViewLooping.ViewLoopingMode.Normal;
				}
			}
		}

		// Token: 0x060074D1 RID: 29905 RVA: 0x003660C0 File Offset: 0x003642C0
		private void SyncLoopingSkillToggleFromMode()
		{
			bool flag = this.loopingSkillToggle == null;
			if (!flag)
			{
				bool shouldBeOn = this.CurrentMode == ViewLooping.ViewLoopingMode.SelectLoopingSkill;
				bool flag2 = this.loopingSkillToggle.isOn != shouldBeOn;
				if (flag2)
				{
					this.loopingSkillToggle.SetIsOnWithoutNotify(shouldBeOn);
				}
			}
		}

		// Token: 0x060074D2 RID: 29906 RVA: 0x0036610C File Offset: 0x0036430C
		private void RefreshScorll()
		{
			this.RefreshListStructure();
			this.scroll.OnRowClicked -= this.ScrollOnClicked;
			this.scroll.OnRowClicked += this.ScrollOnClicked;
			this.scroll.RowSelectedProvider = new Func<int, object, bool>(this.ScrollSelectedProvider);
		}

		// Token: 0x060074D3 RID: 29907 RVA: 0x0036616C File Offset: 0x0036436C
		private void ScrollOnClicked(int index, RowItem rowItem)
		{
			short templateId = this._filteredSkillList[index].TemplateId;
			bool flag = this.CurrentMode != ViewLooping.ViewLoopingMode.SelectReferenceSkill;
			if (flag)
			{
				bool flag2 = templateId == this._loopingNeigong;
				if (flag2)
				{
					this.ShowDialogToChangeLoopingNeigong(-1);
				}
				else
				{
					this.ShowDialogToChangeLoopingNeigong(templateId);
				}
			}
			else
			{
				this.OnReferenceSelectClickCombatSkill(templateId);
			}
		}

		// Token: 0x060074D4 RID: 29908 RVA: 0x003661D0 File Offset: 0x003643D0
		private bool ScrollSelectedProvider(int index, object rowData)
		{
			CombatSkillDisplayDataCharacterMenuListItem data = rowData as CombatSkillDisplayDataCharacterMenuListItem;
			bool flag = this.CurrentMode != ViewLooping.ViewLoopingMode.SelectReferenceSkill;
			bool result;
			if (flag)
			{
				result = (this._loopingNeigong == data.TemplateId);
			}
			else
			{
				result = this._referenceSkillList.Contains(data.TemplateId);
			}
			return result;
		}

		// Token: 0x060074D5 RID: 29909 RVA: 0x00366220 File Offset: 0x00364420
		private void InitSortAndFilter()
		{
			this._loopingSortAndFilterController = new CombatSkillSortAndFilterController(this.loopingSortAndFilter, true, EFilterType.Looping);
			this._loopingSortAndFilterController.Init(new Action(this.RefreshListData), "Looping");
			this.scroll.SetSortController(this._loopingSortAndFilterController);
			this._referenceSortAndFilterController = new CombatSkillSortAndFilterController(this.referenceSortAndFilter, true, EFilterType.Reference);
			this._referenceSortAndFilterController.Init(new Action(this.RefreshListData), "Reference");
		}

		// Token: 0x060074D6 RID: 29910 RVA: 0x003662A0 File Offset: 0x003644A0
		private void RefreshListData()
		{
			CombatSkillSortAndFilterController sortAndFilterController = this.SortAndFilterController;
			Func<IFilterableCombatSkill, bool> func;
			if ((func = ((sortAndFilterController != null) ? sortAndFilterController.GenerateFilter() : null)) == null && (func = ViewLooping.<>c.<>9__118_0) == null)
			{
				func = (ViewLooping.<>c.<>9__118_0 = ((IFilterableCombatSkill _) => true));
			}
			Func<IFilterableCombatSkill, bool> filter = func;
			IEnumerable<IFilterableCombatSkill> filtered = this._skillList.Where(filter);
			this._filteredSkillList.Clear();
			IFilterableCombatSkill[] iFilterableCombatSkills = (filtered as IFilterableCombatSkill[]) ?? filtered.ToArray<IFilterableCombatSkill>();
			for (int i = 0; i < iFilterableCombatSkills.Length; i++)
			{
				CombatSkillDisplayDataCharacterMenuListItem item = iFilterableCombatSkills[i] as CombatSkillDisplayDataCharacterMenuListItem;
				bool flag = this.CurrentMode == ViewLooping.ViewLoopingMode.SelectReferenceSkill && item.TemplateId == this._loopingNeigong;
				if (!flag)
				{
					bool flag2 = this.CurrentMode == ViewLooping.ViewLoopingMode.SelectLoopingSkill && this._referenceSkillList.Contains(item.TemplateId);
					if (!flag2)
					{
						this._filteredSkillList.Add(item);
					}
				}
			}
			CombatSkillSortAndFilterController sortAndFilterController2 = this.SortAndFilterController;
			SortStateData sortData = (sortAndFilterController2 != null) ? sortAndFilterController2.SortAndFilterState.SortData : null;
			List<SortItemState> list = (sortData != null) ? sortData.ItemStates : null;
			bool hasActiveSort = list != null && list.Count > 0;
			bool flag3 = hasActiveSort;
			if (flag3)
			{
				Comparison<IFilterableCombatSkill> comparer = this.SortAndFilterController.GenerateComparer(this._filteredSkillList);
				bool flag4 = comparer != null;
				if (flag4)
				{
					this._filteredSkillList.Sort(comparer);
				}
			}
			else
			{
				this.ApplyDefaultSort();
			}
			bool flag5 = this._customFilter != null;
			if (flag5)
			{
				List<CombatSkillDisplayDataCharacterMenuListItem> tmp = (from s in this._filteredSkillList
				where this._customFilter(s)
				select s).ToList<CombatSkillDisplayDataCharacterMenuListItem>();
				this._filteredSkillList.ClearAndAddRange(tmp);
				this._customFilter = null;
			}
			this.scroll.SetData<CombatSkillDisplayDataCharacterMenuListItem>(this._filteredSkillList, -1);
			CombatSkillSortAndFilterController loopingSortAndFilterController = this._loopingSortAndFilterController;
			if (loopingSortAndFilterController != null)
			{
				loopingSortAndFilterController.AfterFilter(this._skillList);
			}
			CombatSkillSortAndFilterController referenceSortAndFilterController = this._referenceSortAndFilterController;
			if (referenceSortAndFilterController != null)
			{
				referenceSortAndFilterController.AfterFilter(this._skillList);
			}
		}

		// Token: 0x060074D7 RID: 29911 RVA: 0x00366490 File Offset: 0x00364690
		private void RefreshListStructure()
		{
			IEnumerable<ColumnDefinition> columnDefinitions = this.GenerateColumnDefinitions();
			this.scroll.SetRowTemplate((this.CurrentMode != ViewLooping.ViewLoopingMode.SelectReferenceSkill) ? this.loopingSkillRowTemplate : this.referenceSkillRowTemplate);
			this.scroll.Init<CombatSkillDisplayDataCharacterMenuListItem>(columnDefinitions, true, null, null);
		}

		// Token: 0x060074D8 RID: 29912 RVA: 0x003664D8 File Offset: 0x003646D8
		private void OnDisable()
		{
			bool flag = this.loopingSkillToggle != null;
			if (flag)
			{
				this.loopingSkillToggle.onValueChanged.RemoveListener(new UnityAction<bool>(this.OnLoopingSkillToggleChanged));
			}
			bool inGuiding = SingletonObject.getInstance<TutorialChapterModel>().InGuiding;
			if (inGuiding)
			{
				TaiwuEventDomainMethod.Call.TriggerListener(EventActionKey.DefValue.TutorialExitViewLooping, false);
			}
			this.CurrentMode = ViewLooping.ViewLoopingMode.Normal;
			UIManager.Instance.SetEscHandler(null);
			this._modified = false;
			this._customFilter = null;
			ViewLooping._onShowAction = null;
		}

		// Token: 0x060074D9 RID: 29913 RVA: 0x0036655C File Offset: 0x0036475C
		private void UpdateData()
		{
			this._curConcentration = this._displayData.CurConcentration;
			this._maxConcentration = this._displayData.MaxConcentration;
			this._currNeili = this._displayData.CurrNeili;
			this._loopingNeigong = this._displayData.LoopingNeigong;
			this._learnedSkillList.Clear();
			bool flag = this._displayData.LearnedSkillList != null;
			if (flag)
			{
				this._learnedSkillList.AddRange(this._displayData.LearnedSkillList);
			}
			this._skillList.Clear();
			this._skillList.AddRange(this._displayData.CombatSkillDisplayDataList);
			this._combatSkillDisplayDataDict.Clear();
			bool flag2 = this._displayData.CombatSkillDisplayDataList != null;
			if (flag2)
			{
				foreach (CombatSkillDisplayDataCharacterMenuListItem data in this._displayData.CombatSkillDisplayDataList)
				{
					this._combatSkillDisplayDataDict[data.TemplateId] = data;
				}
			}
			this._referenceSkillList.Clear();
			bool flag3 = this._displayData.ReferenceSkillList != null;
			if (flag3)
			{
				this._referenceSkillList.AddRange(this._displayData.ReferenceSkillList);
			}
			this._referenceSkillSlotUnlockStates = this._displayData.ReferenceSkillSlotUnlockStates;
			this._taiwuExtraNeiliAllocationProgress = ((this._displayData.TaiwuExtraNeiliAllocationProgress.Items != null) ? this._displayData.TaiwuExtraNeiliAllocationProgress : IntList.Create());
			this._extraNeiliPerLoop = new ValueTuple<int, int>(this._displayData.ExtraNeiliPerLoopMin, this._displayData.ExtraNeiliPerLoopMax);
			this._extraNeiliAllocationPerLoop = ((this._displayData.ExtraNeiliAllocationPerLoop.Items != null) ? this._displayData.ExtraNeiliAllocationPerLoop : IntList.Create());
			this._taiwuQiArtStrategyList = (this._displayData.TaiwuQiArtStrategyList ?? new List<QiArtStrategyDisplayData>());
			this._availableStrategies = ((this._displayData.AvailableStrategies.Items != null) ? this._displayData.AvailableStrategies : SByteList.Create());
			this._loopingEventSkillIdList = (this._displayData.LoopingEventSkillIdList ?? new List<short>());
			this._loopInLifeSkillCombatCount = this._displayData.LoopInLifeSkillCombatCount;
			this._loopInCombatCount = this._displayData.LoopInCombatCount;
			this._selectedStrategySlot = this._taiwuQiArtStrategyList.FindIndex((QiArtStrategyDisplayData x) => x.TemplateId < 0);
		}

		// Token: 0x060074DA RID: 29914 RVA: 0x003667EC File Offset: 0x003649EC
		private void Refresh()
		{
			ViewLooping.<>c__DisplayClass122_0 CS$<>8__locals1 = new ViewLooping.<>c__DisplayClass122_0();
			CS$<>8__locals1.<>4__this = this;
			this.eventHolder.gameObject.SetActive(this._loopingNeigong >= 0);
			CombatSkillItem skillConfig = CombatSkill.Instance[this._loopingNeigong];
			for (int i = 0; i < this.neiliItems.Length; i++)
			{
				int minDelta = this._extraNeiliAllocationPerLoop.Items[i];
				int maxDelta = this._extraNeiliAllocationPerLoop.Items[i + 4];
				this.neiliItems[i].Set(this._taiwuExtraNeiliAllocationProgress, (byte)i, skillConfig, minDelta, maxDelta);
			}
			this.RefreshSkillItems();
			this.RefreshStrategyItems();
			for (int j = 0; j < this.commonAvailableStrategyItems.Length; j++)
			{
				sbyte strategyId = (j < this._availableStrategies.Items.Count) ? this._availableStrategies.Items[j] : -1;
				QiArtStrategyItem config = (strategyId >= 0) ? QiArtStrategy.Instance[strategyId] : null;
				bool isConcentrationEnough = config != null && this._curConcentration >= (int)config.ConcentrationCost;
				bool isNeiliEnough = config != null && this._currNeili >= (int)config.NeiliCost;
				this.commonAvailableStrategyItems[j].Set(config, isConcentrationEnough, isNeiliEnough, this._usedStrategies.Contains(j));
			}
			for (int k = 0; k < this.extraAvailableStrategyItems.Length; k++)
			{
				int index = k + this.commonAvailableStrategyItems.Length;
				sbyte strategyId2 = (index < this._availableStrategies.Items.Count) ? this._availableStrategies.Items[index] : -1;
				QiArtStrategyItem config2 = (strategyId2 >= 0) ? QiArtStrategy.Instance[strategyId2] : null;
				bool isConcentrationEnough2 = config2 != null && this._curConcentration >= (int)config2.ConcentrationCost;
				bool isNeiliEnough2 = config2 != null && this._currNeili >= (int)config2.NeiliCost;
				this.extraAvailableStrategyItems[k].Set(config2, isConcentrationEnough2, isNeiliEnough2, this._usedStrategies.Contains(index));
			}
			this.selectStrategyConcentration.text = string.Format("{0}/{1}", this._curConcentration, this._maxConcentration);
			ValueTuple<int, int> totalReferenceSkillBonus = this.GetTotalReferenceSkillBonus();
			int totalNeiliBonus = totalReferenceSkillBonus.Item1;
			int totalNeiliAllocationBonus = totalReferenceSkillBonus.Item2;
			string neiliBonusText = (totalNeiliBonus > 0) ? string.Format("+{0}%", totalNeiliBonus) : "";
			string neiliAllocationBonusText = (totalNeiliAllocationBonus > 0) ? string.Format("+{0}%", totalNeiliAllocationBonus) : "";
			string fiveElementText = this.GetFiveElementTransferAmountText();
			for (int l = 0; l < this.bounsInfoArr.Length; l++)
			{
				TextMeshProUGUI textMeshProUGUI = this.bounsInfoArr[l];
				if (!true)
				{
				}
				string text;
				switch (l)
				{
				case 0:
					text = neiliBonusText;
					break;
				case 1:
					text = fiveElementText;
					break;
				case 2:
					text = neiliAllocationBonusText;
					break;
				default:
					text = "";
					break;
				}
				if (!true)
				{
				}
				textMeshProUGUI.text = text;
				this.bounsInfoGoArr[l].SetActive(!string.IsNullOrEmpty(this.bounsInfoArr[l].text));
			}
			this.SetLoopingProgress();
			this.loopingNeili.text = this.GetNeiliDisplayText();
			this.RefreshMainPanel();
			this.loopingEventConcentration.text = string.Format("{0}/{1}", this._curConcentration, this._maxConcentration);
			int eventRate = (this._loopingNeigong >= 0) ? Math.Min(100, LoopingCommonUtils.CalcLoopingEventRate(CombatSkill.Instance[this._loopingNeigong], this._referenceSkillList)) : 0;
			this.loopingEventRate.text = string.Format("{0}%", eventRate);
			this.loopingEventBtn.interactable = this.HasEvent;
			this.eff_loopingEventBtnActive.gameObject.SetActive(this.HasEvent);
			this.RefreshEventTips();
			bool flag = this.skillHolder;
			if (flag)
			{
				this.RefreshListData();
			}
			ViewLooping.<>c__DisplayClass122_0 CS$<>8__locals2 = CS$<>8__locals1;
			ViewLooping.ViewLoopingMode currentMode = this.CurrentMode;
			CS$<>8__locals2.isSelectSkillMode = (currentMode == ViewLooping.ViewLoopingMode.SelectLoopingSkill || currentMode == ViewLooping.ViewLoopingMode.SelectReferenceSkill);
			Array.ForEach<ParticleSystem>(this.rightLines, delegate(ParticleSystem e)
			{
				e.transform.gameObject.SetActive(CS$<>8__locals1.<>4__this.CurrentMode != ViewLooping.ViewLoopingMode.SelectStrategy);
			});
			Array.ForEach<ParticleSystem>(this.leftLines, delegate(ParticleSystem e)
			{
				e.transform.gameObject.SetActive(!CS$<>8__locals1.isSelectSkillMode);
			});
		}

		// Token: 0x060074DB RID: 29915 RVA: 0x00366C7C File Offset: 0x00364E7C
		private void RefreshStrategyItems()
		{
			List<QiArtStrategyDisplayData> dataList = new List<QiArtStrategyDisplayData>();
			for (int i = 0; i < this._taiwuQiArtStrategyList.Count; i++)
			{
				bool flag = this._taiwuQiArtStrategyList[i] != null && this._taiwuQiArtStrategyList[i].TemplateId != -1;
				if (flag)
				{
					dataList.Add(this._taiwuQiArtStrategyList[i]);
				}
			}
			for (int j = 0; j < this.currentStrategyItems.Length; j++)
			{
				QiArtStrategyDisplayData strategyData = (j < dataList.Count) ? dataList[j] : null;
				this.currentStrategyItems[j].Set(strategyData);
				bool flag2 = this.CurrentMode == ViewLooping.ViewLoopingMode.SelectStrategy;
				if (flag2)
				{
					bool flag3 = j == dataList.Count;
					if (flag3)
					{
						this.currentStrategyItems[j].SetHighlightActive();
					}
					bool flag4 = j > dataList.Count;
					if (flag4)
					{
						this.currentStrategyItems[j].SetEmptyActive();
					}
				}
			}
		}

		// Token: 0x060074DC RID: 29916 RVA: 0x00366D84 File Offset: 0x00364F84
		private void RefreshSkillItems()
		{
			bool showAllSlotSelection = this.CurrentMode == ViewLooping.ViewLoopingMode.SelectReferenceSkill;
			for (int i = 0; i < this.skillItems.Length; i++)
			{
				bool isUnlock = this.IsReferenceSlotUnlock(i);
				int requireValue = GlobalConfig.Instance.ReferenceSkillSlotUnlockParams[i];
				short skillId = (i < this._referenceSkillList.Count) ? this._referenceSkillList[i] : -1;
				CombatSkillDisplayDataCharacterMenuListItem skillData = null;
				CombatSkillDisplayDataCharacterMenuListItem data;
				bool flag = skillId >= 0 && this._combatSkillDisplayDataDict.TryGetValue(skillId, out data);
				if (flag)
				{
					skillData = data;
				}
				CombatSkillItem loopingConfig = (this._loopingNeigong >= 0) ? CombatSkill.Instance[this._loopingNeigong] : null;
				CombatSkillItem referenceConfig = (skillData != null) ? CombatSkill.Instance[skillData.TemplateId] : null;
				int bonus = (referenceConfig != null) ? ViewLooping.GetReferenceSkillBonusPercent(loopingConfig, referenceConfig) : 0;
				this.skillItems[i].Set(skillData, isUnlock, requireValue, bonus, showAllSlotSelection);
			}
		}

		// Token: 0x060074DD RID: 29917 RVA: 0x00366E74 File Offset: 0x00365074
		private void SetLoopingProgress()
		{
			bool flag = this.loopingProgressArr.Length < 3;
			if (!flag)
			{
				ViewLooping.NeiliCalcData data = this.GetNeiliCalcData();
				bool flag2 = !data.IsValid;
				if (!flag2)
				{
					this.loopingProgressArr[0].fillAmount = (float)data.ObtainedNeili / (float)data.MaxNeili;
					this.loopingProgressArr[1].fillAmount = (float)(data.ObtainedNeili + data.RealMin) / (float)data.MaxNeili;
					this.loopingProgressArr[2].fillAmount = (float)(data.ObtainedNeili + data.RealMax) / (float)data.MaxNeili;
				}
			}
		}

		// Token: 0x060074DE RID: 29918 RVA: 0x00366F10 File Offset: 0x00365110
		private void RefreshEventTips()
		{
			TooltipInvoker tip = this.loopingEventTip;
			TooltipInvoker tooltipInvoker = tip;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = new ArgumentBox();
			}
			string color = "brightred";
			string color2 = "brightblue";
			string combatColor = (this._loopInCombatCount == 0) ? color : color2;
			string lifeSkillColor = (this._loopInLifeSkillCombatCount == 0) ? color : color2;
			string combatString = string.Format("<color=#{0}>{1}</color>", combatColor, this._loopInCombatCount);
			string lifeSkillString = string.Format("<color=#{0}>{1}</color>", lifeSkillColor, this._loopInLifeSkillCombatCount);
			tip.RuntimeParam.Set("CombatCountText", combatString);
			tip.RuntimeParam.Set("LifeSkillCountText", lifeSkillString);
		}

		// Token: 0x060074DF RID: 29919 RVA: 0x00366FC0 File Offset: 0x003651C0
		private void RefreshMainPanel()
		{
			foreach (RectTransform item in this.loopingSkillBgArr)
			{
				item.gameObject.SetActive(this._loopingNeigong >= 0);
			}
			for (int i = 0; i < this.loopingQiBackArr.Length; i++)
			{
				Image image = this.loopingQiBackArr[i];
				Sprite sprite;
				if (this._loopingNeigong < 0)
				{
					Sprite[] array2 = this.qiBackSpirtes;
					sprite = array2[array2.Length - 1];
				}
				else
				{
					sprite = this.qiBackSpirtes[i];
				}
				image.sprite = sprite;
			}
			this.loopingPanelBack.sprite = this.loopingPanelBackSpirtes[(this._loopingNeigong >= 0) ? 1 : 0];
			this.loopingSkillFrame.gameObject.SetActive(this._loopingNeigong >= 0);
			this.loopingSkillFrame.transform.parent.gameObject.SetActive(this._loopingNeigong >= 0);
			this.fiveElementsFrame.gameObject.SetActive(this._loopingNeigong >= 0);
			this.selectSkillText.SetActive(this._loopingNeigong < 0);
			TooltipInvoker tip = this.loopingSkillFrame.GetComponent<TooltipInvoker>();
			tip.enabled = (this._loopingNeigong >= 0);
			bool flag = this._loopingNeigong >= 0;
			if (flag)
			{
				CombatSkillItem config = CombatSkill.Instance[this._loopingNeigong];
				Color gradeColor = Colors.Instance.GradeColors[(int)config.Grade];
				this.loopingSkillIcon.SetSprite(config.Icon, false, null);
				this.loopingSkillIcon.SetColor(CommonUtils.GetFiveElementColor((int)config.FiveElements));
				this.loopingSkillFrame.SetSprite(ViewLooping.FramePaths[(int)config.EquipType] + config.Grade.ToString(), false, null);
				this.fiveElementsFrame.gameObject.SetActive(false);
				tip.Type = TipType.CombatSkill;
				tip.RuntimeParam = EasyPool.Get<ArgumentBox>().Set("CombatSkillId", this._loopingNeigong).Set("CharId", this.TaiwuCharId);
			}
			else
			{
				for (int j = 0; j < this.neiliLines.Count; j++)
				{
					this.neiliLines[j].Line.SetActive(false);
				}
			}
		}

		// Token: 0x060074E0 RID: 29920 RVA: 0x00367224 File Offset: 0x00365424
		private void RefreshNeiliProportion()
		{
			sbyte transferType = this._neiliDisplayData.TransferType;
			sbyte dstType = this._neiliDisplayData.DestType;
			sbyte srcType = -1;
			bool haveTransfer = dstType >= 0;
			bool flag = haveTransfer;
			if (flag)
			{
				if (!true)
				{
				}
				sbyte b;
				switch (transferType)
				{
				case 0:
					b = FiveElementsType.Countered[(int)dstType];
					break;
				case 1:
					b = FiveElementsType.Countering[(int)dstType];
					break;
				case 2:
					b = FiveElementsType.Produced[(int)dstType];
					break;
				default:
					b = FiveElementsType.Producing[(int)dstType];
					break;
				}
				if (!true)
				{
				}
				srcType = b;
			}
			this.SetNeiliValue(this._neiliDisplayData.NeiliProportion, srcType, dstType, (int)this._neiliDisplayData.Amount);
			this.SetNeiliLine(srcType, dstType);
		}

		// Token: 0x060074E1 RID: 29921 RVA: 0x003672D0 File Offset: 0x003654D0
		private unsafe void SetNeiliValue(NeiliProportionOfFiveElements currValue, sbyte srcType, sbyte dstType, int amount)
		{
			StringBuilder sb = new StringBuilder();
			for (sbyte i = 0; i < 5; i += 1)
			{
				Transform obj = this.fiveElementsValue.GetChild((int)i);
				sb.Clear();
				sb.Append(*currValue[(int)i]);
				sb.Append('%');
				bool flag = amount != 0;
				if (flag)
				{
					bool flag2 = srcType == i;
					if (flag2)
					{
						sb.Append(string.Format("-{0}%", amount).SetColor("darkred"));
					}
					else
					{
						bool flag3 = dstType == i;
						if (flag3)
						{
							sb.Append(string.Format("+{0}%", amount).SetColor("lightblue"));
						}
					}
				}
				obj.GetChild(0).GetComponent<TextMeshProUGUI>().SetText(sb.ToString(), true);
				obj.GetChild(1).gameObject.SetActive(amount != 0 && (srcType == i || dstType == i));
			}
		}

		// Token: 0x060074E2 RID: 29922 RVA: 0x003673D0 File Offset: 0x003655D0
		private void SetNeiliLine(sbyte srcType, sbyte dstType)
		{
			foreach (ViewLooping.NeiliLine neiliLine in this.neiliLines)
			{
				neiliLine.Line.SetActive(srcType == neiliLine.Type2 && dstType == neiliLine.Type1);
			}
		}

		// Token: 0x060074E3 RID: 29923 RVA: 0x00367440 File Offset: 0x00365640
		public void SetHighlightNode(sbyte type)
		{
			NeiliTypeItem neiliTypeConfig = NeiliType.Instance[type];
			for (sbyte nodeType = 0; nodeType < 6; nodeType += 1)
			{
				NeiliTypeItem config = NeiliType.Instance[nodeType];
				Transform obj = this.highlightNode.GetChild((int)nodeType);
				bool isBuff = config.ColorType == 1;
				obj.GetComponent<TooltipInvoker>().RuntimeParam = new ArgumentBox().Set("neiliType", (int)nodeType);
				obj.GetComponent<TooltipInvoker>().Refresh(false, -1);
				obj.GetComponent<CImage>().SetSprite(isBuff ? "ui9_icon_five_elements_highlight_big_1" : "ui9_icon_five_elements_highlight_big_0", false, null);
				obj.gameObject.SetActive(nodeType == neiliTypeConfig.TemplateId && nodeType != 5);
			}
		}

		// Token: 0x060074E4 RID: 29924 RVA: 0x00367500 File Offset: 0x00365700
		public IEnumerable<ColumnDefinition> GenerateColumnDefinitions()
		{
			LayoutOption option = new LayoutOption(100f, 100f, 100f, 1);
			bool flag = this.CurrentMode != ViewLooping.ViewLoopingMode.SelectReferenceSkill;
			if (flag)
			{
				ColumnDefinition<CombatSkillDisplayDataCharacterMenuListItem, CombatSkillDisplayDataCharacterMenuListItem> columnDefinition = new ColumnDefinition<CombatSkillDisplayDataCharacterMenuListItem, CombatSkillDisplayDataCharacterMenuListItem>();
				columnDefinition.LayoutOption = new LayoutOption(441f, 100f, 100f, 1);
				columnDefinition.TableHeadLabel = (() => LanguageKey.LK_Name.Tr());
				columnDefinition.CellDataGenerator = ((CombatSkillDisplayDataCharacterMenuListItem data) => data);
				columnDefinition.SortId = 0;
				yield return columnDefinition;
				ColumnDefinition<CombatSkillDisplayDataCharacterMenuListItem, string> columnDefinition2 = new ColumnDefinition<CombatSkillDisplayDataCharacterMenuListItem, string>();
				columnDefinition2.LayoutOption = new LayoutOption(191f, 100f, 100f, 1);
				columnDefinition2.TableHeadLabel = (() => LanguageKey.LK_LoopingInfomation_Title_Neili.Tr());
				columnDefinition2.CellDataGenerator = ((CombatSkillDisplayDataCharacterMenuListItem data) => string.Format("{0}/{1}", data.ObtainedNeili, data.MaxObtainableNeili).SetColor((data.ObtainedNeili < data.MaxObtainableNeili) ? "brightblue" : "brightred"));
				columnDefinition2.SortId = 144;
				yield return columnDefinition2;
				ColumnDefinition<CombatSkillDisplayDataCharacterMenuListItem, string> columnDefinition3 = new ColumnDefinition<CombatSkillDisplayDataCharacterMenuListItem, string>();
				columnDefinition3.LayoutOption = new LayoutOption(191f, 100f, 100f, 1);
				columnDefinition3.TableHeadLabel = (() => LanguageKey.LK_LoopingInfomation_Title_FiveElementTranferAmound.Tr());
				columnDefinition3.CellDataGenerator = delegate(CombatSkillDisplayDataCharacterMenuListItem data)
				{
					sbyte destType = data.FiveElementDestTypeWhileLooping;
					sbyte transferType = data.FiveElementTransferTypeWhileLooping;
					bool haveTransfer = destType >= 0;
					bool flag2 = haveTransfer;
					string result;
					if (flag2)
					{
						if (!true)
						{
						}
						sbyte b;
						switch (transferType)
						{
						case 0:
							b = FiveElementsType.Countered[(int)destType];
							break;
						case 1:
							b = FiveElementsType.Countering[(int)destType];
							break;
						case 2:
							b = FiveElementsType.Produced[(int)destType];
							break;
						default:
							b = FiveElementsType.Producing[(int)destType];
							break;
						}
						if (!true)
						{
						}
						sbyte srcType = b;
						result = string.Format("<SpName={0}{1}><SpName={2}><SpName={3}{4}>", new object[]
						{
							"ui9_icon_elements_big_",
							srcType,
							"ui9_back_eventwindow_loop_arrow_1",
							"ui9_icon_elements_big_",
							destType
						});
					}
					else
					{
						result = string.Empty;
					}
					return result;
				};
				columnDefinition3.SortId = 145;
				yield return columnDefinition3;
			}
			else
			{
				ColumnDefinition<CombatSkillDisplayDataCharacterMenuListItem, CombatSkillDisplayDataCharacterMenuListItem> columnDefinition4 = new ColumnDefinition<CombatSkillDisplayDataCharacterMenuListItem, CombatSkillDisplayDataCharacterMenuListItem>();
				columnDefinition4.LayoutOption = new LayoutOption(290f, 100f, 100f, 1);
				columnDefinition4.TableHeadLabel = (() => LanguageKey.LK_Name.Tr());
				columnDefinition4.CellDataGenerator = ((CombatSkillDisplayDataCharacterMenuListItem data) => data);
				columnDefinition4.SortId = 0;
				yield return columnDefinition4;
				ColumnDefinition<CombatSkillDisplayDataCharacterMenuListItem, string> columnDefinition5 = new ColumnDefinition<CombatSkillDisplayDataCharacterMenuListItem, string>();
				columnDefinition5.LayoutOption = new LayoutOption(132f, 100f, 100f, 1);
				columnDefinition5.TableHeadLabel = (() => LanguageKey.LK_Looping_ReferenceSkill_Title_Neili.Tr());
				columnDefinition5.CellDataGenerator = delegate(CombatSkillDisplayDataCharacterMenuListItem data)
				{
					CombatSkillItem loopingConfig = (this._loopingNeigong >= 0) ? CombatSkill.Instance[this._loopingNeigong] : null;
					CombatSkillItem referenceConfig = CombatSkill.Instance[data.TemplateId];
					int bonus = ViewLooping.GetReferenceSkillBonusPercent(loopingConfig, referenceConfig);
					return (bonus > 0) ? string.Format("+{0}%", bonus) : "";
				};
				columnDefinition5.SortId = 146;
				yield return columnDefinition5;
				ColumnDefinition<CombatSkillDisplayDataCharacterMenuListItem, string> columnDefinition6 = new ColumnDefinition<CombatSkillDisplayDataCharacterMenuListItem, string>();
				columnDefinition6.LayoutOption = new LayoutOption(132f, 100f, 100f, 1);
				columnDefinition6.TableHeadLabel = (() => LanguageKey.LK_Looping_ReferenceSkill_Title_NeiliAllocation.Tr());
				columnDefinition6.CellDataGenerator = delegate(CombatSkillDisplayDataCharacterMenuListItem data)
				{
					CombatSkillItem loopingConfig = (this._loopingNeigong >= 0) ? CombatSkill.Instance[this._loopingNeigong] : null;
					CombatSkillItem referenceConfig = CombatSkill.Instance[data.TemplateId];
					int bonus = ViewLooping.GetReferenceSkillBonusPercent(loopingConfig, referenceConfig);
					return (bonus > 0) ? string.Format("+{0}%", bonus) : "";
				};
				columnDefinition6.SortId = 147;
				yield return columnDefinition6;
				ColumnDefinition<CombatSkillDisplayDataCharacterMenuListItem, string> columnDefinition7 = new ColumnDefinition<CombatSkillDisplayDataCharacterMenuListItem, string>();
				columnDefinition7.LayoutOption = new LayoutOption(132f, 100f, 100f, 1);
				columnDefinition7.TableHeadLabel = (() => LanguageKey.LK_LoopingEvent.Tr());
				columnDefinition7.CellDataGenerator = delegate(CombatSkillDisplayDataCharacterMenuListItem data)
				{
					CombatSkillItem config = CombatSkill.Instance[data.TemplateId];
					return string.Format("+{0}%", config.QiArtStrategyGenerateProbability);
				};
				columnDefinition7.SortId = 148;
				yield return columnDefinition7;
				ColumnDefinition<CombatSkillDisplayDataCharacterMenuListItem, CombatSkillItem> columnDefinition8 = new ColumnDefinition<CombatSkillDisplayDataCharacterMenuListItem, CombatSkillItem>();
				columnDefinition8.LayoutOption = new LayoutOption(132f, 100f, 100f, 1);
				columnDefinition8.TableHeadLabel = (() => LanguageKey.LK_Looping_ReferenceSkill_Title_Strategy.Tr());
				columnDefinition8.CellDataGenerator = ((CombatSkillDisplayDataCharacterMenuListItem data) => CombatSkill.Instance[data.TemplateId]);
				columnDefinition8.SortId = 149;
				yield return columnDefinition8;
			}
			yield break;
		}

		// Token: 0x060074E5 RID: 29925 RVA: 0x00367510 File Offset: 0x00365710
		private void ShowDialogToChangeLoopingNeigong(short skillTemplateId)
		{
			bool flag = skillTemplateId == this._loopingNeigong;
			if (!flag)
			{
				bool flag2 = !this.HasEvent;
				if (flag2)
				{
					this.DoChangeLoopingNeigong(skillTemplateId);
				}
				else
				{
					this._changeLoopingNeigongDialog.Title = LocalStringManager.Get(LanguageKey.LK_LoopingEvent);
					this._changeLoopingNeigongDialog.Content = LocalStringManager.Get(LanguageKey.LK_Dialog_SwitchLoopingNeigong).ColorReplace();
					this._changeLoopingNeigongDialog.Yes = delegate()
					{
						this.DoChangeLoopingNeigong(skillTemplateId);
					};
					this._changeLoopingNeigongDialog.No = delegate()
					{
					};
					UIManager.Instance.SetEscHandler(delegate
					{
						UIManager.Instance.HideUI(UIElement.Dialog);
					});
					UIElement dialog = UIElement.Dialog;
					dialog.OnHide = (Action)Delegate.Combine(dialog.OnHide, new Action(delegate()
					{
						UIManager.Instance.SetEscHandler(null);
					}));
					UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", this._changeLoopingNeigongDialog));
					UIManager.Instance.MaskUI(UIElement.Dialog);
				}
			}
		}

		// Token: 0x060074E6 RID: 29926 RVA: 0x0036766C File Offset: 0x0036586C
		private void DoChangeLoopingNeigong(short skillTemplateId)
		{
			bool modified = this._modified;
			if (modified)
			{
				TaiwuDomainMethod.Call.ClearCurrentLoopingNeigongEvent();
				this.loopingEventBtn.interactable = false;
				this.eff_loopingEventBtnActive.gameObject.SetActive(false);
			}
			TaiwuDomainMethod.Call.SetTaiwuLoopingNeigong(skillTemplateId);
			bool flag = this.CurrentMode == ViewLooping.ViewLoopingMode.SelectStrategy;
			if (flag)
			{
				this.CurrentMode = ViewLooping.ViewLoopingMode.Normal;
			}
			this.RequestData();
		}

		// Token: 0x060074E7 RID: 29927 RVA: 0x003676D0 File Offset: 0x003658D0
		private void OnReferenceSelectClickCombatSkill(short skillTemplateId)
		{
			int existingIndex = this._referenceSkillList.IndexOf(skillTemplateId);
			bool flag = existingIndex >= 0;
			if (flag)
			{
				this.ShowDialogToClearReferenceCombatSkill(existingIndex);
			}
			else
			{
				bool flag2 = skillTemplateId < 0;
				if (!flag2)
				{
					for (int i = 0; i < this._referenceSkillList.Count; i++)
					{
						bool flag3 = this._referenceSkillList[i] == -1 && this.IsReferenceSlotUnlock(i);
						if (flag3)
						{
							this.ShowDialogToSetReferenceCombatSkill(skillTemplateId, i);
							return;
						}
					}
					this.ShowDialogToSetReferenceCombatSkill(skillTemplateId, this._selectedSkillSlotIndex);
				}
			}
		}

		// Token: 0x060074E8 RID: 29928 RVA: 0x00367764 File Offset: 0x00365964
		private void ShowDialogToSetReferenceCombatSkill(short skillTemplateId, int slotIndex)
		{
			bool flag = -1 == this._loopingNeigong;
			if (flag)
			{
				this.SetReferenceCombatSkill(skillTemplateId, slotIndex);
			}
			else
			{
				bool flag2 = !this.HasEvent;
				if (flag2)
				{
					this.SetReferenceCombatSkill(skillTemplateId, slotIndex);
				}
				else
				{
					this._changeLoopingNeigongDialog.Title = LocalStringManager.Get(LanguageKey.LK_LoopingEvent);
					this._changeLoopingNeigongDialog.Content = LocalStringManager.Get(LanguageKey.LK_Dialog_SwitchLoopingNeigong).ColorReplace();
					this._changeLoopingNeigongDialog.Yes = delegate()
					{
						this.SetReferenceCombatSkill(skillTemplateId, slotIndex);
					};
					this._changeLoopingNeigongDialog.No = delegate()
					{
					};
					UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", this._changeLoopingNeigongDialog));
					UIManager.Instance.MaskUI(UIElement.Dialog);
				}
			}
		}

		// Token: 0x060074E9 RID: 29929 RVA: 0x0036787C File Offset: 0x00365A7C
		private void ShowDialogToClearReferenceCombatSkill(int slotIndex)
		{
			bool flag = -1 == this._loopingNeigong;
			if (flag)
			{
				this.ClearReferenceSkillAt(slotIndex);
			}
			else
			{
				bool flag2 = !this.HasEvent;
				if (flag2)
				{
					this.ClearReferenceSkillAt(slotIndex);
				}
				else
				{
					this._changeLoopingNeigongDialog.Title = LocalStringManager.Get(LanguageKey.LK_LoopingEvent);
					this._changeLoopingNeigongDialog.Content = LocalStringManager.Get(LanguageKey.LK_Dialog_SwitchLoopingNeigong).ColorReplace();
					this._changeLoopingNeigongDialog.Yes = delegate()
					{
						this.ClearReferenceSkillAt(slotIndex);
					};
					this._changeLoopingNeigongDialog.No = delegate()
					{
					};
					UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", this._changeLoopingNeigongDialog));
					UIManager.Instance.MaskUI(UIElement.Dialog);
				}
			}
		}

		// Token: 0x060074EA RID: 29930 RVA: 0x0036797E File Offset: 0x00365B7E
		private void SetReferenceCombatSkill(short skillTemplateId, int slotIndex)
		{
			TaiwuDomainMethod.Call.SetReferenceCombatSkillAt(slotIndex, skillTemplateId);
			this.RequestData();
		}

		// Token: 0x060074EB RID: 29931 RVA: 0x00367990 File Offset: 0x00365B90
		private void ClearReferenceSkillAt(int slotIndex)
		{
			this._referenceSkillList.RemoveAt(slotIndex);
			this._referenceSkillList.Add(-1);
			for (int i = this._referenceSkillList.Count - 1; i >= slotIndex; i--)
			{
				bool flag = this.IsReferenceSlotUnlock(i);
				if (flag)
				{
					TaiwuDomainMethod.Call.SetReferenceCombatSkillAt(i, this._referenceSkillList[i]);
				}
			}
		}

		// Token: 0x060074EC RID: 29932 RVA: 0x003679F5 File Offset: 0x00365BF5
		private void OnClickInformationRemoveButton()
		{
			this.ShowDialogToChangeLoopingNeigong(-1);
		}

		// Token: 0x060074ED RID: 29933 RVA: 0x00367A00 File Offset: 0x00365C00
		private void TrySetStrategy(int availableIndex)
		{
			bool flag = this._selectedStrategySlot < 0;
			if (!flag)
			{
				sbyte strategyId = this._availableStrategies.Items[availableIndex];
				TaiwuDomainMethod.Call.SetQiArtStrategy(this._selectedStrategySlot, strategyId);
				this._usedStrategies.Add(availableIndex);
				this._modified = true;
				this.RequestData();
			}
		}

		// Token: 0x060074EE RID: 29934 RVA: 0x00367A58 File Offset: 0x00365C58
		protected override void OnClick(Transform btn)
		{
			string btnName = btn.name;
			string text = btnName;
			string a = text;
			if (!(a == "CloseBtn"))
			{
				if (!(a == "LoopingEventBtn"))
				{
					if (!(a == "DeleteLoopingSkillBtn"))
					{
						if (a == "CloseSelectLoopingSkillBtn")
						{
							this.CurrentMode = ViewLooping.ViewLoopingMode.Normal;
						}
					}
					else
					{
						this.OnClickInformationRemoveButton();
					}
				}
				else
				{
					this.CurrentMode = ViewLooping.ViewLoopingMode.SelectStrategy;
				}
			}
			else
			{
				this.QuickHide();
			}
		}

		// Token: 0x060074EF RID: 29935 RVA: 0x00367AD0 File Offset: 0x00365CD0
		public override void QuickHide()
		{
			bool flag = this.CurrentMode == ViewLooping.ViewLoopingMode.Normal;
			if (flag)
			{
				base.QuickHide();
			}
			else
			{
				this.CurrentMode = ViewLooping.ViewLoopingMode.Normal;
			}
		}

		// Token: 0x060074F0 RID: 29936 RVA: 0x00367B00 File Offset: 0x00365D00
		public short GetCurrentLoopingNeigongId()
		{
			return this._loopingNeigong;
		}

		// Token: 0x060074F1 RID: 29937 RVA: 0x00367B18 File Offset: 0x00365D18
		private void ApplyDefaultSort()
		{
			bool flag = this.CurrentMode == ViewLooping.ViewLoopingMode.SelectLoopingSkill;
			if (flag)
			{
				this._filteredSkillList.Sort(delegate(CombatSkillDisplayDataCharacterMenuListItem a, CombatSkillDisplayDataCharacterMenuListItem b)
				{
					CombatSkillItem aConfig = CombatSkill.Instance[a.TemplateId];
					CombatSkillItem bConfig = CombatSkill.Instance[b.TemplateId];
					int gradeCompare = bConfig.Grade.CompareTo(aConfig.Grade);
					bool flag3 = gradeCompare != 0;
					int result;
					if (flag3)
					{
						result = gradeCompare;
					}
					else
					{
						result = ((int)(b.MaxObtainableNeili - b.ObtainedNeili)).CompareTo((int)(a.MaxObtainableNeili - a.ObtainedNeili));
					}
					return result;
				});
			}
			else
			{
				bool flag2 = this.CurrentMode == ViewLooping.ViewLoopingMode.SelectReferenceSkill;
				if (flag2)
				{
					this._filteredSkillList.Sort(delegate(CombatSkillDisplayDataCharacterMenuListItem a, CombatSkillDisplayDataCharacterMenuListItem b)
					{
						CombatSkillItem loopingConfig = (this._loopingNeigong >= 0) ? CombatSkill.Instance[this._loopingNeigong] : null;
						CombatSkillItem aConfig = CombatSkill.Instance[a.TemplateId];
						CombatSkillItem bConfig = CombatSkill.Instance[b.TemplateId];
						int aBonus = ViewLooping.GetReferenceSkillBonusPercent(loopingConfig, aConfig);
						int bonusCompare = ViewLooping.GetReferenceSkillBonusPercent(loopingConfig, bConfig).CompareTo(aBonus);
						bool flag3 = bonusCompare != 0;
						int result;
						if (flag3)
						{
							result = bonusCompare;
						}
						else
						{
							int gradeCompare = bConfig.Grade.CompareTo(aConfig.Grade);
							bool flag4 = gradeCompare != 0;
							if (flag4)
							{
								result = gradeCompare;
							}
							else
							{
								List<sbyte> possibleQiArtStrategyList = aConfig.PossibleQiArtStrategyList;
								bool aHasStrategy = possibleQiArtStrategyList != null && possibleQiArtStrategyList.Count > 0;
								possibleQiArtStrategyList = bConfig.PossibleQiArtStrategyList;
								bool bHasStrategy = possibleQiArtStrategyList != null && possibleQiArtStrategyList.Count > 0;
								bool flag5 = aHasStrategy && !bHasStrategy;
								if (flag5)
								{
									result = -1;
								}
								else
								{
									bool flag6 = !aHasStrategy && bHasStrategy;
									if (flag6)
									{
										result = 1;
									}
									else
									{
										result = 0;
									}
								}
							}
						}
						return result;
					});
				}
			}
		}

		// Token: 0x060074F2 RID: 29938 RVA: 0x00367B8C File Offset: 0x00365D8C
		private bool IsReferenceSlotUnlock(int slotIndex)
		{
			return ((int)this._referenceSkillSlotUnlockStates & 1 << slotIndex) != 0;
		}

		// Token: 0x060074F3 RID: 29939 RVA: 0x00367BB0 File Offset: 0x00365DB0
		private bool IsReferencingSkill(short skillTemplateId)
		{
			return this._referenceSkillList.Contains(skillTemplateId);
		}

		// Token: 0x060074F4 RID: 29940 RVA: 0x00367BD0 File Offset: 0x00365DD0
		private static int GetReferenceSkillBonusPercent(CombatSkillItem loopingConfig, CombatSkillItem referenceConfig)
		{
			int bonus = 0;
			bool flag = loopingConfig != null && loopingConfig.LoopBonusSkillList.Contains(referenceConfig.TemplateId);
			if (flag)
			{
				bonus += 10;
			}
			bool flag2 = loopingConfig != null && referenceConfig.SectId == loopingConfig.SectId;
			if (flag2)
			{
				bonus += 20;
			}
			return bonus;
		}

		// Token: 0x060074F5 RID: 29941 RVA: 0x00367C28 File Offset: 0x00365E28
		private string GetFiveElementTransferAmountText()
		{
			bool flag = this._loopingNeigong < 0;
			string result;
			if (flag)
			{
				result = "";
			}
			else
			{
				CombatSkillDisplayDataCharacterMenuListItem skillDisplayData;
				bool flag2 = !this._combatSkillDisplayDataDict.TryGetValue(this._loopingNeigong, out skillDisplayData);
				if (flag2)
				{
					result = "";
				}
				else
				{
					CombatSkillItem skillConfig = CombatSkill.Instance[this._loopingNeigong];
					bool flag3 = skillConfig == null;
					if (flag3)
					{
						result = "";
					}
					else
					{
						sbyte destType = skillDisplayData.FiveElementDestTypeWhileLooping;
						bool flag4 = destType < 0;
						if (flag4)
						{
							result = "";
						}
						else
						{
							sbyte baseTransferAmount = skillConfig.FiveElementChangePerLoop;
							int amountBonusMin = 0;
							int amountBonusMax = 0;
							foreach (QiArtStrategyDisplayData strategy in this._taiwuQiArtStrategyList)
							{
								bool flag5;
								if (strategy == null)
								{
									flag5 = true;
								}
								else
								{
									sbyte templateId = strategy.TemplateId;
									flag5 = false;
								}
								bool flag6 = flag5 || strategy.TemplateId == -1;
								if (!flag6)
								{
									QiArtStrategyItem strategyConfig = QiArtStrategy.Instance[strategy.TemplateId];
									bool flag7 = strategyConfig != null;
									if (flag7)
									{
										amountBonusMin += (int)strategyConfig.MinExtraFiveElements;
										amountBonusMax += (int)strategyConfig.MaxExtraFiveElements;
									}
								}
							}
							int amountMin = (int)baseTransferAmount * (100 + amountBonusMin) / 100;
							int amountMax = (int)baseTransferAmount * (100 + amountBonusMax) / 100;
							result = ((amountMax > amountMin) ? string.Format("{0}~{1}", amountMin, amountMax) : amountMin.ToString());
						}
					}
				}
			}
			return result;
		}

		// Token: 0x060074F6 RID: 29942 RVA: 0x00367DB4 File Offset: 0x00365FB4
		[return: TupleElementNames(new string[]
		{
			"neiliBonus",
			"neiliAllocationBonus"
		})]
		private ValueTuple<int, int> GetTotalReferenceSkillBonus()
		{
			int totalBonus = 0;
			CombatSkillItem loopingConfig = (this._loopingNeigong >= 0) ? CombatSkill.Instance[this._loopingNeigong] : null;
			foreach (short skillId in this._referenceSkillList)
			{
				bool flag = skillId < 0;
				if (!flag)
				{
					CombatSkillDisplayDataCharacterMenuListItem data;
					CombatSkillDisplayDataCharacterMenuListItem skillData = this._combatSkillDisplayDataDict.TryGetValue(skillId, out data) ? data : null;
					bool flag2 = skillData == null;
					if (!flag2)
					{
						CombatSkillItem referenceConfig = CombatSkill.Instance[skillData.TemplateId];
						bool flag3 = referenceConfig != null;
						if (flag3)
						{
							totalBonus += ViewLooping.GetReferenceSkillBonusPercent(loopingConfig, referenceConfig);
						}
					}
				}
			}
			return new ValueTuple<int, int>(totalBonus, totalBonus);
		}

		// Token: 0x060074F7 RID: 29943 RVA: 0x00367E8C File Offset: 0x0036608C
		private ViewLooping.NeiliCalcData GetNeiliCalcData()
		{
			bool flag = this._loopingNeigong < 0;
			ViewLooping.NeiliCalcData result;
			if (flag)
			{
				result = new ViewLooping.NeiliCalcData
				{
					IsValid = false
				};
			}
			else
			{
				CombatSkillDisplayDataCharacterMenuListItem skillDisplayData;
				bool flag2 = !this._combatSkillDisplayDataDict.TryGetValue(this._loopingNeigong, out skillDisplayData);
				if (flag2)
				{
					result = new ViewLooping.NeiliCalcData
					{
						IsValid = false
					};
				}
				else
				{
					CombatSkillItem skillConfig = CombatSkill.Instance[this._loopingNeigong];
					bool flag3 = skillConfig == null;
					if (flag3)
					{
						result = new ViewLooping.NeiliCalcData
						{
							IsValid = false
						};
					}
					else
					{
						short obtainedNeili = skillDisplayData.ObtainedNeili;
						short basicNeiliPerLoop = skillConfig.ObtainedNeiliPerLoop;
						short maxNeili = skillDisplayData.MaxObtainableNeili;
						bool flag4 = maxNeili <= 0;
						if (flag4)
						{
							result = new ViewLooping.NeiliCalcData
							{
								IsValid = false
							};
						}
						else
						{
							int realMin = Math.Min((int)basicNeiliPerLoop + this._extraNeiliPerLoop.Item1, (int)(maxNeili - obtainedNeili));
							int realMax = Math.Min((int)basicNeiliPerLoop + this._extraNeiliPerLoop.Item2, (int)(maxNeili - obtainedNeili));
							result = new ViewLooping.NeiliCalcData
							{
								ObtainedNeili = (int)obtainedNeili,
								MaxNeili = (int)maxNeili,
								RealMin = realMin,
								RealMax = realMax,
								IsValid = true
							};
						}
					}
				}
			}
			return result;
		}

		// Token: 0x060074F8 RID: 29944 RVA: 0x00367FD0 File Offset: 0x003661D0
		private string GetNeiliDisplayText()
		{
			ViewLooping.NeiliCalcData data = this.GetNeiliCalcData();
			bool flag = !data.IsValid;
			string result;
			if (flag)
			{
				result = "";
			}
			else
			{
				string extraNeiliString = (data.RealMax > data.RealMin) ? string.Format("{0}~{1}", data.RealMin, data.RealMax) : data.RealMin.ToString();
				string coloredExtraNeiliString = (data.RealMax > 0) ? ("<color=#brightblue>+" + extraNeiliString + "</color>") : "";
				result = string.Format("{0}{1}/{2}", data.ObtainedNeili, coloredExtraNeiliString, data.MaxNeili).ColorReplace();
			}
			return result;
		}

		// Token: 0x17000D30 RID: 3376
		// (get) Token: 0x060074F9 RID: 29945 RVA: 0x00368088 File Offset: 0x00366288
		// (set) Token: 0x060074FA RID: 29946 RVA: 0x00368090 File Offset: 0x00366290
		private ViewLooping.ViewLoopingMode CurrentMode
		{
			get
			{
				return this._currentMode;
			}
			set
			{
				ViewLooping.ViewLoopingMode previousMode = this._currentMode;
				this._currentMode = value;
				this.OnCurrentModeChanged(previousMode);
			}
		}

		// Token: 0x060074FB RID: 29947 RVA: 0x003680B4 File Offset: 0x003662B4
		private void OnCurrentModeChanged(ViewLooping.ViewLoopingMode previousMode = ViewLooping.ViewLoopingMode.Normal)
		{
			ViewLooping.<>c__DisplayClass160_0 CS$<>8__locals1 = new ViewLooping.<>c__DisplayClass160_0();
			CS$<>8__locals1.<>4__this = this;
			bool flag = previousMode == ViewLooping.ViewLoopingMode.SelectStrategy;
			if (flag)
			{
				bool modified = this._modified;
				if (modified)
				{
					TaiwuDomainMethod.Call.ClearCurrentLoopingNeigongEvent();
					this.loopingEventBtn.interactable = false;
					this.eff_loopingEventBtnActive.gameObject.SetActive(false);
				}
			}
			ViewLooping.<>c__DisplayClass160_0 CS$<>8__locals2 = CS$<>8__locals1;
			ViewLooping.ViewLoopingMode currentMode = this.CurrentMode;
			CS$<>8__locals2.isSelectSkillMode = (currentMode == ViewLooping.ViewLoopingMode.SelectLoopingSkill || currentMode == ViewLooping.ViewLoopingMode.SelectReferenceSkill);
			this.panelHolder.DOLocalMoveX((float)(CS$<>8__locals1.isSelectSkillMode ? -802 : 0), 1f, false);
			this.panelHand1.DOAnchorPosX((float)(CS$<>8__locals1.isSelectSkillMode ? -646 : 574), 1f, false);
			this.panelHand2.DOAnchorPosX((float)(CS$<>8__locals1.isSelectSkillMode ? -1376 : -574), 1f, false);
			this.skillHolder.gameObject.SetActive(this.CurrentMode != ViewLooping.ViewLoopingMode.SelectStrategy);
			this.currentStrategyHolder.gameObject.SetActive(!CS$<>8__locals1.isSelectSkillMode);
			this.bounsInfoArrHolder.localPosition = new Vector3((float)(CS$<>8__locals1.isSelectSkillMode ? 200 : 0), 0f, 0f);
			this.stragetyHolder.gameObject.SetActive(this.CurrentMode == ViewLooping.ViewLoopingMode.SelectStrategy);
			bool isScrollHolderIn = previousMode == ViewLooping.ViewLoopingMode.Normal & CS$<>8__locals1.isSelectSkillMode;
			bool isScrollHolderOut = (previousMode == ViewLooping.ViewLoopingMode.SelectLoopingSkill || previousMode == ViewLooping.ViewLoopingMode.SelectReferenceSkill) && this.CurrentMode == ViewLooping.ViewLoopingMode.Normal;
			this.scrollHolder.DOKill(false);
			bool flag2 = isScrollHolderIn;
			if (flag2)
			{
				this.loopingSortAndFilter.gameObject.SetActive(this.CurrentMode == ViewLooping.ViewLoopingMode.SelectLoopingSkill);
				this.referenceSortAndFilter.gameObject.SetActive(this.CurrentMode == ViewLooping.ViewLoopingMode.SelectReferenceSkill);
				this.scrollHolder.anchoredPosition = this.scrollHolder.anchoredPosition.SetX(895f);
				this.scrollHolder.gameObject.SetActive(true);
				this.scrollHolder.DOAnchorPosX(4f, 0.75f, false);
			}
			else
			{
				bool flag3 = isScrollHolderOut;
				if (flag3)
				{
					this.scrollHolder.DOAnchorPosX(895f, 0.75f, false).OnComplete(delegate
					{
						CS$<>8__locals1.<>4__this.scrollHolder.gameObject.SetActive(false);
					});
				}
				else
				{
					this.scrollHolder.gameObject.SetActive(CS$<>8__locals1.isSelectSkillMode);
					this.loopingSortAndFilter.gameObject.SetActive(this.CurrentMode == ViewLooping.ViewLoopingMode.SelectLoopingSkill);
					this.referenceSortAndFilter.gameObject.SetActive(this.CurrentMode == ViewLooping.ViewLoopingMode.SelectReferenceSkill);
				}
			}
			for (int i = 0; i < this.currentStrategyItems.Length; i++)
			{
				this.currentStrategyItems[i].SetIsInEditingMode(this.CurrentMode == ViewLooping.ViewLoopingMode.SelectStrategy);
			}
			bool flag4 = this.CurrentMode == ViewLooping.ViewLoopingMode.SelectStrategy;
			if (flag4)
			{
				this.RefreshStrategyItems();
			}
			this.loopingSkillToggle.interactable = (this.CurrentMode != ViewLooping.ViewLoopingMode.SelectStrategy);
			this.deleteLoopingSkillBtn.interactable = (this.CurrentMode != ViewLooping.ViewLoopingMode.SelectStrategy);
			Array.ForEach<ParticleSystem>(this.rightLines, delegate(ParticleSystem e)
			{
				e.transform.gameObject.SetActive(CS$<>8__locals1.<>4__this.CurrentMode != ViewLooping.ViewLoopingMode.SelectStrategy);
			});
			Array.ForEach<ParticleSystem>(this.leftLines, delegate(ParticleSystem e)
			{
				e.transform.gameObject.SetActive(!CS$<>8__locals1.isSelectSkillMode);
			});
			TextMeshProUGUI textMeshProUGUI = this.titleLabel;
			ViewLooping.ViewLoopingMode currentMode2 = this.CurrentMode;
			if (!true)
			{
			}
			string text;
			if (currentMode2 != ViewLooping.ViewLoopingMode.SelectStrategy)
			{
				text = LanguageKey.LK_Looping_Title.Tr();
			}
			else
			{
				text = LanguageKey.LK_LoopingEvent.Tr();
			}
			if (!true)
			{
			}
			textMeshProUGUI.text = text;
			GameObject gameObject = this.loopingEventBtn.transform.parent.gameObject;
			currentMode = this.CurrentMode;
			gameObject.SetActive(currentMode != ViewLooping.ViewLoopingMode.SelectLoopingSkill && currentMode != ViewLooping.ViewLoopingMode.SelectReferenceSkill && currentMode != ViewLooping.ViewLoopingMode.SelectStrategy);
			switch (this.CurrentMode)
			{
			case ViewLooping.ViewLoopingMode.SelectLoopingSkill:
				this.RefreshScorll();
				this.RefreshListData();
				this.scroll.SetSortController(this._loopingSortAndFilterController);
				break;
			case ViewLooping.ViewLoopingMode.SelectReferenceSkill:
				this.RefreshScorll();
				this.RefreshListData();
				this.scroll.SetSortController(this._referenceSortAndFilterController);
				break;
			case ViewLooping.ViewLoopingMode.SelectStrategy:
				GlobalDomainMethod.Call.InvokeGuidingTrigger(65);
				break;
			}
			this.SyncLoopingSkillToggleFromMode();
			this.RefreshSkillItems();
		}

		// Token: 0x060074FC RID: 29948 RVA: 0x00368503 File Offset: 0x00366703
		[Button("切换显示状态")]
		public void Set(int value)
		{
			this.CurrentMode = (ViewLooping.ViewLoopingMode)value;
		}

		// Token: 0x060074FD RID: 29949 RVA: 0x0036850E File Offset: 0x0036670E
		public void RemoveReferenceSkill(int slotIndex)
		{
			this._selectedSkillSlotIndex = slotIndex;
			this.ClearReferenceSkillAt(this._selectedSkillSlotIndex);
			this.RequestData();
		}

		// Token: 0x060074FE RID: 29950 RVA: 0x0036852C File Offset: 0x0036672C
		public void SetSelectedReferenceSkillSlotIndex(int slotIndex)
		{
			this._selectedSkillSlotIndex = slotIndex;
			this.CurrentMode = ViewLooping.ViewLoopingMode.SelectReferenceSkill;
		}

		// Token: 0x060074FF RID: 29951 RVA: 0x0036853E File Offset: 0x0036673E
		public void SetStrategy(int strategyIndex)
		{
			this.TrySetStrategy(strategyIndex);
			this.RequestData();
		}

		// Token: 0x04005758 RID: 22360
		[SerializeField]
		private TextMeshProUGUI titleLabel;

		// Token: 0x04005759 RID: 22361
		[SerializeField]
		private Sprite[] qiBackSpirtes;

		// Token: 0x0400575A RID: 22362
		[SerializeField]
		private Sprite[] loopingPanelBackSpirtes;

		// Token: 0x0400575B RID: 22363
		[SerializeField]
		private ListStyleGeneralScroll scroll;

		// Token: 0x0400575C RID: 22364
		[SerializeField]
		private SortAndFilter loopingSortAndFilter;

		// Token: 0x0400575D RID: 22365
		[SerializeField]
		private SortAndFilter referenceSortAndFilter;

		// Token: 0x0400575E RID: 22366
		[Header("行模板配置")]
		[SerializeField]
		private RowItem loopingSkillRowTemplate;

		// Token: 0x0400575F RID: 22367
		[SerializeField]
		private RowItem referenceSkillRowTemplate;

		// Token: 0x04005760 RID: 22368
		[SerializeField]
		private NeiliItem[] neiliItems;

		// Token: 0x04005761 RID: 22369
		[SerializeField]
		private SkillItem[] skillItems;

		// Token: 0x04005762 RID: 22370
		[SerializeField]
		private CurrentStrategyItem[] currentStrategyItems;

		// Token: 0x04005763 RID: 22371
		[SerializeField]
		private AvailableStrategyItem[] commonAvailableStrategyItems;

		// Token: 0x04005764 RID: 22372
		[SerializeField]
		private AvailableStrategyItem[] extraAvailableStrategyItems;

		// Token: 0x04005765 RID: 22373
		[SerializeField]
		private TextMeshProUGUI selectStrategyConcentration;

		// Token: 0x04005766 RID: 22374
		[SerializeField]
		private TextMeshProUGUI[] bounsInfoArr;

		// Token: 0x04005767 RID: 22375
		[SerializeField]
		private GameObject[] bounsInfoGoArr;

		// Token: 0x04005768 RID: 22376
		[SerializeField]
		private RectTransform bounsInfoArrHolder;

		// Token: 0x04005769 RID: 22377
		[SerializeField]
		private CToggle loopingSkillToggle;

		// Token: 0x0400576A RID: 22378
		[SerializeField]
		private CImage loopingSkillIcon;

		// Token: 0x0400576B RID: 22379
		[SerializeField]
		private CImage loopingSkillFrame;

		// Token: 0x0400576C RID: 22380
		[SerializeField]
		private CImage fiveElementsFrame;

		// Token: 0x0400576D RID: 22381
		[SerializeField]
		private GameObject selectSkillText;

		// Token: 0x0400576E RID: 22382
		[SerializeField]
		private CImage[] loopingProgressArr;

		// Token: 0x0400576F RID: 22383
		[SerializeField]
		private TextMeshProUGUI loopingNeili;

		// Token: 0x04005770 RID: 22384
		[SerializeField]
		private RectTransform[] loopingSkillBgArr;

		// Token: 0x04005771 RID: 22385
		[SerializeField]
		private CImage[] loopingQiBackArr;

		// Token: 0x04005772 RID: 22386
		[SerializeField]
		private CImage loopingPanelBack;

		// Token: 0x04005773 RID: 22387
		[SerializeField]
		private TextMeshProUGUI loopingEventConcentration;

		// Token: 0x04005774 RID: 22388
		[SerializeField]
		private TextMeshProUGUI loopingEventRate;

		// Token: 0x04005775 RID: 22389
		[SerializeField]
		private CButton loopingEventBtn;

		// Token: 0x04005776 RID: 22390
		[SerializeField]
		private CImage loopingEventImg;

		// Token: 0x04005777 RID: 22391
		[SerializeField]
		private TooltipInvoker loopingEventTip;

		// Token: 0x04005778 RID: 22392
		[SerializeField]
		private RectTransform eventHolder;

		// Token: 0x04005779 RID: 22393
		[SerializeField]
		private RectTransform currentStrategyHolder;

		// Token: 0x0400577A RID: 22394
		[SerializeField]
		private Transform fiveElementsValue;

		// Token: 0x0400577B RID: 22395
		[SerializeField]
		private Transform highlightNode;

		// Token: 0x0400577C RID: 22396
		[SerializeField]
		private CButton deleteLoopingSkillBtn;

		// Token: 0x0400577D RID: 22397
		[SerializeField]
		private RectTransform panelHolder;

		// Token: 0x0400577E RID: 22398
		[SerializeField]
		private RectTransform panelHand1;

		// Token: 0x0400577F RID: 22399
		[SerializeField]
		private RectTransform panelHand2;

		// Token: 0x04005780 RID: 22400
		[SerializeField]
		private RectTransform neiliHolder;

		// Token: 0x04005781 RID: 22401
		[SerializeField]
		private RectTransform mainHolder;

		// Token: 0x04005782 RID: 22402
		[SerializeField]
		private RectTransform skillHolder;

		// Token: 0x04005783 RID: 22403
		[SerializeField]
		private RectTransform stragetyHolder;

		// Token: 0x04005784 RID: 22404
		[SerializeField]
		private RectTransform scrollHolder;

		// Token: 0x04005785 RID: 22405
		[SerializeField]
		private ParticleSystem[] leftLines;

		// Token: 0x04005786 RID: 22406
		[SerializeField]
		private ParticleSystem[] rightLines;

		// Token: 0x04005787 RID: 22407
		[SerializeField]
		private List<ViewLooping.NeiliLine> neiliLines;

		// Token: 0x04005788 RID: 22408
		[Header("特效")]
		[SerializeField]
		private ParticleSystem eff_loopingEventBtnActive;

		// Token: 0x04005789 RID: 22409
		private static Action<ViewLooping> _onShowAction;

		// Token: 0x0400578A RID: 22410
		private int _curConcentration;

		// Token: 0x0400578B RID: 22411
		private int _maxConcentration;

		// Token: 0x0400578C RID: 22412
		private int _currNeili;

		// Token: 0x0400578D RID: 22413
		private short _loopingNeigong = -1;

		// Token: 0x0400578E RID: 22414
		private readonly List<short> _learnedSkillList = new List<short>();

		// Token: 0x0400578F RID: 22415
		private readonly Dictionary<short, CombatSkillDisplayDataCharacterMenuListItem> _combatSkillDisplayDataDict = new Dictionary<short, CombatSkillDisplayDataCharacterMenuListItem>();

		// Token: 0x04005790 RID: 22416
		private readonly List<short> _referenceSkillList = new List<short>();

		// Token: 0x04005791 RID: 22417
		private byte _referenceSkillSlotUnlockStates;

		// Token: 0x04005792 RID: 22418
		private IntList _taiwuExtraNeiliAllocationProgress;

		// Token: 0x04005793 RID: 22419
		private ValueTuple<int, int> _extraNeiliPerLoop;

		// Token: 0x04005794 RID: 22420
		private IntList _extraNeiliAllocationPerLoop;

		// Token: 0x04005795 RID: 22421
		private List<QiArtStrategyDisplayData> _taiwuQiArtStrategyList;

		// Token: 0x04005796 RID: 22422
		private SByteList _availableStrategies;

		// Token: 0x04005797 RID: 22423
		private readonly HashSet<int> _usedStrategies = new HashSet<int>();

		// Token: 0x04005798 RID: 22424
		private int _selectedStrategySlot = -1;

		// Token: 0x04005799 RID: 22425
		private List<short> _loopingEventSkillIdList;

		// Token: 0x0400579A RID: 22426
		private sbyte _loopInLifeSkillCombatCount;

		// Token: 0x0400579B RID: 22427
		private sbyte _loopInCombatCount;

		// Token: 0x0400579C RID: 22428
		private Func<CombatSkillDisplayDataCharacterMenuListItem, bool> _customFilter;

		// Token: 0x0400579D RID: 22429
		private CombatSkillSortAndFilterController _loopingSortAndFilterController;

		// Token: 0x0400579E RID: 22430
		private CombatSkillSortAndFilterController _referenceSortAndFilterController;

		// Token: 0x0400579F RID: 22431
		private readonly List<CombatSkillDisplayDataCharacterMenuListItem> _skillList = new List<CombatSkillDisplayDataCharacterMenuListItem>();

		// Token: 0x040057A0 RID: 22432
		private readonly List<CombatSkillDisplayDataCharacterMenuListItem> _filteredSkillList = new List<CombatSkillDisplayDataCharacterMenuListItem>();

		// Token: 0x040057A1 RID: 22433
		private int _skillScrollSelectedSkillIndex;

		// Token: 0x040057A2 RID: 22434
		private int _selectedSkillSlotIndex;

		// Token: 0x040057A3 RID: 22435
		private LoopingViewDisplayData _displayData;

		// Token: 0x040057A4 RID: 22436
		private TaiwuNeiliProportionDisplayData _neiliDisplayData;

		// Token: 0x040057A5 RID: 22437
		private CharacterDisplayDataForNeiliPage _neiliPageData;

		// Token: 0x040057A6 RID: 22438
		private static readonly string[] FramePaths = new string[]
		{
			"ui9_icon_combat_skill_type_neigong_",
			"ui9_icon_combat_skill_type_attack_",
			"ui9_icon_combat_skill_type_agile_",
			"ui9_icon_combat_skill_type_defense_",
			"ui9_icon_combat_skill_type_assist_"
		};

		// Token: 0x040057A7 RID: 22439
		private const int SkillModeOffsetX = 200;

		// Token: 0x040057A8 RID: 22440
		private const int PanelHandNormal1 = 574;

		// Token: 0x040057A9 RID: 22441
		private const int PanelHandSelectSkill1 = -646;

		// Token: 0x040057AA RID: 22442
		private const int PanelHandNormal2 = -574;

		// Token: 0x040057AB RID: 22443
		private const int PanelHandSelectSkill2 = -1376;

		// Token: 0x040057AC RID: 22444
		private const int ScrollHolderNormalX = 4;

		// Token: 0x040057AD RID: 22445
		private const int ScrollHolderStartX = 895;

		// Token: 0x040057AE RID: 22446
		private bool _modified = false;

		// Token: 0x040057AF RID: 22447
		private readonly DialogCmd _changeLoopingNeigongDialog = new DialogCmd();

		// Token: 0x040057B0 RID: 22448
		private ViewLooping.ViewLoopingMode _currentMode;

		// Token: 0x02001E98 RID: 7832
		[Serializable]
		public struct NeiliLine
		{
			// Token: 0x0400CA42 RID: 51778
			public sbyte Type1;

			// Token: 0x0400CA43 RID: 51779
			public sbyte Type2;

			// Token: 0x0400CA44 RID: 51780
			public GameObject Line;
		}

		// Token: 0x02001E99 RID: 7833
		private struct NeiliCalcData
		{
			// Token: 0x0400CA45 RID: 51781
			public int ObtainedNeili;

			// Token: 0x0400CA46 RID: 51782
			public int MaxNeili;

			// Token: 0x0400CA47 RID: 51783
			public int RealMin;

			// Token: 0x0400CA48 RID: 51784
			public int RealMax;

			// Token: 0x0400CA49 RID: 51785
			public bool IsValid;
		}

		// Token: 0x02001E9A RID: 7834
		public enum ViewLoopingMode
		{
			// Token: 0x0400CA4B RID: 51787
			Normal,
			// Token: 0x0400CA4C RID: 51788
			SelectLoopingSkill,
			// Token: 0x0400CA4D RID: 51789
			SelectReferenceSkill,
			// Token: 0x0400CA4E RID: 51790
			SelectStrategy
		}
	}
}
