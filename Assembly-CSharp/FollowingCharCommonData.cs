using System;
using System.Collections.Generic;
using GameData.DLC.FiveLoong;
using UnityEngine;

// Token: 0x020003C2 RID: 962
public class FollowingCharCommonData : MonoBehaviour
{
	// Token: 0x040029F7 RID: 10743
	public List<LoongInfo> LoongInfos;

	// Token: 0x040029F8 RID: 10744
	public HashSet<int> UsingLocalNickNameIdSet = new HashSet<int>();

	// Token: 0x040029F9 RID: 10745
	public IAsyncMethodRequestHandler RequestHandler;
}
