using System;
using Config;
using FrameWork;
using GameData.Domains.Building;
using TMPro;
using UnityEngine;

// Token: 0x02000278 RID: 632
public class MouseTipBuildingTeachBook : MouseTipBase
{
	// Token: 0x17000483 RID: 1155
	// (get) Token: 0x0600292E RID: 10542 RVA: 0x0013308A File Offset: 0x0013128A
	protected override bool CanStick
	{
		get
		{
			return true;
		}
	}

	// Token: 0x0600292F RID: 10543 RVA: 0x00133090 File Offset: 0x00131290
	protected override void Init(ArgumentBox argsBox)
	{
		ShopBuildingTeachBookData teachBookData;
		argsBox.Get<ShopBuildingTeachBookData>("TeachBookData", out teachBookData);
		GameObject readBookInfo = base.CGet<GameObject>("ReadBookInfo");
		Refers readBookInfoItem = base.CGet<Refers>("ReadBookInfoItem");
		readBookInfo.SetActive(teachBookData.TeachBookResult == 0);
		base.CGet<GameObject>("FailReasonLayout").SetActive(teachBookData.TeachBookResult != 0);
		bool flag = teachBookData.TeachBookResult == 0;
		if (flag)
		{
			CommonUtils.PrepareEnoughChildren(readBookInfo.transform, readBookInfoItem.gameObject, teachBookData.TeachBookInfo.Count, new CommonUtils.PrepareExtraItemInfo?(new CommonUtils.PrepareExtraItemInfo
			{
				TemplateOrder = CommonUtils.EPrepareTemplateOrder.AfterExtraItems,
				ExtraItemCount = 1
			}));
			for (int i = 0; i < teachBookData.TeachBookInfo.Count; i++)
			{
				ValueTuple<short, byte, sbyte> valueTuple = teachBookData.TeachBookInfo[i];
				short skillBookTemplateId = valueTuple.Item1;
				byte pageId = valueTuple.Item2;
				sbyte direct = valueTuple.Item3;
				Refers item = readBookInfo.transform.GetChild(i + 1).GetComponent<Refers>();
				TextMeshProUGUI info = item.CGet<TextMeshProUGUI>("BookInfo");
				SkillBookItem bookConfig = SkillBook.Instance[skillBookTemplateId];
				if (!true)
				{
				}
				string text2;
				if (direct != 0)
				{
					if (direct != 1)
					{
						text2 = "pinkyellow";
					}
					else
					{
						text2 = "brightred";
					}
				}
				else
				{
					text2 = "brightblue";
				}
				if (!true)
				{
				}
				string directColor = text2;
				string bookName = bookConfig.Name.SetColor(string.Format("GradeColor_{0}", bookConfig.Grade));
				string pageName = LocalStringManager.Get(string.Format("LK_Book_Page_Index_{0}", pageId)).SetColor(directColor);
				string text = LocalStringManager.GetFormat(LanguageKey.LK_Building_TeachBookSecondContent1, bookName, pageName);
				info.text = text;
			}
		}
		else
		{
			sbyte teachBookResult = teachBookData.TeachBookResult;
			if (!true)
			{
			}
			string text2;
			if (teachBookResult != 1)
			{
				if (teachBookResult != 2)
				{
					text2 = LocalStringManager.Get(LanguageKey.LK_Building_TeachBook_FailReason3);
				}
				else
				{
					text2 = LocalStringManager.Get(LanguageKey.LK_Building_TeachBook_FailReason2);
				}
			}
			else
			{
				text2 = LocalStringManager.Get(LanguageKey.LK_Building_TeachBook_FailReason1);
			}
			if (!true)
			{
			}
			string failReason = text2;
			base.CGet<TextMeshProUGUI>("FailReason").SetText(failReason, true);
		}
	}

	// Token: 0x06002930 RID: 10544 RVA: 0x001332AF File Offset: 0x001314AF
	public override void Refresh(ArgumentBox argBox)
	{
		this.Init(argBox);
	}
}
