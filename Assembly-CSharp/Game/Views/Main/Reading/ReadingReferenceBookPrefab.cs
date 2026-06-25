using System;
using System.Collections.Generic;
using Game.Components.Item;
using TMPro;
using UnityEngine;

namespace Game.Views.Main.Reading
{
	// Token: 0x02000970 RID: 2416
	public class ReadingReferenceBookPrefab : MonoBehaviour
	{
		// Token: 0x04005608 RID: 22024
		public ItemBack item;

		// Token: 0x04005609 RID: 22025
		public TextMeshProUGUI txtName;

		// Token: 0x0400560A RID: 22026
		public List<ReadingBookPages> pageList;

		// Token: 0x0400560B RID: 22027
		public GameObject highlight;

		// Token: 0x0400560C RID: 22028
		public TextMeshProUGUI durability;

		// Token: 0x0400560D RID: 22029
		public GameObject completeBg;

		// Token: 0x0400560E RID: 22030
		public TextMeshProUGUI efficiency;

		// Token: 0x0400560F RID: 22031
		public TextMeshProUGUI inspiration;

		// Token: 0x04005610 RID: 22032
		public Transform strategiesRoot;
	}
}
