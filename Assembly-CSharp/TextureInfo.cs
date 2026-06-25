using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000047 RID: 71
public class TextureInfo : ScriptableObject
{
	// Token: 0x06000263 RID: 611 RVA: 0x0000E3AC File Offset: 0x0000C5AC
	public static void Init()
	{
		ResLoader.Load<TextureInfo>(TextureInfo.FilePath, delegate(TextureInfo info)
		{
			TextureInfo.Instance = info;
			TextureInfo.Instance.InitSelf();
		}, delegate(string path)
		{
			GLog.TagError("TextureInfo", "Failed to Load TextureInfo from " + path, Array.Empty<object>());
		}, false);
	}

	// Token: 0x06000264 RID: 612 RVA: 0x0000E404 File Offset: 0x0000C604
	public void InitSelf()
	{
		this._runTimeCache = new Dictionary<string, TextureInfo.TextureDetail>();
		bool flag = this.AllTextureInfos != null;
		if (flag)
		{
			foreach (TextureInfo.TextureDetail detail in this.AllTextureInfos)
			{
				bool flag2 = !string.IsNullOrEmpty(detail.TextureName) && !this._runTimeCache.ContainsKey(detail.TextureName);
				if (flag2)
				{
					this._runTimeCache.Add(detail.TextureName, detail);
				}
			}
		}
	}

	// Token: 0x06000265 RID: 613 RVA: 0x0000E488 File Offset: 0x0000C688
	public bool HasTexture(string textureName)
	{
		return this._runTimeCache != null && this._runTimeCache.ContainsKey(textureName);
	}

	// Token: 0x06000266 RID: 614 RVA: 0x0000E4B4 File Offset: 0x0000C6B4
	public bool SetTexture(RawImage rawImage, string textureName)
	{
		bool flag = rawImage == null || string.IsNullOrEmpty(textureName);
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			bool flag2 = rawImage.texture != null && rawImage.texture.name == textureName;
			if (flag2)
			{
				rawImage.enabled = true;
				result = true;
			}
			else
			{
				TextureInfo.TextureDetail detail;
				bool flag3 = this._runTimeCache.TryGetValue(textureName, out detail);
				if (flag3)
				{
					string fullPath = "RemakeResources/Textures/" + detail.RelativePath;
					ResLoader.Load<Texture2D>(fullPath, delegate(Texture2D texture)
					{
						bool flag5 = rawImage != null;
						if (flag5)
						{
							Texture2D modTexture = TextureInfo.GetTextureFromMod(textureName, texture);
							rawImage.texture = (modTexture ?? texture);
							rawImage.enabled = true;
						}
					}, delegate(string path)
					{
						GLog.TagWarn("TextureInfo", "Failed to load texture " + textureName + " from " + path, Array.Empty<object>());
					}, false);
					result = true;
				}
				else
				{
					Texture2D modOnlyTexture = TextureInfo.GetTextureFromMod(textureName, null);
					bool flag4 = modOnlyTexture != null;
					if (flag4)
					{
						rawImage.texture = modOnlyTexture;
						rawImage.enabled = true;
						result = true;
					}
					else
					{
						rawImage.enabled = false;
						result = false;
					}
				}
			}
		}
		return result;
	}

	// Token: 0x06000267 RID: 615 RVA: 0x0000E5EC File Offset: 0x0000C7EC
	public bool GetTexture(string textureName, Action<Texture2D> onGet)
	{
		bool flag = string.IsNullOrEmpty(textureName);
		bool result;
		if (flag)
		{
			Action<Texture2D> onGet2 = onGet;
			if (onGet2 != null)
			{
				onGet2(null);
			}
			result = false;
		}
		else
		{
			TextureInfo.TextureDetail detail;
			bool flag2 = this._runTimeCache.TryGetValue(textureName, out detail);
			if (flag2)
			{
				string fullPath = "RemakeResources/Textures/" + detail.RelativePath;
				ResLoader.Load<Texture2D>(fullPath, delegate(Texture2D texture)
				{
					Texture2D modTexture = TextureInfo.GetTextureFromMod(textureName, texture);
					Action<Texture2D> onGet5 = onGet;
					if (onGet5 != null)
					{
						onGet5(modTexture ?? texture);
					}
				}, delegate(string path)
				{
					GLog.TagWarn("TextureInfo", "Failed to load texture " + textureName + " from " + path, Array.Empty<object>());
					Action<Texture2D> onGet5 = onGet;
					if (onGet5 != null)
					{
						onGet5(null);
					}
				}, false);
				result = true;
			}
			else
			{
				Texture2D modOnlyTexture = TextureInfo.GetTextureFromMod(textureName, null);
				bool flag3 = modOnlyTexture != null;
				if (flag3)
				{
					Action<Texture2D> onGet3 = onGet;
					if (onGet3 != null)
					{
						onGet3(modOnlyTexture);
					}
					result = true;
				}
				else
				{
					Action<Texture2D> onGet4 = onGet;
					if (onGet4 != null)
					{
						onGet4(null);
					}
					result = false;
				}
			}
		}
		return result;
	}

	// Token: 0x06000268 RID: 616 RVA: 0x0000E6D8 File Offset: 0x0000C8D8
	private static Texture2D GetTextureFromMod(string textureName, Texture2D referenceTexture)
	{
		return ModManager.GetOverwriteGraphicsTexture(textureName);
	}

	// Token: 0x04000125 RID: 293
	public static TextureInfo Instance;

	// Token: 0x04000126 RID: 294
	public static readonly string FilePath = "GameAtlas/TextureInfos";

	// Token: 0x04000127 RID: 295
	public const string TexturesBasePath = "RemakeResources/Textures/";

	// Token: 0x04000128 RID: 296
	public string[] ManagedTopDirs;

	// Token: 0x04000129 RID: 297
	public TextureInfo.TextureDetail[] AllTextureInfos;

	// Token: 0x0400012A RID: 298
	private Dictionary<string, TextureInfo.TextureDetail> _runTimeCache;

	// Token: 0x020010C0 RID: 4288
	[Serializable]
	public class TextureDetail
	{
		// Token: 0x04009449 RID: 37961
		public string TextureName;

		// Token: 0x0400944A RID: 37962
		public string RelativePath;
	}
}
