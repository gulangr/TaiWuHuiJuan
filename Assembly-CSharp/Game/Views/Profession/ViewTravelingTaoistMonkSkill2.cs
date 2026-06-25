using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Config;
using FrameWork;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Taiwu.Profession;
using GameData.Domains.TaiwuEvent;
using GameData.GameDataBridge;
using GameData.Serializer;
using TMPro;
using UnityEngine;

namespace Game.Views.Profession
{
	// Token: 0x020007CE RID: 1998
	public class ViewTravelingTaoistMonkSkill2 : UIBase
	{
		// Token: 0x060061A8 RID: 25000 RVA: 0x002CCD58 File Offset: 0x002CAF58
		private void Awake()
		{
			this.leftArea.OnClickFeature += this.OnFeatureClickHandler;
			this.rightArea.OnClickFeature += this.OnFeatureClickHandler;
			this.leftArea.OnPointerEnterFeature += this.OnPointerEnterFeatureHandler;
			this.leftArea.OnPointerExitFeature += this.OnPointerExitFeatureHandler;
			this.rightArea.OnPointerEnterFeature += this.OnPointerEnterFeatureHandler;
			this.rightArea.OnPointerExitFeature += this.OnPointerExitFeatureHandler;
		}

		// Token: 0x060061A9 RID: 25001 RVA: 0x002CCDF8 File Offset: 0x002CAFF8
		public override void OnInit(ArgumentBox argsBox)
		{
			this._featureGroup.Clear();
			this._mutexFeatureDic.Clear();
			this._taiwuFeaturesLeft.Clear();
			this._taiwuFeaturesTobeExchange.Clear();
			this._targetChrFeaturesLeft.Clear();
			this._targetChrFeaturesTobeExchange.Clear();
			argsBox.Get("characterId", out this._targetCharId);
			int taiwuId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			this._taiwuId = this.GetValidLastSelectedCharId(taiwuId);
			this._onConfirm = null;
			this.buttonClosePopup.ClearAndAddListener(delegate
			{
				this.QuickHide();
			});
			this.confirmBtn.ClearAndAddListener(new Action(this.ConfirmExchange));
			this.UpdateExchangeBtnState(false);
			this.SetLeftArea();
			this.SetRightArea();
			this.healthChangeValue.text = "0.00%";
			this.NeedDataListenerId = true;
			this.NeedWaitData = true;
			UIElement element = this.Element;
			element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(this.RequestCharacterDisplayData));
		}

		// Token: 0x060061AA RID: 25002 RVA: 0x002CCF0B File Offset: 0x002CB10B
		private void RequestCharacterDisplayData()
		{
			CharacterDomainMethod.Call.GetCharacterDisplayData(this.Element.GameDataListenerId, this._taiwuId);
			this.Element.ShowAfterRefresh();
		}

		// Token: 0x060061AB RID: 25003 RVA: 0x002CCF34 File Offset: 0x002CB134
		private void SetHealthChangeStatus()
		{
			bool flag = this._characterDisplayData == null;
			if (!flag)
			{
				int cost = (int)(this._characterDisplayData.LeftMaxHealth - this._previewLeftMaxHealth);
				EHealthType healthType = CommonUtils.GetHealthType(Math.Min(this._characterDisplayData.Health, this._characterDisplayData.LeftMaxHealth), this._characterDisplayData.LeftMaxHealth, this._taiwuId);
				short previewHealth = Math.Min(this._characterDisplayData.Health, this._previewLeftMaxHealth);
				EHealthType healthTypePreview = CommonUtils.GetHealthType(previewHealth, this._previewLeftMaxHealth, this._taiwuId);
				StringBuilder sb = EasyPool.Get<StringBuilder>();
				sb.Append(CommonUtils.GetHealthString(healthType));
				sb.Append(TMPTextSpriteHelper.GetStringWithTextSpriteTag("ui9_back_eventwindow_loop_arrow_1"));
				sb.Append(CommonUtils.GetHealthString(healthTypePreview));
				this.healthChangeStatus.text = sb.ToString();
				this.healthChangeStatus.GetComponent<TMPTextSpriteHelper>().Parse();
				EasyPool.Free<StringBuilder>(sb);
			}
		}

		// Token: 0x060061AC RID: 25004 RVA: 0x002CD028 File Offset: 0x002CB228
		private void SetLeftArea()
		{
			this.leftArea.Set(this, this._taiwuId, this._taiwuId, this._taiwuFeaturesLeft, this._taiwuFeaturesTobeExchange, this._featureGroup, this._mutexFeatureDic, true);
		}

