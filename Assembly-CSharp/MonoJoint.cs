using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200007A RID: 122
[ExecuteAlways]
public class MonoJoint : MonoBehaviour
{
	// Token: 0x0600045B RID: 1115 RVA: 0x0001CC7C File Offset: 0x0001AE7C
	public void JointSync()
	{
		this.SyncInternal();
	}

	// Token: 0x0600045C RID: 1116 RVA: 0x0001CC88 File Offset: 0x0001AE88
	public ValueTuple<bool, MonoJoint.ControlInfo> FindGameObjectControlInfo(GameObject target)
	{
		bool flag = this.ControlList == null || this.ControlList.Count <= 0;
		ValueTuple<bool, MonoJoint.ControlInfo> result;
		if (flag)
		{
			result = new ValueTuple<bool, MonoJoint.ControlInfo>(false, default(MonoJoint.ControlInfo));
		}
		else
		{
			foreach (MonoJoint.ControlInfo info in this.ControlList)
			{
				bool flag2 = info.Target.Equals(target);
				if (flag2)
				{
					return new ValueTuple<bool, MonoJoint.ControlInfo>(true, info);
				}
			}
			result = new ValueTuple<bool, MonoJoint.ControlInfo>(false, default(MonoJoint.ControlInfo));
		}
		return result;
	}

	// Token: 0x0600045D RID: 1117 RVA: 0x0001CD40 File Offset: 0x0001AF40
	private void SyncInternal()
	{
		bool flag = this.ControlList == null || null == base.gameObject;
		if (!flag)
		{
			for (int index = 0; index < this.ControlList.Count; index++)
			{
				MonoJoint.ControlInfo info = this.ControlList[index];
				bool flag2 = null == info.Target;
				if (!flag2)
				{
					bool flag3 = this.Modifier != null;
					if (flag3)
					{
						info = this.Modifier(info);
					}
					bool flag4 = info.ActiveControlType == MonoJoint.ActiveControlType.SameWithSelf;
					if (flag4)
					{
						info.Target.SetActive(base.gameObject.activeInHierarchy);
					}
					else
					{
						bool flag5 = info.ActiveControlType == MonoJoint.ActiveControlType.OppositeWithSelf;
						if (flag5)
						{
							info.Target.SetActive(!base.gameObject.activeInHierarchy);
						}
					}
					bool localPositionJoint = info.LocalPositionJoint;
					if (localPositionJoint)
					{
						info.Target.transform.position = base.transform.position;
						Vector3 posVec = info.Target.transform.localPosition;
						posVec += info.LocalPositionOffset;
						info.Target.transform.localPosition = posVec;
					}
					bool rotateJoint = info.RotateJoint;
					if (rotateJoint)
					{
						Vector3 angleVec = base.transform.localEulerAngles + info.EulerAnlgeOffset;
						info.Target.transform.localEulerAngles = angleVec;
					}
					bool localScaleJoint = info.LocalScaleJoint;
					if (localScaleJoint)
					{
						Vector3 scaleVec = base.transform.localScale + info.LocalScaleOffset;
						info.Target.transform.localScale = scaleVec;
					}
				}
			}
		}
	}

	// Token: 0x0600045E RID: 1118 RVA: 0x0001CEF1 File Offset: 0x0001B0F1
	private void OnEnable()
	{
		this.SyncInternal();
	}

	// Token: 0x0600045F RID: 1119 RVA: 0x0001CEFB File Offset: 0x0001B0FB
	private void OnDisable()
	{
		this.SyncInternal();
	}

	// Token: 0x06000460 RID: 1120 RVA: 0x0001CF08 File Offset: 0x0001B108
	private void LateUpdate()
	{
		bool selfUpdate = this.SelfUpdate;
		if (selfUpdate)
		{
			this.SyncInternal();
		}
	}

	// Token: 0x06000461 RID: 1121 RVA: 0x0001CF28 File Offset: 0x0001B128
	private void OnDestroy()
	{
		bool flag = this.ControlList != null;
		if (flag)
		{
			foreach (MonoJoint.ControlInfo child in this.ControlList)
			{
				bool flag2 = !child.KeepWhenDestory;
				if (flag2)
				{
					Object.Destroy(child.Target);
				}
			}
		}
	}

	// Token: 0x040002BF RID: 703
	[NonSerialized]
	public Func<MonoJoint.ControlInfo, MonoJoint.ControlInfo> Modifier;

	// Token: 0x040002C0 RID: 704
	public List<MonoJoint.ControlInfo> ControlList;

	// Token: 0x040002C1 RID: 705
	public bool SelfUpdate;

	// Token: 0x020010F1 RID: 4337
	public enum ActiveControlType
	{
		// Token: 0x040094D8 RID: 38104
		None,
		// Token: 0x040094D9 RID: 38105
		SameWithSelf,
		// Token: 0x040094DA RID: 38106
		OppositeWithSelf
	}

	// Token: 0x020010F2 RID: 4338
	[Serializable]
	public struct ControlInfo
	{
		// Token: 0x040094DB RID: 38107
		public GameObject Target;

		// Token: 0x040094DC RID: 38108
		public bool KeepWhenDestory;

		// Token: 0x040094DD RID: 38109
		public MonoJoint.ActiveControlType ActiveControlType;

		// Token: 0x040094DE RID: 38110
		public bool LocalPositionJoint;

		// Token: 0x040094DF RID: 38111
		public Vector3 LocalPositionOffset;

		// Token: 0x040094E0 RID: 38112
		public bool LocalScaleJoint;

		// Token: 0x040094E1 RID: 38113
		public Vector3 LocalScaleOffset;

		// Token: 0x040094E2 RID: 38114
		public bool RotateJoint;

		// Token: 0x040094E3 RID: 38115
		public Vector3 EulerAnlgeOffset;
	}
}
