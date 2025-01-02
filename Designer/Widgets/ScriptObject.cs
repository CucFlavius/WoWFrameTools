using System;
using System.Runtime.InteropServices;
using LuaNET.Lua51;
using Spectre.Console;
using static LuaNET.Lua51.Lua;

namespace WoWFrameTools.Widgets
{
    public delegate void ScriptHandler(ScriptObject frame, string eventName, string? extraParam = null);
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/UIOBJECT_Object
    /// </summary>
    public class ScriptObject : FrameScriptObject
    {
        protected FrameScriptObject? _parent;
        //private string? _parentKey;
        
        private readonly Dictionary<string, List<ScriptHandler>?> _scripts;
        private readonly Dictionary<string, int> _scriptRefs;

        protected ScriptObject(string objectType, string? name, FrameScriptObject? parent) : base(objectType, name)
        {
            _scripts = new Dictionary<string, List<ScriptHandler>?>();
            _scriptRefs = new Dictionary<string, int>();
            _parent = parent;
        }

        /// <summary>
        /// https://warcraft.wiki.gg/wiki/API_Object_ClearParentKey
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void ClearParentKey() { throw new NotImplementedException(); }
        private int internal_ClearParentKey(lua_State L)
        {
            var obj = GetThis(L, 1) as ScriptObject;
            obj?.ClearParentKey();
            return 0;
        }

        /// <summary>
        /// https://warcraft.wiki.gg/wiki/API_Object_GetDebugName
        /// </summary>
        /// <param name="preferParentKey"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public string GetDebugName(bool preferParentKey = false)
        {
            throw new NotImplementedException();
        }
        private int internal_GetDebugName(lua_State L)
        {
            var obj = GetThis(L, 1) as ScriptObject;
            bool preferParentKey = false;
            if (lua_gettop(L) >= 2)
            {
                preferParentKey = (lua_toboolean(L, 2) != 0);
            }
            lua_pushstring(L, obj?.GetDebugName(preferParentKey) ?? "");
            return 1;
        }
        
        /// <summary>
        /// https://warcraft.wiki.gg/wiki/API_Object_GetParent
        /// </summary>
        /// <returns></returns>
        public FrameScriptObject? GetParent() => _parent;
        private int internal_GetParent(lua_State L)
        {
            var obj = GetThis(L, 1) as ScriptObject;
            var parent = obj?.GetParent();
            if (parent == null)
            {
                lua_pushnil(L);
                return 1;
            }
            
            LuaHelpers.PushFrameScriptObject(L, parent);
            return 1;
        }

        /// <summary>
        /// https://warcraft.wiki.gg/wiki/API_Object_GetParentKey
        /// </summary>
        /// <returns></returns>
        public string? GetParentKey()
        {
            throw new NotImplementedException();
            /*=> _parentKey;*/
        }

        private int internal_GetParentKey(lua_State L)
        {
            var obj = GetThis(L, 1) as ScriptObject;
            string? parentKey = obj?.GetParentKey();
            lua_pushstring(L, parentKey ?? "");
            return 1;
        }
        
        /// <summary>
        /// https://warcraft.wiki.gg/wiki/API_Object_SetParentKey
        /// </summary>
        /// <param name="key"></param>
        public void SetParentKey(string key)
        {
            throw new NotImplementedException();
            /*_parentKey = key;*/
        }

        private int internal_SetParentKey(lua_State L)
        {
            var obj = GetThis(L, 1) as ScriptObject;
            if (obj != null)
            {
                var parentKey = lua_tostring(L, 2) ?? "";
                obj.SetParentKey(parentKey);
            }
            return 0;
        }

