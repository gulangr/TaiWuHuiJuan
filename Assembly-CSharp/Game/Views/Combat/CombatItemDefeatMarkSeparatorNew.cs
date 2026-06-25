using System;
using UnityEngine;

namespace Game.Views.Combat
{
	// Token: 0x02000B30 RID: 2864
	public class CombatItemDefeatMarkSeparatorNew : MonoBehaviour
	{
		// Token: 0x06008C41 RID: 35905 RVA: 0x0040CAF4 File Offset: 0x0040ACF4
		public void Set(RectTransform parentMark)
		{
			RectTransform rect = (RectTransform)base.transform;
			bool flag = rect.parent != parentMark;
			if (flag)
			{
				rect.SetParent(parentMark, false);
			}
			rect.anchoredPosition = Vector2.zero;
			base.gameObject.SetActive(true);
		}
	}
}
