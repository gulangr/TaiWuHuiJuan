using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Config;
using DG.Tweening;
using FrameWork;
using Game.Views.Main.Reading;
using GameData.Domains.Character;
using GameData.Domains.Extra;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Taiwu;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using Spine.Unity;
using TMPro;
using UnityEngine;

namespace GearMate
{
	// Token: 0x0200061E RID: 1566
	public class GearMateSubPageSkill : GearMateSubPageBase
	{
		// Token: 0x17000948 RID: 2376
		// (get) Token: 0x060049E2 RID: 18914 RVA: 0x00227973 File Offset: 0x00225B73
		private int PageCount
		{
			get
			{
				return this.isCombatSkill ? 6 : 5;
			}
		}

		// Token: 0x17000949 RID: 2377
		// (get) Token: 0x060049E3 RID: 18915 RVA: 0x00227981 File Offset: 0x00225B81
		private ItemGradeFilterSetting.ItemGradeFilterSourceType CurItemGradeFilterSourceType
		{
			get
			{
				return ItemGradeFilterSetting.GetItemGradeFilterSourceType(this._itemSourceType, this._itemSourceType == ItemSourceType.Inventory, true);
			}
		}

		// Token: 0x1700094A RID: 2378
		// (get) Token: 0x060049E4 RID: 18916 RVA: 0x00227998 File Offset: 0x00225B98
		private int SkillTypeIndex
		{
			get
			{
				CToggleGroupObsolete group = this.isCombatSkill ? this._combatSkillTypeTogGroup : this._lifeSkillTypeTogGroup;
				return group.GetActive().Key;
			}
		}

		// Token: 0x1700094B RID: 2379
		// (get) Token: 0x060049E5 RID: 18917 RVA: 0x002279CC File Offset: 0x00225BCC
		private int CurGearMateId
		{
			get
			{
				return base.Parent.GearMateId;
			}
		}

		// Token: 0x1700094C RID: 2380
		// (get) Token: 0x060049E6 RID: 18918 RVA: 0x002279D9 File Offset: 0x00225BD9
		private int taiwuId
		{
			get
			{
				return SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			}
		}

		// Token: 0x060049E7 RID: 18919 RVA: 0x002279E8 File Offset: 0x00225BE8
		public override void OnGearMateDataChanged()
		{
			this._pageProgressDic.Clear();
			this._pageProgressDicDefault.Clear();
			this._bookTotalProgressDic.Clear();
			bool flag = this.isCombatSkill;
			if (flag)
			{
				foreach (KeyValuePair<short, TaiwuCombatSkill> pair in base.GearMate.CombatSkillReadingProgress)
				{
					sbyte[] progressArray = pair.Value.GetAllBookPageReadingProgress();
					this._pageProgressDic.Add(pair.Key, progressArray);
					this._pageProgressDicDefault.Add(pair.Key, (sbyte[])progressArray.Clone());
					sbyte totalProgress = (sbyte)(this._pageProgressDic[pair.Key].Sum() / this._pageProgressDic[pair.Key].Length);
					this._bookTotalProgressDic.Add(pair.Key, totalProgress);
				}
			}
			else
			{
				foreach (KeyValuePair<short, TaiwuLifeSkill> pair2 in base.GearMate.LifeSkillReadingProgress)
				{
					sbyte[] progressArray2 = pair2.Value.GetAllBookPageReadingProgress();
					this._pageProgressDic.Add(pair2.Key, progressArray2);
					this._pageProgressDicDefault.Add(pair2.Key, (sbyte[])progressArray2.Clone());
					sbyte totalProgress2 = (sbyte)(this._pageProgressDic[pair2.Key].Sum() / this._pageProgressDic[pair2.Key].Length);
					this._bookTotalProgressDic.Add(pair2.Key, totalProgress2);
				}
			}
			bool flag2 = this._previewChapterItemKey.IsValid();
			if (flag2)
			{
				this.RefreshBookAfterUsed();
				this._previewChapterItemKey = ItemKey.Invalid;
			}
			this.RefreshReadBooks();
			this.RefreshBooks();
			this.RefreshBtnTip();
			this.RenderSkillType(ItemKey.Invalid, false);
		}

		// Token: 0x060049E8 RID: 18920 RVA: 0x00227C1C File Offset: 0x00225E1C
		public override void OnGearMateCharacterIdChanged(int lastId)
		{
			base.OnGearMateCharacterIdChanged(lastId);
			bool flag = this.CurGearMateId != lastId;
			if (flag)
			{
				bool flag2 = lastId != -1;
				if (flag2)
				{
					base.RemoveMonitorFieldId(4, 0, (ulong)lastId);
					this.DeselectAllBooks();
					this.OnSelectedBookChanged();
					this.RefreshReadBooks();
					this.RefreshBooks();
				}
				base.AppendMonitorFieldId(new UIBase.MonitorDataField(4, 0, (ulong)this.CurGearMateId, this._fields));
			}
		}

		// Token: 0x060049E9 RID: 18921 RVA: 0x00227C94 File Offset: 0x00225E94
		protected override IList<UIBase.MonitorDataField> GetMonitorFields()
		{
			return new List<UIBase.MonitorDataField>
			{
				new UIBase.MonitorDataField(4, 0, (ulong)this.taiwuId, new uint[]
				{
					66U
				})
			};
		}

		// Token: 0x060049EA RID: 18922 RVA: 0x00227CD0 File Offset: 0x00225ED0
		protected override void InitInternal()
		{
			base.InitInternal();
			this._curSkillType = (this.isCombatSkill ? 0 : 0);
			this.InitRefers();
			this.InitSkillTypeToggles();
			this._confirmBtn.ClearAndAddListener(new Action(this.OnConfirm));
			this._btnSelectAll.ClearAndAddListener(new Action(this.OnClickSelectAll));
			this._btnMultiplyOption.ClearAndAddListener(new Action(this.OnClickMultiplyOption));
			this._itemSourceToggleGroup.InitPreOnToggle(-1);
			this._itemSourceToggleGroup.OnActiveToggleChange = new Action<CToggleObsolete, CToggleObsolete>(this.OnItemSourceToggleChange);
			UIElement element = base.Parent.Element;
			element.OnHide = (Action)Delegate.Combine(element.OnHide, new Action(this.OnUIGearMateHide));
		}

		// Token: 0x060049EB RID: 18923 RVA: 0x00227D9D File Offset: 0x00225F9D
		private void OnSkillTypeToggleChanged(CToggleObsolete newToggle, CToggleObsolete oldToggle)
		{
			this.RefreshBooks();
		}

		// Token: 0x060049EC RID: 18924 RVA: 0x00227DA8 File Offset: 0x00225FA8
		private void InitSkillTypeToggles()
		{
			this._combatSkillTypeTogGroup.gameObject.SetActive(this.isCombatSkill);
			this._lifeSkillTypeTogGroup.gameObject.SetActive(!this.isCombatSkill);
			bool flag = this.isCombatSkill;
			if (flag)
			{
				this._combatSkillTypeTogGroup.InitPreOnToggle(-1);
				this._combatSkillTypeTogGroup.OnActiveToggleChange = new Action<CToggleObsolete, CToggleObsolete>(this.OnSkillTypeToggleChanged);
				for (int i = 0; i < CombatSkillTypeTogGroup.ToggleConfig.Count; i++)
				{
					CombatSkillTypeTogGroup.ToggleInfo toggleInfo = CombatSkillTypeTogGroup.ToggleConfig[i];
					CToggleObsolete tog = this._combatSkillTypeTogGroup.Get(i);
					Refers togRefers = tog.GetComponent<Refers>();
					togRefers.CGet<TextMeshProUGUI>("Label").text = LocalStringManager.Get(toggleInfo.LabelKey);
					togRefers.CGet<CImage>("Icon").SetSprite(toggleInfo.GetIcon(), false, null);
					togRefers.CGet<GameObject>("BookCountBack").SetActive(false);
				}
			}
			else
			{
				this._lifeSkillTypeTogGroup.InitPreOnToggle(-1);
				this._lifeSkillTypeTogGroup.OnActiveToggleChange = new Action<CToggleObsolete, CToggleObsolete>(this.OnSkillTypeToggleChanged);
				int count = 16;
				for (int j = 0; j < this._lifeSkillTypeTogGroup.transform.childCount; j++)
				{
					this._lifeSkillTypeTogGroup.transform.GetChild(j).gameObject.SetActive(j <= count);
				}
				for (int k = 0; k < count; k++)
				{
					CToggleObsolete tog2 = this._lifeSkillTypeTogGroup.Get(k);
					Refers refer = tog2.GetComponent<Refers>();
					refer.CGet<TextMeshProUGUI>("Label").text = Config.LifeSkillType.Instance[k].Name;
					string sp = CommonUtils.GetFilterLifeSkillTypeIcon(k);
					refer.CGet<CImage>("Icon").SetSprite(sp, false, null);
					refer.CGet<GameObject>("BookCountBack").SetActive(false);
				}
			}
		}

		// Token: 0x060049ED RID: 18925 RVA: 0x00227FAC File Offset: 0x002261AC
		private void InitRefers()
		{
			this._particle = base.CGet<ParticleSystem>("Particle");
			this._anim = base.CGet<SkeletonGraphic>("Anim");
			this._confirmBtn = base.CGet<CButtonObsolete>("ButtonConfirm");
			this._skillType = base.CGet<Refers>("SkillType");
			this._curBookAfter = base.CGet<Refers>("CurBookAfter");
			this._curBookNow = base.CGet<Refers>("CurBookNow");
			this._pagesRefers = base.CGet<Refers>("PagesRefers");
			this._have = base.CGet<TextMeshProUGUI>("Have");
			this._need = base.CGet<TextMeshProUGUI>("Need");
			this._selectBookScroll = base.CGet<InfinityScrollLegacy>("SelectBookScroll");
			this._combatSkillTypeTogGroup = base.CGet<CToggleGroupObsolete>("CombatSkillTypeTogGroup");
			this._lifeSkillTypeTogGroup = base.CGet<CToggleGroupObsolete>("LifeSkillTypeTogGroup");
			this._readStateAfterEmpty = base.CGet<GameObject>("ReadStateAfterEmpty");
			this._readStateNowEmpty = base.CGet<GameObject>("ReadStateNowEmpty");
			this._curBookNowEmpty = base.CGet<GameObject>("CurBookNowEmpty");
			this._itemSourceToggleGroup = base.CGet<CToggleGroupObsolete>("ItemSourceToggleGroup");
			this._btnSelectAll = base.CGet<CButtonObsolete>("BtnSelectAll");
			this._btnMultiplyOption = base.CGet<CButtonObsolete>("BtnMultiplyOption");
			bool flag = this.isCombatSkill;
			if (flag)
			{
				this._chapters = base.CGet<RectTransform>("Chapters");
			}
		}

		// Token: 0x060049EE RID: 18926 RVA: 0x00228107 File Offset: 0x00226307
		public override void OnListenerIdReady()
		{
			this.ClearBookDisplayData();
			this.ResetBookScroll();
			this.GetData();
		}

		// Token: 0x060049EF RID: 18927 RVA: 0x00228120 File Offset: 0x00226320
		private void OnItemSourceToggleChange(CToggleObsolete newToggle, CToggleObsolete oldToggle)
		{
			bool flag = !newToggle;
			if (!flag)
			{
				this._itemSourceType = this._itemSourceTypeArray[newToggle.Key];
				this.RefreshBooks();
				MonoJoint componentInChildren = newToggle.GetComponentInChildren<MonoJoint>(true);
				if (componentInChildren != null)
				{
					componentInChildren.JointSync();
				}
			}
		}

