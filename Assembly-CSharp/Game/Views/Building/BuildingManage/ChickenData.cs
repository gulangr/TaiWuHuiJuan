using System;
using Config;

namespace Game.Views.Building.BuildingManage
{
	// Token: 0x02000BF6 RID: 3062
	public struct ChickenData
	{
		// Token: 0x17001070 RID: 4208
		// (get) Token: 0x06009BB0 RID: 39856 RVA: 0x0048F556 File Offset: 0x0048D756
		public readonly string Name
		{
			get
			{
				return this.NickName.IsNullOrEmpty() ? Chicken.Instance[this.TemplateId].Name : this.NickName;
			}
		}

		// Token: 0x040078A1 RID: 30881
		public int Id;

		// Token: 0x040078A2 RID: 30882
		public short TemplateId;

		// Token: 0x040078A3 RID: 30883
		public string NickName;

		// Token: 0x040078A4 RID: 30884
		public sbyte Happiness;
	}
}
