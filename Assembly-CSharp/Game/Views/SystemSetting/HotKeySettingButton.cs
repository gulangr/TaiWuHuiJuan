using System;
using FrameWork.UISystem.UIElements;
using UnityEngine;

namespace Game.Views.SystemSetting
{
	// Token: 0x02000761 RID: 1889
	public class HotKeySettingButton : MonoBehaviour
	{
		// Token: 0x06005B64 RID: 23396 RVA: 0x002A7188 File Offset: 0x002A5388
		private void Awake()
		{
			this.pointerTrigger.EnterEvent.RemoveAllListeners();
			this.pointerTrigger.EnterEvent.AddListener(delegate()
			{
				this.hover.gameObject.SetActive(this.hotKeyButton.interactable);
			});
			this.pointerTrigger.ExitEvent.RemoveAllListeners();
			this.pointerTrigger.ExitEvent.AddListener(delegate()
			{
				this.hover.gameObject.SetActive(false);
			});
		}

		// Token: 0x06005B65 RID: 23397 RVA: 0x002A71F4 File Offset: 0x002A53F4
		public void Set(HotKeyCommand command, bool isMouseKey, bool isInteractable, Action action, bool isConflict)
		{
			this.hotKeyDisplayItem.Set(command, isMouseKey, false);
			this.hotKeyButton.interactable = isInteractable;
			this.hotKeyButton.ClearAndAddListener(action);
			this.conflictIcon.gameObject.SetActive(isConflict);
			this.bg.sprite = this.sprites[(!isConflict) ? 0 : 1];
			if (isInteractable)
			{
				this.hSVStyleRoot.SetDefault();
			}
			else
			{
				this.hSVStyleRoot.SetDefaultBlack();
			}
			this.canvasGroup.alpha = (isInteractable ? 1f : 0.5f);
		}

		// Token: 0x06005B66 RID: 23398 RVA: 0x002A7298 File Offset: 0x002A5498
		public void SetInteractable(bool interactable)
		{
			this.hotKeyButton.interactable = interactable;
			if (interactable)
			{
				this.hSVStyleRoot.SetDefault();
			}
			else
			{
				this.hSVStyleRoot.SetDefaultGrayAndBlack();
			}
			this.canvasGroup.alpha = (interactable ? 1f : 0.5f);
		}

		// Token: 0x04003F07 RID: 16135
		[SerializeField]
		private CButton hotKeyButton;

		// Token: 0x04003F08 RID: 16136
		[SerializeField]
		private HotKeyDisplayItem hotKeyDisplayItem;

		// Token: 0x04003F09 RID: 16137
		[SerializeField]
		private HSVStyleRoot hSVStyleRoot;

		// Token: 0x04003F0A RID: 16138
		[SerializeField]
		private PointerTrigger pointerTrigger;

		// Token: 0x04003F0B RID: 16139
		[SerializeField]
		private CanvasGroup canvasGroup;

		// Token: 0x04003F0C RID: 16140
		[SerializeField]
		private CImage hover;

		// Token: 0x04003F0D RID: 16141
		[SerializeField]
		private CImage bg;

		// Token: 0x04003F0E RID: 16142
		[SerializeField]
		private CImage conflictIcon;

		// Token: 0x04003F0F RID: 16143
		[SerializeField]
		private Sprite[] sprites;
	}
}
