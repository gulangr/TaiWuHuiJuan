using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using GameData.Domains.Character;
using GameData.Domains.Extra;
using GameData.Domains.Item.Display;
using GameData.Domains.Taiwu.Profession;
using GameData.GameDataBridge;
using GameData.Serializer;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000395 RID: 917
public class UI_ProfessionSkillConfirm : UIBase
{
	// Token: 0x06003711 RID: 14097 RVA: 0x001BB9D0 File Offset: 0x001B9BD0
	public override void OnInit(ArgumentBox argsBox)
	{
		this._onPostConfirm = null;
		argsBox.Get<ProfessionSkillArg>("ProfessionSkillArg", out this._professionSkillArg);
		argsBox.Get<Action>("OnConfirm", out this._onConfirm);
		argsBox.Get<Action>("OnPostConfirm", out this._onPostConfirm);
		argsBox.Get("BeggarMoneyCount", out this._beggarMoneyCount);
		argsBox.Get<ItemDisplayData>("RebirthCricketItemData", out this._rebirthCricketItemData);
		this._instantlyConfirm = true;
		bool instantlyConfirm;
		bool flag = argsBox.Get("InstantlyConfirm", out instantlyConfirm);
		if (flag)
		{
			this._instantlyConfirm = instantlyConfirm;
		}
		bool flag2 = !argsBox.Get<ResourceInts>("CostResources", out this._costResources);
		if (flag2)
		{
			this._costResources.Initialize();
		}
		ProfessionData profession = SingletonObject.getInstance<ProfessionModel>().GetProfessionData(this._professionSkillArg.ProfessionId);
		this._skillIndex = profession.GetSkillIndex(this._professionSkillArg.SkillId);
		this._canvasGroup = base.gameObject.GetComponent<CanvasGroup>();
		this._canvasGroup.alpha = 0.01f;
	}

	// Token: 0x06003712 RID: 14098 RVA: 0x001BBAD1 File Offset: 0x001B9CD1
	private void Awake()
	{
		GEvent.Add(UiEvents.RealConfirmExecuteProfessionSkill, new GEvent.Callback(this.RealConfirmExecuteProfessionSkill));
	}

	// Token: 0x06003713 RID: 14099 RVA: 0x001BBAF0 File Offset: 0x001B9CF0
	private void OnDestroy()
	{
		GEvent.Remove(UiEvents.RealConfirmExecuteProfessionSkill, new GEvent.Callback(this.RealConfirmExecuteProfessionSkill));
	}

	// Token: 0x06003714 RID: 14100 RVA: 0x001BBB0F File Offset: 0x001B9D0F
	public override void InitMonitorFieldIds()
	{
		this.MonitorFields.Add(new UIBase.MonitorDataField(4, 0, (ulong)((long)SingletonObject.getInstance<BasicGameData>().TaiwuCharId), new uint[]
		{
			34U,
			66U
		}));
	}

