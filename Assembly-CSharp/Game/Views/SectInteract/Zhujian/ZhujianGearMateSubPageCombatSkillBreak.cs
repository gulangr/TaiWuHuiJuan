using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Config;
using FrameWork;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using Game.Components.SortAndFilter;
using Game.Components.SortAndFilter.CombatSkill;
using Game.Views.CharacterMenu;
using GameData.Domains.CombatSkill;
using GameData.Domains.Extra;
using GameData.Domains.Item;
using GameData.Domains.Story;
using GameData.Domains.World.Display;
using GameData.Serializer;
using GameData.Utilities;
using Spine;
using Spine.Unity;
using TMPro;
using UnityEngine;

namespace Game.Views.SectInteract.Zhujian
{
	// Token: 0x020009CF RID: 2511
	public class ZhujianGearMateSubPageCombatSkillBreak : ZhujianGearMateSubPage
	{
		// Token: 0x17000D7E RID: 3454
		// (get) Token: 0x060079FC RID: 31228 RVA: 0x0038A892 File Offset: 0x00388A92
		private int TaiwuCharId
		{
			get
			{
				return SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			}
		}

		// Token: 0x17000D7F RID: 3455
		// (get) Token: 0x060079FD RID: 31229 RVA: 0x0038A89E File Offset: 0x00388A9E
		private List<CombatSkillDisplayData> SkillList
		{
			get
			{
				return this._breakoutDisplayData.CombatSkillDisplayDataList;
			}
		}

		// Token: 0x060079FE RID: 31230 RVA: 0x0038A8AC File Offset: 0x00388AAC
		private void Awake()
		{
			this._sortAndFilterController = new CombatSkillSortAndFilterController(this.sortAndFilter, false, EFilterType.Common);
			this._sortAndFilterController.Init(new Action(this.RefreshScrollData), "ZhujianGearMateSubPageCombatSkillBreak");
			this.infinityScroll.OnItemRender += this.OnItemRender;
			this.confirmBtn.ClearAndAddListener(new Action(this.OnConfirm));
		}

		// Token: 0x060079FF RID: 31231 RVA: 0x0038A91C File Offset: 0x00388B1C
		private void OnConfirm()
		{
			this._isUpgrading = true;
			this.confirmBtn.interactable = false;
			this.ParentView.SetChangeButtonInteractable(false);
			AudioManager.Instance.PlaySound("SFX_GearMate_transmission_combatskill", false, false);
			ExtraDomainMethod.Call.UpgradeGearMate(this.GearMateId, 10, ItemKey.Invalid, (int)this._selectedSkillId);
			TrackEntry trackEntry = this.skeleton.AnimationState.SetAnimation(0, "move", false);
			trackEntry.Complete += this.OnAnimationComplete;
			int randomValue = Random.Range(0, 3);
			this.ParentView.ShowBubble(LocalStringManager.Get(LanguageKey.LK_GearMateAttribute_SpeakWord0 + randomValue), 2f);
			this.ParentView.DoGearMateAnimation("break_1");
		}

		// Token: 0x06007A00 RID: 31232 RVA: 0x0038A9D7 File Offset: 0x00388BD7
		private void OnAnimationComplete(TrackEntry trackEntry)
		{
			this._isUpgrading = false;
			this.ParentView.SetChangeButtonInteractable(true);
			this.RequestData();
		}

		// Token: 0x06007A01 RID: 31233 RVA: 0x0038A9F8 File Offset: 0x00388BF8
		private void OnItemRender(int index, GameObject obj)
		{
			CombatSkillDisplayData data = this._filteredSkillList[index];
			ZhujianGearMateCombatSkillItem item = obj.GetComponent<ZhujianGearMateCombatSkillItem>();
			string reason = string.Empty;
			BoolArray16 banReason = this.BannedSkillDict[data.TemplateId];
			GearMateBreakoutBanReason reasonEnum = GearMateBreakoutBanReason.NotLearnedBook;
			string gearMateName = this.GetGearMateName();
			StringBuilder sb = EasyPool.Get<StringBuilder>();
			sb.Clear();
			bool flag = banReason.Get(0);
			if (flag)
			{
				sb.AppendLine(LocalStringManager.GetFormat(LanguageKey.LK_GearMateBreak_Scroll_Tips_NotLearned, gearMateName).SetColor("brightred").ColorReplace());
				reason = sb.ToString();
			}
			else
			{
				bool flag2 = banReason.Get(1);
				if (flag2)
				{
					sb.AppendLine(LocalStringManager.GetFormat(LanguageKey.LK_GearMateBreak_Scroll_Tips_Broken, gearMateName).SetColor("brightred").ColorReplace());
					reason = sb.ToString();
					reasonEnum = GearMateBreakoutBanReason.SameBreakResult;
				}
			}
			SByteList progress = default(SByteList);
			Dictionary<short, SByteList> gearMateCombatSkillReadingProgress = this._breakoutDisplayData.GearMateCombatSkillReadingProgress;
			if (gearMateCombatSkillReadingProgress != null)
			{
				gearMateCombatSkillReadingProgress.TryGetValue(data.TemplateId, out progress);
			}
			item.Set(data, delegate
			{
				bool flag3 = this._selectedSkillId != data.TemplateId;
				if (flag3)
				{
					this._selectedSkillId = data.TemplateId;
				}
				else
				{
					this._selectedSkillId = -1;
				}
				this.infinityScroll.ReRender();
				this.RefreshCombatSkillItems();
			}, reason, reasonEnum, progress);
			EasyPool.Free<StringBuilder>(sb);
			item.SetSelected(this._selectedSkillId == data.TemplateId);
		}

