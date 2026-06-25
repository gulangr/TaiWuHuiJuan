using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.Adventure
{
	// Token: 0x02000C6B RID: 3179
	[RequireComponent(typeof(Graphic))]
	public class AdventureOutsideBlockVertexModifier : BaseMeshEffect
	{
		// Token: 0x170010F0 RID: 4336
		// (get) Token: 0x0600A1DE RID: 41438 RVA: 0x004BAEB5 File Offset: 0x004B90B5
		// (set) Token: 0x0600A1DF RID: 41439 RVA: 0x004BAEC0 File Offset: 0x004B90C0
		public float Brightness
		{
			get
			{
				return this.brightness;
			}
			set
			{
				bool flag = !Mathf.Approximately(this.brightness, value);
				if (flag)
				{
					this.brightness = value;
					bool flag2 = base.graphic != null;
					if (flag2)
					{
						base.graphic.SetVerticesDirty();
					}
				}
			}
		}

		// Token: 0x170010F1 RID: 4337
		// (get) Token: 0x0600A1E0 RID: 41440 RVA: 0x004BAF06 File Offset: 0x004B9106
		// (set) Token: 0x0600A1E1 RID: 41441 RVA: 0x004BAF10 File Offset: 0x004B9110
		public float Saturation
		{
			get
			{
				return this.saturation;
			}
			set
			{
				bool flag = !Mathf.Approximately(this.saturation, value);
				if (flag)
				{
					this.saturation = value;
					bool flag2 = base.graphic != null;
					if (flag2)
					{
						base.graphic.SetVerticesDirty();
					}
				}
			}
		}

		// Token: 0x0600A1E2 RID: 41442 RVA: 0x004BAF58 File Offset: 0x004B9158
		public override void ModifyMesh(VertexHelper vh)
		{
			bool flag = !this.IsActive() || base.graphic == null;
			if (!flag)
			{
				UIVertex vert = default(UIVertex);
				int count = vh.currentVertCount;
				for (int i = 0; i < count; i++)
				{
					vh.PopulateUIVertex(ref vert, i);
					vert.uv1 = new Vector4(0f, 0f, this.brightness, this.saturation);
					vh.SetUIVertex(vert, i);
				}
			}
		}

		// Token: 0x04007DC8 RID: 32200
		[SerializeField]
		protected float brightness = 1f;

		// Token: 0x04007DC9 RID: 32201
		[SerializeField]
		protected float saturation = 1f;
	}
}
