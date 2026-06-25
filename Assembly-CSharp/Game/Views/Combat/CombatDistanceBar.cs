using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using GameData.Domains.Character;
using GameData.Domains.Combat;
using UnityEngine;

namespace Game.Views.Combat
{
	// Token: 0x02000B05 RID: 2821
	public class CombatDistanceBar : MonoBehaviour
	{
		// Token: 0x17000F50 RID: 3920
		// (get) Token: 0x06008ACC RID: 35532 RVA: 0x00403E76 File Offset: 0x00402076
		// (set) Token: 0x06008ACD RID: 35533 RVA: 0x00403E7E File Offset: 0x0040207E
		public Func<float> BarScaleProvider { get; set; }

		// Token: 0x06008ACE RID: 35534 RVA: 0x00403E88 File Offset: 0x00402088
		public void SetInteract(bool canInteract)
		{
			if (canInteract)
			{
				this.UpdateInteractType();
			}
			else
			{
				this.UpdateInteractType(CombatDistanceBar.EInteractType.Invalid);
			}
		}

		// Token: 0x06008ACF RID: 35535 RVA: 0x00403EAC File Offset: 0x004020AC
		public void RefreshRange(CombatRangeText.EType rangeType, OuterAndInnerShorts range)
		{
			float scale = this.BarScaleProvider();
			bool flag = rangeType == CombatRangeText.EType.Self;
			if (flag)
			{
				this.selfRangeReverse.Refresh(range, scale);
			}
		}

		// Token: 0x06008AD0 RID: 35536 RVA: 0x00403EDC File Offset: 0x004020DC
		public void RefreshMark()
		{
			float scale = this.BarScaleProvider();
			this.selfDistanceBar.Refresh(scale);
			this.enemyDistanceBar.Refresh(scale);
		}

		// Token: 0x06008AD1 RID: 35537 RVA: 0x00403F10 File Offset: 0x00402110
		public void RefreshTarget(short selfTarget, short enemyTarget)
		{
			float scale = this.BarScaleProvider();
			this.selfDistanceTarget.Refresh(scale, selfTarget);
			this.enemyDistanceTarget.Refresh(scale, enemyTarget);
			this.selfDistancePreview.Refresh(scale, this._previewTarget);
		}

		// Token: 0x06008AD2 RID: 35538 RVA: 0x00403F5C File Offset: 0x0040215C
		public void RefreshPreview(short newPreviewTarget)
		{
			bool flag = newPreviewTarget == this._previewTarget;
			if (!flag)
			{
				this._previewTarget = newPreviewTarget;
				float scale = this.BarScaleProvider();
				this.selfDistancePreview.Refresh(scale, newPreviewTarget);
			}
		}

		// Token: 0x06008AD3 RID: 35539 RVA: 0x00403F9C File Offset: 0x0040219C
		public void OnPointerEnter(bool isAlly)
		{
			bool flag = this._interactType == CombatDistanceBar.EInteractType.Invalid;
			if (!flag)
			{
				this.UpdateInteractType(isAlly ? CombatDistanceBar.EInteractType.Self : CombatDistanceBar.EInteractType.Enemy);
			}
		}

		// Token: 0x06008AD4 RID: 35540 RVA: 0x00403FC8 File Offset: 0x004021C8
		public void OnPointerExit(bool isAlly)
		{
			bool flag = this._interactType == CombatDistanceBar.EInteractType.Invalid;
			if (!flag)
			{
				this.UpdateInteractType(CombatDistanceBar.EInteractType.None);
			}
		}

		// Token: 0x06008AD5 RID: 35541 RVA: 0x00403FF0 File Offset: 0x004021F0
		private void OnEnable()
		{
			bool flag = this._interactType == CombatDistanceBar.EInteractType.Invalid;
			if (!flag)
			{
				this.UpdateInteractType();
			}
		}

		// Token: 0x06008AD6 RID: 35542 RVA: 0x00404014 File Offset: 0x00402214
		private void OnDisable()
		{
			bool flag = this._interactType == CombatDistanceBar.EInteractType.Invalid;
			if (!flag)
			{
				this.UpdateInteractType(CombatDistanceBar.EInteractType.InvalidTemporary);
				this._mousePressedInBounds = false;
			}
		}

