using System;
using System.Collections.Generic;

namespace Game.Views
{
	// Token: 0x020006FA RID: 1786
	public class ConfirmDialogCmd
	{
		// Token: 0x04003976 RID: 14710
		public string Title;

		// Token: 0x04003977 RID: 14711
		public string ContentUpper;

		// Token: 0x04003978 RID: 14712
		public List<ConfirmDialogCost> ConfirmDialogCost;

		// Token: 0x04003979 RID: 14713
		public List<ChangeInfo> ChangeInfos;

		// Token: 0x0400397A RID: 14714
		public List<GainInfo> GainInfos;

		// Token: 0x0400397B RID: 14715
		public string ContentLower;

		// Token: 0x0400397C RID: 14716
		public Action Yes;

		// Token: 0x0400397D RID: 14717
		public Action No;

		// Token: 0x0400397E RID: 14718
		public sbyte ValueStyle;
	}
}
