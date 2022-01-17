using System;
using System.Collections.Concurrent;
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

        private ConcurrentDictionary<PlayerIndex, PlayerInputManager> PlayerInputManagers { get; } =
            new ConcurrentDictionary<PlayerIndex, PlayerInputManager>();

        public int PlayerCount => PlayerInputManagers.Count;

        private object _bindingsLock = new object();
        private List<InputActionBinding> Bindings { get; } = new List<InputActionBinding>();

        public InputManager(Game game, IServiceProvider serviceProvider) : base(game)
        {
            _serviceProvider = serviceProvider;
            UpdateOrder = -10;
            //var playerOne = GetOrAddPlayerManager(PlayerIndex.One);
        }

        public bool TryRemovePlayerManager(PlayerIndex playerIndex)
        {
            if (PlayerInputManagers.TryRemove(playerIndex, out var manager))
            {
                manager?.Dispose();
                return true;
            }

            return false;
        }
        
        public PlayerInputManager GetOrAddPlayerManager(PlayerIndex playerIndex)
        {
            return PlayerInputManagers.GetOrAdd(playerIndex, index =>
            {
                var playerInputManager = new PlayerInputManager(index);

                var listeners = _serviceProvider.GetService<IEnumerable<IInputListenerFactory>>();
                if (listeners != null)
                {
                    foreach (var listenerFactory in listeners)
                    {
                        var listener = listenerFactory.CreateInputListener(index);
                        if (listener != null)
                            playerInputManager.AddListener(listener);
                    }
                }

                InputManagerAdded?.Invoke(this, new PlayerInputManagerAdded(index, playerInputManager));
                return playerInputManager;
            });
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

            CheckGlobalBindings(playerInputManagers);
        }

        private void CheckGlobalBindings(PlayerInputManager[] playerInputManagers)
        {
            InputActionBinding[] bindings;

            lock (_bindingsLock)
            {
                bindings = Bindings.ToArray();
            }

            foreach (var playerInputManager in playerInputManagers)
            {
                foreach (var binding in bindings)
                {
                    if (playerInputManager.CheckBinding(binding))
                    {
                        binding.Action?.Invoke();
                        InputCommandTriggered?.Invoke(this, new InputBindingEventArgs(playerInputManager, binding));
                    }
                }
            }
        }
        
        public InputActionBinding RegisterListener(InputCommand command, InputBindingTrigger trigger,
            InputActionPredicate                                predicate,
            Action                                              action)
        {
            var binding = new InputActionBinding(command, trigger, predicate, action);

            lock (_bindingsLock)
            {
                Bindings.Add(binding);
            }

            return binding;
        }

        public void UnregisterListener(InputActionBinding binding)
        {
            lock (_bindingsLock)
            {
                Bindings.Remove(binding);
            }
        }

        public bool Any(Func<PlayerInputManager, bool> playerInputManagerFunc)
        {
            return PlayerInputManagers.Values.ToArray().Any(playerInputManagerFunc);
        }
    }
}