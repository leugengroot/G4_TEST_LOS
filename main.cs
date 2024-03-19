using Godot;

public partial class main : Node2D
{
	
	public override void _Ready()
	{		
		GetTree().ChangeSceneToFile("res://UI/UI.tscn");
		//test
	}

	
	public override void _Process(double delta)
	{
	}
}
