using System;
using System.Collections.Generic;
using System.Linq;
using AdventureEditor.Beta;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.Adventure;
using GameData.Utilities;
using Google.Protobuf.Collections;
using TMPro;
using UnityEngine;

// Token: 0x0200018F RID: 399
public class MajorEventCostItemTemplate : MajorEventTemplate<AdventureCostItem>
{
	// Token: 0x1700027B RID: 635
	// (get) Token: 0x0600167B RID: 5755 RVA: 0x0008AA7A File Offset: 0x00088C7A
	public override IList<AdventureCostItem> DataList
	{
		get
		{
			return this._costData.CostItems;
		}
	}

	// Token: 0x0600167C RID: 5756 RVA: 0x0008AA87 File Offset: 0x00088C87
	public override void RefreshAll()
	{
		this._adventureCostDataEditor.RefreshCostItem();
	}

	// Token: 0x0600167D RID: 5757 RVA: 0x0008AA95 File Offset: 0x00088C95
	public override void RefreshData()
	{
		this.RefreshTemplateIds();
		this.noCostToggle.isOn = base.Data.NoCost;
	}

	// Token: 0x0600167E RID: 5758 RVA: 0x0008AAB8 File Offset: 0x00088CB8
	public void EditItem()
	{
		ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>().Set("MultipleChoice", true).Set("SelectType", UI_ItemTemplateSelector.ESelectType.ItemTemplate);
		string key = "InitialSelection";
		RepeatedField<AdventureItemReference> availableItems = base.Data.AvailableItems;
		object obj;
		if (availableItems == null)
		{
			obj = null;
		}
		else
		{
			obj = (from data in availableItems
			select new EditingAdventureData.ItemCostItem
			{
				Item1 = (sbyte)data.Type,
				Item2 = (short)data.TemplateId
			}).ToArray<EditingAdventureData.ItemCostItem>();
		}
		ArgumentBox argBox = argumentBox.SetObject(key, obj ?? Array.Empty<EditingAdventureData.ItemCostItem>()).SetObject("OnConfirm", new UI_ItemTemplateSelector.OnConfirmHandler(delegate(List<EditingAdventureData.ItemCostItem> list)
		{
			base.Data.AvailableItems.Clear();
			foreach (EditingAdventureData.ItemCostItem item in list)
			{
				base.Data.AvailableItems.Add(new AdventureItemReference
				{
					Type = (int)item.Item1,
					TemplateId = (int)item.Item2
				});
			}
			this.RefreshTemplateIds();
		}));
		UIElement.ItemTemplateSelector.SetOnInitArgs(argBox);
		UIManager.Instance.ShowUI(UIElement.ItemTemplateSelector, true);
	}

	// Token: 0x0600167F RID: 5759 RVA: 0x0008AB68 File Offset: 0x00088D68
	public void EditTemplateIds(string str)
	{
		base.Data.AvailableItems.Clear();
		string[] numberStrings = str.Trim(new char[]
		{
			'{',
			'}'
		}).Split(',', StringSplitOptions.None);
		List<int> result = (from n in numberStrings.Select(delegate(string s)
		{
			int num;
			return int.TryParse(s.Trim(), out num) ? new int?(num) : null;
		})
		where n != null
		select n.Value).ToList<int>();
		for (int i = 0; i < result.Count; i += 2)
		{
			AdventureItemReference availableItem = new AdventureItemReference
			{
				Type = result[i],
				TemplateId = result[i + 1]
			};
			base.Data.AvailableItems.Add(availableItem);
		}
	}

	// Token: 0x06001680 RID: 5760 RVA: 0x0008AC68 File Offset: 0x00088E68
	public void RefreshTemplateIds()
	{
		this.templates.text = string.Join("|", base.Data.AvailableItems.Select(delegate(AdventureItemReference x)
		{
			IItemConfig item = ItemConfigHelper.GetConfig((sbyte)x.Type, (short)x.TemplateId);
			return string.Format("<color=#GradeColor_{0}>{1}</color>", item.Grade, item.Name);
		})).ColorReplace();
	}

	// Token: 0x06001681 RID: 5761 RVA: 0x0008ACBF File Offset: 0x00088EBF
	public void SetCostData(ref AdventureCostData costData)
	{
		this._costData = costData;
	}

	// Token: 0x06001682 RID: 5762 RVA: 0x0008ACCA File Offset: 0x00088ECA
	public void SetAdventureCostDataEditor(AdventureCostDataEditor editor)
	{
		this._adventureCostDataEditor = editor;
	}

	// Token: 0x06001683 RID: 5763 RVA: 0x0008ACD4 File Offset: 0x00088ED4
	public void OnEnable()
	{
		this.noCostToggle.onValueChanged.ResetListener(delegate(bool on)
		{
			base.Data.NoCost = on;
		});
	}

	// Token: 0x0400124F RID: 4687
	private AdventureCostData _costData;

	// Token: 0x04001250 RID: 4688
	[SerializeField]
	private TMP_Text templates;

	// Token: 0x04001251 RID: 4689
	[SerializeField]
	private CToggle noCostToggle;

	// Token: 0x04001252 RID: 4690
	private AdventureCostDataEditor _adventureCostDataEditor;
}
