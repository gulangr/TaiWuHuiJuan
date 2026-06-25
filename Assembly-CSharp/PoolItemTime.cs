using System;
using UnityEngine;

// Token: 0x0200002E RID: 46
public class PoolItemTime
{
	// Token: 0x1700003F RID: 63
	// (get) Token: 0x060001A6 RID: 422 RVA: 0x0000AEDF File Offset: 0x000090DF
	// (set) Token: 0x060001A7 RID: 423 RVA: 0x0000AEE7 File Offset: 0x000090E7
	public bool DestroyStatus { get; private set; }

	// Token: 0x060001A8 RID: 424 RVA: 0x0000AEF0 File Offset: 0x000090F0
	public PoolItemTime(GameObject gameObject)
	{
		this.gameObject = gameObject;
		this.DestroyStatus = false;
	}

	// Token: 0x060001A9 RID: 425 RVA: 0x0000AF0C File Offset: 0x0000910C
	public GameObject Active()
	{
		this.gameObject.SetActive(true);
		this.DestroyStatus = false;
		return this.gameObject;
	}

	// Token: 0x060001AA RID: 426 RVA: 0x0000AF39 File Offset: 0x00009139
	public void Destroy()
	{
		this.gameObject.SetActive(false);
		this.DestroyStatus = true;
		this.expireTime = Time.time;
	}

	// Token: 0x060001AB RID: 427 RVA: 0x0000AF5C File Offset: 0x0000915C
	public bool IsExpire()
	{
		bool flag = !this.DestroyStatus;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			bool flag2 = Time.time - this.expireTime >= 10f;
			result = flag2;
		}
		return result;
	}

	// Token: 0x060001AC RID: 428 RVA: 0x0000AFA0 File Offset: 0x000091A0
	public bool IsValid()
	{
		return null != this.gameObject;
	}

	// Token: 0x040000C8 RID: 200
	public readonly GameObject gameObject;

	// Token: 0x040000C9 RID: 201
	public float expireTime;
}
