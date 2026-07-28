namespace Assignment_ConsoleUtilityTool {
    public class Program {
        public static void Main(string[] args) {
            Console.WriteLine("Welcome!");
            Console.WriteLine("");

            // Ask the user for his information
            UserInformation userInformation = GetUserInformation();

            Console.WriteLine("Got user information. Information:");
            Console.WriteLine($"Name: {userInformation.name}");
            Console.WriteLine($"Email: {userInformation.email}");
            Console.WriteLine($"Age: {userInformation.age}");

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
                        userInput.ToLower();

                        if (int.TryParse(userInput, out int userRespondsIndex) && userRespondsIndex < 5 && userRespondsIndex > 0) {
                            UserChangeInformationType informationType = (UserChangeInformationType)userRespondsIndex;
                            GetUserChangeInformationInput(ref userInformation, informationType);

                            break;
                        } else {
                            Console.WriteLine("This isn't a valid input, please try again!");
                        }
                    }

                    Console.WriteLine("Updated user information. Information:");
                    Console.WriteLine($"Name: {userInformation.name}");
                    Console.WriteLine($"Email: {userInformation.email}");
                    Console.WriteLine($"Age: {userInformation.age}");
                } else {
                    while (true) {
                        Console.WriteLine("This is not a valid answer, please try again: ");
                    }
                }
            }

            // Rest of program...
            return;
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
}

// STUFF THAT CONSFUSED ME:

// Using public class UserInformation() {} instead of public class UserInformation {} whilst making a constructor gives me this error:
// "A constructor declared in a type with parameter list must have 'this' constructor initializer"

// I couldn't figure out how to convert an index to the value of the UserChangeInformationType to then pass into the GetUserChangeInformationInput() function.
// Took me about 20 minutes to figure out. Got the answer from this article: https://stackoverflow.com/questions/23563960/how-to-get-enum-value-by-string-or-int