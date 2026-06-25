using System;
using Config;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x020004AD RID: 1197
	public class AccessoryPosionPropMenu : AccessoryCommonPropTypeMenu
	{
		// Token: 0x1700071E RID: 1822
		// (get) Token: 0x060041A4 RID: 16804 RVA: 0x002023F6 File Offset: 0x002005F6
		public override int Id
		{
			get
			{
				return 11;
			}
		}

		// Token: 0x060041A5 RID: 16805 RVA: 0x002023FC File Offset: 0x002005FC
		public sealed override void OnInit()
		{
			this._optionNames.Clear();
			this._optionNames.Add("LK_Poison_Name_0");
			this._optionNames.Add("LK_Poison_Name_1");
			this._optionNames.Add("LK_Poison_Name_2");
			this._optionNames.Add("LK_Poison_Name_3");
			this._optionNames.Add("LK_Poison_Name_4");
			this._optionNames.Add("LK_Poison_Name_5");
		}

		// Token: 0x060041A6 RID: 16806 RVA: 0x0020247C File Offset: 0x0020067C
		protected override bool CheckOptionValid(int index, AccessoryItem accessoryConfig)
		{
			return AccessoryPosionPropMenu.CheckOptionValidStatic(index, accessoryConfig);
		}

		// Token: 0x060041A7 RID: 16807 RVA: 0x00202498 File Offset: 0x00200698
		public static bool CheckOptionValidStatic(int index, AccessoryItem accessoryConfig)
		{
			bool result;
			switch (index)
			{
			case 0:
				result = (accessoryConfig.ResistOfHotPoison > 0);
				break;
			case 1:
				result = (accessoryConfig.ResistOfGloomyPoison > 0);
				break;
			case 2:
				result = (accessoryConfig.ResistOfColdPoison > 0);
				break;
			case 3:
				result = (accessoryConfig.ResistOfRedPoison > 0);
				break;
			case 4:
				result = (accessoryConfig.ResistOfRottenPoison > 0);
				break;
			case 5:
				result = (accessoryConfig.ResistOfIllusoryPoison > 0);
				break;
			default:
				result = true;
				break;
			}
			return result;
		}

		// Token: 0x060041A8 RID: 16808 RVA: 0x00202518 File Offset: 0x00200718
		public static bool CheckAnyOptionValidStatic(AccessoryItem accessoryConfig)
		{
			for (int i = 0; i < Enum.GetValues(typeof(EAccessoryPosionPropType)).Length; i++)
			{
				bool flag = AccessoryPosionPropMenu.CheckOptionValidStatic(i, accessoryConfig);
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x1700071F RID: 1823
		// (get) Token: 0x060041A9 RID: 16809 RVA: 0x0020255F File Offset: 0x0020075F
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(0, 4));
			}
		}
	}
}
