using System;
using UnityEngine;
using UnityEngine.Sprites;
using UnityEngine.UI;

// Token: 0x020000A3 RID: 163
[RequireComponent(typeof(CanvasRenderer))]
[RequireComponent(typeof(Image))]
[ExecuteAlways]
public class UIAtlasSpriteUVToMaterial : MonoBehaviour
{
	// Token: 0x060005A9 RID: 1449 RVA: 0x00025B24 File Offset: 0x00023D24
	private void Awake()
	{
		this.m_CanvasRenderer = base.GetComponent<CanvasRenderer>();
		this.m_Image = base.GetComponent<Image>();
		CImage cImage;
		bool flag;
		if (this.m_Image != null)
		{
			cImage = (this.m_Image as CImage);
			flag = (cImage != null);
		}
		else
		{
			flag = false;
		}
		bool flag2 = flag;
		if (flag2)
		{
			cImage.OnUpdateMaterial += this.OnMaterialUpdate;
		}
	}

	// Token: 0x060005AA RID: 1450 RVA: 0x00025B84 File Offset: 0x00023D84
	private void OnEnable()
	{
		bool isStarted = this._isStarted;
		if (isStarted)
		{
			this.UpdateToMaterial();
		}
	}

	// Token: 0x060005AB RID: 1451 RVA: 0x00025BA5 File Offset: 0x00023DA5
	private void Start()
	{
		this.UpdateToMaterial();
	}

	// Token: 0x060005AC RID: 1452 RVA: 0x00025BB0 File Offset: 0x00023DB0
	private void OnDestroy()
	{
		CImage cImage;
		bool flag;
		if (this.m_Image != null)
		{
			cImage = (this.m_Image as CImage);
			flag = (cImage != null);
		}
		else
		{
			flag = false;
		}
		bool flag2 = flag;
		if (flag2)
		{
			cImage.OnUpdateMaterial -= this.OnMaterialUpdate;
		}
	}

	// Token: 0x060005AD RID: 1453 RVA: 0x00025BF8 File Offset: 0x00023DF8
	public void UpdateToMaterial()
	{
		bool flag = !base.enabled;
		if (!flag)
		{
			Sprite sprite = this.m_Image.overrideSprite;
			bool flag2 = sprite != null && this.m_Image.material != this.m_Image.defaultMaterial;
			if (flag2)
			{
				Vector4 uvRect = DataUtility.GetOuterUV(sprite);
				Material material = this.m_CanvasRenderer.GetMaterial();
				bool flag3 = material != null;
				if (flag3)
				{
					material.SetVector(UIAtlasSpriteUVToMaterial.ShaderPropsID_AtlasUVRect, uvRect);
					Debug.Log(string.Format("[PBDebug] Set material {0} _AtlasUVRect to {1}", material.name, uvRect));
				}
			}
		}
	}

	// Token: 0x060005AE RID: 1454 RVA: 0x00025CA1 File Offset: 0x00023EA1
	private void OnMaterialUpdate(Graphic graphic)
	{
		this.UpdateToMaterial();
	}

	// Token: 0x0400049A RID: 1178
	public static readonly int ShaderPropsID_AtlasUVRect = Shader.PropertyToID("_AtlasUVRect");

	// Token: 0x0400049B RID: 1179
	private CanvasRenderer m_CanvasRenderer;

	// Token: 0x0400049C RID: 1180
	private Image m_Image;

	// Token: 0x0400049D RID: 1181
	private bool _isStarted = false;
}
