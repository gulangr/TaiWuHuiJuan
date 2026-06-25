using System;
using FrameWork.Tools.JsonConverter;
using Newtonsoft.Json;
using UnityEngine;

namespace AiEditor
{
	// Token: 0x02000671 RID: 1649
	public class AiEditorMoveNodeData
	{
		// Token: 0x04003603 RID: 13827
		[JsonConverter(typeof(JsonConverterVector2))]
		public Vector2 PrevPosition;

		// Token: 0x04003604 RID: 13828
		[JsonConverter(typeof(JsonConverterVector2))]
		public Vector2 CurrPosition;
	}
}
