using System;
using System.Collections;
using System.Collections.Generic;
using CharacterDataMonitor;
using Config;
using DG.Tweening;
using FrameWork;
using GameData.Domains.Extra;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Taiwu;
using Spine.Unity;
using TMPro;
using UnityEngine;

namespace GearMate
{
	// Token: 0x02000616 RID: 1558
	public class GearMateConsummateUpdatePage : GearMateSubPageBase
	{
		// Token: 0x06004913 RID: 18707 RVA: 0x00222A27 File Offset: 0x00220C27
		private void OnDisable()
		{
			this._playingUpgradeAnim = false;
			this._yellowProgress.transform.GetComponent<DOTweenAnimation>().DORestart();
		}

		// Token: 0x06004914 RID: 18708 RVA: 0x00222A47 File Offset: 0x00220C47
		protected override void InitInternal()
		{
			this.InitRefers();
			this.InitLevelTips();
		}

		// Token: 0x06004915 RID: 18709 RVA: 0x00222A58 File Offset: 0x00220C58
		public override void OnEnableBySwitchPage(int pageIndex)
		{
			bool flag = base.GearMate == null;
			if (!flag)
			{
				this._pageIndex = pageIndex;
				this.RebuildPreviewBySelectedItems();
			}
		}

		// Token: 0x06004916 RID: 18710 RVA: 0x00222A84 File Offset: 0x00220C84
		private void RebuildPreviewBySelectedItems()
		{
			Dictionary<ItemSourceType, Dictionary<ItemKey, int>> dict = base.Parent.ItemSelector.GetTypeToSelectedItemDict(this._pageIndex);
			this.ResetProcessValue(false);
			foreach (KeyValuePair<ItemSourceType, Dictionary<ItemKey, int>> keyValuePair in dict)
			{
				ItemSourceType itemSourceType;
				Dictionary<ItemKey, int> dictionary;
				keyValuePair.Deconstruct(out itemSourceType, out dictionary);
				Dictionary<ItemKey, int> items = dictionary;
				foreach (KeyValuePair<ItemKey, int> item in items)
				{
					this.OnItemChanged(item.Key, item.Value, false, false, false);
				}
			}
		}

		// Token: 0x06004917 RID: 18711 RVA: 0x00222B54 File Offset: 0x00220D54
		public override void UnMonitorFields()
		{
			base.UnMonitorFields();
			this.RemoveConsummateLevelListener();
		}

		// Token: 0x06004918 RID: 18712 RVA: 0x00222B65 File Offset: 0x00220D65
		public override void OnGearMateCharacterIdChanged(int lastId)
		{
			this.ResetConsummateMonitor(base.Parent.GearMateId);
		}

		// Token: 0x06004919 RID: 18713 RVA: 0x00222B7A File Offset: 0x00220D7A
		public override void OnGearMateDataChanged()
		{
			this._currentProcessValue = base.GearMate.ConsummateLevelProgress;
			this.ResetConsummateMonitor(base.GearMate.Id);
		}

		// Token: 0x0600491A RID: 18714 RVA: 0x00222BA0 File Offset: 0x00220DA0
		private void OnGotCurrentConsummateLevel()
		{
			this._currentConsummateLevel = (int)this._consummateMonitor.Level;
			int processPercent;
			this.GetIncreaseCountAndProcessValue(this._currentProcessValue, out processPercent);
			this._currentConsummateLevelIncreaseCount = this.GetFeatureUpgradeLevel(false);
			this._currentProgressPercent = processPercent;
			this._previewProcessValue = this._currentProcessValue;
			this._previewConsummateLevel = this._currentConsummateLevel;
			this._previewProgressPercent = this._currentProgressPercent;
			this._previewConsummateLevelIncreaseCount = this._currentConsummateLevelIncreaseCount;
			this.SetConsummateProgressBar(false);
			this.SetProgressTextValueText(false);
			this.SetProgressPercentText(false);
			this.SetButtonState(false);
			this.SetMachineWaterHeight();
			this.ShowTip();
			this.RebuildPreviewBySelectedItems();
		}

		// Token: 0x0600491B RID: 18715 RVA: 0x00222C47 File Offset: 0x00220E47
		private void ResetConsummateMonitor(int gearMateCharId)
		{
			this.RemoveConsummateLevelListener();
			this._consummateMonitor = SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<ConsummateLevelMonitor>(gearMateCharId, false);
			this._consummateMonitor.AddConsummateLevelListener(new Action(this.OnConsummateChanged));
		}

