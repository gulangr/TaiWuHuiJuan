using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020002FE RID: 766
public class SideSelectButton : MonoBehaviour, ILanguage
{
	// Token: 0x170004DD RID: 1245
	// (get) Token: 0x06002CD3 RID: 11475 RVA: 0x001611E0 File Offset: 0x0015F3E0
	// (set) Token: 0x06002CD4 RID: 11476 RVA: 0x001611E8 File Offset: 0x0015F3E8
	public LocalStringManager.LanguageType LanguageType
	{
		get
		{
			return this.languageType;
		}
		set
		{
			this.OnLanguageChange(value);
		}
	}

	// Token: 0x170004DE RID: 1246
	// (get) Token: 0x06002CD5 RID: 11477 RVA: 0x001611F2 File Offset: 0x0015F3F2
	// (set) Token: 0x06002CD6 RID: 11478 RVA: 0x001611FA File Offset: 0x0015F3FA
	public bool ShowNum
	{
		get
		{
			return this.showNum;
		}
		set
		{
			this.showNum = value;
			this.UpdateShowNum();
		}
	}

	// Token: 0x170004DF RID: 1247
	// (get) Token: 0x06002CD7 RID: 11479 RVA: 0x0016120B File Offset: 0x0015F40B
	// (set) Token: 0x06002CD8 RID: 11480 RVA: 0x00161213 File Offset: 0x0015F413
	public string Text
	{
		get
		{
			return this.text;
		}
		set
		{
			this.text = value;
			this.UpdateText();
		}
	}

	// Token: 0x170004E0 RID: 1248
	// (get) Token: 0x06002CD9 RID: 11481 RVA: 0x00161224 File Offset: 0x0015F424
	// (set) Token: 0x06002CDA RID: 11482 RVA: 0x0016122C File Offset: 0x0015F42C
	public int CurrNum
	{
		get
		{
			return this.currNum;
		}
		set
		{
			this.currNum = value;
			this.UpdateNum();
		}
	}

	// Token: 0x170004E1 RID: 1249
	// (get) Token: 0x06002CDB RID: 11483 RVA: 0x0016123D File Offset: 0x0015F43D
	// (set) Token: 0x06002CDC RID: 11484 RVA: 0x00161245 File Offset: 0x0015F445
	public int MaxNum
	{
		get
		{
			return this.maxNum;
		}
		set
		{
			this.maxNum = value;
			this.UpdateNum();
		}
	}

	// Token: 0x170004E2 RID: 1250
	// (get) Token: 0x06002CDD RID: 11485 RVA: 0x00161256 File Offset: 0x0015F456
	// (set) Token: 0x06002CDE RID: 11486 RVA: 0x0016126C File Offset: 0x0015F46C
	public ValueTuple<int, int> Num
	{
		get
		{
			return new ValueTuple<int, int>(this.CurrNum, this.MaxNum);
		}
		set
		{
			int item = value.Item1;
			int item2 = value.Item2;
			this.CurrNum = item;
			this.MaxNum = item2;
			this.ShowNum = (value.Item1 >= 0 && value.Item2 >= 0);
			this.UpdateNum();
		}
	}

	// Token: 0x06002CDF RID: 11487 RVA: 0x001612C4 File Offset: 0x0015F4C4
	public void UpdateNum()
	{
		bool flag = this.currNum < 0 || this.maxNum < 0 || !this.showNum;
		if (!flag)
		{
			string str = string.Format("({0}/{1})", this.currNum, this.maxNum);
			foreach (TMP_Text innerText in this.numbers)
			{
				innerText.text = str;
			}
		}
	}

	// Token: 0x06002CE0 RID: 11488 RVA: 0x0016133C File Offset: 0x0015F53C
	public void UpdateShowNum()
	{
		foreach (TMP_Text number in this.numbers)
		{
			number.gameObject.SetActive(this.showNum);
		}
		this.UpdateLayout(false);
	}

