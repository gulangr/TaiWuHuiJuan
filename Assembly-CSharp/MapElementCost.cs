using System;
using GameData.Domains.Map;
using TMPro;
using UnityEngine;

// Token: 0x020003E8 RID: 1000
public class MapElementCost : MapElementBase
{
	// Token: 0x06003C36 RID: 15414 RVA: 0x001E61E4 File Offset: 0x001E43E4
	public static bool CheckMaybeExist(Location location)
	{
		bool flag = MapElementBase.MapModel.ViewMode == WorldMapModel.EViewMode.Info && MapElementBase.MapModel.CurrentAreaId == location.AreaId;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			bool flag2 = MapElementBase.MapModel.ShowingAreaId != location.AreaId;
			result = (!flag2 && MapElementBase.MapModel.PathContains(location));
		}
		return result;
	}

	// Token: 0x1700061F RID: 1567
	// (get) Token: 0x06003C37 RID: 15415 RVA: 0x001E6248 File Offset: 0x001E4448
	public override EMapLayer Layer
	{
		get
		{
			return EMapLayer.PopulationMarkAndTaiwu;
		}
	}

	// Token: 0x06003C38 RID: 15416 RVA: 0x001E624B File Offset: 0x001E444B
	public override void Scale(float wheel)
	{
		base.ScaleReverse(wheel);
	}

	// Token: 0x06003C39 RID: 15417 RVA: 0x001E6258 File Offset: 0x001E4458
	protected override void OnRefresh()
	{
		int moveCost = MapElementBase.MapModel.GetPathBlockMoveCost(base.BlockLocation);
		bool active = moveCost >= 0;
		bool costEnough = SingletonObject.getInstance<TimeManager>().IsActionPointEnough(moveCost);
		this.SetCostActive(active);
		bool flag = active;
		if (flag)
		{
			this.txtMeshCostTime.text = ((float)moveCost * 0.1f).ToString("F1").SetColor(costEnough ? "pinkyellow" : "grey");
			this.imgCostBack.SetSprite(costEnough ? "map_luxian_3" : "map_luxian_4", false, null);
		}
	}

	// Token: 0x06003C3A RID: 15418 RVA: 0x001E62ED File Offset: 0x001E44ED
	protected override void OnCollect()
	{
		this.SetCostActive(false);
	}

	// Token: 0x06003C3B RID: 15419 RVA: 0x001E62F8 File Offset: 0x001E44F8
	private void SetCostActive(bool inActive)
	{
		this.rectTsImageCost.gameObject.SetActive(inActive);
		this.rectTsTxtCost.gameObject.SetActive(inActive);
	}

	// Token: 0x04002B4A RID: 11082
	[SerializeField]
	private RectTransform rectTsImageCost;

	// Token: 0x04002B4B RID: 11083
	[SerializeField]
	private RectTransform rectTsTxtCost;

	// Token: 0x04002B4C RID: 11084
	[SerializeField]
	private TextMeshProUGUI txtMeshCostTime;

	// Token: 0x04002B4D RID: 11085
	[SerializeField]
	private CImage imgCostBack;
}
