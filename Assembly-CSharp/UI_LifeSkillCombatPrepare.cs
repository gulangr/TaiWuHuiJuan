using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Config;
using Config.ConfigCells.Character;
using FrameWork;
using Game.Components.Avatar;
using Game.Views.CharacterMenu;
using GameData.Common;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Taiwu;
using GameData.Domains.Taiwu.Debate;
using GameData.Domains.TaiwuEvent;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using Spine.Unity;
using TMPro;
using UnityEngine;

// Token: 0x0200023E RID: 574
public class UI_LifeSkillCombatPrepare : UIBase
{
	// Token: 0x06002555 RID: 9557 RVA: 0x00111F20 File Offset: 0x00110120
	private void Awake()
	{
		PoolManager.SetSrcObject("LifeSkillTypeSelectionCellTemplate", this.LifeSkillTypeSelectionCellTemplate);
		Vector2 size = this.LifeSkillTypeSelectionCellTemplate.GetComponent<RectTransform>().rect.size + new Vector2(12f, 12f);
		int i = 0;
		int count = 16;
		while (i < count)
		{
			GameObject cell = PoolManager.GetObject("LifeSkillTypeSelectionCellTemplate");
			cell.transform.SetParent(this.Cells);
			cell.transform.localScale = Vector3.one;
			cell.name = string.Format("{0}_{1}", "LifeSkillType", i);
			cell.GetComponent<RectTransform>().localPosition = new Vector3(size.x * (-1.5f + (float)(i % 4)), size.y * (1.5f - (float)(i / 4)));
			i++;
		}
	}

	// Token: 0x06002556 RID: 9558 RVA: 0x0011200B File Offset: 0x0011020B
	private void OnEnable()
	{
		GEvent.Add(UiEvents.OnLifeSkillCombatForceSilentResult, new GEvent.Callback(this.OnLifeSkillCombatForceSilentResult));
	}

	// Token: 0x06002557 RID: 9559 RVA: 0x0011202A File Offset: 0x0011022A
	private void OnDisable()
	{
		GEvent.Remove(UiEvents.OnLifeSkillCombatForceSilentResult, new GEvent.Callback(this.OnLifeSkillCombatForceSilentResult));
	}

	// Token: 0x06002558 RID: 9560 RVA: 0x0011204C File Offset: 0x0011024C
	private void OnDestroy()
	{
		foreach (object obj in this.Cells)
		{
			RectTransform cell = (RectTransform)obj;
			PoolManager.Destroy("LifeSkillTypeSelectionCellTemplate", cell.gameObject);
		}
		PoolManager.RemoveData("LifeSkillTypeSelectionCellTemplate");
	}

