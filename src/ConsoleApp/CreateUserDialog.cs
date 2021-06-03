using System;
using ClassLibrary;
using Terminal.Gui;

public class CreateUserDialog : Dialog
{
    public bool canceled;
    protected TextField inputLogin;
    protected TextField inputName;
    protected TextField inputPass;
    protected CheckBox isModerator;
    public CreateUserDialog()
    {
        this.Title = "Create user";
        Button okBtn = new Button(2, 0, "OK");
        okBtn.Clicked += OnOkButtonClicked;
        this.Add(okBtn);
        Button cancelBtn = new Button(10, 0, "Cancel");
        cancelBtn.Clicked += OnCancelButtonClicked;
        this.Add(cancelBtn);
        Label lLabel = new Label(2, 2, "Login: ");
        this.inputLogin = new TextField(20, 2, 40, "");
        this.Add(lLabel, inputLogin);
        Label nLabel = new Label(2, 4, "Name: ");
        this.inputName = new TextField(20, 4, 40, "");
        this.Add(nLabel, inputName);
        Label pLabel = new Label(2, 6, "Password: ");
        this.inputPass = new TextField(20, 6, 40, "");
        this.Add(pLabel, inputPass);
        this.isModerator = new CheckBox(2, 8, "Is moderator: ");
        this.Add(isModerator);
    }
    public User GetUser()
    {
        string mod = (isModerator.Checked) ? "yes" : "no";
        return new User(0, inputName.Text.ToString(), inputLogin.Text.ToString(), mod, inputPass.Text.ToString(), null);
    }
    private void OnCancelButtonClicked()
    {
        this.canceled = true;
        Application.RequestStop();
    }
    private void OnOkButtonClicked()
    {
        this.canceled = false;
        Application.RequestStop();
    }
}