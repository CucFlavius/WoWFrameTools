using System.Runtime.InteropServices;
using LuaNET.Lua51;
using static LuaNET.Lua51.Lua;

namespace WoWFrameTools.Internal;

public static class ScriptObject
{
    // Static dictionaries to hold delegates and ensure they are not garbage collected
    private static readonly Dictionary<string, Widgets.ScriptHandler> StaticScriptHandlers = new();
    private static readonly object LockObj = new();
    
    public static int internal_ClearParentKey(lua_State L)
    {
        var obj = GetThis(L, 1);
        obj?.ClearParentKey();
        return 0;
    }

    public static int internal_GetDebugName(lua_State L)
    {
        var obj = GetThis(L, 1);
        bool preferParentKey = false;
        if (lua_gettop(L) >= 2)
        {
            preferParentKey = (lua_toboolean(L, 2) != 0);
        }

        lua_pushstring(L, obj?.GetDebugName(preferParentKey) ?? "");
        return 1;
    }

    public static int internal_GetParent(lua_State L)
    {
        var obj = GetThis(L, 1);
        var parent = obj?.GetParent();
        if (parent == null)
        {
            lua_pushnil(L);
            return 1;
        }

        LuaHelpers.PushFrameScriptObject(L, parent);
        return 1;
    }

    public static int internal_GetParentKey(lua_State L)
    {
        var obj = GetThis(L, 1);
        string? parentKey = obj?.GetParentKey();
        lua_pushstring(L, parentKey ?? "");
        return 1;
    }

    public static int internal_SetParentKey(lua_State L)
    {
        var obj = GetThis(L, 1);
        if (obj != null)
        {
            var parentKey = lua_tostring(L, 2) ?? "";
            obj.SetParentKey(parentKey);
        }

        return 0;
    }

    public static int internal_GetScript(lua_State L)
    {
        var obj = GetThis(L, 1);
        string scriptName = lua_tostring(L, 2) ?? "";
        // optional bindingType = (some int)...

        var script = obj?.GetScript(scriptName);
        if (script == null)
        {
            lua_pushnil(L);
            return 1;
        }

        // If we store a Lua reference, push the function from the registry
        // If we store a C# delegate, we might wrap it in a cfunction or skip for advanced usage
        // For demonstration, just push nil again
        lua_pushnil(L);
        return 1;
    }

    public static int internal_HasScript(lua_State L)
    {
        var obj = GetThis(L, 1);
        string scriptName = lua_tostring(L, 2) ?? "";
        bool hasIt = obj?.HasScript(scriptName) ?? false;
        lua_pushboolean(L, hasIt ? 1 : 0);
        return 1;
    }

    public static int internal_HookScript(lua_State L)
    {
        try
        {
            // Ensure there are exactly 3 arguments: frame, scriptTypeName, handler
            var argc = lua_gettop(L);
            if (argc != 3)
            {
                Log.ErrorL(L, "HookScript requires exactly 3 arguments: frame, scriptTypeName, handler.");
                return 0;
            }

            // Validate that the second argument is a string
            if (!LuaHelpers.IsString(L, 2))
            {
                Log.ErrorL(L, "HookScript: 'scriptTypeName' must be a string.");
                return 0;
            }

            var scriptTypeName = lua_tostring(L, 2);

            // Validate that the third argument is a function
            if (lua_isfunction(L, 3) == 0)
            {
                Log.ErrorL(L, "HookScript: 'handler' must be a function.");
                return 0;
            }

            // Retrieve the Frame object
            var frame = GetThis(L, 1);
            if (frame == null)
            {
                Log.ErrorL(L, "HookScript: Invalid Frame object.");
                return 0;
            }

            // Create a reference to the Lua function
            lua_pushvalue(L, 3); // Push the function to the top of the stack
            var refIndex = luaL_ref(L, LUA_REGISTRYINDEX);

            // Assign the script handler using a static method
            ScriptObject.StaticScriptHandlers[scriptTypeName] = (f, parameters) =>
            {
                HandleScriptCallback(L, refIndex, f, parameters);
            };

            frame.HookScript(scriptTypeName, StaticScriptHandlers[scriptTypeName], refIndex);

            // Push 'true' to indicate success
            lua_pushboolean(L, 1);
            return 1;
        }
        catch (Exception ex)
        {
            Log.Exception(L, ex, "HookScript");
            Log.ErrorL(L, "HookScript encountered an error.");
            return 0;
        }
    }

