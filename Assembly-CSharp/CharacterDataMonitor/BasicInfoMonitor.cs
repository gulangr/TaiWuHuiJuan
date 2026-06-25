using System;
using System.Collections.Generic;
using System.Diagnostics;
using FrameWork;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Extra;
using GameData.Domains.Taiwu;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;

namespace CharacterDataMonitor
{
	// Token: 0x020006B8 RID: 1720
	public class BasicInfoMonitor : MonitorDataItemBase
	{
		// Token: 0x14000036 RID: 54
		// (add) Token: 0x060050D2 RID: 20690 RVA: 0x0025B230 File Offset: 0x00259430
		// (remove) Token: 0x060050D3 RID: 20691 RVA: 0x0025B268 File Offset: 0x00259468
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event Action OnNameRelatedDataChangeEvent;

		// Token: 0x14000037 RID: 55
		// (add) Token: 0x060050D4 RID: 20692 RVA: 0x0025B2A0 File Offset: 0x002594A0
		// (remove) Token: 0x060050D5 RID: 20693 RVA: 0x0025B2D8 File Offset: 0x002594D8
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event Action<int> OnCharNameRelatedDataChangeEvent;

		// Token: 0x14000038 RID: 56
		// (add) Token: 0x060050D6 RID: 20694 RVA: 0x0025B310 File Offset: 0x00259510
		// (remove) Token: 0x060050D7 RID: 20695 RVA: 0x0025B348 File Offset: 0x00259548
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event Action OnGenderDataReadyEvent;

		// Token: 0x14000039 RID: 57
		// (add) Token: 0x060050D8 RID: 20696 RVA: 0x0025B380 File Offset: 0x00259580
		// (remove) Token: 0x060050D9 RID: 20697 RVA: 0x0025B3B8 File Offset: 0x002595B8
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event Action OnTitleDataUpdateEvent;

		// Token: 0x1400003A RID: 58
		// (add) Token: 0x060050DA RID: 20698 RVA: 0x0025B3F0 File Offset: 0x002595F0
		// (remove) Token: 0x060050DB RID: 20699 RVA: 0x0025B428 File Offset: 0x00259628
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event Action OnFavorabilityChangeEvent;

		// Token: 0x1400003B RID: 59
		// (add) Token: 0x060050DC RID: 20700 RVA: 0x0025B460 File Offset: 0x00259660
		// (remove) Token: 0x060050DD RID: 20701 RVA: 0x0025B498 File Offset: 0x00259698
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event Action<int> OnCharFavorabilityChangeEvent;

		// Token: 0x1400003C RID: 60
		// (add) Token: 0x060050DE RID: 20702 RVA: 0x0025B4D0 File Offset: 0x002596D0
		// (remove) Token: 0x060050DF RID: 20703 RVA: 0x0025B508 File Offset: 0x00259708
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event Action OnDebtsOfTaiwuChangeEvent;

		// Token: 0x1400003D RID: 61
		// (add) Token: 0x060050E0 RID: 20704 RVA: 0x0025B540 File Offset: 0x00259740
		// (remove) Token: 0x060050E1 RID: 20705 RVA: 0x0025B578 File Offset: 0x00259778
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event Action OnIsInteractedCharacterChangeEvent;

		// Token: 0x170009E8 RID: 2536
		// (get) Token: 0x060050E2 RID: 20706 RVA: 0x0025B5AD File Offset: 0x002597AD
		public sbyte Gender
		{
			get
			{
				return this.NameRelatedData.Gender;
			}
		}

		// Token: 0x170009E9 RID: 2537
		// (get) Token: 0x060050E3 RID: 20707 RVA: 0x0025B5BA File Offset: 0x002597BA
		public List<short> TitleIdList { get; }

		// Token: 0x170009EA RID: 2538
		// (get) Token: 0x060050E4 RID: 20708 RVA: 0x0025B5C2 File Offset: 0x002597C2
		// (set) Token: 0x060050E5 RID: 20709 RVA: 0x0025B5CA File Offset: 0x002597CA
		public short FavorabilityToTaiwu { get; private set; }

		// Token: 0x170009EB RID: 2539
		// (get) Token: 0x060050E6 RID: 20710 RVA: 0x0025B5D3 File Offset: 0x002597D3
		// (set) Token: 0x060050E7 RID: 20711 RVA: 0x0025B5DB File Offset: 0x002597DB
		public bool IsInteractedCharacter { get; private set; }

