namespace RpgGame.Global;

using System.Collections.Generic;
using System.Linq;



using Godot;

public partial class FuncMap : Node
{
	public static string IsTileBlocking(Vector2 coords, TileMap map)
	{
		var tileData = map.GetCellTileData(0, (Vector2I)coords, false);

		if (tileData == null) return null;
		var test = (string)tileData.GetCustomData("block_movement");

		return (string)tileData.GetCustomData("block_movement");

	}
	
	public static Node FindTilemapNode(Node parentNode, string tilemapName)
	{
		// Check if the parent node itself is the tilemap
		if (parentNode is TileMap && parentNode.Name == tilemapName)
		{
			return parentNode;
		}

		// Recursively search child nodes for the tilemap
		var nodes = parentNode.GetChildren();
		for (var index = 0; index < nodes.Count; index++)
		{
			var child = nodes[index];
			var foundTilemap = FindTilemapNode(child, tilemapName);
			if (foundTilemap != null)
			{
				return foundTilemap;
			}
		}
		return null;
	}
	
	public static string GetCustomTileData(TileMap tileMap, Vector2 coords, string customData)
	{
		return tileMap.GetCellTileData(0, (Vector2I)coords).GetCustomData(customData).ToString();
	}

	public static void OpenDoor(Vector2 pos, Vector2 dir, TileMap tileMap)
	{
		tileMap.SetCell(0, new Vector2I((int)pos.X + (int)dir.X, (int)pos.Y + (int)dir.Y), 1, new Vector2I(14,3));
	}

	public static void CloseDoorsAfterPlayer(Dictionary<string, Vector2> doors, TileMap map)
	{
		if (Data.ActiveMapName == Data.OverworldName) return;
		var playerPos = Data.CurrentPlayerPos;

		// loop through all doors in map and check if player is more than 3 tiles away and close them if necessary
		foreach (var doorPos in from door in doors select new Vector2((float)door.Value[0], (float)door.Value[1]) into doorPos where playerPos != doorPos where GetCustomTileData(map, doorPos, "type") != "door1" where Mathf.Abs(doorPos.X - playerPos.X) > 3 ||
			         Mathf.Abs(doorPos.Y - playerPos.Y) > 3 select doorPos)
		{
			map.SetCell(0, new Vector2I((int)doorPos.X, (int)doorPos.Y), 1, new Vector2I(10,3));
		}
	}
	
	public static void ClearTile(Vector2 pos, TileMap map)
	{
		map.SetCell(0, (Vector2I)pos, 1, new Vector2I(0,0));
	}
	
	public static void HideTile(Vector2 pos, TileMap map)
	{
		//use Vector2i(0, 0) for black tiles
		map.SetCell(0, (Vector2I)pos, 0, new Vector2I(0,0));
	}
}
