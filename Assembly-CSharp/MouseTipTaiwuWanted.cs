using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FrameWork;
using GameData.Domains.Character.Display;
using GameData.Domains.Organization.Display;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000400 RID: 1024
public class MouseTipTaiwuWanted : MouseTipBase
{
	// Token: 0x17000638 RID: 1592
	// (get) Token: 0x06003D2B RID: 15659 RVA: 0x001ECAD3 File Offset: 0x001EACD3
	protected override bool CanStick
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06003D2C RID: 15660 RVA: 0x001ECAD8 File Offset: 0x001EACD8
	protected override void Init(ArgumentBox argsBox)
	{
		bool flag = !PoolManager.HasData("UI_MouseTipTaiwuWanted_CharTemplate");
		if (flag)
		{
			PoolManager.SetSrcObject("UI_MouseTipTaiwuWanted_CharTemplate", this.CharTemplate);
		}
		SettlementBountyDisplayData data;
		bool flag2 = !argsBox.Get<SettlementBountyDisplayData>("Data", out data);
		if (!flag2)
		{
			this.ClearInstances();
			YieldHelper yieldHelper = SingletonObject.getInstance<YieldHelper>();
			bool flag3 = this._coroutine != null;
			if (flag3)
			{
				yieldHelper.StopCoroutine(this._coroutine);
			}
			yieldHelper.StartCoroutine(this._coroutine = this.RefreshRoutine(data));
		}
	}

	// Token: 0x06003D2D RID: 15661 RVA: 0x001ECB60 File Offset: 0x001EAD60
	private IEnumerator RefreshRoutine(SettlementBountyDisplayData data)
	{
		MouseTipTaiwuWanted.<>c__DisplayClass10_0 CS$<>8__locals1 = new MouseTipTaiwuWanted.<>c__DisplayClass10_0();
		CS$<>8__locals1.data = data;
		CS$<>8__locals1.taiWuId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
		int contentCount = 0;
		foreach (int charId in SingletonObject.getInstance<CharacterMonitorModel>().GetTaiwuTeamCharIds())
		{
			NameRelatedData nameRelatedData = default(NameRelatedData);
			Refers instance = null;
			RectTransform view = null;
			Refers unit = null;
			bool flag = charId == CS$<>8__locals1.taiWuId;
			if (flag)
			{
				int index = 0;
				IEnumerable<int> source = from key in CS$<>8__locals1.data.BountyCharacterDisplayDataDict.Keys
				where key < 0
				select key;
				Func<int, CharacterDisplayDataForSettlementBounty> selector;
				if ((selector = CS$<>8__locals1.<>9__2) == null)
				{
					selector = (CS$<>8__locals1.<>9__2 = ((int key) => CS$<>8__locals1.data.BountyCharacterDisplayDataDict[key]));
				}
				foreach (CharacterDisplayDataForSettlementBounty charData3 in from charData in source.Select(selector)
				where ((charData != null) ? charData.SettlementBounty : null) != null
				select charData)
				{
					nameRelatedData = charData3.NameRelatedData;
					bool flag2 = null == instance;
					if (flag2)
					{
						instance = PoolManager.GetObject<Refers>("UI_MouseTipTaiwuWanted_CharTemplate");
						Transform instanceTransform = instance.gameObject.transform;
						instanceTransform.SetParent(base.transform, true);
						instanceTransform.localScale = Vector3.one;
						view = instance.CGet<RectTransform>("View");
						unit = instance.CGet<Refers>("Unit");
						instanceTransform = null;
					}
					Refers child = (index < view.childCount) ? view.GetChild(index).GetComponent<Refers>() : Object.Instantiate<GameObject>(unit.gameObject, view).GetComponent<Refers>();
					CS$<>8__locals1.<RefreshRoutine>g__RefreshBountyUnit|0(child, charData3, index);
					index++;
					contentCount++;
					child = null;
					charData3 = null;
				}
				IEnumerator<CharacterDisplayDataForSettlementBounty> enumerator2 = null;
				bool flag3 = null == view;
				if (flag3)
				{
					continue;
				}
				int num;
				for (int i = view.childCount - 1; i >= index; i = num - 1)
				{
					Transform child2 = view.GetChild(i);
					bool flag4 = child2.gameObject != unit.gameObject;
					if (flag4)
					{
						Object.Destroy(child2.gameObject);
					}
					child2 = null;
					num = i;
				}
			}
			else
			{
				CharacterDisplayDataForSettlementBounty charData2;
				bool flag5 = !CS$<>8__locals1.data.BountyCharacterDisplayDataDict.TryGetValue(charId, out charData2) || charData2.SettlementBounty == null;
				if (flag5)
				{
					continue;
				}
				nameRelatedData = charData2.NameRelatedData;
				instance = PoolManager.GetObject<Refers>("UI_MouseTipTaiwuWanted_CharTemplate");
				Transform instanceTransform2 = instance.gameObject.transform;
				instanceTransform2.SetParent(base.transform, true);
				instanceTransform2.localScale = Vector3.one;
				view = instance.CGet<RectTransform>("View");
				unit = instance.CGet<Refers>("Unit");
				int num;
				for (int j = view.childCount - 1; j >= 0; j = num - 1)
				{
					Transform child3 = view.GetChild(j);
					bool flag6 = child3.gameObject != unit.gameObject;
					if (flag6)
					{
						Object.Destroy(child3.gameObject);
					}
					child3 = null;
					num = j;
				}
				CS$<>8__locals1.<RefreshRoutine>g__RefreshBountyUnit|0(unit, charData2, 0);
				contentCount++;
				charData2 = null;
				instanceTransform2 = null;
			}
			this._charInstances.Add(instance);
			view.localScale = Vector3.zero;
			TextMeshProUGUI label = instance.CGet<TextMeshProUGUI>("Name");
			label.text = (NameCenter.GetMonasticTitleOrDisplayName(ref nameRelatedData, charId == CS$<>8__locals1.taiWuId, false) ?? "");
			LayoutRebuilder.ForceRebuildLayoutImmediate(view);
			LayoutRebuilder.MarkLayoutForRebuild(view);
			yield return new WaitForEndOfFrame();
			RectTransform targetRectTransform = instance.GetComponent<RectTransform>();
			Vector2 offsetMax = targetRectTransform.offsetMax;
			Vector2 offsetMin = targetRectTransform.offsetMin;
			offsetMax.y = label.GetComponent<RectTransform>().offsetMax.y;
			offsetMin.y = view.GetComponent<RectTransform>().offsetMin.y;
			targetRectTransform.offsetMax = offsetMax;
			targetRectTransform.offsetMin = offsetMin;
			yield return new WaitForEndOfFrame();
			view.localScale = Vector3.one;
			nameRelatedData = default(NameRelatedData);
			instance = null;
			view = null;
			unit = null;
			label = null;
			targetRectTransform = null;
			offsetMax = default(Vector2);
			offsetMin = default(Vector2);
		}
		List<int>.Enumerator enumerator = default(List<int>.Enumerator);
		yield return new WaitForEndOfFrame();
		this.RefreshAutoSizeLayout(contentCount);
		RectTransform selfRect = base.GetComponent<RectTransform>();
		LayoutRebuilder.ForceRebuildLayoutImmediate(selfRect);
		LayoutRebuilder.MarkLayoutForRebuild(selfRect);
		this._coroutine = null;
		yield break;
		yield break;
	}