		// Token: 0x06007A02 RID: 31234 RVA: 0x0038AB4C File Offset: 0x00388D4C
		private new string GetGearMateName()
		{
			string gearMateName = null;
			bool flag = this.ParentView != null && this.ParentView.DisplayData != null;
			if (flag)
			{
				gearMateName = NameCenter.GetMonasticTitleOrDisplayName(this.ParentView.DisplayData, false);
			}
			bool flag2 = string.IsNullOrEmpty(gearMateName);
			if (flag2)
			{
				gearMateName = LocalStringManager.Get(LanguageKey.LK_GearMate_Tab_0);
			}
			return gearMateName;
		}

		// Token: 0x06007A03 RID: 31235 RVA: 0x0038ABB0 File Offset: 0x00388DB0
		private void RefreshScrollData()
		{
			this._filteredSkillList.Clear();
			bool flag = this.SkillList == null;
			if (flag)
			{
				this.infinityScroll.SetDataCount(0);
			}
			else
			{
				CombatSkillSortAndFilterController sortAndFilterController = this._sortAndFilterController;
				Func<IFilterableCombatSkill, bool> filter = (sortAndFilterController != null) ? sortAndFilterController.GenerateFilter() : null;
				CombatSkillSortAndFilterController sortAndFilterController2 = this._sortAndFilterController;
				Comparison<IFilterableCombatSkill> comparer = (sortAndFilterController2 != null) ? sortAndFilterController2.GenerateComparer(this._filteredSkillList) : null;
				foreach (CombatSkillDisplayData skill in this.SkillList)
				{
					bool flag2 = filter == null || filter(skill);
					if (flag2)
					{
						this._filteredSkillList.Add(skill);
					}
				}
				bool flag3 = comparer != null;
				if (flag3)
				{
					this._filteredSkillList.Sort(comparer);
				}
				List<CombatSkillDisplayData> selectable = (from s in this._filteredSkillList
				where this.BannedSkillDict[s.TemplateId] == 0
				select s).ToList<CombatSkillDisplayData>();
				List<CombatSkillDisplayData> nonSelectable = (from s in this._filteredSkillList
				where this.BannedSkillDict[s.TemplateId] > 0
				select s).ToList<CombatSkillDisplayData>();
				this._filteredSkillList.Clear();
				this._filteredSkillList.AddRange(selectable);
				this._filteredSkillList.AddRange(nonSelectable);
				this.infinityScroll.SetDataCount(this._filteredSkillList.Count);
				this._sortAndFilterController.AfterFilter(this._filteredSkillList);
			}
		}

		// Token: 0x06007A04 RID: 31236 RVA: 0x0038AD20 File Offset: 0x00388F20
		protected override void OnShowDataRequest()
		{
			this.RequestData();
		}

		// Token: 0x06007A05 RID: 31237 RVA: 0x0038AD2A File Offset: 0x00388F2A
		private void RequestData()
		{
			StoryDomainMethod.AsyncCall.GetGearMateBreakoutDisplayData(this.ParentView, this.GearMateId, delegate(int offset, RawDataPool dataPoll)
			{
				Serializer.Deserialize(dataPoll, offset, ref this._breakoutDisplayData);
				this.UpdateData();
				this.Refresh();
				base.SetContentReady();
			});
		}

		// Token: 0x06007A06 RID: 31238 RVA: 0x0038AD4B File Offset: 0x00388F4B
		private void UpdateData()
		{
			this._selectedSkillId = -1;
			this.BannedSkillDict.Clear();
			this._breakoutDisplayData.GearMateBreakoutCombatSkillBanReasonList.ForEach(delegate(ShortPair e)
			{
				this.BannedSkillDict[e.First] = (ushort)e.Second;
			});
		}

		// Token: 0x06007A07 RID: 31239 RVA: 0x0038AD7E File Offset: 0x00388F7E
		private void Refresh()
		{
			this.RefreshScrollData();
			this.skeleton.AnimationState.SetAnimation(0, "idle", true);
			this.RefreshCombatSkillItems();
		}

