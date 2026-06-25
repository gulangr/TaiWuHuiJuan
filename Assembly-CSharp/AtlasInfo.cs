using System;
using System.Collections.Generic;
using FrameWork;
using UnityEngine;
using UnityEngine.U2D;

// Token: 0x02000043 RID: 67
public class AtlasInfo : ScriptableObject
{
	// Token: 0x0600023B RID: 571 RVA: 0x0000D328 File Offset: 0x0000B528
	public static void Init()
	{
		SingletonObject.getInstance<SpritePackerHandler>();
		ResLoader.Load<AtlasInfo>(AtlasInfo.FilePath, delegate(AtlasInfo a)
		{
			AtlasInfo.Instance = a;
			AtlasInfo.Instance.InitSelf();
		}, delegate(string path)
		{
			GLog.TagError("Atlas", "Failed to Load AtlasInfo from " + path, Array.Empty<object>());
		}, false);
		MapAtlasInfo.Init();
	}

	// Token: 0x0600023C RID: 572 RVA: 0x0000D38C File Offset: 0x0000B58C
	public void InitSelf()
	{
		this._runTimeCache = new Dictionary<string, Dictionary<string, Sprite>>();
		this.AllPackerInfos.ForEach(delegate(int index, AtlasInfo.PackerDetail pd)
		{
			Dictionary<string, Sprite> map = new Dictionary<string, Sprite>();
			int i = 0;
			int max = pd.SpriteNames.Count;
			while (i < max)
			{
				Sprite sp = null;
				bool flag = pd.SpriteList != null && pd.SpriteList.Count > 0;
				if (flag)
				{
					sp = pd.SpriteList[i];
				}
				map.Add(pd.SpriteNames[i], sp);
				i++;
			}
			this._runTimeCache.Add(pd.PackerName, map);
			return false;
		});
		this._loadedPackers = new Dictionary<string, SpriteAtlas>();
		this._canUnloadList = new List<string>();
		this.LoadConstantPackers();
		GEvent.Add(UiEvents.UnloadPackers, new GEvent.Callback(this.DoUnloadPackers));
	}

	// Token: 0x0600023D RID: 573 RVA: 0x0000D3F4 File Offset: 0x0000B5F4
	public void RegisterLoadedPacker(string packerName, SpriteAtlas packer)
	{
		bool flag = this._runTimeCache.ContainsKey(packerName);
		if (flag)
		{
			SpriteAtlas prevPacker;
			bool flag2 = this._loadedPackers.TryGetValue(packerName, out prevPacker) && prevPacker.GetInstanceID() == packer.GetInstanceID();
			if (!flag2)
			{
				this._loadedPackers[packerName] = packer;
			}
		}
	}

	// Token: 0x0600023E RID: 574 RVA: 0x0000D449 File Offset: 0x0000B649
	public void MarkPackerUnload(SpriteAtlas[] spriteAtlases)
	{
		if (spriteAtlases != null)
		{
			spriteAtlases.ForEach(delegate(int index, SpriteAtlas spAtlas)
			{
				bool flag = null != spAtlas;
				if (flag)
				{
					string packerName = spAtlas.name;
					bool flag2 = !this._canUnloadList.Contains(packerName);
					if (flag2)
					{
						this._canUnloadList.Add(packerName);
					}
				}
				return false;
			});
		}
	}

	// Token: 0x0600023F RID: 575 RVA: 0x0000D464 File Offset: 0x0000B664
	private void DoUnloadPackers(ArgumentBox argsBox)
	{
		UIBase[] allBases = UIManager.Instance.GetComponentsInChildren<UIBase>(true);
		foreach (UIBase uiBase in allBases)
		{
			foreach (SpriteAtlas spPacker in uiBase.RelativeAtlases)
			{
				bool flag = null != spPacker;
				if (flag)
				{
					this._canUnloadList.Remove(spPacker.name);
				}
			}
		}
		this._canUnloadList.ForEach(delegate(string e)
		{
			this._loadedPackers.Remove(e);
			Dictionary<string, Sprite> map;
			bool flag2 = this._runTimeCache.TryGetValue(e, out map);
			if (flag2)
			{
				List<string> keys = EasyPool.Get<List<string>>();
				keys.AddRange(map.Keys);
				foreach (string key in keys)
				{
					map[key] = null;
				}
				EasyPool.Free<List<string>>(keys);
			}
		});
		this._canUnloadList.Clear();
	}

