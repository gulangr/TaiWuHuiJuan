using System;
using Game.Components.Avatar;
using TMPro;
using UnityEngine;

// Token: 0x02000406 RID: 1030
public class ProfessionMaskTeammateRise : MonoBehaviour
{
	// Token: 0x17000649 RID: 1609
	// (get) Token: 0x06003D50 RID: 15696 RVA: 0x001ED0E4 File Offset: 0x001EB2E4
	public Game.Components.Avatar.Avatar Avatar
	{
		get
		{
			return this.avatar;
		}
	}

	// Token: 0x1700064A RID: 1610
	// (get) Token: 0x06003D51 RID: 15697 RVA: 0x001ED0EC File Offset: 0x001EB2EC
	public TextMeshProUGUI NameLabel
	{
		get
		{
			return this.nameLabel;
		}
	}

	// Token: 0x1700064B RID: 1611
	// (get) Token: 0x06003D52 RID: 15698 RVA: 0x001ED0F4 File Offset: 0x001EB2F4
	public CanvasGroup Root
	{
		get
		{
			return this.root;
		}
	}

	// Token: 0x04002C1C RID: 11292
	[SerializeField]
	private Game.Components.Avatar.Avatar avatar;

	// Token: 0x04002C1D RID: 11293
	[SerializeField]
	private TextMeshProUGUI nameLabel;

	// Token: 0x04002C1E RID: 11294
	[SerializeField]
	private CanvasGroup root;
}
