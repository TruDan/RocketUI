using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using RocketUI.Input.Listeners;

namespace RocketUI.Input
{
    public class PlayerInputManagerEvent
    {
        public PlayerIndex PlayerIndex { get; }
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
        private Dictionary<PlayerIndex, PlayerInputManager> PlayerInputManagers { get; } = new Dictionary<PlayerIndex, PlayerInputManager>();

        public int PlayerCount => PlayerInputManagers.Count;

        public EventHandler<PlayerInputManagerAdded> InputManagerAdded;

        public InputManager(Game game) : base(game)
        {
            UpdateOrder = -10;
            var playerOne     = GetOrAddPlayerManager(PlayerIndex.One);
            var mouseListener = new MouseInputListener(PlayerIndex.One);
            var vrListener    = new VRControllerInputListener(PlayerIndex.One);

            playerOne.AddListener(mouseListener);
            playerOne.AddListener(vrListener);
            playerOne.AddListener(new KeyboardInputListener());
        }

        public PlayerInputManager GetOrAddPlayerManager(PlayerIndex playerIndex)
        {
            if (!PlayerInputManagers.TryGetValue(playerIndex, out var playerInputManager))
            {
                playerInputManager = new PlayerInputManager(playerIndex);
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
