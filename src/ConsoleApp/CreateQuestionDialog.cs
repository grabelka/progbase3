using System;
using ClassLibrary;
using Terminal.Gui;

public class CreateQuestionDialog : Dialog
{
    public bool canceled;
    protected TextField inputUser;
    protected TextField inputTitle;
    protected TextView textView;
    public CreateQuestionDialog()
    {
        this.Title = "Create question";
        Button okBtn = new Button(2, 0, "OK");
        okBtn.Clicked += OnOkButtonClicked;
        this.Add(okBtn);
        Button cancelBtn = new Button(10, 0, "Cancel");
        cancelBtn.Clicked += OnCancelButtonClicked;
        this.Add(cancelBtn);
        Label uLabel = new Label(2, 2, "User id: ");
        this.inputUser = new TextField(20, 2, 40, "");
        this.Add(uLabel, inputUser);
        Label tLabel = new Label(2, 4, "Title: ");
        this.inputTitle = new TextField(20, 4, 40, "");
        this.Add(tLabel, inputTitle);
        Label vLabel = new Label(2, 6, "Text: ");
        this.textView = new TextView()
        {
            X = 20,
            Y = 6,
            Width = Dim.Fill(5),  // margin width
            Height = Dim.Percent(50),
            Text = "",
        };
        this.Add(vLabel, textView);
    }
    public Question GetQuestion()
    {
        Int32.TryParse(inputUser.Text.ToString(), out int res);
        return new Question(0, res, inputTitle.Text.ToString(), textView.Text.ToString(), DateTime.Now, null, null);
    }
    private void OnCancelButtonClicked()
    {
        this.canceled = true;
        Application.RequestStop();
    }
    private void OnOkButtonClicked()
    {
        this.canceled = false;
        if(!Int32.TryParse(inputUser.Text.ToString(), out int res)) this.canceled = true;
        Application.RequestStop();
    }
}