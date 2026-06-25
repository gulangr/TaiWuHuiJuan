using System;
using DisplayConfig;
using TMPro;
using UnityEngine;

namespace Game.Components.Character
{
	// Token: 0x02000F3F RID: 3903
	public class Personality : MonoBehaviour
	{
		// Token: 0x0600B352 RID: 45906 RVA: 0x0051A30E File Offset: 0x0051850E
		public void Set(sbyte personalityType, int value)
		{
			this.bg.SetSprite("ui9_icon_personality_big_" + personalityType.ToString(), false, null);
			this.label.text = value.ToString();
			this.RefreshTips(personalityType);
		}

		// Token: 0x0600B353 RID: 45907 RVA: 0x0051A34C File Offset: 0x0051854C
		private void RefreshTips(sbyte personalityType)
		{
			TooltipInvoker tip = this.bg.GetComponent<TooltipInvoker>();
			tip.IsLanguageKey = false;
			tip.Type = TipType.Simple;
			PersonalityItem config = Personality.Instance[(int)personalityType];
			tip.PresetParam = new string[]
			{
				config.Name,
				config.Desc
			};
		}

		// Token: 0x04008B47 RID: 35655
		[SerializeField]
		protected CImage bg;

		// Token: 0x04008B48 RID: 35656
		[SerializeField]
		protected TextMeshProUGUI label;
	}
}
