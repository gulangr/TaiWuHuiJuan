using System;
using System.Collections;
using Config;
using DG.Tweening;
using GameData.Domains.Extra;
using GameData.Domains.Map;
using Spine;
using Spine.Unity;
using TMPro;
using UnityEngine;

namespace Game.Views.Bottom
{
	// Token: 0x02000C2F RID: 3119
	public class CollectResourceItem : MonoBehaviour
	{
		// Token: 0x170010BE RID: 4286
		// (get) Token: 0x06009E6F RID: 40559 RVA: 0x004A1518 File Offset: 0x0049F718
		private int CountFactorType
		{
			get
			{
				return (int)((this._resourceType >= 0) ? ResourceType.Instance[this._resourceType].CollectMultiplier : 1);
			}
		}

		// Token: 0x170010BF RID: 4287
		// (get) Token: 0x06009E70 RID: 40560 RVA: 0x004A153B File Offset: 0x0049F73B
		private int CountFactor0
		{
			get
			{
				return (SingletonObject.getInstance<BasicGameData>().WorldResourceAmountType == 0) ? 2 : 1;
			}
		}

		// Token: 0x06009E71 RID: 40561 RVA: 0x004A1550 File Offset: 0x0049F750
		private void LateUpdate()
		{
			bool flag = this._resourceType < 0;
			if (!flag)
			{
				bool flag2 = this._parent.ShowingResIconCacheList[(int)this._resourceType].Count > 81;
				if (flag2)
				{
					int count = this._parent.ShowingResIconCacheList[(int)this._resourceType].Count / 2;
					for (int i = 0; i < count; i++)
					{
						Rigidbody2D b = this._parent.ShowingResIconCacheList[(int)this._resourceType][i].GetComponent<Rigidbody2D>();
						b.constraints = ((b.attachedColliderCount > 0) ? RigidbodyConstraints2D.FreezePosition : RigidbodyConstraints2D.None);
					}
				}
			}
		}

		// Token: 0x06009E72 RID: 40562 RVA: 0x004A15F4 File Offset: 0x0049F7F4
		public void Init(ViewCollectResource parent)
		{
			this._parent = parent;
		}

		// Token: 0x06009E73 RID: 40563 RVA: 0x004A1600 File Offset: 0x0049F800
		public void Set(CollectResourceResult result)
		{
			this._resourceType = result.ResourceType;
			this._count = result.ResourceCount;
			this._hasItem = (result.ItemDisplayData != null);
			this._maxCount = result.ResourceCount / this.CountFactorType / this.CountFactor0;
			this._stopped = false;
			this.textBg.SetActive(true);
			bool flag = this._maxCount > 10;
			if (flag)
			{
				ViewCollectResource.CollectResourceType currentCollectType = this._parent.CurrentCollectType;
				ViewCollectResource.CollectResourceType collectResourceType = currentCollectType;
				if (collectResourceType != ViewCollectResource.CollectResourceType.Choosy)
				{
					if (collectResourceType != ViewCollectResource.CollectResourceType.SavageSkill)
					{
						this._maxCount = Mathf.Min(210, 10 + (this._maxCount - 10) * 2 / 3);
					}
					else
					{
						this._maxCount = Mathf.Min(40, 10 + (this._maxCount - 10) / 10);
					}
				}
				else
				{
					this._maxCount = Mathf.Min(40, this._maxCount / 1000);
				}
			}
			switch (this._parent.CurrentCollectType)
			{
			case ViewCollectResource.CollectResourceType.Normal:
			case ViewCollectResource.CollectResourceType.SavageSkill:
				this.InitNormal();
				break;
			case ViewCollectResource.CollectResourceType.Choosy:
				this.InitChoosy();
				break;
			case ViewCollectResource.CollectResourceType.Practice:
				this.InitPractice();
				break;
			}
		}

		// Token: 0x06009E74 RID: 40564 RVA: 0x004A172B File Offset: 0x0049F92B
		public void Set(TreasureFindResult result)
		{
			this.textBg.SetActive(false);
			this._digResult = result;
			this.InitTreasure();
		}

