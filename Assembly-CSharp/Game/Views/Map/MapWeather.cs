using System;
using System.Collections.Generic;
using Config;
using EasyButtons;
using FrameWork;
using GameData.Domains.Map;
using GameData.Utilities;
using Map.RenderSystem;
using UnityEngine;

namespace Game.Views.Map
{
	// Token: 0x02000941 RID: 2369
	public class MapWeather : MonoBehaviour
	{
		// Token: 0x17000CBD RID: 3261
		// (get) Token: 0x06006E7F RID: 28287 RVA: 0x00331DAE File Offset: 0x0032FFAE
		private WorldMapModel mapModel
		{
			get
			{
				return SingletonObject.getInstance<WorldMapModel>();
			}
		}

		// Token: 0x06006E80 RID: 28288 RVA: 0x00331DB8 File Offset: 0x0032FFB8
		private void Awake()
		{
			bool flag = this.weatherCloud == null;
			if (flag)
			{
				this.weatherCloud = base.transform.Find("CloudEffect").GetComponent<MapWeatherCloud>();
			}
			bool flag2 = this.weatherCloud == null;
			if (!flag2)
			{
				bool activeSelf = this.weatherCloud.gameObject.activeSelf;
				if (activeSelf)
				{
					this.weatherCloud.gameObject.SetActive(false);
				}
			}
		}

		// Token: 0x06006E81 RID: 28289 RVA: 0x00331E2C File Offset: 0x0033002C
		private void OnEnable()
		{
			this.UpdateWeather();
			GEvent.Add(UiEvents.WorldMapPlayerAreaChange, new GEvent.Callback(this.OnEventUpdate));
			GEvent.Add(UiEvents.WorldMapUpdateAreaOffset, new GEvent.Callback(this.OnEventUpdate));
			GEvent.Add(UiEvents.WeatherChanged, new GEvent.Callback(this.WeatherChanged));
			GEvent.Add(UiEvents.GmUpdateWeather, new GEvent.Callback(this.TestWeatherForGm));
		}

		// Token: 0x06006E82 RID: 28290 RVA: 0x00331EA8 File Offset: 0x003300A8
		private void OnDisable()
		{
			GEvent.Remove(UiEvents.WorldMapPlayerAreaChange, new GEvent.Callback(this.OnEventUpdate));
			GEvent.Remove(UiEvents.WorldMapUpdateAreaOffset, new GEvent.Callback(this.OnEventUpdate));
			GEvent.Remove(UiEvents.WeatherChanged, new GEvent.Callback(this.WeatherChanged));
			GEvent.Remove(UiEvents.GmUpdateWeather, new GEvent.Callback(this.TestWeatherForGm));
		}

		// Token: 0x06006E83 RID: 28291 RVA: 0x00331F20 File Offset: 0x00330120
		private void OnEventUpdate(ArgumentBox argBox)
		{
			this.UpdateWeather();
			WeatherItem weatherItem = Weather.Instance[this._showingWeatherTemplateId];
			float[] array = (weatherItem != null) ? weatherItem.CloudLayers : null;
			bool flag = array != null && array.Length > 0 && this.weatherCloud != null;
			if (flag)
			{
				this.weatherCloud.ReGenerateOnAreaChanged();
			}
		}

		// Token: 0x06006E84 RID: 28292 RVA: 0x00331F7A File Offset: 0x0033017A
		private void WeatherChanged(ArgumentBox argBox)
		{
			this.UpdateWeather();
		}

		// Token: 0x06006E85 RID: 28293 RVA: 0x00331F83 File Offset: 0x00330183
		[Button]
		private void TestWeather(sbyte weatherTemplateId)
		{
			this._testingWeatherTemplateId = weatherTemplateId;
			this.UpdateWeather();
		}

		// Token: 0x06006E86 RID: 28294 RVA: 0x00331F94 File Offset: 0x00330194
		private void TestWeatherForGm(ArgumentBox argumentBox)
		{
			sbyte weatherTemplateId;
			argumentBox.Get("WeatherTemplateId", out weatherTemplateId);
			this._testingWeatherTemplateId = weatherTemplateId;
			this.UpdateWeather();
			this._testingWeatherTemplateId = -1;
		}

