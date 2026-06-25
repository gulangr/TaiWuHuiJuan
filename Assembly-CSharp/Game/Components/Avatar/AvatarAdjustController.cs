using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using GameData.Domains.Character;
using GameData.Domains.Character.AvatarSystem;
using GameData.Domains.Character.AvatarSystem.AvatarRes;
using UnityEngine;

namespace Game.Components.Avatar
{
	// Token: 0x02000F6F RID: 3951
	public class AvatarAdjustController : MonoBehaviour
	{
		// Token: 0x17001490 RID: 5264
		// (get) Token: 0x0600B515 RID: 46357 RVA: 0x0052813B File Offset: 0x0052633B
		private sbyte DisplayGender
		{
			get
			{
				return this._transGender ? Gender.Flip(this._gender) : this._gender;
			}
		}

		// Token: 0x17001491 RID: 5265
		// (get) Token: 0x0600B516 RID: 46358 RVA: 0x00528158 File Offset: 0x00526358
		public AvatarGroup AvatarGroup
		{
			get
			{
				return AvatarAdjustController._allAvatarGroups[(int)this._curAvatarGroupIndex][(int)(this.DisplayGender % 2)];
			}
		}

		// Token: 0x17001492 RID: 5266
		// (get) Token: 0x0600B517 RID: 46359 RVA: 0x00528173 File Offset: 0x00526373
		// (set) Token: 0x0600B518 RID: 46360 RVA: 0x0052817B File Offset: 0x0052637B
		public sbyte BornMonth { get; private set; }