		// Token: 0x06009E75 RID: 40565 RVA: 0x004A174C File Offset: 0x0049F94C
		private void InitNormal()
		{
			this.Destroy();
			CanvasGroup canvasGroup = base.GetComponent<CanvasGroup>();
			this._currentCollectLevel = Mathf.Clamp(this._count / 48, 1, 3);
			string path = string.Format("RemakeResources/Prefab/Legacy/Core/Gather/gather_{0}_{1}", this._resourceTypeNames[(int)this._resourceType], this._currentCollectLevel);
			this.workDesc.text = LocalStringManager.Get(string.Format("LK_Resource_Collect_Title_{0}", this._resourceType));
			this.resourceCount.text = 0.ToString();
			canvasGroup.DOKill(false);
			canvasGroup.alpha = 0f;
			ResLoader.Load<GameObject>(path, delegate(GameObject prefab)
			{
				this._animationObj = Object.Instantiate<GameObject>(prefab, this.animationPosition);
				foreach (SkeletonGraphic graphic in this._animationObj.GetComponentsInChildren<SkeletonGraphic>())
				{
					bool flag = graphic.skeletonDataAsset == null;
					if (flag)
					{
						graphic.enabled = false;
					}
					else
					{
						graphic.color = graphic.color.SetAlpha(0f);
					}
				}
			}, null, false);
		}

		// Token: 0x06009E76 RID: 40566 RVA: 0x004A1808 File Offset: 0x0049FA08
		private void InitChoosy()
		{
			int level = Mathf.Clamp(this._count / GlobalConfig.Instance.ChoosyResourceBaseCost / 10, 1, 3);
			bool flag = level == this._currentCollectLevel;
			if (!flag)
			{
				this.Destroy();
				this._currentCollectLevel = level;
				string path = string.Format("RemakeResources/Prefab/Legacy/Core/Gather/gather_{0}_{1}", this._resourceTypeNames[(int)this._resourceType], this._currentCollectLevel);
				this.workDesc.text = ResourceType.Instance[this._resourceType].Name;
				this.resourceCount.text = this._count.ToString();
				ResLoader.Load<GameObject>(path, delegate(GameObject prefab)
				{
					this._animationObj = Object.Instantiate<GameObject>(prefab, this.animationPosition);
					foreach (SkeletonGraphic graphic in this._animationObj.GetComponentsInChildren<SkeletonGraphic>())
					{
						bool flag2 = graphic.skeletonDataAsset == null;
						if (flag2)
						{
							graphic.enabled = false;
						}
						graphic.startingLoop = false;
						graphic.AnimationState.SetEmptyAnimations(0f);
					}
				}, null, false);
			}
		}

		// Token: 0x06009E77 RID: 40567 RVA: 0x004A18C0 File Offset: 0x0049FAC0
		public void InitPractice()
		{
			this.Destroy();
			CanvasGroup canvasGroup = base.GetComponent<CanvasGroup>();
			canvasGroup.DOKill(false);
			canvasGroup.alpha = 0f;
			this.workDesc.text = LanguageKey.LK_PracticeCombatSkill_AnimationName.Tr();
			this.resourceCount.text = 0.ToString();
			ResLoader.Load<GameObject>("RemakeResources/Prefab/Legacy/Core/PracticeCombatSkill/practiceCombatSkill", delegate(GameObject prefab)
			{
				this._animationObj = Object.Instantiate<GameObject>(prefab, this.animationPosition);
				foreach (SkeletonGraphic graphic in this._animationObj.GetComponentsInChildren<SkeletonGraphic>())
				{
					bool flag = graphic.skeletonDataAsset == null;
					if (flag)
					{
						graphic.enabled = false;
					}
					else
					{
						graphic.color = graphic.color.SetAlpha(0f);
					}
				}
				AudioManager.Instance.PlaySound("ui_training", false, false);
			}, null, false);
		}

