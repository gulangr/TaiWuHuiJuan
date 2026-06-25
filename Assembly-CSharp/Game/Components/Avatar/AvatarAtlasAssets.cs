using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

namespace Game.Components.Avatar
{
	// Token: 0x02000F80 RID: 3968
	public class AvatarAtlasAssets : ScriptableObject
	{
		// Token: 0x0600B63B RID: 46651 RVA: 0x0052F896 File Offset: 0x0052DA96
		public static void Init()
		{
			ResLoader.Load<AvatarAtlasAssets>("GameAtlas/AvatarAtlasInfos", delegate(AvatarAtlasAssets a)
			{
				a._runTimeCache = new Dictionary<string, Sprite[]>();
				a._avatarAssetRuntimeMap = new Dictionary<byte, AvatarAtlasAssets.SingleAvatarAtlasAsset>();
				foreach (AvatarAtlasAssets.SingleAvatarAtlasAsset avatarAsset in a.AvatarAtlasDataList)
				{
					avatarAsset.RuntimeBodyMap = new Dictionary<string, SpriteAtlas[]>();
					avatarAsset.HeadAtlasArray = new SpriteAtlas[3];
					a._avatarAssetRuntimeMap.Add(avatarAsset.AvatarId, avatarAsset);
				}
				AvatarAtlasAssets.Instance = a;
				try
				{
					AvatarAtlasAssets.TryLoadDlcAvatars();
				}
				catch (Exception e)
				{
					Debug.LogError(e);
				}
			}, null, false);
		}

		// Token: 0x0600B63C RID: 46652 RVA: 0x0052F8C8 File Offset: 0x0052DAC8
		private static void TryLoadDlcAvatars()
		{
			List<ValueTuple<string, uint>> dlcNameList = new List<ValueTuple<string, uint>>
			{
				new ValueTuple<string, uint>("GiftFromConchShip1", DlcManager.DlcIdGiftFromConchShip1),
				new ValueTuple<string, uint>("GiftFromConchShip2", DlcManager.DlcIdGiftFromConchShip2),
				new ValueTuple<string, uint>("InteractOfLove", DlcManager.DlcIdInteractOfLove),
				new ValueTuple<string, uint>("FiveLoong", DlcManager.DlcIdFiveLoong),
				new ValueTuple<string, uint>("HappyNewYear2024", DlcManager.DlcIdHappyNewYear2024),
				new ValueTuple<string, uint>("EightYears", DlcManager.DlcIdEightYears),
				new ValueTuple<string, uint>("GreenHillsRemain", DlcManager.DlcIdGreenHillsRemain)
			};
			dlcNameList.ForEach(delegate(ValueTuple<string, uint> e)
			{
				bool flag = SingletonObject.getInstance<DlcManager>().IsDlcInstalled(e.Item2);
				if (flag)
				{
					AvatarAtlasAssets.LoadDlcAvatarAssetFromAssetBundle(e.Item1.ToLower() + "_avatarpackers");
				}
			});
		}

		// Token: 0x0600B63D RID: 46653 RVA: 0x0052F99C File Offset: 0x0052DB9C
		private static void LoadDlcAvatarAssetFromAssetBundle(string bundleName)
		{
			bool flag = string.IsNullOrEmpty(bundleName);
			if (!flag)
			{
				ResLoader.LoadByName<AssetBundle>(bundleName, delegate(AssetBundle bundle)
				{
					SpriteAtlas[] allAtlases = bundle.LoadAllAssets<SpriteAtlas>();
					int i = 0;
					int max = allAtlases.Length;
					while (i < max)
					{
						SpriteAtlas atlas = allAtlases[i];
						byte avatarId = AvatarAtlasAssets.GetAvatarIdByAtlasName(atlas.name);
						AvatarAtlasAssets.SingleAvatarAtlasAsset avatarAsset;
						bool flag2 = AvatarAtlasAssets.Instance._avatarAssetRuntimeMap.TryGetValue(avatarId, out avatarAsset);
						if (flag2)
						{
							AvatarAtlasAssets.AppendAtlasToAvatar(avatarAsset, atlas);
						}
						i++;
					}
				}, null);
			}
		}

		// Token: 0x0600B63E RID: 46654 RVA: 0x0052F9E0 File Offset: 0x0052DBE0
		public static byte GetAvatarIdByAtlasName(string atlasName)
		{
			string prefix = "avatar_";
			atlasName = atlasName.Remove(0, prefix.Length);
			int indexOfSplit = atlasName.IndexOf('_');
			bool flag = indexOfSplit < 0;
			byte result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				string avatarIdString = atlasName.Substring(0, indexOfSplit);
				byte avatarId;
				bool flag2 = byte.TryParse(avatarIdString, out avatarId);
				if (flag2)
				{
					result = avatarId;
				}
				else
				{
					result = 0;
				}
			}
			return result;
		}

