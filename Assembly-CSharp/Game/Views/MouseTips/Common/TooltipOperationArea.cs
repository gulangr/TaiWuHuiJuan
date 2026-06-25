using System;
using Game.Components.Common;
using Game.Views.CharacterMenu;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Taiwu;
using UnityEngine;

namespace Game.Views.MouseTips.Common
{
	// Token: 0x020008B6 RID: 2230
	public class TooltipOperationArea : MonoBehaviour
	{
		// Token: 0x17000C98 RID: 3224
		// (get) Token: 0x06006A87 RID: 27271 RVA: 0x00312D86 File Offset: 0x00310F86
		public bool IsShowingHotkeyDisplayViewEncyclopedia
		{
			get
			{
				return this.hotkeyDisplayViewEncyclopedia.gameObject.activeSelf;
			}
		}

		// Token: 0x06006A88 RID: 27272 RVA: 0x00312D98 File Offset: 0x00310F98
		private void RefreshRoot()
		{
			bool isShow = this.hotkeyDisplayDetail.gameObject.activeSelf || this.hotkeyDisplayLockItem.gameObject.activeSelf || this.hotkeyDisplayViewEncyclopedia.gameObject.activeSelf;
			base.gameObject.SetActive(isShow);
		}

		// Token: 0x06006A89 RID: 27273 RVA: 0x00312DEB File Offset: 0x00310FEB
		public void ShowHotkeyDisplayDetail(bool isShow)
		{
			this.hotkeyDisplayDetail.gameObject.SetActive(isShow);
			this.RefreshRoot();
		}

		// Token: 0x06006A8A RID: 27274 RVA: 0x00312E07 File Offset: 0x00311007
		public void RefreshPressToDetail()
		{
			this.hotkeyDisplayDetail.Refresh(EHotKeyDisplayType.Detail);
		}

		// Token: 0x06006A8B RID: 27275 RVA: 0x00312E18 File Offset: 0x00311018
		public void RefreshCancelDetail()
		{
			this.hotkeyDisplayDetail.Refresh(EHotKeyDisplayType.CancelDetail);
		}

		// Token: 0x06006A8C RID: 27276 RVA: 0x00312E2C File Offset: 0x0031102C
		public void RefreshHotkeyDisplayLockItem(ItemDisplayData itemData)
		{
			bool flag = !this.hotkeyDisplayLockItem;
			if (!flag)
			{
				int taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
				bool flag2;
				if (itemData != null && itemData.OwnerCharId == taiwuCharId)
				{
					ItemSourceType itemSourceTypeEnum = itemData.ItemSourceTypeEnum;
					if ((itemSourceTypeEnum == ItemSourceType.Inventory || itemSourceTypeEnum == ItemSourceType.Equipment) && ViewCharacterMenuItems.CurrItemOperation == ItemOperationType.EItemOperationType.Invalid && ItemTemplateHelper.IsTransferable(itemData.RealKey.ItemType, itemData.RealKey.TemplateId) && UIManager.Instance.IsFocusElement(UIElement.CharacterMenu) && ViewCharacterMenu.CurSubToggleIndex == ECharacterSubToggleBase.ItemBase)
					{
						flag2 = !UIElement.CharacterMenu.UiBaseAs<ViewCharacterMenu>().StackView.isActiveAndEnabled;
						goto IL_95;
					}
				}
				flag2 = false;
				IL_95:
				bool isShow = flag2;
				this.hotkeyDisplayLockItem.gameObject.SetActive(isShow);
				bool flag3 = isShow;
				if (flag3)
				{
					this.hotkeyDisplayLockItem.Refresh(itemData.IsLocked ? EHotKeyDisplayType.UnlockItem : EHotKeyDisplayType.LockItem);
				}
				this.RefreshRoot();
			}
		}

		// Token: 0x06006A8D RID: 27277 RVA: 0x00312F09 File Offset: 0x00311109
		public void SetLockItemActive(bool isActive)
		{
			this.hotkeyDisplayLockItem.gameObject.SetActive(false);
		}

		// Token: 0x06006A8E RID: 27278 RVA: 0x00312F20 File Offset: 0x00311120
		public void RefreshHotkeyDisplayViewEncyclopedia(bool isShow)
		{
			bool isOnViewEncyclopedia = UIManager.Instance.IsFocusElement(UIElement.Encyclopedia);
			this.hotkeyDisplayViewEncyclopedia.gameObject.SetActive(!isOnViewEncyclopedia && isShow);
			this.hotkeyDisplayViewEncyclopedia.Refresh(EHotKeyDisplayType.ViewEncyclopedia);
			this.RefreshRoot();
		}

		// Token: 0x04004CF2 RID: 19698
		[SerializeField]
		private Game.Components.Common.HotkeyDisplay hotkeyDisplayDetail;

		// Token: 0x04004CF3 RID: 19699
		[SerializeField]
		private Game.Components.Common.HotkeyDisplay hotkeyDisplayLockItem;

		// Token: 0x04004CF4 RID: 19700
		[SerializeField]
		private Game.Components.Common.HotkeyDisplay hotkeyDisplayViewEncyclopedia;
	}
}
