using System.Runtime.InteropServices;
using LuaNET.Lua51;
using static LuaNET.Lua51.Lua;

namespace WoWFrameTools.Widgets
{
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/UIOBJECT_ScriptRegion
    /// </summary>
    public class ScriptRegion : ScriptObject, IScriptRegionResizing
    {
        public Dictionary<string, Point> _points { get; set; }
        public float _width { get; set; }
        public float _height { get; set; }
        public float _alpha { get; set; }
        public float _scale { get; set; }
        private bool _visible;
        private bool _mouseEnabled;

        protected ScriptRegion(string objectType, string? name, ScriptRegion? parent) : base(objectType, name, parent)
        {
            _points = [];
        }
        // ScriptRegion:CanChangeProtectedState() : canChange - Returns true if protected properties of the region can be changed by non-secure scripts.
        // ScriptRegion:CollapsesLayout() : collapsesLayout
        
        /// <summary>
        /// https://warcraft.wiki.gg/wiki/API_ScriptRegion_EnableMouse
        /// ScriptRegion:EnableMouse([enable]) - Sets whether the region should receive mouse input.
        /// </summary>
        /// <param name="enable"></param>
        private void EnableMouse(bool enable)
        {
            _mouseEnabled = enable;
        }
        private int internal_EnableMouse(lua_State L)
        {
            var frame = GetThis(L, 1) as ScriptRegion;

            var argc = lua_gettop(L);
            if (argc < 2)
            {
                Log.ErrorL(L, "EnableMouse requires exactly 1 argument: enable.");
                return 0; // Unreachable
            }

            var enable = lua_toboolean(L, 2) != 0;
            frame?.EnableMouse(enable);

            return 0;
        }
        
        // ScriptRegion:EnableMouseMotion([enable]) - Sets whether the region should receive mouse hover events.
        // ScriptRegion:EnableMouseWheel([enable]) - Sets whether the region should receive mouse wheel input.
        // ScriptRegion:GetBottom() : bottom #restrictedframe - Returns the offset to the bottom edge of the region.
        // ScriptRegion:GetCenter() : x, y #restrictedframe - Returns the offset to the center of the region.
        
        /// <summary>
        /// https://warcraft.wiki.gg/wiki/API_ScriptRegion_GetSize
        /// ScriptRegion:GetHeight([ignoreRect]) : height - Returns the height of the region.
        /// </summary>
        /// <returns></returns>
        private float GetHeight()
        {
            return _height;
        }
        private int internal_GetHeight(lua_State L)
        {
            var frame = GetThis(L, 1) as ScriptRegion;
            var height = frame?.GetHeight() ?? 0;

            lua_pushnumber(L, height);
            return 1;
        }
        
        // ScriptRegion:GetLeft() : left #restrictedframe - Returns the offset to the left edge of the region.
        // ScriptRegion:GetRect() : left, bottom, width, height #restrictedframe - Returns the coords and size of the region.
        // ScriptRegion:GetRight() : right #restrictedframe - Returns the offset to the right edge of the region.
        // ScriptRegion:GetScaledRect() : left, bottom, width, height - Returns the scaled coords and size of the region.
        // ScriptRegion:GetSize([ignoreRect]) : width, height - Returns the width and height of the region.
        // ScriptRegion:GetSourceLocation() : location - Returns the script name and line number where the region was created.
        // ScriptRegion:GetTop() : top #restrictedframe - Returns the offset to the top edge of the region.
        
        /// <summary>
        /// https://warcraft.wiki.gg/wiki/API_ScriptRegion_GetSize
        /// ScriptRegion:GetWidth([ignoreRect]) : width - Returns the width of the region.
        /// </summary>
        /// <returns></returns>
        private float GetWidth()
        {
            return _width;
        }
        private int internal_GetWidth(lua_State L)
        {
            var frame = GetThis(L, 1) as ScriptRegion;
            var height = frame?.GetWidth() ?? 0;

            lua_pushnumber(L, height);
            return 1;
        }
        
        /// <summary>
        /// https://warcraft.wiki.gg/wiki/API_ScriptRegion_Hide
        /// ScriptRegion:Hide() #secureframe - Hides the region.
        /// </summary>
        [Attributes.SecureFrame]
        private void Hide()
        {
            if (_visible)
            {
                _visible = false;
                
                // Fire OnHide event
                TriggerEvent("Hide");
                
                // TODO : Pause OnUpdate script
            }
        }
        private int internal_Hide(lua_State L)
        {
            var frame = GetThis(L, 1) as ScriptRegion;
            frame?.Hide();
            return 0;
        }
        
