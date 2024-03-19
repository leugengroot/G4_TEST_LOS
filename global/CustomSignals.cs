namespace RpgGame.Global;

using Godot;

public partial class CustomSignals : Node
{
	[Signal]
	public delegate void PlayerMovedEventHandler(Vector2 position, RayCast2D rayCastLos);
	[Signal]
	public delegate void NewMapEnteredEventHandler(string newMapName);
	[Signal]
	public delegate void ActionStartedEventHandler(Data.Mode playerMode);
	[Signal]
	public delegate void DialogStartedEventHandler(Data.Mode playerMode);
}