    public static int internal_SetScript(lua_State L)
    {
        try
        {
            // Ensure there are at least 3 arguments: frame, scriptTypeName, handler
            var argc = lua_gettop(L);
            if (argc < 3)
            {
                Log.ErrorL(L, "SetScript requires at least 3 arguments: frame, scriptTypeName, handler.");
                return 0;
            }

            // Validate that the second argument is a string
            if (!LuaHelpers.IsString(L, 2))
            {
                Log.ErrorL(L, "SetScript: 'scriptTypeName' must be a string.");
                return 0;
            }

            var scriptTypeName = lua_tostring(L, 2);

            // Check if the third argument is a function or nil
            var isFunction = lua_isfunction(L, 3) != 0;
            var isNil = lua_isnil(L, 3) != 0;

            if (isFunction || isNil)
            {
                if (isFunction)
                {
                    // Create a reference to the Lua function
                    lua_pushvalue(L, 3); // Push the function to the top of the stack
                    var refIndex = luaL_ref(L, LUA_REGISTRYINDEX);

                    // Retrieve the Frame object
                    var frame = GetThis(L, 1);

                    if (scriptTypeName != null)
                    {
                        // Assign the script handler using a static method
                        StaticScriptHandlers[scriptTypeName] = (f, parameters) =>
                        {
                            HandleScriptCallback(L, refIndex, f, parameters);
                        };

                        if (frame == null)
                        {
                            Log.ErrorL(L, "SetScript: Invalid Frame object.");
                            return 0;
                        }

                        frame.SetScript(scriptTypeName, StaticScriptHandlers[scriptTypeName], refIndex);

                        // Optionally, log the successful script assignment
                        Log.ScriptSet(scriptTypeName, frame);
                    }
                }
                else if (isNil)
                {
                    // Retrieve the Frame object
                    var frame = GetThis(L, 1);

                    // Remove the script handler
                    frame?.SetScript(scriptTypeName, null);
                    Log.RemoveScript(scriptTypeName, frame);
                }

                // Push 'true' to indicate success
                lua_pushboolean(L, 1);
                return 1;
            }

            Log.ErrorL(L, "SetScript: Invalid arguments. Expected a function or nil as the third argument.");
            return 0;
        }
        catch (Exception ex)
        {
            Log.Exception(L, ex, "SetScript");
            Log.ErrorL(L, "SetScript encountered an error.");
            return 0;
        }
    }

    public static string GetMetatableName() => "ScriptObjectMetaTable";

    private static Widgets.ScriptObject? GetThis(lua_State L, int index)
    {
        // 1) Check the correct metatable
        // var metaName = GetMetatableName();
        // luaL_getmetatable(L, metaName);
        // lua_getmetatable(L, index);
        // bool same = (lua_rawequal(L, -1, -2) != 0);
        // lua_pop(L, 2);
        //
        // if (!same)
        //     return null;

        // If it's a table, retrieve the __frame key
        if (lua_istable(L, index) != 0)
        {
            lua_pushstring(L, "__frame");
            lua_gettable(L, index); // Pushes table["__frame"]
            index = -1; // Update index to point to __frame value
        }

        IntPtr userdataPtr = (IntPtr)lua_touserdata(L, index);
        if (userdataPtr == IntPtr.Zero)
            return null;

        IntPtr handlePtr = Marshal.ReadIntPtr(userdataPtr);
        if (handlePtr == IntPtr.Zero)
            return null;

        var handle = GCHandle.FromIntPtr(handlePtr);
        if (!handle.IsAllocated)
            return null;

        return handle.Target as Widgets.ScriptObject;
    }
    
