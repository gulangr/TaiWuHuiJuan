using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Coffee.UIExtensions;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using Game.Views.Combat.Migrate;
using GameData.Domains.Character.Display;
using GameData.Domains.Combat;
using GameData.Domains.Item;
using GameData.Utilities;
using TMPro;
using UICommon.Character;
using UnityEngine;

namespace Game.Views.Combat
{
	// Token: 0x02000B33 RID: 2867
	public class CombatTeammate : MonoBehaviour, ICombatComponent
	{
		// Token: 0x17000F77 RID: 3959
		// (get) Token: 0x06008C60 RID: 35936 RVA: 0x0040D8C1 File Offset: 0x0040BAC1
		private CombatModel Model
		{
			get
			{
				return SingletonObject.getInstance<CombatModel>();
			}
		}

		// Token: 0x06008C61 RID: 35937 RVA: 0x0040D8C8 File Offset: 0x0040BAC8
		private void Awake()
		{
			this._characterAvatar = new CharacterAvatar(this.avatar, true);
			this.button.ClearAndAddListener(delegate
			{
				IReadOnlyList<int> team = this.ally ? this.Model.SelfTeam : this.Model.EnemyTeam;
				bool flag2 = this.index + 1 >= team.Count;
				if (!flag2)
				{
					CombatUtils.ShowCharMenu(team[this.index + 1]);
				}
			});
			PointerTrigger pointerTrigger = this.button.GetComponent<PointerTrigger>();
			pointerTrigger.EnterEvent.RemoveAllListeners();
			pointerTrigger.EnterEvent.AddListener(delegate()
			{
				this.highLight.SetActive(true);
			});
			pointerTrigger.ExitEvent.RemoveAllListeners();
			pointerTrigger.ExitEvent.AddListener(delegate()
			{
				this.highLight.SetActive(false);
			});
			bool flag = this.ally;
			if (flag)
			{
				for (int i = 0; i < this.commandHolder.childCount; i++)
				{
					CombatTeammateCommand cmdRefers = this.commandHolder.GetChild(i).GetComponent<CombatTeammateCommand>();
					CButton cmdBtn = cmdRefers.button;
					PointerTrigger cmdPointerTrigger = cmdBtn.GetComponent<PointerTrigger>();
					CImage highLightImg = cmdRefers.highLight;
					cmdPointerTrigger.EnterEvent.AddListener(delegate()
					{
						bool interactable = cmdBtn.interactable;
						if (interactable)
						{
							highLightImg.gameObject.SetActive(true);
						}
					});
					cmdPointerTrigger.ExitEvent.AddListener(delegate()
					{
						highLightImg.gameObject.SetActive(false);
					});
				}
			}
		}

		// Token: 0x06008C62 RID: 35938 RVA: 0x0040D9F4 File Offset: 0x0040BBF4
		private void OnDataReady()
		{
			this.ResetCommands();
			IReadOnlyList<int> team = this.ally ? this.Model.SelfTeam : this.Model.EnemyTeam;
			this._characterAvatar.CharacterId = -1;
			bool flag = this.index + 1 < team.Count;
			if (flag)
			{
				int charId = team[this.index + 1];
				this._characterAvatar.CharacterId = charId;
				this.teammateName.text = NameCenter.GetMonasticTitleOrDisplayName(this.Model.DisplayDataCache[charId], false);
				this.UpdateNameString(charId);
				TooltipInvoker tip = this.button.GetComponent<TooltipInvoker>();
				CombatUtils.UpdateInjuryTips(tip, charId);
				base.gameObject.SetActive(true);
			}
			else
			{
				base.gameObject.SetActive(false);
			}
		}

		// Token: 0x06008C63 RID: 35939 RVA: 0x0040DAC8 File Offset: 0x0040BCC8
		private void OnChangeChar()
		{
			int toCharId = this.Model.ChangingToCharId;
			bool flag = !this.ShouldHandle(toCharId);
			if (!flag)
			{
				this.UpdateNameString(toCharId);
			}
		}

		// Token: 0x06008C64 RID: 35940 RVA: 0x0040DAFC File Offset: 0x0040BCFC
		private void OnTimeScaleChanged()
		{
			bool paused = this.Model.TimeScale == 0f;
			this.button.interactable = paused;
			PointerTrigger pointerTrigger = this.button.GetComponent<PointerTrigger>();
			pointerTrigger.enabled = paused;
			bool flag = paused;
			if (flag)
			{
				bool flag2 = RectTransformUtility.RectangleContainsScreenPoint(this.button.GetComponent<RectTransform>(), Input.mousePosition, UIManager.Instance.UiCamera);
				if (flag2)
				{
					pointerTrigger.EnterEvent.Invoke();
				}
			}
		}

		// Token: 0x06008C65 RID: 35941 RVA: 0x0040DB78 File Offset: 0x0040BD78
		private void OnDefeatMarkCollectionChanged(int charId, DefeatMarkCollection oldValue)
		{
			bool flag = !this.ShouldHandle(charId);
			if (!flag)
			{
				CombatSubProcessorCharacter processor = this.Model.ProcessorCharacters[charId];
				DefeatMarkCollection marks = processor.DefeatMarkCollection;
				this.defeatMark.Set(marks, processor.OldDisorderOfQi, processor.OldInjuries, processor.OldPoison);
			}
		}

