using System;
using System.Runtime.InteropServices;
using LuaNET.Lua51;
using WoWFrameTools.API;
using static LuaNET.Lua51.Lua;

namespace WoWFrameTools.Widgets
{
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/UIOBJECT_FrameScriptObject
    /// </summary>
    public class FrameScriptObject
    {
        private readonly string? _name;
        private readonly string _objectType;
        private bool _isForbidden;
        
        public IntPtr UserdataPtr { get; set; }
        public int LuaRegistryRef { get; set; }
        public GCHandle Handle { get; set; }
        
        public FrameScriptObject(string objectType, string? name)
        {
            _name = name;
            _objectType = objectType;
        }
        
        /// <summary>
        /// https://warcraft.wiki.gg/wiki/API_FrameScriptObject_GetName
        /// </summary>
        /// <returns></returns>
        public string GetName() => _name;
        private int internal_GetName(lua_State L)
        {
            var obj = GetThis(L, 1);
            var name = obj?.GetName();
            lua_pushstring(L, name);
            return 1;
        }
        
        /// <summary>
        /// https://warcraft.wiki.gg/wiki/API_FrameScriptObject_GetObjectType
        /// </summary>
        /// <returns></returns>
        public string GetObjectType() => _objectType;
        private int internal_GetObjectType(lua_State L)
        {
            var obj = GetThis(L, 1);
            lua_pushstring(L, obj?.GetObjectType() ?? "");
            return 1;
        }
        
        /// <summary>
        /// https://warcraft.wiki.gg/wiki/API_FrameScriptObject_IsForbidden
        /// </summary>
        /// <returns></returns>
        public bool IsForbidden() => _isForbidden;
        private int internal_IsForbidden(lua_State L)
        {
            var obj = GetThis(L, 1);
            lua_pushboolean(L, (obj != null && obj.IsForbidden()) ? 1 : 0);
            return 1;
        }
        
        /// <summary>
        /// https://warcraft.wiki.gg/wiki/API_FrameScriptObject_IsObjectType
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public bool IsObjectType(string objectType) => _objectType == objectType;
        private int internal_IsObjectType(lua_State L)
        {
            var obj = GetThis(L, 1);
            string str = lua_tostring(L, 2) ?? "";
            bool result = (obj != null && obj.IsObjectType(str));
            lua_pushboolean(L, result ? 1 : 0);
            return 1;
        }

        /// <summary>
        /// https://warcraft.wiki.gg/wiki/API_FrameScriptObject_SetForbidden
        /// </summary>
        /// <param name="forbidden"></param>
        public void SetForbidden(bool forbidden)
        {
            _isForbidden = forbidden;
        }
        private int internal_SetForbidden(lua_State L)
        {
            var obj = GetThis(L, 1);
            if (obj != null)
            {
                bool isF = (lua_toboolean(L, 2) != 0);
                obj.SetForbidden(isF);
            }
            return 0;
        }

        // ----------- Virtual Registration ---------------

        /// <summary>
        /// The name of the metatable for this class.
        /// Derived classes can override if they want "FrameMetaTable", etc.
        /// </summary>
        public virtual string GetMetatableName() => "FrameScriptObjectMetaTable";

        /// <summary>
        /// Virtual method to register this class's Lua metatable.
        /// Derived classes can override or extend it.
        /// </summary>
        public virtual void RegisterMetaTable(lua_State L)
        {
            // 1) Create a new metatable with the name from GetMetatableName()
            var metaName = GetMetatableName();
            luaL_newmetatable(L, metaName);

            // 2) __index = metatable
            lua_pushvalue(L, -1);
            lua_setfield(L, -2, "__index");

            // 3) Register instance methods
            LuaHelpers.RegisterMethod(L, "GetName", internal_GetName);
            LuaHelpers.RegisterMethod(L, "GetObjectType", internal_GetObjectType);
            LuaHelpers.RegisterMethod(L, "IsForbidden", internal_IsForbidden);
            LuaHelpers.RegisterMethod(L, "IsObjectType", internal_IsObjectType);
            LuaHelpers.RegisterMethod(L, "SetForbidden", internal_SetForbidden);

            // 4) If you want a GC metamethod
            lua_pushcfunction(L, (state) => internal_ObjectGC(state));
            lua_setfield(L, -2, "__gc");

            // 5) pop
            lua_pop(L, 1);
        }
        private int internal_ObjectGC(lua_State L)
        {
            // standard GC approach
            IntPtr userdataPtr = (IntPtr)lua_touserdata(L, 1);
            if (userdataPtr != IntPtr.Zero)
            {
                IntPtr handlePtr = Marshal.ReadIntPtr(userdataPtr);
                if (handlePtr != IntPtr.Zero)
                {
                    GCHandle handle = GCHandle.FromIntPtr(handlePtr);
                    if (handle.IsAllocated)
                        handle.Free();
                }
            }
            return 0;
        }

        /// <summary>
        /// Helper to retrieve 'this' object from the Lua stack at 'index'
        /// using the metatable approach. 
        /// </summary>
        public FrameScriptObject? GetThis(lua_State L, int index)
        {
            /*
            var metaName = GetMetatableName();
            luaL_getmetatable(L, metaName);
            lua_getmetatable(L, index);
            
            if (lua_isuserdata(L, index) != 0)
            {
                IntPtr userdataPtr = (IntPtr)lua_touserdata(L, index);
                if (UIObjects._frameRegistry.TryGetValue(userdataPtr, out var frame))
                {
                    return frame;
                }
            }
            else if (lua_istable(L, index) != 0)
            {
                // Assume the table has a '__frame' field containing the userdata
                lua_pushstring(L, "__frame");      // Push key '__frame'
                lua_gettable(L, index);             // Get table['__frame']
                if (lua_islightuserdata(L, -1) != 0)
                {
                    IntPtr userdataPtr = (IntPtr)lua_touserdata(L, -1);
                    lua_pop(L, 1);                   // Remove '__frame' value from stack
                    if (UIObjects._frameRegistry.TryGetValue(userdataPtr, out var frame))
                    {
                        return frame;
                    }
                }
                else
                {
                    lua_pop(L, 1);                   // Remove '__frame' value from stack
                }
            }

            return null;
            */
            
            // 1) Check the correct metatable
            var metaName = GetMetatableName();
            luaL_getmetatable(L, metaName);
            lua_getmetatable(L, index);
            bool same = (lua_rawequal(L, -1, -2) != 0);
            lua_pop(L, 2);

            if (!same)
                return null;

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

            return handle.Target as FrameScriptObject;
            
        }
    }
}