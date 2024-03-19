namespace RpgGame.Entities.Player;

using System.Collections.Generic;
using Godot;
using Global;
using Vector2 = Godot.Vector2;

public partial class Player : Area2D
{
	private RayCast2D _collisionRayCast;
	private RayCast2D _losRayCast;
	private TileMap _geoTileMap;
	private Dictionary<string, Vector2> _doorPositions;
	private CustomSignals _customSignals;

	public override void _Ready()
	{
		_customSignals = GetNode<CustomSignals>("/root/CustomSignals");
		_collisionRayCast = GetNode<RayCast2D>("CollisionRayCast");
		_losRayCast = GetNode<RayCast2D>("LosRayCast");
		_geoTileMap = GetParent().GetNode<TileMap>("GeoMap");
		_doorPositions = Data.ActiveMapDoors;
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		var dirKey = FuncGeneral.GetPressedKey(@event);

		switch (Data.PlayerMode)
		{
			case Data.Mode.MOVE:
				HandleMoveKeys(dirKey);
				HandleEnterKey(@event);
				HandleOpenKey(@event);
				HandleTalkKey(@event);
				break;

			case Data.Mode.OPEN:
				Open(@event);
				break;

			case Data.Mode.TALK:
				Talk(@event);
				break;
		}

		HandleArrowKeysInSpecialMode(@event);
	}

	private void HandleMoveKeys(string dirKey)
	{
		// if in MOVE and key is ARROW key 
		if (Data.PlayerMode == Data.Mode.MOVE && Data.DirectionInputs.ContainsKey(dirKey))
		{
			if (IsTileNotBlocking(Data.DirectionInputs[dirKey]))
			{
				Move(dirKey);
			}
		}
	}
	private void HandleEnterKey(InputEvent @event)
	{
		if (@event.IsActionPressed("ENTER"))
		{
			_customSignals.EmitSignal(nameof(CustomSignals.ActionStarted), "ENTER");
			Enter();
		}
	}
	private void HandleOpenKey(InputEvent @event)
	{
		if (@event.IsActionPressed("OPEN"))
		{
			_customSignals.EmitSignal(nameof(CustomSignals.ActionStarted), "OPEN");
			Data.PlayerMode = Data.Mode.OPEN;
		}
	}
	private void HandleTalkKey(InputEvent @event)
	{
		if (@event.IsActionPressed("TALK"))
		{
			_customSignals.EmitSignal(nameof(CustomSignals.ActionStarted), "TALK");
			Data.PlayerMode = Data.Mode.TALK;
		}
	}
	private void HandleArrowKeysInSpecialMode(InputEvent @event)
	{
		switch (Data.PlayerMode)
		{
			case Data.Mode.OPEN:
				Open(@event);
				break;
		}
	}


	private void Enter()
	{
		foreach (var location in Data.ActiveMapConnections)
		{
			if (GetCurrentPlayerPos() == location.Value)
			{
				Data.LastPosOnOverworld = location.Value;
				_customSignals.EmitSignal(nameof(CustomSignals.NewMapEntered), location.Key);
			}
		}
	}
	private void Move(string key)
	{
		Position += Data.DirectionInputs[key] * Data.TileSize;
		Data.CurrentPlayerPos = GetCurrentPlayerPos();

		_customSignals.EmitSignal(nameof(CustomSignals.PlayerMoved), Position, _losRayCast);
		_customSignals.EmitSignal(nameof(CustomSignals.ActionStarted), FuncGeneral.MapDirToDirection(key));

		FuncMap.CloseDoorsAfterPlayer(_doorPositions , _geoTileMap);
		CheckIfMapEdgeReached();
	}
	private void Talk(InputEvent @event)
	{
		GD.Print("TALK");
	}
	private void Open(InputEvent @event)
	{
		foreach (var dir in Data.DirectionInputs.Keys)
		{	
			if (@event.IsActionPressed(dir))
			{
				if (CheckForRayCastCollision(dir) == "door1")
				{
					GD.Print("OPEN DOOR!!");
					FuncMap.OpenDoor(GetCurrentPlayerPos(), Data.DirectionInputs[dir], _geoTileMap);
				}
				Data.PlayerMode = Data.Mode.MOVE;
			}
		}
	}


	private string CheckForRayCastCollision(string dir)
	{
		_collisionRayCast.TargetPosition = Data.DirectionInputs[dir] * Data.TileSize;
		_collisionRayCast.ForceRaycastUpdate();
		
		if (_collisionRayCast.IsColliding())
		{
			switch (_collisionRayCast.GetCollider().Get("type").ToString())
			{
				case "NPC":
					return "NPC";
				case "ENEMY":
					return "ENEMY";
				default:
					var collider = _collisionRayCast.GetCollider();
					var contactPoint = _collisionRayCast.GetCollisionPoint() + Data.DirectionInputs[dir] * Data.TileSize;
					Vector2 cellPosition = _geoTileMap.LocalToMap(contactPoint);
					var coords = GetCurrentPlayerPos() + Data.DirectionInputs[dir];
					return FuncMap.GetCustomTileData(_geoTileMap, coords, "type");		
			}
		}
		else
		{
			return "no_collision";
		}
	}
	private bool IsTileNotBlocking(Vector2 dir)
	{
		var tileBlocking = FuncMap.IsTileBlocking(GetCurrentPlayerPos() + dir, _geoTileMap);
		if (tileBlocking != "true") return true;
		else return false;
	}
	private Vector2 GetCurrentPlayerPos()
	{
		return (Vector2)_geoTileMap.LocalToMap(Position);
	}
	private void CheckIfMapEdgeReached()
	{
		Vector2 pos = GetCurrentPlayerPos();

		if (Data.ActiveMapName != Data.OverworldName)
		{
			if (pos.Y < 0 || pos.Y > 50 || pos.X < 0 || pos.X > 50)
			{
				_customSignals.EmitSignal(nameof(CustomSignals.NewMapEntered), Data.OverworldName);
			}
		}
	}
}
