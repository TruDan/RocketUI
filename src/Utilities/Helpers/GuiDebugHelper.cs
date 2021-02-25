﻿using System;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using RocketUI.Layout;
using RocketUI.Utilities.Extensions;

namespace RocketUI.Utilities.Helpers
{

    public class GuiDebugHelper : GameComponent, IDisposable
    {
		
		private static readonly float DebugFontScale = 0.5f;

		private static readonly Color DebugTextBackground = Color.WhiteSmoke * 0.6f;
		private static readonly Color DebugTextForeground = Color.Black * 0.95f;

		private static readonly Color OuterBoundsBackground = Color.LightGoldenrodYellow * 0.1f;
		private static readonly Color BoundsBackground = Color.LightSeaGreen * 0.2f;
		private static readonly Color InnerBoundsBackground = Color.CornflowerBlue * 0.1f;

		public bool Enabled { get; set; } = false;
		public bool BoundingBoxesEnabled { get; set; } = true;
		public bool BoundingBoxesHoverEnabled { get; set; } = true;
		public bool HoverInfoEnabled { get; set; } = true;

		public Keys ToggleDebugHotKey { get; set; } = Keys.Pause;

		protected GuiManager GuiManager { get; }

		protected IGuiRenderer Renderer => GuiManager.GuiRenderer;
		protected GuiSpriteBatch SpriteBatch => GuiManager.GuiSpriteBatch;

		private KeyboardState _previousKeyboard, _currentKeyboard;
		private MouseState _previousMouse, _currentMouse;

		private Vector2 _cursorPosition;
		private RocketElement _topMostHighlighted;
		private RocketElement _topMostFocused;

		public IGuiElement HighlightedElement;

		public GuiDebugHelper(Game game, GuiManager manager) : base(game)
		{
			GuiManager = manager;

			GuiManager.DrawScreen -= GuiManagerOnDrawScreen;
			GuiManager.DrawScreen += GuiManagerOnDrawScreen;
		}

        public RocketElement TopMostHighlighted
        {
            get { return _topMostHighlighted; }
        }

        private void GuiManagerOnDrawScreen(object sender, GuiDrawScreenEventArgs e)
		{
			DrawScreen(e.Screen);
		}


		public override void Update(GameTime gameTime)
		{
			if(!Enabled)
				return;
			
			base.Update(gameTime);

			_previousKeyboard = _currentKeyboard;
			_currentKeyboard = Keyboard.GetState();

			_previousMouse = _currentMouse;
			_currentMouse = Mouse.GetState();

			if (_previousKeyboard.IsKeyDown(ToggleDebugHotKey) && _currentKeyboard.IsKeyUp(ToggleDebugHotKey))
			{
				Enabled = !Enabled;
            }

			if (!Enabled) return;
			if (!BoundingBoxesEnabled) return;

			if ((_previousMouse.LeftButton == ButtonState.Pressed && _currentMouse.LeftButton != ButtonState.Pressed)
				|| (_previousMouse.RightButton == ButtonState.Pressed && _currentMouse.RightButton != ButtonState.Pressed))
				{
				_topMostFocused = _topMostFocused == null ? _topMostHighlighted : null;
			}

			if (_previousKeyboard.IsKeyDown(Keys.Escape) && _currentKeyboard.IsKeyUp(Keys.Escape))
			{
				_topMostFocused = null;
			}

			// add extra updates below here
			if (_topMostFocused == null)
			{
				_cursorPosition = Renderer.Unproject(_currentMouse.Position.ToVector2());
			}

			if (GuiManager.FocusManager.TryGetElementAt(_cursorPosition, e => e is RocketElement c,
														out var controlMatchingPosition))
			{
				_topMostHighlighted = controlMatchingPosition as RocketElement;
			}
			else
			{
				_topMostHighlighted = null;
			}

		}

		public void DrawScreen(IGuiScreen screen)
		{
			if (!Enabled) return;

            using (SpriteBatch.BeginClipBounds(screen.RenderBounds, false))
            {
                
			    if (HighlightedElement != null)
			    {
				    DrawDebug(HighlightedElement);
			    }
                if (BoundingBoxesEnabled)
                {
                	screen.ForEachChild(c => DrawElementRecursive(c));
                }

                if (BoundingBoxesHoverEnabled)
                {
                	DrawDebug(TopMostHighlighted);
                }

                // draw info at cursor
                if (HoverInfoEnabled)
                {
                	var e = _topMostFocused ?? TopMostHighlighted;
                	if (e != null)
                	{
                		var p = e.ParentElement as RocketElement;

                		var info = GetElementInfo(e);

                		DrawDebugString(_cursorPosition, info, Color.WhiteSmoke * 0.85f, Color.Black, 2, 1, 1);

                		if (p != null)
                		{
                			var infoParent = GetElementInfo(p);
                			DrawDebugString(_cursorPosition, infoParent, Color.WhiteSmoke * 0.85f, Color.Black, 2, -1, 1);
                		}
                	}
                }   
            }
        }

