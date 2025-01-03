
namespace WoWFrameTools;

public class MainMenu
{
    private readonly UI _ui;
    
    public MainMenu(UI ui)
    {
        _ui = ui;
    }

    public void Load()
    {
        _ui.AddMenu(new Menu(
            [
                new SubMenuItem("File", 
                [
                    new Item("New Project", null),
                    new Item("Open Project", null),
                    new Item("Save", null)
                ]),
                new SubMenuItem("Edit", 
                [
                    new Item("Undo", null),
                    new Item("Redo", null),
                    new Item("Cut", null),
                    new Item("Copy", null),
                    new Item("Paste", null)
                ]),
                new SubMenuItem("View", 
                [
                    new Item("Toolbox", null),
                    new Item("World Manager", null),
                    new Item("Data Manager", ToggleDataManager)
                ]),
            ]));
    }
    
    private void ToggleDataManager()
    {

    }
}