using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using Game.Components.Character;
using Game.Components.ListStyleGeneralScroll.Item;
using Game.Components.SortAndFilter.Item.Apply;
using GameData.Domains.Building;
using GameData.Domains.Character;
using GameData.Domains.Character.AvatarSystem;
using GameData.Domains.Character.Display;
using GameData.Domains.CombatSkill;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.TaiwuEvent;
using GameData.Domains.World.Display;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.Obtain
{
	// Token: 0x020007DA RID: 2010
	public class ViewObtain : UIBase
	{
		// Token: 0x060061F3 RID: 25075 RVA: 0x002CEC50 File Offset: 0x002CCE50
		public override void OnInit(ArgumentBox argsBox)
		{
			bool flag = !argsBox.Get<Action>("CloseAction", out this._closeAction);
			if (flag)
			{
				this._closeAction = null;
			}
			bool flag2 = argsBox.Get<List<ItemDisplayData>>("ItemList", out this._itemList);
			if (flag2)
			{
				this._type = EObtainType.Item;
			}
			string format = "Init ViewObtain: {0}";
			List<ItemDisplayData> itemList = this._itemList;
			AdaptableLog.Info(string.Format(format, (itemList != null) ? itemList.Count : 0));
			bool flag3 = !argsBox.Get("InWareHouse", out this._warehouse);
			if (flag3)
			{
				this._warehouse = false;
			}
			bool flag4 = argsBox.Get<List<int>>("CharIdList", out this._charIdList);
			if (flag4)
			{
				this._type = EObtainType.Character;
			}
			bool flag5 = argsBox.Get<List<Chicken>>("ChickenList", out this._chickenList);
			if (flag5)
			{
				this._type = EObtainType.Chicken;
			}
			bool flag6 = argsBox.Get("CombatSkillId", out this._combatSkillId);
			if (flag6)
			{
				this._type = EObtainType.GetCombatSkill;
			}
			bool flag7 = argsBox.Get("BreakResult", out this._combatSkillBreakSuccess);
			if (flag7)
			{
				this._type = EObtainType.BreakCombatSkill;
			}
			bool flag8 = !argsBox.Get("CombatSkillMaxPower", out this._combatSkillMaxPower);
			if (flag8)
			{
				this._combatSkillMaxPower = -1;
			}
			bool flag9 = argsBox.Get<IntPair>("Proficiency", out this._combatSkillProficiency);
			if (flag9)
			{
				this._type = EObtainType.PracticeCombatSKill;
			}
			bool flag10 = !argsBox.Get<List<AvatarData>>("ShaveAvatar", out this._shaveAvatar);
			if (flag10)
			{
				this._shaveAvatar = new List<AvatarData>();
			}
			bool flag11 = !argsBox.Get("ShaveCharName", out this._shaveName);
			if (flag11)
			{
				this._shaveName = "";
			}
			bool flag12 = !argsBox.Get("ShaveCharAge", out this._shaveAge);
			if (flag12)
			{
				this._shaveAge = 16;
			}
			bool flag13 = argsBox.Get("ShaveResult", out this._shaveResult);
			if (flag13)
			{
				this._type = EObtainType.Shave;
			}
			bool flag14 = argsBox.Get<List<short>>("UnlockedDebateStrategyList", out this._unlockedDebateStrategyList);
			if (flag14)
			{
				this._type = EObtainType.DebateStrategy;
			}
			bool flag15 = argsBox.Get<List<short>>("LegacyList", out this._legacyList);
			if (flag15)
			{
				this._type = EObtainType.Legacy;
			}
			bool flag16 = argsBox.Get<List<short>>("FeatureList", out this._featureList);
			if (flag16)
			{
				this._type = EObtainType.Feature;
			}
			sbyte typeSbyte;
			bool flag17 = argsBox.Get("ObtainType", out typeSbyte);
			if (flag17)
			{
				this._type = (EObtainType)typeSbyte;
			}
			bool flag18 = !argsBox.Get("CallTriggerListner", out this._callTriggerListener);
			if (flag18)
			{
				this._callTriggerListener = true;
			}
			argsBox.Get("IsFree", out this._isFree);
			base.GetComponent<CanvasGroup>().alpha = 0f;
			this.title.SetSprite(string.Format(this._titles[this._type], SingletonObject.getInstance<GlobalSettings>().Language.ToLower()), false, null);
			this.titleBackGround.SetTexture(string.Format(this._titleColors[this._type], 0));
			this.backGround.SetTexture(string.Format(this._titleColors[this._type], 1));
		}

		// Token: 0x060061F4 RID: 25076 RVA: 0x002CEF60 File Offset: 0x002CD160
		private void Awake()
		{
			this.AudioIn = "ui_collect_get";
			CScrollRect rectCardScroll = this.cardScroll.GetComponent<CScrollRect>();
			rectCardScroll.SetClick(new Action(this.OnClickScroll));
			rectCardScroll.OnScrollEvent += this.OnDragScroll;
			this.cardScroll.OnItemRender += this.OnItemRender;
			this.cardScroll.OnItemHide += this.OnItemHide;
			this.btnClose.ClearAndAddListener(new Action(this.OnClick));
			this.switchToggleGroup.Init(1);
			this.switchToggleGroup.SetWithoutNotify(1);
			this.switchToggleGroup.OnActiveIndexChange += this.OnClickSwitch;
			this.itemScroll.Init("ViewObtainItem", ESortAndFilterControllerType.Item, false, new Action<ITradeableContent, RowItemLine>(this.OnItemLineRender), null, ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Amount | ItemListScroll.EColumnType.Type | ItemListScroll.EColumnType.Weight | ItemListScroll.EColumnType.Value | ItemListScroll.EColumnType.Durability, null, null, null);
		}

		// Token: 0x060061F5 RID: 25077 RVA: 0x002CF04C File Offset: 0x002CD24C
		private void OnEnable()
		{
			this._quickShow = false;
			this._showedIndex = 0;
			this._isListStyle = false;
			this.CloseAll();
			switch (this._type)
			{
			case EObtainType.Item:
			case EObtainType.GetLifeSkill:
			case EObtainType.GetCombatSkill:
			case EObtainType.Craft:
			case EObtainType.Repair:
			case EObtainType.Disassemble:
			case EObtainType.Refine:
			case EObtainType.IdentifyPoison:
			case EObtainType.AttachPoison:
			case EObtainType.DetoxPoison:
			case EObtainType.BuildingResourceReturn:
				this.ShowItem();
				break;
			case EObtainType.Legacy:
				this.ShowLegacy();
				break;
			case EObtainType.PracticeCombatSKill:
			case EObtainType.BreakCombatSkill:
				this.ShowCombatSkill();
				break;
			case EObtainType.Feature:
				this.ShowFeature();
				break;
			case EObtainType.Teammate:
			case EObtainType.Villager:
			case EObtainType.Kidnap:
			case EObtainType.Character:
				this.ShowCharacter();
				break;
			case EObtainType.Chicken:
				this.ShowChicken();
				break;
			case EObtainType.Shave:
				this.ShowShave();
				break;
			case EObtainType.DebateStrategy:
				this.ShowDebateStrategy();
				break;
			}
			this.switchToggleGroup.SetWithoutNotify(1);
			SingletonObject.getInstance<YieldHelper>().DelayFrameDo(10U, new Action(this.Open));
			SingletonObject.getInstance<YieldHelper>().DelayFrameDo(10U, delegate
			{
				this._canInteract = true;
				this.StartAnimation();
			});
		}

		// Token: 0x060061F6 RID: 25078 RVA: 0x002CF168 File Offset: 0x002CD368
		private void OnDisable()
		{
			bool flag = this.GetCount() > 0;
			if (flag)
			{
				this.cardScroll.SetDataCount(0);
			}
			bool callTriggerListener = this._callTriggerListener;
			if (callTriggerListener)
			{
				TaiwuEventDomainMethod.Call.TriggerListener("GetItemShowed", true);
				SingletonObject.getInstance<WorldMapModel>().ChangeTaiwuMoveState(WorldMapModel.MoveState.WaitEventShow);
			}
			Action closeAction = this._closeAction;
			if (closeAction != null)
			{
				closeAction();
			}
			List<ItemDisplayData> itemList = this._itemList;
			bool flag2 = itemList != null && itemList.Count > 0;
			if (flag2)
			{
				ItemKey itemKey = this._itemList[0].Key;
				bool flag3 = itemKey.ItemType == 12 && itemKey.TemplateId == 369;
				if (flag3)
				{
					TaiwuEventDomainMethod.Call.CloseUI("GetSectMainStoryFulongFeatherCoat", false, -1);
				}
			}
		}

		// Token: 0x060061F7 RID: 25079 RVA: 0x002CF220 File Offset: 0x002CD420
		private void Update()
		{
			bool keyDown = Input.GetKeyDown(KeyCode.Mouse0);
			if (keyDown)
			{
				this.OnBeginDragScroll();
			}
			else
			{
				bool flag = CommonCommandKit.RightMouse.Check(this.Element, false, false, false, true, false);
				if (flag)
				{
					this.OnClick();
				}
				else
				{
					bool flag2 = HotKeyCommand.CheckAnyKeyDown();
					if (flag2)
					{
						bool flag3 = !Input.GetKeyDown(KeyCode.Mouse1);
						if (flag3)
						{
							this.OnClick();
							AudioManager.Instance.PlaySound("ui_default_click_left", false, false);
						}
					}
				}
			}
		}

		// Token: 0x060061F8 RID: 25080 RVA: 0x002CF2A0 File Offset: 0x002CD4A0
		protected override void OnClick(Transform btn)
		{
			bool flag = btn.name == "CommonButtonClose";
			if (flag)
			{
				this.OnClick();
			}
		}

		// Token: 0x060061F9 RID: 25081 RVA: 0x002CF2CC File Offset: 0x002CD4CC
		private void OnClick()
		{
			bool flag = !this._canInteract;
			if (!flag)
			{
				bool flag2 = this._showedIndex >= this.GetCount();
				if (flag2)
				{
					this.QuickHide();
				}
				else
				{
					this.QuickCompleteAnimIn();
				}
			}
		}

		// Token: 0x060061FA RID: 25082 RVA: 0x002CF30E File Offset: 0x002CD50E
		private void OnBeginDragScroll()
		{
			this._isDragging = false;
		}

		// Token: 0x060061FB RID: 25083 RVA: 0x002CF318 File Offset: 0x002CD518
		private void OnDragScroll()
		{
			this._isDragging = true;
		}

		// Token: 0x060061FC RID: 25084 RVA: 0x002CF324 File Offset: 0x002CD524
		private void OnClickScroll()
		{
			bool flag = !this._isDragging;
			if (flag)
			{
				this.OnClick();
			}
		}

		// Token: 0x060061FD RID: 25085 RVA: 0x002CF348 File Offset: 0x002CD548
		private void OnItemLineRender(ITradeableContent data, RowItemLine line)
		{
			RowItemMain rowItemMain = line.GetComponentInChildren<RowItemMain>();
			ItemKey key = data.Key;
			bool isExp = key.ItemType == 12 && key.TemplateId == 8;
			bool isResource = ItemTemplateHelper.IsMiscResource(data.Key.ItemType, data.Key.TemplateId);
			rowItemMain.SetData(data);
			line.Set(rowItemMain, !isExp && !isResource);
			line.SetSelected(false);
			line.SetRowInteraction(false, -1, null);
		}

		// Token: 0x060061FE RID: 25086 RVA: 0x002CF3C0 File Offset: 0x002CD5C0
		private void OnItemRender(int index, GameObject obj)
		{
			switch (this._type)
			{
			case EObtainType.Item:
			case EObtainType.GetLifeSkill:
			case EObtainType.GetCombatSkill:
			case EObtainType.Craft:
			case EObtainType.Repair:
			case EObtainType.Disassemble:
			case EObtainType.Refine:
			case EObtainType.IdentifyPoison:
			case EObtainType.AttachPoison:
			case EObtainType.DetoxPoison:
			case EObtainType.BuildingResourceReturn:
			{
				AdaptableLog.Info(string.Format("Render Item: {0}, Total:{1}", index, this._itemList.Count));
				ItemDisplayData data = this._itemList[index];
				RowItemMain rowItemMain = obj.GetComponent<RowItemMain>();
				ItemKey key = data.Key;
				bool isExp = key.ItemType == 12 && key.TemplateId == 8;
				bool isResource = ItemTemplateHelper.IsMiscResource(data.Key.ItemType, data.Key.TemplateId);
				rowItemMain.SetData(this._itemList[index]);
				obj.GetComponent<CardItem>().Set(rowItemMain, !isExp && !isResource);
				break;
			}
			case EObtainType.Legacy:
				obj.GetComponent<GetLegacy>().Set(this._legacyList[index], this._isFree);
				break;
			case EObtainType.Feature:
				obj.GetComponent<Feature>().Set(this._featureList[index], -1, false, -1);
				break;
			case EObtainType.Teammate:
			case EObtainType.Villager:
			case EObtainType.Kidnap:
			case EObtainType.Character:
				obj.GetComponent<GetCharacter>().Set(this._characterDisplayData[index]);
				break;
			case EObtainType.Chicken:
				obj.GetComponent<GetChicken>().Set(this._chickenList[index]);
				break;
			case EObtainType.DebateStrategy:
				obj.GetComponent<GetStrategy>().Set(this._unlockedDebateStrategyList[index]);
				break;
			}
			bool quickShow = this._quickShow;
			if (quickShow)
			{
				obj.GetComponent<CanvasGroup>().alpha = 1f;
			}
		}

		// Token: 0x060061FF RID: 25087 RVA: 0x002CF588 File Offset: 0x002CD788
		private void OnItemHide(GameObject obj)
		{
			obj.GetComponent<CanvasGroup>().alpha = 0f;
			CardItem component = obj.GetComponent<CardItem>();
			if (component != null)
			{
				component.OnItemHide();
			}
		}

		// Token: 0x06006200 RID: 25088 RVA: 0x002CF5B0 File Offset: 0x002CD7B0
		private void OnClickSwitch(int previousIndex, int currentIndex)
		{
			bool flag = !this._canSwitchTypes.Contains(this._type);
			if (!flag)
			{
				this._isListStyle = (currentIndex == 1);
				this.cardScroll.gameObject.SetActive(!this._isListStyle);
				this.itemScroll.gameObject.SetActive(this._isListStyle && this._itemTypes.Contains(this._type));
				this.QuickCompleteAnimIn();
				this._quickShow = true;
			}
		}

		// Token: 0x06006201 RID: 25089 RVA: 0x002CF638 File Offset: 0x002CD838
		private void ShowItem()
		{
			bool flag = EObtainType.Repair == this._type;
			if (flag)
			{
				foreach (ItemDisplayData item in this._itemList)
				{
					item.Amount = 1;
				}
			}
			this.cardScroll.UpdateStyle(InfinityScroll.ScrollDirection.FromTop, Math.Min(this._itemList.Count, 7), this.GetGap(), this.cardScroll.padding, this.itemPrefab);
			this.itemScroll.SetItemList(this._itemList);
			SingletonObject.getInstance<YieldHelper>().DelayFrameDo(5U, delegate
			{
				AdaptableLog.Info(string.Format("Set Showing Item Data Count:{0}", this._itemList.Count));
				this.cardScroll.SetDataCount(this._itemList.Count);
			});
		}

		// Token: 0x06006202 RID: 25090 RVA: 0x002CF6FC File Offset: 0x002CD8FC
		private void ShowCharacter()
		{
			CharacterDomainMethod.AsyncCall.GetCharacterDisplayDataList(this, this._charIdList, delegate(int offset, RawDataPool dataPool)
			{
				Serializer.Deserialize(dataPool, offset, ref this._characterDisplayData);
				this.cardScroll.UpdateStyle(InfinityScroll.ScrollDirection.FromTop, Math.Min(this._charIdList.Count, 4), this.GetGap(), this.cardScroll.padding, this.characterPrefab);
				SingletonObject.getInstance<YieldHelper>().DelayFrameDo(5U, delegate
				{
					this.cardScroll.SetDataCount(this._characterDisplayData.Count);
				});
			});
		}

		// Token: 0x06006203 RID: 25091 RVA: 0x002CF718 File Offset: 0x002CD918
		private void ShowChicken()
		{
			this.cardScroll.UpdateStyle(InfinityScroll.ScrollDirection.FromTop, Math.Min(this._chickenList.Count, 2), this.GetGap(), this.cardScroll.padding, this.chickenPrefab);
			SingletonObject.getInstance<YieldHelper>().DelayFrameDo(5U, delegate
			{
				this.cardScroll.SetDataCount(this._chickenList.Count);
			});
		}

		// Token: 0x06006204 RID: 25092 RVA: 0x002CF774 File Offset: 0x002CD974
		private void ShowDebateStrategy()
		{
			this.cardScroll.UpdateStyle(InfinityScroll.ScrollDirection.FromTop, Math.Min(this._unlockedDebateStrategyList.Count, 4), this.GetGap(), this.cardScroll.padding, this.debateStrategyPrefab);
			SingletonObject.getInstance<YieldHelper>().DelayFrameDo(5U, delegate
			{
				this.cardScroll.SetDataCount(this._unlockedDebateStrategyList.Count);
			});
		}

		// Token: 0x06006205 RID: 25093 RVA: 0x002CF7D0 File Offset: 0x002CD9D0
		private void ShowLegacy()
		{
			this.cardScroll.UpdateStyle(InfinityScroll.ScrollDirection.FromTop, Math.Min(this._legacyList.Count, 3), this.GetGap(), this.cardScroll.padding, this.legacyPrefab);
			SingletonObject.getInstance<YieldHelper>().DelayFrameDo(5U, delegate
			{
				this.cardScroll.SetDataCount(this._legacyList.Count);
			});
		}

		// Token: 0x06006206 RID: 25094 RVA: 0x002CF82C File Offset: 0x002CDA2C
		private void ShowFeature()
		{
			this.cardScroll.UpdateStyle(InfinityScroll.ScrollDirection.FromTop, Math.Min(this._featureList.Count, 5), this.GetGap(), this.cardScroll.padding, this.featurePrefab);
			SingletonObject.getInstance<YieldHelper>().DelayFrameDo(5U, delegate
			{
				this.cardScroll.SetDataCount(this._featureList.Count);
			});
		}

		// Token: 0x06006207 RID: 25095 RVA: 0x002CF887 File Offset: 0x002CDA87
		private void ShowShave()
		{
			this.shave.Set(this._shaveName, this._shaveAvatar[0], this._shaveAvatar[1], this._shaveAge);
		}

		// Token: 0x06006208 RID: 25096 RVA: 0x002CF8BA File Offset: 0x002CDABA
		private void ShowCombatSkill()
		{
			CombatSkillDomainMethod.AsyncCall.GetCombatSkillDisplayData(this, SingletonObject.getInstance<BasicGameData>().TaiwuCharId, new List<short>
			{
				this._combatSkillId
			}, delegate(int offset, RawDataPool dataPool)
			{
				List<CombatSkillDisplayData> data = new List<CombatSkillDisplayData>();
				Serializer.Deserialize(dataPool, offset, ref data);
				switch (this._type)
				{
				case EObtainType.PracticeCombatSKill:
					this.combatSkill.Set(data[0], this._combatSkillProficiency);
					break;
				case EObtainType.BreakCombatSkill:
				{
					bool combatSkillBreakSuccess = this._combatSkillBreakSuccess;
					if (combatSkillBreakSuccess)
					{
						this.combatSkill.Set(data[0], this._combatSkillMaxPower);
					}
					else
					{
						this.combatSkill.Set(data[0], false);
					}
					break;
				}
				case EObtainType.GetCombatSkill:
					this.combatSkill.Set(data[0]);
					break;
				}
			});
		}

		// Token: 0x06006209 RID: 25097 RVA: 0x002CF8EC File Offset: 0x002CDAEC
		private void CloseAll()
		{
			this.cardScroll.gameObject.SetActive(false);
			this.itemScroll.gameObject.SetActive(false);
			this.shave.gameObject.SetActive(false);
			this.combatSkill.gameObject.SetActive(false);
			this.hintBlue.SetActive(false);
			this.hintRed.SetActive(false);
		}

		// Token: 0x0600620A RID: 25098 RVA: 0x002CF95C File Offset: 0x002CDB5C
		private void Open()
		{
			base.GetComponent<CanvasGroup>().alpha = 1f;
			this.switchToggleGroup.gameObject.SetActive(this._canSwitchTypes.Contains(this._type));
			switch (this._type)
			{
			case EObtainType.Item:
			case EObtainType.GetLifeSkill:
			case EObtainType.GetCombatSkill:
			case EObtainType.Craft:
			case EObtainType.Repair:
			case EObtainType.Disassemble:
			case EObtainType.Refine:
			case EObtainType.AttachPoison:
			case EObtainType.DetoxPoison:
			case EObtainType.BuildingResourceReturn:
			{
				this.ShowCardScroll();
				bool warehouse = this._warehouse;
				if (warehouse)
				{
					this.hintBlueText.SetText(LanguageKey.LK_Get_Item_Into_Warehouse.Tr().ColorReplace(), true);
					this.hintBlue.SetActive(true);
				}
				break;
			}
			case EObtainType.Legacy:
				this.ShowCardScroll();
				break;
			case EObtainType.PracticeCombatSKill:
				this.combatSkill.gameObject.SetActive(true);
				break;
			case EObtainType.BreakCombatSkill:
			{
				this.combatSkill.gameObject.SetActive(true);
				bool combatSkillBreakSuccess = this._combatSkillBreakSuccess;
				if (combatSkillBreakSuccess)
				{
					this.hintBlueText.SetText(LanguageKey.LK_Skill_Break_Success.Tr(), true);
					this.hintBlue.SetActive(true);
				}
				else
				{
					this.hintRedText.SetText(LanguageKey.LK_Skill_Break_Failed.Tr(), true);
					this.hintRed.SetActive(true);
				}
				break;
			}
			case EObtainType.IdentifyPoison:
				this.ShowCardScroll();
				this.hintBlueText.SetText(LanguageKey.LK_Poison_Identify_HasPoison.Tr().ColorReplace(), true);
				this.hintBlue.SetActive(true);
				break;
			case EObtainType.Feature:
				this.ShowCardScroll();
				break;
			case EObtainType.Teammate:
			case EObtainType.Villager:
			case EObtainType.Kidnap:
			case EObtainType.Character:
				this.ShowCardScroll();
				break;
			case EObtainType.Chicken:
				this.ShowCardScroll();
				this.hintBlueText.SetText(LanguageKey.LK_GetItem_ChickenTips.Tr(), true);
				this.hintBlue.SetActive(true);
				break;
			case EObtainType.Shave:
			{
				this.shave.gameObject.SetActive(true);
				bool shaveResult = this._shaveResult;
				if (shaveResult)
				{
					this.hintBlueText.SetText(LanguageKey.LK_GetItem_ShaveSuccess.Tr(), true);
					this.hintBlue.SetActive(true);
				}
				else
				{
					this.hintRedText.SetText(LanguageKey.LK_GetItem_ShaveFail.Tr(), true);
					this.hintRed.SetActive(true);
				}
				break;
			}
			case EObtainType.DebateStrategy:
				this.ShowCardScroll();
				break;
			}
		}

		// Token: 0x0600620B RID: 25099 RVA: 0x002CFBCC File Offset: 0x002CDDCC
		private void ShowCardScroll()
		{
			this.cardScroll.GetComponent<RectTransform>().anchoredPosition = ((this.GetCount() > this.cardScroll.lineCount) ? new Vector2(10f, -45f) : new Vector2(10f, -110f));
			this.cardScroll.gameObject.SetActive(!this._canSwitchTypes.Contains(this._type) || !this._isListStyle);
		}

		// Token: 0x0600620C RID: 25100 RVA: 0x002CFC50 File Offset: 0x002CDE50
		private void ShowItemScroll()
		{
			float contentHeight = this.itemScrollContent.rect.height + this.itemScrollHeadContent.rect.height;
			RectTransform rectTransform = this.itemScroll.GetComponent<RectTransform>();
			float height = rectTransform.rect.height;
			rectTransform.anchoredPosition = ((contentHeight > height) ? Vector2.zero : new Vector2(0f, (contentHeight - height) / 2f));
			this.itemScroll.gameObject.SetActive(this._isListStyle);
		}

		// Token: 0x0600620D RID: 25101 RVA: 0x002CFCE0 File Offset: 0x002CDEE0
		private int GetCount()
		{
			EObtainType type = this._type;
			if (!true)
			{
			}
			int result;
			switch (type)
			{
			case EObtainType.Item:
			case EObtainType.GetLifeSkill:
			case EObtainType.GetCombatSkill:
			case EObtainType.Craft:
			case EObtainType.Repair:
			case EObtainType.Disassemble:
			case EObtainType.Refine:
			case EObtainType.IdentifyPoison:
			case EObtainType.AttachPoison:
			case EObtainType.DetoxPoison:
			case EObtainType.BuildingResourceReturn:
				result = this._itemList.Count;
				goto IL_C4;
			case EObtainType.Legacy:
				result = this._legacyList.Count;
				goto IL_C4;
			case EObtainType.Feature:
				result = this._featureList.Count;
				goto IL_C4;
			case EObtainType.Teammate:
			case EObtainType.Villager:
			case EObtainType.Kidnap:
			case EObtainType.Character:
				result = this._charIdList.Count;
				goto IL_C4;
			case EObtainType.Chicken:
				result = this._chickenList.Count;
				goto IL_C4;
			case EObtainType.DebateStrategy:
				result = this._unlockedDebateStrategyList.Count;
				goto IL_C4;
			}
			result = -1;
			IL_C4:
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x0600620E RID: 25102 RVA: 0x002CFDBC File Offset: 0x002CDFBC
		private Vector2 GetGap()
		{
			EObtainType type = this._type;
			if (!true)
			{
			}
			Vector2 result;
			if (type != EObtainType.Item)
			{
				result = this._defaultGap;
			}
			else
			{
				result = this._itemGap;
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x0600620F RID: 25103 RVA: 0x002CFDF8 File Offset: 0x002CDFF8
		private void StartAnimation()
		{
			AdaptableLog.Info("Start Playing Animation");
			Sequence inSequence = this._inSequence;
			if (inSequence != null)
			{
				inSequence.Kill(false);
			}
			this.panel.sizeDelta = new Vector2(this.panel.sizeDelta.x, 0f);
			this.title.GetComponent<RectTransform>().localScale = new Vector3(2f, 2f, 2f);
			this._inSequence = DOTween.Sequence();
			this._inSequence.AppendInterval(0.01f);
			this._inSequence.Join(this.panel.DOSizeDelta(new Vector2(this.panel.sizeDelta.x, 960f), 0.2f, false));
			this._inSequence.Join(this.title.GetComponent<RectTransform>().DOScale(Vector3.one, 0.2f).SetEase(Ease.InQuint));
			this._inSequence.AppendCallback(delegate
			{
				bool flag;
				if (this._itemList != null)
				{
					flag = this._itemList.Exists((ItemDisplayData d) => ItemTemplateHelper.GetGrade(d.Key.ItemType, d.Key.TemplateId) >= 6);
				}
				else
				{
					flag = false;
				}
				string soundName = flag ? "ui_collect_legend" : "ui_collect_normal";
				AudioManager.Instance.PlaySound(soundName, false, false);
				bool flag2 = base.gameObject.activeInHierarchy && this.GetCount() > 0 && !this._quickShow && (!this._canSwitchTypes.Contains(this._type) || !this._isListStyle);
				if (flag2)
				{
					this._acquisitionRoutine = base.StartCoroutine(this.ShowGetItemCoroutine());
				}
				else
				{
					this._showedIndex = int.MaxValue;
				}
			});
			this._inSequence.Play<Sequence>();
		}

		// Token: 0x06006210 RID: 25104 RVA: 0x002CFF13 File Offset: 0x002CE113
		private IEnumerator ShowGetItemCoroutine()
		{
			AdaptableLog.Info("Start Playing ItemIn Animation");
			WaitForSeconds waitForSeconds = new WaitForSeconds(0.2f);
			CScrollRect scrollRect = this.cardScroll.GetComponent<CScrollRect>();
			scrollRect.SetScrollEnable(false);
			scrollRect.ScrollTo(Vector2.zero, 0.3f);
			this._showedIndex = -1;
			yield return new WaitForEndOfFrame();
			int count = this.GetCount();
			AdaptableLog.Info(string.Format("Start Playing ItemIn Animation Alpha: {0}", count));
			int i = 0;
			int max = this.GetCount();
			while (i < max)
			{
				GameObject obj = this.cardScroll.GetActiveCell(i);
				bool flag = obj != null;
				if (flag)
				{
					ViewObtain.<>c__DisplayClass95_0 CS$<>8__locals1 = new ViewObtain.<>c__DisplayClass95_0();
					CS$<>8__locals1.canvas = obj.GetComponent<CanvasGroup>();
					RectTransform rectTransform = obj.GetComponent<RectTransform>();
					CS$<>8__locals1.graphic = obj.GetComponent<CEmptyGraphic>();
					rectTransform.localScale = Vector3.one * 2f;
					rectTransform.DOScale(Vector3.one, 0.2f).SetEase(Ease.InExpo);
					CS$<>8__locals1.canvas.alpha = 0f;
					AdaptableLog.Info(string.Format("Item Animation Start: {0}, total {1}, alpha {2}", i, max, CS$<>8__locals1.canvas.alpha));
					CS$<>8__locals1.index = i;
					CS$<>8__locals1.canvas.DOFade(1f, 0.2f).SetEase(Ease.InExpo).OnComplete(delegate
					{
						AdaptableLog.Info(string.Format("Item Animation Done: {0}, alpha {1}", CS$<>8__locals1.index, CS$<>8__locals1.canvas.alpha));
						CS$<>8__locals1.graphic.enabled = true;
					});
					CS$<>8__locals1 = null;
					rectTransform = null;
				}
				this._showedIndex = i;
				yield return waitForSeconds;
				this.cardScroll.ScrollTo(i + 1, 0.3f);
				obj = null;
				int num = i + 1;
				i = num;
			}
			this.ScrollToEnd();
			scrollRect.SetScrollEnable(true);
			this._acquisitionRoutine = null;
			this._showedIndex = this.GetCount();
			yield break;
		}

		// Token: 0x06006211 RID: 25105 RVA: 0x002CFF24 File Offset: 0x002CE124
		private void QuickCompleteAnimIn()
		{
			bool quickShow = this._quickShow;
			if (!quickShow)
			{
				AdaptableLog.Info("Skip Animation");
				this._canInteract = false;
				this._quickShow = true;
				this.panel.sizeDelta = new Vector2(this.panel.sizeDelta.x, 960f);
				this.title.GetComponent<RectTransform>().localScale = Vector3.one;
				bool flag = this._acquisitionRoutine != null;
				if (flag)
				{
					base.StopCoroutine(this._acquisitionRoutine);
					this._acquisitionRoutine = null;
				}
				int count = this.GetCount();
				AdaptableLog.Info(string.Format("Skip Animation Alpha Count: {0}", count));
				bool flag2 = count > 0;
				if (flag2)
				{
					for (int i = 0; i < count; i++)
					{
						GameObject obj = this.cardScroll.GetActiveCell(i);
						AdaptableLog.Info(string.Format("index: {0}", i));
						bool flag3 = obj != null;
						if (flag3)
						{
							CanvasGroup canvas = obj.GetComponent<CanvasGroup>();
							RectTransform rectTransform = obj.GetComponent<RectTransform>();
							canvas.DOComplete(true);
							canvas.alpha = 1f;
							AdaptableLog.Info(string.Format("Obj {0} alpha:{1}", i, canvas.alpha));
							rectTransform.DOComplete(true);
							rectTransform.localScale = Vector3.one;
						}
					}
					this.ScrollToEnd();
					this.cardScroll.GetComponent<CScrollRect>().SetScrollEnable(true);
				}
				SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(0.5f, delegate
				{
					this._showedIndex = int.MaxValue;
					this._canInteract = true;
				});
			}
		}

		// Token: 0x06006212 RID: 25106 RVA: 0x002D00D0 File Offset: 0x002CE2D0
		private void ScrollToEnd()
		{
			CScrollRect scrollRect = this.cardScroll.GetComponent<CScrollRect>();
			RectTransform content = scrollRect.Content;
			float height = this.cardScroll.GetComponent<RectTransform>().rect.height;
			bool flag = content.rect.height > height;
			if (flag)
			{
				scrollRect.ScrollTo(content.anchoredPosition.SetY(content.rect.height - height), 0.3f);
			}
		}

		// Token: 0x0400440F RID: 17423
		[SerializeField]
		private CButton btnClose;

		// Token: 0x04004410 RID: 17424
		[SerializeField]
		private InfinityScroll cardScroll;

		// Token: 0x04004411 RID: 17425
		[SerializeField]
		private CImage title;

		// Token: 0x04004412 RID: 17426
		[SerializeField]
		private CRawImage titleBackGround;

		// Token: 0x04004413 RID: 17427
		[SerializeField]
		private CRawImage backGround;

		// Token: 0x04004414 RID: 17428
		[SerializeField]
		private RectTransform panel;

		// Token: 0x04004415 RID: 17429
		[SerializeField]
		private GameObject itemPrefab;

		// Token: 0x04004416 RID: 17430
		[SerializeField]
		private GameObject characterPrefab;

		// Token: 0x04004417 RID: 17431
		[SerializeField]
		private GameObject chickenPrefab;

		// Token: 0x04004418 RID: 17432
		[SerializeField]
		private GameObject debateStrategyPrefab;

		// Token: 0x04004419 RID: 17433
		[SerializeField]
		private GameObject legacyPrefab;

		// Token: 0x0400441A RID: 17434
		[SerializeField]
		private GameObject featurePrefab;

		// Token: 0x0400441B RID: 17435
		[SerializeField]
		private GetShave shave;

		// Token: 0x0400441C RID: 17436
		[SerializeField]
		private GetCombatSkill combatSkill;

		// Token: 0x0400441D RID: 17437
		[SerializeField]
		private GameObject hintBlue;

		// Token: 0x0400441E RID: 17438
		[SerializeField]
		private GameObject hintRed;

		// Token: 0x0400441F RID: 17439
		[SerializeField]
		private TextMeshProUGUI hintBlueText;

		// Token: 0x04004420 RID: 17440
		[SerializeField]
		private TextMeshProUGUI hintRedText;

		// Token: 0x04004421 RID: 17441
		[SerializeField]
		private CToggleGroup switchToggleGroup;

		// Token: 0x04004422 RID: 17442
		[SerializeField]
		private ItemListScroll itemScroll;

		// Token: 0x04004423 RID: 17443
		[SerializeField]
		private RectTransform itemScrollContent;

		// Token: 0x04004424 RID: 17444
		[SerializeField]
		private RectTransform itemScrollHeadContent;

		// Token: 0x04004425 RID: 17445
		private const float BackgroundAniTime = 0.2f;

		// Token: 0x04004426 RID: 17446
		private const float ItemShowAniTime = 0.2f;

		// Token: 0x04004427 RID: 17447
		private const float EnableDelayTime = 0.5f;

		// Token: 0x04004428 RID: 17448
		private const int BackgroundHeight = 960;

		// Token: 0x04004429 RID: 17449
		private Vector2 _defaultGap = new Vector2(32f, 64f);

		// Token: 0x0400442A RID: 17450
		private Vector2 _itemGap = new Vector2(32f, 48f);

		// Token: 0x0400442B RID: 17451
		private const int ItemLineCount = 7;

		// Token: 0x0400442C RID: 17452
		private const int CharacterLineCount = 4;

		// Token: 0x0400442D RID: 17453
		private const int ChickenLineCount = 2;

		// Token: 0x0400442E RID: 17454
		private const int DebateStrategyLineCount = 4;

		// Token: 0x0400442F RID: 17455
		private const int LegacyLineCount = 3;

		// Token: 0x04004430 RID: 17456
		private const int FeatureLineCount = 5;

		// Token: 0x04004431 RID: 17457
		private readonly Dictionary<EObtainType, string> _titles = new Dictionary<EObtainType, string>
		{
			{
				EObtainType.Item,
				"ui9_text_obtain_title_item_{0}"
			},
			{
				EObtainType.Legacy,
				"ui9_text_obtain_title_legacy_{0}"
			},
			{
				EObtainType.GetLifeSkill,
				"ui9_text_obtain_title_life_skill_get_{0}"
			},
			{
				EObtainType.PracticeCombatSKill,
				"ui9_text_obtain_title_combat_skill_proficiency_{0}"
			},
			{
				EObtainType.BreakCombatSkill,
				"ui9_text_obtain_title_combat_skill_break_{0}"
			},
			{
				EObtainType.GetCombatSkill,
				"ui9_text_obtain_title_combat_skill_get_{0}"
			},
			{
				EObtainType.BuildingResourceReturn,
				"ui9_text_obtain_title_building_resource_return_{0}"
			},
			{
				EObtainType.Craft,
				"ui9_text_obtain_title_craft_{0}"
			},
			{
				EObtainType.Repair,
				"ui9_text_obtain_title_repair_{0}"
			},
			{
				EObtainType.Disassemble,
				"ui9_text_obtain_title_disassemble_{0}"
			},
			{
				EObtainType.Refine,
				"ui9_text_obtain_title_refine_{0}"
			},
			{
				EObtainType.IdentifyPoison,
				"ui9_text_obtain_title_poison_detected_{0}"
			},
			{
				EObtainType.AttachPoison,
				"ui9_text_obtain_title_poison_attached_{0}"
			},
			{
				EObtainType.DetoxPoison,
				"ui9_text_obtain_title_poison_detoxed_{0}"
			},
			{
				EObtainType.Feature,
				"ui9_text_obtain_title_feature_{0}"
			},
			{
				EObtainType.Teammate,
				"ui9_text_obtain_title_teammate_{0}"
			},
			{
				EObtainType.Villager,
				"ui9_text_obtain_title_villager_{0}"
			},
			{
				EObtainType.Kidnap,
				"ui9_text_obtain_title_kidnap_{0}"
			},
			{
				EObtainType.Character,
				"ui9_text_obtain_title_character_{0}"
			},
			{
				EObtainType.Chicken,
				"ui9_text_obtain_title_chicken_{0}"
			},
			{
				EObtainType.Shave,
				"ui9_text_obtain_title_shave_{0}"
			},
			{
				EObtainType.DebateStrategy,
				"ui9_text_obtain_title_debate_strategy_{0}"
			}
		};

		// Token: 0x04004432 RID: 17458
		private readonly Dictionary<EObtainType, string> _titleColors = new Dictionary<EObtainType, string>
		{
			{
				EObtainType.PracticeCombatSKill,
				"ui9_tex_obtain_orange_{0}"
			},
			{
				EObtainType.Feature,
				"ui9_tex_obtain_blue_{0}"
			},
			{
				EObtainType.Item,
				"ui9_tex_obtain_orange_{0}"
			},
			{
				EObtainType.Legacy,
				"ui9_tex_obtain_orange_{0}"
			},
			{
				EObtainType.BuildingResourceReturn,
				"ui9_tex_obtain_orange_{0}"
			},
			{
				EObtainType.IdentifyPoison,
				"ui9_tex_obtain_purple_{0}"
			},
			{
				EObtainType.AttachPoison,
				"ui9_tex_obtain_purple_{0}"
			},
			{
				EObtainType.DetoxPoison,
				"ui9_tex_obtain_purple_{0}"
			},
			{
				EObtainType.Shave,
				"ui9_tex_obtain_blue_{0}"
			},
			{
				EObtainType.Teammate,
				"ui9_tex_obtain_blue_{0}"
			},
			{
				EObtainType.Villager,
				"ui9_tex_obtain_blue_{0}"
			},
			{
				EObtainType.Kidnap,
				"ui9_tex_obtain_blue_{0}"
			},
			{
				EObtainType.Character,
				"ui9_tex_obtain_blue_{0}"
			},
			{
				EObtainType.Chicken,
				"ui9_tex_obtain_blue_{0}"
			},
			{
				EObtainType.DebateStrategy,
				"ui9_tex_obtain_green_{0}"
			},
			{
				EObtainType.GetLifeSkill,
				"ui9_tex_obtain_green_{0}"
			},
			{
				EObtainType.BreakCombatSkill,
				"ui9_tex_obtain_darkgold_{0}"
			},
			{
				EObtainType.GetCombatSkill,
				"ui9_tex_obtain_darkgold_{0}"
			},
			{
				EObtainType.Craft,
				"ui9_tex_obtain_red_{0}"
			},
			{
				EObtainType.Repair,
				"ui9_tex_obtain_red_{0}"
			},
			{
				EObtainType.Disassemble,
				"ui9_tex_obtain_red_{0}"
			},
			{
				EObtainType.Refine,
				"ui9_tex_obtain_red_{0}"
			}
		};

		// Token: 0x04004433 RID: 17459
		private readonly HashSet<EObtainType> _canSwitchTypes = new HashSet<EObtainType>
		{
			EObtainType.Item,
			EObtainType.Craft,
			EObtainType.Repair,
			EObtainType.Disassemble,
			EObtainType.Refine,
			EObtainType.IdentifyPoison,
			EObtainType.AttachPoison,
			EObtainType.DetoxPoison,
			EObtainType.BuildingResourceReturn
		};

		// Token: 0x04004434 RID: 17460
		private readonly HashSet<EObtainType> _itemTypes = new HashSet<EObtainType>
		{
			EObtainType.Item,
			EObtainType.Craft,
			EObtainType.Repair,
			EObtainType.Disassemble,
			EObtainType.Refine,
			EObtainType.IdentifyPoison,
			EObtainType.AttachPoison,
			EObtainType.DetoxPoison,
			EObtainType.BuildingResourceReturn
		};

		// Token: 0x04004435 RID: 17461
		private Action _closeAction;

		// Token: 0x04004436 RID: 17462
		private List<ItemDisplayData> _itemList = new List<ItemDisplayData>();

		// Token: 0x04004437 RID: 17463
		private List<int> _charIdList;

		// Token: 0x04004438 RID: 17464
		private List<Chicken> _chickenList;

		// Token: 0x04004439 RID: 17465
		private bool _warehouse;

		// Token: 0x0400443A RID: 17466
		private CombatSkillDisplayData _combatSkill;

		// Token: 0x0400443B RID: 17467
		private short _combatSkillId;

		// Token: 0x0400443C RID: 17468
		private bool _combatSkillBreakSuccess;

		// Token: 0x0400443D RID: 17469
		private int _combatSkillMaxPower;

		// Token: 0x0400443E RID: 17470
		private IntPair _combatSkillProficiency;

		// Token: 0x0400443F RID: 17471
		private List<AvatarData> _shaveAvatar;

		// Token: 0x04004440 RID: 17472
		private string _shaveName;

		// Token: 0x04004441 RID: 17473
		private bool _shaveResult;

		// Token: 0x04004442 RID: 17474
		private short _shaveAge;

		// Token: 0x04004443 RID: 17475
		private bool _canInteract;

		// Token: 0x04004444 RID: 17476
		private bool _callTriggerListener;

		// Token: 0x04004445 RID: 17477
		private List<short> _unlockedDebateStrategyList;

		// Token: 0x04004446 RID: 17478
		private List<short> _featureList;

		// Token: 0x04004447 RID: 17479
		private List<short> _legacyList;

		// Token: 0x04004448 RID: 17480
		private List<CharacterDisplayData> _characterDisplayData;

		// Token: 0x04004449 RID: 17481
		private EObtainType _type;

		// Token: 0x0400444A RID: 17482
		private int _showedIndex;

		// Token: 0x0400444B RID: 17483
		private Coroutine _acquisitionRoutine;

		// Token: 0x0400444C RID: 17484
		private bool _isListStyle;

		// Token: 0x0400444D RID: 17485
		private bool _isDragging;

		// Token: 0x0400444E RID: 17486
		private bool _quickShow;

		// Token: 0x0400444F RID: 17487
		private Sequence _inSequence;

		// Token: 0x04004450 RID: 17488
		private bool _isFree;
	}
}