		// Token: 0x0600B63F RID: 46655 RVA: 0x0052FA40 File Offset: 0x0052DC40
		public string GetAtlasName(byte avatarId, bool isHead, sbyte size, string spriteName)
		{
			string result;
			if (isHead)
			{
				if (!true)
				{
				}
				string text;
				switch (size)
				{
				case 0:
					text = string.Format("avatar_{0}_head_big", avatarId);
					break;
				case 1:
					text = string.Format("avatar_{0}_head_normal", avatarId);
					break;
				case 2:
					text = string.Format("avatar_{0}_head_small", avatarId);
					break;
				default:
					text = string.Empty;
					break;
				}
				if (!true)
				{
				}
				result = text;
			}
			else
			{
				string atlasName = spriteName.Replace("color", string.Empty).Replace("skin", string.Empty);
				bool flag = atlasName.Contains("clothpart");
				if (flag)
				{
					atlasName = atlasName.Substring(0, atlasName.LastIndexOf("_", StringComparison.Ordinal)).Replace("clothpart", "cloth");
				}
				if (!true)
				{
				}
				string text;
				switch (size)
				{
				case 0:
					text = atlasName + "_big";
					break;
				case 1:
					text = atlasName + "_normal";
					break;
				case 2:
					text = atlasName + "_small";
					break;
				default:
					text = string.Empty;
					break;
				}
				if (!true)
				{
				}
				result = text;
			}
			return result;
		}

		// Token: 0x0600B640 RID: 46656 RVA: 0x0052FB60 File Offset: 0x0052DD60
		private static void AppendAtlasToAvatar(AvatarAtlasAssets.SingleAvatarAtlasAsset avatarAtlasAsset, SpriteAtlas spriteAtlas)
		{
			string key = spriteAtlas.name.Replace("_big", string.Empty).Replace("_normal", string.Empty).Replace("_small", string.Empty);
			SpriteAtlas[] atlasArray;
			bool flag = !avatarAtlasAsset.RuntimeBodyMap.TryGetValue(key, out atlasArray);
			if (flag)
			{
				atlasArray = new SpriteAtlas[3];
				avatarAtlasAsset.RuntimeBodyMap.Add(key, atlasArray);
			}
			bool flag2 = spriteAtlas.name.EndsWith("_big");
			if (flag2)
			{
				atlasArray[0] = spriteAtlas;
			}
			bool flag3 = spriteAtlas.name.EndsWith("_normal");
			if (flag3)
			{
				atlasArray[1] = spriteAtlas;
			}
			bool flag4 = spriteAtlas.name.EndsWith("_small");
			if (flag4)
			{
				atlasArray[2] = spriteAtlas;
			}
		}

		// Token: 0x0600B641 RID: 46657 RVA: 0x0052FC1C File Offset: 0x0052DE1C
		public Sprite[] GetSpriteArray(byte avatarId, string spriteName)
		{
			AvatarAtlasAssets.SingleAvatarAtlasAsset avatarAtlasAsset;
			bool flag = !this._avatarAssetRuntimeMap.TryGetValue(avatarId, out avatarAtlasAsset);
			Sprite[] result;
			if (flag)
			{
				Debug.LogError(string.Format("invalid avatar id {0} to GetSprite", avatarId));
				result = null;
			}
			else
			{
				Sprite[] array;
				bool flag2 = !this._runTimeCache.TryGetValue(spriteName, out array);
				if (flag2)
				{
					array = new Sprite[3];
					this._runTimeCache.Add(spriteName, array);
					array[0] = this.GetSprite(avatarId, spriteName, 0);
					array[1] = this.GetSprite(avatarId, spriteName, 1);
					array[2] = this.GetSprite(avatarId, spriteName, 2);
				}
				result = array;
			}
			return result;
		}

		// Token: 0x0600B642 RID: 46658 RVA: 0x0052FCB4 File Offset: 0x0052DEB4
		public Sprite GetCoverSprite(byte avatarId, string originalName, sbyte size)
		{
			string coverName = originalName + "_cover";
			Sprite[] array;
			bool flag = this._runTimeCache.TryGetValue(coverName, out array) && array[(int)size] != null;
			Sprite result2;
			if (flag)
			{
				result2 = array[(int)size];
			}
			else
			{
				string atlasKey = this.GetAtlasName(avatarId, false, size, coverName);
				AvatarAtlasAssets.SingleAvatarAtlasAsset asset;
				SpriteAtlas[] atlasArray;
				bool flag2 = this._avatarAssetRuntimeMap.TryGetValue(avatarId, out asset) && asset.RuntimeBodyMap.TryGetValue(atlasKey, out atlasArray) && atlasArray[(int)size] != null;
				if (flag2)
				{
					Sprite result = atlasArray[(int)size].GetSprite(coverName);
					bool flag3 = !this._runTimeCache.ContainsKey(coverName);
					if (flag3)
					{
						this._runTimeCache.Add(coverName, new Sprite[3]);
					}
					this._runTimeCache[coverName][(int)size] = result;
					result2 = result;
				}
				else
				{
					result2 = this.GetSprite(avatarId, coverName, size);
				}
			}
			return result2;
		}

