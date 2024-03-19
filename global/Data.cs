namespace RpgGame.Global;

using System.Collections.Generic;
using Godot;

public partial class Data : Node
{
	#region consts
	public const string OverworldName = "Bornland";
	public const int TileSize = 16;
	public const int Visibility = 3;
	#endregion

	#region misc
	public static Vector2 MapOffset = new(TileSize / 2, TileSize / 2);
	private static readonly Vector2 PlayerStartPos = new Vector2(13,25) * TileSize;
	#endregion

	#region enums and dictionaries
	public enum Mode
	{
		MOVE,
		TALK,
		OPEN,
		ATTACK,
		ZSTATS
	}

	public enum MapMode{OVERWORLD, COMBAT, CITY}

	public static readonly Dictionary<string, Vector2> DirectionInputs = new Dictionary<string, Vector2>()
	{
		{ "RIGHT_ARROW", Vector2.Right },
		{ "LEFT_ARROW", Vector2.Left },
		{ "UP_ARROW", Vector2.Up },
		{ "DOWN_ARROW", Vector2.Down }
	};
	#endregion

	public static string NextMap { get; set; }
	public static Vector2 LastPosOnOverworld { get; set; } = PlayerStartPos;
	public static Mode PlayerMode { get; set; }
	public static Vector2 CurrentPlayerPos { get; set; }
	
	// active map
	public static string  ActiveMapName { get; set; }
	public static string  ActiveMapType { get; set; }
	public static Dictionary<string, Vector2>  ActiveMapConnections { get; set; }
	public static Dictionary<string, Vector2>  ActiveMapNpcs { get; set; }
	public static Dictionary<string, Vector2>  ActiveMapDoors { get; set; }
	public static Vector2  ActiveMapSize { get; set; }
	public static Vector2 ActiveMapPlayerStartPos { get; set; }
}
