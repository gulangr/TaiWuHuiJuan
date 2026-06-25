using System;
using Config;
using FrameWork;
using Game.Views.Combat;
using GameData.DLC.FiveLoong;
using GameData.Domains.Extra;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Organization;
using GameData.Serializer;
using GameData.Utilities;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x0200022F RID: 559
public class ItemView : Refers
{
	// Token: 0x0600239B RID: 9115 RVA: 0x001057E7 File Offset: 0x001039E7
	public static string GetGradeBack(sbyte grade)
	{
		return ItemView.ItemGradeIconBack.CheckIndex((int)(grade / 3)) ? ItemView.ItemGradeIconBack[(int)(grade / 3)] : string.Empty;
	}

	// Token: 0x0600239C RID: 9116 RVA: 0x00105808 File Offset: 0x00103A08
	public static string GetGradeIcon(sbyte grade)
	{
		return ItemView.ItemGradeBack.CheckIndex((int)grade) ? ItemView.ItemGradeBack[(int)grade] : string.Empty;
	}

	// Token: 0x0600239D RID: 9117 RVA: 0x00105825 File Offset: 0x00103A25
	public static string GetGradeText(sbyte grade)
	{
		return LocalStringManager.Get(string.Format("LK_ShortGrade_{0}", grade));
	}

	// Token: 0x0600239E RID: 9118 RVA: 0x0010583C File Offset: 0x00103A3C
	public static string GetLastGradeText(sbyte grade)
	{
		return (LocalStringManager.Get(string.Format("LK_Num_{0}", (int)(9 - grade))) + LocalStringManager.Get(LanguageKey.LK_Item_Grade)).SetColor(Colors.Instance.GradeColors[(int)grade]);
	}

	// Token: 0x1700037E RID: 894
	// (get) Token: 0x0600239F RID: 9119 RVA: 0x0010587A File Offset: 0x00103A7A
	// (set) Token: 0x060023A0 RID: 9120 RVA: 0x00105882 File Offset: 0x00103A82
	public ItemDisplayData Data { get; private set; }

	// Token: 0x1700037F RID: 895
	// (get) Token: 0x060023A1 RID: 9121 RVA: 0x0010588B File Offset: 0x00103A8B
	// (set) Token: 0x060023A2 RID: 9122 RVA: 0x00105893 File Offset: 0x00103A93
	public CricketView CricketView { get; private set; }

	// Token: 0x17000380 RID: 896
	// (get) Token: 0x060023A3 RID: 9123 RVA: 0x0010589C File Offset: 0x00103A9C
	// (set) Token: 0x060023A4 RID: 9124 RVA: 0x001058A4 File Offset: 0x00103AA4
	public JiaoEggView JiaoEggView { get; private set; }

	// Token: 0x17000381 RID: 897
	// (get) Token: 0x060023A5 RID: 9125 RVA: 0x001058B0 File Offset: 0x00103AB0
	private bool IsCricket
	{
		get
		{
			bool flag = this.Data != null;
			return flag && this.Data.Key.ItemType == 11;
		}
	}

	// Token: 0x17000382 RID: 898
	// (get) Token: 0x060023A6 RID: 9126 RVA: 0x001058E7 File Offset: 0x00103AE7
	// (set) Token: 0x060023A7 RID: 9127 RVA: 0x001058EF File Offset: 0x00103AEF
	public bool IsLocked { get; private set; }

	// Token: 0x060023A8 RID: 9128 RVA: 0x001058F8 File Offset: 0x00103AF8
	private void RefreshCricket()
	{
		bool isCricket = this.IsCricket;
		if (isCricket)
		{
			this.GetCricket();
		}
		else
		{
			this.ReturnCricket();
		}
	}

	// Token: 0x060023A9 RID: 9129 RVA: 0x00105924 File Offset: 0x00103B24
	private void GetCricket()
	{
		bool flag = !this.CricketView;
		if (flag)
		{
			this.CricketView = SingletonObject.getInstance<ItemViewPool>().Get<CricketView>();
			Transform cricketTransform = this.CricketView.transform;
			cricketTransform.SetParent(base.CGet<RectTransform>("CricketHolder"));
			cricketTransform.localPosition = Vector3.zero;
			cricketTransform.localScale = Vector3.one;
		}
		this.CricketView.SetCricketData(this.Data.CricketColorId, this.Data.CricketPartId, false, this.Data, false);
	}

	// Token: 0x060023AA RID: 9130 RVA: 0x001059B8 File Offset: 0x00103BB8
	private void ReturnCricket()
	{
		bool flag = this.CricketView;
		if (flag)
		{
			bool flag2 = !SingletonObject.IsDestroying;
			if (flag2)
			{
				this.CricketView.CGet<SkeletonGraphic>("SkeletonGraphic").enabled = true;
				this.CricketView.GetComponent<TooltipInvoker>().enabled = true;
				ItemViewPool instance = SingletonObject.getInstance<ItemViewPool>();
				if (instance != null)
				{
					instance.Return<CricketView>(this.CricketView);
				}
			}
			this.CricketView = null;
		}
	}

