using System;
using System.Linq;
using Terraria.ModLoader.IO;

namespace QuantumStorage;

public readonly struct Frequency
{
	private readonly Colors[] colors;

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

	public override int GetHashCode() => int.Parse($"{(byte)colors[0]}{(byte)colors[1]}{(byte)colors[2]}");

	public Frequency Clone() => new Frequency(colors);

	public override bool Equals(object obj)
	{
		if (obj is Frequency freq) return freq.colors.SequenceEqual(colors);
		return false;
	}
}

public class FrequencySerializer : TagSerializer<Frequency, TagCompound>
{
	public override TagCompound Serialize(Frequency value) => new()
	{
		["Color1"] = (byte)value[0],
		["Color2"] = (byte)value[1],
		["Color3"] = (byte)value[2]
	};

	public override Frequency Deserialize(TagCompound tag)
	{
		try
		{
			return new Frequency((Colors)tag.GetByte("Color1"), (Colors)tag.GetByte("Color2"), (Colors)tag.GetByte("Color3"));
		}
		catch
		{
			return new Frequency();
		}
	}
}