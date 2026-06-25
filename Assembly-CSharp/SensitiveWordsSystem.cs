using System;
using System.Collections.Generic;
using Config;
using FrameWork;

// Token: 0x02000022 RID: 34
public class SensitiveWordsSystem
{
	// Token: 0x17000033 RID: 51
	// (get) Token: 0x06000103 RID: 259 RVA: 0x00007A84 File Offset: 0x00005C84
	public static SensitiveWordsSystem Instance
	{
		get
		{
			bool flag = SensitiveWordsSystem._instance == null;
			if (flag)
			{
				SensitiveWordsSystem._instance = new SensitiveWordsSystem();
			}
			return SensitiveWordsSystem._instance;
		}
	}

	// Token: 0x06000104 RID: 260 RVA: 0x00007AB1 File Offset: 0x00005CB1
	public void Init()
	{
		this._sensitiveWordsMap = new Dictionary<char, SensitiveWordsNode>();
		SensitiveWords.Instance.Iterate(delegate(SensitiveWordsItem e)
		{
			this.AddWord(e);
			return true;
		});
	}

	// Token: 0x06000105 RID: 261 RVA: 0x00007AD8 File Offset: 0x00005CD8
	public bool HasAnySensitiveWords(string text, byte maxMatchLength = 10)
	{
		bool flag6 = string.IsNullOrEmpty(text);
		bool result;
		if (flag6)
		{
			result = false;
		}
		else
		{
			List<SensitiveWordsMatchResult> checkList = EasyPool.Get<List<SensitiveWordsMatchResult>>();
			List<SensitiveWordsMatchResult> cacheList = EasyPool.Get<List<SensitiveWordsMatchResult>>();
			bool flag = false;
			int i = 0;
			int max = text.Length;
			Action<SensitiveWordsMatchResult> <>9__0;
			while (i < max)
			{
				char nodeKey = text[i];
				cacheList.Clear();
				int j = 0;
				int jMax = checkList.Count;
				while (j < jMax)
				{
					List<SensitiveWordsMatchResult> resultList = checkList[j].CheckMatch(nodeKey, i, (int)maxMatchLength);
					bool flag2 = resultList == null;
					if (!flag2)
					{
						List<SensitiveWordsMatchResult> list = resultList;
						Action<SensitiveWordsMatchResult> action;
						if ((action = <>9__0) == null)
						{
							action = (<>9__0 = delegate(SensitiveWordsMatchResult e)
							{
								bool isMatchSuccess = e.IsMatchSuccess;
								if (isMatchSuccess)
								{
									flag = true;
								}
								cacheList.Add(e);
							});
						}
						list.ForEach(action);
						EasyPool.Free<List<SensitiveWordsMatchResult>>(resultList);
						bool flag3 = flag;
						if (flag3)
						{
							break;
						}
					}
					j++;
				}
				bool flag4 = flag;
				if (flag4)
				{
					break;
				}
				bool flag5 = this._sensitiveWordsMap.ContainsKey(nodeKey);
				if (flag5)
				{
					SensitiveWordsMatchResult item = new SensitiveWordsMatchResult();
					item.ResultList = new List<ValueTuple<int, SensitiveWordsNode>>();
					item.ResultList.Add(new ValueTuple<int, SensitiveWordsNode>(i, this._sensitiveWordsMap[nodeKey]));
					cacheList.Add(item);
				}
				List<SensitiveWordsMatchResult> cacheList3 = checkList;
				List<SensitiveWordsMatchResult> cacheList2 = cacheList;
				cacheList = cacheList3;
				checkList = cacheList2;
				i++;
			}
			EasyPool.Free<List<SensitiveWordsMatchResult>>(checkList);
			EasyPool.Free<List<SensitiveWordsMatchResult>>(cacheList);
			result = flag;
		}
		return result;
	}

