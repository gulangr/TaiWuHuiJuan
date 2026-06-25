using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using Game.Views.MouseTips.Item.Common;
using GameData.Domains.Character.AvatarSystem;
using GameData.Domains.Item.Display;
using UnityEngine;

namespace Game.Views.MouseTips.Item
{
	// Token: 0x0200089D RID: 2205
	public class TooltipClothing : TooltipItemBase
	{
		// Token: 0x17000C88 RID: 3208
		// (get) Token: 0x06006971 RID: 26993 RVA: 0x00307BEE File Offset: 0x00305DEE
		protected override bool CanStick
		{
			get
			{
				bool result;
				if (UIManager.Instance.CheckPopupElementIsInTop(UIElement.CharacterMenuEquip))
				{
					ItemDisplayData itemData = this._itemData;
					result = (itemData != null && itemData.UsingType == ItemDisplayData.ItemUsingType.Equiped);
				}
				else
				{
					result = true;
				}
				return result;
			}
		}

		// Token: 0x06006972 RID: 26994 RVA: 0x00307C20 File Offset: 0x00305E20
		protected override void Init(ArgumentBox argsBox)
		{
			argsBox.Get<ItemDisplayData>("ItemData", out this._itemData);
			argsBox.Get("IsInCompareUI", out this._isInCompareUI);
			argsBox.Get("TemplateDataOnly", out this._templateDataOnly);
			bool flag = !argsBox.Get("CharId", out this._charId);
			if (flag)
			{
				this._charId = -1;
			}
			this._itemKey = this._itemData.RealKey;
			short templateId = this._itemKey.TemplateId;
			bool flag2 = templateId == 95 || templateId == 96;
			if (flag2)
			{
				this._itemData.EquipmentEffectIds = new List<short>
				{
					66
				};
			}
			this.configData = Clothing.Instance[this._itemKey.TemplateId];
			base.Init(argsBox);
			ItemDisplayData itemData = this._itemData;
			bool hasEffect = itemData != null && itemData.HaveEquipmentEffect();
			this.DisableDetail = !hasEffect;
			base.PostInit();
			this.Refresh();
		}

		// Token: 0x06006973 RID: 26995 RVA: 0x00307D1E File Offset: 0x00305F1E
		public override void SetNewData(ArgumentBox argsBox)
		{
			this.Init(argsBox);
			this.Refresh();
		}

		// Token: 0x06006974 RID: 26996 RVA: 0x00307D30 File Offset: 0x00305F30
		public override void Refresh()
		{
			base.Refresh();
			this.RefreshBaseProperty();
			UIElement element = this.Element;
			if (element != null)
			{
				element.ShowAfterRefresh();
			}
		}

		// Token: 0x06006975 RID: 26997 RVA: 0x00307D54 File Offset: 0x00305F54
		private void RefreshBaseProperty()
		{
			int type = ECharacterPropertyReferencedType.Attraction.ToInt();
			CharacterPropertyDisplayItem propertyConfig = CharacterPropertyDisplay.Instance[CharacterPropertyReferenced.Instance[type].DisplayType];
			AvatarManager avatarManager = SingletonObject.getInstance<AvatarManager>();
			short maleCharm = avatarManager.GetAsset(3, EAvatarElementsType.Cloth, new short[]
			{
				this.configData.DisplayId
			}).Config.ElemCharm;
			short femaleCharm = avatarManager.GetAsset(4, EAvatarElementsType.Cloth, new short[]
			{
				this.configData.DisplayId
			}).Config.ElemCharm;
			string maleCharmStr = maleCharm.ToString().SetColor((maleCharm > 0) ? "brightblue" : "brightred");
			string femaleCharmStr = femaleCharm.ToString().SetColor((femaleCharm > 0) ? "brightblue" : "brightred");
			string content = LanguageKey.LK_Mousetip_Clothing_Charm.TrFormat(maleCharmStr, femaleCharmStr);
			this.propertyCharm.Set(propertyConfig.TipsIcon, propertyConfig.Name, content, true);
		}

		// Token: 0x06006976 RID: 26998 RVA: 0x00307E48 File Offset: 0x00306048
		protected override void InitItemDisableFunctionList()
		{
			base.InitItemDisableFunctionList();
			bool flag = !this.configData.Repairable;
			if (flag)
			{
				this._disableFunctionList.Add(ItemFunction.Repairable);
			}
			bool flag2 = !this.configData.Transferable;
			if (flag2)
			{
				this._disableFunctionList.Add(ItemFunction.Transferable);
			}
			bool flag3 = !this.configData.Poisonable;
			if (flag3)
			{
				this._disableFunctionList.Add(ItemFunction.Poisonable);
			}
			bool flag4 = !this.configData.Refinable;
			if (flag4)
			{
				this._disableFunctionList.Add(ItemFunction.Refinable);
			}
		}

		// Token: 0x04004BC2 RID: 19394
		[Header("基本属性")]
		[SerializeField]
		private TooltipItemProperty propertyCharm;

		// Token: 0x04004BC3 RID: 19395
		private ClothingItem configData;
	}
}
