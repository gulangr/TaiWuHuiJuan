using System;
using UnityEngine;

namespace Game.Views.Adventure
{
	// Token: 0x02000C70 RID: 3184
	public class AdventureTaiwuStateHolder : MonoBehaviour
	{
		// Token: 0x0600A1F4 RID: 41460 RVA: 0x004BB6B8 File Offset: 0x004B98B8
		public void SetHeight(int count)
		{
			int lineCount = AdventureTaiwuStateHolder.GetLineCount(count);
			bool flag = lineCount >= 5;
			if (flag)
			{
				lineCount = 5;
			}
			this.self.SetHeight((float)lineCount * 30f);
		}

		// Token: 0x0600A1F5 RID: 41461 RVA: 0x004BB6F0 File Offset: 0x004B98F0
		private static int GetLineCount(int count)
		{
			return count / 5 + ((count % 5 == 0) ? 0 : 1);
		}

		// Token: 0x04007DEE RID: 32238
		[SerializeField]
		private RectTransform self;

		// Token: 0x04007DEF RID: 32239
		private const float _lineHeight = 30f;

		// Token: 0x04007DF0 RID: 32240
		public const int LineCount = 5;
	}
}
