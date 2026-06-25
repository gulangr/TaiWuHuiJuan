using System;
using UnityEngine;

// Token: 0x02000093 RID: 147
public class TargetScaleAlign : MonoBehaviour
{
	// Token: 0x17000086 RID: 134
	// (get) Token: 0x0600052F RID: 1327 RVA: 0x000236CB File Offset: 0x000218CB
	// (set) Token: 0x06000530 RID: 1328 RVA: 0x000236D4 File Offset: 0x000218D4
	public Transform Target
	{
		get
		{
			return this.target;
		}
		set
		{
			bool flag = this.target == value;
			if (!flag)
			{
				this.target = value;
				bool flag2 = null == value;
				if (flag2)
				{
					this.UnregisterUpdate();
				}
				else
				{
					bool flag3 = this.isApplyOnUpdate;
					if (flag3)
					{
						this.RegisterUpdate();
					}
				}
				bool flag4 = this.isApplyOnEnable && base.enabled;
				if (flag4)
				{
					this.Apply();
				}
			}
		}
	}

	// Token: 0x17000087 RID: 135
	// (get) Token: 0x06000531 RID: 1329 RVA: 0x00023743 File Offset: 0x00021943
	// (set) Token: 0x06000532 RID: 1330 RVA: 0x0002374C File Offset: 0x0002194C
	public bool IsApplyOnUpdate
	{
		get
		{
			return this.isApplyOnUpdate;
		}
		set
		{
			bool flag = this.isApplyOnUpdate == value;
			if (!flag)
			{
				this.isApplyOnUpdate = value;
				bool flag2 = this.isApplyOnUpdate;
				if (flag2)
				{
					this.RegisterUpdate();
				}
				else
				{
					this.UnregisterUpdate();
				}
			}
		}
	}

	// Token: 0x06000533 RID: 1331 RVA: 0x00023790 File Offset: 0x00021990
	private void OnEnable()
	{
		bool flag = null == this.target;
		if (!flag)
		{
			bool flag2 = this.isApplyOnUpdate;
			if (flag2)
			{
				this.RegisterUpdate();
			}
			this.Apply();
		}
	}

	// Token: 0x06000534 RID: 1332 RVA: 0x000237CC File Offset: 0x000219CC
	private void OnDisable()
	{
		bool flag = this.isApplyOnUpdate;
		if (flag)
		{
			this.UnregisterUpdate();
		}
	}

	// Token: 0x06000535 RID: 1333 RVA: 0x000237F0 File Offset: 0x000219F0
	public void Apply()
	{
		float targetScale = this.target.localScale.x;
		bool flag = this.isRemap;
		if (flag)
		{
			targetScale = TargetScaleAlign.GetAlignScale(targetScale, this.remapTargetMinMax, this.remapMinMax, this.isClamp);
		}
		base.transform.localScale = new Vector3(targetScale, targetScale, targetScale);
	}

	// Token: 0x06000536 RID: 1334 RVA: 0x00023848 File Offset: 0x00021A48
	private void RegisterUpdate()
	{
		bool flag = !this._isRegisterUpdate && base.enabled && base.gameObject.activeInHierarchy;
		if (flag)
		{
			SingletonObject.getInstance<YieldHelper>().StartUpdate(new Action<float>(this.UpdateCall));
			this._isRegisterUpdate = true;
		}
	}

	// Token: 0x06000537 RID: 1335 RVA: 0x00023898 File Offset: 0x00021A98
	private void UnregisterUpdate()
	{
		bool isRegisterUpdate = this._isRegisterUpdate;
		if (isRegisterUpdate)
		{
			YieldHelper yhelper = SingletonObject.getInstance<YieldHelper>();
			bool flag = yhelper != null;
			if (flag)
			{
				yhelper.StopUpdate(new Action<float>(this.UpdateCall));
			}
			this._isRegisterUpdate = false;
		}
	}

	// Token: 0x06000538 RID: 1336 RVA: 0x000238DD File Offset: 0x00021ADD
	private void UpdateCall(float deltaTime)
	{
		this.Apply();
	}

	// Token: 0x06000539 RID: 1337 RVA: 0x000238E8 File Offset: 0x00021AE8
	public static float GetAlignScale(float targetScale, Vector2 remapTargetMinMax, Vector2 remapMinMax, bool isClamp)
	{
		return TargetScaleAlign.GetAlignScale(targetScale, remapTargetMinMax.x, remapTargetMinMax.y, remapMinMax.x, remapMinMax.y, isClamp);
	}

	// Token: 0x0600053A RID: 1338 RVA: 0x0002391C File Offset: 0x00021B1C
	public static float GetAlignScale(float value, float from1, float to1, float from2, float to2, bool isClamp = true)
	{
		bool flag = to1 - from1 == 0f;
		float result;
		if (flag)
		{
			result = from2;
		}
		else
		{
			if (isClamp)
			{
				value = Mathf.Clamp(value, from1, to1);
			}
			result = (value - from1) / (to1 - from1) * (to2 - from2) + from2;
		}
		return result;
	}

	// Token: 0x04000437 RID: 1079
	[SerializeField]
	private Transform target;

	// Token: 0x04000438 RID: 1080
	[SerializeField]
	private bool isRemap = false;

	// Token: 0x04000439 RID: 1081
	[Tooltip("当isRemap为true时，输入的目标scale范围")]
	[SerializeField]
	private Vector2 remapTargetMinMax = new Vector2(0f, 1f);

	// Token: 0x0400043A RID: 1082
	[Tooltip("当isRemap为true时，输出的scale范围")]
	[SerializeField]
	private Vector2 remapMinMax = new Vector2(0.5f, 1f);

	// Token: 0x0400043B RID: 1083
	[SerializeField]
	private bool isClamp = false;

	// Token: 0x0400043C RID: 1084
	[Space]
	[SerializeField]
	private bool isApplyOnEnable = true;

	// Token: 0x0400043D RID: 1085
	[SerializeField]
	private bool isApplyOnUpdate = false;

	// Token: 0x0400043E RID: 1086
	private bool _isRegisterUpdate = false;
}
