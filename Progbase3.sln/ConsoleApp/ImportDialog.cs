using System;
using Terminal.Gui;

public class ImportDialog : Dialog
{
    public bool cancaled;
    public string path = "";
    public ImportDialog()
    {
        this.Title = "Import";
        Button btn = new Button(2, 0, "Choose directory");
        btn.Clicked += OnButtonClicked;
        Button cnc = new Button(20, 10, "Cancel");
        cnc.Clicked += OnCancelButtonClicked;
        Button ok = new Button(10, 10, "Ok");
        ok.Clicked += OnOklButtonClicked;
        this.Add(btn, cnc, ok);
    }
    private void OnButtonClicked()
    {
        OpenDialog dialog = new OpenDialog("Open XML", "Open");
        dialog.CanChooseDirectories = true;
        dialog.CanChooseFiles = false;
        Application.Run(dialog);
        if(!dialog.Canceled) path = Convert.ToString(dialog.FilePath);
    }
    private void OnOklButtonClicked()
    {
        Application.RequestStop();
    }
    private void OnCancelButtonClicked()
    {
        Application.RequestStop();
        cancaled = true;
    }
}