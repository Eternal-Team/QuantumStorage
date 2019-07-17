using QuantumStorage.Global;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace QuantumStorage
{
	public class QuantumStorage : Mod
	{
		public override void Load()
		{
			Utility.Initialize();

			TagSerializer.AddSerializer(new FrequencySerializer());
		}

		public override void Unload() => BaseLibrary.Utility.UnloadNullableTypes();
	}
}