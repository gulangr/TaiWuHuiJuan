using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace FrameWork.Tools.EnhancedRichText
{
	// Token: 0x0200103E RID: 4158
	public class TagInfo
	{
		// Token: 0x0600BDCC RID: 48588 RVA: 0x00562D54 File Offset: 0x00560F54
		public void FindNextTagInText(int curIndex, string enhancedText)
		{
			this.Properties.Clear();
			this.Tag = string.Empty;
			this.Value = string.Empty;
			this.Length = 0;
			int nextStartTagIndex = enhancedText.IndexOf('<', curIndex);
			bool flag = nextStartTagIndex < 0;
			if (flag)
			{
				this.Index = -1;
			}
			else
			{
				int nextEndTagIndex = enhancedText.IndexOf('>', nextStartTagIndex);
				bool flag2 = nextEndTagIndex < 0;
				if (flag2)
				{
					this.Index = -1;
				}
				else
				{
					this.Length = nextEndTagIndex - nextStartTagIndex + 1;
					string[] propertiesArr = enhancedText.Substring(nextStartTagIndex + 1, this.Length - 2).Split(Array.Empty<char>());
					ValueTuple<string, string> valueTuple = this.ParsePropertyPair(propertiesArr[0]);
					this.Tag = valueTuple.Item1;
					this.Value = valueTuple.Item2;
					for (int i = 1; i < propertiesArr.Length; i++)
					{
						ValueTuple<string, string> valueTuple2 = this.ParsePropertyPair(propertiesArr[i]);
						string property = valueTuple2.Item1;
						string val = valueTuple2.Item2;
						this.Properties.Add(property, val);
					}
					this.Index = nextStartTagIndex;
				}
			}
		}

		// Token: 0x0600BDCD RID: 48589 RVA: 0x00562E64 File Offset: 0x00561064
		[return: TupleElementNames(new string[]
		{
			"key",
			"val"
		})]
		private ValueTuple<string, string> ParsePropertyPair(string str)
		{
			int nextEqIndex = str.IndexOf('=');
			return (nextEqIndex == -1) ? new ValueTuple<string, string>(str, string.Empty) : new ValueTuple<string, string>(str.Substring(0, nextEqIndex), str.Substring(nextEqIndex + 1, str.Length - nextEqIndex - 1));
		}

		// Token: 0x04009206 RID: 37382
		public int Index;

		// Token: 0x04009207 RID: 37383
		public int Length;

		// Token: 0x04009208 RID: 37384
		public string Tag;

		// Token: 0x04009209 RID: 37385
		public string Value;

		// Token: 0x0400920A RID: 37386
		public readonly Dictionary<string, string> Properties = new Dictionary<string, string>();
	}
}
