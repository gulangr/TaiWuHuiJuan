using System;
using UnityEngine;

namespace Game.Views.Encyclopedia.Utilities
{
	// Token: 0x02000A75 RID: 2677
	public class SingletonBehaviour<T> : MonoBehaviour where T : SingletonBehaviour<T>
	{
		// Token: 0x17000E71 RID: 3697
		// (get) Token: 0x060083C5 RID: 33733 RVA: 0x003D4EFA File Offset: 0x003D30FA
		// (set) Token: 0x060083C6 RID: 33734 RVA: 0x003D4F01 File Offset: 0x003D3101
		public static T Instance { get; protected set; }

		// Token: 0x060083C7 RID: 33735 RVA: 0x003D4F0C File Offset: 0x003D310C
		protected virtual void Awake()
		{
			bool flag = SingletonBehaviour<T>.Instance != null && SingletonBehaviour<T>.Instance != this;
			if (flag)
			{
				Object.Destroy(this);
				throw new Exception("An Instance of this singleton already exists.");
			}
			SingletonBehaviour<T>.Instance = (T)((object)this);
		}
	}
}
