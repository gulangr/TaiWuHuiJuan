using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using GameData.Domains.Character.AvatarSystem;
using GameData.Domains.Item.Display;
using GameDataExtensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.CharacterMenu
{
	// Token: 0x02000B63 RID: 2915
	[RequireComponent(typeof(HorizontalLayoutGroup))]
	public class CharacterMenuEquipItemProperty : MonoBehaviour
	{
		// Token: 0x0600905E RID: 36958 RVA: 0x00434A40 File Offset: 0x00432C40
		public void Set(string text, bool isActive = true)
		{
			this.SetLabelActive(isActive);
			this.Text = text;
		}

		// Token: 0x0600905F RID: 36959 RVA: 0x00434A54 File Offset: 0x00432C54
		public void SetPower(ItemDisplayData data)
		{
			this.Text = ((this.LabelActive = data.ShouldShowPower()) ? LanguageKey.LK_CharacterMenuEquip_Power.TrFormat(data.PowerInfo.Power.ToString()) : LanguageKey.LK_CharacterMenuEquip_Power_Invalid.Tr()).ColorReplace();
			bool flag = this.mouseTip != null;
			if (flag)
			{
				this.mouseTip.enabled = data.ShouldShowPower();
				bool flag2 = !data.ShouldShowPower();
				if (!flag2)
				{
					this.mouseTip.Type = TipType.SingleDesc;
					TooltipInvoker tooltipInvoker = this.mouseTip;
					if (tooltipInvoker.RuntimeParam == null)
					{
						tooltipInvoker.RuntimeParam = new ArgumentBox();
					}
					this.mouseTip.RuntimeParam.Set("arg0", LocalStringManager.GetFormat(LanguageKey.LK_MouseTip_WeaponAndArmor_Power, data.PowerInfo.Power.ToString()) ?? "");
				}
			}
		}

		// Token: 0x06009060 RID: 36960 RVA: 0x00434B40 File Offset: 0x00432D40
		public void SetDurability(ItemDisplayData data)
		{
			this.Text = ((this.LabelActive = (data.MaxDurability > 0)) ? LanguageKey.LK_CharacterMenuEquip_Durability.TrFormat(data.Durability, data.MaxDurability) : LanguageKey.LK_CharacterMenuEquip_Durability_Invalid.Tr()).ColorReplace();
			bool flag = this.mouseTip != null;
			if (flag)
			{
				this.mouseTip.enabled = (data.MaxDurability > 0);
				bool flag2 = data.MaxDurability <= 0;
				if (!flag2)
				{
					this.mouseTip.Type = TipType.SingleDesc;
					TooltipInvoker tooltipInvoker = this.mouseTip;
					if (tooltipInvoker.RuntimeParam == null)
					{
						tooltipInvoker.RuntimeParam = new ArgumentBox();
					}
					this.mouseTip.RuntimeParam.Set("arg0", LocalStringManager.Get(LanguageKey.LK_Durability) + LocalStringManager.Get(LanguageKey.LK_Colon_Symbol) + LanguageKey.LK_CharacterMenuEquip_Durability.TrFormat(data.Durability, data.MaxDurability));
				}
			}
		}

		// Token: 0x06009061 RID: 36961 RVA: 0x00434C50 File Offset: 0x00432E50
		public void SetWeight(ItemDisplayData data)
		{
			this.Set(NumberFormatUtils.FormatItemWeight(data.Weight).ColorReplace(), true);
			bool flag = this.mouseTip != null;
			if (flag)
			{
				this.mouseTip.Type = TipType.SingleDesc;
				TooltipInvoker tooltipInvoker = this.mouseTip;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = new ArgumentBox();
				}
				this.mouseTip.RuntimeParam.Set("arg0", LocalStringManager.GetFormat(LanguageKey.LK_MouseTip_Weight, NumberFormatUtils.FormatItemWeight(data.Weight)));
			}
		}

		// Token: 0x06009062 RID: 36962 RVA: 0x00434CDC File Offset: 0x00432EDC
		public void SetCharm(ItemDisplayData data, int avatarId)
		{
			AvatarManager avatarManager = SingletonObject.getInstance<AvatarManager>();
			int charm = (int)((data.Key.ItemType == 3) ? avatarManager.GetAsset(avatarId, EAvatarElementsType.Cloth, new short[]
			{
				Clothing.Instance[data.Key.TemplateId].DisplayId
			}).Config.ElemCharm : 0);
			this.Text = ((this.LabelActive = (charm > 0)) ? LanguageKey.LK_CharacterMenuEquip_Charm.TrFormat(charm).ColorReplace() : ((charm < 0) ? LanguageKey.LK_CharacterMenuEquip_Charm_Descrease.TrFormat(-charm).ColorReplace() : LanguageKey.LK_CharacterMenuEquip_Charm_NoChange.Tr().ColorReplace()));
		}

		// Token: 0x06009063 RID: 36963 RVA: 0x00434D90 File Offset: 0x00432F90
		[Obsolete("老需求，现在不显示")]
		public void SetClothStyle(ItemDisplayData data)
		{
			ClothingItem weavedClothingConfig = Clothing.Instance[data.IsWeaved ? data.WeavedClothingTemplateId : data.Key.TemplateId];
			this.Set((weavedClothingConfig == null) ? LanguageKey.LK_CharacterMenuEquip_ClothStyle_Invalid.Tr().ColorReplace() : LanguageKey.LK_CharacterMenuEquip_ClothStyle.TrFormat(weavedClothingConfig.Name.SetColor(Colors.Instance.GradeColors[(int)weavedClothingConfig.Grade])).ColorReplace(), true);
		}

		// Token: 0x06009064 RID: 36964 RVA: 0x00434E10 File Offset: 0x00433010
		[Obsolete("老需求，现在不显示")]
		public void SetBestProfession(ItemDisplayData data)
		{
			foreach (ProfessionItem item in ((IEnumerable<ProfessionItem>)Profession.Instance))
			{
				bool flag = item.BonusClothing == data.Key.TemplateId;
				if (flag)
				{
					this.Set(LanguageKey.LK_CharacterMenuEquip_BestProfession.TrFormat(item.Name).ColorReplace(), true);
					return;
				}
			}
			this.Set(LanguageKey.LK_CharacterMenuEquip_BestProfession_Invalid.Tr().ColorReplace(), false);
		}

		// Token: 0x06009065 RID: 36965 RVA: 0x00434EA8 File Offset: 0x004330A8
		[Obsolete("老需求，现在不显示")]
		public string SetTravelTimeReduction(ItemDisplayData data)
		{
			return this.Text = ((this.LabelActive = (data.TravelTimeReduction > 0)) ? LanguageKey.LK_CharacterMenuEquip_TravelTimeReduction.TrFormat(data.TravelTimeReduction).ColorReplace() : LanguageKey.LK_CharacterMenuEquip_TravelTimeReduction_Invalid.Tr().ColorReplace());
		}

		// Token: 0x06009066 RID: 36966 RVA: 0x00434F00 File Offset: 0x00433100
		public string SetInventoryLoad(ItemDisplayData data)
		{
			this.Text = ((this.LabelActive = (data.MaxInventoryLoadBonus > 0)) ? LanguageKey.LK_CharacterMenuEquip_InventoryLoad.TrFormat(NumberFormatUtils.FormatItemWeight((int)data.MaxInventoryLoadBonus)).ColorReplace() : LanguageKey.LK_CharacterMenuEquip_InventoryLoad_Invalid.Tr().ColorReplace());
			bool flag = this.mouseTip != null;
			if (flag)
			{
				this.mouseTip.enabled = (data.MaxInventoryLoadBonus > 0);
				bool flag2 = data.MaxInventoryLoadBonus > 0;
				if (flag2)
				{
					this.mouseTip.Type = TipType.SingleDesc;
					TooltipInvoker tooltipInvoker = this.mouseTip;
					if (tooltipInvoker.RuntimeParam == null)
					{
						tooltipInvoker.RuntimeParam = new ArgumentBox();
					}
					this.mouseTip.RuntimeParam.Set("arg0", LanguageKey.LK_MouseTip_InventoryLoadBonus.TrFormat(NumberFormatUtils.FormatItemWeight((int)data.MaxInventoryLoadBonus)));
				}
			}
			return this.Text;
		}

		// Token: 0x06009067 RID: 36967 RVA: 0x00434FEC File Offset: 0x004331EC
		public string SetTamePoint(ItemDisplayData data)
		{
			bool flag = this.LabelActive = (data.Key.ItemType == 4 && Carrier.Instance[data.Key.TemplateId].TamePoint >= 0);
			string result;
			if (flag)
			{
				base.gameObject.SetActive(true);
				this.Text = LanguageKey.LK_CharacterMenuEquip_TamePoint.TrFormat(data.CarrierTamePoint);
				bool flag2 = this.mouseTip != null;
				if (flag2)
				{
					this.mouseTip.enabled = (data.CarrierTamePoint > 0);
					bool flag3 = data.CarrierTamePoint > 0;
					if (flag3)
					{
						this.mouseTip.Type = TipType.SingleDesc;
						TooltipInvoker tooltipInvoker = this.mouseTip;
						if (tooltipInvoker.RuntimeParam == null)
						{
							tooltipInvoker.RuntimeParam = new ArgumentBox();
						}
						this.mouseTip.RuntimeParam.Set("arg0", LocalStringManager.GetFormat(LanguageKey.LK_MouseTip_TamePoint, data.CarrierTamePoint));
					}
				}
				result = this.Text;
			}
			else
			{
				base.gameObject.SetActive(false);
				result = "";
			}
			return result;
		}

		// Token: 0x17000FB4 RID: 4020
		// (get) Token: 0x06009068 RID: 36968 RVA: 0x00435116 File Offset: 0x00433316
		// (set) Token: 0x06009069 RID: 36969 RVA: 0x00435123 File Offset: 0x00433323
		public string Text
		{
			get
			{
				return this.desc.text;
			}
			set
			{
				this.desc.text = value;
			}
		}

		// Token: 0x17000FB5 RID: 4021
		// (get) Token: 0x0600906A RID: 36970 RVA: 0x00435132 File Offset: 0x00433332
		// (set) Token: 0x0600906B RID: 36971 RVA: 0x0043513F File Offset: 0x0043333F
		public bool LabelActive
		{
			get
			{
				return this.icon.enabled;
			}
			set
			{
				this.SetLabelActive(value);
			}
		}

		// Token: 0x0600906C RID: 36972 RVA: 0x0043514C File Offset: 0x0043334C
		public void SetLabelActive(bool isActive)
		{
			if (isActive)
			{
				this.icon.enabled = true;
				this.icon.sprite = this.iconSprite;
			}
			else
			{
				this.icon.enabled = false;
			}
		}

		// Token: 0x04006F0F RID: 28431
		[SerializeField]
		private CImage icon;

		// Token: 0x04006F10 RID: 28432
		[SerializeField]
		private TMP_Text desc;

		// Token: 0x04006F11 RID: 28433
		[SerializeField]
		private Sprite defaultSprite;

		// Token: 0x04006F12 RID: 28434
		[SerializeField]
		private Sprite iconSprite;

		// Token: 0x04006F13 RID: 28435
		[SerializeField]
		private HorizontalLayoutGroup layout;

		// Token: 0x04006F14 RID: 28436
		[SerializeField]
		private TooltipInvoker mouseTip;
	}
}
