using System;
using Config;
using GameData.Domains.Building;
using UnityEngine;

namespace Building
{
	// Token: 0x0200065E RID: 1630
	public class BuildingExpandTaiwuVillageDisk : MonoBehaviour
	{
		// Token: 0x06004D9C RID: 19868 RVA: 0x002498E8 File Offset: 0x00247AE8
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

		// Token: 0x06004D9D RID: 19869 RVA: 0x002499B5 File Offset: 0x00247BB5
		public Transform GetUnlock(sbyte orgTemplateId)
		{
			return this.orgUnlockTags[(int)(orgTemplateId - 1)].transform;
		}

		// Token: 0x06004D9E RID: 19870 RVA: 0x002499C6 File Offset: 0x00247BC6
		public void SetUnlock(sbyte orgTemplateId, bool unlock)
		{
			this.orgUnlockTags[(int)(orgTemplateId - 1)].gameObject.SetActive(unlock);
		}

		// Token: 0x06004D9F RID: 19871 RVA: 0x002499E0 File Offset: 0x00247BE0
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

		// Token: 0x06004DA0 RID: 19872 RVA: 0x00249A3A File Offset: 0x00247C3A
		public void SetCoreAndGrassUnlock(sbyte coreType, bool unlock)
		{
			this.SetCoreUnlock(coreType, unlock);
			this.SetGrassUnlock(coreType, unlock);
		}

		// Token: 0x06004DA1 RID: 19873 RVA: 0x00249A50 File Offset: 0x00247C50
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

		// Token: 0x06004DA2 RID: 19874 RVA: 0x00249AB0 File Offset: 0x00247CB0
		public void SetGrassUnlock(sbyte coreType, bool unlock)
		{
			if (!true)
			{
			}
			GameObject gameObject;
			switch (coreType)
			{
			case -1:
				gameObject = this.evilGrass;
				break;
			case 0:
				gameObject = this.neutralGrass;
				break;
			case 1:
				gameObject = this.goodGrass;
				break;
			default:
				gameObject = null;
				break;
			}
			if (!true)
			{
			}
			GameObject grass = gameObject;
			bool flag = grass != null;
			if (flag)
			{
				grass.SetActive(!unlock);
			}
		}

		// Token: 0x040035CF RID: 13775
		[SerializeField]
		private GameObject[] orgUnlockTags;

		// Token: 0x040035D0 RID: 13776
		[SerializeField]
		private GameObject goodCore;

		// Token: 0x040035D1 RID: 13777
		[SerializeField]
		private GameObject evilCore;

		// Token: 0x040035D2 RID: 13778
		[SerializeField]
		private GameObject neutralCore;

		// Token: 0x040035D3 RID: 13779
		[SerializeField]
		private GameObject goodGrass;

		// Token: 0x040035D4 RID: 13780
		[SerializeField]
		private GameObject evilGrass;

		// Token: 0x040035D5 RID: 13781
		[SerializeField]
		private GameObject neutralGrass;
	}
}