		// Token: 0x06009E78 RID: 40568 RVA: 0x004A1934 File Offset: 0x0049FB34
		public void InitTreasure()
		{
			this.Destroy();
			this.workDesc.text = LanguageKey.LK_PracticeCombatSkill_AnimationName.Tr();
			this.resourceCount.text = "";
			ResLoader.Load<GameObject>("RemakeResources/Prefab/Legacy/Core/Gather/digging_" + (this._digResult.AnyResource ? "nothing_coin" : (this._digResult.Success ? "getting" : "nothing")), delegate(GameObject prefab)
			{
				this._animationObj = Object.Instantiate<GameObject>(prefab, this.animationPosition);
				foreach (SkeletonGraphic player in this._animationObj.GetComponentsInChildren<SkeletonGraphic>())
				{
					bool flag = player.skeletonDataAsset == null;
					if (flag)
					{
						player.enabled = false;
					}
				}
				AudioManager.Instance.PlaySound("ui_collect_coins", false, false);
			}, null, false);
		}

		// Token: 0x06009E79 RID: 40569 RVA: 0x004A19BB File Offset: 0x0049FBBB
		public IEnumerator DropItem()
		{
			int itemIndex = this._hasItem ? Math.Max(0, this._maxCount - 4) : -1;
			float resCollectTime = this._hasItem ? 2f : 4f;
			float generateGap = Mathf.Min(resCollectTime / (float)Math.Max(this._maxCount - 1, 1), 0.3f);
			WaitForSeconds wait = new WaitForSeconds(generateGap);
			yield return wait;
			bool flag = this._parent.CurrentCollectType != ViewCollectResource.CollectResourceType.Choosy;
			if (flag)
			{
				this.PlayNormalAnim();
			}
			int num;
			for (int i = 0; i < this._maxCount; i = num)
			{
				GameObject dropIcon = this._parent.GetDropIcon(i == itemIndex, this._resourceType);
				dropIcon.transform.localScale = ((this._parent.CurrentCollectType == ViewCollectResource.CollectResourceType.SavageSkill) ? new Vector3(0.5f, 0.5f, 0.5f) : new Vector3(0.8f, 0.8f, 0.8f));
				dropIcon.transform.SetParent(this.iconHolder, false);
				dropIcon.transform.localPosition = new Vector3((float)Random.Range(-50, 50), (float)(200 + i), 0f);
				Rigidbody2D rigidBody = dropIcon.GetComponent<Rigidbody2D>();
				rigidBody.constraints = RigidbodyConstraints2D.None;
				rigidBody.AddForce((i == itemIndex) ? new Vector2(Random.Range(-50f, 50f), 0f) : new Vector2(Random.Range(-10f, 10f), 0f));
				dropIcon.gameObject.SetActive(true);
				yield return wait;
				bool flag2 = !this._stopped && this._parent.CurrentCollectType != ViewCollectResource.CollectResourceType.Choosy;
				if (flag2)
				{
					this.resourceCount.text = ((this._maxCount == 0) ? 0L : ((long)this._count * (long)i / (long)this._maxCount)).ToString();
				}
				dropIcon = null;
				rigidBody = null;
				num = i + 1;
			}
			yield return wait;
			this.Stop();
			this.resourceCount.text = this._count.ToString();
			yield return new WaitForSeconds(generateGap + 0.3f);
			this._parent.SetIsCollecting(false);
			yield break;
		}

		// Token: 0x06009E7A RID: 40570 RVA: 0x004A19CA File Offset: 0x0049FBCA
		public void SetCountText()
		{
			this.resourceCount.text = this._count.ToString();
		}

		// Token: 0x06009E7B RID: 40571 RVA: 0x004A19E4 File Offset: 0x0049FBE4
		public void Hide()
		{
			CanvasGroup canvasGroup = base.GetComponent<CanvasGroup>();
			SkeletonGraphic[] skeletonGraphics = this.animationPosition.GetComponentsInChildren<SkeletonGraphic>();
			foreach (SkeletonGraphic graphic in skeletonGraphics)
			{
				graphic.DOKill(false);
				graphic.DOFade(0f, 0.3f);
			}
			canvasGroup.DOKill(false);
			canvasGroup.DOFade(0f, 0.3f);
		}

