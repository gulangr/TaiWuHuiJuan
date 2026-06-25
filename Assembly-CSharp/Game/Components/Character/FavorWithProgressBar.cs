using System;
using System.Runtime.CompilerServices;
using Game.Components.Common;
using GameData.Domains.Character.Display;
using GameData.Domains.Character.Relation;
using UnityEngine;

namespace Game.Components.Character
{
	// Token: 0x02000F23 RID: 3875
	public class FavorWithProgressBar : MonoBehaviour
	{
		// Token: 0x0600B265 RID: 45669 RVA: 0x00512FC4 File Offset: 0x005111C4
		public void Set(CharacterDisplayData characterDisplayData, bool interacted)
		{
			bool show = characterDisplayData != null && characterDisplayData.CharacterId != SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			this.favorProperty.gameObject.SetActive(show);
			bool flag = !show;
			if (!flag)
			{
				string levelName = CommonUtils.GetFavorStringByInteracted(characterDisplayData.FavorabilityToTaiwu, interacted);
				string levelIcon = CommonUtils.GetFavorabilityIconName(characterDisplayData.FavorabilityToTaiwu, interacted);
				string title = LanguageKey.LK_Favorability.Tr();
				this.favorProperty.Set(levelIcon, title, levelName, null, false);
				ValueTuple<short, short> favorabilityRange = FavorabilityType.GetFavorabilityRange(characterDisplayData.FavorabilityToTaiwu);
				short min = favorabilityRange.Item1;
				short max = favorabilityRange.Item2;
				this.favorProgress.fillAmount = (float)(characterDisplayData.FavorabilityToTaiwu - min) / (float)(max - min);
				this.favorProgress.SetSpriteOnly(FavorWithProgressBar.<Set>g__GetFavorFillSpriteNameByFavorType|2_0(FavorabilityType.GetFavorabilityType(characterDisplayData.FavorabilityToTaiwu)), false, null);
			}
		}

		// Token: 0x0600B267 RID: 45671 RVA: 0x005130B0 File Offset: 0x005112B0
		[CompilerGenerated]
		internal static string <Set>g__GetFavorFillSpriteNameByFavorType|2_0(sbyte favorType)
		{
			if (!true)
			{
			}
			string result;
			switch (favorType)
			{
			case -6:
				result = "taiwuevent_01_progress_8";
				break;
			case -5:
				result = "taiwuevent_01_progress_8";
				break;
			case -4:
				result = "taiwuevent_01_progress_8";
				break;
			case -3:
				result = "taiwuevent_01_progress_7";
				break;
			case -2:
				result = "taiwuevent_01_progress_7";
				break;
			case -1:
				result = "taiwuevent_01_progress_7";
				break;
			case 0:
				result = "taiwuevent_01_progress_6";
				break;
			case 1:
				result = "taiwuevent_01_progress_5";
				break;
			case 2:
				result = "taiwuevent_01_progress_4";
				break;
			case 3:
				result = "taiwuevent_01_progress_3";
				break;
			case 4:
				result = "taiwuevent_01_progress_2";
				break;
			case 5:
				result = "taiwuevent_01_progress_1";
				break;
			case 6:
				result = "taiwuevent_01_progress_0";
				break;
			default:
				result = "taiwuevent_01_progress_6";
				break;
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x04008A66 RID: 35430
		[SerializeField]
		private PropertyItem favorProperty;

		// Token: 0x04008A67 RID: 35431
		[SerializeField]
		private CImage favorProgress;
	}
}
