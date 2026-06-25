using System;
using System.Collections;
using FrameWork;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020002A6 RID: 678
public class MouseTipJiaoNurturance : MouseTipBase
{
	// Token: 0x1700049D RID: 1181
	// (get) Token: 0x06002A5A RID: 10842 RVA: 0x00144213 File Offset: 0x00142413
	protected override bool CanStick
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06002A5B RID: 10843 RVA: 0x00144218 File Offset: 0x00142418
	protected override void Init(ArgumentBox argsBox)
	{
		TextMeshProUGUI textTitle = base.CGet<TextMeshProUGUI>("Title");
		TextMeshProUGUI textNurturance = base.CGet<TextMeshProUGUI>("Nurturance");
		TextMeshProUGUI textAttribute = base.CGet<TextMeshProUGUI>("Attribute");
		TextMeshProUGUI textSpecialContent = base.CGet<TextMeshProUGUI>("SpecialContent");
		string title;
		argsBox.Get("arg0", out title);
		string nurturance;
		argsBox.Get("arg1", out nurturance);
		string attribute;
		argsBox.Get("arg2", out attribute);
		string specialContent;
		argsBox.Get("arg3", out specialContent);
		textTitle.text = title;
		textNurturance.text = nurturance.ColorReplace();
		textNurturance.GetComponent<TMPTextSpriteHelper>().Parse();
		textAttribute.text = attribute.ColorReplace();
		textAttribute.GetComponent<TMPTextSpriteHelper>().Parse();
		textSpecialContent.text = specialContent.ColorReplace();
		textSpecialContent.GetComponent<TMPTextSpriteHelper>().Parse();
		textSpecialContent.GetComponent<ContentSizeFitter>().SetLayoutVertical();
		LayoutRebuilder.ForceRebuildLayoutImmediate(base.CGet<RectTransform>("Content"));
		SingletonObject.getInstance<YieldHelper>().StartYield(this.Wait(textSpecialContent));
	}

	// Token: 0x06002A5C RID: 10844 RVA: 0x00144314 File Offset: 0x00142514
	public override void Refresh(ArgumentBox argBox)
	{
		this.Init(argBox);
	}

	// Token: 0x06002A5D RID: 10845 RVA: 0x0014431F File Offset: 0x0014251F
	private IEnumerator Wait(TextMeshProUGUI textSpecialContent)
	{
		yield return null;
		textSpecialContent.GetComponent<ContentSizeFitter>().SetLayoutVertical();
		LayoutRebuilder.ForceRebuildLayoutImmediate(base.CGet<RectTransform>("Content"));
		yield break;
	}
}
