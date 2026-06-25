using System;
using System.Text.RegularExpressions;
using Game.Views.Encyclopedia.Event;
using Game.Views.Encyclopedia.Utilities;

namespace Game.Views.Encyclopedia
{
	// Token: 0x02000A50 RID: 2640
	[Serializable]
	public class EncyclopediaContentItem : IEncyclopediaSearchableContent
	{
		// Token: 0x0600826D RID: 33389 RVA: 0x003CCB2C File Offset: 0x003CAD2C
		public EncyclopediaContentItem(int templateId, string title1, string title2, string title3, string title4, string title5, EEncyclopediaContentLayer layer, string content, EEncyclopediaContentLevel level, EEncyclopediaContentFonts fonts, EEncyclopediaContentLayout[] layout, string[] enabledHyperLinks, int[] inserts, string key)
		{
			this.TemplateId = templateId;
			this.Title1 = title1;
			this.Title2 = title2;
			this.Title3 = title3;
			this.Title4 = title4;
			this.Title5 = title5;
			this.Layer = layer;
			this.Content = content;
			this.Level = level;
			this.Fonts = fonts;
			this.Layout = layout;
			this.EnabledHyperLinks = enabledHyperLinks;
			this.Inserts = inserts;
			this.Key = key;
			this.ProcessIndex(key);
		}

		// Token: 0x0600826E RID: 33390 RVA: 0x003CCBB8 File Offset: 0x003CADB8
		public void ProcessIndex(string key)
		{
			int index = key.IndexOf('-');
			this.ParentKey1 = ((index >= 0) ? key.Substring(0, index) : key);
			index = ((index != -1) ? key.IndexOf('-', index + 1) : -1);
			this.ParentKey2 = ((index >= 0) ? key.Substring(0, index) : ((this.Layer == EEncyclopediaContentLayer.Two) ? key : string.Empty));
			index = ((index != -1) ? key.IndexOf('-', index + 1) : -1);
			this.ParentKey3 = ((index >= 0) ? key.Substring(0, index) : ((this.Layer == EEncyclopediaContentLayer.Three) ? key : ((this.Layer == EEncyclopediaContentLayer.Content) ? EncyclopediaContentItem.NumberEnding.Replace(key, "") : string.Empty)));
			index = ((index != -1) ? key.IndexOf('-', index + 1) : -1);
			this.ParentKey4 = ((index >= 0) ? key.Substring(0, index) : ((this.Layer == EEncyclopediaContentLayer.Four) ? key : ((this.Layer == EEncyclopediaContentLayer.Content) ? EncyclopediaContentItem.NumberEnding.Replace(key, "") : string.Empty)));
		}

		// Token: 0x0600826F RID: 33391 RVA: 0x003CCCC0 File Offset: 0x003CAEC0
		bool IEncyclopediaSearchableContent.Contains(string str)
		{
			EEncyclopediaContentLayer layer = this.Layer;
			if (!true)
			{
			}
			int num;
			bool result = layer - EEncyclopediaContentLayer.Three <= 2 && !string.IsNullOrEmpty(str) && ((IEncyclopediaSearchableContent)this).Content.NormalTextContains(str, out num);
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x06008270 RID: 33392 RVA: 0x003CCD05 File Offset: 0x003CAF05
		bool IEncyclopediaSearchableContent.StartsWith(string str)
		{
			return this.Content.ToLower().StartsWith(str.ToLower());
		}

		// Token: 0x17000E43 RID: 3651
		// (get) Token: 0x06008271 RID: 33393 RVA: 0x003CCD1D File Offset: 0x003CAF1D
		int IEncyclopediaSearchableContent.Length
		{
			get
			{
				return this.Content.Length;
			}
		}

		// Token: 0x17000E44 RID: 3652
		// (get) Token: 0x06008272 RID: 33394 RVA: 0x003CCD2C File Offset: 0x003CAF2C
		string IEncyclopediaSearchableContent.Content
		{
			get
			{
				EEncyclopediaContentLayer layer = this.Layer;
				if (!true)
				{
				}
				string result;
				switch (layer)
				{
				case EEncyclopediaContentLayer.Three:
					result = this.Title3;
					break;
				case EEncyclopediaContentLayer.Four:
					result = this.Title4;
					break;
				case EEncyclopediaContentLayer.Five:
					result = this.Title5;
					break;
				default:
					result = string.Empty;
					break;
				}
				if (!true)
				{
				}
				return result;
			}
		}

		// Token: 0x17000E45 RID: 3653
		// (get) Token: 0x06008273 RID: 33395 RVA: 0x003CCD82 File Offset: 0x003CAF82
		string IEncyclopediaSearchableContent.DisplayText
		{
			get
			{
				return ((IEncyclopediaSearchableContent)this).Content;
			}
		}

		// Token: 0x17000E46 RID: 3654
		// (get) Token: 0x06008274 RID: 33396 RVA: 0x003CCD8C File Offset: 0x003CAF8C
		int IEncyclopediaSearchableContent.ClickEventType
		{
			get
			{
				EEncyclopediaContentLayer layer = this.Layer;
				if (!true)
				{
				}
				int result = 0;
				if (!true)
				{
				}
				return result;
			}
		}

		// Token: 0x17000E47 RID: 3655
		// (get) Token: 0x06008275 RID: 33397 RVA: 0x003CCDB0 File Offset: 0x003CAFB0
		IEventArgs IEncyclopediaSearchableContent.ClickEventArgs
		{
			get
			{
				EEncyclopediaContentLayer layer = this.Layer;
				if (!true)
				{
				}
				IEventArgs result;
				if (layer - EEncyclopediaContentLayer.Three > 2)
				{
					result = null;
				}
				else
				{
					result = EventArgs<string>.CreateEventArgs(this.Key);
				}
				if (!true)
				{
				}
				return result;
			}
		}

		// Token: 0x040063C0 RID: 25536
		public readonly int TemplateId;

		// Token: 0x040063C1 RID: 25537
		public readonly string Title1;

		// Token: 0x040063C2 RID: 25538
		public readonly string Title2;

		// Token: 0x040063C3 RID: 25539
		public readonly string Title3;

		// Token: 0x040063C4 RID: 25540
		public readonly string Title4;

		// Token: 0x040063C5 RID: 25541
		public readonly string Title5;

		// Token: 0x040063C6 RID: 25542
		public readonly EEncyclopediaContentLayer Layer;

		// Token: 0x040063C7 RID: 25543
		public readonly string Content;

		// Token: 0x040063C8 RID: 25544
		public readonly EEncyclopediaContentLevel Level;

		// Token: 0x040063C9 RID: 25545
		public readonly EEncyclopediaContentFonts Fonts;

		// Token: 0x040063CA RID: 25546
		public readonly EEncyclopediaContentLayout[] Layout;

		// Token: 0x040063CB RID: 25547
		public readonly string[] EnabledHyperLinks;

		// Token: 0x040063CC RID: 25548
		public readonly int[] Inserts;

		// Token: 0x040063CD RID: 25549
		public readonly string Key;

		// Token: 0x040063CE RID: 25550
		public string ParentKey1;

		// Token: 0x040063CF RID: 25551
		public string ParentKey2;

		// Token: 0x040063D0 RID: 25552
		public string ParentKey3;

		// Token: 0x040063D1 RID: 25553
		public string ParentKey4;

		// Token: 0x040063D2 RID: 25554
		private static readonly Regex NumberEnding = new Regex("[0-9]+$", RegexOptions.Compiled);
	}
}
