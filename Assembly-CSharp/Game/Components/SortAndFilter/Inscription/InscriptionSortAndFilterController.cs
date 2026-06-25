using System;
using System.Collections.Generic;
using Game.Views.Main.Inscription;

namespace Game.Components.SortAndFilter.Inscription
{
	// Token: 0x02000DFE RID: 3582
	public class InscriptionSortAndFilterController : SortAndFilterController<CheckInscriptionCharData>
	{
		// Token: 0x0600AAD3 RID: 43731 RVA: 0x004E9707 File Offset: 0x004E7907
		public InscriptionSortAndFilterController(ISortAndFilterView sortAndFilter, bool ignorePinOrder = false) : base(sortAndFilter, LanguageKey.UI_CheckInspcription_Title)
		{
			this.SortController = new InscriptionSortController
			{
				IgnorePinOrder = ignorePinOrder
			};
		}

		// Token: 0x0600AAD4 RID: 43732 RVA: 0x004E972A File Offset: 0x004E792A
		protected override IEnumerable<FilterLineBase<CheckInscriptionCharData>> GenerateFilterLines()
		{
			yield return new InscriptionFilterLine();
			yield break;
		}
	}
}
