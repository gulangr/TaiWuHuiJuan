using System;
using System.Collections.Generic;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Common;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Taiwu;
using GameData.Serializer;
using GameData.Utilities;
using UnityEngine;

namespace Game.Views.Cricket.Combat
{
	// Token: 0x02000ACD RID: 2765
	public class CricketCombatPlan : MonoBehaviour, ICricketCombatComponent
	{
		// Token: 0x06008836 RID: 34870 RVA: 0x003F3698 File Offset: 0x003F1898
		private void Awake()
		{
			this.presetList.Setup(new Action(this.OnClickAddPlan), new Action(this.OnClickDuplicatePlan), new Action(this.OnClickClearPlan), new Action(this.OnClickRemovePlan), new Action<int, int>(this.OnPlanChanged));
			this.presetList.SetAmount(3);
			this.presetList.UpdateButtonsInteractable(3);
			this.UpdatePlanButtonsTip();
		}

		// Token: 0x17000F00 RID: 3840
		// (get) Token: 0x06008837 RID: 34871 RVA: 0x003F370F File Offset: 0x003F190F
		// (set) Token: 0x06008838 RID: 34872 RVA: 0x003F3717 File Offset: 0x003F1917
		public ICricketCombatHandler Handler { get; set; }

		// Token: 0x06008839 RID: 34873 RVA: 0x003F3720 File Offset: 0x003F1920
		public void OnEvent(ECricketCombatGlobalEventType type, ArgumentBox argBox)
		{
			bool flag = type == ECricketCombatGlobalEventType.RequestData;
			if (flag)
			{
				this.RequestData();
			}
			bool flag2 = type == ECricketCombatGlobalEventType.CricketDataReady;
			if (flag2)
			{
				base.gameObject.SetActive(true);
			}
			bool flag3 = CricketCombatKit.Board.Status != ECricketCombatStatus.Preparing && base.gameObject.activeSelf;
			if (flag3)
			{
				base.gameObject.SetActive(false);
			}
		}

		// Token: 0x0600883A RID: 34874 RVA: 0x003F377D File Offset: 0x003F197D
		private void RequestData()
		{
			TaiwuDomainMethod.AsyncCall.GetLastCricketPlan(this.Handler.Async, new AsyncMethodCallbackDelegate(this.HandlerGetLastCricketPlan));
			TaiwuDomainMethod.AsyncCall.GetCricketPlanCount(this.Handler.Async, new AsyncMethodCallbackDelegate(this.HandlerGetCricketPlanCount));
		}

		// Token: 0x0600883B RID: 34875 RVA: 0x003F37BA File Offset: 0x003F19BA
		private void HandlerGetCricketPlanCount(int offset, RawDataPool pool)
		{
			Serializer.Deserialize(pool, offset, ref this._planCount);
			this.presetList.SetAmount(this._planCount);
			this.presetList.UpdateButtonsInteractable(this._planCount);
			this.UpdatePlanButtonsTip();
		}

		// Token: 0x0600883C RID: 34876 RVA: 0x003F37F8 File Offset: 0x003F19F8
		private void HandlerGetLastCricketPlan(int offset, RawDataPool pool)
		{
			Serializer.Deserialize(pool, offset, ref this._cricketPlanIndex);
			this.presetList.SetCurrentPreset(this._cricketPlanIndex);
			TaiwuDomainMethod.AsyncCall.RequestValidCricketPlan(this.Handler.Async, this._cricketPlanIndex, CricketCombatKit.Board.Requires, new AsyncMethodCallbackDelegate(this.HandlerGetValidPlan));
		}

		// Token: 0x0600883D RID: 34877 RVA: 0x003F3854 File Offset: 0x003F1A54
		private void OnPlanChanged(int newIndex, int oldIndex)
		{
			bool flag = newIndex < 0 || this._cricketPlanIndex == newIndex;
			if (!flag)
			{
				CricketCombatKit.Board.ChangeAllowSelectCricket(false);
				this._cricketPlanIndex = newIndex;
				TaiwuDomainMethod.Call.SetLastCricketPlan(this._cricketPlanIndex);
				TaiwuDomainMethod.AsyncCall.RequestValidCricketPlan(this.Handler.Async, this._cricketPlanIndex, CricketCombatKit.Board.Requires, new AsyncMethodCallbackDelegate(this.HandlerGetValidPlan));
			}
		}

