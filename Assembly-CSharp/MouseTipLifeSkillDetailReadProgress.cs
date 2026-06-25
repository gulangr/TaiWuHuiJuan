using System;
using System.Collections;
using System.Linq;
using Config;
using FrameWork;
using TMPro;
using UnityEngine;

// Token: 0x020002B1 RID: 689
public class MouseTipLifeSkillDetailReadProgress : MouseTipBase
{
	// Token: 0x06002A8E RID: 10894 RVA: 0x00146210 File Offset: 0x00144410
	protected override void Init(ArgumentBox argsBox)
	{
		MouseTipLifeSkillDetailReadProgress.<>c__DisplayClass8_0 CS$<>8__locals1 = new MouseTipLifeSkillDetailReadProgress.<>c__DisplayClass8_0();
		CS$<>8__locals1.<>4__this = this;
		bool flag = !PoolManager.HasData("UI_MouseTipLifeSkillDetailReadProgress/PageTemplate");
		if (flag)
		{
			PoolManager.SetSrcObject("UI_MouseTipLifeSkillDetailReadProgress/PageTemplate", this.PageTemplate);
		}
		this.ClearInstances();
		SkillBookItem bookConfig;
		bool flag2 = !argsBox.Get<sbyte[]>(MouseTipLifeSkillDetailReadProgress.ArgKeyReadProgresses, out CS$<>8__locals1.readProgresses) || !argsBox.Get<SkillBookItem>(MouseTipLifeSkillDetailReadProgress.ArgKeyBookConfig, out bookConfig);
		if (!flag2)
		{
			this.Title.text = bookConfig.Name;
			this.Desc.text = bookConfig.Desc;
			this._coroutine = SingletonObject.getInstance<YieldHelper>().ReStartCoroutine(this._coroutine, CS$<>8__locals1.<Init>g__Routine|0());
		}
	}

	// Token: 0x06002A8F RID: 10895 RVA: 0x001462C4 File Offset: 0x001444C4
	private void ClearInstances()
	{
		foreach (Transform instance in from i in Enumerable.Range(0, this.PagesRoot.childCount)
		select this.PagesRoot.GetChild(i))
		{
			bool flag = instance != this.PageTemplate.transform;
			if (flag)
			{
				PoolManager.Destroy("UI_MouseTipLifeSkillDetailReadProgress/PageTemplate", instance.gameObject);
			}
		}
	}

	// Token: 0x06002A90 RID: 10896 RVA: 0x00146350 File Offset: 0x00144550
	private void OnDestroy()
	{
		this.ClearInstances();
		PoolManager.RemoveData("UI_MouseTipLifeSkillDetailReadProgress/PageTemplate");
	}

	// Token: 0x04001EC4 RID: 7876
	public TextMeshProUGUI Title;

	// Token: 0x04001EC5 RID: 7877
	public TextMeshProUGUI Desc;

	// Token: 0x04001EC6 RID: 7878
	public RectTransform PagesRoot;

	// Token: 0x04001EC7 RID: 7879
	public GameObject PageTemplate;

	// Token: 0x04001EC8 RID: 7880
	private IEnumerator _coroutine;

	// Token: 0x04001EC9 RID: 7881
	public static readonly string ArgKeyReadProgresses = "ReadProgresses";

	// Token: 0x04001ECA RID: 7882
	public static readonly string ArgKeyBookConfig = "BookConfig";

	// Token: 0x04001ECB RID: 7883
	private const string PageItemPrefabKey = "UI_MouseTipLifeSkillDetailReadProgress/PageTemplate";
}
