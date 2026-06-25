using System;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Views.MapBlockCharList
{
	// Token: 0x02000938 RID: 2360
	public class MapBlockCharScroll : MonoBehaviour, IBeginDragHandler, IEventSystemHandler, IDragHandler, IEndDragHandler
	{
		// Token: 0x17000CB9 RID: 3257
		// (get) Token: 0x06006E0D RID: 28173 RVA: 0x0032D619 File Offset: 0x0032B819
		public RectTransform Content
		{
			get
			{
				return this.scrollRect.Content;
			}
		}

		// Token: 0x06006E0E RID: 28174 RVA: 0x0032D628 File Offset: 0x0032B828
		public void Init(IMapBlockCharDataSource parent)
		{
			this._parent = parent;
			parent.CanClick = true;
		}

		// Token: 0x06006E0F RID: 28175 RVA: 0x0032D648 File Offset: 0x0032B848
		public bool SetDataCount(int count, bool forceRefresh = false)
		{
			this.bg.sizeDelta = this.bg.sizeDelta.SetY(Mathf.Min(this.initScrollSize + this.contentSize * (float)count, this.maxScrollSize));
			this.raycast.enabled = (count > this.maxNonScrollCount);
			bool flag = forceRefresh || count <= this.maxNonScrollCount;
			if (flag)
			{
				this.charScroll.ScrollTo(0, 0f);
			}
			this.charScroll.SetDataCount(count);
			this.scrollRect.SetScrollEnable(count > this.maxNonScrollCount);
			return count > this.maxNonScrollCount;
		}

		// Token: 0x06006E10 RID: 28176 RVA: 0x0032D6F9 File Offset: 0x0032B8F9
		private void Awake()
		{
			this.charScroll.OnItemRender += this.OnItemRender;
		}

		// Token: 0x06006E11 RID: 28177 RVA: 0x0032D713 File Offset: 0x0032B913
		private void OnItemRender(int index, GameObject obj)
		{
			IMapBlockCharDataSource parent = this._parent;
			if (parent != null)
			{
				parent.OnItemRender(index, obj);
			}
		}

		// Token: 0x06006E12 RID: 28178 RVA: 0x0032D729 File Offset: 0x0032B929
		public void OnDrag(PointerEventData eventData)
		{
			this.scrollRect.OnDrag(eventData);
		}

		// Token: 0x06006E13 RID: 28179 RVA: 0x0032D738 File Offset: 0x0032B938
		public void OnBeginDrag(PointerEventData eventData)
		{
			this._parent.CanClick = false;
			this.scrollRect.OnBeginDrag(eventData);
		}

		// Token: 0x06006E14 RID: 28180 RVA: 0x0032D755 File Offset: 0x0032B955
		public void OnEndDrag(PointerEventData eventData)
		{
			this.scrollRect.OnEndDrag(eventData);
			this._parent.CanClick = true;
		}

		// Token: 0x040051A7 RID: 20903
		[SerializeField]
		private InfinityScroll charScroll;

		// Token: 0x040051A8 RID: 20904
		[SerializeField]
		private CScrollRect scrollRect;

		// Token: 0x040051A9 RID: 20905
		[Tooltip("禁止scroll的最大物品数，渲染数目多于此数字时将打开scroll功能")]
		[SerializeField]
		private int maxNonScrollCount = 8;

		// Token: 0x040051AA RID: 20906
		[SerializeField]
		private CEmptyGraphic raycast;

		// Token: 0x040051AB RID: 20907
		[SerializeField]
		private RectTransform bg;

		// Token: 0x040051AC RID: 20908
		[SerializeField]
		private float contentSize;

		// Token: 0x040051AD RID: 20909
		[SerializeField]
		private float maxScrollSize;

		// Token: 0x040051AE RID: 20910
		[SerializeField]
		private float initScrollSize;

		// Token: 0x040051AF RID: 20911
		private IMapBlockCharDataSource _parent;

		// Token: 0x040051B0 RID: 20912
		private int _version;
	}
}
