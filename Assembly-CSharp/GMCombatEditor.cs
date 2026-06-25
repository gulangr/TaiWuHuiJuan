using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using GameData.Common;
using GameData.Domains.Combat;
using GameData.GameDataBridge;
using GameData.Serializer;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x0200020E RID: 526
public class GMCombatEditor : MonoBehaviour
{
	// Token: 0x0600216E RID: 8558 RVA: 0x000F33B0 File Offset: 0x000F15B0
	private void Awake()
	{
		LayoutElement element = base.GetComponent<LayoutElement>();
		element.minHeight = base.GetComponent<RectTransform>().rect.height;
	}

	// Token: 0x0600216F RID: 8559 RVA: 0x000F33E0 File Offset: 0x000F15E0
	public void Init()
	{
		CButtonObsolete[] btnList = base.GetComponentsInChildren<CButtonObsolete>(true);
		for (int i = 0; i < btnList.Length; i++)
		{
			CButtonObsolete btn = btnList[i];
			bool autoListen = btn.AutoListen;
			if (autoListen)
			{
				btn.onClick.AddListener(delegate()
				{
					this.OnClick(btn);
				});
			}
		}
		List<string> trickTypeList = (from item in Config.TrickType.Instance
		select item.Name).ToList<string>();
		this.TrickTypeDropdown.ClearOptions();
		this.TrickTypeDropdown.AddOptions(trickTypeList);
		this.TrickTypeDropdown.value = 0;
		this.CombatSignDropdown.ClearOptions();
		this.CombatSignDropdown.AddOptions(this._signTypeName.ToList<string>());
		this.CombatSignDropdown.value = 0;
		this.CombatSignDropdown.onValueChanged.AddListener(new UnityAction<int>(this.CombatSignChanged));
		this._signType = GMCombatEditor.ESignBodyOptionsType.Invalid;
		this.CombatSignChanged(this.CombatSignDropdown.value);
	}

	// Token: 0x06002170 RID: 8560 RVA: 0x000F350C File Offset: 0x000F170C
	private void CombatSignChanged(int arg0)
	{
		if (!true)
		{
		}
		GMCombatEditor.ESignBodyOptionsType esignBodyOptionsType;
		switch (arg0)
		{
		case 0:
			esignBodyOptionsType = GMCombatEditor.ESignBodyOptionsType.All;
			goto IL_5F;
		case 1:
			esignBodyOptionsType = GMCombatEditor.ESignBodyOptionsType.BodyPart;
			goto IL_5F;
		case 2:
			esignBodyOptionsType = GMCombatEditor.ESignBodyOptionsType.BodyPart;
			goto IL_5F;
		case 3:
			esignBodyOptionsType = GMCombatEditor.ESignBodyOptionsType.BodyPart;
			goto IL_5F;
		case 4:
			esignBodyOptionsType = GMCombatEditor.ESignBodyOptionsType.BodyPart;
			goto IL_5F;
		case 5:
			esignBodyOptionsType = GMCombatEditor.ESignBodyOptionsType.Poison;
			goto IL_5F;
		case 9:
			esignBodyOptionsType = GMCombatEditor.ESignBodyOptionsType.BodyPart;
			goto IL_5F;
		case 10:
			esignBodyOptionsType = GMCombatEditor.ESignBodyOptionsType.Poison;
			goto IL_5F;
		}
		esignBodyOptionsType = GMCombatEditor.ESignBodyOptionsType.Invalid;
		IL_5F:
		if (!true)
		{
		}
		GMCombatEditor.ESignBodyOptionsType signType = esignBodyOptionsType;
		bool flag = signType == this._signType;
		if (!flag)
		{
			this._signType = signType;
			if (!true)
			{
			}
			IEnumerable<string> collection;
			if (signType != GMCombatEditor.ESignBodyOptionsType.BodyPart)
			{
				if (signType != GMCombatEditor.ESignBodyOptionsType.Poison)
				{
					collection = Array.Empty<string>();
				}
				else
				{
					collection = (from x in this.Range(6)
					select LocalStringManager.Get(string.Format("LK_Poison_Name_{0}", x))).Append(this._signTypeName[0]);
				}
			}
			else
			{
				collection = (from x in this.Range(7)
				select BodyPart.Instance[x].Name).Append(this._signTypeName[0]);
			}
			if (!true)
			{
			}
			List<string> options = new List<string>(collection);
			this.CombatSignBodyDropdown.ClearOptions();
			this.CombatSignBodyDropdown.AddOptions(options);
			this.CombatSignBodyDropdown.value = 0;
		}
	}

