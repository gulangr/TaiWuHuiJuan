using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Config;
using FrameWork;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using Game.Components.Common;
using Game.Components.Item;
using Game.Components.ListStyleGeneralScroll;
using Game.Components.SortAndFilter;
using Game.Components.SortAndFilter.Heal;
using Game.Views.Migrate;
using GameData.Common;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Extra;
using GameData.Domains.Global;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Map;
using GameData.Domains.TaiwuEvent;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using GameDataExtensions;
using Spine.Unity;
using TMPro;
using UnityEngine;

// Token: 0x02000384 RID: 900
public class UI_Heal : UIBase
{
	// Token: 0x1700059F RID: 1439
	// (get) Token: 0x06003506 RID: 13574 RVA: 0x001A7EB7 File Offset: 0x001A60B7
	private MapHealSimulateResult CurHealInjuryCost
	{
		get
		{
			return this._healInjuryCostDict.GetOrDefault(this._selectedPatient);
		}
	}

	// Token: 0x170005A0 RID: 1440
	// (get) Token: 0x06003507 RID: 13575 RVA: 0x001A7ECA File Offset: 0x001A60CA
	private MapHealSimulateResult CurHealPoisonCost
	{
		get
		{
			return this._healPoisonCostDict.GetOrDefault(this._selectedPatient);
		}
	}

	// Token: 0x170005A1 RID: 1441
	// (get) Token: 0x06003508 RID: 13576 RVA: 0x001A7EDD File Offset: 0x001A60DD
	private MapHealSimulateResult CurHealQiDisorderCost
	{
		get
		{
			return this._healQiDisorderCostDict.GetOrDefault(this._selectedPatient);
		}
	}

	// Token: 0x170005A2 RID: 1442
	// (get) Token: 0x06003509 RID: 13577 RVA: 0x001A7EF0 File Offset: 0x001A60F0
	private MapHealSimulateResult CurHealHealthCost
	{
		get
		{
			return this._healHealthCostDict.GetOrDefault(this._selectedPatient);
		}
	}

	// Token: 0x0600350A RID: 13578 RVA: 0x001A7F04 File Offset: 0x001A6104
	private ItemDisplayData GetUsingTool(sbyte repairType)
	{
		bool flag = this._previewToolOverride != null;
		ItemDisplayData result;
		if (flag)
		{
			result = this._previewToolOverride;
		}
		else
		{
			result = (this._itemSelector.SelectedItem ?? this._currentGearMateRepairAutoTool.GetOrDefault(repairType));
		}
		return result;
	}

	// Token: 0x170005A3 RID: 1443
	// (get) Token: 0x0600350B RID: 13579 RVA: 0x001A7F47 File Offset: 0x001A6147
	private bool IsUsingToolAutoSelected
	{
		get
		{
			return this._itemSelector.SelectedItem == null;
		}
	}

	// Token: 0x170005A4 RID: 1444
	// (get) Token: 0x0600350C RID: 13580 RVA: 0x001A7F57 File Offset: 0x001A6157
	private bool IsSelectedPatientGearMate
	{
		get
		{
			return this._selectedPatient >= 0 && SingletonObject.getInstance<CharacterMonitorModel>().IsTaiwuGearMate(this._selectedPatient);
		}
	}

	// Token: 0x0600350D RID: 13581 RVA: 0x001A7F75 File Offset: 0x001A6175
	private bool HasSelectedDoctor()
	{
		return this._selectedDoctor >= 0;
	}

	// Token: 0x0600350E RID: 13582 RVA: 0x001A7F83 File Offset: 0x001A6183
	private bool HasSelectedPatient()
	{
		return this._selectedPatient >= 0;
	}

	// Token: 0x0600350F RID: 13583 RVA: 0x001A7F91 File Offset: 0x001A6191
	private bool IsGearMate(int charId)
	{
		return SingletonObject.getInstance<CharacterMonitorModel>().IsTaiwuGearMate(charId);
	}

	// Token: 0x06003510 RID: 13584 RVA: 0x001A7FA0 File Offset: 0x001A61A0
	public override void OnInit(ArgumentBox argsBox)
	{
		this.NeedDataListenerId = true;
		bool flag = !this._isInitItemSelector;
		if (flag)
		{
			this._itemSelector = new UI_Heal.ItemSelector(this, this.itemSelectorScroll);
			this._isInitItemSelector = true;
		}
		this._taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
		List<int> doctorList;
		argsBox.Get<List<int>>("DoctorList", out doctorList);
		doctorList.RemoveAll((int doctorId) => SingletonObject.getInstance<CharacterMonitorModel>().IsTaiwuBeastTeammate(doctorId));
		List<int> patientList;
		argsBox.Get<List<int>>("PatientList", out patientList);
		argsBox.Get("NeedPay", out this._needPlay);
		bool flag2 = !argsBox.Get("CurrentCharacterId", out this._currentCharacterId);
		if (flag2)
		{
			this._currentCharacterId = this._taiwuCharId;
		}
		else
		{
			doctorList.Sort(delegate(int a, int b)
			{
				bool flag3 = a == this._currentCharacterId && b != this._currentCharacterId;
				int result;
				if (flag3)
				{
					result = -1;
				}
				else
				{
					bool flag4 = a != this._currentCharacterId && b == this._currentCharacterId;
					if (flag4)
					{
						result = 1;
					}
					else
					{
						result = a.CompareTo(b);
					}
				}
				return result;
			});
			patientList.Sort(delegate(int a, int b)
			{
				bool flag3 = a == this._currentCharacterId && b != this._currentCharacterId;
				int result;
				if (flag3)
				{
					result = -1;
				}
				else
				{
					bool flag4 = a != this._currentCharacterId && b == this._currentCharacterId;
					if (flag4)
					{
						result = 1;
					}
					else
					{
						result = a.CompareTo(b);
					}
				}
				return result;
			});
		}
		argsBox.Get("ExpensiveHeal", out this._expensiveHeal);
		string str = "_doctorList:";
		List<int> doctorList2 = this._doctorList;
		Debug.Log(str + ((doctorList2 != null) ? doctorList2.ToString() : null));
		string str2 = "doctorList:";
		List<int> list = doctorList;
		Debug.Log(str2 + ((list != null) ? list.ToString() : null));
		this._doctorList.Clear();
		this._doctorList.AddRange(doctorList);
		this._originDoctorList.Clear();
		this._originDoctorList.AddRange(doctorList);
		this._patientList.Clear();
		this._patientList.AddRange(patientList);
		this._originPatientList.Clear();
		this._originPatientList.AddRange(patientList);
		this._displayDataDict.Clear();
		this._doctorHealCountDict.Clear();
		this._doctorAttainmentDict.Clear();
		this._patientInjuryDict.Clear();
		this._patientPoisonDict.Clear();
		this._patientHealthDict.Clear();
		this._patientMaxHealthDict.Clear();
		this._patientAvailableFeatureDict.Clear();
		this._doctorSortDataList.Clear();
		this._patientSortDataList.Clear();
		this.canvasFront.alpha = 0f;
		this.healGearMateSelectedToolDurability.gameObject.SetActive(false);
		GlobalDomainMethod.Call.InvokeGuidingTrigger(62);
		UIElement element = this.Element;
		element.OnShowed = (Action)Delegate.Combine(element.OnShowed, new Action(delegate()
		{
			this.canvasFront.alpha = 1f;
		}));
		UIElement element2 = this.Element;
		element2.OnHide = (Action)Delegate.Combine(element2.OnHide, new Action(delegate()
		{
			TaiwuEventDomainMethod.Call.TriggerListener("HealActionComplete", true);
			GEvent.OnEvent(UiEvents.HealUiClosed, null);
		}));
		ItemDomainMethod.AsyncCall.GetEmptyToolKey(this, delegate(int offset, RawDataPool pool)
		{
			Serializer.Deserialize(pool, offset, ref this._emptyToolKey);
		});
		foreach (int doctorId2 in this._doctorList)
		{
			CharacterDomainMethod.AsyncCall.GetUsableCombatResources(this, doctorId2, delegate(int offset, RawDataPool pool)
			{
				CombatResources combatResources = default(CombatResources);
				Serializer.Deserialize(pool, offset, ref combatResources);
				this._doctorHealCountDict[this._doctorList[this._doctorHealCountDict.Count]] = combatResources;
			});
		}
		using (List<int>.Enumerator enumerator2 = this._patientList.GetEnumerator())
		{
			while (enumerator2.MoveNext())
			{
				int charId = enumerator2.Current;
				CharacterDomainMethod.AsyncCall.GetLeftMaxHealth(this, charId, delegate(int offset, RawDataPool pool)
				{
					short maxHealth = 0;
					Serializer.Deserialize(pool, offset, ref maxHealth);
					this._patientMaxHealthDict[charId] = maxHealth;
					this.RefreshPatientSortData(charId);
					this.patientScroll.RefreshCell(this._patientList.IndexOf(charId));
					this.UpdatePatientInfo();
					this.UpdateHealHealthBtn(-1);
				});
			}
		}
		foreach (int charId2 in this._patientList)
		{
			this.RequestPatientAvailableFeature(charId2);
		}
		this.RefreshGearMateToolItemView(null, false);
		SingletonObject.getInstance<YieldHelper>().DelayFrameDo(2U, delegate
		{
			List<int> allCharList = EasyPool.Get<List<int>>();
			allCharList.Clear();
			allCharList.AddRange(this._doctorList);
			foreach (int charId3 in this._patientList)
			{
				bool flag3 = !allCharList.Contains(charId3);
				if (flag3)
				{
					allCharList.Add(charId3);
				}
			}
			this._selectedDoctor = -1;
			this._selectedPatient = -1;
			this.healCurrDoctor.avatarBack.SetSprite("ui9_back_heal_player_0", false, null);
			this.healCurrPatient.avatarBack.SetSprite("ui9_back_heal_player_0", false, null);
			CharacterDomainMethod.Call.GetCharacterDisplayDataList(this.Element.GameDataListenerId, allCharList);
			EasyPool.Free<List<int>>(allCharList);
			this.RefreshDate();
			this.RefreshSpiritualDebt(-1);
		});
	}

	// Token: 0x06003511 RID: 13585 RVA: 0x001A838C File Offset: 0x001A658C
	public override void InitMonitorFieldIds()
	{
		this.MonitorFields.Add(new UIBase.MonitorDataField(4, 0, (ulong)((long)this._taiwuCharId), new uint[]
		{
			34U
		}));
		foreach (int doctorId in this._doctorList)
		{
			this.MonitorFields.Add(new UIBase.MonitorDataField(4, 0, (ulong)((long)doctorId), new uint[]
			{
				97U
			}));
		}
		foreach (int doctorId2 in this._patientList)
		{
			this.MonitorFields.Add(new UIBase.MonitorDataField(4, 0, (ulong)((long)doctorId2), new uint[]
			{
				26U,
				44U,
				21U,
				19U,
				17U,
				55U
			}));
		}
	}

	// Token: 0x06003512 RID: 13586 RVA: 0x001A8484 File Offset: 0x001A6684
	private void RefreshGearMateToolDurability()
	{
		bool flag = !this.IsSelectedPatientGearMate;
		if (flag)
		{
			this.healGearMateSelectedToolDurability.gameObject.SetActive(false);
		}
		else
		{
			this.healGearMateSelectedToolDurability.gameObject.SetActive(true);
			ItemDisplayData selectedTool = this._itemSelector.SelectedItem;
			bool flag2 = selectedTool != null && selectedTool.Key != this._emptyToolKey;
			if (flag2)
			{
				this.healGearMateSelectedToolDurability.value.text = string.Format("{0}/{1}", selectedTool.Durability, selectedTool.MaxDurability);
			}
			else
			{
				this.healGearMateSelectedToolDurability.value.text = "-/-";
			}
		}
	}

	// Token: 0x06003513 RID: 13587 RVA: 0x001A853C File Offset: 0x001A673C
	private void RefreshDurabilityCostLabel(sbyte repairType)
	{
		List<GearMateRepairRequirementDisplayData> requireItemList;
		bool flag = !this._gearMateRepairRequirementDict.TryGetValue(this._selectedPatient, out requireItemList);
		if (!flag)
		{
			GearMateRepairRequirementDisplayData requireItem = requireItemList[(int)repairType];
			ItemDisplayData usingTool = this.GetUsingTool(repairType);
			bool flag2 = usingTool == null;
			if (!flag2)
			{
				bool flag3 = usingTool.Key == this._emptyToolKey;
				if (flag3)
				{
					this.healGearMateSelectedToolDurability.value.text = "-/-";
					this.healGearMateSelectedToolDurability.gameObject.SetActive(true);
				}
				else
				{
					short cost = CraftTool.Instance[usingTool.Key.TemplateId].DurabilityCost[(int)requireItem.ItemGrade];
					this.healGearMateSelectedToolDurability.value.text = string.Format("{0}{1}/{2}", usingTool.Durability, string.Format("-{0}", cost).SetColor("brightred"), usingTool.MaxDurability);
					this.healGearMateSelectedToolDurability.gameObject.SetActive(true);
				}
			}
		}
	}

	// Token: 0x06003514 RID: 13588 RVA: 0x001A864C File Offset: 0x001A684C
	private void Awake()
	{
		this.InitActionButtonPointerTriggers();
		bool flag = this.doctorSortAndFilter != null;
		if (flag)
		{
			this._doctorSortController = new HealDoctorSortAndFilterController(this.doctorSortAndFilter);
			this._doctorSortController.Init(new Action(this.OnDoctorSortDataListChanged), "HealDoctorSort");
		}
		this.doctorScroll.OnItemRender += this.OnRenderDoctor;
		bool flag2 = this.patientSortAndFilter != null;
		if (flag2)
		{
			this._patientSortController = new HealPatientSortAndFilterController(this.patientSortAndFilter);
			this._patientSortController.Init(new Action(this.OnPatientSortDataListChanged), "HealPatientSort");
		}
		this.patientScroll.OnItemRender += this.OnRenderPatient;
	}

	// Token: 0x06003515 RID: 13589 RVA: 0x001A8714 File Offset: 0x001A6914
	private void OnEnable()
	{
		GEvent.Add(EEvents.OnActionPointChange, new GEvent.Callback(this.OnDaysInMonthChange));
		GEvent.Add(UiEvents.TopUiChanged, new GEvent.Callback(this.OnTopUiChanged));
		GEvent.Add(EEvents.OnActionPointChange, new GEvent.Callback(this.OnActionPointChange));
		GEvent.Add(EEvents.OnAreaSpiritualDebtChange, new GEvent.Callback(this.OnAreaSpiritualDebtChange));
	}

	// Token: 0x06003516 RID: 13590 RVA: 0x001A8788 File Offset: 0x001A6988
	private void OnDisable()
	{
		GEvent.Remove(EEvents.OnActionPointChange, new GEvent.Callback(this.OnDaysInMonthChange));
		GEvent.Remove(UiEvents.TopUiChanged, new GEvent.Callback(this.OnTopUiChanged));
		GEvent.Remove(EEvents.OnActionPointChange, new GEvent.Callback(this.OnActionPointChange));
		GEvent.Remove(EEvents.OnAreaSpiritualDebtChange, new GEvent.Callback(this.OnAreaSpiritualDebtChange));
		GEvent.OnEvent(UiEvents.OnRefreshCharacterHealUIBottom, null);
		this._itemSelector.Clear();
		this.doctorScroll.UpdateData(0);
		this.patientScroll.UpdateData(0);
	}

	// Token: 0x06003517 RID: 13591 RVA: 0x001A8830 File Offset: 0x001A6A30
	private void InitActionButtonPointerTriggers()
	{
		this.InitInjuryPointerTrigger();
		this.InitOutInjuryPointerTrigger();
		this.InitInnerInjuryPointerTrigger();
		this.InitPoisonPointerTrigger();
		this.InitQiDisorderPointerTrigger();
		this.InitHealthPointerTrigger();
		this.InitGearMatePoisonPointerTrigger();
		this.InitGearMateQiDisorderPointerTrigger();
	}

	// Token: 0x06003518 RID: 13592 RVA: 0x001A886C File Offset: 0x001A6A6C
	private void InitInjuryPointerTrigger()
	{
		PointerTrigger injuryTrigger = this.healInjury.GetComponent<PointerTrigger>();
		injuryTrigger.EnterEvent.RemoveAllListeners();
		injuryTrigger.EnterEvent.AddListener(delegate()
		{
			this.OnInjuryTriggerChanged(true);
		});
		injuryTrigger.ExitEvent.RemoveAllListeners();
		injuryTrigger.ExitEvent.AddListener(delegate()
		{
			this.OnInjuryTriggerChanged(false);
		});
	}

	// Token: 0x06003519 RID: 13593 RVA: 0x001A88D0 File Offset: 0x001A6AD0
	private unsafe void OnInjuryTriggerChanged(bool isEnter)
	{
		bool isSelectedPatientGearMate = this.IsSelectedPatientGearMate;
		if (!isSelectedPatientGearMate)
		{
			bool isPreview = false;
			if (isEnter)
			{
				int doctorId = this._selectedDoctor;
				bool flag = !this._doctorHealCountDict.ContainsKey(doctorId);
				if (flag)
				{
					return;
				}
				int leftDays = UI_Heal.GetLeftDate();
				int leftSpiritualDebt = this.GetLeftSpiritualDebt();
				bool herbEnough = this._needPlay || *this._taiwuResource[5] >= this.CurHealInjuryCost.CostHerb;
				bool moneyEnough = !this._needPlay || *this._taiwuResource[6] >= this.CurHealInjuryCost.CostMoney;
				bool timeEnough = leftDays > 0;
				bool spiritualDebtEnough = leftSpiritualDebt > this.CurHealInjuryCost.CostSpiritualDebt;
				bool hasHealCount = this._doctorHealCountDict[doctorId].HealingCount > 0;
				bool canHealInjury = hasHealCount && herbEnough && moneyEnough && timeEnough && this.CurHealInjuryCost.HealEffect > 0;
				bool flag2 = this._expensiveHeal && !spiritualDebtEnough;
				if (flag2)
				{
					canHealInjury = false;
				}
				isPreview = canHealInjury;
			}
			this.RefreshCostResource(5, UI_Heal.EHealType.Injury, isPreview);
			bool needPlay = this._needPlay;
			if (needPlay)
			{
				this.RefreshCostResource(6, UI_Heal.EHealType.Injury, isPreview);
			}
			this.RefreshCharInfoLifeSkill(8, isPreview, false, UI_Heal.EHealType.Injury);
			this.RefreshCharInfoHeal(UI_Heal.EHealType.Injury, isPreview);
			this.RefreshSpiritualDebt(isEnter ? 0 : -1);
		}
	}

	// Token: 0x0600351A RID: 13594 RVA: 0x001A8A2C File Offset: 0x001A6C2C
	private void InitOutInjuryPointerTrigger()
	{
		PointerTrigger outInjuryTrigger = this.healOutInjury.GetComponent<PointerTrigger>();
		outInjuryTrigger.EnterEvent.RemoveAllListeners();
		outInjuryTrigger.EnterEvent.AddListener(delegate()
		{
			this.OnOutInjuryTriggerChanged(true, true);
		});
		outInjuryTrigger.ExitEvent.RemoveAllListeners();
		outInjuryTrigger.ExitEvent.AddListener(delegate()
		{
			this.OnOutInjuryTriggerChanged(false, false);
		});
	}

	// Token: 0x0600351B RID: 13595 RVA: 0x001A8A90 File Offset: 0x001A6C90
	private void OnOutInjuryTriggerChanged(bool isEnter, bool isShowAutoSelectedToolAttainment = false)
	{
		bool flag = !this.IsSelectedPatientGearMate;
		if (!flag)
		{
			int canHealOutResult;
			bool canHealOutInjury = this._currentGearMateCanRepair.TryGetValue(0, out canHealOutResult) && canHealOutResult == 0;
			bool isPreview = isEnter && canHealOutInjury;
			this.RefreshCostResource(2, UI_Heal.EHealType.OuterInjury, isPreview);
			this.RefreshCharInfoLifeSkill(6, isPreview, this.IsUsingToolAutoSelected && isEnter && isShowAutoSelectedToolAttainment, UI_Heal.EHealType.Invalid);
			this.RefreshCharInfoHeal(UI_Heal.EHealType.OuterInjury, isPreview);
			this.OnGearMateButtonTrigger(isEnter, 0, isShowAutoSelectedToolAttainment);
		}
	}

	// Token: 0x0600351C RID: 13596 RVA: 0x001A8AFC File Offset: 0x001A6CFC
	private void OnGearMateButtonTrigger(bool isEnter, sbyte repairType, bool isShowAutoSelectedTool = true)
	{
		if (isEnter)
		{
			this.RefreshDurabilityCostLabel(repairType);
		}
		else
		{
			this.RefreshGearMateToolDurability();
		}
		bool flag = !this.IsUsingToolAutoSelected;
		if (!flag)
		{
			bool flag2 = isEnter && isShowAutoSelectedTool;
			if (flag2)
			{
				ItemDisplayData usingTool = this.GetUsingTool(repairType);
				this.RefreshGearMateToolItemView(usingTool, this.IsUsingToolAutoSelected);
				this._itemSelector.ReRenderByAutoSelectedItem(this._currentGearMateRepairAutoTool[repairType]);
			}
			else
			{
				this.RefreshGearMateToolItemView(null, false);
				this._itemSelector.ReRenderByAutoSelectedItem(null);
			}
		}
	}

	// Token: 0x0600351D RID: 13597 RVA: 0x001A8B80 File Offset: 0x001A6D80
	private void InitInnerInjuryPointerTrigger()
	{
		PointerTrigger innerInjuryTrigger = this.healInnerInjury.GetComponent<PointerTrigger>();
		innerInjuryTrigger.EnterEvent.RemoveAllListeners();
		innerInjuryTrigger.EnterEvent.AddListener(delegate()
		{
			this.OnInnerInjuryTriggerChanged(true, true);
		});
		innerInjuryTrigger.ExitEvent.RemoveAllListeners();
		innerInjuryTrigger.ExitEvent.AddListener(delegate()
		{
			this.OnInnerInjuryTriggerChanged(false, false);
		});
	}

	// Token: 0x0600351E RID: 13598 RVA: 0x001A8BE4 File Offset: 0x001A6DE4
	private void OnInnerInjuryTriggerChanged(bool isEnter, bool isShowAutoSelectedToolAttainment = false)
	{
		bool flag = !this.IsSelectedPatientGearMate;
		if (!flag)
		{
			int canHealInnerResult;
			bool canHealInnerInjury = this._currentGearMateCanRepair.TryGetValue(1, out canHealInnerResult) && canHealInnerResult == 0;
			bool isPreview = isEnter && canHealInnerInjury;
			this.RefreshCostResource(1, UI_Heal.EHealType.InnerInjury, isPreview);
			this.RefreshCharInfoLifeSkill(7, isPreview, this.IsUsingToolAutoSelected && isEnter && isShowAutoSelectedToolAttainment, UI_Heal.EHealType.Invalid);
			this.RefreshCharInfoHeal(UI_Heal.EHealType.InnerInjury, isPreview);
			this.OnGearMateButtonTrigger(isEnter, 1, isShowAutoSelectedToolAttainment);
		}
	}

