using System;
using GameData.Domains.Mod;

namespace Game.Components.SortAndFilter.Mod
{
	// Token: 0x02000D16 RID: 3350
	public class ModSortAndFilterData
	{
		// Token: 0x04008359 RID: 33625
		public ModId ModId;

		// Token: 0x0400835A RID: 33626
		public bool IsLocal;

		// Token: 0x0400835B RID: 33627
		public bool IsExpired;

		// Token: 0x0400835C RID: 33628
		public string Name;

		// Token: 0x0400835D RID: 33629
		public float Rate;

		// Token: 0x0400835E RID: 33630
		public uint UpdateTime;

		// Token: 0x0400835F RID: 33631
		public uint UploadTime;

		// Token: 0x04008360 RID: 33632
		public int Size;

		// Token: 0x04008361 RID: 33633
		public int Tags;

		// Token: 0x04008362 RID: 33634
		public Func<ModId, bool> IsEnabled;

		// Token: 0x04008363 RID: 33635
		public Func<ModId, int> GetOrder;
	}
}
