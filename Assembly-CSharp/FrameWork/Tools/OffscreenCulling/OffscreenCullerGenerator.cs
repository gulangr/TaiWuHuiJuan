using System;
using UnityEngine;

namespace FrameWork.Tools.OffscreenCulling
{
	// Token: 0x02001036 RID: 4150
	public class OffscreenCullerGenerator : MonoBehaviour
	{
		// Token: 0x040091E5 RID: 37349
		[Tooltip("是否给父物体自身也添加 OffscreenCuller（若父物体本身带画面组件）。")]
		[SerializeField]
		private bool includeSelf = false;

		// Token: 0x040091E6 RID: 37350
		[Tooltip("新生成的 OffscreenCuller 的【显现→隐藏】检测间隔帧数。")]
		[SerializeField]
		private int hideCheckIntervalFrames = 10;

		// Token: 0x040091E7 RID: 37351
		[Tooltip("新生成的 OffscreenCuller 的【隐藏→显现】检测间隔帧数（通常更小，回屏更跟手）。")]
		[SerializeField]
		private int showCheckIntervalFrames = 3;
	}
}
