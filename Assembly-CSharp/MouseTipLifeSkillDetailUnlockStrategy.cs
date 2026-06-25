using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork;
using Game.Views.Encyclopedia;
using UnityEngine;

// Token: 0x020002B3 RID: 691
public class MouseTipLifeSkillDetailUnlockStrategy : MouseTipBase
{
	// Token: 0x06002A9C RID: 10908 RVA: 0x00146620 File Offset: 0x00144820
	protected override void Init(ArgumentBox argsBox)
	{
		MouseTipLifeSkillDetailUnlockStrategy.<>c__DisplayClass6_0 CS$<>8__locals1 = new MouseTipLifeSkillDetailUnlockStrategy.<>c__DisplayClass6_0();
		CS$<>8__locals1.<>4__this = this;
		this.Element.ForceListenCommand = true;
		bool flag = !PoolManager.HasData("MouseTipLifeSkillDetailUnlockStrategy/BookTemplate");
		if (flag)
		{
			PoolManager.SetSrcObject("MouseTipLifeSkillDetailUnlockStrategy/BookTemplate", this.BookTemplate);
		}
		bool flag2 = !PoolManager.HasData("MouseTipLifeSkillDetailUnlockStrategy/PageTemplate");
		if (flag2)
		{
			PoolManager.SetSrcObject("MouseTipLifeSkillDetailUnlockStrategy/PageTemplate", this.PageTemplate);
		}
		this.ClearInstances();
		bool flag3 = !argsBox.Get<MouseTipLifeSkillDetailUnlockInformation.ProgressProvider>(MouseTipLifeSkillDetailUnlockInformation.ArgKeyProgressProvider, out CS$<>8__locals1.progressProvider) || !argsBox.Get<LifeSkillTypeItem>(MouseTipLifeSkillDetailUnlockInformation.ArgKeyLifeSkillType, out CS$<>8__locals1.typeConfig);
		if (!flag3)
		{
			this._coroutine = SingletonObject.getInstance<YieldHelper>().ReStartCoroutine(this._coroutine, CS$<>8__locals1.<Init>g__Routine|0());
		}
	}

	// Token: 0x06002A9D RID: 10909 RVA: 0x001466E0 File Offset: 0x001448E0
	private void ClearInstances()
	{
		foreach (Transform instance in from i in Enumerable.Range(0, this.BooksRoot.childCount)
		select this.BooksRoot.GetChild(i))
		{
			bool flag = instance == this.BookTemplate.transform || instance == this.PageTemplate.transform;
			if (!flag)
			{
				RectTransform pages = instance.GetComponent<Refers>().CGet<RectTransform>("Pages");
				IEnumerable<int> source = Enumerable.Range(0, pages.childCount);
				Func<int, Transform> selector;
				Func<int, Transform> <>9__1;
				if ((selector = <>9__1) == null)
				{
					selector = (<>9__1 = ((int i) => pages.GetChild(i)));
				}
				foreach (Transform page in source.Select(selector))
				{
					PoolManager.Destroy("MouseTipLifeSkillDetailUnlockStrategy/PageTemplate", page.gameObject);
				}
				PoolManager.Destroy("MouseTipLifeSkillDetailUnlockStrategy/BookTemplate", instance.gameObject);
			}
		}
	}

	// Token: 0x06002A9E RID: 10910 RVA: 0x0014682C File Offset: 0x00144A2C
	private void Update()
	{
		bool flag = CommonCommandKit.PrimaryInteraction.Check(this.Element, false, false, false, true, false);
		if (flag)
		{
			ViewEncyclopediaPanel.OpenLink(EncyclopediaTipLink.DefValue.DebateStrategy);
		}
	}

	// Token: 0x06002A9F RID: 10911 RVA: 0x0014685E File Offset: 0x00144A5E
	private void OnDestroy()
	{
		this.ClearInstances();
		PoolManager.RemoveData("MouseTipLifeSkillDetailUnlockStrategy/BookTemplate");
		PoolManager.RemoveData("MouseTipLifeSkillDetailUnlockStrategy/PageTemplate");
	}

	// Token: 0x04001ED5 RID: 7893
	public RectTransform BooksRoot;

	// Token: 0x04001ED6 RID: 7894
	public GameObject BookTemplate;

	// Token: 0x04001ED7 RID: 7895
	public GameObject PageTemplate;

	// Token: 0x04001ED8 RID: 7896
	private IEnumerator _coroutine;

	// Token: 0x04001ED9 RID: 7897
	private const string BookItemPrefabKey = "MouseTipLifeSkillDetailUnlockStrategy/BookTemplate";

	// Token: 0x04001EDA RID: 7898
	private const string PageItemPrefabKey = "MouseTipLifeSkillDetailUnlockStrategy/PageTemplate";
}
