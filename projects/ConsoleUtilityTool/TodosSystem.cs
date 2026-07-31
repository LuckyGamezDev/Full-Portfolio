using Aspose.Html;
using Aspose.Html.Toolkit.Markdown.Syntax;
using MainProgram;

namespace Todos {

    public static class TodosSystem {

        public static Todo CreateNewTodoFromUserInput(bool showInfoPrompt = true) {
            if (showInfoPrompt) {
                Console.WriteLine("To create a new todo, please fill in the following information: ");
            }

            string todoName;
            string todoDescription;
            string[] todoExpirationDateArray;
            Todo.Importancy todoImportancy;

            // Get the todo name
            Program.GetUserInput(out todoName, "Name: ");

            // Get the todo description
            Program.GetUserInput(out todoDescription, "Description: ");

            // Get the todo expiration
            bool isCorrectDate = false;
            while (true) {
                Program.GetUserInput(out string todoExpiration, "Expiration date (D/M/Y) (eg. 27/09/2010): ");
                todoExpirationDateArray = todoExpiration.Split('/');

                for (int i = 0; i < todoExpirationDateArray.Length; i++) {
                    if (!int.TryParse(todoExpirationDateArray[i], out int dateResult)) {
                        Console.WriteLine("This isn't a valid date!");
                    }

                    switch (i) {
                        case 0: // Day
                            if (dateResult > 31) {
                                Console.WriteLine("The day date is invalid! Please try again!");
                                isCorrectDate = false;

                                break;
                            }

                            break;
                        case 1: // Month
                            if (dateResult > 12) {
                                Console.WriteLine("The month date is invalid! Please try again!");
                                isCorrectDate = false;

                                break;
                            }

                            break;
                        case 2: // Year
                            if (dateResult > 4000) {
                                Console.WriteLine("Dates further than 4000 are not supported. Please input an earlier year!");
                                isCorrectDate = false;

                                break;
                            }

                            isCorrectDate = true;
                            break;
                    }
                }

                if (isCorrectDate) {
                    break;
                }
            }


            Console.WriteLine(
                "Specify the importancy of this todo by inputting the according number:" +
                "\n 1. Low" +
                "\n 2. Medium" +
                "\n 3. High"
            );

            // Get the todo importancy
            while (true) {
                // Get the importancy index
                Program.GetUserInput(out int todoImportancyIndex, "Number: ", "This isn't a valid number, make sure it doesn't include any letters!");

                if (todoImportancyIndex <= 0 || todoImportancyIndex > Enum.GetValues(typeof(Todo.Importancy)).Length) {
                    Console.WriteLine("This isn't a valid number index. Please specify a correct index");
                } else {
                    todoImportancy = (Todo.Importancy)todoImportancyIndex;
                    break;
                }
            }

            return new Todo(todoName, todoDescription, todoExpirationDateArray, todoImportancy);
        }

