using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using EasyButtons;
using FrameWork;
using GameData.DLC.FiveLoong;
using GameData.Domains.Extra;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Organization;
using GameData.Serializer;
using GameData.Utilities;
using GameDataExtensions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x02000336 RID: 822
public class CommonTableRowForItem : Refers, ILanguage
{
	// Token: 0x06002FA5 RID: 12197 RVA: 0x0017506C File Offset: 0x0017326C
	[Button("修改语言", LabelColor = "#0080FF")]
	public void OnLanguageChange(LocalStringManager.LanguageType lang)
	{
		this.language = lang;
		foreach (TMP_Text text in this.texts)
		{
			text.overflowMode = ((lang == LocalStringManager.LanguageType.CN) ? TextOverflowModes.Ellipsis : TextOverflowModes.Overflow);
			text.fontSize = (float)((lang == LocalStringManager.LanguageType.CN) ? 24 : 22);
		}
	}

	// Token: 0x1700053D RID: 1341
	// (get) Token: 0x06002FA6 RID: 12198 RVA: 0x001750BC File Offset: 0x001732BC
	private TMP_Text Desc
	{
		get
		{
			return this.texts[0];
		}
	}

	// Token: 0x1700053E RID: 1342
	// (get) Token: 0x06002FA7 RID: 12199 RVA: 0x001750C6 File Offset: 0x001732C6
	private TMP_Text Type
	{
		get
		{
			return this.texts[1];
		}
	}

	// Token: 0x1700053F RID: 1343
	// (get) Token: 0x06002FA8 RID: 12200 RVA: 0x001750D0 File Offset: 0x001732D0
	private TMP_Text Weight
	{
		get
		{
			return this.texts[2];
		}
	}

	// Token: 0x17000540 RID: 1344
	// (get) Token: 0x06002FA9 RID: 12201 RVA: 0x001750DA File Offset: 0x001732DA
	private TMP_Text Value
	{
		get
		{
			return this.texts[3];
		}
	}

	// Token: 0x17000541 RID: 1345
	// (get) Token: 0x06002FAA RID: 12202 RVA: 0x001750E4 File Offset: 0x001732E4
	private TMP_Text Durability
	{
		get
		{
			return this.texts[4];
		}
	}

	// Token: 0x17000542 RID: 1346
	// (get) Token: 0x06002FAB RID: 12203 RVA: 0x001750EE File Offset: 0x001732EE
	private TMP_Text Quantity
	{
		get
		{
			return this.texts[5];
		}
	}

	// Token: 0x17000543 RID: 1347
	// (get) Token: 0x06002FAC RID: 12204 RVA: 0x001750F8 File Offset: 0x001732F8
	// (set) Token: 0x06002FAD RID: 12205 RVA: 0x00175100 File Offset: 0x00173300
	public ItemDisplayData Data { get; private set; }

	// Token: 0x17000544 RID: 1348
	// (get) Token: 0x06002FAE RID: 12206 RVA: 0x00175109 File Offset: 0x00173309
	// (set) Token: 0x06002FAF RID: 12207 RVA: 0x00175114 File Offset: 0x00173314
	public bool IsLocked
	{
		get
		{
			return this.isLocked;
		}
		set
		{
			this.isLocked = value;
			this.SetInteractable(!this.isLocked);
			this.lockStatusGo.SetActive(this.isLocked);
			this.lockSlashGo.SetActive(this.isLocked);
			this.SetSelectState(false);
			foreach (TMP_Text item in this.texts)
			{
				item.color = (this.isLocked ? this.grey : this.pinkYellow);
			}
			foreach (CommonBookPageReadingStatus item2 in this.bookPageReadingStatus)
			{
				item2.Disable = this.isLocked;
			}
		}
	}

	// Token: 0x06002FB0 RID: 12208 RVA: 0x001751CA File Offset: 0x001733CA
	public void SetLocked(bool isLocked)
	{
		this.IsLocked = isLocked;
	}

	// Token: 0x17000545 RID: 1349
	// (get) Token: 0x06002FB1 RID: 12209 RVA: 0x001751D4 File Offset: 0x001733D4
	// (set) Token: 0x06002FB2 RID: 12210 RVA: 0x001751DC File Offset: 0x001733DC
	public bool Interactable { get; private set; }

	// Token: 0x06002FB3 RID: 12211 RVA: 0x001751E5 File Offset: 0x001733E5
	public void SetReadingStatus(int index, CommonBookPageReadingStatus.Status status)
	{
		this.bookPageReadingStatus[index].SetPageStatus((int)status, this.isLocked);
	}

	// Token: 0x06002FB4 RID: 12212 RVA: 0x001751FC File Offset: 0x001733FC
	public void SetTextWithRefreshHeight(TMP_Text tmpText, string text)
	{
		TMP_TextInfo textInfo = tmpText.textInfo;
		int num = (textInfo != null) ? textInfo.lineCount : 0;
		tmpText.text = text;
		TMP_TextInfo textInfo2 = tmpText.textInfo;
		int num2 = (textInfo2 != null) ? textInfo2.lineCount : 0;
	}

	// Token: 0x06002FB5 RID: 12213 RVA: 0x00175238 File Offset: 0x00173438
	public void SetDesc(string text)
	{
		this.SetTextWithRefreshHeight(this.Desc, text);
	}

	// Token: 0x06002FB6 RID: 12214 RVA: 0x00175248 File Offset: 0x00173448
	public void SetType(string text)
	{
		this.SetTextWithRefreshHeight(this.Type, text);
	}

	// Token: 0x06002FB7 RID: 12215 RVA: 0x00175258 File Offset: 0x00173458
	public void SetWeight(string text)
	{
		this.SetTextWithRefreshHeight(this.Weight, text);
	}

	// Token: 0x06002FB8 RID: 12216 RVA: 0x00175268 File Offset: 0x00173468
	public void SetValue(string text)
	{
		this.SetTextWithRefreshHeight(this.Value, text);
	}

