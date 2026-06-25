using System;
using Game.Components.Avatar;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using TMPro;
using UnityEngine;

namespace Game.Views.Cricket
{
	// Token: 0x02000AB2 RID: 2738
	public class CricketCombatResultInfoItem : MonoBehaviour
	{
		// Token: 0x06008665 RID: 34405 RVA: 0x003E8CA8 File Offset: 0x003E6EA8
		public void Set(CharacterDisplayData data, bool isTaiwu, CricketCombatResultMetricType metricType, int delta)
		{
			this.avatar.Refresh(data, true);
			this.nameText.text = NameCenter.GetMonasticTitleOrDisplayName(data, isTaiwu);
			if (metricType != CricketCombatResultMetricType.Happiness)
			{
				if (metricType == CricketCombatResultMetricType.Favorability)
				{
					this.stateIcon.SetSprite(CommonUtils.GetFavorabilityIconName(data.FavorabilityToTaiwu, true), false, null);
					this.descText.text = LanguageKey.LK_Favorability.Tr();
				}
			}
			else
			{
				sbyte happinessLevel = HappinessType.GetHappinessType(data.Happiness);
				this.stateIcon.SetSprite(CommonUtils.GetHappinessIconName(happinessLevel), false, null);
				this.descText.text = LanguageKey.LK_Main_SummaryInfo_Happiness.Tr();
			}
			this.deltaText.text = CricketCombatResultInfoItem.GetDeltaText(delta);
		}

		// Token: 0x06008666 RID: 34406 RVA: 0x003E8D64 File Offset: 0x003E6F64
		private static string GetDeltaText(int delta)
		{
			bool flag = delta == 0;
			string result;
			if (flag)
			{
				result = "0";
			}
			else
			{
				string value = CommonUtils.GetDisplayStringForNum(Math.Abs(delta), 100000);
				result = ((delta > 0) ? ("+" + value).SetColor("brightblue") : ("-" + value).SetColor("brightred"));
			}
			return result;
		}

		// Token: 0x04006739 RID: 26425
		[SerializeField]
		private Game.Components.Avatar.Avatar avatar;

		// Token: 0x0400673A RID: 26426
		[SerializeField]
		private TextMeshProUGUI nameText;

		// Token: 0x0400673B RID: 26427
		[SerializeField]
		private CImage stateIcon;

		// Token: 0x0400673C RID: 26428
		[SerializeField]
		private TextMeshProUGUI descText;

		// Token: 0x0400673D RID: 26429
		[SerializeField]
		private TextMeshProUGUI deltaText;
	}
}