	// Token: 0x06000106 RID: 262 RVA: 0x00007C68 File Offset: 0x00005E68
	public List<SensitiveWordsMatchResult> TryMatch(string text, byte maxMatchLength = 10)
	{
		bool flag = string.IsNullOrEmpty(text);
		List<SensitiveWordsMatchResult> result;
		if (flag)
		{
			result = null;
		}
		else
		{
			Dictionary<int, SensitiveWordsMatchResult> matchResults = new Dictionary<int, SensitiveWordsMatchResult>();
			List<SensitiveWordsMatchResult> checkList = EasyPool.Get<List<SensitiveWordsMatchResult>>();
			List<SensitiveWordsMatchResult> cacheList = EasyPool.Get<List<SensitiveWordsMatchResult>>();
			int i = 0;
			int max = text.Length;
			Action<SensitiveWordsMatchResult> <>9__0;
			while (i < max)
			{
				char nodeKey = text[i];
				cacheList.Clear();
				int j = 0;
				int jMax = checkList.Count;
				while (j < jMax)
				{
					List<SensitiveWordsMatchResult> resultList = checkList[j].CheckMatch(nodeKey, i, (int)maxMatchLength);
					bool flag2 = resultList == null;
					if (!flag2)
					{
						List<SensitiveWordsMatchResult> list = resultList;
						Action<SensitiveWordsMatchResult> action;
						if ((action = <>9__0) == null)
						{
							action = (<>9__0 = delegate(SensitiveWordsMatchResult e)
							{
								int hashCode = e.GetHashCode();
								bool flag4 = e.IsMatchSuccess && !matchResults.ContainsKey(hashCode);
								if (flag4)
								{
									matchResults.Add(hashCode, e);
								}
								cacheList.Add(e);
							});
						}
						list.ForEach(action);
						EasyPool.Free<List<SensitiveWordsMatchResult>>(resultList);
					}
					j++;
				}
				bool flag3 = this._sensitiveWordsMap.ContainsKey(nodeKey);
				if (flag3)
				{
					SensitiveWordsMatchResult item = new SensitiveWordsMatchResult();
					item.ResultList = new List<ValueTuple<int, SensitiveWordsNode>>();
					item.ResultList.Add(new ValueTuple<int, SensitiveWordsNode>(i, this._sensitiveWordsMap[nodeKey]));
					cacheList.Add(item);
				}
				List<SensitiveWordsMatchResult> cacheList3 = checkList;
				List<SensitiveWordsMatchResult> cacheList2 = cacheList;
				cacheList = cacheList3;
				checkList = cacheList2;
				i++;
			}
			EasyPool.Free<List<SensitiveWordsMatchResult>>(checkList);
			EasyPool.Free<List<SensitiveWordsMatchResult>>(cacheList);
			result = new List<SensitiveWordsMatchResult>(matchResults.Values);
		}
		return result;
	}

	// Token: 0x06000107 RID: 263 RVA: 0x00007DEC File Offset: 0x00005FEC
	public string GetLegalResult(string text, char replaceCharacter = '*', byte maxMatchLength = 10)
	{
		bool flag = string.IsNullOrEmpty(text);
		string result2;
		if (flag)
		{
			result2 = text;
		}
		else
		{
			List<SensitiveWordsMatchResult> resultList = this.TryMatch(text, maxMatchLength);
			bool flag2 = resultList == null;
			if (flag2)
			{
				result2 = text;
			}
			else
			{
				char[] charArray = text.ToCharArray();
				int i = 0;
				int max = resultList.Count;
				while (i < max)
				{
					SensitiveWordsMatchResult result = resultList[i];
					int j = 0;
					int jMax = result.ResultList.Count;
					while (j < jMax)
					{
						ValueTuple<int, SensitiveWordsNode> tuple = result.ResultList[j];
						charArray[tuple.Item1] = replaceCharacter;
						j++;
					}
					i++;
				}
				result2 = new string(charArray);
			}
		}
		return result2;
	}

	// Token: 0x06000108 RID: 264 RVA: 0x00007EA0 File Offset: 0x000060A0
	private void AddWord(SensitiveWordsItem item)
	{
		string word = item.Content;
		bool flag = string.IsNullOrEmpty(word);
		if (!flag)
		{
			word = word.Trim().Replace("\r", "");
			int index = 0;
			SensitiveWordsNode curNode;
			bool flag2 = !this._sensitiveWordsMap.TryGetValue(word[index], out curNode);
			if (flag2)
			{
				curNode = new SensitiveWordsNode(word[index]);
				this._sensitiveWordsMap.Add(word[index], curNode);
			}
			int max = word.Length;
			while (curNode != null)
			{
				index++;
				bool flag3 = index >= max;
				if (flag3)
				{
					break;
				}
				curNode = curNode.AddChildChar(word[index]);
				bool flag4 = index == max - 1;
				if (flag4)
				{
					curNode.SetIsEndNode(true);
					curNode.Config = item;
				}
			}
		}
	}

	// Token: 0x040000A8 RID: 168
	public static float SensitiveWordAnimationStayTime = 3f;

	// Token: 0x040000A9 RID: 169
	public static float SensitiveWordAnimationFadeTime = 0.5f;

	// Token: 0x040000AA RID: 170
	private static SensitiveWordsSystem _instance;

	// Token: 0x040000AB RID: 171
	private Dictionary<char, SensitiveWordsNode> _sensitiveWordsMap;
}
