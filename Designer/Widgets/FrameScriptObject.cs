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

        protected FrameScriptObject(string objectType, string? name)
        {
            _name = name;
            _objectType = objectType;
        }
        
        /// <summary>
        /// https://warcraft.wiki.gg/wiki/API_FrameScriptObject_GetName
        /// </summary>
        /// <returns></returns>
        public string? GetName() => _name;
        
        /// <summary>
        /// https://warcraft.wiki.gg/wiki/API_FrameScriptObject_GetObjectType
        /// </summary>
        /// <returns></returns>
        public string GetObjectType() => _objectType;
        
        /// <summary>
        /// https://warcraft.wiki.gg/wiki/API_FrameScriptObject_IsForbidden
        /// </summary>
        /// <returns></returns>
        public bool IsForbidden() => _isForbidden;
        
        /// <summary>
        /// https://warcraft.wiki.gg/wiki/API_FrameScriptObject_IsObjectType
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public bool IsObjectType(string objectType) => _objectType == objectType;
        
        /// <summary>
        /// https://warcraft.wiki.gg/wiki/API_FrameScriptObject_SetForbidden
        /// </summary>
        /// <param name="forbidden"></param>
        public void SetForbidden(bool forbidden)
        {
            _isForbidden = forbidden;
        }
    }
}