using System;
using System.Collections.Generic;
using System.Diagnostics;
using Config;
using GameData.Domains.Character;
using GameData.Domains.Character.AvatarSystem;
using GameData.GameDataBridge;
using GameData.Serializer;

namespace CharacterDataMonitor
{
	// Token: 0x020006B7 RID: 1719
	public class AvatarInfoMonitor : MonitorDataItemBase
	{
		// Token: 0x14000032 RID: 50
		// (add) Token: 0x060050A8 RID: 20648 RVA: 0x0025A5DC File Offset: 0x002587DC
		// (remove) Token: 0x060050A9 RID: 20649 RVA: 0x0025A614 File Offset: 0x00258814
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event Action OnAvatarDataChangeEvent;

		// Token: 0x14000033 RID: 51
		// (add) Token: 0x060050AA RID: 20650 RVA: 0x0025A64C File Offset: 0x0025884C
		// (remove) Token: 0x060050AB RID: 20651 RVA: 0x0025A684 File Offset: 0x00258884
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event Action OnClothDisplayIdChangeEvent;

		// Token: 0x14000034 RID: 52
		// (add) Token: 0x060050AC RID: 20652 RVA: 0x0025A6BC File Offset: 0x002588BC
		// (remove) Token: 0x060050AD RID: 20653 RVA: 0x0025A6F4 File Offset: 0x002588F4
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event Action OnHasNewGoodsChangeEvent;

		// Token: 0x14000035 RID: 53
		// (add) Token: 0x060050AE RID: 20654 RVA: 0x0025A72C File Offset: 0x0025892C
		// (remove) Token: 0x060050AF RID: 20655 RVA: 0x0025A764 File Offset: 0x00258964
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event Action OnCreatingTypeChangeEvent;

		// Token: 0x170009DE RID: 2526
		// (get) Token: 0x060050B0 RID: 20656 RVA: 0x0025A799 File Offset: 0x00258999
		public AvatarData AvatarData { get; }

		// Token: 0x170009DF RID: 2527
		// (get) Token: 0x060050B1 RID: 20657 RVA: 0x0025A7A1 File Offset: 0x002589A1
		// (set) Token: 0x060050B2 RID: 20658 RVA: 0x0025A7A9 File Offset: 0x002589A9
		public short AvatarAge { get; private set; }

		// Token: 0x170009E0 RID: 2528
		// (get) Token: 0x060050B3 RID: 20659 RVA: 0x0025A7B2 File Offset: 0x002589B2
		// (set) Token: 0x060050B4 RID: 20660 RVA: 0x0025A7BA File Offset: 0x002589BA
		public bool HasNewGoods { get; private set; }

		// Token: 0x170009E1 RID: 2529
		// (get) Token: 0x060050B5 RID: 20661 RVA: 0x0025A7C3 File Offset: 0x002589C3
		// (set) Token: 0x060050B6 RID: 20662 RVA: 0x0025A7CB File Offset: 0x002589CB
		public short TemplateId { get; private set; }

		// Token: 0x170009E2 RID: 2530
		// (get) Token: 0x060050B7 RID: 20663 RVA: 0x0025A7D4 File Offset: 0x002589D4
		// (set) Token: 0x060050B8 RID: 20664 RVA: 0x0025A7DC File Offset: 0x002589DC
		public sbyte XiangshuType { get; private set; }

		// Token: 0x170009E3 RID: 2531
		// (get) Token: 0x060050B9 RID: 20665 RVA: 0x0025A7E5 File Offset: 0x002589E5
		// (set) Token: 0x060050BA RID: 20666 RVA: 0x0025A7ED File Offset: 0x002589ED
		public byte CreatingType { get; private set; }

		// Token: 0x170009E4 RID: 2532
		// (get) Token: 0x060050BB RID: 20667 RVA: 0x0025A7F6 File Offset: 0x002589F6
		// (set) Token: 0x060050BC RID: 20668 RVA: 0x0025A7FE File Offset: 0x002589FE
		public uint DarkAshProtector { get; private set; }

