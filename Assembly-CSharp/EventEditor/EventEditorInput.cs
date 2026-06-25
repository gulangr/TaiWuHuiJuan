using System;
using FrameWork;
using TMPro;
using UnityEngine;

namespace EventEditor
{
	// Token: 0x02000644 RID: 1604
	public class EventEditorInput : EventEditorSubPageBase
	{
		// Token: 0x06004C18 RID: 19480 RVA: 0x0023E205 File Offset: 0x0023C405
		public static void Init(EventEditorInput instance)
		{
			EventEditorInput.Instance = instance;
			EventEditorInput.Instance.InternalInit();
		}

		// Token: 0x06004C19 RID: 19481 RVA: 0x0023E219 File Offset: 0x0023C419
		protected override void InternalInit()
		{
			base.gameObject.SetActive(false);
			this._input = this.inputField;
			this._input.richText = false;
			this._inputRect = this._input.GetComponent<RectTransform>();
		}

		// Token: 0x06004C1A RID: 19482 RVA: 0x0023E253 File Offset: 0x0023C453
		public override void Show()
		{
		}

		// Token: 0x06004C1B RID: 19483 RVA: 0x0023E258 File Offset: 0x0023C458
		public void Show(ArgumentBox argBox)
		{
			Action<string> onEndEdit;
			bool flag = argBox.Get<Action<string>>("OnEditComplete", out onEndEdit);
			if (flag)
			{
				this._input.onEndEdit.AddListener(delegate(string str)
				{
					Action<string> onEndEdit = onEndEdit;
					if (onEndEdit != null)
					{
						onEndEdit(str);
					}
					this.Hide();
				});
			}
			this._dynamicSize = false;
			this._input.lineType = TMP_InputField.LineType.SingleLine;
			this._minHeight = 40f;
			bool multiLine;
			bool flag2 = argBox.Get("MultiLine", out multiLine);
			if (flag2)
			{
				bool flag3 = multiLine;
				if (flag3)
				{
					this._input.lineType = TMP_InputField.LineType.MultiLineNewline;
					this._dynamicSize = true;
				}
			}
			TextMeshProUGUI text;
			bool flag4 = argBox.Get<TextMeshProUGUI>("TextComponent", out text);
			if (flag4)
			{
				this._inputRect.pivot = text.rectTransform.pivot;
				this._inputRect.position = text.rectTransform.position;
				this._inputRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, text.rectTransform.sizeDelta.x);
				this._inputRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, text.rectTransform.sizeDelta.y);
				this._input.textComponent.fontSize = text.fontSize;
				TextMeshProUGUI placeHolder = this._input.placeholder as TextMeshProUGUI;
				bool flag5 = placeHolder != null;
				if (flag5)
				{
					placeHolder.fontSize = text.fontSize;
				}
				this._input.textComponent.color = text.color;
				this._input.textComponent.alignment = text.alignment;
				this._minHeight = text.rectTransform.sizeDelta.y + 40f;
			}
			string content;
			bool flag6 = argBox.Get("PreContent", out content);
			if (flag6)
			{
				this._input.text = content;
			}
			base.gameObject.SetActive(true);
			this._input.ActivateInputField();
		}

		// Token: 0x06004C1C RID: 19484 RVA: 0x0023E440 File Offset: 0x0023C640
		private void Update()
		{
			bool dynamicSize = this._dynamicSize;
			if (dynamicSize)
			{
				this._inputRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Max(this._input.textComponent.preferredHeight + 40f, this._minHeight));
			}
		}

		// Token: 0x06004C1D RID: 19485 RVA: 0x0023E488 File Offset: 0x0023C688
		public override void Hide()
		{
			this._input.onEndEdit.RemoveAllListeners();
			base.gameObject.SetActive(false);
		}

		// Token: 0x040034DC RID: 13532
		public static EventEditorInput Instance;

		// Token: 0x040034DD RID: 13533
		private TMP_InputField _input;

		// Token: 0x040034DE RID: 13534
		private RectTransform _inputRect;

		// Token: 0x040034DF RID: 13535
		private bool _dynamicSize;

		// Token: 0x040034E0 RID: 13536
		private float _minHeight;

		// Token: 0x040034E1 RID: 13537
		[SerializeField]
		private TMP_InputField inputField;
	}
}
