using System;
using System.Collections.Generic;
using Coffee.UIExtensions;
using Config;
using FrameWork;
using GameData.Domains.Character;
using GameData.Domains.CombatSkill;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Taiwu;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

// Token: 0x020001A0 RID: 416
public class ReadAndLoop : Refers
{
	// Token: 0x06001769 RID: 5993 RVA: 0x0008FD6F File Offset: 0x0008DF6F
	private void Awake()
	{
		GEvent.Add(EEvents.OnTaiwuCharIdChange, new GEvent.Callback(this.OnTaiwuCharIdChange));
	}

	// Token: 0x0600176A RID: 5994 RVA: 0x0008FD8B File Offset: 0x0008DF8B
	private void OnDestroy()
	{
		GEvent.Remove(EEvents.OnTaiwuCharIdChange, new GEvent.Callback(this.OnTaiwuCharIdChange));
	}

	// Token: 0x0600176B RID: 5995 RVA: 0x0008FDA7 File Offset: 0x0008DFA7
	public void Init(UIBase parent)
	{
		this._parent = parent;
		this.InitRefers();
		this._taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
		this.InitParticlePoolSrcObject();
	}

	// Token: 0x0600176C RID: 5996 RVA: 0x0008FDCF File Offset: 0x0008DFCF
	private void InitParticlePoolSrcObject()
	{
	}

	// Token: 0x0600176D RID: 5997 RVA: 0x0008FDD4 File Offset: 0x0008DFD4
	private void OnTaiwuCharIdChange(ArgumentBox argumentBox)
	{
		int newTaiwuCharId;
		argumentBox.Get("NewTaiwuCharId", out newTaiwuCharId);
		this._taiwuCharId = newTaiwuCharId;
	}

	// Token: 0x0600176E RID: 5998 RVA: 0x0008FDF8 File Offset: 0x0008DFF8
	public void InitRefers()
	{
		this._loopingBallList = base.CGetList<CImage>("LoopingBall");
		this._activeLoopButton = base.CGet<CButtonObsolete>("ActiveLoopButton");
		this._activeLoopStageProgressImage = base.CGet<CImage>("ActiveLoopStageProgressImage");
		this._activeLoopStateIcon = base.CGet<CImage>("ActiveLoopStateIcon");
		this._activeReadButton = base.CGet<CButtonObsolete>("ActiveReadButton");
		this._activeReadStageProgressImage = base.CGet<CImage>("ActiveReadStageProgressImage");
		this._activeReadStateIcon = base.CGet<CImage>("ActiveReadStateIcon");
		this._leftLoopResource = base.CGet<TextMeshProUGUI>("LeftLoopResource");
		this._leftReadResource = base.CGet<TextMeshProUGUI>("LeftReadResource");
		this._loopBallsTipDisplayer = base.CGet<TooltipInvoker>("LoopBallsTipDisplayer");
		this._loopEventParticle = base.CGet<UIParticle>("LoopEventParticle");
		this._loopNeigongSlot = base.CGet<CButtonObsolete>("LoopNeigongSlot");
		this._loopTipDisplayer = base.CGet<TooltipInvoker>("LoopTipDisplayer");
		this._readBallsTipDisplayer = base.CGet<TooltipInvoker>("ReadBallsTipDisplayer");
		this._readEventParticle = base.CGet<UIParticle>("ReadEventParticle");
		this._readTipDisplayer = base.CGet<TooltipInvoker>("ReadTipDisplayer");
		this._readingPages = base.CGet<CommonPageStates>("ReadingPages");
		this._readingSlot = base.CGet<CButtonObsolete>("ReadingSlot");
		TooltipInvoker tooltipInvoker = this._loopBallsTipDisplayer;
		if (tooltipInvoker.RuntimeParam == null)
		{
			tooltipInvoker.RuntimeParam = new ArgumentBox();
		}
		tooltipInvoker = this._readBallsTipDisplayer;
		if (tooltipInvoker.RuntimeParam == null)
		{
			tooltipInvoker.RuntimeParam = new ArgumentBox();
		}
	}

	// Token: 0x0600176F RID: 5999 RVA: 0x0008FF72 File Offset: 0x0008E172
	public void OnUpdate()
	{
		this.HandleActiveLoopButtonHold();
		this.HandleActiveReadButtonHold();
		this.UpdateReadingBookProgressTransition();
	}

	// Token: 0x06001770 RID: 6000 RVA: 0x0008FF8A File Offset: 0x0008E18A
	private void OnAttributeChanged()
	{
		this.RefreshActiveReadButton();
		this.RefreshActiveLoopButton();
	}

	// Token: 0x06001771 RID: 6001 RVA: 0x0008FF9C File Offset: 0x0008E19C
	public void RefreshLoopingNeigong(short loopingNeigong, short obtainedNeili, int taiwuCharId, Dictionary<short, CombatSkillDisplayData> combatSkillDisplayDataDict, List<short> loopingEventSkillIdList)
	{
		this.RefreshNeiliBall(obtainedNeili, combatSkillDisplayDataDict);
		this._loopBallsTipDisplayer.RuntimeParam.Set("ObtainedNeili", obtainedNeili);
		this._loopingEventSkillIdList = loopingEventSkillIdList;
		this._loopingNeigong = loopingNeigong;
		this.RefreshNeiliFiveElementBall();
		bool hasEvent = this.HasLoopingEvent();
		this.SetParticleActive(this._loopEventParticle, hasEvent);
		Refers refers = this._loopNeigongSlot.GetComponent<Refers>();
		GameObject finished = refers.CGet<GameObject>("Finished");
		CImage loopNeigongFrame = refers.CGet<CImage>("LoopNeigongFrame");
		CImage loopNeigongMask = refers.CGet<CImage>("LoopNeigongMask");
		GameObject addLoopNeigongIcon = refers.CGet<GameObject>("AddLoopNeigongIcon");
		CImage loopNeigongIcon = refers.CGet<CImage>("LoopNeigongIcon");
		bool isLoopingNeiGongExist = this._loopingNeigong >= 0;
		this._loopBallsTipDisplayer.RuntimeParam.Set("HasLoopingNeigong", isLoopingNeiGongExist);
		bool flag = isLoopingNeiGongExist && !combatSkillDisplayDataDict.ContainsKey(this._loopingNeigong);
		if (!flag)
		{
			this._loopTipDisplayer.Type = (isLoopingNeiGongExist ? TipType.CombatSkill : TipType.Simple);
			this._loopTipDisplayer.NeedRefresh = isLoopingNeiGongExist;
			TooltipInvoker loopTipDisplayer = this._loopTipDisplayer;
			if (loopTipDisplayer.RuntimeParam == null)
			{
				loopTipDisplayer.RuntimeParam = new ArgumentBox();
			}
			addLoopNeigongIcon.SetActive(!isLoopingNeiGongExist);
			loopNeigongFrame.gameObject.SetActive(isLoopingNeiGongExist);
			loopNeigongMask.gameObject.SetActive(isLoopingNeiGongExist);
			bool flag2 = isLoopingNeiGongExist;
			if (flag2)
			{
				CombatSkillItem configData = CombatSkill.Instance[this._loopingNeigong];
				short totalObtainableNeili = combatSkillDisplayDataDict[this._loopingNeigong].MaxObtainableNeili;
				bool progressEnabled = obtainedNeili >= 0;
				bool isFinished = obtainedNeili >= totalObtainableNeili;
				this.SwitchLoopProgress(isFinished);
				this.RefreshLoopNeiliProgress(obtainedNeili, totalObtainableNeili);
				this._loopTipDisplayer.RuntimeParam.Set("CombatSkillId", this._loopingNeigong);
				this._loopTipDisplayer.RuntimeParam.Set("CharId", taiwuCharId);
				loopNeigongIcon.SetSprite(configData.Icon, false, null);
				int fiveElementType = CommonCombatSkill.FiveElementTextureSuffix[(int)configData.FiveElements];
				loopNeigongFrame.SetSprite("ui_sp_base_combatskill_icon_{0}_{1}".GetFormat(configData.EquipType, fiveElementType), false, null);
				loopNeigongMask.SetSprite("ui_sp_base_combatskill_icon_{0}_mask".GetFormat(configData.EquipType), false, null);
				bool flag3 = progressEnabled;
				if (flag3)
				{
					this._loopBallsTipDisplayer.RuntimeParam.Set("MaxNeili", totalObtainableNeili);
					finished.SetActive(isFinished);
				}
				else
				{
					finished.SetActive(false);
				}
			}
			else
			{
				this.RefreshLoopNeiliProgress(0, 1);
				this._loopTipDisplayer.RuntimeParam.Set("arg0", LocalStringManager.Get(LanguageKey.LK_Neigong_Looping));
				this._loopTipDisplayer.RuntimeParam.Set("arg1", LocalStringManager.Get(LanguageKey.LK_Neigong_Looping_Tip_None));
				finished.SetActive(false);
			}
		}
	}

