using System;
using System.Collections.Generic;
using GameData.Domains.Character;
using GameData.Domains.Item;
using GameData.Domains.Taiwu;
using GameData.Domains.Taiwu.Display;
using TMPro;
using UnityEngine;

namespace Game.Views.Villager
{
	// Token: 0x02000743 RID: 1859
	public class VillagerDetailSummary : MonoBehaviour
	{
		// Token: 0x06005A2A RID: 23082 RVA: 0x0029D504 File Offset: 0x0029B704
		public unsafe void RefreshAsFarmer()
		{
			for (int i = 0; i < 6; i++)
			{
				this.things[i].gameObject.SetActive(true);
				this.things[i].text = LocalStringManager.GetFormat(this.famerKeys[i], *this.MaterialResourceCount[i], *this.MigratorCount[i]);
				this.things[i].GetComponent<TMPTextSpriteHelper>().Parse();
			}
			for (int j = 6; j < this.things.Length; j++)
			{
				this.things[j].gameObject.SetActive(false);
			}
			base.gameObject.SetActive(true);
		}

		// Token: 0x06005A2B RID: 23083 RVA: 0x0029D5C4 File Offset: 0x0029B7C4
		public void RefreshAsMerchant()
		{
			for (int i = 0; i < 12; i++)
			{
				this.things[i].gameObject.SetActive(true);
				this.things[i].text = LocalStringManager.GetFormat(this.merchantKeys[i], this.MerchantCount[i]);
				this.things[i].GetComponent<TMPTextSpriteHelper>().Parse();
			}
			base.gameObject.SetActive(true);
		}

		// Token: 0x06005A2C RID: 23084 RVA: 0x0029D644 File Offset: 0x0029B844
		public unsafe void ReadData(TaiwuVillagerRoleDisplayData data)
		{
			this.MaterialResourceCount.Initialize();
			this.CollectorCount.Initialize();
			this.MigratorCount.Initialize();
			Array.Fill<int>(this.MerchantCount, 0);
			int num;
			if (data != null)
			{
				Dictionary<int, VillagerRoleCharacterDisplayData> villagers = data.Villagers;
				if (villagers != null)
				{
					num = ((villagers.Count > 0) ? 1 : 0);
					goto IL_4B;
				}
			}
			num = 0;
			IL_4B:
			bool flag = num == 0;
			if (!flag)
			{
				foreach (VillagerRoleCharacterDisplayData villager in data.Villagers.Values)
				{
					VillagerWorkData work = villager.VillagerWorkData;
					if (work == null)
					{
						goto IL_A2;
					}
					sbyte workType = work.WorkType;
					if (workType != 10 && workType != 14)
					{
						goto IL_A2;
					}
					bool flag2 = true;
					IL_A5:
					bool flag3 = flag2;
					if (flag3)
					{
						bool flag4 = work.WorkType == 10;
						if (flag4)
						{
							*this.MaterialResourceCount[(int)work.ResourceType] += (int)((short)villager.CollectResourceAmount);
							(*this.CollectorCount[(int)work.ResourceType])++;
						}
						else
						{
							(*this.MigratorCount[(int)work.ResourceType])++;
						}
						continue;
					}
					VillagerRoleArrangementDisplayDataWrapper arrangementDisplayData = villager.ArrangementDisplayData;
					bool flag5 = arrangementDisplayData != null && arrangementDisplayData.ArrangementTemplateId == 8;
					if (flag5)
					{
						int[] merchantCount = this.MerchantCount;
						TemplateKey itemTemplateKey = villager.ItemTemplateKey;
						if (!true)
						{
						}
						sbyte b;
						short num2;
						itemTemplateKey.Deconstruct(out b, out num2);
						int num3;
						switch (b)
						{
						case -1:
							if (num2 != -1)
							{
								goto IL_242;
							}
							num3 = 0;
							break;
						case 0:
							if (num2 != -1)
							{
								goto IL_242;
							}
							num3 = 8;
							break;
						case 1:
							if (num2 != -1)
							{
								goto IL_242;
							}
							num3 = 9;
							break;
						case 2:
							if (num2 != 200)
							{
								goto IL_242;
							}
							num3 = 7;
							break;
						case 3:
						case 4:
							goto IL_242;
						case 5:
							if (num2 != -1)
							{
								goto IL_242;
							}
							num3 = 1;
							break;
						case 6:
							if (num2 != -1)
							{
								goto IL_242;
							}
							num3 = 6;
							break;
						case 7:
							if (num2 != -1)
							{
								goto IL_242;
							}
							num3 = 4;
							break;
						case 8:
							if (num2 != 800)
							{
								if (num2 != 801)
								{
									goto IL_242;
								}
								num3 = 3;
							}
							else
							{
								num3 = 2;
							}
							break;
						case 9:
							if (num2 != -1)
							{
								goto IL_242;
							}
							num3 = 5;
							break;
						case 10:
							if (num2 != 1000)
							{
								if (num2 != 1001)
								{
									goto IL_242;
								}
								num3 = 10;
							}
							else
							{
								num3 = 11;
							}
							break;
						default:
							goto IL_242;
						}
						IL_248:
						if (!true)
						{
						}
						ref int ptr = ref merchantCount[num3];
						ref int ptr2 = ref ptr;
						int num4 = ptr;
						ptr2 = num4 + 1;
						continue;
						IL_242:
						num3 = 12;
						goto IL_248;
					}
					continue;
					IL_A2:
					flag2 = false;
					goto IL_A5;
				}
			}
		}

		// Token: 0x04003E0D RID: 15885
		private const int MerchantTogCount = 12;

		// Token: 0x04003E0E RID: 15886
		[SerializeField]
		private TMP_Text[] things;

		// Token: 0x04003E0F RID: 15887
		[SerializeField]
		private string[] famerKeys;

		// Token: 0x04003E10 RID: 15888
		[SerializeField]
		private string[] merchantKeys;

		// Token: 0x04003E11 RID: 15889
		[NonSerialized]
		public ResourceInts MaterialResourceCount;

		// Token: 0x04003E12 RID: 15890
		[NonSerialized]
		public ResourceInts CollectorCount;

		// Token: 0x04003E13 RID: 15891
		[NonSerialized]
		public ResourceInts MigratorCount;

		// Token: 0x04003E14 RID: 15892
		[NonSerialized]
		public int[] MerchantCount = new int[13];
	}
}
