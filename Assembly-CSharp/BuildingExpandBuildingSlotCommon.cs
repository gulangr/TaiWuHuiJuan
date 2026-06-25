using System;
using System.Collections.Generic;
using Coffee.UIExtensions;
using Config;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using FrameWork.UISystem.Components.EffectPlayer;
using GameData.Domains.Building;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

// Token: 0x020001A3 RID: 419
public abstract class BuildingExpandBuildingSlotCommon : MonoBehaviour
{
	// Token: 0x060017BE RID: 6078
	protected abstract List<ResourceInfo> GetConfig();

	// Token: 0x1700028F RID: 655
	// (get) Token: 0x060017BF RID: 6079
	protected abstract BuildingExpandBuildingSlotCommon.EParticle ParticleType { get; }

	// Token: 0x17000290 RID: 656
	// (get) Token: 0x060017C0 RID: 6080 RVA: 0x0009225C File Offset: 0x0009045C
	private BuildingModel BuildingModel
	{
		get
		{
			return SingletonObject.getInstance<BuildingModel>();
		}
	}

	// Token: 0x060017C1 RID: 6081 RVA: 0x00092263 File Offset: 0x00090463
	private void OnEnable()
	{
		GEvent.Add(UiEvents.BuildingBlockDataChange, new GEvent.Callback(this.OnBuildingBlockDataChange));
		GEvent.Add(EEvents.OnTaiwuResourceChange, new GEvent.Callback(this.OnTaiwuResourceChange));
	}

	// Token: 0x060017C2 RID: 6082 RVA: 0x00092298 File Offset: 0x00090498
	private void OnDisable()
	{
		GEvent.Remove(UiEvents.BuildingBlockDataChange, new GEvent.Callback(this.OnBuildingBlockDataChange));
		GEvent.Remove(EEvents.OnTaiwuResourceChange, new GEvent.Callback(this.OnTaiwuResourceChange));
	}

	// Token: 0x060017C3 RID: 6083 RVA: 0x000922D0 File Offset: 0x000904D0
	private void LateUpdate()
	{
		bool refreshDirty = this._refreshDirty;
		if (refreshDirty)
		{
			this.RefreshInner();
			this._refreshDirty = false;
		}
	}

	// Token: 0x060017C4 RID: 6084 RVA: 0x000922F8 File Offset: 0x000904F8
	private void OnBuildingBlockDataChange(ArgumentBox _)
	{
		this._refreshDirty = true;
	}

	// Token: 0x060017C5 RID: 6085 RVA: 0x00092302 File Offset: 0x00090502
	private void OnTaiwuResourceChange(ArgumentBox _)
	{
		this._refreshDirty = true;
	}

	// Token: 0x060017C6 RID: 6086 RVA: 0x0009230C File Offset: 0x0009050C
	public void Refresh(UI_BuildingManage parent, BuildingBlockKey key, BuildingBlockData blockData)
	{
		this._parent = parent;
		this._key = key;
		this._blockData = blockData;
		this.ResetSlots();
		this._refreshDirty = true;
	}

	// Token: 0x060017C7 RID: 6087 RVA: 0x00092332 File Offset: 0x00090532
	private void RefreshInner()
	{
		this.RefreshSlots();
	}

	// Token: 0x060017C8 RID: 6088 RVA: 0x0009233C File Offset: 0x0009053C
	private void RefreshSlots()
	{
		List<ResourceInfo> config = this.GetConfig();
		BuildingBlockData data = this.BuildingModel.GetTaiwuBuildingData(this._key);
		for (int i = 0; i < config.Count; i++)
		{
			this.RefreshSlot(config, i, data);
		}
	}

	// Token: 0x060017C9 RID: 6089 RVA: 0x00092384 File Offset: 0x00090584
	private void RefreshSlot(List<ResourceInfo> config, int configIndex, BuildingBlockData data)
	{
		GameObject cellContent = this._cellContents[configIndex];
		ResourceInfo cost = config[configIndex];
		Refers addButtonRefers = this._addButtonRefers[configIndex];
		bool unlocked = data.SlotIsUnlocked(configIndex + 1);
		this.RefreshCellContent(configIndex, cellContent, unlocked);
		addButtonRefers.gameObject.SetActive(!unlocked);
		bool flag = !unlocked;
		if (flag)
		{
			this.RefreshAddButton(cost, addButtonRefers, configIndex);
		}
	}