		// Token: 0x170009E5 RID: 2533
		// (get) Token: 0x060050BD RID: 20669 RVA: 0x0025A807 File Offset: 0x00258A07
		// (set) Token: 0x060050BE RID: 20670 RVA: 0x0025A80F File Offset: 0x00258A0F
		public OrganizationInfo OrganizationInfo { get; private set; }

		// Token: 0x170009E6 RID: 2534
		// (get) Token: 0x060050BF RID: 20671 RVA: 0x0025A818 File Offset: 0x00258A18
		protected override bool IsPureFieldMonitor
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170009E7 RID: 2535
		// (get) Token: 0x060050C0 RID: 20672 RVA: 0x0025A81C File Offset: 0x00258A1C
		public override List<uint> RelativeFieldIds
		{
			get
			{
				return new List<uint>
				{
					1U,
					2U,
					15U,
					39U,
					75U,
					117U,
					8U
				};
			}
		}

		// Token: 0x060050C1 RID: 20673 RVA: 0x0025A86A File Offset: 0x00258A6A
		public AvatarInfoMonitor()
		{
			this.AvatarData = new AvatarData();
		}

		// Token: 0x060050C2 RID: 20674 RVA: 0x0025A880 File Offset: 0x00258A80
		protected override void MonitorData()
		{
			bool isDead = base.Character.IsDead;
			if (!isDead)
			{
				bool flag = this._equipmentMonitor == null;
				if (flag)
				{
					this._equipmentMonitor = SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<EquipmentMonitor>(base.CharacterId, false);
					this._equipmentMonitor.AddEquipmentChangeListener(new Action<sbyte>(this.OnEquipmentChange));
				}
				foreach (uint fieldId in this.RelativeFieldIds)
				{
					GameDataBridge.AddDataMonitor(base.ListenerId, base.DomainId, base.DataId, base.CharId, fieldId);
				}
				base.Character.CallMethod(47);
				base.Character.CallMethod(34);
			}
		}

		// Token: 0x060050C3 RID: 20675 RVA: 0x0025A960 File Offset: 0x00258B60
		public override void Refresh()
		{
			bool isDead = base.Character.IsDead;
			if (!isDead)
			{
				base.Character.CallMethod(47);
				base.Character.CallMethod(34);
				this._forceRefreshFlag = true;
			}
		}

		// Token: 0x060050C4 RID: 20676 RVA: 0x0025A9A4 File Offset: 0x00258BA4
		public override void OnDataInit()
		{
			Action onAvatarDataChangeEvent = this.OnAvatarDataChangeEvent;
			if (onAvatarDataChangeEvent != null)
			{
				onAvatarDataChangeEvent();
			}
			Action onCreatingTypeChangeEvent = this.OnCreatingTypeChangeEvent;
			if (onCreatingTypeChangeEvent != null)
			{
				onCreatingTypeChangeEvent();
			}
			Action onClothDisplayIdChangeEvent = this.OnClothDisplayIdChangeEvent;
			if (onClothDisplayIdChangeEvent != null)
			{
				onClothDisplayIdChangeEvent();
			}
			Action onHasNewGoodsChangeEvent = this.OnHasNewGoodsChangeEvent;
			if (onHasNewGoodsChangeEvent != null)
			{
				onHasNewGoodsChangeEvent();
			}
		}

		// Token: 0x060050C5 RID: 20677 RVA: 0x0025A9FC File Offset: 0x00258BFC
		public override void InitFromDeadCharacter(DeadCharacter deadCharacter)
		{
			this.TemplateId = deadCharacter.TemplateId;
			this.AvatarData.Copy(deadCharacter.Avatar);
			this.AvatarData.ClothDisplayId = deadCharacter.ClothingDisplayId;
			this.AvatarAge = deadCharacter.GetActualAge();
			this.CreatingType = Config.Character.Instance[this.TemplateId].CreatingType;
			this.DataFlag = 2;
		}

