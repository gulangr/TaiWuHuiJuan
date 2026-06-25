using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000036 RID: 54
[Serializable]
public class RefersBase : MonoBehaviour
{
	// Token: 0x17000042 RID: 66
	// (get) Token: 0x060001DE RID: 478 RVA: 0x0000BAEF File Offset: 0x00009CEF
	public RectTransform RectTransform
	{
		get
		{
			return base.GetComponent<RectTransform>();
		}
	}

	// Token: 0x060001DF RID: 479 RVA: 0x0000BAF8 File Offset: 0x00009CF8
	public virtual Object LGet(string aName, bool logEnable = true)
	{
		int index = this.Names.IndexOf(aName);
		bool flag = this.Names.CheckIndex(index);
		Object result;
		if (flag)
		{
			result = this.Objects[index];
		}
		else
		{
			if (logEnable)
			{
				GLog.Error(aName + " does not exists in Refers name list!:" + base.name);
			}
			result = null;
		}
		return result;
	}

	// Token: 0x060001E0 RID: 480 RVA: 0x0000BB54 File Offset: 0x00009D54
	public T CGet<T>(string aName) where T : Object
	{
		return this.LGet(aName, true) as T;
	}

	// Token: 0x060001E1 RID: 481 RVA: 0x0000BB78 File Offset: 0x00009D78
	public virtual bool CTryGet<T>(string aName, out T t) where T : Object
	{
		t = default(T);
		int index = this.Names.IndexOf(aName);
		bool flag = this.Names.CheckIndex(index);
		bool result;
		if (flag)
		{
			t = (this.Objects[index] as T);
			result = true;
		}
		else
		{
			result = false;
		}
		return result;
	}

	// Token: 0x060001E2 RID: 482 RVA: 0x0000BBD0 File Offset: 0x00009DD0
	public List<T> CGetList<T>(string aNamePre) where T : Object
	{
		return new List<T>(this.CGetIter<T>(aNamePre));
	}

	// Token: 0x060001E3 RID: 483 RVA: 0x0000BBEE File Offset: 0x00009DEE
	public virtual IEnumerable<T> CGetIter<T>(string aNamePre) where T : Object
	{
		int num;
		for (int i = 0; i < this.Names.Count; i = num)
		{
			bool flag = this.Names[i].StartsWith(aNamePre);
			if (flag)
			{
				T obj = this.CGet<T>(this.Names[i]);
				bool flag2 = null != obj;
				if (flag2)
				{
					yield return obj;
				}
				obj = default(T);
			}
			num = i + 1;
		}
		yield break;
	}

	// Token: 0x060001E4 RID: 484 RVA: 0x0000BC08 File Offset: 0x00009E08
	public void AddMono(Object obj, string key = "")
	{
		bool flag = this.Objects == null;
		if (flag)
		{
			this.Objects = new List<Object>();
			this.Names = new List<string>();
		}
		bool flag2 = this.Objects.Contains(obj);
		if (!flag2)
		{
			int max = Mathf.Max(this.Objects.Count, this.Names.Count);
			for (int i = 0; i < max; i++)
			{
				bool flag3 = i >= this.Objects.Count;
				if (flag3)
				{
					this.Objects.Add(null);
				}
				bool flag4 = i >= this.Names.Count;
				if (flag4)
				{
					this.Names.Add(string.Empty);
				}
			}
			this.Objects.Add(obj);
			bool flag5 = !string.IsNullOrEmpty(key);
			if (flag5)
			{
				this.Names.Add(key);
			}
			else
			{
				bool flag6 = !this.Names.Contains(obj.name);
				if (flag6)
				{
					this.Names.Add(obj.name);
				}
				else
				{
					string typeStr = obj.GetType().ToString();
					typeStr = typeStr.Substring(typeStr.LastIndexOf('.') + 1);
					string baseKey = obj.name + "_" + typeStr;
					int nextIndex = 1;
					key = baseKey;
					while (this.Names.Contains(key))
					{
						key = string.Format("{0}_{1}", baseKey, nextIndex++);
					}
					this.Names.Add(key);
				}
			}
		}
	}

	// Token: 0x17000043 RID: 67
	// (get) Token: 0x060001E5 RID: 485 RVA: 0x0000BDAB File Offset: 0x00009FAB
	public int Length
	{
		get
		{
			return this.Objects.Count;
		}
	}

	// Token: 0x060001E6 RID: 486 RVA: 0x0000BDB8 File Offset: 0x00009FB8
	protected void DelayCall(Action action, float delay)
	{
		bool activeInHierarchy = base.gameObject.activeInHierarchy;
		if (activeInHierarchy)
		{
			base.StartCoroutine(this.CoDelayCall(action, delay));
		}
	}

	// Token: 0x060001E7 RID: 487 RVA: 0x0000BDE4 File Offset: 0x00009FE4
	protected void DelayCallRealTime(Action action, float delay)
	{
		bool activeInHierarchy = base.gameObject.activeInHierarchy;
		if (activeInHierarchy)
		{
			base.StartCoroutine(this.CoDelayCallRealTime(action, delay));
		}
	}

	// Token: 0x060001E8 RID: 488 RVA: 0x0000BE10 File Offset: 0x0000A010
	protected Coroutine DelayCallReturnCoroutine(Action action, float delay)
	{
		bool activeInHierarchy = base.gameObject.activeInHierarchy;
		Coroutine result;
		if (activeInHierarchy)
		{
			result = base.StartCoroutine(this.CoDelayCall(action, delay));
		}
		else
		{
			result = null;
		}
		return result;
	}

	// Token: 0x060001E9 RID: 489 RVA: 0x0000BE44 File Offset: 0x0000A044
	protected void DelayFrameCall(Action action, uint frame)
	{
		bool activeInHierarchy = base.gameObject.activeInHierarchy;
		if (activeInHierarchy)
		{
			base.StartCoroutine(this.CoDelayFrameCall(action, frame));
		}
	}

	// Token: 0x060001EA RID: 490 RVA: 0x0000BE70 File Offset: 0x0000A070
	private IEnumerator CoDelayCall(Action action, float delay)
	{
		yield return new WaitForSeconds(delay);
		if (action != null)
		{
			action();
		}
		yield break;
	}

	// Token: 0x060001EB RID: 491 RVA: 0x0000BE8D File Offset: 0x0000A08D
	private IEnumerator CoDelayCallRealTime(Action action, float delay)
	{
		yield return new WaitForSecondsRealtime(delay);
		if (action != null)
		{
			action();
		}
		yield break;
	}

	// Token: 0x060001EC RID: 492 RVA: 0x0000BEAA File Offset: 0x0000A0AA
	private IEnumerator CoDelayFrameCall(Action action, uint frame)
	{
		int i = 0;
		while ((long)i < (long)((ulong)frame))
		{
			yield return null;
			int num = i;
			i = num + 1;
		}
		if (action != null)
		{
			action();
		}
		yield break;
	}

	// Token: 0x040000F1 RID: 241
	[FormerlySerializedAs("Monos")]
	public List<Object> Objects;

	// Token: 0x040000F2 RID: 242
	public List<string> Names;

	// Token: 0x040000F3 RID: 243
	public float UserFloat;

	// Token: 0x040000F4 RID: 244
	public int UserInt;

	// Token: 0x040000F5 RID: 245
	public string UserString;

	// Token: 0x040000F6 RID: 246
	public bool UserBool;

	// Token: 0x040000F7 RID: 247
	public object UserObject;
}
