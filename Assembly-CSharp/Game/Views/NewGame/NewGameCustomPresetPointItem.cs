using System;
using FrameWork;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.NewGame
{
	// Token: 0x020007E6 RID: 2022
	public class NewGameCustomPresetPointItem : MonoBehaviour
	{
		// Token: 0x0600628F RID: 25231 RVA: 0x002D1E28 File Offset: 0x002D0028
		public void Initialize(sbyte type, Action<sbyte, int> onValueDelta)
		{
			this._type = type;
			this._onValueDelta = onValueDelta;
			this.addButton.onClick.ResetListener(new Action(this.OnClickAdd));
			this.removeButton.onClick.ResetListener(new Action(this.OnClickRemove));
			this.valueText.onEndEdit.AddListener(new UnityAction<string>(this.OnValueEndEdit));
			this.valueText.contentType = TMP_InputField.ContentType.Custom;
			this.valueText.characterValidation = TMP_InputField.CharacterValidation.Digit;
			this._addButtonTip = this.addButton.gameObject.GetOrAddComponent<TooltipInvoker>();
			this._addButtonTip.Type = TipType.SingleDesc;
		}

		// Token: 0x06006290 RID: 25232 RVA: 0x002D1ED8 File Offset: 0x002D00D8
		public void RefreshItem(string iconName, string displayName, int value, int spentPoints, bool canAdd, bool canRemove, string addButtonDisableTip = null)
		{
			this.icon.SetSprite(iconName, false, null);
			this.nameText.text = displayName;
			this._currentValue = value;
			this.valueText.text = value.ToString();
			this.spentPointText.text = LanguageKey.LK_Brackets_Fix.TrFormat(spentPoints.ToString().SetColor("lightblue"));
			this.addButton.interactable = canAdd;
			this.removeButton.interactable = canRemove;
			bool flag = !canAdd && !string.IsNullOrEmpty(addButtonDisableTip);
			if (flag)
			{
				TooltipInvoker addButtonTip = this._addButtonTip;
				if (addButtonTip.RuntimeParam == null)
				{
					addButtonTip.RuntimeParam = new ArgumentBox();
				}
				this._addButtonTip.RuntimeParam.Clear();
				this._addButtonTip.RuntimeParam.Set("arg0", addButtonDisableTip);
				this._addButtonTip.Refresh(false, -1);
			}
			this._addButtonTip.enabled = (!canAdd && !string.IsNullOrEmpty(addButtonDisableTip));
		}

		// Token: 0x06006291 RID: 25233 RVA: 0x002D1FE8 File Offset: 0x002D01E8
		private void OnValueEndEdit(string input)
		{
			int parsedValue;
			bool flag = string.IsNullOrWhiteSpace(input) || !int.TryParse(input.Trim(), out parsedValue);
			if (flag)
			{
				this.valueText.text = this._currentValue.ToString();
			}
			else
			{
				parsedValue = Mathf.Max(0, parsedValue);
				this._onValueDelta(this._type, parsedValue - this._currentValue);
				this.valueText.text = this._currentValue.ToString();
			}
		}

		// Token: 0x06006292 RID: 25234 RVA: 0x002D2068 File Offset: 0x002D0268
		private void OnClickAdd()
		{
			this._onValueDelta(this._type, 1);
		}

		// Token: 0x06006293 RID: 25235 RVA: 0x002D207E File Offset: 0x002D027E
		private void OnClickRemove()
		{
			this._onValueDelta(this._type, -1);
		}

		// Token: 0x04004494 RID: 17556
		[SerializeField]
		private CImage icon;

		// Token: 0x04004495 RID: 17557
		[SerializeField]
		private TextMeshProUGUI nameText;

		// Token: 0x04004496 RID: 17558
		[SerializeField]
		private TMP_InputField valueText;

		// Token: 0x04004497 RID: 17559
		[SerializeField]
		private TextMeshProUGUI spentPointText;

		// Token: 0x04004498 RID: 17560
		[SerializeField]
		private CButton addButton;

		// Token: 0x04004499 RID: 17561
		[SerializeField]
		private CButton removeButton;

		// Token: 0x0400449A RID: 17562
		private sbyte _type;

		// Token: 0x0400449B RID: 17563
		private Action<sbyte, int> _onValueDelta;

		// Token: 0x0400449C RID: 17564
		private TooltipInvoker _addButtonTip;

		// Token: 0x0400449D RID: 17565
		private int _currentValue;
	}
}
