using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Game.Views.Encyclopedia.Utilities
{
	// Token: 0x02000A74 RID: 2676
	public class OptimizedHtmlPatternMatcher
	{
		// Token: 0x060083BF RID: 33727 RVA: 0x003D4CF8 File Offset: 0x003D2EF8
		public OptimizedHtmlPatternMatcher(string pattern)
		{
			this.Pattern = pattern;
			this._next = OptimizedHtmlPatternMatcher.ComputeKmpNext(pattern);
			this._matchPositions = new int[pattern.Length];
			this._segmentBuffer = new List<ValueTuple<int, int>>(pattern.Length);
		}

		// Token: 0x060083C0 RID: 33728 RVA: 0x003D4D4D File Offset: 0x003D2F4D
		public bool Matches(string input)
		{
			return this.FindMatches(input).Any(([TupleElementNames(new string[]
			{
				"index",
				"start",
				"end"
			})] ValueTuple<int, int, int> _) => true);
		}

		// Token: 0x060083C1 RID: 33729 RVA: 0x003D4D7A File Offset: 0x003D2F7A
		[return: TupleElementNames(new string[]
		{
			"index",
			"start",
			"end"
		})]
		public IEnumerable<ValueTuple<int, int, int>> FindMatches(string x)
		{
			bool flag = string.IsNullOrEmpty(x);
			if (flag)
			{
				yield break;
			}
			int num;
			for (int i = 0; i < this._matchPositions.Length; i = num + 1)
			{
				this._matchPositions[i] = -1;
				num = i;
			}
			this._segmentBuffer.Clear();
			int patternLength = this.Pattern.Length;
			int j = 0;
			bool inTag = false;
			int matchIndex = 0;
			for (int k = 0; k < x.Length; k = num + 1)
			{
				char currentChar = x[k];
				bool flag2 = currentChar == '<';
				if (flag2)
				{
					inTag = true;
				}
				else
				{
					bool flag3 = currentChar == '>';
					if (flag3)
					{
						inTag = false;
					}
					else
					{
						bool flag4 = inTag;
						if (!flag4)
						{
							while (j > 0 && char.ToUpper(currentChar) != char.ToUpper(this.Pattern[j]))
							{
								j = this._next[j - 1];
								this.ClearPositionsFrom(j);
							}
							bool flag5 = char.ToUpper(currentChar) == char.ToUpper(this.Pattern[j]);
							if (flag5)
							{
								this._matchPositions[j] = k;
								num = j;
								j = num + 1;
								bool flag6 = j == patternLength;
								if (flag6)
								{
									OptimizedHtmlPatternMatcher.GetContinuousSegmentsToBuffer(this._matchPositions, this._segmentBuffer);
									foreach (ValueTuple<int, int> segment in this._segmentBuffer)
									{
										yield return new ValueTuple<int, int, int>(matchIndex, segment.Item1, segment.Item2 + 1);
										segment = default(ValueTuple<int, int>);
									}
									List<ValueTuple<int, int>>.Enumerator enumerator = default(List<ValueTuple<int, int>>.Enumerator);
									num = matchIndex;
									matchIndex = num + 1;
									j = this._next[j - 1];
									bool flag7 = j > 0;
									if (flag7)
									{
										for (int l = j; l < patternLength; l = num + 1)
										{
											this._matchPositions[l] = -1;
											num = l;
										}
									}
									else
									{
										for (int m = 0; m < patternLength; m = num + 1)
										{
											this._matchPositions[m] = -1;
											num = m;
										}
									}
								}
							}
							else
							{
								bool flag8 = j > 0;
								if (flag8)
								{
									j = 0;
									for (int n = 0; n < patternLength; n = num + 1)
									{
										this._matchPositions[n] = -1;
										num = n;
									}
								}
							}
						}
					}
				}
				num = k;
			}
			yield break;
			yield break;
		}

		// Token: 0x060083C2 RID: 33730 RVA: 0x003D4D94 File Offset: 0x003D2F94
		private void ClearPositionsFrom(int fromIndex)
		{
			for (int i = fromIndex; i < this._matchPositions.Length; i++)
			{
				this._matchPositions[i] = -1;
			}
		}

		// Token: 0x060083C3 RID: 33731 RVA: 0x003D4DC4 File Offset: 0x003D2FC4
		private static void GetContinuousSegmentsToBuffer(int[] positions, [TupleElementNames(new string[]
		{
			"start",
			"end"
		})] List<ValueTuple<int, int>> buffer)
		{
			buffer.Clear();
			int segmentStart = -1;
			int segmentEnd = -1;
			for (int i = 0; i < positions.Length; i++)
			{
				bool flag = positions[i] == -1;
				if (!flag)
				{
					bool flag2 = segmentStart == -1;
					if (flag2)
					{
						segmentStart = positions[i];
						segmentEnd = positions[i];
					}
					else
					{
						bool flag3 = positions[i] == segmentEnd + 1;
						if (flag3)
						{
							segmentEnd = positions[i];
						}
						else
						{
							buffer.Add(new ValueTuple<int, int>(segmentStart, segmentEnd));
							segmentStart = positions[i];
							segmentEnd = positions[i];
						}
					}
				}
			}
			bool flag4 = segmentStart != -1;
			if (flag4)
			{
				buffer.Add(new ValueTuple<int, int>(segmentStart, segmentEnd));
			}
		}

		// Token: 0x060083C4 RID: 33732 RVA: 0x003D4E60 File Offset: 0x003D3060
		private static int[] ComputeKmpNext(string pattern)
		{
			bool flag = pattern.Length == 0;
			int[] result;
			if (flag)
			{
				result = Array.Empty<int>();
			}
			else
			{
				int[] next = new int[pattern.Length];
				next[0] = 0;
				int i = 1;
				int len = 0;
				while (i < pattern.Length)
				{
					bool flag2 = pattern[i] == pattern[len];
					if (flag2)
					{
						len = (next[i++] = len + 1);
					}
					else
					{
						bool flag3 = len > 0;
						if (flag3)
						{
							len = next[len - 1];
						}
						else
						{
							next[i++] = 0;
						}
					}
				}
				result = next;
			}
			return result;
		}

		// Token: 0x040064DE RID: 25822
		public readonly string Pattern;

		// Token: 0x040064DF RID: 25823
		private readonly int[] _next;

		// Token: 0x040064E0 RID: 25824
		private int[] _matchPositions;

		// Token: 0x040064E1 RID: 25825
		[TupleElementNames(new string[]
		{
			"start",
			"end"
		})]
		private List<ValueTuple<int, int>> _segmentBuffer;

		// Token: 0x040064E2 RID: 25826
		public StringBuilder Sb = new StringBuilder();
	}
}
