using System;
using System.Collections.Generic;
using Config;
using GameData.DLC.FiveLoong;
using GameData.Domains.Character.Display;
using GameData.Domains.Map;
using UnityEngine;

// Token: 0x020003CB RID: 971
public class MapBlockCharEnemy2 : MonoBehaviour
{
	// Token: 0x06003AA3 RID: 15011 RVA: 0x001DC4AE File Offset: 0x001DA6AE
	public void InitNormal(bool canInteract, MapBlockData mapBlock, CharacterDisplayData characterDisplayData, List<LoongInfo> loongInfos)
	{
		this.mapBlockCharNormal.Init(canInteract, mapBlock, characterDisplayData, loongInfos);
		this.mapBlockCharNormal.gameObject.SetActive(true);
		this.mapBlockCharRandomEnemy.gameObject.SetActive(false);
	}

	// Token: 0x06003AA4 RID: 15012 RVA: 0x001DC4E6 File Offset: 0x001DA6E6
	public void InitRandomEnemy(bool canInteract, MapBlockData mapBlock, CharacterItem charConfig, int animalId, sbyte duration)
	{
		this.mapBlockCharRandomEnemy.Init(canInteract, mapBlock, charConfig, animalId, duration);
		this.mapBlockCharRandomEnemy.gameObject.SetActive(true);
		this.mapBlockCharNormal.gameObject.SetActive(false);
	}

	// Token: 0x06003AA5 RID: 15013 RVA: 0x001DC520 File Offset: 0x001DA720
	public void OnHide()
	{
		this.mapBlockCharNormal.OnHide();
		this.mapBlockCharRandomEnemy.OnHide();
	}

	// Token: 0x04002A3C RID: 10812
	[SerializeField]
	private MapBlockCharNormal2 mapBlockCharNormal;

	// Token: 0x04002A3D RID: 10813
	[SerializeField]
	private MapBlockCharRandomEnemy2 mapBlockCharRandomEnemy;
}
