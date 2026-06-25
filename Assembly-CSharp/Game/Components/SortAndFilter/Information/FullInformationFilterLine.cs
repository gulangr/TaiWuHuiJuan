using System;
using System.Collections.Generic;

namespace Game.Components.SortAndFilter.Information
{
	// Token: 0x02000E00 RID: 3584
	public class FullInformationFilterLine : DetailedFilterLineLogic<InformationSortAndFilterData>
	{
		// Token: 0x170012D2 RID: 4818
		// (get) Token: 0x0600AADC RID: 43740 RVA: 0x004E9A4A File Offset: 0x004E7C4A
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x170012D3 RID: 4819
		// (get) Token: 0x0600AADD RID: 43741 RVA: 0x004E9A4D File Offset: 0x004E7C4D
		protected override int Level
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x170012D4 RID: 4820
		// (get) Token: 0x0600AADE RID: 43742 RVA: 0x004E9A50 File Offset: 0x004E7C50
		protected override bool IndividualLine
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600AADF RID: 43743 RVA: 0x004E9A54 File Offset: 0x004E7C54
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return null;
		}

		// Token: 0x0600AAE0 RID: 43744 RVA: 0x004E9A67 File Offset: 0x004E7C67
		protected override IEnumerable<DetailedFilterMenuLogic<InformationSortAndFilterData>> GenerateMenus()
		{
			yield return new MainMenu();
			yield return new SecondAreaMenu();
			yield return new SecondSectMenu();
			yield return new SecondLifeSkillMenu();
			yield return new SecondWesternMenu();
			yield return new SecondSwordTombMenu();
			yield return new SecondProfessionMenu();
			yield break;
		}
	}
}