	// Token: 0x06003715 RID: 14101 RVA: 0x001BBB40 File Offset: 0x001B9D40
	public override void OnNotifyGameData(List<NotificationWrapper> notifications)
	{
		foreach (NotificationWrapper wrapper in notifications)
		{
			Notification notification = wrapper.Notification;
			byte type = notification.Type;
			byte b = type;
			if (b == 0)
			{
				bool flag = notification.Uid.DomainId == 4 && notification.Uid.DataId == 0;
				if (flag)
				{
					uint subId = notification.Uid.SubId1;
					uint num = subId;
					if (num != 34U)
					{
						if (num == 66U)
						{
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._exp);
							this.Refresh();
						}
					}
					else
					{
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._resources);
						this.Refresh();
					}
				}
			}
		}
	}

	// Token: 0x06003716 RID: 14102 RVA: 0x001BBC3C File Offset: 0x001B9E3C
	private void Refresh()
	{
		this.Element.ShowAfterRefresh();
		ProfessionSkillItem professionSkillConfig = ProfessionSkill.Instance[this._professionSkillArg.SkillId];
		bool canExecuteSkill = true;
		bool showTime = professionSkillConfig.TimeCost > 0;
		base.CGet<GameObject>("Time").SetActive(showTime);
		bool flag = showTime;
		if (flag)
		{
			int remainTime = SingletonObject.getInstance<TimeManager>().GetRemainingActionPointConvertToDays();
			base.CGet<GameObject>("Time").transform.Find("Current").GetComponentInChildren<TextMeshProUGUI>().text = LocalStringManager.GetFormat(LanguageKey.LK_Make_Resource_Require_Meet, remainTime.ToString().SetColor((remainTime < (int)professionSkillConfig.TimeCost) ? "brightred" : "brightblue"), professionSkillConfig.TimeCost);
			bool flag2 = remainTime < (int)professionSkillConfig.TimeCost;
			if (flag2)
			{
				canExecuteSkill = false;
			}
		}
		bool showExp = professionSkillConfig.ExpCost > 0;
		base.CGet<GameObject>("Exp").SetActive(showExp);
		bool flag3 = showExp;
		if (flag3)
		{
			base.CGet<GameObject>("Exp").transform.Find("Current").GetComponentInChildren<TextMeshProUGUI>().text = LocalStringManager.GetFormat(LanguageKey.LK_Make_Resource_Require_Meet, CommonUtils.GetDisplayStringForNum(this._exp, 100000).SetColor((this._exp < professionSkillConfig.ExpCost) ? "brightred" : "brightblue"), professionSkillConfig.ExpCost.ToString());
			bool flag4 = this._exp < professionSkillConfig.ExpCost;
			if (flag4)
			{
				canExecuteSkill = false;
			}
		}
		bool showResource = false;
		GameObject resourceHolder = base.CGet<GameObject>("ResourceHolder");
		bool hsaCustom = this._costResources.GetSum() > 0;
		int i;
		int j;
		for (i = 0; i < 8; i = j + 1)
		{
			Transform child = resourceHolder.transform.GetChild(i);
			ResourceInfo resourceInfo = professionSkillConfig.ResourcesCost.Find((ResourceInfo r) => (int)r.ResourceType == i && r.ResourceCount > 0);
			int resourceCount = hsaCustom ? this._costResources.Get(i) : resourceInfo.ResourceCount;
			bool flag5 = resourceCount > 0;
			if (flag5)
			{
				showResource = true;
				child.gameObject.SetActive(true);
				child.GetComponentInChildren<TextMeshProUGUI>().text = LocalStringManager.GetFormat(LanguageKey.LK_Make_Resource_Require_Meet, CommonUtils.GetDisplayStringForNum(this._resources.Get(i), 100000).SetColor((this._resources.Get(i) < resourceCount) ? "brightred" : "brightblue"), resourceCount.ToString());
				bool flag6 = this._resources.Get(i) < resourceCount;
				if (flag6)
				{
					canExecuteSkill = false;
				}
			}
			else
			{
				child.gameObject.SetActive(false);
			}
			j = i;
		}
		bool noCost = !showTime && !showExp && !showResource;
		resourceHolder.SetActive(!noCost);
		bool activeSelf = resourceHolder.activeSelf;
		if (activeSelf)
		{
			LayoutRebuilder.ForceRebuildLayoutImmediate(resourceHolder.GetComponent<RectTransform>());
		}
		base.CGet<TextMeshProUGUI>("Content").text = LocalStringManager.GetFormat(noCost ? LanguageKey.LK_ProfessionSkill_Confirm_NoCost : LanguageKey.LK_ProfessionSkill_Confirm_Cost, professionSkillConfig.Name);
		base.CGet<TextMeshProUGUI>("CoolDown").text = professionSkillConfig.SkillCoolDown.ToString();
		SingletonObject.getInstance<YieldHelper>().DelayFrameDo(2U, delegate
		{
			LayoutRebuilder.ForceRebuildLayoutImmediate(base.transform as RectTransform);
		});
		SingletonObject.getInstance<YieldHelper>().DelayFrameDo(3U, delegate
		{
			this._canvasGroup.alpha = 1f;
		});
		base.CGet<CButtonObsolete>("Confirm").interactable = canExecuteSkill;
	}

	// Token: 0x06003717 RID: 14103 RVA: 0x001BBFD6 File Offset: 0x001BA1D6
	private void OnCancelClick()
	{
		ExtraDomainMethod.Call.ConfirmExecuteSkill(this._professionSkillArg, false);
		UIManager.Instance.HideUI(UIElement.ProfessionSkillConfirm);
		GEvent.OnEvent(UiEvents.ProfessionSkillConfirmSelectCancel, null);
	}

	// Token: 0x06003718 RID: 14104 RVA: 0x001BC008 File Offset: 0x001BA208
	private void OnConfirmClick()
	{
		Action onConfirm = this._onConfirm;
		if (onConfirm != null)
		{
			onConfirm();
		}
		UIManager.Instance.HideUI(UIElement.ProfessionSkillConfirm);
		bool flag = !this._instantlyConfirm;
		if (!flag)
		{
			this.Confirm();
		}
	}

	// Token: 0x06003719 RID: 14105 RVA: 0x001BC04E File Offset: 0x001BA24E
	private void Confirm()
	{
		ProfessionSkillController.ExecuteSkillDirect(this._professionSkillArg, this._skillIndex, this._beggarMoneyCount, this._rebirthCricketItemData, this._onPostConfirm);
	}

	// Token: 0x0600371A RID: 14106 RVA: 0x001BC074 File Offset: 0x001BA274
	public override void QuickHide()
	{
		base.QuickHide();
		this.OnCancelClick();
	}

	// Token: 0x0600371B RID: 14107 RVA: 0x001BC088 File Offset: 0x001BA288
	protected override void OnClick(Transform btn)
	{
		string name = btn.name;
		string a = name;
		if (!(a == "Confirm"))
		{
			if (a == "Cancel")
			{
				this.OnCancelClick();
			}
		}
		else
		{
			this.OnConfirmClick();
		}
	}

	// Token: 0x0600371C RID: 14108 RVA: 0x001BC0D0 File Offset: 0x001BA2D0
	private void Update()
	{
		bool flag = CommonCommandKit.Space.Check(this.Element, false, false, false, true, false) && base.CGet<CButtonObsolete>("Confirm").interactable;
		if (flag)
		{
			this.OnConfirmClick();
		}
		else
		{
			bool flag2 = CommonCommandKit.Esc.Check(this.Element, false, false, false, true, false);
			if (flag2)
			{
				this.QuickHide();
			}
		}
	}

	// Token: 0x0600371D RID: 14109 RVA: 0x001BC139 File Offset: 0x001BA339
	private void RealConfirmExecuteProfessionSkill(ArgumentBox _)
	{
		this.Confirm();
	}

	// Token: 0x040027C7 RID: 10183
	private int _skillIndex;

	// Token: 0x040027C8 RID: 10184
	private int _beggarMoneyCount;

	// Token: 0x040027C9 RID: 10185
	private ItemDisplayData _rebirthCricketItemData;

	// Token: 0x040027CA RID: 10186
	private ProfessionSkillArg _professionSkillArg;

	// Token: 0x040027CB RID: 10187
	private Action _onConfirm;

	// Token: 0x040027CC RID: 10188
	private Action _onPostConfirm;

	// Token: 0x040027CD RID: 10189
	private ResourceInts _resources;

	// Token: 0x040027CE RID: 10190
	private ResourceInts _costResources;

	// Token: 0x040027CF RID: 10191
	private int _exp;

	// Token: 0x040027D0 RID: 10192
	private bool _instantlyConfirm = true;

	// Token: 0x040027D1 RID: 10193
	private CanvasGroup _canvasGroup;
}
