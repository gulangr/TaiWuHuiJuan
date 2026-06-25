using System;
using System.Collections.Generic;

namespace EventEditor
{
	// Token: 0x02000632 RID: 1586
	public class TreeColumnData
	{
		// Token: 0x0400341B RID: 13339
		public sbyte Depth;

		// Token: 0x0400341C RID: 13340
		public EventGroupData EventGroupData;

		// Token: 0x0400341D RID: 13341
		public List<ValueTuple<string, string, string>> JumpToEventList;

		// Token: 0x0400341E RID: 13342
		public List<ValueTuple<string, List<ValueTuple<string, string, string>>>> OptionOnSelectToEventList;

		// Token: 0x0400341F RID: 13343
		public string SelectingEvent;
	}
}
