using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork;
using Game.Components.Combat;
using GameData.Domains.Combat;
using TMPro;
using UnityEngine;

namespace Game.Views.Combat
{
	// Token: 0x02000B38 RID: 2872
	public class ViewCombatDamageDetailLegacy : UIBase
	{
		// Token: 0x17000F9A RID: 3994
		// (get) Token: 0x06008EB9 RID: 36537 RVA: 0x00426C1E File Offset: 0x00424E1E
		private CombatModel Model
		{
			get
			{
				return SingletonObject.getInstance<CombatModel>();
			}
		}

		// Token: 0x17000F9B RID: 3995
		// (get) Token: 0x06008EBA RID: 36538 RVA: 0x00426C25 File Offset: 0x00424E25
		private DamageCompareData Data
		{
			get
			{
				return this.Model.DamageCompareData;
			}
		}

		// Token: 0x06008EBB RID: 36539 RVA: 0x00426C32 File Offset: 0x00424E32
		public override void OnInit(ArgumentBox argsBox)
		{
			this.SetupEvents();
			this.RefreshData();
		}

		// Token: 0x06008EBC RID: 36540 RVA: 0x00426C43 File Offset: 0x00424E43
		private void OnDisable()
		{
			this.RemoveEvents();
		}

		// Token: 0x06008EBD RID: 36541 RVA: 0x00426C4D File Offset: 0x00424E4D
		private void SetupEvents()
		{
			this.Model.AddEvent(ECombatEvents.OnDamageCompareDataChanged, new OnCombatEvent(this.OnDamageCompareDataChanged));
			this.Model.AddEvent(ECombatEvents.OnSkillDamageDataChanged, new OnCombatEvent(this.OnTrackingDamageDataChanged));
		}

		// Token: 0x06008EBE RID: 36542 RVA: 0x00426C84 File Offset: 0x00424E84
		private void RemoveEvents()
		{
			this.Model.RemoveEvent(ECombatEvents.OnDamageCompareDataChanged, new OnCombatEvent(this.OnDamageCompareDataChanged));
			this.Model.RemoveEvent(ECombatEvents.OnSkillDamageDataChanged, new OnCombatEvent(this.OnTrackingDamageDataChanged));
		}

		// Token: 0x06008EBF RID: 36543 RVA: 0x00426CBB File Offset: 0x00424EBB
		private void OnDamageCompareDataChanged()
		{
			this.RefreshData();
		}

		// Token: 0x06008EC0 RID: 36544 RVA: 0x00426CC5 File Offset: 0x00424EC5
		private void OnTrackingDamageDataChanged()
		{
			this.RefreshData();
		}

		// Token: 0x06008EC1 RID: 36545 RVA: 0x00426CD0 File Offset: 0x00424ED0
		private void RefreshData()
		{
			bool flag = this.Data == null || this.Data.SkillId < 0;
			if (!flag)
			{
				this.RefreshAttackDefense();
				this.RefreshHitAvoid();
				this.RefreshEquipment();
				this.RefreshDamage();
				this.RefreshPoison();
			}
		}

