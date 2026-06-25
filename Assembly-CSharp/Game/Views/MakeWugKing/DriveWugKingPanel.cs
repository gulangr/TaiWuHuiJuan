using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Coffee.UIExtensions;
using Config;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Story;
using GameData.Serializer;
using GameData.Utilities;
using Spine.Unity;
using TMPro;
using UnityEngine;

namespace Game.Views.MakeWugKing
{
	// Token: 0x02000947 RID: 2375
	public class DriveWugKingPanel : MonoBehaviour
	{
		// Token: 0x17000CD2 RID: 3282
		// (get) Token: 0x06006FF8 RID: 28664 RVA: 0x0033D6FE File Offset: 0x0033B8FE
		public sbyte SelectedWugKingType
		{
			get
			{
				return this._selectedWugKingType;
			}
		}

		// Token: 0x17000CD3 RID: 3283
		// (get) Token: 0x06006FF9 RID: 28665 RVA: 0x0033D706 File Offset: 0x0033B906
		private sbyte SelectingDriveType
		{
			get
			{
				return (this.driveTypeToggleGroup.GetActiveIndex() == 0) ? 1 : 2;
			}
		}

		// Token: 0x06006FFA RID: 28666 RVA: 0x0033D719 File Offset: 0x0033B919
		private void Awake()
		{
			this.EnsureDriveTargetContentRootCached();
		}

		// Token: 0x06006FFB RID: 28667 RVA: 0x0033D724 File Offset: 0x0033B924
		private void EnsureDriveTargetContentRootCached()
		{
			bool flag = this.driveTargetContentRoot != null || this.avatarTarget == null;
			if (!flag)
			{
				this._cachedDriveTargetContentRoot = ((this.avatarTarget.transform as RectTransform) ?? (this.avatarTarget.transform.parent as RectTransform));
			}
		}

		// Token: 0x17000CD4 RID: 3284
		// (get) Token: 0x06006FFC RID: 28668 RVA: 0x0033D784 File Offset: 0x0033B984
		private RectTransform ResolvedDriveTargetContentRoot
		{
			get
			{
				return (this.driveTargetContentRoot != null) ? this.driveTargetContentRoot : this._cachedDriveTargetContentRoot;
			}
		}

		// Token: 0x06006FFD RID: 28669 RVA: 0x0033D7A4 File Offset: 0x0033B9A4
		private void SetDriveTargetDisplayLoading(bool loading)
		{
			if (loading)
			{
				bool isDriveTargetDisplayLoading = this._isDriveTargetDisplayLoading;
				if (!isDriveTargetDisplayLoading)
				{
					this.SetDriveTargetDisplayLoadingImmediate(true);
				}
			}
			else
			{
				bool flag = !this._isDriveTargetDisplayLoading;
				if (!flag)
				{
					this.SetDriveTargetDisplayLoadingImmediate(false);
				}
			}
		}

		// Token: 0x06006FFE RID: 28670 RVA: 0x0033D7E8 File Offset: 0x0033B9E8
		private void SetDriveTargetDisplayLoadingImmediate(bool loading)
		{
			RectTransform root = this.ResolvedDriveTargetContentRoot;
			if (loading)
			{
				this._isDriveTargetDisplayLoading = true;
				bool flag = root != null;
				if (flag)
				{
					this._driveTargetContentRootOriginalPos = root.anchoredPosition;
					root.anchoredPosition = new Vector2(10000f, 10000f);
					this._driveTargetDisplayMovedOffscreen = true;
				}
				else
				{
					this._driveTargetDisplayMovedOffscreen = false;
				}
				bool flag2 = this.driveTargetAreaLoading != null;
				if (flag2)
				{
					this.driveTargetAreaLoading.gameObject.SetActive(true);
				}
			}
			else
			{
				this._isDriveTargetDisplayLoading = false;
				bool flag3 = this._driveTargetDisplayMovedOffscreen && root != null;
				if (flag3)
				{
					root.anchoredPosition = this._driveTargetContentRootOriginalPos;
					this._driveTargetDisplayMovedOffscreen = false;
				}
				bool flag4 = this.driveTargetAreaLoading != null;
				if (flag4)
				{
					this.driveTargetAreaLoading.gameObject.SetActive(false);
				}
			}
		}

