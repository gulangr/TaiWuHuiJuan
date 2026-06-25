using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Config;
using EasyButtons;
using GameData.Combat.Math;
using Map.RenderSystem;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Views.Map
{
	// Token: 0x02000942 RID: 2370
	public class MapWeatherCloud : MonoBehaviour, IMapWeatherCloudHandler
	{
		// Token: 0x17000CBE RID: 3262
		// (get) Token: 0x06006E8F RID: 28303 RVA: 0x0033255C File Offset: 0x0033075C
		private int FinalCloudCount
		{
			get
			{
				bool flag = this.isBuildingArea;
				int result;
				if (flag)
				{
					result = this.cloudCount * this._cloudCountPercent;
				}
				else
				{
					WorldMapModel mapModel = SingletonObject.getInstance<WorldMapModel>();
					short showingAreaId = mapModel.ShowingAreaId;
					int mapSize = (int)((showingAreaId >= 0) ? mapModel.GetAreaSize(showingAreaId) : 0);
					result = Mathf.Max(mapSize * this._cloudCountPercent, 1);
				}
				return result;
			}
		}

		// Token: 0x17000CBF RID: 3263
		// (get) Token: 0x06006E90 RID: 28304 RVA: 0x003325C0 File Offset: 0x003307C0
		public Vector2 SamplePos
		{
			get
			{
				this.UpdateMapBoundary(false);
				Vector2 minPos = this._boundary.min;
				Vector2 maxPos = this._boundary.max;
				bool flag = Mathf.Approximately(this._boundary.size.sqrMagnitude, 0f);
				if (flag)
				{
					minPos = new Vector2(-1048576f, -1048576f);
					maxPos = minPos + Vector2.one;
				}
				bool flag2 = this._cdf == null;
				int width;
				int height;
				int y;
				int x;
				if (flag2)
				{
					bool flag3 = this.sampleDistributionTexture;
					if (!flag3)
					{
						return new Vector2(Random.Range(minPos.x, maxPos.x), Random.Range(minPos.y, maxPos.y));
					}
					width = this.sampleDistributionTexture.width;
					height = this.sampleDistributionTexture.height;
					Color[] pixels = this.sampleDistributionTexture.GetPixels();
					float[,] weights = new float[width, height];
					float totalWeight = 0f;
					for (y = 0; y < height; y++)
					{
						for (x = 0; x < width; x++)
						{
							Color c = pixels[y * width + x];
							float grayscale = c.grayscale;
							weights[x, y] = Mathf.Pow(1f - grayscale, 2f);
							totalWeight += weights[x, y];
						}
					}
					this._cdf = new float[width * height];
					float cumulative = 0f;
					for (int i = 0; i < width * height; i++)
					{
						x = i % width;
						y = i / width;
						cumulative += weights[x, y] / totalWeight;
						this._cdf[i] = cumulative;
					}
				}
				width = this.sampleDistributionTexture.width;
				height = this.sampleDistributionTexture.height;
				float randomValue = Random.value;
				int index = this.<get_SamplePos>g__BinarySearchCdf|20_0(randomValue);
				x = index % width;
				y = index / width;
				float worldX = Mathf.Lerp(minPos.x, maxPos.x, ((float)x + Random.value) / (float)width);
				float worldY = Mathf.Lerp(minPos.y, maxPos.y, ((float)y + Random.value) / (float)height);
				return new Vector2(worldX, worldY);
			}
		}

		// Token: 0x17000CC0 RID: 3264
		// (get) Token: 0x06006E91 RID: 28305 RVA: 0x0033280A File Offset: 0x00330A0A
		public float SampleFactor
		{
			get
			{
				return this.factorRange.GetRandom<float>();
			}
		}

		// Token: 0x17000CC1 RID: 3265
		// (get) Token: 0x06006E92 RID: 28306 RVA: 0x00332818 File Offset: 0x00330A18
		public string SampleImage
		{
			get
			{
				return this._cloudSpritePrefix + Random.Range(1, 5).ToString();
			}
		}

		// Token: 0x17000CC2 RID: 3266
		// (get) Token: 0x06006E93 RID: 28307 RVA: 0x0033283F File Offset: 0x00330A3F
		float IMapWeatherCloudHandler.BaseScale
		{
			get
			{
				return this.target.localScale.x;
			}
		}

		// Token: 0x17000CC3 RID: 3267
		// (get) Token: 0x06006E94 RID: 28308 RVA: 0x00332851 File Offset: 0x00330A51
		Vector2 IMapWeatherCloudHandler.BaseOffset
		{
			get
			{
				return -(((RectTransform)base.transform).anchoredPosition * this.target.localScale.x + this.target.anchoredPosition);
			}
		}

		// Token: 0x06006E95 RID: 28309 RVA: 0x00332890 File Offset: 0x00330A90
		bool IMapWeatherCloudHandler.ShouldReset(Vector2 pos)
		{
			return !this._boundary.Contains(pos);
		}

		// Token: 0x06006E96 RID: 28310 RVA: 0x003328B4 File Offset: 0x00330AB4
		private void Update()
		{
			MapWeatherCloudController[] array = this.cloudControllers;
			bool flag = array == null || array.Length <= 0;
			if (!flag)
			{
				this.UpdateMapBoundary(false);
				Array.Sort<MapWeatherCloudController>(this.cloudControllers);
				for (int i = 0; i < this.cloudControllers.Length; i++)
				{
					this.cloudControllers[i].SetSiblingIndex(i);
				}
				foreach (MapWeatherCloudController controller in this.cloudControllers)
				{
					controller.Move(this.moveDelta * this._cloudSpeedPercent);
				}
			}
		}

		// Token: 0x06006E97 RID: 28311 RVA: 0x00332954 File Offset: 0x00330B54
		private void UpdateMapBoundary(bool force = false)
		{
			bool flag = this.isBuildingArea;
			if (flag)
			{
				this._boundary = this.buildingAreaRect;
				base.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
			}
			else
			{
				MapRenderSystem renderSystem = SingletonObject.getInstance<MapRenderSystem>();
				this._boundary = renderSystem.CalcBoundary(force);
				this._boundary.min = this._boundary.min * this.distributionScale;
				this._boundary.max = this._boundary.max * this.distributionScale;
				WorldMapModel mapModel = SingletonObject.getInstance<WorldMapModel>();
				short showingAreaId = mapModel.ShowingAreaId;
				base.GetComponent<RectTransform>().anchoredPosition = MapRenderSystem.CalcDefaultMapLayerPosition(showingAreaId);
			}
		}

		// Token: 0x06006E98 RID: 28312 RVA: 0x003329F8 File Offset: 0x00330BF8
		[Button]
		private void ReGenerate()
		{
			this.UpdateMapBoundary(true);
			bool flag = this.cloudControllers == null || this.cloudControllers.Length != this.FinalCloudCount;
			if (flag)
			{
				this.GenerateAndBindControllers();
			}
			else
			{
				this.ResetControllers();
			}
		}

		// Token: 0x06006E99 RID: 28313 RVA: 0x00332A40 File Offset: 0x00330C40
		private void GenerateAndBindControllers()
		{
			Transform templateCloud = this.layerCloud.GetChild(0);
			Transform templateShadow = this.layerShadow.GetChild(0);
			int finalCloudCount = this.FinalCloudCount;
			this.cloudControllers = new MapWeatherCloudController[finalCloudCount];
			for (int i = 0; i < finalCloudCount; i++)
			{
				bool flag = this.layerCloud.childCount <= i;
				if (flag)
				{
					Object.Instantiate<Transform>(templateCloud, this.layerCloud);
				}
				RectTransform cloud = (RectTransform)this.layerCloud.GetChild(i);
				this.ShowIfInHide(cloud);
				bool flag2 = this.layerShadow.childCount <= i;
				if (flag2)
				{
					Object.Instantiate<Transform>(templateShadow, this.layerShadow);
				}
				RectTransform shadow = (RectTransform)this.layerShadow.GetChild(i);
				this.ShowIfInHide(shadow);
				this.cloudControllers[i] = new MapWeatherCloudController(this, i, cloud, shadow, this.cloudMaxAlpha);
			}
			this.HideExceedObjects(this.layerCloud, finalCloudCount);
			this.HideExceedObjects(this.layerShadow, finalCloudCount);
		}

		// Token: 0x06006E9A RID: 28314 RVA: 0x00332B4C File Offset: 0x00330D4C
		private void ResetControllers()
		{
			foreach (MapWeatherCloudController controller in this.cloudControllers)
			{
				controller.ResetPos();
				controller.UpdateAlpha(this.cloudMaxAlpha);
			}
		}

		// Token: 0x06006E9B RID: 28315 RVA: 0x00332B8C File Offset: 0x00330D8C
		private void ShowIfInHide(RectTransform rt)
		{
			bool activeSelf = rt.gameObject.activeSelf;
			if (!activeSelf)
			{
				rt.gameObject.SetActive(true);
			}
		}

		// Token: 0x06006E9C RID: 28316 RVA: 0x00332BB8 File Offset: 0x00330DB8
		private void HideExceedObjects(Transform root, int usedCount)
		{
			for (int i = usedCount; i < root.childCount; i++)
			{
				Transform child = root.GetChild(i);
				bool activeSelf = child.gameObject.activeSelf;
				if (activeSelf)
				{
					child.gameObject.SetActive(false);
				}
			}
		}

		// Token: 0x06006E9D RID: 28317 RVA: 0x00332C04 File Offset: 0x00330E04
		public void SetCloudParameters(WeatherItem weather)
		{
			this.factorRange = weather.CloudLayers.ToArray<float>();
			this._cloudCountPercent = weather.CloudCount;
			this._cloudSpeedPercent = (float)weather.CloudSpeed / 100f;
			this._cloudSpritePrefix = "map_weather_cloud_" + weather.CloudType.ToString().ToLower() + "_";
			this.ReGenerate();
		}

		// Token: 0x06006E9E RID: 28318 RVA: 0x00332C79 File Offset: 0x00330E79
		public void ReGenerateOnAreaChanged()
		{
			this.ReGenerate();
		}

		// Token: 0x06006E9F RID: 28319 RVA: 0x00332C83 File Offset: 0x00330E83
		[Button("UpdateSampleDistributionTexture")]
		public void UpdateSampleDistributionTexture(Texture2D value)
		{
			this.sampleDistributionTexture = value;
			this._cdf = null;
		}

		// Token: 0x06006EA1 RID: 28321 RVA: 0x00332CF4 File Offset: 0x00330EF4
		[CompilerGenerated]
		private int <get_SamplePos>g__BinarySearchCdf|20_0(float value)
		{
			int left = 0;
			int right = this._cdf.Length - 1;
			while (left < right)
			{
				int mid = left + (right - left) / 2;
				bool flag = this._cdf[mid] < value;
				if (flag)
				{
					left = mid + 1;
				}
				else
				{
					right = mid;
				}
			}
			return left;
		}

		// Token: 0x04005242 RID: 21058
		[Header("是否为产业界面")]
		[FormerlySerializedAs("isBuidingArea")]
		[SerializeField]
		internal bool isBuildingArea;

		// Token: 0x04005243 RID: 21059
		[SerializeField]
		internal Rect buildingAreaRect;

		// Token: 0x04005244 RID: 21060
		[SerializeField]
		internal Transform layerCloud;

		// Token: 0x04005245 RID: 21061
		[SerializeField]
		internal Transform layerShadow;

		// Token: 0x04005246 RID: 21062
		[SerializeField]
		private RectTransform target;

		// Token: 0x04005247 RID: 21063
		[SerializeField]
		private float[] factorRange = new float[]
		{
			0.5f,
			1.5f,
			2.5f
		};

		// Token: 0x04005248 RID: 21064
		[SerializeField]
		private Vector2 moveDelta = new Vector2(0.6f, 0.2f);

		// Token: 0x04005249 RID: 21065
		[SerializeField]
		private float distributionScale = 2f;

		// Token: 0x0400524A RID: 21066
		[SerializeField]
		private int cloudCount = 100;

		// Token: 0x0400524B RID: 21067
		[SerializeField]
		private MapWeatherCloudController[] cloudControllers;

		// Token: 0x0400524C RID: 21068
		[SerializeField]
		private Texture2D sampleDistributionTexture;

		// Token: 0x0400524D RID: 21069
		[SerializeField]
		private float cloudMaxAlpha = 0.8f;

		// Token: 0x0400524E RID: 21070
		private CValuePercent _cloudCountPercent;

		// Token: 0x0400524F RID: 21071
		private float _cloudSpeedPercent;

		// Token: 0x04005250 RID: 21072
		private string _cloudSpritePrefix;

		// Token: 0x04005251 RID: 21073
		private float[] _cdf;

		// Token: 0x04005252 RID: 21074
		private Rect _boundary;
	}
}
