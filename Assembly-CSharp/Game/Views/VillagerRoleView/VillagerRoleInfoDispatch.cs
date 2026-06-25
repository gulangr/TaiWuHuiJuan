using System;
using System.Collections;
using System.Collections.Generic;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;

namespace Game.Views.VillagerRoleView
{
	// Token: 0x0200073B RID: 1851
	public class VillagerRoleInfoDispatch : MonoBehaviour
	{
		// Token: 0x06005982 RID: 22914 RVA: 0x0029865D File Offset: 0x0029685D
		private void Awake()
		{
			this.dispatchButton.ClearAndAddListener(new Action(this.OpenDispatchPanel));
		}

		// Token: 0x06005983 RID: 22915 RVA: 0x00298678 File Offset: 0x00296878
		public void OnInit()
		{
			this._lastRoleTemplateId = -1;
		}

		// Token: 0x06005984 RID: 22916 RVA: 0x00298684 File Offset: 0x00296884
		public void Refresh(VillagerRoleItem roleConfig, ViewVillagerRole parent)
		{
			this._config = roleConfig;
			bool flag = this._lastRoleTemplateId == roleConfig.TemplateId;
			if (flag)
			{
				this._lastRoleTemplateId = roleConfig.TemplateId;
			}
			else
			{
				this.dispatchButton.interactable = VillagerRoleInfoDispatch.RoleFunctionEnabled(roleConfig.TemplateId);
				this.dispatchButton.gameObject.GetOrAddComponent<DisableStyleRoot>().SetStyleEffect(!this.dispatchButton.interactable, false);
				this.dispatchButton.gameObject.SetActive(CommonUtils.CheckRoleDispatch(roleConfig.TemplateId));
				this._chickenBgCheckItems.Clear();
				this._parent = parent;
				this.RefreshInner();
				this._lastRoleTemplateId = roleConfig.TemplateId;
			}
		}

		// Token: 0x06005985 RID: 22917 RVA: 0x00298738 File Offset: 0x00296938
		internal static bool RoleFunctionEnabled(short roleId)
		{
			return roleId != 5 || SingletonObject.getInstance<FunctionLockManager>().IsFunctionUnlock(31);
		}

		// Token: 0x06005986 RID: 22918 RVA: 0x00298760 File Offset: 0x00296960
		private void RefreshInner()
		{
			this._dispatchConfigs.Clear();
			foreach (VillagerRoleArrangementItem dispatchConfig in ((IEnumerable<VillagerRoleArrangementItem>)VillagerRoleArrangement.Instance))
			{
				bool flag = dispatchConfig.VillagerRole != this._config.TemplateId;
				if (!flag)
				{
					this._dispatchConfigs.Add(dispatchConfig);
				}
			}
			this._dispatchConfigs.Sort(new Comparison<VillagerRoleArrangementItem>(this.CompareDispatchConfig));
			CommonUtils.PrepareEnoughChildren(this.dispatchItemRoot, this.dispatchItemTemplate.gameObject, this._dispatchConfigs.Count, null);
			for (int i = 0; i < this._dispatchConfigs.Count; i++)
			{
				VillagerRoleArrangementItem dispatchConfig2 = this._dispatchConfigs[i];
				Refers refers = this.dispatchItemRoot.GetChild(i).GetComponent<Refers>();
				this.RefreshDispatchItem(refers, dispatchConfig2);
			}
			SingletonObject.getInstance<YieldHelper>().StartCoroutine(this.DelayCheckChickenBg());
		}

		// Token: 0x06005987 RID: 22919 RVA: 0x00298884 File Offset: 0x00296A84
		private int CompareDispatchConfig(VillagerRoleArrangementItem x, VillagerRoleArrangementItem y)
		{
			int unlockCompare = x.UnlockByChicken.CompareTo(y.UnlockByChicken);
			bool flag = unlockCompare != 0;
			int result;
			if (flag)
			{
				result = unlockCompare;
			}
			else
			{
				result = x.TemplateId.CompareTo(y.TemplateId);
			}
			return result;
		}

		// Token: 0x06005988 RID: 22920 RVA: 0x002988C5 File Offset: 0x00296AC5
		private IEnumerator DelayCheckChickenBg()
		{
			yield return null;
			this.SelectBgForChickenArea();
			yield break;
		}

		// Token: 0x06005989 RID: 22921 RVA: 0x002988D4 File Offset: 0x00296AD4
		private void SelectBgForChickenArea()
		{
			foreach (VillagerRoleInfoDispatch.ChickenBgCheckItem check in this._chickenBgCheckItems)
			{
				check.ChickenBg.SetSprite((check.ToCheckRect.rect.height > 50f) ? "villagerrole_base_13" : "villagerrole_base_12", false, null);
			}
		}