		// Token: 0x060061AD RID: 25005 RVA: 0x002CD068 File Offset: 0x002CB268
		private void SetRightArea()
		{
			this.rightArea.Set(this, this._targetCharId, this._taiwuId, this._targetChrFeaturesLeft, this._targetChrFeaturesTobeExchange, this._featureGroup, this._mutexFeatureDic, true);
		}

		// Token: 0x060061AE RID: 25006 RVA: 0x002CD0A8 File Offset: 0x002CB2A8
		private void OnFeatureClickHandler(int featureId, ExchangeFeatureHolderItem.FeatureLocation location)
		{
			switch (location)
			{
			case ExchangeFeatureHolderItem.FeatureLocation.TaiwuLeft:
			{
				bool flag = this._mutexFeatureDic.ContainsKey(featureId);
				if (flag)
				{
					this.SwapDoubleLeftToExchange(featureId, location);
				}
				else
				{
					this.SwapSingle(this._taiwuFeaturesLeft, this._taiwuFeaturesTobeExchange, this.leftArea.FeatureScroll, this.leftArea.FeatureScrollTobeExchange, featureId);
				}
				break;
			}
			case ExchangeFeatureHolderItem.FeatureLocation.TaiwuTobeExchanged:
			{
				int targetFeatureId;
				bool flag2 = this._mutexFeatureDic.TryGetValue(featureId, out targetFeatureId);
				if (flag2)
				{
					this.SwapDouble(false, featureId, targetFeatureId);
				}
				else
				{
					this.SwapSingle(this._taiwuFeaturesTobeExchange, this._taiwuFeaturesLeft, this.leftArea.FeatureScrollTobeExchange, this.leftArea.FeatureScroll, featureId);
				}
				break;
			}
			case ExchangeFeatureHolderItem.FeatureLocation.TargetLeft:
			{
				bool flag3 = this._mutexFeatureDic.ContainsKey(featureId);
				if (flag3)
				{
					this.SwapDoubleLeftToExchange(featureId, location);
				}
				else
				{
					this.SwapSingle(this._targetChrFeaturesLeft, this._targetChrFeaturesTobeExchange, this.rightArea.FeatureScroll, this.rightArea.FeatureScrollTobeExchange, featureId);
				}
				break;
			}
			case ExchangeFeatureHolderItem.FeatureLocation.TargetTobeExchanged:
			{
				int taiwuFeatureId;
				bool flag4 = this._mutexFeatureDic.TryGetValue(featureId, out taiwuFeatureId);
				if (flag4)
				{
					this.SwapDouble(false, taiwuFeatureId, featureId);
				}
				else
				{
					this.SwapSingle(this._targetChrFeaturesTobeExchange, this._targetChrFeaturesLeft, this.rightArea.FeatureScrollTobeExchange, this.rightArea.FeatureScroll, featureId);
				}
				break;
			}
			}
		}

		// Token: 0x060061AF RID: 25007 RVA: 0x002CD215 File Offset: 0x002CB415
		public override void QuickHide()
		{
			TaiwuEventDomainMethod.Call.TriggerListener("TravelingTaoistMonkSkill2Executed", true);
			base.QuickHide();
		}

		// Token: 0x060061B0 RID: 25008 RVA: 0x002CD22C File Offset: 0x002CB42C
		private void OnPointerEnterFeatureHandler(int featureId, ExchangeFeatureHolderItem.FeatureLocation location)
		{
			int mutexFeatureId;
			bool flag = this._mutexFeatureDic.TryGetValue(featureId, out mutexFeatureId);
			if (flag)
			{
				ExchangeFeatureHolderItem otherArea = (location == ExchangeFeatureHolderItem.FeatureLocation.TaiwuLeft || location == ExchangeFeatureHolderItem.FeatureLocation.TaiwuTobeExchanged) ? this.rightArea : this.leftArea;
				this.HighlightFeatureInArea(mutexFeatureId, otherArea);
			}
		}

		// Token: 0x060061B1 RID: 25009 RVA: 0x002CD270 File Offset: 0x002CB470
		private void OnPointerExitFeatureHandler(int featureId, ExchangeFeatureHolderItem.FeatureLocation location)
		{
			int mutexFeatureId;
			bool flag = this._mutexFeatureDic.TryGetValue(featureId, out mutexFeatureId);
			if (flag)
			{
				ExchangeFeatureHolderItem otherArea = (location == ExchangeFeatureHolderItem.FeatureLocation.TaiwuLeft || location == ExchangeFeatureHolderItem.FeatureLocation.TaiwuTobeExchanged) ? this.rightArea : this.leftArea;
				this.UnhighlightFeatureInArea(mutexFeatureId, otherArea);
			}
		}

