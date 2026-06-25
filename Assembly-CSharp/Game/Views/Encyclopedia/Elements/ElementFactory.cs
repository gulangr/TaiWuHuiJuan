using System;
using Game.Views.Encyclopedia.SyntaxTree;
using Game.Views.Encyclopedia.Utilities;
using Game.Views.Encyclopedia.Views;
using UnityEngine;

namespace Game.Views.Encyclopedia.Elements
{
	// Token: 0x02000A84 RID: 2692
	internal class ElementFactory : SingletonBehaviour<ElementFactory>
	{
		// Token: 0x06008401 RID: 33793 RVA: 0x003D5A99 File Offset: 0x003D3C99
		protected override void Awake()
		{
			this._textHighlightRenderPool = new PoolItem("textHighlightRender", this.textHighlightRenderPrefab);
			base.Awake();
		}

		// Token: 0x06008402 RID: 33794 RVA: 0x003D5ABC File Offset: 0x003D3CBC
		internal Element CreateElement(NodeData data, Transform parent)
		{
			NodeContentType nodeContentType = data.NodeContentType;
			if (!true)
			{
			}
			int num;
			Element result;
			switch (nodeContentType)
			{
			case NodeContentType.Title:
				num = 0;
				break;
			case NodeContentType.Text:
				num = 1;
				break;
			case NodeContentType.ImageText:
				result = Object.Instantiate<ImageTextElement>(this.imageTextElementPrefab, parent, false);
				goto IL_F6;
			case NodeContentType.Table:
				result = Object.Instantiate<TableElement>(this.tableElementPrefab, parent, false);
				goto IL_F6;
			case NodeContentType.TableCollection:
				result = Object.Instantiate<TableCollectionElement>(this.tableCollectionElementPrefab, parent, false);
				goto IL_F6;
			case NodeContentType.Link:
				goto IL_DF;
			case NodeContentType.SingleText:
				result = Object.Instantiate<SingleTextElement>(this.singleTextElementPrefab, parent, false);
				goto IL_F6;
			case NodeContentType.Image:
				result = Object.Instantiate<ImageElement>(this.imageElementPrefab, parent, false);
				goto IL_F6;
			default:
				goto IL_DF;
			}
			if (data.ConfigItem.Layer != EEncyclopediaContentLayer.Four)
			{
				if (num == 0)
				{
					result = Object.Instantiate<TextElement>(this.textElementPrefab, parent, false);
					goto IL_F6;
				}
				if (num == 1)
				{
					result = Object.Instantiate<TextElement>(this.textElementPrefab, parent, false);
					goto IL_F6;
				}
			}
			result = Object.Instantiate<TitleTextElement>(this.titleTextElementPrefab, parent, false);
			goto IL_F6;
			IL_DF:
			throw new ArgumentOutOfRangeException("NodeContentType", data.NodeContentType, null);
			IL_F6:
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x06008403 RID: 33795 RVA: 0x003D5BC8 File Offset: 0x003D3DC8
		internal TitleElement CreateFavoritesItemLevelOne(Transform parent)
		{
			return Object.Instantiate<TitleElement>(this.favoritesItemLevelOnePrefab, parent);
		}

		// Token: 0x06008404 RID: 33796 RVA: 0x003D5BE8 File Offset: 0x003D3DE8
		internal TitleElement CreateLevelOneTitleItem(Transform parent)
		{
			return Object.Instantiate<TitleElement>(this.levelOneTitlePrefab, parent, false);
		}

		// Token: 0x06008405 RID: 33797 RVA: 0x003D5C08 File Offset: 0x003D3E08
		internal TitleElement CreateLevelOneTitleItemForSearch(Transform parent)
		{
			return Object.Instantiate<TitleElement>(this.levelOneTitleItemForSearch, parent, false);
		}

		// Token: 0x06008406 RID: 33798 RVA: 0x003D5C28 File Offset: 0x003D3E28
		internal TitleElement CreateLevelTwoTitleItem(Transform parent)
		{
			return Object.Instantiate<TitleElement>(this.levelTwoTitlePrefab, parent, false);
		}

		// Token: 0x06008407 RID: 33799 RVA: 0x003D5C48 File Offset: 0x003D3E48
		internal TitleElement CreateLevelThreeTitleItem(Transform parent)
		{
			return Object.Instantiate<TitleElement>(this.levelThreeTitlePrefab, parent, false);
		}

		// Token: 0x06008408 RID: 33800 RVA: 0x003D5C68 File Offset: 0x003D3E68
		internal TitleElement CreateLevelFourTitleItem(Transform parent)
		{
			return Object.Instantiate<TitleElement>(this.levelFourTitlePrefab, parent, false);
		}

		// Token: 0x06008409 RID: 33801 RVA: 0x003D5C88 File Offset: 0x003D3E88
		internal TitleElement GetOrCreateLevelOneTitleForSearch(Transform parent, int index)
		{
			int count = index + 1;
			bool flag = parent.childCount < count;
			TitleElement titleElement;
			if (flag)
			{
				titleElement = SingletonBehaviour<ElementFactory>.Instance.CreateLevelOneTitleItemForSearch(parent);
			}
			else
			{
				titleElement = parent.GetChild(index).gameObject.GetComponent<TitleElement>();
			}
			bool flag2 = !titleElement.gameObject.activeSelf;
			if (flag2)
			{
				titleElement.gameObject.SetActive(true);
			}
			return titleElement;
		}

		// Token: 0x0600840A RID: 33802 RVA: 0x003D5CF4 File Offset: 0x003D3EF4
		internal TitleElement GetOrCreateLevelTwoTitle(Transform parent, int index)
		{
			int count = index + 1;
			bool flag = parent.childCount < count;
			TitleElement titleElement;
			if (flag)
			{
				titleElement = SingletonBehaviour<ElementFactory>.Instance.CreateLevelTwoTitleItem(parent);
			}
			else
			{
				titleElement = parent.GetChild(index).gameObject.GetComponent<TitleElement>();
			}
			bool flag2 = !titleElement.gameObject.activeSelf;
			if (flag2)
			{
				titleElement.gameObject.SetActive(true);
			}
			return titleElement;
		}

		// Token: 0x0600840B RID: 33803 RVA: 0x003D5D60 File Offset: 0x003D3F60
		internal TitleElement GetOrCreateLevelThreeTitle(Transform parent, int index)
		{
			int count = index + 1;
			bool flag = parent.childCount < count;
			TitleElement titleElement;
			if (flag)
			{
				titleElement = SingletonBehaviour<ElementFactory>.Instance.CreateLevelThreeTitleItem(parent);
			}
			else
			{
				titleElement = parent.GetChild(index).gameObject.GetComponent<TitleElement>();
			}
			bool flag2 = !titleElement.gameObject.activeSelf;
			if (flag2)
			{
				titleElement.gameObject.SetActive(true);
			}
			return titleElement;
		}

		// Token: 0x0600840C RID: 33804 RVA: 0x003D5DCC File Offset: 0x003D3FCC
		internal TitleElement GetOrCreateLevelFourTitle(Transform parent, int index)
		{
			int count = index + 1;
			bool flag = parent.childCount < count;
			TitleElement titleElement;
			if (flag)
			{
				titleElement = SingletonBehaviour<ElementFactory>.Instance.CreateLevelFourTitleItem(parent);
			}
			else
			{
				titleElement = parent.GetChild(index).gameObject.GetComponent<TitleElement>();
			}
			bool flag2 = !titleElement.gameObject.activeSelf;
			if (flag2)
			{
				titleElement.gameObject.SetActive(true);
			}
			return titleElement;
		}

		// Token: 0x0600840D RID: 33805 RVA: 0x003D5E38 File Offset: 0x003D4038
		internal LabelItem GetOrCreateLabelItem(int index, Transform parent)
		{
			int count = index + 1;
			bool flag = parent.childCount < count;
			LabelItem labelItem;
			if (flag)
			{
				labelItem = this.CreateLabelItem(parent);
			}
			else
			{
				labelItem = parent.GetChild(index).gameObject.GetComponent<LabelItem>();
			}
			bool flag2 = !labelItem.gameObject.activeSelf;
			if (flag2)
			{
				labelItem.gameObject.SetActive(true);
			}
			return labelItem;
		}

		// Token: 0x0600840E RID: 33806 RVA: 0x003D5EA0 File Offset: 0x003D40A0
		internal LabelItem CreateLabelItem(Transform parent)
		{
			return Object.Instantiate<LabelItem>(this.labelItemPrefab, parent, false);
		}

		// Token: 0x0600840F RID: 33807 RVA: 0x003D5EBF File Offset: 0x003D40BF
		public TextHighlightRender CreateTextHighlightRender()
		{
			return this._textHighlightRenderPool.GetObject().GetComponent<TextHighlightRender>();
		}

		// Token: 0x06008410 RID: 33808 RVA: 0x003D5ED1 File Offset: 0x003D40D1
		public void ReturnTextHighlightRender(TextHighlightRender textHighlightRender)
		{
			this._textHighlightRenderPool.DestroyObject(textHighlightRender.gameObject);
		}

		// Token: 0x04006512 RID: 25874
		[SerializeField]
		private TitleElement favoritesItemLevelOnePrefab;

		// Token: 0x04006513 RID: 25875
		[SerializeField]
		private TitleElement levelOneTitlePrefab;

		// Token: 0x04006514 RID: 25876
		[SerializeField]
		private TitleElement levelOneTitleItemForSearch;

		// Token: 0x04006515 RID: 25877
		[SerializeField]
		private TitleElement levelTwoTitlePrefab;

		// Token: 0x04006516 RID: 25878
		[SerializeField]
		private TitleElement levelThreeTitlePrefab;

		// Token: 0x04006517 RID: 25879
		[SerializeField]
		private TitleElement levelFourTitlePrefab;

		// Token: 0x04006518 RID: 25880
		[SerializeField]
		private CellElement cellElementPrefab;

		// Token: 0x04006519 RID: 25881
		[SerializeField]
		private ImageTextElement imageTextElementPrefab;

		// Token: 0x0400651A RID: 25882
		[SerializeField]
		private TableElement tableElementPrefab;

		// Token: 0x0400651B RID: 25883
		[SerializeField]
		private TableCollectionElement tableCollectionElementPrefab;

		// Token: 0x0400651C RID: 25884
		[SerializeField]
		private TextElement textElementPrefab;

		// Token: 0x0400651D RID: 25885
		[SerializeField]
		private TitleTextElement titleTextElementPrefab;

		// Token: 0x0400651E RID: 25886
		[SerializeField]
		private SingleTextElement singleTextElementPrefab;

		// Token: 0x0400651F RID: 25887
		[SerializeField]
		private ImageElement imageElementPrefab;

		// Token: 0x04006520 RID: 25888
		[SerializeField]
		private LabelItem labelItemPrefab;

		// Token: 0x04006521 RID: 25889
		[SerializeField]
		private GameObject textHighlightRenderPrefab;

		// Token: 0x04006522 RID: 25890
		private PoolItem _textHighlightRenderPool;
	}
}
