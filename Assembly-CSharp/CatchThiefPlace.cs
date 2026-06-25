using System;
using Config;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

// Token: 0x020001C1 RID: 449
public class CatchThiefPlace : MonoBehaviour
{
	// Token: 0x170002D8 RID: 728
	// (get) Token: 0x06001BDD RID: 7133 RVA: 0x000C0A7B File Offset: 0x000BEC7B
	// (set) Token: 0x06001BDE RID: 7134 RVA: 0x000C0A83 File Offset: 0x000BEC83
	public sbyte ThiefLevel { get; private set; }

	// Token: 0x170002D9 RID: 729
	// (get) Token: 0x06001BDF RID: 7135 RVA: 0x000C0A8C File Offset: 0x000BEC8C
	public CatchThiefPlaceItem PlaceConfig
	{
		get
		{
			return Config.CatchThiefPlace.Instance[this._placeTemplateId];
		}
	}

	// Token: 0x170002DA RID: 730
	// (get) Token: 0x06001BE0 RID: 7136 RVA: 0x000C0A9E File Offset: 0x000BEC9E
	public CatchThiefLevelItem ThiefConfig
	{
		get
		{
			return CatchThiefLevel.Instance[this.ThiefLevel];
		}
	}

	// Token: 0x170002DB RID: 731
	// (get) Token: 0x06001BE1 RID: 7137 RVA: 0x000C0AB0 File Offset: 0x000BECB0
	public float Pitch
	{
		get
		{
			return 1.35f - 0.65f * (float)this.ThiefConfig.SingPitch / 100f;
		}
	}

	// Token: 0x170002DC RID: 732
	// (get) Token: 0x06001BE2 RID: 7138 RVA: 0x000C0AD0 File Offset: 0x000BECD0
	public float Size
	{
		get
		{
			return 0.3f + 0.7f * (float)this.ThiefConfig.SingSize / 100f;
		}
	}

	// Token: 0x170002DD RID: 733
	// (get) Token: 0x06001BE3 RID: 7139 RVA: 0x000C0AF0 File Offset: 0x000BECF0
	public RectTransform RectTransform
	{
		get
		{
			return (this._rectTransform != null) ? this._rectTransform : (this._rectTransform = (RectTransform)base.transform);
		}
	}

	// Token: 0x06001BE4 RID: 7140 RVA: 0x000C0B28 File Offset: 0x000BED28
	public void Init(int index, Action<int> onClick)
	{
		base.GetComponent<CButtonObsolete>().ClearAndAddListener(delegate
		{
			onClick(index);
		});
	}

	// Token: 0x06001BE5 RID: 7141 RVA: 0x000C0B62 File Offset: 0x000BED62
	private void OnDisable()
	{
		AudioManager.Instance.RemoveAudioSource(this.singSource);
	}

	// Token: 0x06001BE6 RID: 7142 RVA: 0x000C0B78 File Offset: 0x000BED78
	public void Set(sbyte placeTemplateId, int catchThiefTimes)
	{
		this._placeTemplateId = placeTemplateId;
		CatchThiefPlaceItem config = this.PlaceConfig;
		int[] allWeights = config.LevelWeights[Mathf.Clamp(catchThiefTimes, 0, config.LevelWeights.Length - 1)];
		int randomLevel = allWeights.GetRandomIndex();
		this.ThiefLevel = (sbyte)Mathf.Clamp(randomLevel, 0, CatchThiefLevel.Instance.Count);
		this.icon.SetSprite(config.Icon, false, null);
		Transform iconTransform = this.icon.transform;
		iconTransform.DOKill(false);
		iconTransform.localRotation = Quaternion.Euler(Vector3.zero);
		this.singSource.DOKill(false);
		this.singSource.volume = 0f;
		this.singSource.pitch = this.Pitch;
		this.singCircle.DOKill(false);
		this.singCircle.localScale = Vector3.zero;
	}

	// Token: 0x06001BE7 RID: 7143 RVA: 0x000C0C54 File Offset: 0x000BEE54
	public void SetSing(float startTime, int singCount)
	{
		this._startTime = startTime;
		this._count = singCount;
	}

	// Token: 0x06001BE8 RID: 7144 RVA: 0x000C0C68 File Offset: 0x000BEE68
	public void StopPlaying()
	{
		this.singSource.DOKill(false);
		bool isPlaying = this.singSource.isPlaying;
		if (isPlaying)
		{
			this.singSource.DOFade(0f, 0.1f).SetDelay(0.2f).OnComplete(new TweenCallback(this.singSource.Stop));
		}
	}

