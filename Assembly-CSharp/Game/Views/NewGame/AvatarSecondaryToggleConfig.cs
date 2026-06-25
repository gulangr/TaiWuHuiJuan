using System;

namespace Game.Views.NewGame
{
	// Token: 0x020007DD RID: 2013
	public struct AvatarSecondaryToggleConfig
	{
		// Token: 0x06006235 RID: 25141 RVA: 0x002D115D File Offset: 0x002CF35D
		public AvatarSecondaryToggleConfig(string imageBase, bool interactable = true)
		{
			this.ImageBase = imageBase;
			this.Interactable = interactable;
		}

		// Token: 0x04004465 RID: 17509
		public string ImageBase;

		// Token: 0x04004466 RID: 17510
		public bool Interactable;
	}
}
