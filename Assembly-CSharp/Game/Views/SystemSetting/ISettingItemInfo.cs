using System;

namespace Game.Views.SystemSetting
{
	// Token: 0x0200077F RID: 1919
	public interface ISettingItemInfo
	{
		// Token: 0x17000AF3 RID: 2803
		// (get) Token: 0x06005C52 RID: 23634
		SettingItemBaseAttribute Attribute { get; }

		// Token: 0x17000AF4 RID: 2804
		// (get) Token: 0x06005C53 RID: 23635
		Type PropertyType { get; }

		// Token: 0x06005C54 RID: 23636
		object GetValueBoxed();

		// Token: 0x06005C55 RID: 23637
		void SetValueBoxed(object value);

		// Token: 0x06005C56 RID: 23638
		void RefreshValueTo(SettingItemBase target);

		// Token: 0x06005C57 RID: 23639
		bool GetValueAsBool();
	}
}
