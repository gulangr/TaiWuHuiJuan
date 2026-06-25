using System;
using System.Collections.Generic;
using Game.Components.Avatar;
using UnityEngine;

namespace Config
{
	// Token: 0x020006E1 RID: 1761
	[CreateAssetMenu(fileName = "AvatarSetting", menuName = "Config/AvatarSetting")]
	public class AvatarSetting : ScriptableObject
	{
		// Token: 0x0600539E RID: 21406 RVA: 0x0026C71C File Offset: 0x0026A91C
		private void OnEnable()
		{
			this._skeletonHeadOffsetCache = null;
			this._clothingCoverNamesCache = null;
			this._skeletonDataNamesCache = null;
		}

		// Token: 0x0600539F RID: 21407 RVA: 0x0026C734 File Offset: 0x0026A934
		public static void Init()
		{
			bool flag = null != AvatarSetting.Instance;
			if (!flag)
			{
				ResLoader.Load<AvatarSetting>("GameAtlas/AvatarSetting", delegate(AvatarSetting configObj)
				{
					AvatarSetting.Instance = configObj;
				}, null, false);
			}
		}

		// Token: 0x060053A0 RID: 21408 RVA: 0x0026C780 File Offset: 0x0026A980
		public Vector2 GetFixedAvatarOffset(int setIndex, string avatarName, AvatarSize avatarSize)
		{
			bool flag = setIndex <= 0 || setIndex > this.fixedAvatarOffsetSets.Length || string.IsNullOrEmpty(avatarName);
			Vector2 zero;
			if (flag)
			{
				zero = Vector2.zero;
			}
			else
			{
				AvatarSetting.FixedAvatarOffsetSet set = this.fixedAvatarOffsetSets[setIndex - 1];
				foreach (AvatarSetting.FixedAvatarOffsetData data in set.offsetData)
				{
					bool flag2 = data.avatarName == avatarName;
					if (flag2)
					{
						if (!true)
						{
						}
						Vector2 result;
						switch (avatarSize)
						{
						case AvatarSize.Big:
							result = data.largeOffset;
							break;
						case AvatarSize.Normal:
							result = data.mediumOffset;
							break;
						case AvatarSize.Small:
							result = data.smallOffset;
							break;
						default:
							result = Vector2.zero;
							break;
						}
						if (!true)
						{
						}
						return result;
					}
				}
				zero = Vector2.zero;
			}
			return zero;
		}

