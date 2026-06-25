using System;
using System.Collections.Generic;
using Spine;

namespace Game.Views.Cricket
{
	// Token: 0x02000ABA RID: 2746
	public class CricketViewSpineHelper : ISingletonInit, IDisposable
	{
		// Token: 0x060086CD RID: 34509 RVA: 0x003EAF7B File Offset: 0x003E917B
		public void Dispose()
		{
			this.spineDataParsers.Clear();
		}

		// Token: 0x060086CE RID: 34510 RVA: 0x003EAF8C File Offset: 0x003E918C
		public void Init()
		{
			for (ECricketSlot slotType = ECricketSlot.Head; slotType < ECricketSlot.Count; slotType++)
			{
				CricketViewSpineHelper.SlotTypeToNameMap[(int)slotType] = slotType.ToString();
			}
			this.resultDataBuffer = new CricketViewSpineHelper.DisplayData[CricketViewSpineHelper.SlotTypeToNameMap.Length];
		}

		// Token: 0x060086CF RID: 34511 RVA: 0x003EAFD4 File Offset: 0x003E91D4
		public static void SetSpineDisplay(short id, Skeleton skeleton, Skin skin, bool isTripleTail)
		{
			CricketViewSpineHelper instance = SingletonObject.getInstance<CricketViewSpineHelper>();
			CricketViewSpineHelper.DisplayData[] displayData = instance.GetDisplayData((int)id, skeleton, skin, isTripleTail);
			Slot[] slots = skeleton.Slots.Items;
			foreach (CricketViewSpineHelper.DisplayData data in displayData)
			{
				bool flag = data.SlotIndex < 0 || data.SlotIndex >= slots.Length;
				if (!flag)
				{
					Slot slot = slots[data.SlotIndex];
					slot.A = (float)(data.Active ? 1 : 0);
					bool active = data.Active;
					if (active)
					{
						skeleton.SetAttachment(slot.Data.Name, data.AttachmentName);
						bool flag2 = data.HideOtherSlot >= 0;
						if (flag2)
						{
							Slot hideSlot = slots[data.HideOtherSlot];
							hideSlot.A = 0f;
						}
					}
				}
			}
		}

		// Token: 0x060086D0 RID: 34512 RVA: 0x003EB0C4 File Offset: 0x003E92C4
		public CricketViewSpineHelper.DisplayData[] GetDisplayData(int id, Skeleton skeleton, Skin skin, bool isTripleTail)
		{
			string name = this.GetOrSetSkeletonName(skeleton.Data);
			CricketViewSpineHelper.SpineDataParser parser;
			bool flag = !this.spineDataParsers.TryGetValue(name, out parser);
			if (flag)
			{
				parser = new CricketViewSpineHelper.SpineDataParser(skeleton.Data);
				this.spineDataParsers[name] = parser;
			}
			else
			{
				bool flag2 = parser.SkeletonHash != skeleton.Data.Hash;
				if (flag2)
				{
					parser.Rebuild(skeleton.Data);
				}
			}
			parser.GetDisplayData(id, skin, this.resultDataBuffer);
			if (isTripleTail)
			{
				this.resultDataBuffer[58].Active = false;
			}
			return this.resultDataBuffer;
		}

		// Token: 0x060086D1 RID: 34513 RVA: 0x003EB174 File Offset: 0x003E9374
		public static int GetOffsetedAttachmentId(int slotIndex, int attachmentId)
		{
			return slotIndex * 1000 + attachmentId;
		}

		// Token: 0x060086D2 RID: 34514 RVA: 0x003EB190 File Offset: 0x003E9390
		public unsafe static int TryGetNumberFromAttachmentName(ReadOnlySpan<char> attachmentName, int[] number)
		{
			int numberCount = 0;
			int num = 0;
			bool hasNum = false;
			int multiplier = 1;
			for (int i = attachmentName.Length - 1; i >= 0; i--)
			{
				bool flag = numberCount >= number.Length;
				if (flag)
				{
					break;
				}
				char c = (char)(*attachmentName[i]);
				bool flag2 = char.IsDigit(c);
				if (flag2)
				{
					hasNum = true;
					num += (int)(c - '0') * multiplier;
					multiplier *= 10;
				}
				else
				{
					bool flag3 = hasNum;
					if (flag3)
					{
						number[numberCount] = num;
						num = 0;
						hasNum = false;
						numberCount++;
						multiplier = 1;
					}
					bool flag4 = c != '_';
					if (flag4)
					{
						break;
					}
				}
			}
			return numberCount;
		}

