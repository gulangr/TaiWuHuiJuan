using System;
using Unity.Burst;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Jobs;

namespace FrameWork.Tools.OffscreenCulling
{
	// Token: 0x02001037 RID: 4151
	[BurstCompile]
	public struct OffscreenCullJob : IJobParallelForTransform
	{
		// Token: 0x0600BDAC RID: 48556 RVA: 0x00561D1C File Offset: 0x0055FF1C
		public void Execute(int index, TransformAccess transform)
		{
			Matrix4x4 mvp = this.VP * transform.localToWorldMatrix;
			float minX = float.MaxValue;
			float minY = float.MaxValue;
			float maxX = float.MinValue;
			float maxY = float.MinValue;
			bool anyBehind = false;
			int baseI = index * 4;
			for (int i = 0; i < 4; i++)
			{
				Vector3 lc = this.LocalCorners[baseI + i];
				Vector4 clip = mvp * new Vector4(lc.x, lc.y, lc.z, 1f);
				float w = clip.w;
				bool flag = w <= 0.0001f;
				if (flag)
				{
					anyBehind = true;
					w = 0.0001f;
				}
				float ndcX = clip.x / w;
				float ndcY = clip.y / w;
				float sx = (ndcX * 0.5f + 0.5f) * this.ScreenWidth;
				float sy = (ndcY * 0.5f + 0.5f) * this.ScreenHeight;
				bool flag2 = sx < minX;
				if (flag2)
				{
					minX = sx;
				}
				bool flag3 = sx > maxX;
				if (flag3)
				{
					maxX = sx;
				}
				bool flag4 = sy < minY;
				if (flag4)
				{
					minY = sy;
				}
				bool flag5 = sy > maxY;
				if (flag5)
				{
					maxY = sy;
				}
			}
			float p = this.Paddings[index];
			bool outside = !anyBehind && (maxX < -p || minX > this.ScreenWidth + p || maxY < -p || minY > this.ScreenHeight + p);
			this.Results[index] = !outside;
		}

		// Token: 0x040091E8 RID: 37352
		[global::ReadOnly]
		[NativeDisableParallelForRestriction]
		public NativeArray<Vector3> LocalCorners;

		// Token: 0x040091E9 RID: 37353
		[global::ReadOnly]
		[NativeDisableParallelForRestriction]
		public NativeArray<float> Paddings;

		// Token: 0x040091EA RID: 37354
		public Matrix4x4 VP;

		// Token: 0x040091EB RID: 37355
		public float ScreenWidth;

		// Token: 0x040091EC RID: 37356
		public float ScreenHeight;

		// Token: 0x040091ED RID: 37357
		[WriteOnly]
		public NativeArray<bool> Results;
	}
}
