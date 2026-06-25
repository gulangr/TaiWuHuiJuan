using System;
using System.Collections.Generic;
using System.Diagnostics;
using Config;
using Config.ConfigCells.Character;
using GameData.Domains.Character;
using GameData.GameDataBridge;
using GameData.Serializer;

namespace CharacterDataMonitor
{
	// Token: 0x020006C2 RID: 1730
	public class FeatureMonitor : MonitorDataItemBase
	{
		// Token: 0x1400005E RID: 94
		// (add) Token: 0x0600521D RID: 21021 RVA: 0x002607C4 File Offset: 0x0025E9C4
		// (remove) Token: 0x0600521E RID: 21022 RVA: 0x002607FC File Offset: 0x0025E9FC
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event Action OnFeatureChangeEvent;

		// Token: 0x1400005F RID: 95
		// (add) Token: 0x0600521F RID: 21023 RVA: 0x00260834 File Offset: 0x0025EA34
		// (remove) Token: 0x06005220 RID: 21024 RVA: 0x0026086C File Offset: 0x0025EA6C
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event Action<int> OnCharFeatureChangeEvent;

		// Token: 0x17000A21 RID: 2593
		// (get) Token: 0x06005221 RID: 21025 RVA: 0x002608A1 File Offset: 0x0025EAA1
		public override List<uint> RelativeFieldIds
		{
			get
			{
				return new List<uint>
				{
					17U,
					55U
				};
			}
		}

		// Token: 0x17000A22 RID: 2594
		// (get) Token: 0x06005222 RID: 21026 RVA: 0x002608BA File Offset: 0x0025EABA
		protected override bool IsPureFieldMonitor
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06005223 RID: 21027 RVA: 0x002608BD File Offset: 0x0025EABD
		public FeatureMonitor()
		{
			this.FeatureIds = new List<short>();
		}

		// Token: 0x06005224 RID: 21028 RVA: 0x002608D4 File Offset: 0x0025EAD4
		protected override void MonitorData()
		{
			bool isDead = base.Character.IsDead;
			if (!isDead)
			{
				foreach (uint fieldId in this.RelativeFieldIds)
				{
					GameDataBridge.AddDataMonitor(base.ListenerId, base.DomainId, base.DataId, base.CharId, fieldId);
				}
			}
		}

		// Token: 0x06005225 RID: 21029 RVA: 0x00260958 File Offset: 0x0025EB58
		public override void OnDataInit()
		{
			Action onFeatureChangeEvent = this.OnFeatureChangeEvent;
			if (onFeatureChangeEvent != null)
			{
				onFeatureChangeEvent();
			}
			Action<int> onCharFeatureChangeEvent = this.OnCharFeatureChangeEvent;
			if (onCharFeatureChangeEvent != null)
			{
				onCharFeatureChangeEvent(base.CharacterId);
			}
		}

		// Token: 0x06005226 RID: 21030 RVA: 0x00260988 File Offset: 0x0025EB88
		public override void InitFromDeadCharacter(DeadCharacter deadCharacter)
		{
			this.FeatureIds.Clear();
			bool flag = deadCharacter.FeatureIds != null;
			if (flag)
			{
				this.FeatureIds.AddRange(deadCharacter.FeatureIds);
			}
			this.DataFlag = 2;
		}

		// Token: 0x06005227 RID: 21031 RVA: 0x002609C8 File Offset: 0x0025EBC8
		protected override bool IsValidMonitor()
		{
			Action onFeatureChangeEvent = this.OnFeatureChangeEvent;
			bool result;
			if (onFeatureChangeEvent == null || onFeatureChangeEvent.GetInvocationList().Length == 0)
			{
				Action<int> onCharFeatureChangeEvent = this.OnCharFeatureChangeEvent;
				result = (onCharFeatureChangeEvent != null && onCharFeatureChangeEvent.GetInvocationList().Length != 0);
			}
			else
			{
				result = true;
			}
			return result;
		}

		// Token: 0x06005228 RID: 21032 RVA: 0x00260A0C File Offset: 0x0025EC0C
		public override void UnMonitorData()
		{
			foreach (uint fieldId in this.RelativeFieldIds)
			{
				GameDataBridge.AddDataUnMonitor(base.ListenerId, base.DomainId, base.DataId, base.CharId, fieldId);
			}
		}