		// Token: 0x060049F0 RID: 18928 RVA: 0x0022816C File Offset: 0x0022636C
		private unsafe void RenderSkillType(ItemKey itemKey, bool hideAddPreview = false)
		{
			GearMateSubPageSkill.<>c__DisplayClass72_0 CS$<>8__locals1;
			CS$<>8__locals1.<>4__this = this;
			bool flag = base.Parent == null || !base.Parent.gameObject.activeInHierarchy;
			if (!flag)
			{
				TextMeshProUGUI attainmentDesc = this._skillType.CGet<TextMeshProUGUI>("AttainmentDesc");
				TextMeshProUGUI typeName = this._skillType.CGet<TextMeshProUGUI>("TypeName");
				CImage skillIcon = this._skillType.CGet<CImage>("SkillIcon");
				TextMeshProUGUI qualificationNow = this._skillType.CGet<TextMeshProUGUI>("QualificationNow");
				TextMeshProUGUI qualificationAdd = this._skillType.CGet<TextMeshProUGUI>("QualificationAdd");
				TextMeshProUGUI attainmentNow = this._skillType.CGet<TextMeshProUGUI>("AttainmentNow");
				TextMeshProUGUI attainmentAdd = this._skillType.CGet<TextMeshProUGUI>("AttainmentAdd");
				RectTransform stages = this._skillType.CGet<RectTransform>("Stages");
				CS$<>8__locals1.disPlayer = this._skillType.CGet<TooltipInvoker>("MouseTipDisplayer");
				qualificationAdd.text = "";
				attainmentAdd.text = "";
				bool flag2 = this._curAnim != null;
				if (flag2)
				{
					this._curAnim.DOPause();
				}
				this._curParticle = null;
				sbyte curGrade = -1;
				bool flag3 = itemKey.IsValid();
				if (flag3)
				{
					SkillBookItem config = SkillBook.Instance[itemKey.TemplateId];
					this._curSkillType = (this.isCombatSkill ? config.CombatSkillType : config.LifeSkillType);
					curGrade = config.Grade;
				}
				else
				{
					bool flag4 = this._curAnim != null;
					if (flag4)
					{
						this._curAnim.DOPause();
					}
				}
				CS$<>8__locals1.skillTypeName = (this.isCombatSkill ? CombatSkillType.Instance[this._curSkillType].Name : Config.LifeSkillType.Instance[this._curSkillType].Name);
				typeName.text = CS$<>8__locals1.skillTypeName;
				for (sbyte grade = 0; grade < 9; grade += 1)
				{
					Refers stageRefers = stages.GetChild((int)grade).GetComponent<Refers>();
					CImage icon = stageRefers.CGet<CImage>("Icon");
					TextMeshProUGUI gradeText = stageRefers.CGet<TextMeshProUGUI>("Grade");
					ParticleSystem particle = stageRefers.CGet<ParticleSystem>("Particle");
					gradeText.text = LocalStringManager.Get(string.Format("LK_ShortGrade_{0}", grade));
					icon.SetSprite("sp_icon_pinji_0", false, null);
					icon.SetColor(Color.gray);
					bool flag5 = this.isCombatSkill;
					if (flag5)
					{
						bool flag6 = base.GearMate.IsCombatSkillBuffed(this._curSkillType, grade);
						if (flag6)
						{
							icon.SetSprite(ItemView.GetGradeIcon(grade), false, null);
							icon.SetColor(Color.white);
						}
					}
					else
					{
						bool flag7 = base.GearMate.IsLifeSkillBuffed(this._curSkillType, grade);
						if (flag7)
						{
							icon.SetSprite(ItemView.GetGradeIcon(grade), false, null);
							icon.SetColor(Color.white);
						}
					}
				}
				if (!hideAddPreview)
				{
					bool flag8 = this.isCombatSkill;
					if (flag8)
					{
						skillIcon.SetSprite(string.Format("sp_icon_wuxue_{0}", this._curSkillType), false, null);
						int curQualificationValue = (int)(*this._combatSkillQualifications[(int)this._curSkillType]);
						qualificationNow.text = curQualificationValue.ToString();
						int curAttainmentValue = (int)(*this._combatSkillAttainments[(int)this._curSkillType]);
						attainmentNow.text = curAttainmentValue.ToString();
						int alreadyAddQual = 0;
						this._sectInfos.Clear();
						for (sbyte grade2 = 0; grade2 < 9; grade2 += 1)
						{
							short skillTemplateId = -1;
							short[] attainmentPanels = this._attainmentPanels;
							bool flag9 = attainmentPanels != null && attainmentPanels.Length > 0;
							if (flag9)
							{
								skillTemplateId = CombatSkillAttainmentPanelsHelper.Get(this._attainmentPanels, this._curSkillType, grade2);
							}
							bool flag10 = skillTemplateId >= 0;
							if (flag10)
							{
								CombatSkillHelper.CalcAttainments_RecordSectInfo(this._sectInfos, CombatSkill.Instance[skillTemplateId].SectId, grade2);
							}
							bool flag11 = base.GearMate.IsCombatSkillBuffed(this._curSkillType, grade2);
							if (flag11)
							{
								alreadyAddQual += (int)((grade2 + 1) * 2);
							}
						}
						this.<RenderSkillType>g__RefreshTip|72_0(alreadyAddQual, ref CS$<>8__locals1);
						int index = CombatSkillHelper.CalcAttainments_GetPrimarySectIndex(this._sectInfos);
						bool hasSectBonus = index >= 0 && this._sectInfos[index].OrgTemplateId > 0;
						string sectName = hasSectBonus ? LocalStringManager.Get(string.Format("LK_Sect_Name_Short_{0}", this._sectInfos[index].OrgTemplateId)) : null;
						string attainmentLevelDesc = hasSectBonus ? LocalStringManager.GetFormat(LanguageKey.LK_Combat_Skill_Attainment_Panel_Sect_Bonus_Title, sectName) : LocalStringManager.Get(LanguageKey.LK_Combat_Skill_Attainment_Panel_No_Sect_Bonus_Title).SetColor("grey");
						attainmentDesc.text = attainmentLevelDesc;
						int qualificationAddValue = 0;
						bool flag12 = curGrade == -1;
						if (!flag12)
						{
							bool flag13 = !base.GearMate.IsCombatSkillBuffed(this._curSkillType, curGrade);
							if (flag13)
							{
								Refers stageRefers2 = stages.GetChild((int)curGrade).GetComponent<Refers>();
								ParticleSystem particle2 = stageRefers2.CGet<ParticleSystem>("Particle");
								bool flag14 = this._finishedCountAfter == this.PageCount;
								if (flag14)
								{
									qualificationAddValue = (int)(2 * (curGrade + 1));
									qualificationAdd.text = string.Format("+{0}", qualificationAddValue);
									this._curAnim = stageRefers2.CGet<DOTweenAnimation>("Anim");
									this._curAnim.DOPlay();
									this._curParticle = particle2;
								}
							}
							int bonus = (int)(GlobalConfig.Instance.AddAttainmentPerGrade[(int)curGrade] / 5) * this._finishedCountAfter;
							int newAttainment = (curQualificationValue + qualificationAddValue) * (100 + bonus) / 100 + bonus;
							int attainmentAddValue = newAttainment - curAttainmentValue;
							this._isQualificationLevelUp = (qualificationAddValue > 0);
						}
					}
					else
					{
						skillIcon.SetSprite(string.Format("sp_icon_dajiyi_{0}", this._curSkillType), false, null);
						int curQualificationValue2 = (int)(*this._lifeSkillQualifications[(int)this._curSkillType]);
						qualificationNow.text = curQualificationValue2.ToString();
						int curAttainmentValue2 = (int)(*this._lifeSkillAttainments[(int)this._curSkillType]);
						attainmentNow.text = curAttainmentValue2.ToString();
						int alreadyAddQual2 = 0;
						for (sbyte grade3 = 0; grade3 < 9; grade3 += 1)
						{
							bool flag15 = base.GearMate.IsLifeSkillBuffed(this._curSkillType, grade3);
							if (flag15)
							{
								alreadyAddQual2 += (int)((grade3 + 1) * 2);
							}
						}
						this.<RenderSkillType>g__RefreshTip|72_0(alreadyAddQual2, ref CS$<>8__locals1);
						int readedBookCount = 0;
						short[] skillIdList = Config.LifeSkillType.Instance[this._curSkillType].SkillList;
						short[] array = skillIdList;
						for (int i = 0; i < array.Length; i++)
						{
							short skillId = array[i];
							short id = skillId;
							bool flag16 = this._learnedLifeSkills.FindIndex((GameData.Domains.Character.LifeSkillItem item) => item.SkillTemplateId == id) >= 0;
							if (flag16)
							{
								readedBookCount++;
							}
						}
						int attainmentLevel = readedBookCount / 3;
						string attainmentLevelDesc2 = LocalStringManager.Get(string.Format("LK_Life_Skill_Attainment_Level_{0}", attainmentLevel));
						bool flag17 = attainmentLevel < 2;
						if (flag17)
						{
							attainmentLevelDesc2 = attainmentLevelDesc2.SetColor("grey");
						}
						attainmentDesc.text = attainmentLevelDesc2;
						int qualificationAddValue2 = 0;
						bool flag18 = curGrade == -1;
						if (!flag18)
						{
							bool flag19 = !base.GearMate.IsLifeSkillBuffed(this._curSkillType, curGrade);
							if (flag19)
							{
								Refers stageRefers3 = stages.GetChild((int)curGrade).GetComponent<Refers>();
								ParticleSystem particle3 = stageRefers3.CGet<ParticleSystem>("Particle");
								bool flag20 = this._finishedCountAfter == this.PageCount;
								if (flag20)
								{
									this._curAnim = stageRefers3.CGet<DOTweenAnimation>("Anim");
									this._curAnim.DOPlay();
									this._curParticle = particle3;
									qualificationAddValue2 = (int)(2 * (curGrade + 1));
									qualificationAdd.text = string.Format("+{0}", qualificationAddValue2);
								}
							}
							int bonus2 = (int)(GlobalConfig.Instance.AddAttainmentPerGrade[(int)curGrade] / 5) * this._finishedCountAfter;
							int newAttainment2 = (curQualificationValue2 + qualificationAddValue2) * (100 + bonus2) / 100 + bonus2;
							int attainmentAddValue2 = newAttainment2 - curAttainmentValue2;
							this._isQualificationLevelUp = (qualificationAddValue2 > 0);
						}
					}
				}
			}
		}

		// Token: 0x060049F1 RID: 18929 RVA: 0x0022897C File Offset: 0x00226B7C
		public void OnConfirm()
		{
			int count = 1;
			ItemDisplayData itemData = this._bookDisplayData[this._curBookKey.Id];
			string itemName = ItemTemplateHelper.GetName(itemData.Key.ItemType, itemData.Key.TemplateId);
			sbyte grade = ItemTemplateHelper.GetGrade(itemData.Key.ItemType, itemData.Key.TemplateId);
			itemName = itemName.SetGradeColor((int)grade);
			sbyte gradeLimit = SingletonObject.getInstance<GameSort>().GetItemGradeFilterSetting().GetGrade(this.CurItemGradeFilterSourceType);
			bool hasLimit = gradeLimit >= 0 && grade >= gradeLimit;
			bool flag = hasLimit;
			if (flag)
			{
				DialogCmd dialogCmd = new DialogCmd();
				dialogCmd.Type = 1;
				dialogCmd.Title = LocalStringManager.Get(LanguageKey.LK_Common_Attention);
				dialogCmd.Content = LocalStringManager.GetFormat(LanguageKey.LK_Item_UseWarning_Precious, itemName, count);
				dialogCmd.Yes = delegate()
				{
					this.<OnConfirm>g__ProceedUse|73_0();
				};
				UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", dialogCmd));
				UIManager.Instance.MaskUI(UIElement.Dialog);
			}
			else
			{
				this.<OnConfirm>g__ProceedUse|73_0();
			}
		}

		// Token: 0x060049F2 RID: 18930 RVA: 0x00228A9C File Offset: 0x00226C9C
		public override bool ConfirmButtonInteractable()
		{
			return this._confirmBtn.interactable;
		}

		// Token: 0x060049F3 RID: 18931 RVA: 0x00228ABC File Offset: 0x00226CBC
		private void OnClickSelectAll()
		{
			sbyte grade = SingletonObject.getInstance<GameSort>().GetItemGradeFilterSetting().GetGrade(this.CurItemGradeFilterSourceType);
			List<ItemKey> books = this.GetMultiplySelectResult(grade);
			bool isSame = this.CompareSelectedList(books);
			bool flag = isSame;
			if (flag)
			{
				this.DeselectAllBooks();
			}
			this.OnSelectedBookChanged();
			this.RefreshBooks();
		}

