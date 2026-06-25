using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using GameData.Domains.Character;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using TMPro;
using UnityEngine;

// Token: 0x02000290 RID: 656
public class MouseTipDisassembleItem : MouseTipBase
{
	// Token: 0x17000490 RID: 1168
	// (get) Token: 0x060029E2 RID: 10722 RVA: 0x0013DD38 File Offset: 0x0013BF38
	protected override bool CanStick
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060029E3 RID: 10723 RVA: 0x0013DD3C File Offset: 0x0013BF3C
	protected override void Init(ArgumentBox argsBox)
	{
		ItemDisplayData tool;
		argsBox.Get<ItemDisplayData>("Tool", out tool);
		short durabilityCost;
		argsBox.Get("DurabilityCost", out durabilityCost);
		bool durabilityIsMeet = durabilityCost <= tool.Durability;
		bool isEmptyTool = ItemTemplateHelper.IsEmptyTool(tool.Key.ItemType, tool.Key.TemplateId);
		string color = (!durabilityIsMeet && !isEmptyTool) ? "brightred" : "brightblue";
		string curDurabilityStr = isEmptyTool ? LocalStringManager.Get(LanguageKey.LK_Infinity) : tool.Durability.ToString();
		string durabilityStr = string.Format("{0}-{1}", curDurabilityStr.SetColor(color), durabilityCost).SetColor("pinkyellow");
		string maxDurabilityStr = isEmptyTool ? LocalStringManager.Get(LanguageKey.LK_Infinity) : tool.MaxDurability.ToString();
		string content = LocalStringManager.GetFormat(LanguageKey.LK_Item_Operaiton_Tip_Tool, durabilityStr, maxDurabilityStr);
		base.CGet<TextMeshProUGUI>("ToolDurability").text = content;
		ResourceInts resource;
		bool flag = !argsBox.Get<ResourceInts>("Resource", out resource);
		if (flag)
		{
			resource = default(ResourceInts);
			resource.Initialize();
		}
		Refers[] resourceArray = base.CGet<Refers>("Resource").CGetList<Refers>("Layout").ToArray();
		resourceArray.ForEach(delegate(int i, Refers r)
		{
			int num = resource.Get(i);
			bool show = num > 0;
			resourceArray[i].gameObject.SetActive(show);
			bool flag2 = show;
			if (flag2)
			{
				ResourceTypeItem resourceConfig = Config.ResourceType.Instance[i];
				resourceArray[i].CGet<CImage>("Icon").SetSprite(resourceConfig.Icon, false, null);
				string content2 = LocalStringManager.GetFormat(LanguageKey.LK_Item_Disassemble_Tip_Resource_Name, resourceConfig.Name) + num.ToString().SetColor("pinkyellow");
				resourceArray[i].CGet<TextMeshProUGUI>("Content").text = content2;
			}
			return false;
		});
		List<short> materialIdList2;
		argsBox.Get<List<short>>("CertainMaterial", out materialIdList2);
		Refers[] itemArray = base.CGet<Refers>("CertainMaterial").CGetList<Refers>("ItemView").ToArray();
		bool showHeader2 = false;
		itemArray.ForEach(delegate(int index, Refers item)
		{
			bool show = materialIdList2 != null && materialIdList2.CheckIndex(index) && materialIdList2[index] > -1;
			item.gameObject.SetActive(show);
			ItemView itemView = item as ItemView;
			bool flag2 = itemView && show;
			if (flag2)
			{
				showHeader2 = true;
				ItemKey key = new ItemKey(5, 0, materialIdList2[index], -1);
				itemView.SetData(new ItemDisplayData
				{
					Key = key
				}, false, -1, false, true, null, false, true);
				itemView.CGet<TextMeshProUGUI>("Name").text = ItemTemplateHelper.GetName(key.ItemType, key.TemplateId).SetColor(Colors.Instance.GradeColors[(int)ItemTemplateHelper.GetGrade(key.ItemType, key.TemplateId)]);
			}
			return false;
		});
		base.CGet<GameObject>("CertainHeader").SetActive(showHeader2);
		List<short> materialIdList;
		argsBox.Get<List<short>>("ChanceMaterial", out materialIdList);
		Refers[] itemArray2 = base.CGet<Refers>("ChanceMaterial").CGetList<Refers>("ItemView").ToArray();
		bool showHeader = false;
		itemArray2.ForEach(delegate(int index, Refers item)
		{
			bool show = materialIdList != null && materialIdList.CheckIndex(index) && materialIdList[index] > -1;
			item.gameObject.SetActive(show);
			ItemView itemView = item as ItemView;
			bool flag2 = itemView && show;
			if (flag2)
			{
				showHeader = true;
				ItemKey key = new ItemKey(5, 0, materialIdList[index], -1);
				itemView.SetData(new ItemDisplayData
				{
					Key = key
				}, false, -1, false, true, null, false, true);
				itemView.CGet<TextMeshProUGUI>("Name").text = ItemTemplateHelper.GetName(key.ItemType, key.TemplateId).SetColor(Colors.Instance.GradeColors[(int)ItemTemplateHelper.GetGrade(key.ItemType, key.TemplateId)]);
			}
			return false;
		});
		base.CGet<GameObject>("ChanceHeader").SetActive(showHeader);
	}
}
