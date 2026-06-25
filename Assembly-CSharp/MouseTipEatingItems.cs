using System;
using FrameWork;
using GameData.Domains.Character;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

// Token: 0x02000293 RID: 659
public class MouseTipEatingItems : MouseTipBase
{
	// Token: 0x17000492 RID: 1170
	// (get) Token: 0x060029F4 RID: 10740 RVA: 0x0013E765 File Offset: 0x0013C965
	protected override bool CanStick
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060029F5 RID: 10741 RVA: 0x0013E768 File Offset: 0x0013C968
	protected unsafe override void Init(ArgumentBox argsBox)
	{
		int charId;
		argsBox.Get("CharId", out charId);
		EatingItems eatingItems;
		argsBox.Get<EatingItems>("EatingItems", out eatingItems);
		int availableSlotCount;
		argsBox.Get("AvailableSlotCount", out availableSlotCount);
		TextMeshProUGUI tipsOn = base.CGet<TextMeshProUGUI>("CanEatTipsOn");
		TextMeshProUGUI tipsOff = base.CGet<TextMeshProUGUI>("CanEatTipsOff");
		RectTransform itemHolder = base.CGet<RectTransform>("ItemHolder");
		tipsOn.gameObject.SetActive(availableSlotCount > 0);
		tipsOff.gameObject.SetActive(availableSlotCount <= 0);
		((availableSlotCount > 0) ? tipsOn : tipsOff).text = LocalStringManager.GetFormat("LK_Can_Eat_Count", availableSlotCount);
		for (int i = 0; i < itemHolder.childCount; i++)
		{
			ItemKey itemKey = (ItemKey)(*(ref eatingItems.ItemKeys.FixedElementField + (IntPtr)i * 8));
			ItemView itemView = itemHolder.GetChild(i).GetComponent<ItemView>();
			itemView.gameObject.SetActive(false);
			bool flag = itemKey.IsValid();
			if (flag)
			{
				ItemDomainMethod.AsyncCall.GetItemDisplayData(null, itemKey, charId, delegate(int offset, RawDataPool rawDataPool)
				{
					ItemDisplayData displayData = new ItemDisplayData();
					Serializer.Deserialize(rawDataPool, offset, ref displayData);
					itemView.SetData(displayData, false, 1, false, true, null, false, true);
					itemView.gameObject.SetActive(true);
				});
			}
		}
	}
}