	// Token: 0x060017CA RID: 6090 RVA: 0x000923F0 File Offset: 0x000905F0
	private void RefreshCellContent(int configIndex, GameObject cellContent, bool unlocked)
	{
		cellContent.SetActive(unlocked);
		bool flag = this._needUnlockAnimationSlotIndices.Contains(configIndex) && unlocked;
		if (flag)
		{
			Sequence eq;
			bool flag2 = this._unlockAnimations.TryGetValue(configIndex, out eq);
			if (flag2)
			{
				eq.Kill(false);
				this._unlockAnimations.Remove(configIndex);
			}
			cellContent.GetComponent<CanvasGroup>().alpha = 0f;
			cellContent.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -100f);
			this._unlockAnimations.Add(configIndex, this.StartCellTween(cellContent));
			this._needUnlockAnimationSlotIndices.Remove(configIndex);
		}
	}

	// Token: 0x060017CB RID: 6091 RVA: 0x00092490 File Offset: 0x00090690
	private Sequence StartCellTween(GameObject slot)
	{
		CanvasGroup canvasGroup = slot.GetComponent<CanvasGroup>();
		bool flag = !canvasGroup;
		Sequence result;
		if (flag)
		{
			result = null;
		}
		else
		{
			Sequence sequence = DOTween.Sequence().Join(slot.GetComponent<RectTransform>().DOAnchorPosY(0f, 0.7f, false).SetEase(Ease.OutExpo)).Join(canvasGroup.DOFade(1f, 0.7f).SetEase(Ease.OutBack));
			sequence.PlayForward();
			result = sequence;
		}
		return result;
	}

	// Token: 0x060017CC RID: 6092 RVA: 0x00092508 File Offset: 0x00090708
	private void RefreshAddButton(ResourceInfo cost, Refers refers, int index)
	{
		CButtonObsolete button = refers.CGet<CButtonObsolete>("Button");
		RectTransform buttonRect = button.GetComponent<RectTransform>();
		int haveResource = this.BuildingModel.GetResourceCount(cost.ResourceType);
		bool interactable = haveResource >= cost.ResourceCount;
		BuildingExpandBuildingSlotCommon.RefreshButtonInteractable(button, interactable);
		bool flag = interactable;
		if (flag)
		{
			button.ClearAndAddListener(delegate
			{
				this.AlertAndUpgrade(index, index + 1, cost, haveResource, buttonRect);
			});
		}
		BuildingExpandBuildingSlotCommon.RefreshAddButtonResource(refers, haveResource, cost);
	}

	// Token: 0x060017CD RID: 6093 RVA: 0x000925B0 File Offset: 0x000907B0
	private static void RefreshButtonInteractable(CButtonObsolete button, bool interactable)
	{
		button.interactable = interactable;
		button.transform.Find("OnIcon").gameObject.SetActive(interactable);
		button.transform.Find("OffIcon").gameObject.SetActive(!interactable);
	}

	// Token: 0x060017CE RID: 6094 RVA: 0x00092604 File Offset: 0x00090804
	private static void RefreshAddButtonResource(Refers refers, int haveResource, ResourceInfo cost)
	{
		TextMeshProUGUI resourceType = refers.CGet<TextMeshProUGUI>("ResourceType");
		CImage resourceIcon = refers.CGet<CImage>("ResourceIcon");
		TextMeshProUGUI resourceCount = refers.CGet<TextMeshProUGUI>("ResourceCount");
		ResourceTypeItem resourceTypeConfig = ResourceType.Instance[cost.ResourceType];
		resourceType.text = resourceTypeConfig.Name;
		resourceCount.text = CommonUtils.GetColoredStringByCompare(haveResource, cost.ResourceCount, cost.ResourceCount.CompareTo(haveResource), true);
		resourceIcon.SetSprite(resourceTypeConfig.Icon, false, null);
	}

	// Token: 0x060017CF RID: 6095 RVA: 0x00092684 File Offset: 0x00090884
	private void AlertAndUpgrade(int index, int levelSlot, ResourceInfo cost, int haveResource, RectTransform particleAnchor)
	{
		AsyncMethodCallbackDelegate <>9__1;
		ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>().SetObject("Desc1", LanguageKey.LK_Building_UpgradeBuildingSlot_Confirm_Desc1).SetObject("Desc2", LanguageKey.LK_Building_UpgradeBuildingSlot_Confirm_Desc2).SetObject("NeedResources", new List<ResourceInfo>
		{
			cost
		}).SetObject("HaveResources", new List<int>
		{
			haveResource
		}).SetObject("OnConfirm", new Action(delegate
		{
			this._needUnlockAnimationSlotIndices.Add(index);
			IAsyncMethodRequestHandler parent = this._parent;
			BuildingBlockKey key = this._key;
			int levelSlot2 = levelSlot;
			AsyncMethodCallbackDelegate callback;
			if ((callback = <>9__1) == null)
			{
				callback = (<>9__1 = delegate(int offset, RawDataPool pool)
				{
					bool success = false;
					Serializer.Deserialize(pool, offset, ref success);
					bool flag = success;
					if (flag)
					{
						AudioManager.Instance.PlaySound("ui_industry_put", false, false);
						this.PlayParticle(particleAnchor);
					}
					else
					{
						Debug.LogError("upgrade slot building failed");
					}
				});
			}
			BuildingDomainMethod.AsyncCall.UpgradeSlotBuilding(parent, key, levelSlot2, callback);
		}));
		UIElement.CostResourceConfirm.SetOnInitArgs(argumentBox);
		UIManager.Instance.ShowUI(UIElement.CostResourceConfirm, true);
	}

	// Token: 0x060017D0 RID: 6096 RVA: 0x00092750 File Offset: 0x00090950
	private void PlayParticle(RectTransform particleAnchor)
	{
		BuildingExpandBuildingSlotCommon.EParticle particleType = this.ParticleType;
		if (!true)
		{
		}
		UIParticle uiparticle;
		if (particleType != BuildingExpandBuildingSlotCommon.EParticle.NormalSize)
		{
			if (particleType != BuildingExpandBuildingSlotCommon.EParticle.BigSize)
			{
				uiparticle = null;
			}
			else
			{
				uiparticle = this.bigSizeParticle;
			}
		}
		else
		{
			uiparticle = this.normalSizeParticle;
		}
		if (!true)
		{
		}
		UIParticle particle = uiparticle;
		bool flag = !particle;
		if (!flag)
		{
			particle.transform.position = particleAnchor.position;
			this._particlePlayHelper.PlayOnceParticle(particle, 1f, null);
		}
	}

	// Token: 0x060017D1 RID: 6097 RVA: 0x000927C4 File Offset: 0x000909C4
	private void ResetSlots()
	{
		this._cellContents.Clear();
		for (int i = 0; i < this.GetConfig().Count; i++)
		{
			Transform child = this.slotRoot.Find(string.Format("Slot_{0}", i + 1));
			this._cellContents.Add(child.gameObject);
		}
		this._addButtonRefers.Clear();
		for (int j = 0; j < this.GetConfig().Count; j++)
		{
			Transform transform = this.addButtonRoot.Find(string.Format("AddButton_{0}", j + 1));
			Refers addButton = (transform != null) ? transform.GetComponent<Refers>() : null;
			this._addButtonRefers.Add(addButton);
		}
	}

	// Token: 0x0400131B RID: 4891
	private readonly List<GameObject> _cellContents = new List<GameObject>();

	// Token: 0x0400131C RID: 4892
	private readonly List<Refers> _addButtonRefers = new List<Refers>();

	// Token: 0x0400131D RID: 4893
	private UI_BuildingManage _parent;

	// Token: 0x0400131E RID: 4894
	private BuildingBlockKey _key;

	// Token: 0x0400131F RID: 4895
	private BuildingBlockData _blockData;

	// Token: 0x04001320 RID: 4896
	private readonly UIParticlePlayHelper _particlePlayHelper = new UIParticlePlayHelper();

	// Token: 0x04001321 RID: 4897
	private readonly HashSet<int> _needUnlockAnimationSlotIndices = new HashSet<int>();

	// Token: 0x04001322 RID: 4898
	private readonly Dictionary<int, Sequence> _unlockAnimations = new Dictionary<int, Sequence>();

	// Token: 0x04001323 RID: 4899
	private const float UnlockAnimationOffsetY = -100f;

	// Token: 0x04001324 RID: 4900
	private const float UnlockAnimationDuration = 0.7f;

	// Token: 0x04001325 RID: 4901
	private bool _refreshDirty;

	// Token: 0x04001326 RID: 4902
	[SerializeField]
	private RectTransform slotRoot;

	// Token: 0x04001327 RID: 4903
	[SerializeField]
	private RectTransform addButtonRoot;

	// Token: 0x04001328 RID: 4904
	[SerializeField]
	private UIParticle normalSizeParticle;

	// Token: 0x04001329 RID: 4905
	[SerializeField]
	private UIParticle bigSizeParticle;

	// Token: 0x020012EA RID: 4842
	protected enum EParticle
	{
		// Token: 0x04009C00 RID: 39936
		NormalSize,
		// Token: 0x04009C01 RID: 39937
		BigSize
	}
}
