using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Extra;
using GameData.Domains.Taiwu;
using GameData.Domains.Taiwu.Profession;
using GameData.Domains.Taiwu.Profession.SkillsData;
using GameData.Domains.TaiwuEvent;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

// Token: 0x020002F3 RID: 755
public class ProfessionBottomMenu : Refers
{
	// Token: 0x170004D9 RID: 1241
	// (get) Token: 0x06002C20 RID: 11296 RVA: 0x00159383 File Offset: 0x00157583
	private ProfessionModel Model
	{
		get
		{
			return SingletonObject.getInstance<ProfessionModel>();
		}
	}

	// Token: 0x06002C21 RID: 11297 RVA: 0x0015938A File Offset: 0x0015758A
	private void OnEnable()
	{
		this.RefreshByEvent(null);
		GEvent.Add(UiEvents.OnProfessionDataChange, new GEvent.Callback(this.RefreshByEvent));
	}

	// Token: 0x06002C22 RID: 11298 RVA: 0x001593B1 File Offset: 0x001575B1
	private void OnDisable()
	{
		GEvent.Remove(UiEvents.OnProfessionDataChange, new GEvent.Callback(this.RefreshByEvent));
	}

	// Token: 0x06002C23 RID: 11299 RVA: 0x001593D0 File Offset: 0x001575D0
	public void Init(IAsyncMethodRequestHandler handler)
	{
		this._handler = handler;
		bool flag = !this._inited;
		if (flag)
		{
			this.InitRefers();
			this.InitSlotRefers();
		}
		this._inited = true;
	}

	// Token: 0x06002C24 RID: 11300 RVA: 0x00159409 File Offset: 0x00157609
	public void RefreshByEvent(ArgumentBox argbox)
	{
		TaiwuDomainMethod.AsyncCall.RequestTaiwuResourceDisplayData(null, delegate(int offset, RawDataPool pool)
		{
			TaiwuResourceDisplayData data = new TaiwuResourceDisplayData();
			Serializer.Deserialize(pool, offset, ref data);
			this._resources = new ResourceInts(data.Resources);
			this._exp = data.Exp;
			this.Refresh();
		});
	}