	// Token: 0x06002FB9 RID: 12217 RVA: 0x00175278 File Offset: 0x00173478
	public void SetDurability(string text)
	{
		this.SetTextWithRefreshHeight(this.Durability, text);
	}

	// Token: 0x06002FBA RID: 12218 RVA: 0x00175288 File Offset: 0x00173488
	public void SetQuantity(string text)
	{
		this.SetTextWithRefreshHeight(this.Quantity, text);
	}

	// Token: 0x06002FBB RID: 12219 RVA: 0x00175298 File Offset: 0x00173498
	public void SetEquipStatus(int status)
	{
		this.SetEquipStatus((CommonTableRowForItem.EquipStatus)status);
	}

	// Token: 0x06002FBC RID: 12220 RVA: 0x001752A4 File Offset: 0x001734A4
	public void SetEquipStatus(CommonTableRowForItem.EquipStatus status)
	{
		switch (status)
		{
		case CommonTableRowForItem.EquipStatus.Preset:
			this.equipStatus.sprite = this.spritePreset;
			goto IL_52;
		case CommonTableRowForItem.EquipStatus.Equip:
			this.equipStatus.sprite = this.spriteEquip;
			goto IL_52;
		}
		this.equipStatusGo.SetActive(false);
		return;
		IL_52:
		this.equipStatusGo.SetActive(true);
	}

	// Token: 0x06002FBD RID: 12221 RVA: 0x00175310 File Offset: 0x00173510
	public void SetReadingStatus(int status)
	{
		this.SetReadingStatus((CommonTableRowForItem.ReadingStatus)status);
	}

	// Token: 0x06002FBE RID: 12222 RVA: 0x0017531C File Offset: 0x0017351C
	public void SetReadingStatus(CommonTableRowForItem.ReadingStatus status)
	{
		switch (status)
		{
		case CommonTableRowForItem.ReadingStatus.Reading:
			this.readingStatus.sprite = this.spriteReading;
			goto IL_9A;
		case CommonTableRowForItem.ReadingStatus.ReadingDone:
			this.readingStatus.sprite = this.spriteReadingDone;
			goto IL_9A;
		case CommonTableRowForItem.ReadingStatus.Referencing:
			this.readingStatus.sprite = this.spriteReferencing;
			goto IL_9A;
		case CommonTableRowForItem.ReadingStatus.ReferencingDone:
			this.readingStatus.sprite = this.spriteReferencingDone;
			goto IL_9A;
		case CommonTableRowForItem.ReadingStatus.Done:
			this.readingStatus.sprite = this.spriteBookDone;
			goto IL_9A;
		}
		this.readingStatusGo.SetActive(false);
		return;
		IL_9A:
		this.readingStatusGo.SetActive(true);
	}

	// Token: 0x06002FBF RID: 12223 RVA: 0x001753D0 File Offset: 0x001735D0
	public void SetFavoriteStatus(int status)
	{
		this.SetFavoriteStatus((CommonTableRowForItem.FavoriteStatus)status);
	}

	// Token: 0x06002FC0 RID: 12224 RVA: 0x001753DC File Offset: 0x001735DC
	public void SetFavoriteStatus(CommonTableRowForItem.FavoriteStatus status)
	{
		switch (status)
		{
		case CommonTableRowForItem.FavoriteStatus.Hate:
			this.loveStatus.sprite = this.spriteHate;
			goto IL_52;
		case CommonTableRowForItem.FavoriteStatus.Love:
			this.loveStatus.sprite = this.spriteLove;
			goto IL_52;
		}
		this.loveStatusGo.SetActive(false);
		return;
		IL_52:
		this.loveStatusGo.SetActive(true);
	}

	// Token: 0x06002FC1 RID: 12225 RVA: 0x00175448 File Offset: 0x00173648
	public void SetWidth(IEnumerable<float> width, int status = -1)
	{
		float totalWidth = 0f;
		Func<float, int, CommonTableCellForItem> <>9__0;
		Func<float, int, CommonTableCellForItem> selector;
		if ((selector = <>9__0) == null)
		{
			selector = (<>9__0 = delegate(float x, int i)
			{
				Transform child = this.GetTableRowChild(i);
				bool flag2 = x != 0f;
				if (flag2)
				{
					if (child != null)
					{
						RectTransform component = child.GetComponent<RectTransform>();
						if (component != null)
						{
							component.SetWidth(x - (float)this.spacing);
						}
					}
				}
				if (child != null)
				{
					child.gameObject.SetActive(x > 0f);
				}
				bool flag3 = x > 0f && child != null;
				if (flag3)
				{
					totalWidth += x;
				}
				return (child != null) ? child.GetComponent<CommonTableCellForItem>() : null;
			});
		}
		foreach (CommonTableCellForItem item in width.Select(selector))
		{
			bool flag = status != -1;
			if (flag)
			{
				if (item != null)
				{
					item.SetCurrentStatus(status);
				}
			}
		}
		base.RectTransform.SetWidth(totalWidth - (float)this.spacing / 2f);
	}

	// Token: 0x06002FC2 RID: 12226 RVA: 0x00175504 File Offset: 0x00173704
	[Button("修改图片背景，可以在Unity或者运行时调用，保证最后一个图片的背景显示正常", LabelColor = "#80FF00")]
	public void RefreshBg()
	{
		CommonTableHead commonTableHead = this.tableHead;
		Sprite[] src = (commonTableHead != null) ? commonTableHead.LastSprites : null;
		Sprite[] dst;
		bool flag;
		if (src != null)
		{
			CommonTableHead commonTableHead2 = this.tableHead;
			dst = ((commonTableHead2 != null) ? commonTableHead2.NormalSprites : null);
			flag = (dst == null);
		}
		else
		{
			flag = true;
		}
		bool flag2 = flag;
		if (!flag2)
		{
			int i = Math.Min(src.Length, dst.Length);
			while (i-- > 0)
			{
				bool lastMeet = false;
				Sprite sprite = src[i];
				Sprite normal = dst[i];
				int idx = this.container.childCount;
				while (idx-- > 0)
				{
					CommonTableCellForItem cell = this.container.GetChild(idx).GetComponent<CommonTableCellForItem>();
					bool flag3 = cell == null;
					if (!flag3)
					{
						bool modified = false;
						bool flag4 = lastMeet;
						if (flag4)
						{
							int j = cell.Sprites.Length;
							while (j-- > 0)
							{
								bool flag5 = cell.Sprites[j] == sprite;
								if (flag5)
								{
									cell.Sprites[j] = normal;
									modified = true;
								}
							}
						}
						else
						{
							bool activeSelf = this.container.GetChild(idx).gameObject.activeSelf;
							if (activeSelf)
							{
								lastMeet = true;
								int k = cell.Sprites.Length;
								while (k-- > 0)
								{
									bool flag6 = cell.Sprites[k] == normal;
									if (flag6)
									{
										cell.Sprites[k] = sprite;
										modified = true;
									}
								}
							}
						}
						bool flag7 = modified;
						if (flag7)
						{
							cell.SetCurrentStatus(cell.Status);
						}
					}
				}
			}
		}
	}

