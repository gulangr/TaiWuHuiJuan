using System;
using Config;
using DG.Tweening;
using GameData.Domains.Building;
using UnityEngine;

namespace Building
{
	// Token: 0x0200065C RID: 1628
	public class BuildingExpandTaiwuVillageAnimationController : MonoBehaviour
	{
		// Token: 0x06004D95 RID: 19861 RVA: 0x002495CD File Offset: 0x002477CD
		private void OnEnable()
		{
			this.explosionGood.SetActive(false);
			this.explosionEvil.SetActive(false);
			this.explosionNeutral.SetActive(false);
		}

		// Token: 0x06004D96 RID: 19862 RVA: 0x002495F7 File Offset: 0x002477F7
		public void Bind(IBuildingExpandTaiwuVillageSteleProvider provider)
		{
			this._steleProvider = provider;
		}

		// Token: 0x06004D97 RID: 19863 RVA: 0x00249600 File Offset: 0x00247800
		public Sequence PlayUnlockEffect(BuildingBlockData blockData, sbyte orgTemplateId)
		{
			sbyte goodness = Organization.Instance[orgTemplateId].Goodness;
			GameObject explosion = this.GetExplosionGo(goodness);
			bool unlockNewCore = this.CheckUnlockNewCore(blockData, orgTemplateId);
			BuildingExpandTaiwuVillageStele stele = this._steleProvider.GetStele(orgTemplateId);
			Sequence sequence = DOTween.Sequence();
			sequence.AppendCallback(delegate
			{
				this.effectPlayer.PlayEffectAt(stele.transform, "eff_shibei_shengji", 0.8f, false);
			});
			sequence.AppendCallback(delegate
			{
				stele.Set(false, -1);
			});
			sequence.AppendCallback(delegate
			{
				this.effectPlayer.PlayEffectAt(this.disk.GetUnlock(orgTemplateId), "eff_shibei_ui_dianliang", 0.5f, false);
			});
			sequence.AppendCallback(delegate
			{
				this.disk.SetUnlock(orgTemplateId, false);
			});
			sequence.AppendCallback(delegate
			{
				AudioManager.Instance.PlaySound(unlockNewCore ? "ui_building_expand_taiwuvillage_core" : "ui_building_expand_taiwuvillage_normal", false, false);
			});
			bool unlockNewCore4 = unlockNewCore;
			if (unlockNewCore4)
			{
				sequence.AppendCallback(delegate
				{
					explosion.SetActive(true);
				});
				sequence.AppendCallback(delegate
				{
					explosion.GetComponent<ParticleSystem>().Play(true);
				});
				sequence.AppendCallback(delegate
				{
					this.effectPlayer.PlayEffectAt(this.disk.GetCoreUnlock(goodness), "eff_shibei_ui_5gedianliang", 0.8f, false);
				});
				sequence.AppendCallback(delegate
				{
					this.disk.SetCoreUnlock(goodness, false);
				});
				sequence.AppendInterval(0.15f);
				sequence.AppendCallback(delegate
				{
					this.disk.SetGrassUnlock(goodness, true);
				});
			}
			sequence.AppendInterval(0.2f - (unlockNewCore ? 0.15f : 0f));
			sequence.AppendCallback(delegate
			{
				stele.AnimationToUpgraded(0.5f);
			});
			sequence.AppendInterval(0.10000001f);
			sequence.AppendCallback(delegate
			{
				this.disk.SetUnlock(orgTemplateId, true);
			});
			bool unlockNewCore2 = unlockNewCore;
			if (unlockNewCore2)
			{
				sequence.AppendCallback(delegate
				{
					this.disk.SetCoreUnlock(goodness, true);
				});
			}
			sequence.AppendInterval(0.5f);
			bool unlockNewCore3 = unlockNewCore;
			if (unlockNewCore3)
			{
				sequence.AppendInterval(0.8f);
				sequence.AppendCallback(delegate
				{
					explosion.SetActive(false);
				});
			}
			return sequence;
		}

		// Token: 0x06004D98 RID: 19864 RVA: 0x00249808 File Offset: 0x00247A08
		private GameObject GetExplosionGo(sbyte goodness)
		{
			if (!true)
			{
			}
			GameObject result;
			switch (goodness)
			{
			case -1:
				result = this.explosionEvil;
				break;
			case 0:
				result = this.explosionNeutral;
				break;
			case 1:
				result = this.explosionGood;
				break;
			default:
				throw new ArgumentOutOfRangeException("goodness", goodness, null);
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x06004D99 RID: 19865 RVA: 0x00249864 File Offset: 0x00247A64
		private bool CheckUnlockNewCore(BuildingBlockData blockData, sbyte orgTemplateId)
		{
			sbyte goodness = Organization.Instance[orgTemplateId].Goodness;
			int count = 1;
			for (sbyte i = 1; i <= 15; i += 1)
			{
				bool flag = i == orgTemplateId || !blockData.SlotIsUnlocked((int)(i - 1));
				if (!flag)
				{
					bool flag2 = Organization.Instance[i].Goodness == goodness;
					if (flag2)
					{
						count++;
					}
				}
			}
			return count == 5;
		}

		// Token: 0x040035C9 RID: 13769
		[SerializeField]
		private EffectPlayer effectPlayer;

		// Token: 0x040035CA RID: 13770
		[SerializeField]
		private BuildingExpandTaiwuVillageDisk disk;

		// Token: 0x040035CB RID: 13771
		[SerializeField]
		private GameObject explosionGood;

		// Token: 0x040035CC RID: 13772
		[SerializeField]
		private GameObject explosionEvil;

		// Token: 0x040035CD RID: 13773
		[SerializeField]
		private GameObject explosionNeutral;

		// Token: 0x040035CE RID: 13774
		private IBuildingExpandTaiwuVillageSteleProvider _steleProvider;
	}
}
