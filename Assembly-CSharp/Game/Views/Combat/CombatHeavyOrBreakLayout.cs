using System;
using Config;
using GameData.Domains.Combat;
using TMPro;

namespace Game.Views.Combat
{
	// Token: 0x02000B2E RID: 2862
	public class CombatHeavyOrBreakLayout : Refers
	{
		// Token: 0x17000F74 RID: 3956
		// (get) Token: 0x06008C3A RID: 35898 RVA: 0x0040C901 File Offset: 0x0040AB01
		private TextMeshProUGUI HeavyOrBreakText
		{
			get
			{
				return base.CGet<TextMeshProUGUI>("HeavyOrBreakText");
			}
		}

		// Token: 0x06008C3B RID: 35899 RVA: 0x0040C910 File Offset: 0x0040AB10
		public void Set(HeavyOrBreakInjuryData data, int bodyPart)
		{
			EHeavyOrBreakType type = (bodyPart >= 0 && bodyPart < 7) ? data[bodyPart] : EHeavyOrBreakType.None;
			this.Set(type, bodyPart);
		}

		// Token: 0x06008C3C RID: 35900 RVA: 0x0040C93C File Offset: 0x0040AB3C
		public void Set(EHeavyOrBreakType type, int bodyPart)
		{
			base.gameObject.SetActive(type > EHeavyOrBreakType.None);
			bool flag = type == this._heavyOrBreakType && bodyPart == this._bodyPart;
			if (!flag)
			{
				this._heavyOrBreakType = type;
				this._bodyPart = bodyPart;
				bool flag2 = type == EHeavyOrBreakType.None;
				if (!flag2)
				{
					if (!true)
					{
					}
					LanguageKey languageKey;
					if (type != EHeavyOrBreakType.Heavy)
					{
						if (type != EHeavyOrBreakType.Break)
						{
							throw new ArgumentException(string.Format("Not support heavy or break type {0}", type));
						}
						languageKey = LanguageKey.LK_Combat_Injury_Break;
					}
					else
					{
						languageKey = LanguageKey.LK_Combat_Injury_Heavy;
					}
					if (!true)
					{
					}
					LanguageKey template = languageKey;
					string icon = string.Format("mousetip_buwei_{0}", DefeatMarkKey.ParseUiIncorrectBodyPart(bodyPart));
					this.HeavyOrBreakText.text = LocalStringManager.GetFormat(template, icon, BodyPart.Instance[bodyPart].Name).ColorReplace();
					this.HeavyOrBreakText.GetComponent<TMPTextSpriteHelper>().Parse();
				}
			}
		}

		// Token: 0x04006B50 RID: 27472
		private EHeavyOrBreakType _heavyOrBreakType;

		// Token: 0x04006B51 RID: 27473
		private int _bodyPart;
	}
}