		// Token: 0x0600491C RID: 18716 RVA: 0x00222C7B File Offset: 0x00220E7B
		private void RemoveConsummateLevelListener()
		{
			ConsummateLevelMonitor consummateMonitor = this._consummateMonitor;
			if (consummateMonitor != null)
			{
				consummateMonitor.RemoveConsummateLevelListener(new Action(this.OnConsummateChanged));
			}
		}

		// Token: 0x0600491D RID: 18717 RVA: 0x00222C9C File Offset: 0x00220E9C
		private void OnConsummateChanged()
		{
			bool playingUpgradeAnim = this._playingUpgradeAnim;
			if (!playingUpgradeAnim)
			{
				this._currentConsummateLevel = (int)this._consummateMonitor.Level;
				this.OnGotCurrentConsummateLevel();
			}
		}

		// Token: 0x0600491E RID: 18718 RVA: 0x00222CD0 File Offset: 0x00220ED0
		private void SetProgressTextValueText(bool isPreview = false)
		{
			bool flag = this.IsMaxLevel();
			if (flag)
			{
				this._progressTextValue.text = "100%";
			}
			else
			{
				this._progressTextValue.text = (isPreview ? this._previewProgressPercent.ToString() : this._currentProgressPercent.ToString()) + "%";
			}
		}

		// Token: 0x0600491F RID: 18719 RVA: 0x00222D2C File Offset: 0x00220F2C
		private void SetProgressPercentText(bool isPreview = false)
		{
			bool flag = this.IsMaxLevel();
			if (flag)
			{
				this._progressPercentText.text = LocalStringManager.Get(LanguageKey.LK_GearMate_Consummate_Max);
			}
			else
			{
				this._progressPercentText.text = (isPreview ? this._previewProgressPercent.ToString() : this._currentProgressPercent.ToString()) + "%";
				bool flag2 = isPreview && this._previewConsummateLevelIncreaseCount > 0;
				if (flag2)
				{
					TextMeshProUGUI progressPercentText = this._progressPercentText;
					progressPercentText.text = progressPercentText.text + " +" + this._previewConsummateLevelIncreaseCount.ToString();
				}
			}
		}

		// Token: 0x06004920 RID: 18720 RVA: 0x00222DCC File Offset: 0x00220FCC
		private void SetConsummateProgressYellow(Action action = null, float time = 0f)
		{
			this._yellowProgress.color = new Color(1f, 1f, 1f, 1f);
			float processPercent = (float)(45 * (Math.Min(this._currentConsummateLevel, 18) - 2)) / 742f + 0.023f;
			base.StartCoroutine(this.ChangeFillAmountOverTime(this._yellowProgress, time, Math.Min(Math.Max(processPercent, 0f), 1f), action));
		}

		// Token: 0x06004921 RID: 18721 RVA: 0x00222E4C File Offset: 0x0022104C
		private void SetConsummateProgressBar(bool isPreviewProcess = false)
		{
			CImage processMask = isPreviewProcess ? this._cacheProgress : this._blueProgress;
			int showLevel = (isPreviewProcess ? this._previewConsummateLevel : this._currentConsummateLevel) - 2;
			float processPercent = (float)(45 * showLevel) / 742f + 0.023f;
			processMask.fillAmount = Math.Min(Math.Max(processPercent, 0f), 1f);
			bool flag = !isPreviewProcess;
			if (flag)
			{
				this._progressBar.SetActive(this._currentConsummateLevel > 2);
				this._progressBar.transform.localScale = new Vector3((float)showLevel * 90f / 1443f, 1f, 1f);
				this._cacheProgress.fillAmount = 0f;
			}
		}

		// Token: 0x06004922 RID: 18722 RVA: 0x00222F0C File Offset: 0x0022110C
		private IEnumerator ChangeFillAmountOverTime(CImage cImage, float duration, float targetPercent, Action action = null)
		{
			float startAmount = cImage.fillAmount;
			float timeElapsed = 0f;
			while (timeElapsed < duration)
			{
				timeElapsed += Time.deltaTime;
				float normalizedTime = timeElapsed / duration;
				cImage.fillAmount = Mathf.Lerp(startAmount, targetPercent, normalizedTime);
				yield return null;
			}
			cImage.fillAmount = targetPercent;
			if (action != null)
			{
				action();
			}
			yield break;
		}

