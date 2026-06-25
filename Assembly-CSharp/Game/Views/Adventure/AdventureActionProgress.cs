using System;
using GameData.Adventure;
using TMPro;
using UnityEngine;

namespace Game.Views.Adventure
{
	// Token: 0x02000C60 RID: 3168
	public class AdventureActionProgress : MonoBehaviour
	{
		// Token: 0x0600A179 RID: 41337 RVA: 0x004B75A0 File Offset: 0x004B57A0
		public void SetValue(AdventureActionData actionData, int remainTime)
		{
			this.paramProgressIcon.SetSpriteOnly(ViewAdventureRemake.GetElementParamStateIconName(actionData.Icon, false), false, null);
			this.paramProgress.fillAmount = AdventureActionProgress.GetActionProgress(remainTime, actionData.Time);
			this.paramName.text = actionData.Name;
		}

		// Token: 0x0600A17A RID: 41338 RVA: 0x004B75F4 File Offset: 0x004B57F4
		public static float GetActionProgress(int remainTime, int totalTime)
		{
			return 1f - (float)remainTime / (float)totalTime;
		}

		// Token: 0x04007D3E RID: 32062
		[SerializeField]
		private TextMeshProUGUI paramName;

		// Token: 0x04007D3F RID: 32063
		[SerializeField]
		private CImage paramProgress;

		// Token: 0x04007D40 RID: 32064
		[SerializeField]
		private CImage paramProgressIcon;
	}
}
