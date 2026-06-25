using System;
using System.Collections;
using System.Linq;
using Config;
using FrameWork;
using Game.Views.Encyclopedia;
using GameData.Domains.Information;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

// Token: 0x020002B2 RID: 690
public class MouseTipLifeSkillDetailUnlockInformation : MouseTipBase
{
	// Token: 0x06002A94 RID: 10900 RVA: 0x00146394 File Offset: 0x00144594
	protected override void Init(ArgumentBox argsBox)
	{
		this.Element.ForceListenCommand = true;
		bool flag = !PoolManager.HasData("MouseTipLifeSkillDetailUnlockInformation/PageTemplate");
		if (flag)
		{
			PoolManager.SetSrcObject("MouseTipLifeSkillDetailUnlockInformation/PageTemplate", this.PageTemplate);
		}
		this.ClearInstances();
		LifeSkillTypeItem typeConfig;
		MouseTipLifeSkillDetailUnlockInformation.ProgressProvider progressProvider;
		int characterId;
		bool flag2 = !argsBox.Get(MouseTipLifeSkillDetailUnlockInformation.ArgKeyCharacterId, out characterId) || !argsBox.Get<MouseTipLifeSkillDetailUnlockInformation.ProgressProvider>(MouseTipLifeSkillDetailUnlockInformation.ArgKeyProgressProvider, out progressProvider) || !argsBox.Get<LifeSkillTypeItem>(MouseTipLifeSkillDetailUnlockInformation.ArgKeyLifeSkillType, out typeConfig);
		if (!flag2)
		{
			InformationDomainMethod.AsyncCall.GetCharacterNormalInformation(null, characterId, delegate(int offset, RawDataPool pool)
			{
				NormalInformationCollection c = new NormalInformationCollection();
				Serializer.Deserialize(pool, offset, ref c);
				this._coroutine = SingletonObject.getInstance<YieldHelper>().ReStartCoroutine(this._coroutine, base.<Init>g__Routine|1(c));
			});
		}
	}

	// Token: 0x06002A95 RID: 10901 RVA: 0x0014643C File Offset: 0x0014463C
	internal static void RefreshBookPageView(Refers instance, SkillBookItem bookConfig, bool hasRead, int page, out Action<RectTransform> refitContentSize)
	{
		string color = hasRead ? "lightblue" : "brightred";
		TextMeshProUGUI labelBookName = instance.CGet<TextMeshProUGUI>("BookName");
		labelBookName.text = bookConfig.Name.SetColor(color);
		instance.CGet<TextMeshProUGUI>("PageName").text = LocalStringManager.Get("LK_Book_Page_Index_" + page.ToString()).SetColor(color);
		CImage gradeImage = instance.CGet<CImage>("GradeBack");
		gradeImage.gameObject.SetActive(true);
		gradeImage.SetSprite(ItemView.GetGradeIcon(bookConfig.Grade), false, null);
		TextMeshProUGUI componentInChildren = gradeImage.GetComponentInChildren<TextMeshProUGUI>();
		if (componentInChildren != null)
		{
			componentInChildren.SetText(ItemView.GetGradeText(bookConfig.Grade), true);
		}
		refitContentSize = delegate(RectTransform root)
		{
			UI_RecruitPeopleOverview.RefitContentSize(root, labelBookName.rectTransform);
		};
	}

	// Token: 0x06002A96 RID: 10902 RVA: 0x00146514 File Offset: 0x00144714
	private void ClearInstances()
	{
		foreach (Transform instance in from i in Enumerable.Range(0, this.PagesRoot.childCount)
		select this.PagesRoot.GetChild(i))
		{
			bool flag = instance != this.PageTemplate.transform;
			if (flag)
			{
				PoolManager.Destroy("MouseTipLifeSkillDetailUnlockInformation/PageTemplate", instance.gameObject);
			}
		}
	}

	// Token: 0x06002A97 RID: 10903 RVA: 0x001465A0 File Offset: 0x001447A0
	private void Update()
	{
		bool flag = CommonCommandKit.PrimaryInteraction.Check(this.Element, false, false, false, true, false);
		if (flag)
		{
			ViewEncyclopediaPanel.OpenLink(EncyclopediaTipLink.DefValue.FineArtsInsight);
		}
	}

	// Token: 0x06002A98 RID: 10904 RVA: 0x001465D2 File Offset: 0x001447D2
	private void OnDestroy()
	{
		this.ClearInstances();
		PoolManager.RemoveData("MouseTipLifeSkillDetailUnlockInformation/PageTemplate");
	}

	// Token: 0x04001ECC RID: 7884
	public TextMeshProUGUI Desc;

	// Token: 0x04001ECD RID: 7885
	public TextMeshProUGUI LevelDesc;

	// Token: 0x04001ECE RID: 7886
	public RectTransform PagesRoot;

	// Token: 0x04001ECF RID: 7887
	public GameObject PageTemplate;

	// Token: 0x04001ED0 RID: 7888
	private IEnumerator _coroutine;

	// Token: 0x04001ED1 RID: 7889
	public static readonly string ArgKeyCharacterId = "CharacterId";

	// Token: 0x04001ED2 RID: 7890
	public static readonly string ArgKeyLifeSkillType = "LifeSkillType";

	// Token: 0x04001ED3 RID: 7891
	public static readonly string ArgKeyProgressProvider = "ProgressProvider";

	// Token: 0x04001ED4 RID: 7892
	private const string PageItemPrefabKey = "MouseTipLifeSkillDetailUnlockInformation/PageTemplate";

	// Token: 0x0200160B RID: 5643
	// (Invoke) Token: 0x0600D0B4 RID: 53428
	public delegate sbyte[] ProgressProvider(short skillTemplateId);
}
