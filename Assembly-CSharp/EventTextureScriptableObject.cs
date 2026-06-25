using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000205 RID: 517
[CreateAssetMenu(fileName = "EventTextureScriptableObject", menuName = "ScriptableObjects/EventTextureScriptableObject", order = 1)]
public class EventTextureScriptableObject : ScriptableObject
{
	// Token: 0x04001983 RID: 6531
	public static EventTextureScriptableObject Instance;

	// Token: 0x04001984 RID: 6532
	public List<string> eventTexturePathList = new List<string>();
}
