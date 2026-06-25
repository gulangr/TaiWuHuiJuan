using System;
using System.Text;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.Domains.World;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace Game.Views.EventWindow
{
	// Token: 0x02000A37 RID: 2615
	public class EventCharacterBack : MonoBehaviour
	{
		// Token: 0x17000E21 RID: 3617
		// (get) Token: 0x06008116 RID: 33046 RVA: 0x003C1698 File Offset: 0x003BF898
		public Vector2 LeftRightHolderOriginPosX
		{
			get
			{
				bool flag = this._leftRightHolderOriginPosX == null;
				if (flag)
				{
					this._leftRightHolderOriginPosX = new Vector2?(new Vector2(this.leftHolder.transform.localPosition.x, this.rightHolder.transform.localPosition.x));
				}
				return this._leftRightHolderOriginPosX.Value;
			}
		}

		// Token: 0x17000E22 RID: 3618
		// (get) Token: 0x06008117 RID: 33047 RVA: 0x003C1703 File Offset: 0x003BF903
		public Transform LeftHolder
		{
			get
			{
				return this.leftHolder.transform;
			}
		}

		// Token: 0x17000E23 RID: 3619
		// (get) Token: 0x06008118 RID: 33048 RVA: 0x003C1710 File Offset: 0x003BF910
		public Transform RightHolder
		{
			get
			{
				return this.rightHolder.transform;
			}
		}

		// Token: 0x06008119 RID: 33049 RVA: 0x003C1720 File Offset: 0x003BF920
		public void Refresh(bool show)
		{
			WorldMapModel mapModel = SingletonObject.getInstance<WorldMapModel>();
			int currentDate = SingletonObject.getInstance<BasicGameData>().CurrDate;
			this._random = new Random(currentDate + (int)mapModel.CurrentBlockId);
			this.leftHolder.gameObject.SetActive(true);
			this.rightHolder.gameObject.SetActive(true);
			int leftBackNum = this._random.Next(0, 2);
			int leftMidNum = this._random.Next(0, 2);
			int leftFrontNum = this._random.Next(0, 2);
			int rightBackNum = (leftBackNum == 0) ? 1 : 0;
			int rightMidNum = (leftMidNum == 0) ? 1 : 0;
			int rightFrontNum = (leftFrontNum == 0) ? 1 : 0;
			AdventureRemakeModel adventureModel = SingletonObject.getInstance<AdventureRemakeModel>();
			MapBlockItem config = mapModel.PlayerAtBlock.GetConfig();
			bool flag = mapModel.PlayerAtBlock == null || !adventureModel.NotInAdventureAndMajorEvent || !show || config.ArtEventBackground.IsNullOrEmpty();
			if (flag)
			{
				string leftBackName = this.GetDefaultAssetName(EventCharacterBack.ShotType.Back, leftBackNum);
				string rightBackName = this.GetDefaultAssetName(EventCharacterBack.ShotType.Back, rightBackNum);
				string leftMidName = this.GetDefaultAssetName(EventCharacterBack.ShotType.Middle, leftMidNum);
				string rightMidName = this.GetDefaultAssetName(EventCharacterBack.ShotType.Middle, rightMidNum);
				string leftFrontName = this.GetDefaultAssetName(EventCharacterBack.ShotType.Front, leftFrontNum);
				string rightFrontName = this.GetDefaultAssetName(EventCharacterBack.ShotType.Front, rightFrontNum);
				this.SetAsset(leftBackName, this.leftBack);
				this.SetAsset(rightBackName, this.rightBack);
				this.SetAsset(leftMidName, this.leftMid);
				this.SetAsset(rightMidName, this.rightMid);
				this.SetAsset(leftFrontName, this.leftFront);
				this.SetAsset(rightFrontName, this.rightFront);
			}
			else
			{
				bool flag2 = this.testSwitch;
				if (flag2)
				{
					config = MapBlock.Instance[this.testMapBlockTemplateId];
				}
				bool winter = config.EventBackgroundWinter && TimeKit.GetCurrSeason() == 3;
				string leftBackName2 = this.GetAssetName(config, EventCharacterBack.ShotType.Back, winter, leftBackNum);
				string rightBackName2 = this.GetAssetName(config, EventCharacterBack.ShotType.Back, winter, rightBackNum);
				string leftMidName2 = this.GetAssetName(config, EventCharacterBack.ShotType.Middle, winter, leftMidNum);
				string rightMidName2 = this.GetAssetName(config, EventCharacterBack.ShotType.Middle, winter, rightMidNum);
				string leftFrontName2 = this.GetAssetName(config, EventCharacterBack.ShotType.Front, winter, leftFrontNum);
				string rightFrontName2 = this.GetAssetName(config, EventCharacterBack.ShotType.Front, winter, rightFrontNum);
				this.SetAsset(leftBackName2, this.leftBack);
				this.SetAsset(rightBackName2, this.rightBack);
				this.SetAsset(leftMidName2, this.leftMid);
				this.SetAsset(rightMidName2, this.rightMid);
				this.SetAsset(leftFrontName2, this.leftFront);
				this.SetAsset(rightFrontName2, this.rightFront);
			}
		}

		// Token: 0x0600811A RID: 33050 RVA: 0x003C1988 File Offset: 0x003BFB88
		private string GetAssetName(MapBlockItem mapBlockItem, EventCharacterBack.ShotType shotType, bool winter, int num)
		{
			StringBuilder sb = EasyPool.Get<StringBuilder>();
			sb.Clear();
			sb.Append(this.assetPath).Append("tex_block_event_").Append(this.GetShortTypeStr(shotType)).Append(mapBlockItem.ArtEventBackground).Append("_").Append(num);
			if (winter)
			{
				sb.Append("_winter");
			}
			sb.Append("_SkeletonData");
			string assetName = sb.ToString();
			EasyPool.Free<StringBuilder>(sb);
			return assetName;
		}

		// Token: 0x0600811B RID: 33051 RVA: 0x003C1A14 File Offset: 0x003BFC14
		private string GetDefaultAssetName(EventCharacterBack.ShotType shotType, int num)
		{
			StringBuilder sb = EasyPool.Get<StringBuilder>();
			sb.Clear();
			sb.Append(this.assetPath).Append("tex_block_event_").Append(this.GetShortTypeStr(shotType)).Append("darkcloud_").Append(num);
			sb.Append("_SkeletonData");
			string assetName = sb.ToString();
			EasyPool.Free<StringBuilder>(sb);
			return assetName;
		}

		// Token: 0x0600811C RID: 33052 RVA: 0x003C1A84 File Offset: 0x003BFC84
		private void SetAsset(string path, SkeletonGraphic skeletonGraphic)
		{
			ResLoader.Load<SkeletonDataAsset>(path, delegate(SkeletonDataAsset asset)
			{
				skeletonGraphic.skeletonDataAsset = asset;
				skeletonGraphic.Initialize(true);
				Spine.AnimationState animationState = skeletonGraphic.AnimationState;
				if (animationState != null)
				{
					animationState.SetAnimation(0, "animation", true);
				}
			}, delegate(string error)
			{
				Debug.LogWarning(path + "  load fail");
			}, false);
		}

		// Token: 0x0600811D RID: 33053 RVA: 0x003C1ACC File Offset: 0x003BFCCC
		private string GetShortTypeStr(EventCharacterBack.ShotType shotType)
		{
			bool flag = shotType == EventCharacterBack.ShotType.Back;
			string result;
			if (flag)
			{
				result = "back_";
			}
			else
			{
				bool flag2 = shotType == EventCharacterBack.ShotType.Front;
				if (flag2)
				{
					result = "front_";
				}
				else
				{
					result = "mid_";
				}
			}
			return result;
		}

		// Token: 0x04006282 RID: 25218
		[SerializeField]
		private GameObject leftHolder;

		// Token: 0x04006283 RID: 25219
		[SerializeField]
		private GameObject rightHolder;

		// Token: 0x04006284 RID: 25220
		[SerializeField]
		private SkeletonGraphic leftBack;

		// Token: 0x04006285 RID: 25221
		[SerializeField]
		private SkeletonGraphic leftMid;

		// Token: 0x04006286 RID: 25222
		[SerializeField]
		private SkeletonGraphic leftFront;

		// Token: 0x04006287 RID: 25223
		[SerializeField]
		private SkeletonGraphic rightBack;

		// Token: 0x04006288 RID: 25224
		[SerializeField]
		private SkeletonGraphic rightMid;

		// Token: 0x04006289 RID: 25225
		[SerializeField]
		private SkeletonGraphic rightFront;

		// Token: 0x0400628A RID: 25226
		[SerializeField]
		private bool testSwitch;

		// Token: 0x0400628B RID: 25227
		[SerializeField]
		private short testMapBlockTemplateId;

		// Token: 0x0400628C RID: 25228
		[SerializeField]
		private CButton testRefreshBtn;

		// Token: 0x0400628D RID: 25229
		private const string NameStr = "tex_block_event_";

		// Token: 0x0400628E RID: 25230
		private readonly string assetPath = "RemakeResources/SpineAnimations/EventCharacterBack/";

		// Token: 0x0400628F RID: 25231
		private const string WinterStr = "_winter";

		// Token: 0x04006290 RID: 25232
		private const string DefaultCould = "darkcloud_";

		// Token: 0x04006291 RID: 25233
		private Random _random;

		// Token: 0x04006292 RID: 25234
		private Vector2? _leftRightHolderOriginPosX;

		// Token: 0x02001FE3 RID: 8163
		private enum ShotType
		{
			// Token: 0x0400CF19 RID: 53017
			Front,
			// Token: 0x0400CF1A RID: 53018
			Middle,
			// Token: 0x0400CF1B RID: 53019
			Back
		}
	}
}
