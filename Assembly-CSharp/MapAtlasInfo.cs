using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000044 RID: 68
public class MapAtlasInfo : ScriptableObject
{
	// Token: 0x1700004C RID: 76
	// (get) Token: 0x06000252 RID: 594 RVA: 0x0000DF5B File Offset: 0x0000C15B
	// (set) Token: 0x06000253 RID: 595 RVA: 0x0000DF62 File Offset: 0x0000C162
	public static MapAtlasInfo Instance { get; private set; }

	// Token: 0x06000254 RID: 596 RVA: 0x0000DF6A File Offset: 0x0000C16A
	public static void Init()
	{
		ResLoader.Load<MapAtlasInfo>("GameAtlas/MapAtlasInfo", delegate(MapAtlasInfo result)
		{
			MapAtlasInfo.Instance = result;
			int i = 0;
			int max = MapAtlasInfo.Instance.MapAtlasInfoList.Count;
			while (i < max)
			{
				MapAtlasInfo.Instance.MapAtlasInfoList[i].Init();
				i++;
			}
			MapAtlasInfo.Instance._loadedAtlas = new Dictionary<string, SingleAtlas>();
		}, null, false);
	}

	// Token: 0x06000255 RID: 597 RVA: 0x0000DF9C File Offset: 0x0000C19C
	public void AddAtlasInfo(string atlasName, List<string> spriteNameList)
	{
		bool flag = this.MapAtlasInfoList == null;
		if (flag)
		{
			this.MapAtlasInfoList = new List<MapAtlasInfo.MapBlockAtlas>();
		}
		this.MapAtlasInfoList.RemoveAll((MapAtlasInfo.MapBlockAtlas e) => e.AtlasName == atlasName);
		MapAtlasInfo.MapBlockAtlas info = new MapAtlasInfo.MapBlockAtlas
		{
			AtlasName = atlasName,
			SpriteNameList = spriteNameList
		};
		this.MapAtlasInfoList.Add(info);
	}

	// Token: 0x06000256 RID: 598 RVA: 0x0000E010 File Offset: 0x0000C210
	public string GetAtlasNameBySpriteName(string spriteName)
	{
		int i = 0;
		int max = this.MapAtlasInfoList.Count;
		while (i < max)
		{
			MapAtlasInfo.MapBlockAtlas info = this.MapAtlasInfoList[i];
			bool flag = info.HasSprite(spriteName);
			if (flag)
			{
				return info.AtlasName;
			}
			i++;
		}
		return string.Empty;
	}

	// Token: 0x06000257 RID: 599 RVA: 0x0000E06C File Offset: 0x0000C26C
	public void GetSprite(string spriteName, Action<Sprite> onGetAction)
	{
		foreach (KeyValuePair<string, SingleAtlas> pair in this._loadedAtlas)
		{
			bool flag = pair.Value.HasSprite(spriteName);
			if (flag)
			{
				Sprite sprite = pair.Value.GetSprite(spriteName);
				bool flag2 = null == sprite;
				if (flag2)
				{
					Debug.LogWarning(spriteName + " failed get sprite from atlas " + pair.Key);
				}
				else
				{
					Action<Sprite> onGetAction2 = onGetAction;
					if (onGetAction2 != null)
					{
						onGetAction2(sprite);
					}
				}
				return;
			}
		}
		int i = 0;
		int max = this.MapAtlasInfoList.Count;
		while (i < max)
		{
			MapAtlasInfo.MapBlockAtlas info = this.MapAtlasInfoList[i];
			bool flag3 = info.HasSprite(spriteName);
			if (flag3)
			{
				ResLoader.Load<SingleAtlas>(string.Format("GameAtlas/MapAtlases/{0}", info.AtlasName), delegate(SingleAtlas atlas)
				{
					atlas.Init();
					this._loadedAtlas.Add(info.AtlasName, atlas);
					Sprite sprite2 = atlas.GetSprite(spriteName);
					bool flag4 = null == sprite2;
					if (flag4)
					{
						Debug.LogWarning(spriteName + " failed get sprite from atlas " + atlas.Name);
					}
					else
					{
						Action<Sprite> onGetAction3 = onGetAction;
						if (onGetAction3 != null)
						{
							onGetAction3(sprite2);
						}
					}
				}, null, false);
				return;
			}
			i++;
		}
		Debug.LogWarning("failed to get map block sprite with name " + spriteName);
	}

	// Token: 0x0400011D RID: 285
	public const string InfoFilePath = "GameAtlas/MapAtlasInfo";

	// Token: 0x0400011E RID: 286
	private const string MapAtlasPathBase = "GameAtlas/MapAtlases/{0}";

	// Token: 0x0400011F RID: 287
	[SerializeField]
	private List<MapAtlasInfo.MapBlockAtlas> MapAtlasInfoList;

	// Token: 0x04000120 RID: 288
	private Dictionary<string, SingleAtlas> _loadedAtlas;

	// Token: 0x020010BB RID: 4283
	[Serializable]
	private class MapBlockAtlas
	{
		// Token: 0x0600C066 RID: 49254 RVA: 0x0056B42D File Offset: 0x0056962D
		public void Init()
		{
			this._runtimeCache = new Dictionary<string, bool>();
			this.SpriteNameList.ForEach(delegate(string e)
			{
				this._runtimeCache.Add(e, true);
			});
		}

		// Token: 0x0600C067 RID: 49255 RVA: 0x0056B454 File Offset: 0x00569654
		public bool HasSprite(string spriteName)
		{
			return this._runtimeCache.ContainsKey(spriteName);
		}

		// Token: 0x0400943E RID: 37950
		public string AtlasName;

		// Token: 0x0400943F RID: 37951
		public List<string> SpriteNameList = new List<string>();

		// Token: 0x04009440 RID: 37952
		private Dictionary<string, bool> _runtimeCache;
	}
}
