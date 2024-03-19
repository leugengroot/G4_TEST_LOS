using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;
using RpgGame.Map;

namespace RpgGame.Global.FuncLos;

public partial class FuncLos : Node
{
    private static System.Collections.Generic.Dictionary<string, Vector2> _npcs = new();

    static List<Line2D> lineList = new List<Line2D>();

    public static void CoverVisibilityTilemap(TileMap map, Vector2 mapSize)
    {
        for (var x = 0; x < mapSize.X; x++)
        {
            for (var y = 0; y < mapSize.Y; y++)
            {
                FuncMap.HideTile(new Vector2(x, y), map);
            }
        }
    }

    
    
    public static List<Vector2> UpdateLos(
        Vector2 mapPlayerPosition,
        RayCast2D losRayCast,
        TileMap visTileMap
    )
    {
        _npcs = Data.ActiveMapNpcs;
        List<Vector2> markedTiles = new List<Vector2>();

        var targetPointList = CreateTargetPoints(Data.Visibility);
         
        foreach (var mapTargetPoint in targetPointList)
        {
            var los = AddLine(visTileMap, false);
            var pointOrigin = visTileMap.MapToLocal((Vector2I)Data.CurrentPlayerPos);
            var pointTarget = visTileMap.MapToLocal((Vector2I)Data.CurrentPlayerPos + (Vector2I)mapTargetPoint);
            losRayCast.TargetPosition = pointTarget;
            losRayCast.ForceRaycastUpdate();
            los.Points = new[] { pointOrigin, pointTarget};
            GD.Print("mapTargetPoint: " + mapTargetPoint);
            
            if (losRayCast.IsColliding())
            {
                los.DefaultColor = new Color(1, 0, 0);
                
                //
                losRayCast.ForceRaycastUpdate();
                var localCollisionPoint = losRayCast.GetCollisionPoint();
                var mapCollisionPoint = visTileMap.LocalToMap(localCollisionPoint);
                GD.Print("collision detect at: local: " + localCollisionPoint + " / map: " + mapCollisionPoint);
                //
                
                var newPath = CreateAStarPath(mapPlayerPosition, mapCollisionPoint, visTileMap);
                ClearPath(newPath.ToList(), visTileMap, markedTiles); ;
            }
            else
            {
                var mapRayCastEndpoint = mapPlayerPosition + mapTargetPoint;
                //var actualMapSize = Data.ActiveMapSize;
                //mapCollisionPoint.X = Mathf.Clamp(mapTargetPoint.X, 0, actualMapSize.X - 1);
                //mapCollisionPoint.Y = Mathf.Clamp(mapTargetPoint.Y, 0, actualMapSize.Y - 1);

                var newPath = CreateAStarPath(mapPlayerPosition, mapRayCastEndpoint, visTileMap);
                ClearPath(newPath.ToList(), visTileMap, markedTiles);
            }
        }
        
        //KillLines(lineList);

        void KillLines(List<Line2D> line2Ds)
        {
            foreach (var line in line2Ds)
            {
                RemoveLine(line);
            }
        }

        return markedTiles;
    }

    private static Line2D  AddLine(TileMap tileMap, bool paint)
    {
        Line2D los = new Line2D();
        if(paint)tileMap.AddChild(los);
        los.Width = 1;
        lineList.Add(los);
        return los;
    }
    
    


    public static void DisableAllSprites(TileMap map)
    {
        Array<Node> nodes = map.GetParent().GetChildren();
        foreach (Node node in nodes)
        {
            if (node is CharacterBody2D)
            {
                Sprite2D sprite = node.GetNode<Sprite2D>("Sprite2D");
                if (sprite != null)
                {
                    sprite.Visible = false;
                }
            }
        }
    }

    private static Vector2 MapToTilemapCoordinates(Vector2 position, TileMap map)
    {
        return map.LocalToMap((position));
    }

    private static Vector2[] CreateAStarPath(Vector2 from, Vector2 to, TileMap tileMap)
    {
        var astarGrid = FuncAstar.CreateAStarGrid(tileMap, true);
        var path = FuncAstar.CreateAStarPath(astarGrid, from, to);

        for (int i = 0; i < path.Length; i++)
        {
            path[i] = tileMap.LocalToMap(path[i]);
        }

        return path;
    }

    private static void ClearPath(List<Vector2> path, TileMap map, List<Vector2> markedTiles)
    {
        foreach (var tile in path)
        {
            FuncMap.ClearTile(tile, map);
            markedTiles.Add(tile);

            foreach (var npc in _npcs)
            {
                if (npc.Key == "") continue;
                var npcPos = npc.Value;
                if (npcPos == tile)
                {
                    map.GetParent().GetNode((string)npc.Key).GetNode<Sprite2D>("Sprite2D").Visible = true;
                }
            }
        }
    }
    
    public static List<Vector2> CreateTargetPoints(float radius)
    {
        var points = new List<Vector2>();

        // Top row
        for (var a = -radius; a <= radius; a++)
        {
            points.Add(new Vector2(a, -radius));
        }

        // Bottom row
        for (var b = -radius; b <= radius; b++)
        {
            points.Add(new Vector2(b, radius));
        }

        // Left side

        for (var c = -radius; c < radius; c++)
        {
            points.Add(new Vector2(-radius, c + 1));
        }

        // Right side
        for (var d = -radius; d < radius; d++)
        {
            points.Add(new Vector2(radius, d + 1));
        }

        return points;
    }
    
    
    public static void RemoveLine(Line2D line)
    {
        if (line != null && line.IsInsideTree())
        {
            line.QueueFree();
            line = null;
        }
    }
}