		// Token: 0x06008C66 RID: 35942 RVA: 0x0040DBD0 File Offset: 0x0040BDD0
		private void OnCurrTeammateCommandsChanged(int charId)
		{
			bool flag = !this.ShouldHandle(charId);
			if (!flag)
			{
				this.UpdateTeammateCmdShow(charId);
			}
		}

		// Token: 0x06008C67 RID: 35943 RVA: 0x0040DBF8 File Offset: 0x0040BDF8
		private void OnShowTransferInjuryCommandChanged(int charId)
		{
			bool isAlly = this.Model.CharIsAlly(charId);
			bool flag = this.ally != isAlly;
			if (!flag)
			{
				IReadOnlyList<int> team = isAlly ? this.Model.SelfTeam : this.Model.EnemyTeam;
				int teammateIndex = team.IndexOf(charId) - 1;
				bool flag2 = teammateIndex != -1;
				if (!flag2)
				{
					bool flag3 = this.index + 1 >= team.Count;
					if (!flag3)
					{
						this.UpdateTeammateCmdShow(team[this.index + 1]);
					}
				}
			}
		}

		// Token: 0x06008C68 RID: 35944 RVA: 0x0040DC8C File Offset: 0x0040BE8C
		private void OnTeammateCommandBanReasonsChanged(int charId)
		{
			bool flag = !this.ShouldHandle(charId);
			if (!flag)
			{
				this.UpdateTeammateCmdShow(charId);
			}
		}

		// Token: 0x06008C69 RID: 35945 RVA: 0x0040DCB4 File Offset: 0x0040BEB4
		private void OnTeammateCommandCanUseChanged(int charId)
		{
			bool flag = !this.ShouldHandle(charId);
			if (!flag)
			{
				CombatSubProcessorCharacter processor = this.Model.ProcessorCharacters[charId];
				List<sbyte> currCmdList = processor.CurrTeammateCommands;
				List<bool> canUseList = processor.TeammateCommandCanUse;
				CombatSubProcessorCharacter orDefault = this.Model.ProcessorCharacters.GetOrDefault(this.Model.MainCharId(this.ally));
				bool showTransferInjuryCommand = orDefault != null && orDefault.ShowTransferInjuryCommand;
				for (int commandIndex = 0; commandIndex < this.commandHolder.childCount; commandIndex++)
				{
					CombatTeammateCommand cmdRefers = this.commandHolder.GetChild(commandIndex).GetComponent<CombatTeammateCommand>();
					bool flag2 = cmdRefers.gameObject.activeSelf && canUseList.Count > commandIndex;
					if (flag2)
					{
						bool canUse = processor.GetTeammateCmdCanUse(commandIndex);
						CButton cmdBtn = cmdRefers.button;
						bool flag3 = cmdBtn.interactable != canUse;
						if (flag3)
						{
							cmdBtn.interactable = canUse;
							sbyte commandType = showTransferInjuryCommand ? 13 : currCmdList[commandIndex];
							TeammateCommandItem cmdConfig = TeammateCommand.Instance[commandType];
							CombatTeammate.UpdateTeammateCommandDisplay(cmdRefers, cmdConfig, canUse, this.ally);
							cmdRefers.lockObj.SetActive(!canUse);
							bool flag4 = RectTransformUtility.RectangleContainsScreenPoint(cmdBtn.GetComponent<RectTransform>(), Input.mousePosition, UIManager.Instance.UiCamera);
							if (flag4)
							{
								cmdRefers.highLight.gameObject.SetActive(canUse && this.ally);
							}
						}
					}
				}
			}
		}

		// Token: 0x06008C6A RID: 35946 RVA: 0x0040DE48 File Offset: 0x0040C048
		private void OnTeammateCommandCdChanged(int charId)
		{
			bool flag = !this.ShouldHandle(charId);
			if (!flag)
			{
				RectTransform cmdHolder = this.commandHolder;
				CombatSubProcessorCharacter processor = this.Model.ProcessorCharacters[charId];
				List<CountdownData> cdList = processor.TeammateCommandCd;
				sbyte executingCmdType = processor.ExecutingTeammateCommand;
				for (int i = 0; i < cmdHolder.childCount; i++)
				{
					CombatTeammateCommand cmdRefers = cmdHolder.GetChild(i).GetComponent<CombatTeammateCommand>();
					bool flag2 = !cmdRefers.gameObject.activeSelf || (int)executingCmdType == cmdRefers.UserInt;
					if (!flag2)
					{
						CImage cmdMask = cmdRefers.mask;
						CountdownData cd = cdList[i];
						bool flag3 = this.ally && cmdMask.fillAmount > 0f && cd.Off;
						if (flag3)
						{
							this.PlayUiParticle(cmdRefers.activeEffect);
							this.PlayUiParticle(cmdRefers.activeEffect2);
						}
						float fill = this.GetTeammateCmdCdPercent(charId, i);
						cmdRefers.maskTimeCountDown.SetText(Mathf.CeilToInt((float)cd.Left / 10000f).ToString(), true);
						cmdRefers.maskTimeCountDown.gameObject.SetActive(cd.Left > 0);
						this.SetMaskFillAmount(cmdRefers, fill);
					}
				}
			}
		}

