namespace RocketUI
{
    public interface IElementRenderer<T> where T : IGuiElement
    {

        void Draw(T element);

    }
}