		private string GetElementInfo(RocketElement e)
		{
			var p = e.ParentElement as RocketElement;

			var info = new StringBuilder();

			info.AppendLine($"Type: {e.GetType().Name}");
			info.AppendLine($"Position: {e.Position}");
			info.AppendLine($"Size: {e.Size}");
			info.AppendLine($"Min/Pref/Max W/H: {e.PreferredMinSize}, {e.PreferredSize}, {e.PreferredMaxSize}");
			info.AppendLine($"Bounds: {e.Bounds}");
			info.AppendLine();
			info.AppendLine($"Anchor: {e.Anchor.ToString()} - {e.Anchor.ToFullString()}");
			info.AppendLine($"AutoSizeMode: {e.AutoSizeMode.ToString()}");
			info.AppendLine();
			info.AppendLine($"Margin: {e.Margin}");
			info.AppendLine($"Padding: {e.Padding}");
			info.AppendLine($"Layout X/Y: {e.LayoutOffsetX}, {e.LayoutOffsetY}");
			info.AppendLine($"Layout W/H: {e.LayoutWidth} x {e.LayoutHeight}");
			info.AppendLine();
			info.AppendLine($"Parent Type: {p?.GetType().Name ?? "--"}");
			//info.AppendLine($"Siblings: ({p?.ChildCount - 1 ?? 0}) {{{string.Join(", ", p?.AllChildren.Where(c => c != e).Select(c => c.GetType().Name) ?? new string[0])}}}");
			//info.AppendLine($"Children: ({e.ChildCount}) {{{string.Join(", ", e.AllChildren.Select(c => c.GetType().Name))}}}");

			if (e is StackContainer eStack)
			{
				info.AppendLine();
				info.AppendLine($"Stack - Orientation: {eStack.Orientation.ToString()}");
				info.AppendLine($"Stack - ChildAnchor: {eStack.ChildAnchor.ToString()} - {eStack.ChildAnchor.ToFullString()}");

				info.AppendLine();
				var childAlign = StackContainer.NormalizeAlignmentForArrange(eStack.Orientation, eStack.ChildAnchor);
				info.AppendLine($"Stack - Child Alignment: {childAlign.ToString()} - {childAlign.ToFullString()}");

			}

			return info.ToString();
		}

		private void DrawElementRecursive(IGuiElement element)
		{
			DrawDebug(element);

			element.ForEachChild(c => DrawElementRecursive(c));
		}

		private void DrawDebug(IGuiElement element)
		{
            if (element == null) return;
			if (!Enabled) return;
			var isHighlighted = element == _topMostHighlighted;


			if (element.OuterBounds != element.Bounds)
			{
				DrawDebugBounds(element.OuterBounds, OuterBoundsBackground, false, true, false, false);
			}

			DrawDebugBounds(element.Bounds, BoundsBackground, false, true, false, false);

			if (element.AutoSizeMode == AutoSizeMode.None)
			{
				DrawDebugBounds(element.RenderBounds, Color.Blue, false, true, isHighlighted, isHighlighted);
			}

			if (element.AutoSizeMode == AutoSizeMode.GrowAndShrink)
			{
				DrawDebugBounds(element.RenderBounds, Color.YellowGreen, false, true, isHighlighted, isHighlighted);
			}

			if (element.AutoSizeMode == AutoSizeMode.GrowOnly)
			{
				DrawDebugBounds(element.RenderBounds, Color.LawnGreen, false, true, isHighlighted, isHighlighted);
			}

			if (element.InnerBounds != element.Bounds)
			{
				DrawDebugBounds(element.InnerBounds, InnerBoundsBackground, true, true, false, false);
			}

			// cursor highlight 
			{
				if (element.OuterBounds.Contains(_cursorPosition))
				{
					DrawDebugBounds(element.OuterBounds, Color.OrangeRed, isHighlighted, true, false, false);
				}

				if (element.Bounds.Contains(_cursorPosition))
				{
					DrawDebugBounds(element.Bounds, Color.Red, isHighlighted, true, false, isHighlighted, isHighlighted);
					if (isHighlighted)
						DrawDebugString(element.Bounds.TopCenter(), element.GetType().Name, Color.Red * 0.25f, Color.White);
				}

				if (element.Bounds.Contains(_cursorPosition))
				{
					DrawDebugBounds(element.InnerBounds, Color.MediumVioletRed, isHighlighted, true, false, false);
				}
			}

			if (isHighlighted)
			{
				DrawDebugString(element.Bounds.TopCenter(), element.GetType().Name);
			}
		}