		// Token: 0x06008C6B RID: 35947 RVA: 0x0040DFA0 File Offset: 0x0040C1A0
		private void OnAttackCommandWeaponKeyChanged(int charId)
		{
			bool flag = !this.ShouldHandle(charId);
			if (!flag)
			{
				this.UpdateTeammateCmdTipsByImplement(charId, ETeammateCommandImplement.Attack);
				this.UpdateTeammateCmdTipsByImplement(charId, ETeammateCommandImplement.GearMateA);
			}
		}

		// Token: 0x06008C6C RID: 35948 RVA: 0x0040DFD4 File Offset: 0x0040C1D4
		private void OnTeammateCommandTimePercentChanged(int charId)
		{
			bool flag = !this.ShouldHandle(charId);
			if (!flag)
			{
				CombatSubProcessorCharacter processor = this.Model.ProcessorCharacters[charId];
				byte percent = processor.TeammateCommandTimePercent;
				sbyte executingCmdType = processor.ExecutingTeammateCommand;
				bool flag2 = executingCmdType < 0;
				if (!flag2)
				{
					int commandIndex = processor.CurrTeammateCommands.IndexOf(executingCmdType);
					CombatTeammateCommand cmdRefers = this.FindTeammateCmdRefers(executingCmdType);
					bool flag3 = cmdRefers != null;
					if (flag3)
					{
						float fill = (percent == 0) ? this.GetTeammateCmdCdPercent(charId, commandIndex) : ((float)(100 - percent) / 100f);
						this.SetMaskFillAmount(cmdRefers, fill);
					}
				}
			}
		}

		// Token: 0x06008C6D RID: 35949 RVA: 0x0040E06C File Offset: 0x0040C26C
		private void SetMaskFillAmount(CombatTeammateCommand command, float amount)
		{
			command.mask.fillAmount = amount;
			float x = command.mask.GetComponent<RectTransform>().sizeDelta.x * amount * (this.ally ? 1f : -1f);
			command.maskLine.anchoredPosition = command.maskLine.anchoredPosition.SetX(x);
		}

		// Token: 0x06008C6E RID: 35950 RVA: 0x0040E0D4 File Offset: 0x0040C2D4
		private void OnAttackCommandTrickTypeChanged(int charId)
		{
			bool flag = !this.ShouldHandle(charId);
			if (!flag)
			{
				this.UpdateTeammateCmdTipsByImplement(charId, ETeammateCommandImplement.Attack);
				this.UpdateTeammateCmdTipsByImplement(charId, ETeammateCommandImplement.GearMateA);
			}
		}

		// Token: 0x06008C6F RID: 35951 RVA: 0x0040E108 File Offset: 0x0040C308
		private void OnAttackCommandSkillIdChanged(int charId)
		{
			bool flag = !this.ShouldHandle(charId);
			if (!flag)
			{
				CombatSubProcessorCharacter processor = this.Model.ProcessorCharacters[charId];
				short attackSkillId = processor.AttackCommandSkillId;
				bool flag2 = attackSkillId < 0;
				if (!flag2)
				{
					this.UpdateTeammateCmdTipsByImplement(charId, ETeammateCommandImplement.AttackSkill);
				}
			}
		}

		// Token: 0x06008C70 RID: 35952 RVA: 0x0040E154 File Offset: 0x0040C354
		private void OnDefendCommandSkillIdChanged(int charId)
		{
			bool flag = !this.ShouldHandle(charId);
			if (!flag)
			{
				CombatSubProcessorCharacter processor = this.Model.ProcessorCharacters[charId];
				short defendSkillId = processor.DefendCommandSkillId;
				bool flag2 = defendSkillId < 0;
				if (!flag2)
				{
					this.UpdateTeammateCmdTipsByImplement(charId, ETeammateCommandImplement.Defend);
					this.UpdateTeammateCmdTipsByImplement(charId, ETeammateCommandImplement.GearMateB);
				}
			}
		}

		// Token: 0x06008C71 RID: 35953 RVA: 0x0040E1A8 File Offset: 0x0040C3A8
		private void OnExecutingTeammateCommandChanged(int charId, sbyte oldExecutingTeammateCommand)
		{
			bool flag = !this.ShouldHandle(charId);
			if (!flag)
			{
				CombatSubProcessorCharacter processor = this.Model.ProcessorCharacters[charId];
				sbyte executingCmdType = processor.ExecutingTeammateCommand;
				bool flag2 = oldExecutingTeammateCommand == executingCmdType;
				if (!flag2)
				{
					CombatTeammateCommand executingCommandRefers = this.FindTeammateCmdRefers(executingCmdType);
					bool flag3 = executingCommandRefers != null && TeammateCommand.Instance[executingCmdType].AffectFrame > 0;
					if (flag3)
					{
						executingCommandRefers.mask.SetSprite((TeammateCommand.Instance[executingCmdType].AffectFrame >= 0) ? "ui9_combat_teammate_8" : "ui9_combat_teammate_4", false, null);
					}
					bool flag4 = oldExecutingTeammateCommand >= 0;
					if (flag4)
					{
						CombatTeammateCommand originCommandRefers = this.FindTeammateCmdRefers(oldExecutingTeammateCommand);
						bool flag5 = originCommandRefers != null;
						if (flag5)
						{
							originCommandRefers.mask.SetSprite("ui9_combat_teammate_4", false, null);
						}
					}
				}
			}
		}