		// Token: 0x170009EC RID: 2540
		// (get) Token: 0x060050E8 RID: 20712 RVA: 0x0025B5E4 File Offset: 0x002597E4
		public bool HasAlertness
		{
			get
			{
				return this.Alertness != 0;
			}
		}

		// Token: 0x170009ED RID: 2541
		// (get) Token: 0x060050E9 RID: 20713 RVA: 0x0025B5EF File Offset: 0x002597EF
		protected override bool IsPureFieldMonitor
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170009EE RID: 2542
		// (get) Token: 0x060050EA RID: 20714 RVA: 0x0025B5F2 File Offset: 0x002597F2
		public override List<uint> RelativeFieldIds
		{
			get
			{
				return new List<uint>
				{
					1U,
					16U,
					38U,
					37U,
					8U,
					3U
				};
			}
		}

		// Token: 0x060050EB RID: 20715 RVA: 0x0025B62C File Offset: 0x0025982C
		protected override void MonitorData()
		{
			bool flag = !base.IsTaiwu;
			if (flag)
			{
				int taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
				base.Character.CallMethod<int>(11, taiwuCharId);
				bool flag2 = !base.Character.IsDead;
				if (flag2)
				{
					base.Character.CallMethod(159);
				}
			}
			else
			{
				this.FavorabilityToTaiwu = short.MinValue;
			}
			base.Character.CallMethod(8);
			GEvent.Add(UiEvents.NickNameChanged, new GEvent.Callback(this.OnSomeoneNickNameChanged));
			bool isDead = base.Character.IsDead;
			if (!isDead)
			{
				foreach (uint fieldId in this.RelativeFieldIds)
				{
					GameDataBridge.AddDataMonitor(base.ListenerId, base.DomainId, base.DataId, base.CharId, fieldId);
				}
				base.Character.CallMethod(94);
			}
		}

		// Token: 0x060050EC RID: 20716 RVA: 0x0025B744 File Offset: 0x00259944
		public override void UnMonitorData()
		{
			GEvent.Remove(UiEvents.NickNameChanged, new GEvent.Callback(this.OnSomeoneNickNameChanged));
			foreach (uint fieldId in this.RelativeFieldIds)
			{
				GameDataBridge.AddDataUnMonitor(base.ListenerId, base.DomainId, base.DataId, base.CharId, fieldId);
			}
		}

		// Token: 0x060050ED RID: 20717 RVA: 0x0025B7D0 File Offset: 0x002599D0
		private void OnSomeoneNickNameChanged(ArgumentBox argbox)
		{
			int charId;
			argbox.Get("CharacterId", out charId);
			bool flag = (long)charId == (long)base.CharId;
			if (flag)
			{
				base.Character.CallMethod(8);
			}
		}

		// Token: 0x060050EE RID: 20718 RVA: 0x0025B80C File Offset: 0x00259A0C
		public override void OnDataInit()
		{
			Action onNameRelatedDataChangeEvent = this.OnNameRelatedDataChangeEvent;
			if (onNameRelatedDataChangeEvent != null)
			{
				onNameRelatedDataChangeEvent();
			}
			Action<int> onCharNameRelatedDataChangeEvent = this.OnCharNameRelatedDataChangeEvent;
			if (onCharNameRelatedDataChangeEvent != null)
			{
				onCharNameRelatedDataChangeEvent(base.CharacterId);
			}
			Action onGenderDataReadyEvent = this.OnGenderDataReadyEvent;
			if (onGenderDataReadyEvent != null)
			{
				onGenderDataReadyEvent();
			}
			Action onTitleDataUpdateEvent = this.OnTitleDataUpdateEvent;
			if (onTitleDataUpdateEvent != null)
			{
				onTitleDataUpdateEvent();
			}
			Action onFavorabilityChangeEvent = this.OnFavorabilityChangeEvent;
			if (onFavorabilityChangeEvent != null)
			{
				onFavorabilityChangeEvent();
			}
			Action onIsInteractedCharacterChangeEvent = this.OnIsInteractedCharacterChangeEvent;
			if (onIsInteractedCharacterChangeEvent != null)
			{
				onIsInteractedCharacterChangeEvent();
			}
			Action<int> onCharFavorabilityChangeEvent = this.OnCharFavorabilityChangeEvent;
			if (onCharFavorabilityChangeEvent != null)
			{
				onCharFavorabilityChangeEvent(base.CharacterId);
			}
			Action onDebtsOfTaiwuChangeEvent = this.OnDebtsOfTaiwuChangeEvent;
			if (onDebtsOfTaiwuChangeEvent != null)
			{
				onDebtsOfTaiwuChangeEvent();
			}
		}

