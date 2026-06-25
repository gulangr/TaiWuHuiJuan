using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using Game.Components.Common;
using GameData.Common;
using GameData.Domains.Character;
using GameData.GameDataBridge;
using GameData.Serializer;
using UnityEngine;

namespace Game.Views.MouseTips
{
	// Token: 0x0200086F RID: 2159
	public class MouseTipMixPoisonEffectOutCombat : MouseTipBase
	{
		// Token: 0x17000C77 RID: 3191
		// (get) Token: 0x06006815 RID: 26645 RVA: 0x002F95A8 File Offset: 0x002F77A8
		protected override bool CanStick
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06006816 RID: 26646 RVA: 0x002F95AB File Offset: 0x002F77AB
		protected override void Init(ArgumentBox argsBox)
		{
			argsBox.Get("CharacterId", out this._characterId);
			argsBox.Get<int[]>("MarkCount", out this._markCount);
			this.encyclopediaHotKey.Refresh(EHotKeyDisplayType.ViewEncyclopedia);
			this.InitMixPoisonOutCombatInfo();
		}

		// Token: 0x06006817 RID: 26647 RVA: 0x002F95E8 File Offset: 0x002F77E8
		private void InitMixPoisonOutCombatInfo()
		{
			foreach (MixPoisonDetailInfo mixPoisonDetailInfo in this.mixPoisonDetailInfos)
			{
				mixPoisonDetailInfo.gameObject.SetActive(false);
			}
			int index = 0;
			short templateId = 0;
			while ((int)templateId < this._markCount.Length)
			{
				bool flag = this._markCount[(int)templateId] > 0;
				if (flag)
				{
					MixPoisonDetailInfo detailInfo = this.mixPoisonDetailInfos[index++];
					detailInfo.gameObject.SetActive(true);
					MixPoisonEffectItem mixPoisonEffectItem = MixPoisonEffect.Instance[(int)templateId];
					MedicineItem medicineItem = Medicine.Instance[mixPoisonEffectItem.MedicineId];
					for (int i = 0; i < mixPoisonEffectItem.HasPoisonTypes.Length; i++)
					{
						detailInfo.poisonImages[i].SetSprite("ui9_icon_poison_0_" + mixPoisonEffectItem.HasPoisonTypes[i].ToString(), true, null);
					}
					detailInfo.mixPoisonName.SetText(medicineItem.Name.SetColor("darkpurple"), true);
					detailInfo.desc.SetText(mixPoisonEffectItem.ShortDesc.ColorReplace(), true);
				}
				templateId += 1;
			}
		}

		// Token: 0x06006818 RID: 26648 RVA: 0x002F9723 File Offset: 0x002F7923
		public override void InitMonitorFieldIds()
		{
			this.MonitorFields.Add(new UIBase.MonitorDataField(4, 0, (ulong)((long)this._characterId), new uint[]
			{
				44U
			}));
		}

		// Token: 0x06006819 RID: 26649 RVA: 0x002F974C File Offset: 0x002F794C
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
					bool flag = uid.DomainId == 4 && uid.DataId == 0 && (int)uid.SubId0 == this._characterId && uid.SubId1 == 44U;
					if (flag)
					{
						PoisonInts poisons = default(PoisonInts);
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref poisons);
						this.poisonInfo.Refresh(poisons);
					}
				}
			}
		}

		// Token: 0x0600681A RID: 26650 RVA: 0x002F9820 File Offset: 0x002F7A20
		private void Update()
		{
			this.CheckHotkeyDisplayViewEncyclopedia();
		}

		// Token: 0x0600681B RID: 26651 RVA: 0x002F982C File Offset: 0x002F7A2C
		private void CheckHotkeyDisplayViewEncyclopedia()
		{
			bool flag = this.Element != null && CommonCommandKit.PrimaryInteraction.Check(this.Element, true, true, false, true, false);
			if (flag)
			{
				ArgumentBox args = EasyPool.Get<ArgumentBox>().Set("key", EncyclopediaTipLink.DefValue.MixPoison.RefName);
				UIElement.Encyclopedia.SetOnInitArgs(args);
				UIManager.Instance.ShowUI(UIElement.Encyclopedia, true);
			}
		}

		// Token: 0x040049A6 RID: 18854
		[SerializeField]
		private PoisonInfo poisonInfo;

		// Token: 0x040049A7 RID: 18855
		[SerializeField]
		private MixPoisonDetailInfo[] mixPoisonDetailInfos;

		// Token: 0x040049A8 RID: 18856
		[SerializeField]
		protected Game.Components.Common.HotkeyDisplay encyclopediaHotKey;

		// Token: 0x040049A9 RID: 18857
		private int _characterId;

		// Token: 0x040049AA RID: 18858
		private int[] _markCount;
	}
}
