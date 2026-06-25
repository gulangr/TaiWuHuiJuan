using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace Game.Components.ListStyleGeneralScroll.CellContent
{
	// Token: 0x02000EC8 RID: 3784
	public struct MakeMaterialTipData
	{
		// Token: 0x0400877C RID: 34684
		public sbyte ResourceType;

		// Token: 0x0400877D RID: 34685
		public int Hardness;

		// Token: 0x0400877E RID: 34686
		public sbyte MinGrade;

		// Token: 0x0400877F RID: 34687
		public sbyte MaxGrade;

		// Token: 0x04008780 RID: 34688
		public List<ItemDisplayData> ItemDataList;
	}
}
