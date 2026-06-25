using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Coffee.UIExtensions;
using Config;
using DG.Tweening;
using FrameWork;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using Game.Components.Common;
using Game.Components.Item;
using Game.Components.ListStyleGeneralScroll.Item;
using Game.Components.SortAndFilter;
using Game.Components.SortAndFilter.Item.Apply;
using Game.Views.Item;
using GameData.Domains.Character;
using GameData.Domains.Extra;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Taiwu;
using GameData.Domains.World.Display;
using GameData.Utilities;
using Spine.Unity;
using TMPro;
using UnityEngine;

namespace Game.Views.SectInteract.Zhujian
{
	// Token: 0x020009D5 RID: 2517
	public class ZhujianGearMateSubPageSkillBase : MonoBehaviour
	{
		// Token: 0x17000D86 RID: 3462
		// (get) Token: 0x06007AA4 RID: 31396 RVA: 0x0038F028 File Offset: 0x0038D228
		private ItemSourceType CurItemSourceType
		{
			get
			{
				ZhujianGearMateSubPageSkillBase.ItemSourceTogKey activeIndex = (ZhujianGearMateSubPageSkillBase.ItemSourceTogKey)this.toggleGroupItemSource.GetActiveIndex();
				if (!true)
				{
				}
				ItemSourceType result;
				switch (activeIndex)
				{
				case ZhujianGearMateSubPageSkillBase.ItemSourceTogKey.Inventory:
					result = ItemSourceType.Inventory;
					break;
				case ZhujianGearMateSubPageSkillBase.ItemSourceTogKey.Warehouse:
					result = ItemSourceType.Warehouse;
					break;
				case ZhujianGearMateSubPageSkillBase.ItemSourceTogKey.Treasury:
					result = ItemSourceType.Treasury;
					break;
				default:
					throw new ArgumentOutOfRangeException();
				}
				if (!true)
				{
				}
				return result;
			}
		}

		// Token: 0x17000D87 RID: 3463
		// (get) Token: 0x06007AA5 RID: 31397 RVA: 0x0038F070 File Offset: 0x0038D270
		private int PageCount
		{
			get
			{
				return this._isCombatSkill ? 6 : 5;
			}
		}

		// Token: 0x17000D88 RID: 3464
		// (get) Token: 0x06007AA6 RID: 31398 RVA: 0x0038F07E File Offset: 0x0038D27E
		private GearMate GearMate
		{
			get
			{
				return this._displayData.GearMate;
			}
		}

		// Token: 0x17000D89 RID: 3465
		// (get) Token: 0x06007AA7 RID: 31399 RVA: 0x0038F08B File Offset: 0x0038D28B
		private int _exp
		{
			get
			{
				return this._displayData.TaiwuExp;
			}
		}

		// Token: 0x17000D8A RID: 3466
		// (get) Token: 0x06007AA8 RID: 31400 RVA: 0x0038F098 File Offset: 0x0038D298
		private Dictionary<int, SkillBookPageDisplayData> PageDisplayData
		{
			get
			{
				return this._displayData.PageDisplayDataDict;
			}
		}

		// Token: 0x17000D8B RID: 3467
		// (get) Token: 0x06007AA9 RID: 31401 RVA: 0x0038F0A5 File Offset: 0x0038D2A5
		private short[] AttainmentPanels
		{
			get
			{
				return this._displayData.CombatSkillAttainmentPanels;
			}
		}

		// Token: 0x17000D8C RID: 3468
		// (get) Token: 0x06007AAA RID: 31402 RVA: 0x0038F0B2 File Offset: 0x0038D2B2
		private LifeSkillShorts LifeSkillQualifications
		{
			get
			{
				return this._displayData.LifeSkillQualifications;
			}
		}

		// Token: 0x17000D8D RID: 3469
		// (get) Token: 0x06007AAB RID: 31403 RVA: 0x0038F0BF File Offset: 0x0038D2BF
		private CombatSkillShorts CombatSkillQualifications
		{
			get
			{
				return this._displayData.CombatSkillQualifications;
			}
		}

		// Token: 0x17000D8E RID: 3470
		// (get) Token: 0x06007AAC RID: 31404 RVA: 0x0038F0CC File Offset: 0x0038D2CC
		private LifeSkillShorts LifeSkillAttainments
		{
			get
			{
				return this._displayData.LifeSkillAttainments;
			}
		}

		// Token: 0x17000D8F RID: 3471
		// (get) Token: 0x06007AAD RID: 31405 RVA: 0x0038F0D9 File Offset: 0x0038D2D9
		private CombatSkillShorts CombatSkillAttainments
		{
			get
			{
				return this._displayData.CombatSkillAttainments;
			}
		}

		// Token: 0x17000D90 RID: 3472
		// (get) Token: 0x06007AAE RID: 31406 RVA: 0x0038F0E6 File Offset: 0x0038D2E6
		private List<GameData.Domains.Character.LifeSkillItem> LearnedLifeSkills
		{
			get
			{
				return this._displayData.LearnedLifeSkills ?? new List<GameData.Domains.Character.LifeSkillItem>();
			}
		}

		// Token: 0x17000D91 RID: 3473
		// (get) Token: 0x06007AAF RID: 31407 RVA: 0x0038F0FC File Offset: 0x0038D2FC
		private ItemListScroll ItemList
		{
			get
			{
				return (this.multiplyItemListScroll != null) ? this.multiplyItemListScroll.CurMultiplyScrollView : null;
			}
		}

		// Token: 0x06007AB0 RID: 31408 RVA: 0x0038F11A File Offset: 0x0038D31A
		public void Init(ViewZhujianGearMate parentView, bool isCombatSkill, Action requestGearMateData)
		{
			this._parentView = parentView;
			this._isCombatSkill = isCombatSkill;
			this._requestGearMateData = requestGearMateData;
		}

		// Token: 0x06007AB1 RID: 31409 RVA: 0x0038F134 File Offset: 0x0038D334
		private void Awake()
		{
			this.buttonConfirm.ClearAndAddListener(new Action(this.OnConfirm));
			ESortAndFilterControllerType sortAndFilterType = this._isCombatSkill ? ESortAndFilterControllerType.CombatSkillBook : ESortAndFilterControllerType.LifeSkillBook;
			string sortSaveKey = this._isCombatSkill ? "ZhujianGearMateSubPageCombatSkill" : "ZhujianGearMateSubPageLifeSkill";
			this.EnsureMultiplyItemListScrollReference();
			bool flag = this.multiplyItemListScroll != null;
			if (flag)
			{
				this.multiplyItemListScroll.SetupItemListScroll(new ManagedMultiplyItemListScrollSetup
				{
					MainSortSaveKey = sortSaveKey,
					SortType = sortAndFilterType,
					EnableRowInteraction = true,
					OnRender = new Action<ITradeableContent, RowItemLine>(this.OnItemRender),
					OnClick = new Action<ITradeableContent, RowItemLine>(this.OnItemClick),
					ColumnType = (ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Book)
				});
			}
			this.InitMultiplyItemListScroll();
			this._bookListSortAndFilter = ((this.ItemList != null) ? this.ItemList.GetComponentInChildren<SortAndFilter>(true) : null);
			this.RestoreSortButtonGroupAfterTableHead();
			GameObject prefab = this._isCombatSkill ? this.combatSkillBookPreview.gameObject : this.lifeSkillBookPreview.gameObject;
			this.previewScroll.UpdateStyle(prefab, 0);
			this.previewScroll.OnItemRender += this.PreviewScrollOnItemRender;
			this.toggleGroupItemSource.Init(-1);
			this.toggleGroupItemSource.OnActiveIndexChange += this.ToggleGroupItemSourceOnActiveIndexChange;
			this.propertyCost.Set("ui9_icon_resource_big_8", LanguageKey.LK_Exp_Cost_Tips.Tr(), string.Empty, null, false);
			this.combatSkillBookPreview.gameObject.SetActive(false);
			this.lifeSkillBookPreview.gameObject.SetActive(false);
			this.textConfirm.text = (this._isCombatSkill ? LanguageKey.LK_GearMate_SkillBook_Combat_Confirm.Tr() : LanguageKey.LK_GearMate_SkillBook_Life_Confirm.Tr());
		}

		// Token: 0x06007AB2 RID: 31410 RVA: 0x0038F301 File Offset: 0x0038D501
		private void OnDestroy()
		{
			this.previewScroll.OnItemRender -= this.PreviewScrollOnItemRender;
			this.toggleGroupItemSource.OnActiveIndexChange -= this.ToggleGroupItemSourceOnActiveIndexChange;
		}

		// Token: 0x06007AB3 RID: 31411 RVA: 0x0038F334 File Offset: 0x0038D534
		private void EnsureMultiplyItemListScrollReference()
		{
			bool flag = this.multiplyItemListScroll != null;
			if (!flag)
			{
				this.multiplyItemListScroll = base.GetComponentInChildren<ManagedMultiplyItemListScroll>(true);
			}
		}