		// Token: 0x06009E7C RID: 40572 RVA: 0x004A1A54 File Offset: 0x0049FC54
		public void Stop()
		{
			bool stopped = this._stopped;
			if (!stopped)
			{
				this._stopped = true;
				this.resourceCount.text = this._count.ToString();
				SkeletonGraphic[] componentsInChildren = this._animationObj.GetComponentsInChildren<SkeletonGraphic>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					SkeletonGraphic graphic = componentsInChildren[i];
					TrackEntry track = graphic.AnimationState.GetCurrent(0);
					bool flag = track != null;
					if (flag)
					{
						track.Complete += delegate(TrackEntry _)
						{
							Spine.AnimationState animationState = graphic.AnimationState;
							if (animationState != null)
							{
								animationState.ClearTrack(0);
							}
						};
					}
				}
			}
		}

		// Token: 0x06009E7D RID: 40573 RVA: 0x004A1AEC File Offset: 0x0049FCEC
		public void Destroy()
		{
			this._currentCollectLevel = -1;
			bool flag = this._animationObj != null;
			if (flag)
			{
				Object.Destroy(this._animationObj);
				this._animationObj = null;
			}
		}

		// Token: 0x06009E7E RID: 40574 RVA: 0x004A1B28 File Offset: 0x0049FD28
		private void PlayNormalAnim()
		{
			for (int i = 0; i < this.animationPosition.childCount; i++)
			{
				Transform root = this.animationPosition.GetChild(i);
				bool flag = !root.gameObject.activeSelf;
				if (!flag)
				{
					this.PlayCollectAudio();
					SkeletonGraphic[] skeletonGraphics = this.animationPosition.GetComponentsInChildren<SkeletonGraphic>();
					foreach (SkeletonGraphic graphic in skeletonGraphics)
					{
						graphic.color = graphic.color.SetAlpha(0f);
						graphic.DOKill(false);
						graphic.DOFade(1f, 0.3f);
						graphic.AnimationState.SetAnimation(0, "animation", true);
					}
				}
			}
			base.GetComponent<CanvasGroup>().DOFade(1f, 0.3f);
		}

		// Token: 0x06009E7F RID: 40575 RVA: 0x004A1C0C File Offset: 0x0049FE0C
		public float PlayChoosyAnim()
		{
			for (int i = 0; i < this.animationPosition.childCount; i++)
			{
				Transform root = this.animationPosition.GetChild(i);
				bool flag = !root.gameObject.activeSelf;
				if (!flag)
				{
					this.PlayCollectAudio();
					SkeletonGraphic[] skeletonGraphics = this.animationPosition.GetComponentsInChildren<SkeletonGraphic>();
					foreach (SkeletonGraphic graphic in skeletonGraphics)
					{
						graphic.AnimationState.SetAnimation(0, "animation", true);
					}
				}
			}
			return 2f;
		}

		// Token: 0x06009E80 RID: 40576 RVA: 0x004A1CAC File Offset: 0x0049FEAC
		public void PlayTreasureAnim(bool isDigSeries)
		{
			CanvasGroup canvasGroup = base.GetComponent<CanvasGroup>();
			bool flag = Mathf.Approximately(canvasGroup.alpha, 1f) && isDigSeries;
			if (!flag)
			{
				SkeletonGraphic[] skeletonGraphics = this.animationPosition.GetComponentsInChildren<SkeletonGraphic>();
				foreach (SkeletonGraphic player in skeletonGraphics)
				{
					player.color = player.color.SetAlpha(0f);
					player.DOKill(false);
					player.DOFade(1f, 0.3f);
				}
				canvasGroup.DOFade(1f, 0.3f);
			}
		}

