using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200040B RID: 1035
public class TangramSpecialNumber : MonoBehaviour
{
	// Token: 0x06003DE0 RID: 15840 RVA: 0x001F1124 File Offset: 0x001EF324
	public void SetNumber(int left, int right, ETangramNumberColor leftColor, ETangramNumberColor rightColor)
	{
		this.currentLeft = left;
		this.currentRight = right;
		List<int> leftChar = TangramSpecialNumber.GetDigits(left);
		List<int> rightChar = TangramSpecialNumber.GetDigits(right);
		CommonUtils.PrepareEnoughChildren(base.transform, this.prefab.gameObject, leftChar.Count + rightChar.Count + 1, null);
		int index = 0;
		string leftPrefix = (leftColor == ETangramNumberColor.Red) ? this.redNumberPrefix : this.normalNumberPrefix;
		string rightPrefix = (rightColor == ETangramNumberColor.Red) ? this.redNumberPrefix : this.normalNumberPrefix;
		for (int i = 0; i < leftChar.Count; i++)
		{
			base.transform.GetChild(index++).GetComponent<CImage>().SetSprite(string.Format("{0}{1}", leftPrefix, leftChar[i]), false, null);
		}
		base.transform.GetChild(index++).GetComponent<CImage>().SetSprite(this.slashIcon, false, null);
		for (int j = 0; j < rightChar.Count; j++)
		{
			base.transform.GetChild(index++).GetComponent<CImage>().SetSprite(string.Format("{0}{1}", rightPrefix, rightChar[j]), false, null);
		}
	}

	// Token: 0x06003DE1 RID: 15841 RVA: 0x001F1270 File Offset: 0x001EF470
	public static List<int> GetDigits(int number)
	{
		List<int> digits = new List<int>();
		bool flag = number == 0;
		List<int> result;
		if (flag)
		{
			digits.Add(0);
			result = digits;
		}
		else
		{
			bool flag2 = number < 0;
			if (flag2)
			{
				number = -number;
			}
			while (number > 0)
			{
				digits.Insert(0, number % 10);
				number /= 10;
			}
			result = digits;
		}
		return result;
	}

	// Token: 0x04002C8C RID: 11404
	public string redNumberPrefix;

	// Token: 0x04002C8D RID: 11405
	public string normalNumberPrefix;

	// Token: 0x04002C8E RID: 11406
	public string slashIcon;

	// Token: 0x04002C8F RID: 11407
	public CImage prefab;

	// Token: 0x04002C90 RID: 11408
	public int currentLeft = 0;

	// Token: 0x04002C91 RID: 11409
	public int currentRight = 0;
}
