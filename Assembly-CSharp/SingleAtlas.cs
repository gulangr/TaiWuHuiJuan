using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000045 RID: 69
[Serializable]
public class SingleAtlas : ScriptableObject
{
	// Token: 0x06000259 RID: 601 RVA: 0x0000E205 File Offset: 0x0000C405
	public void Init()
	{
		this._runtimeCacheMap = new Dictionary<string, Sprite>();
		this.SpriteList.ForEach(delegate(Sprite e)
		{
			bool flag = null == e;
			if (!flag)
			{
				this._runtimeCacheMap.Add(e.name, e);
				this.Texture = e.texture;
			}
		});
	}

	// Token: 0x0600025A RID: 602 RVA: 0x0000E22C File Offset: 0x0000C42C
	public bool HasSprite(string spriteName)
	{
		bool flag = this._runtimeCacheMap == null;
		if (flag)
		{
			throw new Exception(this.Name + " has not init yet,call Init before use");
		}
		return this._runtimeCacheMap.ContainsKey(spriteName);
	}

	// Token: 0x0600025B RID: 603 RVA: 0x0000E270 File Offset: 0x0000C470
	public Sprite GetSprite(string spriteName)
	{
		Sprite sprite;
		this._runtimeCacheMap.TryGetValue(spriteName, out sprite);
		return sprite;
	}

	// Token: 0x04000121 RID: 289
	public string Name;

	// Token: 0x04000122 RID: 290
	[NonSerialized]
	public Texture2D Texture;

	// Token: 0x04000123 RID: 291
	public List<Sprite> SpriteList;

	// Token: 0x04000124 RID: 292
	[NonSerialized]
	private Dictionary<string, Sprite> _runtimeCacheMap;
}
