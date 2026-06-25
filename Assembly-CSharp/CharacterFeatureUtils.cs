using System;
using System.Collections.Generic;
using Config;

// Token: 0x02000106 RID: 262
public static class CharacterFeatureUtils
{
	// Token: 0x0600092F RID: 2351 RVA: 0x0003F388 File Offset: 0x0003D588
	public static List<short> GetDisplayFeatureIds(List<short> featureIds)
	{
		List<short> result = new List<short>();
		bool flag = featureIds == null;
		List<short> result2;
		if (flag)
		{
			result2 = result;
		}
		else
		{
			for (int i = 0; i < featureIds.Count; i++)
			{
				CharacterFeatureItem config = CharacterFeature.Instance.GetItem(featureIds[i]);
				bool flag2 = config != null && !config.Hidden;
				if (flag2)
				{
					result.Add(featureIds[i]);
				}
			}
			result2 = result;
		}
		return result2;
	}

	// Token: 0x06000930 RID: 2352 RVA: 0x0003F400 File Offset: 0x0003D600
	public static List<short> GetDisplayFeatureIds(short birthFeatureId, List<short> featureIds)
	{
		List<short> result = new List<short>
		{
			birthFeatureId
		};
		bool flag = featureIds == null;
		List<short> result2;
		if (flag)
		{
			result2 = result;
		}
		else
		{
			for (int i = 0; i < featureIds.Count; i++)
			{
				CharacterFeatureItem config = CharacterFeature.Instance.GetItem(featureIds[i]);
				bool flag2 = config != null && !config.Hidden;
				if (flag2)
				{
					result.Add(featureIds[i]);
				}
			}
			result2 = result;
		}
		return result2;
	}
}
