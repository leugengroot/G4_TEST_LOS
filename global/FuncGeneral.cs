using System.Linq;

namespace RpgGame.Global;

using Godot;
using System;
using System.Collections.Generic;

public partial class FuncGeneral : Node
{
    public static string GetPressedKey(InputEvent @event)
    {
        foreach (var dir in Data.DirectionInputs.Keys.Where(dir => @event.IsActionPressed(dir)))
        {
            return dir;
        }

        return @event.ToString();
    }

    public static string MapDirToDirection(string dir)
    {
        return dir switch
        {
            "UP_ARROW" => "NORTH",
            "DOWN_ARROW" => "SOUTH",
            "RIGHT_ARROW" => "EAST",
            "LEFT_ARROW" => "WEST",
            _ => throw new ArgumentException("Unsupported direction: " + dir)
        };
    }
}