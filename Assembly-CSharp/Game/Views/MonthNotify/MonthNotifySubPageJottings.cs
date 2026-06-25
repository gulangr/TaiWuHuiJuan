using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Character;
using Game.Components.Item;
using Game.Views.CharacterMenu;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Extra;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Taiwu;
using GameData.Domains.World.Notification;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.MonthNotify
{
	// Token: 0x020008C2 RID: 2242
	public class MonthNotifySubPageJottings : MonoBehaviour
	{
		// Token: 0x17000C9D RID: 3229
		// (get) Token: 0x06006ACD RID: 27341 RVA: 0x003150EC File Offset: 0x003132EC
		private static int TaiwuId
		{
			get
			{
				return SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			}
		}

		// Token: 0x06006ACE RID: 27342 RVA: 0x003150F8 File Offset: 0x003132F8
		private void OnEnable()
		{
			this._isLoopingOpened = false;
			this.btnLooping.enabled = false;
			SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(1f, delegate
			{
				this.btnLooping.enabled = true;
			});
		}

		// Token: 0x06006ACF RID: 27343 RVA: 0x0031512C File Offset: 0x0031332C
		public void Init(Action closeAction)
		{
			this.btnInjury.ClearAndAddListener(new Action(this.OnClickBtnInjury));
			this.btnHeal.ClearAndAddListener(new Action(this.OnClickBtnHeal));
			this.btnMedicine.ClearAndAddListener(new Action(this.OnClickBtnUsingMedicine));
			this.btnEat.ClearAndAddListener(new Action(this.OnClickBtnEating));
			this.btnLooping.ClearAndAddListener(new Action(this.OnClickBtnLooping));
			this.btnReading.ClearAndAddListener(new Action(this.OnClickBtnReading));
			this.btnNeili.ClearAndAddListener(new Action(this.OnClickBtnNeili));
			this.skillPointerTrigger.EnterEvent.ResetListener(delegate()
			{
				this.skillHover.SetActive(true);
			});
			this.skillPointerTrigger.ExitEvent.ResetListener(delegate()
			{
				this.skillHover.SetActive(false);
			});
			this.readPointerTrigger.EnterEvent.ResetListener(delegate()
			{
				this.readHover.SetActive(true);
			});
			this.readPointerTrigger.ExitEvent.ResetListener(delegate()
			{
				this.readHover.SetActive(false);
			});
			this.neiliTypePanel.Init();
			this.AssignJottingsStateTitles();
			this.btnClose.ClearAndAddListener(closeAction);
		}

		// Token: 0x06006AD0 RID: 27344 RVA: 0x00315278 File Offset: 0x00313478
		private void AssignJottingsStateTitles()
		{
			bool flag = this.outerInjuryTitle != null;
			if (flag)
			{
				this.outerInjuryTitle.text = LocalStringManager.Get(LanguageKey.LK_Out_Injury);
			}
			bool flag2 = this.innerInjuryTitle != null;
			if (flag2)
			{
				this.innerInjuryTitle.text = LocalStringManager.Get(LanguageKey.LK_Inner_Injury);
			}
			bool flag3 = this.posionTitle != null;
			if (flag3)
			{
				this.posionTitle.text = LocalStringManager.Get(LanguageKey.LK_Poison);
			}
			bool flag4 = this.happinessTitle != null;
			if (flag4)
			{
				this.happinessTitle.text = LocalStringManager.Get(LanguageKey.LK_Main_SummaryInfo_Happiness);
			}
			bool flag5 = this.healthTitle != null;
			if (flag5)
			{
				this.healthTitle.text = LocalStringManager.Get(LanguageKey.LK_Health);
			}
			bool flag6 = this.disorderTitle != null;
			if (flag6)
			{
				this.disorderTitle.text = LocalStringManager.Get(LanguageKey.LK_Qi_Disorder);
			}
		}

		// Token: 0x06006AD1 RID: 27345 RVA: 0x00315370 File Offset: 0x00313570
		public void Set(MonthNotify data, Dictionary<ItemKey, ItemDisplayData> eatenItem)
		{
			this._data = data;
			this._eatenItem = eatenItem;
			this.Refresh();
			CharacterDomainMethod.AsyncCall.GetCharacterInjuryDisplayData(null, MonthNotifySubPageJottings.TaiwuId, delegate(int offset, RawDataPool pool)
			{
				CharacterInjuryDisplayData characterInjuryDisplayData = null;
				Serializer.Deserialize(pool, offset, ref characterInjuryDisplayData);
				this.btnMedicine.interactable = (characterInjuryDisplayData.InventoryMedicineItemCount > 0);
			});
			CharacterDomainMethod.AsyncCall.GetSomeoneKidnapCharacters(null, MonthNotifySubPageJottings.TaiwuId, delegate(int offset, RawDataPool dataPool)
			{
				Serializer.Deserialize(dataPool, offset, ref this._kidnappedCharacterList);
			});
			TaiwuDomainMethod.AsyncCall.GetLoopingViewDisplayData(null, delegate(int offset, RawDataPool dataPool)
			{
				Serializer.Deserialize(dataPool, offset, ref this._loopingViewDisplayData);
			});
		}

		// Token: 0x06006AD2 RID: 27346 RVA: 0x003153D6 File Offset: 0x003135D6
		private void Refresh()
		{
			this.RefreshState();
			this.RefreshSkill();
			this.RefreshNeiliType();
			this.RefreshRead();
		}

		// Token: 0x06006AD3 RID: 27347 RVA: 0x003153F8 File Offset: 0x003135F8
		private unsafe void RefreshState()
		{
			ValueTuple<sbyte, sbyte> bothSum = this._data.PrevInjuries.GetBothSum();
			sbyte prevOuter = bothSum.Item1;
			sbyte prevInner = bothSum.Item2;
			ValueTuple<sbyte, sbyte> bothSum2 = this._data.CurrInjuries.GetBothSum();
			sbyte currOuter = bothSum2.Item1;
			sbyte currInner = bothSum2.Item2;
			int prevPoisonValue = this._data.PrevPoison.Sum();
			int currPoisonValue = this._data.CurrPoison.Sum();
			sbyte currHappinessType = HappinessType.GetHappinessType(this._data.CurrHappiness);
			EHealthType prevHealthType = CommonUtils.GetHealthType(this._data.PrevHealth, this._data.PrevMaxLeftHealth, -1);
			EHealthType currHealthType = CommonUtils.GetHealthType(this._data.CurrHealth, this._data.CurrMaxLeftHealth, -1);
			sbyte currQiLevel = DisorderLevelOfQi.GetDisorderLevelOfQi(this._data.CurrQiDisorder);
			int qiType = (currQiLevel < 2) ? 0 : 1;
			this.prevOuterInjury.text = prevOuter.ToString();
			this.currOuterInjury.text = this.GetNumberString((int)prevOuter, (int)currOuter);
			this.prevInnerInjury.text = prevInner.ToString();
			this.currInnerInjury.text = this.GetNumberString((int)prevInner, (int)currInner);
			this.prevPoison.text = prevPoisonValue.ToString();
			this.currPoison.text = this.GetNumberString(prevPoisonValue, currPoisonValue);
			this.happinessIcon.SetSprite(string.Format("{0}{1}", "ui9_icon_happiness_big_", currHappinessType), false, null);
			this.prevHappiness.text = CommonUtils.GetHappinessString(HappinessType.GetHappinessType(this._data.PrevHappiness));
			this.currHappiness.text = CommonUtils.GetHappinessString(currHappinessType) + this.GetPercentString((int)this._data.PrevHappiness, (int)this._data.CurrHappiness, 119, false);
			this.healthIcon.SetSprite(CommonUtils.GetHealthIcon(currHealthType), false, null);
			this.prevHealth.text = CommonUtils.GetHealthString(prevHealthType);
			this.currHealth.text = CommonUtils.GetHealthString(currHealthType) + this.GetPercentString((int)this._data.PrevHealth, (int)this._data.CurrHealth, (int)this._data.CurrMaxLeftHealth, false);
			this.qiDisorderIcon.SetSprite(string.Format("{0}{1}", "ui9_icon_qi_disorder_", qiType), false, null);
			this.prevQiDisorder.text = CommonUtils.GetQiDisorderString(DisorderLevelOfQi.GetDisorderLevelOfQi(this._data.PrevQiDisorder));
			this.currQiDisorder.text = CommonUtils.GetQiDisorderString(currQiLevel) + this.GetPercentString((int)this._data.PrevQiDisorder, (int)this._data.CurrQiDisorder, (int)DisorderLevelOfQi.MaxValue, true);
			for (int i = 0; i < this.mainAttributes.childCount; i++)
			{
				this.mainAttributes.GetChild(i).GetChild(2).GetComponent<TextMeshProUGUI>().text = this.GetNumberString((int)(*this._data.PrevAttributes[i]), (int)(*this._data.CurrAttributes[i]), (int)(*this._data.CurrMaxAttributes[i]));
			}
			for (int j = 0; j < this.injuryEatItems.Length; j++)
			{
				ItemKey itemKey = this._data.Eaten.GetItem(j);
				ItemDisplayData item;
				this.injuryEatItems[j].Set(MonthNotifySubPageJottings.TaiwuId, true, (itemKey.IsValid() && this._eatenItem.TryGetValue(itemKey, out item)) ? item : null, this._data.Eaten.GetDuration(j), false);
			}
			bool flag = this._data.CurrMaxNeili == 0;
			if (flag)
			{
				this.neiliLabel.text = "0/0";
				this.neiliProgress.fillAmount = 0f;
			}
			else
			{
				this.neiliLabel.text = this.GetNumberString(this._data.PrevNeili, this._data.CurrNeili, this._data.CurrMaxNeili);
				this.neiliProgress.fillAmount = (float)this._data.CurrNeili / (float)this._data.CurrMaxNeili;
			}
		}

		// Token: 0x06006AD4 RID: 27348 RVA: 0x00315834 File Offset: 0x00313A34
		private void RefreshSkill()
		{
			bool flag = this._data.LoopingCombatSkillTemplateId >= 0;
			if (flag)
			{
				CombatSkillItem config = CombatSkill.Instance[this._data.LoopingCombatSkillTemplateId];
				this.skillLabel.text = config.Name.SetColor(Colors.Instance.GradeColors[(int)config.Grade]);
				this.skillIcon.SetSprite(config.Icon, false, null);
				this.skillIcon.SetColor(Colors.Instance.FiveElementsColors[(int)config.FiveElements]);
				this.skillIcon.gameObject.SetActive(true);
				this.skillTips.Type = TipType.CombatSkill;
				this.skillTips.NeedRefresh = true;
				TooltipInvoker tooltipInvoker = this.skillTips;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = new ArgumentBox();
				}
				this.skillTips.RuntimeParam.Set("CombatSkillId", this._data.LoopingCombatSkillTemplateId);
				this.skillTips.RuntimeParam.Set("CharId", MonthNotifySubPageJottings.TaiwuId);
				this.loopFinished.SetActive(this._data.LoopingObtainedNeili >= this._data.LoopingTotalObtainableNeili);
				this.loopEventParticle.gameObject.SetActive(this._data.LoopingEvent);
				this.neiliObtainLabel.text = this._data.LoopingObtainedNeili.ToString().SetColor("brightblue") + "/" + this._data.LoopingTotalObtainableNeili.ToString();
				this.neiliObtainBack.SetActive(true);
			}
			else
			{
				this.skillLabel.text = "";
				this.skillIcon.gameObject.SetActive(false);
				this.neiliObtainBack.SetActive(false);
				this.skillTips.Type = TipType.Simple;
				TooltipInvoker tooltipInvoker = this.skillTips;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = new ArgumentBox();
				}
				this.skillTips.RuntimeParam.Set("arg0", LocalStringManager.Get(LanguageKey.LK_Neigong_Looping));
				this.skillTips.RuntimeParam.Set("arg1", LocalStringManager.Get(LanguageKey.LK_Neigong_Looping_Tip_None));
				this.loopFinished.SetActive(false);
				this.loopEventParticle.gameObject.SetActive(false);
			}
			this.skillFill.fillAmount = ((this._data.ActiveLoopingProgress < GlobalConfig.Instance.MaxActiveNeigongLoopingProgress) ? ((float)(this._data.ActiveLoopingProgress % 10) * 0.1f) : 1f);
			for (int i = 0; i < this.neiliAllocations.childCount; i++)
			{
				this.neiliAllocations.GetChild(i).GetComponent<MonthNotifyNeiliAllocationType>().Set(this._data.PrevExtraNeiliAllocationProgress[i], this._data.CurrExtraNeiliAllocationProgress[i]);
			}
		}

		// Token: 0x06006AD5 RID: 27349 RVA: 0x00315B38 File Offset: 0x00313D38
		private unsafe void RefreshNeiliType()
		{
			this.neiliTypePanel.Set(this._data.CurrNeiliType);
			this.neiliTypePanel.Set(this._data.CurrNeiliPercent, this._data.NeiliTransferType, this._data.NeiliDstType, this._data.NeiliTransferAmount);
			int maxDeltaType = -1;
			int maxDeltaValue = 0;
			int minDeltaType = -1;
			int minDeltaValue = int.MaxValue;
			for (int i = 0; i < 5; i++)
			{
				int val = (int)(*this._data.CurrNeiliPercent[i] - *this._data.PrevNeiliPercent[i]);
				bool flag = val > maxDeltaValue;
				if (flag)
				{
					maxDeltaType = i;
					maxDeltaValue = val;
				}
				bool flag2 = val < minDeltaValue;
				if (flag2)
				{
					minDeltaType = i;
					minDeltaValue = val;
				}
			}
			bool flag3 = maxDeltaType < 0;
			if (flag3)
			{
				this.neiliPercentBack.SetActive(false);
			}
			else
			{
				this.prevNeiliPercentTitle.text = LocalStringManager.Get(string.Format("LK_FiveElements_Type_{0}", minDeltaType));
				this.prevNeiliPercentValue.text = this.GetPercentString((int)(*this._data.PrevNeiliPercent[minDeltaType]), (int)(*this._data.CurrNeiliPercent[minDeltaType]), 100, false);
				this.currNeiliPercentTitle.text = LocalStringManager.Get(string.Format("LK_FiveElements_Type_{0}", maxDeltaType));
				this.currNeiliPercentValue.text = this.GetPercentString((int)(*this._data.PrevNeiliPercent[maxDeltaType]), (int)(*this._data.CurrNeiliPercent[maxDeltaType]), 100, false);
				this.neiliPercentBack.SetActive(true);
			}
			NeiliTypeItem prevConfig = NeiliType.Instance[this._data.PrevNeiliType];
			this.neiliTypePrevIcon.SetSprite(string.Format("{0}{1}", "ui9_icon_five_elements_", prevConfig.FiveElements), false, null);
			this.neiliTypePrevLabel.text = prevConfig.Name.SetColor(CommonUtils.GetFiveElementsColor((sbyte)prevConfig.FiveElements));
			bool flag4 = this._data.PrevNeiliType == this._data.CurrNeiliType;
			if (flag4)
			{
				this.neiliTypeArrow.SetActive(false);
				this.neiliTypeCurrIcon.gameObject.SetActive(false);
				this.neiliTypeCurrLabel.gameObject.SetActive(false);
			}
			else
			{
				NeiliTypeItem currConfig = NeiliType.Instance[this._data.CurrNeiliType];
				this.neiliTypeCurrIcon.SetSprite(string.Format("{0}{1}", "ui9_icon_five_elements_", currConfig.FiveElements), false, null);
				this.neiliTypeCurrLabel.text = currConfig.Name.SetColor(CommonUtils.GetFiveElementsColor((sbyte)currConfig.FiveElements));
				this.neiliTypeArrow.SetActive(true);
				this.neiliTypeCurrIcon.gameObject.SetActive(true);
				this.neiliTypeCurrLabel.gameObject.SetActive(true);
			}
		}

		// Token: 0x06006AD6 RID: 27350 RVA: 0x00315E3C File Offset: 0x0031403C
		private void RefreshRead()
		{
			TooltipInvoker tooltipInvoker = this.readTips;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
			}
			bool flag = this._data.ReadingBook.IsValid();
			if (flag)
			{
				SkillBookItem config = SkillBook.Instance[this._data.ReadingBook.TemplateId];
				short temp;
				int prevDurability = (int)(this._data.BookPrevDurability.TryGetValue(this._data.ReadingBook, out temp) ? temp : 0);
				int currDurability = (int)(this._data.BookCurrDurability.TryGetValue(this._data.ReadingBook, out temp) ? temp : 0);
				int maxDurability = (int)(this._data.BookMaxDurability.TryGetValue(this._data.ReadingBook, out temp) ? temp : 0);
				this.readLabel.text = config.Name.SetColor(Colors.Instance.GradeColors[(int)config.Grade]);
				this.readIcon.gameObject.SetActive(true);
				this.readIcon.Set(new ItemDisplayData(this._data.ReadingBook.ItemType, this._data.ReadingBook.TemplateId), false);
				this.readTips.Type = TipType.ReadingBook;
				this.readTips.RuntimeParam.SetObject("currentReadingBookKey", this._data.ReadingBook);
				this.readTips.RuntimeParam.SetObject("referenceBooks", this._data.ReferenceBooks.ToArray());
				this.readFinished.SetActive(this._data.ReadingProgress >= 100);
				this.readEventParticle.gameObject.SetActive(this._data.ReadingEvent);
				this.durabilityLabel.text = (this.GetNumberString(prevDurability, currDurability, maxDurability) ?? "");
				this.durabilityBack.SetActive(true);
				bool flag2 = config.ItemSubType == 1001;
				if (flag2)
				{
					for (int i = 0; i < 6; i++)
					{
						MonthNotifyBookPageItem item = this.pages.GetChild(i).GetComponent<MonthNotifyBookPageItem>();
						sbyte direction = (i == 0) ? -1 : this._data.ReadingBookType[i];
						sbyte type = (i == 0) ? this._data.ReadingBookType[i] : ((sbyte)(i - 1));
						item.Set(i, direction, type, (int)this._data.PrevReadingProgress[i], (int)this._data.CurrReadingProgress[i]);
						item.gameObject.SetActive(true);
					}
				}
				else
				{
					for (int j = 0; j < 5; j++)
					{
						MonthNotifyBookPageItem item2 = this.pages.GetChild(j).GetComponent<MonthNotifyBookPageItem>();
						item2.Set(j, (int)this._data.PrevReadingProgress[j], (int)this._data.CurrReadingProgress[j]);
						item2.gameObject.SetActive(true);
					}
					this.pages.GetChild(5).gameObject.SetActive(false);
				}
				this.noReadingBook.SetActive(false);
			}
			else
			{
				this.readLabel.text = "";
				this.readIcon.gameObject.SetActive(false);
				this.readTips.Type = TipType.Simple;
				this.readTips.RuntimeParam.Set("arg0", LocalStringManager.Get(LanguageKey.LK_Reading_Title));
				this.readTips.RuntimeParam.Set("arg1", LocalStringManager.Get(LanguageKey.LK_Read_Tip_None));
				this.readFinished.SetActive(false);
				this.readEventParticle.gameObject.SetActive(false);
				this.durabilityBack.SetActive(false);
				for (int k = 0; k < this.pages.childCount; k++)
				{
					this.pages.GetChild(k).gameObject.SetActive(false);
				}
				this.noReadingBook.SetActive(true);
			}
			this.readFill.fillAmount = ((this._data.ActiveReadingProgress < GlobalConfig.Instance.MaxActiveReadingProgress) ? ((float)(this._data.ActiveReadingProgress % 10) * 0.1f) : 1f);
			int bookCount = 0;
			for (int l = 0; l < this.referenceBooks.childCount; l++)
			{
				ItemKey itemKey = this._data.ReferenceBooks[l];
				Transform obj = this.referenceBooks.GetChild(l);
				bool flag3 = !itemKey.IsValid();
				if (flag3)
				{
					obj.gameObject.SetActive(false);
				}
				else
				{
					SkillBookItem config2 = SkillBook.Instance[itemKey.TemplateId];
					short temp2;
					int prevDurability2 = (int)(this._data.BookPrevDurability.TryGetValue(itemKey, out temp2) ? temp2 : 0);
					int currDurability2 = (int)(this._data.BookCurrDurability.TryGetValue(itemKey, out temp2) ? temp2 : 0);
					int maxDurability2 = (int)(this._data.BookMaxDurability.TryGetValue(itemKey, out temp2) ? temp2 : 0);
					obj.GetChild(0).GetComponent<TextMeshProUGUI>().text = config2.Name.SetColor(Colors.Instance.GradeColors[(int)config2.Grade]);
					obj.GetChild(1).GetComponent<TextMeshProUGUI>().text = "+" + this._data.BookRefSpeed[itemKey].ToString() + "%";
					obj.GetChild(2).GetComponent<TextMeshProUGUI>().text = (this.GetNumberString(prevDurability2, currDurability2, maxDurability2) ?? "");
					TooltipInvoker tips = obj.GetComponent<TooltipInvoker>();
					tooltipInvoker = tips;
					if (tooltipInvoker.RuntimeParam == null)
					{
						tooltipInvoker.RuntimeParam = new ArgumentBox();
					}
					tips.RuntimeParam.Set<ItemDisplayData>("ItemData", new ItemDisplayData(itemKey, 1));
					tips.RuntimeParam.Set("ShowPageInfo", false);
					tips.RuntimeParam.Set("TemplateDataOnly", true);
					obj.gameObject.SetActive(true);
					bookCount++;
				}
			}
			this.noRefBook.SetActive(bookCount == 0);
		}

		// Token: 0x06006AD7 RID: 27351 RVA: 0x003164A0 File Offset: 0x003146A0
		private void OnClickBtnInjury()
		{
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.Clear();
			argBox.Set("CharacterId", MonthNotifySubPageJottings.TaiwuId);
			argBox.Set("CanOperate", true);
			UIElement.CharacterMenu.SetOnInitArgs(argBox);
			UIManager.Instance.StackToUI(UIElement.CharacterMenu);
			ArgumentBox args = new ArgumentBox();
			args.SetObject("TargetPageIndex", ECharacterSubToggleBase.CharacterBase);
			args.Set("BaseAttributeIndex", 1);
			GEvent.OnEvent(UiEvents.OnNeedOpenCharacterMenuSubPage, args);
		}

		// Token: 0x06006AD8 RID: 27352 RVA: 0x0031652C File Offset: 0x0031472C
		private void OnClickBtnHeal()
		{
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			HashSet<int> patientList = EasyPool.Get<HashSet<int>>();
			HashSet<int> doctorList = EasyPool.Get<HashSet<int>>();
			BasicGameData data = SingletonObject.getInstance<BasicGameData>();
			CharacterMonitorModel monitor = SingletonObject.getInstance<CharacterMonitorModel>();
			argBox.Clear();
			patientList.Clear();
			doctorList.Clear();
			patientList.Add(data.TaiwuCharId);
			patientList.UnionWith(monitor.GetTaiwuTeamCharIds());
			doctorList.UnionWith(patientList);
			patientList.UnionWith(monitor.GetTaiwuSpecialGroup());
			doctorList.UnionWith(monitor.GetTaiwuGearMateGroup());
			argBox.SetObject("DoctorList", new List<int>(doctorList));
			argBox.SetObject("PatientList", new List<int>(patientList));
			argBox.Set("NeedPay", false);
			bool flag = this._kidnappedCharacterList != null;
			if (flag)
			{
				for (int i = 0; i < this._kidnappedCharacterList.GetCount(); i++)
				{
					patientList.Add(this._kidnappedCharacterList.Get(i).CharId);
				}
			}
			UIElement.Heal.SetOnInitArgs(argBox);
			UIManager.Instance.StackToUI(UIElement.Heal);
		}

		// Token: 0x06006AD9 RID: 27353 RVA: 0x00316644 File Offset: 0x00314844
		private void OnClickBtnUsingMedicine()
		{
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.Clear();
			argBox.Set("CurrentCharacterId", MonthNotifySubPageJottings.TaiwuId);
			UIElement.UsingMedicineItem.SetOnInitArgs(argBox);
			UIManager.Instance.StackToUI(UIElement.UsingMedicineItem);
		}

		// Token: 0x06006ADA RID: 27354 RVA: 0x0031668C File Offset: 0x0031488C
		private void OnClickBtnEating()
		{
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.Clear();
			argBox.Set("CharacterId", MonthNotifySubPageJottings.TaiwuId);
			argBox.Set("CanOperate", true);
			argBox.SetObject("ViewCharacterMenuTaretPage", new SubPageIndex(ECharacterSubToggleBase.ItemBase, ECharacterSubPage.None));
			UIElement.CharacterMenu.SetOnInitArgs(argBox);
			UIManager.Instance.StackToUI(UIElement.CharacterMenu);
		}

		// Token: 0x06006ADB RID: 27355 RVA: 0x003166FC File Offset: 0x003148FC
		private void OnClickBtnLooping()
		{
			bool isLoopingOpened = this._isLoopingOpened;
			if (!isLoopingOpened)
			{
				this._isLoopingOpened = true;
				ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
				argBox.Clear();
				UIElement.Looping.SetOnInitArgs(argBox.SetObject("LoopingViewDisplayData", this._loopingViewDisplayData));
				UIManager.Instance.StackToUI(UIElement.Looping);
			}
		}

		// Token: 0x06006ADC RID: 27356 RVA: 0x00316756 File Offset: 0x00314956
		private void OnClickBtnReading()
		{
			UIManager.Instance.StackToUI(UIElement.Reading);
		}

		// Token: 0x06006ADD RID: 27357 RVA: 0x0031676C File Offset: 0x0031496C
		private void OnClickBtnNeili()
		{
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.Clear();
			argBox.Set("CharacterId", MonthNotifySubPageJottings.TaiwuId);
			argBox.Set("CanOperate", true);
			argBox.SetObject("ViewCharacterMenuTaretPage", new SubPageIndex(ECharacterSubToggleBase.NeiliBase, ECharacterSubPage.None));
			UIElement.CharacterMenu.SetOnInitArgs(argBox);
			UIManager.Instance.StackToUI(UIElement.CharacterMenu);
		}

		// Token: 0x06006ADE RID: 27358 RVA: 0x003167DC File Offset: 0x003149DC
		private string GetNumberString(int prev, int curr)
		{
			int delta = curr - prev;
			if (!true)
			{
			}
			string text;
			if (delta < 0)
			{
				text = string.Format("({0})", delta).SetColor("brightred");
			}
			else
			{
				text = string.Format("(+{0})", delta).SetColor("brightblue");
			}
			if (!true)
			{
			}
			string deltaString = text;
			return curr.ToString() + deltaString;
		}

		// Token: 0x06006ADF RID: 27359 RVA: 0x0031684C File Offset: 0x00314A4C
		private string GetNumberString(int prev, int curr, int max)
		{
			int delta = curr - prev;
			if (!true)
			{
			}
			string text;
			if (delta < 0)
			{
				text = string.Format("({0})", delta).SetColor("brightred");
			}
			else
			{
				text = string.Format("(+{0})", delta).SetColor("brightblue");
			}
			if (!true)
			{
			}
			string deltaString = text;
			return curr.ToString() + "/" + max.ToString() + deltaString;
		}

		// Token: 0x06006AE0 RID: 27360 RVA: 0x003168C8 File Offset: 0x00314AC8
		private string GetPercentString(int prev, int curr, int max, bool reversed = false)
		{
			int delta = curr - prev;
			int percent = delta * 100 / Math.Max(max, 1);
			if (!true)
			{
			}
			string result;
			if (delta < 0)
			{
				result = string.Format("({0}%)", percent).SetColor(reversed ? "brightblue" : "brightred");
			}
			else
			{
				result = string.Format("(+{0}%)", percent).SetColor(reversed ? "brightred" : "brightblue");
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x04004D3F RID: 19775
		[SerializeField]
		protected TextMeshProUGUI outerInjuryTitle;

		// Token: 0x04004D40 RID: 19776
		[SerializeField]
		protected TextMeshProUGUI innerInjuryTitle;

		// Token: 0x04004D41 RID: 19777
		[SerializeField]
		protected TextMeshProUGUI posionTitle;

		// Token: 0x04004D42 RID: 19778
		[SerializeField]
		protected TextMeshProUGUI happinessTitle;

		// Token: 0x04004D43 RID: 19779
		[SerializeField]
		protected TextMeshProUGUI healthTitle;

		// Token: 0x04004D44 RID: 19780
		[SerializeField]
		protected TextMeshProUGUI disorderTitle;

		// Token: 0x04004D45 RID: 19781
		[SerializeField]
		protected TextMeshProUGUI prevOuterInjury;

		// Token: 0x04004D46 RID: 19782
		[SerializeField]
		protected TextMeshProUGUI currOuterInjury;

		// Token: 0x04004D47 RID: 19783
		[SerializeField]
		protected TextMeshProUGUI prevInnerInjury;

		// Token: 0x04004D48 RID: 19784
		[SerializeField]
		protected TextMeshProUGUI currInnerInjury;

		// Token: 0x04004D49 RID: 19785
		[SerializeField]
		protected TextMeshProUGUI prevPoison;

		// Token: 0x04004D4A RID: 19786
		[SerializeField]
		protected TextMeshProUGUI currPoison;

		// Token: 0x04004D4B RID: 19787
		[SerializeField]
		protected CImage happinessIcon;

		// Token: 0x04004D4C RID: 19788
		[SerializeField]
		protected TextMeshProUGUI prevHappiness;

		// Token: 0x04004D4D RID: 19789
		[SerializeField]
		protected TextMeshProUGUI currHappiness;

		// Token: 0x04004D4E RID: 19790
		[SerializeField]
		protected CImage healthIcon;

		// Token: 0x04004D4F RID: 19791
		[SerializeField]
		protected TextMeshProUGUI prevHealth;

		// Token: 0x04004D50 RID: 19792
		[SerializeField]
		protected TextMeshProUGUI currHealth;

		// Token: 0x04004D51 RID: 19793
		[SerializeField]
		protected CImage qiDisorderIcon;

		// Token: 0x04004D52 RID: 19794
		[SerializeField]
		protected TextMeshProUGUI prevQiDisorder;

		// Token: 0x04004D53 RID: 19795
		[SerializeField]
		protected TextMeshProUGUI currQiDisorder;

		// Token: 0x04004D54 RID: 19796
		[SerializeField]
		protected Transform mainAttributes;

		// Token: 0x04004D55 RID: 19797
		[SerializeField]
		protected InjuryEatItem[] injuryEatItems;

		// Token: 0x04004D56 RID: 19798
		[SerializeField]
		protected CButton btnInjury;

		// Token: 0x04004D57 RID: 19799
		[SerializeField]
		protected CButton btnHeal;

		// Token: 0x04004D58 RID: 19800
		[SerializeField]
		protected CButton btnMedicine;

		// Token: 0x04004D59 RID: 19801
		[SerializeField]
		protected CButton btnEat;

		// Token: 0x04004D5A RID: 19802
		[SerializeField]
		protected PointerTrigger skillPointerTrigger;

		// Token: 0x04004D5B RID: 19803
		[SerializeField]
		protected GameObject skillHover;

		// Token: 0x04004D5C RID: 19804
		[SerializeField]
		protected TextMeshProUGUI skillLabel;

		// Token: 0x04004D5D RID: 19805
		[SerializeField]
		protected CImage skillIcon;

		// Token: 0x04004D5E RID: 19806
		[SerializeField]
		protected CImage skillFill;

		// Token: 0x04004D5F RID: 19807
		[SerializeField]
		protected CButton btnLooping;

		// Token: 0x04004D60 RID: 19808
		[SerializeField]
		protected TooltipInvoker skillTips;

		// Token: 0x04004D61 RID: 19809
		[SerializeField]
		protected GameObject loopFinished;

		// Token: 0x04004D62 RID: 19810
		[SerializeField]
		protected GameObject loopEventParticle;

		// Token: 0x04004D63 RID: 19811
		[SerializeField]
		protected GameObject neiliObtainBack;

		// Token: 0x04004D64 RID: 19812
		[SerializeField]
		protected TextMeshProUGUI neiliObtainLabel;

		// Token: 0x04004D65 RID: 19813
		[SerializeField]
		protected Transform neiliAllocations;

		// Token: 0x04004D66 RID: 19814
		[SerializeField]
		protected TextMeshProUGUI neiliLabel;

		// Token: 0x04004D67 RID: 19815
		[SerializeField]
		protected CImage neiliProgress;

		// Token: 0x04004D68 RID: 19816
		[SerializeField]
		protected NeiliTypePanel neiliTypePanel;

		// Token: 0x04004D69 RID: 19817
		[SerializeField]
		protected GameObject neiliPercentBack;

		// Token: 0x04004D6A RID: 19818
		[SerializeField]
		protected TextMeshProUGUI prevNeiliPercentTitle;

		// Token: 0x04004D6B RID: 19819
		[SerializeField]
		protected TextMeshProUGUI prevNeiliPercentValue;

		// Token: 0x04004D6C RID: 19820
		[SerializeField]
		protected TextMeshProUGUI currNeiliPercentTitle;

		// Token: 0x04004D6D RID: 19821
		[SerializeField]
		protected TextMeshProUGUI currNeiliPercentValue;

		// Token: 0x04004D6E RID: 19822
		[SerializeField]
		protected GameObject neiliTypeArrow;

		// Token: 0x04004D6F RID: 19823
		[SerializeField]
		protected TextMeshProUGUI neiliTypeCurrLabel;

		// Token: 0x04004D70 RID: 19824
		[SerializeField]
		protected CImage neiliTypeCurrIcon;

		// Token: 0x04004D71 RID: 19825
		[SerializeField]
		protected TextMeshProUGUI neiliTypePrevLabel;

		// Token: 0x04004D72 RID: 19826
		[SerializeField]
		protected CImage neiliTypePrevIcon;

		// Token: 0x04004D73 RID: 19827
		[SerializeField]
		protected CButton btnNeili;

		// Token: 0x04004D74 RID: 19828
		[SerializeField]
		protected PointerTrigger readPointerTrigger;

		// Token: 0x04004D75 RID: 19829
		[SerializeField]
		protected GameObject readHover;

		// Token: 0x04004D76 RID: 19830
		[SerializeField]
		protected TextMeshProUGUI readLabel;

		// Token: 0x04004D77 RID: 19831
		[SerializeField]
		protected ItemBack readIcon;

		// Token: 0x04004D78 RID: 19832
		[SerializeField]
		protected CImage readFill;

		// Token: 0x04004D79 RID: 19833
		[SerializeField]
		protected CButton btnReading;

		// Token: 0x04004D7A RID: 19834
		[SerializeField]
		protected TooltipInvoker readTips;

		// Token: 0x04004D7B RID: 19835
		[SerializeField]
		protected GameObject readFinished;

		// Token: 0x04004D7C RID: 19836
		[SerializeField]
		protected GameObject readEventParticle;

		// Token: 0x04004D7D RID: 19837
		[SerializeField]
		protected GameObject durabilityBack;

		// Token: 0x04004D7E RID: 19838
		[SerializeField]
		protected TextMeshProUGUI durabilityLabel;

		// Token: 0x04004D7F RID: 19839
		[SerializeField]
		protected Transform referenceBooks;

		// Token: 0x04004D80 RID: 19840
		[SerializeField]
		protected Transform pages;

		// Token: 0x04004D81 RID: 19841
		[SerializeField]
		protected GameObject noReadingBook;

		// Token: 0x04004D82 RID: 19842
		[SerializeField]
		protected GameObject noRefBook;

		// Token: 0x04004D83 RID: 19843
		[SerializeField]
		protected CButton btnClose;

		// Token: 0x04004D84 RID: 19844
		private KidnappedCharacterList _kidnappedCharacterList;

		// Token: 0x04004D85 RID: 19845
		private LoopingViewDisplayData _loopingViewDisplayData;

		// Token: 0x04004D86 RID: 19846
		private bool _isLoopingOpened;

		// Token: 0x04004D87 RID: 19847
		private MonthNotify _data;

		// Token: 0x04004D88 RID: 19848
		private Dictionary<ItemKey, ItemDisplayData> _eatenItem;
	}
}