	// Token: 0x06002C25 RID: 11301 RVA: 0x00159420 File Offset: 0x00157620
	public void Refresh()
	{
		this.RefreshExtraSkill();
		for (int i = 0; i < this._equipSkillItems.Count; i++)
		{
			List<Refers> subList = this._equipSkillItems[i];
			for (int j = 0; j < subList.Count; j++)
			{
				Refers refers = subList[j];
				int skillId = this.Model.GetProfessionSkillFromSlot(i, j);
				bool isEmpty = skillId == -1;
				ValueTuple<ProfessionData, int> valueTuple = this.Model.FindProfessionDataBySkillId(skillId);
				ProfessionData professionData = valueTuple.Item1;
				int skillIndex = valueTuple.Item2;
				GameObject content = refers.CGet<GameObject>("Content");
				GameObject empty = refers.CGet<GameObject>("Empty");
				TooltipInvoker tip = refers.GetComponent<TooltipInvoker>();
				content.SetActive(!isEmpty);
				empty.SetActive(isEmpty);
				tip.enabled = !isEmpty;
				bool flag = !isEmpty;
				if (flag)
				{
					int professionId = professionData.TemplateId;
					ProfessionItem professionConfig = Profession.Instance[professionId];
					ProfessionSkillItem professionSkillConfig = ProfessionSkill.Instance[skillId];
					bool isUnlocked = professionData.IsSkillUnlocked(skillIndex);
					ProfessionSkillItem config = ProfessionSkill.Instance[skillId];
					GameObject unlocked = refers.CGet<GameObject>("Unlocked");
					GameObject locked = refers.CGet<GameObject>("Locked");
					CImage icon = refers.CGet<CImage>("Icon");
					GameObject coolDown = refers.CGet<GameObject>("CoolDown");
					TextMeshProUGUI label = refers.CGet<TextMeshProUGUI>("Label");
					TextMeshProUGUI cdLabel = refers.CGet<TextMeshProUGUI>("CdLabel");
					CImage hover = refers.CGet<CImage>("Hover");
					CImage bg = unlocked.GetComponent<CImage>();
					int x = (int)professionSkillConfig.TriggerType;
					int y = (int)(unlocked ? professionSkillConfig.Level : 0);
					bg.SetSprite(string.Format("bottom_profession_tubiao_{0}_{1}", x, y), false, null);
					icon.SetSprite(config.Icon, false, null);
					locked.SetActive(!isUnlocked);
					unlocked.SetActive(isUnlocked);
					label.text = config.Name;
					bool isCoolDown = professionData.IsSkillCooldown(SingletonObject.getInstance<BasicGameData>().CurrDate, skillIndex);
					coolDown.SetActive(isCoolDown);
					int coolDownTime = professionData.SkillOffCooldownDates[i] - SingletonObject.getInstance<BasicGameData>().CurrDate;
					cdLabel.text = Mathf.Max(0, coolDownTime).ToString();
					bool isActive = config.TriggerType == EProfessionSkillTriggerType.Active;
					bool flag2 = skillIndex == 3 && professionConfig.ExtraProfessionSkill > 0;
					this._extraSkillBg.SetActive(true);
					refers.CGet<GameObject>("Numbers").SetActive(false);
					TeaTasterSkillsData skillsData;
					bool flag3;
					if (skillId == 66)
					{
						skillsData = (professionData.SkillsData as TeaTasterSkillsData);
						flag3 = (skillsData != null);
					}
					else
					{
						flag3 = false;
					}
					bool flag4 = flag3;
					if (flag4)
					{
						this.RefreshNumbers(refers, skillsData.ActionPointGained / 10, GlobalConfig.Instance.ProfessionSkillRecoverActionPointLimit / 10);
					}
					CButtonObsolete button = refers.GetComponent<CButtonObsolete>();
					this.RefreshButton(button, professionId, skillIndex, professionConfig, professionData, refers, icon, isCoolDown, isActive);
					ProfessionBottomMenu.RefreshPointerTrigger(refers, isActive, hover, button);
					this.RefreshTips(tip, isUnlocked, skillId, professionData, professionId, skillIndex);
				}
				else
				{
					this._extraSkillBg.SetActive(false);
					PointerTrigger pointerTrigger = refers.GetComponent<PointerTrigger>();
					pointerTrigger.enabled = false;
					CButtonObsolete button2 = refers.GetComponent<CButtonObsolete>();
					button2.onClick.RemoveAllListeners();
				}
			}
		}
	}

	// Token: 0x06002C26 RID: 11302 RVA: 0x00159773 File Offset: 0x00157973
	public static void ExtraProfessionSkillSelectFeature()
	{
		CharacterDomainMethod.AsyncCall.GetCharacterDisplayData(null, SingletonObject.getInstance<BasicGameData>().TaiwuCharId, delegate(int offset, RawDataPool dataPool)
		{
			CharacterDisplayData data = null;
			Serializer.Deserialize(dataPool, offset, ref data);
			string taiwuName = NameCenter.GetNameByDisplayData(data, true, false);
			string content = LocalStringManager.GetFormat(LanguageKey.Event_ExtraProfessionSkill_SelectFeature, taiwuName);
			List<short> featureList = new List<short>
			{
				252,
				253,
				254,
				255,
				256,
				257,
				258,
				259,
				260,
				261
			};
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.SetObject("featureList", featureList);
			argBox.Set("texPath", "Profession/tex_profession_activate");
			argBox.Set("desc", content);
			argBox.SetObject("callBack", new Action<short>(ProfessionBottomMenu.ExecuteExtraProfessionSkillSetFeature));
			UIElement.EventStyleFeatureSelect.SetOnInitArgs(argBox);
			UIManager.Instance.MaskUI(UIElement.EventStyleFeatureSelect);
		});
	}

