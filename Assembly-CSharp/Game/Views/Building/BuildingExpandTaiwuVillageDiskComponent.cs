using System;
using Config;
using GameData.Domains.Building;
using UnityEngine;

namespace Game.Views.Building
{
	// Token: 0x02000BD5 RID: 3029
	public class BuildingExpandTaiwuVillageDiskComponent : MonoBehaviour
	{
		// Token: 0x0600988B RID: 39051 RVA: 0x00470E84 File Offset: 0x0046F084
		public void Set(BuildingBlockData blockData, BuildingBlockItem config)
		{
			int goodCount = 0;
			int evilCount = 0;
			int neutralCount = 0;
			for (int i = 0; i < (int)config.MaxLevel; i++)
			{
				bool unlocked = blockData.SlotIsUnlocked(i);
				this.orgUnlockTags[i].gameObject.SetActive(unlocked);
				bool flag = !unlocked;
				if (!flag)
				{
					OrganizationItem orgConfig = Organization.Instance[i + 1];
					switch (orgConfig.Goodness)
					{
					case -1:
						evilCount++;
						break;
					case 0:
						neutralCount++;
						break;
					case 1:
						goodCount++;
						break;
					}
				}
			}
			this.SetCoreAndGrassUnlock(1, goodCount >= 5);
			this.SetCoreAndGrassUnlock(-1, evilCount >= 5);
			this.SetCoreAndGrassUnlock(0, neutralCount >= 5);
		}

		// Token: 0x0600988C RID: 39052 RVA: 0x00470F51 File Offset: 0x0046F151
		public Transform GetUnlock(sbyte orgTemplateId)
		{
			return this.orgUnlockTags[(int)(orgTemplateId - 1)].transform;
		}

		// Token: 0x0600988D RID: 39053 RVA: 0x00470F62 File Offset: 0x0046F162
		public void SetUnlock(sbyte orgTemplateId, bool unlock)
		{
			this.orgUnlockTags[(int)(orgTemplateId - 1)].gameObject.SetActive(unlock);
		}

		// Token: 0x0600988E RID: 39054 RVA: 0x00470F7C File Offset: 0x0046F17C
		public Transform GetCoreUnlock(sbyte coreType)
		{
			if (!true)
			{
			}
			Transform result;
			switch (coreType)
			{
			case -1:
				result = this.evilCore.transform;
				break;
			case 0:
				result = this.neutralCore.transform;
				break;
			case 1:
				result = this.goodCore.transform;
				break;
			default:
				result = null;
				break;
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x0600988F RID: 39055 RVA: 0x00470FD6 File Offset: 0x0046F1D6
		public void SetCoreAndGrassUnlock(sbyte coreType, bool unlock)
		{
			this.SetCoreUnlock(coreType, unlock);
		}

		// Token: 0x06009890 RID: 39056 RVA: 0x00470FE4 File Offset: 0x0046F1E4
		public void SetCoreUnlock(sbyte coreType, bool unlock)
		{
			if (!true)
			{
			}
			GameObject gameObject;
			switch (coreType)
			{
			case -1:
				gameObject = this.evilCore;
				break;
			case 0:
				gameObject = this.neutralCore;
				break;
			case 1:
				gameObject = this.goodCore;
				break;
			default:
				gameObject = null;
				break;
			}
			if (!true)
			{
			}
			GameObject core = gameObject;
			bool flag = core != null;
			if (flag)
			{
				core.SetActive(unlock);
			}
		}

		// Token: 0x04007558 RID: 30040
		[SerializeField]
		private GameObject[] orgUnlockTags;

		// Token: 0x04007559 RID: 30041
		[SerializeField]
		private GameObject goodCore;

		// Token: 0x0400755A RID: 30042
		[SerializeField]
		private GameObject evilCore;

		// Token: 0x0400755B RID: 30043
		[SerializeField]
		private GameObject neutralCore;

		// Token: 0x0400755C RID: 30044
		public const int CoreOrganizationCount = 5;
	}
}