		// Token: 0x06004923 RID: 18723 RVA: 0x00222F38 File Offset: 0x00221138
		public override void OnItemChanged(ItemKey itemKey, int amount, bool queueAnim = false, bool isAllSelected = false, bool playItemAnim = true)
		{
			sbyte grade = Config.Material.Instance[itemKey.TemplateId].Grade;
			this._previewProcessValue += GearMateSubPageBase.CalcGradeProcessValue(grade) * amount;
			int processPercent;
			int increaseCount = this.GetIncreaseCountAndProcessValue(this._previewProcessValue, out processPercent);
			this._previewProgressPercent = processPercent;
			this._previewConsummateLevel = increaseCount + this._currentConsummateLevel;
			this._previewConsummateLevelIncreaseCount = increaseCount;
			this.SetConsummateProgressBar(true);
			this.SetProgressTextValueText(true);
			this.SetProgressPercentText(true);
			bool flag = amount > 0 && playItemAnim;
			if (flag)
			{
				GameObject itemPrefab = Object.Instantiate<GameObject>(this._itemPrefab, this._itemPrefab.transform.parent);
				ItemDrop itemDrop = itemPrefab.GetComponent<ItemDrop>();
				itemDrop.eff_gearmate_zhujian_tubiaozha = this._eff_gearmate_zhujian_tubiaozha;
				itemDrop.OnTrigger += delegate()
				{
					this._eff_gearmate_zhujian_huoxing.GetComponent<ParticleSystem>().Play();
					bool flag2 = !queueAnim;
					if (flag2)
					{
						this.SetMachineWaterHeight();
					}
				};
				itemPrefab.transform.position = Vector3.Lerp(this._itemPrefabLeftPoint.transform.position, this._itemPrefabRightPoint.transform.position, Random.Range(0f, 1f));
				itemPrefab.GetComponent<CImage>().SetSprite(Config.Material.Instance[itemKey.TemplateId].Icon, false, null);
				bool queueAnim2 = queueAnim;
				if (queueAnim2)
				{
					base.ItemDrop(itemPrefab);
				}
				else
				{
					itemPrefab.SetActive(true);
				}
			}
			else
			{
				this.SetMachineWaterHeight();
			}
			this.SetButtonState(this._previewProcessValue != this._currentProcessValue);
		}

		// Token: 0x06004924 RID: 18724 RVA: 0x002230D0 File Offset: 0x002212D0
		protected override void SetMachineWaterHeight()
		{
			GearMateConsummateUpdatePage.<>c__DisplayClass28_0 CS$<>8__locals1 = new GearMateConsummateUpdatePage.<>c__DisplayClass28_0();
			CS$<>8__locals1.<>4__this = this;
			int value = this._previewProcessValue - this._currentProcessValue;
			CS$<>8__locals1.height = (float)value / (float)(value + GearMateSubPageBase.CalcGradeProcessValue(6));
			bool flag = (double)CS$<>8__locals1.height < 0.01;
			if (flag)
			{
				CS$<>8__locals1.height = 0.01f;
			}
			CS$<>8__locals1.<SetMachineWaterHeight>g__Action|0();
			bool flag2 = this.HeightCoroutine != null;
			if (flag2)
			{
				base.StopCoroutine(this.HeightCoroutine);
			}
			float time = 1.5f;
			bool flag3 = CS$<>8__locals1.height < 0.9f;
			if (flag3)
			{
				time = ((base.TotalDropItemCount > 0) ? ((float)base.TotalDropItemCount * 0.3f) : 1.5f);
			}
			this.HeightCoroutine = base.StartCoroutine(base.ScaleCoroutine(this._handle.transform, time, new Vector3(Math.Min(CS$<>8__locals1.height, 1f), 1f, 1f), new Action(CS$<>8__locals1.<SetMachineWaterHeight>g__Action|0)));
		}

		// Token: 0x06004925 RID: 18725 RVA: 0x002231D4 File Offset: 0x002213D4
		private int GetIncreaseCountAndProcessValue(int processValue, out int resultProcessPercent)
		{
			int increaseCount = 0;
			int resultProcessValue = processValue;
			while (resultProcessValue >= this.GetUpgradeRequirement(increaseCount))
			{
				resultProcessValue -= this.GetUpgradeRequirement(increaseCount);
				increaseCount++;
			}
			resultProcessPercent = resultProcessValue * 100 / this.GetUpgradeRequirement(increaseCount);
			return increaseCount;
		}

