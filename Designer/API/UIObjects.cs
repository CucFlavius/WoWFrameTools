using System.Runtime.InteropServices;
using LuaNET.Lua51;
using WoWFrameTools.Widgets;
using static LuaNET.Lua51.Lua;

namespace WoWFrameTools.API;

public static class UIObjects
{
    public static Frame? UIParent;
    public static Minimap? Minimap;
    public static Dictionary<string?, Frame> _nameToFrameRegistry = new();
    public static readonly Dictionary<IntPtr, Frame> _frameRegistry = new();
    public static readonly Dictionary<IntPtr, Texture> _textureRegistry = new();
    public static readonly Dictionary<IntPtr, FontString> _fontStringRegistry = new();
    public static readonly Dictionary<IntPtr, ModelSceneActor> _actorRegistry = new();
    public static readonly Dictionary<IntPtr, Line> _lineRegistry = new();
    public static readonly Dictionary<string, HashSet<Frame>> _eventToFrames = new();

    public static void CreateGlobalFrame(lua_State L, Frame? frame)
    {
        // 5. Allocate a GCHandle to prevent garbage collection
        GCHandle handle = GCHandle.Alloc(frame);
        IntPtr handlePtr = GCHandle.ToIntPtr(handle);

        // 6. Create userdata with the size of IntPtr
        IntPtr frameUserdataPtr = (IntPtr)lua_newuserdata(L, (UIntPtr)IntPtr.Size);

        // Write the handlePtr into the userdata memory
        Marshal.WriteIntPtr(frameUserdataPtr, handlePtr);

        // 7. Set the metatable for the userdata
        luaL_getmetatable(L, Internal.Frame.GetMetatableName()); // Ensure FrameMetaTable is set up
        lua_setmetatable(L, -2);

        // 8. Add the Frame to the registry for later retrieval
        _frameRegistry[frameUserdataPtr] = frame;
        _nameToFrameRegistry[frame.GetName()] = frame;

        // Assign the userdataPtr and LuaRegistryRef to the Frame instance
        frame.UserdataPtr = frameUserdataPtr;

        // Create a reference to the userdata in the Lua registry
        lua_pushvalue(L, -1); // Push the userdata
        int refIndex = luaL_ref(L, LUA_REGISTRYINDEX);
        frame.LuaRegistryRef = refIndex;

        // 9. **Create a Lua table and embed the userdata**
        // Create a new Lua table
        lua_newtable(L); // Push a new table onto the stack

        // Set the Frame userdata in the table with a hidden key
        lua_pushstring(L, "__frame"); // Key
        lua_pushlightuserdata(L, (UIntPtr)frameUserdataPtr); // Value (light userdata)
        lua_settable(L, -3); // table["__frame"] = userdata

        // Set the metatable for the table to handle method calls and property accesses
        luaL_getmetatable(L, Internal.Frame.GetMetatableName()); // Push the FrameMetaTable
        lua_setmetatable(L, -2); // setmetatable(table, "FrameMetaTable")

        // 10. Log the creation
        Log.CreateFrame(frame);
        
        // set global eg. "Minimap"
        lua_rawgeti(L, LUA_REGISTRYINDEX, refIndex);
        lua_setglobal(L, frame.GetName());
    }

    public static int CreateFrame(lua_State L)
    {
        try
        {
            var argc = lua_gettop(L);
            
            // 1. Retrieve arguments from Lua
            string frameType = lua_tostring(L, 1) ?? "Frame";
            string? name = "";
            
            if (argc >= 2)
            {
                name = lua_tostring(L, 2);
            }
            
            // 2. Handle the parent frame (optional, default to UIParent)
            var parentFrame = argc >= 3 ? LuaHelpers.GetFrame(L, 3) : UIParent;

            // 3. Optional template and ID
            string? template = null;
            if (argc >= 4)
            {
                template = lua_tostring(L, 4) ?? "";
            }

            var id = 0;
            if (argc >= 5)
            {
                id = LuaHelpers.IsNumber(L, 5) ? (int)lua_tonumber(L, 5) : 0;
            }

            // 4. Create a new Frame instance
            var frame = frameType.ToLower() switch
            {
                "frame" => new Frame(frameType, name, parentFrame, template, id),
                "button" => new Button(name, parentFrame, template, id),
                "editbox" => new EditBox(name, parentFrame, template, id),
                "gametooltip" => new GameTooltip(name, parentFrame, template, id),
                "modelscene" => new ModelScene(name, parentFrame, template, id),
                _ => throw new NotImplementedException($"Unsupported frame type: {frameType}")
            };

            var metaTableName = frameType.ToLower() switch
            {
                "frame" => Internal.Frame.GetMetatableName(),
                "button" => Internal.Button.GetMetatableName(),
                "editbox" => Internal.EditBox.GetMetatableName(),
                "gametooltip" => Internal.GameTooltip.GetMetatableName(),
                "modelscene" => Internal.ModelScene.GetMetatableName(),
                _ => throw new NotImplementedException($"Unsupported frame type: {frameType}")
            };

            // 5. Allocate a GCHandle to prevent garbage collection
            var handle = GCHandle.Alloc(frame);
            var handlePtr = GCHandle.ToIntPtr(handle);

            // 6. Create userdata with the size of IntPtr
            var userdataPtr = (IntPtr)lua_newuserdata(L, (UIntPtr)IntPtr.Size);

            // Write the handlePtr into the userdata memory
            Marshal.WriteIntPtr(userdataPtr, handlePtr);

            // 7. Set the metatable for the userdata
            luaL_getmetatable(L, metaTableName); // Ensure FrameMetaTable is set up
            lua_setmetatable(L, -2);

            // 8. Add the Frame to the registry for later retrieval
            _frameRegistry[userdataPtr] = frame;
            if (name != null)
                _nameToFrameRegistry[name] = frame;

            // Assign the userdataPtr and LuaRegistryRef to the Frame instance
            frame.UserdataPtr = userdataPtr;

            // Create a reference to the userdata in the Lua registry
            lua_pushvalue(L, -1); // Push the userdata
            int refIndex = luaL_ref(L, LUA_REGISTRYINDEX);
            frame.LuaRegistryRef = refIndex;

            // 9. **Create a Lua table and embed the userdata**
            // Create a new Lua table
            lua_newtable(L); // Push a new table onto the stack

            // Set the Frame userdata in the table with a hidden key
            lua_pushstring(L, "__frame"); // Key
            lua_pushlightuserdata(L, (UIntPtr)userdataPtr); // Value (light userdata)
            lua_settable(L, -3); // table["__frame"] = userdata

            // Set the metatable for the table to handle method calls and property accesses
            luaL_getmetatable(L, metaTableName); // Push the FrameMetaTable
            lua_setmetatable(L, -2); // setmetatable(table, "FrameMetaTable")

            // 10. Log the creation
            Log.CreateFrame(frame);

            return 1; // Return the table
        }
        catch (Exception ex)
        {
            // Log the exception
            Log.Exception(ex);

            // Push error message to Lua
            Log.ErrorL(L, $"Error in CreateFrame: {ex.Message}");
            return 0; // Unreachable
        }
    }
}