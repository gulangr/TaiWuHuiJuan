using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Config;
using GameData.Domains.Character.AvatarSystem;
using UnityEngine;

namespace Game.Views.NewGame
{
	// Token: 0x020007F1 RID: 2033
	public static class NewGameSubPageAvatarColorHelper
	{
		// Token: 0x17000BF2 RID: 3058
		// (get) Token: 0x06006345 RID: 25413 RVA: 0x002D75B0 File Offset: 0x002D57B0
		[TupleElementNames(new string[]
		{
			"id",
			"color"
		})]
		public static List<ValueTuple<byte, Color>> SkinColors
		{
			[return: TupleElementNames(new string[]
			{
				"id",
				"color"
			})]
			get
			{
				bool flag = NewGameSubPageAvatarColorHelper._skinColors == null;
				if (flag)
				{
					NewGameSubPageAvatarColorHelper.InitColors();
				}
				return NewGameSubPageAvatarColorHelper._skinColors;
			}
		}

		// Token: 0x17000BF3 RID: 3059
		// (get) Token: 0x06006346 RID: 25414 RVA: 0x002D75DC File Offset: 0x002D57DC
		[TupleElementNames(new string[]
		{
			"id",
			"color"
		})]
		public static List<ValueTuple<byte, Color>> FeatureColors
		{
			[return: TupleElementNames(new string[]
			{
				"id",
				"color"
			})]
			get
			{
				bool flag = NewGameSubPageAvatarColorHelper._featureColors == null;
				if (flag)
				{
					NewGameSubPageAvatarColorHelper.InitColors();
				}
				return NewGameSubPageAvatarColorHelper._featureColors;
			}
		}

		// Token: 0x17000BF4 RID: 3060
		// (get) Token: 0x06006347 RID: 25415 RVA: 0x002D7608 File Offset: 0x002D5808
		[TupleElementNames(new string[]
		{
			"id",
			"color"
		})]
		public static List<ValueTuple<byte, Color>> HairColors
		{
			[return: TupleElementNames(new string[]
			{
				"id",
				"color"
			})]
			get
			{
				bool flag = NewGameSubPageAvatarColorHelper._hairColors == null;
				if (flag)
				{
					NewGameSubPageAvatarColorHelper.InitColors();
				}
				return NewGameSubPageAvatarColorHelper._hairColors;
			}
		}

		// Token: 0x17000BF5 RID: 3061
		// (get) Token: 0x06006348 RID: 25416 RVA: 0x002D7634 File Offset: 0x002D5834
		[TupleElementNames(new string[]
		{
			"id",
			"color"
		})]
		public static List<ValueTuple<byte, Color>> ClothColors
		{
			[return: TupleElementNames(new string[]
			{
				"id",
				"color"
			})]
			get
			{
				bool flag = NewGameSubPageAvatarColorHelper._clothColors == null;
				if (flag)
				{
					NewGameSubPageAvatarColorHelper.InitColors();
				}
				return NewGameSubPageAvatarColorHelper._clothColors;
			}
		}

		// Token: 0x17000BF6 RID: 3062
		// (get) Token: 0x06006349 RID: 25417 RVA: 0x002D7660 File Offset: 0x002D5860
		[TupleElementNames(new string[]
		{
			"id",
			"color"
		})]
		public static List<ValueTuple<byte, Color>> LipColors
		{
			[return: TupleElementNames(new string[]
			{
				"id",
				"color"
			})]
			get
			{
				bool flag = NewGameSubPageAvatarColorHelper._lipColors == null;
				if (flag)
				{
					NewGameSubPageAvatarColorHelper.InitColors();
				}
				return NewGameSubPageAvatarColorHelper._lipColors;
			}
		}

		// Token: 0x17000BF7 RID: 3063
		// (get) Token: 0x0600634A RID: 25418 RVA: 0x002D768C File Offset: 0x002D588C
		[TupleElementNames(new string[]
		{
			"id",
			"color"
		})]
		public static List<ValueTuple<byte, Color>> EyeBallColors
		{
			[return: TupleElementNames(new string[]
			{
				"id",
				"color"
			})]
			get
			{
				bool flag = NewGameSubPageAvatarColorHelper._eyeBallColors == null;
				if (flag)
				{
					NewGameSubPageAvatarColorHelper.InitColors();
				}
				return NewGameSubPageAvatarColorHelper._eyeBallColors;
			}
		}