		// Token: 0x06008C72 RID: 35954 RVA: 0x0040E288 File Offset: 0x0040C488
		private float GetTeammateCmdCdPercent(int teammateId, int index1)
		{
			CombatSubProcessorCharacter processor;
			bool flag = !this.Model.ProcessorCharacters.TryGetValue(teammateId, out processor);
			float result;
			if (flag)
			{
				result = 0f;
			}
			else
			{
				result = processor.TeammateCommandCd[index1].Progress;
			}
			return result;
		}

		// Token: 0x06008C73 RID: 35955 RVA: 0x0040E2D0 File Offset: 0x0040C4D0
		private void UpdateTeammateCmdShow(int charId)
		{
			CombatSubProcessorCharacter processor = this.Model.ProcessorCharacters[charId];
			List<sbyte> currCmdList = processor.CurrTeammateCommands;
			List<SByteList> cmdBanReasonList = processor.TeammateCommandBanReasons;
			sbyte executingTeammateCommand = processor.ExecutingTeammateCommand;
			CombatSubProcessorCharacter orDefault = this.Model.ProcessorCharacters.GetOrDefault(this.Model.MainCharId(this.ally));
			bool showTransferInjuryCommand = orDefault != null && orDefault.ShowTransferInjuryCommand;
			int cmdCount = Math.Min((currCmdList != null) ? currCmdList.Count : 0, (cmdBanReasonList != null) ? cmdBanReasonList.Count : 0);
			for (int commandIndex = 0; commandIndex < cmdCount; commandIndex++)
			{
				sbyte cmdType = showTransferInjuryCommand ? 13 : currCmdList[commandIndex];
				CombatTeammateCommand cmdRefers = this.commandHolder.GetChild(commandIndex).GetComponent<CombatTeammateCommand>();
				cmdRefers.gameObject.SetActive(cmdType >= 0 && (!showTransferInjuryCommand || commandIndex == 0));
				bool flag = !cmdRefers.gameObject.activeSelf;
				if (!flag)
				{
					TeammateCommandItem cmdConfig = TeammateCommand.Instance[cmdType];
					CButton cmdBtn = cmdRefers.button;
					bool canUse = processor.GetTeammateCmdCanUse(commandIndex);
					cmdRefers.UserInt = (int)cmdType;
					cmdRefers.mask.SetSprite((cmdConfig.AffectFrame >= 0 && executingTeammateCommand == cmdType) ? "ui9_combat_teammate_8" : "ui9_combat_teammate_4", false, null);
					CombatTeammate.UpdateTeammateCommandDisplay(cmdRefers, cmdConfig, canUse, this.ally);
					cmdBtn.interactable = canUse;
					cmdRefers.lockObj.SetActive(!canUse);
					bool flag2 = this.ally;
					if (flag2)
					{
						cmdBtn.onClick.RemoveAllListeners();
						int commandIndex1 = commandIndex;
						int mainSelf = this.Model.SelfTeam[0];
						CombatSubProcessorCharacter mainSelfProcessor = this.Model.ProcessorCharacters[mainSelf];
						cmdBtn.onClick.AddListener(delegate()
						{
							bool flag3 = mainSelfProcessor.CombatReserveData.TeammateCharId == charId && mainSelfProcessor.CombatReserveData.TeammateCmdIndex == commandIndex1;
							if (flag3)
							{
								CombatDomainMethod.Call.ClearAllReserveAction();
							}
							else
							{
								this.Model.DoRequestUseTeammateCommand(commandIndex1, charId);
							}
						});
					}
					this.UpdateTeammateCmdTipsByIndex(charId, commandIndex);
				}
			}
			for (int i = cmdCount; i < this.commandHolder.childCount; i++)
			{
				CombatTeammateCommand cmdRefers2 = this.commandHolder.GetChild(i).GetComponent<CombatTeammateCommand>();
				cmdRefers2.gameObject.SetActive(false);
			}
		}

		// Token: 0x06008C74 RID: 35956 RVA: 0x0040E540 File Offset: 0x0040C740
		private void UpdateTeammateCmdTipsByImplement(int charId, ETeammateCommandImplement implement)
		{
			CombatSubProcessorCharacter processor = this.Model.ProcessorCharacters[charId];
			int commandIndex = processor.FindTeammateCmdIndex(implement);
			bool flag = commandIndex < 0;
			if (!flag)
			{
				this.UpdateTeammateCmdTipsByIndex(charId, commandIndex);
			}
		}

