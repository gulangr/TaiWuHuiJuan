using System;
using TMPro;
using UnityEngine;

// Token: 0x020003F7 RID: 1015
[CreateAssetMenu(fileName = "VersionInputValidator.asset", menuName = "TextMeshPro/InputValidators", order = 100)]
[Serializable]
public class VersionInputValidator : TMP_InputValidator
{
	// Token: 0x06003CFE RID: 15614 RVA: 0x001EAC6C File Offset: 0x001E8E6C
	public override char Validate(ref string text, ref int pos, char ch)
	{
		bool flag = pos > 0 && pos >= this.characterLimit;
		char result;
		if (flag)
		{
			result = '\0';
		}
		else
		{
			bool flag2 = ch == '.';
			if (flag2)
			{
				text += ch.ToString();
				pos++;
				result = ch;
			}
			else
			{
				bool flag3 = ch >= '0' && ch <= '9';
				if (flag3)
				{
					text += ch.ToString();
					pos++;
					result = ch;
				}
				else
				{
					result = '\0';
				}
			}
		}
		return result;
	}

	// Token: 0x04002BAD RID: 11181
	[SerializeField]
	private int characterLimit;
}