        public static Todo CreateNewTodoFromUserInput(Todo todoToEdit, bool showInfoPrompt = false) {
            if (showInfoPrompt) {
                Console.WriteLine("To create a new todo, please fill in the following information: ");
            }

            Console.WriteLine("You can press 'Enter' on an empty input to use the todo's original input!");

            string todoName;
            string todoDescription;
            string[] todoExpirationDateArray;
            Todo.Importancy todoImportancy;

            // Get the todo name
            if (!Program.GetUserInput(out todoName, "Name: ", cancelWhenNull: true)) { // If the user input is null
                todoName = todoToEdit.name; // Set the todo name to the initial, previous name

                Console.WriteLine(todoName);
            }

            // Get the todo description
            if (!Program.GetUserInput(out todoDescription, "Description: ", cancelWhenNull: true)) { // If the user input is null
                todoDescription = todoToEdit.description; // Set the todo description to the initial, previous description

                Console.WriteLine(todoDescription);
            }

            // Get the todo expiration
            bool isCorrectDate = false;
            while (true) {
                if (!Program.GetUserInput(out string todoExpiration, "Expiration date (D/M/Y) (eg. 27/09/2010): ", cancelWhenNull: true)) {
                    todoExpirationDateArray = todoToEdit.dueDateArray; // Assign the due date to the previous, initial due date

                    Console.WriteLine($"{todoExpirationDateArray[0]}/{ todoExpirationDateArray[1]}/{todoExpirationDateArray[2]}");

                    break;
                } else { // If the user DID input a (new) value, handle it

                    todoExpirationDateArray = todoExpiration.Split('/');

                    for (int i = 0; i < todoExpirationDateArray.Length; i++) {
                        if (!int.TryParse(todoExpirationDateArray[i], out int dateResult)) {
                            Console.WriteLine("This isn't a valid date!");
                        }

                        switch (i) {
                            case 0: // Day
                                if (dateResult > 31) {
                                    Console.WriteLine("The day date is invalid! Please try again!");
                                    isCorrectDate = false;

                                    break;
                                }

                                break;
                            case 1: // Month
                                if (dateResult > 12) {
                                    Console.WriteLine("The month date is invalid! Please try again!");
                                    isCorrectDate = false;

                                    break;
                                }

                                break;
                            case 2: // Year
                                if (dateResult > 4000) {
                                    Console.WriteLine("Dates further than 4000 are not supported. Please input an earlier year!");
                                    isCorrectDate = false;

                                    break;
                                }

                                isCorrectDate = true;
                                break;
                        }
                    }

                    if (isCorrectDate) {
                        break;
                    }
                }
            }


            Console.WriteLine(
                "Specify the importancy of this todo by inputting the according number:" +
                "\n 1. Low" +
                "\n 2. Medium" +
                "\n 3. High"
            );

            // Get the todo importancy
            while (true) {
                // Get the importancy index
                if (!Program.GetUserInput(out int todoImportancyIndex, "Number: ", "This isn't a valid number, make sure it doesn't include any letters!", cancelWhenNull: true)) {
                    todoImportancy = todoToEdit.importancy; // Assign the importancy to the previous, initial value

                    Console.WriteLine(todoImportancy.ToString());

                    break;
                }

                if (todoImportancyIndex <= 0 || todoImportancyIndex > Enum.GetValues(typeof(Todo.Importancy)).Length) {
                    Console.WriteLine("This isn't a valid number index. Please specify a correct index");
                } else {
                    todoImportancy = (Todo.Importancy)todoImportancyIndex;
                    break;
                }
            }

            return new Todo(todoName, todoDescription, todoExpirationDateArray, todoImportancy);
        }

        public static void PrintAllTodosFromList(List<Todo> todosList) {
            int todoIndex = 1;
            for (int i = 0; i < todosList.Count; i++) {
                Console.WriteLine($"{todoIndex}: {todosList[i].name} | {todosList[i].dueDateArray[0]}/{todosList[i].dueDateArray[1]}/{todosList[i].dueDateArray[2]}");

                todoIndex++;
            }
        }
    }

    public static class TodoHandler {
        private static List<Todo> todosList = new List<Todo>();

        public static void AddTodo(Todo todo) {
            todosList.Add(todo);
        }

        public static void EditExistingTodo(int todoIndex, Todo replacementTodo) {
            if (todosList.Count <= 0) {
                Console.WriteLine("Unable to edit note since there aren't any notes saved!");
            }

            todosList[todoIndex] = replacementTodo; // Overwrite the old todo with the newly, edited todo
        }

        public static void RemoveTodo(int todoIndex) {
            if (todosList.Count <= 0) {
                Console.WriteLine("Unable to delete note since there aren't any notes saved!");
            }

            todosList.RemoveAt(todoIndex);
        }

        public static List<Todo> GetTodosList() {
            return new List<Todo>(todosList);
        }