		// Token: 0x0600B519 RID: 46361 RVA: 0x00528184 File Offset: 0x00526384
		public void TryApplyCustomHandlersFromArgumentBox(ArgumentBox box)
		{
			bool flag = null != this.AvatarAdjustItemBodyType;
			if (flag)
			{
				this.AvatarAdjustItemBodyType.CustomAssetsFilterHandler = null;
			}
			bool flag2 = null != this.AvatarAdjustItemSkinColor;
			if (flag2)
			{
				this.AvatarAdjustItemSkinColor.CustomAssetsFilterHandler = null;
			}
			bool flag3 = null != this.AvatarAdjustItemFrontHair;
			if (flag3)
			{
				this.AvatarAdjustItemFrontHair.CustomAssetsFilterHandler = null;
			}
			bool flag4 = null != this.AvatarAdjustItemBackHair;
			if (flag4)
			{
				this.AvatarAdjustItemBackHair.CustomAssetsFilterHandler = null;
			}
			bool flag5 = null != this.AvatarAdjustItemEyeBrows;
			if (flag5)
			{
				this.AvatarAdjustItemEyeBrows.CustomAssetsFilterHandler = null;
			}
			bool flag6 = null != this.AvatarAdjustItemEyes;
			if (flag6)
			{
				this.AvatarAdjustItemEyes.CustomAssetsFilterHandler = null;
			}
			bool flag7 = null != this.AvatarAdjustItemNose;
			if (flag7)
			{
				this.AvatarAdjustItemNose.CustomAssetsFilterHandler = null;
			}
			bool flag8 = null != this.AvatarAdjustItemMouth;
			if (flag8)
			{
				this.AvatarAdjustItemMouth.CustomAssetsFilterHandler = null;
			}
			bool flag9 = null != this.AvatarAdjustItemBeard1;
			if (flag9)
			{
				this.AvatarAdjustItemBeard1.CustomAssetsFilterHandler = null;
			}
			bool flag10 = null != this.AvatarAdjustItemBeard2;
			if (flag10)
			{
				this.AvatarAdjustItemBeard2.CustomAssetsFilterHandler = null;
			}
			bool flag11 = null != this.AvatarAdjustItemFeature1;
			if (flag11)
			{
				this.AvatarAdjustItemFeature1.CustomAssetsFilterHandler = null;
			}
			bool flag12 = null != this.AvatarAdjustItemFeature2;
			if (flag12)
			{
				this.AvatarAdjustItemFeature2.CustomAssetsFilterHandler = null;
			}
			bool flag13 = null != this.AvatarAdjustItemCloth;
			if (flag13)
			{
				this.AvatarAdjustItemCloth.CustomAssetsFilterHandler = null;
			}
			bool flag14 = box == null;
			if (!flag14)
			{
				Func<sbyte, bool> handler0;
				bool flag15 = null != this.AvatarAdjustItemBodyType && box.Get<Func<sbyte, bool>>("BodyTypeFilter", out handler0);
				if (flag15)
				{
					this.AvatarAdjustItemBodyType.CustomAssetsFilterHandler = handler0;
				}
				Func<List<ValueTuple<byte, Color>>, List<ValueTuple<byte, Color>>> handler;
				bool flag16 = null != this.AvatarAdjustItemSkinColor && box.Get<Func<List<ValueTuple<byte, Color>>, List<ValueTuple<byte, Color>>>>("SkinColorFilter", out handler);
				if (flag16)
				{
					this.AvatarAdjustItemSkinColor.CustomAssetsFilterHandler = handler;
				}
				bool flag17 = null != this.AvatarAdjustItemFrontHair;
				if (flag17)
				{
					Func<List<HairRes>, List<HairRes>> handler2;
					bool flag18 = box.Get<Func<List<HairRes>, List<HairRes>>>("FrontHairFilter", out handler2);
					if (flag18)
					{
						this.AvatarAdjustItemFrontHair.CustomAssetsFilterHandler = handler2;
					}
					Func<bool> predicate;
					bool flag19 = null != this.AvatarAdjustItemFrontHair.ShaveBaldToggle && box.Get<Func<bool>>("CanShaveHairBald", out predicate);
					if (flag19)
					{
						this.AvatarAdjustItemFrontHair.ShaveBaldToggle.interactable = predicate();
					}
				}
				bool flag20 = null != this.AvatarAdjustItemBackHair;
				if (flag20)
				{
					Func<List<HairRes>, List<HairRes>> handler3;
					bool flag21 = box.Get<Func<List<HairRes>, List<HairRes>>>("BackHairFilter", out handler3);
					if (flag21)
					{
						this.AvatarAdjustItemBackHair.CustomAssetsFilterHandler = handler3;
					}
					Func<bool> predicate2;
					bool flag22 = null != this.AvatarAdjustItemBackHair.ShaveBaldToggle && box.Get<Func<bool>>("CanShaveHairBald", out predicate2);
					if (flag22)
					{
						this.AvatarAdjustItemBackHair.ShaveBaldToggle.interactable = predicate2();
					}
				}
				bool flag23 = null != this.AvatarAdjustItemEyeBrows;
				if (flag23)
				{
					Func<List<AvatarAsset>, List<AvatarAsset>> handler4;
					bool flag24 = box.Get<Func<List<AvatarAsset>, List<AvatarAsset>>>("EyeBrowFilter", out handler4);
					if (flag24)
					{
						this.AvatarAdjustItemEyeBrows.CustomAssetsFilterHandler = handler4;
					}
					Func<bool> predicate3;
					bool flag25 = null != this.AvatarAdjustItemEyeBrows.ShaveBaldToggle && box.Get<Func<bool>>("CanShaveEyebrowBald", out predicate3);
					if (flag25)
					{
						this.AvatarAdjustItemEyeBrows.ShaveBaldToggle.interactable = predicate3();
					}
				}
				Func<List<EyeRes>, List<EyeRes>> handler5;
				bool flag26 = null != this.AvatarAdjustItemEyes && box.Get<Func<List<EyeRes>, List<EyeRes>>>("EyesFilter", out handler5);
				if (flag26)
				{
					this.AvatarAdjustItemEyes.CustomAssetsFilterHandler = handler5;
				}
				Func<List<AvatarAsset>, List<AvatarAsset>> handler6;
				bool flag27 = null != this.AvatarAdjustItemNose && box.Get<Func<List<AvatarAsset>, List<AvatarAsset>>>("NoseFilter", out handler6);
				if (flag27)
				{
					this.AvatarAdjustItemNose.CustomAssetsFilterHandler = handler6;
				}
				Func<List<MouthRes>, List<MouthRes>> handler7;
				bool flag28 = null != this.AvatarAdjustItemMouth && box.Get<Func<List<MouthRes>, List<MouthRes>>>("MouthFilter", out handler7);
				if (flag28)
				{
					this.AvatarAdjustItemMouth.CustomAssetsFilterHandler = handler7;
				}
				bool flag29 = null != this.AvatarAdjustItemBeard1;
				if (flag29)
				{
					Func<List<AvatarAsset>, List<AvatarAsset>> handler8;
					bool flag30 = box.Get<Func<List<AvatarAsset>, List<AvatarAsset>>>("Beard1Filter", out handler8);
					if (flag30)
					{
						this.AvatarAdjustItemBeard1.CustomAssetsFilterHandler = handler8;
					}
					Func<bool> predicate4;
					bool flag31 = null != this.AvatarAdjustItemBeard1.ShaveBaldToggle && box.Get<Func<bool>>("CanShaveBeard1Bald", out predicate4);
					if (flag31)
					{
						this.AvatarAdjustItemBeard1.ShaveBaldToggle.interactable = predicate4();
					}
				}
				bool flag32 = null != this.AvatarAdjustItemBeard2;
				if (flag32)
				{
					Func<List<AvatarAsset>, List<AvatarAsset>> handler9;
					bool flag33 = box.Get<Func<List<AvatarAsset>, List<AvatarAsset>>>("Beard2Filter", out handler9);
					if (flag33)
					{
						this.AvatarAdjustItemBeard2.CustomAssetsFilterHandler = handler9;
					}
					Func<bool> predicate5;
					bool flag34 = null != this.AvatarAdjustItemBeard2.ShaveBaldToggle && box.Get<Func<bool>>("CanShaveBeard2Bald", out predicate5);
					if (flag34)
					{
						this.AvatarAdjustItemBeard2.ShaveBaldToggle.interactable = predicate5();
					}
				}
				Func<List<AvatarAsset>, List<AvatarAsset>> handler10;
				bool flag35 = null != this.AvatarAdjustItemFeature1 && box.Get<Func<List<AvatarAsset>, List<AvatarAsset>>>("Feature1Filter", out handler10);
				if (flag35)
				{
					this.AvatarAdjustItemFeature1.CustomAssetsFilterHandler = handler10;
				}
				Func<List<AvatarAsset>, List<AvatarAsset>> handler11;
				bool flag36 = null != this.AvatarAdjustItemFeature2 && box.Get<Func<List<AvatarAsset>, List<AvatarAsset>>>("Feature2Filter", out handler11);
				if (flag36)
				{
					this.AvatarAdjustItemFeature2.CustomAssetsFilterHandler = handler11;
				}
				Func<List<BodyRes>, List<BodyRes>> handler12;
				bool flag37 = null != this.AvatarAdjustItemCloth && box.Get<Func<List<BodyRes>, List<BodyRes>>>("ClothFilter", out handler12);
				if (flag37)
				{
					this.AvatarAdjustItemCloth.CustomAssetsFilterHandler = handler12;
				}
			}
		}