        // ScriptRegion:IsCollapsed() : isCollapsed
        // ScriptRegion:SetCollapsesLayout(collapsesLayout)
        // ScriptRegion:IsAnchoringRestricted() : isRestricted - Returns true if the region has cross-region anchoring restrictions applied.
        // ScriptRegion:IsDragging() : isDragging - Returns true if the region is being dragged.
        // ScriptRegion:IsMouseClickEnabled() : enabled - Returns true if the region can receive mouse clicks.
        // ScriptRegion:IsMouseEnabled() : enabled - Returns true if the region can receive mouse input.
        // ScriptRegion:IsMouseMotionEnabled() : enabled - Returns true if the region can receive mouse hover events.
        // ScriptRegion:IsMouseMotionFocus() : isMouseMotionFocus - Returns true if the mouse cursor is hovering over the region.
        // ScriptRegion:IsMouseOver([offsetTop [, offsetBottom [, offsetLeft [, offsetRight]]]]) : isMouseOver - Returns true if the mouse cursor is hovering over the region.
        // ScriptRegion:IsMouseWheelEnabled() : enabled - Returns true if the region can receive mouse wheel input.
        // ScriptRegion:IsProtected() : isProtected, isProtectedExplicitly - Returns whether the region is currently protected.
        // ScriptRegion:IsRectValid() : isValid - Returns true if the region can be positioned on the screen.
        // ScriptRegion:IsShown() : isShown - Returns true if the region should be shown; it depends on the parents if it's visible.
        // ScriptRegion:IsVisible() : isVisible - Returns true if the region and its parents are shown.
        // ScriptRegion:SetMouseClickEnabled([enabled]) - Sets whether the region should receive mouse clicks.
        // ScriptRegion:SetMouseMotionEnabled([enabled]) - Sets whether the region should receive mouse hover events.
        
        /// <summary>
        /// https://warcraft.wiki.gg/wiki/API_ScriptRegion_SetParent
        /// ScriptRegion:SetParent([parent]) - Sets the parent of the region.
        /// </summary>
        private void SetParent(ScriptRegion? parent)
        {
            _parent = parent;
        }
        private int internal_SetParent(lua_State L)
        {
            var frame = GetThis(L, 1) as ScriptRegion;
            var parent = GetThis(L, 2) as ScriptRegion;
            frame?.SetParent(parent);
            return 0;
        }
        
        // ScriptRegion:SetPassThroughButtons([button1, ...]) #nocombat - Allows the region to propagate mouse clicks to underlying regions or the world frame.
        // ScriptRegion:SetPropagateMouseClicks(propagate)
        // ScriptRegion:SetPropagateMouseMotion(propagate)
        // ScriptRegion:SetShown([show]) #secureframe - Shows or hides the region.
        
        /// <summary>
        /// https://warcraft.wiki.gg/wiki/API_ScriptRegion_Show
        /// ScriptRegion:Show() #secureframe - Shows the region.
        /// </summary>
        [Attributes.SecureFrame]
        private void Show()
        {
            if (!_visible)
            {
                _visible = true;
                
                // Fire OnShow script
                TriggerEvent("Show");
                // TODO : Resume OnUpdate script
            }
        }
        private int internal_Show(lua_State L)
        {
            var frame = GetThis(L, 1) as ScriptRegion;
            frame?.Show();
            return 0;
        }
        
        // IScriptRegionResizing //
        // ScriptRegionResizing:AdjustPointsOffset(x, y) #secureframe - Adjusts the x and y offset of the region.
        
        // ScriptRegionResizing:ClearAllPoints() - Removes all anchor points from the region.
        