		// Token: 0x0600B643 RID: 46659 RVA: 0x0052FD94 File Offset: 0x0052DF94
		public Sprite GetSprite(byte avatarId, string spriteName, sbyte size)
		{
			Sprite result = null;
			AvatarAtlasAssets.SingleAvatarAtlasAsset avatarAtlasAsset;
			bool flag = !this._avatarAssetRuntimeMap.TryGetValue(avatarId, out avatarAtlasAsset);
			Sprite result2;
			if (flag)
			{
				Debug.LogError(string.Format("invalid avatar id {0} to GetSprite", avatarId));
				result2 = null;
			}
			else
			{
				Sprite[] array;
				bool flag2 = this._runTimeCache.TryGetValue(spriteName, out array);
				if (flag2)
				{
					result = array[(int)size];
				}
				else
				{
					array = new Sprite[3];
					this._runTimeCache.Add(spriteName, array);
				}
				bool flag3 = null != result;
				if (flag3)
				{
					result2 = result;
				}
				else
				{
					bool isHeadAsset = !spriteName.Contains("cloth");
					bool flag4 = isHeadAsset && null != avatarAtlasAsset.HeadAtlasArray[(int)size];
					if (flag4)
					{
						result = avatarAtlasAsset.HeadAtlasArray[(int)size].GetSprite(spriteName);
						array[(int)size] = result;
					}
					bool flag5 = null != result;
					if (flag5)
					{
						result2 = result;
					}
					else
					{
						string atlasName = this.GetAtlasName(avatarId, isHeadAsset, size, spriteName);
						SpriteAtlas[] atlasArray;
						bool flag6 = avatarAtlasAsset.RuntimeBodyMap.TryGetValue(atlasName, out atlasArray);
						if (flag6)
						{
							bool flag7 = null != atlasArray[(int)size];
							if (flag7)
							{
								result = atlasArray[(int)size].GetSprite(spriteName);
							}
							array[(int)size] = result;
						}
						bool flag8 = null != result;
						if (flag8)
						{
							result2 = result;
						}
						else
						{
							SpriteAtlas atlas = ResLoader.SyncLoad<SpriteAtlas>("GameAtlas/AvatarPackers/" + atlasName);
							bool flag9 = atlas == null;
							if (flag9)
							{
								Debug.LogError("Failed to find atlas " + atlasName);
								result2 = null;
							}
							else
							{
								result = atlas.GetSprite(spriteName);
								array[(int)size] = result;
								bool flag10 = isHeadAsset;
								if (flag10)
								{
									avatarAtlasAsset.HeadAtlasArray[(int)size] = atlas;
								}
								else
								{
									bool flag11 = !avatarAtlasAsset.RuntimeBodyMap.TryGetValue(atlasName, out atlasArray);
									if (flag11)
									{
										avatarAtlasAsset.RuntimeBodyMap.Add(atlasName, new SpriteAtlas[3]);
									}
									avatarAtlasAsset.RuntimeBodyMap[atlasName][(int)size] = atlas;
								}
								result2 = result;
							}
						}
					}
				}
			}
			return result2;
		}

		// Token: 0x04008D6B RID: 36203
		public List<AvatarAtlasAssets.SingleAvatarAtlasAsset> AvatarAtlasDataList;

		// Token: 0x04008D6C RID: 36204
		public static AvatarAtlasAssets Instance;

		// Token: 0x04008D6D RID: 36205
		public const string FilePath = "GameAtlas/AvatarAtlasInfos";

		// Token: 0x04008D6E RID: 36206
		private Dictionary<string, Sprite[]> _runTimeCache;

		// Token: 0x04008D6F RID: 36207
		private Dictionary<byte, AvatarAtlasAssets.SingleAvatarAtlasAsset> _avatarAssetRuntimeMap;

		// Token: 0x020025B7 RID: 9655
		[Serializable]
		public class SingleAvatarAtlasAsset
		{
			// Token: 0x0400E8CD RID: 59597
			public byte AvatarId;

			// Token: 0x0400E8CE RID: 59598
			[NonSerialized]
			public SpriteAtlas[] HeadAtlasArray;

			// Token: 0x0400E8CF RID: 59599
			[Obsolete]
			[NonSerialized]
			public List<SpriteAtlas> BodyAtlasList;

			// Token: 0x0400E8D0 RID: 59600
			public Dictionary<string, SpriteAtlas[]> RuntimeBodyMap;
		}
	}
}
