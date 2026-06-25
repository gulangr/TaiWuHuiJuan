using System;
using Config;
using Game.Views.Combat;
using Game.Views.Make;
using GameData.DLC.FiveLoong;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using TMPro;
using UnityEngine;

namespace Game.Components.Item
{
	// Token: 0x02000EF1 RID: 3825
	public class ItemBack : MonoBehaviour
	{
		// Token: 0x170013D5 RID: 5077
		// (get) Token: 0x0600AF6C RID: 44908 RVA: 0x004FF349 File Offset: 0x004FD549
		public bool ShowInvalidIcon
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600AF6D RID: 44909 RVA: 0x004FF34C File Offset: 0x004FD54C
		private void OnEnable()
		{
			bool flag = this.icon.sprite != null;
			if (flag)
			{
				this.icon.SetAlpha(1f);
			}
		}

		// Token: 0x0600AF6E RID: 44910 RVA: 0x004FF380 File Offset: 0x004FD580
		public void SetIcon(string iconName)
		{
			this.icon.SetSprite(iconName, false, delegate
			{
				this.icon.SetAlpha(1f);
			});
		}

		// Token: 0x0600AF6F RID: 44911 RVA: 0x004FF39D File Offset: 0x004FD59D
		public void SetIcon(Sprite sprite)
		{
			this.icon.sprite = sprite;
			this.icon.SetAlpha(1f);
		}

		// Token: 0x0600AF70 RID: 44912 RVA: 0x004FF3BE File Offset: 0x004FD5BE
		public void SetBack(sbyte grade)
		{
			GradeBackVisual.ApplyTint(this.gradeBack, grade);
		}

		// Token: 0x0600AF71 RID: 44913 RVA: 0x004FF3D0 File Offset: 0x004FD5D0
		public void Set(ITradeableContent data, bool hideCountOnOne = false)
		{
			bool flag = data != null && data.CharacterId != -1;
			if (flag)
			{
				this.SetIcon((data.Gender == 1) ? this.male : this.female);
				this.SetBack(data.Grade);
				this.SetCount(0, false);
			}
			else
			{
				bool isInvalid = data == null || !data.Key.HasTemplate;
				sbyte? b = (data != null) ? new sbyte?(data.Key.ItemType) : null;
				int? num = (b != null) ? new int?((int)b.GetValueOrDefault()) : null;
				int num2 = 11;
				bool isCricket = num.GetValueOrDefault() == num2 & num != null;
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
				bool isMiscResource = data != null && ItemTemplateHelper.IsMiscResource(data.Key.ItemType, data.Key.TemplateId);
				bool isRandomMake = MakeSubPageMakeHelper.CheckIsRandomMake(data);
				bool flag4 = isInvalid || isRandomMake;
				if (flag4)
				{
					string spName = this.ShowInvalidIcon ? "ui9_icon_item_empty_big" : string.Empty;
					this.SetIcon(spName);
				}
				else
				{
					bool flag5 = isMiscResource;
					string spName2;
					if (flag5)
					{
						sbyte resourceType = ItemTemplateHelper.GetMiscResourceType(data.Key.ItemType, data.Key.TemplateId);
						spName2 = CombatDrops.GetIconName(data.Amount, resourceType, true);
					}
					else
					{
						ItemKey key = data.Key;
						bool flag6 = key.ItemType == 12 && key.TemplateId == 8;
						if (flag6)
						{
							spName2 = CombatDrops.GetExpIconName(data.Amount, true);
						}
						else
						{
							bool flag7 = isCricket;
							if (flag7)
							{
								CricketPartsItem colorConfig = CricketParts.Instance[data.CricketColorId];
								spName2 = colorConfig.Icon;
							}
							else
							{
								bool flag8 = isJiaoEgg;
								if (flag8)
								{
									spName2 = (data.JiaoLoongDisplayData.Jiao.Gender ? "ui9_icon_jiao_egg_gender_1" : "ui9_icon_jiao_egg_gender_0");
								}
								else
								{
									spName2 = ItemTemplateHelper.GetIcon(data.Key.ItemType, data.Key.TemplateId);
								}
							}
						}
					}
					this.SetIcon(spName2);
				}
				int grade = (int)((isInvalid || isRandomMake) ? 0 : (isCricket ? new ValueTuple<short, short>(data.CricketColorId, data.CricketPartId).CalcCricketGrade() : ItemTemplateHelper.GetGrade(data.Key.ItemType, data.Key.TemplateId)));
				this.SetBack((sbyte)grade);
				this.SetCount((data != null) ? data.Amount : 0, hideCountOnOne);
			}
		}

		// Token: 0x0600AF72 RID: 44914 RVA: 0x004FF688 File Offset: 0x004FD888
		public void SetCount(int itemCount, bool hideCountOnOne = false)
		{
			bool flag = this.count;
			if (flag)
			{
				this.count.SetText("x" + CommonUtils.GetDisplayStringForNum(itemCount, 100000), true);
			}
			bool flag2 = this.countBack;
			if (flag2)
			{
				int showCount = hideCountOnOne ? 1 : 0;
				this.countBack.gameObject.SetActive(itemCount > showCount);
			}
			bool flag3 = this.countIcon;
			if (flag3)
			{
				this.countIcon.gameObject.SetActive(false);
			}
		}

		// Token: 0x0600AF73 RID: 44915 RVA: 0x004FF718 File Offset: 0x004FD918
		public void SetCountVisible(bool visible)
		{
			bool flag = this.countBack;
			if (flag)
			{
				this.countBack.SetActive(visible);
			}
		}

		// Token: 0x0600AF74 RID: 44916 RVA: 0x004FF744 File Offset: 0x004FD944
		public void SetCountInfo(string content, string iconName)
		{
			bool flag = this.count;
			if (flag)
			{
				this.count.SetText(content, true);
			}
			bool flag2 = this.countBack;
			if (flag2)
			{
				this.countBack.SetActive(true);
			}
			bool flag3 = this.countIcon;
			if (flag3)
			{
				this.countIcon.SetSprite(iconName, false, null);
				this.countIcon.gameObject.SetActive(!iconName.IsNullOrEmpty());
			}
		}

		// Token: 0x0600AF75 RID: 44917 RVA: 0x004FF7C5 File Offset: 0x004FD9C5
		public void SetGradeOverride(sbyte grade)
		{
			this.SetBack(grade);
		}

		// Token: 0x04008805 RID: 34821
		[SerializeField]
		private CImage icon;

		// Token: 0x04008806 RID: 34822
		[SerializeField]
		private CImage gradeBack;

		// Token: 0x04008807 RID: 34823
		[SerializeField]
		private GameObject countBack;

		// Token: 0x04008808 RID: 34824
		[SerializeField]
		private CImage countIcon;

		// Token: 0x04008809 RID: 34825
		[SerializeField]
		private TextMeshProUGUI count;

		// Token: 0x0400880A RID: 34826
		[Header("男女肖像icon，显示俘虏时用")]
		[SerializeField]
		private Sprite male;

		// Token: 0x0400880B RID: 34827
		[Header("男女肖像icon，显示俘虏时用")]
		[SerializeField]
		private Sprite female;
	}
}
