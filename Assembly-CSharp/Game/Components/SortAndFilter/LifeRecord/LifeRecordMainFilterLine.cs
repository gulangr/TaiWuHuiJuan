using System;
using System.Collections.Generic;
using GameData.Domains.LifeRecord;

namespace Game.Components.SortAndFilter.LifeRecord
{
	// Token: 0x02000D1E RID: 3358
	public class LifeRecordMainFilterLine : DetailedFilterLineLogic<TransferableRecord>
	{
		// Token: 0x17001198 RID: 4504
		// (get) Token: 0x0600A74F RID: 42831 RVA: 0x004DD98D File Offset: 0x004DBB8D
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600A750 RID: 42832 RVA: 0x004DD990 File Offset: 0x004DBB90
		protected override IEnumerable<DetailedFilterMenuLogic<TransferableRecord>> GenerateMenus()
		{
			yield return new LifeRecordTypeMenu();
			yield break;
		}

		// Token: 0x17001199 RID: 4505
		// (get) Token: 0x0600A751 RID: 42833 RVA: 0x004DD9A0 File Offset: 0x004DBBA0
		protected override int Level
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x1700119A RID: 4506
		// (get) Token: 0x0600A752 RID: 42834 RVA: 0x004DD9A3 File Offset: 0x004DBBA3
		protected override bool IndividualLine
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600A753 RID: 42835 RVA: 0x004DD9A8 File Offset: 0x004DBBA8
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return null;
		}
	}
}
