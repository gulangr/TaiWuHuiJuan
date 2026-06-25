using System;
using UnityEngine;

namespace GameData.GameDataBridge.UnityAdapter
{
	// Token: 0x02000FBA RID: 4026
	public class GameDataBridgeUnityAdapterLateUpdate : MonoBehaviour
	{
		// Token: 0x0600B931 RID: 47409 RVA: 0x00546540 File Offset: 0x00544740
		private void LateUpdate()
		{
			bool shouldDisconnect = GameDataBridge.ShouldDisconnect;
			if (!shouldDisconnect)
			{
				bool flag = !GameDataBridgeUnityAdapter.IsConnected;
				if (!flag)
				{
					GameDataBridge.TransferPendingOperations();
				}
			}
		}
	}
}
