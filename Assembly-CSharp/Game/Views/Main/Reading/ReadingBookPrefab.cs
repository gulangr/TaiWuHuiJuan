using System;
using System.Collections.Generic;
using Game.Components.Item;
using TMPro;
using UnityEngine;

namespace Game.Views.Main.Reading
{
	// Token: 0x02000966 RID: 2406
	public class ReadingBookPrefab : MonoBehaviour
	{
		// Token: 0x040055BE RID: 21950
		public ItemBack item;

		// Token: 0x040055BF RID: 21951
		public TextMeshProUGUI txtName;

		// Token: 0x040055C0 RID: 21952
		public List<ReadingBookPages> pageList;

		// Token: 0x040055C1 RID: 21953
		public GameObject highlight;

		// Token: 0x040055C2 RID: 21954
		public TextMeshProUGUI durability;

		// Token: 0x040055C3 RID: 21955
		public GameObject completeBg;
	}
}
