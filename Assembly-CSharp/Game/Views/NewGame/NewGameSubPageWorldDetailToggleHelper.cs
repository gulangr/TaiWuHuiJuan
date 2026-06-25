using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Game.Views.NewGame
{
	// Token: 0x02000817 RID: 2071
	public class NewGameSubPageWorldDetailToggleHelper : MonoBehaviour
	{
		// Token: 0x060065A5 RID: 26021 RVA: 0x002E6F88 File Offset: 0x002E5188
		public void Refresh(List<string> titleList)
		{
			for (int index = 0; index < titleList.Count; index++)
			{
				this.textGroupList[index].text = titleList[index];
			}
		}

		// Token: 0x040046E0 RID: 18144
		[SerializeField]
		private List<TextMeshProUGUI> textGroupList;
	}
}
