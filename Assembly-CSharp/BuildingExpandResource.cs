using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork;
using Game.Views.Building;
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
using UnityEngine.Events;

// Token: 0x020001A6 RID: 422
public class BuildingExpandResource : MonoBehaviour
{
	// Token: 0x17000293 RID: 659
	// (get) Token: 0x060017D9 RID: 6105 RVA: 0x00092920 File Offset: 0x00090B20
	private BuildingModel BuildingModel
	{
		get
		{
			return SingletonObject.getInstance<BuildingModel>();
		}
	}

	// Token: 0x17000294 RID: 660
	// (get) Token: 0x060017DA RID: 6106 RVA: 0x00092927 File Offset: 0x00090B27
	private BuildingBlockItem BlockConfig
	{
		get
		{
			return BuildingBlock.Instance[this._blockData.TemplateId];
		}
	}

	// Token: 0x17000295 RID: 661
	// (get) Token: 0x060017DB RID: 6107 RVA: 0x0009293E File Offset: 0x00090B3E
	private int CurrentLevel
	{
		get
		{
			return this.BuildingModel.GetTaiwuSpecialBuildingLevel(this._key);
		}
	}

	// Token: 0x17000296 RID: 662
	// (get) Token: 0x060017DC RID: 6108 RVA: 0x00092951 File Offset: 0x00090B51
	private int ToConsumeCount
	{
		get
		{
			return Mathf.RoundToInt(this.levelSlider.value);
		}
	}

	// Token: 0x17000297 RID: 663
	// (get) Token: 0x060017DD RID: 6109 RVA: 0x00092963 File Offset: 0x00090B63
	private int ToUpgradeLevelDelta
	{
		get
		{
			return this.ToConsumeCount;
		}
	}

	// Token: 0x17000298 RID: 664
	// (get) Token: 0x060017DE RID: 6110 RVA: 0x0009296B File Offset: 0x00090B6B
	private int TargetLevel
	{
		get
		{
			return Math.Min(20, this.CurrentLevel + this.ToUpgradeLevelDelta);
		}
	}

	// Token: 0x17000299 RID: 665
	// (get) Token: 0x060017DF RID: 6111 RVA: 0x00092981 File Offset: 0x00090B81
	private int SliderMax
	{
		get
		{
			return Math.Max(1, Math.Min(this._haveCount, 20 - this.CurrentLevel));
		}
	}

	// Token: 0x060017E0 RID: 6112 RVA: 0x000929A0 File Offset: 0x00090BA0
	private void Awake()
	{
		this.SetupLevelBalls();
		this.SetupSlider();
		this.upgradeButton.ClearAndAddListener(new Action(this.ConfirmUpgrade));
		this.addButton.ClearAndAddListener(delegate
		{
			this.levelSlider.value += 1f;
		});
		this.reduceButton.ClearAndAddListener(delegate
		{
			this.levelSlider.value -= 1f;
		});
	}

	// Token: 0x060017E1 RID: 6113 RVA: 0x00092A04 File Offset: 0x00090C04
	private void SetupLevelBalls()
	{
		this._levelBallList.Clear();
		for (int i = 0; i < this.levelBallRoot.childCount; i++)
		{
			GameObject child = this.levelBallRoot.GetChild(i).gameObject;
			this._levelBallList.Add(child);
			child.SetActive(false);
		}
		for (int j = 0; j < this.levelBallGroupRoot.childCount; j++)
		{
			GameObject child2 = this.levelBallGroupRoot.GetChild(j).gameObject;
			this._levelBallGroupList.Add(child2.GetComponent<CImage>());
			this._levelBallGroupListActive.Add(child2.transform.GetChild(0).GetComponent<CImage>());
		}
	}

	// Token: 0x060017E2 RID: 6114 RVA: 0x00092AC3 File Offset: 0x00090CC3
	private void SetupSlider()
	{
		this.levelSlider.onValueChanged.AddListener(new UnityAction<float>(this.OnSliderValueChanged));
		this.levelSlider.interactable = false;
	}

	// Token: 0x060017E3 RID: 6115 RVA: 0x00092AF0 File Offset: 0x00090CF0
	private void OnEnable()
	{
		GEvent.Add(UiEvents.BuildingBlockDataChange, new GEvent.Callback(this.OnBuildingBlockDataChange));
	}

