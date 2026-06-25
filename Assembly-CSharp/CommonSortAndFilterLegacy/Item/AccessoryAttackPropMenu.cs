using System;
using Config;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x020004AB RID: 1195
	public class AccessoryAttackPropMenu : AccessoryCommonPropTypeMenu
	{
		// Token: 0x1700071A RID: 1818
		// (get) Token: 0x06004196 RID: 16790 RVA: 0x002020F6 File Offset: 0x002002F6
		public override int Id
		{
			get
			{
				return 9;
			}
		}

		// Token: 0x06004197 RID: 16791 RVA: 0x002020FC File Offset: 0x002002FC
		public sealed override void OnInit()
		{
			this._optionNames.Clear();
			this._optionNames.Add("LK_Penetrate_Outer");
			this._optionNames.Add("LK_Penetrate_Inner");
			this._optionNames.Add("LK_HitType_0");
			this._optionNames.Add("LK_HitType_1");
			this._optionNames.Add("LK_HitType_2");
			this._optionNames.Add("LK_HitType_3");
		}

		// Token: 0x06004198 RID: 16792 RVA: 0x0020217C File Offset: 0x0020037C
		protected override bool CheckOptionValid(int index, AccessoryItem accessoryConfig)
		{
			return AccessoryAttackPropMenu.CheckOptionValidStatic(index, accessoryConfig);
		}

		// Token: 0x06004199 RID: 16793 RVA: 0x00202198 File Offset: 0x00200398
		public static bool CheckOptionValidStatic(int index, AccessoryItem accessoryConfig)
		{
			bool result;
			switch (index)
			{
			case 0:
				result = (accessoryConfig.PenetrateOfOuter > 0);
				break;
			case 1:
				result = (accessoryConfig.PenetrateOfInner > 0);
				break;
			case 2:
				result = (accessoryConfig.HitRateStrength > 0);
				break;
			case 3:
				result = (accessoryConfig.HitRateTechnique > 0);
				break;
			case 4:
				result = (accessoryConfig.HitRateSpeed > 0);
				break;
			case 5:
				result = (accessoryConfig.HitRateMind > 0);
				break;
			default:
				result = true;
				break;
			}
			return result;
		}

		// Token: 0x0600419A RID: 16794 RVA: 0x00202218 File Offset: 0x00200418
		public static bool CheckAnyOptionValidStatic(AccessoryItem accessoryConfig)
		{
			for (int i = 0; i < Enum.GetValues(typeof(EAccessoryAttackPropType)).Length; i++)
			{
				bool flag = AccessoryAttackPropMenu.CheckOptionValidStatic(i, accessoryConfig);
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x1700071B RID: 1819
		// (get) Token: 0x0600419B RID: 16795 RVA: 0x0020225F File Offset: 0x0020045F
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(0, 2));
			}
		}
	}
}
