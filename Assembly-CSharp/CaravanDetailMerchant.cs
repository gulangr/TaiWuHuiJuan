using System;
using System.Collections;
using Config;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x02000319 RID: 793
public class CaravanDetailMerchant : UIFixedScaleChildrenManual
{
	// Token: 0x17000515 RID: 1301
	// (get) Token: 0x06002E7D RID: 11901 RVA: 0x0016EFE8 File Offset: 0x0016D1E8
	private float iconWidth
	{
		get
		{
			return this.layoutRectTrans.GetChild(0).GetComponent<RectTransform>().rect.width;
		}
	}

	// Token: 0x06002E7E RID: 11902 RVA: 0x0016F014 File Offset: 0x0016D214
	protected override void Awake()
	{
		base.Awake();
		this.layoutRectTrans = base.CGet<RectTransform>("BG");
		this.subBgRect = base.CGet<RectTransform>("BgShort");
		PointerTrigger pointerTrigger = this.layoutRectTrans.GetComponent<PointerTrigger>();
		PointerTrigger pointerTriggerShort = this.subBgRect.GetComponent<PointerTrigger>();
		pointerTrigger.EnterEvent.RemoveAllListeners();
		pointerTrigger.EnterEvent.AddListener(new UnityAction(this.OnPointerEnter));
		pointerTrigger.ExitEvent.RemoveAllListeners();
		pointerTrigger.ExitEvent.AddListener(new UnityAction(this.OnPointerExit));
	}

	// Token: 0x06002E7F RID: 11903 RVA: 0x0016F0AC File Offset: 0x0016D2AC
	private void OnEnable()
	{
		bool flag = null != this.layoutRectTrans;
		if (flag)
		{
			this.ResetSize();
		}
	}

	// Token: 0x06002E80 RID: 11904 RVA: 0x0016F0D4 File Offset: 0x0016D2D4
	private void OnDisable()
	{
		base.StopAllCoroutines();
		bool flag = !this.layoutRectTrans;
		if (!flag)
		{
			TextMeshProUGUI typeName = base.CGet<TextMeshProUGUI>("Name");
			bool flag2 = null == typeName;
			if (!flag2)
			{
				CanvasGroup typeNameCanvasGroup = typeName.GetComponent<CanvasGroup>();
				typeNameCanvasGroup.DOKill(true);
				typeNameCanvasGroup.alpha = 0f;
				this.ResetSize();
			}
		}
	}

	// Token: 0x06002E81 RID: 11905 RVA: 0x0016F138 File Offset: 0x0016D338
	public void SetMerchantType(sbyte typeId)
	{
		MerchantTypeItem config = MerchantType.Instance.GetItem(typeId);
		base.CGet<TextMeshProUGUI>("Name").text = config.Name;
		CImage iconComp = base.CGet<CImage>("icon");
		iconComp.SetSprite(config.Icon, false, null);
	}

	// Token: 0x06002E82 RID: 11906 RVA: 0x0016F184 File Offset: 0x0016D384
	public void OnPointerEnter()
	{
		base.StopAllCoroutines();
		float maxNameWidth = 0f;
		TextMeshProUGUI typeName = base.CGet<TextMeshProUGUI>("Name");
		typeName.gameObject.SetActive(true);
		CanvasGroup typeNameCanvasGroup = typeName.GetComponent<CanvasGroup>();
		typeNameCanvasGroup.DOKill(false);
		this.DelayAnim(0.2f, delegate
		{
			typeNameCanvasGroup.DOFade(1f, 0.2f);
		});
		RectTransform typeNameRectTrans = typeName.GetComponent<RectTransform>();
		LayoutRebuilder.ForceRebuildLayoutImmediate(typeNameRectTrans);
		float width = typeNameRectTrans.rect.width;
		bool flag = width > maxNameWidth;
		if (flag)
		{
			maxNameWidth = width;
		}
		Vector2 size = this.layoutRectTrans.sizeDelta;
		size.x = maxNameWidth + this.iconWidth;
		this.layoutRectTrans.DOKill(false);
		this.layoutRectTrans.DOSizeDelta(size, 0.2f, false);
		Vector2 sizeShort = new Vector2(size.x - 30f, size.y);
		this.subBgRect.DOKill(false);
		this.subBgRect.DOSizeDelta(sizeShort, 0.2f, false);
		Action actionPointerEnter = this.ActionPointerEnter;
		if (actionPointerEnter != null)
		{
			actionPointerEnter();
		}
	}

