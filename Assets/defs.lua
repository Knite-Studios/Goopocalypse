-- This file contains IntelliSense definitions for the Lua scripting API.
-- Please do not call any global functions (unless explicitly stated).

Hero = {
    Name = "", -- string
    Attributes = {}, -- Dictionary<Attribute, object>
    GetAttributeValue = function(attribute) end, -- function(attribute: Attribute): object
    HasAttribute = function(attribute) end, -- function(attribute: Attribute): boolean
}
