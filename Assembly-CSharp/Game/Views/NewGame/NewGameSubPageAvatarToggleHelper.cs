using System;
using Game.Components.Avatar;
using GameData.Domains.Character.AvatarSystem;
using TMPro;
using UnityEngine;

namespace Game.Views.NewGame
{
	// Token: 0x020007FD RID: 2045
	public class NewGameSubPageAvatarToggleHelper : MonoBehaviour
	{
		// Token: 0x060063D3 RID: 25555 RVA: 0x002DC1F4 File Offset: 0x002DA3F4
		public void Refresh(AvatarData data, short age, sbyte gender)
		{
			bool flag = data == null;
			if (!flag)
			{
				bool flag2 = this.avatar != null;
				if (flag2)
				{
					this.avatar.Refresh(data, age);
				}
				bool flag3 = this.genderIcon != null;
				if (flag3)
				{
					string spriteName = (gender == 0) ? "ui9_btn_create_gender_female_0" : "ui9_btn_create_gender_male_0";
					this.genderIcon.SetSprite(spriteName, false, null);
				}
				bool flag4 = this.ageText != null;
				if (flag4)
				{
					this.ageText.text = LocalStringManager.GetFormat(LanguageKey.LK_Age, age);
				}
			}
		}

		// Token: 0x040045AF RID: 17839
		[SerializeField]
		private Game.Components.Avatar.Avatar avatar;

		// Token: 0x040045B0 RID: 17840
		[SerializeField]
		private CImage genderIcon;

		// Token: 0x040045B1 RID: 17841
		[SerializeField]
		private TextMeshProUGUI ageText;
	}
}
