using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000D6B RID: 3435
	public class AccessoryPropMenu : DetailedFilterMenuLogic<ITradeableContent>
	{
		// Token: 0x1700120F RID: 4623
		// (get) Token: 0x0600A84F RID: 43087 RVA: 0x004E0748 File Offset: 0x004DE948
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x17001210 RID: 4624
		// (get) Token: 0x0600A850 RID: 43088 RVA: 0x004E074B File Offset: 0x004DE94B
		public override int Id
		{
			get
			{
				return 5;
			}
		}

		// Token: 0x0600A851 RID: 43089 RVA: 0x004E0750 File Offset: 0x004DE950
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_SelectItem_Column_AccessoryEffect;
		}

		// Token: 0x0600A852 RID: 43090 RVA: 0x004E076C File Offset: 0x004DE96C
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			List<FilterDropdownItemConfig> configs = new List<FilterDropdownItemConfig>();
			for (int i = 0; i < 10; i++)
			{
				configs.Add(new FilterDropdownItemConfig
				{
					Text = LocalStringManager.Get(string.Format("LK_CommonSortAndFilter_Filter_Item_Third_Accessory_{0}", i))
				});
			}
			return configs;
		}

		// Token: 0x0600A853 RID: 43091 RVA: 0x004E07CC File Offset: 0x004DE9CC
		public override bool IsDataMatch(ITradeableContent data, IReadOnlyCollection<int> selectedIndices)
		{
			AccessoryItem accessoryConfig = Accessory.Instance[data.Key.TemplateId];
			bool flag = accessoryConfig == null;
			return !flag && selectedIndices.Any((int index) => this.CheckMatch(accessoryConfig, index));
		}

		// Token: 0x0600A854 RID: 43092 RVA: 0x004E082C File Offset: 0x004DEA2C
		private bool CheckMatch(AccessoryItem accessoryConfig, int index)
		{
			switch (index)
			{
			case 0:
				return accessoryConfig.Strength > 0 || accessoryConfig.Intelligence > 0 || accessoryConfig.Dexterity > 0 || accessoryConfig.Concentration > 0 || accessoryConfig.Vitality > 0 || accessoryConfig.Energy > 0;
			case 1:
				return accessoryConfig.RecoveryOfStance > 0 || accessoryConfig.RecoveryOfBreath > 0 || accessoryConfig.AttackSpeed > 0 || accessoryConfig.MoveSpeed > 0 || accessoryConfig.CastSpeed > 0 || accessoryConfig.WeaponSwitchSpeed > 0 || accessoryConfig.InnerRatio > 0 || accessoryConfig.RecoveryOfQiDisorder > 0 || accessoryConfig.RecoveryOfFlaw > 0 || accessoryConfig.RecoveryOfBlockedAcupoint > 0;
			case 2:
				return accessoryConfig.PenetrateOfOuter > 0 || accessoryConfig.PenetrateOfInner > 0 || accessoryConfig.HitRateStrength > 0 || accessoryConfig.HitRateTechnique > 0 || accessoryConfig.HitRateSpeed > 0 || accessoryConfig.HitRateMind > 0;
			case 3:
				return accessoryConfig.PenetrateResistOfOuter > 0 || accessoryConfig.PenetrateResistOfInner > 0 || accessoryConfig.AvoidRateStrength > 0 || accessoryConfig.AvoidRateTechnique > 0 || accessoryConfig.AvoidRateSpeed > 0 || accessoryConfig.AvoidRateMind > 0;
			case 4:
				return accessoryConfig.ResistOfHotPoison > 0 || accessoryConfig.ResistOfGloomyPoison > 0 || accessoryConfig.ResistOfColdPoison > 0 || accessoryConfig.ResistOfRedPoison > 0 || accessoryConfig.ResistOfRottenPoison > 0 || accessoryConfig.ResistOfIllusoryPoison > 0;
			case 5:
				return accessoryConfig.BonusCombatSkillSect >= 0;
			case 6:
				return accessoryConfig.CombatSkillAddMaxPower > 0;
			case 7:
			{
				bool flag = accessoryConfig.MaxInventoryLoadBonus > 0;
				if (flag)
				{
					return true;
				}
				break;
			}
			case 8:
			{
				bool flag2 = accessoryConfig.DropRateBonus > 0;
				if (flag2)
				{
					return true;
				}
				break;
			}
			case 9:
			{
				bool flag3 = accessoryConfig.BaseCaptureRateBonus > 0;
				if (flag3)
				{
					return true;
				}
				break;
			}
			}
			return true;
		}

		// Token: 0x02002466 RID: 9318
		private enum EAccessoryPropType
		{
			// Token: 0x0400E412 RID: 58386
			MainProp,
			// Token: 0x0400E413 RID: 58387
			SecondaryProp,
			// Token: 0x0400E414 RID: 58388
			AtkProp,
			// Token: 0x0400E415 RID: 58389
			DefProp,
			// Token: 0x0400E416 RID: 58390
			ToxicResist,
			// Token: 0x0400E417 RID: 58391
			Pow,
			// Token: 0x0400E418 RID: 58392
			MaximumPow,
			// Token: 0x0400E419 RID: 58393
			MaxInventoryLoadBonus,
			// Token: 0x0400E41A RID: 58394
			DropRateBonus,
			// Token: 0x0400E41B RID: 58395
			CaptureRateBonus
		}
	}
}
