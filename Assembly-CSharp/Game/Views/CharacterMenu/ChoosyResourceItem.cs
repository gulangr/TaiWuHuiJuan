using System;
using Config;
using FrameWork.UISystem.UIElements;
using Game.Components.Item;
using GameData.Domains.Item.Display;
using TMPro;
using UnityEngine;

namespace Game.Views.CharacterMenu
{
	// Token: 0x02000B7A RID: 2938
	public class ChoosyResourceItem : MonoBehaviour
	{
		// Token: 0x0600913C RID: 37180 RVA: 0x0043AE0C File Offset: 0x0043900C
		public void Init(sbyte resourceType, Action onClickButtonMore, Action onClickButtonLess, Action<string> onEndEdit, Action<float> onSliderValueChanged)
		{
			this._resourceType = resourceType;
			short templateId = Convert.ToInt16((int)this._resourceType);
			this._itemData = new ItemDisplayData(12, templateId);
			ResourceTypeItem config = ResourceType.Instance[this._resourceType];
			this.itemBack.Set(this._itemData, false);
			this.imageIcon.SetSprite(config.Icon, false, null);
			this.textName.text = config.Name;
			this.buttonMore.ClearAndAddListener(onClickButtonMore);
			this.buttonLess.ClearAndAddListener(onClickButtonLess);
			this.inputField.onFocusSelectAll = true;
			this.inputField.onEndEdit.RemoveAllListeners();
			this.inputField.onEndEdit.AddListener(delegate(string value)
			{
				onEndEdit(value);
			});
			this.slider.onValueChanged.RemoveAllListeners();
			this.slider.onValueChanged.AddListener(delegate(float value)
			{
				onSliderValueChanged(value);
			});
			this.slider.wholeNumbers = true;
		}

		// Token: 0x0600913D RID: 37181 RVA: 0x0043AF2C File Offset: 0x0043912C
		public void Set(int ownAmount, int selectAmount)
		{
			this._itemData.Amount = ownAmount;
			this.itemBack.Set(this._itemData, false);
			this._ownAmount = ownAmount;
			this.textAmount.text = CommonUtils.GetDisplayStringForNum(ownAmount, 100000);
			int target = selectAmount;
			this.inputField.SetTextWithoutNotify(target.ToString());
			this.inputField.interactable = (ownAmount > this._choosyCostUnit);
			this.slider.minValue = 0f;
			this.slider.maxValue = (float)(ownAmount / this._choosyCostUnit);
			this.slider.SetValueWithoutNotify((float)(target / this._choosyCostUnit));
			this.slider.interactable = (ownAmount > this._choosyCostUnit);
		}

		// Token: 0x0600913E RID: 37182 RVA: 0x0043AFF3 File Offset: 0x004391F3
		public void SetButtonLessInteractable(bool interactable)
		{
			this.buttonLess.interactable = interactable;
		}

		// Token: 0x0600913F RID: 37183 RVA: 0x0043B003 File Offset: 0x00439203
		public void SetButtonMoreInteractable(bool interactable)
		{
			this.buttonMore.interactable = interactable;
		}

		// Token: 0x04006FDD RID: 28637
		[SerializeField]
		private ItemBack itemBack;

		// Token: 0x04006FDE RID: 28638
		[SerializeField]
		private CButton buttonMore;

		// Token: 0x04006FDF RID: 28639
		[SerializeField]
		private CButton buttonLess;

		// Token: 0x04006FE0 RID: 28640
		[SerializeField]
		private TMP_InputField inputField;

		// Token: 0x04006FE1 RID: 28641
		[SerializeField]
		private CSlider slider;

		// Token: 0x04006FE2 RID: 28642
		[SerializeField]
		private CImage imageIcon;

		// Token: 0x04006FE3 RID: 28643
		[SerializeField]
		private TextMeshProUGUI textName;

		// Token: 0x04006FE4 RID: 28644
		[SerializeField]
		private TextMeshProUGUI textAmount;

		// Token: 0x04006FE5 RID: 28645
		private ItemDisplayData _itemData;

		// Token: 0x04006FE6 RID: 28646
		private sbyte _resourceType;

		// Token: 0x04006FE7 RID: 28647
		private int _ownAmount;

		// Token: 0x04006FE8 RID: 28648
		private readonly int _choosyCostUnit = GlobalConfig.Instance.ChoosyResourceBaseCost;
	}
}
