using System;
using System.Collections.Generic;
using Coffee.UIExtensions;
using Config;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Building;
using GameData.Domains.Extra;
using GameData.Domains.World;
using GameData.Serializer;
using GameData.Utilities;
using UnityEngine;

namespace Game.Views.Building.BuildingManage
{
	// Token: 0x02000C0F RID: 3087
	public class SubPageExpandTaiwuVow : MonoBehaviour, IBuildingExpandTaiwuVillageSteleHandler, IBuildingExpandTaiwuVillageSteleProvider
	{
		// Token: 0x1700108F RID: 4239
		// (get) Token: 0x06009CA6 RID: 40102 RVA: 0x00495FC5 File Offset: 0x004941C5
		private BuildingBlockItem ConfigData
		{
			get
			{
				return BuildingBlock.Instance[44];
			}
		}

		// Token: 0x06009CA7 RID: 40103 RVA: 0x00495FD4 File Offset: 0x004941D4
		public static bool CheckCanOpenExpandPage(short blockTemplateId)
		{
			bool flag = blockTemplateId == 44;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				BuildingBlockItem blockConfig = BuildingBlock.Instance[blockTemplateId];
				bool isResource = blockConfig.Class == EBuildingBlockClass.BornResource;
				bool flag2 = isResource;
				result = flag2;
			}
			return result;
		}

		// Token: 0x06009CA8 RID: 40104 RVA: 0x00496018 File Offset: 0x00494218
		public void Init(Action onRefresh)
		{
			bool inited = this._inited;
			if (!inited)
			{
				this._onRefresh = onRefresh;
				this.scaleBgTest.SetActive(true);
				this.effUpgrade.gameObject.SetActive(false);
				this.animationController.Bind(this);
				this._steles.Clear();
				BuildingExpandTaiwuVillageSteleComponent[] steles = base.GetComponentsInChildren<BuildingExpandTaiwuVillageSteleComponent>();
				foreach (BuildingExpandTaiwuVillageSteleComponent stele in steles)
				{
					stele.Bind(this);
					this._steles[stele.orgTemplateId] = stele;
				}
				this.btnConfirmUpgrade.ClearAndAddListener(new Action(this.OnConfirm));
				this.btnCancelUpgrade.ClearAndAddListener(new Action(this.OnCancel));
				this.animationController.Init();
				this._inited = true;
			}
		}

		// Token: 0x06009CA9 RID: 40105 RVA: 0x004960F4 File Offset: 0x004942F4
		private void OnEnable()
		{
			GEvent.Add(UiEvents.BuildingBlockDataChange, new GEvent.Callback(this.OnBuildingBlockDataChange));
		}

		// Token: 0x06009CAA RID: 40106 RVA: 0x00496110 File Offset: 0x00494310
		private void OnDisable()
		{
			GEvent.Remove(UiEvents.BuildingBlockDataChange, new GEvent.Callback(this.OnBuildingBlockDataChange));
			this._effInited = false;
			this._isUnlockAnimating = false;
		}

		// Token: 0x06009CAB RID: 40107 RVA: 0x0049613C File Offset: 0x0049433C
		public void Refresh(BuildingBlockData blockData, BuildingBlockKey key)
		{
			this._key = key;
			this._buildingModel = SingletonObject.getInstance<BuildingModel>();
			bool isUnlockAnimating = this._isUnlockAnimating;
			if (isUnlockAnimating)
			{
				this._buildingBlockData = blockData;
			}
			else
			{
				this.RequestGetIsJiaoPoolOpen();
				this.ResetStatus();
			}
		}