		// Token: 0x06004926 RID: 18726 RVA: 0x0022321C File Offset: 0x0022141C
		private int GetUpgradeRequirement(int increaseCount = 0)
		{
			int consummateLevel = this._currentConsummateLevel + increaseCount;
			if (!true)
			{
			}
			int result;
			switch (consummateLevel)
			{
			case 0:
				result = 30;
				break;
			case 1:
				result = 60;
				break;
			case 2:
				result = 120;
				break;
			default:
				result = 240 * (consummateLevel - 2);
				break;
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x06004927 RID: 18727 RVA: 0x00223274 File Offset: 0x00221474
		public override void ResetProcessValue(bool isChangeSource = false)
		{
			this._currentProcessValue = base.GearMate.ConsummateLevelProgress;
			bool flag = !isChangeSource;
			if (flag)
			{
				this.ResetConsummateMonitor(base.GearMate.Id);
			}
			else
			{
				this.OnGotCurrentConsummateLevel();
			}
		}

		// Token: 0x06004928 RID: 18728 RVA: 0x002232B8 File Offset: 0x002214B8
		public override void Confirm(ItemKeyAndCount itemKeyAndCount, ItemSourceType itemSourceType)
		{
			this._playingUpgradeAnim = true;
			ExtraDomainMethod.Call.UpgradeGearMate(base.GearMateDisplayData.CharacterId, 6, itemKeyAndCount.ItemKey, itemKeyAndCount.Count, itemSourceType);
		}

		// Token: 0x06004929 RID: 18729 RVA: 0x002232E4 File Offset: 0x002214E4
		public override void PointEnterConfirmButton()
		{
			bool flag = this.ConfirmButtonInteractable();
			if (flag)
			{
				base.PointEnterConfirmButton();
			}
		}

		// Token: 0x0600492A RID: 18730 RVA: 0x00223304 File Offset: 0x00221504
		public override void PlayUpgradeAnim(Action action)
		{
			this._gearMateMachine_0.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "move", false);
			AudioManager.Instance.PlaySound("SFX_GearMate_machine_loop", false, false);
			int randomValue = Random.Range(0, 3);
			base.Parent.Avatar.ShowBubble(LocalStringManager.Get(LanguageKey.LK_GearMateConsummate_SpeakWord0 + randomValue), 1.5f);
			base.Parent.Avatar.DoGearMateAnimation("break_1");
			bool flag = this.HeightCoroutine != null;
			if (flag)
			{
				base.StopCoroutine(this.HeightCoroutine);
			}
			this.HeightCoroutine = base.StartCoroutine(base.ScaleCoroutine(this._handle.transform, 1.13f, new Vector3(0f, 1f, 1f), null));
			float x = this._handle.transform.localScale.x;
			if (!true)
			{
			}
			int num;
			if (x < 0.67f)
			{
				if (x < 0.33f)
				{
					num = 0;
				}
				else
				{
					num = 1;
				}
			}
			else
			{
				num = 2;
			}
			if (!true)
			{
			}
			int jinshuiIndex = num;
			ParticleSystem jinshuiParticle = this._jinshuiGroup.transform.GetChild(jinshuiIndex).GetComponent<ParticleSystem>();
			jinshuiParticle.Play();
			this._cacheProgress.fillAmount = 0f;
			action = (Action)Delegate.Combine(action, new Action(delegate()
			{
				this._yellowProgress.transform.GetComponent<DOTweenAnimation>().DORestart();
			}));
			int count = this._previewConsummateLevel - this._currentConsummateLevel;
			float time = 1.16f / (float)count;
			SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(0.34f, delegate
			{
				this._blueProgress.fillAmount = 0f;
				this.SetConsummateProgressYellow(null, 0f);
				base.<PlayUpgradeAnim>g__UpdateAnim|2(count);
			});
		}

		// Token: 0x0600492B RID: 18731 RVA: 0x002234C4 File Offset: 0x002216C4
		private int GetFeatureUpgradeLevel(bool isPreview = false)
		{
			int increaseCount = 0;
			int processValue = isPreview ? this._previewProcessValue : this._currentProcessValue;
			while (processValue >= this.GetUpgradeRequirement(increaseCount))
			{
				processValue -= this.GetUpgradeRequirement(increaseCount);
				increaseCount++;
			}
			return increaseCount;
		}

		// Token: 0x0600492C RID: 18732 RVA: 0x00223510 File Offset: 0x00221710
		private void ShowTip()
		{
			TooltipInvoker tip = this._buttonConfirm.transform.GetComponent<TooltipInvoker>();
			tip.enabled = true;
			TooltipInvoker tooltipInvoker = tip;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = new ArgumentBox();
			}
			string gearMateName = NameCenter.GetMonasticTitleOrDisplayName(base.Parent.GearMateDisplayData, false);
			tip.RuntimeParam.Set("arg0", LocalStringManager.Get(LanguageKey.LK_GearMateConsummate_Title));
			tip.RuntimeParam.Set("arg1", LocalStringManager.GetFormat(LanguageKey.UI_MouseTipGearMateUpgradeConsummate_Desc, gearMateName).ColorReplace());
			for (int i = 0; i < 9; i++)
			{
				GameObject levelTip = this._levels.CGet<GameObject>(string.Format("Level_{0}", i));
				levelTip.GetComponent<Refers>().CGet<GameObject>("On").SetActive(2 * i + 2 <= this._previewConsummateLevel);
			}
		}

