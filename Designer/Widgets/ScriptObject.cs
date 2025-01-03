﻿using System;
using System.Runtime.InteropServices;
using LuaNET.Lua51;
using Spectre.Console;
using static LuaNET.Lua51.Lua;

namespace WoWFrameTools.Widgets
{
    public abstract class Parameters
    {
        public abstract string type { get; }
    }
    
    public class ShowParameters : Parameters
    {
        public override string type => "OnShow";
        
        public override string ToString()
        {
            return String.Empty;
        }
    }
    
    public class HideParameters : Parameters
    {
        public override string type => "OnHide";

        public override string ToString()
        {
            return String.Empty;
        }
    }
    
    public class EventParameters : Parameters
    {
        public override string type => "OnEvent";
        public string eventName { get; set; }
        public string? extraParam { get; set; }

        public override string ToString()
        {
            if (extraParam == null)
                return $"Event: {eventName}";
            else
                return $"Event: {eventName}, ExtraParam: {extraParam}";
        }
    }
    
    public class UpdateParameters : Parameters
    {
        public override string type => "OnUpdate";
        public float elapsed { get; set; }
        
        public override string ToString()
        {
            return $"Elapsed: {elapsed}";
        }
    }
    
    public delegate void ScriptHandler(ScriptObject frame, Parameters? parameters);
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/UIOBJECT_Object
    /// </summary>
    public class ScriptObject : FrameScriptObject
    {
        public ScriptObject? _parent;
        public readonly List<ScriptObject?> _children;
        
        //private string? _parentKey;
        private readonly Dictionary<string, List<ScriptHandler>?> _scripts;
        private readonly Dictionary<string, int> _scriptRefs;

        protected ScriptObject(string objectType, string? name, ScriptObject? parent) : base(objectType, name)
        {
            _scripts = [];
            _scriptRefs = [];
            _children = [];
            _parent = parent;
            parent?._children.Add(this);
        }

        /// <summary>
        /// https://warcraft.wiki.gg/wiki/API_Object_ClearParentKey
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void ClearParentKey() { throw new NotImplementedException(); }

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
        
        /// <summary>
        /// https://warcraft.wiki.gg/wiki/API_Object_GetParent
        /// </summary>
        /// <returns></returns>
        public FrameScriptObject? GetParent() => _parent;

        /// <summary>
        /// https://warcraft.wiki.gg/wiki/API_Object_GetParentKey
        /// </summary>
        /// <returns></returns>
        public string? GetParentKey()
        {
            throw new NotImplementedException();
            /*=> _parentKey;*/
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
        
        /// <summary>
        /// https://warcraft.wiki.gg/wiki/API_ScriptObject_HasScript
        /// </summary>
        /// <param name="scriptName"></param>
        /// <returns></returns>
        public bool HasScript(string scriptName)
        {
            return _scripts.ContainsKey(scriptName);
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
        
        // ----------- Scripts ---------------

        /// <summary>
        /// Static method to handle script callbacks to ensure delegates are static.
        /// </summary>

        public void OnShow()
        {
            // Handle 'OnEvent' script type
            if (_scripts.TryGetValue("OnShow", out var eventHandlers))
            {
                // Make a copy to prevent modification during iteration
                if (eventHandlers != null)
                {
                    var handlersCopy = new List<ScriptHandler>(eventHandlers);
                    foreach (var handler in handlersCopy)
                    {
                        var parameters = new ShowParameters();
                        handler(this, parameters);
                        Log.OnScript(parameters, this);
                    }
                }
                
            }
        }
        
        public void OnHide()
        {
            // Handle 'OnEvent' script type
            if (_scripts.TryGetValue("OnHide", out var eventHandlers))
            {
                // Make a copy to prevent modification during iteration
                if (eventHandlers != null)
                {
                    var handlersCopy = new List<ScriptHandler>(eventHandlers);
                    foreach (var handler in handlersCopy)
                    {
                        var parameters = new HideParameters();
                        handler(this, parameters);
                        Log.OnScript(parameters, this);
                    }
                }
            }
        }

        public void OnEvent(string eventName, string? param = null)
        {
            // Handle 'OnEvent' script type
            if (_scripts.TryGetValue("OnEvent", out var eventHandlers))
            {
                // Make a copy to prevent modification during iteration
                if (eventHandlers != null)
                {
                    var handlersCopy = new List<ScriptHandler>(eventHandlers);
                    foreach (var handler in handlersCopy)
                    {
                        var parameters = new EventParameters { eventName = eventName, extraParam = param };
                        handler(this, parameters);
                        Log.OnScript(parameters, this);
                    }
                }
            }
        }
        
        public void OnUpdate(float elapsed)
        {
            if (_scripts.TryGetValue("OnUpdate", out var eventHandlers))
            {
                // Make a copy to prevent modification during iteration
                if (eventHandlers != null)
                {
                    var handlersCopy = new List<ScriptHandler>(eventHandlers);
                    foreach (var handler in handlersCopy)
                    {
                        var parameters = new UpdateParameters { elapsed = elapsed };
                        handler(this, parameters);
                        //Log.OnScript(parameters, this);   // lolno
                    }
                }
            }
        }
    }
}