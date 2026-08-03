using Todos;

namespace MainProgram {
    public class MainProgram {

        private static UserInformation userInformation;

        private static bool shouldRunProgram = true;

        public static void Main(string[] args) {
            // Register a new user since this is the first time starting the program
            RegisterUser();

            // Before beginning the loop, init some test todos first for easier testing:
            //TodoHandler.InitTestTodos();

            while (shouldRunProgram) { // Main loop
                ProgramOptions userProgramOptionChoice = GetUserProgramOptionsChoice();

                HandleUserProgramOptionChoice(userProgramOptionChoice);
            }
        }

        private static void RegisterUser() {
            PrintLog("Welcome!\n", ConsoleColor.Blue);

            // Ask the user for his information
            userInformation = GetNewUserInformation();

            PrintLog("Got user information. Information:", ConsoleColor.Blue);
            PrintUserInformation(userInformation);

            while (true) {

                PrintLog("Please confirm if this is correct by typing either 'correct' or 'incorrect':", ConsoleColor.Blue);

                string userInput = Console.ReadLine();

                if (userInput == "correct") {
                    PrintLog("Thanks for clarrifying, you can now use this program!", ConsoleColor.Green);

                    break;
                } else if (userInput == "incorrect") {
                    PrintLog(
                        "Please specify which input(s) is/are incorrect by typing the according number of one of the following options: " +
                        "\n1: name,\n2: email,\n3: age,\n4: multiple\n\n" +
                        "If more than 1 is incorrect, re-input all the asked information once more: ",
                        ConsoleColor.Blue
                    );

                    while (true) {
                        GetUserInput(out int userRespondsIndex, "Number");

                        if (userRespondsIndex > 0 && userRespondsIndex <= Enum.GetValues(typeof(UserChangeInformationType)).Length) {

                            UserChangeInformationType informationType = (UserChangeInformationType)userRespondsIndex;
                            GetUserChangeInformationInput(ref userInformation, informationType);

                            break;
                        } else {
                            PrintLog("This isn't a valid input, please try again!", ConsoleColor.DarkYellow);
                        }
                    }

                    PrintLog("Updated user information. Information:", ConsoleColor.Green);
                    PrintUserInformation(userInformation);
                } else {
                    while (true) {
                        PrintLog("This is not a valid answer, please try again: ", ConsoleColor.DarkYellow);
                    }
                }
            }

            PrintLog("\nYou can start creating TODOs and this program will store them all in a nice, organized file", ConsoleColor.Blue);
        }

        public static UserInformation GetNewUserInformation() {
            string? userName = "";
            string? userEmail = "";
            int userAge = -1;

            PrintLog("Please answer the following questions to get started!", ConsoleColor.Magenta);

            // Add some extra logic for making sure the name doesn't include any indexes
            while (true) {
                // Get the user's name
                GetUserInput(out userName, "Name: ", "User name invalid. Please try again!");
                bool containsInt = userName.Any(char.IsDigit); // Copy pasted from someone, did not know this was a thing

                if (containsInt) {
                    PrintLog("This is not a valid name since it includes a number. Please make sure your name only includes letters!", ConsoleColor.DarkYellow);
                } else {
                    break;
                }
            }

            // Add some extra logic for fetching the user email as we need to confirm it has the correct suffix (which is @gmail.com)
            while (true) {
                // Get the user's email
                GetUserInput(out userEmail, "Email: ", "User email invalid. Please try again!");

                string[] userEmailSplit = userEmail.Split('@');
                if (!userEmail.EndsWith("@gmail.com") || string.IsNullOrEmpty(userEmailSplit[0]) || userEmailSplit.Length < 2) {
                    PrintLog("This email isn't valid. Please make sure that the email starts with something and ends with '@gmail.com' as gmail is the only accepted email at this time!", ConsoleColor.DarkYellow);
                } else {
                    break;
                }
            }

            // Add some extra logic for fetching the user age since we need to parse it to an int
            while (true) {
                // Get the user's age
                GetUserInput(out userAge, "Age: ", "This isn't a valid input, please try again!");

                if (int.IsNegative(userAge)) {
                    PrintLog("This isn't a valid number, make sure it is not negative and doesn't include any letters!", ConsoleColor.DarkYellow);
                } else {
                    break;
                }
            }

            return new UserInformation(userName, userEmail, userAge);
        }

        public static bool GetUserInput(out string? userInput, string userInputQuestion, string invalidInputMessage = "Input invalid, please try again!", bool cancelWhenNull = false) {
            while (true) {
                PrintLog(userInputQuestion, ConsoleColor.Magenta);

                userInput = Console.ReadLine();

                if (!string.IsNullOrEmpty(userInput)) {
                    return true;
                } else {
                    if (cancelWhenNull) return false;

                    PrintLog(invalidInputMessage, ConsoleColor.DarkYellow);
                }
            }
        }

