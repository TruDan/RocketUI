﻿using System;
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
        private readonly IServiceProvider _serviceProvider;

        private Dictionary<PlayerIndex, PlayerInputManager> PlayerInputManagers { get; } =
            new Dictionary<PlayerIndex, PlayerInputManager>();

        public int PlayerCount => PlayerInputManagers.Count;

        public EventHandler<PlayerInputManagerAdded> InputManagerAdded;

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

            foreach (var playerInputManager in PlayerInputManagers.Values.ToArray())
            {
                playerInputManager.Update(gameTime);
            }
        }

        public bool Any(Func<PlayerInputManager, bool> playerInputManagerFunc)
        {
            return PlayerInputManagers.Values.ToArray().Any(playerInputManagerFunc);
        }
    }
}