	// Token: 0x06002FC3 RID: 12227 RVA: 0x0017569F File Offset: 0x0017389F
	public void SetEnterEvent(UnityAction onEnter)
	{
		this._onMouseEnterCallback = onEnter;
	}

	// Token: 0x06002FC4 RID: 12228 RVA: 0x001756A9 File Offset: 0x001738A9
	public void SetExitEvent(UnityAction onExit)
	{
		this._onMouseExitCallback = onExit;
	}

	// Token: 0x06002FC5 RID: 12229 RVA: 0x001756B3 File Offset: 0x001738B3
	public void OnMouseEnter()
	{
		this.OnMouseHover();
		UnityAction onMouseEnterCallback = this._onMouseEnterCallback;
		if (onMouseEnterCallback != null)
		{
			onMouseEnterCallback();
		}
	}

	// Token: 0x06002FC6 RID: 12230 RVA: 0x001756CF File Offset: 0x001738CF
	public void OnMouseHover()
	{
		this.SetStatus(true);
	}

	// Token: 0x06002FC7 RID: 12231 RVA: 0x001756D9 File Offset: 0x001738D9
	public void OnMouseExit()
	{
		this.SetStatus(false);
		UnityAction onMouseExitCallback = this._onMouseExitCallback;
		if (onMouseExitCallback != null)
		{
			onMouseExitCallback();
		}
	}

	// Token: 0x06002FC8 RID: 12232 RVA: 0x001756F8 File Offset: 0x001738F8
	private void SetStatus(bool hover)
	{
		int status = (hover || this._selected) ? 1 : 0;
		bool flag = this.isLocked;
		if (flag)
		{
			status += 2;
		}
		CommonTableCellForItem component = base.GetComponent<CommonTableCellForItem>();
		if (component != null)
		{
			component.SetCurrentStatus(status);
		}
		for (int i = 0; i < this.container.childCount; i++)
		{
			CommonTableCellForItem component2 = this.container.GetChild(i).GetComponent<CommonTableCellForItem>();
			if (component2 != null)
			{
				component2.SetCurrentStatus(status);
			}
		}
		foreach (GameObject go in this.activeWhileHoverObjects)
		{
			go.SetActive(status == 1);
		}
	}

	// Token: 0x06002FC9 RID: 12233 RVA: 0x001757A0 File Offset: 0x001739A0
	public void SetSelectState(bool selected)
	{
		this._selected = selected;
		this.SetStatus(selected);
		this.selectedGo.SetActive(selected);
	}

	// Token: 0x06002FCA RID: 12234 RVA: 0x001757BF File Offset: 0x001739BF
	public void SetInteractable(bool interactable)
	{
		this.Interactable = interactable;
		this.button.interactable = interactable;
	}

	// Token: 0x06002FCB RID: 12235 RVA: 0x001757D7 File Offset: 0x001739D7
	private TextMeshProUGUI GetInteractionStateText()
	{
		return this.Names.Contains("Attainment") ? base.CGet<TextMeshProUGUI>("Attainment") : null;
	}

	// Token: 0x06002FCC RID: 12236 RVA: 0x001757FC File Offset: 0x001739FC
	public void ShowInteractionStateAddition(int value)
	{
		TextMeshProUGUI interactionStateText = this.GetInteractionStateText();
		bool flag = null != interactionStateText;
		if (flag)
		{
			interactionStateText.text = LocalStringManager.GetFormat(LanguageKey.LK_CombatSkill_SpecialBreak_ConvertToExp_Item_ProgressValue, value).SetColor("brightblue");
			interactionStateText.transform.parent.gameObject.SetActive(true);
		}
	}

	// Token: 0x06002FCD RID: 12237 RVA: 0x00175858 File Offset: 0x00173A58
	public void ShowInteractionStateAttainment(sbyte lifeSkillType, short requiredValue, bool isMeet)
	{
		TextMeshProUGUI interactionStateText = this.GetInteractionStateText();
		bool flag = null != interactionStateText;
		if (flag)
		{
			LanguageKey key = isMeet ? LanguageKey.LK_Item_Operation_LifeSkill_Require_Meet : LanguageKey.LK_Item_Operation_LifeSkill_Require_Not_Meet;
			string valueStr = requiredValue.ToString().SetColor(isMeet ? "brightblue" : "brightred");
			interactionStateText.text = LocalStringManager.GetFormat(key, LifeSkillType.Instance[lifeSkillType].Name, valueStr);
			interactionStateText.transform.parent.gameObject.SetActive(true);
		}
		this.SetInteractable(isMeet);
	}

	// Token: 0x06002FCE RID: 12238 RVA: 0x001758E4 File Offset: 0x00173AE4
	public void SetItemNotCanSelectReason(string notSelect)
	{
		TextMeshProUGUI interactionStateText = this.GetInteractionStateText();
		bool flag = interactionStateText;
		if (flag)
		{
			interactionStateText.text = notSelect;
			interactionStateText.transform.parent.gameObject.SetActive(true);
		}
	}

