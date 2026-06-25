using System;
using System.IO;
using System.Runtime.CompilerServices;
using Game.Components.Avatar;
using GameData.Domains.Character.AvatarSystem.AvatarRes;
using UnityEngine;

namespace GameData.Domains.Character.AvatarSystem
{
	// Token: 0x02000FCE RID: 4046
	public static class AvatarManagerUtils
	{
		// Token: 0x0600B933 RID: 47411 RVA: 0x00546578 File Offset: 0x00544778
		public static AvatarManagerUtils.AvatarDisplayAsset GetDisplayAsset(this AvatarAsset asset)
		{
			if (asset.ExternalObject == null)
			{
				asset.ExternalObject = new AvatarManagerUtils.AvatarDisplayAsset();
			}
			return (AvatarManagerUtils.AvatarDisplayAsset)asset.ExternalObject;
		}

		// Token: 0x0600B934 RID: 47412 RVA: 0x005465AC File Offset: 0x005447AC
		public static Sprite GetSprite(this AvatarManager manager, int avatarId, EAvatarElementsType elemType, sbyte size, params short[] elemIds)
		{
			AvatarAsset asset = manager.GetAsset(avatarId, elemType, elemIds);
			bool flag = asset == null;
			Sprite result2;
			if (flag)
			{
				result2 = null;
			}
			else
			{
				AvatarAsset avatarAsset = asset;
				if (avatarAsset.ExternalObject == null)
				{
					avatarAsset.ExternalObject = new AvatarManagerUtils.AvatarDisplayAsset();
				}
				AvatarManagerUtils.AvatarDisplayAsset displayAsset = asset.GetDisplayAsset();
				AvatarManagerUtils.AvatarDisplayAsset avatarDisplayAsset = displayAsset;
				if (avatarDisplayAsset.Sprites == null)
				{
					avatarDisplayAsset.Sprites = new Sprite[3];
				}
				Sprite result = displayAsset.Sprites[(int)size];
				bool flag2 = null == result;
				if (flag2)
				{
					bool flag3 = elemType == EAvatarElementsType.Head;
					string spriteName;
					if (flag3)
					{
						spriteName = asset.HeadConfig.NameOrPath;
					}
					else
					{
						spriteName = asset.Config.NameOrPath;
					}
					result = AvatarAtlasAssets.Instance.GetSprite((byte)avatarId, spriteName, size);
					displayAsset.Sprites[(int)size] = result;
				}
				result2 = result;
			}
			return result2;
		}

		// Token: 0x0600B935 RID: 47413 RVA: 0x0054666C File Offset: 0x0054486C
		public static Sprite GetHighLightForHair(this AvatarManager manager, int avatarId, EAvatarElementsType hairElementType, sbyte size, params short[] elemIds)
		{
			bool flag = hairElementType != EAvatarElementsType.Hair1 && hairElementType != EAvatarElementsType.Hair2;
			Sprite result2;
			if (flag)
			{
				result2 = null;
			}
			else
			{
				AvatarAsset asset = manager.GetAsset(avatarId, hairElementType, elemIds);
				bool flag2 = asset == null;
				if (flag2)
				{
					result2 = null;
				}
				else
				{
					AvatarManagerUtils.AvatarDisplayAsset displayAsset = asset.GetDisplayAsset();
					AvatarManagerUtils.AvatarDisplayAsset avatarDisplayAsset = displayAsset;
					if (avatarDisplayAsset.HighLightSprites == null)
					{
						avatarDisplayAsset.HighLightSprites = new Sprite[3];
					}
					Sprite result = displayAsset.HighLightSprites[(int)size];
					bool flag3 = result;
					if (flag3)
					{
						result2 = result;
					}
					else
					{
						string path = asset.Config.NameOrPath + "_highlight";
						result = AvatarAtlasAssets.Instance.GetSprite((byte)avatarId, path, size);
						displayAsset.HighLightSprites[(int)size] = result;
						result2 = result;
					}
				}
			}
			return result2;
		}

		// Token: 0x0600B936 RID: 47414 RVA: 0x00546724 File Offset: 0x00544924
		public static Sprite GetClothHatBackPart(this AvatarManager manager, int avatarId, sbyte size, params short[] elemIds)
		{
			AvatarAsset asset = manager.GetAsset(avatarId, EAvatarElementsType.Cloth, elemIds);
			bool flag = asset == null;
			Sprite result2;
			if (flag)
			{
				result2 = null;
			}
			else
			{
				AvatarManagerUtils.AvatarDisplayAsset displayAsset = asset.GetDisplayAsset();
				AvatarManagerUtils.AvatarDisplayAsset avatarDisplayAsset = displayAsset;
				if (avatarDisplayAsset.HatSprites == null)
				{
					avatarDisplayAsset.HatSprites = new Sprite[3];
				}
				Sprite result = displayAsset.HatSprites[(int)size];
				bool flag2 = result;
				if (flag2)
				{
					result2 = result;
				}
				else
				{
					string path = asset.Config.HatBack;
					result = AvatarAtlasAssets.Instance.GetSprite((byte)avatarId, path, size);
					displayAsset.HatSprites[(int)size] = result;
					result2 = result;
				}
			}
			return result2;
		}

