using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Microsoft.Xna.Framework;
using Portable.Xaml.Markup;
using RocketUI.Attributes;
using RocketUI.Serialization;

namespace RocketUI
{
	public delegate bool GuiElementPredicate(IGuiElement element);

	public delegate bool GuiElementPredicate<in TGuiElement>(TGuiElement element)
		where TGuiElement : class, IGuiElement;

	
	[RuntimeNameProperty(nameof(Id))]
	[ContentProperty(nameof(Children))]
	public partial class RocketElement : IGuiElement, IDisposable
	{
		private PropertyStore _properties;

		/// <summary>
		/// Gets the dictionary of properties for this widget
		/// </summary>
		public PropertyStore Properties
		{
			get
			{
				if (_properties == null)
				{
					_properties = new PropertyStore(this);
				}

				return _properties;
			}
		}

		[DebuggerVisible]
		public Guid Id { get; } = Guid.NewGuid();
        
		public string Name { get; set; }
		public object Tag  { get; set; }
		
		private IGuiScreen       _screen;
		private IGuiElement      _parentElement;
		private IGuiFocusContext _focusContext;

		//[DebuggerVisible(Visible = false)]
		//public IGuiScreen Screen
		//{
		//	get => _screen;
		//	private set
		//	{
		//		var currentScreen = _screen;
		//		_screen = value;
		//		OnScreenChanged(currentScreen, _screen);

		//		InvalidateLayout();
		//	}
		//}

		[DebuggerVisible(Visible = false)]
		public virtual IGuiScreen RootScreen
		{
			get => ParentElement?.RootScreen;
		}

		[DebuggerVisible(Visible = false)]
		public IGuiElement ParentElement
		{
			get => _parentElement;
			set
			{
				var previousParent = _parentElement;
				_parentElement = value;
				//TryFindParentOfType<IGuiScreen>(e => true, out IGuiScreen screen);
				//Screen = screen;

				OnParentElementChanged(previousParent, _parentElement);

				InvalidateLayout();
			}
		}


		[DebuggerVisible(Visible = false)]
		public virtual IGuiFocusContext FocusContext
		{
			get { return _focusContext ?? ParentElement?.FocusContext ?? RootScreen; }
			set { _focusContext = value; }
		}


		[DebuggerVisible(Visible = false)]
		public IGuiElement[] ChildElements
		{
			get
			{
				lock (_childrenLock)
				{
					return Children.ToArray();
				}
			}
		}

		private object _childrenLock = new object();

		private ObservableCollection<IGuiElement> _children;
		
		[DebuggerVisible(Visible = false)] 
		public ObservableCollection<IGuiElement> Children
		{
			get
			{
				if (_children == null)
				{
					_children = new ObservableCollection<IGuiElement>();
					_children.CollectionChanged += ChildrenOnCollectionChanged;
				}
				return _children;
			}
		}

		[DebuggerVisible(Visible = false)]
		public bool HasChildren => Children.Count > 0;

		public int ChildCount => Children.Count;

		[DebuggerVisible(Visible = false)]
		internal IReadOnlyList<IGuiElement> AllChildren =>
			ChildElements.OfType<RocketElement>().SelectMany(c => new[] {c}.Union(c.AllChildren)).ToList();

		#region Drawing

		[DebuggerVisible]
		public virtual Vector2 RenderPosition { get; set; }

		[DebuggerVisible]
		public virtual Size RenderSize { get; set; }

		[DebuggerVisible]
		public virtual Rectangle RenderBounds { get; set; }

		[DebuggerVisible]
		public bool IsVisible { get; set; } = true;

		public Matrix LayoutTransform { get; set; } = Matrix.Identity;
		public Matrix RenderTransform { get; set; } = Matrix.Identity;

		public void Draw(GuiSpriteBatch graphics, GameTime gameTime)
		{
			if (!IsVisible) return;
			if (RenderBounds.Size == Point.Zero) return;

			IDisposable clipDispose = null;

			if (ClipToBounds)
				clipDispose = graphics.BeginClipBounds(RenderBounds, true);

			using (clipDispose)
			{
				if (_initialised)
				{
					OnDraw(graphics, gameTime);
				}

				ForEachChild(c => c?.Draw(graphics, gameTime));
			}
		}

		#endregion

		private IGuiRenderer _guiRenderer;
		private bool         _initialised;

		protected IGuiRenderer GuiRenderer => _guiRenderer;

		public RocketElement()
		{
		}
		
		public T FindControl<T>(string name) 
			where T : class, IGuiElement
		{
			TryFindDeepestChildOfType<T>(element => !string.IsNullOrWhiteSpace(element.Name) && string.Equals(name, element.Name), out var e);
			return e;
		}

		#region Methods

		public void Init(IGuiRenderer renderer, bool force = false)
		{
			if (!_initialised || force)
			{
				_guiRenderer = renderer;
				OnInit(renderer);
			}

			ForEachChild(c => c.Init(renderer, force));

			_initialised = true;
		}

		protected virtual void OnInit(IGuiRenderer renderer)
		{
			Background.TryResolveTexture(renderer);
			BackgroundOverlay.TryResolveTexture(renderer);
		}

		public void Update(GameTime gameTime)
		{
			if (_initialised)
			{
				OnUpdate(gameTime);
			}

			ForEachChild(c => c.Update(gameTime));
		}

		protected virtual void OnUpdate(GameTime gameTime)
		{
		}


