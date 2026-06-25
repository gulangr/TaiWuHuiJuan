using System;
using GameData.Domains.Item;
using TMPro;
using UnityEngine;

namespace Game.Views.CharacterMenu
{
	// Token: 0x02000B72 RID: 2930
	public class EatInfoItemWugKingContentTemplate : MonoBehaviour
	{
		// Token: 0x060090E5 RID: 37093 RVA: 0x00438E34 File Offset: 0x00437034
		public void Setup(string title, ItemKey itemKey, int charId)
		{
			this.titleTxt.text = title.ColorReplace();
			this.wugKingEffect.Refresh(itemKey, charId, true);
		}

		// Token: 0x04006F8C RID: 28556
		[SerializeField]
		private TextMeshProUGUI titleTxt;

		// Token: 0x04006F8D RID: 28557
		[SerializeField]
		private TipWugKingEffect wugKingEffect;
	}
}
