using System;
using System.Collections.Generic;
using Config;
using GameData.DLC.FiveLoong;
using GameData.Domains.Character.Display;
using GameData.Domains.Map;
using UnityEngine;

// Token: 0x020003CA RID: 970
public class MapBlockCharEnemy : MonoBehaviour
{
	// Token: 0x06003A9F RID: 15007 RVA: 0x001DC418 File Offset: 0x001DA618
	public void InitNormal(bool canInteract, MapBlockData mapBlock, CharacterDisplayData characterDisplayData, List<LoongInfo> loongInfos)
	{
		this.mapBlockCharNormal.Init(canInteract, mapBlock, characterDisplayData, loongInfos);
		this.mapBlockCharNormal.gameObject.SetActive(true);
		this.mapBlockCharRandomEnemy.gameObject.SetActive(false);
	}

	// Token: 0x06003AA0 RID: 15008 RVA: 0x001DC450 File Offset: 0x001DA650
	public void InitRandomEnemy(bool canInteract, MapBlockData mapBlock, CharacterItem charConfig, int animalId, sbyte duration)
	{
		this.mapBlockCharRandomEnemy.Init(canInteract, mapBlock, charConfig, animalId, duration);
		this.mapBlockCharRandomEnemy.gameObject.SetActive(true);
		this.mapBlockCharNormal.gameObject.SetActive(false);
	}

	// Token: 0x06003AA1 RID: 15009 RVA: 0x001DC48A File Offset: 0x001DA68A
	public void OnHide()
	{
		this.mapBlockCharNormal.OnHide();
		this.mapBlockCharRandomEnemy.OnHide();
	}

	// Token: 0x04002A3A RID: 10810
	[SerializeField]
	private MapBlockCharNormal mapBlockCharNormal;

	// Token: 0x04002A3B RID: 10811
	[SerializeField]
	private MapBlockCharRandomEnemy mapBlockCharRandomEnemy;
}
