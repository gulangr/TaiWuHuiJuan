using System;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using TMPro;
using UnityEngine;

namespace Game.Views.EventWindow
{
	// Token: 0x02000A3E RID: 2622
	public class EventWindowItemView : MonoBehaviour
	{
		// Token: 0x17000E2D RID: 3629
		// (get) Token: 0x06008178 RID: 33144 RVA: 0x003C40E9 File Offset: 0x003C22E9
		// (set) Token: 0x06008179 RID: 33145 RVA: 0x003C40F1 File Offset: 0x003C22F1
		public ItemDisplayData _displayData { get; private set; }

		// Token: 0x0600817A RID: 33146 RVA: 0x003C40FA File Offset: 0x003C22FA
		private void Awake()
		{
			this.btnMain.ClearAndAddListener(new Action(this.OnClickMain));
		}

		// Token: 0x0600817B RID: 33147 RVA: 0x003C4115 File Offset: 0x003C2315
		private void OnClickMain()
		{
			Action onClick = this._onClick;
			if (onClick != null)
			{
				onClick();
			}
		}

		// Token: 0x0600817C RID: 33148 RVA: 0x003C412C File Offset: 0x003C232C
		public void Init(ItemDisplayData displayData, Action onClick)
		{
			this._displayData = displayData;
			this._onClick = onClick;
			string itemSprite = ItemTemplateHelper.GetIcon(this._displayData.Key.ItemType, this._displayData.Key.TemplateId);
			sbyte grade = ItemTemplateHelper.GetGrade(this._displayData.Key.ItemType, this._displayData.Key.TemplateId);
			this.icon.SetSprite(itemSprite, false, null);
			this.gradeIcon.SetSprite(string.Format("{0}{1}", "ui9_icon_item_grade_", grade), false, null);
			bool flag = this.nameText != null;
			if (flag)
			{
				this.nameText.text = ItemTemplateHelper.GetName(this._displayData.Key.ItemType, this._displayData.Key.TemplateId);
			}
			ItemKey itemKey = this._displayData.Key;
			TooltipInvoker tooltipInvoker = this.mouseTips;
			sbyte itemType = itemKey.ItemType;
			if (!true)
			{
			}
			if (itemType != 10)
			{
				throw new Exception(string.Format("not supported: {0}", itemKey.ItemType));
			}
			TipType type = TipType.SkillBook;
			if (!true)
			{
			}
			tooltipInvoker.Type = type;
			TooltipInvoker tooltipInvoker2 = this.mouseTips;
			if (tooltipInvoker2.RuntimeParam == null)
			{
				tooltipInvoker2.RuntimeParam = new ArgumentBox();
			}
			ItemDisplayData itemData = new ItemDisplayData(itemKey.ItemType, itemKey.TemplateId);
			this.mouseTips.RuntimeParam.Set<ItemDisplayData>("ItemData", itemData);
			this.mouseTips.RuntimeParam.Set("IsInCompareUI", false);
			this.mouseTips.RuntimeParam.Set("DisableCompare", true);
		}

		// Token: 0x040062E5 RID: 25317
		[SerializeField]
		private CImage icon;

		// Token: 0x040062E6 RID: 25318
		[SerializeField]
		private TextMeshProUGUI nameText;

		// Token: 0x040062E7 RID: 25319
		[SerializeField]
		private CImage gradeIcon;

		// Token: 0x040062E8 RID: 25320
		[SerializeField]
		private TooltipInvoker mouseTips;

		// Token: 0x040062E9 RID: 25321
		[SerializeField]
		private CButton btnMain;

		// Token: 0x040062EB RID: 25323
		private Action _onClick;
	}
}