		// Token: 0x0600B51A RID: 46362 RVA: 0x005286F4 File Offset: 0x005268F4
		public void Init(ref AvatarData data, sbyte gender, short age, Avatar[] avatars, bool autoUpdateGrowable = true, bool transGender = false)
		{
			AvatarAdjustController.InitGroupColors();
			AvatarAdjustController.InitAllAvatarGroups();
			this._adjustItemList = new List<AvatarAdjustItemBase>();
			bool flag = null != this.AvatarAdjustItemGender;
			if (flag)
			{
				this._adjustItemList.Add(this.AvatarAdjustItemGender);
			}
			bool flag2 = null != this.AvatarAdjustItemBasic;
			if (flag2)
			{
				this._adjustItemList.Add(this.AvatarAdjustItemBasic);
			}
			bool flag3 = null != this.AvatarAdjustItemBodyType;
			if (flag3)
			{
				this._adjustItemList.Add(this.AvatarAdjustItemBodyType);
			}
			bool flag4 = null != this.AvatarAdjustItemSkinColor;
			if (flag4)
			{
				this._adjustItemList.Add(this.AvatarAdjustItemSkinColor);
			}
			bool flag5 = null != this.AvatarAdjustItemFrontHair;
			if (flag5)
			{
				this._adjustItemList.Add(this.AvatarAdjustItemFrontHair);
			}
			bool flag6 = null != this.AvatarAdjustItemBackHair;
			if (flag6)
			{
				this._adjustItemList.Add(this.AvatarAdjustItemBackHair);
			}
			bool flag7 = null != this.AvatarAdjustItemEyeBrows;
			if (flag7)
			{
				this._adjustItemList.Add(this.AvatarAdjustItemEyeBrows);
			}
			bool flag8 = null != this.AvatarAdjustItemEyes;
			if (flag8)
			{
				this._adjustItemList.Add(this.AvatarAdjustItemEyes);
			}
			bool flag9 = null != this.AvatarAdjustItemNose;
			if (flag9)
			{
				this._adjustItemList.Add(this.AvatarAdjustItemNose);
			}
			bool flag10 = null != this.AvatarAdjustItemMouth;
			if (flag10)
			{
				this._adjustItemList.Add(this.AvatarAdjustItemMouth);
			}
			bool flag11 = null != this.AvatarAdjustItemBeard1;
			if (flag11)
			{
				this._adjustItemList.Add(this.AvatarAdjustItemBeard1);
			}
			bool flag12 = null != this.AvatarAdjustItemBeard2;
			if (flag12)
			{
				this._adjustItemList.Add(this.AvatarAdjustItemBeard2);
			}
			bool flag13 = null != this.AvatarAdjustItemFeature1;
			if (flag13)
			{
				this._adjustItemList.Add(this.AvatarAdjustItemFeature1);
			}
			bool flag14 = null != this.AvatarAdjustItemFeature2;
			if (flag14)
			{
				this._adjustItemList.Add(this.AvatarAdjustItemFeature2);
			}
			bool flag15 = null != this.AvatarAdjustItemCloth;
			if (flag15)
			{
				this._adjustItemList.Add(this.AvatarAdjustItemCloth);
			}
			this._autoUpdateGrowable = autoUpdateGrowable;
			this._gender = gender;
			this._age = age;
			bool flag16 = data == null;
			if (flag16)
			{
				ValueTuple<AvatarData, short> tuple = this.AvatarGroup.GetRandomAvatar(GameApp.Random, true, true, true, false);
				tuple.Item1.ClothDisplayId = tuple.Item2;
				short attraction = (short)Utils_Random.SkewDistribute(450f, 152.2f, 1.6666666f, 0, 900);
				tuple.Item1.AdjustToBaseCharm(GameApp.Random, attraction);
				data = tuple.Item1;
			}
			this.AvatarData = data;
			this._curAvatarGroupIndex = (sbyte)this.GetGroupIndexByAvatarId(data.AvatarId).Item1;
			bool flag17 = data.Gender != gender && !transGender;
			if (flag17)
			{
				this.SetGender(this._gender);
			}
			this._transGender = transGender;
			this._adjustItemList.ForEach(delegate(AvatarAdjustItemBase e)
			{
				e.SetController(this);
			});
			this.UpdateShowAbility();
			bool flag18 = this.AvatarList == null;
			if (flag18)
			{
				this.AvatarList = new List<Avatar>();
			}
			else
			{
				this.AvatarList.Clear();
			}
			this.AvatarList.AddRange(avatars);
			this.AvatarList.ForEach(delegate(Avatar e)
			{
				e.Refresh(this.AvatarData, this._age);
			});
			Action onAvatarUpdate = this.OnAvatarUpdate;
			if (onAvatarUpdate != null)
			{
				onAvatarUpdate();
			}
			this.UpdateShaveBaldToggles();
			this.UpdateItemsVisible();
			this.UpdateBackHairItems();
			bool flag19 = null != this.AvatarAdjustItemFrontHair;
			if (flag19)
			{
				this.AvatarAdjustItemFrontHair.OnFrontHairIdChange = new Action(this.UpdateBackHairItems);
			}
			this.InitFeatureToggle();
		}