		// Token: 0x06007AB4 RID: 31412 RVA: 0x0038F364 File Offset: 0x0038D564
		private void InitMultiplyItemListScroll()
		{
			bool flag = this.multiplyItemListScroll == null;
			if (!flag)
			{
				bool flag2 = !this.multiplyItemListScroll.HasInit;
				if (flag2)
				{
					this.multiplyItemListScroll.Init(null);
				}
				this.multiplyItemListScroll.CanSelectItemPredicate = new Func<ItemDisplayData, bool>(this.CanSelectBookItem);
				this.multiplyItemListScroll.GetSelectLimitOverride = ((ItemDisplayData _, int defaultLimit) => 1);
				this.multiplyItemListScroll.SelectAllHandler = new Action(this.OnSelectAllItems);
				this.multiplyItemListScroll.ClearSelectionHandler = new Action(this.OnClearAllSelection);
				this.multiplyItemListScroll.GetSelectedCountForLabel = (() => this._selectedBooks.Count);
				this.multiplyItemListScroll.CanEnableSelectAllButton = new Func<bool>(this.HasSelectableBooks);
				bool flag3 = !this.multiplyItemListScroll.IsMultiItemSelect;
				if (flag3)
				{
					this.multiplyItemListScroll.EnterMultiplyMode(false);
				}
				this._multiplyReady = true;
			}
		}

		// Token: 0x06007AB5 RID: 31413 RVA: 0x0038F46B File Offset: 0x0038D66B
		private bool CanSelectBookItem(ItemDisplayData item)
		{
			return this.IsBookInteractable(item);
		}

