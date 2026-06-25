using System;
using Config;
using Game.Views.Combat;
using GameData.DLC.FiveLoong;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameDataExtensions;
using TMPro;
using UnityEngine;

namespace Game.Components.ListStyleGeneralScroll.CellContent
{
	// Token: 0x02000EBF RID: 3775
	public class ItemIconAndNameCell : MonoBehaviour, ICellContent<ITradeableContent>, ICellContent
	{
		// Token: 0x0600AEF3 RID: 44787 RVA: 0x004FB284 File Offset: 0x004F9484
		public void SetData(ITradeableContent data)
		{
			bool isInvalid = data.Key.Equals(ItemKey.Invalid);
			bool isCricket = data.Key.ItemType == 11;
			bool? flag;
			if (data == null)
			{
				flag = null;
			}
			else
			{
				JiaoLoongDisplayData jiaoLoongDisplayData = data.JiaoLoongDisplayData;
				flag = ((jiaoLoongDisplayData != null) ? new bool?(jiaoLoongDisplayData.IsEgg) : null);
			}
			bool? flag2 = flag;
			bool isJiaoEgg = flag2.GetValueOrDefault();
			bool isMiscResource = ItemTemplateHelper.IsMiscResource(data.Key.ItemType, data.Key.TemplateId);
			this.label.text = data.GetName(false);
			bool flag3 = data.CharacterId != -1;
			if (flag3)
			{
				this.icon.sprite = ((data.Gender == 1) ? this.male : this.female);
			}
			else
			{
				bool flag4 = isInvalid;
				if (flag4)
				{
					this.icon.SetSprite("ui9_icon_item_empty_big", false, null);
				}
				else
				{
					bool flag5 = isMiscResource;
					string spName;
					if (flag5)
					{
						sbyte resourceType = ItemTemplateHelper.GetMiscResourceType(data.Key.ItemType, data.Key.TemplateId);
						spName = CombatDrops.GetIconName(data.Amount, resourceType, true);
					}
					else
					{
						ItemKey key = data.Key;
						bool flag6 = key.ItemType == 12 && key.TemplateId == 8;
						if (flag6)
						{
							spName = CommonUtils.GetResOrExpIcon(-1, true);
						}
						else
						{
							bool flag7 = isCricket;
							if (flag7)
							{
								CricketPartsItem colorConfig = CricketParts.Instance[data.CricketColorId];
								spName = colorConfig.Icon;
							}
							else
							{
								bool flag8 = isJiaoEgg;
								if (flag8)
								{
									spName = (data.JiaoLoongDisplayData.Jiao.Gender ? "ui9_icon_jiao_egg_gender_1" : "ui9_icon_jiao_egg_gender_0");
								}
								else
								{
									spName = ItemTemplateHelper.GetIcon(data.Key.ItemType, data.Key.TemplateId);
								}
							}
						}
					}
					this.icon.SetSprite(spName, false, null);
				}
			}
			int grade = (int)(isInvalid ? ((data.CharacterId != -1) ? data.Grade : 0) : (isCricket ? new ValueTuple<short, short>(data.CricketColorId, data.CricketPartId).CalcCricketGrade() : ItemTemplateHelper.GetGrade(data.Key.ItemType, data.Key.TemplateId)));
			this.gradeIcon.SetSprite("ui9_icon_item_grade_" + grade.ToString(), false, null);
			bool flag9 = data.UsingType == ItemDisplayData.ItemUsingType.EquipmentPlaned;
			if (flag9)
			{
				this.itemStateIcon.SetSprite("ui9_icon_item_state_equip_0", false, null);
				this.itemStateIcon.gameObject.SetActive(true);
				TooltipInvoker tips = this.itemStateIcon.GetComponent<TooltipInvoker>();
				tips.PresetParam = new string[]
				{
					LanguageKey.LK_ItemUsingType_EquipmentPlaned.Tr()
				};
				tips.enabled = true;
			}
			else
			{
				bool flag10 = data.UsingType == ItemDisplayData.ItemUsingType.Reading;
				if (flag10)
				{
					this.itemStateIcon.SetSprite("ui9_icon_item_state_read_0", false, null);
					this.itemStateIcon.gameObject.SetActive(true);
					TooltipInvoker tips2 = this.itemStateIcon.GetComponent<TooltipInvoker>();
					tips2.PresetParam = new string[]
					{
						LanguageKey.LK_ItemUsingType_Reading.Tr()
					};
					tips2.enabled = true;
				}
				else
				{
					bool flag11 = data.UsingType == ItemDisplayData.ItemUsingType.Referring;
					if (flag11)
					{
						this.itemStateIcon.SetSprite("ui9_icon_item_state_read_1", false, null);
						this.itemStateIcon.gameObject.SetActive(true);
						TooltipInvoker tips3 = this.itemStateIcon.GetComponent<TooltipInvoker>();
						tips3.PresetParam = new string[]
						{
							LanguageKey.LK_ItemUsingType_Referring.Tr()
						};
						tips3.enabled = true;
					}
					else
					{
						bool isReadingFinished = data.IsReadingFinished;
						if (isReadingFinished)
						{
							this.itemStateIcon.SetSprite("ui9_icon_item_state_read_2", false, null);
							this.itemStateIcon.gameObject.SetActive(true);
							TooltipInvoker tips4 = this.itemStateIcon.GetComponent<TooltipInvoker>();
							tips4.PresetParam = new string[]
							{
								LanguageKey.LK_ItemUsingType_ReadingFinished.Tr()
							};
							tips4.enabled = true;
						}
						else
						{
							bool isThreeCorpseKeepingLegendaryBook = data.IsThreeCorpseKeepingLegendaryBook;
							if (isThreeCorpseKeepingLegendaryBook)
							{
								this.itemStateIcon.SetSprite("ui9_icon_keeping_book", false, null);
								this.itemStateIcon.gameObject.SetActive(true);
								TooltipInvoker tips5 = this.itemStateIcon.GetComponent<TooltipInvoker>();
								tips5.PresetParam = new string[]
								{
									LanguageKey.LK_ItemDisplayData_ThreeCorpseKeepingLegendaryBook.Tr()
								};
								tips5.enabled = true;
							}
							else
							{
								this.itemStateIcon.gameObject.SetActive(false);
							}
						}
					}
				}
			}
		}

		// Token: 0x04008754 RID: 34644
		[SerializeField]
		private CImage icon;

		// Token: 0x04008755 RID: 34645
		[SerializeField]
		private CImage gradeIcon;

		// Token: 0x04008756 RID: 34646
		[SerializeField]
		private TextMeshProUGUI label;

		// Token: 0x04008757 RID: 34647
		[SerializeField]
		private CImage itemStateIcon;

		// Token: 0x04008758 RID: 34648
		[Header("男女肖像icon，显示俘虏时用")]
		[SerializeField]
		private Sprite male;

		// Token: 0x04008759 RID: 34649
		[Header("男女肖像icon，显示俘虏时用")]
		[SerializeField]
		private Sprite female;
	}
}
