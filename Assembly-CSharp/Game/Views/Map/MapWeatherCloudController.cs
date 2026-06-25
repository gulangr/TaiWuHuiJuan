using System;
using FrameWork.Tools;
using UnityEngine;

namespace Game.Views.Map
{
	// Token: 0x02000943 RID: 2371
	[Serializable]
	public class MapWeatherCloudController : IComparable<MapWeatherCloudController>
	{
		// Token: 0x17000CC4 RID: 3268
		// (get) Token: 0x06006EA2 RID: 28322 RVA: 0x00332D43 File Offset: 0x00330F43
		public bool Invalid
		{
			get
			{
				return this.cloud == null || this.shadow == null;
			}
		}

		// Token: 0x06006EA3 RID: 28323 RVA: 0x00332D64 File Offset: 0x00330F64
		public MapWeatherCloudController(IMapWeatherCloudHandler handler, int index, RectTransform cloud, RectTransform shadow, float maxAlpha)
		{
			this._handler = handler;
			this.index = index;
			this.cloud = cloud;
			this.shadow = shadow;
			this.cloud.name = index.ToString();
			this.shadow.name = index.ToString();
			this._cachedCloudImage = cloud.GetComponentInChildren<CImage>();
			this._cachedShadowImage = shadow.GetComponentInChildren<CImage>();
			this._cachedShadowScale = shadow.localScale.x;
			this._cachedMaxAlpha = maxAlpha;
			this.ResetPos();
		}

		// Token: 0x06006EA4 RID: 28324 RVA: 0x00332DF8 File Offset: 0x00330FF8
		public void Move(Vector2 delta)
		{
			this.pos += delta;
			bool flag = this._handler.ShouldReset(this.pos);
			if (flag)
			{
				this.hiding = true;
			}
			bool flag2 = this.hiding && this.visibleDelta <= 0f;
			if (flag2)
			{
				this.ResetPos();
			}
			else
			{
				this.UpdatePos();
			}
		}

		// Token: 0x06006EA5 RID: 28325 RVA: 0x00332E64 File Offset: 0x00331064
		public void ResetPos()
		{
			this.pos = this._handler.SamplePos;
			this.factor = this._handler.SampleFactor;
			string spriteName = this._handler.SampleImage;
			this._cachedCloudImage.SetSprite(spriteName, true, null);
			this._cachedShadowImage.SetSprite(spriteName, true, null);
			this.visibleDelta = 0f;
			this.hiding = false;
			this.UpdatePos();
		}

		// Token: 0x06006EA6 RID: 28326 RVA: 0x00332ED7 File Offset: 0x003310D7
		public void UpdateAlpha(float maxAlpha)
		{
			this._cachedMaxAlpha = maxAlpha;
			this.UpdatePos();
		}

		// Token: 0x06006EA7 RID: 28327 RVA: 0x00332EE8 File Offset: 0x003310E8
		private void UpdatePos()
		{
			this.shadow.anchoredPosition = this.pos;
			float cameraFactor = 1f / this._handler.BaseScale;
			float deltaFactor = cameraFactor - this.factor;
			bool showCloud = deltaFactor > 0.2f;
			bool flag = showCloud != this.cloud.gameObject.activeSelf;
			if (flag)
			{
				this.cloud.gameObject.SetActive(showCloud);
			}
			bool flag2 = !showCloud;
			if (!flag2)
			{
				float physicalFactor = cameraFactor / deltaFactor;
				float cloudScale = physicalFactor * this._cachedShadowScale;
				this.cloud.localScale = Vector3.one * cloudScale;
				this.cloud.anchoredPosition = physicalFactor * this.pos + (1f - physicalFactor) * cameraFactor * this._handler.BaseOffset;
				this.alpha = Mathf.Min(this._cachedMaxAlpha, (deltaFactor - 0.2f) / 0.2f);
				this.alpha = Mathf.Min(this.alpha, this.CalcScreenAlpha());
				bool flag3 = this.hiding;
				if (flag3)
				{
					this.visibleDelta = Mathf.Max(0f, this.visibleDelta - Time.deltaTime);
				}
				else
				{
					bool flag4 = this.visibleDelta < 1f;
					if (flag4)
					{
						this.visibleDelta = Mathf.Min(1f, this.visibleDelta + Time.deltaTime);
					}
				}
				float finalAlpha = this.alpha * this.visibleDelta / 1f;
				this._cachedCloudImage.color = this._cachedCloudImage.color.SetAlpha(finalAlpha);
				this._cachedShadowImage.color = this._cachedShadowImage.color.SetAlpha(this.visibleDelta * 0.3f * this._cachedMaxAlpha);
			}
		}