        public static bool GetUserInput(out int userInput, string userInputQuestion, string invalidInputMessage = "Input invalid, please try again!", bool cancelWhenNull = false) {
            while (true) {
                // Get the user inputted number
                PrintLog(userInputQuestion, ConsoleColor.Magenta);

                string? input = Console.ReadLine();

                if (!string.IsNullOrEmpty(input)) {
                    if (int.TryParse(input, out userInput)) {
                        return true;
                    } else {
                        if (cancelWhenNull) return false;

                        PrintLog(invalidInputMessage, ConsoleColor.DarkYellow);
                    }
                } else {
                    if (cancelWhenNull) {
                        userInput = -1;

                        return false;
                    }

                    PrintLog(invalidInputMessage, ConsoleColor.DarkYellow);
                }
            }
        }

        public static void GetUserChangeInformationInput(ref UserInformation currentUserInformation, UserChangeInformationType changeType) {
            switch (changeType) {
                case UserChangeInformationType.name:
                    // Get the user's name
                    GetUserInput(out currentUserInformation.name, "Name: ", "User name invalid. Please try again!");

                    break;
                case UserChangeInformationType.email:
                    string userEmail;
                    while (true) {
                        // Get the user's email
                        GetUserInput(out userEmail, "Email: ", "User email invalid. Please try again!");

                        if (!userEmail.EndsWith("@gmail.com")) {
                            PrintLog("This email isn't valid. Please make sure that the email ends with '@gmail.com' as gmail is the only accepted email at this time!", ConsoleColor.DarkYellow);
                        } else {
                            break;
                        }
                    }

                    currentUserInformation.email = userEmail;
                    break;
                case UserChangeInformationType.age:
                    int userAge;
                    while (true) {
                        // Get the user's age
                        GetUserInput(out userAge, "Age: ", "This isn't a valid input, please try again!");

                        if (int.IsNegative(userAge)) {
                            PrintLog("This isn't a valid number, make sure it is not negative and doesn't include any letters!", ConsoleColor.DarkYellow);
                        } else {
                            break;
                        }
                    }

                    currentUserInformation.age = userAge;
                    break;
                case UserChangeInformationType.multiple:
                    currentUserInformation = GetUserInformation();

                    break;
            }
        }

        public static ProgramOptions GetUserProgramOptionsChoice() {
            PrintLog("Please input the number for one the according actions: ", ConsoleColor.Blue);
            PrintAllProgramOptions();

            while (true) {
                // Get the user inputted number
                GetUserInput(out int inputIndex, "Number: ", "This isn't a valid input, please try again!");

                if (inputIndex <= 0 || inputIndex > Enum.GetValues(typeof(ProgramOptions)).Length) {
                    PrintLog("This isn't a valid number index. Please specify a correct index", ConsoleColor.DarkYellow);
                } else {
                    return (ProgramOptions)inputIndex;
                }
            }
        }

        public static void HandleUserProgramOptionChoice(ProgramOptions programOption) {
            List<Todo> todosList = TodoHandler.GetTodosList();
            int todoInputIndex;

            switch (programOption) {
                case ProgramOptions.Create:
                    Todo createdTodo = TodosSystem.CreateNewTodoFromUserInput();

                    TodoHandler.AddTodo(createdTodo);

                    break;
                case ProgramOptions.Edit:

                    if (todosList.Count <= 0) {
                        PrintLog("There are no todos to be edited, please create a new todo to enable this functionality!", ConsoleColor.DarkYellow);
                        return;
                    }

                    PrintLog("Please specify the number of the todo you would like to edit: ", ConsoleColor.Magenta);
                    TodosSystem.PrintAllTodosFromList(todosList);

                    while (true) {
                        // Get the user inputted number
                        GetUserInput(out todoInputIndex, "Todo Number: ", "This isn't a valid input, please try again!");

                        if (todoInputIndex <= 0 || todoInputIndex > todosList.Count) {
                            PrintLog("This isn't a valid number index. Please specify a correct index", ConsoleColor.DarkYellow);
                        } else {
                            break;
                        }
                    }

                    Todo todoToEdit = todosList[todoInputIndex - 1]; // Get the todo from the list which the user wants to edit

                    // Add logic that makes the user able to edit the todo properly
                    PrintLog(
                        $"Note information: " +
                        $"\n\n Name: {todoToEdit.name}" +
                        $"\n\n Description: {todoToEdit.description}" +
                        $"\n\n Due Date: {todoToEdit.dueDateArray[0]}/{todoToEdit.dueDateArray[1]}/{todoToEdit.dueDateArray[2]}" +
                        $"\n\n Importancy: {todoToEdit.importancy.ToString()}\n",
                        ConsoleColor.Blue
                    );

                    PrintLog("Please fill in all the information to edit the todo accordingly: ", ConsoleColor.Magenta);

                    Todo editedTodo = TodosSystem.CreateNewTodoFromUserInput(todoToEdit);

                    PrintLog("Are you sure?", ConsoleColor.Magenta);
                    while (true) {
                        GetUserInput(out string? userResponse, "y/n");

                        userResponse = userResponse.ToLower();
                        if (userResponse == "y") {
                            break;
                        } else if (userResponse == "n") {
                            PrintLog("Canceled opperation.", ConsoleColor.White);
                            return;
                        }
                    }

                    TodoHandler.EditExistingTodo(todoInputIndex - 1, editedTodo);

                    PrintLog("Successfully edited the todo!", ConsoleColor.Green);
                    break;
                case ProgramOptions.Remove:
                    if (todosList.Count <= 0) {
                        PrintLog("There are no todos to be deleted, please create a new todo to enable this functionality!", ConsoleColor.DarkYellow);
                        return;
                    }

                    PrintLog("Please specify the number of the according todo of which you want to remove: ", ConsoleColor.Blue);
                    TodosSystem.PrintAllTodosFromList(todosList);

                    while (true) {
                        // Get the user inputted number
                        GetUserInput(out todoInputIndex, "Todo Number: ", "This isn't a valid input, please try again!");

                        if (todoInputIndex <= 0 || todoInputIndex > todosList.Count) {
                            PrintLog("This isn't a valid number index. Please specify a correct index", ConsoleColor.DarkYellow);
                        } else {
                            break;
                        }
                    }

                    PrintLog("Are you sure?", ConsoleColor.Blue);
                    while (true) {
                        GetUserInput(out string? userResponse, "y/n");

                        userResponse = userResponse.ToLower();
                        if (userResponse == "y") {
                            break;
                        } else if (userResponse == "n") {
                            PrintLog("Canceled opperation.", ConsoleColor.White);
                            return;
                        }
                    }

                    TodoHandler.RemoveTodo(todoInputIndex - 1);

                    PrintLog("Removed TODO succesfully!", ConsoleColor.Green);
                    break;
                case ProgramOptions.ViewAll:
                    if (todosList.Count <= 0) {
                        PrintLog("There are no todos to be viewed, please create a new todo to enable this functionality!", ConsoleColor.DarkYellow);
                        return;
                    }

                    TodosSystem.PrintAllTodosFromList(todosList);
                    break;
                case ProgramOptions.Export:
                    if (todosList.Count <= 0) {
                        PrintLog("There are no todos to be exported, please create a new todo to enable this functionality!", ConsoleColor.DarkYellow);
                        return;
                    }

                    TodoHandler.ExportTodosToFile();
                    break;
                case ProgramOptions.ExitProgram:
                    shouldRunProgram = false;
                    break;
            }
        }

