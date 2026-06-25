using System;
using UnityEngine;

namespace Game.Views.Main.Reading
{
	// Token: 0x0200096E RID: 2414
	public class ReadingPageSupplyState : MonoBehaviour
	{
		// Token: 0x06007387 RID: 29575 RVA: 0x0035A798 File Offset: 0x00358998
		public void SetSupplySprite(sbyte pageState)
		{
			string languageKey = SingletonObject.getInstance<GlobalSettings>().Language.ToLower();
			string frameSp = (pageState == 0) ? "ui9_icon_reading_page_complete_finish" : "ui9_icon_reading_page_complete_lost";
			string iconSp = (pageState == 0) ? ("ui9_text_reading_page_complete_finish_" + languageKey) : ("ui9_text_reading_page_supplement_damaged_" + languageKey);
			this.frame.SetSprite(frameSp, false, null);
			this.icon.SetSprite(iconSp, false, null);
		}

		// Token: 0x040055EC RID: 21996
		public CImage frame;

		// Token: 0x040055ED RID: 21997
		public CImage icon;
	}
}
