using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Views.Building.BuildingAreaQuickActionMenu.LeftButton;
using Game.Views.Building.BuildingManage;
using GameData.Domains.Building;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Extra;
using GameData.Domains.Item.Display;
using GameData.Domains.Organization;
using GameData.Domains.Story;
using GameData.Domains.Taiwu;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.Building.BuildingAreaQuickActionMenu
{
	// Token: 0x02000C27 RID: 3111
	public class ViewBuildingQuickActionMenu : UIBase
	{
		// Token: 0x170010B1 RID: 4273
		// (get) Token: 0x06009DCC RID: 40396 RVA: 0x0049E0EA File Offset: 0x0049C2EA
		// (set) Token: 0x06009DCD RID: 40397 RVA: 0x0049E0F2 File Offset: 0x0049C2F2
		public short AreaId { get; private set; }

		// Token: 0x170010B2 RID: 4274
		// (get) Token: 0x06009DCE RID: 40398 RVA: 0x0049E0FB File Offset: 0x0049C2FB
		// (set) Token: 0x06009DCF RID: 40399 RVA: 0x0049E103 File Offset: 0x0049C303
		public short BlockId { get; private set; }

		// Token: 0x170010B3 RID: 4275
		// (get) Token: 0x06009DD0 RID: 40400 RVA: 0x0049E10C File Offset: 0x0049C30C
		// (set) Token: 0x06009DD1 RID: 40401 RVA: 0x0049E114 File Offset: 0x0049C314
		public BuildingBlockData BlockData { get; set; }

		// Token: 0x170010B4 RID: 4276
		// (get) Token: 0x06009DD2 RID: 40402 RVA: 0x0049E11D File Offset: 0x0049C31D
		// (set) Token: 0x06009DD3 RID: 40403 RVA: 0x0049E125 File Offset: 0x0049C325
		public BuildingBlockItem ConfigData { get; private set; }

		// Token: 0x170010B5 RID: 4277
		// (get) Token: 0x06009DD4 RID: 40404 RVA: 0x0049E12E File Offset: 0x0049C32E
		// (set) Token: 0x06009DD5 RID: 40405 RVA: 0x0049E136 File Offset: 0x0049C336
		public BuildingBlockKey BlockKey { get; private set; }

		// Token: 0x170010B6 RID: 4278
		// (get) Token: 0x06009DD6 RID: 40406 RVA: 0x0049E13F File Offset: 0x0049C33F
		public bool IsTaiwuVillageBuilding
		{
			get
			{
				return this._isTaiwuVillageBuilding;
			}
		}

		// Token: 0x170010B7 RID: 4279
		// (get) Token: 0x06009DD7 RID: 40407 RVA: 0x0049E147 File Offset: 0x0049C347
		// (set) Token: 0x06009DD8 RID: 40408 RVA: 0x0049E14F File Offset: 0x0049C34F
		public BuildingModel BuildingModel { get; private set; }

		// Token: 0x170010B8 RID: 4280
		// (get) Token: 0x06009DD9 RID: 40409 RVA: 0x0049E158 File Offset: 0x0049C358
		// (set) Token: 0x06009DDA RID: 40410 RVA: 0x0049E160 File Offset: 0x0049C360
		public LeftLayoutManager LeftLayoutManager { get; private set; }

		// Token: 0x06009DDB RID: 40411 RVA: 0x0049E16C File Offset: 0x0049C36C
		public float GetBaseRadius()
		{
			return (this.ConfigData.Width > 1) ? this.largeBuildingConfig.baseRadius : this.smallBuildingConfig.baseRadius;
		}

		// Token: 0x06009DDC RID: 40412 RVA: 0x0049E1A4 File Offset: 0x0049C3A4
		public BuildingSizeConfig GetCurrentConfig()
		{
			return (this.ConfigData.Width > 1) ? this.largeBuildingConfig : this.smallBuildingConfig;
		}

		// Token: 0x06009DDD RID: 40413 RVA: 0x0049E1D2 File Offset: 0x0049C3D2
		private void OnBuildingBlockDataChange(ArgumentBox argsBox)
		{
			this.RefreshCircleMenuButtons();
		}

		// Token: 0x06009DDE RID: 40414 RVA: 0x0049E1DC File Offset: 0x0049C3DC
		public void RefreshCircleMenuButtons()
		{
			foreach (ViewBuildingQuickActionMenu.ButtonState buttonState in this._managedButtons)
			{
				bool flag = buttonState.ButtonObject == null;
				if (!flag)
				{
					CButtonObsolete button = buttonState.ButtonObject.GetComponent<CButtonObsolete>();
					bool flag2 = button != null && buttonState.IsEnabledFunc != null;
					if (flag2)
					{
						button.interactable = buttonState.IsEnabledFunc();
					}
				}
			}
		}

		// Token: 0x06009DDF RID: 40415 RVA: 0x0049E27C File Offset: 0x0049C47C
		public void RefreshAllButtonStates()
		{
			this.RefreshCircleMenuButtons();
		}

		// Token: 0x06009DE0 RID: 40416 RVA: 0x0049E286 File Offset: 0x0049C486
		private void InitButtonStates()
		{
			this.RefreshCircleMenuButtons();
		}

		// Token: 0x06009DE1 RID: 40417 RVA: 0x0049E290 File Offset: 0x0049C490
		private void OnQuickRemoveHover(GameObject buttonObj)
		{
		}

		// Token: 0x06009DE2 RID: 40418 RVA: 0x0049E293 File Offset: 0x0049C493
		private void OnRepairHover(GameObject buttonObj)
		{
		}

		// Token: 0x06009DE3 RID: 40419 RVA: 0x0049E296 File Offset: 0x0049C496
		private void OnRenameHover(GameObject buttonObj)
		{
		}

		// Token: 0x06009DE4 RID: 40420 RVA: 0x0049E29C File Offset: 0x0049C49C
		private int CountItem(Dictionary<ItemSourceType, List<ItemDisplayData>> itemDict, sbyte itemType, short itemTemplateId)
		{
			return (from item in (from v in itemDict.Values
			where v != null
			select v).SelectMany((List<ItemDisplayData> list) => list)
			where item.Key.ItemType == itemType && item.Key.TemplateId == itemTemplateId && !item.IsLocked
			select item).Sum((ItemDisplayData item) => item.Amount);
		}

		// Token: 0x06009DE5 RID: 40421 RVA: 0x0049E348 File Offset: 0x0049C548
		public void FetchItemCount(short itemTemplateId, Action<int> callback)
		{
			Dictionary<ItemSourceType, List<ItemDisplayData>> itemDict = new Dictionary<ItemSourceType, List<ItemDisplayData>>();
			List<ItemSourceType> itemSources = new List<ItemSourceType>
			{
				ItemSourceType.Inventory,
				ItemSourceType.Warehouse,
				ItemSourceType.Treasury
			};
			int completedCount = 0;
			bool hasCompleted = false;
			AsyncMethodCallbackDelegate <>9__0;
			foreach (ItemSourceType itemSource in itemSources)
			{
				ItemSourceType itemSourceType = itemSource;
				AsyncMethodCallbackDelegate callback2;
				if ((callback2 = <>9__0) == null)
				{
					callback2 = (<>9__0 = delegate(int offset, RawDataPool pool)
					{
						bool hasCompleted = hasCompleted;
						if (!hasCompleted)
						{
							try
							{
								ValueTuple<ItemSourceType, List<ItemDisplayData>> tuple = default(ValueTuple<ItemSourceType, List<ItemDisplayData>>);
								Serializer.Deserialize(pool, offset, ref tuple);
								itemDict[tuple.Item1] = tuple.Item2;
								int completedCount = completedCount;
								completedCount++;
								bool flag = completedCount == itemSources.Count;
								if (flag)
								{
									hasCompleted = true;
									int totalCount = this.CountItem(itemDict, 12, itemTemplateId);
									Action<int> callback3 = callback;
									if (callback3 != null)
									{
										callback3(totalCount);
									}
								}
							}
							catch (Exception)
							{
								bool flag2 = !hasCompleted;
								if (flag2)
								{
									hasCompleted = true;
									Action<int> callback4 = callback;
									if (callback4 != null)
									{
										callback4(0);
									}
								}
							}
						}
					});
				}
				TaiwuDomainMethod.AsyncCall.GetAllItems(this, itemSourceType, callback2);
			}
		}

		// Token: 0x06009DE6 RID: 40422 RVA: 0x0049E418 File Offset: 0x0049C618
		public void FetchAvailableWorkers(Action<List<int>> onComplete)
		{
			TaiwuDomainMethod.AsyncCall.GetAllVillagersAvailableForWork(this, delegate(int offset, RawDataPool pool)
			{
				Serializer.Deserialize(pool, offset, ref this._availableWorkers);
				Action<List<int>> onComplete2 = onComplete;
				if (onComplete2 != null)
				{
					onComplete2(this._availableWorkers);
				}
			});
		}

		// Token: 0x06009DE7 RID: 40423 RVA: 0x0049E44D File Offset: 0x0049C64D
		public List<int> GetAvailableWorkers()
		{
			return this._availableWorkers;
		}

		// Token: 0x06009DE8 RID: 40424 RVA: 0x0049E458 File Offset: 0x0049C658
		public void PrepareWorkerRelatedData()
		{
			for (int i = 0; i < this._shopManagerListCached.Length; i++)
			{
				this._shopManagerListCached[i] = -1;
			}
			List<int> shopManagerList = this.BuildingModel.GetBuildingShopManager(this.BlockKey);
			int j = 0;
			while (j < shopManagerList.Count && j < this._shopManagerListCached.Length)
			{
				this._shopManagerListCached[j] = shopManagerList[j];
				j++;
			}
			this._unlockWorkingList.Clear();
		}

		// Token: 0x06009DE9 RID: 40425 RVA: 0x0049E4DC File Offset: 0x0049C6DC
		public void PrepareWorkerRelatedDataAsync(Action onComplete)
		{
			this.PrepareWorkerRelatedData();
			CharacterDomainMethod.AsyncCall.GetCharacterDisplayDataList(this, this._availableWorkers, delegate(int offset, RawDataPool pool)
			{
				List<CharacterDisplayData> dataList = new List<CharacterDisplayData>();
				Serializer.Deserialize(pool, offset, ref dataList);
				this._charDisplayDataDict.Clear();
				foreach (CharacterDisplayData data in dataList)
				{
					this._charDisplayDataDict[data.CharacterId] = new CharacterDisplayData
					{
						CharacterId = data.CharacterId,
						CompletelyInfected = data.CompletelyInfected
					};
				}
				Action onComplete2 = onComplete;
				if (onComplete2 != null)
				{
					onComplete2();
				}
			});
		}

		// Token: 0x06009DEA RID: 40426 RVA: 0x0049E520 File Offset: 0x0049C720
		public bool IsAvailableWorker(int id)
		{
			Dictionary<int, VillagerWorkData> villagerWork = this.BuildingModel.VillagerWork;
			bool flag = villagerWork != null && villagerWork.ContainsKey(id);
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				CharacterDisplayData displayData;
				bool flag2 = this._charDisplayDataDict.TryGetValue(id, out displayData) && displayData.CompletelyInfected;
				result = !flag2;
			}
			return result;
		}

		// Token: 0x06009DEB RID: 40427 RVA: 0x0049E574 File Offset: 0x0049C774
		public bool HasAvailableWorkerForExpandRemove()
		{
			int count = this._availableWorkers.Count((int id) => !this._shopManagerListCached.Contains(id) && this.IsAvailableWorker(id));
			count += this._unlockWorkingList.Count;
			return count > 0;
		}

		// Token: 0x06009DEC RID: 40428 RVA: 0x0049E5B0 File Offset: 0x0049C7B0
		public override void OnInit(ArgumentBox argsBox)
		{
			bool flag = argsBox == null;
			if (flag)
			{
				this.NeedWaitData = false;
			}
			else
			{
				short areaId;
				argsBox.Get("AreaId", out areaId);
				this.AreaId = areaId;
				short blockId;
				argsBox.Get("BlockId", out blockId);
				this.BlockId = blockId;
				argsBox.Get("BuildingBlockIndex", out this._buildingBlockIndex);
				BuildingBlockData blockData;
				argsBox.Get<BuildingBlockData>("BuildingBlockData", out blockData);
				this.BlockData = blockData;
				argsBox.Get("IsTaiwuVillageBuilding", out this._isTaiwuVillageBuilding);
				RectTransform backRectTransform;
				argsBox.Get<RectTransform>("BackRectTransform", out backRectTransform);
				this._backRectTransform = backRectTransform;
				float cameraMoveDuration;
				argsBox.Get("CameraMoveDuration", out cameraMoveDuration);
				this.QuickActionMenuCameraMoveDuration = cameraMoveDuration;
				float currentScale;
				argsBox.Get("CurrentScale", out currentScale);
				this.CurrentScale = currentScale;
				argsBox.Get<BuildingAreaData>("BuildingAreaData", out this._areaData);
				this.BlockKey = new BuildingBlockKey(this.AreaId, this.BlockId, this._buildingBlockIndex);
				this.ConfigData = BuildingBlock.Instance[this.BlockData.TemplateId];
				this._settlementId = SingletonObject.getInstance<WorldMapModel>().GetTaiwuVillageSettlementId();
				this.BuildingModel = SingletonObject.getInstance<BuildingModel>();
				this.ApplyScale();
				this.AlignToBuilding();
				this.NeedWaitData = true;
				BuildingDomainMethod.AsyncCall.GetBuildingManageDisplayData(this, this.BlockKey, delegate(int offset, RawDataPool pool)
				{
					Serializer.Deserialize(pool, offset, ref this._displayData);
					this.ShowCircleMenu();
					SingletonObject.getInstance<YieldHelper>().DelayFrameDo(2U, delegate
					{
						this.Element.ShowAfterRefresh();
					});
				});
			}
		}

		// Token: 0x06009DED RID: 40429 RVA: 0x0049E714 File Offset: 0x0049C914
		private void Awake()
		{
			bool flag = this.leftButtonTemplate != null;
			if (flag)
			{
				PoolManager.SetSrcObjectWithTurnOff("UIBuildingQuickActionMenu.LeftButton", this.leftButtonTemplate);
			}
			bool flag2 = this.topButtonTemplate != null;
			if (flag2)
			{
				PoolManager.SetSrcObjectWithTurnOff("UIBuildingQuickActionMenu.TopButton", this.topButtonTemplate);
			}
			bool flag3 = this.rightButtonTemplate != null;
			if (flag3)
			{
				PoolManager.SetSrcObjectWithTurnOff("UIBuildingQuickActionMenu.RightButton", this.rightButtonTemplate);
			}
			bool flag4 = this.bottomButtonTemplate != null;
			if (flag4)
			{
				PoolManager.SetSrcObjectWithTurnOff("UIBuildingQuickActionMenu.BottomButton", this.bottomButtonTemplate);
			}
		}

		// Token: 0x06009DEE RID: 40430 RVA: 0x0049E7A8 File Offset: 0x0049C9A8
		private void OnDestroy()
		{
			bool flag = this.leftButtonTemplate != null;
			if (flag)
			{
				PoolManager.RemoveData("UIBuildingQuickActionMenu.LeftButton");
			}
			bool flag2 = this.topButtonTemplate != null;
			if (flag2)
			{
				PoolManager.RemoveData("UIBuildingQuickActionMenu.TopButton");
			}
			bool flag3 = this.rightButtonTemplate != null;
			if (flag3)
			{
				PoolManager.RemoveData("UIBuildingQuickActionMenu.RightButton");
			}
			bool flag4 = this.bottomButtonTemplate != null;
			if (flag4)
			{
				PoolManager.RemoveData("UIBuildingQuickActionMenu.BottomButton");
			}
		}

		// Token: 0x06009DEF RID: 40431 RVA: 0x0049E824 File Offset: 0x0049CA24
		private void OnEnable()
		{
			GEvent.Add(UiEvents.CloseBuildingManage, new GEvent.Callback(this.OnCloseBuildingManage));
			GEvent.Add(UiEvents.BuildingBlockDataChange, new GEvent.Callback(this.OnBuildingBlockDataChange));
			base.StartCoroutine(this.DelayedAlignToBuilding());
		}

		// Token: 0x06009DF0 RID: 40432 RVA: 0x0049E871 File Offset: 0x0049CA71
		private IEnumerator DelayedAlignToBuilding()
		{
			yield return null;
			bool flag = this.QuickActionMenuCameraMoveDuration > 0f;
			if (flag)
			{
				yield return new WaitForSeconds(this.QuickActionMenuCameraMoveDuration);
			}
			this.ApplyScale();
			this.AlignToBuilding();
			yield break;
		}

		// Token: 0x06009DF1 RID: 40433 RVA: 0x0049E880 File Offset: 0x0049CA80
		private void OnDisable()
		{
			base.StopAllCoroutines();
			GEvent.Remove(UiEvents.CloseBuildingManage, new GEvent.Callback(this.OnCloseBuildingManage));
			GEvent.Remove(UiEvents.BuildingBlockDataChange, new GEvent.Callback(this.OnBuildingBlockDataChange));
		}

		// Token: 0x06009DF2 RID: 40434 RVA: 0x0049E8BC File Offset: 0x0049CABC
		private void ClearAllButtons()
		{
			bool flag = this.leftButtonTemplate != null;
			if (flag)
			{
				foreach (LeftButtonProcessor button in this._leftButtons)
				{
					PoolManager.Destroy("UIBuildingQuickActionMenu.LeftButton", button.ButtonObject);
				}
			}
			this._leftButtons.Clear();
			bool flag2 = this.topButtonTemplate != null;
			if (flag2)
			{
				foreach (GameObject button2 in this._topButtons)
				{
					PoolManager.Destroy("UIBuildingQuickActionMenu.TopButton", button2);
				}
			}
			this._topButtons.Clear();
			bool flag3 = this.rightButtonTemplate != null;
			if (flag3)
			{
				foreach (GameObject button3 in this._rightButtons)
				{
					PoolManager.Destroy("UIBuildingQuickActionMenu.RightButton", button3);
				}
			}
			this._rightButtons.Clear();
			bool flag4 = this.bottomButtonTemplate != null;
			if (flag4)
			{
				foreach (GameObject button4 in this._bottomButtons)
				{
					PoolManager.Destroy("UIBuildingQuickActionMenu.BottomButton", button4);
				}
			}
			this._bottomButtons.Clear();
			this._managedButtons.Clear();
		}

		// Token: 0x06009DF3 RID: 40435 RVA: 0x0049EA90 File Offset: 0x0049CC90
		private void ShowCircleMenu()
		{
			this.ClearAllButtons();
			this.SetupCircleBackground();
			this.ApplyScale();
			this.LeftLayoutManager = new LeftLayoutManager(this);
			this.CreateLeftButtonProcessors();
			this.CreateTopButtons();
			this.CreateRightButtons();
			this.CreateBottomButtons();
			this.InitButtonStates();
		}

		// Token: 0x06009DF4 RID: 40436 RVA: 0x0049EAE4 File Offset: 0x0049CCE4
		private void ApplyScale()
		{
			bool flag = this.circleMenuRoot != null;
			if (flag)
			{
				this.circleMenuRoot.transform.localScale = Vector3.one * this.CurrentScale;
			}
		}

		// Token: 0x06009DF5 RID: 40437 RVA: 0x0049EB28 File Offset: 0x0049CD28
		private void SetupCircleBackground()
		{
			bool flag = this.smallBuildingConfig.Background == null || this.largeBuildingConfig.Background == null;
			if (!flag)
			{
				this.smallBuildingConfig.Background.gameObject.SetActive(this.ConfigData.Width <= 1);
				this.largeBuildingConfig.Background.gameObject.SetActive(this.ConfigData.Width > 1);
			}
		}

		// Token: 0x06009DF6 RID: 40438 RVA: 0x0049EBB0 File Offset: 0x0049CDB0
		protected override void OnClick(Transform btn)
		{
			bool flag = btn.name == "BgButton";
			if (flag)
			{
				this.OnBackgroundClick();
			}
		}

		// Token: 0x06009DF7 RID: 40439 RVA: 0x0049EBDC File Offset: 0x0049CDDC
		private void OnBackgroundClick()
		{
			GEvent.OnEvent(UiEvents.QuickActionMenuBackgroundClicked, EasyPool.Get<ArgumentBox>().SetObject("buildingKey", this.BlockKey).Set("areaId", this.AreaId).Set("blockId", this.BlockId));
			this.QuickHide();
		}

		// Token: 0x06009DF8 RID: 40440 RVA: 0x0049EC3B File Offset: 0x0049CE3B
		private void OpenBuildingManage(Game.Views.Building.BuildingManage.BuildingManageTogKey tabKey = Game.Views.Building.BuildingManage.BuildingManageTogKey.Invalid)
		{
			GEvent.OnEvent(UiEvents.RequestOpenBuildingManage, EasyPool.Get<ArgumentBox>().SetObject("buildingKey", this.BlockKey).Set("tabKey", tabKey));
		}

		// Token: 0x06009DF9 RID: 40441 RVA: 0x0049EC78 File Offset: 0x0049CE78
		private void OnCloseBuildingManage(ArgumentBox argsBox)
		{
			bool isClose;
			bool flag = argsBox != null && argsBox.Get("isClose", out isClose) && isClose;
			if (flag)
			{
				this.QuickHide();
			}
		}

		// Token: 0x06009DFA RID: 40442 RVA: 0x0049ECA8 File Offset: 0x0049CEA8
		public void UpdateBuildingArea()
		{
			UIElement.BuildingArea.UiBaseAs<ViewBuildingArea>().UpdateBuildingData(this.BlockKey, this.BlockData, true);
		}

		// Token: 0x06009DFB RID: 40443 RVA: 0x0049ECC8 File Offset: 0x0049CEC8
		private void AlignToBuilding()
		{
			bool flag = this._backRectTransform != null && this.circleMenuRoot != null;
			if (flag)
			{
				Vector3[] rectCorners = new Vector3[4];
				this._backRectTransform.GetWorldCorners(rectCorners);
				Vector3 centerWorld = (rectCorners[0] + rectCorners[2]) * 0.5f;
				RectTransform parent;
				bool flag2 = this.circleMenuRoot.transform.parent.TryGetComponent<RectTransform>(out parent);
				if (flag2)
				{
					Vector3 centerLocal = parent.InverseTransformPoint(centerWorld);
					this.circleMenuRoot.GetComponent<RectTransform>().anchoredPosition = centerLocal;
				}
			}
		}

		// Token: 0x06009DFC RID: 40444 RVA: 0x0049ED6C File Offset: 0x0049CF6C
		public static void SetButtonSprites(CButton button, string[] spriteNames, bool needPressState = true)
		{
			button.GetComponent<CImage>().SetSprite(spriteNames[0], false, null);
			SpriteState spriteState = default(SpriteState);
			ResLoader.Load<Sprite>("RemakeResources/UIGraphics5.0/Ui9Building/" + spriteNames[1], delegate(Sprite sprite)
			{
				spriteState.highlightedSprite = sprite;
			}, null, false);
			ResLoader.Load<Sprite>("RemakeResources/UIGraphics5.0/Ui9Building/" + spriteNames[2], delegate(Sprite sprite)
			{
				spriteState.pressedSprite = (needPressState ? sprite : null);
				spriteState.selectedSprite = sprite;
			}, null, false);
			ResLoader.Load<Sprite>("RemakeResources/UIGraphics5.0/Ui9Building/" + spriteNames[3], delegate(Sprite sprite)
			{
				spriteState.disabledSprite = sprite;
				button.spriteState = spriteState;
			}, null, false);
		}

		// Token: 0x06009DFD RID: 40445 RVA: 0x0049EE14 File Offset: 0x0049D014
		private void CreateBottomButtons()
		{
			List<BottomButtonType> showTabTypeList = new List<BottomButtonType>();
			List<Vector2> buttonPositions = new List<Vector2>();
			for (int i = 0; i < 1; i++)
			{
				BottomButtonType type = (BottomButtonType)i;
				bool canShowTab = this.GetCanShowTab(type);
				if (canShowTab)
				{
					showTabTypeList.Add(type);
					buttonPositions.Add(Vector2.zero);
				}
			}
			BuildingSizeConfig config = this.GetCurrentConfig();
			float centerAngle = config.bottomGroupCenterAngle;
			float spacingAngle = config.bottomGroupSpacingAngle;
			List<float> angles = BuildingActionUtils.CalculateGroupAngles(centerAngle, buttonPositions.Count, spacingAngle);
			for (int index = 0; index < showTabTypeList.Count; index++)
			{
				BottomButtonType type2 = showTabTypeList[index];
				this.CreateBottomButton(type2, angles[index]);
			}
		}

		// Token: 0x06009DFE RID: 40446 RVA: 0x0049EED0 File Offset: 0x0049D0D0
		private bool GetCanShowTab(BottomButtonType type)
		{
			if (!true)
			{
			}
			if (type != BottomButtonType.Remove)
			{
				throw new ArgumentOutOfRangeException("type", type, null);
			}
			bool result = ViewBuildingManage.CanShowRemove(this.BlockData, this._isTaiwuVillageBuilding);
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x06009DFF RID: 40447 RVA: 0x0049EF14 File Offset: 0x0049D114
		private string GetTabName(BottomButtonType type)
		{
			if (!true)
			{
			}
			if (type != BottomButtonType.Remove)
			{
				throw new ArgumentOutOfRangeException("type", type, null);
			}
			string result = LanguageKey.LK_Building_Remove.Tr();
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x06009E00 RID: 40448 RVA: 0x0049EF50 File Offset: 0x0049D150
		private Action GetClickTabAction(BottomButtonType type)
		{
			if (!true)
			{
			}
			if (type != BottomButtonType.Remove)
			{
				throw new ArgumentOutOfRangeException("type", type, null);
			}
			Action result = new Action(this.OnRemoveTabClick);
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x06009E01 RID: 40449 RVA: 0x0049EF8C File Offset: 0x0049D18C
		public static string[] GetTabSpriteNames(BottomButtonType type)
		{
			string prefix = "ui9_btn_building_quick_action_menu_" + Enum.GetName(typeof(BottomButtonType), type).ToLower();
			return new string[]
			{
				prefix + "_0",
				prefix + "_1",
				prefix + "_2",
				prefix + "_3"
			};
		}

		// Token: 0x06009E02 RID: 40450 RVA: 0x0049F004 File Offset: 0x0049D204
		private void CreateBottomButton(BottomButtonType buttonType, float angle)
		{
			GameObject buttonObj = PoolManager.GetObject("UIBuildingQuickActionMenu.BottomButton");
			buttonObj.transform.SetParent(this.bottomButtonsRoot, false);
			buttonObj.SetActive(true);
			string buttonName = this.GetTabName(buttonType);
			CButton button = buttonObj.GetComponent<CButton>();
			button.interactable = true;
			string[] spriteNames = ViewBuildingQuickActionMenu.GetTabSpriteNames(buttonType);
			ViewBuildingQuickActionMenu.SetButtonSprites(button, spriteNames, true);
			Action action = this.GetClickTabAction(buttonType);
			button.ClearAndAddListener(action);
			TooltipInvoker tip = buttonObj.GetComponent<TooltipInvoker>();
			tip.PresetParam = new string[]
			{
				buttonName
			};
			RectTransform rectTransform = buttonObj.GetComponent<RectTransform>();
			rectTransform.anchoredPosition = BuildingActionUtils.PolarToCartesian(angle, this.GetBaseRadius());
			this._bottomButtons.Add(buttonObj);
			this._managedButtons.Add(new ViewBuildingQuickActionMenu.ButtonState(buttonObj, () => this.GetCanShowTab(buttonType), null, null, null));
		}

		// Token: 0x170010B9 RID: 4281
		// (get) Token: 0x06009E03 RID: 40451 RVA: 0x0049F0F9 File Offset: 0x0049D2F9
		public List<LeftButtonProcessor> LeftButtons
		{
			get
			{
				return this._leftButtons;
			}
		}

		// Token: 0x06009E04 RID: 40452 RVA: 0x0049F104 File Offset: 0x0049D304
		private void CreateLeftButtonProcessors()
		{
			this._leftButtons.Clear();
			this._leftButtons.Add(new QuickExpandProcessor(this, this.CreateLeftButtonObject(), LeftButtonType.QuickExpand));
			this._leftButtons.Add(new QuickRemoveProcessor(this, this.CreateLeftButtonObject(), LeftButtonType.QuickRemove));
			this._leftButtons.Add(new QuickRenameProcessor(this, this.CreateLeftButtonObject(), LeftButtonType.QuickRename));
			this._leftButtons.Add(new QuickRepairProcessor(this, this.CreateLeftButtonObject(), LeftButtonType.QuickRepair));
			foreach (LeftButtonProcessor leftButton in this._leftButtons)
			{
				leftButton.PrepareData();
			}
		}

		// Token: 0x06009E05 RID: 40453 RVA: 0x0049F1CC File Offset: 0x0049D3CC
		private GameObject CreateLeftButtonObject()
		{
			GameObject buttonObj = PoolManager.GetObject("UIBuildingQuickActionMenu.LeftButton");
			buttonObj.transform.SetParent(this.leftButtonsRoot, false);
			buttonObj.SetActive(false);
			return buttonObj;
		}

		// Token: 0x06009E06 RID: 40454 RVA: 0x0049F208 File Offset: 0x0049D408
		public static string[] GetTabSpriteNames(LeftButtonType type)
		{
			bool flag = type == LeftButtonType.QuickRemove;
			string[] result;
			if (flag)
			{
				result = ViewBuildingQuickActionMenu.GetTabSpriteNames(BottomButtonType.Remove);
			}
			else
			{
				string prefix = "ui9_btn_building_quick_action_menu_" + Enum.GetName(typeof(LeftButtonType), type).ToLower();
				result = new string[]
				{
					prefix + "_0",
					prefix + "_1",
					prefix + "_2",
					prefix + "_3"
				};
			}
			return result;
		}

		// Token: 0x06009E07 RID: 40455 RVA: 0x0049F290 File Offset: 0x0049D490
		public void ShowRenameDialog(string currentName)
		{
			new RenameCfg
			{
				Title = LanguageKey.LK_Building_QuickAction_Rename_Title.Tr(),
				Description = LanguageKey.LK_Building_QuickAction_Rename_Desc.TrFormat(currentName),
				EmptyDesc = LanguageKey.LK_Building_QuickAction_Rename_Empty.Tr(),
				Default = currentName,
				Submit = new Action<string>(this.OnRenameConfirm),
				CharCount = ViewBuildingManage.GetBuildingNameCharCount()
			}.Show();
		}

		// Token: 0x06009E08 RID: 40456 RVA: 0x0049F2FE File Offset: 0x0049D4FE
		private void OnRenameConfirm(string name)
		{
			BuildingDomainMethod.Call.SetBuildingCustomName(this.BlockKey, name);
		}

		// Token: 0x06009E09 RID: 40457 RVA: 0x0049F310 File Offset: 0x0049D510
		private void CreateRightButtons()
		{
			List<RightButtonType> showTabTypeList = new List<RightButtonType>();
			List<Vector2> buttonPositions = new List<Vector2>();
			for (int i = 0; i < 9; i++)
			{
				RightButtonType type = (RightButtonType)i;
				bool canShowTab = this.GetCanShowTab(type);
				if (canShowTab)
				{
					showTabTypeList.Add(type);
					buttonPositions.Add(Vector2.zero);
				}
			}
			BuildingSizeConfig config = this.GetCurrentConfig();
			float centerAngle = config.rightGroupCenterAngle;
			float spacingAngle = config.rightGroupSpacingAngle;
			List<float> angles = BuildingActionUtils.CalculateGroupAngles(centerAngle, buttonPositions.Count, spacingAngle);
			for (int index = 0; index < showTabTypeList.Count; index++)
			{
				RightButtonType type2 = showTabTypeList[index];
				this.CreateRightButton(type2, angles[index]);
			}
		}

		// Token: 0x06009E0A RID: 40458 RVA: 0x0049F3CC File Offset: 0x0049D5CC
		private bool GetCanShowTab(RightButtonType type)
		{
			if (!true)
			{
			}
			bool result;
			switch (type)
			{
			case RightButtonType.Info:
				result = ViewBuildingManage.CanShowInfo();
				break;
			case RightButtonType.Shop:
				result = ViewBuildingManage.CanShowShop(this.BlockData, this._isTaiwuVillageBuilding);
				break;
			case RightButtonType.Production:
				result = ViewBuildingManage.CanShowProduction(this.BlockData, this._isTaiwuVillageBuilding);
				break;
			case RightButtonType.Entertain:
			case RightButtonType.Reward:
				result = ViewBuildingManage.CanShowEntertainAndReword(this.BlockData);
				break;
			case RightButtonType.Expand:
				result = (ViewBuildingManage.CanShowExpand(this.BlockData) || ViewBuildingManage.CanShowUpgrade(this.BlockData));
				break;
			case RightButtonType.SamsaraPlatformRecord:
				result = ViewBuildingManage.CanShowSamsaraPlatformRecord(this.BlockData);
				break;
			case RightButtonType.TeaHouseCaravanAwareness:
				result = ViewBuildingManage.CanShowTeaHorseCaravanAware(this.BlockData);
				break;
			case RightButtonType.ChickenCoop:
				result = ViewBuildingManage.CanShowChickenCoop(this.BlockData);
				break;
			default:
				throw new ArgumentOutOfRangeException("type", type, null);
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x06009E0B RID: 40459 RVA: 0x0049F4AC File Offset: 0x0049D6AC
		private string GetTabName(RightButtonType type)
		{
			if (!true)
			{
			}
			string result;
			switch (type)
			{
			case RightButtonType.Info:
				result = LanguageKey.LK_Building_Message.Tr();
				break;
			case RightButtonType.Shop:
				result = LanguageKey.LK_Building_Shop.Tr();
				break;
			case RightButtonType.Production:
				result = LanguageKey.LK_Building_Production.Tr();
				break;
			case RightButtonType.Entertain:
				result = LanguageKey.LK_Building_Tab_Entertain.Tr();
				break;
			case RightButtonType.Reward:
				result = LanguageKey.LK_Building_Tab_Reward.Tr();
				break;
			case RightButtonType.Expand:
				result = ViewBuildingManage.GetExpandToggleText(this.ConfigData);
				break;
			case RightButtonType.SamsaraPlatformRecord:
				result = LanguageKey.LK_Building_Samsara_Platform_Record.Tr();
				break;
			case RightButtonType.TeaHouseCaravanAwareness:
				result = LanguageKey.LK_Building_TeaHorse_Awareness_Title.Tr();
				break;
			case RightButtonType.ChickenCoop:
				result = LanguageKey.LK_Building_ChickenCoop.Tr();
				break;
			default:
				throw new ArgumentOutOfRangeException("type", type, null);
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x06009E0C RID: 40460 RVA: 0x0049F578 File Offset: 0x0049D778
		private Action GetClickTabAction(RightButtonType type)
		{
			if (!true)
			{
			}
			Action result;
			switch (type)
			{
			case RightButtonType.Info:
				result = new Action(this.OnInfoTabClick);
				break;
			case RightButtonType.Shop:
				result = new Action(this.OnShopTabClick);
				break;
			case RightButtonType.Production:
				result = new Action(this.OnProductionTabClick);
				break;
			case RightButtonType.Entertain:
				result = new Action(this.OnEntertainTabClick);
				break;
			case RightButtonType.Reward:
				result = new Action(this.OnRewardTabClick);
				break;
			case RightButtonType.Expand:
				result = new Action(this.OnExpandTabClick);
				break;
			case RightButtonType.SamsaraPlatformRecord:
				result = new Action(this.OnSamsaraPlatformRecordTabClick);
				break;
			case RightButtonType.TeaHouseCaravanAwareness:
				result = new Action(this.OnTeaHouseCaravanAwarenessTabClick);
				break;
			case RightButtonType.ChickenCoop:
				result = new Action(this.OnChickenCoopTabClick);
				break;
			default:
				throw new ArgumentOutOfRangeException("type", type, null);
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x06009E0D RID: 40461 RVA: 0x0049F65C File Offset: 0x0049D85C
		public static string[] GetTabSpriteNames(RightButtonType type)
		{
			string prefix = "ui9_btn_building_quick_action_menu_" + Enum.GetName(typeof(RightButtonType), type).ToLower();
			return new string[]
			{
				prefix + "_0",
				prefix + "_1",
				prefix + "_2",
				prefix + "_3"
			};
		}

		// Token: 0x06009E0E RID: 40462 RVA: 0x0049F6D4 File Offset: 0x0049D8D4
		private void CreateRightButton(RightButtonType buttonType, float angle)
		{
			GameObject buttonObj = PoolManager.GetObject("UIBuildingQuickActionMenu.RightButton");
			buttonObj.transform.SetParent(this.rightButtonsRoot, false);
			buttonObj.SetActive(true);
			string buttonName = this.GetTabName(buttonType);
			CButton button = buttonObj.GetComponent<CButton>();
			button.interactable = true;
			string[] spriteNames = ViewBuildingQuickActionMenu.GetTabSpriteNames(buttonType);
			ViewBuildingQuickActionMenu.SetButtonSprites(button, spriteNames, true);
			Action action = this.GetClickTabAction(buttonType);
			button.ClearAndAddListener(action);
			TooltipInvoker tip = buttonObj.GetComponent<TooltipInvoker>();
			tip.PresetParam = new string[]
			{
				buttonName
			};
			RectTransform rectTransform = buttonObj.GetComponent<RectTransform>();
			rectTransform.anchoredPosition = BuildingActionUtils.PolarToCartesian(angle, this.GetBaseRadius());
			this._rightButtons.Add(buttonObj);
			this._managedButtons.Add(new ViewBuildingQuickActionMenu.ButtonState(buttonObj, () => this.GetCanShowTab(buttonType), null, null, null));
		}

		// Token: 0x06009E0F RID: 40463 RVA: 0x0049F7CC File Offset: 0x0049D9CC
		private bool CanShowDamageTab()
		{
			return false;
		}

		// Token: 0x06009E10 RID: 40464 RVA: 0x0049F7DF File Offset: 0x0049D9DF
		private void OnInfoTabClick()
		{
			this.OpenBuildingManage(Game.Views.Building.BuildingManage.BuildingManageTogKey.Info);
			this.QuickHide();
		}

		// Token: 0x06009E11 RID: 40465 RVA: 0x0049F7F1 File Offset: 0x0049D9F1
		private void OnShopTabClick()
		{
			this.OpenBuildingManage(Game.Views.Building.BuildingManage.BuildingManageTogKey.Shop);
			this.QuickHide();
		}

		// Token: 0x06009E12 RID: 40466 RVA: 0x0049F803 File Offset: 0x0049DA03
		private void OnProductionTabClick()
		{
			this.OpenBuildingManage(Game.Views.Building.BuildingManage.BuildingManageTogKey.Production);
			this.QuickHide();
		}

		// Token: 0x06009E13 RID: 40467 RVA: 0x0049F818 File Offset: 0x0049DA18
		private void OnExpandTabClick()
		{
			short templateId = this.ConfigData.TemplateId;
			bool flag = templateId == 47 || templateId == 46 || templateId == 48;
			if (flag)
			{
				this.OpenBuildingManage(Game.Views.Building.BuildingManage.BuildingManageTogKey.Upgrade);
			}
			else
			{
				this.OpenBuildingManage(Game.Views.Building.BuildingManage.BuildingManageTogKey.Expand);
			}
			this.QuickHide();
		}

		// Token: 0x06009E14 RID: 40468 RVA: 0x0049F867 File Offset: 0x0049DA67
		private void OnRemoveTabClick()
		{
			this.OpenBuildingManage(Game.Views.Building.BuildingManage.BuildingManageTogKey.Remove);
			this.QuickHide();
		}

		// Token: 0x06009E15 RID: 40469 RVA: 0x0049F87A File Offset: 0x0049DA7A
		private void OnEntertainTabClick()
		{
			this.OpenBuildingManage(Game.Views.Building.BuildingManage.BuildingManageTogKey.Entertain);
			this.QuickHide();
		}

		// Token: 0x06009E16 RID: 40470 RVA: 0x0049F88D File Offset: 0x0049DA8D
		private void OnRewardTabClick()
		{
			this.OpenBuildingManage(Game.Views.Building.BuildingManage.BuildingManageTogKey.Reward);
			this.QuickHide();
		}

		// Token: 0x06009E17 RID: 40471 RVA: 0x0049F8A0 File Offset: 0x0049DAA0
		private void OnSamsaraPlatformRecordTabClick()
		{
			this.OpenBuildingManage(Game.Views.Building.BuildingManage.BuildingManageTogKey.SamsaraPlatformRecord);
			this.QuickHide();
		}

		// Token: 0x06009E18 RID: 40472 RVA: 0x0049F8B2 File Offset: 0x0049DAB2
		private void OnTeaHouseCaravanAwarenessTabClick()
		{
			this.OpenBuildingManage(Game.Views.Building.BuildingManage.BuildingManageTogKey.TeaHouseCaravanAwareness);
			this.QuickHide();
		}

		// Token: 0x06009E19 RID: 40473 RVA: 0x0049F8C4 File Offset: 0x0049DAC4
		private void OnChickenCoopTabClick()
		{
			this.OpenBuildingManage(Game.Views.Building.BuildingManage.BuildingManageTogKey.Chicken);
			this.QuickHide();
		}

		// Token: 0x06009E1A RID: 40474 RVA: 0x0049F8D8 File Offset: 0x0049DAD8
		private void CreateTopButtons()
		{
			bool flag = this.BlockData.TemplateId == 44;
			if (flag)
			{
				this.CreateTopButton(TopButtonType.Cricket, delegate
				{
					BuildingActionUtils.ShowCricketCollection();
				}, null);
				this.RefreshTopButtonsLayout();
				bool canOperateStoneRoom = SingletonObject.getInstance<BuildingModel>().CanOperateStoneRoom;
				if (canOperateStoneRoom)
				{
					this.CreateTopButton(TopButtonType.StoneRoom, delegate
					{
						BuildingActionUtils.ShowStoneHouse(this.BlockKey);
					}, null);
					this.RefreshTopButtonsLayout();
				}
				ExtraDomainMethod.AsyncCall.GetIsJiaoPoolOpen(this, delegate(int offset, RawDataPool dataPool)
				{
					bool isOpen = false;
					Serializer.Deserialize(dataPool, offset, ref isOpen);
					bool flag18 = isOpen;
					if (flag18)
					{
						this.CreateTopButton(TopButtonType.JiaoPool, delegate
						{
							BuildingActionUtils.ShowJiaoPool(this._areaData);
						}, null);
						this.RefreshTopButtonsLayout();
					}
				});
			}
			else
			{
				bool flag2 = this.BlockData.TemplateId == 52;
				if (flag2)
				{
					bool isTaiwuVillageBuilding = this._isTaiwuVillageBuilding;
					if (isTaiwuVillageBuilding)
					{
						CButton button = this.CreateTopButton(TopButtonType.KungfuPracticeRoom, new Action(BuildingActionUtils.ShowKungfuPracticeRoom), null);
						ViewBuildingManage.RefreshKungfuRoomButton(button, this._displayData);
						this.RefreshTopButtonsLayout();
					}
					this.CreateTopButton(TopButtonType.PracticeCombatSkill, delegate
					{
						ViewBuildingManage.OpenKungfuRoomButtonPracticingCombatSkill(this.BlockKey, this._displayData);
					}, null);
					this.RefreshTopButtonsLayout();
				}
				else
				{
					bool flag3 = this.BlockData.TemplateId == 50;
					if (flag3)
					{
						this.CreateTopButton(TopButtonType.SamsaraPlatform, delegate
						{
							BuildingActionUtils.ShowSamsaraPlatform(this.BlockKey);
						}, null);
						this.RefreshTopButtonsLayout();
						OrganizationDomainMethod.AsyncCall.GetSectFunctionStatus(this, 11, SectFunctionStatuses.SectFunctionStatusType.SpecialInteractionUnlocked, delegate(int offset, RawDataPool dataPool)
						{
							bool isOpen = false;
							Serializer.Deserialize(dataPool, offset, ref isOpen);
							bool flag18 = isOpen;
							if (flag18)
							{
								this.CreateTopButton(TopButtonType.SwapSoul, delegate
								{
									BuildingActionUtils.ShowSwapSoul();
								}, null);
								this.RefreshTopButtonsLayout();
							}
						});
						StoryDomainMethod.AsyncCall.JingangMonkSoulBtnShow(this, delegate(int offset, RawDataPool dataPool)
						{
							bool isOpen = false;
							Serializer.Deserialize(dataPool, offset, ref isOpen);
							bool flag18 = isOpen;
							if (flag18)
							{
								this.CreateTopButton(TopButtonType.MonkSoul, delegate
								{
									BuildingActionUtils.ShowMonkSoul();
								}, null);
								this.RefreshTopButtonsLayout();
							}
						});
					}
					else
					{
						bool flag4 = this.BlockData.TemplateId == 49;
						if (flag4)
						{
							this.CreateTopButton(TopButtonType.Trough, delegate
							{
								BuildingActionUtils.ShowTrough();
							}, null);
							this.RefreshTopButtonsLayout();
							OrganizationDomainMethod.AsyncCall.GetSectFunctionStatus(this, 14, SectFunctionStatuses.SectFunctionStatusType.SpecialInteractionUnlocked, delegate(int offset, RawDataPool dataPool)
							{
								bool isOpen = false;
								Serializer.Deserialize(dataPool, offset, ref isOpen);
								bool flag18 = isOpen;
								if (flag18)
								{
									this.CreateTopButton(TopButtonType.AssignChicken, delegate
									{
										BuildingActionUtils.ShowVillageRoleChickenAssign();
									}, null);
									this.RefreshTopButtonsLayout();
								}
							});
						}
						else
						{
							bool flag5 = this.BlockData.TemplateId == 45;
							if (flag5)
							{
								this.CreateTopButton(TopButtonType.TaiwuVillageLineage, new Action(BuildingActionUtils.ShowVillageRole), null);
								this.CreateTopButton(TopButtonType.TaiwuLifeSummary, new Action(BuildingActionUtils.ShowTaiwuLifeSummary), null);
								this.RefreshTopButtonsLayout();
							}
							else
							{
								bool flag6 = this.BlockData.TemplateId == 51;
								if (flag6)
								{
									this.CreateTopButton(TopButtonType.TeaHouseCaravan, delegate
									{
										BuildingActionUtils.ShowTeaHorseCaravan(this.BlockData, this.BlockKey);
									}, null);
									this.RefreshTopButtonsLayout();
								}
								else
								{
									bool flag7 = this.BlockData.TemplateId == 48;
									if (flag7)
									{
										this.CreateTopButton(TopButtonType.Warehouse, delegate
										{
											BuildingActionUtils.ShowWarehouse(ItemSourceType.Warehouse);
										}, null);
										this.RefreshTopButtonsLayout();
										bool showMoreTog = SingletonObject.getInstance<BasicGameData>().CanShowMoreTogOnWarehouse();
										bool flag8 = showMoreTog;
										if (flag8)
										{
											this.CreateTopButton(TopButtonType.Treasury, delegate
											{
												BuildingActionUtils.ShowWarehouse(ItemSourceType.Treasury);
											}, null);
											this.RefreshTopButtonsLayout();
											this.CreateTopButton(TopButtonType.Stock, delegate
											{
												BuildingActionUtils.ShowWarehouse(ItemSourceType.Stock);
											}, null);
											this.RefreshTopButtonsLayout();
										}
									}
									else
									{
										bool flag9 = this.BlockData.TemplateId >= 276 && this.BlockData.TemplateId <= 282;
										if (flag9)
										{
											this.CreateTopButton(TopButtonType.MerchantBuilding, delegate
											{
												BuildingActionUtils.ShowMerchant(this.ConfigData, this.AreaId);
											}, null);
											this.RefreshTopButtonsLayout();
										}
										else
										{
											bool flag10 = this.BlockData.TemplateId == 283;
											if (flag10)
											{
												this.CreateTopButton(TopButtonType.WuHuZhenBao, delegate
												{
													BuildingActionUtils.ShowSpecialShop(this.ConfigData);
												}, null);
												this.RefreshTopButtonsLayout();
											}
											else
											{
												bool flag11 = this.BlockData.TemplateId >= 284 && this.BlockData.TemplateId <= 302;
												if (flag11)
												{
													this.CreateTopButton(TopButtonType.SettlementTreasury, delegate
													{
														BuildingActionUtils.ShowTreasuryShop(this.BlockData);
													}, null);
													this.RefreshTopButtonsLayout();
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			bool canMakeItem = this.ConfigData.CanMakeItem;
			if (canMakeItem)
			{
				this.CreateTopButton(TopButtonType.Make, delegate
				{
					BuildingActionUtils.ShowMake(this.BlockData, this.BlockKey, UI_Make.UIMakeTab.Make);
				}, null);
				this.RefreshTopButtonsLayout();
			}
			bool flag12 = this.ConfigData.AddReadingLifeSkillBookEfficiency == 6 || this.ConfigData.AddReadingLifeSkillBookEfficiency == 7 || this.ConfigData.AddReadingLifeSkillBookEfficiency == 10 || this.ConfigData.AddReadingLifeSkillBookEfficiency == 11 || this.ConfigData.AddReadingLifeSkillBookEfficiency == 8 || this.ConfigData.AddReadingLifeSkillBookEfficiency == 9 || this.BlockData.TemplateId == 257 || this.BlockData.TemplateId == 258;
			if (flag12)
			{
				this.CreateTopButton(TopButtonType.Repair, delegate
				{
					BuildingActionUtils.ShowMake(this.BlockData, this.BlockKey, UI_Make.UIMakeTab.Repair);
				}, null);
				this.RefreshTopButtonsLayout();
			}
			bool flag13 = this.ConfigData.AddReadingLifeSkillBookEfficiency == 9;
			if (flag13)
			{
				this.CreateTopButton(TopButtonType.Poison, delegate
				{
					BuildingActionUtils.ShowMake(this.BlockData, this.BlockKey, UI_Make.UIMakeTab.Poison);
				}, null);
				this.RefreshTopButtonsLayout();
				this.CreateTopButton(TopButtonType.RemovePoison, delegate
				{
					BuildingActionUtils.ShowMake(this.BlockData, this.BlockKey, UI_Make.UIMakeTab.RemovePoison);
				}, null);
				this.RefreshTopButtonsLayout();
			}
			bool flag14 = this.ConfigData.AddReadingLifeSkillBookEfficiency == 6 || this.ConfigData.AddReadingLifeSkillBookEfficiency == 7 || this.ConfigData.AddReadingLifeSkillBookEfficiency == 10 || this.ConfigData.AddReadingLifeSkillBookEfficiency == 11 || this.BlockData.TemplateId == 257 || this.BlockData.TemplateId == 258;
			if (flag14)
			{
				this.CreateTopButton(TopButtonType.Refine, delegate
				{
					BuildingActionUtils.ShowMake(this.BlockData, this.BlockKey, UI_Make.UIMakeTab.Refine);
				}, null);
				this.RefreshTopButtonsLayout();
			}
			bool flag15 = this.ConfigData.AddReadingLifeSkillBookEfficiency == 10;
			if (flag15)
			{
				this.CreateTopButton(TopButtonType.Weave, delegate
				{
					BuildingActionUtils.ShowMake(this.BlockData, this.BlockKey, UI_Make.UIMakeTab.Weave);
				}, null);
				this.RefreshTopButtonsLayout();
			}
			bool flag16 = this.ConfigData.TemplateId >= 303 && this.ConfigData.TemplateId <= 317;
			if (flag16)
			{
				this.CreateTopButton(TopButtonType.Prison, delegate
				{
					BuildingActionUtils.ShowPrison();
				}, null);
				this.RefreshTopButtonsLayout();
				this.CreateTopButton(TopButtonType.Bounty, delegate
				{
					BuildingActionUtils.ShowBounty(this.BlockKey);
				}, null);
				this.RefreshTopButtonsLayout();
				this.CreateTopButton(TopButtonType.Law, delegate
				{
					BuildingActionUtils.ShowLaw(this.BlockKey);
				}, null);
				this.RefreshTopButtonsLayout();
			}
			bool flag17 = this.ConfigData.ArtisanOrderAvailable && this._isTaiwuVillageBuilding;
			if (flag17)
			{
				this.CreateTopButton(TopButtonType.Craftsman, delegate
				{
					BuildingActionUtils.ShowCraftMan(this.BlockData, this.BlockKey);
				}, null);
				this.RefreshTopButtonsLayout();
			}
		}

		// Token: 0x06009E1B RID: 40475 RVA: 0x0049FF78 File Offset: 0x0049E178
		private CButton CreateTopButton(TopButtonType buttonType, Action action, Func<bool> isEnabledFunc = null)
		{
			GameObject buttonObj = PoolManager.GetObject("UIBuildingQuickActionMenu.TopButton");
			buttonObj.transform.SetParent(this.topButtonsRoot, false);
			buttonObj.SetActive(true);
			string buttonName = ViewBuildingQuickActionMenu.GetTabName(buttonType, this.ConfigData.TemplateId);
			CButton button = buttonObj.GetComponent<CButton>();
			button.interactable = (this.BlockData.OperationType != 0);
			string[] spriteNames = ViewBuildingQuickActionMenu.GetTabSpriteNames(buttonType);
			ViewBuildingQuickActionMenu.SetButtonSprites(button, spriteNames, true);
			button.ClearAndAddListener(delegate
			{
				Action action2 = action;
				if (action2 != null)
				{
					action2();
				}
			});
			TooltipInvoker tip = buttonObj.GetComponent<TooltipInvoker>();
			StringBuilder stringBuilder = EasyPool.Get<StringBuilder>();
			stringBuilder.AppendLine(buttonName);
			bool flag = !button.interactable;
			if (flag)
			{
				stringBuilder.AppendLine(LanguageKey.LK_Building_Constructing.Tr());
			}
			tip.PresetParam = new string[]
			{
				stringBuilder.ToString()
			};
			EasyPool.Free<StringBuilder>(stringBuilder);
			this._topButtons.Add(buttonObj);
			List<ViewBuildingQuickActionMenu.ButtonState> managedButtons = this._managedButtons;
			GameObject button2 = buttonObj;
			Func<bool> isEnabled = isEnabledFunc;
			if (isEnabledFunc == null && (isEnabled = ViewBuildingQuickActionMenu.<>c.<>9__134_1) == null)
			{
				isEnabled = (ViewBuildingQuickActionMenu.<>c.<>9__134_1 = (() => true));
			}
			managedButtons.Add(new ViewBuildingQuickActionMenu.ButtonState(button2, isEnabled, null, null, null));
			return button;
		}

		// Token: 0x06009E1C RID: 40476 RVA: 0x004A00B4 File Offset: 0x0049E2B4
		private void RefreshTopButtonsLayout()
		{
			BuildingSizeConfig config = this.GetCurrentConfig();
			float spacingAngle = config.topGroupSpacingAngle;
			float centerAngle = config.topGroupCenterAngle;
			List<float> angles = BuildingActionUtils.CalculateGroupAngles(centerAngle, this._topButtons.Count, spacingAngle);
			for (int i = 0; i < this._topButtons.Count; i++)
			{
				GameObject buttonObj = this._topButtons[i];
				RectTransform rectTransform = buttonObj.GetComponent<RectTransform>();
				rectTransform.anchoredPosition = BuildingActionUtils.PolarToCartesian(angles[i], this.GetBaseRadius());
			}
		}

		// Token: 0x06009E1D RID: 40477 RVA: 0x004A0140 File Offset: 0x0049E340
		public static string GetTabName(TopButtonType type, short buildingBlockTemplateId)
		{
			if (!true)
			{
			}
			string result;
			switch (type)
			{
			case TopButtonType.SettlementTreasury:
				result = LanguageKey.LK_Building_Treasury.Tr();
				goto IL_27E;
			case TopButtonType.Prison:
				result = LanguageKey.LK_SettlementPrison.Tr();
				goto IL_27E;
			case TopButtonType.Bounty:
				result = LanguageKey.LK_BountyAmount_Short.Tr();
				goto IL_27E;
			case TopButtonType.Law:
				result = LanguageKey.LK_Law_Title.Tr();
				goto IL_27E;
			case TopButtonType.MerchantBuilding:
				result = LanguageKey.LK_Merchant.Tr();
				goto IL_27E;
			case TopButtonType.WuHuZhenBao:
				result = LanguageKey.LK_Merchant.Tr();
				goto IL_27E;
			case TopButtonType.Make:
				result = ((buildingBlockTemplateId == 257 || buildingBlockTemplateId == 258) ? LocalStringManager.Get(LanguageKey.LK_Building_FixWood) : LocalStringManager.Get(LanguageKey.LK_Make_Item));
				goto IL_27E;
			case TopButtonType.Repair:
				result = LanguageKey.LK_Building_Btn_Repair.Tr();
				goto IL_27E;
			case TopButtonType.Poison:
				result = LanguageKey.LK_Poison_Item.Tr();
				goto IL_27E;
			case TopButtonType.RemovePoison:
				result = LanguageKey.LK_Remove_Poison.Tr();
				goto IL_27E;
			case TopButtonType.Refine:
				result = LanguageKey.LK_Strengthen_Item.Tr();
				goto IL_27E;
			case TopButtonType.Weave:
				result = LanguageKey.LK_Weave_Item.Tr();
				goto IL_27E;
			case TopButtonType.Craftsman:
				result = LanguageKey.LK_Craftsman_Entry.Tr();
				goto IL_27E;
			case TopButtonType.TaiwuVillageLineage:
				result = LanguageKey.LK_Building_TaiwuVillageLineage_Name.Tr();
				goto IL_27E;
			case TopButtonType.TaiwuLifeSummary:
				result = LanguageKey.LK_TaiwuSummary_Title_Short.Tr();
				goto IL_27E;
			case TopButtonType.Warehouse:
				result = LanguageKey.LK_Warehouse.Tr();
				goto IL_27E;
			case TopButtonType.Treasury:
				result = LanguageKey.LK_Treasury.Tr();
				goto IL_27E;
			case TopButtonType.Stock:
				result = LanguageKey.LK_StockStorage.Tr();
				goto IL_27E;
			case TopButtonType.Trough:
				result = LanguageKey.LK_Trough.Tr();
				goto IL_27E;
			case TopButtonType.KungfuPracticeRoom:
				result = LanguageKey.LK_Building_KungfuRoomIcon.Tr();
				goto IL_27E;
			case TopButtonType.PracticeCombatSkill:
				result = LanguageKey.LK_PracticeCombatSkill_Name.Tr();
				goto IL_27E;
			case TopButtonType.Cricket:
				result = LanguageKey.LK_Building_Btn_Cricket.Tr();
				goto IL_27E;
			case TopButtonType.StoneRoom:
				result = LanguageKey.LK_Building_StoneRoom.Tr();
				goto IL_27E;
			case TopButtonType.JiaoPool:
				result = LanguageKey.LK_Building_Jiaochi.Tr();
				goto IL_27E;
			case TopButtonType.TeaHouseCaravan:
				result = LanguageKey.LK_Building_TeaHouseCaravan.Tr();
				goto IL_27E;
			case TopButtonType.SamsaraPlatform:
				result = LanguageKey.LK_Building_Samsara_Platform.Tr();
				goto IL_27E;
			case TopButtonType.AssignChicken:
				result = LanguageKey.LK_AssignChicken_Title.Tr();
				goto IL_27E;
			case TopButtonType.ChickenCoopEvent:
				result = LanguageKey.LK_Trigger_ChickenCoop_Event.Tr();
				goto IL_27E;
			case TopButtonType.SwapSoul:
				result = LanguageKey.LK_Building_Btn_SoulSwapCeremony.Tr();
				goto IL_27E;
			case TopButtonType.MonkSoul:
				result = LanguageKey.UI_SectMainStory_Jingang_MonkSoul.Tr();
				goto IL_27E;
			}
			throw new ArgumentOutOfRangeException("type", type, null);
			IL_27E:
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x06009E1E RID: 40478 RVA: 0x004A03D0 File Offset: 0x0049E5D0
		public static TopButtonType GetTabSpriteType(TopButtonType type)
		{
			if (!true)
			{
			}
			TopButtonType result;
			switch (type)
			{
			case TopButtonType.SettlementTreasury:
				result = TopButtonType.SettlementTreasury;
				break;
			case TopButtonType.Prison:
				result = TopButtonType.Prison;
				break;
			case TopButtonType.Bounty:
				result = TopButtonType.Bounty;
				break;
			case TopButtonType.Law:
				result = TopButtonType.Law;
				break;
			case TopButtonType.MerchantBuilding:
				result = TopButtonType.MerchantBuilding;
				break;
			case TopButtonType.WuHuZhenBao:
				result = TopButtonType.MerchantBuilding;
				break;
			case TopButtonType.Make:
				result = TopButtonType.Make;
				break;
			case TopButtonType.Repair:
				result = TopButtonType.Repair;
				break;
			case TopButtonType.Poison:
				result = TopButtonType.Poison;
				break;
			case TopButtonType.RemovePoison:
				result = TopButtonType.RemovePoison;
				break;
			case TopButtonType.Refine:
				result = TopButtonType.Refine;
				break;
			case TopButtonType.Weave:
				result = TopButtonType.Weave;
				break;
			case TopButtonType.Craftsman:
				result = TopButtonType.Craftsman;
				break;
			case TopButtonType.TaiwuVillageLineage:
				result = TopButtonType.TaiwuVillageLineage;
				break;
			case TopButtonType.TaiwuLifeSummary:
				result = TopButtonType.TaiwuLifeSummary;
				break;
			case TopButtonType.Warehouse:
				result = TopButtonType.Warehouse;
				break;
			case TopButtonType.Treasury:
				result = TopButtonType.Treasury;
				break;
			case TopButtonType.Stock:
				result = TopButtonType.Stock;
				break;
			case TopButtonType.Trough:
				result = TopButtonType.Trough;
				break;
			case TopButtonType.KungfuPracticeRoom:
				result = TopButtonType.KungfuPracticeRoom;
				break;
			case TopButtonType.PracticeCombatSkill:
				result = TopButtonType.PracticeCombatSkill;
				break;
			case TopButtonType.Cricket:
				result = TopButtonType.Cricket;
				break;
			case TopButtonType.StoneRoom:
				result = TopButtonType.StoneRoom;
				break;
			case TopButtonType.JiaoPool:
				result = TopButtonType.JiaoPool;
				break;
			case TopButtonType.TeaHouseCaravan:
				result = TopButtonType.TeaHouseCaravan;
				break;
			case TopButtonType.SamsaraPlatform:
				result = TopButtonType.SamsaraPlatform;
				break;
			case TopButtonType.ChickenCoop:
				result = TopButtonType.ChickenCoop;
				break;
			case TopButtonType.AssignChicken:
				result = TopButtonType.ChickenCoop;
				break;
			case TopButtonType.ChickenCoopEvent:
				result = TopButtonType.ChickenCoop;
				break;
			case TopButtonType.SwapSoul:
				result = TopButtonType.SwapSoul;
				break;
			case TopButtonType.MonkSoul:
				result = TopButtonType.SwapSoul;
				break;
			default:
				throw new ArgumentOutOfRangeException("type", type, null);
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x06009E1F RID: 40479 RVA: 0x004A0530 File Offset: 0x0049E730
		private static string[] GetTabSpriteNames(TopButtonType type)
		{
			TopButtonType realType = ViewBuildingQuickActionMenu.GetTabSpriteType(type);
			string prefix = "ui9_btn_building_quick_action_menu_" + Enum.GetName(typeof(TopButtonType), realType).ToLower();
			return new string[]
			{
				prefix + "_0",
				prefix + "_1",
				prefix + "_2",
				prefix + "_3"
			};
		}

		// Token: 0x04007A47 RID: 31303
		[Header("背景和根节点")]
		[SerializeField]
		private GameObject backgroundMask;

		// Token: 0x04007A48 RID: 31304
		[SerializeField]
		private GameObject circleMenuRoot;

		// Token: 0x04007A49 RID: 31305
		[Header("按钮组根节点")]
		[SerializeField]
		private RectTransform leftButtonsRoot;

		// Token: 0x04007A4A RID: 31306
		[SerializeField]
		private RectTransform topButtonsRoot;

		// Token: 0x04007A4B RID: 31307
		[SerializeField]
		private RectTransform rightButtonsRoot;

		// Token: 0x04007A4C RID: 31308
		[SerializeField]
		private RectTransform bottomButtonsRoot;

		// Token: 0x04007A4D RID: 31309
		[Header("按钮模板")]
		[SerializeField]
		private GameObject leftButtonTemplate;

		// Token: 0x04007A4E RID: 31310
		[SerializeField]
		private GameObject topButtonTemplate;

		// Token: 0x04007A4F RID: 31311
		[SerializeField]
		private GameObject rightButtonTemplate;

		// Token: 0x04007A50 RID: 31312
		[SerializeField]
		private GameObject bottomButtonTemplate;

		// Token: 0x04007A51 RID: 31313
		[Header("建筑尺寸配置")]
		[Tooltip("小建筑配置")]
		[SerializeField]
		private BuildingSizeConfig smallBuildingConfig = new BuildingSizeConfig();

		// Token: 0x04007A52 RID: 31314
		[Tooltip("大建筑配置")]
		[SerializeField]
		private BuildingSizeConfig largeBuildingConfig = new BuildingSizeConfig();

		// Token: 0x04007A53 RID: 31315
		private const string LeftButtonPoolKey = "UIBuildingQuickActionMenu.LeftButton";

		// Token: 0x04007A54 RID: 31316
		private const string TopButtonPoolKey = "UIBuildingQuickActionMenu.TopButton";

		// Token: 0x04007A55 RID: 31317
		private const string RightButtonPoolKey = "UIBuildingQuickActionMenu.RightButton";

		// Token: 0x04007A56 RID: 31318
		private const string BottomButtonPoolKey = "UIBuildingQuickActionMenu.BottomButton";

		// Token: 0x04007A59 RID: 31321
		private short _buildingBlockIndex;

		// Token: 0x04007A5D RID: 31325
		private bool _isTaiwuVillageBuilding;

		// Token: 0x04007A5E RID: 31326
		private short _settlementId;

		// Token: 0x04007A60 RID: 31328
		private RectTransform _backRectTransform;

		// Token: 0x04007A61 RID: 31329
		private readonly List<GameObject> _topButtons = new List<GameObject>();

		// Token: 0x04007A62 RID: 31330
		private readonly List<GameObject> _rightButtons = new List<GameObject>();

		// Token: 0x04007A63 RID: 31331
		private readonly List<GameObject> _bottomButtons = new List<GameObject>();

		// Token: 0x04007A64 RID: 31332
		private List<int> _availableWorkers = new List<int>();

		// Token: 0x04007A65 RID: 31333
		private readonly int[] _shopManagerListCached = new int[7];

		// Token: 0x04007A66 RID: 31334
		private List<int> _unlockWorkingList = new List<int>();

		// Token: 0x04007A67 RID: 31335
		private readonly Dictionary<int, CharacterDisplayData> _charDisplayDataDict = new Dictionary<int, CharacterDisplayData>();

		// Token: 0x04007A68 RID: 31336
		private BuildingAreaData _areaData;

		// Token: 0x04007A69 RID: 31337
		private readonly List<ViewBuildingQuickActionMenu.ButtonState> _managedButtons = new List<ViewBuildingQuickActionMenu.ButtonState>();

		// Token: 0x04007A6A RID: 31338
		private List<GameObject> _tipContentObjs;

		// Token: 0x04007A6C RID: 31340
		private float QuickActionMenuCameraMoveDuration = 0.15f;

		// Token: 0x04007A6D RID: 31341
		private float CurrentScale = 1f;

		// Token: 0x04007A6E RID: 31342
		private BuildingManageDisplayData _displayData;

		// Token: 0x04007A6F RID: 31343
		private readonly List<LeftButtonProcessor> _leftButtons = new List<LeftButtonProcessor>();

		// Token: 0x04007A70 RID: 31344
		[SerializeField]
		private GameObject renameObject;

		// Token: 0x04007A71 RID: 31345
		[SerializeField]
		private TMP_InputField renameInputField;

		// Token: 0x04007A72 RID: 31346
		[SerializeField]
		private CButton renameConfirmButton;

		// Token: 0x04007A73 RID: 31347
		[SerializeField]
		private CButton renameCancelButton;

		// Token: 0x0200233E RID: 9022
		private class ButtonState
		{
			// Token: 0x060102C9 RID: 66249 RVA: 0x00652E6E File Offset: 0x0065106E
			public ButtonState(GameObject button, Func<bool> isEnabled, GameObject hoverObj = null, Action<GameObject> hoverEnter = null, Action<GameObject> hoverExit = null)
			{
				this.ButtonObject = button;
				this.HoverObject = hoverObj;
				this.IsEnabledFunc = isEnabled;
				this.HoverEnterCallback = hoverEnter;
				this.HoverExitCallback = hoverExit;
			}

			// Token: 0x0400DE35 RID: 56885
			public GameObject ButtonObject;

			// Token: 0x0400DE36 RID: 56886
			public GameObject HoverObject;

			// Token: 0x0400DE37 RID: 56887
			public Func<bool> IsEnabledFunc;

			// Token: 0x0400DE38 RID: 56888
			public Action<GameObject> HoverEnterCallback;

			// Token: 0x0400DE39 RID: 56889
			public Action<GameObject> HoverExitCallback;
		}
	}
}