        /// <summary>
        /// https://warcraft.wiki.gg/wiki/API_ScriptObject_GetScript
        /// </summary>
        /// <param name="scriptTypeName"></param>
        /// <param name="bindingType"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public ScriptHandler GetScript(string scriptTypeName, string? bindingType = null)
        {
            return _scripts.ContainsKey(scriptTypeName) ? _scripts[scriptTypeName][0] : null;
        }
        private int internal_GetScript(lua_State L)
        {
            var obj = GetThis(L, 1) as ScriptObject;
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
        
        /// <summary>
        /// https://warcraft.wiki.gg/wiki/API_ScriptObject_HasScript
        /// </summary>
        /// <param name="scriptName"></param>
        /// <returns></returns>
        public bool HasScript(string scriptName)
        {
            return _scripts.ContainsKey(scriptName);
        }
        private int internal_HasScript(lua_State L)
        {
            var obj = GetThis(L, 1) as ScriptObject;
            string scriptName = lua_tostring(L, 2) ?? "";
            bool hasIt = obj?.HasScript(scriptName) ?? false;
            lua_pushboolean(L, hasIt ? 1 : 0);
            return 1;
        }
        
        /// <summary>
        /// https://warcraft.wiki.gg/wiki/API_ScriptObject_HookScript
        /// </summary>
        /// <param name="scriptTypeName"></param>
        /// <param name="script"></param>
        /// <param name="refIndex"></param>
        public void HookScript(string scriptTypeName, ScriptHandler script, int refIndex)
        {
            if (!_scripts.ContainsKey(scriptTypeName)) _scripts[scriptTypeName] = [];

            _scripts[scriptTypeName]?.Add(script);
            _scriptRefs[scriptTypeName] = refIndex;

            Log.HookScript(scriptTypeName, this);
        }
        public int internal_HookScript(lua_State L)
        {
            //return 0;
            
            try
            {
                // Ensure there are exactly 3 arguments: frame, scriptTypeName, handler
                var argc = lua_gettop(L);
                if (argc != 3)
                {
                    Log.ErrorL(L, "HookScript requires exactly 3 arguments: frame, scriptTypeName, handler.");
                    return 0; // Unreachable
                }

                // Validate that the second argument is a string
                if (!LuaHelpers.IsString(L, 2))
                {
                    Log.ErrorL(L, "HookScript: 'scriptTypeName' must be a string.");
                    return 0; // Unreachable
                }

                var scriptTypeName = lua_tostring(L, 2);

                // Validate that the third argument is a function
                if (lua_isfunction(L, 3) == 0)
                {
                    Log.ErrorL(L, "HookScript: 'handler' must be a function.");
                    return 0; // Unreachable
                }

                // Retrieve the Frame object
                var frame = GetThis(L, 1) as ScriptObject;
                if (frame == null)
                {
                    Log.ErrorL(L, "HookScript: Invalid Frame object.");
                    return 0; // Unreachable
                }

                // Create a reference to the Lua function
                lua_pushvalue(L, 3); // Push the function to the top of the stack
                var refIndex = luaL_ref(L, LUA_REGISTRYINDEX);

                // Assign the script handler
                frame.HookScript(scriptTypeName, (f, eventName, extraParam) =>
                {
                    try
                    {
                        // Retrieve the Lua function from the registry
                        lua_rawgeti(L, LUA_REGISTRYINDEX, refIndex);

                        // Retrieve the userdata from the registry using LuaRegistryRef
                        lua_rawgeti(L, LUA_REGISTRYINDEX, f.LuaRegistryRef); // Push 'self' userdata

                        // Push 'eventName'
                        lua_pushstring(L, eventName); // Push 'event'

                        // Call the Lua function with 2 arguments, 0 results
                        if (lua_pcall(L, 2, 0, 0) != 0)
                        {
                            // Retrieve and log the error message
                            var error = lua_tostring(L, -1);
                            Console.WriteLine($"Lua Error in HookScript '{scriptTypeName}': {error}");
                            lua_pop(L, 1); // Remove error message
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Exception while executing HookScript '{scriptTypeName}': {ex.Message}");
                    }
                }, refIndex); // Pass the refIndex to manage references

                // Push 'true' to indicate success
                lua_pushboolean(L, 1);
                return 1;
            }
            catch (Exception ex)
            {
                // Log the exception
                //Console.WriteLine($"Exception in HookScript: {ex.Message}");
                Log.Exception(L, ex, "HookScript");
                Log.ErrorL(L, "HookScript encountered an error.");
                return 0; // Unreachable
            }
        }
        
        /// <summary>
        /// https://warcraft.wiki.gg/wiki/API_ScriptObject_SetScript
        /// </summary>
        /// <param name="scriptTypeName"></param>
        /// <param name="script"></param>
        /// <param name="refIndex"></param>
        public void SetScript(string? scriptTypeName, ScriptHandler? script, int? refIndex = null)
        {
            if (scriptTypeName == null)
            {
                AnsiConsole.WriteLine("SetScript: scriptTypeName is null.");
                return;
            }
            
            if (script == null)
            {
                if (_scripts.ContainsKey(scriptTypeName))
                {
                    _scripts.Remove(scriptTypeName);
                    _scriptRefs.Remove(scriptTypeName);
                }
                return;
            }

            if (!_scripts.ContainsKey(scriptTypeName)) _scripts[scriptTypeName] = [];

            if (_scripts[scriptTypeName].Count > 0)
                // Replace the first (primary) handler
                _scripts[scriptTypeName][0] = script;
            else
                _scripts[scriptTypeName].Add(script);
        }
        public int internal_SetScript(lua_State L)
        {
            //return 0;
            
            // Ensure there are at least 2 arguments: Frame and scriptTypeName
            var argc = lua_gettop(L);
            if (argc < 3)
            {
                Log.ErrorL(L, "SetScript requires at least 2 arguments: frame and scriptTypeName.");
                return 0; // Unreachable
            }
            
            // Validate that the second argument is a string
            if (!LuaHelpers.IsString(L, 2))
            {
                Log.ErrorL(L, "SetScript: 'scriptTypeName' must be a string.");
                return 0; // Unreachable
            }

            // Now safely retrieve the scriptTypeName
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
                    var frame = GetThis(L, 1) as ScriptObject;

                    
                    //AnsiConsole.MarkupLine($"[yellow]Setting script '{scriptTypeName}' for frame {frame}[/]");
                    
                    // Assign the script handler with proper 'self' and 'event' parameters
                    frame.SetScript(scriptTypeName, (f, eventName, extraParam) =>
                    {
                        try
                        {
                            //AnsiConsole.MarkupLine($"[yellow]Handling event '{eventName}' with param '{extraParam}' for frame {f}[/]");

                            // Retrieve the Lua function from the registry
                            lua_rawgeti(L, LUA_REGISTRYINDEX, refIndex);

                            // Retrieve the userdata from the registry using LuaRegistryRef
                            lua_rawgeti(L, LUA_REGISTRYINDEX, f.LuaRegistryRef); // Push 'self' userdata

                            // Push 'eventName'
                            lua_pushstring(L, eventName); // Push 'event'

                            // Push 'extraParam' if present
                            if (!string.IsNullOrEmpty(extraParam)) lua_pushstring(L, extraParam);

                            // Determine the number of arguments (2 or 3)
                            var args = string.IsNullOrEmpty(extraParam) ? 2 : 3;
                            
                            // Call the Lua function with 'self', 'eventName', and optionally 'extraParam'
                            var status = lua_pcall(L, args, 0, 0);

                            if (status != 0)
                            {
                                // Retrieve and log the error message
                                var errorMessage = lua_tostring(L, -1);
                                var stackTrace = lua_tostring(L, -2); // Optional if available
                                Log.ErrorL(L, $"Error in event '{eventName}': {errorMessage}. StackTrace {stackTrace}");
                                lua_pop(L, 1);
                            }
                            //AnsiConsole.MarkupLine($"[green]Successfully handled event '{eventName}'.[/]");
                        }
                        catch (Exception ex)
                        {
                            Log.Error($"Exception while executing script [green]'{scriptTypeName}'[/] event [yellow]{eventName}[/] for [yellow]{f}[/]");
                            Log.Exception(L, ex, "SetScript");
                        }
                    }, refIndex); // Pass the refIndex to manage references


                    // Optionally, log the successful script assignment
                    Log.ScriptSet(scriptTypeName, frame);
                }
                else if (isNil)
                {
                    // Retrieve the Frame object
                    var frame = GetThis(L, 1) as ScriptObject;

                    // Remove the script handler
                    frame?.SetScript(scriptTypeName, null);
                    Log.RemoveScript(scriptTypeName, frame);
                }

                // Push 'true' to indicate success
                lua_pushboolean(L, 1);
                return 1;
            }

            Log.ErrorL(L, "SetScript: Invalid arguments. Expected a function or nil as the third argument.");
            return 0; // Unreachable
        }
        
