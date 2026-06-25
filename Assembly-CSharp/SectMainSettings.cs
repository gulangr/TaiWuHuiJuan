using System;
using GameData.Domains.Story;
using GameData.Serializer;
using GameData.Utilities;

// Token: 0x020002FD RID: 765
public static class SectMainSettings
{
	// Token: 0x06002CD1 RID: 11473 RVA: 0x00161170 File Offset: 0x0015F370
	public static void GetSectMainStoryIsActive(sbyte orgTemplateId, Action<int> callback = null, IAsyncMethodRequestHandler parent = null)
	{
		bool flag = callback == null;
		if (!flag)
		{
			bool flag2 = GameApp.Instance.GetCurrentGameStateName() != EGameState.InGame;
			if (flag2)
			{
				callback(int.MinValue);
			}
			else
			{
				StoryDomainMethod.AsyncCall.GetSectMainStoryActiveStatus(parent, orgTemplateId, delegate(int offset, RawDataPool pool)
				{
					int ret = 0;
					Serializer.Deserialize(pool, offset, ref ret);
					callback(ret);
				});
			}
		}
	}

	// Token: 0x06002CD2 RID: 11474 RVA: 0x001611D6 File Offset: 0x0015F3D6
	public static void SetSectMainStoryIsActive(sbyte orgTemplateId, bool pause)
	{
		StoryDomainMethod.Call.SetSectMainStoryActiveStatus(orgTemplateId, pause);
	}
}
