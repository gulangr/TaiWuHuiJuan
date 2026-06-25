using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using GameData.Domains.Character;
using GameData.Domains.Character.Creation;
using GameData.Domains.Character.Display;
using GameData.Domains.Character.Relation;
using GameData.Domains.Map;
using GameData.Domains.TaiwuEvent;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

// Token: 0x020003C7 RID: 967
public abstract class MapBlockCharBase2 : MonoBehaviour
{
	// Token: 0x170005F1 RID: 1521
	// (get) Token: 0x06003A59 RID: 14937 RVA: 0x001DA91A File Offset: 0x001D8B1A
	public CButtonObsolete Button
	{
		get
		{
			return base.GetComponent<CButtonObsolete>();
		}
	}

	// Token: 0x170005F2 RID: 1522
	// (get) Token: 0x06003A5A RID: 14938 RVA: 0x001DA922 File Offset: 0x001D8B22
	protected WorldMapModel WorldMapModel
	{
		get
		{
			return SingletonObject.getInstance<WorldMapModel>();
		}
	}

	// Token: 0x170005F3 RID: 1523
	// (get) Token: 0x06003A5B RID: 14939 RVA: 0x001DA929 File Offset: 0x001D8B29
	protected int CurrentBlockId
	{
		get
		{
			return (int)this.WorldMapModel.CurrentBlockId;
		}
	}

	// Token: 0x170005F4 RID: 1524
	// (get) Token: 0x06003A5C RID: 14940 RVA: 0x001DA936 File Offset: 0x001D8B36
	protected bool IsMoving
	{
		get
		{
			return this.WorldMapModel.TaiwuMoveState > WorldMapModel.MoveState.Idle;
		}
	}

	// Token: 0x06003A5D RID: 14941
	protected abstract void RefreshName();

	// Token: 0x06003A5E RID: 14942
	protected abstract void RefreshOrganization();

	// Token: 0x06003A5F RID: 14943 RVA: 0x001DA946 File Offset: 0x001D8B46
	protected virtual void Update()
	{
		this.CheckHoverStates();
	}