        public static void ExportTodosToFile() {
            if (todosList.Count <= 0) {
                Console.WriteLine("Unable to export note(s) since there aren't any notes saved!");

                return;
            }

            List<Todo> sortedTodosList = GetTodosListSortInOrder();

            Console.WriteLine("Organized new todos list: ");
            foreach (Todo todo in sortedTodosList) {
                Console.WriteLine($"{todo.name} | {todo.dueDateArray[0]}/{todo.dueDateArray[1]}/{todo.dueDateArray[2]}");
            }

            // Export this as an organized file
            UserInformation userInfo = Program.GetUserInformation();

            MarkdownSyntaxTree markdown = new MarkdownSyntaxTree(new Configuration());
            MarkdownSyntaxFactory markdownSyntaxFactory = markdown.SyntaxFactory;

            var userInfoTopText = markdownSyntaxFactory.Text(
                $"Name: {userInfo.name}" +
                $"\nEmail: {userInfo.email}" +
                $"\nAge: {userInfo.age}"
            );

            markdown.AppendChild(userInfoTopText);

            markdown.NewLine(markdownSyntaxFactory);
            markdown.NewLine(markdownSyntaxFactory);
            var todosHeading = markdownSyntaxFactory.AtxHeading("TODOs", 2);
            markdown.NewLine(markdownSyntaxFactory);
            markdown.NewLine(markdownSyntaxFactory);

            markdown.AppendChild(todosHeading);

            int index = 1;
            foreach (Todo todo in sortedTodosList) {
                // Write all the TODO information here

                var todoHeading = markdownSyntaxFactory.AtxHeading($"\n\n{todo.name}", 2);

                var todoText = markdownSyntaxFactory.Text(
                    $"\n\nDescription:\n{todo.description}" +
                    $"\n\nDue: {todo.dueDateArray[0]}/{todo.dueDateArray[1]}/{todo.dueDateArray[2]}" +
                    $"\nImportancy:{todo.importancy}"
                );

                markdown.AppendChild(todoHeading);
                markdown.AppendChild(todoText);

                markdown.NewLine(markdownSyntaxFactory);

                index++;
            }

            // Get the directory as to where the file should be saved
            string workingDirectory = Environment.CurrentDirectory;
            string parentDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;

            try {
                markdown.Save(Path.Combine(parentDirectory, "Todo_Export.md"));
            } catch (Exception exception) {
                Console.WriteLine("Failed to create file. Exception: " + exception);
            }

            //// Write file ownership to the user
            //exportFileStreamWriter.WriteLine(
            //    $"**Name:** {userInfo.name}" +
            //    $"\n**Email:** {userInfo.email}" +
            //    $"\n**Age:** {userInfo.age}"
            //);

            //exportFileStreamWriter.WriteLine("\n\n#TODOs\n\n");

            //exportFileStreamWriter.WriteLine("---");

            Console.WriteLine("Succesfully exported todos to file!");
        }

