using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Config;
using FrameWork;
using GameData.Common;
using GameData.Domains.CombatSkill;
using GameData.GameDataBridge;
using GameData.Serializer;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000286 RID: 646
public class MouseTipCombatSkillBuff : MouseTipBase
{
	// Token: 0x1700048A RID: 1162
	// (get) Token: 0x060029A2 RID: 10658 RVA: 0x0013B320 File Offset: 0x00139520
	protected override bool CanStick
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060029A3 RID: 10659 RVA: 0x0013B324 File Offset: 0x00139524
	protected override void Init(ArgumentBox argsBox)
	{
		this._skills.Clear();
		this._directionReceived.Clear();
		this._powerReceived.Clear();
		this._cdFrameReceived.Clear();
		this._canAffectReceived.Clear();
		argsBox.Get("CombatBegin", out this._combatBegin);
		argsBox.Get<List<short>>("CombatSkillIdList", out this._combatSkillIdList);
		argsBox.Get("CharId", out this._charId);
		argsBox.Get("IsNeiGong", out this._isNeiGong);
		argsBox.Get("IsAlly", out this._isAlly);
		this.isInited = true;
	}

	// Token: 0x060029A4 RID: 10660 RVA: 0x0013B3D0 File Offset: 0x001395D0
	public override void InitMonitorFieldIds()
	{
		bool flag = this._combatSkillIdList == null;
		if (!flag)
		{
			foreach (short skillId in this._combatSkillIdList)
			{
				this._skills.Add(skillId, new ValueTuple<sbyte, short, short, bool>(0, 0, 0, false));
				this.MonitorFields.Add(new UIBase.MonitorDataField(7, 0, (ulong)new CombatSkillKey(this._charId, skillId), new uint[]
				{
					12U,
					9U
				}));
				bool flag2 = !this._combatBegin;
				if (flag2)
				{
					this.MonitorFields.Add(new UIBase.MonitorDataField(8, 29, (ulong)new CombatSkillKey(this._charId, skillId), new uint[]
					{
						2U,
						9U
					}));
				}
			}
		}
	}

	// Token: 0x060029A5 RID: 10661 RVA: 0x0013B4C8 File Offset: 0x001396C8
	private void Awake()
	{
		this._title = base.CGet<TextMeshProUGUI>("Title");
		for (int i = 1; i <= 5; i++)
		{
			this._rows.Add(base.CGet<GameObject>(string.Format("Row{0}", i)));
		}
		for (int j = 0; j < 5; j++)
		{
			this._rows[j].transform.GetChild(0).gameObject.SetActive(false);
			this._rows[j].transform.GetChild(1).gameObject.SetActive(false);
			this._rows[j].SetActive(false);
		}
	}

	// Token: 0x060029A6 RID: 10662 RVA: 0x0013B588 File Offset: 0x00139788
	protected override void OnDisable()
	{
		base.OnDisable();
		for (int i = 0; i < 5; i++)
		{
			this._rows[i].transform.GetChild(0).gameObject.SetActive(false);
			this._rows[i].transform.GetChild(1).gameObject.SetActive(false);
			this._rows[i].SetActive(false);
		}
	}

