using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Config;
using Config.ConfigCells.Character;
using FrameWork;
using Game.Views.MouseTips.Common;
using Game.Views.MouseTips.Item.Common;
using GameData.Domains.Extra;
using GameData.Domains.Item.Display;
using GameData.Domains.LegendaryBook;
using GameData.Serializer;
using GameData.Utilities;
using UnityEngine;

namespace Game.Views.MouseTips.Item
{
	// Token: 0x020008A5 RID: 2213
	public class TooltipLegendaryBook : TooltipItemBase
	{
		// Token: 0x17000C91 RID: 3217
		// (get) Token: 0x060069D6 RID: 27094 RVA: 0x0030C2D3 File Offset: 0x0030A4D3
		protected override bool CanStick
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060069D7 RID: 27095 RVA: 0x0030C2D8 File Offset: 0x0030A4D8
		protected override void Init(ArgumentBox argsBox)
		{
			argsBox.Get<ItemDisplayData>("ItemData", out this._itemData);
			argsBox.Get("TemplateDataOnly", out this._templateDataOnly);
			this._itemKey = this._itemData.RealKey;
			this._combatSkillType = Convert.ToSByte((int)(this._itemKey.TemplateId - 240));
			this._combatSkillTypeConfig = CombatSkillType.Instance[this._combatSkillType];
			this.configData = Misc.Instance[this._itemKey.TemplateId];
			bool isTaiwuOwned;
			if (!this._templateDataOnly)
			{
				ItemDisplayData itemData = this._itemData;
				int? num = (itemData != null) ? new int?(itemData.OwnerCharId) : null;
				int taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
				isTaiwuOwned = (num.GetValueOrDefault() == taiwuCharId & num != null);
			}
			else
			{
				isTaiwuOwned = false;
			}
			this._isTaiwuOwned = isTaiwuOwned;
			this.DisableDetail = !this._isTaiwuOwned;
			base.Init(argsBox);
			base.PostInit();
			ExtraDomainMethod.AsyncCall.GetTipLegendaryBookDisplayData(this, this._combatSkillType, new AsyncMethodCallbackDelegate(this.OnGetTipData));
		}

		// Token: 0x060069D8 RID: 27096 RVA: 0x0030C3ED File Offset: 0x0030A5ED
		private void OnGetTipData(int offset, RawDataPool dataPool)
		{
			Serializer.Deserialize(dataPool, offset, ref this._displayData);
			this.Refresh();
		}

		// Token: 0x060069D9 RID: 27097 RVA: 0x0030C408 File Offset: 0x0030A608
		public override void Refresh()
		{
			base.Refresh();
			this.RefreshCombatSkillType();
			this.taiwuArea.SetActive(this._isTaiwuOwned);
			bool isTaiwuOwned = this._isTaiwuOwned;
			if (isTaiwuOwned)
			{
				this.RefreshState();
				this.RefreshAddProperty();
				this.RefreshSlot();
				this.RefreshSlotDetail();
			}
			UIElement element = this.Element;
			if (element != null)
			{
				element.ShowAfterRefresh();
			}
		}

		// Token: 0x060069DA RID: 27098 RVA: 0x0030C470 File Offset: 0x0030A670
		private void RefreshCombatSkillType()
		{
			this.propertyType.Set(this._combatSkillTypeConfig.TipsIcon, LanguageKey.LK_CombatSkill_Type_Tip.Tr(), this._combatSkillTypeConfig.Name, true);
		}

		// Token: 0x060069DB RID: 27099 RVA: 0x0030C4A0 File Offset: 0x0030A6A0
		private void RefreshState()
		{
			this.propertyChallengeCount.Set("", LanguageKey.LK_MouseTip_LegendaryBook_State_Challenge_Title.Tr(), string.Format("{0}/{1}", this._displayData.BreakPlateCount, ELegendaryBookSlotState.BothUnlocked.ToInt()), true);
			TooltipLegendaryBook.<>c__DisplayClass19_0 CS$<>8__locals1;
			CS$<>8__locals1.maxPropertyCount = 0;
			CS$<>8__locals1.curPropertyCount = 0;
			CS$<>8__locals1.maxQualificationCount = 0;
			CS$<>8__locals1.curQualificationCount = 0;
			TooltipLegendaryBook.<RefreshState>g__CalcCount|19_0(this._combatSkillTypeConfig.LegendaryBookAddPropertyYin, this._displayData.BonusCountYin, ref CS$<>8__locals1);
			TooltipLegendaryBook.<RefreshState>g__CalcCount|19_0(this._combatSkillTypeConfig.LegendaryBookAddPropertyYang, this._displayData.BonusCountYang, ref CS$<>8__locals1);
			this.propertyPropertyCount.Set("", LanguageKey.LK_MouseTip_LegendaryBook_State_Property_Title.Tr(), string.Format("{0}/{1}", CS$<>8__locals1.curPropertyCount, CS$<>8__locals1.maxPropertyCount), true);
			this.propertyQualificationCount.Set("", LanguageKey.LK_MouseTip_LegendaryBook_State_Qualification_Title.Tr(), string.Format("{0}/{1}", CS$<>8__locals1.curQualificationCount, CS$<>8__locals1.maxQualificationCount), true);
		}

