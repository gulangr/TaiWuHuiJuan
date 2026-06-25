using System;
using System.Diagnostics;
using FrameWork.ModSystem;
using TMPro;
using UnityEngine;

namespace Game.Views.Mod
{
	// Token: 0x020008CA RID: 2250
	public abstract class SettingItemBase : MonoBehaviour
	{
		// Token: 0x17000CA6 RID: 3238
		// (get) Token: 0x06006B54 RID: 27476 RVA: 0x003192FC File Offset: 0x003174FC
		public SettingEntry SettingEntry
		{
			get
			{
				return this._settingEntry;
			}
		}

		// Token: 0x14000079 RID: 121
		// (add) Token: 0x06006B55 RID: 27477 RVA: 0x00319304 File Offset: 0x00317504
		// (remove) Token: 0x06006B56 RID: 27478 RVA: 0x0031933C File Offset: 0x0031753C
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<bool> OnValueChanged;

		// Token: 0x06006B57 RID: 27479 RVA: 0x00319374 File Offset: 0x00317574
		public virtual void Initialize(SettingEntry entry)
		{
			this._settingEntry = entry;
			bool flag = this.titleLabel != null;
			if (flag)
			{
				this.titleLabel.text = entry.DisplayName;
			}
			bool flag2 = this.descLabel != null;
			if (flag2)
			{
				this.descLabel.text = entry.Description;
			}
		}

		// Token: 0x06006B58 RID: 27480
		public abstract void SetInteractable(bool interactable);

		// Token: 0x06006B59 RID: 27481
		public abstract void SetWithoutNotify();

		// Token: 0x06006B5A RID: 27482
		public abstract void ApplyValue();

		// Token: 0x06006B5B RID: 27483
		public abstract void ResetValue();

		// Token: 0x06006B5C RID: 27484 RVA: 0x003193CD File Offset: 0x003175CD
		protected void NotifyValueChanged(bool changed)
		{
			Action<bool> onValueChanged = this.OnValueChanged;
			if (onValueChanged != null)
			{
				onValueChanged(changed);
			}
		}

		// Token: 0x04004DE8 RID: 19944
		[SerializeField]
		private TextMeshProUGUI titleLabel;

		// Token: 0x04004DE9 RID: 19945
		[SerializeField]
		private TextMeshProUGUI descLabel;

		// Token: 0x04004DEA RID: 19946
		private SettingEntry _settingEntry;
	}
}
