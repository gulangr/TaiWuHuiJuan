using System;
using UnityEngine.U2D;

// Token: 0x02000046 RID: 70
public class SpritePackerHandler : ISingletonInit, IDisposable
{
	// Token: 0x0600025E RID: 606 RVA: 0x0000E2D6 File Offset: 0x0000C4D6
	public void Dispose()
	{
		SpriteAtlasManager.atlasRequested -= this.OnRequestPacker;
		SpriteAtlasManager.atlasRegistered -= this.OnAtlasRegistered;
	}

	// Token: 0x0600025F RID: 607 RVA: 0x0000E2FD File Offset: 0x0000C4FD
	public void Init()
	{
		SpriteAtlasManager.atlasRequested += this.OnRequestPacker;
		SpriteAtlasManager.atlasRegistered += this.OnAtlasRegistered;
	}

	// Token: 0x06000260 RID: 608 RVA: 0x0000E324 File Offset: 0x0000C524
	private void OnRequestPacker(string tag, Action<SpriteAtlas> onLoad)
	{
		bool flag = tag.Contains("avatar_") || null == AtlasInfo.Instance;
		if (!flag)
		{
			bool flag2 = !SingletonObject.getInstance<DlcManager>().LoadPacker(tag, onLoad);
			if (flag2)
			{
				AtlasInfo.Instance.LoadPacker(tag, onLoad);
			}
		}
	}

	// Token: 0x06000261 RID: 609 RVA: 0x0000E374 File Offset: 0x0000C574
	private void OnAtlasRegistered(SpriteAtlas atlas)
	{
		bool flag = null != AtlasInfo.Instance;
		if (flag)
		{
			AtlasInfo.Instance.RegisterLoadedPacker(atlas.name, atlas);
		}
	}
}