		// Token: 0x06008EC2 RID: 36546 RVA: 0x00426D24 File Offset: 0x00424F24
		private void RefreshAttackDefense()
		{
			bool isAlly = this.Data.IsAlly;
			long outerAttack = (long)Mathf.Max(this.Data.OuterAttackValue, 1);
			long outerDefend = (long)Mathf.Max(this.Data.OuterDefendValue, 1);
			long innerAttack = (long)Mathf.Max(this.Data.InnerAttackValue, 1);
			long innerDefend = (long)Mathf.Max(this.Data.InnerDefendValue, 1);
			bool flag = this.Data.OuterAttackValue >= 0;
			if (flag)
			{
				bool selfHigher = outerAttack > outerDefend;
				bool enemyHigher = outerDefend > outerAttack;
				bool flag2 = isAlly;
				if (flag2)
				{
					this.selfOuterValue.Set("ui9_icon_attribute_attack_big_0", LocalStringManager.Get(LanguageKey.LK_Penetrate_Outer), CommonUtils.GetDisplayStringForNum(outerAttack), selfHigher);
					this.enemyOuterValue.Set("ui9_icon_attribute_defence_big_0", LocalStringManager.Get(LanguageKey.LK_Penetrate_Resist_Outer), CommonUtils.GetDisplayStringForNum(outerDefend), enemyHigher);
				}
				else
				{
					this.selfOuterValue.Set("ui9_icon_attribute_defence_big_0", LocalStringManager.Get(LanguageKey.LK_Penetrate_Resist_Outer), CommonUtils.GetDisplayStringForNum(outerDefend), enemyHigher);
					this.enemyOuterValue.Set("ui9_icon_attribute_attack_big_0", LocalStringManager.Get(LanguageKey.LK_Penetrate_Outer), CommonUtils.GetDisplayStringForNum(outerAttack), selfHigher);
				}
				int outerPercent = Mathf.Min((int)(outerAttack * 100L / outerDefend), 9999);
				this.outerDamageResult.Set("ui9_icon_attribute_defence_big_0", LanguageKey.LK_Combat_OuterDamage_Bonus.Tr(), string.Format("{0}%", outerPercent));
			}
			bool flag3 = this.Data.InnerAttackValue >= 0;
			if (flag3)
			{
				bool selfHigher2 = innerAttack > innerDefend;
				bool enemyHigher2 = innerDefend > innerAttack;
				bool flag4 = isAlly;
				if (flag4)
				{
					this.selfInnerValue.Set("ui9_icon_attribute_attack_big_1", LocalStringManager.Get(LanguageKey.LK_Penetrate_Inner), CommonUtils.GetDisplayStringForNum(innerAttack), selfHigher2);
					this.enemyInnerValue.Set("ui9_icon_attribute_defence_big_1", LocalStringManager.Get(LanguageKey.LK_Penetrate_Resist_Inner), CommonUtils.GetDisplayStringForNum(innerDefend), enemyHigher2);
				}
				else
				{
					this.selfInnerValue.Set("ui9_icon_attribute_defence_big_1", LocalStringManager.Get(LanguageKey.LK_Penetrate_Resist_Inner), CommonUtils.GetDisplayStringForNum(innerDefend), enemyHigher2);
					this.enemyInnerValue.Set("ui9_icon_attribute_attack_big_1", LocalStringManager.Get(LanguageKey.LK_Penetrate_Inner), CommonUtils.GetDisplayStringForNum(innerAttack), selfHigher2);
				}
				int innerPercent = Mathf.Min((int)(innerAttack * 100L / innerDefend), 9999);
				this.innerDamageResult.Set("ui9_icon_attribute_defence_big_1", LanguageKey.LK_Combat_InnerDamage_Bonus.Tr(), string.Format("{0}%", innerPercent));
			}
		}

		// Token: 0x06008EC3 RID: 36547 RVA: 0x00426F94 File Offset: 0x00425194
		private void RefreshHitAvoid()
		{
			bool isAlly = this.Data.IsAlly;
			for (int i = 0; i < this.hitAvoidRows.Length; i++)
			{
				this.hitAvoidRows[i].SetActive(false);
			}
			for (sbyte hitType = 0; hitType < 4; hitType += 1)
			{
				int index = this.Data.HitType.IndexOf(hitType);
				bool hasData = index >= 0;
				int hitValue = hasData ? this.Data.HitValue[index] : -1;
				int avoidValue = hasData ? this.Data.AvoidValue[index] : -1;
				bool flag = !hasData;
				if (!flag)
				{
					bool flag2 = index < this.hitAvoidRows.Length && this.hitAvoidRows[index] != null;
					if (flag2)
					{
						this.hitAvoidRows[index].SetActive(true);
					}
					bool flag3 = index < this.selfHitAvoidValues.Length && this.selfHitAvoidValues[index] != null;
					if (flag3)
					{
						bool flag4 = isAlly;
						if (flag4)
						{
							bool selfHigher = hitValue > avoidValue;
							this.selfHitAvoidValues[index].Set("ui9_icon_attribute_hit_big_" + hitType.ToString(), ViewCombatDamageDetailLegacy.GetHitValueName(hitType), (hitValue >= 0) ? hitValue.ToString() : "-", selfHigher);
						}
						else
						{
							bool selfHigher2 = avoidValue > hitValue;
							this.selfHitAvoidValues[index].Set("ui9_icon_attribute_avoid_big_" + hitType.ToString(), ViewCombatDamageDetailLegacy.GetAvoidValueName(hitType), (avoidValue >= 0) ? avoidValue.ToString() : "-", selfHigher2);
						}
					}
					bool flag5 = index < this.enemyHitAvoidValues.Length && this.enemyHitAvoidValues[index] != null;
					if (flag5)
					{
						bool flag6 = isAlly;
						if (flag6)
						{
							bool enemyHigher = avoidValue > hitValue;
							this.enemyHitAvoidValues[index].Set("ui9_icon_attribute_avoid_big_" + hitType.ToString(), ViewCombatDamageDetailLegacy.GetAvoidValueName(hitType), (avoidValue >= 0) ? avoidValue.ToString() : "-", enemyHigher);
						}
						else
						{
							bool enemyHigher2 = hitValue > avoidValue;
							this.enemyHitAvoidValues[index].Set("ui9_icon_attribute_hit_big_" + hitType.ToString(), ViewCombatDamageDetailLegacy.GetHitValueName(hitType), (hitValue >= 0) ? hitValue.ToString() : "-", enemyHigher2);
						}
					}
					bool flag7 = index < this.hitResults.Length && this.hitResults[index] != null;
					if (flag7)
					{
						bool flag8 = hasData;
						if (flag8)
						{
							string resultText = (hitValue > avoidValue) ? LanguageKey.LK_Combat_Hit.TrFormat(this.GetHitSuccessLevel(hitType)) : LanguageKey.LK_Combat_Miss.Tr();
							this.hitResults[index].text = resultText;
						}
						else
						{
							this.hitResults[index].text = "-";
						}
					}
				}
			}
		}

