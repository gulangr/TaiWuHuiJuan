using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

// Token: 0x02000070 RID: 112
[RequireComponent(typeof(TMP_InputField))]
public class InputCharValidator : MonoBehaviour
{
	// Token: 0x0600041F RID: 1055 RVA: 0x000199F8 File Offset: 0x00017BF8
	private void Awake()
	{
		if (this.Input == null)
		{
			this.Input = base.GetComponent<TMP_InputField>();
		}
		TMP_Text textComponent = this.Input.textComponent;
		this._asset = (((textComponent != null) ? textComponent.font : null) ?? this.Input.fontAsset);
		bool convertAllSpacesToSpace = this._convertAllSpacesToSpace;
		if (convertAllSpacesToSpace)
		{
			TMP_InputField input = this.Input;
			input.onValidateInput = (TMP_InputField.OnValidateInput)Delegate.Combine(input.onValidateInput, new TMP_InputField.OnValidateInput(InputCharValidator.ConvertSpace));
		}
		bool flag = !this._allowRichTextSpecialChar;
		if (flag)
		{
			TMP_InputField input2 = this.Input;
			input2.onValidateInput = (TMP_InputField.OnValidateInput)Delegate.Combine(input2.onValidateInput, new TMP_InputField.OnValidateInput(InputCharValidator.ValidateInput));
		}
		bool flag2 = this.validateFonts;
		if (flag2)
		{
			TMP_InputField input3 = this.Input;
			input3.onValidateInput = (TMP_InputField.OnValidateInput)Delegate.Combine(input3.onValidateInput, new TMP_InputField.OnValidateInput(this.ValidateFonts));
		}
	}

	// Token: 0x06000420 RID: 1056 RVA: 0x00019ADF File Offset: 0x00017CDF
	private static char ConvertSpace(string text, int charIndex, char addedChar)
	{
		return InputCharValidator.Spaces.Contains(addedChar) ? ' ' : addedChar;
	}

	// Token: 0x06000421 RID: 1057 RVA: 0x00019AF3 File Offset: 0x00017CF3
	private static char ValidateInput(string text, int charIndex, char addedChar)
	{
		return InputCharValidator.InvalidChars.Contains(addedChar) ? '\0' : addedChar;
	}

	// Token: 0x06000422 RID: 1058 RVA: 0x00019B08 File Offset: 0x00017D08
	private char ValidateFonts(string text, int charIndex, char addedChar)
	{
		return this.fallbackAssets.Prepend(this._asset).Any((TMP_FontAsset x) => x.HasCharacter(addedChar, false, false)) ? addedChar : '\0';
	}

	// Token: 0x04000287 RID: 647
	private static readonly HashSet<char> InvalidChars = new HashSet<char>
	{
		'<',
		'>',
		'\\'
	};

	// Token: 0x04000288 RID: 648
	private static readonly HashSet<char> PoorChars = new HashSet<char>();

	// Token: 0x04000289 RID: 649
	private static readonly HashSet<char> Spaces = new HashSet<char>
	{
		'\u0001',
		'\u0002',
		'\u0003',
		'\u0004',
		'\u0005',
		'\u0006',
		'\a',
		'\b',
		'\t',
		'\n',
		'\v',
		'\f',
		'\r',
		'\u000e',
		'\u000f',
		'\u0010',
		'\u0011',
		'\u0012',
		'\u0013',
		'\u0014',
		'\u0015',
		'\u0016',
		'\u0017',
		'\u0018',
		'\u0019',
		' ',
		'\u001a',
		'\u001b',
		'\u001c',
		'\u001d',
		'\u001e',
		'\u001f',
		'\u007f',
		'\u0085',
		'\u2028',
		'\u2029'
	};

	// Token: 0x0400028A RID: 650
	public TMP_InputField Input;

	// Token: 0x0400028B RID: 651
	private TMP_FontAsset _asset;

	// Token: 0x0400028C RID: 652
	[SerializeField]
	private TMP_FontAsset[] fallbackAssets;

	// Token: 0x0400028D RID: 653
	[SerializeField]
	private bool _convertAllSpacesToSpace = true;

	// Token: 0x0400028E RID: 654
	[SerializeField]
	private bool _allowRichTextSpecialChar = true;

	// Token: 0x0400028F RID: 655
	[SerializeField]
	private bool validateFonts = false;
}
