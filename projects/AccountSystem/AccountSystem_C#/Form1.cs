using Microsoft.Data.Sqlite;

namespace AccountSystem_Final {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        public static SqliteConnection? connection = null;

        public static string? currentUsernameInput = null;
        public static string? currentPasswordInput = null;

        private void Form1_Load(object sender, EventArgs e) {
            InitSql();
        }

        private void usernameInputField_TextChanged(object sender, EventArgs e) {
            currentUsernameInput = usernameInputField.Text;
        }

        private void passwordInputField_TextChanged(object sender, EventArgs e) {
            currentPasswordInput = passwordInputField.Text;
        }

        private void loginButton_Click(object sender, EventArgs e) {
            if (currentUsernameInput != null && currentPasswordInput != null) {
                LoginUser(currentUsernameInput, currentPasswordInput);
            }
        }

        private void createUserButton_Click(object sender, EventArgs e) {
            if (currentUsernameInput != null && currentPasswordInput != null) {
                CreateUser(currentUsernameInput, currentPasswordInput);
            }
        }

        private static void InitSql() {
            connection = new SqliteConnection(@"Data Source=Z:\Programming\VisualStudio Projects\CS\AccountSystem_Final\users.db");

            // Open the connection
            connection.Open();

            string commandString = "CREATE TABLE IF NOT EXISTS users(username TEXT UNIQUE, password TEXT)";

            using SqliteCommand command = new SqliteCommand(commandString, connection);

            command.ExecuteNonQuery();
        }

        private void CreateUser(string username, string password) {
            string salt = BCrypt.Net.BCrypt.GenerateSalt();
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, salt);

            string sqlCommandString = "INSERT OR IGNORE INTO users(username, password) VALUES ($username, $password)";

            using SqliteCommand command = new SqliteCommand(sqlCommandString, connection);

            command.Parameters.AddWithValue("$username", username);
            command.Parameters.AddWithValue("$password", hashedPassword);

            command.ExecuteNonQuery();

            resultText.Text = "Created User Succesfully!";
            resultText.ForeColor = Color.Green;
        }

        private bool LoginUser(string username, string password) {
            string sqlCommandString = "SELECT password FROM users WHERE username = $username";

            using SqliteCommand command = new SqliteCommand(sqlCommandString, connection);

            command.Parameters.AddWithValue("$username", username);

            var dataReader = command.ExecuteReader();

            while (dataReader.Read()) {
                if (BCrypt.Net.BCrypt.Verify(password, dataReader.GetString(0))) {
                    resultText.Text = "Login Succesful!";
                    resultText.ForeColor = Color.Green;

                    return true;
                }
            }

            resultText.Text = "Login Failed!";
            resultText.ForeColor = Color.Red;

            return false;
        }
    }
}
