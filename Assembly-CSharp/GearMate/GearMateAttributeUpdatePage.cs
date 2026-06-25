using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using GameData.Common;
using GameData.Domains.Character;
using GameData.Domains.Extra;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Taiwu;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using Spine.Unity;
using TMPro;
using UnityEngine;

namespace GearMate
{
	// Token: 0x02000610 RID: 1552
	public class GearMateAttributeUpdatePage : GearMateSubPageBase
	{
		// Token: 0x060048BB RID: 18619 RVA: 0x0022032F File Offset: 0x0021E52F
		protected override void InitInternal()
		{
			this.InitRefers();
		}

		// Token: 0x060048BC RID: 18620 RVA: 0x0022033C File Offset: 0x0021E53C
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

		// Token: 0x060048BD RID: 18621 RVA: 0x0022041C File Offset: 0x0021E61C
		protected override IList<UIBase.MonitorDataField> GetMonitorFields()
		{
			return new List<UIBase.MonitorDataField>
			{
				new UIBase.MonitorDataField(4, 0, (ulong)base.Parent.GearMateId, new uint[]
				{
					18U
				})
			};
		}

		// Token: 0x060048BE RID: 18622 RVA: 0x0022045C File Offset: 0x0021E65C
		protected override void HandleDataModification(Notification notification, NotificationWrapper wrapper)
		{
			DataUid uid = notification.Uid;
			RawDataPool pool = wrapper.DataPool;
			int offset = notification.ValueOffset;
			bool flag = uid.DomainId == 4 && uid.DataId == 0 && (int)uid.SubId0 == base.Parent.GearMateId;
			if (flag)
			{
				bool flag2 = uid.SubId1 == 18U;
				if (flag2)
				{
					MainAttributes attributes = default(MainAttributes);
					Serializer.Deserialize(pool, offset, ref attributes);
					for (sbyte i = 0; i < 6; i += 1)
					{
						this._currentAttributeValue[(int)i] = (int)attributes.Get(i);
					}
					this.OnGearMateDataChanged();
				}
			}
		}

		// Token: 0x060048BF RID: 18623 RVA: 0x00220500 File Offset: 0x0021E700
		public override void OnGearMateDataChanged()
		{
			for (int i = 0; i < base.GearMate.MainAttributeProgress.Length; i++)
			{
				this._currentProcessValue[i] = base.GearMate.MainAttributeProgress[i];
				this._previewProcessValue[i] = base.GearMate.MainAttributeProgress[i];
				this._previewAttributeValue[i] = this._currentAttributeValue[i];
				this.UpdateProcessPercent(i, false);
				this.UpdateProcessPercent(i, true);
				this.SetAttributeProgressBar(i);
				Refers refers = this._attributeBgList[i].GetComponent<Refers>();
				refers.CGet<TextMeshProUGUI>("CurValue").text = LocalStringManager.GetFormat(LanguageKey.LK_Combat_DamageValue_MarkCount, this._currentAttributeValue[i], LocalStringManager.Get(GearMateUpgradeType.GetMainAttributeUpgradeTypeName((sbyte)i)));
			}
			this.SetMachineWaterHeight();
			this._buttonConfirm.interactable = false;
			this.ShowTip();
		}

		// Token: 0x060048C0 RID: 18624 RVA: 0x002205E8 File Offset: 0x0021E7E8
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
			tipDisplayer.RuntimeParam.Set("Desc", LocalStringManager.GetFormat(LanguageKey.UI_MouseTipGearMateUpgradeAttribute_Desc, gearMateName).ColorReplace());
		}

