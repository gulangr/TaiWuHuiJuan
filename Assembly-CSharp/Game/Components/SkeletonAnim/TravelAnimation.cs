using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using Game.Views.World;
using GameData.Domains.Character.Display;
using GameData.Domains.Item.Display;
using GameData.Domains.Taiwu;
using GameData.Serializer;
using GameData.Utilities;
using JetBrains.Annotations;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace Game.Components.SkeletonAnim
{
	// Token: 0x02000E9A RID: 3738
	public class TravelAnimation : MonoBehaviour
	{
		// Token: 0x17001396 RID: 5014
		// (get) Token: 0x0600AD66 RID: 44390 RVA: 0x004F1BF7 File Offset: 0x004EFDF7
		public short TaiwuCarrierId
		{
			get
			{
				return this.MapModel.TaiwuCarrier;
			}
		}

		// Token: 0x0600AD67 RID: 44391 RVA: 0x004F1C04 File Offset: 0x004EFE04
		public void Set(CharacterDisplayData charDisplayData, IEnumerable<ItemDisplayData> equipments, short carrierId, bool isMove = true)
		{
			this._charDisplayData = charDisplayData;
			this._equipments = equipments.ToList<ItemDisplayData>();
			this._carrierId = carrierId;
			this.PlayCarrierAnim(isMove);
		}

		// Token: 0x17001397 RID: 5015
		// (get) Token: 0x0600AD68 RID: 44392 RVA: 0x004F1C2C File Offset: 0x004EFE2C
		private bool CharIsKid
		{
			get
			{
				CharacterDisplayData charDisplayData = this._charDisplayData;
				short? num = (charDisplayData != null) ? new short?(charDisplayData.AvatarRelatedData.DisplayAge) : null;
				int? num2 = (num != null) ? new int?((int)num.GetValueOrDefault()) : null;
				int num3 = 16;
				return num2.GetValueOrDefault() < num3 & num2 != null;
			}
		}

		// Token: 0x17001398 RID: 5016
		// (get) Token: 0x0600AD69 RID: 44393 RVA: 0x004F1C95 File Offset: 0x004EFE95
		private WorldMapModel MapModel
		{
			get
			{
				return SingletonObject.getInstance<WorldMapModel>();
			}
		}

		// Token: 0x17001399 RID: 5017
		// (get) Token: 0x0600AD6A RID: 44394 RVA: 0x004F1C9C File Offset: 0x004EFE9C
		public TravelSkeletonItem TravelSkeleton
		{
			get
			{
				return (this._carrierId == short.MaxValue) ? Config.TravelSkeleton.DefValue.KidnappedCarrier : ViewPartWorldMap.GetSkeleton(this._carrierId);
			}
		}

		// Token: 0x0600AD6B RID: 44395 RVA: 0x004F1CBD File Offset: 0x004EFEBD
		public void SetScale(float scale, float x, float y)
		{
			this.selfScaleController.localScale = new Vector3(scale, scale, scale);
			this.selfScaleController.localPosition = new Vector3(x, y, 0f);
		}

		// Token: 0x0600AD6C RID: 44396 RVA: 0x004F1CEC File Offset: 0x004EFEEC
		public void SetDirection(bool isRight)
		{
			bool flag = this.maskWithOffset;
			if (flag)
			{
				this.maskWithOffset.localPosition = (isRight ? this.rightOffset : this.leftOffset);
			}
			base.transform.localScale = base.transform.localScale.SetX(Mathf.Abs(base.transform.localScale.x) * (float)(isRight ? 1 : -1));
		}

		// Token: 0x0600AD6D RID: 44397 RVA: 0x004F1D65 File Offset: 0x004EFF65
		public void PlayCarrierAnim(bool isMove)
		{
			this.PlayCarrierAnimCharacter(isMove);
			this.PlayCarrierAnimCarrier(isMove);
		}

		// Token: 0x0600AD6E RID: 44398 RVA: 0x004F1D78 File Offset: 0x004EFF78
		private void PlayCarrierAnimCharacter(bool isMove)
		{
			this.PlayCarrierAnimCharacterEquipments(this.characterSkeleton);
			string animName = isMove ? this.TravelSkeleton.Animation : this.TravelSkeleton.AnimationIdle;
			this.PlayCarrierAnimCharacterAnimation(this.characterSkeleton, animName);
			bool flag = this.characterSkeletonOutline != null;
			if (flag)
			{
				this.PlayCarrierAnimCharacterEquipments(this.characterSkeletonOutline);
				string animNameOutline = isMove ? this.TravelSkeleton.Animation : this.TravelSkeleton.AnimationIdle;
				this.PlayCarrierAnimCharacterAnimation(this.characterSkeletonOutline, animNameOutline);
			}
		}

		// Token: 0x0600AD6F RID: 44399 RVA: 0x004F1E08 File Offset: 0x004F0008
		private void PlayCarrierAnimCharacterAnimation(SkeletonGraphic charSkeleton, string animName)
		{
			string animPath = "RemakeResources/SpineAnimations/Character/TravelAnimations/" + animName;
			ResLoader.Load<RawAnimationAsset>(animPath, delegate(RawAnimationAsset rawAnimation)
			{
				Spine.Animation anim = rawAnimation.GetAnimation(charSkeleton.skeletonDataAsset);
				charSkeleton.AnimationState.SetAnimation(0, anim, true);
				charSkeleton.gameObject.SetActive(true);
			}, null, false);
		}

		// Token: 0x0600AD70 RID: 44400 RVA: 0x004F1E44 File Offset: 0x004F0044
		private void PlayCarrierAnimCharacterEquipments(SkeletonGraphic skeleton)
		{
			TaiwuDomainMethod.AsyncCall.RequestTaiwuEquipWithoutHideForSkeleton(null, delegate(int offset, RawDataPool pool)
			{
				List<ItemDisplayData> equips = new List<ItemDisplayData>();
				Serializer.Deserialize(pool, offset, ref equips);
				CombatAnimationUtils.UpdateSkeleton(skeleton, this._charDisplayData, equips);
			});
		}

		// Token: 0x0600AD71 RID: 44401 RVA: 0x004F1E7C File Offset: 0x004F007C
		private void PlayCarrierAnimCarrier(bool isMove)
		{
			this.carrierSkeleton.gameObject.SetActive(this.TravelSkeleton.AnyCarrier);
			SkeletonGraphic skeletonGraphic = this.carrierSkeletonOutline;
			if (skeletonGraphic != null)
			{
				skeletonGraphic.gameObject.SetActive(this.TravelSkeleton.AnyCarrier);
			}
			bool flag = !this.TravelSkeleton.AnyCarrier;
			if (!flag)
			{
				string carrierAnim = isMove ? this.TravelSkeleton.CarrierAnimation : this.TravelSkeleton.CarrierAnimationIdle;
				string carrierAnimPath = "RemakeResources/SpineAnimations/Carrier/" + this.TravelSkeleton.CarrierAnimationPath;
				ResLoader.Load<SkeletonDataAsset>(carrierAnimPath, delegate(SkeletonDataAsset animData)
				{
					this.carrierSkeleton.skeletonDataAsset = animData;
					this.carrierSkeleton.initialSkinName = this.TravelSkeleton.CarrierAnimationSkin;
					this.carrierSkeleton.Initialize(true);
					this.carrierSkeleton.transform.localScale = Vector3.one * (this.CharIsKid ? 0.85f : 1f);
					this.carrierSkeleton.AnimationState.SetAnimation(0, carrierAnim, true);
					this.carrierSkeleton.gameObject.SetActive(true);
					bool flag2 = this.carrierSkeletonOutline != null;
					if (flag2)
					{
						this.carrierSkeletonOutline.skeletonDataAsset = animData;
						this.carrierSkeletonOutline.initialSkinName = this.TravelSkeleton.CarrierAnimationSkin;
						this.carrierSkeletonOutline.Initialize(true);
						this.carrierSkeletonOutline.transform.localScale = Vector3.one * (this.CharIsKid ? 0.85f : 1f);
						this.carrierSkeletonOutline.AnimationState.SetAnimation(0, carrierAnim, true);
						this.carrierSkeletonOutline.gameObject.SetActive(true);
					}
				}, null, false);
			}
		}

		// Token: 0x040085E4 RID: 34276
		private CharacterDisplayData _charDisplayData;

		// Token: 0x040085E5 RID: 34277
		private List<ItemDisplayData> _equipments;

		// Token: 0x040085E6 RID: 34278
		private short _carrierId;

		// Token: 0x040085E7 RID: 34279
		[SerializeField]
		private SkeletonGraphic carrierSkeleton;

		// Token: 0x040085E8 RID: 34280
		[SerializeField]
		private SkeletonGraphic characterSkeleton;

		// Token: 0x040085E9 RID: 34281
		[SerializeField]
		private SkeletonGraphic subCharacterSkeleton;

		// Token: 0x040085EA RID: 34282
		[SerializeField]
		private SkeletonGraphic carrierSkeletonOutline;

		// Token: 0x040085EB RID: 34283
		[SerializeField]
		private SkeletonGraphic characterSkeletonOutline;

		// Token: 0x040085EC RID: 34284
		[SerializeField]
		private SkeletonGraphic subCharacterSkeletonOutline;

		// Token: 0x040085ED RID: 34285
		[CanBeNull]
		[SerializeField]
		private RectTransform selfScaleController;

		// Token: 0x040085EE RID: 34286
		[CanBeNull]
		[SerializeField]
		private RectTransform maskWithOffset;

		// Token: 0x040085EF RID: 34287
		[SerializeField]
		private Vector2 rightOffset = new Vector2(200f, 150f);

		// Token: 0x040085F0 RID: 34288
		[SerializeField]
		private Vector2 leftOffset = new Vector2(-200f, 150f);

		// Token: 0x040085F1 RID: 34289
		public const float MoveStepTime = 5f;
	}
}
