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
	// Token: 0x0200063F RID: 1599
	public class EventEditorConsumeInfo : MonoBehaviour
	{
		// Token: 0x06004B81 RID: 19329 RVA: 0x002380C8 File Offset: 0x002362C8
		public void LoadData(EventOptionCost consumeInfo)
		{
			this.InitConsumeTypes();
			this.consumeType.SetValueWithoutNotify((int)consumeInfo.ConsumeType);
			this.consumeType.onValueChanged.RemoveAllListeners();
			this.consumeType.onValueChanged.AddListener(new UnityAction<int>(this.OnEditConsumeType));
			this.consumeAmount.SetTextWithoutNotify(consumeInfo.Expression ?? consumeInfo.CostAmount.ToString());
			this.consumeAmount.onEndEdit.RemoveAllListeners();
			this.consumeAmount.onEndEdit.AddListener(new UnityAction<string>(this.OnEditConsumeAmount));
			this.autoConsume.SetIsOnWithoutNotify(consumeInfo.AutoConsume);
			this.autoConsume.onValueChanged.RemoveAllListeners();
			this.autoConsume.onValueChanged.AddListener(new UnityAction<bool>(this.OnEditAutoConsume));
			this.isExpression.SetIsOnWithoutNotify(consumeInfo.Expression != null);
			this.deleteBtn.ClearAndAddListener(new Action(this.Delete));
			this._data = consumeInfo;
		}

		// Token: 0x06004B82 RID: 19330 RVA: 0x002381E0 File Offset: 0x002363E0
		private void InitConsumeTypes()
		{
			bool flag = EventEditorConsumeInfo.RefNameList.Count == 0;
			if (flag)
			{
				IReadOnlyDictionary<string, int> refNameMap = EventOptionConsumeType.Instance.RefNameMap;
				EventEditorConsumeInfo.RefNameList.AddRange(refNameMap.Keys);
				EventEditorConsumeInfo.RefNameList.Sort((string a, string b) => refNameMap[a].CompareTo(refNameMap[b]));
				EventEditorConsumeInfo.RefNameList.RemoveAt(0);
			}
			this.consumeType.ClearOptions();
			this.consumeType.AddOptions(EventEditorConsumeInfo.RefNameList);
		}

		// Token: 0x06004B83 RID: 19331 RVA: 0x00238270 File Offset: 0x00236470
		private void OnEditConsumeAmount(string val)
		{
			bool isOn = this.isExpression.isOn;
			if (isOn)
			{
				this._data.Expression = val;
				this._data.CostAmount = 0;
			}
			else
			{
				int amount;
				this._data.CostAmount = (int.TryParse(val, out amount) ? amount : 0);
			}
			OptionConsumeEditor.Instance.SetDirty();
		}

		// Token: 0x06004B84 RID: 19332 RVA: 0x002382CD File Offset: 0x002364CD
		private void OnEditAutoConsume(bool autoConsume)
		{
			this._data.AutoConsume = autoConsume;
			OptionConsumeEditor.Instance.SetDirty();
		}

		// Token: 0x06004B85 RID: 19333 RVA: 0x002382E7 File Offset: 0x002364E7
		private void OnEditConsumeType(int consumeType)
		{
			this._data.ConsumeType = (sbyte)consumeType;
			OptionConsumeEditor.Instance.SetDirty();
		}

		// Token: 0x06004B86 RID: 19334 RVA: 0x00238304 File Offset: 0x00236504
		private void Delete()
		{
			int index = base.transform.GetSiblingIndex();
			OptionConsumeEditor.Instance.RemoveIndex(index);
		}

		// Token: 0x04003475 RID: 13429
		public CDropdown consumeType;

		// Token: 0x04003476 RID: 13430
		public TMP_InputField consumeAmount;

		// Token: 0x04003477 RID: 13431
		public CToggle isExpression;

		// Token: 0x04003478 RID: 13432
		public CToggle autoConsume;

		// Token: 0x04003479 RID: 13433
		public CButton deleteBtn;

		// Token: 0x0400347A RID: 13434
		private EventOptionCost _data;

		// Token: 0x0400347B RID: 13435
		private static readonly List<string> RefNameList = new List<string>();
	}
}
