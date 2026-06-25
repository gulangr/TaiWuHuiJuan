using System;
using UnityEngine.UI;

// Token: 0x020000CE RID: 206
public class CRawImage : RawImage
{
	// Token: 0x06000710 RID: 1808 RVA: 0x000316DE File Offset: 0x0002F8DE
	protected override void Awake()
	{
		base.Awake();
	}

	// Token: 0x06000711 RID: 1809 RVA: 0x000316E8 File Offset: 0x0002F8E8
	protected override void OnEnable()
	{
		base.OnEnable();
		this.UpdateShader();
	}

	// Token: 0x06000712 RID: 1810 RVA: 0x000316FC File Offset: 0x0002F8FC
	private void UpdateShader()
	{
		string shaderName = this.material.shader.name;
		bool flag = UGUIUtils.MatUpdateDic.ContainsKey(shaderName);
		if (flag)
		{
			UGUIUtils.MatUpdateDic[shaderName](this);
		}
	}

	// Token: 0x06000713 RID: 1811 RVA: 0x00031740 File Offset: 0x0002F940
	public bool SetTexture(string textureName)
	{
		bool flag = string.IsNullOrEmpty(textureName);
		bool result;
		if (flag)
		{
			base.texture = null;
			result = false;
		}
		else
		{
			TextureInfo instance = TextureInfo.Instance;
			result = (instance != null && instance.SetTexture(this, textureName));
		}
		return result;
	}
}
