using System;
using System.Collections.Generic;
using Config;
using DG.Tweening;
using GameData.Domains.Building;
using GameData.Domains.Global;
using UnityEngine;

namespace Game.Views.Building
{
	// Token: 0x02000BDA RID: 3034
	public class TaiwuVillageExpandAnimationController : MonoBehaviour
	{
		// Token: 0x060098BC RID: 39100 RVA: 0x00471BA1 File Offset: 0x0046FDA1
		private void OnEnable()
		{
			this.explosionGood.SetActive(false);
			this.explosionEvil.SetActive(false);
			this.explosionNeutral.SetActive(false);
		}

		// Token: 0x060098BD RID: 39101 RVA: 0x00471BCB File Offset: 0x0046FDCB
		public void Bind(IBuildingExpandTaiwuVillageSteleProvider provider)
		{
			this._steleProvider = provider;
		}

		// Token: 0x060098BE RID: 39102 RVA: 0x00471BD4 File Offset: 0x0046FDD4
		public void AppendWheelPointEff(Sequence sequence, BuildingBlockData blockData, sbyte orgTemplateId)
		{
			sbyte goodness = Organization.Instance[orgTemplateId].Goodness;
			bool unlockNewCore = this.CheckUnlockNewCore(blockData, orgTemplateId);
			sequence.AppendCallback(delegate
			{
				this.effectPlayer.PlayEffectAt(this.disk.GetUnlock(orgTemplateId), "eff_shibei_ui_dianliang", 0.5f, false);
			});
			sequence.AppendCallback(delegate
			{
				this.disk.SetUnlock(orgTemplateId, true);
			});
			bool flag = unlockNewCore;
			if (flag)
			{
				sequence.AppendCallback(delegate
				{
					this.effectPlayer.PlayEffectAt(this.disk.GetCoreUnlock(goodness), "eff_shibei_ui_5gedianliang", 0.8f, false);
				});
				sequence.AppendCallback(delegate
				{
					this.disk.SetCoreUnlock(goodness, true);
				});
			}
		}

		// Token: 0x060098BF RID: 39103 RVA: 0x00471C74 File Offset: 0x0046FE74
		public void AppendFogWave(Sequence seq, BuildingBlockData blockData, sbyte orgTemplateId, int previousUnlockAmount)
		{
			sbyte goodness = Organization.Instance[orgTemplateId].Goodness;
			bool unlockNewCore = this.CheckUnlockNewCore(blockData, orgTemplateId);
			CImage fogImage = null;
			bool flag = previousUnlockAmount == 1;
			if (flag)
			{
				fogImage = this.fog1;
			}
			else
			{
				bool flag2 = previousUnlockAmount == 9;
				if (flag2)
				{
					fogImage = this.fog15;
				}
				else
				{
					bool flag3 = unlockNewCore && this.fog5.color.a > 0f;
					if (flag3)
					{
						fogImage = this.fog5;
					}
				}
			}
			bool flag4 = fogImage != null;
			if (flag4)
			{
				fogImage.SetAlpha(1f);
				seq.AppendCallback(delegate
				{
					fogImage.DOFade(0f, this.fogFadeDuration);
				});
			}
			GameObject explosion = this.GetExplosionGo(goodness);
			GameObject basicCover = this.GetBasicBgGo(goodness);
			bool flag5 = unlockNewCore;
			if (flag5)
			{
				seq.AppendCallback(delegate
				{
					basicCover.SetActive(false);
				});
				seq.AppendCallback(delegate
				{
					explosion.SetActive(true);
				});
				seq.AppendCallback(delegate
				{
					explosion.GetComponent<ParticleSystem>().Play(true);
				});
			}
			seq.AppendInterval(1f);
		}

