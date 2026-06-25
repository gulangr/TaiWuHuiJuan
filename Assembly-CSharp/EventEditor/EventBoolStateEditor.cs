using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.Domains.TaiwuEvent.EventOption;
using UnityEngine;

namespace EventEditor
{
	// Token: 0x0200063B RID: 1595
	public class EventBoolStateEditor : EventEditorSubPageBase
	{
		// Token: 0x06004B59 RID: 19289 RVA: 0x00236D97 File Offset: 0x00234F97
		public static void Init(EventBoolStateEditor instance)
		{
			EventBoolStateEditor.Instance = instance;
			EventBoolStateEditor.Instance.InternalInit();
			EventBoolStateEditor.Instance.Hide();
		}

		// Token: 0x06004B5A RID: 19290 RVA: 0x00236DB6 File Offset: 0x00234FB6
		protected override void InternalInit()
		{
			this.OperateStack = new OperateStack(32);
			this.btnConfirm.ClearAndAddListener(new Action(this.OnConfirm));
			this.btnCancel.ClearAndAddListener(new Action(this.OnCancel));
		}

		// Token: 0x06004B5B RID: 19291 RVA: 0x00236DF8 File Offset: 0x00234FF8
		public void Show(Dictionary<short, EventBoolStateInfo> eventBoolStateList, Action<Dictionary<short, EventBoolStateInfo>> onConfirm)
		{
			this._isDirty = false;
			this._onConfirm = onConfirm;
			this._boolStates = (eventBoolStateList ?? new Dictionary<short, EventBoolStateInfo>());
			int needCount = EventBoolState.Instance.Count - this.contentRoot.childCount;
			for (int i = 0; i < needCount; i++)
			{
				GameObject go = Object.Instantiate<GameObject>(this.goBoolStateItemTemp, this.contentRoot);
				go.SetActive(true);
			}
			this.goBoolStateItemTemp.SetActive(false);
			short j = 0;
			while ((int)j < EventBoolState.Instance.Count)
			{
				EventBoolStateItem config = EventBoolState.Instance[j];
				EventBoolStateInfo eventBoolStateInfo;
				if ((eventBoolStateInfo = this._boolStates.GetValueOrDefault(j)) == null)
				{
					EventBoolStateInfo eventBoolStateInfo2 = new EventBoolStateInfo();
					eventBoolStateInfo2.EventBoolStateTemplateId = config.TemplateId;
					eventBoolStateInfo2.BoolState = false;
					eventBoolStateInfo = eventBoolStateInfo2;
					eventBoolStateInfo2.RemoveBeforeNextEvent = config.RemoveBeforeNextEvent;
				}
				EventBoolStateInfo stateInfo = eventBoolStateInfo;
				EventEditorBoolStateItem entry = this.contentRoot.GetChild((int)j).GetComponent<EventEditorBoolStateItem>();
				entry.LoadData(stateInfo, config);
				j += 1;
			}
			this.OperateStack.Clear();
			base.gameObject.SetActive(true);
			this.btnConfirm.interactable = false;
		}

		// Token: 0x06004B5C RID: 19292 RVA: 0x00236F26 File Offset: 0x00235126
		public override void Show()
		{
			this.Show(null, null);
		}

		// Token: 0x06004B5D RID: 19293 RVA: 0x00236F32 File Offset: 0x00235132
		public override void Hide()
		{
			base.gameObject.SetActive(false);
		}

		// Token: 0x06004B5E RID: 19294 RVA: 0x00236F42 File Offset: 0x00235142
		public void SetDirty()
		{
			this._isDirty = true;
			this.btnConfirm.interactable = true;
		}

		// Token: 0x06004B5F RID: 19295 RVA: 0x00236F59 File Offset: 0x00235159
		public void OnConfirm()
		{
			this.RemoveDefaultData();
			this._isDirty = false;
			this.btnConfirm.interactable = false;
			Action<Dictionary<short, EventBoolStateInfo>> onConfirm = this._onConfirm;
			if (onConfirm != null)
			{
				onConfirm(this._boolStates);
			}
		}

		// Token: 0x06004B60 RID: 19296 RVA: 0x00236F90 File Offset: 0x00235190
		public void OnCancel()
		{
			bool isDirty = this._isDirty;
			if (isDirty)
			{
				DialogCmd cmd = new DialogCmd
				{
					Title = LocalStringManager.Get(LanguageKey.UI_InstructionEditor_Confirm_Edit),
					Content = LocalStringManager.Get(LanguageKey.UI_InstructionEditor_Confirm_Edit_Desc),
					Type = 3,
					Yes = new Action(this.OnConfirm),
					No = new Action(this.Hide),
					GroupYesText = LocalStringManager.Get(LanguageKey.LK_Yes),
					GroupNoText = LocalStringManager.Get(LanguageKey.LK_No)
				};
				UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
				UIManager.Instance.MaskUI(UIElement.Dialog);
			}
			else
			{
				this.Hide();
			}
		}

		// Token: 0x06004B61 RID: 19297 RVA: 0x00237054 File Offset: 0x00235254
		private void RemoveDefaultData()
		{
			List<KeyValuePair<short, EventBoolStateInfo>> boolStateList = this._boolStates.ToList<KeyValuePair<short, EventBoolStateInfo>>();
			foreach (KeyValuePair<short, EventBoolStateInfo> pair in boolStateList)
			{
				EventBoolStateItem config = EventBoolState.Instance[pair.Key];
				bool flag = !pair.Value.BoolState && pair.Value.RemoveBeforeNextEvent == config.RemoveBeforeNextEvent;
				if (flag)
				{
					this._boolStates.Remove(pair.Key);
				}
			}
		}

		// Token: 0x06004B62 RID: 19298 RVA: 0x00237100 File Offset: 0x00235300
		public void UpdateData(EventBoolStateItem config, EventBoolStateInfo stateInfo)
		{
			bool flag = stateInfo.BoolState || stateInfo.RemoveBeforeNextEvent != config.RemoveBeforeNextEvent;
			if (flag)
			{
				this._boolStates.TryAdd(config.TemplateId, stateInfo);
			}
			else
			{
				this._boolStates.Remove(stateInfo.EventBoolStateTemplateId);
			}
		}

		// Token: 0x04003458 RID: 13400
		public static EventBoolStateEditor Instance;

		// Token: 0x04003459 RID: 13401
		private Dictionary<short, EventBoolStateInfo> _boolStates;

		// Token: 0x0400345A RID: 13402
		private Action<Dictionary<short, EventBoolStateInfo>> _onConfirm;

		// Token: 0x0400345B RID: 13403
		private bool _isDirty;

		// Token: 0x0400345C RID: 13404
		[SerializeField]
		private CButton btnConfirm;

		// Token: 0x0400345D RID: 13405
		[SerializeField]
		private CButton btnCancel;

		// Token: 0x0400345E RID: 13406
		[SerializeField]
		private RectTransform contentRoot;

		// Token: 0x0400345F RID: 13407
		[SerializeField]
		private GameObject goBoolStateItemTemp;
	}
}
