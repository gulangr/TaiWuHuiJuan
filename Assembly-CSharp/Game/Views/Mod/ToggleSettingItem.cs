using System;
using FrameWork.ModSystem;
using FrameWork.UISystem.UIElements;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.Mod
{
	// Token: 0x020008CC RID: 2252
	public class ToggleSettingItem : SettingItemBase
	{
		// Token: 0x06006B69 RID: 27497 RVA: 0x00319694 File Offset: 0x00317894
		private void Awake()
		{
			this.toggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnToggleChanged));
		}

		// Token: 0x06006B6A RID: 27498 RVA: 0x003196B4 File Offset: 0x003178B4
		public override void Initialize(SettingEntry entry)
		{
			base.Initialize(entry);
			this._typedEntry = (ToggleSetting)entry;
			this._tempValue = this._typedEntry.Value;
			this.SetWithoutNotify();
			this._isInitialized = true;
		}

		// Token: 0x06006B6B RID: 27499 RVA: 0x003196EA File Offset: 0x003178EA
		public override void SetWithoutNotify()
		{
			this.toggle.SetIsOnWithoutNotify(this._tempValue);
		}

		// Token: 0x06006B6C RID: 27500 RVA: 0x00319700 File Offset: 0x00317900
		private void OnToggleChanged(bool value)
		{
			bool flag = !this._isInitialized;
			if (!flag)
			{
				bool changed = this._tempValue != value;
				this._tempValue = value;
				base.NotifyValueChanged(changed);
			}
		}

		// Token: 0x06006B6D RID: 27501 RVA: 0x0031973C File Offset: 0x0031793C
		public override void SetInteractable(bool interactable)
		{
			bool flag = this.toggle != null;
			if (flag)
			{
				this.toggle.interactable = interactable;
			}
		}

		// Token: 0x06006B6E RID: 27502 RVA: 0x00319768 File Offset: 0x00317968
		public override void ApplyValue()
		{
			bool flag = this._typedEntry != null;
			if (flag)
			{
				this._typedEntry.Value = this._tempValue;
			}
		}

		// Token: 0x06006B6F RID: 27503 RVA: 0x00319798 File Offset: 0x00317998
		public override void ResetValue()
		{
			bool flag = this._typedEntry != null;
			if (flag)
			{
				this._tempValue = this._typedEntry.Value;
				this.SetWithoutNotify();
			}
		}

		// Token: 0x06006B70 RID: 27504 RVA: 0x003197D0 File Offset: 0x003179D0
		private void OnDestroy()
		{
			bool flag = this.toggle != null;
			if (flag)
			{
				this.toggle.onValueChanged.RemoveListener(new UnityAction<bool>(this.OnToggleChanged));
			}
		}

		// Token: 0x04004DF2 RID: 19954
		[SerializeField]
		private CToggle toggle;

		// Token: 0x04004DF3 RID: 19955
		private ToggleSetting _typedEntry;

		// Token: 0x04004DF4 RID: 19956
		private bool _isInitialized;

		// Token: 0x04004DF5 RID: 19957
		private bool _tempValue;
	}
}
