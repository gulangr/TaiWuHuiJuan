using System;
using System.Collections.Generic;
using CharacterDataMonitor;
using Config;
using FrameWork;
using GameData.Domains.Extra;
using GameData.Domains.Item;
using GameData.Domains.Taiwu;
using Spine.Unity;
using TMPro;
using UnityEngine;

namespace GearMate
{
	// Token: 0x02000617 RID: 1559
	public class GearMateFeatureUpdatePage : GearMateSubPageBase
	{
		// Token: 0x06004934 RID: 18740 RVA: 0x002239F4 File Offset: 0x00221BF4
		protected override void InitInternal()
		{
			this.InitRefers();
		}

		// Token: 0x06004935 RID: 18741 RVA: 0x00223A00 File Offset: 0x00221C00
		public override void OnEnableBySwitchPage(int pageIndex)
		{
			bool flag = base.GearMate == null;
			if (!flag)
			{
				Dictionary<ItemSourceType, Dictionary<ItemKey, int>> dict = base.Parent.ItemSelector.GetTypeToSelectedItemDict(pageIndex);
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
		}

		// Token: 0x06004936 RID: 18742 RVA: 0x00223AE0 File Offset: 0x00221CE0
		public override void OnGearMateDataChanged()
		{
			this._currentProcessValue = base.GearMate.FeatureProgress;
			this._currentFeatureLevelCount = 0;
			this._featureMonitor = SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<FeatureMonitor>(base.GearMate.Id, false);
			foreach (short featureId in this._featureMonitor.FeatureIds)
			{
				CharacterFeatureItem config = CharacterFeature.Instance[featureId];
				ECharacterFeatureType type = config.Type;
				bool flag = type == ECharacterFeatureType.Good || type == ECharacterFeatureType.Bad;
				if (flag)
				{
					this._currentFeatureLevelCount += (int)config.Level;
				}
			}
			int increaseCount;
			int processPercent;
			this.GetIncreaseCountAndProcessValue(out increaseCount, out processPercent, false);
			this._currentFeatureLevelIncreaseCount = increaseCount;
			this._currentProcessPercent = processPercent;
			this._previewProcessValue = this._currentProcessValue;
			this._previewFeatureLevelCount = this._currentFeatureLevelCount;
			this._previewProcessPercent = this._currentProcessPercent;
			this._previewFeatureLevelIncreaseCount = this._currentFeatureLevelIncreaseCount;
			this.SetFeatureProgressBar();
			foreach (TextMeshProUGUI textValue in this._processTextValueList)
			{
				textValue.text = this._currentProcessPercent.ToString() + "%";
			}
			this._processPercentText.text = this._currentProcessPercent.ToString() + "%";
			this._levelTextValue.text = this._currentFeatureLevelIncreaseCount.ToString();
			this.SetButtonState(false);
			this.SetMachineWaterHeight();
			this.ShowTip();
		}

		// Token: 0x06004937 RID: 18743 RVA: 0x00223CAC File Offset: 0x00221EAC
		private void SetFeatureProgressBar()
		{
			this._cache.fillAmount = (float)this._previewProcessPercent / 100f;
			bool flag = this._previewFeatureLevelIncreaseCount > this._currentFeatureLevelIncreaseCount;
			if (flag)
			{
				this._processMaskBlue.transform.localScale = new Vector3(0.001f, 1f, 1f);
			}
			else
			{
				this._processMaskBlue.transform.localScale = new Vector3((this._currentProcessPercent == 0) ? 0.001f : ((float)this._currentProcessPercent / 100f), 1f, 1f);
			}
		}

		// Token: 0x06004938 RID: 18744 RVA: 0x00223D48 File Offset: 0x00221F48
		public override void OnItemChanged(ItemKey itemKey, int amount, bool queueAnim = false, bool isAllSelected = false, bool playItemAnim = true)
		{
			sbyte grade = ItemTemplateHelper.GetGrade(itemKey.ItemType, itemKey.TemplateId);
			this._previewProcessValue += GearMateSubPageBase.CalcGradeProcessValue(grade) * amount;
			int increaseCount;
			int processPercent;
			this.GetIncreaseCountAndProcessValue(out increaseCount, out processPercent, true);
			this._previewProcessPercent = processPercent;
			this._previewFeatureLevelCount = increaseCount + this._currentFeatureLevelCount;
			this._previewFeatureLevelIncreaseCount = increaseCount;
			this.SetFeatureProgressBar();
			foreach (TextMeshProUGUI textValue in this._processTextValueList)
			{
				textValue.text = this._previewProcessPercent.ToString() + "%";
			}
			this._processPercentText.text = this._previewProcessPercent.ToString() + "%";
			bool flag = this._previewFeatureLevelIncreaseCount - this._currentFeatureLevelIncreaseCount > 0;
			if (flag)
			{
				TextMeshProUGUI processPercentText = this._processPercentText;
				processPercentText.text = processPercentText.text + " +" + (this._previewFeatureLevelIncreaseCount - this._currentFeatureLevelIncreaseCount).ToString();
			}
			this.SetButtonState(this._previewProcessValue != this._currentProcessValue);
			bool flag2 = amount > 0 && playItemAnim;
			if (flag2)
			{
				GameObject itemPrefab = Object.Instantiate<GameObject>(this._itemPrefab, this._itemPrefab.transform.parent);
				ItemDrop itemDrop = itemPrefab.GetComponent<ItemDrop>();
				itemDrop.eff_gearmate_zhujian_tubiaozha = this._eff_gearmate_zhujian_tubiaozha;
				itemDrop.OnTrigger += delegate()
				{
					this._eff_gearmate_zhujian_huoxing.GetComponent<ParticleSystem>().Play();
					bool flag3 = !queueAnim;
					if (flag3)
					{
						this.SetMachineWaterHeight();
					}
				};
				itemPrefab.transform.position = Vector3.Lerp(this._itemPrefabLeftPoint.transform.position, this._itemPrefabRightPoint.transform.position, Random.Range(0f, 1f));
				itemPrefab.GetComponent<CImage>().SetSprite(ItemTemplateHelper.GetIcon(itemKey.ItemType, itemKey.TemplateId), false, null);
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
		}

		// Token: 0x06004939 RID: 18745 RVA: 0x00223F8C File Offset: 0x0022218C
		public override void ResetProcessValue(bool isAllSelected = false)
		{
			this._currentProcessValue = base.GearMate.FeatureProgress;
			this._currentFeatureLevelCount = 0;
			this._featureMonitor = SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<FeatureMonitor>(base.GearMate.Id, false);
			foreach (short featureId in this._featureMonitor.FeatureIds)
			{
				CharacterFeatureItem config = CharacterFeature.Instance[featureId];
				ECharacterFeatureType type = config.Type;
				bool flag = type == ECharacterFeatureType.Good || type == ECharacterFeatureType.Bad;
				if (flag)
				{
					this._currentFeatureLevelCount += (int)config.Level;
				}
			}
			int increaseCount;
			int processPercent;
			this.GetIncreaseCountAndProcessValue(out increaseCount, out processPercent, false);
			this._currentFeatureLevelIncreaseCount = increaseCount;
			this._currentProcessPercent = processPercent;
			this._previewProcessValue = this._currentProcessValue;
			this._previewFeatureLevelCount = this._currentFeatureLevelCount;
			this._previewProcessPercent = this._currentProcessPercent;
			this._previewFeatureLevelIncreaseCount = this._currentFeatureLevelIncreaseCount;
			this.SetFeatureProgressBar();
			foreach (TextMeshProUGUI textValue in this._processTextValueList)
			{
				textValue.text = this._currentProcessPercent.ToString() + "%";
			}
			this._processPercentText.text = this._currentProcessPercent.ToString() + "%";
			this._levelTextValue.text = this._currentFeatureLevelIncreaseCount.ToString();
			this.SetButtonState(false);
			this.SetMachineWaterHeight();
			this.ShowTip();
		}

		// Token: 0x0600493A RID: 18746 RVA: 0x00224158 File Offset: 0x00222358
		private void GetIncreaseCountAndProcessValue(out int increaseCount, out int processPercent, bool isPreview = false)
		{
			increaseCount = 0;
			int processValue = isPreview ? this._previewProcessValue : this._currentProcessValue;
			while (processValue >= this.GetUpgradeReqirement(increaseCount))
			{
				processValue -= this.GetUpgradeReqirement(increaseCount);
				increaseCount++;
			}
			processPercent = processValue * 100 / this.GetUpgradeReqirement(increaseCount);
		}

		// Token: 0x0600493B RID: 18747 RVA: 0x002241B4 File Offset: 0x002223B4
		protected override void SetMachineWaterHeight()
		{
			GearMateFeatureUpdatePage.<>c__DisplayClass17_0 CS$<>8__locals1 = new GearMateFeatureUpdatePage.<>c__DisplayClass17_0();
			CS$<>8__locals1.<>4__this = this;
			int value = this._previewProcessValue - this._currentProcessValue;
			CS$<>8__locals1.height = (float)value / (float)(value + GearMateSubPageBase.CalcGradeProcessValue(8));
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

		// Token: 0x0600493C RID: 18748 RVA: 0x002242B8 File Offset: 0x002224B8
		private int GetUpgradeReqirement(int increaseCount = 0)
		{
			return (this._currentFeatureLevelCount + 1 + increaseCount) * 10;
		}

		// Token: 0x0600493D RID: 18749 RVA: 0x002242D7 File Offset: 0x002224D7
		public override void Confirm(ItemKeyAndCount itemKeyAndCount, ItemSourceType itemSourceType)
		{
			ExtraDomainMethod.Call.UpgradeGearMate(base.GearMateDisplayData.CharacterId, 7, itemKeyAndCount.ItemKey, itemKeyAndCount.Count, itemSourceType);
		}

		// Token: 0x0600493E RID: 18750 RVA: 0x002242FC File Offset: 0x002224FC
		public override void PointEnterConfirmButton()
		{
			bool flag = this.ConfirmButtonInteractable();
			if (flag)
			{
				base.PointEnterConfirmButton();
			}
		}

		// Token: 0x0600493F RID: 18751 RVA: 0x0022431C File Offset: 0x0022251C
		public override void PlayUpgradeAnim(Action action)
		{
			this._gearMateMachine_0.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "move", false);
			AudioManager.Instance.PlaySound("SFX_GearMate_machine_loop", false, false);
			Action <>9__4;
			action = (Action)Delegate.Combine(action, new Action(delegate()
			{
				GearMateSubPageBase <>4__this = this;
				GearMateSubPageBase <>4__this2 = this;
				GameObject eff_gearmate_zhujian_jindutiaohuang = this._eff_gearmate_zhujian_jindutiaohuang;
				Action action2;
				if ((action2 = <>9__4) == null)
				{
					action2 = (<>9__4 = delegate()
					{
						this._processMaskYellow.transform.localScale = new Vector3(0.001f, 1f, 1f);
					});
				}
				<>4__this.StartCoroutine(<>4__this2.SetProcessYellowTransparent(eff_gearmate_zhujian_jindutiaohuang, action2, 0.5f));
			}));
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
			int randomValue = Random.Range(0, 3);
			base.Parent.Avatar.ShowBubble(LocalStringManager.Get(LanguageKey.LK_GearMateFeature_SpeakWord0 + randomValue), 1.5f);
			base.Parent.Avatar.DoGearMateAnimation("break_1");
			this._processPercentText.text = this._previewProcessPercent.ToString() + "%";
			bool flag2 = this._previewFeatureLevelIncreaseCount - this._currentFeatureLevelIncreaseCount > 0;
			if (flag2)
			{
				TextMeshProUGUI processPercentText = this._processPercentText;
				processPercentText.text = processPercentText.text + " +" + (this._previewFeatureLevelIncreaseCount - this._currentFeatureLevelIncreaseCount).ToString();
			}
			this._levelTextValue.text = this._currentFeatureLevelIncreaseCount.ToString();
			this._cache.fillAmount = 0f;
			this._processMaskBlue.transform.localScale = new Vector3((this._currentProcessPercent == 0) ? 0.001f : ((float)this._currentProcessPercent / 100f), 1f, 1f);
			int count = this._previewFeatureLevelIncreaseCount - this._currentFeatureLevelIncreaseCount;
			SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(0.34f, delegate
			{
				this._processMaskBlue.transform.localScale = new Vector3(0.001f, 1f, 1f);
				this._processMaskYellow.transform.localScale = new Vector3((this._currentProcessPercent == 0) ? 0.001f : ((float)this._currentProcessPercent / 100f), 1f, 1f);
				this.StartCoroutine(this.ContinuousScaleCoroutine(this._processMaskYellow.transform, count, 1.16f / (float)(count + 1), new Vector3((float)this._previewProcessPercent / 100f, 1f, 1f), 0, new Action<int, int>(base.<PlayUpgradeAnim>g__PlaySingleUpdateAnim|2), action));
			});
		}

		// Token: 0x06004940 RID: 18752 RVA: 0x00224583 File Offset: 0x00222783
		public override void SetButtonState(bool state)
		{
			this._buttonConfirm.interactable = (state && this._currentFeatureLevelCount < 84);
		}

		// Token: 0x06004941 RID: 18753 RVA: 0x002245A4 File Offset: 0x002227A4
		private void ShowTip()
		{
			TooltipInvoker tipDisplayer = this._buttonConfirm.transform.GetComponent<TooltipInvoker>();
			tipDisplayer.enabled = true;
			TooltipInvoker tooltipInvoker = tipDisplayer;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = new ArgumentBox();
			}
			string gearMateName = NameCenter.GetMonasticTitleOrDisplayName(base.Parent.GearMateDisplayData, false);
			for (int i = 0; i < 3; i++)
			{
				tipDisplayer.RuntimeParam.Set(string.Format("Desc{0}", i), LocalStringManager.GetFormat(LanguageKey.UI_MouseTipGearMateUpgradeFeature_Desc0 + i, gearMateName).ColorReplace());
			}
			tipDisplayer.RuntimeParam.Set("Value", LocalStringManager.GetFormat(LanguageKey.UI_MouseTipGearMateUpgradeFeature_Notice, this._currentFeatureLevelIncreaseCount).ColorReplace());
			this._featureNumber.enabled = true;
			tooltipInvoker = this._featureNumber;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = new ArgumentBox();
			}
			this._featureNumber.RuntimeParam.Set("arg0", LocalStringManager.Get(LanguageKey.UI_MouseTipGearMateUpgradeFeature_LevelTip_Title).ColorReplace());
			this._featureNumber.RuntimeParam.Set("arg1", LocalStringManager.GetFormat(LanguageKey.UI_MouseTipGearMateUpgradeFeature_LevelTip_Desc, gearMateName, this._currentFeatureLevelIncreaseCount).ColorReplace());
			base.CGet<GameObject>("Notice").SetActive(this._currentFeatureLevelCount >= 84);
		}

