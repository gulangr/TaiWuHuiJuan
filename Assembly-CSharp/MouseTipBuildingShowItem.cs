using System;
using System.Collections.Generic;
using FrameWork;
using FrameWork.UISystem.Components;
using Game.Components.ListStyleGeneralScroll.Item;
using GameData.Domains.Item;
using TMPro;
using UICommon.Character.Elements;
using UnityEngine;

// Token: 0x020003FB RID: 1019
public class MouseTipBuildingShowItem : MouseTipBase
{
	// Token: 0x17000635 RID: 1589
	// (get) Token: 0x06003D0B RID: 15627 RVA: 0x001EB29F File Offset: 0x001E949F
	protected override bool CanStick
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06003D0C RID: 15628 RVA: 0x001EB2A4 File Offset: 0x001E94A4
	protected override void Init(ArgumentBox argsBox)
	{
		argsBox.Get<List<ItemKey>>("itemKeys", out this._itemKeys);
		sbyte maxCount;
		argsBox.Get("maxCount", out maxCount);
		this.title.text = LanguageKey.LK_Building_ShopItemCount.TrFormat(this._itemKeys.Count, maxCount);
		this.ShowItems();
	}

	// Token: 0x06003D0D RID: 15629 RVA: 0x001EB305 File Offset: 0x001E9505
	private void ShowItems()
	{
		this.soldItemHodler.Rebuild<RectTransform>(this._itemKeys.Count, delegate(RectTransform item, int index)
		{
			ItemKey itemData = this._itemKeys[index];
			bool haveItem = itemData.HasTemplate;
			item.gameObject.SetActive(haveItem);
			bool flag = haveItem;
			if (flag)
			{
				item.GetComponent<RowItemMain>().SetData(itemData);
			}
		});
	}

	// Token: 0x04002BC1 RID: 11201
	[SerializeField]
	private TMP_Text title;

	// Token: 0x04002BC2 RID: 11202
	[SerializeField]
	private TemplatedContainerAssemblyNew soldItemHodler;

	// Token: 0x04002BC3 RID: 11203
	private List<ItemKey> _itemKeys;

	// Token: 0x04002BC4 RID: 11204
	private readonly List<ItemResourceButton> _itemPrefabList = new List<ItemResourceButton>();

	// Token: 0x04002BC5 RID: 11205
	private ItemResourceButton _itemPrefab;

	// Token: 0x04002BC6 RID: 11206
	private Transform _itemHolder;
}
