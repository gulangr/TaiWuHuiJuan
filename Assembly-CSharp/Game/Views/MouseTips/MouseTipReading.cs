using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork;
using Game.Views.Main.Reading;
using GameData.Domains.Character;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Taiwu;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips
{
	// Token: 0x02000875 RID: 2165
	public class MouseTipReading : MouseTipBase
	{
		// Token: 0x17000C7D RID: 3197
		// (get) Token: 0x0600683E RID: 26686 RVA: 0x002FA86E File Offset: 0x002F8A6E
		protected override bool CanStick
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600683F RID: 26687 RVA: 0x002FA874 File Offset: 0x002F8A74
		protected override void Init(ArgumentBox argsBox)
		{
			this._curSuppliedStateList.Clear();
			argsBox.Get<ItemKey>("currentReadingBookKey", out this._curReadingBook);
			argsBox.Get<ItemKey[]>("referenceBooks", out this._referenceBooks);
			this.referenceBooksList = this._referenceBooks.ToList<ItemKey>();
			this.UpdateCurrReadingBookInfo();
			this.UpdateReferenceBook();
		}

		// Token: 0x06006840 RID: 26688 RVA: 0x002FA8D4 File Offset: 0x002F8AD4
		private void UpdateReadingRequire()
		{
			SkillBookItem skillBookItem = SkillBook.Instance.GetItem(this._curReadingBook.TemplateId);
			LifeSkillShorts shorts = new LifeSkillShorts((from x in this._lifeSkillAttainments
			select (short)x).ToArray<short>());
			bool isCombatSkillBook = this._isCombatSkillBook;
			int taiwuAttainment;
			sbyte finalType;
			bool isFinaleLifeSkillType;
			if (isCombatSkillBook)
			{
				sbyte sectId = CombatSkill.Instance[skillBookItem.CombatSkillTemplateId].SectId;
				taiwuAttainment = this._combatSkillAttainments[(int)skillBookItem.CombatSkillType];
				ValueTuple<sbyte, short> attainmentWithSectApprovalBonus = CommonUtils.GetAttainmentWithSectApprovalBonus(sectId, (short)taiwuAttainment, shorts);
				sbyte t = attainmentWithSectApprovalBonus.Item1;
				short replacedAttainment = attainmentWithSectApprovalBonus.Item2;
				taiwuAttainment = (int)replacedAttainment;
				finalType = ((t != -1) ? t : skillBookItem.CombatSkillType);
				isFinaleLifeSkillType = (t != -1);
			}
			else
			{
				taiwuAttainment = this._lifeSkillAttainments[(int)skillBookItem.LifeSkillType];
				finalType = skillBookItem.LifeSkillType;
				isFinaleLifeSkillType = true;
			}
			bool flag = isFinaleLifeSkillType;
			string iconName;
			string skillName;
			if (flag)
			{
				LifeSkillTypeItem config = Config.LifeSkillType.Instance[finalType];
				iconName = config.DisplayIcon;
				skillName = config.Name;
			}
			else
			{
				CombatSkillTypeItem config2 = CombatSkillType.Instance.GetItem(finalType);
				iconName = config2.DisplayIcon;
				skillName = config2.Name;
			}
			int needAttainment = (int)SkillGradeData.Instance.GetItem(skillBookItem.Grade).ReadingAttainmentRequirement;
			this.skillTypeIcon.SetSprite(iconName, false, null);
			this.skillTypeText.SetText(skillName + "：", true);
			this.readingRequireText.SetText(((taiwuAttainment < needAttainment) ? taiwuAttainment.ToString().SetColor("brightred") : taiwuAttainment.ToString().SetColor("brightblue")) + "/" + needAttainment.ToString().SetColor("pinkyellow"), true);
		}

		// Token: 0x06006841 RID: 26689 RVA: 0x002FAAA0 File Offset: 0x002F8CA0
		private void UpdateReferenceBook()
		{
			bool allInvalid = true;
			for (int i = 0; i < this._referenceBooks.Length; i++)
			{
				bool flag = this._referenceBooks[i].IsValid();
				if (flag)
				{
					allInvalid = false;
				}
			}
			bool flag2 = allInvalid;
			if (flag2)
			{
				this.referenceBookRoot.SetActive(false);
			}
			else
			{
				this.referenceBookRoot.SetActive(true);
				for (int j = 0; j < 3; j++)
				{
					this.referenceBook[j].SetActive(false);
				}
				for (int k = 0; k < this._referenceBooks.Length; k++)
				{
					bool flag3 = this._referenceBooks[k].IsValid();
					if (flag3)
					{
						this.referenceBook[k].SetActive(true);
						SkillBookItem skillBookConfig = SkillBook.Instance.GetItem(this._referenceBooks[k].TemplateId);
						this.referenceBookTexts[k].SetText(skillBookConfig.Name.SetGradeColor((int)skillBookConfig.Grade), true);
					}
				}
			}
		}

		// Token: 0x06006842 RID: 26690 RVA: 0x002FABC0 File Offset: 0x002F8DC0
		private void UpdateSkillBookPageDisplayData()
		{
			int pageCount = this._isCombatSkillBook ? 6 : 5;
			this._curSuppliedStateList.Clear();
			this._curSuppliedStateList.AddRange(Enumerable.Repeat<sbyte>(-1, pageCount));
			for (int i = 0; i < 6; i++)
			{
				CImage pageImg = this.pages[i];
				bool isActivePage = i < pageCount;
				pageImg.gameObject.SetActive(isActivePage);
				this.highLightList.CGet<GameObject>(string.Format("HighLight_{0}", i)).SetActive(false);
				bool flag = isActivePage;
				if (flag)
				{
					this.UpdateReferenceBookSupplyData(i, pageImg, this.highLightList);
				}
			}
		}

		// Token: 0x06006843 RID: 26691 RVA: 0x002FAC64 File Offset: 0x002F8E64
		private void UpdateReferenceBookSupplyData(int pageIndex, CImage pageImg, Refers hightLightList)
		{
			bool isHighLightActive = false;
			bool flag = this._referenceBookPageDisplayDataList != null;
			if (flag)
			{
				foreach (SkillBookPageDisplayData referenceBook in this._referenceBookPageDisplayDataList)
				{
					bool flag2 = this._curReadingBook.TemplateEquals(referenceBook.ItemKey);
					if (flag2)
					{
						sbyte readingState = this._skillBookPageDisplayData.State[pageIndex];
						bool lengthEnough = pageIndex < referenceBook.State.Length;
						sbyte refState = lengthEnough ? referenceBook.State[pageIndex] : -1;
						bool hasSupply = lengthEnough && refState < readingState;
						bool flag3 = referenceBook.IsCombatBook && referenceBook.Type[pageIndex] != this._skillBookPageDisplayData.Type[pageIndex];
						if (flag3)
						{
							hasSupply = false;
						}
						bool flag4 = this._curSuppliedStateList.Count > pageIndex;
						if (flag4)
						{
							bool flag5 = hasSupply;
							if (flag5)
							{
								isHighLightActive = true;
								bool flag6 = this._curSuppliedStateList[pageIndex] == -1 || this._curSuppliedStateList[pageIndex] > refState;
								if (flag6)
								{
									this._curSuppliedStateList[pageIndex] = refState;
								}
							}
							else
							{
								bool flag7 = this._curSuppliedStateList[pageIndex] == -1;
								if (flag7)
								{
									this._curSuppliedStateList[pageIndex] = readingState;
								}
							}
						}
					}
				}
			}
			hightLightList.CGet<GameObject>(string.Format("HighLight_{0}", pageIndex)).SetActive(isHighLightActive);
			bool flag8 = this._curSuppliedStateList[pageIndex] != -1;
			if (flag8)
			{
				ReadingDisplayHelper.SetPageCompleteState(this._curSuppliedStateList[pageIndex], pageImg);
			}
			else
			{
				ReadingDisplayHelper.SetPageCompleteState(this._skillBookPageDisplayData.State[pageIndex], pageImg);
			}
		}

		// Token: 0x06006844 RID: 26692 RVA: 0x002FAE48 File Offset: 0x002F9048
		private void UpdateItemDispalyData()
		{
			this.bookInfoText.text = LocalStringManager.Get(LanguageKey.LK_ReadingInfo);
			this.bookInfoTextValue.text = CommonUtils.GetDurabilityString((int)this._itemDisplayData.Durability, (int)this._itemDisplayData.MaxDurability);
		}

		// Token: 0x06006845 RID: 26693 RVA: 0x002FAE88 File Offset: 0x002F9088
		private void UpdateCurrReadingBookInfo()
		{
			bool flag = !this._curReadingBook.IsValid();
			if (!flag)
			{
				SkillBookItem configData = SkillBook.Instance.GetItem(this._curReadingBook.TemplateId);
				bool flag2 = configData == null;
				if (!flag2)
				{
					this._isCombatSkillBook = (ItemTemplateHelper.GetItemSubType(this._curReadingBook.ItemType, this._curReadingBook.TemplateId) == 1001);
					this.bookName.SetText(LocalStringManager.GetFormat(LanguageKey.LK_CurrentReadingBook, configData.Name), true);
					this.desc.SetText(configData.Desc, true);
					ItemDomainMethod.AsyncCall.GetItemDisplayData(this, this._curReadingBook, delegate(int offset, RawDataPool dataPool)
					{
						Serializer.Deserialize(dataPool, offset, ref this._itemDisplayData);
						this.UpdateItemDispalyData();
					});
					ItemDomainMethod.AsyncCall.GetSkillBookPagesInfo(this, this._curReadingBook, delegate(int offset, RawDataPool dataPool)
					{
						Serializer.Deserialize(dataPool, offset, ref this._skillBookPageDisplayData);
					});
					ItemDomainMethod.AsyncCall.GetSkillBookPageDisplayDataList(this, this.referenceBooksList, delegate(int offset, RawDataPool dataPool)
					{
						Serializer.Deserialize(dataPool, offset, ref this._referenceBookPageDisplayDataList);
						this.UpdateSkillBookPageDisplayData();
					});
					TaiwuDomainMethod.AsyncCall.GetCurrReadingEventBonusRate(this, delegate(int offset, RawDataPool dataPool)
					{
						short inspireRetio = 0;
						Serializer.Deserialize(dataPool, offset, ref inspireRetio);
						this.inspireRatioText.text = LanguageKey.LK_ReadingInspireRatio.Tr();
						this.inspireRatioTextValue.text = inspireRetio.ToString() + "%";
					});
					TaiwuDomainMethod.AsyncCall.GetTotalReadingProgress(this, this._curReadingBook.Id, delegate(int offset, RawDataPool dataPool)
					{
						sbyte progress = 0;
						Serializer.Deserialize(dataPool, offset, ref progress);
						this.finishedTip.SetActive(false);
						this.expText1.text = LocalStringManager.Get(LanguageKey.LK_ExpGain) + LocalStringManager.Get(LanguageKey.LK_Colon_Symbol);
						bool flag3 = progress == 100;
						if (flag3)
						{
							TaiwuDomainMethod.AsyncCall.GetExpByRereading(this, false, 100, delegate(int offset1, RawDataPool dataPool1)
							{
								int expGainAmount = 0;
								Serializer.Deserialize(dataPool1, offset1, ref expGainAmount);
								this.finishedTip.SetActive(true);
								this.expGainValue.text = string.Format("<color=#pinkyellow>+{0}</color>", expGainAmount).ColorReplace();
							});
						}
						TaiwuDomainMethod.AsyncCall.GetReadingResult(this, delegate(int offset2, RawDataPool dataPool2)
						{
							int[] progress2 = null;
							Serializer.Deserialize(dataPool2, offset2, ref progress2);
							int currReadingEfficiency = 0;
							bool flag4 = progress < 100;
							if (flag4)
							{
								for (int i = 0; i < this._skillBookPageDisplayData.ReadingProgress.Length; i++)
								{
									bool flag5 = this._skillBookPageDisplayData.ReadingProgress[i] < 100;
									if (flag5)
									{
										currReadingEfficiency += Math.Min(progress2[i], (int)(100 - this._skillBookPageDisplayData.ReadingProgress[i]));
									}
								}
							}
							else
							{
								foreach (int t in progress2)
								{
									currReadingEfficiency += t;
								}
							}
							this.readingProgressText.text = LanguageKey.LK_TotalReadingEfficiency.Tr();
							this.readingProgressTextValue.text = currReadingEfficiency.ToString() + "%";
						});
					});
					CharacterDomainMethod.AsyncCall.GetAllCombatSkillAttainment(this, SingletonObject.getInstance<BasicGameData>().TaiwuCharId, delegate(int offset, RawDataPool dataPool)
					{
						Serializer.Deserialize(dataPool, offset, ref this._combatSkillAttainments);
					});
					CharacterDomainMethod.AsyncCall.GetAllLifeSkillAttainment(this, SingletonObject.getInstance<BasicGameData>().TaiwuCharId, delegate(int offset, RawDataPool dataPool)
					{
						Serializer.Deserialize(dataPool, offset, ref this._lifeSkillAttainments);
						this.UpdateReadingRequire();
					});
				}
			}
		}

		// Token: 0x040049D7 RID: 18903
		[SerializeField]
		private CImage skillTypeIcon;

		// Token: 0x040049D8 RID: 18904
		[SerializeField]
		private TextMeshProUGUI skillTypeText;

		// Token: 0x040049D9 RID: 18905
		[SerializeField]
		private TextMeshProUGUI readingRequireText;

		// Token: 0x040049DA RID: 18906
		[Header("参考书籍")]
		[SerializeField]
		private TextMeshProUGUI[] referenceBookTexts;

		// Token: 0x040049DB RID: 18907
		[SerializeField]
		private GameObject[] referenceBook;

		// Token: 0x040049DC RID: 18908
		[SerializeField]
		private GameObject referenceBookRoot;

		// Token: 0x040049DD RID: 18909
		[Header("研读效果")]
		[SerializeField]
		private Refers highLightList;

		// Token: 0x040049DE RID: 18910
		[SerializeField]
		private CImage[] pages;

		// Token: 0x040049DF RID: 18911
		[SerializeField]
		private TextMeshProUGUI bookInfoText;

		// Token: 0x040049E0 RID: 18912
		[SerializeField]
		private TextMeshProUGUI bookInfoTextValue;

		// Token: 0x040049E1 RID: 18913
		[SerializeField]
		private TextMeshProUGUI bookName;

		// Token: 0x040049E2 RID: 18914
		[SerializeField]
		private TextMeshProUGUI desc;

		// Token: 0x040049E3 RID: 18915
		[SerializeField]
		private TextMeshProUGUI inspireRatioText;

		// Token: 0x040049E4 RID: 18916
		[SerializeField]
		private TextMeshProUGUI inspireRatioTextValue;

		// Token: 0x040049E5 RID: 18917
		[SerializeField]
		private GameObject finishedTip;

		// Token: 0x040049E6 RID: 18918
		[SerializeField]
		private TextMeshProUGUI expText1;

		// Token: 0x040049E7 RID: 18919
		[SerializeField]
		private TextMeshProUGUI expGainValue;

		// Token: 0x040049E8 RID: 18920
		[SerializeField]
		private TextMeshProUGUI readingProgressText;

		// Token: 0x040049E9 RID: 18921
		[SerializeField]
		private TextMeshProUGUI readingProgressTextValue;

		// Token: 0x040049EA RID: 18922
		private ItemKey _curReadingBook;

		// Token: 0x040049EB RID: 18923
		private ItemKey[] _referenceBooks = new ItemKey[]
		{
			ItemKey.Invalid,
			ItemKey.Invalid,
			ItemKey.Invalid
		};

		// Token: 0x040049EC RID: 18924
		private List<ItemKey> referenceBooksList = new List<ItemKey>();

		// Token: 0x040049ED RID: 18925
		private ItemDisplayData _itemDisplayData;

		// Token: 0x040049EE RID: 18926
		private SkillBookPageDisplayData _skillBookPageDisplayData;

		// Token: 0x040049EF RID: 18927
		private List<SkillBookPageDisplayData> _referenceBookPageDisplayDataList;

		// Token: 0x040049F0 RID: 18928
		private int[] _lifeSkillAttainments = null;

		// Token: 0x040049F1 RID: 18929
		private int[] _combatSkillAttainments = null;

		// Token: 0x040049F2 RID: 18930
		private bool _isCombatSkillBook;

		// Token: 0x040049F3 RID: 18931
		private readonly List<sbyte> _curSuppliedStateList = new List<sbyte>();
	}
}
