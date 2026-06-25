using System;
using System.Collections;
using System.Collections.Generic;
using Config;
using FrameWork;
using Game.Components.SortAndFilter;
using Game.Components.SortAndFilter.Item;
using GameData.Domains.Character;
using GameData.Domains.Extra;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Story;
using GameData.Domains.Taiwu;
using GameData.Domains.World.Display;
using GameData.GameDataBridge;
using GameData.Serializer;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.Views.SectInteract.Zhujian
{
	// Token: 0x020009CD RID: 2509
	public class ZhujianGearMateSubPageAttribute : ZhujianGearMateSubPage
	{
		// Token: 0x060079CB RID: 31179 RVA: 0x003890D8 File Offset: 0x003872D8
		private static int GetMaterialSubFilterIndex(int attributeType)
		{
			if (!true)
			{
			}
			int result;
			switch (attributeType)
			{
			case 0:
				result = 3;
				break;
			case 1:
				result = 0;
				break;
			case 2:
				result = 4;
				break;
			case 3:
				result = 6;
				break;
			case 4:
				result = 1;
				break;
			case 5:
				result = 2;
				break;
			default:
				result = -1;
				break;
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x060079CC RID: 31180 RVA: 0x00389130 File Offset: 0x00387330
		public override void Init(ViewZhujianGearMate parent)
		{
			base.Init(parent);
			this.masterLightEffect.SetActive(false);
			this.btnConfirm.onClick.AddListener(new UnityAction(this.OnConfirmClicked));
			this.itemSelector.SetSortControllerFactory((ISortAndFilterView sortAndFilter) => new MaterialAsRootSortAndFilterController(sortAndFilter));
			this.itemSelector.Init();
			this.itemSelector.SetMaterialFilter(-1);
			this.itemSelector.SetTitle("LK_GearMate_Attribute_Selector_Title");
			this.itemSelector.OnSelectionChanged -= this.UpdateAttributePreview;
			this.itemSelector.OnSelectionChanged += this.UpdateAttributePreview;
			this.itemSelector.OnSourceTypeChanged -= this.OnSourceTypeChanged;
			this.itemSelector.OnSourceTypeChanged += this.OnSourceTypeChanged;
			this.itemSelector.OnItemSelected -= this.OnItemSelected;
			this.itemSelector.OnItemSelected += this.OnItemSelected;
			this.itemSelector.OnFilterManuallyChanged -= this.OnFilterManuallyChanged;
			this.itemSelector.OnFilterManuallyChanged += this.OnFilterManuallyChanged;
			this.itemSelector.SetBaseFilter((ItemDisplayData item) => item.Key.ItemType == 5 && Config.Material.Instance[item.Key.TemplateId].ResourceType <= 5);
			this.itemSelector.GetMaxSelectableCount = new Func<ItemDisplayData, int>(this.GetItemMaxSelectableCount);
			this.itemSelector.GetDisabledItemTip = delegate(ItemDisplayData item)
			{
				sbyte resourceType = Config.Material.Instance[item.Key.TemplateId].ResourceType;
				sbyte upgradeType = GearMateUpgradeType.GetMainAttributeUpgradeTypeByResourceType(resourceType);
				bool flag = upgradeType >= 0;
				string result;
				if (flag)
				{
					string attrName = LocalStringManager.Get(GearMateUpgradeType.GetMainAttributeUpgradeTypeName(upgradeType));
					string name = null;
					bool flag2 = this.ParentView != null && this.ParentView.DisplayData != null;
					if (flag2)
					{
						name = NameCenter.GetMonasticTitleOrDisplayName(this.ParentView.DisplayData, false);
					}
					bool flag3 = string.IsNullOrEmpty(name);
					if (flag3)
					{
						name = LocalStringManager.Get(LanguageKey.LK_GearMate_Tab_0);
					}
					result = LocalStringManager.GetFormat(LanguageKey.LK_GearMate_UpgradeMaxTip, name, attrName).SetColor("brightred");
				}
				else
				{
					result = "";
				}
				return result;
			};
			foreach (ZhujianGearMateAttribute attrUI in this.attributeUIs)
			{
				attrUI.InitFilterButton();
				attrUI.OnFilterButtonClicked += this.OnAttributeFilterClicked;
			}
		}

		// Token: 0x060079CD RID: 31181 RVA: 0x0038933C File Offset: 0x0038753C
		private void OnAttributeFilterClicked(int attributeType)
		{
			bool flag = attributeType == this._currentFilterAttributeType;
			if (flag)
			{
				this._currentFilterAttributeType = -1;
				this.itemSelector.SetMaterialFilter(-1);
			}
			else
			{
				this._currentFilterAttributeType = attributeType;
				int filterIndex = ZhujianGearMateSubPageAttribute.GetMaterialSubFilterIndex(attributeType);
				bool flag2 = filterIndex >= 0;
				if (flag2)
				{
					this.itemSelector.SetMaterialFilter(filterIndex);
				}
			}
		}

		// Token: 0x060079CE RID: 31182 RVA: 0x00389394 File Offset: 0x00387594
		private void OnFilterManuallyChanged()
		{
			this._currentFilterAttributeType = -1;
		}

		// Token: 0x060079CF RID: 31183 RVA: 0x003893A0 File Offset: 0x003875A0
		private void OnSourceTypeChanged(int sourceIndex)
		{
			bool flag = this._currentSourceIndex == sourceIndex;
			if (!flag)
			{
				this._currentSourceIndex = sourceIndex;
				this.UpdateAttributeDisplay();
			}
		}

		// Token: 0x060079D0 RID: 31184 RVA: 0x003893CC File Offset: 0x003875CC
		private static ItemSourceType ResolveItemSourceType(int sourceIndex)
		{
			if (!true)
			{
			}
			ItemSourceType result;
			switch (sourceIndex)
			{
			case 0:
				result = ItemSourceType.Inventory;
				break;
			case 1:
				result = ItemSourceType.Warehouse;
				break;
			case 2:
				result = ItemSourceType.Treasury;
				break;
			default:
				result = ItemSourceType.Inventory;
				break;
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x060079D1 RID: 31185 RVA: 0x0038940C File Offset: 0x0038760C
		private static int GetAttributeTypeByDisplayIndex(int displayIndex)
		{
			return (displayIndex >= 0 && displayIndex < ZhujianGearMateSubPageAttribute.AttributeDisplayOrder.Length) ? ZhujianGearMateSubPageAttribute.AttributeDisplayOrder[displayIndex] : displayIndex;
		}

		// Token: 0x060079D2 RID: 31186 RVA: 0x00389438 File Offset: 0x00387638
		private static int GetDisplayIndexByAttributeType(int attributeType)
		{
			for (int i = 0; i < ZhujianGearMateSubPageAttribute.AttributeDisplayOrder.Length; i++)
			{
				bool flag = ZhujianGearMateSubPageAttribute.AttributeDisplayOrder[i] == attributeType;
				if (flag)
				{
					return i;
				}
			}
			return attributeType;
		}

		// Token: 0x060079D3 RID: 31187 RVA: 0x00389475 File Offset: 0x00387675
		private void OnItemSelected(ItemKey itemKey)
		{
			this.PlayItemDropAnimation(itemKey);
		}

		// Token: 0x060079D4 RID: 31188 RVA: 0x00389480 File Offset: 0x00387680
		private int GetItemMaxSelectableCount(ItemDisplayData item)
		{
			bool flag = this._gearMateData == null;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				sbyte resourceType = Config.Material.Instance[item.Key.TemplateId].ResourceType;
				sbyte upgradeType = GearMateUpgradeType.GetMainAttributeUpgradeTypeByResourceType(resourceType);
				bool flag2 = upgradeType < 0 || upgradeType >= 6;
				if (flag2)
				{
					result = 0;
				}
				else
				{
					int currentAttr = this._currentPreviewAttributeValue[(int)upgradeType];
					int maxAttr = (int)GlobalConfig.Instance.MaxValueOfMaxMainAttributes;
					bool flag3 = currentAttr >= maxAttr;
					if (flag3)
					{
						result = 0;
					}
					else
					{
						int currentProg = this._previewProcessValue[(int)upgradeType];
						int grade = (int)Config.Material.Instance[item.Key.TemplateId].Grade;
						int valuePerItem = ZhujianGearMateSubPageAttribute.CalcGradeProcessValue((sbyte)grade);
						int canAddCount = 0;
						int tempAttr = currentAttr;
						int tempProg = currentProg;
						for (int i = 0; i < item.Amount; i++)
						{
							bool flag4 = tempAttr >= maxAttr;
							if (flag4)
							{
								break;
							}
							tempProg += valuePerItem;
							int num;
							int increase = ZhujianGearMateSubPageAttribute.CalcIncreaseCount(tempProg, this._currentAttributeValue[(int)upgradeType], out num);
							int projectedAttr = this._currentAttributeValue[(int)upgradeType] + increase;
							bool flag5 = projectedAttr > maxAttr;
							if (flag5)
							{
								canAddCount++;
								break;
							}
							canAddCount++;
							tempAttr = projectedAttr;
						}
						result = canAddCount;
					}
				}
			}
			return result;
		}

		// Token: 0x060079D5 RID: 31189 RVA: 0x003895C4 File Offset: 0x003877C4
		private void OnConfirmClicked()
		{
			bool flag = this.itemSelector.SelectedItems.Count == 0 || this.GearMateId < 0 || this._isUpgrading;
			if (!flag)
			{
				this._isUpgrading = true;
				this.btnConfirm.interactable = false;
				this.itemSelector.SetSourceToggleGroupInteractable(false);
				this.itemSelector.SetBlocked(true);
				this.PlayUpgradeAnim(delegate
				{
					foreach (ValueTuple<ItemKey, int, ItemSourceType> valueTuple in this.itemSelector.GetAllSelectedItemsWithSource())
					{
						ItemKey itemKey = valueTuple.Item1;
						int count = valueTuple.Item2;
						ItemSourceType sourceType = valueTuple.Item3;
						sbyte resourceType = Config.Material.Instance[itemKey.TemplateId].ResourceType;
						sbyte upgradeType = GearMateUpgradeType.GetMainAttributeUpgradeTypeByResourceType(resourceType);
						bool flag2 = upgradeType >= 0;
						if (flag2)
						{
							ExtraDomainMethod.Call.UpgradeGearMate(this.GearMateId, upgradeType, itemKey, count, sourceType);
						}
					}
					this.itemSelector.ClearAllSelections(false);
					this.ClearDropQueue();
					this.RequestGearMateData();
					this._isUpgrading = false;
					this.btnConfirm.interactable = (this.itemSelector.SelectedItems.Count > 0);
					this.itemSelector.SetSourceToggleGroupInteractable(true);
					this.itemSelector.SetBlocked(false);
					this.ParentView.SetChangeButtonInteractable(true);
				});
			}
		}

		// Token: 0x060079D6 RID: 31190 RVA: 0x0038963D File Offset: 0x0038783D
		protected override void OnShowDataRequest()
		{
			this.itemSelector.ClearAllSelections(false);
			this.RequestGearMateData();
		}

		// Token: 0x060079D7 RID: 31191 RVA: 0x00389654 File Offset: 0x00387854
		public override void OnHide()
		{
			base.OnHide();
			this.ClearDropQueue();
			this.itemSelector.SetBlocked(false);
			this._isUpgrading = false;
		}

		// Token: 0x060079D8 RID: 31192 RVA: 0x00389679 File Offset: 0x00387879
		public override void OnListenerIdReady()
		{
			base.OnListenerIdReady();
			this.RequestGearMateData();
		}

		// Token: 0x060079D9 RID: 31193 RVA: 0x0038968C File Offset: 0x0038788C
		public override void SetGearMateId(int gearMateId)
		{
			bool flag = this.GearMateId == gearMateId;
			if (!flag)
			{
				base.SetGearMateId(gearMateId);
				this.itemSelector.ClearAllSelections(false);
				bool isVisible = this.IsVisible;
				if (isVisible)
				{
					this.RequestGearMateData();
				}
			}
		}

		// Token: 0x060079DA RID: 31194 RVA: 0x003896D4 File Offset: 0x003878D4
		public override bool CanQuickHide()
		{
			return !this._isUpgrading;
		}

		// Token: 0x060079DB RID: 31195 RVA: 0x003896F0 File Offset: 0x003878F0
		private void RequestGearMateData()
		{
			bool flag = this.GearMateId < 0 || this.ListenerId == 0;
			if (!flag)
			{
				StoryDomainMethod.Call.GetSectZhujianGearMateAttributeDisplayData(this.ListenerId, this.GearMateId, SingletonObject.getInstance<BasicGameData>().TaiwuCharId);
			}
		}

		// Token: 0x060079DC RID: 31196 RVA: 0x00389738 File Offset: 0x00387938
		public override void OnNotifyGameData(List<NotificationWrapper> notifications)
		{
			foreach (NotificationWrapper wrapper in notifications)
			{
				Notification notification = wrapper.Notification;
				bool flag = notification.Type == 1;
				if (flag)
				{
					bool flag2 = notification.DomainId == 20 && notification.MethodId == 17;
					if (flag2)
					{
						SectZhujianGearMateAttributeDisplayData displayData = new SectZhujianGearMateAttributeDisplayData();
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref displayData);
						this._gearMateData = displayData.GearMate;
						this.itemSelector.SetItems(displayData.Items);
						bool canTransfer = displayData.CanUseWarehouse;
						this.itemSelector.SetSourceToggleInteractable(1, canTransfer);
						this.itemSelector.SetSourceToggleInteractable(2, canTransfer);
						bool flag3 = !canTransfer && this._currentSourceIndex > 0;
						if (flag3)
						{
							this.itemSelector.SetSourceToggle(0);
						}
						bool flag4 = displayData.MainAttributes != null;
						if (flag4)
						{
							int i = 0;
							while (i < displayData.MainAttributes.Count && i < this._currentAttributeValue.Length)
							{
								this._currentAttributeValue[i] = displayData.MainAttributes[i];
								i++;
							}
						}
						this.UpdateAttributeDisplay();
						base.SetContentReady();
					}
				}
			}
		}

		// Token: 0x060079DD RID: 31197 RVA: 0x003898C4 File Offset: 0x00387AC4
		private void UpdateAttributeDisplay()
		{
			bool flag = this._gearMateData == null;
			if (!flag)
			{
				for (int i = 0; i < 6; i++)
				{
					this._previewProcessValue[i] = this._gearMateData.MainAttributeProgress[i];
					this._currentPreviewAttributeValue[i] = this._currentAttributeValue[i];
				}
				foreach (KeyValuePair<ItemKey, int> kvp in this.itemSelector.SelectedItems)
				{
					ItemKey itemKey = kvp.Key;
					int count = kvp.Value;
					sbyte resourceType = Config.Material.Instance[itemKey.TemplateId].ResourceType;
					sbyte upgradeType = GearMateUpgradeType.GetMainAttributeUpgradeTypeByResourceType(resourceType);
					bool flag2 = upgradeType >= 0 && upgradeType < 6;
					if (flag2)
					{
						sbyte grade = Config.Material.Instance[itemKey.TemplateId].Grade;
						int value = ZhujianGearMateSubPageAttribute.CalcGradeProcessValue(grade) * count;
						this._previewProcessValue[(int)upgradeType] += value;
					}
				}
				for (int j = 0; j < 6; j++)
				{
					bool flag3 = j >= this._gearMateData.MainAttributeProgress.Length;
					if (!flag3)
					{
						int currentProgress = this._gearMateData.MainAttributeProgress[j];
						int currentAttrValue = this._currentAttributeValue[j];
						int previewProgress = this._previewProcessValue[j];
						int resultProgress;
						int increaseCount = ZhujianGearMateSubPageAttribute.CalcIncreaseCount(previewProgress, currentAttrValue, out resultProgress);
						int previewAttrValue = currentAttrValue + increaseCount;
						this._currentPreviewAttributeValue[j] = previewAttrValue;
					}
				}
				int displayIndex = 0;
				while (displayIndex < this.attributeUIs.Count && displayIndex < ZhujianGearMateSubPageAttribute.AttributeDisplayOrder.Length)
				{
					int attributeType = ZhujianGearMateSubPageAttribute.GetAttributeTypeByDisplayIndex(displayIndex);
					bool flag4 = attributeType >= this._gearMateData.MainAttributeProgress.Length;
					if (!flag4)
					{
						ZhujianGearMateAttribute ui = this.attributeUIs[displayIndex];
						int currentProgress2 = this._gearMateData.MainAttributeProgress[attributeType];
						int currentAttrValue2 = this._currentAttributeValue[attributeType];
						int requirement = ZhujianGearMateSubPageAttribute.GetUpgradeRequirement(currentAttrValue2);
						int previewProgress2 = this._previewProcessValue[attributeType];
						int resultProgress2;
						int increaseCount2 = ZhujianGearMateSubPageAttribute.CalcIncreaseCount(previewProgress2, currentAttrValue2, out resultProgress2);
						int previewAttrValue2 = currentAttrValue2 + increaseCount2;
						int previewRequirement = ZhujianGearMateSubPageAttribute.GetUpgradeRequirement(previewAttrValue2);
						int percent = (requirement > 0) ? (currentProgress2 * 100 / requirement) : 0;
						int previewPercent = (previewRequirement > 0) ? (resultProgress2 * 100 / previewRequirement) : 0;
						int deltaPercent = ((increaseCount2 > 0 || previewProgress2 > currentProgress2) && requirement > 0) ? ((previewProgress2 - currentProgress2) * 100 / requirement) : 0;
						ui.Refresh(attributeType, currentAttrValue2, percent, previewPercent, increaseCount2 > 0 || previewProgress2 > currentProgress2, increaseCount2, deltaPercent);
					}
					displayIndex++;
				}
				bool flag5 = this._dropCoroutine == null && this._dropItemQueue.Count == 0;
				if (flag5)
				{
					this.SetMachineWaterHeight(0.5f);
				}
				this.btnConfirm.interactable = (this.itemSelector.SelectedItems.Count > 0 && !this._isUpgrading);
				this.UpdateAttributeIncreaseIndicators();
				TooltipInvoker tipDisplayer = this.btnConfirm.GetComponent<TooltipInvoker>();
				bool flag6 = tipDisplayer != null;
				if (flag6)
				{
					tipDisplayer.enabled = true;
					bool flag7 = tipDisplayer.RuntimeParam == null;
					if (flag7)
					{
						tipDisplayer.RuntimeParam = new ArgumentBox();
					}
					ViewZhujianGearMate parentView = this.ParentView;
					string gearMateName = (((parentView != null) ? parentView.DisplayData : null) != null) ? NameCenter.GetMonasticTitleOrDisplayName(this.ParentView.DisplayData, false) : LocalStringManager.Get(LanguageKey.LK_GearMate_Tab_0);
					tipDisplayer.RuntimeParam.Set("Desc", LocalStringManager.GetFormat(LanguageKey.UI_MouseTipGearMateUpgradeAttribute_Desc, gearMateName).ColorReplace());
				}
			}
		}

		// Token: 0x060079DE RID: 31198 RVA: 0x00389C74 File Offset: 0x00387E74
		private void UpdateAttributePreview()
		{
			this.UpdateAttributeDisplay();
		}

		// Token: 0x060079DF RID: 31199 RVA: 0x00389C80 File Offset: 0x00387E80
		private void PlayItemDropAnimation(ItemKey itemKey)
		{
			GameObject itemObj = Object.Instantiate<GameObject>(this.itemDropPrefab, this.itemDropPrefab.transform.parent);
			itemObj.SetActive(false);
			Vector3 pos = Vector3.Lerp(this.itemDropLeftPoint.position, this.itemDropRightPoint.position, Random.Range(0f, 1f));
			itemObj.transform.position = pos;
			CImage image = itemObj.GetComponent<CImage>();
			bool flag = image != null;
			if (flag)
			{
				image.SetSprite(Config.Material.Instance[itemKey.TemplateId].Icon, false, null);
			}
			ZhujianGearMateDropItem itemDrop = itemObj.GetComponent<ZhujianGearMateDropItem>();
			bool flag2 = itemDrop != null;
			if (flag2)
			{
				itemDrop.ExplosionPrefab = this.itemExplodeEffect;
				itemDrop.OnTrigger += delegate()
				{
					ParticleSystem ps;
					bool flag5 = this.sparkEffect.TryGetComponent<ParticleSystem>(out ps);
					if (flag5)
					{
						ps.Play();
					}
				};
			}
			this._dropItemQueue.Enqueue(itemObj);
			bool flag3 = this._dropCoroutine == null;
			if (flag3)
			{
				bool flag4 = itemDrop != null;
				if (flag4)
				{
					itemDrop.OnTrigger += delegate()
					{
						this.SetMachineWaterHeight(1.5f);
					};
				}
				this._dropCoroutine = base.StartCoroutine(this.ProcessDropQueue());
			}
		}

		// Token: 0x060079E0 RID: 31200 RVA: 0x00389DA3 File Offset: 0x00387FA3
		private IEnumerator ProcessDropQueue()
		{
			yield return null;
			bool flag = this._dropItemQueue.Count == 0;
			if (flag)
			{
				this._dropCoroutine = null;
				yield break;
			}
			float averageInterval = 1f / (float)this._dropItemQueue.Count;
			bool flag2 = averageInterval > 0.3f;
			if (flag2)
			{
				averageInterval = 0.3f;
			}
			while (this._dropItemQueue.Count > 0)
			{
				int count = 1;
				bool flag3 = Time.deltaTime > averageInterval;
				if (flag3)
				{
					count = Mathf.RoundToInt(Time.deltaTime / averageInterval);
				}
				int i = 0;
				while (i < count && this._dropItemQueue.Count > 0)
				{
					GameObject item = this._dropItemQueue.Dequeue();
					bool flag4 = item != null;
					if (flag4)
					{
						item.SetActive(true);
					}
					int num = i;
					i = num + 1;
					bool flag5 = i >= 2;
					if (flag5)
					{
						break;
					}
					item = null;
				}
				bool flag6 = this._dropItemQueue.Count > 0;
				if (flag6)
				{
					yield return new WaitForSeconds(averageInterval);
				}
			}
			this._dropCoroutine = null;
			yield break;
		}

		// Token: 0x060079E1 RID: 31201 RVA: 0x00389DB4 File Offset: 0x00387FB4
		private void ClearDropQueue()
		{
			bool flag = this._dropCoroutine != null;
			if (flag)
			{
				base.StopCoroutine(this._dropCoroutine);
				this._dropCoroutine = null;
			}
			while (this._dropItemQueue.Count > 0)
			{
				GameObject item = this._dropItemQueue.Dequeue();
				bool flag2 = item != null;
				if (flag2)
				{
					Object.Destroy(item);
				}
			}
		}

		// Token: 0x060079E2 RID: 31202 RVA: 0x00389E1C File Offset: 0x0038801C
		private static int GetUpgradeRequirement(int attrValue)
		{
			return (1 + attrValue) * 10;
		}

		// Token: 0x060079E3 RID: 31203 RVA: 0x00389E34 File Offset: 0x00388034
		private static int CalcIncreaseCount(int processValue, int attrValue, out int resultProcessValue)
		{
			int increaseCount = 0;
			resultProcessValue = processValue;
			int req = ZhujianGearMateSubPageAttribute.GetUpgradeRequirement(attrValue + increaseCount);
			while (resultProcessValue >= req && req > 0)
			{
				resultProcessValue -= req;
				increaseCount++;
				req = ZhujianGearMateSubPageAttribute.GetUpgradeRequirement(attrValue + increaseCount);
			}
			return increaseCount;
		}

		// Token: 0x060079E4 RID: 31204 RVA: 0x00389E7C File Offset: 0x0038807C
		private new static int CalcGradeProcessValue(sbyte grade)
		{
			return 5 * (int)Math.Pow(2.0, (double)grade);
		}

		// Token: 0x060079E5 RID: 31205 RVA: 0x00389EA4 File Offset: 0x003880A4
		private new void SetMachineWaterHeight(float duration = 0.5f)
		{
			int totalDelta = 0;
			bool flag = this._gearMateData != null;
			if (flag)
			{
				for (int i = 0; i < 6; i++)
				{
					bool flag2 = i < this._previewProcessValue.Length && i < this._gearMateData.MainAttributeProgress.Length;
					if (flag2)
					{
						totalDelta += this._previewProcessValue[i] - this._gearMateData.MainAttributeProgress[i];
					}
				}
			}
			float height = (float)totalDelta / (float)(totalDelta + ZhujianGearMateSubPageAttribute.CalcGradeProcessValue(8));
			bool flag3 = height < 0.001f;
			if (flag3)
			{
				height = 0.001f;
			}
			bool flag4 = height > 1f;
			if (flag4)
			{
				height = 1f;
			}
			bool flag5 = this._heightCoroutine != null;
			if (flag5)
			{
				base.StopCoroutine(this._heightCoroutine);
			}
			bool showSpark = height > 0.001f;
			this.sparkEffect.SetActive(showSpark);
			bool flag6 = !this._isUpgrading;
			if (flag6)
			{
				bool flag7 = showSpark;
				if (flag7)
				{
					this.masterLightEffect.SetActive(true);
				}
			}
			this._heightCoroutine = base.StartCoroutine(this.ScaleHandle(height, duration, delegate
			{
				bool flag8 = !this._isUpgrading && height <= 0.001f;
				if (flag8)
				{
					this.masterLightEffect.SetActive(false);
				}
			}));
		}

		// Token: 0x060079E6 RID: 31206 RVA: 0x00389FFC File Offset: 0x003881FC
		private void PlayUpgradeAnim(Action onComplete)
		{
			this.ParentView.SetChangeButtonInteractable(false);
			this.machineSpine.AnimationState.SetAnimation(0, "move", false);
			AudioManager.Instance.PlaySound("SFX_GearMate_machine_loop", false, false);
			bool flag = this._heightCoroutine != null;
			if (flag)
			{
				base.StopCoroutine(this._heightCoroutine);
			}
			this._heightCoroutine = base.StartCoroutine(this.ScaleHandle(0f, this.animDuration, null));
			int randomValue = Random.Range(0, 3);
			this.ParentView.ShowBubble(LocalStringManager.Get(LanguageKey.LK_GearMateAttribute_SpeakWord0 + randomValue), this.animDuration);
			this.ParentView.DoGearMateAnimation("break_1");
			int coroutineCount = 0;
			bool anyUpgrade = false;
			int upgradeCount = 0;
			for (int i = 0; i < 6; i++)
			{
				int displayIndex = ZhujianGearMateSubPageAttribute.GetDisplayIndexByAttributeType(i);
				bool flag2 = displayIndex >= this.attributeUIs.Count;
				if (!flag2)
				{
					ZhujianGearMateAttribute ui = this.attributeUIs[displayIndex];
					int currentProgress = this._gearMateData.MainAttributeProgress[i];
					int previewProgress = this._previewProcessValue[i];
					bool flag3 = previewProgress > currentProgress;
					if (flag3)
					{
						anyUpgrade = true;
						upgradeCount++;
						int coroutineCount2 = coroutineCount;
						coroutineCount = coroutineCount2 + 1;
						ui.SetLightActive(true);
						int currentAttrValue = this._currentAttributeValue[i];
						int resultProgress;
						int increaseCount = ZhujianGearMateSubPageAttribute.CalcIncreaseCount(previewProgress, currentAttrValue, out resultProgress);
						int previewAttrValue = currentAttrValue + increaseCount;
						int currentReq = ZhujianGearMateSubPageAttribute.GetUpgradeRequirement(currentAttrValue);
						int previewReq = ZhujianGearMateSubPageAttribute.GetUpgradeRequirement(previewAttrValue);
						float currentPercent = (currentReq > 0) ? ((float)currentProgress / (float)currentReq) : 0f;
						float finalTargetPercent = (previewReq > 0) ? ((float)resultProgress / (float)previewReq) : 0f;
						base.StartCoroutine(this.AnimateAttributeUpgrade(ui, i, currentAttrValue, increaseCount, currentPercent, finalTargetPercent, delegate()
						{
							ui.SetLightActive(false);
							int num = coroutineCount - 1;
							coroutineCount = num;
							bool flag6 = num == 0;
							if (flag6)
							{
								bool hasPreviewChange = this.HasAnyPreviewChange();
								this.masterLightEffect.SetActive(hasPreviewChange);
								Action onComplete2 = onComplete;
								if (onComplete2 != null)
								{
									onComplete2();
								}
							}
						}));
					}
				}
			}
			bool flag4 = upgradeCount > 0;
			if (flag4)
			{
				this.masterLightEffect.SetActive(true);
			}
			bool flag5 = !anyUpgrade;
			if (flag5)
			{
				base.StartCoroutine(ZhujianGearMateSubPageAttribute.WaitForAnim(this.animDuration, onComplete));
			}
		}

		// Token: 0x060079E7 RID: 31207 RVA: 0x0038A25C File Offset: 0x0038845C
		private IEnumerator AnimateAttributeUpgrade(ZhujianGearMateAttribute ui, int index, int startValue, int increaseCount, float startPercent, float endPercent, Action onComplete)
		{
			float totalAnimTime = this.animDuration;
			float timePerStage = totalAnimTime / (float)(increaseCount + 1);
			int currentValue = startValue;
			float target = (increaseCount > 0) ? 1f : endPercent;
			yield return base.StartCoroutine(ZhujianGearMateSubPageAttribute.AnimateProgress(ui, startPercent, target, timePerStage));
			bool flag = increaseCount > 0;
			if (flag)
			{
				int num = currentValue + 1;
				currentValue = num;
				this.PlayUpgradeEffect(ui, index, num);
				for (int i = 0; i < increaseCount - 1; i = num + 1)
				{
					yield return base.StartCoroutine(ZhujianGearMateSubPageAttribute.AnimateProgress(ui, 0f, 1f, timePerStage));
					num = currentValue + 1;
					currentValue = num;
					this.PlayUpgradeEffect(ui, index, num);
					num = i;
				}
				yield return base.StartCoroutine(ZhujianGearMateSubPageAttribute.AnimateProgress(ui, 0f, endPercent, timePerStage));
			}
			if (onComplete != null)
			{
				onComplete();
			}
			yield break;
		}

		// Token: 0x060079E8 RID: 31208 RVA: 0x0038A2AB File Offset: 0x003884AB
		private static IEnumerator AnimateProgress(ZhujianGearMateAttribute ui, float start, float end, float duration)
		{
			float elapsed = 0f;
			while (elapsed < duration)
			{
				elapsed += Time.deltaTime;
				float t = elapsed / duration;
				ui.SetProgress(Mathf.Lerp(start, end, t));
				yield return null;
			}
			ui.SetProgress(end);
			yield break;
		}

		// Token: 0x060079E9 RID: 31209 RVA: 0x0038A2D0 File Offset: 0x003884D0
		private void PlayUpgradeEffect(ZhujianGearMateAttribute ui, int index, int value)
		{
			AudioManager.Instance.PlaySound("SFX_GearMate_machine_up", false, false);
			bool flag = this.upgradeEffectPrefab;
			if (flag)
			{
				GameObject effect = Object.Instantiate<GameObject>(this.upgradeEffectPrefab, ui.transform);
				ParticleSystem ps;
				bool flag2 = effect.TryGetComponent<ParticleSystem>(out ps);
				if (flag2)
				{
					ps.Play();
				}
				Object.Destroy(effect, 2f);
			}
			ui.SetValueText(Mathf.Clamp(value, 0, (int)GlobalConfig.Instance.MaxValueOfMaxMainAttributes).ToString());
		}

		// Token: 0x060079EA RID: 31210 RVA: 0x0038A354 File Offset: 0x00388554
		private IEnumerator ScaleHandle(float endScaleX, float duration, Action onComplete = null)
		{
			float time = 0f;
			Vector3 startScale = this.handle.localScale;
			Vector3 endScale = new Vector3(endScaleX, startScale.y, startScale.z);
			while (time < duration)
			{
				time += Time.deltaTime;
				this.handle.localScale = Vector3.Lerp(startScale, endScale, time / duration);
				yield return null;
			}
			this.handle.localScale = endScale;
			if (onComplete != null)
			{
				onComplete();
			}
			yield break;
		}

		// Token: 0x060079EB RID: 31211 RVA: 0x0038A378 File Offset: 0x00388578
		private static IEnumerator WaitForAnim(float time, Action onComplete)
		{
			yield return new WaitForSeconds(time);
			if (onComplete != null)
			{
				onComplete();
			}
			yield break;
		}

		// Token: 0x060079EC RID: 31212 RVA: 0x0038A390 File Offset: 0x00388590
		private void UpdateAttributeIncreaseIndicators()
		{
			int i = 0;
			while (i < 6 && i < this.attributeIncreaseIndicators.Length)
			{
				bool shouldShow = this._previewProcessValue[i] > this._gearMateData.MainAttributeProgress[i];
				bool flag = this.attributeIncreaseIndicators[i] != null;
				if (flag)
				{
					this.attributeIncreaseIndicators[i].SetActive(shouldShow);
				}
				i++;
			}
		}

		// Token: 0x060079ED RID: 31213 RVA: 0x0038A3FC File Offset: 0x003885FC
		private bool HasAnyPreviewChange()
		{
			bool flag = this._gearMateData == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				for (int i = 0; i < 6; i++)
				{
					bool flag2 = i < this._previewProcessValue.Length && i < this._gearMateData.MainAttributeProgress.Length;
					if (flag2)
					{
						bool flag3 = this._previewProcessValue[i] > this._gearMateData.MainAttributeProgress[i];
						if (flag3)
						{
							return true;
						}
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x04005C4F RID: 23631
		[SerializeField]
		private List<ZhujianGearMateAttribute> attributeUIs;

		// Token: 0x04005C50 RID: 23632
		[SerializeField]
		private Button btnConfirm;

		// Token: 0x04005C51 RID: 23633
		[SerializeField]
		private ZhujianGearMateItemSelector itemSelector;

		// Token: 0x04005C52 RID: 23634
		[Header("Animation")]
		[SerializeField]
		private SkeletonGraphic machineSpine;

		// Token: 0x04005C53 RID: 23635
		[SerializeField]
		private Transform handle;

		// Token: 0x04005C54 RID: 23636
		[SerializeField]
		private GameObject sparkEffect;

		// Token: 0x04005C55 RID: 23637
		[SerializeField]
		private GameObject upgradeEffectPrefab;

		// Token: 0x04005C56 RID: 23638
		[SerializeField]
		private float animDuration = 2f;

		// Token: 0x04005C57 RID: 23639
		[Header("Item Drop")]
		[SerializeField]
		private GameObject itemDropPrefab;

		// Token: 0x04005C58 RID: 23640
		[SerializeField]
		private Transform itemDropLeftPoint;

		// Token: 0x04005C59 RID: 23641
		[SerializeField]
		private Transform itemDropRightPoint;

		// Token: 0x04005C5A RID: 23642
		[SerializeField]
		private GameObject itemExplodeEffect;

		// Token: 0x04005C5B RID: 23643
		[SerializeField]
		private GameObject masterLightEffect;

		// Token: 0x04005C5C RID: 23644
		[Header("Attribute Increase Indicators")]
		[SerializeField]
		private GameObject[] attributeIncreaseIndicators;

		// Token: 0x04005C5D RID: 23645
		private Coroutine _heightCoroutine;

		// Token: 0x04005C5E RID: 23646
		private Queue<GameObject> _dropItemQueue = new Queue<GameObject>();

		// Token: 0x04005C5F RID: 23647
		private Coroutine _dropCoroutine;

		// Token: 0x04005C60 RID: 23648
		private new const float DropItemInterval = 0.3f;

		// Token: 0x04005C61 RID: 23649
		private const float DropDuration = 1f;

		// Token: 0x04005C62 RID: 23650
		private bool _isUpgrading = false;

		// Token: 0x04005C63 RID: 23651
		private const int BaseUpgradeRequirement = 10;

		// Token: 0x04005C64 RID: 23652
		private const int ProcessValueMultiplier = 5;

		// Token: 0x04005C65 RID: 23653
		private const int ProcessValueBase = 2;

		// Token: 0x04005C66 RID: 23654
		private static readonly int[] AttributeDisplayOrder = new int[]
		{
			0,
			3,
			1,
			4,
			2,
			5
		};

		// Token: 0x04005C67 RID: 23655
		private GearMate _gearMateData;

		// Token: 0x04005C68 RID: 23656
		private int[] _currentAttributeValue = new int[6];

		// Token: 0x04005C69 RID: 23657
		private int[] _previewProcessValue = new int[6];

		// Token: 0x04005C6A RID: 23658
		private int[] _currentPreviewAttributeValue = new int[6];

		// Token: 0x04005C6B RID: 23659
		private int _currentSourceIndex = 0;

		// Token: 0x04005C6C RID: 23660
		private int _currentFilterAttributeType = -1;
	}
}
