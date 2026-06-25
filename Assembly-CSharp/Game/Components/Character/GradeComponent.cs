using System;
using FrameWork.UI.LanguageRule;
using TMPro;
using UnityEngine;

namespace Game.Components.Character
{
	// Token: 0x02000F27 RID: 3879
	public class GradeComponent : MonoBehaviour
	{
		// Token: 0x0600B286 RID: 45702 RVA: 0x00513EA0 File Offset: 0x005120A0
		public void OnLanguageChange(LocalStringManager.LanguageType curLanguageType)
		{
			this.gradeTextHelper.OnLanguageChange(curLanguageType);
		}

		// Token: 0x0600B287 RID: 45703 RVA: 0x00513EB0 File Offset: 0x005120B0
		public void Set(string text, int grade = -1)
		{
			this.gradeText.text = text;
			bool flag = this.grades.CheckIndex(grade);
			if (flag)
			{
				foreach (GameObject icon in this.gradeIcons)
				{
					icon.SetActive(true);
				}
				this.gradeIcon.sprite = this.grades[grade];
			}
			else
			{
				foreach (GameObject icon2 in this.gradeIcons)
				{
					icon2.SetActive(false);
				}
			}
		}

		// Token: 0x04008A87 RID: 35463
		[SerializeField]
		private Sprite[] grades;

		// Token: 0x04008A88 RID: 35464
		[SerializeField]
		internal TMP_Text gradeText;

		// Token: 0x04008A89 RID: 35465
		[SerializeField]
		private LanguageRuleTips gradeTextHelper;

		// Token: 0x04008A8A RID: 35466
		[SerializeField]
		private CImage gradeIcon;

		// Token: 0x04008A8B RID: 35467
		[SerializeField]
		private GameObject[] gradeIcons;
	}
}
