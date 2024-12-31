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

    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_ModelScene_SetLightAmbientColor
    /// ModelScene:SetLightAmbientColor(colorR, colorG, colorB)
    /// </summary>
    /// <param name="L"></param>
    /// <returns></returns>
    public static int internal_SetLightAmbientColor(lua_State L)
    {
        var frame = GetFrame(L, 1) as ModelScene;

        var argc = lua_gettop(L);
        if (argc != 4)
        {
            AnsiConsole.MarkupLine("[red]Error: SetLightAmbientColor requires 3 arguments.[/]");
            return 0;
        }
        
        var colorR = (float)lua_tonumber(L, 2);
        var colorG = (float)lua_tonumber(L, 3);
        var colorB = (float)lua_tonumber(L, 4);
        
        frame?.SetLightAmbientColor(colorR, colorG, colorB);
        
        return 0;
    }
    
    public static int internal_SetLightDiffuseColor(lua_State L)
    {
        var frame = GetFrame(L, 1) as ModelScene;

        var argc = lua_gettop(L);
        if (argc != 4)
        {
            AnsiConsole.MarkupLine("[red]Error: SetLightDiffuseColor requires 3 arguments.[/]");
            return 0;
        }
        
        var colorR = (float)lua_tonumber(L, 2);
        var colorG = (float)lua_tonumber(L, 3);
        var colorB = (float)lua_tonumber(L, 4);
        
        frame?.SetLightDiffuseColor(colorR, colorG, colorB);
        
        return 0;
    }
    
    public static int internal_SetLightVisible(lua_State L)
    {
        var frame = GetFrame(L, 1) as ModelScene;

        var argc = lua_gettop(L);
        if (argc != 2)
        {
            AnsiConsole.MarkupLine("[red]Error: SetLightVisible requires 1 argument.[/]");
            return 0;
        }
        
        var visible = lua_toboolean(L, 2);
        
        frame?.SetLightVisible(visible != 0);
        
        return 0;
    }

    public static int internal_SetFogColor(lua_State L)
    {
        var frame = GetFrame(L, 1) as ModelScene;

        var argc = lua_gettop(L);
        if (argc < 4)
        {
            AnsiConsole.MarkupLine("[red]Error: SetFogColor requires 3 arguments.[/]");
            return 0;
        }
        
        var colorR = (float)lua_tonumber(L, 2);
        var colorG = (float)lua_tonumber(L, 3);
        var colorB = (float)lua_tonumber(L, 4);
        
        frame?.SetFogColor(colorR, colorG, colorB);
        
        return 0;
    }
    
    public static int internal_SetFogFar(lua_State L)
    {
        var frame = GetFrame(L, 1) as ModelScene;

        var argc = lua_gettop(L);
        if (argc != 2)
        {
            AnsiConsole.MarkupLine("[red]Error: SetFogFar requires 1 argument.[/]");
            return 0;
        }
        
        var far = (float)lua_tonumber(L, 2);
        
        frame?.SetFogFar(far);
        
        return 0;
    }
    
    public static int internal_SetFogNear(lua_State L)
    {
        var frame = GetFrame(L, 1) as ModelScene;

        var argc = lua_gettop(L);
        if (argc != 2)
        {
            AnsiConsole.MarkupLine("[red]Error: SetFogNear requires 1 argument.[/]");
            return 0;
        }
        
        var near = (float)lua_tonumber(L, 2);
        
        frame?.SetFogNear(near);
        
        return 0;
    }
    
    public static int internal_ClearFog(lua_State L)
    {
        var frame = GetFrame(L, 1) as ModelScene;

        frame?.ClearFog();
        
        return 0;
    }
}