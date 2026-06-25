using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork;
using Game.Views.MouseTips.Common;
using Game.Views.MouseTips.Item.Common;
using GameData.Domains.Item.Display;
using UnityEngine;

namespace Game.Views.MouseTips.Item
{
	// Token: 0x020008A1 RID: 2209
	public class TooltipFood : TooltipItemBase
	{
		// Token: 0x17000C8D RID: 3213
		// (get) Token: 0x060069AA RID: 27050 RVA: 0x00309B03 File Offset: 0x00307D03
		protected override bool CanStick
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060069AB RID: 27051 RVA: 0x00309B08 File Offset: 0x00307D08
		protected override void Init(ArgumentBox argsBox)
		{
			argsBox.Get<ItemDisplayData>("ItemData", out this._itemData);
			argsBox.Get("TemplateDataOnly", out this._templateDataOnly);
			this._itemKey = this._itemData.RealKey;
			this.configData = Food.Instance[this._itemKey.TemplateId];
			base.Init(argsBox);
			this.DisableDetail = (!this.HasSelfPoison && !this.HasAttachedPoison);
			base.PostInit();
			this.Refresh();
		}

		// Token: 0x060069AC RID: 27052 RVA: 0x00309B96 File Offset: 0x00307D96
		public override void SetNewData(ArgumentBox argsBox)
		{
			this.Init(argsBox);
			this.Refresh();
		}

		// Token: 0x060069AD RID: 27053 RVA: 0x00309BA8 File Offset: 0x00307DA8
		public override void Refresh()
		{
			base.Refresh();
			this.RefreshTime();
			this.RefreshAddProperty();
			this.RefreshCombat();
			UIElement element = this.Element;
			if (element != null)
			{
				element.ShowAfterRefresh();
			}
		}

		// Token: 0x060069AE RID: 27054 RVA: 0x00309BDC File Offset: 0x00307DDC
		private void RefreshTime()
		{
			this.propertyTime.gameObject.SetActive(this.configData.Duration > 0);
			this.propertyTime.SetValue(this.configData.Duration.ToString().SetColor("brightyellow"));
		}

		// Token: 0x060069AF RID: 27055 RVA: 0x00309C30 File Offset: 0x00307E30
		private void RefreshAddProperty()
		{
			List<TooltipItemProperty> list = this.layoutAddProperty.GetComponentsInChildren<TooltipItemProperty>(true).ToList<TooltipItemProperty>();
			int index = 0;
			for (ECharacterPropertyReferencedType type = ECharacterPropertyReferencedType.Strength; type <= ECharacterPropertyReferencedType.ResistOfIllusoryPoison; type++)
			{
				int baseValue = this.GetAddPropertyBaseValue(type);
				TooltipUtil.AppendAddProperty(ref index, list, (short)type, baseValue, baseValue, this.IsDetail, false, false, false, true, false, false, false);
			}
			for (ECharacterPropertyReferencedType type2 = ECharacterPropertyReferencedType.RecoveryStrength; type2 <= ECharacterPropertyReferencedType.RecoveryIntelligence; type2++)
			{
				int baseValue2 = this.GetAddPropertyBaseValue(type2);
				TooltipUtil.AppendAddProperty(ref index, list, (short)type2, baseValue2, baseValue2, this.IsDetail, false, false, false, true, false, false, true);
			}
			for (int i = index; i < list.Count; i++)
			{
				list[i].gameObject.SetActive(false);
			}
			this.layoutAddProperty.gameObject.SetActive(index > 0);
		}

