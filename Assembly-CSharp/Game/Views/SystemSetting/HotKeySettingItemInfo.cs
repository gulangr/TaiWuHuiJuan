using System;

namespace Game.Views.SystemSetting
{
	// Token: 0x0200076C RID: 1900
	public class HotKeySettingItemInfo : SettingItemInfo<HotKeyCommand>
	{
		// Token: 0x17000AC5 RID: 2757
		// (get) Token: 0x06005BED RID: 23533 RVA: 0x002AAAB5 File Offset: 0x002A8CB5
		public HotKeyCommand Command
		{
			get
			{
				return this._command;
			}
		}

		// Token: 0x17000AC6 RID: 2758
		// (get) Token: 0x06005BEE RID: 23534 RVA: 0x002AAABD File Offset: 0x002A8CBD
		public byte KitId
		{
			get
			{
				return this._kitId;
			}
		}

		// Token: 0x17000AC7 RID: 2759
		// (get) Token: 0x06005BEF RID: 23535 RVA: 0x002AAAC5 File Offset: 0x002A8CC5
		public ESettingSubCategory SubCategory
		{
			get
			{
				return this._subCategory;
			}
		}

		// Token: 0x06005BF0 RID: 23536 RVA: 0x002AAAD0 File Offset: 0x002A8CD0
		public HotKeySettingItemInfo(HotKeyCommand command, byte kitId, ESettingSubCategory subCategory, int order)
		{
			HotKeySettingAttribute hotKeySettingAttribute = new HotKeySettingAttribute(subCategory, order, command.DescLanguageId);
			hotKeySettingAttribute.HotKeyCommand = command;
			base..ctor(hotKeySettingAttribute, () => command, delegate(HotKeyCommand value)
			{
			});
			this._command = command;
			this._kitId = kitId;
			this._subCategory = subCategory;
		}

		// Token: 0x04003F72 RID: 16242
		private readonly HotKeyCommand _command;

		// Token: 0x04003F73 RID: 16243
		private readonly byte _kitId;

		// Token: 0x04003F74 RID: 16244
		private readonly ESettingSubCategory _subCategory;
	}
}