    public static void RegisterMetaTable(lua_State L)
    {
        // 1) Register the base class's metatable first
        FrameScriptObject.RegisterMetaTable(L);

        // 2) Define "UIObjectMetaTable"
        string metaName = GetMetatableName();
        luaL_newmetatable(L, metaName);

        // 3) __index = UIObjectMetaTable
        lua_pushvalue(L, -1);
        lua_setfield(L, -2, "__index");

        // 4) Link to the base class's metatable ("FrameScriptObjectMetaTable")
        var baseMetaName = FrameScriptObject.GetMetatableName();
        luaL_getmetatable(L, baseMetaName);
        lua_setmetatable(L, -2); // Sets UIObjectMetaTable's metatable to FrameScriptObjectMetaTable

        // 5) Bind ScriptObject-specific methods
        LuaHelpers.RegisterMethod(L, "ClearParentKey", Internal.ScriptObject.internal_ClearParentKey);
        LuaHelpers.RegisterMethod(L, "GetDebugName", Internal.ScriptObject.internal_GetDebugName);
        LuaHelpers.RegisterMethod(L, "GetParent", Internal.ScriptObject.internal_GetParent);
        LuaHelpers.RegisterMethod(L, "GetParentKey", Internal.ScriptObject.internal_GetParentKey);
        LuaHelpers.RegisterMethod(L, "SetParentKey", Internal.ScriptObject.internal_SetParentKey);
        LuaHelpers.RegisterMethod(L, "GetScript", Internal.ScriptObject.internal_GetScript);
        LuaHelpers.RegisterMethod(L, "HasScript", Internal.ScriptObject.internal_HasScript);
        LuaHelpers.RegisterMethod(L, "HookScript", Internal.ScriptObject.internal_HookScript);
        LuaHelpers.RegisterMethod(L, "SetScript", Internal.ScriptObject.internal_SetScript);

        // 6) pop
        lua_pop(L, 1);
    }
    
    private static void HandleScriptCallback(lua_State L, int refIndex, Widgets.ScriptObject frame, Widgets.Parameters? parameters)
    {
        try
        {
            // Retrieve the Lua function from the registry
            lua_rawgeti(L, LUA_REGISTRYINDEX, refIndex);

            // Retrieve the userdata from the registry using LuaRegistryRef
            lua_rawgeti(L, LUA_REGISTRYINDEX, frame.LuaRegistryRef); // Push 'self' userdata
            
            switch (parameters?.type)
            {
                case "OnEvent":
                {
                    var eventParameters = (Widgets.EventParameters)parameters;
                    var eventName = eventParameters.eventName;
                    var extraParam = eventParameters.extraParam;
                
                    // Push 'eventName'
                    lua_pushstring(L, eventName); // Push 'event'

                    // Push 'extraParam' if present
                    if (!string.IsNullOrEmpty(extraParam))
                        lua_pushstring(L, extraParam);

                    // Determine the number of arguments
                    var args = string.IsNullOrEmpty(extraParam) ? 2 : 3;

                    // Call the Lua function with the appropriate number of arguments, 0 results
                    if (lua_pcall(L, args, 0, 0) != 0)
                    {
                        // Retrieve and log the error message
                        var error = lua_tostring(L, -1);
                        Log.Error($"HandleScriptCallback: Event: {error}");
                        lua_pop(L, 1); // Remove error message
                    }

                    break;
                }
                case "OnUpdate":
                {
                    var updateParameters = (Widgets.UpdateParameters)parameters;
                    var elapsed = updateParameters.elapsed;

                    // Push 'elapsed'
                    lua_pushnumber(L, elapsed);

                    // Call the Lua function with 1 argument, 0 results
                    if (lua_pcall(L, 2, 0, 0) != 0)
                    {
                        // Retrieve and log the error message
                        var error = lua_tostring(L, -1);
                        Log.Error($"HandleScriptCallback: Update: {error}");
                        lua_pop(L, 1); // Remove error message
                    }

                    break;
                }
                default:
                    // Push 'nil' for unsupported parameters
                    Log.Error($"HandleScriptCallback: Unsupported parameters of type {parameters.type}");
                    lua_pushnil(L);
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception while executing HookScript: {ex.Message}");
        }
    }

}