		// Token: 0x060061B2 RID: 25010 RVA: 0x002CD2B4 File Offset: 0x002CB4B4
		private void HighlightFeatureInArea(int featureId, ExchangeFeatureHolderItem area)
		{
			int index = area.FeaturesLeft.IndexOf(featureId);
			bool flag = index >= 0;
			if (flag)
			{
				GameObject cell = area.FeatureScroll.GetActiveCell(index);
				bool flag2 = cell != null;
				if (flag2)
				{
					ExchangeFeatureItem item = cell.GetComponent<ExchangeFeatureItem>();
					bool flag3 = item != null;
					if (flag3)
					{
						item.SetHighlight(true);
					}
				}
			}
			else
			{
				index = area.FeaturesToBeExchange.IndexOf(featureId);
				bool flag4 = index >= 0;
				if (flag4)
				{
					GameObject cell2 = area.FeatureScrollTobeExchange.GetActiveCell(index);
					bool flag5 = cell2 != null;
					if (flag5)
					{
						ExchangeFeatureItem item2 = cell2.GetComponent<ExchangeFeatureItem>();
						bool flag6 = item2 != null;
						if (flag6)
						{
							item2.SetHighlight(true);
						}
					}
				}
			}
		}

		// Token: 0x060061B3 RID: 25011 RVA: 0x002CD370 File Offset: 0x002CB570
		private void UnhighlightFeatureInArea(int featureId, ExchangeFeatureHolderItem area)
		{
			int index = area.FeaturesLeft.IndexOf(featureId);
			bool flag = index >= 0;
			if (flag)
			{
				GameObject cell = area.FeatureScroll.GetActiveCell(index);
				bool flag2 = cell != null;
				if (flag2)
				{
					ExchangeFeatureItem item = cell.GetComponent<ExchangeFeatureItem>();
					bool flag3 = item != null;
					if (flag3)
					{
						item.SetHighlight(false);
					}
				}
			}
			else
			{
				index = area.FeaturesToBeExchange.IndexOf(featureId);
				bool flag4 = index >= 0;
				if (flag4)
				{
					GameObject cell2 = area.FeatureScrollTobeExchange.GetActiveCell(index);
					bool flag5 = cell2 != null;
					if (flag5)
					{
						ExchangeFeatureItem item2 = cell2.GetComponent<ExchangeFeatureItem>();
						bool flag6 = item2 != null;
						if (flag6)
						{
							item2.SetHighlight(false);
						}
					}
				}
			}
		}

		// Token: 0x060061B4 RID: 25012 RVA: 0x002CD42C File Offset: 0x002CB62C
		private void SwapDoubleLeftToExchange(int featureId, ExchangeFeatureHolderItem.FeatureLocation featureLocation)
		{
			bool flag = featureLocation == ExchangeFeatureHolderItem.FeatureLocation.TaiwuLeft;
			if (flag)
			{
				int targetCharId = this._mutexFeatureDic[featureId];
				this.SwapDouble(true, featureId, targetCharId);
			}
			else
			{
				bool flag2 = featureLocation == ExchangeFeatureHolderItem.FeatureLocation.TargetLeft;
				if (flag2)
				{
					int taiwuCharId = this._mutexFeatureDic[featureId];
					this.SwapDouble(true, taiwuCharId, featureId);
				}
			}
		}

		// Token: 0x060061B5 RID: 25013 RVA: 0x002CD480 File Offset: 0x002CB680
		private void SwapDouble(bool leftToExchange, int taiwuFeatureId, int targetCharFeatureId)
		{
			if (leftToExchange)
			{
				this.SwapSingle(this._taiwuFeaturesLeft, this._taiwuFeaturesTobeExchange, this.leftArea.FeatureScroll, this.leftArea.FeatureScrollTobeExchange, taiwuFeatureId);
				this.SwapSingle(this._targetChrFeaturesLeft, this._targetChrFeaturesTobeExchange, this.rightArea.FeatureScroll, this.rightArea.FeatureScrollTobeExchange, targetCharFeatureId);
			}
			else
			{
				this.SwapSingle(this._taiwuFeaturesTobeExchange, this._taiwuFeaturesLeft, this.leftArea.FeatureScrollTobeExchange, this.leftArea.FeatureScroll, taiwuFeatureId);
				this.SwapSingle(this._targetChrFeaturesTobeExchange, this._targetChrFeaturesLeft, this.rightArea.FeatureScrollTobeExchange, this.rightArea.FeatureScroll, targetCharFeatureId);
			}
		}

