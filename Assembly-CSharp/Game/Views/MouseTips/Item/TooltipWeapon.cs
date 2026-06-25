using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Config;
using FrameWork;
using Game.Views.MouseTips.Item.Common;
using GameData.Domains.Combat;
using GameData.Domains.Global;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips.Item
{
	// Token: 0x020008AA RID: 2218
	public class TooltipWeapon : TooltipItemBase
	{
		// Token: 0x17000C96 RID: 3222
		// (get) Token: 0x06006A21 RID: 27169 RVA: 0x0030F8B1 File Offset: 0x0030DAB1
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

		// Token: 0x06006A22 RID: 27170 RVA: 0x0030F8E4 File Offset: 0x0030DAE4
		protected override void Init(ArgumentBox argsBox)
		{
			argsBox.Get<ItemKey>("ItemKey", out this._itemKey);
			bool flag = argsBox.Get<ItemDisplayData>("ItemData", out this._itemData);
			if (flag)
			{
				this._itemKey = this._itemData.RealKey;
			}
			argsBox.Get("IsInCompareUI", out this._isInCompareUI);
			argsBox.Get("TemplateDataOnly", out this._templateDataOnly);
			argsBox.Get("GetNewItemDisplayData", out this._GetNewItemDisplayData);
			bool flag2 = !argsBox.Get("CharId", out this._charId);
			if (flag2)
			{
				this._charId = -1;
			}
			this.configData = Weapon.Instance[this._itemKey.TemplateId];
			base.Init(argsBox);
			bool flag3 = this.Element == null;
			if (flag3)
			{
				this.OnListenerIdReady();
			}
			else
			{
				UIElement element = this.Element;
				element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(this.OnListenerIdReady));
			}
			base.PostInit();
			GlobalDomainMethod.Call.InvokeGuidingTrigger(263);
		}

		// Token: 0x06006A23 RID: 27171 RVA: 0x0030F9F1 File Offset: 0x0030DBF1
		public override void SetNewData(ArgumentBox argsBox)
		{
			this.Init(argsBox);
			this.Refresh();
		}

		// Token: 0x06006A24 RID: 27172 RVA: 0x0030FA04 File Offset: 0x0030DC04
		private void OnListenerIdReady()
		{
			bool getNewItemDisplayData = this._GetNewItemDisplayData;
			if (getNewItemDisplayData)
			{
				ItemKey requestKey;
				int requestCharId;
				if (this._itemData != null)
				{
					ItemKey key = this._itemData.Key;
					int num = this._itemData.OwnerCharId;
					requestKey = key;
					requestCharId = num;
				}
				else
				{
					ItemKey itemKey = this._itemKey;
					int num = this._charId;
					requestKey = itemKey;
					requestCharId = num;
				}
				ItemDomainMethod.AsyncCall.GetItemDisplayData(this, requestKey, requestCharId, delegate(int offset, RawDataPool dataPool)
				{
					Serializer.Deserialize(dataPool, offset, ref this._itemData);
					this.Refresh();
				});
			}
			else
			{
				this.Refresh();
			}
		}

		// Token: 0x06006A25 RID: 27173 RVA: 0x0030FA78 File Offset: 0x0030DC78
		public override void Refresh()
		{
			bool flag = this.NeedRefresh && this._itemData == null;
			if (!flag)
			{
				this.InnatePoisons = this.configData.InnatePoisons;
				base.Refresh();
				this.RefreshWeaponProperty();
				this.RefreshPenetrateProperty();
				this.RefreshHitProperty();
				bool templateDataOnly = this._templateDataOnly;
				if (templateDataOnly)
				{
					this.requirementArea.Refresh(!this._fixedPower, this._itemKey, false);
				}
				else
				{
					this.requirementArea.Refresh(!this._fixedPower, this._itemData);
				}
				this.RefreshTricksDetail();
				this.RefreshAttackPropertyDetail();
				this.tooltipItemRefiningEffect.SetForWeapon(this._itemData.RefiningEffects);
				UIElement element = this.Element;
				if (element != null)
				{
					element.ShowAfterRefresh();
				}
			}
		}

		// Token: 0x06006A26 RID: 27174 RVA: 0x0030FB4A File Offset: 0x0030DD4A
		private void RefreshWeaponProperty()
		{
			this.RefreshTricks();
			this.RefreshAttackRange();
			this.RefreshPrepareFrame();
			this.RefreshRequirementsPower();
			this.RefreshEquipmentAttack();
			this.RefreshEquipmentDefense();
		}

		// Token: 0x06006A27 RID: 27175 RVA: 0x0030FB78 File Offset: 0x0030DD78
		private void RefreshTricks()
		{
			List<sbyte> trickList = this._itemData.WeaponTrickList ?? this.configData.Tricks;
			Transform template = this.layoutTricks.GetChild(0);
			for (int i = 0; i < trickList.Count; i++)
			{
				sbyte trickId = trickList[i];
				TrickTypeItem trickConfig = Config.TrickType.Instance[trickId];
				Transform item = (i < this.layoutTricks.childCount) ? this.layoutTricks.GetChild(i) : Object.Instantiate<Transform>(template, this.layoutTricks);
				item.gameObject.SetActive(true);
				TextMeshProUGUI componentInChildren = item.GetComponentInChildren<TextMeshProUGUI>();
				if (componentInChildren != null)
				{
					componentInChildren.SetText(trickConfig.Name.SetColor(trickConfig.FontColor), true);
				}
			}
			for (int j = trickList.Count; j < this.layoutTricks.childCount; j++)
			{
				this.layoutTricks.GetChild(j).gameObject.SetActive(false);
			}
		}

		// Token: 0x06006A28 RID: 27176 RVA: 0x0030FC7C File Offset: 0x0030DE7C
		private void RefreshPrepareFrame()
		{
			bool flag = this._charId >= 0;
			if (flag)
			{
				ItemDomainMethod.AsyncCall.GetWeaponPrepareFrame(this, this._charId, this._itemKey, new AsyncMethodCallbackDelegate(this.OnGetWeaponPrepareFrame));
			}
			else
			{
				this.SetPrepareFrame(CFormula.CalcAttackStartupOrRecoveryFrame(100, this.configData.BaseStartupFrames));
			}
		}

		// Token: 0x06006A29 RID: 27177 RVA: 0x0030FCD4 File Offset: 0x0030DED4
		private void OnGetWeaponPrepareFrame(int offset, RawDataPool pool)
		{
			bool flag = this == null;
			if (!flag)
			{
				int prepareFrame = 0;
				Serializer.Deserialize(pool, offset, ref prepareFrame);
				this.SetPrepareFrame(prepareFrame);
			}
		}

		// Token: 0x06006A2A RID: 27178 RVA: 0x0030FD04 File Offset: 0x0030DF04
		private void SetPrepareFrame(int prepareFrame)
		{
			float seconds = (float)prepareFrame / 60f;
			this.textPrepareFrame.text = seconds.ToString("F2");
		}

		// Token: 0x06006A2B RID: 27179 RVA: 0x0030FD34 File Offset: 0x0030DF34
		private void RefreshAttackRange()
		{
			bool flag = this._charId >= 0;
			if (flag)
			{
				ItemDomainMethod.AsyncCall.GetWeaponAttackRange(this, this._charId, this._itemKey, new AsyncMethodCallbackDelegate(this.OnGetWeaponAttackRange));
			}
			else
			{
				this.SetAttackRange((int)this.configData.MinDistance, (int)this.configData.MaxDistance);
			}
		}

		// Token: 0x06006A2C RID: 27180 RVA: 0x0030FD90 File Offset: 0x0030DF90
		private void OnGetWeaponAttackRange(int offset, RawDataPool dataPool)
		{
			bool flag = this == null;
			if (!flag)
			{
				ValueTuple<int, int> attackRange = default(ValueTuple<int, int>);
				Serializer.Deserialize(dataPool, offset, ref attackRange);
				this.SetAttackRange(attackRange.Item1, attackRange.Item2);
			}
		}

		// Token: 0x06006A2D RID: 27181 RVA: 0x0030FDD0 File Offset: 0x0030DFD0
		private void SetAttackRange(int min, int max)
		{
			this.textAttackRangeMin.text = base.GetBonusValue((int)this.configData.MinDistance, min, new Func<int, string>(TooltipWeapon.<SetAttackRange>g__Handler|37_0), "", false);
			this.textAttackRangeMax.text = base.GetBonusValue((int)this.configData.MaxDistance, max, new Func<int, string>(TooltipWeapon.<SetAttackRange>g__Handler|37_0), "", false);
		}

		// Token: 0x06006A2E RID: 27182 RVA: 0x0030FE40 File Offset: 0x0030E040
		private void RefreshRequirementsPower()
		{
			this.textRequirementsPower.text = TooltipItemRequirementArea.GetPowerStr(this._templateDataOnly ? null : this._itemData);
			short power = this._itemData.PowerInfo.Power;
			this.propertyChangeTrick.Set(string.Empty, LanguageKey.LK_ItemTips_Weapon_ChangeTrick.Tr(), string.Format("{0}%", (int)(this.configData.ChangeTrickPercent * power / 100)), true);
			this.propertyPursueRate.Set(string.Empty, LanguageKey.LK_ItemTips_Weapon_PursueRate.Tr(), string.Format("{0}%", (int)(this.configData.PursueAttackFactor * power / 100)), true);
		}

		// Token: 0x06006A2F RID: 27183 RVA: 0x0030FEF8 File Offset: 0x0030E0F8
		private void RefreshEquipmentAttack()
		{
			string iconName = TipsRefiningEffect.GetRefinePropertyIconName(ERefiningEffectWeaponType.EquipmentAttack, true);
			string propertyName = TipsRefiningEffect.GetRefinePropertyName(ERefiningEffectWeaponType.EquipmentAttack);
			string content = base.GetBonusValue((int)this.configData.BaseEquipmentAttack, (int)this._itemData.EquipmentAttack, new Func<int, string>(this.EquipmentPropertyHandler), "", false);
			this.propertyEquipmentAttack.Set(iconName, propertyName, content, true);
		}

		// Token: 0x06006A30 RID: 27184 RVA: 0x0030FF54 File Offset: 0x0030E154
		private void RefreshEquipmentDefense()
		{
			string iconName = TipsRefiningEffect.GetRefinePropertyIconName(ERefiningEffectWeaponType.EquipmentDefense, true);
			string propertyName = TipsRefiningEffect.GetRefinePropertyName(ERefiningEffectWeaponType.EquipmentDefense);
			string content = base.GetBonusValue((int)this.configData.BaseEquipmentDefense, (int)this._itemData.EquipmentDefense, new Func<int, string>(this.EquipmentPropertyHandler), "", false);
			this.propertyEquipmentDefense.Set(iconName, propertyName, content, true);
		}

		// Token: 0x06006A31 RID: 27185 RVA: 0x0030FFB0 File Offset: 0x0030E1B0
		private string EquipmentPropertyHandler(int value)
		{
			return ((float)value / 100f).ToString("F2");
		}

		// Token: 0x06006A32 RID: 27186 RVA: 0x0030FFD8 File Offset: 0x0030E1D8
		private void RefreshPenetrateProperty()
		{
			int totalPenetrate = (int)(this._itemData.PenetrationInfo.Item1 * this._itemData.PowerInfo.Power / 100);
			int innerPenetrate = totalPenetrate * (int)this._itemData.WeaponInnerRatio / 100;
			int outerPenetrate = totalPenetrate - innerPenetrate;
			int baseTotalPenetrate = (int)(this.configData.BasePenetrationFactor * this._itemData.PowerInfo.Power / 100);
			int baseInnerPenetrate = baseTotalPenetrate * (int)this._itemData.WeaponInnerRatio / 100;
			int baseOuterPenetrate = baseTotalPenetrate - baseInnerPenetrate;
			this.propertyAttackOuter.Set("ui9_icon_attribute_attack_big_0", LanguageKey.LK_Penetrate_Outer.Tr(), TooltipItemBase.GetBonusValue(baseOuterPenetrate, outerPenetrate, false, null, "outterinjury", true), true);
			this.propertyAttackInner.Set("ui9_icon_attribute_attack_big_1", LanguageKey.LK_Penetrate_Inner.Tr(), TooltipItemBase.GetBonusValue(baseInnerPenetrate, innerPenetrate, false, null, "innerinjury", true), true);
		}

		// Token: 0x06006A33 RID: 27187 RVA: 0x003100B0 File Offset: 0x0030E2B0
		private unsafe void RefreshHitProperty()
		{
			for (sbyte hitType = 0; hitType < 4; hitType += 1)
			{
				TooltipItemProperty hitRefers = this.propertyHitTypes[(int)hitType];
				int hitValue = (int)(*(ref this._itemData.HitAvoidFactor.Items.FixedElementField + (IntPtr)hitType * 2));
				string trickStr = this.GetTricksStrByHitType(hitType);
				bool isShow = hitValue != 0 || !trickStr.IsNullOrEmpty();
				hitRefers.gameObject.SetActive(isShow);
				string iconName = "ui9_icon_attribute_hit_big_" + hitType.ToString();
				LanguageKey nameKey = LanguageKey.LK_HitType_0 + (int)hitType;
				string typeName = this.IsDetail ? (nameKey.Tr() + trickStr) : nameKey.Tr();
				string color = (hitValue > 0) ? "brightblue" : ((hitValue < 0) ? "brightred" : string.Empty);
				string content = isShow ? string.Format("{0}%", 100 + hitValue).SetColor(color) : "100%";
				hitRefers.Set(iconName, typeName, content, true);
			}
		}

		// Token: 0x06006A34 RID: 27188 RVA: 0x003101B4 File Offset: 0x0030E3B4
		private string GetTricksStrByHitType(sbyte hitType)
		{
			StringBuilder stringBuilder = EasyPool.Get<StringBuilder>();
			HashSet<int> processedTricks = EasyPool.Get<HashSet<int>>();
			List<sbyte> weaponTrickList = this._itemData.WeaponTrickList ?? Weapon.Instance[this._itemKey.TemplateId].Tricks;
			foreach (sbyte trick in weaponTrickList)
			{
				bool flag = !processedTricks.Add((int)trick);
				if (!flag)
				{
					TrickTypeItem config = Config.TrickType.Instance[trick];
					bool flag2 = config.AvoidType == hitType;
					if (flag2)
					{
						bool flag3 = stringBuilder.Length > 0;
						if (flag3)
						{
							stringBuilder.Append(LanguageKey.LK_Separator.Tr());
						}
						stringBuilder.Append(config.Name.SetColor(config.FontColor));
					}
				}
			}
			string str = (stringBuilder.Length > 0) ? LanguageKey.LK_Brackets_Fix.TrFormat(stringBuilder) : string.Empty;
			EasyPool.Free<HashSet<int>>(processedTricks);
			EasyPool.Free<StringBuilder>(stringBuilder);
			return str;
		}

		// Token: 0x06006A35 RID: 27189 RVA: 0x003102DC File Offset: 0x0030E4DC
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
			bool flag5 = !this.configData.CanChangeTrick;
			if (flag5)
			{
				this._disableFunctionList.Add(ItemFunction.CanChangeTrick);
			}
		}

		// Token: 0x06006A36 RID: 27190 RVA: 0x00310390 File Offset: 0x0030E590
		private void RefreshTricksDetail()
		{
			List<sbyte> trickList = this._itemData.WeaponTrickList ?? this.configData.Tricks;
			trickList = trickList.Distinct<sbyte>().ToList<sbyte>();
			Transform template = this.layoutTricksDetail.GetChild(0);
			for (int i = 0; i < trickList.Count; i++)
			{
				Transform child = (i < this.layoutTricksDetail.childCount) ? this.layoutTricksDetail.GetChild(i) : Object.Instantiate<Transform>(template, this.layoutTricksDetail);
				child.gameObject.SetActive(true);
				TooltipItemProperty property = child.GetComponent<TooltipItemProperty>();
				sbyte trick = trickList[i];
				TrickTypeItem config = Config.TrickType.Instance[trick];
				property.Set("", config.Name.SetColor(config.FontColor), config.Desc, true);
			}
			for (int j = trickList.Count; j < this.layoutTricksDetail.childCount; j++)
			{
				this.layoutTricksDetail.GetChild(j).gameObject.SetActive(false);
			}
		}

		// Token: 0x06006A37 RID: 27191 RVA: 0x003104AC File Offset: 0x0030E6AC
		private void RefreshAttackPropertyDetail()
		{
			string attackTip = LanguageKey.LK_ItemTips_WeaponEffect_Tips.Tr();
			this.propertyAttackDetail.SetValue(attackTip);
			sbyte currInnerRatio = this._itemData.RealKey.IsValid() ? this._itemData.WeaponInnerRatio : this.configData.DefaultInnerRatio;
			int outerRatio = (int)(100 - currInnerRatio);
			this.innerRatioText.text = string.Format("{0}%", currInnerRatio);
			this.outerRatioText.text = string.Format("{0}%", outerRatio);
			float ratio = (float)currInnerRatio / 100f;
			this.innerRatioFillImage.fillAmount = ratio;
			this.outerRatioFillImage.fillAmount = 1f - ratio;
			float barWidth = this.innerOuterRatioBar.rect.width;
			this.innerOuterRatioHandle.anchoredPosition = new Vector2((1f - ratio) * barWidth, this.innerOuterRatioHandle.anchoredPosition.y);
		}

		// Token: 0x06006A3A RID: 27194 RVA: 0x003105CC File Offset: 0x0030E7CC
		[CompilerGenerated]
		internal static string <SetAttackRange>g__Handler|37_0(int value)
		{
			return ((float)value / 10f).ToString("F1");
		}

		// Token: 0x04004C8B RID: 19595
		[Header("兵器属性")]
		[SerializeField]
		private Transform layoutTricks;

		// Token: 0x04004C8C RID: 19596
		[SerializeField]
		private TextMeshProUGUI textPrepareFrame;

		// Token: 0x04004C8D RID: 19597
		[SerializeField]
		private TextMeshProUGUI textAttackRangeMin;

		// Token: 0x04004C8E RID: 19598
		[SerializeField]
		private TextMeshProUGUI textAttackRangeMax;

		// Token: 0x04004C8F RID: 19599
		[SerializeField]
		private TextMeshProUGUI textRequirementsPower;

		// Token: 0x04004C90 RID: 19600
		[SerializeField]
		private TooltipItemProperty propertyEquipmentAttack;

		// Token: 0x04004C91 RID: 19601
		[SerializeField]
		private TooltipItemProperty propertyEquipmentDefense;

		// Token: 0x04004C92 RID: 19602
		[SerializeField]
		private TooltipItemProperty propertyChangeTrick;

		// Token: 0x04004C93 RID: 19603
		[SerializeField]
		private TooltipItemProperty propertyPursueRate;

		// Token: 0x04004C94 RID: 19604
		[Header("攻击属性")]
		[SerializeField]
		private TooltipItemProperty propertyAttackOuter;

		// Token: 0x04004C95 RID: 19605
		[SerializeField]
		private TooltipItemProperty propertyAttackInner;

		// Token: 0x04004C96 RID: 19606
		[Header("命中属性")]
		[SerializeField]
		private TooltipItemProperty[] propertyHitTypes;

		// Token: 0x04004C97 RID: 19607
		[Header("详细模式 发挥需求")]
		[SerializeField]
		private TooltipItemRequirementArea requirementArea;

		// Token: 0x04004C98 RID: 19608
		[Header("详细模式 招式")]
		[SerializeField]
		private Transform layoutTricksDetail;

		// Token: 0x04004C99 RID: 19609
		[Header("详细模式 攻击属性 内外功比例")]
		[SerializeField]
		private TooltipItemProperty propertyAttackDetail;

		// Token: 0x04004C9A RID: 19610
		[SerializeField]
		private RectTransform innerOuterRatioBar;

		// Token: 0x04004C9B RID: 19611
		[SerializeField]
		private CImage innerRatioFillImage;

		// Token: 0x04004C9C RID: 19612
		[SerializeField]
		private CImage outerRatioFillImage;

		// Token: 0x04004C9D RID: 19613
		[SerializeField]
		private RectTransform innerOuterRatioHandle;

		// Token: 0x04004C9E RID: 19614
		[SerializeField]
		private TextMeshProUGUI outerRatioText;

		// Token: 0x04004C9F RID: 19615
		[SerializeField]
		private TextMeshProUGUI innerRatioText;

		// Token: 0x04004CA0 RID: 19616
		[Header("详细模式 精制效果")]
		[SerializeField]
		private TooltipItemRefiningEffect tooltipItemRefiningEffect;

		// Token: 0x04004CA1 RID: 19617
		private bool _GetNewItemDisplayData;

		// Token: 0x04004CA2 RID: 19618
		private WeaponItem configData;
	}
}
