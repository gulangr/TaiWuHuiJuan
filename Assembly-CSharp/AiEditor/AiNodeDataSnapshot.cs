using System;
using System.Collections.Generic;
using System.Linq;
using FrameWork.Tools.JsonConverter;
using Newtonsoft.Json;
using UnityEngine;

namespace AiEditor
{
	// Token: 0x0200067B RID: 1659
	public class AiNodeDataSnapshot
	{
		// Token: 0x06004E6B RID: 20075 RVA: 0x0024D774 File Offset: 0x0024B974
		public void ParseMapping(ref int conditionCount, ref int actionCount)
		{
			int type = this.Type;
			int num = type;
			if (num != 1)
			{
				if (num == 2)
				{
					for (int i = 0; i < this.Ids.Count; i++)
					{
						this.Ids[i] = actionCount + i;
					}
					actionCount += this.SubTypes.Count;
				}
			}
			else
			{
				for (int j = 0; j < this.Ids.Count; j += 3)
				{
					this.Ids[j] = conditionCount + j / 3;
				}
				conditionCount += this.SubTypes.Count;
			}
		}

		// Token: 0x06004E6C RID: 20076 RVA: 0x0024D820 File Offset: 0x0024BA20
		public bool DataEquals(AiNodeDataSnapshot data)
		{
			return this.Position.Equals(data.Position) && this.RuntimeId.Equals(data.RuntimeId) && this.Type.Equals(data.Type) && this.Ids.SequenceEqual(data.Ids) && this.SubTypes.SequenceEqual(data.SubTypes) && this.Params.SequenceEqual(data.Params);
		}

		// Token: 0x04003636 RID: 13878
		[JsonConverter(typeof(JsonConverterVector2))]
		public Vector2 Position;

		// Token: 0x04003637 RID: 13879
		public int RuntimeId;

		// Token: 0x04003638 RID: 13880
		public int Type;

		// Token: 0x04003639 RID: 13881
		public List<int> Ids = new List<int>();

		// Token: 0x0400363A RID: 13882
		public List<int> SubTypes = new List<int>();

		// Token: 0x0400363B RID: 13883
		public List<List<string>> Params = new List<List<string>>();
	}
}
