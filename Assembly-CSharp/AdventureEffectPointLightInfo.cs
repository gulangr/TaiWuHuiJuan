using System;
using UnityEngine;

// Token: 0x0200014C RID: 332
public class AdventureEffectPointLightInfo : MonoBehaviour
{
	// Token: 0x04000F9F RID: 3999
	public AdventureEffectPointLightInfo.ShapeType Shape = AdventureEffectPointLightInfo.ShapeType.OneByOne;

	// Token: 0x04000FA0 RID: 4000
	public AdventureEffectPointLightInfo.LightMode Mode = AdventureEffectPointLightInfo.LightMode.Uniform;

	// Token: 0x04000FA1 RID: 4001
	public float VirtualZ = 1f;

	// Token: 0x04000FA2 RID: 4002
	[Range(0f, 90f)]
	public float Angle = 45f;

	// Token: 0x04000FA3 RID: 4003
	public float Range = 1f;

	// Token: 0x04000FA4 RID: 4004
	public bool NoRangeClamp;

	// Token: 0x04000FA5 RID: 4005
	public Color LightColor = Color.white;

	// Token: 0x04000FA6 RID: 4006
	public float Intensity = 1f;

	// Token: 0x04000FA7 RID: 4007
	public int FullIntensityRange = 1;

	// Token: 0x04000FA8 RID: 4008
	[Range(0f, 16f)]
	public int Priority = 7;

	// Token: 0x04000FA9 RID: 4009
	public float Duration = 1f;

	// Token: 0x0200122B RID: 4651
	public enum LightMode
	{
		// Token: 0x040099BF RID: 39359
		Uniform,
		// Token: 0x040099C0 RID: 39360
		Smooth
	}

	// Token: 0x0200122C RID: 4652
	public enum ShapeType
	{
		// Token: 0x040099C2 RID: 39362
		OneByOne = 1,
		// Token: 0x040099C3 RID: 39363
		ThreeByThree = 3
	}
}
