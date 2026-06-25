using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using FrameWork;
using GameData.Utilities;
using UnityEngine;

// Token: 0x02000104 RID: 260
public static class CatchUtils
{
	// Token: 0x170000EE RID: 238
	// (get) Token: 0x0600090B RID: 2315 RVA: 0x0003E71B File Offset: 0x0003C91B
	public static float BaseSingTimeMin
	{
		get
		{
			return GlobalConfig.Instance.CricketSingBaseTimeMin;
		}
	}

	// Token: 0x170000EF RID: 239
	// (get) Token: 0x0600090C RID: 2316 RVA: 0x0003E727 File Offset: 0x0003C927
	public static float RandomSingTime
	{
		get
		{
			return Random.Range(CatchUtils.BaseSingTimeMin, GlobalConfig.Instance.CricketSingBaseTimeMax);
		}
	}

	// Token: 0x170000F0 RID: 240
	// (get) Token: 0x0600090D RID: 2317 RVA: 0x0003E73D File Offset: 0x0003C93D
	public static float RandomDelayTime
	{
		get
		{
			return Random.Range(GlobalConfig.Instance.CricketSingDelayTimeMin, GlobalConfig.Instance.CricketSingDelayTimeMax);
		}
	}

	// Token: 0x0600090E RID: 2318 RVA: 0x0003E758 File Offset: 0x0003C958
	private static void CheckCricketGroupConfig(int groupCount)
	{
		Tester.Assert(GlobalConfig.Instance.CricketSingGroupCricketCountMin.Length == groupCount, "");
		Tester.Assert(GlobalConfig.Instance.CricketSingGroupCricketCountMax.Length == groupCount, "");
		Tester.Assert(GlobalConfig.Instance.CricketSingGroupStartTimeMin.Length == groupCount, "");
		Tester.Assert(GlobalConfig.Instance.CricketSingGroupStartTimeMax.Length == groupCount, "");
		Tester.Assert(GlobalConfig.Instance.CricketSingGroupSingCountMin.Length == groupCount, "");
		Tester.Assert(GlobalConfig.Instance.CricketSingGroupSingCountMax.Length == groupCount, "");
	}

	// Token: 0x0600090F RID: 2319 RVA: 0x0003E804 File Offset: 0x0003CA04
	[return: TupleElementNames(new string[]
	{
		"index",
		"singTime",
		"singCount"
	})]
	private static ValueTuple<int, float, short> RandomGroupPlace(int groupIndex, int placeIndex)
	{
		float singTime = Random.Range(GlobalConfig.Instance.CricketSingGroupStartTimeMin[groupIndex], GlobalConfig.Instance.CricketSingGroupStartTimeMax[groupIndex]);
		short singCount = (short)Random.Range((int)GlobalConfig.Instance.CricketSingGroupSingCountMin[groupIndex], (int)GlobalConfig.Instance.CricketSingGroupSingCountMax[groupIndex]);
		return new ValueTuple<int, float, short>(placeIndex, singTime, singCount);
	}

	// Token: 0x06000910 RID: 2320 RVA: 0x0003E85B File Offset: 0x0003CA5B
	[return: TupleElementNames(new string[]
	{
		"index",
		"singTime",
		"singCount"
	})]
	public static IEnumerable<ValueTuple<int, float, short>> RandomGroups(int kingIndex)
	{
		int groupCount = GlobalConfig.Instance.CricketSingGroupCricketCountMin.Length;
		CatchUtils.CheckCricketGroupConfig(groupCount);
		List<int> groupPool = EasyPool.Get<List<int>>();
		groupPool.Clear();
		int num;
		for (int i = 0; i < 21; i = num + 1)
		{
			groupPool.Add(i);
			num = i;
		}
		bool flag = kingIndex >= 0;
		if (flag)
		{
			groupPool.Remove(kingIndex);
			int kingGroup = Random.Range(0, groupCount);
			yield return CatchUtils.RandomGroupPlace(kingGroup, kingIndex);
		}
		for (int groupIndex = 0; groupIndex < groupCount; groupIndex = num + 1)
		{
			int placeCount = Random.Range((int)GlobalConfig.Instance.CricketSingGroupCricketCountMin[groupIndex], (int)(GlobalConfig.Instance.CricketSingGroupCricketCountMax[groupIndex] + 1));
			placeCount = Mathf.Min(placeCount, groupPool.Count);
			for (int j = 0; j < placeCount; j = num + 1)
			{
				int placeIndex = groupPool.GetRandom<int>();
				groupPool.Remove(placeIndex);
				yield return CatchUtils.RandomGroupPlace(groupIndex, placeIndex);
				num = j;
			}
			num = groupIndex;
		}
		EasyPool.Free<List<int>>(groupPool);
		yield break;
	}

	// Token: 0x04000C08 RID: 3080
	public const sbyte CatchPlaceCount = 21;

	// Token: 0x04000C09 RID: 3081
	public const float TotalCatchTime = 30f;

	// Token: 0x04000C0A RID: 3082
	public const short LoudSingLevel = 80;
}
