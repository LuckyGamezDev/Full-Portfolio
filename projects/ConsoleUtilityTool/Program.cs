namespace Assignment_ConsoleUtilityTool {
    public class Program {
        public static void Main(string[] args) {
            Console.WriteLine("Welcome!");
            Console.WriteLine("");

            // Ask the user for his information
            UserInformation userInformation = GetUserInformation();

            Console.WriteLine("Got user information. Information:");
            PrintUserInformation(userInformation);

            while (true) {

                Console.WriteLine("Please confirm if this is correct by typing either 'correct' or 'incorrect':");

                string userInput = Console.ReadLine();

                if (userInput == "correct") {
                    Console.WriteLine("Thanks for clarrifying, you can now use this program!");

                    break;
                } else if (userInput == "incorrect") {
                    Console.WriteLine("Please specify which input(s) is/are incorrect by typing the according number of one of the following options: " +
                        "\n1: name,\n 2: email,\n 3: age,\n 4: multiple\n\n" +
                        "If more than 1 is incorrect, re-input all the asked information once more: ");

                    while (true) {
                        userInput = Console.ReadLine();
                        userInput = userInput.ToLower();

                        if (int.TryParse(userInput, out int userRespondsIndex) && userRespondsIndex > 0 
                            && userRespondsIndex <= Enum.GetValues(typeof(UserChangeInformationType)).Length) {

                            UserChangeInformationType informationType = (UserChangeInformationType)userRespondsIndex;
                            GetUserChangeInformationInput(ref userInformation, informationType);

                            break;
                        } else {
                            Console.WriteLine("This isn't a valid input, please try again!");
                        }
                    }

                    Console.WriteLine("Updated user information. Information:");
                    PrintUserInformation(userInformation);
                } else {
                    while (true) {
                        Console.WriteLine("This is not a valid answer, please try again: ");
                    }
                }
            }

            Console.WriteLine("\n You can start creating TODOs and this program will store them all in a nice, organized file");

            ProgramOptions userProgramOptionChoice = GetUserProgramOptionsChoice();

            
        }

        public static UserInformation GetUserInformation() {
            string? userName = "";
            string? userEmail = "";
            int userAge = -1;

            Console.WriteLine("Please answer the following questions to get started!");

            // Add some extra logic for making sure the name doesn't include any indexes
            while (true) {
                // Get the user's name
                GetUserInput(out userName, "Name: ", "User name invalid. Please try again!");
                bool containsInt = userName.Any(char.IsDigit); // Copy pasted from someone, did not know this was a thing

                if (containsInt) {
                    Console.WriteLine("This is not a valid name since it includes a number. Please make sure your name only includes letters!");
                } else {
                    break;
                }
            }

            // Add some extra logic for fetching the user email as we need to confirm it has the correct suffix (which is @gmail.com)
            while (true) {
                // Get the user's email
                GetUserInput(out userEmail, "Email: ", "User email invalid. Please try again!");

                if (!userEmail.EndsWith("@gmail.com")) {
                    Console.WriteLine("This email isn't valid. Please make sure that the email ends with '@gmail.com' as gmail is the only accepted email at this time!");
                } else {
                    break;
                }
            }

            // Add some extra logic for fetching the user age since we need to parse it to an int
            while (true) {
                // Get the user's age
                GetUserInput(out string? userAgeInput, "Age: ", "This isn't a valid input, please try again!");

                if (!int.TryParse(userAgeInput, out userAge) || int.IsNegative(userAge)) {
                    Console.WriteLine("This isn't a valid number, make sure it is not negative and doesn't include any letters!");
                } else {
                    break;
                }
            }

            return new UserInformation(userName, userEmail, userAge);
        }

        public static void GetUserInput(out string? userInput, string userInputQuestion, string invalidInputMessage = "Input invalid, please try again!") {
            while (true) {
                Console.WriteLine(userInputQuestion);

                userInput = Console.ReadLine();

                if (!string.IsNullOrEmpty(userInput)) {
                    return;
                } else {
                    Console.WriteLine(invalidInputMessage);
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
                            Console.WriteLine("This email isn't valid. Please make sure that the email ends with '@gmail.com' as gmail is the only accepted email at this time!");
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
                        GetUserInput(out string? userAgeInput, "Age: ", "This isn't a valid input, please try again!");

                        if (!int.TryParse(userAgeInput, out userAge) || int.IsNegative(userAge)) {
                            Console.WriteLine("This isn't a valid number, make sure it is not negative and doesn't include any letters!");
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
            Console.WriteLine("Please input the number for one the according actions: ");
            PrintAllProgramOptions();

            while (true) {
                // Get the user inputted number
                GetUserInput(out string? inputIndexString, "Number: ", "This isn't a valid input, please try again!");

                if (!int.TryParse(inputIndexString, out int inputIndex)) {
                    Console.WriteLine("This isn't a valid number, make sure it doesn't include any letters!");
                } else {
                    if (inputIndex <= 0 || inputIndex > Enum.GetValues(typeof(ProgramOptions)).Length) {
                        Console.WriteLine("This isn't a valid number index. Please specify a correct index");
                    } else {
                        return (ProgramOptions)inputIndex;
                    }
                }
            }
        }

        public static void HandleUserProgramOptionChoice(ProgramOptions programOption) {
            List<Todo> todosList = TodoHandler.GetTodosList();
            int todoInputIndex;

            switch (programOption) {
                case ProgramOptions.Create:
                    Todo createdTodo = CreateNewTodoFromUserInput();
                    break;
                case ProgramOptions.Edit:

                    Console.WriteLine("Please specify the number of the todo you would like to edit: ");
                    PrintAllTodosFromList(todosList);

                    while (true) {
                        // Get the user inputted number
                        GetUserInput(out string? inputIndexString, "Todo Number: ", "This isn't a valid input, please try again!");

                        if (!int.TryParse(inputIndexString, out todoInputIndex)) {
                            Console.WriteLine("This isn't a valid number, make sure it doesn't include any letters!");
                        } else {
                            if (todoInputIndex <= 0 || todoInputIndex > todosList.Count + 1) {
                                Console.WriteLine("This isn't a valid number index. Please specify a correct index");
                            } else {
                                break;
                            }
                        }
                    }

                    Todo todoToEdit = todosList[todoInputIndex]; // Get the todo from the list which the user wants to edit

                    // Add logic that makes the user able to edit the todo properly
                    break;
                case ProgramOptions.Remove:
                    Console.WriteLine("Please specify the number of the according todo of which you want to remove: ");
                    PrintAllTodosFromList(todosList);

                    while (true) {
                        // Get the user inputted number
                        GetUserInput(out string? inputIndexString, "Todo Number: ", "This isn't a valid input, please try again!");

                        if (!int.TryParse(inputIndexString, out todoInputIndex)) {
                            Console.WriteLine("This isn't a valid number, make sure it doesn't include any letters!");
                        } else {
                            if (todoInputIndex <= 0 || todoInputIndex > todosList.Count + 1) {
                                Console.WriteLine("This isn't a valid number index. Please specify a correct index");
                            } else {
                                break;
                            }
                        }
                    }

                    Console.WriteLine("Are you sure?");
                    while (true) {
                        GetUserInput(out string? userResponse, "Y/N");

                        userResponse = userResponse.ToLower();
                        if (userResponse == "y") {
                            break;
                        } else if (userResponse == "n") {
                            Console.WriteLine("Canceled opperation.");
                            return;
                        }
                    }

                    TodoHandler.RemoveTodo(todoInputIndex);

                    Console.WriteLine("Removed TODO succesfully!");
                    break;
                case ProgramOptions.ViewAll:
                    break;
                case ProgramOptions.Export:
                    break;
            }
        }

        public static void PrintAllTodosFromList(List<Todo> todosList) {
            int todoIndex = 1;
            for (int i = 0; i < todosList.Count; i++) {
                Console.WriteLine($"{todoIndex}: {todosList[i].name}");
            }
        }

        public static Todo CreateNewTodoFromUserInput() {
            Console.WriteLine("To create a new todo, please fill in the following information: ");
            Console.WriteLine("Name: ");

            string todoName;
            string todoDescription;
            string todoExpiration;
            Todo.Importancy todoImportancy;

            // Get the todo name
            GetUserInput(out todoName, "Name: ");

            // Get the todo description
            GetUserInput(out todoDescription, "Specify all the information for the todo: ");

            // Get the todo expiration
            GetUserInput(out todoExpiration, "Specify the expiration date of this todo: ");


            Console.WriteLine(
                "Specify the importancy of this todo by inputting the according number:" +
                "\n 1. Low" +
                "\n 2. Medium" +
                "\n 3. High"
            );

            // Get the todo importancy
            while (true) {
                // Get the importancy index
                GetUserInput(out string? todoImportancyIndexString, "Number: ", "This isn't a valid input, please try again!");

                if (!int.TryParse(todoImportancyIndexString, out int todoImportancyIndex)) {
                    Console.WriteLine("This isn't a valid number, make sure it doesn't include any letters!");
                } else {
                    if (todoImportancyIndex <= 0 || todoImportancyIndex > Enum.GetValues(typeof(Todo.Importancy)).Length) {
                        Console.WriteLine("This isn't a valid number index. Please specify a correct index");
                    } else {
                        todoImportancy = (Todo.Importancy)todoImportancyIndex;
                        break;
                    }
                }
            }

            return new Todo(todoName, todoDescription, todoExpiration, todoImportancy);
        }

        public static void PrintUserInformation(UserInformation userInformation) {
            // Print all three in the same log to avoid race condition logs that split up this log in half
            Console.WriteLine(
                $"Name: {userInformation.name}" +
                $"\nEmail: {userInformation.email}" +
                $"\nAge: {userInformation.age}"
            );
        }

        public static void PrintAllProgramOptions() {
            Console.WriteLine(
                "1. Create a new TODO" +
                "\n 2. Edit an existing TODO" +
                "\n 3. Remove a TODO" +
                "\n 4. View all TODOs" +
                "\n 5. Export TODOs to file"
            );
        }
    }


    public static class TodoHandler {
        private static List<Todo> todosList = new List<Todo>();

        public static void AddTodo(Todo todo) {
            todosList.Add(todo);
        }

        public static void EditExistingTodo(int todoIndex, Todo replacementTodo) {
            todosList[todoIndex] = replacementTodo; // Overwrite the old todo with the newly, edited todo
        }

        public static void RemoveTodo(int todoIndex) {
            todosList.RemoveAt(todoIndex);
        }

        public static List<Todo> GetTodosList() {
            return new List<Todo>(todosList);
        }

        public static void ExportTodosToFile() {

        }
    }

    public class Todo {
        public string name;
        public string description;
        public string dueDate;

        public Importancy importancy;

        public Todo(string name, string description, string dueDate, Importancy importancy) {
            this.name = name;
            this.description = description;
            this.dueDate = dueDate;
            this.importancy = importancy;
        }

        public enum Importancy {
            Low,
            Medium,
            High
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
        Export = 5
    }
}

// TODO:
// Add logic that makes the user able to edit the todo properly | Line 220
// Create a function for fetching an index from the user input since there's a lot of code that is being copied

// STUFF THAT CONSFUSED ME:

// Using public class UserInformation() {} instead of public class UserInformation {} whilst making a constructor gives me this error:
// "A constructor declared in a type with parameter list must have 'this' constructor initializer"

// I couldn't figure out how to convert an index to the value of the UserChangeInformationType to then pass into the GetUserChangeInformationInput() function.
// Took me about 20 minutes to figure out. Got the answer from this article: https://stackoverflow.com/questions/23563960/how-to-get-enum-value-by-string-or-int

// WORKED DURATION: 4 hours