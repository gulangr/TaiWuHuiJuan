using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Coffee.UIExtensions;
using Config;
using FrameWork;
using GameData.Domains.Character;
using GameData.Domains.CombatSkill;
using GameData.Domains.Extra;
using GameData.Domains.Item;
using GameData.Domains.Taiwu;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using Spine;
using Spine.Unity;
using TMPro;
using UnityEngine;

namespace GearMate
{
	// Token: 0x02000613 RID: 1555
	public class GearMateCombatBreakPage : GearMateSubPageBase
	{
		// Token: 0x1700092C RID: 2348
		// (get) Token: 0x060048E7 RID: 18663 RVA: 0x00221871 File Offset: 0x0021FA71
		private CombatSkillModel CombatSkillModel
		{
			get
			{
				return SingletonObject.getInstance<CombatSkillModel>();
			}
		}

		// Token: 0x1700092D RID: 2349
		// (get) Token: 0x060048E8 RID: 18664 RVA: 0x00221878 File Offset: 0x0021FA78
		private int CurGearMateId
		{
			get
			{
				return base.Parent.GearMateId;
			}
		}

		// Token: 0x060048E9 RID: 18665 RVA: 0x00221888 File Offset: 0x0021FA88
		protected override void InitInternal()
		{
			base.InitInternal();
			this.InitRefers();
			this._selectedCombatSkillTemplateId = -1;
			this._combatSkillScrollView.Init();
			this._combatSkillScrollView.SetCombatSkillList(this.CombatSkillModel.GetCache(base.TaiwuId), true, true, "gearmate_combatskill", new Action<CombatSkillDisplayData, CombatSkillView>(this.OnRenderCombatSkill), false, false, null, true);
			this.InitCombatSkillScrollViewMaterial();
			this._taiwuCombatSkillItem = new CombatSkillItemHelper(this._taiwuSkillItem);
			this._gearMateCombatSkillItem = new CombatSkillItemHelper(this._gearMateSkillItem);
			this._taiwuCombatSkillItem.RefreshCombatSkillSlot(null);
			this._taiwuCombatSkillItem.RefreshPageLayout(null);
			this._taiwuCombatSkillItem.RefreshBonusLayout(null, -1, default(LifeSkillShorts), null);
			this._gearMateCombatSkillItem.RefreshCombatSkillSlot(null);
			this._gearMateCombatSkillItem.RefreshPageLayout(null);
			this._gearMateCombatSkillItem.RefreshBonusLayout(null, -1, default(LifeSkillShorts), null);
			this._confirmButton.ClearAndAddListener(new Action(this.OnConfirm));
			this.InitSpine1();
			UIElement element = base.Parent.Element;
			element.OnHide = (Action)Delegate.Combine(element.OnHide, new Action(this.OnUIGearMateHide));
		}

		// Token: 0x060048EA RID: 18666 RVA: 0x002219C3 File Offset: 0x0021FBC3
		private void OnEnable()
		{
			GEvent.Add(UiEvents.ReceivedCombatSkillDisplayData, new GEvent.Callback(this.ReceivedCombatSkillDisplayData));
			this.RequestCombatSkillDisplayDataByLearnedCombatSkillList();
		}

		// Token: 0x060048EB RID: 18667 RVA: 0x002219E9 File Offset: 0x0021FBE9
		private void OnDisable()
		{
			GEvent.Remove(UiEvents.ReceivedCombatSkillDisplayData, new GEvent.Callback(this.ReceivedCombatSkillDisplayData));
			this._transParticle.gameObject.SetActive(false);
		}

		// Token: 0x060048EC RID: 18668 RVA: 0x00221A1A File Offset: 0x0021FC1A
		private void OnGearMateCanBreakoutCombatSkillUpdate()
		{
			this.RefreshCombatScrollByCached();
		}

		// Token: 0x060048ED RID: 18669 RVA: 0x00221A24 File Offset: 0x0021FC24
		public override void OnGearMateCharacterIdChanged(int lastId)
		{
			this.RequestGearMateCanBreakOutList();
			bool flag = this.CurGearMateId != lastId;
			if (flag)
			{
				bool flag2 = lastId != -1;
				if (flag2)
				{
					base.RemoveMonitorFieldId(4, 0, (ulong)lastId);
				}
				base.AppendMonitorFieldId(new UIBase.MonitorDataField(4, 0, (ulong)this.CurGearMateId, this._fields));
			}
		}

