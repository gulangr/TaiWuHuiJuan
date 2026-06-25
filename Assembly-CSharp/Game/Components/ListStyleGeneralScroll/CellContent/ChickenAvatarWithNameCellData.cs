using System;
using Config;

namespace Game.Components.ListStyleGeneralScroll.CellContent
{
	// Token: 0x02000EB7 RID: 3767
	public class ChickenAvatarWithNameCellData
	{
		// Token: 0x0600AEDD RID: 44765 RVA: 0x004FAB28 File Offset: 0x004F8D28
		public ChickenAvatarWithNameCellData(short templateId)
		{
			ChickenItem config = Chicken.Instance[templateId];
			this.DisplayName = config.Name;
			this.iconName = config.Display;
		}

		// Token: 0x04008739 RID: 34617
		public string iconName;

		// Token: 0x0400873A RID: 34618
		public string DisplayName;

		// Token: 0x0400873B RID: 34619
		public Action<int> OnClickCallback;

		// Token: 0x0400873C RID: 34620
		public Action<TooltipInvoker, int> MouseTipModifier;
	}
}
