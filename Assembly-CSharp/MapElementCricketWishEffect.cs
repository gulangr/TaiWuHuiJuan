using System;
using Coffee.UIExtensions;
using DG.Tweening;
using GameData.Domains.Map;
using UnityEngine;

// Token: 0x020003EA RID: 1002
public class MapElementCricketWishEffect : MapElementBase
{
	// Token: 0x06003C4A RID: 15434 RVA: 0x001E682C File Offset: 0x001E4A2C
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
				bool flag3 = MapElementBase.MapModel.CurrentAreaId != location.AreaId;
				result = (!flag3 && MapElementBase.MapModel.CricketWishEffectLocations.Contains(location));
			}
		}
		return result;
	}

	// Token: 0x17000621 RID: 1569
	// (get) Token: 0x06003C4B RID: 15435 RVA: 0x001E68B2 File Offset: 0x001E4AB2
	public override EMapLayer Layer
	{
		get
		{
			return EMapLayer.TemporaryMark;
		}
	}

	// Token: 0x06003C4C RID: 15436 RVA: 0x001E68B5 File Offset: 0x001E4AB5
	public override void Scale(float wheel)
	{
		base.ScaleReverse(wheel);
	}

	// Token: 0x06003C4D RID: 15437 RVA: 0x001E68C0 File Offset: 0x001E4AC0
	protected override void OnCreate()
	{
		this.effectRoot.SetActive(false);
	}

	// Token: 0x06003C4E RID: 15438 RVA: 0x001E68D0 File Offset: 0x001E4AD0
	protected override void OnRefresh()
	{
		bool isPlaying = this._isPlaying;
		if (!isPlaying)
		{
			this._isPlaying = true;
			this.effectRoot.SetActive(true);
			this.effect.Play();
			this._finishTween = DOVirtual.DelayedCall(this.duration, new TweenCallback(this.FinishEffect), true).SetAutoKill(true);
		}
	}

	// Token: 0x06003C4F RID: 15439 RVA: 0x001E692E File Offset: 0x001E4B2E
	protected override void OnCollect()
	{
		this.KillFinishTween();
		this.StopEffect();
		MapElementBase.MapModel.CricketWishEffectLocations.Remove(base.BlockLocation);
	}

	// Token: 0x06003C50 RID: 15440 RVA: 0x001E6958 File Offset: 0x001E4B58
	private void FinishEffect()
	{
		this._finishTween = null;
		this.StopEffect();
		bool flag = MapElementBase.MapModel.CricketWishEffectLocations.Remove(base.BlockLocation);
		if (flag)
		{
			GEvent.OnEvent(UiEvents.CricketWishEffectChanged, null);
		}
	}

	// Token: 0x06003C51 RID: 15441 RVA: 0x001E699E File Offset: 0x001E4B9E
	private void StopEffect()
	{
		this._isPlaying = false;
		this.effect.Stop();
		this.effectRoot.SetActive(false);
	}

	// Token: 0x06003C52 RID: 15442 RVA: 0x001E69C4 File Offset: 0x001E4BC4
	private void KillFinishTween()
	{
		bool flag = this._finishTween == null;
		if (!flag)
		{
			this._finishTween.Kill(false);
			this._finishTween = null;
		}
	}

	// Token: 0x04002B57 RID: 11095
	[SerializeField]
	private GameObject effectRoot;

	// Token: 0x04002B58 RID: 11096
	[SerializeField]
	private UIParticle effect;

	// Token: 0x04002B59 RID: 11097
	[SerializeField]
	private float duration = 1.5f;

	// Token: 0x04002B5A RID: 11098
	private Tween _finishTween;

	// Token: 0x04002B5B RID: 11099
	private bool _isPlaying;
}