		// Token: 0x060048C1 RID: 18625 RVA: 0x0022065C File Offset: 0x0021E85C
		private void UpdateProcessPercent(int index, bool isPreviewProcess = false)
		{
			bool flag = !isPreviewProcess;
			if (flag)
			{
				this._currentProcessPercent[index] = this._currentProcessValue[index] * 100 / this.GetUpgradeRequirement(index, false, 0);
				Refers refers = this._attributeBgList[index].GetComponent<Refers>();
				refers.CGet<TextMeshProUGUI>("AttributePercent").text = this._currentProcessPercent[index].ToString() + "%";
			}
			else
			{
				int processValue;
				int increaseCount = this.CalcIncreaseCount(index, this._previewProcessValue[index], out processValue);
				this._previewProcessPercent[index] = processValue * 100 / this.GetUpgradeRequirement(index, true, increaseCount);
				this._previewAttributeValue[index] = increaseCount + this._currentAttributeValue[index];
				Refers refers2 = this._attributeBgList[index].GetComponent<Refers>();
				refers2.CGet<TextMeshProUGUI>("NewValue").text = GearMateAttributeUpdatePage.ClampAttr(this._previewAttributeValue[index]).ToString();
				refers2.CGet<TextMeshProUGUI>("AttributePercent").text = this._previewProcessPercent[index].ToString() + "%";
			}
		}

		// Token: 0x060048C2 RID: 18626 RVA: 0x00220778 File Offset: 0x0021E978
		private int CalcIncreaseCount(int index, int processValue, out int resultProcessValue)
		{
			int increaseCount = 0;
			resultProcessValue = processValue;
			while (resultProcessValue >= this.GetUpgradeRequirement(index, true, increaseCount))
			{
				resultProcessValue -= this.GetUpgradeRequirement(index, true, increaseCount);
				increaseCount++;
			}
			return increaseCount;
		}

		// Token: 0x060048C3 RID: 18627 RVA: 0x002207BC File Offset: 0x0021E9BC
		private void SetAttributeProgressBar(int index)
		{
			Transform processMaskBlue = this._attributeBgList[index].GetComponent<Refers>().CGet<GameObject>("ProcessMaskBlue").transform;
			CImage cache = this._attributeBgList[index].GetComponent<Refers>().CGet<CImage>("Cache");
			cache.fillAmount = (float)this._previewProcessPercent[index] / 100f;
			bool flag = this._previewAttributeValue[index] <= this._currentAttributeValue[index];
			if (flag)
			{
				processMaskBlue.localScale = new Vector3((this._currentProcessPercent[index] == 0) ? 0.001f : ((float)this._currentProcessPercent[index] / 100f), 1f, 1f);
			}
			else
			{
				processMaskBlue.localScale = new Vector3(0.001f, 1f, 1f);
			}
		}

		// Token: 0x060048C4 RID: 18628 RVA: 0x0022088C File Offset: 0x0021EA8C
		public void SetAttributeProgressBarByAllSelected()
		{
			for (int i = 0; i < base.GearMate.MainAttributeProgress.Length; i++)
			{
				this.SetAttributeProgressBar(i);
			}
		}

		// Token: 0x060048C5 RID: 18629 RVA: 0x002208C0 File Offset: 0x0021EAC0
		public override void OnItemChanged(ItemKey itemKey, int amount, bool queueAnim = false, bool isAllSelected = false, bool playItemAnim = true)
		{
			sbyte grade = Config.Material.Instance[itemKey.TemplateId].Grade;
			sbyte resourceType = Config.Material.Instance[itemKey.TemplateId].ResourceType;
			int value = GearMateSubPageBase.CalcGradeProcessValue(grade) * amount;
			sbyte upgradeType = 0;
			switch (resourceType)
			{
			case 0:
				upgradeType = 3;
				this._previewProcessValue[(int)upgradeType] += value;
				break;
			case 1:
				upgradeType = 4;
				this._previewProcessValue[4] += value;
				break;
			case 2:
				upgradeType = 0;
				this._previewProcessValue[0] += value;
				break;
			case 3:
				upgradeType = 5;
				this._previewProcessValue[5] += value;
				break;
			case 4:
				upgradeType = 1;
				this._previewProcessValue[1] += value;
				break;
			case 5:
				upgradeType = 2;
				this._previewProcessValue[2] += value;
				break;
			}
			this.UpdateProcessPercent((int)upgradeType, true);
			bool flag = !isAllSelected;
			if (flag)
			{
				this.SetAttributeProgressBar((int)upgradeType);
			}
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
		}

