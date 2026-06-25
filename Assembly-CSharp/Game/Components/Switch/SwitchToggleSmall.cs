using System;
using FrameWork.UISystem.UIElements;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Components.Switch
{
	// Token: 0x02000C8C RID: 3212
	public class SwitchToggleSmall : CToggle
	{
		// Token: 0x0600A3B7 RID: 41911 RVA: 0x004C934D File Offset: 0x004C754D
		protected override void Awake()
		{
			base.Awake();
			this.OnClick(base.isOn);
		}

		// Token: 0x0600A3B8 RID: 41912 RVA: 0x004C9364 File Offset: 0x004C7564
		public void SetWithoutNotify(bool on)
		{
			base.SetIsOnWithoutNotify(on);
			CImage cimage = this.backgroundImage;
			if (!on)
			{
				Sprite sprite = this.disableSprite;
				SpriteState spriteState = this.disableSpState;
				cimage.sprite = sprite;
				base.spriteState = spriteState;
			}
			else
			{
				Sprite sprite = this.enableSprite;
				SpriteState spriteState = this.enableSpState;
				cimage.sprite = sprite;
				base.spriteState = spriteState;
			}
		}

		// Token: 0x0600A3B9 RID: 41913 RVA: 0x004C93CC File Offset: 0x004C75CC
		[Obsolete("use SetWithoutNotify to disambiguity")]
		public new void SetIsOnWithoutNotify(bool on)
		{
			this.SetWithoutNotify(on);
		}

		// Token: 0x0600A3BA RID: 41914 RVA: 0x004C93D8 File Offset: 0x004C75D8
		public void OnClick(bool on)
		{
			CImage cimage = this.backgroundImage;
			if (!on)
			{
				Sprite sprite = this.disableSprite;
				SpriteState spriteState = this.disableSpState;
				cimage.sprite = sprite;
				base.spriteState = spriteState;
			}
			else
			{
				Sprite sprite = this.enableSprite;
				SpriteState spriteState = this.enableSpState;
				cimage.sprite = sprite;
				base.spriteState = spriteState;
			}
			this.OnDeselect(null);
		}

		// Token: 0x040081BC RID: 33212
		[SerializeField]
		private CImage backgroundImage;

		// Token: 0x040081BD RID: 33213
		[SerializeField]
		private SpriteState enableSpState;

		// Token: 0x040081BE RID: 33214
		[SerializeField]
		private SpriteState disableSpState;

		// Token: 0x040081BF RID: 33215
		[SerializeField]
		private Sprite enableSprite;

		// Token: 0x040081C0 RID: 33216
		[SerializeField]
		private Sprite disableSprite;
	}
}
