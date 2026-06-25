using System;
using GameData.Serializer;
using UnityEngine;

namespace GameData.GameDataBridge.UnityAdapter
{
	// Token: 0x02000FB9 RID: 4025
	public class GameDataBridgeUnityAdapter : MonoBehaviour
	{
		// Token: 0x0600B92C RID: 47404 RVA: 0x005464A8 File Offset: 0x005446A8
		private void Awake()
		{
			Debug.Log("Awaking GameDataBridgeUnityAdapter.");
		}

		// Token: 0x0600B92D RID: 47405 RVA: 0x005464B6 File Offset: 0x005446B6
		private void Start()
		{
			Debug.Log("Starting GameDataBridgeUnityAdapter.");
			Serializer.Initialize();
			DisplayEventHandler.Initialize();
			GameDataBridge.Initialize();
			GlobalOperations.Initialize();
		}

		// Token: 0x0600B92E RID: 47406 RVA: 0x005464DC File Offset: 0x005446DC
		private void Update()
		{
			GameDataBridge.CheckWarningMessages();
			bool flag = !GameDataBridge.CheckErrorMessages();
			if (flag)
			{
				GameDataBridge.ShouldDisconnect = true;
			}
			else
			{
				bool shouldDisconnect = GameDataBridge.ShouldDisconnect;
				if (!shouldDisconnect)
				{
					bool flag2 = !GameDataBridgeUnityAdapter.IsConnected;
					if (!flag2)
					{
						GameDataBridge.ProcessNotifications();
					}
				}
			}
		}

		// Token: 0x0600B92F RID: 47407 RVA: 0x00546528 File Offset: 0x00544728
		private void OnDestroy()
		{
			GlobalOperations.UnInitialize();
			GameDataBridge.UnInitialize();
		}

		// Token: 0x04008F7A RID: 36730
		public static bool IsConnected;
	}
}
