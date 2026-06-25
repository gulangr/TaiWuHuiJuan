using System;
using Config;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x020004A9 RID: 1193
	public class AccessoryMainPropMenu : AccessoryCommonPropTypeMenu
	{
		// Token: 0x17000716 RID: 1814
		// (get) Token: 0x06004188 RID: 16776 RVA: 0x00201D74 File Offset: 0x001FFF74
		public override int Id
		{
			get
			{
				return 7;
			}
		}

		// Token: 0x06004189 RID: 16777 RVA: 0x00201D78 File Offset: 0x001FFF78
		public sealed override void OnInit()
		{
			this._optionNames.Clear();
			this._optionNames.Add("LK_Main_Attribute_Strength");
			this._optionNames.Add("LK_Main_Attribute_Intelligence");
			this._optionNames.Add("LK_Main_Attribute_Dexterity");
			this._optionNames.Add("LK_Main_Attribute_Concentration");
			this._optionNames.Add("LK_Main_Attribute_Vitality");
			this._optionNames.Add("LK_Main_Attribute_Energy");
		}

		// Token: 0x0600418A RID: 16778 RVA: 0x00201DF8 File Offset: 0x001FFFF8
		protected override bool CheckOptionValid(int index, AccessoryItem accessoryConfig)
		{
			return AccessoryMainPropMenu.CheckOptionValidStatic(index, accessoryConfig);
		}

		// Token: 0x0600418B RID: 16779 RVA: 0x00201E14 File Offset: 0x00200014
		public static bool CheckOptionValidStatic(int index, AccessoryItem accessoryConfig)
		{
			bool result;
			switch (index)
			{
			case 0:
				result = (accessoryConfig.Strength > 0);
				break;
			case 1:
				result = (accessoryConfig.Intelligence > 0);
				break;
			case 2:
				result = (accessoryConfig.Dexterity > 0);
				break;
			case 3:
				result = (accessoryConfig.Concentration > 0);
				break;
			case 4:
				result = (accessoryConfig.Vitality > 0);
				break;
			case 5:
				result = (accessoryConfig.Energy > 0);
				break;
			default:
				result = true;
				break;
			}
			return result;
		}

		// Token: 0x0600418C RID: 16780 RVA: 0x00201E94 File Offset: 0x00200094
		public static bool CheckAnyOptionValidStatic(AccessoryItem accessoryConfig)
		{
			for (int i = 0; i < Enum.GetValues(typeof(EAccessoryMainPropType)).Length; i++)
			{
				bool flag = AccessoryMainPropMenu.CheckOptionValidStatic(i, accessoryConfig);
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x17000717 RID: 1815
		// (get) Token: 0x0600418D RID: 16781 RVA: 0x00201EDB File Offset: 0x002000DB
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(0, 0));
			}
		}
	}
}