		// Token: 0x060050EF RID: 20719 RVA: 0x0025B8B8 File Offset: 0x00259AB8
		public override void InitFromDeadCharacter(DeadCharacter deadCharacter)
		{
			this.TitleIdList.Clear();
			bool flag = deadCharacter.TitleIds != null && deadCharacter.TitleIds.Count > 0;
			if (flag)
			{
				this.TitleIdList.AddRange(deadCharacter.TitleIds);
			}
			this.NameRelatedData.CharTemplateId = deadCharacter.TemplateId;
			this.NameRelatedData.FullName = deadCharacter.FullName;
			this.NameRelatedData.Gender = deadCharacter.Gender;
			this.NameRelatedData.MonasticTitle = deadCharacter.MonasticTitle;
			this.NameRelatedData.MonkType = deadCharacter.MonkType;
			this.NameRelatedData.OrgGrade = deadCharacter.OrganizationInfo.Grade;
			this.NameRelatedData.OrgTemplateId = deadCharacter.OrganizationInfo.OrgTemplateId;
			this.NameRelatedData.NickNameId = -1;
			this.NameRelatedData.ExtraNameTextTemplateId = -1;
			this.NameRelatedData.CustomDisplayNameId = -1;
			ExtraDomainMethod.AsyncCall.GetCharacterCustomDisplayName(null, base.CharacterId, delegate(int offset, RawDataPool dataPool)
			{
				int customDisplayNameId = -1;
				Serializer.Deserialize(dataPool, offset, ref customDisplayNameId);
				this.NameRelatedData.CustomDisplayNameId = customDisplayNameId;
				Action onNameRelatedDataChangeEvent = this.OnNameRelatedDataChangeEvent;
				if (onNameRelatedDataChangeEvent != null)
				{
					onNameRelatedDataChangeEvent();
				}
				Action<int> onCharNameRelatedDataChangeEvent = this.OnCharNameRelatedDataChangeEvent;
				if (onCharNameRelatedDataChangeEvent != null)
				{
					onCharNameRelatedDataChangeEvent(base.CharacterId);
				}
			});
			TaiwuDomainMethod.AsyncCall.GetFollowingNpcNickNameId(null, base.CharacterId, delegate(int offset, RawDataPool dataPool)
			{
				int nickNameId = -1;
				Serializer.Deserialize(dataPool, offset, ref nickNameId);
				this.NameRelatedData.NickNameId = nickNameId;
				Action onNameRelatedDataChangeEvent = this.OnNameRelatedDataChangeEvent;
				if (onNameRelatedDataChangeEvent != null)
				{
					onNameRelatedDataChangeEvent();
				}
				Action<int> onCharNameRelatedDataChangeEvent = this.OnCharNameRelatedDataChangeEvent;
				if (onCharNameRelatedDataChangeEvent != null)
				{
					onCharNameRelatedDataChangeEvent(base.CharacterId);
				}
			});
			this.Alertness = 0;
			this.DataFlag = 2;
		}