		// Token: 0x060050C6 RID: 20678 RVA: 0x0025AA6C File Offset: 0x00258C6C
		protected override bool IsValidMonitor()
		{
			Action onAvatarDataChangeEvent = this.OnAvatarDataChangeEvent;
			if (onAvatarDataChangeEvent == null || onAvatarDataChangeEvent.GetInvocationList().Length == 0)
			{
				Action onClothDisplayIdChangeEvent = this.OnClothDisplayIdChangeEvent;
				if (onClothDisplayIdChangeEvent == null || onClothDisplayIdChangeEvent.GetInvocationList().Length == 0)
				{
					Action onHasNewGoodsChangeEvent = this.OnHasNewGoodsChangeEvent;
					if (onHasNewGoodsChangeEvent == null || onHasNewGoodsChangeEvent.GetInvocationList().Length == 0)
					{
						Action onCreatingTypeChangeEvent = this.OnCreatingTypeChangeEvent;
						return onCreatingTypeChangeEvent != null && onCreatingTypeChangeEvent.GetInvocationList().Length != 0;
					}
				}
			}
			return true;
		}

		// Token: 0x060050C7 RID: 20679 RVA: 0x0025AAE0 File Offset: 0x00258CE0
		public override void UnMonitorData()
		{
			foreach (uint fieldId in this.RelativeFieldIds)
			{
				GameDataBridge.AddDataUnMonitor(base.ListenerId, base.DomainId, base.DataId, base.CharId, fieldId);
			}
			EquipmentMonitor equipmentMonitor = this._equipmentMonitor;
			if (equipmentMonitor != null)
			{
				equipmentMonitor.RemoveEquipmentChangeListener(new Action<sbyte>(this.OnEquipmentChange));
			}
			this._equipmentMonitor = null;
		}

