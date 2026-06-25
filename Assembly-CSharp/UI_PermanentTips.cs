using System;
using System.Collections.Generic;
using FrameWork;
using UnityEngine;

// Token: 0x020002EF RID: 751
public class UI_PermanentTips : UIBase
{
	// Token: 0x06002C06 RID: 11270 RVA: 0x00158A06 File Offset: 0x00156C06
	public override void OnInit(ArgumentBox argsBox)
	{
	}

	// Token: 0x06002C07 RID: 11271 RVA: 0x00158A09 File Offset: 0x00156C09
	private void Awake()
	{
		PoolManager.SetSrcObject("UI_PermanentTipsObject", base.CGet<Refers>("TipObjectPrefab").gameObject);
		this._tipsObjectList = new List<MouseTipBase>();
	}

	// Token: 0x06002C08 RID: 11272 RVA: 0x00158A32 File Offset: 0x00156C32
	private void OnDestroy()
	{
		PoolManager.RemoveData("UI_PermanentTipsObject");
	}

	// Token: 0x06002C09 RID: 11273 RVA: 0x00158A40 File Offset: 0x00156C40
	private void Update()
	{
		bool flag = CommonCommandKit.ClearStickTips.Check(this.Element, false, false, false, true, false);
		if (flag)
		{
			this.ClearAllPermanentTips();
		}
	}

	// Token: 0x06002C0A RID: 11274 RVA: 0x00158A6E File Offset: 0x00156C6E
	private void RemoveTip(MouseTipBase tipBase)
	{
		PoolManager.Destroy("UI_PermanentTipsObject", tipBase.transform.parent.gameObject);
		Object.Destroy(tipBase.gameObject);
	}

	// Token: 0x06002C0B RID: 11275 RVA: 0x00158A98 File Offset: 0x00156C98
	public void AddPermanentTips(MouseTipBase tipBase)
	{
		bool flag = tipBase == null || !tipBase.Element.Ready || tipBase.transform.localScale.x < 1f;
		if (!flag)
		{
			RectTransform tipsTransform = tipBase.GetComponent<RectTransform>();
			Refers objRefers = PoolManager.GetObject<Refers>("UI_PermanentTipsObject");
			RectTransform referTransform = objRefers.GetComponent<RectTransform>();
			PointerTrigger ptrTrigger = objRefers.GetComponent<PointerTrigger>();
			UIRectDragMove dragMove = objRefers.GetComponent<UIRectDragMove>();
			CButtonObsolete closeBtn = objRefers.CGet<CButtonObsolete>("Close");
			CImage outlineImage = objRefers.CGet<CImage>("OutLineImage");
			tipBase.Element.UnMonitorData();
			tipBase.Element.UiBase = null;
			tipBase.Element.Destroy();
			tipBase.Element = null;
			tipBase.HasStick = true;
			tipBase.OnSticked();
			referTransform.SetParent(base.CGet<RectTransform>("TipsArea"), false);
			referTransform.pivot = tipsTransform.pivot;
			referTransform.position = tipsTransform.position;
			referTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, tipsTransform.sizeDelta.x + 10f);
			referTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, tipsTransform.sizeDelta.y + 10f);
			SingletonObject.getInstance<YieldHelper>().DelayFrameDo(1U, delegate
			{
				referTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, tipsTransform.sizeDelta.x + 10f);
				referTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, tipsTransform.sizeDelta.y + 10f);
			});
			tipsTransform.pivot = Vector2.one * 0.5f;
			tipsTransform.anchorMin = tipsTransform.pivot;
			tipsTransform.anchorMax = tipsTransform.pivot;
			tipsTransform.SetParent(referTransform, false);
			tipsTransform.SetSiblingIndex(1);
			tipsTransform.anchoredPosition = Vector2.zero;
			tipsTransform.gameObject.SetActive(true);
			this._tipsObjectList.Add(tipBase);
			objRefers.GetComponent<CButtonObsolete>().ClearAndAddListener(delegate
			{
				referTransform.transform.SetAsLastSibling();
			});
			closeBtn.ClearAndAddListener(delegate
			{
				this._tipsObjectList.Remove(tipBase);
				this.RemoveTip(tipBase);
			});
			closeBtn.gameObject.SetActive(false);
			outlineImage.gameObject.SetActive(false);
			ptrTrigger.EnterEvent.RemoveAllListeners();
			ptrTrigger.EnterEvent.AddListener(delegate()
			{
				closeBtn.gameObject.SetActive(true);
				outlineImage.gameObject.SetActive(true);
			});
			ptrTrigger.ExitEvent.RemoveAllListeners();
			ptrTrigger.ExitEvent.AddListener(delegate()
			{
				closeBtn.gameObject.SetActive(false);
				outlineImage.gameObject.SetActive(false);
			});
			dragMove.BeginDragCallback = delegate()
			{
				dragMove.GetComponent<CanvasGroup>().blocksRaycasts = true;
			};
		}
	}

	// Token: 0x06002C0C RID: 11276 RVA: 0x00158DAB File Offset: 0x00156FAB
	public void ClearAllPermanentTips()
	{
		this._tipsObjectList.ForEach(new Action<MouseTipBase>(this.RemoveTip));
		this._tipsObjectList.Clear();
	}

	// Token: 0x04001FEF RID: 8175
	private const string TipsObjectKey = "UI_PermanentTipsObject";

	// Token: 0x04001FF0 RID: 8176
	private const int EdgeExpansionSize = 10;

	// Token: 0x04001FF1 RID: 8177
	private List<MouseTipBase> _tipsObjectList;
}
