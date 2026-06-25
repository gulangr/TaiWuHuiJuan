using System;
using Coffee.UIExtensions;
using DG.Tweening;
using Game.Views.Combat;
using UnityEngine;

namespace Game.Views.SectInteract.Wudang
{
	// Token: 0x020009DA RID: 2522
	public class DefendHeavenlyTreeFeedResourceItem : MonoBehaviour
	{
		// Token: 0x06007B5C RID: 31580 RVA: 0x00394C20 File Offset: 0x00392E20
		public void Show(sbyte resourceType, int amount)
		{
			bool flag = this._originPos == Vector3.zero;
			if (flag)
			{
				this._originPos = base.transform.position;
			}
			string spName = CombatDrops.GetIconName(amount, resourceType, true);
			this.imageIcon.SetSprite(spName, false, null);
			this.imageIcon.SetAlpha(1f);
			base.transform.position = this._originPos;
			bool activeSelf = base.gameObject.activeSelf;
			if (!activeSelf)
			{
				base.gameObject.SetActive(true);
				this.effectIdle.Play();
				this.effectDisappear.Clear();
			}
		}

		// Token: 0x06007B5D RID: 31581 RVA: 0x00394CC4 File Offset: 0x00392EC4
		public void Hide()
		{
			base.gameObject.SetActive(false);
		}

		// Token: 0x06007B5E RID: 31582 RVA: 0x00394CD4 File Offset: 0x00392ED4
		public float PlayEffectDisappear(Vector3 targetPos)
		{
			base.transform.DOKill(false);
			Sequence sequence = DOTween.Sequence().AppendCallback(delegate
			{
				this.transform.DOMove(targetPos, 0.4f, false);
			}).AppendInterval(0.2f).AppendCallback(delegate
			{
				this.effectIdle.Clear();
				this.effectDisappear.Play();
				this.imageIcon.DOFade(0f, 0.2f);
			}).AppendInterval(0.5f).AppendCallback(delegate
			{
				this.Hide();
			}).SetTarget(base.transform);
			return sequence.Duration(true);
		}

		// Token: 0x04005DA7 RID: 23975
		[SerializeField]
		private CImage imageIcon;

		// Token: 0x04005DA8 RID: 23976
		[SerializeField]
		private UIParticle effectIdle;

		// Token: 0x04005DA9 RID: 23977
		[SerializeField]
		private UIParticle effectDisappear;

		// Token: 0x04005DAA RID: 23978
		private Vector3 _originPos;
	}
}
