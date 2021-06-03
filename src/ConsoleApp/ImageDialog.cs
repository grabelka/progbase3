using System;
using ClassLibrary;
using Terminal.Gui;

public class ImageDialog : Dialog
{
    public bool canceled;
    public string start;
    public string end;
    DateField startDate;
    DateField endDate;
    public ImageDialog()
    {
        this.Title = "Get stats";
        Button okBtn = new Button(2, 0, "OK");
        okBtn.Clicked += OnOkButtonClicked;
        Button cancelBtn = new Button(10, 0, "Cancel");
        cancelBtn.Clicked += OnCancelButtonClicked;
        this.Add(cancelBtn, okBtn);
        startDate = new DateField(2, 2, System.DateTime.Now);
        endDate = new DateField(14, 2, System.DateTime.Now);
        this.Add(startDate, endDate);
    }
    private void OnCancelButtonClicked()
    {
        this.canceled = true;
        Application.RequestStop();
    }
    private void OnOkButtonClicked()
    {
        string[] buf = Convert.ToString(startDate.Text).Split('/');
        start = buf[2] + "-" + buf[0] + "-" + buf[1];
        buf = Convert.ToString(endDate.Text).Split('/');
        end = buf[2] + "-" + buf[0] + "-" + buf[1];
        this.canceled = false;
        Application.RequestStop();
    }
}