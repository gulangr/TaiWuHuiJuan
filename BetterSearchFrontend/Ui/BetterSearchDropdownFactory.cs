using System;
using System.Collections.Generic;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BetterSearchFrontend.Ui
{
	// Token: 0x0200000C RID: 12
	internal static class BetterSearchDropdownFactory
	{
		// Token: 0x06000051 RID: 81 RVA: 0x000038EC File Offset: 0x00001AEC
		public static CDropdown CreateScopeDropdown(Transform parent, List<string> options)
		{
			GameObject gameObject = BetterSearchDropdownFactory.CreateRectObject(parent, "BetterSearchScopeDropdown", new Vector2(128f, 42f));
			Sprite scopeBackgroundSprite = BetterSearchDropdownFactory.ScopeBackgroundSprite;
			CImage targetGraphic = BetterSearchDropdownFactory.AddImage(gameObject, scopeBackgroundSprite, BetterSearchDropdownFactory.GetSpriteTint(scopeBackgroundSprite, BetterSearchDropdownFactory.FallbackScopeBackgroundColor));
			CDropdown cdropdown = gameObject.AddComponent<CDropdown>();
			cdropdown.targetGraphic = targetGraphic;
			TextMeshProUGUI textMeshProUGUI = BetterSearchDropdownFactory.CreateText(gameObject.transform, Vector2.zero, Vector2.zero, 18f, string.Empty);
			textMeshProUGUI.name = "Label";
			textMeshProUGUI.alignment = 514;
			BetterSearchDropdownFactory.Stretch(textMeshProUGUI.rectTransform, BetterSearchDropdownFactory.CaptionOffsetMin, BetterSearchDropdownFactory.CaptionOffsetMax);
			cdropdown.captionText = textMeshProUGUI;
			BetterSearchDropdownFactory.EnsureDropdownArrow(gameObject.transform);
			GameObject gameObject2 = BetterSearchDropdownFactory.CreateDropdownTemplate(gameObject.transform);
			cdropdown.template = gameObject2.GetComponent<RectTransform>();
			TextMeshProUGUI componentInChildren = gameObject2.GetComponentInChildren<Toggle>(true).GetComponentInChildren<TextMeshProUGUI>(true);
			cdropdown.itemText = componentInChildren;
			gameObject2.SetActive(false);
			cdropdown.ClearOptions();
			cdropdown.AddOptions(options);
			cdropdown.SetValueWithoutNotify(0);
			BetterSearchDropdownFactory.NormalizeScopeDropdown(cdropdown);
			return cdropdown;
		}

		// Token: 0x06000052 RID: 82 RVA: 0x000039F4 File Offset: 0x00001BF4
		public static void NormalizeScopeDropdown(CDropdown dropdown)
		{
			if (dropdown == null)
			{
				return;
			}
			RectTransform rectTransform = dropdown.transform as RectTransform;
			if (rectTransform != null)
			{
				rectTransform.sizeDelta = new Vector2(128f, Mathf.Max(34f, rectTransform.sizeDelta.y));
			}
			if (dropdown.captionText != null)
			{
				BetterSearchDropdownFactory.Stretch(dropdown.captionText.rectTransform, BetterSearchDropdownFactory.CaptionOffsetMin, BetterSearchDropdownFactory.CaptionOffsetMax);
			}
			CImage cimage = dropdown.targetGraphic as CImage;
			if (cimage == null)
			{
				cimage = dropdown.GetComponent<CImage>();
			}
			if (cimage != null)
			{
				Sprite scopeBackgroundSprite = BetterSearchDropdownFactory.ScopeBackgroundSprite;
				cimage.sprite = scopeBackgroundSprite;
				cimage.color = BetterSearchDropdownFactory.GetSpriteTint(scopeBackgroundSprite, BetterSearchDropdownFactory.FallbackScopeBackgroundColor);
				cimage.type = 1;
				dropdown.targetGraphic = cimage;
			}
			BetterSearchDropdownFactory.EnsureDropdownArrow(dropdown.transform);
		}

		// Token: 0x06000053 RID: 83 RVA: 0x00003ACC File Offset: 0x00001CCC
		private static RectTransform EnsureDropdownArrow(Transform parent)
		{
			RectTransform rectTransform = parent.Find("Arrow") as RectTransform;
			if (rectTransform == null)
			{
				rectTransform = BetterSearchDropdownFactory.CreateDropdownArrow(parent);
			}
			rectTransform.gameObject.SetActive(true);
			rectTransform.anchorMin = new Vector2(1f, 0.5f);
			rectTransform.anchorMax = new Vector2(1f, 0.5f);
			rectTransform.pivot = new Vector2(1f, 0.5f);
			rectTransform.sizeDelta = new Vector2(16f, 10f);
			rectTransform.anchoredPosition = new Vector2(-10f, 0f);
			rectTransform.localRotation = Quaternion.identity;
			rectTransform.localScale = Vector3.one;
			rectTransform.SetAsLastSibling();
			return rectTransform;
		}

		// Token: 0x06000054 RID: 84 RVA: 0x00003B8C File Offset: 0x00001D8C
		private static RectTransform CreateDropdownArrow(Transform parent)
		{
			GameObject gameObject = BetterSearchDropdownFactory.CreateRectObject(parent, "Arrow", new Vector2(16f, 10f));
			CImage cimage = BetterSearchDropdownFactory.AddImage(gameObject, BetterSearchDropdownFactory.DropdownArrowSprite, BetterSearchDropdownFactory.Skin.TextColor);
			cimage.type = 0;
			cimage.raycastTarget = false;
			return gameObject.GetComponent<RectTransform>();
		}

		// Token: 0x06000055 RID: 85 RVA: 0x00003BDC File Offset: 0x00001DDC
		private static GameObject CreateDropdownTemplate(Transform parent)
		{
			GameObject gameObject = BetterSearchDropdownFactory.CreateRectObject(parent, "Template", new Vector2(0f, 168f));
			gameObject.SetActive(false);
			RectTransform component = gameObject.GetComponent<RectTransform>();
			component.anchorMin = new Vector2(0f, 0f);
			component.anchorMax = new Vector2(1f, 0f);
			component.pivot = new Vector2(0.5f, 1f);
			component.anchoredPosition = Vector2.zero;
			component.sizeDelta = new Vector2(0f, 168f);
			GameObject gameObject2 = BetterSearchDropdownFactory.CreateRectObject(gameObject.transform, "VerticalScrollView", Vector2.zero);
			BetterSearchDropdownFactory.Stretch(gameObject2.GetComponent<RectTransform>(), Vector2.zero, Vector2.zero);
			BetterSearchDropdownFactory.AddImage(gameObject2, BetterSearchDropdownFactory.Skin.DropdownListSprite, BetterSearchDropdownFactory.GetSpriteTint(BetterSearchDropdownFactory.Skin.DropdownListSprite, BetterSearchDropdownFactory.Skin.DropdownColor));
			CScrollRect cscrollRect = gameObject2.AddComponent<CScrollRect>();
			cscrollRect.Direction = 1;
			GameObject gameObject3 = BetterSearchDropdownFactory.CreateRectObject(gameObject2.transform, "Viewport", Vector2.zero);
			RectTransform component2 = gameObject3.GetComponent<RectTransform>();
			BetterSearchDropdownFactory.Stretch(component2, Vector2.zero, new Vector2(-8f, 0f));
			gameObject3.AddComponent<RectMask2D>();
			GameObject gameObject4 = BetterSearchDropdownFactory.CreateRectObject(gameObject3.transform, "Content", new Vector2(0f, 42f));
			RectTransform component3 = gameObject4.GetComponent<RectTransform>();
			component3.anchorMin = new Vector2(0f, 1f);
			component3.anchorMax = new Vector2(1f, 1f);
			component3.pivot = BetterSearchDropdownFactory.TopCenter;
			component3.anchoredPosition = Vector2.zero;
			component3.sizeDelta = new Vector2(0f, 42f);
			GameObject gameObject5 = BetterSearchDropdownFactory.CreateRectObject(gameObject4.transform, "Item", new Vector2(0f, 42f));
			RectTransform component4 = gameObject5.GetComponent<RectTransform>();
			component4.anchorMin = new Vector2(0f, 1f);
			component4.anchorMax = new Vector2(1f, 1f);
			component4.pivot = new Vector2(0.5f, 1f);
			component4.sizeDelta = new Vector2(0f, 42f);
			CImage targetGraphic = BetterSearchDropdownFactory.AddImage(gameObject5, BetterSearchDropdownFactory.Skin.DropdownItemSprite, BetterSearchDropdownFactory.GetSpriteTint(BetterSearchDropdownFactory.Skin.DropdownItemSprite, BetterSearchDropdownFactory.Skin.DropdownColor));
			gameObject5.AddComponent<Toggle>().targetGraphic = targetGraphic;
			LayoutElement layoutElement = gameObject5.AddComponent<LayoutElement>();
			layoutElement.minHeight = 42f;
			layoutElement.preferredHeight = 42f;
			TextMeshProUGUI textMeshProUGUI = BetterSearchDropdownFactory.CreateText(gameObject5.transform, Vector2.zero, Vector2.zero, 18f, string.Empty);
			textMeshProUGUI.name = "Item Label";
			textMeshProUGUI.alignment = 514;
			BetterSearchDropdownFactory.Stretch(textMeshProUGUI.rectTransform, new Vector2(6f, 0f), new Vector2(-6f, 0f));
			GameObject gameObject6 = BetterSearchDropdownFactory.CreateRectObject(gameObject5.transform, "line", new Vector2(0f, 2f));
			RectTransform component5 = gameObject6.GetComponent<RectTransform>();
			component5.anchorMin = new Vector2(0f, 0f);
			component5.anchorMax = new Vector2(1f, 0f);
			component5.pivot = new Vector2(0.5f, 0f);
			component5.anchoredPosition = Vector2.zero;
			component5.sizeDelta = new Vector2(0f, 2f);
			BetterSearchDropdownFactory.AddImage(gameObject6, BetterSearchDropdownFactory.Skin.DropdownLineSprite, BetterSearchDropdownFactory.GetSpriteTint(BetterSearchDropdownFactory.Skin.DropdownLineSprite, new Color(BetterSearchDropdownFactory.Skin.TextColor.r, BetterSearchDropdownFactory.Skin.TextColor.g, BetterSearchDropdownFactory.Skin.TextColor.b, 0.22f))).raycastTarget = false;
			cscrollRect.Viewport = component2;
			cscrollRect.Content = component3;
			cscrollRect.ScrollBar = BetterSearchDropdownFactory.CreateDropdownScrollbar(gameObject2.transform);
			return gameObject;
		}

		// Token: 0x06000056 RID: 86 RVA: 0x00003FA8 File Offset: 0x000021A8
		private static Scrollbar CreateDropdownScrollbar(Transform parent)
		{
			GameObject gameObject = BetterSearchDropdownFactory.CreateRectObject(parent, "VerticalScrollbar", new Vector2(8f, 0f));
			RectTransform component = gameObject.GetComponent<RectTransform>();
			component.anchorMin = new Vector2(1f, 0f);
			component.anchorMax = new Vector2(1f, 1f);
			component.pivot = new Vector2(1f, 0.5f);
			component.anchoredPosition = Vector2.zero;
			component.sizeDelta = new Vector2(8f, 0f);
			BetterSearchDropdownFactory.AddImage(gameObject, BetterSearchDropdownFactory.Skin.ScrollbarSprite, BetterSearchDropdownFactory.GetSpriteTint(BetterSearchDropdownFactory.Skin.ScrollbarSprite, BetterSearchDropdownFactory.Skin.DropdownColor));
			GameObject gameObject2 = BetterSearchDropdownFactory.CreateRectObject(gameObject.transform, "Sliding Area", Vector2.zero);
			BetterSearchDropdownFactory.Stretch(gameObject2.GetComponent<RectTransform>(), new Vector2(1f, 1f), new Vector2(-1f, -1f));
			GameObject gameObject3 = BetterSearchDropdownFactory.CreateRectObject(gameObject2.transform, "Handle", Vector2.zero);
			RectTransform component2 = gameObject3.GetComponent<RectTransform>();
			BetterSearchDropdownFactory.Stretch(component2, Vector2.zero, Vector2.zero);
			CImage targetGraphic = BetterSearchDropdownFactory.AddImage(gameObject3, BetterSearchDropdownFactory.Skin.ScrollbarSprite, BetterSearchDropdownFactory.GetSpriteTint(BetterSearchDropdownFactory.Skin.ScrollbarSprite, BetterSearchDropdownFactory.Skin.ScrollbarColor));
			Scrollbar scrollbar = gameObject.AddComponent<Scrollbar>();
			scrollbar.direction = 3;
			scrollbar.targetGraphic = targetGraphic;
			scrollbar.handleRect = component2;
			scrollbar.size = 1f;
			scrollbar.value = 0f;
			return scrollbar;
		}

		// Token: 0x06000057 RID: 87 RVA: 0x0000411F File Offset: 0x0000231F
		private static TextMeshProUGUI CreateText(Transform parent, Vector2 anchoredPos, Vector2 sizeDelta, float fontSize, string text)
		{
			TextMeshProUGUI textMeshProUGUI = BetterSearchDropdownFactory.CreateRectObject(parent, "Text", sizeDelta).AddComponent<TextMeshProUGUI>();
			BetterSearchDropdownFactory.SetRect(textMeshProUGUI.rectTransform, parent, anchoredPos, sizeDelta);
			BetterSearchDropdownFactory.ApplyText(textMeshProUGUI, fontSize, text, 514);
			return textMeshProUGUI;
		}

		// Token: 0x06000058 RID: 88 RVA: 0x00004150 File Offset: 0x00002350
		private static CImage AddImage(GameObject gameObject, Sprite sprite, Color color)
		{
			CImage cimage = gameObject.GetComponent<CImage>();
			if (cimage == null)
			{
				cimage = gameObject.AddComponent<CImage>();
			}
			cimage.sprite = sprite;
			cimage.color = color;
			cimage.type = 1;
			return cimage;
		}

		// Token: 0x06000059 RID: 89 RVA: 0x0000418C File Offset: 0x0000238C
		private static void ApplyText(TextMeshProUGUI text, float fontSize, string value, TextAlignmentOptions alignment)
		{
			if (BetterSearchDropdownFactory.Skin.Font != null)
			{
				text.font = BetterSearchDropdownFactory.Skin.Font;
			}
			text.text = (value ?? string.Empty);
			text.fontSize = fontSize;
			text.fontStyle = 0;
			text.fontWeight = 400;
			text.color = BetterSearchDropdownFactory.Skin.TextColor;
			text.alignment = alignment;
			text.outlineColor = Color.clear;
			text.outlineWidth = 0f;
			text.characterSpacing = 0f;
			text.wordSpacing = 0f;
			text.enableWordWrapping = false;
			text.raycastTarget = false;
		}

		// Token: 0x0600005A RID: 90 RVA: 0x0000423C File Offset: 0x0000243C
		private static GameObject CreateRectObject(Transform parent, string name, Vector2 sizeDelta)
		{
			GameObject gameObject = new GameObject(name);
			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			rectTransform.pivot = BetterSearchDropdownFactory.Center;
			rectTransform.anchorMin = BetterSearchDropdownFactory.Center;
			rectTransform.anchorMax = BetterSearchDropdownFactory.Center;
			BetterSearchDropdownFactory.SetRect(rectTransform, parent, Vector2.zero, sizeDelta);
			gameObject.layer = 5;
			return gameObject;
		}

		// Token: 0x0600005B RID: 91 RVA: 0x00004289 File Offset: 0x00002489
		private static void SetRect(RectTransform rectTransform, Transform parent, Vector2 anchoredPosition, Vector2 sizeDelta)
		{
			rectTransform.SetParent(parent, false);
			rectTransform.anchoredPosition = anchoredPosition;
			rectTransform.sizeDelta = sizeDelta;
			rectTransform.localRotation = Quaternion.identity;
			rectTransform.localScale = Vector3.one;
		}

		// Token: 0x0600005C RID: 92 RVA: 0x000042B7 File Offset: 0x000024B7
		private static void Stretch(RectTransform rectTransform, Vector2 offsetMin, Vector2 offsetMax)
		{
			rectTransform.anchorMin = Vector2.zero;
			rectTransform.anchorMax = Vector2.one;
			rectTransform.offsetMin = offsetMin;
			rectTransform.offsetMax = offsetMax;
		}

		// Token: 0x0600005D RID: 93 RVA: 0x000042DD File Offset: 0x000024DD
		private static Color GetSpriteTint(Sprite sprite, Color fallbackColor)
		{
			if (sprite == null || (!string.IsNullOrEmpty(sprite.name) && sprite.name.StartsWith("BetterSearch")))
			{
				return fallbackColor;
			}
			return Color.white;
		}

		// Token: 0x17000001 RID: 1
		// (get) Token: 0x0600005E RID: 94 RVA: 0x0000430E File Offset: 0x0000250E
		private static Sprite DropdownArrowSprite
		{
			get
			{
				if (BetterSearchDropdownFactory._dropdownArrowSprite == null)
				{
					BetterSearchDropdownFactory._dropdownArrowSprite = BetterSearchDropdownFactory.CreateDropdownArrowSprite();
				}
				return BetterSearchDropdownFactory._dropdownArrowSprite;
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x0600005F RID: 95 RVA: 0x0000432C File Offset: 0x0000252C
		private static Sprite ScopeBackgroundSprite
		{
			get
			{
				if (BetterSearchDropdownFactory.Skin.DropdownItemSprite != null)
				{
					return BetterSearchDropdownFactory.Skin.DropdownItemSprite;
				}
				if (BetterSearchDropdownFactory.Skin.DropdownListSprite != null)
				{
					return BetterSearchDropdownFactory.Skin.DropdownListSprite;
				}
				return BetterSearchDropdownFactory.Skin.DropdownSprite;
			}
		}

		// Token: 0x06000060 RID: 96 RVA: 0x00004380 File Offset: 0x00002580
		private static Sprite CreateDropdownArrowSprite()
		{
			Texture2D texture2D = new Texture2D(16, 10, 4, false);
			Color32[] array = new Color32[160];
			Color32 color;
			color..ctor(byte.MaxValue, byte.MaxValue, byte.MaxValue, 0);
			Color32 color2;
			color2..ctor(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = color;
			}
			float num = 7.5f;
			for (int j = 0; j < 10; j++)
			{
				int num2 = Mathf.RoundToInt((float)(j + 1) * 0.68f);
				for (int k = 0; k < 16; k++)
				{
					if (Mathf.Abs((float)k - num) <= (float)num2)
					{
						array[j * 16 + k] = color2;
					}
				}
			}
			texture2D.SetPixels32(array);
			texture2D.Apply();
			texture2D.name = "BetterSearchDropdownArrow";
			texture2D.hideFlags = 61;
			Sprite sprite = Sprite.Create(texture2D, new Rect(0f, 0f, 16f, 10f), new Vector2(0.5f, 0.5f), 10f);
			sprite.name = "BetterSearchDropdownArrow";
			sprite.hideFlags = 61;
			return sprite;
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000061 RID: 97 RVA: 0x000044B1 File Offset: 0x000026B1
		private static BetterSearchSkin Skin
		{
			get
			{
				return BetterSearchSkinProvider.Skin;
			}
		}

		// Token: 0x0400002B RID: 43
		public const float ScopeDropdownWidth = 128f;

		// Token: 0x0400002C RID: 44
		private static readonly Vector2 Center = new Vector2(0.5f, 0.5f);

		// Token: 0x0400002D RID: 45
		private static readonly Vector2 TopCenter = new Vector2(0.5f, 1f);

		// Token: 0x0400002E RID: 46
		private static readonly Vector2 CaptionOffsetMin = new Vector2(8f, 0f);

		// Token: 0x0400002F RID: 47
		private static readonly Vector2 CaptionOffsetMax = new Vector2(-30f, 0f);

		// Token: 0x04000030 RID: 48
		private static readonly Color FallbackScopeBackgroundColor = new Color(0.115f, 0.098f, 0.078f, 0.98f);

		// Token: 0x04000031 RID: 49
		private static Sprite _dropdownArrowSprite;
	}
}
