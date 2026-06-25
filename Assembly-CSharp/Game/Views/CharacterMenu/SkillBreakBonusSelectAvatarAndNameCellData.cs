using System;
using Config;
using GameData.Domains.Character.Display;
using GameData.Domains.Taiwu;

namespace Game.Views.CharacterMenu
{
	// Token: 0x02000B82 RID: 2946
	public class SkillBreakBonusSelectAvatarAndNameCellData
	{
		// Token: 0x06009197 RID: 37271 RVA: 0x0043D7BF File Offset: 0x0043B9BF
		public string GetName()
		{
			return NameCenter.GetMonasticTitleOrDisplayName(this.DisplayData, false);
		}

		// Token: 0x06009198 RID: 37272 RVA: 0x0043D7CD File Offset: 0x0043B9CD
		public string GetGrade()
		{
			return LocalStringManager.Get(string.Format("LK_Grade_{0}", this.Bonus.Grade));
		}

		// Token: 0x06009199 RID: 37273 RVA: 0x0043D7EE File Offset: 0x0043B9EE
		public string GetFavor()
		{
			return (this.DisplayData.CharacterId < 0) ? "-" : CommonUtils.GetFavorStringByLevel(this.Bonus.FavorabilityType);
		}

		// Token: 0x0600919A RID: 37274 RVA: 0x0043D818 File Offset: 0x0043BA18
		public string GetAttainment()
		{
			CombatSkillItem config = CombatSkill.Instance.GetItem(this.SkillId);
			bool flag = this.DisplayData.CharacterId >= 0 && this.Bonus.Type == ESkillBreakPlateBonusType.Friend && config != null;
			string attainment;
			if (flag)
			{
				CombatSkillTypeItem typeConfig = CombatSkillType.Instance.GetItem(config.Type);
				attainment = string.Format("{0}:{1}", typeConfig.Name, this.Bonus.FriendAttainment);
			}
			else
			{
				attainment = "-";
			}
			return attainment;
		}

		// Token: 0x04007022 RID: 28706
		public CharacterDisplayData DisplayData;

		// Token: 0x04007023 RID: 28707
		public SkillBreakPlateBonus Bonus;

		// Token: 0x04007024 RID: 28708
		public short SkillId;
	}
}
