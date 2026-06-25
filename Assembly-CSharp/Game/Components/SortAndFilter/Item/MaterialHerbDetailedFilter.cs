using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000DAE RID: 3502
	public class MaterialHerbDetailedFilter : DetailedFilterLineLogic<ITradeableContent>
	{
		// Token: 0x17001264 RID: 4708
		// (get) Token: 0x0600A95F RID: 43359 RVA: 0x004E51C7 File Offset: 0x004E33C7
		public override int Id
		{
			get
			{
				return 32;
			}
		}

		// Token: 0x17001265 RID: 4709
		// (get) Token: 0x0600A960 RID: 43360 RVA: 0x004E51CB File Offset: 0x004E33CB
		protected override int Level
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x0600A961 RID: 43361 RVA: 0x004E51CE File Offset: 0x004E33CE
		protected override IEnumerable<DetailedFilterMenuLogic<ITradeableContent>> GenerateMenus()
		{
			yield return new MaterialHerbTreatmentMenu();
			yield return new MaterialHerbBuffMenu();
			yield return new MaterialHerbCureMenu();
			yield return new MaterialHerbPropertyMenu();
			yield break;
		}

		// Token: 0x0600A962 RID: 43362 RVA: 0x004E51E0 File Offset: 0x004E33E0
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(6, ToggleKey.CreateIndexKey(4))
			};
		}
	}
}
