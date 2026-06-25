using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace AiEditor
{
	// Token: 0x02000673 RID: 1651
	public class AiEditorCommandRecord
	{
		// Token: 0x06004DEB RID: 19947 RVA: 0x0024B6A8 File Offset: 0x002498A8
		private void Record(EAiEditorCommandType type, int runtimeId, string data)
		{
			this._revertedData.Clear();
			this._data.Push(new AiEditorCommandData
			{
				Type = type,
				RuntimeId = runtimeId,
				Data = data
			});
		}

		// Token: 0x06004DEC RID: 19948 RVA: 0x0024B6F0 File Offset: 0x002498F0
		public void RecordAddNode(int runtimeId, AiNodeDataSnapshot nodeData)
		{
			string data = JsonConvert.SerializeObject(new AiEditorAddNodeData
			{
				NodeData = nodeData
			});
			this.Record(EAiEditorCommandType.AddNode, runtimeId, data);
		}

		// Token: 0x06004DED RID: 19949 RVA: 0x0024B71C File Offset: 0x0024991C
		public void RecordDeleteNode(int runtimeId, AiBlueprintSnapshot prevData)
		{
			string data = JsonConvert.SerializeObject(new AiEditorDeleteNodeData
			{
				PrevData = prevData
			});
			this.Record(EAiEditorCommandType.DeleteNode, runtimeId, data);
		}

		// Token: 0x06004DEE RID: 19950 RVA: 0x0024B748 File Offset: 0x00249948
		public void RecordActionNode(int runtimeId, AiNodeDataSnapshot prevData, AiNodeDataSnapshot currData)
		{
			bool flag = prevData.DataEquals(currData);
			if (!flag)
			{
				string data = JsonConvert.SerializeObject(new AiEditorActionNodeData
				{
					PrevData = prevData,
					CurrData = currData
				});
				this.Record(EAiEditorCommandType.ActionNode, runtimeId, data);
			}
		}

		// Token: 0x06004DEF RID: 19951 RVA: 0x0024B788 File Offset: 0x00249988
		public void RecordMoveNode(int runtimeId, Vector2 prevPosition, Vector2 currPosition)
		{
			bool flag = prevPosition == currPosition;
			if (!flag)
			{
				string data = JsonConvert.SerializeObject(new AiEditorMoveNodeData
				{
					PrevPosition = prevPosition,
					CurrPosition = currPosition
				});
				this.Record(EAiEditorCommandType.MoveNode, runtimeId, data);
			}
		}

		// Token: 0x06004DF0 RID: 19952 RVA: 0x0024B7C8 File Offset: 0x002499C8
		public AiEditorCommandData? Revert()
		{
			bool flag = this._data.Count == 0;
			AiEditorCommandData? result;
			if (flag)
			{
				result = null;
			}
			else
			{
				AiEditorCommandData data = this._data.Pop();
				this._revertedData.Push(data);
				result = new AiEditorCommandData?(data);
			}
			return result;
		}

		// Token: 0x06004DF1 RID: 19953 RVA: 0x0024B818 File Offset: 0x00249A18
		public AiEditorCommandData? Evolve()
		{
			bool flag = this._revertedData.Count == 0;
			AiEditorCommandData? result;
			if (flag)
			{
				result = null;
			}
			else
			{
				AiEditorCommandData data = this._revertedData.Pop();
				this._data.Push(data);
				result = new AiEditorCommandData?(data);
			}
			return result;
		}

		// Token: 0x06004DF2 RID: 19954 RVA: 0x0024B867 File Offset: 0x00249A67
		public void Clear()
		{
			this._data.Clear();
			this._revertedData.Clear();
		}

		// Token: 0x04003608 RID: 13832
		private readonly Stack<AiEditorCommandData> _data = new Stack<AiEditorCommandData>();

		// Token: 0x04003609 RID: 13833
		private readonly Stack<AiEditorCommandData> _revertedData = new Stack<AiEditorCommandData>();
	}
}
