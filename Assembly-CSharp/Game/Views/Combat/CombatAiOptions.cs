using System;
using System.Collections.Generic;
using System.IO;
using Config;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Combat;
using GameData.GameDataBridge;
using GameData.Serializer;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.Combat
{
	// Token: 0x02000B22 RID: 2850
	public class CombatAiOptions : MonoBehaviour
	{
		// Token: 0x06008BC3 RID: 35779 RVA: 0x00408A2C File Offset: 0x00406C2C
		private void Awake()
		{
			Dictionary<int, string> implementToOptions = new Dictionary<int, string>();
			foreach (TeammateCommandItem config in ((IEnumerable<TeammateCommandItem>)TeammateCommand.Instance))
			{
				implementToOptions[(int)config.Option] = config.Name;
			}
			CommonUtils.PrepareEnoughChildren(this.cmdTypeRoot.transform, this.cmdTypeRoot.GetChild(0).gameObject, 25, null);
			for (int i = 0; i < 25; i++)
			{
				this.cmdTypeRoot.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>().text = implementToOptions[i];
				this.cmdTypeRoot.GetChild(i).name = string.Format("CmdType{0}", i);
			}
			CButton[] btnList = base.GetComponentsInChildren<CButton>(true);
			for (int j = 0; j < btnList.Length; j++)
			{
				CButton btn = btnList[j];
				btn.onClick.AddListener(delegate()
				{
					this.OnClick(btn);
				});
				this._btn2CheckMark.Add(btn.name, btn.transform.Find("CheckBox/CheckMark").gameObject);
			}
		}

		// Token: 0x06008BC4 RID: 35780 RVA: 0x00408BB0 File Offset: 0x00406DB0
		private void OnEnable()
		{
			bool flag = this.autoSave;
			if (flag)
			{
				this.Load();
			}
			this.autoAttack.onValueChanged.RemoveAllListeners();
			this.autoAttack.isOn = this.AiOptions.AutoAttack;
			this.autoAttack.onValueChanged.AddListener(new UnityAction<bool>(this.OnAutoAttackTogChange));
			this.autoMove.onValueChanged.RemoveAllListeners();
			this.autoMove.isOn = this.AiOptions.AutoMove;
			this.autoMove.onValueChanged.AddListener(new UnityAction<bool>(this.OnAutoMoveTogChange));
			this.UpdateAutoAttackSubBtn();
			this.UpdateAutoMoveSubBtn();
			this._btn2CheckMark["ChangeWeapon"].SetActive(this.AiOptions.AutoChangeWeapon);
			this._btn2CheckMark["ChangeTrick"].SetActive(this.AiOptions.AutoChangeTrick);
			this._btn2CheckMark["AutoUnlock"].SetActive(this.AiOptions.AutoUnlock);
			this._btn2CheckMark["SkipRawCreate"].SetActive(this.AiOptions.SkipRawCreate);
			this.SetSkipRawCreateBtnStyle(this.AiOptions.AutoUnlock);
			this._btn2CheckMark["TryDodge"].SetActive(this.AiOptions.TryDodge);
			this._btn2CheckMark["AutoSaveMovingTarget"].SetActive(this.AiOptions.SaveMoveTarget);
			this._btn2CheckMark["AutoCastAttack"].SetActive(this.AiOptions.AutoCastSkill[0]);
			this._btn2CheckMark["AutoCastAgile"].SetActive(this.AiOptions.AutoCastSkill[1]);
			this._btn2CheckMark["AutoCastDefense"].SetActive(this.AiOptions.AutoCastSkill[2]);
			this._btn2CheckMark["AutoCastBuff"].SetActive(this.AiOptions.AutoCostNeiliAllocation);
			this._btn2CheckMark["AutoCostTrick"].SetActive(this.AiOptions.AutoCostTrick);
			this._btn2CheckMark["AutoInterrupt"].SetActive(this.AiOptions.AutoInterrupt);
			this._btn2CheckMark["AutoClearAgile"].SetActive(this.AiOptions.AutoClearAgile);
			this._btn2CheckMark["AutoClearDefense"].SetActive(this.AiOptions.AutoClearDefense);
			this._btn2CheckMark["AutoHealInjury"].SetActive(this.AiOptions.AutoUseOtherAction[0]);
			this._btn2CheckMark["AutoHealPoison"].SetActive(this.AiOptions.AutoUseOtherAction[1]);
			this._btn2CheckMark["AutoFlee"].SetActive(this.AiOptions.AutoUseOtherAction[2]);
			sbyte type = 0;
			while ((int)type < this.AiOptions.AutoUseTeammateCommand.Length)
			{
				this._btn2CheckMark[string.Format("CmdType{0}", type)].SetActive(this.AiOptions.AutoUseTeammateCommand[(int)type]);
				type += 1;
			}
			this._listenerId = GameDataBridge.RegisterListener(new GameDataBridge.NotificationHandler(this.OnNotifyGameData));
		}

		// Token: 0x06008BC5 RID: 35781 RVA: 0x00408F18 File Offset: 0x00407118
		private void OnDisable()
		{
			bool flag = this.autoSave;
			if (flag)
			{
				this.Save();
			}
			CombatDomainMethod.Call.SetAiOptions(this.AiOptions);
			GameDataBridge.UnregisterListener(this._listenerId);
		}

		// Token: 0x06008BC6 RID: 35782 RVA: 0x00408F4F File Offset: 0x0040714F
		private void OnNotifyGameData(List<NotificationWrapper> notifications)
		{
		}

		// Token: 0x06008BC7 RID: 35783 RVA: 0x00408F52 File Offset: 0x00407152
		private void OnAutoAttackTogChange(bool value)
		{
			this.AiOptions.AutoAttack = value;
			this.UpdateAutoAttackSubBtn();
			this.onOptionsChanged.Invoke();
		}

		// Token: 0x06008BC8 RID: 35784 RVA: 0x00408F74 File Offset: 0x00407174
		private void OnAutoMoveTogChange(bool value)
		{
			this.AiOptions.AutoMove = value;
			this.UpdateAutoMoveSubBtn();
			this.onOptionsChanged.Invoke();
		}

		// Token: 0x06008BC9 RID: 35785 RVA: 0x00408F98 File Offset: 0x00407198
		private void OnClick(CButton btn)
		{
			string btnName = btn.name;
			bool flag = btnName == "ChangeWeapon";
			if (flag)
			{
				this.AiOptions.AutoChangeWeapon = !this.AiOptions.AutoChangeWeapon;
				this._btn2CheckMark[btnName].SetActive(this.AiOptions.AutoChangeWeapon);
				this.onOptionsChanged.Invoke();
			}
			else
			{
				bool flag2 = btnName == "ChangeTrick";
				if (flag2)
				{
					this.AiOptions.AutoChangeTrick = !this.AiOptions.AutoChangeTrick;
					this._btn2CheckMark[btnName].SetActive(this.AiOptions.AutoChangeTrick);
					this.onOptionsChanged.Invoke();
				}
				else
				{
					bool flag3 = btnName == "AutoUnlock";
					if (flag3)
					{
						this.AiOptions.AutoUnlock = !this.AiOptions.AutoUnlock;
						this._btn2CheckMark[btnName].SetActive(this.AiOptions.AutoUnlock);
						this.onOptionsChanged.Invoke();
						this.SetSkipRawCreateBtnStyle(this.AiOptions.AutoUnlock);
					}
					else
					{
						bool flag4 = btnName == "SkipRawCreate";
						if (flag4)
						{
							this.AiOptions.SkipRawCreate = !this.AiOptions.SkipRawCreate;
							this._btn2CheckMark[btnName].SetActive(this.AiOptions.SkipRawCreate);
							this.onOptionsChanged.Invoke();
						}
						else
						{
							bool flag5 = btnName == "TryDodge";
							if (flag5)
							{
								this.AiOptions.TryDodge = !this.AiOptions.TryDodge;
								this._btn2CheckMark[btnName].SetActive(this.AiOptions.TryDodge);
								this.onOptionsChanged.Invoke();
							}
							else
							{
								bool flag6 = btnName == "AutoSaveMovingTarget";
								if (flag6)
								{
									this.AiOptions.SaveMoveTarget = !this.AiOptions.SaveMoveTarget;
									this._btn2CheckMark[btnName].SetActive(this.AiOptions.SaveMoveTarget);
									this.onOptionsChanged.Invoke();
								}
								else
								{
									bool flag7 = btnName == "AutoCastAttack";
									if (flag7)
									{
										this.AiOptions.AutoCastSkill[0] = !this.AiOptions.AutoCastSkill[0];
										this._btn2CheckMark[btnName].SetActive(this.AiOptions.AutoCastSkill[0]);
										this.onOptionsChanged.Invoke();
									}
									else
									{
										bool flag8 = btnName == "AutoCastAgile";
										if (flag8)
										{
											this.AiOptions.AutoCastSkill[1] = !this.AiOptions.AutoCastSkill[1];
											this._btn2CheckMark[btnName].SetActive(this.AiOptions.AutoCastSkill[1]);
											this.onOptionsChanged.Invoke();
										}
										else
										{
											bool flag9 = btnName == "AutoCastDefense";
											if (flag9)
											{
												this.AiOptions.AutoCastSkill[2] = !this.AiOptions.AutoCastSkill[2];
												this._btn2CheckMark[btnName].SetActive(this.AiOptions.AutoCastSkill[2]);
												this.onOptionsChanged.Invoke();
											}
											else
											{
												bool flag10 = btnName == "AutoCastBuff";
												if (flag10)
												{
													this.AiOptions.AutoCostNeiliAllocation = !this.AiOptions.AutoCostNeiliAllocation;
													this._btn2CheckMark[btnName].SetActive(this.AiOptions.AutoCostNeiliAllocation);
													this.onOptionsChanged.Invoke();
												}
												else
												{
													bool flag11 = btnName == "AutoCostTrick";
													if (flag11)
													{
														this.AiOptions.AutoCostTrick = !this.AiOptions.AutoCostTrick;
														this._btn2CheckMark[btnName].SetActive(this.AiOptions.AutoCostTrick);
														this.onOptionsChanged.Invoke();
													}
													else
													{
														bool flag12 = btnName == "AutoInterrupt";
														if (flag12)
														{
															this.AiOptions.AutoInterrupt = !this.AiOptions.AutoInterrupt;
															this._btn2CheckMark[btnName].SetActive(this.AiOptions.AutoInterrupt);
															this.onOptionsChanged.Invoke();
														}
														else
														{
															bool flag13 = btnName == "AutoClearAgile";
															if (flag13)
															{
																this.AiOptions.AutoClearAgile = !this.AiOptions.AutoClearAgile;
																this._btn2CheckMark[btnName].SetActive(this.AiOptions.AutoClearAgile);
																this.onOptionsChanged.Invoke();
															}
															else
															{
																bool flag14 = btnName == "AutoClearDefense";
																if (flag14)
																{
																	this.AiOptions.AutoClearDefense = !this.AiOptions.AutoClearDefense;
																	this._btn2CheckMark[btnName].SetActive(this.AiOptions.AutoClearDefense);
																	this.onOptionsChanged.Invoke();
																}
																else
																{
																	bool flag15 = btnName == "AutoHealInjury";
																	if (flag15)
																	{
																		this.AiOptions.AutoUseOtherAction[0] = !this.AiOptions.AutoUseOtherAction[0];
																		this._btn2CheckMark[btnName].SetActive(this.AiOptions.AutoUseOtherAction[0]);
																		this.onOptionsChanged.Invoke();
																	}
																	else
																	{
																		bool flag16 = btnName == "AutoHealPoison";
																		if (flag16)
																		{
																			this.AiOptions.AutoUseOtherAction[1] = !this.AiOptions.AutoUseOtherAction[1];
																			this._btn2CheckMark[btnName].SetActive(this.AiOptions.AutoUseOtherAction[1]);
																			this.onOptionsChanged.Invoke();
																		}
																		else
																		{
																			bool flag17 = btnName == "AutoFlee";
																			if (flag17)
																			{
																				this.AiOptions.AutoUseOtherAction[2] = !this.AiOptions.AutoUseOtherAction[2];
																				this._btn2CheckMark[btnName].SetActive(this.AiOptions.AutoUseOtherAction[2]);
																				this.onOptionsChanged.Invoke();
																			}
																			else
																			{
																				bool flag18 = btnName.StartsWith("CmdType");
																				if (flag18)
																				{
																					int index = int.Parse(btnName.Substring(7));
																					this.AiOptions.AutoUseTeammateCommand[index] = !this.AiOptions.AutoUseTeammateCommand[index];
																					this._btn2CheckMark[btnName].SetActive(this.AiOptions.AutoUseTeammateCommand[index]);
																					this.onOptionsChanged.Invoke();
																				}
																			}
																		}
																	}
																}
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06008BCA RID: 35786 RVA: 0x00409640 File Offset: 0x00407840
		private string InitSaveFilePath()
		{
			string archiveDir = GameApp.GetArchiveDirPath();
			bool flag = !Directory.Exists(archiveDir);
			if (flag)
			{
				Directory.CreateDirectory(archiveDir);
			}
			return Path.Combine(archiveDir, "CombatAiSetting.lua");
		}

		// Token: 0x06008BCB RID: 35787 RVA: 0x00409678 File Offset: 0x00407878
		public static void SyncToBackend()
		{
			string path = Path.Combine(GameApp.GetArchiveDirPath(), "CombatAiSetting.lua");
			bool flag = File.Exists(path);
			if (flag)
			{
				try
				{
					AiOptions aiOptions;
					GameData.Serializer.CommonObjectSerializer.Deserialize<AiOptions>(File.ReadAllText(path), out aiOptions, GameData.Serializer.CommonObjectSerializer.MarshalFormat.LuaWithReturnPrefix);
					CombatDomainMethod.Call.SetAiOptions(aiOptions);
				}
				catch
				{
				}
			}
		}

		// Token: 0x06008BCC RID: 35788 RVA: 0x004096D4 File Offset: 0x004078D4
		private void UpdateAutoAttackSubBtn()
		{
			this.changeWeapon.interactable = this.AiOptions.AutoAttack;
			this.changeWeapon.GetComponent<DisableStyleRoot>().SetStyleEffect(!this.AiOptions.AutoAttack, false);
			this.changeTrick.interactable = this.AiOptions.AutoAttack;
			this.changeTrick.GetComponent<DisableStyleRoot>().SetStyleEffect(!this.AiOptions.AutoAttack, false);
			this.autoUnlock.interactable = this.AiOptions.AutoAttack;
			this.autoUnlock.GetComponent<DisableStyleRoot>().SetStyleEffect(!this.AiOptions.AutoAttack, false);
			this.SetSkipRawCreateBtnStyle(this.AiOptions.AutoUnlock);
		}

		// Token: 0x06008BCD RID: 35789 RVA: 0x0040979C File Offset: 0x0040799C
		private void UpdateAutoMoveSubBtn()
		{
			this.tryDodge.interactable = this.AiOptions.AutoMove;
			this.tryDodge.GetComponent<DisableStyleRoot>().SetStyleEffect(!this.AiOptions.AutoMove, false);
			this.autoSaveMovingTarget.interactable = !this.AiOptions.AutoMove;
			this.autoSaveMovingTarget.GetComponent<DisableStyleRoot>().SetStyleEffect(this.AiOptions.AutoMove, false);
		}

		// Token: 0x06008BCE RID: 35790 RVA: 0x00409818 File Offset: 0x00407A18
		public void Save()
		{
			if (this._saveFilePath == null)
			{
				this._saveFilePath = this.InitSaveFilePath();
			}
			string marshalData;
			GameData.Serializer.CommonObjectSerializer.Serialize<AiOptions>(this.AiOptions, out marshalData, GameData.Serializer.CommonObjectSerializer.MarshalFormat.LuaWithReturnPrefix);
			File.WriteAllText(this._saveFilePath, marshalData);
		}

		// Token: 0x06008BCF RID: 35791 RVA: 0x00409858 File Offset: 0x00407A58
		public void Load()
		{
			if (this._saveFilePath == null)
			{
				this._saveFilePath = this.InitSaveFilePath();
			}
			bool flag = File.Exists(this._saveFilePath);
			if (flag)
			{
				try
				{
					GameData.Serializer.CommonObjectSerializer.Deserialize<AiOptions>(File.ReadAllText(this._saveFilePath), out this.AiOptions, GameData.Serializer.CommonObjectSerializer.MarshalFormat.LuaWithReturnPrefix);
				}
				catch (Exception e)
				{
					Debug.LogError(e);
					File.Delete(this._saveFilePath);
					this.ResetOptions();
				}
			}
			else
			{
				this.ResetOptions();
			}
		}

		// Token: 0x06008BD0 RID: 35792 RVA: 0x004098E4 File Offset: 0x00407AE4
		public void ResetOptions()
		{
			this.AiOptions.Reset();
			this.AiOptions.AutoChangeWeapon = false;
			this.AiOptions.AutoUseOtherAction[2] = false;
		}

		// Token: 0x06008BD1 RID: 35793 RVA: 0x00409910 File Offset: 0x00407B10
		public void InitCombatAiOptions()
		{
			this.autoAttack.onValueChanged.RemoveAllListeners();
			this.autoAttack.isOn = this.AiOptions.AutoAttack;
			this.autoAttack.onValueChanged.AddListener(new UnityAction<bool>(this.OnAutoAttackTogChange));
			this.autoMove.onValueChanged.RemoveAllListeners();
			this.autoMove.isOn = this.AiOptions.AutoMove;
			this.autoMove.onValueChanged.AddListener(new UnityAction<bool>(this.OnAutoAttackTogChange));
			this.UpdateAutoAttackSubBtn();
			this.UpdateAutoMoveSubBtn();
			this._btn2CheckMark["ChangeWeapon"].SetActive(this.AiOptions.AutoChangeWeapon);
			this._btn2CheckMark["ChangeTrick"].SetActive(this.AiOptions.AutoChangeTrick);
			this._btn2CheckMark["AutoUnlock"].SetActive(this.AiOptions.AutoUnlock);
			this._btn2CheckMark["SkipRawCreate"].SetActive(this.AiOptions.SkipRawCreate);
			this._btn2CheckMark["TryDodge"].SetActive(this.AiOptions.TryDodge);
			this._btn2CheckMark["AutoSaveMovingTarget"].SetActive(this.AiOptions.SaveMoveTarget);
			this._btn2CheckMark["AutoCastAttack"].SetActive(this.AiOptions.AutoCastSkill[0]);
			this._btn2CheckMark["AutoCastAgile"].SetActive(this.AiOptions.AutoCastSkill[1]);
			this._btn2CheckMark["AutoCastDefense"].SetActive(this.AiOptions.AutoCastSkill[2]);
			this._btn2CheckMark["AutoCastBuff"].SetActive(this.AiOptions.AutoCostNeiliAllocation);
			this._btn2CheckMark["AutoCostTrick"].SetActive(this.AiOptions.AutoCostTrick);
			this._btn2CheckMark["AutoInterrupt"].SetActive(this.AiOptions.AutoInterrupt);
			this._btn2CheckMark["AutoClearAgile"].SetActive(this.AiOptions.AutoClearAgile);
			this._btn2CheckMark["AutoClearDefense"].SetActive(this.AiOptions.AutoClearDefense);
			this._btn2CheckMark["AutoHealInjury"].SetActive(this.AiOptions.AutoUseOtherAction[0]);
			this._btn2CheckMark["AutoHealPoison"].SetActive(this.AiOptions.AutoUseOtherAction[1]);
			this._btn2CheckMark["AutoFlee"].SetActive(this.AiOptions.AutoUseOtherAction[2]);
			sbyte type = 0;
			while ((int)type < this.AiOptions.AutoUseTeammateCommand.Length)
			{
				this._btn2CheckMark[string.Format("CmdType{0}", type)].SetActive(this.AiOptions.AutoUseTeammateCommand[(int)type]);
				type += 1;
			}
		}

		// Token: 0x06008BD2 RID: 35794 RVA: 0x00409C3E File Offset: 0x00407E3E
		public void OpenAllOptions()
		{
			this.SetAll(true);
			this.InitCombatAiOptions();
			UnityEvent unityEvent = this.onOptionsChanged;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
		}

		// Token: 0x06008BD3 RID: 35795 RVA: 0x00409C62 File Offset: 0x00407E62
		public void CloseAllOptions()
		{
			this.SetAll(false);
			this.InitCombatAiOptions();
			UnityEvent unityEvent = this.onOptionsChanged;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
		}

		// Token: 0x06008BD4 RID: 35796 RVA: 0x00409C88 File Offset: 0x00407E88
		private void SetAll(bool value)
		{
			this.AiOptions.AutoAttack = value;
			this.AiOptions.AutoChangeWeapon = value;
			this.AiOptions.AutoChangeTrick = value;
			this.SetAutoUnlock(value);
			this.AiOptions.SkipRawCreate = value;
			this.AiOptions.AutoMove = value;
			this.AiOptions.TryDodge = value;
			this.AiOptions.SaveMoveTarget = !value;
			this.AiOptions.AutoCostNeiliAllocation = value;
			this.AiOptions.AutoInterrupt = value;
			this.AiOptions.AutoCostTrick = value;
			this.AiOptions.AutoClearAgile = value;
			this.AiOptions.AutoClearDefense = value;
			for (int i = 0; i < this.AiOptions.AutoCastSkill.Length; i++)
			{
				this.AiOptions.AutoCastSkill[i] = value;
			}
			for (int j = 0; j < this.AiOptions.AutoUseOtherAction.Length; j++)
			{
				this.AiOptions.AutoUseOtherAction[j] = value;
			}
			for (int k = 0; k < this.AiOptions.AutoUseTeammateCommand.Length; k++)
			{
				this.AiOptions.AutoUseTeammateCommand[k] = value;
			}
		}

		// Token: 0x06008BD5 RID: 35797 RVA: 0x00409DB8 File Offset: 0x00407FB8
		public bool IsAllOptionsOpen()
		{
			bool flag = !this.AiOptions.AutoAttack || !this.AiOptions.AutoChangeWeapon || !this.AiOptions.AutoChangeTrick || !this.AiOptions.AutoUnlock || !this.AiOptions.SkipRawCreate;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = !this.AiOptions.AutoMove || !this.AiOptions.TryDodge || this.AiOptions.SaveMoveTarget;
				if (flag2)
				{
					result = false;
				}
				else
				{
					bool flag3 = !this.AiOptions.AutoCostNeiliAllocation || !this.AiOptions.AutoInterrupt || !this.AiOptions.AutoCostTrick || !this.AiOptions.AutoClearAgile || !this.AiOptions.AutoClearDefense;
					if (flag3)
					{
						result = false;
					}
					else
					{
						for (int i = 0; i < this.AiOptions.AutoCastSkill.Length; i++)
						{
							bool flag4 = !this.AiOptions.AutoCastSkill[i];
							if (flag4)
							{
								return false;
							}
						}
						for (int j = 0; j < this.AiOptions.AutoUseOtherAction.Length; j++)
						{
							bool flag5 = !this.AiOptions.AutoUseOtherAction[j];
							if (flag5)
							{
								return false;
							}
						}
						for (int k = 0; k < this.AiOptions.AutoUseTeammateCommand.Length; k++)
						{
							bool flag6 = !this.AiOptions.AutoUseTeammateCommand[k];
							if (flag6)
							{
								return false;
							}
						}
						result = true;
					}
				}
			}
			return result;
		}

		// Token: 0x06008BD6 RID: 35798 RVA: 0x00409F60 File Offset: 0x00408160
		public bool IsAllOptionsClosed()
		{
			bool flag = this.AiOptions.AutoAttack || this.AiOptions.AutoChangeWeapon || this.AiOptions.AutoChangeTrick || this.AiOptions.AutoUnlock || this.AiOptions.SkipRawCreate;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = this.AiOptions.AutoMove || this.AiOptions.TryDodge || !this.AiOptions.SaveMoveTarget;
				if (flag2)
				{
					result = false;
				}
				else
				{
					bool flag3 = this.AiOptions.AutoCostNeiliAllocation || this.AiOptions.AutoInterrupt || this.AiOptions.AutoCostTrick || this.AiOptions.AutoClearAgile || this.AiOptions.AutoClearDefense;
					if (flag3)
					{
						result = false;
					}
					else
					{
						for (int i = 0; i < this.AiOptions.AutoCastSkill.Length; i++)
						{
							bool flag4 = this.AiOptions.AutoCastSkill[i];
							if (flag4)
							{
								return false;
							}
						}
						for (int j = 0; j < this.AiOptions.AutoUseOtherAction.Length; j++)
						{
							bool flag5 = this.AiOptions.AutoUseOtherAction[j];
							if (flag5)
							{
								return false;
							}
						}
						for (int k = 0; k < this.AiOptions.AutoUseTeammateCommand.Length; k++)
						{
							bool flag6 = this.AiOptions.AutoUseTeammateCommand[k];
							if (flag6)
							{
								return false;
							}
						}
						result = true;
					}
				}
			}
			return result;
		}

		// Token: 0x06008BD7 RID: 35799 RVA: 0x0040A0FA File Offset: 0x004082FA
		private void SetAutoUnlock(bool autoUnlock)
		{
			this.AiOptions.AutoUnlock = autoUnlock;
			this.SetSkipRawCreateBtnStyle(autoUnlock);
		}

		// Token: 0x06008BD8 RID: 35800 RVA: 0x0040A114 File Offset: 0x00408314
		private void SetSkipRawCreateBtnStyle(bool autoUnlock)
		{
			bool interactable = autoUnlock && this.AiOptions.AutoAttack;
			this.skipRawCreate.GetComponent<DisableStyleRoot>().SetStyleEffect(!interactable, false);
			this.skipRawCreate.interactable = interactable;
		}

		// Token: 0x04006B02 RID: 27394
		[SerializeField]
		private CToggle autoAttack;

		// Token: 0x04006B03 RID: 27395
		[SerializeField]
		private CButton changeWeapon;

		// Token: 0x04006B04 RID: 27396
		[SerializeField]
		private CToggle autoMove;

		// Token: 0x04006B05 RID: 27397
		[SerializeField]
		private CButton tryDodge;

		// Token: 0x04006B06 RID: 27398
		[SerializeField]
		private List<RectTransform> cmdTypeList;

		// Token: 0x04006B07 RID: 27399
		[SerializeField]
		private RectTransform cmdTypeRoot;

		// Token: 0x04006B08 RID: 27400
		public GameObject title;

		// Token: 0x04006B09 RID: 27401
		[SerializeField]
		private CButton changeTrick;

		// Token: 0x04006B0A RID: 27402
		[SerializeField]
		private CButton autoUnlock;

		// Token: 0x04006B0B RID: 27403
		[SerializeField]
		private CButton autoSaveMovingTarget;

		// Token: 0x04006B0C RID: 27404
		[SerializeField]
		private CButton skipRawCreate;

		// Token: 0x04006B0D RID: 27405
		private string _saveFilePath;

		// Token: 0x04006B0E RID: 27406
		[NonSerialized]
		public AiOptions AiOptions = new AiOptions();

		// Token: 0x04006B0F RID: 27407
		public bool autoSave = true;

		// Token: 0x04006B10 RID: 27408
		public UnityEvent onOptionsChanged = new UnityEvent();

		// Token: 0x04006B11 RID: 27409
		private int _listenerId;

		// Token: 0x04006B12 RID: 27410
		private readonly Dictionary<string, GameObject> _btn2CheckMark = new Dictionary<string, GameObject>();
	}
}
