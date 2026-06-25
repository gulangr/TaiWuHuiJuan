using System;
using GameData.Combat.Cricket;
using GameData.Domains.Character.Display;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;

namespace Game.Views.Cricket.Combat
{
	// Token: 0x02000AC7 RID: 2759
	public class CricketCombatDisplayData
	{
		// Token: 0x17000EFD RID: 3837
		// (get) Token: 0x06008812 RID: 34834 RVA: 0x003F2D02 File Offset: 0x003F0F02
		public bool HasSkillReplace
		{
			get
			{
				return this.OriginalSkill != ECricketCombatSkillType.None && this.Data.Skill != this.OriginalSkill;
			}
		}

		// Token: 0x06008813 RID: 34835 RVA: 0x003F2D26 File Offset: 0x003F0F26
		public CricketCombatDisplayData(ItemDisplayData displayData) : this(displayData, null)
		{
		}

		// Token: 0x06008814 RID: 34836 RVA: 0x003F2D34 File Offset: 0x003F0F34
		public CricketCombatDisplayData(ItemDisplayData displayData, CharacterDisplayDataForGeneralScrollList selectedPolymorphCharacter)
		{
			CricketCore core = CricketCoreUtils.BuildCricketBaseProperty(displayData);
			CricketData data;
			bool flag = CricketCombatKit.Board.AllCricketData.TryGetValue(displayData.Key.Id, out data);
			if (flag)
			{
				core += data.SpiritAddProperties;
			}
			this.OriginalSkill = CricketCombatDisplayData.ResolveOriginalSkill(displayData);
			ECricketCombatSkillType skill = CricketCombatDisplayData.ResolveCombatSkill(displayData, selectedPolymorphCharacter, this.OriginalSkill);
			short colorId = displayData.CricketColorId;
			this.Data = new CricketCombatData(colorId == 0, core, skill, (int)displayData.Durability, (int)displayData.MaxDurability);
			bool flag2 = data != null;
			if (flag2)
			{
				this.Data.Injury = data;
				this.Data.Sp = this.Data.MaxSp;
				this.Data.Hp = this.Data.MaxHp;
			}
			this.Name = CricketCombatKit.GetCricketName(displayData);
		}

		// Token: 0x06008815 RID: 34837 RVA: 0x003F2E18 File Offset: 0x003F1018
		private static ECricketCombatSkillType ResolveOriginalSkill(ItemDisplayData displayData)
		{
			bool flag = !CricketPolymorphHelper.IsCricketPolymorphEnabled;
			ECricketCombatSkillType result;
			if (flag)
			{
				result = ECricketCombatSkillType.None;
			}
			else
			{
				int selfSkill = CricketPolymorphHelper.FindCricketSkill((int)displayData.CricketPartId, (int)displayData.CricketColorId);
				result = (ECricketCombatSkillType)((selfSkill >= 0) ? selfSkill : -1);
			}
			return result;
		}

		// Token: 0x06008816 RID: 34838 RVA: 0x003F2E54 File Offset: 0x003F1054
		private static ECricketCombatSkillType ResolveCombatSkill(ItemDisplayData displayData, CharacterDisplayDataForGeneralScrollList selectedPolymorphCharacter, ECricketCombatSkillType originalSkill)
		{
			bool flag = selectedPolymorphCharacter != null && CricketPolymorphHelper.IsCricketPolymorphEnabled;
			if (flag)
			{
				int skillValue = CricketPolymorphHelper.FindCricketPolymorphSkill(selectedPolymorphCharacter.CharacterTemplateId, selectedPolymorphCharacter.Gender);
				bool flag2 = skillValue >= 0;
				if (flag2)
				{
					return (ECricketCombatSkillType)skillValue;
				}
			}
			return originalSkill;
		}

		// Token: 0x04006852 RID: 26706
		public CricketCombatData Data;

		// Token: 0x04006853 RID: 26707
		public ECricketCombatSkillType OriginalSkill;

		// Token: 0x04006854 RID: 26708
		public string Name;
	}
}
