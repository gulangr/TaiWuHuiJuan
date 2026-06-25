using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using Game.Views.Main.Inscription;
using GameData.Domains.Character;

namespace Game.Components.SortAndFilter.Inscription
{
	// Token: 0x02000DFD RID: 3581
	public class InscriptionFilterLine : DetailedFilterLineLogic<CheckInscriptionCharData>
	{
		// Token: 0x170012CF RID: 4815
		// (get) Token: 0x0600AACE RID: 43726 RVA: 0x004E96E5 File Offset: 0x004E78E5
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600AACF RID: 43727 RVA: 0x004E96E8 File Offset: 0x004E78E8
		protected override IEnumerable<DetailedFilterMenuLogic<CheckInscriptionCharData>> GenerateMenus()
		{
			yield return new InscriptionFilterLine.GenderMenu();
			yield return new InscriptionFilterLine.BehaviorTypeMenu();
			yield break;
		}

		// Token: 0x170012D0 RID: 4816
		// (get) Token: 0x0600AAD0 RID: 43728 RVA: 0x004E96F8 File Offset: 0x004E78F8
		protected override int Level
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600AAD1 RID: 43729 RVA: 0x004E96FB File Offset: 0x004E78FB
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return null;
		}

		// Token: 0x020024B5 RID: 9397
		private enum EMenuId
		{
			// Token: 0x0400E572 RID: 58738
			Gender,
			// Token: 0x0400E573 RID: 58739
			BehaviorType
		}

		// Token: 0x020024B6 RID: 9398
		private class GenderMenu : DetailedFilterMenuLogic<CheckInscriptionCharData>
		{
			// Token: 0x17001B27 RID: 6951
			// (get) Token: 0x060108B6 RID: 67766 RVA: 0x00664A43 File Offset: 0x00662C43
			public override int Id
			{
				get
				{
					return 0;
				}
			}

			// Token: 0x060108B7 RID: 67767 RVA: 0x00664A48 File Offset: 0x00662C48
			public override StringKey GetMenuBarLabel()
			{
				return LanguageKey.LK_CommonSortAndFilter_DetailFilterMenu_Villager_0;
			}

			// Token: 0x060108B8 RID: 67768 RVA: 0x00664A64 File Offset: 0x00662C64
			public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
			{
				return new List<FilterDropdownItemConfig>
				{
					new FilterDropdownItemConfig
					{
						Text = LanguageKey.LK_Gender_Man
					},
					new FilterDropdownItemConfig
					{
						Text = LanguageKey.LK_Gender_Woman
					}
				};
			}

			// Token: 0x060108B9 RID: 67769 RVA: 0x00664AC0 File Offset: 0x00662CC0
			public override bool IsDataMatch(CheckInscriptionCharData data, IReadOnlyCollection<int> selectedIndices)
			{
				foreach (int index in selectedIndices)
				{
					int num = index;
					int num2 = num;
					if (num2 != 0)
					{
						if (num2 == 1)
						{
							bool flag = data.Character.Gender == 0;
							if (flag)
							{
								return true;
							}
						}
					}
					else
					{
						bool flag2 = data.Character.Gender == 1;
						if (flag2)
						{
							return true;
						}
					}
				}
				return false;
			}
		}

		// Token: 0x020024B7 RID: 9399
		private class BehaviorTypeMenu : DetailedFilterMenuLogic<CheckInscriptionCharData>
		{
			// Token: 0x17001B28 RID: 6952
			// (get) Token: 0x060108BB RID: 67771 RVA: 0x00664B5D File Offset: 0x00662D5D
			public override int Id
			{
				get
				{
					return 1;
				}
			}

			// Token: 0x060108BC RID: 67772 RVA: 0x00664B60 File Offset: 0x00662D60
			public override StringKey GetMenuBarLabel()
			{
				return LanguageKey.LK_CommonSortAndFilter_DetailFilterMenu_Villager_1;
			}

			// Token: 0x060108BD RID: 67773 RVA: 0x00664B7C File Offset: 0x00662D7C
			public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
			{
				return (from item in Config.BehaviorType.Instance
				select new FilterDropdownItemConfig
				{
					Text = StringKey.CreateDirect(item.Name)
				}).ToList<FilterDropdownItemConfig>();
			}

			// Token: 0x060108BE RID: 67774 RVA: 0x00664BBC File Offset: 0x00662DBC
			public override bool IsDataMatch(CheckInscriptionCharData data, IReadOnlyCollection<int> selectedIndices)
			{
				sbyte behaviorType = GameData.Domains.Character.BehaviorType.GetBehaviorType(data.Character.Morality);
				return selectedIndices.Any(delegate(int index)
				{
					BehaviorTypeItem config = Config.BehaviorType.Instance.ElementAtOrDefault(index);
					return config != null && config.TemplateId == (short)behaviorType;
				});
			}
		}
	}
}
