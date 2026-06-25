using System;
using System.Linq;
using Game.Views.Encyclopedia.Views;
using UnityEngine;

namespace Game.Views.Encyclopedia.Elements
{
	// Token: 0x02000A83 RID: 2691
	public class Element : MonoBehaviour
	{
		// Token: 0x17000E78 RID: 3704
		// (get) Token: 0x060083F7 RID: 33783 RVA: 0x003D5941 File Offset: 0x003D3B41
		public NodeData NodeData
		{
			get
			{
				return this._nodeData;
			}
		}

		// Token: 0x17000E79 RID: 3705
		// (get) Token: 0x060083F8 RID: 33784 RVA: 0x003D5949 File Offset: 0x003D3B49
		public RectTransform RectTransform
		{
			get
			{
				return base.transform as RectTransform;
			}
		}

		// Token: 0x060083F9 RID: 33785 RVA: 0x003D5956 File Offset: 0x003D3B56
		public void Init(NodeData nodeData, NodeData target = null)
		{
			this._nodeData = nodeData;
			this._targetNodeData = target;
			this._cachedSearchingValue = string.Empty;
			this.OnInit();
			this.RefreshShowStatus();
		}

		// Token: 0x060083FA RID: 33786 RVA: 0x003D5980 File Offset: 0x003D3B80
		public virtual void RefreshShowStatus()
		{
			bool flag = !base.gameObject.activeSelf;
			if (flag)
			{
				base.gameObject.SetActive(true);
			}
		}

		// Token: 0x060083FB RID: 33787 RVA: 0x003D59AD File Offset: 0x003D3BAD
		protected virtual void OnInit()
		{
		}

		// Token: 0x060083FC RID: 33788 RVA: 0x003D59B0 File Offset: 0x003D3BB0
		public void Release(bool setActive = true)
		{
			bool flag = setActive && base.gameObject.activeSelf;
			if (flag)
			{
				base.gameObject.SetActive(false);
			}
			this.OnRelease();
		}

		// Token: 0x060083FD RID: 33789 RVA: 0x003D59E7 File Offset: 0x003D3BE7
		protected virtual void OnRelease()
		{
		}

		// Token: 0x060083FE RID: 33790 RVA: 0x003D59EC File Offset: 0x003D3BEC
		protected int GetLayoutPadding()
		{
			EEncyclopediaContentLayout layout = (this.NodeData.ConfigItem.Layout.Length != 0) ? this.NodeData.ConfigItem.Layout.First<EEncyclopediaContentLayout>() : EEncyclopediaContentLayout.None;
			if (!true)
			{
			}
			int result;
			switch (layout)
			{
			case EEncyclopediaContentLayout.None:
				result = 0;
				break;
			case EEncyclopediaContentLayout.NoIdent:
				result = 0;
				break;
			case EEncyclopediaContentLayout.Enum0:
				result = 0;
				break;
			case EEncyclopediaContentLayout.Enum1:
				result = 47;
				break;
			case EEncyclopediaContentLayout.Enum2:
				result = 71;
				break;
			case EEncyclopediaContentLayout.Enum3:
				result = 95;
				break;
			case EEncyclopediaContentLayout.Enum4:
				result = 119;
				break;
			default:
				result = 0;
				break;
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x060083FF RID: 33791 RVA: 0x003D5A7D File Offset: 0x003D3C7D
		public virtual RectTransform GetSearchingRect(SearchIndex searchIndex)
		{
			return base.GetComponent<RectTransform>();
		}

		// Token: 0x0400650F RID: 25871
		[SerializeField]
		[ReadOnly]
		private NodeData _nodeData;

		// Token: 0x04006510 RID: 25872
		protected NodeData _targetNodeData;

		// Token: 0x04006511 RID: 25873
		protected string _cachedSearchingValue = string.Empty;
	}
}