	// Token: 0x0600351F RID: 13599 RVA: 0x001A8C50 File Offset: 0x001A6E50
	private void InitPoisonPointerTrigger()
	{
		PointerTrigger poisonTrigger = this.healPoison.GetComponent<PointerTrigger>();
		poisonTrigger.EnterEvent.RemoveAllListeners();
		poisonTrigger.EnterEvent.AddListener(delegate()
		{
			this.OnPoisonTriggerChanged(true);
		});
		poisonTrigger.ExitEvent.RemoveAllListeners();
		poisonTrigger.ExitEvent.AddListener(delegate()
		{
			this.OnPoisonTriggerChanged(false);
		});
	}

	// Token: 0x06003520 RID: 13600 RVA: 0x001A8CB4 File Offset: 0x001A6EB4
	private unsafe void OnPoisonTriggerChanged(bool isEnter)
	{
		bool isSelectedPatientGearMate = this.IsSelectedPatientGearMate;
		if (!isSelectedPatientGearMate)
		{
			bool isPreview = false;
			if (isEnter)
			{
				int doctorId = this._selectedDoctor;
				bool flag = !this._doctorHealCountDict.ContainsKey(doctorId);
				if (flag)
				{
					return;
				}
				int leftDays = SingletonObject.getInstance<TimeManager>().GetRemainingActionPointConvertToDays();
				int leftSpiritualDebt = this.GetLeftSpiritualDebt();
				bool herbEnough = this._needPlay || *this._taiwuResource[5] >= this.CurHealPoisonCost.CostHerb;
				bool moneyEnough = !this._needPlay || *this._taiwuResource[6] >= this.CurHealPoisonCost.CostMoney;
				bool timeEnough = leftDays > 0;
				bool spiritualDebtEnough = leftSpiritualDebt > this.CurHealPoisonCost.CostSpiritualDebt;
				bool hasHealCount = this._doctorHealCountDict[doctorId].DetoxCount > 0;
				bool canHealPoison = hasHealCount && herbEnough && moneyEnough && timeEnough && this.CurHealPoisonCost.HealEffect > 0;
				bool flag2 = this._expensiveHeal && !spiritualDebtEnough;
				if (flag2)
				{
					canHealPoison = false;
				}
				isPreview = canHealPoison;
			}
			this.RefreshCostResource(5, UI_Heal.EHealType.Poison, isPreview);
			bool needPlay = this._needPlay;
			if (needPlay)
			{
				this.RefreshCostResource(6, UI_Heal.EHealType.Poison, isPreview);
			}
			this.RefreshCharInfoLifeSkill(9, isPreview, false, UI_Heal.EHealType.Poison);
			this.RefreshCharInfoHeal(UI_Heal.EHealType.Poison, isPreview);
			this.RefreshSpiritualDebt(isEnter ? 1 : -1);
		}
	}

	// Token: 0x06003521 RID: 13601 RVA: 0x001A8E14 File Offset: 0x001A7014
	private void InitQiDisorderPointerTrigger()
	{
		PointerTrigger qiDisorderTrigger = this.healQiDisorder.GetComponent<PointerTrigger>();
		qiDisorderTrigger.EnterEvent.RemoveAllListeners();
		qiDisorderTrigger.EnterEvent.AddListener(delegate()
		{
			this.OnQiDisorderPointerChanged(true);
		});
		qiDisorderTrigger.ExitEvent.RemoveAllListeners();
		qiDisorderTrigger.ExitEvent.AddListener(delegate()
		{
			this.OnQiDisorderPointerChanged(false);
		});
	}

	// Token: 0x06003522 RID: 13602 RVA: 0x001A8E78 File Offset: 0x001A7078
	private unsafe void OnQiDisorderPointerChanged(bool isEnter)
	{
		bool isSelectedPatientGearMate = this.IsSelectedPatientGearMate;
		if (!isSelectedPatientGearMate)
		{
			bool isPreview = false;
			if (isEnter)
			{
				int doctorId = this._selectedDoctor;
				bool flag = !this._doctorHealCountDict.ContainsKey(doctorId);
				if (flag)
				{
					return;
				}
				int leftDays = SingletonObject.getInstance<TimeManager>().GetRemainingActionPointConvertToDays();
				int leftSpiritualDebt = this.GetLeftSpiritualDebt();
				bool herbEnough = this._needPlay || *this._taiwuResource[5] >= this.CurHealQiDisorderCost.CostHerb;
				bool moneyEnough = !this._needPlay || *this._taiwuResource[6] >= this.CurHealQiDisorderCost.CostMoney;
				bool timeEnough = leftDays > 0;
				bool spiritualDebtEnough = leftSpiritualDebt > this.CurHealQiDisorderCost.CostSpiritualDebt;
				bool hasHealCount = this._doctorHealCountDict[doctorId].BreathingCount > 0;
				bool canHealQiDisorder = hasHealCount && herbEnough && moneyEnough && timeEnough && this.CurHealQiDisorderCost.HealEffect > 0;
				bool flag2 = this._expensiveHeal && !spiritualDebtEnough;
				if (flag2)
				{
					canHealQiDisorder = false;
				}
				isPreview = canHealQiDisorder;
			}
			LifeSkillShorts attainments = this._doctorAttainmentDict.GetOrDefault(this._selectedDoctor);
			sbyte skillType = UI_Heal.GetMaxAttainmentForNormal(attainments);
			this.RefreshCostResource(5, UI_Heal.EHealType.QiDisorder, isPreview);
			bool needPlay = this._needPlay;
			if (needPlay)
			{
				this.RefreshCostResource(6, UI_Heal.EHealType.QiDisorder, isPreview);
			}
			this.RefreshCharInfoLifeSkill(skillType, isPreview, false, UI_Heal.EHealType.QiDisorder);
			this.RefreshCharInfoHeal(UI_Heal.EHealType.QiDisorder, isPreview);
			this.RefreshSpiritualDebt(isEnter ? 2 : -1);
		}
	}

	// Token: 0x06003523 RID: 13603 RVA: 0x001A8FF4 File Offset: 0x001A71F4
	private void InitHealthPointerTrigger()
	{
		PointerTrigger healthTrigger = this.healHealth.GetComponent<PointerTrigger>();
		healthTrigger.EnterEvent.RemoveAllListeners();
		healthTrigger.EnterEvent.AddListener(delegate()
		{
			this.OnHealthPointerChanged(true);
		});
		healthTrigger.ExitEvent.RemoveAllListeners();
		healthTrigger.ExitEvent.AddListener(delegate()
		{
			this.OnHealthPointerChanged(false);
		});
	}

	// Token: 0x06003524 RID: 13604 RVA: 0x001A9058 File Offset: 0x001A7258
	private unsafe void OnHealthPointerChanged(bool isEnter)
	{
		bool isSelectedPatientGearMate = this.IsSelectedPatientGearMate;
		if (!isSelectedPatientGearMate)
		{
			bool isPreview = false;
			if (isEnter)
			{
				int doctorId = this._selectedDoctor;
				bool flag = !this._doctorHealCountDict.ContainsKey(doctorId);
				if (flag)
				{
					return;
				}
				int leftDays = SingletonObject.getInstance<TimeManager>().GetRemainingActionPointConvertToDays();
				int leftSpiritualDebt = this.GetLeftSpiritualDebt();
				bool herbEnough = this._needPlay || *this._taiwuResource[5] >= this.CurHealHealthCost.CostHerb;
				bool moneyEnough = !this._needPlay || *this._taiwuResource[6] >= this.CurHealHealthCost.CostMoney;
				bool timeEnough = leftDays > 0;
				bool spiritualDebtEnough = leftSpiritualDebt > this.CurHealHealthCost.CostSpiritualDebt;
				bool hasHealCount = this._doctorHealCountDict.GetOrDefault(doctorId).RecoverCount > 0;
				bool canHealHealth = hasHealCount && herbEnough && moneyEnough && timeEnough && this.CurHealHealthCost.HealEffect > 0;
				bool flag2 = this._expensiveHeal && !spiritualDebtEnough;
				if (flag2)
				{
					canHealHealth = false;
				}
				isPreview = canHealHealth;
			}
			LifeSkillShorts attainments = this._doctorAttainmentDict.GetOrDefault(this._selectedDoctor);
			sbyte skillType = UI_Heal.GetMaxAttainmentForNormal(attainments);
			this.RefreshCostResource(5, UI_Heal.EHealType.Health, isPreview);
			bool needPlay = this._needPlay;
			if (needPlay)
			{
				this.RefreshCostResource(6, UI_Heal.EHealType.Health, isPreview);
			}
			this.RefreshCharInfoLifeSkill(skillType, isPreview, false, UI_Heal.EHealType.Health);
			this.RefreshCharInfoHeal(UI_Heal.EHealType.Health, isPreview);
			this.RefreshSpiritualDebt(isEnter ? 3 : -1);
		}
	}

	// Token: 0x06003525 RID: 13605 RVA: 0x001A91D4 File Offset: 0x001A73D4
	private void InitGearMatePoisonPointerTrigger()
	{
		PointerTrigger trigger = this.gearMateHealPoison.GetComponent<PointerTrigger>();
		trigger.EnterEvent.RemoveAllListeners();
		trigger.EnterEvent.AddListener(delegate()
		{
			this.OnGearMateHealPoisonPointerChanged(true, true);
		});
		trigger.ExitEvent.RemoveAllListeners();
		trigger.ExitEvent.AddListener(delegate()
		{
			this.OnGearMateHealPoisonPointerChanged(false, false);
		});
	}

	// Token: 0x06003526 RID: 13606 RVA: 0x001A9238 File Offset: 0x001A7438
	private void OnGearMateHealPoisonPointerChanged(bool isEnter, bool isShowAutoSelectedToolAttainment = false)
	{
		bool flag = !this.IsSelectedPatientGearMate;
		if (!flag)
		{
			int canHealInnerResult;
			bool canHealPoison = this._currentGearMateCanRepair.TryGetValue(2, out canHealInnerResult) && canHealInnerResult == 0;
			bool isPreview = isEnter && canHealPoison;
			this.RefreshCostResource(3, UI_Heal.EHealType.Poison, isPreview);
			this.RefreshCharInfoLifeSkill(11, isPreview, this.IsUsingToolAutoSelected && isEnter && isShowAutoSelectedToolAttainment, UI_Heal.EHealType.Invalid);
			this.RefreshCharInfoHeal(UI_Heal.EHealType.Poison, isPreview);
			this.OnGearMateButtonTrigger(isEnter, 2, isShowAutoSelectedToolAttainment);
		}
	}

	// Token: 0x06003527 RID: 13607 RVA: 0x001A92A4 File Offset: 0x001A74A4
	private void InitGearMateQiDisorderPointerTrigger()
	{
		PointerTrigger trigger = this.gearMateHealQiDisorder.GetComponent<PointerTrigger>();
		trigger.EnterEvent.RemoveAllListeners();
		trigger.EnterEvent.AddListener(delegate()
		{
			this.OnGearMateHealQiDisorderPointerChanged(true, true);
		});
		trigger.ExitEvent.RemoveAllListeners();
		trigger.ExitEvent.AddListener(delegate()
		{
			this.OnGearMateHealQiDisorderPointerChanged(false, false);
		});
	}

	// Token: 0x06003528 RID: 13608 RVA: 0x001A9308 File Offset: 0x001A7508
	private void OnGearMateHealQiDisorderPointerChanged(bool isEnter, bool isShowAutoSelectedToolAttainment = false)
	{
		bool flag = !this.IsSelectedPatientGearMate;
		if (!flag)
		{
			int canHealInnerResult;
			bool canHealQiDisorder = this._currentGearMateCanRepair.TryGetValue(3, out canHealInnerResult) && canHealInnerResult == 0;
			bool isPreview = isEnter && canHealQiDisorder;
			this.RefreshCostResource(4, UI_Heal.EHealType.QiDisorder, isPreview);
			this.RefreshCharInfoLifeSkill(10, isPreview, this.IsUsingToolAutoSelected && isEnter && isShowAutoSelectedToolAttainment, UI_Heal.EHealType.Invalid);
			this.RefreshCharInfoHeal(UI_Heal.EHealType.QiDisorder, isPreview);
			this.OnGearMateButtonTrigger(isEnter, 3, isShowAutoSelectedToolAttainment);
		}
	}