	// Token: 0x06002171 RID: 8561 RVA: 0x000F365C File Offset: 0x000F185C
	private void OnDestroy()
	{
		this.OnLeaveWorld();
	}

	// Token: 0x06002172 RID: 8562 RVA: 0x000F3668 File Offset: 0x000F1868
	public void OnWorldDataReady()
	{
		this.OnLeaveWorld();
		this._gameDataListenerId = GameDataBridge.RegisterListener(new GameDataBridge.NotificationHandler(this.OnNotifyGameData));
		GameDataBridge.AddDataMonitor(this._gameDataListenerId, 8, 19, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._gameDataListenerId, 8, 16, ulong.MaxValue, uint.MaxValue);
	}

	// Token: 0x06002173 RID: 8563 RVA: 0x000F36B8 File Offset: 0x000F18B8
	public void OnLeaveWorld()
	{
		bool flag = this._gameDataListenerId != -1;
		if (flag)
		{
			GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 8, 19, ulong.MaxValue, uint.MaxValue);
			GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 8, 16, ulong.MaxValue, uint.MaxValue);
			bool flag2 = this._enemyCharId >= 0;
			if (flag2)
			{
				GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 8, 10, (ulong)this._enemyCharId, this._enemySKillSubIds);
				this._enemyCharId = -1;
			}
			GameDataBridge.UnregisterListener(this._gameDataListenerId);
			this._gameDataListenerId = -1;
		}
	}

	// Token: 0x06002174 RID: 8564 RVA: 0x000F3744 File Offset: 0x000F1944
	private void OnNotifyGameData(List<NotificationWrapper> notifications)
	{
		foreach (NotificationWrapper wrapper in notifications)
		{
			Notification notification = wrapper.Notification;
			byte type = notification.Type;
			byte b = type;
			if (b != 0)
			{
				if (b != 1)
				{
				}
			}
			else
			{
				DataUid uid = notification.Uid;
				bool flag = uid.DomainId == 8;
				if (flag)
				{
					bool flag2 = uid.DataId == 19;
					if (flag2)
					{
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._combatStatus);
						bool active = this._combatStatus == 1;
						this.Mask.SetActive(!active);
					}
					else
					{
						bool flag3 = uid.DataId == 16;
						if (flag3)
						{
							bool flag4 = this._enemyCharId >= 0;
							if (flag4)
							{
								GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 8, 10, (ulong)this._enemyCharId, this._enemySKillSubIds);
							}
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._enemyCharId);
							bool flag5 = this._combatStatus == 1 && this._enemyCharId >= 0;
							if (flag5)
							{
								GameDataBridge.AddDataMonitor(this._gameDataListenerId, 8, 10, (ulong)this._enemyCharId, this._enemySKillSubIds);
							}
						}
						else
						{
							bool flag6 = uid.DataId == 10 && uid.SubId0 == (ulong)((long)this._enemyCharId);
							if (flag6)
							{
								List<short> skillList = (uid.SubId1 == 52U) ? this._enemyAttackSkillList : ((uid.SubId1 == 53U) ? this._enemyAgileSkillList : this._enemyDefenseSkillList);
								Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref skillList);
								this.UpdateEnemyCombatSkillList();
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x06002175 RID: 8565 RVA: 0x000F3938 File Offset: 0x000F1B38
	public void OnChangeSwitch(Toggle toggle)
	{
		string name = toggle.name;
		string a = name;
		if (!(a == "EnemyAiEnabled"))
		{
			if (a == "FreeCast")
			{
				CombatDomainMethod.Call.GmCmd_EnableSkillFreeCast(toggle.isOn);
			}
		}
		else
		{
			CombatDomainMethod.Call.GmCmd_EnableEnemyAi(!toggle.isOn);
		}
	}

	// Token: 0x06002176 RID: 8566 RVA: 0x000F3990 File Offset: 0x000F1B90
	public void OnClick(CButtonObsolete button)
	{
		string btnName = button.name;
		bool flag = btnName == "HealInjury";
		if (flag)
		{
			CombatDomainMethod.Call.GmCmd_ForceHealAllInjury();
		}
		else
		{
			bool flag2 = btnName == "HealPoison";
			if (flag2)
			{
				CombatDomainMethod.Call.GmCmd_ForceHealAllPoison();
			}
			else
			{
				bool flag3 = btnName == "EnemyDie";
				if (flag3)
				{
					CombatDomainMethod.Call.GmCmd_ForceEnemyDefeat();
				}
				else
				{
					bool flag4 = btnName == "SelfDie";
					if (flag4)
					{
						CombatDomainMethod.Call.GmCmd_ForceSelfDefeat();
					}
					else
					{
						bool flag5 = btnName == "CastSkill";
						if (flag5)
						{
							CombatDomainMethod.Call.GmCmd_ForceEnemyUseSkill(this._enemyCombatSkillList[this.EnemyCombatSkillDropDown.value]);
						}
						else
						{
							bool flag6 = btnName == "EnemyHealInjury";
							if (flag6)
							{
								CombatDomainMethod.Call.GmCmd_ForceEnemyUseOtherAction(0);
							}
							else
							{
								bool flag7 = btnName == "EnemyHealPoison";
								if (flag7)
								{
									CombatDomainMethod.Call.GmCmd_ForceEnemyUseOtherAction(1);
								}
								else
								{
									bool flag8 = btnName == "SelfGetTrick";
									if (flag8)
									{
										CombatDomainMethod.Call.GmCmd_AddTrick(true, (sbyte)this.TrickTypeDropdown.value);
									}
									else
									{
										bool flag9 = btnName == "EnemyGetTrick";
										if (flag9)
										{
											CombatDomainMethod.Call.GmCmd_AddTrick(false, (sbyte)this.TrickTypeDropdown.value);
										}
										else
										{
											bool flag10 = btnName == "SelfNeiliChange";
											if (flag10)
											{
												short[] list = new short[4];
												for (int i = 0; i < list.Length; i++)
												{
													string text = this.NeiliChangeBoard.transform.Find(string.Format("Neili_{0}", i)).GetComponent<TMP_InputField>().text;
													short value;
													list[i] = (short.TryParse(text, out value) ? value : 0);
												}
												CombatDomainMethod.Call.GmCmd_SetNeiliAllocation(true, list);
											}
											else
											{
												bool flag11 = btnName == "EnemyNeiliChange";
												if (flag11)
												{
													short[] list2 = new short[4];
													for (int j = 0; j < list2.Length; j++)
													{
														string text2 = this.NeiliChangeBoard.transform.Find(string.Format("Neili_{0}", j)).GetComponent<TMP_InputField>().text;
														short value2;
														list2[j] = (short.TryParse(text2, out value2) ? value2 : 0);
													}
													CombatDomainMethod.Call.GmCmd_SetNeiliAllocation(false, list2);
												}
												else
												{
													bool flag12 = btnName == "RecoverBreathAndStance";
													if (flag12)
													{
														CombatDomainMethod.Call.GmCmd_ForceRecoverBreathAndStance();
													}
													else
													{
														bool flag13 = btnName.EndsWith("GetSign");
														if (flag13)
														{
															bool isSelf = btnName.StartsWith("Self");
															this.OnClickGetSign(isSelf);
														}
														else
														{
															bool flag14 = btnName.EndsWith("ClearSign");
															if (flag14)
															{
																bool isSelf2 = btnName.StartsWith("Self");
																this.OnClickClearSign(isSelf2);
															}
															else
															{
																bool flag15 = btnName == "SetImmortal";
																if (flag15)
																{
																	CombatDomainMethod.Call.GmCmd_SetImmortal(this.SetImmortalParm1.isOn, this.SetImmortalParm2.isOn);
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

	// Token: 0x06002177 RID: 8567 RVA: 0x000F3C90 File Offset: 0x000F1E90
	private void OnClickGetSign(bool isAlly)
	{
		sbyte subType = (sbyte)this.CombatSignBodyDropdown.value;
		int count;
		bool flag = !int.TryParse(this.CombatSignAmount.text, out count);
		if (flag)
		{
			count = 0;
		}
		bool flag2 = count <= 0;
		if (!flag2)
		{
			switch (this.CombatSignDropdown.value)
			{
			case 0:
				CombatDomainMethod.Call.GmCmd_AddAllDefeatMark(isAlly, count);
				break;
			case 1:
				CombatDomainMethod.Call.GmCmd_AddInjury(isAlly, subType, true, count);
				break;
			case 2:
				CombatDomainMethod.Call.GmCmd_AddInjury(isAlly, subType, false, count);
				break;
			case 3:
				CombatDomainMethod.Call.GmCmd_AddFlaw(isAlly, subType, count);
				break;
			case 4:
				CombatDomainMethod.Call.GmCmd_AddAcupoint(isAlly, subType, count);
				break;
			case 5:
				CombatDomainMethod.Call.GmCmd_AddPoison(isAlly, subType, count);
				break;
			case 6:
				CombatDomainMethod.Call.GmCmd_AddDie(isAlly, count);
				break;
			case 7:
				CombatDomainMethod.Call.GmCmd_AddMind(isAlly, count);
				break;
			case 8:
				CombatDomainMethod.Call.GmCmd_AddFatal(isAlly, count);
				break;
			case 9:
				CombatDomainMethod.Call.GmCmd_AddInjury(isAlly, subType, true, count, true);
				CombatDomainMethod.Call.GmCmd_AddInjury(isAlly, subType, false, count, true);
				break;
			case 10:
				CombatDomainMethod.Call.GmCmd_AddPoison(isAlly, subType, count, true);
				break;
			default:
				throw new Exception(string.Format("CombatSignDropdown.value={0}", this.CombatSignDropdown.value));
			}
		}
	}

	// Token: 0x06002178 RID: 8568 RVA: 0x000F3DD0 File Offset: 0x000F1FD0
	private void OnClickClearSign(bool isAlly)
	{
		switch (this.CombatSignDropdown.value)
		{
		case 0:
			CombatDomainMethod.Call.GmCmd_HealAllDefeatMark(isAlly);
			break;
		case 1:
			CombatDomainMethod.Call.GmCmd_HealInjury(isAlly, true);
			break;
		case 2:
			CombatDomainMethod.Call.GmCmd_HealInjury(isAlly, false);
			break;
		case 3:
			CombatDomainMethod.Call.GmCmd_HealAllFlaw(isAlly);
			break;
		case 4:
			CombatDomainMethod.Call.GmCmd_HealAllAcupoint(isAlly);
			break;
		case 5:
		case 10:
			CombatDomainMethod.Call.GmCmd_ForceHealAllPoison(isAlly);
			break;
		case 6:
			CombatDomainMethod.Call.GmCmd_HealAllDie(isAlly);
			break;
		case 7:
			CombatDomainMethod.Call.GmCmd_HealAllMind(isAlly);
			break;
		case 8:
			CombatDomainMethod.Call.GmCmd_HealAllFatal(isAlly);
			break;
		case 9:
			CombatDomainMethod.Call.GmCmd_ForceHealAllInjury(isAlly);
			break;
		default:
			throw new Exception(string.Format("CombatSignDropdown.value={0}", this.CombatSignDropdown.value));
		}
	}

	// Token: 0x06002179 RID: 8569 RVA: 0x000F3E9C File Offset: 0x000F209C
	private void UpdateEnemyCombatSkillList()
	{
		List<string> skillNameList = new List<string>();
		this._enemyCombatSkillList.Clear();
		this._enemyCombatSkillList.AddRange(this._enemyAttackSkillList);
		this._enemyCombatSkillList.AddRange(this._enemyAgileSkillList);
		this._enemyCombatSkillList.AddRange(this._enemyDefenseSkillList);
		this._enemyCombatSkillList.RemoveAll((short id) => id < 0);
		using (List<short>.Enumerator enumerator = this._enemyCombatSkillList.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				int combatSkillId = (int)enumerator.Current;
				skillNameList.Add(CombatSkill.Instance[combatSkillId].Name);
			}
		}
		this.EnemyCombatSkillDropDown.ClearOptions();
		this.EnemyCombatSkillDropDown.AddOptions(skillNameList);
		this.EnemyCombatSkillDropDown.interactable = (this._enemyCombatSkillList.Count > 0);
		bool interactable = this.EnemyCombatSkillDropDown.interactable;
		if (interactable)
		{
			this.EnemyCombatSkillDropDown.value = 0;
		}
		this.CastSkillBtn.interactable = (this._enemyCombatSkillList.Count > 0);
	}

	// Token: 0x0600217A RID: 8570 RVA: 0x000F3FE0 File Offset: 0x000F21E0
	private IEnumerable<int> Range(int count)
	{
		int num;
		for (int i = 0; i < count; i = num + 1)
		{
			yield return i;
			num = i;
		}
		yield break;
	}

	// Token: 0x040019CD RID: 6605
	private readonly string[] _signTypeName = new string[]
	{
		"全部",
		"内伤",
		"外伤",
		"破绽",
		"点穴",
		"中毒",
		"必死",
		"心神",
		"重创",
		"旧伤",
		"顽毒"
	};

	// Token: 0x040019CE RID: 6606
	public CDropdownLegacy TrickTypeDropdown;

	// Token: 0x040019CF RID: 6607
	public CDropdownLegacy CombatSignDropdown;

	// Token: 0x040019D0 RID: 6608
	public CDropdownLegacy CombatSignBodyDropdown;

	// Token: 0x040019D1 RID: 6609
	public CDropdownLegacy EnemyCombatSkillDropDown;

	// Token: 0x040019D2 RID: 6610
	public TMP_InputField CombatSignAmount;

	// Token: 0x040019D3 RID: 6611
	public GameObject NeiliChangeBoard;

	// Token: 0x040019D4 RID: 6612
	public GameObject Mask;

	// Token: 0x040019D5 RID: 6613
	public CButtonObsolete CastSkillBtn;

	// Token: 0x040019D6 RID: 6614
	private int _gameDataListenerId = -1;

	// Token: 0x040019D7 RID: 6615
	private sbyte _combatStatus;

	// Token: 0x040019D8 RID: 6616
	private int _enemyCharId = -1;

	// Token: 0x040019D9 RID: 6617
	private readonly List<short> _enemyCombatSkillList = new List<short>();

	// Token: 0x040019DA RID: 6618
	private readonly List<short> _enemyAttackSkillList = new List<short>();

	// Token: 0x040019DB RID: 6619
	private readonly List<short> _enemyAgileSkillList = new List<short>();

	// Token: 0x040019DC RID: 6620
	private readonly List<short> _enemyDefenseSkillList = new List<short>();

	// Token: 0x040019DD RID: 6621
	private readonly uint[] _enemySKillSubIds = new uint[]
	{
		52U,
		53U,
		54U
	};

	// Token: 0x040019DE RID: 6622
	private GMCombatEditor.ESignBodyOptionsType _signType;

	// Token: 0x040019DF RID: 6623
	public CToggleObsolete SetImmortalParm1;

	// Token: 0x040019E0 RID: 6624
	public CToggleObsolete SetImmortalParm2;

	// Token: 0x02001495 RID: 5269
	private enum ESignType
	{
		// Token: 0x0400A1B3 RID: 41395
		All,
		// Token: 0x0400A1B4 RID: 41396
		InnerInjury,
		// Token: 0x0400A1B5 RID: 41397
		OuterInjury,
		// Token: 0x0400A1B6 RID: 41398
		Flaw,
		// Token: 0x0400A1B7 RID: 41399
		Acupoint,
		// Token: 0x0400A1B8 RID: 41400
		Poison,
		// Token: 0x0400A1B9 RID: 41401
		Die,
		// Token: 0x0400A1BA RID: 41402
		Mind,
		// Token: 0x0400A1BB RID: 41403
		Fatal,
		// Token: 0x0400A1BC RID: 41404
		OldInjury,
		// Token: 0x0400A1BD RID: 41405
		OldPoison
	}

	// Token: 0x02001496 RID: 5270
	private enum ESignBodyOptionsType
	{
		// Token: 0x0400A1BF RID: 41407
		Invalid,
		// Token: 0x0400A1C0 RID: 41408
		All,
		// Token: 0x0400A1C1 RID: 41409
		BodyPart,
		// Token: 0x0400A1C2 RID: 41410
		Poison
	}
}
