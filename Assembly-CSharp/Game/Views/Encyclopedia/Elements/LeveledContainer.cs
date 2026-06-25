using System;
using FrameWork.UISystem.UIElements;
using UnityEngine;

namespace Game.Views.Encyclopedia.Elements
{
	// Token: 0x02000A89 RID: 2697
	public class LeveledContainer : MonoBehaviour
	{
		// Token: 0x0400652B RID: 25899
		[SerializeField]
		internal RectTransform self;

		// Token: 0x0400652C RID: 25900
		[SerializeField]
		internal RectTransform low;

		// Token: 0x0400652D RID: 25901
		[SerializeField]
		internal RectTransform mid;

		// Token: 0x0400652E RID: 25902
		[SerializeField]
		internal RectTransform high;

		// Token: 0x0400652F RID: 25903
		[SerializeField]
		internal RectTransform bottom;

		// Token: 0x04006530 RID: 25904
		internal CScrollRect scrollContainer;
	}
}