	// Token: 0x060023AB RID: 9131 RVA: 0x00105A30 File Offset: 0x00103C30
	public void SetIcon(string spName)
	{
		CImage icon = base.CGet<CImage>("Icon");
		icon.SetSprite(spName, true, null);
	}

	// Token: 0x060023AC RID: 9132 RVA: 0x00105A54 File Offset: 0x00103C54
	public TooltipInvoker GetMouseTip()
	{
		return base.GetComponent<TooltipInvoker>();
	}

	// Token: 0x060023AD RID: 9133 RVA: 0x00105A5C File Offset: 0x00103C5C
	public void SetData(ItemDisplayData data, bool detailView, int amount = -1, bool locked = false, bool showBookPageInfo = true, TooltipInvoker extraMouseTip = null, bool templateDataOnly = false, bool showUsingBg = true)
	{
		bool flag = !base.gameObject.activeSelf;
		if (flag)
		{
			base.gameObject.SetActive(true);
		}
		this.Data = data;
		this._isDetail = detailView;
		bool isInvalid = data.Key.Equals(ItemKey.Invalid);
		bool isCricket = this.IsCricket;
		bool isJiaoEgg = false;
		bool isEmptyTool = ItemTemplateHelper.IsEmptyTool(data.Key.ItemType, data.Key.TemplateId);
		bool isMiscResource = ItemTemplateHelper.IsMiscResource(data.Key.ItemType, data.Key.TemplateId);
		this.RefreshCricket();
		this.RefreshJiaoEggView();
		CImage icon = base.CGet<CImage>("Icon");
		TooltipInvoker tipDisplayer = base.GetComponent<TooltipInvoker>();
		PointerTrigger trigger = base.GetComponent<PointerTrigger>();
		amount = ((amount < 0) ? data.Amount : amount);
		bool showCount = amount > 1 && !isInvalid && !isEmptyTool;
		this.SetCount(showCount, isMiscResource, amount);
		icon.enabled = (!isCricket && !isJiaoEgg);
		bool enabled = icon.enabled;
		if (enabled)
		{
			bool flag2 = isInvalid;
			if (flag2)
			{
				string spName = this.ShowInvalidIcon ? "sp_11_goods_none" : string.Empty;
				icon.SetSprite(spName, false, null);
				icon.rectTransform.SetSize(ItemView.ForbidIconSize);
			}
			else
			{
				bool flag3 = isMiscResource;
				string spName2;
				if (flag3)
				{
					sbyte resourceType = ItemTemplateHelper.GetMiscResourceType(data.Key.ItemType, data.Key.TemplateId);
					spName2 = CombatDrops.GetIconName(amount, resourceType, true);
				}
				else
				{
					spName2 = ItemTemplateHelper.GetIcon(data.Key.ItemType, data.Key.TemplateId);
				}
				icon.SetSprite(spName2, false, null);
				Vector2 size = detailView ? ItemView.DetailIconSize : ItemView.BaseIconSize;
				bool flag4 = !this.FixedIconSize.Equals(Vector2.zero);
				if (flag4)
				{
					size = this.FixedIconSize;
				}
				icon.rectTransform.SetSize(size);
			}
		}
		bool showGrade = !isInvalid && !data.LoveTokenDataItem.IsValid && !isEmptyTool && !isMiscResource;
		int grade = isInvalid ? 0 : (isCricket ? this.CricketView.Level : ((int)ItemTemplateHelper.GetGrade(data.Key.ItemType, data.Key.TemplateId)));
		this.SetGrade(showGrade, (sbyte)grade);
		bool flag5 = this.Names.Contains("LoveToken");
		if (flag5)
		{
			base.CGet<GameObject>("LoveToken").SetActive(data.LoveTokenDataItem.IsValid);
		}
		tipDisplayer.enabled = (!isInvalid && this.EnbaleTip);
		bool flag6 = !isInvalid;
		if (flag6)
		{
			this.SetMouseTipDisplayer(tipDisplayer, showBookPageInfo, templateDataOnly);
			bool flag7 = extraMouseTip != null;
			if (flag7)
			{
				this.SetMouseTipDisplayer(extraMouseTip, showBookPageInfo, templateDataOnly);
			}
		}
		trigger.EnterEvent.RemoveAllListeners();
		trigger.EnterEvent.AddListener(delegate()
		{
			base.CGet<CImage>("EnterMark").gameObject.SetActive(true);
		});
		trigger.ExitEvent.RemoveAllListeners();
		trigger.ExitEvent.AddListener(delegate()
		{
			base.CGet<CImage>("EnterMark").gameObject.SetActive(false);
		});
		bool flag8 = this.Names.Contains("Name");
		if (flag8)
		{
			bool flag9 = ItemTemplateHelper.IsJiao(data.Key.ItemType, data.Key.TemplateId);
			if (flag9)
			{
				ExtraDomainMethod.AsyncCall.GetJiaoLoongNameRelatedData(null, data.Key, delegate(int offset, RawDataPool dataPool)
				{
					JiaoLoongNameRelatedData nameRelatedData = default(JiaoLoongNameRelatedData);
					Serializer.Deserialize(dataPool, offset, ref nameRelatedData);
					bool flag14 = ItemTemplateHelper.IsJiao(this.Data.Key.ItemType, this.Data.Key.TemplateId);
					if (flag14)
					{
						base.CGet<TextMeshProUGUI>("Name").text = nameRelatedData.GetName();
					}
				});
			}
			else
			{
				bool flag10 = isMiscResource;
				if (flag10)
				{
					sbyte resourceType2 = ItemTemplateHelper.GetMiscResourceType(data.Key.ItemType, data.Key.TemplateId);
					ResourceTypeItem resourceTypeConfig = ResourceType.Instance[resourceType2];
					base.CGet<TextMeshProUGUI>("Name").text = resourceTypeConfig.Name;
				}
				else
				{
					base.CGet<TextMeshProUGUI>("Name").text = (isInvalid ? ((UIManager.Instance.IsFocusElement(UIElement.MakeOld) || UIManager.Instance.IsFocusElement(UIElement.Make)) ? LocalStringManager.Get(LanguageKey.LK_Make_None_Tool) : LocalStringManager.Get(LanguageKey.LK_None)) : (isCricket ? this.CricketView.Name : ItemTemplateHelper.GetName(data.Key.ItemType, data.Key.TemplateId)));
				}
			}
		}
		if (detailView)
		{
			base.CGet<TextMeshProUGUI>("Type").transform.parent.gameObject.SetActive(!isInvalid);
			base.CGet<TextMeshProUGUI>("Type").text = (isInvalid ? string.Empty : LocalStringManager.Get(string.Format("LK_ItemType_{0}", data.Key.ItemType)));
			base.CGet<TextMeshProUGUI>("Value").text = (isInvalid ? "0" : data.Value.ToString());
			base.CGet<TextMeshProUGUI>("Weight").text = (isInvalid ? "0" : NumberFormatUtils.FormatItemWeight(data.Weight));
		}
		bool flag11 = !isInvalid;
		if (flag11)
		{
			this.SetLocked(locked);
		}
		bool showUsing = showUsingBg && (data.UsingType != ItemDisplayData.ItemUsingType.Invalid || data.IsReadingFinished);
		bool flag12 = this.Names.Contains("UsingBg");
		if (flag12)
		{
			GameObject usingBg = base.CGet<GameObject>("UsingBg");
			usingBg.SetActive(showUsing);
			bool flag13 = showUsing;
			if (flag13)
			{
				string typeName = data.GetUsingTypeName();
				TextMeshProUGUI componentInChildren = usingBg.GetComponentInChildren<TextMeshProUGUI>();
				if (componentInChildren != null)
				{
					componentInChildren.SetText(typeName, true);
				}
			}
		}
		this.RefreshThreeCorpseKeepingLegendaryBookMark(data);
		base.CGet<CImage>("CheckMark").gameObject.SetActive(false);
		this.SetHateAndLoveIconVisibility(false, false);
	}

