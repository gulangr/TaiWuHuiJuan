using System;
using Config;
using FrameWork;
using GameData.Domains.Character;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using TMPro;

// Token: 0x020002CB RID: 715
public class MouseTipRepairItem : MouseTipBase
{
	// Token: 0x170004B6 RID: 1206
	// (get) Token: 0x06002B14 RID: 11028 RVA: 0x0014E74E File Offset: 0x0014C94E
	protected override bool CanStick
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06002B15 RID: 11029 RVA: 0x0014E754 File Offset: 0x0014C954
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
		ItemDisplayData item;
		argsBox.Get<ItemDisplayData>("Item", out item);
		bool isSufficient = (float)item.Durability > (float)item.MaxDurability * 0.5f;
		string durabilityStr2 = item.Durability.ToString();
		bool flag = !isSufficient;
		if (flag)
		{
			durabilityStr2 = durabilityStr2.SetColor("brightred");
		}
		base.CGet<TextMeshProUGUI>("Durability").text = LocalStringManager.GetFormat(LanguageKey.LK_Item_Repair_Tip_Durability, durabilityStr2, item.MaxDurability);
		ResourceInts resource;
		bool flag2 = !argsBox.Get<ResourceInts>("Resource", out resource);
		if (flag2)
		{
			resource = default(ResourceInts);
			resource.Initialize();
		}
		ResourceInts needResource;
		bool flag3 = !argsBox.Get<ResourceInts>("NeedResource", out needResource);
		if (flag3)
		{
			needResource = default(ResourceInts);
			needResource.Initialize();
		}
		Refers[] resourceArray = base.CGet<Refers>("Resource").CGetList<Refers>("Layout").ToArray();
		resourceArray.ForEach(delegate(int i, Refers r)
		{
			int num = needResource.Get(i);
			bool show = num > 0;
			resourceArray[i].gameObject.SetActive(show);
			bool flag4 = show;
			if (flag4)
			{
				ResourceTypeItem resourceConfig = Config.ResourceType.Instance[i];
				resourceArray[i].CGet<CImage>("Icon").SetSprite(resourceConfig.Icon, false, null);
				bool curResourceMeet = resource.Get(i) >= needResource.Get(i);
				LanguageKey resourceKey = curResourceMeet ? LanguageKey.LK_Refine_Resource_Require_Meet : LanguageKey.LK_Refine_Resource_Require_Not_Meet;
				string color2 = curResourceMeet ? "brightblue" : "brightred";
				string content2 = LocalStringManager.GetFormat(resourceKey, Config.ResourceType.Instance[i].Name, resource.Get(i).ToString().SetColor(color2), needResource.Get(i));
				resourceArray[i].CGet<TextMeshProUGUI>("Content").text = content2;
			}
			return false;
		});
	}
}