		#region Draw Helpers

		private void DrawDebugBounds(Rectangle bounds, Color color, bool drawBackground = false, bool drawBorders = true, bool drawCoordinates = false, bool drawSize = false, bool highlight = false)
		{
			// Bounding Rectangle
			if (drawBackground)
			{
				SpriteBatch.FillRectangle(bounds, color * 0.15f);
			}

			if (drawBorders)
			{
				SpriteBatch.DrawRectangle(bounds, color, 1);
			}

			var bgColor = highlight ? color * 0.2f : DebugTextBackground;
			var fgColor = highlight ? color : DebugTextForeground;

			var pos = bounds.Location;
			if (drawCoordinates)
			{
				DrawDebugString(bounds.TopLeft(), $"({pos.X}, {pos.Y})", bgColor, fgColor, Alignment.BottomLeft);
			}

			if (drawSize)
			{
				DrawDebugString(bounds.TopRight(), $"[{bounds.Width} x {bounds.Height}]", bgColor, fgColor);
			}
		}

		private void DrawDebugString(Vector2 position,   object obj, Alignment align = Alignment.TopLeft)
        {
            var x = (align & (Alignment.CenterX | Alignment.FillX)) != 0 ? 0 : ((align & Alignment.MinX) != 0 ? -1 : 1);
            var y = (align & (Alignment.CenterY | Alignment.FillY)) != 0 ? 0 : ((align & Alignment.MinY) != 0 ? -1 : 1);

            DrawDebugString(position, obj.ToString(), DebugTextBackground, DebugTextForeground, 2, x, y);
        }
        private void DrawDebugString(Vector2 position, object obj, Color color, Alignment align = Alignment.TopLeft)
        {
            var x = (align & (Alignment.CenterX | Alignment.FillX)) != 0 ? 0 : ((align & Alignment.MinX) != 0 ? -1 : 1);
            var y = (align & (Alignment.CenterY | Alignment.FillY)) != 0 ? 0 : ((align & Alignment.MinY) != 0 ? -1 : 1);

            DrawDebugString(position, obj.ToString(), DebugTextBackground, color, 2, x, y);
        }
        private void DrawDebugString(Vector2 position, object obj, Color? background, Color color, Alignment align = Alignment.TopLeft)
        {
            var x = (align & (Alignment.CenterX | Alignment.FillX)) != 0 ? 0 : ((align & Alignment.MinX) != 0 ? -1 : 1);
            var y = (align & (Alignment.CenterY | Alignment.FillY)) != 0 ? 0 : ((align & Alignment.MinY) != 0 ? -1 : 1);

            DrawDebugString(position, obj.ToString(), background, color, 2, x, y);
        }
        private void DrawDebugString(Vector2 position, object obj, Color color, int padding = 2, int xAlign = 0, int yAlign = 0)
        {
            DrawDebugString(position, obj.ToString(), color, padding, xAlign, yAlign);
        }
        private void DrawDebugString(Vector2 position, string text, Color? background, Color color, int padding = 2, int xAlign = 0, int yAlign = 0)
        {
            if (Renderer.Font == null) return;

            var p = position;
            var s = Renderer.Font.MeasureString(text, DebugFontScale);

            var bounds = new Rectangle(p.ToPoint(), s.ToPoint());
            bounds.Inflate(padding, padding);

            if (xAlign == 1)
            {
                p.X -= bounds.Width;
            }
            else if(xAlign == 0)
            {
                p.X -= (bounds.Width / 2f);
            }
            else if (xAlign == -1)
            {
                //p.X = bounds.Left;
            }

            if (yAlign == 1)
            {
                p.Y -= bounds.Height;
            }
            else if(yAlign == 0)
            {
                p.Y -= (bounds.Height / 2f);
            }
            else if (yAlign == -1)
            {
                //p.Y = bounds.Top;
            }

            p = Vector2.Clamp(p, Vector2.Zero, SpriteBatch.SpriteBatch.GraphicsDevice.Viewport.Bounds.Size.ToVector2() - bounds.Size.ToVector2());
            
            if (background.HasValue)
            {
                SpriteBatch.FillRectangle(new Rectangle(p.ToPoint(), bounds.Size), background.Value);
            }
            
            bounds.Inflate(-padding, -padding);
            SpriteBatch.DrawString(p, text, Renderer.Font, color, FontStyle.None, DebugFontScale);
        }

