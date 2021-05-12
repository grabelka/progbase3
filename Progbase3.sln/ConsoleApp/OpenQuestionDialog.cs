using System;
using Terminal.Gui;

public class OpenQuestionDialog : Dialog
{
    public bool deleted;
    public bool updated;
    protected Question question;
    Label inputId;
    Label inputTitle;
    TextView textView;
    Label created;
    public OpenQuestionDialog()
    {
        this.Title = "Open question";
        Button backBtn = new Button(2, 0, "Back");
        backBtn.Clicked += OnBackButtonClicked;
        this.Add(backBtn);
        Label uLabel = new Label(2, 2, "User id: ");
        this.inputId = new Label(20, 2, "");
        this.Add(uLabel, inputId);
        Label tLabel = new Label(2, 4, "Title: ");
        this.inputTitle = new Label(20, 4, "");
        this.Add(tLabel, inputTitle);
        Label vLabel = new Label(2, 6, "Text: ");
        this.textView = new TextView()
        {
            X = 20,
            Y = 6,
            Width = Dim.Fill(5),  // margin width
            Height = Dim.Percent(50),
            Text = "",
            ReadOnly = true
        };
        this.Add(vLabel, textView);
        Label cLabel = new Label(2, 10, "Created : ");
        this.created = new Label(20, 10, "");
        this.Add(cLabel, created);
        Button edit = new Button(30, 14, "Edit");
        edit.Clicked += OnEdit;
        this.Add(edit);
        Button delete = new Button(40, 14, "Delete");
        delete.Clicked += OnDelete;
        this.Add(delete);
    }
    void OnEdit()
    {
        EditQuestionDialog dialog = new EditQuestionDialog();
        dialog.SetQuestion(this.question);
        Application.Run(dialog);
        if (!dialog.canceled)
        {
            Question updatedQuestion = dialog.GetQuestion();  
            this.updated = true;
            this.SetQuestion(updatedQuestion);
        }
    }
    void OnDelete()
    {
        int index = MessageBox.Query("Delete question", "Are you sure?", "No", "Yes");
        if (index == 1)
        {
            this.deleted = true;
            Application.RequestStop();
        }
    }
    public void SetQuestion(Question question)
    {
        this.question = question;
        this.inputId.Text = question.userId.ToString();
        this.inputTitle.Text = question.title.ToString();
        this.created.Text = question.created.ToShortDateString();
        this.textView.Text = question.text.ToString();
    }
    public Question GetQuestion()
    {
        return this.question;
    }
    private void OnBackButtonClicked()
    {
        Application.RequestStop();
    }
}