		// Token: 0x0600634B RID: 25419 RVA: 0x002D76B8 File Offset: 0x002D58B8
		private static void InitColors()
		{
			AvatarManager manager = SingletonObject.getInstance<AvatarManager>();
			bool flag = NewGameSubPageAvatarColorHelper._skinColors == null;
			if (flag)
			{
				NewGameSubPageAvatarColorHelper._skinColors = manager.SkinColorsWeight.ConvertAll<ValueTuple<byte, Color>>((byte[] e) => new ValueTuple<byte, Color>(e[0], AvatarSkinColors.Instance[e[0]].ColorHex.HexStringToColor()));
			}
			bool flag2 = NewGameSubPageAvatarColorHelper._featureColors == null;
			if (flag2)
			{
				NewGameSubPageAvatarColorHelper._featureColors = manager.FeatureColorsWeight.ConvertAll<ValueTuple<byte, Color>>((byte[] e) => new ValueTuple<byte, Color>(e[0], AvatarFeatureColors.Instance[e[0]].ColorHex.HexStringToColor()));
			}
			bool flag3 = NewGameSubPageAvatarColorHelper._hairColors == null;
			if (flag3)
			{
				NewGameSubPageAvatarColorHelper._hairColors = manager.HairColorsWeight.ConvertAll<ValueTuple<byte, Color>>((byte[] e) => new ValueTuple<byte, Color>(e[0], AvatarHairColors.Instance[e[0]].ColorHex.HexStringToColor()));
			}
			bool flag4 = NewGameSubPageAvatarColorHelper._clothColors == null;
			if (flag4)
			{
				NewGameSubPageAvatarColorHelper._clothColors = manager.ClothColorsWeight.ConvertAll<ValueTuple<byte, Color>>((byte[] e) => new ValueTuple<byte, Color>(e[0], AvatarClothColors.Instance[e[0]].ColorHex.HexStringToColor()));
			}
			bool flag5 = NewGameSubPageAvatarColorHelper._lipColors == null;
			if (flag5)
			{
				NewGameSubPageAvatarColorHelper._lipColors = manager.LipColorsWeight.ConvertAll<ValueTuple<byte, Color>>((byte[] e) => new ValueTuple<byte, Color>(e[0], AvatarLipColors.Instance[e[0]].ColorHex.HexStringToColor()));
			}
			bool flag6 = NewGameSubPageAvatarColorHelper._eyeBallColors == null;
			if (flag6)
			{
				NewGameSubPageAvatarColorHelper._eyeBallColors = manager.EyeballColorsWeight.ConvertAll<ValueTuple<byte, Color>>((byte[] e) => new ValueTuple<byte, Color>(e[0], AvatarEyeballColors.Instance[e[0]].ColorHex.HexStringToColor()));
			}
		}

		// Token: 0x0400452C RID: 17708
		[TupleElementNames(new string[]
		{
			"id",
			"color"
		})]
		private static List<ValueTuple<byte, Color>> _skinColors;

		// Token: 0x0400452D RID: 17709
		[TupleElementNames(new string[]
		{
			"id",
			"color"
		})]
		private static List<ValueTuple<byte, Color>> _featureColors;

		// Token: 0x0400452E RID: 17710
		[TupleElementNames(new string[]
		{
			"id",
			"color"
		})]
		private static List<ValueTuple<byte, Color>> _hairColors;

		// Token: 0x0400452F RID: 17711
		[TupleElementNames(new string[]
		{
			"id",
			"color"
		})]
		private static List<ValueTuple<byte, Color>> _clothColors;

		// Token: 0x04004530 RID: 17712
		[TupleElementNames(new string[]
		{
			"id",
			"color"
		})]
		private static List<ValueTuple<byte, Color>> _lipColors;

		// Token: 0x04004531 RID: 17713
		[TupleElementNames(new string[]
		{
			"id",
			"color"
		})]
		private static List<ValueTuple<byte, Color>> _eyeBallColors;
	}
}