	// Token: 0x06002C27 RID: 11303 RVA: 0x001597A8 File Offset: 0x001579A8
	private static void ExecuteExtraProfessionSkillSetFeature(short id)
	{
		bool flag = id < 0;
		if (!flag)
		{
			DialogCmd cmd = new DialogCmd
			{
				Title = LocalStringManager.Get(LanguageKey.LK_ProfessionSkill_Confirm_Title),
				Content = string.Concat(new string[]
				{
					LocalStringManager.GetFormat(LanguageKey.LK_ProfessionSkill_Confirm_NoCost, LocalStringManager.GetFormat(LanguageKey.LK_ExtraProfessionSkill_Title, Array.Empty<object>())),
					"\n",
					LocalStringManager.Get(LanguageKey.LK_ProfessionSkill_Confirm_CoolDown),
					" <SpName=mousetip_shijie> 1",
					LocalStringManager.Get(LanguageKey.LK_ProfessionSkill_Confirm_Confirm)
				}),
				Type = 1,
				Yes = delegate()
				{
					SingletonObject.getInstance<ProfessionModel>().SetLastTimeCastExtraSkill();
					TaiwuEventDomainMethod.Call.CloseUI("UI_ExtraProfessionSkillConfirmed", false, (int)id);
				},
				No = null
			};
			UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
			UIManager.Instance.MaskUI(UIElement.Dialog);
		}
	}

	// Token: 0x06002C28 RID: 11304 RVA: 0x00159890 File Offset: 0x00157A90
	private void RefreshExtraSkill()
	{
		bool flag = !this.Model.IsExtraProfessionSkillUnlocked;
		if (flag)
		{
			this._extraSkill.SetActive(false);
		}
		else
		{
			Refers refers = this._extraSkill.GetComponent<Refers>();
			CButtonObsolete button = this._extraSkill.GetComponent<CButtonObsolete>();
			bool isReady = this.Model.IsExtraProfessionSkillReady();
			TooltipInvoker tips = refers.GetComponent<TooltipInvoker>();
			button.ClearAndAddListener(new Action(ProfessionBottomMenu.ExtraProfessionSkillSelectFeature));
			button.interactable = isReady;
			refers.CGet<GameObject>("CoolDown").SetActive(!isReady);
			ProfessionBottomMenu.RefreshPointerTrigger(refers, true, refers.CGet<CImage>("Hover"), button);
			this._extraSkill.SetActive(true);
			tips.Type = TipType.ExtraProfessionSkill;
			tips.RuntimeParam = EasyPool.Get<ArgumentBox>();
		}
	}

	// Token: 0x06002C29 RID: 11305 RVA: 0x0015995C File Offset: 0x00157B5C
	private void RefreshNumbers(Refers refers, int valueLeft, int valueRight)
	{
		bool flag = valueLeft >= 10;
		if (flag)
		{
			refers.CGet<GameObject>("NumLeft1").GetComponent<CImage>().SetSprite(string.Format("sp_number_0_{0}", valueLeft % 10), false, null);
			refers.CGet<GameObject>("NumLeft2").GetComponent<CImage>().SetSprite(string.Format("sp_number_0_{0}", valueLeft / 10), false, null);
			refers.CGet<GameObject>("NumLeft2").SetActive(true);
		}
		else
		{
			refers.CGet<GameObject>("NumLeft1").GetComponent<CImage>().SetSprite(string.Format("sp_number_0_{0}", valueLeft), false, null);
			refers.CGet<GameObject>("NumLeft2").SetActive(false);
		}
		bool flag2 = valueRight >= 10;
		if (flag2)
		{
			refers.CGet<GameObject>("NumRight2").GetComponent<CImage>().SetSprite(string.Format("sp_number_0_{0}", valueRight % 10), false, null);
			refers.CGet<GameObject>("NumRight1").GetComponent<CImage>().SetSprite(string.Format("sp_number_0_{0}", valueRight / 10), false, null);
			refers.CGet<GameObject>("NumRight2").SetActive(true);
		}
		else
		{
			refers.CGet<GameObject>("NumRight1").GetComponent<CImage>().SetSprite(string.Format("sp_number_0_{0}", valueRight), false, null);
			refers.CGet<GameObject>("NumRight2").SetActive(false);
		}
		refers.CGet<GameObject>("Numbers").SetActive(true);
	}