		// Token: 0x060050C8 RID: 20680 RVA: 0x0025AB78 File Offset: 0x00258D78
		public override void OnNotifyData(NotificationWrapper wrapper)
		{
			byte type = wrapper.Notification.Type;
			byte b = type;
			if (b != 0)
			{
				if (b == 1)
				{
					bool flag = wrapper.Notification.MethodId == 47;
					if (flag)
					{
						short clothDisplayId = this.AvatarData.ClothDisplayId;
						Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref clothDisplayId);
						bool changeFlag = clothDisplayId != this.AvatarData.ClothDisplayId;
						this.AvatarData.ClothDisplayId = clothDisplayId;
						bool flag2 = base.Init && changeFlag;
						if (flag2)
						{
							Action onAvatarDataChangeEvent = this.OnAvatarDataChangeEvent;
							if (onAvatarDataChangeEvent != null)
							{
								onAvatarDataChangeEvent();
							}
							Action onClothDisplayIdChangeEvent = this.OnClothDisplayIdChangeEvent;
							if (onClothDisplayIdChangeEvent != null)
							{
								onClothDisplayIdChangeEvent();
							}
						}
					}
					else
					{
						bool flag3 = wrapper.Notification.MethodId == 34;
						if (flag3)
						{
							bool newFlag = false;
							Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref newFlag);
							bool changeFlag2 = newFlag != this.HasNewGoods;
							this.HasNewGoods = newFlag;
							bool flag4 = base.Init && changeFlag2;
							if (flag4)
							{
								Action onHasNewGoodsChangeEvent = this.OnHasNewGoodsChangeEvent;
								if (onHasNewGoodsChangeEvent != null)
								{
									onHasNewGoodsChangeEvent();
								}
							}
							bool forceRefreshFlag = this._forceRefreshFlag;
							if (forceRefreshFlag)
							{
								this._forceRefreshFlag = false;
								Action onAvatarDataChangeEvent2 = this.OnAvatarDataChangeEvent;
								if (onAvatarDataChangeEvent2 != null)
								{
									onAvatarDataChangeEvent2();
								}
							}
						}
					}
				}
			}
			else
			{
				bool flag5 = wrapper.Notification.Uid.SubId1 == 1U;
				if (flag5)
				{
					short templateId = -1;
					Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref templateId);
					bool flag6 = templateId > 0;
					if (flag6)
					{
						bool changeFlag3 = templateId != this.TemplateId;
						this.TemplateId = templateId;
						bool flag7 = base.Init && changeFlag3;
						if (flag7)
						{
							Action onAvatarDataChangeEvent3 = this.OnAvatarDataChangeEvent;
							if (onAvatarDataChangeEvent3 != null)
							{
								onAvatarDataChangeEvent3();
							}
						}
					}
				}
				else
				{
					bool flag8 = wrapper.Notification.Uid.SubId1 == 2U;
					if (flag8)
					{
						byte creatingType = 0;
						Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref creatingType);
						bool changed = this.CreatingType != creatingType;
						this.CreatingType = creatingType;
						bool flag9 = base.Init && changed;
						if (flag9)
						{
							Action onCreatingTypeChangeEvent = this.OnCreatingTypeChangeEvent;
							if (onCreatingTypeChangeEvent != null)
							{
								onCreatingTypeChangeEvent();
							}
						}
					}
					else
					{
						bool flag10 = wrapper.Notification.Uid.SubId1 == 15U;
						if (flag10)
						{
							sbyte xiangshuType = 0;
							Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref xiangshuType);
							this.XiangshuType = xiangshuType;
						}
						else
						{
							bool flag11 = wrapper.Notification.Uid.SubId1 == 39U;
							if (flag11)
							{
								AvatarData avatarData = new AvatarData();
								Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref avatarData);
								short clothDisplayId2 = this.AvatarData.ClothDisplayId;
								this.AvatarData.Copy(avatarData);
								this.AvatarData.ClothDisplayId = clothDisplayId2;
								try
								{
									bool init = base.Init;
									if (init)
									{
										Action onAvatarDataChangeEvent4 = this.OnAvatarDataChangeEvent;
										if (onAvatarDataChangeEvent4 != null)
										{
											onAvatarDataChangeEvent4();
										}
									}
								}
								catch (Exception e)
								{
									string tag = "AvatarInfoMonitor";
									string format = "{0:HH:mm:ss}[{1}]:{2}";
									object arg = DateTime.Now.ToLocalTime();
									Action onAvatarDataChangeEvent5 = this.OnAvatarDataChangeEvent;
									GLog.TagError(tag, string.Format(format, arg, (onAvatarDataChangeEvent5 != null) ? onAvatarDataChangeEvent5.GetInvocationList()[0].Target : null, e), Array.Empty<object>());
								}
							}
							else
							{
								bool flag12 = wrapper.Notification.Uid.SubId1 == 75U;
								if (flag12)
								{
									short age = -1;
									Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref age);
									bool changeFlag4 = age != this.AvatarAge;
									this.AvatarAge = age;
									try
									{
										bool flag13 = base.Init && changeFlag4;
										if (flag13)
										{
											Action onAvatarDataChangeEvent6 = this.OnAvatarDataChangeEvent;
											if (onAvatarDataChangeEvent6 != null)
											{
												onAvatarDataChangeEvent6();
											}
										}
									}
									catch (Exception e2)
									{
										string tag2 = "AvatarInfoMonitor";
										string format2 = "{0:HH:mm:ss}[{1}]:{2}";
										object arg2 = DateTime.Now.ToLocalTime();
										Action onAvatarDataChangeEvent7 = this.OnAvatarDataChangeEvent;
										GLog.TagError(tag2, string.Format(format2, arg2, (onAvatarDataChangeEvent7 != null) ? onAvatarDataChangeEvent7.GetInvocationList()[0].Target : null, e2), Array.Empty<object>());
									}
									bool flag14 = !base.Init;
									if (flag14)
									{
										this.DataFlag = 1;
									}
								}
								else
								{
									bool flag15 = wrapper.Notification.Uid.SubId1 == 117U;
									if (flag15)
									{
										uint darkAshProtector = 0U;
										Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref darkAshProtector);
										bool changeFlag5 = darkAshProtector != this.DarkAshProtector;
										this.DarkAshProtector = darkAshProtector;
										this.AvatarData.DarkAshStyle = CommonUtils.GetDarkAshStyle(base.CharacterId, darkAshProtector);
										bool flag16 = base.Init && changeFlag5;
										if (flag16)
										{
											Action onAvatarDataChangeEvent8 = this.OnAvatarDataChangeEvent;
											if (onAvatarDataChangeEvent8 != null)
											{
												onAvatarDataChangeEvent8();
											}
										}
									}
									else
									{
										bool flag17 = wrapper.Notification.Uid.SubId1 == 8U;
										if (flag17)
										{
											OrganizationInfo orgInfo = default(OrganizationInfo);
											Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref orgInfo);
											bool changeFlag6 = !orgInfo.Equals(this.OrganizationInfo);
											this.OrganizationInfo = orgInfo;
											bool flag18 = base.Init && changeFlag6;
											if (flag18)
											{
												Action onAvatarDataChangeEvent9 = this.OnAvatarDataChangeEvent;
												if (onAvatarDataChangeEvent9 != null)
												{
													onAvatarDataChangeEvent9();
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x060050C9 RID: 20681 RVA: 0x0025B100 File Offset: 0x00259300
		private void OnEquipmentChange(sbyte equipSlot)
		{
			bool flag = equipSlot == 4;
			if (flag)
			{
				base.Character.CallMethod(47);
			}
		}

		// Token: 0x060050CA RID: 20682 RVA: 0x0025B124 File Offset: 0x00259324
		public void AddOnAvatarDataChangeEventListener(Action listener)
		{
			this.OnAvatarDataChangeEvent -= listener;
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnAvatarDataChangeEvent += listener;
		}

		// Token: 0x060050CB RID: 20683 RVA: 0x0025B146 File Offset: 0x00259346
		public void RemoveOnAvatarDataChangeEventListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnAvatarDataChangeEvent -= listener;
			base.OnChangeEventRemoved();
		}

		// Token: 0x060050CC RID: 20684 RVA: 0x0025B167 File Offset: 0x00259367
		public void AddOnClothDisplayIdChangeEventListener(Action listener)
		{
			this.OnClothDisplayIdChangeEvent -= listener;
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnClothDisplayIdChangeEvent += listener;
		}

		// Token: 0x060050CD RID: 20685 RVA: 0x0025B189 File Offset: 0x00259389
		public void RemoveOnClothDisplayIdChangeEventListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnClothDisplayIdChangeEvent -= listener;
			base.OnChangeEventRemoved();
		}

		// Token: 0x060050CE RID: 20686 RVA: 0x0025B1AA File Offset: 0x002593AA
		public void AddOnHasNewGoodsChangeEventListener(Action listener)
		{
			this.OnHasNewGoodsChangeEvent -= listener;
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnHasNewGoodsChangeEvent += listener;
		}

		// Token: 0x060050CF RID: 20687 RVA: 0x0025B1CC File Offset: 0x002593CC
		public void RemoveOnHasNewGoodsChangeEventListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnHasNewGoodsChangeEvent -= listener;
			base.OnChangeEventRemoved();
		}

		// Token: 0x060050D0 RID: 20688 RVA: 0x0025B1ED File Offset: 0x002593ED
		public void AddOnCreatingTypeChangeEventListener(Action listener)
		{
			this.OnCreatingTypeChangeEvent -= listener;
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnCreatingTypeChangeEvent += listener;
		}

		// Token: 0x060050D1 RID: 20689 RVA: 0x0025B20F File Offset: 0x0025940F
		public void RemoveOnCreatingTypeChangeEventListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnCreatingTypeChangeEvent -= listener;
			base.OnChangeEventRemoved();
		}

		// Token: 0x0400377D RID: 14205
		private EquipmentMonitor _equipmentMonitor;

		// Token: 0x04003786 RID: 14214
		private bool _forceRefreshFlag;
	}
}
