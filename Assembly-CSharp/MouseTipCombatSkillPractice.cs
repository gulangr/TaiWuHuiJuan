using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using GameData.Common;
using GameData.GameDataBridge;
using GameData.Serializer;
using TMPro;
using UnityEngine;

// Token: 0x02000288 RID: 648
public class MouseTipCombatSkillPractice : MouseTipBase
{
	// Token: 0x1700048B RID: 1163
	// (get) Token: 0x060029B9 RID: 10681 RVA: 0x0013C396 File Offset: 0x0013A596
	protected override bool CanStick
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060029BA RID: 10682 RVA: 0x0013C39C File Offset: 0x0013A59C
	protected override void Init(ArgumentBox argsBox)
	{
		UIManager.Instance.HideUI(UIElement.MouseTipCombatSkill);
		this._argumentBox = (ArgumentBox)argsBox.Clone();
		argsBox.Get("CombatSkillId", out this._combatSkillId);
		argsBox.Get("CharId", out this._taiwuCharId);
		argsBox.Get("NeedExp", out this._needExp);
		argsBox.Get<List<bool>>("CannotPracticeReasons", out this._cannotPracticeReasons);
		CombatSkillItem config = CombatSkill.Instance[this._combatSkillId];
		CombatSkillTypeItem typeConfig = CombatSkillType.Instance[config.Type];
		base.CGet<TextMeshProUGUI>("Name").SetText(config.Name, true);
		base.CGet<GameObject>("Tip2Holder").SetActive(this._cannotPracticeReasons[1]);
		base.CGet<GameObject>("Tip3Holder").SetActive(this._cannotPracticeReasons[2]);
		base.CGet<GameObject>("Tip4Holder").SetActive(this._cannotPracticeReasons[3]);
		bool flag = this._cannotPracticeReasons[1];
		if (flag)
		{
			string gradeNum = LocalStringManager.Get(string.Format("LK_Num_{0}", (int)(9 - config.Grade)));
			gradeNum = (gradeNum + LocalStringManager.Get(LanguageKey.LK_Grade)).SetColor(Colors.Instance.GradeColors[(int)config.Grade]);
			base.CGet<TextMeshProUGUI>("Tip2").text = LocalStringManager.GetFormat(LanguageKey.LK_CombatSkillBreakoutOld_Tip1, gradeNum, typeConfig.Name).ColorReplace();
		}
		bool flag2 = this._cannotPracticeReasons[2];
		if (flag2)
		{
			base.CGet<TextMeshProUGUI>("Tip3").text = LocalStringManager.GetFormat(LanguageKey.LK_CombatSkillBreakoutOld_Tip2, Array.Empty<object>()).ColorReplace();
		}
		bool flag3 = this._cannotPracticeReasons[3];
		if (flag3)
		{
			base.CGet<TextMeshProUGUI>("Tip4").text = LocalStringManager.GetFormat(LanguageKey.LK_CombatSkillBreakoutOld_Tip3, Array.Empty<object>()).ColorReplace();
		}
	}

	// Token: 0x060029BB RID: 10683 RVA: 0x0013C594 File Offset: 0x0013A794
	public override void InitMonitorFieldIds()
	{
		this.MonitorFields.Add(new UIBase.MonitorDataField(4, 0, (ulong)this._taiwuCharId, new uint[]
		{
			66U
		}));
	}

	// Token: 0x060029BC RID: 10684 RVA: 0x0013C5BC File Offset: 0x0013A7BC
	public override void OnNotifyGameData(List<NotificationWrapper> notifications)
	{
		foreach (NotificationWrapper wrapper in notifications)
		{
			Notification notification = wrapper.Notification;
			byte type = notification.Type;
			byte b = type;
			if (b == 0)
			{
				DataUid uid = notification.Uid;
				bool flag = uid.DomainId == 4 && uid.DataId == 0 && (int)uid.SubId0 == this._taiwuCharId;
				if (flag)
				{
					bool flag2 = uid.SubId1 == 66U;
					if (flag2)
					{
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._expOwn);
						string expColor = this._cannotPracticeReasons[0] ? "brightred" : "brightblue";
						base.CGet<TextMeshProUGUI>("Tip1").text = LocalStringManager.GetFormat(LanguageKey.LK_ExpCost, CommonUtils.GetDisplayStringForNum(this._expOwn, 100000).SetColor(expColor), this._needExp).ColorReplace();
					}
				}
			}
		}
	}

	// Token: 0x060029BD RID: 10685 RVA: 0x0013C6EC File Offset: 0x0013A8EC
	protected override void OnDisable()
	{
		base.OnDisable();
		UIManager.Instance.HideUI(UIElement.MouseTipCombatSkill);
		this._content.gameObject.SetActive(true);
	}

	// Token: 0x060029BE RID: 10686 RVA: 0x0013C718 File Offset: 0x0013A918
	private void Awake()
	{
		this._content = base.CGet<GameObject>("Content");
	}

	// Token: 0x04001E52 RID: 7762
	private short _combatSkillId;

	// Token: 0x04001E53 RID: 7763
	private int _taiwuCharId;

	// Token: 0x04001E54 RID: 7764
	private int _expOwn;

	// Token: 0x04001E55 RID: 7765
	private int _needExp;

	// Token: 0x04001E56 RID: 7766
	private List<bool> _cannotPracticeReasons;

	// Token: 0x04001E57 RID: 7767
	private ArgumentBox _argumentBox;

	// Token: 0x04001E58 RID: 7768
	private GameObject _content;
}
