using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Global;
using GameData.Domains.Map;
using GameData.Domains.World;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.Bottom
{
	// Token: 0x02000C46 RID: 3142
	public class TimeBall : MonoBehaviour
	{
		// Token: 0x06009FAF RID: 40879 RVA: 0x004A9850 File Offset: 0x004A7A50
		public void Init(UIBase uiBase)
		{
			this.parent = uiBase;
			this.tip.Type = TipType.Advance;
			this.tip.RuntimeParam = new ArgumentBox();
			this.tip.enabled = true;
		}

		// Token: 0x06009FB0 RID: 40880 RVA: 0x004A9885 File Offset: 0x004A7A85
		private void ResetAdvanceMonthState()
		{
			this._advanceActionPointConfirmed = false;
			this._advanceLoopingNeigongConfirmed = false;
			this._advanceInventoryOverflowConfirmed = false;
			this.IsProcessing = false;
		}

		// Token: 0x06009FB1 RID: 40881 RVA: 0x004A98A4 File Offset: 0x004A7AA4
		private void Awake()
		{
			this.timeBallEffect.InitEffect();
			this.timeBallButton.onClick.ResetListener(delegate()
			{
				this.RequestAdvanceMonth(null);
			});
			this._advanceDialogCmd.No = new Action(this.ResetAdvanceMonthState);
			GEvent.Add(EEvents.SetAdvanceMonthLock, new GEvent.Callback(this.SetAdvanceMonthLock));
		}

		// Token: 0x06009FB2 RID: 40882 RVA: 0x004A990B File Offset: 0x004A7B0B
		private void OnDestroy()
		{
			GEvent.Remove(EEvents.SetAdvanceMonthLock, new GEvent.Callback(this.SetAdvanceMonthLock));
		}

		// Token: 0x06009FB3 RID: 40883 RVA: 0x004A9927 File Offset: 0x004A7B27
		private void SetAdvanceMonthLock(ArgumentBox argBox)
		{
			argBox.Get<DialogCmd>("Reason", out this._advanceMonthLock);
		}

		// Token: 0x06009FB4 RID: 40884 RVA: 0x004A993C File Offset: 0x004A7B3C
		private void OnEnable()
		{
			GEvent.Add(EEvents.OnMonthChange, new GEvent.Callback(this.OnMonthChange));
			GEvent.Add(EEvents.RequestAdvanceMonth, new GEvent.Callback(this.RequestAdvanceMonth));
			GEvent.Add(EEvents.OnActionPointChange, new GEvent.Callback(this.UpdateDate));
			GEvent.Add(UiEvents.ShowAdvanceMonthConfirm, new GEvent.Callback(this.OnClickAdvanceMonth));
			GEvent.Add(UiEvents.RequestBottomTimeDisk, new GEvent.Callback(this.OnRequestBottomTimeDisk));
			GEvent.Add(UiEvents.AdventureBanTimeBall, new GEvent.Callback(this.AdventureBanTimeBall));
			this.UpdateDate(null);
		}

		// Token: 0x06009FB5 RID: 40885 RVA: 0x004A99F0 File Offset: 0x004A7BF0
		private void OnDisable()
		{
			GEvent.Remove(EEvents.OnMonthChange, new GEvent.Callback(this.OnMonthChange));
			GEvent.Remove(EEvents.RequestAdvanceMonth, new GEvent.Callback(this.RequestAdvanceMonth));
			GEvent.Remove(EEvents.OnActionPointChange, new GEvent.Callback(this.UpdateDate));
			GEvent.Remove(UiEvents.ShowAdvanceMonthConfirm, new GEvent.Callback(this.OnClickAdvanceMonth));
			GEvent.Remove(UiEvents.RequestBottomTimeDisk, new GEvent.Callback(this.OnRequestBottomTimeDisk));
			GEvent.Remove(UiEvents.AdventureBanTimeBall, new GEvent.Callback(this.AdventureBanTimeBall));
		}

		// Token: 0x06009FB6 RID: 40886 RVA: 0x004A9A99 File Offset: 0x004A7C99
		private void OnRequestBottomTimeDisk(ArgumentBox box)
		{
			this.timeBallButton.interactable = false;
			GEvent.OnEvent(UiEvents.ResponseBottomTimeDisk, (box ?? EasyPool.Get<ArgumentBox>()).SetObject("timeText", base.transform as RectTransform));
		}

		// Token: 0x06009FB7 RID: 40887 RVA: 0x004A9AD8 File Offset: 0x004A7CD8
		public void EnableButton(bool enable)
		{
			this.timeBallButton.interactable = enable;
		}

		// Token: 0x06009FB8 RID: 40888 RVA: 0x004A9AE8 File Offset: 0x004A7CE8
		public void UpdateDate(ArgumentBox _)
		{
			int actionPointCurrMonth = SingletonObject.getInstance<BasicGameData>().ActionPointCurrMonth;
			bool flag = actionPointCurrMonth < GuidingChapterTrigger.DefValue.TimeBallAcuPointLessThan10.Int1 && !this._achievementLt10;
			if (flag)
			{
				this._achievementLt10 = true;
				GlobalDomainMethod.Call.InvokeGuidingTrigger(277);
			}
			bool flag2 = actionPointCurrMonth < GuidingChapterTrigger.DefValue.TimeBallAcuPointLessThan5.Int1 && !this._achievementLt5;
			if (flag2)
			{
				this._achievementLt5 = true;
				GlobalDomainMethod.Call.InvokeGuidingTrigger(276);
			}
			this.leftDay.text = string.Format("{0}", actionPointCurrMonth / 10);
			this.timeBallEffect.SetParticleFillAmount((float)actionPointCurrMonth / (float)TimeManager.ActionPointMax);
			this.ResetAdvanceMonthState();
		}

		// Token: 0x06009FB9 RID: 40889 RVA: 0x004A9B9E File Offset: 0x004A7D9E
		private void OnMonthChange(ArgumentBox _)
		{
			this.ResetAdvanceMonthState();
		}

		// Token: 0x06009FBA RID: 40890 RVA: 0x004A9BA8 File Offset: 0x004A7DA8
		public void OnClickAdvanceMonth(ArgumentBox argBox)
		{
			this.IsProcessing = true;
			bool flag = WorldMapModel.Traveling || UIElement.PartWorld.Exist;
			if (!flag)
			{
				TutorialChapterModel tutorialChapterModel = SingletonObject.getInstance<TutorialChapterModel>();
				bool inGuiding = tutorialChapterModel.InGuiding;
				if (inGuiding)
				{
					bool advanceMonthEnable = tutorialChapterModel.AdvanceMonthEnable;
					if (advanceMonthEnable)
					{
						this.AdvanceMonth();
					}
				}
				else
				{
					Action <>9__2;
					Action <>9__1;
					Action <>9__3;
					WorldDomainMethod.AsyncCall.GetAdvanceMonthSoftConditions(this.parent, delegate(int offset, RawDataPool pool)
					{
						AdvanceMonthConditionsDisplayData data = new AdvanceMonthConditionsDisplayData();
						Serializer.Deserialize(pool, offset, ref data);
						this._advanceDialogCmd.Title = LanguageKey.UI_AdvanceMonth_TipTitle.Tr();
						bool flag2 = !this._advanceActionPointConfirmed && data.HasExtraMovePoints;
						if (flag2)
						{
							int leftDays = SingletonObject.getInstance<TimeManager>().GetRemainingActionPointConvertToDays();
							int recovery = (TimeManager.ActionPointRecovery + data.EnergyBonus) / 10;
							UIElement viewConfirmDialogLayoutSmall = UIElement.ViewConfirmDialogLayoutSmall;
							ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>();
							string key = "Cmd";
							ConfirmDialogCmd confirmDialogCmd = new ConfirmDialogCmd();
							confirmDialogCmd.Title = LanguageKey.UI_AdvanceMonth_TipTitle.Tr();
							confirmDialogCmd.ContentUpper = (LanguageKey.LK_AdvanceMonth_LeftActionPoint.Tr() + "\n" + LanguageKey.LK_Advance_Month_Confirm.Tr()).ColorReplace();
							confirmDialogCmd.ChangeInfos = new List<ChangeInfo>
							{
								new ChangeInfo
								{
									Type = EConfirmDialogCostType.ActionPoint,
									fromValue = leftDays,
									toValue = Mathf.Min(TimeManager.ActionPointMax / 10, leftDays + recovery)
								}
							};
							Action yes;
							if ((yes = <>9__2) == null)
							{
								yes = (<>9__2 = delegate()
								{
									this._advanceActionPointConfirmed = true;
									GEvent.OnEvent(UiEvents.ShowAdvanceMonthConfirm, argBox);
								});
							}
							confirmDialogCmd.Yes = yes;
							confirmDialogCmd.No = new Action(this.ResetAdvanceMonthState);
							confirmDialogCmd.ValueStyle = 1;
							viewConfirmDialogLayoutSmall.SetOnInitArgs(argumentBox.SetObject(key, confirmDialogCmd));
							UIManager.Instance.MaskUI(UIElement.ViewConfirmDialogLayoutSmall);
						}
						else
						{
							bool flag3 = !this._advanceLoopingNeigongConfirmed && data.CanLoopingNeigong;
							if (flag3)
							{
								this._advanceDialogCmd.Content = LocalStringManager.Get(LanguageKey.LK_Advance_Month_Warn_No_Loop_Neigong).ColorReplace() + "\n" + LocalStringManager.Get(LanguageKey.LK_Advance_Month_Confirm);
								DialogCmd advanceDialogCmd = this._advanceDialogCmd;
								Action yes2;
								if ((yes2 = <>9__1) == null)
								{
									yes2 = (<>9__1 = delegate()
									{
										this._advanceLoopingNeigongConfirmed = true;
										GEvent.OnEvent(UiEvents.ShowAdvanceMonthConfirm, argBox);
									});
								}
								advanceDialogCmd.Yes = yes2;
								UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", this._advanceDialogCmd));
								UIManager.Instance.MaskUI(UIElement.Dialog);
							}
							else
							{
								bool flag4 = !this._advanceInventoryOverflowConfirmed;
								if (flag4)
								{
									StringBuilder strBuilder = EasyPool.Get<StringBuilder>();
									strBuilder.Clear();
									bool inventoryOverload = data.InventoryOverload;
									if (inventoryOverload)
									{
										strBuilder.Append(LanguageKey.LK_Advance_Month_Warn_Inventory_Overflow.Tr().ColorReplace() + "\n");
									}
									bool warehouseOverload = data.WarehouseOverload;
									if (warehouseOverload)
									{
										strBuilder.Append(LanguageKey.LK_Advance_Month_Warn_Warehouse_Overflow.Tr().ColorReplace() + "\n");
									}
									bool flag5 = strBuilder.Length > 0;
									if (flag5)
									{
										strBuilder.Append(LanguageKey.LK_Advance_Month_Warn_Overflow.Tr().ColorReplace());
										this._advanceDialogCmd.Content = string.Format("{0}\n{1}", strBuilder, LanguageKey.LK_Advance_Month_Confirm.Tr());
										DialogCmd advanceDialogCmd2 = this._advanceDialogCmd;
										Action yes3;
										if ((yes3 = <>9__3) == null)
										{
											yes3 = (<>9__3 = delegate()
											{
												this._advanceInventoryOverflowConfirmed = true;
												GEvent.OnEvent(UiEvents.ShowAdvanceMonthConfirm, argBox);
											});
										}
										advanceDialogCmd2.Yes = yes3;
										UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", this._advanceDialogCmd));
										UIManager.Instance.MaskUI(UIElement.Dialog);
										EasyPool.Free<StringBuilder>(strBuilder);
										return;
									}
									EasyPool.Free<StringBuilder>(strBuilder);
								}
								AudioManager.Instance.PlaySound(string.Format("SFX_tex_month_{0}", SingletonObject.getInstance<TimeManager>().GetMonthInCurrYear()), false, false);
								Action advanceAction;
								bool flag6 = argBox != null && argBox.Get<Action>("callback", out advanceAction) && advanceAction != null;
								if (flag6)
								{
									advanceAction();
								}
								else
								{
									this.AdvanceMonth();
								}
							}
						}
					});
				}
			}
		}

		// Token: 0x06009FBB RID: 40891 RVA: 0x004A9C2C File Offset: 0x004A7E2C
		private void RequestAdvanceMonth(ArgumentBox argBox)
		{
			bool flag = UIManager.Instance.IsElementActive(UIElement.AdventureMajorEvent) || UIManager.Instance.IsElementActive(UIElement.PartWorld);
			if (!flag)
			{
				bool atPastTaiwuVillage = SingletonObject.getInstance<WorldMapModel>().AtPastTaiwuVillage;
				if (atPastTaiwuVillage)
				{
					bool savingWorld = SingletonObject.getInstance<BasicGameData>().SavingWorld;
					if (!savingWorld)
					{
						int leftDays = SingletonObject.getInstance<TimeManager>().GetRemainingActionPointConvertToDays();
						int recovery = TimeManager.ActionPointRecovery / 10;
						UIElement.ViewConfirmDialogLayoutSmall.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", new ConfirmDialogCmd
						{
							Title = LanguageKey.UI_AdvanceMonth_TipTitle.Tr(),
							ContentUpper = (LanguageKey.LK_AdvanceMonth_LeftActionPoint.Tr() + "\n" + LanguageKey.LK_Advance_Month_Confirm.Tr()).ColorReplace(),
							ChangeInfos = new List<ChangeInfo>
							{
								new ChangeInfo
								{
									Type = EConfirmDialogCostType.ActionPoint,
									fromValue = leftDays,
									toValue = Mathf.Min(TimeManager.ActionPointMax / 10, leftDays + recovery)
								}
							},
							Yes = delegate()
							{
								this._advanceActionPointConfirmed = true;
								GlobalOperations.SaveWorld();
							},
							No = new Action(this.ResetAdvanceMonthState),
							ValueStyle = 1
						}));
						UIManager.Instance.MaskUI(UIElement.ViewConfirmDialogLayoutSmall);
					}
				}
				else
				{
					bool flag2 = this._advanceMonthLock != null;
					if (flag2)
					{
						UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", this._advanceMonthLock));
						UIManager.Instance.MaskUI(UIElement.Dialog);
					}
					else
					{
						TutorialChapterModel tutorialChapterModel = SingletonObject.getInstance<TutorialChapterModel>();
						bool inGuiding = tutorialChapterModel.InGuiding;
						if (inGuiding)
						{
							bool advanceMonthEnable = tutorialChapterModel.AdvanceMonthEnable;
							if (advanceMonthEnable)
							{
								this.AdvanceMonth();
							}
						}
						else
						{
							bool adventureBan = this._adventureBan;
							if (!adventureBan)
							{
								MapDomainMethod.AsyncCall.IsContinuousMovingBreak(null, delegate(int offset, RawDataPool dataPoll)
								{
									bool hasEvent = false;
									Serializer.Deserialize(dataPoll, offset, ref hasEvent);
									bool flag3 = !hasEvent;
									if (flag3)
									{
										GEvent.OnEvent(UiEvents.ShowAdvanceMonthConfirm, null);
									}
								});
							}
						}
					}
				}
			}
		}

		// Token: 0x06009FBC RID: 40892 RVA: 0x004A9E20 File Offset: 0x004A8020
		public void AdvanceMonth()
		{
			GlobalDomainMethod.AsyncCall.CheckDriveSpace(this.parent, delegate(int offset, RawDataPool dataPool)
			{
				bool hasSpace = false;
				Serializer.Deserialize(dataPool, offset, ref hasSpace);
				bool flag = hasSpace;
				if (flag)
				{
					this.<AdvanceMonth>g__Action|30_1();
				}
				else
				{
					string archiveDirPath = GameApp.GetArchiveDirPath();
					string disk = Path.GetPathRoot(archiveDirPath);
					string title = LanguageKey.LK_Save_CheckDiskSpace_Title.Tr();
					string content = LanguageKey.LK_Save_CheckDiskSpace_Content.TrFormat(disk).ColorReplace();
					CommonUtils.ShowConfirmDialog(title, content, new Action(this.<AdvanceMonth>g__Action|30_1), null, EDialogType.None);
				}
			});
		}

		// Token: 0x06009FBD RID: 40893 RVA: 0x004A9E3D File Offset: 0x004A803D
		private void AdventureBanTimeBall(ArgumentBox argBox)
		{
			argBox.Get("AdventureBan", out this._adventureBan);
			this.EnableButton(!this._adventureBan);
		}

		// Token: 0x06009FBE RID: 40894 RVA: 0x004A9E62 File Offset: 0x004A8062
		public void EnableTimeBall(bool enable)
		{
			this.EnableButton(enable);
			this.self.localScale = Vector3.one;
			this.self.anchoredPosition = new Vector2(0f, 0f);
		}

		// Token: 0x06009FC3 RID: 40899 RVA: 0x004A9F50 File Offset: 0x004A8150
		[CompilerGenerated]
		private void <AdvanceMonth>g__Action|30_1()
		{
			bool flag = !this._timeBallPassMonthWithAcuPointRemain && SingletonObject.getInstance<BasicGameData>().ActionPointCurrMonth >= GuidingChapterTrigger.DefValue.TimeBallPassMonthWithAcuPointRemain.Int1;
			if (flag)
			{
				this._timeBallPassMonthWithAcuPointRemain = true;
				GlobalDomainMethod.Call.InvokeGuidingTrigger(311);
			}
			WorldDomainMethod.Call.AdvanceMonth();
			GameApp.AdvancingMonth = true;
		}

		// Token: 0x04007B90 RID: 31632
		[SerializeField]
		private RectTransform self;

		// Token: 0x04007B91 RID: 31633
		[SerializeField]
		private TooltipInvoker tip;

		// Token: 0x04007B92 RID: 31634
		[SerializeField]
		private TMP_Text leftDay;

		// Token: 0x04007B93 RID: 31635
		[SerializeField]
		private UIBase parent;

		// Token: 0x04007B94 RID: 31636
		[SerializeField]
		private CButton timeBallButton;

		// Token: 0x04007B95 RID: 31637
		[SerializeField]
		private TimeBallEffect timeBallEffect;

		// Token: 0x04007B96 RID: 31638
		private bool _achievementLt5;

		// Token: 0x04007B97 RID: 31639
		private bool _achievementLt10;

		// Token: 0x04007B98 RID: 31640
		private bool _timeBallPassMonthWithAcuPointRemain;

		// Token: 0x04007B99 RID: 31641
		public bool IsProcessing;

		// Token: 0x04007B9A RID: 31642
		private readonly DialogCmd _advanceDialogCmd = new DialogCmd();

		// Token: 0x04007B9B RID: 31643
		private bool _advanceActionPointConfirmed;

		// Token: 0x04007B9C RID: 31644
		private bool _advanceLoopingNeigongConfirmed;

		// Token: 0x04007B9D RID: 31645
		private bool _advanceInventoryOverflowConfirmed;

		// Token: 0x04007B9E RID: 31646
		private DialogCmd _advanceMonthLock;

		// Token: 0x04007B9F RID: 31647
		[SerializeField]
		private float baseFill = 0.10691824f;

		// Token: 0x04007BA0 RID: 31648
		[SerializeField]
		private float deltaFill = 0.7893082f;

		// Token: 0x04007BA1 RID: 31649
		private bool _adventureBan;
	}
}