		// Token: 0x060050F0 RID: 20720 RVA: 0x0025B9E8 File Offset: 0x00259BE8
		protected override bool IsValidMonitor()
		{
			Action onNameRelatedDataChangeEvent = this.OnNameRelatedDataChangeEvent;
			bool flag;
			if (onNameRelatedDataChangeEvent == null || onNameRelatedDataChangeEvent.GetInvocationList().Length == 0)
			{
				Action<int> onCharNameRelatedDataChangeEvent = this.OnCharNameRelatedDataChangeEvent;
				flag = (onCharNameRelatedDataChangeEvent != null && onCharNameRelatedDataChangeEvent.GetInvocationList().Length != 0);
			}
			else
			{
				flag = true;
			}
			bool flag2 = flag;
			bool result;
			if (flag2)
			{
				result = true;
			}
			else
			{
				Action onGenderDataReadyEvent = this.OnGenderDataReadyEvent;
				bool flag3 = onGenderDataReadyEvent != null && onGenderDataReadyEvent.GetInvocationList().Length != 0;
				if (flag3)
				{
					result = true;
				}
				else
				{
					Action onTitleDataUpdateEvent = this.OnTitleDataUpdateEvent;
					bool flag4 = onTitleDataUpdateEvent != null && onTitleDataUpdateEvent.GetInvocationList().Length != 0;
					if (flag4)
					{
						result = true;
					}
					else
					{
						Action onFavorabilityChangeEvent = this.OnFavorabilityChangeEvent;
						bool flag5;
						if (onFavorabilityChangeEvent == null || onFavorabilityChangeEvent.GetInvocationList().Length == 0)
						{
							Action<int> onCharFavorabilityChangeEvent = this.OnCharFavorabilityChangeEvent;
							flag5 = (onCharFavorabilityChangeEvent != null && onCharFavorabilityChangeEvent.GetInvocationList().Length != 0);
						}
						else
						{
							flag5 = true;
						}
						bool flag6 = flag5;
						if (flag6)
						{
							result = true;
						}
						else
						{
							Action onIsInteractedCharacterChangeEvent = this.OnIsInteractedCharacterChangeEvent;
							bool flag7 = onIsInteractedCharacterChangeEvent != null && onIsInteractedCharacterChangeEvent.GetInvocationList().Length != 0;
							if (flag7)
							{
								result = true;
							}
							else
							{
								Action onDebtsOfTaiwuChangeEvent = this.OnDebtsOfTaiwuChangeEvent;
								bool flag8 = onDebtsOfTaiwuChangeEvent != null && onDebtsOfTaiwuChangeEvent.GetInvocationList().Length != 0;
								result = flag8;
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x060050F1 RID: 20721 RVA: 0x0025BAF4 File Offset: 0x00259CF4
		public override void OnNotifyData(NotificationWrapper wrapper)
		{
			byte type = wrapper.Notification.Type;
			byte b = type;
			if (b != 0)
			{
				if (b == 1)
				{
					bool flag = wrapper.Notification.MethodId == 94;
					if (flag)
					{
						List<short> newTitleIds = new List<short>();
						Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref newTitleIds);
						bool changeFlag = newTitleIds.Count != this.TitleIdList.Count;
						bool flag2 = !changeFlag;
						if (flag2)
						{
							for (int i = 0; i < newTitleIds.Count; i++)
							{
								bool flag3 = newTitleIds[i] != this.TitleIdList[i];
								if (flag3)
								{
									changeFlag = true;
									break;
								}
							}
						}
						this.TitleIdList.Clear();
						this.TitleIdList.AddRange(newTitleIds);
						bool flag4 = base.Init && changeFlag;
						if (flag4)
						{
							Action onTitleDataUpdateEvent = this.OnTitleDataUpdateEvent;
							if (onTitleDataUpdateEvent != null)
							{
								onTitleDataUpdateEvent();
							}
						}
					}
					else
					{
						bool flag5 = wrapper.Notification.MethodId == 11;
						if (flag5)
						{
							short favorabilityToTaiwu = 0;
							Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref favorabilityToTaiwu);
							bool changeFlag2 = favorabilityToTaiwu != this.FavorabilityToTaiwu;
							this.FavorabilityToTaiwu = favorabilityToTaiwu;
							bool flag6 = base.Init && changeFlag2;
							if (flag6)
							{
								Action onFavorabilityChangeEvent = this.OnFavorabilityChangeEvent;
								if (onFavorabilityChangeEvent != null)
								{
									onFavorabilityChangeEvent();
								}
								Action<int> onCharFavorabilityChangeEvent = this.OnCharFavorabilityChangeEvent;
								if (onCharFavorabilityChangeEvent != null)
								{
									onCharFavorabilityChangeEvent(base.CharacterId);
								}
							}
							base.Character.CallMethod(159);
						}
						else
						{
							bool flag7 = wrapper.Notification.MethodId == 159;
							if (flag7)
							{
								bool isInteracted = false;
								Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref isInteracted);
								this.IsInteractedCharacter = isInteracted;
								bool init = base.Init;
								if (init)
								{
									Action onIsInteractedCharacterChangeEvent = this.OnIsInteractedCharacterChangeEvent;
									if (onIsInteractedCharacterChangeEvent != null)
									{
										onIsInteractedCharacterChangeEvent();
									}
								}
							}
							else
							{
								bool flag8 = wrapper.Notification.MethodId == 8;
								if (flag8)
								{
									Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref this.NameRelatedData);
									bool init2 = base.Init;
									if (init2)
									{
										Action onNameRelatedDataChangeEvent = this.OnNameRelatedDataChangeEvent;
										if (onNameRelatedDataChangeEvent != null)
										{
											onNameRelatedDataChangeEvent();
										}
										Action onGenderDataReadyEvent = this.OnGenderDataReadyEvent;
										if (onGenderDataReadyEvent != null)
										{
											onGenderDataReadyEvent();
										}
										Action<int> onCharNameRelatedDataChangeEvent = this.OnCharNameRelatedDataChangeEvent;
										if (onCharNameRelatedDataChangeEvent != null)
										{
											onCharNameRelatedDataChangeEvent(base.CharacterId);
										}
									}
									else
									{
										this.DataFlag = 1;
									}
								}
							}
						}
					}
				}
			}
			else
			{
				bool flag9 = wrapper.Notification.Uid.SubId1 == 1U || wrapper.Notification.Uid.SubId1 == 16U || wrapper.Notification.Uid.SubId1 == 38U || wrapper.Notification.Uid.SubId1 == 37U || wrapper.Notification.Uid.SubId1 == 8U || wrapper.Notification.Uid.SubId1 == 3U;
				if (flag9)
				{
					bool init3 = base.Init;
					if (init3)
					{
						base.Character.CallMethod(8);
					}
				}
			}
		}

		// Token: 0x060050F2 RID: 20722 RVA: 0x0025BE34 File Offset: 0x0025A034
		public override void Refresh()
		{
			bool init = base.Init;
			if (init)
			{
				Action onNameRelatedDataChangeEvent = this.OnNameRelatedDataChangeEvent;
				if (onNameRelatedDataChangeEvent != null)
				{
					onNameRelatedDataChangeEvent();
				}
				Action<int> onCharNameRelatedDataChangeEvent = this.OnCharNameRelatedDataChangeEvent;
				if (onCharNameRelatedDataChangeEvent != null)
				{
					onCharNameRelatedDataChangeEvent(base.CharacterId);
				}
				Action onGenderDataReadyEvent = this.OnGenderDataReadyEvent;
				if (onGenderDataReadyEvent != null)
				{
					onGenderDataReadyEvent();
				}
				Action onFavorabilityChangeEvent = this.OnFavorabilityChangeEvent;
				if (onFavorabilityChangeEvent != null)
				{
					onFavorabilityChangeEvent();
				}
				Action onIsInteractedCharacterChangeEvent = this.OnIsInteractedCharacterChangeEvent;
				if (onIsInteractedCharacterChangeEvent != null)
				{
					onIsInteractedCharacterChangeEvent();
				}
				Action<int> onCharFavorabilityChangeEvent = this.OnCharFavorabilityChangeEvent;
				if (onCharFavorabilityChangeEvent != null)
				{
					onCharFavorabilityChangeEvent(base.CharacterId);
				}
				Action onDebtsOfTaiwuChangeEvent = this.OnDebtsOfTaiwuChangeEvent;
				if (onDebtsOfTaiwuChangeEvent != null)
				{
					onDebtsOfTaiwuChangeEvent();
				}
			}
			bool isDead = base.Character.IsDead;
			if (isDead)
			{
				Action onTitleDataUpdateEvent = this.OnTitleDataUpdateEvent;
				if (onTitleDataUpdateEvent != null)
				{
					onTitleDataUpdateEvent();
				}
				Action onFavorabilityChangeEvent2 = this.OnFavorabilityChangeEvent;
				if (onFavorabilityChangeEvent2 != null)
				{
					onFavorabilityChangeEvent2();
				}
				Action onIsInteractedCharacterChangeEvent2 = this.OnIsInteractedCharacterChangeEvent;
				if (onIsInteractedCharacterChangeEvent2 != null)
				{
					onIsInteractedCharacterChangeEvent2();
				}
				Action<int> onCharFavorabilityChangeEvent2 = this.OnCharFavorabilityChangeEvent;
				if (onCharFavorabilityChangeEvent2 != null)
				{
					onCharFavorabilityChangeEvent2(base.CharacterId);
				}
			}
			else
			{
				bool flag = !base.IsTaiwu;
				if (flag)
				{
					int taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
					base.Character.CallMethod<int>(11, taiwuCharId);
				}
				base.Character.CallMethod(94);
			}
		}

		// Token: 0x060050F3 RID: 20723 RVA: 0x0025BF72 File Offset: 0x0025A172
		public BasicInfoMonitor()
		{
			this.NameRelatedData = default(NameRelatedData);
			this.TitleIdList = new List<short>();
			this.FavorabilityToTaiwu = 0;
		}

		// Token: 0x060050F4 RID: 20724 RVA: 0x0025BF9B File Offset: 0x0025A19B
		public void AddNameDataListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnNameRelatedDataChangeEvent -= listener;
			this.OnNameRelatedDataChangeEvent += listener;
		}

		// Token: 0x060050F5 RID: 20725 RVA: 0x0025BFBD File Offset: 0x0025A1BD
		public void RemoveNameDataListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnNameRelatedDataChangeEvent -= listener;
			base.OnChangeEventRemoved();
		}

