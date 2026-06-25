using System;
using UnityEngine;

namespace Game.Views.Migrate
{
	// Token: 0x0200090C RID: 2316
	public class WorldMapAdditionalEffectsInfo : MonoBehaviour
	{
		// Token: 0x06006D92 RID: 28050 RVA: 0x0032B0F8 File Offset: 0x003292F8
		public GameObject GetTargetGameObject(string targetTag)
		{
			GameObject result;
			if (!(targetTag == "3_alter"))
			{
				if (!(targetTag == "2_alter"))
				{
					if (!(targetTag == "1_alter"))
					{
						if (!(targetTag == "1"))
						{
							if (!(targetTag == "2"))
							{
								if (!(targetTag == "3"))
								{
									Debug.LogError("Can't Find Target GameObject By Tag (" + targetTag + ") !");
									result = null;
								}
								else
								{
									result = this.go3;
								}
							}
							else
							{
								result = this.go2;
							}
						}
						else
						{
							result = this.go1;
						}
					}
					else
					{
						result = this.goAlter1;
					}
				}
				else
				{
					result = this.goAlter2;
				}
			}
			else
			{
				result = this.goAlter3;
			}
			return result;
		}

		// Token: 0x0400508D RID: 20621
		public GameObject go3;

		// Token: 0x0400508E RID: 20622
		public GameObject go2;

		// Token: 0x0400508F RID: 20623
		public GameObject goBloodLight;

		// Token: 0x04005090 RID: 20624
		public GameObject goAlter3;

		// Token: 0x04005091 RID: 20625
		public GameObject goAlter2;

		// Token: 0x04005092 RID: 20626
		public GameObject go1;

		// Token: 0x04005093 RID: 20627
		public GameObject goAlter1;

		// Token: 0x02001DE7 RID: 7655
		private static class GoTag
		{
			// Token: 0x0400C7D6 RID: 51158
			public const string TagAlter1 = "1_alter";

			// Token: 0x0400C7D7 RID: 51159
			public const string TagAlter2 = "2_alter";

			// Token: 0x0400C7D8 RID: 51160
			public const string TagAlter3 = "3_alter";

			// Token: 0x0400C7D9 RID: 51161
			public const string Tag1 = "1";

			// Token: 0x0400C7DA RID: 51162
			public const string Tag2 = "2";

			// Token: 0x0400C7DB RID: 51163
			public const string Tag3 = "3";
		}
	}
}