		// Token: 0x0600883E RID: 34878 RVA: 0x003F38C4 File Offset: 0x003F1AC4
		private void OnClickAddPlan()
		{
			TaiwuDomainMethod.Call.AddCricketPlan();
			this._planCount++;
			this._cricketPlanIndex = this._planCount - 1;
			this.presetList.SetAmount(this._planCount);
			this.presetList.UpdateButtonsInteractable(this._planCount);
			this.UpdatePlanButtonsTip();
			this.presetList.SetCurrentPreset(this._cricketPlanIndex);
			CricketCombatKit.Board.ChangeAllowSelectCricket(false);
			TaiwuDomainMethod.AsyncCall.RequestValidCricketPlan(this.Handler.Async, this._cricketPlanIndex, CricketCombatKit.Board.Requires, new AsyncMethodCallbackDelegate(this.HandlerGetValidPlan));
		}

		// Token: 0x0600883F RID: 34879 RVA: 0x003F396C File Offset: 0x003F1B6C
		private void OnClickDuplicatePlan()
		{
			TaiwuDomainMethod.Call.CloneCricketPlan();
			this._planCount++;
			this.presetList.SetAmount(this._planCount);
			this.presetList.UpdateButtonsInteractable(this._planCount);
			this.UpdatePlanButtonsTip();
			this.presetList.SetCurrentPreset(this._planCount - 1);
			this._cricketPlanIndex = this._planCount - 1;
			TaiwuDomainMethod.AsyncCall.RequestValidCricketPlan(this.Handler.Async, this._cricketPlanIndex, CricketCombatKit.Board.Requires, new AsyncMethodCallbackDelegate(this.HandlerGetValidPlan));
		}

		// Token: 0x06008840 RID: 34880 RVA: 0x003F3A08 File Offset: 0x003F1C08
		private void OnClickClearPlan()
		{
			CommonUtils.ShowConfirmDialog(LocalStringManager.Get(LanguageKey.LK_Common_Attention), LocalStringManager.Get(LanguageKey.LK_CricketCombat_PlanClear_Confirm), delegate
			{
				List<ItemKey> cricketPlanData = this._cricketPlanData;
				if (cricketPlanData != null)
				{
					cricketPlanData.Clear();
				}
				CricketCombatKit.Board.UpdateCurrentCricketPlan(this._cricketPlanIndex, this._cricketPlanData);
				CricketCombatKit.Board.SelfCrickets.Clear();
				this.Handler.OnEvent(ECricketCombatGlobalEventType.SelfCricketChanged, null);
				TaiwuDomainMethod.Call.ClearCricketPlan(this._cricketPlanIndex);
			}, null, EDialogType.None);
		}

		// Token: 0x06008841 RID: 34881 RVA: 0x003F3A34 File Offset: 0x003F1C34
		private void OnClickRemovePlan()
		{
			bool flag = this._planCount <= 1;
			if (!flag)
			{
				CommonUtils.ShowConfirmDialog(LocalStringManager.Get(LanguageKey.LK_Common_Attention), LocalStringManager.Get(LanguageKey.LK_CricketCombat_PlanDelete_Confirm), delegate
				{
					TaiwuDomainMethod.Call.DeleteCricketPlan();
					this._planCount--;
					bool flag2 = this._cricketPlanIndex >= this._planCount;
					if (flag2)
					{
						this._cricketPlanIndex = this._planCount - 1;
					}
					this.presetList.SetAmount(this._planCount);
					this.presetList.UpdateButtonsInteractable(this._planCount);
					this.UpdatePlanButtonsTip();
					this.presetList.SetCurrentPreset(this._cricketPlanIndex);
					CricketCombatKit.Board.ChangeAllowSelectCricket(false);
					TaiwuDomainMethod.AsyncCall.RequestValidCricketPlan(this.Handler.Async, this._cricketPlanIndex, CricketCombatKit.Board.Requires, new AsyncMethodCallbackDelegate(this.HandlerGetValidPlan));
				}, null, EDialogType.None);
			}
		}

