using System;

namespace UICommon.Character.Elements
{
	// Token: 0x020005F9 RID: 1529
	public struct ToggleTransitionIconSpriteNames
	{
		// Token: 0x06004833 RID: 18483 RVA: 0x0021DF8D File Offset: 0x0021C18D
		public ToggleTransitionIconSpriteNames(string normal, string highlighted, string selected, string disabled)
		{
			this.Normal = normal;
			this.Highlighted = highlighted;
			this.Selected = selected;
			this.Disabled = disabled;
		}

		// Token: 0x06004834 RID: 18484 RVA: 0x0021DFB0 File Offset: 0x0021C1B0
		public static ToggleTransitionIconSpriteNames Default()
		{
			return new ToggleTransitionIconSpriteNames(null, null, null, null);
		}

		// Token: 0x06004835 RID: 18485 RVA: 0x0021DFCC File Offset: 0x0021C1CC
		public static ToggleTransitionIconSpriteNames CreateWithSameIcon(string iconName)
		{
			return new ToggleTransitionIconSpriteNames(iconName, iconName, iconName, iconName);
		}

		// Token: 0x040031E0 RID: 12768
		public readonly string Normal;

		// Token: 0x040031E1 RID: 12769
		public readonly string Highlighted;

		// Token: 0x040031E2 RID: 12770
		public readonly string Selected;

		// Token: 0x040031E3 RID: 12771
		public readonly string Disabled;
	}
}