	// Token: 0x06001BE9 RID: 7145 RVA: 0x000C0CC8 File Offset: 0x000BEEC8
	public void UpdateSing(float timer)
	{
		bool flag = this._currentVolume > 0;
		if (flag)
		{
			float singTotalTime = this._currentTotalTime + ((this._currentVolume >= 80) ? CatchUtils.BaseSingTimeMin : 0f);
			this._currentTimer += Time.deltaTime;
			bool flag2 = timer / 30f > 0.8f && this._loudTime <= 0f;
			if (flag2)
			{
				this._currentTimer = 0f;
				this.UpdateSingVolume();
				this._currentVolume = 80;
				this.UpdateSingCircle();
				this._loudTime = 0.1f;
			}
			else
			{
				bool flag3 = this._currentTimer >= singTotalTime;
				if (flag3)
				{
					bool flag4 = this._currentVolume >= 80;
					if (flag4)
					{
						bool flag5 = Utils_Random.RandomCheck((int)this.ThiefLevel, 100);
						if (flag5)
						{
							this._currentVolume = 0;
						}
						else
						{
							bool flag6 = Utils_Random.RandomCheck((this._count <= 0) ? 40 : 30, 100);
							if (flag6)
							{
								this._currentVolume = (short)Mathf.Clamp((int)(this._currentVolume + (short)(10 + this.ThiefLevel * 2)), 0, 100);
							}
							else
							{
								this._currentVolume = (short)Mathf.Clamp((int)(this._currentVolume - (short)(10 + this.ThiefLevel * 2)), 0, 100);
							}
						}
					}
					else
					{
						bool flag7 = Utils_Random.RandomCheck((int)this.ThiefLevel, 100);
						if (flag7)
						{
							this._currentVolume = 80;
						}
						else
						{
							bool flag8 = Utils_Random.RandomCheck((this._count <= 0) ? 25 : 35, 100);
							if (flag8)
							{
								this._currentVolume = (short)Mathf.Clamp((int)(this._currentVolume - (short)(10 + this.ThiefLevel * 2)), 0, 100);
							}
							else
							{
								this._currentVolume = (short)Mathf.Clamp((int)(this._currentVolume + (short)(10 + this.ThiefLevel * 2)), 0, 100);
							}
						}
					}
					this._currentTimer = 0f;
					this.UpdateSingVolume();
					bool flag9 = this._currentVolume > 0;
					if (flag9)
					{
						this.UpdateSingCircle();
					}
				}
				else
				{
					bool flag10 = this._currentVolume >= 80;
					if (flag10)
					{
						bool flag11 = this._loudTime >= this._currentTotalTime / 2f;
						if (flag11)
						{
							this.UpdateSingCircle();
							this._loudTime = 0f;
						}
						this._loudTime += Time.deltaTime;
					}
				}
			}
		}
		else
		{
			bool flag12 = this._delayTime > 0f;
			if (flag12)
			{
				this._delayTime -= Time.deltaTime;
			}
			else
			{
				bool flag13 = timer >= this._startTime && this._count > 0;
				if (flag13)
				{
					this._count--;
					this._currentVolume = (short)Random.Range((int)((this.ThiefLevel + 1) * 2), (int)((this.ThiefLevel + 1) * 4));
					this._currentTotalTime = CatchUtils.RandomSingTime + (float)this.ThiefLevel * GlobalConfig.Instance.CricketSingGradeTime;
					this._delayTime = CatchUtils.RandomDelayTime;
					this._currentTimer = 0f;
					this._loudTime = 0f;
					this.UpdateSingVolume();
					this.UpdateSingCircle();
				}
			}
		}
	}

	// Token: 0x06001BEA RID: 7146 RVA: 0x000C0FE4 File Offset: 0x000BF1E4
	private void UpdateSingVolume()
	{
		float maxVolume = (float)SingletonObject.getInstance<GlobalSettings>().SeVolume / 100f;
		float volume = (this._currentVolume > 0) ? (maxVolume * 0.4f + maxVolume * 0.6f * (float)this._currentVolume / 100f - (float)(9 - this.ThiefLevel) * 0.025f) : 0f;
		volume = ((SingletonObject.getInstance<GlobalSettings>().SeOn && !AudioManager.Instance.GetShouldMuteIfFocus()) ? Mathf.Clamp(volume, 0f, 1f) : 0f);
		this.singSource.DOKill(false);
		this.singSource.DOFade(volume, 0.1f);
		AudioManager.Instance.AddAudioSource(this.singSource, volume);
	}

	// Token: 0x06001BEB RID: 7147 RVA: 0x000C10A4 File Offset: 0x000BF2A4
	private void UpdateSingCircle()
	{
		float aniTime = (this._currentVolume >= 80) ? (this._currentTotalTime / 2f) : this._currentTotalTime;
		int shakePower = (this._currentVolume >= 80) ? 20 : 10;
		CImage placeImg = this.icon;
		Transform singImg = this.singCircle;
		singImg.DOKill(false);
		singImg.localScale = Vector3.forward;
		singImg.DOScale(((this._currentVolume >= 80) ? 1f : 0.5f) * this.Size, aniTime);
		singImg.GetComponent<CImage>().color = Color.white;
		singImg.GetComponent<CImage>().DOFade(0f, aniTime / 2f).SetDelay(aniTime / 2f);
		placeImg.rectTransform.DOShakeRotation((this._currentVolume >= 80) ? 0.1f : 0.2f, new Vector3(0f, 0f, (float)(shakePower + shakePower * (int)this.ThiefLevel * 10 / 100)), 10, 90f, true, ShakeRandomnessMode.Full).OnComplete(delegate
		{
			placeImg.rectTransform.localRotation = Quaternion.Euler(Vector3.zero);
		});
		bool flag = !this.singSource.isPlaying;
		if (flag)
		{
			this.singSource.Play();
		}
	}

	// Token: 0x040015B9 RID: 5561
	private sbyte _placeTemplateId;

	// Token: 0x040015BA RID: 5562
	[SerializeField]
	private CImage icon;

	// Token: 0x040015BB RID: 5563
	[SerializeField]
	private AudioSource singSource;

	// Token: 0x040015BC RID: 5564
	[SerializeField]
	private Transform singCircle;

	// Token: 0x040015BD RID: 5565
	private RectTransform _rectTransform;

	// Token: 0x040015BE RID: 5566
	private float _startTime;

	// Token: 0x040015BF RID: 5567
	private int _count;

	// Token: 0x040015C0 RID: 5568
	private short _currentVolume;

	// Token: 0x040015C1 RID: 5569
	private float _currentTotalTime;

	// Token: 0x040015C2 RID: 5570
	private float _currentTimer;

	// Token: 0x040015C3 RID: 5571
	private float _delayTime;

	// Token: 0x040015C4 RID: 5572
	private float _loudTime;
}