		// Token: 0x06008EC4 RID: 36548 RVA: 0x00427284 File Offset: 0x00425484
		private void RefreshEquipment()
		{
			bool isAlly = this.Data.IsAlly;
			bool flag = this.Data.WeaponAttack >= 0 && this.Data.ArmorDefend >= 0;
			if (flag)
			{
				bool weaponHigher = this.Data.WeaponAttack > this.Data.ArmorDefend;
				bool armorHigher = this.Data.ArmorDefend > this.Data.WeaponAttack;
				bool flag2 = isAlly;
				if (flag2)
				{
					this.selfFirstEquipValue.Set("ui9_icon_attribute_attack_big_0", LocalStringManager.Get(LanguageKey.LK_Weapon_Break), this.Data.WeaponAttack.ToString(), weaponHigher);
					this.enemyFirstEquipValue.Set("ui9_icon_attribute_defence_big_0", LocalStringManager.Get(LanguageKey.LK_Armor_Toughness), this.Data.ArmorDefend.ToString(), armorHigher);
				}
				else
				{
					this.selfFirstEquipValue.Set("ui9_icon_attribute_defence_big_0", LocalStringManager.Get(LanguageKey.LK_Armor_Toughness), this.Data.ArmorDefend.ToString(), armorHigher);
					this.enemyFirstEquipValue.Set("ui9_icon_attribute_attack_big_0", LocalStringManager.Get(LanguageKey.LK_Weapon_Break), this.Data.WeaponAttack.ToString(), weaponHigher);
				}
				int attackEffect = this.CalculateEquipmentEffect(this.Data.WeaponAttack, this.Data.ArmorDefend);
				this.firstEquipEffectResult.Set(isAlly ? "ui9_icon_attribute_defence_big_0" : "ui9_icon_attribute_attack_big_0", LanguageKey.LK_Combat_OuterDamage_Bonus.Tr(), string.Format("{0}%", attackEffect));
			}
			bool flag3 = this.Data.WeaponDefend >= 0 && this.Data.ArmorAttack >= 0;
			if (flag3)
			{
				bool weaponDefendHigher = this.Data.WeaponDefend > this.Data.ArmorAttack;
				bool armorAttackHigher = this.Data.ArmorAttack > this.Data.WeaponDefend;
				bool flag4 = isAlly;
				if (flag4)
				{
					this.selfSecondEquipValue.Set("ui9_icon_attribute_defence_big_1", LocalStringManager.Get(LanguageKey.LK_Weapon_Toughness), this.Data.WeaponDefend.ToString(), weaponDefendHigher);
					this.enemySecondEquipValue.Set("ui9_icon_attribute_attack_big_1", LocalStringManager.Get(LanguageKey.LK_Armor_Break), this.Data.ArmorAttack.ToString(), armorAttackHigher);
				}
				else
				{
					this.selfSecondEquipValue.Set("ui9_icon_attribute_defence_big_1", LocalStringManager.Get(LanguageKey.LK_Armor_Break), this.Data.ArmorAttack.ToString(), armorAttackHigher);
					this.enemySecondEquipValue.Set("ui9_icon_attribute_attack_big_1", LocalStringManager.Get(LanguageKey.LK_Weapon_Toughness), this.Data.WeaponDefend.ToString(), weaponDefendHigher);
				}
				int defenseEffect = this.CalculateEquipmentEffect(this.Data.ArmorAttack, this.Data.WeaponDefend);
				this.secondEquipEffectResult.Set(isAlly ? "ui9_icon_attribute_defence_big_1" : "ui9_icon_attribute_attack_big_1", LanguageKey.LK_Combat_InnerDamage_Bonus.Tr(), string.Format("{0}%", defenseEffect));
			}
		}

