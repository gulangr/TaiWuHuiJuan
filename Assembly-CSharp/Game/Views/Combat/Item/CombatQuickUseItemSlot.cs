using System;
using FrameWork.UISystem.UIElements;
using Game.Components.Item;
using Game.Components.ListStyleGeneralScroll.Item;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.Combat.Item
{
	// Token: 0x02000B3C RID: 2876
	public class CombatQuickUseItemSlot : MonoBehaviour
	{
		// Token: 0x17000FA0 RID: 4000
		// (get) Token: 0x06008EFD RID: 36605 RVA: 0x00429E86 File Offset: 0x00428086
		// (set) Token: 0x06008EFE RID: 36606 RVA: 0x00429E8E File Offset: 0x0042808E
		public ItemKey ItemKey { get; private set; }

		// Token: 0x06008EFF RID: 36607 RVA: 0x00429E98 File Offset: 0x00428098
		public void Refresh(int index, bool isShow, bool interactable, ItemKey itemKey, ItemDisplayData itemData, string wisdomIcon, string wisdomColor, Action<int, ItemDisplayData> onClickButtonUse, Action onClickButtonSetting, Action<short> onEnter = null, Action onExit = null)
		{
			base.gameObject.SetActive(isShow);
			bool flag = !isShow;
			if (!flag)
			{
				this.ItemKey = itemKey;
				bool hasTemplate = itemKey.HasTemplate;
				this.rootValid.gameObject.SetActive(hasTemplate);
				this.buttonAdd.gameObject.SetActive(!hasTemplate);
				this.tip.enabled = hasTemplate;
				bool flag2 = hasTemplate;
				if (flag2)
				{
					ItemDisplayData itemData2 = itemData;
					int amount = (itemData2 != null) ? itemData2.Amount : 0;
					bool hasAmount = amount > 0;
					this.validCanvasGroup.alpha = (hasAmount ? 1f : 0.3f);
					string icon = ItemTemplateHelper.GetIcon(itemKey.ItemType, itemKey.TemplateId);
					this.itemBack.SetIcon(icon);
					sbyte grade = ItemTemplateHelper.GetGrade(itemKey.ItemType, itemKey.TemplateId);
					this.itemBack.SetBack(grade);
					this.itemBack.SetCount(amount, false);
					this.imageWisdomIcon.SetSprite(wisdomIcon, false, null);
					int wisdomCost = itemKey.GetConsumedFeatureMedals();
					string wisdomCostStr = string.Format("X{0}", wisdomCost);
					this.textWisdomCount.text = (wisdomColor.IsNullOrEmpty() ? wisdomCostStr : wisdomCostStr.SetColor(wisdomColor));
					this.buttonUse.ClearAndAddListener(delegate
					{
						onClickButtonUse(index, itemData);
					});
					this.buttonUse.interactable = interactable;
					ItemDisplayData tempItemData = itemData;
					if (tempItemData == null)
					{
						tempItemData = new ItemDisplayData(itemKey.ItemType, itemKey.TemplateId);
					}
					RowItemLine.SetMouseTipDisplayer(true, tempItemData, this.tip);
					sbyte maxUseDistance = ItemTemplateHelper.GetMaxUseDistance(itemKey.ItemType, itemKey.TemplateId);
					this.pointerTrigger.enabled = (interactable && maxUseDistance > 0);
					bool enabled = this.pointerTrigger.enabled;
					if (enabled)
					{
						PointerTrigger pointerTrigger = this.pointerTrigger;
						if (pointerTrigger.EnterEvent == null)
						{
							pointerTrigger.EnterEvent = new UnityEvent();
						}
						this.pointerTrigger.EnterEvent.RemoveAllListeners();
						bool flag3 = onEnter != null;
						if (flag3)
						{
							this.pointerTrigger.EnterEvent.AddListener(delegate()
							{
								onEnter((short)maxUseDistance);
							});
						}
						pointerTrigger = this.pointerTrigger;
						if (pointerTrigger.ExitEvent == null)
						{
							pointerTrigger.ExitEvent = new UnityEvent();
						}
						this.pointerTrigger.ExitEvent.RemoveAllListeners();
						bool flag4 = onExit != null;
						if (flag4)
						{
							this.pointerTrigger.ExitEvent.AddListener(delegate()
							{
								onExit();
							});
						}
					}
				}
				else
				{
					this.buttonAdd.interactable = (onClickButtonSetting != null);
					bool interactable2 = this.buttonAdd.interactable;
					if (interactable2)
					{
						this.buttonAdd.ClearAndAddListener(onClickButtonSetting);
					}
				}
			}
		}

		// Token: 0x04006D31 RID: 27953
		[SerializeField]
		private GameObject rootValid;

		// Token: 0x04006D32 RID: 27954
		[SerializeField]
		private CanvasGroup validCanvasGroup;

		// Token: 0x04006D33 RID: 27955
		[SerializeField]
		private CButton buttonUse;

		// Token: 0x04006D34 RID: 27956
		[SerializeField]
		private ItemBack itemBack;

		// Token: 0x04006D35 RID: 27957
		[SerializeField]
		private CImage imageWisdomIcon;

		// Token: 0x04006D36 RID: 27958
		[SerializeField]
		private TextMeshProUGUI textWisdomCount;

		// Token: 0x04006D37 RID: 27959
		[SerializeField]
		private CButton buttonAdd;

		// Token: 0x04006D38 RID: 27960
		[SerializeField]
		private TooltipInvoker tip;

		// Token: 0x04006D39 RID: 27961
		[SerializeField]
		private PointerTrigger pointerTrigger;
	}
}