		// Token: 0x0600B937 RID: 47415 RVA: 0x005467B0 File Offset: 0x005449B0
		public static Sprite GetCoverSprite(this AvatarManager manager, int avatarId, EAvatarElementsType elemType, sbyte size, params short[] elemIds)
		{
			AvatarAsset asset = manager.GetAsset(avatarId, elemType, elemIds);
			bool flag = asset == null;
			Sprite result;
			if (flag)
			{
				result = null;
			}
			else
			{
				string path = asset.Config.NameOrPath;
				result = AvatarAtlasAssets.Instance.GetCoverSprite((byte)avatarId, path, size);
			}
			return result;
		}

		// Token: 0x0600B938 RID: 47416 RVA: 0x005467F4 File Offset: 0x005449F4
		public static Sprite GetFeatureSprite(this AvatarManager manager, int avatarId, EAvatarElementsType elemType, sbyte size, bool isLeft, params short[] elemIds)
		{
			AvatarManagerUtils.<>c__DisplayClass6_0 CS$<>8__locals1;
			CS$<>8__locals1.size = size;
			CS$<>8__locals1.isLeft = isLeft;
			CS$<>8__locals1.avatarId = avatarId;
			CS$<>8__locals1.asset = manager.GetAsset(CS$<>8__locals1.avatarId, elemType, elemIds);
			bool flag = CS$<>8__locals1.asset == null;
			Sprite result;
			if (flag)
			{
				result = null;
			}
			else
			{
				AvatarManagerUtils.AvatarDisplayAsset displayAsset = CS$<>8__locals1.asset.GetDisplayAsset();
				bool isLeft2 = CS$<>8__locals1.isLeft;
				if (isLeft2)
				{
					result = AvatarManagerUtils.<GetFeatureSprite>g__GetResult|6_0(displayAsset.Sprites, ref CS$<>8__locals1);
				}
				else
				{
					result = AvatarManagerUtils.<GetFeatureSprite>g__GetResult|6_0(displayAsset.RightFeatureSprites, ref CS$<>8__locals1);
				}
			}
			return result;
		}

		// Token: 0x0600B939 RID: 47417 RVA: 0x0054687C File Offset: 0x00544A7C
		public static float FloatScaleBySize(float srcSize, AvatarSize size)
		{
			bool flag = size == AvatarSize.Big;
			float result;
			if (flag)
			{
				result = srcSize * 2f;
			}
			else
			{
				bool flag2 = size == AvatarSize.Small;
				if (flag2)
				{
					result = srcSize * 0.5f;
				}
				else
				{
					result = srcSize;
				}
			}
			return result;
		}

		// Token: 0x0600B93A RID: 47418 RVA: 0x005468B4 File Offset: 0x00544AB4
		public static short GetAvatarVeilTemplateIdByAvatarId(byte avatarId)
		{
			short result;
			switch (avatarId)
			{
			case 1:
				result = 0;
				break;
			case 2:
				result = 1;
				break;
			case 3:
				result = 2;
				break;
			case 4:
				result = 3;
				break;
			case 5:
				result = 4;
				break;
			case 6:
				result = 5;
				break;
			default:
				result = -1;
				break;
			}
			return result;
		}

		// Token: 0x0600B93B RID: 47419 RVA: 0x00546908 File Offset: 0x00544B08
		public static short GetAvatarJieqingMaskTemplateIdByAvatarId(byte avatarId)
		{
			short result;
			switch (avatarId)
			{
			case 1:
				result = 36;
				break;
			case 2:
				result = 37;
				break;
			case 3:
				result = 38;
				break;
			case 4:
				result = 39;
				break;
			case 5:
				result = 40;
				break;
			case 6:
				result = 41;
				break;
			default:
				switch (avatarId)
				{
				case 251:
					result = 42;
					break;
				case 252:
					result = 43;
					break;
				case 253:
					result = 44;
					break;
				case 254:
					result = 45;
					break;
				default:
					result = -1;
					break;
				}
				break;
			}
			return result;
		}