	// Token: 0x06000240 RID: 576 RVA: 0x0000D4FF File Offset: 0x0000B6FF
	public void LoadConstantPackers()
	{
		this._constantPackers.ForEach(delegate(string e)
		{
			this.LoadPacker(e, null);
		});
	}

	// Token: 0x06000241 RID: 577 RVA: 0x0000D51C File Offset: 0x0000B71C
	public void LoadPacker(string packerName, Action<SpriteAtlas> onLoad = null)
	{
		bool flag = this._loadedPackers.ContainsKey(packerName);
		if (flag)
		{
			Action<SpriteAtlas> onLoad2 = onLoad;
			if (onLoad2 != null)
			{
				onLoad2(this.GetLoadedPacker(packerName));
			}
		}
		else
		{
			ResLoader.Load<SpriteAtlas>("GameAtlas/Packers/" + packerName, delegate(SpriteAtlas packer)
			{
				this.RegisterLoadedPacker(packerName, packer);
				Action<SpriteAtlas> onLoad3 = onLoad;
				if (onLoad3 != null)
				{
					onLoad3(packer);
				}
			}, null, false);
		}
	}

	// Token: 0x06000242 RID: 578 RVA: 0x0000D5A0 File Offset: 0x0000B7A0
	public bool HasPacker(string packerName)
	{
		return this._runTimeCache.ContainsKey(packerName);
	}

	// Token: 0x06000243 RID: 579 RVA: 0x0000D5C0 File Offset: 0x0000B7C0
	public bool SetImageSprite(CImage image, string spriteName)
	{
		AtlasInfo.<>c__DisplayClass17_0 CS$<>8__locals1 = new AtlasInfo.<>c__DisplayClass17_0();
		CS$<>8__locals1.image = image;
		CS$<>8__locals1.spriteName = spriteName;
		CS$<>8__locals1.<>4__this = this;
		bool flag = null != CS$<>8__locals1.image && null != CS$<>8__locals1.image.sprite && CS$<>8__locals1.image.sprite.name == CS$<>8__locals1.spriteName;
		bool result;
		if (flag)
		{
			CS$<>8__locals1.image.SetEnabled(true);
			bool autoSize = CS$<>8__locals1.image.AutoSize;
			if (autoSize)
			{
				CS$<>8__locals1.image.SetNativeSize();
			}
			result = true;
		}
		else
		{
			foreach (KeyValuePair<string, Dictionary<string, Sprite>> pair in this._runTimeCache)
			{
				Sprite sprite;
				bool flag2 = pair.Value.TryGetValue(CS$<>8__locals1.spriteName, out sprite);
				if (flag2)
				{
					bool flag3 = null != sprite && null != CS$<>8__locals1.image;
					if (flag3)
					{
						CS$<>8__locals1.image.sprite = sprite;
						AtlasInfo.SetSpriteFromMod(CS$<>8__locals1.image, CS$<>8__locals1.spriteName);
						CS$<>8__locals1.<SetImageSprite>g__AfterSetSprite|0();
						return true;
					}
					string key = pair.Key;
					Action<SpriteAtlas> onLoad;
					if ((onLoad = CS$<>8__locals1.<>9__1) == null)
					{
						onLoad = (CS$<>8__locals1.<>9__1 = delegate(SpriteAtlas atlas)
						{
							bool flag6 = null != CS$<>8__locals1.image;
							if (flag6)
							{
								CS$<>8__locals1.image.sprite = atlas.GetSprite(CS$<>8__locals1.spriteName);
								AtlasInfo.SetSpriteFromMod(CS$<>8__locals1.image, CS$<>8__locals1.spriteName);
								CS$<>8__locals1.<>4__this.CacheSprite(atlas.name, CS$<>8__locals1.spriteName, CS$<>8__locals1.image.sprite);
								base.<SetImageSprite>g__AfterSetSprite|0();
							}
							else
							{
								GLog.TagWarn("AtlasInfo", "Failed to set sprite " + CS$<>8__locals1.spriteName + ",can not find target!", Array.Empty<object>());
							}
						});
					}
					this.LoadPacker(key, onLoad);
					return true;
				}
			}
			bool flag4 = AtlasInfo.SetSpriteFromMod(CS$<>8__locals1.image, CS$<>8__locals1.spriteName);
			if (flag4)
			{
				CS$<>8__locals1.<SetImageSprite>g__AfterSetSprite|0();
				result = true;
			}
			else
			{
				bool flag5 = CS$<>8__locals1.image;
				if (flag5)
				{
					CS$<>8__locals1.image.SetEnabled(false);
				}
				result = false;
			}
		}
		return result;
	}

