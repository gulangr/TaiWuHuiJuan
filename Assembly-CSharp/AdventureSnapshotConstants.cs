using System;
using GameData.Adventure;
using GameData.Adventure.Editor;

// Token: 0x0200018D RID: 397
public static class AdventureSnapshotConstants
{
	// Token: 0x17000278 RID: 632
	// (get) Token: 0x06001667 RID: 5735 RVA: 0x0008A6CB File Offset: 0x000888CB
	public static AdventureParameterSnapshot ViewTypeDefaultNear
	{
		get
		{
			return new AdventureParameterSnapshot
			{
				Key = "view_range_0",
				InitialValue = 1,
				Type = EAdventureParameterType.Influence,
				Name = LocalStringManager.Get(LanguageKey.LK_Adventure_Editor_ViewRange_Near)
			};
		}
	}

	// Token: 0x17000279 RID: 633
	// (get) Token: 0x06001668 RID: 5736 RVA: 0x0008A700 File Offset: 0x00088900
	public static AdventureParameterSnapshot ViewTypeDefaultFar
	{
		get
		{
			return new AdventureParameterSnapshot
			{
				Key = "view_range_1",
				InitialValue = 3,
				Type = EAdventureParameterType.Influence,
				Name = LocalStringManager.Get(LanguageKey.LK_Adventure_Editor_ViewRange_Far)
			};
		}
	}
}
