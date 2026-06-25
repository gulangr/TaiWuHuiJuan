using System;
using UnityEngine;

namespace Game.Views.NewGame
{
	// Token: 0x020007F4 RID: 2036
	[Serializable]
	public class FacePartAvatarPreviewSetting
	{
		// Token: 0x06006357 RID: 25431 RVA: 0x002D7C78 File Offset: 0x002D5E78
		public Vector2 GetOffset(sbyte bodyType)
		{
			if (!true)
			{
			}
			Vector2 result;
			switch (bodyType)
			{
			case 0:
				result = this.thinOffset;
				break;
			case 1:
				result = this.normalOffset;
				break;
			case 2:
				result = this.fatOffset;
				break;
			default:
				result = this.normalOffset;
				break;
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x06006358 RID: 25432 RVA: 0x002D7CCC File Offset: 0x002D5ECC
		public float GetScale(sbyte bodyType)
		{
			if (!true)
			{
			}
			float result;
			switch (bodyType)
			{
			case 0:
				result = this.thinScale;
				break;
			case 1:
				result = this.normalScale;
				break;
			case 2:
				result = this.fatScale;
				break;
			default:
				result = this.normalScale;
				break;
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x04004548 RID: 17736
		[Tooltip("瘦体型偏移")]
		public Vector2 thinOffset;

		// Token: 0x04004549 RID: 17737
		[Tooltip("标准体型偏移")]
		public Vector2 normalOffset;

		// Token: 0x0400454A RID: 17738
		[Tooltip("胖体型偏移")]
		public Vector2 fatOffset;

		// Token: 0x0400454B RID: 17739
		[Tooltip("瘦体型缩放")]
		public float thinScale = 1f;

		// Token: 0x0400454C RID: 17740
		[Tooltip("标准体型缩放")]
		public float normalScale = 1f;

		// Token: 0x0400454D RID: 17741
		[Tooltip("胖体型缩放")]
		public float fatScale = 1f;
	}
}