	// Token: 0x06002FCF RID: 12239 RVA: 0x00175924 File Offset: 0x00173B24
	public void HideInteractionState()
	{
		bool flag = this.IsLocked;
		if (!flag)
		{
			TextMeshProUGUI interactionStateText = this.GetInteractionStateText();
			bool flag2 = null != interactionStateText;
			if (flag2)
			{
				interactionStateText.transform.parent.gameObject.SetActive(false);
			}
			this.SetInteractable(true);
		}
	}

	// Token: 0x06002FD0 RID: 12240 RVA: 0x00175970 File Offset: 0x00173B70
	public void ShowInteractionStateLocked()
	{
		TextMeshProUGUI interactionStateText = this.GetInteractionStateText();
		bool flag = null != interactionStateText;
		if (flag)
		{
			interactionStateText.text = LocalStringManager.Get(LanguageKey.LK_Item_Operation_Locked).SetColor("brightred");
			interactionStateText.transform.parent.gameObject.SetActive(true);
		}
		this.SetInteractable(false);
	}

	// Token: 0x06002FD1 RID: 12241 RVA: 0x001759CC File Offset: 0x00173BCC
	public void SetInteractionStateLockText(string text)
	{
		TextMeshProUGUI interactionStateText = this.GetInteractionStateText();
		bool flag = null != interactionStateText;
		if (flag)
		{
			interactionStateText.text = text.SetColor("brightred");
			interactionStateText.transform.parent.gameObject.SetActive(true);
		}
	}

	// Token: 0x06002FD2 RID: 12242 RVA: 0x00175A18 File Offset: 0x00173C18
	public void SetClickEvent(UnityAction onClick)
	{
		CButtonObsolete btn = base.GetComponent<CButtonObsolete>();
		btn.onClick.RemoveAllListeners();
		bool flag = onClick != null;
		if (flag)
		{
			btn.onClick.AddListener(onClick);
		}
	}

	// Token: 0x06002FD3 RID: 12243 RVA: 0x00175A50 File Offset: 0x00173C50
	public void Click()
	{
		CButtonObsolete btn = base.GetComponent<CButtonObsolete>();
		bool interactable = btn.interactable;
		if (interactable)
		{
			Button.ButtonClickedEvent onClick = btn.onClick;
			if (onClick != null)
			{
				onClick.Invoke();
			}
		}
	}

	// Token: 0x06002FD4 RID: 12244 RVA: 0x00175A84 File Offset: 0x00173C84
	public Transform GetTableRowChild(int contentIndex)
	{
		bool flag = contentIndex < 0 || contentIndex >= this.container.childCount;
		Transform result;
		if (flag)
		{
			result = null;
		}
		else
		{
			result = this.container.GetChild(contentIndex);
		}
		return result;
	}

	// Token: 0x06002FD5 RID: 12245 RVA: 0x00175AC4 File Offset: 0x00173CC4
	public void SetData(ItemDisplayData itemDisplayData)
	{
		this.Data = itemDisplayData;
		this.commonItemBack.SetData(itemDisplayData, -1);
		string name = itemDisplayData.GetName(false);
		this.SetDesc(name);
		this.SetType(LocalStringManager.Get(string.Format("LK_ItemType_{0}", itemDisplayData.Key.ItemType)));
		long value = itemDisplayData.IsResource ? ((long)(itemDisplayData.Amount * (int)GlobalConfig.ResourcesWorth[(int)itemDisplayData.ResourceType])) : itemDisplayData.Value;
		this.SetValue(CommonUtils.GetDisplayStringForNum(value));
		this.SetWeight(NumberFormatUtils.FormatItemWeight(itemDisplayData.Weight));
		this.SetQuantity(CommonUtils.GetDisplayStringForNum(itemDisplayData.Amount, 100000) ?? "");
		this.SetFavoriteStatus(CommonTableRowForItem.FavoriteStatus.None);
		ItemDisplayData.ItemUsingType usingType = itemDisplayData.UsingType;
		if (!true)
		{
		}
		CommonTableRowForItem.EquipStatus equipStatus2;
		if (usingType != ItemDisplayData.ItemUsingType.EquipmentPlaned)
		{
			if (usingType != ItemDisplayData.ItemUsingType.Equiped)
			{
				equipStatus2 = CommonTableRowForItem.EquipStatus.None;
			}
			else
			{
				equipStatus2 = CommonTableRowForItem.EquipStatus.Equip;
			}
		}
		else
		{
			equipStatus2 = CommonTableRowForItem.EquipStatus.Preset;
		}
		if (!true)
		{
		}
		CommonTableRowForItem.EquipStatus equipStatus = equipStatus2;
		this.SetEquipStatus(equipStatus);
		ItemDisplayData.ItemUsingType usingType2 = itemDisplayData.UsingType;
		if (!true)
		{
		}
		CommonTableRowForItem.ReadingStatus readingStatus2;
		if (usingType2 != ItemDisplayData.ItemUsingType.Reading)
		{
			if (usingType2 != ItemDisplayData.ItemUsingType.Referring)
			{
				readingStatus2 = (itemDisplayData.IsReadingFinished ? CommonTableRowForItem.ReadingStatus.Done : CommonTableRowForItem.ReadingStatus.None);
			}
			else
			{
				readingStatus2 = (itemDisplayData.IsReadingFinished ? CommonTableRowForItem.ReadingStatus.ReferencingDone : CommonTableRowForItem.ReadingStatus.Referencing);
			}
		}
		else
		{
			readingStatus2 = (itemDisplayData.IsReadingFinished ? CommonTableRowForItem.ReadingStatus.ReadingDone : CommonTableRowForItem.ReadingStatus.Reading);
		}
		if (!true)
		{
		}
		CommonTableRowForItem.ReadingStatus readingStatus = readingStatus2;
		this.SetReadingStatus(readingStatus);
		TooltipInvoker tipDisplayer = this.GetMouseTip();
		tipDisplayer.enabled = true;
		bool templateDataOnly = !itemDisplayData.RealKey.IsValid();
		this.SetMouseTipDisplayer(tipDisplayer, true, templateDataOnly);
	}

