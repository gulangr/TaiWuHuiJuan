using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork;
using UnityEngine;

namespace Game.Components.Character
{
	// Token: 0x02000F44 RID: 3908
	public class TeammateCommands : MonoBehaviour
	{
		// Token: 0x0600B371 RID: 45937 RVA: 0x0051AA48 File Offset: 0x00518C48
		public void Set(int attackMedal, int defenceMedal, int wisdomMedal, List<sbyte> commandList, List<sbyte> replacedCommands)
		{
			bool flag = this.commands == null;
			if (!flag)
			{
				bool flag2 = commandList == null;
				if (flag2)
				{
					foreach (TeammateCommand command in this.commands)
					{
						command.gameObject.SetActive(false);
					}
					this.noneCommandTitleGo.SetActive(true);
				}
				else
				{
					Dictionary<int, int> leftMedalDict = EasyPool.Get<Dictionary<int, int>>();
					leftMedalDict.Clear();
					leftMedalDict.Add(0, Math.Abs(attackMedal));
					leftMedalDict.Add(1, Math.Abs(defenceMedal));
					leftMedalDict.Add(2, Math.Abs(wisdomMedal));
					bool flag3;
					if (replacedCommands != null)
					{
						flag3 = replacedCommands.Any((sbyte c) => c >= 0 && TeammateCommand.Instance[c].Type == ETeammateCommandType.Negative);
					}
					else
					{
						flag3 = false;
					}
					List<sbyte> commandsToUse = flag3 ? replacedCommands : commandList;
					bool hasCommand = false;
					for (int i = 0; i < 3; i++)
					{
						int cmdType = (int)((i < commandsToUse.Count) ? commandsToUse[i] : -1);
						bool flag4 = cmdType < 0;
						if (flag4)
						{
							this.commands[i].gameObject.SetActive(false);
						}
						else
						{
							hasCommand = true;
							this.commands[i].gameObject.SetActive(true);
							this.commands[i].Set(cmdType, leftMedalDict);
							TeammateCommandItem config = TeammateCommand.Instance[cmdType];
							bool flag5 = config.MedalType >= 0 && leftMedalDict.ContainsKey((int)config.MedalType);
							if (flag5)
							{
								Dictionary<int, int> dictionary = leftMedalDict;
								int medalType = (int)config.MedalType;
								dictionary[medalType] -= (int)config.MedalCount;
							}
						}
					}
					this.noneCommandTitleGo.SetActive(!hasCommand);
					EasyPool.Free<Dictionary<int, int>>(leftMedalDict);
				}
			}
		}

		// Token: 0x0600B372 RID: 45938 RVA: 0x0051AC1C File Offset: 0x00518E1C
		public void ChangeCommandsActive(bool active)
		{
			for (int i = this.commands.Length - 1; i >= 0; i--)
			{
				TeammateCommand c = this.commands[i];
				c.gameObject.SetActive(active);
			}
			bool flag = this.noneCommandTitleGo != null;
			if (flag)
			{
				this.noneCommandTitleGo.SetActive(!active);
			}
		}

		// Token: 0x04008B60 RID: 35680
		[SerializeField]
		protected TeammateCommand[] commands;

		// Token: 0x04008B61 RID: 35681
		[SerializeField]
		private GameObject noneCommandTitleGo;
	}
}
