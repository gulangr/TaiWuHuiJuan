using System;
using System.Collections.Generic;
using System.Linq;
using Coffee.UIExtensions;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Item;
using Game.Views.Building.BuildingManage;
using GameData.Combat.Math;
using GameData.Domains.Building;
using GameData.Domains.Item.Display;
using GameData.Domains.Map;
using GameData.Domains.Taiwu;
using GameData.Domains.World;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.Building
{
	// Token: 0x02000BD3 RID: 3027
	public class BuildingExpandResourceComponent : MonoBehaviour
	{
		// Token: 0x17001051 RID: 4177
		// (get) Token: 0x06009853 RID: 38995 RVA: 0x0046F935 File Offset: 0x0046DB35
		private BuildingModel BuildingModel
		{
			get
			{
				return SingletonObject.getInstance<BuildingModel>();
			}
		}

		// Token: 0x17001052 RID: 4178
		// (get) Token: 0x06009854 RID: 38996 RVA: 0x0046F93C File Offset: 0x0046DB3C
		private BuildingBlockItem BlockConfig
		{
			get
			{
				return BuildingBlock.Instance[this._blockData.TemplateId];
			}
		}

		// Token: 0x17001053 RID: 4179
		// (get) Token: 0x06009855 RID: 38997 RVA: 0x0046F953 File Offset: 0x0046DB53
		private int CurrentLevel
		{
			get
			{
				return this.BuildingModel.GetTaiwuSpecialBuildingLevel(this._key);
			}
		}

		// Token: 0x17001054 RID: 4180
		// (get) Token: 0x06009856 RID: 38998 RVA: 0x0046F966 File Offset: 0x0046DB66
		private int ToConsumeCount
		{
			get
			{
				return Mathf.RoundToInt((float)this.setSelectItems.CurCount);
			}
		}

		// Token: 0x17001055 RID: 4181
		// (get) Token: 0x06009857 RID: 38999 RVA: 0x0046F979 File Offset: 0x0046DB79
		private int ToUpgradeLevelDelta
		{
			get
			{
				return this.ToConsumeCount;
			}
		}

		// Token: 0x17001056 RID: 4182
		// (get) Token: 0x06009858 RID: 39000 RVA: 0x0046F981 File Offset: 0x0046DB81
		private int TargetLevel
		{
			get
			{
				return Math.Min(20, this.CurrentLevel + this.ToUpgradeLevelDelta);
			}
		}

		// Token: 0x17001057 RID: 4183
		// (get) Token: 0x06009859 RID: 39001 RVA: 0x0046F997 File Offset: 0x0046DB97
		private int SliderMax
		{
			get
			{
				return Math.Max(1, Math.Min(this._haveCount, 20 - this.CurrentLevel));
			}
		}

		// Token: 0x0600985A RID: 39002 RVA: 0x0046F9B3 File Offset: 0x0046DBB3
		private void Awake()
		{
			this.SetupSlider();
			this.upgradeButton.ClearAndAddListener(new Action(this.ConfirmUpgrade));
		}

		// Token: 0x0600985B RID: 39003 RVA: 0x0046F9D8 File Offset: 0x0046DBD8
		public void Init(Action onRefresh)
		{
			bool inited = this._inited;
			if (!inited)
			{
				this._onRefresh = onRefresh;
				this.upgradeButtonTips.Type = TipType.SingleDesc;
				this._inited = true;
				this._buildingExpandResourceTable.Init();
			}
		}

		// Token: 0x0600985C RID: 39004 RVA: 0x0046FA18 File Offset: 0x0046DC18
		private void SetupSlider()
		{
			this.setSelectItems.SetInteractable(false);
		}

		// Token: 0x0600985D RID: 39005 RVA: 0x0046FA28 File Offset: 0x0046DC28
		private void OnEnable()
		{
			GEvent.Add(UiEvents.BuildingBlockDataChange, new GEvent.Callback(this.OnBuildingBlockDataChange));
		}

		// Token: 0x0600985E RID: 39006 RVA: 0x0046FA44 File Offset: 0x0046DC44
		private void OnDisable()
		{
			GEvent.Remove(UiEvents.BuildingBlockDataChange, new GEvent.Callback(this.OnBuildingBlockDataChange));
		}

		// Token: 0x0600985F RID: 39007 RVA: 0x0046FA60 File Offset: 0x0046DC60
		private void OnBuildingBlockDataChange(ArgumentBox argBox)
		{
			this.RefreshInner();
		}

		// Token: 0x06009860 RID: 39008 RVA: 0x0046FA6A File Offset: 0x0046DC6A
		public void Refresh(BuildingBlockKey key, BuildingBlockData blockData)
		{
			this._key = key;
			this._blockData = blockData;
			this.RefreshInner();
		}

		// Token: 0x06009861 RID: 39009 RVA: 0x0046FA84 File Offset: 0x0046DC84
		private void RefreshInner()
		{
			bool flag = this.BlockConfig.Class != EBuildingBlockClass.BornResource;
			if (!flag)
			{
				bool flag2 = this.BlockConfig.Type == EBuildingBlockType.UselessResource;
				if (!flag2)
				{
					int currentLevel = this.CurrentLevel;
					this.RefreshIcon();
					this.RefreshCurrentLevelLabel();
					this.RefreshCoreItem();
					this.GetAllItems();
					this.RefreshUpgradeButtonInteractable();
					this.RefreshEffectInfo();
				}
			}
		}

		// Token: 0x06009862 RID: 39010 RVA: 0x0046FAF0 File Offset: 0x0046DCF0
		private void ConfigLevelSlider()
		{
			this.setSelectItems.Rerfresh(this.SliderMax, 1, 1, false, false, 1, new Action<int>(this.OnSliderValueChanged));
			this.setSelectItems.SetInteractable(true);
			this.OnSliderValueChanged(1);
		}

		// Token: 0x06009863 RID: 39011 RVA: 0x0046FB36 File Offset: 0x0046DD36
		private void RefreshIcon()
		{
			ViewBuildingArea.SetBuildingIcon(this.buildingIcon, this.BlockConfig, false, null);
		}

		// Token: 0x06009864 RID: 39012 RVA: 0x0046FB50 File Offset: 0x0046DD50
		private void RefreshCurrentLevelLabel()
		{
			bool isMaxLevel = this.CurrentLevel == 20;
			this.currentLevelLabel.gameObject.SetActive(!isMaxLevel);
			this.currentLevelLabel.text = this.CurrentLevel.ToString();
			this.sliderArea.SetActive(!isMaxLevel);
			this.maxHintArea.SetActive(isMaxLevel);
		}

		// Token: 0x06009865 RID: 39013 RVA: 0x0046FBB8 File Offset: 0x0046DDB8
		private void GetAllItems()
		{
			this._itemDict.Clear();
			int c = 0;
			AsyncMethodCallbackDelegate <>9__0;
			foreach (ItemSourceType itemSource in BuildingExpandResourceComponent.ItemSources)
			{
				IAsyncMethodRequestHandler requestHandler = null;
				ItemSourceType itemSourceType = itemSource;
				AsyncMethodCallbackDelegate callback;
				if ((callback = <>9__0) == null)
				{
					callback = (<>9__0 = delegate(int offset, RawDataPool pool)
					{
						ValueTuple<ItemSourceType, List<ItemDisplayData>> tuple = default(ValueTuple<ItemSourceType, List<ItemDisplayData>>);
						Serializer.Deserialize(pool, offset, ref tuple);
						this._itemDict[tuple.Item1] = tuple.Item2;
						int c = c;
						c++;
						bool flag = c == BuildingExpandResourceComponent.ItemSources.Count;
						if (flag)
						{
							this.OnGotAllItems();
						}
					});
				}
				TaiwuDomainMethod.AsyncCall.GetAllItems(requestHandler, itemSourceType, callback);
			}
		}

		// Token: 0x06009866 RID: 39014 RVA: 0x0046FC50 File Offset: 0x0046DE50
		private void OnGotAllItems()
		{
			this._haveCount = this.CountItem(12, this.BlockConfig.BuildingCoreItem);
			this.ConfigLevelSlider();
			this.RefreshUpgradeButtonInteractable();
			this.RefreshNeedItemInfo();
		}

		// Token: 0x06009867 RID: 39015 RVA: 0x0046FC84 File Offset: 0x0046DE84
		private int CountItem(sbyte itemType, short itemTemplateId)
		{
			return (from item in (from v in this._itemDict.Values
			where v != null
			select v).SelectMany((List<ItemDisplayData> list) => list)
			where item.Key.ItemType == itemType && item.Key.TemplateId == itemTemplateId && !item.IsLocked
			select item).Sum((ItemDisplayData item) => item.Amount);
		}

		// Token: 0x06009868 RID: 39016 RVA: 0x0046FD34 File Offset: 0x0046DF34
		private void RefreshCoreItem()
		{
			MiscItem itemConfig = Misc.Instance[this.BlockConfig.BuildingCoreItem];
			this.coreItemView.SetIcon(itemConfig.Icon);
			this.coreItemView.SetBack(itemConfig.Grade);
			this.mouseTip.Type = TipType.Misc;
			this.mouseTip.RuntimeParam = EasyPool.Get<ArgumentBox>().Set("TemplateDataOnly", false).SetObject("ItemData", new ItemDisplayData(12, itemConfig.TemplateId));
		}

		// Token: 0x06009869 RID: 39017 RVA: 0x0046FDBC File Offset: 0x0046DFBC
		private void ConfirmUpgrade()
		{
			DialogCmd cmd = new DialogCmd
			{
				Title = LocalStringManager.Get(LanguageKey.LK_Building_Expand_UpgradeResourceBuilding_Title),
				Content = LocalStringManager.Get(LanguageKey.LK_Building_Expand_UpgradeResourceBuilding_Dialog_Content),
				Type = 1,
				Yes = new Action(this.ReallyUpgrade),
				No = null
			};
			UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
			UIManager.Instance.MaskUI(UIElement.Dialog);
		}

		// Token: 0x0600986A RID: 39018 RVA: 0x0046FE3B File Offset: 0x0046E03B
		private void ReallyUpgrade()
		{
			BuildingDomainMethod.AsyncCall.UpgradeResourceBuilding(null, this._key, this.ToUpgradeLevelDelta, delegate(int offset, RawDataPool pool)
			{
				bool success = false;
				Serializer.Deserialize(pool, offset, ref success);
				bool flag = success;
				if (flag)
				{
					Action onRefresh = this._onRefresh;
					if (onRefresh != null)
					{
						onRefresh();
					}
					this.buildingParticle.Play();
				}
			});
		}

		// Token: 0x0600986B RID: 39019 RVA: 0x0046FE5D File Offset: 0x0046E05D
		private void OnSliderValueChanged(int value)
		{
			this.OnTargetLevelChanged();
		}

		// Token: 0x0600986C RID: 39020 RVA: 0x0046FE67 File Offset: 0x0046E067
		private void OnTargetLevelChanged()
		{
			this.RefreshTargetLevelLabel();
			this.RefreshNeedItemInfo();
			this.RefreshScales();
			this.RefreshUpgradeButtonInteractable();
		}

		// Token: 0x0600986D RID: 39021 RVA: 0x0046FE88 File Offset: 0x0046E088
		private void RefreshNeedItemInfo()
		{
			string haveTxt = this._haveCount.ToString().SetColor((this._haveCount < this.ToConsumeCount) ? "brightred" : "brightblue");
			this.coreItemNeedCountTxt.text = haveTxt + "/" + this.ToConsumeCount.ToString();
		}

		// Token: 0x0600986E RID: 39022 RVA: 0x0046FEE8 File Offset: 0x0046E0E8
		private void RefreshScales()
		{
			this._scaleInfos.Clear();
			foreach (short scaleTemplateId in this.BlockConfig.ExpandInfos)
			{
				BuildingScaleItem config = BuildingScale.Instance[scaleTemplateId];
				bool flag = config.Formula >= 0;
				if (!flag)
				{
					this._scaleInfos.Add(new BuildingExpandResourceComponent.BuildingScaleInfo
					{
						ScaleTemplateId = scaleTemplateId
					});
				}
			}
			int currentLevel = this.CurrentLevel;
			Refers refers = this.scaleLayoutRoot.GetComponent<Refers>();
			bool flag2 = this._scaleInfos.Count < 1;
			if (!flag2)
			{
				BuildingExpandResourceComponent.BuildingScaleInfo scaleInfo = this._scaleInfos[0];
				TextMeshProUGUI currentValue = refers.CGet<TextMeshProUGUI>("CurrentValue");
				TextMeshProUGUI nameLabel = refers.CGet<TextMeshProUGUI>("NameLabel");
				TextMeshProUGUI previewValue = refers.CGet<TextMeshProUGUI>("PreviewValue");
				GameObject arrow = refers.CGet<GameObject>("Arrow");
				BuildingScaleItem scaleConfig = BuildingScale.Instance[scaleInfo.ScaleTemplateId];
				currentValue.text = scaleConfig.LevelEffect[currentLevel - 1].ToString();
				nameLabel.text = scaleConfig.Name;
				bool flag3 = this.TargetLevel > currentLevel;
				if (flag3)
				{
					previewValue.text = scaleConfig.LevelEffect[this.TargetLevel - 1].ToString();
					arrow.SetActive(true);
				}
				else
				{
					bool flag4 = currentLevel == 20;
					if (flag4)
					{
						currentValue.text = string.Empty;
						previewValue.text = scaleConfig.LevelEffect[currentLevel - 1].ToString().SetColor("pinkyellow");
					}
					else
					{
						previewValue.text = string.Empty;
					}
					arrow.SetActive(false);
				}
			}
		}

		// Token: 0x0600986F RID: 39023 RVA: 0x004700DC File Offset: 0x0046E2DC
		private bool CanUpgrade()
		{
			return this.CurrentLevel < 20 && this._haveCount > 0;
		}

		// Token: 0x06009870 RID: 39024 RVA: 0x00470104 File Offset: 0x0046E304
		private void RefreshTargetLevelLabel()
		{
			bool larger = this.TargetLevel > this.CurrentLevel;
			bool isMaxLevel = this.CurrentLevel == 20;
			this.targetLevelLabel.gameObject.SetActive(larger || isMaxLevel);
			this.targetLevelLabel.text = this.TargetLevel.ToString().SetColor(isMaxLevel ? "pinkyellow" : "brightblue");
		}

		// Token: 0x06009871 RID: 39025 RVA: 0x00470170 File Offset: 0x0046E370
		private void RefreshUpgradeButtonInteractable()
		{
			bool notEnoughAmount = this._haveCount <= 0;
			bool reachMaxLevel = this.CurrentLevel >= 20;
			this.upgradeButton.interactable = (!notEnoughAmount && !reachMaxLevel);
			this.upgradeButtonTips.enabled = !this.upgradeButton.interactable;
			bool flag = reachMaxLevel;
			if (flag)
			{
				TooltipInvoker tooltipInvoker = this.upgradeButtonTips;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = new ArgumentBox();
				}
				this.upgradeButtonTips.RuntimeParam.Set("arg0", LanguageKey.LK_Mousetip_ExpandResource_UpgradeDisable_0.Tr());
			}
			bool flag2 = notEnoughAmount;
			if (flag2)
			{
				TooltipInvoker tooltipInvoker = this.upgradeButtonTips;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = new ArgumentBox();
				}
				this.upgradeButtonTips.RuntimeParam.Set("arg0", LanguageKey.LK_Mousetip_ExpandResource_UpgradeDisable_1.Tr());
			}
		}

		// Token: 0x06009872 RID: 39026 RVA: 0x00470255 File Offset: 0x0046E455
		private static void RefreshAddReduceButtonByInteractable(Transform transform, bool interactable)
		{
			transform.GetComponent<PointerTrigger>().enabled = interactable;
		}

		// Token: 0x06009873 RID: 39027 RVA: 0x00470268 File Offset: 0x0046E468
		private void RefreshEffectInfo()
		{
			this._scaleTemplateIdList = GameData.Domains.Building.SharedMethods.GetResourceBlockEffectScaleTemplateIdList(this._blockData.TemplateId);
			this._buildingExpandResourceTable.Setup(this._blockData);
			this._buildingExpandResourceTable.SetupPage(this._scaleTemplateIdList);
			BuildingDomainMethod.AsyncCall.GetTaiwuVillageResourceBlockEffectInfo(null, this._blockData.TemplateId, delegate(int offset, RawDataPool dataPool)
			{
				Serializer.Deserialize(dataPool, offset, ref this._blocksInfo);
				this._buildingExpandResourceTable.UpdateData(this._blocksInfo);
				this.effectDesc.SetText(this.BlockConfig.EffectDesc, true);
				RectTransform holder = this.effectHolder;
				for (int i = 0; i < this._scaleTemplateIdList.Count; i++)
				{
					Transform child = holder.GetChild(i);
					child.gameObject.SetActive(i < this._scaleTemplateIdList.Count);
					bool activeInHierarchy = child.gameObject.activeInHierarchy;
					if (activeInHierarchy)
					{
						BuildingScaleItem scaleConfig = BuildingScale.Instance[this._scaleTemplateIdList[i]];
						Refers childRefers = child.GetComponent<Refers>();
						childRefers.CGet<TextMeshProUGUI>("Title").SetText(scaleConfig.Name, true);
						TextMeshProUGUI value = childRefers.CGet<TextMeshProUGUI>("Value");
						CImage arrowIcon = childRefers.CGet<CImage>("Icon");
						arrowIcon.gameObject.SetActive(false);
						int sum = this.GetEffectSum(scaleConfig.Formula);
						TooltipInvoker mouseTip = childRefers.CGet<TooltipInvoker>("MouseTip");
						mouseTip.enabled = (scaleConfig.Class == EBuildingScaleClass.MemberResourceIncome);
						string difficultyIconName = string.Empty;
						bool flag = scaleConfig.Class == EBuildingScaleClass.MemberResourceIncome;
						if (flag)
						{
							byte worldResourceType = (scaleConfig.ResourceType < 6) ? 4 : 5;
							CValuePercent percent = (int)GameData.Domains.World.SharedMethods.GetGainResourcePercent(worldResourceType);
							sum *= percent;
							bool flag2 = (int)percent > 100;
							if (flag2)
							{
								difficultyIconName = "ui9_back_expand_resource_arrow_0";
								arrowIcon.rectTransform.localScale = this._inverseScale;
							}
							bool flag3 = (int)percent < 100;
							if (flag3)
							{
								difficultyIconName = "ui9_back_expand_resource_arrow_1";
								arrowIcon.rectTransform.localScale = this._reverseScale;
							}
							TooltipInvoker tooltipInvoker = mouseTip;
							if (tooltipInvoker.RuntimeParam == null)
							{
								tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
							}
							mouseTip.RuntimeParam.Set("arg0", LocalStringManager.Get(LanguageKey.LK_Building_PredictProduct_Tips_Text_20) + ((int)percent).ToString() + "%");
						}
						string valueStr = ViewBuildingManage.GetBuildingScaleFormatString(scaleConfig.Type, sum);
						bool flag4 = !difficultyIconName.IsNullOrEmpty();
						if (flag4)
						{
							arrowIcon.gameObject.SetActive(true);
							arrowIcon.SetSprite(difficultyIconName, false, null);
						}
						value.SetText(valueStr, true);
					}
				}
				for (int j = this._scaleTemplateIdList.Count; j < holder.childCount; j++)
				{
					holder.GetChild(j).gameObject.SetActive(false);
				}
			});
		}

		// Token: 0x06009874 RID: 39028 RVA: 0x004702D0 File Offset: 0x0046E4D0
		private int GetEffectSum(int formulaTemplateId)
		{
			int[] baseValue = new int[Math.Min(this._blocksInfo.Count, 5)];
			for (int index = 0; index < baseValue.Length; index++)
			{
				int level = this.GetLevel(index);
				int percentage = GameData.Domains.Building.SharedMethods.GetResourceBlockEffectPercentage(index);
				int effectValue = GameData.Domains.Building.SharedMethods.GetResourceBlockEffectPercentageValue(level, percentage);
				baseValue[index] = effectValue;
			}
			return GameData.Domains.Building.SharedMethods.CalcResourceBlockTotalEffectValue(formulaTemplateId, baseValue);
		}

		// Token: 0x06009875 RID: 39029 RVA: 0x00470340 File Offset: 0x0046E540
		private int GetLevel(int index)
		{
			BuildingBlockData blockData = this._blocksInfo[index];
			Location taiwuVillageLocation = SingletonObject.getInstance<WorldMapModel>().GetTaiwuVillageBlock();
			BuildingBlockKey blockKey = new BuildingBlockKey(taiwuVillageLocation.AreaId, taiwuVillageLocation.BlockId, blockData.BlockIndex);
			return (int)this.BuildingModel.GetBuildingLevel(blockKey, blockData);
		}

		// Token: 0x04007529 RID: 29993
		[SerializeField]
		private BuildingExpandResourceTableComponent _buildingExpandResourceTable;

		// Token: 0x0400752A RID: 29994
		private readonly Dictionary<ItemSourceType, List<ItemDisplayData>> _itemDict = new Dictionary<ItemSourceType, List<ItemDisplayData>>();

		// Token: 0x0400752B RID: 29995
		private static readonly List<ItemSourceType> ItemSources = new List<ItemSourceType>
		{
			ItemSourceType.Inventory,
			ItemSourceType.Warehouse,
			ItemSourceType.Treasury
		};

		// Token: 0x0400752C RID: 29996
		private BuildingBlockKey _key;

		// Token: 0x0400752D RID: 29997
		private BuildingBlockData _blockData;

		// Token: 0x0400752E RID: 29998
		private const int MaxLevel = 20;

		// Token: 0x0400752F RID: 29999
		private const int MaxScaleAmount = 3;

		// Token: 0x04007530 RID: 30000
		private int _haveCount;

		// Token: 0x04007531 RID: 30001
		private bool _inited = false;

		// Token: 0x04007532 RID: 30002
		private Action _onRefresh;

		// Token: 0x04007533 RID: 30003
		private Vector3 _reverseScale = new Vector3(1f, -1f, 1f);

		// Token: 0x04007534 RID: 30004
		private Vector3 _inverseScale = new Vector3(1f, 1f, 1f);

		// Token: 0x04007535 RID: 30005
		private readonly List<BuildingExpandResourceComponent.BuildingScaleInfo> _scaleInfos = new List<BuildingExpandResourceComponent.BuildingScaleInfo>();

		// Token: 0x04007536 RID: 30006
		private List<BuildingBlockData> _blocksInfo;

		// Token: 0x04007537 RID: 30007
		private List<short> _scaleTemplateIdList;

		// Token: 0x04007538 RID: 30008
		[SerializeField]
		private CImage buildingIcon;

		// Token: 0x04007539 RID: 30009
		[SerializeField]
		private ItemBack coreItemView;

		// Token: 0x0400753A RID: 30010
		[SerializeField]
		private TooltipInvoker mouseTip;

		// Token: 0x0400753B RID: 30011
		[SerializeField]
		private TextMeshProUGUI coreItemNeedCountTxt;

		// Token: 0x0400753C RID: 30012
		[SerializeField]
		private TextMeshProUGUI currentLevelLabel;

		// Token: 0x0400753D RID: 30013
		[SerializeField]
		private TextMeshProUGUI targetLevelLabel;

		// Token: 0x0400753E RID: 30014
		[SerializeField]
		private ExpandResourceSetSelectAmount setSelectItems;

		// Token: 0x0400753F RID: 30015
		[SerializeField]
		private CButton upgradeButton;

		// Token: 0x04007540 RID: 30016
		[SerializeField]
		private TooltipInvoker upgradeButtonTips;

		// Token: 0x04007541 RID: 30017
		[SerializeField]
		private RectTransform scaleLayoutRoot;

		// Token: 0x04007542 RID: 30018
		[SerializeField]
		private GameObject sliderArea;

		// Token: 0x04007543 RID: 30019
		[SerializeField]
		private GameObject maxHintArea;

		// Token: 0x04007544 RID: 30020
		[Header("Effect")]
		[SerializeField]
		private RectTransform effectTitle;

		// Token: 0x04007545 RID: 30021
		[SerializeField]
		private TextMeshProUGUI effectDesc;

		// Token: 0x04007546 RID: 30022
		[SerializeField]
		private RectTransform effectHolder;

		// Token: 0x04007547 RID: 30023
		[SerializeField]
		private UIParticle buildingParticle;

		// Token: 0x0200229C RID: 8860
		private struct BuildingScaleInfo
		{
			// Token: 0x0400DB8F RID: 56207
			public short ScaleTemplateId;
		}
	}
}