	// Token: 0x06000244 RID: 580 RVA: 0x0000D790 File Offset: 0x0000B990
	public bool SetImageSpriteOnly(CImage image, string spriteName)
	{
		AtlasInfo.<>c__DisplayClass18_0 CS$<>8__locals1 = new AtlasInfo.<>c__DisplayClass18_0();
		CS$<>8__locals1.image = image;
		CS$<>8__locals1.spriteName = spriteName;
		CS$<>8__locals1.<>4__this = this;
		bool flag = null != CS$<>8__locals1.image && null != CS$<>8__locals1.image.sprite && CS$<>8__locals1.image.sprite.name == CS$<>8__locals1.spriteName;
		bool result;
		if (flag)
		{
			bool autoSize = CS$<>8__locals1.image.AutoSize;
			if (autoSize)
			{
				CS$<>8__locals1.image.SetNativeSize();
			}
			result = true;
		}
		else
		{
			foreach (KeyValuePair<string, Dictionary<string, Sprite>> pair in this._runTimeCache)
			{
				Sprite sprite;
				bool flag2 = pair.Value.TryGetValue(CS$<>8__locals1.spriteName, out sprite);
				if (flag2)
				{
					bool flag3 = null != sprite && null != CS$<>8__locals1.image;
					if (flag3)
					{
						CS$<>8__locals1.image.sprite = sprite;
						AtlasInfo.SetSpriteFromMod(CS$<>8__locals1.image, CS$<>8__locals1.spriteName);
						CS$<>8__locals1.<SetImageSpriteOnly>g__AfterSetSprite|0();
						return true;
					}
					string key = pair.Key;
					Action<SpriteAtlas> onLoad;
					if ((onLoad = CS$<>8__locals1.<>9__1) == null)
					{
						onLoad = (CS$<>8__locals1.<>9__1 = delegate(SpriteAtlas atlas)
						{
							bool flag5 = null != CS$<>8__locals1.image;
							if (flag5)
							{
								CS$<>8__locals1.image.sprite = atlas.GetSprite(CS$<>8__locals1.spriteName);
								AtlasInfo.SetSpriteFromMod(CS$<>8__locals1.image, CS$<>8__locals1.spriteName);
								CS$<>8__locals1.<>4__this.CacheSprite(atlas.name, CS$<>8__locals1.spriteName, CS$<>8__locals1.image.sprite);
								base.<SetImageSpriteOnly>g__AfterSetSprite|0();
							}
							else
							{
								GLog.TagWarn("AtlasInfo", "Failed to set sprite " + CS$<>8__locals1.spriteName + ",can not find target!", Array.Empty<object>());
							}
						});
					}
					this.LoadPacker(key, onLoad);
					return true;
				}
			}
			bool flag4 = AtlasInfo.SetSpriteFromMod(CS$<>8__locals1.image, CS$<>8__locals1.spriteName);
			if (flag4)
			{
				CS$<>8__locals1.<SetImageSpriteOnly>g__AfterSetSprite|0();
				result = true;
			}
			else
			{
				result = false;
			}
		}
		return result;
	}

