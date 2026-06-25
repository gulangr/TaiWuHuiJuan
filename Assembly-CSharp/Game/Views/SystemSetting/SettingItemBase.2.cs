using System;
using System.Diagnostics;

namespace Game.Views.SystemSetting
{
	// Token: 0x02000766 RID: 1894
	public abstract class SettingItemBase<T> : SettingItemBase
	{
		// Token: 0x06005BA2 RID: 23458 RVA: 0x002A87A5 File Offset: 0x002A69A5
		public override void Initialize(ISettingItemInfo info)
		{
			base.Initialize(info);
			this._typedInfo = (SettingItemInfo<T>)info;
		}

		// Token: 0x06005BA3 RID: 23459 RVA: 0x002A87BC File Offset: 0x002A69BC
		public T GetTypedValue()
		{
			return this._typedInfo.GetValue();
		}

		// Token: 0x06005BA4 RID: 23460
		public abstract void SetTypedValue(T value);

		// Token: 0x14000074 RID: 116
		// (add) Token: 0x06005BA5 RID: 23461 RVA: 0x002A87CC File Offset: 0x002A69CC
		// (remove) Token: 0x06005BA6 RID: 23462 RVA: 0x002A8804 File Offset: 0x002A6A04
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<T> OnTypedValueChanged;

		// Token: 0x06005BA7 RID: 23463 RVA: 0x002A8839 File Offset: 0x002A6A39
		protected void InvokeTypedValueChanged(T value)
		{
			this._typedInfo.SetValue(value);
			Action<T> onTypedValueChanged = this.OnTypedValueChanged;
			if (onTypedValueChanged != null)
			{
				onTypedValueChanged(value);
			}
			base.NotifyChanged();
		}

		// Token: 0x04003F34 RID: 16180
		protected SettingItemInfo<T> _typedInfo;
	}
}
