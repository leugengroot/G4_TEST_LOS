using System.Collections.Generic;
using System.Linq;

namespace RpgGame.Global;

using Godot;
using System;

public partial class FuncAstar : Node
{
	public static Vector2[] CreateAStarPath(AStarGrid2D astarGrid, Vector2 from, Vector2 to)
	{
		var path = astarGrid.GetPointPath((Vector2I)from, (Vector2I)to); // Get the path from AStarGrid2D

		switch (path.Length)
		{
			case > 0:
				return path; // Return if path is found
			default:
				Array.Clear(path, 0, path.Length); // Clear if no path found
				return path; // Return empty list
		}
	}
	
	
	
	public static AStarGrid2D CreateAStarGrid(TileMap tileMap, bool allowDiagonals)
	{
		// Assuming AStarGrid2D is a class implementing A* grid functionality
		AStarGrid2D astarGrid = new AStarGrid2D();

		// Get used cells from the tilemap
		List<Vector2I> usedCells = tileMap.GetUsedCells(0).ToList(); // Convert to list for loop iteration

		// Set default heuristic and diagonal mode based on arguments
		astarGrid.DefaultComputeHeuristic = AStarGrid2D.Heuristic.Manhattan;
		astarGrid.DefaultEstimateHeuristic = AStarGrid2D.Heuristic.Manhattan;
		astarGrid.DiagonalMode = allowDiagonals ? AStarGrid2D.DiagonalModeEnum.Always : AStarGrid2D.DiagonalModeEnum.Never;

		// Set grid size and cell size based on tilemap
		astarGrid.Size = tileMap.GetUsedRect().Size;
		astarGrid.CellSize = new Vector2(Data.TileSize, Data.TileSize);

		// Update the grid
		astarGrid.Update();

		// Mark used cells as solid
		foreach (Vector2 cell in usedCells)
		{
			astarGrid.SetPointSolid((Vector2I)cell, false);
		}

		return astarGrid;
	}
}
