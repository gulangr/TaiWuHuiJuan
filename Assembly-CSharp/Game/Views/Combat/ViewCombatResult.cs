using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Common;
using GameData.Domains.Combat;
using GameData.Domains.Global;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Taiwu;
using GameData.Domains.TaiwuEvent;
using GameData.Serializer;
using GameData.Utilities;
using UnityEngine;

namespace Game.Views.Combat
{
	// Token: 0x02000B39 RID: 2873
	public class ViewCombatResult : UIBase
	{
		// Token: 0x06008ECC RID: 36556 RVA: 0x00427BF8 File Offset: 0x00425DF8
		public override void OnInit(ArgumentBox argsBox)
		{
			argsBox.Get("CombatResult", out this._combatResult);
			argsBox.Get("CombatType", out this._combatType);
			argsBox.Get("MainEnemyId", out this._mainEnemyId);
			argsBox.Get("MainEnemyTemplateId", out this._mainEnemyTemplateId);
			argsBox.Get<CombatResultDisplayData>("DisplayData", out this._displayData);
			argsBox.Get("IsMartialArtist", out this._isMartialArtist);
			bool isWin = CombatResultType.IsPlayerWin(this._combatResult);
			this.winLoseDisplay.Set(isWin);
			this.lootPage.gameObject.SetActive(false);
			this.statusPage.gameObject.SetActive(false);
			this.InitTabGroup();
			TaiwuDomainMethod.AsyncCall.RequestTaiwuResourceDisplayData(this, delegate(int offset, RawDataPool pool)
			{
				TaiwuResourceDisplayData data = new TaiwuResourceDisplayData();
				Serializer.Deserialize(pool, offset, ref data);
				int[] currentResourcesArray = data.Resources;
				this.lootPage.Init(this._displayData, this._selectedItems, new Action<bool>(this.OnSelectAllChanged), currentResourcesArray, data.Exp, data.Debt);
				this.lootPage.gameObject.SetActive(true);
			});
			this.statusPage.Init(this._displayData);
			this.InitLegacyAnimation();
			UIElement element = this.Element;
			element.OnShowed = (Action)Delegate.Combine(element.OnShowed, new Action(this.OnShowed));
		}

		// Token: 0x06008ECD RID: 36557 RVA: 0x00427D0C File Offset: 0x00425F0C
		private void InitLegacyAnimation()
		{
			List<short> legacyTemplateIds = this._displayData.LegacyTemplateIds;
			bool showLegacy;
			if (legacyTemplateIds == null)
			{
				showLegacy = false;
			}
			else
			{
				showLegacy = legacyTemplateIds.All((short id) => id >= 0);
			}
			this._showLegacy = showLegacy;
			bool isSwordTombLegacy;
			if (this._showLegacy)
			{
				isSwordTombLegacy = this._displayData.LegacyTemplateIds.Exists((short id) => id <= 8);
			}
			else
			{
				isSwordTombLegacy = false;
			}
			this._isSwordTombLegacy = isSwordTombLegacy;
			this._legacyAnimInteractable = false;
			bool flag = this.legacySwordTombInEffect != null;
			if (flag)
			{
				this.legacySwordTombInEffect.gameObject.SetActive(false);
			}
			bool flag2 = this.legacySwordTombLoopEffect != null;
			if (flag2)
			{
				this.legacySwordTombLoopEffect.gameObject.SetActive(false);
			}
			bool flag3 = this.legacySwordTombOutEffect != null;
			if (flag3)
			{
				this.legacySwordTombOutEffect.gameObject.SetActive(false);
			}
			bool flag4 = this.legacyNormalInEffect != null;
			if (flag4)
			{
				this.legacyNormalInEffect.gameObject.SetActive(false);
			}
			bool flag5 = this.legacyNormalLoopEffect != null;
			if (flag5)
			{
				this.legacyNormalLoopEffect.gameObject.SetActive(false);
			}
			bool flag6 = this.legacyNormalOutEffect != null;
			if (flag6)
			{
				this.legacyNormalOutEffect.gameObject.SetActive(false);
			}
			bool flag7 = this.legacyAnimPanel != null;
			if (flag7)
			{
				this.legacyAnimPanel.gameObject.SetActive(false);
				this.legacyAnimPanel.ClearAndAddListener(new Action(this.CloseLegacyAnimPanel));
			}
			bool flag8 = this.legacyContinue != null;
			if (flag8)
			{
				this.legacyContinue.SetActive(false);
			}
		}

