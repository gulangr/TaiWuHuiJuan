using System;
using FrameWork;
using Game.Components.Common;
using GameData.Domains.TaiwuEvent;
using UnityEngine;

namespace Game.Views.GameLineAnim
{
	// Token: 0x02000A21 RID: 2593
	public class ViewGameLineAnim : UIBase
	{
		// Token: 0x06007F39 RID: 32569 RVA: 0x003B45B8 File Offset: 0x003B27B8
		public override void OnInit(ArgumentBox argsBox)
		{
			this.unlockSkillSlotAnimItem.gameObject.SetActive(false);
			this.hotkeyDisplay.gameObject.SetActive(false);
			this.hotkeyDisplay.Refresh(EHotKeyDisplayType.AnyKeyContinue);
			this._canHide = false;
			argsBox.Get("DisplayEventType", out this._displayEventType);
			int displayEventType = this._displayEventType;
			int num = displayEventType;
			if (num == 145)
			{
				this.unlockSkillSlotAnimItem.gameObject.SetActive(true);
				this.unlockSkillSlotAnimItem.Set(argsBox, delegate
				{
					this.ShowHideTips();
				});
			}
		}

		// Token: 0x06007F3A RID: 32570 RVA: 0x003B4651 File Offset: 0x003B2851
		private void ShowHideTips()
		{
			this.hotkeyDisplay.gameObject.SetActive(true);
			this._canHide = true;
		}

		// Token: 0x06007F3B RID: 32571 RVA: 0x003B4670 File Offset: 0x003B2870
		private void Update()
		{
			bool flag = this._canHide && HotKeyCommand.CheckAnyKeyDown();
			if (flag)
			{
				this.QuickHide();
			}
		}

		// Token: 0x06007F3C RID: 32572 RVA: 0x003B469C File Offset: 0x003B289C
		private void OnDisable()
		{
			int displayEventType = this._displayEventType;
			int num = displayEventType;
			if (num == 145)
			{
				TaiwuEventDomainMethod.Call.TriggerListener("ShowUnlockSkillSlotAnimOver", true);
			}
		}

		// Token: 0x04006155 RID: 24917
		[SerializeField]
		private UnlockSkillSlotAnimItem unlockSkillSlotAnimItem;

		// Token: 0x04006156 RID: 24918
		[SerializeField]
		private Game.Components.Common.HotkeyDisplay hotkeyDisplay;

		// Token: 0x04006157 RID: 24919
		private bool _canHide;

		// Token: 0x04006158 RID: 24920
		private int _displayEventType;
	}
}