	// Token: 0x06001772 RID: 6002 RVA: 0x00090274 File Offset: 0x0008E474
	private bool HasLoopingEvent()
	{
		return this._loopingEventSkillIdList != null && this._loopingEventSkillIdList.Contains(this._loopingNeigong);
	}

	// Token: 0x06001773 RID: 6003 RVA: 0x000902A4 File Offset: 0x0008E4A4
	public void RefreshReading(IAsyncMethodRequestHandler asyncMethodRequestHandler, ItemKey currentReadingBookKey, ItemKey[] referenceBooks, List<int> readingEventBookList)
	{
		Refers readingSlotRefers = this._readingSlot.GetComponent<Refers>();
		CImage bookIcon = readingSlotRefers.CGet<CImage>("BookIcon");
		CImage addBookIcon = readingSlotRefers.CGet<CImage>("AddBookIcon");
		GameObject finished = readingSlotRefers.CGet<GameObject>("Finished");
		TooltipInvoker readTipDisplayer = this._readTipDisplayer;
		if (readTipDisplayer.RuntimeParam == null)
		{
			readTipDisplayer.RuntimeParam = new ArgumentBox();
		}
		this._currentReadingBookKey = currentReadingBookKey;
		SkillBookItem bookConfigData = this._currentReadingBookKey.IsValid() ? SkillBook.Instance.GetItem(this._currentReadingBookKey.TemplateId) : null;
		bool hasBook = this._currentReadingBookKey.IsValid() && bookConfigData != null;
		bookIcon.gameObject.SetActive(hasBook);
		addBookIcon.gameObject.SetActive(!hasBook);
		bool flag = hasBook;
		if (flag)
		{
			bookIcon.SetSprite(bookConfigData.Icon, false, null);
			this._readTipDisplayer.Type = TipType.ReadingBook;
			this._readTipDisplayer.RuntimeParam.SetObject("currentReadingBookKey", this._currentReadingBookKey);
			this._readTipDisplayer.RuntimeParam.SetObject("referenceBooks", referenceBooks);
			TaiwuDomainMethod.AsyncCall.GetTotalReadingProgress(asyncMethodRequestHandler, this._currentReadingBookKey.Id, delegate(int offset, RawDataPool dataPool)
			{
				sbyte progress = 0;
				Serializer.Deserialize(dataPool, offset, ref progress);
				finished.SetActive(progress >= 100);
			});
			this._readEventParticle.gameObject.SetActive(readingEventBookList.Contains(this._currentReadingBookKey.Id));
		}
		else
		{
			this._readTipDisplayer.Type = TipType.Simple;
			this._readTipDisplayer.RuntimeParam.Set("arg0", LocalStringManager.Get(LanguageKey.LK_Reading_Title));
			this._readTipDisplayer.RuntimeParam.Set("arg1", LocalStringManager.Get(LanguageKey.LK_Read_Tip_None));
			finished.SetActive(false);
			this._readEventParticle.gameObject.SetActive(false);
		}
	}

	// Token: 0x06001774 RID: 6004 RVA: 0x00090484 File Offset: 0x0008E684
	public void SetInteractable(bool forcedCanOpenCharacterMenu)
	{
		this._loopNeigongSlot.transform.Find("Locked").gameObject.SetActive(!forcedCanOpenCharacterMenu);
		this._loopNeigongSlot.interactable = forcedCanOpenCharacterMenu;
		this._loopNeigongSlot.GetComponent<PointerTrigger>().enabled = forcedCanOpenCharacterMenu;
		Refers readingSlotRefers = this._readingSlot.GetComponent<Refers>();
		GameObject locked = readingSlotRefers.CGet<GameObject>("Locked");
		locked.SetActive(!forcedCanOpenCharacterMenu);
		this._readingSlot.interactable = forcedCanOpenCharacterMenu;
		this._readingSlot.GetComponent<PointerTrigger>().enabled = forcedCanOpenCharacterMenu;
	}

	// Token: 0x06001775 RID: 6005 RVA: 0x00090518 File Offset: 0x0008E718
	private void RefreshReadingProgress()
	{
		this.SelectReadCircleParticle();
		this.RefreshReadStageIcon();
		bool flag = this._activeReadingProgress == 0;
		if (flag)
		{
			this._activeReadStageProgressImage.fillAmount = 0f;
		}
		bool isTen = this._activeReadingProgress % 10 == 0;
		bool crossStage = this._lastReadingProgress != this._activeReadingProgress && this._activeReadingProgress > 0 && isTen;
		this._activeReadStageProgressImage.fillAmount = this.GetReadAndLoopStageProgressFillAmount((int)(this._activeReadingProgress / 10));
		bool flag2 = crossStage;
		if (flag2)
		{
			SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(0.1f, delegate
			{
				bool flag3 = base.gameObject == null;
				if (!flag3)
				{
					AudioManager.Instance.PlaySound("SFX_reading_ui_liuguang_flash", false, false);
				}
			});
		}
		this._lastReadingProgress = this._activeReadingProgress;
	}

	// Token: 0x06001776 RID: 6006 RVA: 0x000905C8 File Offset: 0x0008E7C8
	private float GetReadAndLoopStageProgressFillAmount(int progressLevel)
	{
		bool flag = progressLevel < 0 || progressLevel > 3;
		float result;
		if (flag)
		{
			result = 0f;
		}
		else
		{
			result = (float)progressLevel / 3f;
		}
		return result;
	}

	// Token: 0x06001777 RID: 6007 RVA: 0x000905F9 File Offset: 0x0008E7F9
	private void SelectReadCircleParticle()
	{
	}

	// Token: 0x06001778 RID: 6008 RVA: 0x000905FC File Offset: 0x0008E7FC
	private void SelectLoopCircleParticle()
	{
	}

