namespace RpgGame.Global;

using Godot;

public partial class Scenes : Node
{
	public static void AddNode(string scenePath, Node anchor)
{
    // Load the scene
    var sceneToLoad = ResourceLoader.Load<PackedScene>(scenePath);

    // Create an instance of the scene
    var instance = sceneToLoad.Instantiate();

    // Add the instance as a child of the anchor node
    anchor.AddChild(instance);
}
}
