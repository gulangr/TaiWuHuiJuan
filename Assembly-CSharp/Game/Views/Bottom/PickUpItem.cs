using System;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Item;
using GameData.Domains.Map;
using TMPro;
using UnityEngine;

namespace Game.Views.Bottom
{
	// Token: 0x02000C41 RID: 3137
	public class PickUpItem : MonoBehaviour
	{
		// Token: 0x06009F4D RID: 40781 RVA: 0x004A719C File Offset: 0x004A539C
		private void Awake()
		{
			this.pointerTrigger.EnterEvent.RemoveAllListeners();
			this.pointerTrigger.EnterEvent.AddListener(delegate()
			{
				this.SetHighlight(true);
			});
			this.pointerTrigger.ExitEvent.RemoveAllListeners();
			this.pointerTrigger.ExitEvent.AddListener(delegate()
			{
				this.SetHighlight(false);
			});
		}

		// Token: 0x06009F4E RID: 40782 RVA: 0x004A7206 File Offset: 0x004A5406
		public void SetHighlight(bool isHighlight)
		{
			this.hover.gameObject.SetActive(isHighlight && this.button.interactable);
		}

		// Token: 0x06009F4F RID: 40783 RVA: 0x004A722C File Offset: 0x004A542C
		public void Set(string iconName, string name, string amount, bool hasBattle, bool autoClearBattle, uint banReason, MapPickup.EMapPickupType pickupType, Action onClick)
		{
			this.itemBack.SetIcon(iconName);
			this.nameText.text = name;
			this.amountText.text = amount;
			bool flag = this.battleSign;
			if (flag)
			{
				this.battleSign.gameObject.SetActive(hasBattle);
				if (hasBattle)
				{
					this.battleSign.SetSprite(autoClearBattle ? "ui9_icon_map_pick_up_combat_0" : "ui9_icon_map_pick_up_combat_1", false, null);
				}
			}
			bool enabledByReadAndLoop = banReason == 0U;
			this.button.interactable = enabledByReadAndLoop;
			bool flag2 = this.styleRoot;
			if (flag2)
			{
				this.styleRoot.SetInteractable(enabledByReadAndLoop);
			}
			this.mouseTipDisplayer.enabled = !enabledByReadAndLoop;
			TooltipInvoker tooltipInvoker = this.mouseTipDisplayer;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = new ArgumentBox();
			}
			bool flag3 = !enabledByReadAndLoop;
			if (flag3)
			{
				LanguageKey key = (pickupType == MapPickup.EMapPickupType.LoopEffect) ? LanguageKey.LK_Taiwu_No_LoopingNeigong : LanguageKey.LK_Taiwu_No_ReadingBook;
				this.mouseTipDisplayer.RuntimeParam.Set("arg0", LocalStringManager.Get(key));
			}
			this.button.ClearAndAddListener(onClick);
		}

		// Token: 0x04007B31 RID: 31537
		[SerializeField]
		private ItemBack itemBack;

		// Token: 0x04007B32 RID: 31538
		[SerializeField]
		private TextMeshProUGUI nameText;

		// Token: 0x04007B33 RID: 31539
		[SerializeField]
		private TextMeshProUGUI amountText;

		// Token: 0x04007B34 RID: 31540
		[SerializeField]
		private CImage battleSign;

		// Token: 0x04007B35 RID: 31541
		[SerializeField]
		private CButton button;

		// Token: 0x04007B36 RID: 31542
		[SerializeField]
		private CImage hover;

		// Token: 0x04007B37 RID: 31543
		[SerializeField]
		private PointerTrigger pointerTrigger;

		// Token: 0x04007B38 RID: 31544
		[SerializeField]
		private TooltipInvoker mouseTipDisplayer;

		// Token: 0x04007B39 RID: 31545
		[SerializeField]
		private HSVStyleRoot styleRoot;
	}
}