		// Token: 0x060048EE RID: 18670 RVA: 0x00221A80 File Offset: 0x0021FC80
		private void RequestGearMateCanBreakOutList()
		{
			bool flag = this._learnedCombatSkillList.Count == 0;
			if (!flag)
			{
				ExtraDomainMethod.Call.GetGearMateBreakoutCombatSkillBanReasonList(base.ListenerId, base.Parent.GearMateId, this._learnedCombatSkillList);
			}
		}

		// Token: 0x060048EF RID: 18671 RVA: 0x00221ABF File Offset: 0x0021FCBF
		public override void OnGearMateDataChanged()
		{
			this.RefreshCombatScrollByCached();
			this.RefreshCenter(this._selectedCombatSkillTemplateId);
		}

		// Token: 0x060048F0 RID: 18672 RVA: 0x00221AD8 File Offset: 0x0021FCD8
		protected override IList<UIBase.MonitorDataField> GetMonitorFields()
		{
			return new List<UIBase.MonitorDataField>
			{
				new UIBase.MonitorDataField(4, 0, (ulong)base.TaiwuId, new uint[]
				{
					66U,
					59U,
					97U
				})
			};
		}

		// Token: 0x060048F1 RID: 18673 RVA: 0x00221B18 File Offset: 0x0021FD18
		protected override void HandleDataModification(Notification notification, NotificationWrapper wrapper)
		{
			ushort domainId = notification.Uid.DomainId;
			ushort dataId = notification.Uid.DataId;
			RawDataPool pool = wrapper.DataPool;
			int offset = notification.ValueOffset;
			bool flag = domainId == 4 && dataId == 0 && notification.Uid.SubId1 == 59U;
			if (flag)
			{
				bool flag2 = (int)notification.Uid.SubId0 == base.TaiwuId;
				if (flag2)
				{
					Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._learnedCombatSkillList);
					this.RequestCombatSkillDisplayDataByLearnedCombatSkillList();
					this.RequestGearMateCanBreakOutList();
				}
			}
			bool flag3 = domainId == 4 && dataId == 0 && notification.Uid.SubId1 == 66U;
			if (flag3)
			{
				bool flag4 = (int)notification.Uid.SubId0 == base.TaiwuId;
				if (flag4)
				{
					Serializer.Deserialize(pool, offset, ref this._taiwuExp);
					this.RefreshExp();
				}
			}
			bool flag5 = domainId == 4 && dataId == 0 && notification.Uid.SubId1 == 97U;
			if (flag5)
			{
				bool flag6 = (int)notification.Uid.SubId0 == this.CurGearMateId;
				if (flag6)
				{
					Serializer.Deserialize(pool, offset, ref this._gearMateLifeSkillAttainments);
				}
				else
				{
					bool flag7 = (int)notification.Uid.SubId0 == base.TaiwuId;
					if (flag7)
					{
						Serializer.Deserialize(pool, offset, ref this._taiwuLifeSkillAttainments);
					}
				}
				this.RefreshCenter(this._selectedCombatSkillTemplateId);
			}
		}

		// Token: 0x060048F2 RID: 18674 RVA: 0x00221C88 File Offset: 0x0021FE88
		public override void HandleMethodReturn(Notification notification, NotificationWrapper wrapper)
		{
			ushort domainId = notification.DomainId;
			ushort methodId = notification.MethodId;
			RawDataPool pool = wrapper.DataPool;
			int offset = notification.ValueOffset;
			bool flag = domainId == 19;
			if (flag)
			{
				bool flag2 = methodId == 152;
				if (flag2)
				{
					List<ShortPair> gearMateBreakoutCombatBanReasonSkillList = new List<ShortPair>();
					Serializer.Deserialize(pool, offset, ref gearMateBreakoutCombatBanReasonSkillList);
					this._gearMateBreakoutCombatBanReasonDict.Clear();
					foreach (ShortPair pair in gearMateBreakoutCombatBanReasonSkillList)
					{
						this._gearMateBreakoutCombatBanReasonDict[pair.First] = (ushort)pair.Second;
					}
					this.OnGearMateCanBreakoutCombatSkillUpdate();
				}
			}
		}

