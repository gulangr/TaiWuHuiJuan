using System;
using TMPro;
using UnityEngine;

// Token: 0x0200008D RID: 141
[RequireComponent(typeof(TMP_InputField))]
public class PositiveIntInputValidator : MonoBehaviour
{
	// Token: 0x0600050A RID: 1290 RVA: 0x00022BA4 File Offset: 0x00020DA4
	private void Awake()
	{
		bool flag = this.inputField == null;
		if (flag)
		{
			this.inputField = base.GetComponent<TMP_InputField>();
		}
		bool flag2 = this.inputField != null;
		if (flag2)
		{
			TMP_InputField tmp_InputField = this.inputField;
			tmp_InputField.onValidateInput = (TMP_InputField.OnValidateInput)Delegate.Combine(tmp_InputField.onValidateInput, new TMP_InputField.OnValidateInput(this.ValidateInput));
		}
	}

	// Token: 0x0600050B RID: 1291 RVA: 0x00022C08 File Offset: 0x00020E08
	private char ValidateInput(string text, int charIndex, char addedChar)
	{
		bool flag = !char.IsDigit(addedChar);
		char result;
		if (flag)
		{
			result = '\0';
		}
		else
		{
			bool flag2 = charIndex == 0 && addedChar == '0';
			if (flag2)
			{
				result = '\0';
			}
			else
			{
				result = addedChar;
			}
		}
		return result;
	}

	// Token: 0x0600050C RID: 1292 RVA: 0x00022C40 File Offset: 0x00020E40
	private void OnDestroy()
	{
		bool flag = this.inputField != null;
		if (flag)
		{
			TMP_InputField tmp_InputField = this.inputField;
			tmp_InputField.onValidateInput = (TMP_InputField.OnValidateInput)Delegate.Remove(tmp_InputField.onValidateInput, new TMP_InputField.OnValidateInput(this.ValidateInput));
		}
	}

	// Token: 0x04000413 RID: 1043
	[SerializeField]
	private TMP_InputField inputField;
}
