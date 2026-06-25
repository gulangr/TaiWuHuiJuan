using System;

namespace DisplayConfig
{
	// Token: 0x020006E0 RID: 1760
	public class Personality
	{
		// Token: 0x17000A55 RID: 2645
		public PersonalityItem this[int index]
		{
			get
			{
				return this._dataArray[index];
			}
		}

		// Token: 0x0400388A RID: 14474
		public static Personality Instance = new Personality();

		// Token: 0x0400388B RID: 14475
		private readonly PersonalityItem[] _dataArray = new PersonalityItem[]
		{
			new PersonalityItem(0, LanguageKey.LK_Personality_Calm_Name, LanguageKey.LK_Personality_Calm_Desc, "ui_sp_icon_personality_0"),
			new PersonalityItem(1, LanguageKey.LK_Personality_Clever_Name, LanguageKey.LK_Personality_Clever_Desc, "ui_sp_icon_personality_1"),
			new PersonalityItem(2, LanguageKey.LK_Personality_Enthusiastic_Name, LanguageKey.LK_Personality_Enthusiastic_Desc, "ui_sp_icon_personality_2"),
			new PersonalityItem(3, LanguageKey.LK_Personality_Brave_Name, LanguageKey.LK_Personality_Brave_Desc, "ui_sp_icon_personality_3"),
			new PersonalityItem(4, LanguageKey.LK_Personality_Firm_Name, LanguageKey.LK_Personality_Firm_Desc, "ui_sp_icon_personality_4"),
			new PersonalityItem(5, LanguageKey.LK_Personality_Lucky_Name, LanguageKey.LK_Personality_Lucky_Desc, "ui_sp_icon_personality_5"),
			new PersonalityItem(6, LanguageKey.LK_Personality_Perceptive_Name, LanguageKey.LK_Personality_Perceptive_Desc, "ui_sp_icon_personality_6")
		};
	}
}
