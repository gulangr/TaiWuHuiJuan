using System;
using System.Collections.Generic;
using Config;
using GameData.Domains.Global;
using GameData.Serializer;
using GameData.Utilities;

namespace Game.Views.Achievement
{
	// Token: 0x02000C81 RID: 3201
	internal sealed class GameStatProgressReader
	{
		// Token: 0x17001110 RID: 4368
		// (get) Token: 0x0600A372 RID: 41842 RVA: 0x004C7EAA File Offset: 0x004C60AA
		public bool HasAny
		{
			get
			{
				return this._globalRecords.Count > 0 || this._localRecords.Count > 0;
			}
		}

		// Token: 0x0600A373 RID: 41843 RVA: 0x004C7ECB File Offset: 0x004C60CB
		public void ApplyGlobalModification(RawDataPool dataPool, int valueOffset)
		{
			Serializer.DeserializeModifications<short>(dataPool, valueOffset, this._globalRecords);
		}

		// Token: 0x0600A374 RID: 41844 RVA: 0x004C7EDB File Offset: 0x004C60DB
		public void ApplyLocalModification(RawDataPool dataPool, int valueOffset)
		{
			Serializer.DeserializeModifications<short>(dataPool, valueOffset, this._localRecords);
		}

		// Token: 0x0600A375 RID: 41845 RVA: 0x004C7EEC File Offset: 0x004C60EC
		public bool TryGetValue(short statId, out int value)
		{
			EStatInfoSaveType saveType = StatInfo.Instance[statId].SaveType;
			Dictionary<short, GameStatRecordWrapper> records = (saveType == EStatInfoSaveType.Global) ? this._globalRecords : this._localRecords;
			GameStatRecordWrapper wrapper;
			bool flag = records.TryGetValue(statId, out wrapper) && wrapper != null;
			bool result;
			if (flag)
			{
				value = wrapper.GetStat();
				result = true;
			}
			else
			{
				value = 0;
				result = false;
			}
			return result;
		}

		// Token: 0x04007F2D RID: 32557
		private readonly Dictionary<short, GameStatRecordWrapper> _globalRecords = new Dictionary<short, GameStatRecordWrapper>();

		// Token: 0x04007F2E RID: 32558
		private readonly Dictionary<short, GameStatRecordWrapper> _localRecords = new Dictionary<short, GameStatRecordWrapper>();
	}
}
