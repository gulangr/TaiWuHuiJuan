using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using GameData.Domains.Character;
using GameData.GameDataBridge;
using GameData.Serializer;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000374 RID: 884
public class UI_CharacterPracticeConfirm : UIBase
{
	// Token: 0x060033C7 RID: 13255 RVA: 0x00199B00 File Offset: 0x00197D00
	public override void OnInit(ArgumentBox argsBox)
	{
		argsBox.Get("SkillTemplateId", out this._combatSkillTemplateId);
		argsBox.Get<Action>("OnConfirm", out this._onConfirm);
		argsBox.Get("CostExp", out this._expCost);
		this._canvasGroup = base.gameObject.GetComponent<CanvasGroup>();
		this._canvasGroup.alpha = 0.01f;
	}

	// Token: 0x060033C8 RID: 13256 RVA: 0x00199B66 File Offset: 0x00197D66
	public override void InitMonitorFieldIds()
	{
		this.MonitorFields.Add(new UIBase.MonitorDataField(4, 0, (ulong)((long)SingletonObject.getInstance<BasicGameData>().TaiwuCharId), new uint[]
		{
			34U,
			66U
		}));
	}

	// Token: 0x060033C9 RID: 13257 RVA: 0x00199B98 File Offset: 0x00197D98
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
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._expOwn);
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

	// Token: 0x060033CA RID: 13258 RVA: 0x00199C8C File Offset: 0x00197E8C
	private void Refresh()
	{
		this.Element.ShowAfterRefresh();
		base.CGet<PopupWindow>("PopupWindowBase").OnConfirmClick = new Action(this.OnConfirmClick);
		base.CGet<PopupWindow>("PopupWindowBase").OnCancelClick = new Action(this.OnCancelClick);
		base.CGet<GameObject>("Time").SetActive(true);
		int remainTime = SingletonObject.getInstance<TimeManager>().GetRemainingActionPointConvertToDays();
		base.CGet<GameObject>("Time").transform.Find("Current").GetComponentInChildren<TextMeshProUGUI>().text = LocalStringManager.GetFormat(LanguageKey.LK_Make_Resource_Require_Meet, remainTime.ToString().SetColor("brightblue"), this._timeCost);
		base.CGet<GameObject>("Exp").SetActive(true);
		base.CGet<GameObject>("Exp").transform.Find("Current").GetComponentInChildren<TextMeshProUGUI>().text = LocalStringManager.GetFormat(LanguageKey.LK_Make_Resource_Require_Meet, CommonUtils.GetDisplayStringForNum(this._expOwn, 100000).SetColor("brightblue"), this._expCost);
		GameObject resourceHolder = base.CGet<GameObject>("ResourceHolder");
		for (int i = 0; i < 8; i++)
		{
			Transform child = resourceHolder.transform.GetChild(i);
			child.gameObject.SetActive(false);
		}
		LayoutRebuilder.ForceRebuildLayoutImmediate(resourceHolder.GetComponent<RectTransform>());
		CombatSkillItem config = CombatSkill.Instance[this._combatSkillTemplateId];
		base.CGet<TextMeshProUGUI>("Content").text = LocalStringManager.GetFormat(LanguageKey.LK_CombatSkillBreakoutOld_TimeCost_Tip, config.Name);
		base.CGet<TextMeshProUGUI>("ConfirmTip").text = LocalStringManager.GetFormat(LanguageKey.LK_CombatSkillBreakoutOld_Confirm_Tip, Array.Empty<object>());
		SingletonObject.getInstance<YieldHelper>().DelayFrameDo(2U, delegate
		{
			this._canvasGroup.alpha = 1f;
		});
	}

	// Token: 0x060033CB RID: 13259 RVA: 0x00199E5E File Offset: 0x0019805E
	private void OnCancelClick()
	{
		UIManager.Instance.HideUI(UIElement.CharacterPracticeConfirm);
	}

	// Token: 0x060033CC RID: 13260 RVA: 0x00199E71 File Offset: 0x00198071
	private void OnConfirmClick()
	{
		Action onConfirm = this._onConfirm;
		if (onConfirm != null)
		{
			onConfirm();
		}
		UIManager.Instance.HideUI(UIElement.CharacterPracticeConfirm);
	}

	// Token: 0x060033CD RID: 13261 RVA: 0x00199E96 File Offset: 0x00198096
	public override void QuickHide()
	{
		base.QuickHide();
		this.OnCancelClick();
	}

	// Token: 0x060033CE RID: 13262 RVA: 0x00199EA8 File Offset: 0x001980A8
	private void Update()
	{
		bool flag = CommonCommandKit.Space.Check(this.Element, false, false, false, true, false);
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

	// Token: 0x040025B1 RID: 9649
	private int _expCost;

	// Token: 0x040025B2 RID: 9650
	private int _expOwn;

	// Token: 0x040025B3 RID: 9651
	private readonly int _timeCost = 10;

	// Token: 0x040025B4 RID: 9652
	private short _combatSkillTemplateId;

	// Token: 0x040025B5 RID: 9653
	private Action _onConfirm;

	// Token: 0x040025B6 RID: 9654
	private ResourceInts _resources;

	// Token: 0x040025B7 RID: 9655
	private CanvasGroup _canvasGroup;
}
