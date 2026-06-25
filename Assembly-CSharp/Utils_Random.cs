using System;
using System.Collections.Generic;
using System.Linq;
using FrameWork.ModSystem;
using MoonSharp.Interpreter;
using Redzen.Random;
using UnityEngine;

// Token: 0x0200003F RID: 63
public static class Utils_Random
{
	// Token: 0x0600021D RID: 541 RVA: 0x0000C9C4 File Offset: 0x0000ABC4
	public static List<Table> GenerateRandomList(int count, List<Table> paras, int weightIdx = 2)
	{
		IEnumerable<KeyValuePair<Table, int>> keyValues = from a in paras
		select new KeyValuePair<Table, int>(a, a.Get(weightIdx));
		return Utils_Random.GenerateRandomList<Table>(count, keyValues);
	}

	// Token: 0x0600021E RID: 542 RVA: 0x0000CA00 File Offset: 0x0000AC00
	public static List<T> GenerateRandomList<T>(int count, IEnumerable<KeyValuePair<T, int>> paras)
	{
		List<T> result = new List<T>(count);
		float totalWeight = 0f;
		float totalCount = 0f;
		foreach (KeyValuePair<T, int> entry in paras)
		{
			totalWeight += (float)entry.Value;
		}
		float minWeight = (float)count / totalWeight;
		List<KeyValuePair<T, int>> copyParas = new List<KeyValuePair<T, int>>(paras);
		while (copyParas.Count > 0 && result.Count < count)
		{
			int idx = Random.Range(0, copyParas.Count);
			KeyValuePair<T, int> curPara = copyParas[idx];
			copyParas.RemoveAt(idx);
			int curWeight = curPara.Value;
			totalCount += (float)count * ((float)curWeight / totalWeight);
			bool flag = (float)curWeight < minWeight;
			if (flag)
			{
				bool addOne = Utils_Random.RandomCheck((float)curWeight / totalCount, 1f);
				bool flag2 = addOne;
				if (flag2)
				{
					result.Insert(Random.Range(0, result.Count), curPara.Key);
				}
			}
			else
			{
				int addCount = Mathf.RoundToInt(totalCount) - result.Count;
				for (int i = 0; i < addCount; i++)
				{
					result.Insert(Random.Range(0, result.Count), curPara.Key);
				}
			}
		}
		return result;
	}

	// Token: 0x0600021F RID: 543 RVA: 0x0000CB64 File Offset: 0x0000AD64
	public static T[] GenerateRandomWeightCell<T>(IRandomSource random, IList<T[]> core, int weightIdx = 1) where T : IComparable, IConvertible
	{
		int totalWeight = 0;
		T[] coreWeightMaxCell = null;
		for (int i = 0; i < core.Count; i++)
		{
			T[] cellCore = core[i];
			totalWeight += cellCore[weightIdx].ToInt32(null);
			bool flag = coreWeightMaxCell == null || coreWeightMaxCell[weightIdx].CompareTo(cellCore[weightIdx]) < 0;
			if (flag)
			{
				coreWeightMaxCell = cellCore;
			}
		}
		int randWeight = random.Next(totalWeight);
		int weightLine = 0;
		for (int j = core.Count - 1; j >= 0; j--)
		{
			weightLine += core[j][weightIdx].ToInt32(null);
			bool flag2 = randWeight >= totalWeight - weightLine;
			if (flag2)
			{
				return core[j];
			}
		}
		return coreWeightMaxCell;
	}

	// Token: 0x06000220 RID: 544 RVA: 0x0000CC58 File Offset: 0x0000AE58
	public static int[] GenerateRandomWeightCell(IList<int[]> core, int weightIdx = 1)
	{
		int totalWeight = 0;
		int[] coreWeightMaxCell = null;
		foreach (int[] cellCore in core)
		{
			totalWeight += cellCore[weightIdx];
			bool flag = coreWeightMaxCell == null || coreWeightMaxCell[weightIdx] < cellCore[weightIdx];
			if (flag)
			{
				coreWeightMaxCell = cellCore;
			}
		}
		int randWeight = GameApp.RandomRange(0, totalWeight);
		int weightLine = 0;
		for (int i = core.Count - 1; i >= 0; i--)
		{
			weightLine += core[i][weightIdx];
			bool flag2 = randWeight >= totalWeight - weightLine;
			if (flag2)
			{
				return core[i];
			}
		}
		return coreWeightMaxCell;
	}

	// Token: 0x06000221 RID: 545 RVA: 0x0000CD24 File Offset: 0x0000AF24
	public static T GetRandomResult<T>(IEnumerable<KeyValuePair<T, int>> weights)
	{
		int totalWeight = 0;
		foreach (KeyValuePair<T, int> one in weights)
		{
			totalWeight += one.Value;
		}
		int randomValue = Random.Range(0, totalWeight);
		foreach (KeyValuePair<T, int> one2 in weights)
		{
			randomValue -= one2.Value;
			bool flag = randomValue < 0;
			if (flag)
			{
				return one2.Key;
			}
		}
		throw new Exception("Error Params");
	}

