using System;
using TMPro;
using UnityEngine;

namespace BetterSearchFrontend.Ui
{
	// Token: 0x0200000E RID: 14
	internal static class BetterSearchSkinProvider
	{
		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000066 RID: 102 RVA: 0x00004677 File Offset: 0x00002877
		public static BetterSearchSkin Skin
		{
			get
			{
				if (BetterSearchSkinProvider._skin == null)
				{
					BetterSearchSkinProvider._skin = BetterSearchSkin.CreateFallback();
					BetterSearchSkinProvider.ReadNativeAssets(BetterSearchSkinProvider._skin);
				}
				return BetterSearchSkinProvider._skin;
			}
		}

		// Token: 0x06000067 RID: 103 RVA: 0x0000469C File Offset: 0x0000289C
		private static void ReadNativeAssets(BetterSearchSkin skin)
		{
			skin.Font = BetterSearchSkinProvider.LoadFirstFont();
			skin.DropdownSprite = (BetterSearchSkinProvider.LoadSprite("RemakeResources/UIGraphics5.0/Ui9Common/ui9_btn_dropdown_menu_0") ?? skin.DropdownSprite);
			skin.DropdownListSprite = (BetterSearchSkinProvider.LoadSprite("RemakeResources/UIGraphics5.0/Ui9Common/ui9_sp_btn_screening") ?? skin.DropdownListSprite);
			skin.DropdownItemSprite = (BetterSearchSkinProvider.LoadSprite("RemakeResources/UIGraphics5.0/Ui9Common/ui9_back_menu_bg_0") ?? skin.DropdownItemSprite);
			skin.DropdownLineSprite = (BetterSearchSkinProvider.LoadSprite("RemakeResources/UIGraphics5.0/Ui9Common/ui9_line_horizontal_1") ?? skin.DropdownLineSprite);
			skin.ScrollbarSprite = (BetterSearchSkinProvider.LoadSprite("RemakeResources/UIGraphics5.0/Ui9Common/ui9_btn_scroll_base_0") ?? skin.ScrollbarSprite);
			if (skin.DropdownSprite != null && !BetterSearchSkinProvider.IsFallbackSprite(skin.DropdownSprite))
			{
				skin.DropdownColor = Color.white;
			}
		}

		// Token: 0x06000068 RID: 104 RVA: 0x0000475C File Offset: 0x0000295C
		private static TMP_FontAsset LoadFirstFont()
		{
			for (int i = 0; i < BetterSearchSkinProvider.FontAssetPaths.Length; i++)
			{
				TMP_FontAsset tmp_FontAsset = BetterSearchSkinProvider.LoadAsset<TMP_FontAsset>(BetterSearchSkinProvider.FontAssetPaths[i]);
				if (tmp_FontAsset != null)
				{
					return tmp_FontAsset;
				}
			}
			return null;
		}

		// Token: 0x06000069 RID: 105 RVA: 0x00004794 File Offset: 0x00002994
		private static Sprite LoadSprite(string path)
		{
			return BetterSearchSkinProvider.LoadAsset<Sprite>(path);
		}

		// Token: 0x0600006A RID: 106 RVA: 0x0000479C File Offset: 0x0000299C
		private static T LoadAsset<T>(string path) where T : Object
		{
			T result;
			try
			{
				result = ResLoader.SyncLoad<T>(path);
			}
			catch (Exception)
			{
				result = default(T);
			}
			return result;
		}

		// Token: 0x0600006B RID: 107 RVA: 0x000047D0 File Offset: 0x000029D0
		private static bool IsFallbackSprite(Sprite sprite)
		{
			return sprite == null || (!string.IsNullOrEmpty(sprite.name) && sprite.name.StartsWith("BetterSearch", StringComparison.Ordinal));
		}

		// Token: 0x0400003B RID: 59
		private static readonly string[] FontAssetPaths = new string[]
		{
			"RemakeResources/Fonts/Font SDF GB2312",
			"RemakeResources/Fonts/Font SDF BIG5 Core 1",
			"RemakeResources/Fonts/NotoSerifKR-SDF"
		};

		// Token: 0x0400003C RID: 60
		private const string DropdownSpritePath = "RemakeResources/UIGraphics5.0/Ui9Common/ui9_btn_dropdown_menu_0";

		// Token: 0x0400003D RID: 61
		private const string DropdownListSpritePath = "RemakeResources/UIGraphics5.0/Ui9Common/ui9_sp_btn_screening";

		// Token: 0x0400003E RID: 62
		private const string DropdownItemSpritePath = "RemakeResources/UIGraphics5.0/Ui9Common/ui9_back_menu_bg_0";

		// Token: 0x0400003F RID: 63
		private const string DropdownLineSpritePath = "RemakeResources/UIGraphics5.0/Ui9Common/ui9_line_horizontal_1";

		// Token: 0x04000040 RID: 64
		private const string ScrollbarSpritePath = "RemakeResources/UIGraphics5.0/Ui9Common/ui9_btn_scroll_base_0";

		// Token: 0x04000041 RID: 65
		private static BetterSearchSkin _skin;
	}
}
