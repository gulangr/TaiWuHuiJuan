using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork;
using Game.Views.MouseTips.Common;
using Game.Views.MouseTips.Item.Common;
using GameData.Domains.Item.Display;
using GameData.Domains.Taiwu;
using GameData.Serializer;
using GameData.Utilities;
using UnityEngine;

namespace Game.Views.MouseTips.Item
{
	// Token: 0x020008A9 RID: 2217
	public class TooltipTeaWine : TooltipItemBase
	{
		// Token: 0x17000C95 RID: 3221
		// (get) Token: 0x06006A13 RID: 27155 RVA: 0x0030F11D File Offset: 0x0030D31D
		protected override bool CanStick
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06006A14 RID: 27156 RVA: 0x0030F120 File Offset: 0x0030D320
		protected override void Init(ArgumentBox argsBox)
		{
			argsBox.Get<ItemDisplayData>("ItemData", out this._itemData);
			argsBox.Get("TemplateDataOnly", out this._templateDataOnly);
			bool flag = !argsBox.Get("AddProfessionBonus", out this._shouldAddProfessionBonus);
			if (flag)
			{
				this._shouldAddProfessionBonus = false;
			}
			this._itemKey = this._itemData.RealKey;
			this.configData = TeaWine.Instance[this._itemKey.TemplateId];
			base.Init(argsBox);
			this.DisableDetail = (!this.HasSelfPoison && !this.HasAttachedPoison);
			base.PostInit();
			this.Refresh();
		}

		// Token: 0x06006A15 RID: 27157 RVA: 0x0030F1CD File Offset: 0x0030D3CD
		public override void SetNewData(ArgumentBox argsBox)
		{
			this.Init(argsBox);
			this.Refresh();
		}

		// Token: 0x06006A16 RID: 27158 RVA: 0x0030F1E0 File Offset: 0x0030D3E0
		public override void Refresh()
		{
			base.Refresh();
			bool flag = this._shouldAddProfessionBonus && this.configData.ItemSubType == 901;
			if (flag)
			{
				TaiwuDomainMethod.AsyncCall.GetWineTasterBonusPercentage(null, delegate(int offset, RawDataPool pool)
				{
					Serializer.Deserialize(pool, offset, ref this._addPercent);
					this.RefreshAddProperty();
				});
			}
			else
			{
				this._addPercent = 0;
			}
			this.RefreshTime();
			this.RefreshRecoverActionPoint();
			this.RefreshAddProperty();
			this.RefreshCombat();
			UIElement element = this.Element;
			if (element != null)
			{
				element.ShowAfterRefresh();
			}
		}

		// Token: 0x06006A17 RID: 27159 RVA: 0x0030F264 File Offset: 0x0030D464
		private void RefreshTime()
		{
			this.propertyTime.gameObject.SetActive(this.configData.Duration > 0);
			this.propertyTime.SetValue(this.configData.Duration.ToString().SetColor("brightyellow"));
		}

		// Token: 0x06006A18 RID: 27160 RVA: 0x0030F2B8 File Offset: 0x0030D4B8
		private void RefreshQiDisorder()
		{
			bool flag = this.configData.DirectChangeOfQiDisorder == 0;
			if (flag)
			{
				this.propertyQiDisorder.gameObject.SetActive(false);
			}
			else
			{
				this.propertyQiDisorder.gameObject.SetActive(true);
				string value = this.GetDirectChangeOfQiDisorderValue();
				this.propertyQiDisorder.SetValue(value);
			}
		}

		// Token: 0x06006A19 RID: 27161 RVA: 0x0030F314 File Offset: 0x0030D514
		private string GetDirectChangeOfQiDisorderValue()
		{
			int qiDisorder = (int)(this.configData.DirectChangeOfQiDisorder / 10);
			int qiMax = qiDisorder * GlobalConfig.Instance.TeaWineEffectDisorderOfQiDelta[1] / 100;
			int qiMin = qiDisorder * GlobalConfig.Instance.TeaWineEffectDisorderOfQiDelta[0] / 100;
			return (qiDisorder >= 0) ? ("+" + LanguageKey.LK_SurroundWithChineseBrackets.TrFormat(string.Format("{0}~{1}", qiMin, qiMax))).SetColor("brightred") : ("-" + LanguageKey.LK_SurroundWithChineseBrackets.TrFormat(string.Format("{0}~{1}", Math.Abs(qiMin), Math.Abs(qiMax)))).SetColor("brightblue");
		}

		// Token: 0x06006A1A RID: 27162 RVA: 0x0030F3D8 File Offset: 0x0030D5D8
		private void RefreshRecoverActionPoint()
		{
			bool flag = this.configData.ActionPointRecover == 0;
			if (flag)
			{
				this.propertyActionPoint.gameObject.SetActive(false);
			}
			else
			{
				bool showProfession = (!this._templateDataOnly && SingletonObject.getInstance<ProfessionModel>().IsProfessionalSkillUnlocked(66)) || this.ForceShowProfession;
				this.propertyActionPoint.gameObject.SetActive(showProfession);
				bool flag2 = showProfession;
				if (flag2)
				{
					string color = (this.configData.ActionPointRecover > 0) ? "brightblue" : "brightred";
					string value = string.Format("{0:+0;-0}", this.configData.ActionPointRecover / 10).SetColor(color);
					this.propertyActionPoint.SetValue(value);
				}
			}
		}

