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
	// Token: 0x0200086E RID: 2158
	public class MouseTipMixPoisonEffectDetailed : MouseTipBase
	{
		// Token: 0x17000C76 RID: 3190
		// (get) Token: 0x0600680D RID: 26637 RVA: 0x002F9319 File Offset: 0x002F7519
		protected override bool CanStick
		{
			get
			{
				return false;
			}
		}

		// Token: 0x0600680E RID: 26638 RVA: 0x002F931C File Offset: 0x002F751C
		protected override void Init(ArgumentBox argsBox)
		{
			argsBox.Get("CharacterId", out this._characterId);
			this.detailHotKey.Refresh(EHotKeyDisplayType.CancelDetail);
			this.encyclopediaHotKey.Refresh(EHotKeyDisplayType.ViewEncyclopedia);
			this.InitMixPoisonInfo();
		}

		// Token: 0x0600680F RID: 26639 RVA: 0x002F9354 File Offset: 0x002F7554
		private void InitMixPoisonInfo()
		{
			short templateId = 0;
			while ((int)templateId < this.mixPoisonDetailInfos.Length)
			{
				MixPoisonEffectItem mixPoisonEffectItem = MixPoisonEffect.Instance[(int)templateId];
				MixPoisonDetailInfo detailInfo = this.mixPoisonDetailInfos[(int)templateId];
				for (int i = 0; i < mixPoisonEffectItem.HasPoisonTypes.Length; i++)
				{
					detailInfo.poisonImages[i].SetSprite("ui9_icon_poison_0_" + mixPoisonEffectItem.HasPoisonTypes[i].ToString(), true, null);
				}
				MedicineItem medicineItem = Medicine.Instance[mixPoisonEffectItem.MedicineId];
				detailInfo.mixPoisonName.SetText(medicineItem.Name.SetColor("darkpurple"), true);
				detailInfo.desc.SetText(mixPoisonEffectItem.ShortDesc.ColorReplace(), true);
				templateId += 1;
			}
		}

		// Token: 0x06006810 RID: 26640 RVA: 0x002F942B File Offset: 0x002F762B
		public override void InitMonitorFieldIds()
		{
			this.MonitorFields.Add(new UIBase.MonitorDataField(4, 0, (ulong)((long)this._characterId), new uint[]
			{
				44U
			}));
		}

		// Token: 0x06006811 RID: 26641 RVA: 0x002F9454 File Offset: 0x002F7654
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

		// Token: 0x06006812 RID: 26642 RVA: 0x002F9528 File Offset: 0x002F7728
		private void Update()
		{
			this.CheckHotkeyDisplayViewEncyclopedia();
		}

		// Token: 0x06006813 RID: 26643 RVA: 0x002F9534 File Offset: 0x002F7734
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

		// Token: 0x040049A1 RID: 18849
		[SerializeField]
		private PoisonInfo poisonInfo;

		// Token: 0x040049A2 RID: 18850
		[SerializeField]
		private MixPoisonDetailInfo[] mixPoisonDetailInfos;

		// Token: 0x040049A3 RID: 18851
		[SerializeField]
		protected Game.Components.Common.HotkeyDisplay detailHotKey;

		// Token: 0x040049A4 RID: 18852
		[SerializeField]
		protected Game.Components.Common.HotkeyDisplay encyclopediaHotKey;

		// Token: 0x040049A5 RID: 18853
		private int _characterId;
	}
}