		// Token: 0x0600492D RID: 18733 RVA: 0x002235FC File Offset: 0x002217FC
		public override void SetButtonState(bool state)
		{
			this._buttonConfirm.interactable = (state && this._currentConsummateLevel < 18);
		}

		// Token: 0x0600492E RID: 18734 RVA: 0x0022361C File Offset: 0x0022181C
		public override bool CheckItemInteractable(ItemDisplayData itemDisplayData, out int canSelectCount)
		{
			canSelectCount = 0;
			int previewValue = this._previewConsummateLevel;
			bool flag = previewValue >= 18;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				sbyte grade = Config.Material.Instance[itemDisplayData.Key.TemplateId].Grade;
				int addProcessValue = GearMateSubPageBase.CalcGradeProcessValue(grade);
				int previewProcessValue = this._previewProcessValue;
				for (int i = 0; i < itemDisplayData.Amount; i++)
				{
					int num;
					int increaseCount = this.GetIncreaseCountAndProcessValue(previewProcessValue, out num);
					int curValue = this._currentConsummateLevel + increaseCount;
					bool flag2 = curValue < 18;
					if (flag2)
					{
						previewProcessValue += addProcessValue;
						canSelectCount++;
					}
				}
				result = true;
			}
			return result;
		}

		// Token: 0x0600492F RID: 18735 RVA: 0x002236C4 File Offset: 0x002218C4
		public override void RefreshItemTipNotInteractable(ItemView itemView)
		{
			TooltipInvoker tip = itemView.GetMouseTip();
			tip.enabled = true;
			tip.Type = TipType.SingleDesc;
			string charName = base.GetGearMateName();
			string typeName = LocalStringManager.Get(LanguageKey.LK_Consummate_Level);
			string content = LocalStringManager.GetFormat(LanguageKey.LK_GearMate_UpgradeMaxTip, charName, typeName).SetColor("brightred");
			string[] presetParam = tip.PresetParam;
			bool flag = presetParam == null || presetParam.Length <= 0;
			if (flag)
			{
				tip.PresetParam = new string[1];
			}
			tip.PresetParam[0] = content;
			tip.RuntimeParam = null;
		}

		// Token: 0x06004930 RID: 18736 RVA: 0x00223750 File Offset: 0x00221950
		private void InitLevelTips()
		{
			for (int i = 2; i < 19; i++)
			{
				bool flag = i % 2 == 0;
				if (flag)
				{
					GameObject levelTip = this._levels.CGet<GameObject>(string.Format("Level_{0}", i / 2 - 1));
					TooltipInvoker tip = levelTip.GetComponent<TooltipInvoker>();
					ConsummateLevelItem config = ConsummateLevel.Instance[i];
					tip.PresetParam[0] = config.Name.ColorReplace();
					tip.PresetParam[1] = string.Concat(new string[]
					{
						LocalStringManager.GetFormat(LanguageKey.LK_Consummate_Level_Tips, i).ColorReplace(),
						"\n",
						LocalStringManager.GetFormat(LanguageKey.LK_Consummate_Neili_Tips, 20 * i).ColorReplace(),
						"\n\n",
						config.Desc.ColorReplace()
					});
				}
				else
				{
					TooltipInvoker tip2 = this._levels.CGet<TooltipInvoker>(string.Format("LevelTip_{0}", i / 2 - 1));
					tip2.PresetParam[0] = LocalStringManager.GetFormat(LanguageKey.LK_Consummate_Level_Tips, i).ColorReplace() + "\n" + LocalStringManager.GetFormat(LanguageKey.LK_Consummate_Neili_Tips, 20 * i).ColorReplace();
				}
			}
		}

