using System;
using FrameWork.UISystem.UIElements;
using Game.Views.SectInteract.Xuehou;
using TMPro;
using UnityEngine;

namespace Game.Views.SectInteract
{
	// Token: 0x020009B9 RID: 2489
	public class XuehouNeiliAllocationItem : MonoBehaviour
	{
		// Token: 0x17000D6B RID: 3435
		// (get) Token: 0x0600789B RID: 30875 RVA: 0x00381B3A File Offset: 0x0037FD3A
		private ViewJixi ViewJixi
		{
			get
			{
				return UIElement.Jixi.UiBaseAs<ViewJixi>();
			}
		}

		// Token: 0x17000D6C RID: 3436
		// (get) Token: 0x0600789C RID: 30876 RVA: 0x00381B46 File Offset: 0x0037FD46
		public CButton NeiliAllocButton
		{
			get
			{
				return this.button;
			}
		}

		// Token: 0x0600789D RID: 30877 RVA: 0x00381B4E File Offset: 0x0037FD4E
		public void Init(Action action)
		{
			this.button.ClearAndAddListener(action);
		}

		// Token: 0x0600789E RID: 30878 RVA: 0x00381B60 File Offset: 0x0037FD60
		public void Set(int curNeili, float curPercent, int previewNeili, float previewPercent, bool isEnable, LanguageKey languageKey)
		{
			this._curPercent = curPercent;
			string changeString = "";
			bool flag = curNeili != previewNeili;
			if (flag)
			{
				bool flag2 = curNeili > previewNeili;
				if (flag2)
				{
					int changeValue = curNeili - previewNeili;
					changeString = string.Format(" - {0}", changeValue);
					changeString = changeString.SetColor(Color.red);
					this._curProgressState = XuehouNeiliAllocationItem.EProgressState.Reduce;
					this._isProgressOver = (changeValue >= 1);
					bool flag3 = changeValue == 1 && curNeili == (int)GlobalConfig.Instance.MaxExtraNeiliAllocation;
					if (flag3)
					{
						this._isProgressOver = false;
					}
				}
				else
				{
					int changeValue = previewNeili - curNeili;
					changeString = string.Format(" + {0}", changeValue);
					changeString = changeString.SetColor(Color.cyan);
					this._curProgressState = XuehouNeiliAllocationItem.EProgressState.Add;
					this._isProgressOver = (changeValue >= 1);
				}
			}
			else
			{
				this._curProgressState = XuehouNeiliAllocationItem.EProgressState.None;
				bool flag4 = curPercent < previewPercent;
				if (flag4)
				{
					this._curProgressState = XuehouNeiliAllocationItem.EProgressState.Add;
				}
				bool flag5 = curPercent > previewPercent;
				if (flag5)
				{
					this._curProgressState = XuehouNeiliAllocationItem.EProgressState.Reduce;
				}
				this._isProgressOver = false;
			}
			switch (this._curProgressState)
			{
			case XuehouNeiliAllocationItem.EProgressState.None:
				this.progress.fillAmount = previewPercent;
				this.progressAdd.fillAmount = previewPercent;
				this.progressReduce.fillAmount = previewPercent;
				this.progress.gameObject.SetActive(true);
				this.progressReduce.gameObject.SetActive(false);
				this.progressAdd.gameObject.SetActive(false);
				break;
			case XuehouNeiliAllocationItem.EProgressState.Add:
				this.AddProcess(previewPercent);
				break;
			case XuehouNeiliAllocationItem.EProgressState.Reduce:
				this.ReduceProgress(previewPercent);
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			this.value.text = string.Format("{0}{1}/{2}", curNeili, changeString, GlobalConfig.Instance.MaxExtraNeiliAllocation);
			this.button.interactable = isEnable;
			TooltipInvoker mouseTip = this.button.GetComponent<TooltipInvoker>();
			mouseTip.enabled = !isEnable;
			mouseTip.PresetParam[0] = languageKey.ToString();
		}

		// Token: 0x0600789F RID: 30879 RVA: 0x00381D68 File Offset: 0x0037FF68
		public void ReduceProgress(float percent)
		{
			this.progressReduce.gameObject.SetActive(true);
			this.progressAdd.gameObject.SetActive(false);
			bool isProgressOver = this._isProgressOver;
			if (isProgressOver)
			{
				this.progressReduce.fillAmount = percent;
				this.progress.gameObject.SetActive(false);
			}
			else
			{
				this.progressReduce.fillAmount = this._curPercent;
				this.progress.fillAmount = percent;
				this.progress.gameObject.SetActive(true);
			}
		}

		// Token: 0x060078A0 RID: 30880 RVA: 0x00381DFC File Offset: 0x0037FFFC
		public void AddProcess(float percent)
		{
			this.progressReduce.gameObject.SetActive(false);
			this.progressAdd.gameObject.SetActive(true);
			bool isProgressOver = this._isProgressOver;
			if (isProgressOver)
			{
				this.progressAdd.fillAmount = percent;
				this.progress.gameObject.SetActive(false);
			}
			else
			{
				this.progressAdd.fillAmount = percent;
				this.progress.fillAmount = this._curPercent;
				this.progress.gameObject.SetActive(true);
			}
		}

		// Token: 0x04005B45 RID: 23365
		[SerializeField]
		private TextMeshProUGUI value;

		// Token: 0x04005B46 RID: 23366
		[SerializeField]
		private CImage progress;

		// Token: 0x04005B47 RID: 23367
		[SerializeField]
		private CImage progressAdd;

		// Token: 0x04005B48 RID: 23368
		[SerializeField]
		private CImage progressReduce;

		// Token: 0x04005B49 RID: 23369
		[SerializeField]
		private CButton button;

		// Token: 0x04005B4A RID: 23370
		[SerializeField]
		private sbyte neiliAllocType = -1;

		// Token: 0x04005B4B RID: 23371
		[SerializeField]
		private bool isTaiwu;

		// Token: 0x04005B4C RID: 23372
		private bool _isProgressOver;

		// Token: 0x04005B4D RID: 23373
		private XuehouNeiliAllocationItem.EProgressState _curProgressState;

		// Token: 0x04005B4E RID: 23374
		private float _curPercent;

		// Token: 0x02001EFE RID: 7934
		private enum EProgressState
		{
			// Token: 0x0400CBDD RID: 52189
			None,
			// Token: 0x0400CBDE RID: 52190
			Add,
			// Token: 0x0400CBDF RID: 52191
			Reduce
		}
	}
}