		public void AddChild(IGuiElement element)
		{
			if (element == this) return;
			if (element.ParentElement == this) return;
			
			lock (_childrenLock)
			{
				if (Children.Contains(element)) return;

				Children.Add(element);
			}

			element.ParentElement = this;

			if (_initialised)
			{
				element.Init(_guiRenderer);
			}

			OnChildAdded(element);

			ChildAdded?.Invoke(this, new GuiElementChildEventArgs(this, element));
			
			InvalidateLayout();
		}

		public void RemoveChild(IGuiElement element)
		{
			if (element == this) return;

			OnChildRemoved(element);

			lock (_childrenLock)
			{
				Children.Remove(element);
			}

			if(element.ParentElement == this)
				element.ParentElement = null;
			
			ChildRemoved?.Invoke(this, new GuiElementChildEventArgs(this, element));
			
			InvalidateLayout();
		}
		
		
		private void ChildrenOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.NewItems != null)
			{
				foreach (IGuiElement item in e.NewItems)
				{
					item.ParentElement = this;
					if(_initialised)
						item.Init(_guiRenderer, true);

					OnChildAdded(item);

					InvalidateLayout();
				}
			}

			if (e.OldItems != null)
			{
				foreach (IGuiElement item in e.OldItems)
				{
					OnChildRemoved(item);

					if (item.ParentElement == this)
						item.ParentElement = null;
					
					InvalidateLayout();
				}
			}

		}

		public void ClearChildren()
		{
			var children = ChildElements;

			foreach (var child in children)
			{
				RemoveChild(child);
			}
		}

		#endregion


		#region Hierachy Transcending

		public bool TryTranscendChildren(GuiElementPredicate predicate, bool recurse = true)
		{
			if (!HasChildren) return false;

			var children = ChildElements;

			// First scan the children at this level
			foreach (var child in children)
			{
				if (predicate(child))
				{
					return true;
				}
			}

			if (!recurse) return false;

			// If the children on this level do not match, check their children.
			foreach (var child in children)
			{
				if (child.TryTranscendChildren(predicate, true))
				{
					return true;
				}
			}

			return false;
		}

		public bool TryFindParent(GuiElementPredicate predicate, out IGuiElement parentElement)
		{
			if (ParentElement == null)
			{
				parentElement = null;
				return false;
			}

			if (predicate(ParentElement))
			{
				parentElement = ParentElement;
				return true;
			}

			return ParentElement.TryFindParent(predicate, out parentElement);
		}

		public bool TryFindParentOfType<TGuiElement>(GuiElementPredicate<TGuiElement> predicate,
													 out TGuiElement                  parentElement)
			where TGuiElement : class, IGuiElement
		{
			var result = TryFindParent(e => e is TGuiElement e1 && predicate(e1), out IGuiElement element);

			parentElement = element as TGuiElement;
			return result;
		}

		public bool TryFindDeepestChild(GuiElementPredicate predicate, out IGuiElement childElement)
		{
			childElement = null;
			if (!HasChildren) return false;

			var children = ChildElements;

			foreach (var child in children)
			{
				if (predicate(child))
				{
					childElement = child;

					if (child.TryFindDeepestChild(predicate, out var recurseChild))
					{
						childElement = recurseChild;
						return true;
					}

					return true;
				}
			}

			// If the children on this level do not match, check their children.
			foreach (var child in children)
			{
				if (child.TryFindDeepestChild(predicate, out var recurseChild))
				{
					childElement = recurseChild;
					return true;
				}
			}

			return false;
		}

		public bool TryFindDeepestChildOfType<TGuiElement>(GuiElementPredicate<TGuiElement> predicate,
														   out TGuiElement                  childElement)
			where TGuiElement : class, IGuiElement
		{
			var result = TryFindDeepestChild(e => e is TGuiElement e1 && predicate(e1), out IGuiElement element);

			childElement = element as TGuiElement;
			return result;
		}

		public IEnumerable<TResult> ForEachChild<TResult>(Func<IGuiElement, TResult> valueSelector)
		{
			if (HasChildren)
			{
				foreach (var child in ChildElements)
				{
					yield return valueSelector(child);
				}
			}
		}

		public void ForEachChild(Action<IGuiElement> childAction)
		{
			if (!HasChildren) return;

			foreach (var child in ChildElements)
			{
				childAction(child);
			}
		}

		private void ForEachChild<TElement>(Action<TElement> childAction) where TElement : class, IGuiElement
		{
			ForEachChild(c =>
			{
				if (c is TElement e) childAction(e);
			});
		}

		#endregion

		#region Event Handlers

		public EventHandler<GuiElementChildEventArgs> ChildAdded;
		public EventHandler<GuiElementChildEventArgs> ChildRemoved;
		
		protected virtual void OnChildAdded(IGuiElement element)
		{
		}

		protected virtual void OnChildRemoved(IGuiElement element)
		{
		}

		protected virtual void OnScreenChanged(IGuiScreen previousScreen, IGuiScreen newScreen)
		{
		}

		protected virtual void OnParentElementChanged(IGuiElement previousParent, IGuiElement newParent)
		{
			ForEachChild(e => e.ParentElement = this);
		}

		protected virtual void OnUpdateLayout()
		{
		}

		#endregion

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~RocketElement()
		{
			Dispose(false);
		}
	}

	public class GuiElementEventArgs
	{
		public IGuiElement Element { get; }

		internal GuiElementEventArgs(IGuiElement element)
		{
			Element = element;
		}
	}
	public class GuiElementChildEventArgs
	{
		public IGuiElement Parent { get; }
		public IGuiElement Child { get; }

		public GuiElementChildEventArgs(IGuiElement parent, IGuiElement child)
		{
			Parent = parent;
			Child = child;
		}
	}
}