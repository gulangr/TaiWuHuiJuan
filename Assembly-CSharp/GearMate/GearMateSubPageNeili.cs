using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GameData.Domains.Extra;
using GameData.Domains.Item;
using GameData.Domains.Taiwu;
using Spine.Unity;
using TMPro;
using UnityEngine;

namespace GearMate
{
	// Token: 0x0200061C RID: 1564
	public class GearMateSubPageNeili : GearMateSubPageBase
	{
		// Token: 0x1700093A RID: 2362
		// (get) Token: 0x060049B6 RID: 18870 RVA: 0x0022685D File Offset: 0x00224A5D
		private CButtonObsolete BtnConfirm
		{
			get
			{
				return base.CGet<CButtonObsolete>("ButtonConfirm");
			}
		}

		// Token: 0x1700093B RID: 2363
		// (get) Token: 0x060049B7 RID: 18871 RVA: 0x0022686A File Offset: 0x00224A6A
		private GameObject Notice
		{
			get
			{
				return base.CGet<GameObject>("Notice");
			}
		}

		// Token: 0x1700093C RID: 2364
		// (get) Token: 0x060049B8 RID: 18872 RVA: 0x00226877 File Offset: 0x00224A77
		private Refers NeiliProgress
		{
			get
			{
				return base.CGet<Refers>("NeiliProgress");
			}
		}

		// Token: 0x1700093D RID: 2365
		// (get) Token: 0x060049B9 RID: 18873 RVA: 0x00226884 File Offset: 0x00224A84
		private Refers NeiliAllocationProgress
		{
			get
			{
				return base.CGet<Refers>("NeiliAllocationProgress");
			}
		}

		// Token: 0x1700093E RID: 2366
		// (get) Token: 0x060049BA RID: 18874 RVA: 0x00226891 File Offset: 0x00224A91
		private Refers Attack
		{
			get
			{
				return base.CGet<Refers>("Attack");
			}
		}

		// Token: 0x1700093F RID: 2367
		// (get) Token: 0x060049BB RID: 18875 RVA: 0x0022689E File Offset: 0x00224A9E
		private Refers Agility
		{
			get
			{
				return base.CGet<Refers>("Agility");
			}
		}

		// Token: 0x17000940 RID: 2368
		// (get) Token: 0x060049BC RID: 18876 RVA: 0x002268AB File Offset: 0x00224AAB
		private Refers Defence
		{
			get
			{
				return base.CGet<Refers>("Defence");
			}
		}

		// Token: 0x17000941 RID: 2369
		// (get) Token: 0x060049BD RID: 18877 RVA: 0x002268B8 File Offset: 0x00224AB8
		private Refers Assist
		{
			get
			{
				return base.CGet<Refers>("Assist");
			}
		}

		// Token: 0x17000942 RID: 2370
		// (get) Token: 0x060049BE RID: 18878 RVA: 0x002268C5 File Offset: 0x00224AC5
		private GameObject ItemPrefab
		{
			get
			{
				return base.CGet<GameObject>("ItemPrefab");
			}
		}

		// Token: 0x17000943 RID: 2371
		// (get) Token: 0x060049BF RID: 18879 RVA: 0x002268D2 File Offset: 0x00224AD2
		private GameObject ItemPrefabLeftPoint
		{
			get
			{
				return base.CGet<GameObject>("ItemPrefabLeftPoint");
			}
		}

		// Token: 0x17000944 RID: 2372
		// (get) Token: 0x060049C0 RID: 18880 RVA: 0x002268DF File Offset: 0x00224ADF
		private GameObject ItemPrefabRightPoint
		{
			get
			{
				return base.CGet<GameObject>("ItemPrefabRightPoint");
			}
		}

		// Token: 0x17000945 RID: 2373
		// (get) Token: 0x060049C1 RID: 18881 RVA: 0x002268EC File Offset: 0x00224AEC
		private GameObject FilterParticle
		{
			get
			{
				return base.CGet<GameObject>("FilterParticle");
			}
		}

		// Token: 0x17000946 RID: 2374
		// (get) Token: 0x060049C2 RID: 18882 RVA: 0x002268F9 File Offset: 0x00224AF9
		private GameObject DropParticle
		{
			get
			{
				return base.CGet<GameObject>("DropParticle");
			}
		}