		// Token: 0x060086D3 RID: 34515 RVA: 0x003EB238 File Offset: 0x003E9438
		public static string GetSkinKey(Skin skin)
		{
			string skinKey = skin.Name;
			bool flag = string.IsNullOrEmpty(skinKey);
			if (flag)
			{
				skinKey = "___";
			}
			return skinKey;
		}

		// Token: 0x060086D4 RID: 34516 RVA: 0x003EB264 File Offset: 0x003E9464
		public string GetOrSetSkeletonName(SkeletonData skeletonData)
		{
			string name = skeletonData.Name;
			bool flag = string.IsNullOrEmpty(name);
			if (flag)
			{
				name = "___" + CricketViewSpineHelper.IdCounter++.ToString();
				skeletonData.Name = name;
			}
			return name;
		}

		// Token: 0x060086D6 RID: 34518 RVA: 0x003EB2C8 File Offset: 0x003E94C8
		// Note: this type is marked as 'beforefieldinit'.
		static CricketViewSpineHelper()
		{
			Dictionary<ECricketSlot, ECricketSlot> dictionary = new Dictionary<ECricketSlot, ECricketSlot>();
			dictionary[ECricketSlot.Trophi_lmask] = ECricketSlot.Trophi_l;
			dictionary[ECricketSlot.Trophi_rmask] = ECricketSlot.Trophi_r;
			dictionary[ECricketSlot.FrontLeg_lmask] = ECricketSlot.FrontLeg_l;
			dictionary[ECricketSlot.FrontLeg_rmask] = ECricketSlot.FrontLeg_r;
			dictionary[ECricketSlot.BackLeg_lmask] = ECricketSlot.BackLeg_l;
			dictionary[ECricketSlot.BackLeg_rmask] = ECricketSlot.BackLeg_r;
			dictionary[ECricketSlot.BackPaw_lmask] = ECricketSlot.BackPaw_l;
			dictionary[ECricketSlot.BackPaw_rmask] = ECricketSlot.BackPaw_r;
			dictionary[ECricketSlot.Leg_lmask] = ECricketSlot.Leg_l;
			dictionary[ECricketSlot.Leg_rmask] = ECricketSlot.Leg_r;
			CricketViewSpineHelper.ShowOnlyDefaultSlot = dictionary;
			Dictionary<ECricketSlot, ECricketSlot> dictionary2 = new Dictionary<ECricketSlot, ECricketSlot>();
			dictionary2[ECricketSlot.Trophi_m] = ECricketSlot.Trophi_l;
			CricketViewSpineHelper.HideOtherSlot = dictionary2;
		}

		// Token: 0x04006790 RID: 26512
		private static int IdCounter = 0;

		// Token: 0x04006791 RID: 26513
		public const int AttachmentIdOffset = 1000;

		// Token: 0x04006792 RID: 26514
		public static readonly string[] SlotTypeToNameMap = new string[59];

		// Token: 0x04006793 RID: 26515
		private static readonly int[] numberParseBuffer = new int[4];

		// Token: 0x04006794 RID: 26516
		private static readonly HashSet<ECricketSlot> showDefaultSlotList = new HashSet<ECricketSlot>();

		// Token: 0x04006795 RID: 26517
		private readonly Dictionary<string, CricketViewSpineHelper.SpineDataParser> spineDataParsers = new Dictionary<string, CricketViewSpineHelper.SpineDataParser>();

		// Token: 0x04006796 RID: 26518
		private CricketViewSpineHelper.DisplayData[] resultDataBuffer;