		// Token: 0x060048C6 RID: 18630 RVA: 0x00220AD8 File Offset: 0x0021ECD8
		protected override void SetMachineWaterHeight()
		{
			GearMateAttributeUpdatePage.<>c__DisplayClass17_0 CS$<>8__locals1 = new GearMateAttributeUpdatePage.<>c__DisplayClass17_0();
			CS$<>8__locals1.<>4__this = this;
			int value = 0;
			for (int i = 0; i < base.GearMate.MainAttributeProgress.Length; i++)
			{
				value += this._previewProcessValue[i] - this._currentProcessValue[i];
			}
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

		// Token: 0x060048C7 RID: 18631 RVA: 0x00220C02 File Offset: 0x0021EE02
		public override void SetButtonState(bool state)
		{
			this._buttonConfirm.interactable = state;
		}

		// Token: 0x060048C8 RID: 18632 RVA: 0x00220C14 File Offset: 0x0021EE14
		public override void ResetProcessValue(bool isAllSelected = false)
		{
			for (int i = 0; i < base.GearMate.MainAttributeProgress.Length; i++)
			{
				this._previewProcessValue[i] = this._currentProcessValue[i];
				this._previewProcessPercent[i] = this._currentProcessPercent[i];
				this._previewAttributeValue[i] = this._currentAttributeValue[i];
				this.UpdateProcessPercent(i, false);
				this.UpdateProcessPercent(i, true);
				bool flag = !isAllSelected;
				if (flag)
				{
					this.SetAttributeProgressBar(i);
				}
			}
			this.SetButtonState(false);
			this.SetMachineWaterHeight();
		}

		// Token: 0x060048C9 RID: 18633 RVA: 0x00220CA4 File Offset: 0x0021EEA4
		private int GetUpgradeRequirement(int index, bool isPreview = false, int increaseCount = 0)
		{
			return (1 + this._currentAttributeValue[index] + (isPreview ? increaseCount : 0)) * 10;
		}

		// Token: 0x060048CA RID: 18634 RVA: 0x00220CCC File Offset: 0x0021EECC
		public override void Confirm(ItemKeyAndCount itemKeyAndCount, ItemSourceType itemSourceType)
		{
			sbyte resourceType = Config.Material.Instance[itemKeyAndCount.ItemKey.TemplateId].ResourceType;
			sbyte upgradeType = GearMateUpgradeType.GetMainAttributeUpgradeTypeByResourceType(resourceType);
			bool flag = upgradeType >= 0;
			if (flag)
			{
				ExtraDomainMethod.Call.UpgradeGearMate(base.GearMateDisplayData.CharacterId, upgradeType, itemKeyAndCount.ItemKey, itemKeyAndCount.Count, itemSourceType);
			}
		}

		// Token: 0x060048CB RID: 18635 RVA: 0x00220D28 File Offset: 0x0021EF28
		public override void PointEnterConfirmButton()
		{
			bool flag = this.ConfirmButtonInteractable();
			if (flag)
			{
				base.PointEnterConfirmButton();
			}
		}

		// Token: 0x060048CC RID: 18636 RVA: 0x00220D48 File Offset: 0x0021EF48
		public override void PlayUpgradeAnim(Action action)
		{
			GearMateAttributeUpdatePage.<>c__DisplayClass23_0 CS$<>8__locals1 = new GearMateAttributeUpdatePage.<>c__DisplayClass23_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.action = action;
			this._gearMateMachine_0.AnimationState.SetAnimation(0, "move", false);
			AudioManager.Instance.PlaySound("SFX_GearMate_machine_loop", false, false);
			bool flag = this.HeightCoroutine != null;
			if (flag)
			{
				base.StopCoroutine(this.HeightCoroutine);
			}
			this.HeightCoroutine = base.StartCoroutine(base.ScaleCoroutine(this._handle.transform, 1.5f, new Vector3(0f, 1f, 1f), null));
			int randomValue = Random.Range(0, 3);
			base.Parent.Avatar.ShowBubble(LocalStringManager.Get(LanguageKey.LK_GearMateAttribute_SpeakWord0 + randomValue), 1.5f);
			base.Parent.Avatar.DoGearMateAnimation("break_1");
			CS$<>8__locals1.coroutineCount = 0;
			for (int i = 0; i < base.GearMate.MainAttributeProgress.Length; i++)
			{
				Refers refers = this._attributeBgList[i].GetComponent<Refers>();
				GameObject lightGo = refers.CGet<GameObject>("Light");
				CImage cache = refers.CGet<CImage>("Cache");
				cache.fillAmount = 0f;
				bool flag2 = this._previewProcessValue[i] > this._currentProcessValue[i];
				if (flag2)
				{
					GearMateAttributeUpdatePage.<>c__DisplayClass23_2 CS$<>8__locals2 = new GearMateAttributeUpdatePage.<>c__DisplayClass23_2();
					CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
					CS$<>8__locals2.duangkuangPrefab = refers.CGet<GameObject>("eff_gearmate_zhujian_duankuang");
					Transform processMask = this._attributeBgList[i].GetComponent<Refers>().CGet<GameObject>("ProcessMaskYellow").transform;
					int coroutineCount = CS$<>8__locals2.CS$<>8__locals1.coroutineCount;
					CS$<>8__locals2.CS$<>8__locals1.coroutineCount = coroutineCount + 1;
					lightGo.SetActive(true);
					processMask.localScale = new Vector3(Math.Max(0.001f, (float)this._currentProcessPercent[i] / 100f), 1f, 1f);
					this._attributeBgList[i].GetComponent<Refers>().CGet<GameObject>("ProcessMaskBlue").transform.localScale = new Vector3(0.001f, 1f, 1f);
					int count = this._previewAttributeValue[i] - this._currentAttributeValue[i];
					base.StartCoroutine(base.ContinuousScaleCoroutine(processMask, count, 1.5f / (float)(count + 1), new Vector3((float)this._previewProcessPercent[i] / 100f, 1f, 1f), i, new Action<int, int>(CS$<>8__locals2.<PlayUpgradeAnim>g__PlaySingleUpdateAnim|3), new Action(CS$<>8__locals2.CS$<>8__locals1.<PlayUpgradeAnim>g__coroutineEndAction|0)));
				}
			}
		}

		// Token: 0x060048CD RID: 18637 RVA: 0x00220FF0 File Offset: 0x0021F1F0
		public override bool CheckItemInteractable(ItemDisplayData itemDisplayData, out int canSelectCount)
		{
			sbyte resourceType = Config.Material.Instance[itemDisplayData.Key.TemplateId].ResourceType;
			sbyte upgradeType = GearMateUpgradeType.GetMainAttributeUpgradeTypeByResourceType(resourceType);
			canSelectCount = 0;
			bool flag = upgradeType < 0;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				int previewValue = this._previewAttributeValue[(int)upgradeType];
				bool flag2 = previewValue >= (int)GlobalConfig.Instance.MaxValueOfMaxMainAttributes;
				if (flag2)
				{
					result = false;
				}
				else
				{
					sbyte grade = Config.Material.Instance[itemDisplayData.Key.TemplateId].Grade;
					int addProcessValue = GearMateSubPageBase.CalcGradeProcessValue(grade);
					int previewProcessValue = this._previewProcessValue[(int)upgradeType];
					for (int i = 0; i < itemDisplayData.Amount; i++)
					{
						int num;
						int increaseCount = this.CalcIncreaseCount((int)upgradeType, previewProcessValue, out num);
						int curValue = this._currentAttributeValue[(int)upgradeType] + increaseCount;
						bool flag3 = curValue < (int)GlobalConfig.Instance.MaxValueOfMaxMainAttributes;
						if (flag3)
						{
							previewProcessValue += addProcessValue;
							canSelectCount++;
						}
					}
					result = true;
				}
			}
			return result;
		}