		// Token: 0x06008ECE RID: 36558 RVA: 0x00427ECC File Offset: 0x004260CC
		private void OnShowed()
		{
			GlobalDomainMethod.Call.InvokeGuidingTrigger(281);
			bool isWin = CombatResultType.IsPlayerWin(this._combatResult);
			AudioManager.Instance.PlaySound(isWin ? "battle_victory" : "battle_fail", false, false);
			bool flag = !this._showLegacy || this.legacyAnimPanel == null;
			if (!flag)
			{
				Sequence sequence = DOTween.Sequence();
				ParticleSystem inEffect = this._isSwordTombLegacy ? this.legacySwordTombInEffect : this.legacyNormalInEffect;
				ParticleSystem loopEffect = this._isSwordTombLegacy ? this.legacySwordTombLoopEffect : this.legacyNormalLoopEffect;
				sequence.AppendInterval(0.5f);
				sequence.AppendCallback(delegate
				{
					this.legacyAnimPanel.gameObject.SetActive(true);
					bool flag2 = inEffect != null;
					if (flag2)
					{
						inEffect.gameObject.SetActive(true);
						inEffect.Play(true);
					}
					AudioManager.Instance.PlaySound("ui_collect_legacy", false, false);
				});
				sequence.AppendInterval(1f);
				sequence.AppendCallback(delegate
				{
					bool flag2 = inEffect != null;
					if (flag2)
					{
						inEffect.gameObject.SetActive(false);
					}
					bool flag3 = loopEffect != null;
					if (flag3)
					{
						loopEffect.gameObject.SetActive(true);
						loopEffect.Play(true);
					}
					bool flag4 = this.legacyContinue != null;
					if (flag4)
					{
						this.legacyContinue.SetActive(true);
					}
					this._legacyAnimInteractable = true;
				});
			}
		}

		// Token: 0x06008ECF RID: 36559 RVA: 0x00427FB4 File Offset: 0x004261B4
		private void InitTabGroup()
		{
			this.tabGroup.OnActiveIndexChange += this.OnTabChanged;
			this.tabGroup.Init(0);
			ToggleGroupHotkeyController.Set(this.Element, this.tabGroup, 0, null);
			this.tabGroup.Set(0, false);
		}

		// Token: 0x06008ED0 RID: 36560 RVA: 0x00428009 File Offset: 0x00426209
		private void OnTabChanged(int newIndex, int oldIndex)
		{
			this.lootPage.gameObject.SetActive(newIndex == 0);
			this.statusPage.gameObject.SetActive(newIndex == 1);
		}

		// Token: 0x06008ED1 RID: 36561 RVA: 0x00428038 File Offset: 0x00426238
		private void OnSelectAllChanged(bool selectAll)
		{
			this._selectedItems.Clear();
			bool flag = selectAll && this._displayData.ItemList != null;
			if (flag)
			{
				foreach (ItemDisplayData itemData in this._displayData.ItemList)
				{
					this._selectedItems.Add(itemData.Key, itemData.Amount);
				}
			}
		}

		// Token: 0x06008ED2 RID: 36562 RVA: 0x004280CC File Offset: 0x004262CC
		protected override void OnClick(Transform btn)
		{
			bool flag = btn.name == "ConfirmButton";
			if (flag)
			{
				this.OnConfirm();
			}
		}

