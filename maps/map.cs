using RpgGame.Global.FuncLos;

namespace RpgGame.Map;

using Godot;
using Global;
using Dictionary = Godot.Collections.Dictionary;
using Array = Godot.Collections.Array;
using System.Collections.Generic;

public partial class map : Node2D
{
	private CustomSignals _customSignals;
	private List<Vector2> _markedTilesFromLastRound = new List<Vector2>();
	private Dictionary<string, Vector2> _npcs = new Dictionary<string, Vector2>();
	
	public override void _Ready()
	{
		_customSignals = GetNode<CustomSignals>("/root/CustomSignals"); 
		SetMapDataInGlobalData();	
		SetupPlayer();	
		//TODO: SetupNpcs()
		ConnectSignals();
		InitalizeLos();
	}

	private void InitalizeLos()
	{
		var tileMap = GetNode<TileMap>("GeoMap");
		Data.ActiveMapSize = tileMap.GetUsedRect().Size;
		FuncLos.CoverVisibilityTilemap(GetNode<TileMap>("VisMap"), Data.ActiveMapSize);
		FuncLos.DisableAllSprites(GetNode<TileMap>("GeoMap"));
		RevealLos(Data.CurrentPlayerPos, GetNode("Player").GetNode("LosRayCast") as RayCast2D);
	}

	private void RevealLos(Vector2 playerPos, RayCast2D collRayCast2D)
	{
		ClearTilesFromLastRound();
		
		_markedTilesFromLastRound = FuncLos.UpdateLos(
			Data.CurrentPlayerPos,
			collRayCast2D,
			FuncMap.FindTilemapNode(GetParent(), "VisMap") as TileMap);
	}
	
	public void ClearTilesFromLastRound()
	{
		foreach (Vector2 tile in _markedTilesFromLastRound)
		{
			FuncMap.HideTile(tile, FuncMap.FindTilemapNode(GetParent(), "VisMap") as TileMap); 
			
			foreach (var npc in _npcs) 
			{
				// Access NPC position assuming specific structure
				var npcPosition = npc.Value;

				if (npcPosition == tile)
				{
					Sprite2D sprite = GetNode<Sprite2D>(npc.Key); 
					if (sprite != null) // Check for null before accessing
					{
						sprite.Visible = false;
					}
				}
			}
		}
		_markedTilesFromLastRound.Clear();
	}
	
	private void SetMapDataInGlobalData()
	{		
		string nodeName = Name;
		var jsonData = ExternalData.LoadJsonFile("maps/mapData/", nodeName.ToLower() + ".json");
		 
		var jsonLoader = new Json();
		var error = jsonLoader.Parse(jsonData);
		CheckJsonDataForError(error);
		var mapDict = (Dictionary)jsonLoader.Data;
		
		
		Data.ActiveMapName = (string)mapDict["name"];
		Data.ActiveMapType = (string)mapDict["type"];
		Data.ActiveMapNpcs = StructureMapPoi((Array)mapDict["npc_data"]);
		Data.ActiveMapConnections = StructureMapPoi((Array)mapDict["map_connection_data"]);
		Data.ActiveMapDoors = StructureMapPoi((Array)mapDict["doors"]);
		Data.ActiveMapPlayerStartPos = (Vector2)mapDict["player_start_position"];
	}

	private Dictionary<string, Vector2>  StructureMapPoi(Array rawData){
		var mapConnectionPairs = new Dictionary<string, Vector2>();
		
		foreach (Dictionary conData in rawData)
		{
			var locName = (string)conData["name"];
			var positionArray = (Array)conData["position"];
			var posX = (int)positionArray[0];
			var posY = (int)positionArray[1];
			var position = new Vector2(posX, posY);
			mapConnectionPairs[locName] = position;
		}
		return mapConnectionPairs;
	}

	private void SetupPlayer()
	{
		var sceneRoot = GetParent().GetNode(Data.ActiveMapName);
		Scenes.AddNode("res://entities/player/player.tscn", sceneRoot);

		if(Data.ActiveMapName == Data.OverworldName){
			GetNode<Area2D>("Player").Position = Data.LastPosOnOverworld + Data.MapOffset;
		}
		else{
			GetNode<Area2D>("Player").Position = Data.ActiveMapPlayerStartPos + Data.MapOffset;
		}
	}

	private void ConnectSignals()
	{	
		_customSignals.PlayerMoved += OnPlayerMoved;              
		_customSignals.NewMapEntered  += OnPlayerHasEnteredNewMap;              	
	}

	private void OnPlayerMoved(Vector2 position, RayCast2D rayCastLos)
	{
		//GD.Print("player moved");
		RevealLos(Data.CurrentPlayerPos, rayCastLos);
	}


	private void OnPlayerHasEnteredNewMap(string newMapName)
	{
		GD.Print("player has entered new map");
		Data.NextMap = newMapName;
		GetTree().ChangeSceneToFile("res://ui/ui.tscn");
	}

	private static void CheckJsonDataForError(Error error)
	{
		if (error != Error.Ok)
		{
			GD.Print(error);
			return;
		}
	}
}
