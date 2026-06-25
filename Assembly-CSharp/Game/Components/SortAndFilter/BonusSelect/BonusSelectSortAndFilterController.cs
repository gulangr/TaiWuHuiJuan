using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using GameData.Domains.Taiwu.Display;

namespace Game.Components.SortAndFilter.BonusSelect
{
	// Token: 0x02000E8B RID: 3723
	public class BonusSelectSortAndFilterController : SortAndFilterController<SkillBreakBonusSelectableItem>
	{
		// Token: 0x0600ACFC RID: 44284 RVA: 0x004EFF3B File Offset: 0x004EE13B
		public BonusSelectSortAndFilterController(ISortAndFilterView sortAndFilter) : base(sortAndFilter, LanguageKey.EventEditor_Error_DuplicateGroupKey)
		{
			this.SortController = new BonusSelectSortController();
		}

		// Token: 0x0600ACFD RID: 44285 RVA: 0x004EFF52 File Offset: 0x004EE152
		public void SetContext(short skillId)
		{
			this._skillId = skillId;
		}

		// Token: 0x0600ACFE RID: 44286 RVA: 0x004EFF5C File Offset: 0x004EE15C
		protected override IEnumerable<FilterLineBase<SkillBreakBonusSelectableItem>> GenerateFilterLines()
		{
			yield return new MainFilterLine();
			yield return new RelationSecondaryFilterLine();
			yield return new BookSecondaryFilterLine();
			yield return new MedicineSecondaryFilterLine();
			yield return new MaterialSecondaryFilterLine();
			yield return new FoodSecondaryFilterLine();
			yield return new BloodDewSecondaryFilterLine();
			yield break;
		}

		// Token: 0x0600ACFF RID: 44287 RVA: 0x004EFF6C File Offset: 0x004EE16C
		protected override void OnFilterChanged(int lineId)
		{
			base.OnFilterChanged(lineId);
			bool flag = lineId != 0;
			if (!flag)
			{
				this.RefreshSubFilterDisabledByEffectType();
			}
		}

		// Token: 0x0600AD00 RID: 44288 RVA: 0x004EFF94 File Offset: 0x004EE194
		private void RefreshSubFilterDisabledByEffectType()
		{
			int lineIndex = base.GetLineIndexById(0);
			bool flag = lineIndex < 0 || lineIndex >= base.SortAndFilterState.LineStates.Count;
			if (!flag)
			{
				LineState lineState = base.SortAndFilterState.LineStates[lineIndex];
				EBonusItemType typeFilter = (EBonusItemType)lineState.ToggleGroupState.Index;
				ELineId subFilterLineId = ELineId.MainFilter;
				IReadOnlyList<sbyte> filterTypeArray = null;
				bool found = true;
				switch (typeFilter)
				{
				case EBonusItemType.Character:
					subFilterLineId = ELineId.RelationSecondary;
					filterTypeArray = RelationTypeSecondaryMenu.RelationFilterTypes;
					break;
				case EBonusItemType.Book:
					subFilterLineId = ELineId.BookSecondary;
					filterTypeArray = BookTypeSecondaryMenu.LifeSkillBookFilterTypes;
					break;
				case EBonusItemType.Medicine:
					subFilterLineId = ELineId.MedicineSecondary;
					filterTypeArray = MedicineTypeSecondaryMenu.MedicineFilterTypes;
					break;
				case EBonusItemType.Material:
					subFilterLineId = ELineId.MaterialSecondary;
					filterTypeArray = MaterialTypeSecondaryMenu.MaterialFilterTypes;
					break;
				case EBonusItemType.Food:
					subFilterLineId = ELineId.FoodSecondary;
					filterTypeArray = FoodTypeSecondaryMenu.FoodFilterTypes;
					break;
				case EBonusItemType.BloodDew:
					subFilterLineId = ELineId.BloodDewSecondary;
					filterTypeArray = BloodDewTypeSecondaryMenu.BloodDewFilterTypes;
					break;
				default:
					found = false;
					break;
				}
				bool flag2 = !found || filterTypeArray == null;
				if (!flag2)
				{
					CombatSkillItem combatSkillItem = CombatSkill.Instance.GetItem(this._skillId);
					sbyte equipType = combatSkillItem.EquipType;
					string disabledTooltip = LocalStringManager.Get(LanguageKey.LK_Skill_Break_BonusSelect_SubFilterToggle_Tips_Disable);
					for (int i = 0; i < filterTypeArray.Count; i++)
					{
						SkillBreakBonusEffectItem effectConfig = SkillBreakBonusEffect.Instance[filterTypeArray[i]];
						if (!true)
						{
						}
						int num;
						switch (equipType)
						{
						case 0:
							num = (int)effectConfig.EffectNeigong;
							break;
						case 1:
							num = (int)effectConfig.EffectAttack;
							break;
						case 2:
							num = (int)effectConfig.EffectAgile;
							break;
						case 3:
							num = (int)effectConfig.EffectDefense;
							break;
						case 4:
							num = (int)effectConfig.EffectAssist;
							break;
						default:
							num = -1;
							break;
						}
						if (!true)
						{
						}
						int implementId = num;
						bool hasImplementId = implementId >= 0;
						bool disabledByCombatSkill = this.DisabledByCombatSkill(effectConfig.TemplateId, combatSkillItem);
						bool interactable = hasImplementId && !disabledByCombatSkill;
						base.SetDropdownOptionInteractable((int)subFilterLineId, 0, i, interactable, disabledTooltip);
					}
				}
			}
		}

		// Token: 0x0600AD01 RID: 44289 RVA: 0x004F0180 File Offset: 0x004EE380
		private bool DisabledByCombatSkill(sbyte filterEffectTemplateId, CombatSkillItem combatSkillItem)
		{
			bool flag = filterEffectTemplateId == 41;
			bool result;
			if (flag)
			{
				result = MedicineTypeSecondaryMenu.PoisonGroup.Select(delegate(sbyte key)
				{
					List<sbyte> invalidBreakBonusTypes2 = combatSkillItem.InvalidBreakBonusTypes;
					return invalidBreakBonusTypes2 != null && invalidBreakBonusTypes2.Contains(key);
				}).All((bool disabled) => disabled);
			}
			else
			{
				List<sbyte> invalidBreakBonusTypes = combatSkillItem.InvalidBreakBonusTypes;
				result = (invalidBreakBonusTypes != null && invalidBreakBonusTypes.Contains(filterEffectTemplateId));
			}
			return result;
		}

		// Token: 0x040085C2 RID: 34242
		private short _skillId;
	}
}