		// Token: 0x060061B6 RID: 25014 RVA: 0x002CD544 File Offset: 0x002CB744
		private void SwapSingle(List<int> from, List<int> to, InfinityScroll fromScroll, InfinityScroll toScroll, int id)
		{
			from.Remove(id);
			to.Add(id);
			fromScroll.UpdateData(from.Count);
			toScroll.UpdateData(to.Count);
			this.UpdateExchangeBtnState(false);
			int cost = this.GetMaxHpCost();
			bool flag = cost > 0 && this._characterDisplayData != null;
			if (flag)
			{
				CharacterDomainMethod.Call.GetPreviewLeftMaxHealth(this.Element.GameDataListenerId, this._taiwuId, (short)cost);
			}
			else
			{
				CharacterDisplayData characterDisplayData = this._characterDisplayData;
				this._previewLeftMaxHealth = ((characterDisplayData != null) ? characterDisplayData.LeftMaxHealth : -1);
				this.SetHealthChangeStatus();
				this.healthChangeValue.text = string.Format("{0:f2}%", (float)cost / (float)this._characterDisplayData.LeftMaxHealth);
			}
		}

		// Token: 0x060061B7 RID: 25015 RVA: 0x002CD60C File Offset: 0x002CB80C
		public override void OnNotifyGameData(List<NotificationWrapper> notifications)
		{
			foreach (NotificationWrapper wrapper in notifications)
			{
				Notification notification = wrapper.Notification;
				byte type = notification.Type;
				byte b = type;
				if (b == 1)
				{
					bool flag = notification.DomainId == 4 && notification.MethodId == 131;
					if (flag)
					{
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._characterDisplayData);
						CharacterDisplayData characterDisplayData = this._characterDisplayData;
						this._previewLeftMaxHealth = ((characterDisplayData != null) ? characterDisplayData.LeftMaxHealth : -1);
						this.SetHealthChangeStatus();
					}
					else
					{
						bool flag2 = notification.DomainId == 4 && notification.MethodId == 221;
						if (flag2)
						{
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._previewLeftMaxHealth);
							bool flag3 = this._characterDisplayData != null && this.GetMaxHpCost() > 0;
							if (flag3)
							{
								int cost = (int)(this._characterDisplayData.LeftMaxHealth - this._previewLeftMaxHealth);
								float percent = (cost < (int)this._characterDisplayData.LeftMaxHealth) ? ((float)cost / (float)this._characterDisplayData.LeftMaxHealth) : 1f;
								bool flag4 = this._characterDisplayData.LeftMaxHealth > 0;
								if (flag4)
								{
									this.healthChangeValue.text = string.Format("{0:f2}%", percent * 100f);
								}
								this.SetHealthChangeStatus();
							}
						}
					}
				}
			}
		}

		// Token: 0x060061B8 RID: 25016 RVA: 0x002CD7B8 File Offset: 0x002CB9B8
		private void UpdateExchangeBtnState(bool confirmed = false)
		{
			TooltipInvoker disPlayer = this.confirmBtn.GetComponent<TooltipInvoker>();
			disPlayer.NeedRefresh = true;
			TooltipInvoker tooltipInvoker = disPlayer;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = new ArgumentBox();
			}
			disPlayer.enabled = true;
			if (confirmed)
			{
				this.confirmBtn.interactable = false;
				disPlayer.RuntimeParam.Set("arg0", LocalStringManager.Get(LanguageKey.LK_ProfessionTravelingTaoistMonkSkill2Text7));
			}
			else
			{
				bool flag = this._taiwuFeaturesTobeExchange.Count != this._targetChrFeaturesTobeExchange.Count;
				if (flag)
				{
					this.confirmBtn.interactable = false;
					disPlayer.RuntimeParam.Set("arg0", LocalStringManager.Get(LanguageKey.LK_ProfessionTravelingTaoistMonkSkill2Text3));
				}
				else
				{
					bool flag2 = this._taiwuFeaturesTobeExchange.Count == 0 || this._targetChrFeaturesTobeExchange.Count == 0;
					if (flag2)
					{
						this.confirmBtn.interactable = false;
						disPlayer.RuntimeParam.Set("arg0", LocalStringManager.Get(LanguageKey.LK_ProfessionTravelingTaoistMonkSkill2Text11));
					}
					else
					{
						bool flag3 = this._previewLeftMaxHealth <= 0;
						if (flag3)
						{
							this.confirmBtn.interactable = false;
							disPlayer.RuntimeParam.Set("arg0", LocalStringManager.Get(LanguageKey.LK_ProfessionTravelingTaoistMonkSkill2Text10));
						}
						else
						{
							this.confirmBtn.interactable = true;
							disPlayer.enabled = false;
						}
					}
				}
			}
		}

		// Token: 0x060061B9 RID: 25017 RVA: 0x002CD924 File Offset: 0x002CBB24
		private int GetMaxHpCost()
		{
			int levelSum = 0;
			foreach (int id in this._targetChrFeaturesTobeExchange)
			{
				levelSum += Mathf.Abs((int)CharacterFeature.Instance[id].Level);
			}
			int curSeniority = SingletonObject.getInstance<ProfessionModel>().GetProfessionData(14).Seniority;
			int maxSeniority = 3000000;
			return (int)((double)levelSum * (9.0 - 6.0 * (double)curSeniority / (double)maxSeniority));
		}

		// Token: 0x060061BA RID: 25018 RVA: 0x002CD9CC File Offset: 0x002CBBCC
		private int GetValidLastSelectedCharId(int taiwuId)
		{
			bool flag = ViewTravelingTaoistMonkSkill2._lastSelectedCharId == -1;
			int result;
			if (flag)
			{
				ViewTravelingTaoistMonkSkill2._lastSelectedCharId = taiwuId;
				result = taiwuId;
			}
			else
			{
				CharacterMonitorModel monitor = SingletonObject.getInstance<CharacterMonitorModel>();
				List<int> charIdList = monitor.GetTaiwuTeamCharIds();
				bool flag2 = charIdList.Contains(ViewTravelingTaoistMonkSkill2._lastSelectedCharId);
				if (flag2)
				{
					result = ViewTravelingTaoistMonkSkill2._lastSelectedCharId;
				}
				else
				{
					ViewTravelingTaoistMonkSkill2._lastSelectedCharId = taiwuId;
					result = taiwuId;
				}
			}
			return result;
		}

		// Token: 0x060061BB RID: 25019 RVA: 0x002CDA24 File Offset: 0x002CBC24
		public void ReplaceCharacter(int charId)
		{
			bool flag = this._taiwuId == charId;
			if (!flag)
			{
				this._taiwuId = charId;
				ViewTravelingTaoistMonkSkill2._lastSelectedCharId = charId;
				this._characterDisplayData = null;
				this._previewLeftMaxHealth = -1;
				this.RequestCharacterDisplayData();
				this._featureGroup.Clear();
				this._mutexFeatureDic.Clear();
				this._taiwuFeaturesLeft.Clear();
				this._taiwuFeaturesTobeExchange.Clear();
				this._targetChrFeaturesLeft.Clear();
				this._targetChrFeaturesTobeExchange.Clear();
				this.SetLeftArea();
				this.SetRightArea();
				this.UpdateExchangeBtnState(false);
				this.healthChangeValue.text = "0.00%";
			}
		}

		// Token: 0x060061BC RID: 25020 RVA: 0x002CDAD8 File Offset: 0x002CBCD8
		private void ConfirmExchange()
		{
			Debug.Log("ConfirmExchange");
			ProfessionSkillArg professionSkillArg2 = new ProfessionSkillArg();
			professionSkillArg2.ProfessionId = 14;
			professionSkillArg2.SkillId = 58;
			professionSkillArg2.CharId = this._taiwuId;
			professionSkillArg2.CharIds = new List<int>
			{
				this._targetCharId
			};
			professionSkillArg2.BookIds = new List<int>(this._taiwuFeaturesTobeExchange);
			professionSkillArg2.EffectBlocks = new List<short>(from x in this._targetChrFeaturesTobeExchange
			select (short)x);
			professionSkillArg2.IsSuccess = true;
			ProfessionSkillArg professionSkillArg = professionSkillArg2;
			ArgumentBox argsBox = EasyPool.Get<ArgumentBox>();
			argsBox.SetObject("ProfessionSkillArg", professionSkillArg);
			argsBox.SetObject("OnConfirm", new Action(this.<ConfirmExchange>g__OnProfessionSkillConfirm|42_0));
			UIElement.ProfessionSkillConfirm.SetOnInitArgs(argsBox);
			UIManager.Instance.MaskUI(UIElement.ProfessionSkillConfirm);
		}

		// Token: 0x060061C0 RID: 25024 RVA: 0x002CDC40 File Offset: 0x002CBE40
		[CompilerGenerated]
		private void <ConfirmExchange>g__OnProfessionSkillConfirm|42_0()
		{
			int taiwuBadFeature = 0;
			foreach (int featureId in this._taiwuFeaturesTobeExchange)
			{
				CharacterFeatureItem config = CharacterFeature.Instance[featureId];
				bool flag = config.Type == ECharacterFeatureType.Bad;
				if (flag)
				{
					taiwuBadFeature++;
				}
			}
			int targetBadFeature = 0;
			foreach (int featureId2 in this._targetChrFeaturesTobeExchange)
			{
				CharacterFeatureItem config2 = CharacterFeature.Instance[featureId2];
				bool flag2 = config2.Type == ECharacterFeatureType.Bad;
				if (flag2)
				{
					targetBadFeature++;
				}
			}
			this.RequestCharacterDisplayData();
			this._taiwuFeaturesLeft.Clear();
			this._targetChrFeaturesLeft.Clear();
			this._taiwuFeaturesTobeExchange.Clear();
			this._targetChrFeaturesTobeExchange.Clear();
			this.UpdateExchangeBtnState(true);
			this.healthChangeValue.text = "0.00%";
			Action onConfirm = this._onConfirm;
			if (onConfirm != null)
			{
				onConfirm();
			}
			UIManager.Instance.HideUI(UIElement.TravelingTaoistMonkSkill2);
		}

		// Token: 0x040043C3 RID: 17347
		private int _targetCharId = -1;

		// Token: 0x040043C4 RID: 17348
		private int _taiwuId = -1;

		// Token: 0x040043C5 RID: 17349
		private static int _lastSelectedCharId = -1;

		// Token: 0x040043C6 RID: 17350
		[SerializeField]
		private CButton buttonClosePopup;

		// Token: 0x040043C7 RID: 17351
		[SerializeField]
		private CButton confirmBtn;

		// Token: 0x040043C8 RID: 17352
		[SerializeField]
		private TextMeshProUGUI healthChangeValue;

		// Token: 0x040043C9 RID: 17353
		[SerializeField]
		private CImage healthCurrentStatusIcon;

		// Token: 0x040043CA RID: 17354
		[SerializeField]
		private TextMeshProUGUI healthCurrentStatus;

		// Token: 0x040043CB RID: 17355
		[SerializeField]
		private TextMeshProUGUI healthChangeStatus;

		// Token: 0x040043CC RID: 17356
		[SerializeField]
		private ExchangeFeatureHolderItem leftArea;

		// Token: 0x040043CD RID: 17357
		[SerializeField]
		private ExchangeFeatureHolderItem rightArea;

		// Token: 0x040043CE RID: 17358
		private CharacterDisplayData _characterDisplayData;

		// Token: 0x040043CF RID: 17359
		private short _previewLeftMaxHealth = -1;

		// Token: 0x040043D0 RID: 17360
		private readonly List<int> _taiwuFeaturesLeft = new List<int>();

		// Token: 0x040043D1 RID: 17361
		private readonly List<int> _taiwuFeaturesTobeExchange = new List<int>();

		// Token: 0x040043D2 RID: 17362
		private readonly List<int> _targetChrFeaturesLeft = new List<int>();

		// Token: 0x040043D3 RID: 17363
		private readonly List<int> _targetChrFeaturesTobeExchange = new List<int>();

		// Token: 0x040043D4 RID: 17364
		private readonly Dictionary<int, List<int>> _featureGroup = new Dictionary<int, List<int>>();

		// Token: 0x040043D5 RID: 17365
		private readonly Dictionary<int, int> _mutexFeatureDic = new Dictionary<int, int>();

		// Token: 0x040043D6 RID: 17366
		private Action _onConfirm;

		// Token: 0x040043D7 RID: 17367
		private Coroutine _coroutine;

		// Token: 0x02001D24 RID: 7460
		private enum FeatureLocation
		{
			// Token: 0x0400C525 RID: 50469
			TaiwuLeft,
			// Token: 0x0400C526 RID: 50470
			TaiwuTobeExchanged,
			// Token: 0x0400C527 RID: 50471
			TargetLeft,
			// Token: 0x0400C528 RID: 50472
			TargetTobeExchanged
		}
	}
}