		// Token: 0x060098C0 RID: 39104 RVA: 0x00471DC0 File Offset: 0x0046FFC0
		public void RefreshFogDirect(BuildingBlockData buildingBlockData, int currentUnlockedAmount)
		{
			this.fog1.gameObject.SetActive(false);
			this.fog5.gameObject.SetActive(false);
			this.fog15.gameObject.SetActive(false);
			bool flag = currentUnlockedAmount == 15;
			if (!flag)
			{
				this.fog15.gameObject.SetActive(true);
				bool unlockGood;
				bool unlockEvil;
				bool unlockNeutral;
				this.CheckUnlockCore(buildingBlockData, out unlockGood, out unlockEvil, out unlockNeutral);
				this.GetBasicBgGo(1).SetActive(!unlockGood);
				this.GetBasicBgGo(-1).SetActive(!unlockEvil);
				this.GetBasicBgGo(0).SetActive(!unlockNeutral);
				bool flag2 = unlockGood || unlockEvil || unlockNeutral;
				if (!flag2)
				{
					this.fog5.gameObject.SetActive(true);
					bool flag3 = currentUnlockedAmount == 1;
					if (flag3)
					{
						this.fog1.gameObject.SetActive(true);
						GlobalDomainMethod.Call.InvokeGuidingTrigger(305);
					}
				}
			}
		}

		// Token: 0x060098C1 RID: 39105 RVA: 0x00471EB4 File Offset: 0x004700B4
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

