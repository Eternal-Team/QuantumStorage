using System;
using System.Linq;
using Terraria.ModLoader.IO;

namespace QuantumStorage
{
	public class Frequency : ICloneable
	{
		public Colors[] colors;

		public bool IsSet => colors[0] != Colors.None && colors[1] != Colors.None && colors[2] != Colors.None;

		public Frequency()
		{
			colors = new[] { Colors.None, Colors.None, Colors.None };
		}

		public Frequency(params Colors[] colors) => this.colors = new[] { colors[0], colors[1], colors[2] };

		public Colors this[int index]
		{
			get => colors[index];
			set => colors[index] = value;
		}

		public override int GetHashCode() => int.Parse($"{(int)colors[0]}{(int)colors[1]}{(int)colors[2]}");

		public object Clone() => new Frequency(colors);

		public override bool Equals(object obj)
		{
			if (obj is Frequency freq) return freq.colors.SequenceEqual(colors);
			return false;
		}
	}

	public class FrequencySerializer : TagSerializer<Frequency, TagCompound>
	{
		public override TagCompound Serialize(Frequency value) => new TagCompound
		{
			["Value"] = value.colors.Select(x => (int)x).ToList()
		};

		public override Frequency Deserialize(TagCompound tag)
		{
			try
			{
				return new Frequency(tag.GetList<int>("Value").Select(x => (Colors)x).ToArray());
			}
			catch
			{
				return new Frequency();
			}
		}
	}
}