	// Token: 0x060017E4 RID: 6116 RVA: 0x00092B0C File Offset: 0x00090D0C
	private void OnDisable()
	{
		GEvent.Remove(UiEvents.BuildingBlockDataChange, new GEvent.Callback(this.OnBuildingBlockDataChange));
	}

	// Token: 0x060017E5 RID: 6117 RVA: 0x00092B28 File Offset: 0x00090D28
	private void OnBuildingBlockDataChange(ArgumentBox argBox)
	{
		this.RefreshInner();
	}

	// Token: 0x060017E6 RID: 6118 RVA: 0x00092B32 File Offset: 0x00090D32
	public void Refresh(UI_BuildingManage parent, BuildingBlockKey key, BuildingBlockData blockData)
	{
		this._parent = parent;
		this._key = key;
		this._blockData = blockData;
		this.RefreshInner();
	}

	// Token: 0x060017E7 RID: 6119 RVA: 0x00092B54 File Offset: 0x00090D54
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
				this.RefreshLevelBalls(currentLevel);
				this.RefreshCoreItem();
				this.GetAllItems();
				this.RefreshUpgradeButtonInteractable();
				this.RefreshEffectInfo();
			}
		}
	}

	// Token: 0x060017E8 RID: 6120 RVA: 0x00092BC8 File Offset: 0x00090DC8
	private void ConfigLevelSlider()
	{
		this.levelSlider.minValue = 0f;
		this.levelSlider.maxValue = (float)this.SliderMax;
		this.levelSlider.interactable = true;
		this.levelSlider.value = 1f;
		this.OnSliderValueChanged(1f);
	}

	// Token: 0x060017E9 RID: 6121 RVA: 0x00092C24 File Offset: 0x00090E24
	private void RefreshIcon()
	{
		ViewBuildingArea.SetBuildingIcon(this.buildingIcon, this.BlockConfig, false, null);
	}

	// Token: 0x060017EA RID: 6122 RVA: 0x00092C3C File Offset: 0x00090E3C
	private void RefreshCurrentLevelLabel()
	{
		bool isMaxLevel = this.CurrentLevel == 20;
		this.currentLevelLabel.gameObject.SetActive(!isMaxLevel);
		this.currentLevelLabel.text = this.CurrentLevel.ToString();
		this.sliderArea.SetActive(!isMaxLevel);
		this.maxHintArea.SetActive(isMaxLevel);
	}

	// Token: 0x060017EB RID: 6123 RVA: 0x00092CA4 File Offset: 0x00090EA4
	private void GetAllItems()
	{
		this._itemDict.Clear();
		int c = 0;
		AsyncMethodCallbackDelegate <>9__0;
		foreach (ItemSourceType itemSource in BuildingExpandResource.ItemSources)
		{
			IAsyncMethodRequestHandler parent = this._parent;
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
					bool flag = c == BuildingExpandResource.ItemSources.Count;
					if (flag)
					{
						this.OnGotAllItems();
					}
				});
			}
			TaiwuDomainMethod.AsyncCall.GetAllItems(parent, itemSourceType, callback);
		}
	}

	// Token: 0x060017EC RID: 6124 RVA: 0x00092D40 File Offset: 0x00090F40
	private void OnGotAllItems()
	{
		this._haveCount = this.CountItem(12, this.BlockConfig.BuildingCoreItem);
		this.ConfigLevelSlider();
		this.RefreshHaveCountLabel();
		this.RefreshAddReduce();
		this.RefreshUpgradeButtonInteractable();
	}

	// Token: 0x060017ED RID: 6125 RVA: 0x00092D78 File Offset: 0x00090F78
	private void RefreshHaveCountLabel()
	{
		this.coreItemHaveCountLabel.text = this._haveCount.ToString().SetColor((this._haveCount == 0) ? "brightred" : "brightblue");
	}

	// Token: 0x060017EE RID: 6126 RVA: 0x00092DAC File Offset: 0x00090FAC
	private int CountItem(sbyte itemType, short itemTemplateId)
	{
		return (from item in (from v in this._itemDict.Values
		where v != null
		select v).SelectMany((List<ItemDisplayData> list) => list)
		where item.Key.ItemType == itemType && item.Key.TemplateId == itemTemplateId
		select item).Sum((ItemDisplayData item) => item.Amount);
	}

	// Token: 0x060017EF RID: 6127 RVA: 0x00092E5C File Offset: 0x0009105C
	private void RefreshLevelBalls(int level)
	{
		for (int i = 0; i < this._levelBallList.Count; i++)
		{
			this._levelBallList[i].SetActive(i < level);
		}
		for (int j = 0; j < this._levelBallGroupList.Count; j++)
		{
			bool isHighlight = j < level / 5;
			this._levelBallGroupListActive[j].gameObject.SetActive(isHighlight);
		}
	}

	// Token: 0x060017F0 RID: 6128 RVA: 0x00092EDC File Offset: 0x000910DC
	private void RefreshCoreItem()
	{
		this.coreItemView.SetData(new ItemDisplayData(12, this.BlockConfig.BuildingCoreItem), -1);
		this.needItemName.text = Misc.Instance[this.BlockConfig.BuildingCoreItem].Name;
	}

	// Token: 0x060017F1 RID: 6129 RVA: 0x00092F30 File Offset: 0x00091130
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

	// Token: 0x060017F2 RID: 6130 RVA: 0x00092FAF File Offset: 0x000911AF
	private void ReallyUpgrade()
	{
		BuildingDomainMethod.AsyncCall.UpgradeResourceBuilding(this._parent, this._key, this.ToUpgradeLevelDelta, delegate(int offset, RawDataPool pool)
		{
			bool success = false;
			Serializer.Deserialize(pool, offset, ref success);
			bool flag = success;
			if (flag)
			{
			}
		});
	}

	// Token: 0x060017F3 RID: 6131 RVA: 0x00092FEC File Offset: 0x000911EC
	private void OnSliderValueChanged(float value)
	{
		bool flag = this.ClampSliderValue();
		if (!flag)
		{
			this.OnTargetLevelChanged();
		}
	}

	// Token: 0x060017F4 RID: 6132 RVA: 0x0009300D File Offset: 0x0009120D
	private void OnTargetLevelChanged()
	{
		this.RefreshTargetLevelLabel();
		this.RefreshAddReduce();
		this.RefreshNeedItemInfo();
		this.RefreshScales();
		this.RefreshUpgradeButtonInteractable();
	}

	// Token: 0x060017F5 RID: 6133 RVA: 0x00093033 File Offset: 0x00091233
	private void RefreshNeedItemInfo()
	{
		this.RefreshNeedCountLabel();
		this.RefreshHaveCountLabel();
	}

	// Token: 0x060017F6 RID: 6134 RVA: 0x00093044 File Offset: 0x00091244
	private bool ClampSliderValue()
	{
		bool flag = this.levelSlider.value < 1f;
		bool result;
		if (flag)
		{
			this.levelSlider.value = 1f;
			result = true;
		}
		else
		{
			result = false;
		}
		return result;
	}

	// Token: 0x060017F7 RID: 6135 RVA: 0x00093084 File Offset: 0x00091284
	private void RefreshScales()
	{
		this._scaleInfos.Clear();
		foreach (short scaleTemplateId in this.BlockConfig.ExpandInfos)
		{
			BuildingScaleItem config = BuildingScale.Instance[scaleTemplateId];
			bool flag = config.Formula >= 0;
			if (!flag)
			{
				this._scaleInfos.Add(new BuildingExpandResource.BuildingScaleInfo
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
			BuildingExpandResource.BuildingScaleInfo scaleInfo = this._scaleInfos[0];
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

	// Token: 0x060017F8 RID: 6136 RVA: 0x00093278 File Offset: 0x00091478
	private void RefreshAddReduce()
	{
		bool canAdd = this.levelSlider.value < (float)this.SliderMax;
		BuildingExpandResource.RefreshAddReduceButtonByInteractable(this.addButton.transform, canAdd);
		this.addButton.interactable = canAdd;
		bool canReduce = this.levelSlider.value > 1f;
		this.reduceButton.interactable = canReduce;
		BuildingExpandResource.RefreshAddReduceButtonByInteractable(this.reduceButton.transform, canReduce);
	}

	// Token: 0x060017F9 RID: 6137 RVA: 0x000932EC File Offset: 0x000914EC
	private bool CanUpgrade()
	{
		return this.CurrentLevel < 20 && this._haveCount > 0;
	}

	// Token: 0x060017FA RID: 6138 RVA: 0x00093314 File Offset: 0x00091514
	private void RefreshNeedCountLabel()
	{
		this.coreItemNeedCountLabel.text = this.ToConsumeCount.ToString();
	}

	// Token: 0x060017FB RID: 6139 RVA: 0x0009333C File Offset: 0x0009153C
	private void RefreshTargetLevelLabel()
	{
		bool larger = this.TargetLevel > this.CurrentLevel;
		bool isMaxLevel = this.CurrentLevel == 20;
		this.targetLevelLabel.gameObject.SetActive(larger || isMaxLevel);
		this.targetLevelLabel.text = this.TargetLevel.ToString().SetColor(isMaxLevel ? "pinkyellow" : "brightblue");
		this.levelUpArrow.SetActive(larger);
	}

	// Token: 0x060017FC RID: 6140 RVA: 0x000933B4 File Offset: 0x000915B4
	private void RefreshUpgradeButtonInteractable()
	{
		bool interactable = this.CanUpgrade();
		this.upgradeButton.interactable = interactable;
	}

	// Token: 0x060017FD RID: 6141 RVA: 0x000933D6 File Offset: 0x000915D6
	private static void RefreshAddReduceButtonByInteractable(Transform transform, bool interactable)
	{
		transform.GetComponent<PointerTrigger>().enabled = interactable;
	}

	// Token: 0x060017FE RID: 6142 RVA: 0x000933E8 File Offset: 0x000915E8
	private void RefreshEffectInfo()
	{
		this._scaleTemplateIdList = GameData.Domains.Building.SharedMethods.GetResourceBlockEffectScaleTemplateIdList(this._blockData.TemplateId);
		this._buildingExpandResourceTable.Setup(this._blockData);
		this._buildingExpandResourceTable.SetupPage(this._scaleTemplateIdList);
		Refers refers = base.GetComponent<Refers>();
		BuildingDomainMethod.AsyncCall.GetTaiwuVillageResourceBlockEffectInfo(null, this._blockData.TemplateId, delegate(int offset, RawDataPool dataPool)
		{
			Serializer.Deserialize(dataPool, offset, ref this._blocksInfo);
			this._buildingExpandResourceTable.UpdateData(this._blocksInfo);
			refers.CGet<TextMeshProUGUI>("EffectDesc").SetText(this.BlockConfig.EffectDesc, true);
			RectTransform holder = refers.CGet<RectTransform>("EffectHolder");
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
							difficultyIconName = "sp_01_gn_shuxingxiugai_2";
						}
						bool flag3 = (int)percent < 100;
						if (flag3)
						{
							difficultyIconName = "sp_01_gn_shuxingxiugai_0";
						}
						TooltipInvoker tooltipInvoker = mouseTip;
						if (tooltipInvoker.RuntimeParam == null)
						{
							tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
						}
						mouseTip.RuntimeParam.Set("arg0", LocalStringManager.Get(LanguageKey.LK_Building_PredictProduct_Tips_Text_20) + ((int)percent).ToString() + "%");
					}
					string valueStr = UI_BuildingManage.GetBuildingScaleFormatString(scaleConfig.Type, sum);
					bool flag4 = !difficultyIconName.IsNullOrEmpty();
					if (flag4)
					{
						valueStr = "<SpName=" + difficultyIconName + ">" + valueStr;
					}
					value.SetText(valueStr, true);
					value.GetComponent<TMPTextSpriteHelper>().Parse();
				}
			}
		});
	}

	// Token: 0x060017FF RID: 6143 RVA: 0x00093468 File Offset: 0x00091668
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

	// Token: 0x06001800 RID: 6144 RVA: 0x000934D8 File Offset: 0x000916D8
	private int GetLevel(int index)
	{
		BuildingBlockData blockData = this._blocksInfo[index];
		Location taiwuVillageLocation = SingletonObject.getInstance<WorldMapModel>().GetTaiwuVillageBlock();
		BuildingBlockKey blockKey = new BuildingBlockKey(taiwuVillageLocation.AreaId, taiwuVillageLocation.BlockId, blockData.BlockIndex);
		return (int)this.BuildingModel.GetBuildingLevel(blockKey, blockData);
	}

	// Token: 0x0400132A RID: 4906
	[SerializeField]
	private BuildingExpandResourceTable _buildingExpandResourceTable;

	// Token: 0x0400132B RID: 4907
	private readonly List<GameObject> _levelBallList = new List<GameObject>();

	// Token: 0x0400132C RID: 4908
	private readonly List<CImage> _levelBallGroupList = new List<CImage>();

	// Token: 0x0400132D RID: 4909
	private readonly List<CImage> _levelBallGroupListActive = new List<CImage>();

	// Token: 0x0400132E RID: 4910
	private readonly Dictionary<ItemSourceType, List<ItemDisplayData>> _itemDict = new Dictionary<ItemSourceType, List<ItemDisplayData>>();

	// Token: 0x0400132F RID: 4911
	private static readonly List<ItemSourceType> ItemSources = new List<ItemSourceType>
	{
		ItemSourceType.Inventory,
		ItemSourceType.Warehouse,
		ItemSourceType.Treasury
	};

	// Token: 0x04001330 RID: 4912
	private UI_BuildingManage _parent;

	// Token: 0x04001331 RID: 4913
	private BuildingBlockKey _key;

	// Token: 0x04001332 RID: 4914
	private BuildingBlockData _blockData;

	// Token: 0x04001333 RID: 4915
	private const int MaxLevel = 20;

	// Token: 0x04001334 RID: 4916
	private const int MaxScaleAmount = 3;

	// Token: 0x04001335 RID: 4917
	private int _haveCount;

	// Token: 0x04001336 RID: 4918
	private readonly List<BuildingExpandResource.BuildingScaleInfo> _scaleInfos = new List<BuildingExpandResource.BuildingScaleInfo>();

	// Token: 0x04001337 RID: 4919
	private List<BuildingBlockData> _blocksInfo;

	// Token: 0x04001338 RID: 4920
	private List<short> _scaleTemplateIdList;

	// Token: 0x04001339 RID: 4921
	[SerializeField]
	private CImage buildingIcon;

	// Token: 0x0400133A RID: 4922
	[SerializeField]
	private RectTransform levelBallRoot;

	// Token: 0x0400133B RID: 4923
	[SerializeField]
	private RectTransform levelBallGroupRoot;

	// Token: 0x0400133C RID: 4924
	[SerializeField]
	private CommonItemBack coreItemView;

	// Token: 0x0400133D RID: 4925
	[SerializeField]
	private TextMeshProUGUI coreItemHaveCountLabel;

	// Token: 0x0400133E RID: 4926
	[SerializeField]
	private TextMeshProUGUI coreItemNeedCountLabel;

	// Token: 0x0400133F RID: 4927
	[SerializeField]
	private TextMeshProUGUI currentLevelLabel;

	// Token: 0x04001340 RID: 4928
	[SerializeField]
	private TextMeshProUGUI targetLevelLabel;

	// Token: 0x04001341 RID: 4929
	[SerializeField]
	private GameObject levelUpArrow;

	// Token: 0x04001342 RID: 4930
	[SerializeField]
	private CButtonObsolete addButton;

	// Token: 0x04001343 RID: 4931
	[SerializeField]
	private CButtonObsolete reduceButton;

	// Token: 0x04001344 RID: 4932
	[SerializeField]
	private CButtonObsolete upgradeButton;

	// Token: 0x04001345 RID: 4933
	[SerializeField]
	private CSliderLegacy levelSlider;

	// Token: 0x04001346 RID: 4934
	[SerializeField]
	private RectTransform sliderBackRoot;

	// Token: 0x04001347 RID: 4935
	[SerializeField]
	private RectTransform sliderDotRoot;

	// Token: 0x04001348 RID: 4936
	[SerializeField]
	private RectTransform scaleLayoutRoot;

	// Token: 0x04001349 RID: 4937
	[SerializeField]
	private GameObject sliderArea;

	// Token: 0x0400134A RID: 4938
	[SerializeField]
	private GameObject maxHintArea;

	// Token: 0x0400134B RID: 4939
	[SerializeField]
	private TextMeshProUGUI needItemName;

	// Token: 0x020012ED RID: 4845
	private struct BuildingScaleInfo
	{
		// Token: 0x04009C0C RID: 39948
		public short ScaleTemplateId;
	}
}