		// Token: 0x06008EC5 RID: 36549 RVA: 0x00427584 File Offset: 0x00425784
		private void RefreshDamage()
		{
			SkillDamageData data = this.Model.SkillDamageData;
			SkillDamageSectionData total = data.Total;
			int num;
			if (total == null)
			{
				num = 0;
			}
			else
			{
				num = total.Values.Count((KeyValuePair<DefeatMarkKey, int> x) => x.Key.Type != EMarkType.Poison);
			}
			Dictionary<int, SkillDamageSectionData> sections = data.Sections;
			int num2;
			if (sections == null)
			{
				num2 = 0;
			}
			else
			{
				num2 = sections.Values.Sum((SkillDamageSectionData x) => x.Values.Count((KeyValuePair<DefeatMarkKey, int> y) => y.Key.Type != EMarkType.Poison));
			}
			int count = num + num2;
			this.damageArea.SetActive(count > 0);
			bool flag = count == 0;
			if (!flag)
			{
				CommonUtils.PrepareEnoughChildren(this.damageContainer, this.damageItemPrefab.gameObject, count, new CommonUtils.PrepareExtraItemInfo?(new CommonUtils.PrepareExtraItemInfo
				{
					ExtraItemCount = 1,
					TemplateOrder = CommonUtils.EPrepareTemplateOrder.AfterExtraItems
				}));
				int i = 1;
				bool flag2 = data.Sections != null;
				if (flag2)
				{
					foreach (KeyValuePair<int, SkillDamageSectionData> keyValuePair in data.Sections)
					{
						int num3;
						SkillDamageSectionData skillDamageSectionData;
						keyValuePair.Deconstruct(out num3, out skillDamageSectionData);
						int index = num3;
						SkillDamageSectionData section = skillDamageSectionData;
						foreach (KeyValuePair<DefeatMarkKey, int> keyValuePair2 in section.Values)
						{
							DefeatMarkKey defeatMarkKey;
							keyValuePair2.Deconstruct(out defeatMarkKey, out num3);
							DefeatMarkKey markKey = defeatMarkKey;
							int value = num3;
							bool flag3 = markKey.Type == EMarkType.Poison;
							if (!flag3)
							{
								DamageDetailDamageItem item = this.damageContainer.GetChild(i++).GetComponent<DamageDetailDamageItem>();
								item.Set(index, markKey, value);
							}
						}
					}
				}
				bool flag4 = data.Total != null;
				if (flag4)
				{
					foreach (KeyValuePair<DefeatMarkKey, int> keyValuePair2 in data.Total.Values)
					{
						int num3;
						DefeatMarkKey defeatMarkKey;
						keyValuePair2.Deconstruct(out defeatMarkKey, out num3);
						DefeatMarkKey markKey2 = defeatMarkKey;
						int value2 = num3;
						bool flag5 = markKey2.Type == EMarkType.Poison;
						if (!flag5)
						{
							DamageDetailDamageItem item2 = this.damageContainer.GetChild(i++).GetComponent<DamageDetailDamageItem>();
							item2.Set(markKey2, value2);
						}
					}
				}
			}
		}

