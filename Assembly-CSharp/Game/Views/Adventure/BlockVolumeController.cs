using System;
using GameData.Adventure;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.Adventure
{
	// Token: 0x02000C76 RID: 3190
	public class BlockVolumeController : MonoBehaviour
	{
		// Token: 0x170010FA RID: 4346
		// (get) Token: 0x0600A216 RID: 41494 RVA: 0x004BC045 File Offset: 0x004BA245
		// (set) Token: 0x0600A217 RID: 41495 RVA: 0x004BC050 File Offset: 0x004BA250
		public float VolumeHeight
		{
			get
			{
				return this._volumeHeight;
			}
			set
			{
				this._volumeHeight = value;
				int i = 0;
				RectTransform[] groundAnchors = this._groundAnchors;
				int len = (groundAnchors != null) ? groundAnchors.Length : 0;
				while (i < len)
				{
					RectTransform anchor = this._groundAnchors[i];
					Vector3 p = anchor.localPosition;
					p.y = this._volumeHeight * this._volumeRealFullHeight;
					anchor.localPosition = p;
					i++;
				}
				float e = this._volumeHeight;
				int j = 0;
				AdventureVolumeVertexModifier[] volumeImages = this._volumeImages;
				int len2 = (volumeImages != null) ? volumeImages.Length : 0;
				while (j < len2)
				{
					AdventureVolumeVertexModifier modifier = this._volumeImages[j];
					bool flag = modifier == null || modifier.TargetGraphic == null;
					if (!flag)
					{
						float rectHeight = modifier.GetRectHeight();
						Vector4 volControl = modifier.VolumeControl;
						volControl.x = ((rectHeight > 0f) ? ((rectHeight - this._volumeRealFullHeight) / rectHeight) : 0f);
						volControl.y = e;
						volControl.w = 1f;
						modifier.VolumeControl = volControl;
					}
					j++;
				}
			}
		}

		// Token: 0x0600A218 RID: 41496 RVA: 0x004BC170 File Offset: 0x004BA370
		public void SetGridIndex(AdventureBlockIndex index)
		{
			bool flag = this._volumeImages == null;
			if (!flag)
			{
				foreach (AdventureVolumeVertexModifier mod in this._volumeImages)
				{
					bool flag2 = mod != null;
					if (flag2)
					{
						mod.GridIndex = index;
					}
				}
			}
		}

		// Token: 0x170010FB RID: 4347
		// (get) Token: 0x0600A219 RID: 41497 RVA: 0x004BC1BE File Offset: 0x004BA3BE
		// (set) Token: 0x0600A21A RID: 41498 RVA: 0x004BC1E0 File Offset: 0x004BA3E0
		public bool RaycastTargetEnable
		{
			get
			{
				return this._raycastTargets.CheckIndex(0) && this._raycastTargets[0].raycastTarget;
			}
			set
			{
				bool flag = this._raycastTargets == null;
				if (!flag)
				{
					foreach (Graphic graphic in this._raycastTargets)
					{
						graphic.raycastTarget = value;
					}
				}
			}
		}

		// Token: 0x170010FC RID: 4348
		// (get) Token: 0x0600A21B RID: 41499 RVA: 0x004BC221 File Offset: 0x004BA421
		// (set) Token: 0x0600A21C RID: 41500 RVA: 0x004BC229 File Offset: 0x004BA429
		public float VolumeRealFullHeight
		{
			get
			{
				return this._volumeRealFullHeight;
			}
			set
			{
				this._volumeRealFullHeight = value;
			}
		}

		// Token: 0x0600A21D RID: 41501 RVA: 0x004BC234 File Offset: 0x004BA434
		public float GetVolumeHeightOffsetY()
		{
			return this._volumeHeight * this._volumeRealFullHeight;
		}

		// Token: 0x0600A21E RID: 41502 RVA: 0x004BC254 File Offset: 0x004BA454
		public void SetVolumeBrightness(float brightness)
		{
			foreach (AdventureVolumeVertexModifier modifier in this._volumeImages)
			{
				bool flag = modifier != null;
				if (flag)
				{
					modifier.Brightness = brightness;
				}
			}
		}

		// Token: 0x0600A21F RID: 41503 RVA: 0x004BC294 File Offset: 0x004BA494
		public void SetVolumeSaturation(float saturation)
		{
			foreach (AdventureVolumeVertexModifier modifier in this._volumeImages)
			{
				bool flag = modifier != null;
				if (flag)
				{
					modifier.Saturation = saturation;
				}
			}
		}

		// Token: 0x04007E05 RID: 32261
		[SerializeField]
		private RectTransform[] _groundAnchors;

		// Token: 0x04007E06 RID: 32262
		[SerializeField]
		private Graphic[] _raycastTargets;

		// Token: 0x04007E07 RID: 32263
		[SerializeField]
		private AdventureVolumeVertexModifier[] _volumeImages;

		// Token: 0x04007E08 RID: 32264
		[SerializeField]
		private float _volumeRealFullHeight;

		// Token: 0x04007E09 RID: 32265
		[Range(0f, 1f)]
		[SerializeField]
		private float _volumeHeight = 1f;
	}
}
