namespace RocketUI
{
    public interface IGuiScreen : IGuiElement, IGuiFocusContext
    {
        GuiManager GuiManager { get; }
        void UpdateLayout();

    }
}