		// Token: 0x060049F4 RID: 18932 RVA: 0x00228B0C File Offset: 0x00226D0C
		private void OnClickMultiplyOption()
		{
			bool flag = UIManager.Instance.IsFocusElement(UIElement.ItemMultiplyOptionOld);
			if (flag)
			{
				UIManager.Instance.HideUI(UIElement.ItemMultiplyOptionOld);
			}
			else
			{
				RectTransform rectTrans = this._btnMultiplyOption.GetComponent<RectTransform>();
				Vector3 localPos = default(Vector3).SetY(rectTrans.rect.height * 0.5f);
				Vector3 pos = rectTrans.TransformPoint(localPos);
				ArgumentBox args = EasyPool.Get<ArgumentBox>().SetObject("AnchorItem", rectTrans).SetObject("Pos", pos).Set("Type", this.CurItemGradeFilterSourceType).Set("IsGearMate", true);
				UIElement.ItemMultiplyOptionOld.SetOnInitArgs(args);
				UIManager.Instance.ShowUI(UIElement.ItemMultiplyOptionOld, true);
			}
		}

		// Token: 0x060049F5 RID: 18933 RVA: 0x00228BDC File Offset: 0x00226DDC
		public override void ConfirmClick()
		{
			this.OnConfirm();
		}

		// Token: 0x060049F6 RID: 18934 RVA: 0x00228BE8 File Offset: 0x00226DE8
		public override void PointEnterConfirmButton()
		{
			bool flag = this.ConfirmButtonInteractable();
			if (flag)
			{
				base.PointEnterConfirmButton();
			}
		}

		// Token: 0x060049F7 RID: 18935 RVA: 0x00228C08 File Offset: 0x00226E08
		private int RenderPagesPreview(ItemKey itemKey, sbyte[] progressRead, SkillBookPageDisplayData pageDisplayData, bool doNotAddProgress, short skillBookId)
		{
			GearMateSubPageSkill.<>c__DisplayClass79_0 CS$<>8__locals1;
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.skillBookId = skillBookId;
			CS$<>8__locals1.pageDisplayData = pageDisplayData;
			CS$<>8__locals1.doNotAddProgress = doNotAddProgress;
			CS$<>8__locals1.progressRead = progressRead;
			CS$<>8__locals1.bookIds = new List<int>();
			CS$<>8__locals1.configData = SkillBook.Instance[itemKey.TemplateId];
			int finishedCountAfterRead = 0;
			for (int i = 0; i < this.PageCount; i++)
			{
				Refers pageRefers = this._pagesRefers.CGet<Refers>(string.Format("Page_{0}", i));
				bool isFinishedAfter = this.<RenderPagesPreview>g__RenderSinglePage|79_1(pageRefers, i, ref CS$<>8__locals1);
				bool flag = isFinishedAfter;
				if (flag)
				{
					finishedCountAfterRead++;
				}
			}
			bool flag2 = this.isCombatSkill;
			if (flag2)
			{
				this.<RenderPagesPreview>g__RefreshPagesTip|79_0(ref CS$<>8__locals1);
			}
			return finishedCountAfterRead;
		}

		// Token: 0x060049F8 RID: 18936 RVA: 0x00228CD0 File Offset: 0x00226ED0
		private void RenderChapterIfRead(sbyte[] progressRead)
		{
			this._readIds.Clear();
			for (int i = 0; i < 15; i++)
			{
				bool read = progressRead[i] >= 100;
				bool flag = read;
				if (flag)
				{
					this._readIds.Add(i);
				}
				this.GetChapter(i).CGet<GameObject>("LabelOn").SetActive(read);
				this.GetChapter(i).CGet<GameObject>("LabelOff").SetActive(!read);
				this.GetChapter(i).CGet<DisableStyleRoot>("DisableRoot").SetStyleEffect(!read, false);
			}
		}

		// Token: 0x060049F9 RID: 18937 RVA: 0x00228D6C File Offset: 0x00226F6C
		private static List<int> GetReadIdsFromProgressArray(sbyte[] progressRead)
		{
			List<int> readIds = new List<int>();
			for (int i = 0; i < 15; i++)
			{
				bool flag = progressRead[i] >= 100;
				if (flag)
				{
					readIds.Add(i);
				}
			}
			return readIds;
		}

		// Token: 0x060049FA RID: 18938 RVA: 0x00228DB0 File Offset: 0x00226FB0
		private void InitChapters()
		{
			for (int i = 0; i < this._chapters.childCount; i++)
			{
				Refers refers = this._chapters.GetChild(i).GetComponent<Refers>();
				refers.CGet<GameObject>("Back").SetActive(false);
				refers.CGet<GameObject>("LabelOn").SetActive(false);
				refers.CGet<GameObject>("LabelOff").SetActive(true);
				refers.CGet<DisableStyleRoot>("DisableRoot").SetStyleEffect(true, false);
			}
		}

		// Token: 0x060049FB RID: 18939 RVA: 0x00228E38 File Offset: 0x00227038
		private Refers GetChapter(int id)
		{
			return this._chapters.GetChild(id).GetComponent<Refers>();
		}

		// Token: 0x060049FC RID: 18940 RVA: 0x00228E5C File Offset: 0x0022705C
		private void RenderBookAfter(ItemKey itemKey, sbyte[] progressRead, SkillBookPageDisplayData pageDisplayData)
		{
			bool flag = !itemKey.IsValid();
			if (!flag)
			{
				TextMeshProUGUI bookName = this._curBookAfter.CGet<TextMeshProUGUI>("Name");
				CImage gradeBack = this._curBookAfter.CGet<CImage>("GradeBack");
				TextMeshProUGUI grade = this._curBookAfter.CGet<TextMeshProUGUI>("Grade");
				SkillBookItem configData = SkillBook.Instance[itemKey.TemplateId];
				bookName.text = configData.Name;
				grade.text = LocalStringManager.Get(string.Format("LK_ShortGrade_{0}", configData.Grade));
				gradeBack.SetSprite(ItemView.GetGradeIcon(configData.Grade), false, null);
				for (int i = 0; i < 6; i++)
				{
					Refers stateRefers = this._curBookAfter.CGet<Refers>(string.Format("ReadState_{0}", i));
					bool flag2 = i >= this.PageCount;
					if (flag2)
					{
						stateRefers.gameObject.SetActive(false);
					}
					else
					{
						stateRefers.gameObject.SetActive(true);
						int realId = i;
						bool flag3 = this.isCombatSkill;
						if (flag3)
						{
							realId = (int)this.GetSkillBookRealPage(pageDisplayData, i);
						}
						stateRefers.CGet<GameObject>("Yes").SetActive(progressRead[realId] >= 100);
						stateRefers.CGet<GameObject>("No").SetActive(progressRead[realId] < 100);
					}
				}
			}
		}

		// Token: 0x060049FD RID: 18941 RVA: 0x00228FC8 File Offset: 0x002271C8
		private void RenderBookNow(ItemKey itemKey)
		{
			bool flag = !itemKey.IsValid();
			if (!flag)
			{
				TextMeshProUGUI bookName = this._curBookNow.CGet<TextMeshProUGUI>("Name");
				CImage gradeBack = this._curBookNow.CGet<CImage>("GradeBack");
				TextMeshProUGUI grade = this._curBookNow.CGet<TextMeshProUGUI>("Grade");
				SkillBookItem configData = SkillBook.Instance[itemKey.TemplateId];
				SkillBookPageDisplayData pageDisplayData = this._pageDisplayData[itemKey.Id];
				bookName.text = configData.Name;
				grade.text = LocalStringManager.Get(string.Format("LK_ShortGrade_{0}", configData.Grade));
				gradeBack.SetSprite(ItemView.GetGradeIcon(configData.Grade), false, null);
				for (int i = 0; i < 6; i++)
				{
					CImage pageImg = this._curBookNow.CGet<CImage>(string.Format("Page_{0}", i));
					bool flag2 = i >= this.PageCount;
					if (flag2)
					{
						pageImg.transform.parent.gameObject.SetActive(false);
					}
					else
					{
						pageImg.transform.parent.gameObject.SetActive(true);
						ReadingDisplayHelper.SetPageCompleteState(pageDisplayData.State[i], pageImg);
					}
				}
			}
		}

		// Token: 0x060049FE RID: 18942 RVA: 0x00229118 File Offset: 0x00227318
		private void OnRenderScrollBook(int index, Refers refers)
		{
			ItemKey itemKey = this._availableBookList[index];
			CToggleObsolete bookTog = this.OnRenderBook(index, refers, itemKey);
			bookTog.gameObject.SetActive(false);
			bookTog.onValueChanged.RemoveAllListeners();
			bookTog.isOn = this._selectedBooks.Contains(itemKey);
			bookTog.onValueChanged.AddListener(delegate(bool isOn)
			{
				if (isOn)
				{
					this.DeselectOtherBook(itemKey);
					this.SelectBook(itemKey);
					bool flag2 = this._currentSelectBookToggle != null;
					if (flag2)
					{
						this._currentSelectBookToggle.isOn = (bookTog == this._currentSelectBookToggle);
					}
					this._currentSelectBookToggle = bookTog;
				}
				else
				{
					bool flag3 = this._selectedBooks.Contains(itemKey);
					if (flag3)
					{
						this.DeselectBook(itemKey);
					}
				}
			});
			bookTog.gameObject.SetActive(true);
			bookTog.interactable = true;
			SkillBookItem config = SkillBook.Instance[itemKey.TemplateId];
			short templateId = config.LifeSkillTemplateId;
			bool flag = this.isCombatSkill;
			if (flag)
			{
				templateId = config.CombatSkillTemplateId;
			}
			TooltipInvoker disPlayer = refers.CGet<TooltipInvoker>("MouseTipDisplayer");
			disPlayer.Type = TipType.SingleDesc;
			TooltipInvoker tooltipInvoker = disPlayer;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = new ArgumentBox();
			}
			disPlayer.RuntimeParam.Set("arg0", LocalStringManager.Get(LanguageKey.UI_MouseTipGearMateSkillBookRead));
			bool bookEnabled = !this._disabledBooks.Contains((int)templateId);
			bookTog.interactable = bookEnabled;
			PointerTrigger pointerTrigger = refers.GetComponent<PointerTrigger>();
			pointerTrigger.enabled = bookEnabled;
			refers.CGet<DisableStyleRoot>("DisableStyleRoot").SetStyleEffect(!bookEnabled, false);
			disPlayer.enabled = !bookEnabled;
		}

		// Token: 0x060049FF RID: 18943 RVA: 0x002292A4 File Offset: 0x002274A4
		private CToggleObsolete OnRenderBook(int index, Refers refers, ItemKey itemKey)
		{
			SkillBookItem configData = SkillBook.Instance[itemKey.TemplateId];
			bool flag = !this._bookDisplayData.ContainsKey(itemKey.Id);
			CToggleObsolete result;
			if (flag)
			{
				result = null;
			}
			else
			{
				ItemDisplayData itemDisplayData = this._bookDisplayData[itemKey.Id];
				SkillBookPageDisplayData pageDisplayData = this._pageDisplayData[itemKey.Id];
				CToggleObsolete bookTog = refers.GetComponent<CToggleObsolete>();
				refers.CGet<TextMeshProUGUI>("Name").text = configData.Name;
				refers.CGet<TextMeshProUGUI>("Durability").text = CommonUtils.GetDurabilityString((int)itemDisplayData.Durability, (int)itemDisplayData.MaxDurability);
				CImage gradeBack = refers.CGet<CImage>("GradeBack");
				TextMeshProUGUI grade = refers.CGet<TextMeshProUGUI>("Grade");
				grade.text = LocalStringManager.Get(string.Format("LK_ShortGrade_{0}", configData.Grade));
				gradeBack.SetSprite(ItemView.GetGradeIcon(configData.Grade), false, null);
				int pageCount = (configData.ItemSubType == 1001) ? 6 : 5;
				for (int i = 0; i < 6; i++)
				{
					CImage pageImg = refers.CGet<CImage>(string.Format("Page_{0}", i));
					bool flag2 = i >= pageCount;
					if (flag2)
					{
						pageImg.transform.parent.gameObject.SetActive(false);
					}
					else
					{
						pageImg.transform.parent.gameObject.SetActive(true);
						ReadingDisplayHelper.SetPageCompleteState(pageDisplayData.State[i], pageImg);
					}
				}
				result = bookTog;
			}
			return result;
		}

