using System;
using System.Collections.Generic;
using System.Linq;
using FrameWork.UISystem.UIElements;
using GameData.Adventure;
using TMPro;
using UnityEngine;

// Token: 0x02000190 RID: 400
public class MajorEventCostResourceTemplate : MajorEventTemplate<AdventureCostResource>
{
	// Token: 0x1700027C RID: 636
	// (get) Token: 0x06001687 RID: 5767 RVA: 0x0008ADA4 File Offset: 0x00088FA4
	public override IList<AdventureCostResource> DataList
	{
		get
		{
			return this._costData.CostResources;
		}
	}

	// Token: 0x06001688 RID: 5768 RVA: 0x0008ADB1 File Offset: 0x00088FB1
	public void Init()
	{
		this.cdType.options = (from index in MajorEventCostResourceTemplate.NameKeys
		select new CDropdown.OptionData(index.Tr())).ToList<CDropdown.OptionData>();
	}

	// Token: 0x06001689 RID: 5769 RVA: 0x0008ADED File Offset: 0x00088FED
	public override void RefreshAll()
	{
		this._adventureCostDataEditor.RefreshCostResource();
	}

	// Token: 0x0600168A RID: 5770 RVA: 0x0008ADFB File Offset: 0x00088FFB
	public override void RefreshData()
	{
		this.RefreshType();
		this.RefreshCount();
	}

	// Token: 0x0600168B RID: 5771 RVA: 0x0008AE0C File Offset: 0x0008900C
	public void EditType(int value)
	{
		base.Data.Type = value;
		this.RefreshType();
	}

	// Token: 0x0600168C RID: 5772 RVA: 0x0008AE23 File Offset: 0x00089023
	public void RefreshType()
	{
		this.cdType.SetValueWithoutNotify(base.Data.Type);
	}

	// Token: 0x0600168D RID: 5773 RVA: 0x0008AE40 File Offset: 0x00089040
	public void EditCount(string str)
	{
		int value = base.Data.Value;
		AdventureMajorEventTool.EditInt(ref value, str, "EditCount");
		base.Data.Value = value;
	}

	// Token: 0x0600168E RID: 5774 RVA: 0x0008AE78 File Offset: 0x00089078
	public void RefreshCount()
	{
		this.count.SetTextWithoutNotify(base.Data.Value.ToString());
	}

	// Token: 0x0600168F RID: 5775 RVA: 0x0008AEA4 File Offset: 0x000890A4
	public void SetCostData(ref AdventureCostData costData)
	{
		this._costData = costData;
	}

	// Token: 0x06001690 RID: 5776 RVA: 0x0008AEAF File Offset: 0x000890AF
	public void SetAdventureCostDataEditor(AdventureCostDataEditor editor)
	{
		this._adventureCostDataEditor = editor;
	}

	// Token: 0x04001253 RID: 4691
	private AdventureCostData _costData;

	// Token: 0x04001254 RID: 4692
	[SerializeField]
	private CDropdown cdType;

	// Token: 0x04001255 RID: 4693
	[SerializeField]
	private TMP_InputField count;

	// Token: 0x04001256 RID: 4694
	private static readonly List<LanguageKey> NameKeys = new List<LanguageKey>
	{
		LanguageKey.LK_Resource_Name_Food,
		LanguageKey.LK_Resource_Name_Wood,
		LanguageKey.LK_Resource_Name_Metal,
		LanguageKey.LK_Resource_Name_Jade,
		LanguageKey.LK_Resource_Name_Fabric,
		LanguageKey.LK_Resource_Name_Herb,
		LanguageKey.LK_Resource_Name_Money,
		LanguageKey.LK_Resource_Name_Authority
	};

	// Token: 0x04001257 RID: 4695
	private AdventureCostDataEditor _adventureCostDataEditor;
}
