using System;
using System.Runtime.CompilerServices;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Views.Building.BuildingManage;
using Game.Views.World;
using GameData.Domains.Map;
using GameData.Domains.Taiwu;
using GameData.Domains.Taiwu.Profession;
using GameData.Domains.Taiwu.Profession.SkillsData;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.Profession
{
	// Token: 0x020007CA RID: 1994
	public class ViewProfessionTravelerStation : UIBase
	{
		// Token: 0x06006173 RID: 24947 RVA: 0x002CAF20 File Offset: 0x002C9120
		public override void OnInit(ArgumentBox argsBox)
		{
			this.ProfessionModel = SingletonObject.getInstance<ProfessionModel>();
			this.MapModel = SingletonObject.getInstance<WorldMapModel>();
			this._currHealth = -1;
			this._leftMaxHealth = -1;
			this.areaMap.SelectedAreaTemplateId = -1;
			foreach (TravelerStation t in this.travelerStations)
			{
				t.Init();
			}
			UIElement element = this.Element;
			element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(this.RequestData));
		}

		// Token: 0x06006174 RID: 24948 RVA: 0x002CAFA7 File Offset: 0x002C91A7
		private void RequestData()
		{
			this.areaMap.Init(false, this, true);
			TaiwuDomainMethod.AsyncCall.RequestTravelerSkillsDisplayData(this, delegate(int offset, RawDataPool pool)
			{
				TravelerSkillsDisplayData travelerSkillsDisplayData = new TravelerSkillsDisplayData();
				Serializer.Deserialize(pool, offset, ref travelerSkillsDisplayData);
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
							this.travelerStations[i].blockHolder.RefreshImpl(travelerSkillsDisplayData.DisplayData[i], (string x) => x);
							this.travelerStations[i].AreaId = travelerSkillsDisplayData.DisplayData[i].Location.AreaId;
						}
						else
						{
							this.SetEmptyPalace(i, skillData);
						}
					}
					foreach (TravelerStation item in this.travelerStations)
					{
						item.canvasGroup.alpha = 1f;
					}
				}
				this._currHealth = travelerSkillsDisplayData.CurrHealth;
				this._leftMaxHealth = travelerSkillsDisplayData.LeftMaxHealth;
				this.RefreshMoveBtn();
				this.Element.ShowAfterRefresh();
			});
		}

		// Token: 0x06006175 RID: 24949 RVA: 0x002CAFCC File Offset: 0x002C91CC
		private void OnExtraMapBlockDataRequested(ArgumentBox argbox)
		{
			this.RequestData();
		}

		// Token: 0x06006176 RID: 24950 RVA: 0x002CAFD5 File Offset: 0x002C91D5
		private void OnEnable()
		{
			GEvent.Add(UiEvents.OnExtraMapBlockDataRequested, new GEvent.Callback(this.OnExtraMapBlockDataRequested));
		}

		// Token: 0x06006177 RID: 24951 RVA: 0x002CAFF4 File Offset: 0x002C91F4
		private void OnDisable()
		{
			GEvent.Remove(UiEvents.OnExtraMapBlockDataRequested, new GEvent.Callback(this.OnExtraMapBlockDataRequested));
		}

		// Token: 0x06006178 RID: 24952 RVA: 0x002CB014 File Offset: 0x002C9214
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
					TravelerStation travelerStation = this.travelerStations[i];
					CButton moveBtn = travelerStation.moveBtn;
					TravelerPalaceData palaceData = (skillData != null) ? skillData.TryGetPalaceData(i) : null;
					bool inTaiwuVillage = palaceData != null && palaceData.Location.AreaId == this.MapModel.GetTaiwuVillageAreaId();
					moveBtn.interactable = (healthEnough && actionPointEnough && (canInterStateTravel || inTaiwuVillage));
					TooltipInvoker mouseTipDisplayer = travelerStation.moveBtnMouseTipDisplayer;
					mouseTipDisplayer.enabled = !moveBtn.interactable;
					travelerStation.moveBtnDisableStyleRoot.SetStyleEffect(!moveBtn.interactable, false);
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
						TooltipInvoker tooltipInvoker = mouseTipDisplayer;
						ArgumentBox argumentBox;
						if ((argumentBox = tooltipInvoker.RuntimeParam) == null)
						{
							argumentBox = (tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>());
						}
						argumentBox.Set("arg0", tipLKey.Tr());
					}
				}
			}
		}

		// Token: 0x06006179 RID: 24953 RVA: 0x002CB1C0 File Offset: 0x002C93C0
		private void SetPalace(int index, TravelerSkillsData skillData)
		{
			TravelerStation refers = this.travelerStations[index];
			TravelerPalaceData palaceData = skillData.TryGetPalaceData(index);
			refers.AreaId = palaceData.Location.AreaId;
			this.SetPalaceActive(refers, false);
			CButton moveBtn = refers.moveBtn;
			Action <>9__4;
			Action <>9__5;
			Action <>9__3;
			moveBtn.ClearAndAddListener(delegate
			{
				bool flag = refers.AreaId < 0;
				if (!flag)
				{
					short templateId = this.MapModel.Areas[(int)refers.AreaId].GetTemplateId();
					this.areaMap.LookAtTemplate(templateId, 0.3f, default(Vector2));
					this.areaMap.OnSelectAreaTemplateId = new Action<short>(ViewProfessionTravelerStation.Nop);
					this.areaMap.SelectedAreaTemplateId = templateId;
					this.areaMap.OnSelectAreaTemplateId = null;
					YieldHelper instance = SingletonObject.getInstance<YieldHelper>();
					float sec = 0.6f;
					Action job;
					if ((job = <>9__3) == null)
					{
						job = (<>9__3 = delegate()
						{
							UIElement dialog = UIElement.Dialog;
							ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>();
							string key = "Cmd";
							DialogCmd dialogCmd = new DialogCmd();
							dialogCmd.Title = LocalStringManager.Get(LanguageKey.LK_ProfessionTravelerStation_Tips1).ColorReplace();
							dialogCmd.Content = LocalStringManager.Get(LanguageKey.LK_ProfessionTravelerStation_Tips2).ColorReplace();
							dialogCmd.Type = 1;
							Action yes;
							if ((yes = <>9__4) == null)
							{
								yes = (<>9__4 = delegate()
								{
									this.QuickHide();
									ArgumentBox argumentBox2 = EasyPool.Get<ArgumentBox>();
									argumentBox2.Set("Index", index);
									argumentBox2.Set("TargetAreaId", this._palaceAreaIds[index]);
									GEvent.OnEvent(UiEvents.ProfessionTravelerSkillThreeMove, argumentBox2);
								});
							}
							dialogCmd.Yes = yes;
							Action no;
							if ((no = <>9__5) == null)
							{
								no = (<>9__5 = delegate()
								{
									this.areaMap.SelectedAreaTemplateId = -1;
								});
							}
							dialogCmd.No = no;
							dialogCmd.SpriteHelperSize = new Vector2(26f, 26f);
							dialogCmd.SpriteHelperFitType = TMPTextSpriteHelper.SizeFitType.Native;
							dialog.SetOnInitArgs(argumentBox.SetObject(key, dialogCmd));
							UIManager.Instance.MaskUI(UIElement.Dialog);
						});
					}
					instance.DelaySecondsDo(sec, job);
				}
			});
			Action <>9__6;
			refers.removeBtn.onClick.ResetListener(delegate()
			{
				UIElement dialog = UIElement.Dialog;
				ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>();
				string key = "Cmd";
				DialogCmd dialogCmd = new DialogCmd();
				dialogCmd.Title = LocalStringManager.Get(LanguageKey.LK_ProfessionTravelerStation_Tips3).ColorReplace();
				dialogCmd.Content = LocalStringManager.Get(LanguageKey.LK_ProfessionTravelerStation_Tips4).ColorReplace();
				dialogCmd.Type = 1;
				Action yes;
				if ((yes = <>9__6) == null)
				{
					yes = (<>9__6 = delegate()
					{
						MapDomainMethod.Call.DestroyTravelerPalace(this.Element.GameDataListenerId, index);
						refers.AreaId = -1;
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
			TMP_Text nameTitle = refers.nameTitle;
			nameTitle.SetText(this.GetPalaceName(palaceData), true);
			Action<string> <>9__7;
			refers.editNameButton.ClearAndAddListener(delegate
			{
				RenameCfg renameCfg = new RenameCfg();
				renameCfg.Title = LanguageKey.LK_Building_QuickAction_Rename_Title.Tr();
				renameCfg.Description = LanguageKey.LK_Building_QuickAction_Rename_Desc.TrFormat(this.GetPalaceName(palaceData));
				renameCfg.EmptyDesc = LanguageKey.LK_Building_QuickAction_Rename_Empty.Tr();
				renameCfg.Default = this.GetPalaceName(palaceData);
				Action<string> submit;
				if ((submit = <>9__7) == null)
				{
					submit = (<>9__7 = delegate(string s)
					{
						this.RenameConfirm(s, nameTitle, index, palaceData);
					});
				}
				renameCfg.Submit = submit;
				renameCfg.CharCount = ViewBuildingManage.GetBuildingNameCharCount();
				renameCfg.Show();
			});
		}

		// Token: 0x0600617A RID: 24954 RVA: 0x002CB2B8 File Offset: 0x002C94B8
		private void UpdateBuildBtnUnable()
		{
			for (int i = 0; i < 3; i++)
			{
				TravelerStation refers = this.travelerStations[i];
				short currentAreaId = this.areaMap.MapModel.CurrentAreaId;
				this.SetBuildBtn(refers, currentAreaId >= 0 && currentAreaId < 135);
			}
		}

		// Token: 0x0600617B RID: 24955 RVA: 0x002CB308 File Offset: 0x002C9508
		private void SetBuildBtn(TravelerStation refers, bool canBuild)
		{
			CButton buildBtn = refers.buildBtn;
			buildBtn.interactable = canBuild;
			refers.buildBtnDisableStyleRootRoot.SetStyleEffect(refers.buildBtnMouseTipDisplayer.enabled = !canBuild, false);
			TooltipInvoker buildBtnMouseTipDisplayer = refers.buildBtnMouseTipDisplayer;
			ArgumentBox argumentBox;
			if ((argumentBox = buildBtnMouseTipDisplayer.RuntimeParam) == null)
			{
				argumentBox = (buildBtnMouseTipDisplayer.RuntimeParam = EasyPool.Get<ArgumentBox>());
			}
			string key = "arg0";
			string[] presetParam = refers.buildBtnMouseTipDisplayer.PresetParam;
			int num = 0;
			short currentAreaId = this.areaMap.MapModel.CurrentAreaId;
			argumentBox.Set(key, presetParam[num] = ((currentAreaId < 0 || currentAreaId >= 135) ? LanguageKey.LK_ProfessionTravelerStation_Text11.Tr() : LanguageKey.LK_ProfessionTravelerStation_Text7.Tr()));
			buildBtn.onClick.ResetListener(new Action(this.OnConfirmBuild));
		}

		// Token: 0x0600617C RID: 24956 RVA: 0x002CB3D0 File Offset: 0x002C95D0
		private void SetEmptyPalace(int index, TravelerSkillsData skillData)
		{
			TravelerStation refers = this.travelerStations[index];
			refers.AreaId = -1;
			this.SetPalaceActive(refers, true);
			bool canBuild = this.CanBuildPalace(skillData);
			this.SetBuildBtn(refers, canBuild);
		}

		// Token: 0x0600617D RID: 24957 RVA: 0x002CB408 File Offset: 0x002C9608
		private bool CanBuildPalace(TravelerSkillsData skillData)
		{
			short currentAreaId = this.areaMap.MapModel.CurrentAreaId;
			bool flag = currentAreaId < 0 || currentAreaId >= 135;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				for (int i = 0; i < skillData.PalaceCount; i++)
				{
					TravelerPalaceData travelerPalaceData = skillData.TryGetPalaceData(i);
					Location? location = (travelerPalaceData != null) ? new Location?(travelerPalaceData.Location) : null;
					Location currentLocation = this.MapModel.CurrentLocation;
					bool flag2 = location != null && (location == null || location.GetValueOrDefault() == currentLocation);
					if (flag2)
					{
						return false;
					}
				}
				result = true;
			}
			return result;
		}

		// Token: 0x0600617E RID: 24958 RVA: 0x002CB4C0 File Offset: 0x002C96C0
		private void OnConfirmBuild()
		{
			bool flag = this.MapModel.CurrentAreaId < 0;
			if (!flag)
			{
				short templateId = this.MapModel.Areas[(int)this.MapModel.CurrentAreaId].GetTemplateId();
				this.areaMap.LookAtTemplate(templateId, 0.3f, default(Vector2));
				this.areaMap.OnSelectAreaTemplateId = new Action<short>(ViewProfessionTravelerStation.Nop);
				this.areaMap.SelectedAreaTemplateId = templateId;
				this.areaMap.OnSelectAreaTemplateId = null;
				SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(0.6f, delegate
				{
					ProfessionSkillArg professionSkillArg = new ProfessionSkillArg
					{
						ProfessionId = 11,
						SkillId = 47,
						IsSuccess = true
					};
					UIElement.ProfessionSkillConfirm.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("ProfessionSkillArg", professionSkillArg).SetObject("OnConfirm", new Action(this.<OnConfirmBuild>g__Confirm|20_1)));
					UIManager.Instance.MaskUI(UIElement.ProfessionSkillConfirm);
				});
			}
		}

		// Token: 0x0600617F RID: 24959 RVA: 0x002CB569 File Offset: 0x002C9769
		private static void Nop(short _)
		{
		}

		// Token: 0x06006180 RID: 24960 RVA: 0x002CB56C File Offset: 0x002C976C
		private void SetPalaceActive(TravelerStation refers, bool isEmpty)
		{
			refers.focusBtn.interactable = !isEmpty;
			refers.focusBtn.onClick.ResetListener(delegate()
			{
				bool flag2 = refers.AreaId != -1;
				if (flag2)
				{
					this.areaMap.LookAt(refers.AreaId, 0.3f, default(Vector2));
					this.areaMap.OnSelectAreaTemplateId = new Action<short>(ViewProfessionTravelerStation.Nop);
					this.areaMap.SelectedAreaTemplateId = this.MapModel.Areas[(int)refers.AreaId].GetTemplateId();
					this.areaMap.OnSelectAreaTemplateId = null;
				}
			});
			refers.buildBtn.gameObject.SetActive(isEmpty);
			refers.moveBtn.gameObject.SetActive(!isEmpty);
			refers.editNameButton.gameObject.SetActive(!isEmpty);
			refers.removeBtn.gameObject.SetActive(!isEmpty);
			refers.blockHolder.gameObject.SetActive(!isEmpty);
			refers.nameTitle.transform.parent.gameObject.SetActive(!isEmpty);
			bool flag = refers.AreaId != -1 && !isEmpty;
			if (flag)
			{
				refers.mapEffect.Target = this.areaMap.GetTransform(this.MapModel.Areas[(int)refers.AreaId].GetTemplateId());
				refers.mapEffect.gameObject.SetActive(true);
			}
			else
			{
				refers.mapEffect.gameObject.SetActive(false);
			}
		}

		// Token: 0x06006181 RID: 24961 RVA: 0x002CB6EA File Offset: 0x002C98EA
		private string GetPalaceName(TravelerPalaceData palaceData)
		{
			return palaceData.CustomName.IsNullOrEmpty() ? ViewProfessionTravelerStation.GetPalaceDefaultName(this.MapModel, palaceData) : palaceData.CustomName;
		}

		// Token: 0x06006182 RID: 24962 RVA: 0x002CB710 File Offset: 0x002C9910
		public static string GetPalaceDefaultName(WorldMapModel mapModel, TravelerPalaceData palaceData)
		{
			MapAreaData areaData = mapModel.Areas[(int)palaceData.Location.AreaId];
			short templateId = areaData.GetTemplateId();
			return LocalStringManager.GetFormat(LanguageKey.LK_ProfessionTravelerStation_Text2, MapArea.Instance[templateId].Name);
		}

		// Token: 0x06006183 RID: 24963 RVA: 0x002CB756 File Offset: 0x002C9956
		private void RenameConfirm(string text, TMP_Text nameTitle, int index, TravelerPalaceData palaceData)
		{
			nameTitle.SetText(text.IsNullOrEmpty() ? this.GetPalaceName(palaceData) : text, true);
			MapDomainMethod.Call.ChangeTravelerPalaceName(this.Element.GameDataListenerId, index, text);
		}

		// Token: 0x06006187 RID: 24967 RVA: 0x002CB955 File Offset: 0x002C9B55
		[CompilerGenerated]
		private void <OnConfirmBuild>g__Confirm|20_1()
		{
			AudioManager.Instance.StopLoopSoundWithAmbience("SFX_professionskill_lvren_loop");
			AudioManager.Instance.PlayLoopSoundWithAmbience("SFX_professionskill_lvren_loop");
			this.QuickHide();
		}

		// Token: 0x04004395 RID: 17301
		private const float MoveDuration = 0.3f;

		// Token: 0x04004396 RID: 17302
		private const float PopupDelay = 0.6f;

		// Token: 0x04004397 RID: 17303
		[SerializeField]
		private TravelerStation[] travelerStations;

		// Token: 0x04004398 RID: 17304
		[SerializeField]
		private AreaMap areaMap;

		// Token: 0x04004399 RID: 17305
		public ProfessionModel ProfessionModel;

		// Token: 0x0400439A RID: 17306
		public WorldMapModel MapModel;

		// Token: 0x0400439B RID: 17307
		private short[] _palaceAreaIds = new short[3];

		// Token: 0x0400439C RID: 17308
		private short _currHealth;

		// Token: 0x0400439D RID: 17309
		private short _leftMaxHealth;
	}
}
