using System;
using GameData.Adventure;
using GameData.Adventure.Editor;

// Token: 0x0200017A RID: 378
public class AdventureEditorElementPointLightComponent : AdventureEditorPointLightComponent
{
	// Token: 0x060014FA RID: 5370 RVA: 0x00082237 File Offset: 0x00080437
	public void Setup(AdventureElementSnapshot elementData)
	{
		this._elementData = elementData;
	}

	// Token: 0x060014FB RID: 5371 RVA: 0x00082244 File Offset: 0x00080444
	protected override AdventureLightData GetTargetLightData()
	{
		AdventureElementSnapshot elementData = this._elementData;
		return (elementData != null) ? elementData.LightData : null;
	}

	// Token: 0x060014FC RID: 5372 RVA: 0x00082268 File Offset: 0x00080468
	protected override void ApplyColor(string hex)
	{
		bool flag = this._elementData == null;
		if (!flag)
		{
			AdventureElementSnapshot elementData = this._elementData;
			if (elementData.LightData == null)
			{
				elementData.LightData = new AdventureLightData();
			}
			this._elementData.LightData.ColorInHex = hex;
		}
	}

	// Token: 0x060014FD RID: 5373 RVA: 0x000822B4 File Offset: 0x000804B4
	protected override void ApplyIntensity(float value)
	{
		bool flag = this._elementData == null;
		if (!flag)
		{
			AdventureElementSnapshot elementData = this._elementData;
			if (elementData.LightData == null)
			{
				elementData.LightData = new AdventureLightData();
			}
			this._elementData.LightData.Strength = value;
		}
	}

	// Token: 0x060014FE RID: 5374 RVA: 0x00082300 File Offset: 0x00080500
	protected override void ApplyVirtualZ(float value)
	{
		bool flag = this._elementData == null;
		if (!flag)
		{
			AdventureElementSnapshot elementData = this._elementData;
			if (elementData.LightData == null)
			{
				elementData.LightData = new AdventureLightData();
			}
			this._elementData.LightData.Height = value;
		}
	}

	// Token: 0x04001188 RID: 4488
	private AdventureElementSnapshot _elementData;
}
