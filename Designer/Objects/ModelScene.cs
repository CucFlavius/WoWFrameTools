﻿using LuaNET.Lua51;

namespace WoWFrameTools;

public class ModelScene : Frame
{
    public ModelScene(lua_State luaState) : base(luaState)
    {
    }

    public ModelScene(lua_State luaState, string? frameType, string? name, Frame? parent, string? template, int id) : base(luaState, frameType, name, parent, template, id)
    {
    }
}