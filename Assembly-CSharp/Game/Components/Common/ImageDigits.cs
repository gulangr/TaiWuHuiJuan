using System;
using Game.Constants;
using UnityEngine;

namespace Game.Components.Common
{
	// Token: 0x02000F91 RID: 3985
	public class ImageDigits : MonoBehaviour
	{
		// Token: 0x0600B738 RID: 46904 RVA: 0x00537F18 File Offset: 0x00536118
		public void Set(uint number, int minDigits, int maxDigits, string color)
		{
			uint divisor = 1U;
			int digitCount = 0;
			bool flag = number == 0U;
			if (flag)
			{
				divisor = 1U;
				digitCount = 1;
			}
			else
			{
				for (uint temp = number; temp >= 10U; temp /= 10U)
				{
					divisor *= 10U;
				}
				for (uint temp = divisor; temp > 0U; temp /= 10U)
				{
					digitCount++;
				}
			}
			int zeroCount = Math.Max(0, minDigits - digitCount);
			int totalCount = zeroCount + digitCount;
			float scale = (totalCount <= maxDigits) ? 1f : ((float)maxDigits / (float)totalCount);
			for (int i = 0; i < zeroCount; i++)
			{
				this.Add(0U, i, color);
			}
			for (int j = 0; j < digitCount; j++)
			{
				uint val = number / divisor;
				this.Add(val, j + zeroCount, color);
				number %= divisor;
				divisor /= 10U;
			}
			for (int k = totalCount; k < base.transform.childCount; k++)
			{
				base.transform.GetChild(k).gameObject.SetActive(false);
			}
			base.transform.GetComponent<RectTransform>().localScale = new Vector3(scale, scale, scale);
		}

		// Token: 0x0600B739 RID: 46905 RVA: 0x0053804C File Offset: 0x0053624C
		public void SetInt(int number, int minDigits, int maxDigits, string color)
		{
			bool isNegative = number < 0;
			uint absValue = (uint)(isNegative ? (-(uint)number) : number);
			uint divisor = 1U;
			int digitCount = 0;
			bool flag = absValue == 0U;
			if (flag)
			{
				divisor = 1U;
				digitCount = 1;
			}
			else
			{
				for (uint temp = absValue; temp >= 10U; temp /= 10U)
				{
					divisor *= 10U;
				}
				for (uint temp = divisor; temp > 0U; temp /= 10U)
				{
					digitCount++;
				}
			}
			int zeroCount = Math.Max(0, minDigits - digitCount);
			int totalCount = (isNegative ? 1 : 0) + zeroCount + digitCount;
			float scale = (totalCount <= maxDigits) ? 1f : ((float)maxDigits / (float)totalCount);
			int index = 0;
			bool flag2 = isNegative;
			if (flag2)
			{
				this.Add(10U, index, color);
				index++;
			}
			for (int i = 0; i < zeroCount; i++)
			{
				this.Add(0U, index, color);
				index++;
			}
			for (int j = 0; j < digitCount; j++)
			{
				uint val = absValue / divisor;
				this.Add(val, index, color);
				absValue %= divisor;
				divisor /= 10U;
				index++;
			}
			for (int k = totalCount; k < base.transform.childCount; k++)
			{
				base.transform.GetChild(k).gameObject.SetActive(false);
			}
			base.transform.GetComponent<RectTransform>().localScale = new Vector3(scale, scale, scale);
		}

		// Token: 0x0600B73A RID: 46906 RVA: 0x005381C8 File Offset: 0x005363C8
		private void Add(uint val, int index, string color)
		{
			bool flag = base.transform.childCount <= index;
			if (flag)
			{
				Object.Instantiate<CImage>(this.template, base.transform);
			}
			Transform obj = base.transform.GetChild(index);
			CImage image = obj.GetComponent<CImage>();
			image.SetSprite(PathConstants.Numbers[(int)val], false, null);
			image.SetColor(Colors.Instance[color]);
			obj.gameObject.SetActive(true);
		}

		// Token: 0x04008E4F RID: 36431
		public CImage template;
	}
}