        public static void PrintUserInformation(UserInformation userInformation) {
            PrintLog(
                $"Name: {userInformation.name}" +
                $"\nEmail: {userInformation.email}" +
                $"\nAge: {userInformation.age}",
                ConsoleColor.DarkBlue
            );
        }

        public static UserInformation GetUserInformation() {
            return userInformation;
        }

        public static void PrintAllProgramOptions() {
            PrintLog(
                " 1. Create a New TODO" +
                "\n 2. Edit an Existing TODO" +
                "\n 3. Remove a TODO" +
                "\n 4. View all TODOs" +
                "\n 5. Export TODOs to file" +
                "\n 6. Exit Program",
                ConsoleColor.DarkBlue
            );
        }

        public static void PrintLog(string message, ConsoleColor messageColor) {
            Console.ForegroundColor = messageColor;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }

    public class UserInformation {
        public string name;
        public string email;
        public int age;

        public UserInformation(string name, string email, int age) {
            this.name = name;
            this.email = email;
            this.age = age;
        }
    }

    public enum UserChangeInformationType {
        name = 1,
        email = 2,
        age = 3,
        multiple = 4
    }

    public enum ProgramOptions {
        Create = 1,
        Edit = 2,
        Remove = 3,
        ViewAll = 4,
        Export = 5,
        ExitProgram
    }
}

// TODO:

// STUFF THAT CONSFUSED ME:

// Using public class UserInformation() {} instead of public class UserInformation {} whilst making a constructor gives me this error:
// "A constructor declared in a type with parameter list must have 'this' constructor initializer"

// I couldn't figure out how to convert an index to the value of the UserChangeInformationType to then pass into the GetUserChangeInformationInput() function.
// Took me about 20 minutes to figure out. Got the answer from this article: https://stackoverflow.com/questions/23563960/how-to-get-enum-value-by-string-or-int

// Markdown files can't be written directly by string in C#? You need to use an external method for compatiblity with markdown?

// The markdown file library has a function called 'NewLineTrivia', which I presumed to be an empty, new line. But it instead is I think a line segment seperator
// which nicely organizes the layout. Then I found out that there's also an 'EmptyLine' function, which does do what I expect it to do, but isn't visible in the 
// final markdown file preview (only in the VS code editing layout).

// For some reason have 2 new trivia lines above and under the TODOs header makes all the trivia lines appear under each TODO, but when you remove them, all the
// trivia lines don't appear under each TODO, despite a new trivia line being defined under each TODO seperately, failing to seperate them as intended. Very weird.

// Asked AI for assistance with the Aspose library for markdown file creation, end result is not as intended since the docs genuinely are too unclear to be able to understand

// I FAILED TO:
// Fix the bug where for the importancy of a todo is not being shown by name and instead by its index.

// WORKED DURATION: 10 hours