		// Token: 0x06008EC6 RID: 36550 RVA: 0x00427808 File Offset: 0x00425A08
		private void RefreshPoison()
		{
			SkillDamageData data = this.Model.SkillDamageData;
			SkillDamageSectionData total = data.Total;
			int num;
			if (total == null)
			{
				num = 0;
			}
			else
			{
				num = total.Values.Count((KeyValuePair<DefeatMarkKey, int> x) => x.Key.Type == EMarkType.Poison);
			}
			Dictionary<int, SkillDamageSectionData> sections = data.Sections;
			int num2;
			if (sections == null)
			{
				num2 = 0;
			}
			else
			{
				num2 = sections.Values.Sum((SkillDamageSectionData x) => x.Values.Count((KeyValuePair<DefeatMarkKey, int> y) => y.Key.Type == EMarkType.Poison));
			}
			int count = num + num2;
			this.poisonArea.SetActive(count > 0);
			bool flag = count == 0;
			if (!flag)
			{
				CommonUtils.PrepareEnoughChildren(this.poisonContainer, this.poisonItemPrefab.gameObject, count, new CommonUtils.PrepareExtraItemInfo?(new CommonUtils.PrepareExtraItemInfo
				{
					ExtraItemCount = 1,
					TemplateOrder = CommonUtils.EPrepareTemplateOrder.AfterExtraItems
				}));
				int i = 1;
				bool flag2 = data.Sections != null;
				if (flag2)
				{
					foreach (KeyValuePair<int, SkillDamageSectionData> keyValuePair in data.Sections)
					{
						int num3;
						SkillDamageSectionData skillDamageSectionData;
						keyValuePair.Deconstruct(out num3, out skillDamageSectionData);
						int index = num3;
						SkillDamageSectionData section = skillDamageSectionData;
						foreach (KeyValuePair<DefeatMarkKey, int> keyValuePair2 in section.Values)
						{
							DefeatMarkKey defeatMarkKey;
							keyValuePair2.Deconstruct(out defeatMarkKey, out num3);
							DefeatMarkKey markKey = defeatMarkKey;
							int value = num3;
							bool flag3 = markKey.Type != EMarkType.Poison;
							if (!flag3)
							{
								DamageDetailDamageItem item = this.damageContainer.GetChild(i++).GetComponent<DamageDetailDamageItem>();
								item.Set(index, markKey, value);
							}
						}
					}
				}
				bool flag4 = data.Total != null;
				if (flag4)
				{
					foreach (KeyValuePair<DefeatMarkKey, int> keyValuePair2 in data.Total.Values)
					{
						int num3;
						DefeatMarkKey defeatMarkKey;
						keyValuePair2.Deconstruct(out defeatMarkKey, out num3);
						DefeatMarkKey markKey2 = defeatMarkKey;
						int value2 = num3;
						bool flag5 = markKey2.Type != EMarkType.Poison;
						if (!flag5)
						{
							DamageDetailDamageItem item2 = this.damageContainer.GetChild(i++).GetComponent<DamageDetailDamageItem>();
							item2.Set(markKey2, value2);
						}
					}
				}
			}
		}

		// Token: 0x06008EC7 RID: 36551 RVA: 0x00427A90 File Offset: 0x00425C90
		private int CalculateEquipmentEffect(int attack, int defend)
		{
			bool flag = attack > defend;
			int result;
			if (flag)
			{
				result = 100;
			}
			else
			{
				result = 80 - 10 * attack / Mathf.Max(defend, 1);
			}
			return result;
		}

		// Token: 0x06008EC8 RID: 36552 RVA: 0x00427AC0 File Offset: 0x00425CC0
		private int GetHitSuccessLevel(sbyte hitType)
		{
			bool flag = this.Data.SkillId >= 0;
			int result;
			if (flag)
			{
				CombatSkillItem config = CombatSkill.Instance[this.Data.SkillId];
				sbyte distribution = config.PerHitDamageRateDistribution[(int)hitType];
				result = (int)(distribution / 10);
			}
			else
			{
				result = 10;
			}
			return result;
		}

