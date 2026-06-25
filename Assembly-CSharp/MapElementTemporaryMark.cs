using System;
using System.Linq;
using DG.Tweening;
using GameData.Domains.Map;

// Token: 0x020003F5 RID: 1013
public class MapElementTemporaryMark : MapElementBase
{
	// Token: 0x06003CEB RID: 15595 RVA: 0x001EA824 File Offset: 0x001E8A24
	public static bool CheckMaybeExist(Location location)
	{
		return location == MapElementBase.MapModel.TemporaryMarkLocation || MapElementBase.MapModel.FindMapBlockMarkLocationList.Any((Location x) => x == location) || MapElementBase.MapModel.TaskPanelMainMarkLocationList.Any((Location x) => x == location);
	}

	// Token: 0x17000632 RID: 1586
	// (get) Token: 0x06003CEC RID: 15596 RVA: 0x001EA895 File Offset: 0x001E8A95
	public override EMapLayer Layer
	{
		get
		{
			return EMapLayer.TemporaryMark;
		}
	}

	// Token: 0x06003CED RID: 15597 RVA: 0x001EA898 File Offset: 0x001E8A98
	public override void Scale(float wheel)
	{
		base.ScaleReverse(wheel);
	}

	// Token: 0x06003CEE RID: 15598 RVA: 0x001EA8A2 File Offset: 0x001E8AA2
	protected override void OnRefresh()
	{
	}

	// Token: 0x06003CEF RID: 15599 RVA: 0x001EA8A5 File Offset: 0x001E8AA5
	protected override void OnCollect()
	{
		this._yoyoCount = 0;
		this.doTweenAnimBound.DORestart();
	}

	// Token: 0x06003CF0 RID: 15600 RVA: 0x001EA8BB File Offset: 0x001E8ABB
	public void OnBoundStep()
	{
	}

	// Token: 0x04002BA8 RID: 11176
	public DOTweenAnimation doTweenAnimBound;

	// Token: 0x04002BA9 RID: 11177
	public CImage imgRipple;

	// Token: 0x04002BAA RID: 11178
	private int _yoyoCount;
}