	// Token: 0x06002CE1 RID: 11489 RVA: 0x00161380 File Offset: 0x0015F580
	public void UpdateText()
	{
		bool flag = this.text != null;
		if (flag)
		{
			foreach (TMP_Text innerText in this.texts)
			{
				innerText.text = this.text;
			}
		}
		this.UpdateLayout(true);
	}

	// Token: 0x06002CE2 RID: 11490 RVA: 0x001613D0 File Offset: 0x0015F5D0
	public void UpdateLayout(bool updateMesh = true)
	{
		if (updateMesh)
		{
			this.texts[0].ForceMeshUpdate(false, false);
			bool flag = this.showNum;
			if (flag)
			{
				this.numbers[0].ForceMeshUpdate(false, false);
			}
		}
		string text = this.text;
		bool flag2;
		if (text != null && text.Length > 0)
		{
			if (this.showNum)
			{
				TMP_Text[] array = this.numbers;
				flag2 = (array != null && array.Length > 0);
			}
			else
			{
				flag2 = true;
			}
		}
		else
		{
			flag2 = false;
		}
		bool flag3 = flag2;
		if (flag3)
		{
			this.layout.preferredHeight = (float)(this.baseHeight + (this.showNum ? this.numHeightDiff : 0) + this.lineHeight * (this.texts[0].textInfo.lineCount + (this.showNum ? this.numbers[0].textInfo.lineCount : 0)));
		}
	}

	// Token: 0x06002CE3 RID: 11491 RVA: 0x001614A8 File Offset: 0x0015F6A8
	public void OnLanguageChange(LocalStringManager.LanguageType language)
	{
		bool flag = language == this.languageType;
		if (!flag)
		{
			this.languageType = language;
			foreach (TMP_Text inner in this.texts)
			{
				TextStyle textStyle = inner.GetComponent<TextStyle>();
				bool flag2 = textStyle != null;
				if (flag2)
				{
					textStyle.OnLanguageChange(this.languageType);
				}
			}
		}
	}

	// Token: 0x06002CE4 RID: 11492 RVA: 0x00161508 File Offset: 0x0015F708
	private void OnEnable()
	{
		bool flag = this.fixedTextWidth <= 0f;
		if (!flag)
		{
			foreach (TMP_Text inner in this.texts.Concat(this.numbers))
			{
				RectTransform component = inner.GetComponent<RectTransform>();
				if (component != null)
				{
					component.SetWidth(this.fixedTextWidth);
				}
			}
		}
	}

	// Token: 0x0400207F RID: 8319
	[SerializeField]
	private LocalStringManager.LanguageType languageType = LocalStringManager.LanguageType.CN;

	// Token: 0x04002080 RID: 8320
	[SerializeField]
	private bool showNum;

	// Token: 0x04002081 RID: 8321
	[SerializeField]
	private string text;

	// Token: 0x04002082 RID: 8322
	[SerializeField]
	private int currNum;

	// Token: 0x04002083 RID: 8323
	[SerializeField]
	private int maxNum;

	// Token: 0x04002084 RID: 8324
	[SerializeField]
	private int baseHeight = 16;

	// Token: 0x04002085 RID: 8325
	[SerializeField]
	private int lineHeight = 20;

	// Token: 0x04002086 RID: 8326
	[SerializeField]
	private int numHeightDiff = -10;

	// Token: 0x04002087 RID: 8327
	[SerializeField]
	private TMP_Text[] texts;

	// Token: 0x04002088 RID: 8328
	[SerializeField]
	private TMP_Text[] numbers;

	// Token: 0x04002089 RID: 8329
	[SerializeField]
	private LayoutElement layout;

	// Token: 0x0400208A RID: 8330
	[Tooltip("文本固定长度，小于等于0时不做限制")]
	[SerializeField]
	private float fixedTextWidth = 150f;
}
