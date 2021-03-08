using Microsoft.Xna.Framework;

namespace RocketUI.Input.Listeners
{
    public interface IInputListener
    {
        PlayerIndex PlayerIndex { get; }

        int Order { get; set; }
        
        void Update(GameTime gameTime);

        bool IsDown(InputCommand       command);
        bool IsUp(InputCommand         command);
        bool IsBeginPress(InputCommand command);
        bool IsPressed(InputCommand    command);
    }

    public interface IInputListenerFactory
    {
        IInputListener CreateInputListener(PlayerIndex playerIndex);
    }

    public class DefaultInputListenerFactory<TListener> : IInputListenerFactory
        where TListener : class, IInputListener
    {
        private readonly InputListenerFactory<TListener> _inputListenerFactory;

        public DefaultInputListenerFactory(InputListenerFactory<TListener> inputListenerFactory)
        {
            _inputListenerFactory = inputListenerFactory;
        }

        public IInputListener CreateInputListener(PlayerIndex playerIndex)
        {
            return _inputListenerFactory.Invoke(playerIndex);
        }
    }

    public delegate IInputListener InputListenerFactory(PlayerIndex playerIndex);
    public delegate T InputListenerFactory<T>(PlayerIndex playerIndex) where T : class, IInputListener;
}