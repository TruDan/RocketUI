namespace RocketUI
{
    public interface IElementRenderer<in T> where T : IGuiElement
    {

        void Draw(T element);

    }
}