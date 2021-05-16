using ClassLibrary;
using Terminal.Gui;

public class RegistrationDialog : Dialog
{
    public bool canceled;
    private TextField inputLogin;
    private TextField inputName;
    private TextField inputPass;
    private CheckBox isModerator;
    public RegistrationDialog()
    {
        this.Title = "Registration";
        Button backBtn = new Button(10, 12, "Back");
        backBtn.Clicked += OnBackButtonClicked;
        Button regBtn = new Button(20, 12, "Register");
        regBtn.Clicked += OnRegisterButtonClicked;
        this.Add(backBtn, regBtn);
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
    private void OnBackButtonClicked()
    {
        this.canceled = true;
        Application.RequestStop();
    }
    private void OnRegisterButtonClicked()
    {
        Application.RequestStop();
    }
}