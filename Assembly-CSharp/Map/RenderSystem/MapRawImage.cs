using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Map.RenderSystem
{
	// Token: 0x020006D1 RID: 1745
	public class MapRawImage : RawImage
	{
		// Token: 0x06005318 RID: 21272 RVA: 0x00266AE9 File Offset: 0x00264CE9
		protected override void Start()
		{
			base.Start();
			this._renderSystem = SingletonObject.getInstance<MapRenderSystem>();
		}

		// Token: 0x06005319 RID: 21273 RVA: 0x00266AFE File Offset: 0x00264CFE
		protected override void OnDestroy()
		{
			this.Clear();
			base.OnDestroy();
		}

		// Token: 0x0600531A RID: 21274 RVA: 0x00266B10 File Offset: 0x00264D10
		protected override void OnPopulateMesh(VertexHelper vh)
		{
			vh.Clear();
			bool flag = this._renderSystem != null;
			if (flag)
			{
				List<UIVertex> stream = this.VertexStream;
				bool flag2 = stream != null;
				if (flag2)
				{
					vh.AddUIVertexTriangleStream(stream);
				}
			}
		}

		// Token: 0x0600531B RID: 21275 RVA: 0x00266B4C File Offset: 0x00264D4C
		public void Clear()
		{
			bool flag = this.material != null;
			if (flag)
			{
				for (int i = 1; i <= 7; i++)
				{
					this.material.SetTexture(string.Format("_MainTex{0}", i), null);
				}
			}
			List<UIVertex> vertexStream = this.VertexStream;
			if (vertexStream != null)
			{
				vertexStream.Clear();
			}
			base.canvasRenderer.Clear();
			this.SetAllDirty();
		}

		// Token: 0x0600531C RID: 21276 RVA: 0x00266BC4 File Offset: 0x00264DC4
		public void SetTextureToIndex(sbyte textureType, Texture2D newTexture)
		{
			bool flag = this.material != null;
			if (flag)
			{
				this.material.SetTexture(string.Format("_MainTex{0}", (int)(textureType + 1)), newTexture);
			}
		}

		// Token: 0x04003845 RID: 14405
		private MapRenderSystem _renderSystem;

		// Token: 0x04003846 RID: 14406
		[NonSerialized]
		public List<UIVertex> VertexStream;
	}
}
