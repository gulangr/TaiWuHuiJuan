using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace FrameWork.Tools.JsonConverter
{
	// Token: 0x02001039 RID: 4153
	public class JsonConverterVector2 : JsonConverter<Vector2>
	{
		// Token: 0x0600BDBF RID: 48575 RVA: 0x00562664 File Offset: 0x00560864
		public override void WriteJson(JsonWriter writer, Vector2 value, JsonSerializer serializer)
		{
			JObject jObject = new JObject
			{
				{
					"x",
					value.x
				},
				{
					"y",
					value.y
				}
			};
			serializer.Serialize(writer, jObject);
		}

		// Token: 0x0600BDC0 RID: 48576 RVA: 0x005626B0 File Offset: 0x005608B0
		public override Vector2 ReadJson(JsonReader reader, Type objectType, Vector2 existingValue, bool hasExistingValue, JsonSerializer serializer)
		{
			JObject jObject = serializer.Deserialize<JObject>(reader);
			return new Vector2(jObject.Value<float>("x"), jObject.Value<float>("y"));
		}
	}
}