		// Token: 0x06006EA8 RID: 28328 RVA: 0x003330B8 File Offset: 0x003312B8
		private float CalcScreenAlpha()
		{
			Vector2 screenCenter = new Vector2((float)Screen.width / 2f, (float)Screen.height / 2f);
			Vector2 cloudScreenPos = this.cloud.LocalToScreenPoint(Vector3.zero);
			float xAlpha = MapWeatherCloudController.CalcScreenAlpha(Mathf.Abs(screenCenter.x - cloudScreenPos.x), screenCenter.x);
			float yAlpha = MapWeatherCloudController.CalcScreenAlpha(Mathf.Abs(screenCenter.y - cloudScreenPos.y), screenCenter.y);
			return Mathf.Max(xAlpha, yAlpha);
		}

		// Token: 0x06006EA9 RID: 28329 RVA: 0x00333144 File Offset: 0x00331344
		private static float CalcScreenAlpha(float delta, float size)
		{
			float alphaThreshold = size * 0.8f;
			bool flag = delta >= alphaThreshold;
			float result;
			if (flag)
			{
				result = 1f;
			}
			else
			{
				float hideThreshold = size * 0.1f;
				result = Mathf.Max(0f, (delta - hideThreshold) / (alphaThreshold - hideThreshold));
			}
			return result;
		}

		// Token: 0x06006EAA RID: 28330 RVA: 0x0033318C File Offset: 0x0033138C
		public int CompareTo(MapWeatherCloudController other)
		{
			bool flag = this == other;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				bool flag2 = other == null;
				if (flag2)
				{
					result = 1;
				}
				else
				{
					bool flag3 = !Mathf.Approximately(this.factor, other.factor);
					if (flag3)
					{
						result = this.factor.CompareTo(other.factor);
					}
					else
					{
						result = this.cloud.GetSiblingIndex().CompareTo(other.cloud.GetSiblingIndex());
					}
				}
			}
			return result;
		}

		// Token: 0x06006EAB RID: 28331 RVA: 0x00333204 File Offset: 0x00331404
		public void SetSiblingIndex(int i)
		{
			bool flag = this.cloud.GetSiblingIndex() != i;
			if (flag)
			{
				this.cloud.SetSiblingIndex(i);
			}
			bool flag2 = this.shadow.GetSiblingIndex() != i;
			if (flag2)
			{
				this.shadow.SetSiblingIndex(i);
			}
		}

		// Token: 0x04005253 RID: 21075
		[SerializeField]
		private int index;

		// Token: 0x04005254 RID: 21076
		[SerializeField]
		private float factor;

		// Token: 0x04005255 RID: 21077
		[SerializeField]
		private Vector2 pos;

		// Token: 0x04005256 RID: 21078
		[SerializeField]
		private RectTransform cloud;

		// Token: 0x04005257 RID: 21079
		[SerializeField]
		private RectTransform shadow;

		// Token: 0x04005258 RID: 21080
		[SerializeField]
		private float alpha;

		// Token: 0x04005259 RID: 21081
		[SerializeField]
		private float visibleDelta;

		// Token: 0x0400525A RID: 21082
		[SerializeField]
		private bool hiding;

		// Token: 0x0400525B RID: 21083
		private CImage _cachedCloudImage;

		// Token: 0x0400525C RID: 21084
		private CImage _cachedShadowImage;

		// Token: 0x0400525D RID: 21085
		private float _cachedShadowScale;

		// Token: 0x0400525E RID: 21086
		private IMapWeatherCloudHandler _handler;

		// Token: 0x0400525F RID: 21087
		private float _cachedMaxAlpha;
	}
}
