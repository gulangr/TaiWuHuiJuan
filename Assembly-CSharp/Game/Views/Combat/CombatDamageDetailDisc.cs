using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using Game.Components.Combat;
using GameData.Domains.Character;
using GameData.Domains.Combat;
using TMPro;
using UnityEngine;

namespace Game.Views.Combat
{
	// Token: 0x02000AEE RID: 2798
	public class CombatDamageDetailDisc : MonoBehaviour
	{
		// Token: 0x17000F30 RID: 3888
		// (get) Token: 0x06008980 RID: 35200 RVA: 0x003F9F97 File Offset: 0x003F8197
		private CombatModel Model
		{
			get
			{
				return SingletonObject.getInstance<CombatModel>();
			}
		}

		// Token: 0x06008981 RID: 35201 RVA: 0x003F9F9E File Offset: 0x003F819E
		public void SetData(int sectionIndex)
		{
			this._discSectionIndex = sectionIndex;
			this._stateResolved = false;
			this.Refresh();
		}

		// Token: 0x06008982 RID: 35202 RVA: 0x003F9FB6 File Offset: 0x003F81B6
		public void Refresh()
		{
			this.SetState(this.DetermineState());
			this.RefreshAttribute();
			this.RefreshHitTypeIcon();
			this.RefreshChengshu();
			this.RefreshDamageItems();
			this.RefreshCritical();
		}

		// Token: 0x06008983 RID: 35203 RVA: 0x003F9FEC File Offset: 0x003F81EC
		private bool IsInAttackRange()
		{
			DamageCompareData compareData = this.Model.DamageCompareData;
			bool flag = compareData == null;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				CombatSubProcessorCharacter processor = compareData.IsAlly ? this.Model.SelfCharacter : this.Model.EnemyCharacter;
				bool flag2 = processor == null;
				if (flag2)
				{
					result = true;
				}
				else
				{
					OuterAndInnerShorts attackRange = processor.AttackRange;
					int minDist = Mathf.Max((int)this.Model.Config.MinDistance, (int)attackRange.Outer);
					int maxDist = Mathf.Min((int)this.Model.Config.MaxDistance, (int)attackRange.Inner);
					result = (minDist <= (int)this.Model.CurrentDistance && (int)this.Model.CurrentDistance <= maxDist);
				}
			}
			return result;
		}

		// Token: 0x06008984 RID: 35204 RVA: 0x003FA0B4 File Offset: 0x003F82B4
		private CombatDamageDetailDisc.EState DetermineState()
		{
			SkillDamageData damageData = this.Model.SkillDamageData;
			SkillDamageSectionData mySection = null;
			bool flag = ((damageData != null) ? damageData.Sections : null) != null;
			if (flag)
			{
				damageData.Sections.TryGetValue(this._discSectionIndex, out mySection);
			}
			bool flag2 = mySection != null && mySection.Result >= ESkillDamageSectionResult.Checked;
			CombatDamageDetailDisc.EState result;
			if (flag2)
			{
				this._stateResolved = true;
				result = ((mySection.Result >= ESkillDamageSectionResult.Hit) ? CombatDamageDetailDisc.EState.Hit : CombatDamageDetailDisc.EState.Miss);
			}
			else
			{
				bool stateResolved = this._stateResolved;
				if (stateResolved)
				{
					result = this._currentState;
				}
				else
				{
					bool flag3 = this.HasSectionDamageData();
					if (flag3)
					{
						result = CombatDamageDetailDisc.EState.Hit;
					}
					else
					{
						result = CombatDamageDetailDisc.EState.NotConducted;
					}
				}
			}
			return result;
		}