		// Token: 0x06007AB6 RID: 31414 RVA: 0x0038F474 File Offset: 0x0038D674
		private bool HasSelectableBooks()
		{
			this.FillFilteredSourceItems(this._filteredSourceBuffer);
			for (int i = 0; i < this._filteredSourceBuffer.Count; i++)
			{
				bool flag = this.CanSelectBookItem(this._filteredSourceBuffer[i]);
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06007AB7 RID: 31415 RVA: 0x0038F4CA File Offset: 0x0038D6CA
		private void FillFilteredSourceItems(List<ItemDisplayData> buffer)
		{
			buffer.Clear();
			ManagedMultiplyItemListScroll managedMultiplyItemListScroll = this.multiplyItemListScroll;
			if (managedMultiplyItemListScroll != null)
			{
				managedMultiplyItemListScroll.CollectFilteredSourceItems(buffer);
			}
		}

		// Token: 0x06007AB8 RID: 31416 RVA: 0x0038F4E8 File Offset: 0x0038D6E8
		private void OnSelectAllItems()
		{
			this._isBatchSelectingBooks = true;
			try
			{
				this.ClearAllBooksSelection();
				this.FillFilteredSourceItems(this._selectAllWorkList);
				for (int i = 0; i < this._selectAllWorkList.Count; i++)
				{
					ItemDisplayData itemData = this._selectAllWorkList[i];
					bool flag = !this.CanSelectBookItem(itemData) || this.FindSelectedBook(itemData) != null;
					if (!flag)
					{
						bool usingTypeNeedConfirm = itemData.GetUsingTypeNeedConfirm();
						bool flag2 = usingTypeNeedConfirm;
						if (!flag2)
						{
							this.SelectBook(itemData);
						}
					}
				}
			}
			finally
			{
				this._isBatchSelectingBooks = false;
			}
			this.OnSelectedBookChanged();
			this.SyncMultiplySelection();
		}

		// Token: 0x06007AB9 RID: 31417 RVA: 0x0038F59C File Offset: 0x0038D79C
		private void OnClearAllSelection()
		{
			this._isBatchSelectingBooks = true;
			try
			{
				this.ClearAllBooksSelection();
			}
			finally
			{
				this._isBatchSelectingBooks = false;
			}
			this.OnSelectedBookChanged();
			this.SyncMultiplySelection();
		}

		// Token: 0x06007ABA RID: 31418 RVA: 0x0038F5E4 File Offset: 0x0038D7E4
		private void ClearAllBooksSelection()
		{
			foreach (ItemDisplayData book in this._selectedBooks.ToArray())
			{
				this.DeselectBook(book);
			}
		}

		// Token: 0x06007ABB RID: 31419 RVA: 0x0038F61C File Offset: 0x0038D81C
		private void RestoreSortButtonGroupAfterTableHead()
		{
			ItemListScroll itemList = this.ItemList;
			SortAndFilterConfig sortAndFilterConfig;
			if (itemList == null)
			{
				sortAndFilterConfig = null;
			}
			else
			{
				SortAndFilterController<ITradeableContent> sortAndFilterController = itemList.SortAndFilterController;
				sortAndFilterConfig = ((sortAndFilterController != null) ? sortAndFilterController.OriginalConfig : null);
			}
			SortAndFilterConfig config = sortAndFilterConfig;
			bool flag = config == null || this._bookListSortAndFilter == null;
			if (!flag)
			{
				this._bookListSortAndFilter.Setup(config);
			}
		}

		// Token: 0x06007ABC RID: 31420 RVA: 0x0038F66E File Offset: 0x0038D86E
		public void ResetToggleGroupItemSource()
		{
			this.toggleGroupItemSource.Set(0, false);
		}

		// Token: 0x06007ABD RID: 31421 RVA: 0x0038F680 File Offset: 0x0038D880
		private void OnItemClick(ITradeableContent content, RowItemLine rowItemLine)
		{
			ZhujianGearMateSubPageSkillBase.<>c__DisplayClass83_0 CS$<>8__locals1 = new ZhujianGearMateSubPageSkillBase.<>c__DisplayClass83_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.itemData = (content as ItemDisplayData);
			bool isSelected = this.IsSelected(CS$<>8__locals1.itemData);
			bool flag = isSelected;
			if (flag)
			{
				this.DeselectBook(CS$<>8__locals1.itemData);
			}
			else
			{
				bool usingTypeNeedConfirm = CS$<>8__locals1.itemData.GetUsingTypeNeedConfirm();
				bool flag2 = usingTypeNeedConfirm;
				if (flag2)
				{
					string title = LanguageKey.LK_Common_Attention.Tr();
					string desc = CS$<>8__locals1.itemData.GetUsingOperationConfirmTip(ItemDisplayData.ItemUsingOperationType.Default);
					CommonUtils.ShowConfirmDialog(title, desc, new Action(CS$<>8__locals1.<OnItemClick>g__OnSelect|0), new Action(this.SyncMultiplySelection), EDialogType.None);
				}
				else
				{
					CS$<>8__locals1.<OnItemClick>g__OnSelect|0();
				}
			}
		}

		// Token: 0x06007ABE RID: 31422 RVA: 0x0038F72C File Offset: 0x0038D92C
		private bool IsSelected(ITradeableContent itemData)
		{
			ItemDisplayData displayData = itemData as ItemDisplayData;
			bool flag = displayData == null;
			return !flag && this._selectedBooks.Exists((ItemDisplayData d) => d.RealKey.Equals(displayData.RealKey));
		}

		// Token: 0x06007ABF RID: 31423 RVA: 0x0038F77C File Offset: 0x0038D97C
		private ItemDisplayData FindSelectedBook(ItemDisplayData itemData)
		{
			return this._selectedBooks.Find((ItemDisplayData d) => d.RealKey.Equals(itemData.RealKey));
		}

		// Token: 0x06007AC0 RID: 31424 RVA: 0x0038F7B0 File Offset: 0x0038D9B0
		private void OnItemRender(ITradeableContent itemData, RowItemLine rowItemLine)
		{
			RowItemMain rowItemMain = rowItemLine.GetComponentInChildren<RowItemMain>();
			rowItemMain.SetData(itemData);
			rowItemLine.Set(rowItemMain, true);
			bool flag = this._multiplyReady && this.multiplyItemListScroll.IsMultiItemSelect;
			if (flag)
			{
				this.multiplyItemListScroll.OnRenderItemMultiply(itemData, rowItemLine);
			}
			else
			{
				bool interactable = this.IsBookInteractable(itemData);
				rowItemLine.SetInteractable(interactable, true);
				rowItemLine.SetDisabled(!interactable);
				rowItemLine.SetSelected(this.IsSelected(itemData));
			}
		}

		// Token: 0x06007AC1 RID: 31425 RVA: 0x0038F82C File Offset: 0x0038DA2C
		private bool IsBookInteractable(ITradeableContent itemData)
		{
			bool flag = itemData == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				short id = this._isCombatSkill ? SkillBook.Instance[itemData.RealKey.TemplateId].CombatSkillTemplateId : SkillBook.Instance[itemData.RealKey.TemplateId].LifeSkillTemplateId;
				sbyte[] progressReadDefault = this.GetDefaultProgressRead(id);
				sbyte[] progressRead = this.GetProgressRead(id);
				int count = this._isCombatSkill ? 15 : this.PageCount;
				bool flag2;
				if (!progressReadDefault.Take(count).All((sbyte p) => p >= 100))
				{
					flag2 = progressRead.Take(count).All((sbyte p) => p >= 100);
				}
				else
				{
					flag2 = true;
				}
				bool isReadAll = flag2;
				bool interactable = itemData.Interactable && !itemData.IsLocked && !isReadAll;
				result = interactable;
			}
			return result;
		}

		// Token: 0x06007AC2 RID: 31426 RVA: 0x0038F92D File Offset: 0x0038DB2D
		private void ToggleGroupItemSourceOnActiveIndexChange(int newIndex, int oldIndex)
		{
			this.RefreshBooks();
		}

		// Token: 0x06007AC3 RID: 31427 RVA: 0x0038F938 File Offset: 0x0038DB38
		public void OnConfirm()
		{
			UIManager.Instance.ShowUI(UIElement.FullScreenMask, true);
			this.buttonConfirm.interactable = false;
			bool flag = this._curGradeAnim != null;
			if (flag)
			{
				this._curGradeAnim.DOPause();
				this._curGradeAnim = null;
			}
			this._finishedCountAfter = -1;
			SkeletonGraphic skeletonGraphic = this.skeletonGraphic;
			if (skeletonGraphic != null)
			{
				skeletonGraphic.AnimationState.SetAnimation(0, "move", false);
			}
			UIParticle uiparticle = this.uiParticle;
			if (uiparticle != null)
			{
				uiparticle.Play();
			}
			UIParticle uiparticle2 = this.uiParticle;
			float num;
			if (uiparticle2 == null)
			{
				num = 0f;
			}
			else
			{
				num = uiparticle2.particles.Max((ParticleSystem p) => p.main.duration);
			}
			float uiParticleMaxDuration = num;
			float duration = uiParticleMaxDuration + 0.5f;
			this.ShowBubble(duration);
			SkeletonGraphic skeletonGraphic2 = this.skeletonGraphic;
			float spineDuration = (skeletonGraphic2 != null) ? skeletonGraphic2.AnimationState.GetCurrent(0).Animation.Duration : 0f;
			SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(spineDuration, delegate
			{
				this.ConfirmSelectedBooks();
				this.OnSelectedBookChanged();
				bool flag2 = this._curGradeParticle != null;
				if (flag2)
				{
					this._curGradeParticle.Play();
					AudioManager.Instance.PlaySound("SFX_GearMate_consuming_aptitude", false, false);
					this._curGradeParticle = null;
				}
				UIManager.Instance.HideUI(UIElement.FullScreenMask);
			});
			AudioManager.Instance.PlaySound("SFX_GearMate_consuming_click", false, false);
		}

		// Token: 0x06007AC4 RID: 31428 RVA: 0x0038FA5C File Offset: 0x0038DC5C
		public void Refresh(SectZhujianGearMateSkillDisplayData displayData)
		{
			this._displayData = displayData;
			this._selectedBooks.Clear();
			this._pageProgressDic.Clear();
			this._pageProgressDicDefault.Clear();
			this._bookTotalProgressDic.Clear();
			bool isCombatSkill = this._isCombatSkill;
			if (isCombatSkill)
			{
				foreach (KeyValuePair<short, TaiwuCombatSkill> pair in this.GearMate.CombatSkillReadingProgress)
				{
					sbyte[] progressArray = pair.Value.GetAllBookPageReadingProgress();
					this._pageProgressDic.Add(pair.Key, progressArray);
					this._pageProgressDicDefault.Add(pair.Key, (sbyte[])progressArray.Clone());
					sbyte totalProgress = (sbyte)(this._pageProgressDic[pair.Key].Sum() / this._pageProgressDic[pair.Key].Length);
					this._bookTotalProgressDic.Add(pair.Key, totalProgress);
				}
			}
			else
			{
				foreach (KeyValuePair<short, TaiwuLifeSkill> pair2 in this.GearMate.LifeSkillReadingProgress)
				{
					sbyte[] progressArray2 = pair2.Value.GetAllBookPageReadingProgress();
					this._pageProgressDic.Add(pair2.Key, progressArray2);
					this._pageProgressDicDefault.Add(pair2.Key, (sbyte[])progressArray2.Clone());
					sbyte totalProgress2 = (sbyte)(this._pageProgressDic[pair2.Key].Sum() / this._pageProgressDic[pair2.Key].Length);
					this._bookTotalProgressDic.Add(pair2.Key, totalProgress2);
				}
			}
			ItemSourceToggleHelper.RefreshInteractableForInteract(this.toggleGroupItemSource, this._displayData.CanUseWarehouse, false);
			this.RefreshBooks();
			this.RefreshBtnTip();
			this.OnSelectedBookChanged();
		}

		// Token: 0x06007AC5 RID: 31429 RVA: 0x0038FC88 File Offset: 0x0038DE88
		private void RefreshBtnTip()
		{
			TooltipInvoker disPlayer = this.tipButtonConfirm;
			string title = this._isCombatSkill ? LocalStringManager.Get(LanguageKey.LK_GearMateCombatSkillTitle) : LocalStringManager.Get(LanguageKey.LK_GearMateLifeSkillTitle);
			string gearMateName = NameCenter.GetMonasticTitleOrDisplayName(this._displayData.GearMateDisplayData, false);
			disPlayer.enabled = true;
			TooltipInvoker tooltipInvoker = disPlayer;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = new ArgumentBox();
			}
			disPlayer.Type = TipType.GeneralLines;
			disPlayer.RuntimeParam.Set("Title", title);
			int lineCount = 0;
			string lineOne = LocalStringManager.GetFormat(LanguageKey.UI_MouseTipGearMateSkillBtn_2, gearMateName);
			string lineTwo = LocalStringManager.Get(LanguageKey.UI_MouseTipGearMateSkillBtn_3);
			string line3 = LocalStringManager.Get(LanguageKey.UI_MouseTipGearMateSkillBtn_4);
			string line4 = LocalStringManager.Get(LanguageKey.UI_MouseTipGearMateSkillBtn_5);
			string line5 = LocalStringManager.Get(LanguageKey.UI_MouseTipGearMateSkillBtn_6);
			disPlayer.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData(7, new List<string>
			{
				lineOne
			}, null));
			disPlayer.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData(1, new List<string>
			{
				lineTwo
			}, null));
			disPlayer.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData(2, new List<string>
			{
				"mousetip_bookstate_0",
				line3
			}, null));
			disPlayer.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData(2, new List<string>
			{
				"mousetip_bookstate_1",
				line4
			}, null));
			disPlayer.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData(2, new List<string>
			{
				"mousetip_bookstate_2",
				line5
			}, null));
			disPlayer.RuntimeParam.Set("LineCount", lineCount);
			disPlayer.Refresh(false, -1);
		}

		// Token: 0x06007AC6 RID: 31430 RVA: 0x0038FE94 File Offset: 0x0038E094
		private bool IsBookRead(ItemKey key)
		{
			SkillBookItem config = SkillBook.Instance[key.TemplateId];
			short templateId = this._isCombatSkill ? config.CombatSkillTemplateId : config.LifeSkillTemplateId;
			sbyte progress;
			return this._bookTotalProgressDic.TryGetValue(templateId, out progress) && progress >= 100;
		}

		// Token: 0x06007AC7 RID: 31431 RVA: 0x0038FEEC File Offset: 0x0038E0EC
		private void RefreshBooks()
		{
			this._availableBookList.Clear();
			ItemSourceType curItemSourceType = this.CurItemSourceType;
			foreach (ItemDisplayData itemData in this._displayData.CanReadBookItemList)
			{
				itemData.Interactable = !this.IsBookRead(itemData.RealKey);
				bool flag = itemData.ItemSourceTypeEnum != curItemSourceType;
				if (!flag)
				{
					this._availableBookList.Add(itemData);
				}
			}
			bool flag2 = this.multiplyItemListScroll != null;
			if (flag2)
			{
				this.multiplyItemListScroll.SetItems(this._availableBookList);
			}
			else
			{
				ItemListScroll itemList = this.ItemList;
				if (itemList != null)
				{
					itemList.SetItemList(this._availableBookList);
				}
			}
			ItemListScroll itemList2 = this.ItemList;
			if (itemList2 != null)
			{
				SortAndFilterController<ITradeableContent> sortAndFilterController = itemList2.SortAndFilterController;
				if (sortAndFilterController != null)
				{
					sortAndFilterController.NotifyDataChanged(this._availableBookList);
				}
			}
			this.SyncMultiplySelection();
		}

		// Token: 0x06007AC8 RID: 31432 RVA: 0x0038FFF4 File Offset: 0x0038E1F4
		private void SyncMultiplySelection()
		{
			bool flag = !this._multiplyReady || this.multiplyItemListScroll == null;
			if (!flag)
			{
				Dictionary<ItemDisplayData, int> dict = this.multiplyItemListScroll.SelectedMultiplyItemDict;
				List<ItemDisplayData> list = this.multiplyItemListScroll.SelectedMultiplyItemOrderedList;
				dict.Clear();
				list.Clear();
				foreach (ItemDisplayData book in this._selectedBooks)
				{
					dict[book] = 1;
					list.Add(book);
				}
				this.multiplyItemListScroll.SyncSelectedListPanelVisibility();
				bool isBatchSelectionOperation = this.multiplyItemListScroll.IsBatchSelectionOperation;
				if (!isBatchSelectionOperation)
				{
					this.multiplyItemListScroll.RefreshSelectionVisual(new bool?(this.multiplyItemListScroll.IsSelectedListExpanded));
				}
			}
		}

		// Token: 0x06007AC9 RID: 31433 RVA: 0x003900DC File Offset: 0x0038E2DC
		private void PreviewScrollOnItemRender(int index, GameObject obj)
		{
			ItemDisplayData itemData = this._selectedBooks[index];
			ItemKey itemKey = itemData.RealKey;
			short id = this._isCombatSkill ? SkillBook.Instance[itemKey.TemplateId].CombatSkillTemplateId : SkillBook.Instance[itemKey.TemplateId].LifeSkillTemplateId;
			sbyte[] progressReadDefault = this.GetDefaultProgressRead(id);
			sbyte[] progressRead = this.GetProgressRead(id);
			bool isCombatSkill = this._isCombatSkill;
			if (isCombatSkill)
			{
				GearMateCombatSkillBookPreview bookPreview = obj.GetComponent<GearMateCombatSkillBookPreview>();
				bookPreview.Refresh(itemData, progressReadDefault, progressRead, new Action<ItemDisplayData>(this.OnClickCancelPreview));
				this._finishedCountAfter = this.RenderPagesPreview(bookPreview, progressRead, id);
			}
			else
			{
				GearMateLifeSkillBookPreview bookPreview2 = obj.GetComponent<GearMateLifeSkillBookPreview>();
				bookPreview2.Refresh(itemData, progressReadDefault, progressRead, new Action<ItemDisplayData>(this.OnClickCancelPreview));
				this._finishedCountAfter = this.RenderPagesPreview(bookPreview2, progressRead);
			}
		}

		// Token: 0x06007ACA RID: 31434 RVA: 0x003901B7 File Offset: 0x0038E3B7
		private void OnClickCancelPreview(ItemDisplayData itemData)
		{
			this.DeselectBook(itemData);
		}

		// Token: 0x06007ACB RID: 31435 RVA: 0x003901C4 File Offset: 0x0038E3C4
		private void ShowBubble(float duration)
		{
			bool isCombatSkill = this._isCombatSkill;
			LanguageKey[] levelUpKeys;
			LanguageKey[] normalKeys;
			if (isCombatSkill)
			{
				levelUpKeys = new LanguageKey[]
				{
					LanguageKey.LK_GearMateCombatSkill_SpeakWord0,
					LanguageKey.LK_GearMateCombatSkill_SpeakWord1
				};
				normalKeys = new LanguageKey[]
				{
					LanguageKey.LK_GearMateCombatSkill_SpeakWord2,
					LanguageKey.LK_GearMateCombatSkill_SpeakWord3
				};
			}
			else
			{
				levelUpKeys = new LanguageKey[]
				{
					LanguageKey.LK_GearMateLifeSkill_SpeakWord0,
					LanguageKey.LK_GearMateLifeSkill_SpeakWord1
				};
				normalKeys = new LanguageKey[]
				{
					LanguageKey.LK_GearMateLifeSkill_SpeakWord2,
					LanguageKey.LK_GearMateLifeSkill_SpeakWord3
				};
			}
			int id = Random.Range(0, 2);
			string content = this._isQualificationLevelUp ? LocalStringManager.Get(levelUpKeys[id]) : LocalStringManager.Get(normalKeys[id]);
			this._parentView.ShowBubble(content, duration);
			this._parentView.DoGearMateAnimation("break_2");
		}

		// Token: 0x06007ACC RID: 31436 RVA: 0x00390284 File Offset: 0x0038E484
		private unsafe void RenderSkillType(ItemDisplayData itemData)
		{
			ZhujianGearMateSubPageSkillBase.<>c__DisplayClass98_0 CS$<>8__locals1;
			CS$<>8__locals1.<>4__this = this;
			DOTweenAnimation curGradeAnim = this._curGradeAnim;
			if (curGradeAnim != null)
			{
				curGradeAnim.DOPause();
			}
			this._curGradeParticle = null;
			bool hasSelect = itemData != null;
			this.skillTypeRoot.SetActive(hasSelect);
			bool flag = !hasSelect;
			if (!flag)
			{
				ItemKey itemKey = itemData.RealKey;
				TextMeshProUGUI attainmentDesc = this.textSkillLevel;
				TextMeshProUGUI typeName = this.textSkillName;
				CImage skillIcon = this.imageSkillIcon;
				CS$<>8__locals1.disPlayer = this.tipSkillArea;
				SkillBookItem config = SkillBook.Instance[itemKey.TemplateId];
				this._curSkillType = (this._isCombatSkill ? config.CombatSkillType : config.LifeSkillType);
				sbyte curGrade = config.Grade;
				CS$<>8__locals1.skillTypeName = (this._isCombatSkill ? CombatSkillType.Instance[this._curSkillType].Name : Config.LifeSkillType.Instance[this._curSkillType].Name);
				typeName.text = CS$<>8__locals1.skillTypeName;
				for (sbyte grade = 0; grade < 9; grade += 1)
				{
					GameObject stageRefers = this.gradeArray[(int)grade];
					TextMeshProUGUI gradeText = stageRefers.GetComponentInChildren<TextMeshProUGUI>();
					string gradeStr = ZhujianGearMateSubPageSkillBase.GetShortGradeDisplay(grade);
					bool isLearned = false;
					bool isCombatSkill = this._isCombatSkill;
					if (isCombatSkill)
					{
						bool flag2 = this.GearMate.IsCombatSkillBuffed(this._curSkillType, grade);
						if (flag2)
						{
							isLearned = true;
						}
					}
					else
					{
						bool flag3 = this.GearMate.IsLifeSkillBuffed(this._curSkillType, grade);
						if (flag3)
						{
							isLearned = true;
						}
					}
					gradeText.text = (isLearned ? gradeStr.SetGradeColor((int)grade) : gradeStr.SetColor("grey"));
				}
				bool isCombatSkill2 = this._isCombatSkill;
				if (isCombatSkill2)
				{
					CombatSkillTypeItem skillConfig = CombatSkillType.Instance[this._curSkillType];
					skillIcon.SetSprite(skillConfig.DisplayIconOutLine, false, null);
					short curQualificationValue = *this.CombatSkillQualifications[(int)this._curSkillType];
					short curAttainmentValue = *this.CombatSkillAttainments[(int)this._curSkillType];
					int alreadyAddQual = 0;
					this._sectInfos.Clear();
					for (sbyte grade2 = 0; grade2 < 9; grade2 += 1)
					{
						short skillTemplateId = -1;
						short[] attainmentPanels = this.AttainmentPanels;
						bool flag4 = attainmentPanels != null && attainmentPanels.Length > 0;
						if (flag4)
						{
							skillTemplateId = CombatSkillAttainmentPanelsHelper.Get(this.AttainmentPanels, this._curSkillType, grade2);
						}
						bool flag5 = skillTemplateId >= 0;
						if (flag5)
						{
							CombatSkillHelper.CalcAttainments_RecordSectInfo(this._sectInfos, CombatSkill.Instance[skillTemplateId].SectId, grade2);
						}
						bool flag6 = this.GearMate.IsCombatSkillBuffed(this._curSkillType, grade2);
						if (flag6)
						{
							alreadyAddQual += (int)((grade2 + 1) * 2);
						}
					}
					this.<RenderSkillType>g__RefreshTip|98_0(alreadyAddQual, ref CS$<>8__locals1);
					int index = CombatSkillHelper.CalcAttainments_GetPrimarySectIndex(this._sectInfos);
					bool hasSectBonus = index >= 0 && this._sectInfos[index].OrgTemplateId > 0;
					string sectName = hasSectBonus ? LocalStringManager.Get(string.Format("LK_Sect_Name_Short_{0}", this._sectInfos[index].OrgTemplateId)) : null;
					string attainmentLevelDesc = hasSectBonus ? LocalStringManager.GetFormat(LanguageKey.LK_Combat_Skill_Attainment_Panel_Sect_Bonus_Title, sectName) : LocalStringManager.Get(LanguageKey.LK_Combat_Skill_Attainment_Panel_No_Sect_Bonus_Title).SetColor("grey");
					attainmentDesc.text = attainmentLevelDesc;
					this.TryRefreshSkillPropertyPreview();
					int qualificationAddValue = 0;
					bool flag7 = curGrade == -1;
					if (!flag7)
					{
						bool flag8 = !this.GearMate.IsCombatSkillBuffed(this._curSkillType, curGrade);
						if (flag8)
						{
							GameObject gradeObj = this.gradeArray[(int)curGrade];
							UIParticle particle = gradeObj.GetComponentInChildren<UIParticle>();
							bool flag9 = this._finishedCountAfter == this.PageCount;
							if (flag9)
							{
								qualificationAddValue = (int)(2 * (curGrade + 1));
								this._curGradeAnim = gradeObj.GetComponentInChildren<DOTweenAnimation>();
								this._curGradeAnim.DOPlay();
								this._curGradeParticle = particle;
							}
						}
						this._isQualificationLevelUp = (qualificationAddValue > 0);
					}
				}
				else
				{
					LifeSkillTypeItem skillConfig2 = Config.LifeSkillType.Instance[this._curSkillType];
					skillIcon.SetSprite(skillConfig2.DisplayIconOutLine, false, null);
					short curQualificationValue2 = *this.LifeSkillQualifications[(int)this._curSkillType];
					short curAttainmentValue2 = *this.LifeSkillAttainments[(int)this._curSkillType];
					int alreadyAddQual2 = 0;
					for (sbyte grade3 = 0; grade3 < 9; grade3 += 1)
					{
						bool flag10 = this.GearMate.IsLifeSkillBuffed(this._curSkillType, grade3);
						if (flag10)
						{
							alreadyAddQual2 += (int)((grade3 + 1) * 2);
						}
					}
					this.<RenderSkillType>g__RefreshTip|98_0(alreadyAddQual2, ref CS$<>8__locals1);
					int readedBookCount = 0;
					short[] skillIdList = Config.LifeSkillType.Instance[this._curSkillType].SkillList;
					short[] array = skillIdList;
					for (int i = 0; i < array.Length; i++)
					{
						short skillId = array[i];
						short id = skillId;
						bool flag11 = this.LearnedLifeSkills.FindIndex((GameData.Domains.Character.LifeSkillItem item) => item.SkillTemplateId == id) >= 0;
						if (flag11)
						{
							readedBookCount++;
						}
					}
					int attainmentLevel = readedBookCount / 3;
					LanguageKey key = ZhujianGearMateSubPageSkillBase.AttainmentLevelName[attainmentLevel];
					string attainmentLevelDesc2 = key.Tr();
					bool flag12 = attainmentLevel < 2;
					if (flag12)
					{
						attainmentLevelDesc2 = attainmentLevelDesc2.SetColor("grey");
					}
					attainmentDesc.text = attainmentLevelDesc2;
					this.TryRefreshSkillPropertyPreview();
					int qualificationAddValue2 = 0;
					bool flag13 = curGrade == -1;
					if (!flag13)
					{
						bool flag14 = !this.GearMate.IsLifeSkillBuffed(this._curSkillType, curGrade);
						if (flag14)
						{
							GameObject gradeObj2 = this.gradeArray[(int)curGrade];
							UIParticle particle2 = gradeObj2.GetComponentInChildren<UIParticle>();
							bool flag15 = this._finishedCountAfter == this.PageCount;
							if (flag15)
							{
								this._curGradeAnim = gradeObj2.GetComponentInChildren<DOTweenAnimation>();
								this._curGradeAnim.DOPlay();
								this._curGradeParticle = particle2;
								qualificationAddValue2 = (int)(2 * (curGrade + 1));
							}
						}
						this._isQualificationLevelUp = (qualificationAddValue2 > 0);
					}
				}
			}
		}

		// Token: 0x06007ACD RID: 31437 RVA: 0x00390864 File Offset: 0x0038EA64
		private int RenderPagesPreview(GearMateCombatSkillBookPreview bookPreview, sbyte[] progressRead, short skillBookId)
		{
			ZhujianGearMateSubPageSkillBase.<>c__DisplayClass99_0 CS$<>8__locals1;
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.progressRead = progressRead;
			ItemKey itemKey = bookPreview.ItemKey;
			CS$<>8__locals1.pageDisplayData = this.PageDisplayData[itemKey.Id];
			CS$<>8__locals1.bookIds = new List<int>();
			int finishedCountAfterRead = 0;
			for (int i = 0; i < this.PageCount; i++)
			{
				bool isFinishedAfter = this.<RenderPagesPreview>g__RenderSingleChapter|99_0(i, ref CS$<>8__locals1);
				bool flag = isFinishedAfter;
				if (flag)
				{
					finishedCountAfterRead++;
				}
			}
			bool isCombatSkill = this._isCombatSkill;
			if (isCombatSkill)
			{
				List<int> defaultReadIds = ZhujianGearMateSubPageSkillBase.GetReadIdsFromProgressArray(this.GetDefaultProgressRead(skillBookId));
				bookPreview.RefreshTip(defaultReadIds, CS$<>8__locals1.bookIds);
			}
			return finishedCountAfterRead;
		}

		// Token: 0x06007ACE RID: 31438 RVA: 0x00390914 File Offset: 0x0038EB14
		private int RenderPagesPreview(GearMateLifeSkillBookPreview bookPreview, sbyte[] progressRead)
		{
			ZhujianGearMateSubPageSkillBase.<>c__DisplayClass100_0 CS$<>8__locals1;
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.progressRead = progressRead;
			ItemKey itemKey = bookPreview.ItemKey;
			CS$<>8__locals1.pageDisplayData = this.PageDisplayData[itemKey.Id];
			int finishedCountAfterRead = 0;
			for (int i = 0; i < this.PageCount; i++)
			{
				bool isFinishedAfter = this.<RenderPagesPreview>g__RenderSingleChapter|100_0(i, ref CS$<>8__locals1);
				bool flag = isFinishedAfter;
				if (flag)
				{
					finishedCountAfterRead++;
				}
			}
			return finishedCountAfterRead;
		}

		// Token: 0x06007ACF RID: 31439 RVA: 0x0039098C File Offset: 0x0038EB8C
		private byte GetSkillBookRealPage(SkillBookPageDisplayData pageDisplayData, int i)
		{
			bool flag = !this._isCombatSkill;
			byte result;
			if (flag)
			{
				result = (byte)i;
			}
			else
			{
				bool flag2 = i == 0;
				if (flag2)
				{
					result = (byte)pageDisplayData.Type[0];
				}
				else
				{
					int idx = (pageDisplayData.Type[i] == 0) ? (4 + i) : (9 + i);
					result = (byte)idx;
				}
			}
			return result;
		}

		// Token: 0x06007AD0 RID: 31440 RVA: 0x003909E4 File Offset: 0x0038EBE4
		private static List<int> GetReadIdsFromProgressArray(sbyte[] progressRead)
		{
			List<int> readIds = new List<int>();
			for (int i = 0; i < 15; i++)
			{
				bool flag = progressRead[i] >= 100;
				if (flag)
				{
					readIds.Add(i);
				}
			}
			return readIds;
		}

		// Token: 0x06007AD1 RID: 31441 RVA: 0x00390A28 File Offset: 0x0038EC28
		private sbyte[] GetDefaultProgressRead(short id)
		{
			sbyte[] progressReadDefault;
			bool flag = !this._pageProgressDicDefault.TryGetValue(id, out progressReadDefault);
			if (flag)
			{
				progressReadDefault = new sbyte[15];
			}
			return progressReadDefault;
		}

		// Token: 0x06007AD2 RID: 31442 RVA: 0x00390A58 File Offset: 0x0038EC58
		private sbyte[] GetProgressRead(short id)
		{
			sbyte[] progressReadDefault;
			bool flag = !this._pageProgressDic.TryGetValue(id, out progressReadDefault);
			if (flag)
			{
				progressReadDefault = new sbyte[15];
			}
			return progressReadDefault;
		}

		// Token: 0x06007AD3 RID: 31443 RVA: 0x00390A88 File Offset: 0x0038EC88
		private int GetBookRequiredExp(ItemKey key)
		{
			int needReadCount = 0;
			short id = this._isCombatSkill ? SkillBook.Instance[key.TemplateId].CombatSkillTemplateId : SkillBook.Instance[key.TemplateId].LifeSkillTemplateId;
			sbyte[] progressRead = this.GetProgressRead(id);
			for (int i = 0; i < this.PageCount; i++)
			{
				bool flag = progressRead[(int)this.GetSkillBookRealPage(this.PageDisplayData[key.Id], i)] < 100;
				if (flag)
				{
					needReadCount++;
				}
			}
			sbyte grade = ItemTemplateHelper.GetGrade(key.ItemType, key.TemplateId);
			short pageExp = SkillGradeData.Instance[grade].ReadingExpGainPerPage;
			return (int)pageExp * needReadCount;
		}

		// Token: 0x06007AD4 RID: 31444 RVA: 0x00390B48 File Offset: 0x0038ED48
		private unsafe void GetProgressAdd(ItemKey itemKey, ref int* progressAddArr)
		{
			SkillBookPageDisplayData pageDisplayData = this.PageDisplayData[itemKey.Id];
			for (int i = 0; i < this.PageCount; i++)
			{
				sbyte progressAdd = 10;
				sbyte pageState = pageDisplayData.State[i];
				sbyte b = pageState;
				sbyte b2 = b;
				if (b2 != 0)
				{
					if (b2 == 1)
					{
						progressAdd = 40;
					}
				}
				else
				{
					progressAdd = 100;
				}
				*(progressAddArr + (IntPtr)i * 4) += (int)progressAdd;
			}
		}

		// Token: 0x06007AD5 RID: 31445 RVA: 0x00390BB8 File Offset: 0x0038EDB8
		private void SelectBook(ItemDisplayData itemData)
		{
			this._curBookItemData = itemData;
			ItemKey itemKey = itemData.RealKey;
			SkillBookPageDisplayData pageDisplayData = this.PageDisplayData[itemKey.Id];
			short id = this._isCombatSkill ? SkillBook.Instance[itemKey.TemplateId].CombatSkillTemplateId : SkillBook.Instance[itemKey.TemplateId].LifeSkillTemplateId;
			sbyte[] progressRead = this.GetProgressRead(id);
			this._expRequirement += this.GetBookRequiredExp(itemKey);
			for (int i = 0; i < this.PageCount; i++)
			{
				sbyte progressAdd = 10;
				sbyte pageState = pageDisplayData.State[i];
				sbyte b = pageState;
				sbyte b2 = b;
				if (b2 != 0)
				{
					if (b2 == 1)
					{
						progressAdd = 40;
					}
				}
				else
				{
					progressAdd = 100;
				}
				byte realId = this.GetSkillBookRealPage(pageDisplayData, i);
				progressRead[(int)realId] = (sbyte)Mathf.Min((int)(progressRead[(int)realId] + progressAdd), 100);
			}
			this._pageProgressDic[id] = progressRead;
			bool flag = this.FindSelectedBook(itemData) == null;
			if (flag)
			{
				this._selectedBooks.Add(itemData);
			}
			bool flag2 = !this._isBatchSelectingBooks;
			if (flag2)
			{
				this.OnSelectedBookChanged();
			}
		}

		// Token: 0x06007AD6 RID: 31446 RVA: 0x00390CE4 File Offset: 0x0038EEE4
		private unsafe void DeselectBook(ItemDisplayData itemData)
		{
			ZhujianGearMateSubPageSkillBase.<>c__DisplayClass108_0 CS$<>8__locals1;
			CS$<>8__locals1.<>4__this = this;
			ItemKey itemKey = itemData.RealKey;
			short id = this._isCombatSkill ? SkillBook.Instance[itemKey.TemplateId].CombatSkillTemplateId : SkillBook.Instance[itemKey.TemplateId].LifeSkillTemplateId;
			CS$<>8__locals1.progressRead = this.GetProgressRead(id);
			CS$<>8__locals1.progressReadDefault = this.GetDefaultProgressRead(id);
			CS$<>8__locals1.pageDisplayData = this.PageDisplayData[itemKey.Id];
			this.<DeselectBook>g__ResetProgressRead|108_1(CS$<>8__locals1.pageDisplayData, CS$<>8__locals1.progressRead, CS$<>8__locals1.progressReadDefault, ref CS$<>8__locals1);
			int tempExpCost = 0;
			int* progressAddArr = stackalloc int[checked(unchecked((UIntPtr)this.PageCount) * 4)];
			CS$<>8__locals1.progressAddArr = progressAddArr;
			List<ItemKey> curTemplateItemkeyList = EasyPool.Get<List<ItemKey>>();
			curTemplateItemkeyList.Clear();
			foreach (ItemDisplayData selectedItemData in this._selectedBooks)
			{
				ItemKey bookItemkey = selectedItemData.RealKey;
				short bookItemkeyId = this._isCombatSkill ? SkillBook.Instance[bookItemkey.TemplateId].CombatSkillTemplateId : SkillBook.Instance[bookItemkey.TemplateId].LifeSkillTemplateId;
				bool flag = bookItemkeyId == id;
				if (flag)
				{
					this.<DeselectBook>g__CalcExpCost|108_0(ref tempExpCost, bookItemkey, ref CS$<>8__locals1);
					curTemplateItemkeyList.Add(bookItemkey);
				}
			}
			this._expRequirement -= tempExpCost;
			ItemDisplayData existing = this.FindSelectedBook(itemData);
			bool flag2 = existing != null;
			if (flag2)
			{
				this._selectedBooks.Remove(existing);
			}
			curTemplateItemkeyList.Remove(itemKey);
			for (int i = 0; i < this.PageCount; i++)
			{
				CS$<>8__locals1.progressAddArr[i] = 0;
			}
			this.<DeselectBook>g__ResetProgressRead|108_1(CS$<>8__locals1.pageDisplayData, CS$<>8__locals1.progressRead, CS$<>8__locals1.progressReadDefault, ref CS$<>8__locals1);
			for (int j = 0; j < curTemplateItemkeyList.Count; j++)
			{
				this.<DeselectBook>g__CalcExpCost|108_0(ref this._expRequirement, curTemplateItemkeyList[j], ref CS$<>8__locals1);
			}
			curTemplateItemkeyList.Clear();
			EasyPool.Free<List<ItemKey>>(curTemplateItemkeyList);
			bool flag3 = !this._isBatchSelectingBooks;
			if (flag3)
			{
				this.OnSelectedBookChanged();
			}
		}

		// Token: 0x06007AD7 RID: 31447 RVA: 0x00390F34 File Offset: 0x0038F134
		private void OnSelectedBookChanged()
		{
			string needStr = this._expRequirement.ToString();
			string ownStr = CommonUtils.GetDisplayStringForNum(this._exp, 100000).SetColor((this._exp >= this._expRequirement) ? "brightblue" : "brightred");
			this.propertyCost.SetValue(ownStr + "/" + needStr);
			this.UpdateConfirmButton();
			bool flag = this._selectedBooks.Count == 0;
			if (flag)
			{
				this._curBookItemData = null;
			}
			else
			{
				List<ItemDisplayData> selectedBooks = this._selectedBooks;
				this._curBookItemData = selectedBooks[selectedBooks.Count - 1];
				this._finishedCountAfter = -1;
			}
			this.previewScroll.SetDataCount(this._selectedBooks.Count);
			this.RenderSkillType(this._curBookItemData);
			this.RefreshEmptyState();
			bool flag2 = !this._isBatchSelectingBooks;
			if (flag2)
			{
				this.SyncMultiplySelection();
			}
		}

		// Token: 0x06007AD8 RID: 31448 RVA: 0x0039101C File Offset: 0x0038F21C
		private static string GetShortGradeDisplay(sbyte grade)
		{
			bool flag = ZhujianGearMateSubPageSkillBase._shortGradeDisplayCache == null;
			if (flag)
			{
				ZhujianGearMateSubPageSkillBase._shortGradeDisplayCache = new string[9];
				for (sbyte g = 0; g < 9; g += 1)
				{
					LocalStringManager.LanguageType curLanguageType = LocalStringManager.CurLanguageType;
					LanguageKey key = (curLanguageType == LocalStringManager.LanguageType.CN || curLanguageType == LocalStringManager.LanguageType.CNH) ? LanguageKey.LK_ShortGrade_0 : LanguageKey.LK_Grade_0;
					ZhujianGearMateSubPageSkillBase._shortGradeDisplayCache[(int)g] = (key + (int)g).Tr();
				}
			}
			return ZhujianGearMateSubPageSkillBase._shortGradeDisplayCache[(int)grade];
		}

		// Token: 0x06007AD9 RID: 31449 RVA: 0x00391090 File Offset: 0x0038F290
		private void RefreshEmptyState()
		{
			this.maskRoot.gameObject.SetActive(this._selectedBooks.Count != 0);
			this.rawRoot.gameObject.SetActive(this._selectedBooks.Count != 0);
			this.emptyRoot.gameObject.SetActive(this._selectedBooks.Count == 0);
		}

		// Token: 0x06007ADA RID: 31450 RVA: 0x003910FC File Offset: 0x0038F2FC
		private void ConfirmSelectedBooks()
		{
			sbyte type = this._isCombatSkill ? 8 : 9;
			foreach (ItemDisplayData itemData in this._selectedBooks)
			{
				ExtraDomainMethod.Call.UpgradeGearMate(this._displayData.GearMate.Id, type, itemData.RealKey, 1, itemData.ItemSourceTypeEnum);
			}
			this._selectedBooks.Clear();
			this._requestGearMateData();
		}

		// Token: 0x06007ADB RID: 31451 RVA: 0x00391198 File Offset: 0x0038F398
		private void UpdateConfirmButton()
		{
			this.buttonConfirm.interactable = (this._selectedBooks.Count != 0 && this._exp >= this._expRequirement);
		}

		// Token: 0x06007ADC RID: 31452 RVA: 0x003911C8 File Offset: 0x0038F3C8
		private unsafe void TryRefreshSkillPropertyPreview()
		{
			bool flag = this._curBookItemData == null || !this.skillTypeRoot.activeSelf;
			if (!flag)
			{
				bool isCombatSkill = this._isCombatSkill;
				if (isCombatSkill)
				{
					this.propertySkillQualification.SetSingleValue(this.CombatSkillQualifications[(int)this._curSkillType].ToString());
					this.propertySkillAttainment.SetSingleValue(this.CombatSkillAttainments[(int)this._curSkillType].ToString());
				}
				else
				{
					short curQual = *this.LifeSkillQualifications[(int)this._curSkillType];
					short curAtt = *this.LifeSkillAttainments[(int)this._curSkillType];
					this.RefreshSkillPropertyPreview((int)curQual, (int)curAtt);
				}
			}
		}

		// Token: 0x06007ADD RID: 31453 RVA: 0x00391288 File Offset: 0x0038F488
		private void RefreshSkillPropertyPreview(int curQualificationValue, int curAttainmentValue)
		{
			int qualificationAdd;
			int attainmentPageBonus;
			this.CollectPreviewDeltasForLifeSkillType(this._curSkillType, out qualificationAdd, out attainmentPageBonus);
			int predictedQualification = curQualificationValue + qualificationAdd;
			ZhujianGearMateSubPageSkillBase.ApplyPropertyPreview(this.propertySkillQualification, curQualificationValue, predictedQualification);
			int predictedAttainment = ZhujianGearMateSubPageSkillBase.CalcPredictedAttainment(curQualificationValue, qualificationAdd, attainmentPageBonus);
			ZhujianGearMateSubPageSkillBase.ApplyPropertyPreview(this.propertySkillAttainment, curAttainmentValue, predictedAttainment);
		}

		// Token: 0x06007ADE RID: 31454 RVA: 0x003912D0 File Offset: 0x0038F4D0
		private void CollectPreviewDeltasForLifeSkillType(sbyte lifeSkillType, out int qualificationAdd, out int attainmentPageBonus)
		{
			qualificationAdd = 0;
			attainmentPageBonus = 0;
			foreach (ItemDisplayData itemData in this.GetSelectedLifeSkillBooks(lifeSkillType))
			{
				int bookQualAdd;
				int bookAttBonus;
				this.CollectPreviewDeltasForBook(itemData, lifeSkillType, out bookQualAdd, out bookAttBonus);
				qualificationAdd += bookQualAdd;
				attainmentPageBonus += bookAttBonus;
			}
		}

		// Token: 0x06007ADF RID: 31455 RVA: 0x0039133C File Offset: 0x0038F53C
		private IEnumerable<ItemDisplayData> GetSelectedLifeSkillBooks(sbyte lifeSkillType)
		{
			foreach (ItemDisplayData itemData in this._selectedBooks)
			{
				SkillBookItem config = SkillBook.Instance[itemData.RealKey.TemplateId];
				bool flag = config.LifeSkillType == lifeSkillType;
				if (flag)
				{
					yield return itemData;
				}
				config = null;
				itemData = null;
			}
			List<ItemDisplayData>.Enumerator enumerator = default(List<ItemDisplayData>.Enumerator);
			yield break;
			yield break;
		}

		// Token: 0x06007AE0 RID: 31456 RVA: 0x00391354 File Offset: 0x0038F554
		private void CollectPreviewDeltasForBook(ItemDisplayData itemData, sbyte lifeSkillType, out int qualificationAdd, out int attainmentPageBonus)
		{
			qualificationAdd = 0;
			attainmentPageBonus = 0;
			bool flag = itemData == null;
			if (!flag)
			{
				ItemKey itemKey = itemData.RealKey;
				SkillBookItem config = SkillBook.Instance[itemKey.TemplateId];
				sbyte grade = config.Grade;
				bool flag2 = grade < 0;
				if (!flag2)
				{
					short skillBookId = config.LifeSkillTemplateId;
					int finishedCount = this.CountLifeSkillFinishedPagesAfterRead(skillBookId, itemKey);
					bool flag3 = finishedCount == this.PageCount && !this.GearMate.IsLifeSkillBuffed(lifeSkillType, grade);
					if (flag3)
					{
						qualificationAdd = (int)(2 * (grade + 1));
					}
					attainmentPageBonus = (int)(GlobalConfig.Instance.AddAttainmentPerGrade[(int)grade] / 5) * finishedCount;
				}
			}
		}

		// Token: 0x06007AE1 RID: 31457 RVA: 0x003913F0 File Offset: 0x0038F5F0
		private static int CalcPredictedAttainment(int curQualificationValue, int qualificationAdd, int attainmentPageBonus)
		{
			int qualForAttainmentFormula = curQualificationValue + qualificationAdd;
			return qualForAttainmentFormula * (100 + attainmentPageBonus) / 100 + attainmentPageBonus;
		}

		// Token: 0x06007AE2 RID: 31458 RVA: 0x00391414 File Offset: 0x0038F614
		private int CountLifeSkillFinishedPagesAfterRead(short skillBookId, ItemKey itemKey)
		{
			sbyte[] progressRead = this.GetProgressRead(skillBookId);
			int finishedCount = 0;
			for (int i = 0; i < 5; i++)
			{
				bool flag = progressRead[i] >= 100;
				if (flag)
				{
					finishedCount++;
				}
			}
			return finishedCount;
		}

		// Token: 0x06007AE3 RID: 31459 RVA: 0x0039145C File Offset: 0x0038F65C
		private static void ApplyPropertyPreview(PropertyItem property, int currentValue, int predictedValue)
		{
			bool flag = predictedValue > currentValue;
			if (flag)
			{
				property.SetValueChange(currentValue, predictedValue, true);
			}
			else
			{
				property.SetSingleValue(currentValue.ToString());
			}
		}

		// Token: 0x06007AE5 RID: 31461 RVA: 0x00391508 File Offset: 0x0038F708
		// Note: this type is marked as 'beforefieldinit'.
		static ZhujianGearMateSubPageSkillBase()
		{
			LanguageKey[] array = new LanguageKey[4];
			RuntimeHelpers.InitializeArray(array, fieldof(<PrivateImplementationDetails>.7F969F4C30A4206BCD2135F5F4BDE78E8206817B692D7CA5A622EBFBD4473D4B).FieldHandle);
			ZhujianGearMateSubPageSkillBase.AttainmentLevelName = array;
		}

		// Token: 0x06007AE8 RID: 31464 RVA: 0x00391594 File Offset: 0x0038F794
		[CompilerGenerated]
		private void <RenderSkillType>g__RefreshTip|98_0(int alreadyAdd = 0, ref ZhujianGearMateSubPageSkillBase.<>c__DisplayClass98_0 A_2)
		{
			string skillIconName = this._isCombatSkill ? CombatSkillType.Instance[this._curSkillType].TipsIcon : Config.LifeSkillType.Instance[this._curSkillType].Icon;
			string gearMateName = NameCenter.GetMonasticTitleOrDisplayName(this._displayData.GearMateDisplayData, false);
			A_2.disPlayer.Type = TipType.GeneralLines;
			TooltipInvoker disPlayer = A_2.disPlayer;
			if (disPlayer.RuntimeParam == null)
			{
				disPlayer.RuntimeParam = new ArgumentBox();
			}
			A_2.disPlayer.RuntimeParam.Set("Title", LocalStringManager.Get(LanguageKey.UI_MouseTipGearMateSkillQualification_0));
			int lineCount = 0;
			string skillNameAndIcon = "<SpName=" + skillIconName + ">" + A_2.skillTypeName;
			string lineOne = LocalStringManager.GetFormat(LanguageKey.UI_MouseTipGearMateSkillQualification_1, skillNameAndIcon, gearMateName, skillNameAndIcon);
			string lineTwo = LocalStringManager.GetFormat(LanguageKey.UI_MouseTipGearMateSkillQualification_2, alreadyAdd);
			A_2.disPlayer.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData(7, new List<string>
			{
				lineOne
			}, null));
			A_2.disPlayer.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData(5, new List<string>
			{
				lineTwo
			}, null));
			A_2.disPlayer.RuntimeParam.Set("LineCount", lineCount);
			A_2.disPlayer.Refresh(false, -1);
			A_2.disPlayer.enabled = true;
		}

		// Token: 0x06007AE9 RID: 31465 RVA: 0x00391718 File Offset: 0x0038F918
		[CompilerGenerated]
		private bool <RenderPagesPreview>g__RenderSingleChapter|99_0(int i, ref ZhujianGearMateSubPageSkillBase.<>c__DisplayClass99_0 A_2)
		{
			byte realId = this.GetSkillBookRealPage(A_2.pageDisplayData, i);
			sbyte progressAfter = A_2.progressRead[(int)realId];
			bool isCombatSkill = this._isCombatSkill;
			if (isCombatSkill)
			{
				bool flag = i == 0;
				if (flag)
				{
					sbyte type = A_2.pageDisplayData.Type[0];
					A_2.bookIds.Add((int)type);
				}
				else
				{
					int idx = (A_2.pageDisplayData.Type[i] == 0) ? (4 + i) : (9 + i);
					A_2.bookIds.Add(idx);
				}
			}
			bool isCombatSkill2 = this._isCombatSkill;
			bool result;
			if (isCombatSkill2)
			{
				bool flag2 = realId < 5;
				if (flag2)
				{
					result = (A_2.progressRead[0] >= 100 || A_2.progressRead[1] >= 100 || A_2.progressRead[2] >= 100 || A_2.progressRead[3] >= 100 || A_2.progressRead[4] >= 100 || progressAfter >= 100);
				}
				else
				{
					bool flag3 = realId < 10;
					if (flag3)
					{
						result = (progressAfter >= 100 || A_2.progressRead[(int)(realId + 5)] >= 100);
					}
					else
					{
						result = (progressAfter >= 100 || A_2.progressRead[(int)(realId - 5)] >= 100);
					}
				}
			}
			else
			{
				result = (progressAfter >= 100);
			}
			return result;
		}

		// Token: 0x06007AEA RID: 31466 RVA: 0x00391860 File Offset: 0x0038FA60
		[CompilerGenerated]
		private bool <RenderPagesPreview>g__RenderSingleChapter|100_0(int i, ref ZhujianGearMateSubPageSkillBase.<>c__DisplayClass100_0 A_2)
		{
			byte realId = this.GetSkillBookRealPage(A_2.pageDisplayData, i);
			sbyte progressAfter = A_2.progressRead[(int)realId];
			return progressAfter >= 100;
		}

		// Token: 0x06007AEB RID: 31467 RVA: 0x00391894 File Offset: 0x0038FA94
		[CompilerGenerated]
		private unsafe void <DeselectBook>g__CalcExpCost|108_0(ref int exp, ItemKey itemKey, ref ZhujianGearMateSubPageSkillBase.<>c__DisplayClass108_0 A_3)
		{
			exp += this.GetBookRequiredExp(itemKey);
			this.GetProgressAdd(itemKey, ref A_3.progressAddArr);
			for (int i = 0; i < this.PageCount; i++)
			{
				byte realId = this.GetSkillBookRealPage(A_3.pageDisplayData, i);
				A_3.progressRead[(int)realId] = (sbyte)Mathf.Min(A_3.progressAddArr[realId] + (int)A_3.progressReadDefault[(int)realId], 100);
			}
		}

		// Token: 0x06007AEC RID: 31468 RVA: 0x00391908 File Offset: 0x0038FB08
		[CompilerGenerated]
		private void <DeselectBook>g__ResetProgressRead|108_1(SkillBookPageDisplayData pageDisplayData, sbyte[] progressRead, sbyte[] progressReadDefault, ref ZhujianGearMateSubPageSkillBase.<>c__DisplayClass108_0 A_4)
		{
			for (int i = 0; i < this.PageCount; i++)
			{
				byte realId = this.GetSkillBookRealPage(pageDisplayData, i);
				progressRead[(int)realId] = progressReadDefault[(int)realId];
			}
		}

		// Token: 0x04005CEE RID: 23790
		[SerializeField]
		private ManagedMultiplyItemListScroll multiplyItemListScroll;

		// Token: 0x04005CEF RID: 23791
		[SerializeField]
		private CToggleGroup toggleGroupItemSource;

		// Token: 0x04005CF0 RID: 23792
		[SerializeField]
		private InfinityScroll previewScroll;

		// Token: 0x04005CF1 RID: 23793
		[SerializeField]
		private GearMateCombatSkillBookPreview combatSkillBookPreview;

		// Token: 0x04005CF2 RID: 23794
		[SerializeField]
		private GearMateLifeSkillBookPreview lifeSkillBookPreview;

		// Token: 0x04005CF3 RID: 23795
		[SerializeField]
		private GameObject skillTypeRoot;

		// Token: 0x04005CF4 RID: 23796
		[SerializeField]
		private CImage imageSkillIcon;

		// Token: 0x04005CF5 RID: 23797
		[SerializeField]
		private TextMeshProUGUI textSkillName;

		// Token: 0x04005CF6 RID: 23798
		[SerializeField]
		private TextMeshProUGUI textSkillLevel;

		// Token: 0x04005CF7 RID: 23799
		[SerializeField]
		private PropertyItem propertySkillQualification;

		// Token: 0x04005CF8 RID: 23800
		[SerializeField]
		private PropertyItem propertySkillAttainment;

		// Token: 0x04005CF9 RID: 23801
		[SerializeField]
		private GameObject[] gradeArray;

		// Token: 0x04005CFA RID: 23802
		[SerializeField]
		private TooltipInvoker tipSkillArea;

		// Token: 0x04005CFB RID: 23803
		[SerializeField]
		private PropertyItem propertyCost;

		// Token: 0x04005CFC RID: 23804
		[SerializeField]
		private CButton buttonConfirm;

		// Token: 0x04005CFD RID: 23805
		[SerializeField]
		private TextMeshProUGUI textConfirm;

		// Token: 0x04005CFE RID: 23806
		[SerializeField]
		private TooltipInvoker tipButtonConfirm;

		// Token: 0x04005CFF RID: 23807
		[SerializeField]
		private UIParticle uiParticle;

		// Token: 0x04005D00 RID: 23808
		[SerializeField]
		private SkeletonGraphic skeletonGraphic;

		// Token: 0x04005D01 RID: 23809
		[SerializeField]
		private GameObject rawRoot;

		// Token: 0x04005D02 RID: 23810
		[SerializeField]
		private GameObject maskRoot;

		// Token: 0x04005D03 RID: 23811
		[SerializeField]
		private GameObject emptyRoot;

		// Token: 0x04005D04 RID: 23812
		private SectZhujianGearMateSkillDisplayData _displayData;

		// Token: 0x04005D05 RID: 23813
		private DOTweenAnimation _curGradeAnim;

		// Token: 0x04005D06 RID: 23814
		private static readonly LanguageKey[] AttainmentLevelName;

		// Token: 0x04005D07 RID: 23815
		private bool _isCombatSkill = true;

		// Token: 0x04005D08 RID: 23816
		private readonly List<ItemDisplayData> _availableBookList = new List<ItemDisplayData>();

		// Token: 0x04005D09 RID: 23817
		private readonly List<ItemDisplayData> _filteredSourceBuffer = new List<ItemDisplayData>();

		// Token: 0x04005D0A RID: 23818
		private readonly List<ItemDisplayData> _selectAllWorkList = new List<ItemDisplayData>();

		// Token: 0x04005D0B RID: 23819
		private bool _isBatchSelectingBooks;

		// Token: 0x04005D0C RID: 23820
		private readonly List<ItemDisplayData> _selectedBooks = new List<ItemDisplayData>();

		// Token: 0x04005D0D RID: 23821
		private readonly Dictionary<short, sbyte[]> _pageProgressDic = new Dictionary<short, sbyte[]>();

		// Token: 0x04005D0E RID: 23822
		private readonly Dictionary<short, sbyte[]> _pageProgressDicDefault = new Dictionary<short, sbyte[]>();

		// Token: 0x04005D0F RID: 23823
		private readonly Dictionary<short, sbyte> _bookTotalProgressDic = new Dictionary<short, sbyte>();

		// Token: 0x04005D10 RID: 23824
		private UIParticle _curGradeParticle;

		// Token: 0x04005D11 RID: 23825
		private sbyte _curSkillType;

		// Token: 0x04005D12 RID: 23826
		private int _finishedCountAfter = -1;

		// Token: 0x04005D13 RID: 23827
		private readonly List<CombatSkillHelper.AttainmentSectInfo> _sectInfos = new List<CombatSkillHelper.AttainmentSectInfo>(9);

		// Token: 0x04005D14 RID: 23828
		private int _expRequirement;

		// Token: 0x04005D15 RID: 23829
		private ItemDisplayData _curBookItemData;

		// Token: 0x04005D16 RID: 23830
		private bool _isQualificationLevelUp;

		// Token: 0x04005D17 RID: 23831
		private ViewZhujianGearMate _parentView;

		// Token: 0x04005D18 RID: 23832
		private Action _requestGearMateData;

		// Token: 0x04005D19 RID: 23833
		private SortAndFilter _bookListSortAndFilter;

		// Token: 0x04005D1A RID: 23834
		private bool _multiplyReady;

		// Token: 0x04005D1B RID: 23835
		private static string[] _shortGradeDisplayCache;

		// Token: 0x02001F3B RID: 7995
		private enum ItemSourceTogKey
		{
			// Token: 0x0400CCEC RID: 52460
			Inventory,
			// Token: 0x0400CCED RID: 52461
			Warehouse,
			// Token: 0x0400CCEE RID: 52462
			Treasury
		}
	}
}