		// Token: 0x06009E81 RID: 40577 RVA: 0x004A1D48 File Offset: 0x0049FF48
		private void PlayCollectAudio()
		{
			bool flag = !CollectResourceItem._collectAudioResInfos.CheckIndex((int)this._resourceType);
			if (!flag)
			{
				string preparePlayAudio = "";
				string text = this._resourceTypeNames[(int)this._resourceType];
				string text2 = text;
				uint num = <PrivateImplementationDetails>.ComputeStringHash(text2);
				if (num <= 1743038524U)
				{
					if (num != 768543721U)
					{
						if (num != 1028682697U)
						{
							if (num != 1743038524U)
							{
								goto IL_183;
							}
							if (!(text2 == "fabric"))
							{
								goto IL_183;
							}
						}
						else if (!(text2 == "food"))
						{
							goto IL_183;
						}
					}
					else if (!(text2 == "jewelry"))
					{
						goto IL_183;
					}
				}
				else if (num <= 2226448744U)
				{
					if (num != 1783197767U)
					{
						if (num != 2226448744U)
						{
							goto IL_183;
						}
						if (!(text2 == "wood"))
						{
							goto IL_183;
						}
					}
					else if (!(text2 == "herbal"))
					{
						goto IL_183;
					}
				}
				else if (num != 2585652531U)
				{
					if (num != 3780168015U)
					{
						goto IL_183;
					}
					if (!(text2 == "money"))
					{
						goto IL_183;
					}
					preparePlayAudio = CollectResourceItem._collectAudioResInfos[(int)this._resourceType];
					goto IL_183;
				}
				else if (!(text2 == "ore"))
				{
					goto IL_183;
				}
				string audioRes = CollectResourceItem._collectAudioResInfos[(int)this._resourceType];
				int currentCollectLevel = this._currentCollectLevel;
				int num2 = currentCollectLevel;
				if (num2 - 1 > 2)
				{
					preparePlayAudio = string.Format("{0}_{1}", audioRes, 1);
				}
				else
				{
					preparePlayAudio = string.Format("{0}_{1}", audioRes, this._currentCollectLevel);
				}
				IL_183:
				bool flag2 = string.IsNullOrEmpty(preparePlayAudio);
				if (!flag2)
				{
					bool flag3 = AudioManager.Instance.IsPlayingSound(preparePlayAudio);
					if (!flag3)
					{
						Debug.Log(string.Format("Collect Resource Audio: {0} CollectCount: {1} CollectLevel:{2}", preparePlayAudio, this._count, this._currentCollectLevel));
						AudioManager.Instance.PlaySound(preparePlayAudio, false, false);
					}
				}
			}
		}

		// Token: 0x04007A86 RID: 31366
		public GameObject textBg;

		// Token: 0x04007A87 RID: 31367
		public TextMeshProUGUI workDesc;

		// Token: 0x04007A88 RID: 31368
		public TextMeshProUGUI resourceCount;

		// Token: 0x04007A89 RID: 31369
		public Transform iconHolder;

		// Token: 0x04007A8A RID: 31370
		public Transform animationPosition;

		// Token: 0x04007A8B RID: 31371
		private ViewCollectResource _parent;

		// Token: 0x04007A8C RID: 31372
		private sbyte _resourceType;

		// Token: 0x04007A8D RID: 31373
		private int _maxCount;

		// Token: 0x04007A8E RID: 31374
		private int _count;

		// Token: 0x04007A8F RID: 31375
		private bool _hasItem;

		// Token: 0x04007A90 RID: 31376
		private TreasureFindResult _digResult;

		// Token: 0x04007A91 RID: 31377
		private bool _stopped;

		// Token: 0x04007A92 RID: 31378
		private GameObject _animationObj;

		// Token: 0x04007A93 RID: 31379
		private int _currentCollectLevel = -1;

		// Token: 0x04007A94 RID: 31380
		private readonly string[] _resourceTypeNames = new string[]
		{
			"food",
			"wood",
			"ore",
			"jewelry",
			"fabric",
			"herbal",
			"money"
		};