		// Token: 0x06004942 RID: 18754 RVA: 0x00224704 File Offset: 0x00222904
		private void InitRefers()
		{
			this._processTextValueList = base.CGetList<TextMeshProUGUI>("ProcessTextValue_");
			this._processMaskYellow = base.CGet<GameObject>("ProcessMaskYellow");
			this._processMaskBlue = base.CGet<GameObject>("ProcessMaskBlue");
			this._eff_gearmate_zhujian_changkuang = base.CGet<GameObject>("eff_gearmate_zhujian_changkuang");
			this._eff_gearmate_zhujian_fangkuang = base.CGet<GameObject>("eff_gearmate_zhujian_fangkuang");
			this._levelTextValue = base.CGet<TextMeshProUGUI>("LevelTextValue");
			this._processPercentText = base.CGet<TextMeshProUGUI>("ProcessPercentText");
			this._gearMateMachine_0 = base.CGet<GameObject>("GearMateMachine_0");
			this._handle = base.CGet<GameObject>("handle");
			this._eff_gearmate_zhujian_huoxing = base.CGet<GameObject>("eff_gearmate_zhujian_huoxing");
			this._buttonConfirm = base.CGet<CButtonObsolete>("ButtonConfirm");
			this._itemPrefab = base.CGet<GameObject>("ItemPrefab");
			this._eff_gearmate_zhujian_tubiaozha = base.CGet<GameObject>("eff_gearmate_zhujian_tubiaozha");
			this._itemPrefabLeftPoint = base.CGet<GameObject>("ItemPrefabLeftPoint");
			this._itemPrefabRightPoint = base.CGet<GameObject>("ItemPrefabRightPoint");
			this._jinshuiGroup = base.CGet<GameObject>("jinshuiGroup");
			this._featureNumber = base.CGet<TooltipInvoker>("FeatureNumber");
			this._cache = base.CGet<CImage>("Cache");
			this._eff_gearmate_zhujian_jindutiaolan = base.CGet<GameObject>("eff_gearmate_zhujian_jindutiaolan");
			this._eff_gearmate_zhujian_jindutiaohuang = base.CGet<GameObject>("eff_gearmate_zhujian_jindutiaohuang");
		}

