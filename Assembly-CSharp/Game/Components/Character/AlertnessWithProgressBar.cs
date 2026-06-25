using System;
using FrameWork;
using Game.Components.Common;
using GameData.Domains.Character.Alertness;
using GameData.Domains.Character.Display;
using UnityEngine;

namespace Game.Components.Character
{
	// Token: 0x02000F0C RID: 3852
	public class AlertnessWithProgressBar : MonoBehaviour
	{
		// Token: 0x0600B16D RID: 45421 RVA: 0x0050D348 File Offset: 0x0050B548
		public void Set(CharacterDisplayData characterDisplayData)
		{
			bool show = characterDisplayData != null && characterDisplayData.CharacterId != SingletonObject.getInstance<BasicGameData>().TaiwuCharId && characterDisplayData.CreatingType == 1;
			this.alertnessProperty.gameObject.SetActive(show);
			bool flag = !show;
			if (!flag)
			{
				sbyte level = CharacterAlertnessData.GetLevel(characterDisplayData.Alertness);
				string levelName = CommonUtils.GetAlertnessName((int)level);
				string levelIcon = CommonUtils.GetAlertnessIcon((int)level);
				string title = LanguageKey.LK_Alertness.Tr();
				this.alertnessProperty.Set(levelIcon, title, levelName, null, false);
				this.alertnessProperty.Tip.Type = TipType.Alertness;
				TooltipInvoker tip = this.alertnessProperty.Tip;
				if (tip.RuntimeParam == null)
				{
					tip.RuntimeParam = EasyPool.Get<ArgumentBox>();
				}
				this.alertnessProperty.Tip.RuntimeParam.Clear();
				this.alertnessProperty.Tip.RuntimeParam.Set("charId", characterDisplayData.CharacterId);
				int total = GlobalConfig.Instance.AlertnessMax - GlobalConfig.Instance.AlertnessMin;
				int alertness = characterDisplayData.Alertness;
				int cur = GlobalConfig.Instance.AlertnessMax - alertness;
				this.alertnessProgress.fillAmount = (float)cur / (float)total;
				this.alertnessProgress.SetColor(Colors.Instance.AlertnessColors[(int)level]);
			}
		}

		// Token: 0x0400897E RID: 35198
		[SerializeField]
		private PropertyItem alertnessProperty;

		// Token: 0x0400897F RID: 35199
		[SerializeField]
		private CImage alertnessProgress;
	}
}
