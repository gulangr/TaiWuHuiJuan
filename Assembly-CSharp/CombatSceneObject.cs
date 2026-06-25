using System;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

// Token: 0x0200015D RID: 349
[ExecuteAlways]
public class CombatSceneObject : MonoBehaviour
{
	// Token: 0x1700021F RID: 543
	// (get) Token: 0x06001330 RID: 4912 RVA: 0x0007646E File Offset: 0x0007466E
	// (set) Token: 0x06001331 RID: 4913 RVA: 0x00076476 File Offset: 0x00074676
	public int GroupId
	{
		get
		{
			return this.groupId;
		}
		set
		{
			this.groupId = Mathf.Max(0, value);
		}
	}

	// Token: 0x17000220 RID: 544
	// (get) Token: 0x06001332 RID: 4914 RVA: 0x00076485 File Offset: 0x00074685
	// (set) Token: 0x06001333 RID: 4915 RVA: 0x0007648D File Offset: 0x0007468D
	public bool UseAppearanceProbability
	{
		get
		{
			return this.useAppearanceProbability;
		}
		set
		{
			this.useAppearanceProbability = value;
		}
	}

	// Token: 0x17000221 RID: 545
	// (get) Token: 0x06001334 RID: 4916 RVA: 0x00076496 File Offset: 0x00074696
	// (set) Token: 0x06001335 RID: 4917 RVA: 0x000764A3 File Offset: 0x000746A3
	public float AppearanceProbability
	{
		get
		{
			return Mathf.Clamp01(this.appearanceProbability);
		}
		set
		{
			this.appearanceProbability = Mathf.Clamp01(value);
		}
	}

	// Token: 0x17000222 RID: 546
	// (get) Token: 0x06001336 RID: 4918 RVA: 0x000764B1 File Offset: 0x000746B1
	// (set) Token: 0x06001337 RID: 4919 RVA: 0x000764B9 File Offset: 0x000746B9
	public bool UseMonthRestriction
	{
		get
		{
			return this.useMonthRestriction;
		}
		set
		{
			this.useMonthRestriction = value;
		}
	}

	// Token: 0x17000223 RID: 547
	// (get) Token: 0x06001338 RID: 4920 RVA: 0x000764C2 File Offset: 0x000746C2
	public IReadOnlyList<int> ActiveMonths
	{
		get
		{
			return this.activeMonths;
		}
	}

	// Token: 0x06001339 RID: 4921 RVA: 0x000764CA File Offset: 0x000746CA
	public void Awake()
	{
		this.RectTrans = base.GetComponent<RectTransform>();
		this.EnsureActiveMonthsInitialized();
	}

	// Token: 0x0600133A RID: 4922 RVA: 0x000764E0 File Offset: 0x000746E0
	private void EnsureActiveMonthsInitialized()
	{
		bool flag = this.activeMonths == null;
		if (flag)
		{
			this.activeMonths = new List<int>();
		}
		this.SanitizeActiveMonths();
	}

	// Token: 0x0600133B RID: 4923 RVA: 0x00076510 File Offset: 0x00074710
	private void SanitizeActiveMonths()
	{
		bool flag = this.activeMonths == null;
		if (flag)
		{
			this.activeMonths = new List<int>();
		}
		else
		{
			HashSet<int> unique = new HashSet<int>();
			for (int i = this.activeMonths.Count - 1; i >= 0; i--)
			{
				int month = this.activeMonths[i];
				bool flag2 = month < 1 || month > 12 || !unique.Add(month);
				if (flag2)
				{
					this.activeMonths.RemoveAt(i);
				}
			}
			this.activeMonths.Sort();
		}
	}

	// Token: 0x0600133C RID: 4924 RVA: 0x000765A8 File Offset: 0x000747A8
	public void SetActiveMonths(IEnumerable<int> months)
	{
		bool flag = this.activeMonths == null;
		if (flag)
		{
			this.activeMonths = new List<int>();
		}
		this.activeMonths.Clear();
		bool flag2 = months == null;
		if (!flag2)
		{
			HashSet<int> unique = new HashSet<int>();
			foreach (int month in months)
			{
				bool flag3 = month < 1 || month > 12;
				if (!flag3)
				{
					bool flag4 = unique.Add(month);
					if (flag4)
					{
						this.activeMonths.Add(month);
					}
				}
			}
			this.activeMonths.Sort();
		}
	}

	// Token: 0x0600133D RID: 4925 RVA: 0x00076664 File Offset: 0x00074864
	public bool IsMonthAllowed(int month)
	{
		bool flag = !this.useMonthRestriction;
		return flag || (this.activeMonths != null && this.activeMonths.Contains(month));
	}

	// Token: 0x17000224 RID: 548
	// (get) Token: 0x0600133E RID: 4926 RVA: 0x0007669E File Offset: 0x0007489E
	public bool HasProbabilityOverride
	{
		get
		{
			return this.useAppearanceProbability;
		}
	}

	// Token: 0x0600133F RID: 4927 RVA: 0x000766A6 File Offset: 0x000748A6
	private void OnValidate()
	{
		this.appearanceProbability = Mathf.Clamp01(this.appearanceProbability);
		this.groupId = Mathf.Max(0, this.groupId);
		this.EnsureActiveMonthsInitialized();
	}

	// Token: 0x06001340 RID: 4928 RVA: 0x000766D4 File Offset: 0x000748D4
	public void DoRandomFlip()
	{
		bool randomFlip = this.RandomFlip;
		if (randomFlip)
		{
			SkeletonGraphic animGraphic = base.GetComponent<SkeletonGraphic>();
			bool flag = null != animGraphic;
			if (flag)
			{
				animGraphic.initialFlipX = (Random.Range(0, 2) == 0);
			}
			else
			{
				Vector3 scale = this.RectTrans.localScale;
				scale.x = (float)((Random.Range(0, 2) == 0) ? 1 : -1);
				this.RectTrans.localScale = scale;
			}
		}
	}

	// Token: 0x06001341 RID: 4929 RVA: 0x00076744 File Offset: 0x00074944
	public void Scroll(float xDelta)
	{
		Vector2 anchorPos = this.RectTrans.anchoredPosition;
		anchorPos.x += xDelta;
		this.RectTrans.anchoredPosition = anchorPos;
	}

	// Token: 0x06001342 RID: 4930 RVA: 0x00076778 File Offset: 0x00074978
	public void Reset()
	{
		bool flag = null == this.RectTrans;
		if (flag)
		{
			this.Awake();
		}
		bool flag2 = float.IsNaN(this.RecordX);
		if (!flag2)
		{
			Vector2 anchorPos = this.RectTrans.anchoredPosition;
			anchorPos.x = this.RecordX;
			this.RectTrans.anchoredPosition = anchorPos;
		}
	}

	// Token: 0x04001024 RID: 4132
	[HideInInspector]
	public bool RandomFlip;

	// Token: 0x04001025 RID: 4133
	public RectTransform RectTrans;

	// Token: 0x04001026 RID: 4134
	public float RecordX = float.NaN;

	// Token: 0x04001027 RID: 4135
	[SerializeField]
	private int groupId;

	// Token: 0x04001028 RID: 4136
	[SerializeField]
	private bool useAppearanceProbability;

	// Token: 0x04001029 RID: 4137
	[SerializeField]
	[Range(0f, 1f)]
	private float appearanceProbability = 1f;

	// Token: 0x0400102A RID: 4138
	[SerializeField]
	private bool useMonthRestriction;

	// Token: 0x0400102B RID: 4139
	[SerializeField]
	private List<int> activeMonths = new List<int>();
}
