using System;
using System.Collections.Generic;
using System.IO;
using AudioKit;
using GameData.Utilities;
using UnityEngine;

// Token: 0x02000016 RID: 22
public class DlcPackageInfo : ScriptableObject
{
	// Token: 0x17000027 RID: 39
	// (get) Token: 0x060000AA RID: 170 RVA: 0x00004A14 File Offset: 0x00002C14
	public string Identifier
	{
		get
		{
			bool flag = string.IsNullOrEmpty(this.Author);
			if (flag)
			{
				throw new Exception("Dlc identifier create failure: author empty !");
			}
			bool flag2 = this.Id == 0U;
			if (flag2)
			{
				throw new Exception("Dlc identifier create failure: id error !");
			}
			return string.Format("{0}_Dlc_{1}", this.Author, this.Id);
		}
	}

	// Token: 0x17000028 RID: 40
	// (get) Token: 0x060000AB RID: 171 RVA: 0x00004A73 File Offset: 0x00002C73
	public ulong Version
	{
		get
		{
			return BitOperation.PackVersion(this.MajorVersion, this.MinorVersion, this.BuildVersion, this.RevisionVersion);
		}
	}

	// Token: 0x060000AC RID: 172 RVA: 0x00004A94 File Offset: 0x00002C94
	public string GetEventBackFilePath(string eventBackTextureName)
	{
		bool flag = this.EventBackTextures == null || this.EventBackTextures.Count <= 0;
		string result;
		if (flag)
		{
			result = string.Empty;
		}
		else
		{
			bool flag2 = this._runtimeCacheOfEventBack == null;
			if (flag2)
			{
				this._runtimeCacheOfEventBack = new Dictionary<string, string>();
				foreach (string path in this.EventBackTextures)
				{
					string texName = Path.GetFileNameWithoutExtension(path);
					this._runtimeCacheOfEventBack.Add(texName, path);
				}
			}
			string texturePath;
			bool flag3 = !this._runtimeCacheOfEventBack.TryGetValue(eventBackTextureName, out texturePath);
			if (flag3)
			{
				result = string.Empty;
			}
			else
			{
				result = texturePath.PathFix();
			}
		}
		return result;
	}

	// Token: 0x04000054 RID: 84
	[NonSerialized]
	public uint Id;

	// Token: 0x04000055 RID: 85
	public string Name;

	// Token: 0x04000056 RID: 86
	public ushort MajorVersion = 0;

	// Token: 0x04000057 RID: 87
	public ushort MinorVersion = 0;

	// Token: 0x04000058 RID: 88
	public ushort BuildVersion = 0;

	// Token: 0x04000059 RID: 89
	public ushort RevisionVersion = 0;

	// Token: 0x0400005A RID: 90
	[ReadOnly]
	public string Author = "ConchShip";

	// Token: 0x0400005B RID: 91
	[NonSerialized]
	public string ResourceDirectory;

	// Token: 0x0400005C RID: 92
	public float SortingOrder;

	// Token: 0x0400005D RID: 93
	public AtlasInfo AtlasInfo;

	// Token: 0x0400005E RID: 94
	public AudioInfos AudioInfo;

	// Token: 0x0400005F RID: 95
	public List<string> EventBackTextures;

	// Token: 0x04000060 RID: 96
	public List<string> Events;

	// Token: 0x04000061 RID: 97
	private Dictionary<string, string> _runtimeCacheOfEventBack;
}