		// Token: 0x04006797 RID: 26519
		private static readonly HashSet<ECricketSlot> OptionalSlots = new HashSet<ECricketSlot>
		{
			ECricketSlot.Cirrus_l2,
			ECricketSlot.Cirrus_r2,
			ECricketSlot.Trophi_l2,
			ECricketSlot.Trophi_r2,
			ECricketSlot.Trophi_m,
			ECricketSlot.WingLmask,
			ECricketSlot.WingRmask,
			ECricketSlot.Wing_l_1,
			ECricketSlot.Wing_r_1,
			ECricketSlot.WingLmask_1,
			ECricketSlot.WingRmask_1,
			ECricketSlot.Wing_l_2,
			ECricketSlot.Wing_r_2,
			ECricketSlot.WingLmask_2,
			ECricketSlot.WingRmask_2,
			ECricketSlot.MidLeg_l,
			ECricketSlot.MidLeg_r,
			ECricketSlot.MidPaw_l,
			ECricketSlot.MidPaw_r,
			ECricketSlot.MidLeg_lmask,
			ECricketSlot.MidLeg_rmask,
			ECricketSlot.MidPaw_lmask,
			ECricketSlot.MidPaw_rmask
		};

		// Token: 0x04006798 RID: 26520
		private static readonly Dictionary<ECricketSlot, ECricketSlot> ShowOnlyDefaultSlot;

		// Token: 0x04006799 RID: 26521
		private static readonly Dictionary<ECricketSlot, ECricketSlot> HideOtherSlot;

		// Token: 0x0200207A RID: 8314
		public struct DisplayData
		{
			// Token: 0x0600F757 RID: 63319 RVA: 0x00629068 File Offset: 0x00627268
			public override string ToString()
			{
				return string.Format("[id:{0}, attach:{1}, active:{2}], hideOther:{3}", new object[]
				{
					this.SlotIndex,
					this.AttachmentName,
					this.Active,
					this.HideOtherSlot
				});
			}

			// Token: 0x0400D133 RID: 53555
			public int SlotIndex;

			// Token: 0x0400D134 RID: 53556
			public string AttachmentName;

			// Token: 0x0400D135 RID: 53557
			public bool Active;

			// Token: 0x0400D136 RID: 53558
			public int HideOtherSlot;
		}

		// Token: 0x0200207B RID: 8315
		public class SpineDataParser
		{
			// Token: 0x0600F758 RID: 63320 RVA: 0x006290BD File Offset: 0x006272BD
			public SpineDataParser(SkeletonData skeletonData)
			{
				this.Rebuild(skeletonData);
			}

			// Token: 0x0600F759 RID: 63321 RVA: 0x006290E8 File Offset: 0x006272E8
			public void Rebuild(SkeletonData skeletonData)
			{
				this.SkinAttachmentMap.Clear();
				this.SkeletonHash = skeletonData.Hash;
				for (int slotType = 0; slotType < 59; slotType++)
				{
					string slotName = CricketViewSpineHelper.SlotTypeToNameMap[slotType];
					SlotData slot = skeletonData.FindSlot(slotName);
					this.SlotTypeToIndexMap[slotType] = ((slot != null) ? slot.Index : -1);
				}
				bool flag = skeletonData.DefaultSkin != null;
				if (flag)
				{
					string skinKey = CricketViewSpineHelper.GetSkinKey(skeletonData.DefaultSkin);
					this.SkinAttachmentMap[skinKey] = new CricketViewSpineHelper.AttachmentDataParser(skeletonData.DefaultSkin);
				}
			}

