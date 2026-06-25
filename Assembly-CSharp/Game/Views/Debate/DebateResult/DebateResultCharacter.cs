using System;
using Game.Components.Avatar;
using Game.Components.Common;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Character.Relation;
using TMPro;
using UnityEngine;

namespace Game.Views.Debate.DebateResult
{
	// Token: 0x02000AA9 RID: 2729
	public class DebateResultCharacter : MonoBehaviour
	{
		// Token: 0x06008624 RID: 34340 RVA: 0x003E6E9C File Offset: 0x003E509C
		public void Refresh(CharacterDisplayData data, bool isFavor, int delta, bool isTaiwu)
		{
			this.avatar.Refresh(data, true);
			string charName = NameCenter.GetMonasticTitleOrDisplayName(data, isTaiwu);
			this.textName.text = charName;
			if (isFavor)
			{
				sbyte favorLevel = FavorabilityType.GetFavorabilityType(data.FavorabilityToTaiwu);
				string icon = CommonUtils.GetFavorabilityIconName((short)favorLevel, true);
				string title = LanguageKey.LK_Favorability.Tr();
				string value = ViewDebateResult.GetValueString(delta, true);
				this.propertyChange.Set(icon, title, value, null, false);
			}
			else
			{
				sbyte happinessLevel = HappinessType.GetHappinessType(data.Happiness);
				string icon2 = CommonUtils.GetHappinessIconName(happinessLevel);
				string title2 = LanguageKey.LK_Main_SummaryInfo_Happiness.Tr();
				string value2 = ViewDebateResult.GetValueString(delta, true);
				this.propertyChange.Set(icon2, title2, value2, null, false);
			}
		}

		// Token: 0x040066F4 RID: 26356
		[SerializeField]
		private Game.Components.Avatar.Avatar avatar;

		// Token: 0x040066F5 RID: 26357
		[SerializeField]
		private TextMeshProUGUI textName;

		// Token: 0x040066F6 RID: 26358
		[SerializeField]
		private PropertyItem propertyChange;
	}
}
