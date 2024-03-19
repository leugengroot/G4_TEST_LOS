namespace RpgGame.Global;



using Godot;

public partial class UI : Control
{	
	public override void _Ready()
	{
		// get the container which holds the current tilemap
		var container = GetNode("MainContainer/TileMapContainer/TileMapViewport");	

		string mapToLoad;

		switch (Data.NextMap)
		{
			case "combatmap_test":
				mapToLoad = "res://maps/combat/combatmap_test.tscn"; 
			break;
			
			case "festum":
				mapToLoad = "res://maps/cities/festum/festum.tscn";
				break;
			
			default:
				mapToLoad = "res://maps/regions/bornland.tscn";
				break;			
		}

		// Load and add the map instance
		var mapScene = ResourceLoader.Load<PackedScene>(mapToLoad);
		var mapInstance = mapScene.Instantiate();
		container.AddChild(mapInstance);
	
	}

	public override void _Process(double delta)
	{
	}
}
