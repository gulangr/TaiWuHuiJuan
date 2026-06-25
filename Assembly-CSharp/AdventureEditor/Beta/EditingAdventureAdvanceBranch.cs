using System;
using Config;

namespace AdventureEditor.Beta
{
	// Token: 0x020006AC RID: 1708
	public class EditingAdventureAdvanceBranch : EditingAdventureBranch
	{
		// Token: 0x06004FAF RID: 20399 RVA: 0x002540D8 File Offset: 0x002522D8
		public override string ToString()
		{
			string openCondition = LocalStringManager.Get(LanguageKey.UI_AdventureEditor_AdvanceBranch_Info_OpenCondition_NotSet);
			bool flag = -1 != this.EnterSkillType;
			if (flag)
			{
				openCondition = LocalStringManager.GetFormat(LanguageKey.UI_AdventureEditor_AdvanceBranch_Info_OpenCondition, LifeSkillType.Instance[this.EnterSkillType].Name, this.EnterSkillRequire);
			}
			return openCondition;
		}

		// Token: 0x06004FB0 RID: 20400 RVA: 0x00254134 File Offset: 0x00252334
		public AdventureAdvancedBranch ToAdventureAdvancedBranch()
		{
			AdventureAdvancedBranch branch = new AdventureAdvancedBranch
			{
				ParentBranchId = this.ParentBranchId,
				EnterSkillType = this.EnterSkillType,
				EnterSkillRequire = this.EnterSkillRequire
			};
			base.ToAdventureBranch(branch);
			return branch;
		}

		// Token: 0x040036E4 RID: 14052
		public int ParentBranchId;

		// Token: 0x040036E5 RID: 14053
		public sbyte EnterSkillType;

		// Token: 0x040036E6 RID: 14054
		public short EnterSkillRequire;

		// Token: 0x040036E7 RID: 14055
		public string BelongBranchGuid;
	}
}