		// Token: 0x0600B93C RID: 47420 RVA: 0x00546994 File Offset: 0x00544B94
		public static short GetAvatarDarkAshTemplateIdOffset(byte avatarId)
		{
			if (!true)
			{
			}
			short result;
			switch (avatarId)
			{
			case 1:
				result = 0;
				break;
			case 2:
				result = 1;
				break;
			case 3:
				result = 2;
				break;
			case 4:
				result = 3;
				break;
			case 5:
				result = 4;
				break;
			case 6:
				result = 5;
				break;
			default:
				switch (avatarId)
				{
				case 251:
					result = 6;
					break;
				case 252:
					result = 7;
					break;
				case 253:
					result = 8;
					break;
				case 254:
					result = 9;
					break;
				default:
					result = -1;
					break;
				}
				break;
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x0600B93D RID: 47421 RVA: 0x00546A1C File Offset: 0x00544C1C
		public static short GetAvatarBlushTemplateIdByAvatarId(byte avatarId)
		{
			short result;
			switch (avatarId)
			{
			case 1:
				result = 24;
				break;
			case 2:
				result = 25;
				break;
			case 3:
				result = 26;
				break;
			case 4:
				result = 27;
				break;
			case 5:
				result = 28;
				break;
			case 6:
				result = 29;
				break;
			default:
				result = -1;
				break;
			}
			return result;
		}

		// Token: 0x0600B93E RID: 47422 RVA: 0x00546A74 File Offset: 0x00544C74
		public static void InitModAvatar()
		{
			string modRootDir = ModManager.GetModFactoryRootFolder();
			bool flag = !Directory.Exists(modRootDir);
			if (!flag)
			{
				string[] modFolders = Directory.GetDirectories(modRootDir, "*", SearchOption.TopDirectoryOnly);
				foreach (string modFolder in modFolders)
				{
					bool flag2 = !ModManager.IsModEnabled(modFolder);
					if (!flag2)
					{
						string avatarFolder = Path.Combine(modFolder, ModManager.GetPublishAvatarAsset());
						bool flag3 = !Directory.Exists(avatarFolder);
						if (!flag3)
						{
							string[] avatarDirectories = Directory.GetDirectories(avatarFolder, "*", SearchOption.TopDirectoryOnly);
							foreach (string folder in avatarDirectories)
							{
								int modIndex;
								bool flag4 = int.TryParse(new DirectoryInfo(folder).Name, out modIndex);
								if (flag4)
								{
									AvatarManagerUtils.LoadModAvatar(folder, modIndex);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x0600B93F RID: 47423 RVA: 0x00546B54 File Offset: 0x00544D54
		private static Sprite LoadTextureToSprite(this AvatarManager manager, string texPath)
		{
			bool flag = File.Exists(texPath);
			if (flag)
			{
				using (FileStream fStream = new FileStream(texPath, FileMode.Open))
				{
					byte[] imgBuffer = new byte[fStream.Length];
					fStream.Read(imgBuffer, 0, imgBuffer.Length);
					fStream.Close();
					Texture2D texture2D = new Texture2D(1024, 1024);
					texture2D.LoadImage(imgBuffer);
					return Sprite.Create(texture2D, new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height), Vector2.zero);
				}
			}
			return null;
		}

		// Token: 0x0600B940 RID: 47424 RVA: 0x00546C00 File Offset: 0x00544E00
		private static void LoadModAvatar(string modRootFolder, int avatarId)
		{
		}

		// Token: 0x0600B941 RID: 47425 RVA: 0x00546C04 File Offset: 0x00544E04
		[CompilerGenerated]
		internal static Sprite <GetFeatureSprite>g__GetResult|6_0(Sprite[] sprites, ref AvatarManagerUtils.<>c__DisplayClass6_0 A_1)
		{
			bool flag = sprites == null;
			if (flag)
			{
				sprites = new Sprite[3];
			}
			Sprite result = sprites[(int)A_1.size];
			bool flag2 = null == result;
			if (flag2)
			{
				bool flag3 = !A_1.asset.Config.CanMirror;
				string spriteName;
				if (flag3)
				{
					spriteName = A_1.asset.Config.NameOrPath;
				}
				else
				{
					spriteName = A_1.asset.Config.NameOrPath + (A_1.isLeft ? "_l" : "_r");
				}
				result = AvatarAtlasAssets.Instance.GetSprite((byte)A_1.avatarId, spriteName, A_1.size);
				sprites[(int)A_1.size] = result;
			}
			return result;
		}

		// Token: 0x02002617 RID: 9751
		public class AvatarDisplayAsset
		{
			// Token: 0x0400E9A1 RID: 59809
			public Sprite[] Sprites;

			// Token: 0x0400E9A2 RID: 59810
			public Sprite[] RightFeatureSprites;

			// Token: 0x0400E9A3 RID: 59811
			public Sprite[] HighLightSprites;

			// Token: 0x0400E9A4 RID: 59812
			public Sprite[] HatSprites;
		}
	}
}
