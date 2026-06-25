using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Config;
using DG.Tweening;
using Game.Views.Legacy.WorldMap;
using GameData.Domains.Extra;
using GameData.Domains.Map;
using GameData.Serializer;
using GameData.Utilities;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x020003F0 RID: 1008
public class MapElementMerchant : MapElementBase
{
	// Token: 0x06003C96 RID: 15510 RVA: 0x001E8604 File Offset: 0x001E6804
	public static bool CheckMaybeExist(Location location)
	{
		bool flag = MapElementBase.MapModel.ViewMode == WorldMapModel.EViewMode.Info && MapElementBase.MapModel.CurrentAreaId == location.AreaId;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			bool flag2 = MapElementBase.MapModel.ShowingAreaId != location.AreaId;
			if (flag2)
			{
				result = false;
			}
			else
			{
				MapBlockData block = MapElementBase.MapModel.GetBlockData(location);
				bool flag3 = block == null;
				if (flag3)
				{
					result = false;
				}
				else
				{
					bool flag4 = !block.Visible || MapElementBase.IsHideCharacterSet;
					if (flag4)
					{
						result = false;
					}
					else
					{
						bool flag5 = block.CharacterSet == null || block.CharacterSet.Count == 0;
						result = !flag5;
					}
				}
			}
		}
		return result;
	}

	// Token: 0x17000629 RID: 1577
	// (get) Token: 0x06003C97 RID: 15511 RVA: 0x001E86B5 File Offset: 0x001E68B5
	protected override bool AutoSetActive
	{
		get
		{
			return false;
		}
	}

	// Token: 0x1700062A RID: 1578
	// (get) Token: 0x06003C98 RID: 15512 RVA: 0x001E86B8 File Offset: 0x001E68B8
	public override EMapLayer Layer
	{
		get
		{
			return EMapLayer.Merchant;
		}
	}

	// Token: 0x06003C99 RID: 15513 RVA: 0x001E86BB File Offset: 0x001E68BB
	public override void Scale(float wheel)
	{
		base.ScaleReverse(wheel);
	}

	// Token: 0x06003C9A RID: 15514 RVA: 0x001E86C8 File Offset: 0x001E68C8
	protected override void OnCreate()
	{
		PointerTrigger pointerTrigger = this.layoutRectTrans.GetComponent<PointerTrigger>();
		PointerTrigger pointerTrigger2 = pointerTrigger;
		if (pointerTrigger2.EnterEvent == null)
		{
			pointerTrigger2.EnterEvent = new UnityEvent();
		}
		pointerTrigger.EnterEvent.RemoveAllListeners();
		pointerTrigger.EnterEvent.AddListener(new UnityAction(this.OnPointerEnter));
		pointerTrigger2 = pointerTrigger;
		if (pointerTrigger2.ExitEvent == null)
		{
			pointerTrigger2.ExitEvent = new UnityEvent();
		}
		pointerTrigger.ExitEvent.RemoveAllListeners();
		pointerTrigger.ExitEvent.AddListener(new UnityAction(this.OnPointerExit));
		this.Init();
	}

	// Token: 0x06003C9B RID: 15515 RVA: 0x001E875B File Offset: 0x001E695B
	protected override void OnRefresh()
	{
		this.Dispatcher.ClearAsyncMethodCalls();
		ExtraDomainMethod.AsyncCall.GetBlockMerchantTypes(this.Dispatcher, base.BlockLocation, new AsyncMethodCallbackDelegate(this.HandlerGetBlockMerchantTypes));
	}

	// Token: 0x06003C9C RID: 15516 RVA: 0x001E8788 File Offset: 0x001E6988
	protected override void OnCollect()
	{
	}

	// Token: 0x06003C9D RID: 15517 RVA: 0x001E878C File Offset: 0x001E698C
	private void OnEnable()
	{
		bool flag = null != this.layoutRectTrans;
		if (flag)
		{
			this.ResetSize();
		}
	}

	// Token: 0x06003C9E RID: 15518 RVA: 0x001E87B4 File Offset: 0x001E69B4
	private void OnDisable()
	{
		base.StopAllCoroutines();
		bool flag = !this.layoutRectTrans;
		if (!flag)
		{
			foreach (MapElementMerchantTypeItem typeItem in this.merchantTypeItemArray)
			{
				typeItem.CanvasGroup.DOKill(true);
				typeItem.CanvasGroup.alpha = 0f;
			}
			this.ResetSize();
		}
	}

	// Token: 0x06003C9F RID: 15519 RVA: 0x001E8820 File Offset: 0x001E6A20
	private void HandlerGetBlockMerchantTypes(int offset, RawDataPool pool)
	{
		List<sbyte> typeSet = null;
		Serializer.Deserialize(pool, offset, ref typeSet);
		this.Refresh(typeSet, false);
	}

	// Token: 0x06003CA0 RID: 15520 RVA: 0x001E8844 File Offset: 0x001E6A44
	public void Init()
	{
		for (int i = 0; i < this.merchantTypeItemArray.Length; i++)
		{
			sbyte curType = (sbyte)i;
			MapElementMerchantTypeItem typeItem = this.merchantTypeItemArray[i];
			typeItem.Init((int)curType);
		}
	}

	// Token: 0x06003CA1 RID: 15521 RVA: 0x001E8880 File Offset: 0x001E6A80
	public void Refresh(List<sbyte> typeSet, bool isPreview)
	{
		bool shouldActive = typeSet != null && typeSet.Count > 0;
		bool flag = base.gameObject.activeSelf != shouldActive;
		if (flag)
		{
			base.gameObject.SetActive(shouldActive);
		}
		bool flag2 = !shouldActive;
		if (!flag2)
		{
			for (int i = 0; i < this.merchantTypeItemArray.Length; i++)
			{
				sbyte curType = (sbyte)i;
				MapElementMerchantTypeItem typeItem = this.merchantTypeItemArray[i];
				short key = MapElementDisplayRuleItem.Instance.First((MapElementDisplayRuleItemItem item) => item.Group == 2 && item.MerchantType == curType).TemplateId;
				bool settingState = MapElementBase.GlobalSettings.GetMapElementDisplayRuleItemState(key, true);
				bool isShow = typeSet.Contains(curType) && (isPreview || settingState);
				typeItem.gameObject.SetActive(isShow);
			}
		}
	}

	// Token: 0x06003CA2 RID: 15522 RVA: 0x001E895C File Offset: 0x001E6B5C
	private void OnPointerEnter()
	{
		base.StopAllCoroutines();
		MapElementMerchantTypeItem[] array = this.merchantTypeItemArray;
		for (int i = 0; i < array.Length; i++)
		{
			MapElementMerchantTypeItem typeItem = array[i];
			bool flag = !typeItem.gameObject.activeSelf;
			if (!flag)
			{
				typeItem.CanvasGroup.DOKill(false);
				this.DelayAnim(0.2f, delegate
				{
					typeItem.CanvasGroup.DOFade(1f, 0.2f);
				});
			}
		}
		this.CanvasSortingLayerOverrideAdd();
	}

	// Token: 0x06003CA3 RID: 15523 RVA: 0x001E89E4 File Offset: 0x001E6BE4
	private void OnPointerExit()
	{
		MapElementMerchantTypeItem[] array = this.merchantTypeItemArray;
		for (int i = 0; i < array.Length; i++)
		{
			MapElementMerchantTypeItem typeItem = array[i];
			bool flag = !typeItem.gameObject.activeSelf;
			if (!flag)
			{
				typeItem.CanvasGroup.DOKill(true);
				this.DelayAnim(1f, delegate
				{
					typeItem.CanvasGroup.DOFade(0f, 0.2f);
				});
			}
		}
		this.CanvasSortingLayerOverrideRemove();
	}

	// Token: 0x06003CA4 RID: 15524 RVA: 0x001E8A64 File Offset: 0x001E6C64
	private void DelayAnim(float delay, Action action)
	{
		bool flag = base.gameObject && base.gameObject.activeInHierarchy;
		if (flag)
		{
			base.StartCoroutine(this.CorDelayAnim(delay, action));
		}
	}

	// Token: 0x06003CA5 RID: 15525 RVA: 0x001E8AA0 File Offset: 0x001E6CA0
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

	// Token: 0x06003CA6 RID: 15526 RVA: 0x001E8ABD File Offset: 0x001E6CBD
	private void ResetSize()
	{
		this.CanvasSortingLayerOverrideRemove();
	}

	// Token: 0x06003CA7 RID: 15527 RVA: 0x001E8AC8 File Offset: 0x001E6CC8
	private void CanvasSortingLayerOverrideAdd()
	{
		Canvas canvas;
		bool flag = !base.gameObject.TryGetComponent<Canvas>(out canvas);
		if (flag)
		{
			canvas = base.gameObject.AddComponent<Canvas>();
		}
		GraphicRaycaster graphicRaycaster;
		bool flag2 = !base.gameObject.TryGetComponent<GraphicRaycaster>(out graphicRaycaster);
		if (flag2)
		{
			graphicRaycaster = base.gameObject.AddComponent<GraphicRaycaster>();
		}
		canvas.overrideSorting = true;
		canvas.sortingLayerName = "UI";
		canvas.sortingOrder = this.CanvasSortingLayerOverrideWhenOpen;
		canvas.additionalShaderChannels = (AdditionalCanvasShaderChannels.TexCoord1 | AdditionalCanvasShaderChannels.TexCoord2 | AdditionalCanvasShaderChannels.TexCoord3 | AdditionalCanvasShaderChannels.Normal | AdditionalCanvasShaderChannels.Tangent);
	}

	// Token: 0x06003CA8 RID: 15528 RVA: 0x001E8B40 File Offset: 0x001E6D40
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

	// Token: 0x04002B83 RID: 11139
	[SerializeField]
	private RectTransform layoutRectTrans;

	// Token: 0x04002B84 RID: 11140
	[SerializeField]
	private MapElementMerchantTypeItem[] merchantTypeItemArray;

	// Token: 0x04002B85 RID: 11141
	public int CanvasSortingLayerOverrideWhenOpen;
}
