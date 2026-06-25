using System;
using System.Diagnostics;
using System.Text;
using Config;
using FrameWork;
using Game.Views.Building;
using TMPro;
using UnityEngine;

// Token: 0x020001A9 RID: 425
public class BuildingInfoView : Refers
{
	// Token: 0x14000013 RID: 19
	// (add) Token: 0x0600181F RID: 6175 RVA: 0x00094134 File Offset: 0x00092334
	// (remove) Token: 0x06001820 RID: 6176 RVA: 0x0009416C File Offset: 0x0009236C
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<short, int> OnClickBuildingInfoView;

	// Token: 0x06001821 RID: 6177 RVA: 0x000941A4 File Offset: 0x000923A4
	public void UpdateBuildingInfo(short buildingTemplateId, int index, ValueTuple<UI_BuildingManage.EBuildingNotAvailableType, string> result)
	{
		this._index = index;
		this._buildingTemplateId = buildingTemplateId;
		BuildingBlockItem buildingConfig = BuildingBlock.Instance[buildingTemplateId];
		ViewBuildingArea.SetBuildingIcon(base.CGet<CImage>("Icon"), buildingConfig, false, null);
		base.CGet<TextMeshProUGUI>("BuildingName").text = buildingConfig.Name;
		CImage needBlock = base.CGet<CImage>("NeedBlock");
		bool width = buildingConfig.Width == 1;
		bool flag = width;
		if (flag)
		{
			needBlock.SetSprite("building_zhange_0", false, null);
		}
		else
		{
			needBlock.SetSprite("building_zhange_1", false, null);
		}
		base.CGet<CButtonObsolete>("Button").ClearAndAddListener(new Action(this.OnClickSelf));
		bool enabled = result.Item1 == UI_BuildingManage.EBuildingNotAvailableType.None;
		base.CGet<CButtonObsolete>("Button").interactable = true;
		TooltipInvoker mouseTipDisplayer = base.CGet<TooltipInvoker>("MouseTipDisplayer");
		mouseTipDisplayer.enabled = true;
		TooltipInvoker tooltipInvoker = mouseTipDisplayer;
		if (tooltipInvoker.RuntimeParam == null)
		{
			tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
		}
		mouseTipDisplayer.RuntimeParam.Set("arg0", buildingConfig.Name);
		this._stringBuilder.Clear();
		this._stringBuilder.Append(buildingConfig.Desc);
		bool flag2 = !string.IsNullOrEmpty(buildingConfig.FuncDesc);
		if (flag2)
		{
			this._stringBuilder.Append("\n \n").Append(buildingConfig.FuncDesc.SetColor("pinkyellow"));
		}
		mouseTipDisplayer.RuntimeParam.Set("arg1", this._stringBuilder.ToString());
		base.CGet<GameObject>("Mask").SetActive(result.Item1 == UI_BuildingManage.EBuildingNotAvailableType.Locked);
		base.CGet<GameObject>("TipsBack").SetActive(!enabled);
		base.CGet<TextMeshProUGUI>("InvalidTip").text = result.Item2;
	}

	// Token: 0x06001822 RID: 6178 RVA: 0x00094375 File Offset: 0x00092575
	private void OnClickSelf()
	{
		Action<short, int> onClickBuildingInfoView = this.OnClickBuildingInfoView;
		if (onClickBuildingInfoView != null)
		{
			onClickBuildingInfoView(this._buildingTemplateId, this._index);
		}
	}

	// Token: 0x04001366 RID: 4966
	private short _buildingTemplateId;

	// Token: 0x04001367 RID: 4967
	private int _index;

	// Token: 0x04001368 RID: 4968
	private StringBuilder _stringBuilder = new StringBuilder();
}
