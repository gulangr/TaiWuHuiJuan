using System;
using UnityEngine;

namespace Game.Views.Adventure
{
	// Token: 0x02000C66 RID: 3174
	public class AdventureEntranceLightingEffect : MonoBehaviour
	{
		// Token: 0x0600A198 RID: 41368 RVA: 0x004B850C File Offset: 0x004B670C
		public void CaptureTargetAzimuth()
		{
			bool flag = !AdventureLightingManager.HasActiveLighting();
			if (flag)
			{
				this._isInitialized = false;
			}
			else
			{
				bool flag2 = AdventureLightingManager.Instance != null;
				if (flag2)
				{
					this._targetAzimuth = AdventureLightingManager.Instance.GlobalAzimuthAngle;
					this._targetIncidence = AdventureLightingManager.Instance.GlobalIncidenceAngle;
					this._targetColor = AdventureLightingManager.Instance.GlobalColor;
					Color.RGBToHSV(this._targetColor, out this._targetH, out this._targetS, out this._targetV);
					this._isInitialized = true;
				}
			}
		}

		// Token: 0x0600A199 RID: 41369 RVA: 0x004B8598 File Offset: 0x004B6798
		public void UpdateLightingOffset(float progress)
		{
			bool flag = !this._isInitialized || !AdventureLightingManager.HasActiveLighting();
			if (!flag)
			{
				float currentOffset = Mathf.Lerp(this.StartOffset, 0f, progress);
				float newAzimuth = (this._targetAzimuth + currentOffset) % 360f;
				bool flag2 = newAzimuth < 0f;
				if (flag2)
				{
					newAzimuth += 360f;
				}
				AdventureLightingManager.Instance.GlobalAzimuthAngle = newAzimuth;
				float easeProgress = Mathf.Pow(progress, this.IncidenceCurvePower);
				float newIncidence = Mathf.Lerp(0f, this._targetIncidence, easeProgress);
				AdventureLightingManager.Instance.GlobalIncidenceAngle = newIncidence;
				float currentH = Mathf.Lerp(0.6f, this._targetH, progress);
				Color newColor = Color.HSVToRGB(currentH, this._targetS, this._targetV);
				newColor.a = this._targetColor.a;
				AdventureLightingManager.Instance.GlobalColor = newColor;
			}
		}

		// Token: 0x04007D71 RID: 32113
		[Tooltip("初始角度偏移量")]
		public float StartOffset = -180f;

		// Token: 0x04007D72 RID: 32114
		[Tooltip("入射角过渡曲线强度 (1=线性, >1=下凸/EaseIn)")]
		[Range(1f, 10f)]
		public float IncidenceCurvePower = 3f;

		// Token: 0x04007D73 RID: 32115
		private float _targetAzimuth;

		// Token: 0x04007D74 RID: 32116
		private float _targetIncidence;

		// Token: 0x04007D75 RID: 32117
		private Color _targetColor;

		// Token: 0x04007D76 RID: 32118
		private float _targetH;

		// Token: 0x04007D77 RID: 32119
		private float _targetS;

		// Token: 0x04007D78 RID: 32120
		private float _targetV;

		// Token: 0x04007D79 RID: 32121
		private bool _isInitialized;
	}
}
