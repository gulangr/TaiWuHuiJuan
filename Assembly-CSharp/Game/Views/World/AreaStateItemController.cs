using System;
using System.Collections.Generic;
using Config;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.World
{
	// Token: 0x02000727 RID: 1831
	public class AreaStateItemController : MonoBehaviour
	{
		// Token: 0x06005790 RID: 22416 RVA: 0x0028AD5C File Offset: 0x00288F5C
		private void Awake()
		{
			bool flag = AreaStateItemController.Checker.Enabled.Length != MapLegend.Instance.Count;
			if (flag)
			{
				this._initialized = false;
			}
			bool initialized = this._initialized;
			if (!initialized)
			{
				AreaStateItemController.Checker.Enabled = new bool[MapLegend.Instance.Count];
				this._initialized = true;
				this.multiSelect.Clear();
				this.multiSelectIndexes.Clear();
				int i = this.multiSelectToggles.transform.childCount;
				while (i-- > 0)
				{
					Object.Destroy(this.multiSelectToggles.transform.GetChild(i).gameObject);
				}
				foreach (MapLegendItem item in ((IEnumerable<MapLegendItem>)MapLegend.Instance))
				{
					bool flag2 = !item.ShowInAreaMap;
					if (!flag2)
					{
						MapLegend legend = Object.Instantiate<MapLegend>(this.mapLegendTemplate, this.multiSelectToggles.transform, false);
						legend.Init(item, this.multiSelect);
						this.multiSelectIndexes.Add((int)item.TemplateId);
						sbyte templateId = item.TemplateId;
						sbyte b = templateId;
						switch (b)
						{
						case 3:
							this.loong1 = legend.gameObject;
							break;
						case 4:
						case 5:
							break;
						case 6:
							this.dreamback = legend.gameObject;
							break;
						case 7:
							this.loong2 = legend.gameObject;
							break;
						default:
							if (b == 17)
							{
								this.infectedDaemon = legend.gameObject;
							}
							break;
						}
					}
				}
				this.multiSelect.allowSwitchOff = (this.multiSelect.allowUncheck = true);
				this.multiSelect.Init();
				this.multiSelect.OnActiveIndexChange -= this.Select;
				this.multiSelect.OnActiveIndexChange += this.Select;
				this.openAll.onClick.ResetListener(delegate()
				{
					this._raiseEvent = false;
					this.multiSelect.SelectAll(false);
					this._raiseEvent = true;
					ViewPartWorldMap.RefreshData();
				});
				this.closeAll.onClick.ResetListener(delegate()
				{
					this._raiseEvent = false;
					this.multiSelect.DeSelectAll(false);
					this._raiseEvent = true;
					ViewPartWorldMap.RefreshData();
				});
				this.hideAll.ClearAndAddListener(new Action(this.ChangeHide));
				this.closeBtn.ClearAndAddListener(new Action(this.OnClickCloseButton));
			}
		}

		// Token: 0x06005791 RID: 22417 RVA: 0x0028AFE0 File Offset: 0x002891E0
		private void OnClickCloseButton()
		{
			base.gameObject.SetActive(false);
		}

		// Token: 0x06005792 RID: 22418 RVA: 0x0028AFF0 File Offset: 0x002891F0
		private void ChangeHide()
		{
			this.RefreshHide(true);
		}

		// Token: 0x06005793 RID: 22419 RVA: 0x0028AFFC File Offset: 0x002891FC
		private void RefreshHide(bool change)
		{
			if (change)
			{
				this._disableToggle = !this._disableToggle;
			}
			AreaStateItemController.Checker.HideAll = this._disableToggle;
			this.multiSelect.SetInteractable(!this._disableToggle);
			bool raiseEvent = this._raiseEvent;
			if (raiseEvent)
			{
				ViewPartWorldMap.RefreshData();
			}
			this.hideAllText.SetText(this._disableToggle ? LanguageKey.LK_Travel_EnableAll.Tr() : LanguageKey.LK_Travel_DisableAll.Tr(), true);
		}

		// Token: 0x06005794 RID: 22420 RVA: 0x0028B080 File Offset: 0x00289280
		public void Select(int on, int off)
		{
			bool flag = this.multiSelectIndexes.CheckIndex(on) && AreaStateItemController.Checker.Enabled.CheckIndex(this.multiSelectIndexes[on]);
			if (flag)
			{
				AreaStateItemController.Checker[this.multiSelectIndexes[on]] = true;
			}
			bool flag2 = this.multiSelectIndexes.CheckIndex(off) && AreaStateItemController.Checker.Enabled.CheckIndex(this.multiSelectIndexes[off]);
			if (flag2)
			{
				AreaStateItemController.Checker[this.multiSelectIndexes[off]] = false;
			}
			bool raiseEvent = this._raiseEvent;
			if (raiseEvent)
			{
				ViewPartWorldMap.RefreshData();
			}
		}

		// Token: 0x06005795 RID: 22421 RVA: 0x0028B130 File Offset: 0x00289330
		public void OnInit()
		{
			this.Awake();
			this._raiseEvent = true;
			AreaStateItemController.Checker.Set(SingletonObject.getInstance<GlobalSettings>());
			int i = AreaStateItemController.Checker.Enabled.Length;
			while (i-- > 0)
			{
				bool flag = AreaStateItemController.Checker.Enabled[i];
				if (flag)
				{
					this.multiSelect.SelectWithoutNotify(i);
				}
				else
				{
					this.multiSelect.DeSelectWithoutNotify(i);
				}
			}
			bool loongActivated = SingletonObject.getInstance<DlcManager>().IsDlcInstalled(2764950U);
			this.loong1.SetActive(loongActivated);
			this.loong2.SetActive(loongActivated);
			BasicGameData data = SingletonObject.getInstance<BasicGameData>();
			this.dreamback.SetActive(data.IsDreamBack);
			this.infectedDaemon.SetActive(data.ChallengeModeData.IsEnabled(EChallengeModeImplement.NI2));
			this._disableToggle = AreaStateItemController.Checker.HideAll;
			this.RefreshHide(false);
		}

		// Token: 0x04003C09 RID: 15369
		public static readonly GlobalSettingsChecker Checker = new GlobalSettingsChecker();

		// Token: 0x04003C0A RID: 15370
		[SerializeField]
		private CToggleGroupMultiSelect multiSelect;

		// Token: 0x04003C0B RID: 15371
		[SerializeField]
		private CButton hideAll;

		// Token: 0x04003C0C RID: 15372
		[SerializeField]
		private TextMeshProUGUI hideAllText;

		// Token: 0x04003C0D RID: 15373
		[SerializeField]
		private GameObject loong1;

		// Token: 0x04003C0E RID: 15374
		[SerializeField]
		private GameObject loong2;

		// Token: 0x04003C0F RID: 15375
		[SerializeField]
		private GameObject dreamback;

		// Token: 0x04003C10 RID: 15376
		[SerializeField]
		private GameObject infectedDaemon;

		// Token: 0x04003C11 RID: 15377
		[SerializeField]
		private GridLayoutGroup multiSelectToggles;

		// Token: 0x04003C12 RID: 15378
		[SerializeField]
		private MapLegend mapLegendTemplate;

		// Token: 0x04003C13 RID: 15379
		[SerializeField]
		private CButton closeBtn;

		// Token: 0x04003C14 RID: 15380
		[SerializeField]
		private CButton closeAll;

		// Token: 0x04003C15 RID: 15381
		[SerializeField]
		private CButton openAll;

		// Token: 0x04003C16 RID: 15382
		private bool _initialized;

		// Token: 0x04003C17 RID: 15383
		private bool _disableToggle;

		// Token: 0x04003C18 RID: 15384
		private List<int> multiSelectIndexes = new List<int>();

		// Token: 0x04003C19 RID: 15385
		private bool _raiseEvent = true;
	}
}
