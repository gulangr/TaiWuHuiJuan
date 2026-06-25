using System;
using Config;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x020004AC RID: 1196
	public class AccessoryDefPropMenu : AccessoryCommonPropTypeMenu
	{
		// Token: 0x1700071C RID: 1820
		// (get) Token: 0x0600419D RID: 16797 RVA: 0x00202276 File Offset: 0x00200476
		public override int Id
		{
			get
			{
				return 10;
			}
		}

		// Token: 0x0600419E RID: 16798 RVA: 0x0020227C File Offset: 0x0020047C
		public sealed override void OnInit()
		{
			this._optionNames.Clear();
			this._optionNames.Add("LK_Penetrate_Resist_Outer");
			this._optionNames.Add("LK_Penetrate_Resist_Inner");
			this._optionNames.Add("LK_AvoidType_0");
			this._optionNames.Add("LK_AvoidType_1");
			this._optionNames.Add("LK_AvoidType_2");
			this._optionNames.Add("LK_AvoidType_3");
		}

		// Token: 0x0600419F RID: 16799 RVA: 0x002022FC File Offset: 0x002004FC
		protected override bool CheckOptionValid(int index, AccessoryItem accessoryConfig)
		{
			return AccessoryDefPropMenu.CheckOptionValidStatic(index, accessoryConfig);
		}

		// Token: 0x060041A0 RID: 16800 RVA: 0x00202318 File Offset: 0x00200518
		public static bool CheckOptionValidStatic(int index, AccessoryItem accessoryConfig)
		{
			bool result;
			switch (index)
			{
			case 0:
				result = (accessoryConfig.PenetrateResistOfOuter > 0);
				break;
			case 1:
				result = (accessoryConfig.PenetrateResistOfInner > 0);
				break;
			case 2:
				result = (accessoryConfig.AvoidRateStrength > 0);
				break;
			case 3:
				result = (accessoryConfig.AvoidRateTechnique > 0);
				break;
			case 4:
				result = (accessoryConfig.AvoidRateSpeed > 0);
				break;
			case 5:
				result = (accessoryConfig.AvoidRateMind > 0);
				break;
			default:
				result = true;
				break;
			}
			return result;
		}

		// Token: 0x060041A1 RID: 16801 RVA: 0x00202398 File Offset: 0x00200598
		public static bool CheckAnyOptionValidStatic(AccessoryItem accessoryConfig)
		{
			for (int i = 0; i < Enum.GetValues(typeof(EAccessoryDefencePropType)).Length; i++)
			{
				bool flag = AccessoryDefPropMenu.CheckOptionValidStatic(i, accessoryConfig);
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x1700071D RID: 1821
		// (get) Token: 0x060041A2 RID: 16802 RVA: 0x002023DF File Offset: 0x002005DF
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(0, 3));
			}
		}
	}
}