	// Token: 0x06002C2A RID: 11306 RVA: 0x00159AE4 File Offset: 0x00157CE4
	private void RefreshTips(TooltipInvoker tip, bool isUnlocked, int skillId, ProfessionData professionData, int professionId, int skillIndex)
	{
		bool flag = !isUnlocked;
		if (flag)
		{
			bool flag2 = tip.RuntimeParam != null;
			if (flag2)
			{
				EasyPool.Free<ArgumentBox>(tip.RuntimeParam);
				tip.RuntimeParam = null;
			}
			tip.Type = TipType.SingleDesc;
			bool flag3 = tip.PresetParam == null || tip.PresetParam.Length == 0;
			if (flag3)
			{
				tip.PresetParam = new string[1];
			}
			tip.PresetParam[0] = LocalStringManager.Get(LanguageKey.LK_ProfessionSkill_Seniority_NotMeet);
		}
		else
		{
			tip.Type = TipType.ProfessionSkill;
			ArgumentBox tipRuntimeParam = EasyPool.Get<ArgumentBox>();
			tipRuntimeParam.Set("ProfessionSkillId", skillId);
			tipRuntimeParam.SetObject("ProfessionData", professionData);
			tipRuntimeParam.Set("ProfessionId", professionId);
			tipRuntimeParam.Set("SkillIndex", skillIndex);
			tipRuntimeParam.Set("Exp", this._exp);
			tipRuntimeParam.SetObject("Resources", this._resources);
			tip.RuntimeParam = tipRuntimeParam;
		}
	}

	// Token: 0x06002C2B RID: 11307 RVA: 0x00159BD7 File Offset: 0x00157DD7
	public void UpdateExp(int exp)
	{
		this._exp = exp;
	}

	// Token: 0x06002C2C RID: 11308 RVA: 0x00159BE1 File Offset: 0x00157DE1
	public void UpdateResources(ResourceInts resourceInts)
	{
		this._resources = resourceInts;
	}

	// Token: 0x06002C2D RID: 11309 RVA: 0x00159BEC File Offset: 0x00157DEC
	private static void RefreshPointerTrigger(Refers refers, bool isActive, CImage hover, CButtonObsolete button)
	{
		PointerTrigger pointerTrigger = refers.GetComponent<PointerTrigger>();
		pointerTrigger.enabled = isActive;
		if (isActive)
		{
			bool flag = pointerTrigger.EnterEvent.GetPersistentEventCount() == 1;
			if (flag)
			{
				pointerTrigger.EnterEvent.AddListener(delegate()
				{
					hover.gameObject.SetActive(button.interactable);
				});
			}
			bool flag2 = pointerTrigger.ExitEvent.GetPersistentEventCount() == 1;
			if (flag2)
			{
				pointerTrigger.ExitEvent.AddListener(delegate()
				{
					hover.gameObject.SetActive(false);
				});
			}
		}
	}