        private static List<Todo> GetTodosListSortInOrder() {
            if (todosList.Count <= 0) {
                Console.WriteLine("Unable to sort notes since there aren't any notes saved!");

                return new List<Todo>();
            }

            // Do a loop to order all the todos in the right order
            List<Todo> todosInOrderList = new List<Todo>();
            Todo? mostPriorityTodo = null;
            while (todosInOrderList.Count < todosList.Count) {
                foreach (Todo todo in todosList) {
                    if (todosInOrderList.Contains(todo)) { // If the todo we are currently on is already sorted, skip it
                        continue;
                    }

                    if (mostPriorityTodo == null) {
                        mostPriorityTodo = todo; // This todo is not yet sorted so set it as most priority to compare it to others

                        continue; // Skip to the next todo to avoid comparing this todo to itself
                    }

                    int[] todoDateArray = { int.Parse(todo.dueDateArray[0]), int.Parse(todo.dueDateArray[1]), int.Parse(todo.dueDateArray[2]) };
                    int[] mostPriorityTodoDateArray = { int.Parse(mostPriorityTodo.dueDateArray[0]), int.Parse(mostPriorityTodo.dueDateArray[1]), int.Parse(mostPriorityTodo.dueDateArray[2]) };

                    if (todoDateArray[2] < mostPriorityTodoDateArray[2]) { // If the todo is older in year
                        mostPriorityTodo = todo; // This is the new most priority todo since if the year is older, it's definitely due earlier

                        continue;
                    } else if (todoDateArray[2] == mostPriorityTodoDateArray[2]) { // If the todo has the same due year as the current most priority one
                        if (todoDateArray[1] < mostPriorityTodoDateArray[1]) { // If the todo is older in month
                            mostPriorityTodo = todo; // This is now the new most priority todo since it has the same month but an older month

                            continue;
                        } else if (todoDateArray[1] == mostPriorityTodoDateArray[1]) { // If the month both are the same (also the year because of previous check)
                            if (todoDateArray[0] < mostPriorityTodoDateArray[0]) { // If the day of the todo is older
                                mostPriorityTodo = todo; // This is the new most priority todo since its day is older than the previous most priority
                            } else if (todoDateArray[0] == mostPriorityTodoDateArray[0]) { // If they both are the exact same date
                                                                                           // Check priority
                                switch (todo.importancy) {
                                    case Todo.Importancy.Low:
                                        if (mostPriorityTodo.importancy == Todo.Importancy.Low) { // Both are due on the same date and have the same importancy
                                                                                                  // Leave the current most priority todo as is
                                            break;
                                        }

                                        // The current most priority is higher importancy, so leave as is
                                        break;
                                    case Todo.Importancy.Medium:
                                        switch (mostPriorityTodo.importancy) {
                                            case Todo.Importancy.Low:
                                                // Set this todo as the new most priority since it is higher in priority
                                                mostPriorityTodo = todo;

                                                break;
                                            case Todo.Importancy.Medium: // Both are the same importancy and date
                                                                         // Leave the current most priority todo as is
                                                break;
                                            case Todo.Importancy.High: // The current most priority has higher importancy
                                                                       // Leave the current most priority todo as is
                                                break;
                                        }
                                        break;
                                    case Todo.Importancy.High:
                                        switch (mostPriorityTodo.importancy) {
                                            case Todo.Importancy.Low:
                                                // Set this todo as the new most priority since it is higher in priority
                                                mostPriorityTodo = todo;

                                                break;
                                            case Todo.Importancy.Medium:
                                                // Set this todo as the new most priority since it is higher in priority
                                                mostPriorityTodo = todo;
                                                break;
                                            case Todo.Importancy.High: // Both are the same importancy and date
                                                                       // Leave the current most priority todo as is
                                                break;
                                        }
                                        break;
                                }
                            }
                        }
                    }
                }

                todosInOrderList.Add(mostPriorityTodo);
                mostPriorityTodo = null;
            }

            return todosInOrderList;
        }

        public static void InitTestTodos() {
            Todo[] initTodos = {
                new Todo("first todo", "first todo YAY", ["27", "09", "2020"], Todo.Importancy.High),
                new Todo("second todo", "second todo", ["28", "09", "2013"], Todo.Importancy.High),
                new Todo("third todo", "third todo", ["27", "04", "2015"], Todo.Importancy.High),
                new Todo("fourth todo", "fourth todo", ["27", "12", "2023"], Todo.Importancy.High),
                new Todo("fifth todo", "fifth todo", ["21", "02", "2024"], Todo.Importancy.High)
            };

            todosList.AddRange(initTodos);
        }
    }

    public class Todo {
        public string name;
        public string description;
        public string[] dueDateArray;

        public Importancy importancy;

        public Todo(string name, string description, string[] dueDate, Importancy importancy) {
            this.name = name;
            this.description = description;
            this.dueDateArray = dueDate;
            this.importancy = importancy;
        }

        public enum Importancy {
            Low,
            Medium,
            High
        }
    }

    public static class MarkdownSyntaxTreeExtensions {
        public static void NewLine(this MarkdownSyntaxTree md, MarkdownSyntaxFactory mdf) {
            md.AppendChild(mdf.NewLineTrivia());
        }
    }
}