		// Token: 0x06006A1B RID: 27163 RVA: 0x0030F498 File Offset: 0x0030D698
		private void RefreshAddProperty()
		{
			List<TooltipItemProperty> list = this.layoutAddProperty.GetComponentsInChildren<TooltipItemProperty>(true).ToList<TooltipItemProperty>();
			int index = 0;
			for (ECharacterPropertyReferencedType type = ECharacterPropertyReferencedType.HitRateStrength; type <= ECharacterPropertyReferencedType.PenetrateResistOfInner; type++)
			{
				int baseValue = this.GetAddPropertyBaseValue(type);
				int finalValue = (baseValue <= 0) ? baseValue : (baseValue * (100 + this._addPercent) / 100);
				TooltipUtil.AppendAddProperty(ref index, list, (short)type, baseValue, finalValue, this.IsDetail, false, false, false, true, false, false, false);
			}
			for (ECharacterPropertyReferencedType type2 = ECharacterPropertyReferencedType.InnerRatio; type2 <= ECharacterPropertyReferencedType.RecoveryOfQiDisorder; type2++)
			{
				int baseValue2 = this.GetAddPropertyBaseValue(type2);
				int finalValue2 = (baseValue2 <= 0) ? baseValue2 : (baseValue2 * (100 + this._addPercent) / 100);
				TooltipUtil.AppendAddProperty(ref index, list, (short)type2, baseValue2, finalValue2, this.IsDetail, false, false, false, true, false, false, false);
			}
			bool flag = this.configData.DirectChangeOfQiDisorder != 0;
			if (flag)
			{
				bool flag2 = index < list.Count;
				TooltipItemProperty addPropertyUi;
				if (flag2)
				{
					addPropertyUi = list[index];
				}
				else
				{
					TooltipItemProperty template = list.First<TooltipItemProperty>();
					addPropertyUi = Object.Instantiate<TooltipItemProperty>(template, template.transform.parent);
					addPropertyUi.transform.localScale = Vector3.one;
				}
				addPropertyUi.Set("mousetip_neixiwenluan", LanguageKey.LK_Qi_Disorder.Tr(), this.GetDirectChangeOfQiDisorderValue(), true);
				index++;
			}
			for (int i = index; i < list.Count; i++)
			{
				list[i].gameObject.SetActive(false);
			}
			this.layoutAddProperty.gameObject.SetActive(index > 0);
		}

		// Token: 0x06006A1C RID: 27164 RVA: 0x0030F634 File Offset: 0x0030D834
		private int GetAddPropertyBaseValue(ECharacterPropertyReferencedType type)
		{
			if (!true)
			{
			}
			short result;
			switch (type)
			{
			case ECharacterPropertyReferencedType.HitRateStrength:
				result = this.configData.HitRateStrength;
				goto IL_152;
			case ECharacterPropertyReferencedType.HitRateTechnique:
				result = this.configData.HitRateTechnique;
				goto IL_152;
			case ECharacterPropertyReferencedType.HitRateSpeed:
				result = this.configData.HitRateSpeed;
				goto IL_152;
			case ECharacterPropertyReferencedType.HitRateMind:
				result = this.configData.HitRateMind;
				goto IL_152;
			case ECharacterPropertyReferencedType.PenetrateOfOuter:
				result = this.configData.PenetrateOfOuter;
				goto IL_152;
			case ECharacterPropertyReferencedType.PenetrateOfInner:
				result = this.configData.PenetrateOfInner;
				goto IL_152;
			case ECharacterPropertyReferencedType.AvoidRateStrength:
				result = this.configData.AvoidRateStrength;
				goto IL_152;
			case ECharacterPropertyReferencedType.AvoidRateTechnique:
				result = this.configData.AvoidRateTechnique;
				goto IL_152;
			case ECharacterPropertyReferencedType.AvoidRateSpeed:
				result = this.configData.AvoidRateSpeed;
				goto IL_152;
			case ECharacterPropertyReferencedType.AvoidRateMind:
				result = this.configData.AvoidRateMind;
				goto IL_152;
			case ECharacterPropertyReferencedType.PenetrateResistOfOuter:
				result = this.configData.PenetrateResistOfOuter;
				goto IL_152;
			case ECharacterPropertyReferencedType.PenetrateResistOfInner:
				result = this.configData.PenetrateResistOfInner;
				goto IL_152;
			case ECharacterPropertyReferencedType.InnerRatio:
				result = this.configData.InnerRatio;
				goto IL_152;
			case ECharacterPropertyReferencedType.RecoveryOfQiDisorder:
				result = this.configData.RecoveryOfQiDisorder;
				goto IL_152;
			}
			throw new ArgumentOutOfRangeException("type", type, null);
			IL_152:
			if (!true)
			{
			}
			return (int)result;
		}

		// Token: 0x06006A1D RID: 27165 RVA: 0x0030F7A0 File Offset: 0x0030D9A0
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

		// Token: 0x06006A1E RID: 27166 RVA: 0x0030F7F8 File Offset: 0x0030D9F8
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

		// Token: 0x04004C82 RID: 19586
		[Header("加成属性")]
		[SerializeField]
		private Transform layoutAddProperty;

		// Token: 0x04004C83 RID: 19587
		[SerializeField]
		private TooltipItemProperty propertyTime;

		// Token: 0x04004C84 RID: 19588
		[SerializeField]
		private TooltipItemProperty propertyQiDisorder;

		// Token: 0x04004C85 RID: 19589
		[SerializeField]
		private TooltipItemProperty propertyActionPoint;

		// Token: 0x04004C86 RID: 19590
		[Header("战斗使用")]
		[SerializeField]
		private GameObject rootCombat;

		// Token: 0x04004C87 RID: 19591
		[SerializeField]
		private TooltipItemProperty propertyCost;

		// Token: 0x04004C88 RID: 19592
		private TeaWineItem configData;

		// Token: 0x04004C89 RID: 19593
		private bool _shouldAddProfessionBonus;

		// Token: 0x04004C8A RID: 19594
		private int _addPercent = 0;
	}
}