	// Token: 0x06003A60 RID: 14944 RVA: 0x001DA950 File Offset: 0x001D8B50
	protected static bool IsMouseInRect(RectTransform rect)
	{
		Vector2 localPos;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, Input.mousePosition, UIManager.Instance.UiCamera, out localPos);
		return rect.rect.Contains(localPos);
	}

	// Token: 0x06003A61 RID: 14945 RVA: 0x001DA98E File Offset: 0x001D8B8E
	public void Init(bool canInteract, MapBlockData mapBlock, CharacterDisplayData displayData)
	{
		this._canInteract = canInteract;
		this.MapBlock = mapBlock;
		this.CharId = -1;
		this.DisplayData = displayData;
	}

	// Token: 0x06003A62 RID: 14946 RVA: 0x001DA9AD File Offset: 0x001D8BAD
	protected virtual void Refresh()
	{
		this.RefreshName();
		this.RefreshOrganization();
		this.RefreshInteraction();
		this.RefreshCustomInfo();
		this.RefreshCustomButton();
	}

	// Token: 0x06003A63 RID: 14947 RVA: 0x001DA9D4 File Offset: 0x001D8BD4
	protected virtual void OnClickButton()
	{
		bool flag = this.WorldMapModel.ShowingAreaId != this.WorldMapModel.CurrentAreaId;
		if (flag)
		{
			GEvent.OnEvent(UiEvents.WorldMapResetMapCamera, EasyPool.Get<ArgumentBox>().Set("isAnim", false));
		}
	}

	// Token: 0x06003A64 RID: 14948 RVA: 0x001DAA1E File Offset: 0x001D8C1E
	protected static string GetIconGrave(int level)
	{
		return string.Format("blockchar_icon_fenmu_{0}", level);
	}

	// Token: 0x06003A65 RID: 14949 RVA: 0x001DAA30 File Offset: 0x001D8C30
	public void DisableButtonClick()
	{
		this._skipButtonClickRegistration = true;
	}

	// Token: 0x06003A66 RID: 14950 RVA: 0x001DAA3C File Offset: 0x001D8C3C
	private void RefreshInteraction()
	{
		SelectableCursorTriggerObsolete selectableCursorTrigger = this.Button.GetComponent<SelectableCursorTriggerObsolete>();
		this.Button.interactable = (this._canInteract && (this.MapBlock == null || this.CurrentBlockId == (int)this.MapBlock.BlockId));
		this.Button.onClick.RemoveAllListeners();
		if (selectableCursorTrigger != null)
		{
			selectableCursorTrigger.AddCursorEvent();
		}
		bool flag = !this._canInteract || this._skipButtonClickRegistration;
		if (!flag)
		{
			this.Button.ClearAndAddListener(new Action(this.OnClickButton));
		}
	}

	// Token: 0x06003A67 RID: 14951 RVA: 0x001DAAD8 File Offset: 0x001D8CD8
	protected virtual void RefreshCustomInfo()
	{
		bool flag = this.slotRoot == null;
		if (!flag)
		{
			bool flag2 = this.DisplayData == null;
			if (!flag2)
			{
				List<short> tids = this.WorldMapModel.CustomMapBlockCharInfoList;
				for (int i = 0; i < 6; i++)
				{
					Transform child = this.slotRoot.GetChild(i);
					bool flag3 = i < tids.Count;
					if (flag3)
					{
						child.gameObject.SetActive(true);
						short tid = tids[i];
						MapBlockCharCustomInfoItem info = MapBlockCharCustomInfo.Instance[tid];
						Refers refers = child.GetComponent<Refers>();
						CImage infoIcon = refers.CGet<CImage>("InfoIcon");
						CImage selected = refers.CGet<CImage>("Selected");
						TextMeshProUGUI infoLabel = refers.CGet<TextMeshProUGUI>("InfoLabel");
						TooltipInvoker tip = child.GetComponent<TooltipInvoker>();
						tip.enabled = false;
						TooltipInvoker tooltipInvoker = tip;
						if (tooltipInvoker.RuntimeParam == null)
						{
							tooltipInvoker.RuntimeParam = new ArgumentBox();
						}
						EMapBlockCharCustomInfoDisplayType type = info.DisplayType;
						infoIcon.gameObject.SetActive(type == EMapBlockCharCustomInfoDisplayType.Icon);
						infoLabel.gameObject.SetActive(type == EMapBlockCharCustomInfoDisplayType.Text);
						EMapBlockCharCustomInfoDisplayType emapBlockCharCustomInfoDisplayType = type;
						EMapBlockCharCustomInfoDisplayType emapBlockCharCustomInfoDisplayType2 = emapBlockCharCustomInfoDisplayType;
						if (emapBlockCharCustomInfoDisplayType2 != EMapBlockCharCustomInfoDisplayType.Icon)
						{
							if (emapBlockCharCustomInfoDisplayType2 == EMapBlockCharCustomInfoDisplayType.Text)
							{
								this.RefreshAsText(infoLabel, tip, info, tid);
							}
						}
						else
						{
							this.RefreshAsIcon(infoIcon, tip, info, tid);
						}
					}
					else
					{
						child.gameObject.SetActive(false);
					}
				}
			}
		}
	}

	// Token: 0x06003A68 RID: 14952 RVA: 0x001DAC54 File Offset: 0x001D8E54
	private void RefreshAsText(TextMeshProUGUI infoLabel, TooltipInvoker tip, MapBlockCharCustomInfoItem info, short tid)
	{
		if (tid != 0)
		{
			if (tid == 10)
			{
				this.RefreshSpetialInfo(infoLabel, info, tip);
			}
		}
		else
		{
			this.RefreshAge(infoLabel, info, tip);
		}
	}

	// Token: 0x06003A69 RID: 14953 RVA: 0x001DAC8D File Offset: 0x001D8E8D
	protected virtual void RefreshSpetialInfo(TextMeshProUGUI infoLabel, MapBlockCharCustomInfoItem info, TooltipInvoker tip)
	{
		infoLabel.text = string.Empty;
	}

	// Token: 0x06003A6A RID: 14954 RVA: 0x001DAC9C File Offset: 0x001D8E9C
	protected virtual void RefreshAge(TextMeshProUGUI infoLabel, MapBlockCharCustomInfoItem info, TooltipInvoker tip)
	{
		tip.enabled = true;
		infoLabel.text = this.DisplayData.PhysiologicalAge.ToString();
		tip.RuntimeParam.Set("arg0", info.TipContent.GetFormat(this.DisplayData.PhysiologicalAge));
	}

	// Token: 0x06003A6B RID: 14955 RVA: 0x001DACF8 File Offset: 0x001D8EF8
	private void RefreshAsIcon(CImage infoIcon, TooltipInvoker tip, MapBlockCharCustomInfoItem info, short tid)
	{
		switch (tid)
		{
		case 1:
			this.RefreshTitle(infoIcon, info, tip);
			break;
		case 2:
			this.RefreshHealth(infoIcon, info, tip);
			break;
		case 3:
			this.RefreshFame(infoIcon, info, tip);
			break;
		case 4:
			this.RefreshGender(infoIcon, info, tip);
			break;
		case 5:
			this.RefreshOrganization(infoIcon, info, tip);
			break;
		case 6:
			this.RefreshCharm(infoIcon, info, tip);
			break;
		case 7:
			this.RefreshHappiness(infoIcon, info, tip);
			break;
		case 8:
			this.RefreshBehaviorType(infoIcon, info, tip);
			break;
		case 9:
			this.RefreshFavorability(infoIcon, info, tip);
			break;
		case 13:
			this.RefreshRelation(infoIcon, info, tip);
			break;
		}
	}

	// Token: 0x06003A6C RID: 14956 RVA: 0x001DADC4 File Offset: 0x001D8FC4
	protected virtual void RefreshTitle(CImage infoIcon, MapBlockCharCustomInfoItem info, TooltipInvoker tip)
	{
		tip.enabled = true;
		List<short> titleIds = this.DisplayData.TitleIds;
		string title = (titleIds != null && titleIds.Count > 0) ? CharacterTitle.Instance[this.DisplayData.TitleIds[0]].Name : LocalStringManager.Get(LanguageKey.LK_None);
		tip.RuntimeParam.Set("arg0", info.TipContent.GetFormat(title));
	}

	// Token: 0x06003A6D RID: 14957 RVA: 0x001DAE40 File Offset: 0x001D9040
	protected virtual void RefreshHealth(CImage infoIcon, MapBlockCharCustomInfoItem info, TooltipInvoker tip)
	{
		tip.enabled = true;
		CharacterDomainMethod.AsyncCall.GetLeftMaxHealth(null, this.CharId, delegate(int offset, RawDataPool pool)
		{
			short maxHealth = 0;
			Serializer.Deserialize(pool, offset, ref maxHealth);
			EHealthType healthType = CommonUtils.GetHealthType(this.DisplayData.Health, maxHealth, this.CharId);
			CImage infoIcon2 = infoIcon;
			string str = "ui_mousetip_healystate_{0}";
			int num = (int)healthType;
			infoIcon2.SetSprite(str.GetFormat(num.ToString()), false, null);
			string healthStr = CommonUtils.GetHealthString(healthType);
			tip.RuntimeParam.Set("arg0", info.TipContent.GetFormat(healthStr));
		});
	}

	// Token: 0x06003A6E RID: 14958 RVA: 0x001DAE98 File Offset: 0x001D9098
	protected virtual void RefreshFame(CImage infoIcon, MapBlockCharCustomInfoItem info, TooltipInvoker tip)
	{
		tip.enabled = true;
		sbyte fameType = FameType.GetFameType(this.DisplayData.FameType);
		infoIcon.SetSprite(CommonUtils.GetFameIconLegacy(fameType), false, null);
		tip.RuntimeParam.Set("arg0", info.TipContent.GetFormat(CommonUtils.GetFameString(fameType)));
	}

	// Token: 0x06003A6F RID: 14959 RVA: 0x001DAEF0 File Offset: 0x001D90F0
	protected virtual void RefreshGender(CImage infoIcon, MapBlockCharCustomInfoItem info, TooltipInvoker tip)
	{
		tip.enabled = true;
		CommonUtils.EDisplayGender displayGender = CommonUtils.GetDisplayGender(this.DisplayData.Gender, this.DisplayData.TemplateId);
		infoIcon.SetSprite(CommonUtils.GetGenderIcon(displayGender), false, null);
		tip.RuntimeParam.Set("arg0", info.TipContent.GetFormat(CommonUtils.GetGenderString(displayGender)));
	}

	// Token: 0x06003A70 RID: 14960 RVA: 0x001DAF54 File Offset: 0x001D9154
	protected virtual void RefreshOrganization(CImage infoIcon, MapBlockCharCustomInfoItem info, TooltipInvoker tip)
	{
		tip.enabled = true;
		sbyte orgGrade = this.DisplayData.OrgInfo.Grade;
		infoIcon.SetSprite(CommonUtils.GetIdentityIcon(orgGrade), false, null);
		string orgString = CommonUtils.GetOrganizationGradeString(this.DisplayData.OrgInfo, this.DisplayData.Gender, this.DisplayData.PhysiologicalAge, -1);
		tip.RuntimeParam.Set("arg0", info.TipContent.GetFormat(orgString));
	}

	// Token: 0x06003A71 RID: 14961 RVA: 0x001DAFD0 File Offset: 0x001D91D0
	protected virtual void RefreshCharm(CImage infoIcon, MapBlockCharCustomInfoItem info, TooltipInvoker tip)
	{
		tip.enabled = true;
		infoIcon.SetSprite(CommonUtils.GetCharmLevelIconLegacy(this.DisplayData.Charm, this.DisplayData.PhysiologicalAge, this.DisplayData.AvatarRelatedData.ClothingDisplayId, this.DisplayData.AvatarRelatedData.AvatarData.FaceVisible), false, null);
		string charmText = CommonUtils.GetCharmLevelText(this.DisplayData.Charm, this.DisplayData.Gender, this.DisplayData.PhysiologicalAge, this.DisplayData.AvatarRelatedData.ClothingDisplayId, CreatingType.IsFixedPresetType(this.DisplayData.CreatingType), this.DisplayData.AvatarRelatedData.AvatarData.FaceVisible);
		tip.RuntimeParam.Set("arg0", info.TipContent.GetFormat(charmText));
	}

	// Token: 0x06003A72 RID: 14962 RVA: 0x001DB0A8 File Offset: 0x001D92A8
	protected virtual void RefreshHappiness(CImage infoIcon, MapBlockCharCustomInfoItem info, TooltipInvoker tip)
	{
		tip.enabled = true;
		sbyte happinessType = HappinessType.GetHappinessType(this.DisplayData.Happiness);
		infoIcon.SetSprite(CommonUtils.GetHappinessIconLegacy(happinessType), false, null);
		tip.RuntimeParam.Set("arg0", info.TipContent.GetFormat(CommonUtils.GetHappinessString(happinessType)));
	}

	// Token: 0x06003A73 RID: 14963 RVA: 0x001DB100 File Offset: 0x001D9300
	protected virtual void RefreshBehaviorType(CImage infoIcon, MapBlockCharCustomInfoItem info, TooltipInvoker tip)
	{
		tip.enabled = true;
		BehaviorTypeItem behaviorConfig = Config.BehaviorType.Instance.GetItem((short)this.DisplayData.BehaviorType);
		infoIcon.SetSprite(behaviorConfig.Icon, false, null);
		tip.RuntimeParam.Set("arg0", info.TipContent.GetFormat(behaviorConfig.Name));
	}

	// Token: 0x06003A74 RID: 14964 RVA: 0x001DB160 File Offset: 0x001D9360
	protected virtual void RefreshFavorability(CImage infoIcon, MapBlockCharCustomInfoItem info, TooltipInvoker tip)
	{
		tip.enabled = true;
		infoIcon.SetSprite(CommonUtils.GetFavorIconLegacy(this.DisplayData.FavorabilityToTaiwu), false, null);
		tip.RuntimeParam.Set("arg0", info.TipContent.GetFormat(CommonUtils.GetFavorString(this.DisplayData.FavorabilityToTaiwu)));
	}

	// Token: 0x06003A75 RID: 14965 RVA: 0x001DB1BC File Offset: 0x001D93BC
	protected virtual void RefreshRelation(CImage infoIcon, MapBlockCharCustomInfoItem info, TooltipInvoker tip)
	{
		tip.enabled = true;
		ushort relationFromTaiwu = this.DisplayData.RelationFromTaiwu;
		ushort relationToTaiwu = this.DisplayData.RelationToTaiwu;
		ushort displayRelation = (relationFromTaiwu != ushort.MaxValue) ? relationFromTaiwu : relationToTaiwu;
		bool flag = displayRelation != ushort.MaxValue;
		if (flag)
		{
			bool hasLove = RelationType.HasRelation(displayRelation, 16384);
			bool hasHate = RelationType.HasRelation(displayRelation, 32768);
			bool flag2 = hasLove;
			if (flag2)
			{
				tip.RuntimeParam.Set("arg0", info.TipContent.GetFormat(LocalStringManager.Get(LanguageKey.LK_RelationShip_Adored)));
			}
			else
			{
				bool flag3 = hasHate;
				if (flag3)
				{
					tip.RuntimeParam.Set("arg0", info.TipContent.GetFormat(LocalStringManager.Get(LanguageKey.LK_RelationShip_Enemy)));
				}
				else
				{
					tip.RuntimeParam.Set("arg0", info.TipContent.GetFormat(LocalStringManager.Get(LanguageKey.LK_RelationShip_Friend)));
				}
			}
		}
		else
		{
			tip.RuntimeParam.Set("arg0", info.TipContent.GetFormat(LocalStringManager.Get(LanguageKey.LK_None)));
		}
	}

	// Token: 0x06003A76 RID: 14966 RVA: 0x001DB2DC File Offset: 0x001D94DC
	protected virtual void RefreshCustomButton()
	{
		bool flag = this.customButtonTopRoot == null;
		if (!flag)
		{
			this.ClearCustomButtons();
			for (int i = 0; i < this.customButtonTopRoot.childCount; i++)
			{
				Transform node = this.customButtonTopRoot.GetChild(i);
				node.gameObject.SetActive(false);
			}
			bool flag2 = !this._canInteract;
			if (!flag2)
			{
				List<short> buttonList = this.WorldMapModel.CustomMapBlockCharButtonList;
				bool flag3 = buttonList == null || buttonList.Count == 0;
				if (!flag3)
				{
					List<MapBlockCharCustomButtonItem> topLevelButtons = new List<MapBlockCharCustomButtonItem>();
					List<MapBlockCharCustomButtonItem> interactButtons = new List<MapBlockCharCustomButtonItem>();
					foreach (short templateId in buttonList)
					{
						MapBlockCharCustomButtonItem buttonConfig = MapBlockCharCustomButton.Instance[templateId];
						bool flag4 = buttonConfig.DisplayGroup == EMapBlockCharCustomButtonDisplayGroup.Interact;
						if (flag4)
						{
							interactButtons.Add(buttonConfig);
						}
						else
						{
							topLevelButtons.Add(buttonConfig);
						}
					}
					int topNodeCount = Mathf.Min(topLevelButtons.Count, 2);
					for (int j = 0; j < topNodeCount; j++)
					{
						Transform nodeTransform = this.customButtonTopRoot.GetChild(j);
						nodeTransform.gameObject.SetActive(true);
						Transform buttonTransform = nodeTransform.Find("Button");
						bool flag5 = buttonTransform != null;
						if (flag5)
						{
							this.SetupButton(buttonTransform.GetComponent<CButtonObsolete>(), topLevelButtons[j]);
						}
					}
					bool flag6 = interactButtons.Count > 0;
					if (flag6)
					{
						Transform interactNodeTransform = this.customButtonTopRoot.GetChild(2);
						interactNodeTransform.gameObject.SetActive(true);
						Transform mainButtonTransform = interactNodeTransform.Find("Button");
						Transform extraButtonsTransform = interactNodeTransform.Find("ExtraButtons");
						bool flag7 = mainButtonTransform != null;
						if (flag7)
						{
							this.SetupButton(mainButtonTransform.GetComponent<CButtonObsolete>(), interactButtons[0]);
						}
						bool flag8 = interactButtons.Count > 1 && extraButtonsTransform != null;
						if (flag8)
						{
							int maxExtraCount = Mathf.Min(interactButtons.Count - 1, 5);
							for (int k = 1; k <= maxExtraCount; k++)
							{
								CButtonObsolete extraButton = this.CreateExtraButton(interactButtons[k], extraButtonsTransform);
								bool flag9 = extraButton != null;
								if (flag9)
								{
									this._customButtonInstances.Add(extraButton);
								}
							}
							this.UpdateExtraButtonsVisibility(extraButtonsTransform);
							this.SetupHoverLogic(mainButtonTransform.gameObject, extraButtonsTransform.gameObject);
						}
					}
					this.RefreshButtonInteractable();
				}
			}
		}
	}

	// Token: 0x06003A77 RID: 14967 RVA: 0x001DB584 File Offset: 0x001D9784
	private void RefreshButtonInteractable()
	{
		CharacterDomainMethod.AsyncCall.GetCharacterDisplayDataForMapBlock(null, this.CharId, delegate(int offset, RawDataPool pool)
		{
			CharacterDisplayDataForMapBlock displayData = null;
			Serializer.Deserialize(pool, offset, ref displayData);
			Dictionary<short, bool> dict = displayData.VisibleCharacterInteractionEventOptionDict;
			foreach (CButtonObsolete button in this._customButtonInstances)
			{
				short templateId;
				bool flag = button != null && this._buttonTemplateIdMap.TryGetValue(button, out templateId);
				if (flag)
				{
					this.RefreshButtonInteractive(button, templateId, dict);
				}
			}
		});
	}

	// Token: 0x06003A78 RID: 14968 RVA: 0x001DB5A0 File Offset: 0x001D97A0
	private void RefreshButtonInteractive(CButtonObsolete button, short templateId, Dictionary<short, bool> dict)
	{
		bool flag = button == null;
		if (!flag)
		{
			bool isInteractable = false;
			MapBlockCharCustomButtonItem buttonConfig = MapBlockCharCustomButton.Instance[templateId];
			EMapBlockCharCustomButtonLogicType logicType = buttonConfig.LogicType;
			EMapBlockCharCustomButtonLogicType emapBlockCharCustomButtonLogicType = logicType;
			if (emapBlockCharCustomButtonLogicType != EMapBlockCharCustomButtonLogicType.Interact)
			{
				if (emapBlockCharCustomButtonLogicType == EMapBlockCharCustomButtonLogicType.Special)
				{
					bool flag2 = templateId == 8;
					if (flag2)
					{
						short topInteractionId = GameLifetimeDataManager.GetTopNInteractionEventOptions();
						bool interactable;
						bool flag3 = topInteractionId != -1 && dict != null && dict.TryGetValue(topInteractionId, out interactable);
						if (flag3)
						{
							isInteractable = interactable;
						}
					}
				}
			}
			else
			{
				List<short> optionIds = buttonConfig.InteractionEventOption;
				foreach (short optionId in optionIds)
				{
					bool interactable2;
					bool flag4 = dict != null && dict.TryGetValue(optionId, out interactable2) && interactable2;
					if (flag4)
					{
						isInteractable = true;
						break;
					}
				}
			}
			button.interactable = isInteractable;
		}
	}

	// Token: 0x06003A79 RID: 14969 RVA: 0x001DB690 File Offset: 0x001D9890
	private void ClearCustomButtons()
	{
		foreach (CButtonObsolete button in this._customButtonInstances)
		{
			bool flag = button != null;
			if (flag)
			{
				bool flag2 = button.transform.parent != null && button.transform.parent.name == "ExtraButtons";
				if (flag2)
				{
					Object.DestroyImmediate(button.gameObject);
				}
				else
				{
					button.onClick.RemoveAllListeners();
					button.gameObject.SetActive(false);
				}
			}
		}
		this._customButtonInstances.Clear();
		this._buttonTemplateIdMap.Clear();
		this.ClearHoverLogic();
	}

	// Token: 0x06003A7A RID: 14970 RVA: 0x001DB76C File Offset: 0x001D996C
	private void ClearHoverLogic()
	{
		this._hoverStates.Clear();
		bool flag = this.customButtonTopRoot == null;
		if (!flag)
		{
			for (int i = 0; i < this.customButtonTopRoot.childCount; i++)
			{
				Transform node = this.customButtonTopRoot.GetChild(i);
				Transform extraButtons = node.Find("ExtraButtons");
				bool flag2 = extraButtons != null;
				if (flag2)
				{
					this.UpdateExtraButtonsVisibility(extraButtons);
				}
			}
		}
	}

	// Token: 0x06003A7B RID: 14971 RVA: 0x001DB7E8 File Offset: 0x001D99E8
	private void SetupButton(CButtonObsolete button, MapBlockCharCustomButtonItem buttonConfig)
	{
		bool flag = button == null;
		if (!flag)
		{
			button.gameObject.SetActive(true);
			TextMeshProUGUI textComponent = button.GetComponentInChildren<TextMeshProUGUI>();
			bool flag2 = textComponent != null;
			if (flag2)
			{
				textComponent.text = buttonConfig.Name;
			}
			button.ClearAndAddListener(delegate
			{
				this.OnCustomButtonClick(buttonConfig.TemplateId);
			});
			this._customButtonInstances.Add(button);
			this._buttonTemplateIdMap[button] = buttonConfig.TemplateId;
		}
	}

	// Token: 0x06003A7C RID: 14972 RVA: 0x001DB880 File Offset: 0x001D9A80
	private CButtonObsolete CreateExtraButton(MapBlockCharCustomButtonItem buttonConfig, Transform parent)
	{
		CButtonObsolete buttonInstance = Object.Instantiate<CButtonObsolete>(this.customButtonTemplate, parent);
		buttonInstance.gameObject.SetActive(true);
		TextMeshProUGUI textComponent = buttonInstance.GetComponentInChildren<TextMeshProUGUI>();
		bool flag = textComponent != null;
		if (flag)
		{
			textComponent.text = buttonConfig.Name;
		}
		buttonInstance.ClearAndAddListener(delegate
		{
			this.OnCustomButtonClick(buttonConfig.TemplateId);
		});
		this._buttonTemplateIdMap[buttonInstance] = buttonConfig.TemplateId;
		return buttonInstance;
	}

	// Token: 0x06003A7D RID: 14973 RVA: 0x001DB914 File Offset: 0x001D9B14
	private void UpdateExtraButtonsVisibility(Transform extraButtonsTransform)
	{
		bool flag = extraButtonsTransform == null;
		if (!flag)
		{
			bool hasVisibleButton = false;
			for (int i = 0; i < extraButtonsTransform.childCount; i++)
			{
				Transform child = extraButtonsTransform.GetChild(i);
				bool activeInHierarchy = child.gameObject.activeInHierarchy;
				if (activeInHierarchy)
				{
					hasVisibleButton = true;
					break;
				}
			}
			extraButtonsTransform.gameObject.SetActive(hasVisibleButton);
		}
	}

	// Token: 0x06003A7E RID: 14974 RVA: 0x001DB978 File Offset: 0x001D9B78
	private void CheckHoverStates()
	{
		foreach (KeyValuePair<Transform, MapBlockCharBase2.HoverState> kvp in this._hoverStates)
		{
			Transform containerTransform = kvp.Key;
			MapBlockCharBase2.HoverState hoverState = kvp.Value;
			bool isMouseInRect = MapBlockCharBase2.IsMouseInRect((RectTransform)containerTransform);
			bool flag = isMouseInRect && !hoverState.isHovering;
			if (flag)
			{
				hoverState.isHovering = true;
				this.ShowExtraButtons(hoverState);
			}
			else
			{
				bool flag2 = !isMouseInRect && hoverState.isHovering;
				if (flag2)
				{
					hoverState.isHovering = false;
					this.HideExtraButtons(hoverState);
				}
			}
		}
	}

	// Token: 0x06003A7F RID: 14975 RVA: 0x001DBA34 File Offset: 0x001D9C34
	private void ShowExtraButtons(MapBlockCharBase2.HoverState hoverState)
	{
		bool flag = hoverState.extraButtons != null && hoverState.extraButtons.transform.childCount > 0;
		if (flag)
		{
			hoverState.extraButtons.SetActive(true);
		}
	}

	// Token: 0x06003A80 RID: 14976 RVA: 0x001DBA7C File Offset: 0x001D9C7C
	private void HideExtraButtons(MapBlockCharBase2.HoverState hoverState)
	{
		bool flag = hoverState.extraButtons != null;
		if (flag)
		{
			hoverState.extraButtons.SetActive(false);
		}
	}

	// Token: 0x06003A81 RID: 14977 RVA: 0x001DBAAC File Offset: 0x001D9CAC
	private void SetupHoverLogic(GameObject mainButton, GameObject extraButtons)
	{
		bool flag = mainButton == null || extraButtons == null;
		if (!flag)
		{
			Transform containerTransform = mainButton.transform.parent;
			this._hoverStates[containerTransform] = new MapBlockCharBase2.HoverState
			{
				extraButtons = extraButtons,
				isHovering = false
			};
		}
	}

	// Token: 0x06003A82 RID: 14978 RVA: 0x001DBB00 File Offset: 0x001D9D00
	private void OnCustomButtonClick(short templateId)
	{
		bool flag = this.CharId == -1 || !this._canInteract;
		if (!flag)
		{
			bool flag2 = templateId == 8;
			if (flag2)
			{
				short topInteractionId = GameLifetimeDataManager.GetTopNInteractionEventOptions();
				bool flag3 = topInteractionId == -1;
				if (!flag3)
				{
					TaiwuEventDomainMethod.AsyncCall.JumpToInteractionEventOptionByInteractionId(null, this.CharId, topInteractionId, delegate(int offset, RawDataPool pool)
					{
						bool success = false;
						Serializer.Deserialize(pool, offset, ref success);
					});
				}
			}
			else
			{
				TaiwuEventDomainMethod.AsyncCall.JumpToInteractionEventOption(null, this.CharId, templateId, delegate(int offset, RawDataPool pool)
				{
					bool success = false;
					Serializer.Deserialize(pool, offset, ref success);
				});
			}
		}
	}

	// Token: 0x06003A83 RID: 14979 RVA: 0x001DBBA1 File Offset: 0x001D9DA1
	public virtual void OnHide()
	{
	}

	// Token: 0x04002A19 RID: 10777
	private const int MaxInfomationSlot = 6;

	// Token: 0x04002A1A RID: 10778
	private const int CustomButtonTopSlotCount = 3;

	// Token: 0x04002A1B RID: 10779
	private const int MaxInteractionButtonCount = 6;

	// Token: 0x04002A1C RID: 10780
	private const EMapBlockCharCustomButtonDisplayGroup CustomButtonInteractType = EMapBlockCharCustomButtonDisplayGroup.Interact;

	// Token: 0x04002A1D RID: 10781
	private const int CustomButtonInteractGroupIndex = 2;

	// Token: 0x04002A1E RID: 10782
	protected static readonly string NpcAvatarTexturePath = "NpcFace/SmallFace";

	// Token: 0x04002A1F RID: 10783
	[SerializeField]
	protected TextMeshProUGUI nameLabel;

	// Token: 0x04002A20 RID: 10784
	[SerializeField]
	protected TextMeshProUGUI organizationLabel;

	// Token: 0x04002A21 RID: 10785
	[SerializeField]
	protected RectTransform slotRoot;

	// Token: 0x04002A22 RID: 10786
	[SerializeField]
	protected CButtonObsolete customButtonTemplate;

	// Token: 0x04002A23 RID: 10787
	[SerializeField]
	protected RectTransform customButtonTopRoot;

	// Token: 0x04002A24 RID: 10788
	private bool _canInteract;

	// Token: 0x04002A25 RID: 10789
	protected MapBlockData MapBlock;

	// Token: 0x04002A26 RID: 10790
	protected int CharId = -1;

	// Token: 0x04002A27 RID: 10791
	protected CharacterDisplayData DisplayData;

	// Token: 0x04002A28 RID: 10792
	private readonly List<CButtonObsolete> _customButtonInstances = new List<CButtonObsolete>();

	// Token: 0x04002A29 RID: 10793
	private readonly Dictionary<CButtonObsolete, short> _buttonTemplateIdMap = new Dictionary<CButtonObsolete, short>();

	// Token: 0x04002A2A RID: 10794
	private readonly Dictionary<Transform, MapBlockCharBase2.HoverState> _hoverStates = new Dictionary<Transform, MapBlockCharBase2.HoverState>();

	// Token: 0x04002A2B RID: 10795
	private bool _skipButtonClickRegistration = false;

	// Token: 0x02001866 RID: 6246
	private class HoverState
	{
		// Token: 0x0400AE79 RID: 44665
		public GameObject extraButtons;

		// Token: 0x0400AE7A RID: 44666
		public bool isHovering;
	}
}
