using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.ExternalTexture
{
	// Token: 0x02001052 RID: 4178
	public class SpriteCache
	{
		// Token: 0x0600BE8C RID: 48780 RVA: 0x00565E10 File Offset: 0x00564010
		public Sprite GetOrCreateSprite(Texture2D texture, Sprite referenceSprite = null)
		{
			Sprite sprite;
			bool flag = this._spriteCache.TryGetValue(texture, out sprite);
			Sprite result;
			if (flag)
			{
				result = sprite;
			}
			else
			{
				sprite = Sprite.Create(texture, new Rect(0f, 0f, (float)texture.width, (float)texture.height), referenceSprite ? referenceSprite.pivot : new Vector2(0.5f, 0.5f));
				this._spriteCache.Add(texture, sprite);
				result = sprite;
			}
			return result;
		}

		// Token: 0x04009252 RID: 37458
		private readonly Dictionary<Texture2D, Sprite> _spriteCache = new Dictionary<Texture2D, Sprite>();
	}
}
