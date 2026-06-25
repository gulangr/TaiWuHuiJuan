using System;
using GameData.Domains.Character.AvatarSystem;
using UnityEngine;

namespace Game.Views.NewGame
{
	// Token: 0x020007F8 RID: 2040
	public abstract class NewGameSubPageAvatarPageBase : MonoBehaviour
	{
		// Token: 0x060063AA RID: 25514 RVA: 0x002DB039 File Offset: 0x002D9239
		public virtual void Init(IAvatarSubPageParent avatarPage)
		{
			this.AvatarPage = avatarPage;
			this.IsInitialized = true;
		}

		// Token: 0x060063AB RID: 25515 RVA: 0x002DB04C File Offset: 0x002D924C
		protected virtual void OnEnable()
		{
			bool isInitialized = this.IsInitialized;
			if (isInitialized)
			{
				this.UpdateUI();
			}
		}

		// Token: 0x060063AC RID: 25516
		public abstract void UpdateUI();

		// Token: 0x060063AD RID: 25517 RVA: 0x002DB070 File Offset: 0x002D9270
		protected AvatarData GetAvatarData()
		{
			IAvatarSubPageParent avatarPage = this.AvatarPage;
			return (avatarPage != null) ? avatarPage.GetAvatarData() : null;
		}

		// Token: 0x060063AE RID: 25518 RVA: 0x002DB094 File Offset: 0x002D9294
		protected void RefreshAvatarAndMarkDirty()
		{
			IAvatarSubPageParent avatarPage = this.AvatarPage;
			if (avatarPage != null)
			{
				avatarPage.RefreshAvatar();
			}
			IAvatarSubPageParent avatarPage2 = this.AvatarPage;
			if (avatarPage2 != null)
			{
				avatarPage2.MarkAvatarDirty();
			}
		}

		// Token: 0x04004586 RID: 17798
		protected IAvatarSubPageParent AvatarPage;

		// Token: 0x04004587 RID: 17799
		protected bool IsInitialized;
	}
}
