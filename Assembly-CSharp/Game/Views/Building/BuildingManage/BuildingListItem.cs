using System;
using System.Collections.Generic;
using System.Text;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Building;
using GameData.Domains.Extra;
using GameData.Domains.Map;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Game.Views.Building.BuildingManage
{
	// Token: 0x02000BF0 RID: 3056
	public class BuildingListItem : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
	{
		// Token: 0x17001066 RID: 4198
		// (get) Token: 0x06009B3F RID: 39743 RVA: 0x0048ACBF File Offset: 0x00488EBF
		private BuildingModel BuildingModel
		{
			get
			{
				return SingletonObject.getInstance<BuildingModel>();
			}
		}

		// Token: 0x06009B40 RID: 39744 RVA: 0x0048ACC8 File Offset: 0x00488EC8
		private void Start()
		{
			bool flag = this.hoverImage != null;
			if (flag)
			{
				this.hoverImage.gameObject.SetActive(false);
			}
		}

		// Token: 0x06009B41 RID: 39745 RVA: 0x0048ACF8 File Offset: 0x00488EF8
		public void OnPointerEnter(PointerEventData eventData)
		{
			bool flag = this.hoverImage == null;
			if (!flag)
			{
				this.RefreshHoverSprite();
				this.hoverImage.gameObject.SetActive(true);
			}
		}

		// Token: 0x06009B42 RID: 39746 RVA: 0x0048AD34 File Offset: 0x00488F34
		public void OnPointerExit(PointerEventData eventData)
		{
			bool flag = this.hoverImage != null;
			if (flag)
			{
				this.hoverImage.gameObject.SetActive(false);
			}
		}

		// Token: 0x06009B43 RID: 39747 RVA: 0x0048AD64 File Offset: 0x00488F64
		private void RefreshHoverSprite()
		{
			this.hoverImage.sprite = (this.selected.activeSelf ? this.hoverSelectedSprite : this.hoverUnselectedSprite);
		}

		// Token: 0x06009B44 RID: 39748 RVA: 0x0048AD90 File Offset: 0x00488F90
		private void Update()
		{
			this.dropDownExpand.gameObject.SetActive(this.dropdown.IsExpanded);
			bool isExpanded = this.dropdown.IsExpanded;
			if (isExpanded)
			{
				CToggle[] toggles = this.dropdown.template.GetComponentsInChildren<CToggle>();
				for (int index = 0; index < toggles.Length; index++)
				{
					CToggle toggle = toggles[index];
					toggle.SetIsOnWithoutNotify(this._index == index);
				}
			}
		}

		// Token: 0x06009B45 RID: 39749 RVA: 0x0048AE08 File Offset: 0x00489008
		public void Refresh(BuildingListPanel.TogKey activeKey, MapBlockData mapBlockData, List<BuildingBlockData> blockDataList, int index, bool isSelected, Action<BuildingBlockData> selectBuilding, BuildingExceptionData exceptionData, Func<IAsyncMethodRequestHandler> getRequestHandle)
		{
			this._index = index;
			this._blockDataList = blockDataList;
			this._getRequestHandle = getRequestHandle;
			BuildingBlockData curBlockData = this._blockDataList[index];
			bool flag = curBlockData == null || curBlockData.TemplateId <= 0;
			if (!flag)
			{
				Location location = mapBlockData.GetLocation();
				this.button.ClearAndAddListener(new Action(this.OnClick));
				BuildingBlockKey buildingBlockKey = new BuildingBlockKey(location.AreaId, location.BlockId, curBlockData.BlockIndex);
				BuildingBlockItem configData = BuildingBlock.Instance[curBlockData.TemplateId];
				bool flag2 = configData.TemplateId <= 0;
				if (!flag2)
				{
					string buildingName = ViewBuildingManage.GetBuildingName(buildingBlockKey, curBlockData.TemplateId, mapBlockData.TemplateId, true, true);
					bool showDropdown = blockDataList.Count > 1;
					this.dropdown.gameObject.SetActive(showDropdown);
					bool flag3 = showDropdown;
					if (flag3)
					{
						this.dropdown.onValueChanged.RemoveAllListeners();
						this.dropdown.ClearOptions();
						foreach (BuildingBlockData blockData in blockDataList)
						{
							BuildingBlockKey blockKey = new BuildingBlockKey(location.AreaId, location.BlockId, blockData.BlockIndex);
							string name = ViewBuildingManage.GetBuildingName(blockKey, blockData.TemplateId, mapBlockData.TemplateId, true, false);
							this.dropdown.options.Add(new CDropdown.OptionData(name));
						}
						this.dropdown.value = index;
						this.dropdown.onValueChanged.AddListener(new UnityAction<int>(this.OnDropdownValueChanged));
					}
					this._selectBuilding = selectBuilding;
					this.selected.SetActive(isSelected);
					bool flag4 = this.hoverImage != null && this.hoverImage.gameObject.activeSelf;
					if (flag4)
					{
						this.RefreshHoverSprite();
					}
					ViewBuildingManage.SetBuildingIcon(this.imageBuildingIcon, configData, false, null);
					this.textBuildingName.text = buildingName;
					this.RefreshManagePeopleNumTag(activeKey, buildingBlockKey);
					this.RefreshManageProcessTag(activeKey, buildingBlockKey, curBlockData);
					this.RefreshBuildingDestroyTag(configData, curBlockData);
					this.RefreshBuildingErrorTag(activeKey, buildingBlockKey, exceptionData);
					this.RefreshBuildingResourceTag(activeKey, buildingBlockKey, exceptionData);
				}
			}
		}

		// Token: 0x06009B46 RID: 39750 RVA: 0x0048B060 File Offset: 0x00489260
		private void UpdateBuildingListShopInfo(BuildingBlockData blockData, BuildingBlockKey blockKey)
		{
			bool flag = blockData.TemplateId == 105;
			if (flag)
			{
				BuildingDomainMethod.AsyncCall.GetFixBookProgress(null, blockKey, delegate(int offset, RawDataPool dataPool)
				{
					int progress = 0;
					Serializer.Deserialize(dataPool, offset, ref progress);
					progress = Math.Min(progress, 100);
					this._stringBuilder1.Clear();
					this._stringBuilder1.Append(progress).Append("%");
					this.textShopProgress.SetText(this._stringBuilder1.ToString(), true);
					this.imageShopProgress.fillAmount = (float)progress / 100f;
					this._stringBuilder2.Clear();
					this._stringBuilder2.AppendFormat(LocalStringManager.Get(LanguageKey.LK_Building_ManageProgress), this._stringBuilder1);
					this.tipShopProgress.PresetParam[0] = this._stringBuilder2.ToString();
					this.rootShopProgress.SetActive(progress > 0);
				});
			}
			else
			{
				this._stringBuilder1.Clear();
				this._stringBuilder1.Append(blockData.ShopProgressPercentage).Append("%");
				this.textShopProgress.SetText(this._stringBuilder1.ToString(), true);
				this.imageShopProgress.fillAmount = blockData.ShopProgressFill;
				this._stringBuilder2.Clear();
				this._stringBuilder2.AppendFormat(LocalStringManager.Get(LanguageKey.LK_Building_ManageProgress), this._stringBuilder1);
				this.tipShopProgress.PresetParam[0] = this._stringBuilder2.ToString();
			}
		}

		// Token: 0x06009B47 RID: 39751 RVA: 0x0048B12C File Offset: 0x0048932C
		private void UpdateShopPeopleInfo(BuildingBlockKey blockKey, BuildingBlockData blockData, BuildingBlockItem configData)
		{
			int count = this.BuildingModel.GetBuildingShopManagerCount(blockKey);
			this.rootShopManage.SetActive(blockData.OperationType != 0 && blockData.OperationType != 1 && count <= 0);
			this.rootDamage.SetActive(!this.rootShopManage.activeSelf && blockData.OperationType != 0 && blockData.Durability < configData.MaxDurability);
			this._stringBuilder1.Clear();
			this._stringBuilder1.Append(count).Append("/").Append(7);
			this.textShopCount.SetText(this._stringBuilder1);
			this.tipShopCount.PresetParam[0] = LocalStringManager.GetFormat(LanguageKey.LK_Building_ShopManagerPeople, this._stringBuilder1.ToString());
		}

		// Token: 0x06009B48 RID: 39752 RVA: 0x0048B1FD File Offset: 0x004893FD
		public void UpdateShopGetItemInfo(BuildingBlockKey blockKey)
		{
			BuildingDomainMethod.AsyncCall.GetBuildingEarningData(null, blockKey, delegate(int offset, RawDataPool dataPool)
			{
				BuildingEarningsData earningData = new BuildingEarningsData();
				Serializer.Deserialize(dataPool, offset, ref earningData);
				int count = 0;
				bool flag = earningData != null && earningData.CollectionItemList != null;
				if (flag)
				{
					count += earningData.CollectionItemList.Count;
				}
				bool flag2 = earningData != null && earningData.RecruitLevelList != null;
				if (flag2)
				{
					count += earningData.RecruitLevelList.Count;
				}
				bool flag3 = earningData != null && earningData.CollectionResourceList != null;
				if (flag3)
				{
					count += earningData.CollectionResourceList.Count;
				}
				bool flag4 = earningData != null && earningData.ShopSoldItemEarnList != null;
				if (flag4)
				{
					for (int i = 0; i < earningData.ShopSoldItemEarnList.Count; i++)
					{
						bool flag5 = earningData.ShopSoldItemEarnList[i].First != -1;
						if (flag5)
						{
							count++;
						}
					}
				}
				this.rootGetItem.SetActive(count > 0);
				this.textGetItem.SetText(count.ToString(), true);
				this._stringBuilder1.Clear();
				this._stringBuilder1.AppendFormat(LocalStringManager.Get(LanguageKey.LK_Building_WaitReceiveNum), count);
				this.tipGetItem.PresetParam[0] = this._stringBuilder1.ToString();
			});
		}

		// Token: 0x06009B49 RID: 39753 RVA: 0x0048B214 File Offset: 0x00489414
		public void UpdateMakeItemInfo(BuildingBlockKey blockKey)
		{
			BuildingDomainMethod.AsyncCall.GetMakingItemData(null, blockKey, delegate(int offset, RawDataPool dataPool)
			{
				MakeItemData makeItemData = new MakeItemData();
				Serializer.Deserialize(dataPool, offset, ref makeItemData);
				this.rootMakeItem.SetActive(makeItemData != null && makeItemData.LeftTime != 0);
				bool flag = makeItemData != null;
				if (flag)
				{
					this._stringBuilder1.Clear();
					this._stringBuilder1.AppendFormat(LocalStringManager.Get(LanguageKey.LK_Building_MakeingItemCostTime), makeItemData.LeftTime);
					this.textMakeItem.SetText(makeItemData.LeftTime.ToString(), true);
					this.tipMakeItem.PresetParam[0] = this._stringBuilder1.ToString();
				}
			});
		}

		// Token: 0x06009B4A RID: 39754 RVA: 0x0048B22B File Offset: 0x0048942B
		private void OnDropdownValueChanged(int value)
		{
			this._selectBuilding(this._blockDataList[value]);
		}

		// Token: 0x06009B4B RID: 39755 RVA: 0x0048B246 File Offset: 0x00489446
		private void OnClick()
		{
			this._selectBuilding(this._blockDataList[0]);
		}

		// Token: 0x06009B4C RID: 39756 RVA: 0x0048B264 File Offset: 0x00489464
		private void RefreshBuildingDestroyTag(BuildingBlockItem configData, BuildingBlockData blockData)
		{
			int percent = (int)((blockData.OperationType != 0) ? ((configData.MaxDurability - blockData.Durability) * 100 / configData.MaxDurability) : 0);
			this.txtDestroyProcess.text = string.Format("{0}%", percent);
			this.goBuildingDestroyItem.gameObject.SetActive(percent > 0);
			this._stringBuilder1.Clear();
			this._stringBuilder1.Append(this.txtDestroyProcess.text);
			TooltipInvoker tips = this.goBuildingDestroyItem.GetComponent<TooltipInvoker>();
			tips.Type = TipType.SingleDesc;
			string[] presetParam = tips.PresetParam;
			int num = 0;
			string str = LocalStringManager.Get(LanguageKey.LK_Building_Damage);
			StringBuilder stringBuilder = this._stringBuilder1;
			presetParam[num] = str + ((stringBuilder != null) ? stringBuilder.ToString() : null);
		}

		// Token: 0x06009B4D RID: 39757 RVA: 0x0048B328 File Offset: 0x00489528
		private void RefreshManageProcessTag(BuildingListPanel.TogKey activeKey, BuildingBlockKey blockKey, BuildingBlockData blockData)
		{
			this.goManageProcessItem.SetActive(false);
			bool flag = activeKey > BuildingListPanel.TogKey.Shop;
			if (!flag)
			{
				TooltipInvoker tips = this.goManageProcessItem.GetComponent<TooltipInvoker>();
				bool flag2 = blockData.TemplateId == 105;
				if (flag2)
				{
					Func<IAsyncMethodRequestHandler> getRequestHandle = this._getRequestHandle;
					IAsyncMethodRequestHandler rq = (getRequestHandle != null) ? getRequestHandle() : null;
					BuildingDomainMethod.AsyncCall.GetFixBookProgress(rq, blockKey, delegate(int offset, RawDataPool dataPool)
					{
						int progress = 0;
						Serializer.Deserialize(dataPool, offset, ref progress);
						progress = Math.Min(progress, 100);
						this._stringBuilder1.Clear();
						this._stringBuilder1.Append(progress).Append("%");
						this.imgManageProcess.fillAmount = (float)progress / 100f;
						this._stringBuilder2.Clear();
						this._stringBuilder2.AppendFormat(LocalStringManager.Get(LanguageKey.LK_Building_ManageProgress), this._stringBuilder1);
						tips.PresetParam[0] = this._stringBuilder2.ToString();
						this.goManageProcessItem.SetActive(true);
					});
				}
				else
				{
					this._stringBuilder1.Clear();
					this._stringBuilder1.Append(blockData.ShopProgressPercentage).Append("%");
					this.imgManageProcess.fillAmount = blockData.ShopProgressFill;
					this._stringBuilder2.Clear();
					this._stringBuilder2.AppendFormat(LocalStringManager.Get(LanguageKey.LK_Building_ManageProgress), this._stringBuilder1);
					tips.PresetParam[0] = this._stringBuilder2.ToString();
					this.goManageProcessItem.SetActive(true);
				}
			}
		}

		// Token: 0x06009B4E RID: 39758 RVA: 0x0048B434 File Offset: 0x00489634
		private void RefreshManagePeopleNumTag(BuildingListPanel.TogKey activeKey, BuildingBlockKey blockKey)
		{
			switch (activeKey)
			{
			case BuildingListPanel.TogKey.Invalid:
			case BuildingListPanel.TogKey.Resource:
				this.goManagePeopleItem.SetActive(false);
				break;
			case BuildingListPanel.TogKey.Shop:
			case BuildingListPanel.TogKey.Make:
			{
				int count = this.BuildingModel.GetBuildingShopManagerCount(blockKey);
				this.txtManagePeopleNum.text = string.Format("{0}", count);
				this.goManagePeopleItem.SetActive(true);
				TooltipInvoker tips = this.goManagePeopleItem.GetComponent<TooltipInvoker>();
				this._stringBuilder1.Clear();
				this._stringBuilder1.Append(count).Append("/").Append(7);
				tips.PresetParam[0] = LocalStringManager.GetFormat(LanguageKey.LK_Building_ShopManagerPeople, this._stringBuilder1.ToString());
				break;
			}
			}
		}

		// Token: 0x06009B4F RID: 39759 RVA: 0x0048B4FC File Offset: 0x004896FC
		private void RefreshBuildingErrorTag(BuildingListPanel.TogKey activeKey, BuildingBlockKey blockKey, BuildingExceptionData exceptionData)
		{
			this.goBuildingErrorItem.SetActive(false);
			switch (activeKey)
			{
			case BuildingListPanel.TogKey.Shop:
			case BuildingListPanel.TogKey.Make:
			{
				BuildingExceptionItem item;
				exceptionData.BuildingExceptionDict.TryGetValue(blockKey, out item);
				int count = (item == null || item.ExceptionTypeList == null) ? 0 : item.ExceptionTypeList.Count;
				this.txtBuildingErrorNum.text = string.Format("{0}", count);
				this.goBuildingErrorItem.SetActive(count > 0);
				bool flag = item != null && item.ExceptionTypeList != null;
				if (flag)
				{
					TooltipInvoker tip = this.goBuildingErrorItem.GetComponent<TooltipInvoker>();
					this._stringBuilder1.Clear();
					foreach (sbyte exceptionType in item.ExceptionTypeList)
					{
						string strKey = ViewBuildingArea.GetBuildingExceptionString((BuildingExceptionType)exceptionType);
						this._stringBuilder1.AppendLine(strKey.SetColor("darkred"));
					}
					string tipContent = this._stringBuilder1.ToString();
					TooltipInvoker tooltipInvoker = tip;
					if (tooltipInvoker.RuntimeParam == null)
					{
						tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
					}
					tip.RuntimeParam.Set("arg0", tipContent);
				}
				break;
			}
			}
		}

		// Token: 0x06009B50 RID: 39760 RVA: 0x0048B668 File Offset: 0x00489868
		private void RefreshBuildingResourceTag(BuildingListPanel.TogKey activeKey, BuildingBlockKey blockKey, BuildingExceptionData exceptionData)
		{
			this.goResourceCdItem.SetActive(false);
			if (activeKey - BuildingListPanel.TogKey.Invalid > 2)
			{
				if (activeKey == BuildingListPanel.TogKey.Resource)
				{
					Func<IAsyncMethodRequestHandler> getRequestHandle = this._getRequestHandle;
					IAsyncMethodRequestHandler rq = (getRequestHandle != null) ? getRequestHandle() : null;
					ExtraDomainMethod.AsyncCall.GetResourceBlockProducingCoreCooldown(rq, blockKey, delegate(int offset, RawDataPool pool)
					{
						int cooldown = 0;
						Serializer.Deserialize(pool, offset, ref cooldown);
						this.txtResourceCd.text = string.Format("{0}", Mathf.Max(0, cooldown));
						this.goResourceCdItem.SetActive(true);
						TooltipInvoker tips = this.goResourceCdItem.GetComponent<TooltipInvoker>();
						int lineCount = 0;
						tips.Type = TipType.GeneralLines;
						TooltipInvoker tooltipInvoker = tips;
						if (tooltipInvoker.RuntimeParam == null)
						{
							tooltipInvoker.RuntimeParam = new ArgumentBox();
						}
						tips.RuntimeParam.Set("Title", LanguageKey.LK_MouseTip_CoreProducingCooldown_Title.Tr());
						tips.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData
						{
							Type = 11,
							Args = new List<string>
							{
								LanguageKey.LK_MouseTip_CoreProducingCooldown_Desc1.Tr()
							}
						});
						tips.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData
						{
							Type = 5,
							Args = new List<string>
							{
								LanguageKey.LK_MouseTip_CoreProducingCooldown_Desc2.TrFormat(Mathf.Max(0, cooldown))
							},
							ExtraArgs = new List<object>
							{
								5
							}
						});
						tips.RuntimeParam.Set("LineCount", lineCount);
					});
				}
			}
		}

		// Token: 0x04007811 RID: 30737
		[Header("基础信息")]
		[SerializeField]
		private CButton button;

		// Token: 0x04007812 RID: 30738
		[SerializeField]
		private GameObject selected;

		// Token: 0x04007813 RID: 30739
		[SerializeField]
		private CDropdown dropdown;

		// Token: 0x04007814 RID: 30740
		[SerializeField]
		private CImage dropDownExpand;

		// Token: 0x04007815 RID: 30741
		[SerializeField]
		private CImage imageBuildingIcon;

		// Token: 0x04007816 RID: 30742
		[SerializeField]
		private TextMeshProUGUI textBuildingName;

		// Token: 0x04007817 RID: 30743
		[SerializeField]
		private TextMeshProUGUI textBuildingLevel;

		// Token: 0x04007818 RID: 30744
		[SerializeField]
		private GameObject rootBuildingLevel;

		// Token: 0x04007819 RID: 30745
		[Header("经营信息")]
		[SerializeField]
		private GameObject rootShopProgress;

		// Token: 0x0400781A RID: 30746
		[SerializeField]
		private GameObject rootShopManage;

		// Token: 0x0400781B RID: 30747
		[SerializeField]
		private TextMeshProUGUI textShopProgress;

		// Token: 0x0400781C RID: 30748
		[SerializeField]
		private CImage imageShopProgress;

		// Token: 0x0400781D RID: 30749
		[SerializeField]
		private TooltipInvoker tipShopProgress;

		// Token: 0x0400781E RID: 30750
		[Header("经营人数")]
		[SerializeField]
		private GameObject rootShopCount;

		// Token: 0x0400781F RID: 30751
		[SerializeField]
		private TextMeshProUGUI textShopCount;

		// Token: 0x04007820 RID: 30752
		[SerializeField]
		private TooltipInvoker tipShopCount;

		// Token: 0x04007821 RID: 30753
		[Header("建筑操作")]
		[SerializeField]
		private GameObject rootBuild;

		// Token: 0x04007822 RID: 30754
		[SerializeField]
		private GameObject rootRemove;

		// Token: 0x04007823 RID: 30755
		[SerializeField]
		private GameObject rootDamage;

		// Token: 0x04007824 RID: 30756
		[Header("获得物品")]
		[SerializeField]
		private GameObject rootGetItem;

		// Token: 0x04007825 RID: 30757
		[SerializeField]
		private TooltipInvoker tipGetItem;

		// Token: 0x04007826 RID: 30758
		[SerializeField]
		private TextMeshProUGUI textGetItem;

		// Token: 0x04007827 RID: 30759
		[Header("制造信息")]
		[SerializeField]
		private GameObject rootMakeItem;

		// Token: 0x04007828 RID: 30760
		[SerializeField]
		private TooltipInvoker tipMakeItem;

		// Token: 0x04007829 RID: 30761
		[SerializeField]
		private TextMeshProUGUI textMakeItem;

		// Token: 0x0400782A RID: 30762
		[Header("建筑受损Tag")]
		[SerializeField]
		private GameObject goBuildingDestroyItem;

		// Token: 0x0400782B RID: 30763
		[SerializeField]
		private TextMeshProUGUI txtDestroyProcess;

		// Token: 0x0400782C RID: 30764
		[Header("经营人数Tag")]
		[SerializeField]
		private GameObject goManagePeopleItem;

		// Token: 0x0400782D RID: 30765
		[SerializeField]
		private TextMeshProUGUI txtManagePeopleNum;

		// Token: 0x0400782E RID: 30766
		[SerializeField]
		private CImage imgManageProcessInManagePeopleItem;

		// Token: 0x0400782F RID: 30767
		[Header("建筑异常Tag")]
		[SerializeField]
		private GameObject goBuildingErrorItem;

		// Token: 0x04007830 RID: 30768
		[SerializeField]
		private TextMeshProUGUI txtBuildingErrorNum;

		// Token: 0x04007831 RID: 30769
		[Header("经营进度Tag")]
		[SerializeField]
		private GameObject goManageProcessItem;

		// Token: 0x04007832 RID: 30770
		[SerializeField]
		private CImage imgManageProcess;

		// Token: 0x04007833 RID: 30771
		[Header("资源冷却Tag")]
		[SerializeField]
		private GameObject goResourceCdItem;

		// Token: 0x04007834 RID: 30772
		[SerializeField]
		private TextMeshProUGUI txtResourceCd;

		// Token: 0x04007835 RID: 30773
		[Header("Hover")]
		[SerializeField]
		private CImage hoverImage;

		// Token: 0x04007836 RID: 30774
		[SerializeField]
		private Sprite hoverSelectedSprite;

		// Token: 0x04007837 RID: 30775
		[SerializeField]
		private Sprite hoverUnselectedSprite;

		// Token: 0x04007838 RID: 30776
		private readonly StringBuilder _stringBuilder1 = new StringBuilder();

		// Token: 0x04007839 RID: 30777
		private readonly StringBuilder _stringBuilder2 = new StringBuilder();

		// Token: 0x0400783A RID: 30778
		private List<BuildingBlockData> _blockDataList;

		// Token: 0x0400783B RID: 30779
		private Action<BuildingBlockData> _selectBuilding;

		// Token: 0x0400783C RID: 30780
		private int _index;

		// Token: 0x0400783D RID: 30781
		private Func<IAsyncMethodRequestHandler> _getRequestHandle;
	}
}
