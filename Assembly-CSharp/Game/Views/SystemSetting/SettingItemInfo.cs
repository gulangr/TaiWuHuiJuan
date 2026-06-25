using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Game.Views.SystemSetting
{
	// Token: 0x02000780 RID: 1920
	public class SettingItemInfo<T> : ISettingItemInfo
	{
		// Token: 0x17000AF5 RID: 2805
		// (get) Token: 0x06005C58 RID: 23640 RVA: 0x002AB747 File Offset: 0x002A9947
		public SettingItemBaseAttribute Attribute { get; }

		// Token: 0x17000AF6 RID: 2806
		// (get) Token: 0x06005C59 RID: 23641 RVA: 0x002AB74F File Offset: 0x002A994F
		public Type PropertyType
		{
			get
			{
				return typeof(T);
			}
		}

		// Token: 0x06005C5A RID: 23642 RVA: 0x002AB75B File Offset: 0x002A995B
		public SettingItemInfo(SettingItemBaseAttribute attr, Func<T> getter, Action<T> setter)
		{
			this.Attribute = attr;
			this._getter = getter;
			this._setter = setter;
		}

		// Token: 0x06005C5B RID: 23643 RVA: 0x002AB77A File Offset: 0x002A997A
		public T GetValue()
		{
			return this._getter();
		}

		// Token: 0x06005C5C RID: 23644 RVA: 0x002AB787 File Offset: 0x002A9987
		public void SetValue(T value)
		{
			this._setter(value);
		}

		// Token: 0x06005C5D RID: 23645 RVA: 0x002AB796 File Offset: 0x002A9996
		object ISettingItemInfo.GetValueBoxed()
		{
			return this._getter();
		}

		// Token: 0x06005C5E RID: 23646 RVA: 0x002AB7A8 File Offset: 0x002A99A8
		void ISettingItemInfo.SetValueBoxed(object value)
		{
			this._setter((T)((object)value));
		}

		// Token: 0x06005C5F RID: 23647 RVA: 0x002AB7BC File Offset: 0x002A99BC
		void ISettingItemInfo.RefreshValueTo(SettingItemBase target)
		{
			T value = this._getter();
			SettingItemBase<T> typed = target as SettingItemBase<T>;
			bool flag = typed != null;
			if (flag)
			{
				typed.SetTypedValue(value);
			}
			else
			{
				target.SetValue(value);
			}
		}

		// Token: 0x06005C60 RID: 23648 RVA: 0x002AB7FC File Offset: 0x002A99FC
		unsafe bool ISettingItemInfo.GetValueAsBool()
		{
			T value = this._getter();
			bool flag = typeof(T) == typeof(bool);
			bool result;
			if (flag)
			{
				result = *Unsafe.As<T, bool>(ref value);
			}
			else
			{
				bool flag2 = typeof(T) == typeof(int);
				if (flag2)
				{
					result = (*Unsafe.As<T, int>(ref value) != 0);
				}
				else
				{
					bool flag3 = typeof(T) == typeof(sbyte);
					if (flag3)
					{
						result = (*Unsafe.As<T, sbyte>(ref value) != 0);
					}
					else
					{
						bool flag4 = typeof(T) == typeof(float);
						if (flag4)
						{
							result = (*Unsafe.As<T, float>(ref value) != 0f);
						}
						else
						{
							Debug.LogWarning(string.Format("类型 {0} 不支持", typeof(T)));
							result = true;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x04003FDB RID: 16347
		private readonly Func<T> _getter;

		// Token: 0x04003FDC RID: 16348
		private readonly Action<T> _setter;
	}
}
