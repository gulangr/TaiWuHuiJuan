using System;
using UnityEngine.UI;

namespace Game.Components.ListStyleGeneralScroll
{
	// Token: 0x02000EA3 RID: 3747
	public struct LayoutOption
	{
		// Token: 0x0600AD88 RID: 44424 RVA: 0x004F24D2 File Offset: 0x004F06D2
		public LayoutOption(float minWidth, float flexibleWidth, float preferredWidth, int priority)
		{
			this = default(LayoutOption);
			this.MinWidth = minWidth;
			this.FlexibleWidth = flexibleWidth;
			this.PreferredWidth = preferredWidth;
			this.Priority = priority;
		}

		// Token: 0x0600AD89 RID: 44425 RVA: 0x004F24FC File Offset: 0x004F06FC
		public void ApplyTo(LayoutElement layoutElement)
		{
			bool flag = this.MinWidth > 0f;
			if (flag)
			{
				layoutElement.minWidth = this.MinWidth;
			}
			bool flag2 = this.MaxWidth > 0f;
			if (flag2)
			{
				layoutElement.preferredWidth = this.MaxWidth;
			}
			bool flag3 = this.PreferredWidth > 0f;
			if (flag3)
			{
				layoutElement.preferredWidth = this.PreferredWidth;
			}
			bool flag4 = this.FlexibleWidth >= 0f;
			if (flag4)
			{
				layoutElement.flexibleWidth = this.FlexibleWidth;
			}
			layoutElement.layoutPriority = this.Priority;
		}

		// Token: 0x0400860A RID: 34314
		public float MinWidth;

		// Token: 0x0400860B RID: 34315
		public float MaxWidth;

		// Token: 0x0400860C RID: 34316
		public float PreferredWidth;

		// Token: 0x0400860D RID: 34317
		public float FlexibleWidth;

		// Token: 0x0400860E RID: 34318
		public int Priority;
	}
}
