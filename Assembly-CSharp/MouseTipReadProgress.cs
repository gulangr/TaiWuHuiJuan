using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using TMPro;
using UnityEngine;

// Token: 0x020002CA RID: 714
public class MouseTipReadProgress : MouseTipBase
{
	// Token: 0x06002B11 RID: 11025 RVA: 0x0014E4AD File Offset: 0x0014C6AD
	protected override void Init(ArgumentBox argsBox)
	{
		this._title = base.CGet<TextMeshProUGUI>("Title");
		this._pageHolderList = base.CGetList<Refers>("PageHolder_");
		this.Refresh(argsBox);
	}

	// Token: 0x06002B12 RID: 11026 RVA: 0x0014E4DC File Offset: 0x0014C6DC
	public override void Refresh(ArgumentBox argBox)
	{
		ItemKey readingBook;
		argBox.Get<ItemKey>("ReadingBook", out readingBook);
		bool hasReadingBook;
		argBox.Get("HasReadingBook", out hasReadingBook);
		SkillBookPageDisplayData pagesInfo;
		argBox.Get<SkillBookPageDisplayData>("PageInfo", out pagesInfo);
		this._title.text = LocalStringManager.Get(LanguageKey.LK_MouseTipReadProgress_Title);
		bool flag = hasReadingBook && pagesInfo != null;
		if (flag)
		{
			bool isCombatBook = pagesInfo.IsCombatBook;
			this._pageHolderList[5].gameObject.SetActive(isCombatBook);
			int count = isCombatBook ? 6 : 5;
			for (int i = 0; i < count; i++)
			{
				Refers refers = this._pageHolderList[i];
				CImage icon = refers.CGet<CImage>("Icon");
				icon.gameObject.SetActive(isCombatBook);
				TextMeshProUGUI pageNumber = refers.CGet<TextMeshProUGUI>("PageNumber");
				bool flag2 = isCombatBook;
				if (flag2)
				{
					pageNumber.text = ((i == 0) ? LocalStringManager.Get(LanguageKey.LK_CombatSkill_Book_First_Page) : LocalStringManager.Get("LK_Book_Page_Index_" + (i - 1).ToString()));
					bool flag3 = i == 0;
					if (flag3)
					{
						icon.SetSprite("mousetip_directory_" + pagesInfo.Type[i].ToString(), false, null);
					}
					else
					{
						icon.SetSprite((pagesInfo.Type[i] == 0) ? "mousetip_zhengni_0" : "mousetip_zhengni_1", false, null);
					}
				}
				else
				{
					pageNumber.text = LocalStringManager.Get("LK_Book_Page_Index_" + i.ToString());
				}
				TextMeshProUGUI pageProgress = refers.CGet<TextMeshProUGUI>("PageProgress");
				bool isFinished = pagesInfo.ReadingProgress[i] == 100;
				string color = isFinished ? "brightblue" : "brightred";
				pageProgress.text = string.Format("<color=#{0}>{1}%</color>", color, pagesInfo.ReadingProgress[i]).ColorReplace();
				GameObject expHolder = refers.CGet<GameObject>("ExpHolder");
				expHolder.SetActive(isFinished);
				bool flag4 = isFinished;
				if (flag4)
				{
					TextMeshProUGUI expText = refers.CGet<TextMeshProUGUI>("ExpText");
					SkillBookItem skillBook = SkillBook.Instance.GetItem(readingBook.TemplateId);
					short expPerPage = SkillGradeData.Instance[skillBook.Grade].ReadingExpGainPerPage;
					expText.text = string.Format("(     +{0})", expPerPage);
				}
			}
		}
	}

	// Token: 0x04001F1C RID: 7964
	private TextMeshProUGUI _title;

	// Token: 0x04001F1D RID: 7965
	private List<Refers> _pageHolderList;
}
