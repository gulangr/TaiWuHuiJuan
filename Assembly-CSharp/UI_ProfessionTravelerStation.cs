using System;
using System.Collections.Generic;
using Config;
using DG.Tweening;
using FrameWork;
using GameData.Common;
using GameData.Domains.Character;
using GameData.Domains.Map;
using GameData.Domains.Taiwu.Profession;
using GameData.Domains.Taiwu.Profession.SkillsData;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

// Token: 0x020002F6 RID: 758
public class UI_ProfessionTravelerStation : UIBase
{
	// Token: 0x170004DA RID: 1242
	// (get) Token: 0x06002C6B RID: 11371 RVA: 0x0015C39E File Offset: 0x0015A59E
	private ProfessionModel ProfessionModel
	{
		get
		{
			return SingletonObject.getInstance<ProfessionModel>();
		}
	}

	// Token: 0x170004DB RID: 1243
	// (get) Token: 0x06002C6C RID: 11372 RVA: 0x0015C3A5 File Offset: 0x0015A5A5
	private WorldMapModel _mapModel
	{
		get
		{
			return SingletonObject.getInstance<WorldMapModel>();
		}
	}

	// Token: 0x06002C6D RID: 11373 RVA: 0x0015C3AC File Offset: 0x0015A5AC
	public override void OnInit(ArgumentBox argsBox)
	{
		this._prefabRefersList = base.CGetList<Refers>("PrefabRefer_");
		this.InitData();
		List<Location> locationList = new List<Location>();
		ProfessionData professionData;
		bool flag = this.ProfessionModel.TaiwuProfessions.TryGetValue(11, out professionData);
		if (flag)
		{
			TravelerSkillsData skillData = professionData.GetSkillsData<TravelerSkillsData>();
			for (int i = 0; i < 3; i++)
			{
				bool flag2 = i < skillData.PalaceCount;
				if (flag2)
				{
					TravelerPalaceData palaceData = skillData.TryGetPalaceData(i);
					locationList.Add(palaceData.Location);
				}
			}
		}
		this._mapModel.RequestExtraMapBlockData(locationList, true, true);
	}

	// Token: 0x06002C6E RID: 11374 RVA: 0x0015C44C File Offset: 0x0015A64C
	private void InitData()
	{
		this._currHealth = -1;
		this._leftMaxHealth = -1;
		this._taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
		for (int i = 0; i < this._prefabRefersList.Count; i++)
		{
			Refers refers = this._prefabRefersList[i];
			CanvasGroup canvasGroup = refers.CGet<CanvasGroup>("SensitiveWarningTip");
			canvasGroup.alpha = 0f;
		}
		for (int j = 0; j < this._palaceAreaIds.Length; j++)
		{
			this._palaceAreaIds[j] = -1;
		}
	}

	// Token: 0x06002C6F RID: 11375 RVA: 0x0015C4E4 File Offset: 0x0015A6E4
	private void OnExtraMapBlockDataRequested(ArgumentBox argbox)
	{
		ProfessionData professionData;
		bool flag = this.ProfessionModel.TaiwuProfessions.TryGetValue(11, out professionData);
		if (flag)
		{
			TravelerSkillsData skillData = professionData.GetSkillsData<TravelerSkillsData>();
			for (int i = 0; i < 3; i++)
			{
				bool flag2 = i < skillData.PalaceCount;
				if (flag2)
				{
					this.SetPalace(i, skillData);
				}
				else
				{
					this.SetEmptyPalace(i, skillData);
				}
			}
		}
	}

	// Token: 0x06002C70 RID: 11376 RVA: 0x0015C54E File Offset: 0x0015A74E
	private void OnEnable()
	{
		GEvent.Add(UiEvents.OnExtraMapBlockDataRequested, new GEvent.Callback(this.OnExtraMapBlockDataRequested));
	}

	// Token: 0x06002C71 RID: 11377 RVA: 0x0015C56D File Offset: 0x0015A76D
	private void OnDisable()
	{
		GEvent.Remove(UiEvents.OnExtraMapBlockDataRequested, new GEvent.Callback(this.OnExtraMapBlockDataRequested));
	}