	// Token: 0x06003D2E RID: 15662 RVA: 0x001ECB78 File Offset: 0x001EAD78
	private void RefreshAutoSizeLayout(int contentCount)
	{
		bool flag = null == this.autoSizeTrans;
		if (!flag)
		{
			bool isSingle = contentCount <= 1;
			this.autoSizeTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, isSingle ? this.widthSingle : this.widthMulti);
			foreach (Refers instance in this._charInstances)
			{
				instance.CGet<GameObject>("Line").SetActive(!isSingle);
			}
		}
	}

	// Token: 0x06003D2F RID: 15663 RVA: 0x001ECC14 File Offset: 0x001EAE14
	private void ClearInstances()
	{
		foreach (Refers instance in this._charInstances)
		{
			PoolManager.Destroy("UI_MouseTipTaiwuWanted_CharTemplate", instance.gameObject);
		}
		this._charInstances.Clear();
	}

	// Token: 0x06003D30 RID: 15664 RVA: 0x001ECC84 File Offset: 0x001EAE84
	private void OnDestroy()
	{
		this.ClearInstances();
		PoolManager.RemoveData("UI_MouseTipTaiwuWanted_CharTemplate");
	}

	// Token: 0x04002BF8 RID: 11256
	public GameObject CharTemplate;

	// Token: 0x04002BF9 RID: 11257
	public RectTransform autoSizeTrans;

	// Token: 0x04002BFA RID: 11258
	public float widthMulti = 1076f;

	// Token: 0x04002BFB RID: 11259
	public float widthSingle = 548f;

	// Token: 0x04002BFC RID: 11260
	private const string CharItemPrefabKey = "UI_MouseTipTaiwuWanted_CharTemplate";

	// Token: 0x04002BFD RID: 11261
	private readonly List<Refers> _charInstances = new List<Refers>();

	// Token: 0x04002BFE RID: 11262
	private IEnumerator _coroutine;
}
