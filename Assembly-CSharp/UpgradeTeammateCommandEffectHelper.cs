using System;
using System.Collections.Generic;
using System.Linq;
using Coffee.UIExtensions;
using Config;
using UnityEngine;

// Token: 0x02000318 RID: 792
public static class UpgradeTeammateCommandEffectHelper
{
	// Token: 0x06002E74 RID: 11892 RVA: 0x0016EC84 File Offset: 0x0016CE84
	public static string GetIcon(TeammateCommandItem config, int index)
	{
		ETeammateCommandImplement implement = config.Implement;
		if (!true)
		{
		}
		string result;
		if (implement != ETeammateCommandImplement.AddHit)
		{
			if (implement != ETeammateCommandImplement.AddAvoid)
			{
				result = null;
			}
			else
			{
				result = string.Format("sp_icon_attribute_{0}", UpgradeTeammateCommandEffectHelper.IconPattern2[index]);
			}
		}
		else
		{
			result = string.Format("sp_icon_attribute_{0}", UpgradeTeammateCommandEffectHelper.IconPattern1[index]);
		}
		if (!true)
		{
		}
		return result;
	}

	// Token: 0x06002E75 RID: 11893 RVA: 0x0016ECEC File Offset: 0x0016CEEC
	public static bool HasIcon(TeammateCommandItem config, int index)
	{
		ETeammateCommandImplement implement = config.Implement;
		if (!true)
		{
		}
		bool result;
		if (implement != ETeammateCommandImplement.AddHit)
		{
			result = (implement == ETeammateCommandImplement.AddAvoid && index < UpgradeTeammateCommandEffectHelper.IconPattern2.Length);
		}
		else
		{
			result = (index < UpgradeTeammateCommandEffectHelper.IconPattern1.Length);
		}
		if (!true)
		{
		}
		return result;
	}

	// Token: 0x06002E76 RID: 11894 RVA: 0x0016ED3C File Offset: 0x0016CF3C
	public static string GetValueColor(TeammateCommandItem config, TeammateCommandItem normalConfig, int index)
	{
		bool flag = config.TemplateId == normalConfig.TemplateId;
		string result;
		if (flag)
		{
			result = "pinkyellow";
		}
		else
		{
			result = UpgradeTeammateCommandEffectHelper.GetValueColor2(config, index);
		}
		return result;
	}

	// Token: 0x06002E77 RID: 11895 RVA: 0x0016ED70 File Offset: 0x0016CF70
	public static string GetValueColor2(TeammateCommandItem config, int index)
	{
		string result;
		switch (config.EffectDisplayPositiveList[index])
		{
		case 0:
			result = "pinkyellow";
			break;
		case 1:
			result = "brightblue";
			break;
		case 2:
			result = "brightred";
			break;
		default:
			result = null;
			break;
		}
		return result;
	}

	// Token: 0x06002E78 RID: 11896 RVA: 0x0016EDBC File Offset: 0x0016CFBC
	public static void RefreshValueLoopParticle(TeammateCommandItem advanceConfig, TeammateCommandItem normalConfig, int index, UIParticle upParticle, UIParticle downParticle)
	{
		bool isUpgraded = advanceConfig.TemplateId == normalConfig.TemplateId;
		sbyte colorType = advanceConfig.EffectDisplayPositiveList[index];
		bool needParticleByDiff = UpgradeTeammateCommandEffectHelper.NeedParticle(advanceConfig, normalConfig, index);
		bool showUp = colorType == 1 && !isUpgraded && needParticleByDiff;
		upParticle.gameObject.SetActive(showUp);
		bool showDown = colorType == 2 && !isUpgraded && needParticleByDiff;
		downParticle.gameObject.SetActive(showDown);
		bool flag = showUp;
		if (flag)
		{
			upParticle.Play();
		}
		else
		{
			bool flag2 = showDown;
			if (flag2)
			{
				downParticle.Play();
			}
		}
	}

	// Token: 0x06002E79 RID: 11897 RVA: 0x0016EE48 File Offset: 0x0016D048
	public static void PlayUpgradeParticle(TeammateCommandItem advanceConfig, TeammateCommandItem normalConfig, int index, UIParticle upParticle, UIParticle downParticle)
	{
		bool flag = !UpgradeTeammateCommandEffectHelper.NeedParticle(advanceConfig, normalConfig, index);
		if (!flag)
		{
			sbyte colorType = advanceConfig.EffectDisplayPositiveList[index];
			bool flag2 = colorType == 1;
			if (flag2)
			{
				UpgradeTeammateCommandEffectHelper.PlayOnceParticle(upParticle, 1f);
			}
			else
			{
				bool flag3 = colorType == 2;
				if (flag3)
				{
					UpgradeTeammateCommandEffectHelper.PlayOnceParticle(downParticle, 1f);
				}
			}
		}
	}

	// Token: 0x06002E7A RID: 11898 RVA: 0x0016EEA0 File Offset: 0x0016D0A0
	private static void PlayOnceParticle(UIParticle particle, float time)
	{
		Debug.Log(string.Format("play: {0} for {1} seconds", particle.name, time));
		particle.gameObject.SetActive(true);
		particle.Play();
		Coroutine coroutine;
		bool flag = UpgradeTeammateCommandEffectHelper._playOneParticleCoroutines.TryGetValue(particle, out coroutine);
		if (flag)
		{
			SingletonObject.getInstance<YieldHelper>().StopYield(coroutine);
		}
		Coroutine newCoroutine = SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(time, delegate
		{
			particle.gameObject.SetActive(false);
			particle.Pause();
		});
		UpgradeTeammateCommandEffectHelper._playOneParticleCoroutines[particle] = newCoroutine;
	}

	// Token: 0x06002E7B RID: 11899 RVA: 0x0016EF48 File Offset: 0x0016D148
	public static bool NeedParticle(TeammateCommandItem advanceConfig, TeammateCommandItem normalConfig, int indexForAdvance)
	{
		bool flag = normalConfig == advanceConfig;
		bool result;
		if (flag)
		{
			Debug.LogWarning("NeedParticle need different advance config ant normal config");
			result = false;
		}
		else
		{
			string displayText = advanceConfig.EffectDisplayTextList[indexForAdvance];
			string value = advanceConfig.EffectDisplayValueList[indexForAdvance];
			result = (!normalConfig.EffectDisplayTextList.Contains(displayText) || normalConfig.EffectDisplayValueList[normalConfig.EffectDisplayTextList.IndexOf(displayText)] != value);
		}
		return result;
	}

	// Token: 0x040021B9 RID: 8633
	public static readonly int[] IconPattern1 = new int[]
	{
		10,
		12,
		14,
		16
	};

	// Token: 0x040021BA RID: 8634
	public static readonly int[] IconPattern2 = new int[]
	{
		11,
		13,
		15,
		17
	};

	// Token: 0x040021BB RID: 8635
	private static readonly Dictionary<UIParticle, Coroutine> _playOneParticleCoroutines = new Dictionary<UIParticle, Coroutine>();
}
