using System;
using Game.Views.Encyclopedia.Event;
using Game.Views.Encyclopedia.Utilities;

namespace Game.Views.Encyclopedia
{
	// Token: 0x02000A61 RID: 2657
	[Serializable]
	public class EncyclopediaReferenceItem : IEncyclopediaSearchableContent
	{
		// Token: 0x060082AA RID: 33450 RVA: 0x003CE2E9 File Offset: 0x003CC4E9
		public EncyclopediaReferenceItem(int templateId, string linkId, EEncyclopediaReferenceInsertType insertType, string param, string[] stringParams, string[] desc, string title)
		{
			this.TemplateId = templateId;
			this.LinkId = linkId;
			this.InsertType = insertType;
			this.Param = param;
			this.Params = stringParams;
			this.Desc = desc;
			this.Title = title;
		}

		// Token: 0x060082AB RID: 33451 RVA: 0x003CE328 File Offset: 0x003CC528
		bool IEncyclopediaSearchableContent.Contains(string str)
		{
			EEncyclopediaReferenceInsertType insertType = this.InsertType;
			if (!true)
			{
			}
			bool result;
			if (insertType != EEncyclopediaReferenceInsertType.HyperLink)
			{
				result = false;
			}
			else
			{
				bool flag;
				if (!string.IsNullOrEmpty(str))
				{
					string[] desc = this.Desc;
					if (desc != null && desc.Length > 0)
					{
						int num;
						flag = this.Desc[0].NormalTextContains(str, out num);
						goto IL_3C;
					}
				}
				flag = false;
				IL_3C:
				result = flag;
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x060082AC RID: 33452 RVA: 0x003CE380 File Offset: 0x003CC580
		bool IEncyclopediaSearchableContent.StartsWith(string str)
		{
			EEncyclopediaReferenceInsertType insertType = this.InsertType;
			if (!true)
			{
			}
			bool result;
			if (insertType != EEncyclopediaReferenceInsertType.HyperLink)
			{
				result = false;
			}
			else
			{
				bool flag;
				if (!string.IsNullOrEmpty(str))
				{
					string[] desc = this.Desc;
					if (desc != null && desc.Length > 0)
					{
						flag = this.Desc[0].ToLower().StartsWith(str);
						goto IL_3F;
					}
				}
				flag = false;
				IL_3F:
				result = flag;
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x17000E57 RID: 3671
		// (get) Token: 0x060082AD RID: 33453 RVA: 0x003CE3D8 File Offset: 0x003CC5D8
		int IEncyclopediaSearchableContent.Length
		{
			get
			{
				EEncyclopediaReferenceInsertType insertType = this.InsertType;
				if (!true)
				{
				}
				int result;
				if (insertType != EEncyclopediaReferenceInsertType.HyperLink)
				{
					if (!true)
					{
					}
					<PrivateImplementationDetails>.ThrowSwitchExpressionException(insertType);
				}
				else
				{
					string[] len = this.Desc;
					result = ((len != null && len.Length > 0) ? len[0].Length : 0);
				}
				if (!true)
				{
				}
				return result;
			}
		}

		// Token: 0x17000E58 RID: 3672
		// (get) Token: 0x060082AE RID: 33454 RVA: 0x003CE42C File Offset: 0x003CC62C
		string IEncyclopediaSearchableContent.Content
		{
			get
			{
				EEncyclopediaReferenceInsertType insertType = this.InsertType;
				if (!true)
				{
				}
				string result;
				if (insertType != EEncyclopediaReferenceInsertType.HyperLink)
				{
					result = string.Empty;
				}
				else
				{
					string[] desc = this.Desc;
					result = ((desc != null && desc.Length > 0) ? this.Desc[0] : string.Empty);
				}
				if (!true)
				{
				}
				return result;
			}
		}

		// Token: 0x17000E59 RID: 3673
		// (get) Token: 0x060082AF RID: 33455 RVA: 0x003CE47C File Offset: 0x003CC67C
		string IEncyclopediaSearchableContent.DisplayText
		{
			get
			{
				EEncyclopediaReferenceInsertType insertType = this.InsertType;
				if (!true)
				{
				}
				string result;
				if (insertType != EEncyclopediaReferenceInsertType.HyperLink)
				{
					result = string.Empty;
				}
				else
				{
					string[] desc = this.Desc;
					result = ((desc != null && desc.Length > 0) ? ("<link=\"0\"><u><color=#brightblue>" + this.Desc[0] + "</color></u></link>").ColorReplace() : string.Empty);
				}
				if (!true)
				{
				}
				return result;
			}
		}

		// Token: 0x17000E5A RID: 3674
		// (get) Token: 0x060082B0 RID: 33456 RVA: 0x003CE4E0 File Offset: 0x003CC6E0
		int IEncyclopediaSearchableContent.ClickEventType
		{
			get
			{
				EEncyclopediaReferenceInsertType insertType = this.InsertType;
				if (!true)
				{
				}
				int result;
				if (insertType != EEncyclopediaReferenceInsertType.HyperLink)
				{
					result = 0;
				}
				else
				{
					result = 1;
				}
				if (!true)
				{
				}
				return result;
			}
		}

		// Token: 0x17000E5B RID: 3675
		// (get) Token: 0x060082B1 RID: 33457 RVA: 0x003CE50C File Offset: 0x003CC70C
		IEventArgs IEncyclopediaSearchableContent.ClickEventArgs
		{
			get
			{
				EEncyclopediaReferenceInsertType insertType = this.InsertType;
				if (!true)
				{
				}
				IEventArgs result;
				if (insertType != EEncyclopediaReferenceInsertType.HyperLink)
				{
					result = null;
				}
				else
				{
					result = EventArgs<string>.CreateEventArgs(this.Param);
				}
				if (!true)
				{
				}
				return result;
			}
		}

		// Token: 0x04006442 RID: 25666
		public readonly int TemplateId;

		// Token: 0x04006443 RID: 25667
		public readonly string LinkId;

		// Token: 0x04006444 RID: 25668
		public readonly EEncyclopediaReferenceInsertType InsertType;

		// Token: 0x04006445 RID: 25669
		public readonly string Param;

		// Token: 0x04006446 RID: 25670
		public readonly string[] Params;

		// Token: 0x04006447 RID: 25671
		public readonly string[] Desc;

		// Token: 0x04006448 RID: 25672
		public readonly string Title;
	}
}