		// Token: 0x06008C75 RID: 35957 RVA: 0x0040E57C File Offset: 0x0040C77C
		private void UpdateTeammateCmdTipsByIndex(int charId, int commandIndex)
		{
			CombatSubProcessorCharacter processor = this.Model.ProcessorCharacters[charId];
			List<sbyte> cmdTypes = processor.CurrTeammateCommands;
			List<SByteList> commandBanReasons = processor.TeammateCommandBanReasons;
			bool flag = cmdTypes == null || commandBanReasons == null;
			if (!flag)
			{
				bool flag2 = !cmdTypes.CheckIndex(commandIndex) || !commandBanReasons.CheckIndex(commandIndex);
				if (!flag2)
				{
					CombatSubProcessorCharacter orDefault = this.Model.ProcessorCharacters.GetOrDefault(this.Model.MainCharId(this.ally));
					sbyte cmdType = (orDefault != null && orDefault.ShowTransferInjuryCommand) ? 13 : cmdTypes[commandIndex];
					TeammateCommandItem cmdConfig = (cmdType < 0) ? null : TeammateCommand.Instance[cmdType];
					bool flag3 = cmdConfig == null;
					if (!flag3)
					{
						CombatTeammateCommand cmdRefers = this.FindTeammateCmdRefers(cmdType);
						bool flag4 = cmdRefers == null;
						if (!flag4)
						{
							SByteList cmdBanReason = commandBanReasons[commandIndex];
							ETeammateCommandImplement cmdImplement = cmdConfig.Implement;
							StringBuilder builder = EasyPool.Get<StringBuilder>();
							builder.Clear();
							builder.Append(cmdConfig.Description.ColorReplace());
							ItemKey attackWeapon = processor.AttackCommandWeaponKey;
							sbyte attackTrick = processor.AttackCommandTrickType;
							short attackSkillId = processor.AttackCommandSkillId;
							short defendSkillId = processor.DefendCommandSkillId;
							bool flag5 = cmdImplement.IsAttack() && attackWeapon.IsValid() && attackTrick >= 0;
							if (flag5)
							{
								builder.Append("\n\n" + Weapon.Instance[attackWeapon.TemplateId].Name + "-" + Config.TrickType.Instance[attackTrick].Name);
							}
							else
							{
								bool flag6 = cmdImplement == ETeammateCommandImplement.AttackSkill && attackSkillId >= 0;
								if (flag6)
								{
									builder.Append("\n\n" + CombatSkill.Instance[attackSkillId].Name);
								}
								else
								{
									bool flag7 = cmdImplement.IsDefend() && defendSkillId >= 0;
									if (flag7)
									{
										builder.Append("\n\n" + CombatSkill.Instance[defendSkillId].Name);
									}
								}
							}
							List<sbyte> items = cmdBanReason.Items;
							bool flag8 = items != null && items.Count > 0 && cmdConfig.Type != ETeammateCommandType.Negative;
							if (flag8)
							{
								builder.Append("\n\n");
								foreach (sbyte banReasonType in cmdBanReason.Items)
								{
									ETeammateCommandBanReason eBanReasonType = (ETeammateCommandBanReason)banReasonType;
									builder.Append(LocalStringManager.Get("LK_Combat_TeammateCommandBanReason_" + eBanReasonType.ToString()).SetColor("brightred"));
									builder.Append('\n');
								}
								builder.Remove(builder.Length - 1, 1);
							}
							string tipsText = builder.ToString();
							EasyPool.Free<StringBuilder>(builder);
							TooltipInvoker mouseTip = cmdRefers.button.GetComponent<TooltipInvoker>();
							mouseTip.PresetParam[0] = cmdConfig.Name;
							mouseTip.PresetParam[1] = tipsText;
						}
					}
				}
			}
		}

		// Token: 0x06008C76 RID: 35958 RVA: 0x0040E8A0 File Offset: 0x0040CAA0
		private CombatTeammateCommand FindTeammateCmdRefers(sbyte cmdType)
		{
			bool flag = cmdType < 0;
			CombatTeammateCommand result;
			if (flag)
			{
				result = null;
			}
			else
			{
				for (int i = 0; i < this.commandHolder.childCount; i++)
				{
					CombatTeammateCommand cmdRefers = this.commandHolder.GetChild(i).GetComponent<CombatTeammateCommand>();
					bool flag2 = cmdRefers.gameObject.activeSelf && cmdRefers.UserInt == (int)cmdType;
					if (flag2)
					{
						return cmdRefers;
					}
				}
				result = null;
			}
			return result;
		}

		// Token: 0x06008C77 RID: 35959 RVA: 0x0040E914 File Offset: 0x0040CB14
		private CombatTeammateCommand FindTeammateCmdRefers(ETeammateCommandImplement implement)
		{
			for (int i = 0; i < this.commandHolder.childCount; i++)
			{
				CombatTeammateCommand cmdRefers = this.commandHolder.GetChild(i).GetComponent<CombatTeammateCommand>();
				ETeammateCommandImplement cmdImplement = (cmdRefers.UserInt < 0) ? ETeammateCommandImplement.Invalid : TeammateCommand.Instance[cmdRefers.UserInt].Implement;
				bool flag = cmdRefers.gameObject.activeSelf && cmdImplement == implement;
				if (flag)
				{
					return cmdRefers;
				}
			}
			return null;
		}

