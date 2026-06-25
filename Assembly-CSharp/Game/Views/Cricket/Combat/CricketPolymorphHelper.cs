using System;
using Config;
using GameData.Domains.Character.Display;

namespace Game.Views.Cricket.Combat
{
	// Token: 0x02000ADF RID: 2783
	public static class CricketPolymorphHelper
	{
		// Token: 0x17000F1D RID: 3869
		// (get) Token: 0x060088F4 RID: 35060 RVA: 0x003F68F5 File Offset: 0x003F4AF5
		public static bool IsCricketPolymorphEnabled
		{
			get
			{
				return SingletonObject.getInstance<DlcManager>().IsDlcInstalled(4528730U);
			}
		}

		// Token: 0x060088F5 RID: 35061 RVA: 0x003F6908 File Offset: 0x003F4B08
		public static bool IsCricketPolymorphCharacter(short templateId)
		{
			return templateId >= 968 && templateId <= 1011;
		}

		// Token: 0x060088F6 RID: 35062 RVA: 0x003F6930 File Offset: 0x003F4B30
		public static bool IsCricketPolymorphCharacter(CharacterDisplayDataForGeneralScrollList charData)
		{
			return charData != null && CricketPolymorphHelper.IsCricketPolymorphCharacter(charData.CharacterTemplateId);
		}

		// Token: 0x060088F7 RID: 35063 RVA: 0x003F6954 File Offset: 0x003F4B54
		public static int FindCricketPolymorphSkill(short templateId, sbyte gender)
		{
			int count = CricketParts.Instance.Count;
			short i = 0;
			while ((int)i < count)
			{
				CricketPartsItem item = CricketParts.Instance[i];
				bool flag = item == null;
				if (!flag)
				{
					bool match = (gender == 1) ? (item.CharacterMale == templateId) : (item.CharacterFemale == templateId);
					bool flag2 = match;
					if (flag2)
					{
						return item.Skill;
					}
				}
				i += 1;
			}
			return -1;
		}

		// Token: 0x060088F8 RID: 35064 RVA: 0x003F69CC File Offset: 0x003F4BCC
		public static int FindCricketSkill(int cricketPartId, int cricketColorId)
		{
			int partId = (cricketPartId > 0) ? cricketPartId : cricketColorId;
			CricketPartsItem item = CricketParts.Instance[partId];
			return (item != null) ? item.Skill : -1;
		}

		// Token: 0x040068D8 RID: 26840
		public const uint CricketPolymorphDlcAppId = 4528730U;

		// Token: 0x040068D9 RID: 26841
		private const short CricketPolymorphTemplateIdMin = 968;

		// Token: 0x040068DA RID: 26842
		private const short CricketPolymorphTemplateIdMax = 1011;
	}
}
