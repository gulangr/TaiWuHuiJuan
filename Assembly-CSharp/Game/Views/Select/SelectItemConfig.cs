using System;
using System.Collections.Generic;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Taiwu;
using TMPro;
using UnityEngine;

namespace Game.Views.Select
{
	// Token: 0x020007A7 RID: 1959
	public class SelectItemConfig
	{
		// Token: 0x17000B81 RID: 2945
		// (get) Token: 0x06005E97 RID: 24215 RVA: 0x002B7C7A File Offset: 0x002B5E7A
		// (set) Token: 0x06005E98 RID: 24216 RVA: 0x002B7C82 File Offset: 0x002B5E82
		public SelectItemRules Rules { get; set; }

		// Token: 0x17000B82 RID: 2946
		// (get) Token: 0x06005E99 RID: 24217 RVA: 0x002B7C8B File Offset: 0x002B5E8B
		// (set) Token: 0x06005E9A RID: 24218 RVA: 0x002B7C93 File Offset: 0x002B5E93
		public SelectItemsCallback Callback { get; set; }

		// Token: 0x17000B83 RID: 2947
		// (get) Token: 0x06005E9B RID: 24219 RVA: 0x002B7C9C File Offset: 0x002B5E9C
		// (set) Token: 0x06005E9C RID: 24220 RVA: 0x002B7CA4 File Offset: 0x002B5EA4
		public ESelectItemColumnType? ColumnFlags { get; set; }

		// Token: 0x17000B84 RID: 2948
		// (get) Token: 0x06005E9D RID: 24221 RVA: 0x002B7CAD File Offset: 0x002B5EAD
		// (set) Token: 0x06005E9E RID: 24222 RVA: 0x002B7CB5 File Offset: 0x002B5EB5
		public IReadOnlyList<ITradeableContent> ExternalItems { get; set; }

		// Token: 0x17000B85 RID: 2949
		// (get) Token: 0x06005E9F RID: 24223 RVA: 0x002B7CBE File Offset: 0x002B5EBE
		// (set) Token: 0x06005EA0 RID: 24224 RVA: 0x002B7CC6 File Offset: 0x002B5EC6
		public IReadOnlyList<ITradeableContent> ExternalWarehouseItems { get; set; }

		// Token: 0x17000B86 RID: 2950
		// (get) Token: 0x06005EA1 RID: 24225 RVA: 0x002B7CCF File Offset: 0x002B5ECF
		// (set) Token: 0x06005EA2 RID: 24226 RVA: 0x002B7CD7 File Offset: 0x002B5ED7
		public IReadOnlyList<ITradeableContent> ExternalTreasuryItems { get; set; }

		// Token: 0x17000B87 RID: 2951
		// (get) Token: 0x06005EA3 RID: 24227 RVA: 0x002B7CE0 File Offset: 0x002B5EE0
		// (set) Token: 0x06005EA4 RID: 24228 RVA: 0x002B7CE8 File Offset: 0x002B5EE8
		public string Title { get; set; } = string.Empty;

		// Token: 0x17000B88 RID: 2952
		// (get) Token: 0x06005EA5 RID: 24229 RVA: 0x002B7CF1 File Offset: 0x002B5EF1
		// (set) Token: 0x06005EA6 RID: 24230 RVA: 0x002B7CF9 File Offset: 0x002B5EF9
		public string SelectedTitle { get; set; } = string.Empty;

		// Token: 0x17000B89 RID: 2953
		// (get) Token: 0x06005EA7 RID: 24231 RVA: 0x002B7D02 File Offset: 0x002B5F02
		// (set) Token: 0x06005EA8 RID: 24232 RVA: 0x002B7D0A File Offset: 0x002B5F0A
		public LanguageKey SelectedToggleKey { get; set; } = LanguageKey.Invalid;

		// Token: 0x17000B8A RID: 2954
		// (get) Token: 0x06005EA9 RID: 24233 RVA: 0x002B7D13 File Offset: 0x002B5F13
		// (set) Token: 0x06005EAA RID: 24234 RVA: 0x002B7D1B File Offset: 0x002B5F1B
		public int MaxSelectCount { get; set; } = 0;

		// Token: 0x17000B8B RID: 2955
		// (get) Token: 0x06005EAB RID: 24235 RVA: 0x002B7D24 File Offset: 0x002B5F24
		// (set) Token: 0x06005EAC RID: 24236 RVA: 0x002B7D2C File Offset: 0x002B5F2C
		public int MinSelectCount { get; set; } = 0;

		// Token: 0x17000B8C RID: 2956
		// (get) Token: 0x06005EAD RID: 24237 RVA: 0x002B7D35 File Offset: 0x002B5F35
		// (set) Token: 0x06005EAE RID: 24238 RVA: 0x002B7D3D File Offset: 0x002B5F3D
		public Func<IReadOnlyList<SelectedItemData>, string> CustomTextGenerator { get; set; }