		// Token: 0x06008C78 RID: 35960 RVA: 0x0040E998 File Offset: 0x0040CB98
		private void ResetCommands()
		{
			for (int i = 0; i < this.commandHolder.childCount; i++)
			{
				this.commandHolder.GetChild(i).gameObject.SetActive(false);
			}
		}

		// Token: 0x06008C79 RID: 35961 RVA: 0x0040E9D8 File Offset: 0x0040CBD8
		private bool ShouldHandle(int charId)
		{
			bool isAlly = this.Model.CharIsAlly(charId);
			bool flag = isAlly != this.ally;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				IReadOnlyList<int> team = isAlly ? this.Model.SelfTeam : this.Model.EnemyTeam;
				int teammateIndex = team.IndexOf(charId) - 1;
				bool flag2 = teammateIndex != this.index;
				if (flag2)
				{
					result = false;
				}
				else
				{
					bool flag3 = !this.Model.ProcessorCharacters.ContainsKey(charId);
					result = !flag3;
				}
			}
			return result;
		}

		// Token: 0x06008C7A RID: 35962 RVA: 0x0040EA6C File Offset: 0x0040CC6C
		public void Setup()
		{
			this.Model.AddEvent(ECombatEvents.OnDataReady, new OnCombatEvent(this.OnDataReady));
			this.Model.AddEvent(ECombatEvents.ChangeChar, new OnCombatEvent(this.OnChangeChar));
			this.Model.AddEvent(ECombatEvents.OnTimeScaleChanged, new OnCombatEvent(this.OnTimeScaleChanged));
			CombatModel model = this.Model;
			model.OnDefeatMarkCollectionChanged = (OnCharacterDataChangedEvent<DefeatMarkCollection>)Delegate.Combine(model.OnDefeatMarkCollectionChanged, new OnCharacterDataChangedEvent<DefeatMarkCollection>(this.OnDefeatMarkCollectionChanged));
			CombatModel model2 = this.Model;
			model2.OnCurrTeammateCommandsChanged = (OnCharacterDataChangedEvent)Delegate.Combine(model2.OnCurrTeammateCommandsChanged, new OnCharacterDataChangedEvent(this.OnCurrTeammateCommandsChanged));
			CombatModel model3 = this.Model;
			model3.OnShowTransferInjuryCommandChanged = (OnCharacterDataChangedEvent)Delegate.Combine(model3.OnShowTransferInjuryCommandChanged, new OnCharacterDataChangedEvent(this.OnShowTransferInjuryCommandChanged));
			CombatModel model4 = this.Model;
			model4.OnTeammateCommandBanReasonsChanged = (OnCharacterDataChangedEvent)Delegate.Combine(model4.OnTeammateCommandBanReasonsChanged, new OnCharacterDataChangedEvent(this.OnTeammateCommandBanReasonsChanged));
			CombatModel model5 = this.Model;
			model5.OnTeammateCommandCanUseChanged = (OnCharacterDataChangedEvent)Delegate.Combine(model5.OnTeammateCommandCanUseChanged, new OnCharacterDataChangedEvent(this.OnTeammateCommandCanUseChanged));
			CombatModel model6 = this.Model;
			model6.OnTeammateCommandCdChanged = (OnCharacterDataChangedEvent)Delegate.Combine(model6.OnTeammateCommandCdChanged, new OnCharacterDataChangedEvent(this.OnTeammateCommandCdChanged));
			CombatModel model7 = this.Model;
			model7.OnAttackCommandWeaponKeyChanged = (OnCharacterDataChangedEvent)Delegate.Combine(model7.OnAttackCommandWeaponKeyChanged, new OnCharacterDataChangedEvent(this.OnAttackCommandWeaponKeyChanged));
			CombatModel model8 = this.Model;
			model8.OnTeammateCommandTimePercentChanged = (OnCharacterDataChangedEvent)Delegate.Combine(model8.OnTeammateCommandTimePercentChanged, new OnCharacterDataChangedEvent(this.OnTeammateCommandTimePercentChanged));
			CombatModel model9 = this.Model;
			model9.OnAttackCommandTrickTypeChanged = (OnCharacterDataChangedEvent)Delegate.Combine(model9.OnAttackCommandTrickTypeChanged, new OnCharacterDataChangedEvent(this.OnAttackCommandTrickTypeChanged));
			CombatModel model10 = this.Model;
			model10.OnAttackCommandSkillIdChanged = (OnCharacterDataChangedEvent)Delegate.Combine(model10.OnAttackCommandSkillIdChanged, new OnCharacterDataChangedEvent(this.OnAttackCommandSkillIdChanged));
			CombatModel model11 = this.Model;
			model11.OnDefendCommandSkillIdChanged = (OnCharacterDataChangedEvent)Delegate.Combine(model11.OnDefendCommandSkillIdChanged, new OnCharacterDataChangedEvent(this.OnDefendCommandSkillIdChanged));
			CombatModel model12 = this.Model;
			model12.OnExecutingTeammateCommandChanged = (OnCharacterDataChangedEvent<sbyte>)Delegate.Combine(model12.OnExecutingTeammateCommandChanged, new OnCharacterDataChangedEvent<sbyte>(this.OnExecutingTeammateCommandChanged));
		}

