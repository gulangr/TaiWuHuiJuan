using System;

namespace Game.Components.ListStyleGeneralScroll.CellContent
{
	// Token: 0x02000EBA RID: 3770
	public struct IconAndTextCellData
	{
		// Token: 0x0600AEE5 RID: 44773 RVA: 0x004FACA9 File Offset: 0x004F8EA9
		public IconAndTextCellData(string iconName, string text, bool showIcon = true, bool hideIconIfEmpty = false, bool useNativeSize = false, bool labelInFront = false)
		{
			this.IconName = iconName;
			this.Text = text;
			this.ShowIcon = showIcon;
			this.HideIconIfEmpty = hideIconIfEmpty;
			this.UseNativeSize = useNativeSize;
			this.LabelInFront = labelInFront;
		}

		// Token: 0x0600AEE6 RID: 44774 RVA: 0x004FACDC File Offset: 0x004F8EDC
		public static IconAndTextCellData TextOnly(string text)
		{
			return new IconAndTextCellData(null, text, false, false, false, false);
		}

		// Token: 0x04008741 RID: 34625
		public string IconName;

		// Token: 0x04008742 RID: 34626
		public string Text;

		// Token: 0x04008743 RID: 34627
		public bool ShowIcon;

		// Token: 0x04008744 RID: 34628
		public bool HideIconIfEmpty;

		// Token: 0x04008745 RID: 34629
		public bool UseNativeSize;

		// Token: 0x04008746 RID: 34630
		public bool LabelInFront;
	}
}
