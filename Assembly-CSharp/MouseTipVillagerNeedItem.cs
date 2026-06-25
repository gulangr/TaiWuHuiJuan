using System;
using System.Collections.Generic;
using System.Linq;
using FrameWork;
using Game.Components.Avatar;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Item;
using GameData.Domains.Taiwu;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

// Token: 0x02000401 RID: 1025
public class MouseTipVillagerNeedItem : MouseTipBase
{
	// Token: 0x17000639 RID: 1593
	// (get) Token: 0x06003D32 RID: 15666 RVA: 0x001ECCC3 File Offset: 0x001EAEC3
	protected override bool CanStick
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06003D33 RID: 15667 RVA: 0x001ECCC8 File Offset: 0x001EAEC8
	protected override void Init(ArgumentBox argsBox)
	{
		CanvasGroup canvasGroup = base.GetComponent<CanvasGroup>();
		canvasGroup.alpha = 0f;
		this.NeedWaitData = true;
		argsBox.Get<ItemKey>("itemKey", out this._itemKey);
		argsBox.Get<ItemKey>("itemKeyReal", out this._itemKeyReal);
		this.title.text = LocalStringManager.Get(LanguageKey.LK_VillagerNeed_Tip_Title);
		sbyte itemGrade = ItemTemplateHelper.GetGrade(this._itemKey.ItemType, this._itemKey.TemplateId);
		string itemName = ItemTemplateHelper.GetName(this._itemKey.ItemType, this._itemKey.TemplateId).SetGradeColor((int)itemGrade);
		string amountContent = "x1".SetColor("pinkyellow");
		string itemNameContent = string.Concat(new string[]
		{
			" ",
			itemName,
			" ",
			amountContent,
			" "
		});
		this.descText.text = LocalStringManager.GetFormat(LanguageKey.LK_VillagerNeed_Tip_Content, itemNameContent);
		this.descText.GetComponent<TMPTextSpriteHelper>().Parse();
		Func<int, sbyte> <>9__1;
		Action <>9__3;
		TaiwuDomainMethod.AsyncCall.GetTreasuryItemNeededCharDict(this, this._itemKey, delegate(int offset, RawDataPool pool)
		{
			this._treasuryItemNeededCharDict.Clear();
			DictIntSbyteWrapper wrapper = new DictIntSbyteWrapper();
			Serializer.Deserialize(pool, offset, ref wrapper);
			bool flag = ((wrapper != null) ? wrapper.Value : null) != null;
			if (flag)
			{
				this._treasuryItemNeededCharDict.AddRangeOverride(wrapper.Value);
			}
			int total = this._treasuryItemNeededCharDict.Count;
			this.countTip.text = LocalStringManager.GetFormat(LanguageKey.LK_VillagerNeed_Tip_Count, total);
			int charDataRequestCount = (total > 10) ? 9 : total;
			int layoutSlotCount = (total > 10) ? 10 : total;
			CommonUtils.PrepareEnoughChildren(this.avatarHolder, this.avatarCellTemplate.gameObject, layoutSlotCount, null);
			this.dotCell.gameObject.SetActive(total > 10);
			IEnumerable<int> keys = this._treasuryItemNeededCharDict.Keys;
			Func<int, sbyte> keySelector;
			if ((keySelector = <>9__1) == null)
			{
				keySelector = (<>9__1 = ((int id) => this._treasuryItemNeededCharDict[id]));
			}
			List<int> charIdList = keys.OrderBy(keySelector).Take(charDataRequestCount).ToList<int>();
			CharacterDomainMethod.AsyncCall.GetCharacterDisplayDataList(this, charIdList, delegate(int offset2, RawDataPool pool2)
			{
				Serializer.Deserialize(pool2, offset2, ref this._charDataList);
				for (int i = 0; i < this._charDataList.Count; i++)
				{
					Refers refers = this.avatarHolder.GetChild(i).GetComponent<Refers>();
					Game.Components.Avatar.Avatar avatar = refers.CGet<Game.Components.Avatar.Avatar>("Avatar");
					avatar.gameObject.SetActive(true);
					CharacterDisplayData charData = this._charDataList[i];
					avatar.Refresh(charData, true);
					string charName = NameCenter.GetMonasticTitleOrDisplayName(charData, false);
					refers.CGet<TextMeshProUGUI>("Name").SetText(charName, true);
					TextMeshProUGUI timeText = refers.CGet<TextMeshProUGUI>("Time");
					timeText.text = LocalStringManager.GetFormat(LanguageKey.LK_VillagerNeed_Tip_Time, Mathf.Max(1, (int)this._treasuryItemNeededCharDict[charData.CharacterId]).ToString().SetColor("pinkyellow"));
					timeText.GetComponent<TMPTextSpriteHelper>().Parse();
				}
				bool flag2 = total > 10;
				if (flag2)
				{
					MouseTipVillagerNeedItem.SetupOverflowPlaceholderCell(this.avatarHolder.GetChild(9).GetComponent<Refers>());
				}
				this.Element.ShowAfterRefresh();
				YieldHelper instance = SingletonObject.getInstance<YieldHelper>();
				uint frame = 1U;
				Action job;
				if ((job = <>9__3) == null)
				{
					job = (<>9__3 = delegate()
					{
						canvasGroup.alpha = 1f;
					});
				}
				instance.DelayFrameDo(frame, job);
			});
		});
		this.moreInfoTips0.text = LocalStringManager.Get(LanguageKey.LK_KeyDown_Tips);
		this.moreInfoTips1.text = LocalStringManager.Get(LanguageKey.LK_VillagerNeed_Tip_OpenDetail);
	}

