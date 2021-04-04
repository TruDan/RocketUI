using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Xna.Framework;
using RocketUI.Input.Listeners;
using SharpVR;

namespace RocketUI.Input
{
    public class PlayerInputManagerEvent
    {
        public PlayerIndex        PlayerIndex        { get; }
        public PlayerInputManager PlayerInputManager { get; }

        protected PlayerInputManagerEvent(PlayerIndex index, PlayerInputManager inputManager)
        {
            PlayerIndex = index;
            PlayerInputManager = inputManager;
        }
    }

    public sealed class PlayerInputManagerAdded : PlayerInputManagerEvent
    {
        public PlayerInputManagerAdded(PlayerIndex index, PlayerInputManager inputManager) : base(index, inputManager)
        {
        }
    }

    public sealed class PlayerInputManagerRemoved : PlayerInputManagerEvent
    {
        public PlayerInputManagerRemoved(PlayerIndex index, PlayerInputManager inputManager) : base(index, inputManager)
        {
        }
    }

    public class InputManager : GameComponent
    {
        public event EventHandler<PlayerInputManagerAdded> InputManagerAdded;
        public event EventHandler<InputBindingEventArgs>   InputCommandTriggered;

        private readonly IServiceProvider _serviceProvider;

        private Dictionary<PlayerIndex, PlayerInputManager> PlayerInputManagers { get; } =
            new Dictionary<PlayerIndex, PlayerInputManager>();

        public int PlayerCount => PlayerInputManagers.Count;

        public List<InputActionBinding> Bindings { get; } = new List<InputActionBinding>();

        public InputManager(Game game, IServiceProvider serviceProvider) : base(game)
        {
            _serviceProvider = serviceProvider;
            UpdateOrder = -10;
            //var playerOne = GetOrAddPlayerManager(PlayerIndex.One);
        }

        public PlayerInputManager GetOrAddPlayerManager(PlayerIndex playerIndex)
        {
            if (!PlayerInputManagers.TryGetValue(playerIndex, out var playerInputManager))
            {
                playerInputManager = new PlayerInputManager(playerIndex);

                var listeners = _serviceProvider.GetService<IEnumerable<IInputListenerFactory>>();
                if (listeners != null)
                {
                    foreach (var listenerFactory in listeners)
                    {
                        var listener = listenerFactory.CreateInputListener(playerIndex);
                        if (listener != null)
                            playerInputManager.AddListener(listener);
                    }
                }

                PlayerInputManagers.Add(playerIndex, playerInputManager);

                InputManagerAdded?.Invoke(this, new PlayerInputManagerAdded(playerIndex, playerInputManager));
            }

            return playerInputManager;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            //if (!Game.IsActive)
            //    return;

            var playerInputManagers = PlayerInputManagers.Values.ToArray();
            foreach (var playerInputManager in playerInputManagers)
            {
                playerInputManager.Update(gameTime);
            }

            CheckTriggeredBindings(playerInputManagers);
        }

        private void CheckTriggeredBindings(PlayerInputManager[] playerInputManagers)
        {
            foreach (var binding in Bindings)
            {
                foreach (var playerInputManager in playerInputManagers)
                {
                    if ((binding.Trigger == InputBindingTrigger.Continuous &&
                         playerInputManager.IsDown(binding.InputCommand))
                        || (binding.Trigger == InputBindingTrigger.Discrete &&
                            playerInputManager.IsBeginPress(binding.InputCommand)))
                    {
                        if (binding.Predicate())
                        {
                            // triggered
                            HandleBindingTriggered(playerInputManager, binding);
                        }
                    }
                }
            }
        }

        private void HandleBindingTriggered(PlayerInputManager playerInputManager, InputActionBinding binding)
        {
            binding.Action?.Invoke();
            InputCommandTriggered?.Invoke(this, new InputBindingEventArgs(playerInputManager, binding));
        }


        public InputActionBinding RegisterListener(InputCommand command, InputBindingTrigger trigger,
            InputActionPredicate                                predicate,
            Action                                              action)
        {
            var binding = new InputActionBinding(command, trigger, predicate, action);
            Bindings.Add(binding);
            return binding;
        }

        public void UnregisterListener(InputActionBinding binding)
        {
            Bindings.Remove(binding);
        }

        public bool Any(Func<PlayerInputManager, bool> playerInputManagerFunc)
        {
            return PlayerInputManagers.Values.ToArray().Any(playerInputManagerFunc);
        }
    }
}