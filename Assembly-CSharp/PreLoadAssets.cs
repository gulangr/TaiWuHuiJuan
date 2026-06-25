using System;
using Spine.Unity;
using UnityEngine;

// Token: 0x0200014A RID: 330
public class PreLoadAssets : MonoBehaviour
{
	// Token: 0x1700020F RID: 527
	// (get) Token: 0x0600127E RID: 4734 RVA: 0x00070D44 File Offset: 0x0006EF44
	// (set) Token: 0x0600127D RID: 4733 RVA: 0x00070D3C File Offset: 0x0006EF3C
	public static PreLoadAssets Instance { get; private set; }

	// Token: 0x0600127F RID: 4735 RVA: 0x00070D4B File Offset: 0x0006EF4B
	private void Awake()
	{
		PreLoadAssets.Instance = this;
	}

	// Token: 0x04000F9A RID: 3994
	[HideInInspector]
	public SkeletonDataAsset BaseCombatAnimations;

	// Token: 0x04000F9B RID: 3995
	[HideInInspector]
	public SkeletonDataAsset TravelAnimations;
}