		// Token: 0x06008C7B RID: 35963 RVA: 0x0040EC9C File Offset: 0x0040CE9C
		public void Close()
		{
			this.Model.RemoveEvent(ECombatEvents.OnDataReady, new OnCombatEvent(this.OnDataReady));
			this.Model.RemoveEvent(ECombatEvents.ChangeChar, new OnCombatEvent(this.OnChangeChar));
			this.Model.RemoveEvent(ECombatEvents.OnTimeScaleChanged, new OnCombatEvent(this.OnTimeScaleChanged));
			CombatModel model = this.Model;
			model.OnDefeatMarkCollectionChanged = (OnCharacterDataChangedEvent<DefeatMarkCollection>)Delegate.Remove(model.OnDefeatMarkCollectionChanged, new OnCharacterDataChangedEvent<DefeatMarkCollection>(this.OnDefeatMarkCollectionChanged));
			CombatModel model2 = this.Model;
			model2.OnCurrTeammateCommandsChanged = (OnCharacterDataChangedEvent)Delegate.Remove(model2.OnCurrTeammateCommandsChanged, new OnCharacterDataChangedEvent(this.OnCurrTeammateCommandsChanged));
			CombatModel model3 = this.Model;
			model3.OnShowTransferInjuryCommandChanged = (OnCharacterDataChangedEvent)Delegate.Remove(model3.OnShowTransferInjuryCommandChanged, new OnCharacterDataChangedEvent(this.OnShowTransferInjuryCommandChanged));
			CombatModel model4 = this.Model;
			model4.OnTeammateCommandBanReasonsChanged = (OnCharacterDataChangedEvent)Delegate.Remove(model4.OnTeammateCommandBanReasonsChanged, new OnCharacterDataChangedEvent(this.OnTeammateCommandBanReasonsChanged));
			CombatModel model5 = this.Model;
			model5.OnTeammateCommandCanUseChanged = (OnCharacterDataChangedEvent)Delegate.Remove(model5.OnTeammateCommandCanUseChanged, new OnCharacterDataChangedEvent(this.OnTeammateCommandCanUseChanged));
			CombatModel model6 = this.Model;
			model6.OnTeammateCommandCdChanged = (OnCharacterDataChangedEvent)Delegate.Remove(model6.OnTeammateCommandCdChanged, new OnCharacterDataChangedEvent(this.OnTeammateCommandCdChanged));
			CombatModel model7 = this.Model;
			model7.OnAttackCommandWeaponKeyChanged = (OnCharacterDataChangedEvent)Delegate.Remove(model7.OnAttackCommandWeaponKeyChanged, new OnCharacterDataChangedEvent(this.OnAttackCommandWeaponKeyChanged));
			CombatModel model8 = this.Model;
			model8.OnTeammateCommandTimePercentChanged = (OnCharacterDataChangedEvent)Delegate.Remove(model8.OnTeammateCommandTimePercentChanged, new OnCharacterDataChangedEvent(this.OnTeammateCommandTimePercentChanged));
			CombatModel model9 = this.Model;
			model9.OnAttackCommandTrickTypeChanged = (OnCharacterDataChangedEvent)Delegate.Remove(model9.OnAttackCommandTrickTypeChanged, new OnCharacterDataChangedEvent(this.OnAttackCommandTrickTypeChanged));
			CombatModel model10 = this.Model;
			model10.OnAttackCommandSkillIdChanged = (OnCharacterDataChangedEvent)Delegate.Remove(model10.OnAttackCommandSkillIdChanged, new OnCharacterDataChangedEvent(this.OnAttackCommandSkillIdChanged));
			CombatModel model11 = this.Model;
			model11.OnDefendCommandSkillIdChanged = (OnCharacterDataChangedEvent)Delegate.Remove(model11.OnDefendCommandSkillIdChanged, new OnCharacterDataChangedEvent(this.OnDefendCommandSkillIdChanged));
			CombatModel model12 = this.Model;
			model12.OnExecutingTeammateCommandChanged = (OnCharacterDataChangedEvent<sbyte>)Delegate.Remove(model12.OnExecutingTeammateCommandChanged, new OnCharacterDataChangedEvent<sbyte>(this.OnExecutingTeammateCommandChanged));
			bool flag = this._characterAvatar != null;
			if (flag)
			{
				this._characterAvatar.CharacterId = -1;
			}
		}

