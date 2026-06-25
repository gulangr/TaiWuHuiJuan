using System;
using CharacterDataMonitor;
using FrameWork;
using GameData.Domains.Building;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

// Token: 0x0200026E RID: 622
public class MouseTipAdvance : MouseTipBase
{
	// Token: 0x1700047C RID: 1148
	// (get) Token: 0x060028F6 RID: 10486 RVA: 0x0012FEEB File Offset: 0x0012E0EB
	protected override bool CanStick
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060028F7 RID: 10487 RVA: 0x0012FEF0 File Offset: 0x0012E0F0
	protected override void Init(ArgumentBox argsBox)
	{
		int taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
		this._disorderOfQiMonitor = SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<DisorderOfQiMonitor>(taiwuCharId, false);
		this._disorderOfQiMonitor.AddChangeOfMainAttributeListener(new Action(this.RefreshProperty));
		this._disorderOfQiMonitor.AddChangeOfQiDisorderListener(new Action(this.RefreshQi));
		this._disorderOfQiMonitor.Refresh();
		base.CGet<GameObject>("SolarTermsEffect").SetActive(false);
		this.RefreshMovePoint();
	}

	// Token: 0x060028F8 RID: 10488 RVA: 0x0012FF70 File Offset: 0x0012E170
	private void RefreshProperty()
	{
		RectTransform addPropertyHolder = base.CGet<RectTransform>("AddPropertyHolder");
		TipsAddProperty addProperty = base.CGet<TipsAddProperty>("AddProperty");
		int index = 0;
		for (int i = 0; i < 6; i++)
		{
			index += MouseTip_Util.AppendAddProperty(addPropertyHolder, addProperty, index, (short)(0 + i), (int)this._disorderOfQiMonitor.MainAttribute[i], true, false, false, true, false, false);
		}
	}

	// Token: 0x060028F9 RID: 10489 RVA: 0x0012FFCC File Offset: 0x0012E1CC
	private void RefreshQi()
	{
		int qiDisorder = (int)(this._disorderOfQiMonitor.ChangeOfQiDisorder / 10);
		base.CGet<GameObject>("QiDisorder").SetActive(qiDisorder != 0);
		base.CGet<GameObject>("QiDisorder").GetComponent<MonoJoint>().JointSync();
		base.CGet<TextMeshProUGUI>("AddQiDisorder").text = ((qiDisorder >= 0) ? string.Format("+{0}", qiDisorder) : "");
		base.CGet<TextMeshProUGUI>("ReduceQiDisorder").text = ((qiDisorder >= 0) ? "" : qiDisorder.ToString());
	}

	// Token: 0x060028FA RID: 10490 RVA: 0x00130063 File Offset: 0x0012E263
	private void RefreshMovePoint()
	{
		BuildingDomainMethod.AsyncCall.GetTaiwuLocationResourceBlockEffect(this, EBuildingScaleEffect.ActionPointRegenBonus, delegate(int offset, RawDataPool dataPool)
		{
			int actionPointRegenBonus = 0;
			Serializer.Deserialize(dataPool, offset, ref actionPointRegenBonus);
			string actionTimeBaseStr = LocalStringManager.GetFormat(LanguageKey.UI_AdvanceMonth_ActionPointRecoveryBase, (TimeManager.ActionPointRecovery / 10).ToString()).ColorReplace();
			TextMeshProUGUI actionTimeBase = base.CGet<TextMeshProUGUI>("ActionTimeBase");
			actionTimeBase.SetText(actionTimeBaseStr, true);
			actionTimeBase.GetComponent<TMPTextSpriteHelper>().Parse();
			int extra = actionPointRegenBonus;
			int sum = TimeManager.ActionPointRecovery + extra;
			string sumStr = UI_BuildingManage.GetBuildingScaleFormatString(EBuildingScaleType.MovePoint, sum);
			TextMeshProUGUI actionTimeSum = base.CGet<TextMeshProUGUI>("ActionTimeSum");
			actionTimeSum.SetText(LocalStringManager.GetFormat(LanguageKey.UI_AdvanceMonth_ActionPointRecoverySum, sumStr).ColorReplace(), true);
			actionTimeSum.GetComponent<TMPTextSpriteHelper>().Parse();
			TextMeshProUGUI actionTimeExtra = base.CGet<TextMeshProUGUI>("ActionTimeExtra");
			actionTimeExtra.rectTransform.parent.parent.gameObject.SetActive(extra > 0);
			bool flag = extra > 0;
			if (flag)
			{
				string extraStr = UI_BuildingManage.GetBuildingScaleFormatString(EBuildingScaleType.MovePoint, extra);
				actionTimeExtra.SetText(LocalStringManager.GetFormat(LanguageKey.UI_AdvanceMonth_ActionPointRecoveryExtra, extraStr).ColorReplace(), true);
				actionTimeExtra.GetComponent<TMPTextSpriteHelper>().Parse();
			}
		});
	}

	// Token: 0x060028FB RID: 10491 RVA: 0x0013007C File Offset: 0x0012E27C
	protected override void OnDisable()
	{
		base.OnDisable();
		this._disorderOfQiMonitor.RemoveChangeOfMainAttributeListener(new Action(this.RefreshProperty));
		this._disorderOfQiMonitor.RemoveChangeOfQiDisorderListener(new Action(this.RefreshQi));
		base.CGet<TextMeshProUGUI>("ActionTimeExtra").rectTransform.parent.parent.gameObject.SetActive(false);
	}

	// Token: 0x04001DE7 RID: 7655
	private DisorderOfQiMonitor _disorderOfQiMonitor;
}
