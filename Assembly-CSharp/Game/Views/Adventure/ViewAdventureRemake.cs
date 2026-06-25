using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Coffee.UIExtensions;
using Config;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using Game.Components.Adventure;
using Game.HotCommand;
using Game.Views.Map;
using Game.Views.Migrate;
using GameData.Adventure;
using GameData.Domains.Adventure;
using GameData.Domains.Global;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Map;
using GameData.Domains.TaiwuEvent;
using GameData.Domains.World;
using GameData.Serializer;
using GameData.Utilities;
using Google.Protobuf.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.Views.Adventure
{
	// Token: 0x02000C7C RID: 3196
	public class ViewAdventureRemake : UIBase, IPreloadElement
	{
		// Token: 0x0600A247 RID: 41543 RVA: 0x004BD978 File Offset: 0x004BBB78
		private void InitMapValidArea()
		{
			bool flag = this._mapSize % 2 == 0;
			if (flag)
			{
				throw new ArgumentException("最长对角线必须是奇数");
			}
			this._mapIsInnerArray = new bool[(int)this._mapSize, (int)this._mapSize];
			int center = (int)this.CenterCoord;
			for (int i = 0; i < (int)this._mapSize; i++)
			{
				for (int j = 0; j < (int)this._mapSize; j++)
				{
					int distance = Math.Abs(i - center) + Math.Abs(j - center);
					this._mapIsInnerArray[i, j] = (distance <= this._adventureSize);
				}
			}
		}

		// Token: 0x0600A248 RID: 41544 RVA: 0x004BDA20 File Offset: 0x004BBC20
		private bool IsRenderIndexInner(int x, int y)
		{
			return this._mapIsInnerArray[x, y];
		}

		// Token: 0x0600A249 RID: 41545 RVA: 0x004BDA40 File Offset: 0x004BBC40
		private bool IsRenderIndexExist(byte x, byte y)
		{
			return this._renderCoordList.Contains(new ByteCoordinate(x, y));
		}

		// Token: 0x0600A24A RID: 41546 RVA: 0x004BDA64 File Offset: 0x004BBC64
		private void InitTaiwuLocation()
		{
			this.TaiwuRenderBlockIndex = this.DataIndexToRenderIndex(this.AdventureTaiwu.Index);
			this.UpdateTaiwuRenderLocationAndIcon(this.TaiwuRenderBlockIndex);
			this.taiwuIconRoot.gameObject.SetActive(false);
			WorldMapModel worldMapModel = SingletonObject.getInstance<WorldMapModel>();
			string spriteName = "adventure_element_character_protagonist_" + worldMapModel.TaiwuGender.ToString();
			this.taiwuIconRoot.taiwuIcon.SetSprite(spriteName, false, null);
		}

		// Token: 0x17001101 RID: 4353
		// (get) Token: 0x0600A24B RID: 41547 RVA: 0x004BDAD9 File Offset: 0x004BBCD9
		private AdventureRemakeMapBlockItem BlockConfig
		{
			get
			{
				return AdventureRemakeMapBlock.Instance[this._adventureEnvironmentTemplateId];
			}
		}

		// Token: 0x17001102 RID: 4354
		// (get) Token: 0x0600A24C RID: 41548 RVA: 0x004BDAEB File Offset: 0x004BBCEB
		private byte CenterCoord
		{
			get
			{
				return (byte)(this._adventureSize + this.OutCircleCount);
			}
		}

		// Token: 0x17001103 RID: 4355
		// (get) Token: 0x0600A24D RID: 41549 RVA: 0x004BDAFB File Offset: 0x004BBCFB
		private int OutCircleCount
		{
			get
			{
				return this.GetOutCircleCount(this._adventureSize, this.BlockConfig.CircleCount);
			}
		}

		// Token: 0x0600A24E RID: 41550 RVA: 0x004BDB14 File Offset: 0x004BBD14
		private int GetOutCircleCount(int size, int configCircleCount)
		{
			bool flag = size <= 4;
			int result;
			if (flag)
			{
				result = configCircleCount;
			}
			else
			{
				bool flag2 = size == 5;
				if (flag2)
				{
					result = Math.Min(4, configCircleCount);
				}
				else
				{
					result = Math.Min(3, configCircleCount);
				}
			}
			return result;
		}

		// Token: 0x0600A24F RID: 41551 RVA: 0x004BDB50 File Offset: 0x004BBD50
		public override void OnInit(ArgumentBox argsBox)
		{
			this.BanOperate(true);
			this.ClearMovingPawn();
			this.StopContinuousMove();
			bool flag = argsBox == null;
			if (flag)
			{
				this.Element.ShowAfterRefresh();
				this.BanOperate(false);
				this._cameraMoving = false;
			}
			else
			{
				this.PrepareOuterBlockPiece();
				argsBox.Get("AdventureId", out this._adventureId);
				this._adventureSize = this.AdventureRuntime.Size;
				MapBlockItem mapBlockConfig = MapBlock.Instance[this.WorldMapModel.PlayerAtBlock.TemplateId];
				this._adventureEnvironmentTemplateId = mapBlockConfig.AdventureEnvironment;
				this._showTransitionAnim = true;
				bool archiveEnter;
				bool flag2 = argsBox.Get("ArchiveEnter", out archiveEnter) && archiveEnter;
				if (flag2)
				{
					this._showTransitionAnim = false;
				}
				GEvent.OnEvent(UiEvents.AdventureRemakeEnter, null);
				this._mapSize = this.CenterCoord * 2 + 1;
				this._unitNormalRenderArray2D = new AdventureUnitNormal[(int)this._mapSize, (int)this._mapSize];
				this._unitPeripheralArray2D = new AdventureUnitPeripheral[(int)this._mapSize, (int)this._mapSize];
				this.InitRefers();
				this.ResetState();
				this.RefreshSpecial();
				this.InitMapValidArea();
				this.NeedDataListenerId = true;
				this.InitUnitFlatData();
				this.InitStartScale();
				this.ResetViewValue();
				this.RefreshViewValue();
				this.ApplyAdventureLightingConfig();
				this._random = new Random(this.AdventureRuntime.DisplayRandomSeed);
				YieldHelper yieldHelper = SingletonObject.getInstance<YieldHelper>();
				bool flag3 = this._routine != null;
				if (flag3)
				{
					yieldHelper.StopCoroutine(this._routine);
				}
				yieldHelper.StartCoroutine(this._routine = this.Routine());
				GlobalDomainMethod.Call.InvokeGuidingTrigger(139);
				GEvent.OnEvent(UiEvents.TaskGroupDataUpdated, null);
			}
		}

		// Token: 0x0600A250 RID: 41552 RVA: 0x004BDD14 File Offset: 0x004BBF14
		private void RefreshSpecial()
		{
			bool specialBottom = this.AdventureRemakeModel.SpecialBottomAdventure(this._adventureId);
			this.temporaryItemBtn.gameObject.SetActive(!specialBottom);
			this.advanceDaysBtn.gameObject.SetActive(!specialBottom);
		}

		// Token: 0x0600A251 RID: 41553 RVA: 0x004BDD5E File Offset: 0x004BBF5E
		private void BanOperate(bool ban)
		{
			CommandKitBase.SetDisable(ban);
			this.actionMask.gameObject.SetActive(ban);
		}

		// Token: 0x0600A252 RID: 41554 RVA: 0x004BDD7A File Offset: 0x004BBF7A
		private void InitRefers()
		{
			this.searchByName.onValueChanged.ResetListener(new Action<string>(this.OnSearchElementValueChange));
			this.filterClearButton.onClick.ResetListener(delegate()
			{
				this.searchByName.text = "";
			});
		}

		// Token: 0x0600A253 RID: 41555 RVA: 0x004BDDB8 File Offset: 0x004BBFB8
		private void AwakeInit()
		{
			this._elementPanel = this.elementScrollView;
			this._elementPanel.InitLoop(0, new Action<Transform, int>(this.OnRenderElementItem), null);
			this.globalParameterPanel = this.parameterScrollView;
			this.globalParameterPanel.OnItemRender += this.OnRenderGlobalParameterItem;
			this._temporaryItemVerticalScrollView = this.temporaryItemVerticalScrollView;
			this._temporaryItemVerticalScrollView.OnItemRender += this.TemporaryItemRender;
			this.temporaryItemBtn.onClick.ResetListener(delegate()
			{
				this.SwitchTemporaryItemLocation(!this._temporaryItemVerticalScrollView.gameObject.activeInHierarchy);
			});
			this.advanceDaysBtn.onClick.ResetListener(new Action(this.AdvanceDaysBtnClick));
			this.adventureAdvanceDays.gameObject.SetActive(false);
			this.advanceDaysTips.gameObject.SetActive(false);
			this._taiwuStateHolder = this.taiwuStateHolder;
			this._taiwuStateHolder.OnItemRender += this.OnRenderTaiwuStateLine;
			this.simpleViewModeToggle.onValueChanged.ResetListener(delegate(bool isOn)
			{
				this._simpleViewModeOn = isOn;
				this.SimpleViewModeSwitch();
			});
		}

		// Token: 0x0600A254 RID: 41556 RVA: 0x004BDED4 File Offset: 0x004BC0D4
		private void OnSearchElementValueChange(string text)
		{
			bool flag = this._displayItems == null || string.IsNullOrEmpty(text);
			if (flag)
			{
				this.RefreshElementPanel(this._needRenderDataIndex);
			}
			else
			{
				this._displayItems = this._displayItems.Where(delegate(ViewAdventureRemake.ElementDisplayItem item)
				{
					bool isExitItem = item.IsExitItem;
					bool result;
					if (isExitItem)
					{
						result = true;
					}
					else
					{
						AdventureElement element = item.Element;
						AdventureElementData elementData = AdventureRemakeModel.Core.GetAdventureElementData(element.CoreId);
						bool flag2 = elementData.Name.Contains(text) || elementData.Desc.Contains(text);
						if (flag2)
						{
							result = true;
						}
						else
						{
							foreach (AdventureParameterData parameter in element.Core.Parameters)
							{
								AdventureParameterValue value = element.GetParameter(parameter.Key);
								bool flag3 = parameter.Type == EAdventureParameterType.State;
								if (flag3)
								{
									bool flag4 = value.Current >= 0 && parameter.Name.Contains(text);
									if (flag4)
									{
										return true;
									}
								}
							}
							result = false;
						}
					}
					return result;
				}).ToList<ViewAdventureRemake.ElementDisplayItem>();
				this.RefreshElementPanelCount();
			}
		}

		// Token: 0x0600A255 RID: 41557 RVA: 0x004BDF42 File Offset: 0x004BC142
		private IEnumerator Routine()
		{
			this._renderCoordList = this.GenerateTraversalOrder((int)this._mapSize).ToList<ByteCoordinate>();
			this.FilterRenderCoordList(ref this._renderCoordList);
			DOTween.Kill(this.unitRoot, false);
			int num;
			for (int i = 0; i < (int)this._mapSize; i = num + 1)
			{
				for (int j = 0; j < (int)this._mapSize; j = num + 1)
				{
					this._unitNormalRenderArray2D[i, j] = null;
					this._unitPeripheralArray2D[i, j] = null;
					num = j;
				}
				num = i;
			}
			List<GameObject> pooledNormals = new List<GameObject>();
			List<GameObject> pooledPeripherals = new List<GameObject>();
			for (int k = this.unitRoot.childCount - 1; k >= 0; k = num - 1)
			{
				GameObject child = this.unitRoot.GetChild(k).gameObject;
				bool flag = child.GetComponent<AdventureUnitNormal>() != null;
				if (flag)
				{
					child.SetActive(false);
					pooledNormals.Add(child);
				}
				else
				{
					bool flag2 = child.GetComponent<AdventureUnitPeripheral>() != null;
					if (flag2)
					{
						child.SetActive(false);
						pooledPeripherals.Add(child);
					}
				}
				child = null;
				num = k;
			}
			for (int l = 0; l < this._renderCoordList.Count; l = num + 1)
			{
				ByteCoordinate renderCoord = this._renderCoordList[l];
				bool isInner = this.IsRenderIndexInner((int)renderCoord.X, (int)renderCoord.Y);
				bool flag3 = isInner;
				GameObject go;
				if (flag3)
				{
					ViewAdventureRemake.<>c__DisplayClass82_0 CS$<>8__locals1 = new ViewAdventureRemake.<>c__DisplayClass82_0();
					bool flag4 = pooledNormals.Count > 0;
					if (flag4)
					{
						List<GameObject> list = pooledNormals;
						go = list[list.Count - 1];
						pooledNormals.RemoveAt(pooledNormals.Count - 1);
						go.SetActive(true);
					}
					else
					{
						go = Object.Instantiate<GameObject>(this.innerBlockTemplate, this.unitRoot);
					}
					go.transform.SetSiblingIndex(l);
					AdventureUnitNormal unitNormal = go.GetComponent<AdventureUnitNormal>();
					CS$<>8__locals1.unitCanvas = unitNormal.canvasGroup;
					this._unitNormalRenderArray2D[(int)renderCoord.X, (int)renderCoord.Y] = unitNormal;
					unitNormal.gameObject.name = renderCoord.ToString();
					RectTransform unitNormalRect = unitNormal.GetComponent<RectTransform>();
					unitNormalRect.localPosition = this.GetUnitNormalLocation(renderCoord);
					unitNormal.SetMicroHolderActive(true);
					CS$<>8__locals1.control = unitNormal.GetComponent<BlockVolumeController>();
					CS$<>8__locals1.control.VolumeHeight = 0f;
					int dataCoordX = (int)(renderCoord.X - this.CenterCoord);
					int dataCoordY = (int)(renderCoord.Y - this.CenterCoord);
					float[] heights = new float[9];
					for (int h2 = 0; h2 < 9; h2 = num + 1)
					{
						AdventureBlockIndex dataBlockIndex = new AdventureBlockIndex(dataCoordX, dataCoordY, h2);
						AdventureBlockData blockCore = this.AdventureRuntime.GetBlockCore(dataBlockIndex);
						heights[h2] = blockCore.Height;
						dataBlockIndex = default(AdventureBlockIndex);
						blockCore = null;
						num = h2;
					}
					AdventureHeightCalculator.HeightResult heightResult = AdventureHeightCalculator.CalculateNormalized(heights);
					float blockHeight = heightResult.VolumeHeight;
					float time = this.<Routine>g__CalcActionInTime|82_1(1);
					bool showTransitionAnim = this._showTransitionAnim;
					if (showTransitionAnim)
					{
						DOVirtual.Float(0f, blockHeight, time, delegate(float h)
						{
							CS$<>8__locals1.control.VolumeHeight = h;
						}).SetAutoKill(true);
						DOVirtual.Float(0f, 1f, time, delegate(float alpha)
						{
							CS$<>8__locals1.unitCanvas.alpha = alpha;
						}).SetAutoKill(true);
					}
					else
					{
						CS$<>8__locals1.control.VolumeHeight = blockHeight;
						CS$<>8__locals1.unitCanvas.alpha = 1f;
					}
					float subTime = 0.3f;
					for (int subIndex = 0; subIndex < unitNormal.container.childCount; subIndex = num + 1)
					{
						ViewAdventureRemake.<>c__DisplayClass82_1 CS$<>8__locals2 = new ViewAdventureRemake.<>c__DisplayClass82_1();
						Transform microChild = unitNormal.container.GetChild(subIndex);
						AdventureUnitMicro unitMicro = microChild.GetComponent<AdventureUnitMicro>();
						unitMicro.SetPointerTriggerEnable(true);
						unitMicro.button.name = string.Format("button({0},{1})", renderCoord.X, renderCoord.Y);
						CS$<>8__locals2.sub = microChild.GetComponent<BlockVolumeController>();
						unitNormal.SetUnitMicro(unitMicro, this.UnitMicroSiblingIndexToDataIndex(subIndex));
						AdventureBlockIndex renderIndex = new AdventureBlockIndex((int)renderCoord.X, (int)renderCoord.Y, this.UnitMicroSiblingIndexToDataIndex(subIndex));
						unitMicro.RenderBlockIndex = renderIndex;
						AdventureBlockIndex dataBlockIndex2 = this.RenderIndexToDataIndex(unitMicro.RenderBlockIndex);
						CS$<>8__locals2.sub.VolumeHeight = 0f;
						AdventureBlock blockData = this.AdventureRuntime.GetBlock(dataBlockIndex2);
						AdventureBlockData blockCore2 = this.AdventureRuntime.GetBlockCore(dataBlockIndex2);
						float microHeight = heightResult.MicroHeights[dataBlockIndex2.I];
						DOVirtual.Float(0f, microHeight, (!this._showTransitionAnim) ? 0f : subTime, delegate(float h)
						{
							CS$<>8__locals2.sub.VolumeHeight = h;
						}).SetDelay(time + (float)subIndex * 0.1f).SetAutoKill(true);
						string icon = blockData.SpecialIcon;
						bool flag5 = string.IsNullOrEmpty(icon);
						if (flag5)
						{
							icon = blockCore2.Icon;
						}
						bool flag6 = string.IsNullOrEmpty(icon);
						if (flag6)
						{
							icon = "adventure_block_default";
						}
						bool flag7 = string.IsNullOrEmpty(blockData.SpecialIcon);
						if (flag7)
						{
							unitMicro.RecoverDecorates();
						}
						else
						{
							unitMicro.HideDecorates();
						}
						unitMicro.groundSurface.SetSprite(icon, true, null);
						unitMicro.SetDecorate(blockCore2.Decorates.ToList<string>());
						AdventureBlockIndex dataIndex = this.RenderIndexToDataIndex(renderIndex);
						CImage cloud = unitMicro.cloud;
						GameObject elementRoot = unitMicro.elementRoot;
						CanvasGroup decorateCanvas = unitMicro.blockDecoratesCanvasGroup;
						CanvasGroup elementCanvas = elementRoot.GetComponent<CanvasGroup>();
						cloud.SetAlpha(1f);
						bool inCloud = this.BlockInCloud(dataIndex);
						this._cloudDict[dataBlockIndex2] = inCloud;
						cloud.gameObject.SetActive(inCloud);
						elementCanvas.alpha = (float)(inCloud ? 0 : 1);
						decorateCanvas.alpha = (float)((inCloud || this._simpleViewModeOn || !string.IsNullOrEmpty(blockData.SpecialIcon)) ? 0 : 1);
						bool flag8 = inCloud;
						if (flag8)
						{
							cloud.SetSprite(this.GetRandomCloudIconName(), true, null);
						}
						CS$<>8__locals2 = null;
						microChild = null;
						unitMicro = null;
						renderIndex = default(AdventureBlockIndex);
						dataBlockIndex2 = default(AdventureBlockIndex);
						blockData = null;
						blockCore2 = null;
						icon = null;
						dataIndex = default(AdventureBlockIndex);
						cloud = null;
						elementRoot = null;
						decorateCanvas = null;
						elementCanvas = null;
						num = subIndex;
					}
					CS$<>8__locals1 = null;
					unitNormal = null;
					unitNormalRect = null;
					heights = null;
					heightResult = default(AdventureHeightCalculator.HeightResult);
				}
				else
				{
					ViewAdventureRemake.<>c__DisplayClass82_2 CS$<>8__locals3 = new ViewAdventureRemake.<>c__DisplayClass82_2();
					bool flag9 = pooledPeripherals.Count > 0;
					if (flag9)
					{
						List<GameObject> list2 = pooledPeripherals;
						go = list2[list2.Count - 1];
						pooledPeripherals.RemoveAt(pooledPeripherals.Count - 1);
						go.SetActive(true);
					}
					else
					{
						go = Object.Instantiate<GameObject>(this.outerBlockTemplate, this.unitRoot);
					}
					go.transform.SetSiblingIndex(l);
					CS$<>8__locals3.peripheral = go.GetComponent<AdventureUnitPeripheral>();
					this._unitPeripheralArray2D[(int)renderCoord.X, (int)renderCoord.Y] = CS$<>8__locals3.peripheral;
					CS$<>8__locals3.peripheral.gameObject.name = renderCoord.ToString();
					RectTransform peripheralRect = CS$<>8__locals3.peripheral.GetComponent<RectTransform>();
					peripheralRect.localPosition = this.GetUnitNormalLocation(renderCoord);
					CS$<>8__locals3.peripheral.SetBlockPieceActive(true);
					CS$<>8__locals3.control = CS$<>8__locals3.peripheral.GetComponent<BlockVolumeController>();
					CS$<>8__locals3.control.VolumeHeight = 0f;
					int intRandom = this._random.Next();
					float blockHeight2 = (float)intRandom / 2.1474836E+09f;
					int blockIndex = this._random.Next(this._outerBlockPool.Count);
					CS$<>8__locals3.peripheral.SetBlockPiece(this._outerBlockPool[blockIndex]);
					int circleIndex = this.GetCircleIndex(renderCoord);
					bool flag10 = this._flatDict != null && this._flatDict.Keys.Contains(renderCoord);
					if (flag10)
					{
						blockHeight2 = this._flatDict[renderCoord];
					}
					else
					{
						blockHeight2 = Mathf.Lerp(this.BlockConfig.CircleHeight[circleIndex * 2], this.BlockConfig.CircleHeight[circleIndex * 2 + 1], blockHeight2);
					}
					float time2 = this.<Routine>g__CalcActionInTime|82_1(circleIndex + 2);
					bool showTransitionAnim2 = this._showTransitionAnim;
					if (showTransitionAnim2)
					{
						DOVirtual.Float(0f, blockHeight2, time2, delegate(float h)
						{
							CS$<>8__locals3.control.VolumeHeight = h;
						}).SetAutoKill(true);
						DOVirtual.Float(0f, 1f, time2, delegate(float alpha)
						{
							CS$<>8__locals3.peripheral.canvasGroup.alpha = alpha;
						}).SetAutoKill(true);
					}
					else
					{
						CS$<>8__locals3.control.VolumeHeight = blockHeight2;
						CS$<>8__locals3.peripheral.canvasGroup.alpha = 1f;
					}
					CS$<>8__locals3 = null;
					peripheralRect = null;
				}
				renderCoord = default(ByteCoordinate);
				go = null;
				num = l;
			}
			foreach (GameObject go2 in pooledNormals)
			{
				go2.SetActive(false);
				go2 = null;
			}
			List<GameObject>.Enumerator enumerator = default(List<GameObject>.Enumerator);
			foreach (GameObject go3 in pooledPeripherals)
			{
				go3.SetActive(false);
				go3 = null;
			}
			List<GameObject>.Enumerator enumerator2 = default(List<GameObject>.Enumerator);
			this.ResetDarkParam();
			yield return new WaitForEndOfFrame();
			this._displayItems.Clear();
			this._allElementMap.Clear();
			this._visibleElementMap.Clear();
			this._elementPanel.totalCount = 0;
			this._elementPanel.RefillCells(0, false);
			for (int _i = 0; _i < (int)this._mapSize; _i = num + 1)
			{
				for (int _j = 0; _j < (int)this._mapSize; _j = num + 1)
				{
					AdventureUnitNormal _normal = this._unitNormalRenderArray2D[_i, _j];
					bool flag11 = _normal == null;
					if (!flag11)
					{
						for (int _k = 0; _k < 9; _k = num + 1)
						{
							AdventureUnitMicro _micro = _normal.GetUnitMicro(_k);
							bool flag12 = _micro == null;
							if (!flag12)
							{
								_micro.elementsHolder.gameObject.SetActive(false);
								_micro = null;
							}
							num = _k;
						}
						_normal = null;
					}
					num = _j;
				}
				num = _i;
			}
			this.Element.ShowAfterRefresh();
			this._routine = null;
			this.InitTaiwuLocation();
			this.FocusSlow();
			this.AdventureRemakeOpenPartTwo();
			this.ChangeToDarkCircle(true);
			AudioManager.Instance.PlaySound("ui_adventure_earthbreak", false, false);
			yield return new WaitForSeconds(this._unitRiseMaxTime);
			this.UpdateTaiwuIcon();
			yield return this.CoRefreshDisplay();
			this.AdventureRefreshBlockEffect(null);
			yield return new WaitForSeconds(this._unitRiseMaxTime / 2f);
			bool showTransitionAnim3 = this._showTransitionAnim;
			if (showTransitionAnim3)
			{
				AdventureDomainMethod.AsyncCall.PostEnterAdventureRemake(this, this._adventureId, delegate(int offset, RawDataPool dataPool)
				{
					bool hasEvent = false;
					Serializer.Deserialize(dataPool, offset, ref hasEvent);
					bool flag13 = hasEvent;
					if (flag13)
					{
						base.DelayCall(delegate
						{
							this.BanOperate(false);
						}, 1f);
					}
					else
					{
						this.BanOperate(false);
					}
				});
			}
			else
			{
				this.BanOperate(false);
			}
			yield break;
		}

		// Token: 0x0600A256 RID: 41558 RVA: 0x004BDF54 File Offset: 0x004BC154
		private Vector2 GetUnitNormalLocation(ByteCoordinate renderCoord)
		{
			int num = (int)(renderCoord.X - this.CenterCoord);
			int num2 = (int)(renderCoord.Y - this.CenterCoord);
			int xx = num;
			int yy = num2;
			ValueTuple<int, int> valueTuple = this.RotateClockwise90(xx, yy);
			int x = valueTuple.Item1;
			int y = valueTuple.Item2;
			float localX = (float)(y - x) * this._normalBlockSize.x / 2f;
			float localY = (float)(-(float)x - y) * this._normalBlockSize.y / 2f;
			return new Vector2(localX, localY);
		}

		// Token: 0x0600A257 RID: 41559 RVA: 0x004BDFD8 File Offset: 0x004BC1D8
		private int GetCircleIndex(ByteCoordinate renderCoord)
		{
			int num = (int)(renderCoord.X - this.CenterCoord);
			int num2 = (int)(renderCoord.Y - this.CenterCoord);
			int dataCoordX = num;
			int dataCoordY = num2;
			return ViewAdventureRemake.GetCircleIndex(dataCoordX, dataCoordY, this._adventureSize);
		}

		// Token: 0x0600A258 RID: 41560 RVA: 0x004BE018 File Offset: 0x004BC218
		public static int GetCircleIndex(int dataCoordX, int dataCoordY, int validSize)
		{
			int max = (int)Mathf.Max(MathF.Abs((float)dataCoordX), MathF.Abs((float)dataCoordY));
			int distance = max - validSize;
			bool flag = distance <= 1;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				result = distance - 1;
			}
			return result;
		}

		// Token: 0x0600A259 RID: 41561 RVA: 0x004BE058 File Offset: 0x004BC258
		private IEnumerable<ByteCoordinate> GenerateTraversalOrder(int n)
		{
			return this.GenerateTraversalOrder3(n);
		}

		// Token: 0x0600A25A RID: 41562 RVA: 0x004BE071 File Offset: 0x004BC271
		private IEnumerable<ByteCoordinate> GenerateTraversalOrder3(int n)
		{
			int maxDiff = n - 1;
			int num;
			for (int s = 0; s <= 2 * maxDiff; s = num + 1)
			{
				int currentDiff = maxDiff - s;
				int jMin = Mathf.Max(0, currentDiff);
				int jMax = Mathf.Min(n - 1, n - 1 + currentDiff);
				for (int i = jMax; i >= jMin; i = num - 1)
				{
					int j = i - currentDiff;
					yield return new ByteCoordinate((byte)j, (byte)i);
					num = i;
				}
				num = s;
			}
			yield break;
		}

		// Token: 0x0600A25B RID: 41563 RVA: 0x004BE088 File Offset: 0x004BC288
		private IEnumerable<ByteCoordinate> GetLeftEdge(int n)
		{
			int num;
			for (int i = 0; i < n; i = num + 1)
			{
				yield return new ByteCoordinate((byte)i, 0);
				num = i;
			}
			yield break;
		}

		// Token: 0x0600A25C RID: 41564 RVA: 0x004BE09F File Offset: 0x004BC29F
		private IEnumerable<ByteCoordinate> GetBottomEdge(int n)
		{
			int num;
			for (int i = 0; i < n; i = num + 1)
			{
				yield return new ByteCoordinate((byte)(n - 1), (byte)i);
				num = i;
			}
			yield break;
		}

		// Token: 0x0600A25D RID: 41565 RVA: 0x004BE0B6 File Offset: 0x004BC2B6
		private IEnumerable<ByteCoordinate> GetRightEdge(int n)
		{
			int num;
			for (int i = 0; i < n; i = num + 1)
			{
				yield return new ByteCoordinate((byte)i, (byte)(n - 1));
				num = i;
			}
			yield break;
		}

		// Token: 0x0600A25E RID: 41566 RVA: 0x004BE0CD File Offset: 0x004BC2CD
		private IEnumerable<ByteCoordinate> GetTopEdge(int n)
		{
			int num;
			for (int i = 0; i < n; i = num + 1)
			{
				yield return new ByteCoordinate(0, (byte)i);
				num = i;
			}
			yield break;
		}

		// Token: 0x0600A25F RID: 41567 RVA: 0x004BE0E4 File Offset: 0x004BC2E4
		private void InitUnitFlatData()
		{
			bool flag = this.BlockConfig.FlatPercentage > 0;
			if (flag)
			{
				this.ClearUnitFlatData();
				this._outerUnitCount = this.GetOuterUnitCount();
				this._flatUnitCount = this.GetFlatUnitCount(this._outerUnitCount);
				this.GetAllFlatStartPoint();
				this.GetSelectedFlatStartPoint();
				this.InitFlatDict();
			}
		}

		// Token: 0x0600A260 RID: 41568 RVA: 0x004BE140 File Offset: 0x004BC340
		private void InitStartScale()
		{
			this.moveAndScaleRoot.SetScaleProcess(this.moveAndScaleRoot.Min.x);
		}

		// Token: 0x0600A261 RID: 41569 RVA: 0x004BE160 File Offset: 0x004BC360
		private void InitFlatDict()
		{
			int growCount = 0;
			while (this._flatDict.Count < this._flatUnitCount && growCount < 20)
			{
				growCount++;
				for (int groupIndex = 0; groupIndex < this._selectedFlatStartPoint.Count; groupIndex++)
				{
					ByteCoordinate startUnit = this._selectedFlatStartPoint[groupIndex];
					bool flag = !this._flatDict.Keys.Contains(startUnit);
					if (flag)
					{
						this._flatDict.Add(startUnit, this._flatHeights[groupIndex]);
					}
					ByteCoordinate nextUnit = this.GetNextFlatUnit(startUnit);
					bool flag2 = !this._flatDict.Keys.Contains(nextUnit);
					if (flag2)
					{
						this._flatDict.Add(nextUnit, this._flatHeights[groupIndex]);
					}
					bool flag3 = this._flatDict.Count >= this._flatUnitCount;
					if (flag3)
					{
						break;
					}
				}
			}
		}

		// Token: 0x0600A262 RID: 41570 RVA: 0x004BE25C File Offset: 0x004BC45C
		private ByteCoordinate GetNextFlatUnit(ByteCoordinate startUnit)
		{
			int loopCount = 0;
			ByteCoordinate nextUnit;
			do
			{
				EAdventureDirection direction = ViewAdventureRemake.GetRandomDirection();
				nextUnit = ViewAdventureRemake.GetNextFlatUnitByDirection(startUnit, direction);
				startUnit = nextUnit;
				loopCount++;
			}
			while (!this.CheckNextFlatUnitValid(nextUnit) && loopCount < 21);
			return nextUnit;
		}

		// Token: 0x0600A263 RID: 41571 RVA: 0x004BE2A0 File Offset: 0x004BC4A0
		public static ByteCoordinate GetNextFlatUnitByDirection(ByteCoordinate startUnit, EAdventureDirection direction)
		{
			byte x2 = startUnit.X;
			byte y2 = startUnit.Y;
			byte x = x2;
			byte y = y2;
			bool flag = direction == EAdventureDirection.Up;
			if (flag)
			{
				y += 1;
			}
			else
			{
				bool flag2 = direction == EAdventureDirection.Down;
				if (flag2)
				{
					y -= 1;
				}
				else
				{
					bool flag3 = direction == EAdventureDirection.Left;
					if (flag3)
					{
						x -= 1;
					}
					else
					{
						bool flag4 = direction == EAdventureDirection.Right;
						if (flag4)
						{
							x += 1;
						}
					}
				}
			}
			return new ByteCoordinate(x, y);
		}

		// Token: 0x0600A264 RID: 41572 RVA: 0x004BE30C File Offset: 0x004BC50C
		private bool CheckNextFlatUnitValid(ByteCoordinate coordinate)
		{
			return !this._flatDict.Keys.Contains(coordinate) && coordinate.X < this._mapSize && coordinate.Y < this._mapSize && !this.IsRenderIndexInner((int)coordinate.X, (int)coordinate.Y);
		}

		// Token: 0x0600A265 RID: 41573 RVA: 0x004BE368 File Offset: 0x004BC568
		public static EAdventureDirection GetRandomDirection()
		{
			return (EAdventureDirection)Random.Range(0, 4);
		}

		// Token: 0x0600A266 RID: 41574 RVA: 0x004BE384 File Offset: 0x004BC584
		private void ClearUnitFlatData()
		{
			if (this._flatDict == null)
			{
				this._flatDict = new Dictionary<ByteCoordinate, float>();
			}
			this._flatDict.Clear();
			if (this._allFlatStartPoint == null)
			{
				this._allFlatStartPoint = new List<ByteCoordinate>();
			}
			this._allFlatStartPoint.Clear();
			if (this._selectedFlatStartPoint == null)
			{
				this._selectedFlatStartPoint = new List<ByteCoordinate>();
			}
			this._selectedFlatStartPoint.Clear();
			if (this._flatHeights == null)
			{
				this._flatHeights = new List<float>();
			}
			this._flatHeights.Clear();
		}

		// Token: 0x0600A267 RID: 41575 RVA: 0x004BE410 File Offset: 0x004BC610
		private int GetOuterUnitCount()
		{
			int validSize = this._adventureSize;
			int validCount = 2 * validSize * validSize + 2 * validSize + 1;
			return (int)(this._mapSize * this._mapSize) - validCount;
		}

		// Token: 0x0600A268 RID: 41576 RVA: 0x004BE448 File Offset: 0x004BC648
		private int GetFlatUnitCount(int outerCount)
		{
			return (int)((float)(outerCount * this.BlockConfig.FlatPercentage) / 100f);
		}

		// Token: 0x0600A269 RID: 41577 RVA: 0x004BE470 File Offset: 0x004BC670
		private void GetAllFlatStartPoint()
		{
			this._allFlatStartPoint.Add(new ByteCoordinate(0, 0));
			this._allFlatStartPoint.Add(new ByteCoordinate(0, this.CenterCoord));
			this._allFlatStartPoint.Add(new ByteCoordinate(0, this.CenterCoord * 2));
			this._allFlatStartPoint.Add(new ByteCoordinate(this.CenterCoord, 0));
			this._allFlatStartPoint.Add(new ByteCoordinate(this.CenterCoord * 2, 0));
			this._allFlatStartPoint.Add(new ByteCoordinate(this.CenterCoord, this.CenterCoord * 2));
			this._allFlatStartPoint.Add(new ByteCoordinate(this.CenterCoord * 2, this.CenterCoord));
			this._allFlatStartPoint.Add(new ByteCoordinate(this.CenterCoord * 2, this.CenterCoord * 2));
		}

		// Token: 0x0600A26A RID: 41578 RVA: 0x004BE55C File Offset: 0x004BC75C
		private void GetSelectedFlatStartPoint()
		{
			int sumCount = (int)Random.Range(2f, MathF.Min(5f, (float)this._allFlatStartPoint.Count));
			for (int i = 0; i < sumCount; i++)
			{
				ByteCoordinate startPoint = this._allFlatStartPoint.GetRandom<ByteCoordinate>();
				this._selectedFlatStartPoint.Add(startPoint);
				this._allFlatStartPoint.Remove(startPoint);
				this._flatHeights.Add(this.BlockConfig.FlatHeight.GetRandom<float>());
			}
		}

		// Token: 0x0600A26B RID: 41579 RVA: 0x004BE5E0 File Offset: 0x004BC7E0
		private void Awake()
		{
			this.paths.OverrideVertices = this._indicateLineValidVertices;
			this.invalidPaths.OverrideVertices = this._indicateLineInvalidVertices;
			this.influenceEdge.OverrideVertices = this._influenceEdgeVertices;
			this._pawnTemplatePool = new PoolItem("UI_AdventureRemake", this.pawnAnimTemplate);
			this._elementsFadeTemplatePool = new PoolItem("UI_AdventureRemake", this.elementsFadeTemplate);
			this._dialogPool = new PoolItem("AdventureDialog", this.adventureDialogPrefab);
			this._overlapHolderPool = new PoolItem("OverlapHolder", this.overlapHolderPrefab);
			this.AwakeInit();
		}

		// Token: 0x0600A26C RID: 41580 RVA: 0x004BE680 File Offset: 0x004BC880
		private void OnDestroy()
		{
			PoolItem pawnTemplatePool = this._pawnTemplatePool;
			if (pawnTemplatePool != null)
			{
				pawnTemplatePool.Destroy();
			}
			this._pawnTemplatePool = null;
			PoolItem elementsFadeTemplatePool = this._elementsFadeTemplatePool;
			if (elementsFadeTemplatePool != null)
			{
				elementsFadeTemplatePool.Destroy();
			}
			this._elementsFadeTemplatePool = null;
			PoolItem dialogPool = this._dialogPool;
			if (dialogPool != null)
			{
				dialogPool.Destroy();
			}
			this._dialogPool = null;
			PoolItem overlapHolderPool = this._overlapHolderPool;
			if (overlapHolderPool != null)
			{
				overlapHolderPool.Destroy();
			}
			this._overlapHolderPool = null;
		}

		// Token: 0x0600A26D RID: 41581 RVA: 0x004BE6F4 File Offset: 0x004BC8F4
		private void OnEnable()
		{
			this._doingMove = false;
			ViewWorldMap.InAdventureRemake = true;
			this.AddAllEvent();
			this.Element.OnHide = null;
			UIElement element = this.Element;
			element.OnHide = (Action)Delegate.Combine(element.OnHide, new Action(delegate()
			{
				TaiwuEventDomainMethod.Call.TriggerListener("AdventureRemakeExit", true);
				AudioManager.Instance.PlaySound("ui_map_whoosh_exit", false, false);
			}));
		}

		// Token: 0x0600A26E RID: 41582 RVA: 0x004BE760 File Offset: 0x004BC960
		private void OnDisable()
		{
			this.ReturnAllDialog();
			this.waitUpdateShowTimeElementId.Clear();
			ViewWorldMap.InAdventureRemake = false;
			this.RemoveAllEvent();
			this.ClearMovingPawn();
			this.ClearPathsAndCost();
			this.HideElementRange();
			this._elementPanel.totalCount = 0;
			this._elementPanel.RefillCells(0, false);
			List<AdventureBlockIndex> list = this._paths;
			if (list != null)
			{
				list.Clear();
			}
			UIManager.Instance.SetEscHandler(null);
		}

		// Token: 0x0600A26F RID: 41583 RVA: 0x004BE7DC File Offset: 0x004BC9DC
		private void AddAllEvent()
		{
			GEvent.Add(UiEvents.AdventureUnitMicroClick, new GEvent.Callback(this.AdventureUnitMicroClick));
			GEvent.Add(UiEvents.AdventureUnitMicroPointerEnter, new GEvent.Callback(this.AdventureUnitMicroPointerEnter));
			GEvent.Add(UiEvents.AdventureUnitMicroPointerExit, new GEvent.Callback(this.AdventureUnitMicroPointerExit));
			GEvent.Add(UiEvents.AdventureExitClick, new GEvent.Callback(this.AdventureExitClick));
			GEvent.Add(UiEvents.OnAdventureTaiwuChanged, new GEvent.Callback(this.OnAdventureTaiwuChanged));
			GEvent.Add(UiEvents.MonthNotifyProcessComplete, new GEvent.Callback(this.MonthNotifyProcessComplete));
			GEvent.Add(UiEvents.AdventureRemakeDictChange, new GEvent.Callback(this.AdventureRemakeDictChange));
			GEvent.Add(UiEvents.TopUiChanged, new GEvent.Callback(this.TopUiChanged));
			GEvent.Add(UiEvents.AdventureRemakeFinish, new GEvent.Callback(this.AdventureRemakeFinish));
			GEvent.Add(UiEvents.AdventureElementAlertAnim, new GEvent.Callback(this.AdventureElementAlertAnim));
			GEvent.Add(UiEvents.AdventureBlockChangeIcon, new GEvent.Callback(this.AdventureBlockChangeIcon));
			GEvent.Add(UiEvents.AdventureElementShowHideEffect, new GEvent.Callback(this.AdventureElementShowHideEffect));
			GEvent.Add(UiEvents.AdventureRefreshBlockEffect, new GEvent.Callback(this.AdventureRefreshBlockEffect));
			GEvent.Add(UiEvents.AdventureRefreshGlobalEffect, new GEvent.Callback(this.AdventureRefreshGlobalEffect));
			GEvent.Add(UiEvents.AdventureAdvanceDaysSet, new GEvent.Callback(this.AdventureAdvanceDaysSet));
			GEvent.Add(UiEvents.AdventureTaiwuShowDialog, new GEvent.Callback(this.AdventureTaiwuShowDialog));
			GEvent.Add(UiEvents.AdventureElementShowDialog, new GEvent.Callback(this.AdventureElementShowDialog));
			GEvent.Add(UiEvents.AdventureResetCamera, new GEvent.Callback(this.AdventureResetCamera));
			GEvent.Add(UiEvents.AdventureElementDeleteAnim, new GEvent.Callback(this.AdventureElementDeleteAnim));
			GEvent.Add(UiEvents.AdventureCameraMoveToBlock, new GEvent.Callback(this.AdventureCameraMoveToBlock));
			GEvent.Add(UiEvents.AdventureDelayAction, new GEvent.Callback(this.AdventureDelayAction));
			GEvent.Add(EEvents.OnActionPointChange, new GEvent.Callback(this.OnActionPointChange));
			GEvent.Add(UiEvents.AdventureLightingSettingChanged, new GEvent.Callback(this.AdventureLightingSettingChanged));
			GEvent.Add(UiEvents.AdventureStartSelectElement, new GEvent.Callback(this.AdventureStartSelectElement));
			GEvent.Add(UiEvents.AdventureEventHandled, new GEvent.Callback(this.AdventureEventHandled));
			GEvent.Add(EEvents.OnMonthChange, new GEvent.Callback(this.OnMonthChange));
		}

		// Token: 0x0600A270 RID: 41584 RVA: 0x004BEAB4 File Offset: 0x004BCCB4
		private void RemoveAllEvent()
		{
			GEvent.Remove(UiEvents.AdventureUnitMicroClick, new GEvent.Callback(this.AdventureUnitMicroClick));
			GEvent.Remove(UiEvents.AdventureUnitMicroPointerEnter, new GEvent.Callback(this.AdventureUnitMicroPointerEnter));
			GEvent.Remove(UiEvents.AdventureUnitMicroPointerExit, new GEvent.Callback(this.AdventureUnitMicroPointerExit));
			GEvent.Remove(UiEvents.AdventureExitClick, new GEvent.Callback(this.AdventureExitClick));
			GEvent.Remove(UiEvents.OnAdventureTaiwuChanged, new GEvent.Callback(this.OnAdventureTaiwuChanged));
			GEvent.Remove(UiEvents.MonthNotifyProcessComplete, new GEvent.Callback(this.MonthNotifyProcessComplete));
			GEvent.Remove(UiEvents.AdventureRemakeDictChange, new GEvent.Callback(this.AdventureRemakeDictChange));
			GEvent.Remove(UiEvents.TopUiChanged, new GEvent.Callback(this.TopUiChanged));
			GEvent.Remove(UiEvents.AdventureRemakeFinish, new GEvent.Callback(this.AdventureRemakeFinish));
			GEvent.Remove(UiEvents.AdventureElementAlertAnim, new GEvent.Callback(this.AdventureElementAlertAnim));
			GEvent.Remove(UiEvents.AdventureBlockChangeIcon, new GEvent.Callback(this.AdventureBlockChangeIcon));
			GEvent.Remove(UiEvents.AdventureElementShowHideEffect, new GEvent.Callback(this.AdventureElementShowHideEffect));
			GEvent.Remove(UiEvents.AdventureRefreshBlockEffect, new GEvent.Callback(this.AdventureRefreshBlockEffect));
			GEvent.Remove(UiEvents.AdventureRefreshGlobalEffect, new GEvent.Callback(this.AdventureRefreshGlobalEffect));
			GEvent.Remove(UiEvents.AdventureAdvanceDaysSet, new GEvent.Callback(this.AdventureAdvanceDaysSet));
			GEvent.Remove(UiEvents.AdventureTaiwuShowDialog, new GEvent.Callback(this.AdventureTaiwuShowDialog));
			GEvent.Remove(UiEvents.AdventureElementShowDialog, new GEvent.Callback(this.AdventureElementShowDialog));
			GEvent.Remove(UiEvents.AdventureResetCamera, new GEvent.Callback(this.AdventureResetCamera));
			GEvent.Remove(UiEvents.AdventureCameraMoveToBlock, new GEvent.Callback(this.AdventureCameraMoveToBlock));
			GEvent.Remove(UiEvents.AdventureDelayAction, new GEvent.Callback(this.AdventureDelayAction));
			GEvent.Remove(EEvents.OnActionPointChange, new GEvent.Callback(this.OnActionPointChange));
			GEvent.Remove(UiEvents.AdventureLightingSettingChanged, new GEvent.Callback(this.AdventureLightingSettingChanged));
			GEvent.Remove(UiEvents.AdventureStartSelectElement, new GEvent.Callback(this.AdventureStartSelectElement));
			GEvent.Remove(UiEvents.AdventureEventHandled, new GEvent.Callback(this.AdventureEventHandled));
			GEvent.Remove(EEvents.OnMonthChange, new GEvent.Callback(this.OnMonthChange));
		}

		// Token: 0x0600A271 RID: 41585 RVA: 0x004BED70 File Offset: 0x004BCF70
		private void OnMonthChange(ArgumentBox argBox)
		{
			int remainMonths = this.AdventureRuntime.CalcRemainMonths(SingletonObject.getInstance<BasicGameData>().CurrDate);
			this.remainTimeTips.gameObject.SetActive(remainMonths == 1);
		}

		// Token: 0x0600A272 RID: 41586 RVA: 0x004BEDA9 File Offset: 0x004BCFA9
		private void TopUiChanged(ArgumentBox argBox)
		{
			this.advanceDaysBtn.interactable = (SingletonObject.getInstance<TimeManager>().GetRemainingActionPointConvertToDays() > 0);
			this.StopContinuousMove();
		}

		// Token: 0x0600A273 RID: 41587 RVA: 0x004BEDCC File Offset: 0x004BCFCC
		private void OnActionPointChange(ArgumentBox argBox)
		{
			bool flag = AdventureLightingManager.HasActiveLighting();
			if (flag)
			{
				AdventureLightingManager.Instance.RotateAzimuth();
			}
		}

		// Token: 0x0600A274 RID: 41588 RVA: 0x004BEDF0 File Offset: 0x004BCFF0
		private void AdventureLightingSettingChanged(ArgumentBox argBox)
		{
			bool flag = !base.isActiveAndEnabled || this.AdventureTaiwu.NotInAdventure;
			if (!flag)
			{
				this.RefreshAllMicroBlockColor();
			}
		}

		// Token: 0x0600A275 RID: 41589 RVA: 0x004BEE24 File Offset: 0x004BD024
		private void Update()
		{
			bool flag = this.AdventureTaiwu.NotInAdventure || this.adventureRemakeFinish.Finish || !this._transitionFinish;
			if (!flag)
			{
				bool mouseButtonDown = Input.GetMouseButtonDown(1);
				if (mouseButtonDown)
				{
					bool flag2 = this.startSelectElement && UIManager.Instance.IsFocusElement(UIElement.AdventureRemake);
					if (flag2)
					{
						this.CancelSelectElement();
					}
					this.StopContinuousMove();
					this.RefreshElementPanel(this.RenderIndexToDataIndex(this.TaiwuRenderBlockIndex));
					this.HideAndRefresh();
				}
				this.UpdateMove();
				this.UpdateTaiwuAction();
				this.UpdateAdventureBanTimeBall();
				this.UpdateTaiwuShowDialog();
				this.UpdateElementShowDialog();
				this.UpdateDialogShowTime();
				this.UpdateShowElementDeleteAnim();
				bool flag3 = AdventureKit.SimpleViewMode.Check(this.Element, false, false, false, true, false);
				if (flag3)
				{
					this.simpleViewModeToggle.isOn = !this.simpleViewModeToggle.isOn;
				}
				this.moveAndScaleRoot.enabled = !this._doingMove;
			}
		}

		// Token: 0x0600A276 RID: 41590 RVA: 0x004BEF2C File Offset: 0x004BD12C
		private void RefreshPath(AdventureBlockIndex toRenderIndex)
		{
			this.ClearPathsAndCost();
			this.FindPaths(toRenderIndex);
			this.RefreshCostByPath(this._paths);
			this.RenderPathLine();
		}

		// Token: 0x0600A277 RID: 41591 RVA: 0x004BEF54 File Offset: 0x004BD154
		private void RefreshCostByPath(List<AdventureBlockIndex> path)
		{
			bool flag = path == null || path.Count == 0;
			if (!flag)
			{
				int leftAction = SingletonObject.getInstance<BasicGameData>().ActionPointCurrMonth;
				int actionSum = 0;
				this.costHolder.Rebuild<RectTransform>(path.Count, delegate(RectTransform goCost, int index)
				{
					AdventureBlockIndex pathDataIndex = path[index];
					AdventureBlockIndex renderCoord = this.DataIndexToRenderIndex(pathDataIndex);
					Vector2 pos = this.GetUnitMicroCenterPos(renderCoord);
					AdventureUnitMicro unitMicro = this.GetUnitMicro(renderCoord);
					bool flag2 = index > 0;
					if (flag2)
					{
						int actionCost = this.AdventureRuntime.GetMoveCost(pathDataIndex);
						actionSum += actionCost;
						this.SetCostText(goCost.gameObject, actionSum, unitMicro);
					}
					else
					{
						goCost.gameObject.SetActive(false);
					}
					bool flag3 = leftAction >= actionSum;
					if (flag3)
					{
						this._indicateLineValidVertices.Add(pos);
					}
					else
					{
						bool flag4 = this._indicateLineInvalidVertices.Count <= 0;
						if (flag4)
						{
							List<Vector2> indicateLineInvalidVertices = this._indicateLineInvalidVertices;
							List<Vector2> indicateLineValidVertices = this._indicateLineValidVertices;
							indicateLineInvalidVertices.Add(indicateLineValidVertices[indicateLineValidVertices.Count - 1]);
						}
						this._indicateLineInvalidVertices.Add(pos);
					}
				});
			}
		}

		// Token: 0x0600A278 RID: 41592 RVA: 0x004BEFD0 File Offset: 0x004BD1D0
		private void FindPaths(AdventureBlockIndex toRenderIndex)
		{
			IReadOnlyList<AdventureBlockIndex> path = this.AdventureRuntime.FindShortestPath(this.RenderIndexToDataIndex(this.TaiwuRenderBlockIndex), this.RenderIndexToDataIndex(toRenderIndex));
			this._paths = ((path != null) ? path.ToList<AdventureBlockIndex>() : null);
		}

		// Token: 0x0600A279 RID: 41593 RVA: 0x004BF00F File Offset: 0x004BD20F
		private void ClearPathsAndCost()
		{
			this.ClearPathLine();
			this.costHolder.Rebuild<RectTransform>(0, null);
		}

		// Token: 0x0600A27A RID: 41594 RVA: 0x004BF028 File Offset: 0x004BD228
		private void SetCostText(GameObject goCost, int moveCost, AdventureUnitMicro micro)
		{
			bool active = moveCost >= 0;
			bool costEnough = SingletonObject.getInstance<TimeManager>().IsActionPointEnough(moveCost);
			AdventureRemakeCostBackInfo costBackInfo = goCost.GetComponent<AdventureRemakeCostBackInfo>();
			bool flag = active;
			if (flag)
			{
				TextMeshProUGUI costText = costBackInfo.txtMeshCost;
				costText.text = ((float)moveCost * 0.1f).ToString("F1").SetColor(costEnough ? "pinkyellow" : "grey");
				costBackInfo.imgCostBack.SetSprite(costEnough ? "map_luxian_3" : "map_luxian_4", false, null);
				goCost.transform.position = micro.transform.position;
				RectTransform referRectTransform = costBackInfo.rectTs;
				referRectTransform.anchoredPosition = new Vector2(referRectTransform.anchoredPosition.x, referRectTransform.anchoredPosition.y + micro.GetComponent<BlockVolumeController>().GetVolumeHeightOffsetY());
			}
			costBackInfo.imgCostBack.gameObject.SetActive(active);
		}

		// Token: 0x0600A27B RID: 41595 RVA: 0x004BF114 File Offset: 0x004BD314
		private void ClearPathLine()
		{
			this._indicateLineValidVertices.Clear();
			this._indicateLineInvalidVertices.Clear();
			this.RenderPathLine();
		}

		// Token: 0x0600A27C RID: 41596 RVA: 0x004BF136 File Offset: 0x004BD336
		private void RenderPathLine()
		{
			this.paths.GetComponent<CImage>().SetVerticesDirty();
			this.invalidPaths.GetComponent<CImage>().SetVerticesDirty();
		}

		// Token: 0x0600A27D RID: 41597 RVA: 0x004BF15C File Offset: 0x004BD35C
		private Vector2 GetUnitMicroCenterPos(AdventureBlockIndex renderIndex)
		{
			Vector2 centerPos = new Vector2(0f, 48f);
			AdventureUnitNormal unitNormal = this._unitNormalRenderArray2D[renderIndex.X, renderIndex.Y];
			RectTransform unitNormalRectTransform = unitNormal.GetComponent<RectTransform>();
			BlockVolumeController unitNormalVolume = unitNormal.GetComponent<BlockVolumeController>();
			centerPos.x = unitNormalRectTransform.anchoredPosition.x;
			centerPos.y = unitNormalRectTransform.anchoredPosition.y + unitNormalVolume.GetVolumeHeightOffsetY();
			AdventureUnitMicro unitMicro = unitNormal.GetUnitMicro(renderIndex.I);
			RectTransform unitMicroRectTransform = unitMicro.GetComponent<RectTransform>();
			BlockVolumeController unitMicroVolume = unitMicro.GetComponent<BlockVolumeController>();
			centerPos.x += unitMicroRectTransform.anchoredPosition.x;
			centerPos.y += unitMicroRectTransform.anchoredPosition.y + unitMicroVolume.GetVolumeHeightOffsetY();
			return centerPos;
		}

		// Token: 0x0600A27E RID: 41598 RVA: 0x004BF22C File Offset: 0x004BD42C
		private List<Vector2> GetUnitMicroVertex(AdventureBlockIndex renderIndex, EAdventureDirection direction)
		{
			Vector2 centerPos = this.GetUnitMicroCenterPos(renderIndex);
			float offsetY = 4f;
			centerPos.y -= offsetY;
			List<Vector2> vertices = new List<Vector2>();
			bool flag = direction == EAdventureDirection.Up;
			if (flag)
			{
				centerPos.y += this._microBlockSize.y / 2f;
				vertices.Add(centerPos);
				centerPos.y -= this._microBlockSize.y / 2f;
				centerPos.x += this._microBlockSize.x / 2f;
				vertices.Add(centerPos);
			}
			else
			{
				bool flag2 = direction == EAdventureDirection.Right;
				if (flag2)
				{
					centerPos.x += this._microBlockSize.x / 2f;
					vertices.Add(centerPos);
					centerPos.x -= this._microBlockSize.x / 2f;
					centerPos.y -= this._microBlockSize.y / 2f;
					vertices.Add(centerPos);
				}
				else
				{
					bool flag3 = direction == EAdventureDirection.Down;
					if (flag3)
					{
						centerPos.y -= this._microBlockSize.y / 2f;
						vertices.Add(centerPos);
						centerPos.y += this._microBlockSize.y / 2f;
						centerPos.x -= this._microBlockSize.x / 2f;
						vertices.Add(centerPos);
					}
					else
					{
						bool flag4 = direction == EAdventureDirection.Left;
						if (flag4)
						{
							centerPos.x -= this._microBlockSize.x / 2f;
							vertices.Add(centerPos);
							centerPos.x += this._microBlockSize.x / 2f;
							centerPos.y += this._microBlockSize.y / 2f;
							vertices.Add(centerPos);
						}
					}
				}
			}
			return vertices;
		}

		// Token: 0x0600A27F RID: 41599 RVA: 0x004BF42C File Offset: 0x004BD62C
		private AdventureUnitMicro GetUnitMicro(AdventureBlockIndex renderIndex)
		{
			AdventureUnitNormal unitNormal = this._unitNormalRenderArray2D[renderIndex.X, renderIndex.Y];
			return unitNormal.GetUnitMicro(renderIndex.I);
		}

		// Token: 0x0600A280 RID: 41600 RVA: 0x004BF464 File Offset: 0x004BD664
		private void UpdateMove()
		{
			this._hotkeyMoveDirectionIsDown = false;
			bool flag = !this._hotkeyMoveDirectionIsDown;
			if (flag)
			{
				bool flag2 = MapCommandKit.MoveUp.Check(this.Element, true, false, false, true, false);
				if (flag2)
				{
					this._hotkeyMoveDirection = EAdventureDirection.Up;
					this._hotkeyMoveDirectionIsDown = true;
				}
				else
				{
					bool flag3 = MapCommandKit.MoveLeft.Check(this.Element, true, false, false, true, false);
					if (flag3)
					{
						this._hotkeyMoveDirection = EAdventureDirection.Left;
						this._hotkeyMoveDirectionIsDown = true;
					}
					else
					{
						bool flag4 = MapCommandKit.MoveDown.Check(this.Element, true, false, false, true, false);
						if (flag4)
						{
							this._hotkeyMoveDirection = EAdventureDirection.Down;
							this._hotkeyMoveDirectionIsDown = true;
						}
						else
						{
							bool flag5 = MapCommandKit.MoveRight.Check(this.Element, true, false, false, true, false);
							if (flag5)
							{
								this._hotkeyMoveDirection = EAdventureDirection.Right;
								this._hotkeyMoveDirectionIsDown = true;
							}
						}
					}
				}
			}
			bool hotkeyMoveDirectionIsDown = this._hotkeyMoveDirectionIsDown;
			if (hotkeyMoveDirectionIsDown)
			{
				bool continuousMoving = this._continuousMoving;
				if (continuousMoving)
				{
					this.StopContinuousMove();
				}
				else
				{
					this.Move(this._hotkeyMoveDirection);
				}
			}
		}

		// Token: 0x0600A281 RID: 41601 RVA: 0x004BF56C File Offset: 0x004BD76C
		private void Move(EAdventureDirection moveDirection)
		{
			bool flag = this._doingMove || this.TaiwuInAction || this.startSelectElement || this._cameraMoving;
			if (!flag)
			{
				this._doingMove = true;
				this.ClearPathsAndCost();
				bool continuousMoving = this._continuousMoving;
				if (continuousMoving)
				{
					this.RefreshCostByPath(this._paths);
				}
				else
				{
					AdventureBlockIndex targetRenderIndex = this.DataIndexToRenderIndex(this.RenderIndexToDataIndex(this.TaiwuRenderBlockIndex).Move(moveDirection));
					this.FindPaths(targetRenderIndex);
					this.RefreshCostByPath(this._paths);
				}
				List<AdventureBlockIndex> list = this._paths;
				bool flag2 = list != null && list.Count > 0;
				if (flag2)
				{
					AdventureBlockIndex firstPath = this._paths[1];
					int actionCost = this.AdventureRuntime.GetMoveCost(firstPath);
					bool flag3 = actionCost > SingletonObject.getInstance<BasicGameData>().ActionPointCurrMonth;
					if (flag3)
					{
						this._doingMove = false;
						DialogCmd cmd = new DialogCmd
						{
							Type = 1,
							Title = LocalStringManager.Get(LanguageKey.UI_AdvanceMonth_TipTitle),
							Content = LocalStringManager.Get(LanguageKey.LK_Left_Time_Not_Enough_Tip).ColorReplace() + "\n" + LocalStringManager.Get(LanguageKey.LK_Advance_Month_Confirm),
							Yes = delegate()
							{
								GEvent.OnEvent(UiEvents.ShowAdvanceMonthConfirm, EasyPool.Get<ArgumentBox>().SetObject("callback", new Action(this.AdvanceMonth)));
							}
						};
						UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
						UIManager.Instance.MaskUI(UIElement.Dialog);
						return;
					}
				}
				this.RenderPathLine();
				AdventureDomainMethod.AsyncCall.MoveInAdventure(this, (sbyte)moveDirection, delegate(int offset, RawDataPool pool)
				{
					bool success = false;
					Serializer.Deserialize(pool, offset, ref success);
					this.SelectedRenderBlockIndex = null;
					bool flag4 = !success;
					if (flag4)
					{
						this._doingMove = false;
						this.StopContinuousMove();
					}
				});
			}
		}

		// Token: 0x0600A282 RID: 41602 RVA: 0x004BF703 File Offset: 0x004BD903
		private void AdvanceMonth()
		{
			GlobalDomainMethod.AsyncCall.CheckDriveSpace(this, delegate(int offset, RawDataPool dataPool)
			{
				bool hasSpace = false;
				Serializer.Deserialize(dataPool, offset, ref hasSpace);
				bool flag = hasSpace;
				if (flag)
				{
					ViewAdventureRemake.<AdvanceMonth>g__Action|142_1();
				}
				else
				{
					string archiveDirPath = GameApp.GetArchiveDirPath();
					string disk = Path.GetPathRoot(archiveDirPath);
					string title = LocalStringManager.Get(LanguageKey.LK_Save_CheckDiskSpace_Title);
					string content = LocalStringManager.GetFormat(LanguageKey.LK_Save_CheckDiskSpace_Content, disk).ColorReplace();
					CommonUtils.ShowConfirmDialog(title, content, new Action(ViewAdventureRemake.<AdvanceMonth>g__Action|142_1), null, EDialogType.None);
				}
			});
		}

		// Token: 0x0600A283 RID: 41603 RVA: 0x004BF730 File Offset: 0x004BD930
		private void ContinuousMove()
		{
			List<AdventureBlockIndex> list = this._paths;
			bool flag = list != null && list.Count > 1;
			if (flag)
			{
				this.Move(this._paths[0].GetDirection(this._paths[1]));
				bool flag2 = this._paths.Count > 1;
				if (flag2)
				{
					this._paths.RemoveAt(0);
				}
			}
			else
			{
				this._continuousMoving = false;
				this._doingMove = false;
				this.ClearPathsAndCost();
			}
		}

		// Token: 0x0600A284 RID: 41604 RVA: 0x004BF7B8 File Offset: 0x004BD9B8
		private void StopContinuousMove()
		{
			bool continuousMoving = this._continuousMoving;
			if (continuousMoving)
			{
				this._continuousMoving = false;
			}
			List<AdventureBlockIndex> list = this._paths;
			if (list != null)
			{
				list.Clear();
			}
			this.ClearPathsAndCost();
			this.SelectedRenderBlockIndex = null;
			this.selected.gameObject.SetActive(false);
		}

		// Token: 0x0600A285 RID: 41605 RVA: 0x004BF812 File Offset: 0x004BDA12
		private void MoveToNextBlock(AdventureBlockIndex toRenderIndex)
		{
			base.StartCoroutine(this.CoPlayMoveAnim(this.TaiwuRenderBlockIndex, toRenderIndex));
		}

		// Token: 0x0600A286 RID: 41606 RVA: 0x004BF829 File Offset: 0x004BDA29
		private IEnumerator CoPlayMoveAnim(AdventureBlockIndex fromRenderIndex, AdventureBlockIndex toRenderIndex)
		{
			this._doingMove = true;
			this.TaiwuJumpAnim(toRenderIndex);
			AdventureUnitMicro unitMicro = this.GetUnitMicro(toRenderIndex);
			this.CameraFocus(unitMicro.GetComponent<RectTransform>(), ViewAdventureRemake.MoveDuration);
			yield return new WaitForSeconds(ViewAdventureRemake.MoveDuration);
			this.OnMoveAniComplete();
			yield break;
		}

		// Token: 0x0600A287 RID: 41607 RVA: 0x004BF848 File Offset: 0x004BDA48
		private void OnMoveAniComplete()
		{
			this.UpdateTaiwuIconScale();
			this.TaiwuMoveSmoke();
			this.ClearMovingPawn();
			this.UpdateTaiwuIcon();
			this._doingMove = false;
			bool continuousMoving = this._continuousMoving;
			if (continuousMoving)
			{
				this.ContinuousMove();
			}
			else
			{
				List<AdventureBlockIndex> list = this._paths;
				if (list != null)
				{
					list.Clear();
				}
				this.ClearPathsAndCost();
			}
		}

		// Token: 0x0600A288 RID: 41608 RVA: 0x004BF8AC File Offset: 0x004BDAAC
		private void UpdateTaiwuIconScale()
		{
			CImage taiwuIcon = this.taiwuIconRoot.taiwuIcon;
			RectTransform taiwuIconRect = taiwuIcon.GetComponent<RectTransform>();
			GameObject actionHodler = this.taiwuIconRoot.paramProgressTemplate;
			RectTransform actionRect = actionHodler.GetComponent<RectTransform>();
			List<AdventureElement> elementList;
			bool flag;
			if (this._visibleElementMap.TryGetValue(this.AdventureTaiwu.Index, out elementList))
			{
				flag = elementList.Any((AdventureElement e) => !AdventureRemakeModel.Core.GetAdventureElementData(e.CoreId).VisibleIgnoreSorting);
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			if (flag2)
			{
				taiwuIconRect.localScale = new Vector3(0.8f, 0.8f, 0.8f);
				taiwuIconRect.localPosition = new Vector3(-50f, 0f, 0f);
				actionRect.anchoredPosition = new Vector3(73f, 29f, 0f);
			}
			else
			{
				taiwuIconRect.localScale = Vector3.one;
				taiwuIconRect.localPosition = Vector3.zero;
				actionRect.anchoredPosition = new Vector3(122f, 47f, 0f);
			}
		}

		// Token: 0x0600A289 RID: 41609 RVA: 0x004BF9C0 File Offset: 0x004BDBC0
		private void TaiwuJumpAnim(AdventureBlockIndex toRenderIndex)
		{
			AdventureUnitMicro parent = this.FindUnitMicro(toRenderIndex);
			RectTransform taiwuIconRect = this.taiwuIconRoot.GetComponent<RectTransform>();
			taiwuIconRect.SetParent(parent.jumpTarget, true);
			taiwuIconRect.localScale = Vector3.one;
			AudioManager.Instance.PlaySound("ui_adventure_foot_" + Random.Range(1, 4).ToString(), false, false);
			taiwuIconRect.DOLocalJump(Vector3.zero, this.jumpPower, 1, ViewWorldMap.MoveStepTime, false).SetEase(Ease.OutQuad).OnUpdate(delegate
			{
				Vector3 pos = this.unitRoot.transform.InverseTransformPoint(taiwuIconRect.position);
				bool flag = this._indicateLineValidVertices.Count > 0;
				if (flag)
				{
					this._indicateLineValidVertices[0] = pos;
					this.RenderPathLine();
				}
			});
		}

		// Token: 0x0600A28A RID: 41610 RVA: 0x004BFA74 File Offset: 0x004BDC74
		private void OnAdventureTaiwuChanged(ArgumentBox argBox)
		{
			this.advanceDaysBtn.interactable = (SingletonObject.getInstance<TimeManager>().GetRemainingActionPointConvertToDays() > 0);
			bool notInAdventure = this.AdventureTaiwu.NotInAdventure;
			if (notInAdventure)
			{
				base.DelayFrameCall(new Action(this.TaiwuNotInAdventure), 1U);
			}
			else
			{
				bool flag = !this.AdventureRemakeModel.AdventureRemakeDict.Keys.Contains(this._adventureId);
				if (!flag)
				{
					AdventureBlockIndex newTaiwuRenderIndex = this.DataIndexToRenderIndex(this.AdventureTaiwu.Index);
					bool flag2 = newTaiwuRenderIndex != this.TaiwuRenderBlockIndex;
					if (flag2)
					{
						this.MoveToNextBlock(newTaiwuRenderIndex);
						this.TaiwuRenderBlockIndex = newTaiwuRenderIndex;
						bool flag3 = !this._continuousMoving;
						if (flag3)
						{
							this.UpdateUnitMicroSelected(this.TaiwuRenderBlockIndex);
						}
					}
					AdventureBlockIndex taiwuDataCoord = this.RenderIndexToDataIndex(this.TaiwuRenderBlockIndex);
					this.RefreshElementPanel(taiwuDataCoord);
					this.HideAndRefresh();
				}
			}
		}

		// Token: 0x0600A28B RID: 41611 RVA: 0x004BFB56 File Offset: 0x004BDD56
		private void MonthNotifyProcessComplete(ArgumentBox argBox)
		{
			this.ClearPathsAndCost();
		}

		// Token: 0x17001104 RID: 4356
		// (get) Token: 0x0600A28C RID: 41612 RVA: 0x004BFB60 File Offset: 0x004BDD60
		private AdventureRuntime AdventureRuntime
		{
			get
			{
				return this.AdventureRemakeModel.AdventureRemakeDict[this._adventureId];
			}
		}

		// Token: 0x17001105 RID: 4357
		// (get) Token: 0x0600A28D RID: 41613 RVA: 0x004BFB78 File Offset: 0x004BDD78
		private AdventureRemakeModel AdventureRemakeModel
		{
			get
			{
				return SingletonObject.getInstance<AdventureRemakeModel>();
			}
		}

		// Token: 0x17001106 RID: 4358
		// (get) Token: 0x0600A28E RID: 41614 RVA: 0x004BFB7F File Offset: 0x004BDD7F
		private AdventureTaiwu AdventureTaiwu
		{
			get
			{
				return this.AdventureRemakeModel.AdventureTaiwu;
			}
		}

		// Token: 0x17001107 RID: 4359
		// (get) Token: 0x0600A28F RID: 41615 RVA: 0x004BFB8C File Offset: 0x004BDD8C
		private WorldMapModel WorldMapModel
		{
			get
			{
				return SingletonObject.getInstance<WorldMapModel>();
			}
		}

		// Token: 0x17001108 RID: 4360
		// (get) Token: 0x0600A290 RID: 41616 RVA: 0x004BFB93 File Offset: 0x004BDD93
		// (set) Token: 0x0600A291 RID: 41617 RVA: 0x004BFB9B File Offset: 0x004BDD9B
		public AdventureBlockIndex TaiwuRenderBlockIndex { get; set; }

		// Token: 0x17001109 RID: 4361
		// (get) Token: 0x0600A292 RID: 41618 RVA: 0x004BFBA4 File Offset: 0x004BDDA4
		// (set) Token: 0x0600A293 RID: 41619 RVA: 0x004BFBAC File Offset: 0x004BDDAC
		public AdventureBlockIndex? SelectedRenderBlockIndex { get; set; }

		// Token: 0x0600A294 RID: 41620 RVA: 0x004BFBB8 File Offset: 0x004BDDB8
		private void ResetState()
		{
			this._isFinish = false;
			this._skipFinishAnim = false;
			this.remainTimeTips.gameObject.SetActive(false);
			this._transitionFinish = false;
			this._actionCanContinue = true;
			this._renderCoordList.Clear();
			this._cloudDict.Clear();
			this.selected.SetParent(base.transform);
			this.selected.gameObject.SetActive(false);
			this.taiwuIconRoot.gameObject.SetActive(false);
			this.taiwuIconRoot.taiwuDialog.gameObject.SetActive(false);
			this.adventureRemakeFinish.Finish = false;
			this._doingMove = false;
			this.ClearPathsAndCost();
			this.ResetMask();
			this.SwitchTemporaryItemLocation(false);
			this._taiwuAdvanceMovePointLeft = 0;
			this.waitUpdateShowTimeElementId.Clear();
			this._visibleElementMap.Clear();
		}

		// Token: 0x0600A295 RID: 41621 RVA: 0x004BFCA4 File Offset: 0x004BDEA4
		private void AdventureUnitMicroClick(ArgumentBox argBox)
		{
			AdventureBlockIndex renderIndex;
			argBox.Get<AdventureBlockIndex>("AdventureUnitMicroIndex", out renderIndex);
			bool flag = this.IsRenderIndexInner(renderIndex.X, renderIndex.Y);
			if (flag)
			{
				this.OnUnitMicroClick(renderIndex);
				bool flag2 = this.startSelectElement;
				if (flag2)
				{
					int distance = this.RenderIndexToDataIndex(renderIndex).GetManhattanDistance(this.selectElementCenterDataIndex);
					bool inRange = distance <= this.selectElementRange;
					bool flag3 = inRange;
					if (flag3)
					{
						this.RefreshElementPanel(this.RenderIndexToDataIndex(renderIndex));
					}
					else
					{
						this._elementPanel.transform.parent.gameObject.SetActive(false);
					}
				}
				else
				{
					this.RefreshElementPanel(this.RenderIndexToDataIndex(renderIndex));
				}
			}
		}

		// Token: 0x0600A296 RID: 41622 RVA: 0x004BFD5C File Offset: 0x004BDF5C
		private void OnUnitMicroClick(AdventureBlockIndex renderIndex)
		{
			this.UpdateUnitMicroSelected(renderIndex);
			bool flag = this.startSelectElement;
			if (!flag)
			{
				bool flag2 = renderIndex == this.TaiwuRenderBlockIndex;
				if (flag2)
				{
					this.ClearPathsAndCost();
					this.SelectedRenderBlockIndex = new AdventureBlockIndex?(renderIndex);
				}
				else
				{
					bool flag3 = renderIndex == this.SelectedRenderBlockIndex;
					if (flag3)
					{
						this._continuousMoving = true;
						this.FindPaths(renderIndex);
						this.ContinuousMove();
					}
					else
					{
						this.SelectedRenderBlockIndex = new AdventureBlockIndex?(renderIndex);
						this.RefreshPath(renderIndex);
					}
				}
			}
		}

		// Token: 0x0600A297 RID: 41623 RVA: 0x004BFDFC File Offset: 0x004BDFFC
		private void UpdateUnitMicroSelected(AdventureBlockIndex renderIndex)
		{
			AdventureUnitMicro parent = this.FindUnitMicro(renderIndex);
			bool flag = parent == null;
			if (!flag)
			{
				this.selected.transform.SetParent(parent.selectedHolder, false);
				this.selected.localScale = Vector3.one;
				this.selected.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(0f, 0f, -45f));
				this.selected.gameObject.SetActive(true);
			}
		}

		// Token: 0x0600A298 RID: 41624 RVA: 0x004BFE7F File Offset: 0x004BE07F
		private void UpdateTaiwuRenderLocationAndIcon(AdventureBlockIndex index)
		{
			this.TaiwuRenderBlockIndex = index;
			this.UpdateTaiwuIcon();
		}

		// Token: 0x0600A299 RID: 41625 RVA: 0x004BFE94 File Offset: 0x004BE094
		private void UpdateTaiwuIcon()
		{
			RectTransform taiwuIconRect = this.taiwuIconRoot.GetComponent<RectTransform>();
			AdventureUnitMicro micro = this.FindUnitMicro(this.TaiwuRenderBlockIndex);
			taiwuIconRect.SetParent(micro.jumpTarget, false);
			taiwuIconRect.localPosition = Vector3.zero;
			taiwuIconRect.localScale = Vector3.one;
			taiwuIconRect.gameObject.SetActive(true);
			AdventurePointLight adventurePointLight;
			if ((adventurePointLight = this._taiwuPointLight) == null)
			{
				adventurePointLight = (this._taiwuPointLight = this.taiwuIconRoot.GetComponent<AdventurePointLight>());
			}
			AdventurePointLight light = adventurePointLight;
			bool flag = light != null;
			if (flag)
			{
				light.enabled = true;
				light.BlockIndex = this.TaiwuRenderBlockIndex;
				this.SyncTaiwuPointLightRange(light);
			}
		}

		// Token: 0x0600A29A RID: 41626 RVA: 0x004BFF3C File Offset: 0x004BE13C
		private void CacheAdventureLightingDefaults()
		{
			bool lightingDefaultsInitialized = this._lightingDefaultsInitialized;
			if (!lightingDefaultsInitialized)
			{
				if (this._lightingManager == null)
				{
					this._lightingManager = base.GetComponent<AdventureLightingManager>();
				}
				if (this._taiwuPointLight == null)
				{
					this._taiwuPointLight = this.taiwuIconRoot.GetComponent<AdventurePointLight>();
				}
				bool flag = this._lightingManager != null;
				if (flag)
				{
					this._defaultGlobalColor = this._lightingManager.GlobalColor;
					this._defaultGlobalIntensity = this._lightingManager.GlobalIntensity;
					this._defaultGlobalIncidenceAngle = this._lightingManager.GlobalIncidenceAngle;
					this._defaultRotationAnglePerStep = this._lightingManager.RotationAnglePerStep;
				}
				bool flag2 = this._taiwuPointLight != null;
				if (flag2)
				{
					this._defaultPointLightColor = this._taiwuPointLight.LightColor;
					this._defaultPointLightIntensity = this._taiwuPointLight.Intensity;
					this._defaultPointLightVirtualZ = this._taiwuPointLight.VirtualZ;
				}
				this._lightingDefaultsInitialized = true;
			}
		}

		// Token: 0x0600A29B RID: 41627 RVA: 0x004C0028 File Offset: 0x004BE228
		private void ApplyAdventureLightingConfig()
		{
			this.CacheAdventureLightingDefaults();
			AdventureData adventureData = AdventureRemakeModel.Core.GetAdventureData(this.AdventureRuntime.CoreId);
			this.ApplyWorldLightingConfig(adventureData);
			this.ApplyTaiwuPointLightConfig(adventureData);
		}

		// Token: 0x0600A29C RID: 41628 RVA: 0x004C0064 File Offset: 0x004BE264
		private void ApplyWorldLightingConfig(AdventureData adventureData)
		{
			bool flag = this._lightingManager == null;
			if (!flag)
			{
				int azimuthValue = this.AdventureRuntime.GetParameterOrDefault("LightingInitialAzimuth", 135).Current;
				AdventureLightData lightData = (adventureData != null) ? adventureData.LightingWorld : null;
				bool flag2 = lightData == null || !ViewAdventureRemake.HasLightingData(lightData);
				if (flag2)
				{
					this._lightingManager.GlobalColor = this._defaultGlobalColor;
					this._lightingManager.GlobalIntensity = this._defaultGlobalIntensity;
					this._lightingManager.GlobalIncidenceAngle = this._defaultGlobalIncidenceAngle;
					this._lightingManager.GlobalAzimuthAngle = (float)azimuthValue;
				}
				else
				{
					this._lightingManager.GlobalColor = ViewAdventureRemake.TryParseLightingColor(lightData.ColorInHex, this._defaultGlobalColor);
					this._lightingManager.GlobalIntensity = lightData.Strength;
					this._lightingManager.GlobalIncidenceAngle = Mathf.Clamp(lightData.Height, 0f, 90f);
					this._lightingManager.GlobalAzimuthAngle = (float)azimuthValue;
				}
				this._lightingManager.RotationAnglePerStep = ((adventureData == null) ? this._defaultRotationAnglePerStep : (adventureData.LightingRotate ? ((float)adventureData.LightingRotateAngle) : 0f));
			}
		}

		// Token: 0x0600A29D RID: 41629 RVA: 0x004C0198 File Offset: 0x004BE398
		private void ApplyTaiwuPointLightConfig(AdventureData adventureData)
		{
			bool flag = this._taiwuPointLight == null;
			if (!flag)
			{
				AdventureLightData lightData = (adventureData != null) ? adventureData.LightingTaiwu : null;
				bool flag2 = lightData == null || !ViewAdventureRemake.HasLightingData(lightData);
				if (flag2)
				{
					this._taiwuPointLight.LightColor = this._defaultPointLightColor;
					this._taiwuPointLight.Intensity = this._defaultPointLightIntensity;
					this._taiwuPointLight.VirtualZ = this._defaultPointLightVirtualZ;
				}
				else
				{
					this._taiwuPointLight.LightColor = ViewAdventureRemake.TryParseLightingColor(lightData.ColorInHex, this._defaultPointLightColor);
					this._taiwuPointLight.Intensity = lightData.Strength;
					this._taiwuPointLight.VirtualZ = lightData.Height;
				}
				this.SyncTaiwuPointLightRange(this._taiwuPointLight);
			}
		}

		// Token: 0x0600A29E RID: 41630 RVA: 0x004C0260 File Offset: 0x004BE460
		private void SyncTaiwuPointLightRange(AdventurePointLight light)
		{
			bool flag = light == null;
			if (!flag)
			{
				light.Range = (float)Mathf.Max(0, this._viewTypeNear);
			}
		}

		// Token: 0x0600A29F RID: 41631 RVA: 0x004C0290 File Offset: 0x004BE490
		private static bool HasLightingData(AdventureLightData lightData)
		{
			return !string.IsNullOrEmpty(lightData.ColorInHex) || !Mathf.Approximately(lightData.Strength, 0f) || !Mathf.Approximately(lightData.Height, 0f);
		}

		// Token: 0x0600A2A0 RID: 41632 RVA: 0x004C02D8 File Offset: 0x004BE4D8
		private static Color TryParseLightingColor(string colorInHex, Color fallback)
		{
			bool flag = string.IsNullOrEmpty(colorInHex);
			Color result;
			if (flag)
			{
				result = fallback;
			}
			else
			{
				string htmlColor = colorInHex.StartsWith("#") ? colorInHex : ("#" + colorInHex);
				Color parsedColor;
				result = (ColorUtility.TryParseHtmlString(htmlColor, out parsedColor) ? parsedColor : fallback);
			}
			return result;
		}

		// Token: 0x0600A2A1 RID: 41633 RVA: 0x004C0324 File Offset: 0x004BE524
		private void RefreshStaticPointLights()
		{
			AdventureData adventureData = AdventureRemakeModel.Core.GetAdventureData(this.AdventureRuntime.CoreId);
			RepeatedField<AdventurePointLightData> lightingPoints = (adventureData != null) ? adventureData.LightingPoints : null;
			bool flag = lightingPoints == null;
			if (!flag)
			{
				for (int i = 0; i < this._unitNormalRenderArray2D.GetLength(0); i++)
				{
					for (int j = 0; j < this._unitNormalRenderArray2D.GetLength(1); j++)
					{
						AdventureUnitNormal unitNormal = this._unitNormalRenderArray2D[i, j];
						bool flag2 = unitNormal == null;
						if (!flag2)
						{
							for (int k = 0; k < 9; k++)
							{
								AdventureUnitMicro unitMicro = unitNormal.GetUnitMicro(k);
								bool flag3 = unitMicro == null;
								if (!flag3)
								{
									AdventurePointLight light = unitMicro.gameObject.GetOrAddComponent<AdventurePointLight>();
									unitMicro.microPointLight = light;
									light.enabled = false;
								}
							}
						}
					}
				}
				foreach (AdventurePointLightData lightData in lightingPoints)
				{
					AdventureBlockIndex renderIndex = this.DataIndexToRenderIndex(lightData.Index);
					bool flag4 = renderIndex.X < 0 || renderIndex.X >= this._unitNormalRenderArray2D.GetLength(0) || renderIndex.Y < 0 || renderIndex.Y >= this._unitNormalRenderArray2D.GetLength(1);
					if (!flag4)
					{
						AdventureUnitNormal unitNormal2 = this._unitNormalRenderArray2D[renderIndex.X, renderIndex.Y];
						bool flag5 = unitNormal2 == null;
						if (!flag5)
						{
							AdventureUnitMicro unitMicro2 = unitNormal2.GetUnitMicro(renderIndex.I);
							bool flag6 = unitMicro2 == null;
							if (!flag6)
							{
								AdventurePointLight light2 = unitMicro2.microPointLight;
								bool flag7 = light2 == null;
								if (flag7)
								{
									light2 = unitMicro2.gameObject.AddComponent<AdventurePointLight>();
									unitMicro2.microPointLight = light2;
								}
								AdventureLightData lData = lightData.LightData;
								light2.LightColor = ViewAdventureRemake.TryParseLightingColor(lData.ColorInHex, this._defaultPointLightColor);
								light2.Intensity = lData.Strength;
								light2.VirtualZ = lData.Height;
								light2.Range = (float)Mathf.Abs(lightData.Range);
								light2.NoRangeClamp = (lightData.Range < 0);
								light2.BlockIndex = renderIndex;
								light2.enabled = true;
							}
						}
					}
				}
			}
		}

		// Token: 0x0600A2A2 RID: 41634 RVA: 0x004C05C8 File Offset: 0x004BE7C8
		public static Vector2 GetVolumeOffset(int index)
		{
			if (!true)
			{
			}
			Vector2 result;
			switch (index)
			{
			case 0:
				result = new Vector2(-1f, -1f);
				break;
			case 1:
				result = new Vector2(0f, -1f);
				break;
			case 2:
				result = new Vector2(1f, -1f);
				break;
			case 3:
				result = new Vector2(-1f, 0f);
				break;
			case 4:
				result = new Vector2(0f, 0f);
				break;
			case 5:
				result = new Vector2(1f, 0f);
				break;
			case 6:
				result = new Vector2(-1f, 1f);
				break;
			case 7:
				result = new Vector2(0f, 1f);
				break;
			case 8:
				result = new Vector2(1f, 1f);
				break;
			default:
				result = Vector2.zero;
				break;
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x0600A2A3 RID: 41635 RVA: 0x004C06C4 File Offset: 0x004BE8C4
		private AdventureUnitMicro FindUnitMicro(AdventureBlockIndex renderIndex)
		{
			AdventureUnitNormal unitNormal = this._unitNormalRenderArray2D[renderIndex.X, renderIndex.Y];
			bool flag = unitNormal != null;
			AdventureUnitMicro result;
			if (flag)
			{
				AdventureUnitMicro unitMicro = unitNormal.GetUnitMicro(renderIndex.I);
				result = unitMicro;
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x0600A2A4 RID: 41636 RVA: 0x004C070C File Offset: 0x004BE90C
		protected override void OnClick(Transform btn)
		{
			string name = btn.name;
			string a = name;
			if (a == "ParameterScrollViewButton")
			{
				this.globalParameterPanel.gameObject.SetActive(!this.globalParameterPanel.gameObject.activeSelf);
				bool activeSelf = this.globalParameterPanel.gameObject.activeSelf;
				if (activeSelf)
				{
					this.RefreshGlobalParameterPanel();
				}
			}
		}

		// Token: 0x0600A2A5 RID: 41637 RVA: 0x004C0774 File Offset: 0x004BE974
		private void AdventureResetCamera(ArgumentBox argBox)
		{
			this.CameraFocusTaiwu(1f);
		}

		// Token: 0x0600A2A6 RID: 41638 RVA: 0x004C0783 File Offset: 0x004BE983
		private void AdventureCameraMoveToBlock(ArgumentBox argBox)
		{
			base.StartCoroutine(this.CameraMoveToBlock(argBox));
		}

		// Token: 0x0600A2A7 RID: 41639 RVA: 0x004C0794 File Offset: 0x004BE994
		private IEnumerator CameraMoveToBlock(ArgumentBox argBox)
		{
			this._cameraMoving = true;
			this._checkDuration = ViewAdventureRemake.MoveDuration;
			AdventureBlockIndex blockDataIndex;
			argBox.Get<AdventureBlockIndex>("BlockIndex", out blockDataIndex);
			float oneWayDuration;
			argBox.Get("OneWayDuration", out oneWayDuration);
			float stayDuration;
			argBox.Get("StayDuration", out stayDuration);
			bool cameraReset;
			argBox.Get("CameraReset", out cameraReset);
			AdventureUnitMicro micro = this.GetUnitMicro(this.DataIndexToRenderIndex(blockDataIndex));
			this.CameraFocus(micro.GetComponent<RectTransform>(), oneWayDuration);
			yield return new WaitForSeconds(oneWayDuration);
			bool flag = stayDuration > 0f;
			if (flag)
			{
				yield return new WaitForSeconds(stayDuration);
			}
			bool flag2 = cameraReset;
			if (flag2)
			{
				this.CameraFocusTaiwu(oneWayDuration);
				yield return new WaitForSeconds(oneWayDuration);
			}
			yield return new WaitForSeconds(0.1f);
			this._cameraMoving = false;
			TaiwuEventDomainMethod.Call.TriggerListener("AdventureCameraMoveToBlockFinish", true);
			yield break;
		}

		// Token: 0x0600A2A8 RID: 41640 RVA: 0x004C07AA File Offset: 0x004BE9AA
		private void AdventureDelayAction(ArgumentBox argBox)
		{
			base.StartCoroutine(this.AdventureDelay(argBox));
		}

		// Token: 0x0600A2A9 RID: 41641 RVA: 0x004C07BB File Offset: 0x004BE9BB
		private IEnumerator AdventureDelay(ArgumentBox argBox)
		{
			this._cameraMoving = true;
			float delayDuration;
			argBox.Get("DelayDuration", out delayDuration);
			yield return new WaitForSeconds(delayDuration);
			this._cameraMoving = false;
			TaiwuEventDomainMethod.Call.TriggerListener("AdventureDelayFinish", true);
			yield break;
		}

		// Token: 0x0600A2AA RID: 41642 RVA: 0x004C07D4 File Offset: 0x004BE9D4
		private AdventureBlockIndex DataIndexToRenderIndex(AdventureBlockIndex index)
		{
			return new AdventureBlockIndex(index.X + (int)this.CenterCoord, index.Y + (int)this.CenterCoord, index.I);
		}

		// Token: 0x0600A2AB RID: 41643 RVA: 0x004C080C File Offset: 0x004BEA0C
		private AdventureBlockIndex RenderIndexToDataIndex(AdventureBlockIndex index)
		{
			return new AdventureBlockIndex(index.X - (int)this.CenterCoord, index.Y - (int)this.CenterCoord, index.I);
		}

		// Token: 0x0600A2AC RID: 41644 RVA: 0x004C0844 File Offset: 0x004BEA44
		private int UnitMicroSiblingIndexToDataIndex(int siblingIndex)
		{
			return this._unitMicroIndexMappings[siblingIndex];
		}

		// Token: 0x0600A2AD RID: 41645 RVA: 0x004C0864 File Offset: 0x004BEA64
		[return: TupleElementNames(new string[]
		{
			"x",
			"y"
		})]
		private ValueTuple<int, int> RotateCounterClockwise90(int x, int y)
		{
			return new ValueTuple<int, int>(y, -x);
		}

		// Token: 0x0600A2AE RID: 41646 RVA: 0x004C0880 File Offset: 0x004BEA80
		[return: TupleElementNames(new string[]
		{
			"x",
			"y"
		})]
		private ValueTuple<int, int> RotateClockwise90(int x, int y)
		{
			return new ValueTuple<int, int>(-y, x);
		}

		// Token: 0x0600A2AF RID: 41647 RVA: 0x004C089A File Offset: 0x004BEA9A
		private void AdventureExitClick(ArgumentBox argBox)
		{
			this.ExitAdventure();
		}

		// Token: 0x0600A2B0 RID: 41648 RVA: 0x004C08A4 File Offset: 0x004BEAA4
		private void ExitAdventure()
		{
			AdventureDomainMethod.AsyncCall.ExitAdventureRemake(null, delegate(int offset, RawDataPool pool)
			{
				bool result = false;
				Serializer.Deserialize(pool, offset, ref result);
				bool flag = result;
				if (flag)
				{
				}
			});
		}

		// Token: 0x0600A2B1 RID: 41649 RVA: 0x004C08D0 File Offset: 0x004BEAD0
		private void RefreshExitIcon()
		{
			for (int i = 0; i < (int)this._mapSize; i++)
			{
				for (int j = 0; j < (int)this._mapSize; j++)
				{
					AdventureUnitNormal unitNormal = this._unitNormalRenderArray2D[i, j];
					bool flag = unitNormal == null;
					if (!flag)
					{
						for (int k = 0; k < 9; k++)
						{
							AdventureUnitMicro unitMicro = unitNormal.GetUnitMicro(k);
							bool flag2 = unitMicro == null;
							if (!flag2)
							{
								AdventureBlockIndex dataIndex = this.RenderIndexToDataIndex(unitMicro.RenderBlockIndex);
								bool isExit = this.AdventureRuntime.IsExitPoint(dataIndex);
								unitMicro.SetExit(isExit, this.TaiwuRenderBlockIndex == unitMicro.RenderBlockIndex);
							}
						}
					}
				}
			}
		}

		// Token: 0x0600A2B2 RID: 41650 RVA: 0x004C09A4 File Offset: 0x004BEBA4
		private void AdventureRemakeOpenPartTwo()
		{
			this.SetMoveAndScaleRootSize();
			this.OpenPartTwo();
		}

		// Token: 0x0600A2B3 RID: 41651 RVA: 0x004C09B8 File Offset: 0x004BEBB8
		private void SetMoveAndScaleRootSize()
		{
			RectTransform moveAndScaleRootRect = this.moveAndScaleRoot.GetComponent<RectTransform>();
			moveAndScaleRootRect.sizeDelta = new Vector2((float)((int)this._mapSize + this.grayMaskSizeX * 2) * this._normalBlockSize.x, (float)((int)this._mapSize + this.grayMaskSizeY * 2) * this._normalBlockSize.y);
		}

		// Token: 0x0600A2B4 RID: 41652 RVA: 0x004C0A18 File Offset: 0x004BEC18
		private void OpenPartTwo()
		{
			GEvent.OnEvent(UiEvents.AdventureRemakeOpenPartTwo, null);
			this._blendMaskSequence = ViewWorldMap.ResetScaleTween(this._blendMaskSequence);
			CImage mask = this.adventureMask;
			CImage blendImage = this.adventureBlend;
			Tweener maskTweenPart2 = DOVirtual.Float(1f, 0f, 0.45f, delegate(float alpha)
			{
				mask.SetAlpha(alpha);
			}).SetEase(Ease.Linear);
			Tweener blendTweenPart2 = DOVirtual.Float(0.3f, 0f, 0.45f, delegate(float alpha)
			{
				blendImage.SetAlpha(alpha);
			}).SetEase(Ease.Linear);
			maskTweenPart2.OnComplete(delegate
			{
				mask.gameObject.SetActive(false);
			});
			this._blendMaskSequence.Append(maskTweenPart2);
			this._blendMaskSequence.Join(blendTweenPart2);
			this._blendMaskSequence.Play<Sequence>();
		}

		// Token: 0x0600A2B5 RID: 41653 RVA: 0x004C0AF0 File Offset: 0x004BECF0
		private void ResetMask()
		{
			CImage mask = this.adventureMask;
			CImage blendImage = this.adventureBlend;
			mask.SetAlpha(1f);
			blendImage.SetAlpha(0.3f);
			mask.gameObject.SetActive(true);
		}

		// Token: 0x0600A2B6 RID: 41654 RVA: 0x004C0B34 File Offset: 0x004BED34
		private void CameraFocus(RectTransform rect)
		{
			RectTransform moveAndScaleRootRect = this.moveAndScaleRoot.GetComponent<RectTransform>();
			Vector3 localPosition = moveAndScaleRootRect.InverseTransformPoint(rect.position);
			moveAndScaleRootRect.anchoredPosition = new Vector2(-localPosition.x * moveAndScaleRootRect.localScale.x, -localPosition.y * moveAndScaleRootRect.localScale.y);
		}

		// Token: 0x0600A2B7 RID: 41655 RVA: 0x004C0B90 File Offset: 0x004BED90
		private void CameraFocus(Vector2 anchoredPosition, float duration)
		{
			RectTransform moveAndScaleRootRect = this.moveAndScaleRoot.GetComponent<RectTransform>();
			Vector2 startPos = moveAndScaleRootRect.anchoredPosition;
			Vector2 endPos = anchoredPosition;
			DOVirtual.Float(0f, 1f, duration, delegate(float stepVal)
			{
				moveAndScaleRootRect.anchoredPosition = Vector2.Lerp(startPos, moveAndScaleRootRect.localScale.x * endPos, stepVal);
			});
		}

		// Token: 0x0600A2B8 RID: 41656 RVA: 0x004C0BEC File Offset: 0x004BEDEC
		private void CameraFocus(RectTransform rect, float duration)
		{
			RectTransform moveAndScaleRootRect = this.moveAndScaleRoot.GetComponent<RectTransform>();
			Vector3 localPosition = moveAndScaleRootRect.InverseTransformPoint(rect.position);
			Vector2 startPos = moveAndScaleRootRect.anchoredPosition;
			Vector2 endPos = new Vector2(-localPosition.x, -localPosition.y);
			DOVirtual.Float(0f, 1f, duration, delegate(float stepVal)
			{
				moveAndScaleRootRect.anchoredPosition = Vector2.Lerp(startPos, moveAndScaleRootRect.localScale.x * endPos, stepVal);
			});
		}

		// Token: 0x0600A2B9 RID: 41657 RVA: 0x004C0C6C File Offset: 0x004BEE6C
		private void CameraFocusTaiwu(float duration)
		{
			RectTransform taiwuIconRect = this.taiwuIconRoot.GetComponent<RectTransform>();
			RectTransform moveAndScaleRootRect = this.moveAndScaleRoot.GetComponent<RectTransform>();
			Vector3 localPosition = moveAndScaleRootRect.InverseTransformPoint(taiwuIconRect.position);
			Vector2 startPos = moveAndScaleRootRect.anchoredPosition;
			Vector2 endPos = new Vector2(-localPosition.x, -localPosition.y);
			DOVirtual.Float(0f, 1f, duration, delegate(float stepVal)
			{
				moveAndScaleRootRect.anchoredPosition = Vector2.Lerp(startPos, moveAndScaleRootRect.localScale.x * endPos, stepVal);
			});
		}

		// Token: 0x0600A2BA RID: 41658 RVA: 0x004C0CF8 File Offset: 0x004BEEF8
		private void FocusSlow()
		{
			AdventureUnitMicro taiwuMicro = this.GetUnitMicro(this.TaiwuRenderBlockIndex);
			this.moveAndScaleRoot.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
			this.moveAndScaleRoot.enabled = false;
			bool flag = this.lightingEffect != null && this._showTransitionAnim;
			if (flag)
			{
				this.lightingEffect.CaptureTargetAzimuth();
				this.lightingEffect.UpdateLightingOffset(0f);
			}
			RectTransform taiwuMicroRectTs = taiwuMicro.GetComponent<RectTransform>();
			this.CameraFocus(taiwuMicroRectTs);
			float totalTime = this._unitRiseMaxTime + this.focusToTaiwuTime;
			DOVirtual.Float(0f, totalTime, (!this._showTransitionAnim) ? 0f : totalTime, delegate(float time)
			{
				float scale = this.curve.Evaluate(time);
				this.moveAndScaleRoot.transform.localScale = new Vector3(scale, scale, scale);
				this.CameraFocus(taiwuMicroRectTs);
				bool flag2 = this.lightingEffect != null && this._showTransitionAnim && totalTime > 0f;
				if (flag2)
				{
					this.lightingEffect.UpdateLightingOffset(time / totalTime);
				}
			}).SetAutoKill(true).OnComplete(delegate
			{
				bool flag2 = this.lightingEffect != null && this._showTransitionAnim;
				if (flag2)
				{
					this.lightingEffect.UpdateLightingOffset(1f);
				}
				this.moveAndScaleRoot.enabled = true;
				this.moveAndScaleRoot.Reset();
				this._transitionFinish = true;
			});
		}

		// Token: 0x0600A2BB RID: 41659 RVA: 0x004C0DF4 File Offset: 0x004BEFF4
		private void RefreshGlobalParameterPanel()
		{
			bool flag = this._adventureId < 0;
			if (!flag)
			{
				AdventureData adventureData = AdventureRemakeModel.Core.GetAdventureData(this.AdventureRuntime.CoreId);
				this._globalParameterList = (from p in adventureData.Parameters.Where(delegate(AdventureParameterData p)
				{
					if (p != null && !string.IsNullOrEmpty(p.Name))
					{
						EAdventureParameterType type = p.Type;
						if (type == EAdventureParameterType.State || type == EAdventureParameterType.Normal)
						{
							return p.Style != 0;
						}
					}
					return false;
				})
				select new ValueTuple<AdventureParameterData, AdventureParameterValue>(p, this.AdventureRuntime.GetParameter(p.Key)) into pair
				where pair.Item1.Type == EAdventureParameterType.Normal || pair.Item2.AsProgress > 0f || pair.Item2.Max < 0
				orderby (int)pair.Item1.Type
				select pair).ToList<ValueTuple<AdventureParameterData, AdventureParameterValue>>();
				this.globalParameterPanel.SetDataCount(this._globalParameterList.Count);
			}
		}

		// Token: 0x0600A2BC RID: 41660 RVA: 0x004C0ED0 File Offset: 0x004BF0D0
		private void OnRenderGlobalParameterItem(int index, GameObject go)
		{
			AdventureRemakeParameterItem item = go.GetComponent<AdventureRemakeParameterItem>();
			ValueTuple<AdventureParameterData, AdventureParameterValue> valueTuple = this._globalParameterList[index];
			AdventureParameterData parameterData = valueTuple.Item1;
			AdventureParameterValue parameterValue = valueTuple.Item2;
			item.SetValue(parameterData, parameterValue, true, index == this._globalParameterList.Count - 1);
		}

		// Token: 0x1700110A RID: 4362
		// (get) Token: 0x0600A2BD RID: 41661 RVA: 0x004C0F18 File Offset: 0x004BF118
		public List<ViewAdventureRemake.ElementDisplayItem> DisplayItems
		{
			get
			{
				return this._displayItems;
			}
		}

		// Token: 0x0600A2BE RID: 41662 RVA: 0x004C0F20 File Offset: 0x004BF120
		private void RefreshAdventureElement()
		{
			this.RefreshElementMapData();
			this.RefreshElementMapShow();
			this.RefreshElementPanel(this.AdventureTaiwu.Index);
		}

		// Token: 0x0600A2BF RID: 41663 RVA: 0x004C0F43 File Offset: 0x004BF143
		private void RefreshTaiwuInfo()
		{
			this.RefreshTaiwuProgress();
			this.RefreshTaiwuState();
			this.RefreshTaiwuInfluence();
		}

		// Token: 0x0600A2C0 RID: 41664 RVA: 0x004C0F5C File Offset: 0x004BF15C
		private void RefreshElementMapData()
		{
			this._allElementMap.Clear();
			foreach (KeyValuePair<AdventureBlockIndex, List<AdventureElement>> keyValuePair in this._visibleElementMap)
			{
				AdventureBlockIndex adventureBlockIndex;
				List<AdventureElement> list3;
				keyValuePair.Deconstruct(out adventureBlockIndex, out list3);
				AdventureBlockIndex index = adventureBlockIndex;
				List<AdventureElement> list = list3;
				foreach (AdventureElement element in list)
				{
					this._prevPawnCoordinates[element.Id] = index;
				}
			}
			this._preVisibleElementMap = this._visibleElementMap.ToDictionary((KeyValuePair<AdventureBlockIndex, List<AdventureElement>> kvp) => kvp.Key, (KeyValuePair<AdventureBlockIndex, List<AdventureElement>> kvp) => new List<AdventureElement>(kvp.Value));
			this._visibleElementMap.Clear();
			List<AdventureElement> elements = this.AdventureRuntime.GetAllElements().ToList<AdventureElement>();
			foreach (AdventureElement element2 in elements)
			{
				bool flag = this._allElementMap.ContainsKey(element2.Index);
				if (flag)
				{
					this._allElementMap[element2.Index].Add(element2);
				}
				else
				{
					this._allElementMap.Add(element2.Index, new List<AdventureElement>
					{
						element2
					});
				}
				bool visible = element2.Visible;
				if (visible)
				{
					bool flag2 = this._visibleElementMap.ContainsKey(element2.Index);
					if (flag2)
					{
						this._visibleElementMap[element2.Index].Add(element2);
					}
					else
					{
						this._visibleElementMap.Add(element2.Index, new List<AdventureElement>
						{
							element2
						});
					}
				}
			}
			foreach (KeyValuePair<AdventureBlockIndex, List<AdventureElement>> keyValuePair in this._visibleElementMap)
			{
				AdventureBlockIndex adventureBlockIndex;
				List<AdventureElement> list3;
				keyValuePair.Deconstruct(out adventureBlockIndex, out list3);
				AdventureBlockIndex index2 = adventureBlockIndex;
				List<AdventureElement> list2 = list3;
				foreach (AdventureElement element3 in list2)
				{
					AdventureBlockIndex prevIndex;
					bool flag3 = !this._prevPawnCoordinates.TryGetValue(element3.Id, out prevIndex) || index2.Equals(prevIndex);
					if (!flag3)
					{
						this._movingPawns[element3.Id] = new ValueTuple<AdventureBlockIndex, AdventureBlockIndex>(prevIndex, index2);
					}
				}
			}
		}

		// Token: 0x0600A2C1 RID: 41665 RVA: 0x004C1268 File Offset: 0x004BF468
		private Dictionary<AdventureBlockIndex, List<AdventureElement>> GetElementMap()
		{
			return GMFunc.AdventureRemakeShowAllElement ? this._allElementMap : this._visibleElementMap;
		}

		// Token: 0x0600A2C2 RID: 41666 RVA: 0x004C1290 File Offset: 0x004BF490
		private void RefreshElementMapShow()
		{
			Dictionary<AdventureBlockIndex, List<AdventureElement>> elementMap = this.GetElementMap();
			for (int i = 0; i < (int)this._mapSize; i++)
			{
				for (int j = 0; j < (int)this._mapSize; j++)
				{
					bool flag = !this.IsRenderIndexInner(i, j);
					if (!flag)
					{
						AdventureUnitNormal unitNormal = this._unitNormalRenderArray2D[i, j];
						for (int k = 0; k < 9; k++)
						{
							AdventureUnitMicro unitMicro = unitNormal.GetUnitMicro(k);
							bool flag2 = unitMicro == null;
							if (!flag2)
							{
								AdventureBlockIndex dataBlockIndex = this.RenderIndexToDataIndex(unitMicro.RenderBlockIndex);
								bool inCloud = this.BlockInCloud(dataBlockIndex);
								bool flag3 = inCloud;
								if (!flag3)
								{
									TemplatedContainerAssemblyNew elementIconList = unitMicro.elementsHolder;
									AdventureRemakeParamProgressTempInfo paramProgressInfo = unitMicro.paramProgressTempInfo;
									TemplatedContainerAssemblyNew paramStateHolder = unitMicro.paramStateHolder;
									CImage influenceIcon = unitMicro.influenceIcon;
									List<AdventureElement> elementList;
									bool flag4 = elementMap.TryGetValue(dataBlockIndex, out elementList) && elementList.Count > 0;
									if (flag4)
									{
										ValueTuple<List<AdventureElement>, List<AdventureElement>> sortingElementsData = this.GetSortingElementsData(elementList);
										List<AdventureElement> elementsIgnoreSorting = sortingElementsData.Item1;
										List<AdventureElement> elementsNotIgnoreSorting = sortingElementsData.Item2;
										bool flag5 = elementsIgnoreSorting.Count > 0;
										if (flag5)
										{
											ViewAdventureRemake.<>c__DisplayClass229_2 CS$<>8__locals3 = new ViewAdventureRemake.<>c__DisplayClass229_2();
											ViewAdventureRemake.<>c__DisplayClass229_2 CS$<>8__locals4 = CS$<>8__locals3;
											List<AdventureElement> elementsIgnoreSorting2 = elementsIgnoreSorting;
											CS$<>8__locals4.elementFirst = elementsIgnoreSorting2[elementsIgnoreSorting2.Count - 1];
											AdventureElementData elementDataFirst = AdventureRemakeModel.Core.GetAdventureElementData(CS$<>8__locals3.elementFirst.CoreId);
											int remainTime;
											AdventureActionData elementActionData = this.AdventureRuntime.QueryElementActionData(CS$<>8__locals3.elementFirst, out remainTime);
											paramProgressInfo.gameObject.SetActive(elementActionData != null);
											bool flag6 = elementActionData != null;
											if (flag6)
											{
												paramProgressInfo.txtMeshParamName.SetText(elementActionData.Name, true);
												paramProgressInfo.imgParamProgress.fillAmount = AdventureActionProgress.GetActionProgress(remainTime, elementActionData.Time);
												paramProgressInfo.imgParamProgressIcon.SetSprite(ViewAdventureRemake.GetElementParamStateIconName(elementActionData.Icon, false), false, null);
											}
											List<AdventureParameterData> influenceParams = (from p in elementDataFirst.Parameters
											where !string.IsNullOrEmpty(p.Name) && p.Type == EAdventureParameterType.Influence && CS$<>8__locals3.elementFirst.GetParameter(p.Key).Current >= 0
											select p).ToList<AdventureParameterData>();
											bool hasInfluence = influenceParams.Any<AdventureParameterData>();
											influenceIcon.gameObject.SetActive(hasInfluence);
											bool flag7 = hasInfluence;
											if (flag7)
											{
												influenceIcon.SetSprite(ViewAdventureRemake.GetElementParamStateIconName(influenceParams.First<AdventureParameterData>().Icon, false), true, null);
											}
											List<AdventureParameterData> stateParamList = (from e in CS$<>8__locals3.elementFirst.Core.Parameters
											where e != null && !string.IsNullOrEmpty(e.Name) && ((e.Type == EAdventureParameterType.State && CS$<>8__locals3.elementFirst.GetParameter(e.Key).Current > 0) || CS$<>8__locals3.elementFirst.GetParameter(e.Key).Max < 0)
											select e).ToList<AdventureParameterData>();
											paramStateHolder.gameObject.SetActive(stateParamList.Count > 0);
											bool flag8 = stateParamList.Count > 0;
											if (flag8)
											{
												AdventureElementStateHolder elementStateHolder = paramStateHolder.GetComponent<AdventureElementStateHolder>();
												elementStateHolder.RefreshDisplay(stateParamList, CS$<>8__locals3.elementFirst);
											}
										}
										elementIconList.gameObject.SetActive(elementsIgnoreSorting.Count > 0);
										elementIconList.Rebuild<RectTransform>(elementsIgnoreSorting.Count, delegate(RectTransform goElementIcon, int index)
										{
											AdventureElement adventureElement = elementsIgnoreSorting[index];
											AdventureElementData elementData = AdventureRemakeModel.Core.GetAdventureElementData(adventureElement.CoreId);
											string icon = ViewAdventureRemake.GetElementIcon(elementData, adventureElement);
											CImage imgElementIcon = goElementIcon.GetComponent<AdventureRemakeElementIconTempInfo>().imgIcon;
											imgElementIcon.SetSprite(icon, true, null);
											AdventureElementVertexModifier modifier = imgElementIcon.gameObject.GetOrAddComponent<AdventureElementVertexModifier>();
											modifier.GridIndex = unitMicro.RenderBlockIndex;
											bool flag10 = this._movingPawns.ContainsKey(adventureElement.Id);
											if (flag10)
											{
												this._movingPawnActualImages[adventureElement.Id] = goElementIcon.gameObject;
												goElementIcon.gameObject.SetActive(false);
											}
											else
											{
												goElementIcon.gameObject.SetActive(true);
											}
											bool lightDataHasValue = elementData.LightDataHasValue;
											if (lightDataHasValue)
											{
												AdventurePointLight elementLight = goElementIcon.gameObject.GetOrAddComponent<AdventurePointLight>();
												AdventureLightData lData = elementData.LightData;
												elementLight.LightColor = ViewAdventureRemake.TryParseLightingColor(lData.ColorInHex, Color.white);
												elementLight.Intensity = lData.Strength;
												elementLight.VirtualZ = lData.Height;
												elementLight.Range = (float)adventureElement.GetParameterOrDefault("LightPointRange", 2).Current;
												elementLight.NoRangeClamp = ((float)adventureElement.GetParameterOrDefault("LightNoRangeClamp", 0).Current >= 0.5f);
												elementLight.BlockIndex = unitMicro.RenderBlockIndex;
												elementLight.enabled = true;
											}
											else
											{
												AdventurePointLight elementLight2 = goElementIcon.GetComponent<AdventurePointLight>();
												bool flag11 = elementLight2 != null;
												if (flag11)
												{
													elementLight2.enabled = false;
												}
											}
										});
										bool flag9 = elementsNotIgnoreSorting.Count >= 1;
										if (flag9)
										{
											TemplatedContainerAssemblyNew overlapHolder = this.GetOrCreateOverlapHolder(unitMicro);
											overlapHolder.gameObject.SetActive(true);
											AdventureOverlapElement overlapElement = overlapHolder.GetComponent<AdventureOverlapElement>();
											overlapHolder.Rebuild<RectTransform>(elementsNotIgnoreSorting.Count, delegate(RectTransform goOverlapElement, int index)
											{
												CImage omittedIcon = goOverlapElement.GetComponent<AdventureRemakeOverlapElementTempInfo>().imgIcon;
												AdventureElement omittedElement = elementsNotIgnoreSorting[index];
												AdventureElementData omittedElementData = AdventureRemakeModel.Core.GetAdventureElementData(omittedElement.CoreId);
												string iconName = ViewAdventureRemake.GetElementIcon(omittedElementData, omittedElement);
												AdventureDialog.SetElementAvatarIcon(iconName, omittedIcon);
												AdventureElementVertexModifier modifier = omittedIcon.gameObject.GetOrAddComponent<AdventureElementVertexModifier>();
												modifier.GridIndex = unitMicro.RenderBlockIndex;
											});
											overlapElement.RefreshDisplay(elementsNotIgnoreSorting);
										}
										else
										{
											this.ReturnOverlapHolderToPool(unitMicro);
										}
									}
									else
									{
										elementIconList.gameObject.SetActive(false);
										this.ReturnOverlapHolderToPool(unitMicro);
										paramProgressInfo.gameObject.SetActive(false);
										paramStateHolder.gameObject.SetActive(false);
										influenceIcon.gameObject.SetActive(false);
									}
									this.RefreshViewEffect(unitMicro);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x0600A2C3 RID: 41667 RVA: 0x004C16DC File Offset: 0x004BF8DC
		private TemplatedContainerAssemblyNew GetOrCreateOverlapHolder(AdventureUnitMicro micro)
		{
			bool flag = micro.overlapSlot.childCount > 0;
			if (flag)
			{
				TemplatedContainerAssemblyNew existing = micro.overlapSlot.GetChild(0).GetComponent<TemplatedContainerAssemblyNew>();
				bool flag2 = existing != null;
				if (flag2)
				{
					micro.overlapSlot.gameObject.SetActive(true);
					return existing;
				}
			}
			GameObject go = this._overlapHolderPool.GetObject();
			TemplatedContainerAssemblyNew holder = go.GetComponent<TemplatedContainerAssemblyNew>();
			go.transform.SetParent(micro.overlapSlot, false);
			go.transform.localPosition = Vector3.zero;
			go.transform.localScale = Vector3.one;
			micro.overlapSlot.gameObject.SetActive(true);
			return holder;
		}

		// Token: 0x0600A2C4 RID: 41668 RVA: 0x004C1798 File Offset: 0x004BF998
		private void ReturnOverlapHolderToPool(AdventureUnitMicro micro)
		{
			bool flag = micro.overlapSlot == null || micro.overlapSlot.childCount == 0;
			if (!flag)
			{
				GameObject existing = micro.overlapSlot.GetChild(0).gameObject;
				existing.transform.SetParent(null, false);
				this._overlapHolderPool.DestroyObject(existing);
				micro.overlapSlot.gameObject.SetActive(false);
			}
		}

		// Token: 0x0600A2C5 RID: 41669 RVA: 0x004C180C File Offset: 0x004BFA0C
		public static string GetElementIcon(AdventureElementData elementData, AdventureElement adventureElement)
		{
			string icon = elementData.Icon;
			bool flag = adventureElement.Visible && !adventureElement.VisibleByDefault && elementData.VisibleCondition.Count > 0;
			if (flag)
			{
				AdventureElementVisibleData visibleData = elementData.VisibleCondition[adventureElement.VisibleIndex];
				bool flag2 = !string.IsNullOrEmpty(visibleData.VisibleIcon);
				if (flag2)
				{
					icon = visibleData.VisibleIcon;
				}
			}
			return icon;
		}

		// Token: 0x0600A2C6 RID: 41670 RVA: 0x004C187C File Offset: 0x004BFA7C
		private string GetElementIcon(AdventureElementData elementData, int visibleIndex)
		{
			string icon = elementData.Icon;
			bool flag = visibleIndex >= 0 && visibleIndex != int.MaxValue && elementData.VisibleCondition.Count > 0;
			if (flag)
			{
				AdventureElementVisibleData visibleData = elementData.VisibleCondition[visibleIndex];
				bool flag2 = !string.IsNullOrEmpty(visibleData.VisibleIcon);
				if (flag2)
				{
					icon = visibleData.VisibleIcon;
				}
			}
			return icon;
		}

		// Token: 0x0600A2C7 RID: 41671 RVA: 0x004C18E4 File Offset: 0x004BFAE4
		private void RefreshElementPanel(AdventureBlockIndex dataIndex)
		{
			this._displayItems.Clear();
			this._needRenderDataIndex = dataIndex;
			Dictionary<AdventureBlockIndex, List<AdventureElement>> elementMap = this.GetElementMap();
			List<AdventureElement> value;
			bool flag = elementMap.TryGetValue(dataIndex, out value);
			if (flag)
			{
				IOrderedEnumerable<AdventureElement> sortedElements = from e in value
				orderby AdventureRemakeModel.Core.GetAdventureElementData(e.CoreId).VisiblePriority
				select e;
				foreach (AdventureElement element in sortedElements)
				{
					this._displayItems.Add(ViewAdventureRemake.ElementDisplayItem.FromElement(element));
				}
			}
			bool flag2 = this.AdventureRuntime.IsExitPoint(dataIndex);
			if (flag2)
			{
				this._displayItems.Add(ViewAdventureRemake.ElementDisplayItem.CreateExitItem(dataIndex));
			}
			this._elementPanel.transform.parent.gameObject.SetActive(this._displayItems.Count > 0);
			this.RefreshElementPanelCount();
			this.countLabel.SetText(this._displayItems.Count.ToString(), true);
		}

		// Token: 0x0600A2C8 RID: 41672 RVA: 0x004C1A08 File Offset: 0x004BFC08
		private void RefreshElementPanelCount()
		{
			bool flag = this._elementPanel.totalCount != this._displayItems.Count;
			if (flag)
			{
				this._elementPanel.totalCount = this._displayItems.Count;
				this._elementPanel.RefillCells(0, false);
			}
			else
			{
				this._elementPanel.RefreshCells();
			}
		}

		// Token: 0x0600A2C9 RID: 41673 RVA: 0x004C1A6C File Offset: 0x004BFC6C
		private void OnRenderElementItem(Transform item, int index)
		{
			bool flag = this._displayItems == null || this._displayItems.Count == 0 || index >= this._displayItems.Count;
			if (!flag)
			{
				ViewAdventureRemake.ElementDisplayItem displayItem = this._displayItems[index];
				AdventureBlockIndex taiwuDataCoord = this.RenderIndexToDataIndex(this.TaiwuRenderBlockIndex);
				bool isOnBlock = taiwuDataCoord == displayItem.BlockIndex;
				AdventureElementTemplate template = item.GetComponent<AdventureElementTemplate>();
				bool isExitItem = displayItem.IsExitItem;
				if (isExitItem)
				{
					template.RefreshExit(displayItem, isOnBlock);
					PointerTrigger pointerTrigger = template.pointerTrigger;
					if (pointerTrigger != null)
					{
						pointerTrigger.EnterEvent.ResetListener(delegate()
						{
							template.interactHover.gameObject.SetActive(true);
						});
					}
					PointerTrigger pointerTrigger2 = template.pointerTrigger;
					if (pointerTrigger2 != null)
					{
						pointerTrigger2.ExitEvent.ResetListener(delegate()
						{
							template.interactHover.gameObject.SetActive(false);
						});
					}
				}
				else
				{
					int remainTime;
					AdventureActionData actionData = this.AdventureRuntime.QueryElementActionData(displayItem.Element, out remainTime);
					bool canInteract = template.RefreshDisplay(displayItem, displayItem.Element, taiwuDataCoord, actionData, remainTime, delegate
					{
						bool flag2 = this.startSelectElement;
						if (flag2)
						{
							this.SelectElement(displayItem.Element.Id);
						}
						else
						{
							AdventureDomainMethod.Call.InteractElement(this.Element.GameDataListenerId, displayItem.Element.Id);
						}
					}, this.AdventureRuntime, this.startSelectElement);
					PointerTrigger pointerTrigger3 = template.pointerTrigger;
					if (pointerTrigger3 != null)
					{
						Func<AdventureParameterData, bool> <>9__5;
						pointerTrigger3.EnterEvent.ResetListener(delegate()
						{
							template.interactHover.gameObject.SetActive(canInteract);
							ValueTuple<int, int> elementRange = this.GetElementRange(displayItem.Element);
							int range = elementRange.Item1;
							int style = elementRange.Item2;
							AdventureElementData data = AdventureRemakeModel.Core.GetAdventureElementData(displayItem.Element.CoreId);
							Color influenceBlockColor = Color.white;
							Color influenceEdgeColor = Color.white;
							bool showBlock = false;
							bool showEdge = false;
							IEnumerable<AdventureParameterData> parameters = data.Parameters;
							Func<AdventureParameterData, bool> predicate;
							if ((predicate = <>9__5) == null)
							{
								predicate = (<>9__5 = ((AdventureParameterData p) => !string.IsNullOrEmpty(p.Name) && p.Type == EAdventureParameterType.Influence && displayItem.Element.GetParameter(p.Key).Current >= 0));
							}
							List<AdventureParameterData> influenceList = parameters.Where(predicate).ToList<AdventureParameterData>();
							bool flag2 = influenceList.Count > 0;
							if (flag2)
							{
								showBlock = ColorUtility.TryParseHtmlString(influenceList[0].InfluenceBlockColorHex, out influenceBlockColor);
								showEdge = ColorUtility.TryParseHtmlString(influenceList[0].InfluenceEdgeColorHex, out influenceEdgeColor);
							}
							this.ShowElementRange(this.DataIndexToRenderIndex(displayItem.Element.Index), range, influenceBlockColor, influenceEdgeColor, showBlock, showEdge, style, false);
						});
					}
					PointerTrigger pointerTrigger4 = template.pointerTrigger;
					if (pointerTrigger4 != null)
					{
						pointerTrigger4.ExitEvent.ResetListener(delegate()
						{
							template.interactHover.gameObject.SetActive(false);
							this.HideElementRange();
						});
					}
				}
			}
		}

		// Token: 0x0600A2CA RID: 41674 RVA: 0x004C1C60 File Offset: 0x004BFE60
		private void TemporaryItemRender(int index, GameObject item)
		{
			AdventureTemporyItemLine itemLine = item.GetComponent<AdventureTemporyItemLine>();
			itemLine.RefreshDisplay(this._chunkTemporaryItemDisplayDataList[index]);
		}

		// Token: 0x0600A2CB RID: 41675 RVA: 0x004C1C88 File Offset: 0x004BFE88
		private void RefreshItemScrollView()
		{
			this._temporaryItemList = this.AdventureRuntime.GetTemporaryItemsTaiwu().ToList<GameData.Domains.Adventure.AdventureItem>();
			this._temporaryItemKeyList = (from e in this.AdventureRuntime.GetTemporaryItemsTaiwu()
			where e.ItemCount > 0
			select e.ItemKey).ToList<ItemKey>();
			bool flag = this._temporaryItemKeyList.Count > 0;
			if (flag)
			{
				ItemDomainMethod.AsyncCall.GetItemDisplayDataList(this, this._temporaryItemKeyList, -1, delegate(int offset, RawDataPool dataPool)
				{
					Serializer.Deserialize(dataPool, offset, ref this._temporaryItemDisplayDataList);
					foreach (ItemDisplayData itemDisplayData in this._temporaryItemDisplayDataList)
					{
						foreach (GameData.Domains.Adventure.AdventureItem item in this._temporaryItemList)
						{
							bool flag2 = itemDisplayData.Key.Equals(item.ItemKey);
							if (flag2)
							{
								itemDisplayData.Amount = item.ItemCount;
							}
						}
					}
					this._chunkTemporaryItemDisplayDataList = ViewAdventureRemake.Chunk<ItemDisplayData>(this._temporaryItemDisplayDataList, 5).ToList<List<ItemDisplayData>>();
					InfinityScroll infinityScroll2 = this._temporaryItemVerticalScrollView;
					if (infinityScroll2 != null)
					{
						infinityScroll2.SetDataCount(this._chunkTemporaryItemDisplayDataList.Count);
					}
					this.temporaryItemEmpty.gameObject.SetActive(this._chunkTemporaryItemDisplayDataList.Count <= 0);
				});
			}
			else
			{
				this._temporaryItemVerticalScrollView = this.temporaryItemVerticalScrollView;
				InfinityScroll infinityScroll = this._temporaryItemVerticalScrollView;
				if (infinityScroll != null)
				{
					infinityScroll.SetDataCount(0);
				}
				this.temporaryItemEmpty.gameObject.SetActive(true);
			}
		}

		// Token: 0x0600A2CC RID: 41676 RVA: 0x004C1D70 File Offset: 0x004BFF70
		private void SwitchTemporaryItemLocation(bool show)
		{
			RectTransform temporaryItemBtnRect = this.temporaryItemBtn.GetComponent<RectTransform>();
			temporaryItemBtnRect.localPosition = new Vector3(temporaryItemBtnRect.localPosition.x, show ? 355f : -15f, 0f);
			this.temporaryItemVerticalScrollView.gameObject.SetActive(show);
			this.temporaryItemExpandBtn.localEulerAngles = (show ? Vector3.zero : new Vector3(0f, 0f, 180f));
		}

		// Token: 0x0600A2CD RID: 41677 RVA: 0x004C1DF4 File Offset: 0x004BFFF4
		private void AdventureRemakeDictChange(ArgumentBox argBox)
		{
			bool flag = !this.AdventureRemakeModel.AdventureRemakeDict.Keys.Contains(this._adventureId);
			if (!flag)
			{
				bool notInAdventure = this.AdventureTaiwu.NotInAdventure;
				if (!notInAdventure)
				{
					bool flag2 = this._isRefreshing || this._showTransitionAnim;
					if (flag2)
					{
						this.RefreshDisplay();
					}
					else
					{
						base.StartCoroutine(this.CoRefreshDisplay());
					}
				}
			}
		}

		// Token: 0x0600A2CE RID: 41678 RVA: 0x004C1E63 File Offset: 0x004C0063
		private IEnumerator CoRefreshElementMapShowBatch()
		{
			Dictionary<AdventureBlockIndex, List<AdventureElement>> elementMap = this.GetElementMap();
			int count = 0;
			int num;
			for (int i = 0; i < (int)this._mapSize; i = num + 1)
			{
				for (int j = 0; j < (int)this._mapSize; j = num + 1)
				{
					bool flag = !this.IsRenderIndexInner(i, j);
					if (!flag)
					{
						AdventureUnitNormal unitNormal = this._unitNormalRenderArray2D[i, j];
						int k = 0;
						while (k < 9)
						{
							ViewAdventureRemake.<>c__DisplayClass247_0 CS$<>8__locals1 = new ViewAdventureRemake.<>c__DisplayClass247_0();
							CS$<>8__locals1.<>4__this = this;
							CS$<>8__locals1.unitMicro = unitNormal.GetUnitMicro(k);
							bool flag2 = CS$<>8__locals1.unitMicro == null;
							if (!flag2)
							{
								AdventureBlockIndex dataBlockIndex = this.RenderIndexToDataIndex(CS$<>8__locals1.unitMicro.RenderBlockIndex);
								bool inCloud = this.BlockInCloud(dataBlockIndex);
								bool flag3 = inCloud;
								if (!flag3)
								{
									TemplatedContainerAssemblyNew elementIconList = CS$<>8__locals1.unitMicro.elementsHolder;
									AdventureRemakeParamProgressTempInfo paramProgressInfo = CS$<>8__locals1.unitMicro.paramProgressTempInfo;
									TemplatedContainerAssemblyNew paramStateHolder = CS$<>8__locals1.unitMicro.paramStateHolder;
									CImage influenceIcon = CS$<>8__locals1.unitMicro.influenceIcon;
									List<AdventureElement> elementList;
									bool flag4 = elementMap.TryGetValue(dataBlockIndex, out elementList) && elementList.Count > 0;
									if (flag4)
									{
										ViewAdventureRemake.<>c__DisplayClass247_1 CS$<>8__locals2 = new ViewAdventureRemake.<>c__DisplayClass247_1();
										CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
										ValueTuple<List<AdventureElement>, List<AdventureElement>> sortingElementsData = this.GetSortingElementsData(elementList);
										CS$<>8__locals2.elementsIgnoreSorting = sortingElementsData.Item1;
										CS$<>8__locals2.elementsNotIgnoreSorting = sortingElementsData.Item2;
										bool flag5 = CS$<>8__locals2.elementsIgnoreSorting.Count > 0;
										if (flag5)
										{
											ViewAdventureRemake.<>c__DisplayClass247_2 CS$<>8__locals3 = new ViewAdventureRemake.<>c__DisplayClass247_2();
											ViewAdventureRemake.<>c__DisplayClass247_2 CS$<>8__locals4 = CS$<>8__locals3;
											List<AdventureElement> elementsIgnoreSorting = CS$<>8__locals2.elementsIgnoreSorting;
											CS$<>8__locals4.elementFirst = elementsIgnoreSorting[elementsIgnoreSorting.Count - 1];
											AdventureElementData elementDataFirst = AdventureRemakeModel.Core.GetAdventureElementData(CS$<>8__locals3.elementFirst.CoreId);
											int remainTime;
											AdventureActionData elementActionData = this.AdventureRuntime.QueryElementActionData(CS$<>8__locals3.elementFirst, out remainTime);
											paramProgressInfo.gameObject.SetActive(elementActionData != null);
											bool flag6 = elementActionData != null;
											if (flag6)
											{
												paramProgressInfo.txtMeshParamName.SetText(elementActionData.Name, true);
												paramProgressInfo.imgParamProgress.fillAmount = AdventureActionProgress.GetActionProgress(remainTime, elementActionData.Time);
												paramProgressInfo.imgParamProgressIcon.SetSprite(ViewAdventureRemake.GetElementParamStateIconName(elementActionData.Icon, false), false, null);
											}
											List<AdventureParameterData> influenceParams = (from p in elementDataFirst.Parameters
											where !string.IsNullOrEmpty(p.Name) && p.Type == EAdventureParameterType.Influence && CS$<>8__locals3.elementFirst.GetParameter(p.Key).Current >= 0
											select p).ToList<AdventureParameterData>();
											bool hasInfluence = influenceParams.Any<AdventureParameterData>();
											influenceIcon.gameObject.SetActive(hasInfluence);
											bool flag7 = hasInfluence;
											if (flag7)
											{
												influenceIcon.SetSprite(ViewAdventureRemake.GetElementParamStateIconName(influenceParams.First<AdventureParameterData>().Icon, false), true, null);
											}
											List<AdventureParameterData> stateParamList = (from e in CS$<>8__locals3.elementFirst.Core.Parameters
											where e != null && !string.IsNullOrEmpty(e.Name) && ((e.Type == EAdventureParameterType.State && CS$<>8__locals3.elementFirst.GetParameter(e.Key).Current > 0) || CS$<>8__locals3.elementFirst.GetParameter(e.Key).Max < 0)
											select e).ToList<AdventureParameterData>();
											paramStateHolder.gameObject.SetActive(stateParamList.Count > 0);
											bool flag8 = stateParamList.Count > 0;
											if (flag8)
											{
												AdventureElementStateHolder elementStateHolder = paramStateHolder.GetComponent<AdventureElementStateHolder>();
												elementStateHolder.RefreshDisplay(stateParamList, CS$<>8__locals3.elementFirst);
												elementStateHolder = null;
											}
											CS$<>8__locals3 = null;
											elementDataFirst = null;
											elementActionData = null;
											influenceParams = null;
											stateParamList = null;
										}
										elementIconList.gameObject.SetActive(CS$<>8__locals2.elementsIgnoreSorting.Count > 0);
										elementIconList.Rebuild<RectTransform>(CS$<>8__locals2.elementsIgnoreSorting.Count, delegate(RectTransform goElementIcon, int idx)
										{
											AdventureElement adventureElement = CS$<>8__locals2.elementsIgnoreSorting[idx];
											AdventureElementData elementData = AdventureRemakeModel.Core.GetAdventureElementData(adventureElement.CoreId);
											string icon = ViewAdventureRemake.GetElementIcon(elementData, adventureElement);
											CImage imgElementIcon = goElementIcon.GetComponent<AdventureRemakeElementIconTempInfo>().imgIcon;
											imgElementIcon.SetSprite(icon, true, null);
											AdventureElementVertexModifier modifier = imgElementIcon.gameObject.GetOrAddComponent<AdventureElementVertexModifier>();
											modifier.GridIndex = CS$<>8__locals2.CS$<>8__locals1.unitMicro.RenderBlockIndex;
											bool flag11 = CS$<>8__locals2.CS$<>8__locals1.<>4__this._movingPawns.ContainsKey(adventureElement.Id);
											if (flag11)
											{
												CS$<>8__locals2.CS$<>8__locals1.<>4__this._movingPawnActualImages[adventureElement.Id] = goElementIcon.gameObject;
												goElementIcon.gameObject.SetActive(false);
											}
											else
											{
												goElementIcon.gameObject.SetActive(true);
											}
											bool lightDataHasValue = elementData.LightDataHasValue;
											if (lightDataHasValue)
											{
												AdventurePointLight elementLight = goElementIcon.gameObject.GetOrAddComponent<AdventurePointLight>();
												AdventureLightData lData = elementData.LightData;
												elementLight.LightColor = ViewAdventureRemake.TryParseLightingColor(lData.ColorInHex, Color.white);
												elementLight.Intensity = lData.Strength;
												elementLight.VirtualZ = lData.Height;
												elementLight.Range = (float)adventureElement.GetParameterOrDefault("LightPointRange", 2).Current;
												elementLight.NoRangeClamp = ((float)adventureElement.GetParameterOrDefault("LightNoRangeClamp", 0).Current >= 0.5f);
												elementLight.BlockIndex = CS$<>8__locals2.CS$<>8__locals1.unitMicro.RenderBlockIndex;
												elementLight.enabled = true;
											}
											else
											{
												AdventurePointLight elementLight2 = goElementIcon.GetComponent<AdventurePointLight>();
												bool flag12 = elementLight2 != null;
												if (flag12)
												{
													elementLight2.enabled = false;
												}
											}
										});
										bool flag9 = CS$<>8__locals2.elementsNotIgnoreSorting.Count >= 1;
										if (flag9)
										{
											TemplatedContainerAssemblyNew overlapHolder = this.GetOrCreateOverlapHolder(CS$<>8__locals2.CS$<>8__locals1.unitMicro);
											overlapHolder.gameObject.SetActive(true);
											AdventureOverlapElement overlapElement = overlapHolder.GetComponent<AdventureOverlapElement>();
											overlapHolder.Rebuild<RectTransform>(CS$<>8__locals2.elementsNotIgnoreSorting.Count, delegate(RectTransform goOverlapElement, int idx2)
											{
												CImage omittedIcon = goOverlapElement.GetComponent<AdventureRemakeOverlapElementTempInfo>().imgIcon;
												AdventureElement omittedElement = CS$<>8__locals2.elementsNotIgnoreSorting[idx2];
												AdventureElementData omittedElementData = AdventureRemakeModel.Core.GetAdventureElementData(omittedElement.CoreId);
												omittedIcon.SetSprite(ViewAdventureRemake.GetElementUIIconName(ViewAdventureRemake.GetElementIcon(omittedElementData, omittedElement)), false, null);
												AdventureElementVertexModifier modifier = omittedIcon.gameObject.GetOrAddComponent<AdventureElementVertexModifier>();
												modifier.GridIndex = CS$<>8__locals2.CS$<>8__locals1.unitMicro.RenderBlockIndex;
											});
											overlapElement.RefreshDisplay(CS$<>8__locals2.elementsNotIgnoreSorting);
											overlapHolder = null;
											overlapElement = null;
										}
										else
										{
											this.ReturnOverlapHolderToPool(CS$<>8__locals2.CS$<>8__locals1.unitMicro);
										}
										CS$<>8__locals2 = null;
									}
									else
									{
										elementIconList.gameObject.SetActive(false);
										this.ReturnOverlapHolderToPool(CS$<>8__locals1.unitMicro);
										paramProgressInfo.gameObject.SetActive(false);
										paramStateHolder.gameObject.SetActive(false);
										influenceIcon.gameObject.SetActive(false);
									}
									this.RefreshViewEffect(CS$<>8__locals1.unitMicro);
									bool flag10 = count % 30 == 0;
									if (flag10)
									{
										yield return null;
									}
									CS$<>8__locals1 = null;
									dataBlockIndex = default(AdventureBlockIndex);
									elementIconList = null;
									paramProgressInfo = null;
									paramStateHolder = null;
									influenceIcon = null;
									elementList = null;
								}
							}
							num = k;
							k = num + 1;
							num = count;
							count = num + 1;
						}
						unitNormal = null;
					}
					num = j;
				}
				num = i;
			}
			yield break;
		}

		// Token: 0x0600A2CF RID: 41679 RVA: 0x004C1E72 File Offset: 0x004C0072
		private IEnumerator CoRefreshDisplay()
		{
			this._isRefreshing = true;
			this.RefreshExitIcon();
			this.RefreshItemScrollView();
			this.RefreshGlobalParameterPanel();
			this.RefreshElementMapData();
			yield return this.CoRefreshElementMapShowBatch();
			this.RefreshElementPanel(this.AdventureTaiwu.Index);
			this.RefreshTaiwuInfo();
			this.RefreshViewValue();
			this.RefreshAllMicroBlockColor();
			this.RefreshStaticPointLights();
			this.PlayMovingPawnAnim();
			this.PlayElementsFadeAnim();
			this.RefreshCloud();
			this.RefreshGlobalEffect();
			this._isRefreshing = false;
			yield break;
		}

		// Token: 0x0600A2D0 RID: 41680 RVA: 0x004C1E84 File Offset: 0x004C0084
		private void RefreshDisplay()
		{
			this.RefreshExitIcon();
			this.RefreshItemScrollView();
			this.RefreshGlobalParameterPanel();
			this.RefreshAdventureElement();
			this.RefreshTaiwuInfo();
			this.RefreshViewValue();
			this.RefreshAllMicroBlockColor();
			this.RefreshStaticPointLights();
			this.PlayMovingPawnAnim();
			this.PlayElementsFadeAnim();
			this.RefreshCloud();
		}

		// Token: 0x0600A2D1 RID: 41681 RVA: 0x004C1EE0 File Offset: 0x004C00E0
		private void ChangeToDarkCircle(bool changeToDark)
		{
			Dictionary<int, List<AdventureUnitPeripheral>> circleUnits = new Dictionary<int, List<AdventureUnitPeripheral>>();
			for (byte i = 0; i < this._mapSize; i += 1)
			{
				for (byte j = 0; j < this._mapSize; j += 1)
				{
					AdventureUnitPeripheral peripheral = this._unitPeripheralArray2D[(int)i, (int)j];
					bool flag = peripheral == null || !this.IsRenderIndexExist(i, j);
					if (!flag)
					{
						int circleIndex = this.GetCircleIndex(new ByteCoordinate(i, j));
						List<AdventureUnitPeripheral> list;
						bool flag2 = !circleUnits.TryGetValue(circleIndex, out list);
						if (flag2)
						{
							list = new List<AdventureUnitPeripheral>();
							circleUnits[circleIndex] = list;
						}
						list.Add(peripheral);
					}
				}
			}
			foreach (KeyValuePair<int, List<AdventureUnitPeripheral>> keyValuePair in circleUnits)
			{
				int num;
				List<AdventureUnitPeripheral> units2;
				keyValuePair.Deconstruct(out num, out units2);
				int circleIndex2 = num;
				List<AdventureUnitPeripheral> units = units2;
				float delayTime = Mathf.Min(0.3f * (float)circleIndex2, 1f);
				if (changeToDark)
				{
					DOVirtual.Float(1f, this.OuterBlockBrightness, 1.5f, delegate(float value)
					{
						this.UpdateUnitsColorParam(units, "_Brightness", value);
					}).SetEase(Ease.OutCubic).SetDelay(delayTime);
					DOVirtual.Float(1f, this.OuterBlockSaturation, 1.5f, delegate(float value)
					{
						this.UpdateUnitsColorParam(units, "_Saturation", value);
					}).SetEase(Ease.OutCubic).SetDelay(delayTime);
				}
				else
				{
					DOVirtual.Float(this.OuterBlockBrightness, 1f, 1.5f, delegate(float value)
					{
						this.UpdateUnitsColorParam(units, "_Brightness", value);
					}).SetEase(Ease.InQuint).SetDelay(delayTime);
					DOVirtual.Float(this.OuterBlockSaturation, 1f, 1.5f, delegate(float value)
					{
						this.UpdateUnitsColorParam(units, "_Saturation", value);
					}).SetEase(Ease.InQuint).SetDelay(delayTime);
				}
			}
		}

		// Token: 0x0600A2D2 RID: 41682 RVA: 0x004C2100 File Offset: 0x004C0300
		private void UpdateUnitsColorParam(List<AdventureUnitPeripheral> units, string param, float value)
		{
			bool isBrightness = param == "_Brightness";
			foreach (AdventureUnitPeripheral unit in units)
			{
				bool flag = unit == null;
				if (!flag)
				{
					this.SetUnitParam(unit, isBrightness, value);
				}
			}
		}

		// Token: 0x0600A2D3 RID: 41683 RVA: 0x004C2170 File Offset: 0x004C0370
		private void SetUnitParam(AdventureUnitPeripheral unit, bool isBrightness, float value)
		{
			if (isBrightness)
			{
				unit.SetPieceBrightness(value);
			}
			else
			{
				unit.SetPieceSaturation(value);
			}
		}

		// Token: 0x0600A2D4 RID: 41684 RVA: 0x004C2198 File Offset: 0x004C0398
		private void ResetDarkParam()
		{
			for (byte i = 0; i < this._mapSize; i += 1)
			{
				for (byte j = 0; j < this._mapSize; j += 1)
				{
					AdventureUnitPeripheral peripheral = this._unitPeripheralArray2D[(int)i, (int)j];
					bool flag = peripheral != null;
					if (flag)
					{
						peripheral.SetPieceBrightness(1f);
						peripheral.SetPieceSaturation(1f);
						BlockVolumeController bv = peripheral.GetComponent<BlockVolumeController>();
						bv.SetVolumeBrightness(1f);
						bv.SetVolumeSaturation(1f);
					}
				}
			}
		}

		// Token: 0x0600A2D5 RID: 41685 RVA: 0x004C2232 File Offset: 0x004C0432
		private void AdventureRefreshGlobalEffect(ArgumentBox box)
		{
			this.RefreshGlobalEffect();
		}

		// Token: 0x0600A2D6 RID: 41686 RVA: 0x004C223C File Offset: 0x004C043C
		private void RefreshGlobalEffect()
		{
			int templateId = -1;
			AdventureParameterValue parameter;
			bool flag = this.AdventureRuntime.TryGetParameter("ConchShipPresetKey_Adventure_Global_Particle", out parameter);
			if (flag)
			{
				templateId = parameter.Current;
			}
			bool flag2 = templateId < 0;
			if (flag2)
			{
				this.DestroyEffect();
			}
			else
			{
				this.ShowPerformanceEffect(templateId);
			}
		}

		// Token: 0x0600A2D7 RID: 41687 RVA: 0x004C2284 File Offset: 0x004C0484
		private void ShowPerformanceEffect(int templateId)
		{
			AdventureRemakePerformanceEffectItem config = AdventureRemakePerformanceEffect.Instance[templateId];
			foreach (short effectTemplateId in config.Effects)
			{
				AdventureRemakePerformanceEffectParamItem effectConfig = AdventureRemakePerformanceEffectParam.Instance[effectTemplateId];
				bool flag = effectConfig.Type == EAdventureRemakePerformanceEffectParamType.OuterBlock;
				if (flag)
				{
					this.AddOuterBlockEffect(effectConfig);
				}
				bool flag2 = effectConfig.Type == EAdventureRemakePerformanceEffectParamType.FullscreenCenterLocation;
				if (flag2)
				{
					this.AddFullscreenCenterLocationEffect(effectConfig);
				}
				bool flag3 = effectConfig.Type == EAdventureRemakePerformanceEffectParamType.FullscreenRandomLocation;
				if (flag3)
				{
					this.AddFullscreenRandomLocationEffect(effectConfig);
				}
				bool flag4 = effectConfig.Type == EAdventureRemakePerformanceEffectParamType.FrontVerticalStripe;
				if (flag4)
				{
					this.AddFrontVerticalStripeEffect(effectConfig);
				}
				bool flag5 = effectConfig.Type == EAdventureRemakePerformanceEffectParamType.BackVerticalStripe;
				if (flag5)
				{
					this.AddBackVerticalStripeEffect(effectConfig);
				}
			}
		}

		// Token: 0x0600A2D8 RID: 41688 RVA: 0x004C2368 File Offset: 0x004C0568
		private void AddOuterBlockEffect(AdventureRemakePerformanceEffectParamItem effectConfig)
		{
			int count = this._renderCoordList.Count * (int)effectConfig.PercentageCount / 100;
			List<ByteCoordinate> showEffectList = (from x in this._renderCoordList
			orderby Random.value
			select x).Take(count).ToList<ByteCoordinate>();
			string fullPath = this.GetAdventureEffectLoadPath(effectConfig.LoadName);
			ResLoader.Load<GameObject>(fullPath, delegate(GameObject go)
			{
				foreach (ByteCoordinate renderCoord in showEffectList)
				{
					bool flag = !this.IsRenderIndexInner((int)renderCoord.X, (int)renderCoord.Y);
					if (flag)
					{
						AdventureUnitPeripheral peripheral = this._unitPeripheralArray2D[(int)renderCoord.X, (int)renderCoord.Y];
						bool flag2 = peripheral == null;
						if (!flag2)
						{
							UIParticle particleHolder = peripheral.normalBlockParticleHolder;
							particleHolder.enabled = true;
							GameObject effectGo = Object.Instantiate<GameObject>(go, particleHolder.transform);
							this._waitToDestroyList.Add(effectGo);
							particleHolder.RefreshParticles();
							for (int i = 0; i < particleHolder.transform.childCount; i++)
							{
								Transform child = particleHolder.transform.GetChild(i);
								ParticleSystem particle = child.GetComponent<ParticleSystem>();
								bool flag3 = particle != null;
								if (flag3)
								{
									this.DelayCall(delegate
									{
										particle.Play();
									}, Random.value);
								}
							}
						}
					}
				}
			}, null, false);
		}

		// Token: 0x0600A2D9 RID: 41689 RVA: 0x004C23F8 File Offset: 0x004C05F8
		private void AddBackVerticalStripeEffect(AdventureRemakePerformanceEffectParamItem effectConfig)
		{
			IEnumerable<ByteCoordinate> topEdgeCoordList = this.GetTopEdge((int)this._mapSize);
			IEnumerable<ByteCoordinate> leftEdgeCoordList = this.GetLeftEdge((int)this._mapSize);
			int count = Random.Range((int)effectConfig.CountRange[0], (int)effectConfig.CountRange[1]);
			int count2 = count / 2;
			int count3 = count2;
			bool flag = count % 2 == 1;
			if (flag)
			{
				count3++;
			}
			List<ByteCoordinate> topCoordList = (from x in topEdgeCoordList
			orderby Random.value
			select x).Take(count2).ToList<ByteCoordinate>();
			List<ByteCoordinate> leftCoordList = (from x in leftEdgeCoordList
			orderby Random.value
			select x).Take(count3).ToList<ByteCoordinate>();
			leftCoordList.AddRange(topCoordList);
			string fullPath = this.GetAdventureEffectLoadPath(effectConfig.LoadName);
			ResLoader.Load<GameObject>(fullPath, delegate(GameObject go)
			{
				foreach (ByteCoordinate renderCoord in leftCoordList)
				{
					Vector2 locationPos = this.GetUnitNormalLocation(renderCoord);
					Transform template = this.InstantiateEffect(this.backParticleHolder, go, effectConfig);
					template.localPosition = locationPos;
				}
			}, null, false);
		}

		// Token: 0x0600A2DA RID: 41690 RVA: 0x004C2510 File Offset: 0x004C0710
		private void AddFrontVerticalStripeEffect(AdventureRemakePerformanceEffectParamItem effectConfig)
		{
			IEnumerable<ByteCoordinate> bottomEdgeCoordList = this.GetBottomEdge((int)this._mapSize);
			IEnumerable<ByteCoordinate> rightEdgeCoordList = this.GetRightEdge((int)this._mapSize);
			int count = Random.Range((int)effectConfig.CountRange[0], (int)effectConfig.CountRange[1]);
			int count2 = count / 2;
			int count3 = count2;
			bool flag = count % 2 == 1;
			if (flag)
			{
				count3++;
			}
			List<ByteCoordinate> bottomCoordList = (from x in bottomEdgeCoordList
			orderby Random.value
			select x).Take(count2).ToList<ByteCoordinate>();
			List<ByteCoordinate> rightCoordList = (from x in rightEdgeCoordList
			orderby Random.value
			select x).Take(count3).ToList<ByteCoordinate>();
			rightCoordList.AddRange(bottomCoordList);
			string fullPath = this.GetAdventureEffectLoadPath(effectConfig.LoadName);
			ResLoader.Load<GameObject>(fullPath, delegate(GameObject go)
			{
				foreach (ByteCoordinate renderCoord in rightCoordList)
				{
					Vector2 locationPos = this.GetUnitNormalLocation(renderCoord);
					Transform template = this.InstantiateEffect(this.frontParticleHolder, go, effectConfig);
					template.localPosition = locationPos;
				}
			}, null, false);
		}

		// Token: 0x0600A2DB RID: 41691 RVA: 0x004C2628 File Offset: 0x004C0828
		private Transform InstantiateEffect(RectTransform particleHolder, GameObject particleGo, AdventureRemakePerformanceEffectParamItem effectConfig)
		{
			Transform template = Object.Instantiate<Transform>(particleHolder.GetChild(0), particleHolder);
			template.gameObject.SetActive(true);
			GameObject particle = Object.Instantiate<GameObject>(particleGo, template);
			UIParticle templateUIParticle = template.GetComponent<UIParticle>();
			this._waitToDestroyList.Add(template.gameObject);
			templateUIParticle.scale = (float)effectConfig.Scale;
			ParticleSystem[] localParticleSystemArray = particle.GetComponentsInChildren<ParticleSystem>();
			foreach (ParticleSystem localParticleSystem in localParticleSystemArray)
			{
				ParticleSystem.MainModule main = localParticleSystem.main;
				bool flag = effectConfig.ParticleSimulationSpeed > 0f;
				if (flag)
				{
					main.simulationSpeed = effectConfig.ParticleSimulationSpeed;
				}
				bool flag2 = effectConfig.ParticleStartDelay > 0f;
				if (flag2)
				{
					ParticleSystem.MinMaxCurve constantCurve = new ParticleSystem.MinMaxCurve(effectConfig.ParticleStartDelay);
					main.startDelay = constantCurve;
				}
				float[] array2 = effectConfig.ParticleStartDelayRandom;
				bool flag3 = array2 != null && array2.Length > 0;
				if (flag3)
				{
					ParticleSystem.MinMaxCurve randomCurve = new ParticleSystem.MinMaxCurve(effectConfig.ParticleStartDelayRandom[0], effectConfig.ParticleStartDelayRandom[1]);
					main.startDelay = randomCurve;
				}
				array2 = effectConfig.ParticleStartSpeedRandom;
				bool flag4 = array2 != null && array2.Length > 0;
				if (flag4)
				{
					ParticleSystem.MinMaxCurve randomCurve2 = new ParticleSystem.MinMaxCurve(effectConfig.ParticleStartSpeedRandom[0], effectConfig.ParticleStartSpeedRandom[1]);
					main.startSpeed = randomCurve2;
				}
			}
			templateUIParticle.RefreshParticles();
			foreach (ParticleSystem localParticleSystem2 in localParticleSystemArray)
			{
				localParticleSystem2.Play();
			}
			return template;
		}

		// Token: 0x0600A2DC RID: 41692 RVA: 0x004C27B8 File Offset: 0x004C09B8
		private void AddFullscreenRandomLocationEffect(AdventureRemakePerformanceEffectParamItem effectConfig)
		{
			int edge = 150;
			int count = Random.Range((int)effectConfig.CountRange[0], (int)effectConfig.CountRange[1]);
			string fullPath = this.GetAdventureEffectLoadPath(effectConfig.LoadName);
			ResLoader.Load<GameObject>(fullPath, delegate(GameObject go)
			{
				for (int i = 0; i < count; i++)
				{
					Transform template = this.InstantiateEffect(this.fullScreenParticleHolder, go, effectConfig);
					Vector2 randomScreenPosition = new Vector2((float)Random.Range(edge, Screen.width - edge), (float)Random.Range(edge, Screen.height - edge));
					Vector2 localPoint;
					RectTransformUtility.ScreenPointToLocalPointInRectangle(this.fullScreenParticleHolder, randomScreenPosition, UIManager.Instance.UiCamera, out localPoint);
					template.localPosition = localPoint;
				}
			}, null, false);
		}

		// Token: 0x0600A2DD RID: 41693 RVA: 0x004C2834 File Offset: 0x004C0A34
		private void AddFullscreenCenterLocationEffect(AdventureRemakePerformanceEffectParamItem effectConfig)
		{
			string fullPath = this.GetAdventureEffectLoadPath(effectConfig.LoadName);
			ResLoader.Load<GameObject>(fullPath, delegate(GameObject go)
			{
				Transform template = this.InstantiateEffect(this.fullScreenParticleHolder, go, effectConfig);
				template.localPosition = Vector3.zero;
			}, null, false);
		}

		// Token: 0x0600A2DE RID: 41694 RVA: 0x004C2880 File Offset: 0x004C0A80
		private void DestroyEffect()
		{
			foreach (GameObject go in this._waitToDestroyList)
			{
				Object.Destroy(go);
			}
			AdventureUnitPeripheral[,] unitPeripheralArray2D = this._unitPeripheralArray2D;
			int upperBound = unitPeripheralArray2D.GetUpperBound(0);
			int upperBound2 = unitPeripheralArray2D.GetUpperBound(1);
			for (int i = unitPeripheralArray2D.GetLowerBound(0); i <= upperBound; i++)
			{
				for (int j = unitPeripheralArray2D.GetLowerBound(1); j <= upperBound2; j++)
				{
					AdventureUnitPeripheral peripheral = unitPeripheralArray2D[i, j];
					bool flag = peripheral != null;
					if (flag)
					{
						peripheral.UpdateParticleHolderActiveState();
					}
				}
			}
		}

		// Token: 0x0600A2DF RID: 41695 RVA: 0x004C2944 File Offset: 0x004C0B44
		private void FilterRenderCoordList(ref List<ByteCoordinate> renderCoordList)
		{
			List<ByteCoordinate> outermostLayer = new List<ByteCoordinate>();
			List<ByteCoordinate> nextOuterLayer = new List<ByteCoordinate>();
			for (int i = renderCoordList.Count - 1; i >= 0; i--)
			{
				ByteCoordinate coord = renderCoordList[i];
				int index = this.GetCircleIndex(coord);
				bool flag = index == this.OutCircleCount - 1;
				if (flag)
				{
					outermostLayer.Add(coord);
				}
				else
				{
					bool flag2 = index == this.OutCircleCount - 2;
					if (flag2)
					{
						nextOuterLayer.Add(coord);
					}
				}
			}
			outermostLayer = (from x in outermostLayer
			orderby this._random.Next()
			select x).Take(outermostLayer.Count * 70 / 100).ToList<ByteCoordinate>();
			nextOuterLayer = (from x in nextOuterLayer
			orderby this._random.Next()
			select x).Take(nextOuterLayer.Count * 40 / 100).ToList<ByteCoordinate>();
			renderCoordList = renderCoordList.Except(outermostLayer).Except(nextOuterLayer).ToList<ByteCoordinate>();
		}

		// Token: 0x0600A2E0 RID: 41696 RVA: 0x004C2A30 File Offset: 0x004C0C30
		public void Preload()
		{
			for (int i = 0; i < 100; i++)
			{
				GameObject inner = Object.Instantiate<GameObject>(this.innerBlockTemplate, this.unitRoot);
				inner.SetActive(false);
			}
			for (int j = 0; j < 300; j++)
			{
				GameObject outer = Object.Instantiate<GameObject>(this.outerBlockTemplate, this.unitRoot);
				outer.SetActive(false);
			}
		}

		// Token: 0x0600A2E1 RID: 41697 RVA: 0x004C2A9F File Offset: 0x004C0C9F
		private void AdventureRemakeFinish(ArgumentBox argBox)
		{
			this._isFinish = true;
			argBox.Get("SkipFinishAnim", out this._skipFinishAnim);
		}

		// Token: 0x0600A2E2 RID: 41698 RVA: 0x004C2ABC File Offset: 0x004C0CBC
		private void AdventureRemakeFinish()
		{
			GEvent.OnEvent(UiEvents.AdventureBanTimeBall, EasyPool.Get<ArgumentBox>().Set("AdventureBan", true));
			CommandKitBase.SetDisable(true);
			bool skipFinishAnim = this._skipFinishAnim;
			if (skipFinishAnim)
			{
				this.ExitAdventureAction();
			}
			else
			{
				this.adventureRemakeFinish.Show(new Action(this.ExitAdventureAction), false);
			}
		}

		// Token: 0x0600A2E3 RID: 41699 RVA: 0x004C2B1D File Offset: 0x004C0D1D
		private void ExitAdventureAction()
		{
			GEvent.OnEvent(UiEvents.AdventureBanTimeBall, EasyPool.Get<ArgumentBox>().Set("AdventureBan", false));
			CommandKitBase.SetDisable(false);
			this._elementDeleteAnimDict.Clear();
			this.AdventureExit();
		}

		// Token: 0x0600A2E4 RID: 41700 RVA: 0x004C2B5C File Offset: 0x004C0D5C
		private void AdventureExit()
		{
			this.DestroyEffect();
			this.RemoveBlockEffect();
			this.RemoveAllEvent();
			UIManager.Instance.StackBack(null);
			GEvent.OnEvent(UiEvents.AdventureRemakeExit, null);
			GEvent.OnEvent(UiEvents.TaskGroupDataUpdated, null);
		}

		// Token: 0x0600A2E5 RID: 41701 RVA: 0x004C2BB0 File Offset: 0x004C0DB0
		private void TaiwuNotInAdventure()
		{
			bool isFinish = this._isFinish;
			if (isFinish)
			{
				this.AdventureRemakeFinish();
			}
			else
			{
				this.AdventureExit();
			}
		}

		// Token: 0x0600A2E6 RID: 41702 RVA: 0x004C2BD8 File Offset: 0x004C0DD8
		private void AdventureElementAlertAnim(ArgumentBox argBox)
		{
			int elementId;
			argBox.Get("ElementId", out elementId);
			AdventureElement element = this.AdventureRuntime.GetElement(elementId);
			bool flag = element == null || !element.Visible;
			if (!flag)
			{
				AdventureBlockIndex elementIndex = element.Index;
				AdventureBlockIndex renderIndex = this.DataIndexToRenderIndex(elementIndex);
				AdventureUnitMicro unitMicro = this.GetUnitMicro(renderIndex);
				CanvasGroup alertAnim = unitMicro.alertAnim;
				alertAnim.alpha = 1f;
				unitMicro.alertAnimPos.SetActive(true);
				DOVirtual.Float(1f, 0f, 2f, delegate(float a)
				{
					alertAnim.alpha = a;
				}).SetAutoKill(true).SetEase(Ease.InCirc).OnComplete(delegate
				{
					unitMicro.alertAnimPos.SetActive(false);
				});
			}
		}

		// Token: 0x0600A2E7 RID: 41703 RVA: 0x004C2CB4 File Offset: 0x004C0EB4
		private void TaiwuMoveSmoke()
		{
			AdventureBlockIndex renderIndex = this.DataIndexToRenderIndex(this.AdventureTaiwu.Index);
			AdventureUnitMicro unitMicro = this.GetUnitMicro(renderIndex);
			AdventureRemakeUniqueParticleHolderInfo uphInfo = this.uniqueParticleHolder.GetComponent<AdventureRemakeUniqueParticleHolderInfo>();
			UIParticle taiwuMoveSmoke = uphInfo.taiwuMoveSmoke;
			taiwuMoveSmoke.GetComponent<RectTransform>().position = unitMicro.elementsHolder.transform.position;
			Vector3 localPos = taiwuMoveSmoke.GetComponent<RectTransform>().localPosition;
			taiwuMoveSmoke.GetComponent<RectTransform>().localPosition = localPos.SetY(localPos.y + unitMicro.volumeController.GetVolumeHeightOffsetY());
			taiwuMoveSmoke.RefreshParticles();
			taiwuMoveSmoke.Play();
		}

		// Token: 0x0600A2E8 RID: 41704 RVA: 0x004C2D4C File Offset: 0x004C0F4C
		private void AdventureBlockChangeIcon(ArgumentBox argBox)
		{
			List<AdventureBlockIndexForSerialize> blockIndexList;
			argBox.Get<List<AdventureBlockIndexForSerialize>>("BlockIndexList", out blockIndexList);
			IReadOnlyList<AdventureBlock> allBlocks = this.AdventureRuntime.GetAllBlocks();
			List<AdventureBlock> showEffectList = new List<AdventureBlock>();
			foreach (AdventureBlock block in allBlocks)
			{
				AdventureBlockIndex renderIndex = this.DataIndexToRenderIndex(block.Index);
				AdventureUnitMicro unitMicro = this.GetUnitMicro(renderIndex);
				bool flag = unitMicro == null;
				if (!flag)
				{
					bool flag2 = string.IsNullOrEmpty(block.SpecialIcon);
					if (flag2)
					{
						AdventureBlockData blockData = this.AdventureRuntime.GetBlockCore(block.Index);
						unitMicro.groundSurface.SetSprite(blockData.Icon, false, null);
						unitMicro.RecoverDecorates();
					}
					else
					{
						bool flag3 = blockIndexList.Contains(block.Index);
						if (flag3)
						{
							showEffectList.Add(block);
						}
						unitMicro.groundSurface.SetSprite(block.SpecialIcon, false, null);
						unitMicro.HideDecorates();
					}
				}
			}
			this.AdventureBlockAddEffect(showEffectList, "eff_adventure_new_dipi", ViewAdventureRemake.BlockEffectType.Single);
		}

		// Token: 0x0600A2E9 RID: 41705 RVA: 0x004C2E7C File Offset: 0x004C107C
		private void AdventureElementShowHideEffect(ArgumentBox argBox)
		{
			int elementId;
			argBox.Get("ElementId", out elementId);
			bool isBlockElement;
			argBox.Get("IsBlockElement", out isBlockElement);
			AdventureElement element = this.AdventureRuntime.GetElement(elementId);
			bool flag = element == null;
			if (!flag)
			{
				AdventureBlockIndex elementIndex = this.AdventureRuntime.GetElement(elementId).Index;
				List<AdventureBlock> showEffectList = new List<AdventureBlock>
				{
					this.AdventureRuntime.GetBlock(elementIndex)
				};
				this.AdventureBlockAddEffect(showEffectList, isBlockElement ? "eff_adventure_new_dibiao" : "eff_adventure_new_juese", ViewAdventureRemake.BlockEffectType.Single);
			}
		}

		// Token: 0x0600A2EA RID: 41706 RVA: 0x004C2F04 File Offset: 0x004C1104
		private void AdventureRefreshBlockEffect(ArgumentBox argBox)
		{
			IReadOnlyList<AdventureBlock> allBlocks = this.AdventureRuntime.GetAllBlocks();
			Dictionary<string, List<AdventureBlock>> addEffect = new Dictionary<string, List<AdventureBlock>>();
			foreach (AdventureBlock block in allBlocks)
			{
				bool flag = string.IsNullOrEmpty(block.SpecialParticle);
				if (flag)
				{
					this.AdventureBlockRemoveEffect(block);
				}
				else
				{
					bool flag2 = !addEffect.ContainsKey(block.SpecialParticle);
					if (flag2)
					{
						addEffect[block.SpecialParticle] = new List<AdventureBlock>();
					}
					addEffect[block.SpecialParticle].Add(block);
				}
			}
			foreach (KeyValuePair<string, List<AdventureBlock>> keyValuePair in addEffect)
			{
				string text;
				List<AdventureBlock> list;
				keyValuePair.Deconstruct(out text, out list);
				string templateIdStr = text;
				List<AdventureBlock> blocks = list;
				AdventureRemakeBlockEffectItem config = this.GetBlockEffectConfigByName(templateIdStr);
				bool flag3 = config == null;
				if (flag3)
				{
					Debug.LogWarning("Config is null,templateIdStr is " + templateIdStr);
				}
				else
				{
					ViewAdventureRemake.BlockEffectType blockEffectType = this.LocationTypeToBlockEffectType(config.Location);
					this.AdventureBlockAddEffect(blocks, config.LoadName, blockEffectType);
				}
			}
		}

		// Token: 0x0600A2EB RID: 41707 RVA: 0x004C3050 File Offset: 0x004C1250
		private void AdventureBlockAddEffect(IEnumerable<AdventureBlock> blocks, string effectName, ViewAdventureRemake.BlockEffectType type)
		{
			ResLoader.Load<GameObject>(this.GetAdventureEffectLoadPath(effectName), delegate(GameObject go)
			{
				foreach (AdventureBlock block in blocks)
				{
					AdventureBlockIndex renderIndex = this.DataIndexToRenderIndex(block.Index);
					AdventureUnitMicro unitMicro = this.GetUnitMicro(renderIndex);
					UIParticle microBlockParticleHolder = this.GetParticleHolderByType(type, unitMicro);
					microBlockParticleHolder.enabled = true;
					RectTransform innerBlockParticleRect = microBlockParticleHolder.GetComponent<RectTransform>();
					this.DestroyChildEffect(innerBlockParticleRect, effectName);
					Transform childEffect = this.GetChildEffect(innerBlockParticleRect, effectName);
					bool flag = childEffect == null;
					if (flag)
					{
						childEffect = Object.Instantiate<GameObject>(go, innerBlockParticleRect.transform).GetComponent<RectTransform>();
					}
					childEffect.gameObject.SetActive(true);
					microBlockParticleHolder.RefreshParticles();
					RectTransform childTrans = childEffect.GetComponent<RectTransform>();
					float parentLossyScaleX = innerBlockParticleRect.transform.parent.lossyScale.x;
					childTrans.localScale = new Vector3(parentLossyScaleX, parentLossyScaleX, parentLossyScaleX);
					microBlockParticleHolder.Play();
				}
			}, null, false);
		}

		// Token: 0x0600A2EC RID: 41708 RVA: 0x004C30A0 File Offset: 0x004C12A0
		private void AdventureBlockRemoveEffect(AdventureBlock block)
		{
			AdventureBlockIndex renderIndex = this.DataIndexToRenderIndex(block.Index);
			AdventureUnitMicro unitMicro = this.GetUnitMicro(renderIndex);
			this.RemoveBlockParticle(unitMicro.microBlockParticleHolderDown);
			this.RemoveBlockParticle(unitMicro.microBlockParticleHolderTop);
		}

		// Token: 0x0600A2ED RID: 41709 RVA: 0x004C30E0 File Offset: 0x004C12E0
		private void RemoveBlockParticle(UIParticle microBlockParticleHolder)
		{
			RectTransform innerBlockParticleRect = microBlockParticleHolder.GetComponent<RectTransform>();
			for (int i = innerBlockParticleRect.childCount - 1; i >= 0; i--)
			{
				Transform child = innerBlockParticleRect.GetChild(i);
				Object.Destroy(child.gameObject);
			}
			microBlockParticleHolder.RefreshParticles();
			microBlockParticleHolder.enabled = false;
		}

		// Token: 0x0600A2EE RID: 41710 RVA: 0x004C3134 File Offset: 0x004C1334
		private Transform GetChildEffect(RectTransform parent, string effectName)
		{
			for (int i = parent.childCount - 1; i >= 0; i--)
			{
				Transform child = parent.GetChild(i);
				bool flag = child.name.Contains(effectName);
				if (flag)
				{
					return child;
				}
			}
			return null;
		}

		// Token: 0x0600A2EF RID: 41711 RVA: 0x004C3184 File Offset: 0x004C1384
		private void DestroyChildEffect(RectTransform parent, string expectEffectName)
		{
			for (int i = parent.childCount - 1; i >= 0; i--)
			{
				Transform child = parent.GetChild(i);
				ParticleSystem particleSystem;
				bool flag = !child.TryGetComponent<ParticleSystem>(out particleSystem);
				if (!flag)
				{
					bool flag2 = !child.name.Contains(expectEffectName);
					if (flag2)
					{
						Object.Destroy(child.gameObject);
					}
				}
			}
		}

		// Token: 0x0600A2F0 RID: 41712 RVA: 0x004C31EC File Offset: 0x004C13EC
		private string GetAdventureEffectLoadPath(string effectName)
		{
			return "RemakeResources/Particle/UIEffectPrefabs/AdventureRemake/" + effectName;
		}

		// Token: 0x0600A2F1 RID: 41713 RVA: 0x004C320C File Offset: 0x004C140C
		private void RemoveBlockEffect()
		{
			for (int i = 0; i < (int)this._mapSize; i++)
			{
				for (int j = 0; j < (int)this._mapSize; j++)
				{
					bool flag = !this.IsRenderIndexInner(i, j);
					if (!flag)
					{
						AdventureUnitNormal unitNormal = this._unitNormalRenderArray2D[i, j];
						for (int k = 0; k < 9; k++)
						{
							AdventureUnitMicro unitMicro = unitNormal.GetUnitMicro(k);
							bool flag2 = unitMicro == null;
							if (!flag2)
							{
								this.RemoveBlockParticle(unitMicro.microBlockParticleHolderSingle);
								this.RemoveBlockParticle(unitMicro.microBlockParticleHolderDown);
								this.RemoveBlockParticle(unitMicro.microBlockParticleHolderTop);
							}
						}
					}
				}
			}
		}

		// Token: 0x0600A2F2 RID: 41714 RVA: 0x004C32D4 File Offset: 0x004C14D4
		private UIParticle GetParticleHolderByType(ViewAdventureRemake.BlockEffectType type, AdventureUnitMicro unitMicro)
		{
			bool flag = type == ViewAdventureRemake.BlockEffectType.Single;
			UIParticle result;
			if (flag)
			{
				result = unitMicro.microBlockParticleHolderSingle;
			}
			else
			{
				bool flag2 = type == ViewAdventureRemake.BlockEffectType.LoopDown;
				if (flag2)
				{
					result = unitMicro.microBlockParticleHolderDown;
				}
				else
				{
					result = unitMicro.microBlockParticleHolderTop;
				}
			}
			return result;
		}

		// Token: 0x0600A2F3 RID: 41715 RVA: 0x004C3310 File Offset: 0x004C1510
		private ViewAdventureRemake.BlockEffectType LocationTypeToBlockEffectType(EAdventureRemakeBlockEffectLocation location)
		{
			bool flag = location == EAdventureRemakeBlockEffectLocation.Down;
			ViewAdventureRemake.BlockEffectType result;
			if (flag)
			{
				result = ViewAdventureRemake.BlockEffectType.LoopDown;
			}
			else
			{
				result = ViewAdventureRemake.BlockEffectType.LoopTop;
			}
			return result;
		}

		// Token: 0x0600A2F4 RID: 41716 RVA: 0x004C3330 File Offset: 0x004C1530
		private AdventureRemakeBlockEffectItem GetBlockEffectConfigByName(string templateIdStr)
		{
			short templateId;
			bool flag = short.TryParse(templateIdStr, out templateId);
			AdventureRemakeBlockEffectItem result;
			if (flag)
			{
				result = AdventureRemakeBlockEffect.Instance[templateId];
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x0600A2F5 RID: 41717 RVA: 0x004C3360 File Offset: 0x004C1560
		private void AdventureTaiwuShowDialog(ArgumentBox argBox)
		{
			string key;
			argBox.Get("Key", out key);
			AdventureTextData adventureTextData = this.GetAdventureTextData(key);
			bool flag = adventureTextData != null;
			if (flag)
			{
				this._taiwuDialogTextHashSet.Add(adventureTextData);
			}
		}

		// Token: 0x0600A2F6 RID: 41718 RVA: 0x004C339C File Offset: 0x004C159C
		private void AdventureElementShowDialog(ArgumentBox argBox)
		{
			string key;
			argBox.Get("Key", out key);
			int elementId;
			argBox.Get("ElementId", out elementId);
			AdventureElement element = this.AdventureRuntime.GetElement(elementId);
			bool flag = element == null;
			if (!flag)
			{
				AdventureTextData adventureTextData = this.GetAdventureTextData(key);
				bool flag2 = adventureTextData == null;
				if (!flag2)
				{
					HashSet<AdventureTextData> value;
					bool flag3 = this._elementDialogTextDict.TryGetValue(elementId, out value);
					if (flag3)
					{
						value.Add(adventureTextData);
					}
					else
					{
						this._elementDialogTextDict[elementId] = new HashSet<AdventureTextData>
						{
							adventureTextData
						};
					}
				}
			}
		}

		// Token: 0x0600A2F7 RID: 41719 RVA: 0x004C342C File Offset: 0x004C162C
		private void UpdateElementShowDialog()
		{
			bool flag = this._elementDialogTextDict.Count <= 0;
			if (!flag)
			{
				foreach (KeyValuePair<int, HashSet<AdventureTextData>> keyValuePair in this._elementDialogTextDict)
				{
					int elementId2;
					HashSet<AdventureTextData> hashSet;
					keyValuePair.Deconstruct(out elementId2, out hashSet);
					int elementId = elementId2;
					HashSet<AdventureTextData> texts = hashSet;
					bool flag2 = texts.Count == 0;
					if (!flag2)
					{
						AdventureDomainMethod.AsyncCall.SelectAndMarkCustomTextInvoked(this, this._adventureId, (from x in texts
						select x.Key).Distinct<string>().ToList<string>(), delegate(int offset, RawDataPool pool)
						{
							this.HandlerElementShowDialog(pool, offset, elementId);
						});
					}
				}
				this._elementDialogTextDict.Clear();
			}
		}

		// Token: 0x0600A2F8 RID: 41720 RVA: 0x004C3528 File Offset: 0x004C1728
		private AdventureDialog GetDialogForMicro(AdventureUnitMicro micro)
		{
			bool flag = micro.dialogSlot.childCount > 0;
			if (flag)
			{
				AdventureDialog existing = micro.dialogSlot.GetChild(0).GetComponent<AdventureDialog>();
				bool flag2 = existing != null;
				if (flag2)
				{
					micro.dialogSlot.gameObject.SetActive(true);
					return existing;
				}
			}
			GameObject go = this._dialogPool.GetObject();
			AdventureDialog dialog = go.GetComponent<AdventureDialog>();
			Transform t = go.transform;
			t.SetParent(micro.dialogSlot, false);
			t.localPosition = Vector3.zero;
			t.localScale = Vector3.one;
			micro.dialogSlot.gameObject.SetActive(true);
			return dialog;
		}

		// Token: 0x0600A2F9 RID: 41721 RVA: 0x004C35E0 File Offset: 0x004C17E0
		private void ReturnDialogToPool(AdventureUnitMicro micro)
		{
			bool flag = micro.dialogSlot.childCount == 0;
			if (!flag)
			{
				while (micro.dialogSlot.childCount > 0)
				{
					GameObject existing = micro.dialogSlot.GetChild(0).gameObject;
					AdventureDialog dialog = existing.GetComponent<AdventureDialog>();
					bool flag2 = dialog != null;
					if (flag2)
					{
						dialog.KillTween();
					}
					existing.transform.SetParent(null, false);
					this._dialogPool.DestroyObject(existing);
				}
				micro.dialogSlot.gameObject.SetActive(false);
			}
		}

		// Token: 0x0600A2FA RID: 41722 RVA: 0x004C3674 File Offset: 0x004C1874
		private void HandlerElementShowDialog(RawDataPool pool, int offset, int elementId)
		{
			string textKey = string.Empty;
			Serializer.Deserialize(pool, offset, ref textKey);
			bool flag = string.IsNullOrEmpty(textKey);
			if (!flag)
			{
				AdventureTextData textData = this.GetAdventureTextData(textKey);
				bool flag2 = textData == null;
				if (!flag2)
				{
					AdventureElement element = this.AdventureRuntime.GetElement(elementId);
					bool flag3 = element == null;
					if (!flag3)
					{
						AdventureBlockIndex renderIndex = this.DataIndexToRenderIndex(element.Index);
						AdventureUnitMicro unitMicro = this.GetUnitMicro(renderIndex);
						this.waitUpdateShowTimeElementId[elementId] = element.Index;
						AdventureDialog dialog = this.GetDialogForMicro(unitMicro);
						AdventureElementData elementData = AdventureRemakeModel.Core.GetAdventureElementData(element.CoreId);
						unitMicro.SetDialogPosY();
						dialog.RefreshDialog(textData, elementData);
					}
				}
			}
		}

		// Token: 0x0600A2FB RID: 41723 RVA: 0x004C3728 File Offset: 0x004C1928
		private AdventureDialog TryGetElementDialog(int elementId)
		{
			AdventureElement element = this.AdventureRuntime.GetElement(elementId);
			bool flag = element == null;
			AdventureDialog result;
			if (flag)
			{
				result = null;
			}
			else
			{
				AdventureBlockIndex renderIndex = this.DataIndexToRenderIndex(element.Index);
				AdventureUnitMicro unitMicro = this.GetUnitMicro(renderIndex);
				result = this.GetDialogForMicro(unitMicro);
			}
			return result;
		}

		// Token: 0x0600A2FC RID: 41724 RVA: 0x004C3774 File Offset: 0x004C1974
		private void UpdateTaiwuShowDialog()
		{
			bool flag = this._taiwuDialogTextHashSet.Count <= 0;
			if (!flag)
			{
				AdventureDomainMethod.AsyncCall.SelectAndMarkCustomTextInvoked(this, this._adventureId, (from x in this._taiwuDialogTextHashSet
				select x.Key).Distinct<string>().ToList<string>(), new AsyncMethodCallbackDelegate(this.HandlerTaiwuShowDialog));
				this._taiwuDialogTextHashSet.Clear();
			}
		}

		// Token: 0x0600A2FD RID: 41725 RVA: 0x004C37F4 File Offset: 0x004C19F4
		private void HandlerTaiwuShowDialog(int offset, RawDataPool pool)
		{
			string textKey = string.Empty;
			Serializer.Deserialize(pool, offset, ref textKey);
			bool flag = string.IsNullOrEmpty(textKey);
			if (!flag)
			{
				AdventureTextData textData = this.GetAdventureTextData(textKey);
				bool flag2 = textData == null;
				if (!flag2)
				{
					this.taiwuIconRoot.SetDialogPosY();
					this.taiwuIconRoot.taiwuDialog.RefreshDialog(textData, null);
				}
			}
		}

		// Token: 0x0600A2FE RID: 41726 RVA: 0x004C3850 File Offset: 0x004C1A50
		private void UpdateDialogShowTime()
		{
			bool notInAdventure = this.AdventureTaiwu.NotInAdventure;
			if (!notInAdventure)
			{
				bool flag = !UIManager.Instance.IsFocusElement(UIElement.AdventureRemake);
				if (!flag)
				{
					this.taiwuIconRoot.taiwuDialog.UpdateShowTime();
					bool flag2 = this.waitUpdateShowTimeElementId.Count <= 0;
					if (!flag2)
					{
						this.needRefreshIndexDialogElementId.Clear();
						this.needRemoveDialogElementId.Clear();
						foreach (KeyValuePair<int, AdventureBlockIndex> keyValuePair in this.waitUpdateShowTimeElementId)
						{
							int num;
							AdventureBlockIndex adventureBlockIndex;
							keyValuePair.Deconstruct(out num, out adventureBlockIndex);
							int elementId = num;
							AdventureBlockIndex dataIndex = adventureBlockIndex;
							AdventureElement element = this.AdventureRuntime.GetElement(elementId);
							bool flag3 = element == null;
							if (flag3)
							{
								AdventureBlockIndex renderIndex = this.DataIndexToRenderIndex(dataIndex);
								AdventureUnitMicro unitMicro = this.GetUnitMicro(renderIndex);
								this.ReturnDialogToPool(unitMicro);
								this.needRemoveDialogElementId.Add(elementId);
							}
							else
							{
								bool flag4 = element.Index != dataIndex;
								if (flag4)
								{
									AdventureBlockIndex oldRenderIndex = this.DataIndexToRenderIndex(dataIndex);
									AdventureUnitMicro oldMicro = this.GetUnitMicro(oldRenderIndex);
									AdventureBlockIndex newRenderIndex = this.DataIndexToRenderIndex(element.Index);
									AdventureUnitMicro newMicro = this.GetUnitMicro(newRenderIndex);
									bool flag5 = oldMicro.dialogSlot.childCount > 0;
									if (flag5)
									{
										while (newMicro.dialogSlot.childCount > 0)
										{
											GameObject dup = newMicro.dialogSlot.GetChild(0).gameObject;
											dup.transform.SetParent(null, false);
											this._dialogPool.DestroyObject(dup);
										}
										GameObject dialogGo = oldMicro.dialogSlot.GetChild(0).gameObject;
										dialogGo.transform.SetParent(newMicro.dialogSlot, true);
										dialogGo.transform.localPosition = Vector3.zero;
									}
									this.needRefreshIndexDialogElementId.Add(elementId);
								}
								else
								{
									AdventureDialog dialog = this.TryGetElementDialog(elementId);
									bool flag6 = dialog == null;
									if (!flag6)
									{
										bool flag7 = dialog.UpdateShowTime();
										if (flag7)
										{
											bool flag8 = !dialog.gameObject.activeSelf;
											if (flag8)
											{
												AdventureUnitMicro micro = this.GetUnitMicro(this.DataIndexToRenderIndex(dataIndex));
												this.ReturnDialogToPool(micro);
												this.needRemoveDialogElementId.Add(element.Id);
											}
										}
									}
								}
							}
						}
						bool flag9 = this.needRefreshIndexDialogElementId.Count > 0;
						if (flag9)
						{
							foreach (int elementId2 in this.needRefreshIndexDialogElementId)
							{
								AdventureElement element2 = this.AdventureRuntime.GetElement(elementId2);
								this.waitUpdateShowTimeElementId[elementId2] = element2.Index;
							}
						}
						bool flag10 = this.needRemoveDialogElementId.Count > 0;
						if (flag10)
						{
							foreach (int elementId3 in this.needRemoveDialogElementId)
							{
								this.waitUpdateShowTimeElementId.Remove(elementId3);
							}
						}
					}
				}
			}
		}

		// Token: 0x0600A2FF RID: 41727 RVA: 0x004C3BDC File Offset: 0x004C1DDC
		private AdventureTextData GetAdventureTextData(string key)
		{
			AdventureData adventureData = AdventureRemakeModel.Core.GetAdventureData(this.AdventureRuntime.CoreId);
			foreach (AdventureTextData textData in adventureData.CustomTexts)
			{
				bool flag = textData.Key.Equals(key);
				if (flag)
				{
					return textData;
				}
			}
			return null;
		}

		// Token: 0x0600A300 RID: 41728 RVA: 0x004C3C5C File Offset: 0x004C1E5C
		private void ReturnAllDialog()
		{
			for (int i = 0; i < (int)this._mapSize; i++)
			{
				for (int j = 0; j < (int)this._mapSize; j++)
				{
					bool flag = !this.IsRenderIndexInner(i, j);
					if (!flag)
					{
						AdventureUnitNormal unitNormal = this._unitNormalRenderArray2D[i, j];
						for (int k = 0; k < 9; k++)
						{
							AdventureUnitMicro unitMicro = unitNormal.GetUnitMicro(k);
							bool flag2 = unitMicro == null;
							if (!flag2)
							{
								this.ReturnDialogToPool(unitMicro);
							}
						}
					}
				}
			}
		}

		// Token: 0x0600A301 RID: 41729 RVA: 0x004C3CFC File Offset: 0x004C1EFC
		private string GetRandomCloudIconName()
		{
			AdventureData data = AdventureRemakeModel.Core.GetAdventureData(this.AdventureRuntime.CoreId);
			int index = this._random.Next(3);
			return string.Format("adventure_block_invisible_{0}_{1}", data.Style, index);
		}

		// Token: 0x0600A302 RID: 41730 RVA: 0x004C3D4C File Offset: 0x004C1F4C
		private bool BlockInCloud(AdventureBlockIndex dataIndex)
		{
			AdventureBlock adventureBlock = this.AdventureRuntime.GetBlock(dataIndex);
			bool flag = adventureBlock == null;
			return !flag && adventureBlock.InCloud;
		}

		// Token: 0x0600A303 RID: 41731 RVA: 0x004C3D80 File Offset: 0x004C1F80
		private void RefreshCloud()
		{
			for (int i = 0; i < (int)this._mapSize; i++)
			{
				for (int j = 0; j < (int)this._mapSize; j++)
				{
					bool flag = !this.IsRenderIndexInner(i, j);
					if (!flag)
					{
						AdventureUnitNormal unitNormal = this._unitNormalRenderArray2D[i, j];
						for (int k = 0; k < 9; k++)
						{
							AdventureUnitMicro unitMicro = unitNormal.GetUnitMicro(k);
							bool flag2 = unitMicro == null;
							if (!flag2)
							{
								AdventureBlockIndex dataBlockIndex = this.RenderIndexToDataIndex(unitMicro.RenderBlockIndex);
								bool inCloud = this.BlockInCloud(dataBlockIndex);
								GameObject elementRoot = unitMicro.elementRoot;
								TemplatedContainerAssemblyNew decorateHolder = unitMicro.blockDecoratesHolder;
								bool flag3 = this._cloudDict[dataBlockIndex] && !inCloud;
								if (flag3)
								{
									this._cloudDict[dataBlockIndex] = false;
									CImage cloud = unitMicro.cloud;
									cloud.DOKill(false);
									cloud.DOFade(0f, this._cloudFadeDuration).OnComplete(delegate
									{
										cloud.gameObject.SetActive(false);
										RectTransform elementRect = elementRoot.GetComponent<RectTransform>();
										elementRect.DOKill(false);
										CanvasGroup elementCanvas2 = elementRoot.GetComponent<CanvasGroup>();
										elementCanvas2.DOKill(false);
										RectTransform decorateRect = decorateHolder.GetComponent<RectTransform>();
										decorateRect.DOKill(false);
										CanvasGroup decorateCanvas = unitMicro.blockDecoratesCanvasGroup;
										decorateCanvas.DOKill(false);
										elementRect.ChangeLocalPositionY(-10f);
										decorateRect.ChangeLocalPositionY(-10f);
										elementCanvas2.alpha = 0f;
										decorateCanvas.alpha = 0f;
										elementCanvas2.DOFade(1f, this._elementFadeDuration);
										bool flag6 = !this._simpleViewModeOn;
										if (flag6)
										{
											decorateCanvas.DOFade(1f, this._elementFadeDuration);
										}
										elementRect.DOLocalMoveY(0f, this._elementFadeDuration, false);
										decorateRect.DOLocalMoveY(0f, this._elementFadeDuration, false);
									});
								}
								bool flag4 = !this._cloudDict[dataBlockIndex] && inCloud;
								if (flag4)
								{
									this._cloudDict[dataBlockIndex] = true;
									CImage cloud2 = unitMicro.cloud;
									cloud2.DOKill(false);
									cloud2.SetSprite(this.GetRandomCloudIconName(), true, null);
									cloud2.SetAlpha(0f);
									cloud2.gameObject.SetActive(true);
									cloud2.DOFade(1f, this._cloudFadeDuration);
									CanvasGroup elementCanvas = elementRoot.GetComponent<CanvasGroup>();
									elementCanvas.DOKill(false);
									elementCanvas.alpha = 1f;
									CanvasGroup decoratesCanvas = unitMicro.blockDecoratesCanvasGroup;
									decoratesCanvas.DOKill(false);
									bool flag5 = !this._simpleViewModeOn;
									if (flag5)
									{
										decoratesCanvas.alpha = 1f;
									}
									elementCanvas.DOFade(0f, this._elementFadeDuration);
									decoratesCanvas.DOFade(0f, this._elementFadeDuration);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x0600A304 RID: 41732 RVA: 0x004C4014 File Offset: 0x004C2214
		private void AdventureUnitMicroPointerEnter(ArgumentBox argBox)
		{
			AdventureBlockIndex renderIndex;
			argBox.Get<AdventureBlockIndex>("AdventureUnitMicroIndex", out renderIndex);
			ValueTuple<int, AdventureElement, int> elementRange = this.GetElementRange(renderIndex);
			int range = elementRange.Item1;
			AdventureElement element = elementRange.Item2;
			int style = elementRange.Item3;
			AdventureElementData data = AdventureRemakeModel.Core.GetAdventureElementData(element.CoreId);
			Color influenceBlockColor = Color.white;
			Color influenceEdgeColor = Color.white;
			bool showBlock = false;
			bool showEdge = false;
			List<AdventureParameterData> influenceList = (from p in data.Parameters
			where !string.IsNullOrEmpty(p.Name) && p.Type == EAdventureParameterType.Influence && element.GetParameter(p.Key).Current >= 0
			select p).ToList<AdventureParameterData>();
			bool flag = influenceList.Count > 0;
			if (flag)
			{
				showBlock = ColorUtility.TryParseHtmlString(influenceList[0].InfluenceBlockColorHex, out influenceBlockColor);
				showEdge = ColorUtility.TryParseHtmlString(influenceList[0].InfluenceEdgeColorHex, out influenceEdgeColor);
			}
			this.ShowElementRange(renderIndex, range, influenceBlockColor, influenceEdgeColor, showBlock, showEdge, style, false);
		}

		// Token: 0x0600A305 RID: 41733 RVA: 0x004C40F8 File Offset: 0x004C22F8
		private void AdventureUnitMicroPointerExit(ArgumentBox argBox)
		{
			bool notInAdventure = this.AdventureTaiwu.NotInAdventure;
			if (!notInAdventure)
			{
				AdventureBlockIndex renderIndex;
				argBox.Get<AdventureBlockIndex>("AdventureUnitMicroIndex", out renderIndex);
				this.HideElementRange();
			}
		}

		// Token: 0x0600A306 RID: 41734 RVA: 0x004C412C File Offset: 0x004C232C
		private void ShowElementRange(AdventureBlockIndex centerRenderIndex, int range, Color influenceBlockColor, Color influenceEdgeColor, bool showBlock, bool showEdge, int style, bool selectElementMode = false)
		{
			bool flag = this.startSelectElement && !selectElementMode;
			if (!flag)
			{
				bool flag2 = range < 0;
				if (!flag2)
				{
					bool isShowingInfluenceRange = this._isShowingInfluenceRange;
					if (isShowingInfluenceRange)
					{
						this.HideElementRange();
					}
					this._isShowingInfluenceRange = true;
					this._influenceColor = influenceBlockColor;
					this._influenceRangeCenter = centerRenderIndex;
					this._influenceRange = range;
					this._currentInfluenceStyle = style;
					this._inRangeIndices.Clear();
					influenceEdgeColor.a = 0.3f;
					for (int i = 0; i < (int)this._mapSize; i++)
					{
						for (int j = 0; j < (int)this._mapSize; j++)
						{
							bool flag3 = !this.IsRenderIndexInner(i, j);
							if (!flag3)
							{
								AdventureUnitNormal unitNormal = this._unitNormalRenderArray2D[i, j];
								for (int k = 0; k < 9; k++)
								{
									AdventureUnitMicro unitMicro = unitNormal.GetUnitMicro(k);
									bool flag4 = unitMicro == null;
									if (!flag4)
									{
										AdventureBlockIndex renderBlockIndex = unitMicro.RenderBlockIndex;
										int distance = this.GetInfluenceDistance(centerRenderIndex, renderBlockIndex, style);
										bool inRange = distance <= range;
										bool flag5 = inRange;
										if (flag5)
										{
											this._inRangeIndices.Add(renderBlockIndex);
											bool flag6 = !renderBlockIndex.Equals(centerRenderIndex);
											if (flag6)
											{
												TemplatedContainerAssemblyNew parent = unitMicro.elementsHolder;
												parent.HandleChild(delegate(GameObject goElement, int _)
												{
													AdventureRemakeElementIconTempInfo iconInfo = goElement.GetComponent<AdventureRemakeElementIconTempInfo>();
													bool flag9 = ((iconInfo != null) ? iconInfo.imgIcon : null) == null;
													if (!flag9)
													{
														iconInfo.imgIcon.gameObject.GetOrAddComponent<AdventureElementVertexModifier>().SetOutline(true);
													}
												});
											}
										}
									}
								}
							}
						}
					}
					if (showEdge)
					{
						this._edgeBlockIndexDict.Clear();
						foreach (AdventureBlockIndex renderIndex in this._inRangeIndices)
						{
							int directionValue = 0;
							foreach (EAdventureDirection direction in AdventureBlockIndex.Directions)
							{
								bool flag7 = !this._inRangeIndices.Contains(renderIndex.Move(direction));
								if (flag7)
								{
									directionValue |= 1 << (int)direction;
								}
							}
							bool flag8 = directionValue != 0;
							if (flag8)
							{
								this._edgeBlockIndexDict.Add(renderIndex, directionValue);
							}
						}
						this.CalcInfluenceEdgeVertices();
						this.SetInfluenceEdgeLineColor(influenceEdgeColor);
						this.RenderInfluenceEdgeLine();
					}
					this.RefreshAllMicroBlockColor();
				}
			}
		}

		// Token: 0x0600A307 RID: 41735 RVA: 0x004C43D0 File Offset: 0x004C25D0
		private void HideElementRange()
		{
			bool flag = this.startSelectElement;
			if (!flag)
			{
				this._isShowingInfluenceRange = false;
				this._influenceRange = 0;
				for (int i = 0; i < (int)this._mapSize; i++)
				{
					for (int j = 0; j < (int)this._mapSize; j++)
					{
						bool flag2 = !this.IsRenderIndexInner(i, j);
						if (!flag2)
						{
							AdventureUnitNormal unitNormal = this._unitNormalRenderArray2D[i, j];
							for (int k = 0; k < 9; k++)
							{
								AdventureUnitMicro unitMicro = unitNormal.GetUnitMicro(k);
								bool flag3 = unitMicro == null;
								if (!flag3)
								{
									bool flag4 = this._inRangeIndices.Contains(unitMicro.RenderBlockIndex);
									if (flag4)
									{
										TemplatedContainerAssemblyNew parent = unitMicro.elementsHolder;
										parent.HandleChild(delegate(GameObject goElement, int _)
										{
											AdventureRemakeElementIconTempInfo iconInfo = goElement.GetComponent<AdventureRemakeElementIconTempInfo>();
											bool flag5 = ((iconInfo != null) ? iconInfo.imgIcon : null) == null;
											if (!flag5)
											{
												iconInfo.imgIcon.gameObject.GetOrAddComponent<AdventureElementVertexModifier>().SetOutline(false);
											}
										});
									}
									this.RefreshViewEffect(unitMicro);
								}
							}
						}
					}
				}
				this.ClearInfluenceEdgeLine();
				this._inRangeIndices.Clear();
			}
		}

		// Token: 0x0600A308 RID: 41736 RVA: 0x004C44FC File Offset: 0x004C26FC
		private void RefreshAllMicroBlockColor()
		{
			for (int i = 0; i < (int)this._mapSize; i++)
			{
				for (int j = 0; j < (int)this._mapSize; j++)
				{
					bool flag = !this.IsRenderIndexInner(i, j);
					if (!flag)
					{
						AdventureUnitNormal unitNormal = this._unitNormalRenderArray2D[i, j];
						for (int k = 0; k < 9; k++)
						{
							AdventureUnitMicro unitMicro = unitNormal.GetUnitMicro(k);
							bool flag2 = unitMicro == null;
							if (!flag2)
							{
								this.RefreshMicroBlockColor(unitMicro);
							}
						}
					}
				}
			}
		}

		// Token: 0x0600A309 RID: 41737 RVA: 0x004C459C File Offset: 0x004C279C
		private void RefreshMicroBlockColor(AdventureUnitMicro unitMicro)
		{
			ColorStyleRoot colorStyleRoot;
			bool flag = !unitMicro.TryGetComponent<ColorStyleRoot>(out colorStyleRoot);
			if (!flag)
			{
				bool flag2 = AdventureLightingManager.HasActiveLighting();
				Color color;
				if (flag2)
				{
					color = Color.white;
				}
				else
				{
					AdventureBlockIndex dataCoord = this.RenderIndexToDataIndex(unitMicro.RenderBlockIndex);
					int taiwuViewDistance = this.GetInfluenceDistance(this.AdventureTaiwu.Index, dataCoord, this._viewStyle);
					AdventureBlockEffect.ViewType viewType = this.CalcViewType(taiwuViewDistance);
					color = this.GetViewColor(viewType);
				}
				int influenceDistance = this.GetInfluenceDistance(this._influenceRangeCenter, unitMicro.RenderBlockIndex, this._currentInfluenceStyle);
				bool isOutsideElementRange = this._isShowingInfluenceRange && influenceDistance > this._influenceRange;
				bool flag3 = isOutsideElementRange;
				if (flag3)
				{
					color = new Color(color.r * 0.5f, color.g * 0.5f, color.b * 0.5f, color.a);
				}
				HashSet<Graphic> skipSet = EasyPool.Get<HashSet<Graphic>>();
				skipSet.Clear();
				bool flag4 = this._isShowingInfluenceRange && influenceDistance <= this._influenceRange;
				if (flag4)
				{
					color = this._influenceColor;
					Graphic[] elementGraphics = unitMicro.elementRoot.GetComponentsInChildren<Graphic>();
					foreach (Graphic graphic in elementGraphics)
					{
						skipSet.Add(graphic);
					}
				}
				colorStyleRoot.SetColor(color, skipSet);
				EasyPool.Free<HashSet<Graphic>>(skipSet);
			}
		}

		// Token: 0x0600A30A RID: 41738 RVA: 0x004C46F8 File Offset: 0x004C28F8
		private int GetInfluenceDistance(AdventureBlockIndex centerRenderIndex, AdventureBlockIndex renderIndex, int style)
		{
			if (!true)
			{
			}
			int result;
			if (style != 0)
			{
				if (style != 1)
				{
					result = int.MaxValue;
				}
				else
				{
					result = centerRenderIndex.GetRegionBoxDistance(renderIndex);
				}
			}
			else
			{
				result = centerRenderIndex.GetManhattanDistance(renderIndex);
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x0600A30B RID: 41739 RVA: 0x004C4740 File Offset: 0x004C2940
		[return: TupleElementNames(new string[]
		{
			"range",
			"element",
			"style"
		})]
		private ValueTuple<int, AdventureElement, int> GetElementRange(AdventureBlockIndex renderIndex)
		{
			int maxValue = -1;
			int style = 0;
			AdventureElement element = null;
			List<AdventureElement> elements;
			bool flag = this._visibleElementMap.TryGetValue(this.RenderIndexToDataIndex(renderIndex), out elements) && elements.Count > 0;
			if (flag)
			{
				List<AdventureElement> elementsNotIgnoreSorting = (from e in elements
				where !AdventureRemakeModel.Core.GetAdventureElementData(e.CoreId).VisibleIgnoreSorting
				orderby AdventureRemakeModel.Core.GetAdventureElementData(e.CoreId).VisiblePriority
				select e).ToList<AdventureElement>();
				List<AdventureElement> elementsIgnoreSorting = (from e in elements
				where AdventureRemakeModel.Core.GetAdventureElementData(e.CoreId).VisibleIgnoreSorting
				orderby AdventureRemakeModel.Core.GetAdventureElementData(e.CoreId).VisiblePriority
				select e).ToList<AdventureElement>();
				bool flag2 = elementsNotIgnoreSorting.Count > 0;
				if (flag2)
				{
					elementsIgnoreSorting.Add(elementsNotIgnoreSorting[0]);
				}
				List<AdventureElement> list = elementsIgnoreSorting;
				element = list[list.Count - 1];
				IEnumerable<AdventureParameterData> parameterDataList = element.GetParameterData(EAdventureParameterType.Influence);
				foreach (AdventureParameterData data in parameterDataList)
				{
					AdventureParameterValue value = element.GetParameter(data.Key);
					bool flag3 = value.Current > maxValue;
					if (flag3)
					{
						maxValue = value.Current;
						style = data.Style;
					}
				}
			}
			return new ValueTuple<int, AdventureElement, int>(maxValue, element, style);
		}

		// Token: 0x0600A30C RID: 41740 RVA: 0x004C48DC File Offset: 0x004C2ADC
		[return: TupleElementNames(new string[]
		{
			"range",
			"style"
		})]
		private ValueTuple<int, int> GetElementRange(AdventureElement element)
		{
			int maxValue = -1;
			int style = 0;
			IEnumerable<AdventureParameterData> parameterDataList = element.GetParameterData(EAdventureParameterType.Influence);
			bool flag = parameterDataList == null;
			ValueTuple<int, int> result;
			if (flag)
			{
				result = new ValueTuple<int, int>(maxValue, style);
			}
			else
			{
				foreach (AdventureParameterData data in parameterDataList)
				{
					AdventureParameterValue value = element.GetParameter(data.Key);
					bool flag2 = value.Current > maxValue;
					if (flag2)
					{
						maxValue = value.Current;
						style = data.Style;
					}
				}
				result = new ValueTuple<int, int>(maxValue, style);
			}
			return result;
		}

		// Token: 0x0600A30D RID: 41741 RVA: 0x004C4988 File Offset: 0x004C2B88
		private void RefreshElementInfluence()
		{
			PointerEventData pointerData = new PointerEventData(EventSystem.current)
			{
				position = Input.mousePosition
			};
			RaycastAllManager raycastManager = SingletonObject.getInstance<RaycastAllManager>();
			bool flag = raycastManager == null;
			if (!flag)
			{
				List<RaycastResult> results = raycastManager.GetCurrentFrameResults();
				foreach (RaycastResult result in results)
				{
					DelayPointerTrigger delay;
					bool flag2 = result.gameObject.TryGetComponent<DelayPointerTrigger>(out delay);
					if (flag2)
					{
						delay.OnPointerEnterNotDelay(pointerData);
					}
				}
			}
		}

		// Token: 0x0600A30E RID: 41742 RVA: 0x004C4A2C File Offset: 0x004C2C2C
		private void HideAndRefresh()
		{
			this.HideElementRange();
			this.RefreshElementInfluence();
		}

		// Token: 0x0600A30F RID: 41743 RVA: 0x004C4A3D File Offset: 0x004C2C3D
		private void RenderInfluenceEdgeLine()
		{
			this.influenceEdge.GetComponent<CImage>().SetVerticesDirty();
		}

		// Token: 0x0600A310 RID: 41744 RVA: 0x004C4A51 File Offset: 0x004C2C51
		private void ClearInfluenceEdgeLine()
		{
			this._influenceEdgeVertices.Clear();
			this.RenderInfluenceEdgeLine();
		}

		// Token: 0x0600A311 RID: 41745 RVA: 0x004C4A68 File Offset: 0x004C2C68
		private EAdventureDirection GetNextDirection(EAdventureDirection direction)
		{
			bool flag = direction == EAdventureDirection.Up;
			EAdventureDirection result;
			if (flag)
			{
				result = EAdventureDirection.Right;
			}
			else
			{
				bool flag2 = direction == EAdventureDirection.Right;
				if (flag2)
				{
					result = EAdventureDirection.Down;
				}
				else
				{
					bool flag3 = direction == EAdventureDirection.Down;
					if (flag3)
					{
						result = EAdventureDirection.Left;
					}
					else
					{
						result = EAdventureDirection.Up;
					}
				}
			}
			return result;
		}

		// Token: 0x0600A312 RID: 41746 RVA: 0x004C4AA0 File Offset: 0x004C2CA0
		private void AddInfluenceEdgeVertices(AdventureBlockIndex renderIndex, int directions)
		{
			for (int i = 0; i < this.directionList.Count; i++)
			{
				EAdventureDirection direction = this.directionList[i];
				EAdventureDirection nextDirection = this.GetNextDirection(direction);
				EAdventureDirection currentDirection = EAdventureDirection.Up;
				bool flag = (directions & 1 << (int)direction) == 0 && (directions & 1 << (int)nextDirection) != 0;
				if (flag)
				{
					currentDirection = nextDirection;
					for (int j = 0; j < 4; j++)
					{
						bool flag2 = (directions & 1 << (int)currentDirection) != 0;
						if (flag2)
						{
							List<Vector2> vertices = this.GetUnitMicroVertex(renderIndex, currentDirection);
							foreach (Vector2 vertex in vertices)
							{
								bool flag3 = !this._influenceEdgeVertices.Contains(vertex);
								if (flag3)
								{
									this._influenceEdgeVertices.Add(vertex);
								}
							}
						}
						currentDirection = this.GetNextDirection(currentDirection);
					}
				}
			}
		}

		// Token: 0x0600A313 RID: 41747 RVA: 0x004C4BB8 File Offset: 0x004C2DB8
		private void CalcInfluenceEdgeVertices()
		{
			List<AdventureBlockIndex> keys = this._edgeBlockIndexDict.Keys.ToList<AdventureBlockIndex>();
			List<AdventureBlockIndex> result = this.SortEdgeBlockIndex(keys);
			this._influenceEdgeVertices.Clear();
			foreach (AdventureBlockIndex key in result)
			{
				int value = this._edgeBlockIndexDict[key];
				this.AddInfluenceEdgeVertices(key, value);
			}
			this._influenceEdgeVertices.Add(this._influenceEdgeVertices[0]);
		}

		// Token: 0x0600A314 RID: 41748 RVA: 0x004C4C5C File Offset: 0x004C2E5C
		private void SetInfluenceEdgeLineColor(Color color)
		{
			Line2DGenerator line = this.influenceEdge;
			line.Colored.StartColor = color;
			line.Colored.EndColor = color;
		}

		// Token: 0x0600A315 RID: 41749 RVA: 0x004C4C8C File Offset: 0x004C2E8C
		private List<AdventureBlockIndex> SortEdgeBlockIndex(List<AdventureBlockIndex> vertices)
		{
			bool flag = vertices == null || vertices.Count < 3;
			List<AdventureBlockIndex> result2;
			if (flag)
			{
				result2 = null;
			}
			else
			{
				vertices.Sort((AdventureBlockIndex a, AdventureBlockIndex b) => a.InDirection(b, EAdventureDirection.Up) ? 1 : -1);
				List<AdventureBlockIndex> result = new List<AdventureBlockIndex>();
				AdventureBlockIndex currentDataIndex = vertices[0];
				result.Add(currentDataIndex);
				vertices.RemoveAt(0);
				AdventureBlockIndex centerRenderIndex = this._influenceRangeCenter;
				int count = 0;
				while (vertices.Count > 0)
				{
					count++;
					List<List<EAdventureDirection>> directionCircle = currentDataIndex.InDirection(centerRenderIndex, EAdventureDirection.Right) ? this.directionCircleLeft : this.directionCircleUp;
					for (int i = 0; i < directionCircle.Count; i++)
					{
						List<EAdventureDirection> directions = directionCircle[i];
						AdventureBlockIndex potentialNextIndex = currentDataIndex;
						for (int j = 0; j < directions.Count; j++)
						{
							potentialNextIndex = potentialNextIndex.Move(directions[j]);
						}
						bool flag2 = vertices.Contains(potentialNextIndex);
						if (flag2)
						{
							currentDataIndex = potentialNextIndex;
							vertices.Remove(potentialNextIndex);
							result.Add(potentialNextIndex);
							break;
						}
					}
					bool flag3 = count > 10000;
					if (flag3)
					{
						break;
					}
				}
				result2 = result;
			}
			return result2;
		}

		// Token: 0x0600A316 RID: 41750 RVA: 0x004C4DDC File Offset: 0x004C2FDC
		private bool PlayMovingPawnAnim()
		{
			bool flag = this._movingPawns.Count <= 0;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				foreach (KeyValuePair<int, ValueTuple<AdventureBlockIndex, AdventureBlockIndex>> keyValuePair in this._movingPawns)
				{
					int num;
					ValueTuple<AdventureBlockIndex, AdventureBlockIndex> valueTuple;
					keyValuePair.Deconstruct(out num, out valueTuple);
					int elementId = num;
					ValueTuple<AdventureBlockIndex, AdventureBlockIndex> posData = valueTuple;
					GameObject image = this._pawnTemplatePool.GetObject();
					RectTransform rectTransform = image.GetComponent<RectTransform>();
					AdventureElement element = this.AdventureRuntime.GetElement(elementId);
					bool flag2 = element == null;
					if (flag2)
					{
						Debug.LogWarning(string.Format("PlayMovingPawnAnim elementId:{0} not exist", elementId));
					}
					else
					{
						int coreId = element.CoreId;
						AdventureElementData data = AdventureRemakeModel.Core.GetAdventureElementData(coreId);
						string icon = ViewAdventureRemake.GetElementIcon(data, element);
						CImage pawnImage = image.GetComponent<CImage>();
						pawnImage.SetSprite(icon, true, null);
						AdventureBlockIndex toRenderIndex = this.DataIndexToRenderIndex(posData.Item2);
						AdventureVertexModifier pawnModifier = pawnImage.gameObject.GetOrAddComponent<AdventureVertexModifier>();
						pawnModifier.GridIndex = toRenderIndex;
						AdventureBlockIndex preRenderIndex = this.DataIndexToRenderIndex(posData.Item1);
						AdventureUnitMicro preMicro = this.GetUnitMicro(preRenderIndex);
						AdventureUnitMicro toMicro = this.GetUnitMicro(toRenderIndex);
						rectTransform.position = preMicro.jumpTarget.position;
						rectTransform.SetParent(toMicro.jumpTarget, true);
						rectTransform.localScale = Vector3.one;
						rectTransform.DOLocalJump(Vector3.zero, this.jumpPower, 1, ViewWorldMap.MoveStepTime, false).SetEase(Ease.OutQuad);
						this._movingPawnImages.Add(image);
					}
				}
				base.DelayCall(new Action(this.ClearMovingPawn), ViewWorldMap.MoveStepTime);
				result = true;
			}
			return result;
		}

		// Token: 0x0600A317 RID: 41751 RVA: 0x004C4FB8 File Offset: 0x004C31B8
		private void ClearMovingPawn()
		{
			foreach (GameObject image in this._movingPawnImages)
			{
				this._pawnTemplatePool.DestroyObject(image);
			}
			foreach (GameObject image2 in this._movingPawnActualImages.Values)
			{
				image2.SetActive(true);
			}
			this._prevPawnCoordinates.Clear();
			this._movingPawnActualImages.Clear();
			this._movingPawnImages.Clear();
			this._movingPawns.Clear();
		}

		// Token: 0x0600A318 RID: 41752 RVA: 0x004C5090 File Offset: 0x004C3290
		public static string GetElementParamStateIconName(string icon, bool isBackground)
		{
			return "ui9_icon_adventure_state_" + icon + "_" + (isBackground ? 1 : 0).ToString();
		}

		// Token: 0x0600A319 RID: 41753 RVA: 0x004C50C4 File Offset: 0x004C32C4
		private void AdventureElementDeleteAnim(ArgumentBox argBox)
		{
			int elementCoreId;
			argBox.Get("ElementCoreId", out elementCoreId);
			int visibleIndex;
			argBox.Get("VisibleIndex", out visibleIndex);
			AdventureBlockIndex dataIndex;
			argBox.Get<AdventureBlockIndex>("BlockIndex", out dataIndex);
			List<ValueTuple<int, int>> list;
			bool flag = !this._elementDeleteAnimDict.TryGetValue(dataIndex, out list);
			if (flag)
			{
				list = new List<ValueTuple<int, int>>();
				this._elementDeleteAnimDict[dataIndex] = list;
			}
			list.Add(new ValueTuple<int, int>(elementCoreId, visibleIndex));
		}

		// Token: 0x0600A31A RID: 41754 RVA: 0x004C5138 File Offset: 0x004C3338
		private void UpdateShowElementDeleteAnim()
		{
			bool flag = !UIManager.Instance.IsFocusElement(UIElement.AdventureRemake);
			if (!flag)
			{
				foreach (KeyValuePair<AdventureBlockIndex, List<ValueTuple<int, int>>> pair in this._elementDeleteAnimDict)
				{
					ValueTuple<int, int> valueTuple = (from value in pair.Value
					orderby AdventureRemakeModel.Core.GetAdventureElementData(value.Item1).VisiblePriority
					select value).First<ValueTuple<int, int>>();
					int coreId = valueTuple.Item1;
					int visibleIndex = valueTuple.Item2;
					AdventureElementData elementData = AdventureRemakeModel.Core.GetAdventureElementData(coreId);
					base.StartCoroutine(this.PlayElementDeleteAnim(pair.Key, elementData, visibleIndex));
				}
				this._elementDeleteAnimDict.Clear();
				this.UpdateTaiwuIconScale();
			}
		}

		// Token: 0x0600A31B RID: 41755 RVA: 0x004C521C File Offset: 0x004C341C
		private IEnumerator PlayElementDeleteAnim(AdventureBlockIndex dataIndex, AdventureElementData elementData, int visibleIndex)
		{
			AdventureUnitMicro unitMicro = this.GetUnitMicro(this.DataIndexToRenderIndex(dataIndex));
			CImage deleteAnim = unitMicro.deleteAnim;
			string icon = this.GetElementIcon(elementData, visibleIndex);
			deleteAnim.SetSprite(icon, false, null);
			deleteAnim.gameObject.SetActive(true);
			yield return new WaitForSeconds(0.5f);
			RectTransform animRect = deleteAnim.GetComponent<RectTransform>();
			TweenCallback <>9__1;
			animRect.DORotate(new Vector3(0f, 0f, 90f), 0.5f, RotateMode.Fast).SetEase(Ease.OutBounce).OnComplete(delegate
			{
				TweenerCore<Color, Color, ColorOptions> t = deleteAnim.DOFade(0f, 0.2f);
				TweenCallback action;
				if ((action = <>9__1) == null)
				{
					action = (<>9__1 = delegate()
					{
						deleteAnim.gameObject.SetActive(false);
						deleteAnim.SetAlpha(1f);
						animRect.localEulerAngles = Vector3.zero;
					});
				}
				t.OnComplete(action);
			}).SetAutoKill<TweenerCore<Quaternion, Vector3, QuaternionOptions>>();
			yield break;
		}

		// Token: 0x0600A31C RID: 41756 RVA: 0x004C5240 File Offset: 0x004C3440
		private void PlayElementsFadeAnim()
		{
			HashSet<int> visibleIds = new HashSet<int>(from e in this._visibleElementMap.Values.SelectMany((List<AdventureElement> list) => list)
			select e.Id);
			HashSet<int> allIds = new HashSet<int>(from e in this._allElementMap.Values.SelectMany((List<AdventureElement> list) => list)
			select e.Id);
			HashSet<int> preVisibleAllIds = (from e in this._preVisibleElementMap.Values.SelectMany((List<AdventureElement> list) => list)
			select e.Id).ToHashSet<int>();
			List<int> removedFromRuntime = (from id in preVisibleAllIds
			where !allIds.Contains(id)
			select id).ToList<int>();
			foreach (int elementId in removedFromRuntime)
			{
				AdventureBlockIndex trackedIndex;
				bool flag = this.waitUpdateShowTimeElementId.TryGetValue(elementId, out trackedIndex);
				if (flag)
				{
					AdventureUnitMicro trackedMicro = this.GetUnitMicro(this.DataIndexToRenderIndex(trackedIndex));
					this.ReturnDialogToPool(trackedMicro);
					this.waitUpdateShowTimeElementId.Remove(elementId);
				}
			}
			Func<AdventureElement, bool> <>9__11;
			Dictionary<AdventureBlockIndex, List<AdventureElement>> changeVisibleElementMap = (from x in this._preVisibleElementMap.Select(delegate(KeyValuePair<AdventureBlockIndex, List<AdventureElement>> kvp)
			{
				AdventureBlockIndex key = kvp.Key;
				IEnumerable<AdventureElement> value = kvp.Value;
				Func<AdventureElement, bool> predicate;
				if ((predicate = <>9__11) == null)
				{
					predicate = (<>9__11 = ((AdventureElement elem) => !visibleIds.Contains(elem.Id) && allIds.Contains(elem.Id)));
				}
				return new
				{
					Key = key,
					FilteredElements = value.Where(predicate).ToList<AdventureElement>()
				};
			})
			where x.FilteredElements.Count > 0
			select x).ToDictionary(x => x.Key, x => x.FilteredElements);
			foreach (KeyValuePair<AdventureBlockIndex, List<AdventureElement>> kv in changeVisibleElementMap)
			{
				ValueTuple<List<AdventureElement>, List<AdventureElement>> sortingElementsData = this.GetSortingElementsData(kv.Value);
				List<AdventureElement> elementsIgnoreSorting = sortingElementsData.Item1;
				GameObject fadeObject = this._elementsFadeTemplatePool.GetObject();
				TemplatedContainerAssemblyNew fadeAssembly = fadeObject.GetComponent<TemplatedContainerAssemblyNew>();
				CanvasGroup fadeCanvas = fadeObject.GetComponent<CanvasGroup>();
				RectTransform rectTransform = fadeObject.GetComponent<RectTransform>();
				AdventureBlockIndex dataIndex = kv.Key;
				AdventureBlockIndex renderIndex = this.DataIndexToRenderIndex(dataIndex);
				fadeAssembly.gameObject.SetActive(false);
				fadeAssembly.Rebuild<RectTransform>(elementsIgnoreSorting.Count, delegate(RectTransform goFade, int index)
				{
					AdventureElement element2 = elementsIgnoreSorting[index];
					CImage image = goFade.GetComponent<CImage>();
					AdventureElementData elementData = AdventureRemakeModel.Core.GetAdventureElementData(element2.CoreId);
					string icon = ViewAdventureRemake.GetElementIcon(elementData, element2);
					image.SetSprite(icon, false, null);
					AdventureVertexModifier modifier = image.gameObject.GetOrAddComponent<AdventureVertexModifier>();
					modifier.GridIndex = renderIndex;
				});
				AdventureUnitMicro unitMicro = this.GetUnitMicro(renderIndex);
				fadeCanvas.alpha = 1f;
				rectTransform.SetParent(unitMicro.jumpTarget, false);
				rectTransform.localPosition = Vector3.zero;
				fadeCanvas.gameObject.SetActive(true);
				fadeCanvas.DOFade(0f, this.elementsFadeTime).SetEase(Ease.InCubic).OnComplete(delegate
				{
					this._elementsFadeTemplatePool.DestroyObject(fadeObject);
				});
				foreach (AdventureElement element in kv.Value)
				{
					AdventureBlockIndex trackedIndex2;
					bool flag2 = this.waitUpdateShowTimeElementId.TryGetValue(element.Id, out trackedIndex2);
					if (flag2)
					{
						AdventureUnitMicro trackedMicro2 = this.GetUnitMicro(this.DataIndexToRenderIndex(trackedIndex2));
						this.ReturnDialogToPool(trackedMicro2);
						this.waitUpdateShowTimeElementId.Remove(element.Id);
					}
				}
			}
		}

		// Token: 0x0600A31D RID: 41757 RVA: 0x004C5698 File Offset: 0x004C3898
		[return: TupleElementNames(new string[]
		{
			"ignore",
			"notIgnore"
		})]
		private ValueTuple<List<AdventureElement>, List<AdventureElement>> GetSortingElementsData(List<AdventureElement> elements)
		{
			List<AdventureElement> elementsNotIgnoreSorting = (from e in elements
			where !AdventureRemakeModel.Core.GetAdventureElementData(e.CoreId).VisibleIgnoreSorting
			orderby AdventureRemakeModel.Core.GetAdventureElementData(e.CoreId).VisiblePriority
			select e).ToList<AdventureElement>();
			List<AdventureElement> elementsIgnoreSorting = (from e in elements
			where AdventureRemakeModel.Core.GetAdventureElementData(e.CoreId).VisibleIgnoreSorting
			orderby AdventureRemakeModel.Core.GetAdventureElementData(e.CoreId).VisiblePriority descending
			select e).ToList<AdventureElement>();
			bool flag = elementsNotIgnoreSorting.Count > 0;
			if (flag)
			{
				AdventureElement elementFirst = elementsNotIgnoreSorting[0];
				elementsNotIgnoreSorting.RemoveAt(0);
				elementsIgnoreSorting.Add(elementFirst);
			}
			return new ValueTuple<List<AdventureElement>, List<AdventureElement>>(elementsIgnoreSorting, elementsNotIgnoreSorting);
		}

		// Token: 0x1700110B RID: 4363
		// (get) Token: 0x0600A31E RID: 41758 RVA: 0x004C5778 File Offset: 0x004C3978
		private static float MoveDuration
		{
			get
			{
				return ViewWorldMap.MoveStepTime + ViewWorldMap.MoveInterval;
			}
		}

		// Token: 0x1700110C RID: 4364
		// (get) Token: 0x0600A31F RID: 41759 RVA: 0x004C5785 File Offset: 0x004C3985
		private bool TaiwuInAction
		{
			get
			{
				return this.AdventureRuntime.InActionTaiwu() || this.TaiwuInActiveAction;
			}
		}

		// Token: 0x1700110D RID: 4365
		// (get) Token: 0x0600A320 RID: 41760 RVA: 0x004C579D File Offset: 0x004C399D
		private bool TaiwuInActiveAction
		{
			get
			{
				return this._taiwuAdvanceMovePointLeft > 0;
			}
		}

		// Token: 0x1700110E RID: 4366
		// (get) Token: 0x0600A321 RID: 41761 RVA: 0x004C57A8 File Offset: 0x004C39A8
		// (set) Token: 0x0600A322 RID: 41762 RVA: 0x004C57B0 File Offset: 0x004C39B0
		public int TaiwuAdvanceMovePointInitValue { get; set; }

		// Token: 0x0600A323 RID: 41763 RVA: 0x004C57BC File Offset: 0x004C39BC
		private void RefreshTaiwuProgress()
		{
			int remainTime;
			AdventureActionData taiwuActionData = this.AdventureRuntime.QueryTaiwuActionData(out remainTime);
			AdventureRemakeParamProgressTempInfo progressTempInfo = this.taiwuIconRoot.paramProgressTemplate.GetComponent<AdventureRemakeParamProgressTempInfo>();
			progressTempInfo.gameObject.SetActive(taiwuActionData != null || this.TaiwuInActiveAction);
			this.advanceDaysTips.gameObject.SetActive(this.TaiwuInActiveAction);
			bool flag = taiwuActionData != null;
			if (flag)
			{
				this.TaiwuAdvanceMovePointInitValue = 0;
				this._taiwuAdvanceMovePointLeft = 0;
				progressTempInfo.txtMeshParamName.SetText(taiwuActionData.Name, true);
				progressTempInfo.imgParamProgressIcon.SetSprite(ViewAdventureRemake.GetElementParamStateIconName(taiwuActionData.Icon, false), false, null);
				progressTempInfo.imgParamProgress.fillAmount = 1f - (float)remainTime / (float)taiwuActionData.Time;
			}
			else
			{
				bool taiwuInActiveAction = this.TaiwuInActiveAction;
				if (taiwuInActiveAction)
				{
					progressTempInfo.txtMeshParamName.SetText(LocalStringManager.Get(LanguageKey.LK_Adventure_AdvanceDays), true);
					progressTempInfo.imgParamProgressIcon.SetSprite(ViewAdventureRemake.GetElementParamStateIconName("waiting", false), false, null);
					progressTempInfo.imgParamProgress.fillAmount = (float)(this.TaiwuAdvanceMovePointInitValue - this._taiwuAdvanceMovePointLeft) / (float)this.TaiwuAdvanceMovePointInitValue;
				}
			}
		}

		// Token: 0x0600A324 RID: 41764 RVA: 0x004C58DC File Offset: 0x004C3ADC
		private void AdventureAdvanceDaysSet(ArgumentBox argBox)
		{
			int advanceDaysSet;
			argBox.Get("AdvanceDays", out advanceDaysSet);
			this._taiwuAdvanceMovePointLeft = (this.TaiwuAdvanceMovePointInitValue = advanceDaysSet * 10);
			this._progressDuration = ViewAdventureRemake.MoveDuration / 10f;
		}

		// Token: 0x0600A325 RID: 41765 RVA: 0x004C5920 File Offset: 0x004C3B20
		private void StopTaiwuAction()
		{
			this._taiwuAdvanceMovePointLeft = 0;
			bool flag = !this.AdventureRuntime.InActionTaiwu();
			if (flag)
			{
				GameObject paramHolder = this.taiwuIconRoot.paramProgressTemplate;
				paramHolder.gameObject.SetActive(false);
				this.advanceDaysTips.gameObject.SetActive(false);
			}
		}

		// Token: 0x0600A326 RID: 41766 RVA: 0x004C5974 File Offset: 0x004C3B74
		private void AdventureEventHandled(ArgumentBox argBox)
		{
			this._actionCanContinue = true;
		}

		// Token: 0x0600A327 RID: 41767 RVA: 0x004C5980 File Offset: 0x004C3B80
		private void UpdateTaiwuAction()
		{
			bool flag = CommonCommandKit.Space.Check(this.Element, false, false, false, true, false);
			if (flag)
			{
				this.StopTaiwuAction();
			}
			bool flag2 = this.TaiwuInAction && UIManager.Instance.IsFocusElement(this.Element);
			if (flag2)
			{
				bool flag3 = this._progressDuration > ViewAdventureRemake.MoveDuration / 10f && this._actionCanContinue;
				if (flag3)
				{
					this._progressDuration = 0f;
					this._actionCanContinue = false;
					bool taiwuInActiveAction = this.TaiwuInActiveAction;
					if (taiwuInActiveAction)
					{
						UIManager.Instance.SetEscHandler(this.TaiwuInActiveAction ? new Action(this.StopTaiwuAction) : null);
						AdventureDomainMethod.AsyncCall.ConsumeActionPointInAdventure(this, 1, delegate(int offset, RawDataPool pool)
						{
							Serializer.Deserialize(pool, offset, ref this._actionCanContinue);
							bool actionCanContinue = this._actionCanContinue;
							if (actionCanContinue)
							{
								this._taiwuAdvanceMovePointLeft--;
								this.RefreshTaiwuProgress();
							}
							else
							{
								Debug.LogWarning("_actionCanContinue false");
							}
						});
						this.RefreshTaiwuProgress();
					}
					else
					{
						AdventureDomainMethod.AsyncCall.AddProgress(this, delegate(int offset, RawDataPool pool)
						{
							Serializer.Deserialize(pool, offset, ref this._actionCanContinue);
							bool flag4 = !this._actionCanContinue;
							if (flag4)
							{
								Debug.LogWarning("_actionCanContinue false");
							}
						});
					}
					base.DelayCall(new Action(this.ClearMovingPawn), ViewAdventureRemake.MoveDuration);
				}
				else
				{
					this._progressDuration += Time.deltaTime;
				}
			}
		}

		// Token: 0x0600A328 RID: 41768 RVA: 0x004C5A9C File Offset: 0x004C3C9C
		private void UpdateAdventureBanTimeBall()
		{
			bool flag = this._checkDuration > ViewAdventureRemake.MoveDuration;
			if (flag)
			{
				this._checkDuration = 0f;
				bool banMask = this.TaiwuInAction || this._cameraMoving;
				bool banTimeBall = banMask || this.startSelectElement;
				GEvent.OnEvent(UiEvents.AdventureBanTimeBall, EasyPool.Get<ArgumentBox>().Set("AdventureBan", banTimeBall));
				this.actionMask.gameObject.SetActive(banMask);
			}
			else
			{
				this._checkDuration += Time.deltaTime;
			}
		}

		// Token: 0x0600A329 RID: 41769 RVA: 0x004C5B30 File Offset: 0x004C3D30
		private void OnRenderTaiwuStateLine(int index, GameObject go)
		{
			AdventureTaiwuStateLine line = go.GetComponent<AdventureTaiwuStateLine>();
			List<ValueTuple<AdventureParameterData, AdventureParameterValue>> data = this._taiwuStateLineList[index];
			line.SetValue(data);
		}

		// Token: 0x0600A32A RID: 41770 RVA: 0x004C5B5C File Offset: 0x004C3D5C
		private void RefreshTaiwuState()
		{
			AdventureData adventureData = AdventureRemakeModel.Core.GetAdventureData(this.AdventureRuntime.CoreId);
			List<ValueTuple<AdventureParameterData, AdventureParameterValue>> taiwuStateList = (from p in adventureData.Parameters
			where !string.IsNullOrEmpty(p.Name) && p.Style == 0 && p.Type == EAdventureParameterType.State
			select p into paramData
			select new ValueTuple<AdventureParameterData, AdventureParameterValue>(paramData, this.AdventureRuntime.GetParameter(paramData.Key)) into x
			where x.Item2.AsProgress > 0f || x.Item2.Max < 0
			select x).ToList<ValueTuple<AdventureParameterData, AdventureParameterValue>>();
			this._taiwuStateHolder.gameObject.SetActive(taiwuStateList.Count > 0);
			this._taiwuStateLineList = ViewAdventureRemake.Chunk<ValueTuple<AdventureParameterData, AdventureParameterValue>>(taiwuStateList, 5).ToList<List<ValueTuple<AdventureParameterData, AdventureParameterValue>>>();
			this._taiwuStateHolder.SetDataCount(this._taiwuStateLineList.Count);
		}

		// Token: 0x0600A32B RID: 41771 RVA: 0x004C5C28 File Offset: 0x004C3E28
		public static IEnumerable<List<T>> Chunk<T>(IEnumerable<T> source, int chunkSize)
		{
			bool flag = chunkSize <= 0;
			if (flag)
			{
				throw new ArgumentException("Chunk size must be positive.", "chunkSize");
			}
			List<T> buffer = new List<T>(chunkSize);
			foreach (T item in source)
			{
				buffer.Add(item);
				bool flag2 = buffer.Count == chunkSize;
				if (flag2)
				{
					yield return buffer;
					buffer = new List<T>(chunkSize);
				}
				item = default(T);
			}
			IEnumerator<T> enumerator = null;
			bool flag3 = buffer.Count > 0;
			if (flag3)
			{
				yield return buffer;
			}
			yield break;
			yield break;
		}

		// Token: 0x0600A32C RID: 41772 RVA: 0x004C5C40 File Offset: 0x004C3E40
		private void RefreshTaiwuInfluence()
		{
			AdventureData adventureData = AdventureRemakeModel.Core.GetAdventureData(this.AdventureRuntime.CoreId);
			List<AdventureParameterData> taiwuInfluenceList = (from p in adventureData.Parameters
			where !string.IsNullOrEmpty(p.Name) && !string.IsNullOrEmpty(p.Icon) && p.Type == EAdventureParameterType.Influence
			select p into paramData
			where this.AdventureRuntime.GetParameter(paramData.Key).Current >= 0
			select paramData).ToList<AdventureParameterData>();
			CImage taiwuInfluenceIcon = this.taiwuIconRoot.influenceIcon;
			taiwuInfluenceIcon.gameObject.SetActive(taiwuInfluenceList.Count > 0);
			bool flag = taiwuInfluenceList.Count > 0;
			if (flag)
			{
				AdventureParameterData taiwuInfluence = taiwuInfluenceList[0];
				taiwuInfluenceIcon.SetSprite(ViewAdventureRemake.GetElementParamStateIconName(taiwuInfluence.Icon, false), true, null);
				DelayPointerTrigger pointerTrigger = taiwuInfluenceIcon.GetComponent<DelayPointerTrigger>();
				int range = this.AdventureRuntime.GetParameter(taiwuInfluence.Key).Current;
				int style = taiwuInfluence.Style;
				Color influenceBlockColor;
				bool showBlock = ColorUtility.TryParseHtmlString(taiwuInfluence.InfluenceBlockColorHex, out influenceBlockColor);
				Color influenceEdgeColor;
				bool showEdge = ColorUtility.TryParseHtmlString(taiwuInfluence.InfluenceEdgeColorHex, out influenceEdgeColor);
				pointerTrigger.enterEvent.ResetListener(delegate()
				{
					this.ShowElementRange(this.TaiwuRenderBlockIndex, range, influenceBlockColor, influenceEdgeColor, showBlock, showEdge, style, false);
				});
				pointerTrigger.exitEvent.ResetListener(new Action(this.HideElementRange));
			}
		}

		// Token: 0x0600A32D RID: 41773 RVA: 0x004C5DA8 File Offset: 0x004C3FA8
		private Color GetViewColor(AdventureBlockEffect.ViewType viewType)
		{
			if (!true)
			{
			}
			Color result;
			if (viewType != AdventureBlockEffect.ViewType.Near)
			{
				if (viewType != AdventureBlockEffect.ViewType.Far)
				{
					result = this._colorOutView;
				}
				else
				{
					result = this._colorFar;
				}
			}
			else
			{
				result = this._colorNear;
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x0600A32E RID: 41774 RVA: 0x004C5DEC File Offset: 0x004C3FEC
		private void RefreshViewValue()
		{
			AdventureParameterData viewNearParamData = this.AdventureRuntime.GetParameterData("view_range_0");
			this._viewTypeNear = this.AdventureRuntime.GetParameter("view_range_0").Current;
			this._viewTypeFar = this.AdventureRuntime.GetParameter("view_range_1").Current;
			this._viewStyle = viewNearParamData.Style;
			AdventurePointLight light;
			if ((light = this._taiwuPointLight) == null)
			{
				light = (this._taiwuPointLight = this.taiwuIconRoot.GetComponent<AdventurePointLight>());
			}
			this.SyncTaiwuPointLightRange(light);
		}

		// Token: 0x0600A32F RID: 41775 RVA: 0x004C5E78 File Offset: 0x004C4078
		private void ResetViewValue()
		{
			this._viewTypeNear = 0;
			this._viewTypeFar = 0;
			this._viewStyle = 0;
			bool flag = this._taiwuPointLight != null;
			if (flag)
			{
				this.SyncTaiwuPointLightRange(this._taiwuPointLight);
			}
		}

		// Token: 0x0600A330 RID: 41776 RVA: 0x004C5EB8 File Offset: 0x004C40B8
		private void RefreshViewEffect(AdventureUnitMicro unitMicro)
		{
			bool notInAdventure = this.AdventureTaiwu.NotInAdventure;
			if (!notInAdventure)
			{
				this.RefreshMicroBlockColor(unitMicro);
				this.RefreshMicroBlockParticle(unitMicro);
			}
		}

		// Token: 0x0600A331 RID: 41777 RVA: 0x004C5EE8 File Offset: 0x004C40E8
		private void RefreshMicroBlockParticle(AdventureUnitMicro unitMicro)
		{
			AdventureBlockIndex dataCoord = this.RenderIndexToDataIndex(unitMicro.RenderBlockIndex);
			int distance = this.GetInfluenceDistance(this.AdventureTaiwu.Index, dataCoord, this._viewStyle);
			AdventureBlockEffect.ViewType viewType = this.CalcViewType(distance);
			UIParticle uiParticle = unitMicro.microBlockParticleHolderDown;
			RectTransform uiParticleRectTransform = uiParticle.GetComponent<RectTransform>();
			for (int i = 0; i < uiParticleRectTransform.childCount; i++)
			{
				Transform child = uiParticleRectTransform.GetChild(i);
				AdventureBlockEffect adventureBlockEffect = child.GetComponent<AdventureBlockEffect>();
				bool flag = adventureBlockEffect == null;
				if (!flag)
				{
					adventureBlockEffect.CurrentViewType = viewType;
				}
			}
		}

		// Token: 0x0600A332 RID: 41778 RVA: 0x004C5F80 File Offset: 0x004C4180
		private AdventureBlockEffect.ViewType CalcViewType(int distance)
		{
			AdventureBlockEffect.ViewType viewType = AdventureBlockEffect.ViewType.Near;
			bool flag = distance > this._viewTypeFar;
			if (flag)
			{
				viewType = AdventureBlockEffect.ViewType.OutView;
			}
			else
			{
				bool flag2 = distance > this._viewTypeNear && distance <= this._viewTypeFar;
				if (flag2)
				{
					viewType = AdventureBlockEffect.ViewType.Far;
				}
			}
			return viewType;
		}

		// Token: 0x0600A333 RID: 41779 RVA: 0x004C5FC4 File Offset: 0x004C41C4
		private void SimpleViewModeSwitch()
		{
			for (int i = 0; i < (int)this._mapSize; i++)
			{
				for (int j = 0; j < (int)this._mapSize; j++)
				{
					bool flag = !this.IsRenderIndexInner(i, j);
					if (!flag)
					{
						AdventureUnitNormal unitNormal = this._unitNormalRenderArray2D[i, j];
						for (int k = 0; k < 9; k++)
						{
							AdventureUnitMicro unitMicro = unitNormal.GetUnitMicro(k);
							bool flag2 = unitMicro == null;
							if (!flag2)
							{
								CanvasGroup decorateCanvas = unitMicro.blockDecoratesCanvasGroup;
								decorateCanvas.DOFade((float)(this._simpleViewModeOn ? 0 : 1), 0.3f);
							}
						}
					}
				}
			}
		}

		// Token: 0x0600A334 RID: 41780 RVA: 0x004C6084 File Offset: 0x004C4284
		private void AdventureStartSelectElement(ArgumentBox argBox)
		{
			this.startSelectElement = true;
			argBox.Get<AdventureBlockIndex>("CenterIndex", out this.selectElementCenterDataIndex);
			argBox.Get("Range", out this.selectElementRange);
			argBox.Get("SaveKey", out this.selectElementSaveKey);
			this.ClearPathsAndCost();
			AdventureBlockIndex renderIndex = this.DataIndexToRenderIndex(this.selectElementCenterDataIndex);
			this.ShowElementRange(renderIndex, this.selectElementRange, this.selectElementRangeColor, this.selectElementEdgeColor, false, true, 0, true);
		}

		// Token: 0x0600A335 RID: 41781 RVA: 0x004C6104 File Offset: 0x004C4304
		private void SelectElement(int elementId)
		{
			ViewAdventureRemake.<>c__DisplayClass416_0 CS$<>8__locals1 = new ViewAdventureRemake.<>c__DisplayClass416_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.elementId = elementId;
			string title = LocalStringManager.Get(LanguageKey.Lk_Adventure_Select_Element);
			string content = LocalStringManager.Get(LanguageKey.Lk_Adventure_Select_Element_Content);
			CommonUtils.ShowConfirmDialog(title, content, new Action(CS$<>8__locals1.<SelectElement>g__ConfirmAction|0), null, EDialogType.None);
		}

		// Token: 0x0600A336 RID: 41782 RVA: 0x004C6153 File Offset: 0x004C4353
		private void CancelSelectElement()
		{
			TaiwuEventDomainMethod.Call.TriggerListener("AdventureSelectElementFinish", true);
			this.startSelectElement = false;
			this.HideElementRange();
		}

		// Token: 0x0600A337 RID: 41783 RVA: 0x004C6170 File Offset: 0x004C4370
		private void AdvanceDaysBtnClick()
		{
			bool activeInHierarchy = this.adventureAdvanceDays.gameObject.activeInHierarchy;
			if (activeInHierarchy)
			{
				this.adventureAdvanceDays.gameObject.SetActive(false);
			}
			else
			{
				this.adventureAdvanceDays.gameObject.SetActive(true);
				this.adventureAdvanceDays.Show();
			}
		}

		// Token: 0x0600A338 RID: 41784 RVA: 0x004C61C8 File Offset: 0x004C43C8
		private void PrepareOuterBlockPiece()
		{
			List<MapBlockData> mapBlockDatas = EasyPool.Get<List<MapBlockData>>();
			this.WorldMapModel.GetNeighborList(this.WorldMapModel.CurrentAreaId, this.WorldMapModel.CurrentBlockId, mapBlockDatas, 3, true);
			Dictionary<short, MapBlockData> pool = new Dictionary<short, MapBlockData>();
			foreach (MapBlockData blockData in mapBlockDatas)
			{
				MapBlockItem config = blockData.GetConfig();
				bool settlement = WorldMapModel.IsSettlementBlock(config);
				bool flag = settlement;
				if (!flag)
				{
					pool.TryAdd(config.TemplateId, blockData);
				}
			}
			EasyPool.Free<List<MapBlockData>>(mapBlockDatas);
			this._outerBlockPool = pool.Values.ToList<MapBlockData>();
			bool flag2 = this._outerBlockPool.Count == 0;
			if (flag2)
			{
				this._outerBlockPool.Add(this.WorldMapModel.CurrentBlockData);
			}
		}

		// Token: 0x0600A339 RID: 41785 RVA: 0x004C62B4 File Offset: 0x004C44B4
		public static string GetElementUIIconName(string name)
		{
			return name + "_small";
		}

		// Token: 0x0600A33A RID: 41786 RVA: 0x004C62D4 File Offset: 0x004C44D4
		public static string GetElementAvatarIconName(string name)
		{
			return name + "_avatar";
		}

		// Token: 0x0600A33B RID: 41787 RVA: 0x004C62F1 File Offset: 0x004C44F1
		IEnumerator IPreloadElement.Preload()
		{
			TimeSpan maxExecuteDurationPerFrame = TimeSpan.FromMilliseconds(32.0);
			DateTime startTime = DateTime.UtcNow;
			int num;
			for (int i = 0; i < 100; i = num + 1)
			{
				GameObject inner = Object.Instantiate<GameObject>(this.innerBlockTemplate, this.unitRoot);
				inner.SetActive(false);
				bool flag = DateTime.UtcNow - startTime > maxExecuteDurationPerFrame;
				if (flag)
				{
					yield return null;
					startTime = DateTime.UtcNow;
				}
				inner = null;
				num = i;
			}
			for (int j = 0; j < 300; j = num + 1)
			{
				GameObject outer = Object.Instantiate<GameObject>(this.outerBlockTemplate, this.unitRoot);
				outer.SetActive(false);
				bool flag2 = DateTime.UtcNow - startTime > maxExecuteDurationPerFrame;
				if (flag2)
				{
					yield return null;
					startTime = DateTime.UtcNow;
				}
				outer = null;
				num = j;
			}
			yield break;
		}

		// Token: 0x0600A342 RID: 41794 RVA: 0x004C67A8 File Offset: 0x004C49A8
		[CompilerGenerated]
		private float <Routine>g__CalcActionInTime|82_1(int index)
		{
			return Mathf.Min((float)index * 0.2f, this._unitRiseMaxTime);
		}

		// Token: 0x0600A345 RID: 41797 RVA: 0x004C683C File Offset: 0x004C4A3C
		[CompilerGenerated]
		internal static void <AdvanceMonth>g__Action|142_1()
		{
			int leftDays = SingletonObject.getInstance<BasicGameData>().ActionPointCurrMonth / 10;
			bool flag = leftDays == 0;
			if (flag)
			{
				WorldDomainMethod.Call.AdvanceMonth();
				GameApp.AdvancingMonth = true;
			}
			else
			{
				WorldDomainMethod.Call.AdvanceDaysInMonth(leftDays);
			}
		}

		// Token: 0x04007E47 RID: 32327
		[SerializeField]
		private RectTransform unitRoot;

		// Token: 0x04007E48 RID: 32328
		[SerializeField]
		private GameObject innerBlockTemplate;

		// Token: 0x04007E49 RID: 32329
		[SerializeField]
		private GameObject outerBlockTemplate;

		// Token: 0x04007E4A RID: 32330
		[SerializeField]
		private float focusToTaiwuTime = 1.3f;

		// Token: 0x04007E4B RID: 32331
		[SerializeField]
		private AnimationCurve curve;

		// Token: 0x04007E4C RID: 32332
		[SerializeField]
		private int grayMaskSizeX = 5;

		// Token: 0x04007E4D RID: 32333
		[SerializeField]
		private int grayMaskSizeY = 7;

		// Token: 0x04007E4E RID: 32334
		public TMP_InputField searchByName;

		// Token: 0x04007E4F RID: 32335
		[SerializeField]
		private CButton filterClearButton;

		// Token: 0x04007E50 RID: 32336
		[SerializeField]
		private CToggle simpleViewModeToggle;

		// Token: 0x04007E51 RID: 32337
		[SerializeField]
		private TaiwuIconRoot taiwuIconRoot;

		// Token: 0x04007E52 RID: 32338
		[SerializeField]
		private AdventureEntranceLightingEffect lightingEffect;

		// Token: 0x04007E53 RID: 32339
		[SerializeField]
		private AdventureRemakeFinish adventureRemakeFinish;

		// Token: 0x04007E54 RID: 32340
		[SerializeField]
		private AdventureAdvanceDays adventureAdvanceDays;

		// Token: 0x04007E55 RID: 32341
		[SerializeField]
		private CButton temporaryItemBtn;

		// Token: 0x04007E56 RID: 32342
		[SerializeField]
		private CButton advanceDaysBtn;

		// Token: 0x04007E57 RID: 32343
		[SerializeField]
		private GameObject advanceDaysTips;

		// Token: 0x04007E58 RID: 32344
		[SerializeField]
		private GameObject remainTimeTips;

		// Token: 0x04007E59 RID: 32345
		[SerializeField]
		private CImage adventureBlend;

		// Token: 0x04007E5A RID: 32346
		[SerializeField]
		private CImage adventureMask;

		// Token: 0x04007E5B RID: 32347
		[SerializeField]
		private GameObject actionMask;

		// Token: 0x04007E5C RID: 32348
		[SerializeField]
		private GameObject pawnAnimTemplate;

		// Token: 0x04007E5D RID: 32349
		[SerializeField]
		private InfinityScroll parameterScrollView;

		// Token: 0x04007E5E RID: 32350
		[SerializeField]
		private InfinityScroll taiwuStateHolder;

		// Token: 0x04007E5F RID: 32351
		[SerializeField]
		private InfinityScroll temporaryItemVerticalScrollView;

		// Token: 0x04007E60 RID: 32352
		[SerializeField]
		private Line2DGenerator invalidPaths;

		// Token: 0x04007E61 RID: 32353
		[SerializeField]
		private Line2DGenerator paths;

		// Token: 0x04007E62 RID: 32354
		[SerializeField]
		private LoopVerticalScrollRect elementScrollView;

		// Token: 0x04007E63 RID: 32355
		[SerializeField]
		private UnityEngine.Material blockVolume;

		// Token: 0x04007E64 RID: 32356
		[SerializeField]
		private MouseWheelScale moveAndScaleRoot;

		// Token: 0x04007E65 RID: 32357
		[SerializeField]
		private RectTransform backParticleHolder;

		// Token: 0x04007E66 RID: 32358
		[SerializeField]
		private RectTransform frontParticleHolder;

		// Token: 0x04007E67 RID: 32359
		[SerializeField]
		private RectTransform fullScreenParticleHolder;

		// Token: 0x04007E68 RID: 32360
		[SerializeField]
		private RectTransform selected;

		// Token: 0x04007E69 RID: 32361
		[SerializeField]
		private RectTransform temporaryItemExpandBtn;

		// Token: 0x04007E6A RID: 32362
		[SerializeField]
		private RectTransform uniqueParticleHolder;

		// Token: 0x04007E6B RID: 32363
		[SerializeField]
		private TemplatedContainerAssemblyNew costHolder;

		// Token: 0x04007E6C RID: 32364
		[SerializeField]
		private TextMeshProUGUI countLabel;

		// Token: 0x04007E6D RID: 32365
		[SerializeField]
		private GameObject temporaryItemEmpty;

		// Token: 0x04007E6E RID: 32366
		[SerializeField]
		private GameObject elementsFadeTemplate;

		// Token: 0x04007E6F RID: 32367
		[SerializeField]
		private GameObject adventureDialogPrefab;

		// Token: 0x04007E70 RID: 32368
		[SerializeField]
		private GameObject overlapHolderPrefab;

		// Token: 0x04007E71 RID: 32369
		[SerializeField]
		private Line2DGenerator influenceEdge;

		// Token: 0x04007E72 RID: 32370
		[SerializeField]
		private Vector2 _normalBlockSize = new Vector2(640f, 332f);

		// Token: 0x04007E73 RID: 32371
		private readonly Vector2 _microBlockSize = new Vector2(212f, 110f);

		// Token: 0x04007E74 RID: 32372
		private IEnumerator _routine;

		// Token: 0x04007E75 RID: 32373
		private bool _isRefreshing;

		// Token: 0x04007E76 RID: 32374
		private Random _random;

		// Token: 0x04007E77 RID: 32375
		private AdventureLightingManager _lightingManager;

		// Token: 0x04007E78 RID: 32376
		private AdventurePointLight _taiwuPointLight;

		// Token: 0x04007E79 RID: 32377
		private bool _lightingDefaultsInitialized;

		// Token: 0x04007E7A RID: 32378
		private Color _defaultGlobalColor;

		// Token: 0x04007E7B RID: 32379
		private float _defaultGlobalIntensity;

		// Token: 0x04007E7C RID: 32380
		private float _defaultGlobalIncidenceAngle;

		// Token: 0x04007E7D RID: 32381
		private float _defaultRotationAnglePerStep;

		// Token: 0x04007E7E RID: 32382
		private Color _defaultPointLightColor;

		// Token: 0x04007E7F RID: 32383
		private float _defaultPointLightIntensity;

		// Token: 0x04007E80 RID: 32384
		private float _defaultPointLightVirtualZ;

		// Token: 0x04007E81 RID: 32385
		private bool _transitionFinish;

		// Token: 0x04007E82 RID: 32386
		private byte _mapSize;

		// Token: 0x04007E83 RID: 32387
		private bool[,] _mapIsInnerArray;

		// Token: 0x04007E84 RID: 32388
		private bool _showTransitionAnim;

		// Token: 0x04007E85 RID: 32389
		private List<ByteCoordinate> _renderCoordList = new List<ByteCoordinate>();

		// Token: 0x04007E86 RID: 32390
		private int _adventureSize;

		// Token: 0x04007E87 RID: 32391
		private readonly float _unitRiseMaxTime = 1f;

		// Token: 0x04007E88 RID: 32392
		private Dictionary<ByteCoordinate, float> _flatDict;

		// Token: 0x04007E89 RID: 32393
		private List<ByteCoordinate> _allFlatStartPoint;

		// Token: 0x04007E8A RID: 32394
		private List<ByteCoordinate> _selectedFlatStartPoint;

		// Token: 0x04007E8B RID: 32395
		private List<float> _flatHeights;

		// Token: 0x04007E8C RID: 32396
		private int _outerUnitCount;

		// Token: 0x04007E8D RID: 32397
		private int _flatUnitCount;

		// Token: 0x04007E8E RID: 32398
		private readonly List<Vector2> _indicateLineValidVertices = new List<Vector2>();

		// Token: 0x04007E8F RID: 32399
		private readonly List<Vector2> _indicateLineInvalidVertices = new List<Vector2>();

		// Token: 0x04007E90 RID: 32400
		private List<AdventureBlockIndex> _paths;

		// Token: 0x04007E91 RID: 32401
		private EAdventureDirection _hotkeyMoveDirection;

		// Token: 0x04007E92 RID: 32402
		private bool _hotkeyMoveDirectionIsDown;

		// Token: 0x04007E93 RID: 32403
		private bool _doingMove;

		// Token: 0x04007E94 RID: 32404
		private bool _continuousMoving;

		// Token: 0x04007E95 RID: 32405
		private int _moveCount = 0;

		// Token: 0x04007E96 RID: 32406
		private Action _continuousMovingCallback;

		// Token: 0x04007E97 RID: 32407
		private int _adventureId;

		// Token: 0x04007E98 RID: 32408
		private short _adventureEnvironmentTemplateId;

		// Token: 0x04007E99 RID: 32409
		private AdventureUnitNormal[,] _unitNormalRenderArray2D;

		// Token: 0x04007E9A RID: 32410
		private AdventureUnitPeripheral[,] _unitPeripheralArray2D;

		// Token: 0x04007E9D RID: 32413
		private readonly Dictionary<int, int> _unitMicroIndexMappings = new Dictionary<int, int>
		{
			{
				0,
				6
			},
			{
				1,
				3
			},
			{
				2,
				7
			},
			{
				3,
				0
			},
			{
				4,
				4
			},
			{
				5,
				8
			},
			{
				6,
				1
			},
			{
				7,
				5
			},
			{
				8,
				2
			}
		};

		// Token: 0x04007E9E RID: 32414
		private Sequence _blendMaskSequence;

		// Token: 0x04007E9F RID: 32415
		[SerializeField]
		private InfinityScroll globalParameterPanel;

		// Token: 0x04007EA0 RID: 32416
		[TupleElementNames(new string[]
		{
			"parameterData",
			"parameterValue"
		})]
		private List<ValueTuple<AdventureParameterData, AdventureParameterValue>> _globalParameterList = new List<ValueTuple<AdventureParameterData, AdventureParameterValue>>();

		// Token: 0x04007EA1 RID: 32417
		private LoopVerticalScrollRect _elementPanel;

		// Token: 0x04007EA2 RID: 32418
		private Dictionary<AdventureBlockIndex, List<AdventureElement>> _allElementMap = new Dictionary<AdventureBlockIndex, List<AdventureElement>>();

		// Token: 0x04007EA3 RID: 32419
		private Dictionary<AdventureBlockIndex, List<AdventureElement>> _visibleElementMap = new Dictionary<AdventureBlockIndex, List<AdventureElement>>();

		// Token: 0x04007EA4 RID: 32420
		private List<ViewAdventureRemake.ElementDisplayItem> _displayItems = new List<ViewAdventureRemake.ElementDisplayItem>();

		// Token: 0x04007EA5 RID: 32421
		private AdventureBlockIndex _needRenderDataIndex;

		// Token: 0x04007EA6 RID: 32422
		private List<ItemKey> _temporaryItemKeyList;

		// Token: 0x04007EA7 RID: 32423
		private List<GameData.Domains.Adventure.AdventureItem> _temporaryItemList;

		// Token: 0x04007EA8 RID: 32424
		private List<ItemDisplayData> _temporaryItemDisplayDataList;

		// Token: 0x04007EA9 RID: 32425
		private List<List<ItemDisplayData>> _chunkTemporaryItemDisplayDataList;

		// Token: 0x04007EAA RID: 32426
		private InfinityScroll _temporaryItemVerticalScrollView;

		// Token: 0x04007EAB RID: 32427
		private const int MapShowBatchSize = 30;

		// Token: 0x04007EAC RID: 32428
		public float OuterBlockBrightness = 0.15f;

		// Token: 0x04007EAD RID: 32429
		public float OuterBlockSaturation = 0f;

		// Token: 0x04007EAE RID: 32430
		private const float DarkDuration = 1.5f;

		// Token: 0x04007EAF RID: 32431
		private const float DarkByCircleDelayTime = 0.3f;

		// Token: 0x04007EB0 RID: 32432
		private const float DarkByCircleDelayTimeMax = 1f;

		// Token: 0x04007EB1 RID: 32433
		private readonly List<GameObject> _waitToDestroyList = new List<GameObject>();

		// Token: 0x04007EB2 RID: 32434
		private bool _isFinish;

		// Token: 0x04007EB3 RID: 32435
		private bool _skipFinishAnim;

		// Token: 0x04007EB4 RID: 32436
		private readonly Dictionary<int, HashSet<AdventureTextData>> _elementDialogTextDict = new Dictionary<int, HashSet<AdventureTextData>>();

		// Token: 0x04007EB5 RID: 32437
		private readonly HashSet<AdventureTextData> _taiwuDialogTextHashSet = new HashSet<AdventureTextData>();

		// Token: 0x04007EB6 RID: 32438
		private readonly Dictionary<int, AdventureBlockIndex> waitUpdateShowTimeElementId = new Dictionary<int, AdventureBlockIndex>();

		// Token: 0x04007EB7 RID: 32439
		private readonly List<int> needRefreshIndexDialogElementId = new List<int>();

		// Token: 0x04007EB8 RID: 32440
		private readonly List<int> needRemoveDialogElementId = new List<int>();

		// Token: 0x04007EB9 RID: 32441
		private readonly Dictionary<AdventureBlockIndex, bool> _cloudDict = new Dictionary<AdventureBlockIndex, bool>();

		// Token: 0x04007EBA RID: 32442
		private const int CloudStyleCount = 3;

		// Token: 0x04007EBB RID: 32443
		private readonly float _cloudFadeDuration = 0.6f;

		// Token: 0x04007EBC RID: 32444
		private readonly float _elementFadeDuration = 0.3f;

		// Token: 0x04007EBD RID: 32445
		private readonly List<AdventureBlockIndex> _inRangeIndices = new List<AdventureBlockIndex>();

		// Token: 0x04007EBE RID: 32446
		private bool _isShowingInfluenceRange;

		// Token: 0x04007EBF RID: 32447
		private Color _influenceColor;

		// Token: 0x04007EC0 RID: 32448
		private AdventureBlockIndex _influenceRangeCenter;

		// Token: 0x04007EC1 RID: 32449
		private int _influenceRange;

		// Token: 0x04007EC2 RID: 32450
		private int _currentInfluenceStyle;

		// Token: 0x04007EC3 RID: 32451
		private readonly Dictionary<AdventureBlockIndex, int> _edgeBlockIndexDict = new Dictionary<AdventureBlockIndex, int>();

		// Token: 0x04007EC4 RID: 32452
		private readonly List<Vector2> _influenceEdgeVertices = new List<Vector2>();

		// Token: 0x04007EC5 RID: 32453
		private readonly List<EAdventureDirection> directionList = new List<EAdventureDirection>
		{
			EAdventureDirection.Left,
			EAdventureDirection.Up,
			EAdventureDirection.Right,
			EAdventureDirection.Down
		};

		// Token: 0x04007EC6 RID: 32454
		private readonly List<List<EAdventureDirection>> directionCircleUp = new List<List<EAdventureDirection>>
		{
			new List<EAdventureDirection>
			{
				EAdventureDirection.Up
			},
			new List<EAdventureDirection>
			{
				EAdventureDirection.Up,
				EAdventureDirection.Right
			},
			new List<EAdventureDirection>
			{
				EAdventureDirection.Right
			},
			new List<EAdventureDirection>
			{
				EAdventureDirection.Right,
				EAdventureDirection.Down
			},
			new List<EAdventureDirection>
			{
				EAdventureDirection.Down
			},
			new List<EAdventureDirection>
			{
				EAdventureDirection.Down,
				EAdventureDirection.Left
			},
			new List<EAdventureDirection>
			{
				EAdventureDirection.Left
			},
			new List<EAdventureDirection>
			{
				EAdventureDirection.Left,
				EAdventureDirection.Up
			}
		};

		// Token: 0x04007EC7 RID: 32455
		private readonly List<List<EAdventureDirection>> directionCircleLeft = new List<List<EAdventureDirection>>
		{
			new List<EAdventureDirection>
			{
				EAdventureDirection.Right
			},
			new List<EAdventureDirection>
			{
				EAdventureDirection.Right,
				EAdventureDirection.Down
			},
			new List<EAdventureDirection>
			{
				EAdventureDirection.Down
			},
			new List<EAdventureDirection>
			{
				EAdventureDirection.Down,
				EAdventureDirection.Left
			},
			new List<EAdventureDirection>
			{
				EAdventureDirection.Left
			},
			new List<EAdventureDirection>
			{
				EAdventureDirection.Left,
				EAdventureDirection.Up
			},
			new List<EAdventureDirection>
			{
				EAdventureDirection.Up
			},
			new List<EAdventureDirection>
			{
				EAdventureDirection.Up,
				EAdventureDirection.Right
			}
		};

		// Token: 0x04007EC8 RID: 32456
		[SerializeField]
		private float jumpPower = 50f;

		// Token: 0x04007EC9 RID: 32457
		private PoolItem _pawnTemplatePool;

		// Token: 0x04007ECA RID: 32458
		private Dictionary<int, AdventureBlockIndex> _prevPawnCoordinates = new Dictionary<int, AdventureBlockIndex>();

		// Token: 0x04007ECB RID: 32459
		[TupleElementNames(new string[]
		{
			"preDataIndex",
			"toDataIndex"
		})]
		private Dictionary<int, ValueTuple<AdventureBlockIndex, AdventureBlockIndex>> _movingPawns = new Dictionary<int, ValueTuple<AdventureBlockIndex, AdventureBlockIndex>>();

		// Token: 0x04007ECC RID: 32460
		private Dictionary<int, GameObject> _movingPawnActualImages = new Dictionary<int, GameObject>();

		// Token: 0x04007ECD RID: 32461
		private List<GameObject> _movingPawnImages = new List<GameObject>();

		// Token: 0x04007ECE RID: 32462
		[TupleElementNames(new string[]
		{
			"coreId",
			"visibleIndex"
		})]
		private Dictionary<AdventureBlockIndex, List<ValueTuple<int, int>>> _elementDeleteAnimDict = new Dictionary<AdventureBlockIndex, List<ValueTuple<int, int>>>();

		// Token: 0x04007ECF RID: 32463
		[SerializeField]
		private float elementsFadeTime = 0.3f;

		// Token: 0x04007ED0 RID: 32464
		private PoolItem _elementsFadeTemplatePool;

		// Token: 0x04007ED1 RID: 32465
		private PoolItem _dialogPool;

		// Token: 0x04007ED2 RID: 32466
		private PoolItem _overlapHolderPool;

		// Token: 0x04007ED3 RID: 32467
		private Dictionary<AdventureBlockIndex, List<AdventureElement>> _preVisibleElementMap = new Dictionary<AdventureBlockIndex, List<AdventureElement>>();

		// Token: 0x04007ED4 RID: 32468
		private float _progressDuration;

		// Token: 0x04007ED5 RID: 32469
		private float _checkDuration;

		// Token: 0x04007ED6 RID: 32470
		private bool _actionCanContinue;

		// Token: 0x04007ED7 RID: 32471
		private bool _cameraMoving;

		// Token: 0x04007ED9 RID: 32473
		private int _taiwuAdvanceMovePointLeft;

		// Token: 0x04007EDA RID: 32474
		private InfinityScroll _taiwuStateHolder;

		// Token: 0x04007EDB RID: 32475
		[TupleElementNames(new string[]
		{
			"paramData",
			"paramValue"
		})]
		private List<List<ValueTuple<AdventureParameterData, AdventureParameterValue>>> _taiwuStateLineList;

		// Token: 0x04007EDC RID: 32476
		private int _viewTypeNear;

		// Token: 0x04007EDD RID: 32477
		private int _viewTypeFar;

		// Token: 0x04007EDE RID: 32478
		private int _viewStyle;

		// Token: 0x04007EDF RID: 32479
		private readonly Color _colorNear = Color.white;

		// Token: 0x04007EE0 RID: 32480
		private readonly Color _colorFar = new Color(0.7f, 0.7f, 0.7f);

		// Token: 0x04007EE1 RID: 32481
		private readonly Color _colorOutView = new Color(0.4f, 0.4f, 0.4f);

		// Token: 0x04007EE2 RID: 32482
		private bool _simpleViewModeOn;

		// Token: 0x04007EE3 RID: 32483
		[SerializeField]
		public Color selectElementRangeColor = Color.grey;

		// Token: 0x04007EE4 RID: 32484
		[SerializeField]
		public Color selectElementEdgeColor = Color.grey;

		// Token: 0x04007EE5 RID: 32485
		private bool startSelectElement;

		// Token: 0x04007EE6 RID: 32486
		private AdventureBlockIndex selectElementCenterDataIndex;

		// Token: 0x04007EE7 RID: 32487
		private int selectElementRange;

		// Token: 0x04007EE8 RID: 32488
		private string selectElementSaveKey;

		// Token: 0x04007EE9 RID: 32489
		private const int Distance = 3;

		// Token: 0x04007EEA RID: 32490
		private List<MapBlockData> _outerBlockPool;

		// Token: 0x020023B1 RID: 9137
		public struct ElementDisplayItem
		{
			// Token: 0x06010416 RID: 66582 RVA: 0x00658FC8 File Offset: 0x006571C8
			public static ViewAdventureRemake.ElementDisplayItem FromElement(AdventureElement element)
			{
				return new ViewAdventureRemake.ElementDisplayItem
				{
					Element = element,
					IsExitItem = false,
					BlockIndex = element.Index
				};
			}

			// Token: 0x06010417 RID: 66583 RVA: 0x00658FFC File Offset: 0x006571FC
			public static ViewAdventureRemake.ElementDisplayItem CreateExitItem(AdventureBlockIndex index)
			{
				return new ViewAdventureRemake.ElementDisplayItem
				{
					Element = null,
					IsExitItem = true,
					BlockIndex = index
				};
			}

			// Token: 0x0400DF9D RID: 57245
			public AdventureElement Element;

			// Token: 0x0400DF9E RID: 57246
			public bool IsExitItem;

			// Token: 0x0400DF9F RID: 57247
			public AdventureBlockIndex BlockIndex;
		}

		// Token: 0x020023B2 RID: 9138
		private enum BlockEffectType
		{
			// Token: 0x0400DFA1 RID: 57249
			Single,
			// Token: 0x0400DFA2 RID: 57250
			LoopDown,
			// Token: 0x0400DFA3 RID: 57251
			LoopTop
		}
	}
}
