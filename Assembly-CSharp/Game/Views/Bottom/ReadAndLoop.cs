using System;
using System.Runtime.CompilerServices;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Item;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.CombatSkill;
using GameData.Domains.Extra;
using GameData.Domains.Global;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Taiwu;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Views.Bottom
{
	// Token: 0x02000C42 RID: 3138
	public class ReadAndLoop : MonoBehaviour
	{
		// Token: 0x170010D0 RID: 4304
		// (get) Token: 0x06009F53 RID: 40787 RVA: 0x004A7374 File Offset: 0x004A5574
		internal CombatSkillDisplayData LoopingSkillDisplayData
		{
			get
			{
				return this._loopingSkillDisplayData;
			}
		}

		// Token: 0x170010D1 RID: 4305
		// (get) Token: 0x06009F54 RID: 40788 RVA: 0x004A737C File Offset: 0x004A557C
		internal int[] ExtraNeiliAllocationProgress
		{
			get
			{
				return this._extraNeiliAllocationProgress;
			}
		}

		// Token: 0x170010D2 RID: 4306
		// (get) Token: 0x06009F55 RID: 40789 RVA: 0x004A7384 File Offset: 0x004A5584
		internal sbyte CachedNeiliType
		{
			get
			{
				return this._cachedNeiliType;
			}
		}

		// Token: 0x170010D3 RID: 4307
		// (get) Token: 0x06009F56 RID: 40790 RVA: 0x004A738C File Offset: 0x004A558C
		internal SkillBookPageDisplayData BookPagesInfo
		{
			get
			{
				return this._bookPagesInfo;
			}
		}

		// Token: 0x06009F57 RID: 40791 RVA: 0x004A7394 File Offset: 0x004A5594
		private void Awake()
		{
			this.InitActiveCostTipInvoker(this.skillCostTipDisplayer);
			this.InitActiveCostTipInvoker(this.readCostTipDisplayer);
			this.skillButton.onClick.ResetListener(new Action(this.<Awake>g__SkillButton|58_0));
			this.readButton.onClick.ResetListener(new Action(ReadAndLoop.<Awake>g__ReadButton|58_1));
		}

		// Token: 0x06009F58 RID: 40792 RVA: 0x004A73F8 File Offset: 0x004A55F8
		private void OnEnable()
		{
			GEvent.Add(EEvents.OnActionPointChange, new GEvent.Callback(this.RequestData));
			GEvent.Add(UiEvents.TopUiChanged, new GEvent.Callback(this.RequestData));
			GEvent.Add(UiEvents.MapPickupDataChanged, new GEvent.Callback(this.RequestData));
			GEvent.Add(UiEvents.OnTaiwuReadingBookProgressMayChange, new GEvent.Callback(this.OnReadingBookProgressMayChange));
		}

		// Token: 0x06009F59 RID: 40793 RVA: 0x004A7470 File Offset: 0x004A5670
		private void OnDisable()
		{
			GEvent.Remove(EEvents.OnActionPointChange, new GEvent.Callback(this.RequestData));
			GEvent.Remove(UiEvents.TopUiChanged, new GEvent.Callback(this.RequestData));
			GEvent.Remove(UiEvents.MapPickupDataChanged, new GEvent.Callback(this.RequestData));
			GEvent.Remove(UiEvents.OnTaiwuReadingBookProgressMayChange, new GEvent.Callback(this.OnReadingBookProgressMayChange));
		}

		// Token: 0x06009F5A RID: 40794 RVA: 0x004A74E7 File Offset: 0x004A56E7
		private void OnReadingBookProgressMayChange(ArgumentBox _)
		{
			this.RequestReadingBookPagesInfo(this._auxiliaryTipDataRequestId);
		}

		// Token: 0x06009F5B RID: 40795 RVA: 0x004A74F8 File Offset: 0x004A56F8
		private void RequestData(ArgumentBox _)
		{
			bool flag = !this._processing;
			if (flag)
			{
				this.RequestData();
			}
		}

		// Token: 0x06009F5C RID: 40796 RVA: 0x004A751A File Offset: 0x004A571A
		public void Init(IAsyncMethodRequestHandler parent)
		{
			this._parent = parent;
		}

		// Token: 0x06009F5D RID: 40797 RVA: 0x004A7523 File Offset: 0x004A5723
		public void RequestData()
		{
			TaiwuDomainMethod.AsyncCall.RequestReadingAndLooping(this._parent, delegate(int offset, RawDataPool dataPool)
			{
				this._processing = true;
				Serializer.Deserialize(dataPool, offset, ref this.Data);
				bool flag = !this._achievement1 && this.Data.LoopingHasEvent;
				if (flag)
				{
					this._achievement1 = true;
					GlobalDomainMethod.Call.InvokeGuidingTrigger(306);
				}
				bool flag2 = !this._achievement2 && this.Data.BookHasEvent;
				if (flag2)
				{
					this._achievement2 = true;
					GlobalDomainMethod.Call.InvokeGuidingTrigger(307);
				}
				this.Refresh();
				this.RequestAuxiliaryTipData();
			});
		}

		// Token: 0x06009F5E RID: 40798 RVA: 0x004A7540 File Offset: 0x004A5740
		private void RequestAuxiliaryTipData()
		{
			ReadAndLoop.<>c__DisplayClass73_0 CS$<>8__locals1 = new ReadAndLoop.<>c__DisplayClass73_0();
			CS$<>8__locals1.<>4__this = this;
			bool flag = this._parent == null;
			if (!flag)
			{
				ReadAndLoop.<>c__DisplayClass73_0 CS$<>8__locals2 = CS$<>8__locals1;
				int num = this._auxiliaryTipDataRequestId + 1;
				this._auxiliaryTipDataRequestId = num;
				CS$<>8__locals2.requestId = num;
				int taiwuId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
				CharacterDomainMethod.AsyncCall.GetExtraNeiliAllocationProgress(this._parent, taiwuId, delegate(int offset, RawDataPool pool)
				{
					bool flag3 = CS$<>8__locals1.requestId != CS$<>8__locals1.<>4__this._auxiliaryTipDataRequestId;
					if (!flag3)
					{
						Serializer.Deserialize(pool, offset, ref CS$<>8__locals1.<>4__this._extraNeiliAllocationProgress);
						CS$<>8__locals1.<>4__this.RefreshSkillLoopProgress();
					}
				});
				bool flag2 = this.Data.LoopingId >= 0;
				if (flag2)
				{
					CombatSkillDomainMethod.AsyncCall.GetCombatSkillDisplayDataOnce(this._parent, taiwuId, this.Data.LoopingId, delegate(int offset, RawDataPool pool)
					{
						bool flag3 = CS$<>8__locals1.requestId != CS$<>8__locals1.<>4__this._auxiliaryTipDataRequestId;
						if (!flag3)
						{
							Serializer.Deserialize(pool, offset, ref CS$<>8__locals1.<>4__this._loopingSkillDisplayData);
							CS$<>8__locals1.<>4__this.RefreshSkillLoopProgress();
						}
					});
					CharacterDomainMethod.AsyncCall.GetCharacterDisplayDataForNeiliPage(this._parent, taiwuId, delegate(int offset, RawDataPool pool)
					{
						bool flag3 = CS$<>8__locals1.requestId != CS$<>8__locals1.<>4__this._auxiliaryTipDataRequestId;
						if (!flag3)
						{
							CharacterDisplayDataForNeiliPage neiliPageData = null;
							Serializer.Deserialize(pool, offset, ref neiliPageData);
							CS$<>8__locals1.<>4__this._cachedNeiliType = ((neiliPageData != null) ? neiliPageData.NeiliType : -1);
							CS$<>8__locals1.<>4__this.RefreshSkillLoopProgress();
						}
					});
				}
				else
				{
					this._loopingSkillDisplayData = null;
					this._cachedNeiliType = -1;
					this.RefreshSkillLoopProgress();
				}
				this.RequestReadingBookPagesInfo(CS$<>8__locals1.requestId);
			}
		}

		// Token: 0x06009F5F RID: 40799 RVA: 0x004A7624 File Offset: 0x004A5824
		private void RequestReadingBookPagesInfo(int requestId)
		{
			bool flag = this._parent == null;
			if (!flag)
			{
				bool flag2 = !this.Data.BookKey.IsValid();
				if (flag2)
				{
					this._bookPagesInfo = null;
					this.RefreshReadingBookProgress(this.Data.BookKey, this._bookPagesInfo);
				}
				else
				{
					ItemDomainMethod.AsyncCall.GetSkillBookPagesInfo(this._parent, this.Data.BookKey, delegate(int offset, RawDataPool pool)
					{
						bool flag3 = requestId != this._auxiliaryTipDataRequestId;
						if (!flag3)
						{
							Serializer.Deserialize(pool, offset, ref this._bookPagesInfo);
							this.RefreshReadingBookProgress(this.Data.BookKey, this._bookPagesInfo);
						}
					});
				}
			}
		}

		// Token: 0x06009F60 RID: 40800 RVA: 0x004A76B4 File Offset: 0x004A58B4
		private void RefreshReadingBookProgress(ItemKey currentReadingBookKey, SkillBookPageDisplayData pagesInfo)
		{
			bool isCombatBook = pagesInfo != null && pagesInfo.IsCombatBook;
			bool hasBook = currentReadingBookKey.IsValid() && ((pagesInfo != null) ? pagesInfo.ReadingProgress : null) != null && pagesInfo.ReadingProgress.Length != 0;
			bool flag = this.readingBookPartition5;
			if (flag)
			{
				this.readingBookPartition5.SetActive(hasBook && !isCombatBook);
			}
			bool flag2 = this.readingBookPartition6;
			if (flag2)
			{
				this.readingBookPartition6.SetActive(hasBook && isCombatBook);
			}
			bool flag3 = !hasBook;
			if (flag3)
			{
				this._activeReadingBookProgressFill = null;
				this._targetReadingBookProgress = 0f;
				this._currentReadingBookProgress = 0f;
				ReadAndLoop.SetReadingBookProgressFill(this.readingBookProgressFill5, 0f);
				ReadAndLoop.SetReadingBookProgressFill(this.readingBookProgressFill6, 0f);
			}
			else
			{
				this._activeReadingBookProgressFill = (isCombatBook ? this.readingBookProgressFill6 : this.readingBookProgressFill5);
				ReadAndLoop.SetReadingBookProgressFill(isCombatBook ? this.readingBookProgressFill5 : this.readingBookProgressFill6, 0f);
				this._targetReadingBookProgress = ReadAndLoop.CalcReadingBookTotalProgressRatio(pagesInfo);
			}
		}

		// Token: 0x06009F61 RID: 40801 RVA: 0x004A77C4 File Offset: 0x004A59C4
		private static float CalcReadingBookTotalProgressRatio(SkillBookPageDisplayData pagesInfo)
		{
			int totalProgress = 0;
			foreach (sbyte p in pagesInfo.ReadingProgress)
			{
				totalProgress += (int)p;
			}
			int maxProgress = pagesInfo.ReadingProgress.Length * 100;
			return (maxProgress > 0) ? ((float)totalProgress / (float)maxProgress) : 0f;
		}

		// Token: 0x06009F62 RID: 40802 RVA: 0x004A7818 File Offset: 0x004A5A18
		private static void SetReadingBookProgressFill(CImage fill, float amount)
		{
			bool flag = fill;
			if (flag)
			{
				fill.fillAmount = amount;
			}
		}

		// Token: 0x06009F63 RID: 40803 RVA: 0x004A7838 File Offset: 0x004A5A38
		private void UpdateReadingBookProgressTransition()
		{
			bool flag = this._activeReadingBookProgressFill == null;
			if (!flag)
			{
				bool flag2 = Mathf.Abs(this._currentReadingBookProgress - this._targetReadingBookProgress) < 0.001f;
				if (flag2)
				{
					this._currentReadingBookProgress = this._targetReadingBookProgress;
					this._activeReadingBookProgressFill.fillAmount = this._currentReadingBookProgress;
				}
				else
				{
					this._currentReadingBookProgress = Mathf.Lerp(this._currentReadingBookProgress, this._targetReadingBookProgress, Time.deltaTime * 5f);
					this._activeReadingBookProgressFill.fillAmount = this._currentReadingBookProgress;
				}
			}
		}

		// Token: 0x06009F64 RID: 40804 RVA: 0x004A78CC File Offset: 0x004A5ACC
		private void RefreshSkillLoopProgress()
		{
			bool hasLoop = this.Data.LoopingId >= 0;
			bool neiliFull = hasLoop && this.IsSkillLoopNeiliFull();
			bool flag = this.skillLoopPartition4;
			if (flag)
			{
				this.skillLoopPartition4.SetActive(neiliFull);
			}
			bool flag2 = this.skillLoopProgressFill4;
			if (flag2)
			{
				this.skillLoopProgressFill4.gameObject.SetActive(hasLoop && !neiliFull);
			}
			bool flag3 = !hasLoop;
			if (flag3)
			{
				this._hasSkillLoopProgress = false;
				this._targetSkillLoopProgress = 0f;
				this._currentSkillLoopProgress = 0f;
				ReadAndLoop.SetReadingBookProgressFill(this.skillLoopProgressFill4, 0f);
				this.ResetLoop4AllocationProgress();
				this.RefreshLoopFinishedAndEvent();
			}
			else
			{
				bool flag4 = !neiliFull;
				if (flag4)
				{
					this._hasSkillLoopProgress = true;
					this._targetSkillLoopProgress = this.CalcSkillLoopTotalProgressRatio();
					this.ResetLoop4AllocationProgress();
				}
				else
				{
					this._hasSkillLoopProgress = false;
					this._targetSkillLoopProgress = 1f;
					this._currentSkillLoopProgress = 1f;
					ReadAndLoop.SetReadingBookProgressFill(this.skillLoopProgressFill4, 1f);
					this.RefreshLoop4AllocationProgress();
				}
				this.RefreshLoopFinishedAndEvent();
			}
		}

		// Token: 0x06009F65 RID: 40805 RVA: 0x004A79EE File Offset: 0x004A5BEE
		private bool IsSkillLoopNeiliFull()
		{
			return this._loopingSkillDisplayData != null && this._loopingSkillDisplayData.MaxObtainableNeili > 0 && this._loopingSkillDisplayData.ObtainedNeili >= this._loopingSkillDisplayData.MaxObtainableNeili;
		}

		// Token: 0x06009F66 RID: 40806 RVA: 0x004A7A24 File Offset: 0x004A5C24
		private void ResetLoop4AllocationProgress()
		{
			bool flag = this.loop4 == null;
			if (!flag)
			{
				for (int i = 0; i < this.loop4.Length; i++)
				{
					ReadAndLoop.SetReadingBookProgressFill(this.loop4[i], 0f);
				}
			}
		}

		// Token: 0x06009F67 RID: 40807 RVA: 0x004A7A6C File Offset: 0x004A5C6C
		private void RefreshLoop4AllocationProgress()
		{
			bool flag = this.loop4 == null || this._extraNeiliAllocationProgress == null;
			if (!flag)
			{
				int i = 0;
				while (i < this.loop4.Length && i < 4)
				{
					int progress = (i < this._extraNeiliAllocationProgress.Length) ? this._extraNeiliAllocationProgress[i] : 0;
					ReadAndLoop.SetReadingBookProgressFill(this.loop4[i], ReadAndLoop.CalcExtraNeiliAllocationFillRatio(progress));
					i++;
				}
			}
		}

		// Token: 0x06009F68 RID: 40808 RVA: 0x004A7AE0 File Offset: 0x004A5CE0
		private static float CalcExtraNeiliAllocationFillRatio(int allocationProgress)
		{
			bool flag = allocationProgress >= LoopingCommonUtils.GetNeiliAllocationMaxProgress();
			float result;
			if (flag)
			{
				result = 0.25f;
			}
			else
			{
				ValueTuple<int, float> valueTuple = LoopingCommonUtils.CalculateNeiliProgressInfo(allocationProgress, -1, -1);
				int completed = valueTuple.Item1;
				float progressPercentage = valueTuple.Item2;
				short maxAllocation = GlobalConfig.Instance.MaxExtraNeiliAllocation;
				bool flag2 = maxAllocation <= 0;
				if (flag2)
				{
					result = 0f;
				}
				else
				{
					float typeProgress = Mathf.Clamp01(((float)completed + progressPercentage) / (float)maxAllocation);
					result = typeProgress * 0.25f;
				}
			}
			return result;
		}

		// Token: 0x06009F69 RID: 40809 RVA: 0x004A7B58 File Offset: 0x004A5D58
		private bool IsAllExtraNeiliAllocationComplete()
		{
			bool flag = this.Data.LoopingId < 0 || !this.IsSkillLoopNeiliFull() || this._extraNeiliAllocationProgress == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				int maxProgress = LoopingCommonUtils.GetNeiliAllocationMaxProgress();
				for (int i = 0; i < 4; i++)
				{
					bool flag2 = this._extraNeiliAllocationProgress.Length <= i || this._extraNeiliAllocationProgress[i] < maxProgress;
					if (flag2)
					{
						return false;
					}
				}
				result = true;
			}
			return result;
		}

		// Token: 0x06009F6A RID: 40810 RVA: 0x004A7BD4 File Offset: 0x004A5DD4
		private void RefreshLoopFinishedAndEvent()
		{
			bool flag = this.Data.LoopingId < 0;
			if (flag)
			{
				this.loopFinished.SetActive(false);
				this.loopEventParticle.gameObject.SetActive(false);
			}
			else
			{
				this.loopFinished.SetActive(this.Data.LoopingFinished);
				this.loopEventParticle.gameObject.SetActive(this.Data.LoopingHasEvent);
			}
		}

		// Token: 0x06009F6B RID: 40811 RVA: 0x004A7C4C File Offset: 0x004A5E4C
		private float CalcSkillLoopTotalProgressRatio()
		{
			bool flag = this._loopingSkillDisplayData == null || this._loopingSkillDisplayData.MaxObtainableNeili <= 0;
			float result;
			if (flag)
			{
				result = 0f;
			}
			else
			{
				result = Mathf.Clamp01((float)this._loopingSkillDisplayData.ObtainedNeili / (float)this._loopingSkillDisplayData.MaxObtainableNeili);
			}
			return result;
		}

		// Token: 0x06009F6C RID: 40812 RVA: 0x004A7CA4 File Offset: 0x004A5EA4
		private void UpdateSkillLoopProgressTransition()
		{
			bool flag = !this._hasSkillLoopProgress || this.skillLoopProgressFill4 == null;
			if (!flag)
			{
				bool flag2 = Mathf.Abs(this._currentSkillLoopProgress - this._targetSkillLoopProgress) < 0.001f;
				if (flag2)
				{
					this._currentSkillLoopProgress = this._targetSkillLoopProgress;
					this.skillLoopProgressFill4.fillAmount = this._currentSkillLoopProgress;
				}
				else
				{
					this._currentSkillLoopProgress = Mathf.Lerp(this._currentSkillLoopProgress, this._targetSkillLoopProgress, Time.deltaTime * 5f);
					this.skillLoopProgressFill4.fillAmount = this._currentSkillLoopProgress;
				}
			}
		}

		// Token: 0x06009F6D RID: 40813 RVA: 0x004A7D44 File Offset: 0x004A5F44
		private void InitActiveCostTipInvoker(TooltipInvoker displayer)
		{
			bool flag = !displayer;
			if (!flag)
			{
				if (displayer.RuntimeParam == null)
				{
					displayer.RuntimeParam = EasyPool.Get<ArgumentBox>();
				}
				displayer.RuntimeParam.SetObject("Source", this);
			}
		}

		// Token: 0x06009F6E RID: 40814 RVA: 0x004A7D8C File Offset: 0x004A5F8C
		private void RefreshTips()
		{
			bool flag = !this.activeReadTipDisplayer || !this.activeLoopTipDisplayer;
			if (!flag)
			{
				ref string ptr = ref this.activeReadTipDisplayer.PresetParam[0];
				ref string ptr2 = ref this.activeReadTipDisplayer.PresetParam[1];
				ref string ptr3 = ref this.activeLoopTipDisplayer.PresetParam[0];
				ref string ptr4 = ref this.activeLoopTipDisplayer.PresetParam[1];
				string text = LanguageKey.LK_ReadingProgressStatus_Tips_Title.Tr();
				int num = (int)(this.Data.ActiveReadingProgress / 10);
				if (!true)
				{
				}
				string arg;
				switch (num)
				{
				case 0:
					arg = "brightblue";
					break;
				case 1:
					arg = "pinkyellow";
					break;
				case 2:
					arg = "brightred";
					break;
				default:
					arg = "grey";
					break;
				}
				if (!true)
				{
				}
				string text2 = LanguageKey.LK_ReadingProgressStatus_Tips_New_Desc.TrFormat(arg, GlobalConfig.Instance.ActiveReadProgressAffectedEfficiency[Math.Min(GlobalConfig.Instance.ActiveReadProgressAffectedEfficiency.Length - 1, (int)(this.Data.ActiveReadingProgress / 10))]).ColorReplace();
				string text3 = LanguageKey.LK_LoopingProgressStatus_Tips_Title.Tr();
				int num2 = (int)(this.Data.ActiveLoopingProgress / 10);
				if (!true)
				{
				}
				string arg2;
				switch (num2)
				{
				case 0:
					arg2 = "brightblue";
					break;
				case 1:
					arg2 = "pinkyellow";
					break;
				case 2:
					arg2 = "brightred";
					break;
				default:
					arg2 = "grey";
					break;
				}
				if (!true)
				{
				}
				string text4 = LanguageKey.LK_LoopingProgressStatus_Tips_New_Desc.TrFormat(arg2, GlobalConfig.Instance.ActiveLoopProgressAffectedEfficiency[Math.Min(GlobalConfig.Instance.ActiveLoopProgressAffectedEfficiency.Length - 1, (int)(this.Data.ActiveLoopingProgress / 10))]).ColorReplace();
				ptr = text;
				ptr2 = text2;
				ptr3 = text3;
				ptr4 = text4;
			}
		}

		// Token: 0x06009F6F RID: 40815 RVA: 0x004A7F60 File Offset: 0x004A6160
		public void Refresh()
		{
			bool flag = this.Data.BookKey.IsValid() && !this.Data.BookFinish;
			if (flag)
			{
				this.readIcon.gameObject.SetActive(true);
				this.readIcon.Set(new ItemDisplayData(this.Data.BookKey.ItemType, this.Data.BookKey.TemplateId), false);
			}
			else
			{
				this.readIcon.gameObject.SetActive(false);
			}
			CombatSkillItem skillCfg = CombatSkill.Instance[this.Data.LoopingId];
			bool flag2 = skillCfg != null;
			if (flag2)
			{
				this.skillIcon.enabled = true;
				this.skillIcon.SetSprite(skillCfg.Icon, false, null);
				this.skillIcon.SetColor(Colors.Instance.FiveElementsColors[(int)skillCfg.FiveElements]);
			}
			else
			{
				this.skillIcon.enabled = false;
				this.skillIcon.SetColor(Color.white);
			}
			SkillBookItem bookConfigData = this.Data.BookKey.IsValid() ? SkillBook.Instance.GetItem(this.Data.BookId) : null;
			TooltipInvoker tooltipInvoker = this.readTipDisplayer;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
			}
			bool flag3 = bookConfigData != null;
			if (flag3)
			{
				this.readTipDisplayer.Type = TipType.ReadingBook;
				this.readTipDisplayer.RuntimeParam.SetObject("currentReadingBookKey", this.Data.BookKey);
				this.readTipDisplayer.RuntimeParam.SetObject("referenceBooks", this.Data.ReferenceBook ?? ReadAndLoop.InvalidKeys);
				this.readFinished.SetActive(this.Data.BookFinish);
				this.readEventParticle.gameObject.SetActive(this.Data.BookHasEvent);
			}
			else
			{
				this.readTipDisplayer.Type = TipType.Simple;
				this.readTipDisplayer.RuntimeParam.Set("arg0", LocalStringManager.Get(LanguageKey.LK_Reading_Title));
				this.readTipDisplayer.RuntimeParam.Set("arg1", LocalStringManager.Get(LanguageKey.LK_Read_Tip_None));
				this.readFinished.SetActive(false);
				this.readEventParticle.gameObject.SetActive(false);
			}
			this.loopTipDisplayer.Type = ((this.Data.LoopingId >= 0) ? TipType.CombatSkill : TipType.Simple);
			this.loopTipDisplayer.NeedRefresh = (this.Data.LoopingId >= 0);
			tooltipInvoker = this.loopTipDisplayer;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = new ArgumentBox();
			}
			bool flag4 = this.Data.LoopingId >= 0;
			if (flag4)
			{
				this.loopTipDisplayer.RuntimeParam.Set("CombatSkillId", this.Data.LoopingId);
				this.loopTipDisplayer.RuntimeParam.Set("CharId", SingletonObject.getInstance<BasicGameData>().TaiwuCharId);
				this.RefreshLoopFinishedAndEvent();
			}
			else
			{
				this.loopTipDisplayer.RuntimeParam.Set("arg0", LocalStringManager.Get(LanguageKey.LK_Neigong_Looping));
				this.loopTipDisplayer.RuntimeParam.Set("arg1", LocalStringManager.Get(LanguageKey.LK_Neigong_Looping_Tip_None));
				this.loopFinished.SetActive(false);
				this.loopEventParticle.gameObject.SetActive(false);
			}
			this.skillFill.fillAmount = ReadAndLoop.CalcActiveStageProgressFill((int)this.Data.ActiveLoopingProgress, (int)GlobalConfig.Instance.MaxActiveNeigongLoopingProgress);
			this.readFill.fillAmount = ReadAndLoop.CalcActiveStageProgressFill((int)this.Data.ActiveReadingProgress, (int)GlobalConfig.Instance.MaxActiveReadingProgress);
			this.skillStatus.sprite = this.activeLoopOrReadStatus[Math.Min((int)(this.Data.ActiveLoopingProgress / 10), this.activeLoopOrReadStatus.Length - 1)];
			this.readStatus.sprite = this.activeLoopOrReadStatus[Math.Min((int)(this.Data.ActiveReadingProgress / 10), this.activeLoopOrReadStatus.Length - 1)];
			this.RefreshActiveProgressSounds();
			this.RequestAuxiliaryTipData();
			this.RefreshTips();
			this.RefreshButton();
			this._processing = false;
		}

		// Token: 0x06009F70 RID: 40816 RVA: 0x004A83BC File Offset: 0x004A65BC
		private unsafe void RefreshButton()
		{
			short concentrationCost = GlobalConfig.Instance.ActiveNeigongLoopingAttributeCost;
			short haveConcentration = *this.Data.MainAttributes[2];
			this.skillCost.text = LanguageKey.LK_ActiveLoop_Tip_Cost.TrFormat(haveConcentration, string.Format("<color=#{0}>{1}</color>", (haveConcentration < concentrationCost) ? "brightred" : "brightblue", concentrationCost)).ColorReplace();
			short haveIntelligence = *this.Data.MainAttributes[5];
			short costIntelligence = GlobalConfig.Instance.ActiveReadingAttributeCost;
			this.readCost.text = LanguageKey.LK_ActiveRead_Tip_Cost.TrFormat(haveIntelligence, string.Format("<color=#{0}>{1}</color>", (haveIntelligence < costIntelligence) ? "brightred" : "brightblue", costIntelligence)).ColorReplace();
			TooltipInvoker tooltipInvoker = this.skillCostTipDisplayer;
			if (tooltipInvoker != null)
			{
				tooltipInvoker.Refresh(false, -1);
			}
			TooltipInvoker tooltipInvoker2 = this.readCostTipDisplayer;
			if (tooltipInvoker2 != null)
			{
				tooltipInvoker2.Refresh(false, -1);
			}
			this.skillActiveButton.interactable = this.CanActiveLoop();
			this.readActiveButton.interactable = this.CanActiveRead();
			this._lockActiveReadWhenWaitingDataUpdate = false;
			this._lockActiveLoopWhenWaitingDataUpdate = false;
			TutorialChapterModel tutorialChapterModel = SingletonObject.getInstance<TutorialChapterModel>();
			this.readButton.interactable = tutorialChapterModel.GetFunctionStatus(11);
			this.skillButton.interactable = tutorialChapterModel.GetFunctionStatus(12);
		}

		// Token: 0x06009F71 RID: 40817 RVA: 0x004A8513 File Offset: 0x004A6713
		private static float CalcActiveStageProgressFill(int progress, int maxProgress)
		{
			return (progress < maxProgress) ? ((float)(progress % 10) * 0.1f) : 1f;
		}

		// Token: 0x06009F72 RID: 40818 RVA: 0x004A852C File Offset: 0x004A672C
		public void OnActiveReadPointerDown(BaseEventData data = null)
		{
			bool flag;
			if (this.CanActiveRead())
			{
				PointerEventData pointerEventData = data as PointerEventData;
				flag = (pointerEventData != null && pointerEventData.button == PointerEventData.InputButton.Right);
			}
			else
			{
				flag = true;
			}
			bool flag2 = flag;
			if (!flag2)
			{
				this._activeReadPointerDownTime = Time.realtimeSinceStartup;
				bool lockActiveReadWhenWaitingDataUpdate = this._lockActiveReadWhenWaitingDataUpdate;
				if (!lockActiveReadWhenWaitingDataUpdate)
				{
					this._lockActiveReadWhenWaitingDataUpdate = true;
					this.ActiveReadOnce();
					this.PlayActiveReadOrLoopOnceSound(0f);
					this._isActiveReadPointerDown = true;
				}
			}
		}

		// Token: 0x06009F73 RID: 40819 RVA: 0x004A859C File Offset: 0x004A679C
		public void OnActiveLoopPointerDown(BaseEventData data = null)
		{
			bool flag;
			if (this.CanActiveLoop())
			{
				PointerEventData pointerEventData = data as PointerEventData;
				flag = (pointerEventData != null && pointerEventData.button == PointerEventData.InputButton.Right);
			}
			else
			{
				flag = true;
			}
			bool flag2 = flag;
			if (!flag2)
			{
				this._activeLoopPointerDownTime = Time.realtimeSinceStartup;
				bool lockActiveLoopWhenWaitingDataUpdate = this._lockActiveLoopWhenWaitingDataUpdate;
				if (!lockActiveLoopWhenWaitingDataUpdate)
				{
					this._lockActiveLoopWhenWaitingDataUpdate = true;
					this.ActiveLoopOnce();
					this.PlayActiveReadOrLoopOnceSound(0f);
					this._isActiveLoopPointerDown = true;
				}
			}
		}

		// Token: 0x06009F74 RID: 40820 RVA: 0x004A860C File Offset: 0x004A680C
		public void OnActiveReadPointerUp()
		{
			this._activeReadPointerUpTime = Time.realtimeSinceStartup;
			bool flag = this._isActiveReadPointerDown && this._activeReadPointerUpTime - this._activeReadPointerDownTime > 0.3f;
			if (flag)
			{
				this.PlayerActiveReadOrLoopReverseSound();
			}
			this._isActiveReadPointerDown = false;
			this.Refresh();
		}

		// Token: 0x06009F75 RID: 40821 RVA: 0x004A8660 File Offset: 0x004A6860
		public void OnActiveLoopPointerUp()
		{
			this._activeLoopPointerUpTime = Time.realtimeSinceStartup;
			bool flag = this._isActiveLoopPointerDown && this._activeLoopPointerUpTime - this._activeLoopPointerDownTime > 0.3f;
			if (flag)
			{
				this.PlayerActiveReadOrLoopReverseSound();
			}
			this._isActiveLoopPointerDown = false;
			this.Refresh();
		}

		// Token: 0x06009F76 RID: 40822 RVA: 0x004A86B4 File Offset: 0x004A68B4
		private void ActiveLoopOnce()
		{
			bool processing = this._processing;
			if (!processing)
			{
				bool flag = !this._achievement3;
				if (flag)
				{
					this._achievement3 = true;
					GlobalDomainMethod.Call.InvokeGuidingTrigger(317);
				}
				TaiwuDomainMethod.Call.ActiveNeigongLoopingOnce();
				this.RequestData();
				this.EmitActiveReadOrLoopEffectParticle(this.eff_LoopTransOnce.gameObject, ref this._loopTransOnceParticles);
			}
		}

		// Token: 0x06009F77 RID: 40823 RVA: 0x004A8714 File Offset: 0x004A6914
		private void ActiveReadOnce()
		{
			bool processing = this._processing;
			if (!processing)
			{
				bool flag = !this._achievement3;
				if (flag)
				{
					this._achievement3 = true;
					GlobalDomainMethod.Call.InvokeGuidingTrigger(317);
				}
				TaiwuDomainMethod.Call.ActiveReadOnce();
				GEvent.OnEvent(UiEvents.OnTaiwuReadingBookProgressMayChange, null);
				this.RequestData();
				this.EmitActiveReadOrLoopEffectParticle(this.eff_ReadTransOnce.gameObject, ref this._readTransOnceParticles);
			}
		}

		// Token: 0x06009F78 RID: 40824 RVA: 0x004A8788 File Offset: 0x004A6988
		private void EmitActiveReadOrLoopEffectParticle(GameObject pRoot, ref ParticleSystem[] particles)
		{
			if (particles == null)
			{
				particles = pRoot.GetComponentsInChildren<ParticleSystem>(true);
			}
			bool flag = particles == null;
			if (!flag)
			{
				foreach (ParticleSystem particle in particles)
				{
					bool flag2 = particle == null;
					if (!flag2)
					{
						particle.Emit(1);
					}
				}
			}
		}

		// Token: 0x06009F79 RID: 40825 RVA: 0x004A87E0 File Offset: 0x004A69E0
		private void HandleActiveLoopButtonHold()
		{
			float now = Time.realtimeSinceStartup;
			bool flag = !this._isActiveLoopPointerDown;
			if (flag)
			{
				this.TryStopActiveLoopHoldSound(now);
			}
			else
			{
				bool canLoop = this.CanActiveLoop();
				bool flag2 = !canLoop;
				if (flag2)
				{
					this.TryStopActiveLoopHoldSound(now);
					this.PlayerActiveReadOrLoopReverseSound();
					this._isActiveLoopPointerDown = false;
				}
				else
				{
					bool flag3 = (double)(now - this._activeLoopPointerDownTime) > 0.2;
					if (flag3)
					{
						this.TryStartActiveLoopHoldSound();
						bool flag4 = (double)(now - this._activeLoopLastTriggerTime) > 0.1;
						if (flag4)
						{
							this._activeLoopLastTriggerTime = now;
							bool lockActiveLoopWhenWaitingDataUpdate = this._lockActiveLoopWhenWaitingDataUpdate;
							if (!lockActiveLoopWhenWaitingDataUpdate)
							{
								this._lockActiveLoopWhenWaitingDataUpdate = true;
								this.ActiveLoopOnce();
								this.PlayActiveReadOrLoopOnceSound(Time.realtimeSinceStartup - this._activeLoopPointerDownTime);
							}
						}
					}
				}
			}
		}

		// Token: 0x06009F7A RID: 40826 RVA: 0x004A88AC File Offset: 0x004A6AAC
		private void HandleActiveReadButtonHold()
		{
			float now = Time.realtimeSinceStartup;
			bool flag = !this._isActiveReadPointerDown;
			if (flag)
			{
				this.TryStopActiveReadHoldSound(now);
			}
			else
			{
				bool canRead = this.CanActiveRead();
				bool flag2 = !canRead;
				if (flag2)
				{
					this.TryStopActiveReadHoldSound(now);
					this.PlayerActiveReadOrLoopReverseSound();
					this._isActiveReadPointerDown = false;
				}
				else
				{
					bool flag3 = (double)(now - this._activeReadPointerDownTime) > 0.2;
					if (flag3)
					{
						this.TryStartActiveReadHoldSound();
						bool flag4 = (double)(now - this._activeReadLastTriggerTime) > 0.1;
						if (flag4)
						{
							this._activeReadLastTriggerTime = now;
							bool lockActiveReadWhenWaitingDataUpdate = this._lockActiveReadWhenWaitingDataUpdate;
							if (!lockActiveReadWhenWaitingDataUpdate)
							{
								this._lockActiveReadWhenWaitingDataUpdate = true;
								this.ActiveReadOnce();
								this.PlayActiveReadOrLoopOnceSound(Time.realtimeSinceStartup - this._activeReadPointerDownTime);
							}
						}
					}
				}
			}
		}

		// Token: 0x06009F7B RID: 40827 RVA: 0x004A8978 File Offset: 0x004A6B78
		private unsafe bool CanActiveRead()
		{
			return this.Data.BookKey.IsValid() && !ReadAndLoop.IsInTutorial && SingletonObject.getInstance<TimeManager>().IsActionDayEnough((int)GlobalConfig.Instance.ActiveReadingTimeCost) && *this.Data.MainAttributes[5] >= GlobalConfig.Instance.ActiveReadingAttributeCost && !UIElement.PartWorld.Exist && this.Data.ActiveReadingProgress < GlobalConfig.Instance.MaxActiveReadingProgress;
		}

		// Token: 0x06009F7C RID: 40828 RVA: 0x004A89F8 File Offset: 0x004A6BF8
		private unsafe bool CanActiveLoop()
		{
			return this.Data.LoopingId != -1 && !ReadAndLoop.IsInTutorial && SingletonObject.getInstance<TimeManager>().IsActionDayEnough((int)GlobalConfig.Instance.ActiveReadingTimeCost) && *this.Data.MainAttributes[2] >= GlobalConfig.Instance.ActiveNeigongLoopingAttributeCost && !UIElement.PartWorld.Exist && this.Data.ActiveLoopingProgress < GlobalConfig.Instance.MaxActiveNeigongLoopingProgress;
		}

		// Token: 0x06009F7D RID: 40829 RVA: 0x004A8A74 File Offset: 0x004A6C74
		private float GetLoopSincePointerUpDuration()
		{
			return Time.realtimeSinceStartup - this._activeLoopPointerUpTime;
		}

		// Token: 0x06009F7E RID: 40830 RVA: 0x004A8A94 File Offset: 0x004A6C94
		private float GetReadSincePointerUpDuration()
		{
			return Time.realtimeSinceStartup - this._activeReadPointerUpTime;
		}

		// Token: 0x06009F7F RID: 40831 RVA: 0x004A8AB4 File Offset: 0x004A6CB4
		private void TryStartActiveReadOrLoopHoldSound(ref bool playingFlag, Func<float> sincePointerUpDurationGetter)
		{
			bool flag = playingFlag;
			if (!flag)
			{
				AudioCommand cmd = new AudioCommand
				{
					AudioType = SEType.Sound,
					Loop = true,
					AudioName = "SFX_reading_ui_liuguang_loop",
					OnPlayUpdate = delegate(AudioCommandOnPlayeUpdateParam param)
					{
						this.OnReadOrLoopHoldSoundUpdate(param, sincePointerUpDurationGetter);
					}
				};
				AudioManager.Instance.Play(cmd);
				playingFlag = true;
			}
		}

		// Token: 0x06009F80 RID: 40832 RVA: 0x004A8B20 File Offset: 0x004A6D20
		private void OnReadOrLoopHoldSoundUpdate(AudioCommandOnPlayeUpdateParam param, Func<float> sincePointerUpDurationGetter)
		{
			AudioSource source = param.player;
			bool flag = this._isActiveLoopPointerDown || this._isActiveReadPointerDown;
			if (flag)
			{
				float pitch = Mathf.Min(1.5f, 1f + param.eclapsedTime * 0.5f);
				float volume = Mathf.Min(1f, param.eclapsedTime);
				source.pitch = pitch;
				source.volume = volume;
			}
			else
			{
				float sincePointerUpDuration = sincePointerUpDurationGetter();
				float volume2 = Mathf.Max(0f, 1f - sincePointerUpDuration / 0.2f);
				source.volume = volume2;
			}
		}

		// Token: 0x06009F81 RID: 40833 RVA: 0x004A8BBC File Offset: 0x004A6DBC
		private void TryStopActiveReadOrLoopHoldSound(float now, float pointerUpTime, ref bool flag)
		{
			bool flag2 = now - pointerUpTime > 0.2f & flag;
			if (flag2)
			{
				AudioManager.Instance.StopSound("SFX_reading_ui_liuguang_loop");
				flag = false;
			}
		}

		// Token: 0x06009F82 RID: 40834 RVA: 0x004A8BF0 File Offset: 0x004A6DF0
		private void TryStopActiveReadHoldSound(float now)
		{
			this.TryStopActiveReadOrLoopHoldSound(now, this._activeReadPointerUpTime, ref this._isActiveReadingHoldSoundPlaying);
		}

		// Token: 0x06009F83 RID: 40835 RVA: 0x004A8C07 File Offset: 0x004A6E07
		private void TryStopActiveLoopHoldSound(float now)
		{
			this.TryStopActiveReadOrLoopHoldSound(now, this._activeLoopPointerUpTime, ref this._isActiveLoopHoldSoundPlaying);
		}

		// Token: 0x06009F84 RID: 40836 RVA: 0x004A8C1E File Offset: 0x004A6E1E
		private void TryStartActiveReadHoldSound()
		{
			this.TryStartActiveReadOrLoopHoldSound(ref this._isActiveReadingHoldSoundPlaying, new Func<float>(this.GetReadSincePointerUpDuration));
		}

		// Token: 0x06009F85 RID: 40837 RVA: 0x004A8C3A File Offset: 0x004A6E3A
		private void TryStartActiveLoopHoldSound()
		{
			this.TryStartActiveReadOrLoopHoldSound(ref this._isActiveLoopHoldSoundPlaying, new Func<float>(this.GetLoopSincePointerUpDuration));
		}

		// Token: 0x06009F86 RID: 40838 RVA: 0x004A8C58 File Offset: 0x004A6E58
		private void PlayActiveReadOrLoopOnceSound(float pointerDownTime)
		{
			float pitch = Mathf.Min(1.5f, 1f + pointerDownTime * 0.5f);
			AudioCommand cmd = new AudioCommand
			{
				AudioType = SEType.Sound,
				Loop = false,
				AudioName = "SFX_reading_ui_liuguang_click",
				Pitch = pitch
			};
			AudioManager.Instance.Play(cmd);
		}

		// Token: 0x06009F87 RID: 40839 RVA: 0x004A8CB0 File Offset: 0x004A6EB0
		private void PlayerActiveReadOrLoopReverseSound()
		{
			AudioCommand cmd = new AudioCommand
			{
				AudioType = SEType.Sound,
				Loop = false,
				AudioName = "SFX_reading_ui_liuguang_end",
				Pitch = 1f
			};
			AudioManager.Instance.Play(cmd);
		}

		// Token: 0x06009F88 RID: 40840 RVA: 0x004A8CF4 File Offset: 0x004A6EF4
		private void RefreshActiveProgressSounds()
		{
			short readingProgress = this.Data.ActiveReadingProgress;
			bool readCrossStage = this._lastReadingProgress != readingProgress && readingProgress > 0 && readingProgress % 10 == 0;
			bool flag = readCrossStage;
			if (flag)
			{
				SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(0.1f, delegate
				{
					bool flag3 = this == null || base.gameObject == null;
					if (!flag3)
					{
						AudioManager.Instance.PlaySound("SFX_reading_ui_liuguang_flash", false, false);
					}
				});
			}
			this._lastReadingProgress = readingProgress;
			short loopingProgress = this.Data.ActiveLoopingProgress;
			bool loopCrossStage = this._lastLoopingProgress != loopingProgress && loopingProgress > 0 && loopingProgress % 10 == 0;
			bool flag2 = loopCrossStage;
			if (flag2)
			{
				SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(0.1f, delegate
				{
					bool flag3 = this == null || base.gameObject == null;
					if (!flag3)
					{
						AudioManager.Instance.PlaySound("SFX_reading_ui_liuguang_flash", false, false);
					}
				});
			}
			this._lastLoopingProgress = loopingProgress;
		}

		// Token: 0x06009F89 RID: 40841 RVA: 0x004A8DA2 File Offset: 0x004A6FA2
		private void Update()
		{
			this.HandleActiveLoopButtonHold();
			this.HandleActiveReadButtonHold();
			this.UpdateReadingBookProgressTransition();
			this.UpdateSkillLoopProgressTransition();
		}

		// Token: 0x170010D4 RID: 4308
		// (get) Token: 0x06009F8A RID: 40842 RVA: 0x004A8DC1 File Offset: 0x004A6FC1
		private static bool IsInTutorial
		{
			get
			{
				return SingletonObject.getInstance<TutorialChapterModel>().InGuiding;
			}
		}

		// Token: 0x06009F8D RID: 40845 RVA: 0x004A8E19 File Offset: 0x004A7019
		[CompilerGenerated]
		private void <Awake>g__SkillButton|58_0()
		{
			TaiwuDomainMethod.AsyncCall.GetLoopingViewDisplayData(this._parent, delegate(int offset, RawDataPool dataPool)
			{
				LoopingViewDisplayData displayData = null;
				Serializer.Deserialize(dataPool, offset, ref displayData);
				UIElement.Looping.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("LoopingViewDisplayData", displayData));
				UIManager.Instance.ShowUI(UIElement.Looping, true);
			});
		}

		// Token: 0x06009F8E RID: 40846 RVA: 0x004A8E47 File Offset: 0x004A7047
		[CompilerGenerated]
		internal static void <Awake>g__ReadButton|58_1()
		{
			UIManager.Instance.ShowUI(UIElement.Reading, true);
		}

		// Token: 0x04007B3A RID: 31546
		[SerializeField]
		private Sprite[] activeLoopOrReadStatus;

		// Token: 0x04007B3B RID: 31547
		[SerializeField]
		private CImage skillStatus;

		// Token: 0x04007B3C RID: 31548
		[SerializeField]
		private CImage readStatus;

		// Token: 0x04007B3D RID: 31549
		[SerializeField]
		private CImage skillFill;

		// Token: 0x04007B3E RID: 31550
		[SerializeField]
		private CImage readFill;

		// Token: 0x04007B3F RID: 31551
		[SerializeField]
		private CImage skillIcon;

		// Token: 0x04007B40 RID: 31552
		[SerializeField]
		private ItemBack readIcon;

		// Token: 0x04007B41 RID: 31553
		[SerializeField]
		private CButton skillButton;

		// Token: 0x04007B42 RID: 31554
		[SerializeField]
		private CButton readButton;

		// Token: 0x04007B43 RID: 31555
		[SerializeField]
		private CButton skillActiveButton;

		// Token: 0x04007B44 RID: 31556
		[SerializeField]
		private CButton readActiveButton;

		// Token: 0x04007B45 RID: 31557
		[SerializeField]
		private TooltipInvoker readTipDisplayer;

		// Token: 0x04007B46 RID: 31558
		[SerializeField]
		private TooltipInvoker loopTipDisplayer;

		// Token: 0x04007B47 RID: 31559
		[SerializeField]
		private TooltipInvoker activeReadTipDisplayer;

		// Token: 0x04007B48 RID: 31560
		[SerializeField]
		private TooltipInvoker activeLoopTipDisplayer;

		// Token: 0x04007B49 RID: 31561
		[SerializeField]
		private GameObject readFinished;

		// Token: 0x04007B4A RID: 31562
		[SerializeField]
		private GameObject readEventParticle;

		// Token: 0x04007B4B RID: 31563
		[SerializeField]
		private GameObject loopFinished;

		// Token: 0x04007B4C RID: 31564
		[SerializeField]
		private GameObject loopEventParticle;

		// Token: 0x04007B4D RID: 31565
		[SerializeField]
		private TMP_Text skillCost;

		// Token: 0x04007B4E RID: 31566
		[SerializeField]
		private TMP_Text readCost;

		// Token: 0x04007B4F RID: 31567
		[SerializeField]
		private CImage[] loop4;

		// Token: 0x04007B50 RID: 31568
		[SerializeField]
		private CImage[] read5;

		// Token: 0x04007B51 RID: 31569
		[SerializeField]
		private CImage[] read6;

		// Token: 0x04007B52 RID: 31570
		[SerializeField]
		private TooltipInvoker skillCostTipDisplayer;

		// Token: 0x04007B53 RID: 31571
		[SerializeField]
		private TooltipInvoker readCostTipDisplayer;

		// Token: 0x04007B54 RID: 31572
		[SerializeField]
		private GameObject skillLoopPartition4;

		// Token: 0x04007B55 RID: 31573
		[SerializeField]
		private GameObject readingBookPartition5;

		// Token: 0x04007B56 RID: 31574
		[SerializeField]
		private GameObject readingBookPartition6;

		// Token: 0x04007B57 RID: 31575
		[SerializeField]
		private CImage skillLoopProgressFill4;

		// Token: 0x04007B58 RID: 31576
		[SerializeField]
		private CImage readingBookProgressFill5;

		// Token: 0x04007B59 RID: 31577
		[SerializeField]
		private CImage readingBookProgressFill6;

		// Token: 0x04007B5A RID: 31578
		[SerializeField]
		private GameObject eff_LoopTransOnce;

		// Token: 0x04007B5B RID: 31579
		[SerializeField]
		private GameObject eff_ReadTransOnce;

		// Token: 0x04007B5C RID: 31580
		private static readonly ItemKey[] InvalidKeys = new ItemKey[]
		{
			ItemKey.Invalid,
			ItemKey.Invalid,
			ItemKey.Invalid
		};

		// Token: 0x04007B5D RID: 31581
		internal ReadAndLoopDisplayData Data = new ReadAndLoopDisplayData();

		// Token: 0x04007B5E RID: 31582
		private IAsyncMethodRequestHandler _parent;

		// Token: 0x04007B5F RID: 31583
		private int[] _extraNeiliAllocationProgress;

		// Token: 0x04007B60 RID: 31584
		private CombatSkillDisplayData _loopingSkillDisplayData;

		// Token: 0x04007B61 RID: 31585
		private SkillBookPageDisplayData _bookPagesInfo;

		// Token: 0x04007B62 RID: 31586
		private sbyte _cachedNeiliType = -1;

		// Token: 0x04007B63 RID: 31587
		private int _auxiliaryTipDataRequestId;

		// Token: 0x04007B64 RID: 31588
		private float _currentReadingBookProgress;

		// Token: 0x04007B65 RID: 31589
		private float _targetReadingBookProgress;

		// Token: 0x04007B66 RID: 31590
		private CImage _activeReadingBookProgressFill;

		// Token: 0x04007B67 RID: 31591
		private float _currentSkillLoopProgress;

		// Token: 0x04007B68 RID: 31592
		private float _targetSkillLoopProgress;

		// Token: 0x04007B69 RID: 31593
		private bool _hasSkillLoopProgress;

		// Token: 0x04007B6A RID: 31594
		private ParticleSystem[] _loopTransOnceParticles;

		// Token: 0x04007B6B RID: 31595
		private ParticleSystem[] _readTransOnceParticles;

		// Token: 0x04007B6C RID: 31596
		private bool _processing;

		// Token: 0x04007B6D RID: 31597
		private bool _achievement1;

		// Token: 0x04007B6E RID: 31598
		private bool _achievement2;

		// Token: 0x04007B6F RID: 31599
		private bool _achievement3;

		// Token: 0x04007B70 RID: 31600
		private bool _lockActiveReadWhenWaitingDataUpdate;

		// Token: 0x04007B71 RID: 31601
		private bool _lockActiveLoopWhenWaitingDataUpdate;

		// Token: 0x04007B72 RID: 31602
		private short _lastReadingProgress;

		// Token: 0x04007B73 RID: 31603
		private short _lastLoopingProgress;

		// Token: 0x04007B74 RID: 31604
		private const float Loop4SegmentMaxFillAmount = 0.25f;

		// Token: 0x04007B75 RID: 31605
		private float _activeReadPointerDownTime;

		// Token: 0x04007B76 RID: 31606
		private float _activeLoopPointerDownTime;

		// Token: 0x04007B77 RID: 31607
		private float _activeReadPointerUpTime;

		// Token: 0x04007B78 RID: 31608
		private float _activeLoopPointerUpTime;

		// Token: 0x04007B79 RID: 31609
		private bool _isActiveReadPointerDown;

		// Token: 0x04007B7A RID: 31610
		private bool _isActiveLoopPointerDown;

		// Token: 0x04007B7B RID: 31611
		private float _activeLoopLastTriggerTime;

		// Token: 0x04007B7C RID: 31612
		private float _activeReadLastTriggerTime;

		// Token: 0x04007B7D RID: 31613
		private bool _isActiveReadingHoldSoundPlaying;

		// Token: 0x04007B7E RID: 31614
		private bool _isActiveLoopHoldSoundPlaying;

		// Token: 0x04007B7F RID: 31615
		private const string ReadOrLoopHoldSoundName = "SFX_reading_ui_liuguang_loop";

		// Token: 0x04007B80 RID: 31616
		private const string ReadOrLoopClickSoundName = "SFX_reading_ui_liuguang_click";

		// Token: 0x04007B81 RID: 31617
		private const string ReadOrLoopReversSoundName = "SFX_reading_ui_liuguang_end";

		// Token: 0x04007B82 RID: 31618
		private const string ReadOrLoopProgressSoundName = "SFX_reading_ui_liuguang_flash";

		// Token: 0x04007B83 RID: 31619
		private const string GrayBall = "bottom_point_base";

		// Token: 0x04007B84 RID: 31620
		private const string MaxNeiliBall = "bottom_point_0_0";
	}
}
