using System;
using Terminal.Gui;

public class ExportDialog : Dialog
{
    public bool cancaled;
    public string path = "";
    public string start;
    public string end;
    DateField startDate;
    DateField endDate;
    public ExportDialog()
    {
        this.Title = "Import";
        startDate = new DateField(2, 2, System.DateTime.Now);
        endDate = new DateField(14, 2, System.DateTime.Now);
        Button btn = new Button(2, 0, "Choose directory");
        btn.Clicked += OnButtonClicked;
        Button cnc = new Button(20, 10, "Cancel");
        cnc.Clicked += OnCancelButtonClicked;
        Button ok = new Button(10, 10, "Ok");
        ok.Clicked += OnOklButtonClicked;
        this.Add(btn, cnc, ok, startDate, endDate);
    }
    private void OnButtonClicked()
    {
        OpenDialog dialog = new OpenDialog("Open XML", "Open");
        dialog.CanChooseDirectories = true;
        dialog.CanChooseFiles = false;
        Application.Run(dialog);
        if(!dialog.Canceled) path = Convert.ToString(dialog.FilePath);
        string[] buf = Convert.ToString(startDate.Text).Split('/');
        start = buf[2] + "-" + buf[0] + "-" + buf[1];
        buf = Convert.ToString(endDate.Text).Split('/');
        end = buf[2] + "-" + buf[0] + "-" + buf[1];
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