		// Token: 0x040032DE RID: 13022
		private int _currentProcessValue;

		// Token: 0x040032DF RID: 13023
		private int _currentProcessPercent;

		// Token: 0x040032E0 RID: 13024
		private int _currentFeatureLevelCount;

		// Token: 0x040032E1 RID: 13025
		private int _currentFeatureLevelIncreaseCount;

		// Token: 0x040032E2 RID: 13026
		private int _previewProcessValue;

		// Token: 0x040032E3 RID: 13027
		private int _previewProcessPercent;

		// Token: 0x040032E4 RID: 13028
		private int _previewFeatureLevelCount;

		// Token: 0x040032E5 RID: 13029
		private int _previewFeatureLevelIncreaseCount;

		// Token: 0x040032E6 RID: 13030
		private FeatureMonitor _featureMonitor;

		// Token: 0x040032E7 RID: 13031
		private const int MaxFeatureLevelCount = 84;

		// Token: 0x040032E8 RID: 13032
		private List<TextMeshProUGUI> _processTextValueList;

		// Token: 0x040032E9 RID: 13033
		private GameObject _processMaskYellow;

		// Token: 0x040032EA RID: 13034
		private GameObject _processMaskBlue;

		// Token: 0x040032EB RID: 13035
		private GameObject _eff_gearmate_zhujian_changkuang;

