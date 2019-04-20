using QuantumStorage.Global;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace QuantumStorage
{
	public class QuantumStorage : Mod
	{
		public override void Load()
		{
			TagSerializer.AddSerializer(new FrequencySerializer());
		}
	}
}