using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Config;
using FrameWork;
using FrameWork.UISystem.Components;
using Game.Components.Common;
using GameData.Common;
using GameData.Domains.Character;
using GameData.Domains.Combat.MixPoison;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using UnityEngine;

namespace Game.Views.MouseTips
{
	// Token: 0x02000870 RID: 2160
	public class MouseTipMixPoisonEffectSimple : MouseTipBase
	{
		// Token: 0x17000C78 RID: 3192
		// (get) Token: 0x0600681D RID: 26653 RVA: 0x002F98A0 File Offset: 0x002F7AA0
		protected override bool CanStick
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000C79 RID: 3193
		// (get) Token: 0x0600681E RID: 26654 RVA: 0x002F98A3 File Offset: 0x002F7AA3
		private CombatModel Model
		{
			get
			{
				return SingletonObject.getInstance<CombatModel>();
			}
		}

		// Token: 0x0600681F RID: 26655 RVA: 0x002F98AC File Offset: 0x002F7AAC
		protected override void Init(ArgumentBox argsBox)
		{
			argsBox.Get("CharacterId", out this._characterId);
			argsBox.Get<MixPoisonAffectedCountCollection>("MixPoisonAffectedCount", out this._mixPoisonAffectedCountCollection);
			this.detailHotKey.Refresh(EHotKeyDisplayType.Detail);
			this.encyclopediaHotKey.Refresh(EHotKeyDisplayType.ViewEncyclopedia);
			this.InitMixPoisonInfo();
		}

		// Token: 0x06006820 RID: 26656 RVA: 0x002F9904 File Offset: 0x002F7B04
		private void InitMixPoisonInfo()
		{
			this.countInfo.Clear();
			CombatSubProcessorCharacter processor = this.Model.ProcessorCharacters.GetOrDefault(this._characterId);
			MixPoisonAffectedCountCollection canAffectCollection = (processor != null) ? processor.MixPoisonCanAffectCount : null;
			sbyte i = 0;
			while ((int)i < MixPoisonEffect.Instance.Count)
			{
				int canAffectCount = (canAffectCollection != null) ? canAffectCollection.GetAffectedCount(i) : 0;
				bool flag = canAffectCount == 0;
				if (!flag)
				{
					MixPoisonEffectItem mixPoisonEffectItem = MixPoisonEffect.Instance[i];
					int affectedCount = this._mixPoisonAffectedCountCollection.GetAffectedCount(mixPoisonEffectItem.TemplateId);
					int leftCount = canAffectCount - affectedCount;
					leftCount = Mathf.Max(leftCount, 0);
					bool flag2 = canAffectCount > 0;
					if (flag2)
					{
						this.countInfo.Add(new ValueTuple<short, int>((short)i, leftCount));
					}
				}
				i += 1;
			}
			this.infoHolder.Rebuild<MixPoisonSimpleInfo>(this.countInfo.Count, delegate(MixPoisonSimpleInfo info, int index)
			{
				ValueTuple<short, int> valueTuple = this.countInfo[index];
				short templateId = valueTuple.Item1;
				int leftCount2 = valueTuple.Item2;
				MixPoisonEffectItem mixPoisonEffectItem2 = MixPoisonEffect.Instance[(int)templateId];
				for (int j = 0; j < mixPoisonEffectItem2.HasPoisonTypes.Length; j++)
				{
					info.poisonImages[j].SetSprite("ui9_icon_poison_0_" + mixPoisonEffectItem2.HasPoisonTypes[j].ToString(), true, null);
				}
				string mixPoisonName = Medicine.Instance[mixPoisonEffectItem2.MedicineId].Name;
				bool flag3 = leftCount2 > 0;
				if (flag3)
				{
					mixPoisonName = mixPoisonName.SetColor("darkpurple").ColorReplace();
				}
				info.mixPoisonName.SetText(mixPoisonName, true);
				string nameStr = string.Empty;
				for (int k = 0; k < mixPoisonEffectItem2.AffectPoisonTypes.Length; k++)
				{
					bool flag4 = k != 0;
					if (flag4)
					{
						nameStr += "/";
					}
					bool flag5 = leftCount2 > 0;
					if (flag5)
					{
						nameStr += string.Format(CommonUtils.GetPoisonNameColor(mixPoisonEffectItem2.AffectPoisonTypes[k]), CommonUtils.GetPoisonName(mixPoisonEffectItem2.AffectPoisonTypes[k])).ColorReplace();
					}
					else
					{
						nameStr += CommonUtils.GetPoisonName(mixPoisonEffectItem2.AffectPoisonTypes[k]);
					}
				}
				info.affectPoisonTypes.SetText(nameStr.ColorReplace(), true);
				info.disableStyle.SetStyleEffect(leftCount2 == 0, false);
				info.leftCount.SetText(leftCount2.ToString(), true);
				info.back.enabled = (index % 2 != 0);
			});
		}

		// Token: 0x06006821 RID: 26657 RVA: 0x002F99EF File Offset: 0x002F7BEF
		public override void InitMonitorFieldIds()
		{
			this.MonitorFields.Add(new UIBase.MonitorDataField(4, 0, (ulong)((long)this._characterId), new uint[]
			{
				44U
			}));
		}

		// Token: 0x06006822 RID: 26658 RVA: 0x002F9A18 File Offset: 0x002F7C18
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

		// Token: 0x06006823 RID: 26659 RVA: 0x002F9AEC File Offset: 0x002F7CEC
		private void Update()
		{
			this.CheckHotkeyDisplayViewEncyclopedia();
		}

		// Token: 0x06006824 RID: 26660 RVA: 0x002F9AF8 File Offset: 0x002F7CF8
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

		// Token: 0x040049AB RID: 18859
		[SerializeField]
		private PoisonInfo poisonInfo;

		// Token: 0x040049AC RID: 18860
		[SerializeField]
		private TemplatedContainerAssemblyNew infoHolder;

		// Token: 0x040049AD RID: 18861
		[SerializeField]
		protected Game.Components.Common.HotkeyDisplay detailHotKey;

		// Token: 0x040049AE RID: 18862
		[SerializeField]
		protected Game.Components.Common.HotkeyDisplay encyclopediaHotKey;

		// Token: 0x040049AF RID: 18863
		private int _characterId;

		// Token: 0x040049B0 RID: 18864
		private MixPoisonAffectedCountCollection _mixPoisonAffectedCountCollection;

		// Token: 0x040049B1 RID: 18865
		[TupleElementNames(new string[]
		{
			"templateId",
			"leftCount"
		})]
		private readonly List<ValueTuple<short, int>> countInfo = new List<ValueTuple<short, int>>();
	}
}
