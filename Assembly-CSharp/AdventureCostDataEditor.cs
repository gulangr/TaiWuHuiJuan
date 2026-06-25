using System;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using GameData.Adventure;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000169 RID: 361
public class AdventureCostDataEditor : MonoBehaviour
{
	// Token: 0x06001413 RID: 5139 RVA: 0x0007D934 File Offset: 0x0007BB34
	private void Awake()
	{
		this.costResourceTemplate.Init();
		this.addCostResource.ClearAndAddListener(new Action(this.AddCostResource));
		this.addCostItem.ClearAndAddListener(new Action(this.AddCostItem));
		this.costTimeInput.onEndEdit.AddListener(new UnityAction<string>(this.EditCostTime));
	}

	// Token: 0x06001414 RID: 5140 RVA: 0x0007D99C File Offset: 0x0007BB9C
	private void RefreshCostResource(AdventureCostData cost)
	{
		this.costResourceHolder.Rebuild<MajorEventCostResourceTemplate>(cost.CostResources.Count, delegate(MajorEventCostResourceTemplate refer, int index)
		{
			refer.SetCostData(ref cost);
			refer.SetAdventureCostDataEditor(this);
			refer.RefreshData(index);
		});
	}

	// Token: 0x06001415 RID: 5141 RVA: 0x0007D9E8 File Offset: 0x0007BBE8
	private void RefreshCostItem(AdventureCostData cost)
	{
		this.costItemHolder.Rebuild<MajorEventCostItemTemplate>(cost.CostItems.Count, delegate(MajorEventCostItemTemplate refer, int index)
		{
			refer.SetCostData(ref cost);
			refer.SetAdventureCostDataEditor(this);
			refer.RefreshData(index);
		});
	}

	// Token: 0x06001416 RID: 5142 RVA: 0x0007DA34 File Offset: 0x0007BC34
	private void EditCostTime(string str)
	{
		int time = this._costData.CostTime;
		AdventureMajorEventTool.EditInt(ref time, str, "EditCostTime");
		this._costData.CostTime = time;
	}

	// Token: 0x06001417 RID: 5143 RVA: 0x0007DA69 File Offset: 0x0007BC69
	private void AddCostItem()
	{
		this._costData.CostItems.Add(new AdventureCostItem());
		this.RefreshCostItem(this._costData);
	}

	// Token: 0x06001418 RID: 5144 RVA: 0x0007DA8F File Offset: 0x0007BC8F
	private void AddCostResource()
	{
		this._costData.CostResources.Add(new AdventureCostResource
		{
			Type = 0,
			Value = 0
		});
		this.RefreshCostResource(this._costData);
	}

	// Token: 0x06001419 RID: 5145 RVA: 0x0007DAC5 File Offset: 0x0007BCC5
	public void RefreshCostResource()
	{
		this.RefreshCostResource(this._costData);
	}

	// Token: 0x0600141A RID: 5146 RVA: 0x0007DAD5 File Offset: 0x0007BCD5
	public void RefreshCostItem()
	{
		this.RefreshCostItem(this._costData);
	}

	// Token: 0x0600141B RID: 5147 RVA: 0x0007DAE5 File Offset: 0x0007BCE5
	public void Refresh()
	{
		this.Refresh(ref this._costData);
	}

	// Token: 0x0600141C RID: 5148 RVA: 0x0007DAF8 File Offset: 0x0007BCF8
	public void Refresh(ref AdventureCostData cost)
	{
		this._costData = cost;
		TMP_InputField tmp_InputField = this.costTimeInput;
		AdventureCostData adventureCostData;
		if ((adventureCostData = cost) == null)
		{
			AdventureCostData adventureCostData2;
			cost = (adventureCostData2 = new AdventureCostData());
			adventureCostData = adventureCostData2;
		}
		tmp_InputField.SetTextWithoutNotify(adventureCostData.CostTime.ToString());
		this.RefreshCostItem(cost);
		this.RefreshCostResource(cost);
	}

	// Token: 0x040010E3 RID: 4323
	[SerializeField]
	private TemplatedContainerAssemblyNew costItemHolder;

	// Token: 0x040010E4 RID: 4324
	[SerializeField]
	private TemplatedContainerAssemblyNew costResourceHolder;

	// Token: 0x040010E5 RID: 4325
	[SerializeField]
	private TMP_InputField costTimeInput;

	// Token: 0x040010E6 RID: 4326
	[SerializeField]
	private MajorEventCostItemTemplate costItemTemplate;

	// Token: 0x040010E7 RID: 4327
	[SerializeField]
	private MajorEventCostResourceTemplate costResourceTemplate;

	// Token: 0x040010E8 RID: 4328
	[SerializeField]
	private CButton addCostResource;

	// Token: 0x040010E9 RID: 4329
	[SerializeField]
	private CButton addCostItem;

	// Token: 0x040010EA RID: 4330
	private AdventureCostData _costData;
}
