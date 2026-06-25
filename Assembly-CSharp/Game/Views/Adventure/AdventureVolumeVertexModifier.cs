using System;
using UnityEngine;

namespace Game.Views.Adventure
{
	// Token: 0x02000C75 RID: 3189
	public class AdventureVolumeVertexModifier : AdventureVertexModifier
	{
		// Token: 0x170010F8 RID: 4344
		// (get) Token: 0x0600A20F RID: 41487 RVA: 0x004BBEA9 File Offset: 0x004BA0A9
		// (set) Token: 0x0600A210 RID: 41488 RVA: 0x004BBEB4 File Offset: 0x004BA0B4
		public Vector4 VolumeControl
		{
			get
			{
				return this._volumeControl;
			}
			set
			{
				bool flag = this._volumeControl != value;
				if (flag)
				{
					this._volumeControl = value;
					bool flag2 = base.graphic != null;
					if (flag2)
					{
						base.graphic.SetVerticesDirty();
					}
				}
			}
		}

		// Token: 0x170010F9 RID: 4345
		// (get) Token: 0x0600A211 RID: 41489 RVA: 0x004BBEF7 File Offset: 0x004BA0F7
		// (set) Token: 0x0600A212 RID: 41490 RVA: 0x004BBF00 File Offset: 0x004BA100
		public Vector2 OriginalUV
		{
			get
			{
				return this._originalUV;
			}
			set
			{
				bool flag = this._originalUV != value;
				if (flag)
				{
					this._originalUV = value;
					bool flag2 = base.graphic != null;
					if (flag2)
					{
						base.graphic.SetVerticesDirty();
					}
				}
			}
		}

		// Token: 0x0600A213 RID: 41491 RVA: 0x004BBF44 File Offset: 0x004BA144
		public float GetRectHeight()
		{
			return (base.graphic != null) ? base.graphic.rectTransform.rect.height : 0f;
		}

		// Token: 0x0600A214 RID: 41492 RVA: 0x004BBF84 File Offset: 0x004BA184
		protected override void ModifyVertex(ref UIVertex vertex, Rect rect, int index)
		{
			float normalizedX = (rect.width > 0f) ? ((vertex.position.x - rect.x) / rect.width) : 0.5f;
			float normalizedY = (rect.height > 0f) ? ((vertex.position.y - rect.y) / rect.height) : 0.5f;
			vertex.uv3 = new Vector4(normalizedX, normalizedY, this._volumeControl.z, 1f);
			vertex.tangent = new Vector4(this._volumeControl.x, this._volumeControl.y, 0f, 0f);
		}

		// Token: 0x04007E03 RID: 32259
		[SerializeField]
		private Vector4 _volumeControl;

		// Token: 0x04007E04 RID: 32260
		[SerializeField]
		private Vector2 _originalUV;
	}
}
