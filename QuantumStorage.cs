using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace QuantumStorage;

public class QuantumStorage : Mod
{
	public const string AssetPath = "QuantumStorage/Assets/";
	public const string TexturePath = AssetPath + "Textures/";

	internal static Asset<Texture2D> TextureGemsSide;
	internal static Asset<Texture2D> TextureGemsMiddle;
	internal static Asset<Texture2D> TextureRingSmall;
	internal static Asset<Texture2D> TextureRingBig;

	public override void Load()
	{
		if (Main.dedServ) return;

		TextureGemsSide = ModContent.Request<Texture2D>(TexturePath + "Tiles/GemSide_0");
		TextureGemsMiddle = ModContent.Request<Texture2D>(TexturePath + "Tiles/GemMiddle_0");
		TextureRingSmall = ModContent.Request<Texture2D>(TexturePath + "Items/RingSmall");
		TextureRingBig = ModContent.Request<Texture2D>(TexturePath + "Items/RingBig");
	}

	// public override void HandlePacket(BinaryReader reader, int whoAmI) => Net.HandlePacket(reader, whoAmI);
}