		// Token: 0x06009CAC RID: 40108 RVA: 0x00496180 File Offset: 0x00494380
		public void OnConfirm()
		{
			bool flag = this._selectedOrgTemplateId < 0;
			if (!flag)
			{
				this.EnableClickMask();
				this._isUnlockAnimating = true;
				this.RequestUnlockBuildingLevelSlot();
				BuildingExpandTaiwuVillageSteleComponent targetStele = this._steles[this._selectedOrgTemplateId];
				bool needScale = this.unlockScale != 1f;
				Sequence seq = DOTween.Sequence();
				this.scaleRoot.DOKill(false);
				Vector2 oldPivot = this.scaleRoot.pivot;
				BuildingExpandTaiwuVillageSteleComponent focusSteleComponent = this._steles[this._selectedOrgTemplateId];
				bool flag2 = needScale;
				if (flag2)
				{
					this.scaleRoot.pivot = SubPageExpandTaiwuVow.CalcPivotFromChild(this.scaleRoot, focusSteleComponent.GetComponent<RectTransform>());
					this.scaleRoot.localScale = Vector3.one;
					seq.Append(this.scaleRoot.DOScale(this.unlockScale, 0.27f).SetEase(Ease.OutQuad));
					seq.AppendInterval(0.27f);
				}
				seq.AppendCallback(delegate
				{
					targetStele.selected2.GetComponent<CImage>().DOFade(0f, 0.5f);
					this.effUpgrade.transform.parent = targetStele.transform;
					this.effUpgrade.transform.localPosition = this.effUpgradeOffset;
					this.effUpgrade.gameObject.SetActive(true);
					this.effUpgrade.Play();
					targetStele.PlayUnlockEff();
					this.animationController.PlayGrassDisappear(this._selectedOrgTemplateId, 2.2f);
				});
				this.animationController.AppendWheelPointEff(seq, this._buildingBlockData, this._selectedOrgTemplateId);
				seq.AppendInterval(2.2f);
				seq.AppendCallback(delegate
				{
					focusSteleComponent.AnimationToUpgraded(1f);
					this.effUpgrade.gameObject.SetActive(false);
					targetStele.HideUnlockEff();
					this.upgradeCanvasGroup.DOFade(0f, this.upgradeViewFadeDuration);
				});
				bool flag3 = needScale;
				if (flag3)
				{
					seq.Append(this.scaleRoot.DOScale(1f, 0.27f));
					seq.AppendCallback(delegate
					{
						this.scaleRoot.pivot = oldPivot;
					});
				}
				this.animationController.AppendFogWave(seq, this._buildingBlockData, this._selectedOrgTemplateId, this._currentUnlockedAmount);
				seq.AppendCallback(new TweenCallback(this.FinishUnlockAnimation));
				seq.PlayForward();
			}
		}

		// Token: 0x06009CAD RID: 40109 RVA: 0x0049634C File Offset: 0x0049454C
		private void FinishUnlockAnimation()
		{
			this._isUnlockAnimating = false;
			this._buildingBlockData = this._buildingModel.GetTaiwuBuildingData(this._key);
			this.ResetStatus();
		}

		// Token: 0x06009CAE RID: 40110 RVA: 0x00496374 File Offset: 0x00494574
		public void OnCancel()
		{
			this.ResetStatus();
		}

		// Token: 0x06009CAF RID: 40111 RVA: 0x00496380 File Offset: 0x00494580
		private void OnBuildingBlockDataChange(ArgumentBox argumentBox)
		{
			bool isUnlockAnimating = this._isUnlockAnimating;
			if (isUnlockAnimating)
			{
				this._buildingBlockData = this._buildingModel.GetTaiwuBuildingData(this._key);
			}
			else
			{
				this.ResetStatus();
			}
		}

		// Token: 0x06009CB0 RID: 40112 RVA: 0x004963BC File Offset: 0x004945BC
		private void ResetStatus()
		{
			this._selectedOrgTemplateId = -1;
			this._buildingBlockData = this._buildingModel.GetTaiwuBuildingData(this._key);
			this.upgrade.gameObject.SetActive(false);
			this.upgrade.Set(this._buildingBlockData, this.ConfigData);
			this.RefreshSteles();
		}

		// Token: 0x06009CB1 RID: 40113 RVA: 0x00496419 File Offset: 0x00494619
		private void RequestGetIsJiaoPoolOpen()
		{
			this.upgrade.Set(false);
			ExtraDomainMethod.AsyncCall.GetIsJiaoPoolOpen(null, new AsyncMethodCallbackDelegate(this.HandlerGetIsJiaoPoolOpen));
		}

		// Token: 0x06009CB2 RID: 40114 RVA: 0x0049643C File Offset: 0x0049463C
		private void HandlerGetIsJiaoPoolOpen(int offset, RawDataPool pool)
		{
			bool jiaoPoolIsOpen = false;
			Serializer.Deserialize(pool, offset, ref jiaoPoolIsOpen);
			this.upgrade.Set(jiaoPoolIsOpen);
		}

