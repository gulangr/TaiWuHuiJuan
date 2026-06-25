using System;
using System.Collections;
using System.Collections.Generic;
using Config;
using Game.Views.Combat;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Combat;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Taiwu;
using GameData.Serializer;
using GameData.Utilities;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace Game.Components.SkeletonAnim
{
	// Token: 0x02000E99 RID: 3737
	public class CharacterAnimation : MonoBehaviour
	{
		// Token: 0x0600AD4C RID: 44364 RVA: 0x004F13C5 File Offset: 0x004EF5C5
		public void Set(CharacterDisplayData charDisplayData, List<ItemDisplayData> equipItems, bool isMove = true, List<sbyte> hideEquipSlots = null)
		{
			this._charDisplayData = charDisplayData;
			this._equipItems = equipItems;
			this._isAnimPaused = false;
			short currentWeaponId = this.CurrentWeaponId;
			CombatAnimationUtils.UpdateSkeleton(this.skeletonGraphic, this._charDisplayData, this._equipItems, hideEquipSlots);
			this.PlayIdle();
		}

		// Token: 0x0600AD4D RID: 44365 RVA: 0x004F1405 File Offset: 0x004EF605
		public void Set(CharacterDisplayData characterDisplayData)
		{
			TaiwuDomainMethod.AsyncCall.RequestTaiwuEquipWithoutHideForSkeleton(null, delegate(int offset, RawDataPool pool)
			{
				List<ItemDisplayData> equips = new List<ItemDisplayData>();
				Serializer.Deserialize(pool, offset, ref equips);
				CombatAnimationUtils.UpdateSkeleton(this.skeletonGraphic, this._charDisplayData, equips);
			});
		}

		// Token: 0x17001395 RID: 5013
		// (get) Token: 0x0600AD4E RID: 44366 RVA: 0x004F141C File Offset: 0x004EF61C
		// (set) Token: 0x0600AD4F RID: 44367 RVA: 0x004F1444 File Offset: 0x004EF644
		public short CurrentWeaponId
		{
			get
			{
				short value = this.GetCurWeaponId();
				CombatAnimationUtils.UpdateSkeletonWeapon(this.skeletonGraphic, (int)value);
				return value;
			}
			set
			{
				bool flag = this._currentWeaponId == value;
				if (!flag)
				{
					this._currentWeaponId = value;
					CombatAnimationUtils.UpdateSkeletonWeapon(this.skeletonGraphic, (int)value);
					this.PlayIdle();
				}
			}
		}

		// Token: 0x0600AD50 RID: 44368 RVA: 0x004F147C File Offset: 0x004EF67C
		private void OnEnable()
		{
			bool flag = this._pendingAnim != null;
			if (flag)
			{
				base.StartCoroutine(this._pendingAnim);
				this._pendingAnim = null;
			}
		}

		// Token: 0x0600AD51 RID: 44369 RVA: 0x004F14B0 File Offset: 0x004EF6B0
		private short GetCurWeaponId()
		{
			bool flag = this._currentWeaponId != 0;
			short result;
			if (flag)
			{
				result = this._currentWeaponId;
			}
			else
			{
				for (sbyte i = 0; i < 3; i += 1)
				{
					bool flag2 = this._equipItems[(int)i].Key != ItemKey.Invalid;
					if (flag2)
					{
						return this._equipItems[(int)i].Key.TemplateId;
					}
				}
				result = 0;
			}
			return result;
		}

		// Token: 0x0600AD52 RID: 44370 RVA: 0x004F1524 File Offset: 0x004EF724
		public void PauseAnimation()
		{
			bool isAnimPaused = this._isAnimPaused;
			if (!isAnimPaused)
			{
				this._isAnimPaused = true;
				this.skeletonGraphic.AnimationState.TimeScale = 0f;
				this._pendingAnim = null;
				bool flag = this._currentDelayCoroutine != null;
				if (flag)
				{
					base.StopCoroutine(this._currentDelayCoroutine);
					this._currentDelayCoroutine = null;
				}
			}
		}

		// Token: 0x0600AD53 RID: 44371 RVA: 0x004F1588 File Offset: 0x004EF788
		public void ResumeAnimation()
		{
			bool flag = !this._isAnimPaused || this.skeletonGraphic == null;
			if (!flag)
			{
				this._isAnimPaused = false;
				this.skeletonGraphic.AnimationState.TimeScale = 1f;
				bool flag2 = this._pendingAnim != null;
				if (flag2)
				{
					this._currentDelayCoroutine = base.StartCoroutine(this._pendingAnim);
					this._pendingAnim = null;
				}
				bool flag3 = this._currentDelayCoroutine == null;
				if (flag3)
				{
					this.PlayIdle();
				}
			}
		}

		// Token: 0x0600AD54 RID: 44372 RVA: 0x004F160C File Offset: 0x004EF80C
		public bool PlayIdle()
		{
			bool isAnimPaused = this._isAnimPaused;
			bool result;
			if (isAnimPaused)
			{
				result = false;
			}
			else
			{
				this._currentCharacterAnimType = CharacterAnimation.ECharacterAnimType.Idle;
				Spine.Animation anim = this.GetAnimation("C_000");
				bool flag = anim == null;
				if (flag)
				{
					result = false;
				}
				else
				{
					TrackEntry entry = this.skeletonGraphic.AnimationState.SetAnimation(0, anim, true);
					this.DelayThenRandomNext(entry, 5f);
					result = true;
				}
			}
			return result;
		}

		// Token: 0x0600AD55 RID: 44373 RVA: 0x004F1674 File Offset: 0x004EF874
		private void PlayRandomAnimation()
		{
			bool isAnimPaused = this._isAnimPaused;
			if (!isAnimPaused)
			{
				switch (GameApp.RandomRange(0, Enum.GetNames(typeof(CharacterAnimation.ECharacterAnimType)).Length))
				{
				case 0:
					this.PlayIdle();
					break;
				case 1:
				{
					this._currentCharacterAnimType = CharacterAnimation.ECharacterAnimType.Walk;
					Spine.Animation animation = this.GetAnimation(this.GetWalkAnim(this.CurrentWeaponId));
					bool flag = animation == null;
					if (!flag)
					{
						TrackEntry walkEntry = this.skeletonGraphic.AnimationState.SetAnimation(0, animation, true);
						this.DelayThenRandomNext(walkEntry, 3f);
					}
					break;
				}
				case 2:
				{
					Spine.Animation animation = this.GetAnimation(this.GetFastWalkAnim(this.CurrentWeaponId));
					bool flag2 = animation == null;
					if (!flag2)
					{
						TrackEntry fwEntry = this.skeletonGraphic.AnimationState.SetAnimation(0, animation, true);
						this._currentCharacterAnimType = CharacterAnimation.ECharacterAnimType.FastWalk;
						this.DelayThenRandomNext(fwEntry, 3f);
					}
					break;
				}
				case 3:
				{
					Spine.Animation animation = this.GetAnimation(this.GetJumpAnim());
					bool flag3 = animation == null;
					if (!flag3)
					{
						this._currentCharacterAnimType = CharacterAnimation.ECharacterAnimType.JumpMove;
						TrackEntry jumpEntry = this.skeletonGraphic.AnimationState.SetAnimation(0, animation, false);
						jumpEntry.Complete += delegate(TrackEntry _)
						{
							this.PlayRandomAnimation();
						};
					}
					break;
				}
				case 4:
					this.PlayAttackSequence(3);
					break;
				case 5:
				{
					Spine.Animation animation = this.GetAnimation(this.GetAvoidAnis(this.CurrentWeaponId));
					bool flag4 = animation == null;
					if (!flag4)
					{
						this._currentCharacterAnimType = CharacterAnimation.ECharacterAnimType.Avoid;
						TrackEntry avoidEntry = this.skeletonGraphic.AnimationState.SetAnimation(0, animation, false);
						avoidEntry.Complete += delegate(TrackEntry _)
						{
							this.PlayRandomAnimation();
						};
					}
					break;
				}
				}
			}
		}

		// Token: 0x0600AD56 RID: 44374 RVA: 0x004F182C File Offset: 0x004EFA2C
		private void PlayAttackSequence(int count = 3)
		{
			bool flag = count <= 0;
			if (flag)
			{
				this.PlayRandomAnimation();
			}
			else
			{
				this._currentCharacterAnimType = CharacterAnimation.ECharacterAnimType.Attack;
				Spine.Animation animation = this.GetAnimation(this.GetAttackAnim(this.CurrentWeaponId));
				bool flag2 = animation == null;
				if (!flag2)
				{
					TrackEntry attackEntry = this.skeletonGraphic.AnimationState.SetAnimation(0, animation, false);
					attackEntry.Complete += delegate(TrackEntry _)
					{
						this.PlayAttackSequence(count - 1);
					};
				}
			}
		}

		// Token: 0x0600AD57 RID: 44375 RVA: 0x004F18B8 File Offset: 0x004EFAB8
		private void DelayThenRandomNext(TrackEntry entry, float delay)
		{
			bool activeInHierarchy = base.gameObject.activeInHierarchy;
			if (activeInHierarchy)
			{
				this._currentDelayCoroutine = base.StartCoroutine(this.DelayThenRandomNextInternal(entry, delay));
			}
			else
			{
				this._pendingAnim = this.DelayThenRandomNextInternal(entry, delay);
			}
		}

		// Token: 0x0600AD58 RID: 44376 RVA: 0x004F18F9 File Offset: 0x004EFAF9
		private IEnumerator DelayThenRandomNextInternal(TrackEntry entry, float delay)
		{
			yield return new WaitForSecondsRealtime(delay);
			bool isAnimPaused = this._isAnimPaused;
			if (isAnimPaused)
			{
				this._pendingAnim = null;
				this._currentDelayCoroutine = null;
				yield break;
			}
			bool flag = this.skeletonGraphic.AnimationState.GetCurrent(0) == entry;
			if (flag)
			{
				this.PlayRandomAnimation();
			}
			this._pendingAnim = null;
			this._currentDelayCoroutine = null;
			yield break;
		}

		// Token: 0x0600AD59 RID: 44377 RVA: 0x004F1918 File Offset: 0x004EFB18
		public void InterruptAndPlayRandom()
		{
			bool isAnimPaused = this._isAnimPaused;
			if (!isAnimPaused)
			{
				bool flag = this._currentCharacterAnimType == CharacterAnimation.ECharacterAnimType.JumpMove;
				if (!flag)
				{
					TrackEntry currentEntry = this.skeletonGraphic.AnimationState.GetCurrent(0);
					bool flag2 = currentEntry != null;
					if (flag2)
					{
						this.skeletonGraphic.AnimationState.Apply(this.skeletonGraphic.Skeleton);
					}
					this._pendingAnim = null;
					bool flag3 = this._currentDelayCoroutine != null;
					if (flag3)
					{
						base.StopCoroutine(this._currentDelayCoroutine);
						this._currentDelayCoroutine = null;
					}
					this.PlayRandomAnimation();
				}
			}
		}

		// Token: 0x0600AD5A RID: 44378 RVA: 0x004F19AC File Offset: 0x004EFBAC
		private string GetAvoidAnis(short id)
		{
			return Weapon.Instance[id].AvoidAnis.GetRandom<string>();
		}

		// Token: 0x0600AD5B RID: 44379 RVA: 0x004F19C4 File Offset: 0x004EFBC4
		private string GetAttackAnim(short id)
		{
			sbyte action = Weapon.Instance[id].WeaponAction;
			sbyte trick = Weapon.Instance[id].Tricks.GetRandom<sbyte>();
			return string.Format("{0}_{1}", Config.TrickType.Instance[trick].AttackAnimations[(int)action], GameApp.RandomRange(0, 6));
		}

		// Token: 0x0600AD5C RID: 44380 RVA: 0x004F1A25 File Offset: 0x004EFC25
		private string GetWalkAnim(short id)
		{
			return (id < 0) ? CharacterAnimation._walkAnims.GetRandom<string>() : ((GameApp.RandomRange(0, 2) == 0) ? Weapon.Instance[id].ForwardAni : Weapon.Instance[id].BackwardAni);
		}

		// Token: 0x0600AD5D RID: 44381 RVA: 0x004F1A62 File Offset: 0x004EFC62
		private string GetFastWalkAnim(short id)
		{
			return (id < 0) ? CharacterAnimation._fastWalkAnims.GetRandom<string>() : ((GameApp.RandomRange(0, 2) == 0) ? Weapon.Instance[id].FastForwardAni : Weapon.Instance[id].FastBackwardAni);
		}

		// Token: 0x0600AD5E RID: 44382 RVA: 0x004F1A9F File Offset: 0x004EFC9F
		private string GetJumpAnim()
		{
			return CharacterAnimation._jumpMoveAnims.GetRandom<string>();
		}

		// Token: 0x0600AD5F RID: 44383 RVA: 0x004F1AAB File Offset: 0x004EFCAB
		private Spine.Animation GetAnimation(string animName)
		{
			return (animName != null && this.CanShowCharacterPreview()) ? UIElement.Combat.UiBaseAs<ViewCombat>().GetCommonAnim(this.skeletonGraphic, animName) : null;
		}

		// Token: 0x0600AD60 RID: 44384 RVA: 0x004F1AD4 File Offset: 0x004EFCD4
		public bool CanShowCharacterPreview()
		{
			bool flag = this._charDisplayData == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				short charTemplateId = this._charDisplayData.TemplateId;
				bool isBoss = ViewCombat.CharId2BossId.ContainsKey(charTemplateId);
				bool isAnimal = GameData.Domains.Combat.SharedConstValue.CharId2AnimalId.ContainsKey(charTemplateId);
				bool isBaby = AgeGroup.GetAgeGroup(this._charDisplayData.PhysiologicalAge) == 0;
				result = (!isBoss && !isAnimal && !isBaby);
			}
			return result;
		}

		// Token: 0x040085D3 RID: 34259
		private CharacterDisplayData _charDisplayData;

		// Token: 0x040085D4 RID: 34260
		private List<ItemDisplayData> _equipItems;

		// Token: 0x040085D5 RID: 34261
		private short _currentWeaponId;

		// Token: 0x040085D6 RID: 34262
		[SerializeField]
		private SkeletonGraphic skeletonGraphic;

		// Token: 0x040085D7 RID: 34263
		private CharacterAnimation.ECharacterAnimType _currentCharacterAnimType;

		// Token: 0x040085D8 RID: 34264
		private bool _isAnimPaused = true;

		// Token: 0x040085D9 RID: 34265
		private const float idleDuration = 5f;

		// Token: 0x040085DA RID: 34266
		private const float walkDuration = 3f;

		// Token: 0x040085DB RID: 34267
		private const float fastWalkDuration = 3f;

		// Token: 0x040085DC RID: 34268
		private const sbyte attackCount = 3;

		// Token: 0x040085DD RID: 34269
		private Coroutine _currentDelayCoroutine;

		// Token: 0x040085DE RID: 34270
		private IEnumerator _pendingAnim;

		// Token: 0x040085DF RID: 34271
		private const string _idleAnimName = "C_000";

		// Token: 0x040085E0 RID: 34272
		private const sbyte _pursueAttackCount = 6;

		// Token: 0x040085E1 RID: 34273
		private static readonly string[] _walkAnims = new string[]
		{
			"M_001",
			"M_002"
		};

		// Token: 0x040085E2 RID: 34274
		private static readonly string[] _fastWalkAnims = new string[]
		{
			"MR_001",
			"MR_002"
		};

		// Token: 0x040085E3 RID: 34275
		private static readonly string[] _jumpMoveAnims = new string[]
		{
			"M_003_fly",
			"M_004_fly"
		};

		// Token: 0x0200250D RID: 9485
		private enum ECharacterAnimType
		{
			// Token: 0x0400E683 RID: 59011
			Idle,
			// Token: 0x0400E684 RID: 59012
			Walk,
			// Token: 0x0400E685 RID: 59013
			FastWalk,
			// Token: 0x0400E686 RID: 59014
			JumpMove,
			// Token: 0x0400E687 RID: 59015
			Attack,
			// Token: 0x0400E688 RID: 59016
			Avoid
		}
	}
}