	// Token: 0x06002C2E RID: 11310 RVA: 0x00159C80 File Offset: 0x00157E80
	private void RefreshButton(CButtonObsolete button, int professionId, int skillIndex, ProfessionItem professionConfig, ProfessionData professionData, Refers refers, CImage icon, bool isCoolDown, bool isActive)
	{
		ProfessionSkillItem skillCfg = professionData.GetSkillConfig(skillIndex);
		bool flag = skillCfg.TemplateId == 3;
		if (flag)
		{
			button.ClearAndAddListener(delegate
			{
				ProfessionSkillArg professionSkillArg = new ProfessionSkillArg
				{
					ProfessionId = professionId,
					SkillId = skillCfg.TemplateId,
					IsSuccess = true
				};
				ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>();
				argumentBox.Clear();
				argumentBox.SetObject("ProfessionSkillArg", professionSkillArg);
				argumentBox.Set("PreConirmTimeCost", GlobalConfig.Instance.SavageSkill3_OpenItemSelectTimeCost);
				UIElement.ProfessionSkillPreConfirm.SetOnInitArgs(argumentBox);
				UIManager.Instance.ShowUI(UIElement.ProfessionSkillPreConfirm, true);
			});
		}
		else
		{
			button.ClearAndAddListener(delegate
			{
				ExtraDomainMethod.Call.ExecuteActiveProfessionSkill(professionId, skillIndex);
			});
		}
		bool flag2 = skillIndex == 3 && professionConfig.ExtraProfessionSkill > 0;
		bool canUse = UIManager.Instance.IsFocusElement(UIElement.StateMainWorld);
		ProfessionSkillStateHelper.OnGetResult <>9__3;
		ExtraDomainMethod.AsyncCall.CanExecuteProfessionSkill(this._handler, professionId, skillIndex, delegate(int offset, RawDataPool dataPool)
		{
			bool canExecute = false;
			Serializer.Deserialize(dataPool, offset, ref canExecute);
			button.interactable = (canExecute & canUse);
			IAsyncMethodRequestHandler handler = this._handler;
			int templateId = skillCfg.TemplateId;
			ResourceInts resources = this._resources;
			int exp = this._exp;
			ProfessionSkillStateHelper.OnGetResult onGetResult;
			if ((onGetResult = <>9__3) == null)
			{
				onGetResult = (<>9__3 = delegate(ProfessionSkillStateHelper.Result result)
				{
					bool needGray = isCoolDown || (isActive && !button.interactable) || (skillCfg.Type != EProfessionSkillType.Passive && !result.CanUse);
					icon.SetColor(needGray ? Color.gray : Color.white);
				});
			}
			ProfessionSkillStateHelper.AsyncGetSkillUseState(handler, templateId, resources, exp, onGetResult);
		});
	}

	// Token: 0x06002C2F RID: 11311 RVA: 0x00159D74 File Offset: 0x00157F74
	private void InitSlotRefers()
	{
		this._equipSkillItems = new List<List<Refers>>();
		int levelCount = 4;
		Refers[] allChildRefers = this._skillSlots.GetComponentsInChildren<Refers>();
		int i;
		int num;
		for (i = 0; i < levelCount; i = num + 1)
		{
			List<Refers> subList = new List<Refers>();
			int slotCount = levelCount - i;
			int j;
			for (j = 0; j < slotCount; j = num + 1)
			{
				Refers item = allChildRefers.Find((Refers r) => r.gameObject.name == string.Format("Slot_{0}_{1}", i, j));
				subList.Add(item);
				num = j;
			}
			this._equipSkillItems.Add(subList);
			num = i;
		}
	}

	// Token: 0x06002C30 RID: 11312 RVA: 0x00159E4E File Offset: 0x0015804E
	private void InitRefers()
	{
		this._skillSlots = base.CGet<RectTransform>("SkillSlots");
		this._extraSkillBg = base.CGet<GameObject>("ExtraSkillBg");
		this._extraSkill = base.CGet<GameObject>("ExtraSkill");
	}

	// Token: 0x04001FF6 RID: 8182
	private List<List<Refers>> _equipSkillItems;

	// Token: 0x04001FF7 RID: 8183
	private bool _inited = false;

	// Token: 0x04001FF8 RID: 8184
	private IAsyncMethodRequestHandler _handler;

	// Token: 0x04001FF9 RID: 8185
	private ResourceInts _resources;

	// Token: 0x04001FFA RID: 8186
	private int _exp;

	// Token: 0x04001FFB RID: 8187
	private RectTransform _skillSlots;

	// Token: 0x04001FFC RID: 8188
	private GameObject _extraSkillBg;

	// Token: 0x04001FFD RID: 8189
	private GameObject _extraSkill;
}
