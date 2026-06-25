using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Views.CharacterMenu;
using Game.Views.CharacterMenu.Practice;
using Game.Views.SectInteract.Shaolin;
using GameData.Domains.Character.Display;
using GameData.Domains.Combat;
using GameData.Domains.CombatSkill;
using GameData.Domains.Extra;
using GameData.Domains.Organization;
using GameData.Domains.Taiwu;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UISkillBreakPlate;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Components.Common
{
	// Token: 0x02000F88 RID: 3976
	public class CombatSkillPanel : MonoBehaviour
	{
		// Token: 0x1700149E RID: 5278
		// (get) Token: 0x0600B6D0 RID: 46800 RVA: 0x00535495 File Offset: 0x00533695
		public int NormalPagesCount
		{
			get
			{
				return this.otherPageToggleGroup.GetIsOnCount();
			}
		}

		// Token: 0x1700149F RID: 5279
		// (get) Token: 0x0600B6D1 RID: 46801 RVA: 0x005354A2 File Offset: 0x005336A2
		public int OutlinePagesCount
		{
			get
			{
				return this.outlinePageToggleGroup.GetIsOnCount();
			}
		}

		// Token: 0x170014A0 RID: 5280
		// (get) Token: 0x0600B6D2 RID: 46802 RVA: 0x005354AF File Offset: 0x005336AF
		public List<int> ActivatedNormalPages
		{
			get
			{
				return this.otherPageToggleGroup.GetActiveIndices();
			}
		}

		// Token: 0x170014A1 RID: 5281
		// (get) Token: 0x0600B6D3 RID: 46803 RVA: 0x005354BC File Offset: 0x005336BC
		public int ActivatedOutlinePage
		{
			get
			{
				return this.outlinePageToggleGroup.GetActiveIndex();
			}
		}

		// Token: 0x0600B6D4 RID: 46804 RVA: 0x005354C9 File Offset: 0x005336C9
		public bool IsNormalPageActivated(int i)
		{
			return this.otherPageToggleGroup.Get(i).isOn;
		}

		// Token: 0x170014A2 RID: 5282
		// (get) Token: 0x0600B6D5 RID: 46805 RVA: 0x005354DC File Offset: 0x005336DC
		private int CharId
		{
			get
			{
				return this._charData.CharId;
			}
		}

		// Token: 0x170014A3 RID: 5283
		// (get) Token: 0x0600B6D6 RID: 46806 RVA: 0x005354E9 File Offset: 0x005336E9
		private short SkillTemplateId
		{
			get
			{
				return this._skillData.CombatSkillDisplayData.TemplateId;
			}
		}

		// Token: 0x170014A4 RID: 5284
		// (get) Token: 0x0600B6D7 RID: 46807 RVA: 0x005354FB File Offset: 0x005336FB
		private bool CurrCharIsTaiwu
		{
			get
			{
				return SingletonObject.getInstance<BasicGameData>().TaiwuCharId == this._charData.CharId;
			}
		}

		// Token: 0x170014A5 RID: 5285
		// (get) Token: 0x0600B6D8 RID: 46808 RVA: 0x00535514 File Offset: 0x00533714
		private ushort ReadingState
		{
			get
			{
				return this._skillData.CombatSkillDisplayData.ReadingState;
			}
		}

		// Token: 0x170014A6 RID: 5286
		// (get) Token: 0x0600B6D9 RID: 46809 RVA: 0x00535526 File Offset: 0x00533726
		private ushort ActivationState
		{
			get
			{
				return this._skillData.CombatSkillDisplayData.ActivationState;
			}
		}

		// Token: 0x170014A7 RID: 5287
		// (get) Token: 0x0600B6DA RID: 46810 RVA: 0x00535538 File Offset: 0x00533738
		private bool IsBroken
		{
			get
			{
				return CombatSkillStateHelper.IsBrokenOut(this.ActivationState);
			}
		}

		// Token: 0x170014A8 RID: 5288
		// (get) Token: 0x0600B6DB RID: 46811 RVA: 0x00535545 File Offset: 0x00533745
		private bool IsRevoked
		{
			get
			{
				return this._skillData.CombatSkillDisplayData.Revoked;
			}
		}

		// Token: 0x170014A9 RID: 5289
		// (get) Token: 0x0600B6DC RID: 46812 RVA: 0x00535557 File Offset: 0x00533757
		private bool IsLuohanBreak
		{
			get
			{
				return this._skillData.CombatSkillDisplayData.LuohanId >= 0;
			}
		}

		// Token: 0x170014AA RID: 5290
		// (get) Token: 0x0600B6DD RID: 46813 RVA: 0x0053556F File Offset: 0x0053376F
		private GameData.Domains.Taiwu.SkillBreakPlate Plate
		{
			get
			{
				return this._skillData.SkillBreakPlate;
			}
		}

		// Token: 0x0600B6DE RID: 46814 RVA: 0x0053557C File Offset: 0x0053377C
		public void Init(Action updateBackendData, Action<CombatSkillDisplayData> updateFrontendData, RectTransform parent, Func<bool> canOperate = null, Action onReady = null)
		{
			this._updateBackendData = updateBackendData;
			this._updateFrontendData = updateFrontendData;
			this._canOperate = canOperate;
			this._onReady = onReady;
			this.jumpPanel.SetParent(parent);
			this.jumpPanel.anchoredPosition = Vector2.zero;
			this.btnInnerAdjust.ClearAndAddListener(delegate
			{
				float currentValue = this.innerRatioSlider.value;
				this.innerRatioSlider.value = Mathf.Clamp(currentValue + 1f, 0f, this.innerRatioSlider.maxValue);
			});
			this.btnOuterAdjust.ClearAndAddListener(delegate
			{
				float currentValue = this.innerRatioSlider.value;
				this.innerRatioSlider.value = Mathf.Clamp(currentValue - 1f, 0f, this.innerRatioSlider.maxValue);
			});
			this.InitPreset();
			this.InitLuohan();
			this.InitBreakPanel();
			this.InitMastered();
			this.InitJump();
			this.InitInnerRatio();
		}

		// Token: 0x0600B6DF RID: 46815 RVA: 0x00535620 File Offset: 0x00533820
		public void Set(CombatSkillPracticeDisplayData skillData, CharacterDisplayDataForPractice charData, bool keepOutline)
		{
			this._skillData = skillData;
			this._charData = charData;
			this._keepOutline = keepOutline;
			this.UpdateCenter();
			this.UpdatePreset();
			this.UpdateBreakPanel();
			this.UpdateLuohan();
			this.UpdateMastered();
			this.UpdateJump();
			this.UpdateInnerRatio();
			this.UpdateInteractable();
		}

		// Token: 0x0600B6E0 RID: 46816 RVA: 0x0053567C File Offset: 0x0053387C
		private void UpdateLuohanInteractable()
		{
			bool flag = this.luohanBreak == null;
			if (!flag)
			{
				bool canOperate = this._canOperate == null || this._canOperate();
				CButton btn = this.luohanBreak.GetComponent<CButton>();
				btn.interactable = (btn.interactable && canOperate && this.NormalPagesCount == 5 && this.OutlinePagesCount == 1);
			}
		}

		// Token: 0x0600B6E1 RID: 46817 RVA: 0x005356E5 File Offset: 0x005338E5
		private void OnDisable()
		{
			this._innerRatioLock = true;
			this._skillData = null;
			this._charData = null;
		}

		// Token: 0x0600B6E2 RID: 46818 RVA: 0x005356FD File Offset: 0x005338FD
		private void InitPreset()
		{
			this.presetToggleGroup.Init(-1);
			this.presetToggleGroup.OnActiveIndexChange += this.OnPresetChange;
		}

		// Token: 0x0600B6E3 RID: 46819 RVA: 0x00535725 File Offset: 0x00533925
		private void InitLuohan()
		{
			this.luohanBreak.Init(new Action<sbyte>(this.OnConfirmLuohan));
		}

		// Token: 0x0600B6E4 RID: 46820 RVA: 0x00535740 File Offset: 0x00533940
		private void InitBreakPanel()
		{
			List<CToggle> outlineTogList = this.outlinePageToggleGroup.GetAll();
			for (int index = 0; index < outlineTogList.Count; index++)
			{
				CToggle tog = outlineTogList[index];
				PracticeSlice slice = tog.GetComponent<PracticeSlice>();
				slice.AutoRotateNameBg();
				slice.RefreshStyle();
				slice.SetNum((index + 1).ToString().SetColor("white"));
				slice.SetNameLabel(string.Format("ui9_text_combat_skill_outline_{0}_cn", index));
			}
			this.outlinePageToggleGroup.Init(-1);
			this.outlinePageToggleGroup.OnActiveIndexChange += this.OnOutlinePageToggleChange;
			List<CToggle> pageTogList = this.otherPageToggleGroup.GetAll();
			byte index2 = 0;
			while ((int)index2 < pageTogList.Count)
			{
				CToggle tog2 = pageTogList[(int)index2];
				PracticeSlice slice2 = tog2.GetComponent<PracticeSlice>();
				bool isDirect = CombatSkillPanel.CheckIsDirectByToggleIndex((int)index2);
				byte actualIndex = (byte)CombatSkillPanel.GetActualIndex((int)index2);
				bool flag = !isDirect;
				if (flag)
				{
					this._reverseOtherPageDict[(int)index2] = actualIndex;
					this._reverseOtherPageDict[(int)actualIndex] = index2;
				}
				slice2.AutoRotateNameBg();
				slice2.RefreshStyle();
				slice2.SetNum(CombatSkillPanel.GetPageId((int)index2).ToString().SetColor(isDirect ? "81ddff" : "ffb7b7"));
				slice2.SetNameLabel(isDirect ? string.Format("ui9_text_combat_skill_direct_{0}_cn", actualIndex) : string.Format("ui9_text_combat_skill_reverse_{0}_cn", actualIndex));
				index2 += 1;
			}
			this.otherPageToggleGroup.Init();
			this.otherPageToggleGroup.OnActiveIndexChange += this.OnOtherPageToggleChange;
		}

		// Token: 0x0600B6E5 RID: 46821 RVA: 0x00535902 File Offset: 0x00533B02
		private void InitMastered()
		{
			this.menuPracticeMastered.Init(new Action(this.OnMasteredClick));
		}

		// Token: 0x0600B6E6 RID: 46822 RVA: 0x0053591D File Offset: 0x00533B1D
		private void InitJump()
		{
			this.menuPracticeJump.Init(new Action<short>(this.OnJumpSet));
		}

		// Token: 0x0600B6E7 RID: 46823 RVA: 0x00535938 File Offset: 0x00533B38
		private void InitInnerRatio()
		{
			this.innerRatioSlider.onValueChanged.RemoveAllListeners();
			this.innerRatioSlider.onValueChanged.AddListener(new UnityAction<float>(this.OnInnerRatioSliderChange));
			PointerTrigger pointerTrigger = this.innerRatioSlider.GetComponent<PointerTrigger>();
			pointerTrigger.EnterEvent.RemoveAllListeners();
			pointerTrigger.EnterEvent.AddListener(delegate()
			{
				this.innerRatioHover.SetActive(true);
			});
			pointerTrigger.ExitEvent.RemoveAllListeners();
			pointerTrigger.ExitEvent.AddListener(delegate()
			{
				this.innerRatioHover.SetActive(false);
			});
			this._innerRatioTips[0] = LanguageKey.LK_Skill_InnerOuterRatio.Tr();
			this.innerRatioTips.PresetParam = this._innerRatioTips;
		}

		// Token: 0x0600B6E8 RID: 46824 RVA: 0x005359EB File Offset: 0x00533BEB
		private void OnPresetChange(int togNew, int togOld)
		{
			TaiwuDomainMethod.Call.ChangeCombatSkillBreakPlate(this.SkillTemplateId, (sbyte)togNew);
			Action updateBackendData = this._updateBackendData;
			if (updateBackendData != null)
			{
				updateBackendData();
			}
		}

		// Token: 0x0600B6E9 RID: 46825 RVA: 0x00535A0E File Offset: 0x00533C0E
		private void OnConfirmLuohan(sbyte templateId)
		{
			TaiwuDomainMethod.Call.SetLuohanBreak(this.SkillTemplateId, templateId, 2);
			Action updateBackendData = this._updateBackendData;
			if (updateBackendData != null)
			{
				updateBackendData();
			}
		}

		// Token: 0x0600B6EA RID: 46826 RVA: 0x00535A34 File Offset: 0x00533C34
		private void OnOutlinePageToggleChange(int togNew, int togOld)
		{
			bool flag = togNew >= 0;
			if (flag)
			{
				PracticeSlice slice = this.outlinePageToggleGroup.Get(togNew).GetComponent<PracticeSlice>();
				bool flag2 = slice;
				if (flag2)
				{
					slice.SetSelected(true);
				}
			}
			bool flag3 = togOld >= 0;
			if (flag3)
			{
				PracticeSlice sliceOld = this.outlinePageToggleGroup.Get(togOld).GetComponent<PracticeSlice>();
				bool flag4 = sliceOld;
				if (flag4)
				{
					sliceOld.SetSelected(false);
				}
			}
			Action<CombatSkillDisplayData> updateFrontendData = this._updateFrontendData;
			if (updateFrontendData != null)
			{
				updateFrontendData(this._skillData.CombatSkillDisplayData);
			}
		}

		// Token: 0x0600B6EB RID: 46827 RVA: 0x00535AC8 File Offset: 0x00533CC8
		private void OnOtherPageToggleChange(int togNew, int togOld)
		{
			int index = (togNew >= 0) ? togNew : togOld;
			bool flag = index < 0;
			if (!flag)
			{
				bool isOn = this.otherPageToggleGroup.Get(index).isOn;
				byte pageId = CombatSkillPanel.GetPageId(index);
				sbyte direction = CombatSkillPanel.GetDirection(index);
				this.otherPageToggleGroup.Get(index).GetComponent<PracticeSlice>().SetSelected(isOn);
				bool flag2 = isOn;
				if (flag2)
				{
					this.otherPageToggleGroup.DeSelectWithoutNotify((int)this._reverseOtherPageDict[index]);
					this.otherPageToggleGroup.Get((int)this._reverseOtherPageDict[index]).GetComponent<PracticeSlice>().SetSelected(false);
					CombatSkillDomainMethod.Call.SetActivePage(-1, this.CharId, this.SkillTemplateId, pageId, direction);
				}
				else
				{
					bool flag3 = !this.IsBroken;
					if (flag3)
					{
						CombatSkillDomainMethod.Call.DeActivePage(-1, this.CharId, this.SkillTemplateId, pageId, direction);
					}
				}
				ArgumentBox args = new ArgumentBox();
				args.Set("CharId", this.CharId);
				args.Set("SkillId", this.SkillTemplateId);
				GEvent.OnEvent(UiEvents.OnCombatSkillMasteryChanged, args);
				Action updateBackendData = this._updateBackendData;
				if (updateBackendData != null)
				{
					updateBackendData();
				}
			}
		}

		// Token: 0x0600B6EC RID: 46828 RVA: 0x00535BF8 File Offset: 0x00533DF8
		private void OnMasteredClick()
		{
			bool mastered = this._skillData.CombatSkillDisplayData.Mastered;
			if (mastered)
			{
				ExtraDomainMethod.Call.RemoveCharacterMasteredCombatSkill(-1, this.CharId, this.SkillTemplateId);
			}
			else
			{
				ExtraDomainMethod.Call.AddCharacterMasteredCombatSkill(-1, this.CharId, this.SkillTemplateId);
			}
			Action updateBackendData = this._updateBackendData;
			if (updateBackendData != null)
			{
				updateBackendData();
			}
		}

		// Token: 0x0600B6ED RID: 46829 RVA: 0x00535C54 File Offset: 0x00533E54
		private void OnJumpSet(short value)
		{
			CombatDomainMethod.Call.SetJumpThreshold(-1, this.SkillTemplateId, value);
			Action updateBackendData = this._updateBackendData;
			if (updateBackendData != null)
			{
				updateBackendData();
			}
		}

		// Token: 0x0600B6EE RID: 46830 RVA: 0x00535C78 File Offset: 0x00533E78
		private void OnInnerRatioSliderChange(float value)
		{
			bool flag;
			if (!this._innerRatioLock)
			{
				CombatSkillPracticeDisplayData skillData = this._skillData;
				if (((skillData != null) ? skillData.CombatSkillDisplayData : null) != null)
				{
					flag = (this._charData == null);
					goto IL_29;
				}
			}
			flag = true;
			IL_29:
			bool flag2 = flag;
			if (!flag2)
			{
				sbyte expectRatio = (sbyte)(100f - value);
				this.btnOuterAdjust.interactable = (expectRatio < 100);
				this.btnInnerAdjust.interactable = (expectRatio > 0);
				GameDataBridge.AddDataModification<sbyte>(7, 0, (ulong)new CombatSkillKey(this.CharId, this.SkillTemplateId), 5U, expectRatio);
				sbyte currRatio = (sbyte)Mathf.Clamp((int)expectRatio, this._innerRatioRange.x, this._innerRatioRange.y);
				this._skillData.CombatSkillDisplayData.ExpectInnerRatio = expectRatio;
				this._skillData.CombatSkillDisplayData.CurrInnerRatio = currRatio;
				this.fill.fillAmount = Mathf.Clamp(value / this.innerRatioSlider.maxValue, 0f, 1f - this.rangeDown.fillAmount);
				bool flag3 = (int)expectRatio < this._innerRatioRange.x;
				if (flag3)
				{
					this.expect.GetComponent<RectTransform>().anchoredPosition = new Vector2(this.innerRatioSlider.GetComponent<RectTransform>().rect.width * (1f - this.rangeDown.fillAmount), 0f);
					this.expect.SetActive(true);
				}
				else
				{
					bool flag4 = (int)expectRatio > this._innerRatioRange.y;
					if (flag4)
					{
						this.expect.GetComponent<RectTransform>().anchoredPosition = new Vector2(this.innerRatioSlider.GetComponent<RectTransform>().rect.width * this.rangeUp.fillAmount, 0f);
						this.expect.SetActive(true);
					}
					else
					{
						this.expect.SetActive(false);
					}
				}
				this._innerRatioTips[1] = LanguageKey.LK_Skill_InnerOuterRatioTip.TrFormat(((int)(100 - this._skillData.CombatSkillDisplayData.CurrInnerRatio)).ToString() ?? "", this._skillData.CombatSkillDisplayData.CurrInnerRatio.ToString() ?? "");
				this.innerRatioText.SetText(LanguageKey.LK_Skill_InnerRatio.TrFormat(string.Format("{0}", this._skillData.CombatSkillDisplayData.CurrInnerRatio)), true);
				this.outerRatioText.SetText(LanguageKey.LK_Skill_OuterRatio.TrFormat(string.Format("{0}", (int)(100 - this._skillData.CombatSkillDisplayData.CurrInnerRatio))), true);
				this.hoverRatioRateInnerText.SetText(expectRatio.ToString(), true);
				this.hoverRatioRateOuterText.SetText(((int)(100 - expectRatio)).ToString(), true);
				this.innerRatioTips.Refresh(false, -1);
				Action<CombatSkillDisplayData> updateFrontendData = this._updateFrontendData;
				if (updateFrontendData != null)
				{
					updateFrontendData(this._skillData.CombatSkillDisplayData);
				}
			}
		}

		// Token: 0x0600B6EF RID: 46831 RVA: 0x00535F70 File Offset: 0x00534170
		private void UpdateCenter()
		{
			this.centerCombatSkill.Set(this._skillData.CombatSkillDisplayData);
			CombatSkillItem config = CombatSkill.Instance[this._skillData.CombatSkillDisplayData.TemplateId];
			this.centerCombatSkillBack.SetSprite("ui9_icon_combat_skill_frame_circle_" + config.Grade.ToString(), false, null);
		}

		// Token: 0x0600B6F0 RID: 46832 RVA: 0x00535FD4 File Offset: 0x005341D4
		private void UpdatePreset()
		{
			sbyte index = this._skillData.CombatSkillDisplayData.BreakPlateIndex;
			bool showBreakPlatePreset = this.CurrCharIsTaiwu;
			this.presetToggleGroup.SetWithoutNotify((int)index);
			this.presetToggleGroup.Get(2).gameObject.SetActive(true);
			this.presetToggleGroup.gameObject.SetActive(showBreakPlatePreset && (this._canOperate == null || this._canOperate()));
			this.presetText.SetActive(showBreakPlatePreset && (this._canOperate == null || this._canOperate()));
		}

		// Token: 0x0600B6F1 RID: 46833 RVA: 0x00536074 File Offset: 0x00534274
		private void UpdateMastered()
		{
			this.menuPracticeMastered.Set(this._skillData.CombatSkillDisplayData);
		}

		// Token: 0x0600B6F2 RID: 46834 RVA: 0x0053608E File Offset: 0x0053428E
		private void UpdateJump()
		{
			this.menuPracticeJump.Set(this._skillData.CombatSkillDisplayData);
		}

		// Token: 0x0600B6F3 RID: 46835 RVA: 0x005360A8 File Offset: 0x005342A8
		private void UpdateLuohan()
		{
			bool flag = !this.CurrCharIsTaiwu;
			if (flag)
			{
				this.luohanBreak.gameObject.SetActive(false);
			}
			else
			{
				OrganizationDomainMethod.AsyncCall.GetSectFunctionStatus(null, 1, SectFunctionStatuses.SectFunctionStatusType.UpgradedInteractionUnlocked, delegate(int offset, RawDataPool dataPool)
				{
					bool flag2;
					if (this._charData != null)
					{
						CombatSkillPracticeDisplayData skillData = this._skillData;
						flag2 = (((skillData != null) ? skillData.CombatSkillDisplayData : null) == null);
					}
					else
					{
						flag2 = true;
					}
					bool flag3 = flag2;
					if (!flag3)
					{
						bool isOpen = false;
						Serializer.Deserialize(dataPool, offset, ref isOpen);
						bool flag4 = !isOpen;
						if (flag4)
						{
							this.luohanBreak.gameObject.SetActive(false);
							Action onReady = this._onReady;
							if (onReady != null)
							{
								onReady();
							}
						}
						else
						{
							this.luohanBreak.SetData(this._charData.LuohanAccessories);
							this.luohanBreak.Set(this._skillData.CombatSkillDisplayData.LuohanId);
							this.luohanBreak.gameObject.SetActive(true);
							this.UpdateLuohanInteractable();
							Action onReady2 = this._onReady;
							if (onReady2 != null)
							{
								onReady2();
							}
						}
					}
				});
			}
		}

		// Token: 0x0600B6F4 RID: 46836 RVA: 0x005360F0 File Offset: 0x005342F0
		private void UpdateBreakPanel()
		{
			this.discBack.texture = (this.IsLuohanBreak ? this.luohanDiscBack : this.normalDiscBack);
			ushort activateState = this.ActivationState;
			int activeOutline = -1;
			bool isBroken = this.IsBroken;
			if (isBroken)
			{
				byte i = 0;
				while ((int)i < this.outlinePageToggleGroup.Count())
				{
					bool flag = CombatSkillStateHelper.IsPageActive(activateState, i);
					if (flag)
					{
						activeOutline = (int)i;
					}
					i += 1;
				}
			}
			else
			{
				GameData.Domains.Taiwu.SkillBreakPlate plate = this.Plate;
				bool flag2 = plate != null && !plate.Failed;
				if (flag2)
				{
					activeOutline = (int)this.Plate.OutlineType;
					activateState = this.Plate.SelectedPages;
				}
			}
			bool flag3 = activeOutline >= 0;
			if (flag3)
			{
				this.outlinePageToggleGroup.Set(activeOutline, false);
			}
			else
			{
				bool flag4 = this.IsBroken || !this._keepOutline;
				if (flag4)
				{
					this.outlinePageToggleGroup.DeSelect(false);
				}
			}
			for (int j = 0; j < this.otherPageToggleGroup.Count(); j++)
			{
				byte index = CombatSkillStateHelper.GetPageInternalIndex(-1, CombatSkillPanel.GetDirection(j), CombatSkillPanel.GetPageId(j));
				bool flag5 = CombatSkillStateHelper.IsPageActive(activateState, index);
				if (flag5)
				{
					this.otherPageToggleGroup.SelectWithoutNotify(j);
					this.otherPageToggleGroup.Get(j).GetComponent<PracticeSlice>().SetSelected(true);
				}
				else
				{
					this.otherPageToggleGroup.DeSelectWithoutNotify(j);
					this.otherPageToggleGroup.Get(j).GetComponent<PracticeSlice>().SetSelected(false);
				}
			}
			this.RefreshOutlinePages();
			this.RefreshOtherPages();
		}

		// Token: 0x0600B6F5 RID: 46837 RVA: 0x00536294 File Offset: 0x00534494
		private void UpdateInnerRatio()
		{
			this._innerRatioLock = true;
			sbyte baseRatio = this._skillData.CombatSkillDisplayData.BaseInnerRatio;
			int changeRange = (int)((short)this._skillData.CombatSkillDisplayData.InnerRatioChangeRange * this._charData.InnerRatio / 100);
			this._innerRatioRange.x = Math.Max((int)baseRatio - changeRange, 0);
			this._innerRatioRange.y = Math.Min((int)baseRatio + changeRange, 100);
			this.rangeUp.fillAmount = (float)(100 - this._innerRatioRange.y) / 100f;
			this.rangeDown.fillAmount = (float)this._innerRatioRange.x / 100f;
			this.innerRatioSlider.value = (float)(100 - this._skillData.CombatSkillDisplayData.ExpectInnerRatio);
			this._innerRatioLock = false;
			this.OnInnerRatioSliderChange(this.innerRatioSlider.value);
		}

		// Token: 0x0600B6F6 RID: 46838 RVA: 0x00536380 File Offset: 0x00534580
		private void UpdateInteractable()
		{
			bool canOperate = this._canOperate == null || this._canOperate();
			this.menuPracticeMastered.SetInteractable(canOperate);
			this.menuPracticeJump.SetInteractable(canOperate);
			this.innerRatioSlider.interactable = canOperate;
		}

		// Token: 0x0600B6F7 RID: 46839 RVA: 0x005363CC File Offset: 0x005345CC
		public void SetInnerRatioVisible(bool visible)
		{
			this.innerRatioSlider.gameObject.SetActive(visible);
			this.innerRatioTips.gameObject.SetActive(visible);
			this.btnOuterAdjust.gameObject.SetActive(visible);
			this.btnInnerAdjust.gameObject.SetActive(visible);
		}

		// Token: 0x0600B6F8 RID: 46840 RVA: 0x00536424 File Offset: 0x00534624
		private void RefreshOutlinePages()
		{
			bool canOperate = this._canOperate == null || this._canOperate();
			sbyte i = 0;
			while ((int)i < this.outlinePageToggleGroup.Count())
			{
				PracticeSlice outlinePageRefers = this.outlinePageToggleGroup.Get((int)i).GetComponent<PracticeSlice>();
				bool isPageRead = CombatSkillStateHelper.IsPageRead(this._skillData.CombatSkillDisplayData.ReadingState, CombatSkillStateHelper.GetOutlinePageInternalIndex(i));
				bool canClickOutline = this.CurrCharIsTaiwu && !this.IsBroken && !this.IsRevoked && isPageRead && (this.Plate == null || this.Plate.Failed);
				this.outlinePageToggleGroup.Get((int)i).interactable = (canOperate && canClickOutline);
				bool showPage = !this.IsRevoked && isPageRead;
				outlinePageRefers.SetNameByActive(showPage);
				outlinePageRefers.SetInteractable(canOperate && canClickOutline);
				outlinePageRefers.SetPageShow(showPage);
				TooltipInvoker tip = outlinePageRefers.GetPageTip();
				TooltipInvoker noPageTip = outlinePageRefers.GetNoPageTip();
				PracticeSkillPlatePageUtils.RefreshOutlinePageTip(tip, i, true, showPage);
				PracticeSkillPlatePageUtils.RefreshOutlinePageTip(noPageTip, i, true, showPage);
				i += 1;
			}
		}

		// Token: 0x0600B6F9 RID: 46841 RVA: 0x00536540 File Offset: 0x00534740
		private void RefreshOtherPages()
		{
			bool canOperate = this._canOperate == null || this._canOperate();
			for (byte i = 0; i < 5; i += 1)
			{
				byte directIndex = CombatSkillStateHelper.GetNormalPageInternalIndex(0, i + 1);
				byte reverseIndex = CombatSkillStateHelper.GetNormalPageInternalIndex(1, i + 1);
				bool isDirectRead = CombatSkillStateHelper.IsPageRead(this.ReadingState, directIndex);
				bool isReverseRead = CombatSkillStateHelper.IsPageRead(this.ReadingState, reverseIndex);
				bool canCancel = !this.IsBroken && this.Plate == null;
				GameData.Domains.Taiwu.SkillBreakPlate plate = this.Plate;
				bool isFailed = plate != null && plate.Failed;
				sbyte pageActiveDirection = CombatSkillStateHelper.GetPageActiveDirection(this.ActivationState, i + 1);
				bool canSwitchToDirect = pageActiveDirection == 1;
				bool canSwitchToReverse = pageActiveDirection == 0;
				bool showDirectPage = !this.IsRevoked && isDirectRead;
				bool showReversePage = !this.IsRevoked && isReverseRead;
				PracticeSlice directPageSlice = this.otherPageToggleGroup.Get((int)i).GetComponent<PracticeSlice>();
				PracticeSlice reversePageSlice = this.otherPageToggleGroup.Get((int)this._reverseOtherPageDict[(int)i]).GetComponent<PracticeSlice>();
				TooltipInvoker directTip = directPageSlice.GetPageTip();
				TooltipInvoker directTipNoPage = directPageSlice.GetNoPageTip();
				TooltipInvoker reverseTip = reversePageSlice.GetPageTip();
				TooltipInvoker reverseTipNoPage = reversePageSlice.GetNoPageTip();
				directPageSlice.SetPageShow(showDirectPage);
				directPageSlice.SetNameByActive(showDirectPage);
				reversePageSlice.SetPageShow(showReversePage);
				reversePageSlice.SetNameByActive(showReversePage);
				directPageSlice.SetInteractable(canOperate && this.CurrCharIsTaiwu && !this.IsRevoked && isDirectRead && (canCancel || canSwitchToDirect || isFailed));
				reversePageSlice.SetInteractable(canOperate && this.CurrCharIsTaiwu && !this.IsRevoked && isReverseRead && (canCancel || canSwitchToReverse || isFailed));
				this.RefreshOtherPageTips((int)i, directTip, this.SkillTemplateId, true, true, showDirectPage, true, this.otherPageToggleGroup);
				this.RefreshOtherPageTips((int)i, directTipNoPage, this.SkillTemplateId, true, true, showDirectPage, true, this.otherPageToggleGroup);
				this.RefreshOtherPageTips((int)i, reverseTip, this.SkillTemplateId, false, true, showReversePage, true, this.otherPageToggleGroup);
				this.RefreshOtherPageTips((int)i, reverseTipNoPage, this.SkillTemplateId, false, true, showReversePage, true, this.otherPageToggleGroup);
			}
		}

		// Token: 0x0600B6FA RID: 46842 RVA: 0x00536764 File Offset: 0x00534964
		private void RefreshOtherPageTips(int index, TooltipInvoker tip, short skillId, bool isDirect, bool needNotReadLine = false, bool isRead = false, bool needNotActiveLine = false, CToggleGroupMultiSelect toggleGroup = null)
		{
			tip.Type = TipType.GeneralLines;
			if (tip.RuntimeParam == null)
			{
				tip.RuntimeParam = new ArgumentBox();
			}
			CombatSkillPanel.<>c__DisplayClass94_0 CS$<>8__locals1;
			CS$<>8__locals1.tipParam = tip.RuntimeParam;
			CS$<>8__locals1.lineCount = 0;
			string otherPageName = LocalStringManager.Get(string.Format("LK_CombatSkill_{0}_Page_{1}", isDirect ? "Direct" : "Reverse", index)).SetColor(isDirect ? "81ddff" : "ffb7b7");
			string titleName = LocalStringManager.GetFormat(LanguageKey.LK_Skill_Break_Tip_OtherPage_Title, otherPageName);
			CS$<>8__locals1.tipParam.Set("Title", titleName);
			GeneralLineData effectTitle = new GeneralLineData
			{
				Type = 1,
				Args = new List<string>
				{
					LocalStringManager.Get(LanguageKey.LK_Skill_Break_Tip_OtherPage_EffectTitle)
				}
			};
			CombatSkillPanel.<RefreshOtherPageTips>g__AddNode|94_0(effectTitle, ref CS$<>8__locals1);
			int pageIndex = isDirect ? (index + 5) : (index + 5 + 5);
			List<string> descList = EasyPool.Get<List<string>>();
			PageEffectHelper.GenerateNormalPageEffects(skillId, pageIndex - 5, descList);
			foreach (string desc in descList)
			{
				GeneralLineData effectLine = new GeneralLineData
				{
					Type = 5,
					Args = new List<string>
					{
						desc
					},
					ExtraArgs = new List<object>
					{
						24
					}
				};
				CombatSkillPanel.<RefreshOtherPageTips>g__AddNode|94_0(effectLine, ref CS$<>8__locals1);
			}
			bool flag = needNotReadLine || needNotActiveLine;
			if (flag)
			{
				GeneralLineData space = new GeneralLineData
				{
					Type = 4,
					PreferredHeight = 10f
				};
				CombatSkillPanel.<RefreshOtherPageTips>g__AddNode|94_0(space, ref CS$<>8__locals1);
			}
			if (needNotReadLine)
			{
				bool flag2 = !isRead;
				if (flag2)
				{
					GeneralLineData notReadLine = new GeneralLineData
					{
						Type = 3,
						Args = new List<string>
						{
							LocalStringManager.Get(LanguageKey.LK_Skill_Break_Tip_OtherPage_Desc_NotRead)
						}
					};
					CombatSkillPanel.<RefreshOtherPageTips>g__AddNode|94_0(notReadLine, ref CS$<>8__locals1);
				}
			}
			if (needNotActiveLine)
			{
				int toggleIndex = index + (isDirect ? 0 : 5);
				bool isActivated = toggleGroup.Get(toggleIndex).isOn;
				bool flag3 = !isActivated;
				if (flag3)
				{
					GeneralLineData notActiveLine = new GeneralLineData
					{
						Type = 3,
						Args = new List<string>
						{
							LocalStringManager.Get(LanguageKey.LK_Skill_Break_Tip_OtherPage_Desc_NotActive)
						}
					};
					CombatSkillPanel.<RefreshOtherPageTips>g__AddNode|94_0(notActiveLine, ref CS$<>8__locals1);
				}
			}
			CS$<>8__locals1.tipParam.Set("LineCount", CS$<>8__locals1.lineCount);
			CS$<>8__locals1.tipParam.Set("DisableRaycastTarget", true);
			tip.Refresh(false, -1);
			EasyPool.Free<List<string>>(descList);
		}

		// Token: 0x0600B6FB RID: 46843 RVA: 0x00536A00 File Offset: 0x00534C00
		private static bool CheckIsDirectByToggleIndex(int index)
		{
			return index < 5;
		}

		// Token: 0x0600B6FC RID: 46844 RVA: 0x00536A06 File Offset: 0x00534C06
		private static byte GetPageId(int index)
		{
			return (byte)(CombatSkillPanel.GetActualIndex(index) + 1);
		}

		// Token: 0x0600B6FD RID: 46845 RVA: 0x00536A11 File Offset: 0x00534C11
		private static int GetActualIndex(int index)
		{
			return (int)((byte)(CombatSkillPanel.CheckIsDirectByToggleIndex(index) ? index : (index - 5)));
		}

		// Token: 0x0600B6FE RID: 46846 RVA: 0x00536A22 File Offset: 0x00534C22
		public static sbyte GetDirection(int index)
		{
			return CombatSkillPanel.CheckIsDirectByToggleIndex(index) ? 0 : 1;
		}

		// Token: 0x0600B6FF RID: 46847 RVA: 0x00536A30 File Offset: 0x00534C30
		public void CloseJumpPanel()
		{
			this.menuPracticeJump.QuickHide();
		}

		// Token: 0x0600B706 RID: 46854 RVA: 0x00536BEC File Offset: 0x00534DEC
		[CompilerGenerated]
		internal static void <RefreshOtherPageTips>g__AddNode|94_0(GeneralLineData lineData, ref CombatSkillPanel.<>c__DisplayClass94_0 A_1)
		{
			ArgumentBox tipParam = A_1.tipParam;
			string format = "LineData{0}";
			int num = A_1.lineCount + 1;
			A_1.lineCount = num;
			tipParam.SetObject(string.Format(format, num), lineData);
		}

		// Token: 0x04008DFB RID: 36347
		public RectTransform jumpPanel;

		// Token: 0x04008DFC RID: 36348
		public GameObject presetText;

		// Token: 0x04008DFD RID: 36349
		public CToggleGroup presetToggleGroup;

		// Token: 0x04008DFE RID: 36350
		public ShaolinLuohanBreak luohanBreak;

		// Token: 0x04008DFF RID: 36351
		public CToggleGroup outlinePageToggleGroup;

		// Token: 0x04008E00 RID: 36352
		public CToggleGroupMultiSelect otherPageToggleGroup;

		// Token: 0x04008E01 RID: 36353
		public CharacterMenuCombatSkillItem centerCombatSkill;

		// Token: 0x04008E02 RID: 36354
		public CImage centerCombatSkillBack;

		// Token: 0x04008E03 RID: 36355
		public CRawImage discBack;

		// Token: 0x04008E04 RID: 36356
		public MenuPracticeMastered menuPracticeMastered;

		// Token: 0x04008E05 RID: 36357
		public MenuPracticeJump menuPracticeJump;

		// Token: 0x04008E06 RID: 36358
		public CSlider innerRatioSlider;

		// Token: 0x04008E07 RID: 36359
		public CImage fill;

		// Token: 0x04008E08 RID: 36360
		public CImage rangeUp;

		// Token: 0x04008E09 RID: 36361
		public CImage rangeDown;

		// Token: 0x04008E0A RID: 36362
		public GameObject expect;

		// Token: 0x04008E0B RID: 36363
		public TooltipInvoker innerRatioTips;

		// Token: 0x04008E0C RID: 36364
		public GameObject innerRatioHover;

		// Token: 0x04008E0D RID: 36365
		public TextMeshProUGUI innerRatioText;

		// Token: 0x04008E0E RID: 36366
		public TextMeshProUGUI outerRatioText;

		// Token: 0x04008E0F RID: 36367
		public TextMeshProUGUI hoverRatioRateInnerText;

		// Token: 0x04008E10 RID: 36368
		public TextMeshProUGUI hoverRatioRateOuterText;

		// Token: 0x04008E11 RID: 36369
		public CButton btnInnerAdjust;

		// Token: 0x04008E12 RID: 36370
		public CButton btnOuterAdjust;

		// Token: 0x04008E13 RID: 36371
		[Header("正常突破和佛像突破的盘子图片")]
		public Texture2D normalDiscBack;

		// Token: 0x04008E14 RID: 36372
		public Texture2D luohanDiscBack;

		// Token: 0x04008E15 RID: 36373
		private const string Direct = "81ddff";

		// Token: 0x04008E16 RID: 36374
		private const string Reverse = "ffb7b7";

		// Token: 0x04008E17 RID: 36375
		private Action _updateBackendData;

		// Token: 0x04008E18 RID: 36376
		private Action _onReady;

		// Token: 0x04008E19 RID: 36377
		private Action<CombatSkillDisplayData> _updateFrontendData;

		// Token: 0x04008E1A RID: 36378
		private Func<bool> _canOperate;

		// Token: 0x04008E1B RID: 36379
		private CombatSkillPracticeDisplayData _skillData;

		// Token: 0x04008E1C RID: 36380
		private CharacterDisplayDataForPractice _charData;

		// Token: 0x04008E1D RID: 36381
		private bool _keepOutline;

		// Token: 0x04008E1E RID: 36382
		private readonly Dictionary<int, byte> _reverseOtherPageDict = new Dictionary<int, byte>();

		// Token: 0x04008E1F RID: 36383
		private bool _innerRatioLock = true;

		// Token: 0x04008E20 RID: 36384
		private Vector2Int _innerRatioRange;

		// Token: 0x04008E21 RID: 36385
		private readonly string[] _innerRatioTips = new string[]
		{
			"",
			""
		};
	}
}