		// Token: 0x17000947 RID: 2375
		// (get) Token: 0x060049C3 RID: 18883 RVA: 0x00226906 File Offset: 0x00224B06
		private int GearMateMaxTotalNeiliAllocation
		{
			get
			{
				return (int)(GlobalConfig.Instance.MaxExtraNeiliAllocation * 4);
			}
		}

		// Token: 0x060049C4 RID: 18884 RVA: 0x00226914 File Offset: 0x00224B14
		protected override void InitInternal()
		{
			base.InitInternal();
			for (byte i = 0; i < 4; i += 1)
			{
				sbyte type = (sbyte)(13 + i);
				Refers refers = this.GetNeiliAllocationTypeRefers(type);
				refers.CGet<CButtonObsolete>("Add").ClearAndAddListener(delegate
				{
					ExtraDomainMethod.Call.UpgradeGearMate(this.GearMateDisplayData.CharacterId, type, ItemKey.Invalid, 1);
					ExtraDomainMethod.Call.GetGearMateById(this.Parent.Element.GameDataListenerId, this.Parent.GearMateDisplayData.CharacterId);
				});
				refers.CGet<CButtonObsolete>("Reduce").ClearAndAddListener(delegate
				{
					ExtraDomainMethod.Call.UpgradeGearMate(this.GearMateDisplayData.CharacterId, type, ItemKey.Invalid, -1);
					ExtraDomainMethod.Call.GetGearMateById(this.Parent.Element.GameDataListenerId, this.Parent.GearMateDisplayData.CharacterId);
				});
			}
		}

		// Token: 0x060049C5 RID: 18885 RVA: 0x0022699C File Offset: 0x00224B9C
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

		// Token: 0x060049C6 RID: 18886 RVA: 0x00226A7C File Offset: 0x00224C7C
		private void OnEnable()
		{
			this.Refresh();
		}

		// Token: 0x060049C7 RID: 18887 RVA: 0x00226A86 File Offset: 0x00224C86
		public override void OnGearMateDataChanged()
		{
			base.OnGearMateDataChanged();
			this.Refresh();
		}