	// Token: 0x06002E83 RID: 11907 RVA: 0x0016F2A8 File Offset: 0x0016D4A8
	public void OnPointerExit()
	{
		Debug.Log("test merchant OnPointerExit");
		TextMeshProUGUI typeName = base.CGet<TextMeshProUGUI>("Name");
		bool flag = null == typeName;
		if (!flag)
		{
			CanvasGroup typeNameCanvasGroup = typeName.GetComponent<CanvasGroup>();
			typeNameCanvasGroup.DOKill(true);
			this.DelayAnim(1f, delegate
			{
				typeNameCanvasGroup.DOFade(0f, 0.2f);
			});
			TweenCallback <>9__2;
			this.DelayAnim(1.2f, delegate
			{
				Vector2 size = this.layoutRectTrans.sizeDelta;
				size.x = this.iconWidth;
				this.layoutRectTrans.DOKill(true);
				TweenerCore<Vector2, Vector2, VectorOptions> t = this.layoutRectTrans.DOSizeDelta(size, 0.2f, false);
				TweenCallback action;
				if ((action = <>9__2) == null)
				{
					action = (<>9__2 = delegate()
					{
						this.CanvasSortingLayerOverrideRemove();
					});
				}
				t.OnComplete(action);
				Vector2 sizeShort = new Vector2(size.x - 30f, size.y);
				this.subBgRect.DOKill(false);
				this.subBgRect.DOSizeDelta(sizeShort, 0.2f, false);
			});
			Action actionPointerExit = this.ActionPointerExit;
			if (actionPointerExit != null)
			{
				actionPointerExit();
			}
		}
	}

	// Token: 0x06002E84 RID: 11908 RVA: 0x0016F344 File Offset: 0x0016D544
	private void DelayAnim(float delay, Action action)
	{
		bool flag = base.gameObject && base.gameObject.activeInHierarchy;
		if (flag)
		{
			base.StartCoroutine(this.CorDelayAnim(delay, action));
		}
	}

	// Token: 0x06002E85 RID: 11909 RVA: 0x0016F380 File Offset: 0x0016D580
	private IEnumerator CorDelayAnim(float delay, Action action)
	{
		yield return new WaitForSeconds(delay);
		bool flag = !base.gameObject;
		if (flag)
		{
			yield break;
		}
		action();
		yield break;
	}

	// Token: 0x06002E86 RID: 11910 RVA: 0x0016F3A0 File Offset: 0x0016D5A0
	private void ResetSize()
	{
		this.layoutRectTrans.sizeDelta = this.layoutRectTrans.sizeDelta.SetX(this.iconWidth);
		this.subBgRect.sizeDelta = this.subBgRect.sizeDelta.SetX(this.iconWidth - 30f);
		this.CanvasSortingLayerOverrideRemove();
	}

	// Token: 0x06002E87 RID: 11911 RVA: 0x0016F400 File Offset: 0x0016D600
	private void CanvasSortingLayerOverrideRemove()
	{
		GraphicRaycaster graphicRaycaster;
		bool flag = base.gameObject.TryGetComponent<GraphicRaycaster>(out graphicRaycaster);
		if (flag)
		{
			Object.Destroy(graphicRaycaster);
		}
		Canvas canvas;
		bool flag2 = base.gameObject.TryGetComponent<Canvas>(out canvas);
		if (flag2)
		{
			Object.Destroy(canvas);
		}
	}

	// Token: 0x040021BC RID: 8636
	private RectTransform layoutRectTrans;

	// Token: 0x040021BD RID: 8637
	private RectTransform subBgRect;

	// Token: 0x040021BE RID: 8638
	private const string _IconBasePath = "RemakeResources\\UIGraphics4.0\\Common";

	// Token: 0x040021BF RID: 8639
	public Action ActionPointerEnter;

	// Token: 0x040021C0 RID: 8640
	public Action ActionPointerExit;

	// Token: 0x040021C1 RID: 8641
	public int CanvasSortingLayerOverrideWhenOpen;
}