	// Token: 0x060023AE RID: 9134 RVA: 0x00106000 File Offset: 0x00104200
	public void SetGrade(bool showGrade, sbyte grade = 0)
	{
		base.CGet<CImage>("IconBack").SetSprite(ItemView.GetGradeBack(grade), false, null);
		this.ApplyGradeBack(base.CGet<CImage>("GradeBack"), showGrade, grade);
		TextMeshProUGUI g;
		TextMeshProUGUI gradeLabel = this.CTryGet<TextMeshProUGUI>("Grade", out g) ? g : null;
		this.ApplyGradeLabel(gradeLabel, showGrade, grade);
	}

	// Token: 0x060023AF RID: 9135 RVA: 0x0010605C File Offset: 0x0010425C
	protected virtual void ApplyGradeLabel(TextMeshProUGUI gradeLabel, bool showGrade, sbyte grade)
	{
		bool flag = gradeLabel == null;
		if (!flag)
		{
			gradeLabel.gameObject.SetActive(true);
			gradeLabel.text = LocalStringManager.Get(string.Format("LK_ShortGrade_{0}", grade));
		}
	}

	// Token: 0x060023B0 RID: 9136 RVA: 0x001060A0 File Offset: 0x001042A0
	protected virtual void ApplyGradeBack(CImage gradeBack, bool showGrade, sbyte grade)
	{
		bool flag = gradeBack == null;
		if (!flag)
		{
			gradeBack.gameObject.SetActive(showGrade);
			bool flag2 = !showGrade;
			if (!flag2)
			{
				gradeBack.SetSprite(ItemView.GetGradeIcon(grade), false, null);
			}
		}
	}