		// Token: 0x0600B51B RID: 46363 RVA: 0x00528AC8 File Offset: 0x00526CC8
		private void InitFeatureToggle()
		{
			bool flag = this.AvatarAdjustItemFeature1 != null && this.AvatarData != null;
			if (flag)
			{
				this.AvatarAdjustItemFeature1.SetFeatureToggle(this.AvatarData.Feature1MirrorType);
			}
			bool flag2 = this.AvatarAdjustItemFeature2 != null && this.AvatarData != null;
			if (flag2)
			{
				this.AvatarAdjustItemFeature2.SetFeatureToggle(this.AvatarData.Feature2MirrorType);
			}
		}

		// Token: 0x0600B51C RID: 46364 RVA: 0x00528B40 File Offset: 0x00526D40
		public void SetBornMonth(sbyte month)
		{
			this.BornMonth = month;
		}

		// Token: 0x0600B51D RID: 46365 RVA: 0x00528B4C File Offset: 0x00526D4C
		public void SetAvatarData(AvatarData data)
		{
			this.AvatarData.Copy(data);
			this.UpdateShowAbility();
			this.UpdateItemsVisible();
			this.UpdateBackHairItems();
			this._adjustItemList.ForEach(delegate(AvatarAdjustItemBase e)
			{
				bool flag = !e.Closed;
				if (flag)
				{
					e.OnOpen(false);
				}
			});
			this.RefreshAllAvatar();
		}

		// Token: 0x0600B51E RID: 46366 RVA: 0x00528BB0 File Offset: 0x00526DB0
		public sbyte RandomAvatar()
		{
			this._curAvatarGroupIndex = (sbyte)GameApp.RandomRange(0, AvatarAdjustController._allAvatarGroups.Count);
			ValueTuple<AvatarData, short> tuple = this.AvatarGroup.GetRandomAvatar(GameApp.Random, true, true, true, false);
			tuple.Item1.ClothDisplayId = tuple.Item2;
			short attraction = (short)Utils_Random.SkewDistribute(450f, 152.2f, 1.6666666f, 0, 900);
			tuple.Item1.AdjustToBaseCharm(GameApp.Random, attraction);
			BodyRes bodyRes = this.AvatarGroup.BodyRes.Find((BodyRes e) => e.Id == 1);
			List<AvatarAsset> clothParts = (bodyRes != null) ? bodyRes.ClothParts : null;
			bool flag = clothParts != null && clothParts.Count <= 0;
			if (flag)
			{
				tuple.Item1.ClothPartId = 0;
			}
			else
			{
				tuple.Item1.ClothPartId = (byte)((clothParts != null) ? clothParts[0].Id : 0);
			}
			this.SetAvatarData(tuple.Item1);
			this._adjustItemList.ForEach(delegate(AvatarAdjustItemBase e)
			{
				e.OnQuickAdjustTriggered(0);
			});
			return tuple.Item1.GetBodyType();
		}

		// Token: 0x17001493 RID: 5267
		// (get) Token: 0x0600B51F RID: 46367 RVA: 0x00528CF4 File Offset: 0x00526EF4
		public sbyte CurAvatarGroupIndex
		{
			get
			{
				return this._curAvatarGroupIndex;
			}
		}

		// Token: 0x0600B520 RID: 46368 RVA: 0x00528CFC File Offset: 0x00526EFC
		public void SetAge(short age)
		{
			bool needAllUpdate = age < 16 || (this._age < 16 && age >= 16);
			this._age = age;
			this.UpdateShowAbility();
			this.AvatarList.ForEach(delegate(Avatar e)
			{
				e.AvatarAge = this._age;
				bool needAllUpdate = needAllUpdate;
				if (needAllUpdate)
				{
					e.Refresh();
				}
				else
				{
					e.UpdateBeard();
					e.UpdateWrinkle();
				}
				Action onAvatarUpdate = this.OnAvatarUpdate;
				if (onAvatarUpdate != null)
				{
					onAvatarUpdate();
				}
			});
			this.UpdateItemsVisible();
		}

		// Token: 0x0600B521 RID: 46369 RVA: 0x00528D68 File Offset: 0x00526F68
		public short GetAge()
		{
			return this._age;
		}

		// Token: 0x0600B522 RID: 46370 RVA: 0x00528D80 File Offset: 0x00526F80
		public void SetGender(sbyte gender)
		{
			this._gender = gender;
			this.AvatarData.AvatarId = this.AvatarGroup.Id;
			this.UpdateShowAbility();
			this.UpdateItemsVisible();
			this.RefreshAllAvatar();
		}

		// Token: 0x0600B523 RID: 46371 RVA: 0x00528DB5 File Offset: 0x00526FB5
		public sbyte GetGender()
		{
			return this._gender;
		}

		// Token: 0x0600B524 RID: 46372 RVA: 0x00528DBD File Offset: 0x00526FBD
		public void SetTransGender(bool transGender)
		{
			this._transGender = transGender;
			this.AvatarData.AvatarId = this.AvatarGroup.Id;
			this.UpdateShowAbility();
			this.UpdateItemsVisible();
			this.RefreshAllAvatar();
		}

		// Token: 0x0600B525 RID: 46373 RVA: 0x00528DF2 File Offset: 0x00526FF2
		public bool GetTransGender()
		{
			return this._transGender;
		}

