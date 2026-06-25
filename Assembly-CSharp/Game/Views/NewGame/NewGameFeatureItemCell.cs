using System;
using System.Collections.Generic;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using TMPro;
using UnityEngine;

namespace Game.Views.NewGame
{
	// Token: 0x020007E8 RID: 2024
	public class NewGameFeatureItemCell : MonoBehaviour
	{
		// Token: 0x17000BE5 RID: 3045
		// (get) Token: 0x060062A4 RID: 25252 RVA: 0x002D28F7 File Offset: 0x002D0AF7
		// (set) Token: 0x060062A5 RID: 25253 RVA: 0x002D28FF File Offset: 0x002D0AFF
		public TemplateKey ItemKey { get; private set; }

		// Token: 0x060062A6 RID: 25254 RVA: 0x002D2908 File Offset: 0x002D0B08
		public void Init(TemplateKey itemKey, int currentCount, NewGameFeatureItemGroup group)
		{
			this.ItemKey = itemKey;
			this._group = group;
			string itemSprite = ItemTemplateHelper.GetIcon(this.ItemKey.ItemType, this.ItemKey.TemplateId);
			sbyte grade = ItemTemplateHelper.GetGrade(this.ItemKey.ItemType, this.ItemKey.TemplateId);
			this.icon.SetSprite(itemSprite, false, null);
			this.gradeIcon.SetSprite(string.Format("{0}{1}", "ui9_icon_item_grade_", grade), false, null);
			bool flag = this.nameText != null;
			if (flag)
			{
				this.nameText.text = ItemTemplateHelper.GetName(this.ItemKey.ItemType, this.ItemKey.TemplateId);
			}
			this.UpdateCount(currentCount);
			this.addBtn.ClearAndAddListener(new Action(this.OnAddClick));
			this.subBtn.ClearAndAddListener(new Action(this.OnRemoveClick));
			TooltipInvoker tooltipInvoker = this.mouseTips;
			sbyte itemType = itemKey.ItemType;
			if (!true)
			{
			}
			TipType type;
			switch (itemType)
			{
			case 0:
				type = TipType.Weapon;
				goto IL_199;
			case 1:
				type = TipType.Armor;
				goto IL_199;
			case 2:
				type = TipType.Accessory;
				goto IL_199;
			case 3:
				type = TipType.Clothing;
				goto IL_199;
			case 4:
				type = TipType.Carrier;
				goto IL_199;
			case 5:
				type = TipType.Material;
				goto IL_199;
			case 6:
				type = TipType.CraftTool;
				goto IL_199;
			case 7:
				type = TipType.Food;
				goto IL_199;
			case 8:
				type = TipType.Medicine;
				goto IL_199;
			case 9:
				type = TipType.TeaWine;
				goto IL_199;
			case 10:
				type = TipType.SkillBook;
				goto IL_199;
			case 12:
				type = TipType.Misc;
				goto IL_199;
			}
			throw new Exception(string.Format("not supported: {0}", itemKey.ItemType));
			IL_199:
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
			bool flag2 = this._group.ParentView.FeatureData.TemplateId == 30 && itemKey.ItemType == 2;
			if (flag2)
			{
				ItemDisplayData itemDisplayData = itemData;
				if (itemDisplayData.EquipmentEffectIds == null)
				{
					itemDisplayData.EquipmentEffectIds = new List<short>();
				}
				itemData.EquipmentEffectIds.Add(67);
			}
			this.mouseTips.RuntimeParam.Set<ItemDisplayData>("ItemData", itemData);
			this.mouseTips.RuntimeParam.Set("TemplateDataOnly", true);
			this.mouseTips.RuntimeParam.Set("IsInCompareUI", false);
			this.mouseTips.RuntimeParam.Set("DisableCompare", true);
		}

		// Token: 0x060062A7 RID: 25255 RVA: 0x002D2B9D File Offset: 0x002D0D9D
		public void UpdateCount(int count)
		{
			this.countText.text = count.ToString();
		}

		// Token: 0x060062A8 RID: 25256 RVA: 0x002D2BB3 File Offset: 0x002D0DB3
		public void UpdateButtons(bool canAdd, bool canRemove)
		{
			this.addBtn.interactable = canAdd;
			this.subBtn.interactable = canRemove;
		}

		// Token: 0x060062A9 RID: 25257 RVA: 0x002D2BD0 File Offset: 0x002D0DD0
		private void OnAddClick()
		{
			this._group.OnAdd(this.ItemKey);
		}

		// Token: 0x060062AA RID: 25258 RVA: 0x002D2BE5 File Offset: 0x002D0DE5
		private void OnRemoveClick()
		{
			this._group.OnRemove(this.ItemKey);
		}

		// Token: 0x040044B8 RID: 17592
		[SerializeField]
		private CImage icon;

		// Token: 0x040044B9 RID: 17593
		[SerializeField]
		private TextMeshProUGUI countText;

		// Token: 0x040044BA RID: 17594
		[SerializeField]
		private CButton addBtn;

		// Token: 0x040044BB RID: 17595
		[SerializeField]
		private CButton subBtn;

		// Token: 0x040044BC RID: 17596
		[SerializeField]
		private TextMeshProUGUI nameText;

		// Token: 0x040044BD RID: 17597
		[SerializeField]
		private CImage gradeIcon;

		// Token: 0x040044BE RID: 17598
		[SerializeField]
		private TooltipInvoker mouseTips;

		// Token: 0x040044C0 RID: 17600
		private NewGameFeatureItemGroup _group;
	}
}
