using System;
using System.Collections.Generic;
using System.Linq;

// Token: 0x0200012C RID: 300
internal static class GameLifetimeDataManager
{
	// Token: 0x06000CC3 RID: 3267 RVA: 0x00054910 File Offset: 0x00052B10
	public static void RecordInteractionEventOptionSelected(short interactionEventOptionTemplateId)
	{
		GameLifetimeDataManager._recentInteractionEventOptionQueue.Enqueue(interactionEventOptionTemplateId);
		while (GameLifetimeDataManager._recentInteractionEventOptionQueue.Count > 5)
		{
			GameLifetimeDataManager._recentInteractionEventOptionQueue.Dequeue();
		}
	}

	// Token: 0x06000CC4 RID: 3268 RVA: 0x0005494C File Offset: 0x00052B4C
	public static short GetTopNInteractionEventOptions()
	{
		bool flag = GameLifetimeDataManager._recentInteractionEventOptionQueue.Count == 0;
		short result;
		if (flag)
		{
			result = -1;
		}
		else
		{
			short mostFrequentOption = (from id in GameLifetimeDataManager._recentInteractionEventOptionQueue
			group id by id into @group
			orderby @group.Count<short>() descending
			select @group).First<IGrouping<short, short>>().Key;
			result = mostFrequentOption;
		}
		return result;
	}

	// Token: 0x04000DDA RID: 3546
	private const int RecordLength = 5;

	// Token: 0x04000DDB RID: 3547
	private static readonly Queue<short> _recentInteractionEventOptionQueue = new Queue<short>();
}