		// Token: 0x0600B526 RID: 46374 RVA: 0x00528DFC File Offset: 0x00526FFC
		public void UpdateShowAbilityWithFeatures(List<short> featureIdList)
		{
			this.AvatarData.SetGrowableElementShowingAbility(1, SharedMethods.IsAbleToGrowBeard1(this._age, this._gender, this._transGender, featureIdList));
			this.AvatarData.SetGrowableElementShowingAbility(2, SharedMethods.IsAbleToGrowBeard2(this._age, this._gender, this._transGender, featureIdList));
			this.AvatarData.SetGrowableElementShowingAbility(3, SharedMethods.IsAbleToGrowWrinkle1(this._age, 0));
			this.AvatarData.SetGrowableElementShowingAbility(4, SharedMethods.IsAbleToGrowWrinkle2(this._age, 0));
			this.AvatarData.SetGrowableElementShowingAbility(5, SharedMethods.IsAbleToGrowWrinkle3(this._age, 0));
			this.UpdateItemsVisible();
			this.RefreshAllAvatar();
		}

		// Token: 0x0600B527 RID: 46375 RVA: 0x00528EB0 File Offset: 0x005270B0
		public void SetLockGroupColor(int index)
		{
			bool flag = null != this.AvatarAdjustItemFrontHair && this.AvatarAdjustItemFrontHair.ColorLocked;
			if (flag)
			{
				this.AvatarAdjustItemFrontHair.SetColorIndex(index);
			}
			bool flag2 = null != this.AvatarAdjustItemBackHair && this.AvatarAdjustItemBackHair.ColorLocked;
			if (flag2)
			{
				this.AvatarAdjustItemBackHair.SetColorIndex(index);
			}
			bool flag3 = null != this.AvatarAdjustItemEyeBrows && this.AvatarAdjustItemEyeBrows.ColorLocked;
			if (flag3)
			{
				this.AvatarAdjustItemEyeBrows.SetColorIndex(index);
			}
			bool flag4 = null != this.AvatarAdjustItemBeard1 && this.AvatarAdjustItemBeard1.ColorLocked;
			if (flag4)
			{
				this.AvatarAdjustItemBeard1.SetColorIndex(index);
			}
			bool flag5 = null != this.AvatarAdjustItemBeard2 && this.AvatarAdjustItemBeard2.ColorLocked;
			if (flag5)
			{
				this.AvatarAdjustItemBeard2.SetColorIndex(index);
			}
		}

		// Token: 0x0600B528 RID: 46376 RVA: 0x00528FA4 File Offset: 0x005271A4
		public void UpdateColorLockGroup(AvatarAdjustItemBase item)
		{
			bool flag = null == item;
			if (!flag)
			{
				bool flag2 = item == this.AvatarAdjustItemFrontHair;
				if (flag2)
				{
					bool newState = this.AvatarAdjustItemFrontHair.ColorLocked;
					bool flag3 = null != this.AvatarAdjustItemBackHair && this.AvatarAdjustItemBackHair.ColorLocked != newState;
					if (flag3)
					{
						this.AvatarAdjustItemBackHair.Refers.CGet<CToggleObsolete>("LockSwitch").isOn = newState;
					}
				}
				bool flag4 = item == this.AvatarAdjustItemBackHair;
				if (flag4)
				{
					bool newState = this.AvatarAdjustItemBackHair.ColorLocked;
					bool flag5 = null != this.AvatarAdjustItemFrontHair && this.AvatarAdjustItemFrontHair.ColorLocked != newState;
					if (flag5)
					{
						this.AvatarAdjustItemFrontHair.Refers.CGet<CToggleObsolete>("LockSwitch").isOn = newState;
					}
				}
				bool flag6 = item == this.AvatarAdjustItemBeard1;
				if (flag6)
				{
					bool newState = this.AvatarAdjustItemBeard1.ColorLocked;
					bool flag7 = null != this.AvatarAdjustItemBeard2 && this.AvatarAdjustItemBeard2.ColorLocked != newState;
					if (flag7)
					{
						this.AvatarAdjustItemBeard2.Refers.CGet<CToggleObsolete>("LockSwitch").isOn = newState;
					}
				}
				bool flag8 = item == this.AvatarAdjustItemBeard2;
				if (flag8)
				{
					bool newState = this.AvatarAdjustItemBeard2.ColorLocked;
					bool flag9 = null != this.AvatarAdjustItemBeard1 && this.AvatarAdjustItemBeard1.ColorLocked != newState;
					if (flag9)
					{
						this.AvatarAdjustItemBeard1.Refers.CGet<CToggleObsolete>("LockSwitch").isOn = newState;
					}
				}
			}
		}

		// Token: 0x0600B529 RID: 46377 RVA: 0x00529156 File Offset: 0x00527356
		public void SetCurAvatarGroupIndex(int index)
		{
			index = (int)((sbyte)Mathf.Clamp(index, 0, AvatarAdjustController._allAvatarGroups.Count));
			this._curAvatarGroupIndex = (sbyte)index;
			this.AvatarData.AvatarId = this.AvatarGroup.Id;
			this.RefreshAllAvatar();
		}

		// Token: 0x0600B52A RID: 46378 RVA: 0x00529194 File Offset: 0x00527394
		public ValueTuple<byte, byte> GetGroupIndexByAvatarId(byte avatarId)
		{
			byte i = 0;
			while ((int)i < AvatarAdjustController._allAvatarGroups.Count)
			{
				foreach (AvatarGroup avatarGroup in AvatarAdjustController._allAvatarGroups[(int)i])
				{
					bool flag = avatarGroup.Id == avatarId;
					if (flag)
					{
						return new ValueTuple<byte, byte>(i, (byte)AvatarAdjustController._allAvatarGroups.Count);
					}
				}
				i += 1;
			}
			return new ValueTuple<byte, byte>(1, (byte)AvatarAdjustController._allAvatarGroups.Count);
		}

