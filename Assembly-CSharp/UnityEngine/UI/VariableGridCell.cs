using System;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
	// Token: 0x02000FAD RID: 4013
	[AddComponentMenu("Layout/Variable Grid Layout Group Cell", 140)]
	[RequireComponent(typeof(RectTransform))]
	[ExecuteInEditMode]
	public class VariableGridCell : UIBehaviour
	{
		// Token: 0x170014DC RID: 5340
		// (get) Token: 0x0600B884 RID: 47236 RVA: 0x005424AC File Offset: 0x005406AC
		// (set) Token: 0x0600B885 RID: 47237 RVA: 0x005424C4 File Offset: 0x005406C4
		public virtual bool overrideForceExpandWidth
		{
			get
			{
				return this.m_OverrideForceExpandWidth;
			}
			set
			{
				bool flag = value != this.m_OverrideForceExpandWidth;
				if (flag)
				{
					this.m_OverrideForceExpandWidth = value;
					this.SetDirty();
				}
			}
		}

		// Token: 0x170014DD RID: 5341
		// (get) Token: 0x0600B886 RID: 47238 RVA: 0x005424F4 File Offset: 0x005406F4
		// (set) Token: 0x0600B887 RID: 47239 RVA: 0x0054250C File Offset: 0x0054070C
		public virtual bool forceExpandWidth
		{
			get
			{
				return this.m_ForceExpandWidth;
			}
			set
			{
				bool flag = value != this.m_ForceExpandWidth;
				if (flag)
				{
					this.m_ForceExpandWidth = value;
					this.SetDirty();
				}
			}
		}

		// Token: 0x170014DE RID: 5342
		// (get) Token: 0x0600B888 RID: 47240 RVA: 0x0054253C File Offset: 0x0054073C
		// (set) Token: 0x0600B889 RID: 47241 RVA: 0x00542554 File Offset: 0x00540754
		public virtual bool overrideForceExpandHeight
		{
			get
			{
				return this.m_OverrideForceExpandHeight;
			}
			set
			{
				bool flag = value != this.m_OverrideForceExpandHeight;
				if (flag)
				{
					this.m_OverrideForceExpandHeight = value;
					this.SetDirty();
				}
			}
		}

		// Token: 0x170014DF RID: 5343
		// (get) Token: 0x0600B88A RID: 47242 RVA: 0x00542584 File Offset: 0x00540784
		// (set) Token: 0x0600B88B RID: 47243 RVA: 0x0054259C File Offset: 0x0054079C
		public virtual bool forceExpandHeight
		{
			get
			{
				return this.m_ForceExpandHeight;
			}
			set
			{
				bool flag = value != this.m_ForceExpandHeight;
				if (flag)
				{
					this.m_ForceExpandHeight = value;
					this.SetDirty();
				}
			}
		}

		// Token: 0x170014E0 RID: 5344
		// (get) Token: 0x0600B88C RID: 47244 RVA: 0x005425CC File Offset: 0x005407CC
		// (set) Token: 0x0600B88D RID: 47245 RVA: 0x005425E4 File Offset: 0x005407E4
		public virtual bool overrideCellAlignment
		{
			get
			{
				return this.m_OverrideCellAlignment;
			}
			set
			{
				bool flag = value != this.m_OverrideCellAlignment;
				if (flag)
				{
					this.m_OverrideCellAlignment = value;
					this.SetDirty();
				}
			}
		}

		// Token: 0x170014E1 RID: 5345
		// (get) Token: 0x0600B88E RID: 47246 RVA: 0x00542614 File Offset: 0x00540814
		// (set) Token: 0x0600B88F RID: 47247 RVA: 0x0054262C File Offset: 0x0054082C
		public virtual TextAnchor cellAlignment
		{
			get
			{
				return this.m_CellAlignment;
			}
			set
			{
				bool flag = value != this.m_CellAlignment;
				if (flag)
				{
					this.m_CellAlignment = value;
					this.SetDirty();
				}
			}
		}

		// Token: 0x0600B890 RID: 47248 RVA: 0x0054265A File Offset: 0x0054085A
		protected VariableGridCell()
		{
		}

		// Token: 0x0600B891 RID: 47249 RVA: 0x0054268E File Offset: 0x0054088E
		protected override void OnEnable()
		{
			base.OnEnable();
			this.SetDirty();
		}

		// Token: 0x0600B892 RID: 47250 RVA: 0x0054269F File Offset: 0x0054089F
		protected override void OnTransformParentChanged()
		{
			this.SetDirty();
		}

		// Token: 0x0600B893 RID: 47251 RVA: 0x005426A9 File Offset: 0x005408A9
		protected override void OnDisable()
		{
			this.SetDirty();
			base.OnDisable();
		}

		// Token: 0x0600B894 RID: 47252 RVA: 0x005426BA File Offset: 0x005408BA
		protected override void OnDidApplyAnimationProperties()
		{
			this.SetDirty();
		}

		// Token: 0x0600B895 RID: 47253 RVA: 0x005426C4 File Offset: 0x005408C4
		protected override void OnBeforeTransformParentChanged()
		{
			this.SetDirty();
		}

		// Token: 0x0600B896 RID: 47254 RVA: 0x005426D0 File Offset: 0x005408D0
		protected void SetDirty()
		{
			bool flag = !this.IsActive();
			if (!flag)
			{
				LayoutRebuilder.MarkLayoutForRebuild(base.transform as RectTransform);
			}
		}

		// Token: 0x04008F23 RID: 36643
		[SerializeField]
		private bool m_OverrideForceExpandWidth = false;

		// Token: 0x04008F24 RID: 36644
		[SerializeField]
		private bool m_ForceExpandWidth = false;

		// Token: 0x04008F25 RID: 36645
		[SerializeField]
		private bool m_OverrideForceExpandHeight = false;

		// Token: 0x04008F26 RID: 36646
		[SerializeField]
		private bool m_ForceExpandHeight = false;

		// Token: 0x04008F27 RID: 36647
		[SerializeField]
		private bool m_OverrideCellAlignment = false;

		// Token: 0x04008F28 RID: 36648
		[SerializeField]
		private TextAnchor m_CellAlignment = TextAnchor.UpperLeft;
	}
}
