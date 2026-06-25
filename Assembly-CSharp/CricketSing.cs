using System;
using Config;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

// Token: 0x020001F2 RID: 498
public class CricketSing : MonoBehaviour
{
	// Token: 0x17000342 RID: 834
	// (get) Token: 0x0600207B RID: 8315 RVA: 0x000EC976 File Offset: 0x000EAB76
	public int SingSize
	{
		get
		{
			return (int)(this._isCombineCricket ? (this._colorConfig.SingSize + this._partConfig.SingSize) : this._colorConfig.SingSize);
		}
	}

	// Token: 0x17000343 RID: 835
	// (get) Token: 0x0600207C RID: 8316 RVA: 0x000EC9A4 File Offset: 0x000EABA4
	public int Level
	{
		get
		{
			return this._isCombineCricket ? Mathf.Max((int)this._colorConfig.Level, (int)this._partConfig.Level) : ((int)this._colorConfig.Level);
		}
	}

	// Token: 0x17000344 RID: 836
	// (get) Token: 0x0600207D RID: 8317 RVA: 0x000EC9D6 File Offset: 0x000EABD6
	private float NextSingDelay
	{
		get
		{
			return 1.2f + Random.Range(3f, (float)(4 + this.Level));
		}
	}

	// Token: 0x17000345 RID: 837
	// (get) Token: 0x0600207E RID: 8318 RVA: 0x000EC9F1 File Offset: 0x000EABF1
	// (set) Token: 0x0600207F RID: 8319 RVA: 0x000EC9F9 File Offset: 0x000EABF9
	public bool IsSinging { get; private set; }

	// Token: 0x06002080 RID: 8320 RVA: 0x000ECA02 File Offset: 0x000EAC02
	private void Awake()
	{
		this._singImage = base.GetComponent<CImage>();
	}

	// Token: 0x06002081 RID: 8321 RVA: 0x000ECA14 File Offset: 0x000EAC14
	private void Update()
	{
		bool flag = this._loopDelay <= 0f;
		if (!flag)
		{
			this._loopDelay -= Time.deltaTime;
			bool flag2 = this._loopDelay > 0f;
			if (!flag2)
			{
				bool loopSing = this._loopSing;
				if (loopSing)
				{
					this.SingInternal();
				}
				else
				{
					this.IsSinging = false;
				}
			}
		}
	}

	// Token: 0x06002082 RID: 8322 RVA: 0x000ECA78 File Offset: 0x000EAC78
	public void SetCricketData(short colorId, short partId)
	{
		this._colorConfig = CricketParts.Instance[colorId];
		this._partConfig = CricketParts.Instance[partId];
		this._isCombineCricket = (partId > 0);
	}

	// Token: 0x06002083 RID: 8323 RVA: 0x000ECAA8 File Offset: 0x000EACA8
	public void Sing(bool loop = false, float singSize = -1f)
	{
		bool flag = base.gameObject == null || !base.gameObject.activeInHierarchy || this._singImage == null;
		if (flag)
		{
			this.IsSinging = false;
		}
		else
		{
			bool isSinging = this.IsSinging;
			if (!isSinging)
			{
				this.IsSinging = true;
				this._loopSing = loop;
				this._singSize = singSize;
				this.SingInternal();
			}
		}
	}

	// Token: 0x06002084 RID: 8324 RVA: 0x000ECB18 File Offset: 0x000EAD18
	public void StopLoopSing()
	{
		this.IsSinging = false;
		this._loopSing = false;
		this._loopDelay = -1f;
	}

	// Token: 0x06002085 RID: 8325 RVA: 0x000ECB38 File Offset: 0x000EAD38
	private void SingInternal()
	{
		this._singImage.DOKill(false);
		this._singImage.rectTransform.DOKill(false);
		this._singImage.rectTransform.localScale = Vector3.zero;
		this._singImage.rectTransform.DOScale((0.3f + 0.7f * this._singSize >= 0f) ? this._singSize : ((float)this.SingSize / 100f), 1.2f);
		this._singImage.SetAlpha(1f);
		this._singImage.DOFade(0f, 0.6f).SetDelay(0.6f);
		this._loopDelay = this.NextSingDelay;
	}

	// Token: 0x04001887 RID: 6279
	private const float SingAnimSeconds = 1.2f;

	// Token: 0x04001889 RID: 6281
	private CricketPartsItem _colorConfig;

	// Token: 0x0400188A RID: 6282
	private CricketPartsItem _partConfig;

	// Token: 0x0400188B RID: 6283
	private bool _isCombineCricket;

	// Token: 0x0400188C RID: 6284
	private bool _loopSing;

	// Token: 0x0400188D RID: 6285
	private float _singSize;

	// Token: 0x0400188E RID: 6286
	private float _loopDelay = -1f;

	// Token: 0x0400188F RID: 6287
	private CImage _singImage;
}