		// Token: 0x17000B8D RID: 2957
		// (get) Token: 0x06005EAF RID: 24239 RVA: 0x002B7D46 File Offset: 0x002B5F46
		// (set) Token: 0x06005EB0 RID: 24240 RVA: 0x002B7D4E File Offset: 0x002B5F4E
		public Action<IReadOnlyList<SelectedItemData>, TextMeshProUGUI, GameObject> CustomTextSetter { get; set; }

		// Token: 0x17000B8E RID: 2958
		// (get) Token: 0x06005EB1 RID: 24241 RVA: 0x002B7D57 File Offset: 0x002B5F57
		// (set) Token: 0x06005EB2 RID: 24242 RVA: 0x002B7D5F File Offset: 0x002B5F5F
		public Action<IReadOnlyList<SelectedItemData>, TooltipInvoker> CustomTextToolTipSetter { get; set; }

		// Token: 0x17000B8F RID: 2959
		// (get) Token: 0x06005EB3 RID: 24243 RVA: 0x002B7D68 File Offset: 0x002B5F68
		// (set) Token: 0x06005EB4 RID: 24244 RVA: 0x002B7D70 File Offset: 0x002B5F70
		public bool CanClose { get; set; } = true;

		// Token: 0x17000B90 RID: 2960
		// (get) Token: 0x06005EB5 RID: 24245 RVA: 0x002B7D79 File Offset: 0x002B5F79
		// (set) Token: 0x06005EB6 RID: 24246 RVA: 0x002B7D81 File Offset: 0x002B5F81
		public bool ShowSelectedArea { get; set; } = false;

		// Token: 0x17000B91 RID: 2961
		// (get) Token: 0x06005EB7 RID: 24247 RVA: 0x002B7D8A File Offset: 0x002B5F8A
		// (set) Token: 0x06005EB8 RID: 24248 RVA: 0x002B7D92 File Offset: 0x002B5F92
		public bool ShowQuickButton { get; set; } = false;

		// Token: 0x17000B92 RID: 2962
		// (get) Token: 0x06005EB9 RID: 24249 RVA: 0x002B7D9B File Offset: 0x002B5F9B
		// (set) Token: 0x06005EBA RID: 24250 RVA: 0x002B7DA3 File Offset: 0x002B5FA3
		public List<SelectedItemData> InitialSelectedItems { get; set; }

		// Token: 0x17000B93 RID: 2963
		// (get) Token: 0x06005EBB RID: 24251 RVA: 0x002B7DAC File Offset: 0x002B5FAC
		// (set) Token: 0x06005EBC RID: 24252 RVA: 0x002B7DB4 File Offset: 0x002B5FB4
		public HashSet<ItemKey> BannedItemKeys { get; set; }

		// Token: 0x17000B94 RID: 2964
		// (get) Token: 0x06005EBD RID: 24253 RVA: 0x002B7DBD File Offset: 0x002B5FBD
		// (set) Token: 0x06005EBE RID: 24254 RVA: 0x002B7DC5 File Offset: 0x002B5FC5
		public bool AllowEmpty { get; set; } = false;

		// Token: 0x17000B95 RID: 2965
		// (get) Token: 0x06005EBF RID: 24255 RVA: 0x002B7DCE File Offset: 0x002B5FCE
		// (set) Token: 0x06005EC0 RID: 24256 RVA: 0x002B7DD6 File Offset: 0x002B5FD6
		public ESelectItemOperationMode OperationMode { get; set; } = ESelectItemOperationMode.NoPreSelect;

		// Token: 0x17000B96 RID: 2966
		// (get) Token: 0x06005EC1 RID: 24257 RVA: 0x002B7DDF File Offset: 0x002B5FDF
		// (set) Token: 0x06005EC2 RID: 24258 RVA: 0x002B7DE7 File Offset: 0x002B5FE7
		public ESelectItemMode SelectItemMode { get; set; } = ESelectItemMode.ItemSelect;

		// Token: 0x17000B97 RID: 2967
		// (get) Token: 0x06005EC3 RID: 24259 RVA: 0x002B7DF0 File Offset: 0x002B5FF0
		// (set) Token: 0x06005EC4 RID: 24260 RVA: 0x002B7DF8 File Offset: 0x002B5FF8
		public List<int> VisibleMainFilterToggles { get; set; }

		// Token: 0x17000B98 RID: 2968
		// (get) Token: 0x06005EC5 RID: 24261 RVA: 0x002B7E01 File Offset: 0x002B6001
		// (set) Token: 0x06005EC6 RID: 24262 RVA: 0x002B7E09 File Offset: 0x002B6009
		public bool HideSortAndFilter { get; set; } = false;

