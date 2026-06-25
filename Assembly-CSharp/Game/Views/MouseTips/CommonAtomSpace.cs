using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.MouseTips
{
	// Token: 0x02000823 RID: 2083
	public class CommonAtomSpace : CommonAtomBase
	{
		// Token: 0x06006646 RID: 26182 RVA: 0x002EB4D5 File Offset: 0x002E96D5
		public override void SetMarginLeft(int marginLeft)
		{
		}

		// Token: 0x06006647 RID: 26183 RVA: 0x002EB4D8 File Offset: 0x002E96D8
		public void SetLevel(int level)
		{
			bool flag = level < 0 || level >= CommonAtomSpace.LevelToHeight.Length;
			if (flag)
			{
				level = 1;
			}
			int height = CommonAtomSpace.LevelToHeight[level];
			this.layoutElement.preferredHeight = (float)height;
		}

		// Token: 0x0400478C RID: 18316
		[SerializeField]
		private LayoutElement layoutElement;

		// Token: 0x0400478D RID: 18317
		private static readonly int[] LevelToHeight = new int[]
		{
			0,
			12,
			20
		};
	}
}