		// Token: 0x060098C2 RID: 39106 RVA: 0x00471F10 File Offset: 0x00470110
		private GameObject GetBasicBgGo(sbyte goodness)
		{
			if (!true)
			{
			}
			GameObject result;
			switch (goodness)
			{
			case -1:
				result = this.bgEvilCover;
				break;
			case 0:
				result = this.bgNeuralCover;
				break;
			case 1:
				result = this.bgGoodCover;
				break;
			default:
				throw new ArgumentOutOfRangeException("goodness", goodness, null);
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x060098C3 RID: 39107 RVA: 0x00471F6C File Offset: 0x0047016C
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

		// Token: 0x060098C4 RID: 39108 RVA: 0x00471FE8 File Offset: 0x004701E8
		private void CheckUnlockCore(BuildingBlockData blockData, out bool unlockGood, out bool unlockEvil, out bool unlockNeutral)
		{
			int good = 0;
			int bad = 0;
			int neutral = 0;
			for (sbyte i = 1; i <= 15; i += 1)
			{
				bool flag = !blockData.SlotIsUnlocked((int)(i - 1));
				if (!flag)
				{
					bool flag2 = Organization.Instance[i].Goodness == 1;
					if (flag2)
					{
						good++;
					}
					else
					{
						bool flag3 = Organization.Instance[i].Goodness == -1;
						if (flag3)
						{
							bad++;
						}
						else
						{
							neutral++;
						}
					}
				}
			}
			unlockGood = (good == 5);
			unlockEvil = (bad == 5);
			unlockNeutral = (neutral == 5);
		}

		// Token: 0x060098C5 RID: 39109 RVA: 0x00472080 File Offset: 0x00470280
		public void RefreshGrass(int index, bool unlocked)
		{
			foreach (int item in this.grassDic[index])
			{
				this.grassRoot.GetChild(item).gameObject.SetActive(!unlocked);
			}
		}

		// Token: 0x060098C6 RID: 39110 RVA: 0x004720CC File Offset: 0x004702CC
		public void Init()
		{
			this.grassDic[4] = new int[]
			{
				0,
				1,
				2,
				3
			};
			this.grassDic[3] = new int[]
			{
				4,
				5,
				6,
				7
			};
			this.grassDic[5] = new int[]
			{
				8,
				9
			};
			this.grassDic[2] = new int[]
			{
				10,
				11
			};
			this.grassDic[1] = new int[]
			{
				12,
				13
			};
			this.grassDic[15] = new int[]
			{
				14,
				15,
				16
			};
			this.grassDic[12] = new int[]
			{
				17,
				18,
				19
			};
			this.grassDic[13] = new int[]
			{
				20,
				21
			};
			this.grassDic[14] = new int[]
			{
				22,
				23
			};
			this.grassDic[11] = new int[]
			{
				24,
				25,
				26
			};
			this.grassDic[11] = new int[]
			{
				24,
				25,
				26
			};
			this.grassDic[6] = new int[]
			{
				27,
				33
			};
			this.grassDic[10] = new int[]
			{
				28,
				34
			};
			this.grassDic[7] = new int[]
			{
				29,
				30
			};
			this.grassDic[9] = new int[]
			{
				31,
				32
			};
			this.grassDic[8] = new int[]
			{
				35
			};
		}

		// Token: 0x060098C7 RID: 39111 RVA: 0x004722B4 File Offset: 0x004704B4
		public void PlayGrassDisappear(sbyte index, float unlockWaitRecoverTime)
		{
			foreach (int item in this.grassDic[(int)index])
			{
				this.grassRoot.GetChild(item).gameObject.SetActive(true);
				CanvasGroup canvasGroup = this.grassRoot.GetChild(item).GetComponent<CanvasGroup>();
				canvasGroup.alpha = 1f;
				this.grassRoot.GetChild(item).GetComponent<CanvasGroup>().DOFade(0f, unlockWaitRecoverTime);
			}
		}

		// Token: 0x060098C8 RID: 39112 RVA: 0x00472338 File Offset: 0x00470538
		public void SwitchBgToAllUnlock()
		{
			this.bgUnlocked.gameObject.SetActive(true);
			this.bgBasic.DOFade(0f, this.UnlockAllFadeDuration);
			this.bgUnlocked.DOFade(1f, this.UnlockAllFadeDuration);
		}

		// Token: 0x060098C9 RID: 39113 RVA: 0x00472386 File Offset: 0x00470586
		public void SetBgStatus(bool allUnlocked)
		{
			this.bgBasic.DOFade((float)(allUnlocked ? 0 : 1), 0f);
			this.bgUnlocked.DOFade((float)(allUnlocked ? 1 : 0), 0f);
		}

		// Token: 0x04007587 RID: 30087
		[SerializeField]
		private EffectPlayer effectPlayer;

		// Token: 0x04007588 RID: 30088
		[SerializeField]
		private BuildingExpandTaiwuVillageDiskComponent disk;

		// Token: 0x04007589 RID: 30089
		[SerializeField]
		private GameObject explosionGood;

		// Token: 0x0400758A RID: 30090
		[SerializeField]
		private GameObject explosionEvil;

		// Token: 0x0400758B RID: 30091
		[SerializeField]
		private GameObject explosionNeutral;

		// Token: 0x0400758C RID: 30092
		[Header("雾气")]
		[SerializeField]
		private CImage fog1;

		// Token: 0x0400758D RID: 30093
		[SerializeField]
		private CImage fog5;

		// Token: 0x0400758E RID: 30094
		[SerializeField]
		private CImage fog15;

		// Token: 0x0400758F RID: 30095
		[Header("杂草")]
		[SerializeField]
		private Transform grassRoot;

		// Token: 0x04007590 RID: 30096
		[Header("背景")]
		[SerializeField]
		private CImage bgBasic;

		// Token: 0x04007591 RID: 30097
		[SerializeField]
		private GameObject bgGoodCover;

		// Token: 0x04007592 RID: 30098
		[SerializeField]
		private GameObject bgNeuralCover;

		// Token: 0x04007593 RID: 30099
		[SerializeField]
		private GameObject bgEvilCover;

		// Token: 0x04007594 RID: 30100
		[SerializeField]
		private CImage bgUnlocked;

		// Token: 0x04007595 RID: 30101
		[Header("参数")]
		[SerializeField]
		private float fogFadeDuration = 0.5f;

		// Token: 0x04007596 RID: 30102
		public float UnlockAllFadeDuration = 3.5f;

		// Token: 0x04007597 RID: 30103
		private IBuildingExpandTaiwuVillageSteleProvider _steleProvider;

		// Token: 0x04007598 RID: 30104
		private Dictionary<int, int[]> grassDic = new Dictionary<int, int[]>();
	}
}
