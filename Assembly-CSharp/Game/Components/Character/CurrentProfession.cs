using System;
using Config;
using GameData.Domains.Taiwu.Profession;
using TMPro;
using UnityEngine;

namespace Game.Components.Character
{
	// Token: 0x02000F20 RID: 3872
	public class CurrentProfession : MonoBehaviour
	{
		// Token: 0x0600B258 RID: 45656 RVA: 0x00512B80 File Offset: 0x00510D80
		public void Set(ProfessionData data, bool forceShow = false)
		{
			bool flag = (data == null || data.TemplateId < 0) && !forceShow;
			if (flag)
			{
				this.SetEmpty();
			}
			else
			{
				base.gameObject.SetActive(true);
				bool flag2 = data == null || data.TemplateId < 0;
				if (flag2)
				{
					this.txtValue.text = LanguageKey.LK_Tooltip_CharacterCurrentProfession_Empty.Tr();
				}
				else
				{
					ProfessionItem config = Profession.Instance[data.TemplateId];
					this.txtValue.text = config.Name;
				}
			}
		}

		// Token: 0x0600B259 RID: 45657 RVA: 0x00512C0E File Offset: 0x00510E0E
		public void SetEmpty()
		{
			base.gameObject.SetActive(false);
		}

		// Token: 0x04008A5E RID: 35422
		[SerializeField]
		private TextMeshProUGUI txtValue;
	}
}