		// Token: 0x06008AD7 RID: 35543 RVA: 0x00404040 File Offset: 0x00402240
		private void Update()
		{
			short newPreviewTarget = this.CalcPreviewTarget();
			this.RefreshPreview(newPreviewTarget);
			bool flag = this._interactType != CombatDistanceBar.EInteractType.Self;
			if (!flag)
			{
				bool flag2 = this._previewTarget > 0 && Input.GetMouseButton(0) && this._mousePressedInBounds;
				if (flag2)
				{
					CombatDomainMethod.Call.SetTargetDistance(this._previewTarget);
				}
				else
				{
					bool mouseButtonDown = Input.GetMouseButtonDown(1);
					if (mouseButtonDown)
					{
						CombatDomainMethod.Call.ClearTargetDistance();
					}
				}
				bool mouseButtonDown2 = Input.GetMouseButtonDown(0);
				if (mouseButtonDown2)
				{
					this._mousePressedInBounds = (this._interactType == CombatDistanceBar.EInteractType.Self);
				}
				else
				{
					bool flag3 = !Input.GetMouseButton(0);
					if (flag3)
					{
						this._mousePressedInBounds = false;
					}
				}
			}
		}

		// Token: 0x06008AD8 RID: 35544 RVA: 0x004040E0 File Offset: 0x004022E0
		private short CalcPreviewTarget()
		{
			CanvasGroup selfBarCanvas = this.GetBarCanvas(true);
			bool flag = this._interactType != CombatDistanceBar.EInteractType.Self;
			short result;
			if (flag)
			{
				result = -1;
			}
			else
			{
				Camera uiCamera = UIManager.Instance.UiCamera;
				RectTransform selfBar = (RectTransform)selfBarCanvas.transform;
				Vector2 localPoint;
				bool flag2 = !RectTransformUtility.ScreenPointToLocalPointInRectangle(selfBar, Input.mousePosition, uiCamera, out localPoint);
				if (flag2)
				{
					result = -1;
				}
				else
				{
					float scale = this.BarScaleProvider();
					float distanceDelta = 5f * scale;
					bool flag3 = localPoint.x > distanceDelta * 20f;
					if (flag3)
					{
						result = -1;
					}
					else
					{
						int target = Mathf.RoundToInt(Mathf.Abs(Mathf.Min(localPoint.x, 0f) / distanceDelta)) + 20;
						result = (short)Mathf.Clamp(target, 20, 120);
					}
				}
			}
			return result;
		}

		// Token: 0x06008AD9 RID: 35545 RVA: 0x004041B0 File Offset: 0x004023B0
		private void UpdateInteractType()
		{
			bool flag = RectTransformUtility.RectangleContainsScreenPoint((RectTransform)this.GetBarCanvas(true).transform, Input.mousePosition);
			if (flag)
			{
				this.UpdateInteractType(CombatDistanceBar.EInteractType.Self);
			}
			else
			{
				this.UpdateInteractType(CombatDistanceBar.EInteractType.None);
			}
		}

		// Token: 0x06008ADA RID: 35546 RVA: 0x004041F4 File Offset: 0x004023F4
		private void UpdateInteractType(CombatDistanceBar.EInteractType interactType)
		{
			bool flag = interactType == this._interactType;
			if (!flag)
			{
				bool flag2 = interactType == CombatDistanceBar.EInteractType.Invalid || interactType == CombatDistanceBar.EInteractType.InvalidTemporary;
				if (flag2)
				{
					this.ClearBar();
				}
				else
				{
					bool flag3 = interactType == CombatDistanceBar.EInteractType.Self;
					if (flag3)
					{
						this.ShowBar(true);
					}
					bool flag4 = this._interactType == CombatDistanceBar.EInteractType.Self;
					if (flag4)
					{
						this.HideBar(true);
					}
				}
				this._interactType = interactType;
			}
		}

