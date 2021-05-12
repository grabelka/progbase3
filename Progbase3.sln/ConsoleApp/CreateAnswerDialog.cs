using System;
using Terminal.Gui;

public class CreateAnswerDialog : Dialog
{
    public bool canceled;
    protected TextField inputId;
    protected TextView textView;
    protected CheckBox pinned;
    public CreateAnswerDialog()
    {
        this.Title = "Create answer";
        Button okBtn = new Button(2, 0, "OK");
        okBtn.Clicked += OnOkButtonClicked;
        this.Add(okBtn);
        Button cancelBtn = new Button(10, 0, "Cancel");
        cancelBtn.Clicked += OnCancelButtonClicked;
        this.Add(cancelBtn);
        Label iLabel = new Label(2, 2, "Question id: ");
        this.inputId = new TextField(20, 2, 40, "");
        this.Add(iLabel, inputId);
        Label vLabel = new Label(2, 4, "Text: ");
        this.textView = new TextView()
        {
            X = 20,
            Y = 4,
            Width = Dim.Fill(5),  // margin width
            Height = Dim.Percent(50),
            Text = "",
        };
        this.Add(vLabel, textView);
        this.pinned = new CheckBox(2, 8, "Is pinned: ");
        this.Add(pinned);
    }
    public Answer GetAnswer()
    {
        Int32.TryParse(inputId.Text.ToString(), out int res);
        string pin = (pinned.Checked) ? "yes" : "no";
        return new Answer(0, res, textView.Text.ToString(), DateTime.Now, pin, null);
    }
    private void OnCancelButtonClicked()
    {
        this.canceled = true;
        Application.RequestStop();
    }
    private void OnOkButtonClicked()
    {
        this.canceled = false;
        if(!Int32.TryParse(inputId.Text.ToString(), out int res)) this.canceled = true;
        Application.RequestStop();
    }
}