		// Token: 0x040032EC RID: 13036
		private GameObject _eff_gearmate_zhujian_fangkuang;

		// Token: 0x040032ED RID: 13037
		private TextMeshProUGUI _levelTextValue;

		// Token: 0x040032EE RID: 13038
		private TextMeshProUGUI _processPercentText;

		// Token: 0x040032EF RID: 13039
		private GameObject _gearMateMachine_0;

		// Token: 0x040032F0 RID: 13040
		private GameObject _handle;

		// Token: 0x040032F1 RID: 13041
		private GameObject _eff_gearmate_zhujian_huoxing;

		// Token: 0x040032F2 RID: 13042
		private CButtonObsolete _buttonConfirm;

		// Token: 0x040032F3 RID: 13043
		private GameObject _itemPrefab;

		// Token: 0x040032F4 RID: 13044
		private GameObject _eff_gearmate_zhujian_tubiaozha;

		// Token: 0x040032F5 RID: 13045
		private GameObject _itemPrefabLeftPoint;

		// Token: 0x040032F6 RID: 13046
		private GameObject _itemPrefabRightPoint;

		// Token: 0x040032F7 RID: 13047
		private GameObject _jinshuiGroup;

		// Token: 0x040032F8 RID: 13048
		private TooltipInvoker _featureNumber;

		// Token: 0x040032F9 RID: 13049
		private CImage _cache;

		// Token: 0x040032FA RID: 13050
		private GameObject _eff_gearmate_zhujian_jindutiaolan;

		// Token: 0x040032FB RID: 13051
		private GameObject _eff_gearmate_zhujian_jindutiaohuang;
	}
}