		// Token: 0x060069B0 RID: 27056 RVA: 0x00309D14 File Offset: 0x00307F14
		private unsafe int GetAddPropertyBaseValue(ECharacterPropertyReferencedType type)
		{
			if (!true)
			{
			}
			short result;
			switch (type)
			{
			case ECharacterPropertyReferencedType.Strength:
				result = this.configData.Strength;
				break;
			case ECharacterPropertyReferencedType.Dexterity:
				result = this.configData.Dexterity;
				break;
			case ECharacterPropertyReferencedType.Concentration:
				result = this.configData.Concentration;
				break;
			case ECharacterPropertyReferencedType.Vitality:
				result = this.configData.Vitality;
				break;
			case ECharacterPropertyReferencedType.Energy:
				result = this.configData.Energy;
				break;
			case ECharacterPropertyReferencedType.Intelligence:
				result = this.configData.Intelligence;
				break;
			case ECharacterPropertyReferencedType.HitRateStrength:
				result = this.configData.HitRateStrength;
				break;
			case ECharacterPropertyReferencedType.HitRateTechnique:
				result = this.configData.HitRateTechnique;
				break;
			case ECharacterPropertyReferencedType.HitRateSpeed:
				result = this.configData.HitRateSpeed;
				break;
			case ECharacterPropertyReferencedType.HitRateMind:
				result = this.configData.HitRateMind;
				break;
			case ECharacterPropertyReferencedType.PenetrateOfOuter:
				result = this.configData.PenetrateOfOuter;
				break;
			case ECharacterPropertyReferencedType.PenetrateOfInner:
				result = this.configData.PenetrateOfInner;
				break;
			case ECharacterPropertyReferencedType.AvoidRateStrength:
				result = this.configData.AvoidRateStrength;
				break;
			case ECharacterPropertyReferencedType.AvoidRateTechnique:
				result = this.configData.AvoidRateTechnique;
				break;
			case ECharacterPropertyReferencedType.AvoidRateSpeed:
				result = this.configData.AvoidRateSpeed;
				break;
			case ECharacterPropertyReferencedType.AvoidRateMind:
				result = this.configData.AvoidRateMind;
				break;
			case ECharacterPropertyReferencedType.PenetrateResistOfOuter:
				result = this.configData.PenetrateResistOfOuter;
				break;
			case ECharacterPropertyReferencedType.PenetrateResistOfInner:
				result = this.configData.PenetrateResistOfInner;
				break;
			case ECharacterPropertyReferencedType.RecoveryOfStance:
				result = this.configData.RecoveryOfStance;
				break;
			case ECharacterPropertyReferencedType.RecoveryOfBreath:
				result = this.configData.RecoveryOfBreath;
				break;
			case ECharacterPropertyReferencedType.MoveSpeed:
				result = this.configData.MoveSpeed;
				break;
			case ECharacterPropertyReferencedType.RecoveryOfFlaw:
				result = this.configData.RecoveryOfFlaw;
				break;
			case ECharacterPropertyReferencedType.CastSpeed:
				result = this.configData.CastSpeed;
				break;
			case ECharacterPropertyReferencedType.RecoveryOfBlockedAcupoint:
				result = this.configData.RecoveryOfBlockedAcupoint;
				break;
			case ECharacterPropertyReferencedType.WeaponSwitchSpeed:
				result = this.configData.WeaponSwitchSpeed;
				break;
			case ECharacterPropertyReferencedType.AttackSpeed:
				result = this.configData.AttackSpeed;
				break;
			case ECharacterPropertyReferencedType.InnerRatio:
				result = this.configData.InnerRatio;
				break;
			case ECharacterPropertyReferencedType.RecoveryOfQiDisorder:
				result = this.configData.RecoveryOfQiDisorder;
				break;
			case ECharacterPropertyReferencedType.ResistOfHotPoison:
				result = this.configData.ResistOfHotPoison;
				break;
			case ECharacterPropertyReferencedType.ResistOfGloomyPoison:
				result = this.configData.ResistOfGloomyPoison;
				break;
			case ECharacterPropertyReferencedType.ResistOfColdPoison:
				result = this.configData.ResistOfColdPoison;
				break;
			case ECharacterPropertyReferencedType.ResistOfRedPoison:
				result = this.configData.ResistOfRedPoison;
				break;
			case ECharacterPropertyReferencedType.ResistOfRottenPoison:
				result = this.configData.ResistOfRottenPoison;
				break;
			case ECharacterPropertyReferencedType.ResistOfIllusoryPoison:
				result = this.configData.ResistOfIllusoryPoison;
				break;
			default:
				switch (type)
				{
				case ECharacterPropertyReferencedType.RecoveryStrength:
					result = *this.configData.MainAttributesRegen[0];
					break;
				case ECharacterPropertyReferencedType.RecoveryDexterity:
					result = *this.configData.MainAttributesRegen[1];
					break;
				case ECharacterPropertyReferencedType.RecoveryConcentration:
					result = *this.configData.MainAttributesRegen[2];
					break;
				case ECharacterPropertyReferencedType.RecoveryVitality:
					result = *this.configData.MainAttributesRegen[3];
					break;
				case ECharacterPropertyReferencedType.RecoveryEnergy:
					result = *this.configData.MainAttributesRegen[4];
					break;
				case ECharacterPropertyReferencedType.RecoveryIntelligence:
					result = *this.configData.MainAttributesRegen[5];
					break;
				default:
					throw new ArgumentOutOfRangeException("type", type, null);
				}
				break;
			}
			if (!true)
			{
			}
			return (int)result;
		}

		// Token: 0x060069B1 RID: 27057 RVA: 0x0030A0D0 File Offset: 0x003082D0
		private void RefreshCombat()
		{
			bool isShow = this.configData.ConsumedFeatureMedals >= 0;
			this.rootCombat.SetActive(isShow);
			bool flag = isShow;
			if (flag)
			{
				this.propertyCost.SetValue(this.configData.ConsumedFeatureMedals.ToString().SetColor("brightyellow"));
			}
		}

		// Token: 0x060069B2 RID: 27058 RVA: 0x0030A128 File Offset: 0x00308328
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

		// Token: 0x04004BEA RID: 19434
		[Header("加成属性")]
		[SerializeField]
		private Transform layoutAddProperty;

		// Token: 0x04004BEB RID: 19435
		[SerializeField]
		private TooltipItemProperty propertyTime;

		// Token: 0x04004BEC RID: 19436
		[Header("战斗使用")]
		[SerializeField]
		private GameObject rootCombat;

		// Token: 0x04004BED RID: 19437
		[SerializeField]
		private TooltipItemProperty propertyCost;

		// Token: 0x04004BEE RID: 19438
		private FoodItem configData;
	}
}