	// Token: 0x06002FD6 RID: 12246 RVA: 0x00175C4C File Offset: 0x00173E4C
	private void SetMouseTipDisplayer(TooltipInvoker tipDisplayer, bool showBookPageInfo, bool templateDataOnly)
	{
		tipDisplayer.RuntimeParam = null;
		bool flag = this.Data.Key.ItemType == 12 && this.Data.Key.TemplateId == 239;
		if (flag)
		{
			tipDisplayer.Type = TipType.Fuyu;
			tipDisplayer.NeedRefresh = (UIElement.Combat.Exist && this.Data.Key.ItemType == 0 && this.Data.UsingType == ItemDisplayData.ItemUsingType.Equiped);
			tipDisplayer.RuntimeParam = new ArgumentBox().SetObject("ItemData", this.Data.Clone(-1));
			tipDisplayer.RuntimeParam.Set("ShowPageInfo", this.Data.Key.ItemType == 10 && showBookPageInfo);
			tipDisplayer.RuntimeParam.Set("TemplateDataOnly", templateDataOnly);
		}
		else
		{
			bool flag2 = this.Data.Key.ItemType == 5 && this.Data.Key.TemplateId >= 278 && this.Data.Key.TemplateId <= 308;
			if (flag2)
			{
				tipDisplayer.Type = TipType.JiaoEgg;
				tipDisplayer.NeedRefresh = (UIElement.Combat.Exist && this.Data.Key.ItemType == 0 && this.Data.UsingType == ItemDisplayData.ItemUsingType.Equiped);
				tipDisplayer.RuntimeParam = new ArgumentBox().SetObject("ItemData", this.Data.Clone(-1));
			}
			else
			{
				bool flag3 = this.Data.Key.Id >= 0 && this.Data.Key.ItemType == 5 && this.Data.Key.TemplateId >= 309 && this.Data.Key.TemplateId <= 339;
				if (flag3)
				{
					this.SetJiaoTipDisplayerProperties(tipDisplayer, true, false);
				}
				else
				{
					bool flag4 = this.Data.Key.Id >= 0 && this.Data.Key.ItemType == 4 && this.Data.Key.TemplateId >= 46 && this.Data.Key.TemplateId <= 76;
					if (flag4)
					{
						this.SetJiaoTipDisplayerProperties(tipDisplayer, false, false);
					}
					else
					{
						bool flag5 = this.Data.Key.Id >= 0 && this.Data.Key.ItemType == 4 && this.Data.Key.TemplateId >= 77 && this.Data.Key.TemplateId <= 85;
						if (flag5)
						{
							this.SetJiaoTipDisplayerProperties(tipDisplayer, false, true);
						}
						else
						{
							bool flag6 = ItemTemplateHelper.IsEmptyTool(this.Data.Key.ItemType, this.Data.Key.TemplateId);
							if (flag6)
							{
								tipDisplayer.Type = TipType.SingleDesc;
								tipDisplayer.RuntimeParam = new ArgumentBox().Set("arg0", LocalStringManager.Get(LanguageKey.LK_Make_None_Tool_Tip).ColorReplace());
							}
							else
							{
								bool flag7 = ItemTemplateHelper.IsMiscResource(this.Data.Key.ItemType, this.Data.Key.TemplateId);
								if (flag7)
								{
									tipDisplayer.Type = TipType.Resource;
								}
								else
								{
									tipDisplayer.Type = TooltipManager.ItemTypeToTipType[this.Data.Key.ItemType];
									tipDisplayer.NeedRefresh = (UIElement.Combat.Exist && this.Data.Key.ItemType == 0 && this.Data.UsingType == ItemDisplayData.ItemUsingType.Equiped);
									tipDisplayer.RuntimeParam = new ArgumentBox().SetObject("ItemData", this.Data.Clone(-1));
									tipDisplayer.RuntimeParam.Set("ShowPageInfo", this.Data.Key.ItemType == 10 && showBookPageInfo);
									tipDisplayer.RuntimeParam.Set("TemplateDataOnly", templateDataOnly);
									tipDisplayer.RuntimeParam.Set("CharId", this.Data.OwnerCharId);
									tipDisplayer.RuntimeParam.Set("IsNew", false);
								}
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x06002FD7 RID: 12247 RVA: 0x001760A8 File Offset: 0x001742A8
	private void SetJiaoTipDisplayerProperties(TooltipInvoker tipDisplayer, bool isMisc, bool isLoong)
	{
		ExtraDomainMethod.AsyncCall.GetJiaoLoongDisplayDataByItemKey(null, this.Data.Key, delegate(int offset, RawDataPool dataPool)
		{
			JiaoLoongDisplayData displayData = new JiaoLoongDisplayData();
			Serializer.Deserialize(dataPool, offset, ref displayData);
			tipDisplayer.Type = TipType.Jiao;
			tipDisplayer.NeedRefresh = (UIElement.Combat.Exist && this.Data.Key.ItemType == 0 && this.Data.UsingType == ItemDisplayData.ItemUsingType.Equiped);
			tipDisplayer.RuntimeParam = new ArgumentBox().SetObject("JiaoLoongData", displayData);
		});
	}

	// Token: 0x06002FD8 RID: 12248 RVA: 0x001760E8 File Offset: 0x001742E8
	public void SetResourceTip(string name, bool isTaiwu)
	{
		bool flag = !this.Data.IsResource;
		if (!flag)
		{
			TooltipInvoker tipDisplayer = this.GetMouseTip();
			tipDisplayer.Type = TipType.Resource;
			sbyte resourceType = ItemTemplateHelper.GetMiscResourceType(this.Data.Key.ItemType, this.Data.Key.TemplateId);
			ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>();
			argumentBox.Set("CharName", name);
			argumentBox.Set("ResourceType", resourceType);
			argumentBox.Set("ResourceCount", this.Data.Amount);
			bool flag2 = isTaiwu && ItemTemplateHelper.MiscResourceCanChoosy(this.Data.Key.ItemType, this.Data.Key.TemplateId);
			if (flag2)
			{
				int materialResourceCountMax = SingletonObject.getInstance<BasicGameData>().MaterialResourceMaxCount;
				argumentBox.Set("ResourceCountMax", materialResourceCountMax);
			}
			argumentBox.Set("ShowDetailChange", isTaiwu);
			argumentBox.Set("ShowOfferUpChange", isTaiwu);
			tipDisplayer.RuntimeParam = argumentBox;
		}
	}

	// Token: 0x06002FD9 RID: 12249 RVA: 0x001761E8 File Offset: 0x001743E8
	public void SetFavoriteStatus(short lovingItemSubType, short hatingItemSubType)
	{
		short itemSubType = ItemTemplateHelper.GetItemSubType(this.Data.RealKey.ItemType, this.Data.RealKey.TemplateId);
		CommonTableRowForItem.FavoriteStatus favoriteStatus = (itemSubType == lovingItemSubType) ? CommonTableRowForItem.FavoriteStatus.Love : ((itemSubType == hatingItemSubType) ? CommonTableRowForItem.FavoriteStatus.Hate : CommonTableRowForItem.FavoriteStatus.None);
		this.SetFavoriteStatus(favoriteStatus);
	}

	// Token: 0x06002FDA RID: 12250 RVA: 0x00176234 File Offset: 0x00174434
	public TooltipInvoker GetMouseTip()
	{
		return base.GetComponent<TooltipInvoker>();
	}

	// Token: 0x06002FDB RID: 12251 RVA: 0x0017623C File Offset: 0x0017443C
	public void RefreshTamePoint()
	{
		TextMeshProUGUI tampPoint;
		bool flag = this.CTryGet<TextMeshProUGUI>("TampPoint", out tampPoint);
		if (flag)
		{
			tampPoint.text = (ItemTemplateHelper.HasCarrierTame(this.Data.Key.ItemType, this.Data.Key.TemplateId) ? this.Data.CarrierTamePoint.ToString() : "-");
		}
	}

	// Token: 0x06002FDC RID: 12252 RVA: 0x001762A0 File Offset: 0x001744A0
	public void SetTreasuryTradeMark(ETreasuryOperation operation)
	{
		GameObject settlementTreasuryGo;
		CImage settlementTreasuryMark;
		TooltipInvoker settlementTreasuryMarkTip;
		bool flag = !this.CTryGet<GameObject>("SettlementTreasuryGo", out settlementTreasuryGo) || !this.CTryGet<CImage>("SettlementTreasuryMark", out settlementTreasuryMark) || !this.CTryGet<TooltipInvoker>("SettlementTreasuryMarkTip", out settlementTreasuryMarkTip);
		if (!flag)
		{
			settlementTreasuryMarkTip.enabled = false;
			bool flag2 = operation == ETreasuryOperation.Invalid;
			if (flag2)
			{
				settlementTreasuryGo.SetActive(false);
			}
			else
			{
				if (!true)
				{
				}
				string text;
				switch (operation)
				{
				case ETreasuryOperation.Steal:
					text = "sp_11_treasurystate_0";
					break;
				case ETreasuryOperation.Store:
					text = "sp_11_treasurystate_2";
					break;
				case ETreasuryOperation.Exchange:
					text = "sp_11_treasurystate_1";
					break;
				default:
					throw new ArgumentOutOfRangeException("operation", operation, null);
				}
				if (!true)
				{
				}
				string spName = text;
				settlementTreasuryGo.SetActive(true);
				settlementTreasuryMark.SetSprite(spName, false, null);
			}
		}
	}

	// Token: 0x06002FDD RID: 12253 RVA: 0x00176364 File Offset: 0x00174564
	private void SetShopDebtMark(EItemDebtState state)
	{
		CImage shopDebtMark;
		TooltipInvoker shopDebtMarkTip;
		bool flag = !this.CTryGet<CImage>("ShopDebtMark", out shopDebtMark) || !this.CTryGet<TooltipInvoker>("ShopDebtMarkTip", out shopDebtMarkTip);
		if (!flag)
		{
			bool flag2 = state == EItemDebtState.None || this.IsLocked;
			if (flag2)
			{
				shopDebtMark.gameObject.SetActive(false);
			}
			else
			{
				shopDebtMark.gameObject.SetActive(true);
				if (!true)
				{
				}
				string text;
				if (state != EItemDebtState.Remove)
				{
					if (state != EItemDebtState.Add)
					{
						throw new ArgumentOutOfRangeException("state", state, null);
					}
					text = "ui_sp_arrow_shop_1";
				}
				else
				{
					text = "ui_sp_arrow_shop_0";
				}
				if (!true)
				{
				}
				string spName = text;
				bool isAdd = state == EItemDebtState.Remove;
				shopDebtMark.SetSprite(spName, false, null);
				shopDebtMark.transform.localScale = shopDebtMark.transform.localScale.SetY((float)(isAdd ? 1 : -1));
				LanguageKey strKey = (state == EItemDebtState.Add) ? LanguageKey.LK_Shop_Item_Debt_Add : LanguageKey.LK_Shop_Item_Debt_Repay;
				string[] presetParam = shopDebtMarkTip.PresetParam;
				bool flag3 = presetParam == null || presetParam.Length != 1;
				if (flag3)
				{
					shopDebtMarkTip.PresetParam = new string[1];
				}
				shopDebtMarkTip.PresetParam[0] = LocalStringManager.Get(strKey).ColorReplace();
			}
		}
	}

	// Token: 0x06002FDE RID: 12254 RVA: 0x00176490 File Offset: 0x00174690
	public void SetShopLevelMark(EItemDebtState state, int shopLevel)
	{
		GameObject shopLevelGo;
		bool flag = this.CTryGet<GameObject>("ShopLevelGo", out shopLevelGo);
		if (flag)
		{
			bool show = shopLevel >= 0 && state != EItemDebtState.None && !this.IsLocked;
			shopLevelGo.SetActive(show);
		}
		TextMeshProUGUI shopLevelText;
		bool flag2 = this.CTryGet<TextMeshProUGUI>("ShopLevelText", out shopLevelText);
		if (flag2)
		{
			shopLevelText.text = CommonUtils.GetTraditionalNumber((sbyte)(shopLevel + 1));
		}
		this.SetShopDebtMark(state);
	}

	// Token: 0x06002FDF RID: 12255 RVA: 0x001764FC File Offset: 0x001746FC
	public void RefreshExtraGoodsMark(bool show, MerchantExtraGoodsData.ExtraGoodsType extraGoodsType, sbyte seasonTemplateId = -1)
	{
		GameObject extraGoodsGo;
		CImage extraGoodsMark;
		TooltipInvoker extraGoodsMarkTip;
		bool flag = !this.CTryGet<GameObject>("ExtraGoodsGo", out extraGoodsGo) || !this.CTryGet<CImage>("ExtraGoodsMark", out extraGoodsMark) || !this.CTryGet<TooltipInvoker>("ExtraGoodsMarkTip", out extraGoodsMarkTip);
		if (!flag)
		{
			extraGoodsGo.SetActive(show);
			bool flag2 = !show;
			if (!flag2)
			{
				if (!true)
				{
				}
				string text;
				switch (extraGoodsType)
				{
				case MerchantExtraGoodsData.ExtraGoodsType.Normal:
					text = "shop_icon_jiageshangsheng";
					break;
				case MerchantExtraGoodsData.ExtraGoodsType.Capitalist:
					text = "shop_icon_Merchant tycoon";
					break;
				case MerchantExtraGoodsData.ExtraGoodsType.Season:
				{
					if (!true)
					{
					}
					string text2;
					switch (seasonTemplateId)
					{
					case 0:
						text2 = "ui_sp_icon_shopstate_2";
						break;
					case 1:
						text2 = "ui_sp_icon_shopstate_3";
						break;
					case 2:
						text2 = "ui_sp_icon_shopstate_4";
						break;
					case 3:
						text2 = "ui_sp_icon_shopstate_5";
						break;
					default:
						throw new ArgumentOutOfRangeException("seasonTemplateId", seasonTemplateId, null);
					}
					if (!true)
					{
					}
					text = text2;
					break;
				}
				default:
					throw new ArgumentOutOfRangeException("extraGoodsType", extraGoodsType, null);
				}
				if (!true)
				{
				}
				string spName = text;
				extraGoodsMark.SetSprite(spName, true, null);
				TooltipInvoker tooltipInvoker = extraGoodsMarkTip;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = new ArgumentBox();
				}
				if (!true)
				{
				}
				switch (extraGoodsType)
				{
				case MerchantExtraGoodsData.ExtraGoodsType.Normal:
					text = LocalStringManager.Get(LanguageKey.LK_Shop_ExtraGoods_Normal_Tip);
					break;
				case MerchantExtraGoodsData.ExtraGoodsType.Capitalist:
					text = LocalStringManager.GetFormat(LanguageKey.LK_Shop_ExtraGoods_Capitalist_Tip, ProfessionSkill.Instance[63].Name);
					break;
				case MerchantExtraGoodsData.ExtraGoodsType.Season:
					text = LocalStringManager.GetFormat(LanguageKey.LK_Shop_ExtraGoods_Season_Tip, LocalStringManager.Get(string.Format("LK_Season_{0}", seasonTemplateId)));
					break;
				default:
					throw new ArgumentOutOfRangeException("extraGoodsType", extraGoodsType, null);
				}
				if (!true)
				{
				}
				string tipContent = text;
				extraGoodsMarkTip.RuntimeParam.Set("arg0", tipContent);
			}
		}
	}

	// Token: 0x06002FE0 RID: 12256 RVA: 0x001766BC File Offset: 0x001748BC
	public void SetPriceState(EItemPriceState priceState, string tipContent = null)
	{
		CImage priceStateImage;
		GameObject priceRoot;
		bool flag = !this.CTryGet<CImage>("PriceState", out priceStateImage) || !this.CTryGet<GameObject>("PriceRoot", out priceRoot);
		if (!flag)
		{
			bool isShow = priceState > EItemPriceState.None;
			priceRoot.gameObject.SetActive(isShow);
			bool flag2 = !isShow;
			if (!flag2)
			{
				bool isAdd = priceState == EItemPriceState.Up;
				string sp = isAdd ? "ui_sp_arrow_shop_1" : "ui_sp_arrow_shop_0";
				priceStateImage.SetSprite(sp, false, null);
				int scaleY = isAdd ? 1 : -1;
				priceStateImage.transform.localScale = priceStateImage.transform.localScale.SetY((float)scaleY);
				TooltipInvoker tip;
				bool flag3 = !this.CTryGet<TooltipInvoker>("PriceStateTip", out tip);
				if (!flag3)
				{
					tip.Type = TipType.SingleDesc;
					string[] presetParam = tip.PresetParam;
					bool flag4 = presetParam == null || presetParam.Length != 1;
					if (flag4)
					{
						tip.PresetParam = new string[1];
					}
					tip.PresetParam[0] = tipContent;
				}
			}
		}
	}

	// Token: 0x06002FE1 RID: 12257 RVA: 0x001767B8 File Offset: 0x001749B8
	public void SetVillagerNeedMark(bool show)
	{
		GameObject villagerNeedGo;
		TooltipInvoker villagerNeedMarkTip;
		bool flag = !this.CTryGet<GameObject>("VillagerNeedGo", out villagerNeedGo) || !this.CTryGet<TooltipInvoker>("VillagerNeedMarkTip", out villagerNeedMarkTip);
		if (!flag)
		{
			villagerNeedGo.SetActive(show);
			bool flag2 = !show;
			if (!flag2)
			{
				villagerNeedMarkTip.Type = TipType.VillagerNeedItem;
				ItemKey itemKey = ItemKey.Invalid;
				itemKey.ItemType = this.Data.Key.ItemType;
				itemKey.TemplateId = this.Data.Key.TemplateId;
				villagerNeedMarkTip.RuntimeParam = EasyPool.Get<ArgumentBox>().Set<ItemKey>("itemKey", itemKey);
			}
		}
	}

	// Token: 0x0400229F RID: 8863
	[Header("排版元素间隔，用来确保边框重叠。会主动修改Horizontal Layout的spacing参数")]
	[SerializeField]
	private int spacing = -2;

	// Token: 0x040022A0 RID: 8864
	[Header("0行文本对应的表格高")]
	[SerializeField]
	private float zeroLineHeight = 48f;

	// Token: 0x040022A1 RID: 8865
	[Header("每行文本对应的表格高")]
	[SerializeField]
	private float textLineHeight = 22f;

	// Token: 0x040022A2 RID: 8866
	[Header("中文文本对应的表格高")]
	[SerializeField]
	private float cnRowHeight = 70f;

	// Token: 0x040022A3 RID: 8867
	[Header("表头：CommonTableHeadForItem (Prefab)")]
	[SerializeField]
	private CommonTableHead tableHead;

	// Token: 0x040022A4 RID: 8868
	[SerializeField]
	private bool isLocked;

	// Token: 0x040022A5 RID: 8869
	[SerializeField]
	private CButtonObsolete button;

	// Token: 0x040022A6 RID: 8870
	[SerializeField]
	[ReadOnly]
	private LocalStringManager.LanguageType language;

	// Token: 0x040022A7 RID: 8871
	[SerializeField]
	private TMP_Text[] texts;

	// Token: 0x040022A8 RID: 8872
	[SerializeField]
	private RectTransform container;

	// Token: 0x040022A9 RID: 8873
	[SerializeField]
	private RectTransform descField;

	// Token: 0x040022AA RID: 8874
	[SerializeField]
	private CImage equipStatus;

	// Token: 0x040022AB RID: 8875
	[SerializeField]
	private CImage readingStatus;

	// Token: 0x040022AC RID: 8876
	[SerializeField]
	private CImage loveStatus;

	// Token: 0x040022AD RID: 8877
	[SerializeField]
	private CImage lockStatus;

	// Token: 0x040022AE RID: 8878
	[SerializeField]
	private CImage lockSlash;

	// Token: 0x040022AF RID: 8879
	[SerializeField]
	private GameObject equipStatusGo;

	// Token: 0x040022B0 RID: 8880
	[SerializeField]
	private GameObject readingStatusGo;

	// Token: 0x040022B1 RID: 8881
	[SerializeField]
	private GameObject loveStatusGo;

	// Token: 0x040022B2 RID: 8882
	[SerializeField]
	private GameObject lockStatusGo;

	// Token: 0x040022B3 RID: 8883
	[SerializeField]
	private GameObject lockSlashGo;

	// Token: 0x040022B4 RID: 8884
	[SerializeField]
	private GameObject selectedGo;

	// Token: 0x040022B5 RID: 8885
	[SerializeField]
	private float iconSize = 44f;

	// Token: 0x040022B6 RID: 8886
	[SerializeField]
	private Sprite spritePreset;

	// Token: 0x040022B7 RID: 8887
	[SerializeField]
	private Sprite spriteEquip;

	// Token: 0x040022B8 RID: 8888
	[SerializeField]
	private Sprite spriteReading;

	// Token: 0x040022B9 RID: 8889
	[SerializeField]
	private Sprite spriteReadingDone;

	// Token: 0x040022BA RID: 8890
	[SerializeField]
	private Sprite spriteReferencing;

	// Token: 0x040022BB RID: 8891
	[SerializeField]
	private Sprite spriteReferencingDone;

	// Token: 0x040022BC RID: 8892
	[SerializeField]
	private Sprite spriteBookDone;

	// Token: 0x040022BD RID: 8893
	[SerializeField]
	private Sprite spriteLove;

	// Token: 0x040022BE RID: 8894
	[SerializeField]
	private Sprite spriteHate;

	// Token: 0x040022BF RID: 8895
	[SerializeField]
	private Sprite spriteLockStatus;

	// Token: 0x040022C0 RID: 8896
	[SerializeField]
	private CommonBookPageReadingStatus[] bookPageReadingStatus;

	// Token: 0x040022C1 RID: 8897
	[SerializeField]
	private GameObject[] activeWhileHoverObjects;

	// Token: 0x040022C2 RID: 8898
	[SerializeField]
	private Color pinkYellow;

	// Token: 0x040022C3 RID: 8899
	[SerializeField]
	private Color grey;

	// Token: 0x040022C4 RID: 8900
	[SerializeField]
	private CommonItemBack commonItemBack;

	// Token: 0x040022C7 RID: 8903
	private UnityAction _onMouseEnterCallback;

	// Token: 0x040022C8 RID: 8904
	private UnityAction _onMouseExitCallback;

	// Token: 0x040022C9 RID: 8905
	private bool _selected;

	// Token: 0x020016AF RID: 5807
	public enum EquipStatus : sbyte
	{
		// Token: 0x0400A8AD RID: 43181
		None = -1,
		// Token: 0x0400A8AE RID: 43182
		Preset,
		// Token: 0x0400A8AF RID: 43183
		Equip
	}

	// Token: 0x020016B0 RID: 5808
	public enum ReadingStatus : sbyte
	{
		// Token: 0x0400A8B1 RID: 43185
		None = -1,
		// Token: 0x0400A8B2 RID: 43186
		Reading,
		// Token: 0x0400A8B3 RID: 43187
		ReadingDone,
		// Token: 0x0400A8B4 RID: 43188
		Referencing,
		// Token: 0x0400A8B5 RID: 43189
		ReferencingDone,
		// Token: 0x0400A8B6 RID: 43190
		Done
	}

	// Token: 0x020016B1 RID: 5809
	public enum FavoriteStatus : sbyte
	{
		// Token: 0x0400A8B8 RID: 43192
		None = -1,
		// Token: 0x0400A8B9 RID: 43193
		Hate,
		// Token: 0x0400A8BA RID: 43194
		Love
	}
}