		// Token: 0x0600B52B RID: 46379 RVA: 0x0052921C File Offset: 0x0052741C
		public void OnItemOpen(AvatarAdjustItemBase item)
		{
			for (int i = 4; i < this._adjustItemList.Count; i++)
			{
				bool flag = this._adjustItemList[i] == item;
				if (!flag)
				{
					bool flag2 = !this._adjustItemList[i].Closed;
					if (flag2)
					{
						this._adjustItemList[i].Close(true);
					}
				}
			}
		}

		// Token: 0x0600B52C RID: 46380 RVA: 0x00529289 File Offset: 0x00527489
		public void MarkLayoutDirty()
		{
			this._layoutDirty = true;
		}

		// Token: 0x0600B52D RID: 46381 RVA: 0x00529293 File Offset: 0x00527493
		private void ClampElementIds()
		{
		}

		// Token: 0x0600B52E RID: 46382 RVA: 0x00529298 File Offset: 0x00527498
		private void LateUpdate()
		{
			bool layoutDirty = this._layoutDirty;
			if (layoutDirty)
			{
				this.ReLayout();
				this._layoutDirty = false;
			}
		}

		// Token: 0x0600B52F RID: 46383 RVA: 0x005292C0 File Offset: 0x005274C0
		private void ReLayout()
		{
			float pos = 0f;
			for (int i = 4; i < this._adjustItemList.Count; i++)
			{
				bool flag = this._adjustItemList[i].gameObject.activeSelf && null != this._adjustItemList[i].RectTransform;
				if (flag)
				{
					this._adjustItemList[i].RectTransform.anchoredPosition = Vector2.down * pos;
					pos += this._adjustItemList[i].Size;
				}
			}
			bool flag2 = null != this.Holder;
			if (flag2)
			{
				this.Holder.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, pos);
			}
		}

		// Token: 0x0600B530 RID: 46384 RVA: 0x00529384 File Offset: 0x00527584
		private void UpdateItemsVisible()
		{
			bool flag = null != this.AvatarAdjustItemBeard1;
			if (flag)
			{
				bool showBeard1State = this.AvatarData.GetGrowableElementShowingState(1);
				bool showBeard1Ability = this.AvatarData.GetGrowableElementShowingAbility(1);
				this.AvatarAdjustItemBeard1.gameObject.SetActive(this.DisplayGender == 1 && showBeard1State && showBeard1Ability);
			}
			bool flag2 = null != this.AvatarAdjustItemBeard2;
			if (flag2)
			{
				bool showBeard2State = this.AvatarData.GetGrowableElementShowingState(2);
				bool showBeard2Ability = this.AvatarData.GetGrowableElementShowingAbility(2);
				this.AvatarAdjustItemBeard2.gameObject.SetActive(this.DisplayGender == 1 && showBeard2State && showBeard2Ability);
			}
			AvatarAsset frontHairAsset = this.AvatarGroup.Get(EAvatarElementsType.Hair1, new short[]
			{
				this.AvatarData.FrontHairId
			});
			bool flag3 = null != this.AvatarAdjustItemBackHair;
			if (flag3)
			{
				this.AvatarAdjustItemBackHair.gameObject.SetActive(frontHairAsset != null && !frontHairAsset.Config.DisableRelativeType && this._age >= 16);
			}
			bool flag4 = null != this.AvatarAdjustItemFeature1;
			if (flag4)
			{
				this.AvatarAdjustItemFeature1.gameObject.SetActive(this._age >= 16);
			}
			bool flag5 = null != this.AvatarAdjustItemFeature2;
			if (flag5)
			{
				this.AvatarAdjustItemFeature2.gameObject.SetActive(this._age >= 16);
			}
			for (int i = 0; i < this._adjustItemList.Count; i++)
			{
				bool flag6 = this._adjustItemList[i].gameObject.activeSelf && null != this._adjustItemList[i].RectTransform && !this._adjustItemList[i].Closed;
				if (flag6)
				{
					this._adjustItemList[i].OnOpen(false);
					break;
				}
			}
			this.MarkLayoutDirty();
		}

		// Token: 0x0600B531 RID: 46385 RVA: 0x00529588 File Offset: 0x00527788
		private void UpdateBackHairItems()
		{
			bool flag = null == this.AvatarAdjustItemBackHair;
			if (!flag)
			{
				AvatarAsset frontHairAsset = this.AvatarGroup.Get(EAvatarElementsType.Hair1, new short[]
				{
					this.AvatarData.FrontHairId
				});
				bool flag2 = frontHairAsset != null && !frontHairAsset.Config.DisableRelativeType;
				if (flag2)
				{
					this.AvatarAdjustItemBackHair.gameObject.SetActive(true);
				}
				else
				{
					this.AvatarAdjustItemBackHair.gameObject.SetActive(false);
				}
				this.MarkLayoutDirty();
			}
		}

		// Token: 0x0600B532 RID: 46386 RVA: 0x00529613 File Offset: 0x00527813
		private void RefreshAllAvatar()
		{
			this.AvatarList.ForEach(delegate(Avatar e)
			{
				if (e != null)
				{
					e.Refresh();
				}
			});
			Action onAvatarUpdate = this.OnAvatarUpdate;
			if (onAvatarUpdate != null)
			{
				onAvatarUpdate();
			}
		}

