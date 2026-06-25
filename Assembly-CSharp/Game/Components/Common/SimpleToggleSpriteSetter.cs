using System;
using FrameWork.UISystem.UIElements;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Components.Common
{
	// Token: 0x02000FA4 RID: 4004
	[RequireComponent(typeof(CToggle))]
	public class SimpleToggleSpriteSetter : MonoBehaviour
	{
		// Token: 0x0600B7CB RID: 47051 RVA: 0x0053C72C File Offset: 0x0053A92C
		public void SetSprite(string spritePrefix)
		{
			this.checkMark.SetSprite(spritePrefix + this.suffixes[4], false, null);
			SpriteState state = this.toggle.spriteState;
			this.baseImg.SetSprite(spritePrefix + this.suffixes[3], false, null);
			state.disabledSprite = this.baseImg.sprite;
			this.baseImg.SetSprite(spritePrefix + this.suffixes[2], false, null);
			state.pressedSprite = this.baseImg.sprite;
			this.baseImg.SetSprite(spritePrefix + this.suffixes[1], false, null);
			state.highlightedSprite = this.baseImg.sprite;
			this.baseImg.SetSprite(spritePrefix + this.suffixes[0], false, null);
			state.selectedSprite = this.baseImg.sprite;
			this.toggle.spriteState = state;
		}

		// Token: 0x04008ED8 RID: 36568
		[SerializeField]
		internal CToggle toggle;

		// Token: 0x04008ED9 RID: 36569
		[SerializeField]
		private CImage baseImg;

		// Token: 0x04008EDA RID: 36570
		[SerializeField]
		private CImage checkMark;

		// Token: 0x04008EDB RID: 36571
		[SerializeField]
		private string[] suffixes = new string[]
		{
			"_0",
			"_1",
			"_2",
			"_3",
			"_4"
		};
	}
}