		// Token: 0x06009CB3 RID: 40115 RVA: 0x00496464 File Offset: 0x00494664
		private void RequestUnlockBuildingLevelSlot()
		{
			int i = (int)(this._selectedOrgTemplateId - 1);
			BuildingDomainMethod.AsyncCall.UnlockBuildingLevelSlot(null, this._key, i, new AsyncMethodCallbackDelegate(this.HandlerUnlockBuildingLevelSlot));
		}

		// Token: 0x06009CB4 RID: 40116 RVA: 0x00496495 File Offset: 0x00494695
		private void HandlerUnlockBuildingLevelSlot(int offset, RawDataPool pool)
		{
			this.DisableClickMask();
			Action onRefresh = this._onRefresh;
			if (onRefresh != null)
			{
				onRefresh();
			}
			GEvent.OnEvent(UiEvents.BuildingVowLevelChanged, null);
		}

		// Token: 0x06009CB5 RID: 40117 RVA: 0x004964BF File Offset: 0x004946BF
		private void EnableClickMask()
		{
			this.clickMask.SetActive(true);
			this.btnConfirmUpgrade.interactable = false;
			UIManager.Instance.SetEscHandler(new Action(this.InfinityEscHandler));
		}

		// Token: 0x06009CB6 RID: 40118 RVA: 0x004964F3 File Offset: 0x004946F3
		private void DisableClickMask()
		{
			this.clickMask.SetActive(false);
			this.btnConfirmUpgrade.interactable = true;
			UIManager.Instance.SetEscHandler(null);
		}

		// Token: 0x06009CB7 RID: 40119 RVA: 0x0049651C File Offset: 0x0049471C
		private void InfinityEscHandler()
		{
			UIManager.Instance.SetEscHandler(new Action(this.InfinityEscHandler));
		}

		// Token: 0x06009CB8 RID: 40120 RVA: 0x00496538 File Offset: 0x00494738
		private static bool IsVowSteleInteractionUnlocked()
		{
			return SingletonObject.getInstance<TaskModel>().IsTaskFinished(31);
		}

		// Token: 0x06009CB9 RID: 40121 RVA: 0x00496558 File Offset: 0x00494758
		private void RefreshSteles()
		{
			BuildingBlockItem blockConfig = this.ConfigData;
			this._currentUnlockedAmount = 0;
			bool mouseTipEnabled = SubPageExpandTaiwuVow.IsVowSteleInteractionUnlocked();
			for (int i = 0; i < (int)blockConfig.MaxLevel; i++)
			{
				bool unlocked = this._buildingBlockData.SlotIsUnlocked(i);
				bool flag = unlocked;
				if (flag)
				{
					this._currentUnlockedAmount++;
				}
				sbyte orgTemplateId = (sbyte)(i + 1);
				BuildingExpandTaiwuVillageSteleComponent stele;
				bool flag2 = this._steles.TryGetValue(orgTemplateId, out stele);
				if (flag2)
				{
					stele.Set(unlocked, false, this._selectedOrgTemplateId, mouseTipEnabled);
				}
				this.animationController.RefreshGrass(i + 1, unlocked);
			}
			bool allUnlocked = this._currentUnlockedAmount == (int)blockConfig.MaxLevel;
			this.centerSteleUnlockAll.SetActive(allUnlocked && !this._isUnlockAnimating);
			bool flag3 = !this._isUnlockAnimating && allUnlocked && !this.effComplete.gameObject.activeSelf;
			if (flag3)
			{
				this.effComplete.gameObject.SetActive(true);
				this.effCompleteCanvasGroup.alpha = 0f;
				this.effCompleteCanvasGroup.DOFade(1f, this.animationController.UnlockAllFadeDuration);
				this.animationController.SwitchBgToAllUnlock();
				for (int j = 0; j < (int)blockConfig.MaxLevel; j++)
				{
					sbyte orgTemplateId2 = (sbyte)(j + 1);
					BuildingExpandTaiwuVillageSteleComponent stele2;
					bool flag4 = this._steles.TryGetValue(orgTemplateId2, out stele2);
					if (flag4)
					{
						stele2.AnimationToUpgradedAllUnlock(this.animationController.UnlockAllFadeDuration);
					}
				}
			}
			else
			{
				bool flag5 = !this._isUnlockAnimating && allUnlocked;
				if (flag5)
				{
					for (int k = 0; k < (int)blockConfig.MaxLevel; k++)
					{
						sbyte orgTemplateId3 = (sbyte)(k + 1);
						BuildingExpandTaiwuVillageSteleComponent stele3;
						bool flag6 = this._steles.TryGetValue(orgTemplateId3, out stele3);
						if (flag6)
						{
							stele3.Set(true, true, this._selectedOrgTemplateId, mouseTipEnabled);
						}
					}
					this.animationController.SetBgStatus(true);
				}
				else
				{
					this.animationController.SetBgStatus(false);
				}
			}
			bool flag7 = !this._effInited;
			if (flag7)
			{
				this.animationController.RefreshFogDirect(this._buildingBlockData, this._currentUnlockedAmount);
				this._effInited = true;
			}
			bool reachAmountLimition = this._currentUnlockedAmount >= this.GetVowAmountLimit();
			this.upgrade.SetLimition(reachAmountLimition);
		}