		// Token: 0x06008ED3 RID: 36563 RVA: 0x004280F8 File Offset: 0x004262F8
		private void OnConfirm()
		{
			bool flag = !this._isMartialArtist;
			if (flag)
			{
				CombatDomainMethod.Call.SelectGetItem(new List<ItemKey>(this._selectedItems.Keys), new List<int>(this._selectedItems.Values));
				TaiwuEventDomainMethod.Call.SetListenerEventActionIntArg("CombatOver", "CombatResult", (int)this._combatResult);
				TaiwuEventDomainMethod.Call.SetListenerEventActionIntArg("CombatOver", "CombatType", (int)this._combatType);
				TaiwuEventDomainMethod.Call.SetListenerEventActionIntArg("CombatOver", "MainEnemyId", this._mainEnemyId);
				TaiwuEventDomainMethod.Call.SetListenerEventActionIntArg("CombatOver", "EnemyTemplateId", (int)this._mainEnemyTemplateId);
				TaiwuEventDomainMethod.Call.TriggerListener("CombatOver", true);
			}
			else
			{
				List<ItemDisplayData> selectedList = new List<ItemDisplayData>();
				foreach (ItemDisplayData itemData in this._displayData.ItemList)
				{
					bool flag2 = this._selectedItems.ContainsKey(itemData.Key);
					if (flag2)
					{
						selectedList.Add(itemData);
					}
				}
				CombatDomainMethod.Call.ApplyCombatResultDataEffect(this._displayData, selectedList);
			}
			AudioManager.Instance.PlaySound("ui_default_cancel", false, false);
			UIManager.Instance.HideUI(this.Element);
			UIElement.MapBlockCharList.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("skipRefreshData", true));
			UIManager.Instance.StackBack(null);
			UIElement.StateCombat.Hide(true);
			SingletonObject.getInstance<WorldMapModel>().UpdateBgm();
			short mainEnemyTemplateId = this._mainEnemyTemplateId;
			if (mainEnemyTemplateId >= 681)
			{
				if (mainEnemyTemplateId > 685)
				{
					goto IL_1A1;
				}
			}
			else
			{
				if (mainEnemyTemplateId < 296)
				{
					goto IL_1A1;
				}
				if (mainEnemyTemplateId > 374)
				{
					goto IL_1A1;
				}
			}
			bool flag3 = true;
			goto IL_1A4;
			IL_1A1:
			flag3 = false;
			IL_1A4:
			bool flag4 = flag3;
			if (flag4)
			{
				TaiwuEventDomainMethod.Call.CloseUI("UI_CombatResult", false, (int)this._mainEnemyTemplateId);
			}
		}

		// Token: 0x06008ED4 RID: 36564 RVA: 0x004282D8 File Offset: 0x004264D8
		private void Update()
		{
			bool flag = CommonCommandKit.Space.Check(this.Element, false, false, false, true, false);
			if (flag)
			{
				bool flag2 = this._legacyAnimInteractable && this.legacyAnimPanel != null && this.legacyAnimPanel.gameObject.activeSelf;
				if (flag2)
				{
					this.CloseLegacyAnimPanel();
				}
				else
				{
					this.OnConfirm();
				}
			}
		}

		// Token: 0x06008ED5 RID: 36565 RVA: 0x0042833F File Offset: 0x0042653F
		private void OnDisable()
		{
			this.tabGroup.OnActiveIndexChange -= this.OnTabChanged;
		}

		// Token: 0x06008ED6 RID: 36566 RVA: 0x0042835C File Offset: 0x0042655C
		public override void QuickHide()
		{
			bool flag = this._legacyAnimInteractable && this.legacyAnimPanel != null && this.legacyAnimPanel.gameObject.activeSelf;
			if (flag)
			{
				this.CloseLegacyAnimPanel();
			}
			else
			{
				this.OnConfirm();
			}
		}