			// Token: 0x0600F75A RID: 63322 RVA: 0x0062917C File Offset: 0x0062737C
			public void GetDisplayData(int id, Skin skin, CricketViewSpineHelper.DisplayData[] result)
			{
				string skinKey = CricketViewSpineHelper.GetSkinKey(skin);
				CricketViewSpineHelper.AttachmentDataParser attachmentDataParser;
				bool flag = !this.SkinAttachmentMap.TryGetValue(skinKey, out attachmentDataParser);
				if (flag)
				{
					attachmentDataParser = new CricketViewSpineHelper.AttachmentDataParser(skin);
					this.SkinAttachmentMap[skinKey] = attachmentDataParser;
				}
				int index = 0;
				CricketViewSpineHelper.showDefaultSlotList.Clear();
				for (ECricketSlot slotType = ECricketSlot.Head; slotType < ECricketSlot.Count; slotType++)
				{
					int slotIndex = this.SlotTypeToIndexMap[(int)slotType];
					bool flag2 = slotIndex == -1;
					if (flag2)
					{
						result[index] = new CricketViewSpineHelper.DisplayData
						{
							SlotIndex = -1
						};
						index++;
					}
					else
					{
						CricketViewSpineHelper.DisplayData data = new CricketViewSpineHelper.DisplayData
						{
							SlotIndex = slotIndex,
							Active = true,
							AttachmentName = null,
							HideOtherSlot = -1
						};
						int attachmentId = CricketViewSpineHelper.GetOffsetedAttachmentId(data.SlotIndex, id);
						string attachmentName;
						bool flag3 = !attachmentDataParser.AttachmentIdToNameMap.TryGetValue(attachmentId, out attachmentName);
						if (flag3)
						{
							bool flag4 = CricketViewSpineHelper.OptionalSlots.Contains(slotType);
							if (flag4)
							{
								data.Active = false;
							}
							int defaultAttachmentId = CricketViewSpineHelper.GetOffsetedAttachmentId(data.SlotIndex, -1);
							bool flag5 = attachmentDataParser.AttachmentIdToNameMap.TryGetValue(defaultAttachmentId, out attachmentName);
							if (flag5)
							{
								CricketViewSpineHelper.showDefaultSlotList.Add(slotType);
							}
						}
						ECricketSlot dependSlot;
						bool flag6 = CricketViewSpineHelper.ShowOnlyDefaultSlot.TryGetValue(slotType, out dependSlot) && !CricketViewSpineHelper.showDefaultSlotList.Contains(dependSlot);
						if (flag6)
						{
							data.Active = false;
						}
						ECricketSlot hideSlot;
						bool flag7 = data.Active && CricketViewSpineHelper.HideOtherSlot.TryGetValue(slotType, out hideSlot);
						if (flag7)
						{
							data.HideOtherSlot = this.SlotTypeToIndexMap[(int)hideSlot];
							CricketViewSpineHelper.showDefaultSlotList.Remove(hideSlot);
						}
						data.AttachmentName = attachmentName;
						result[index] = data;
						index++;
					}
				}
			}

			// Token: 0x0400D137 RID: 53559
			public string SkeletonHash;

			// Token: 0x0400D138 RID: 53560
			public readonly int[] SlotTypeToIndexMap = new int[59];

			// Token: 0x0400D139 RID: 53561
			public readonly Dictionary<string, CricketViewSpineHelper.AttachmentDataParser> SkinAttachmentMap = new Dictionary<string, CricketViewSpineHelper.AttachmentDataParser>();
		}

		// Token: 0x0200207C RID: 8316
		public class AttachmentDataParser
		{
			// Token: 0x0600F75B RID: 63323 RVA: 0x00629350 File Offset: 0x00627550
			public AttachmentDataParser(Skin skinData)
			{
				ICollection<Skin.SkinEntry> attachments = skinData.Attachments;
				foreach (Skin.SkinEntry entry in attachments)
				{
					bool flag = string.IsNullOrEmpty(entry.Name);
					if (!flag)
					{
						int count = CricketViewSpineHelper.TryGetNumberFromAttachmentName(entry.Name, CricketViewSpineHelper.numberParseBuffer);
						bool flag2 = count == 0;
						if (flag2)
						{
							bool flag3 = entry.Name[entry.Name.Length - 1] == 'c';
							if (flag3)
							{
								int id = CricketViewSpineHelper.GetOffsetedAttachmentId(entry.SlotIndex, -1);
								this.AttachmentIdToNameMap[id] = entry.Name;
							}
						}
						else
						{
							int id2 = CricketViewSpineHelper.GetOffsetedAttachmentId(entry.SlotIndex, CricketViewSpineHelper.numberParseBuffer[count - 1]);
							this.AttachmentIdToNameMap[id2] = entry.Name;
						}
					}
				}
			}

			// Token: 0x0400D13A RID: 53562
			public readonly Dictionary<int, string> AttachmentIdToNameMap = new Dictionary<int, string>();
		}
	}
}