		// Token: 0x06006E87 RID: 28295 RVA: 0x00331FC8 File Offset: 0x003301C8
		private void UpdateWeather()
		{
			bool flag = !SingletonObject.getInstance<GlobalSettings>().MapWeather;
			if (flag)
			{
				foreach (GameObject particle in this._weatherParticleCache.Values)
				{
					bool activeSelf = particle.activeSelf;
					if (activeSelf)
					{
						particle.SetActive(false);
					}
				}
				this._showingWeatherTemplateId = -1;
			}
			else
			{
				BasicGameData basicModel = SingletonObject.getInstance<BasicGameData>();
				short areaTemplateId = this.mapModel.GetCurrentAreaTemplateId();
				short currentAreaId = this.mapModel.GetAreaIdByAreaTemplateId(areaTemplateId);
				bool flag2 = this.UseBuildingAreaId();
				if (flag2)
				{
					currentAreaId = this._buildingAreaId;
				}
				sbyte stateTemplateId = this.GetStateTemplateId();
				bool flag3 = this._testingWeatherTemplateId >= 0;
				if (flag3)
				{
					this.SetWeather(this._testingWeatherTemplateId);
				}
				else
				{
					sbyte weatherTemplateId;
					bool flag4 = basicModel.AreaStoryWeathers.TryGetValue(currentAreaId, out weatherTemplateId);
					if (flag4)
					{
						this.SetWeather(weatherTemplateId);
					}
					else
					{
						bool flag5 = basicModel.StateWeathers.TryGetValue(stateTemplateId, out weatherTemplateId);
						if (flag5)
						{
							this.SetWeather(weatherTemplateId);
						}
						else
						{
							this.SetWeather(-1);
						}
					}
				}
				bool flag6 = this._loadingParticles.Count > 0;
				if (flag6)
				{
					foreach (GameObject particle2 in this._weatherParticleCache.Values)
					{
						bool activeSelf2 = particle2.activeSelf;
						if (activeSelf2)
						{
							particle2.SetActive(false);
						}
					}
				}
				else
				{
					this.UpdateCloud();
					foreach (KeyValuePair<sbyte, GameObject> keyValuePair in this._weatherParticleCache)
					{
						sbyte b;
						GameObject gameObject;
						keyValuePair.Deconstruct(out b, out gameObject);
						sbyte particleCache = b;
						GameObject particleGo = gameObject;
						MapBlockData block = this.mapModel.PlayerAtBlock;
						bool show = particleCache == this._showingWeatherTemplateId && (block != null || this.isBuildingArea);
						bool flag7 = show != particleGo.activeSelf;
						if (flag7)
						{
							particleGo.SetActive(show);
						}
						bool flag8 = !show;
						if (!flag8)
						{
							WeatherItem config = Weather.Instance[this._showingWeatherTemplateId];
							bool flag9 = this.isBuildingArea || config.Type == EWeatherType.Screen;
							if (flag9)
							{
								particleGo.transform.localPosition = Vector2.zero;
							}
							else
							{
								byte mapSize = this.mapModel.GetAreaSize(block.AreaId);
								ByteCoordinate blockPos = new ByteCoordinate(mapSize / 2, mapSize / 2);
								short blockId = WorldMapModel.CoordinateToIndex(blockPos, mapSize);
								Location location = new Location(block.AreaId, blockId);
								particleGo.transform.localPosition = MapRenderSystem.GetBlockLocalPos(location);
							}
						}
					}
				}
			}
		}

		// Token: 0x06006E88 RID: 28296 RVA: 0x003322EC File Offset: 0x003304EC
		private void UpdateCloud()
		{
			bool flag = this.weatherCloud == null;
			if (!flag)
			{
				WeatherItem config = Weather.Instance[this._showingWeatherTemplateId];
				float[] array = (config != null) ? config.CloudLayers : null;
				bool show = array != null && array.Length > 0;
				bool flag2 = show != this.weatherCloud.gameObject.activeSelf;
				if (flag2)
				{
					this.weatherCloud.gameObject.SetActive(show);
				}
				bool flag3 = !show;
				if (!flag3)
				{
					this.weatherCloud.SetCloudParameters(config);
				}
			}
		}

