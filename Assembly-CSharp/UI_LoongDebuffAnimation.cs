using System;
using System.Collections;
using Config;
using DG.Tweening;
using FrameWork;
using Game.Views.Map;
using GameData.Domains.TaiwuEvent;
using UnityEngine;

// Token: 0x0200024E RID: 590
public class UI_LoongDebuffAnimation : UIBase
{
	// Token: 0x0600272F RID: 10031 RVA: 0x00121342 File Offset: 0x0011F542
	public override void OnInit(ArgumentBox argsBox)
	{
		this._particles = base.CGetList<GameObject>("debuff_").ToArray();
		this.PlayLoongDebuffAnimation(argsBox);
	}

	// Token: 0x06002730 RID: 10032 RVA: 0x00121364 File Offset: 0x0011F564
	private void PlayLoongDebuffAnimation(ArgumentBox argsBox)
	{
		argsBox.Get("LoongCharacterTemplateId", out this._loongCharacterTemplateId);
		this._loongConfig = Loong.Instance[(int)(this._loongCharacterTemplateId - 246)];
		SingletonObject.getInstance<YieldHelper>().StartYield(this.LoongDebuffAnimation());
	}

	// Token: 0x06002731 RID: 10033 RVA: 0x001213B1 File Offset: 0x0011F5B1
	private IEnumerator LoongDebuffAnimation()
	{
		UIElement.BlockInteract.Show();
		yield return this._waitForSeconds;
		GEvent.OnEvent(UiEvents.WorldMapResetMapCamera, EasyPool.Get<ArgumentBox>().Set("isAnim", true));
		ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
		argBox.Set("targetScale", 1f);
		argBox.Set("duration", 0.7f);
		argBox.SetObject("easeMode", Ease.OutQuad);
		argBox.Set("interval", 1f);
		argBox.SetObject("tweenType", EWorldmapScaleTweenType.IntervalPingPong);
		GEvent.OnEvent(UiEvents.DoWorldMapScaleTween, argBox);
		GameObject go = this.FindParticleGo();
		ParticleSystem particle = go.GetComponent<ParticleSystem>();
		bool flag = particle != null;
		if (flag)
		{
			go.SetActive(true);
			particle.Play();
			AudioManager.Instance.PlaySound(this._loongConfig.EnterCombatSound, false, false);
		}
		yield return this._waitForSeconds;
		UIElement.BlockInteract.Hide(false);
		TaiwuEventDomainMethod.Call.TriggerListener("PlayLoongDebuffAnimation", true);
		this.QuickHide();
		go.SetActive(false);
		yield break;
	}

	// Token: 0x06002732 RID: 10034 RVA: 0x001213C0 File Offset: 0x0011F5C0
	private GameObject FindParticleGo()
	{
		for (int i = 0; i < this._particles.Length; i++)
		{
			bool flag = this._particles[i].name.Equals(this._loongConfig.EnterCombatEffect);
			if (flag)
			{
				return this._particles[i];
			}
		}
		return null;
	}

	// Token: 0x04001C96 RID: 7318
	private short _loongCharacterTemplateId;

	// Token: 0x04001C97 RID: 7319
	private GameObject[] _particles;

	// Token: 0x04001C98 RID: 7320
	private LoongItem _loongConfig;

	// Token: 0x04001C99 RID: 7321
	private const float WaitSeconds = 1f;

	// Token: 0x04001C9A RID: 7322
	private WaitForSeconds _waitForSeconds = new WaitForSeconds(1f);
}
