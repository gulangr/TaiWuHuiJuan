using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using Game.Views.CharacterMenu;
using Game.Views.MouseTips.BreakBonus;
using GameData.Common;
using GameData.Domains.Character;
using GameData.Domains.CombatSkill;
using GameData.Domains.Taiwu;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips
{
	// Token: 0x0200084C RID: 2124
	public class MouseTipCombatSkillBonus : MouseTipBase
	{
		// Token: 0x06006723 RID: 26403 RVA: 0x002F0A4C File Offset: 0x002EEC4C
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

		// Token: 0x06006724 RID: 26404 RVA: 0x002F0A90 File Offset: 0x002EEC90
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

		// Token: 0x06006725 RID: 26405 RVA: 0x002F0B18 File Offset: 0x002EED18
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

		// Token: 0x06006726 RID: 26406 RVA: 0x002F0B65 File Offset: 0x002EED65
		protected override void Init(ArgumentBox argsBox)
		{
			this.NeedDataListenerId = true;
			this.ReadArgs(argsBox);
			this.Element.OnListenerIdReady = new Action(this.OnListenerIdReady);
		}

		// Token: 0x06006727 RID: 26407 RVA: 0x002F0B8E File Offset: 0x002EED8E
		private void OnListenerIdReady()
		{
			this.RefreshInternal();
		}

		// Token: 0x06006728 RID: 26408 RVA: 0x002F0B98 File Offset: 0x002EED98
		public override void Refresh(ArgumentBox argsBox)
		{
			this.ReadArgs(argsBox);
			this.RefreshInternal();
		}

		// Token: 0x06006729 RID: 26409 RVA: 0x002F0BAA File Offset: 0x002EEDAA
		private void RefreshInternal()
		{
			this.RefreshCombatSkillName();
			this.RefreshBonusEffects();
		}

		// Token: 0x0600672A RID: 26410 RVA: 0x002F0BBC File Offset: 0x002EEDBC
		private void RefreshCombatSkillName()
		{
			CombatSkillItem skillTemplate = CombatSkill.Instance[this._skillId];
			this.skillNameLabel.text = skillTemplate.Name;
		}

		// Token: 0x0600672B RID: 26411 RVA: 0x002F0BED File Offset: 0x002EEDED
		private void RefreshBonusEffects()
		{
			CombatSkillDomainMethod.AsyncCall.GetCombatSkillBreakBonuses(this, this._charId, this._skillId, delegate(int offset, RawDataPool pool)
			{
				List<SkillBreakPlateBonus> bonuses = null;
				Serializer.Deserialize(pool, offset, ref bonuses);
				this.HandleData(bonuses);
			});
		}

		// Token: 0x0600672C RID: 26412 RVA: 0x002F0C10 File Offset: 0x002EEE10
		private void HandleData(List<SkillBreakPlateBonus> bonuses)
		{
			List<ValueTuple<SkillBreakPlateBonus, List<SkillBreakBonusEffectDisplay>>> result = new List<ValueTuple<SkillBreakPlateBonus, List<SkillBreakBonusEffectDisplay>>>();
			if (bonuses == null)
			{
				bonuses = new List<SkillBreakPlateBonus>();
			}
			foreach (SkillBreakPlateBonus bonus in bonuses)
			{
				bool flag = bonus.Type == ESkillBreakPlateBonusType.None;
				if (!flag)
				{
					List<SkillBreakBonusEffectDisplay> bonusEffectList = EasyPool.Get<List<SkillBreakBonusEffectDisplay>>();
					SkillBreakBonusEffectHelper.GenerateBonusEffectDisplays(this._skillId, bonus, this._lifeSkillAttainments, bonusEffectList);
					result.Add(new ValueTuple<SkillBreakPlateBonus, List<SkillBreakBonusEffectDisplay>>(bonus, bonusEffectList));
				}
			}
			for (int i = 0; i < result.Count; i++)
			{
				bool flag2 = i >= this.holder.childCount;
				if (flag2)
				{
					Object.Instantiate<Transform>(this.holder.GetChild(0), this.holder);
				}
				ValueTuple<SkillBreakPlateBonus, List<SkillBreakBonusEffectDisplay>> valueTuple = result[i];
				SkillBreakPlateBonus bonus2 = valueTuple.Item1;
				List<SkillBreakBonusEffectDisplay> bonusEffectList2 = valueTuple.Item2;
				BonusEffectGroup groupItem = this.holder.GetChild(i).GetComponent<BonusEffectGroup>();
				groupItem.Set(this, bonus2, bonusEffectList2, this._luohanId);
				groupItem.gameObject.SetActive(true);
			}
			for (int j = result.Count; j < this.holder.childCount; j++)
			{
				this.holder.GetChild(j).gameObject.SetActive(false);
			}
		}

		// Token: 0x0600672D RID: 26413 RVA: 0x002F0D84 File Offset: 0x002EEF84
		private void ReadArgs(ArgumentBox argsBox)
		{
			argsBox.Get("SkillId", out this._skillId);
			argsBox.Get("CharId", out this._charId);
			bool flag = !argsBox.Get("luohanId", out this._luohanId);
			if (flag)
			{
				this._luohanId = -1;
			}
		}

		// Token: 0x040048B4 RID: 18612
		private LifeSkillShorts _lifeSkillAttainments = default(LifeSkillShorts);

		// Token: 0x040048B5 RID: 18613
		private short _skillId;

		// Token: 0x040048B6 RID: 18614
		private int _charId;

		// Token: 0x040048B7 RID: 18615
		private sbyte _luohanId;

		// Token: 0x040048B8 RID: 18616
		[SerializeField]
		private Transform holder;

		// Token: 0x040048B9 RID: 18617
		[SerializeField]
		private TextMeshProUGUI skillNameLabel;
	}
}
