using System;
using GameData.Domains.Map;
using UnityEngine;

// Token: 0x020003E5 RID: 997
public abstract class MapElementBase : MonoBehaviour, IMapElement
{
	// Token: 0x1700060C RID: 1548
	// (get) Token: 0x06003BC2 RID: 15298 RVA: 0x001E3E99 File Offset: 0x001E2099
	protected static WorldMapModel MapModel
	{
		get
		{
			return SingletonObject.getInstance<WorldMapModel>();
		}
	}

	// Token: 0x1700060D RID: 1549
	// (get) Token: 0x06003BC3 RID: 15299 RVA: 0x001E3EA0 File Offset: 0x001E20A0
	protected static GlobalSettings GlobalSettings
	{
		get
		{
			return SingletonObject.getInstance<GlobalSettings>();
		}
	}

	// Token: 0x1700060E RID: 1550
	// (get) Token: 0x06003BC4 RID: 15300 RVA: 0x001E3EA7 File Offset: 0x001E20A7
	protected static bool IsHideCharacterSet
	{
		get
		{
			return SingletonObject.getInstance<WorldMapModel>().IsHideCharacterSet();
		}
	}

	// Token: 0x1700060F RID: 1551
	// (get) Token: 0x06003BC5 RID: 15301 RVA: 0x001E3EB3 File Offset: 0x001E20B3
	protected static bool IsMapMoving
	{
		get
		{
			return MapElementBase.MapModel.TaiwuMoveState > WorldMapModel.MoveState.Idle;
		}
	}

	// Token: 0x17000610 RID: 1552
	// (get) Token: 0x06003BC6 RID: 15302 RVA: 0x001E3EC2 File Offset: 0x001E20C2
	protected MapBlockData BlockData
	{
		get
		{
			return MapElementBase.MapModel.GetBlockData(this.BlockLocation);
		}
	}

	// Token: 0x17000611 RID: 1553
	// (get) Token: 0x06003BC7 RID: 15303 RVA: 0x001E3ED4 File Offset: 0x001E20D4
	// (set) Token: 0x06003BC8 RID: 15304 RVA: 0x001E3EDC File Offset: 0x001E20DC
	private protected Location BlockLocation { protected get; private set; }

	// Token: 0x17000612 RID: 1554
	// (get) Token: 0x06003BC9 RID: 15305 RVA: 0x001E3EE5 File Offset: 0x001E20E5
	public virtual Vector2 MapOffset
	{
		get
		{
			return Vector2.zero;
		}
	}

	// Token: 0x06003BCA RID: 15306 RVA: 0x001E3EEC File Offset: 0x001E20EC
	public void BindAsyncMethod()
	{
		base.gameObject.SetActive(this.AutoSetActive);
		this.Dispatcher = DispatcherUtils.RegisterDispatcher();
		this.OnCreate();
	}

	// Token: 0x06003BCB RID: 15307 RVA: 0x001E3F13 File Offset: 0x001E2113
	public void BindPosGenerator(MapElementPosGenerator posGenerator)
	{
		this.PosGenerator = posGenerator;
	}

	// Token: 0x17000613 RID: 1555
	// (get) Token: 0x06003BCC RID: 15308
	public abstract EMapLayer Layer { get; }

	// Token: 0x06003BCD RID: 15309
	public abstract void Scale(float wheel);

	// Token: 0x06003BCE RID: 15310 RVA: 0x001E3F1D File Offset: 0x001E211D
	public void Refresh(Location location)
	{
		this.BlockLocation = location;
		this.OnRefresh();
	}

	// Token: 0x06003BCF RID: 15311 RVA: 0x001E3F2F File Offset: 0x001E212F
	public void Collect()
	{
		this.OnCollect();
		DispatcherUtils.UnregisterDispatcher(this.Dispatcher);
		this.Dispatcher = null;
		this.PosGenerator = null;
	}

	// Token: 0x17000614 RID: 1556
	// (get) Token: 0x06003BD0 RID: 15312 RVA: 0x001E3F53 File Offset: 0x001E2153
	protected virtual bool AutoSetActive
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06003BD1 RID: 15313 RVA: 0x001E3F56 File Offset: 0x001E2156
	protected virtual void OnCreate()
	{
	}

	// Token: 0x06003BD2 RID: 15314
	protected abstract void OnRefresh();

	// Token: 0x06003BD3 RID: 15315
	protected abstract void OnCollect();

	// Token: 0x06003BD4 RID: 15316 RVA: 0x001E3F59 File Offset: 0x001E2159
	protected void ScaleReverse(float wheel)
	{
		MapElementBase.ScaleReverse(base.transform, wheel);
	}

	// Token: 0x06003BD5 RID: 15317 RVA: 0x001E3F68 File Offset: 0x001E2168
	protected static void ScaleReverse(Transform t, float wheel)
	{
		t.localScale = Vector3.one / wheel;
	}

	// Token: 0x04002B14 RID: 11028
	protected DispatcherInstance Dispatcher;

	// Token: 0x04002B15 RID: 11029
	protected MapElementPosGenerator PosGenerator;

	// Token: 0x04002B16 RID: 11030
	public bool UserBool;
}