		// Token: 0x060050F6 RID: 20726 RVA: 0x0025BFDE File Offset: 0x0025A1DE
		public void AddNameDataListener(Action<int> listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnCharNameRelatedDataChangeEvent -= listener;
			this.OnCharNameRelatedDataChangeEvent += listener;
		}

		// Token: 0x060050F7 RID: 20727 RVA: 0x0025C000 File Offset: 0x0025A200
		public void RemoveNameDataListener(Action<int> listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnCharNameRelatedDataChangeEvent -= listener;
			base.OnChangeEventRemoved();
		}

		// Token: 0x060050F8 RID: 20728 RVA: 0x0025C021 File Offset: 0x0025A221
		public void AddGenderListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnGenderDataReadyEvent -= listener;
			this.OnGenderDataReadyEvent += listener;
		}

		// Token: 0x060050F9 RID: 20729 RVA: 0x0025C043 File Offset: 0x0025A243
		public void RemoveGenderListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnGenderDataReadyEvent -= listener;
			base.OnChangeEventRemoved();
		}

		// Token: 0x060050FA RID: 20730 RVA: 0x0025C064 File Offset: 0x0025A264
		public void AddTitleListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnTitleDataUpdateEvent -= listener;
			this.OnTitleDataUpdateEvent += listener;
		}

		// Token: 0x060050FB RID: 20731 RVA: 0x0025C086 File Offset: 0x0025A286
		public void RemoveTitleListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnTitleDataUpdateEvent -= listener;
			base.OnChangeEventRemoved();
		}

		// Token: 0x060050FC RID: 20732 RVA: 0x0025C0A7 File Offset: 0x0025A2A7
		public void AddFavorabilityListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnFavorabilityChangeEvent -= listener;
			this.OnFavorabilityChangeEvent += listener;
		}

		// Token: 0x060050FD RID: 20733 RVA: 0x0025C0C9 File Offset: 0x0025A2C9
		public void RemoveFavorabilityListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnFavorabilityChangeEvent -= listener;
			base.OnChangeEventRemoved();
		}

		// Token: 0x060050FE RID: 20734 RVA: 0x0025C0EA File Offset: 0x0025A2EA
		public void AddIsInteractedCharacterListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnIsInteractedCharacterChangeEvent -= listener;
			this.OnIsInteractedCharacterChangeEvent += listener;
		}

		// Token: 0x060050FF RID: 20735 RVA: 0x0025C10C File Offset: 0x0025A30C
		public void RemoveIsInteractedCharacterListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnIsInteractedCharacterChangeEvent -= listener;
			base.OnChangeEventRemoved();
		}

		// Token: 0x06005100 RID: 20736 RVA: 0x0025C12D File Offset: 0x0025A32D
		public void AddFavorabilityListener(Action<int> listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnCharFavorabilityChangeEvent -= listener;
			this.OnCharFavorabilityChangeEvent += listener;
		}

		// Token: 0x06005101 RID: 20737 RVA: 0x0025C14F File Offset: 0x0025A34F
		public void RemoveFavorabilityListener(Action<int> listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnCharFavorabilityChangeEvent -= listener;
			base.OnChangeEventRemoved();
		}

		// Token: 0x06005102 RID: 20738 RVA: 0x0025C170 File Offset: 0x0025A370
		public void AddDebtsOfTaiwuListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnDebtsOfTaiwuChangeEvent -= listener;
			this.OnDebtsOfTaiwuChangeEvent += listener;
		}

		// Token: 0x06005103 RID: 20739 RVA: 0x0025C192 File Offset: 0x0025A392
		public void RemoveDebtsOfTaiwuListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnDebtsOfTaiwuChangeEvent -= listener;
			base.OnChangeEventRemoved();
		}

		// Token: 0x0400378F RID: 14223
		public NameRelatedData NameRelatedData;

		// Token: 0x04003793 RID: 14227
		public int Alertness;
	}
}
