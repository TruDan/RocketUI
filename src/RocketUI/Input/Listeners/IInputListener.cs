
namespace RocketUI.Input.Listeners
{
    public interface IInputListener
    {
        int PlayerIndex { get; }

        int Order { get; set; }
        
        void Update();

        bool IsDown(InputCommand       command);
        bool IsUp(InputCommand         command);
        bool IsBeginPress(InputCommand command);
        bool IsPressed(InputCommand    command);
    }

    public interface IInputListenerFactory
    {
        IInputListener CreateInputListener(int playerIndex);
    }

    public class DefaultInputListenerFactory<TListener> : IInputListenerFactory
        where TListener : class, IInputListener
    {
        private readonly InputListenerFactory<TListener> _inputListenerFactory;

        public DefaultInputListenerFactory(InputListenerFactory<TListener> inputListenerFactory)
        {
            _inputListenerFactory = inputListenerFactory;
        }

        public IInputListener CreateInputListener(int playerIndex)
        {
            return _inputListenerFactory.Invoke(playerIndex);
        }
    }

    public delegate IInputListener InputListenerFactory(int playerIndex);
    public delegate T InputListenerFactory<T>(int playerIndex) where T : class, IInputListener;
}