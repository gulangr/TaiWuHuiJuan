using System;
using DG.Tweening;
using FrameWork.UISystem.UIElements;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000162 RID: 354
[DisallowMultipleComponent]
public class CatchCricketPlace : MonoBehaviour
{
	// Token: 0x17000226 RID: 550
	// (get) Token: 0x0600136B RID: 4971 RVA: 0x00077344 File Offset: 0x00075544
	public RectTransform RectTransform
	{
		get
		{
			return (this._rectTransform != null) ? this._rectTransform : (this._rectTransform = (RectTransform)base.transform);
		}
	}

	// Token: 0x17000227 RID: 551
	// (get) Token: 0x0600136C RID: 4972 RVA: 0x0007737B File Offset: 0x0007557B
	public CImage PlaceImg
	{
		get
		{
			return this.placeImg;
		}
	}

	// Token: 0x17000228 RID: 552
	// (get) Token: 0x0600136D RID: 4973 RVA: 0x00077383 File Offset: 0x00075583
	public CImage BlockImg
	{
		get
		{
			return this.blockImg;
		}
	}

	// Token: 0x17000229 RID: 553
	// (get) Token: 0x0600136E RID: 4974 RVA: 0x0007738B File Offset: 0x0007558B
	public RectTransform CricketSingImage
	{
		get
		{
			return this.cricketSingImage;
		}
	}

	// Token: 0x1700022A RID: 554
	// (get) Token: 0x0600136F RID: 4975 RVA: 0x00077393 File Offset: 0x00075593
	public CImage HoverImg
	{
		get
		{
			return this.hoverImg;
		}
	}

	// Token: 0x1700022B RID: 555
	// (get) Token: 0x06001370 RID: 4976 RVA: 0x0007739B File Offset: 0x0007559B
	public AudioSource PlaceAudioSource
	{
		get
		{
			return this.placeAudioSource;
		}
	}

	// Token: 0x1700022C RID: 556
	// (get) Token: 0x06001371 RID: 4977 RVA: 0x000773A3 File Offset: 0x000755A3
	// (set) Token: 0x06001372 RID: 4978 RVA: 0x000773AB File Offset: 0x000755AB
	public bool IsShaking { get; set; }

	// Token: 0x06001373 RID: 4979 RVA: 0x000773B4 File Offset: 0x000755B4
	public void Init(int index, Action<int> onClick, Action<CatchCricketPlace> onPointerEnter, Action<CatchCricketPlace> onPointerExit)
	{
		this.button.ClearAndAddListener(delegate
		{
			onClick(index);
		});
		PointerTrigger pointerTrigger = this.pointerTrigger;
		if (pointerTrigger.EnterEvent == null)
		{
			pointerTrigger.EnterEvent = new UnityEvent();
		}
		pointerTrigger = this.pointerTrigger;
		if (pointerTrigger.ExitEvent == null)
		{
			pointerTrigger.ExitEvent = new UnityEvent();
		}
		this.pointerTrigger.EnterEvent.RemoveAllListeners();
		this.pointerTrigger.ExitEvent.RemoveAllListeners();
		this.pointerTrigger.EnterEvent.AddListener(delegate()
		{
			Action<CatchCricketPlace> onPointerEnter2 = onPointerEnter;
			if (onPointerEnter2 != null)
			{
				onPointerEnter2(this);
			}
		});
		this.pointerTrigger.ExitEvent.AddListener(delegate()
		{
			Action<CatchCricketPlace> onPointerExit2 = onPointerExit;
			if (onPointerExit2 != null)
			{
				onPointerExit2(this);
			}
		});
	}

	// Token: 0x06001374 RID: 4980 RVA: 0x00077494 File Offset: 0x00075694
	public void ResetVisual()
	{
		this.placeImg.rectTransform.DOKill(false);
		this.placeImg.rectTransform.localRotation = Quaternion.Euler(Vector3.zero);
		this.placeAudioSource.DOKill(false);
		this.placeAudioSource.volume = 0f;
		this.cricketSingImage.DOKill(false);
		this.cricketSingImage.localScale = Vector3.forward;
		this.hoverImg.gameObject.SetActive(false);
		this.IsShaking = false;
	}

	// Token: 0x04001043 RID: 4163
	[SerializeField]
	private CButton button;

	// Token: 0x04001044 RID: 4164
	[SerializeField]
	private PointerTrigger pointerTrigger;

	// Token: 0x04001045 RID: 4165
	[SerializeField]
	private CImage placeImg;

	// Token: 0x04001046 RID: 4166
	[SerializeField]
	private CImage blockImg;

	// Token: 0x04001047 RID: 4167
	[SerializeField]
	private RectTransform cricketSingImage;

	// Token: 0x04001048 RID: 4168
	[SerializeField]
	private CImage hoverImg;

	// Token: 0x04001049 RID: 4169
	[SerializeField]
	private AudioSource placeAudioSource;

	// Token: 0x0400104A RID: 4170
	private RectTransform _rectTransform;
}
