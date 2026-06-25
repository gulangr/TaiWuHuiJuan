using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using GameData.Domains.Taiwu.Display;

namespace CommonSortAndFilterLegacy.BonusSelect
{
	// Token: 0x020005B4 RID: 1460
	public class BonusSelectSortAndFilterController : CommonSortAndFilterController<SkillBreakBonusSelectableItem>
	{
		// Token: 0x060045C5 RID: 17861 RVA: 0x0020D0CB File Offset: 0x0020B2CB
		public BonusSelectSortAndFilterController(CommonSortAndFilter sortAndFilter) : base(sortAndFilter)
		{
			this.SortController = new BonusSelectSortController();
		}

		// Token: 0x060045C6 RID: 17862 RVA: 0x0020D0E1 File Offset: 0x0020B2E1
		public void SetContext(short skillId)
		{
			this._skillId = skillId;
		}

		// Token: 0x170008BE RID: 2238
		// (get) Token: 0x060045C7 RID: 17863 RVA: 0x0020D0EB File Offset: 0x0020B2EB
		protected override string FilterCustomOrderKey
		{
			get
			{
				return "BonusSelectFilterCustomOrder";
			}
		}

		// Token: 0x060045C8 RID: 17864 RVA: 0x0020D0F2 File Offset: 0x0020B2F2
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

		// Token: 0x060045C9 RID: 17865 RVA: 0x0020D104 File Offset: 0x0020B304
		protected override void OnFilterChanged(int lineId)
		{
			base.OnFilterChanged(lineId);
			bool flag = lineId != 0;
			if (!flag)
			{
				this.RefreshSubFilterDisabledByEffectType();
			}
		}

		// Token: 0x060045CA RID: 17866 RVA: 0x0020D12C File Offset: 0x0020B32C
		private void RefreshSubFilterDisabledByEffectType()
		{
			bool isEmpty = base.IsEmpty;
			if (!isEmpty)
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
		}

		// Token: 0x060045CB RID: 17867 RVA: 0x0020D328 File Offset: 0x0020B528
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

		// Token: 0x04003080 RID: 12416
		private short _skillId;
	}
}
