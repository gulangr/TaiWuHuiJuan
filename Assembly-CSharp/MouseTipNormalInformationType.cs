using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork;
using TMPro;
using UnityEngine;

// Token: 0x020002C4 RID: 708
public class MouseTipNormalInformationType : MouseTipBase
{
	// Token: 0x06002AEF RID: 10991 RVA: 0x0014AF30 File Offset: 0x00149130
	protected override void Init(ArgumentBox argsBox)
	{
		sbyte informationTypeId;
		argsBox.Get("InformationType", out informationTypeId);
		InformationTypeItem informationType = InformationType.Instance.GetItem(informationTypeId);
		bool flag = informationType == null;
		if (!flag)
		{
			TextMeshProUGUI labelA2 = base.CGet<TextMeshProUGUI>("DescA2");
			labelA2.transform.parent.gameObject.SetActive(informationType.TemplateId == 2);
			labelA2.text = LocalStringManager.Get(LanguageKey.LK_NormalInformation_Tips_DescEffectWay_LifeSkill);
			bool flag2 = !PoolManager.HasData(this._poolObjectKey);
			if (flag2)
			{
				PoolManager.SetSrcObject(this._poolObjectKey, this.DescATemplateObject);
			}
			this.DescATemplateObject.gameObject.SetActive(false);
			this.ClearInstances();
			RectTransform view = this.DescATemplateObject.transform.parent.GetComponent<RectTransform>();
			foreach (string descALine in informationType.DescGain.Split("\n", StringSplitOptions.None))
			{
				Refers descA = PoolManager.GetObject<Refers>(this._poolObjectKey);
				Transform descATransform = descA.transform;
				GameObject descAObject = descA.gameObject;
				string[] info = descALine.Split("：", StringSplitOptions.None);
				descATransform.SetParent(view, true);
				descATransform.localScale = Vector3.one;
				descA.CGet<TextMeshProUGUI>("Title").text = ((info.Length != 0) ? (info[0] + ":").ColorReplace() : string.Empty);
				descA.CGet<TextMeshProUGUI>("Text").text = ((info.Length > 1) ? info[1].ColorReplace() : string.Empty);
				descAObject.SetActive(true);
				this._generatedNodes.Add(descAObject);
			}
			YieldHelper yieldHelper = SingletonObject.getInstance<YieldHelper>();
			bool flag3 = this._layoutCoroutine != null;
			if (flag3)
			{
				yieldHelper.StopCoroutine(this._layoutCoroutine);
			}
			yieldHelper.StartCoroutine(this._layoutCoroutine = base.RectTransform.LayoutRebuildRoutine());
			base.CGet<TextMeshProUGUI>("Name").text = informationType.Name;
			base.CGet<TextMeshProUGUI>("Desc").text = informationType.Desc;
			base.CGet<TextMeshProUGUI>("DescB").text = LocalStringManager.Get(LanguageKey.LK_NormalInformation_Tips_DescEffect_2);
			base.CGet<TextMeshProUGUI>("DescB2").text = (from line in informationType.DescEffect.Split("\n", StringSplitOptions.None)
			select "-".SetColor("grey") + line + "\n").Aggregate(new Func<string, string, string>(string.Concat)).ColorReplace();
			base.CGet<TextMeshProUGUI>("DescC").text = informationType.DescEffectWay;
		}
	}

	// Token: 0x06002AF0 RID: 10992 RVA: 0x0014B1E4 File Offset: 0x001493E4
	private void ClearInstances()
	{
		foreach (GameObject instance in this._generatedNodes)
		{
			PoolManager.Destroy(this._poolObjectKey, instance);
		}
		this._generatedNodes.Clear();
	}

	// Token: 0x06002AF1 RID: 10993 RVA: 0x0014B250 File Offset: 0x00149450
	private void OnDestroy()
	{
		this.ClearInstances();
		PoolManager.RemoveData(this._poolObjectKey);
	}

	// Token: 0x04001F03 RID: 7939
	public GameObject DescATemplateObject;

	// Token: 0x04001F04 RID: 7940
	private readonly string _poolObjectKey = "MouseTipNormalInformationType/DescATemplate";

	// Token: 0x04001F05 RID: 7941
	private readonly List<GameObject> _generatedNodes = new List<GameObject>();

	// Token: 0x04001F06 RID: 7942
	private IEnumerator _layoutCoroutine;
}