		// Token: 0x17000B99 RID: 2969
		// (get) Token: 0x06005EC7 RID: 24263 RVA: 0x002B7E12 File Offset: 0x002B6012
		// (set) Token: 0x06005EC8 RID: 24264 RVA: 0x002B7E1A File Offset: 0x002B601A
		public bool IsAllowSameTemplateIdItem { get; set; } = true;

		// Token: 0x17000B9A RID: 2970
		// (get) Token: 0x06005EC9 RID: 24265 RVA: 0x002B7E23 File Offset: 0x002B6023
		// (set) Token: 0x06005ECA RID: 24266 RVA: 0x002B7E2B File Offset: 0x002B602B
		public bool CanSelectLockedItem { get; set; }

		// Token: 0x17000B9B RID: 2971
		// (get) Token: 0x06005ECB RID: 24267 RVA: 0x002B7E34 File Offset: 0x002B6034
		// (set) Token: 0x06005ECC RID: 24268 RVA: 0x002B7E3C File Offset: 0x002B603C
		public bool SplitSelectedAmountIntoSingleEntries { get; set; } = false;

		// Token: 0x17000B9C RID: 2972
		// (get) Token: 0x06005ECD RID: 24269 RVA: 0x002B7E45 File Offset: 0x002B6045
		// (set) Token: 0x06005ECE RID: 24270 RVA: 0x002B7E4D File Offset: 0x002B604D
		public string CustomTextTips { get; set; }

		// Token: 0x17000B9D RID: 2973
		// (get) Token: 0x06005ECF RID: 24271 RVA: 0x002B7E56 File Offset: 0x002B6056
		// (set) Token: 0x06005ED0 RID: 24272 RVA: 0x002B7E5E File Offset: 0x002B605E
		public bool DisableWhenMaxSelected { get; set; } = false;

		// Token: 0x17000B9E RID: 2974
		// (get) Token: 0x06005ED1 RID: 24273 RVA: 0x002B7E67 File Offset: 0x002B6067
		// (set) Token: 0x06005ED2 RID: 24274 RVA: 0x002B7E6F File Offset: 0x002B606F
		public int ResourceMaxValue { get; set; } = -1;

		// Token: 0x06005ED3 RID: 24275 RVA: 0x002B7E78 File Offset: 0x002B6078
		public static SelectItemConfig CreateSingleSelectNoPreSelectConfig(SelectItemRules rules, SelectItemsCallback callback, string title = "", ESelectItemColumnType? columnFlags = null)
		{
			return new SelectItemConfig
			{
				Rules = rules,
				Callback = callback,
				ColumnFlags = columnFlags,
				Title = title,
				MaxSelectCount = 1,
				MinSelectCount = -1,
				ShowSelectedArea = true,
				ShowQuickButton = false,
				OperationMode = ESelectItemOperationMode.NoPreSelect,
				SelectItemMode = ESelectItemMode.RowSelect
			};
		}

		// Token: 0x06005ED4 RID: 24276 RVA: 0x002B7EE0 File Offset: 0x002B60E0
		public static SelectItemConfig CreateSingleSelectConfig(SelectItemRules rules, SelectItemsCallback callback, string title = "", ESelectItemColumnType? columnFlags = null)
		{
			return new SelectItemConfig
			{
				Rules = rules,
				Callback = callback,
				ColumnFlags = columnFlags,
				Title = title,
				MaxSelectCount = 1,
				MinSelectCount = -1,
				ShowSelectedArea = true,
				ShowQuickButton = false,
				OperationMode = ESelectItemOperationMode.NoPreSelect,
				SelectItemMode = ESelectItemMode.ItemSelect
			};
		}

		// Token: 0x06005ED5 RID: 24277 RVA: 0x002B7F48 File Offset: 0x002B6148
		public static SelectItemConfig CreateMultipleSelectConfig(SelectItemRules rules, SelectItemsCallback callback, string title = "", int maxCount = 0, int minCount = -1, ESelectItemColumnType? columnFlags = null)
		{
			return new SelectItemConfig
			{
				Rules = rules,
				Callback = callback,
				ColumnFlags = columnFlags,
				Title = title,
				MaxSelectCount = maxCount,
				MinSelectCount = minCount,
				ShowSelectedArea = true,
				ShowQuickButton = true,
				OperationMode = ESelectItemOperationMode.Slot,
				SelectItemMode = ESelectItemMode.ItemSelect
			};
		}

		// Token: 0x04004189 RID: 16777
		public bool ShowProfessionPreview = false;

		// Token: 0x0400418A RID: 16778
		public string SortKey;

		// Token: 0x0400418B RID: 16779
		public bool HideSourceToggles = false;

		// Token: 0x0400418C RID: 16780
		public bool CheckSameByReferenceOnly = false;
	}
}
