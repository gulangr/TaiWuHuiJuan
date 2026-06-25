using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace FrameWork
{
	// Token: 0x02000FE0 RID: 4064
	public class ExternalTextureFolderManager
	{
		// Token: 0x0600B9A4 RID: 47524 RVA: 0x00549D0B File Offset: 0x00547F0B
		public ExternalTextureFolderManager(string folderPath)
		{
			this._folderPath = folderPath;
			this._loadedTextureDic = new Dictionary<string, ExternalTextureInfo>();
		}

		// Token: 0x0600B9A5 RID: 47525 RVA: 0x00549D28 File Offset: 0x00547F28
		public void UpdateTexture()
		{
			this._cachedSpriteDic = new Dictionary<string, Sprite>();
			bool flag = string.IsNullOrEmpty(this._folderPath);
			if (!flag)
			{
				List<string> files = new List<string>(Directory.GetFiles(this._folderPath, "*.png", SearchOption.AllDirectories));
				files.AddRange(Directory.GetFiles(this._folderPath, "*.jpg", SearchOption.AllDirectories));
				for (int i = 0; i < files.Count; i++)
				{
					string filePath = files[i];
					string fileName = Path.GetFileNameWithoutExtension(filePath);
					FileInfo fileInfo = new FileInfo(filePath);
					ExternalTextureInfo textureInfo;
					bool flag2 = !this._loadedTextureDic.TryGetValue(fileName, out textureInfo);
					if (flag2)
					{
						textureInfo = new ExternalTextureInfo();
						textureInfo.LastWriteTimeUtc = DateTime.MinValue;
						this._loadedTextureDic.Add(fileName, textureInfo);
					}
					textureInfo.FilePath = filePath;
					textureInfo.TextureName = fileName;
					bool flag3 = fileInfo.LastWriteTimeUtc > textureInfo.LastWriteTimeUtc;
					if (flag3)
					{
						textureInfo.LastWriteTimeUtc = fileInfo.LastWriteTimeUtc;
						using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
						{
							byte[] buffer = new byte[stream.Length];
							stream.Read(buffer, 0, buffer.Length);
							bool flag4 = null == textureInfo.Texture;
							if (flag4)
							{
								textureInfo.Texture = new Texture2D(1024, 256);
							}
							textureInfo.Texture.name = fileName;
							bool flag5 = !textureInfo.Texture.LoadImage(buffer);
							if (flag5)
							{
								this._loadedTextureDic.Remove(fileName);
							}
						}
					}
				}
				List<string> removeList = new List<string>();
				using (Dictionary<string, ExternalTextureInfo>.Enumerator enumerator = this._loadedTextureDic.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						KeyValuePair<string, ExternalTextureInfo> pair = enumerator.Current;
						bool flag6 = files.Find((string e) => pair.Value.FilePath == e) == null;
						if (flag6)
						{
							removeList.Add(pair.Key);
						}
					}
				}
				removeList.ForEach(delegate(string e)
				{
					this._loadedTextureDic.Remove(e);
				});
			}
		}

		// Token: 0x0600B9A6 RID: 47526 RVA: 0x00549F7C File Offset: 0x0054817C
		public Texture2D GetTexture(string name)
		{
			ExternalTextureInfo info;
			bool flag = this._loadedTextureDic.TryGetValue(name, out info);
			Texture2D result;
			if (flag)
			{
				result = info.Texture;
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x0600B9A7 RID: 47527 RVA: 0x00549FAC File Offset: 0x005481AC
		public Sprite GetSprite(string name)
		{
			Sprite sprite;
			bool flag = this._cachedSpriteDic.TryGetValue(name, out sprite);
			Sprite result;
			if (flag)
			{
				result = sprite;
			}
			else
			{
				ExternalTextureInfo info;
				bool flag2 = this._loadedTextureDic.TryGetValue(name, out info);
				if (flag2)
				{
					Texture2D texture = info.Texture;
					sprite = Sprite.Create(texture, new Rect(0f, 0f, (float)texture.width, (float)texture.height), Vector2.one * 0.5f);
					sprite.name = name;
					result = sprite;
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		// Token: 0x0600B9A8 RID: 47528 RVA: 0x0054A034 File Offset: 0x00548234
		public List<Texture2D> GetAllTextures()
		{
			Dictionary<string, ExternalTextureInfo> loadedTextureDic = this._loadedTextureDic;
			IEnumerable<Texture2D> enumerable;
			if (loadedTextureDic == null)
			{
				enumerable = null;
			}
			else
			{
				enumerable = from e in loadedTextureDic.Values
				select e.Texture;
			}
			return new List<Texture2D>(enumerable ?? Array.Empty<Texture2D>());
		}

		// Token: 0x0600B9A9 RID: 47529 RVA: 0x0054A08C File Offset: 0x0054828C
		public List<Sprite> GetAllSprites()
		{
			List<Sprite> list = new List<Sprite>();
			foreach (KeyValuePair<string, ExternalTextureInfo> pair in this._loadedTextureDic)
			{
				list.Add(this.GetSprite(pair.Key));
			}
			return list;
		}

		// Token: 0x0600B9AA RID: 47530 RVA: 0x0054A0FC File Offset: 0x005482FC
		public bool HasTexture()
		{
			return this._loadedTextureDic.Count > 0;
		}

		// Token: 0x04008FAE RID: 36782
		private Dictionary<string, ExternalTextureInfo> _loadedTextureDic;

		// Token: 0x04008FAF RID: 36783
		private Dictionary<string, Sprite> _cachedSpriteDic;

		// Token: 0x04008FB0 RID: 36784
		private readonly string _folderPath;
	}
}
