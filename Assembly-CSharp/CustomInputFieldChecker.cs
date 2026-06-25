using System;
using TMPro;
using UnityEngine;

// Token: 0x02000056 RID: 86
[RequireComponent(typeof(TMP_InputField))]
public class CustomInputFieldChecker : MonoBehaviour
{
	// Token: 0x060002DB RID: 731 RVA: 0x00011636 File Offset: 0x0000F836
	private void Awake()
	{
		this.Init();
	}

	// Token: 0x060002DC RID: 732 RVA: 0x00011640 File Offset: 0x0000F840
	private void Init()
	{
		bool inited = this._inited;
		if (!inited)
		{
			bool flag = this._inputField == null;
			if (flag)
			{
				this._inputField = base.GetComponent<TMP_InputField>();
			}
			this._inited = true;
		}
	}

	// Token: 0x060002DD RID: 733 RVA: 0x00011680 File Offset: 0x0000F880
	private void OnEnable()
	{
		this._previousFocus = false;
		bool flag = this._caretRect == null;
		if (flag)
		{
			this._caretRect = this._inputField.textViewport.Find("Caret").GetComponent<RectTransform>();
		}
	}

	// Token: 0x060002DE RID: 734 RVA: 0x000116C7 File Offset: 0x0000F8C7
	private void OnDisable()
	{
		this._previousFocus = false;
		this.OnDeselect(null);
	}

	// Token: 0x060002DF RID: 735 RVA: 0x000116D9 File Offset: 0x0000F8D9
	private void OnDeselect(string arg0)
	{
		this.ResetTextComponentPosition();
	}

	// Token: 0x060002E0 RID: 736 RVA: 0x000116E5 File Offset: 0x0000F8E5
	private void OnSelect(string arg0)
	{
		this._previousCompositionLength = 0;
	}

	// Token: 0x060002E1 RID: 737 RVA: 0x000116F4 File Offset: 0x0000F8F4
	private void LateUpdate()
	{
		string comp = Input.compositionString;
		bool isFocused = this._inputField.isFocused;
		if (isFocused)
		{
			bool flag = comp.Length != this._previousCompositionLength;
			if (flag)
			{
				bool flag2 = this._previousCompositionLength == 0;
				if (flag2)
				{
					this._caretRect.gameObject.SetActive(false);
					this._textComponentAnchoredPosition = this._inputField.textComponent.rectTransform.anchoredPosition;
					this._caretAnchoredPosition = this._caretRect.anchoredPosition;
					this._startingCaretPosition = this._inputField.caretPosition;
					this._startingTextlength = this._inputField.text.Length;
					this._startingStringPosition = this._inputField.stringPosition;
				}
				this._inputField.stringPosition = this._startingStringPosition;
				bool flag3 = comp.Length == 0;
				if (flag3)
				{
					this.ResetTextComponentPosition();
					this._inputField.caretPosition = this._inputField.caretPosition;
					this._caretRect.gameObject.SetActive(true);
				}
				this._previousCompositionLength = comp.Length;
			}
		}
		bool flag4 = this._previousFocus && !this._inputField.isFocused;
		if (flag4)
		{
			this.OnDeselect(null);
		}
		else
		{
			bool flag5 = !this._previousFocus && this._inputField.isFocused;
			if (flag5)
			{
				this.OnSelect(null);
			}
		}
		this._previousFocus = this._inputField.isFocused;
	}

	// Token: 0x060002E2 RID: 738 RVA: 0x0001187C File Offset: 0x0000FA7C
	private void ResetTextComponentPosition()
	{
		this._inputField.textComponent.rectTransform.anchoredPosition = this._textComponentAnchoredPosition;
		this._caretRect.anchoredPosition = this._caretAnchoredPosition;
	}

	// Token: 0x04000187 RID: 391
	[SerializeField]
	private TMP_InputField _inputField;

	// Token: 0x04000188 RID: 392
	[SerializeField]
	private RectTransform _caretRect;

	// Token: 0x04000189 RID: 393
	private int _previousCompositionLength = 0;

	// Token: 0x0400018A RID: 394
	private int _startingCaretPosition = 0;

	// Token: 0x0400018B RID: 395
	private int _startingTextlength = 0;

	// Token: 0x0400018C RID: 396
	private int _startingStringPosition = 0;

	// Token: 0x0400018D RID: 397
	private Vector2 _textComponentAnchoredPosition;

	// Token: 0x0400018E RID: 398
	private Vector2 _caretAnchoredPosition;

	// Token: 0x0400018F RID: 399
	private bool _inited = false;

	// Token: 0x04000190 RID: 400
	private bool _previousFocus = false;
}
