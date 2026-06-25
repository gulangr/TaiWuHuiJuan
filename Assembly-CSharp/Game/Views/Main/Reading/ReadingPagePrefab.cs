using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Game.Views.Main.Reading
{
	// Token: 0x0200096B RID: 2411
	public class ReadingPagePrefab : MonoBehaviour
	{
		// Token: 0x040055D4 RID: 21972
		public int UserInt;

		// Token: 0x040055D5 RID: 21973
		public CImage pageStateIcon;

		// Token: 0x040055D6 RID: 21974
		public Sprite completePageStateSprite;

		// Token: 0x040055D7 RID: 21975
		public Sprite incompletePageStateSprite;

		// Token: 0x040055D8 RID: 21976
		public Sprite lostPageStateSprite;

		// Token: 0x040055D9 RID: 21977
		public List<ReadingStrategySlot> strategySlotList;

		// Token: 0x040055DA RID: 21978
		public List<ReadingBriefStrategy> strategyBriefList;

		// Token: 0x040055DB RID: 21979
		public ReadingPageProgressBar progressBar;

		// Token: 0x040055DC RID: 21980
		public TextMeshProUGUI pageStateText;

		// Token: 0x040055DD RID: 21981
		public TextMeshProUGUI progressText;

		// Token: 0x040055DE RID: 21982
		public GameObject highlight;

		// Token: 0x040055DF RID: 21983
		public GameObject eventHighlight1;

		// Token: 0x040055E0 RID: 21984
		public GameObject eventHighlight2;

		// Token: 0x040055E1 RID: 21985
		public List<TextMeshProUGUI> numPageList;

		// Token: 0x040055E2 RID: 21986
		public List<TextMeshProUGUI> numPageTypeList;

		// Token: 0x040055E3 RID: 21987
		public List<GameObject> combatSkillBookNodes;
	}
}