		// Token: 0x06007A08 RID: 31240 RVA: 0x0038ADA8 File Offset: 0x00388FA8
		private void RefreshCombatSkillItems()
		{
			bool flag = this._selectedSkillId == -1;
			if (flag)
			{
				Array.ForEach<CharacterMenuCombatSkillItem>(this.combatSkillItems, delegate(CharacterMenuCombatSkillItem e)
				{
					e.gameObject.SetActive(false);
				});
				this.costLabel.text = "0".SetColor("brightblue") + "/" + CommonUtils.GetDisplayStringForNum(this._breakoutDisplayData.Exp, 100000);
				Array.ForEach<ZhujianGearMateSkillInfoItem>(this.skillInfoItems, delegate(ZhujianGearMateSkillInfoItem e)
				{
					e.Set();
				});
				this.confirmBtn.interactable = false;
			}
			else
			{
				CombatSkillDomainMethod.AsyncCall.GetCombatSkillDisplayDataOnce(this.ParentView, this.GearMateId, this._selectedSkillId, delegate(int offset, RawDataPool pool)
				{
					CombatSkillDisplayData combatSkillDisplayData = null;
					Serializer.Deserialize(pool, offset, ref combatSkillDisplayData);
					this.combatSkillItems[0].Set(combatSkillDisplayData);
					this.skillInfoItems[0].Set(this.ParentView, this.GearMateId, this._selectedSkillId, this._breakoutDisplayData.GearMateLifeSkillAttainments, combatSkillDisplayData.ActivationState);
					Array.ForEach<CharacterMenuCombatSkillItem>(this.combatSkillItems, delegate(CharacterMenuCombatSkillItem e)
					{
						e.gameObject.SetActive(true);
					});
				});
				CombatSkillDisplayData data = this._breakoutDisplayData.CombatSkillDisplayDataList.FirstOrDefault((CombatSkillDisplayData e) => e.TemplateId == this._selectedSkillId);
				this.combatSkillItems[1].Set(data);
				this.skillInfoItems[1].Set(this.ParentView, this.TaiwuCharId, this._selectedSkillId, this._breakoutDisplayData.LifeSkillAttainments, data.ActivationState);
				int cost = (int)(SkillBreakPlate.Instance[CombatSkill.Instance[this._selectedSkillId].Grade].CostExp * 25);
				this.costLabel.text = CommonUtils.GetDisplayStringForNum(cost, 100000).SetColor("brightblue") + "/" + CommonUtils.GetDisplayStringForNum(this._breakoutDisplayData.Exp, 100000);
				this.confirmBtn.interactable = (cost <= this._breakoutDisplayData.Exp);
			}
			this.confirmBtn.GetComponent<TooltipInvoker>().PresetParam[1] = LocalStringManager.GetFormat(LanguageKey.LK_GearMateBreak_Button_Tips_Normal, this.GetGearMateName());
		}

		// Token: 0x06007A09 RID: 31241 RVA: 0x0038AF94 File Offset: 0x00389194
		public override void SetGearMateId(int gearMateId)
		{
			bool flag = this.GearMateId == gearMateId;
			if (!flag)
			{
				base.SetGearMateId(gearMateId);
				bool isVisible = this.IsVisible;
				if (isVisible)
				{
					this.RequestData();
				}
			}
		}

		// Token: 0x06007A0A RID: 31242 RVA: 0x0038AFCC File Offset: 0x003891CC
		public override bool CanQuickHide()
		{
			return !this._isUpgrading;
		}

		// Token: 0x04005C70 RID: 23664
		[SerializeField]
		private SortAndFilter sortAndFilter;

		// Token: 0x04005C71 RID: 23665
		[SerializeField]
		private InfinityScroll infinityScroll;

		// Token: 0x04005C72 RID: 23666
		[SerializeField]
		private CButton confirmBtn;

		// Token: 0x04005C73 RID: 23667
		[SerializeField]
		private TextMeshProUGUI costLabel;

		// Token: 0x04005C74 RID: 23668
		[SerializeField]
		private CharacterMenuCombatSkillItem[] combatSkillItems;

		// Token: 0x04005C75 RID: 23669
		[SerializeField]
		private ZhujianGearMateSkillInfoItem[] skillInfoItems;

		// Token: 0x04005C76 RID: 23670
		[SerializeField]
		private SkeletonGraphic skeleton;

		// Token: 0x04005C77 RID: 23671
		private SectZhujianGearMateBreakoutDisplayData _breakoutDisplayData;

		// Token: 0x04005C78 RID: 23672
		private CombatSkillSortAndFilterController _sortAndFilterController;

		// Token: 0x04005C79 RID: 23673
		private readonly List<CombatSkillDisplayData> _filteredSkillList = new List<CombatSkillDisplayData>();

		// Token: 0x04005C7A RID: 23674
		private readonly Dictionary<short, BoolArray16> BannedSkillDict = new Dictionary<short, BoolArray16>();

		// Token: 0x04005C7B RID: 23675
		private short _selectedSkillId;

		// Token: 0x04005C7C RID: 23676
		private bool _isUpgrading;
	}
}
