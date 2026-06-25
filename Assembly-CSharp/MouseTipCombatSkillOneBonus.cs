using System;
using System.Collections.Generic;
using FrameWork;
using Game.Views.CharacterMenu;
using GameData.Common;
using GameData.Domains.Character;
using GameData.Domains.Taiwu;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

// Token: 0x02000287 RID: 647
public class MouseTipCombatSkillOneBonus : MouseTipBase
{
	// Token: 0x060029AE RID: 10670 RVA: 0x0013C0E8 File Offset: 0x0013A2E8
	public override void InitMonitorFieldIds()
	{
		bool flag = this._charId < 0;
		if (!flag)
		{
			this.MonitorFields.Add(new UIBase.MonitorDataField(4, 0, (ulong)((long)this._charId), new uint[]
			{
				97U
			}));
		}
	}

	// Token: 0x060029AF RID: 10671 RVA: 0x0013C12C File Offset: 0x0013A32C
	public override void OnNotifyGameData(List<NotificationWrapper> notifications)
	{
		foreach (NotificationWrapper wrapper in notifications)
		{
			Notification notification = wrapper.Notification;
			byte type = notification.Type;
			byte b = type;
			if (b != 0)
			{
				if (b != 1)
				{
				}
			}
			else
			{
				this.HandData(notification.Uid, notification.ValueOffset, wrapper.DataPool);
			}
		}
	}

	// Token: 0x060029B0 RID: 10672 RVA: 0x0013C1B4 File Offset: 0x0013A3B4
	private void HandData(DataUid uid, int valueOffset, RawDataPool dataPool)
	{
		bool flag = uid.DomainId == 4;
		if (flag)
		{
			bool flag2 = uid.DataId == 0 && uid.SubId1 == 97U;
			if (flag2)
			{
				Serializer.Deserialize(dataPool, valueOffset, ref this._lifeSkillAttainments);
				this.RefreshInternal();
			}
		}
	}

	// Token: 0x060029B1 RID: 10673 RVA: 0x0013C201 File Offset: 0x0013A401
	protected override void Init(ArgumentBox argsBox)
	{
		this.NeedDataListenerId = true;
		this.ReadArgs(argsBox);
		this.Element.OnListenerIdReady = new Action(this.OnListenerIdReady);
	}

	// Token: 0x060029B2 RID: 10674 RVA: 0x0013C22A File Offset: 0x0013A42A
	private void OnListenerIdReady()
	{
		this.RefreshInternal();
	}

	// Token: 0x060029B3 RID: 10675 RVA: 0x0013C234 File Offset: 0x0013A434
	public override void Refresh(ArgumentBox argsBox)
	{
		this.ReadArgs(argsBox);
		this.RefreshInternal();
	}

	// Token: 0x060029B4 RID: 10676 RVA: 0x0013C246 File Offset: 0x0013A446
	private void RefreshInternal()
	{
		SkillBreakPlateUtils.AsyncGetBonusName(this, this._bonusData, delegate(string bonusName)
		{
			this.bonusNameLabel.text = bonusName.SetGradeColor((int)this._bonusData.Grade);
		});
		this.RefreshBonusEffects();
	}

	// Token: 0x060029B5 RID: 10677 RVA: 0x0013C26C File Offset: 0x0013A46C
	private void RefreshBonusEffects()
	{
		List<SkillBreakBonusEffectDisplay> bonusEffectList = EasyPool.Get<List<SkillBreakBonusEffectDisplay>>();
		SkillBreakBonusEffectHelper.GenerateBonusEffectDisplays(this._skillId, this._bonusData, this._lifeSkillAttainments, bonusEffectList);
		CommonUtils.PrepareEnoughChildren(this.bonusEffectLayoutRoot, this.bonusEffectItemTemplate.gameObject, bonusEffectList.Count, new CommonUtils.PrepareExtraItemInfo?(new CommonUtils.PrepareExtraItemInfo
		{
			ExtraItemCount = this.extraItemInEffectLayoutRoot,
			TemplateOrder = CommonUtils.EPrepareTemplateOrder.AfterExtraItems
		}));
		for (int i = 0; i < bonusEffectList.Count; i++)
		{
			SkillBreakBonusEffect item = this.bonusEffectLayoutRoot.GetChild(i + this.extraItemInEffectLayoutRoot).GetComponent<SkillBreakBonusEffect>();
			item.Refresh(bonusEffectList[i], SkillBreakBonusEffect.EBonusIconSize.Small);
		}
		EasyPool.Free<List<SkillBreakBonusEffectDisplay>>(bonusEffectList);
	}

	// Token: 0x060029B6 RID: 10678 RVA: 0x0013C321 File Offset: 0x0013A521
	private void ReadArgs(ArgumentBox argsBox)
	{
		argsBox.Get("SkillId", out this._skillId);
		argsBox.Get("CharId", out this._charId);
		argsBox.Get<SkillBreakPlateBonus>("BonusData", out this._bonusData);
	}

	// Token: 0x04001E4A RID: 7754
	private LifeSkillShorts _lifeSkillAttainments = default(LifeSkillShorts);

	// Token: 0x04001E4B RID: 7755
	private SkillBreakPlateBonus _bonusData;

	// Token: 0x04001E4C RID: 7756
	private short _skillId;

	// Token: 0x04001E4D RID: 7757
	private int _charId;

	// Token: 0x04001E4E RID: 7758
	[SerializeField]
	private RectTransform bonusEffectLayoutRoot;

	// Token: 0x04001E4F RID: 7759
	[SerializeField]
	private SkillBreakBonusEffect bonusEffectItemTemplate;

	// Token: 0x04001E50 RID: 7760
	[SerializeField]
	private TextMeshProUGUI bonusNameLabel;

	// Token: 0x04001E51 RID: 7761
	[SerializeField]
	private int extraItemInEffectLayoutRoot = 1;
}
