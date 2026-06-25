using System;
using System.Collections.Generic;
using Config;

namespace Game.Views.Achievement
{
	// Token: 0x02000C7F RID: 3199
	internal static class AchievementProgressResolver
	{
		// Token: 0x0600A36A RID: 41834 RVA: 0x004C7B40 File Offset: 0x004C5D40
		public static bool TryResolve(short achievementId, out short statId, out int target)
		{
			statId = 0;
			target = 0;
			foreach (StatInfoItem stat in ((IEnumerable<StatInfoItem>)StatInfo.Instance))
			{
				bool flag = stat == null;
				if (!flag)
				{
					EStatInfoSaveType saveType = stat.SaveType;
					bool flag2 = saveType != EStatInfoSaveType.Global && saveType != EStatInfoSaveType.Local;
					if (!flag2)
					{
						int maxValue = stat.MaxValue;
						bool flag3 = maxValue == 1 || maxValue == -1;
						if (!flag3)
						{
							bool flag4 = !AchievementProgressResolver.LinksAchievement(stat, achievementId);
							if (!flag4)
							{
								bool flag5 = stat.MaxValue <= target;
								if (!flag5)
								{
									target = stat.MaxValue;
									statId = stat.TemplateId;
								}
							}
						}
					}
				}
			}
			return statId != 0 && target > 0;
		}

		// Token: 0x0600A36B RID: 41835 RVA: 0x004C7C2C File Offset: 0x004C5E2C
		private static bool LinksAchievement(StatInfoItem stat, short achievementId)
		{
			List<short> achievementIds = stat.AchievementTemplateId;
			bool flag = achievementIds == null || achievementIds.Count == 0;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				foreach (short id in achievementIds)
				{
					bool flag2 = id == achievementId;
					if (flag2)
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}
	}
}
