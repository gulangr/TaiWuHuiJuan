using System;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Taiwu;

namespace Game.Views.GameLineScroll
{
	// Token: 0x02000A1B RID: 2587
	public struct TaiwuLifeSummaryInfo
	{
		// Token: 0x06007EE1 RID: 32481 RVA: 0x003B1F5C File Offset: 0x003B015C
		public readonly AvatarRelatedData GetAvatarRelatedData()
		{
			bool flag = this.TaiwuLifeSummaryData.TaiwuCharId == SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			AvatarRelatedData result;
			if (flag)
			{
				result = this.AvatarRelatedData;
			}
			else
			{
				AbridgedCharacter abridgedCharacter = this.TaiwuLifeSummaryData.AbridgedCharacter;
				result = (((abridgedCharacter != null) ? abridgedCharacter.GenerateAvatarRelatedData() : null) ?? this.AvatarRelatedData);
			}
			return result;
		}

		// Token: 0x040060EC RID: 24812
		public AvatarRelatedData AvatarRelatedData;

		// Token: 0x040060ED RID: 24813
		public TaiwuLifeSummary TaiwuLifeSummaryData;

		// Token: 0x040060EE RID: 24814
		public string Surname;

		// Token: 0x040060EF RID: 24815
		public string GivenName;

		// Token: 0x040060F0 RID: 24816
		public short TitleId;
	}
}