		// Token: 0x06005229 RID: 21033 RVA: 0x00260A7C File Offset: 0x0025EC7C
		public override void OnNotifyData(NotificationWrapper wrapper)
		{
			bool flag;
			if (wrapper.Notification.Type == 0)
			{
				uint subId = wrapper.Notification.Uid.SubId1;
				flag = (subId == 17U || subId == 55U);
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			if (flag2)
			{
				base.Character.CallMethod(183);
			}
			else
			{
				bool flag3 = wrapper.Notification.Type == 1 && wrapper.Notification.MethodId == 183;
				if (flag3)
				{
					List<short> newFeatureIds = new List<short>();
					Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref newFeatureIds);
					bool changeFlag = false;
					for (int i = 0; i < newFeatureIds.Count; i++)
					{
						bool flag4 = !this.FeatureIds.Contains(newFeatureIds[i]);
						if (flag4)
						{
							changeFlag = true;
							break;
						}
					}
					for (int j = 0; j < this.FeatureIds.Count; j++)
					{
						bool flag5 = !newFeatureIds.Contains(this.FeatureIds[j]);
						if (flag5)
						{
							changeFlag = true;
							break;
						}
					}
					this.FeatureIds.Clear();
					this.FeatureIds.AddRange(newFeatureIds);
					bool flag6 = base.Init && changeFlag;
					if (flag6)
					{
						Action onFeatureChangeEvent = this.OnFeatureChangeEvent;
						if (onFeatureChangeEvent != null)
						{
							onFeatureChangeEvent();
						}
						Action<int> onCharFeatureChangeEvent = this.OnCharFeatureChangeEvent;
						if (onCharFeatureChangeEvent != null)
						{
							onCharFeatureChangeEvent(base.CharacterId);
						}
					}
					bool flag7 = !base.Init;
					if (flag7)
					{
						this.DataFlag = 1;
					}
				}
			}
		}

		// Token: 0x0600522A RID: 21034 RVA: 0x00260C13 File Offset: 0x0025EE13
		public void AddFeatureListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnFeatureChangeEvent -= listener;
			this.OnFeatureChangeEvent += listener;
		}

		// Token: 0x0600522B RID: 21035 RVA: 0x00260C35 File Offset: 0x0025EE35
		public void RemoveFeatureListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnFeatureChangeEvent -= listener;
			base.OnChangeEventRemoved();
		}

		// Token: 0x0600522C RID: 21036 RVA: 0x00260C56 File Offset: 0x0025EE56
		public void AddFeatureListener(Action<int> listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnCharFeatureChangeEvent -= listener;
			this.OnCharFeatureChangeEvent += listener;
		}

		// Token: 0x0600522D RID: 21037 RVA: 0x00260C78 File Offset: 0x0025EE78
		public void RemoveFeatureListener(Action<int> listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnCharFeatureChangeEvent -= listener;
			base.OnChangeEventRemoved();
		}

		// Token: 0x0600522E RID: 21038 RVA: 0x00260C9C File Offset: 0x0025EE9C
		public int GetFeatureMedalValue(sbyte medalType)
		{
			int baseValue = 0;
			int bonus = 0;
			int featuresIdCount = this.FeatureIds.Count;
			for (int i = 0; i < featuresIdCount; i++)
			{
				short featureId = this.FeatureIds[i];
				FeatureMedals[] allMedals = CharacterFeature.Instance[featureId].FeatureMedals;
				List<sbyte> currValues = allMedals[(int)medalType].Values;
				int currValuesCount = currValues.Count;
				for (int j = 0; j < currValuesCount; j++)
				{
					switch (currValues[j])
					{
					case 0:
						baseValue++;
						break;
					case 1:
						baseValue--;
						break;
					case 2:
						bonus++;
						break;
					case 3:
						bonus -= 3;
						break;
					}
				}
			}
			bool flag = baseValue > 0;
			int result;
			if (flag)
			{
				int finalValue = baseValue + bonus;
				bool flag2 = finalValue < 0;
				if (flag2)
				{
					result = 0;
				}
				else
				{
					bool flag3 = finalValue > 8;
					if (flag3)
					{
						result = 8;
					}
					else
					{
						result = finalValue;
					}
				}
			}
			else
			{
				bool flag4 = baseValue < 0;
				if (flag4)
				{
					int finalValue2 = baseValue - bonus;
					bool flag5 = finalValue2 < -8;
					if (flag5)
					{
						result = -8;
					}
					else
					{
						bool flag6 = finalValue2 > 0;
						if (flag6)
						{
							result = 0;
						}
						else
						{
							result = finalValue2;
						}
					}
				}
				else
				{
					result = 0;
				}
			}
			return result;
		}

		// Token: 0x040037E3 RID: 14307
		public readonly List<short> FeatureIds;
	}
}
