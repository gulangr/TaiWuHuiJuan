using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace FrameWork.ExternalTexture
{
	// Token: 0x0200104E RID: 4174
	public class BaseTextureGroup
	{
		// Token: 0x0600BE76 RID: 48758 RVA: 0x0056587D File Offset: 0x00563A7D
		public BaseTextureGroup(string textureDir)
		{
			this.TextureDir = textureDir;
			this._texturePathMap = new Dictionary<string, string>();
			this._texturesCacheMap = new Dictionary<string, Texture2D>();
		}

		// Token: 0x0600BE77 RID: 48759 RVA: 0x005658A4 File Offset: 0x00563AA4
		public virtual void Init()
		{
		}

		// Token: 0x0600BE78 RID: 48760 RVA: 0x005658A7 File Offset: 0x00563AA7
		public virtual void Dispose()
		{
		}

		// Token: 0x0600BE79 RID: 48761 RVA: 0x005658AA File Offset: 0x00563AAA
		public void AddTexture(string key, string path)
		{
			this._texturePathMap[key] = path;
		}

		// Token: 0x0600BE7A RID: 48762 RVA: 0x005658BC File Offset: 0x00563ABC
		public void RemoveTexture(string key)
		{
			Texture2D texture2D;
			bool flag = this._texturesCacheMap.TryGetValue(key, out texture2D);
			if (flag)
			{
				Object.Destroy(texture2D);
				this._texturesCacheMap.Remove(key);
			}
		}

		// Token: 0x0600BE7B RID: 48763 RVA: 0x005658F4 File Offset: 0x00563AF4
		public Texture2D GetTexture(string key)
		{
			bool flag = string.IsNullOrEmpty(key);
			Texture2D result;
			if (flag)
			{
				result = null;
			}
			else
			{
				Texture2D texture2D;
				bool flag2 = this._texturesCacheMap.TryGetValue(key, out texture2D);
				if (flag2)
				{
					result = texture2D;
				}
				else
				{
					string texturePath;
					bool flag3 = this._texturePathMap.TryGetValue(key, out texturePath);
					if (flag3)
					{
						texture2D = BaseTextureGroup.LoadTexture(texturePath);
						bool flag4 = texture2D;
						if (flag4)
						{
							texture2D.name = key;
							this._texturesCacheMap.Add(key, texture2D);
							return texture2D;
						}
					}
					result = null;
				}
			}
			return result;
		}

		// Token: 0x0600BE7C RID: 48764 RVA: 0x00565974 File Offset: 0x00563B74
		protected static Texture2D LoadTexture(string fullPath)
		{
			bool flag = !File.Exists(fullPath);
			Texture2D result;
			if (flag)
			{
				result = null;
			}
			else
			{
				using (FileStream stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
				{
					byte[] buffer = new byte[stream.Length];
					stream.Read(buffer, 0, buffer.Length);
					Texture2D texture2D = new Texture2D(0, 0);
					texture2D.LoadImage(buffer);
					result = texture2D;
				}
			}
			return result;
		}

		// Token: 0x0400924D RID: 37453
		protected Dictionary<string, string> _texturePathMap;

		// Token: 0x0400924E RID: 37454
		protected Dictionary<string, Texture2D> _texturesCacheMap;

		// Token: 0x0400924F RID: 37455
		public readonly string TextureDir;
	}
}
