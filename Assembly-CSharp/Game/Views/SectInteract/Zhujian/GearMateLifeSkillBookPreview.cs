using System;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Item;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using TMPro;
using UnityEngine;

namespace Game.Views.SectInteract.Zhujian
{
	// Token: 0x020009C4 RID: 2500
	public class GearMateLifeSkillBookPreview : MonoBehaviour
	{
		// Token: 0x17000D78 RID: 3448
		// (get) Token: 0x0600793F RID: 31039 RVA: 0x00385EC6 File Offset: 0x003840C6
		public ItemKey ItemKey
		{
			get
			{
				return this._itemData.RealKey;
			}
		}

		// Token: 0x06007940 RID: 31040 RVA: 0x00385ED4 File Offset: 0x003840D4
		public void Refresh(ItemDisplayData itemData, sbyte[] lastProgressRead, sbyte[] curProgressRead, Action<ItemDisplayData> onClickCancel)
		{
			this._itemData = itemData;
			this.itemBack.Set(itemData, false);
			this.textName.text = ItemTemplateHelper.GetName(this.ItemKey.ItemType, this.ItemKey.TemplateId);
			this.buttonCancel.ClearAndAddListener(delegate
			{
				Action<ItemDisplayData> onClickCancel2 = onClickCancel;
				if (onClickCancel2 != null)
				{
					onClickCancel2(this._itemData);
				}
			});
			for (int i = 0; i < this.chapterArray.Length; i++)
			{
				sbyte curProgress = curProgressRead[i];
				sbyte lastProgress = lastProgressRead[i];
				this.chapterArray[i].Refresh(i, (int)lastProgress, (int)curProgress);
			}
			this.tip.Type = TipType.SkillBook;
			this.tip.RuntimeParam = EasyPool.Get<ArgumentBox>().SetObject("ItemData", itemData.Clone(-1)).Set("ShowPageInfo", true);
		}

		// Token: 0x06007941 RID: 31041 RVA: 0x00385FB8 File Offset: 0x003841B8
		public GearMateLifeSkillBookChapter GetChapter(int index)
		{
			return this.chapterArray[index];
		}

		// Token: 0x04005BED RID: 23533
		[SerializeField]
		private ItemBack itemBack;

		// Token: 0x04005BEE RID: 23534
		[SerializeField]
		private TextMeshProUGUI textName;

		// Token: 0x04005BEF RID: 23535
		[SerializeField]
		private CButton buttonCancel;

		// Token: 0x04005BF0 RID: 23536
		[SerializeField]
		private TooltipInvoker tip;

		// Token: 0x04005BF1 RID: 23537
		[SerializeField]
		private GearMateLifeSkillBookChapter[] chapterArray;

		// Token: 0x04005BF2 RID: 23538
		private ItemDisplayData _itemData;
	}
}