		// Token: 0x06006E89 RID: 28297 RVA: 0x00332380 File Offset: 0x00330580
		private sbyte GetStateTemplateId()
		{
			bool flag = this.UseBuildingAreaId();
			sbyte result;
			if (flag)
			{
				result = this.mapModel.GetStateTemplateIdByAreaId(this._buildingAreaId);
			}
			else
			{
				result = this.mapModel.GetCurrentStateTemplateId();
			}
			return result;
		}

		// Token: 0x06006E8A RID: 28298 RVA: 0x003323BC File Offset: 0x003305BC
		private bool UseBuildingAreaId()
		{
			return this.isBuildingArea && this._buildingAreaId >= 0;
		}

		// Token: 0x06006E8B RID: 28299 RVA: 0x003323E8 File Offset: 0x003305E8
		private void SetWeather(sbyte weatherTemplateId)
		{
			bool flag = weatherTemplateId == this._showingWeatherTemplateId;
			if (!flag)
			{
				this._showingWeatherTemplateId = weatherTemplateId;
				bool flag2 = weatherTemplateId < 0 || this._weatherParticleCache.ContainsKey(weatherTemplateId);
				if (!flag2)
				{
					bool flag3 = this._loadingParticles.Contains(weatherTemplateId);
					if (!flag3)
					{
						WeatherItem config = Weather.Instance[weatherTemplateId];
						bool flag4 = config == null || string.IsNullOrEmpty(config.Particle);
						if (!flag4)
						{
							this._loadingParticles.Add(weatherTemplateId);
							this.CreateCache(weatherTemplateId, config.Particle);
						}
					}
				}
			}
		}

		// Token: 0x06006E8C RID: 28300 RVA: 0x00332478 File Offset: 0x00330678
		private void CreateCache(sbyte weatherTemplateId, string particle)
		{
			WeatherItem config = Weather.Instance[weatherTemplateId];
			Transform parent = (config.Type == EWeatherType.Screen) ? this.fullScreenEffHolder.transform : (this.isBuildingArea ? this.buildingAreaEffHolder : base.transform);
			ResLoader.Load<GameObject>("RemakeResources/Particle/MapBlockEffect/Weather/" + particle, delegate(GameObject prefab)
			{
				GameObject cache = Object.Instantiate<GameObject>(prefab, parent);
				this._weatherParticleCache[weatherTemplateId] = cache;
				this._loadingParticles.Remove(weatherTemplateId);
				this.UpdateWeather();
			}, null, false);
		}

		// Token: 0x06006E8D RID: 28301 RVA: 0x003324FC File Offset: 0x003306FC
		public void SetArea(short areaId)
		{
			this._buildingAreaId = areaId;
			bool activeSelf = base.gameObject.activeSelf;
			if (activeSelf)
			{
				this.UpdateWeather();
			}
		}

		// Token: 0x04005238 RID: 21048
		[Header("是否为产业界面天气")]
		[SerializeField]
		private bool isBuildingArea;

		// Token: 0x04005239 RID: 21049
		[Header("存档天气特效的地方，尽可能不使用UIParticle，使用SortingGroup调整层级，避免性能问题，父节点缩放时需要保持3轴一致，或者自行调整粒子缩放方式")]
		[SerializeField]
		private Transform buildingAreaEffHolder;

		// Token: 0x0400523A RID: 21050
		[Header("该节点需要在不受缩放和拖动的节点下")]
		[SerializeField]
		private Transform fullScreenEffHolder;

		// Token: 0x0400523B RID: 21051
		[SerializeField]
		private MapWeatherCloud weatherCloud;

		// Token: 0x0400523C RID: 21052
		private const string ParticlePrefix = "RemakeResources/Particle/MapBlockEffect/Weather/";

		// Token: 0x0400523D RID: 21053
		private readonly Dictionary<sbyte, GameObject> _weatherParticleCache = new Dictionary<sbyte, GameObject>();

		// Token: 0x0400523E RID: 21054
		private readonly List<sbyte> _loadingParticles = new List<sbyte>();

		// Token: 0x0400523F RID: 21055
		private sbyte _showingWeatherTemplateId = -1;

		// Token: 0x04005240 RID: 21056
		private sbyte _testingWeatherTemplateId = -1;

		// Token: 0x04005241 RID: 21057
		private short _buildingAreaId = -1;
	}
}