		// Token: 0x0600598A RID: 22922 RVA: 0x0029895C File Offset: 0x00296B5C
		private void RefreshDispatchItem(Refers refers, VillagerRoleArrangementItem dispatchConfig)
		{
			bool needUnlockByChicken = dispatchConfig.UnlockByChicken;
			bool isExtraEffectUnlocked = this._parent.IsExtraEffectUnlocked((int)this._config.TemplateId);
			bool useChickenStyle = needUnlockByChicken && isExtraEffectUnlocked;
			CImage chickenArea = refers.CGet<CImage>("ChickenArea");
			TextMeshProUGUI descName = refers.CGet<TextMeshProUGUI>("Name");
			TextMeshProUGUI desc2 = refers.CGet<TextMeshProUGUI>("Desc2");
			CImage picture = refers.CGet<CImage>("Pic");
			GameObject hint = refers.CGet<GameObject>("Hint");
			TextMeshProUGUI hintText = refers.CGet<TextMeshProUGUI>("HintText");
			picture.gameObject.SetActive(false);
			picture.SetSprite(dispatchConfig.Illustration, false, delegate
			{
				picture.gameObject.SetActive(true);
			});
			CanvasGroup descCanvasGroup = desc2.GetComponent<CanvasGroup>();
			descCanvasGroup.alpha = 0f;
			descName.text = dispatchConfig.DescName;
			desc2.text = dispatchConfig.DescContent.ColorReplace();
			bool flag = useChickenStyle;
			if (flag)
			{
				this._chickenBgCheckItems.Add(new VillagerRoleInfoDispatch.ChickenBgCheckItem
				{
					ToCheckRect = chickenArea.GetComponent<RectTransform>(),
					ChickenBg = chickenArea
				});
			}
			bool fulongSpecialInteractUnlocked = this._parent.IsFulongSpecialInteractOpen;
			refers.gameObject.SetActive(!needUnlockByChicken || fulongSpecialInteractUnlocked);
			bool needGray = needUnlockByChicken && !isExtraEffectUnlocked;
			DisableStyleRoot disableStyleRoot = refers.GetComponent<DisableStyleRoot>();
			disableStyleRoot.SetStyleEffect(needGray, false);
			hint.SetActive(needGray);
			bool activeSelf = hint.activeSelf;
			if (activeSelf)
			{
				hintText.text = LanguageKey.Lk_VillagerRoleInfoDispatch_HintText.Tr();
			}
			bool flag2 = needGray;
			if (flag2)
			{
				desc2.color = disableStyleRoot.EffectTextColor;
			}
			else
			{
				desc2.color = Colors.Instance["lightgrey"];
			}
			SingletonObject.getInstance<YieldHelper>().StartCoroutine(this.DelayShowDesc(descCanvasGroup));
			desc2.GetComponent<TMPTextSpriteHelper>().Parse();
		}

		// Token: 0x0600598B RID: 22923 RVA: 0x00298B3E File Offset: 0x00296D3E
		private IEnumerator DelayShowDesc(CanvasGroup descCanvasGroup)
		{
			yield return null;
			yield return null;
			yield return null;
			descCanvasGroup.alpha = 1f;
			yield break;
		}

		// Token: 0x0600598C RID: 22924 RVA: 0x00298B54 File Offset: 0x00296D54
		public void OpenDispatchPanel()
		{
			UIElement.VillagerWork.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("RoleTemplateId", this._config.TemplateId));
			UIManager.Instance.ShowUI(UIElement.VillagerWork, true);
		}

		// Token: 0x0600598D RID: 22925 RVA: 0x00298B90 File Offset: 0x00296D90
		public void RefreshByChickUnlock()
		{
			bool flag = this._config == null || this._parent == null;
			if (!flag)
			{
				this.RefreshInner();
			}
		}

		// Token: 0x04003D8F RID: 15759
		private VillagerRoleItem _config;

		// Token: 0x04003D90 RID: 15760
		private ViewVillagerRole _parent;

		// Token: 0x04003D91 RID: 15761
		private short _lastRoleTemplateId = -1;

		// Token: 0x04003D92 RID: 15762
		private readonly List<VillagerRoleInfoDispatch.ChickenBgCheckItem> _chickenBgCheckItems = new List<VillagerRoleInfoDispatch.ChickenBgCheckItem>();

		// Token: 0x04003D93 RID: 15763
		private readonly List<VillagerRoleArrangementItem> _dispatchConfigs = new List<VillagerRoleArrangementItem>();

		// Token: 0x04003D94 RID: 15764
		[SerializeField]
		private RectTransform dispatchItemRoot;

		// Token: 0x04003D95 RID: 15765
		[SerializeField]
		private Refers dispatchItemTemplate;

		// Token: 0x04003D96 RID: 15766
		[SerializeField]
		private CButton dispatchButton;

		// Token: 0x02001C0E RID: 7182
		private struct ChickenBgCheckItem
		{
			// Token: 0x0400BF69 RID: 49001
			public RectTransform ToCheckRect;

			// Token: 0x0400BF6A RID: 49002
			public CImage ChickenBg;
		}
	}
}
