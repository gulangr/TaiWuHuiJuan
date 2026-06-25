using System;
using UnityEngine;

namespace FrameWork.UI.LanguageRule
{
	// Token: 0x02000FED RID: 4077
	public class LanguageRuleActiveOne : MonoBehaviour, ILanguage
	{
		// Token: 0x0600BA16 RID: 47638 RVA: 0x0054C04C File Offset: 0x0054A24C
		private void OnEnable()
		{
			LocalStringManager.LanguageType currentLanguageType = LocalStringManager.CurLanguageType;
			this.OnLanguageChange(currentLanguageType);
		}

		// Token: 0x0600BA17 RID: 47639 RVA: 0x0054C068 File Offset: 0x0054A268
		public void OnLanguageChange(LocalStringManager.LanguageType languageType)
		{
			switch (languageType)
			{
			case LocalStringManager.LanguageType.CN:
				LanguageRuleActiveOne.SetActiveAndDeactivateOthers(this.cnObj, new GameObject[]
				{
					this.enObj,
					this.koObj,
					this.cnhObj
				});
				break;
			case LocalStringManager.LanguageType.EN:
				LanguageRuleActiveOne.SetActiveAndDeactivateOthers(this.enObj, new GameObject[]
				{
					this.cnObj,
					this.koObj,
					this.cnhObj
				});
				break;
			case LocalStringManager.LanguageType.KO:
				LanguageRuleActiveOne.SetActiveAndDeactivateOthers(this.koObj, new GameObject[]
				{
					this.cnObj,
					this.enObj,
					this.cnhObj
				});
				break;
			case LocalStringManager.LanguageType.CNH:
				LanguageRuleActiveOne.SetActiveAndDeactivateOthers(this.cnhObj, new GameObject[]
				{
					this.cnObj,
					this.enObj,
					this.koObj
				});
				break;
			default:
				throw new ArgumentOutOfRangeException("languageType", languageType, null);
			}
		}

		// Token: 0x0600BA18 RID: 47640 RVA: 0x0054C168 File Offset: 0x0054A368
		private static void SetActiveAndDeactivateOthers(GameObject activeObj, params GameObject[] others)
		{
			foreach (GameObject obj in others)
			{
				bool flag = obj != null && obj != activeObj;
				if (flag)
				{
					obj.SetActive(false);
				}
			}
			bool flag2 = activeObj != null;
			if (flag2)
			{
				activeObj.SetActive(true);
			}
		}

		// Token: 0x04008FE1 RID: 36833
		[SerializeField]
		private GameObject cnObj;

		// Token: 0x04008FE2 RID: 36834
		[SerializeField]
		private GameObject enObj;

		// Token: 0x04008FE3 RID: 36835
		[SerializeField]
		private GameObject koObj;

		// Token: 0x04008FE4 RID: 36836
		[SerializeField]
		private GameObject cnhObj;

		// Token: 0x04008FE5 RID: 36837
		[SerializeField]
		private GameObject jpObj;
	}
}