	// Token: 0x06001779 RID: 6009 RVA: 0x00090600 File Offset: 0x0008E800
	private void RefreshReadStageIcon()
	{
		this._activeReadStateIcon.SetSprite("ui_main_bottom_img_activestate_" + this.CurrentReadStageIndex.ToString(), false, null);
		TooltipInvoker displayer = this._activeReadStateIcon.GetComponent<TooltipInvoker>();
		TooltipInvoker tooltipInvoker = displayer;
		if (tooltipInvoker.RuntimeParam == null)
		{
			tooltipInvoker.RuntimeParam = new ArgumentBox();
		}
		displayer.Type = TipType.GeneralLines;
		displayer.RuntimeParam.Set("Title", LocalStringManager.Get(LanguageKey.LK_ReadingProgressStatus_Tips_Title));
		int lineCount = 0;
		displayer.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData
		{
			Type = 4,
			PreferredHeight = 16f
		});
		displayer.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData(3, new List<string>
		{
			"<color=#grey>" + LocalStringManager.Get(LanguageKey.LK_ReadingProgressStatus_Tips_Desc) + "</color>"
		}, null));
		displayer.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData
		{
			Type = 4,
			PreferredHeight = 20f
		});
		short efficiency = GlobalConfig.Instance.ActiveReadProgressAffectedEfficiency[this.CurrentReadStageIndex];
		int currentReadStageIndex = this.CurrentReadStageIndex;
		if (!true)
		{
		}
		string text;
		switch (currentReadStageIndex)
		{
		case 0:
			text = "brightblue";
			break;
		case 1:
			text = "pinkyellow";
			break;
		case 2:
			text = "brightred";
			break;
		default:
			text = "pinkyellow";
			break;
		}
		if (!true)
		{
		}
		string color = text;
		string efficiencyString = string.Format("<color=#{0}>{1}%</color>", color, efficiency).ColorReplace();
		displayer.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData(5, new List<string>
		{
			LocalStringManager.GetFormat(LanguageKey.LK_LoopingProgressStatus_Tips_Item1, efficiencyString)
		}, null));
		displayer.RuntimeParam.Set("LineCount", lineCount);
		displayer.Refresh(false, -1);
	}

	// Token: 0x0600177A RID: 6010 RVA: 0x0009080C File Offset: 0x0008EA0C
	private void RefreshLoopStageIcon()
	{
		this._activeLoopStateIcon.SetSprite("bottom_state_" + this.CurrentLoopStageIndex.ToString(), false, null);
		TooltipInvoker displayer = this._activeLoopStateIcon.GetComponent<TooltipInvoker>();
		TooltipInvoker tooltipInvoker = displayer;
		if (tooltipInvoker.RuntimeParam == null)
		{
			tooltipInvoker.RuntimeParam = new ArgumentBox();
		}
		displayer.Type = TipType.GeneralLines;
		displayer.RuntimeParam.Set("Title", LocalStringManager.Get(LanguageKey.LK_LoopingProgressStatus_Tips_Title));
		int lineCount = 0;
		displayer.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData
		{
			Type = 4,
			PreferredHeight = 16f
		});
		displayer.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData(3, new List<string>
		{
			"<color=#grey>" + LocalStringManager.Get(LanguageKey.LK_LoopingProgressStatus_Tips_Desc) + "</color>"
		}, null));
		displayer.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData
		{
			Type = 4,
			PreferredHeight = 20f
		});
		short efficiency = GlobalConfig.Instance.ActiveLoopProgressAffectedEfficiency[this.CurrentLoopStageIndex];
		int currentLoopStageIndex = this.CurrentLoopStageIndex;
		if (!true)
		{
		}
		string text;
		switch (currentLoopStageIndex)
		{
		case 0:
			text = "brightblue";
			break;
		case 1:
			text = "pinkyellow";
			break;
		case 2:
			text = "brightred";
			break;
		default:
			text = "pinkyellow";
			break;
		}
		if (!true)
		{
		}
		string color = text;
		string efficiencyString = string.Format("<color=#{0}>{1}%</color>", color, efficiency).ColorReplace();
		displayer.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData(5, new List<string>
		{
			LocalStringManager.GetFormat(LanguageKey.LK_LoopingProgressStatus_Tips_Item1, efficiencyString)
		}, null));
		displayer.RuntimeParam.Set("LineCount", lineCount);
		displayer.Refresh(false, -1);
	}

	// Token: 0x0600177B RID: 6011 RVA: 0x00090A18 File Offset: 0x0008EC18
	public void RefreshLoopingProgress()
	{
		this.SelectLoopCircleParticle();
		this.RefreshLoopStageIcon();
		bool flag = this._activeLoopingProgress == 0;
		if (flag)
		{
			this._activeLoopStageProgressImage.fillAmount = 0f;
		}
		bool isTen = this._activeLoopingProgress % 10 == 0;
		bool crossStage = this._lastLoopingProgress != this._activeLoopingProgress && this._activeLoopingProgress > 0 && isTen;
		this._activeLoopStageProgressImage.fillAmount = this.GetReadAndLoopStageProgressFillAmount((int)(this._activeLoopingProgress / 10));
		bool flag2 = crossStage;
		if (flag2)
		{
			SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(0.1f, delegate
			{
				bool flag3 = base.gameObject == null;
				if (!flag3)
				{
					AudioManager.Instance.PlaySound("SFX_reading_ui_liuguang_flash", false, false);
				}
			});
		}
		this._lastLoopingProgress = this._activeLoopingProgress;
	}

	// Token: 0x0600177C RID: 6012 RVA: 0x00090AC8 File Offset: 0x0008ECC8
	public void RefreshActiveReadButton()
	{
		bool canActiveRead = this.CanActiveRead();
		this._activeReadButton.interactable = canActiveRead;
		this._activeReadButton.GetComponent<PointerTrigger>().enabled = canActiveRead;
		this.UpdateActiveReadTipsData();
		this._leftReadResource.text = this._taiwuMainAttributes[5].ToString();
		this._lockActiveReadWhenWaitingDataUpdate = false;
	}

	// Token: 0x0600177D RID: 6013 RVA: 0x00090B28 File Offset: 0x0008ED28
	public void RefreshActiveLoopButton()
	{
		bool canActiveLoop = this.CanActiveLoop();
		this._activeLoopButton.interactable = canActiveLoop;
		this._activeLoopButton.GetComponent<PointerTrigger>().enabled = canActiveLoop;
		this.UpdateActiveLoopTipsData();
		this._leftLoopResource.text = this._taiwuMainAttributes[2].ToString();
		this._lockActiveLoopWhenWaitingDataUpdate = false;
	}

	// Token: 0x0600177E RID: 6014 RVA: 0x00090B88 File Offset: 0x0008ED88
	private void HandleActiveLoopButtonHold()
	{
		float now = Time.realtimeSinceStartup;
		bool flag = !this._isActiveLoopPointerDown;
		if (flag)
		{
			this.TryStopActiveLoopHoldSound(now);
		}
		else
		{
			bool canLoop = this.CanActiveLoop();
			bool flag2 = !canLoop;
			if (flag2)
			{
				this.TryStopActiveLoopHoldSound(now);
				this.PlayerActiveReadOrLoopReverseSound();
				this.HideLoopEffects();
				this._isActiveLoopPointerDown = false;
			}
			else
			{
				bool flag3 = (double)(now - this._activeLoopPointerDownTime) > 0.2;
				if (flag3)
				{
					bool isCurrentLoopStageIndexValid = this.IsCurrentLoopStageIndexValid;
					if (isCurrentLoopStageIndexValid)
					{
					}
				}
				bool flag4 = (double)(now - this._activeLoopPointerDownTime) > 0.2;
				if (flag4)
				{
					this.DisableAndHideLoopTips();
					this.TryStartActiveLoopHoldSound();
					bool flag5 = (double)(now - this._activeLoopLastTriggerTime) > 0.1;
					if (flag5)
					{
						this._activeLoopLastTriggerTime = now;
						bool lockActiveLoopWhenWaitingDataUpdate = this._lockActiveLoopWhenWaitingDataUpdate;
						if (!lockActiveLoopWhenWaitingDataUpdate)
						{
							this._lockActiveLoopWhenWaitingDataUpdate = true;
							this.ActiveLoopOnce();
							this.PlayActiveReadOrLoopOnceSound(Time.realtimeSinceStartup - this._activeLoopPointerDownTime);
						}
					}
				}
			}
		}
	}

	// Token: 0x0600177F RID: 6015 RVA: 0x00090C90 File Offset: 0x0008EE90
	private void HandleActiveReadButtonHold()
	{
		float now = Time.realtimeSinceStartup;
		bool flag = !this._isActiveReadPointerDown;
		if (flag)
		{
			this.TryStopActiveReadHoldSound(now);
		}
		else
		{
			bool canRead = this.CanActiveRead();
			bool flag2 = !canRead;
			if (flag2)
			{
				this.TryStopActiveReadHoldSound(now);
				this.PlayerActiveReadOrLoopReverseSound();
				this.HideReadEffects();
				this._isActiveReadPointerDown = false;
			}
			else
			{
				bool flag3 = (double)(now - this._activeReadPointerDownTime) > 0.2;
				if (flag3)
				{
					bool isCurrentReadStageIndexValid = this.IsCurrentReadStageIndexValid;
					if (isCurrentReadStageIndexValid)
					{
					}
				}
				bool flag4 = (double)(now - this._activeReadPointerDownTime) > 0.2;
				if (flag4)
				{
					this.TryStartActiveReadHoldSound();
					this.DisableAndHideReadTips();
					bool flag5 = (double)(now - this._activeReadLastTriggerTime) > 0.1;
					if (flag5)
					{
						this._activeReadLastTriggerTime = now;
						bool lockActiveReadWhenWaitingDataUpdate = this._lockActiveReadWhenWaitingDataUpdate;
						if (!lockActiveReadWhenWaitingDataUpdate)
						{
							this._lockActiveReadWhenWaitingDataUpdate = true;
							this.ActiveReadOnce();
							this.PlayActiveReadOrLoopOnceSound(Time.realtimeSinceStartup - this._activeReadPointerDownTime);
						}
					}
				}
			}
		}
	}

	// Token: 0x06001780 RID: 6016 RVA: 0x00090D98 File Offset: 0x0008EF98
	private float GetLoopSincePointerUpDuration()
	{
		return Time.realtimeSinceStartup - this._activeLoopPointerUpTime;
	}

	// Token: 0x06001781 RID: 6017 RVA: 0x00090DB8 File Offset: 0x0008EFB8
	private float GetReadSincePointerUpDuration()
	{
		return Time.realtimeSinceStartup - this._activeReadPointerUpTime;
	}

	// Token: 0x06001782 RID: 6018 RVA: 0x00090DD8 File Offset: 0x0008EFD8
	private void TryStartActiveReadOrLoopHoldSound(ref bool playingFlag, Func<float> sincePointerUpDurationGetter)
	{
		bool flag = playingFlag;
		if (!flag)
		{
			AudioCommand cmd = new AudioCommand
			{
				AudioType = SEType.Sound,
				Loop = true,
				AudioName = "SFX_reading_ui_liuguang_loop",
				OnPlayUpdate = delegate(AudioCommandOnPlayeUpdateParam param)
				{
					this.OnReadOrLoopHoldSoundUpdate(param, sincePointerUpDurationGetter);
				}
			};
			AudioManager.Instance.Play(cmd);
			playingFlag = true;
		}
	}

	// Token: 0x06001783 RID: 6019 RVA: 0x00090E44 File Offset: 0x0008F044
	private void TryStopActiveReadOrLoopHoldSound(float now, float pointerUpTime, ref bool flag)
	{
		bool flag2 = now - pointerUpTime > 0.2f & flag;
		if (flag2)
		{
			AudioManager.Instance.StopSound("SFX_reading_ui_liuguang_loop");
			flag = false;
		}
	}

	// Token: 0x06001784 RID: 6020 RVA: 0x00090E78 File Offset: 0x0008F078
	public void OnReadingProgressUpdate(short activeReadingProgress)
	{
		this._activeReadingProgress = activeReadingProgress;
		this.RefreshReadingProgress();
		this.RefreshActiveReadButton();
	}

	// Token: 0x06001785 RID: 6021 RVA: 0x00090E90 File Offset: 0x0008F090
	public void OnLoopingProgressUpdate(short activeLoopingProgress)
	{
		this._activeLoopingProgress = activeLoopingProgress;
		this.RefreshLoopingProgress();
		this.RefreshActiveLoopButton();
	}

	// Token: 0x06001786 RID: 6022 RVA: 0x00090EA8 File Offset: 0x0008F0A8
	private void TryStopActiveReadHoldSound(float now)
	{
		this.TryStopActiveReadOrLoopHoldSound(now, this._activeReadPointerUpTime, ref this._isActiveReadingHoldSoundPlaying);
	}

	// Token: 0x06001787 RID: 6023 RVA: 0x00090EBF File Offset: 0x0008F0BF
	private void TryStopActiveLoopHoldSound(float now)
	{
		this.TryStopActiveReadOrLoopHoldSound(now, this._activeLoopPointerUpTime, ref this._isActiveLoopHoldSoundPlaying);
	}

	// Token: 0x06001788 RID: 6024 RVA: 0x00090ED6 File Offset: 0x0008F0D6
	private void TryStartActiveReadHoldSound()
	{
		this.TryStartActiveReadOrLoopHoldSound(ref this._isActiveReadingHoldSoundPlaying, new Func<float>(this.GetReadSincePointerUpDuration));
	}

	// Token: 0x06001789 RID: 6025 RVA: 0x00090EF2 File Offset: 0x0008F0F2
	private void TryStartActiveLoopHoldSound()
	{
		this.TryStartActiveReadOrLoopHoldSound(ref this._isActiveLoopHoldSoundPlaying, new Func<float>(this.GetLoopSincePointerUpDuration));
	}

	// Token: 0x0600178A RID: 6026 RVA: 0x00090F10 File Offset: 0x0008F110
	private void PlayActiveReadOrLoopOnceSound(float pointerDownTime)
	{
		float pitch = Mathf.Min(1.5f, 1f + pointerDownTime * 0.5f);
		AudioCommand cmd = new AudioCommand
		{
			AudioType = SEType.Sound,
			Loop = false,
			AudioName = "SFX_reading_ui_liuguang_click",
			Pitch = pitch
		};
		AudioManager.Instance.Play(cmd);
	}

	// Token: 0x0600178B RID: 6027 RVA: 0x00090F68 File Offset: 0x0008F168
	private void PlayerActiveReadOrLoopReverseSound()
	{
		AudioCommand cmd = new AudioCommand
		{
			AudioType = SEType.Sound,
			Loop = false,
			AudioName = "SFX_reading_ui_liuguang_end",
			Pitch = 1f
		};
		AudioManager.Instance.Play(cmd);
	}

	// Token: 0x0600178C RID: 6028 RVA: 0x00090FAC File Offset: 0x0008F1AC
	private void OnReadOrLoopHoldSoundUpdate(AudioCommandOnPlayeUpdateParam param, Func<float> sincePointerUpDurationGetter)
	{
		AudioSource source = param.player;
		bool flag = this._isActiveLoopPointerDown || this._isActiveReadPointerDown;
		if (flag)
		{
			float pitch = Mathf.Min(1.5f, 1f + param.eclapsedTime * 0.5f);
			float volume = Mathf.Min(1f, param.eclapsedTime);
			source.pitch = pitch;
			source.volume = volume;
		}
		else
		{
			float sincePointerUpDuration = sincePointerUpDurationGetter();
			float volume2 = Mathf.Max(0f, 1f - sincePointerUpDuration / 0.2f);
			source.volume = volume2;
		}
	}

	// Token: 0x0600178D RID: 6029 RVA: 0x00091048 File Offset: 0x0008F248
	public void OnActiveReadPointerDown()
	{
		bool flag = !this.CanActiveRead();
		if (!flag)
		{
			this._activeReadPointerDownTime = Time.realtimeSinceStartup;
			bool lockActiveReadWhenWaitingDataUpdate = this._lockActiveReadWhenWaitingDataUpdate;
			if (!lockActiveReadWhenWaitingDataUpdate)
			{
				this._lockActiveReadWhenWaitingDataUpdate = true;
				this.ActiveReadOnce();
				this.PlayActiveReadOrLoopOnceSound(0f);
				this._isActiveReadPointerDown = true;
			}
		}
	}

	// Token: 0x0600178E RID: 6030 RVA: 0x000910A0 File Offset: 0x0008F2A0
	public void OnActiveLoopPointerDown()
	{
		bool flag = !this.CanActiveLoop();
		if (!flag)
		{
			this._activeLoopPointerDownTime = Time.realtimeSinceStartup;
			bool lockActiveLoopWhenWaitingDataUpdate = this._lockActiveLoopWhenWaitingDataUpdate;
			if (!lockActiveLoopWhenWaitingDataUpdate)
			{
				this._lockActiveLoopWhenWaitingDataUpdate = true;
				this.ActiveLoopOnce();
				this.PlayActiveReadOrLoopOnceSound(0f);
				this._isActiveLoopPointerDown = true;
			}
		}
	}

	// Token: 0x0600178F RID: 6031 RVA: 0x000910F8 File Offset: 0x0008F2F8
	public void OnActiveReadPointerUp()
	{
		this._activeReadPointerUpTime = Time.realtimeSinceStartup;
		this.HideReadEffects();
		bool flag = this._isActiveReadPointerDown && this._activeReadPointerUpTime - this._activeReadPointerDownTime > 0.3f;
		if (flag)
		{
			this.PlayerActiveReadOrLoopReverseSound();
		}
		this._isActiveReadPointerDown = false;
		this.RecoverReadTips();
	}

	// Token: 0x06001790 RID: 6032 RVA: 0x00091154 File Offset: 0x0008F354
	public void OnActiveLoopPointerUp()
	{
		this._activeLoopPointerUpTime = Time.realtimeSinceStartup;
		this.HideLoopEffects();
		bool flag = this._isActiveLoopPointerDown && this._activeLoopPointerUpTime - this._activeLoopPointerDownTime > 0.3f;
		if (flag)
		{
			this.PlayerActiveReadOrLoopReverseSound();
		}
		this._isActiveLoopPointerDown = false;
		this.RecoverLoopTips();
	}

	// Token: 0x06001791 RID: 6033 RVA: 0x000911AE File Offset: 0x0008F3AE
	private void SetParticleActive(UIParticle particle, bool isOn)
	{
		particle.gameObject.SetActive(isOn);
	}

	// Token: 0x06001792 RID: 6034 RVA: 0x000911BE File Offset: 0x0008F3BE
	private void HideLoopEffects()
	{
	}

	// Token: 0x06001793 RID: 6035 RVA: 0x000911C1 File Offset: 0x0008F3C1
	private void HideReadEffects()
	{
	}

	// Token: 0x06001794 RID: 6036 RVA: 0x000911C4 File Offset: 0x0008F3C4
	private void RefreshNeiliBall(short obtainedNeili, Dictionary<short, CombatSkillDisplayData> combatSkillDisplayDataDict)
	{
		bool flag = combatSkillDisplayDataDict == null;
		if (!flag)
		{
			bool flag2 = combatSkillDisplayDataDict.Count == 0;
			if (!flag2)
			{
				bool flag3 = this._loopingNeigong == -1;
				if (flag3)
				{
					this._loopingBallList[0].SetSprite("bottom_point_base", false, null);
				}
				else
				{
					CombatSkillDisplayData combatSkillDisplayData;
					bool flag4 = !combatSkillDisplayDataDict.TryGetValue(this._loopingNeigong, out combatSkillDisplayData);
					if (flag4)
					{
						this._loopingBallList[0].SetSprite("bottom_point_base", false, null);
					}
					else
					{
						short totalObtainableNeili = combatSkillDisplayData.MaxObtainableNeili;
						bool flag5 = obtainedNeili < totalObtainableNeili;
						if (flag5)
						{
							this._loopingBallList[0].SetSprite("bottom_point_base", false, null);
						}
						else
						{
							this._loopingBallList[0].SetSprite("bottom_point_0_0", false, null);
						}
					}
				}
			}
		}
	}

	// Token: 0x06001795 RID: 6037 RVA: 0x00091295 File Offset: 0x0008F495
	public void RefreshNeiliType(sbyte neiliType)
	{
		this._cachedNeiliType = neiliType;
		this._loopBallsTipDisplayer.RuntimeParam.Set("NeiliType", neiliType);
		this.RefreshNeiliFiveElementBall();
	}

	// Token: 0x06001796 RID: 6038 RVA: 0x000912BD File Offset: 0x0008F4BD
	public void RefreshMainAttribute(MainAttributes taiwuMainAttributes)
	{
		this._taiwuMainAttributes = taiwuMainAttributes;
		this.OnAttributeChanged();
	}

	// Token: 0x06001797 RID: 6039 RVA: 0x000912D0 File Offset: 0x0008F4D0
	public void RefreshNeiliFiveElementBall()
	{
		CImage targetBall = this._loopingBallList[1];
		bool flag = this._loopingNeigong == -1;
		if (flag)
		{
			this._loopBallsTipDisplayer.RuntimeParam.Set("FiveElementOk", false);
			targetBall.SetSprite("bottom_point_base", false, null);
		}
		else
		{
			NeiliTypeItem typeConfig = NeiliType.Instance[this._cachedNeiliType];
			this._loopBallsTipDisplayer.RuntimeParam.SetObject("NeiliTypeConfig", typeConfig);
			bool flag2 = typeConfig.ColorType == 2;
			if (flag2)
			{
				this._loopBallsTipDisplayer.RuntimeParam.Set("FiveElementOk", false);
				targetBall.SetSprite("bottom_point_base", false, null);
			}
			else
			{
				CombatSkillDomainMethod.AsyncCall.GetCombatSkillDisplayDataOnce(this._parent, this._taiwuCharId, this._loopingNeigong, delegate(int offset, RawDataPool pool)
				{
					CombatSkillDisplayData combatSkillDisplayData = null;
					Serializer.Deserialize(pool, offset, ref combatSkillDisplayData);
					bool flag3 = combatSkillDisplayData.FiveElementDestTypeWhileLooping == -1 || combatSkillDisplayData.FiveElementDestTypeWhileLooping == (sbyte)typeConfig.FiveElements;
					if (flag3)
					{
						this._loopBallsTipDisplayer.RuntimeParam.Set("FiveElementOk", true);
						targetBall.SetSprite(string.Format("bottom_point_1_{0}", typeConfig.FiveElements), false, null);
					}
					else
					{
						this._loopBallsTipDisplayer.RuntimeParam.Set("FiveElementOk", false);
						targetBall.SetSprite("bottom_point_base", false, null);
					}
				});
			}
		}
	}

	// Token: 0x06001798 RID: 6040 RVA: 0x000913CF File Offset: 0x0008F5CF
	public void RefreshTaiwuExtraNeiliAllocationProgress(int[] taiwuExtraNeiliAllocationProgress)
	{
		this._taiwuExtraNeiliAllocationProgress = taiwuExtraNeiliAllocationProgress;
		this._loopBallsTipDisplayer.RuntimeParam.SetObject("TaiwuExtraNeiliAllocationProgress", taiwuExtraNeiliAllocationProgress);
		this.RefreshExtraNeiliAllocationBalls();
		this.RefreshLoopExtraNeiliAllocationProgress(taiwuExtraNeiliAllocationProgress);
	}

	// Token: 0x06001799 RID: 6041 RVA: 0x00091400 File Offset: 0x0008F600
	private void SwitchLoopProgress(bool isFinished)
	{
		Refers refers = this._loopNeigongSlot.GetComponent<Refers>();
		GameObject extraNeiliAllocationProgressLayout = refers.CGet<GameObject>("ExtraNeiliAllocationProgressLayout");
		Refers neiliProgress = refers.CGet<Refers>("NeiliProgress");
		extraNeiliAllocationProgressLayout.SetActive(isFinished);
		neiliProgress.gameObject.SetActive(!isFinished);
	}

	// Token: 0x0600179A RID: 6042 RVA: 0x0009144C File Offset: 0x0008F64C
	private void RefreshLoopNeiliProgress(short obtainedNeili, short totalObtainableNeili)
	{
		Refers refers = this._loopNeigongSlot.GetComponent<Refers>();
		Refers neiliProgress = refers.CGet<Refers>("NeiliProgress");
		CImage progressImage = neiliProgress.CGet<CImage>("ProgressImage");
		CImage progressCurrent = neiliProgress.CGet<CImage>("ProgressCurrent");
		float fillAmount = (float)obtainedNeili / (float)totalObtainableNeili;
		progressImage.fillAmount = fillAmount;
		progressCurrent.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, progressCurrent.transform.parent.GetComponent<RectTransform>().rect.height * fillAmount);
	}

	// Token: 0x0600179B RID: 6043 RVA: 0x000914D4 File Offset: 0x0008F6D4
	private void RefreshLoopExtraNeiliAllocationProgress(int[] taiwuExtraNeiliAllocationProgress)
	{
		Refers refers = this._loopNeigongSlot.GetComponent<Refers>();
		List<Refers> extraNeiliAllocationProgressList = refers.CGetList<Refers>("ExtraNeiliAllocationProgress_");
		refers.CGet<GameObject>("ExtraNeiliAllocationProgressLayout");
		for (int i = 0; i < 4; i++)
		{
			Refers progressRefers = extraNeiliAllocationProgressList[i];
			CImage progress = progressRefers.CGet<CImage>("Progress");
			CImage progressCurrent = progressRefers.CGet<CImage>("ProgressCurrent");
			int extraNeiliAllocation = LoopingCommonUtils.CalcExtraNeiliAllocationFromProgress(taiwuExtraNeiliAllocationProgress[i]);
			float fillAmount = (float)extraNeiliAllocation / (float)GlobalConfig.Instance.MaxExtraNeiliAllocation;
			progress.fillAmount = fillAmount;
			progressCurrent.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, progressCurrent.transform.parent.GetComponent<RectTransform>().rect.height * fillAmount);
		}
	}

	// Token: 0x0600179C RID: 6044 RVA: 0x000915A0 File Offset: 0x0008F7A0
	private void RefreshExtraNeiliAllocationBalls()
	{
		int maxExtraNeiliAllocationProgress = LoopingCommonUtils.GetNeiliAllocationMaxProgress();
		for (int i = 0; i < 4; i++)
		{
			bool isMax = this._taiwuExtraNeiliAllocationProgress[i] >= maxExtraNeiliAllocationProgress;
			this._loopingBallList[i + 2].SetSprite(isMax ? string.Format("bottom_point_2_{0}", i) : "bottom_point_base", false, null);
		}
	}

	// Token: 0x0600179D RID: 6045 RVA: 0x00091608 File Offset: 0x0008F808
	public void RefreshReadingBookPagesInfoBalls(ItemKey currentReadingBookKey, SkillBookPageDisplayData pagesInfo)
	{
		this._readBallsTipDisplayer.RuntimeParam.SetObject("ReadingBook", currentReadingBookKey);
		this._readBallsTipDisplayer.RuntimeParam.Set("HasReadingBook", currentReadingBookKey.IsValid());
		this._readBallsTipDisplayer.RuntimeParam.SetObject("PageInfo", pagesInfo);
		this._readBallsTipDisplayer.enabled = currentReadingBookKey.IsValid();
		this._readingPages.Refresh(currentReadingBookKey, pagesInfo);
	}

	// Token: 0x0600179E RID: 6046 RVA: 0x00091688 File Offset: 0x0008F888
	private unsafe void UpdateActiveReadTipsData()
	{
		TooltipInvoker tipDisplayer = this._activeReadButton.GetComponent<TooltipInvoker>();
		tipDisplayer.Type = TipType.ActiveRead;
		tipDisplayer.NeedRefresh = true;
		TooltipInvoker tooltipInvoker = tipDisplayer;
		if (tooltipInvoker.RuntimeParam == null)
		{
			tooltipInvoker.RuntimeParam = new ArgumentBox();
		}
		tipDisplayer.RuntimeParam.Set("Title", LocalStringManager.Get(LanguageKey.LK_ActiveRead_Tip_Title));
		short haveIntelligence = *this._taiwuMainAttributes[5];
		short costIntelligence = GlobalConfig.Instance.ActiveReadingAttributeCost;
		bool intelligenceEnough = haveIntelligence >= costIntelligence;
		string intColor = intelligenceEnough ? "brightblue" : "brightred";
		string cost = LocalStringManager.GetFormat(LanguageKey.LK_ActiveRead_Tip_Cost, string.Format("<color=#{0}>{1}</color>", intColor, haveIntelligence), costIntelligence);
		int haveTime = SingletonObject.getInstance<TimeManager>().GetRemainingActionPointConvertToDays();
		bool timeEnough = haveTime >= (int)GlobalConfig.Instance.ActiveReadingTimeCost;
		string timeColor = timeEnough ? "brightblue" : "brightred";
		bool isMax = this._activeReadingProgress >= GlobalConfig.Instance.MaxActiveReadingProgress;
		tipDisplayer.RuntimeParam.Set("Cost1", cost);
		string cost2 = LocalStringManager.GetFormat(LanguageKey.LK_Active_Tip_TimeCost, string.Format("<color=#{0}>{1}</color>", timeColor, haveTime), GlobalConfig.Instance.ActiveReadingTimeCost);
		tipDisplayer.RuntimeParam.Set("Cost2", cost2);
		tipDisplayer.RuntimeParam.Set("ShowCost6", !intelligenceEnough);
		tipDisplayer.RuntimeParam.Set("ShowCost7", !timeEnough);
		tipDisplayer.RuntimeParam.Set("ShowCost8", !this._currentReadingBookKey.IsValid());
		tipDisplayer.RuntimeParam.Set("ShowCost10", isMax);
		tipDisplayer.Refresh(false, -1);
	}

	// Token: 0x0600179F RID: 6047 RVA: 0x00091840 File Offset: 0x0008FA40
	private void DisableAndHideReadTips()
	{
		TooltipInvoker tipDisplayer = this._activeReadButton.GetComponent<TooltipInvoker>();
		tipDisplayer.HideTips();
		tipDisplayer.enabled = false;
	}

	// Token: 0x060017A0 RID: 6048 RVA: 0x0009186C File Offset: 0x0008FA6C
	private void RecoverReadTips()
	{
		bool flag = WorldMapModel.Traveling || UIElement.PartWorld.Exist;
		if (!flag)
		{
			TooltipInvoker tipDisplayer = this._activeReadButton.GetComponent<TooltipInvoker>();
			tipDisplayer.enabled = true;
			this.UpdateActiveReadTipsData();
			tipDisplayer.ShowTips();
		}
	}

	// Token: 0x060017A1 RID: 6049 RVA: 0x000918B8 File Offset: 0x0008FAB8
	private void DisableAndHideLoopTips()
	{
		TooltipInvoker tipDisplayer = this._activeLoopButton.GetComponent<TooltipInvoker>();
		tipDisplayer.HideTips();
		tipDisplayer.enabled = false;
	}

	// Token: 0x060017A2 RID: 6050 RVA: 0x000918E4 File Offset: 0x0008FAE4
	private void RecoverLoopTips()
	{
		bool flag = WorldMapModel.Traveling || UIElement.PartWorld.Exist;
		if (!flag)
		{
			TooltipInvoker tipDisplayer = this._activeLoopButton.GetComponent<TooltipInvoker>();
			tipDisplayer.enabled = true;
			this.UpdateActiveLoopTipsData();
			tipDisplayer.ShowTips();
		}
	}

	// Token: 0x060017A3 RID: 6051 RVA: 0x00091930 File Offset: 0x0008FB30
	private unsafe void UpdateActiveLoopTipsData()
	{
		TooltipInvoker tipDisplayer = this._activeLoopButton.GetComponent<TooltipInvoker>();
		tipDisplayer.Type = TipType.ActiveLoop;
		tipDisplayer.NeedRefresh = true;
		TooltipInvoker tooltipInvoker = tipDisplayer;
		if (tooltipInvoker.RuntimeParam == null)
		{
			tooltipInvoker.RuntimeParam = new ArgumentBox();
		}
		tipDisplayer.RuntimeParam.Set("Title", LocalStringManager.Get(LanguageKey.LK_ActiveLoop_Tip_Title));
		short concentrationCost = GlobalConfig.Instance.ActiveNeigongLoopingAttributeCost;
		short haveConcentration = *this._taiwuMainAttributes[2];
		bool concentrationEnough = haveConcentration >= concentrationCost;
		string color = concentrationEnough ? "brightblue" : "brightred";
		string cost = LocalStringManager.GetFormat(LanguageKey.LK_ActiveLoop_Tip_Cost, string.Format("<color=#{0}>{1}</color>", color, haveConcentration), concentrationCost);
		tipDisplayer.RuntimeParam.Set("Cost1", cost);
		short timeCost = GlobalConfig.Instance.ActiveNeigongLoopingTimeCost;
		int haveTime = SingletonObject.getInstance<TimeManager>().GetRemainingActionPointConvertToDays();
		bool timeEnough = haveTime >= (int)timeCost;
		bool isMax = this._activeLoopingProgress >= GlobalConfig.Instance.MaxActiveNeigongLoopingProgress;
		color = (timeEnough ? "brightblue" : "brightred");
		string cost2 = LocalStringManager.GetFormat(LanguageKey.LK_Active_Tip_TimeCost, string.Format("<color=#{0}>{1}</color>", color, haveTime), timeCost);
		tipDisplayer.RuntimeParam.Set("Cost2", cost2);
		tipDisplayer.RuntimeParam.Set("ShowCost6", !concentrationEnough);
		tipDisplayer.RuntimeParam.Set("ShowCost7", !timeEnough);
		tipDisplayer.RuntimeParam.Set("ShowCost8", this._loopingNeigong == -1);
		tipDisplayer.RuntimeParam.Set("ShowCost10", isMax);
		tipDisplayer.Refresh(false, -1);
	}

	// Token: 0x060017A4 RID: 6052 RVA: 0x00091AE0 File Offset: 0x0008FCE0
	public void RefreshReadingBookProgress(ItemKey currentReadingBookKey, SkillBookPageDisplayData pagesInfo)
	{
		Refers readingSlotRefers = this._readingSlot.GetComponent<Refers>();
		CImage readingBookProgressImage = readingSlotRefers.CGet<CImage>("ReadingBookProgressImage");
		RectTransform progressCurrent = readingSlotRefers.CGet<RectTransform>("ProgressCurrent");
		bool flag = !currentReadingBookKey.IsValid() || pagesInfo == null || pagesInfo.ReadingProgress == null || pagesInfo.ReadingProgress.Length == 0;
		if (flag)
		{
			this._targetReadingBookProgress = 0f;
			this._currentReadingBookProgress = 0f;
			readingBookProgressImage.fillAmount = 0f;
			progressCurrent.anchoredPosition = new Vector2(0f, 0f);
		}
		else
		{
			int totalProgress = 0;
			foreach (sbyte p in pagesInfo.ReadingProgress)
			{
				totalProgress += (int)p;
			}
			int maxProgress = pagesInfo.ReadingProgress.Length * 100;
			float fillAmount = (float)totalProgress / (float)maxProgress;
			this._targetReadingBookProgress = fillAmount;
		}
	}

	// Token: 0x060017A5 RID: 6053 RVA: 0x00091BC0 File Offset: 0x0008FDC0
	private void UpdateReadingBookProgressTransition()
	{
		bool flag = Mathf.Abs(this._currentReadingBookProgress - this._targetReadingBookProgress) < 0.001f;
		if (flag)
		{
			this._currentReadingBookProgress = this._targetReadingBookProgress;
		}
		else
		{
			this._currentReadingBookProgress = Mathf.Lerp(this._currentReadingBookProgress, this._targetReadingBookProgress, Time.deltaTime * 5f);
			Refers readingSlotRefers = this._readingSlot.GetComponent<Refers>();
			CImage readingBookProgressImage = readingSlotRefers.CGet<CImage>("ReadingBookProgressImage");
			RectTransform progressCurrent = readingSlotRefers.CGet<RectTransform>("ProgressCurrent");
			readingBookProgressImage.fillAmount = this._currentReadingBookProgress;
			float maxHeight = progressCurrent.parent.GetComponent<RectTransform>().rect.height;
			progressCurrent.anchoredPosition = new Vector2(0f, maxHeight * this._currentReadingBookProgress);
		}
	}

	// Token: 0x060017A6 RID: 6054 RVA: 0x00091C88 File Offset: 0x0008FE88
	private unsafe bool CanActiveRead()
	{
		bool debugMode = ReadAndLoop.DebugMode;
		bool result;
		if (debugMode)
		{
			result = true;
		}
		else
		{
			bool flag = !this._currentReadingBookKey.IsValid();
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = this.IsInTutorial();
				if (flag2)
				{
					result = false;
				}
				else
				{
					bool flag3 = !SingletonObject.getInstance<TimeManager>().IsActionDayEnough((int)GlobalConfig.Instance.ActiveReadingTimeCost);
					if (flag3)
					{
						result = false;
					}
					else
					{
						bool flag4 = *this._taiwuMainAttributes[5] < GlobalConfig.Instance.ActiveReadingAttributeCost;
						if (flag4)
						{
							result = false;
						}
						else
						{
							bool isMax = this._activeReadingProgress >= GlobalConfig.Instance.MaxActiveReadingProgress;
							bool flag5 = isMax;
							result = !flag5;
						}
					}
				}
			}
		}
		return result;
	}

	// Token: 0x060017A7 RID: 6055 RVA: 0x00091D38 File Offset: 0x0008FF38
	private unsafe bool CanActiveLoop()
	{
		bool debugMode = ReadAndLoop.DebugMode;
		bool result;
		if (debugMode)
		{
			result = true;
		}
		else
		{
			bool flag = this._loopingNeigong == -1;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = this.IsInTutorial();
				if (flag2)
				{
					result = false;
				}
				else
				{
					bool flag3 = !SingletonObject.getInstance<TimeManager>().IsActionDayEnough((int)GlobalConfig.Instance.ActiveReadingTimeCost);
					if (flag3)
					{
						result = false;
					}
					else
					{
						bool flag4 = *this._taiwuMainAttributes[2] < GlobalConfig.Instance.ActiveNeigongLoopingAttributeCost;
						if (flag4)
						{
							result = false;
						}
						else
						{
							bool isMax = this._activeLoopingProgress >= GlobalConfig.Instance.MaxActiveNeigongLoopingProgress;
							bool flag5 = isMax;
							result = !flag5;
						}
					}
				}
			}
		}
		return result;
	}

	// Token: 0x060017A8 RID: 6056 RVA: 0x00091DE4 File Offset: 0x0008FFE4
	private bool IsInTutorial()
	{
		TutorialChapterModel model = SingletonObject.getInstance<TutorialChapterModel>();
		return model.InGuiding;
	}

	// Token: 0x17000289 RID: 649
	// (get) Token: 0x060017A9 RID: 6057 RVA: 0x00091E04 File Offset: 0x00090004
	private int CurrentReadStageIndex
	{
		get
		{
			bool flag = !this.IsCurrentReadStageIndexValid;
			int result;
			if (flag)
			{
				result = (int)((GlobalConfig.Instance.MaxActiveReadingProgress - 1) / 10);
			}
			else
			{
				result = (int)(this._activeReadingProgress / 10);
			}
			return result;
		}
	}

	// Token: 0x1700028A RID: 650
	// (get) Token: 0x060017AA RID: 6058 RVA: 0x00091E40 File Offset: 0x00090040
	private int CurrentLoopStageIndex
	{
		get
		{
			bool flag = !this.IsCurrentLoopStageIndexValid;
			int result;
			if (flag)
			{
				result = (int)((GlobalConfig.Instance.MaxActiveNeigongLoopingProgress - 1) / 10);
			}
			else
			{
				result = (int)(this._activeLoopingProgress / 10);
			}
			return result;
		}
	}

	// Token: 0x1700028B RID: 651
	// (get) Token: 0x060017AB RID: 6059 RVA: 0x00091E7A File Offset: 0x0009007A
	private bool IsCurrentReadStageIndexValid
	{
		get
		{
			return this._activeReadingProgress < GlobalConfig.Instance.MaxActiveReadingProgress;
		}
	}

	// Token: 0x1700028C RID: 652
	// (get) Token: 0x060017AC RID: 6060 RVA: 0x00091E8E File Offset: 0x0009008E
	private bool IsCurrentLoopStageIndexValid
	{
		get
		{
			return this._activeLoopingProgress < GlobalConfig.Instance.MaxActiveNeigongLoopingProgress;
		}
	}

	// Token: 0x060017AD RID: 6061 RVA: 0x00091EA4 File Offset: 0x000900A4
	private void ActiveLoopOnce()
	{
		bool debugMode = ReadAndLoop.DebugMode;
		if (debugMode)
		{
			this._activeLoopingProgress += 1;
			bool flag = this._activeLoopingProgress > GlobalConfig.Instance.MaxActiveNeigongLoopingProgress;
			if (flag)
			{
				this._activeLoopingProgress = 0;
			}
			this.OnLoopingProgressUpdate(this._activeLoopingProgress);
			this._lockActiveLoopWhenWaitingDataUpdate = false;
		}
		else
		{
			TaiwuDomainMethod.Call.ActiveNeigongLoopingOnce();
		}
	}

	// Token: 0x060017AE RID: 6062 RVA: 0x00091F04 File Offset: 0x00090104
	private void ActiveReadOnce()
	{
		bool debugMode = ReadAndLoop.DebugMode;
		if (debugMode)
		{
			this._activeReadingProgress += 1;
			bool flag = this._activeReadingProgress > GlobalConfig.Instance.MaxActiveReadingProgress;
			if (flag)
			{
				this._activeReadingProgress = 0;
			}
			this.OnReadingProgressUpdate(this._activeReadingProgress);
			this._lockActiveReadWhenWaitingDataUpdate = false;
		}
		else
		{
			TaiwuDomainMethod.Call.ActiveReadOnce();
			GEvent.OnEvent(UiEvents.OnTaiwuReadingBookProgressMayChange, null);
		}
	}

	// Token: 0x040012DD RID: 4829
	public static bool DebugMode;

	// Token: 0x040012DE RID: 4830
	private UIBase _parent;

	// Token: 0x040012DF RID: 4831
	private List<CImage> _loopingBallList;

	// Token: 0x040012E0 RID: 4832
	private CButtonObsolete _activeLoopButton;

	// Token: 0x040012E1 RID: 4833
	private CImage _activeLoopStageProgressImage;

	// Token: 0x040012E2 RID: 4834
	private CImage _activeLoopStateIcon;

	// Token: 0x040012E3 RID: 4835
	private CButtonObsolete _activeReadButton;

	// Token: 0x040012E4 RID: 4836
	private CImage _activeReadStageProgressImage;

	// Token: 0x040012E5 RID: 4837
	private CImage _activeReadStateIcon;

	// Token: 0x040012E6 RID: 4838
	private TextMeshProUGUI _leftLoopResource;

	// Token: 0x040012E7 RID: 4839
	private TextMeshProUGUI _leftReadResource;

	// Token: 0x040012E8 RID: 4840
	private TooltipInvoker _loopBallsTipDisplayer;

	// Token: 0x040012E9 RID: 4841
	private UIParticle _loopEventParticle;

	// Token: 0x040012EA RID: 4842
	private CButtonObsolete _loopNeigongSlot;

	// Token: 0x040012EB RID: 4843
	private TooltipInvoker _loopTipDisplayer;

	// Token: 0x040012EC RID: 4844
	private TooltipInvoker _readBallsTipDisplayer;

	// Token: 0x040012ED RID: 4845
	private UIParticle _readEventParticle;

	// Token: 0x040012EE RID: 4846
	private TooltipInvoker _readTipDisplayer;

	// Token: 0x040012EF RID: 4847
	private CommonPageStates _readingPages;

	// Token: 0x040012F0 RID: 4848
	private CButtonObsolete _readingSlot;

	// Token: 0x040012F1 RID: 4849
	private bool _isActiveReadPointerDown;

	// Token: 0x040012F2 RID: 4850
	private float _activeReadPointerDownTime;

	// Token: 0x040012F3 RID: 4851
	private float _activeReadPointerUpTime;

	// Token: 0x040012F4 RID: 4852
	private bool _isActiveLoopPointerDown;

	// Token: 0x040012F5 RID: 4853
	private float _activeLoopPointerDownTime;

	// Token: 0x040012F6 RID: 4854
	private float _activeLoopPointerUpTime;

	// Token: 0x040012F7 RID: 4855
	private float _activeReadLastTriggerTime;

	// Token: 0x040012F8 RID: 4856
	private float _activeLoopLastTriggerTime;

	// Token: 0x040012F9 RID: 4857
	private bool _isActiveReadingHoldSoundPlaying;

	// Token: 0x040012FA RID: 4858
	private bool _isActiveLoopHoldSoundPlaying;

	// Token: 0x040012FB RID: 4859
	private bool _lockActiveReadWhenWaitingDataUpdate;

	// Token: 0x040012FC RID: 4860
	private bool _lockActiveLoopWhenWaitingDataUpdate;

	// Token: 0x040012FD RID: 4861
	private float _currentReadingBookProgress;

	// Token: 0x040012FE RID: 4862
	private float _targetReadingBookProgress;

	// Token: 0x040012FF RID: 4863
	private int _taiwuCharId;

	// Token: 0x04001300 RID: 4864
	private short _activeLoopingProgress = -1;

	// Token: 0x04001301 RID: 4865
	private short _activeReadingProgress = -1;

	// Token: 0x04001302 RID: 4866
	private short _lastReadingProgress = -1;

	// Token: 0x04001303 RID: 4867
	private short _lastLoopingProgress = -1;

	// Token: 0x04001304 RID: 4868
	private List<short> _loopingEventSkillIdList;

	// Token: 0x04001305 RID: 4869
	private short _loopingNeigong;

	// Token: 0x04001306 RID: 4870
	private ItemKey _currentReadingBookKey;

	// Token: 0x04001307 RID: 4871
	private sbyte _cachedNeiliType;

	// Token: 0x04001308 RID: 4872
	private int[] _taiwuExtraNeiliAllocationProgress;

	// Token: 0x04001309 RID: 4873
	private MainAttributes _taiwuMainAttributes;

	// Token: 0x0400130A RID: 4874
	private const string ReadOrLoopHoldSoundName = "SFX_reading_ui_liuguang_loop";

	// Token: 0x0400130B RID: 4875
	private const string ReadOrLoopClickSoundName = "SFX_reading_ui_liuguang_click";

	// Token: 0x0400130C RID: 4876
	private const string ReadOrLoopReversSoundName = "SFX_reading_ui_liuguang_end";

	// Token: 0x0400130D RID: 4877
	private const string ReadOrLoopProgressSoundName = "SFX_reading_ui_liuguang_flash";

	// Token: 0x0400130E RID: 4878
	private const string GrayBall = "bottom_point_base";

	// Token: 0x0400130F RID: 4879
	private const string MaxNeiliBall = "bottom_point_0_0";

	// Token: 0x04001310 RID: 4880
	private const string FiveElementBallsPattern = "bottom_point_1_{0}";

	// Token: 0x04001311 RID: 4881
	private const string NeiliAllocationTypeBallsPattern = "bottom_point_2_{0}";
}
