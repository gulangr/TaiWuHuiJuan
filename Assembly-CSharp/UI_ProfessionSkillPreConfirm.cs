using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Character;
using GameData.Domains.Extra;
using GameData.Domains.Item.Display;
using GameData.Domains.Taiwu.Profession;
using GameData.GameDataBridge;
using GameData.Serializer;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000396 RID: 918
public class UI_ProfessionSkillPreConfirm : UIBase
{
	// Token: 0x06003721 RID: 14113 RVA: 0x001BC178 File Offset: 0x001BA378
	public override void OnInit(ArgumentBox argsBox)
	{
		this._onPostConfirm = null;
		argsBox.Get<ProfessionSkillArg>("ProfessionSkillArg", out this._professionSkillArg);
		argsBox.Get<Action>("OnConfirm", out this._onConfirm);
		argsBox.Get<Action>("OnPostConfirm", out this._onPostConfirm);
		argsBox.Get("BeggarMoneyCount", out this._beggarMoneyCount);
		argsBox.Get<ItemDisplayData>("RebirthCricketItemData", out this._rebirthCricketItemData);
		argsBox.Get("PreConirmTimeCost", out this._preConirmTimeCost);
		this._instantlyConfirm = true;
		bool instantlyConfirm;
		bool flag = argsBox.Get("InstantlyConfirm", out instantlyConfirm);
		if (flag)
		{
			this._instantlyConfirm = instantlyConfirm;
		}
		ProfessionData profession = SingletonObject.getInstance<ProfessionModel>().GetProfessionData(this._professionSkillArg.ProfessionId);
		this._skillIndex = profession.GetSkillIndex(this._professionSkillArg.SkillId);
		this._canvasGroup = base.gameObject.GetComponent<CanvasGroup>();
		this._canvasGroup.alpha = 0.01f;
	}

	// Token: 0x06003722 RID: 14114 RVA: 0x001BC267 File Offset: 0x001BA467
	private void Awake()
	{
		GEvent.Add(UiEvents.RealConfirmExecuteProfessionSkill, new GEvent.Callback(this.RealConfirmExecuteProfessionSkill));
	}

	// Token: 0x06003723 RID: 14115 RVA: 0x001BC286 File Offset: 0x001BA486
	private void OnDestroy()
	{
		GEvent.Remove(UiEvents.RealConfirmExecuteProfessionSkill, new GEvent.Callback(this.RealConfirmExecuteProfessionSkill));
	}

	// Token: 0x06003724 RID: 14116 RVA: 0x001BC2A5 File Offset: 0x001BA4A5
	public override void InitMonitorFieldIds()
	{
		this.MonitorFields.Add(new UIBase.MonitorDataField(4, 0, (ulong)((long)SingletonObject.getInstance<BasicGameData>().TaiwuCharId), new uint[]
		{
			34U,
			66U
		}));
	}

