using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Config;
using DG.Tweening;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Character;
using Game.Components.Common;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Global;
using GameData.Domains.Taiwu;
using GameData.Domains.TaiwuEvent;
using GameData.Domains.World;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.Views.CharacterMenu
{
	// Token: 0x02000BAA RID: 2986
	public class ViewCharacterMenuNeili : UI_CharacterMenuSubPageBase
	{
		// Token: 0x1700101D RID: 4125
		// (get) Token: 0x060095B4 RID: 38324 RVA: 0x0045CC31 File Offset: 0x0045AE31
		public override LanguageKey TitleKey
		{
			get
			{
				return LanguageKey.LK_Neili;
			}
		}

		// Token: 0x1700101E RID: 4126
		// (get) Token: 0x060095B5 RID: 38325 RVA: 0x0045CC38 File Offset: 0x0045AE38
		public override bool ShowBaseAttribute
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060095B6 RID: 38326 RVA: 0x0045CC3B File Offset: 0x0045AE3B
		public override bool CheckState(ECharacterSubToggleBase curSubTogglePage, ECharacterSubPage curSubPage)
		{
			return curSubTogglePage == ECharacterSubToggleBase.NeiliBase;
		}

		// Token: 0x1700101F RID: 4127
		// (get) Token: 0x060095B7 RID: 38327 RVA: 0x0045CC41 File Offset: 0x0045AE41
		private bool CurrCharIsTaiwu
		{
			get
			{
				return base.CharacterMenu.CurrentCharacterIsTaiwu;
			}
		}

		// Token: 0x17001020 RID: 4128
		// (get) Token: 0x060095B8 RID: 38328 RVA: 0x0045CC4E File Offset: 0x0045AE4E
		private bool CurrCharIsTaiwuTeam
		{
			get
			{
				return base.CharacterMenu.CurrentCharacterIsTaiwuTeammate;
			}
		}

		// Token: 0x17001021 RID: 4129
		// (get) Token: 0x060095B9 RID: 38329 RVA: 0x0045CC5B File Offset: 0x0045AE5B
		public bool CanAffectedByCombatDifficulty
		{
			get
			{
				return !this.CurrCharIsTaiwu && !this.CurrCharIsTaiwuTeam && !base.CharacterMenu.IsTaiwuSpecialTeammate(base.CharacterMenu.CurCharacterId);
			}
		}

		// Token: 0x060095BA RID: 38330 RVA: 0x0045CC8C File Offset: 0x0045AE8C
		public override void OnInit(ArgumentBox argsBox)
		{
			bool flag = SingletonObject.getInstance<TutorialChapterModel>().InGuiding && this.CurrCharIsTaiwu;
			if (flag)
			{
				SingletonObject.getInstance<TutorialChapterModel>().WaitOpenCharacterNeili = false;
			}
			UIElement element = this.Element;
			element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(delegate()
			{
				this.localLoadingAnim.SetLoadingState(true);
				this.RequestData();
			}));
			UIElement element2 = this.Element;
			element2.OnHide = (Action)Delegate.Combine(element2.OnHide, new Action(this.Tutorial7Request));
		}

		// Token: 0x060095BB RID: 38331 RVA: 0x0045CD0C File Offset: 0x0045AF0C
		public override void OnSubpageVisible()
		{
			this._shouldShowParticle = false;
			base.CharacterMenu.SetSorting(602);
			base.CharacterMenu.SetBaseAttributeState(0);
			base.CharacterMenu.Attribute.RefreshAllocationBonus = new Action(this.RefreshAllocationBonus);
			base.CharacterMenu.Attribute.SetDisplayPoisonResist(true);
		}

		// Token: 0x060095BC RID: 38332 RVA: 0x0045CD6D File Offset: 0x0045AF6D
		public override void OnSubpageInVisible()
		{
			this.HideNeiliAllocationBonus(0);
			base.CharacterMenu.CancelSorting();
			base.CharacterMenu.Attribute.SetDisplayPoisonResist(false);
		}

		// Token: 0x060095BD RID: 38333 RVA: 0x0045CD96 File Offset: 0x0045AF96
		private void Awake()
		{
			this.InitConsummate();
			this.InitNeiliType();
			this.InitNeiliAllocationType();
			this.InitNeiliAllocationBonus();
			this.BackupCanvasOverrideSorting();
		}

		// Token: 0x060095BE RID: 38334 RVA: 0x0045CDBC File Offset: 0x0045AFBC
		private void OnEnable()
		{
			GEvent.Add(UiEvents.TopUiChanged, new GEvent.Callback(this.OnTopUiChanged));
			GlobalDomainMethod.Call.InvokeGuidingTrigger(112);
		}

		// Token: 0x060095BF RID: 38335 RVA: 0x0045CDDF File Offset: 0x0045AFDF
		private new void OnDisable()
		{
			UIManager.Instance.HideUI(UIElement.FiveElementsPanel);
			GEvent.Remove(UiEvents.TopUiChanged, new GEvent.Callback(this.OnTopUiChanged));
		}

		// Token: 0x060095C0 RID: 38336 RVA: 0x0045CE0C File Offset: 0x0045B00C
		private void OnTopUiChanged(ArgumentBox argBox)
		{
			CharacterMenuSubPageElement myElement = this.Element as CharacterMenuSubPageElement;
			bool flag = myElement != null && !myElement.Visible;
			if (!flag)
			{
				bool flag2 = !UIManager.Instance.IsFocusElement(UIElement.CharacterMenu);
				if (flag2)
				{
					this.DisableAllCanvasOverrideSorting();
					this.backgrounds.gameObject.SetActive(false);
					base.CharacterMenu.CancelSorting();
				}
				else
				{
					this.backgrounds.gameObject.SetActive(true);
					this.RestoreAllCanvasOverrideSorting();
					base.CharacterMenu.SetSorting(602);
					base.CharacterMenu.SetBaseAttributeState(0);
				}
				bool flag3 = UIManager.Instance.IsFocusElement(this.Element);
				if (flag3)
				{
					this.RequestData();
				}
			}
		}

		// Token: 0x060095C1 RID: 38337 RVA: 0x0045CED4 File Offset: 0x0045B0D4
		protected override void OnClick(Transform btn)
		{
			string btnName = btn.name;
			bool flag = "FiveElementsInteract" == btnName;
			if (flag)
			{
				ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
				argBox.Set("NeiliType", this._data.NeiliType);
				UIElement.FiveElementsPanel.SetOnInitArgs(argBox);
				UIManager.Instance.ShowUI(UIElement.FiveElementsPanel, true);
			}
		}

		// Token: 0x060095C2 RID: 38338 RVA: 0x0045CF34 File Offset: 0x0045B134
		private void RequestData()
		{
			bool currentCharacterIsTaiwuTeammate = base.CharacterMenu.CurrentCharacterIsTaiwuTeammate;
			if (currentCharacterIsTaiwuTeammate)
			{
				this.UpdateLock();
			}
			CharacterDomainMethod.AsyncCall.GetCharacterDisplayDataForNeiliPage(null, base.CharacterMenu.CurCharacterId, delegate(int offset, RawDataPool dataPool)
			{
				Serializer.Deserialize(dataPool, offset, ref this._data);
				bool flag = this._data == null;
				if (!flag)
				{
					this.UpdateConsummate((int)this._data.ConsummateLevel);
					this.UpdateNeili(this._data.CurrNeili, this._data.MaxNeili);
					this.UpdateNeiliAllocation(this._data.BaseNeiliAllocation, this._data.CombatNeiliAllocation, this._data.NeiliAllocation, this._data.ConsummateLevel, this._data.CurrNeili, this._data.FeatureIds);
					this.UpdateNeiliType(this._data.NeiliType);
					this.UpdateNeiliPercent(this._data.NeiliPercent, this._data.TransferNeiliSrcType, this._data.TransferNeiliDstType, (int)this._data.TransferNeiliAmount);
					this.UpdateNeiliAllocationBonus();
					this.localLoadingAnim.SetLoadingState(false);
					this.lockToggle.interactable = base.CharacterMenu.CanOperate;
					this.Element.ShowAfterRefresh();
				}
			});
		}

		// Token: 0x060095C3 RID: 38339 RVA: 0x0045CF78 File Offset: 0x0045B178
		public override void OnCurrentCharacterChange(int prevCharacterId)
		{
			bool flag = base.CharacterMenu.CurCharacterId < 0;
			if (!flag)
			{
				this._shouldShowParticle = false;
				this.localLoadingAnim.SetLoadingState(true);
				this.RequestData();
			}
		}

		// Token: 0x060095C4 RID: 38340 RVA: 0x0045CFB8 File Offset: 0x0045B1B8
		private void SwitchConsummateText(Transform obj, bool value)
		{
			obj.GetChild(0).gameObject.SetActive(value);
			obj.GetChild(2).GetChild(0).gameObject.SetActive(value);
			RectTransform tips = obj.GetChild(1).GetComponent<RectTransform>();
			Vector2 sizeDelta = tips.sizeDelta;
			tips.sizeDelta = new Vector2((float)(value ? 170 : 20), sizeDelta.y);
		}

		// Token: 0x060095C5 RID: 38341 RVA: 0x0045D028 File Offset: 0x0045B228
		private void PlayConsummateParticle(int value)
		{
			bool isEven = value % 2 == 0;
			Transform particle = isEven ? this.evenParticle : this.oddParticle;
			float startPos = isEven ? -1080f : -538f;
			int count = value / 2 + (isEven ? -1 : 0);
			float pos = startPos + (float)count * 120f;
			particle.GetComponent<RectTransform>().anchoredPosition = particle.GetComponent<RectTransform>().anchoredPosition.SetY(pos);
			particle.GetChild(0).gameObject.SetActive(true);
			particle.GetChild(0).GetComponent<ParticleSystem>().Play();
		}

		// Token: 0x060095C6 RID: 38342 RVA: 0x0045D0BC File Offset: 0x0045B2BC
		private void StopConsummateParticle(bool isEven)
		{
			Transform particle = isEven ? this.evenParticle : this.oddParticle;
			particle.GetChild(0).gameObject.SetActive(false);
		}

		// Token: 0x060095C7 RID: 38343 RVA: 0x0045D0F0 File Offset: 0x0045B2F0
		private void OnAiAllocationLockToggle(bool isOn)
		{
			bool currentCharacterIsTaiwuTeammate = base.CharacterMenu.CurrentCharacterIsTaiwuTeammate;
			if (currentCharacterIsTaiwuTeammate)
			{
				CharacterDomainMethod.Call.SetNeiliAllocationLock(base.CharacterMenu.CurCharacterId, isOn);
				SingletonObject.getInstance<YieldHelper>().DelayFrameDo(2U, new Action(this.UpdateLock));
			}
		}

		// Token: 0x060095C8 RID: 38344 RVA: 0x0045D13C File Offset: 0x0045B33C
		private void InitConsummate()
		{
			int childIndex = this.consummateSign.childCount - 1 - 2;
			for (int i = 1; i <= (int)GlobalConfig.Instance.MaxConsummateLevel; i++)
			{
				Transform obj = this.consummateSign.GetChild(childIndex--);
				bool flag = i % 2 != 0;
				if (flag)
				{
					obj.GetComponent<TooltipInvoker>().PresetParam[0] = CommonUtils.GetConsummateLevelTips(i);
				}
				else
				{
					int index = i / 2 - 1;
					string labelText = LocalStringManager.Get(string.Format("LK_Consummate_Level_{0}", index));
					obj.GetChild(0).GetComponent<TextMeshProUGUI>().SetText(labelText, true);
					TooltipInvoker mouseTip = obj.GetChild(1).GetComponent<TooltipInvoker>();
					ConsummateLevelItem config = ConsummateLevel.Instance[i];
					mouseTip.PresetParam[0] = config.Name.ColorReplace();
					mouseTip.PresetParam[1] = CommonUtils.GetConsummateLevelTips(i) + "\n\n" + config.Desc.ColorReplace();
				}
			}
		}

		// Token: 0x060095C9 RID: 38345 RVA: 0x0045D240 File Offset: 0x0045B440
		private void InitNeiliType()
		{
			this.neiliTypePanel.Init();
			this.fiveElementsInteract.sprite = this.fiveElementsInteractImage[0];
			bool isReadable = this.fiveElementsInteract.sprite.texture.isReadable;
			if (isReadable)
			{
				this.fiveElementsInteract.alphaHitTestMinimumThreshold = 0.5f;
			}
			PointerTrigger pointerTrigger = this.fiveElementsInteract.GetComponent<PointerTrigger>();
			pointerTrigger.EnterEvent.RemoveAllListeners();
			pointerTrigger.EnterEvent.AddListener(delegate()
			{
				this.fiveElementsInteract.sprite = this.fiveElementsInteractImage[1];
			});
			pointerTrigger.ExitEvent.RemoveAllListeners();
			pointerTrigger.ExitEvent.AddListener(delegate()
			{
				this.fiveElementsInteract.sprite = this.fiveElementsInteractImage[0];
			});
		}

		// Token: 0x060095CA RID: 38346 RVA: 0x0045D2EC File Offset: 0x0045B4EC
		private void InitNeiliAllocationType()
		{
			this.neiliAllocationType.Init(new Action<byte>(this.OnClickAdd), new Action<byte>(this.OnClickMinus), new Action<byte>(this.ShowNeiliAllocationBonus), new Action<byte>(this.HideNeiliAllocationBonus), new Action<byte, bool>(this.ShowNeiliAllocationBonusByBtn));
			this.lockToggle.onValueChanged.RemoveAllListeners();
			this.lockToggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnAiAllocationLockToggle));
			this.btnAutoAllocate.ClearAndAddListener(delegate
			{
				CharacterDomainMethod.Call.AutoAllocateNeili(base.CharacterMenu.CurCharacterId);
				bool currentCharacterIsTaiwuTeammate = base.CharacterMenu.CurrentCharacterIsTaiwuTeammate;
				if (currentCharacterIsTaiwuTeammate)
				{
					this.lockToggle.isOn = false;
				}
				this.RequestData();
			});
		}

		// Token: 0x060095CB RID: 38347 RVA: 0x0045D388 File Offset: 0x0045B588
		private void InitNeiliAllocationBonus()
		{
			this._neiliAllocationBonus = new Dictionary<sbyte, Dictionary<short, int>>();
			for (sbyte i = 0; i < 4; i += 1)
			{
				this._neiliAllocationBonus[i] = new Dictionary<short, int>();
				foreach (short type in this._neiliAllocationBonusPropertyTypes)
				{
					this._neiliAllocationBonus[i][type] = 0;
				}
			}
		}

		// Token: 0x060095CC RID: 38348 RVA: 0x0045D420 File Offset: 0x0045B620
		private void UpdateConsummate(int value)
		{
			sbyte maxValue = GlobalConfig.Instance.MaxConsummateLevel;
			value = Math.Min(value, (int)maxValue);
			float progress = this.consummateProgress.GetComponent<RectTransform>().rect.height * (float)((int)maxValue - value) / (float)maxValue;
			int bgIndex = value / 2;
			this.consummateProgress.padding = new Vector4(0f, 0f, 0f, progress);
			for (int i = 0; i < this.backgrounds.childCount; i++)
			{
				this.backgrounds.GetChild(i).gameObject.SetActive(bgIndex == i);
			}
			int childIndex = this.consummateSign.childCount - 1 - 2;
			Transform currObj = null;
			for (int j = 1; j <= (int)GlobalConfig.Instance.MaxConsummateLevel; j++)
			{
				Transform obj = this.consummateSign.GetChild(childIndex--);
				bool flag = j == value;
				if (flag)
				{
					currObj = obj;
				}
				bool flag2 = j % 2 == 0;
				if (flag2)
				{
					this.SwitchConsummateText(obj, false);
				}
			}
			bool flag3 = currObj != null;
			if (flag3)
			{
				bool isEven = value % 2 == 0;
				bool needShowConsummateParticle = this._data.NeedShowConsummateParticle;
				if (needShowConsummateParticle)
				{
					Sequence sequence = DOTween.Sequence();
					sequence.AppendCallback(delegate
					{
						this.PlayConsummateParticle(value);
					});
					sequence.AppendInterval(2f);
					bool isEven3 = isEven;
					if (isEven3)
					{
						sequence.AppendCallback(delegate
						{
							this.SwitchConsummateText(currObj, true);
						});
					}
					sequence.AppendInterval(1f);
					sequence.AppendCallback(delegate
					{
						this.StopConsummateParticle(isEven);
					});
					sequence.Play<Sequence>();
					TaiwuDomainMethod.Call.SetConsummateLevelOnNeiliPage();
				}
				else
				{
					bool isEven2 = isEven;
					if (isEven2)
					{
						this.SwitchConsummateText(currObj, true);
					}
				}
			}
		}

		// Token: 0x060095CD RID: 38349 RVA: 0x0045D66C File Offset: 0x0045B86C
		private void UpdateNeili(int value, int maxValue)
		{
			string neili = value.ToString().SetColor("brightblue");
			string maxNeili = maxValue.ToString().SetColor("yellow");
			string str = LocalStringManager.GetFormat(LanguageKey.LK_Total_Neili_Tips_1, "  " + neili + "/" + maxNeili);
			this.neiliText.SetText(str, true);
			float progress = this.neiliProgress.GetComponent<RectTransform>().rect.width * (float)(maxValue - value) / (float)maxValue;
			this.neiliProgress.padding = new Vector4(0f, 0f, progress, 0f);
		}

		// Token: 0x060095CE RID: 38350 RVA: 0x0045D70C File Offset: 0x0045B90C
		private unsafe void UpdateNeiliAllocation(NeiliAllocation baseValue, NeiliAllocation combatValue, NeiliAllocation value, sbyte consummateLevel, int currNeili, List<short> featureIds)
		{
			ChallengeModeData challengeModeData = SingletonObject.getInstance<BasicGameData>().ChallengeModeData;
			string baseText = baseValue.GetTotal().ToString().SetColor("brightblue");
			string totalText = CombatHelper.GetMaxTotalNeiliAllocationConsideringFeature(consummateLevel, featureIds, challengeModeData).ToString();
			CharacterMenuFunctionControlItem config;
			bool banned = (base.CharacterMenu.TryGetFunctionControlConfig(out config) && base.CharacterMenu.IsFunctionBanned(config.Neili)) || !base.CharacterMenu.CanOperate;
			this.btnAutoAllocate.gameObject.SetActive(base.CharacterMenu.CurrentCharacterIsTaiwuTeammate);
			this.lockToggle.gameObject.SetActive(base.CharacterMenu.CurrentCharacterIsTaiwuTeammate);
			this.neiliAllocationText.SetText(string.Concat(new string[]
			{
				LanguageKey.LK_Max_Neili_Allocation_Tips.Tr(),
				LanguageKey.LK_Colon_Symbol.Tr(),
				" ",
				baseText,
				"/",
				totalText
			}), true);
			this.btnAutoAllocate.interactable = !banned;
			for (byte i = 0; i < 4; i += 1)
			{
				short allocateValue = (combatValue.GetTotal() > 0) ? (*combatValue[(int)i]) : (*value[(int)i]);
				int displayDelta = (int)(*value[(int)i] - *baseValue[(int)i]);
				int displayValue = (int)allocateValue - displayDelta;
				bool canInteract = (this.CurrCharIsTaiwuTeam || this.CurrCharIsTaiwu) && base.CharacterMenu.CanOperate && !SingletonObject.getInstance<DisplayTriggerModel>().IsNeiliAllocationTypeRestricted(i) && !banned;
				this.neiliAllocationType.Set(i, displayValue, displayDelta, true);
				this.neiliAllocationType.SetCanInteract(i, canInteract && CombatHelper.CanAllocateNeiliConsideringFeature(i, baseValue, currNeili, consummateLevel, featureIds, challengeModeData), canInteract && *baseValue[(int)i] > 0);
			}
		}

		// Token: 0x060095CF RID: 38351 RVA: 0x0045D8F9 File Offset: 0x0045BAF9
		private void UpdateNeiliType(sbyte type)
		{
			this.neiliTypePanel.Set(type);
		}

		// Token: 0x060095D0 RID: 38352 RVA: 0x0045D909 File Offset: 0x0045BB09
		private void UpdateNeiliPercent(NeiliProportionOfFiveElements currValue, sbyte transferType, sbyte dstType, int amount)
		{
			this.neiliTypePanel.Set(currValue, -1, -1, -1);
		}

		// Token: 0x060095D1 RID: 38353 RVA: 0x0045D91C File Offset: 0x0045BB1C
		private void UpdateLock()
		{
			CharacterDomainMethod.AsyncCall.IsNeiliAllocationLocked(null, base.CharacterMenu.CurCharacterId, delegate(int offset, RawDataPool dataPool)
			{
				bool res = false;
				Serializer.Deserialize(dataPool, offset, ref res);
				this.lockToggle.SetIsOnWithoutNotify(res);
			});
		}

		// Token: 0x060095D2 RID: 38354 RVA: 0x0045D940 File Offset: 0x0045BB40
		private unsafe void UpdateNeiliAllocationBonus()
		{
			NeiliTypeItem neiliTypeCfg = NeiliType.Instance[this._data.NeiliType];
			for (sbyte allocationType = 0; allocationType < 4; allocationType += 1)
			{
				NeiliAllocationEffectItem allocationCfg = NeiliAllocationEffect.Instance[allocationType];
				foreach (short typeShort in this._neiliAllocationBonusPropertyTypes)
				{
					ECharacterPropertyReferencedType type = (ECharacterPropertyReferencedType)typeShort;
					int stepSize = allocationCfg.GetMapping(type);
					bool flag = stepSize <= 0;
					if (!flag)
					{
						int valuePerStep = neiliTypeCfg.GetMapping(type);
						bool flag2 = valuePerStep == 0;
						if (!flag2)
						{
							short allocation = *this._data.NeiliAllocation[(int)allocationType];
							short allocationEffect = *this._data.NeiliAllocationEffects[(int)allocationType];
							int stepCount = (int)allocation / stepSize + (int)allocationEffect / stepSize;
							int value = valuePerStep * stepCount;
							int oldValue = this._neiliAllocationBonus[allocationType][typeShort];
							bool flag3 = value > oldValue && oldValue != 0 && this._shouldShowParticle;
							if (flag3)
							{
								this.GetPropertyItem(typeShort).PlayParticle();
							}
							bool canAffectedByCombatDifficulty = this.CanAffectedByCombatDifficulty;
							if (canAffectedByCombatDifficulty)
							{
								byte combatDifficulty = SingletonObject.getInstance<BasicGameData>().CombatDifficulty;
								short factor = Config.CombatDifficulty.Instance[combatDifficulty].Penetrations;
								value = value * (int)factor / 100;
							}
							this._neiliAllocationBonus[allocationType][typeShort] = value;
						}
					}
				}
			}
			bool currCharIsTaiwu = this.CurrCharIsTaiwu;
			if (currCharIsTaiwu)
			{
				this._shouldShowParticle = true;
			}
		}

		// Token: 0x060095D3 RID: 38355 RVA: 0x0045DAF4 File Offset: 0x0045BCF4
		private PropertyItem GetPropertyItem(short type)
		{
			if (!true)
			{
			}
			PropertyItem result;
			switch (type)
			{
			case 6:
				result = base.CharacterMenu.Attribute.HitValueItems[0];
				break;
			case 7:
				result = base.CharacterMenu.Attribute.HitValueItems[1];
				break;
			case 8:
				result = base.CharacterMenu.Attribute.HitValueItems[2];
				break;
			case 9:
				result = base.CharacterMenu.Attribute.HitValueItems[3];
				break;
			case 10:
				result = base.CharacterMenu.Attribute.PenetrationOuter;
				break;
			case 11:
				result = base.CharacterMenu.Attribute.PenetrationInner;
				break;
			case 12:
				result = base.CharacterMenu.Attribute.AvoidValueItems[0];
				break;
			case 13:
				result = base.CharacterMenu.Attribute.AvoidValueItems[1];
				break;
			case 14:
				result = base.CharacterMenu.Attribute.AvoidValueItems[2];
				break;
			case 15:
				result = base.CharacterMenu.Attribute.AvoidValueItems[3];
				break;
			case 16:
				result = base.CharacterMenu.Attribute.PenetrationResistOuter;
				break;
			case 17:
				result = base.CharacterMenu.Attribute.PenetrationResistInner;
				break;
			case 18:
				result = base.CharacterMenu.Attribute.RecoveryOfStance;
				break;
			case 19:
				result = base.CharacterMenu.Attribute.RecoveryOfBreath;
				break;
			case 20:
				result = base.CharacterMenu.Attribute.MoveSpeed;
				break;
			case 21:
				result = base.CharacterMenu.Attribute.RecoveryOfFlaw;
				break;
			case 22:
				result = base.CharacterMenu.Attribute.CastSpeed;
				break;
			case 23:
				result = base.CharacterMenu.Attribute.RecoveryOfBlockedAcupoint;
				break;
			case 24:
				result = base.CharacterMenu.Attribute.WeaponSwitchSpeed;
				break;
			case 25:
				result = base.CharacterMenu.Attribute.AttackSpeed;
				break;
			case 26:
				result = base.CharacterMenu.Attribute.InnerRatio;
				break;
			case 27:
				result = base.CharacterMenu.Attribute.RecoveryOfQiDisorder;
				break;
			case 28:
				result = base.CharacterMenu.Attribute.PoisonResistItem[0];
				break;
			case 29:
				result = base.CharacterMenu.Attribute.PoisonResistItem[1];
				break;
			case 30:
				result = base.CharacterMenu.Attribute.PoisonResistItem[2];
				break;
			case 31:
				result = base.CharacterMenu.Attribute.PoisonResistItem[3];
				break;
			case 32:
				result = base.CharacterMenu.Attribute.PoisonResistItem[4];
				break;
			case 33:
				result = base.CharacterMenu.Attribute.PoisonResistItem[5];
				break;
			default:
				throw new ArgumentOutOfRangeException("type", type, null);
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x060095D4 RID: 38356 RVA: 0x0045DE0C File Offset: 0x0045C00C
		private unsafe int GetCurrentData(short type)
		{
			if (!true)
			{
			}
			int result;
			switch (type)
			{
			case 6:
				result = base.CharacterMenu.Attribute.CurrentData.AtkHitAttribute[0];
				break;
			case 7:
				result = base.CharacterMenu.Attribute.CurrentData.AtkHitAttribute[1];
				break;
			case 8:
				result = base.CharacterMenu.Attribute.CurrentData.AtkHitAttribute[2];
				break;
			case 9:
				result = base.CharacterMenu.Attribute.CurrentData.AtkHitAttribute[3];
				break;
			case 10:
				result = base.CharacterMenu.Attribute.CurrentData.AtkPenetrability.Outer;
				break;
			case 11:
				result = base.CharacterMenu.Attribute.CurrentData.AtkPenetrability.Inner;
				break;
			case 12:
				result = base.CharacterMenu.Attribute.CurrentData.DefHitAttribute[0];
				break;
			case 13:
				result = base.CharacterMenu.Attribute.CurrentData.DefHitAttribute[1];
				break;
			case 14:
				result = base.CharacterMenu.Attribute.CurrentData.DefHitAttribute[2];
				break;
			case 15:
				result = base.CharacterMenu.Attribute.CurrentData.DefHitAttribute[3];
				break;
			case 16:
				result = base.CharacterMenu.Attribute.CurrentData.DefPenetrability.Outer;
				break;
			case 17:
				result = base.CharacterMenu.Attribute.CurrentData.DefPenetrability.Inner;
				break;
			case 18:
				result = (int)base.CharacterMenu.Attribute.CurrentData.RecoveryOfStanceAndBreath.Outer;
				break;
			case 19:
				result = (int)base.CharacterMenu.Attribute.CurrentData.RecoveryOfStanceAndBreath.Inner;
				break;
			case 20:
				result = (int)base.CharacterMenu.Attribute.CurrentData.MoveSpeed;
				break;
			case 21:
				result = (int)base.CharacterMenu.Attribute.CurrentData.RecoveryOfFlaw;
				break;
			case 22:
				result = (int)base.CharacterMenu.Attribute.CurrentData.CastSpeed;
				break;
			case 23:
				result = (int)base.CharacterMenu.Attribute.CurrentData.RecoveryOfBlockedAcupoint;
				break;
			case 24:
				result = (int)base.CharacterMenu.Attribute.CurrentData.WeaponSwitchSpeed;
				break;
			case 25:
				result = (int)base.CharacterMenu.Attribute.CurrentData.AttackSpeed;
				break;
			case 26:
				result = (int)base.CharacterMenu.Attribute.CurrentData.InnerRatio;
				break;
			case 27:
				result = (int)base.CharacterMenu.Attribute.CurrentData.RecoveryOfQiDisorder;
				break;
			case 28:
				result = *base.CharacterMenu.Attribute.CurrentData.PoisonResists[0];
				break;
			case 29:
				result = *base.CharacterMenu.Attribute.CurrentData.PoisonResists[1];
				break;
			case 30:
				result = *base.CharacterMenu.Attribute.CurrentData.PoisonResists[2];
				break;
			case 31:
				result = *base.CharacterMenu.Attribute.CurrentData.PoisonResists[3];
				break;
			case 32:
				result = *base.CharacterMenu.Attribute.CurrentData.PoisonResists[4];
				break;
			case 33:
				result = *base.CharacterMenu.Attribute.CurrentData.PoisonResists[5];
				break;
			default:
				throw new ArgumentOutOfRangeException("type", type, null);
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x060095D5 RID: 38357 RVA: 0x0045E214 File Offset: 0x0045C414
		private void RefreshAllocationBonus()
		{
			bool flag = this._currDisplayingType >= 0;
			if (flag)
			{
				this.ShowNeiliAllocationBonus((byte)this._currDisplayingType);
			}
		}

		// Token: 0x060095D6 RID: 38358 RVA: 0x0045E240 File Offset: 0x0045C440
		private void OnClickAdd(byte type)
		{
			CharacterDomainMethod.Call.AllocateNeili(base.CharacterMenu.CurCharacterId, type);
			bool currentCharacterIsTaiwuTeammate = base.CharacterMenu.CurrentCharacterIsTaiwuTeammate;
			if (currentCharacterIsTaiwuTeammate)
			{
				this.lockToggle.isOn = true;
			}
			bool flag = !this.neiliAllocationTypeParticles[(int)type].isPlaying;
			if (flag)
			{
				this.neiliAllocationTypeParticles[(int)type].Play();
			}
			base.CharacterMenu.Attribute.IsDisplayingAllocationBonus = false;
			this.RequestData();
		}

		// Token: 0x060095D7 RID: 38359 RVA: 0x0045E2B8 File Offset: 0x0045C4B8
		private void OnClickMinus(byte type)
		{
			CharacterDomainMethod.Call.DeallocateNeili(base.CharacterMenu.CurCharacterId, type);
			bool currentCharacterIsTaiwuTeammate = base.CharacterMenu.CurrentCharacterIsTaiwuTeammate;
			if (currentCharacterIsTaiwuTeammate)
			{
				this.lockToggle.isOn = true;
			}
			bool flag = !this.neiliAllocationTypeParticles[(int)type].isPlaying;
			if (flag)
			{
				this.neiliAllocationTypeParticles[(int)type].Play();
			}
			base.CharacterMenu.Attribute.IsDisplayingAllocationBonus = false;
			this.RequestData();
		}

		// Token: 0x060095D8 RID: 38360 RVA: 0x0045E330 File Offset: 0x0045C530
		private void ShowNeiliAllocationBonus(byte type)
		{
			bool flag = base.CharacterMenu.Attribute.CurrentData == null;
			if (!flag)
			{
				bool isDisplayingAllocationBonus = base.CharacterMenu.Attribute.IsDisplayingAllocationBonus;
				if (!isDisplayingAllocationBonus)
				{
					base.CharacterMenu.Attribute.IsDisplayingAllocationBonus = true;
					this._currDisplayingType = (sbyte)type;
					Dictionary<short, int> dic = this._neiliAllocationBonus[this._currDisplayingType];
					foreach (KeyValuePair<short, int> keyValuePair in dic)
					{
						short num;
						int num2;
						keyValuePair.Deconstruct(out num, out num2);
						short typeShort = num;
						int value = num2;
						bool flag2 = value <= 0;
						if (!flag2)
						{
							int currValue = this.GetCurrentData(typeShort);
							PropertyItem propertyItem = this.GetPropertyItem(typeShort);
							string addStr = ("+" + value.ToString()).SetColor("brightblue");
							string percent = this.NeedPercent(typeShort) ? "%" : "";
							PropertyItem propertyItem2 = propertyItem;
							num2 = currValue - value;
							propertyItem2.SetValue(num2.ToString() + addStr + percent);
						}
					}
				}
			}
		}

		// Token: 0x060095D9 RID: 38361 RVA: 0x0045E470 File Offset: 0x0045C670
		private void HideNeiliAllocationBonus(byte type)
		{
			bool flag = this._currDisplayingType >= 0;
			if (flag)
			{
				base.CharacterMenu.Attribute.IsDisplayingAllocationBonus = false;
				this._currDisplayingType = -1;
				base.CharacterMenu.Attribute.Refresh();
			}
		}

		// Token: 0x060095DA RID: 38362 RVA: 0x0045E4BC File Offset: 0x0045C6BC
		private void ShowNeiliAllocationBonusByBtn(byte type, bool isAdd)
		{
			bool flag = base.CharacterMenu.Attribute.CurrentData == null;
			if (!flag)
			{
				bool isDisplayingAllocationBonus = base.CharacterMenu.Attribute.IsDisplayingAllocationBonus;
				if (!isDisplayingAllocationBonus)
				{
					base.CharacterMenu.Attribute.IsDisplayingAllocationBonus = true;
					this._currDisplayingType = (sbyte)type;
					CharacterDomainMethod.AsyncCall.PreviewAllocateNeili(null, base.CharacterMenu.CurCharacterId, type, isAdd, delegate(int offset, RawDataPool dataPool)
					{
						AllocateNeiliPreviewDisplayData previewData = null;
						Serializer.Deserialize(dataPool, offset, ref previewData);
						bool flag2 = previewData == null;
						if (!flag2)
						{
							bool flag3 = this._currDisplayingType != (sbyte)type || !this.CharacterMenu.Attribute.IsDisplayingAllocationBonus;
							if (!flag3)
							{
								foreach (KeyValuePair<short, int> keyValuePair in previewData.PropertyDeltas)
								{
									short num;
									int num2;
									keyValuePair.Deconstruct(out num, out num2);
									short typeShort = num;
									int delta = num2;
									bool flag4 = delta == 0;
									if (!flag4)
									{
										int currValue = this.GetCurrentData(typeShort);
										PropertyItem propertyItem = this.GetPropertyItem(typeShort);
										string percent = this.NeedPercent(typeShort) ? "%" : "";
										bool flag5 = delta > 0;
										if (flag5)
										{
											string addStr = ("+" + delta.ToString()).SetColor("brightblue");
											propertyItem.SetValue(currValue.ToString() + addStr + percent);
										}
										else
										{
											string addStr2 = (delta.ToString() ?? "").SetColor("brightred");
											propertyItem.SetValue(currValue.ToString() + addStr2 + percent);
										}
									}
								}
							}
						}
					});
				}
			}
		}

		// Token: 0x060095DB RID: 38363 RVA: 0x0045E550 File Offset: 0x0045C750
		private bool NeedPercent(short type)
		{
			return type >= 18 && type <= 27;
		}

		// Token: 0x060095DC RID: 38364 RVA: 0x0045E574 File Offset: 0x0045C774
		public unsafe bool NeiliAllocateOkForTutorial7()
		{
			for (byte i = 0; i < 4; i += 1)
			{
				bool flag = *this._data.NeiliAllocation[(int)i] < 90;
				if (flag)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x060095DD RID: 38365 RVA: 0x0045E5B8 File Offset: 0x0045C7B8
		private void Tutorial7Request()
		{
			TutorialChapterModel tutorialChapterModel = SingletonObject.getInstance<TutorialChapterModel>();
			bool flag = tutorialChapterModel.IsInTutorialChapter(6);
			if (flag)
			{
				bool flag2 = this.NeiliAllocateOkForTutorial7();
				if (flag2)
				{
					TaiwuEventDomainMethod.Call.TriggerListener("FinishNeiliAllocateInTutorial7", true);
				}
			}
		}

		// Token: 0x060095DE RID: 38366 RVA: 0x0045E5F4 File Offset: 0x0045C7F4
		private void BackupCanvasOverrideSorting()
		{
			this._canvasBackups.Clear();
			Canvas[] allCanvases = base.GetComponentsInChildren<Canvas>(true);
			foreach (Canvas canvas in allCanvases)
			{
				bool overrideSorting = canvas.overrideSorting;
				if (overrideSorting)
				{
					this._canvasBackups.Add(new ValueTuple<Canvas, int>(canvas, canvas.sortingOrder));
				}
			}
		}

		// Token: 0x060095DF RID: 38367 RVA: 0x0045E654 File Offset: 0x0045C854
		public void DisableAllCanvasOverrideSorting()
		{
			foreach (ValueTuple<Canvas, int> valueTuple in this._canvasBackups)
			{
				Canvas canvas = valueTuple.Item1;
				bool flag = canvas != null;
				if (flag)
				{
					canvas.overrideSorting = false;
				}
			}
		}

		// Token: 0x060095E0 RID: 38368 RVA: 0x0045E6C0 File Offset: 0x0045C8C0
		public void RestoreAllCanvasOverrideSorting()
		{
			foreach (ValueTuple<Canvas, int> valueTuple in this._canvasBackups)
			{
				Canvas canvas = valueTuple.Item1;
				int sortingOrder = valueTuple.Item2;
				bool flag = canvas != null;
				if (flag)
				{
					canvas.overrideSorting = true;
					canvas.sortingOrder = sortingOrder;
				}
			}
		}

		// Token: 0x040072D6 RID: 29398
		private const float EvenParticleStartPos = -1080f;

		// Token: 0x040072D7 RID: 29399
		private const float OddParticleStartPos = -538f;

		// Token: 0x040072D8 RID: 29400
		private const float ParticleOffset = 120f;

		// Token: 0x040072D9 RID: 29401
		private readonly List<short> _neiliAllocationBonusPropertyTypes = new List<short>
		{
			6,
			7,
			8,
			9,
			10,
			11,
			12,
			13,
			14,
			15,
			16,
			17,
			18,
			19,
			20,
			21,
			22,
			23,
			24,
			25,
			26,
			27,
			28,
			29,
			30,
			31,
			32,
			33
		};

		// Token: 0x040072DA RID: 29402
		public Transform backgrounds;

		// Token: 0x040072DB RID: 29403
		public RectMask2D consummateProgress;

		// Token: 0x040072DC RID: 29404
		public Transform consummateSign;

		// Token: 0x040072DD RID: 29405
		public TextMeshProUGUI neiliText;

		// Token: 0x040072DE RID: 29406
		public RectMask2D neiliProgress;

		// Token: 0x040072DF RID: 29407
		public TextMeshProUGUI neiliAllocationText;

		// Token: 0x040072E0 RID: 29408
		public NeiliTypePanel neiliTypePanel;

		// Token: 0x040072E1 RID: 29409
		public NeiliAllocationTypes neiliAllocationType;

		// Token: 0x040072E2 RID: 29410
		public ParticleSystem[] neiliAllocationTypeParticles;

		// Token: 0x040072E3 RID: 29411
		public Transform evenParticle;

		// Token: 0x040072E4 RID: 29412
		public Transform oddParticle;

		// Token: 0x040072E5 RID: 29413
		public CButton btnAutoAllocate;

		// Token: 0x040072E6 RID: 29414
		public CToggle lockToggle;

		// Token: 0x040072E7 RID: 29415
		public CImage fiveElementsInteract;

		// Token: 0x040072E8 RID: 29416
		public Sprite[] fiveElementsInteractImage;

		// Token: 0x040072E9 RID: 29417
		[SerializeField]
		private CharacterMenuLocalLoadingAnim localLoadingAnim;

		// Token: 0x040072EA RID: 29418
		private CharacterDisplayDataForNeiliPage _data;

		// Token: 0x040072EB RID: 29419
		private Dictionary<sbyte, Dictionary<short, int>> _neiliAllocationBonus;

		// Token: 0x040072EC RID: 29420
		private bool _shouldShowParticle = false;

		// Token: 0x040072ED RID: 29421
		private sbyte _currDisplayingType = -1;

		// Token: 0x040072EE RID: 29422
		[TupleElementNames(new string[]
		{
			"canvas",
			"sortingOrder"
		})]
		private List<ValueTuple<Canvas, int>> _canvasBackups = new List<ValueTuple<Canvas, int>>();
	}
}