		// Token: 0x06008C7C RID: 35964 RVA: 0x0040EEE4 File Offset: 0x0040D0E4
		private static void UpdateTeammateCommandDisplay(CombatTeammateCommand cmdRefers, TeammateCommandItem cmdConfig, bool canUse, bool isAlly)
		{
			string spriteName = canUse ? ((cmdConfig.Type == ETeammateCommandType.Advance) ? "ui9_combat_teammate_5" : (isAlly ? "ui9_combat_teammate_4" : "ui9_combat_teammate_4")) : ((cmdConfig.Type != ETeammateCommandType.Negative) ? "ui9_combat_teammate_2" : "ui9_combat_teammate_3");
			ETeammateCommandType type = cmdConfig.Type;
			if (!true)
			{
			}
			string text;
			if (type != ETeammateCommandType.Advance)
			{
				if (type != ETeammateCommandType.Negative)
				{
					text = "pinkyellow";
				}
				else
				{
					text = "negativecommand";
				}
			}
			else
			{
				text = "upgradeteammatecommand";
			}
			if (!true)
			{
			}
			string color = text;
			cmdRefers.commandName.text = cmdConfig.Name.SetColor(Colors.Instance[color]);
			cmdRefers.medalIcon.SetSprite(CommonUtils.GetFeatureMedalIcon((int)cmdConfig.MedalType, (int)cmdConfig.MedalCount), false, null);
			DisableStyleRoot component = cmdRefers.commandName.GetComponent<DisableStyleRoot>();
			if (component != null)
			{
				component.SetStyleEffect(!canUse, false);
			}
			CButton cmdBtn = cmdRefers.button;
			cmdBtn.GetComponent<CImage>().SetSprite(spriteName, false, null);
			CImage highLightImg = cmdRefers.highLight;
			highLightImg.SetSprite((cmdConfig.Type == ETeammateCommandType.Advance) ? "ui9_combat_teammate_6" : "ui9_combat_teammate_1", false, null);
		}

		// Token: 0x06008C7D RID: 35965 RVA: 0x0040F000 File Offset: 0x0040D200
		private void PlayUiParticle(ParticleSystem particle)
		{
			particle.Play(true);
			particle.gameObject.SetActive(true);
			base.StartCoroutine(this.HideParticle(particle));
		}

		// Token: 0x06008C7E RID: 35966 RVA: 0x0040F026 File Offset: 0x0040D226
		private void PlayUiParticle(UIParticle particle)
		{
			particle.gameObject.SetActive(true);
			particle.Play();
			base.StartCoroutine(this.HideUIParticle(particle));
		}

		// Token: 0x06008C7F RID: 35967 RVA: 0x0040F04B File Offset: 0x0040D24B
		private IEnumerator HideParticle(ParticleSystem particle)
		{
			float timeAccumulator = 0f;
			while (timeAccumulator < particle.main.duration)
			{
				bool flag = !particle.isPaused;
				if (flag)
				{
					timeAccumulator += Time.deltaTime;
				}
				yield return null;
			}
			particle.Stop(true);
			particle.gameObject.SetActive(false);
			yield break;
		}

		// Token: 0x06008C80 RID: 35968 RVA: 0x0040F061 File Offset: 0x0040D261
		private IEnumerator HideUIParticle(UIParticle particle)
		{
			float maxDuration = 0f;
			ParticleSystem[] pss = particle.GetComponentsInChildren<ParticleSystem>(true);
			foreach (ParticleSystem ps in pss)
			{
				maxDuration = Mathf.Max(maxDuration, ps.main.duration);
				ps = null;
			}
			ParticleSystem[] array = null;
			float timeAccumulator = 0f;
			while (timeAccumulator < maxDuration)
			{
				bool flag = !particle.isPaused;
				if (flag)
				{
					timeAccumulator += Time.deltaTime;
				}
				yield return null;
			}
			particle.Stop();
			particle.gameObject.SetActive(false);
			yield break;
		}

		// Token: 0x06008C81 RID: 35969 RVA: 0x0040F078 File Offset: 0x0040D278
		private void UpdateNameString(int charId)
		{
			CharacterDisplayData displayData = this.Model.DisplayDataCache[charId];
			this.teammateName.text = CombatUtils.GetNameString(displayData, this.ally);
		}

		// Token: 0x06008C82 RID: 35970 RVA: 0x0040F0B0 File Offset: 0x0040D2B0
		public RectTransform GetCommandHolder()
		{
			return this.commandHolder;
		}

		// Token: 0x04006B67 RID: 27495
		private CharacterAvatar _characterAvatar;

		// Token: 0x04006B68 RID: 27496
		[SerializeField]
		private bool ally;

		// Token: 0x04006B69 RID: 27497
		[SerializeField]
		private int index;

		// Token: 0x04006B6A RID: 27498
		[SerializeField]
		private Game.Components.Avatar.Avatar avatar;

		// Token: 0x04006B6B RID: 27499
		[SerializeField]
		private TextMeshProUGUI teammateName;

		// Token: 0x04006B6C RID: 27500
		[SerializeField]
		private CombatDefeatMarkTotalCount defeatMark;

		// Token: 0x04006B6D RID: 27501
		[SerializeField]
		private RectTransform commandHolder;

		// Token: 0x04006B6E RID: 27502
		[SerializeField]
		private CButton button;

		// Token: 0x04006B6F RID: 27503
		[SerializeField]
		private GameObject highLight;
	}
}