		// Token: 0x04007A95 RID: 31381
		private static readonly string[] _collectAudioResInfos = new string[]
		{
			"ui_collect_fhs",
			"ui_gather_wood",
			"ui_gather_ore",
			"ui_gather_jewelry",
			"ui_collect_fhs",
			"ui_collect_fhs",
			"ui_collect_coins"
		};

		// Token: 0x04007A96 RID: 31382
		private const string CollectAnimPrefabPathBase = "RemakeResources/Prefab/Legacy/Core/Gather/gather_{0}_{1}";

		// Token: 0x04007A97 RID: 31383
		private const string PracticeAnimPath = "RemakeResources/Prefab/Legacy/Core/PracticeCombatSkill/practiceCombatSkill";

		// Token: 0x04007A98 RID: 31384
		private const float CollectAniTimeBase = 2f;

		// Token: 0x04007A99 RID: 31385
		private const float GapTime = 0.3f;

		// Token: 0x04007A9A RID: 31386
		private const float FadeTime = 0.3f;

		// Token: 0x04007A9B RID: 31387
		private const int MaxDropResourceIconCount = 210;

		// Token: 0x04007A9C RID: 31388
		private const int MaxDropResourceIconCountSkill = 40;

		// Token: 0x04007A9D RID: 31389
		private const int FreezeCount = 81;

		// Token: 0x04007A9E RID: 31390
		private const float SavageSkillSize = 0.5f;

		// Token: 0x04007A9F RID: 31391
		private const float NormalSize = 0.8f;

		// Token: 0x04007AA0 RID: 31392
		private const int CountFactor1 = 10;

		// Token: 0x04007AA1 RID: 31393
		private const int CountFactor2 = 1000;

		// Token: 0x04007AA2 RID: 31394
		private const int CountFactor3 = 10;

		// Token: 0x04007AA3 RID: 31395
		private const int CountFactor4 = 3;

		// Token: 0x04007AA4 RID: 31396
		private const int CountFactor5 = 48;

		// Token: 0x04007AA5 RID: 31397
		private const int CountFactor6 = 10;

		// Token: 0x04007AA6 RID: 31398
		private const int CountFactor7 = 1;

		// Token: 0x04007AA7 RID: 31399
		private const int CountFactor8 = 3;

		// Token: 0x04007AA8 RID: 31400
		private const int CountFactor9 = 4;

		// Token: 0x04007AA9 RID: 31401
		private const int PositionMin = -50;

		// Token: 0x04007AAA RID: 31402
		private const int PositionMax = 50;

		// Token: 0x04007AAB RID: 31403
		private const int PositionY = 200;

		// Token: 0x04007AAC RID: 31404
		private const float ItemForce = 50f;

		// Token: 0x04007AAD RID: 31405
		private const float ResourceForce = 10f;

		// Token: 0x02002351 RID: 9041
		private static class ResourceTypeName
		{
			// Token: 0x0400DE78 RID: 56952
			public const string Food = "food";

			// Token: 0x0400DE79 RID: 56953
			public const string Wood = "wood";

			// Token: 0x0400DE7A RID: 56954
			public const string Ore = "ore";

			// Token: 0x0400DE7B RID: 56955
			public const string Jewelry = "jewelry";

			// Token: 0x0400DE7C RID: 56956
			public const string Fabric = "fabric";

			// Token: 0x0400DE7D RID: 56957
			public const string Herbal = "herbal";

			// Token: 0x0400DE7E RID: 56958
			public const string Money = "money";
		}

		// Token: 0x02002352 RID: 9042
		private static class CollectLevel
		{
			// Token: 0x0400DE7F RID: 56959
			public const int Many = 3;

			// Token: 0x0400DE80 RID: 56960
			public const int Medium = 2;

			// Token: 0x0400DE81 RID: 56961
			public const int Little = 1;
		}
	}
}
