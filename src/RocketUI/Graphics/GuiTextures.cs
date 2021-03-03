using System;

namespace RocketUI
{
    public enum GuiSoundEffects
    {
        None,
        ButtonHighlight,
        ButtonClick
    }
    
    public struct GuiTextures
    {
        public const string DefaultNamespace = "RocketUI";
        
        public static readonly GuiTextures GameLogo = new GuiTextures(DefaultNamespace, nameof(GameLogo)); 
        public static readonly GuiTextures ProgressBar = new GuiTextures(DefaultNamespace, nameof(ProgressBar)); 
        
        public static readonly GuiTextures SplashBackground = new GuiTextures(DefaultNamespace, nameof(SplashBackground)); 

        public static readonly GuiTextures PanelGeneric = new GuiTextures(DefaultNamespace, nameof(PanelGeneric)); 
        public static readonly GuiTextures PanelGlass = new GuiTextures(DefaultNamespace, nameof(PanelGlass)); 
        public static readonly GuiTextures PanelGlassHighlight = new GuiTextures(DefaultNamespace, nameof(PanelGlassHighlight)); 
        
        public static readonly GuiTextures Crosshair = new GuiTextures(DefaultNamespace, nameof(Crosshair)); 
        
        public static readonly GuiTextures ButtonDefault = new GuiTextures(DefaultNamespace, nameof(ButtonDefault)); 
        public static readonly GuiTextures ButtonHover = new GuiTextures(DefaultNamespace, nameof(ButtonHover)); 
        public static readonly GuiTextures ButtonFocused = new GuiTextures(DefaultNamespace, nameof(ButtonFocused)); 
        public static readonly GuiTextures ButtonDisabled = new GuiTextures(DefaultNamespace, nameof(ButtonDisabled)); 

        public static readonly GuiTextures ScrollBarBackground = new GuiTextures(DefaultNamespace, nameof(ScrollBarBackground)); 

        public static readonly GuiTextures ScrollBarTrackDefault = new GuiTextures(DefaultNamespace, nameof(ScrollBarTrackDefault)); 
        public static readonly GuiTextures ScrollBarTrackHover = new GuiTextures(DefaultNamespace, nameof(ScrollBarTrackHover)); 
        public static readonly GuiTextures ScrollBarTrackFocused = new GuiTextures(DefaultNamespace, nameof(ScrollBarTrackFocused)); 
        public static readonly GuiTextures ScrollBarTrackDisabled = new GuiTextures(DefaultNamespace, nameof(ScrollBarTrackDisabled)); 

        public static readonly GuiTextures ScrollBarUpButtonDefault = new GuiTextures(DefaultNamespace, nameof(ScrollBarUpButtonDefault)); 
        public static readonly GuiTextures ScrollBarUpButtonHover = new GuiTextures(DefaultNamespace, nameof(ScrollBarUpButtonHover)); 
        public static readonly GuiTextures ScrollBarUpButtonFocused = new GuiTextures(DefaultNamespace, nameof(ScrollBarUpButtonFocused)); 
        public static readonly GuiTextures ScrollBarUpButtonDisabled = new GuiTextures(DefaultNamespace, nameof(ScrollBarUpButtonDisabled)); 

        public static readonly GuiTextures ScrollBarDownButtonDefault = new GuiTextures(DefaultNamespace, nameof(ScrollBarDownButtonDefault)); 
        public static readonly GuiTextures ScrollBarDownButtonHover = new GuiTextures(DefaultNamespace, nameof(ScrollBarDownButtonHover)); 
        public static readonly GuiTextures ScrollBarDownButtonFocused = new GuiTextures(DefaultNamespace, nameof(ScrollBarDownButtonFocused)); 
        public static readonly GuiTextures ScrollBarDownButtonDisabled = new GuiTextures(DefaultNamespace, nameof(ScrollBarDownButtonDisabled)); 
        
        public static readonly GuiTextures BarShadow = new GuiTextures(DefaultNamespace, nameof(BarShadow)); 
        public static readonly GuiTextures BarBlue = new GuiTextures(DefaultNamespace, nameof(BarBlue)); 
        public static readonly GuiTextures BarGreen = new GuiTextures(DefaultNamespace, nameof(BarGreen)); 
        public static readonly GuiTextures BarOrange = new GuiTextures(DefaultNamespace, nameof(BarOrange)); 
        public static readonly GuiTextures BarYellow = new GuiTextures(DefaultNamespace, nameof(BarYellow)); 
        public static readonly GuiTextures BatWhite = new GuiTextures(DefaultNamespace, nameof(BatWhite)); 
        
        public static readonly GuiTextures DotShadow = new GuiTextures(DefaultNamespace, nameof(DotShadow)); 
        public static readonly GuiTextures DotBlue = new GuiTextures(DefaultNamespace, nameof(DotBlue)); 
        public static readonly GuiTextures DotGreen = new GuiTextures(DefaultNamespace, nameof(DotGreen)); 
        public static readonly GuiTextures DotOrange = new GuiTextures(DefaultNamespace, nameof(DotOrange)); 
        public static readonly GuiTextures DotYellow = new GuiTextures(DefaultNamespace, nameof(DotYellow)); 
        public static readonly GuiTextures DotWhite = new GuiTextures(DefaultNamespace, nameof(DotWhite));


        public string Namespace;
        public string Key;
        
        public bool   HasValue => !string.IsNullOrEmpty(Key);

        public GuiTextures(string @namespace, string key)
        {
            Namespace = @namespace;
            Key = key;
        }

        public static implicit operator string(GuiTextures guiTextures)
        {
            return guiTextures.HasValue ? guiTextures.ToString() : null;
        }

        public override string ToString()
        {
            return $"{Namespace}:{Key}";
        }

        public static GuiTextures Parse(string text)
        {
            var split = text.Split(':', 2);
            if (split.Length == 2)
            {
                return new GuiTextures(split[0], split[1]);
            }

            throw new FormatException("Invalid GuiTextures format, expected 'namespace:key'");
        }
    }
}