		// Token: 0x06004931 RID: 18737 RVA: 0x0022389C File Offset: 0x00221A9C
		public override bool IsMaxLevel()
		{
			return this._previewConsummateLevel >= 18;
		}

		// Token: 0x06004932 RID: 18738 RVA: 0x002238BC File Offset: 0x00221ABC
		private void InitRefers()
		{
			this._progressPercentText = base.CGet<TextMeshProUGUI>("ProgressPercentText");
			this._progressTextValue = base.CGet<TextMeshProUGUI>("ProgressTextValue");
			this._yellowProgress = base.CGet<CImage>("YellowProgress");
			this._blueProgress = base.CGet<CImage>("BlueProgress");
			this._eff_gearmate_zhujian_fangkuang1 = base.CGet<GameObject>("eff_gearmate_zhujian_fangkuang1");
			this._gearMateMachine_0 = base.CGet<GameObject>("GearMateMachine_0");
			this._progressBar = base.CGet<GameObject>("ProgressBar");
			this._handle = base.CGet<GameObject>("handle");
			this._eff_gearmate_zhujian_huoxing = base.CGet<GameObject>("eff_gearmate_zhujian_huoxing");
			this._buttonConfirm = base.CGet<CButtonObsolete>("ButtonConfirm");
			this._eff_gearmate_zhujian_tubiaozha = base.CGet<GameObject>("eff_gearmate_zhujian_tubiaozha");
			this._itemPrefab = base.CGet<GameObject>("ItemPrefab");
			this._jinshuiGroup = base.CGet<GameObject>("jinshuiGroup");
			this._itemPrefabLeftPoint = base.CGet<GameObject>("ItemPrefabLeftPoint");
			this._itemPrefabRightPoint = base.CGet<GameObject>("ItemPrefabRightPoint");
			this._levels = base.CGet<Refers>("Levels");
			this._cacheProgress = base.CGet<CImage>("CacheProgress");
		}

		// Token: 0x040032C2 RID: 12994
		private int _currentProcessValue;

		// Token: 0x040032C3 RID: 12995
		private int _currentProgressPercent;

		// Token: 0x040032C4 RID: 12996
		private int _currentConsummateLevel;

		// Token: 0x040032C5 RID: 12997
		private int _currentConsummateLevelIncreaseCount;

		// Token: 0x040032C6 RID: 12998
		private int _previewProcessValue;

		// Token: 0x040032C7 RID: 12999
		private int _previewProgressPercent;

		// Token: 0x040032C8 RID: 13000
		private int _previewConsummateLevel;

		// Token: 0x040032C9 RID: 13001
		private int _previewConsummateLevelIncreaseCount;

		// Token: 0x040032CA RID: 13002
		private ConsummateLevelMonitor _consummateMonitor;

		// Token: 0x040032CB RID: 13003
		private bool _playingUpgradeAnim;

		// Token: 0x040032CC RID: 13004
		private int _pageIndex;

		// Token: 0x040032CD RID: 13005
		private TextMeshProUGUI _progressPercentText;

		// Token: 0x040032CE RID: 13006
		private TextMeshProUGUI _progressTextValue;

		// Token: 0x040032CF RID: 13007
		private CImage _yellowProgress;

		// Token: 0x040032D0 RID: 13008
		private CImage _blueProgress;

		// Token: 0x040032D1 RID: 13009
		private GameObject _eff_gearmate_zhujian_fangkuang1;

		// Token: 0x040032D2 RID: 13010
		private GameObject _gearMateMachine_0;

		// Token: 0x040032D3 RID: 13011
		private GameObject _progressBar;

		// Token: 0x040032D4 RID: 13012
		private GameObject _handle;

		// Token: 0x040032D5 RID: 13013
		private GameObject _eff_gearmate_zhujian_huoxing;

		// Token: 0x040032D6 RID: 13014
		private CButtonObsolete _buttonConfirm;

		// Token: 0x040032D7 RID: 13015
		private GameObject _eff_gearmate_zhujian_tubiaozha;

		// Token: 0x040032D8 RID: 13016
		private GameObject _itemPrefab;

		// Token: 0x040032D9 RID: 13017
		private GameObject _jinshuiGroup;

		// Token: 0x040032DA RID: 13018
		private GameObject _itemPrefabLeftPoint;

		// Token: 0x040032DB RID: 13019
		private GameObject _itemPrefabRightPoint;

		// Token: 0x040032DC RID: 13020
		private Refers _levels;

		// Token: 0x040032DD RID: 13021
		private CImage _cacheProgress;
	}
}
