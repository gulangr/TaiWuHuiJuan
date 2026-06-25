using System;
using System.Collections.Generic;
using GameData.Domains.Character.Display;

namespace CommonSortAndFilterLegacy.Prison
{
	// Token: 0x02000473 RID: 1139
	public class MainFilterLine : DetailedFilterLine<CharacterDisplayDataForSettlementPrisoner>
	{
		// Token: 0x170006D3 RID: 1747
		// (get) Token: 0x060040CE RID: 16590 RVA: 0x00200375 File Offset: 0x001FE575
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x060040CF RID: 16591 RVA: 0x00200378 File Offset: 0x001FE578
		protected override IEnumerable<DetailedFilterMenuBase<CharacterDisplayDataForSettlementPrisoner>> GenerateMenus()
		{
			yield return new OrganizationMenu();
			yield return new PunishmentTypeMenu();
			yield break;
		}

		// Token: 0x170006D4 RID: 1748
		// (get) Token: 0x060040D0 RID: 16592 RVA: 0x00200388 File Offset: 0x001FE588
		protected override int Level
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x170006D5 RID: 1749
		// (get) Token: 0x060040D1 RID: 16593 RVA: 0x0020038B File Offset: 0x001FE58B
		protected override bool IndividualLine
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060040D2 RID: 16594 RVA: 0x00200390 File Offset: 0x001FE590
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return null;
		}
	}
}
