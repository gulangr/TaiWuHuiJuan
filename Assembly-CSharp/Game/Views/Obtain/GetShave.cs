using System;
using Game.Components.Avatar;
using GameData.Domains.Character.AvatarSystem;
using TMPro;
using UnityEngine;

namespace Game.Views.Obtain
{
	// Token: 0x020007D8 RID: 2008
	public class GetShave : MonoBehaviour
	{
		// Token: 0x060061EF RID: 25071 RVA: 0x002CEB72 File Offset: 0x002CCD72
		public void Set(string avatarName, AvatarData previous, AvatarData current, short age = 16)
		{
			this.name1.text = avatarName;
			this.name2.text = avatarName;
			this.avatar1.Refresh(previous, age);
			this.avatar2.Refresh(current, age);
		}

		// Token: 0x04004407 RID: 17415
		[SerializeField]
		private TextMeshProUGUI name1;

		// Token: 0x04004408 RID: 17416
		[SerializeField]
		private TextMeshProUGUI name2;

		// Token: 0x04004409 RID: 17417
		[SerializeField]
		private Game.Components.Avatar.Avatar avatar1;

		// Token: 0x0400440A RID: 17418
		[SerializeField]
		private Game.Components.Avatar.Avatar avatar2;
	}
}
