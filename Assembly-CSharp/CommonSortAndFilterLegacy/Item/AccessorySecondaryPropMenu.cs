using System;
using Config;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x020004AA RID: 1194
	public class AccessorySecondaryPropMenu : AccessoryCommonPropTypeMenu
	{
		// Token: 0x17000718 RID: 1816
		// (get) Token: 0x0600418F RID: 16783 RVA: 0x00201EF2 File Offset: 0x002000F2
		public override int Id
		{
			get
			{
				return 8;
			}
		}

		// Token: 0x06004190 RID: 16784 RVA: 0x00201EF8 File Offset: 0x002000F8
		public sealed override void OnInit()
		{
			this._optionNames.Clear();
			this._optionNames.Add("LK_SpeedOf_Stance");
			this._optionNames.Add("LK_SpeedOf_Breath");
			this._optionNames.Add("LK_AttackSpeed");
			this._optionNames.Add("LK_RecoveryOfMobility");
			this._optionNames.Add("LK_CastSpeed");
			this._optionNames.Add("LK_WeaponSwitchSpeed");
			this._optionNames.Add("LK_InnerRatio");
			this._optionNames.Add("LK_RecoveryOfQiDisorder");
			this._optionNames.Add("LK_RecoveryOfFlaw");
			this._optionNames.Add("LK_RecoveryOfBlockedAcupoint");
		}

		// Token: 0x06004191 RID: 16785 RVA: 0x00201FBC File Offset: 0x002001BC
		protected override bool CheckOptionValid(int index, AccessoryItem accessoryConfig)
		{
			return AccessorySecondaryPropMenu.CheckOptionValidStatic(index, accessoryConfig);
		}

		// Token: 0x06004192 RID: 16786 RVA: 0x00201FD8 File Offset: 0x002001D8
		public static bool CheckOptionValidStatic(int index, AccessoryItem accessoryConfig)
		{
			bool result;
			switch (index)
			{
			case 0:
				result = (accessoryConfig.RecoveryOfStance > 0);
				break;
			case 1:
				result = (accessoryConfig.RecoveryOfBreath > 0);
				break;
			case 2:
				result = (accessoryConfig.AttackSpeed > 0);
				break;
			case 3:
				result = (accessoryConfig.MoveSpeed > 0);
				break;
			case 4:
				result = (accessoryConfig.CastSpeed > 0);
				break;
			case 5:
				result = (accessoryConfig.WeaponSwitchSpeed > 0);
				break;
			case 6:
				result = (accessoryConfig.InnerRatio > 0);
				break;
			case 7:
				result = (accessoryConfig.RecoveryOfQiDisorder > 0);
				break;
			case 8:
				result = (accessoryConfig.RecoveryOfFlaw > 0);
				break;
			case 9:
				result = (accessoryConfig.RecoveryOfBlockedAcupoint > 0);
				break;
			default:
				result = true;
				break;
			}
			return result;
		}

		// Token: 0x06004193 RID: 16787 RVA: 0x00202098 File Offset: 0x00200298
		public static bool CheckAnyOptionValidStatic(AccessoryItem accessoryConfig)
		{
			for (int i = 0; i < Enum.GetValues(typeof(EAccessorySecondaryPropType)).Length; i++)
			{
				bool flag = AccessorySecondaryPropMenu.CheckOptionValidStatic(i, accessoryConfig);
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x17000719 RID: 1817
		// (get) Token: 0x06004194 RID: 16788 RVA: 0x002020DF File Offset: 0x002002DF
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(0, 1));
			}
		}
	}
}
