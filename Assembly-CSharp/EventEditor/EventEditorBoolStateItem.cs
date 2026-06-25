using System;
using System.Collections.Generic;
using Config;
using FrameWork.UISystem.UIElements;
using GameData.Domains.TaiwuEvent.EventOption;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace EventEditor
{
	// Token: 0x0200063D RID: 1597
	public class EventEditorBoolStateItem : MonoBehaviour
	{
		// Token: 0x06004B72 RID: 19314 RVA: 0x00237730 File Offset: 0x00235930
		public void LoadData(EventBoolStateInfo boolState, EventBoolStateItem config)
		{
			this._config = config;
			this.eventBoolState.SetIsOnWithoutNotify(boolState.BoolState);
			this.eventBoolState.onValueChanged.RemoveAllListeners();
			this.eventBoolState.onValueChanged.AddListener(new UnityAction<bool>(this.OnEditBoolState));
			this.removeBeforeNextEvent.SetIsOnWithoutNotify(boolState.RemoveBeforeNextEvent);
			this.removeBeforeNextEvent.onValueChanged.RemoveAllListeners();
			this.removeBeforeNextEvent.onValueChanged.AddListener(new UnityAction<bool>(this.OnEditRemoveBeforeNextEvent));
			this.nameLabel.SetText(EventBoolState.Instance[boolState.EventBoolStateTemplateId].Name, true);
			this._data = boolState;
		}

		// Token: 0x06004B73 RID: 19315 RVA: 0x002377EE File Offset: 0x002359EE
		private void OnEditBoolState(bool boolState)
		{
			this._data.BoolState = boolState;
			EventBoolStateEditor.Instance.UpdateData(this._config, this._data);
			EventBoolStateEditor.Instance.SetDirty();
		}

		// Token: 0x06004B74 RID: 19316 RVA: 0x0023781F File Offset: 0x00235A1F
		private void OnEditRemoveBeforeNextEvent(bool remove)
		{
			this._data.RemoveBeforeNextEvent = remove;
			EventBoolStateEditor.Instance.UpdateData(this._config, this._data);
			EventBoolStateEditor.Instance.SetDirty();
		}

		// Token: 0x0400346B RID: 13419
		public TextMeshProUGUI nameLabel;

		// Token: 0x0400346C RID: 13420
		public CToggle eventBoolState;

		// Token: 0x0400346D RID: 13421
		public CToggle removeBeforeNextEvent;

		// Token: 0x0400346E RID: 13422
		private EventBoolStateInfo _data;

		// Token: 0x0400346F RID: 13423
		private EventBoolStateItem _config;

		// Token: 0x04003470 RID: 13424
		private static readonly List<string> RefNameList = new List<string>();
	}
}
