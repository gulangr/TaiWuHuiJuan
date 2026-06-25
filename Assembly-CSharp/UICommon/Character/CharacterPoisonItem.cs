using System;
using System.Collections.Generic;
using CharacterDataMonitor;
using Config;
using DG.Tweening;
using FrameWork;
using GameData.Domains.Item;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace UICommon.Character
{
	// Token: 0x020005E5 RID: 1509
	public class CharacterPoisonItem : CharacterUIElement
	{
		// Token: 0x170008F9 RID: 2297
		// (get) Token: 0x0600473A RID: 18234 RVA: 0x00215969 File Offset: 0x00213B69
		private InjuryPoisonMonitor Item
		{
			get
			{
				return this.MonitorDataItem as InjuryPoisonMonitor;
			}
		}

		// Token: 0x170008FA RID: 2298
		// (get) Token: 0x0600473B RID: 18235 RVA: 0x00215976 File Offset: 0x00213B76
		// (set) Token: 0x0600473C RID: 18236 RVA: 0x0021597E File Offset: 0x00213B7E
		public sbyte PoisonLevel { get; private set; }

		// Token: 0x0600473D RID: 18237 RVA: 0x00215988 File Offset: 0x00213B88
		public CharacterPoisonItem(Refers refers, sbyte poisonType, GameObject poisonedMark = null, CharacterPoisonGroup group = null)
		{
			bool flag = null == refers;
			if (flag)
			{
				throw new Exception("refers can not be null to create CharacterPoisonItem");
			}
			this._immuneHideObjects = new List<GameObject>();
			this.PoisonLevel = 0;
			Refers progressBarRefers;
			bool flag2 = refers.CTryGet<Refers>("ProgressBar", out progressBarRefers);
			if (flag2)
			{
				this.AddImmuneHideObject((progressBarRefers != null) ? progressBarRefers.gameObject : null);
			}
			this.PoisonType = poisonType;
			PoisonItem config = Poison.Instance[poisonType];
			this._poisonLabel = null;
			bool flag3 = refers.CTryGet<TextMeshProUGUI>("PoisonValue", out this._poisonLabel) && null != this._poisonLabel;
			if (flag3)
			{
				this._poisonLabel.text = "-";
			}
			this._poisonResistLabel = null;
			bool flag4 = refers.CTryGet<TextMeshProUGUI>("PoisonResist", out this._poisonResistLabel) && null != this._poisonResistLabel;
			if (flag4)
			{
				this._poisonResistLabel.text = "-";
			}
			GameObject poisonValueLayout;
			bool flag5 = refers.CTryGet<GameObject>("PoisonValueLayout", out poisonValueLayout);
			if (flag5)
			{
				this.AddImmuneHideObject(poisonValueLayout);
			}
			bool flag6 = null != progressBarRefers;
			if (flag6)
			{
				this._progressBar = new ProgressBarFill(1000f, progressBarRefers, this._poisonLabel, this._poisonResistLabel)
				{
					GetProgressString = new Func<float, ValueTuple<string, string>>(this.GetPoisonShowString)
				};
			}
			bool flag7 = refers.CTryGet<TooltipInvoker>("MouseTip", out this._mouseTip) && null != this._mouseTip;
			if (flag7)
			{
				this._mouseTip.Type = TipType.Simple;
				this._mouseTip.IsLanguageKey = false;
				this._mouseTip.PresetParam = new string[]
				{
					config.Name,
					string.Empty
				};
			}
			CImage icon;
			bool flag8 = refers.CTryGet<CImage>("Icon", out icon) && null != icon;
			if (flag8)
			{
				icon.SetSprite(config.Icon, false, null);
			}
			TextMeshProUGUI nameLabel;
			bool flag9 = refers.CTryGet<TextMeshProUGUI>("Name", out nameLabel) && null != nameLabel;
			if (flag9)
			{
				nameLabel.text = config.Name;
			}
			refers.CTryGet<GameObject>("Immune", out this._immuneShowObject);
			this._newAddPoisonObj = refers.CGet<GameObject>("NewAddPoison");
			this._newAddPoisonResistObj = refers.CGet<GameObject>("NewAddResist");
			this._newAddPoisonLabel = refers.CGet<TextMeshProUGUI>("NewAddPoisonValue");
			this._newAddPoisonResistLabel = refers.CGet<TextMeshProUGUI>("NewAddResistValue");
			this._poisonDotList = refers.CGetList<CImage>("Dot_");
			bool flag10 = this._poisonDotList.Count > 0;
			if (flag10)
			{
				this.AddImmuneHideObject(this._poisonDotList[0].transform.parent.parent.gameObject);
			}
			this._poisonedMark = poisonedMark;
			this.Group = group;
		}

		// Token: 0x0600473E RID: 18238 RVA: 0x00215C54 File Offset: 0x00213E54
		private void AddImmuneHideObject(GameObject obj)
		{
			bool flag = null != obj && !this._immuneHideObjects.Contains(obj);
			if (flag)
			{
				this._immuneHideObjects.Add(obj);
			}
		}

		// Token: 0x0600473F RID: 18239 RVA: 0x00215C8E File Offset: 0x00213E8E
		public void SetInitValue(int poisonValue)
		{
			ProgressBarFill progressBar = this._progressBar;
			if (progressBar != null)
			{
				progressBar.SetInitValue(poisonValue);
			}
		}

		// Token: 0x06004740 RID: 18240 RVA: 0x00215CA4 File Offset: 0x00213EA4
		public override MonitorDataItemBase GetMonitorItem(int charId)
		{
			return SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<InjuryPoisonMonitor>(charId, this.IsDead);
		}

		// Token: 0x06004741 RID: 18241 RVA: 0x00215CC8 File Offset: 0x00213EC8
		internal override void BindEvent()
		{
			bool flag = this.Group != null;
			if (!flag)
			{
				this.Item.AddPoisonsListener(new Action<sbyte>(this.FillItem));
				this.Item.AddPoisonResistsListener(new Action<sbyte>(this.FillItem));
			}
		}

		// Token: 0x06004742 RID: 18242 RVA: 0x00215D18 File Offset: 0x00213F18
		public override void UnbindEvent()
		{
			bool flag = this.Group != null;
			if (!flag)
			{
				this.Item.RemovePoisonsListener(new Action<sbyte>(this.FillItem));
				this.Item.RemovePoisonResistsListener(new Action<sbyte>(this.FillItem));
			}
		}

		// Token: 0x06004743 RID: 18243 RVA: 0x00215D65 File Offset: 0x00213F65
		public override void FillElement()
		{
			this.FillItem(this.PoisonType);
		}

		// Token: 0x06004744 RID: 18244 RVA: 0x00215D78 File Offset: 0x00213F78
		public bool FillImmuneInfo()
		{
			bool immuneFlag = this.Item.IsImmune(this.PoisonType);
			bool flag = null != this._immuneShowObject;
			if (flag)
			{
				this._immuneShowObject.SetActive(immuneFlag);
				bool immuneFlag2 = immuneFlag;
				if (immuneFlag2)
				{
					TextMeshProUGUI immuneText = this._immuneShowObject.GetComponentInChildren<TextMeshProUGUI>();
					bool flag2 = null != immuneText;
					if (flag2)
					{
						bool isBornImmune = this.Item.IsBornImmune(this.PoisonType);
						immuneText.text = LocalStringManager.Get(isBornImmune ? LanguageKey.LK_PoisonImmuneSinceBorn : LanguageKey.LK_PoisonImmune);
					}
				}
			}
			List<GameObject> immuneHideObjects = this._immuneHideObjects;
			if (immuneHideObjects != null)
			{
				immuneHideObjects.ForEach(delegate(GameObject e)
				{
					if (e != null)
					{
						e.SetActive(!immuneFlag);
					}
				});
			}
			this.RefreshTip();
			return immuneFlag;
		}

		// Token: 0x06004745 RID: 18245 RVA: 0x00215E50 File Offset: 0x00214050
		public void FillAsCombatState(int oldPoison)
		{
			bool flag = this.FillImmuneInfo();
			if (!flag)
			{
				int poisonNow = this.Item.Poisons[(int)this.PoisonType];
				int oldPoisonLevel = (int)PoisonsAndLevels.CalcPoisonedLevel(oldPoison);
				this.PoisonLevel = PoisonsAndLevels.CalcPoisonedLevel(poisonNow);
				for (int i = 0; i < this._poisonDotList.Count; i++)
				{
					this._poisonDotList[i].gameObject.SetActive(i < (int)this.PoisonLevel);
					bool flag2 = i < (int)this.PoisonLevel;
					if (flag2)
					{
						DOTweenAnimation animation = this._poisonDotList[i].GetComponent<DOTweenAnimation>();
						animation.DOPause();
						animation.DOGoto(0f, false);
						bool flag3 = i >= oldPoisonLevel;
						if (flag3)
						{
							animation.DORestart();
						}
					}
				}
				bool flag4 = null != this._poisonedMark;
				if (flag4)
				{
					this._poisonedMark.SetActive(this.PoisonLevel > 0);
				}
				bool flag5 = this._progressBar != null;
				if (flag5)
				{
					short maxValue = 25000;
					bool flag6 = GlobalConfig.Instance.PoisonLevelThresholds.CheckIndex((int)this.PoisonLevel);
					if (flag6)
					{
						maxValue = GlobalConfig.Instance.PoisonLevelThresholds[(int)this.PoisonLevel];
					}
					this._progressBar.MaxValue = (float)maxValue;
					this._progressBar.SetProgressWithNewlyValue((float)oldPoison, (float)poisonNow);
				}
				else
				{
					bool flag7 = null != this._poisonLabel;
					if (flag7)
					{
						this._poisonLabel.text = oldPoison.ToString();
					}
				}
				this._newAddPoisonObj.SetActive(poisonNow != oldPoison);
				bool flag8 = poisonNow != oldPoison;
				if (flag8)
				{
					this._newAddPoisonLabel.text = ((poisonNow > oldPoison) ? "+" : "") + string.Format("{0}", poisonNow - oldPoison);
				}
				this._newAddPoisonResistObj.SetActive(false);
				this.RefreshTip();
			}
		}

		// Token: 0x06004746 RID: 18246 RVA: 0x00216040 File Offset: 0x00214240
		public override void ResetToEmpty()
		{
			bool flag = this._progressBar != null;
			if (flag)
			{
				this._progressBar.MaxValue = 1000f;
				this._progressBar.SetValueWithoutAnimation(0f);
			}
			bool flag2 = null != this._poisonLabel;
			if (flag2)
			{
				this._poisonLabel.text = "-";
			}
			bool flag3 = null != this._poisonResistLabel;
			if (flag3)
			{
				this._poisonResistLabel.text = "-";
			}
			bool flag4 = null != this._mouseTip;
			if (flag4)
			{
				this._mouseTip.PresetParam[1] = (Poison.Instance[this.PoisonType].Desc ?? "");
			}
			for (int i = 0; i < this._poisonDotList.Count; i++)
			{
				DOTweenAnimation animation = this._poisonDotList[i].GetComponent<DOTweenAnimation>();
				animation.DOPause();
				animation.DOGoto(0f, false);
				this._poisonDotList[i].color = this._poisonDotList[i].color.SetAlpha(1f);
				this._poisonDotList[i].gameObject.SetActive(false);
			}
			bool flag5 = null != this._poisonedMark;
			if (flag5)
			{
				this._poisonedMark.SetActive(false);
			}
			bool flag6 = null != this._newAddPoisonObj;
			if (flag6)
			{
				this._newAddPoisonObj.SetActive(false);
			}
			bool flag7 = null != this._newAddPoisonResistObj;
			if (flag7)
			{
				this._newAddPoisonResistObj.SetActive(false);
			}
			this.PoisonLevel = 0;
		}

		// Token: 0x06004747 RID: 18247 RVA: 0x002161FC File Offset: 0x002143FC
		private ValueTuple<string, string> GetPoisonShowString(float poisonValue)
		{
			bool flag = this.Item == null;
			ValueTuple<string, string> result;
			if (flag)
			{
				result = new ValueTuple<string, string>("-", "-");
			}
			else
			{
				result = new ValueTuple<string, string>(string.Format("{0}", this.Item.Poisons[(int)this.PoisonType]), string.Format("{0}", this.Item.PoisonResists[(int)this.PoisonType]));
			}
			return result;
		}

		// Token: 0x06004748 RID: 18248 RVA: 0x00216274 File Offset: 0x00214474
		private void FillItem(sbyte type)
		{
			int poison = this.Item.Poisons[(int)type];
			this.PoisonLevel = PoisonsAndLevels.CalcPoisonedLevel(poison);
			bool isCombatCharacter = this.Group.IsCombatCharacter;
			if (!isCombatCharacter)
			{
				this._newAddPoisonObj.SetActive(false);
				this._newAddPoisonResistObj.SetActive(false);
				bool flag = this.FillImmuneInfo();
				if (!flag)
				{
					bool flag2 = type != this.PoisonType;
					if (!flag2)
					{
						for (int i = 0; i < this._poisonDotList.Count; i++)
						{
							this._poisonDotList[i].gameObject.SetActive(i < (int)this.PoisonLevel);
							DOTweenAnimation animation = this._poisonDotList[i].GetComponent<DOTweenAnimation>();
							animation.DOPause();
							animation.DOGoto(0f, false);
						}
						bool flag3 = null != this._poisonedMark;
						if (flag3)
						{
							this._poisonedMark.SetActive(this.PoisonLevel > 0);
						}
						bool flag4 = this._progressBar != null;
						if (flag4)
						{
							short maxValue = 25000;
							bool flag5 = GlobalConfig.Instance.PoisonLevelThresholds.CheckIndex((int)this.PoisonLevel);
							if (flag5)
							{
								maxValue = GlobalConfig.Instance.PoisonLevelThresholds[(int)this.PoisonLevel];
							}
							this._progressBar.MaxValue = (float)maxValue;
							this._progressBar.Value = (float)poison;
						}
						else
						{
							bool flag6 = null != this._poisonLabel;
							if (flag6)
							{
								this._poisonLabel.text = poison.ToString();
							}
						}
						this.RefreshTip();
					}
				}
			}
		}

		// Token: 0x06004749 RID: 18249 RVA: 0x00216418 File Offset: 0x00214618
		private void RefreshTip()
		{
			bool flag = this._mouseTip;
			if (flag)
			{
				int poisonValue = this.Item.Poisons[(int)this.PoisonType];
				int poisonResist = this.Item.PoisonResists[(int)this.PoisonType];
				CharacterItem characterCfg = Character.Instance.GetItem(this.Item.TemplateId);
				bool isBornImmune = characterCfg.PoisonImmunities[(int)this.PoisonType] || BitOperation.GetBit(this.Item.ImmunePoisonExtra, (int)this.PoisonType);
				this._mouseTip.Type = TipType.CharacterPoison;
				this._mouseTip.RuntimeParam = EasyPool.Get<ArgumentBox>().Set("PoisonType", this.PoisonType).Set("IsBornImmune", isBornImmune).Set("PoisonResist", poisonResist).Set("PoisonValue", poisonValue).Set("PoisonLevel", this.PoisonLevel);
			}
		}

		// Token: 0x0400312E RID: 12590
		private static readonly ushort[] PoisonResistPropertyIdArray = new ushort[]
		{
			28,
			29,
			30,
			31,
			32,
			33
		};

		// Token: 0x0400312F RID: 12591
		public readonly sbyte PoisonType;

		// Token: 0x04003130 RID: 12592
		public readonly CharacterPoisonGroup Group;

		// Token: 0x04003132 RID: 12594
		private readonly List<GameObject> _immuneHideObjects;

		// Token: 0x04003133 RID: 12595
		private readonly GameObject _immuneShowObject;

		// Token: 0x04003134 RID: 12596
		private readonly ProgressBarFill _progressBar;

		// Token: 0x04003135 RID: 12597
		private readonly TextMeshProUGUI _poisonLabel;

		// Token: 0x04003136 RID: 12598
		private readonly TextMeshProUGUI _poisonResistLabel;

		// Token: 0x04003137 RID: 12599
		private readonly List<CImage> _poisonDotList;

		// Token: 0x04003138 RID: 12600
		private readonly TooltipInvoker _mouseTip;

		// Token: 0x04003139 RID: 12601
		private readonly GameObject _poisonedMark;

		// Token: 0x0400313A RID: 12602
		private readonly GameObject _newAddPoisonObj;

		// Token: 0x0400313B RID: 12603
		private readonly GameObject _newAddPoisonResistObj;

		// Token: 0x0400313C RID: 12604
		private readonly TextMeshProUGUI _newAddPoisonLabel;

		// Token: 0x0400313D RID: 12605
		private readonly TextMeshProUGUI _newAddPoisonResistLabel;
	}
}
