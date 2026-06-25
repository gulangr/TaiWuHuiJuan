using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;

namespace FrameWork.ExternalTexture
{
	// Token: 0x02001050 RID: 4176
	public class NameKeyTextureGroup : BaseTextureGroup
	{
		// Token: 0x0600BE84 RID: 48772 RVA: 0x00565BD8 File Offset: 0x00563DD8
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
				string fileName = Path.GetFileNameWithoutExtension(filePath);
				bool flag2 = this._texturePathMap.ContainsKey(fileName);
				if (!flag2)
				{
					this._texturePathMap.Add(fileName, filePath);
				}
			}
		}

		// Token: 0x0600BE85 RID: 48773 RVA: 0x00565C90 File Offset: 0x00563E90
		public override void Dispose()
		{
			this._texturePathMap.Clear();
			this._texturesCacheMap.Clear();
		}

		// Token: 0x0600BE86 RID: 48774 RVA: 0x00565CAC File Offset: 0x00563EAC
		[Pure]
		public ICollection<string> GetAllTextureNames()
		{
			return this._texturePathMap.Keys;
		}

		// Token: 0x0600BE87 RID: 48775 RVA: 0x00565CC9 File Offset: 0x00563EC9
		public NameKeyTextureGroup(string textureDir) : base(textureDir)
		{
		}
	}
}