		// Token: 0x06008985 RID: 35205 RVA: 0x003FA154 File Offset: 0x003F8354
		private bool HasSectionDamageData()
		{
			DamageCompareData compareData = this.Model.DamageCompareData;
			bool flag = compareData == null || this._discSectionIndex < 0 || this._discSectionIndex >= 3;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = compareData.HitValue[this._discSectionIndex] < 0 || compareData.HitType[this._discSectionIndex] < 0;
				result = (!flag2 && CombatDamageDetailDisc.HasAnyDamageForSection(this._discSectionIndex, compareData, this.Model.SkillDamageData));
			}
			return result;
		}

		// Token: 0x06008986 RID: 35206 RVA: 0x003FA1D7 File Offset: 0x003F83D7
		public CombatDamageDetailDisc.EState GetCurrentState()
		{
			return this._currentState;
		}

		// Token: 0x06008987 RID: 35207 RVA: 0x003FA1E0 File Offset: 0x003F83E0
		private void SetState(CombatDamageDetailDisc.EState state)
		{
			this._currentState = state;
			this.successNode.SetActive(state == CombatDamageDetailDisc.EState.Hit);
			this.missNode.SetActive(state == CombatDamageDetailDisc.EState.Miss);
			this.notTriggerNode.SetActive(state == CombatDamageDetailDisc.EState.NotConducted);
			bool flag = state == CombatDamageDetailDisc.EState.Miss;
			if (flag)
			{
				this.missLabel.text = (this.IsInAttackRange() ? LanguageKey.LK_CombatDamageDetail_SectionStat_1.Tr() : LanguageKey.LK_CombatDamageDetail_SectionStat_1_OutOfRange.Tr());
			}
		}

		// Token: 0x06008988 RID: 35208 RVA: 0x003FA25C File Offset: 0x003F845C
		private void RefreshAttribute()
		{
			DamageCompareData data = this.Model.DamageCompareData;
			bool flag = data == null;
			if (!flag)
			{
				bool flag2 = this._discSectionIndex < 0 || this._discSectionIndex >= 3;
				if (!flag2)
				{
					bool flag3 = data.HitValue[this._discSectionIndex] < 0 || data.HitType[this._discSectionIndex] < 0;
					if (!flag3)
					{
						sbyte hitType = data.HitType[this._discSectionIndex];
						int hitVal = data.HitValue[this._discSectionIndex];
						int avoidVal = data.AvoidValue[this._discSectionIndex];
						string attackIcon = "ui9_icon_attribute_hit_big_" + hitType.ToString();
						string defendIcon = "ui9_icon_attribute_avoid_big_" + hitType.ToString();
						string attackValStr = (hitVal >= 0) ? CommonUtils.GetDisplayStringForNum(hitVal, 100000) : "-";
						string defendValStr = (avoidVal >= 0) ? CommonUtils.GetDisplayStringForNum(avoidVal, 100000) : "-";
						bool isAlly = data.IsAlly;
						if (isAlly)
						{
							this.attackerIcon.SetSprite(attackIcon, false, null);
							this.attackerValue.text = attackValStr;
							this.defenderIcon.SetSprite(defendIcon, false, null);
							this.defenderValue.text = defendValStr;
						}
						else
						{
							this.attackerIcon.SetSprite(defendIcon, false, null);
							this.attackerValue.text = defendValStr;
							this.defenderIcon.SetSprite(attackIcon, false, null);
							this.defenderValue.text = attackValStr;
						}
					}
				}
			}
		}

		// Token: 0x06008989 RID: 35209 RVA: 0x003FA3E0 File Offset: 0x003F85E0
		private void RefreshHitTypeIcon()
		{
			DamageCompareData data = this.Model.DamageCompareData;
			bool flag = data == null;
			if (!flag)
			{
				bool flag2 = this._discSectionIndex < 0 || this._discSectionIndex >= 3;
				if (!flag2)
				{
					bool flag3 = data.HitValue[this._discSectionIndex] < 0 || data.HitType[this._discSectionIndex] < 0;
					if (!flag3)
					{
						sbyte hitType = data.HitType[this._discSectionIndex];
						string iconName = "ui9_icon_attribute_hit_big_" + hitType.ToString();
						for (int i = 0; i < this.hitTypeIcons.Length; i++)
						{
							this.hitTypeIcons[i].SetSprite(iconName, false, null);
						}
					}
				}
			}
		}

		// Token: 0x0600898A RID: 35210 RVA: 0x003FA4A4 File Offset: 0x003F86A4
		private void RefreshChengshu()
		{
			DamageCompareData data = this.Model.DamageCompareData;
			bool flag = data != null && this._discSectionIndex >= 0 && this._discSectionIndex < 3 && data.HitValue[this._discSectionIndex] >= 0 && data.HitType[this._discSectionIndex] >= 0;
			if (flag)
			{
				sbyte hitType = data.HitType[this._discSectionIndex];
				this.chengshuImage.SetSpriteOnly(string.Format("ui9_back_damage_detail_chengshu_{0}", hitType), false, null);
				bool flag2 = data.SkillId >= 0;
				if (flag2)
				{
					CombatSkillItem skillConfig = CombatSkill.Instance[data.SkillId];
					bool flag3 = skillConfig != null && hitType >= 0 && (int)hitType < skillConfig.PerHitDamageRateDistribution.Length;
					if (flag3)
					{
						this.chengshuImage.fillAmount = (float)skillConfig.PerHitDamageRateDistribution[(int)hitType] / 100f;
					}
				}
			}
		}

		// Token: 0x0600898B RID: 35211 RVA: 0x003FA58C File Offset: 0x003F878C
		private void RefreshDamageItems()
		{
			DamageCompareData compareData = this.Model.DamageCompareData;
			bool flag = compareData == null || this._discSectionIndex < 0 || this._discSectionIndex >= 3;
			if (flag)
			{
				this.damageContainer.gameObject.SetActive(false);
			}
			else
			{
				bool flag2 = compareData.HitValue[this._discSectionIndex] < 0 || compareData.HitType[this._discSectionIndex] < 0;
				if (flag2)
				{
					this.damageContainer.gameObject.SetActive(false);
				}
				else
				{
					Dictionary<DefeatMarkKey, int> aggregated = CombatDamageDetailDisc.GetAggregatedDamage(this._discSectionIndex, compareData, this.Model.SkillDamageData);
					bool flag3 = aggregated.Count == 0;
					if (flag3)
					{
						this.damageContainer.gameObject.SetActive(false);
					}
					else
					{
						this.damageContainer.gameObject.SetActive(true);
						DefeatMarkKey[] sortedKeys = aggregated.Keys.OrderBy(new Func<DefeatMarkKey, int>(CombatDamageDetailDisc.GetSectionItemOrder)).ToArray<DefeatMarkKey>();
						CommonUtils.PrepareEnoughChildren(this.damageContainer, this.damageItemPrefab.gameObject, sortedKeys.Length, null);
						for (int i = 0; i < sortedKeys.Length; i++)
						{
							CombatDamageDetailSectionItem item = this.damageContainer.GetChild(i).GetComponent<CombatDamageDetailSectionItem>();
							item.Set(sortedKeys[i], aggregated[sortedKeys[i]]);
						}
					}
				}
			}
		}

		// Token: 0x0600898C RID: 35212 RVA: 0x003FA6FC File Offset: 0x003F88FC
		private static int GetSectionItemOrder(DefeatMarkKey key)
		{
			EMarkType type = key.Type;
			if (!true)
			{
			}
			int result;
			switch (type)
			{
			case EMarkType.Outer:
				result = key.SubType * 2;
				goto IL_67;
			case EMarkType.Inner:
				result = key.SubType * 2 + 1;
				goto IL_67;
			case EMarkType.Poison:
				result = 20 + key.SubType;
				goto IL_67;
			case EMarkType.Mind:
				result = 15;
				goto IL_67;
			case EMarkType.Fatal:
				result = 14;
				goto IL_67;
			}
			result = 99;
			IL_67:
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x0600898D RID: 35213 RVA: 0x003FA77C File Offset: 0x003F897C
		private void RefreshCritical()
		{
			bool isCrit = false;
			SkillDamageData damageData = this.Model.SkillDamageData;
			bool flag = ((damageData != null) ? damageData.Sections : null) != null;
			if (flag)
			{
				int idx = this._discSectionIndex;
				SkillDamageSectionData section;
				bool flag2 = damageData.Sections.TryGetValue(idx, out section);
				if (flag2)
				{
					isCrit = section.Critical;
				}
			}
			this.normalNode.SetActive(!isCrit);
			this.criticalNode.SetActive(isCrit);
		}

		// Token: 0x0600898E RID: 35214 RVA: 0x003FA7F0 File Offset: 0x003F89F0
		private static List<int> GetOrphanSectionIndices(int sectionIndex, DamageCompareData compareData)
		{
			List<int> result = new List<int>();
			int lowerBound = -1;
			for (int i = sectionIndex - 1; i >= 0; i--)
			{
				bool flag = compareData.HitValue[i] >= 0 && compareData.HitType[i] >= 0;
				if (flag)
				{
					lowerBound = i;
					break;
				}
			}
			for (int j = lowerBound + 1; j < sectionIndex; j++)
			{
				bool flag2 = compareData.HitValue[j] < 0 || compareData.HitType[j] < 0;
				if (flag2)
				{
					result.Add(j);
				}
			}
			bool isLast = true;
			for (int k = sectionIndex + 1; k < 3; k++)
			{
				bool flag3 = compareData.HitValue[k] >= 0 && compareData.HitType[k] >= 0;
				if (flag3)
				{
					isLast = false;
					break;
				}
			}
			bool flag4 = isLast;
			if (flag4)
			{
				for (int l = sectionIndex + 1; l < 3; l++)
				{
					bool flag5 = compareData.HitValue[l] < 0 || compareData.HitType[l] < 0;
					if (flag5)
					{
						result.Add(l);
					}
				}
			}
			return result;
		}

		// Token: 0x0600898F RID: 35215 RVA: 0x003FA924 File Offset: 0x003F8B24
		public static bool HasAnyDamageForSection(int sectionIndex, DamageCompareData compareData, SkillDamageData damageData)
		{
			bool flag = ((damageData != null) ? damageData.Sections : null) == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = CombatDamageDetailDisc.SectionHasValues(damageData, sectionIndex);
				if (flag2)
				{
					result = true;
				}
				else
				{
					foreach (int orphan in CombatDamageDetailDisc.GetOrphanSectionIndices(sectionIndex, compareData))
					{
						bool flag3 = CombatDamageDetailDisc.SectionHasValues(damageData, orphan);
						if (flag3)
						{
							return true;
						}
					}
					result = false;
				}
			}
			return result;
		}

		// Token: 0x06008990 RID: 35216 RVA: 0x003FA9B4 File Offset: 0x003F8BB4
		public static Dictionary<DefeatMarkKey, int> GetAggregatedDamage(int sectionIndex, DamageCompareData compareData, SkillDamageData damageData)
		{
			Dictionary<DefeatMarkKey, int> result = new Dictionary<DefeatMarkKey, int>();
			bool flag = ((damageData != null) ? damageData.Sections : null) == null;
			Dictionary<DefeatMarkKey, int> result2;
			if (flag)
			{
				result2 = result;
			}
			else
			{
				CombatDamageDetailDisc.AccumulateSectionValues(result, damageData, sectionIndex);
				foreach (int orphan in CombatDamageDetailDisc.GetOrphanSectionIndices(sectionIndex, compareData))
				{
					CombatDamageDetailDisc.AccumulateSectionValues(result, damageData, orphan);
				}
				result2 = result;
			}
			return result2;
		}

		// Token: 0x06008991 RID: 35217 RVA: 0x003FAA3C File Offset: 0x003F8C3C
		private static void AccumulateSectionValues(Dictionary<DefeatMarkKey, int> result, SkillDamageData damageData, int sectionIndex)
		{
			SkillDamageSectionData section;
			bool flag = !damageData.Sections.TryGetValue(sectionIndex, out section);
			if (!flag)
			{
				bool flag2 = ((section != null) ? section.Values : null) == null;
				if (!flag2)
				{
					foreach (KeyValuePair<DefeatMarkKey, int> kv in section.Values)
					{
						bool flag3 = result.ContainsKey(kv.Key);
						if (flag3)
						{
							DefeatMarkKey key = kv.Key;
							result[key] += kv.Value;
						}
						else
						{
							result[kv.Key] = kv.Value;
						}
					}
				}
			}
		}

		// Token: 0x06008992 RID: 35218 RVA: 0x003FAB14 File Offset: 0x003F8D14
		private static bool SectionHasValues(SkillDamageData damageData, int sectionIndex)
		{
			SkillDamageSectionData section;
			return damageData.Sections.TryGetValue(sectionIndex, out section) && ((section != null) ? section.Values : null) != null && section.Values.Count > 0;
		}

		// Token: 0x04006960 RID: 26976
		private int _discSectionIndex = -1;

		// Token: 0x04006961 RID: 26977
		[Header("状态节点")]
		[SerializeField]
		private GameObject successNode;

		// Token: 0x04006962 RID: 26978
		[SerializeField]
		private GameObject missNode;

		// Token: 0x04006963 RID: 26979
		[SerializeField]
		private TextMeshProUGUI missLabel;

		// Token: 0x04006964 RID: 26980
		[SerializeField]
		private GameObject notTriggerNode;

		// Token: 0x04006965 RID: 26981
		[Header("攻防属性")]
		[SerializeField]
		private CImage attackerIcon;

		// Token: 0x04006966 RID: 26982
		[SerializeField]
		private TextMeshProUGUI attackerValue;

		// Token: 0x04006967 RID: 26983
		[SerializeField]
		private CImage defenderIcon;

		// Token: 0x04006968 RID: 26984
		[SerializeField]
		private TextMeshProUGUI defenderValue;

		// Token: 0x04006969 RID: 26985
		[Header("属性图标")]
		[SerializeField]
		private CImage[] hitTypeIcons;

		// Token: 0x0400696A RID: 26986
		[Header("成数")]
		[SerializeField]
		private CImage chengshuImage;

		// Token: 0x0400696B RID: 26987
		[Header("伤害条目")]
		[SerializeField]
		private RectTransform damageContainer;

		// Token: 0x0400696C RID: 26988
		[SerializeField]
		private CombatDamageDetailSectionItem damageItemPrefab;

		// Token: 0x0400696D RID: 26989
		[Header("暴击")]
		[SerializeField]
		private GameObject normalNode;

		// Token: 0x0400696E RID: 26990
		[SerializeField]
		private GameObject criticalNode;

		// Token: 0x0400696F RID: 26991
		private CombatDamageDetailDisc.EState _currentState;

		// Token: 0x04006970 RID: 26992
		private bool _stateResolved;

		// Token: 0x020020B6 RID: 8374
		public enum EState
		{
			// Token: 0x0400D1F3 RID: 53747
			NotConducted,
			// Token: 0x0400D1F4 RID: 53748
			Hit,
			// Token: 0x0400D1F5 RID: 53749
			Miss
		}
	}
}