	// Token: 0x060029A7 RID: 10663 RVA: 0x0013B608 File Offset: 0x00139808
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
				bool flag = uid.DomainId == 7 && uid.DataId == 0;
				if (flag)
				{
					short skillId = ((CombatSkillKey)uid.SubId0).SkillTemplateId;
					uint subId = uid.SubId1;
					uint num = subId;
					if (num != 9U)
					{
						if (num == 12U)
						{
							sbyte direction = sbyte.MinValue;
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref direction);
							this._skills[skillId] = new ValueTuple<sbyte, short, short, bool>(direction, this._skills[skillId].Item2, this._skills[skillId].Item3, this._skills[skillId].Item4);
							this._directionReceived.Add(skillId);
							bool flag2 = this.IsDataReceived();
							if (flag2)
							{
								this.ProcessDisplayData();
							}
						}
					}
					else
					{
						short power = short.MinValue;
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref power);
						this._skills[skillId] = new ValueTuple<sbyte, short, short, bool>(this._skills[skillId].Item1, power, this._skills[skillId].Item3, this._skills[skillId].Item4);
						this._powerReceived.Add(skillId);
						bool flag3 = this.IsDataReceived();
						if (flag3)
						{
							this.ProcessDisplayData();
						}
					}
				}
				else
				{
					bool flag4 = uid.DomainId == 8 && uid.DataId == 29;
					if (flag4)
					{
						short skillId2 = ((CombatSkillKey)uid.SubId0).SkillTemplateId;
						uint subId2 = uid.SubId1;
						uint num2 = subId2;
						if (num2 != 2U)
						{
							if (num2 == 9U)
							{
								bool canEffect = false;
								Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref canEffect);
								this._skills[skillId2] = new ValueTuple<sbyte, short, short, bool>(this._skills[skillId2].Item1, this._skills[skillId2].Item2, this._skills[skillId2].Item3, canEffect);
								this._canAffectReceived.Add(skillId2);
								bool flag5 = this.IsDataReceived();
								if (flag5)
								{
									this.ProcessDisplayData();
								}
							}
						}
						else
						{
							short cdFrame = short.MinValue;
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref cdFrame);
							this._skills[skillId2] = new ValueTuple<sbyte, short, short, bool>(this._skills[skillId2].Item1, this._skills[skillId2].Item2, cdFrame, this._skills[skillId2].Item4);
							this._cdFrameReceived.Add(skillId2);
							bool flag6 = this.IsDataReceived();
							if (flag6)
							{
								this.ProcessDisplayData();
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x060029A8 RID: 10664 RVA: 0x0013B95C File Offset: 0x00139B5C
	private bool IsDataReceived()
	{
		foreach (short skillId in this._skills.Keys)
		{
			bool combatBegin = this._combatBegin;
			if (combatBegin)
			{
				bool flag = !this._directionReceived.Contains(skillId) || !this._powerReceived.Contains(skillId);
				if (flag)
				{
					return false;
				}
			}
			else
			{
				bool flag2 = !this._directionReceived.Contains(skillId) || !this._powerReceived.Contains(skillId) || !this._cdFrameReceived.Contains(skillId) || !this._canAffectReceived.Contains(skillId);
				if (flag2)
				{
					return false;
				}
			}
		}
		return true;
	}

	// Token: 0x060029A9 RID: 10665 RVA: 0x0013BA40 File Offset: 0x00139C40
	private void ProcessDisplayData()
	{
		bool flag = !this.isInited;
		if (!flag)
		{
			this.isInited = false;
			this._title.text = LocalStringManager.Get(this._isNeiGong ? LanguageKey.LK_CombatSkillType_0 : LanguageKey.LK_CombatSkillType_2);
			int total = 0;
			foreach (ValueTuple<sbyte, short, short, bool> data in this._skills.Values)
			{
				sbyte item = data.Item1;
				bool flag2 = item == 0 || item == 1;
				if (flag2)
				{
					total++;
				}
			}
			bool flag3 = total <= 5;
			if (flag3)
			{
				int count = 0;
				foreach (KeyValuePair<short, ValueTuple<sbyte, short, short, bool>> keyValuePair in this._skills)
				{
					short num;
					ValueTuple<sbyte, short, short, bool> valueTuple;
					keyValuePair.Deconstruct(out num, out valueTuple);
					short skillId = num;
					ValueTuple<sbyte, short, short, bool> skillData = valueTuple;
					bool flag4 = skillData.Item1 != 0 && skillData.Item1 != 1;
					if (!flag4)
					{
						GameObject obj = this._rows[count].transform.GetChild(0).gameObject;
						this._rows[count++].SetActive(true);
						obj.SetActive(true);
						this.FillData(obj.GetComponent<Refers>(), skillId);
					}
				}
			}
			else
			{
				int count2 = 0;
				bool isFirstChild = true;
				foreach (KeyValuePair<short, ValueTuple<sbyte, short, short, bool>> keyValuePair in this._skills)
				{
					short num;
					ValueTuple<sbyte, short, short, bool> valueTuple;
					keyValuePair.Deconstruct(out num, out valueTuple);
					short skillId2 = num;
					ValueTuple<sbyte, short, short, bool> skillData2 = valueTuple;
					bool flag5 = skillData2.Item1 != 0 && skillData2.Item1 != 1;
					if (!flag5)
					{
						int index = isFirstChild ? 0 : 1;
						GameObject obj2 = this._rows[count2].transform.GetChild(index).gameObject;
						this._rows[count2].SetActive(true);
						obj2.SetActive(true);
						this.FillData(obj2.GetComponent<Refers>(), skillId2);
						bool flag6 = !isFirstChild;
						if (flag6)
						{
							count2++;
						}
						isFirstChild = !isFirstChild;
						bool flag7 = count2 >= this._rows.Count;
						if (flag7)
						{
							break;
						}
					}
				}
			}
			LayoutRebuilder.ForceRebuildLayoutImmediate(base.GetComponent<RectTransform>());
			this.Element.ShowAfterRefresh();
		}
	}

	// Token: 0x060029AA RID: 10666 RVA: 0x0013BD08 File Offset: 0x00139F08
	private void FillData(Refers refers, short skillId)
	{
		CombatSkillItem config = CombatSkill.Instance[skillId];
		StringBuilder stringBuilder = new StringBuilder();
		CImage forwardOrReverse = refers.CGet<CImage>("ForwardOrReverse");
		TextMeshProUGUI description = refers.CGet<TextMeshProUGUI>("SkillDescription");
		TextMeshProUGUI extraDescription = refers.CGet<TextMeshProUGUI>("SkillExtraDescription");
		bool isLocked = (!this._skills[skillId].Item4 || this._skills[skillId].Item3 != 0) && !this._combatBegin;
		bool isUpgraded = true;
		refers.CGet<CImage>("Icon").SetSprite(config.Icon, false, null);
		refers.CGet<GameObject>("Locked").SetActive(isLocked);
		refers.CGet<CImage>("GradeBack").SetSprite(ItemView.GetGradeIcon(config.Grade), false, null);
		refers.CGet<TextMeshProUGUI>("Grade").text = ItemView.GetGradeText(config.Grade);
		refers.CGet<TextMeshProUGUI>("SkillName").text = this.BuildString(stringBuilder, Colors.Instance.GradeColors[(int)config.Grade].ColorToHexString("#"), config.Name);
		refers.CGet<TextMeshProUGUI>("Power").text = string.Format("{0}: {1}%", LocalStringManager.Get(LanguageKey.LK_CombatSkill_Power), this._skills[skillId].Item2);
		sbyte item = this._skills[skillId].Item1;
		sbyte b = item;
		if (b != 0)
		{
			if (b != 1)
			{
				forwardOrReverse.gameObject.SetActive(false);
				description.gameObject.SetActive(false);
				isUpgraded = false;
			}
			else
			{
				forwardOrReverse.SetSprite("mousetip_zhengni_1", false, null);
				forwardOrReverse.gameObject.SetActive(true);
				description.text = this.BuildString(stringBuilder, this.GetColorString(!isLocked, false), CommonUtils.GetSpecialEffectDesc((int)skillId, false));
				description.gameObject.SetActive(true);
			}
		}
		else
		{
			forwardOrReverse.SetSprite("mousetip_zhengni_0", false, null);
			forwardOrReverse.gameObject.SetActive(true);
			description.text = this.BuildString(stringBuilder, this.GetColorString(!isLocked, true), CommonUtils.GetSpecialEffectDesc((int)skillId, true));
			description.gameObject.SetActive(true);
		}
		bool flag = isLocked;
		if (flag)
		{
			extraDescription.text = this.BuildString(stringBuilder, Colors.Instance["darkred"].ColorToHexString("#"), LocalStringManager.Get(LanguageKey.LK_Combat_Skill_Locked_Tip));
			extraDescription.gameObject.SetActive(true);
		}
		else
		{
			bool flag2 = !isUpgraded;
			if (flag2)
			{
				extraDescription.text = this.BuildString(stringBuilder, Colors.Instance["grey"].ColorToHexString("#"), LocalStringManager.Get(LanguageKey.LK_Combat_Skill_Not_Break_Out_Tip));
				extraDescription.gameObject.SetActive(true);
			}
			else
			{
				extraDescription.gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x060029AB RID: 10667 RVA: 0x0013BFF4 File Offset: 0x0013A1F4
	private string GetColorString(bool canUse, bool isDirect)
	{
		return Colors.Instance[canUse ? (isDirect ? "brightblue" : "brightred") : "grey"].ColorToHexString("#");
	}

	// Token: 0x060029AC RID: 10668 RVA: 0x0013C034 File Offset: 0x0013A234
	private string BuildString(StringBuilder builder, string color, string text)
	{
		builder.Clear();
		builder.Append("<color=");
		builder.Append(color);
		builder.Append(">");
		builder.Append(text);
		builder.Append("</color>");
		return builder.ToString();
	}

	// Token: 0x04001E38 RID: 7736
	private const string Forward = "mousetip_zhengni_0";

	// Token: 0x04001E39 RID: 7737
	private const string Reverse = "mousetip_zhengni_1";

	// Token: 0x04001E3A RID: 7738
	private const sbyte InvalidDirection = -128;

	// Token: 0x04001E3B RID: 7739
	private const short InvalidPower = -32768;

	// Token: 0x04001E3C RID: 7740
	private const short InvalidCdFrame = -32768;

	// Token: 0x04001E3D RID: 7741
	private bool isInited = false;

	// Token: 0x04001E3E RID: 7742
	private HashSet<short> _directionReceived = new HashSet<short>();

	// Token: 0x04001E3F RID: 7743
	private HashSet<short> _powerReceived = new HashSet<short>();

	// Token: 0x04001E40 RID: 7744
	private HashSet<short> _cdFrameReceived = new HashSet<short>();

	// Token: 0x04001E41 RID: 7745
	private HashSet<short> _canAffectReceived = new HashSet<short>();

	// Token: 0x04001E42 RID: 7746
	[TupleElementNames(new string[]
	{
		"Direction",
		"Power",
		"CdFrame",
		"CanAffect"
	})]
	private Dictionary<short, ValueTuple<sbyte, short, short, bool>> _skills = new Dictionary<short, ValueTuple<sbyte, short, short, bool>>();

	// Token: 0x04001E43 RID: 7747
	private List<short> _combatSkillIdList;

	// Token: 0x04001E44 RID: 7748
	private int _charId;

	// Token: 0x04001E45 RID: 7749
	private bool _isNeiGong;

	// Token: 0x04001E46 RID: 7750
	private bool _isAlly;

	// Token: 0x04001E47 RID: 7751
	private bool _combatBegin;

	// Token: 0x04001E48 RID: 7752
	private List<GameObject> _rows = new List<GameObject>();

	// Token: 0x04001E49 RID: 7753
	private TextMeshProUGUI _title;
}