		// Token: 0x06008ED7 RID: 36567 RVA: 0x004283A8 File Offset: 0x004265A8
		private void CloseLegacyAnimPanel()
		{
			bool flag = !this._legacyAnimInteractable;
			if (!flag)
			{
				this._legacyAnimInteractable = false;
				bool flag2 = this.legacyContinue != null;
				if (flag2)
				{
					this.legacyContinue.SetActive(false);
				}
				Sequence sequence = DOTween.Sequence();
				ParticleSystem loopEffect = this._isSwordTombLegacy ? this.legacySwordTombLoopEffect : this.legacyNormalLoopEffect;
				ParticleSystem outEffect = this._isSwordTombLegacy ? this.legacySwordTombOutEffect : this.legacyNormalOutEffect;
				bool flag3 = loopEffect != null;
				if (flag3)
				{
					loopEffect.gameObject.SetActive(false);
				}
				bool flag4 = outEffect != null;
				if (flag4)
				{
					outEffect.gameObject.SetActive(true);
					outEffect.Play(true);
				}
				sequence.AppendInterval(1f);
				sequence.AppendCallback(delegate
				{
					bool flag5 = outEffect != null;
					if (flag5)
					{
						outEffect.gameObject.SetActive(false);
					}
					bool flag6 = this.legacyAnimPanel != null;
					if (flag6)
					{
						this.legacyAnimPanel.gameObject.SetActive(false);
					}
				});
			}
		}

		// Token: 0x04006CDF RID: 27871
		[SerializeField]
		private CombatResultWinLose winLoseDisplay;

		// Token: 0x04006CE0 RID: 27872
		[SerializeField]
		private CToggleGroup tabGroup;

		// Token: 0x04006CE1 RID: 27873
		[SerializeField]
		private CombatResultLootPage lootPage;

		// Token: 0x04006CE2 RID: 27874
		[SerializeField]
		private CombatResultStatusPage statusPage;

		// Token: 0x04006CE3 RID: 27875
		[SerializeField]
		private CButton confirmButton;

		// Token: 0x04006CE4 RID: 27876
		[Header("遗惠动画")]
		[SerializeField]
		private CButton legacyAnimPanel;

		// Token: 0x04006CE5 RID: 27877
		[SerializeField]
		private GameObject legacyContinue;

		// Token: 0x04006CE6 RID: 27878
		[SerializeField]
		private ParticleSystem legacySwordTombInEffect;

		// Token: 0x04006CE7 RID: 27879
		[SerializeField]
		private ParticleSystem legacySwordTombLoopEffect;

		// Token: 0x04006CE8 RID: 27880
		[SerializeField]
		private ParticleSystem legacySwordTombOutEffect;

		// Token: 0x04006CE9 RID: 27881
		[SerializeField]
		private ParticleSystem legacyNormalInEffect;

		// Token: 0x04006CEA RID: 27882
		[SerializeField]
		private ParticleSystem legacyNormalLoopEffect;

		// Token: 0x04006CEB RID: 27883
		[SerializeField]
		private ParticleSystem legacyNormalOutEffect;

		// Token: 0x04006CEC RID: 27884
		private sbyte _combatResult;

		// Token: 0x04006CED RID: 27885
		private sbyte _combatType;

		// Token: 0x04006CEE RID: 27886
		private int _mainEnemyId;

		// Token: 0x04006CEF RID: 27887
		private short _mainEnemyTemplateId;

		// Token: 0x04006CF0 RID: 27888
		private CombatResultDisplayData _displayData;

		// Token: 0x04006CF1 RID: 27889
		private bool _isMartialArtist;

		// Token: 0x04006CF2 RID: 27890
		private readonly Dictionary<ItemKey, int> _selectedItems = new Dictionary<ItemKey, int>();

		// Token: 0x04006CF3 RID: 27891
		private const int MaxSwordTombLegacyId = 8;

		// Token: 0x04006CF4 RID: 27892
		private bool _showLegacy;

		// Token: 0x04006CF5 RID: 27893
		private bool _isSwordTombLegacy;

		// Token: 0x04006CF6 RID: 27894
		private bool _legacyAnimInteractable;
	}
}