	// Token: 0x06003529 RID: 13609 RVA: 0x001A9374 File Offset: 0x001A7574
	public override void OnNotifyGameData(List<NotificationWrapper> notifications)
	{
		foreach (NotificationWrapper wrapper in notifications)
		{
			Notification notification = wrapper.Notification;
			byte type = notification.Type;
			byte b = type;
			if (b != 0)
			{
				if (b == 1)
				{
					bool flag = notification.DomainId == 4;
					if (flag)
					{
						bool flag2 = notification.MethodId == 48;
						if (flag2)
						{
							List<CharacterDisplayData> displayDataList = null;
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref displayDataList);
							for (int i = 0; i < displayDataList.Count; i++)
							{
								CharacterDisplayData data = displayDataList[i];
								this._displayDataDict[data.CharacterId] = data;
							}
							foreach (KeyValuePair<int, CharacterDisplayData> keyValuePair in this._displayDataDict)
							{
								bool flag3 = this._doctorList.Contains(keyValuePair.Key) && (int)keyValuePair.Value.PhysiologicalAge < GlobalConfig.Instance.AgeBaby;
								if (flag3)
								{
									this._doctorList.RemoveAt(this._doctorList.IndexOf(keyValuePair.Key));
									this._originDoctorList.RemoveAt(this._originDoctorList.IndexOf(keyValuePair.Key));
								}
							}
							this._doctorSortDataList.Clear();
							foreach (int id in this._doctorList)
							{
								LifeSkillShorts attainments = this._doctorAttainmentDict.GetOrDefault(id);
								HealDoctorSortData data2 = new HealDoctorSortData
								{
									CharId = id,
									MedicineAttainment = attainments.Get(8),
									ToxicologyAttainment = attainments.Get(9),
									ForgingAttainment = attainments.Get(6),
									WoodworkingAttainment = attainments.Get(7),
									WeavingAttainment = attainments.Get(10),
									JadeAttainment = attainments.Get(11)
								};
								this._doctorSortDataList.Add(data2);
							}
							SortAndFilterController<HealDoctorSortData> doctorSortController = this._doctorSortController;
							if (doctorSortController != null)
							{
								doctorSortController.NotifyDataChanged(this._doctorSortDataList);
							}
							this._selectedDoctor = -1;
							this.SortAndRefreshDoctorList();
							this._patientSortDataList.Clear();
							foreach (int id2 in this._patientList)
							{
								HealPatientSortData data3 = new HealPatientSortData
								{
									CharId = id2,
									TotalInjuries = this._patientInjuryDict.GetOrDefault(id2).GetSum(),
									TotalPoisons = this._patientPoisonDict.GetOrDefault(id2).Sum(),
									QiDisorder = (int)this._patientQiDisorderDict.GetOrDefault(id2),
									HealthPercent = UI_Heal.GetHealthPercent((int)this._patientHealthDict.GetOrDefault(id2), (int)this._patientMaxHealthDict.GetOrDefault(id2)),
									IsHealthUnknownOrNone = this.IsHealthUnknownOrNone(id2, (int)this._patientHealthDict.GetOrDefault(id2), (int)this._patientMaxHealthDict.GetOrDefault(id2))
								};
								this._patientSortDataList.Add(data3);
								this.TryGetGearMateRepairRequirement(id2);
							}
							HealPatientSortAndFilterController patientSortController = this._patientSortController;
							if (patientSortController != null)
							{
								patientSortController.NotifyDataChanged(this._patientSortDataList);
							}
							this._selectedPatient = -1;
							this.RefreshCurrentSelectionDisplay();
							this.RefreshUiByGearMate();
							this.RefreshTitleText();
							this.RefreshHealButtonsForNoSelection();
							SingletonObject.getInstance<YieldHelper>().DelayFrameDo(2U, delegate
							{
								this.SortAndRefreshPatientList();
								bool flag15 = this._doctorSortDataList.Count > 0;
								if (flag15)
								{
									this.OnDoctorChange(this._doctorSortDataList[0].CharId);
								}
								else
								{
									this.RequestSimulateHealCosts(-1, this._taiwuCharId);
								}
								bool flag16 = this._patientSortDataList.Count > 0;
								if (flag16)
								{
									this.OnPatientChange(this._patientSortDataList.Exists((HealPatientSortData d) => d.CharId == this._currentCharacterId) ? this._currentCharacterId : this._patientSortDataList[0].CharId);
								}
								this.Element.ShowAfterRefresh();
							});
						}
					}
					else
					{
						bool flag4 = notification.DomainId == 19;
						if (flag4)
						{
							bool flag5 = notification.MethodId == 143;
							if (flag5)
							{
								List<GearMateRepairRequirementDisplayData> displayDataList2 = null;
								Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref displayDataList2);
								this._gearMateRepairRequirementDict[displayDataList2[0].GearMateId] = displayDataList2;
								this.OnPatientChangeAndGotRepairRequirement();
							}
						}
					}
				}
			}
			else
			{
				DataUid uid = notification.Uid;
				bool flag6 = uid.DomainId == 4 && uid.DataId == 0;
				if (flag6)
				{
					int charId = (int)uid.SubId0;
					bool flag7 = uid.SubId1 == 97U;
					if (flag7)
					{
						LifeSkillShorts attainments2 = default(LifeSkillShorts);
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref attainments2);
						bool expensiveHeal = this._expensiveHeal;
						if (expensiveHeal)
						{
							ref short ptr = ref attainments2.Items.FixedElementField + (IntPtr)8 * 2;
							ptr *= 2;
							ref short ptr2 = ref attainments2.Items.FixedElementField + (IntPtr)9 * 2;
							ptr2 *= 2;
						}
						this._doctorAttainmentDict[charId] = attainments2;
						this.doctorScroll.RefreshCell(this._doctorList.IndexOf(charId));
						this.UpdateDoctorInfo();
					}
					else
					{
						bool flag8 = uid.SubId1 == 26U;
						if (flag8)
						{
							Injuries injuries = default(Injuries);
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref injuries);
							this._patientInjuryDict[charId] = injuries;
							this.RefreshPatientSortData(charId);
							this.patientScroll.RefreshCell(this._patientList.IndexOf(charId));
							this.UpdatePatientInfo();
							this.UpdateHealInjuryBtn(-1);
						}
						else
						{
							bool flag9 = uid.SubId1 == 44U;
							if (flag9)
							{
								PoisonInts poisons = default(PoisonInts);
								Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref poisons);
								this._patientPoisonDict[charId] = poisons;
								this.RefreshPatientSortData(charId);
								this.patientScroll.RefreshCell(this._patientList.IndexOf(charId));
								this.UpdatePatientInfo();
								this.UpdateHealPoisonBtn(-1);
							}
							else
							{
								bool flag10 = uid.SubId1 == 19U;
								if (flag10)
								{
									short health = 0;
									Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref health);
									this._patientHealthDict[charId] = health;
									this.RefreshPatientSortData(charId);
									this.patientScroll.RefreshCell(this._patientList.IndexOf(charId));
									this.UpdatePatientInfo();
									this.UpdateHealHealthBtn(-1);
								}
								else
								{
									uint subId = uid.SubId1;
									bool flag11 = subId == 17U || subId == 55U;
									if (flag11)
									{
										this.RequestPatientAvailableFeature(charId);
									}
									else
									{
										bool flag12 = uid.SubId1 == 21U;
										if (flag12)
										{
											short disorder = 0;
											Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref disorder);
											this._patientQiDisorderDict[charId] = disorder;
											this.RefreshPatientSortData(charId);
											this.patientScroll.RefreshCell(this._patientList.IndexOf(charId));
											this.UpdatePatientInfo();
											this.UpdateHealQiDisorderBtn(-1);
										}
										else
										{
											bool flag13 = uid.SubId1 == 34U;
											if (flag13)
											{
												Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._taiwuResource);
												this.UpdateHealInjuryBtn(-1);
												this.UpdateHealPoisonBtn(-1);
												this.UpdateHealQiDisorderBtn(-1);
												this.UpdateHealHealthBtn(-1);
												this.UpdateDoctorInfo();
												this.UpdatePatientInfo();
											}
										}
									}
								}
							}
						}
					}
				}
				else
				{
					bool flag14 = uid.DomainId == 3 && uid.DataId == 1 && this._displayDataDict.ContainsKey(this._selectedDoctor) && (short)uid.SubId0 == this._displayDataDict[this._selectedDoctor].OrgInfo.SettlementId && uid.SubId1 == 2U;
					if (flag14)
					{
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._doctorOrgLocation);
						this.OnAreaSpiritualDebtChange(null);
						this.UpdateHealInjuryBtn(-1);
						this.UpdateHealPoisonBtn(-1);
						this.UpdateHealQiDisorderBtn(-1);
						this.UpdateHealHealthBtn(-1);
					}
				}
			}
		}
	}

	// Token: 0x0600352A RID: 13610 RVA: 0x001A9BE0 File Offset: 0x001A7DE0
	protected unsafe override void OnClick(Transform btn)
	{
		string btnName = btn.name;
		bool flag = btnName == "BtnHealInjury";
		if (flag)
		{
			AudioManager.Instance.PlaySound("ui_drug_drink", false, false);
			int doctorId = this._selectedDoctor;
			int patientId = this._selectedPatient;
			int payerId = patientId;
			this.ShowHealAnimation(true);
			CombatResources currentResource = this._doctorHealCountDict[doctorId];
			currentResource.HealingCount -= 1;
			this._doctorHealCountDict[doctorId] = currentResource;
			*this._taiwuResource[5] -= this.CurHealInjuryCost.CostHerb;
			bool needPlay = this._needPlay;
			if (needPlay)
			{
				*this._taiwuResource[6] -= this.CurHealInjuryCost.CostMoney;
			}
			TMP_Text tmp_Text = this.healInjuryCount;
			CombatResources combatResources = this._doctorHealCountDict[doctorId];
			tmp_Text.text = combatResources.HealingCount.ToString();
			this.RefreshCountBack(this.healInjuryCount, (int)this._doctorHealCountDict[doctorId].HealingCount);
			this.healInjury.interactable = false;
			this.OnInjuryTriggerChanged(false);
			MapDomainMethod.Call.HealOnMap(this.Element.GameDataListenerId, 0, doctorId, patientId, this._needPlay, payerId, this._expensiveHeal);
			this.RequestSimulateHealCosts(patientId, -1);
			TaiwuEventDomainMethod.Call.SetListenerEventActionBoolArg("HealActionComplete", "HealAction", true);
		}
		else
		{
			bool flag2 = btnName == "BtnHealPoison";
			if (flag2)
			{
				AudioManager.Instance.PlaySound("ui_drug_drink", false, false);
				int doctorId2 = this._selectedDoctor;
				int patientId2 = this._selectedPatient;
				int payerId2 = patientId2;
				this.ShowHealAnimation(false);
				CombatResources currentResource2 = this._doctorHealCountDict[doctorId2];
				currentResource2.DetoxCount -= 1;
				this._doctorHealCountDict[doctorId2] = currentResource2;
				*this._taiwuResource[5] -= this.CurHealPoisonCost.CostHerb;
				bool needPlay2 = this._needPlay;
				if (needPlay2)
				{
					*this._taiwuResource[6] -= this.CurHealPoisonCost.CostMoney;
				}
				TMP_Text tmp_Text2 = this.healPoisonCount;
				CombatResources combatResources = this._doctorHealCountDict[doctorId2];
				tmp_Text2.text = combatResources.DetoxCount.ToString();
				this.RefreshCountBack(this.healPoisonCount, (int)this._doctorHealCountDict[doctorId2].DetoxCount);
				this.healPoison.interactable = false;
				this.OnPoisonTriggerChanged(false);
				MapDomainMethod.Call.HealOnMap(this.Element.GameDataListenerId, 1, doctorId2, patientId2, this._needPlay, payerId2, this._expensiveHeal);
				this.RequestSimulateHealCosts(patientId2, -1);
				TaiwuEventDomainMethod.Call.SetListenerEventActionBoolArg("HealActionComplete", "HealAction", true);
			}
			else
			{
				bool flag3 = btnName == "BtnHealQiDisorder";
				if (flag3)
				{
					AudioManager.Instance.PlaySound("ui_drug_drink", false, false);
					int doctorId3 = this._selectedDoctor;
					int patientId3 = this._selectedPatient;
					int payerId3 = patientId3;
					this.ShowHealAnimation(false);
					CombatResources currentResource3 = this._doctorHealCountDict[doctorId3];
					currentResource3.BreathingCount -= 1;
					this._doctorHealCountDict[doctorId3] = currentResource3;
					*this._taiwuResource[5] -= this.CurHealQiDisorderCost.CostHerb;
					bool needPlay3 = this._needPlay;
					if (needPlay3)
					{
						*this._taiwuResource[6] -= this.CurHealQiDisorderCost.CostMoney;
					}
					TMP_Text tmp_Text3 = this.healQiDisorderCount;
					CombatResources combatResources = this._doctorHealCountDict[doctorId3];
					tmp_Text3.text = combatResources.BreathingCount.ToString();
					this.RefreshCountBack(this.healQiDisorderCount, (int)this._doctorHealCountDict[doctorId3].BreathingCount);
					this.healQiDisorder.interactable = false;
					this.OnQiDisorderPointerChanged(false);
					MapDomainMethod.Call.HealOnMap(this.Element.GameDataListenerId, 2, doctorId3, patientId3, this._needPlay, payerId3, this._expensiveHeal);
					this.RequestSimulateHealCosts(patientId3, -1);
					TaiwuEventDomainMethod.Call.SetListenerEventActionBoolArg("HealActionComplete", "HealAction", true);
				}
				else
				{
					bool flag4 = btnName == "BtnHealHealth";
					if (flag4)
					{
						AudioManager.Instance.PlaySound("ui_drug_drink", false, false);
						int doctorId4 = this._selectedDoctor;
						int patientId4 = this._selectedPatient;
						int payerId4 = patientId4;
						this.ShowHealAnimation(true);
						CombatResources currentResource4 = this._doctorHealCountDict[doctorId4];
						currentResource4.RecoverCount -= 1;
						this._doctorHealCountDict[doctorId4] = currentResource4;
						*this._taiwuResource[5] -= this.CurHealHealthCost.CostHerb;
						bool needPlay4 = this._needPlay;
						if (needPlay4)
						{
							*this._taiwuResource[6] -= this.CurHealHealthCost.CostMoney;
						}
						TMP_Text tmp_Text4 = this.healHealthCount;
						CombatResources combatResources = this._doctorHealCountDict[doctorId4];
						tmp_Text4.text = combatResources.RecoverCount.ToString();
						this.RefreshCountBack(this.healHealthCount, (int)this._doctorHealCountDict[doctorId4].RecoverCount);
						this.healHealth.interactable = false;
						this.OnHealthPointerChanged(false);
						MapDomainMethod.Call.HealOnMap(this.Element.GameDataListenerId, 3, doctorId4, patientId4, this._needPlay, payerId4, this._expensiveHeal);
						this.RequestSimulateHealCosts(patientId4, -1);
						TaiwuEventDomainMethod.Call.SetListenerEventActionBoolArg("HealActionComplete", "HealAction", true);
					}
					else
					{
						bool flag5 = btnName == "CommonButtonCloseLevelTwo";
						if (flag5)
						{
							this.QuickHide();
						}
						else
						{
							bool flag6 = btnName == "BtnHealOutInjury";
							if (flag6)
							{
								AudioManager.Instance.PlaySound("ui_drug_drink", false, false);
								this.RequestHealGearMate(0);
								this.OnOutInjuryTriggerChanged(false, false);
							}
							else
							{
								bool flag7 = btnName == "BtnHealInnerInjury";
								if (flag7)
								{
									AudioManager.Instance.PlaySound("ui_drug_drink", false, false);
									this.RequestHealGearMate(1);
									this.OnInnerInjuryTriggerChanged(false, false);
								}
								else
								{
									bool flag8 = btnName == "BtnGearMateHealPoison";
									if (flag8)
									{
										AudioManager.Instance.PlaySound("ui_drug_drink", false, false);
										this.RequestHealGearMate(2);
										this.OnGearMateHealPoisonPointerChanged(false, false);
									}
									else
									{
										bool flag9 = btnName == "BtnGearMateHealQiDisorder";
										if (flag9)
										{
											AudioManager.Instance.PlaySound("ui_drug_drink", false, false);
											this.RequestHealGearMate(3);
											this.OnGearMateHealQiDisorderPointerChanged(false, false);
										}
										else
										{
											bool flag10 = btnName == "BtnAutoPut";
											if (flag10)
											{
												this.OnClickBtnAutoPut();
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
	}

	// Token: 0x0600352B RID: 13611 RVA: 0x001AA24C File Offset: 0x001A844C
	private void RequestHealGearMate(sbyte repairType)
	{
		bool flag = !this.IsSelectedPatientGearMate;
		if (!flag)
		{
			ItemDisplayData usingTool = this.GetUsingTool(repairType);
			bool flag2 = usingTool == null;
			if (!flag2)
			{
				this.GetGearMateHealButton(repairType).interactable = false;
				ExtraDomainMethod.Call.RepairGearMate(this._selectedPatient, this._selectedDoctor, repairType, usingTool.Key);
				this.TryGetGearMateRepairRequirement(this._selectedPatient);
				this.AsyncGetDoctorGearMateRepairCount(this._selectedDoctor);
				bool isSelectedPatientGearMate = this.IsSelectedPatientGearMate;
				if (isSelectedPatientGearMate)
				{
					bool hadManualSelection = this._itemSelector.SelectedItem != null;
					this._itemSelector.RefreshItemsByCharacter(this._selectedDoctor, delegate
					{
						bool flag3 = hadManualSelection && this._itemSelector.SelectedItem == null;
						if (flag3)
						{
							this.OnSelectItem(null);
							this.AsyncGetRepairResultAndRefreshButtons();
						}
					});
				}
			}
		}
	}

	// Token: 0x0600352C RID: 13612 RVA: 0x001AA314 File Offset: 0x001A8514
	private CButton GetGearMateHealButton(sbyte repairType)
	{
		if (!true)
		{
		}
		CButton result;
		switch (repairType)
		{
		case 0:
			result = this.healOutInjury;
			break;
		case 1:
			result = this.healInnerInjury;
			break;
		case 2:
			result = this.gearMateHealPoison;
			break;
		case 3:
			result = this.gearMateHealQiDisorder;
			break;
		default:
			throw new ArgumentOutOfRangeException("repairType", repairType, null);
		}
		if (!true)
		{
		}
		return result;
	}

	// Token: 0x0600352D RID: 13613 RVA: 0x001AA380 File Offset: 0x001A8580
	private void ShowHealAnimation(bool isInjury)
	{
		this.HideEffectAndAnim();
		if (isInjury)
		{
			this.healAni.gameObject.SetActive(true);
			this.healAni.AnimationState.SetAnimation(0, "animation", false);
		}
		this._closeAnimTime = 0f;
		this.UpdateHealPoisonBtn(SingletonObject.getInstance<TimeManager>().GetRemainingActionPointConvertToDays() - 1);
		this.UpdateHealInjuryBtn(SingletonObject.getInstance<TimeManager>().GetRemainingActionPointConvertToDays() - 1);
		this.UpdateHealQiDisorderBtn(SingletonObject.getInstance<TimeManager>().GetRemainingActionPointConvertToDays() - 1);
		this.UpdateHealHealthBtn(SingletonObject.getInstance<TimeManager>().GetRemainingActionPointConvertToDays() - 1);
	}

	// Token: 0x0600352E RID: 13614 RVA: 0x001AA424 File Offset: 0x001A8624
	private void Update()
	{
		bool flag = this._closeAnimTime >= 0f;
		if (flag)
		{
			this._closeAnimTime += Time.deltaTime;
			bool flag2 = this._closeAnimTime >= 4f;
			if (flag2)
			{
				this.HideEffectAndAnim();
			}
		}
	}

	// Token: 0x0600352F RID: 13615 RVA: 0x001AA477 File Offset: 0x001A8677
	private void HideEffectAndAnim()
	{
		this._closeAnimTime = -1f;
		this.healAni.gameObject.SetActive(false);
		this.UpdateHealInjuryBtn(-1);
		this.UpdateHealPoisonBtn(-1);
	}

	// Token: 0x06003530 RID: 13616 RVA: 0x001AA4A7 File Offset: 0x001A86A7
	public override void QuickHide()
	{
		this.HideEffectAndAnim();
		AudioManager.Instance.PlaySound("ui_default_cancel", false, false);
		base.QuickHide();
	}

	// Token: 0x06003531 RID: 13617 RVA: 0x001AA4CC File Offset: 0x001A86CC
	private void OnRenderDoctor(int index, GameObject charRefers)
	{
		bool flag = !this._doctorSortDataList.CheckIndex(index);
		if (!flag)
		{
			int charId = this._doctorSortDataList[index].CharId;
			CharacterDisplayData charData;
			bool flag2 = !this._displayDataDict.TryGetValue(charId, out charData);
			if (!flag2)
			{
				LifeSkillShorts attainments;
				bool flag3 = !this._doctorAttainmentDict.TryGetValue(charId, out attainments);
				if (!flag3)
				{
					HealChar charItem = charRefers.GetComponent<HealChar>();
					charItem.Init();
					bool isSelectedPatientGearMate = this.IsSelectedPatientGearMate;
					if (isSelectedPatientGearMate)
					{
						GearMateRepairCount healCount = this._doctorHealGearMateCountDict.GetOrDefault(charId);
						Dictionary<sbyte, int> patientData = this._currentGearMateCanRepair;
						charItem.RefreshDoctorForGear(charData, attainments, healCount, patientData);
					}
					else
					{
						CombatResources healCount2 = this._doctorHealCountDict.GetOrDefault(charId);
						HealPatientSortData patientData2 = new HealPatientSortData
						{
							TotalInjuries = this._patientInjuryDict.GetOrDefault(this._selectedPatient).GetSum(),
							TotalPoisons = this._patientPoisonDict.GetOrDefault(this._selectedPatient).Sum(),
							QiDisorder = (int)this._patientQiDisorderDict.GetOrDefault(this._selectedPatient),
							HealthPercent = (int)this._patientHealthDict.GetOrDefault(this._selectedPatient)
						};
						charItem.RefreshDoctorForNormal(charData, attainments, healCount2, patientData2);
					}
					bool isSelected = charId == this._selectedDoctor;
					charItem.SetIsSelected(isSelected);
					charItem.SetOnClick(delegate
					{
						this.OnDoctorChange(charId);
						this.doctorScroll.ReRender();
					});
				}
			}
		}
	}

	// Token: 0x06003532 RID: 13618 RVA: 0x001AA668 File Offset: 0x001A8868
	private void OnDoctorChange(int charId)
	{
		this._selectedDoctor = charId;
		this.TryAppendDoctorLocationMonitorField(charId);
		this.RefreshCurrentSelectionDisplay();
		this.RefreshUiByGearMate();
		this.UpdateDoctorInfo();
		this.HideEffectAndAnim();
		int doctorId = this._selectedDoctor;
		this.patientScroll.ReRender();
		this.RequestSimulateHealCosts(-1, -1);
		this.AsyncGetDoctorGearMateRepairCount(doctorId);
		bool isSelectedPatientGearMate = this.IsSelectedPatientGearMate;
		if (isSelectedPatientGearMate)
		{
			this._itemSelector.Clear();
			this.RefreshGearMateToolItemView(null, false);
			this._itemSelector.RefreshItemsByCharacter(this._selectedDoctor, new Action(this.AsyncGetRepairResultAndRefreshButtons));
		}
		bool flag = !this.HasSelectedPatient();
		if (flag)
		{
			this.RefreshHealButtonsForNoSelection();
		}
		else
		{
			this.RefreshAutoPutButton();
		}
	}

	// Token: 0x06003533 RID: 13619 RVA: 0x001AA725 File Offset: 0x001A8925
	private void AsyncGetDoctorGearMateRepairCount(int doctorId)
	{
		ExtraDomainMethod.AsyncCall.GetGearMateAvailableRepairCount(this, doctorId, delegate(int offset, RawDataPool pool)
		{
			GearMateRepairCount count = new GearMateRepairCount();
			Serializer.Deserialize(pool, offset, ref count);
			this._doctorHealGearMateCountDict[this._selectedDoctor] = count;
			this.RefreshGearMateButtonHealCount();
			for (sbyte repairType = 0; repairType <= 3; repairType += 1)
			{
				int healResult;
				bool flag = this._currentGearMateCanRepair.TryGetValue(repairType, out healResult);
				if (flag)
				{
					this.RefreshGearMateHealButton(repairType, healResult);
				}
			}
		});
	}

	// Token: 0x06003534 RID: 13620 RVA: 0x001AA73C File Offset: 0x001A893C
	private void RefreshGearMateButtonHealCount()
	{
		sbyte outerInjuryHealingCount = this._doctorHealGearMateCountDict[this._selectedDoctor].OuterInjuryHealingCount;
		this.healOutInjuryCount.text = outerInjuryHealingCount.ToString();
		this.RefreshCountBack(this.healOutInjuryCount, (int)outerInjuryHealingCount);
		sbyte innerInjuryHealingCount = this._doctorHealGearMateCountDict[this._selectedDoctor].InnerInjuryHealingCount;
		this.healInnerInjuryCount.text = innerInjuryHealingCount.ToString();
		this.RefreshCountBack(this.healInnerInjuryCount, (int)innerInjuryHealingCount);
		sbyte detoxCount = this._doctorHealGearMateCountDict[this._selectedDoctor].DetoxCount;
		this.healGearMatePoisonCount.text = detoxCount.ToString();
		this.RefreshCountBack(this.healGearMatePoisonCount, (int)detoxCount);
		sbyte breathingCount = this._doctorHealGearMateCountDict[this._selectedDoctor].BreathingCount;
		this.healGearMateQiDisorderCount.text = breathingCount.ToString();
		this.RefreshCountBack(this.healGearMateQiDisorderCount, (int)breathingCount);
	}

	// Token: 0x06003535 RID: 13621 RVA: 0x001AA82C File Offset: 0x001A8A2C
	private void RefreshCharInfoLifeSkill(sbyte lifeSkillType, bool isPreview = false, bool isShowAutoSelectedToolAttainment = false, UI_Heal.EHealType healType = UI_Heal.EHealType.Invalid)
	{
		bool isGearNeedLifeSkill = lifeSkillType != 8 && lifeSkillType != 9;
		bool isSelectedPatientGearMate = this.IsSelectedPatientGearMate;
		if (isSelectedPatientGearMate)
		{
			bool flag = !this._gearMateRepairRequirementDict.ContainsKey(this._selectedPatient) || !isGearNeedLifeSkill;
			if (flag)
			{
				return;
			}
		}
		else
		{
			bool flag2 = isGearNeedLifeSkill;
			if (flag2)
			{
				return;
			}
		}
		if (!true)
		{
		}
		int num;
		switch (lifeSkillType)
		{
		case 6:
			num = 0;
			break;
		case 7:
			num = 1;
			break;
		case 8:
			num = 0;
			break;
		case 9:
			num = 1;
			break;
		case 10:
			num = 3;
			break;
		case 11:
			num = 2;
			break;
		default:
			throw new ArgumentOutOfRangeException("lifeSkillType", lifeSkillType, null);
		}
		if (!true)
		{
		}
		int index = num;
		Transform grid = this.healCurrDoctor.grid;
		PropertyItem gridChild = grid.GetChild(index).GetComponent<PropertyItem>();
		LifeSkillTypeItem lifeSkillConfig = Config.LifeSkillType.Instance[lifeSkillType];
		LifeSkillShorts attainments = this._doctorAttainmentDict.GetValueOrDefault(this._selectedDoctor);
		sbyte gearMateRepairType = -1;
		bool isSelectedPatientGearMate2 = this.IsSelectedPatientGearMate;
		short attainment;
		int needAttainment;
		if (isSelectedPatientGearMate2)
		{
			gearMateRepairType = UI_Heal.GetRepairTypeFromLifeSkillType(lifeSkillType);
			attainment = this.GetDoctorAttainment(attainments, gearMateRepairType, isShowAutoSelectedToolAttainment);
			needAttainment = this._gearMateRepairRequirementDict[this._selectedPatient][(int)gearMateRepairType].AttainmentCount;
			this.RefreshGearMateDoctorAttainmentTip(gridChild.Tip, lifeSkillType);
		}
		else
		{
			if (!true)
			{
			}
			switch (healType)
			{
			case UI_Heal.EHealType.Injury:
				num = this.CurHealInjuryCost.MaxRequireAttainment;
				goto IL_1F1;
			case UI_Heal.EHealType.Poison:
				num = this.CurHealPoisonCost.MaxRequireAttainment;
				goto IL_1F1;
			case UI_Heal.EHealType.QiDisorder:
				num = this.CurHealQiDisorderCost.MaxRequireAttainment;
				goto IL_1F1;
			case UI_Heal.EHealType.Health:
				num = this.CurHealHealthCost.MaxRequireAttainment;
				goto IL_1F1;
			}
			if (!true)
			{
			}
			int maxRequireAttainment;
			if (lifeSkillType != 8)
			{
				if (lifeSkillType != 9)
				{
					throw new ArgumentOutOfRangeException("lifeSkillType", lifeSkillType, null);
				}
				maxRequireAttainment = this.CurHealPoisonCost.MaxRequireAttainment;
			}
			else
			{
				maxRequireAttainment = this.CurHealInjuryCost.MaxRequireAttainment;
			}
			if (!true)
			{
			}
			num = maxRequireAttainment;
			IL_1F1:
			if (!true)
			{
			}
			needAttainment = num;
			attainment = attainments.Get((int)lifeSkillType);
		}
		string color = ((int)attainment >= needAttainment) ? "brightblue" : "brightred";
		bool flag3 = isPreview && this._previewToolOverride != null && this.IsSelectedPatientGearMate;
		string content;
		if (flag3)
		{
			ItemDisplayData savedOverride = this._previewToolOverride;
			this._previewToolOverride = null;
			short currentDisplayAttainment = this.GetDoctorAttainment(attainments, gearMateRepairType, false);
			this._previewToolOverride = savedOverride;
			int delta = (int)(attainment - currentDisplayAttainment);
			string curText = currentDisplayAttainment.ToString();
			string deltaText = (delta > 0) ? string.Format("+{0}", delta).SetColor("brightblue") : ((delta < 0) ? string.Format("{0}", delta).SetColor("brightred") : null);
			content = ((deltaText != null) ? (curText + deltaText) : curText);
		}
		else if (isPreview)
		{
			string curText2 = attainment.ToString().SetColor(color);
			string needText = (needAttainment == int.MaxValue) ? "-" : needAttainment.ToString();
			content = curText2 + "/" + needText;
		}
		else
		{
			content = attainment.ToString();
		}
		gridChild.Set(lifeSkillConfig.Icon, lifeSkillConfig.Name, content, null, true);
		GameObject highlightArea = grid.GetChild(index).GetComponent<HealCurrDoctorGrid>().hover;
		highlightArea.SetActive(isPreview);
	}

	// Token: 0x06003536 RID: 13622 RVA: 0x001AAB90 File Offset: 0x001A8D90
	private void UpdateDoctorInfo()
	{
		int doctorId = this._selectedDoctor;
		this.healCurrDoctor.avatarBack.SetSprite((!this.HasSelectedDoctor()) ? "ui9_back_heal_player_0" : (this.IsSelectedPatientGearMate ? "ui9_back_heal_player_4" : "ui9_back_heal_player_2"), false, null);
		bool flag = !this.HasSelectedDoctor();
		if (!flag)
		{
			CharacterDisplayData charData;
			bool flag2 = !this._displayDataDict.TryGetValue(doctorId, out charData);
			if (!flag2)
			{
				string charName = NameCenter.GetMonasticTitleOrDisplayName(charData, doctorId == this._taiwuCharId);
				LifeSkillShorts attainments = this._doctorAttainmentDict.GetValueOrDefault(doctorId);
				this.healCurrDoctor.avatar.Refresh(charData, true);
				this.healCurrDoctor.nameFrame.SetName(charName);
				Transform grid = this.healCurrDoctor.grid;
				bool isSelectedPatientGearMate = this.IsSelectedPatientGearMate;
				int lifeSkillCount;
				if (isSelectedPatientGearMate)
				{
					this.RefreshCostResource(2, UI_Heal.EHealType.Invalid, false);
					this.RefreshCostResource(1, UI_Heal.EHealType.Invalid, false);
					this.RefreshCostResource(3, UI_Heal.EHealType.Invalid, false);
					this.RefreshCostResource(4, UI_Heal.EHealType.Invalid, false);
					this.RefreshCharInfoLifeSkill(6, false, false, UI_Heal.EHealType.Invalid);
					this.RefreshCharInfoLifeSkill(7, false, false, UI_Heal.EHealType.Invalid);
					this.RefreshCharInfoLifeSkill(11, false, false, UI_Heal.EHealType.Invalid);
					this.RefreshCharInfoLifeSkill(10, false, false, UI_Heal.EHealType.Invalid);
					lifeSkillCount = 4;
				}
				else
				{
					this.RefreshCostResource(5, UI_Heal.EHealType.Invalid, false);
					bool needPlay = this._needPlay;
					if (needPlay)
					{
						this.RefreshCostResource(6, UI_Heal.EHealType.Invalid, false);
					}
					this.RefreshCharInfoLifeSkill(8, false, false, UI_Heal.EHealType.Invalid);
					this.RefreshCharInfoLifeSkill(9, false, false, UI_Heal.EHealType.Invalid);
					lifeSkillCount = 2;
				}
				for (int i = 0; i < lifeSkillCount; i++)
				{
					grid.GetChild(i).gameObject.SetActive(true);
				}
				for (int j = lifeSkillCount; j < grid.childCount; j++)
				{
					grid.GetChild(j).gameObject.SetActive(false);
				}
			}
		}
	}

	// Token: 0x06003537 RID: 13623 RVA: 0x001AAD60 File Offset: 0x001A8F60
	private unsafe short GetDoctorAttainment(LifeSkillShorts attainments, sbyte repairType, bool isShowAutoSelectedToolAttainment = false)
	{
		sbyte lifeSkillType = UI_Heal.GetLifeSkillTypeFromGearMateRepairType(repairType);
		short attainment = *attainments[(int)lifeSkillType];
		ItemDisplayData usingItem = this.GetUsingTool(repairType);
		bool flag = usingItem != null;
		if (flag)
		{
			short toolAttainment = UI_Make.GetToolAttainment(usingItem.Key.TemplateId, attainment, lifeSkillType);
			bool typeMeet = CraftTool.Instance[usingItem.Key.TemplateId].RequiredLifeSkillTypes.Contains(lifeSkillType);
			bool flag2 = typeMeet && (!this.IsUsingToolAutoSelected || isShowAutoSelectedToolAttainment);
			if (flag2)
			{
				attainment += toolAttainment;
			}
		}
		return attainment;
	}

	// Token: 0x06003538 RID: 13624 RVA: 0x001AADEC File Offset: 0x001A8FEC
	private static sbyte GetLifeSkillTypeFromGearMateRepairType(sbyte repairType)
	{
		if (!true)
		{
		}
		sbyte result;
		switch (repairType)
		{
		case 0:
			result = 6;
			break;
		case 1:
			result = 7;
			break;
		case 2:
			result = 11;
			break;
		case 3:
			result = 10;
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		if (!true)
		{
		}
		return result;
	}

	// Token: 0x06003539 RID: 13625 RVA: 0x001AAE38 File Offset: 0x001A9038
	private static sbyte GetRepairTypeFromLifeSkillType(sbyte lifeSkillType)
	{
		if (!true)
		{
		}
		sbyte result;
		switch (lifeSkillType)
		{
		case 6:
			result = 0;
			goto IL_3D;
		case 7:
			result = 1;
			goto IL_3D;
		case 10:
			result = 3;
			goto IL_3D;
		case 11:
			result = 2;
			goto IL_3D;
		}
		throw new ArgumentOutOfRangeException();
		IL_3D:
		if (!true)
		{
		}
		return result;
	}

	// Token: 0x0600353A RID: 13626 RVA: 0x001AAE8C File Offset: 0x001A908C
	private unsafe void OnRenderPatient(int index, GameObject charRefers)
	{
		bool flag = !this._patientSortDataList.CheckIndex(index);
		if (!flag)
		{
			int charId = this._patientSortDataList[index].CharId;
			CharacterDisplayData charData;
			bool flag2 = !this._displayDataDict.TryGetValue(charId, out charData);
			if (!flag2)
			{
				Injuries injuries;
				bool flag3 = !this._patientInjuryDict.TryGetValue(charId, out injuries);
				if (!flag3)
				{
					PoisonInts poisons;
					bool flag4 = !this._patientPoisonDict.TryGetValue(charId, out poisons);
					if (!flag4)
					{
						short health;
						bool flag5 = !this._patientHealthDict.TryGetValue(charId, out health);
						if (!flag5)
						{
							short maxHealth;
							bool flag6 = !this._patientMaxHealthDict.TryGetValue(charId, out maxHealth);
							if (!flag6)
							{
								short qiDisorder;
								bool flag7 = !this._patientQiDisorderDict.TryGetValue(charId, out qiDisorder);
								if (!flag7)
								{
									HealChar charItem = charRefers.GetComponent<HealChar>();
									charItem.Init();
									bool isSelected = charId == this._selectedPatient;
									charItem.SetIsSelected(isSelected);
									LifeSkillShorts ownAttainments = this._doctorAttainmentDict.GetOrDefault(this._selectedDoctor);
									bool isGearMate = this.IsGearMate(charId);
									bool flag8 = isGearMate;
									if (flag8)
									{
										LifeSkillShorts needAttainments = default(LifeSkillShorts);
										needAttainments.Initialize();
										LifeSkillShorts ownAttainmentsWithTool = default(LifeSkillShorts);
										ownAttainmentsWithTool.Initialize();
										for (sbyte repairType = 0; repairType <= 3; repairType += 1)
										{
											sbyte lifeSkillType = UI_Heal.GetLifeSkillTypeFromGearMateRepairType(repairType);
											sbyte gearMateRepairType = UI_Heal.GetRepairTypeFromLifeSkillType(lifeSkillType);
											short attainment = this.GetDoctorAttainment(ownAttainments, gearMateRepairType, true);
											List<GearMateRepairRequirementDisplayData> orDefault = this._gearMateRepairRequirementDict.GetOrDefault(charId);
											int? num;
											if (orDefault == null)
											{
												num = null;
											}
											else
											{
												GearMateRepairRequirementDisplayData orDefault2 = orDefault.GetOrDefault((int)gearMateRepairType);
												num = ((orDefault2 != null) ? new int?(orDefault2.AttainmentCount) : null);
											}
											int? num2 = num;
											int needAttainment = num2.GetValueOrDefault();
											*needAttainments[(int)lifeSkillType] = (short)needAttainment;
											*ownAttainmentsWithTool[(int)lifeSkillType] = attainment;
										}
										charItem.RefreshPatientForGear(charData, injuries, poisons, qiDisorder, needAttainments, ownAttainmentsWithTool);
									}
									else
									{
										Dictionary<EHealActionType, int> dictionary = new Dictionary<EHealActionType, int>();
										dictionary[EHealActionType.Healing] = this._healInjuryCostDict.GetOrDefault(charId).MaxRequireAttainment;
										dictionary[EHealActionType.Detox] = this._healPoisonCostDict.GetOrDefault(charId).MaxRequireAttainment;
										dictionary[EHealActionType.Breathing] = this._healQiDisorderCostDict.GetOrDefault(charId).MaxRequireAttainment;
										dictionary[EHealActionType.Recover] = this._healHealthCostDict.GetOrDefault(charId).MaxRequireAttainment;
										Dictionary<EHealActionType, int> needAttainments2 = dictionary;
										int healthPercent = UI_Heal.GetHealthPercent((int)health, (int)maxHealth);
										bool useDefaultColor = !this.HasSelectedDoctor();
										charItem.RefreshPatientForNormal(charData, injuries, poisons, qiDisorder, healthPercent, needAttainments2, ownAttainments, useDefaultColor);
									}
									bool flag9 = !isSelected;
									if (flag9)
									{
										charItem.SetIsSelected(isSelected);
									}
									charItem.SetOnClick(delegate
									{
										this.OnPatientChange(charId);
										this.patientScroll.ReRender();
										this.doctorScroll.ReRender();
									});
								}
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x0600353B RID: 13627 RVA: 0x001AB1A8 File Offset: 0x001A93A8
	private void OnPatientChange(int charId)
	{
		this._selectedPatient = charId;
		this.RefreshCurrentSelectionDisplay();
		this.UpdatePatientInfo();
		this.UpdateDoctorInfo();
		bool flag = this.HasSelectedDoctor() && this.IsSelectedPatientGearMate;
		if (flag)
		{
			bool flag2 = !this.itemSelectorRefers.gameObject.activeSelf;
			if (flag2)
			{
				this._itemSelector.RefreshItemsByCharacter(this._selectedDoctor, null);
			}
			this.AsyncGetRepairResultAndRefreshButtons();
		}
		else
		{
			bool flag3 = !this.HasSelectedDoctor();
			if (flag3)
			{
				this._itemSelector.Clear();
				this.RefreshGearMateToolItemView(null, false);
				this.RefreshHealButtonsForNoSelection();
			}
		}
		this.RefreshUiByGearMate();
		this.HideEffectAndAnim();
		bool flag4 = this.HasSelectedDoctor();
		if (flag4)
		{
			this.RequestSimulateHealCosts(-1, -1);
		}
		this.RefreshTitleText();
		this.RefreshDate();
		this.RefreshDoctorSortMenu();
	}

	// Token: 0x0600353C RID: 13628 RVA: 0x001AB280 File Offset: 0x001A9480
	private void RefreshDoctorSortMenu()
	{
		bool isSelectedPatientGearMate = this.IsSelectedPatientGearMate;
		if (isSelectedPatientGearMate)
		{
			this._doctorSortController = new HealGearMateDoctorSortAndFilterController(this.doctorSortAndFilter);
		}
		else
		{
			this._doctorSortController = new HealDoctorSortAndFilterController(this.doctorSortAndFilter);
		}
		this._doctorSortController.Init(new Action(this.OnDoctorSortDataListChanged), this.IsSelectedPatientGearMate ? "HealGearMateDoctorSort" : "HealDoctorSort");
		this.SortAndRefreshDoctorList();
	}

	// Token: 0x0600353D RID: 13629 RVA: 0x001AB2F4 File Offset: 0x001A94F4
	private void TryAppendDoctorLocationMonitorField(int doctorId)
	{
		CharacterDisplayData doctorData;
		bool flag = doctorId < 0 || !this._displayDataDict.TryGetValue(doctorId, out doctorData);
		if (!flag)
		{
			base.AppendMonitorFieldId(new UIBase.MonitorDataField(3, 1, (ulong)doctorData.OrgInfo.SettlementId, new uint[]
			{
				2U
			}));
		}
	}

	// Token: 0x0600353E RID: 13630 RVA: 0x001AB344 File Offset: 0x001A9544
	private void RequestSimulateHealCosts(int targetPatientId = -1, int doctorId = -1)
	{
		UI_Heal.<>c__DisplayClass110_0 CS$<>8__locals1 = new UI_Heal.<>c__DisplayClass110_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.doctorId = doctorId;
		int actualDoctorId = (CS$<>8__locals1.doctorId >= 0) ? CS$<>8__locals1.doctorId : this._selectedDoctor;
		using (List<int>.Enumerator enumerator = this._patientList.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				UI_Heal.<>c__DisplayClass110_1 CS$<>8__locals2 = new UI_Heal.<>c__DisplayClass110_1();
				CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
				CS$<>8__locals2.patientId = enumerator.Current;
				UI_Heal.<>c__DisplayClass110_2 CS$<>8__locals3 = new UI_Heal.<>c__DisplayClass110_2();
				CS$<>8__locals3.CS$<>8__locals2 = CS$<>8__locals2;
				bool flag = this.IsGearMate(CS$<>8__locals3.CS$<>8__locals2.patientId) || (targetPatientId >= 0 && targetPatientId != CS$<>8__locals3.CS$<>8__locals2.patientId);
				if (!flag)
				{
					CS$<>8__locals3.isSelectedPatient = (CS$<>8__locals3.CS$<>8__locals2.patientId == this._selectedPatient);
					MapDomainMethod.AsyncCall.SimulateHealCost(this, 0, actualDoctorId, CS$<>8__locals3.CS$<>8__locals2.patientId, this._needPlay, this._expensiveHeal, new AsyncMethodCallbackDelegate(CS$<>8__locals3.<RequestSimulateHealCosts>g__Callback|0));
					MapDomainMethod.AsyncCall.SimulateHealCost(this, 1, actualDoctorId, CS$<>8__locals3.CS$<>8__locals2.patientId, this._needPlay, this._expensiveHeal, new AsyncMethodCallbackDelegate(CS$<>8__locals3.<RequestSimulateHealCosts>g__Callback|0));
					MapDomainMethod.AsyncCall.SimulateHealCost(this, 2, actualDoctorId, CS$<>8__locals3.CS$<>8__locals2.patientId, this._needPlay, this._expensiveHeal, new AsyncMethodCallbackDelegate(CS$<>8__locals3.<RequestSimulateHealCosts>g__Callback|0));
					MapDomainMethod.AsyncCall.SimulateHealCost(this, 3, actualDoctorId, CS$<>8__locals3.CS$<>8__locals2.patientId, this._needPlay, this._expensiveHeal, new AsyncMethodCallbackDelegate(CS$<>8__locals3.<RequestSimulateHealCosts>g__Callback|0));
				}
			}
		}
	}

	// Token: 0x0600353F RID: 13631 RVA: 0x001AB504 File Offset: 0x001A9704
	private void RefreshTitleText()
	{
		this.doctorTitle.text = LocalStringManager.Get(this.IsSelectedPatientGearMate ? LanguageKey.LK_Doctor_GearMatePatient_Select : LanguageKey.LK_Doctor_Select);
		this.patientTitle.text = LocalStringManager.Get(this.IsSelectedPatientGearMate ? LanguageKey.LK_Patient_GearMatePatient_Select : LanguageKey.LK_Patient_Select);
		this.doctorTitle2.text = LocalStringManager.Get(this.IsSelectedPatientGearMate ? LanguageKey.LK_Doctor_GearMatePatient : LanguageKey.LK_Doctor);
		this.patientTitle2.text = LocalStringManager.Get(this.IsSelectedPatientGearMate ? LanguageKey.LK_Patient_GearMatePatient : LanguageKey.LK_Patient);
	}

	// Token: 0x06003540 RID: 13632 RVA: 0x001AB5A8 File Offset: 0x001A97A8
	private void TryGetGearMateRepairRequirement(int patientId)
	{
		bool flag = this.IsGearMate(patientId);
		if (flag)
		{
			ExtraDomainMethod.Call.GetGearMateRepairRequirementDisplayDatas(this.Element.GameDataListenerId, patientId);
		}
	}

	// Token: 0x06003541 RID: 13633 RVA: 0x001AB5D5 File Offset: 0x001A97D5
	private void OnPatientChangeAndGotRepairRequirement()
	{
		this.AsyncGetRepairResultAndRefreshButtons();
	}

	// Token: 0x06003542 RID: 13634 RVA: 0x001AB5E0 File Offset: 0x001A97E0
	private void AsyncGetRepairResultAndRefreshButtons()
	{
		bool flag = !this.IsSelectedPatientGearMate;
		if (!flag)
		{
			for (sbyte repairType = 0; repairType <= 3; repairType += 1)
			{
				bool flag2 = this._itemSelector.SelectedItem == null;
				if (flag2)
				{
					this.AutoSelectItem(repairType);
					this.UpdateDoctorInfo();
				}
				this.RefreshGearMateToolDurability();
				ItemDisplayData itemDisplayData = this.GetUsingTool(repairType);
				bool flag3 = itemDisplayData == null;
				if (flag3)
				{
					return;
				}
				ItemKey toolKey = itemDisplayData.Key;
				sbyte t = repairType;
				ExtraDomainMethod.AsyncCall.GetGearMateRepairRequirement(this, this._selectedPatient, this._selectedDoctor, repairType, toolKey, delegate(int offset, RawDataPool pool)
				{
					int result = -1;
					Serializer.Deserialize(pool, offset, ref result);
					this.RefreshGearMateHealButton(t, result);
					this._currentGearMateCanRepair[t] = result;
				});
				ExtraDomainMethod.AsyncCall.GetGearMateRepairEffect(this, this._selectedPatient, this._selectedDoctor, repairType, toolKey, delegate(int offset, RawDataPool pool)
				{
					int count = 0;
					Serializer.Deserialize(pool, offset, ref count);
					this._currentGearMateRepairEffect[t] = count;
				});
			}
			this.RefreshAutoPutButton();
		}
	}

	// Token: 0x06003543 RID: 13635 RVA: 0x001AB6C8 File Offset: 0x001A98C8
	private unsafe void AutoSelectItem(sbyte repairType)
	{
		List<ItemDisplayData> items = new List<ItemDisplayData>();
		items.AddRange(this._itemSelector.ShowingItems);
		List<ItemDisplayData> showingItems = this._itemSelector.ShowingItems;
		bool flag = showingItems != null && showingItems.Count == 0;
		if (flag)
		{
			ItemDisplayData emptyItem = new ItemDisplayData
			{
				Key = this._emptyToolKey
			};
			items.Add(emptyItem);
		}
		List<GearMateRepairRequirementDisplayData> requireItemList;
		bool flag2 = !this._gearMateRepairRequirementDict.TryGetValue(this._selectedPatient, out requireItemList);
		if (!flag2)
		{
			GearMateRepairRequirementDisplayData requireItem = requireItemList[(int)repairType];
			sbyte needLifeSkillType = requireItem.LifeSkillType;
			int needLifeSkillAttainment = requireItem.AttainmentCount;
			short hasAttainment = *this._doctorAttainmentDict[this._selectedDoctor][(int)needLifeSkillType];
			items.RemoveAll((ItemDisplayData d) => d.Key.ItemType != 6 || !CraftTool.Instance[d.Key.TemplateId].RequiredLifeSkillTypes.Contains(needLifeSkillType));
			items.Sort(new Comparison<ItemDisplayData>(this.CompareByGrade));
			bool found = false;
			foreach (ItemDisplayData item in items)
			{
				short toolAttainment = UI_Make.GetToolAttainment(item.Key.TemplateId, hasAttainment, needLifeSkillType);
				bool flag3 = (int)(hasAttainment + toolAttainment) >= needLifeSkillAttainment;
				if (flag3)
				{
					this._currentGearMateRepairAutoTool[repairType] = item;
					found = true;
					break;
				}
			}
			bool flag4 = !found;
			if (flag4)
			{
				Dictionary<sbyte, ItemDisplayData> currentGearMateRepairAutoTool = this._currentGearMateRepairAutoTool;
				List<ItemDisplayData> list = items;
				currentGearMateRepairAutoTool[repairType] = list[list.Count - 1];
			}
		}
	}

	// Token: 0x06003544 RID: 13636 RVA: 0x001AB864 File Offset: 0x001A9A64
	private void RefreshAutoPutButton()
	{
		bool flag = this.btnAutoPut == null;
		if (!flag)
		{
			bool flag2 = !this.IsSelectedPatientGearMate || !this.HasSelectedDoctor();
			if (flag2)
			{
				this.btnAutoPut.interactable = false;
			}
			else
			{
				ValueTuple<sbyte, sbyte> bothSum = this._patientInjuryDict.GetOrDefault(this._selectedPatient).GetBothSum();
				PoisonInts poisons = this._patientPoisonDict.GetOrDefault(this._selectedPatient);
				short qiDisorder = this._patientQiDisorderDict.GetOrDefault(this._selectedPatient);
				bool hasAnyWound = bothSum.Item1 > 0 || bothSum.Item2 > 0 || poisons.IsNonZero() || qiDisorder > 0;
				this.btnAutoPut.interactable = hasAnyWound;
			}
		}
	}

	// Token: 0x06003545 RID: 13637 RVA: 0x001AB924 File Offset: 0x001A9B24
	private void OnClickBtnAutoPut()
	{
		List<ItemDisplayData> items = new List<ItemDisplayData>();
		ValueTuple<sbyte, sbyte> bothSum = this._patientInjuryDict.GetOrDefault(this._selectedPatient).GetBothSum();
		PoisonInts poisons = this._patientPoisonDict.GetOrDefault(this._selectedPatient);
		short qiDisorder = this._patientQiDisorderDict.GetOrDefault(this._selectedPatient);
		foreach (object obj in Enum.GetValues(typeof(UI_Heal.EHealType)))
		{
			UI_Heal.EHealType healType = (UI_Heal.EHealType)obj;
			if (!true)
			{
			}
			int num;
			switch (healType)
			{
			case UI_Heal.EHealType.OuterInjury:
				num = (int)bothSum.Item1;
				break;
			case UI_Heal.EHealType.InnerInjury:
				num = (int)bothSum.Item2;
				break;
			case UI_Heal.EHealType.Poison:
				num = poisons.Sum();
				break;
			case UI_Heal.EHealType.QiDisorder:
				num = (int)(qiDisorder / 10);
				break;
			default:
				num = 0;
				break;
			}
			if (!true)
			{
			}
			int value = num;
			bool flag = value <= 0;
			if (!flag)
			{
				if (!true)
				{
				}
				sbyte b;
				switch (healType)
				{
				case UI_Heal.EHealType.OuterInjury:
					b = 0;
					break;
				case UI_Heal.EHealType.InnerInjury:
					b = 1;
					break;
				case UI_Heal.EHealType.Poison:
					b = 2;
					break;
				case UI_Heal.EHealType.QiDisorder:
					b = 3;
					break;
				default:
					throw new ArgumentOutOfRangeException();
				}
				if (!true)
				{
				}
				sbyte repairType = b;
				ItemDisplayData item;
				bool flag2 = this._currentGearMateRepairAutoTool.TryGetValue(repairType, out item);
				if (flag2)
				{
					items.Add(item);
				}
			}
		}
		bool flag3 = items.Count <= 0;
		if (!flag3)
		{
			items.Sort(new Comparison<ItemDisplayData>(this.CompareByGrade));
			ItemDisplayData selectItem = items[0];
			this._itemSelector.SetSelectedItem(selectItem);
			this.OnSelectItem(selectItem);
			this._itemSelector.RefreshScrollView();
		}
	}

	// Token: 0x06003546 RID: 13638 RVA: 0x001ABAF0 File Offset: 0x001A9CF0
	private int CompareByGrade(ItemDisplayData x, ItemDisplayData y)
	{
		bool isXEmptyTool = x.Key == this._emptyToolKey;
		bool isYEmptyTool = y.Key == this._emptyToolKey;
		bool flag = isXEmptyTool && !isYEmptyTool;
		int result;
		if (flag)
		{
			result = -1;
		}
		else
		{
			bool flag2 = !isXEmptyTool && isYEmptyTool;
			if (flag2)
			{
				result = 1;
			}
			else
			{
				result = CraftTool.Instance[x.Key.TemplateId].Grade.CompareTo(CraftTool.Instance[y.Key.TemplateId].Grade);
			}
		}
		return result;
	}

	// Token: 0x06003547 RID: 13639 RVA: 0x001ABB84 File Offset: 0x001A9D84
	private void RefreshGearMateHealButton(sbyte repairType, int result)
	{
		GearMateRepairCount leftCounts = this._doctorHealGearMateCountDict[this._selectedDoctor];
		if (!true)
		{
		}
		ValueTuple<CButton, TextMeshProUGUI, LanguageKey, sbyte, sbyte, LanguageKey> valueTuple;
		switch (repairType)
		{
		case 0:
			valueTuple = new ValueTuple<CButton, TextMeshProUGUI, LanguageKey, sbyte, sbyte, LanguageKey>(this.healOutInjury, this.healOutInjuryText, LanguageKey.LK_Heal_OutInjury, leftCounts.OuterInjuryHealingCount, 2, LanguageKey.LK_Heal_GearMate_OutInjury_Disable_Tips_NoInjury);
			break;
		case 1:
			valueTuple = new ValueTuple<CButton, TextMeshProUGUI, LanguageKey, sbyte, sbyte, LanguageKey>(this.healInnerInjury, this.healInnerInjuryText, LanguageKey.LK_Heal_InnerInjury, leftCounts.InnerInjuryHealingCount, 1, LanguageKey.LK_Heal_GearMate_OutInjury_Disable_Tips_NoInjury);
			break;
		case 2:
			valueTuple = new ValueTuple<CButton, TextMeshProUGUI, LanguageKey, sbyte, sbyte, LanguageKey>(this.gearMateHealPoison, this.healGearMatePoisonText, LanguageKey.LK_Heal_Poison, leftCounts.DetoxCount, 3, LanguageKey.LK_Heal_Poison_Disable_Tips_NoPoison);
			break;
		case 3:
			valueTuple = new ValueTuple<CButton, TextMeshProUGUI, LanguageKey, sbyte, sbyte, LanguageKey>(this.gearMateHealQiDisorder, this.healGearMateQiDisorderText, LanguageKey.LK_Heal_QiDisorder, leftCounts.BreathingCount, 4, LanguageKey.LK_Heal_QiDisorder_Disable_Tips_NoQiDisorder);
			break;
		default:
			throw new ArgumentOutOfRangeException("repairType", repairType, null);
		}
		if (!true)
		{
		}
		ValueTuple<CButton, TextMeshProUGUI, LanguageKey, sbyte, sbyte, LanguageKey> valueTuple2 = valueTuple;
		CButton button = valueTuple2.Item1;
		TextMeshProUGUI buttonText = valueTuple2.Item2;
		LanguageKey textKey = valueTuple2.Item3;
		sbyte leftCount = valueTuple2.Item4;
		sbyte needResourceType = valueTuple2.Item5;
		LanguageKey noWoundKey = valueTuple2.Item6;
		TooltipInvoker tipDisplayer = button.GetComponent<TooltipInvoker>();
		tipDisplayer.enabled = (result != 0);
		TooltipInvoker tooltipInvoker = tipDisplayer;
		if (tooltipInvoker.RuntimeParam == null)
		{
			tooltipInvoker.RuntimeParam = new ArgumentBox();
		}
		button.interactable = (result == 0 && leftCount > 0);
		buttonText.text = ((result == 0 && leftCount > 0) ? LocalStringManager.Get(textKey).SetColor(this.btnTextNormalColor) : LocalStringManager.Get(textKey).SetColor(this.btnTextInteractableColor));
		StringBuilder sb = EasyPool.Get<StringBuilder>();
		sb.Clear();
		bool flag = (result & 1) != 0;
		if (flag)
		{
			sb.AppendLine(LocalStringManager.Get(noWoundKey));
		}
		bool flag2 = (result & 2) != 0;
		if (flag2)
		{
			string resourceName = Config.ResourceType.Instance[needResourceType].Name;
			sb.AppendLine(LocalStringManager.GetFormat(LanguageKey.LK_Heal_GearMate_OutInjury_Disable_Tips_NoResource, resourceName));
		}
		bool flag3 = (result & 4) != 0;
		if (flag3)
		{
			sb.AppendLine(LocalStringManager.Get(LanguageKey.LK_Heal_GearMate_OutInjury_Disable_Tips_ToolType));
		}
		bool flag4 = (result & 8) != 0;
		if (flag4)
		{
			sb.AppendLine(LocalStringManager.Get(LanguageKey.LK_Heal_GearMate_OutInjury_Disable_Tips_Attainment));
		}
		tipDisplayer.RuntimeParam.Set("arg0", sb.ToString().SetColor("brightred"));
	}

	// Token: 0x06003548 RID: 13640 RVA: 0x001ABDE8 File Offset: 0x001A9FE8
	private void RefreshUiByGearMate()
	{
		bool flag = !this.HasSelectedPatient();
		if (flag)
		{
			this.background.SetActive(true);
			this.gearMateSelectedTool.SetActive(false);
			this.itemSelectorRefers.SetActive(false);
			this.midItemRoot.SetActive(false);
			this.buttonGrid.gameObject.SetActive(false);
			this.gearMateButtonGrid.gameObject.SetActive(false);
			this.costGrid.gameObject.SetActive(false);
			this.btnBack.SetActive(false);
		}
		else
		{
			bool hasSelectedDoctorAndPatient = this.HasSelectedDoctor();
			bool isSelectedPatientGearMate = this.IsSelectedPatientGearMate;
			this.background.SetActive(!isSelectedPatientGearMate);
			this.gearMateSelectedTool.SetActive(isSelectedPatientGearMate);
			this.itemSelectorRefers.SetActive(hasSelectedDoctorAndPatient && isSelectedPatientGearMate);
			this.midItemRoot.SetActive(hasSelectedDoctorAndPatient && isSelectedPatientGearMate);
			this.costGrid.gameObject.SetActive(hasSelectedDoctorAndPatient);
			this.btnBack.SetActive(hasSelectedDoctorAndPatient);
			this.buttonGrid.gameObject.SetActive(!isSelectedPatientGearMate);
			this.gearMateButtonGrid.gameObject.SetActive(isSelectedPatientGearMate);
		}
	}

	// Token: 0x06003549 RID: 13641 RVA: 0x001ABF10 File Offset: 0x001AA110
	private unsafe void RefreshGearMateDoctorAttainmentTip(TooltipInvoker tipDisplayer, sbyte lifeSkillType)
	{
		int doctorId = this._selectedDoctor;
		LifeSkillShorts attainment = this._doctorAttainmentDict.GetOrDefault(doctorId);
		short hasAttainment = *attainment[(int)lifeSkillType];
		sbyte repairType = UI_Heal.GetRepairTypeFromLifeSkillType(lifeSkillType);
		ItemDisplayData usingItem = this.GetUsingTool(repairType);
		bool flag = usingItem == null;
		if (flag)
		{
			tipDisplayer.enabled = false;
		}
		else
		{
			bool isEmptyTool = usingItem.Key == this._emptyToolKey;
			short toolAttainment = UI_Make.GetToolAttainment(usingItem.Key.TemplateId, *attainment[(int)lifeSkillType], lifeSkillType);
			bool typeMeet = CraftTool.Instance[usingItem.Key.TemplateId].RequiredLifeSkillTypes.Contains(lifeSkillType);
			bool flag2 = !typeMeet || this.IsUsingToolAutoSelected;
			if (flag2)
			{
				tipDisplayer.enabled = false;
			}
			else
			{
				tipDisplayer.enabled = true;
				short delta = toolAttainment;
				List<HealAttainmentTipsHelper.AttainmentItem> attainmentItems = EasyPool.Get<List<HealAttainmentTipsHelper.AttainmentItem>>();
				attainmentItems.Clear();
				bool flag3 = isEmptyTool;
				if (flag3)
				{
					foreach (sbyte tipSkillType in UI_Heal.GearMateRelatedLifeSkillTypeArray)
					{
						short emptyAttainment = UI_Make.GetToolAttainment(usingItem.Key.TemplateId, *attainment[(int)tipSkillType], tipSkillType);
						attainmentItems.Add(new HealAttainmentTipsHelper.AttainmentItem
						{
							SkillType = tipSkillType,
							DeltaAttainment = (int)emptyAttainment,
							Attainment = (int)(*attainment[(int)tipSkillType])
						});
					}
				}
				else
				{
					attainmentItems.Add(new HealAttainmentTipsHelper.AttainmentItem
					{
						SkillType = lifeSkillType,
						DeltaAttainment = (int)delta,
						Attainment = (int)(*attainment[(int)lifeSkillType])
					});
				}
				bool flag4 = attainmentItems.Count > 0;
				if (flag4)
				{
					HealAttainmentTipsHelper.RefreshTips(tipDisplayer, usingItem.Key, isEmptyTool, attainmentItems, true);
				}
				EasyPool.Free<List<HealAttainmentTipsHelper.AttainmentItem>>(attainmentItems);
			}
		}
	}

	// Token: 0x0600354A RID: 13642 RVA: 0x001AC0DC File Offset: 0x001AA2DC
	private void RefreshCharInfoHeal(UI_Heal.EHealType healType, bool isPreview = false)
	{
		bool isGearMate = this.IsSelectedPatientGearMate;
		int num;
		int num2;
		if (isGearMate)
		{
			if (!true)
			{
			}
			switch (healType)
			{
			case UI_Heal.EHealType.OuterInjury:
				num = 0;
				break;
			case UI_Heal.EHealType.InnerInjury:
				num = 1;
				break;
			case UI_Heal.EHealType.Poison:
				num = 2;
				break;
			case UI_Heal.EHealType.QiDisorder:
				num = 3;
				break;
			default:
				throw new ArgumentOutOfRangeException("healType", healType, null);
			}
			if (!true)
			{
			}
			num2 = num;
		}
		else
		{
			if (!true)
			{
			}
			switch (healType)
			{
			case UI_Heal.EHealType.Injury:
				num = 0;
				goto IL_A7;
			case UI_Heal.EHealType.Poison:
				num = 1;
				goto IL_A7;
			case UI_Heal.EHealType.QiDisorder:
				num = 2;
				goto IL_A7;
			case UI_Heal.EHealType.Health:
				num = 3;
				goto IL_A7;
			}
			throw new ArgumentOutOfRangeException("healType", healType, null);
			IL_A7:
			if (!true)
			{
			}
			num2 = num;
		}
		int index = num2;
		Transform grid = this.healCurrPatient.grid;
		PropertyItem gridChild = grid.GetChild(index).GetComponent<PropertyItem>();
		Injuries injuries = this._patientInjuryDict.GetOrDefault(this._selectedPatient);
		ValueTuple<sbyte, sbyte> bothSum = injuries.GetBothSum();
		PoisonInts poisons = this._patientPoisonDict.GetOrDefault(this._selectedPatient);
		short qiDisorder = this._patientQiDisorderDict.GetOrDefault(this._selectedPatient);
		short health = this._patientHealthDict.GetOrDefault(this._selectedPatient);
		short maxHealth = this._patientMaxHealthDict.GetOrDefault(this._selectedPatient);
		if (!true)
		{
		}
		switch (healType)
		{
		case UI_Heal.EHealType.Injury:
			num = injuries.GetSum();
			break;
		case UI_Heal.EHealType.OuterInjury:
			num = (int)bothSum.Item1;
			break;
		case UI_Heal.EHealType.InnerInjury:
			num = (int)bothSum.Item2;
			break;
		case UI_Heal.EHealType.Poison:
			num = poisons.Sum();
			break;
		case UI_Heal.EHealType.QiDisorder:
			num = (int)(qiDisorder / 10);
			break;
		case UI_Heal.EHealType.Health:
			num = UI_Heal.GetHealthPercent((int)health, (int)maxHealth);
			break;
		default:
			throw new ArgumentOutOfRangeException("healType", healType, null);
		}
		if (!true)
		{
		}
		int value = num;
		if (!true)
		{
		}
		string text;
		switch (healType)
		{
		case UI_Heal.EHealType.Injury:
			text = UI_Heal.IconHelper.GetInjuryIcon(value);
			break;
		case UI_Heal.EHealType.OuterInjury:
			text = UI_Heal.IconHelper.GetOutInjuryIcon(value);
			break;
		case UI_Heal.EHealType.InnerInjury:
			text = UI_Heal.IconHelper.GetInnerInjuryIcon(value);
			break;
		case UI_Heal.EHealType.Poison:
			text = UI_Heal.IconHelper.GetPoisonIcon(value);
			break;
		case UI_Heal.EHealType.QiDisorder:
			text = UI_Heal.IconHelper.GetQiDisorderIcon(UI_Heal.IsQiDisorderMax((int)qiDisorder));
			break;
		case UI_Heal.EHealType.Health:
			text = UI_Heal.IconHelper.GetHealthIcon(UI_Heal.IsDying((int)health));
			break;
		default:
			throw new ArgumentOutOfRangeException("healType", healType, null);
		}
		if (!true)
		{
		}
		string icon = text;
		if (!true)
		{
		}
		switch (healType)
		{
		case UI_Heal.EHealType.Injury:
			text = LanguageKey.LK_Injury.Tr();
			break;
		case UI_Heal.EHealType.OuterInjury:
			text = LanguageKey.LK_Out_Injury.Tr();
			break;
		case UI_Heal.EHealType.InnerInjury:
			text = LanguageKey.LK_Inner_Injury.Tr();
			break;
		case UI_Heal.EHealType.Poison:
			text = LanguageKey.LK_Poison.Tr();
			break;
		case UI_Heal.EHealType.QiDisorder:
			text = LanguageKey.LK_Combat_MarkType_QiDisorder.Tr();
			break;
		case UI_Heal.EHealType.Health:
			text = LanguageKey.LK_Health.Tr();
			break;
		default:
			throw new ArgumentOutOfRangeException("healType", healType, null);
		}
		if (!true)
		{
		}
		string typeName = text;
		int num3;
		if (isGearMate)
		{
			if (!true)
			{
			}
			switch (healType)
			{
			case UI_Heal.EHealType.OuterInjury:
				num = this._currentGearMateRepairEffect.GetOrDefault(0);
				break;
			case UI_Heal.EHealType.InnerInjury:
				num = this._currentGearMateRepairEffect.GetOrDefault(1);
				break;
			case UI_Heal.EHealType.Poison:
				num = this._currentGearMateRepairEffect.GetOrDefault(2);
				break;
			case UI_Heal.EHealType.QiDisorder:
				num = this._currentGearMateRepairEffect.GetOrDefault(3) / 10;
				break;
			default:
				throw new ArgumentOutOfRangeException("healType", healType, null);
			}
			if (!true)
			{
			}
			num3 = num;
		}
		else
		{
			if (!true)
			{
			}
			switch (healType)
			{
			case UI_Heal.EHealType.Injury:
				num = this.CurHealInjuryCost.HealEffect;
				goto IL_3DC;
			case UI_Heal.EHealType.Poison:
				num = this.CurHealPoisonCost.HealEffect;
				goto IL_3DC;
			case UI_Heal.EHealType.QiDisorder:
				num = this.CurHealQiDisorderCost.HealEffect / 10;
				goto IL_3DC;
			case UI_Heal.EHealType.Health:
				num = UI_Heal.CalculateHealHealthPercent((int)health, (int)maxHealth, this.CurHealHealthCost.HealEffect);
				goto IL_3DC;
			}
			throw new ArgumentOutOfRangeException("healType", healType, null);
			IL_3DC:
			if (!true)
			{
			}
			num3 = num;
		}
		int healEffect = num3;
		string valueText = (healType == UI_Heal.EHealType.Health) ? string.Format("{0}%", value) : value.ToString();
		string healSign = (healType == UI_Heal.EHealType.Health) ? "+" : "-";
		string healValue = isPreview ? string.Format("{0}{1}", healSign, healEffect).SetColor("brightblue") : string.Empty;
		string healValueText = (healType == UI_Heal.EHealType.Health && !healValue.IsNullOrEmpty()) ? (healValue + "%") : healValue;
		string content = valueText + healValueText;
		gridChild.Set(icon, typeName, content, null, true);
		gridChild.GetComponent<HealCurrDoctorGrid>().hover.SetActive(isPreview);
		bool flag = !isGearMate;
		if (flag)
		{
			gridChild.Tip.enabled = false;
		}
		else
		{
			StringBuilder stringBuilder = EasyPool.Get<StringBuilder>();
			stringBuilder.Clear();
			switch (healType)
			{
			case UI_Heal.EHealType.OuterInjury:
				stringBuilder.AppendLine(LanguageKey.LK_MouseTip_GearMateNoMonthlyHeal.Tr().ColorReplace());
				stringBuilder.AppendLine(LanguageKey.LK_MouseTip_GearMateHealRequirement_OutterInjury.Tr().ColorReplace());
				break;
			case UI_Heal.EHealType.InnerInjury:
				stringBuilder.AppendLine(LanguageKey.LK_MouseTip_GearMateNoMonthlyHeal.Tr().ColorReplace());
				stringBuilder.AppendLine(LanguageKey.LK_MouseTip_GearMateHealRequirement_InnerInjury.Tr().ColorReplace());
				break;
			case UI_Heal.EHealType.Poison:
				stringBuilder.Append(LanguageKey.LK_MouseTip_GearMateHealRequirement_Poison.Tr().ColorReplace());
				break;
			case UI_Heal.EHealType.QiDisorder:
				stringBuilder.Append(LanguageKey.LK_MouseTip_GearMateHealRequirement_QiDisorder.Tr().ColorReplace());
				break;
			default:
				throw new ArgumentOutOfRangeException("healType", healType, null);
			}
			gridChild.Tip.enabled = true;
			string[] presetParam = gridChild.Tip.PresetParam;
			bool flag2 = presetParam == null || presetParam.Length != 1;
			if (flag2)
			{
				gridChild.Tip.PresetParam = new string[1];
			}
			gridChild.Tip.PresetParam[0] = stringBuilder.ToString();
			stringBuilder.Clear();
		}
	}

	// Token: 0x0600354B RID: 13643 RVA: 0x001AC6DC File Offset: 0x001AA8DC
	private void UpdatePatientInfo()
	{
		int patientId = this._selectedPatient;
		this.healCurrPatient.avatarBack.SetSprite((!this.HasSelectedPatient()) ? "ui9_back_heal_player_0" : (this.IsSelectedPatientGearMate ? "ui9_back_heal_player_3" : "ui9_back_heal_player_1"), false, null);
		bool flag = !this.HasSelectedPatient();
		if (flag)
		{
			bool flag2 = this.btnAutoPut != null;
			if (flag2)
			{
				this.btnAutoPut.interactable = false;
			}
		}
		else
		{
			this.RefreshAutoPutButton();
			CharacterDisplayData charData;
			short health;
			short maxHealth;
			bool flag3 = !this._displayDataDict.TryGetValue(patientId, out charData) || !this._patientHealthDict.TryGetValue(patientId, out health) || !this._patientMaxHealthDict.TryGetValue(patientId, out maxHealth);
			if (!flag3)
			{
				string charName = NameCenter.GetMonasticTitleOrDisplayName(charData, patientId == this._taiwuCharId);
				this.healCurrPatient.avatar.Refresh(charData, true);
				this.healCurrPatient.nameFrame.SetName(charName);
				bool isSelectedPatientGearMate = this.IsSelectedPatientGearMate;
				if (isSelectedPatientGearMate)
				{
					this.RefreshCharInfoHeal(UI_Heal.EHealType.OuterInjury, false);
					this.RefreshCharInfoHeal(UI_Heal.EHealType.InnerInjury, false);
					this.RefreshCharInfoHeal(UI_Heal.EHealType.Poison, false);
					this.RefreshCharInfoHeal(UI_Heal.EHealType.QiDisorder, false);
				}
				else
				{
					this.RefreshCharInfoHeal(UI_Heal.EHealType.Injury, false);
					this.RefreshCharInfoHeal(UI_Heal.EHealType.Poison, false);
					this.RefreshCharInfoHeal(UI_Heal.EHealType.QiDisorder, false);
					this.RefreshCharInfoHeal(UI_Heal.EHealType.Health, false);
				}
			}
		}
	}

	// Token: 0x0600354C RID: 13644 RVA: 0x001AC834 File Offset: 0x001AAA34
	private unsafe void UpdateHealInjuryBtn(int leftDays = -1)
	{
		int doctorId = this._selectedDoctor;
		int patientId = this._selectedPatient;
		bool flag = !this._doctorHealCountDict.ContainsKey(doctorId);
		if (!flag)
		{
			this.healInjuryCount.text = this._doctorHealCountDict[doctorId].HealingCount.ToString();
			this.RefreshCountBack(this.healInjuryCount, (int)this._doctorHealCountDict[doctorId].HealingCount);
			leftDays = ((leftDays >= 0) ? leftDays : UI_Heal.GetLeftDate());
			int leftSpiritualDebt = this.GetLeftSpiritualDebt();
			bool herbEnough = *this._taiwuResource[5] >= this.CurHealInjuryCost.CostHerb;
			bool moneyEnough = !this._needPlay || *this._taiwuResource[6] >= this.CurHealInjuryCost.CostMoney;
			bool timeEnough = leftDays > 0;
			bool spiritualDebtEnough = leftSpiritualDebt > this.CurHealInjuryCost.CostSpiritualDebt;
			bool hasAnyInjury = this._patientInjuryDict.ContainsKey(patientId) && this._patientInjuryDict[patientId].GetSum() > 0;
			bool hasHealCount = this._doctorHealCountDict[doctorId].HealingCount > 0;
			bool canHealInjury = hasHealCount && herbEnough && moneyEnough && timeEnough && this.CurHealInjuryCost.HealEffect > 0;
			bool flag2 = this._expensiveHeal && !spiritualDebtEnough;
			if (flag2)
			{
				canHealInjury = false;
			}
			TooltipInvoker btnTips = this.healInjury.GetComponent<TooltipInvoker>();
			this.healInjury.interactable = canHealInjury;
			bool flag3 = canHealInjury && hasAnyInjury;
			if (flag3)
			{
				this.healInjuryText.text = LocalStringManager.Get(LanguageKey.LK_Heal_Injury).SetColor(this.btnTextNormalColor);
			}
			else
			{
				this.healInjuryText.text = LocalStringManager.Get(LanguageKey.LK_Heal_Injury).SetColor(this.btnTextInteractableColor);
			}
			btnTips.enabled = !canHealInjury;
			bool enabled = btnTips.enabled;
			if (enabled)
			{
				bool flag4 = !hasAnyInjury;
				LanguageKey key;
				if (flag4)
				{
					key = LanguageKey.LK_Heal_Injury_Disable_Tips_NoInjury;
				}
				else
				{
					bool flag5 = !hasHealCount;
					if (flag5)
					{
						key = LanguageKey.LK_Heal_Injury_Disable_Tips_NoCount;
					}
					else
					{
						bool flag6 = !herbEnough;
						if (flag6)
						{
							key = LanguageKey.LK_Heal_Common_Disable_Tips_NoHerb;
						}
						else
						{
							bool flag7 = !moneyEnough;
							if (flag7)
							{
								key = LanguageKey.LK_Heal_Common_Disable_Tips_NoMoney;
							}
							else
							{
								bool flag8 = !timeEnough;
								if (flag8)
								{
									key = LanguageKey.LK_Common_LeftTime_NotEnough;
								}
								else
								{
									bool flag9 = !spiritualDebtEnough && this._expensiveHeal;
									if (flag9)
									{
										key = LanguageKey.LK_Common_SpiritualDebt_NotEnough;
									}
									else
									{
										key = LanguageKey.LK_Heal_Injury_Disable_Tips_NoAttainment;
									}
								}
							}
						}
					}
				}
				btnTips.PresetParam[0] = LocalStringManager.Get(key).SetColor("brightred");
			}
		}
	}

	// Token: 0x0600354D RID: 13645 RVA: 0x001ACAC4 File Offset: 0x001AACC4
	private static int GetLeftDate()
	{
		return SingletonObject.getInstance<TimeManager>().GetRemainingActionPointConvertToDays();
	}

	// Token: 0x0600354E RID: 13646 RVA: 0x001ACAE0 File Offset: 0x001AACE0
	private int GetLeftSpiritualDebt()
	{
		return SingletonObject.getInstance<WorldMapModel>().AreaSpiritualDebt.GetValueOrDefault(this._doctorOrgLocation.AreaId, 0);
	}

	// Token: 0x0600354F RID: 13647 RVA: 0x001ACB10 File Offset: 0x001AAD10
	private unsafe void UpdateHealPoisonBtn(int leftDays = -1)
	{
		int doctorId = this._selectedDoctor;
		int patientId = this._selectedPatient;
		CombatResources doctorHealCount;
		bool flag = !this._doctorHealCountDict.TryGetValue(doctorId, out doctorHealCount);
		if (!flag)
		{
			this.healPoisonCount.text = doctorHealCount.DetoxCount.ToString();
			this.RefreshCountBack(this.healPoisonCount, (int)this._doctorHealCountDict[doctorId].DetoxCount);
			leftDays = ((leftDays >= 0) ? leftDays : SingletonObject.getInstance<TimeManager>().GetRemainingActionPointConvertToDays());
			int leftSpiritualDebt = this.GetLeftSpiritualDebt();
			bool herbEnough = *this._taiwuResource[5] >= this.CurHealPoisonCost.CostHerb;
			bool moneyEnough = !this._needPlay || *this._taiwuResource[6] >= this.CurHealPoisonCost.CostMoney;
			bool timeEnough = leftDays > 0;
			bool spiritualDebtEnough = leftSpiritualDebt > this.CurHealPoisonCost.CostSpiritualDebt;
			bool hasAnyPoison = this._patientPoisonDict.ContainsKey(patientId) && this._patientPoisonDict[patientId].IsNonZero();
			bool hasHealCount = doctorHealCount.DetoxCount > 0;
			bool canHealPoison = hasHealCount && herbEnough && moneyEnough && timeEnough && this.CurHealPoisonCost.HealEffect > 0;
			bool flag2 = this._expensiveHeal && !spiritualDebtEnough;
			if (flag2)
			{
				canHealPoison = false;
			}
			TooltipInvoker btnTips = this.healPoison.GetComponent<TooltipInvoker>();
			this.healPoison.interactable = canHealPoison;
			bool flag3 = canHealPoison && hasAnyPoison;
			if (flag3)
			{
				this.healPoisonText.text = LocalStringManager.Get(LanguageKey.LK_Heal_Poison).SetColor(this.btnTextNormalColor);
			}
			else
			{
				this.healPoisonText.text = LocalStringManager.Get(LanguageKey.LK_Heal_Poison).SetColor(this.btnTextInteractableColor);
			}
			btnTips.enabled = !canHealPoison;
			bool enabled = btnTips.enabled;
			if (enabled)
			{
				bool flag4 = !hasAnyPoison;
				LanguageKey key;
				if (flag4)
				{
					key = LanguageKey.LK_Heal_Poison_Disable_Tips_NoPoison;
				}
				else
				{
					bool flag5 = !hasHealCount;
					if (flag5)
					{
						key = LanguageKey.LK_Heal_Poison_Disable_Tips_NoCount;
					}
					else
					{
						bool flag6 = !herbEnough;
						if (flag6)
						{
							key = LanguageKey.LK_Heal_Common_Disable_Tips_NoHerb;
						}
						else
						{
							bool flag7 = !moneyEnough;
							if (flag7)
							{
								key = LanguageKey.LK_Heal_Common_Disable_Tips_NoMoney;
							}
							else
							{
								bool flag8 = !timeEnough;
								if (flag8)
								{
									key = LanguageKey.LK_Common_LeftTime_NotEnough;
								}
								else
								{
									bool flag9 = !spiritualDebtEnough && this._expensiveHeal;
									if (flag9)
									{
										key = LanguageKey.LK_Common_SpiritualDebt_NotEnough;
									}
									else
									{
										key = LanguageKey.LK_Heal_Poison_Disable_Tips_NoAttainment;
									}
								}
							}
						}
					}
				}
				btnTips.PresetParam[0] = LocalStringManager.Get(key).SetColor("brightred");
			}
		}
	}

	// Token: 0x06003550 RID: 13648 RVA: 0x001ACD90 File Offset: 0x001AAF90
	private void RefreshCountBack(TextMeshProUGUI countValueText, int countValue)
	{
		Transform countBackDisable = countValueText.transform.parent.Find("CountBackDisable");
		countBackDisable.gameObject.SetActive(countValue == 0);
	}

	// Token: 0x06003551 RID: 13649 RVA: 0x001ACDC4 File Offset: 0x001AAFC4
	private unsafe void UpdateHealQiDisorderBtn(int leftDays = -1)
	{
		int doctorId = this._selectedDoctor;
		int patientId = this._selectedPatient;
		TooltipInvoker btnTips = this.healQiDisorder.GetComponent<TooltipInvoker>();
		CombatResources doctorHealCount;
		bool flag = !this._doctorHealCountDict.TryGetValue(doctorId, out doctorHealCount);
		if (flag)
		{
			btnTips.enabled = false;
		}
		else
		{
			this.healQiDisorderCount.text = doctorHealCount.BreathingCount.ToString();
			this.RefreshCountBack(this.healQiDisorderCount, (int)doctorHealCount.BreathingCount);
			leftDays = ((leftDays >= 0) ? leftDays : UI_Heal.GetLeftDate());
			int leftSpiritualDebt = this.GetLeftSpiritualDebt();
			bool herbEnough = *this._taiwuResource[5] >= this.CurHealQiDisorderCost.CostHerb;
			bool moneyEnough = !this._needPlay || *this._taiwuResource[6] >= this.CurHealQiDisorderCost.CostMoney;
			bool timeEnough = leftDays > 0;
			bool spiritualDebtEnough = leftSpiritualDebt > this.CurHealQiDisorderCost.CostSpiritualDebt;
			bool hasAnyQiDisorder = this._patientQiDisorderDict.ContainsKey(patientId) && this._patientQiDisorderDict[patientId] > 0;
			bool hasHealCount = doctorHealCount.BreathingCount > 0;
			bool canHealQiDisorder = hasHealCount && herbEnough && moneyEnough && timeEnough && this.CurHealQiDisorderCost.HealEffect > 0;
			bool flag2 = this._expensiveHeal && !spiritualDebtEnough;
			if (flag2)
			{
				canHealQiDisorder = false;
			}
			this.healQiDisorder.interactable = canHealQiDisorder;
			this.healQiDisorderText.text = LocalStringManager.Get(LanguageKey.LK_Heal_QiDisorder).SetColor((canHealQiDisorder && hasAnyQiDisorder) ? this.btnTextNormalColor : this.btnTextInteractableColor);
			btnTips.enabled = !canHealQiDisorder;
			bool enabled = btnTips.enabled;
			if (enabled)
			{
				bool flag3 = !hasAnyQiDisorder;
				LanguageKey key;
				if (flag3)
				{
					key = LanguageKey.LK_Heal_QiDisorder_Disable_Tips_NoQiDisorder;
				}
				else
				{
					bool flag4 = !hasHealCount;
					if (flag4)
					{
						key = LanguageKey.LK_Heal_QiDisorder_Disable_Tips_NoCount;
					}
					else
					{
						bool flag5 = !herbEnough;
						if (flag5)
						{
							key = LanguageKey.LK_Heal_Common_Disable_Tips_NoHerb;
						}
						else
						{
							bool flag6 = !moneyEnough;
							if (flag6)
							{
								key = LanguageKey.LK_Heal_Common_Disable_Tips_NoMoney;
							}
							else
							{
								bool flag7 = !timeEnough;
								if (flag7)
								{
									key = LanguageKey.LK_Common_LeftTime_NotEnough;
								}
								else
								{
									bool flag8 = !spiritualDebtEnough && this._expensiveHeal;
									if (flag8)
									{
										key = LanguageKey.LK_Common_SpiritualDebt_NotEnough;
									}
									else
									{
										key = LanguageKey.LK_Heal_QiDisorder_Disable_Tips_NoAttainment;
									}
								}
							}
						}
					}
				}
				btnTips.PresetParam[0] = LocalStringManager.Get(key).SetColor("brightred");
			}
		}
	}

	// Token: 0x06003552 RID: 13650 RVA: 0x001AD014 File Offset: 0x001AB214
	private unsafe void UpdateHealHealthBtn(int leftDays = -1)
	{
		int doctorId = this._selectedDoctor;
		int patientId = this._selectedPatient;
		TooltipInvoker btnTips = this.healHealth.GetComponent<TooltipInvoker>();
		short patientHealth;
		short patientMaxHealth;
		bool flag = !this._doctorHealCountDict.ContainsKey(doctorId) || !this._patientHealthDict.TryGetValue(patientId, out patientHealth) || !this._patientMaxHealthDict.TryGetValue(patientId, out patientMaxHealth);
		if (flag)
		{
			btnTips.enabled = false;
		}
		else
		{
			this.healHealthCount.text = this._doctorHealCountDict[doctorId].RecoverCount.ToString();
			this.RefreshCountBack(this.healHealthCount, (int)this._doctorHealCountDict[doctorId].RecoverCount);
			leftDays = ((leftDays >= 0) ? leftDays : UI_Heal.GetLeftDate());
			int leftSpiritualDebt = this.GetLeftSpiritualDebt();
			bool herbEnough = *this._taiwuResource[5] >= this.CurHealHealthCost.CostHerb;
			bool moneyEnough = !this._needPlay || *this._taiwuResource[6] >= this.CurHealHealthCost.CostMoney;
			bool timeEnough = leftDays > 0;
			bool spiritualDebtEnough = leftSpiritualDebt > this.CurHealHealthCost.CostSpiritualDebt;
			bool hasAnyHealthProblem = patientHealth < patientMaxHealth;
			bool hasHealCount = this._doctorHealCountDict[doctorId].RecoverCount > 0;
			bool canHealHealth = hasHealCount && herbEnough && moneyEnough && timeEnough && this.CurHealHealthCost.HealEffect > 0;
			bool flag2 = this._expensiveHeal && !spiritualDebtEnough;
			if (flag2)
			{
				canHealHealth = false;
			}
			this.healHealth.interactable = canHealHealth;
			this.healHealthText.text = LocalStringManager.Get(LanguageKey.LK_Heal_Health).SetColor((canHealHealth && hasAnyHealthProblem) ? this.btnTextNormalColor : this.btnTextInteractableColor);
			btnTips.enabled = !canHealHealth;
			bool enabled = btnTips.enabled;
			if (enabled)
			{
				bool flag3 = this.CurHealHealthCost.MaxRequireAttainment == int.MaxValue;
				LanguageKey key;
				if (flag3)
				{
					key = LanguageKey.LK_Heal_Health_Disable_Tips_NoHealth;
				}
				else
				{
					bool flag4 = !hasAnyHealthProblem;
					if (flag4)
					{
						key = LanguageKey.LK_Heal_Health_Disable_Tips_IsHealth;
					}
					else
					{
						bool flag5 = !hasHealCount;
						if (flag5)
						{
							key = LanguageKey.LK_Heal_Health_Disable_Tips_NoCount;
						}
						else
						{
							bool flag6 = !herbEnough;
							if (flag6)
							{
								key = LanguageKey.LK_Heal_Common_Disable_Tips_NoHerb;
							}
							else
							{
								bool flag7 = !moneyEnough;
								if (flag7)
								{
									key = LanguageKey.LK_Heal_Common_Disable_Tips_NoMoney;
								}
								else
								{
									bool flag8 = !timeEnough;
									if (flag8)
									{
										key = LanguageKey.LK_Common_LeftTime_NotEnough;
									}
									else
									{
										bool flag9 = !spiritualDebtEnough && this._expensiveHeal;
										if (flag9)
										{
											key = LanguageKey.LK_Common_SpiritualDebt_NotEnough;
										}
										else
										{
											key = LanguageKey.LK_Heal_Health_Disable_Tips_NoAttainment;
										}
									}
								}
							}
						}
					}
				}
				btnTips.PresetParam[0] = LocalStringManager.Get(key).SetColor("brightred");
			}
		}
	}

	// Token: 0x06003553 RID: 13651 RVA: 0x001AD2B4 File Offset: 0x001AB4B4
	private static int CalculateHealHealthPercent(int health, int maxHealth, int healDelta)
	{
		bool flag = maxHealth == 0;
		int result;
		if (flag)
		{
			result = 0;
		}
		else
		{
			int currentPercent = UI_Heal.GetHealthPercent(health, maxHealth);
			int newPercent = UI_Heal.GetHealthPercent(health + healDelta, maxHealth);
			result = Math.Clamp(newPercent - currentPercent, 0, 100);
		}
		return result;
	}

	// Token: 0x06003554 RID: 13652 RVA: 0x001AD2F0 File Offset: 0x001AB4F0
	private void OnDaysInMonthChange(ArgumentBox argBox)
	{
		bool flag = !UIManager.Instance.IsFocusElement(this.Element);
		if (!flag)
		{
			this.UpdateHealInjuryBtn(-1);
			this.UpdateHealPoisonBtn(-1);
			GEvent.OnEvent(UiEvents.OnRefreshCharacterHealUIBottom, null);
		}
	}

	// Token: 0x06003555 RID: 13653 RVA: 0x001AD338 File Offset: 0x001AB538
	private void OnTopUiChanged(ArgumentBox argBox)
	{
		bool flag = !UIManager.Instance.CheckPopupElementIsInTop(this.Element) && !UIManager.Instance.IsElementActive(UIElement.CharacterMenu);
		if (flag)
		{
			this.QuickHide();
			GEvent.OnEvent(UiEvents.OnRefreshCharacterHealUIBottom, null);
		}
	}

	// Token: 0x06003556 RID: 13654 RVA: 0x001AD38B File Offset: 0x001AB58B
	private void OnActionPointChange(ArgumentBox argBox)
	{
		this.RefreshDate();
		this.UpdateHealInjuryBtn(-1);
		this.UpdateHealPoisonBtn(-1);
		this.UpdateHealQiDisorderBtn(-1);
		this.UpdateHealHealthBtn(-1);
	}

	// Token: 0x06003557 RID: 13655 RVA: 0x001AD3B5 File Offset: 0x001AB5B5
	private void RefreshDate()
	{
		this.RefreshCostOther(UI_Heal.ECostOtherType.Time, false);
	}

	// Token: 0x06003558 RID: 13656 RVA: 0x001AD3C4 File Offset: 0x001AB5C4
	private void RefreshSpiritualDebt(int type = -1)
	{
		if (!true)
		{
		}
		int num;
		switch (type)
		{
		case 0:
			num = this.CurHealInjuryCost.CostSpiritualDebt;
			break;
		case 1:
			num = this.CurHealPoisonCost.CostSpiritualDebt;
			break;
		case 2:
			num = this.CurHealQiDisorderCost.CostSpiritualDebt;
			break;
		case 3:
			num = this.CurHealHealthCost.CostSpiritualDebt;
			break;
		default:
			num = 0;
			break;
		}
		if (!true)
		{
		}
		int cost = num;
		this._currentSpiritualDebtCost = cost;
		this.OnAreaSpiritualDebtChange(null);
	}

	// Token: 0x06003559 RID: 13657 RVA: 0x001AD43F File Offset: 0x001AB63F
	private void OnAreaSpiritualDebtChange(ArgumentBox argBox = null)
	{
		this.RefreshCostOther(UI_Heal.ECostOtherType.SpiritualDebt, false);
		this.UpdateHealInjuryBtn(-1);
		this.UpdateHealPoisonBtn(-1);
		this.UpdateHealQiDisorderBtn(-1);
		this.UpdateHealHealthBtn(-1);
	}

	// Token: 0x0600355A RID: 13658 RVA: 0x001AD46C File Offset: 0x001AB66C
	private void RefreshCostOther(UI_Heal.ECostOtherType costOtherType, bool isPreview = false)
	{
		bool isSelectedPatientGearMate = this.IsSelectedPatientGearMate;
		if (!isSelectedPatientGearMate)
		{
			if (!true)
			{
			}
			int num;
			if (costOtherType != UI_Heal.ECostOtherType.Time)
			{
				if (costOtherType != UI_Heal.ECostOtherType.SpiritualDebt)
				{
					throw new ArgumentOutOfRangeException("costOtherType", costOtherType, null);
				}
				num = (this._expensiveHeal ? (this._needPlay ? 3 : 2) : -1);
			}
			else
			{
				num = 1;
			}
			if (!true)
			{
			}
			int index = num;
			bool flag = index < 0;
			if (!flag)
			{
				if (!true)
				{
				}
				if (costOtherType != UI_Heal.ECostOtherType.Time)
				{
					if (costOtherType != UI_Heal.ECostOtherType.SpiritualDebt)
					{
						throw new ArgumentOutOfRangeException("costOtherType", costOtherType, null);
					}
					num = this.GetLeftSpiritualDebt();
				}
				else
				{
					num = UI_Heal.GetLeftDate();
				}
				if (!true)
				{
				}
				int own = num;
				if (!true)
				{
				}
				if (costOtherType != UI_Heal.ECostOtherType.Time)
				{
					if (costOtherType != UI_Heal.ECostOtherType.SpiritualDebt)
					{
						throw new ArgumentOutOfRangeException("costOtherType", costOtherType, null);
					}
					num = this._currentSpiritualDebtCost;
				}
				else
				{
					num = 1;
				}
				if (!true)
				{
				}
				int cost = num;
				if (!true)
				{
				}
				string text;
				if (costOtherType != UI_Heal.ECostOtherType.Time)
				{
					if (costOtherType != UI_Heal.ECostOtherType.SpiritualDebt)
					{
						throw new ArgumentOutOfRangeException("costOtherType", costOtherType, null);
					}
					text = WorldMapModel.GetFormatSpiritualDebt(own, 0).SetColor((own >= cost) ? "brightblue" : "brightred");
				}
				else
				{
					text = own.ToString().SetColor((own >= cost) ? "brightblue" : "brightred");
				}
				if (!true)
				{
				}
				string ownText = text;
				this.RefreshCostMaxCount();
				List<Transform> CommonParameterVertical_Trans = new List<Transform>();
				for (int i = 0; i < this.costGrid.childCount; i++)
				{
					bool flag2 = this.costGrid.GetChild(i).GetComponent<HealCostItem>();
					if (flag2)
					{
						CommonParameterVertical_Trans.Add(this.costGrid.GetChild(i));
					}
				}
				HealCostItem costGridChild = CommonParameterVertical_Trans[index].GetComponent<HealCostItem>();
				if (costOtherType != UI_Heal.ECostOtherType.Time)
				{
					if (costOtherType != UI_Heal.ECostOtherType.SpiritualDebt)
					{
						throw new ArgumentOutOfRangeException("costOtherType", costOtherType, null);
					}
					costGridChild.icon.SetSprite("sp_icon_enyixiaohao", false, null);
				}
				else
				{
					costGridChild.icon.SetSprite("sp_icon_shijian", false, null);
				}
				costGridChild.value.text = string.Format("{0}/{1}", ownText, cost);
				TooltipInvoker tip = costGridChild.mouseTip;
				tip.enabled = false;
				PointerTrigger pointerTrigger = costGridChild.pointerTrigger;
				pointerTrigger.enabled = false;
				GameObject highlightArea = costGridChild.highLightArea;
				highlightArea.SetActive(isPreview);
			}
		}
	}

	// Token: 0x0600355B RID: 13659 RVA: 0x001AD6D8 File Offset: 0x001AB8D8
	private void RefreshCostResource(sbyte resourceType, UI_Heal.EHealType healType, bool isPreview = false)
	{
		List<GearMateRepairRequirementDisplayData> requireItemList;
		bool flag = !this._gearMateRepairRequirementDict.TryGetValue(this._selectedPatient, out requireItemList) && this.IsSelectedPatientGearMate;
		if (!flag)
		{
			CharacterDisplayData taiwuCharData;
			bool flag2 = !this._displayDataDict.TryGetValue(this._taiwuCharId, out taiwuCharData);
			if (!flag2)
			{
				if (!true)
				{
				}
				int num;
				switch (resourceType)
				{
				case 1:
					num = 1;
					break;
				case 2:
					num = 0;
					break;
				case 3:
					num = 2;
					break;
				case 4:
					num = 3;
					break;
				case 5:
					num = 0;
					break;
				case 6:
					num = (this._needPlay ? 2 : 0);
					break;
				default:
					throw new ArgumentOutOfRangeException("resourceType", resourceType, null);
				}
				if (!true)
				{
				}
				int index = num;
				if (!true)
				{
				}
				MapHealSimulateResult mapHealSimulateResult;
				switch (healType)
				{
				case UI_Heal.EHealType.Injury:
					mapHealSimulateResult = this.CurHealInjuryCost;
					goto IL_10F;
				case UI_Heal.EHealType.Poison:
					mapHealSimulateResult = this.CurHealPoisonCost;
					goto IL_10F;
				case UI_Heal.EHealType.QiDisorder:
					mapHealSimulateResult = this.CurHealQiDisorderCost;
					goto IL_10F;
				case UI_Heal.EHealType.Health:
					mapHealSimulateResult = this.CurHealHealthCost;
					goto IL_10F;
				}
				mapHealSimulateResult = default(MapHealSimulateResult);
				IL_10F:
				if (!true)
				{
				}
				MapHealSimulateResult healResult = mapHealSimulateResult;
				int num2;
				if (isPreview)
				{
					if (!true)
					{
					}
					switch (resourceType)
					{
					case 1:
						num = requireItemList[1].ResourceCost;
						break;
					case 2:
						num = requireItemList[0].ResourceCost;
						break;
					case 3:
						num = requireItemList[2].ResourceCost;
						break;
					case 4:
						num = requireItemList[3].ResourceCost;
						break;
					case 5:
						num = healResult.CostHerb;
						break;
					case 6:
						num = healResult.CostMoney;
						break;
					default:
						throw new ArgumentOutOfRangeException("resourceType", resourceType, null);
					}
					if (!true)
					{
					}
					num2 = num;
				}
				else
				{
					num2 = 0;
				}
				int cost = num2;
				this.RefreshCostMaxCount();
				List<Transform> CommonParameterVertical_Trans = new List<Transform>();
				for (int i = 0; i < this.costGrid.childCount; i++)
				{
					bool flag3 = this.costGrid.GetChild(i).GetComponent<HealCostItem>();
					if (flag3)
					{
						CommonParameterVertical_Trans.Add(this.costGrid.GetChild(i));
					}
				}
				HealCostItem costGridChild = CommonParameterVertical_Trans[index].GetComponent<HealCostItem>();
				ResourceTypeItem resourceConfig = Config.ResourceType.Instance[resourceType];
				costGridChild.icon.SetSprite(resourceConfig.Icon, false, null);
				int resourceAmount = this._taiwuResource.Get((int)resourceType);
				string value = CommonUtils.GetDisplayStringForNum(resourceAmount, 100000);
				string costStr = isPreview ? CommonUtils.GetDisplayStringForNum(cost, 100000) : "-";
				string valueColor = (!isPreview || resourceAmount >= cost) ? "brightblue" : "brightred";
				string content = value.SetColor(valueColor) + "/" + costStr;
				costGridChild.value.SetText(content, true);
				string charName = NameCenter.GetMonasticTitleOrDisplayName(taiwuCharData, true);
				TooltipInvoker tip = costGridChild.mouseTip;
				tip.Type = TipType.Resource;
				tip.enabled = true;
				TooltipInvoker tooltipInvoker = tip;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = new ArgumentBox();
				}
				tip.RuntimeParam.Set("CharName", charName);
				tip.RuntimeParam.Set("ResourceType", resourceType);
				tip.RuntimeParam.Set("ResourceCount", resourceAmount);
				GameObject highlightArea = costGridChild.highLightArea;
				highlightArea.SetActive(isPreview);
				bool flag4 = !this.IsSelectedPatientGearMate;
				if (flag4)
				{
					this.RefreshCostOther(UI_Heal.ECostOtherType.Time, isPreview);
				}
			}
		}
	}

	// Token: 0x0600355C RID: 13660 RVA: 0x001ADA4C File Offset: 0x001ABC4C
	private void RefreshCostMaxCount()
	{
		int maxCount = this.IsSelectedPatientGearMate ? 4 : (this._expensiveHeal ? (this._needPlay ? 4 : 3) : (this._needPlay ? 3 : 2));
		List<Transform> CommonParameterVertical_Trans = new List<Transform>();
		for (int i = 0; i < this.costGrid.childCount; i++)
		{
			bool flag = this.costGrid.GetChild(i).GetComponent<HealCostItem>();
			if (flag)
			{
				CommonParameterVertical_Trans.Add(this.costGrid.GetChild(i));
			}
		}
		for (int j = 0; j < maxCount; j++)
		{
			bool flag2 = j < CommonParameterVertical_Trans.Count;
			if (flag2)
			{
				HealCostItem costGridChild = CommonParameterVertical_Trans[j].GetComponent<HealCostItem>();
				bool flag3 = !costGridChild.gameObject.activeSelf;
				if (flag3)
				{
					costGridChild.gameObject.SetActive(true);
				}
			}
		}
		for (int k = maxCount; k < CommonParameterVertical_Trans.Count; k++)
		{
			HealCostItem costGridChild2 = CommonParameterVertical_Trans[k].GetComponent<HealCostItem>();
			bool activeSelf = costGridChild2.gameObject.activeSelf;
			if (activeSelf)
			{
				costGridChild2.gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x0600355D RID: 13661 RVA: 0x001ADB83 File Offset: 0x001ABD83
	private void OnSelectItem(ItemDisplayData itemData)
	{
		this.RefreshGearMateToolItemView(itemData, false);
		this.AsyncGetRepairResultAndRefreshButtons();
		this.UpdateDoctorInfo();
	}

	// Token: 0x0600355E RID: 13662 RVA: 0x001ADB9D File Offset: 0x001ABD9D
	private void OnToolCellPointerEnter(ItemDisplayData itemData)
	{
		this._previewToolOverride = itemData;
		this.RefreshDoctorAttainmentPreview();
	}

	// Token: 0x0600355F RID: 13663 RVA: 0x001ADBB0 File Offset: 0x001ABDB0
	private void OnToolCellPointerExit(ItemDisplayData itemData)
	{
		ItemDisplayData previewToolOverride = this._previewToolOverride;
		ItemKey? itemKey = (previewToolOverride != null) ? new ItemKey?(previewToolOverride.Key) : null;
		ItemKey key = itemData.Key;
		bool flag = itemKey != null && (itemKey == null || itemKey.GetValueOrDefault() == key);
		if (flag)
		{
			this._previewToolOverride = null;
			this.UpdateDoctorInfo();
		}
	}

	// Token: 0x06003560 RID: 13664 RVA: 0x001ADC20 File Offset: 0x001ABE20
	private void RefreshDoctorAttainmentPreview()
	{
		bool flag = !this.IsSelectedPatientGearMate || !this.HasSelectedDoctor();
		if (!flag)
		{
			this.RefreshCharInfoLifeSkill(6, true, true, UI_Heal.EHealType.Invalid);
			this.RefreshCharInfoLifeSkill(7, true, true, UI_Heal.EHealType.Invalid);
			this.RefreshCharInfoLifeSkill(11, true, true, UI_Heal.EHealType.Invalid);
			this.RefreshCharInfoLifeSkill(10, true, true, UI_Heal.EHealType.Invalid);
		}
	}

	// Token: 0x06003561 RID: 13665 RVA: 0x001ADC78 File Offset: 0x001ABE78
	private void RefreshGearMateToolItemView(ItemDisplayData itemData, bool needAlpha = false)
	{
		bool hasItem = itemData != null;
		bool flag = hasItem;
		if (flag)
		{
			this.gearMateItem.Set(itemData, false);
			this.gearMateItemName.text = itemData.GetName(false);
		}
		this.gearMateEmptyTool.SetActive(!hasItem);
		this.gearMateSelectedItem.SetActive(hasItem);
	}

	// Token: 0x06003562 RID: 13666 RVA: 0x001ADCD0 File Offset: 0x001ABED0
	private void OnDoctorSortDataListChanged()
	{
		this.SortAndRefreshDoctorList();
	}

	// Token: 0x06003563 RID: 13667 RVA: 0x001ADCDC File Offset: 0x001ABEDC
	private void SortAndRefreshDoctorList()
	{
		SortAndFilterController<HealDoctorSortData> doctorSortController = this._doctorSortController;
		Comparison<HealDoctorSortData> comparer = (doctorSortController != null) ? doctorSortController.GenerateComparer(this._doctorSortDataList) : null;
		bool flag;
		if (comparer != null)
		{
			SortAndFilterController<HealDoctorSortData> doctorSortController2 = this._doctorSortController;
			flag = UI_Heal.HasActiveSort((doctorSortController2 != null) ? doctorSortController2.SortAndFilterState.SortData : null);
		}
		else
		{
			flag = false;
		}
		bool flag2 = flag;
		if (flag2)
		{
			this._doctorSortDataList.Sort(comparer);
		}
		else
		{
			this.SortDoctorByDefaultOrder();
		}
		this.doctorScroll.SetDataCount(this._doctorSortDataList.Count);
	}

	// Token: 0x06003564 RID: 13668 RVA: 0x001ADD5A File Offset: 0x001ABF5A
	private void OnPatientSortDataListChanged()
	{
		this.SortAndRefreshPatientList();
	}

	// Token: 0x06003565 RID: 13669 RVA: 0x001ADD64 File Offset: 0x001ABF64
	private void RefreshPatientSortData(int patientId)
	{
		int index = this._patientSortDataList.FindIndex((HealPatientSortData data) => data.CharId == patientId);
		bool flag = index < 0;
		if (!flag)
		{
			HealPatientSortData data2 = this._patientSortDataList[index];
			data2.TotalInjuries = this._patientInjuryDict.GetOrDefault(patientId).GetSum();
			data2.TotalPoisons = this._patientPoisonDict.GetOrDefault(patientId).Sum();
			data2.QiDisorder = (int)this._patientQiDisorderDict.GetOrDefault(patientId);
			data2.HealthPercent = UI_Heal.GetHealthPercent((int)this._patientHealthDict.GetOrDefault(patientId), (int)this._patientMaxHealthDict.GetOrDefault(patientId));
			data2.IsHealthUnknownOrNone = this.IsHealthUnknownOrNone(patientId, (int)this._patientHealthDict.GetOrDefault(patientId), (int)this._patientMaxHealthDict.GetOrDefault(patientId));
			this.SortAndRefreshPatientList();
		}
	}

	// Token: 0x06003566 RID: 13670 RVA: 0x001ADE70 File Offset: 0x001AC070
	private void RequestPatientAvailableFeature(int patientId)
	{
		CharacterDomainMethod.AsyncCall.GetAvailableFeature(this, patientId, delegate(int offset, RawDataPool pool)
		{
			List<short> featureIds = null;
			Serializer.Deserialize(pool, offset, ref featureIds);
			this._patientAvailableFeatureDict[patientId] = (featureIds ?? new List<short>());
			this.RefreshPatientSortData(patientId);
			this.patientScroll.RefreshCell(this._patientList.IndexOf(patientId));
			this.UpdatePatientInfo();
		});
	}

	// Token: 0x06003567 RID: 13671 RVA: 0x001ADEAC File Offset: 0x001AC0AC
	private void SortAndRefreshPatientList()
	{
		HealPatientSortAndFilterController patientSortController = this._patientSortController;
		Comparison<HealPatientSortData> comparer = (patientSortController != null) ? patientSortController.GenerateComparer(this._patientSortDataList) : null;
		bool flag;
		if (comparer != null)
		{
			HealPatientSortAndFilterController patientSortController2 = this._patientSortController;
			flag = UI_Heal.HasActiveSort((patientSortController2 != null) ? patientSortController2.SortAndFilterState.SortData : null);
		}
		else
		{
			flag = false;
		}
		bool flag2 = flag;
		if (flag2)
		{
			this._patientSortDataList.Sort(comparer);
		}
		else
		{
			this.SortPatientByDefaultOrder();
		}
		this.patientScroll.SetDataCount(this._patientSortDataList.Count);
	}

	// Token: 0x06003568 RID: 13672 RVA: 0x001ADF2C File Offset: 0x001AC12C
	private static bool HasActiveSort(SortStateData sortData)
	{
		bool flag = ((sortData != null) ? sortData.ItemStates : null) == null || sortData.ItemStates.Count == 0;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			for (int i = 0; i < sortData.ItemStates.Count; i++)
			{
				bool flag2 = sortData.ItemStates[i].SortDirection > ESortDirection.None;
				if (flag2)
				{
					return true;
				}
			}
			result = false;
		}
		return result;
	}

	// Token: 0x06003569 RID: 13673 RVA: 0x001ADFA0 File Offset: 0x001AC1A0
	private void RefreshHealButtonsForNoSelection()
	{
		this.healInjury.interactable = false;
		this.healPoison.interactable = false;
		this.healQiDisorder.interactable = false;
		this.healHealth.interactable = false;
		this.healOutInjury.interactable = false;
		this.healInnerInjury.interactable = false;
		this.gearMateHealPoison.interactable = false;
		this.gearMateHealQiDisorder.interactable = false;
		this.healInjury.GetComponent<TooltipInvoker>().enabled = false;
		this.healPoison.GetComponent<TooltipInvoker>().enabled = false;
		this.healQiDisorder.GetComponent<TooltipInvoker>().enabled = false;
		this.healHealth.GetComponent<TooltipInvoker>().enabled = false;
		this.healOutInjury.GetComponent<TooltipInvoker>().enabled = false;
		this.healInnerInjury.GetComponent<TooltipInvoker>().enabled = false;
		this.gearMateHealPoison.GetComponent<TooltipInvoker>().enabled = false;
		this.gearMateHealQiDisorder.GetComponent<TooltipInvoker>().enabled = false;
		this.healInjuryText.text = LocalStringManager.Get(LanguageKey.LK_Heal_Injury).SetColor(this.btnTextInteractableColor);
		this.healPoisonText.text = LocalStringManager.Get(LanguageKey.LK_Heal_Poison).SetColor(this.btnTextInteractableColor);
		this.healQiDisorderText.text = LocalStringManager.Get(LanguageKey.LK_Heal_QiDisorder).SetColor(this.btnTextInteractableColor);
		this.healHealthText.text = LocalStringManager.Get(LanguageKey.LK_Heal_Health).SetColor(this.btnTextInteractableColor);
		this.healOutInjuryText.text = LocalStringManager.Get(LanguageKey.LK_Heal_OutInjury).SetColor(this.btnTextInteractableColor);
		this.healInnerInjuryText.text = LocalStringManager.Get(LanguageKey.LK_Heal_InnerInjury).SetColor(this.btnTextInteractableColor);
		this.healGearMatePoisonText.text = LocalStringManager.Get(LanguageKey.LK_Heal_Poison).SetColor(this.btnTextInteractableColor);
		this.healGearMateQiDisorderText.text = LocalStringManager.Get(LanguageKey.LK_Heal_QiDisorder).SetColor(this.btnTextInteractableColor);
		bool flag = this.btnAutoPut != null;
		if (flag)
		{
			this.btnAutoPut.interactable = false;
		}
	}

	// Token: 0x0600356A RID: 13674 RVA: 0x001AE1CC File Offset: 0x001AC3CC
	private void RefreshCurrentSelectionDisplay()
	{
		bool hasSelectedDoctor = this.HasSelectedDoctor();
		bool hasSelectedPatient = this.HasSelectedPatient();
		this.healCurrDoctor.gameObject.SetActive(true);
		this.healCurrPatient.gameObject.SetActive(true);
		UI_Heal.SetCurrentCharacterDisplayVisible(this.healCurrDoctor, hasSelectedDoctor);
		UI_Heal.SetCurrentCharacterDisplayVisible(this.healCurrPatient, hasSelectedPatient);
		this.currentDoctorEmptyNode.SetActive(!hasSelectedDoctor);
		this.currentPatientEmptyNode.SetActive(!hasSelectedPatient);
		bool flag = !hasSelectedDoctor && hasSelectedPatient && this.IsSelectedPatientGearMate;
		if (flag)
		{
			this.currentDoctorEmptyText.text = LocalStringManager.Get(LanguageKey.LK_Heal_NoSelection_Tip_Doctor_For_GearMate);
		}
		else
		{
			this.currentDoctorEmptyText.text = LocalStringManager.Get(LanguageKey.LK_Heal_NoSelection_Tip_Doctor);
		}
	}

	// Token: 0x0600356B RID: 13675 RVA: 0x001AE289 File Offset: 0x001AC489
	private static void SetCurrentCharacterDisplayVisible(HealCurrDoctor currentCharacter, bool isVisible)
	{
		currentCharacter.avatar.gameObject.SetActive(isVisible);
		currentCharacter.nameFrame.gameObject.SetActive(isVisible);
		currentCharacter.grid.gameObject.SetActive(isVisible);
	}

	// Token: 0x0600356C RID: 13676 RVA: 0x001AE2C4 File Offset: 0x001AC4C4
	private void SortDoctorByDefaultOrder()
	{
		bool flag = this._originDoctorList.Count == 0;
		if (!flag)
		{
			Dictionary<int, int> orderMap = new Dictionary<int, int>(this._originDoctorList.Count);
			for (int i = 0; i < this._originDoctorList.Count; i++)
			{
				orderMap[this._originDoctorList[i]] = i;
			}
			this._doctorSortDataList.Sort(delegate(HealDoctorSortData a, HealDoctorSortData b)
			{
				int aIndex;
				int aOrder = orderMap.TryGetValue(a.CharId, out aIndex) ? aIndex : int.MaxValue;
				int bIndex;
				int bOrder = orderMap.TryGetValue(b.CharId, out bIndex) ? bIndex : int.MaxValue;
				return aOrder.CompareTo(bOrder);
			});
		}
	}

	// Token: 0x0600356D RID: 13677 RVA: 0x001AE350 File Offset: 0x001AC550
	private void SortPatientByDefaultOrder()
	{
		bool flag = this._originPatientList.Count == 0;
		if (!flag)
		{
			Dictionary<int, int> orderMap = new Dictionary<int, int>(this._originPatientList.Count);
			for (int i = 0; i < this._originPatientList.Count; i++)
			{
				orderMap[this._originPatientList[i]] = i;
			}
			this._patientSortDataList.Sort(delegate(HealPatientSortData a, HealPatientSortData b)
			{
				int aIndex;
				int aOrder = orderMap.TryGetValue(a.CharId, out aIndex) ? aIndex : int.MaxValue;
				int bIndex;
				int bOrder = orderMap.TryGetValue(b.CharId, out bIndex) ? bIndex : int.MaxValue;
				return aOrder.CompareTo(bOrder);
			});
		}
	}

	// Token: 0x0600356E RID: 13678 RVA: 0x001AE3DC File Offset: 0x001AC5DC
	public static int GetHealthPercent(int current, int max)
	{
		bool flag = max == 0;
		int result;
		if (flag)
		{
			result = 0;
		}
		else
		{
			result = current * 100 / max;
		}
		return result;
	}

	// Token: 0x0600356F RID: 13679 RVA: 0x001AE400 File Offset: 0x001AC600
	private EHealthType GetPatientHealthType(int characterId, int currentHealth, int maxHealth)
	{
		List<short> availableFeatureIds;
		bool flag = this._patientAvailableFeatureDict.TryGetValue(characterId, out availableFeatureIds);
		EHealthType result;
		if (flag)
		{
			result = HealthTypeHelper.CalcType(availableFeatureIds, (short)currentHealth, (short)maxHealth);
		}
		else
		{
			CharacterDisplayData displayData;
			bool flag2 = this._displayDataDict.TryGetValue(characterId, out displayData) && displayData.FeatureIds != null;
			if (flag2)
			{
				result = HealthTypeHelper.CalcType(displayData.FeatureIds, (short)currentHealth, (short)maxHealth);
			}
			else
			{
				result = HealthTypeHelper.CalcType((short)currentHealth, (short)maxHealth);
			}
		}
		return result;
	}

	// Token: 0x06003570 RID: 13680 RVA: 0x001AE470 File Offset: 0x001AC670
	private bool IsHealthUnknownOrNone(int characterId, int currentHealth, int maxHealth)
	{
		bool flag = maxHealth <= 0;
		return flag || this.GetPatientHealthType(characterId, currentHealth, maxHealth) == EHealthType.Unknown;
	}

	// Token: 0x06003571 RID: 13681 RVA: 0x001AE49D File Offset: 0x001AC69D
	public static bool IsQiDisorderMax(int qiDisorder)
	{
		return qiDisorder * 2 >= (int)DisorderLevelOfQi.MaxValue;
	}

	// Token: 0x06003572 RID: 13682 RVA: 0x001AE4AC File Offset: 0x001AC6AC
	public static bool IsDying(int health)
	{
		return health <= 0;
	}

	// Token: 0x06003573 RID: 13683 RVA: 0x001AE4B5 File Offset: 0x001AC6B5
	public unsafe static sbyte GetMaxAttainmentForNormal(LifeSkillShorts attainments)
	{
		return (*attainments[9] > *attainments[8]) ? 9 : 8;
	}

	// Token: 0x0400267E RID: 9854
	private static readonly sbyte[] GearMateRelatedLifeSkillTypeArray = new sbyte[]
	{
		6,
		7,
		11,
		10
	};

	// Token: 0x0400267F RID: 9855
	private readonly List<int> _doctorList = new List<int>();

	// Token: 0x04002680 RID: 9856
	private readonly List<int> _patientList = new List<int>();

	// Token: 0x04002681 RID: 9857
	private readonly List<int> _originDoctorList = new List<int>();

	// Token: 0x04002682 RID: 9858
	private readonly List<int> _originPatientList = new List<int>();

	// Token: 0x04002683 RID: 9859
	private bool _needPlay;

	// Token: 0x04002684 RID: 9860
	private bool _expensiveHeal = true;

	// Token: 0x04002685 RID: 9861
	private const sbyte ExpensiveHealCoefficient = 2;

	// Token: 0x04002686 RID: 9862
	private int _currentSpiritualDebtCost;

	// Token: 0x04002687 RID: 9863
	private Location _doctorOrgLocation = Location.Invalid;

	// Token: 0x04002688 RID: 9864
	private int _taiwuCharId;

	// Token: 0x04002689 RID: 9865
	private int _currentCharacterId;

	// Token: 0x0400268A RID: 9866
	private readonly Dictionary<int, MapHealSimulateResult> _healInjuryCostDict = new Dictionary<int, MapHealSimulateResult>();

	// Token: 0x0400268B RID: 9867
	private readonly Dictionary<int, MapHealSimulateResult> _healPoisonCostDict = new Dictionary<int, MapHealSimulateResult>();

	// Token: 0x0400268C RID: 9868
	private readonly Dictionary<int, MapHealSimulateResult> _healQiDisorderCostDict = new Dictionary<int, MapHealSimulateResult>();

	// Token: 0x0400268D RID: 9869
	private readonly Dictionary<int, MapHealSimulateResult> _healHealthCostDict = new Dictionary<int, MapHealSimulateResult>();

	// Token: 0x0400268E RID: 9870
	private readonly Dictionary<int, CharacterDisplayData> _displayDataDict = new Dictionary<int, CharacterDisplayData>();

	// Token: 0x0400268F RID: 9871
	private readonly Dictionary<int, CombatResources> _doctorHealCountDict = new Dictionary<int, CombatResources>();

	// Token: 0x04002690 RID: 9872
	private readonly Dictionary<int, GearMateRepairCount> _doctorHealGearMateCountDict = new Dictionary<int, GearMateRepairCount>();

	// Token: 0x04002691 RID: 9873
	private readonly Dictionary<int, LifeSkillShorts> _doctorAttainmentDict = new Dictionary<int, LifeSkillShorts>();

	// Token: 0x04002692 RID: 9874
	private ResourceInts _taiwuResource;

	// Token: 0x04002693 RID: 9875
	private readonly Dictionary<int, Injuries> _patientInjuryDict = new Dictionary<int, Injuries>();

	// Token: 0x04002694 RID: 9876
	private readonly Dictionary<int, PoisonInts> _patientPoisonDict = new Dictionary<int, PoisonInts>();

	// Token: 0x04002695 RID: 9877
	private readonly Dictionary<int, short> _patientQiDisorderDict = new Dictionary<int, short>();

	// Token: 0x04002696 RID: 9878
	private readonly Dictionary<int, short> _patientHealthDict = new Dictionary<int, short>();

	// Token: 0x04002697 RID: 9879
	private readonly Dictionary<int, short> _patientMaxHealthDict = new Dictionary<int, short>();

	// Token: 0x04002698 RID: 9880
	private readonly Dictionary<int, List<short>> _patientAvailableFeatureDict = new Dictionary<int, List<short>>();

	// Token: 0x04002699 RID: 9881
	private readonly Dictionary<int, List<GearMateRepairRequirementDisplayData>> _gearMateRepairRequirementDict = new Dictionary<int, List<GearMateRepairRequirementDisplayData>>();

	// Token: 0x0400269A RID: 9882
	private readonly Dictionary<sbyte, int> _currentGearMateCanRepair = new Dictionary<sbyte, int>();

	// Token: 0x0400269B RID: 9883
	private readonly Dictionary<sbyte, int> _currentGearMateRepairEffect = new Dictionary<sbyte, int>();

	// Token: 0x0400269C RID: 9884
	private readonly Dictionary<sbyte, ItemDisplayData> _currentGearMateRepairAutoTool = new Dictionary<sbyte, ItemDisplayData>();

	// Token: 0x0400269D RID: 9885
	private float _closeAnimTime = -1f;

	// Token: 0x0400269E RID: 9886
	private int _selectedDoctor = -1;

	// Token: 0x0400269F RID: 9887
	private int _selectedPatient = -1;

	// Token: 0x040026A0 RID: 9888
	private bool _referInited;

	// Token: 0x040026A1 RID: 9889
	private UI_Heal.ItemSelector _itemSelector;

	// Token: 0x040026A2 RID: 9890
	private ItemKey _emptyToolKey;

	// Token: 0x040026A3 RID: 9891
	private ItemDisplayData _previewToolOverride;

	// Token: 0x040026A4 RID: 9892
	private bool _isInitItemSelector;

	// Token: 0x040026A5 RID: 9893
	private SortAndFilterController<HealDoctorSortData> _doctorSortController;

	// Token: 0x040026A6 RID: 9894
	private List<HealDoctorSortData> _doctorSortDataList = new List<HealDoctorSortData>();

	// Token: 0x040026A7 RID: 9895
	private HealPatientSortAndFilterController _patientSortController;

	// Token: 0x040026A8 RID: 9896
	private List<HealPatientSortData> _patientSortDataList = new List<HealPatientSortData>();

	// Token: 0x040026A9 RID: 9897
	private const string EmptyAvatarBack = "ui9_back_heal_player_0";

	// Token: 0x040026AA RID: 9898
	private const string PatientAvatarBack = "ui9_back_heal_player_1";

	// Token: 0x040026AB RID: 9899
	private const string DoctorAvatarBack = "ui9_back_heal_player_2";

	// Token: 0x040026AC RID: 9900
	private const string GearMatePatientAvatarBack = "ui9_back_heal_player_3";

	// Token: 0x040026AD RID: 9901
	private const string GearMateDoctorAvatarBack = "ui9_back_heal_player_4";

	// Token: 0x040026AE RID: 9902
	[SerializeField]
	private HealCurrDoctor healCurrDoctor;

	// Token: 0x040026AF RID: 9903
	[SerializeField]
	private HealCurrDoctor healCurrPatient;

	// Token: 0x040026B0 RID: 9904
	[SerializeField]
	private GameObject currentDoctorEmptyNode;

	// Token: 0x040026B1 RID: 9905
	[SerializeField]
	private TextMeshProUGUI currentDoctorEmptyText;

	// Token: 0x040026B2 RID: 9906
	[SerializeField]
	private GameObject currentPatientEmptyNode;

	// Token: 0x040026B3 RID: 9907
	[SerializeField]
	private HealGearMateSelectedToolDurability healGearMateSelectedToolDurability;

	// Token: 0x040026B4 RID: 9908
	[SerializeField]
	private GameObject btnBack;

	// Token: 0x040026B5 RID: 9909
	[SerializeField]
	private GameObject itemSelectorRefers;

	// Token: 0x040026B6 RID: 9910
	[SerializeField]
	private ListStyleGeneralScroll itemSelectorScroll;

	// Token: 0x040026B7 RID: 9911
	[SerializeField]
	private GameObject midItemRoot;

	// Token: 0x040026B8 RID: 9912
	[SerializeField]
	private InfinityScroll doctorScroll;

	// Token: 0x040026B9 RID: 9913
	[SerializeField]
	private InfinityScroll patientScroll;

	// Token: 0x040026BA RID: 9914
	[SerializeField]
	private CButton healHealth;

	// Token: 0x040026BB RID: 9915
	[SerializeField]
	private CButton healInjury;

	// Token: 0x040026BC RID: 9916
	[SerializeField]
	private CButton gearMateHealPoison;

	// Token: 0x040026BD RID: 9917
	[SerializeField]
	private CButton gearMateHealQiDisorder;

	// Token: 0x040026BE RID: 9918
	[SerializeField]
	private CButton healInnerInjury;

	// Token: 0x040026BF RID: 9919
	[SerializeField]
	private CButton healOutInjury;

	// Token: 0x040026C0 RID: 9920
	[SerializeField]
	private CButton healPoison;

	// Token: 0x040026C1 RID: 9921
	[SerializeField]
	private CButton healQiDisorder;

	// Token: 0x040026C2 RID: 9922
	[SerializeField]
	private Transform costGrid;

	// Token: 0x040026C3 RID: 9923
	[SerializeField]
	private GameObject gearMateSelectedItem;

	// Token: 0x040026C4 RID: 9924
	[SerializeField]
	private ItemBack gearMateItem;

	// Token: 0x040026C5 RID: 9925
	[SerializeField]
	private TextMeshProUGUI gearMateItemName;

	// Token: 0x040026C6 RID: 9926
	[SerializeField]
	private GameObject background;

	// Token: 0x040026C7 RID: 9927
	[SerializeField]
	private RectTransform buttonGrid;

	// Token: 0x040026C8 RID: 9928
	[SerializeField]
	private CanvasGroup canvasFront;

	// Token: 0x040026C9 RID: 9929
	[SerializeField]
	private TextMeshProUGUI doctorTitle;

	// Token: 0x040026CA RID: 9930
	[SerializeField]
	private RectTransform gearMateButtonGrid;

	// Token: 0x040026CB RID: 9931
	[SerializeField]
	private GameObject gearMateEmptyTool;

	// Token: 0x040026CC RID: 9932
	[SerializeField]
	private GameObject gearMateSelectedTool;

	// Token: 0x040026CD RID: 9933
	[SerializeField]
	private SkeletonGraphic healAni;

	// Token: 0x040026CE RID: 9934
	[SerializeField]
	private TextMeshProUGUI healGearMatePoisonCount;

	// Token: 0x040026CF RID: 9935
	[SerializeField]
	private TextMeshProUGUI healGearMatePoisonText;

	// Token: 0x040026D0 RID: 9936
	[SerializeField]
	private TextMeshProUGUI healGearMateQiDisorderCount;

	// Token: 0x040026D1 RID: 9937
	[SerializeField]
	private TextMeshProUGUI healGearMateQiDisorderText;

	// Token: 0x040026D2 RID: 9938
	[SerializeField]
	private TextMeshProUGUI healHealthCount;

	// Token: 0x040026D3 RID: 9939
	[SerializeField]
	private TextMeshProUGUI healHealthText;

	// Token: 0x040026D4 RID: 9940
	[SerializeField]
	private TextMeshProUGUI healInjuryCount;

	// Token: 0x040026D5 RID: 9941
	[SerializeField]
	private TextMeshProUGUI healInjuryText;

	// Token: 0x040026D6 RID: 9942
	[SerializeField]
	private TextMeshProUGUI healInnerInjuryCount;

	// Token: 0x040026D7 RID: 9943
	[SerializeField]
	private TextMeshProUGUI healInnerInjuryText;

	// Token: 0x040026D8 RID: 9944
	[SerializeField]
	private TextMeshProUGUI healOutInjuryCount;

	// Token: 0x040026D9 RID: 9945
	[SerializeField]
	private TextMeshProUGUI healOutInjuryText;

	// Token: 0x040026DA RID: 9946
	[SerializeField]
	private TextMeshProUGUI healPoisonCount;

	// Token: 0x040026DB RID: 9947
	[SerializeField]
	private TextMeshProUGUI healPoisonText;

	// Token: 0x040026DC RID: 9948
	[SerializeField]
	private TextMeshProUGUI healQiDisorderCount;

	// Token: 0x040026DD RID: 9949
	[SerializeField]
	private TextMeshProUGUI healQiDisorderText;

	// Token: 0x040026DE RID: 9950
	[SerializeField]
	private TextMeshProUGUI patientTitle;

	// Token: 0x040026DF RID: 9951
	[SerializeField]
	private TextMeshProUGUI doctorTitle2;

	// Token: 0x040026E0 RID: 9952
	[SerializeField]
	private TextMeshProUGUI patientTitle2;

	// Token: 0x040026E1 RID: 9953
	[SerializeField]
	private SortAndFilter doctorSortAndFilter;

	// Token: 0x040026E2 RID: 9954
	[SerializeField]
	private SortAndFilter patientSortAndFilter;

	// Token: 0x040026E3 RID: 9955
	[SerializeField]
	private Color btnTextNormalColor;

	// Token: 0x040026E4 RID: 9956
	[SerializeField]
	private Color btnTextInteractableColor;

	// Token: 0x040026E5 RID: 9957
	[SerializeField]
	private CButton btnAutoPut;

	// Token: 0x0200178A RID: 6026
	private enum ECostOtherType
	{
		// Token: 0x0400ABF5 RID: 44021
		Time,
		// Token: 0x0400ABF6 RID: 44022
		SpiritualDebt
	}

	// Token: 0x0200178B RID: 6027
	private class ItemSelector
	{
		// Token: 0x170016AE RID: 5806
		// (get) Token: 0x0600D45A RID: 54362 RVA: 0x005B5ECE File Offset: 0x005B40CE
		public ItemDisplayData SelectedItem
		{
			get
			{
				return this._selectedItem;
			}
		}

		// Token: 0x170016AF RID: 5807
		// (get) Token: 0x0600D45B RID: 54363 RVA: 0x005B5ED6 File Offset: 0x005B40D6
		public List<ItemDisplayData> Items
		{
			get
			{
				return this._items;
			}
		}

		// Token: 0x170016B0 RID: 5808
		// (get) Token: 0x0600D45C RID: 54364 RVA: 0x005B5EDE File Offset: 0x005B40DE
		public List<ItemDisplayData> ShowingItems
		{
			get
			{
				return this._filteredItem;
			}
		}

		// Token: 0x0600D45D RID: 54365 RVA: 0x005B5EE8 File Offset: 0x005B40E8
		public ItemSelector(UI_Heal parent, ListStyleGeneralScroll itemScrollView)
		{
			this._parent = parent;
			this._items = new List<ItemDisplayData>();
			this._itemScrollView = itemScrollView;
			this._itemScrollView.Init<ITradeableContent>(this.GenerateColumnDefinitions(), true, new Action<int, GameObject>(this.OnRenderItem), new Action<int, RowItem>(this.OnClickItem));
		}

		// Token: 0x0600D45E RID: 54366 RVA: 0x005B5F4C File Offset: 0x005B414C
		public void ReRenderByAutoSelectedItem(ItemDisplayData itemData)
		{
			this._autoSelectedItem = itemData;
			this.RefreshScrollView();
		}

		// Token: 0x0600D45F RID: 54367 RVA: 0x005B5F5D File Offset: 0x005B415D
		public void SetSelectedItem(ItemDisplayData itemData)
		{
			this._selectedItem = itemData;
		}

		// Token: 0x0600D460 RID: 54368 RVA: 0x005B5F68 File Offset: 0x005B4168
		private void OnRenderItem(int index, GameObject go)
		{
			RowItem item = go.GetComponent<RowItem>();
			ItemDisplayData itemData = this._filteredItem[index];
			RowItem rowItem = item;
			ItemKey key = itemData.Key;
			ItemDisplayData selectedItem = this._selectedItem;
			bool selected;
			if (!(key == ((selectedItem != null) ? new ItemKey?(selectedItem.Key) : null)))
			{
				key = itemData.Key;
				ItemDisplayData autoSelectedItem = this._autoSelectedItem;
				selected = (key == ((autoSelectedItem != null) ? new ItemKey?(autoSelectedItem.Key) : null));
			}
			else
			{
				selected = true;
			}
			rowItem.SetSelected(selected);
			item.OnPointerEnterEvent = delegate()
			{
				this._parent.OnToolCellPointerEnter(itemData);
			};
			item.OnPointerExitEvent = delegate()
			{
				this._parent.OnToolCellPointerExit(itemData);
			};
		}

		// Token: 0x0600D461 RID: 54369 RVA: 0x005B6058 File Offset: 0x005B4258
		private void OnClickItem(int index, RowItem item)
		{
			ItemDisplayData itemData = this._filteredItem[index];
			bool flag = this._selectedItem == null || this._selectedItem.Key != itemData.Key;
			if (flag)
			{
				this.SetSelectedItem(itemData);
				this._parent.OnSelectItem(itemData);
				this.FilterItem();
				this.RefreshScrollView();
			}
			else
			{
				this.SetSelectedItem(null);
				this._parent.OnSelectItem(null);
				this.FilterItem();
				this.RefreshScrollView();
			}
		}

		// Token: 0x0600D462 RID: 54370 RVA: 0x005B60E4 File Offset: 0x005B42E4
		public void RefreshItemsByCharacter(int charId, Action callback = null)
		{
			CharacterDomainMethod.AsyncCall.GetAllInventoryItems(this._parent, charId, delegate(int offset, RawDataPool pool)
			{
				this._items = new List<ItemDisplayData>();
				Serializer.Deserialize(pool, offset, ref this._items);
				this.FilterItem();
				this.RefreshScrollView();
				Action callback2 = callback;
				if (callback2 != null)
				{
					callback2();
				}
			});
		}

		// Token: 0x0600D463 RID: 54371 RVA: 0x005B6120 File Offset: 0x005B4320
		private void FilterItem()
		{
			this._filteredItem.Clear();
			ItemDisplayData emptyItem = new ItemDisplayData
			{
				Key = this._parent._emptyToolKey
			};
			this._filteredItem.Add(emptyItem);
			this._filteredItem.AddRange(this._items);
			this._filteredItem.RemoveAll((ItemDisplayData d) => !this.IsValidItem(d));
		}

		// Token: 0x0600D464 RID: 54372 RVA: 0x005B618C File Offset: 0x005B438C
		private bool IsValidItem(ItemDisplayData d)
		{
			bool flag = d.Key.ItemType != 6;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				CraftToolItem craftConfig = CraftTool.Instance[d.Key.TemplateId];
				bool containsValidSkillType = false;
				foreach (sbyte relatedType in craftConfig.RequiredLifeSkillTypes)
				{
					bool flag2 = relatedType == 6 || relatedType == 7 || relatedType == 11 || relatedType == 10;
					if (flag2)
					{
						containsValidSkillType = true;
						break;
					}
				}
				result = containsValidSkillType;
			}
			return result;
		}

		// Token: 0x0600D465 RID: 54373 RVA: 0x005B623C File Offset: 0x005B443C
		public void RefreshScrollView()
		{
			bool flag = this.SelectedItem != null;
			if (flag)
			{
				ItemDisplayData realSelectedItem = this._filteredItem.Find((ItemDisplayData d) => d.Key.Equals(this.SelectedItem.Key));
				this.SetSelectedItem(realSelectedItem);
			}
			this._itemScrollView.SetData<ItemDisplayData>(this._filteredItem, -1);
		}

		// Token: 0x0600D466 RID: 54374 RVA: 0x005B6290 File Offset: 0x005B4490
		public void Clear()
		{
			this._selectedItem = null;
			List<ItemDisplayData> items = this._items;
			if (items != null)
			{
				items.Clear();
			}
			List<ItemDisplayData> filteredItem = this._filteredItem;
			if (filteredItem != null)
			{
				filteredItem.Clear();
			}
			ListStyleGeneralScroll itemScrollView = this._itemScrollView;
			if (itemScrollView != null)
			{
				itemScrollView.ClearInfinityScrollCache();
			}
			this._parent._previewToolOverride = null;
		}

		// Token: 0x0600D467 RID: 54375 RVA: 0x005B62E7 File Offset: 0x005B44E7
		private IEnumerable<ColumnDefinition> GenerateColumnDefinitions()
		{
			using (IEnumerator enumerator = Enum.GetValues(typeof(UI_Heal.ItemSelector.EColumnType)).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					switch ((UI_Heal.ItemSelector.EColumnType)enumerator.Current)
					{
					case UI_Heal.ItemSelector.EColumnType.IconAndName:
						yield return this.CreateIconAndNameColumn();
						break;
					case UI_Heal.ItemSelector.EColumnType.PropertyReferenced:
						yield return this.CreatePropertyReferenced();
						break;
					case UI_Heal.ItemSelector.EColumnType.Durability:
						yield return this.CreateDurability();
						break;
					}
				}
			}
			IEnumerator enumerator = null;
			yield break;
			yield break;
		}

		// Token: 0x0600D468 RID: 54376 RVA: 0x005B62F8 File Offset: 0x005B44F8
		private ColumnDefinition CreateIconAndNameColumn()
		{
			ColumnDefinition<ITradeableContent, ITradeableContent> columnDefinition = new ColumnDefinition<ITradeableContent, ITradeableContent>();
			columnDefinition.LayoutOption = new LayoutOption
			{
				MinWidth = 200f,
				FlexibleWidth = 1f,
				PreferredWidth = 400f,
				Priority = 1
			};
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_Heal_ToolSelector_Column_Name.Tr());
			columnDefinition.CellDataGenerator = ((ITradeableContent data) => data);
			columnDefinition.SortId = 0;
			return columnDefinition;
		}

		// Token: 0x0600D469 RID: 54377 RVA: 0x005B639C File Offset: 0x005B459C
		private ColumnDefinition CreatePropertyReferenced()
		{
			ColumnDefinition<ITradeableContent, string> columnDefinition = new ColumnDefinition<ITradeableContent, string>();
			columnDefinition.LayoutOption = new LayoutOption
			{
				MinWidth = 200f,
				FlexibleWidth = 1f,
				PreferredWidth = 400f,
				Priority = 1
			};
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_Heal_ToolSelector_Column_Effect.Tr());
			columnDefinition.CellDataGenerator = new Func<ITradeableContent, string>(this.GetPropertyReferencedString);
			columnDefinition.SortId = 17;
			return columnDefinition;
		}

		// Token: 0x0600D46A RID: 54378 RVA: 0x005B6430 File Offset: 0x005B4630
		private string GetPropertyReferencedString(ITradeableContent data)
		{
			CraftToolItem configData = CraftTool.Instance[data.Key.TemplateId];
			short attainment = UI_Make.GetToolAttainment(data.Key.TemplateId, -1);
			bool flag = attainment == 0;
			string result;
			if (flag)
			{
				result = LanguageKey.LK_SelectItem_ItemCell_ToolEffect_None.Tr();
			}
			else
			{
				string str = "";
				foreach (sbyte lifeSkillType in configData.RequiredLifeSkillTypes)
				{
					LifeSkillTypeItem skillConfig = Config.LifeSkillType.Instance[lifeSkillType];
					str += string.Format("<SpName={0}>+{1} ", skillConfig.Icon, attainment);
				}
				result = str;
			}
			return result;
		}

		// Token: 0x0600D46B RID: 54379 RVA: 0x005B64FC File Offset: 0x005B46FC
		private ColumnDefinition CreateDurability()
		{
			ColumnDefinition<ITradeableContent, string> columnDefinition = new ColumnDefinition<ITradeableContent, string>();
			columnDefinition.LayoutOption = new LayoutOption
			{
				MinWidth = 200f,
				FlexibleWidth = 1f,
				PreferredWidth = 400f,
				Priority = 1
			};
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_Heal_ToolSelector_Column_Durability.Tr());
			columnDefinition.CellDataGenerator = ((ITradeableContent data) => (data.Key == this._parent._emptyToolKey) ? "-/-" : string.Format("{0}/{1}", data.Durability, data.MaxDurability));
			columnDefinition.SortId = 5;
			return columnDefinition;
		}

		// Token: 0x0400ABF7 RID: 44023
		private UI_Heal _parent;

		// Token: 0x0400ABF8 RID: 44024
		private List<ItemDisplayData> _items;

		// Token: 0x0400ABF9 RID: 44025
		private List<ItemDisplayData> _filteredItem = new List<ItemDisplayData>();

		// Token: 0x0400ABFA RID: 44026
		private ItemDisplayData _selectedItem;

		// Token: 0x0400ABFB RID: 44027
		private ItemDisplayData _autoSelectedItem;

		// Token: 0x0400ABFC RID: 44028
		private ListStyleGeneralScroll _itemScrollView;

		// Token: 0x020026BE RID: 9918
		[Flags]
		private enum EColumnType
		{
			// Token: 0x0400EB5E RID: 60254
			IconAndName = 1,
			// Token: 0x0400EB5F RID: 60255
			PropertyReferenced = 2,
			// Token: 0x0400EB60 RID: 60256
			Durability = 4
		}
	}

	// Token: 0x0200178C RID: 6028
	public static class IconHelper
	{
		// Token: 0x0600D46F RID: 54383 RVA: 0x005B660C File Offset: 0x005B480C
		public static string GetInjuryIcon(int sum)
		{
			return (sum > 0) ? "sp_heal_icon_2" : "sp_heal_icon_0";
		}

		// Token: 0x0600D470 RID: 54384 RVA: 0x005B6630 File Offset: 0x005B4830
		public static string GetOutInjuryIcon(int sum)
		{
			return (sum > 0) ? "sp_heal_icon_6" : "sp_heal_icon_4";
		}

		// Token: 0x0600D471 RID: 54385 RVA: 0x005B6654 File Offset: 0x005B4854
		public static string GetInnerInjuryIcon(int sum)
		{
			return (sum > 0) ? "sp_heal_icon_7" : "sp_heal_icon_5";
		}

		// Token: 0x0600D472 RID: 54386 RVA: 0x005B6678 File Offset: 0x005B4878
		public static string GetPoisonIcon(int sum)
		{
			return (sum > 0) ? "sp_heal_icon_3" : "sp_heal_icon_1";
		}

		// Token: 0x0600D473 RID: 54387 RVA: 0x005B669C File Offset: 0x005B489C
		public static string GetQiDisorderIcon(bool isMax)
		{
			return isMax ? "sp_combat_icon_qi_4" : "sp_combat_icon_qi_5";
		}

		// Token: 0x0600D474 RID: 54388 RVA: 0x005B66C0 File Offset: 0x005B48C0
		public static string GetHealthIcon(bool isDying)
		{
			return isDying ? "sp_icon_jiankang_1" : "sp_icon_jiankang_0";
		}
	}

	// Token: 0x0200178D RID: 6029
	public enum EHealType
	{
		// Token: 0x0400ABFE RID: 44030
		Invalid,
		// Token: 0x0400ABFF RID: 44031
		Injury,
		// Token: 0x0400AC00 RID: 44032
		OuterInjury,
		// Token: 0x0400AC01 RID: 44033
		InnerInjury,
		// Token: 0x0400AC02 RID: 44034
		Poison,
		// Token: 0x0400AC03 RID: 44035
		QiDisorder,
		// Token: 0x0400AC04 RID: 44036
		Health
	}
}