		// Token: 0x06006FFF RID: 28671 RVA: 0x0033D8C8 File Offset: 0x0033BAC8
		private void Update()
		{
		}

		// Token: 0x06007000 RID: 28672 RVA: 0x0033D8CC File Offset: 0x0033BACC
		private void Init()
		{
			bool inited = this._inited;
			if (!inited)
			{
				this._inited = true;
				this.driveTypeToggleGroup.Init(-1);
				this.driveTypeToggleGroup.OnActiveIndexChange += delegate(int active, int inactive)
				{
					this.RefreshEffectDesc();
					this.PlaySwitchBg(this.SelectingDriveType == 1);
					this.RefreshDescInfo();
				};
				this.btnDrive.ClearAndAddListener(new Action(this.OnClickDriveButton));
			}
		}

		// Token: 0x06007001 RID: 28673 RVA: 0x0033D92A File Offset: 0x0033BB2A
		public void Setup(ViewMakeWugKing parent)
		{
			this.Init();
			this._parent = parent;
		}

		// Token: 0x06007002 RID: 28674 RVA: 0x0033D93B File Offset: 0x0033BB3B
		public void OnInit()
		{
			this.Init();
			this._selectedWugKingType = -1;
			this.InitCostDisplay();
		}

		// Token: 0x06007003 RID: 28675 RVA: 0x0033D953 File Offset: 0x0033BB53
		public void OnGotWugKingDriveDisplayDatas()
		{
			this.RefreshUI();
		}

		// Token: 0x06007004 RID: 28676 RVA: 0x0033D95D File Offset: 0x0033BB5D
		private void RefreshUI()
		{
			this.RefreshEatingWugKingGrid();
			this.RefreshDriveButtonState();
			this.RefreshDescInfo();
			this.RefreshEffectDesc();
		}

		// Token: 0x06007005 RID: 28677 RVA: 0x0033D97C File Offset: 0x0033BB7C
		private void RefreshEatingWugKingGrid()
		{
			bool flag = this._parent.WugKingDriveDisplayDatas.Count == 0;
			if (flag)
			{
				this._selectedWugKingType = -1;
			}
			else
			{
				this._selectedWugKingType = this._parent.WugKingDriveDisplayDatas[0].WugType;
				this.mouseTip.Type = TipType.Medicine;
				TooltipInvoker tooltipInvoker = this.mouseTip;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = new ArgumentBox();
				}
				this.mouseTip.RuntimeParam.SetObject("ItemData", this._parent.WugKingDriveDisplayDatas[0].ItemDisplayData);
				this.mouseTip.RuntimeParam.Set("CharId", this._parent.WugKingDriveDisplayDatas[0].CharacterId);
			}
		}