		// Token: 0x06009CBA RID: 40122 RVA: 0x004967C0 File Offset: 0x004949C0
		private void ShowLockedDialog()
		{
			DialogCmd dialogCmd = new DialogCmd
			{
				Type = 2,
				Title = LocalStringManager.Get(LanguageKey.LK_Building_ExpandTaiwuVillage_Locked_Dialog_Title),
				Content = LocalStringManager.Get(LanguageKey.LK_Building_ExpandTaiwuVillage_Locked_Dialog_Desc)
			};
			UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", dialogCmd));
			UIManager.Instance.MaskUI(UIElement.Dialog);
		}

		// Token: 0x06009CBB RID: 40123 RVA: 0x00496828 File Offset: 0x00494A28
		void IBuildingExpandTaiwuVillageSteleHandler.Handle(sbyte orgTemplateId)
		{
			bool flag = !SubPageExpandTaiwuVow.IsVowSteleInteractionUnlocked();
			if (flag)
			{
				this.ShowLockedDialog();
			}
			else
			{
				this._selectedOrgTemplateId = orgTemplateId;
				this.upgrade.gameObject.SetActive(this._selectedOrgTemplateId >= 0);
				bool flag2 = this._selectedOrgTemplateId >= 0;
				if (flag2)
				{
					this.upgradeCanvasGroup.alpha = 1f;
				}
				bool flag3 = this._selectedOrgTemplateId < 0;
				if (!flag3)
				{
					sbyte index = this._selectedOrgTemplateId - 1;
					bool unlocked = this._buildingBlockData.SlotIsUnlocked((int)index);
					this.upgrade.Set(this._selectedOrgTemplateId, unlocked, this._buildingBlockData, this.ConfigData);
					this.RefreshSteles();
				}
			}
		}

		// Token: 0x06009CBC RID: 40124 RVA: 0x004968E5 File Offset: 0x00494AE5
		void IBuildingExpandTaiwuVillageSteleHandler.Cancel()
		{
			this.ResetStatus();
		}

		// Token: 0x06009CBD RID: 40125 RVA: 0x004968F0 File Offset: 0x00494AF0
		BuildingExpandTaiwuVillageSteleComponent IBuildingExpandTaiwuVillageSteleProvider.GetStele(sbyte orgTemplateId)
		{
			return this._steles[orgTemplateId];
		}

		// Token: 0x06009CBE RID: 40126 RVA: 0x00496910 File Offset: 0x00494B10
		private int GetVowAmountLimit()
		{
			bool hasLimit = SingletonObject.getInstance<BasicGameData>().ChallengeModeData.IsEnabled(EChallengeModeImplement.TaiwuVowLimition);
			bool flag = !hasLimit;
			int result;
			if (flag)
			{
				result = int.MaxValue;
			}
			else
			{
				sbyte xiangshuProgress = SingletonObject.getInstance<BasicGameData>().XiangshuProgress;
				int xiangshuLevel = (int)(GameData.Domains.World.SharedMethods.GetXiangshuLevel(xiangshuProgress) - 1);
				result = xiangshuLevel * 2 + 1;
			}
			return result;
		}

		// Token: 0x06009CBF RID: 40127 RVA: 0x00496964 File Offset: 0x00494B64
		private void Update()
		{
			bool flag = UIManager.Instance.CheckPopupElementIsInTop(UIElement.BuildingManage) != this.rootObj.activeSelf;
			if (flag)
			{
				this.rootObj.SetActive(UIManager.Instance.CheckPopupElementIsInTop(UIElement.BuildingManage));
			}
			bool flag2 = CommonCommandKit.Space.Check(UIElement.BuildingManage, false, false, false, true, false);
			if (flag2)
			{
				bool interactable = this.btnConfirmUpgrade.interactable;
				if (interactable)
				{
					this.OnConfirm();
				}
			}
		}

