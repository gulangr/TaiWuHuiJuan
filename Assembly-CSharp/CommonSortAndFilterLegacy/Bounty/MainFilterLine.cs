using System;
using System.Collections.Generic;
using GameData.Domains.Character.Display;

namespace CommonSortAndFilterLegacy.Bounty
{
	// Token: 0x020005AE RID: 1454
	public class MainFilterLine : DetailedFilterLine<CharacterDisplayDataForSettlementBounty>
	{
		// Token: 0x170008B1 RID: 2225
		// (get) Token: 0x060045A1 RID: 17825 RVA: 0x0020CBA8 File Offset: 0x0020ADA8
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x060045A2 RID: 17826 RVA: 0x0020CBAB File Offset: 0x0020ADAB
		protected override IEnumerable<DetailedFilterMenuBase<CharacterDisplayDataForSettlementBounty>> GenerateMenus()
		{
			yield return new PunishmentTypeMenu();
			yield return new PunishmentSeverityMenu();
			yield return new StatusMenu();
			yield return new LocationMenu();
			yield break;
		}

		// Token: 0x170008B2 RID: 2226
		// (get) Token: 0x060045A3 RID: 17827 RVA: 0x0020CBBB File Offset: 0x0020ADBB
		protected override int Level
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x170008B3 RID: 2227
		// (get) Token: 0x060045A4 RID: 17828 RVA: 0x0020CBBE File Offset: 0x0020ADBE
		protected override bool IndividualLine
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060045A5 RID: 17829 RVA: 0x0020CBC4 File Offset: 0x0020ADC4
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return null;
		}
	}
}
