using System;
using Game.Views.Combat;
using GameData.DLC.FiveLoong;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using UnityEngine;

// Token: 0x0200032C RID: 812
public class CommonItemBack : Refers
{
	// Token: 0x1700052F RID: 1327
	// (get) Token: 0x06002F2E RID: 12078 RVA: 0x00172846 File Offset: 0x00170A46
	private CButtonObsolete Button
	{
		get
		{
			return base.CGet<CButtonObsolete>("Button");
		}
	}

	// Token: 0x17000530 RID: 1328
	// (get) Token: 0x06002F2F RID: 12079 RVA: 0x00172853 File Offset: 0x00170A53
	// (set) Token: 0x06002F30 RID: 12080 RVA: 0x0017285B File Offset: 0x00170A5B
	public bool Disable { get; private set; }

	// Token: 0x17000531 RID: 1329
	// (get) Token: 0x06002F31 RID: 12081 RVA: 0x00172864 File Offset: 0x00170A64
	// (set) Token: 0x06002F32 RID: 12082 RVA: 0x0017286C File Offset: 0x00170A6C
	public ItemDisplayData Data { get; private set; }

	// Token: 0x17000532 RID: 1330
	// (get) Token: 0x06002F33 RID: 12083 RVA: 0x00172878 File Offset: 0x00170A78
	private bool IsCricket
	{
		get
		{
			bool flag = this.Data != null;
			return flag && this.Data.Key.ItemType == 11;
		}
	}

	// Token: 0x06002F34 RID: 12084 RVA: 0x001728B0 File Offset: 0x00170AB0
	public void SetData(ItemDisplayData data, int amount = -1)
	{
		this.Data = data;
		bool flag = !base.gameObject.activeSelf;
		if (flag)
		{
			base.gameObject.SetActive(true);
		}
		bool isInvalid = data.Key.Equals(ItemKey.Invalid);
		bool isCricket = this.IsCricket;
		bool? flag2;
		if (data == null)
		{
			flag2 = null;
		}
		else
		{
			JiaoLoongDisplayData jiaoLoongDisplayData = data.JiaoLoongDisplayData;
			flag2 = ((jiaoLoongDisplayData != null) ? new bool?(jiaoLoongDisplayData.IsEgg) : null);
		}
		bool? flag3 = flag2;
		bool isJiaoEgg = flag3.GetValueOrDefault();
		bool isEmptyTool = ItemTemplateHelper.IsEmptyTool(data.Key.ItemType, data.Key.TemplateId);
		bool isMiscResource = ItemTemplateHelper.IsMiscResource(data.Key.ItemType, data.Key.TemplateId);
		CImage icon = base.CGet<CImage>("Icon");
		TooltipInvoker tipDisplayer = base.GetComponent<TooltipInvoker>();
		PointerTrigger trigger = base.GetComponent<PointerTrigger>();
		amount = ((amount < 0) ? data.Amount : amount);
		bool flag4 = amount > 1 && !isInvalid && !isEmptyTool;
		icon.enabled = (!isCricket && !isJiaoEgg);
		base.CGet<CImage>("GradeBack").gameObject.SetActive(true);
		bool enabled = icon.enabled;
		if (enabled)
		{
			bool flag5 = isInvalid;
			if (flag5)
			{
				string spName = this.ShowInvalidIcon ? "sp_11_goods_none" : string.Empty;
				icon.SetSprite(spName, false, null);
			}
			else
			{
				bool flag6 = isMiscResource;
				string spName2;
				if (flag6)
				{
					sbyte resourceType = ItemTemplateHelper.GetMiscResourceType(data.Key.ItemType, data.Key.TemplateId);
					spName2 = CombatDrops.GetIconName(amount, resourceType, true);
				}
				else
				{
					ItemKey key = data.Key;
					bool flag7 = key.ItemType == 12 && key.TemplateId == 8;
					if (flag7)
					{
						spName2 = "sp_tuan_lilian";
						base.CGet<CImage>("GradeBack").gameObject.SetActive(false);
					}
					else
					{
						spName2 = ItemTemplateHelper.GetIcon(data.Key.ItemType, data.Key.TemplateId);
					}
				}
				icon.SetSprite(spName2, false, null);
			}
		}
		int grade = (int)(isInvalid ? 0 : (isCricket ? new ValueTuple<short, short>(data.CricketColorId, data.CricketPartId).CalcCricketGrade() : ItemTemplateHelper.GetGrade(data.Key.ItemType, data.Key.TemplateId)));
		string gradeBack = CommonItemBack.GetGradeBack((sbyte)grade);
		base.CGet<CImage>("GradeBack").SetSprite(gradeBack, false, null);
	}

	// Token: 0x06002F35 RID: 12085 RVA: 0x00172B22 File Offset: 0x00170D22
	public void SetInteractable(bool interactable)
	{
		this.Button.interactable = interactable;
	}

	// Token: 0x06002F36 RID: 12086 RVA: 0x00172B34 File Offset: 0x00170D34
	public void SetDisable(bool disable)
	{
		this.Disable = disable;
		base.CGet<GameObject>("DisableBack").SetActive(disable);
		base.CGet<GameObject>("Disable").SetActive(disable);
		HSVStyleRoot hsvStyleRoot = base.CGet<HSVStyleRoot>("HSVStyleRoot");
		if (disable)
		{
			hsvStyleRoot.SetDefaultGrayAndBlack();
		}
		else
		{
			hsvStyleRoot.SetDefault();
		}
		this.SetInteractable(!disable);
	}

	// Token: 0x06002F37 RID: 12087 RVA: 0x00172B9A File Offset: 0x00170D9A
	public static string GetGradeBack(sbyte grade)
	{
		return "ui_sp_base_goods_" + grade.ToString();
	}

	// Token: 0x0400224E RID: 8782
	public bool ShowInvalidIcon = true;
}
