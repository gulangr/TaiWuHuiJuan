using System;
using System.Collections.Generic;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000DA6 RID: 3494
	public class JadeHardnessMenu : MaterialCommonHardnessMenu
	{
		// Token: 0x1700125F RID: 4703
		// (get) Token: 0x0600A952 RID: 43346 RVA: 0x004E5137 File Offset: 0x004E3337
		public override int Id
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x0600A953 RID: 43347 RVA: 0x004E513A File Offset: 0x004E333A
		protected override IEnumerable<EMaterialFilterHardness> GenerateMaterialHardnessList()
		{
			yield return EMaterialFilterHardness.Stone;
			yield return EMaterialFilterHardness.Jade;
			yield break;
		}
	}
}
