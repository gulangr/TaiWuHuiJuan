using System;
using System.Collections.Generic;
using FrameWork;

// Token: 0x02000020 RID: 32
public class SensitiveWordsMatchResult
{
	// Token: 0x1700002F RID: 47
	// (get) Token: 0x060000F7 RID: 247 RVA: 0x00007738 File Offset: 0x00005938
	public string MatchContent
	{
		get
		{
			bool flag = !this.IsMatchSuccess;
			string result;
			if (flag)
			{
				result = string.Empty;
			}
			else
			{
				result = new string(this.ResultList.ConvertAll<char>((ValueTuple<int, SensitiveWordsNode> e) => e.Item2.NodeKey).ToArray());
			}
			return result;
		}
	}

	// Token: 0x17000030 RID: 48
	// (get) Token: 0x060000F8 RID: 248 RVA: 0x00007793 File Offset: 0x00005993
	// (set) Token: 0x060000F9 RID: 249 RVA: 0x0000779B File Offset: 0x0000599B
	public bool IsMatchSuccess { get; private set; }

	// Token: 0x17000031 RID: 49
	// (get) Token: 0x060000FA RID: 250 RVA: 0x000077A4 File Offset: 0x000059A4
	public ESensitiveWordsType Type
	{
		get
		{
			bool flag = !this.IsMatchSuccess || this.ResultList.Count == 0;
			ESensitiveWordsType result;
			if (flag)
			{
				result = ESensitiveWordsType.Undetermined;
			}
			else
			{
				result = this.ResultList[this.ResultList.Count - 1].Item2.Config.Type;
			}
			return result;
		}
	}

	// Token: 0x060000FB RID: 251 RVA: 0x00007800 File Offset: 0x00005A00
	public List<SensitiveWordsMatchResult> CheckMatch(char key, int index, int maxOffset)
	{
		bool flag = this.ResultList == null || this.ResultList.Count <= 0;
		List<SensitiveWordsMatchResult> result;
		if (flag)
		{
			result = null;
		}
		else
		{
			int lastIndex = this.ResultList[this.ResultList.Count - 1].Item1;
			bool flag2 = index - lastIndex > maxOffset;
			if (flag2)
			{
				result = null;
			}
			else
			{
				List<SensitiveWordsMatchResult> list = EasyPool.Get<List<SensitiveWordsMatchResult>>();
				list.Add(this);
				int i = 0;
				int max = this.ResultList.Count;
				while (i < max)
				{
					ValueTuple<int, SensitiveWordsNode> valueTuple = this.ResultList[i];
					int checkIndex = valueTuple.Item1;
					SensitiveWordsNode checkNode = valueTuple.Item2;
					bool flag3 = index - checkIndex > maxOffset;
					if (!flag3)
					{
						SensitiveWordsNode nextNode = checkNode.GetMatchNode(key);
						bool flag4 = nextNode == null;
						if (!flag4)
						{
							bool flag5 = i < this.ResultList.Count - 1 || this.IsMatchSuccess;
							if (flag5)
							{
								SensitiveWordsMatchResult matchResult = new SensitiveWordsMatchResult();
								matchResult.ResultList = new List<ValueTuple<int, SensitiveWordsNode>>();
								for (int j = 0; j <= i; j++)
								{
									matchResult.ResultList.Add(this.ResultList[j]);
								}
								matchResult.ResultList.Add(new ValueTuple<int, SensitiveWordsNode>(index, nextNode));
								bool isEndNode = nextNode.IsEndNode;
								if (isEndNode)
								{
									matchResult.IsMatchSuccess = true;
								}
								list.Add(matchResult);
							}
							else
							{
								this.ResultList.Add(new ValueTuple<int, SensitiveWordsNode>(index, nextNode));
								bool isEndNode2 = nextNode.IsEndNode;
								if (isEndNode2)
								{
									this.IsMatchSuccess = true;
								}
							}
						}
					}
					i++;
				}
				result = list;
			}
		}
		return result;
	}

	// Token: 0x040000A2 RID: 162
	public List<ValueTuple<int, SensitiveWordsNode>> ResultList;
}
