using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coffee.UIExtensions;
using FrameWork;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using Game.Views.Select;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Extra;
using GameData.Domains.Map;
using GameData.Domains.Story;
using GameData.Domains.Story.SectMainStory;
using GameData.Domains.Taiwu;
using GameData.Domains.TaiwuEvent;
using GameData.Domains.World.Display;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using UnityEngine;

namespace Game.Views.SectInteract.Wudang
{
	// Token: 0x020009DE RID: 2526
	public class ViewDefendHeavenlyTree : UIBase
	{
		// Token: 0x17000D9E RID: 3486
		// (get) Token: 0x06007B6C RID: 31596 RVA: 0x00395432 File Offset: 0x00393632
		private List<SectStoryHeavenlyTreeExtendable> HeavenlyTreeList
		{
			get
			{
				return this._defendHeavenlyTreeDisplayData.HeavenlyTreeList;
			}
		}

		// Token: 0x17000D9F RID: 3487
		// (get) Token: 0x06007B6D RID: 31597 RVA: 0x0039543F File Offset: 0x0039363F
		private Dictionary<int, CharacterDisplayData> CharacterDisplayDataDict
		{
			get
			{
				return this._defendHeavenlyTreeDisplayData.AllCharacterDisplayDataDict;
			}
		}

		// Token: 0x17000DA0 RID: 3488
		// (get) Token: 0x06007B6E RID: 31598 RVA: 0x0039544C File Offset: 0x0039364C
		private SectStoryHeavenlyTreeExtendable CurTreeData
		{
			get
			{
				return this.HeavenlyTreeList[this._curTreeIndex];
			}
		}

		// Token: 0x17000DA1 RID: 3489
		// (get) Token: 0x06007B6F RID: 31599 RVA: 0x0039545F File Offset: 0x0039365F
		private WorldMapModel MapModel
		{
			get
			{
				return SingletonObject.getInstance<WorldMapModel>();
			}
		}

		// Token: 0x17000DA2 RID: 3490
		// (get) Token: 0x06007B70 RID: 31600 RVA: 0x00395466 File Offset: 0x00393666
		private int VisibleBlockDistance
		{
			get
			{
				return 3;
			}
		}

		// Token: 0x06007B71 RID: 31601 RVA: 0x0039546C File Offset: 0x0039366C
		public override void OnInit(ArgumentBox argsBox)
		{
			this._viewTreeLocationList.Clear();
			this._curTreeLocation = (this._originTreeLocation = Location.Invalid);
			this._includeGrownTree = false;
			argsBox.Get<Location>("Location", out this._originTreeLocation);
			argsBox.Get("IncludeGrownTree", out this._includeGrownTree);
			this._curTreeLocation = this._originTreeLocation;
			this._heavenlyTreeBlockDict.Clear();
			this._heavenlyTreeVisibleBlockDict.Clear();
			this.treeView.gameObject.SetActive(false);
			this.operationMask.SetActive(true);
			this.effectClearEnemyMap.Clear();
			this._awaitClearEnemyGrowEffectAfterDataRefresh = false;
			this._deferPlayClearEnemyTreeUpgradeEffect = false;
			this.NeedDataListenerId = true;
			UIElement element = this.Element;
			element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(this.CallRefresh));
		}

		// Token: 0x06007B72 RID: 31602 RVA: 0x00395550 File Offset: 0x00393750
		private void Awake()
		{
			this.villagerScrollView.OnItemRender += this.OnVillagerItemRender;
			this.treeScrollView.OnItemRender += this.OnTreeRender;
			this.blockToggleGroup.OnActiveIndexChange += this.OnBlockToggleChange;
		}

		// Token: 0x06007B73 RID: 31603 RVA: 0x003955A6 File Offset: 0x003937A6
		private void OnDisable()
		{
			TaiwuEventDomainMethod.Call.TriggerListener("DefendHeavenlyTree", true);
			this._awaitClearEnemyGrowEffectAfterDataRefresh = false;
			this._deferPlayClearEnemyTreeUpgradeEffect = false;
		}

		// Token: 0x06007B74 RID: 31604 RVA: 0x003955C3 File Offset: 0x003937C3
		private void CallRefresh()
		{
			StoryDomainMethod.Call.GetDefendHeavenlyTreeDisplayData(this.Element.GameDataListenerId, this._includeGrownTree, this._originTreeLocation, this._viewTreeLocationList);
		}