        // ----------- Virtual Registration ---------------
        
        public override string GetMetatableName() => "ScriptObjectMetaTable";
        
        public override void RegisterMetaTable(lua_State L)
        {
            // 1) Register the base class's metatable first
            base.RegisterMetaTable(L);

            // 2) Define "UIObjectMetaTable"
            string metaName = GetMetatableName();
            luaL_newmetatable(L, metaName);

            // 3) __index = UIObjectMetaTable
            lua_pushvalue(L, -1);
            lua_setfield(L, -2, "__index");

            // 4) Link to the base class's metatable ("FrameScriptObjectMetaTable")
            var baseMetaName = base.GetMetatableName();
            luaL_getmetatable(L, baseMetaName);
            lua_setmetatable(L, -2); // Sets UIObjectMetaTable's metatable to FrameScriptObjectMetaTable

            // 5) Bind ScriptObject-specific methods
            LuaHelpers.RegisterMethod(L, "ClearParentKey", internal_ClearParentKey);
            LuaHelpers.RegisterMethod(L, "GetDebugName", internal_GetDebugName);
            LuaHelpers.RegisterMethod(L, "GetParent", internal_GetParent);
            LuaHelpers.RegisterMethod(L, "GetParentKey", internal_GetParentKey);
            LuaHelpers.RegisterMethod(L, "SetParentKey", internal_SetParentKey);
            LuaHelpers.RegisterMethod(L, "GetScript", internal_GetScript);
            LuaHelpers.RegisterMethod(L, "HasScript", internal_HasScript);
            LuaHelpers.RegisterMethod(L, "HookScript", internal_HookScript);
            LuaHelpers.RegisterMethod(L, "SetScript", internal_SetScript);

            // 6) pop
            lua_pop(L, 1);
        }
        
        // ----------- Events ---------------

        public void TriggerEvent(string eventName, string? param = null)
        {
            // Handle 'OnEvent' script type
            if (_scripts.TryGetValue("OnEvent", out var eventHandlers))
                // Make a copy to prevent modification during iteration
                if (eventHandlers != null)
                {
                    var handlersCopy = new List<ScriptHandler>(eventHandlers);
                    foreach (var handler in handlersCopy)
                    {
                        handler(this, eventName, param);
                    }
                }

            switch (eventName)
            {
                // Handle 'OnShow' script type
                case "Show" when _scripts.TryGetValue("OnShow", out var showHandlers):
                {
                    if (showHandlers != null)
                    {
                        var handlersCopy = new List<ScriptHandler>(showHandlers);
                        foreach (var handler in handlersCopy) handler(this, eventName, param);
                    }

                    break;
                }
                // Handle 'OnHide' script type
                case "Hide" when _scripts.TryGetValue("OnHide", out var hideHandlers):
                {
                    if (hideHandlers != null)
                    {
                        var handlersCopy = new List<ScriptHandler>(hideHandlers);
                        foreach (var handler in handlersCopy) handler(this, eventName, param);
                    }

                    break;
                }
            }
        }
    }
}