		// Token: 0x060053A1 RID: 21409 RVA: 0x0026C854 File Offset: 0x0026AA54
		public bool TryGetFixedAvatarOffset(int setIndex, string avatarName, AvatarSize avatarSize, out Vector2 offset, out float scale)
		{
			offset = Vector2.zero;
			scale = 1f;
			bool flag = setIndex <= 0 || setIndex > this.fixedAvatarOffsetSets.Length || string.IsNullOrEmpty(avatarName);
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				AvatarSetting.FixedAvatarOffsetSet set = this.fixedAvatarOffsetSets[setIndex - 1];
				foreach (AvatarSetting.FixedAvatarOffsetData data in set.offsetData)
				{
					bool flag2 = data.avatarName == avatarName;
					if (flag2)
					{
						if (!true)
						{
						}
						Vector2 vector;
						switch (avatarSize)
						{
						case AvatarSize.Big:
							vector = data.largeOffset;
							break;
						case AvatarSize.Normal:
							vector = data.mediumOffset;
							break;
						case AvatarSize.Small:
							vector = data.smallOffset;
							break;
						default:
							vector = Vector2.zero;
							break;
						}
						if (!true)
						{
						}
						offset = vector;
						scale = data.scale;
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x060053A2 RID: 21410 RVA: 0x0026C94C File Offset: 0x0026AB4C
		public float GetFixedAvatarScale(int setIndex, string avatarName)
		{
			bool flag = setIndex <= 0 || setIndex > this.fixedAvatarOffsetSets.Length || string.IsNullOrEmpty(avatarName);
			float result;
			if (flag)
			{
				result = 1f;
			}
			else
			{
				AvatarSetting.FixedAvatarOffsetSet set = this.fixedAvatarOffsetSets[setIndex - 1];
				foreach (AvatarSetting.FixedAvatarOffsetData data in set.offsetData)
				{
					bool flag2 = data.avatarName == avatarName;
					if (flag2)
					{
						return data.scale;
					}
				}
				result = 1f;
			}
			return result;
		}

		// Token: 0x060053A3 RID: 21411 RVA: 0x0026C9DC File Offset: 0x0026ABDC
		public Vector2 GetNormalAvatarOffset(int setIndex, byte avatarId, AvatarSize avatarSize)
		{
			bool flag = setIndex <= 0 || setIndex > this.normalAvatarOffsetSets.Length;
			Vector2 zero;
			if (flag)
			{
				zero = Vector2.zero;
			}
			else
			{
				AvatarSetting.NormalAvatarOffsetSet set = this.normalAvatarOffsetSets[setIndex - 1];
				foreach (AvatarSetting.NormalAvatarOffsetData data in set.offsetData)
				{
					bool flag2 = data.avatarId == avatarId;
					if (flag2)
					{
						if (!true)
						{
						}
						Vector2 result;
						switch (avatarSize)
						{
						case AvatarSize.Big:
							result = data.largeOffset;
							break;
						case AvatarSize.Normal:
							result = data.mediumOffset;
							break;
						case AvatarSize.Small:
							result = data.smallOffset;
							break;
						default:
							result = Vector2.zero;
							break;
						}
						if (!true)
						{
						}
						return result;
					}
				}
				zero = Vector2.zero;
			}
			return zero;
		}

		// Token: 0x060053A4 RID: 21412 RVA: 0x0026CAA8 File Offset: 0x0026ACA8
		public bool TryGetSkeletonHeadOffsetY(byte avatarId, int headSetIndex, out float yOffset)
		{
			yOffset = 0f;
			bool flag = headSetIndex <= 0 || this.skeletonHeadOffsetData == null || this.skeletonHeadOffsetData.Length == 0;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = this._skeletonHeadOffsetCache == null;
				if (flag2)
				{
					this.BuildSkeletonHeadOffsetCache();
				}
				result = this._skeletonHeadOffsetCache.TryGetValue(AvatarSetting.GetSkeletonHeadOffsetKey(avatarId, headSetIndex), out yOffset);
			}
			return result;
		}

		// Token: 0x060053A5 RID: 21413 RVA: 0x0026CB0C File Offset: 0x0026AD0C
		public float GetSkeletonHeadOffsetY(byte avatarId, int headSetIndex)
		{
			float yOffset;
			return this.TryGetSkeletonHeadOffsetY(avatarId, headSetIndex, out yOffset) ? yOffset : 0f;
		}

		// Token: 0x060053A6 RID: 21414 RVA: 0x0026CB34 File Offset: 0x0026AD34
		private void BuildSkeletonHeadOffsetCache()
		{
			this._skeletonHeadOffsetCache = new Dictionary<int, float>();
			bool flag = this.skeletonHeadOffsetData == null;
			if (!flag)
			{
				for (int i = 0; i < this.skeletonHeadOffsetData.Length; i++)
				{
					AvatarSetting.SkeletonHeadOffsetData item = this.skeletonHeadOffsetData[i];
					bool flag2 = item.headSetIndex <= 0;
					if (!flag2)
					{
						int key = AvatarSetting.GetSkeletonHeadOffsetKey(item.avatarId, (int)item.headSetIndex);
						this._skeletonHeadOffsetCache[key] = item.yOffset;
					}
				}
			}
		}

		// Token: 0x060053A7 RID: 21415 RVA: 0x0026CBC0 File Offset: 0x0026ADC0
		private static int GetSkeletonHeadOffsetKey(byte avatarId, int headSetIndex)
		{
			return (int)avatarId << 16 | (headSetIndex & 65535);
		}

		// Token: 0x060053A8 RID: 21416 RVA: 0x0026CBE0 File Offset: 0x0026ADE0
		public bool HasClothingCover(string baseName)
		{
			bool flag = this._clothingCoverNamesCache == null;
			if (flag)
			{
				this.BuildClothingCoverCache();
			}
			return this._clothingCoverNamesCache.Contains(baseName);
		}

		// Token: 0x060053A9 RID: 21417 RVA: 0x0026CC14 File Offset: 0x0026AE14
		public string[] GetClothingCoverNames()
		{
			return this.clothingCoverNames ?? Array.Empty<string>();
		}

		// Token: 0x060053AA RID: 21418 RVA: 0x0026CC35 File Offset: 0x0026AE35
		public void SetClothingCoverNames(string[] names)
		{
			this.clothingCoverNames = names;
			this._clothingCoverNamesCache = null;
		}

		// Token: 0x060053AB RID: 21419 RVA: 0x0026CC48 File Offset: 0x0026AE48
		private void BuildClothingCoverCache()
		{
			this._clothingCoverNamesCache = new HashSet<string>();
			bool flag = this.clothingCoverNames == null;
			if (!flag)
			{
				foreach (string name in this.clothingCoverNames)
				{
					this._clothingCoverNamesCache.Add(name);
				}
			}
		}

		// Token: 0x060053AC RID: 21420 RVA: 0x0026CC98 File Offset: 0x0026AE98
		public bool HasSkeletonData(string resourcePath)
		{
			bool flag = this._skeletonDataNamesCache == null;
			if (flag)
			{
				this.BuildSkeletonDataCache();
			}
			return this._skeletonDataNamesCache.Contains(resourcePath.Replace('\\', '/'));
		}

		// Token: 0x060053AD RID: 21421 RVA: 0x0026CCD4 File Offset: 0x0026AED4
		public string[] GetSkeletonDataNames()
		{
			return this.skeletonDataNames ?? Array.Empty<string>();
		}

		// Token: 0x060053AE RID: 21422 RVA: 0x0026CCF5 File Offset: 0x0026AEF5
		public void SetSkeletonDataNames(string[] names)
		{
			this.skeletonDataNames = names;
			this._skeletonDataNamesCache = null;
		}

		// Token: 0x060053AF RID: 21423 RVA: 0x0026CD08 File Offset: 0x0026AF08
		private void BuildSkeletonDataCache()
		{
			this._skeletonDataNamesCache = new HashSet<string>();
			bool flag = this.skeletonDataNames == null;
			if (!flag)
			{
				foreach (string name in this.skeletonDataNames)
				{
					this._skeletonDataNamesCache.Add(name);
				}
			}
		}

		// Token: 0x060053B0 RID: 21424 RVA: 0x0026CD58 File Offset: 0x0026AF58
		public string[] GetFixedAvatarOffsetSetNames()
		{
			bool flag = this.fixedAvatarOffsetSets == null || this.fixedAvatarOffsetSets.Length == 0;
			string[] result;
			if (flag)
			{
				result = new string[0];
			}
			else
			{
				string[] names = new string[this.fixedAvatarOffsetSets.Length];
				for (int i = 0; i < this.fixedAvatarOffsetSets.Length; i++)
				{
					names[i] = this.fixedAvatarOffsetSets[i].setName;
				}
				result = names;
			}
			return result;
		}

		// Token: 0x060053B1 RID: 21425 RVA: 0x0026CDCC File Offset: 0x0026AFCC
		public string[] GetNormalAvatarOffsetSetNames()
		{
			bool flag = this.normalAvatarOffsetSets == null || this.normalAvatarOffsetSets.Length == 0;
			string[] result;
			if (flag)
			{
				result = new string[0];
			}
			else
			{
				string[] names = new string[this.normalAvatarOffsetSets.Length];
				for (int i = 0; i < this.normalAvatarOffsetSets.Length; i++)
				{
					names[i] = this.normalAvatarOffsetSets[i].setName;
				}
				result = names;
			}
			return result;
		}

		// Token: 0x0400388C RID: 14476
		public const string AssetPath = "GameAtlas/AvatarSetting";

		// Token: 0x0400388D RID: 14477
		[Header("固定立绘偏移配置套")]
		public AvatarSetting.FixedAvatarOffsetSet[] fixedAvatarOffsetSets;

		// Token: 0x0400388E RID: 14478
		[Header("正常立绘偏移配置套")]
		public AvatarSetting.NormalAvatarOffsetSet[] normalAvatarOffsetSets;

		// Token: 0x0400388F RID: 14479
		[Header("拼接动态立绘头部偏移配置")]
		public AvatarSetting.SkeletonHeadOffsetData[] skeletonHeadOffsetData;

		// Token: 0x04003890 RID: 14480
		[Header("Cover资源")]
		[SerializeField]
		private string[] clothingCoverNames;

		// Token: 0x04003891 RID: 14481
		[Header("骨架资源")]
		[SerializeField]
		private string[] skeletonDataNames;

		// Token: 0x04003892 RID: 14482
		private Dictionary<string, Dictionary<string, AvatarSetting.FixedAvatarOffsetData>> _fixedOffsetCache;

		// Token: 0x04003893 RID: 14483
		private Dictionary<string, AvatarSetting.NormalAvatarOffsetData[]> _normalOffsetCache;

		// Token: 0x04003894 RID: 14484
		private Dictionary<int, float> _skeletonHeadOffsetCache;

		// Token: 0x04003895 RID: 14485
		private HashSet<string> _clothingCoverNamesCache;

		// Token: 0x04003896 RID: 14486
		private HashSet<string> _skeletonDataNamesCache;

		// Token: 0x04003897 RID: 14487
		public static AvatarSetting Instance;

		// Token: 0x02001B1B RID: 6939
		[Serializable]
		public struct FixedAvatarOffsetData
		{
			// Token: 0x0400B840 RID: 47168
			[Header("固定立绘信息")]
			public string avatarName;

			// Token: 0x0400B841 RID: 47169
			[Header("小尺寸偏移")]
			public Vector2 smallOffset;

			// Token: 0x0400B842 RID: 47170
			[Header("中尺寸偏移")]
			public Vector2 mediumOffset;

			// Token: 0x0400B843 RID: 47171
			[Header("大尺寸偏移")]
			public Vector2 largeOffset;

			// Token: 0x0400B844 RID: 47172
			[Header("缩放比例")]
			public float scale;
		}

		// Token: 0x02001B1C RID: 6940
		[Serializable]
		public struct NormalAvatarOffsetData
		{
			// Token: 0x0400B845 RID: 47173
			[Header("体型信息")]
			public byte avatarId;

			// Token: 0x0400B846 RID: 47174
			[Header("小尺寸偏移")]
			public Vector2 smallOffset;

			// Token: 0x0400B847 RID: 47175
			[Header("中尺寸偏移")]
			public Vector2 mediumOffset;

			// Token: 0x0400B848 RID: 47176
			[Header("大尺寸偏移")]
			public Vector2 largeOffset;
		}

		// Token: 0x02001B1D RID: 6941
		[Serializable]
		public struct SkeletonHeadOffsetData
		{
			// Token: 0x0400B849 RID: 47177
			[Header("体型信息")]
			public byte avatarId;

			// Token: 0x0400B84A RID: 47178
			[Header("头图套数")]
			public short headSetIndex;

			// Token: 0x0400B84B RID: 47179
			[Header("动态头部Y偏移")]
			public float yOffset;
		}

		// Token: 0x02001B1E RID: 6942
		[Serializable]
		public struct FixedAvatarOffsetSet
		{
			// Token: 0x0400B84C RID: 47180
			[Header("方案信息")]
			public string setName;

			// Token: 0x0400B84D RID: 47181
			public AvatarSetting.FixedAvatarOffsetData[] offsetData;
		}

		// Token: 0x02001B1F RID: 6943
		[Serializable]
		public struct NormalAvatarOffsetSet
		{
			// Token: 0x0400B84E RID: 47182
			[Header("方案信息")]
			public string setName;

			// Token: 0x0400B84F RID: 47183
			public AvatarSetting.NormalAvatarOffsetData[] offsetData;
		}
	}
}