		// Token: 0x06007B75 RID: 31605 RVA: 0x003955EC File Offset: 0x003937EC
		public override void OnNotifyGameData(List<NotificationWrapper> notifications)
		{
			foreach (NotificationWrapper wrapper in notifications)
			{
				Notification notification = wrapper.Notification;
				byte type = notification.Type;
				byte b = type;
				if (b == 1)
				{
					bool flag = notification.DomainId == 20;
					if (flag)
					{
						bool flag2 = notification.MethodId == 13;
						if (flag2)
						{
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._defendHeavenlyTreeDisplayData);
							this.Refresh();
						}
					}
				}
			}
		}

		// Token: 0x06007B76 RID: 31606 RVA: 0x00395698 File Offset: 0x00393898
		private void Refresh()
		{
			bool awaitClearEnemyGrowEffectAfterDataRefresh = this._awaitClearEnemyGrowEffectAfterDataRefresh;
			if (awaitClearEnemyGrowEffectAfterDataRefresh)
			{
				this._awaitClearEnemyGrowEffectAfterDataRefresh = false;
				List<SectStoryHeavenlyTreeExtendable> heavenlyTreeList = this._defendHeavenlyTreeDisplayData.HeavenlyTreeList;
				SectStoryHeavenlyTreeExtendable treeAfter = (heavenlyTreeList != null) ? heavenlyTreeList.FirstOrDefault((SectStoryHeavenlyTreeExtendable t) => t.Id == this._clearEnemyEffectTreeId) : null;
				bool flag = treeAfter != null && treeAfter.GrowPoint > this._growPointBeforeClearEnemy;
				if (flag)
				{
					this._deferPlayClearEnemyTreeUpgradeEffect = true;
				}
			}
			bool flag2 = this._viewTreeLocationList.Count == 0;
			if (flag2)
			{
				this._viewTreeLocationList.AddRange(from t in this._defendHeavenlyTreeDisplayData.HeavenlyTreeList
				select t.Location);
			}
			this.treeScrollView.SetDataCount(this._defendHeavenlyTreeDisplayData.HeavenlyTreeList.Count);
			bool flag3 = this._defendHeavenlyTreeDisplayData.HeavenlyTreeList.Count > 0;
			if (flag3)
			{
				bool flag4 = this._curTreeLocation == Location.Invalid;
				if (flag4)
				{
					this._curTreeIndex = 0;
					this._curTreeLocation = this.CurTreeData.Location;
				}
				else
				{
					this._curTreeIndex = this.HeavenlyTreeList.FindIndex((SectStoryHeavenlyTreeExtendable t) => t.Location == this._curTreeLocation);
				}
				this.OnClickTree(this._curTreeIndex);
			}
			else
			{
				this.Element.ShowAfterRefresh();
				this.operationMask.SetActive(false);
			}
			SingletonObject.getInstance<YieldHelper>().DelayFrameDo(4U, delegate
			{
				int enemyCount = this.GetTreeEnemyCount(this._curTreeIndex);
				int villagerCount = this.GetTreeVillagerCount(this._curTreeIndex);
				ArgumentBox args = EasyPool.Get<ArgumentBox>().SetObject("displayData", this._defendHeavenlyTreeDisplayData).Set("enemyCount", enemyCount).Set("villagerCount", villagerCount);
				GEvent.OnEvent(UiEvents.ViewDefendHeavenlyTreeRefresh, args);
			});
		}

		// Token: 0x06007B77 RID: 31607 RVA: 0x00395814 File Offset: 0x00393A14
		private void OnTreeRender(int index, GameObject obj)
		{
			SectStoryHeavenlyTreeExtendable treeData = this.HeavenlyTreeList[index];
			DefendHeavenlyTreeItem treeItem = obj.GetComponent<DefendHeavenlyTreeItem>();
			treeItem.Button.ClearAndAddListener(delegate
			{
				this.OnClickTree(index);
			});
			string charName = GameData.Domains.Extra.SharedMethods.GetTreeName((int)treeData.TemplateId);
			int enemyCount = this.GetTreeEnemyCount(index);
			bool isSelected = this._curTreeIndex == index;
			treeItem.SetData(charName, (int)treeData.GrowPoint, enemyCount, isSelected);
		}

		// Token: 0x06007B78 RID: 31608 RVA: 0x003958A4 File Offset: 0x00393AA4
		private void OnClickTree(int index)
		{
			this._curTreeIndex = index;
			this._curTreeId = this.CurTreeData.Id;
			this._curTreeLocation = this.CurTreeData.Location;
			List<MapBlockData> list;
			bool flag = !this._heavenlyTreeBlockDict.TryGetValue(this._curTreeId, out list);
			if (flag)
			{
				int step = this.VisibleBlockDistance + 3;
				ExtraDomainMethod.AsyncCall.GetHeavenlyTreeNearBlocks(this, this._curTreeId, step, delegate(int offset, RawDataPool pool)
				{
					List<MapBlockData> blockList;
					bool flag2 = !this._heavenlyTreeBlockDict.TryGetValue(this._curTreeId, out blockList);
					if (flag2)
					{
						blockList = new List<MapBlockData>();
					}
					Serializer.Deserialize(pool, offset, ref blockList);
					this._heavenlyTreeBlockDict[this._curTreeId] = blockList;
					SectStoryHeavenlyTreeExtendable tree = this.CurTreeData;
					MapBlockData centerBlock = blockList.Find((MapBlockData b) => b.GetLocation() == tree.Location);
					ByteCoordinate centerCoordinate = centerBlock.GetBlockPos();
					List<MapBlockData> visibleBlockList = (from b in blockList
					where (int)b.GetManhattanDistanceToPosWithoutRoot(centerCoordinate.X, centerCoordinate.Y) <= this.VisibleBlockDistance
					select b).ToList<MapBlockData>();
					this._heavenlyTreeVisibleBlockDict[this._curTreeId] = visibleBlockList;
					bool flag3 = blockList.All((MapBlockData b) => this.MapModel.ExtraRequestedBlockData.ContainsKey(b.GetLocation()));
					if (flag3)
					{
						this.RefreshMapView();
					}
					else
					{
						List<Location> locations = (from b in blockList
						select b.GetLocation()).ToList<Location>();
						this.MapModel.RequestExtraMapBlockData(locations, true, true);
						GEvent.AddOneShot(UiEvents.OnExtraMapBlockDataRequested, delegate(ArgumentBox _)
						{
							this.RefreshMapView();
						});
					}
				});
			}
			else
			{
				this.RefreshMapView();
			}
		}

		// Token: 0x06007B79 RID: 31609 RVA: 0x00395928 File Offset: 0x00393B28
		private void RefreshCurTree()
		{
			int enemyCount = this.GetTreeEnemyCount(this._curTreeIndex);
			int villagerCount = this.GetTreeVillagerCount(this._curTreeIndex);
			this.treeView.SetData(this.CurTreeData, enemyCount, villagerCount);
			this.treeView.gameObject.SetActive(true);
			this.RefreshClickButtonFeed();
			bool flag = this._deferPlayClearEnemyTreeUpgradeEffect && this.CurTreeData.Id == this._clearEnemyEffectTreeId;
			if (flag)
			{
				this._deferPlayClearEnemyTreeUpgradeEffect = false;
				this.treeView.PlayEffectUpgrade();
			}
		}

		// Token: 0x06007B7A RID: 31610 RVA: 0x003959B4 File Offset: 0x00393BB4
		private void RefreshMapView()
		{
			this.treeScrollView.ReRender();
			List<MapBlockData> blockList;
			bool flag = !this._heavenlyTreeBlockDict.TryGetValue(this._curTreeId, out blockList);
			if (!flag)
			{
				List<MapBlockData> visibleBlockList;
				bool flag2 = !this._heavenlyTreeVisibleBlockDict.TryGetValue(this._curTreeId, out visibleBlockList);
				if (!flag2)
				{
					this.RefreshCurTree();
					SectStoryHeavenlyTreeExtendable tree = this.CurTreeData;
					MapBlockData centerBlock = blockList.Find((MapBlockData b) => b.GetLocation() == tree.Location);
					ByteCoordinate centerCoordinate = centerBlock.GetBlockPos();
					this.RefreshVillagerScrollView();
					this.blockToggleGroup.Clear();
					this.treeBlockTemplate.gameObject.SetActive(false);
					RectTransform rectTrans = this.treeBlockTemplate.GetComponent<RectTransform>();
					Vector2 distance = rectTrans.rect.size * rectTrans.localScale * 0.5f;
					for (int i = 0; i < blockList.Count; i++)
					{
						MapBlockData blockData = blockList[i];
						bool isCreate = false;
						bool flag3 = i >= this.mapLayer.childCount;
						DefendHeavenlyTreeBlockView treeBlockView;
						if (flag3)
						{
							treeBlockView = Object.Instantiate<DefendHeavenlyTreeBlockView>(this.treeBlockTemplate, this.mapLayer);
							isCreate = true;
						}
						else
						{
							treeBlockView = this.mapLayer.GetChild(i).GetComponent<DefendHeavenlyTreeBlockView>();
						}
						int visibleIndex = visibleBlockList.IndexOf(blockData);
						bool visible = visibleIndex >= 0;
						treeBlockView.gameObject.SetActive(visible);
						bool flag4 = visible;
						if (flag4)
						{
							treeBlockView.Toggle.isOn = false;
							this.blockToggleGroup.Add(treeBlockView.Toggle);
						}
						ByteCoordinate coordinate = blockData.GetBlockPos();
						int offsetX = (int)(coordinate.X - centerCoordinate.X);
						int offsetY = (int)(coordinate.Y - centerCoordinate.Y);
						int renderX = offsetX + offsetY;
						int renderY = offsetY - offsetX;
						float posX = (float)renderX * distance.x;
						float posY = (float)renderY * distance.y;
						Vector2 pos = new Vector2(posX, posY);
						Location location = blockData.GetLocation();
						bool isTree = tree.Location == location;
						int enemyCount = this.GetBlockEnemyCount(blockData);
						int villagerCount = this.GetBlockVillagerCount(blockData);
						treeBlockView.SetData(blockData, isTree, isCreate, visibleIndex, pos, enemyCount, villagerCount, this.mapLayer, this.pathLayer, this.infoLayer, this.characterLayer);
					}
					for (int j = 0; j < blockList.Count; j++)
					{
						MapBlockData curBlockData = blockList[j];
						bool flag5 = !visibleBlockList.Contains(curBlockData);
						if (!flag5)
						{
							DefendHeavenlyTreeBlockView blockView = this.mapLayer.GetChild(j).GetComponent<DefendHeavenlyTreeBlockView>();
							int enemyCount2 = this.GetBlockEnemyCount(curBlockData);
							bool showLine = enemyCount2 > 0;
							blockView.RefreshLine(showLine, tree.Location, blockList, this.pathLayer, this.mapLayer);
						}
					}
					for (int k = blockList.Count; k < this.mapLayer.childCount; k++)
					{
						Transform child = this.mapLayer.GetChild(k);
						DefendHeavenlyTreeBlockView blockView2 = child.GetComponent<DefendHeavenlyTreeBlockView>();
						blockView2.Clear();
					}
					bool flag6 = this._curVisibleBlockIndex < 0;
					if (flag6)
					{
						this._curVisibleBlockIndex = visibleBlockList.FindIndex((MapBlockData b) => b.GetLocation() == tree.Location);
					}
					this.blockToggleGroup.Init(this._curVisibleBlockIndex);
					this.RefreshButtonClearEnemy();
					this.Element.ShowAfterRefresh();
					this.operationMask.SetActive(false);
				}
			}
		}

		// Token: 0x06007B7B RID: 31611 RVA: 0x00395D45 File Offset: 0x00393F45
		private void OnBlockToggleChange(int newTog, int oldTog)
		{
			this._curVisibleBlockIndex = newTog;
			this.villagerScrollView.Refresh(this._curVisibleBlockIndex);
			this.villagerScrollView.ReRender();
		}

		// Token: 0x06007B7C RID: 31612 RVA: 0x00395D70 File Offset: 0x00393F70
		private void RefreshVillagerScrollView()
		{
			List<MapBlockData> visibleBlockList;
			bool flag = !this._heavenlyTreeVisibleBlockDict.TryGetValue(this._curTreeId, out visibleBlockList);
			if (!flag)
			{
				this.villagerScrollView.SetDataCount(visibleBlockList.Count);
			}
		}

		// Token: 0x06007B7D RID: 31613 RVA: 0x00395DAC File Offset: 0x00393FAC
		private void OnVillagerItemRender(int index, GameObject obj)
		{
			ViewDefendHeavenlyTree.<>c__DisplayClass56_0 CS$<>8__locals1 = new ViewDefendHeavenlyTree.<>c__DisplayClass56_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.index = index;
			List<MapBlockData> visibleBlockList;
			bool flag = !this._heavenlyTreeVisibleBlockDict.TryGetValue(this._curTreeId, out visibleBlockList);
			if (!flag)
			{
				CS$<>8__locals1.blockData = visibleBlockList[CS$<>8__locals1.index];
				DefendHeavenlyTreeVillagerItem defendHeavenlyBlockItem = obj.GetComponent<DefendHeavenlyTreeVillagerItem>();
				BuildingModel buildingModel = SingletonObject.getInstance<BuildingModel>();
				KeyValuePair<int, VillagerWorkData> pair2 = buildingModel.VillagerWork.FirstOrDefault((KeyValuePair<int, VillagerWorkData> pair) => pair.Value.AreaId == CS$<>8__locals1.blockData.AreaId && pair.Value.BlockId == CS$<>8__locals1.blockData.BlockId && VillagerWorkType.IsWorkOnMap(pair.Value.WorkType));
				int charId = (pair2.Value != null) ? pair2.Key : -1;
				CharacterDisplayData charData = this.CharacterDisplayDataDict.GetValueOrDefault(charId);
				int enemyCount = this.GetBlockEnemyCount(CS$<>8__locals1.blockData);
				int villagerCount = this.GetBlockVillagerCount(CS$<>8__locals1.blockData);
				bool isSelected = CS$<>8__locals1.index == this._curVisibleBlockIndex;
				defendHeavenlyBlockItem.SetData(CS$<>8__locals1.blockData, charData, isSelected, enemyCount, villagerCount, new Action<int>(CS$<>8__locals1.<OnVillagerItemRender>g__OnSelectChar|2), new Action(CS$<>8__locals1.<OnVillagerItemRender>g__OnCancelChar|3));
				defendHeavenlyBlockItem.Button.ClearAndAddListener(delegate
				{
					CS$<>8__locals1.<>4__this.blockToggleGroup.Set(CS$<>8__locals1.index, false);
				});
			}
		}

		// Token: 0x06007B7E RID: 31614 RVA: 0x00395EC0 File Offset: 0x003940C0
		private void OpenSelectWindow()
		{
			List<int> selectedCharacterIds = new List<int>();
			bool flag = this._exchangingWorkVillagerId > 0;
			if (flag)
			{
				selectedCharacterIds.Add(this._exchangingWorkVillagerId);
			}
			VillagerSelectCharacterSelectionHelper.OpenDefendTreeSelectChar(this._defendHeavenlyTreeDisplayData.WorkAvailableVillagerList, selectedCharacterIds, new SelectCharacterCallback(this.OnSelectWorkingVillageChar), ESelectCharacterInteractionMode.Instant, ESelectCharacterSelectionMode.Single, 1, null, null, new Func<IEnumerable<CharacterDisplayDataForGeneralScrollList>, string>(this.GenerateSendVillagerText));
			this._exchangingWorkVillagerId = -1;
		}

		// Token: 0x06007B7F RID: 31615 RVA: 0x00395F24 File Offset: 0x00394124
		private string GenerateSendVillagerText(IEnumerable<CharacterDisplayDataForGeneralScrollList> dataList)
		{
			CharacterDisplayDataForGeneralScrollList charData = dataList.FirstOrDefault<CharacterDisplayDataForGeneralScrollList>();
			bool flag = charData == null;
			string result;
			if (flag)
			{
				result = string.Empty;
			}
			else
			{
				EHealthType healthType = CommonUtils.GetHealthType(charData.Health, charData.MaxLeftHealth, charData.CharacterId);
				string healthStr = CommonUtils.GetHealthString(healthType);
				result = LocalStringManager.GetFormat(LanguageKey.LK_Wudang_Select_CharacterHealth, healthStr);
			}
			return result;
		}

		// Token: 0x06007B80 RID: 31616 RVA: 0x00395F7C File Offset: 0x0039417C
		private void OnSelectWorkingVillageChar(List<int> selectedCharacterIds)
		{
			ViewDefendHeavenlyTree.<>c__DisplayClass59_0 CS$<>8__locals1 = new ViewDefendHeavenlyTree.<>c__DisplayClass59_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.charId = ((selectedCharacterIds != null) ? selectedCharacterIds.First<int>() : -1);
			CS$<>8__locals1.location = this._selectingWorkLocation;
			bool flag = CS$<>8__locals1.charId < 0;
			if (flag)
			{
				SingletonObject.getInstance<WorldMapModel>().RequestStopVillagerWorkOnMap(CS$<>8__locals1.location, true);
				SingletonObject.getInstance<YieldHelper>().DelayFrameDo(4U, new Action(this.CallRefresh));
			}
			else
			{
				BuildingModel buildingModel = SingletonObject.getInstance<BuildingModel>();
				bool marked = buildingModel.CheckBlockIsMarked(CS$<>8__locals1.location);
				DialogCmd cmd = new DialogCmd
				{
					Title = LocalStringManager.Get(LanguageKey.LK_DefendHeavenlyTree_View),
					Content = LocalStringManager.GetFormat(marked ? LanguageKey.LK_Villager_Collect_Resource_Confirm : LanguageKey.LK_Villager_Collect_Resource_And_Mark_Confirm, NameCenter.GetMonasticTitleOrDisplayName(this.CharacterDisplayDataDict[CS$<>8__locals1.charId], false)).ColorReplace(),
					Type = 1,
					Yes = new Action(CS$<>8__locals1.<OnSelectWorkingVillageChar>g__ConfirmAction|0),
					No = null
				};
				UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
				UIManager.Instance.MaskUI(UIElement.Dialog);
			}
		}

		// Token: 0x06007B81 RID: 31617 RVA: 0x003960A0 File Offset: 0x003942A0
		private int GetBlockEnemyCount(MapBlockData blockData)
		{
			List<MapTemplateEnemyInfo> templateEnemyList = blockData.TemplateEnemyList;
			return ((templateEnemyList != null) ? new int?(templateEnemyList.Count((MapTemplateEnemyInfo e) => e.TemplateId >= 366 && e.TemplateId <= 374 && this.HeavenlyTreeList.Any((SectStoryHeavenlyTreeExtendable t) => t.Location == new Location(blockData.AreaId, e.SourceAdventureBlockId)))) : null).GetValueOrDefault();
		}

		// Token: 0x06007B82 RID: 31618 RVA: 0x00396100 File Offset: 0x00394300
		private int GetTreeEnemyCount(int treeIndex)
		{
			SectStoryHeavenlyTreeExtendable tree = this.HeavenlyTreeList[treeIndex];
			List<MapBlockData> blockList;
			bool flag = !this._heavenlyTreeVisibleBlockDict.TryGetValue(tree.Id, out blockList);
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				int count = 0;
				foreach (MapBlockData blockData in blockList)
				{
					bool flag2 = blockData.TemplateEnemyList == null;
					if (!flag2)
					{
						foreach (MapTemplateEnemyInfo mapTemplateEnemyInfo in blockData.TemplateEnemyList)
						{
							short templateId = mapTemplateEnemyInfo.TemplateId;
							bool flag3 = templateId < 366 || templateId > 374;
							if (!flag3)
							{
								Location targetLocation = new Location(blockData.AreaId, mapTemplateEnemyInfo.SourceAdventureBlockId);
								bool flag4 = tree.Location == targetLocation;
								if (flag4)
								{
									count++;
								}
							}
						}
					}
				}
				result = count;
			}
			return result;
		}

		// Token: 0x06007B83 RID: 31619 RVA: 0x0039623C File Offset: 0x0039443C
		private int GetBlockVillagerCount(MapBlockData blockData)
		{
			VillagerWorkData workData = SingletonObject.getInstance<BuildingModel>().VillagerWork.FirstOrDefault((KeyValuePair<int, VillagerWorkData> p) => p.Value.Location == blockData.GetLocation() && VillagerWorkType.IsWorkOnMap(p.Value.WorkType)).Value;
			bool flag = workData != null;
			int result;
			if (flag)
			{
				result = 1;
			}
			else
			{
				result = 0;
			}
			return result;
		}

		// Token: 0x06007B84 RID: 31620 RVA: 0x00396290 File Offset: 0x00394490
		private int GetTreeVillagerCount(int treeIndex)
		{
			SectStoryHeavenlyTreeExtendable tree = this.HeavenlyTreeList[treeIndex];
			List<MapBlockData> blockList;
			bool flag = !this._heavenlyTreeVisibleBlockDict.TryGetValue(tree.Id, out blockList);
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				int count = 0;
				foreach (MapBlockData mapBlockData in blockList)
				{
					count += this.GetBlockVillagerCount(mapBlockData);
				}
				result = count;
			}
			return result;
		}

		// Token: 0x06007B85 RID: 31621 RVA: 0x0039631C File Offset: 0x0039451C
		protected override void OnClick(Transform btn)
		{
			string name = btn.name;
			string a = name;
			if (!(a == "ButtonCloseView"))
			{
				if (!(a == "ButtonClearEnemy"))
				{
					if (a == "ButtonFeed")
					{
						this.OnClickButtonFeed();
					}
				}
				else
				{
					this.OnClickButtonClearEnemy();
				}
			}
			else
			{
				this.QuickHide();
			}
		}

		// Token: 0x06007B86 RID: 31622 RVA: 0x00396378 File Offset: 0x00394578
		private void RefreshClickButtonFeed()
		{
			this.buttonFeed.interactable = !this.CurTreeData.IsGrowPointMax;
			this.tipButtonFeed.enabled = !this.buttonFeed.interactable;
			bool isGrowPointMax = this.CurTreeData.IsGrowPointMax;
			if (isGrowPointMax)
			{
				this.tipButtonFeed.PresetParam = new string[]
				{
					LanguageKey.LK_DefendHeavenlyTree_Feed_IsMax.Tr().SetColor("brightred")
				};
			}
		}

		// Token: 0x06007B87 RID: 31623 RVA: 0x003963F4 File Offset: 0x003945F4
		private void OnClickButtonFeed()
		{
			int enemyCount = this.GetTreeEnemyCount(this._curTreeIndex);
			int villagerCount = this.GetTreeVillagerCount(this._curTreeIndex);
			ArgumentBox args = EasyPool.Get<ArgumentBox>().Set<Location>("treeLocation", this.CurTreeData.Location).Set("enemyCount", enemyCount).Set("villagerCount", villagerCount).SetObject("displayData", this._defendHeavenlyTreeDisplayData).SetObject("onConfirm", new Action(this.CallRefresh));
			UIElement.DefendHeavenlyTreeFeed.SetOnInitArgs(args);
			UIManager.Instance.MaskUI(UIElement.DefendHeavenlyTreeFeed);
		}

		// Token: 0x06007B88 RID: 31624 RVA: 0x00396490 File Offset: 0x00394690
		private void OnClickButtonClearEnemy()
		{
			HashSet<int> bannedCharIdSet = this._defendHeavenlyTreeDisplayData.TreeClearEnemyAvailableVillagerList.Where(delegate(int id)
			{
				CharacterDisplayData charData = this.CharacterDisplayDataDict[id];
				EHealthType healthType = CommonUtils.GetHealthType(charData.Health, charData.LeftMaxHealth, id);
				return healthType <= EHealthType.CriticallyIll;
			}).ToHashSet<int>();
			VillagerSelectCharacterSelectionHelper.OpenDefendTreeSelectChar(this._defendHeavenlyTreeDisplayData.TreeClearEnemyAvailableVillagerList, null, new SelectCharacterCallback(this.OnSelectClearEnemyChar), ESelectCharacterInteractionMode.Instant, ESelectCharacterSelectionMode.Single, 1, null, bannedCharIdSet, new Func<IEnumerable<CharacterDisplayDataForGeneralScrollList>, string>(this.GenerateCharHealthAndAgeText));
		}

		// Token: 0x06007B89 RID: 31625 RVA: 0x003964F0 File Offset: 0x003946F0
		private string GenerateCharHealthAndAgeText(IEnumerable<CharacterDisplayDataForGeneralScrollList> dataList)
		{
			CharacterDisplayDataForGeneralScrollList charData = dataList.FirstOrDefault<CharacterDisplayDataForGeneralScrollList>();
			bool flag = charData == null;
			string result;
			if (flag)
			{
				result = string.Empty;
			}
			else
			{
				EHealthType healthType = CommonUtils.GetHealthType(charData.Health, charData.MaxLeftHealth, charData.CharacterId);
				string healthStr = CommonUtils.GetHealthString(healthType);
				string healthText = LocalStringManager.GetFormat(LanguageKey.LK_Wudang_Select_CharacterHealth, healthStr);
				string ageText = LocalStringManager.GetFormat(LanguageKey.LK_Wudang_Select_CharacterAge, (int)(charData.CurrAge + 3));
				this._sb.Clear();
				this._sb.Append(ageText);
				this._sb.Append("   ");
				this._sb.Append(healthText);
				result = this._sb.ToString();
			}
			return result;
		}

		// Token: 0x06007B8A RID: 31626 RVA: 0x003965A8 File Offset: 0x003947A8
		private void OnSelectClearEnemyChar(List<int> selectedCharacterIds)
		{
			int charId = selectedCharacterIds.First<int>();
			string title = LanguageKey.LK_DefendHeavenlyTree_ClearEnmey.Tr();
			CharacterDisplayData charData = this.CharacterDisplayDataDict[charId];
			bool isTaiwu = SingletonObject.getInstance<BasicGameData>().TaiwuCharId == charId;
			string charName = NameCenter.GetMonasticTitleOrDisplayName(charData, isTaiwu);
			string content = LanguageKey.LK_DefendHeavenlyTree_ClearEnmey_Confirm.TrFormat(charName);
			CommonUtils.ShowConfirmDialog(title, content, delegate
			{
				this.OnConfirmClearEnemyChar(charId);
			}, null, EDialogType.None);
		}

		// Token: 0x06007B8B RID: 31627 RVA: 0x00396630 File Offset: 0x00394830
		private void OnConfirmClearEnemyChar(int charId)
		{
			this.operationMask.SetActive(true);
			AsyncMethodCallbackDelegate <>9__1;
			SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(0.5f, delegate
			{
				this._clearEnemyEffectTreeId = this._curTreeId;
				this._growPointBeforeClearEnemy = this.CurTreeData.GrowPoint;
				IAsyncMethodRequestHandler <>4__this = this;
				int curTreeId = this._curTreeId;
				int charId2 = charId;
				AsyncMethodCallbackDelegate callback;
				if ((callback = <>9__1) == null)
				{
					callback = (<>9__1 = delegate(int offset, RawDataPool pool)
					{
						SectWudangDefendHeavenlyTreeDisplayData tuple = new ValueTuple<List<Location>, bool>(new List<Location>(), false);
						Serializer.Deserialize(pool, offset, ref tuple);
						List<MapBlockData> blockList = this._heavenlyTreeVisibleBlockDict[this._curTreeId];
						using (List<Location>.Enumerator enumerator = tuple.Locations.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								Location location = enumerator.Current;
								int index = blockList.FindIndex((MapBlockData b) => b.GetLocation() == location);
								CToggle toggle = this.blockToggleGroup.Get(index);
								this.effectPlayer.PlayEffectAt(toggle.transform, this.EffectClearEnemy, 1f, false);
							}
						}
						this.effectClearEnemyMap.Play();
						this._heavenlyTreeBlockDict.Clear();
						this._heavenlyTreeVisibleBlockDict.Clear();
						this._awaitClearEnemyGrowEffectAfterDataRefresh = true;
						this.CallRefresh();
					});
				}
				StoryDomainMethod.AsyncCall.DefendHeavenlyTreeClearEnemy(<>4__this, curTreeId, charId2, callback);
			});
		}

		// Token: 0x06007B8C RID: 31628 RVA: 0x0039667C File Offset: 0x0039487C
		private void RefreshButtonClearEnemy()
		{
			SectStoryHeavenlyTreeExtendable tree = this.CurTreeData;
			bool isTreeHasEnemy = this.GetTreeEnemyCount(this._curTreeIndex) > 0;
			this.buttonClearEnemy.gameObject.SetActive(!tree.IsGrowPointMax);
			this.buttonClearEnemy.interactable = isTreeHasEnemy;
		}

		// Token: 0x04005DCC RID: 24012
		private DefendHeavenlyTreeDisplayData _defendHeavenlyTreeDisplayData;

		// Token: 0x04005DCD RID: 24013
		private readonly Dictionary<int, List<MapBlockData>> _heavenlyTreeBlockDict = new Dictionary<int, List<MapBlockData>>();

		// Token: 0x04005DCE RID: 24014
		private readonly Dictionary<int, List<MapBlockData>> _heavenlyTreeVisibleBlockDict = new Dictionary<int, List<MapBlockData>>();

		// Token: 0x04005DCF RID: 24015
		private Location _originTreeLocation;

		// Token: 0x04005DD0 RID: 24016
		private int _curTreeId;

		// Token: 0x04005DD1 RID: 24017
		private Location _curTreeLocation;

		// Token: 0x04005DD2 RID: 24018
		private int _curTreeIndex;

		// Token: 0x04005DD3 RID: 24019
		private int _curVisibleBlockIndex = -1;

		// Token: 0x04005DD4 RID: 24020
		private int _exchangingWorkVillagerId;

		// Token: 0x04005DD5 RID: 24021
		private Location _selectingWorkLocation;

		// Token: 0x04005DD6 RID: 24022
		private bool _includeGrownTree;

		// Token: 0x04005DD7 RID: 24023
		private readonly List<Location> _viewTreeLocationList = new List<Location>();

		// Token: 0x04005DD8 RID: 24024
		private ushort _growPointBeforeClearEnemy;

		// Token: 0x04005DD9 RID: 24025
		private int _clearEnemyEffectTreeId;

		// Token: 0x04005DDA RID: 24026
		private bool _awaitClearEnemyGrowEffectAfterDataRefresh;

		// Token: 0x04005DDB RID: 24027
		private bool _deferPlayClearEnemyTreeUpgradeEffect;

		// Token: 0x04005DDC RID: 24028
		private StringBuilder _sb = new StringBuilder();

		// Token: 0x04005DDD RID: 24029
		[SerializeField]
		private InfinityScroll villagerScrollView;

		// Token: 0x04005DDE RID: 24030
		[SerializeField]
		private InfinityScroll treeScrollView;

		// Token: 0x04005DDF RID: 24031
		[SerializeField]
		private CToggleGroup blockToggleGroup;

		// Token: 0x04005DE0 RID: 24032
		[SerializeField]
		private RectTransform mapLayer;

		// Token: 0x04005DE1 RID: 24033
		[SerializeField]
		private RectTransform pathLayer;

		// Token: 0x04005DE2 RID: 24034
		[SerializeField]
		private RectTransform infoLayer;

		// Token: 0x04005DE3 RID: 24035
		[SerializeField]
		private RectTransform characterLayer;

		// Token: 0x04005DE4 RID: 24036
		[SerializeField]
		private DefendHeavenlyTreeBlockView treeBlockTemplate;

		// Token: 0x04005DE5 RID: 24037
		[SerializeField]
		private CButton buttonClearEnemy;

		// Token: 0x04005DE6 RID: 24038
		[SerializeField]
		private TooltipInvoker tipButtonClearEnemy;

		// Token: 0x04005DE7 RID: 24039
		[SerializeField]
		private GameObject operationMask;

		// Token: 0x04005DE8 RID: 24040
		[SerializeField]
		private DefendHeavenlyTreeView treeView;

		// Token: 0x04005DE9 RID: 24041
		[SerializeField]
		private CButton buttonFeed;

		// Token: 0x04005DEA RID: 24042
		[SerializeField]
		private TooltipInvoker tipButtonFeed;

		// Token: 0x04005DEB RID: 24043
		[SerializeField]
		private EffectPlayer effectPlayer;

		// Token: 0x04005DEC RID: 24044
		[SerializeField]
		private UIParticle effectClearEnemyMap;

		// Token: 0x04005DED RID: 24045
		private readonly string EffectClearEnemy = "EffectClearEnemy";
	}
}
