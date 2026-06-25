using System;
using UnityEngine;

namespace FrameWork.UISystem.Components
{
	// Token: 0x0200102A RID: 4138
	public class UIRectSizeControllerMaxHeight : UIRectSizeController
	{
		// Token: 0x0600BD40 RID: 48448 RVA: 0x0055FC20 File Offset: 0x0055DE20
		protected override Vector2 SizeCheck(Vector2 newSize)
		{
			bool flag = newSize.y > this.maxHeight;
			if (flag)
			{
				newSize.y = this.maxHeight;
			}
			return newSize;
		}

		// Token: 0x040091A9 RID: 37289
		[SerializeField]
		private float maxHeight;
	}
}