		// Token: 0x06007006 RID: 28678 RVA: 0x0033DA4C File Offset: 0x0033BC4C
		private void RefreshEffectDesc()
		{
			bool flag = this._selectedWugKingType == -1;
			if (flag)
			{
				this.effectDescLabel.text = "";
			}
			else
			{
				bool flag2 = this.SelectingDriveType == 1;
				LanguageKey key;
				if (flag2)
				{
					this.driveTypeName.text = LocalStringManager.Get(LanguageKey.LK_DriveWugKing_Type_Positive);
					sbyte selectedWugKingType = this._selectedWugKingType;
					if (!true)
					{
					}
					LanguageKey languageKey;
					switch (selectedWugKingType)
					{
					case 0:
						languageKey = LanguageKey.LK_DriveWugKing_Effect_Desc_Positive_0;
						break;
					case 1:
						languageKey = LanguageKey.LK_DriveWugKing_Effect_Desc_Positive_1;
						break;
					case 2:
						languageKey = LanguageKey.LK_DriveWugKing_Effect_Desc_Positive_2;
						break;
					case 3:
						languageKey = LanguageKey.LK_DriveWugKing_Effect_Desc_Positive_3;
						break;
					case 4:
						languageKey = LanguageKey.LK_DriveWugKing_Effect_Desc_Positive_4;
						break;
					case 5:
						languageKey = LanguageKey.LK_DriveWugKing_Effect_Desc_Positive_5;
						break;
					case 6:
						languageKey = LanguageKey.LK_DriveWugKing_Effect_Desc_Positive_6;
						break;
					case 7:
						languageKey = LanguageKey.LK_DriveWugKing_Effect_Desc_Positive_7;
						break;
					default:
						throw new ArgumentOutOfRangeException("_selectedWugKingType", "Invalid WugType");
					}
					if (!true)
					{
					}
					key = languageKey;
				}
				else
				{
					this.driveTypeName.text = LocalStringManager.Get(LanguageKey.LK_DriveWugKing_Type_Negative);
					sbyte selectedWugKingType2 = this._selectedWugKingType;
					if (!true)
					{
					}
					LanguageKey languageKey;
					switch (selectedWugKingType2)
					{
					case 0:
						languageKey = LanguageKey.LK_DriveWugKing_Effect_Desc_Negative_0;
						break;
					case 1:
						languageKey = LanguageKey.LK_DriveWugKing_Effect_Desc_Negative_1;
						break;
					case 2:
						languageKey = LanguageKey.LK_DriveWugKing_Effect_Desc_Negative_2;
						break;
					case 3:
						languageKey = LanguageKey.LK_DriveWugKing_Effect_Desc_Negative_3;
						break;
					case 4:
						languageKey = LanguageKey.LK_DriveWugKing_Effect_Desc_Negative_4;
						break;
					case 5:
						languageKey = LanguageKey.LK_DriveWugKing_Effect_Desc_Negative_5;
						break;
					case 6:
						languageKey = LanguageKey.LK_DriveWugKing_Effect_Desc_Negative_6;
						break;
					case 7:
						languageKey = LanguageKey.LK_DriveWugKing_Effect_Desc_Negative_7;
						break;
					default:
						throw new ArgumentOutOfRangeException("_selectedWugKingType", "Invalid WugType");
					}
					if (!true)
					{
					}
					key = languageKey;
				}
				bool flag3 = this.SelectingDriveType == 1;
				if (flag3)
				{
					this.effectDescLabel.text = key.Tr().SetColor("8dc3c3");
				}
				else
				{
					this.effectDescLabel.text = key.Tr().SetColor("brightred");
				}
			}
		}

		// Token: 0x06007007 RID: 28679 RVA: 0x0033DC28 File Offset: 0x0033BE28
		public void OnClickDriveButton()
		{
			bool flag = this._selectedWugKingType < 0;
			if (!flag)
			{
				WugKingDriveDisplayData selectedData = this._parent.WugKingDriveDisplayDatas.Find((WugKingDriveDisplayData d) => d.WugType == this._selectedWugKingType);
				bool flag2 = selectedData == null || !selectedData.CanDrive;
				if (!flag2)
				{
					DialogCmd cmd = new DialogCmd
					{
						Title = LanguageKey.LK_DriveWugKing_Alert_Title.Tr(),
						Content = LanguageKey.LK_DriveWugKing_Alert_Content.Tr(),
						Yes = new Action(this.<OnClickDriveButton>g__Action|60_1)
					};
					UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
					UIManager.Instance.MaskUI(UIElement.Dialog);
				}
			}
		}

		// Token: 0x06007008 RID: 28680 RVA: 0x0033DCE0 File Offset: 0x0033BEE0
		private void PlayDriveSuccessEffect()
		{
			this.btnDrive.interactable = false;
			Sequence sequence = DOTween.Sequence();
			sequence.AppendCallback(delegate
			{
				this.effDriveLine.gameObject.SetActive(true);
				this.effDriveLine.Play();
			});
			sequence.AppendInterval(1.8f);
			sequence.AppendCallback(delegate
			{
				this.effDriveLine.gameObject.SetActive(false);
			});
			sequence.Play<Sequence>();
			DOVirtual.DelayedCall(0.2f, delegate
			{
				this.RefreshUI();
				this._parent.RequestWugKingDriveStatuses();
			}, true);
		}

