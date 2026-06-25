using System;
using Game.Views.Encyclopedia.Event;

namespace Game.Views.Encyclopedia
{
	// Token: 0x02000A63 RID: 2659
	public interface IEncyclopediaSearchableContent
	{
		// Token: 0x060082B7 RID: 33463
		bool Contains(string str);

		// Token: 0x060082B8 RID: 33464
		bool StartsWith(string str);

		// Token: 0x17000E5E RID: 3678
		// (get) Token: 0x060082B9 RID: 33465
		int Length { get; }

		// Token: 0x17000E5F RID: 3679
		// (get) Token: 0x060082BA RID: 33466
		string DisplayText { get; }

		// Token: 0x17000E60 RID: 3680
		// (get) Token: 0x060082BB RID: 33467
		string Content { get; }

		// Token: 0x17000E61 RID: 3681
		// (get) Token: 0x060082BC RID: 33468
		int ClickEventType { get; }

		// Token: 0x17000E62 RID: 3682
		// (get) Token: 0x060082BD RID: 33469
		IEventArgs ClickEventArgs { get; }
	}
}