	// Token: 0x060023B1 RID: 9137 RVA: 0x001060E4 File Offset: 0x001042E4
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
									tipDisplayer.RuntimeParam.Set("IsNew", this.IsNew);
								}
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x060023B2 RID: 9138 RVA: 0x00106548 File Offset: 0x00104748
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

	// Token: 0x060023B3 RID: 9139 RVA: 0x00106588 File Offset: 0x00104788
	public void SetCharId(int charId)
	{
		bool flag = this.Data.OwnerCharId != -1;
		if (flag)
		{
			charId = this.Data.OwnerCharId;
		}
		TooltipInvoker tipDisplayer = base.GetComponent<TooltipInvoker>();
		bool flag2 = tipDisplayer != null;
		if (flag2)
		{
			ArgumentBox runtimeParam = tipDisplayer.RuntimeParam;
			if (runtimeParam != null)
			{
				runtimeParam.Set("CharId", charId);
			}
		}
	}

	// Token: 0x060023B4 RID: 9140 RVA: 0x001065E4 File Offset: 0x001047E4
	public void SetMask(bool show)
	{
		bool flag = this.Names.Contains("Mask");
		if (flag)
		{
			base.CGet<GameObject>("Mask").SetActive(show);
		}
	}

	// Token: 0x060023B5 RID: 9141 RVA: 0x00106618 File Offset: 0x00104818
	public void SetCount(bool showCount, bool isMiscResource = false, int amount = 0)
	{
		TextMeshProUGUI count = base.CGet<TextMeshProUGUI>("Count");
		count.transform.parent.gameObject.SetActive(showCount);
		if (isMiscResource)
		{
			count.text = CommonUtils.GetDisplayStringForNum(amount, 100000);
		}
		else
		{
			bool flag = amount >= 1;
			if (flag)
			{
				count.text = this.GetDurabilityStr(amount);
			}
		}
	}

	// Token: 0x060023B6 RID: 9142 RVA: 0x0010667C File Offset: 0x0010487C
	public void SetSelectedCount(int selectedCount)
	{
		TextMeshProUGUI count = base.CGet<TextMeshProUGUI>("Count");
		count.text = string.Format("{0}/<color=#939393>{1}</color>", selectedCount, this.Data.Amount);
	}

	// Token: 0x060023B7 RID: 9143 RVA: 0x001066C0 File Offset: 0x001048C0
	public string GetDurabilityStr(int amount)
	{
		return (this.Data.MaxDurability == 0) ? string.Format("x{0}", amount).SetColor("lightwhite") : (this.Data.Durability.ToString().SetColor(ItemView.GetColorNameByDurable((int)this.Data.Durability, (int)this.Data.MaxDurability)) + string.Format("/{0}", this.Data.MaxDurability).SetColor("lightwhite"));
	}

	// Token: 0x060023B8 RID: 9144 RVA: 0x00106758 File Offset: 0x00104958
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

	// Token: 0x060023B9 RID: 9145 RVA: 0x00106790 File Offset: 0x00104990
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

	// Token: 0x060023BA RID: 9146 RVA: 0x001067C1 File Offset: 0x001049C1
	public void SetInteractable(bool interactable)
	{
		base.GetComponent<CButtonObsolete>().interactable = interactable;
		this.SetMask(!interactable);
		this.SetAmountPos(!interactable);
	}

	// Token: 0x060023BB RID: 9147 RVA: 0x001067E7 File Offset: 0x001049E7
	public void SetPrevSelected(bool prevSelected)
	{
		base.CGet<GameObject>("PrevMark").SetActive(prevSelected);
	}

	// Token: 0x060023BC RID: 9148 RVA: 0x001067FC File Offset: 0x001049FC
	public void SetEnterEvent(UnityAction onEnter)
	{
		PointerTrigger trigger = base.GetComponent<PointerTrigger>();
		trigger.EnterEvent.RemoveAllListeners();
		trigger.EnterEvent.AddListener(delegate()
		{
			base.CGet<CImage>("EnterMark").gameObject.SetActive(true);
		});
		bool flag = onEnter != null;
		if (flag)
		{
			trigger.EnterEvent.AddListener(onEnter);
		}
	}

	// Token: 0x060023BD RID: 9149 RVA: 0x0010684C File Offset: 0x00104A4C
	public void SetExitEvent(UnityAction onEnter)
	{
		PointerTrigger trigger = base.GetComponent<PointerTrigger>();
		trigger.ExitEvent.RemoveAllListeners();
		trigger.ExitEvent.AddListener(delegate()
		{
			base.CGet<CImage>("EnterMark").gameObject.SetActive(false);
		});
		bool flag = onEnter != null;
		if (flag)
		{
			trigger.ExitEvent.AddListener(onEnter);
		}
	}

	// Token: 0x060023BE RID: 9150 RVA: 0x0010689A File Offset: 0x00104A9A
	public void SetHighLight(bool show)
	{
		base.CGet<CImage>("CheckMark").gameObject.SetActive(show);
	}

	// Token: 0x060023BF RID: 9151 RVA: 0x001068B4 File Offset: 0x00104AB4
	public void SetLocked(bool locked)
	{
		this.IsLocked = locked;
		if (locked)
		{
			this.ShowInteractionStateLocked();
		}
		else
		{
			this.HideInteractionState();
		}
	}

	// Token: 0x060023C0 RID: 9152 RVA: 0x001068DF File Offset: 0x00104ADF
	public void SetSelectState(bool select)
	{
		base.CGet<GameObject>("SelectStatus").SetActive(select);
	}

	// Token: 0x060023C1 RID: 9153 RVA: 0x001068F4 File Offset: 0x00104AF4
	public static string GetColorNameByDurable(int durable, int maxDurable)
	{
		bool flag = durable >= maxDurable;
		string result;
		if (flag)
		{
			result = "lightgreen";
		}
		else
		{
			bool flag2 = (float)durable >= 0.3f * (float)maxDurable;
			if (flag2)
			{
				result = "lightyellow";
			}
			else
			{
				result = "red";
			}
		}
		return result;
	}

	// Token: 0x060023C2 RID: 9154 RVA: 0x00106939 File Offset: 0x00104B39
	private TextMeshProUGUI GetInteractionStateText()
	{
		return this.Names.Contains("Attainment") ? base.CGet<TextMeshProUGUI>("Attainment") : null;
	}

	// Token: 0x060023C3 RID: 9155 RVA: 0x0010695C File Offset: 0x00104B5C
	public void ShowInteractionStateAddition(int value)
	{
		TextMeshProUGUI interactionStateText = this.GetInteractionStateText();
		bool flag = null != interactionStateText;
		if (flag)
		{
			interactionStateText.text = LocalStringManager.GetFormat(LanguageKey.LK_CombatSkill_SpecialBreak_ConvertToExp_Item_ProgressValue, value).SetColor("brightblue");
			interactionStateText.transform.parent.gameObject.SetActive(true);
		}
		this.SetAmountPos(true);
	}

	// Token: 0x060023C4 RID: 9156 RVA: 0x001069C0 File Offset: 0x00104BC0
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
		this.SetAmountPos(true);
	}

	// Token: 0x060023C5 RID: 9157 RVA: 0x00106A54 File Offset: 0x00104C54
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

	// Token: 0x060023C6 RID: 9158 RVA: 0x00106A94 File Offset: 0x00104C94
	public void HideInteractionState()
	{
		bool isLocked = this.IsLocked;
		if (!isLocked)
		{
			TextMeshProUGUI interactionStateText = this.GetInteractionStateText();
			bool flag = null != interactionStateText;
			if (flag)
			{
				interactionStateText.transform.parent.gameObject.SetActive(false);
			}
			this.SetInteractable(true);
			this.SetAmountPos(false);
		}
	}

	// Token: 0x060023C7 RID: 9159 RVA: 0x00106AE8 File Offset: 0x00104CE8
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

	// Token: 0x060023C8 RID: 9160 RVA: 0x00106B44 File Offset: 0x00104D44
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

	// Token: 0x060023C9 RID: 9161 RVA: 0x00106B90 File Offset: 0x00104D90
	public void SetAmountPos(bool isUp)
	{
		TextMeshProUGUI amountText = base.CGet<TextMeshProUGUI>("Count");
		RectTransform amountRect = amountText.transform.parent.GetComponent<RectTransform>();
		amountText.alignment = (isUp ? TextAlignmentOptions.Right : TextAlignmentOptions.Left);
		amountRect.pivot = (isUp ? Vector2.one : Vector2.zero);
		amountRect.anchorMax = (isUp ? Vector2.one : Vector2.up);
		amountRect.anchorMin = (isUp ? Vector2.one : Vector2.up);
		amountRect.anchoredPosition = (isUp ? (this._isDetail ? ItemView.DetailAmountUpPos : ItemView.SimpleAmountUpPos) : (this._isDetail ? ItemView.DetailAmountOriginPos : ItemView.SimpleAmountOriginPos));
	}

	// Token: 0x060023CA RID: 9162 RVA: 0x00106C48 File Offset: 0x00104E48
	public void SetSelectedOrder(bool show, int order)
	{
		bool flag = this.Names.Contains("OrderNumber");
		if (flag)
		{
			TextMeshProUGUI orderNumber = base.CGet<TextMeshProUGUI>("OrderNumber");
			orderNumber.transform.parent.gameObject.SetActive(show);
			order++;
			orderNumber.text = order.ToString();
		}
	}

	// Token: 0x060023CB RID: 9163 RVA: 0x00106CA4 File Offset: 0x00104EA4
	public void ShowDurability()
	{
		string str = this.GetDurabilityStr(int.MaxValue);
		TextMeshProUGUI count = base.CGet<TextMeshProUGUI>("Count");
		count.text = str;
		count.transform.parent.gameObject.SetActive(true);
	}

	// Token: 0x060023CC RID: 9164 RVA: 0x00106CEC File Offset: 0x00104EEC
	public void SetUsedDurability(short usedDurability = 0, bool forceShow = false)
	{
		bool flag = this.Names.Contains("UsedDurability");
		if (flag)
		{
			TextMeshProUGUI text = base.CGet<TextMeshProUGUI>("UsedDurability");
			bool show = usedDurability > 0 || forceShow;
			text.transform.parent.gameObject.SetActive(show);
			bool flag2 = show;
			if (flag2)
			{
				text.text = ((usedDurability >= this.Data.Durability) ? LocalStringManager.Get(LanguageKey.LK_Tool_Used_Durability_Over) : LocalStringManager.GetFormat(LanguageKey.LK_Tool_Used_Durability, string.Format("-{0}", usedDurability)));
			}
		}
	}

	// Token: 0x060023CD RID: 9165 RVA: 0x00106D7C File Offset: 0x00104F7C
	public void SetHateAndLoveIconVisibility(bool hateVisibility, bool loveVisibility)
	{
		bool flag = this.Names.Contains("HateIcon");
		if (flag)
		{
			base.CGet<GameObject>("HateIcon").SetActive(hateVisibility);
		}
		bool flag2 = this.Names.Contains("LoveIcon");
		if (flag2)
		{
			base.CGet<GameObject>("LoveIcon").SetActive(loveVisibility);
		}
	}

	// Token: 0x060023CE RID: 9166 RVA: 0x00106DDA File Offset: 0x00104FDA
	public void SetPointTriggerEnabled(bool enabled)
	{
		base.GetComponent<PointerTrigger>().enabled = enabled;
	}

	// Token: 0x060023CF RID: 9167 RVA: 0x00106DEA File Offset: 0x00104FEA
	public void OnItemHide()
	{
		this.ReturnCricket();
		this.ReturnJiaoEggView();
	}

	// Token: 0x060023D0 RID: 9168 RVA: 0x00106DFC File Offset: 0x00104FFC
	public void SetTreasuryTradeMark(ETreasuryOperation operation)
	{
		CImage image;
		bool flag = !this.CTryGet<CImage>("SettlementTreasuryMark", out image);
		if (!flag)
		{
			image.raycastTarget = false;
			TooltipInvoker tip = image.GetComponent<TooltipInvoker>();
			bool flag2 = tip;
			if (flag2)
			{
				tip.enabled = false;
			}
			bool flag3 = operation == ETreasuryOperation.Invalid;
			if (flag3)
			{
				image.gameObject.SetActive(false);
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
				image.gameObject.SetActive(true);
				image.SetSprite(spName, false, null);
			}
		}
	}

	// Token: 0x060023D1 RID: 9169 RVA: 0x00106EC8 File Offset: 0x001050C8
	public void SetShopDebtMark(EItemDebtState state)
	{
		CImage image;
		bool flag = !this.CTryGet<CImage>("SettlementTreasuryMark", out image);
		if (!flag)
		{
			TooltipInvoker tip = image.GetComponent<TooltipInvoker>();
			bool flag2 = state == EItemDebtState.None || this.IsLocked;
			if (flag2)
			{
				image.gameObject.SetActive(false);
				image.raycastTarget = false;
				bool flag3 = tip;
				if (flag3)
				{
					tip.enabled = false;
				}
			}
			else
			{
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
					text = "sp_11_debts_1";
				}
				else
				{
					text = "sp_11_debts_0";
				}
				if (!true)
				{
				}
				string spName = text;
				image.gameObject.SetActive(true);
				image.SetSprite(spName, false, null);
				image.raycastTarget = true;
				bool flag4 = tip;
				if (flag4)
				{
					tip.enabled = true;
					LanguageKey strKey = (state == EItemDebtState.Add) ? LanguageKey.LK_Shop_Item_Debt_Add : LanguageKey.LK_Shop_Item_Debt_Repay;
					string[] presetParam = tip.PresetParam;
					bool flag5 = presetParam == null || presetParam.Length != 1;
					if (flag5)
					{
						tip.PresetParam = new string[1];
					}
					tip.PresetParam[0] = LocalStringManager.Get(strKey).ColorReplace();
				}
			}
		}
	}

	// Token: 0x060023D2 RID: 9170 RVA: 0x00106FFC File Offset: 0x001051FC
	public void SetShopLevelMark(EItemDebtState state, int shopLevel)
	{
		CImage shopLevelImage;
		bool flag = this.CTryGet<CImage>("ShopLevelImage", out shopLevelImage);
		if (flag)
		{
			bool show = shopLevel >= 0 && state != EItemDebtState.None && !this.IsLocked;
			shopLevelImage.gameObject.SetActive(show);
			bool flag2 = show;
			if (flag2)
			{
				shopLevelImage.SetSprite(string.Format("sp_11_shoplevel_{0}", shopLevel), false, null);
			}
		}
		TextMeshProUGUI shopLevelText;
		bool flag3 = this.CTryGet<TextMeshProUGUI>("ShopLevelText", out shopLevelText);
		if (flag3)
		{
			shopLevelText.text = CommonUtils.GetTraditionalNumber((sbyte)(shopLevel + 1));
		}
	}

	// Token: 0x060023D3 RID: 9171 RVA: 0x00107084 File Offset: 0x00105284
	public void RefreshExtraGoodsMark(bool show, MerchantExtraGoodsData.ExtraGoodsType extraGoodsType, sbyte seasonTemplateId = -1)
	{
		GameObject image;
		bool flag = !this.CTryGet<GameObject>("ExtraGoodsIcon", out image);
		if (!flag)
		{
			image.SetActive(show);
			if (show)
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
						text2 = "sp_goods_icon_siji_0";
						break;
					case 1:
						text2 = "sp_goods_icon_siji_1";
						break;
					case 2:
						text2 = "sp_goods_icon_siji_2";
						break;
					case 3:
						text2 = "sp_goods_icon_siji_3";
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
				image.GetComponent<CImage>().SetSprite(spName, true, null);
				base.CGet<CImage>("IconBack").SetSprite("sp_11_goods_teshu_0", false, null);
				TooltipInvoker tip = image.GetComponent<TooltipInvoker>();
				TooltipInvoker tooltipInvoker = tip;
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
				tip.RuntimeParam.Set("arg0", tipContent);
			}
		}
	}

	// Token: 0x060023D4 RID: 9172 RVA: 0x00107244 File Offset: 0x00105444
	private void RefreshThreeCorpseKeepingLegendaryBookMark(ItemDisplayData data)
	{
		bool flag = !this.Names.Contains("IsThreeCorpseKeepingLegendaryBook");
		if (!flag)
		{
			base.CGet<GameObject>("IsThreeCorpseKeepingLegendaryBook").SetActive(data.IsThreeCorpseKeepingLegendaryBook);
		}
	}

	// Token: 0x060023D5 RID: 9173 RVA: 0x00107284 File Offset: 0x00105484
	public void SetResourceTip(string name, bool isTaiwu)
	{
		bool isMiscResource = ItemTemplateHelper.IsMiscResource(this.Data.Key.ItemType, this.Data.Key.TemplateId);
		bool flag = !isMiscResource;
		if (!flag)
		{
			TooltipInvoker tipDisplayer = base.GetComponent<TooltipInvoker>();
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

	// Token: 0x060023D6 RID: 9174 RVA: 0x001073A4 File Offset: 0x001055A4
	public void SetPoisonCondenseCount(int initCount, int curCount, int maxCount)
	{
		TextMeshProUGUI count = base.CGet<TextMeshProUGUI>("Count");
		string curCountColor = (initCount == curCount) ? "pinkyellow" : ((curCount < maxCount) ? "brightred" : "brightblue");
		count.text = string.Format("{0}/{1}", curCount.ToString().SetColor(curCountColor), maxCount);
		count.transform.parent.gameObject.SetActive(true);
		this.SetAmountPos(true);
		bool flag = curCount == maxCount;
		if (flag)
		{
			base.CGet<CImage>("IconBack").SetSprite("sp_11_goods_teshu_1", false, null);
		}
		else
		{
			sbyte grade = this.Data.Key.IsValid() ? ItemTemplateHelper.GetGrade(8, this.Data.Key.TemplateId) : 0;
			base.CGet<CImage>("IconBack").SetSprite(ItemView.GetGradeBack(grade), false, null);
		}
	}

	// Token: 0x060023D7 RID: 9175 RVA: 0x0010748C File Offset: 0x0010568C
	private void RefreshJiaoEggView()
	{
	}

	// Token: 0x060023D8 RID: 9176 RVA: 0x00107490 File Offset: 0x00105690
	private void GetJiaoEggView()
	{
		bool flag = !this.JiaoEggView;
		if (flag)
		{
			this.JiaoEggView = SingletonObject.getInstance<ItemViewPool>().Get<JiaoEggView>();
			bool flag2 = this.JiaoEggView;
			if (flag2)
			{
				Transform trans = this.JiaoEggView.transform;
				RectTransform holder;
				bool flag3 = this.CTryGet<RectTransform>("JiaoEggHolder", out holder);
				if (flag3)
				{
					trans.SetParent(holder);
				}
				else
				{
					trans.SetParent(base.CGet<RectTransform>("CricketHolder").parent);
				}
				trans.localPosition = Vector3.zero;
				trans.localScale = Vector3.one;
				trans.localRotation = Quaternion.identity;
			}
		}
	}

	// Token: 0x060023D9 RID: 9177 RVA: 0x0010753C File Offset: 0x0010573C
	private void ReturnJiaoEggView()
	{
		bool flag = this.JiaoEggView;
		if (flag)
		{
			bool flag2 = !SingletonObject.IsDestroying;
			if (flag2)
			{
				ItemViewPool instance = SingletonObject.getInstance<ItemViewPool>();
				if (instance != null)
				{
					instance.Return<JiaoEggView>(this.JiaoEggView);
				}
			}
			this.JiaoEggView = null;
		}
	}

	// Token: 0x060023DA RID: 9178 RVA: 0x00107588 File Offset: 0x00105788
	public void SetVillagerNeedMark(bool showBg, bool showIcon = true)
	{
		GameObject bg;
		bool flag = !this.CTryGet<GameObject>("VillagerNeedBg", out bg);
		if (!flag)
		{
			bg.SetActive(showBg);
			bool flag2 = !showBg;
			if (!flag2)
			{
				bool flag3 = bg.transform.childCount <= 0;
				if (!flag3)
				{
					bg.transform.GetChild(0).gameObject.SetActive(showIcon);
					bool flag4 = !showIcon;
					if (!flag4)
					{
						TooltipInvoker tip = bg.GetComponentInChildren<TooltipInvoker>();
						bool flag5 = !tip;
						if (!flag5)
						{
							tip.Type = TipType.VillagerNeedItem;
							ItemKey itemKey = ItemKey.Invalid;
							itemKey.ItemType = this.Data.Key.ItemType;
							itemKey.TemplateId = this.Data.Key.TemplateId;
							tip.RuntimeParam = EasyPool.Get<ArgumentBox>().Set<ItemKey>("itemKey", itemKey);
						}
					}
				}
			}
		}
	}

	// Token: 0x060023DB RID: 9179 RVA: 0x00107674 File Offset: 0x00105874
	public void SetExpDisplay(int count)
	{
		this.SetIcon("sp_tuan_lilian");
		this.SetCount(true, true, count);
		this.SetGrade(false, 0);
	}

	// Token: 0x04001B24 RID: 6948
	private static readonly string[] ItemGradeIconBack = new string[]
	{
		"sp_11_goods_2",
		"sp_11_goods_1",
		"sp_11_goods_0"
	};

	// Token: 0x04001B25 RID: 6949
	private static readonly string[] ItemGradeBack = new string[]
	{
		"sp_icon_pinji_0",
		"sp_icon_pinji_1",
		"sp_icon_pinji_2",
		"sp_icon_pinji_3",
		"sp_icon_pinji_4",
		"sp_icon_pinji_5",
		"sp_icon_pinji_6",
		"sp_icon_pinji_7",
		"sp_icon_pinji_8"
	};

	// Token: 0x04001B27 RID: 6951
	public Vector2 FixedIconSize;

	// Token: 0x04001B28 RID: 6952
	public static readonly Vector2 BaseIconSize = Vector2.one * 76f;

	// Token: 0x04001B29 RID: 6953
	public static readonly Vector2 DetailIconSize = Vector2.one * 69f;

	// Token: 0x04001B2A RID: 6954
	public static readonly Vector2 ForbidIconSize = Vector2.one * 88f;

	// Token: 0x04001B2B RID: 6955
	private const float SimpleItemEquipMarkPosY = 4f;

	// Token: 0x04001B2C RID: 6956
	private const float DetailItemEquipMarkPosY = 13.2f;

	// Token: 0x04001B2D RID: 6957
	private static readonly Vector2 SimpleAmountOriginPos = new Vector2(16.5f, -105.4f);

	// Token: 0x04001B2E RID: 6958
	private static readonly Vector2 SimpleAmountUpPos = new Vector2(-8f, -8f);

	// Token: 0x04001B2F RID: 6959
	private static readonly Vector2 DetailAmountOriginPos = new Vector2(11.2f, -94.7f);

	// Token: 0x04001B30 RID: 6960
	private static readonly Vector2 DetailAmountUpPos = new Vector2(-162f, -14f);

	// Token: 0x04001B31 RID: 6961
	private bool _isDetail;

	// Token: 0x04001B34 RID: 6964
	public bool ShowInvalidIcon = true;

	// Token: 0x04001B36 RID: 6966
	[HideInInspector]
	public bool IsNew;

	// Token: 0x04001B37 RID: 6967
	[HideInInspector]
	public bool EnbaleTip = true;
}