		// Token: 0x06008842 RID: 34882 RVA: 0x003F3A7C File Offset: 0x003F1C7C
		private void UpdatePlanButtonsTip()
		{
			bool isFull = this._planCount >= 9;
			CricketCombatPlan.SetButtonTip(this.presetList.BtnAdd, isFull, LanguageKey.LK_CricketCombat_PlanAdd_Tip, LanguageKey.EventEditor_Error_DuplicateGroupKey);
			CricketCombatPlan.SetButtonTip(this.presetList.BtnDuplicate, isFull, LanguageKey.LK_CricketCombat_PlanCopy_Tip, LanguageKey.EventEditor_Error_DuplicateGroupKey);
			bool isLast = this._planCount <= 1;
			CricketCombatPlan.SetButtonTip(this.presetList.BtnRemove, isLast, LanguageKey.LK_CricketCombat_PlanDelete_Tip, LanguageKey.LK_CricketCombat_PlanDelete_Last);
		}

		// Token: 0x06008843 RID: 34883 RVA: 0x003F3AF4 File Offset: 0x003F1CF4
		private static void SetButtonTip(CButton button, bool useLimitKey, LanguageKey normalKey, LanguageKey limitKey = LanguageKey.EventEditor_Error_DuplicateGroupKey)
		{
			TooltipInvoker tip = button.GetComponent<TooltipInvoker>();
			bool flag = tip == null;
			if (!flag)
			{
				LanguageKey key = useLimitKey ? ((limitKey != LanguageKey.EventEditor_Error_DuplicateGroupKey) ? limitKey : LanguageKey.LK_CricketCombat_PlanLimitMax) : normalKey;
				tip.RuntimeParam = new ArgumentBox().Set("arg0", LocalStringManager.Get(key));
				tip.Refresh(false, -1);
			}
		}

		// Token: 0x06008844 RID: 34884 RVA: 0x003F3B50 File Offset: 0x003F1D50
		private void HandlerGetValidPlan(int offset, RawDataPool pool)
		{
			Serializer.Deserialize(pool, offset, ref this._cricketPlanData);
			CricketCombatKit.Board.UpdateCurrentCricketPlan(this._cricketPlanIndex, this._cricketPlanData);
			for (int i = 0; i < 3; i++)
			{
				List<ItemKey> cricketPlanData = this._cricketPlanData;
				ItemKey? key = (cricketPlanData != null) ? new ItemKey?(cricketPlanData.GetOrDefault(i, ItemKey.Invalid)) : null;
				bool flag;
				if (key != null)
				{
					ItemKey? itemKey = key;
					ItemKey invalid = ItemKey.Invalid;
					flag = (itemKey != null && (itemKey == null || itemKey.GetValueOrDefault() == invalid));
				}
				else
				{
					flag = true;
				}
				bool flag2 = flag;
				if (flag2)
				{
					CricketCombatKit.Board.SelfCrickets.SetOrAdd(i, null, null);
				}
				else
				{
					ItemDisplayData cricket = CricketCombatKit.Board.TaiwuAllowCrickets[key.Value.Id];
					CricketCombatKit.Board.SelfCrickets.SetOrAdd(i, cricket, null);
				}
			}
			CricketCombatKit.Board.ChangeAllowSelectCricket(true);
			this.Handler.OnEvent(ECricketCombatGlobalEventType.SelfCricketChanged, null);
		}

		// Token: 0x04006863 RID: 26723
		[SerializeField]
		private PresetListWithButtons presetList;

		// Token: 0x04006864 RID: 26724
		private const int DefaultPlanCount = 3;

		// Token: 0x04006865 RID: 26725
		private int _cricketPlanIndex;

		// Token: 0x04006866 RID: 26726
		private int _planCount = 3;

		// Token: 0x04006867 RID: 26727
		private List<ItemKey> _cricketPlanData;
	}
}
