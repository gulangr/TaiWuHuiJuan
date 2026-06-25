using System;
using Config;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.MapElement
{
	// Token: 0x02000931 RID: 2353
	public class MapElementDisplayRuleItem : MonoBehaviour
	{
		// Token: 0x17000CB4 RID: 3252
		// (get) Token: 0x06006DCE RID: 28110 RVA: 0x0032C226 File Offset: 0x0032A426
		public short ItemId
		{
			get
			{
				return this._itemId;
			}
		}

		// Token: 0x06006DCF RID: 28111 RVA: 0x0032C230 File Offset: 0x0032A430
		public void Refresh(short itemId, bool interactable)
		{
			this._itemId = itemId;
			bool state = SingletonObject.getInstance<GlobalSettings>().GetMapElementDisplayRuleItemState(itemId, false);
			MapElementDisplayRuleItemItem config = MapElementDisplayRuleItem.Instance[itemId];
			this.imageIcon.SetSprite(config.Icon, false, null);
			this.textName.SetText(config.Name, true);
			this.buttonOpen.gameObject.SetActive(state);
			this.buttonClose.gameObject.SetActive(!state);
			Selectable selectable = this.buttonOpen;
			this.buttonClose.interactable = interactable;
			selectable.interactable = interactable;
			this.tipButtonClose.enabled = (!interactable && this._itemId == 41);
			bool enabled = this.tipButtonClose.enabled;
			if (enabled)
			{
				MapElementDisplayRuleItemItem mapInteractPickupConfig = MapElementDisplayRuleItem.Instance[40];
				string content = LanguageKey.LK_MapElement_MapInteractInvisiblePickup_Disable.TrFormat(mapInteractPickupConfig.Name);
				this.tipButtonClose.PresetParam = new string[]
				{
					content
				};
			}
			this.buttonOpen.ClearAndAddListener(new Action(this.OnClickButtonOpen));
			this.buttonClose.ClearAndAddListener(new Action(this.OnClickButtonClose));
		}

		// Token: 0x06006DD0 RID: 28112 RVA: 0x0032C35C File Offset: 0x0032A55C
		private void OnClickButtonOpen()
		{
			this.SetState(false);
		}

		// Token: 0x06006DD1 RID: 28113 RVA: 0x0032C366 File Offset: 0x0032A566
		private void OnClickButtonClose()
		{
			this.SetState(true);
		}

		// Token: 0x06006DD2 RID: 28114 RVA: 0x0032C370 File Offset: 0x0032A570
		private void SetState(bool isOn)
		{
			GlobalSettings settings = SingletonObject.getInstance<GlobalSettings>();
			settings.SetMapElementDisplayRuleItemState(this._itemId, isOn);
			bool flag = this._itemId == 40 && !isOn;
			if (flag)
			{
				settings.SetMapElementDisplayRuleItemState(41, false);
			}
			GEvent.OnEvent(UiEvents.OnForceRefreshAllMapBlock, null);
		}

		// Token: 0x04005177 RID: 20855
		[SerializeField]
		private CImage imageIcon;

		// Token: 0x04005178 RID: 20856
		[SerializeField]
		private TextMeshProUGUI textName;

		// Token: 0x04005179 RID: 20857
		[SerializeField]
		private CButton buttonOpen;

		// Token: 0x0400517A RID: 20858
		[SerializeField]
		private TooltipInvoker tipButtonClose;

		// Token: 0x0400517B RID: 20859
		[SerializeField]
		private CButton buttonClose;

		// Token: 0x0400517C RID: 20860
		private short _itemId;

		// Token: 0x0400517D RID: 20861
		private bool _lastIsOn;
	}
}
