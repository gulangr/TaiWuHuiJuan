using System;
using System.Diagnostics;
using TMPro;
using UnityEngine;

namespace Game.Views.SystemSetting
{
	// Token: 0x02000765 RID: 1893
	public abstract class SettingItemBase : MonoBehaviour
	{
		// Token: 0x17000AC4 RID: 2756
		// (get) Token: 0x06005B95 RID: 23445 RVA: 0x002A8637 File Offset: 0x002A6837
		// (set) Token: 0x06005B96 RID: 23446 RVA: 0x002A863F File Offset: 0x002A683F
		private protected ISettingItemInfo Info
		{
			protected get
			{
				return this.info;
			}
			private set
			{
				this.info = value;
			}
		}

		// Token: 0x14000072 RID: 114
		// (add) Token: 0x06005B97 RID: 23447 RVA: 0x002A8648 File Offset: 0x002A6848
		// (remove) Token: 0x06005B98 RID: 23448 RVA: 0x002A8680 File Offset: 0x002A6880
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action OnChanged;

		// Token: 0x14000073 RID: 115
		// (add) Token: 0x06005B99 RID: 23449 RVA: 0x002A86B8 File Offset: 0x002A68B8
		// (remove) Token: 0x06005B9A RID: 23450 RVA: 0x002A86F0 File Offset: 0x002A68F0
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<bool> OnInteractableChanged;

		// Token: 0x06005B9B RID: 23451 RVA: 0x002A8728 File Offset: 0x002A6928
		public virtual void Initialize(ISettingItemInfo info)
		{
			this.Info = info;
			this._isInitialized = true;
			bool flag = this.labelText != null;
			if (flag)
			{
				this.labelText.text = info.Attribute.LanguageKey.Tr();
			}
		}

		// Token: 0x06005B9C RID: 23452
		public abstract object GetValue();

		// Token: 0x06005B9D RID: 23453
		public abstract void SetValue(object value);

		// Token: 0x06005B9E RID: 23454
		public abstract void SetInteractable(bool interactable);

		// Token: 0x06005B9F RID: 23455 RVA: 0x002A8771 File Offset: 0x002A6971
		protected void NotifyChanged()
		{
			Action onChanged = this.OnChanged;
			if (onChanged != null)
			{
				onChanged();
			}
		}

		// Token: 0x06005BA0 RID: 23456 RVA: 0x002A8786 File Offset: 0x002A6986
		protected void NotifyInteractableChanged(bool interactable)
		{
			Action<bool> onInteractableChanged = this.OnInteractableChanged;
			if (onInteractableChanged != null)
			{
				onInteractableChanged(interactable);
			}
		}

		// Token: 0x04003F2F RID: 16175
		[SerializeField]
		protected TextMeshProUGUI labelText;

		// Token: 0x04003F30 RID: 16176
		private ISettingItemInfo info;

		// Token: 0x04003F31 RID: 16177
		protected bool _isInitialized;
	}
}
