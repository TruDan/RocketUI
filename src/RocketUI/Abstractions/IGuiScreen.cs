namespace RocketUI
{
    public interface IGuiScreen : IGuiElement, IGuiFocusContext
    {
        GuiManager GuiManager            { get; }
        bool       IsAutomaticallyScaled { get; }
        void UpdateLayout();

    }
}
