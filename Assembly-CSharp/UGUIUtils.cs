using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020000D6 RID: 214
public static class UGUIUtils
{
	// Token: 0x060007A7 RID: 1959 RVA: 0x00035A90 File Offset: 0x00033C90
	public static void UpdateLightFlowShader(Graphic graphic)
	{
		bool flag = null == graphic;
		if (!flag)
		{
			Material mat = graphic.material;
			bool flag2 = mat.shader.name != "ConchShip/UIEffect/LightFlow";
			if (!flag2)
			{
				bool flag3 = graphic is CImage;
				if (flag3)
				{
					CImage image = graphic as CImage;
					Sprite sprite = image.sprite;
					bool flag4 = sprite == null;
					if (!flag4)
					{
						bool isPlaying = Application.isPlaying;
						if (isPlaying)
						{
							graphic.material = new Material(graphic.material);
							graphic.material.name = "LightFlow_" + graphic.GetInstanceID().ToString();
						}
						float texWidth = sprite.rect.width;
						float texHeight = sprite.rect.height;
						bool flag5 = null != sprite.texture;
						if (flag5)
						{
							texWidth = (float)sprite.texture.width;
							texHeight = (float)sprite.texture.height;
						}
						Vector4 spriteInfo = new Vector4(sprite.textureRect.x, sprite.textureRect.y, sprite.textureRect.width, sprite.textureRect.height);
						graphic.material.SetFloat("_TexWidth", texWidth);
						graphic.material.SetFloat("_TexHeight", texHeight);
						graphic.material.SetVector("_SpriteInfo", spriteInfo);
					}
				}
				else
				{
					bool flag6 = graphic is CRawImage;
					if (flag6)
					{
						CRawImage rawImage = graphic as CRawImage;
						Texture texture = rawImage.texture;
						bool flag7 = null == texture;
						if (!flag7)
						{
							bool isPlaying2 = Application.isPlaying;
							if (isPlaying2)
							{
								graphic.material = new Material(graphic.material);
								graphic.material.name = "LightFlow_" + graphic.GetInstanceID().ToString();
							}
							float texWidth2 = (float)texture.width;
							float texHeight2 = (float)texture.height;
							Vector4 spriteInfo2 = new Vector4(0f, 0f, texWidth2, texHeight2);
							graphic.material.SetFloat("_TexWidth", texWidth2);
							graphic.material.SetFloat("_TexHeight", texHeight2);
							graphic.material.SetVector("_SpriteInfo", spriteInfo2);
						}
					}
				}
			}
		}
	}

	// Token: 0x060007A8 RID: 1960 RVA: 0x00035D08 File Offset: 0x00033F08
	public static bool IsScreenAreaInteract()
	{
		bool flag = null != UIManager.Instance;
		if (flag)
		{
			Vector2 screenMousePos = UIManager.Instance.UiCamera.ScreenToViewportPoint(Input.mousePosition);
			bool flag2 = screenMousePos.x < 0f || screenMousePos.x > 1f || screenMousePos.y < 0f || screenMousePos.y > 1f;
			if (flag2)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x040007C4 RID: 1988
	public static readonly Dictionary<string, Action<Graphic>> MatUpdateDic = new Dictionary<string, Action<Graphic>>
	{
		{
			"ConchShip/UIEffect/LightFlow",
			new Action<Graphic>(UGUIUtils.UpdateLightFlowShader)
		}
	};
}