		// Token: 0x060069DC RID: 27100 RVA: 0x0030C5CC File Offset: 0x0030A7CC
		private void RefreshAddProperty()
		{
			TooltipLegendaryBook.<>c__DisplayClass20_0 CS$<>8__locals1;
			CS$<>8__locals1.propertyDict = new Dictionary<ECharacterPropertyReferencedType, int>();
			TooltipLegendaryBook.<RefreshAddProperty>g__CalcProperty|20_1(this._combatSkillTypeConfig.LegendaryBookAddPropertyYin, this._displayData.BonusCountYin, ref CS$<>8__locals1);
			TooltipLegendaryBook.<RefreshAddProperty>g__CalcProperty|20_1(this._combatSkillTypeConfig.LegendaryBookAddPropertyYang, this._displayData.BonusCountYang, ref CS$<>8__locals1);
			List<TooltipItemProperty> list = this.layoutAddProperty.GetComponentsInChildren<TooltipItemProperty>(true).ToList<TooltipItemProperty>();
			int index = 0;
			List<ECharacterPropertyReferencedType> keyList = (from k in CS$<>8__locals1.propertyDict.Keys
			orderby k
			select k).ToList<ECharacterPropertyReferencedType>();
			foreach (ECharacterPropertyReferencedType type in keyList)
			{
				int baseValue = CS$<>8__locals1.propertyDict[type];
				TooltipUtil.AppendAddProperty(ref index, list, (short)type, baseValue, baseValue, this.IsDetail, false, false, false, true, false, false, false);
			}
			for (int i = index; i < list.Count; i++)
			{
				list[i].gameObject.SetActive(false);
			}
			this.layoutAddProperty.gameObject.SetActive(index > 0);
		}

		// Token: 0x060069DD RID: 27101 RVA: 0x0030C720 File Offset: 0x0030A920
		private void RefreshSlot()
		{
			List<short> configSlots = this.GetConfigSlots();
			List<LegendaryBookSlotItem> dataList = (from id in configSlots
			select LegendaryBookSlot.Instance[id] into s
			where !s.Name.IsNullOrEmpty()
			select s).ToList<LegendaryBookSlotItem>();
			List<TooltipItemProperty> slotList = this.layoutSlot.GetComponentsInChildren<TooltipItemProperty>(true).ToList<TooltipItemProperty>();
			TooltipItemProperty template = slotList.First<TooltipItemProperty>();
			for (int i = 0; i < dataList.Count; i++)
			{
				bool flag = i < slotList.Count;
				TooltipItemProperty slot;
				if (flag)
				{
					slot = slotList[i];
				}
				else
				{
					slot = Object.Instantiate<TooltipItemProperty>(template, this.layoutSlot);
					slotList.Add(slot);
				}
				LegendaryBookSlotItem data = dataList[i];
				ValueTuple<bool, int> effectSlotInfo = this.GetEffectSlotInfo(i);
				bool isYang = effectSlotInfo.Item1;
				int index = effectSlotInfo.Item2;
				bool flag2 = index > (isYang ? this._displayData.BonusCountYang : this._displayData.BonusCountYin) / 4 - 1;
				if (flag2)
				{
					slot.Set("", data.Name, "-", true);
				}
				else
				{
					List<short> items = this._displayData.SkillSlot.Items;
					short skillId = (items != null) ? items.GetOrDefault(i, -1) : -1;
					CombatSkillItem skillConfig = CombatSkill.Instance[skillId];
					string skillName = ((skillConfig != null) ? skillConfig.Name.SetGradeColor((int)skillConfig.Grade) : null) ?? LanguageKey.LK_MouseTip_LegendaryBook_Slot_Empty.Tr();
					slot.Set("", data.Name, skillName, true);
				}
			}
			for (int j = dataList.Count; j < slotList.Count; j++)
			{
				slotList[j].gameObject.SetActive(false);
			}
		}

