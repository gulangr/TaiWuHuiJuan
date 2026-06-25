using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork.UISystem.UIElements;
using Game.Views.Building.BuildingManage;
using GameData.Domains.Character;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Taiwu;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.Main.Reading
{
	// Token: 0x02000964 RID: 2404
	public class ReadingBookIntro : MonoBehaviour, IAsyncMethodRequestHandler
	{
		// Token: 0x17000D21 RID: 3361
		// (get) Token: 0x06007363 RID: 29539 RVA: 0x00359C6B File Offset: 0x00357E6B
		public int CurrentReadingEfficiency
		{
			get
			{
				return this._currReadingEfficiency;
			}
		}

		// Token: 0x06007364 RID: 29540 RVA: 0x00359C74 File Offset: 0x00357E74
		public void RefreshAllBookReadingData(ItemKey currReadingBook, bool strategyRefresh)
		{
			SkillBookItem configData = SkillBook.Instance.GetItem(currReadingBook.TemplateId);
			bool flag = configData == null;
			if (!flag)
			{
				bool isCombatSkillBook = ItemTemplateHelper.GetItemSubType(currReadingBook.ItemType, currReadingBook.TemplateId) == 1001;
				ItemDomainMethod.AsyncCall.GetSkillBookPagesInfo(this, currReadingBook, delegate(int offset, RawDataPool dataPool)
				{
					Serializer.Deserialize(dataPool, offset, ref this._skillBookPageDisplayData);
				});
				AsyncMethodCallbackDelegate <>9__4;
				TaiwuDomainMethod.AsyncCall.GetTotalReadingProgress(this, currReadingBook.Id, delegate(int offset, RawDataPool dataPool)
				{
					sbyte progress = 0;
					Serializer.Deserialize(dataPool, offset, ref progress);
					this.expTitle.text = LocalStringManager.Get(LanguageKey.LK_ExpGain) + LocalStringManager.Get(LanguageKey.LK_Colon_Symbol);
					this.expGainRoot.SetActive(progress >= 100);
					bool flag2 = progress == 100;
					if (flag2)
					{
						IAsyncMethodRequestHandler <>4__this = this;
						bool isInBattle = false;
						int remainingSpeedPercent = 100;
						AsyncMethodCallbackDelegate callback;
						if ((callback = <>9__4) == null)
						{
							callback = (<>9__4 = delegate(int offset1, RawDataPool dataPool1)
							{
								int expGain = 0;
								Serializer.Deserialize(dataPool1, offset1, ref expGain);
								this.expGainText.text = string.Format("<color=#pinkyellow>{0}</color>", expGain).ColorReplace();
							});
						}
						TaiwuDomainMethod.AsyncCall.GetExpByRereading(<>4__this, isInBattle, remainingSpeedPercent, callback);
					}
					TaiwuDomainMethod.AsyncCall.GetReadingResult(this, delegate(int offset2, RawDataPool dataPool2)
					{
						int[] progress2 = null;
						Serializer.Deserialize(dataPool2, offset2, ref progress2);
						int currReadingEfficiency = 0;
						int oldReadingEfficiency = this._currReadingEfficiency;
						bool flag3 = progress < 100;
						if (flag3)
						{
							for (int i = 0; i < this._skillBookPageDisplayData.ReadingProgress.Length; i++)
							{
								bool flag4 = this._skillBookPageDisplayData.ReadingProgress[i] < 100;
								if (flag4)
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
						bool strategyRefresh2 = strategyRefresh;
						if (strategyRefresh2)
						{
							bool flag5 = currReadingEfficiency != oldReadingEfficiency;
							if (flag5)
							{
								this._hasEfficiencyDelta = true;
								this._efficiencyDeltaValue = currReadingEfficiency;
								this.ApplyEfficiencyDelta(this._efficiencyDeltaValue);
							}
						}
						else
						{
							this.efficiencyText.SetText(string.Format("{0}%", currReadingEfficiency), true);
						}
						this._currReadingEfficiency = currReadingEfficiency;
					});
				});
				CharacterDomainMethod.AsyncCall.GetAllCombatSkillAttainment(this, SingletonObject.getInstance<BasicGameData>().TaiwuCharId, delegate(int offset, RawDataPool dataPool)
				{
					Serializer.Deserialize(dataPool, offset, ref this._combatSkillAttainments);
				});
				CharacterDomainMethod.AsyncCall.GetAllLifeSkillAttainment(this, SingletonObject.getInstance<BasicGameData>().TaiwuCharId, delegate(int offset, RawDataPool dataPool)
				{
					Serializer.Deserialize(dataPool, offset, ref this._lifeSkillAttainments);
					SkillBookItem skillBookItem = SkillBook.Instance.GetItem(currReadingBook.TemplateId);
					LifeSkillShorts shorts = new LifeSkillShorts((from x in this._lifeSkillAttainments
					select (short)x).ToArray<short>());
					bool isCombatSkillBook = isCombatSkillBook;
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
					bool flag2 = isFinaleLifeSkillType;
					string iconName;
					string skillName;
					if (flag2)
					{
						LifeSkillTypeItem config = Config.LifeSkillType.Instance[finalType];
						iconName = config.DisplayIconOutLine;
						skillName = config.Name;
					}
					else
					{
						CombatSkillTypeItem config2 = CombatSkillType.Instance.GetItem(finalType);
						iconName = config2.DisplayIconOutLine;
						skillName = config2.Name;
					}
					int needAttainment = (int)SkillGradeData.Instance.GetItem(skillBookItem.Grade).ReadingAttainmentRequirement;
					this.skillTypeIcon.SetSprite(iconName, false, null);
					this.skillTypeText.SetText(skillName + "：", true);
					this.readingRequireText.SetText(((taiwuAttainment < needAttainment) ? taiwuAttainment.ToString().SetColor("brightred") : taiwuAttainment.ToString().SetColor("brightblue")) + "/" + needAttainment.ToString().SetColor("pinkyellow"), true);
				});
			}
		}

		// Token: 0x06007365 RID: 29541 RVA: 0x00359D5C File Offset: 0x00357F5C
		public void SetEfficiencyPreview(int previewValue)
		{
			this._isPreviewingEfficiency = true;
			this.efficiencyTextEnd.SetText(string.Format("{0}%", previewValue), true);
			this.efficiencyTextEnd.gameObject.SetActive(true);
			this.efficiencyArrow.SetActive(true);
		}

		// Token: 0x06007366 RID: 29542 RVA: 0x00359DB0 File Offset: 0x00357FB0
		public void ClearEfficiencyPreview()
		{
			bool flag = !this._isPreviewingEfficiency;
			if (!flag)
			{
				this._isPreviewingEfficiency = false;
				bool hasEfficiencyDelta = this._hasEfficiencyDelta;
				if (hasEfficiencyDelta)
				{
					this.ApplyEfficiencyDelta(this._efficiencyDeltaValue);
				}
				else
				{
					this.efficiencyTextEnd.gameObject.SetActive(false);
					this.efficiencyArrow.SetActive(false);
				}
			}
		}

		// Token: 0x06007367 RID: 29543 RVA: 0x00359E10 File Offset: 0x00358010
		private void ApplyEfficiencyDelta(int value)
		{
			this.efficiencyTextEnd.SetText(string.Format("{0}%", value), true);
			this.efficiencyTextEnd.gameObject.SetActive(true);
			this.efficiencyArrow.SetActive(true);
		}

		// Token: 0x06007368 RID: 29544 RVA: 0x00359E4F File Offset: 0x0035804F
		public void RegisterAsyncMethodCall(int requestId)
		{
			this._requestedAsyncMethods.Add(requestId);
		}

		// Token: 0x06007369 RID: 29545 RVA: 0x00359E60 File Offset: 0x00358060
		public void ClearAsyncMethodCalls()
		{
			AsyncMethodDispatcher dispatcher = SingletonObject.getInstance<AsyncMethodDispatcher>();
			foreach (int one in this._requestedAsyncMethods)
			{
				dispatcher.UnregisterAsyncMethodCall(one);
			}
			this._requestedAsyncMethods.Clear();
		}

		// Token: 0x040055A1 RID: 21921
		public ItemResourceButton itemCard;

		// Token: 0x040055A2 RID: 21922
		public GameObject itemEmptyCard;

		// Token: 0x040055A3 RID: 21923
		public CButton itemEmptyBtn;

		// Token: 0x040055A4 RID: 21924
		public TextMeshProUGUI durability;

		// Token: 0x040055A5 RID: 21925
		public TextMeshProUGUI expGainText;

		// Token: 0x040055A6 RID: 21926
		public TextMeshProUGUI expTitle;

		// Token: 0x040055A7 RID: 21927
		public GameObject expGainRoot;

		// Token: 0x040055A8 RID: 21928
		public TextMeshProUGUI efficiencyText;

		// Token: 0x040055A9 RID: 21929
		public GameObject efficiencyArrow;

		// Token: 0x040055AA RID: 21930
		public TextMeshProUGUI efficiencyTextEnd;

		// Token: 0x040055AB RID: 21931
		public CImage skillTypeIcon;

		// Token: 0x040055AC RID: 21932
		public TextMeshProUGUI skillTypeText;

		// Token: 0x040055AD RID: 21933
		public TextMeshProUGUI readingRequireText;

		// Token: 0x040055AE RID: 21934
		public GameObject removeCurBookBtn;

		// Token: 0x040055AF RID: 21935
		public GameObject infoRoot;

		// Token: 0x040055B0 RID: 21936
		private readonly List<int> _requestedAsyncMethods = new List<int>();

		// Token: 0x040055B1 RID: 21937
		private SkillBookPageDisplayData _skillBookPageDisplayData;

		// Token: 0x040055B2 RID: 21938
		private int[] _lifeSkillAttainments = null;

		// Token: 0x040055B3 RID: 21939
		private int[] _combatSkillAttainments = null;

		// Token: 0x040055B4 RID: 21940
		private int _currReadingEfficiency;

		// Token: 0x040055B5 RID: 21941
		private bool _hasEfficiencyDelta;

		// Token: 0x040055B6 RID: 21942
		private bool _isPreviewingEfficiency;

		// Token: 0x040055B7 RID: 21943
		private int _efficiencyDeltaValue;
	}
}
