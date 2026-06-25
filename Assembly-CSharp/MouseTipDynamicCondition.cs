using System;
using System.Collections.Generic;
using FrameWork;
using TMPro;
using UnityEngine;

// Token: 0x02000292 RID: 658
public class MouseTipDynamicCondition : MouseTipBase
{
	// Token: 0x060029EE RID: 10734 RVA: 0x0013E369 File Offset: 0x0013C569
	protected override void Init(ArgumentBox argsBox)
	{
		if (this._conditionPool == null)
		{
			this._conditionPool = new PoolItem(string.Empty, this.ConditionPrefab);
		}
		this.RefreshBaseInfo(argsBox);
		this.RefreshConditionList(argsBox);
	}

	// Token: 0x060029EF RID: 10735 RVA: 0x0013E39A File Offset: 0x0013C59A
	protected override void OnDisable()
	{
		base.OnDisable();
		this.CollectAllConditions();
	}

	// Token: 0x060029F0 RID: 10736 RVA: 0x0013E3AC File Offset: 0x0013C5AC
	private void RefreshBaseInfo(ArgumentBox argsBox)
	{
		string title;
		bool flag = argsBox.Get("Title", out title);
		if (flag)
		{
			this.Title.text = title.ColorReplace();
		}
		else
		{
			this.Title.text = string.Empty;
		}
		string desc;
		bool flag2 = argsBox.Get("Desc", out desc);
		if (flag2)
		{
			this.DescLabel.text = desc.ColorReplace();
			this.DescLabel.GetComponent<TMPTextSpriteHelper>().Parse();
		}
		else
		{
			this.DescLabel.text = string.Empty;
		}
		string subTitle;
		bool flag3 = argsBox.Get("SubTitle", out subTitle);
		if (flag3)
		{
			this.SubTitle.text = LocalStringManager.GetFormat(LanguageKey.LK_SurroundWithChineseSquareBrackets, subTitle.ColorReplace());
		}
		else
		{
			this.SubTitle.text = string.Empty;
		}
		string extraDesc;
		bool flag4 = argsBox.Get("ExtraDesc", out extraDesc);
		if (flag4)
		{
			this.ExtraDescLabel.text = extraDesc.ColorReplace();
		}
		else
		{
			this.ExtraDescLabel.text = string.Empty;
		}
	}

	// Token: 0x060029F1 RID: 10737 RVA: 0x0013E4BC File Offset: 0x0013C6BC
	private void RefreshConditionList(ArgumentBox argsBox)
	{
		List<MouseTipDynamicCondition.ConditionData> dataList;
		bool flag = !argsBox.Get<List<MouseTipDynamicCondition.ConditionData>>("Conditions", out dataList);
		if (!flag)
		{
			base.CGet<TextMeshProUGUI>("SpecialText").gameObject.SetActive(false);
			int i = 0;
			int max = dataList.Count;
			while (i < max)
			{
				GameObject line = this._conditionPool.GetObject();
				line.transform.SetParent(this.ConditionRoot, false);
				Refers refers = line.GetComponent<Refers>();
				MouseTipDynamicCondition.ConditionData data = dataList[i];
				refers.CGet<CImage>("ConditionIcon").SetSprite(data.Icon, false, null);
				bool flag2 = data.Icon.IsNullOrEmpty();
				if (flag2)
				{
					refers.CGet<CImage>("ConditionIcon").gameObject.SetActive(false);
				}
				refers.CGet<TextMeshProUGUI>("ConditionName").text = data.Name.ColorReplace();
				TextMeshProUGUI addLabel = refers.CGet<TextMeshProUGUI>("AddValue");
				addLabel.text = data.AddValueString.ColorReplace();
				addLabel.GetComponent<TMPTextSpriteHelper>().Parse();
				TextMeshProUGUI reduceLabel = refers.CGet<TextMeshProUGUI>("ReduceValue");
				reduceLabel.text = data.ReduceValueString.ColorReplace();
				reduceLabel.GetComponent<TMPTextSpriteHelper>().Parse();
				bool flag3 = data.AddValueString.IsNullOrEmpty() && data.ReduceValueString.IsNullOrEmpty();
				if (flag3)
				{
					refers.CGet<GameObject>("Colon").SetActive(false);
				}
				bool isSpecial = data.IsSpecial;
				if (isSpecial)
				{
					line.transform.Find("Dot").gameObject.SetActive(false);
				}
				bool flag4 = data.IsSpecial && !data.Name.IsNullOrEmpty();
				if (flag4)
				{
					line.gameObject.SetActive(false);
					base.CGet<TextMeshProUGUI>("SpecialText").text = "   " + data.Name.ColorReplace();
					base.CGet<TextMeshProUGUI>("SpecialText").gameObject.SetActive(true);
				}
				i++;
			}
		}
	}

	// Token: 0x060029F2 RID: 10738 RVA: 0x0013E6E4 File Offset: 0x0013C8E4
	private void CollectAllConditions()
	{
		foreach (object obj in this.ConditionRoot)
		{
			Transform child = (Transform)obj;
			bool activeSelf = child.gameObject.activeSelf;
			if (activeSelf)
			{
				this._conditionPool.DestroyObject(child.gameObject);
			}
		}
	}

	// Token: 0x04001E71 RID: 7793
	public TextMeshProUGUI Title;

	// Token: 0x04001E72 RID: 7794
	public TextMeshProUGUI SubTitle;

	// Token: 0x04001E73 RID: 7795
	public TextMeshProUGUI DescLabel;

	// Token: 0x04001E74 RID: 7796
	public GameObject ConditionPrefab;

	// Token: 0x04001E75 RID: 7797
	public TextMeshProUGUI ExtraDescLabel;

	// Token: 0x04001E76 RID: 7798
	public RectTransform ConditionRoot;

	// Token: 0x04001E77 RID: 7799
	private PoolItem _conditionPool;

	// Token: 0x020015FD RID: 5629
	public struct ConditionData
	{
		// Token: 0x0400A69D RID: 42653
		public string Icon;

		// Token: 0x0400A69E RID: 42654
		public string Name;

		// Token: 0x0400A69F RID: 42655
		public string AddValueString;

		// Token: 0x0400A6A0 RID: 42656
		public string ReduceValueString;

		// Token: 0x0400A6A1 RID: 42657
		public bool IsSpecial;
	}
}
