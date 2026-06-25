using System;
using Config;
using FrameWork.UISystem.UIElements;
using Game.Components.Character;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Views.NewGame
{
	// Token: 0x020007E2 RID: 2018
	public class NewGameCustomPresetFeatureItem : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
	{
		// Token: 0x06006268 RID: 25192 RVA: 0x002D1727 File Offset: 0x002CF927
		public void Initialize(short entryId, Action<short> onClickEntry)
		{
			this._entryId = entryId;
			this._onClickEntry = onClickEntry;
			this.selectButton.onClick.ResetListener(new Action(this.OnClickItem));
		}

		// Token: 0x06006269 RID: 25193 RVA: 0x002D1758 File Offset: 0x002CF958
		public void RefreshItem(CharacterFeatureItem featureItem, int pointCost, bool isSelected, bool isDisabled)
		{
			this._isDisabled = isDisabled;
			this.feature.Set(featureItem.TemplateId, -1, false, -1);
			this.feature.SetTipTemplateDataOnly(true);
			this.featurePointText.text = pointCost.ToString().SetColor("lightblue");
			this.selectedState.SetActive(isSelected);
			this.selectButton.interactable = (isSelected || !isDisabled);
			this.hover.SetActive(false);
			this.itemCanvasGroup.alpha = (isDisabled ? 0.5f : 1f);
		}

		// Token: 0x0600626A RID: 25194 RVA: 0x002D17FC File Offset: 0x002CF9FC
		public void OnPointerEnter(PointerEventData eventData)
		{
			bool isDisabled = this._isDisabled;
			if (isDisabled)
			{
				this.hover.SetActive(false);
			}
			else
			{
				this.hover.SetActive(true);
			}
		}

		// Token: 0x0600626B RID: 25195 RVA: 0x002D1831 File Offset: 0x002CFA31
		public void OnPointerExit(PointerEventData eventData)
		{
			this.hover.SetActive(false);
		}

		// Token: 0x0600626C RID: 25196 RVA: 0x002D1841 File Offset: 0x002CFA41
		private void OnClickItem()
		{
			Action<short> onClickEntry = this._onClickEntry;
			if (onClickEntry != null)
			{
				onClickEntry(this._entryId);
			}
		}

		// Token: 0x04004478 RID: 17528
		[SerializeField]
		private Feature feature;

		// Token: 0x04004479 RID: 17529
		[SerializeField]
		private TextMeshProUGUI featurePointText;

		// Token: 0x0400447A RID: 17530
		[SerializeField]
		private CButton selectButton;

		// Token: 0x0400447B RID: 17531
		[SerializeField]
		private GameObject selectedState;

		// Token: 0x0400447C RID: 17532
		[SerializeField]
		private GameObject hover;

		// Token: 0x0400447D RID: 17533
		[SerializeField]
		private CanvasGroup itemCanvasGroup;

		// Token: 0x0400447E RID: 17534
		private short _entryId;

		// Token: 0x0400447F RID: 17535
		private Action<short> _onClickEntry;

		// Token: 0x04004480 RID: 17536
		private bool _isDisabled;
	}
}
