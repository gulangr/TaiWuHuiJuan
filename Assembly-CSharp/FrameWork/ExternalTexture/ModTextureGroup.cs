using System;
using System.Collections.Generic;
using GameData.Domains.Mod;
using UnityEngine;

namespace FrameWork.ExternalTexture
{
	// Token: 0x0200104F RID: 4175
	public class ModTextureGroup : NameKeyTextureGroup
	{
		// Token: 0x0600BE7D RID: 48765 RVA: 0x005659E8 File Offset: 0x00563BE8
		public ModTextureGroup() : base(string.Empty)
		{
			this._previewTexturesCacheMap = new Dictionary<ModId, Texture2D>();
			this._detailTexturesCacheMap = new Dictionary<ModId, List<Texture2D>>();
		}

		// Token: 0x0600BE7E RID: 48766 RVA: 0x00565A10 File Offset: 0x00563C10
		public new void Dispose()
		{
			base.Dispose();
			foreach (KeyValuePair<ModId, Texture2D> keyValuePair in this._previewTexturesCacheMap)
			{
				ModId modId;
				Texture2D texture2D2;
				keyValuePair.Deconstruct(out modId, out texture2D2);
				Texture2D value = texture2D2;
				Object.Destroy(value);
			}
			foreach (KeyValuePair<ModId, List<Texture2D>> keyValuePair2 in this._detailTexturesCacheMap)
			{
				ModId modId;
				List<Texture2D> list;
				keyValuePair2.Deconstruct(out modId, out list);
				List<Texture2D> value2 = list;
				foreach (Texture2D texture2D in value2)
				{
					Object.Destroy(texture2D);
				}
			}
			this._previewTexturesCacheMap.Clear();
			this._detailTexturesCacheMap.Clear();
		}

		// Token: 0x0600BE7F RID: 48767 RVA: 0x00565B30 File Offset: 0x00563D30
		public void AddPreviewTextureCache(ModId modId, Texture2D texture)
		{
			this._previewTexturesCacheMap[modId] = texture;
		}

		// Token: 0x0600BE80 RID: 48768 RVA: 0x00565B40 File Offset: 0x00563D40
		public Texture2D GetPreviewTextureCache(ModId modId)
		{
			Texture2D texture;
			return this._previewTexturesCacheMap.TryGetValue(modId, out texture) ? texture : null;
		}

		// Token: 0x0600BE81 RID: 48769 RVA: 0x00565B61 File Offset: 0x00563D61
		public bool HasPreviewTextureCache(ModId modId)
		{
			return this._previewTexturesCacheMap.ContainsKey(modId);
		}

		// Token: 0x0600BE82 RID: 48770 RVA: 0x00565B70 File Offset: 0x00563D70
		public void AddDetailTextureCache(ModId modId, Texture2D texture)
		{
			List<Texture2D> textureList;
			bool flag = !this._detailTexturesCacheMap.TryGetValue(modId, out textureList);
			if (flag)
			{
				textureList = new List<Texture2D>();
				this._detailTexturesCacheMap.Add(modId, textureList);
			}
			textureList.Add(texture);
		}

		// Token: 0x0600BE83 RID: 48771 RVA: 0x00565BB4 File Offset: 0x00563DB4
		public IReadOnlyList<Texture2D> GetDetailTextureCache(ModId modId)
		{
			List<Texture2D> textureList;
			return this._detailTexturesCacheMap.TryGetValue(modId, out textureList) ? textureList : null;
		}

		// Token: 0x04009250 RID: 37456
		private readonly Dictionary<ModId, Texture2D> _previewTexturesCacheMap;

		// Token: 0x04009251 RID: 37457
		private readonly Dictionary<ModId, List<Texture2D>> _detailTexturesCacheMap;
	}
}