	// Token: 0x06000222 RID: 546 RVA: 0x0000CDE4 File Offset: 0x0000AFE4
	public static bool RandomCheck(float rate, float totalRate = 1f)
	{
		return Random.Range(0f, totalRate) <= rate;
	}

	// Token: 0x06000223 RID: 547 RVA: 0x0000CE08 File Offset: 0x0000B008
	public static bool RandomCheck(int rate, int totalRate = 100)
	{
		return Random.Range(0, totalRate) < rate;
	}

	// Token: 0x06000224 RID: 548 RVA: 0x0000CE24 File Offset: 0x0000B024
	public static double BoxMullerGaussianSample(float mean, float stdDev)
	{
		double u = 1.0 - (double)GameApp.RandomRange(0f, 1f);
		double u2 = 1.0 - (double)GameApp.RandomRange(0f, 1f);
		double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u)) * Math.Sin(6.283185307179586 * u2);
		return (double)mean + randStdNormal * (double)stdDev;
	}

	// Token: 0x06000225 RID: 549 RVA: 0x0000CE9C File Offset: 0x0000B09C
	public static double BoxMullerGaussianSample()
	{
		double u = 1.0 - (double)GameApp.RandomRange(0f, 1f);
		double u2 = 1.0 - (double)GameApp.RandomRange(0f, 1f);
		return Math.Sqrt(-2.0 * Math.Log(u)) * Math.Sin(6.283185307179586 * u2);
	}

	// Token: 0x06000226 RID: 550 RVA: 0x0000CF0C File Offset: 0x0000B10C
	public static int NormalDistribute(float mean, float stdDev)
	{
		return (int)Math.Round(Utils_Random.BoxMullerGaussianSample(mean, stdDev));
	}

	// Token: 0x06000227 RID: 551 RVA: 0x0000CF2C File Offset: 0x0000B12C
	public static int NormalDistribute(float mean, float stdDev, int min, int max)
	{
		int value = (int)Math.Round(Utils_Random.BoxMullerGaussianSample(mean, stdDev));
		bool flag = value < min;
		int result;
		if (flag)
		{
			result = min;
		}
		else
		{
			bool flag2 = value > max;
			if (flag2)
			{
				result = max;
			}
			else
			{
				result = value;
			}
		}
		return result;
	}

	// Token: 0x06000228 RID: 552 RVA: 0x0000CF68 File Offset: 0x0000B168
	public static float NormalDistribute(float mean, float stdDev, float min, float max)
	{
		double value = Utils_Random.BoxMullerGaussianSample(mean, stdDev);
		bool flag = value < (double)min;
		float result;
		if (flag)
		{
			result = min;
		}
		else
		{
			bool flag2 = value > (double)max;
			if (flag2)
			{
				result = max;
			}
			else
			{
				result = (float)value;
			}
		}
		return result;
	}

	// Token: 0x06000229 RID: 553 RVA: 0x0000CFA0 File Offset: 0x0000B1A0
	public static int SkewDistribute(float mean, float stdDev, float skewness, int min = -2147483648, int max = 2147483647)
	{
		Debug.Assert((double)Math.Abs(skewness) > 1.0);
		double doubleValue = Utils_Random.BoxMullerGaussianSample();
		bool flag = skewness > 0f;
		if (flag)
		{
			bool flag2 = doubleValue > 0.0;
			if (flag2)
			{
				doubleValue *= (double)skewness;
			}
		}
		else
		{
			bool flag3 = doubleValue < 0.0;
			if (flag3)
			{
				doubleValue *= (double)(-(double)skewness);
			}
		}
		int intValue = (int)Math.Round((double)mean + doubleValue * (double)stdDev);
		bool flag4 = intValue < min;
		int result;
		if (flag4)
		{
			result = min;
		}
		else
		{
			bool flag5 = intValue > max;
			if (flag5)
			{
				result = max;
			}
			else
			{
				result = intValue;
			}
		}
		return result;
	}

	// Token: 0x0600022A RID: 554 RVA: 0x0000D040 File Offset: 0x0000B240
	[Obsolete("use IList<T>.Shuffle() instead.")]
	public static T[] Disorganize<T>(T[] v)
	{
		for (int i = 0; i < v.Length; i++)
		{
			int idxA = Random.Range(0, v.Length);
			int idxB = Random.Range(0, v.Length);
			T src = v[idxA];
			v[idxA] = v[idxB];
			v[idxB] = src;
		}
		return v;
	}
}
