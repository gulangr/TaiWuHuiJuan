using System;
using System.Collections.Generic;
using System.Globalization;
using FrameWork.ModSystem;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;

namespace Game.Views.Migrate.Mod
{
	// Token: 0x0200092F RID: 2351
	public class WorkshopModTemplate : MonoBehaviour
	{
		// Token: 0x06006DC3 RID: 28099 RVA: 0x0032B988 File Offset: 0x00329B88
		public void RefreshItem(ModInfoWithDisplayData modInfo)
		{
			this.title.SetText(modInfo.Title, true);
			this.author.SetText(LocalStringManager.Get(LanguageKey.LK_Author) + ":" + modInfo.Author, true);
			this.description.SetText(modInfo.Description, true);
			this.score.SetText(modInfo.Score.ToString(), true);
			this.updateDate.SetText(LocalStringManager.Get(LanguageKey.LK_Mod_UpdateData) + ":" + this.GetTimeString(modInfo.UpdateData), true);
			this.publishDate.SetText(LocalStringManager.Get(LanguageKey.LK_Mod_CreateDate) + ":" + this.GetTimeString(modInfo.CreateData), true);
			double fileSize = Math.Round((double)modInfo.FileSize / 1048576.0, 3);
			this.size.SetText(string.Format("{0} MB", fileSize), true);
			List<string> tags = SteamManager.GetTagContentList(modInfo.TagList);
			this.tagHolder.Rebuild<RectTransform>(tags.Count, delegate(RectTransform rect, int index)
			{
				TextMeshProUGUI text = rect.GetComponentInChildren<TextMeshProUGUI>();
				text.SetText(tags[index], true);
			});
		}

		// Token: 0x06006DC4 RID: 28100 RVA: 0x0032BAC8 File Offset: 0x00329CC8
		private string GetTimeString(uint time)
		{
			return DateTimeOffset.FromUnixTimeSeconds((long)((ulong)time)).DateTime.ToLocalTime().ToString(CultureInfo.CurrentCulture);
		}

		// Token: 0x0400515A RID: 20826
		[SerializeField]
		public TextMeshProUGUI title;

		// Token: 0x0400515B RID: 20827
		[SerializeField]
		public TextMeshProUGUI author;

		// Token: 0x0400515C RID: 20828
		[SerializeField]
		public TextMeshProUGUI description;

		// Token: 0x0400515D RID: 20829
		[SerializeField]
		public TextMeshProUGUI score;

		// Token: 0x0400515E RID: 20830
		[SerializeField]
		public TextMeshProUGUI updateDate;

		// Token: 0x0400515F RID: 20831
		[SerializeField]
		public TextMeshProUGUI publishDate;

		// Token: 0x04005160 RID: 20832
		[SerializeField]
		public TextMeshProUGUI size;

		// Token: 0x04005161 RID: 20833
		[SerializeField]
		public CRawImage coverImage;

		// Token: 0x04005162 RID: 20834
		[SerializeField]
		public CButton detailButton;

		// Token: 0x04005163 RID: 20835
		[SerializeField]
		public CToggle subscribeToggle;

		// Token: 0x04005164 RID: 20836
		[SerializeField]
		public TemplatedContainerAssemblyNew tagHolder;

		// Token: 0x04005165 RID: 20837
		[SerializeField]
		public CImage subscribeDecoration;
	}
}