	// Token: 0x06000245 RID: 581 RVA: 0x0000D92C File Offset: 0x0000BB2C
	private static bool SetSpriteFromMod(CImage image, string spriteName)
	{
		Sprite modSprite = ModManager.GetOverwriteGraphicsSprite(spriteName, image.sprite);
		bool flag = !modSprite;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			image.sprite = modSprite;
			result = true;
		}
		return result;
	}

	// Token: 0x06000246 RID: 582 RVA: 0x0000D968 File Offset: 0x0000BB68
	private static Sprite GetSpriteFromMod(string spriteName, Sprite referenceSprite)
	{
		return ModManager.GetOverwriteGraphicsSprite(spriteName, referenceSprite);
	}

	// Token: 0x06000247 RID: 583 RVA: 0x0000D984 File Offset: 0x0000BB84
	public string GetSprite(string spriteName, Action<Sprite> onGet)
	{
		bool flag = !spriteName.IsNullOrEmpty();
		if (flag)
		{
			foreach (KeyValuePair<string, Dictionary<string, Sprite>> pair in this._runTimeCache)
			{
				Sprite sprite;
				bool flag2 = pair.Value.TryGetValue(spriteName, out sprite);
				if (flag2)
				{
					bool flag3 = null != sprite;
					if (flag3)
					{
						Sprite modeSprite = AtlasInfo.GetSpriteFromMod(spriteName, sprite);
						bool flag4 = modeSprite != null;
						if (flag4)
						{
							sprite = modeSprite;
						}
						Action<Sprite> onGet2 = onGet;
						if (onGet2 != null)
						{
							onGet2(sprite);
						}
						return pair.Key;
					}
					this.LoadPacker(pair.Key, delegate(SpriteAtlas atlas)
					{
						sprite = atlas.GetSprite(spriteName);
						Sprite modeSprite2 = AtlasInfo.GetSpriteFromMod(spriteName, sprite);
						bool flag6 = modeSprite2 != null;
						if (flag6)
						{
							sprite = modeSprite2;
						}
						Action<Sprite> onGet5 = onGet;
						if (onGet5 != null)
						{
							onGet5(sprite);
						}
					});
					return pair.Key;
				}
			}
			Action<Sprite> onGet3 = onGet;
			if (onGet3 != null)
			{
				onGet3(null);
			}
		}
		Sprite modSprite = AtlasInfo.GetSpriteFromMod(spriteName, null);
		bool flag5 = modSprite;
		string empty;
		if (flag5)
		{
			Action<Sprite> onGet4 = onGet;
			if (onGet4 != null)
			{
				onGet4(modSprite);
			}
			empty = string.Empty;
		}
		else
		{
			empty = string.Empty;
		}
		return empty;
	}

	// Token: 0x06000248 RID: 584 RVA: 0x0000DB30 File Offset: 0x0000BD30
	public void GetSpriteWithoutPackerName(string spriteName, Action<Sprite> onGet)
	{
		bool flag2 = !spriteName.IsNullOrEmpty();
		if (flag2)
		{
			bool flag = false;
			foreach (KeyValuePair<string, Dictionary<string, Sprite>> pair in this._runTimeCache)
			{
				Sprite sprite;
				bool flag3 = pair.Value.TryGetValue(spriteName, out sprite);
				if (flag3)
				{
					flag = true;
					bool flag4 = null != sprite;
					if (flag4)
					{
						Sprite modeSprite = AtlasInfo.GetSpriteFromMod(spriteName, sprite);
						bool flag5 = modeSprite != null;
						if (flag5)
						{
							sprite = modeSprite;
						}
						Action<Sprite> onGet2 = onGet;
						if (onGet2 != null)
						{
							onGet2(sprite);
						}
						return;
					}
					this.LoadPacker(pair.Key, delegate(SpriteAtlas atlas)
					{
						sprite = atlas.GetSprite(spriteName);
						Sprite modeSprite2 = AtlasInfo.GetSpriteFromMod(spriteName, sprite);
						bool flag8 = modeSprite2 != null;
						if (flag8)
						{
							sprite = modeSprite2;
						}
						this.CacheSprite(atlas.name, spriteName, sprite);
						Action<Sprite> onGet5 = onGet;
						if (onGet5 != null)
						{
							onGet5(sprite);
						}
					});
				}
			}
			Sprite modSprite = AtlasInfo.GetSpriteFromMod(spriteName, null);
			bool flag6 = modSprite;
			if (flag6)
			{
				Action<Sprite> onGet3 = onGet;
				if (onGet3 != null)
				{
					onGet3(modSprite);
				}
			}
			else
			{
				bool flag7 = !flag;
				if (flag7)
				{
					Action<Sprite> onGet4 = onGet;
					if (onGet4 != null)
					{
						onGet4(null);
					}
				}
			}
		}
	}

	// Token: 0x06000249 RID: 585 RVA: 0x0000DCCC File Offset: 0x0000BECC
	public string GetPackerNameBySpriteName(string spName)
	{
		bool flag = spName.IsNullOrEmpty();
		string empty;
		if (flag)
		{
			empty = string.Empty;
		}
		else
		{
			foreach (KeyValuePair<string, Dictionary<string, Sprite>> pair in this._runTimeCache)
			{
				bool flag2 = pair.Value.ContainsKey(spName);
				if (flag2)
				{
					return pair.Key;
				}
			}
			empty = string.Empty;
		}
		return empty;
	}

	// Token: 0x0600024A RID: 586 RVA: 0x0000DD58 File Offset: 0x0000BF58
	private void CacheSprite(string packerName, string spriteName, Sprite sprite)
	{
		Dictionary<string, Sprite> map;
		bool flag = this._runTimeCache.TryGetValue(packerName, out map) && null == map[spriteName];
		if (flag)
		{
			map[spriteName] = sprite;
		}
	}

	// Token: 0x0600024B RID: 587 RVA: 0x0000DD94 File Offset: 0x0000BF94
	public SpriteAtlas GetLoadedPacker(string packerName)
	{
		bool flag = packerName.IsNullOrEmpty();
		SpriteAtlas result;
		if (flag)
		{
			result = null;
		}
		else
		{
			SpriteAtlas packer;
			this._loadedPackers.TryGetValue(packerName, out packer);
			result = packer;
		}
		return result;
	}

	// Token: 0x04000114 RID: 276
	public static AtlasInfo Instance;

	// Token: 0x04000115 RID: 277
	public static readonly string FilePath = "GameAtlas/AtlasInfos";

	// Token: 0x04000116 RID: 278
	public const string PackersPath = "GameAtlas/Packers/";

	// Token: 0x04000117 RID: 279
	private readonly List<string> _constantPackers = new List<string>();

	// Token: 0x04000118 RID: 280
	public AtlasInfo.PackerDetail[] AllPackerInfos;

	// Token: 0x04000119 RID: 281
	private Dictionary<string, Dictionary<string, Sprite>> _runTimeCache;

	// Token: 0x0400011A RID: 282
	private Dictionary<string, SpriteAtlas> _loadedPackers;

	// Token: 0x0400011B RID: 283
	private List<string> _canUnloadList;

	// Token: 0x020010B2 RID: 4274
	[Serializable]
	public class PackerDetail
	{
		// Token: 0x04009424 RID: 37924
		public string PackerName;

		// Token: 0x04009425 RID: 37925
		public List<string> SpriteNames;

		// Token: 0x04009426 RID: 37926
		public List<Sprite> SpriteList;
	}
}
