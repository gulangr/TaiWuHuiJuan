using System;
using TMPro;
using UnityEngine;

namespace BetterSearchFrontend.Ui
{
	// Token: 0x0200000D RID: 13
	internal sealed class BetterSearchSkin
	{
		// Token: 0x06000063 RID: 99 RVA: 0x00004534 File Offset: 0x00002734
		public static BetterSearchSkin CreateFallback()
		{
			Sprite sprite = BetterSearchSkin.CreateSprite("BetterSearchPanel", Color.white);
			return new BetterSearchSkin
			{
				TextColor = new Color(0.93f, 0.87f, 0.72f, 1f),
				DropdownColor = new Color(0.115f, 0.098f, 0.078f, 0.98f),
				ScrollbarColor = new Color(0.52f, 0.35f, 0.11f, 1f),
				DropdownSprite = sprite,
				DropdownListSprite = sprite,
				DropdownItemSprite = sprite,
				DropdownLineSprite = sprite,
				ScrollbarSprite = sprite
			};
		}

		// Token: 0x06000064 RID: 100 RVA: 0x000045D8 File Offset: 0x000027D8
		private static Sprite CreateSprite(string name, Color color)
		{
			Texture2D texture2D = new Texture2D(4, 4, 4, false);
			Color[] array = new Color[16];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = color;
			}
			texture2D.SetPixels(array);
			texture2D.Apply();
			texture2D.name = name;
			texture2D.hideFlags = 61;
			Sprite sprite = Sprite.Create(texture2D, new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height), new Vector2(0.5f, 0.5f), 4f);
			sprite.name = name;
			sprite.hideFlags = 61;
			return sprite;
		}

		// Token: 0x04000032 RID: 50
		public TMP_FontAsset Font;

		// Token: 0x04000033 RID: 51
		public Color TextColor;

		// Token: 0x04000034 RID: 52
		public Color DropdownColor;

		// Token: 0x04000035 RID: 53
		public Color ScrollbarColor;

		// Token: 0x04000036 RID: 54
		public Sprite DropdownSprite;

		// Token: 0x04000037 RID: 55
		public Sprite DropdownListSprite;

		// Token: 0x04000038 RID: 56
		public Sprite DropdownItemSprite;

		// Token: 0x04000039 RID: 57
		public Sprite DropdownLineSprite;

		// Token: 0x0400003A RID: 58
		public Sprite ScrollbarSprite;
	}
}
