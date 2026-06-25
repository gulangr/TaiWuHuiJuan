using System;
using FrameWork.ModSystem;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.Mod
{
	// Token: 0x020008C9 RID: 2249
	public class InputFieldSettingItem : SettingItemBase
	{
		// Token: 0x06006B4B RID: 27467 RVA: 0x0031917C File Offset: 0x0031737C
		private void Awake()
		{
			this.inputField.onEndEdit.AddListener(new UnityAction<string>(this.OnEndEdit));
		}

		// Token: 0x06006B4C RID: 27468 RVA: 0x0031919C File Offset: 0x0031739C
		public override void Initialize(SettingEntry entry)
		{
			base.Initialize(entry);
			this._typedEntry = (InputFieldSetting)entry;
			this._tempValue = this._typedEntry.Value;
			this.SetWithoutNotify();
			this._isInitialized = true;
		}

		// Token: 0x06006B4D RID: 27469 RVA: 0x003191D2 File Offset: 0x003173D2
		public override void SetWithoutNotify()
		{
			this.inputField.SetTextWithoutNotify(this._tempValue);
		}

		// Token: 0x06006B4E RID: 27470 RVA: 0x003191E8 File Offset: 0x003173E8
		private void OnEndEdit(string value)
		{
			bool flag = !this._isInitialized;
			if (!flag)
			{
				bool changed = this._tempValue != value;
				this._tempValue = value;
				base.NotifyValueChanged(changed);
			}
		}

		// Token: 0x06006B4F RID: 27471 RVA: 0x00319224 File Offset: 0x00317424
		public override void SetInteractable(bool interactable)
		{
			bool flag = this.inputField != null;
			if (flag)
			{
				this.inputField.interactable = interactable;
			}
		}

		// Token: 0x06006B50 RID: 27472 RVA: 0x00319250 File Offset: 0x00317450
		public override void ApplyValue()
		{
			bool flag = this._typedEntry != null;
			if (flag)
			{
				this._typedEntry.Value = this._tempValue;
			}
		}

		// Token: 0x06006B51 RID: 27473 RVA: 0x00319280 File Offset: 0x00317480
		public override void ResetValue()
		{
			bool flag = this._typedEntry != null;
			if (flag)
			{
				this._tempValue = this._typedEntry.Value;
				this.SetWithoutNotify();
			}
		}

		// Token: 0x06006B52 RID: 27474 RVA: 0x003192B8 File Offset: 0x003174B8
		private void OnDestroy()
		{
			bool flag = this.inputField != null;
			if (flag)
			{
				this.inputField.onEndEdit.RemoveListener(new UnityAction<string>(this.OnEndEdit));
			}
		}

		// Token: 0x04004DE4 RID: 19940
		[SerializeField]
		private TMP_InputField inputField;

		// Token: 0x04004DE5 RID: 19941
		private InputFieldSetting _typedEntry;

		// Token: 0x04004DE6 RID: 19942
		private bool _isInitialized;

		// Token: 0x04004DE7 RID: 19943
		private string _tempValue;
	}
}