		// Token: 0x060049C8 RID: 18888 RVA: 0x00226A98 File Offset: 0x00224C98
		public override void OnItemChanged(ItemKey itemKey, int amount, bool queueAnim = false, bool isAllSelected = false, bool playItemAnim = true)
		{
			this.SetButtonState(base.Parent.ItemSelector.SelectedItemsTotalCount != 0);
			sbyte grade = ItemTemplateHelper.GetGrade(itemKey.ItemType, itemKey.TemplateId);
			this._addValue = GearMateSubPageBase.CalcGradeProcessValue(grade) * amount;
			int prevDisplayValue = Math.Min(base.Parent.GearMate.Neili, 33572);
			int currDisplayValue = Math.Min(base.Parent.GearMate.Neili + this._addValue, 33572);
			float ratio = this.GetNeiliProgress((float)(base.Parent.GearMate.Neili + this._addValue));
			string displayString = (prevDisplayValue == currDisplayValue) ? prevDisplayValue.ToString() : string.Format("{0}+{1}", prevDisplayValue, currDisplayValue - prevDisplayValue);
			this.NeiliProgress.CGet<TextMeshProUGUI>("Text").SetText(displayString, true);
			this.NeiliProgress.CGet<RectTransform>("ProcessMaskBlue").localScale = this.NeiliProgress.CGet<RectTransform>("ProcessMaskBlue").localScale.SetX(ratio);
			int num;
			int prevMaxCount;
			float num2;
			this.GetNeiliAllocationData(base.Parent.GearMate.Neili, out num, out prevMaxCount, out num2);
			int usedCount;
			int currMaxCount;
			float allocationRatio;
			this.GetNeiliAllocationData(base.Parent.GearMate.Neili + this._addValue, out usedCount, out currMaxCount, out allocationRatio);
			string maxCountText = (prevMaxCount == currMaxCount) ? string.Format("{0}/{1}", prevMaxCount - usedCount, prevMaxCount) : string.Format("{0} + {1}/{2}+{3}", new object[]
			{
				prevMaxCount - usedCount,
				currMaxCount - prevMaxCount,
				prevMaxCount,
				currMaxCount - prevMaxCount
			});
			this.NeiliAllocationProgress.CGet<TextMeshProUGUI>("Text").SetText(maxCountText, true);
			this.NeiliAllocationProgress.CGet<RectTransform>("ProcessMaskBlue").localScale = this.NeiliAllocationProgress.CGet<RectTransform>("ProcessMaskBlue").localScale.SetX(allocationRatio);
			bool flag = amount > 0 && playItemAnim;
			if (flag)
			{
				GameObject itemPrefab = Object.Instantiate<GameObject>(this.ItemPrefab, this.ItemPrefab.transform.parent);
				ItemDrop itemDrop = itemPrefab.GetComponent<ItemDrop>();
				itemDrop.eff_gearmate_zhujian_tubiaozha = this.FilterParticle;
				itemDrop.OnTrigger += delegate()
				{
					this.DropParticle.GetComponent<ParticleSystem>().Play();
					bool flag2 = !queueAnim;
					if (flag2)
					{
						this.SetMachineWaterHeight();
					}
				};
				itemPrefab.transform.position = Vector3.Lerp(this.ItemPrefabLeftPoint.transform.position, this.ItemPrefabRightPoint.transform.position, Random.Range(0f, 1f));
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

		// Token: 0x060049C9 RID: 18889 RVA: 0x00226D9E File Offset: 0x00224F9E
		public override void Confirm(ItemKeyAndCount itemKeyAndCount, ItemSourceType itemSourceType)
		{
			this.SetButtonState(false);
			ExtraDomainMethod.Call.UpgradeGearMate(base.GearMateDisplayData.CharacterId, 11, itemKeyAndCount.ItemKey, itemKeyAndCount.Count, itemSourceType);
		}

		// Token: 0x060049CA RID: 18890 RVA: 0x00226DCC File Offset: 0x00224FCC
		public override void PlayUpgradeAnim(Action action)
		{
			GameObject handle = base.CGet<GameObject>("handle");
			base.CGet<GameObject>("GearMateMachine").GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "move", false);
			AudioManager.Instance.PlaySound("SFX_GearMate_machine_loop", false, false);
			Action <>9__4;
			Action <>9__5;
			action = (Action)Delegate.Combine(action, new Action(delegate()
			{
				GearMateSubPageBase <>4__this = this;
				GearMateSubPageBase <>4__this2 = this;
				GameObject processYellow = this.NeiliProgress.CGet<GameObject>("TransparentParticle");
				Action action2;
				if ((action2 = <>9__4) == null)
				{
					action2 = (<>9__4 = delegate()
					{
						this.NeiliProgress.CGet<RectTransform>("ProcessMaskYellow").localScale = new Vector3(0.001f, 1f, 1f);
					});
				}
				<>4__this.StartCoroutine(<>4__this2.SetProcessYellowTransparent(processYellow, action2, 0.5f));
				GearMateSubPageBase <>4__this3 = this;
				GearMateSubPageBase <>4__this4 = this;
				GameObject processYellow2 = this.NeiliAllocationProgress.CGet<GameObject>("TransparentParticle");
				Action action3;
				if ((action3 = <>9__5) == null)
				{
					action3 = (<>9__5 = delegate()
					{
						this.NeiliAllocationProgress.CGet<RectTransform>("ProcessMaskYellow").localScale = new Vector3(0.001f, 1f, 1f);
					});
				}
				<>4__this3.StartCoroutine(<>4__this4.SetProcessYellowTransparent(processYellow2, action3, 0.5f));
			}));
			bool flag = this.HeightCoroutine != null;
			if (flag)
			{
				base.StopCoroutine(this.HeightCoroutine);
			}
			this.HeightCoroutine = base.StartCoroutine(base.ScaleCoroutine(handle.transform, 1.13f, new Vector3(0f, 1f, 1f), null));
			Transform transform = base.CGet<GameObject>("LeakParticle").transform;
			float x = handle.transform.localScale.x;
			if (!true)
			{
			}
			int index;
			if (x < 0.67f)
			{
				if (x < 0.33f)
				{
					index = 0;
				}
				else
				{
					index = 1;
				}
			}
			else
			{
				index = 2;
			}
			if (!true)
			{
			}
			transform.GetChild(index).GetComponent<ParticleSystem>().Play();
			this.ShowBubble(1.5f);
			int prevDisplayValue = Math.Min(base.Parent.GearMate.Neili, 33572);
			int currDisplayValue = Math.Min(base.Parent.GearMate.Neili + this._addValue, 33572);
			float ratio = this.GetNeiliProgress((float)(base.Parent.GearMate.Neili + this._addValue));
			int prevMaxCount;
			float num;
			this.GetNeiliAllocationData(base.Parent.GearMate.Neili, out index, out prevMaxCount, out num);
			float allocationRatio;
			int usedCount;
			int currMaxCount;
			this.GetNeiliAllocationData(base.Parent.GearMate.Neili + this._addValue, out usedCount, out currMaxCount, out allocationRatio);
			int addCount = currMaxCount - prevMaxCount;
			SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(0.34f, delegate
			{
				this.NeiliProgress.CGet<RectTransform>("ProcessMaskBlue").localScale = this.NeiliProgress.CGet<RectTransform>("ProcessMaskBlue").localScale.SetX(0.001f);
				this.NeiliAllocationProgress.CGet<RectTransform>("ProcessMaskBlue").localScale = this.NeiliProgress.CGet<RectTransform>("ProcessMaskBlue").localScale.SetX(0.001f);
				this.StartCoroutine(this.ContinuousScaleCoroutine(this.NeiliProgress.CGet<RectTransform>("ProcessMaskYellow").transform, currDisplayValue - prevDisplayValue, 1.16f / (float)(currDisplayValue - prevDisplayValue + 1), new Vector3(ratio, 1f, 1f), 0, new Action<int, int>(base.<PlayUpgradeAnim>g__PlayNeiliAnim|2), action));
				this.StartCoroutine(this.ContinuousScaleCoroutine(this.NeiliAllocationProgress.CGet<RectTransform>("ProcessMaskYellow").transform, addCount, 1.16f / (float)(addCount + 1), new Vector3(allocationRatio, 1f, 1f), 0, new Action<int, int>(base.<PlayUpgradeAnim>g__PlayNeiliAllocationAnim|3), null));
			});
		}

		// Token: 0x060049CB RID: 18891 RVA: 0x00226FEF File Offset: 0x002251EF
		public override void SetButtonState(bool state)
		{
			this.BtnConfirm.interactable = (state && base.Parent.GearMate.Neili < 99999);
		}

		// Token: 0x060049CC RID: 18892 RVA: 0x0022701C File Offset: 0x0022521C
		private Refers GetNeiliAllocationTypeRefers(sbyte type)
		{
			if (!true)
			{
			}
			Refers result;
			switch (type)
			{
			case 13:
				result = this.Attack;
				break;
			case 14:
				result = this.Agility;
				break;
			case 15:
				result = this.Defence;
				break;
			case 16:
				result = this.Assist;
				break;
			default:
				throw new ArgumentOutOfRangeException("type", type, null);
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x060049CD RID: 18893 RVA: 0x00227083 File Offset: 0x00225283
		private unsafe int GetNeiliAllocationTypeValue(sbyte type)
		{
			return (int)(*base.Parent.GearMate.NeiliAllocation[(int)((byte)(type - 13))]);
		}

		// Token: 0x060049CE RID: 18894 RVA: 0x002270A0 File Offset: 0x002252A0
		private void GetNeiliAllocationData(int neili, out int usedCount, out int maxCount, out float ratio)
		{
			short cost = GlobalConfig.Instance.ExtraNeiliAllocationFromProgressRatio;
			usedCount = (int)base.Parent.GearMate.NeiliAllocation.GetTotal();
			maxCount = 0;
			while (neili > (int)cost)
			{
				neili -= (int)cost;
				maxCount++;
				bool flag = maxCount % 4 == 0;
				if (flag)
				{
					cost += GlobalConfig.Instance.ExtraNeiliAllocationFromProgressRatioGrowth;
				}
			}
			bool flag2 = maxCount == this.GearMateMaxTotalNeiliAllocation;
			if (flag2)
			{
				ratio = 1f;
			}
			else
			{
				ratio = Math.Max((float)neili / (float)cost, 0.001f);
			}
		}

		// Token: 0x060049CF RID: 18895 RVA: 0x00227130 File Offset: 0x00225330
		private float GetNeiliProgress(float value)
		{
			return Math.Max(value / 33572f, 0.001f);
		}

		// Token: 0x060049D0 RID: 18896 RVA: 0x00227153 File Offset: 0x00225353
		private void Refresh()
		{
			this.Notice.SetActive(base.Parent.GearMate.Neili >= 99999);
			this.RefreshProgress();
			this.RefreshNeiliAllocationTypes();
		}

		// Token: 0x060049D1 RID: 18897 RVA: 0x0022718C File Offset: 0x0022538C
		private void RefreshProgress()
		{
			float ratio = this.GetNeiliProgress((float)base.Parent.GearMate.Neili);
			int displayValue = Math.Min(base.Parent.GearMate.Neili, 33572);
			this.NeiliProgress.CGet<TextMeshProUGUI>("Text").SetText(displayValue.ToString(), true);
			this.NeiliProgress.CGet<RectTransform>("ProcessMaskBlue").localScale = this.NeiliProgress.CGet<RectTransform>("ProcessMaskBlue").localScale.SetX(ratio);
			int usedCount;
			int maxCount;
			float allocationRatio;
			this.GetNeiliAllocationData(base.Parent.GearMate.Neili, out usedCount, out maxCount, out allocationRatio);
			this.NeiliAllocationProgress.CGet<TextMeshProUGUI>("Text").SetText(string.Format("{0}/{1}", maxCount - usedCount, maxCount), true);
			this.NeiliAllocationProgress.CGet<RectTransform>("ProcessMaskBlue").localScale = this.NeiliProgress.CGet<RectTransform>("ProcessMaskBlue").localScale.SetX(allocationRatio);
		}

		// Token: 0x060049D2 RID: 18898 RVA: 0x0022729C File Offset: 0x0022549C
		private void RefreshNeiliAllocationTypes()
		{
			for (sbyte i = 13; i <= 16; i += 1)
			{
				this.RefreshNeiliAllocationType(i);
			}
		}

		// Token: 0x060049D3 RID: 18899 RVA: 0x002272C8 File Offset: 0x002254C8
		private void RefreshNeiliAllocationType(sbyte type)
		{
			Refers refers = this.GetNeiliAllocationTypeRefers(type);
			int value = this.GetNeiliAllocationTypeValue(type);
			int usedCount;
			int maxCount;
			float num;
			this.GetNeiliAllocationData(base.Parent.GearMate.Neili, out usedCount, out maxCount, out num);
			refers.CGet<TextMeshProUGUI>("Value").SetText(string.Format("{0}/{1}", value, GlobalConfig.Instance.MaxExtraNeiliAllocation), true);
			refers.CGet<CButtonObsolete>("Add").interactable = (value < (int)GlobalConfig.Instance.MaxExtraNeiliAllocation && usedCount < maxCount);
			refers.CGet<CButtonObsolete>("Reduce").interactable = (value > 0);
		}

		// Token: 0x060049D4 RID: 18900 RVA: 0x00227370 File Offset: 0x00225570
		private void ShowBubble(float duration)
		{
			LanguageKey[] array = new LanguageKey[3];
			RuntimeHelpers.InitializeArray(array, fieldof(<PrivateImplementationDetails>.BA20F944E14E2AFDE63DA9D7069024021FB5D6ED204A07E164D5C67EBA28168B).FieldHandle);
			LanguageKey[] keys = array;
			int id = Random.Range(0, 3);
			base.Parent.Avatar.ShowBubble(LocalStringManager.Get(keys[id]), duration);
			base.Parent.Avatar.DoGearMateAnimation("break_2");
		}

		// Token: 0x04003323 RID: 13091
		private const float Tolerate = 0.001f;

		// Token: 0x04003324 RID: 13092
		private const int GearMateMaxDisplayNeili = 33572;

		// Token: 0x04003325 RID: 13093
		private const int GearMateMaxNeili = 99999;

		// Token: 0x04003326 RID: 13094
		private int _addValue;
	}
}
