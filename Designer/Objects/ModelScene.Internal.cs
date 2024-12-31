using System.Runtime.InteropServices;
using LuaNET.Lua51;
using Spectre.Console;
using static LuaNET.Lua51.Lua;

namespace WoWFrameTools;

public partial class ModelScene
{
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_Model_SetCameraPosition
    /// Model:SetCameraPosition(positionX, positionY, positionZ)
    /// </summary>
    /// <param name="L"></param>
    /// <returns></returns>
    public static int internal_SetCameraPosition(lua_State L)
    {
        var frame = GetFrame(L, 1) as ModelScene;

        var argc = lua_gettop(L);
        if (argc != 4)
        {
            AnsiConsole.MarkupLine("[red]Error: SetCameraPosition requires 3 arguments.[/]");
            return 0;
        }
        
        var positionX = (float)lua_tonumber(L, 2);
        var positionY = (float)lua_tonumber(L, 3);
        var positionZ = (float)lua_tonumber(L, 4);
        
        frame?.SetCameraPosition(positionX, positionY, positionZ);
        
        return 0;
    }
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_ModelScene_SetCameraOrientationByYawPitchRoll
    /// ModelScene:SetCameraOrientationByYawPitchRoll(yaw, pitch, roll)
    /// </summary>
    /// <param name="L"></param>
    /// <returns></returns>
    public static int internal_SetCameraOrientationByYawPitchRoll(lua_State L)
    {
        var frame = GetFrame(L, 1) as ModelScene;

        var argc = lua_gettop(L);
        if (argc != 4)
        {
            AnsiConsole.MarkupLine("[red]Error: SetCameraOrientationByYawPitchRoll requires 3 arguments.[/]");
            return 0;
        }
        
        var yaw = (float)lua_tonumber(L, 2);
        var pitch = (float)lua_tonumber(L, 3);
        var roll = (float)lua_tonumber(L, 4);
        
        frame?.SetCameraOrientationByYawPitchRoll(yaw, pitch, roll);
        
        return 0;
    }
}