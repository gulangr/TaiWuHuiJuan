using System;
using System.Collections;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Serializer;
using GameData.Utilities;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001FA RID: 506
public class UI_ExtraordinaryCricket : UIBase
{
	// Token: 0x060020D2 RID: 8402 RVA: 0x000EFAA8 File Offset: 0x000EDCA8
	public override void OnInit(ArgumentBox argsBox)
	{
		Time.timeScale = 1.75f;
		this.ResetData();
		ItemKey itemKey;
		argsBox.Get<ItemKey>("ItemKey", out itemKey);
		bool flag = itemKey.Id > 0;
		if (flag)
		{
			ItemDomainMethod.AsyncCall.GetItemDisplayData(this, itemKey, delegate(int offset, RawDataPool dataPool)
			{
				Serializer.Deserialize(dataPool, offset, ref this.cricket);
				base.CGet<CricketView>("CricketView").gameObject.SetActive(false);
				base.CGet<CricketView>("CricketView").SetCricketData(this.cricket.CricketColorId, this.cricket.CricketPartId, true, this.cricket, false);
				base.CGet<CricketView>("CricketView").gameObject.SetActive(true);
				base.CGet<CricketView>("CricketView").Sing(true, true, true, -1f, null, 0f);
			});
		}
	}

	// Token: 0x060020D3 RID: 8403 RVA: 0x000EFAFC File Offset: 0x000EDCFC
	private void OnEnable()
	{
		base.StartCoroutine(this.WaitShow());
		base.CGet<CanvasGroup>("BackGround").DOFade(1f, this.fadeTime).SetEase(Ease.OutCubic);
		base.DelayCall(delegate
		{
			base.CGet<CanvasGroup>("Root").DOFade(1f, this.fadeTime - 0.5f).SetEase(Ease.OutCubic);
		}, 0.5f);
	}

	// Token: 0x060020D4 RID: 8404 RVA: 0x000EFB52 File Offset: 0x000EDD52
	private void OnDisable()
	{
		Time.timeScale = 1f;
		base.CGet<CanvasGroup>("BackGround").alpha = this.backGroundAlphaStartValue;
		base.CGet<CanvasGroup>("Root").alpha = this.circketAlphaStartValue;
	}

	// Token: 0x060020D5 RID: 8405 RVA: 0x000EFB90 File Offset: 0x000EDD90
	private void Update()
	{
		bool flag = UIManager.Instance.IsFocusElement(this.Element);
		if (flag)
		{
			bool flag2 = CommonCommandKit.LeftMouse.Check(this.Element, false, false, false, true, false) || CommonCommandKit.RightMouse.Check(this.Element, false, false, false, true, false) || CommonCommandKit.Space.Check(this.Element, false, false, false, true, false);
			if (flag2)
			{
				UIManager.Instance.HideUI(this.Element);
			}
		}
	}

	// Token: 0x060020D6 RID: 8406 RVA: 0x000EFC11 File Offset: 0x000EDE11
	private IEnumerator Wait()
	{
		yield return new WaitForSeconds(2f);
		base.CGet<GameObject>("eff_cricketcombat_smart_kaishi").SetActive(false);
		base.CGet<GameObject>("eff_cricketcombat_smart_xunhuan").SetActive(true);
		this.PlayParticle(base.CGet<GameObject>("eff_cricketcombat_smart_xunhuan").transform);
		base.CGet<GameObject>("ClickToContinue").SetActive(true);
		yield break;
	}

	// Token: 0x060020D7 RID: 8407 RVA: 0x000EFC20 File Offset: 0x000EDE20
	private IEnumerator WaitShow()
	{
		yield return new WaitForSeconds(1f);
		base.CGet<GameObject>("eff_cricketcombat_smart_kaishi").SetActive(true);
		this.PlayParticle(base.CGet<GameObject>("eff_cricketcombat_smart_kaishi").transform);
		base.CGet<GameObject>("eff_cricketcombat_smart_ziti").SetActive(true);
		TweenerCore<float, float, FloatOptions> tweenerCore = base.CGet<RawImage>("RawImage").material.DOFloat(0f, UI_ExtraordinaryCricket.Rongjie, 1f);
		tweenerCore.onComplete = (TweenCallback)Delegate.Combine(tweenerCore.onComplete, new TweenCallback(delegate()
		{
			base.CGet<GameObject>("Leaf_2").SetActive(true);
			base.CGet<RawImage>("RawImage").gameObject.SetActive(false);
			this.PlayParticle(base.CGet<GameObject>("eff_cricketcombat_smart_ziti").transform);
		}));
		base.StartCoroutine(this.Wait());
		yield break;
	}

	// Token: 0x060020D8 RID: 8408 RVA: 0x000EFC30 File Offset: 0x000EDE30
	private void PlayParticle(Transform transform)
	{
		ParticleSystem[] list = transform.GetComponentsInChildren<ParticleSystem>();
		foreach (ParticleSystem system in list)
		{
			system.Play();
		}
	}

	// Token: 0x060020D9 RID: 8409 RVA: 0x000EFC64 File Offset: 0x000EDE64
	private void ResetData()
	{
		base.CGet<GameObject>("eff_cricketcombat_smart_kaishi").SetActive(false);
		base.CGet<GameObject>("eff_cricketcombat_smart_xunhuan").SetActive(false);
		base.CGet<GameObject>("eff_cricketcombat_smart_ziti").SetActive(false);
		base.CGet<GameObject>("Leaf_2").SetActive(false);
		base.CGet<RawImage>("RawImage").gameObject.SetActive(true);
		base.CGet<RawImage>("RawImage").material.SetFloat(UI_ExtraordinaryCricket.Rongjie, 1f);
	}

	// Token: 0x04001938 RID: 6456
	private ItemDisplayData cricket;

	// Token: 0x04001939 RID: 6457
	private static readonly int Rongjie = Shader.PropertyToID("_rongjie");

	// Token: 0x0400193A RID: 6458
	public float fadeTime = 1.5f;

	// Token: 0x0400193B RID: 6459
	public float backGroundAlphaStartValue = 0.5f;

	// Token: 0x0400193C RID: 6460
	public float circketAlphaStartValue = 0f;
}