        #endregion

		public void Dispose()
		{

		}

		//public void HighlightGuiElement(Guid id)
		//{
		//	Log.Info($"IGuiDebuggerService.HighlightGuiElement(id: {id.ToString()})");
		//	var element = FindGuiElementById(id);
		//	HighlightedElement = element;

		//}

		//public void DisableHighlight()
		//{
		//	Log.Info("IGuiDebuggerService.DisableHighlight()");
		//	HighlightedElement = null;
		//}

		//public GuiElementInfo[] GetAllGuiElementInfos()
		//{
		//	Log.Info("IGuiDebuggerService.GetAllGuiElementInfos()");
		//	return GuiManager.Screens.Select(BuildGuiElementInfo).ToArray();
		//}

		//public GuiElementPropertyInfo[] GetElementPropertyInfos(Guid id)
		//{
		//	Log.Info($"IGuiDebuggerService.GetElementPropertyInfos(id: {id.ToString()})");
		//	var element = FindGuiElementById(id);
		//	if(element == null) return new GuiElementPropertyInfo[0];

		//	var infos = BuildGuiElementPropertyInfos(element);
		//	return infos;
		//}

		//public bool SetElementPropertyValue(Guid id, string propertyName, string propertyValue)
		//{
		//	Log.Info($"IGuiDebuggerService.SetElementPropertyValue(id: {id.ToString()}, propertyName: {propertyName}, propertyValue: {propertyValue})");
		//	var element = FindGuiElementById(id);
		//	if (element == null) return false;

		//	var property = element.GetType().GetProperty(propertyName);
		//	if (property == null) return false;

		//	try
		//	{
		//		var propType = property.PropertyType;
		//		var value    = ConvertPropertyType(propType, propertyValue);
		//		property.SetValue(element, value);
		//		return true;
		//	}
		//	catch
		//	{
		//		return false;
		//	}

		//}

		//private object ConvertPropertyType(Type targetType, string value)
		//{
		//	if (targetType.IsEnum)
		//	{
		//		return Enum.Parse(targetType, value, true);
		//	}

		//	if (targetType == typeof(Size))
		//	{
		//		return Size.Parse(value);
		//	}

		//	if (targetType == typeof(Thickness))
		//	{
		//		return Thickness.Parse(value);
		//	}

		//	if (targetType == typeof(int))
		//	{
		//		return int.Parse(value);
		//	}
			
		//	if (targetType == typeof(double))
		//	{
		//		return double.Parse(value);
		//	}
			
		//	if (targetType == typeof(float))
		//	{
		//		return float.Parse(value);
		//	}
			
		//	if (targetType == typeof(bool))
		//	{
		//		return bool.Parse(value);
		//	}

		//	return Convert.ChangeType(value, targetType);
		//}

		//private IGuiElement FindGuiElementById(Guid id)
		//{
		//	foreach (var screen in GuiManager.Screens.ToArray())
		//	{
		//		if (screen.TryFindDeepestChild(e => e.Id.Equals(id), out IGuiElement foundElement))
		//		{
		//			return foundElement;
		//		}
		//	}

		//	return null;
		//}

		//private GuiElementInfo BuildGuiElementInfo(IGuiElement guiElement)
		//{
		//	var info = new GuiElementInfo();
		//	info.Id = guiElement.Id;
		//	info.ElementType = guiElement.GetType().Name;

		//	info.ChildElements = guiElement.ChildElements.Select(BuildGuiElementInfo).ToArray();
		//	return info;
		//}

		//private GuiElementPropertyInfo[] BuildGuiElementPropertyInfos(IGuiElement guiElement)
		//{
		//	var properties = guiElement.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

		//	var infos = new List<GuiElementPropertyInfo>();
		//	foreach (var prop in properties)
		//	{
		//		var attr = prop.GetCustomAttribute<DebuggerVisibleAttribute>(true);
		//		if (attr == null) continue;
		//		if (!attr.Visible) continue;

		//		if(typeof(IGuiElement).IsAssignableFrom(prop.PropertyType)) continue;

		//		object val = null;
		//		try
		//		{
		//			val = prop.GetValue(guiElement);
					
		//		}
		//		catch(Exception ex)
		//		{
		//			val = "Exception - " + ex.Message;
		//		}

		//		infos.Add(new GuiElementPropertyInfo()
		//		{
		//			Name        = prop.Name,
		//			Type = prop.PropertyType,
		//			Value = val,
		//			StringValue = val?.ToString()
		//		});
		//	}

		//	return infos.ToArray();
		//}
	}
}