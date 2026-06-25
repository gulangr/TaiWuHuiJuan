using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000159 RID: 345
public class CombatPool
{
	// Token: 0x1700021B RID: 539
	// (get) Token: 0x060012F8 RID: 4856 RVA: 0x00073BED File Offset: 0x00071DED
	private Transform Root
	{
		get
		{
			return SingletonObject.getInstance<CombatPoolManager>().transform;
		}
	}

	// Token: 0x060012F9 RID: 4857 RVA: 0x00073BFC File Offset: 0x00071DFC
	public GameObject Get()
	{
		GameObject result = null;
		while (result == null)
		{
			result = ((this._instances.Count > 0) ? this._instances.Pop() : Object.Instantiate<GameObject>(this.Prefab));
		}
		return result;
	}

	// Token: 0x060012FA RID: 4858 RVA: 0x00073C45 File Offset: 0x00071E45
	public void Return(GameObject instance)
	{
		this._instances.Push(instance);
	}

	// Token: 0x060012FB RID: 4859 RVA: 0x00073C58 File Offset: 0x00071E58
	public void EnsureCapacity(int capacity)
	{
		while (this._instances.Count < capacity)
		{
			this._instances.Push(Object.Instantiate<GameObject>(this.Prefab, this.Root));
		}
	}

	// Token: 0x060012FC RID: 4860 RVA: 0x00073C98 File Offset: 0x00071E98
	public void DestroyAll()
	{
		while (this._instances.Count > 0)
		{
			Object.Destroy(this._instances.Pop());
		}
	}

	// Token: 0x04001003 RID: 4099
	private readonly Stack<GameObject> _instances = new Stack<GameObject>();

	// Token: 0x04001004 RID: 4100
	public GameObject Prefab;
}