		// Token: 0x06004A00 RID: 18944 RVA: 0x0022943C File Offset: 0x0022763C
		protected override void HandleDataModification(Notification notification, NotificationWrapper wrapper)
		{
			base.HandleDataModification(notification, wrapper);
			ushort domianId = notification.Uid.DomainId;
			ushort dataId = notification.Uid.DataId;
			RawDataPool pool = wrapper.DataPool;
			int offset = notification.ValueOffset;
			bool flag = domianId == 4;
			if (flag)
			{
				bool flag2 = dataId == 0;
				if (flag2)
				{
					bool flag3 = (int)notification.Uid.SubId0 == this.taiwuId;
					if (flag3)
					{
						bool flag4 = notification.Uid.SubId1 == 66U;
						if (flag4)
						{
							Serializer.Deserialize(pool, offset, ref this._exp);
							this._have.text = this._exp.ToString();
						}
					}
					else
					{
						bool flag5 = (int)notification.Uid.SubId0 == this.CurGearMateId;
						if (flag5)
						{
							bool flag6 = notification.Uid.SubId1 == 32U;
							if (flag6)
							{
								bool flag7 = this.isCombatSkill;
								if (flag7)
								{
									Serializer.Deserialize(pool, offset, ref this._combatSkillQualifications);
								}
							}
							else
							{
								bool flag8 = notification.Uid.SubId1 == 30U;
								if (flag8)
								{
									bool flag9 = !this.isCombatSkill;
									if (flag9)
									{
										Serializer.Deserialize(pool, offset, ref this._lifeSkillQualifications);
									}
								}
								else
								{
									bool flag10 = notification.Uid.SubId1 == 99U;
									if (flag10)
									{
										bool flag11 = this.isCombatSkill;
										if (flag11)
										{
											Serializer.Deserialize(pool, offset, ref this._combatSkillAttainments);
										}
									}
									else
									{
										bool flag12 = notification.Uid.SubId1 == 97U;
										if (flag12)
										{
											bool flag13 = !this.isCombatSkill;
											if (flag13)
											{
												Serializer.Deserialize(pool, offset, ref this._lifeSkillAttainments);
											}
										}
										else
										{
											bool flag14 = notification.Uid.SubId1 == 29U;
											if (flag14)
											{
												bool flag15 = !this.isCombatSkill;
												if (flag15)
												{
													List<GameData.Domains.Character.LifeSkillItem> list = null;
													Serializer.Deserialize(pool, offset, ref list);
													this._learnedLifeSkills.Clear();
													bool flag16 = list != null;
													if (flag16)
													{
														this._learnedLifeSkills.AddRange(list);
													}
												}
											}
											else
											{
												bool flag17 = notification.Uid.SubId1 == 61U;
												if (flag17)
												{
													bool flag18 = this.isCombatSkill;
													if (flag18)
													{
														short[] panelsData = null;
														Serializer.Deserialize(pool, offset, ref panelsData);
														this._attainmentPanels = panelsData;
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06004A01 RID: 18945 RVA: 0x00229690 File Offset: 0x00227890
		public override void HandleMethodReturn(Notification notification, NotificationWrapper wrapper)
		{
			ushort domainId = notification.DomainId;
			ushort methodId = notification.MethodId;
			RawDataPool pool = wrapper.DataPool;
			int offset = notification.ValueOffset;
			bool flag = domainId == 19;
			if (flag)
			{
				bool flag2 = methodId == 154;
				if (flag2)
				{
					List<ItemDisplayData> list = null;
					Serializer.Deserialize(pool, offset, ref list);
					foreach (ItemSourceType type in this._itemSourceTypeArray)
					{
						this._skillBooks.TryAdd(type, new List<ItemKey>());
					}
					List<ItemKey> list2 = new List<ItemKey>();
					bool flag3 = list != null;
					if (flag3)
					{
						foreach (ItemDisplayData item in list)
						{
							this._skillBooks[item.ItemSourceTypeEnum].Add(item.Key);
							this._bookDisplayData[item.Key.Id] = item;
							list2.Add(item.Key);
						}
						ItemDomainMethod.Call.GetSkillBookPageDisplayDataList(base.ListenerId, list2);
					}
					else
					{
						this.RefreshReadBooks();
						TaiwuDomainMethod.Call.CanTransferItemToWarehouse(base.ListenerId);
					}
				}
			}
			else
			{
				bool flag4 = domainId == 6;
				if (flag4)
				{
					bool flag5 = methodId == 28;
					if (flag5)
					{
						List<SkillBookPageDisplayData> list3 = null;
						Serializer.Deserialize(pool, offset, ref list3);
						foreach (SkillBookPageDisplayData item2 in list3)
						{
							this._pageDisplayData[item2.ItemKey.Id] = item2;
						}
						this.RefreshReadBooks();
						TaiwuDomainMethod.Call.CanTransferItemToWarehouse(base.ListenerId);
					}
				}
				else
				{
					bool flag6 = domainId == 5;
					if (flag6)
					{
						bool flag7 = methodId == 42;
						if (flag7)
						{
							bool canTransfer = true;
							Serializer.Deserialize(pool, offset, ref canTransfer);
							Transform toggleGroupTransform = this._itemSourceToggleGroup.transform;
							for (int i = 1; i < 3; i++)
							{
								CToggleObsolete toggle = toggleGroupTransform.GetChild(i).GetComponent<CToggleObsolete>();
								toggle.interactable = canTransfer;
								toggle.GetComponent<TooltipInvoker>().enabled = !toggle.interactable;
							}
							CToggleObsolete tog = this._itemSourceToggleGroup.Get(0);
							this._itemSourceToggleGroup.Set(tog, true);
							this.OnItemSourceToggleChange(tog, null);
						}
					}
				}
			}
		}

		// Token: 0x06004A02 RID: 18946 RVA: 0x00229928 File Offset: 0x00227B28
		private int CompareBook(ItemKey a, ItemKey b)
		{
			SkillBookItem configA = SkillBook.Instance[a.TemplateId];
			short templateIdA = configA.LifeSkillTemplateId;
			bool flag = this.isCombatSkill;
			if (flag)
			{
				templateIdA = configA.CombatSkillTemplateId;
			}
			SkillBookItem configB = SkillBook.Instance[b.TemplateId];
			short templateIdB = configB.LifeSkillTemplateId;
			bool flag2 = this.isCombatSkill;
			if (flag2)
			{
				templateIdB = configB.CombatSkillTemplateId;
			}
			bool aDisabled = this._disabledBooks.Contains((int)templateIdA);
			bool bDisabled = this._disabledBooks.Contains((int)templateIdB);
			bool flag3 = aDisabled != bDisabled;
			int result;
			if (flag3)
			{
				result = (aDisabled ? 1 : -1);
			}
			else
			{
				bool flag4 = a.TemplateId != b.TemplateId;
				if (flag4)
				{
					result = a.TemplateId.CompareTo(b.TemplateId);
				}
				else
				{
					result = a.Id.CompareTo(b.Id);
				}
			}
			return result;
		}

		// Token: 0x06004A03 RID: 18947 RVA: 0x00229A08 File Offset: 0x00227C08
		private void RefreshBtnTip()
		{
			TooltipInvoker disPlayer = base.CGet<TooltipInvoker>("MouseTipDisplayer");
			string title = this.isCombatSkill ? LocalStringManager.Get(LanguageKey.LK_GearMateCombatSkillTitle) : LocalStringManager.Get(LanguageKey.LK_GearMateLifeSkillTitle);
			string gearMateName = NameCenter.GetMonasticTitleOrDisplayName(base.Parent.GearMateDisplayData, false);
			disPlayer.enabled = true;
			TooltipInvoker tooltipInvoker = disPlayer;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = new ArgumentBox();
			}
			disPlayer.Type = TipType.GeneralLines;
			disPlayer.RuntimeParam.Set("Title", title);
			int lineCount = 0;
			string lineOne = LocalStringManager.GetFormat(LanguageKey.UI_MouseTipGearMateSkillBtn_2, gearMateName);
			string lineTwo = LocalStringManager.Get(LanguageKey.UI_MouseTipGearMateSkillBtn_3);
			string line3 = LocalStringManager.Get(LanguageKey.UI_MouseTipGearMateSkillBtn_4);
			string line4 = LocalStringManager.Get(LanguageKey.UI_MouseTipGearMateSkillBtn_5);
			string line5 = LocalStringManager.Get(LanguageKey.UI_MouseTipGearMateSkillBtn_6);
			disPlayer.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData(7, new List<string>
			{
				lineOne
			}, null));
			disPlayer.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData(1, new List<string>
			{
				lineTwo
			}, null));
			disPlayer.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData(2, new List<string>
			{
				"mousetip_bookstate_0",
				line3
			}, null));
			disPlayer.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData(2, new List<string>
			{
				"mousetip_bookstate_1",
				line4
			}, null));
			disPlayer.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData(2, new List<string>
			{
				"mousetip_bookstate_2",
				line5
			}, null));
			disPlayer.RuntimeParam.Set("LineCount", lineCount);
			disPlayer.Refresh(false, -1);
		}

		// Token: 0x06004A04 RID: 18948 RVA: 0x00229C18 File Offset: 0x00227E18
		private void RefreshReadBooks()
		{
			this._disabledBooks.Clear();
			foreach (List<ItemKey> skillBooks in this._skillBooks.Values)
			{
				foreach (ItemKey book in skillBooks)
				{
					bool flag = this.IsBookRead(book);
					if (flag)
					{
						this._disabledBooks.Add((int)book.TemplateId);
					}
				}
			}
		}

		// Token: 0x06004A05 RID: 18949 RVA: 0x00229CD0 File Offset: 0x00227ED0
		private void RefreshBooks()
		{
			this._availableBookList.Clear();
			List<ItemKey> books;
			bool flag = !this._skillBooks.TryGetValue(this._itemSourceType, out books);
			if (!flag)
			{
				foreach (ItemKey itemKey in books)
				{
					bool flag2 = this._usedBooks.Contains(itemKey);
					if (!flag2)
					{
						SkillBookItem config = SkillBook.Instance[itemKey.TemplateId];
						bool flag3 = this.isCombatSkill;
						if (flag3)
						{
							int toggleKey = this.SkillTypeIndex;
							bool flag4 = toggleKey == -1;
							if (flag4)
							{
								this._availableBookList.Add(itemKey);
							}
							else
							{
								bool flag5 = toggleKey >= 0 && toggleKey < CombatSkillTypeTogGroup.ToggleConfig.Count;
								if (flag5)
								{
									CombatSkillTypeTogGroup.ToggleInfo toggleInfo = CombatSkillTypeTogGroup.ToggleConfig[toggleKey];
									CombatSkillItem skillConfig = CombatSkill.Instance[config.CombatSkillTemplateId];
									bool flag6 = skillConfig.Type == toggleInfo.CombatSkillType && skillConfig.EquipType == toggleInfo.EquipType;
									if (flag6)
									{
										this._availableBookList.Add(itemKey);
									}
								}
							}
						}
						else
						{
							sbyte skillType = config.LifeSkillType;
							bool flag7 = this.SkillTypeIndex == -1 || this.SkillTypeIndex == (int)skillType;
							if (flag7)
							{
								this._availableBookList.Add(itemKey);
							}
						}
					}
				}
				this._availableBookList.Sort(new Comparison<ItemKey>(this.CompareBook));
				this._selectBookScroll.UpdateData(this._availableBookList.Count);
				this._btnSelectAll.interactable = (this._availableBookList.Count != 0);
				this._btnSelectAll.GetComponent<Refers>().CGet<GameObject>("Normal").SetActive(this._availableBookList.Count != 0);
				this._btnSelectAll.GetComponent<Refers>().CGet<GameObject>("Disabled").SetActive(this._availableBookList.Count == 0);
			}
		}

		// Token: 0x06004A06 RID: 18950 RVA: 0x00229F00 File Offset: 0x00228100
		private void RefreshBook(ItemKey itemKey, bool doNotAddProgress)
		{
			SkillBookPageDisplayData pageDisplayData = this._pageDisplayData[itemKey.Id];
			short id = this.isCombatSkill ? SkillBook.Instance[itemKey.TemplateId].CombatSkillTemplateId : SkillBook.Instance[itemKey.TemplateId].LifeSkillTemplateId;
			sbyte[] progressRead;
			bool flag = !this._pageProgressDic.TryGetValue(id, out progressRead);
			if (flag)
			{
				progressRead = new sbyte[15];
			}
			bool flag2 = this.isCombatSkill;
			if (flag2)
			{
				this.InitChapters();
				this.RenderChapterIfRead(progressRead);
			}
			this.RenderBookAfter(itemKey, progressRead, pageDisplayData);
			this._finishedCountAfter = this.RenderPagesPreview(itemKey, progressRead, pageDisplayData, doNotAddProgress, id);
			this.RenderSkillType(itemKey, false);
		}

		// Token: 0x06004A07 RID: 18951 RVA: 0x00229FB8 File Offset: 0x002281B8
		private void RefreshBookAfterUsed()
		{
			this.RefreshBook(this._previewChapterItemKey, true);
			this._curBookNowEmpty.SetActive(true);
			this._curBookNow.gameObject.SetActive(false);
			this._pagesRefers.gameObject.SetActive(true);
			for (int i = 0; i < this.PageCount; i++)
			{
				Refers refers = this._pagesRefers.CGet<Refers>(string.Format("Page_{0}", i));
				Refers stateAfter = refers.CGet<Refers>("StateAfter");
				TextMeshProUGUI progressAfterComplete = refers.CGet<TextMeshProUGUI>("ProgressAfterComplete");
				TextMeshProUGUI progressAfterNotComplete = refers.CGet<TextMeshProUGUI>("ProgressAfterNotComplete");
				TextMeshProUGUI progressNow = refers.CGet<TextMeshProUGUI>("ProgressNow");
				GameObject arrow = refers.CGet<GameObject>("Arrow");
				Refers stateNow = refers.CGet<Refers>("StateNow");
				GameObject labelNow = refers.CGet<GameObject>("LabelNow");
				GameObject progressBack = refers.CGet<GameObject>("ProgressBack");
				progressBack.SetActive(false);
				stateNow.gameObject.SetActive(false);
				arrow.SetActive(false);
				progressNow.gameObject.SetActive(false);
				bool finished = progressAfterComplete.text.Equals("100%");
				progressAfterComplete.gameObject.SetActive(finished);
				progressAfterNotComplete.gameObject.SetActive(!finished);
				stateAfter.CGet<GameObject>("Yes").SetActive(finished);
				stateAfter.CGet<GameObject>("No").SetActive(!finished);
			}
		}

		// Token: 0x06004A08 RID: 18952 RVA: 0x0022A130 File Offset: 0x00228330
		private byte GetSkillBookRealPage(SkillBookPageDisplayData pageDisplayData, int i)
		{
			bool flag = !this.isCombatSkill;
			byte result;
			if (flag)
			{
				result = (byte)i;
			}
			else
			{
				bool flag2 = i == 0;
				if (flag2)
				{
					result = (byte)pageDisplayData.Type[0];
				}
				else
				{
					int idx = (pageDisplayData.Type[i] == 0) ? (4 + i) : (9 + i);
					result = (byte)idx;
				}
			}
			return result;
		}

		// Token: 0x06004A09 RID: 18953 RVA: 0x0022A188 File Offset: 0x00228388
		private void ShowBubble(float duration)
		{
			bool flag = this.isCombatSkill;
			LanguageKey[] levelUpKeys;
			LanguageKey[] normalKeys;
			if (flag)
			{
				levelUpKeys = new LanguageKey[]
				{
					LanguageKey.LK_GearMateCombatSkill_SpeakWord0,
					LanguageKey.LK_GearMateCombatSkill_SpeakWord1
				};
				normalKeys = new LanguageKey[]
				{
					LanguageKey.LK_GearMateCombatSkill_SpeakWord2,
					LanguageKey.LK_GearMateCombatSkill_SpeakWord3
				};
			}
			else
			{
				levelUpKeys = new LanguageKey[]
				{
					LanguageKey.LK_GearMateLifeSkill_SpeakWord0,
					LanguageKey.LK_GearMateLifeSkill_SpeakWord1
				};
				normalKeys = new LanguageKey[]
				{
					LanguageKey.LK_GearMateLifeSkill_SpeakWord2,
					LanguageKey.LK_GearMateLifeSkill_SpeakWord3
				};
			}
			int id = Random.Range(0, 2);
			base.Parent.Avatar.ShowBubble(this._isQualificationLevelUp ? LocalStringManager.Get(levelUpKeys[id]) : LocalStringManager.Get(normalKeys[id]), duration);
			base.Parent.Avatar.DoGearMateAnimation("break_2");
		}

		// Token: 0x06004A0A RID: 18954 RVA: 0x0022A24C File Offset: 0x0022844C
		private void SetEmptyState(bool empty)
		{
			bool flag = this.isCombatSkill && empty;
			if (flag)
			{
				this.InitChapters();
			}
			this._readStateAfterEmpty.SetActive(false);
			this.SetPagesEmpty(empty);
			this._curBookAfter.gameObject.SetActive(!empty);
			this._curBookNowEmpty.SetActive(empty);
			this._curBookNow.gameObject.SetActive(!empty);
			this._readStateNowEmpty.SetActive(false);
			this.SetSkillTypeEmpty(this._skillType, empty);
		}

		// Token: 0x06004A0B RID: 18955 RVA: 0x0022A2D4 File Offset: 0x002284D4
		private void SetPagesEmpty(bool empty)
		{
			for (int i = 0; i < this.PageCount; i++)
			{
				Refers refers = this._pagesRefers.CGet<Refers>(string.Format("Page_{0}", i));
				Refers stateAfter = refers.CGet<Refers>("StateAfter");
				TextMeshProUGUI progressAfterComplete = refers.CGet<TextMeshProUGUI>("ProgressAfterComplete");
				TextMeshProUGUI progressAfterNotComplete = refers.CGet<TextMeshProUGUI>("ProgressAfterNotComplete");
				TextMeshProUGUI progressNow = refers.CGet<TextMeshProUGUI>("ProgressNow");
				GameObject arrow = refers.CGet<GameObject>("Arrow");
				Refers stateNow = refers.CGet<Refers>("StateNow");
				GameObject labelNow = refers.CGet<GameObject>("LabelNow");
				GameObject progressBack = refers.CGet<GameObject>("ProgressBack");
				stateAfter.gameObject.SetActive(!empty);
				progressAfterComplete.gameObject.SetActive(!empty);
				progressAfterComplete.transform.parent.gameObject.SetActive(!empty);
				progressAfterNotComplete.gameObject.SetActive(!empty);
				progressNow.gameObject.SetActive(!empty);
				arrow.gameObject.SetActive(!empty);
				stateNow.gameObject.SetActive(!empty);
				labelNow.gameObject.SetActive(true);
				progressBack.gameObject.SetActive(!empty);
			}
		}

		// Token: 0x06004A0C RID: 18956 RVA: 0x0022A420 File Offset: 0x00228620
		private void SetSkillTypeEmpty(Refers refers, bool empty)
		{
			TextMeshProUGUI attainmentDesc = refers.CGet<TextMeshProUGUI>("AttainmentDesc");
			TextMeshProUGUI typeName = refers.CGet<TextMeshProUGUI>("TypeName");
			CImage skillIcon = refers.CGet<CImage>("SkillIcon");
			TextMeshProUGUI qualificationNow = refers.CGet<TextMeshProUGUI>("QualificationNow");
			TextMeshProUGUI qualificationAdd = refers.CGet<TextMeshProUGUI>("QualificationAdd");
			TextMeshProUGUI attainmentNow = refers.CGet<TextMeshProUGUI>("AttainmentNow");
			TextMeshProUGUI attainmentAdd = refers.CGet<TextMeshProUGUI>("AttainmentAdd");
			RectTransform stages = refers.CGet<RectTransform>("Stages");
			TooltipInvoker disPlayer = refers.CGet<TooltipInvoker>("MouseTipDisplayer");
			GameObject qualficationEmpty = refers.CGet<GameObject>("QualficationEmpty");
			GameObject atainmentEmpty = refers.CGet<GameObject>("AttainmentEmpty");
			RectTransform stagesEmpty = refers.CGet<RectTransform>("StagesEmpty");
			qualificationAdd.gameObject.SetActive(!empty);
			attainmentAdd.gameObject.SetActive(!empty);
			attainmentDesc.transform.parent.gameObject.SetActive(!empty);
			typeName.gameObject.SetActive(!empty);
			skillIcon.gameObject.SetActive(!empty);
			disPlayer.enabled = false;
			attainmentNow.gameObject.SetActive(!empty);
			qualificationNow.gameObject.SetActive(!empty);
			qualficationEmpty.SetActive(empty);
			atainmentEmpty.SetActive(empty);
			stages.gameObject.SetActive(!empty);
			stagesEmpty.gameObject.SetActive(empty);
			bool flag = !empty;
			if (!flag)
			{
				for (sbyte grade = 0; grade < 9; grade += 1)
				{
					Refers stageRefers = stagesEmpty.GetChild((int)grade).GetComponent<Refers>();
					CImage icon = stageRefers.CGet<CImage>("Icon");
					TextMeshProUGUI gradeText = stageRefers.CGet<TextMeshProUGUI>("Grade");
					gradeText.text = LocalStringManager.Get(string.Format("LK_ShortGrade_{0}", grade));
					icon.SetSprite("sp_icon_pinji_0", false, null);
					icon.SetColor(Color.gray);
				}
			}
		}

		// Token: 0x06004A0D RID: 18957 RVA: 0x0022A60C File Offset: 0x0022880C
		private bool IsBookRead(ItemKey key)
		{
			SkillBookItem config = SkillBook.Instance[key.TemplateId];
			short templateId = this.isCombatSkill ? config.CombatSkillTemplateId : config.LifeSkillTemplateId;
			sbyte progress;
			return this._bookTotalProgressDic.TryGetValue(templateId, out progress) && progress >= 100;
		}

		// Token: 0x06004A0E RID: 18958 RVA: 0x0022A664 File Offset: 0x00228864
		private int GetBookRequiredExp(ItemKey key)
		{
			int needReadCount = 0;
			short templateId = this.isCombatSkill ? SkillBook.Instance[key.TemplateId].CombatSkillTemplateId : SkillBook.Instance[key.TemplateId].LifeSkillTemplateId;
			sbyte[] progressRead;
			bool flag = !this._pageProgressDic.TryGetValue(templateId, out progressRead);
			if (flag)
			{
				progressRead = new sbyte[15];
			}
			for (int i = 0; i < this.PageCount; i++)
			{
				bool flag2 = progressRead[(int)this.GetSkillBookRealPage(this._pageDisplayData[key.Id], i)] < 100;
				if (flag2)
				{
					needReadCount++;
				}
			}
			sbyte grade = ItemTemplateHelper.GetGrade(key.ItemType, key.TemplateId);
			short pageExp = SkillGradeData.Instance[grade].ReadingExpGainPerPage;
			return (int)pageExp * needReadCount;
		}

		// Token: 0x06004A0F RID: 18959 RVA: 0x0022A73C File Offset: 0x0022893C
		private unsafe void GetProgressAdd(ItemKey itemKey, ref int* progressAddArr)
		{
			SkillBookPageDisplayData pageDisplayData = this._pageDisplayData[itemKey.Id];
			for (int i = 0; i < this.PageCount; i++)
			{
				sbyte progressAdd = 10;
				sbyte pageSatte = pageDisplayData.State[i];
				sbyte b = pageSatte;
				sbyte b2 = b;
				if (b2 != 0)
				{
					if (b2 == 1)
					{
						progressAdd = 40;
					}
				}
				else
				{
					progressAdd = 100;
				}
				*(progressAddArr + (IntPtr)i * 4) += (int)progressAdd;
			}
		}

		// Token: 0x06004A10 RID: 18960 RVA: 0x0022A7AC File Offset: 0x002289AC
		private bool CheckIsFullProgress(ItemKey itemKey)
		{
			SkillBookPageDisplayData pageDisplayData = this._pageDisplayData[itemKey.Id];
			short id = this.isCombatSkill ? SkillBook.Instance[itemKey.TemplateId].CombatSkillTemplateId : SkillBook.Instance[itemKey.TemplateId].LifeSkillTemplateId;
			sbyte[] progressRead;
			bool flag = !this._pageProgressDic.TryGetValue(id, out progressRead);
			if (flag)
			{
				progressRead = new sbyte[15];
			}
			int totalProgress = 0;
			for (int i = 0; i < this.PageCount; i++)
			{
				totalProgress += (int)progressRead[(int)this.GetSkillBookRealPage(pageDisplayData, i)];
			}
			return totalProgress == this.PageCount * 100;
		}

		// Token: 0x06004A11 RID: 18961 RVA: 0x0022A860 File Offset: 0x00228A60
		private void SelectBook(ItemKey itemKey)
		{
			SkillBookPageDisplayData pageDisplayData = this._pageDisplayData[itemKey.Id];
			short id = this.isCombatSkill ? SkillBook.Instance[itemKey.TemplateId].CombatSkillTemplateId : SkillBook.Instance[itemKey.TemplateId].LifeSkillTemplateId;
			sbyte[] progressRead;
			bool flag = !this._pageProgressDic.TryGetValue(id, out progressRead);
			if (flag)
			{
				progressRead = new sbyte[15];
			}
			this._expRequirement += this.GetBookRequiredExp(itemKey);
			for (int i = 0; i < this.PageCount; i++)
			{
				sbyte progressAdd = 10;
				sbyte pageSatte = pageDisplayData.State[i];
				sbyte b = pageSatte;
				sbyte b2 = b;
				if (b2 != 0)
				{
					if (b2 == 1)
					{
						progressAdd = 40;
					}
				}
				else
				{
					progressAdd = 100;
				}
				byte realId = this.GetSkillBookRealPage(pageDisplayData, i);
				this._isAdvance[i] = (progressRead[(int)realId] < 100);
				progressRead[(int)realId] = (sbyte)Mathf.Min((int)(progressRead[(int)realId] + progressAdd), 100);
			}
			this._pageProgressDic[id] = progressRead;
			this._selectedBooks.Add(itemKey);
			this.OnSelectedBookChanged();
		}

		// Token: 0x06004A12 RID: 18962 RVA: 0x0022A984 File Offset: 0x00228B84
		private unsafe void DeselectBook(ItemKey itemKey)
		{
			GearMateSubPageSkill.<>c__DisplayClass106_0 CS$<>8__locals1;
			CS$<>8__locals1.<>4__this = this;
			short id = this.isCombatSkill ? SkillBook.Instance[itemKey.TemplateId].CombatSkillTemplateId : SkillBook.Instance[itemKey.TemplateId].LifeSkillTemplateId;
			bool flag = !this._pageProgressDic.TryGetValue(id, out CS$<>8__locals1.progressRead);
			if (flag)
			{
				CS$<>8__locals1.progressRead = new sbyte[15];
			}
			CS$<>8__locals1.progressReadDefault = this.GetDefaultProgressRead(id);
			CS$<>8__locals1.pageDisplayData = this._pageDisplayData[itemKey.Id];
			this.<DeselectBook>g__ResetProgressRead|106_1(CS$<>8__locals1.pageDisplayData, CS$<>8__locals1.progressRead, CS$<>8__locals1.progressReadDefault, ref CS$<>8__locals1);
			int tempExpCost = 0;
			int* progressAddArr = stackalloc int[checked(unchecked((UIntPtr)this.PageCount) * 4)];
			CS$<>8__locals1.progressAddArr = progressAddArr;
			List<ItemKey> curTemplateItemkeyList = EasyPool.Get<List<ItemKey>>();
			curTemplateItemkeyList.Clear();
			foreach (ItemKey bookItemkey in this._selectedBooks)
			{
				short bookItemkeyId = this.isCombatSkill ? SkillBook.Instance[bookItemkey.TemplateId].CombatSkillTemplateId : SkillBook.Instance[bookItemkey.TemplateId].LifeSkillTemplateId;
				bool flag2 = bookItemkeyId == id;
				if (flag2)
				{
					this.<DeselectBook>g__CalcExpCost|106_0(ref tempExpCost, bookItemkey, ref CS$<>8__locals1);
					curTemplateItemkeyList.Add(bookItemkey);
				}
			}
			this._expRequirement -= tempExpCost;
			this._selectedBooks.Remove(itemKey);
			curTemplateItemkeyList.Remove(itemKey);
			for (int i = 0; i < this.PageCount; i++)
			{
				CS$<>8__locals1.progressAddArr[i] = 0;
			}
			this.<DeselectBook>g__ResetProgressRead|106_1(CS$<>8__locals1.pageDisplayData, CS$<>8__locals1.progressRead, CS$<>8__locals1.progressReadDefault, ref CS$<>8__locals1);
			for (int j = 0; j < curTemplateItemkeyList.Count; j++)
			{
				this.<DeselectBook>g__CalcExpCost|106_0(ref this._expRequirement, curTemplateItemkeyList[j], ref CS$<>8__locals1);
			}
			curTemplateItemkeyList.Clear();
			EasyPool.Free<List<ItemKey>>(curTemplateItemkeyList);
			bool flag3 = this._selectedBooks.Count > 0;
			if (flag3)
			{
				List<ItemKey> selectedBooks = this._selectedBooks;
				this.RefreshIsAdvance(selectedBooks[selectedBooks.Count - 1]);
			}
			this.OnSelectedBookChanged();
		}

		// Token: 0x06004A13 RID: 18963 RVA: 0x0022ABE4 File Offset: 0x00228DE4
		private void DeselectOtherBook(ItemKey itemKey)
		{
			List<ItemKey> toRemoveKey = new List<ItemKey>();
			foreach (ItemKey item in this._selectedBooks)
			{
				bool flag = item != itemKey;
				if (flag)
				{
					toRemoveKey.Add(item);
				}
			}
			foreach (ItemKey item2 in toRemoveKey)
			{
				this.DeselectBook(item2);
			}
		}

		// Token: 0x06004A14 RID: 18964 RVA: 0x0022AC98 File Offset: 0x00228E98
		private sbyte[] GetDefaultProgressRead(short id)
		{
			sbyte[] progressReadDefault;
			bool flag = !this._pageProgressDicDefault.TryGetValue(id, out progressReadDefault);
			if (flag)
			{
				progressReadDefault = new sbyte[15];
			}
			return progressReadDefault;
		}

		// Token: 0x06004A15 RID: 18965 RVA: 0x0022ACCC File Offset: 0x00228ECC
		private void DeselectAllBooks()
		{
			foreach (KeyValuePair<short, sbyte[]> keyValuePair in this._pageProgressDic)
			{
				short num;
				sbyte[] array;
				keyValuePair.Deconstruct(out num, out array);
				short key = num;
				sbyte[] value = array;
				Array.Copy(this._pageProgressDicDefault.ContainsKey(key) ? this._pageProgressDicDefault[key] : new sbyte[15], this._pageProgressDic[key], value.Length);
			}
			this._selectedBooks.Clear();
			this._expRequirement = 0;
		}

		// Token: 0x06004A16 RID: 18966 RVA: 0x0022AD7C File Offset: 0x00228F7C
		private List<ItemKey> GetMultiplySelectResult(sbyte limit)
		{
			List<ItemKey> res = new List<ItemKey>();
			bool flag = limit < 0;
			if (flag)
			{
				limit = sbyte.MaxValue;
			}
			foreach (ItemKey itemKey in this._availableBookList)
			{
				bool flag2 = !this._disabledBooks.Contains((int)itemKey.TemplateId) && ItemTemplateHelper.GetGrade(itemKey.ItemType, itemKey.TemplateId) < limit;
				if (flag2)
				{
					res.Add(itemKey);
				}
			}
			return res;
		}

		// Token: 0x06004A17 RID: 18967 RVA: 0x0022AE1C File Offset: 0x0022901C
		private bool CompareSelectedList(List<ItemKey> list)
		{
			bool isAllSelected = true;
			foreach (ItemKey itemkey in list)
			{
				bool flag = !this.CheckIsFullProgress(itemkey) && !this._selectedBooks.Contains(itemkey);
				if (flag)
				{
					isAllSelected = false;
					this.SelectBook(itemkey);
				}
			}
			return isAllSelected;
		}

		// Token: 0x06004A18 RID: 18968 RVA: 0x0022AE9C File Offset: 0x0022909C
		private void SelectBooks(List<ItemKey> books)
		{
			foreach (ItemKey itemKey in books)
			{
				this._selectedBooks.Add(itemKey);
				this._expRequirement += this.GetBookRequiredExp(itemKey);
			}
		}

		// Token: 0x06004A19 RID: 18969 RVA: 0x0022AF0C File Offset: 0x0022910C
		private void OnSelectedBookChanged()
		{
			this._need.text = this._expRequirement.ToString();
			this._have.text = this._exp.ToString().SetColor((this._exp >= this._expRequirement) ? "brightblue" : "brightred");
			this.UpdateConfirmButton();
			bool flag = this._selectedBooks.Count == 0;
			if (flag)
			{
				this._curBookKey = ItemKey.Invalid;
				bool flag2 = !this._previewChapterItemKey.IsValid();
				if (flag2)
				{
					this.SetEmptyState(true);
				}
				bool flag3 = base.GearMate != null;
				if (flag3)
				{
					this.RenderSkillType(this._curBookKey, true);
				}
			}
			else
			{
				List<ItemKey> selectedBooks = this._selectedBooks;
				this._curBookKey = selectedBooks[selectedBooks.Count - 1];
				this.SetEmptyState(false);
				this._finishedCountAfter = -1;
				this.RenderBookNow(this._curBookKey);
				this.RefreshBook(this._curBookKey, false);
			}
		}

		// Token: 0x06004A1A RID: 18970 RVA: 0x0022B00C File Offset: 0x0022920C
		private void RefreshIsAdvance(ItemKey itemKey)
		{
			SkillBookPageDisplayData pageDisplayData = this._pageDisplayData[itemKey.Id];
			short id = this.isCombatSkill ? SkillBook.Instance[itemKey.TemplateId].CombatSkillTemplateId : SkillBook.Instance[itemKey.TemplateId].LifeSkillTemplateId;
			sbyte[] progressReadDefault = this.GetDefaultProgressRead(id);
			for (int i = 0; i < this.PageCount; i++)
			{
				byte realId = this.GetSkillBookRealPage(pageDisplayData, i);
				this._isAdvance[i] = (progressReadDefault[(int)realId] < 100);
			}
		}

		// Token: 0x06004A1B RID: 18971 RVA: 0x0022B09C File Offset: 0x0022929C
		private void ConfirmSelectedBooks()
		{
			sbyte type = this.isCombatSkill ? 8 : 9;
			foreach (ItemKey itemKey in this._selectedBooks)
			{
				ExtraDomainMethod.Call.UpgradeGearMate(this.CurGearMateId, type, itemKey, 1, this._bookDisplayData[itemKey.Id].ItemSourceTypeEnum);
				this._usedBooks.Add(itemKey);
			}
			this.DeselectAllBooks();
			this.OnSelectedBookChanged();
		}

		// Token: 0x06004A1C RID: 18972 RVA: 0x0022B13C File Offset: 0x0022933C
		private void UpdateConfirmButton()
		{
			this._confirmBtn.interactable = (this._selectedBooks.Count != 0 && this._exp >= this._expRequirement);
		}

		// Token: 0x06004A1D RID: 18973 RVA: 0x0022B16C File Offset: 0x0022936C
		private void ResetBookScroll()
		{
			this._selectBookScroll.OnItemRender = new Action<int, Refers>(this.OnRenderScrollBook);
			this._selectBookScroll.SetDataCount(0);
		}

		// Token: 0x06004A1E RID: 18974 RVA: 0x0022B194 File Offset: 0x00229394
		private void ClearBookDisplayData()
		{
			this._curBookKey = ItemKey.Invalid;
			this._bookDisplayData.Clear();
			this._pageDisplayData.Clear();
			this._availableBookList.Clear();
			this._skillBooks.Clear();
			this._pageProgressDic.Clear();
			this._pageProgressDicDefault.Clear();
			this._bookTotalProgressDic.Clear();
			this._disabledBooks.Clear();
			this._usedBooks.Clear();
			this._selectedBooks.Clear();
			this._expRequirement = 0;
			this.OnSelectedBookChanged();
		}

		// Token: 0x06004A1F RID: 18975 RVA: 0x0022B233 File Offset: 0x00229433
		private void GetData()
		{
			ExtraDomainMethod.Call.GetAllSkillBooksGearMateCanRead(base.ListenerId, this.isCombatSkill);
		}

		// Token: 0x06004A20 RID: 18976 RVA: 0x0022B248 File Offset: 0x00229448
		private void OnUIGearMateHide()
		{
			GameDataBridge.AddDataUnMonitor(base.ListenerId, 4, 0, (ulong)this.CurGearMateId, this._fields);
		}

		// Token: 0x06004A21 RID: 18977 RVA: 0x0022B268 File Offset: 0x00229468
		public GearMateSubPageSkill()
		{
			ItemSourceType[] array = new ItemSourceType[3];
			RuntimeHelpers.InitializeArray(array, fieldof(<PrivateImplementationDetails>.4636993D3E1DA4E9D6B8F87B79E8F7C6D018580D52661950EABC3845C5897A4D).FieldHandle);
			this._itemSourceTypeArray = array;
			this._itemSourceType = ItemSourceType.Inventory;
			this._skillBooks = new Dictionary<ItemSourceType, List<ItemKey>>();
			this._usedBooks = new HashSet<ItemKey>();
			this._selectedBooks = new List<ItemKey>();
			this._availableBookList = new List<ItemKey>();
			this._bookDisplayData = new Dictionary<int, ItemDisplayData>();
			this._pageDisplayData = new Dictionary<int, SkillBookPageDisplayData>();
			this._pageProgressDic = new Dictionary<short, sbyte[]>();
			this._pageProgressDicDefault = new Dictionary<short, sbyte[]>();
			this._bookTotalProgressDic = new Dictionary<short, sbyte>();
			this._finishedCountAfter = -1;
			this._disabledBooks = new List<int>();
			this._readIds = new List<int>();
			this._sectInfos = new List<CombatSkillHelper.AttainmentSectInfo>(9);
			this._curBookKey = ItemKey.Invalid;
			this._stringBuilder = new StringBuilder();
			this._isQualificationLevelUp = false;
			this._previewChapterItemKey = ItemKey.Invalid;
			this._fields = new uint[]
			{
				32U,
				30U,
				97U,
				99U,
				29U,
				61U
			};
			this._learnedLifeSkills = new List<GameData.Domains.Character.LifeSkillItem>();
			this._isAdvance = new bool[6];
			base..ctor();
		}

		// Token: 0x06004A22 RID: 18978 RVA: 0x0022B380 File Offset: 0x00229580
		[CompilerGenerated]
		private void <RenderSkillType>g__RefreshTip|72_0(int alreadyAdd = 0, ref GearMateSubPageSkill.<>c__DisplayClass72_0 A_2)
		{
			string skillIconName = this.isCombatSkill ? string.Format("mousetip_gongfa_{0}", this._curSkillType) : string.Format("mousetip_jiyi_{0}", this._curSkillType);
			string gearMateName = NameCenter.GetMonasticTitleOrDisplayName(base.Parent.GearMateDisplayData, false);
			A_2.disPlayer.Type = TipType.GeneralLines;
			TooltipInvoker disPlayer = A_2.disPlayer;
			if (disPlayer.RuntimeParam == null)
			{
				disPlayer.RuntimeParam = new ArgumentBox();
			}
			A_2.disPlayer.RuntimeParam.Set("Title", LocalStringManager.Get(LanguageKey.UI_MouseTipGearMateSkillQualification_0));
			int lineCount = 0;
			string skillNameAndIcon = "<SpName=" + skillIconName + ">" + A_2.skillTypeName;
			string lineOne = LocalStringManager.GetFormat(LanguageKey.UI_MouseTipGearMateSkillQualification_1, skillNameAndIcon, gearMateName, skillNameAndIcon);
			string lineTwo = LocalStringManager.GetFormat(LanguageKey.UI_MouseTipGearMateSkillQualification_2, alreadyAdd);
			A_2.disPlayer.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData(7, new List<string>
			{
				lineOne
			}, null));
			A_2.disPlayer.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData(5, new List<string>
			{
				lineTwo
			}, null));
			A_2.disPlayer.RuntimeParam.Set("LineCount", lineCount);
			A_2.disPlayer.Refresh(false, -1);
			A_2.disPlayer.enabled = true;
		}

		// Token: 0x06004A24 RID: 18980 RVA: 0x0022B510 File Offset: 0x00229710
		[CompilerGenerated]
		private void <OnConfirm>g__ProceedUse|73_0()
		{
			this._confirmBtn.interactable = false;
			bool flag = this._curAnim != null;
			if (flag)
			{
				this._curAnim.DOPause();
				this._curAnim = null;
			}
			this._finishedCountAfter = -1;
			this._anim.AnimationState.SetAnimation(0, "move", false);
			this._particle.Play();
			this._previewChapterItemKey = this._curBookKey;
			base.Parent.SetDisableClickActive(true);
			this.ShowBubble(this._particle.main.duration + 0.5f);
			SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(this._particle.main.duration + 0.5f, delegate
			{
				base.Parent.SetDisableClickActive(false);
				this._have.text = this._exp.ToString();
				base.Parent.RequestForRefreshGearMate();
				bool flag2 = this._curParticle != null;
				if (flag2)
				{
					this._curParticle.Play();
					AudioManager.Instance.PlaySound("SFX_GearMate_consuming_aptitude", false, false);
					this._curParticle = null;
				}
			});
			AudioManager.Instance.PlaySound("SFX_GearMate_consuming_click", false, false);
			this.ConfirmSelectedBooks();
		}

		// Token: 0x06004A26 RID: 18982 RVA: 0x0022B678 File Offset: 0x00229878
		[CompilerGenerated]
		private void <RenderPagesPreview>g__RefreshPagesTip|79_0(ref GearMateSubPageSkill.<>c__DisplayClass79_0 A_1)
		{
			bool flag = !this.isCombatSkill;
			if (!flag)
			{
				GearMateSubPageSkill.<>c__DisplayClass79_1 CS$<>8__locals1;
				CS$<>8__locals1.disPlayer = base.CGet<TooltipInvoker>("PageMouseTipDisplayer");
				CS$<>8__locals1.disPlayer.enabled = true;
				TooltipInvoker disPlayer = CS$<>8__locals1.disPlayer;
				if (disPlayer.RuntimeParam == null)
				{
					disPlayer.RuntimeParam = new ArgumentBox();
				}
				CS$<>8__locals1.disPlayer.Type = TipType.GeneralLines;
				string title = LocalStringManager.Get(LanguageKey.UI_MouseTipGearMateCombatSkillBook_0);
				CS$<>8__locals1.disPlayer.RuntimeParam.Set("Title", title);
				CS$<>8__locals1.defaultReadIds = GearMateSubPageSkill.GetReadIdsFromProgressArray(this.GetDefaultProgressRead(A_1.skillBookId));
				CS$<>8__locals1.lineCount = 0;
				string line0 = LocalStringManager.GetFormat(LanguageKey.UI_MouseTipGearMateCombatSkillBook_1, A_1.configData.Name.SetColor(Colors.Instance.GradeColors[(int)A_1.configData.Grade]));
				ArgumentBox runtimeParam = CS$<>8__locals1.disPlayer.RuntimeParam;
				string format = "LineData{0}";
				int num = CS$<>8__locals1.lineCount + 1;
				CS$<>8__locals1.lineCount = num;
				runtimeParam.SetObject(string.Format(format, num), new GeneralLineData(5, new List<string>
				{
					line0
				}, null));
				string line = LocalStringManager.Get(LanguageKey.UI_MouseTipGearMateCombatSkillBook_2);
				ArgumentBox runtimeParam2 = CS$<>8__locals1.disPlayer.RuntimeParam;
				string format2 = "LineData{0}";
				num = CS$<>8__locals1.lineCount + 1;
				CS$<>8__locals1.lineCount = num;
				runtimeParam2.SetObject(string.Format(format2, num), new GeneralLineData(1, new List<string>
				{
					line
				}, null));
				this._stringBuilder.Clear();
				string line2 = LocalStringManager.Get(LanguageKey.UI_MouseTipGearMateCombatSkillBook_4);
				this.<RenderPagesPreview>g__Build|79_2(line2, "LK_CombatSkill_First_Page_Type_{0}", true, ref A_1, ref CS$<>8__locals1);
				string line3 = LocalStringManager.Get(LanguageKey.UI_MouseTipGearMateCombatSkillBook_5);
				this.<RenderPagesPreview>g__Build|79_2(line3, "LK_CombatSkill_Direct_Page_{0}", true, ref A_1, ref CS$<>8__locals1);
				string line4 = LocalStringManager.Get(LanguageKey.UI_MouseTipGearMateCombatSkillBook_6);
				this.<RenderPagesPreview>g__Build|79_2(line4, "LK_CombatSkill_Reverse_Page_{0}", true, ref A_1, ref CS$<>8__locals1);
				string line5 = LocalStringManager.Get(LanguageKey.UI_MouseTipGearMateCombatSkillBook_3);
				ArgumentBox runtimeParam3 = CS$<>8__locals1.disPlayer.RuntimeParam;
				string format3 = "LineData{0}";
				num = CS$<>8__locals1.lineCount + 1;
				CS$<>8__locals1.lineCount = num;
				runtimeParam3.SetObject(string.Format(format3, num), new GeneralLineData(1, new List<string>
				{
					line5
				}, null));
				this.<RenderPagesPreview>g__Build|79_2(line2, "LK_CombatSkill_First_Page_Type_{0}", false, ref A_1, ref CS$<>8__locals1);
				this.<RenderPagesPreview>g__Build|79_2(line3, "LK_CombatSkill_Direct_Page_{0}", false, ref A_1, ref CS$<>8__locals1);
				this.<RenderPagesPreview>g__Build|79_2(line4, "LK_CombatSkill_Reverse_Page_{0}", false, ref A_1, ref CS$<>8__locals1);
				CS$<>8__locals1.disPlayer.RuntimeParam.Set("LineCount", CS$<>8__locals1.lineCount);
				CS$<>8__locals1.disPlayer.Refresh(false, -1);
			}
		}

		// Token: 0x06004A27 RID: 18983 RVA: 0x0022B918 File Offset: 0x00229B18
		[CompilerGenerated]
		private void <RenderPagesPreview>g__Build|79_2(string label, string startKeyPattern, bool thisBook = true, ref GearMateSubPageSkill.<>c__DisplayClass79_0 A_4, ref GearMateSubPageSkill.<>c__DisplayClass79_1 A_5)
		{
			this._stringBuilder.Clear();
			this._stringBuilder.Append(label);
			this._stringBuilder.Append(":");
			string disableColor = "545454ff";
			string enableColor = disableColor;
			int id = -1;
			for (ushort i = 0; i < 5; i += 1)
			{
				if (!(startKeyPattern == "LK_CombatSkill_First_Page_Type_{0}"))
				{
					if (!(startKeyPattern == "LK_CombatSkill_Direct_Page_{0}"))
					{
						if (startKeyPattern == "LK_CombatSkill_Reverse_Page_{0}")
						{
							enableColor = "brightred";
							id = (int)(i + 10);
						}
					}
					else
					{
						enableColor = "lightblue";
						id = (int)(i + 5);
					}
				}
				else
				{
					enableColor = "darkpurple";
					id = (int)i;
				}
				string finalColor = disableColor;
				if (thisBook)
				{
					bool flag = A_4.bookIds.Contains(id);
					if (flag)
					{
						finalColor = enableColor;
					}
				}
				else
				{
					bool flag2 = A_5.defaultReadIds.Contains(id);
					if (flag2)
					{
						finalColor = enableColor;
					}
				}
				string s = LocalStringManager.Get(string.Format(startKeyPattern, i)).SetColor(finalColor);
				this._stringBuilder.Append(s);
			}
			ArgumentBox runtimeParam = A_5.disPlayer.RuntimeParam;
			string format = "LineData{0}";
			int num = A_5.lineCount + 1;
			A_5.lineCount = num;
			runtimeParam.SetObject(string.Format(format, num), new GeneralLineData(5, new List<string>
			{
				this._stringBuilder.ToString()
			}, null)
			{
				ExtraArgs = new List<object>
				{
					20
				}
			});
		}

		// Token: 0x06004A28 RID: 18984 RVA: 0x0022BAA4 File Offset: 0x00229CA4
		[CompilerGenerated]
		private bool <RenderPagesPreview>g__RenderSinglePage|79_1(Refers refers, int i, ref GearMateSubPageSkill.<>c__DisplayClass79_0 A_3)
		{
			byte realId = this.GetSkillBookRealPage(A_3.pageDisplayData, i);
			Refers stateAfter = refers.CGet<Refers>("StateAfter");
			TextMeshProUGUI progressAfterComplete = refers.CGet<TextMeshProUGUI>("ProgressAfterComplete");
			TextMeshProUGUI progressAfterNotComplete = refers.CGet<TextMeshProUGUI>("ProgressAfterNotComplete");
			TextMeshProUGUI progressNow = refers.CGet<TextMeshProUGUI>("ProgressNow");
			GameObject arrow = refers.CGet<GameObject>("Arrow");
			Refers stateNow = refers.CGet<Refers>("StateNow");
			GameObject labelNow = refers.CGet<GameObject>("LabelNow");
			GameObject progressBack = refers.CGet<GameObject>("ProgressBack");
			arrow.SetActive(true);
			stateNow.gameObject.SetActive(true);
			progressNow.gameObject.SetActive(true);
			labelNow.SetActive(true);
			progressBack.SetActive(true);
			sbyte pageState = A_3.pageDisplayData.State[i];
			stateNow.CGet<GameObject>("Complete").SetActive(pageState == 0);
			stateNow.CGet<GameObject>("Incomplete").SetActive(pageState == 1);
			stateNow.CGet<GameObject>("Lost").SetActive(pageState == 2);
			int progressAdd = 10;
			bool flag = pageState == 1;
			if (flag)
			{
				progressAdd = 40;
			}
			else
			{
				bool flag2 = pageState == 0;
				if (flag2)
				{
					progressAdd = 100;
				}
			}
			bool doNotAddProgress = A_3.doNotAddProgress;
			if (doNotAddProgress)
			{
				progressAdd = 0;
			}
			progressNow.text = string.Format("+{0}%", progressAdd);
			sbyte progressAfter = A_3.progressRead[(int)realId];
			bool advance = this._isAdvance[i];
			arrow.SetActive(advance);
			bool complete = progressAfter >= 100;
			progressAfterComplete.gameObject.SetActive(complete);
			progressAfterNotComplete.gameObject.SetActive(!complete);
			progressAfterComplete.text = string.Format("{0}%", progressAfter);
			progressAfterNotComplete.text = string.Format("{0}%", progressAfter);
			stateAfter.CGet<GameObject>("Yes").SetActive(A_3.progressRead[(int)realId] >= 100);
			stateAfter.CGet<GameObject>("No").SetActive(A_3.progressRead[(int)realId] < 100);
			bool flag3 = this.isCombatSkill;
			if (flag3)
			{
				bool flag4 = i == 0;
				if (flag4)
				{
					sbyte type = A_3.pageDisplayData.Type[0];
					this.GetChapter((int)type).CGet<GameObject>("Back").SetActive(true);
					bool flag5 = progressAfter >= 100;
					if (flag5)
					{
					}
					A_3.bookIds.Add((int)type);
				}
				else
				{
					int idx = (A_3.pageDisplayData.Type[i] == 0) ? (4 + i) : (9 + i);
					A_3.bookIds.Add(idx);
					this.GetChapter(idx).CGet<GameObject>("Back").SetActive(true);
					bool flag6 = progressAfter >= 100;
					if (flag6)
					{
					}
				}
			}
			bool flag7 = this.isCombatSkill;
			bool result;
			if (flag7)
			{
				bool flag8 = realId < 5;
				if (flag8)
				{
					result = (A_3.progressRead[0] >= 100 || A_3.progressRead[1] >= 100 || A_3.progressRead[2] >= 100 || A_3.progressRead[3] >= 100 || A_3.progressRead[4] >= 100 || progressAfter >= 100);
				}
				else
				{
					bool flag9 = realId < 10;
					if (flag9)
					{
						result = (progressAfter >= 100 || A_3.progressRead[(int)(realId + 5)] >= 100);
					}
					else
					{
						result = (progressAfter >= 100 || A_3.progressRead[(int)(realId - 5)] >= 100);
					}
				}
			}
			else
			{
				result = (progressAfter >= 100);
			}
			return result;
		}

		// Token: 0x06004A29 RID: 18985 RVA: 0x0022BE38 File Offset: 0x0022A038
		[CompilerGenerated]
		private unsafe void <DeselectBook>g__CalcExpCost|106_0(ref int exp, ItemKey itemKey, ref GearMateSubPageSkill.<>c__DisplayClass106_0 A_3)
		{
			exp += this.GetBookRequiredExp(itemKey);
			this.GetProgressAdd(itemKey, ref A_3.progressAddArr);
			for (int i = 0; i < this.PageCount; i++)
			{
				byte realId = this.GetSkillBookRealPage(A_3.pageDisplayData, i);
				A_3.progressRead[(int)realId] = (sbyte)Mathf.Min(A_3.progressAddArr[realId] + (int)A_3.progressReadDefault[(int)realId], 100);
			}
		}

		// Token: 0x06004A2A RID: 18986 RVA: 0x0022BEAC File Offset: 0x0022A0AC
		[CompilerGenerated]
		private void <DeselectBook>g__ResetProgressRead|106_1(SkillBookPageDisplayData pageDisplayData, sbyte[] progressRead, sbyte[] progressReadDefault, ref GearMateSubPageSkill.<>c__DisplayClass106_0 A_4)
		{
			for (int i = 0; i < this.PageCount; i++)
			{
				byte realId = this.GetSkillBookRealPage(pageDisplayData, i);
				progressRead[(int)realId] = progressReadDefault[(int)realId];
			}
		}

		// Token: 0x0400332B RID: 13099
		public bool isCombatSkill;

		// Token: 0x0400332C RID: 13100
		private ParticleSystem _particle;

		// Token: 0x0400332D RID: 13101
		private SkeletonGraphic _anim;

		// Token: 0x0400332E RID: 13102
		private CButtonObsolete _confirmBtn;

		// Token: 0x0400332F RID: 13103
		private Refers _skillType;

		// Token: 0x04003330 RID: 13104
		private Refers _curBookAfter;

		// Token: 0x04003331 RID: 13105
		private Refers _curBookNow;

		// Token: 0x04003332 RID: 13106
		private Refers _pagesRefers;

		// Token: 0x04003333 RID: 13107
		private TextMeshProUGUI _have;

		// Token: 0x04003334 RID: 13108
		private TextMeshProUGUI _need;

		// Token: 0x04003335 RID: 13109
		private InfinityScrollLegacy _selectBookScroll;

		// Token: 0x04003336 RID: 13110
		private CToggleGroupObsolete _lifeSkillTypeTogGroup;

		// Token: 0x04003337 RID: 13111
		private CToggleGroupObsolete _combatSkillTypeTogGroup;

		// Token: 0x04003338 RID: 13112
		private RectTransform _chapters;

		// Token: 0x04003339 RID: 13113
		private GameObject _readStateAfterEmpty;

		// Token: 0x0400333A RID: 13114
		private GameObject _readStateNowEmpty;

		// Token: 0x0400333B RID: 13115
		private GameObject _curBookNowEmpty;

		// Token: 0x0400333C RID: 13116
		private CToggleGroupObsolete _itemSourceToggleGroup;

		// Token: 0x0400333D RID: 13117
		private CButtonObsolete _btnSelectAll;

		// Token: 0x0400333E RID: 13118
		private CButtonObsolete _btnMultiplyOption;

		// Token: 0x0400333F RID: 13119
		private readonly ItemSourceType[] _itemSourceTypeArray;

		// Token: 0x04003340 RID: 13120
		private ItemSourceType _itemSourceType;

		// Token: 0x04003341 RID: 13121
		private readonly Dictionary<ItemSourceType, List<ItemKey>> _skillBooks;

		// Token: 0x04003342 RID: 13122
		private readonly HashSet<ItemKey> _usedBooks;

		// Token: 0x04003343 RID: 13123
		private readonly List<ItemKey> _selectedBooks;

		// Token: 0x04003344 RID: 13124
		private CToggleObsolete _currentSelectBookToggle;

		// Token: 0x04003345 RID: 13125
		private readonly List<ItemKey> _availableBookList;

		// Token: 0x04003346 RID: 13126
		private readonly Dictionary<int, ItemDisplayData> _bookDisplayData;

		// Token: 0x04003347 RID: 13127
		private readonly Dictionary<int, SkillBookPageDisplayData> _pageDisplayData;

		// Token: 0x04003348 RID: 13128
		private readonly Dictionary<short, sbyte[]> _pageProgressDic;

		// Token: 0x04003349 RID: 13129
		private readonly Dictionary<short, sbyte[]> _pageProgressDicDefault;

		// Token: 0x0400334A RID: 13130
		private readonly Dictionary<short, sbyte> _bookTotalProgressDic;

		// Token: 0x0400334B RID: 13131
		private ParticleSystem _curParticle;

		// Token: 0x0400334C RID: 13132
		private DOTweenAnimation _curAnim;

		// Token: 0x0400334D RID: 13133
		private sbyte _curSkillType;

		// Token: 0x0400334E RID: 13134
		private int _finishedCountAfter;

		// Token: 0x0400334F RID: 13135
		private readonly List<int> _disabledBooks;

		// Token: 0x04003350 RID: 13136
		private readonly List<int> _readIds;

		// Token: 0x04003351 RID: 13137
		private short[] _attainmentPanels;

		// Token: 0x04003352 RID: 13138
		private readonly List<CombatSkillHelper.AttainmentSectInfo> _sectInfos;

		// Token: 0x04003353 RID: 13139
		private int _exp;

		// Token: 0x04003354 RID: 13140
		private int _expRequirement;

		// Token: 0x04003355 RID: 13141
		private ItemKey _curBookKey;

		// Token: 0x04003356 RID: 13142
		private readonly StringBuilder _stringBuilder;

		// Token: 0x04003357 RID: 13143
		private bool _isQualificationLevelUp;

		// Token: 0x04003358 RID: 13144
		private ItemKey _previewChapterItemKey;

		// Token: 0x04003359 RID: 13145
		private readonly uint[] _fields;

		// Token: 0x0400335A RID: 13146
		private LifeSkillShorts _lifeSkillQualifications;

		// Token: 0x0400335B RID: 13147
		private CombatSkillShorts _combatSkillQualifications;

		// Token: 0x0400335C RID: 13148
		private LifeSkillShorts _lifeSkillAttainments;

		// Token: 0x0400335D RID: 13149
		private CombatSkillShorts _combatSkillAttainments;

		// Token: 0x0400335E RID: 13150
		private List<GameData.Domains.Character.LifeSkillItem> _learnedLifeSkills;

		// Token: 0x0400335F RID: 13151
		private readonly bool[] _isAdvance;
	}
}
