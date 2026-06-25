using System;
using System.Collections.Generic;

// Token: 0x02000025 RID: 37
public static class DictionaryExtensions
{
	// Token: 0x06000119 RID: 281 RVA: 0x000083E8 File Offset: 0x000065E8
	public static Dictionary<T, U> AddRangeOnlyAdd<T, U>(this Dictionary<T, U> destination, Dictionary<T, U> source)
	{
		bool flag = destination == null;
		if (flag)
		{
			destination = new Dictionary<T, U>(source.Count);
		}
		foreach (KeyValuePair<T, U> e in source)
		{
			destination.Add(e.Key, e.Value);
		}
		return destination;
	}

	// Token: 0x0600011A RID: 282 RVA: 0x00008464 File Offset: 0x00006664
	public static Dictionary<T, U> AddRangeOverride<T, U>(this Dictionary<T, U> destination, Dictionary<T, U> source)
	{
		bool flag = destination == null;
		if (flag)
		{
			destination = new Dictionary<T, U>(source.Count);
		}
		foreach (KeyValuePair<T, U> e in source)
		{
			destination[e.Key] = e.Value;
		}
		return destination;
	}
}
