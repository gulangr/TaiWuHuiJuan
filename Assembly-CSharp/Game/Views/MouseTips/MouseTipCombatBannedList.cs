using System;
using System.Collections.Generic;
using System.Linq;
using FrameWork;
using Game.Views.MouseTips.CombatBanned;
using GameData.Common;
using GameData.Domains.Combat;
using GameData.Domains.Item;
using GameData.GameDataBridge;
using GameData.Serializer;
using UnityEngine;

namespace Game.Views.MouseTips
{
	// Token: 0x02000849 RID: 2121
	public class MouseTipCombatBannedList : MouseTipBase
	{
		// Token: 0x17000C63 RID: 3171
		// (get) Token: 0x06006702 RID: 26370 RVA: 0x002EFFCA File Offset: 0x002EE1CA
		protected override bool CanStick
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06006703 RID: 26371 RVA: 0x002EFFD0 File Offset: 0x002EE1D0
		protected override void Init(ArgumentBox argsBox)
		{
			argsBox.Get("CharId", out this._charId);
			bool isAlly;
			argsBox.Get("IsAlly", out isAlly);
			bool flag = isAlly;
			SilenceData data;
			if (flag)
			{
				argsBox.Get<SilenceData>("AllySilenceData", out data);
			}
			else
			{
				argsBox.Get<SilenceData>("EnemySilenceData", out data);
			}
			GameObject gameObject = this.skillList.gameObject;
			Dictionary<short, CountdownData> combatSkill = data.CombatSkill;
			gameObject.SetActive(combatSkill != null && combatSkill.Count > 0);
			GameObject gameObject2 = this.itemList.gameObject;
			List<ItemKey> weaponKeys = data.WeaponKeys;
			gameObject2.SetActive(weaponKeys != null && weaponKeys.Count > 0);
			int constraint = (data.CombatSkill.Count > 14) ? 4 : 2;
			this.skillList.SetConstraint(constraint);
			this.itemList.SetConstraint(constraint);
			this.skillList.Set(data.CombatSkill.Keys.ToList<short>());
			this.itemList.Set(data.WeaponKeys);
		}

		// Token: 0x06006704 RID: 26372 RVA: 0x002F00D1 File Offset: 0x002EE2D1
		public override void InitMonitorFieldIds()
		{
			this.MonitorFields.Add(new UIBase.MonitorDataField(8, 10, (ulong)this._charId, new uint[]
			{
				140U
			}));
		}

		// Token: 0x06006705 RID: 26373 RVA: 0x002F00FD File Offset: 0x002EE2FD
		protected override void OnDisable()
		{
			this.skillList.Clear();
			this.itemList.Clear();
		}

		// Token: 0x06006706 RID: 26374 RVA: 0x002F0118 File Offset: 0x002EE318
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
					bool flag = uid.DomainId == 8 && uid.DataId == 10;
					if (flag)
					{
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._currSilenceData);
						this.ProcessData();
					}
				}
			}
		}

		// Token: 0x06006707 RID: 26375 RVA: 0x002F01C4 File Offset: 0x002EE3C4
		private void ProcessData()
		{
			this.skillList.HandleData(this._currSilenceData.CombatSkill);
			this.itemList.HandleData(this._currSilenceData.WeaponFrames);
		}

		// Token: 0x040048A6 RID: 18598
		[SerializeField]
		private CombatBannedList skillList;

		// Token: 0x040048A7 RID: 18599
		[SerializeField]
		private CombatBannedList itemList;

		// Token: 0x040048A8 RID: 18600
		private int _charId;

		// Token: 0x040048A9 RID: 18601
		private SilenceData _currSilenceData = new SilenceData();
	}
}
