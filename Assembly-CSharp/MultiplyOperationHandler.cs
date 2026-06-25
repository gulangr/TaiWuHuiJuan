using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Config;
using GameData.Domains.Character;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Taiwu;

// Token: 0x020001CE RID: 462
public class MultiplyOperationHandler
{
	// Token: 0x06001CC1 RID: 7361 RVA: 0x000C9561 File Offset: 0x000C7761
	public void Clear()
	{
		this.SelectedItemDict.Clear();
		this.SelectedItemOrderedList.Clear();
		this._itemToUsedToolDict.Clear();
		this.UsableToolOrderedList.Clear();
		this.AllUsedToolDict.Clear();
	}

	// Token: 0x06001CC2 RID: 7362 RVA: 0x000C95A0 File Offset: 0x000C77A0
	public Dictionary<ItemDisplayData, short> GetUsedToolDict(ItemDisplayData itemData)
	{
		Dictionary<ItemDisplayData, short> usedToolDict;
		bool flag = !this._itemToUsedToolDict.TryGetValue(itemData, out usedToolDict);
		if (flag)
		{
			usedToolDict = new Dictionary<ItemDisplayData, short>();
			this._itemToUsedToolDict.Add(itemData, usedToolDict);
		}
		return usedToolDict;
	}

	// Token: 0x06001CC3 RID: 7363 RVA: 0x000C95E0 File Offset: 0x000C77E0
	public short GetToolRemainDurability(ItemDisplayData tool)
	{
		int totalUsed = 0;
		foreach (ItemDisplayData itemData in this.SelectedItemOrderedList)
		{
			Dictionary<ItemDisplayData, short> usedToolDict;
			short used;
			bool flag = this._itemToUsedToolDict.TryGetValue(itemData, out usedToolDict) && usedToolDict.TryGetValue(tool, out used);
			if (flag)
			{
				totalUsed += (int)used;
			}
		}
		return Convert.ToInt16((int)tool.Durability - totalUsed);
	}

	// Token: 0x06001CC4 RID: 7364 RVA: 0x000C9670 File Offset: 0x000C7870
	public void RefreshAllUsedToolDict()
	{
		this.AllUsedToolDict.Clear();
		foreach (ItemDisplayData itemData in this.SelectedItemOrderedList)
		{
			Dictionary<ItemDisplayData, short> usedToolDict;
			bool flag = this._itemToUsedToolDict.TryGetValue(itemData, out usedToolDict);
			if (flag)
			{
				foreach (KeyValuePair<ItemDisplayData, short> pair in usedToolDict)
				{
					ItemDisplayData tool = pair.Key;
					short used = pair.Value;
					short toolTotalUsed;
					bool flag2 = !this.AllUsedToolDict.TryGetValue(tool, out toolTotalUsed);
					if (flag2)
					{
						this.AllUsedToolDict.Add(tool, 0);
					}
					toolTotalUsed += used;
					this.AllUsedToolDict[tool] = toolTotalUsed;
				}
			}
		}
	}

	// Token: 0x06001CC5 RID: 7365 RVA: 0x000C977C File Offset: 0x000C797C
	public List<MultiplyOperation> GetOperationList(bool preview = false)
	{
		MultiplyOperationHandler.<>c__DisplayClass9_0 CS$<>8__locals1;
		CS$<>8__locals1.preview = preview;
		CS$<>8__locals1.list = new List<MultiplyOperation>();
		foreach (ItemDisplayData itemData in this.SelectedItemOrderedList)
		{
			int count = this.SelectedItemDict[itemData];
			Dictionary<ItemDisplayData, short> toolDict;
			bool hasTool = this._itemToUsedToolDict.TryGetValue(itemData, out toolDict) && toolDict != null && toolDict.Count > 0;
			bool flag = CS$<>8__locals1.preview || !hasTool;
			if (flag)
			{
				MultiplyOperationHandler.<GetOperationList>g__Add|9_0(itemData, count, ItemKey.Invalid, ItemSourceType.Invalid.ToSbyte(), ref CS$<>8__locals1);
			}
			else
			{
				List<ItemDisplayData> toolKeyList = (from tool in toolDict.Keys
				orderby toolDict[tool] descending
				select tool).ToList<ItemDisplayData>();
				foreach (ItemDisplayData tool2 in toolKeyList)
				{
					sbyte grade = ItemTemplateHelper.GetGrade(itemData.Key.ItemType, itemData.Key.TemplateId);
					CraftToolItem toolConfig = CraftTool.Instance[tool2.Key.TemplateId];
					short durabilityCost = toolConfig.DurabilityCost[(int)grade];
					bool flag2 = durabilityCost > 0;
					int usedCount;
					if (flag2)
					{
						short usedDurability = toolDict[tool2];
						usedCount = (int)((usedDurability % durabilityCost == 0) ? (usedDurability / durabilityCost) : (usedDurability / durabilityCost + 1));
						count -= usedCount;
					}
					else
					{
						usedCount = count;
					}
					MultiplyOperationHandler.<GetOperationList>g__Add|9_0(itemData, usedCount, tool2.Key, tool2.ItemSourceType, ref CS$<>8__locals1);
				}
			}
		}
		return CS$<>8__locals1.list;
	}

	// Token: 0x06001CC7 RID: 7367 RVA: 0x000C99D0 File Offset: 0x000C7BD0
	[CompilerGenerated]
	internal static void <GetOperationList>g__Add|9_0(ItemDisplayData itemData, int count, ItemKey toolKey, sbyte toolItemSourceType, ref MultiplyOperationHandler.<>c__DisplayClass9_0 A_4)
	{
		bool isMiscResource = ItemTemplateHelper.IsMiscResource(itemData.Key.ItemType, itemData.Key.TemplateId);
		bool flag = isMiscResource;
		if (flag)
		{
			MultiplyOperation operation = new MultiplyOperation(itemData.Key, toolKey, count, itemData.ItemSourceType, toolItemSourceType);
			A_4.list.Add(operation);
		}
		else
		{
			Inventory inventory = itemData.GetOperationInventoryFromPool(count, A_4.preview);
			foreach (KeyValuePair<ItemKey, int> keyValuePair in inventory.Items)
			{
				ItemKey itemKey2;
				int num;
				keyValuePair.Deconstruct(out itemKey2, out num);
				ItemKey itemKey = itemKey2;
				int curCount = num;
				MultiplyOperation operation2 = new MultiplyOperation(itemKey, toolKey, curCount, itemData.ItemSourceType, toolItemSourceType);
				A_4.list.Add(operation2);
			}
			ItemDisplayData.ReturnInventoryToPool(inventory);
		}
	}

	// Token: 0x04001657 RID: 5719
	public readonly Dictionary<ItemDisplayData, int> SelectedItemDict = new Dictionary<ItemDisplayData, int>();

	// Token: 0x04001658 RID: 5720
	public readonly List<ItemDisplayData> SelectedItemOrderedList = new List<ItemDisplayData>();

	// Token: 0x04001659 RID: 5721
	private readonly Dictionary<ItemDisplayData, Dictionary<ItemDisplayData, short>> _itemToUsedToolDict = new Dictionary<ItemDisplayData, Dictionary<ItemDisplayData, short>>();

	// Token: 0x0400165A RID: 5722
	public readonly List<ItemDisplayData> UsableToolOrderedList = new List<ItemDisplayData>();

	// Token: 0x0400165B RID: 5723
	public readonly Dictionary<ItemDisplayData, short> AllUsedToolDict = new Dictionary<ItemDisplayData, short>();
}