		// Token: 0x060048F3 RID: 18675 RVA: 0x00221D58 File Offset: 0x0021FF58
		private void RefreshExp()
		{
			bool flag = this._selectedCombatSkillTemplateId == -1;
			if (flag)
			{
				this._expArea.SetActive(false);
			}
			else
			{
				this._expArea.SetActive(true);
				int cost = this.GetBreakoutExpCost(this._selectedCombatSkillTemplateId);
				string color = (this._taiwuExp >= cost) ? "brightblue" : "brightred";
				this._expCostLabel.text = string.Format("{0}/{1}", CommonUtils.GetDisplayStringForNum(this._taiwuExp, 100000).SetColor(color), cost);
			}
		}

		// Token: 0x060048F4 RID: 18676 RVA: 0x00221DEC File Offset: 0x0021FFEC
		private int GetBreakoutExpCost(short skillId)
		{
			return (int)(Config.SkillBreakPlate.Instance[CombatSkill.Instance[skillId].Grade].CostExp * 25);
		}

		// Token: 0x060048F5 RID: 18677 RVA: 0x00221E20 File Offset: 0x00220020
		private void RequestCombatSkillDisplayDataByLearnedCombatSkillList()
		{
			bool flag = this._learnedCombatSkillList.Count > 0;
			if (flag)
			{
				this.CombatSkillModel.RequestCombatSkillDisplayData(base.TaiwuId, this._learnedCombatSkillList, null);
			}
			else
			{
				this.CombatSkillModel.Clear(base.TaiwuId);
				this.RefreshCombatScrollByCached();
			}
		}

		// Token: 0x060048F6 RID: 18678 RVA: 0x00221E78 File Offset: 0x00220078
		private void RefreshCombatScrollByCached()
		{
			this._brokenTaiwuCombatSkillDisplayDataList.Clear();
			this._brokenTaiwuCombatSkillDisplayDataList.AddRange(this.CombatSkillModel.GetCache(base.TaiwuId));
			this._brokenTaiwuCombatSkillDisplayDataList.RemoveAll((CombatSkillDisplayData c) => !c.BreakSuccess);
			this._combatSkillScrollView.SortAndFilter.SetLearnedCount((from d in this._brokenTaiwuCombatSkillDisplayDataList
			select d.TemplateId).ToList<short>());
			this._combatSkillScrollView.SetCombatSkillList(this._brokenTaiwuCombatSkillDisplayDataList, false, true, null, null, false, false, null, true);
		}

		// Token: 0x060048F7 RID: 18679 RVA: 0x00221F34 File Offset: 0x00220134
		private void ReceivedCombatSkillDisplayData(ArgumentBox _)
		{
			this.RefreshCombatScrollByCached();
		}