	// Token: 0x06002C72 RID: 11378 RVA: 0x0015C58C File Offset: 0x0015A78C
	public override void InitMonitorFieldIds()
	{
		this.MonitorFields.Add(new UIBase.MonitorDataField(4, 0, (ulong)((long)this._taiwuCharId), new uint[]
		{
			19U
		}));
	}

	// Token: 0x06002C73 RID: 11379 RVA: 0x0015C5B4 File Offset: 0x0015A7B4
	public override void OnNotifyGameData(List<NotificationWrapper> notifications)
	{
		foreach (NotificationWrapper wrapper in notifications)
		{
			Notification notification = wrapper.Notification;
			byte type = notification.Type;
			byte b = type;
			if (b == 0)
			{
				DataUid uid = notification.Uid;
				bool flag = uid.DomainId == 4 && uid.DataId == 0 && (int)uid.SubId0 == this._taiwuCharId && uid.SubId1 == 19U;
				if (flag)
				{
					Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._currHealth);
					CharacterDomainMethod.AsyncCall.GetLeftMaxHealth(this, this._taiwuCharId, delegate(int offset, RawDataPool pool)
					{
						Serializer.Deserialize(pool, offset, ref this._leftMaxHealth);
						this.RefreshMoveBtn();
						this.Element.ShowAfterRefresh();
					});
				}
			}
		}
	}

	// Token: 0x06002C74 RID: 11380 RVA: 0x0015C690 File Offset: 0x0015A890
	private void RefreshMoveBtn()
	{
		bool flag = this._currHealth < 0 || this._leftMaxHealth < 0;
		if (!flag)
		{
			bool healthEnough = this._currHealth * 10 >= this._leftMaxHealth;
			bool actionPointEnough = SingletonObject.getInstance<TimeManager>().IsActionPointEnough(10);
			bool canInterStateTravel = SingletonObject.getInstance<FunctionLockManager>().IsFunctionUnlock(4);
			TravelerSkillsData skillData = null;
			ProfessionData professionData;
			bool flag2 = this.ProfessionModel.TaiwuProfessions.TryGetValue(11, out professionData);
			if (flag2)
			{
				skillData = professionData.GetSkillsData<TravelerSkillsData>();
			}
			for (int i = 0; i < 3; i++)
			{
				Refers refers = this._prefabRefersList[i];
				CButtonObsolete moveBtn = refers.CGet<CButtonObsolete>("MoveBtn");
				TravelerPalaceData palaceData = (skillData != null) ? skillData.TryGetPalaceData(i) : null;
				bool inTaiwuVillage = palaceData != null && palaceData.Location.AreaId == this._mapModel.GetTaiwuVillageAreaId();
				moveBtn.interactable = (healthEnough && actionPointEnough && (canInterStateTravel || inTaiwuVillage));
				TooltipInvoker mouseTipDisplayer = moveBtn.GetComponent<TooltipInvoker>();
				mouseTipDisplayer.enabled = !moveBtn.interactable;
				moveBtn.GetComponent<DisableStyleRoot>().SetStyleEffect(!moveBtn.interactable, false);
				moveBtn.GetComponent<PointerTrigger>().enabled = moveBtn.interactable;
				bool flag3 = !moveBtn.interactable;
				if (flag3)
				{
					bool flag4 = !canInterStateTravel && !inTaiwuVillage;
					LanguageKey tipLKey;
					if (flag4)
					{
						tipLKey = LanguageKey.LK_ProfessionTravelerStation_Text10;
					}
					else
					{
						bool flag5 = healthEnough;
						if (flag5)
						{
							tipLKey = LanguageKey.LK_ProfessionTravelerStation_Text8;
						}
						else
						{
							tipLKey = LanguageKey.LK_ProfessionTravelerStation_Text9;
						}
					}
					bool flag6 = mouseTipDisplayer.RuntimeParam == null;
					if (flag6)
					{
						mouseTipDisplayer.RuntimeParam = new ArgumentBox();
					}
					mouseTipDisplayer.RuntimeParam.Set("arg0", LocalStringManager.Get(tipLKey));
				}
			}
		}
	}

	// Token: 0x06002C75 RID: 11381 RVA: 0x0015C85C File Offset: 0x0015AA5C
	private void SetPalace(int index, TravelerSkillsData skillData)
	{
		Refers refers = this._prefabRefersList[index];
		TravelerPalaceData palaceData = skillData.TryGetPalaceData(index);
		this.SetPalaceActive(refers, false);
		CButtonObsolete moveBtn = refers.CGet<CButtonObsolete>("MoveBtn");
		Action <>9__3;
		moveBtn.ClearAndAddListener(delegate
		{
			UIElement dialog = UIElement.Dialog;
			ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>();
			string key = "Cmd";
			DialogCmd dialogCmd = new DialogCmd();
			dialogCmd.Title = LocalStringManager.Get(LanguageKey.LK_ProfessionTravelerStation_Tips1).ColorReplace();
			dialogCmd.Content = LocalStringManager.Get(LanguageKey.LK_ProfessionTravelerStation_Tips2).ColorReplace();
			dialogCmd.Type = 1;
			Action yes;
			if ((yes = <>9__3) == null)
			{
				yes = (<>9__3 = delegate()
				{
					this.QuickHide();
					ArgumentBox argumentBox2 = EasyPool.Get<ArgumentBox>();
					argumentBox2.Set("Index", index);
					argumentBox2.Set("TargetAreaId", this._palaceAreaIds[index]);
					GEvent.OnEvent(UiEvents.ProfessionTravelerSkillThreeMove, argumentBox2);
				});
			}
			dialogCmd.Yes = yes;
			dialogCmd.SpriteHelperSize = new Vector2(26f, 26f);
			dialogCmd.SpriteHelperFitType = TMPTextSpriteHelper.SizeFitType.Native;
			dialog.SetOnInitArgs(argumentBox.SetObject(key, dialogCmd));
			UIManager.Instance.MaskUI(UIElement.Dialog);
		});
		Action <>9__4;
		refers.CGet<CButtonObsolete>("RemoveBtn").ClearAndAddListener(delegate
		{
			UIElement dialog = UIElement.Dialog;
			ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>();
			string key = "Cmd";
			DialogCmd dialogCmd = new DialogCmd();
			dialogCmd.Title = LocalStringManager.Get(LanguageKey.LK_ProfessionTravelerStation_Tips3).ColorReplace();
			dialogCmd.Content = LocalStringManager.Get(LanguageKey.LK_ProfessionTravelerStation_Tips4).ColorReplace();
			dialogCmd.Type = 1;
			Action yes;
			if ((yes = <>9__4) == null)
			{
				yes = (<>9__4 = delegate()
				{
					MapDomainMethod.Call.DestroyTravelerPalace(this.Element.GameDataListenerId, index);
					this.SetPalaceActive(refers, true);
					this.UpdateBuildBtnUnable();
					AudioManager.Instance.StopLoopSoundWithAmbience("SFX_professionskill_lvren_loop");
					AudioManager.Instance.PlaySound("SFX_professionskill_lvren_stop", false, false);
				});
			}
			dialogCmd.Yes = yes;
			dialog.SetOnInitArgs(argumentBox.SetObject(key, dialogCmd));
			UIManager.Instance.MaskUI(UIElement.Dialog);
		});
		TextMeshProUGUI nameTitle = refers.CGet<TextMeshProUGUI>("NameTitle");
		nameTitle.SetText(this.GetPalaceName(palaceData), true);
		CButtonObsolete editNameButton = refers.CGet<CButtonObsolete>("EditNameButton");
		TMP_InputField nameInput = refers.CGet<TMP_InputField>("NameInput");
		int i = index;
		editNameButton.ClearAndAddListener(delegate
		{
			this.OnClickEditorName(nameInput, editNameButton, nameTitle, i, palaceData);
		});
		Refers blockHolder = refers.CGet<Refers>("BlockHolder");
		this.SetMapBlock(index, blockHolder, palaceData);
	}

	// Token: 0x06002C76 RID: 11382 RVA: 0x0015C9A0 File Offset: 0x0015ABA0
	private void UpdateBuildBtnUnable()
	{
		for (int i = 0; i < 3; i++)
		{
			Refers refers = this._prefabRefersList[i];
			CButtonObsolete buildBtnUnable = refers.CGet<CButtonObsolete>("BuildBtnUnable");
			bool activeSelf = buildBtnUnable.gameObject.activeSelf;
			if (activeSelf)
			{
				buildBtnUnable.gameObject.SetActive(false);
				CButtonObsolete buildBtnEnable = refers.CGet<CButtonObsolete>("BuildBtnEnable");
				buildBtnEnable.gameObject.SetActive(true);
				buildBtnEnable.ClearAndAddListener(new Action(this.OnConfirmBuild));
			}
		}
	}

	// Token: 0x06002C77 RID: 11383 RVA: 0x0015CA28 File Offset: 0x0015AC28
	private void SetMapBlock(int index, Refers blockHolder, TravelerPalaceData palaceData)
	{
		MapBlockData blockData = this._mapModel.GetBlockData(palaceData.Location);
		MapBlockView mapBlockView = blockHolder.CGet<MapBlockView>("MapBlockView");
		MapBlockData rootBlockData = (blockData.RootBlockId > -1) ? this._mapModel.GetBlockData(new Location(blockData.AreaId, blockData.RootBlockId)) : null;
		mapBlockView.Refresh(blockData, rootBlockData);
		this._palaceAreaIds[index] = blockData.AreaId;
		MapDomainMethod.AsyncCall.GetBlockData(this, palaceData.Location.AreaId, palaceData.Location.BlockId, delegate(int offsetR, RawDataPool poolR)
		{
			MapBlockData blockData2 = new MapBlockData();
			Serializer.Deserialize(poolR, offsetR, ref blockData2);
		});
		MapDomainMethod.AsyncCall.GetBlockFullName(this, palaceData.Location, delegate(int offsetData, RawDataPool poolData)
		{
			FullBlockName fullBlockName = default(FullBlockName);
			Serializer.Deserialize(poolData, offsetData, ref fullBlockName);
			string blockName = SingletonObject.getInstance<WorldMapModel>().GetFullBlockName(fullBlockName, true, true, true, true);
			TextMeshProUGUI locationName = blockHolder.CGet<TextMeshProUGUI>("LocationName");
			locationName.text = blockName;
		});
	}

	// Token: 0x06002C78 RID: 11384 RVA: 0x0015CB00 File Offset: 0x0015AD00
	private void SetEmptyPalace(int index, TravelerSkillsData skillData)
	{
		Refers refers = this._prefabRefersList[index];
		this.SetPalaceActive(refers, true);
		bool canBuild = this.CanBuildPalace(skillData);
		CButtonObsolete buildBtnEnable = refers.CGet<CButtonObsolete>("BuildBtnEnable");
		CButtonObsolete buildBtnUnable = refers.CGet<CButtonObsolete>("BuildBtnUnable");
		buildBtnEnable.gameObject.SetActive(canBuild);
		buildBtnUnable.gameObject.SetActive(!canBuild);
		buildBtnEnable.ClearAndAddListener(new Action(this.OnConfirmBuild));
	}

	// Token: 0x06002C79 RID: 11385 RVA: 0x0015CB74 File Offset: 0x0015AD74
	private bool CanBuildPalace(TravelerSkillsData skillData)
	{
		bool result = true;
		for (int i = 0; i < skillData.PalaceCount; i++)
		{
			TravelerPalaceData palaceData = skillData.TryGetPalaceData(i);
			bool flag = palaceData.Location == this._mapModel.CurrentLocation;
			if (flag)
			{
				result = false;
				break;
			}
		}
		return result;
	}

	// Token: 0x06002C7A RID: 11386 RVA: 0x0015CBCC File Offset: 0x0015ADCC
	private void OnConfirmBuild()
	{
		Action confirm = delegate()
		{
			AudioManager.Instance.StopLoopSoundWithAmbience("SFX_professionskill_lvren_loop");
			AudioManager.Instance.PlayLoopSoundWithAmbience("SFX_professionskill_lvren_loop");
			this.QuickHide();
		};
		ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>();
		argumentBox.Clear();
		ProfessionSkillArg professionSkillArg = new ProfessionSkillArg
		{
			ProfessionId = 11,
			SkillId = 47,
			IsSuccess = true
		};
		argumentBox.SetObject("ProfessionSkillArg", professionSkillArg);
		argumentBox.SetObject("OnConfirm", confirm);
		UIElement.ProfessionSkillConfirm.SetOnInitArgs(argumentBox);
		UIManager.Instance.MaskUI(UIElement.ProfessionSkillConfirm);
	}

	// Token: 0x06002C7B RID: 11387 RVA: 0x0015CC48 File Offset: 0x0015AE48
	private void SetPalaceActive(Refers refers, bool isEmpty)
	{
		refers.CGet<GameObject>("NoneBg").SetActive(isEmpty);
		refers.CGet<GameObject>("NameNone").SetActive(isEmpty);
		refers.CGet<CButtonObsolete>("BuildBtnEnable").gameObject.SetActive(isEmpty);
		refers.CGet<CButtonObsolete>("BuildBtnUnable").gameObject.SetActive(isEmpty);
		refers.CGet<CButtonObsolete>("MoveBtn").gameObject.SetActive(!isEmpty);
		refers.CGet<CButtonObsolete>("EditNameButton").gameObject.SetActive(!isEmpty);
		refers.CGet<CButtonObsolete>("RemoveBtn").gameObject.SetActive(!isEmpty);
		refers.CGet<Refers>("BlockHolder").gameObject.SetActive(!isEmpty);
		refers.CGet<TMP_InputField>("NameInput").gameObject.SetActive(false);
		refers.CGet<TextMeshProUGUI>("NameTitle").transform.parent.gameObject.SetActive(!isEmpty);
	}

	// Token: 0x06002C7C RID: 11388 RVA: 0x0015CD4C File Offset: 0x0015AF4C
	private string GetPalaceName(TravelerPalaceData palaceData)
	{
		bool flag = palaceData.CustomName.IsNullOrEmpty();
		string result;
		if (flag)
		{
			result = UI_ProfessionTravelerStation.GetPalaceDefaultName(this._mapModel, palaceData);
		}
		else
		{
			result = palaceData.CustomName;
		}
		return result;
	}

	// Token: 0x06002C7D RID: 11389 RVA: 0x0015CD84 File Offset: 0x0015AF84
	public static string GetPalaceDefaultName(WorldMapModel mapModel, TravelerPalaceData palaceData)
	{
		MapAreaData areaData = mapModel.Areas[(int)palaceData.Location.AreaId];
		short templateId = areaData.GetTemplateId();
		return LocalStringManager.GetFormat(LanguageKey.LK_ProfessionTravelerStation_Text2, MapArea.Instance[templateId].Name);
	}

	// Token: 0x06002C7E RID: 11390 RVA: 0x0015CDCC File Offset: 0x0015AFCC
	private void OnClickEditorName(TMP_InputField inputField, CButtonObsolete btn, TextMeshProUGUI nameTitle, int index, TravelerPalaceData palaceData)
	{
		TMP_FontAsset fontAsset = inputField.textComponent.font;
		this.SetRenameActive(inputField, btn, nameTitle, true);
		inputField.onValueChanged.RemoveAllListeners();
		inputField.textComponent.rectTransform.localPosition = Vector3.zero;
		inputField.text = nameTitle.text;
		inputField.onValueChanged.RemoveAllListeners();
		inputField.onValueChanged.AddListener(delegate(string valueStr)
		{
			inputField.FixAndSetInputFieldText(ref valueStr, fontAsset);
		});
		inputField.onEndEdit.RemoveAllListeners();
		inputField.onEndEdit.AddListener(delegate(string valueStr)
		{
			bool hasSensitiveWord = inputField.SensitiveWordHandle(ref valueStr);
			bool flag2 = hasSensitiveWord;
			if (flag2)
			{
				CanvasGroup canvasGroup = this._prefabRefersList[index].CGet<CanvasGroup>("SensitiveWarningTip");
				canvasGroup.alpha = 1f;
				bool flag3 = this._sensitiveTipsCoroutines[index] != null;
				if (flag3)
				{
					this.StopCoroutine(this._sensitiveTipsCoroutines[index]);
				}
				Tween tween = this._sensitiveTipsTweens[index];
				if (tween != null)
				{
					tween.Kill(false);
				}
				this._sensitiveTipsCoroutines[index] = this.DelayCallReturnCoroutine(delegate
				{
					bool activeInHierarchy = canvasGroup.gameObject.activeInHierarchy;
					if (activeInHierarchy)
					{
						this._sensitiveTipsTweens[index] = canvasGroup.DOFade(0f, SensitiveWordsSystem.SensitiveWordAnimationFadeTime);
					}
				}, SensitiveWordsSystem.SensitiveWordAnimationStayTime);
			}
			this.RenameConfirm(inputField, btn, nameTitle, index, palaceData);
			inputField.textComponent.transform.localPosition = Vector2.zero;
			inputField.transform.Find("Text Area/Caret").localPosition = Vector2.zero;
			btn.gameObject.SetActive(true);
		});
		bool flag = inputField.text.IsNullOrEmpty();
		if (flag)
		{
			inputField.Select();
		}
		inputField.InputOnSelectBindMoveTextEnd();
	}

	// Token: 0x06002C7F RID: 11391 RVA: 0x0015CF07 File Offset: 0x0015B107
	private void SetRenameActive(TMP_InputField inputField, CButtonObsolete btn, TextMeshProUGUI nameTitle, bool start)
	{
		btn.gameObject.SetActive(!start);
		nameTitle.transform.parent.gameObject.SetActive(!start);
		inputField.gameObject.SetActive(start);
	}

	// Token: 0x06002C80 RID: 11392 RVA: 0x0015CF44 File Offset: 0x0015B144
	private void RenameConfirm(TMP_InputField inputField, CButtonObsolete btn, TextMeshProUGUI nameTitle, int index, TravelerPalaceData palaceData)
	{
		this.SetRenameActive(inputField, btn, nameTitle, false);
		string newName = inputField.text.IsNullOrEmpty() ? this.GetPalaceName(palaceData) : inputField.text;
		nameTitle.SetText(newName, true);
		MapDomainMethod.Call.ChangeTravelerPalaceName(this.Element.GameDataListenerId, index, newName);
	}

	// Token: 0x06002C81 RID: 11393 RVA: 0x0015CF98 File Offset: 0x0015B198
	protected override void OnClick(Transform btn)
	{
		string btnName = btn.name;
		string text = btnName;
		string a = text;
		if (a == "CloseBtn")
		{
			this.QuickHide();
		}
	}

	// Token: 0x06002C82 RID: 11394 RVA: 0x0015CFCA File Offset: 0x0015B1CA
	public override void QuickHide()
	{
		AudioManager.Instance.PlaySound("ui_default_cancel", false, false);
		base.QuickHide();
	}

	// Token: 0x04002025 RID: 8229
	private List<Refers> _prefabRefersList;

	// Token: 0x04002026 RID: 8230
	private Coroutine[] _sensitiveTipsCoroutines = new Coroutine[3];

	// Token: 0x04002027 RID: 8231
	private Tween[] _sensitiveTipsTweens = new Tween[3];

	// Token: 0x04002028 RID: 8232
	private short[] _palaceAreaIds = new short[3];

	// Token: 0x04002029 RID: 8233
	private int _taiwuCharId;

	// Token: 0x0400202A RID: 8234
	private short _currHealth;

	// Token: 0x0400202B RID: 8235
	private short _leftMaxHealth;
}