		// Token: 0x0600B533 RID: 46387 RVA: 0x00529654 File Offset: 0x00527854
		private void UpdateShowAbility()
		{
			bool flag = !this._autoUpdateGrowable;
			if (!flag)
			{
				this.AvatarData.SetGrowableElementShowingState(0, true);
				this.AvatarData.SetGrowableElementShowingAbility(0, true);
				this.AvatarData.SetGrowableElementShowingState(1, !this._transGender);
				this.AvatarData.SetGrowableElementShowingState(2, !this._transGender);
				this.AvatarData.SetGrowableElementShowingAbility(1, !this._transGender && (int)this._age >= GlobalConfig.Instance.AgeShowBeard1);
				this.AvatarData.SetGrowableElementShowingAbility(2, !this._transGender && (int)this._age >= GlobalConfig.Instance.AgeShowBeard2);
				this.AvatarData.SetGrowableElementShowingState(6, true);
				this.AvatarData.SetGrowableElementShowingAbility(6, true);
			}
		}

		// Token: 0x0600B534 RID: 46388 RVA: 0x00529734 File Offset: 0x00527934
		private void UpdateShaveBaldToggles()
		{
			bool flag = null != this.AvatarAdjustItemFrontHair && null != this.AvatarAdjustItemFrontHair.ShaveBaldToggle;
			if (flag)
			{
				this.AvatarAdjustItemFrontHair.ShaveBaldToggle.SetIsOnWithoutNotify(1 != this.AvatarData.FrontHairId && !this.AvatarData.GetGrowableElementShowingState(0));
			}
			bool flag2 = null != this.AvatarAdjustItemBackHair && null != this.AvatarAdjustItemBackHair.ShaveBaldToggle;
			if (flag2)
			{
				this.AvatarAdjustItemBackHair.ShaveBaldToggle.SetIsOnWithoutNotify(1 != this.AvatarData.BackHairId && !this.AvatarData.GetGrowableElementShowingState(0));
			}
			bool flag3 = null != this.AvatarAdjustItemEyeBrows && null != this.AvatarAdjustItemEyeBrows.ShaveBaldToggle;
			if (flag3)
			{
				this.AvatarAdjustItemEyeBrows.ShaveBaldToggle.SetIsOnWithoutNotify(1 != this.AvatarData.EyebrowId && !this.AvatarData.GetGrowableElementShowingState(6));
			}
			bool flag4 = null != this.AvatarAdjustItemBeard1 && null != this.AvatarAdjustItemBeard1.ShaveBaldToggle;
			if (flag4)
			{
				this.AvatarAdjustItemBeard1.ShaveBaldToggle.SetIsOnWithoutNotify(1 != this.AvatarData.Beard1Id && !this.AvatarData.GetGrowableElementShowingState(1));
			}
			bool flag5 = null != this.AvatarAdjustItemBeard2 && null != this.AvatarAdjustItemBeard2.ShaveBaldToggle;
			if (flag5)
			{
				this.AvatarAdjustItemBeard2.ShaveBaldToggle.SetIsOnWithoutNotify(1 != this.AvatarData.Beard2Id && !this.AvatarData.GetGrowableElementShowingState(2));
			}
		}

		// Token: 0x0600B535 RID: 46389 RVA: 0x005298F8 File Offset: 0x00527AF8
		public static void InitGroupColors()
		{
			AvatarManager manager = SingletonObject.getInstance<AvatarManager>();
			bool flag = AvatarAdjustController.SkinColors == null;
			if (flag)
			{
				AvatarAdjustController.SkinColors = manager.SkinColorsWeight.ConvertAll<ValueTuple<byte, Color>>((byte[] e) => new ValueTuple<byte, Color>(e[0], AvatarSkinColors.Instance[e[0]].ColorHex.HexStringToColor()));
			}
			bool flag2 = AvatarAdjustController.FeatureColors == null;
			if (flag2)
			{
				AvatarAdjustController.FeatureColors = manager.FeatureColorsWeight.ConvertAll<ValueTuple<byte, Color>>((byte[] e) => new ValueTuple<byte, Color>(e[0], AvatarFeatureColors.Instance[e[0]].ColorHex.HexStringToColor()));
			}
			bool flag3 = AvatarAdjustController.HairColors == null;
			if (flag3)
			{
				AvatarAdjustController.HairColors = manager.HairColorsWeight.ConvertAll<ValueTuple<byte, Color>>((byte[] e) => new ValueTuple<byte, Color>(e[0], AvatarHairColors.Instance[e[0]].ColorHex.HexStringToColor()));
			}
			bool flag4 = AvatarAdjustController.ClothColors == null;
			if (flag4)
			{
				AvatarAdjustController.ClothColors = manager.ClothColorsWeight.ConvertAll<ValueTuple<byte, Color>>((byte[] e) => new ValueTuple<byte, Color>(e[0], AvatarClothColors.Instance[e[0]].ColorHex.HexStringToColor()));
			}
			bool flag5 = AvatarAdjustController.LipColors == null;
			if (flag5)
			{
				AvatarAdjustController.LipColors = manager.LipColorsWeight.ConvertAll<ValueTuple<byte, Color>>((byte[] e) => new ValueTuple<byte, Color>(e[0], AvatarLipColors.Instance[e[0]].ColorHex.HexStringToColor()));
			}
			bool flag6 = AvatarAdjustController.EyeBallColors == null;
			if (flag6)
			{
				AvatarAdjustController.EyeBallColors = manager.EyeballColorsWeight.ConvertAll<ValueTuple<byte, Color>>((byte[] e) => new ValueTuple<byte, Color>(e[0], AvatarEyeballColors.Instance[e[0]].ColorHex.HexStringToColor()));
			}
		}

