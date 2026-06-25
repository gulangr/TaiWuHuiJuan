using System;
using UnityEngine;

namespace FrameWork.UISystem.Components
{
	// Token: 0x0200101F RID: 4127
	internal class AlphaControlTarget
	{
		// Token: 0x0600BD04 RID: 48388 RVA: 0x0055E8C8 File Offset: 0x0055CAC8
		public static AlphaControlTarget CreateByMaterial(Material material)
		{
			Shader shader = material.shader;
			string shaderName = shader.name;
			string text = shaderName;
			string a = text;
			AlphaControlTarget result;
			if (!(a == "ConchShip/UIEffect/Kagerou"))
			{
				if (!(a == "UI/Additive"))
				{
					if (!(a == "UI/Default"))
					{
						if (!(a == "ConchShip/UIEffect/MaskedUVAnim"))
						{
							if (!(a == "Legacy Shaders/Particles/Additive"))
							{
								if (!(a == "ConchShip/UIEffect/MaskedUVAnimAdd"))
								{
									Debug.LogError("Unknown shader: " + shaderName);
									result = null;
								}
								else
								{
									result = new AlphaControlTarget(material, "_Color");
								}
							}
							else
							{
								result = new AlphaControlTarget(material, "_TintColor");
							}
						}
						else
						{
							result = new AlphaControlTarget(material, "_Color");
						}
					}
					else
					{
						result = new AlphaControlTarget(material, "_Color");
					}
				}
				else
				{
					result = new AlphaControlTarget(material, "_Color");
				}
			}
			else
			{
				result = new AlphaControlTarget(material, "_Color");
			}
			return result;
		}

		// Token: 0x0600BD05 RID: 48389 RVA: 0x0055E9AA File Offset: 0x0055CBAA
		public AlphaControlTarget(Material material, string colorPropertyName)
		{
			this._material = material;
			this._colorPropertyName = colorPropertyName;
			this.SetMaxAlphaByCurrentAlpha();
		}

		// Token: 0x0600BD06 RID: 48390 RVA: 0x0055E9C9 File Offset: 0x0055CBC9
		public void SetMaxAlphaByCurrentAlpha()
		{
			this._maxAlpha = this.GetAlpha();
		}

		// Token: 0x0600BD07 RID: 48391 RVA: 0x0055E9D8 File Offset: 0x0055CBD8
		public float GetAlpha()
		{
			Color color = this.GetColor();
			return color.a;
		}

		// Token: 0x0600BD08 RID: 48392 RVA: 0x0055E9F8 File Offset: 0x0055CBF8
		private Color GetColor()
		{
			return this._material.GetColor(this._colorPropertyName);
		}

		// Token: 0x0600BD09 RID: 48393 RVA: 0x0055EA1C File Offset: 0x0055CC1C
		public void SetAlpha(float alpha)
		{
			Color color = this._material.GetColor(this._colorPropertyName);
			this._material.SetColor(this._colorPropertyName, new Color(color.r, color.g, color.b, alpha));
		}

		// Token: 0x0600BD0A RID: 48394 RVA: 0x0055EA66 File Offset: 0x0055CC66
		public void SetAlphaPercent(float percent)
		{
			this.SetAlpha(this._maxAlpha * percent);
		}

		// Token: 0x04009166 RID: 37222
		private Material _material;

		// Token: 0x04009167 RID: 37223
		private float _maxAlpha;

		// Token: 0x04009168 RID: 37224
		private string _colorPropertyName;
	}
}