	// Token: 0x06002559 RID: 9561 RVA: 0x001120C0 File Offset: 0x001102C0
	public override void OnInit(ArgumentBox argsBox)
	{
		this._isGivenUp = false;
		this._forceSilentSucceed = false;
		this._hasForceSilent = false;
		this._lifeSkillType = -1;
		argsBox.Get("self", out this._charIdSelf);
		argsBox.Get("adversary", out this._charIdAdversary);
		argsBox.Get<Action<sbyte, sbyte>>("callBack", out this._callBack);
		sbyte i = 0;
		sbyte count = 16;
		while (i < count)
		{
			this._lifeSkillTypeStatus[(int)i] = new UI_LifeSkillCombatPrepare.LifeSkillTypeState(i);
			i += 1;
		}
		this._lifeSkillTypeIndicesForDisplay.Shuffle(1);
		UI_LifeSkillCombatPrepare._wisdomSelf = -1;
		UI_LifeSkillCombatPrepare._wisdomAdversary = -1;
		this._lifeSkillAttainmentsSelf.Items.FixedElementField = -1;
		this._lifeSkillAttainmentsAdversary.Items.FixedElementField = -1;
		this.PanelSelf.CGet<Bubble>("LeftCloudBubble").Hide();
		this.PanelAdversary.CGet<Bubble>("RightCloudBubble").Hide();
		this.HideLifeSkillAttainment(true);
		this.HideLifeSkillAttainment(false);
		UIElement element = this.Element;
		element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(delegate()
		{
			base.CGet<GameObject>("GiveUpArea").SetActive(false);
			CharacterDomainMethod.AsyncCall.GetCharacterDisplayDataList(this, new List<int>
			{
				this._charIdSelf,
				this._charIdAdversary
			}, delegate(int offset, RawDataPool dataPool)
			{
				List<CharacterDisplayData> list = new List<CharacterDisplayData>();
				Serializer.Deserialize(dataPool, offset, ref list);
				this._characterSelf = list[0];
				this._characterAdversary = list[1];
				this.RefreshPanel(this.PanelSelf, this._characterSelf);
				this.RefreshPanel(this.PanelAdversary, this._characterAdversary);
				this._MainCoroutine = UIManager.Instance.StartCoroutine(this.Process());
				TaiwuDomainMethod.AsyncCall.GetAiBriberyDataOnPrepareLifeSkillCombat(this, this._characterAdversary.CharacterId, delegate(int offset, RawDataPool dataPool)
				{
					BriberyData bribery = null;
					Serializer.Deserialize(dataPool, offset, ref bribery);
					this.ShowAiBriberyDialog(bribery);
				});
			});
			TaiwuDomainMethod.AsyncCall.GetIsTaiwuFirstByLuck(this, this._charIdAdversary, delegate(int offset, RawDataPool dataPool)
			{
				Serializer.Deserialize(dataPool, offset, ref this._isTaiwuFirstByLuck);
			});
		}));
	}

	// Token: 0x0600255A RID: 9562 RVA: 0x001121E4 File Offset: 0x001103E4
	public override void InitMonitorFieldIds()
	{
		uint[] sub1IdList = new uint[]
		{
			97U
		};
		this.MonitorFields.Add(new UIBase.MonitorDataField(4, 0, (ulong)((long)this._charIdSelf), sub1IdList));
		this.MonitorFields.Add(new UIBase.MonitorDataField(4, 0, (ulong)((long)this._charIdAdversary), sub1IdList));
		CharacterDomainMethod.AsyncCall.GetCharacterWisdomCountById(this, this._charIdSelf, delegate(int offset, RawDataPool pool)
		{
			Refers panel = this.PanelSelf;
			int wisdom = 0;
			Serializer.Deserialize(pool, offset, ref wisdom);
			panel.CGet<CImage>("WisdomIcon").SetSprite((wisdom < 0) ? "sp_icon_renwutexing_5" : "sp_icon_renwutexing_11", false, null);
			wisdom = (UI_LifeSkillCombatPrepare._wisdomSelf = Math.Abs(wisdom));
			panel.CGet<TextMeshProUGUI>("WisdomValue").text = wisdom.ToString();
		});
		CharacterDomainMethod.AsyncCall.GetCharacterWisdomCountById(this, this._charIdAdversary, delegate(int offset, RawDataPool pool)
		{
			Refers panel = this.PanelAdversary;
			int wisdom = 0;
			Serializer.Deserialize(pool, offset, ref wisdom);
			panel.CGet<CImage>("WisdomIcon").SetSprite((wisdom < 0) ? "sp_icon_renwutexing_5" : "sp_icon_renwutexing_11", false, null);
			wisdom = (UI_LifeSkillCombatPrepare._wisdomAdversary = Math.Abs(wisdom));
			panel.CGet<TextMeshProUGUI>("WisdomValue").text = wisdom.ToString();
		});
	}

	// Token: 0x0600255B RID: 9563 RVA: 0x00112268 File Offset: 0x00110468
	public override void OnNotifyGameData(List<NotificationWrapper> notifications)
	{
		foreach (NotificationWrapper wrapper in notifications)
		{
			RawDataPool pool = wrapper.DataPool;
			Notification notification = wrapper.Notification;
			ref DataUid uid = ref notification.Uid;
			byte type = notification.Type;
			byte b = type;
			if (b == 0)
			{
				ushort dataId = uid.DataId;
				ushort num = dataId;
				if (num == 0)
				{
					uint subId = uid.SubId1;
					uint num2 = subId;
					if (num2 == 97U)
					{
						bool flag = uid.SubId0 == (ulong)((long)this._charIdSelf);
						if (flag)
						{
							Serializer.Deserialize(pool, notification.ValueOffset, ref this._lifeSkillAttainmentsSelf);
						}
						else
						{
							bool flag2 = uid.SubId0 == (ulong)((long)this._charIdAdversary);
							if (flag2)
							{
								Serializer.Deserialize(pool, notification.ValueOffset, ref this._lifeSkillAttainmentsAdversary);
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x0600255C RID: 9564 RVA: 0x0011236C File Offset: 0x0011056C
	public int GetFeatureMedalValue(IList<short> featureIds, sbyte medalType)
	{
		int baseValue = 0;
		int bonus = 0;
		int featuresIdCount = featureIds.Count;
		for (int i = 0; i < featuresIdCount; i++)
		{
			short featureId = featureIds[i];
			FeatureMedals[] allMedals = CharacterFeature.Instance[featureId].FeatureMedals;
			List<sbyte> currValues = allMedals[(int)medalType].Values;
			int currValuesCount = currValues.Count;
			for (int j = 0; j < currValuesCount; j++)
			{
				switch (currValues[j])
				{
				case 0:
					baseValue++;
					break;
				case 1:
					baseValue--;
					break;
				case 2:
					bonus++;
					break;
				case 3:
					bonus -= 3;
					break;
				}
			}
		}
		bool flag = baseValue > 0;
		int result;
		if (flag)
		{
			int finalValue = baseValue + bonus;
			bool flag2 = finalValue < 0;
			if (flag2)
			{
				result = 0;
			}
			else
			{
				bool flag3 = finalValue > 8;
				if (flag3)
				{
					result = 8;
				}
				else
				{
					result = finalValue;
				}
			}
		}
		else
		{
			bool flag4 = baseValue < 0;
			if (flag4)
			{
				int finalValue2 = baseValue - bonus;
				bool flag5 = finalValue2 < -8;
				if (flag5)
				{
					result = -8;
				}
				else
				{
					bool flag6 = finalValue2 > 0;
					if (flag6)
					{
						result = 0;
					}
					else
					{
						result = finalValue2;
					}
				}
			}
			else
			{
				result = 0;
			}
		}
		return result;
	}

	// Token: 0x0600255D RID: 9565 RVA: 0x0011249D File Offset: 0x0011069D
	private bool IsPreparing()
	{
		return UI_LifeSkillCombatPrepare._wisdomSelf < 0 || UI_LifeSkillCombatPrepare._wisdomAdversary < 0 || this._lifeSkillAttainmentsSelf.Items.FixedElementField < 0 || this._lifeSkillAttainmentsAdversary.Items.FixedElementField < 0;
	}

	// Token: 0x0600255E RID: 9566 RVA: 0x001124DC File Offset: 0x001106DC
	private unsafe IEnumerable<int> GetDescendingOrderedIndices(LifeSkillShorts shorts)
	{
		short* ptr = &shorts.Items.FixedElementField;
		return from i in Enumerable.Range(0, 16)
		orderby ptr[i] descending
		select i;
	}

	// Token: 0x0600255F RID: 9567 RVA: 0x00112520 File Offset: 0x00110720
	private unsafe IEnumerable<int> GetAscendingOrderedIndices(LifeSkillShorts shorts)
	{
		short* ptr = &shorts.Items.FixedElementField;
		return from i in Enumerable.Range(0, 16)
		orderby ptr[i]
		select i;
	}

	// Token: 0x06002560 RID: 9568 RVA: 0x00112564 File Offset: 0x00110764
	private IEnumerable<int> GetVisibleIndices()
	{
		int[] lifeSkillTypeIndicesForDisplayTemp = (int[])this._lifeSkillTypeIndicesForDisplay.Clone();
		lifeSkillTypeIndicesForDisplayTemp.Shuffle(1);
		return from i in lifeSkillTypeIndicesForDisplayTemp
		where this._lifeSkillTypeStatus[i].Interactable
		where !this._lifeSkillTypeStatus[i].Visible
		select i;
	}

	// Token: 0x06002561 RID: 9569 RVA: 0x001125B2 File Offset: 0x001107B2
	private IEnumerator Process()
	{
		yield return new WaitWhile(new Func<bool>(this.IsPreparing));
		int i = 1;
		foreach (int index in this.GetAscendingOrderedIndices(this._lifeSkillAttainmentsSelf))
		{
			this._lifeSkillTypeStatus[index].Interactable = false;
			i++;
			bool flag = i > 6;
			if (flag)
			{
				break;
			}
		}
		IEnumerator<int> enumerator = null;
		i = 1;
		foreach (int index2 in this.GetAscendingOrderedIndices(this._lifeSkillAttainmentsAdversary))
		{
			this._lifeSkillTypeStatus[index2].Interactable = false;
			i++;
			bool flag2 = i > 6;
			if (flag2)
			{
				break;
			}
		}
		IEnumerator<int> enumerator2 = null;
		List<int> left = new List<int>();
		int num;
		for (int index3 = 0; index3 < 16; index3 = num + 1)
		{
			bool interactable = this._lifeSkillTypeStatus[index3].Interactable;
			if (interactable)
			{
				left.Add(index3);
			}
			num = index3;
		}
		CollectionUtils.Shuffle<int>(GameApp.Random, left);
		for (int index4 = left.Count - 1; index4 >= 6; index4 = num - 1)
		{
			this._lifeSkillTypeStatus[left[index4]].Interactable = false;
			num = index4;
		}
		left = null;
		int j = 1;
		foreach (int index5 in this.GetVisibleIndices())
		{
			this._lifeSkillTypeStatus[index5].Visible = true;
			j++;
			bool flag3 = j > 3;
			if (flag3)
			{
				break;
			}
		}
		IEnumerator<int> enumerator3 = null;
		bool isPlayerControlling = true;
		WaitForEndOfFrame wait = new WaitForEndOfFrame();
		WaitForSeconds waitTime = new WaitForSeconds(0.3f);
		int lastUnLockedCount = this._lifeSkillTypeStatus.Count((UI_LifeSkillCombatPrepare.LifeSkillTypeState l) => l.Interactable);
		this._wisdomCost = 0;
		this._wisdomCostNpc = 0;
		this.RefreshCells(isPlayerControlling, true);
		this.SetTalkInfo(0, 0);
		this.Element.ShowAfterRefresh();
		for (;;)
		{
			int currentInteractableCount;
			bool isFinishing;
			do
			{
				if (lastUnLockedCount != (currentInteractableCount = this._lifeSkillTypeStatus.Count((UI_LifeSkillCombatPrepare.LifeSkillTypeState l) => l.Interactable)) && !this.IsFinishing)
				{
					break;
				}
				bool flag4 = currentInteractableCount == 1;
				if (flag4)
				{
					this._lifeSkillType = this._lifeSkillTypeStatus.First((UI_LifeSkillCombatPrepare.LifeSkillTypeState l) => l.Interactable).LifeSkillType;
				}
				yield return wait;
				isFinishing = this.IsFinishing;
			}
			while (!isFinishing);
			IL_49D:
			bool isFinishing2 = this.IsFinishing;
			if (isFinishing2)
			{
				break;
			}
			lastUnLockedCount = currentInteractableCount;
			yield return waitTime;
			isPlayerControlling = !isPlayerControlling;
			this.RefreshCells(isPlayerControlling, false);
			continue;
			goto IL_49D;
		}
		int k = 0;
		int count = this._lifeSkillTypeIndicesForDisplay.Length;
		while (k < count)
		{
			int stateIndex = this._lifeSkillTypeIndicesForDisplay[k];
			UI_LifeSkillCombatPrepare.LifeSkillTypeState state = this._lifeSkillTypeStatus[stateIndex];
			bool flag5 = state.LifeSkillType != this._lifeSkillType;
			if (!flag5)
			{
				Transform cell = this.Cells.GetChild(k);
				Refers refers = cell.GetComponent<Refers>();
				refers.CGet<TextMeshProUGUI>("Name").text = Config.LifeSkillType.Instance[state.LifeSkillType].Name;
				cell.GetComponent<Refers>().CGet<CImage>("Frame").enabled = true;
				break;
			}
			num = k;
			k = num + 1;
		}
		yield return new WaitForSeconds(1.5f);
		this._MainCoroutine = null;
		this.HandleCallBack();
		yield break;
	}

	// Token: 0x06002562 RID: 9570 RVA: 0x001125C4 File Offset: 0x001107C4
	public void RefreshPanel(Refers panel, CharacterDisplayData characterDisplayData)
	{
		panel.CGet<Game.Components.Avatar.Avatar>("Avatar").Refresh(characterDisplayData, true);
		string nameAndAge = NameCenter.GetMonasticTitleOrDisplayName(characterDisplayData, SingletonObject.getInstance<BasicGameData>().TaiwuCharId == characterDisplayData.CharacterId);
		panel.CGet<TextMeshProUGUI>("Name").text = nameAndAge;
		panel.CGet<TextMeshProUGUI>("Tips").text = LocalStringManager.Get((characterDisplayData.CharacterId == this._charIdSelf) ? LanguageKey.UI_LifeSkillBattle_Prepare_Tips_1 : LanguageKey.UI_LifeSkillBattle_Prepare_Tips_2);
		panel.CGet<CButtonObsolete>("Char").ClearAndAddListener(delegate
		{
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.Set("CharacterId", characterDisplayData.CharacterId);
			argBox.Set("CanOperate", false);
			argBox.SetObject("ViewCharacterMenuTaretPage", new SubPageIndex(ECharacterSubToggleBase.AttainmentBase, ECharacterSubPage.AttainmentLifeSkill));
			UIElement.CharacterMenu.SetOnInitArgs(argBox);
			UIManager.Instance.ShowUI(UIElement.CharacterMenu, true);
		});
		bool flag = panel.Names.Contains("LeftCloudBubble");
		if (flag)
		{
			panel.CGet<Bubble>("LeftCloudBubble").Hide();
		}
		else
		{
			bool flag2 = panel.Names.Contains("RightCloudBubble");
			if (flag2)
			{
				panel.CGet<Bubble>("RightCloudBubble").Hide();
			}
		}
	}

	// Token: 0x06002563 RID: 9571 RVA: 0x001126D0 File Offset: 0x001108D0
	public void RefreshCells(bool isPlayerControlling, bool isInit)
	{
		(isPlayerControlling ? this.PanelSelf : this.PanelAdversary).CGet<TextMeshProUGUI>("Tips").transform.parent.gameObject.SetActive(true);
		(isPlayerControlling ? this.PanelAdversary : this.PanelSelf).CGet<TextMeshProUGUI>("Tips").transform.parent.gameObject.SetActive(false);
		int i = 0;
		int count = this._lifeSkillTypeIndicesForDisplay.Length;
		while (i < count)
		{
			int stateIndex = this._lifeSkillTypeIndicesForDisplay[i];
			UI_LifeSkillCombatPrepare.LifeSkillTypeState state = this._lifeSkillTypeStatus[stateIndex];
			Transform cell = this.Cells.GetChild(i);
			Refers refers = cell.GetComponent<Refers>();
			CButtonObsolete button = cell.GetComponent<CButtonObsolete>();
			CImage icon = refers.CGet<CImage>("Icon");
			GameObject banSign = refers.CGet<GameObject>("Ban");
			refers.CGet<GameObject>("Detecting").SetActive(false);
			refers.CGet<TextMeshProUGUI>("Name").text = (state.Interactable ? Config.LifeSkillType.Instance[state.LifeSkillType].Name : ("<color=#grey>" + Config.LifeSkillType.Instance[state.LifeSkillType].Name + "</color>").ColorReplace());
			button.interactable = state.Interactable;
			button.ClearAndAddListener(delegate
			{
			});
			banSign.SetActive(false);
			refers.CGet<CImage>("BanImg").gameObject.SetActive(false);
			bool isVisible = state.IsVisible;
			if (isVisible)
			{
				bool interactable = state.Interactable;
				if (interactable)
				{
					bool flag = isInit || state.InitVisibleSkill.Contains(state.LifeSkillType);
					if (flag)
					{
						icon.SetSprite(string.Format("lifeskillcombat_prepare_icon_0_{0}", state.LifeSkillType), true, null);
						bool flag2 = !state.InitVisibleSkill.Contains(state.LifeSkillType);
						if (flag2)
						{
							state.InitVisibleSkill.Add(state.LifeSkillType);
						}
					}
					else
					{
						icon.SetSprite(string.Format("lifeskillcombat_prepare_icon_2_{0}", state.LifeSkillType), true, null);
					}
				}
				else
				{
					banSign.SetActive(true);
					icon.SetSprite(string.Format("lifeskillcombat_prepare_icon_3_{0}", state.LifeSkillType), true, null);
					refers.CGet<CImage>("BanImg").gameObject.SetActive(true);
				}
			}
			else
			{
				icon.SetSprite("lifeskillcombat_prepare_icon_1_16", true, null);
				refers.CGet<TextMeshProUGUI>("Name").text = LocalStringManager.Get(LanguageKey.UI_LifeSkillBattle_Prepare_Unknown);
			}
			PointerTrigger pointerTrigger = refers.GetComponent<PointerTrigger>();
			pointerTrigger.EnterEvent.RemoveAllListeners();
			pointerTrigger.EnterEvent.AddListener(delegate()
			{
				refers.CGet<GameObject>("Detecting").SetActive(!state.IsVisible);
				bool isVisible2 = state.IsVisible;
				if (isVisible2)
				{
					this.ShowLifeSkillAttainment(true, state.LifeSkillType);
					this.ShowLifeSkillAttainment(false, state.LifeSkillType);
				}
			});
			pointerTrigger.ExitEvent.RemoveAllListeners();
			pointerTrigger.ExitEvent.AddListener(delegate()
			{
				this.HideLifeSkillAttainment(true);
				this.HideLifeSkillAttainment(false);
			});
			if (isPlayerControlling)
			{
				base.CGet<GameObject>("GiveUpArea").SetActive(this._lifeSkillTypeStatus.Count((UI_LifeSkillCombatPrepare.LifeSkillTypeState l) => l.Interactable) == 1);
				bool interactable2 = button.interactable;
				if (interactable2)
				{
					button.ClearAndAddListener(delegate
					{
						this.CGet<GameObject>("GiveUpArea").SetActive(true);
						AudioManager.Instance.PlaySound("ui_art_prohibit", false, false);
						button.interactable = false;
						state.Interactable = false;
						sbyte index = sbyte.Parse(button.gameObject.name.Substring(14, (button.gameObject.name.Length > 15) ? 2 : 1));
						this.ControlParticle(true, (int)index);
						bool isTopThree = this.CheckTypeIsTopThree((sbyte)this._lifeSkillTypeIndicesForDisplay[(int)index], false);
						bool flag3 = isTopThree;
						if (flag3)
						{
							this.SetTalkInfo(2, 1);
						}
						bool flag4 = this._lifeSkillTypeStatus.Count((UI_LifeSkillCombatPrepare.LifeSkillTypeState l) => l.Interactable) == 1;
						if (flag4)
						{
							this.ControlParticle(false, 0);
							SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(1f, delegate
							{
								this.SetTalkInfo(2, 10);
							});
						}
					});
				}
			}
			else
			{
				base.CGet<GameObject>("GiveUpArea").SetActive(true);
			}
			i++;
		}
		base.CGet<TextMeshProUGUI>("WisdomNeedText").text = ((UI_LifeSkillCombatPrepare._wisdomSelf >= this._wisdomCost) ? string.Format("-{0}", this._wisdomCost) : string.Format("<color=#{0}>-{1}</color>", "brightred", this._wisdomCost).ColorReplace());
		if (isPlayerControlling)
		{
			this.RefreshPlayerButton();
		}
		else
		{
			base.CGet<GameObject>("GiveUpArea").SetActive(true);
			this.OperateAiTurn();
		}
	}

	// Token: 0x06002564 RID: 9572 RVA: 0x00112B84 File Offset: 0x00110D84
	private unsafe void ShowLifeSkillAttainment(bool isTaiwu, sbyte type)
	{
		Refers refers = isTaiwu ? this.PanelSelf : this.PanelAdversary;
		LifeSkillShorts lifeSkillAttainments = isTaiwu ? this._lifeSkillAttainmentsSelf : this._lifeSkillAttainmentsAdversary;
		refers.CGet<GameObject>("LifeSkillAttainmentBg").SetActive(true);
		refers.CGet<CImage>("LifeSkillIcon").SetSprite(string.Format("sp_14_iconjiyizhanshi_{0}", type), false, null);
		LifeSkillTypeItem config = Config.LifeSkillType.Instance[type];
		refers.CGet<TextMeshProUGUI>("LifeSkillName").text = config.Name;
		short attainment = *lifeSkillAttainments[(int)type];
		refers.CGet<TextMeshProUGUI>("LifeSkillAttainment").text = attainment.ToString();
	}

	// Token: 0x06002565 RID: 9573 RVA: 0x00112C30 File Offset: 0x00110E30
	private void HideLifeSkillAttainment(bool isTaiwu)
	{
		Refers refers = isTaiwu ? this.PanelSelf : this.PanelAdversary;
		refers.CGet<GameObject>("LifeSkillAttainmentBg").SetActive(false);
	}

	// Token: 0x06002566 RID: 9574 RVA: 0x00112C64 File Offset: 0x00110E64
	private void RefreshPlayerButton()
	{
		CButtonObsolete buttonObserve = base.CGet<CButtonObsolete>("Observe");
		CButtonObsolete buttonGiveup = base.CGet<CButtonObsolete>("Giveup");
		CButtonObsolete buttonBribery = base.CGet<CButtonObsolete>("Bribery");
		bool hasInvisibleCell = this._lifeSkillTypeStatus.Count((UI_LifeSkillCombatPrepare.LifeSkillTypeState l) => !l.IsVisible) >= 1;
		base.CGet<GameObject>("GiveUpArea").SetActive(this._lifeSkillTypeStatus.Count((UI_LifeSkillCombatPrepare.LifeSkillTypeState l) => l.Interactable) == 1);
		this.SetButtonInteractable(buttonObserve, UI_LifeSkillCombatPrepare._wisdomSelf >= this._wisdomCost & hasInvisibleCell);
		buttonObserve.GetComponent<DisableStyleRoot>().SetStyleEffect(!buttonObserve.interactable, false);
		buttonObserve.ClearAndAddListener(delegate
		{
			this.SetButtonInteractable(buttonObserve, false);
			this.PanelSelf.CGet<TextMeshProUGUI>("Tips").transform.parent.gameObject.SetActive(false);
			int i = 0;
			int count = this._lifeSkillTypeIndicesForDisplay.Length;
			while (i < count)
			{
				int stateIndex = this._lifeSkillTypeIndicesForDisplay[i];
				UI_LifeSkillCombatPrepare.LifeSkillTypeState state = this._lifeSkillTypeStatus[stateIndex];
				Transform cell = this.Cells.GetChild(i);
				CButtonObsolete button = cell.GetComponent<CButtonObsolete>();
				button.interactable = !state.IsVisible;
				button.ClearAndAddListener(delegate
				{
					AudioManager.Instance.PlaySound("ui_art_prove", false, false);
					UI_LifeSkillCombatPrepare._wisdomSelf -= this._wisdomCost;
					this._wisdomCost += ((this._wisdomCost < 3) ? 1 : 0);
					this.PanelSelf.CGet<TextMeshProUGUI>("WisdomValue").text = UI_LifeSkillCombatPrepare._wisdomSelf.ToString();
					state.Visible = true;
					this.SetTalkInfo(2, 2);
					SkeletonGraphic observeAni = cell.GetComponent<Refers>().CGet<SkeletonGraphic>("ObserveAni");
					observeAni.gameObject.SetActive(true);
					observeAni.AnimationState.SetAnimation(0, "animation", false);
					this.RefreshCells(true, false);
					this.ShowLifeSkillAttainment(true, state.LifeSkillType);
					this.ShowLifeSkillAttainment(false, state.LifeSkillType);
				});
				i++;
			}
		});
		this.SetButtonInteractable(buttonGiveup, hasInvisibleCell);
		buttonGiveup.ClearAndAddListener(delegate
		{
			this.PanelSelf.CGet<TextMeshProUGUI>("Tips").transform.parent.gameObject.SetActive(false);
			this.CGet<GameObject>("GiveUpArea").SetActive(true);
			this.SetTalkInfo(1, 3);
			this.SetButtonInteractable(buttonGiveup, false);
			this.TaiwuGiveUp();
		});
		this.SetButtonInteractable(buttonBribery, hasInvisibleCell && !this._hasForceSilent);
		Action <>9__6;
		buttonBribery.ClearAndAddListener(delegate
		{
			this.SetButtonInteractable(buttonBribery, false);
			UIElement eventWindow = UIElement.EventWindow;
			Delegate onHide = eventWindow.OnHide;
			Action b;
			if ((b = <>9__6) == null)
			{
				b = (<>9__6 = delegate()
				{
					this.SetButtonInteractable(buttonBribery, hasInvisibleCell && !this._hasForceSilent);
				});
			}
			eventWindow.OnHide = (Action)Delegate.Combine(onHide, b);
			TaiwuEventDomainMethod.Call.OnLifeSkillCombatForceSilent(this._charIdAdversary, 0, 0);
		});
	}

	// Token: 0x06002567 RID: 9575 RVA: 0x00112DE8 File Offset: 0x00110FE8
	private void TaiwuGiveUp()
	{
		this._isGivenUp = true;
		this._lifeSkillType = (from l in this._lifeSkillTypeStatus
		where l.Interactable
		select l.LifeSkillType into lt
		orderby this._lifeSkillAttainmentsAdversary.Get((int)lt) descending
		select lt).First<sbyte>();
		this._lifeSkillTypeStatus.First((UI_LifeSkillCombatPrepare.LifeSkillTypeState s) => s.LifeSkillType == this._lifeSkillType).Visible = true;
	}

	// Token: 0x06002568 RID: 9576 RVA: 0x00112E84 File Offset: 0x00111084
	private void OnLifeSkillCombatForceSilentResult(ArgumentBox argumentBox)
	{
		CButtonObsolete buttonBribery = base.CGet<CButtonObsolete>("Bribery");
		this.SetButtonInteractable(buttonBribery, false);
		this._hasForceSilent = true;
		sbyte type;
		argumentBox.Get("Type", out type);
		bool result;
		argumentBox.Get("Result", out result);
		sbyte behavior;
		argumentBox.Get("Behavior", out behavior);
		if (!true)
		{
		}
		short num;
		if (type != 1)
		{
			if (type != 2)
			{
				throw new ArgumentOutOfRangeException();
			}
			num = (result ? 5 : 7);
		}
		else
		{
			num = (result ? 4 : 6);
		}
		if (!true)
		{
		}
		short taiwuKey = num;
		int charIndex = result ? 1 : 2;
		this.SetTalkInfo((sbyte)charIndex, taiwuKey);
		bool flag = !result;
		if (!flag)
		{
			foreach (UI_LifeSkillCombatPrepare.LifeSkillTypeState lifeSkillTypeState in this._lifeSkillTypeStatus)
			{
				lifeSkillTypeState.Visible = true;
			}
			this.RefreshCells(true, false);
			this.PanelSelf.CGet<TextMeshProUGUI>("Tips").transform.parent.gameObject.SetActive(false);
			int i = 0;
			int count = this._lifeSkillTypeIndicesForDisplay.Length;
			while (i < count)
			{
				int stateIndex = this._lifeSkillTypeIndicesForDisplay[i];
				UI_LifeSkillCombatPrepare.LifeSkillTypeState state = this._lifeSkillTypeStatus[stateIndex];
				Transform cell = this.Cells.GetChild(i);
				CButtonObsolete button = cell.GetComponent<CButtonObsolete>();
				button.interactable = (state.Visible && state.Interactable);
				button.ClearAndAddListener(delegate
				{
					UIManager.Instance.StopCoroutine(this._MainCoroutine);
					this._MainCoroutine = null;
					this._lifeSkillType = state.LifeSkillType;
					this._forceSilentSucceed = true;
					this.HandleCallBack();
				});
				i++;
			}
		}
	}

	// Token: 0x06002569 RID: 9577 RVA: 0x00113030 File Offset: 0x00111230
	private void OperateAiTurn()
	{
		bool flag = this._lifeSkillTypeStatus.Count((UI_LifeSkillCombatPrepare.LifeSkillTypeState l) => l.Interactable) == 1;
		if (!flag)
		{
			bool isOver = false;
			while (!isOver)
			{
				sbyte type = this.CheckVisibleTypeContainsTopCanSelect();
				bool flag2 = type >= 0;
				if (flag2)
				{
					this.DoOperation((int)type);
					isOver = true;
				}
				else
				{
					int typeVisibleCell = this.CheckIsNotVisibleCell();
					bool flag3 = typeVisibleCell >= 0;
					if (flag3)
					{
						for (int i = 0; i < this._lifeSkillTypeIndicesForDisplay.Length; i++)
						{
							bool flag4 = this._lifeSkillTypeIndicesForDisplay[i] == typeVisibleCell;
							if (flag4)
							{
								AudioManager.Instance.PlaySound("ui_art_prove", false, false);
								UI_LifeSkillCombatPrepare._wisdomAdversary -= this._wisdomCostNpc;
								this._wisdomCostNpc++;
								this.PanelAdversary.CGet<TextMeshProUGUI>("WisdomValue").text = UI_LifeSkillCombatPrepare._wisdomAdversary.ToString();
								Refers cell = this.Cells.GetChild(i).GetComponent<Refers>();
								this.SetTalkInfo(1, 2);
								SkeletonGraphic observeAni = cell.CGet<SkeletonGraphic>("ObserveAni");
								observeAni.gameObject.SetActive(true);
								observeAni.AnimationState.SetAnimation(0, "animation", false);
								SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(0.5f, delegate
								{
									this.RefreshCells(false, false);
								});
								isOver = true;
								break;
							}
						}
					}
					else
					{
						this.DoOperation(-1);
						isOver = true;
					}
				}
			}
		}
	}

	// Token: 0x0600256A RID: 9578 RVA: 0x001131D4 File Offset: 0x001113D4
	private unsafe void DoOperation(int type)
	{
		sbyte banedType = -1;
		bool flag = type == -1;
		if (flag)
		{
			int count = (from l in this._lifeSkillTypeStatus
			where l.Interactable
			select l).Count((UI_LifeSkillCombatPrepare.LifeSkillTypeState l) => l.IsVisible);
			bool flag2 = count > 0;
			if (flag2)
			{
				banedType = (from l in this._lifeSkillTypeStatus
				where l.Interactable
				where l.IsVisible
				select l.LifeSkillType into lt
				orderby *(ref this._lifeSkillAttainmentsAdversary.Items.FixedElementField + (IntPtr)lt * 2)
				select lt).First<sbyte>();
			}
			else
			{
				using (IEnumerator<int> enumerator = this.GetVisibleIndices().GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						int index = enumerator.Current;
						banedType = (sbyte)index;
					}
				}
			}
		}
		else
		{
			banedType = (sbyte)type;
		}
		Func<UI_LifeSkillCombatPrepare.LifeSkillTypeState, bool> <>9__7;
		Action <>9__8;
		SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(Random.Range(0.5f, 1f), delegate
		{
			AudioManager.Instance.PlaySound("ui_art_prohibit", false, false);
			bool isTopThree = this.CheckTypeIsTopThree(banedType, true);
			bool flag3 = isTopThree;
			if (flag3)
			{
				this.SetTalkInfo(1, 1);
			}
			IEnumerable<UI_LifeSkillCombatPrepare.LifeSkillTypeState> lifeSkillTypeStatus = this._lifeSkillTypeStatus;
			Func<UI_LifeSkillCombatPrepare.LifeSkillTypeState, bool> predicate;
			if ((predicate = <>9__7) == null)
			{
				predicate = (<>9__7 = ((UI_LifeSkillCombatPrepare.LifeSkillTypeState s) => s.LifeSkillType == banedType));
			}
			lifeSkillTypeStatus.First(predicate).Interactable = false;
			for (int i = 0; i < this._lifeSkillTypeIndicesForDisplay.Length; i++)
			{
				bool flag4;
				if (this._lifeSkillTypeIndicesForDisplay[i] == (int)banedType)
				{
					flag4 = (this._lifeSkillTypeStatus.Count((UI_LifeSkillCombatPrepare.LifeSkillTypeState l) => l.Interactable) != 1);
				}
				else
				{
					flag4 = false;
				}
				bool flag5 = flag4;
				if (flag5)
				{
					this.ControlParticle(true, i);
					break;
				}
			}
			bool flag6 = this._lifeSkillTypeStatus.Count((UI_LifeSkillCombatPrepare.LifeSkillTypeState l) => l.Interactable) == 1;
			if (flag6)
			{
				this.ControlParticle(false, 0);
				YieldHelper instance = SingletonObject.getInstance<YieldHelper>();
				float sec = 1f;
				Action job;
				if ((job = <>9__8) == null)
				{
					job = (<>9__8 = delegate()
					{
						this.SetTalkInfo(1, 10);
					});
				}
				instance.DelaySecondsDo(sec, job);
			}
		});
	}

	// Token: 0x0600256B RID: 9579 RVA: 0x00113370 File Offset: 0x00111570
	private int CheckIsNotVisibleCell()
	{
		int count = this._lifeSkillTypeStatus.Count((UI_LifeSkillCombatPrepare.LifeSkillTypeState l) => !l.IsVisible);
		bool flag = count > 0 && UI_LifeSkillCombatPrepare._wisdomAdversary >= this._wisdomCostNpc;
		if (flag)
		{
			using (IEnumerator<int> enumerator = this.GetVisibleIndices().GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					int index = enumerator.Current;
					this._lifeSkillTypeStatus[index].Visible = true;
					return index;
				}
			}
		}
		return -1;
	}

	// Token: 0x0600256C RID: 9580 RVA: 0x0011341C File Offset: 0x0011161C
	private unsafe sbyte CheckVisibleTypeContainsTopCanSelect()
	{
		List<sbyte> visibleTypes = new List<sbyte>();
		foreach (UI_LifeSkillCombatPrepare.LifeSkillTypeState type in this._lifeSkillTypeStatus)
		{
			bool flag = type.Visible && type.Interactable;
			if (flag)
			{
				visibleTypes.Add(type.LifeSkillType);
			}
		}
		bool flag2 = visibleTypes.Count == 0;
		sbyte result;
		if (flag2)
		{
			result = -1;
		}
		else
		{
			sbyte nextStatus = (from l in this._lifeSkillTypeStatus
			where l.Interactable
			select l.LifeSkillType into lt
			orderby *(ref this._lifeSkillAttainmentsSelf.Items.FixedElementField + (IntPtr)lt * 2) descending
			select lt).First<sbyte>();
			bool flag3 = visibleTypes.Contains(nextStatus);
			if (flag3)
			{
				result = nextStatus;
			}
			else
			{
				result = -1;
			}
		}
		return result;
	}

	// Token: 0x0600256D RID: 9581 RVA: 0x00113510 File Offset: 0x00111710
	private bool CheckTypeIsTopThree(sbyte type, bool isTaiwu)
	{
		int range = 1;
		foreach (int index in this.GetDescendingOrderedIndices(isTaiwu ? this._lifeSkillAttainmentsSelf : this._lifeSkillAttainmentsAdversary))
		{
			bool flag = index == (int)type;
			if (flag)
			{
				return true;
			}
			range++;
			bool flag2 = range > 3;
			if (flag2)
			{
				break;
			}
		}
		return false;
	}

	// Token: 0x0600256E RID: 9582 RVA: 0x00113594 File Offset: 0x00111794
	private void HandleCallBack()
	{
		this._callBack(this.Result, this.FirstTurn);
		UIManager.Instance.HideUI(this.Element);
	}

	// Token: 0x0600256F RID: 9583 RVA: 0x001135C0 File Offset: 0x001117C0
	private void ControlParticle(bool isPlay, int index = 0)
	{
		if (isPlay)
		{
			Refers refers = this.Cells.GetChild(index).GetComponent<Refers>();
			refers.CGet<ParticleSystem>("Anim").gameObject.SetActive(true);
			refers.CGet<ParticleSystem>("Anim").Play(true);
		}
		else
		{
			for (int i = 0; i < this._lifeSkillTypeStatus.Length; i++)
			{
				Refers refers2 = this.Cells.GetChild(i).GetComponent<Refers>();
				refers2.CGet<ParticleSystem>("Anim").Pause(true);
				refers2.CGet<ParticleSystem>("Anim").gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x06002570 RID: 9584 RVA: 0x0011366C File Offset: 0x0011186C
	private void SetTalkInfo(sbyte isTaiwu, short talkTemplateId)
	{
		UI_LifeSkillCombatPrepare.<>c__DisplayClass51_0 CS$<>8__locals1;
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.config = LifeSkillCombatTalk.Instance[talkTemplateId];
		bool flag = CS$<>8__locals1.config == null;
		if (!flag)
		{
			switch (isTaiwu)
			{
			case 0:
				this.<SetTalkInfo>g__SameTimeBubbleShow|51_2(ref CS$<>8__locals1);
				break;
			case 1:
				this.<SetTalkInfo>g__TaiWuBubbleShow|51_0(ref CS$<>8__locals1);
				break;
			case 2:
				this.<SetTalkInfo>g__NpcBubbleShow|51_1(ref CS$<>8__locals1);
				break;
			}
		}
	}

	// Token: 0x06002571 RID: 9585 RVA: 0x001136E0 File Offset: 0x001118E0
	private void SetButtonInteractable(CButtonObsolete button, bool interactable)
	{
		button.interactable = interactable;
		button.GetComponent<DisableStyleRoot>().SetStyleEffect(!interactable, false);
	}

	// Token: 0x06002572 RID: 9586 RVA: 0x001136FC File Offset: 0x001118FC
	private void ShowAiBriberyDialog(BriberyData aiForceTaiwuSilentOperation)
	{
		bool flag = aiForceTaiwuSilentOperation == null;
		if (!flag)
		{
			ArgumentBox box = EasyPool.Get<ArgumentBox>();
			box.Set<CharacterDisplayData>("charData", this._characterAdversary);
			bool isItem = aiForceTaiwuSilentOperation.ItemDisplayData != null;
			bool isSecretInformation = aiForceTaiwuSilentOperation.SecretInformationDisplayPackage != null;
			bool isItem3 = isItem;
			if (isItem3)
			{
				box.Set("isItem", true);
				box.SetObject("itemDisplayData", aiForceTaiwuSilentOperation.ItemDisplayData);
			}
			else
			{
				bool isSecretInformation3 = isSecretInformation;
				if (isSecretInformation3)
				{
					box.SetObject("secretInformationDisplayData", aiForceTaiwuSilentOperation.SecretInformationDisplayPackage.SecretInformationDisplayDataList[0]);
					box.SetObject("secretInformationDisplayPackage", aiForceTaiwuSilentOperation.SecretInformationDisplayPackage);
				}
				else
				{
					box.Set("AiThreaten", true);
				}
			}
			box.SetObject("onYes", new Action(delegate
			{
				this.PanelSelf.CGet<TextMeshProUGUI>("Tips").transform.parent.gameObject.SetActive(false);
				this.CGet<GameObject>("GiveUpArea").SetActive(true);
				short key = 0;
				bool isItem2 = isItem;
				if (isItem2)
				{
					key = 5;
				}
				else
				{
					bool isSecretInformation2 = isSecretInformation;
					if (isSecretInformation2)
					{
						key = 4;
					}
				}
				this.SetTalkInfo(2, key);
				this.TaiwuGiveUp();
			}));
			box.SetObject("onNo", new Action(delegate
			{
				short key = 0;
				bool isItem2 = isItem;
				if (isItem2)
				{
					key = 7;
				}
				else
				{
					bool isSecretInformation2 = isSecretInformation;
					if (isSecretInformation2)
					{
						key = 6;
					}
				}
				this.SetTalkInfo(1, key);
			}));
			UIElement.AiForceSilenceDialog.SetOnInitArgs(box);
			UIManager.Instance.ShowUI(UIElement.AiForceSilenceDialog, true);
		}
	}

	// Token: 0x06002573 RID: 9587 RVA: 0x00113820 File Offset: 0x00111A20
	private sbyte GetIsFirstTurn()
	{
		bool isGivenUp = this._isGivenUp;
		sbyte result;
		if (isGivenUp)
		{
			result = -1;
		}
		else
		{
			bool forceSilentSucceed = this._forceSilentSucceed;
			if (forceSilentSucceed)
			{
				result = 1;
			}
			else
			{
				result = (sbyte)((UI_LifeSkillCombatPrepare._wisdomSelf > UI_LifeSkillCombatPrepare._wisdomAdversary) ? 1 : ((UI_LifeSkillCombatPrepare._wisdomSelf == UI_LifeSkillCombatPrepare._wisdomAdversary) ? this._isTaiwuFirstByLuck : -1));
			}
		}
		return result;
	}

	// Token: 0x170003DC RID: 988
	// (get) Token: 0x06002574 RID: 9588 RVA: 0x00113873 File Offset: 0x00111A73
	public bool IsFinishing
	{
		get
		{
			return this._lifeSkillType != -1;
		}
	}

	// Token: 0x170003DD RID: 989
	// (get) Token: 0x06002575 RID: 9589 RVA: 0x00113881 File Offset: 0x00111A81
	public sbyte Result
	{
		get
		{
			return (this._MainCoroutine != null) ? -1 : this._lifeSkillType;
		}
	}

	// Token: 0x170003DE RID: 990
	// (get) Token: 0x06002576 RID: 9590 RVA: 0x00113894 File Offset: 0x00111A94
	public sbyte FirstTurn
	{
		get
		{
			return this.GetIsFirstTurn();
		}
	}

	// Token: 0x06002586 RID: 9606 RVA: 0x00113B54 File Offset: 0x00111D54
	[CompilerGenerated]
	private void <SetTalkInfo>g__TaiWuBubbleShow|51_0(ref UI_LifeSkillCombatPrepare.<>c__DisplayClass51_0 A_1)
	{
		Bubble bubble = this.PanelSelf.CGet<Bubble>("LeftCloudBubble");
		bubble.SetText(this.<SetTalkInfo>g__GetTalkInfo|51_3(this._characterSelf.BehaviorType, ref A_1), true);
		this.PanelAdversary.CGet<Bubble>("RightCloudBubble").Hide();
	}

	// Token: 0x06002587 RID: 9607 RVA: 0x00113BA4 File Offset: 0x00111DA4
	[CompilerGenerated]
	private void <SetTalkInfo>g__NpcBubbleShow|51_1(ref UI_LifeSkillCombatPrepare.<>c__DisplayClass51_0 A_1)
	{
		Bubble bubble = this.PanelAdversary.CGet<Bubble>("RightCloudBubble");
		bubble.SetText(this.<SetTalkInfo>g__GetTalkInfo|51_3(this._characterAdversary.BehaviorType, ref A_1), true);
		this.PanelSelf.CGet<Bubble>("LeftCloudBubble").Hide();
	}

	// Token: 0x06002588 RID: 9608 RVA: 0x00113BF4 File Offset: 0x00111DF4
	[CompilerGenerated]
	private void <SetTalkInfo>g__SameTimeBubbleShow|51_2(ref UI_LifeSkillCombatPrepare.<>c__DisplayClass51_0 A_1)
	{
		Bubble taiWuBubble = this.PanelSelf.CGet<Bubble>("LeftCloudBubble");
		Bubble npcBubble = this.PanelAdversary.CGet<Bubble>("RightCloudBubble");
		taiWuBubble.SetText(this.<SetTalkInfo>g__GetTalkInfo|51_3(this._characterSelf.BehaviorType, ref A_1), true);
		npcBubble.SetText(this.<SetTalkInfo>g__GetTalkInfo|51_3(this._characterAdversary.BehaviorType, ref A_1), true);
	}

	// Token: 0x06002589 RID: 9609 RVA: 0x00113C58 File Offset: 0x00111E58
	[CompilerGenerated]
	private string <SetTalkInfo>g__GetTalkInfo|51_3(sbyte behaviorType, ref UI_LifeSkillCombatPrepare.<>c__DisplayClass51_0 A_2)
	{
		if (!true)
		{
		}
		string text;
		switch (behaviorType)
		{
		case 0:
			text = A_2.config.JustContent;
			break;
		case 1:
			text = A_2.config.KindContent;
			break;
		case 2:
			text = A_2.config.EvenContent;
			break;
		case 3:
			text = A_2.config.RebelContent;
			break;
		case 4:
			text = A_2.config.EgoisticContent;
			break;
		default:
			if (!true)
			{
			}
			<PrivateImplementationDetails>.ThrowSwitchExpressionException(behaviorType);
			break;
		}
		if (!true)
		{
		}
		string content = text;
		bool flag = content.IsNullOrEmpty();
		if (flag)
		{
			content = A_2.config.NormalContent;
		}
		return content;
	}

	// Token: 0x04001BCE RID: 7118
	public Refers PanelSelf;

	// Token: 0x04001BCF RID: 7119
	public Refers PanelAdversary;

	// Token: 0x04001BD0 RID: 7120
	public RectTransform Cells;

	// Token: 0x04001BD1 RID: 7121
	public GameObject LifeSkillTypeSelectionCellTemplate;

	// Token: 0x04001BD2 RID: 7122
	private Coroutine _MainCoroutine;

	// Token: 0x04001BD3 RID: 7123
	private readonly int[] _lifeSkillTypeIndicesForDisplay = Enumerable.Range(0, 16).ToArray<int>();

	// Token: 0x04001BD4 RID: 7124
	private sbyte _lifeSkillType;

	// Token: 0x04001BD5 RID: 7125
	private int _charIdSelf;

	// Token: 0x04001BD6 RID: 7126
	private int _charIdAdversary;

	// Token: 0x04001BD7 RID: 7127
	private CharacterDisplayData _characterSelf;

	// Token: 0x04001BD8 RID: 7128
	private CharacterDisplayData _characterAdversary;

	// Token: 0x04001BD9 RID: 7129
	private LifeSkillShorts _lifeSkillAttainmentsSelf;

	// Token: 0x04001BDA RID: 7130
	private LifeSkillShorts _lifeSkillAttainmentsAdversary;

	// Token: 0x04001BDB RID: 7131
	public static int _wisdomSelf;

	// Token: 0x04001BDC RID: 7132
	public static int _wisdomAdversary;

	// Token: 0x04001BDD RID: 7133
	private int _wisdomCost;

	// Token: 0x04001BDE RID: 7134
	private int _wisdomCostNpc;

	// Token: 0x04001BDF RID: 7135
	private int _isTaiwuFirstByLuck;

	// Token: 0x04001BE0 RID: 7136
	private Action<sbyte, sbyte> _callBack;

	// Token: 0x04001BE1 RID: 7137
	private bool _hasForceSilent;

	// Token: 0x04001BE2 RID: 7138
	private bool _isGivenUp;

	// Token: 0x04001BE3 RID: 7139
	private bool _forceSilentSucceed;

	// Token: 0x04001BE4 RID: 7140
	private readonly UI_LifeSkillCombatPrepare.LifeSkillTypeState[] _lifeSkillTypeStatus = new UI_LifeSkillCombatPrepare.LifeSkillTypeState[16];

	// Token: 0x02001546 RID: 5446
	private class LifeSkillTypeState
	{
		// Token: 0x1700166C RID: 5740
		// (get) Token: 0x0600CE44 RID: 52804 RVA: 0x0059E544 File Offset: 0x0059C744
		public bool IsVisible
		{
			get
			{
				return this.Visible || !this.Interactable;
			}
		}

		// Token: 0x0600CE45 RID: 52805 RVA: 0x0059E55A File Offset: 0x0059C75A
		public LifeSkillTypeState(sbyte lifeSkillType)
		{
			this.LifeSkillType = lifeSkillType;
			this.Visible = false;
			this.Interactable = true;
			this.InitVisibleSkill = new HashSet<sbyte>();
		}

		// Token: 0x0400A3FF RID: 41983
		public bool Visible;

		// Token: 0x0400A400 RID: 41984
		public bool Interactable;

		// Token: 0x0400A401 RID: 41985
		public readonly sbyte LifeSkillType;

		// Token: 0x0400A402 RID: 41986
		public HashSet<sbyte> InitVisibleSkill;
	}
}
