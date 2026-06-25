using System;
using GameData.Domains.Building;
using TMPro;
using UnityEngine;

namespace Game.Components.MouseTip
{
	// Token: 0x02000E9B RID: 3739
	public class BuildingManagerTipsInfo : MonoBehaviour
	{
		// Token: 0x0600AD73 RID: 44403 RVA: 0x004F1F68 File Offset: 0x004F0168
		public void SetManagerInfo(BuildingManagerDisplayData managerDisplayData, int index)
		{
			ValueTuple<string, string> name = managerDisplayData.CharacterDisplayData.FullName.GetName(managerDisplayData.CharacterDisplayData.Gender, SingletonObject.getInstance<BasicGameData>().CustomTexts);
			string surname = name.Item1;
			string givenName = name.Item2;
			this.characterName.text = surname + givenName;
			this.teachGrade.SetText(LocalStringManager.Get(string.Format("LK_Grade_{0}", managerDisplayData.LeaderTeachGrade)).SetGradeColor((int)managerDisplayData.LeaderTeachGrade), true);
			this.teachGradeHolder.gameObject.SetActive(index == 0);
			this.matchRole.SetText(managerDisplayData.LeaderRoleMatch ? LanguageKey.LK_Building_Arrangement_TeachMatch.Tr() : LanguageKey.LK_Building_Arrangement_TeachNotMatch.Tr(), true);
			this.potentialLeft.SetText(managerDisplayData.LeftPotentialCount.ToString(), true);
			this.potentialHodler.gameObject.SetActive(index != 0);
			this.title.SetText((index == 0) ? LanguageKey.LK_Building_Teaacher.Tr() : (LanguageKey.LK_Building_Student.Tr() + index.ToString()), true);
		}

		// Token: 0x040085F2 RID: 34290
		[SerializeField]
		public TextMeshProUGUI title;

		// Token: 0x040085F3 RID: 34291
		[SerializeField]
		public TextMeshProUGUI characterName;

		// Token: 0x040085F4 RID: 34292
		[SerializeField]
		public TextMeshProUGUI teachGrade;

		// Token: 0x040085F5 RID: 34293
		[SerializeField]
		public TextMeshProUGUI matchRole;

		// Token: 0x040085F6 RID: 34294
		[SerializeField]
		public TextMeshProUGUI potentialLeft;

		// Token: 0x040085F7 RID: 34295
		[SerializeField]
		public GameObject potentialHodler;

		// Token: 0x040085F8 RID: 34296
		[SerializeField]
		public GameObject teachGradeHolder;
	}
}