		// Token: 0x06007009 RID: 28681 RVA: 0x0033DD54 File Offset: 0x0033BF54
		private void RefreshDriveButtonState()
		{
			bool canDrive = false;
			bool flag = this._selectedWugKingType >= 0;
			if (flag)
			{
				WugKingDriveDisplayData selectedData = this._parent.WugKingDriveDisplayDatas.Find((WugKingDriveDisplayData d) => d.WugType == this._selectedWugKingType);
				canDrive = (selectedData != null && selectedData.CanDrive);
				int remainMonths = this.CoolDownRemain(selectedData.StartDate, 6);
				canDrive &= (remainMonths <= 0);
			}
			this.btnDrive.interactable = (canDrive && this._hasEnoughResource && this._hasEnoughActionPoint);
			TooltipInvoker tip = this.btnDrive.GetComponent<TooltipInvoker>();
			tip.enabled = !this.btnDrive.interactable;
			bool enabled = tip.enabled;
			if (enabled)
			{
				TooltipInvoker tooltipInvoker = tip;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = new ArgumentBox();
				}
				this._sbCached.Clear();
				bool flag2 = this._selectedWugKingType < 0;
				if (flag2)
				{
					this._sbCached.AppendLine(LanguageKey.LK_DriveWugKing_DisableReason_NoSelection.Tr());
				}
				else
				{
					bool flag3 = !canDrive;
					if (flag3)
					{
						this._sbCached.AppendLine(LanguageKey.LK_MakeWugKing_CoolingDown.Tr());
					}
				}
				bool flag4 = !this._hasEnoughResource;
				if (flag4)
				{
					this._sbCached.AppendLine(LanguageKey.LK_DriveWugKing_DisableReason_NoResource.Tr());
				}
				bool flag5 = !this._hasEnoughActionPoint;
				if (flag5)
				{
					this._sbCached.AppendLine(LanguageKey.LK_DriveWugKing_DisableReason_NoActionPoint.Tr());
				}
				tip.RuntimeParam.Set("arg0", this._sbCached.ToString());
			}
		}

		// Token: 0x0600700A RID: 28682 RVA: 0x0033DEEC File Offset: 0x0033C0EC
		private void RefreshDescInfo()
		{
			this.wugLayout.SetActive(this._selectedWugKingType >= 0);
			bool flag = this._selectedWugKingType >= 0;
			if (flag)
			{
				WugKingDriveDisplayData selectedData = this._parent.WugKingDriveDisplayDatas.Find((WugKingDriveDisplayData d) => d.WugType == this._selectedWugKingType);
				bool flag2 = selectedData != null && selectedData.CanDrive;
				int remainMonths = this.CoolDownRemain(selectedData.StartDate, 6);
				int displayRemainMonths = Mathf.Clamp(remainMonths, 0, remainMonths);
				this.cdContent.text = LocalStringManager.GetFormat(LanguageKey.LK_MakeWugKing_RemainMonth, displayRemainMonths);
				this.cdContent.GetComponent<TMPTextSpriteHelper>().Parse();
				MedicineItem medConfig = Medicine.Instance[selectedData.ItemDisplayData.Key.TemplateId];
				WugKingItem wugConfig = WugKing.Instance[medConfig.WugType];
				this.wugName.text = medConfig.Name;
				this.skelWug.Skeleton.SetSkin(this._wugKingSkinName[wugConfig.TemplateId]);
				this.skelWug.Skeleton.SetToSetupPose();
				this.skelWug.Update(0f);
				this.SetWugKinAnimation((remainMonths > 0) ? DriveWugKingPanel.EWugKingAnimationType.Cooldown : DriveWugKingPanel.EWugKingAnimationType.CanDrive);
			}
		}

		// Token: 0x0600700B RID: 28683 RVA: 0x0033E030 File Offset: 0x0033C230
		private void SetWugKinAnimation(DriveWugKingPanel.EWugKingAnimationType eWugKingAnimationType)
		{
			string animationName = "idle";
			bool flag = eWugKingAnimationType == DriveWugKingPanel.EWugKingAnimationType.CanDrive;
			if (flag)
			{
				animationName = "activation";
			}
			else
			{
				bool flag2 = eWugKingAnimationType == DriveWugKingPanel.EWugKingAnimationType.Driving;
				if (flag2)
				{
					animationName = "act";
				}
			}
			this.skelWug.AnimationState.SetAnimation(0, animationName, true);
		}

		// Token: 0x0600700C RID: 28684 RVA: 0x0033E076 File Offset: 0x0033C276
		public void SetOwnedResourceCount(int count)
		{
			this._ownedResourceCount = count;
			this.RefreshCostDisplay();
		}

		// Token: 0x0600700D RID: 28685 RVA: 0x0033E088 File Offset: 0x0033C288
		public void RefreshCostDisplay()
		{
			int costResourceCount = GlobalConfig.Instance.WuxianDriveWugKingCostResourceCount;
			short costActionPoint = GlobalConfig.Instance.WuxianDriveWugKingCostActionPoint;
			this._hasEnoughResource = (this._ownedResourceCount >= costResourceCount);
			int currentActionPoint = SingletonObject.getInstance<BasicGameData>().ActionPointCurrMonth;
			this._hasEnoughActionPoint = SingletonObject.getInstance<TimeManager>().IsActionPointEnough((int)costActionPoint);
			bool flag = this.resourceLabel != null;
			if (flag)
			{
				string resourceColor = this._hasEnoughResource ? "brightblue" : "brightred";
				this.resourceLabel.SetText(CommonUtils.GetDisplayStringForNum(this._ownedResourceCount, 100000).SetColor(resourceColor) + "/" + CommonUtils.GetDisplayStringForNum(costResourceCount, 100000), true);
			}
			bool flag2 = this.actionPointLabel != null;
			if (flag2)
			{
				string actionColor = this._hasEnoughActionPoint ? "brightblue" : "brightred";
				this.actionPointLabel.SetText(CommonUtils.GetDisplayStringForNum(currentActionPoint / 10, 100000).SetColor(actionColor) + "/" + CommonUtils.GetDisplayStringForNum((int)(costActionPoint / 10), 100000), true);
			}
			this.RefreshDriveButtonState();
		}

		// Token: 0x0600700E RID: 28686 RVA: 0x0033E1A8 File Offset: 0x0033C3A8
		private void InitCostDisplay()
		{
			sbyte costResourceType = GlobalConfig.Instance.WuxianDriveWugKingCostResourceType;
			bool flag = this.costResourceIcon != null;
			if (flag)
			{
				this.costResourceIcon.SetSprite(string.Format("mousetip_ziyuan_{0}", costResourceType), false, null);
			}
			this.RefreshCostDisplay();
		}

		// Token: 0x0600700F RID: 28687 RVA: 0x0033E1F8 File Offset: 0x0033C3F8
		public void UpdateDriveResourceDisplay()
		{
			sbyte resourceType = GlobalConfig.Instance.WuxianDriveWugKingCostResourceType;
			int ownedCount = this._parent.TaiwuResources.Get((int)resourceType);
			this.SetOwnedResourceCount(ownedCount);
		}

		// Token: 0x06007010 RID: 28688 RVA: 0x0033E22E File Offset: 0x0033C42E
		private void OnEnable()
		{
			this.EnsureDriveTargetContentRootCached();
			this.PlaySwitchBg(this.SelectingDriveType == 1);
			this.SetDriveTargetDisplayLoading(true);
			CharacterDomainMethod.AsyncCall.GetCharacterDisplayData(null, this._parent.DriveTargetCharId, delegate(int offset, RawDataPool pool)
			{
				bool flag = !base.isActiveAndEnabled;
				if (flag)
				{
					this.SetDriveTargetDisplayLoadingImmediate(false);
				}
				else
				{
					CharacterDisplayData displayData = null;
					Serializer.Deserialize(pool, offset, ref displayData);
					this.avatarTarget.Refresh(displayData, true);
					NameRelatedData nameRelatedData = NameCenter.GetNameRelatedData(displayData);
					string nameStr = NameCenter.GetMonasticTitleOrDisplayName(ref nameRelatedData, this._parent.DriveTargetCharId == this._parent.TaiwuCharId, false);
					this.targetName.text = nameStr;
					this.SetDriveTargetDisplayLoading(false);
				}
			});
		}

		// Token: 0x06007011 RID: 28689 RVA: 0x0033E26E File Offset: 0x0033C46E
		private void OnDisable()
		{
			this.SetDriveTargetDisplayLoadingImmediate(false);
		}

		// Token: 0x06007012 RID: 28690 RVA: 0x0033E27C File Offset: 0x0033C47C
		private void PlaySwitchBg(bool isPositive, UIParticle effLoop, UnityEngine.Material matTo, UIParticle effTo)
		{
			this._sequence.Kill(false);
			effLoop.Stop();
			effTo.Stop();
			matTo.DOKill(false);
			this.positiveArea.SetActive(isPositive);
			this.negativeArea.SetActive(!isPositive);
			float duration1 = 0.7111111f;
			float duration2 = 0.4f;
			CanvasGroup loopGroup = effLoop.GetComponent<CanvasGroup>();
			loopGroup.alpha = 0f;
			matTo.SetFloat("_DissolveAmount", 1f);
			matTo.SetFloat("_EdgeWidth", 0.4f);
			Sequence seq = DOTween.Sequence();
			seq.AppendCallback(delegate
			{
				effTo.Play();
			});
			seq.AppendCallback(delegate
			{
				matTo.DOFloat(0.48298f, "_DissolveAmount", duration1).SetEase(Ease.Linear);
			});
			seq.AppendCallback(delegate
			{
				matTo.DOFloat(0.22298f, "_EdgeWidth", duration1).SetEase(Ease.Linear);
			});
			seq.AppendInterval(duration1);
			seq.AppendCallback(delegate
			{
				matTo.DOFloat(-0.1f, "_DissolveAmount", duration2).SetEase(Ease.Linear);
			});
			seq.AppendCallback(delegate
			{
				matTo.DOFloat(0.02f, "_EdgeWidth", duration2).SetEase(Ease.Linear);
			});
			seq.AppendInterval(duration2);
			seq.AppendCallback(delegate
			{
				loopGroup.DOFade(1f, 1f);
			});
			seq.AppendCallback(delegate
			{
				effLoop.Play();
			});
			this._sequence = seq;
			this._sequence.Play<Sequence>();
		}

		// Token: 0x06007013 RID: 28691 RVA: 0x0033E414 File Offset: 0x0033C614
		private void PlaySwitchBg(bool isPositive)
		{
			if (isPositive)
			{
				this.PlaySwitchBg(isPositive, this.effLoopPositiveBg, this.matToPositiveBg, this.effToPositiveBg);
			}
			else
			{
				this.PlaySwitchBg(isPositive, this.effLoopNegativeBg, this.matToNegativeBg, this.effToNegativeBg);
			}
		}

		// Token: 0x06007014 RID: 28692 RVA: 0x0033E460 File Offset: 0x0033C660
		private int CoolDownRemain(int startData, int cooldownMonths = 6)
		{
			int currentDate = SingletonObject.getInstance<BasicGameData>().CurrDate;
			int elapsedMonths = currentDate - startData;
			return cooldownMonths - elapsedMonths;
		}

		// Token: 0x06007018 RID: 28696 RVA: 0x0033E54E File Offset: 0x0033C74E
		[CompilerGenerated]
		private void <OnClickDriveButton>g__Action|60_1()
		{
			this._parent.PlayRoleAnim(ViewMakeWugKing.RoleAnimName.MakeWugLoop, null);
			StoryDomainMethod.AsyncCall.DriveWugKing(this._parent, this._parent.DriveTargetCharId, this._selectedWugKingType, this.SelectingDriveType, delegate(int offset, RawDataPool pool)
			{
				bool result = false;
				Serializer.Deserialize(pool, offset, ref result);
				bool flag = result;
				if (flag)
				{
					this.PlayDriveSuccessEffect();
				}
			});
		}

		// Token: 0x040052F2 RID: 21234
		[Header("背景")]
		[SerializeField]
		private GameObject positiveArea;

		// Token: 0x040052F3 RID: 21235
		[SerializeField]
		private UIParticle effToPositiveBg;

		// Token: 0x040052F4 RID: 21236
		[SerializeField]
		private UIParticle effLoopPositiveBg;

		// Token: 0x040052F5 RID: 21237
		[SerializeField]
		private UnityEngine.Material matToPositiveBg;

		// Token: 0x040052F6 RID: 21238
		[SerializeField]
		private GameObject negativeArea;

		// Token: 0x040052F7 RID: 21239
		[SerializeField]
		private UIParticle effToNegativeBg;

		// Token: 0x040052F8 RID: 21240
		[SerializeField]
		private UIParticle effLoopNegativeBg;

		// Token: 0x040052F9 RID: 21241
		[SerializeField]
		private UnityEngine.Material matToNegativeBg;

		// Token: 0x040052FA RID: 21242
		[Header("蛊动画")]
		[SerializeField]
		private SkeletonGraphic skelWug;

		// Token: 0x040052FB RID: 21243
		[SerializeField]
		private UIParticle effDriveLine;

		// Token: 0x040052FC RID: 21244
		[SerializeField]
		private TooltipInvoker mouseTip;

		// Token: 0x040052FD RID: 21245
		[SerializeField]
		private Game.Components.Avatar.Avatar avatarTarget;

		// Token: 0x040052FE RID: 21246
		[Header("驱动目标展示（加载态，参考 ViewCharacterMenuEquip）")]
		[Tooltip("可选；加载时显示。为空则仅用位移隐藏内容根。")]
		[SerializeField]
		private LoadingAnimation driveTargetAreaLoading;

		// Token: 0x040052FF RID: 21247
		[Tooltip("加载期间移出屏幕的展示根节点；为空则使用 Avatar 自身或其父 RectTransform。")]
		[SerializeField]
		private RectTransform driveTargetContentRoot;

		// Token: 0x04005300 RID: 21248
		[Header("交互")]
		[SerializeField]
		private CToggleGroup driveTypeToggleGroup;

		// Token: 0x04005301 RID: 21249
		[SerializeField]
		private CButton btnDrive;

		// Token: 0x04005302 RID: 21250
		[Header("描述")]
		[SerializeField]
		private GameObject wugLayout;

		// Token: 0x04005303 RID: 21251
		[SerializeField]
		private TextMeshProUGUI driveTypeName;

		// Token: 0x04005304 RID: 21252
		[SerializeField]
		private TextMeshProUGUI wugName;

		// Token: 0x04005305 RID: 21253
		[SerializeField]
		private TextMeshProUGUI cdContent;

		// Token: 0x04005306 RID: 21254
		[SerializeField]
		private TextMeshProUGUI effectDescLabel;

		// Token: 0x04005307 RID: 21255
		[SerializeField]
		private CImage costResourceIcon;

		// Token: 0x04005308 RID: 21256
		[SerializeField]
		private TextMeshProUGUI resourceLabel;

		// Token: 0x04005309 RID: 21257
		[SerializeField]
		private TextMeshProUGUI actionPointLabel;

		// Token: 0x0400530A RID: 21258
		[SerializeField]
		private TextMeshProUGUI targetName;

		// Token: 0x0400530B RID: 21259
		private readonly Dictionary<sbyte, string> _wugKingSkinName = new Dictionary<sbyte, string>
		{
			{
				0,
				"chimu"
			},
			{
				1,
				"chimei"
			},
			{
				2,
				"heixue"
			},
			{
				3,
				"xinmo"
			},
			{
				4,
				"shichi"
			},
			{
				5,
				"baicai"
			},
			{
				6,
				"jinchan"
			},
			{
				7,
				"biyu"
			}
		};

		// Token: 0x0400530C RID: 21260
		private const string WUG_ANIM_COOLDOWN = "idle";

		// Token: 0x0400530D RID: 21261
		private const string WUG_ANIM_CAN_DRIVE = "activation";

		// Token: 0x0400530E RID: 21262
		private const string WUG_ANIM_DRIVING = "act";

		// Token: 0x0400530F RID: 21263
		private sbyte _selectedWugKingType = -1;

		// Token: 0x04005310 RID: 21264
		private int _ownedResourceCount;

		// Token: 0x04005311 RID: 21265
		private bool _hasEnoughResource;

		// Token: 0x04005312 RID: 21266
		private bool _hasEnoughActionPoint;

		// Token: 0x04005313 RID: 21267
		private StringBuilder _sbCached = new StringBuilder();

		// Token: 0x04005314 RID: 21268
		private ViewMakeWugKing _parent;

		// Token: 0x04005315 RID: 21269
		private bool _inited;

		// Token: 0x04005316 RID: 21270
		private const string Blue = "8dc3c3";

		// Token: 0x04005317 RID: 21271
		private Sequence _sequence;

		// Token: 0x04005318 RID: 21272
		private RectTransform _cachedDriveTargetContentRoot;

		// Token: 0x04005319 RID: 21273
		private Vector2 _driveTargetContentRootOriginalPos;

		// Token: 0x0400531A RID: 21274
		private bool _driveTargetDisplayMovedOffscreen;

		// Token: 0x0400531B RID: 21275
		private bool _isDriveTargetDisplayLoading;

		// Token: 0x02001E2E RID: 7726
		private enum EWugKingAnimationType
		{
			// Token: 0x0400C8E3 RID: 51427
			None,
			// Token: 0x0400C8E4 RID: 51428
			Cooldown,
			// Token: 0x0400C8E5 RID: 51429
			CanDrive,
			// Token: 0x0400C8E6 RID: 51430
			Driving
		}
	}
}
