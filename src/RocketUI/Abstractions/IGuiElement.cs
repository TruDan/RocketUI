﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using RocketUI.Attributes;
using RocketUI.Serialization;

namespace RocketUI
{
    public interface IGuiElement
    {
        [DebuggerVisible]
        Guid Id { get; }
        string        Name       { get; set; }
        PropertyStore Properties { get; }
        object        Tag        { get; set; }

        IGuiScreen RootScreen { get; }
        IGuiElement ParentElement { get; set; }
        IGuiFocusContext FocusContext { get; set; }

        IGuiElement[] ChildElements { get; }

        [DebuggerVisible] bool HasChildren { get; }
        int X { set; }
        int Y { set; }
        
        
        [DebuggerVisible] Vector2 RenderPosition { get; }
        [DebuggerVisible] Size RenderSize { get; }
        [DebuggerVisible] Rectangle RenderBounds { get; }
        [DebuggerVisible] bool ClipToBounds { get; }



        void Init(IGuiRenderer renderer, bool force = false);

        void Update(GameTime gameTime);

        void Draw(GuiSpriteBatch graphics, GameTime gameTime);

        void AddChild(IGuiElement element);
        void RemoveChild(IGuiElement element);
        
        #region Hierachy Transcending

        bool TryFindParent(GuiElementPredicate predicate, out IGuiElement parentElement);
        bool TryFindParentOfType<TGuiElement>(GuiElementPredicate<TGuiElement> predicate, out TGuiElement parentElement) where TGuiElement : class, IGuiElement;

        bool TryTranscendChildren(GuiElementPredicate predicate, bool recurse = true);

        bool TryFindDeepestChild(GuiElementPredicate predicate, out IGuiElement childElement);
        bool TryFindDeepestChildOfType<TGuiElement>(GuiElementPredicate<TGuiElement> predicate, out TGuiElement childElement) where TGuiElement : class, IGuiElement;
        
        void ForEachChild(Action<IGuiElement> element);
        IEnumerable<TResult> ForEachChild<TResult>(Func<IGuiElement, TResult> valueSelector);
        
        #endregion

        #region Layout Engine

        #region Properties

        [DebuggerVisible] bool IsVisible { get; set; }
        
        [DebuggerVisible] int MinWidth  { get; set;}
        [DebuggerVisible] int MinHeight { get; set; }
        [DebuggerVisible] int MaxWidth  { get; set; }
        [DebuggerVisible] int MaxHeight { get; set; }
        
        [DebuggerVisible] int Width  { get; set; }
        [DebuggerVisible] int Height { get; set; }
        
        [DebuggerVisible] Thickness Margin  { get; set; }
        [DebuggerVisible] Thickness Padding { get; set; }
        
        [DebuggerVisible] AutoSizeMode AutoSizeMode { get; set; }
        [DebuggerVisible] Alignment    Anchor       { get; set; }

        #endregion


        #region Layout Calculation State Properties

        [DebuggerVisible] int LayoutOffsetX { get; }
        [DebuggerVisible] int LayoutOffsetY { get; }
        [DebuggerVisible] int LayoutWidth   { get; }
        [DebuggerVisible] int LayoutHeight  { get; }
        
        [DebuggerVisible] bool IsLayoutDirty { get; }

        [DebuggerVisible] Size PreferredSize { get; }
        [DebuggerVisible] Size PreferredMinSize { get; }
        [DebuggerVisible] Size PreferredMaxSize { get; }

        #endregion
        
        #region Calculated Properties

        [DebuggerVisible] Rectangle OuterBounds { get; }
        [DebuggerVisible] Rectangle Bounds { get; }
        [DebuggerVisible] Rectangle InnerBounds { get; }
        [DebuggerVisible] Point Position { get; }
        
        [DebuggerVisible] Size Size { get; }
        
        #endregion


        Size Measure(Size availableSize);
        void Arrange(Rectangle newBounds);

        void InvalidateLayout(bool invalidateChildren = true);
        void InvalidateLayout(IGuiElement sender, bool invalidateChildren = true);



        #endregion
    }
}
