using System;
using System.Collections.Generic;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x0200053D RID: 1341
	public class WoodHardnessMenu : CommonMaterialHardnessMenu
	{
		// Token: 0x170007EE RID: 2030
		// (get) Token: 0x06004394 RID: 17300 RVA: 0x00207567 File Offset: 0x00205767
		public override int Id
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x06004395 RID: 17301 RVA: 0x0020756A File Offset: 0x0020576A
		protected override IEnumerable<EMaterialFilterHardness> GenerateMaterialHardnessList()
		{
			yield return EMaterialFilterHardness.Wooden;
			yield return EMaterialFilterHardness.Rattan;
			yield break;
		}
	}
}
