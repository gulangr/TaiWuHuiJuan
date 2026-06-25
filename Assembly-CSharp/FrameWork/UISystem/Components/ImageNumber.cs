using System;
using UnityEngine;

namespace FrameWork.UISystem.Components
{
	// Token: 0x02001018 RID: 4120
	public class ImageNumber : MonoBehaviour
	{
		// Token: 0x0600BC67 RID: 48231 RVA: 0x0055A994 File Offset: 0x00558B94
		private void Awake()
		{
			bool flag = this.template == null && base.transform.childCount > 0;
			if (flag)
			{
				this.template = base.transform.GetChild(0).gameObject;
			}
		}

		// Token: 0x0600BC68 RID: 48232 RVA: 0x0055A9DC File Offset: 0x00558BDC
		public void SetAsInfinity()
		{
			base.transform.GetChild(0).GetComponent<CImage>().SetSprite(this.numberPrefix + "infinity", false, null);
			this.SetDigit(1);
		}

		// Token: 0x0600BC69 RID: 48233 RVA: 0x0055AA10 File Offset: 0x00558C10
		public void Set(int value)
		{
			int digit = 0;
			foreach (char i in Mathf.Abs(value).ToString())
			{
				bool flag = digit == base.transform.childCount;
				if (flag)
				{
					Object.Instantiate<GameObject>(this.template, base.transform);
				}
				string spriteName = this.numberPrefix + i.ToString();
				base.transform.GetChild(digit).GetComponent<CImage>().SetSprite(spriteName, true, null);
				digit++;
			}
			this.SetDigit(digit);
		}

		// Token: 0x0600BC6A RID: 48234 RVA: 0x0055AAB0 File Offset: 0x00558CB0
		public void SetColor(Color color)
		{
			for (int i = 0; i < base.transform.childCount; i++)
			{
				base.transform.GetChild(i).GetComponent<CImage>().color = color;
			}
		}

		// Token: 0x0600BC6B RID: 48235 RVA: 0x0055AAF0 File Offset: 0x00558CF0
		private void SetDigit(int digit)
		{
			for (int i = 0; i < base.transform.childCount; i++)
			{
				base.transform.GetChild(i).gameObject.SetActive(i < digit);
			}
		}

		// Token: 0x0400910F RID: 37135
		[SerializeField]
		private string numberPrefix;

		// Token: 0x04009110 RID: 37136
		[SerializeField]
		private GameObject template;
	}
}