		// Token: 0x060069DE RID: 27102 RVA: 0x0030C904 File Offset: 0x0030AB04
		private void RefreshSlotDetail()
		{
			List<short> configSlots = this.GetConfigSlots();
			List<LegendaryBookSlotItem> dataList = (from id in configSlots
			select LegendaryBookSlot.Instance[id] into s
			where !s.Name.IsNullOrEmpty()
			select s).ToList<LegendaryBookSlotItem>();
			List<TooltipItemProperty> slotList = this.layoutSlotDetail.GetComponentsInChildren<TooltipItemProperty>(true).ToList<TooltipItemProperty>();
			TooltipItemProperty template = slotList.First<TooltipItemProperty>();
			for (int i = 0; i < dataList.Count; i++)
			{
				bool flag = i < slotList.Count;
				TooltipItemProperty slot;
				if (flag)
				{
					slot = slotList[i];
				}
				else
				{
					slot = Object.Instantiate<TooltipItemProperty>(template, this.layoutSlotDetail);
					slotList.Add(slot);
				}
				LegendaryBookSlotItem data = dataList[i];
				slot.Set("", data.Name, data.Desc, true);
			}
			for (int j = dataList.Count; j < slotList.Count; j++)
			{
				slotList[j].gameObject.SetActive(false);
			}
		}

		// Token: 0x060069DF RID: 27103 RVA: 0x0030CA30 File Offset: 0x0030AC30
		private List<short> GetConfigSlots()
		{
			List<short> configSlots = new List<short>();
			configSlots.Add(this._combatSkillTypeConfig.LegendaryBookWeaponSlot);
			configSlots.AddRange(this._combatSkillTypeConfig.LegendaryBookSkillSlots);
			return configSlots;
		}

		// Token: 0x060069E0 RID: 27104 RVA: 0x0030CA70 File Offset: 0x0030AC70
		[return: TupleElementNames(new string[]
		{
			"isYang",
			"index"
		})]
		private ValueTuple<bool, int> GetEffectSlotInfo(int configSlotIndex)
		{
			bool isWeaponSlotReal = this._combatSkillTypeConfig.LegendaryBookWeaponSlotItemSubTypes != null && this._combatSkillTypeConfig.LegendaryBookWeaponSlotItemSubTypes.Count > 0;
			bool flag = isWeaponSlotReal && configSlotIndex == 0;
			ValueTuple<bool, int> result;
			if (flag)
			{
				List<sbyte> legendaryBookEffectSlotYang = this._combatSkillTypeConfig.LegendaryBookEffectSlotYang;
				int num;
				if (legendaryBookEffectSlotYang == null)
				{
					num = -1;
				}
				else
				{
					num = legendaryBookEffectSlotYang.FindIndex((sbyte v) => v == -1);
				}
				int yangIndex = num;
				result = ((yangIndex >= 0) ? new ValueTuple<bool, int>(true, yangIndex) : new ValueTuple<bool, int>(false, -1));
			}
			else
			{
				List<sbyte> legendaryBookEffectSlotYang2 = this._combatSkillTypeConfig.LegendaryBookEffectSlotYang;
				int foundInYang = (legendaryBookEffectSlotYang2 != null) ? legendaryBookEffectSlotYang2.FindIndex((sbyte v) => (int)v == configSlotIndex) : -1;
				bool flag2 = foundInYang >= 0;
				if (flag2)
				{
					result = new ValueTuple<bool, int>(true, foundInYang);
				}
				else
				{
					List<sbyte> legendaryBookEffectSlotYin = this._combatSkillTypeConfig.LegendaryBookEffectSlotYin;
					int foundInYin = (legendaryBookEffectSlotYin != null) ? legendaryBookEffectSlotYin.FindIndex((sbyte v) => (int)v == configSlotIndex) : -1;
					bool flag3 = foundInYin >= 0;
					if (flag3)
					{
						result = new ValueTuple<bool, int>(false, foundInYin);
					}
					else
					{
						result = new ValueTuple<bool, int>(false, -1);
					}
				}
			}
			return result;
		}

		// Token: 0x060069E1 RID: 27105 RVA: 0x0030CBA4 File Offset: 0x0030ADA4
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