		// Token: 0x06008ADB RID: 35547 RVA: 0x00404258 File Offset: 0x00402458
		private void ShowBar(bool isAlly)
		{
			CanvasGroup canvas = this.GetBarCanvas(isAlly);
			canvas.DOKill(false);
			canvas.DOFade(1f, 0.3f).SetUpdate(true);
			bool flag = !SingletonObject.getInstance<GlobalSettings>().DistanceAttackRange;
			if (!flag)
			{
				CanvasGroup rangeCanvas = this.GetRangeCanvas(isAlly);
				rangeCanvas.DOKill(false);
				rangeCanvas.DOFade(1f, 0.3f).SetUpdate(true);
			}
		}

		// Token: 0x06008ADC RID: 35548 RVA: 0x004042C8 File Offset: 0x004024C8
		private void HideBar(bool isAlly)
		{
			CanvasGroup canvas = this.GetBarCanvas(isAlly);
			canvas.DOKill(false);
			canvas.DOFade(0f, 0.3f).SetUpdate(true);
			CanvasGroup rangeCanvas = this.GetRangeCanvas(isAlly);
			rangeCanvas.DOKill(false);
			rangeCanvas.DOFade(0f, 0.3f).SetUpdate(true);
		}

		// Token: 0x06008ADD RID: 35549 RVA: 0x00404324 File Offset: 0x00402524
		private void ClearBar()
		{
			this.GetBarCanvas(true).DOKill(false);
			this.GetBarCanvas(true).alpha = 0f;
			this.GetBarCanvas(false).DOKill(false);
			this.GetBarCanvas(false).alpha = 0f;
			this.GetRangeCanvas(true).DOKill(false);
			this.GetRangeCanvas(true).alpha = 0f;
			this.GetRangeCanvas(false).DOKill(false);
			this.GetRangeCanvas(false).alpha = 0f;
		}

		// Token: 0x06008ADE RID: 35550 RVA: 0x004043B4 File Offset: 0x004025B4
		private CanvasGroup GetBarCanvas(bool isAlly)
		{
			CombatDistanceMark bar = isAlly ? this.selfDistanceBar : this.enemyDistanceBar;
			return bar.GetComponent<CanvasGroup>();
		}

		// Token: 0x06008ADF RID: 35551 RVA: 0x004043E0 File Offset: 0x004025E0
		private CanvasGroup GetRangeCanvas(bool isAlly)
		{
			CombatDistanceRange range = isAlly ? this.selfRangeReverse : this.enemyRangeReverse;
			return range.GetComponent<CanvasGroup>();
		}

		// Token: 0x04006A74 RID: 27252
		[SerializeField]
		private CombatDistanceMark selfDistanceBar;

		// Token: 0x04006A75 RID: 27253
		[SerializeField]
		private CombatDistanceMark enemyDistanceBar;

		// Token: 0x04006A76 RID: 27254
		[SerializeField]
		private CombatDistanceTarget selfDistanceTarget;

		// Token: 0x04006A77 RID: 27255
		[SerializeField]
		private CombatDistanceTarget enemyDistanceTarget;

		// Token: 0x04006A78 RID: 27256
		[SerializeField]
		private CombatDistanceTarget selfDistancePreview;

		// Token: 0x04006A79 RID: 27257
		[SerializeField]
		private CombatDistanceRange selfRangeReverse;

		// Token: 0x04006A7A RID: 27258
		[SerializeField]
		private CombatDistanceRange enemyRangeReverse;

		// Token: 0x04006A7C RID: 27260
		private CombatDistanceBar.EInteractType _interactType = CombatDistanceBar.EInteractType.None;

		// Token: 0x04006A7D RID: 27261
		private short _previewTarget = -1;

		// Token: 0x04006A7E RID: 27262
		private bool _mousePressedInBounds = false;

		// Token: 0x020020CC RID: 8396
		public enum EInteractType
		{
			// Token: 0x0400D22F RID: 53807
			Invalid,
			// Token: 0x0400D230 RID: 53808
			InvalidTemporary,
			// Token: 0x0400D231 RID: 53809
			None,
			// Token: 0x0400D232 RID: 53810
			Self,
			// Token: 0x0400D233 RID: 53811
			Enemy
		}
	}
}
