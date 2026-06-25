using System;
using System.Collections.Generic;
using System.Linq;

namespace CharacterDataMonitor
{
	// Token: 0x020006C6 RID: 1734
	public class MonitorPerFieldListenManager
	{
		// Token: 0x06005288 RID: 21128 RVA: 0x002620E7 File Offset: 0x002602E7
		public void InitMonitorFuncs(Action<uint> monitorFunc, Action<uint> unMonitorFunc)
		{
			this._monitorFunc = monitorFunc;
			this._unMonitorFunc = unMonitorFunc;
		}

		// Token: 0x06005289 RID: 21129 RVA: 0x002620F8 File Offset: 0x002602F8
		public void InitStateMap(List<uint> fieldIds)
		{
			foreach (uint fieldId in fieldIds)
			{
				this._monitorStateMap[fieldId] = false;
			}
		}

		// Token: 0x0600528A RID: 21130 RVA: 0x00262154 File Offset: 0x00260354
		public void OnAddRemoveListener(uint fieldId, List<MulticastDelegate> delegates)
		{
			bool oldState;
			bool flag = !this._monitorStateMap.TryGetValue(fieldId, out oldState);
			if (!flag)
			{
				bool flag2 = MonitorPerFieldListenManager.HasListener(delegates);
				if (flag2)
				{
					bool flag3 = !oldState;
					if (flag3)
					{
						this._monitorFunc(fieldId);
						this._monitorStateMap[fieldId] = true;
					}
				}
				else
				{
					bool flag4 = oldState;
					if (flag4)
					{
						this._unMonitorFunc(fieldId);
						this._monitorStateMap[fieldId] = false;
					}
				}
			}
		}

		// Token: 0x0600528B RID: 21131 RVA: 0x002621D3 File Offset: 0x002603D3
		public void ClearStateMap()
		{
			Dictionary<uint, bool> monitorStateMap = this._monitorStateMap;
			if (monitorStateMap != null)
			{
				monitorStateMap.Clear();
			}
		}

		// Token: 0x0600528C RID: 21132 RVA: 0x002621E8 File Offset: 0x002603E8
		public static bool HasListener(List<MulticastDelegate> delegates)
		{
			return delegates.Any((MulticastDelegate d) => d != null && d.GetInvocationList().Length != 0);
		}

		// Token: 0x040037FB RID: 14331
		private readonly Dictionary<uint, bool> _monitorStateMap = new Dictionary<uint, bool>();

		// Token: 0x040037FC RID: 14332
		private Action<uint> _monitorFunc;

		// Token: 0x040037FD RID: 14333
		private Action<uint> _unMonitorFunc;
	}
}