		// Token: 0x0600B536 RID: 46390 RVA: 0x00529A74 File Offset: 0x00527C74
		private static void InitAllAvatarGroups()
		{
			bool avatarGroupInitState = AvatarAdjustController.AvatarGroupInitState;
			if (!avatarGroupInitState)
			{
				AvatarAdjustController._allAvatarGroups = new List<AvatarGroup[]>();
				List<AvatarGroup> maleGroupList = SingletonObject.getInstance<AvatarManager>().GetAvatarGroupList((AvatarGroup e) => e.Id % 2 == 1);
				List<AvatarGroup> femaleGroupList = SingletonObject.getInstance<AvatarManager>().GetAvatarGroupList((AvatarGroup e) => e.Id % 2 == 0);
				int j;
				int i;
				for (i = 0; i < Mathf.Max(maleGroupList.Count, femaleGroupList.Count); i = j)
				{
					AvatarGroup maleGroup = maleGroupList.Find((AvatarGroup e) => (int)e.Id == 2 * i + 1);
					AvatarGroup femaleGroup = femaleGroupList.Find((AvatarGroup e) => (int)e.Id == 2 * i + 2);
					bool flag = maleGroup != null && maleGroup.Id >= AvatarHead.Instance[6].AvatarId;
					if (flag)
					{
						maleGroup = null;
					}
					bool flag2 = femaleGroup != null && femaleGroup.Id >= AvatarHead.Instance[6].AvatarId;
					if (flag2)
					{
						femaleGroup = null;
					}
					bool flag3 = maleGroup != null || femaleGroup != null;
					if (flag3)
					{
						AvatarAdjustController._allAvatarGroups.Add(new AvatarGroup[]
						{
							femaleGroup,
							maleGroup
						});
					}
					j = i + 1;
				}
				AvatarAdjustController.AvatarGroupInitState = true;
			}
		}

		// Token: 0x04008D10 RID: 36112
		public RectTransform Holder;

		// Token: 0x04008D11 RID: 36113
		public AvatarAdjustItemGender AvatarAdjustItemGender;

		// Token: 0x04008D12 RID: 36114
		public AvatarAdjustItemBasic AvatarAdjustItemBasic;

		// Token: 0x04008D13 RID: 36115
		public AvatarAdjustItemBodyType AvatarAdjustItemBodyType;

		// Token: 0x04008D14 RID: 36116
		public AvatarAdjustItemSkinColor AvatarAdjustItemSkinColor;

		// Token: 0x04008D15 RID: 36117
		public AvatarAdjustItemFrontHair AvatarAdjustItemFrontHair;

		// Token: 0x04008D16 RID: 36118
		public AvatarAdjustItemBackHair AvatarAdjustItemBackHair;

		// Token: 0x04008D17 RID: 36119
		public AvatarAdjustItemEyeBrows AvatarAdjustItemEyeBrows;

		// Token: 0x04008D18 RID: 36120
		public AvatarAdjustItemEyes AvatarAdjustItemEyes;

		// Token: 0x04008D19 RID: 36121
		public AvatarAdjustItemNose AvatarAdjustItemNose;

		// Token: 0x04008D1A RID: 36122
		public AvatarAdjustItemMouth AvatarAdjustItemMouth;

		// Token: 0x04008D1B RID: 36123
		public AvatarAdjustItemBeard1 AvatarAdjustItemBeard1;

		// Token: 0x04008D1C RID: 36124
		public AvatarAdjustItemBeard2 AvatarAdjustItemBeard2;

		// Token: 0x04008D1D RID: 36125
		public AvatarAdjustItemFeature1 AvatarAdjustItemFeature1;

		// Token: 0x04008D1E RID: 36126
		public AvatarAdjustItemFeature2 AvatarAdjustItemFeature2;

		// Token: 0x04008D1F RID: 36127
		public AvatarAdjustItemCloth AvatarAdjustItemCloth;

		// Token: 0x04008D20 RID: 36128
		public static List<ValueTuple<byte, Color>> SkinColors;

		// Token: 0x04008D21 RID: 36129
		public static List<ValueTuple<byte, Color>> FeatureColors;

		// Token: 0x04008D22 RID: 36130
		public static List<ValueTuple<byte, Color>> HairColors;

		// Token: 0x04008D23 RID: 36131
		public static List<ValueTuple<byte, Color>> ClothColors;

		// Token: 0x04008D24 RID: 36132
		public static List<ValueTuple<byte, Color>> LipColors;

		// Token: 0x04008D25 RID: 36133
		public static List<ValueTuple<byte, Color>> EyeBallColors;

		// Token: 0x04008D26 RID: 36134
		private List<AvatarAdjustItemBase> _adjustItemList;

		// Token: 0x04008D27 RID: 36135
		public List<Avatar> AvatarList;

		// Token: 0x04008D28 RID: 36136
		public AvatarData AvatarData;

		// Token: 0x04008D29 RID: 36137
		private sbyte _gender;

		// Token: 0x04008D2A RID: 36138
		private bool _transGender;

		// Token: 0x04008D2B RID: 36139
		public Action OnAvatarUpdate;

		// Token: 0x04008D2D RID: 36141
		private short _age;

		// Token: 0x04008D2E RID: 36142
		private bool _autoUpdateGrowable;

		// Token: 0x04008D2F RID: 36143
		private bool _layoutDirty;

		// Token: 0x04008D30 RID: 36144
		private sbyte _curAvatarGroupIndex;

		// Token: 0x04008D31 RID: 36145
		private static List<AvatarGroup[]> _allAvatarGroups;

		// Token: 0x04008D32 RID: 36146
		public static bool AvatarGroupInitState;
	}
}
