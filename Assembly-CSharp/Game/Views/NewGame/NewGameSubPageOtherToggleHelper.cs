using System;
using TMPro;
using UnityEngine;

namespace Game.Views.NewGame
{
	// Token: 0x0200080E RID: 2062
	public class NewGameSubPageOtherToggleHelper : MonoBehaviour
	{
		// Token: 0x0600653D RID: 25917 RVA: 0x002E4C90 File Offset: 0x002E2E90
		public void Refresh(bool isTutorialOn, bool showQuickStartGame, bool isQuickStartGameOn, bool isInscriptionOn, int selectedInscriptionCharCount)
		{
			this.textTutorial.text = LanguageKey.LK_NewGame_Other_Toggle_Tutorial.TrFormat(this.GetSwitchText(isTutorialOn));
			this.textQuickStartGame.gameObject.SetActive(showQuickStartGame);
			if (showQuickStartGame)
			{
				this.textQuickStartGame.text = LanguageKey.LK_NewGame_Other_Toggle_QuickStartGame.TrFormat(this.GetSwitchText(isQuickStartGameOn));
			}
			this.textInscription.gameObject.SetActive(isInscriptionOn);
			if (isInscriptionOn)
			{
				this.textInscription.text = LanguageKey.LK_NewGame_Other_Toggle_Inscription.TrFormat(selectedInscriptionCharCount);
			}
		}

		// Token: 0x0600653E RID: 25918 RVA: 0x002E4D25 File Offset: 0x002E2F25
		private string GetSwitchText(bool isOn)
		{
			return isOn ? LanguageKey.LK_Option_On.Tr() : LanguageKey.LK_Option_Off.Tr();
		}

		// Token: 0x04004683 RID: 18051
		[SerializeField]
		private TextMeshProUGUI textTutorial;

		// Token: 0x04004684 RID: 18052
		[SerializeField]
		private TextMeshProUGUI textQuickStartGame;

		// Token: 0x04004685 RID: 18053
		[SerializeField]
		private TextMeshProUGUI textInscription;
	}
}
