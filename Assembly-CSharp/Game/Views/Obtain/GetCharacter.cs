using System;
using FrameWork;
using Game.Components.Avatar;
using GameData.Domains.Character.Display;
using TMPro;
using UnityEngine;

namespace Game.Views.Obtain
{
	// Token: 0x020007D3 RID: 2003
	public class GetCharacter : MonoBehaviour
	{
		// Token: 0x060061D1 RID: 25041 RVA: 0x002CE1D8 File Offset: 0x002CC3D8
		public void Set(CharacterDisplayData data)
		{
			this.characterName.text = NameCenter.GetMonasticTitleOrDisplayName(data, false);
			this.avatar.Refresh(data, true);
			this.tips.NeedRefresh = true;
			this.tips.RuntimeParam = new ArgumentBox().Set("CharId", data.CharacterId);
		}

		// Token: 0x040043E6 RID: 17382
		[SerializeField]
		private TextMeshProUGUI characterName;

		// Token: 0x040043E7 RID: 17383
		[SerializeField]
		private Game.Components.Avatar.Avatar avatar;

		// Token: 0x040043E8 RID: 17384
		[SerializeField]
		private TooltipInvoker tips;
	}
}
