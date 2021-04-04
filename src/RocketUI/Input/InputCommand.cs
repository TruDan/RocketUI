using System;

namespace RocketUI.Input
{
    public struct InputCommand
    {
	    public static readonly string DefaultNamespace = "RocketUI";
    
        public static readonly InputCommand MoveForwards = new (DefaultNamespace, nameof(MoveForwards));
        public static readonly InputCommand MoveLeft = new (DefaultNamespace, nameof(MoveLeft));
        public static readonly InputCommand MoveBackwards = new (DefaultNamespace, nameof(MoveBackwards));
        public static readonly InputCommand MoveRight = new (DefaultNamespace, nameof(MoveRight));
        public static readonly InputCommand MoveUp = new (DefaultNamespace, nameof(MoveUp));
        public static readonly InputCommand MoveDown = new (DefaultNamespace, nameof(MoveDown));
        public static readonly InputCommand ToggleDebugInfo = new (DefaultNamespace, nameof(ToggleDebugInfo));
        public static readonly InputCommand LeftClick = new (DefaultNamespace, nameof(LeftClick));
        public static readonly InputCommand MiddleClick = new (DefaultNamespace, nameof(MiddleClick));
        public static readonly InputCommand RightClick = new (DefaultNamespace, nameof(RightClick));
        public static readonly InputCommand Exit = new (DefaultNamespace, nameof(Exit));
        public static readonly InputCommand A = new (DefaultNamespace, nameof(A));
        public static readonly InputCommand B = new (DefaultNamespace, nameof(B));
        public static readonly InputCommand X = new (DefaultNamespace, nameof(X));
        public static readonly InputCommand Y = new (DefaultNamespace, nameof(Y));
        public static readonly InputCommand Start = new (DefaultNamespace, nameof(Start));
		public static readonly InputCommand LookUp = new (DefaultNamespace, nameof(LookUp));
		public static readonly InputCommand LookDown = new (DefaultNamespace, nameof(LookDown));
		public static readonly InputCommand LookLeft = new (DefaultNamespace, nameof(LookLeft));
		public static readonly InputCommand LookRight = new (DefaultNamespace, nameof(LookRight));
		public static readonly InputCommand NavigateUp = new (DefaultNamespace, nameof(NavigateUp));
		public static readonly InputCommand NavigateDown = new (DefaultNamespace, nameof(NavigateDown));
		public static readonly InputCommand NavigateLeft = new (DefaultNamespace, nameof(NavigateLeft));
		public static readonly InputCommand NavigateRight = new (DefaultNamespace, nameof(NavigateRight));
		public static readonly InputCommand Navigate = new (DefaultNamespace, nameof(Navigate));
		public static readonly InputCommand NavigateBack = new (DefaultNamespace, nameof(NavigateBack));
		public static readonly InputCommand ScrollUp = new(DefaultNamespace, nameof(ScrollUp));
		public static readonly InputCommand ScrollDown = new(DefaultNamespace, nameof(ScrollDown));
		public static readonly InputCommand ScrollLeft = new(DefaultNamespace, nameof(ScrollLeft));
		public static readonly InputCommand ScrollRight = new(DefaultNamespace, nameof(ScrollRight));
		
		
		private readonly string _namespace;
		private readonly string _key;

		public bool HasValue => !string.IsNullOrEmpty(Key);

		public InputCommand(string @namespace, string key)
		{
			_namespace = @namespace;
			_key = key;
		}

		public static implicit operator string(InputCommand inputCommand)
		{
			return inputCommand.HasValue ? inputCommand.ToString() : null;
		}

		public override string ToString()
		{
			return $"{_namespace}:{_key}";
		}

		public static InputCommand Parse(string text)
		{
			var split = text.Split(':', 2);
			if (split.Length == 2)
			{
				return new InputCommand(split[0], split[1]);
			}

			throw new FormatException("Invalid GuiTextures format, expected 'namespace:key'");
		}

		public string Namespace => _namespace;

		public string Key => _key;
    }
}