	// Token: 0x06003D34 RID: 15668 RVA: 0x001ECE2C File Offset: 0x001EB02C
	private static void SetupOverflowPlaceholderCell(Refers refers)
	{
		refers.CGet<Game.Components.Avatar.Avatar>("Avatar").gameObject.SetActive(false);
		refers.CGet<TextMeshProUGUI>("Name").SetText("…", true);
		TextMeshProUGUI timeText = refers.CGet<TextMeshProUGUI>("Time");
		timeText.text = string.Empty;
	}

	// Token: 0x06003D35 RID: 15669 RVA: 0x001ECE80 File Offset: 0x001EB080
	private void Update()
	{
		bool flag = UIManager.Instance.IsElementActive(UIElement.TaiwuVillagerNeedItem);
		if (!flag)
		{
			bool altDown = Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.RightAlt);
			bool flag2 = altDown;
			if (flag2)
			{
				bool exist = UIManager.Instance.IsElementActive(UIElement.TaiwuVillagerNeedItem);
				bool flag3 = !exist;
				if (flag3)
				{
					this.OpenCharacterList();
				}
			}
		}
	}

	// Token: 0x06003D36 RID: 15670 RVA: 0x001ECEE8 File Offset: 0x001EB0E8
	private void OpenCharacterList()
	{
		ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
		argBox.SetObject("ItemKey", this._itemKeyReal);
		UIElement.TaiwuVillagerNeedItem.SetOnInitArgs(argBox);
		UIManager.Instance.ShowUI(UIElement.TaiwuVillagerNeedItem, true);
	}

	// Token: 0x04002BFF RID: 11263
	private const int MaxAvatarSlots = 10;

	// Token: 0x04002C00 RID: 11264
	[SerializeField]
	private TextMeshProUGUI title;

	// Token: 0x04002C01 RID: 11265
	[SerializeField]
	private TextMeshProUGUI descText;

	// Token: 0x04002C02 RID: 11266
	[SerializeField]
	private TextMeshProUGUI countTip;

	// Token: 0x04002C03 RID: 11267
	[SerializeField]
	private TextMeshProUGUI moreInfoTips0;

	// Token: 0x04002C04 RID: 11268
	[SerializeField]
	private TextMeshProUGUI moreInfoTips1;

	// Token: 0x04002C05 RID: 11269
	[SerializeField]
	private Transform avatarHolder;

	// Token: 0x04002C06 RID: 11270
	[SerializeField]
	private Refers avatarCellTemplate;

	// Token: 0x04002C07 RID: 11271
	[SerializeField]
	private GameObject dotCell;

	// Token: 0x04002C08 RID: 11272
	private Dictionary<int, sbyte> _treasuryItemNeededCharDict = new Dictionary<int, sbyte>();

	// Token: 0x04002C09 RID: 11273
	private List<CharacterDisplayData> _charDataList = new List<CharacterDisplayData>();

	// Token: 0x04002C0A RID: 11274
	private ItemKey _itemKey = ItemKey.Invalid;

	// Token: 0x04002C0B RID: 11275
	private ItemKey _itemKeyReal = ItemKey.Invalid;
}