		// Token: 0x060048CE RID: 18638 RVA: 0x002210EC File Offset: 0x0021F2EC
		public override void RefreshItemTipNotInteractable(ItemView itemView)
		{
			sbyte resourceType = Config.Material.Instance[itemView.Data.Key.TemplateId].ResourceType;
			sbyte upgradeType = GearMateUpgradeType.GetMainAttributeUpgradeTypeByResourceType(resourceType);
			TooltipInvoker tip = itemView.GetMouseTip();
			tip.enabled = true;
			tip.Type = TipType.SingleDesc;
			string charName = base.GetGearMateName();
			string typeName = GearMateUpgradeType.GetMainAttributeUpgradeTypeName(upgradeType);
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

		// Token: 0x060048CF RID: 18639 RVA: 0x0022119C File Offset: 0x0021F39C
		private void InitRefers()
		{
			this._attributeBgList = base.CGetList<Refers>("AttributeBg_");
			this._gearMateMachine_0 = base.CGet<SkeletonGraphic>("GearMateMachine_0");
			this._handle = base.CGet<GameObject>("handle");
			this._attributeBg_0 = base.CGet<Refers>("AttributeBg_0");
			this._eff_gearmate_zhujian_huoxing = base.CGet<GameObject>("eff_gearmate_zhujian_huoxing");
			this._buttonConfirm = base.CGet<CButtonObsolete>("ButtonConfirm");
			this._itemPrefab = base.CGet<GameObject>("ItemPrefab");
			this._itemPrefabLeftPoint = base.CGet<GameObject>("ItemPrefabLeftPoint");
			this._itemPrefabRightPoint = base.CGet<GameObject>("ItemPrefabRightPoint");
			this._gearMateMachine_3 = base.CGet<PolygonCollider2D>("GearMateMachine_3");
			this._eff_gearmate_zhujian_tubiaozha = base.CGet<GameObject>("eff_gearmate_zhujian_tubiaozha");
		}

		// Token: 0x060048D0 RID: 18640 RVA: 0x00221265 File Offset: 0x0021F465
		private static int ClampAttr(int value)
		{
			return Math.Clamp(value, 0, (int)GlobalConfig.Instance.MaxValueOfMaxMainAttributes);
		}

		// Token: 0x0400328C RID: 12940
		private int[] _currentAttributeValue = new int[6];

		// Token: 0x0400328D RID: 12941
		private int[] _previewAttributeValue = new int[6];

		// Token: 0x0400328E RID: 12942
		private int[] _currentProcessValue = new int[6];

		// Token: 0x0400328F RID: 12943
		private int[] _previewProcessValue = new int[6];

		// Token: 0x04003290 RID: 12944
		private int[] _currentProcessPercent = new int[6];

		// Token: 0x04003291 RID: 12945
		private int[] _previewProcessPercent = new int[6];

		// Token: 0x04003292 RID: 12946
		private List<Refers> _attributeBgList;

		// Token: 0x04003293 RID: 12947
		private SkeletonGraphic _gearMateMachine_0;

		// Token: 0x04003294 RID: 12948
		private GameObject _handle;

		// Token: 0x04003295 RID: 12949
		private Refers _attributeBg_0;

		// Token: 0x04003296 RID: 12950
		private GameObject _eff_gearmate_zhujian_huoxing;

		// Token: 0x04003297 RID: 12951
		private CButtonObsolete _buttonConfirm;

		// Token: 0x04003298 RID: 12952
		private GameObject _itemPrefab;

		// Token: 0x04003299 RID: 12953
		private GameObject _itemPrefabLeftPoint;

		// Token: 0x0400329A RID: 12954
		private GameObject _itemPrefabRightPoint;

		// Token: 0x0400329B RID: 12955
		private PolygonCollider2D _gearMateMachine_3;

		// Token: 0x0400329C RID: 12956
		private GameObject _eff_gearmate_zhujian_tubiaozha;
	}
}