		// Token: 0x060048F8 RID: 18680 RVA: 0x00221F40 File Offset: 0x00220140
		private void OnRenderCombatSkill(CombatSkillDisplayData skillData, CombatSkillView skillView)
		{
			skillView.SetChecked(skillData.TemplateId == this._selectedCombatSkillTemplateId);
			skillView.SetClickEvent(delegate
			{
				this.OnSelectCombatSkill(skillData);
			});
			BoolArray16 banReason = this._gearMateBreakoutCombatBanReasonDict[skillData.TemplateId];
			bool disabled = banReason > 0;
			skillView.SetInteractable(!disabled);
			skillView.GetComponent<DisableStyleRoot>().SetStyleEffect(disabled, false);
			bool flag = disabled;
			if (flag)
			{
				skillView.CGet<TextMeshProUGUI>("Grade").color = Color.black;
			}
			TooltipInvoker tip = skillView.GetMouseTipDisplay();
			tip.Type = (disabled ? TipType.SingleDesc : TipType.CombatSkill);
			bool flag2 = disabled;
			if (flag2)
			{
				TooltipInvoker tooltipInvoker = tip;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = new ArgumentBox();
				}
				string content = this.MakeBanReasonContent(banReason);
				tip.RuntimeParam.Set("arg0", content);
			}
		}

		// Token: 0x060048F9 RID: 18681 RVA: 0x00222040 File Offset: 0x00220240
		private string MakeBanReasonContent(BoolArray16 banReason)
		{
			StringBuilder sb = EasyPool.Get<StringBuilder>();
			sb.Clear();
			string gearMateName = base.GetGearMateName();
			bool flag = banReason.Get(0);
			if (flag)
			{
				sb.AppendLine(LocalStringManager.GetFormat(LanguageKey.LK_GearMateBreak_Scroll_Tips_NotLearned, gearMateName).SetColor("brightred").ColorReplace());
			}
			bool flag2 = banReason.Get(1);
			if (flag2)
			{
				sb.AppendLine(LocalStringManager.GetFormat(LanguageKey.LK_GearMateBreak_Scroll_Tips_Broken, gearMateName).SetColor("brightred").ColorReplace());
			}
			string str = sb.ToString();
			EasyPool.Free<StringBuilder>(sb);
			return str;
		}

		// Token: 0x060048FA RID: 18682 RVA: 0x002220D8 File Offset: 0x002202D8
		private void InitCombatSkillScrollViewMaterial()
		{
			Transform entry = this._combatSkillScrollView.GetComponent<InfinityScrollLegacy>().SrcPrefab.transform;
			CImage[] images = entry.GetComponentsInChildren<CImage>(true);
			foreach (CImage image in images)
			{
				image.material = null;
			}
		}

		// Token: 0x060048FB RID: 18683 RVA: 0x00222128 File Offset: 0x00220328
		private void OnSelectCombatSkill(CombatSkillDisplayData skillData)
		{
			short skillDataTemplateId = skillData.TemplateId;
			this._selectedCombatSkillTemplateId = skillDataTemplateId;
			this.RefreshExp();
			this.RefreshCenter(skillDataTemplateId);
			this.RefreshCombatScrollByCached();
		}

		// Token: 0x060048FC RID: 18684 RVA: 0x0022215C File Offset: 0x0022035C
		private void RefreshCenter(short skillId)
		{
			CombatSkillDisplayData skillData = this._brokenTaiwuCombatSkillDisplayDataList.Find((CombatSkillDisplayData d) => d.TemplateId == skillId);
			this._taiwuCombatSkillItem.RefreshCombatSkillSlot(skillData);
			this._taiwuCombatSkillItem.RefreshPageLayout(skillData);
			bool flag = skillId != -1;
			if (flag)
			{
				CombatSkillDomainMethod.AsyncCall.GetCombatSkillBreakBonuses(base.Parent, base.TaiwuId, skillId, delegate(int offset, RawDataPool pool)
				{
					List<SkillBreakPlateBonus> bonusList = new List<SkillBreakPlateBonus>();
					Serializer.Deserialize(pool, offset, ref bonusList);
					this._taiwuCombatSkillItem.RefreshBonusLayout(bonusList, skillId, this._taiwuLifeSkillAttainments, this.Parent);
				});
			}
			else
			{
				this._taiwuCombatSkillItem.RefreshBonusLayout(null, -1, default(LifeSkillShorts), null);
			}
			bool expEnough = skillId >= 0 && this._taiwuExp >= this.GetBreakoutExpCost(skillId);
			bool flag2 = this.GearMateCanBreakOut(skillId);
			if (flag2)
			{
				this.RefreshConfirmButton(expEnough, skillId);
				CombatSkillDomainMethod.AsyncCall.GetCombatSkillDisplayDataOnce(base.Parent, base.Parent.GearMateId, skillId, delegate(int offset, RawDataPool pool)
				{
					CombatSkillDisplayData combatSkillDisplayData = null;
					Serializer.Deserialize(pool, offset, ref combatSkillDisplayData);
					this._gearMateCombatSkillItem.RefreshCombatSkillSlot(combatSkillDisplayData);
					this._gearMateCombatSkillItem.RefreshPageLayout(combatSkillDisplayData);
				});
				this._gearMateCombatSkillItem.RefreshBonusLayout(base.Parent.GearMate.SkillBreakBonusDict.GetValueOrDefault(skillId), skillId, this._gearMateLifeSkillAttainments, base.Parent);
			}
			else
			{
				this.RefreshConfirmButton(false, skillId);
				this._gearMateCombatSkillItem.RefreshCombatSkillSlot(null);
				this._gearMateCombatSkillItem.RefreshPageLayout(null);
				this._gearMateCombatSkillItem.RefreshBonusLayout(null, -1, default(LifeSkillShorts), null);
			}
		}

		// Token: 0x060048FD RID: 18685 RVA: 0x002222F0 File Offset: 0x002204F0
		private bool GearMateCanBreakOut(short skillDataTemplateId)
		{
			BoolArray16 reason;
			return this._gearMateBreakoutCombatBanReasonDict.TryGetValue(skillDataTemplateId, out reason) && reason == 0;
		}

		// Token: 0x060048FE RID: 18686 RVA: 0x00222320 File Offset: 0x00220520
		private void RefreshConfirmButton(bool interactable, short skillId)
		{
			this._confirmButton.interactable = interactable;
			this._confirmButton.ClickAudioKey = (interactable ? "SFX_GearMate_transmission_click" : null);
			this._confirmButton.transform.Find("Disable").gameObject.SetActive(!interactable);
			TooltipInvoker tip = this._confirmButton.GetComponent<TooltipInvoker>();
			TooltipInvoker tooltipInvoker = tip;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = new ArgumentBox();
			}
			tip.RuntimeParam.Set("arg0", LocalStringManager.Get(LanguageKey.LK_GearMateBreak_Button));
			StringBuilder contentSb = EasyPool.Get<StringBuilder>();
			string gearMateName = base.GetGearMateName();
			contentSb.Append(LocalStringManager.GetFormat(LanguageKey.LK_GearMateBreak_Button_Tips_Normal, gearMateName));
			bool flag = !interactable;
			if (flag)
			{
				contentSb.Append("\n\n");
				bool flag2 = skillId >= 0 && this._taiwuExp < this.GetBreakoutExpCost(skillId);
				if (flag2)
				{
					contentSb.Append(LocalStringManager.Get(LanguageKey.LK_GearMateBreak_Button_Tips_Exp).SetColor("brightred"));
				}
				else
				{
					contentSb.Append(LocalStringManager.GetFormat(LanguageKey.LK_GearMateBreak_Button_Tips_NotLearned, gearMateName).SetColor("brightred"));
				}
			}
			string content = contentSb.ToString().ColorReplace();
			tip.RuntimeParam.Set("arg1", content);
		}

		// Token: 0x060048FF RID: 18687 RVA: 0x00222468 File Offset: 0x00220668
		public void OnConfirm()
		{
			this.PlaySpine1();
			this.PlayParticle();
			this.ShowBubble();
			AudioManager.Instance.PlaySound("SFX_GearMate_transmission_combatskill", false, false);
			base.Parent.SetDisableClickActive(true);
			SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(1f, new Action(this.<OnConfirm>g__OnAnimationEnd|36_0));
		}

		// Token: 0x06004900 RID: 18688 RVA: 0x002224C8 File Offset: 0x002206C8
		public override void PointEnterConfirmButton()
		{
			bool flag = this.ConfirmButtonInteractable();
			if (flag)
			{
				base.PointEnterConfirmButton();
			}
		}

		// Token: 0x06004901 RID: 18689 RVA: 0x002224E8 File Offset: 0x002206E8
		private void ShowBubble()
		{
			string content = LocalStringManager.Get((!base.Parent.GearMate.SkillBreakBonusDict.ContainsKey(this._selectedCombatSkillTemplateId)) ? LanguageKey.LK_GearMateBreak_SpeekWord0 : LanguageKey.LK_GearMateBreak_SpeekWord1);
			base.Parent.Avatar.ShowBubble(content, 5f);
			base.Parent.Avatar.DoGearMateAnimation("break_2");
		}

		// Token: 0x06004902 RID: 18690 RVA: 0x00222557 File Offset: 0x00220757
		private void PlayParticle()
		{
			this._transParticle.gameObject.SetActive(true);
			this._transParticle.Play();
		}

		// Token: 0x06004903 RID: 18691 RVA: 0x00222578 File Offset: 0x00220778
		private void InitSpine1()
		{
			this._spineList[0].AnimationState.Complete += delegate(TrackEntry entry)
			{
				bool flag = entry.Animation.Name == "move";
				if (flag)
				{
					this._spineList[0].AnimationState.SetAnimation(0, "idle", true);
				}
			};
		}

		// Token: 0x06004904 RID: 18692 RVA: 0x0022259E File Offset: 0x0022079E
		private void PlaySpine1()
		{
			this._spineList[0].AnimationState.SetAnimation(0, "move", false);
		}

		// Token: 0x06004905 RID: 18693 RVA: 0x002225C0 File Offset: 0x002207C0
		public override bool ConfirmButtonInteractable()
		{
			return this._confirmButton.interactable;
		}

		// Token: 0x06004906 RID: 18694 RVA: 0x002225DD File Offset: 0x002207DD
		public override void ConfirmClick()
		{
			this.OnConfirm();
		}

		// Token: 0x06004907 RID: 18695 RVA: 0x002225E7 File Offset: 0x002207E7
		private void OnUIGearMateHide()
		{
			GameDataBridge.AddDataUnMonitor(base.ListenerId, 4, 0, (ulong)this.CurGearMateId, this._fields);
		}

		// Token: 0x06004908 RID: 18696 RVA: 0x00222608 File Offset: 0x00220808
		private void InitRefers()
		{
			this._spineList = base.CGetList<SkeletonGraphic>("Spine_");
			this._confirmButton = base.CGet<CButtonObsolete>("ButtonConfirm");
			this._combatSkillScrollView = base.CGet<CombatSkillScrollView>("CombatSkillScrollView");
			this._expArea = base.CGet<GameObject>("ExpArea");
			this._gearMateSkillItem = base.CGet<Refers>("GearMateSkillItem");
			this._taiwuSkillItem = base.CGet<Refers>("TaiwuSkillItem");
			this._expCostLabel = base.CGet<TextMeshProUGUI>("ExpCostLabel");
			this._transParticle = base.CGet<UIParticle>("TransParticle");
		}

		// Token: 0x0600490A RID: 18698 RVA: 0x002226DC File Offset: 0x002208DC
		[CompilerGenerated]
		private void <OnConfirm>g__OnAnimationEnd|36_0()
		{
			base.Parent.SetDisableClickActive(false);
			ExtraDomainMethod.Call.UpgradeGearMate(base.Parent.GearMateId, 10, ItemKey.Invalid, (int)this._selectedCombatSkillTemplateId);
			this.RefreshCenter(-1);
			CombatSkillDomainMethod.AsyncCall.GetCombatSkillDisplayDataOnce(base.Parent, base.Parent.GearMateId, this._selectedCombatSkillTemplateId, delegate(int offset, RawDataPool pool)
			{
				CombatSkillDisplayData combatSkillDisplayData = null;
				Serializer.Deserialize(pool, offset, ref combatSkillDisplayData);
				base.Parent.RequestForRefreshGearMate();
			});
			this._selectedCombatSkillTemplateId = -1;
		}

		// Token: 0x040032AB RID: 12971
		private List<short> _learnedCombatSkillList = new List<short>();

		// Token: 0x040032AC RID: 12972
		private int _taiwuExp;

		// Token: 0x040032AD RID: 12973
		private readonly List<CombatSkillDisplayData> _brokenTaiwuCombatSkillDisplayDataList = new List<CombatSkillDisplayData>();

		// Token: 0x040032AE RID: 12974
		private short _selectedCombatSkillTemplateId;

		// Token: 0x040032AF RID: 12975
		private readonly Dictionary<short, BoolArray16> _gearMateBreakoutCombatBanReasonDict = new Dictionary<short, BoolArray16>();

		// Token: 0x040032B0 RID: 12976
		private CombatSkillItemHelper _taiwuCombatSkillItem;

		// Token: 0x040032B1 RID: 12977
		private CombatSkillItemHelper _gearMateCombatSkillItem;

		// Token: 0x040032B2 RID: 12978
		private LifeSkillShorts _taiwuLifeSkillAttainments;

		// Token: 0x040032B3 RID: 12979
		private LifeSkillShorts _gearMateLifeSkillAttainments;

		// Token: 0x040032B4 RID: 12980
		private readonly uint[] _fields = new uint[]
		{
			97U
		};

		// Token: 0x040032B5 RID: 12981
		private List<SkeletonGraphic> _spineList;

		// Token: 0x040032B6 RID: 12982
		private CButtonObsolete _confirmButton;

		// Token: 0x040032B7 RID: 12983
		private CombatSkillScrollView _combatSkillScrollView;

		// Token: 0x040032B8 RID: 12984
		private GameObject _expArea;

		// Token: 0x040032B9 RID: 12985
		private Refers _gearMateSkillItem;

		// Token: 0x040032BA RID: 12986
		private Refers _taiwuSkillItem;

		// Token: 0x040032BB RID: 12987
		private TextMeshProUGUI _expCostLabel;

		// Token: 0x040032BC RID: 12988
		private UIParticle _transParticle;
	}
}