        /// <summary>
        /// https://warcraft.wiki.gg/wiki/API_ScriptRegionResizing_ClearAllPoints
        /// </summary>
        public void ClearAllPoints()
        {
            _points.Clear();
        }
        public int internal_ClearAllPoints(lua_State L)
        {
            var frame = GetThis(L, 1) as ScriptRegion;

            frame?.ClearAllPoints();

            return 0;
        }
        
        
        // ScriptRegionResizing:ClearPoint(point) - Removes an anchor point from the region by name.
        // ScriptRegionResizing:ClearPointsOffset() #secureframe - Resets the x and y offset on the region to zero.
        // ScriptRegionResizing:GetNumPoints() : numPoints - Returns the number of anchor points for the region.
        // ScriptRegionResizing:GetPoint([anchorIndex [, resolveCollapsed]]) : point, relativeTo, relativePoint, offsetX, offsetY #restrictedframe - Returns an anchor point for the region.
        // ScriptRegionResizing:GetPointByName(point [, resolveCollapsed]) : point, relativeTo, relativePoint, offsetX, offsetY - Returns an anchor point by name for the region.
        
        /// <summary>
        /// https://warcraft.wiki.gg/wiki/API_ScriptRegionResizing_SetAllPoints
        /// ScriptRegionResizing:SetAllPoints(relativeTo [, doResize]) - Positions the region the same as another region.
        /// </summary>
        /// <param name="relativeTo"></param>
        /// <param name="doResize"></param>
        public void SetAllPoints(Frame? relativeTo = null, bool doResize = true)
        {
            
        }
        private int internal_SetAllPoints(lua_State L)
        {
            var frame = GetThis(L, 1) as ScriptRegion;
            if (frame == null)
            {
                lua_pushboolean(L, 0);
                return 1;
            }

            // Retrieve arguments: relativeTo (optional), doResize (optional)
            Frame? relativeTo = null;
            var doResize = false;

            if (lua_gettop(L) >= 2 && lua_isnil(L, 2) != 0)
            {
                var relativeToPtr = (IntPtr)lua_touserdata(L, 2);
                if (relativeToPtr != IntPtr.Zero && API.UIObjects._frameRegistry.TryGetValue(relativeToPtr, out var foundFrame))
                    relativeTo = foundFrame;
                else
                    throw new ArgumentException("Invalid relativeTo frame specified.");
            }

            if (lua_gettop(L) >= 3) doResize = lua_toboolean(L, 3) != 0;

            // Set all points relative to another frame
            frame.SetAllPoints(relativeTo, doResize);

            lua_pushboolean(L, 1);
            return 1;
        }
        
        /// <summary>
        /// https://warcraft.wiki.gg/wiki/API_ScriptRegionResizing_SetSize
        /// ScriptRegionResizing:SetHeight(height) - Sets the height of the region.
        /// </summary>
        /// <param name="height"></param>
        public void SetHeight(float height)
        {
            _height = height;
        }
        private int internal_SetHeight(lua_State L)
        {
            var frame = GetThis(L, 1) as ScriptRegion;

            var argc = lua_gettop(L);
            if (argc < 2)
            {
                Log.ErrorL(L, "SetHeight requires exactly 1 argument: height.");
                return 0; // Unreachable
            }

            var height = (float)lua_tonumber(L, 2);

            frame?.SetHeight(height);

            lua_pushboolean(L, 1);
            return 1;
        }
        
        /// <summary>
        /// https://warcraft.wiki.gg/wiki/API_ScriptRegionResizing_SetPoint
        /// ScriptRegionResizing:SetPoint(point [, relativeTo [, relativePoint]] [, offsetX, offsetY]) #anchorfamily - Sets an anchor point for the region.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="relativeTo"></param>
        /// <param name="relativePoint"></param>
        /// <param name="offsetX"></param>
        /// <param name="offsetY"></param>
        public void SetPoint(string point, Frame? relativeTo = null, string? relativePoint = null, float offsetX = 0, float offsetY = 0)
        {
            _points.Remove(point);
            _points.Add(point, new Point(point, relativeTo, relativePoint, offsetX, offsetY));
        }
        private int internal_SetPoint(lua_State L)
        {
            var region = GetThis(L, 1) as ScriptRegion;

            var argc = lua_gettop(L);
            if (argc < 2)
            {
                Log.ErrorL(L, "SetPoint requires at least 1 argument: point.");
                return 0; // Unreachable
            }

            var point = lua_tostring(L, 2);

            Frame? relativeTo = null;
            string? relativePoint = null;
            float offsetX = 0;
            float offsetY = 0;
            
            if (argc >= 3) relativeTo = LuaHelpers.GetFrame(L, 3);
            
            if (argc >= 4) relativePoint = lua_tostring(L, 4);

            if (argc >= 6)
            {
                offsetX = (float)lua_tonumber(L, 5);
                offsetY = (float)lua_tonumber(L, 6);
            }
            
            region?.SetPoint(point!, relativeTo, relativePoint, offsetX, offsetY);

            lua_pushboolean(L, 1);
            return 1;
        }
        