	// Token: 0x06003725 RID: 14117 RVA: 0x001BC2D8 File Offset: 0x001BA4D8
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
					}
				}
			}
		}
	}

	// Token: 0x06003726 RID: 14118 RVA: 0x001BC3CC File Offset: 0x001BA5CC
	private void Refresh()
	{
		this.Element.ShowAfterRefresh();
		ProfessionSkillItem professionSkillConfig = ProfessionSkill.Instance[this._professionSkillArg.SkillId];
		bool canExecuteSkill = true;
		short timecost = (this._preConirmTimeCost >= 0) ? this._preConirmTimeCost : professionSkillConfig.TimeCost;
		bool showTime = timecost > 0;
		base.CGet<GameObject>("Time").SetActive(showTime);
		bool flag = showTime;
		if (flag)
		{
			int remainTime = SingletonObject.getInstance<TimeManager>().GetRemainingActionPointConvertToDays();
			base.CGet<GameObject>("Time").transform.Find("Current").GetComponentInChildren<TextMeshProUGUI>().text = LocalStringManager.GetFormat(LanguageKey.LK_Make_Resource_Require_Meet, remainTime.ToString().SetColor((remainTime < (int)timecost) ? "brightred" : "brightblue"), timecost);
			bool flag2 = remainTime < (int)timecost;
			if (flag2)
			{
				canExecuteSkill = false;
			}
		}
		int expcost = (this._preConirmExpCost >= 0) ? this._preConirmExpCost : professionSkillConfig.ExpCost;
		bool showExp = expcost > 0;
		base.CGet<GameObject>("Exp").SetActive(showExp);
		bool flag3 = showExp;
		if (flag3)
		{
			base.CGet<GameObject>("Exp").transform.Find("Current").GetComponentInChildren<TextMeshProUGUI>().text = LocalStringManager.GetFormat(LanguageKey.LK_Make_Resource_Require_Meet, CommonUtils.GetDisplayStringForNum(this._exp, 100000).SetColor((this._exp < expcost) ? "brightred" : "brightblue"), expcost.ToString());
			bool flag4 = this._exp < expcost;
			if (flag4)
			{
				canExecuteSkill = false;
			}
		}
		bool showResource = false;
		GameObject resourceHolder = base.CGet<GameObject>("ResourceHolder");
		int i;
		int j;
		for (i = 0; i < 8; i = j + 1)
		{
			Transform child = resourceHolder.transform.GetChild(i);
			ResourceInfo resourceInfo = professionSkillConfig.ResourcesCost.Find((ResourceInfo r) => (int)r.ResourceType == i && r.ResourceCount > 0);
			bool flag5 = (int)resourceInfo.ResourceType == i && resourceInfo.ResourceCount > 0;
			if (flag5)
			{
				showResource = true;
				child.gameObject.SetActive(true);
				child.GetComponentInChildren<TextMeshProUGUI>().text = LocalStringManager.GetFormat(LanguageKey.LK_Make_Resource_Require_Meet, CommonUtils.GetDisplayStringForNum(this._resources.Get(i), 100000).SetColor((this._resources.Get(i) < resourceInfo.ResourceCount) ? "brightred" : "brightblue"), resourceInfo.ResourceCount.ToString());
				bool flag6 = this._resources.Get(i) < resourceInfo.ResourceCount;
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
		base.CGet<TextMeshProUGUI>("CoolDown").text = ((this._preConfirmCoolDown >= 0) ? this._preConfirmCoolDown.ToString() : professionSkillConfig.SkillCoolDown.ToString());
		SingletonObject.getInstance<YieldHelper>().DelayFrameDo(2U, delegate
		{
			LayoutRebuilder.ForceRebuildLayoutImmediate(base.transform as RectTransform);
		});
		SingletonObject.getInstance<YieldHelper>().DelayFrameDo(3U, delegate
		{
			this._canvasGroup.alpha = 1f;
		});
		base.CGet<CButton>("Confirm").interactable = canExecuteSkill;
	}

	// Token: 0x06003727 RID: 14119 RVA: 0x001BC780 File Offset: 0x001BA980
	private void OnCancelClick()
	{
		ExtraDomainMethod.Call.ConfirmExecuteSkill(this._professionSkillArg, false);
		UIManager.Instance.HideUI(this.Element);
	}

	// Token: 0x06003728 RID: 14120 RVA: 0x001BC7A4 File Offset: 0x001BA9A4
	private void OnConfirmClick()
	{
		Action onConfirm = this._onConfirm;
		if (onConfirm != null)
		{
			onConfirm();
		}
		UIManager.Instance.HideUI(this.Element);
		bool flag = !this._instantlyConfirm;
		if (!flag)
		{
			this.Confirm();
		}
	}

	// Token: 0x06003729 RID: 14121 RVA: 0x001BC7EB File Offset: 0x001BA9EB
	private void Confirm()
	{
		this.<Confirm>g__ConfirmExecuteSkill|21_0();
	}

	// Token: 0x0600372A RID: 14122 RVA: 0x001BC7F6 File Offset: 0x001BA9F6
	public override void QuickHide()
	{
		base.QuickHide();
		this.OnCancelClick();
	}

	// Token: 0x0600372B RID: 14123 RVA: 0x001BC808 File Offset: 0x001BAA08
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

	// Token: 0x0600372C RID: 14124 RVA: 0x001BC850 File Offset: 0x001BAA50
	private void Update()
	{
		bool flag = CommonCommandKit.Space.Check(this.Element, false, false, false, true, false) && base.CGet<CButton>("Confirm").interactable;
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

	// Token: 0x0600372D RID: 14125 RVA: 0x001BC8B9 File Offset: 0x001BAAB9
	private void RealConfirmExecuteProfessionSkill(ArgumentBox _)
	{
		this.Confirm();
	}

	// Token: 0x06003731 RID: 14129 RVA: 0x001BC8F8 File Offset: 0x001BAAF8
	[CompilerGenerated]
	private void <Confirm>g__ConfirmExecuteSkill|21_0()
	{
		ExtraDomainMethod.Call.ExecuteActiveProfessionSkill(this._professionSkillArg.ProfessionId, this._skillIndex);
		Action onPostConfirm = this._onPostConfirm;
		if (onPostConfirm != null)
		{
			onPostConfirm();
		}
	}

	// Token: 0x040027D2 RID: 10194
	private int _skillIndex;

	// Token: 0x040027D3 RID: 10195
	private int _beggarMoneyCount;

	// Token: 0x040027D4 RID: 10196
	private ItemDisplayData _rebirthCricketItemData;

	// Token: 0x040027D5 RID: 10197
	private ProfessionSkillArg _professionSkillArg;

	// Token: 0x040027D6 RID: 10198
	private Action _onConfirm;

	// Token: 0x040027D7 RID: 10199
	private Action _onPostConfirm;

	// Token: 0x040027D8 RID: 10200
	private ResourceInts _resources;

	// Token: 0x040027D9 RID: 10201
	private int _exp;

	// Token: 0x040027DA RID: 10202
	private bool _instantlyConfirm = true;

	// Token: 0x040027DB RID: 10203
	private CanvasGroup _canvasGroup;

	// Token: 0x040027DC RID: 10204
	private short _preConirmTimeCost;

	// Token: 0x040027DD RID: 10205
	private int _preConirmExpCost;

	// Token: 0x040027DE RID: 10206
	private short _preConfirmCoolDown;
}
