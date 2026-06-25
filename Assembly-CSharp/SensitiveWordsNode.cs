using System;
using System.Collections.Generic;
using Config;

// Token: 0x02000021 RID: 33
public class SensitiveWordsNode
{
	// Token: 0x17000032 RID: 50
	// (get) Token: 0x060000FD RID: 253 RVA: 0x000079C0 File Offset: 0x00005BC0
	// (set) Token: 0x060000FE RID: 254 RVA: 0x000079C8 File Offset: 0x00005BC8
	public bool IsEndNode { get; private set; }

	// Token: 0x060000FF RID: 255 RVA: 0x000079D1 File Offset: 0x00005BD1
	public SensitiveWordsNode(char character)
	{
		this.NodeKey = character;
	}

	// Token: 0x06000100 RID: 256 RVA: 0x000079E4 File Offset: 0x00005BE4
	public SensitiveWordsNode GetMatchNode(char testChar)
	{
		bool flag = this._childNodesMap == null;
		SensitiveWordsNode result;
		if (flag)
		{
			result = null;
		}
		else
		{
			SensitiveWordsNode node;
			this._childNodesMap.TryGetValue(testChar, out node);
			result = node;
		}
		return result;
	}

	// Token: 0x06000101 RID: 257 RVA: 0x00007A18 File Offset: 0x00005C18
	public SensitiveWordsNode AddChildChar(char nextChar)
	{
		bool flag = this._childNodesMap == null;
		if (flag)
		{
			this._childNodesMap = new Dictionary<char, SensitiveWordsNode>();
		}
		bool flag2 = this._childNodesMap.ContainsKey(nextChar);
		SensitiveWordsNode result;
		if (flag2)
		{
			result = this._childNodesMap[nextChar];
		}
		else
		{
			SensitiveWordsNode childNode = new SensitiveWordsNode(nextChar);
			this._childNodesMap.Add(nextChar, childNode);
			result = childNode;
		}
		return result;
	}

	// Token: 0x06000102 RID: 258 RVA: 0x00007A77 File Offset: 0x00005C77
	public void SetIsEndNode(bool isEnd)
	{
		this.IsEndNode = isEnd;
	}

	// Token: 0x040000A4 RID: 164
	public readonly char NodeKey;

	// Token: 0x040000A6 RID: 166
	public SensitiveWordsItem Config;

	// Token: 0x040000A7 RID: 167
	private Dictionary<char, SensitiveWordsNode> _childNodesMap;
}