        /// <summary>
        /// https://warcraft.wiki.gg/wiki/API_ScriptRegionResizing_SetSize
        /// ScriptRegionResizing:SetSize(x, y) - Sets the width and height of the region.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void SetSize(float width, float height)
        {
            _width = width;
            _height = height;
        }
        private int internal_SetSize(lua_State L)
        {
            // Ensure there are exactly 3 arguments: frame, width, height
            var argc = lua_gettop(L);
            if (argc != 3)
            {
                Log.ErrorL(L, "SetSize requires exactly 3 arguments: frame, width, height.");
                return 0; // Unreachable
            }

            // Retrieve the Frame object
            var frame = GetThis(L, 1) as ScriptRegion;
            if (frame == null)
            {
                Log.ErrorL(L, "SetSize: Invalid Frame object.");
                return 0; // Unreachable
            }

            // Retrieve width and height
            if (!LuaHelpers.IsNumber(L, 2) || !LuaHelpers.IsNumber(L, 3))
            {
                Log.ErrorL(L, "SetSize: 'width' and 'height' must be numbers.");
                return 0; // Unreachable
            }

            var width = (float)lua_tonumber(L, 2);
            var height = (float)lua_tonumber(L, 3);

            // Set the size
            frame.SetSize(width, height);

            // Log the action
            //AnsiConsole.WriteLine($"SetSize called on Frame. Width: {width}, Height: {height}");

            // No return values
            return 0;
        }
        
        /// <summary>
        /// https://warcraft.wiki.gg/wiki/API_ScriptRegionResizing_SetSize
        /// ScriptRegionResizing:SetWidth(width) - Sets the width of the region.
        /// </summary>
        /// <param name="width"></param>
        public void SetWidth(float width)
        {
            _width = width;
        }
        private int internal_SetWidth(lua_State L)
        {
            var frame = GetThis(L, 1) as ScriptRegion;

            var argc = lua_gettop(L);
            if (argc < 2)
            {
                Log.ErrorL(L, "SetWidth requires exactly 1 argument: width.");
                return 0; // Unreachable
            }

            var width = (float)lua_tonumber(L, 2);

            frame?.SetWidth(width);

            lua_pushboolean(L, 1);
            return 1;
        }
        
        // ----------- Virtual Registration ---------------
        
        public override string GetMetatableName() => "ScriptRegionMetaTable";
        
        public override void RegisterMetaTable(lua_State L)
        {
            // 1) call base to register the "ScriptRegionMetaTable"
            base.RegisterMetaTable(L);

            // 2) Now define "ScriptRegionMetaTable"
            var metaName = GetMetatableName();
            luaL_newmetatable(L, metaName);

            // 3) __index = self
            lua_pushvalue(L, -1);
            lua_setfield(L, -2, "__index");

            // 4) Bind methods
            // ScriptRegion
            LuaHelpers.RegisterMethod(L, "Hide", internal_Hide);
            LuaHelpers.RegisterMethod(L, "Show", internal_Show);
            LuaHelpers.RegisterMethod(L, "EnableMouse", internal_EnableMouse);
            LuaHelpers.RegisterMethod(L, "SetParent", internal_SetParent);
            LuaHelpers.RegisterMethod(L, "GetHeight", internal_GetHeight);
            LuaHelpers.RegisterMethod(L, "GetWidth", internal_GetWidth);
            
            // IScriptRegionResizing
            LuaHelpers.RegisterMethod(L, "SetPoint", internal_SetPoint);
            LuaHelpers.RegisterMethod(L, "SetSize", internal_SetSize);
            LuaHelpers.RegisterMethod(L, "SetAllPoints", internal_SetAllPoints);
            LuaHelpers.RegisterMethod(L, "SetHeight", internal_SetHeight);
            LuaHelpers.RegisterMethod(L, "SetWidth", internal_SetWidth);
            LuaHelpers.RegisterMethod(L, "ClearAllPoints", internal_ClearAllPoints);
            
            
            // Optional __gc
            lua_pushcfunction(L, internal_ScriptRegionGC);
            lua_setfield(L, -2, "__gc");

            // 5) pop
            lua_pop(L, 1);
        }

        private int internal_ScriptRegionGC(lua_State L)
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
    }
}