		// Token: 0x06008EC9 RID: 36553 RVA: 0x00427B10 File Offset: 0x00425D10
		private static string GetHitValueName(sbyte type)
		{
			if (!true)
			{
			}
			string result;
			switch (type)
			{
			case 0:
				result = LocalStringManager.Get(LanguageKey.LK_HitType_0);
				break;
			case 1:
				result = LocalStringManager.Get(LanguageKey.LK_HitType_1);
				break;
			case 2:
				result = LocalStringManager.Get(LanguageKey.LK_HitType_2);
				break;
			case 3:
				result = LocalStringManager.Get(LanguageKey.LK_HitType_3);
				break;
			default:
				result = string.Empty;
				break;
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x06008ECA RID: 36554 RVA: 0x00427B80 File Offset: 0x00425D80
		private static string GetAvoidValueName(sbyte type)
		{
			if (!true)
			{
			}
			string result;
			switch (type)
			{
			case 0:
				result = LocalStringManager.Get(LanguageKey.LK_AvoidType_0);
				break;
			case 1:
				result = LocalStringManager.Get(LanguageKey.LK_AvoidType_1);
				break;
			case 2:
				result = LocalStringManager.Get(LanguageKey.LK_AvoidType_2);
				break;
			case 3:
				result = LocalStringManager.Get(LanguageKey.LK_AvoidType_3);
				break;
			default:
				result = string.Empty;
				break;
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x04006CC9 RID: 27849
		[Header("攻击和防御区域")]
		[SerializeField]
		private CombatDamageDetailPropertyItem selfOuterValue;

		// Token: 0x04006CCA RID: 27850
		[SerializeField]
		private CombatDamageDetailPropertyItem enemyOuterValue;

		// Token: 0x04006CCB RID: 27851
		[SerializeField]
		private CombatDamageDetailResultItem outerDamageResult;

		// Token: 0x04006CCC RID: 27852
		[SerializeField]
		private CombatDamageDetailPropertyItem selfInnerValue;

		// Token: 0x04006CCD RID: 27853
		[SerializeField]
		private CombatDamageDetailPropertyItem enemyInnerValue;

		// Token: 0x04006CCE RID: 27854
		[SerializeField]
		private CombatDamageDetailResultItem innerDamageResult;

		// Token: 0x04006CCF RID: 27855
		[Header("命中和回避区域")]
		[SerializeField]
		private CombatDamageDetailPropertyItem[] selfHitAvoidValues;

		// Token: 0x04006CD0 RID: 27856
		[SerializeField]
		private CombatDamageDetailPropertyItem[] enemyHitAvoidValues;

		// Token: 0x04006CD1 RID: 27857
		[SerializeField]
		private TextMeshProUGUI[] hitResults;

		// Token: 0x04006CD2 RID: 27858
		[SerializeField]
		private GameObject[] hitAvoidRows;

		// Token: 0x04006CD3 RID: 27859
		[Header("装备比较区域")]
		[SerializeField]
		private CombatDamageDetailPropertyItem selfFirstEquipValue;

		// Token: 0x04006CD4 RID: 27860
		[SerializeField]
		private CombatDamageDetailPropertyItem enemyFirstEquipValue;

		// Token: 0x04006CD5 RID: 27861
		[SerializeField]
		private CombatDamageDetailResultItem firstEquipEffectResult;

		// Token: 0x04006CD6 RID: 27862
		[SerializeField]
		private CombatDamageDetailPropertyItem selfSecondEquipValue;

		// Token: 0x04006CD7 RID: 27863
		[SerializeField]
		private CombatDamageDetailPropertyItem enemySecondEquipValue;

		// Token: 0x04006CD8 RID: 27864
		[SerializeField]
		private CombatDamageDetailResultItem secondEquipEffectResult;

		// Token: 0x04006CD9 RID: 27865
		[Header("最终伤害区域")]
		[SerializeField]
		private GameObject damageArea;

		// Token: 0x04006CDA RID: 27866
		[SerializeField]
		private RectTransform damageContainer;

		// Token: 0x04006CDB RID: 27867
		[SerializeField]
		private DamageDetailDamageItem damageItemPrefab;

		// Token: 0x04006CDC RID: 27868
		[Header("施加毒素区域")]
		[SerializeField]
		private GameObject poisonArea;

		// Token: 0x04006CDD RID: 27869
		[SerializeField]
		private RectTransform poisonContainer;

		// Token: 0x04006CDE RID: 27870
		[SerializeField]
		private DamageDetailDamageItem poisonItemPrefab;
	}
}
