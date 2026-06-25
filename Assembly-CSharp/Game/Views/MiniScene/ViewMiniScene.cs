using System;
using Config;
using DG.Tweening;
using FrameWork;
using GameData.Domains.Map;
using GameData.Domains.World;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.MiniScene
{
	// Token: 0x020008DB RID: 2267
	public class ViewMiniScene : UIBase
	{
		// Token: 0x17000CB1 RID: 3249
		// (get) Token: 0x06006CB5 RID: 27829 RVA: 0x00322674 File Offset: 0x00320874
		private WorldMapModel MapModel
		{
			get
			{
				return SingletonObject.getInstance<WorldMapModel>();
			}
		}

		// Token: 0x06006CB6 RID: 27830 RVA: 0x0032267B File Offset: 0x0032087B
		public override void OnInit(ArgumentBox argsBox)
		{
		}

		// Token: 0x06006CB7 RID: 27831 RVA: 0x00322680 File Offset: 0x00320880
		private void Awake()
		{
			this._backRect = this.back.rectTransform;
			this._middleRect = this.middle.rectTransform;
			this._frontRect = this.front.rectTransform;
			GEvent.Add(UiEvents.WorldMapPlayerBlockChange, new GEvent.Callback(this.OnWorldMapPlayerBlockChange));
			GEvent.Add(UiEvents.WorldMapEnterNewArea, new GEvent.Callback(this.OnWorldMapEnterNewArea));
			GEvent.Add(UiEvents.TutorialVideoSwitchToMiniSize, new GEvent.Callback(this.TutorialVideoSwitchToMiniSize));
			GEvent.Add(UiEvents.TopUiChanged, new GEvent.Callback(this.TopUiChanged));
			GEvent.Add(UiEvents.PlayAnimToHideMainUI, new GEvent.Callback(this.PlayAnimToHideMainUI));
			GEvent.Add(UiEvents.PlayAnimToShowMainUI, new GEvent.Callback(this.PlayAnimToShowMainUI));
			GEvent.Add(EEvents.OnMonthChange, new GEvent.Callback(this.RefreshImg));
			GEvent.Add(UiEvents.TaiwuVillageShowShrineChange, new GEvent.Callback(this.RefreshImg));
			this.ResetView();
		}

		// Token: 0x06006CB8 RID: 27832 RVA: 0x00322794 File Offset: 0x00320994
		private void OnDestroy()
		{
			GEvent.Remove(UiEvents.WorldMapPlayerBlockChange, new GEvent.Callback(this.OnWorldMapPlayerBlockChange));
			GEvent.Remove(UiEvents.WorldMapEnterNewArea, new GEvent.Callback(this.OnWorldMapEnterNewArea));
			GEvent.Remove(UiEvents.TutorialVideoSwitchToMiniSize, new GEvent.Callback(this.TutorialVideoSwitchToMiniSize));
			GEvent.Remove(UiEvents.TopUiChanged, new GEvent.Callback(this.TopUiChanged));
			GEvent.Remove(UiEvents.PlayAnimToHideMainUI, new GEvent.Callback(this.PlayAnimToHideMainUI));
			GEvent.Remove(UiEvents.PlayAnimToShowMainUI, new GEvent.Callback(this.PlayAnimToShowMainUI));
			GEvent.Remove(EEvents.OnMonthChange, new GEvent.Callback(this.RefreshImg));
			GEvent.Remove(UiEvents.TaiwuVillageShowShrineChange, new GEvent.Callback(this.RefreshImg));
			DOTween.Kill(this.back, false);
			DOTween.Kill(this.middle, false);
			DOTween.Kill(this.front, false);
			DOTween.Kill(this.atmosphere, false);
			DOTween.Kill(this._backRect, false);
			DOTween.Kill(this._middleRect, false);
			DOTween.Kill(this._frontRect, false);
			DOTween.Kill(this.canvasGroup, false);
		}

		// Token: 0x06006CB9 RID: 27833 RVA: 0x003228D5 File Offset: 0x00320AD5
		private void PlayAnimToShowMainUI(ArgumentBox argBox)
		{
			this.mainUIHide = false;
			this.TopUiChanged(argBox);
		}

		// Token: 0x06006CBA RID: 27834 RVA: 0x003228E7 File Offset: 0x00320AE7
		private void PlayAnimToHideMainUI(ArgumentBox argBox)
		{
			this.mainUIHide = true;
			this.TopUiChanged(argBox);
		}

		// Token: 0x06006CBB RID: 27835 RVA: 0x003228FC File Offset: 0x00320AFC
		private void TopUiChanged(ArgumentBox argBox)
		{
			bool flag = !SingletonObject.getInstance<GlobalSettings>().MiniScene;
			if (flag)
			{
				this.canvasGroup.alpha = 0f;
			}
			else
			{
				bool show = !this.mainUIHide && (UIManager.Instance.IsFocusElement(UIElement.StateMainWorld) || UIManager.Instance.IsFocusElement(UIElement.PopupMenu));
				this.canvasGroup.DOFade((float)(show ? 1 : 0), this.fadeDuration);
				bool flag2 = show;
				if (flag2)
				{
					this.OnWorldMapPlayerBlockChange(argBox);
				}
			}
		}

		// Token: 0x06006CBC RID: 27836 RVA: 0x00322988 File Offset: 0x00320B88
		private void TutorialVideoSwitchToMiniSize(ArgumentBox argBox)
		{
			bool isFocus = UIManager.Instance.IsFocusElement(UIElement.StateMainWorld);
			bool flag = isFocus;
			if (flag)
			{
				this.canvasGroup.DOFade(1f, this.fadeDuration);
			}
			this.OnWorldMapPlayerBlockChange(argBox);
		}

		// Token: 0x06006CBD RID: 27837 RVA: 0x003229CA File Offset: 0x00320BCA
		private void OnWorldMapEnterNewArea(ArgumentBox argBox)
		{
			this.OnWorldMapPlayerBlockChange(argBox);
		}

		// Token: 0x06006CBE RID: 27838 RVA: 0x003229D8 File Offset: 0x00320BD8
		private void OnWorldMapPlayerBlockChange(ArgumentBox argBox)
		{
			MapBlockData currBlock = this.MapModel.PlayerAtBlock;
			bool flag = currBlock == null;
			if (!flag)
			{
				MapBlockItem currConfigData = MapBlock.Instance[currBlock.TemplateId];
				bool flag2 = currConfigData == null;
				if (!flag2)
				{
					bool atSettlement = currConfigData.BuildingAreaWidth > 0;
					ViewMiniScene.BlockChangeType settlementChangeType = this.GetChangeType(this._atSettlement, atSettlement);
					this._atSettlement = atSettlement;
					this.loadAtSettlement = (atSettlement && !this._atSettlementRange);
					bool atSettlementRange = currBlock.BelongBlockId >= 0 || currConfigData.BuildingAreaWidth > 0;
					ViewMiniScene.BlockChangeType settlementRangeChangeType = this.GetChangeType(this._atSettlementRange, atSettlementRange);
					this._atSettlementRange = atSettlementRange;
					bool flag3 = currConfigData.SubType == EMapBlockSubType.SwordTomb;
					ViewMiniScene.BlockChangeType swordTombChangeType;
					if (flag3)
					{
						bool atSwordTomb = currConfigData.SubType == EMapBlockSubType.SwordTomb;
						swordTombChangeType = this.GetChangeType(this._atSwordTomb, atSwordTomb);
						this._atSwordTomb = atSwordTomb;
					}
					else
					{
						bool flag4 = currBlock.RootBlockId >= 0;
						if (flag4)
						{
							MapBlockData rootBlockData = this.MapModel.GetBlockData(currBlock.RootBlockId);
							MapBlockItem rootConfigData = MapBlock.Instance[rootBlockData.TemplateId];
							bool atSwordTomb2 = rootConfigData.SubType == EMapBlockSubType.SwordTomb;
							swordTombChangeType = this.GetChangeType(this._atSwordTomb, atSwordTomb2);
							this._atSwordTomb = atSwordTomb2;
						}
						else
						{
							swordTombChangeType = this.GetChangeType(this._atSwordTomb, false);
							this._atSwordTomb = false;
						}
					}
					this.HandleSettlementEvents(settlementChangeType, settlementRangeChangeType, swordTombChangeType, currConfigData);
				}
			}
		}

		// Token: 0x06006CBF RID: 27839 RVA: 0x00322B44 File Offset: 0x00320D44
		private ViewMiniScene.BlockChangeType GetChangeType(bool prevState, bool currState)
		{
			bool flag = prevState && !currState;
			ViewMiniScene.BlockChangeType result;
			if (flag)
			{
				result = ViewMiniScene.BlockChangeType.Leave;
			}
			else
			{
				bool flag2 = !prevState && currState;
				if (flag2)
				{
					result = ViewMiniScene.BlockChangeType.Enter;
				}
				else
				{
					bool flag3 = !prevState && !currState;
					if (flag3)
					{
						result = ViewMiniScene.BlockChangeType.SameLeave;
					}
					else
					{
						result = ViewMiniScene.BlockChangeType.SameEnter;
					}
				}
			}
			return result;
		}

		// Token: 0x06006CC0 RID: 27840 RVA: 0x00322B8C File Offset: 0x00320D8C
		private void HandleSettlementEvents(ViewMiniScene.BlockChangeType settlementChangeType, ViewMiniScene.BlockChangeType settlementRangeChangeType, ViewMiniScene.BlockChangeType swordTombChangeType, MapBlockItem mapBlockItem)
		{
			bool flag = settlementRangeChangeType == ViewMiniScene.BlockChangeType.Enter;
			if (flag)
			{
				this._rangeMapBlockConfig = mapBlockItem;
				bool flag2 = this.SettlementRangeOnlyHaveFront(mapBlockItem);
				if (flag2)
				{
					this.SetFrontImg(this.GetBlockMiniSceneImg(mapBlockItem, ViewMiniScene.ShotType.Front));
					this.PlayEnterAnimation(this.front, this._frontRect, this.endPosY, 0f, null);
				}
				else
				{
					bool flag3 = this.SettlementRangeOnlyHaveMiddle(mapBlockItem);
					if (flag3)
					{
						this.SetMiddleImg(this.GetBlockMiniSceneImg(mapBlockItem, ViewMiniScene.ShotType.Middle));
						this.PlayEnterAnimation(this.middle, this._middleRect, this.endPosY, 0f, null);
					}
					else
					{
						bool flag4 = this.loadAtSettlement;
						if (flag4)
						{
							this._rangeMapBlockConfig = MapBlock.Instance[39];
						}
						this.SetBackImg(this.GetBlockMiniSceneImg(this._rangeMapBlockConfig, ViewMiniScene.ShotType.Back));
						this.SetMiddleImg(this.GetBlockMiniSceneImg(this._rangeMapBlockConfig, ViewMiniScene.ShotType.Middle));
						this.PlayEnterAnimation(this.back, this._backRect, this.endPosY, 0f, null);
						this.PlayEnterAnimation(this.middle, this._middleRect, this.endPosY, this.delayDuration, null);
					}
				}
			}
			else
			{
				bool flag5 = settlementRangeChangeType == ViewMiniScene.BlockChangeType.Leave;
				if (flag5)
				{
					this.PlayLeaveAnimation(this.back, this._backRect, 0f, null);
					this.PlayLeaveAnimation(this.front, this._frontRect, 0f, null);
					this.PlayLeaveAnimation(this.middle, this._middleRect, this.delayDuration, null);
				}
			}
			bool flag6 = settlementChangeType == ViewMiniScene.BlockChangeType.Enter;
			if (flag6)
			{
				this._currMapBlockConfig = mapBlockItem;
				this.SetFrontImg(this.GetBlockMiniSceneImg(mapBlockItem, ViewMiniScene.ShotType.Front));
				this.PlayEnterAnimation(this.front, this._frontRect, this.endPosY, 0f, new Graphic[]
				{
					this.back,
					this.middle
				});
				this.PlayAtmosphereEnterAnimation(this.atmosphere, this.normalAtmosphereColor);
			}
			else
			{
				bool flag7 = settlementChangeType == ViewMiniScene.BlockChangeType.Leave;
				if (flag7)
				{
					this.PlayLeaveAnimation(this.front, this._frontRect, 0f, new Graphic[]
					{
						this.back,
						this.middle
					});
					this.PlayAtmosphereLeaveAnimation(this.atmosphere);
				}
			}
			bool flag8 = swordTombChangeType == ViewMiniScene.BlockChangeType.Enter;
			if (flag8)
			{
				this._currMapBlockConfig = mapBlockItem;
				this.SetBackImg(this.GetBlockMiniSceneImg(mapBlockItem, ViewMiniScene.ShotType.Back));
				this.SetFrontImg(this.GetBlockMiniSceneImg(mapBlockItem, ViewMiniScene.ShotType.Front));
				this.PlayEnterAnimation(this.back, this._backRect, this.endPosY, 0f, null);
				this.PlayEnterAnimation(this.front, this._frontRect, this.endPosY, this.delayDuration, null);
				this.PlayAtmosphereEnterAnimation(this.atmosphere, this.swordTombAtmosphereColor);
			}
			else
			{
				bool flag9 = swordTombChangeType == ViewMiniScene.BlockChangeType.Leave;
				if (flag9)
				{
					this.PlayLeaveAnimation(this.back, this._backRect, 0f, null);
					this.PlayLeaveAnimation(this.front, this._frontRect, this.delayDuration, null);
					this.PlayAtmosphereLeaveAnimation(this.atmosphere);
				}
			}
		}

		// Token: 0x06006CC1 RID: 27841 RVA: 0x00322EA0 File Offset: 0x003210A0
		private void PlayEnterAnimation(Graphic targetGraphic, RectTransform targetRect, float targetPosY, float delay = 0f, Graphic[] dimTargets = null)
		{
			bool flag = targetGraphic == null || targetRect == null;
			if (!flag)
			{
				targetGraphic.DOKill(false);
				targetRect.DOKill(false);
				this.leaveSequence.Kill(false);
				this.enterSequence = DOTween.Sequence();
				this.enterSequence.Join(targetGraphic.DOFade(1f, this.fadeDuration)).SetDelay(delay);
				float currentPosY = targetRect.anchoredPosition.y;
				bool flag2 = !Mathf.Approximately(currentPosY, targetPosY);
				if (flag2)
				{
					this.enterSequence.Join(targetRect.DOLocalMoveY(targetPosY, this.moveDuration, false)).SetDelay(delay);
				}
				bool flag3 = dimTargets != null && dimTargets.Length > 0;
				if (flag3)
				{
					this.enterSequence.OnComplete(delegate
					{
						this.DimBackgrounds(dimTargets);
					});
				}
				this.enterSequence.Play<Sequence>();
			}
		}

		// Token: 0x06006CC2 RID: 27842 RVA: 0x00322FA8 File Offset: 0x003211A8
		private void PlayLeaveAnimation(Graphic targetGraphic, RectTransform targetRect, float delay = 0f, Graphic[] restoreTargets = null)
		{
			bool flag = targetGraphic == null || targetRect == null;
			if (!flag)
			{
				targetGraphic.DOKill(false);
				targetRect.DOKill(false);
				bool flag2 = restoreTargets != null && restoreTargets.Length > 0;
				if (flag2)
				{
					this.RestoreBackgrounds(restoreTargets);
				}
				this.enterSequence.Kill(false);
				this.leaveSequence = DOTween.Sequence();
				this.leaveSequence.Join(targetGraphic.DOFade(0f, this.fadeDuration)).SetDelay(delay);
				float currentPosY = targetRect.anchoredPosition.y;
				bool flag3 = !Mathf.Approximately(currentPosY, this.startPosY);
				if (flag3)
				{
					this.leaveSequence.Join(targetRect.DOLocalMoveY(this.startPosY, this.moveDuration, false)).SetDelay(delay);
				}
				this.leaveSequence.Play<Sequence>();
			}
		}

		// Token: 0x06006CC3 RID: 27843 RVA: 0x0032308A File Offset: 0x0032128A
		private void PlayAtmosphereEnterAnimation(Graphic targetGraphic, Color color)
		{
			targetGraphic.color = color;
			targetGraphic.DOFade(1f, this.fadeDuration);
		}

		// Token: 0x06006CC4 RID: 27844 RVA: 0x003230A7 File Offset: 0x003212A7
		private void PlayAtmosphereLeaveAnimation(Graphic targetGraphic)
		{
			targetGraphic.DOFade(0f, this.fadeDuration);
		}

		// Token: 0x06006CC5 RID: 27845 RVA: 0x003230BC File Offset: 0x003212BC
		private void DimBackgrounds(Graphic[] targets)
		{
			foreach (Graphic target in targets)
			{
				bool flag = target == null;
				if (!flag)
				{
					target.DOKill(false);
					Color currentColor = target.color;
					Color targetColor = new Color(currentColor.r * 0.5f, currentColor.g * 0.5f, currentColor.b * 0.5f, currentColor.a);
					float duration = 0.5f;
					target.DOColor(targetColor, duration);
				}
			}
		}

		// Token: 0x06006CC6 RID: 27846 RVA: 0x00323144 File Offset: 0x00321344
		private void RestoreBackgrounds(Graphic[] targets)
		{
			foreach (Graphic target in targets)
			{
				bool flag = target == null;
				if (!flag)
				{
					target.DOKill(false);
					target.color = new Color(1f, 1f, 1f, target.color.a);
				}
			}
		}

		// Token: 0x06006CC7 RID: 27847 RVA: 0x003231A4 File Offset: 0x003213A4
		private void ResetView()
		{
			this.SetInitialState(this.back, this._backRect);
			this.SetInitialState(this.middle, this._middleRect);
			this.SetInitialState(this.front, this._frontRect);
			this.SetInitialState(this.atmosphere, null);
		}

		// Token: 0x06006CC8 RID: 27848 RVA: 0x003231FC File Offset: 0x003213FC
		private void SetInitialState(Graphic graphic, RectTransform rect)
		{
			bool flag = graphic == null;
			if (!flag)
			{
				graphic.DOKill(false);
				bool flag2 = rect != null;
				if (flag2)
				{
					rect.DOKill(false);
				}
				Color c = graphic.color;
				c.a = 0f;
				graphic.color = c;
				bool flag3 = rect != null;
				if (flag3)
				{
					Vector3 pos = rect.localPosition;
					pos.y = this.startPosY;
					rect.localPosition = pos;
				}
			}
		}

		// Token: 0x06006CC9 RID: 27849 RVA: 0x00323278 File Offset: 0x00321478
		private void RefreshImg(ArgumentBox argBox)
		{
			bool flag = this._atSettlementRange && this._rangeMapBlockConfig != null;
			if (flag)
			{
				bool flag2 = this.SettlementRangeOnlyHaveFront(this._rangeMapBlockConfig);
				if (flag2)
				{
					this.SetFrontImg(this.GetBlockMiniSceneImg(this._rangeMapBlockConfig, ViewMiniScene.ShotType.Front));
				}
				else
				{
					bool flag3 = this.SettlementRangeOnlyHaveMiddle(this._rangeMapBlockConfig);
					if (flag3)
					{
						this.SetMiddleImg(this.GetBlockMiniSceneImg(this._rangeMapBlockConfig, ViewMiniScene.ShotType.Middle));
					}
					else
					{
						this.SetBackImg(this.GetBlockMiniSceneImg(this._rangeMapBlockConfig, ViewMiniScene.ShotType.Back));
						this.SetMiddleImg(this.GetBlockMiniSceneImg(this._rangeMapBlockConfig, ViewMiniScene.ShotType.Middle));
					}
				}
			}
			bool flag4 = this._atSettlement && this._currMapBlockConfig != null;
			if (flag4)
			{
				this.SetFrontImg(this.GetBlockMiniSceneImg(this._currMapBlockConfig, ViewMiniScene.ShotType.Front));
			}
			bool flag5 = this._atSwordTomb && this._currMapBlockConfig != null;
			if (flag5)
			{
				this.SetFrontImg(this.GetBlockMiniSceneImg(this._currMapBlockConfig, ViewMiniScene.ShotType.Front));
				this.SetBackImg(this.GetBlockMiniSceneImg(this._currMapBlockConfig, ViewMiniScene.ShotType.Back));
			}
		}

		// Token: 0x06006CCA RID: 27850 RVA: 0x0032338C File Offset: 0x0032158C
		private bool SettlementRangeOnlyHaveFront(MapBlockItem config)
		{
			short templateId = config.TemplateId;
			return templateId == 37 || templateId == 38 || templateId == 17 || templateId == 18;
		}

		// Token: 0x06006CCB RID: 27851 RVA: 0x003233C4 File Offset: 0x003215C4
		private bool SettlementRangeOnlyHaveMiddle(MapBlockItem config)
		{
			EMapBlockSubType subType = config.SubType;
			return subType == EMapBlockSubType.Wild || subType == EMapBlockSubType.DLCLoong || subType == EMapBlockSubType.Ruin || subType == EMapBlockSubType.DarkPool;
		}

		// Token: 0x06006CCC RID: 27852 RVA: 0x003233FA File Offset: 0x003215FA
		private void SetFrontImg(string img)
		{
			CommonUtils.SetRawImage(this.front, this.texturePath + img, false);
		}

		// Token: 0x06006CCD RID: 27853 RVA: 0x00323416 File Offset: 0x00321616
		private void SetMiddleImg(string img)
		{
			CommonUtils.SetRawImage(this.middle, this.texturePath + img, false);
		}

		// Token: 0x06006CCE RID: 27854 RVA: 0x00323432 File Offset: 0x00321632
		private void SetBackImg(string img)
		{
			CommonUtils.SetRawImage(this.back, this.texturePath + img, false);
		}

		// Token: 0x06006CCF RID: 27855 RVA: 0x00323450 File Offset: 0x00321650
		private string GetBlockMiniSceneImg(MapBlockItem mapBlockItem, ViewMiniScene.ShotType shotType)
		{
			string artName = mapBlockItem.Art;
			short areaTemplateId = this.MapModel.GetCurrentAreaTemplateId();
			MapAreaItem areaConfig = MapArea.Instance[areaTemplateId];
			EMapAreaAreaDirection areaDirection = areaConfig.AreaDirection;
			string shotTypeStr = this.GetShortTypeStr(shotType);
			string artNameStr = this.HandleArtName(artName);
			string directionStr = this.GetDirectionStr(areaDirection, mapBlockItem);
			string winterStr = this.GetWinterStr(mapBlockItem);
			bool flag = mapBlockItem.SubType == EMapBlockSubType.TaiwuCun;
			string result;
			if (flag)
			{
				bool flag2 = shotType > ViewMiniScene.ShotType.Front;
				if (flag2)
				{
					Debug.LogWarning(string.Format("TaiwuCun only supports Front shot type, got {0}", shotType));
				}
				string suffix = this.MapModel.TaiwuVillageShowShrine ? "_2" : "_1";
				result = string.Concat(new string[]
				{
					this.textureNamePrefix,
					shotTypeStr,
					artNameStr,
					suffix,
					directionStr,
					winterStr
				});
			}
			else
			{
				bool flag3 = mapBlockItem.SubType == EMapBlockSubType.SwordTomb;
				if (flag3)
				{
					bool flag4 = shotType == ViewMiniScene.ShotType.Back;
					if (flag4)
					{
						return this.textureNamePrefix + shotTypeStr + "_sword_tomb";
					}
					bool flag5 = shotType == ViewMiniScene.ShotType.Front;
					if (flag5)
					{
						string numStr = this.GetNumStr(mapBlockItem);
						return string.Concat(new string[]
						{
							this.textureNamePrefix,
							shotTypeStr,
							artNameStr,
							numStr,
							directionStr,
							winterStr
						});
					}
				}
				string defaultNumStr = this.GetNumStr(mapBlockItem);
				result = string.Concat(new string[]
				{
					this.textureNamePrefix,
					shotTypeStr,
					artNameStr,
					defaultNumStr,
					directionStr,
					winterStr
				});
			}
			return result;
		}

		// Token: 0x06006CD0 RID: 27856 RVA: 0x003235E4 File Offset: 0x003217E4
		private string GetDirectionStr(EMapAreaAreaDirection direction, MapBlockItem mapBlockItem)
		{
			bool flag = !mapBlockItem.MiniSceneHaveDirection;
			string result;
			if (flag)
			{
				result = "";
			}
			else
			{
				bool flag2 = direction == EMapAreaAreaDirection.North;
				if (flag2)
				{
					result = "_n";
				}
				else
				{
					bool flag3 = direction == EMapAreaAreaDirection.South;
					if (flag3)
					{
						result = "_s";
					}
					else
					{
						result = "_w";
					}
				}
			}
			return result;
		}

		// Token: 0x06006CD1 RID: 27857 RVA: 0x00323630 File Offset: 0x00321830
		private string GetShortTypeStr(ViewMiniScene.ShotType shotType)
		{
			bool flag = shotType == ViewMiniScene.ShotType.Back;
			string result;
			if (flag)
			{
				result = "_back";
			}
			else
			{
				bool flag2 = shotType == ViewMiniScene.ShotType.Front;
				if (flag2)
				{
					result = "_front";
				}
				else
				{
					result = "_mid";
				}
			}
			return result;
		}

		// Token: 0x06006CD2 RID: 27858 RVA: 0x00323668 File Offset: 0x00321868
		private string HandleArtName(string artName)
		{
			bool flag = artName.Contains("_big");
			if (flag)
			{
				artName = artName.Replace("_big", "");
			}
			return "_" + artName;
		}

		// Token: 0x06006CD3 RID: 27859 RVA: 0x003236A8 File Offset: 0x003218A8
		private string GetNumStr(MapBlockItem mapBlockItem)
		{
			bool flag = mapBlockItem.MiniSceneNumbers.Length == 0;
			string result;
			if (flag)
			{
				result = "";
			}
			else
			{
				result = "_" + mapBlockItem.MiniSceneNumbers[Random.Range(0, mapBlockItem.MiniSceneNumbers.Length)].ToString();
			}
			return result;
		}

		// Token: 0x06006CD4 RID: 27860 RVA: 0x003236F8 File Offset: 0x003218F8
		private string GetWinterStr(MapBlockItem mapBlockItem)
		{
			bool flag = mapBlockItem.MiniSceneHaveWinter && TimeKit.GetCurrSeason() == 3;
			string result;
			if (flag)
			{
				result = "_winter";
			}
			else
			{
				result = "";
			}
			return result;
		}

		// Token: 0x04004EE8 RID: 20200
		[SerializeField]
		private CRawImage back;

		// Token: 0x04004EE9 RID: 20201
		[SerializeField]
		private CRawImage middle;

		// Token: 0x04004EEA RID: 20202
		[SerializeField]
		private CRawImage front;

		// Token: 0x04004EEB RID: 20203
		[SerializeField]
		private CRawImage atmosphere;

		// Token: 0x04004EEC RID: 20204
		[SerializeField]
		private CanvasGroup canvasGroup;

		// Token: 0x04004EED RID: 20205
		[Header("Animation Settings")]
		[SerializeField]
		private float fadeDuration = 0.33f;

		// Token: 0x04004EEE RID: 20206
		[SerializeField]
		private float moveDuration = 0.5f;

		// Token: 0x04004EEF RID: 20207
		[SerializeField]
		private float delayDuration = 0.1f;

		// Token: 0x04004EF0 RID: 20208
		[SerializeField]
		private float startPosY = -20f;

		// Token: 0x04004EF1 RID: 20209
		[SerializeField]
		private float endPosY = 0f;

		// Token: 0x04004EF2 RID: 20210
		private readonly string texturePath = "RemakeResources/Textures/MiniScene/";

		// Token: 0x04004EF3 RID: 20211
		private readonly string textureNamePrefix = "tex_block_scene";

		// Token: 0x04004EF4 RID: 20212
		private readonly Color normalAtmosphereColor = new Color(1f, 1f, 1f, 0f);

		// Token: 0x04004EF5 RID: 20213
		private readonly Color swordTombAtmosphereColor = new Color(0.682353f, 0.3882353f, 0.3137255f, 0f);

		// Token: 0x04004EF6 RID: 20214
		private bool _atSettlement;

		// Token: 0x04004EF7 RID: 20215
		private bool _atSettlementRange;

		// Token: 0x04004EF8 RID: 20216
		private bool loadAtSettlement;

		// Token: 0x04004EF9 RID: 20217
		private bool _atSwordTomb;

		// Token: 0x04004EFA RID: 20218
		private bool mainUIHide;

		// Token: 0x04004EFB RID: 20219
		private Sequence enterSequence;

		// Token: 0x04004EFC RID: 20220
		private Sequence leaveSequence;

		// Token: 0x04004EFD RID: 20221
		private RectTransform _backRect;

		// Token: 0x04004EFE RID: 20222
		private RectTransform _middleRect;

		// Token: 0x04004EFF RID: 20223
		private RectTransform _frontRect;

		// Token: 0x04004F00 RID: 20224
		private MapBlockItem _rangeMapBlockConfig;

		// Token: 0x04004F01 RID: 20225
		private MapBlockItem _currMapBlockConfig;

		// Token: 0x02001DDE RID: 7646
		private enum BlockChangeType
		{
			// Token: 0x0400C7BB RID: 51131
			Leave,
			// Token: 0x0400C7BC RID: 51132
			Enter,
			// Token: 0x0400C7BD RID: 51133
			SameEnter,
			// Token: 0x0400C7BE RID: 51134
			SameLeave
		}

		// Token: 0x02001DDF RID: 7647
		private enum ShotType
		{
			// Token: 0x0400C7C0 RID: 51136
			Front,
			// Token: 0x0400C7C1 RID: 51137
			Middle,
			// Token: 0x0400C7C2 RID: 51138
			Back
		}
	}
}
