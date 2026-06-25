using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace FrameWork.ExternalTexture
{
	// Token: 0x02001053 RID: 4179
	public class TextureCenter : ISingletonInit, IDisposable
	{
		// Token: 0x0600BE8E RID: 48782 RVA: 0x00565EA0 File Offset: 0x005640A0
		public void LoadTextureGroupFromPath<T>(string key, string path) where T : BaseTextureGroup
		{
			bool flag = string.IsNullOrEmpty(path);
			if (flag)
			{
				throw new Exception("texture directory to be loaded is empty string.");
			}
			bool flag2 = !Directory.Exists(path);
			if (!flag2)
			{
				BaseTextureGroup currGroup;
				bool flag3 = this._textureGroupMap.TryGetValue(key, out currGroup) && currGroup.TextureDir == path;
				if (flag3)
				{
					currGroup.Dispose();
					currGroup.Init();
				}
				else
				{
					BaseTextureGroup textureGroup = Activator.CreateInstance(typeof(T), new object[]
					{
						path
					}) as BaseTextureGroup;
					textureGroup.Init();
					this._textureGroupMap[key] = textureGroup;
				}
			}
		}

		// Token: 0x0600BE8F RID: 48783 RVA: 0x00565F3C File Offset: 0x0056413C
		public void UpdateTextureGroup(string groupKey)
		{
			BaseTextureGroup textureGroup = this._textureGroupMap[groupKey];
			textureGroup.Dispose();
			textureGroup.Init();
		}

		// Token: 0x0600BE90 RID: 48784 RVA: 0x00565F68 File Offset: 0x00564168
		public BaseTextureGroup GetTextureGroup(string groupKey)
		{
			return this._textureGroupMap[groupKey];
		}

		// Token: 0x0600BE91 RID: 48785 RVA: 0x00565F88 File Offset: 0x00564188
		public bool TryGetTextureGroup(string groupKey, out BaseTextureGroup textureGroup)
		{
			return this._textureGroupMap.TryGetValue(groupKey, out textureGroup);
		}

		// Token: 0x0600BE92 RID: 48786 RVA: 0x00565FA8 File Offset: 0x005641A8
		public void UpdateAllTextureGroups()
		{
			foreach (BaseTextureGroup textureGroup in this._textureGroupMap.Values)
			{
				textureGroup.Dispose();
				textureGroup.Init();
			}
		}

		// Token: 0x0600BE93 RID: 48787 RVA: 0x0056600C File Offset: 0x0056420C
		public Sprite GetOrCreateSprite(Texture2D texture, Sprite referenceSprite)
		{
			return this._spriteCache.GetOrCreateSprite(texture, referenceSprite);
		}

		// Token: 0x0600BE94 RID: 48788 RVA: 0x0056602B File Offset: 0x0056422B
		public void Init()
		{
			this._textureGroupMap = new Dictionary<string, BaseTextureGroup>();
		}

		// Token: 0x0600BE95 RID: 48789 RVA: 0x0056603C File Offset: 0x0056423C
		public void Dispose()
		{
			foreach (BaseTextureGroup group in this._textureGroupMap.Values)
			{
				group.Dispose();
			}
			this._textureGroupMap.Clear();
		}

		// Token: 0x04009253 RID: 37459
		private Dictionary<string, BaseTextureGroup> _textureGroupMap;

		// Token: 0x04009254 RID: 37460
		private readonly SpriteCache _spriteCache = new SpriteCache();
	}
}
