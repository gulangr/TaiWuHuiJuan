using System;
using System.Collections.Generic;
using System.IO;

namespace FrameWork.ExternalTexture
{
	// Token: 0x02001051 RID: 4177
	public class PathKeyTextureGroup : BaseTextureGroup
	{
		// Token: 0x0600BE88 RID: 48776 RVA: 0x00565CD4 File Offset: 0x00563ED4
		public override void Init()
		{
			bool flag = this._texturePathMap.Count > 0 || this._texturesCacheMap.Count > 0;
			if (flag)
			{
				throw new Exception("Texture Map is not empty; group need to be disposed before initializing again.");
			}
			List<string> files = new List<string>(Directory.GetFiles(this.TextureDir, "*.png", SearchOption.AllDirectories));
			files.AddRange(Directory.GetFiles(this.TextureDir, "*.jpg", SearchOption.AllDirectories));
			for (int i = 0; i < files.Count; i++)
			{
				string filePath = files[i];
				string relativePath = filePath.Substring(this.TextureDir.Length + 1);
				relativePath = relativePath.Substring(0, relativePath.LastIndexOf('.'));
				this._texturePathMap.Add(relativePath.PathFix(), filePath);
			}
		}

		// Token: 0x0600BE89 RID: 48777 RVA: 0x00565D9C File Offset: 0x00563F9C
		public override void Dispose()
		{
			this._texturePathMap.Clear();
			this._texturesCacheMap.Clear();
		}

		// Token: 0x0600BE8A RID: 48778 RVA: 0x00565DB7 File Offset: 0x00563FB7
		public PathKeyTextureGroup(string textureDir) : base(PathKeyTextureGroup.RemoveSeparator(textureDir))
		{
		}

		// Token: 0x0600BE8B RID: 48779 RVA: 0x00565DC8 File Offset: 0x00563FC8
		private static string RemoveSeparator(string path)
		{
			bool flag = path.EndsWith("/") || path.EndsWith("\\");
			string result;
			if (flag)
			{
				result = path.Substring(0, path.Length - 1);
			}
			else
			{
				result = path;
			}
			return result;
		}
	}
}
