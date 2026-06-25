using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace FrameWork
{
	// Token: 0x02000FE9 RID: 4073
	[RequireComponent(typeof(VerticalLayoutGroup))]
	public class MultiHorizontalLayoutGroup : MonoBehaviour
	{
		// Token: 0x170014F5 RID: 5365
		// (get) Token: 0x0600B9ED RID: 47597 RVA: 0x0054AFC4 File Offset: 0x005491C4
		public int childCount
		{
			get
			{
				int count = 0;
				foreach (HorizontalLayoutGroup horizontal in this.horizontalLayoutGroups)
				{
					count += horizontal.transform.childCount;
				}
				return count;
			}
		}

		// Token: 0x0600B9EE RID: 47598 RVA: 0x0054B02C File Offset: 0x0054922C
		public void ResetData()
		{
			this.currentHorizontalLayoutGroupIndex = 0;
			this.startShowBackgroundWidth = 0f;
			this.widths.Clear();
			this.verticalLayout = base.GetComponent<VerticalLayoutGroup>();
			this.rectTransform = base.GetComponent<RectTransform>();
			this.horizontalLayoutGroups.Clear();
			for (int i = 0; i < this.verticalLayout.transform.childCount; i++)
			{
				HorizontalLayoutGroup horizontalLayoutGroup = this.verticalLayout.transform.GetChild(i).GetComponent<HorizontalLayoutGroup>();
				bool flag = horizontalLayoutGroup != null;
				if (flag)
				{
					this.horizontalLayoutGroups.Add(horizontalLayoutGroup);
				}
			}
			this.horizontalLayoutBackgrounds.Clear();
			for (int j = 0; j < this.backgrounds.transform.childCount; j++)
			{
				CImage image = this.backgrounds.transform.GetChild(j).GetComponent<CImage>();
				bool flag2 = image != null;
				if (flag2)
				{
					this.horizontalLayoutBackgrounds.Add(image);
				}
			}
			this.horizontalTotalWidth = this.verticalWidth - (float)this.verticalLayout.padding.left - (float)this.verticalLayout.padding.right;
			foreach (HorizontalLayoutGroup horizontal in this.horizontalLayoutGroups)
			{
				for (int k = 0; k < horizontal.transform.childCount; k++)
				{
					horizontal.transform.GetChild(k).gameObject.SetActive(false);
				}
				horizontal.gameObject.SetActive(false);
			}
			foreach (CImage image2 in this.horizontalLayoutBackgrounds)
			{
				image2.gameObject.SetActive(false);
			}
		}

		// Token: 0x0600B9EF RID: 47599 RVA: 0x0054B248 File Offset: 0x00549448
		public Transform GetChild(int index)
		{
			foreach (HorizontalLayoutGroup horizontal in this.horizontalLayoutGroups)
			{
				bool flag = index >= 0 && index < horizontal.transform.childCount;
				if (flag)
				{
					return horizontal.transform.GetChild(index);
				}
				index -= horizontal.transform.childCount;
				bool flag2 = index < 0;
				if (flag2)
				{
					break;
				}
			}
			return null;
		}

		// Token: 0x0600B9F0 RID: 47600 RVA: 0x0054B2E4 File Offset: 0x005494E4
		public T GetChild<T>(int index, T childTemplate) where T : Component
		{
			bool flag = index < this.childCount;
			T component;
			if (flag)
			{
				component = this.GetChild(index).GetComponent<T>();
			}
			else
			{
				component = Object.Instantiate<T>(childTemplate).GetComponent<T>();
			}
			return component;
		}

		// Token: 0x0600B9F1 RID: 47601 RVA: 0x0054B324 File Offset: 0x00549524
		public void AddChildren(List<RectTransform> children, bool showBackground)
		{
			foreach (RectTransform child in children)
			{
				this.AddChildToLayout(child, showBackground);
			}
		}

		// Token: 0x0600B9F2 RID: 47602 RVA: 0x0054B37C File Offset: 0x0054957C
		public void AddChild(RectTransform child, bool showBackground = false)
		{
			this.AddChildToLayout(child, showBackground);
		}

		// Token: 0x0600B9F3 RID: 47603 RVA: 0x0054B388 File Offset: 0x00549588
		private void AddChildToLayout(RectTransform child, bool showBackground)
		{
			HorizontalLayoutGroup layout = this.GetOrCreateHorizontalLayoutGroup();
			RectTransform layoutTransform = layout.transform as RectTransform;
			float showWidth = 0f;
			int showNum = 0;
			for (int i = 0; i < layoutTransform.childCount; i++)
			{
				RectTransform showChild = layoutTransform.GetChild(i).GetComponent<RectTransform>();
				bool activeSelf = showChild.gameObject.activeSelf;
				if (activeSelf)
				{
					showWidth += showChild.sizeDelta.x;
					showNum++;
				}
			}
			showWidth += (float)(showNum - 1) * this.horizontalLayoutGroupDefault.spacing;
			float width = showWidth + this.horizontalLayoutGroupDefault.spacing + child.sizeDelta.x;
			bool nextIndex = width > this.horizontalTotalWidth;
			bool flag = nextIndex;
			if (flag)
			{
				this.currentHorizontalLayoutGroupIndex++;
				this.widths.Clear();
			}
			HorizontalLayoutGroup horizontalLayoutGroup = this.GetOrCreateHorizontalLayoutGroup();
			horizontalLayoutGroup.gameObject.SetActive(true);
			child.SetParent(horizontalLayoutGroup.transform, false);
			child.gameObject.SetActive(true);
			if (showBackground)
			{
				bool flag2 = this.widths.Count <= 0;
				if (flag2)
				{
					this.startShowBackgroundWidth = child.anchoredPosition.x - child.sizeDelta.x / 2f;
				}
				this.widths.Add(child.sizeDelta.x);
			}
			else
			{
				this.widths.Clear();
			}
			CImage background = this.horizontalLayoutBackgrounds[this.currentHorizontalLayoutGroupIndex];
			background.gameObject.SetActive(false);
			if (showBackground)
			{
				Vector2 size = this.GetBackgroundSize(horizontalLayoutGroup.transform as RectTransform);
				this.ShowHorizontalLayoutBackground(this.currentHorizontalLayoutGroupIndex, size, this.startShowBackgroundWidth, child);
				bool flag3 = nextIndex;
				if (flag3)
				{
					this.widths.Clear();
					this.widths.Add(child.sizeDelta.x);
					this.startShowBackgroundWidth = child.anchoredPosition.x - child.sizeDelta.x / 2f;
				}
			}
			background.gameObject.SetActive(showBackground);
		}

		// Token: 0x0600B9F4 RID: 47604 RVA: 0x0054B5B8 File Offset: 0x005497B8
		private HorizontalLayoutGroup GetOrCreateHorizontalLayoutGroup()
		{
			bool flag = this.currentHorizontalLayoutGroupIndex < this.horizontalLayoutGroups.Count;
			HorizontalLayoutGroup result;
			if (flag)
			{
				result = this.horizontalLayoutGroups[this.currentHorizontalLayoutGroupIndex];
			}
			else
			{
				result = this.CreateHorizontalLayoutGroup();
			}
			return result;
		}

		// Token: 0x0600B9F5 RID: 47605 RVA: 0x0054B5FC File Offset: 0x005497FC
		private HorizontalLayoutGroup CreateHorizontalLayoutGroup()
		{
			HorizontalLayoutGroup horizontalLayoutGroupNew = Object.Instantiate<HorizontalLayoutGroup>(this.horizontalLayoutGroupDefault);
			horizontalLayoutGroupNew.name = this.horizontalLayoutGroupDefault.name;
			horizontalLayoutGroupNew.transform.SetParent(this.rectTransform, false);
			this.horizontalLayoutGroups.Add(horizontalLayoutGroupNew);
			this.CreateBackground();
			return horizontalLayoutGroupNew;
		}

		// Token: 0x0600B9F6 RID: 47606 RVA: 0x0054B654 File Offset: 0x00549854
		private CImage CreateBackground()
		{
			RectTransform rectTransform = Object.Instantiate<RectTransform>(this.backgroundDefault);
			rectTransform.name = this.backgroundDefault.name;
			rectTransform.SetParent(this.backgrounds, false);
			rectTransform.gameObject.SetActive(false);
			CImage image = rectTransform.GetComponent<CImage>();
			this.horizontalLayoutBackgrounds.Add(image);
			return image;
		}

		// Token: 0x0600B9F7 RID: 47607 RVA: 0x0054B6B4 File Offset: 0x005498B4
		private Vector2 GetBackgroundSize(RectTransform layout)
		{
			return new Vector2(this.widths.Sum() + (float)(this.widths.Count - 1) * this.horizontalLayoutGroupDefault.spacing + this.backgroundOffset.x * 2f, layout.sizeDelta.y + this.backgroundOffset.y * 2f);
		}

		// Token: 0x0600B9F8 RID: 47608 RVA: 0x0054B720 File Offset: 0x00549920
		private void ShowHorizontalLayoutBackground(int index, Vector2 size, float startWidth, RectTransform child)
		{
			CImage background = this.horizontalLayoutBackgrounds[index];
			RectTransform rectTransform = background.GetComponent<RectTransform>();
			float y = -(child.sizeDelta.y * (float)(index + 1) + (float)index * this.verticalLayout.spacing - child.sizeDelta.y / 2f);
			rectTransform.sizeDelta = size;
			rectTransform.anchoredPosition = new Vector2(startWidth - this.backgroundOffset.x, y);
			background.gameObject.SetActive(true);
		}

		// Token: 0x04008FC6 RID: 36806
		[Header("最大宽度")]
		public float verticalWidth;

		// Token: 0x04008FC7 RID: 36807
		[SerializeField]
		[Header("默认HorizontalLayout模板")]
		private HorizontalLayoutGroup horizontalLayoutGroupDefault;

		// Token: 0x04008FC8 RID: 36808
		[SerializeField]
		[Header("背景节点")]
		private RectTransform backgrounds;

		// Token: 0x04008FC9 RID: 36809
		[SerializeField]
		[Header("默认背景模板")]
		private RectTransform backgroundDefault;

		// Token: 0x04008FCA RID: 36810
		[SerializeField]
		[Header("背景大小的偏移")]
		private Vector2 backgroundOffset = new Vector2(14f, 2f);

		// Token: 0x04008FCB RID: 36811
		private VerticalLayoutGroup verticalLayout;

		// Token: 0x04008FCC RID: 36812
		private RectTransform rectTransform;

		// Token: 0x04008FCD RID: 36813
		private float horizontalTotalWidth;

		// Token: 0x04008FCE RID: 36814
		private int currentHorizontalLayoutGroupIndex;

		// Token: 0x04008FCF RID: 36815
		private List<HorizontalLayoutGroup> horizontalLayoutGroups = new List<HorizontalLayoutGroup>();

		// Token: 0x04008FD0 RID: 36816
		private float startShowBackgroundWidth;

		// Token: 0x04008FD1 RID: 36817
		private List<CImage> horizontalLayoutBackgrounds = new List<CImage>();

		// Token: 0x04008FD2 RID: 36818
		private List<float> widths = new List<float>();
	}
}
