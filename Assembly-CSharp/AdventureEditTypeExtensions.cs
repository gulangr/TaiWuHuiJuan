using System;

// Token: 0x0200016E RID: 366
public static class AdventureEditTypeExtensions
{
	// Token: 0x06001454 RID: 5204 RVA: 0x0007ED30 File Offset: 0x0007CF30
	public static bool Contains(this EAdventureEditType type, EAdventureEditType subType)
	{
		bool flag = type == EAdventureEditType.All;
		return flag || type == subType;
	}
}