		// Token: 0x06009CC0 RID: 40128 RVA: 0x004969E4 File Offset: 0x00494BE4
		private static Vector2 CalcPivotFromChild(RectTransform parent, RectTransform child)
		{
			Vector2 local = parent.InverseTransformPoint(child.position);
			Rect rect = parent.rect;
			return new Vector2((local.x - rect.x) / rect.width, (local.y - rect.y) / rect.height);
		}

		// Token: 0x04007984 RID: 31108
		public const int CoreOrganizationCount = 5;

		// Token: 0x04007985 RID: 31109
		[SerializeField]
		private BuildingExpandTaiwuVillageUpgradeComponent upgrade;

		// Token: 0x04007986 RID: 31110
		[SerializeField]
		private CanvasGroup upgradeCanvasGroup;

		// Token: 0x04007987 RID: 31111
		[SerializeField]
		private TaiwuVillageExpandAnimationController animationController;

		// Token: 0x04007988 RID: 31112
		[SerializeField]
		private GameObject clickMask;

		// Token: 0x04007989 RID: 31113
		[SerializeField]
		private CButton btnConfirmUpgrade;

		// Token: 0x0400798A RID: 31114
		[SerializeField]
		private CButton btnCancelUpgrade;

		// Token: 0x0400798B RID: 31115
		[SerializeField]
		private GameObject rootObj;

		// Token: 0x0400798C RID: 31116
		[SerializeField]
		private EffectPlayer effectPlayer;

		// Token: 0x0400798D RID: 31117
		[Header("激活时缩放的根节点")]
		[SerializeField]
		private RectTransform scaleRoot;

		// Token: 0x0400798E RID: 31118
		[Header("升级时 石碑的升级特效")]
		[SerializeField]
		private UIParticle effUpgrade;

		// Token: 0x0400798F RID: 31119
		[SerializeField]
		private Vector3 effUpgradeOffset = new Vector3(0f, -11f, 0f);

		// Token: 0x04007990 RID: 31120
		[Header("全部升级后的循环特效")]
		[SerializeField]
		private UIParticle effComplete;

		// Token: 0x04007991 RID: 31121
		[SerializeField]
		private CanvasGroup effCompleteCanvasGroup;

		// Token: 0x04007992 RID: 31122
		[Header("解锁时的缩放值 1为不变")]
		[SerializeField]
		private float unlockScale = 1f;

		// Token: 0x04007993 RID: 31123
		[SerializeField]
		private GameObject centerSteleUnlockAll;

		// Token: 0x04007994 RID: 31124
		[Header("参数")]
		[SerializeField]
		private float upgradeViewFadeDuration = 0.2f;

		// Token: 0x04007995 RID: 31125
		[SerializeField]
		private GameObject scaleBgTest;

		// Token: 0x04007996 RID: 31126
		private BuildingBlockKey _key;

		// Token: 0x04007997 RID: 31127
		private BuildingModel _buildingModel;

		// Token: 0x04007998 RID: 31128
		private BuildingBlockData _buildingBlockData;

		// Token: 0x04007999 RID: 31129
		private sbyte _selectedOrgTemplateId = -1;

		// Token: 0x0400799A RID: 31130
		private int _currentUnlockedAmount = 0;

		// Token: 0x0400799B RID: 31131
		private bool _effInited = false;

		// Token: 0x0400799C RID: 31132
		private bool _isUnlockAnimating;

		// Token: 0x0400799D RID: 31133
		private readonly Dictionary<sbyte, BuildingExpandTaiwuVillageSteleComponent> _steles = new Dictionary<sbyte, BuildingExpandTaiwuVillageSteleComponent>();

		// Token: 0x0400799E RID: 31134
		private Action _onRefresh;

		// Token: 0x0400799F RID: 31135
		private bool _inited = false;

		// Token: 0x040079A0 RID: 31136
		private const float unlockScaleUpTime = 0.27f;

		// Token: 0x040079A1 RID: 31137
		private const float unlockWaitRecoverTime = 2.2f;
	}
}