		// Token: 0x060069E3 RID: 27107 RVA: 0x0030CC40 File Offset: 0x0030AE40
		[CompilerGenerated]
		internal static void <RefreshState>g__CalcCount|19_0(List<short> legendaryBookAddPropertyList, int activeCount, ref TooltipLegendaryBook.<>c__DisplayClass19_0 A_2)
		{
			for (int index = 0; index < legendaryBookAddPropertyList.Count; index++)
			{
				short legendaryBookAddProperty = legendaryBookAddPropertyList[index];
				LegendaryBookPropertyBonusTypeItem bonusTypeConfig = LegendaryBookPropertyBonusType.Instance[legendaryBookAddProperty];
				short propertyReferencedId = bonusTypeConfig.PropertyBonusList.First<PropertyAndValue>().PropertyId;
				ECharacterPropertyReferencedType type = CharacterPropertyReferenced.Instance[propertyReferencedId].Type;
				bool isQualification = type >= ECharacterPropertyReferencedType.QualificationNeigong && type <= ECharacterPropertyReferencedType.QualificationCombatMusic;
				bool isActive = index < activeCount;
				bool flag = isQualification;
				if (flag)
				{
					int num = A_2.maxQualificationCount;
					A_2.maxQualificationCount = num + 1;
					bool flag2 = isActive;
					if (flag2)
					{
						num = A_2.curQualificationCount;
						A_2.curQualificationCount = num + 1;
					}
				}
				else
				{
					int num = A_2.maxPropertyCount;
					A_2.maxPropertyCount = num + 1;
					bool flag3 = isActive;
					if (flag3)
					{
						num = A_2.curPropertyCount;
						A_2.curPropertyCount = num + 1;
					}
				}
			}
		}

		// Token: 0x060069E4 RID: 27108 RVA: 0x0030CD24 File Offset: 0x0030AF24
		[CompilerGenerated]
		internal static void <RefreshAddProperty>g__CalcProperty|20_1(List<short> legendaryBookAddPropertyList, int activeCount, ref TooltipLegendaryBook.<>c__DisplayClass20_0 A_2)
		{
			for (int i = 0; i < legendaryBookAddPropertyList.Count; i++)
			{
				short legendaryBookAddProperty = legendaryBookAddPropertyList[i];
				LegendaryBookPropertyBonusTypeItem bonusTypeConfig = LegendaryBookPropertyBonusType.Instance[legendaryBookAddProperty];
				bool isActive = i < activeCount;
				bool flag = !isActive;
				if (!flag)
				{
					foreach (PropertyAndValue propertyAndValue in bonusTypeConfig.PropertyBonusList)
					{
						ECharacterPropertyReferencedType type = CharacterPropertyReferenced.Instance[propertyAndValue.PropertyId].Type;
						A_2.propertyDict[type] = A_2.propertyDict.GetOrDefault(type) + (int)propertyAndValue.Value;
					}
				}
			}
		}

		// Token: 0x04004C35 RID: 19509
		[Header("武学类型")]
		[SerializeField]
		private TooltipItemProperty propertyType;

		// Token: 0x04004C36 RID: 19510
		[Header("解读奇书")]
		[SerializeField]
		private GameObject taiwuArea;

		// Token: 0x04004C37 RID: 19511
		[SerializeField]
		private TooltipItemProperty propertyChallengeCount;

		// Token: 0x04004C38 RID: 19512
		[SerializeField]
		private TooltipItemProperty propertyPropertyCount;

		// Token: 0x04004C39 RID: 19513
		[SerializeField]
		private TooltipItemProperty propertyQualificationCount;

		// Token: 0x04004C3A RID: 19514
		[Header("属性加成")]
		[SerializeField]
		private Transform layoutAddProperty;

		// Token: 0x04004C3B RID: 19515
		[Header("放置栏位")]
		[SerializeField]
		private Transform layoutSlot;

		// Token: 0x04004C3C RID: 19516
		[Header("详细模式-放置栏位")]
		[SerializeField]
		private Transform layoutSlotDetail;

		// Token: 0x04004C3D RID: 19517
		private MiscItem configData;

		// Token: 0x04004C3E RID: 19518
		private CombatSkillTypeItem _combatSkillTypeConfig;

		// Token: 0x04004C3F RID: 19519
		private sbyte _combatSkillType;

		// Token: 0x04004C40 RID: 19520
		private TipLegendaryBookDisplayData _displayData;

		// Token: 0x04004C41 RID: 19521
		private bool _isTaiwuOwned;
	}
}
