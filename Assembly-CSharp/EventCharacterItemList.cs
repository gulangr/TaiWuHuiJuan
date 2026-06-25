using System;
using Game.Components.ListStyleGeneralScroll.Item;
using UnityEngine;

// Token: 0x02000164 RID: 356
public class EventCharacterItemList : MonoBehaviour
{
	// Token: 0x04001095 RID: 4245
	[SerializeField]
	public GameObject[] items = new GameObject[3];

	// Token: 0x04001096 RID: 4246
	[SerializeField]
	public CardItem[] cardItems = new CardItem[3];

	// Token: 0x04001097 RID: 4247
	[SerializeField]
	public CImage[] cricketJars = new CImage[3];
}
