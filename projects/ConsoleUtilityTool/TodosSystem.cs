using Aspose.Html;
using Aspose.Html.Toolkit.Markdown.Syntax;
using MainProgram;

namespace Todos {

    public static class TodosSystem {

        public static Todo CreateNewTodoFromUserInput(bool showInfoPrompt = true) {
            if (showInfoPrompt) {
                MainProgram.MainProgram.PrintLog("To create a new todo, please fill in the following information: ", ConsoleColor.Blue);
            }

            string todoName;
            string todoDescription;
            string[] todoExpirationDateArray;
            Todo.Importancy todoImportancy;

            // Get the todo name
            MainProgram.MainProgram.GetUserInput(out todoName, "Name: ");

            // Get the todo description
            MainProgram.MainProgram.GetUserInput(out todoDescription, "Description: ");

            // Get the todo expiration
            bool isCorrectDate = false;
            while (true) {
                MainProgram.MainProgram.GetUserInput(out string todoExpiration, "Expiration date (D/M/Y) (eg. 27/09/2010): ");
                todoExpirationDateArray = todoExpiration.Split('/');

                for (int i = 0; i < todoExpirationDateArray.Length; i++) {
                    if (!int.TryParse(todoExpirationDateArray[i], out int dateResult)) {
                        MainProgram.MainProgram.PrintLog("This isn't a valid date!", ConsoleColor.DarkYellow);
                    }

                    switch (i) {
                        case 0: // Day
                            if (dateResult > 31) {
                                MainProgram.MainProgram.PrintLog("The day date is invalid! Please try again!", ConsoleColor.DarkYellow);
                                isCorrectDate = false;

                                break;
                            }

                            break;
                        case 1: // Month
                            if (dateResult > 12) {
                                MainProgram.MainProgram.PrintLog("The month date is invalid! Please try again!", ConsoleColor.DarkYellow);
                                isCorrectDate = false;

                                break;
                            }

                            break;
                        case 2: // Year
                            if (dateResult > 4000) {
                                MainProgram.MainProgram.PrintLog("Dates further than 4000 are not supported. Please input an earlier year!", ConsoleColor.DarkYellow);
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


            MainProgram.MainProgram.PrintLog(
                "Specify the importancy of this todo by inputting the according number:" +
                "\n 1. Low" +
                "\n 2. Medium" +
                "\n 3. High",
                ConsoleColor.DarkBlue
            );

            // Get the todo importancy
            while (true) {
                // Get the importancy index
                MainProgram.MainProgram.GetUserInput(out int todoImportancyIndex, "Number: ", "This isn't a valid number, make sure it doesn't include any letters!");

                if (todoImportancyIndex <= 0 || todoImportancyIndex > Enum.GetValues(typeof(Todo.Importancy)).Length) {
                    MainProgram.MainProgram.PrintLog("This isn't a valid number index. Please specify a correct index", ConsoleColor.DarkYellow);
                } else {
                    todoImportancy = (Todo.Importancy)todoImportancyIndex;
                    break;
                }
            }

            Console.WriteLine(todoImportancy.ToString());
            return new Todo(todoName, todoDescription, todoExpirationDateArray, todoImportancy);
        }

        public static Todo CreateNewTodoFromUserInput(Todo todoToEdit, bool showInfoPrompt = false) {
            if (showInfoPrompt) {
                MainProgram.MainProgram.PrintLog("To create a new todo, please fill in the following information: ", ConsoleColor.Blue);
            }

            MainProgram.MainProgram.PrintLog("You can press 'Enter' on an empty input to use the todo's original input!", ConsoleColor.Cyan);

            string todoName;
            string todoDescription;
            string[] todoExpirationDateArray;
            Todo.Importancy todoImportancy;

            // Get the todo name
            if (!MainProgram.MainProgram.GetUserInput(out todoName, "Name: ", cancelWhenNull: true)) { // If the user input is null
                todoName = todoToEdit.name; // Set the todo name to the initial, previous name

                MainProgram.MainProgram.PrintLog(todoName, ConsoleColor.Cyan);
            }

            // Get the todo description
            if (!MainProgram.MainProgram.GetUserInput(out todoDescription, "Description: ", cancelWhenNull: true)) { // If the user input is null
                todoDescription = todoToEdit.description; // Set the todo description to the initial, previous description

                MainProgram.MainProgram.PrintLog(todoDescription, ConsoleColor.Cyan);
            }

            // Get the todo expiration
            bool isCorrectDate = false;
            while (true) {
                if (!MainProgram.MainProgram.GetUserInput(out string todoExpiration, "Expiration date (D/M/Y) (eg. 27/09/2010): ", cancelWhenNull: true)) {
                    todoExpirationDateArray = todoToEdit.dueDateArray; // Assign the due date to the previous, initial due date

                    MainProgram.MainProgram.PrintLog($"{todoExpirationDateArray[0]}/{ todoExpirationDateArray[1]}/{todoExpirationDateArray[2]}", ConsoleColor.Cyan);

                    break;
                } else { // If the user DID input a (new) value, handle it

                    todoExpirationDateArray = todoExpiration.Split('/');

                    for (int i = 0; i < todoExpirationDateArray.Length; i++) {
                        if (!int.TryParse(todoExpirationDateArray[i], out int dateResult)) {
                            MainProgram.MainProgram.PrintLog("This isn't a valid date!", ConsoleColor.DarkYellow);
                        }

                        switch (i) {
                            case 0: // Day
                                if (dateResult > 31) {
                                    MainProgram.MainProgram.PrintLog("The day date is invalid! Please try again!", ConsoleColor.DarkYellow);
                                    isCorrectDate = false;

                                    break;
                                }

                                break;
                            case 1: // Month
                                if (dateResult > 12) {
                                    MainProgram.MainProgram.PrintLog("The month date is invalid! Please try again!", ConsoleColor.DarkYellow);
                                    isCorrectDate = false;

                                    break;
                                }

                                break;
                            case 2: // Year
                                if (dateResult > 4000) {
                                    MainProgram.MainProgram.PrintLog("Dates further than 4000 are not supported. Please input an earlier year!", ConsoleColor.DarkYellow);
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


            MainProgram.MainProgram.PrintLog(
                "Specify the importancy of this todo by inputting the according number:" +
                "\n 1. Low" +
                "\n 2. Medium" +
                "\n 3. High",
                ConsoleColor.DarkBlue
            );

            // Get the todo importancy
            while (true) {
                // Get the importancy index
                if (!MainProgram.MainProgram.GetUserInput(out int todoImportancyIndex, "Number: ", "This isn't a valid number, make sure it doesn't include any letters!", cancelWhenNull: true)) {
                    todoImportancy = todoToEdit.importancy; // Assign the importancy to the previous, initial value

                    MainProgram.MainProgram.PrintLog(todoImportancy.ToString(), ConsoleColor.Cyan);

                    break;
                }

                if (todoImportancyIndex <= 0 || todoImportancyIndex > Enum.GetValues(typeof(Todo.Importancy)).Length) {
                    MainProgram.MainProgram.PrintLog("This isn't a valid number index. Please specify a correct index", ConsoleColor.Cyan);
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
                MainProgram.MainProgram.PrintLog($"{todoIndex}: {todosList[i].name} | {todosList[i].dueDateArray[0]}/{todosList[i].dueDateArray[1]}/{todosList[i].dueDateArray[2]}", ConsoleColor.DarkBlue);

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
                MainProgram.MainProgram.PrintLog("Unable to edit note since there aren't any notes saved!", ConsoleColor.DarkYellow);
            }

            todosList[todoIndex] = replacementTodo; // Overwrite the old todo with the newly, edited todo
        }

        public static void RemoveTodo(int todoIndex) {
            if (todosList.Count <= 0) {
                MainProgram.MainProgram.PrintLog("Unable to delete note since there aren't any notes saved!", ConsoleColor.DarkYellow);
            }

            todosList.RemoveAt(todoIndex);
        }

        public static List<Todo> GetTodosList() {
            return new List<Todo>(todosList);
        }

        public static void ExportTodosToFile() {
            if (todosList.Count <= 0) {
                MainProgram.MainProgram.PrintLog("Unable to export note(s) since there aren't any notes saved!", ConsoleColor.DarkYellow);

                return;
            }

            List<Todo> sortedTodosList = GetTodosListSortInOrder();

            MainProgram.MainProgram.PrintLog("Organized new todos list: ", ConsoleColor.Blue);
            foreach (Todo todo in sortedTodosList) {
                MainProgram.MainProgram.PrintLog($"{todo.name} | {todo.dueDateArray[0]}/{todo.dueDateArray[1]}/{todo.dueDateArray[2]}", ConsoleColor.DarkBlue);
            }

            // Export this as an organized file
            UserInformation userInfo = MainProgram.MainProgram.GetUserInformation();

            MarkdownSyntaxTree markdown = new MarkdownSyntaxTree(new Configuration());
            MarkdownSyntaxFactory markdownSyntaxFactory = markdown.SyntaxFactory;

            var userNameTopText = markdownSyntaxFactory.Text($"Name: {userInfo.name}");
            markdown.AppendChild(userNameTopText);
            markdown.NewLineSeperator(markdownSyntaxFactory);

            var userEmailTopText = markdownSyntaxFactory.Text($"Email: {userInfo.email}");
            markdown.AppendChild(userEmailTopText);
            markdown.NewLineSeperator(markdownSyntaxFactory);

            var userAgeTopText = markdownSyntaxFactory.Text($"Age: {userInfo.age}");
            markdown.AppendChild(userAgeTopText);
            markdown.NewLineSeperator(markdownSyntaxFactory);

            markdown.NewLineSeperator(markdownSyntaxFactory);
            markdown.NewLineSeperator(markdownSyntaxFactory);
            var todosHeading = markdownSyntaxFactory.AtxHeading("TODOs", 1);
            markdown.NewLineSeperator(markdownSyntaxFactory);
            markdown.NewLineSeperator(markdownSyntaxFactory);

            markdown.AppendChild(todosHeading);

            int index = 1;
            foreach (Todo todo in sortedTodosList) {
                // Write all the TODO information here

                markdown.NewLineSeperator(markdownSyntaxFactory);
                markdown.NewLineSeperator(markdownSyntaxFactory);
                var todoNameHeading = markdownSyntaxFactory.AtxHeading(todo.name, 6);
                markdown.AppendChild(todoNameHeading);

                markdown.NewLineSeperator(markdownSyntaxFactory);
                var todoDescriptionHeaderText = markdownSyntaxFactory.Text("Description:");
                markdown.AppendChild(todoDescriptionHeaderText);

                markdown.NewLineSeperator(markdownSyntaxFactory);
                var todoDescriptionText = markdownSyntaxFactory.Text($"{todo.description}");
                markdown.AppendChild(todoDescriptionText);

                markdown.NewLineSeperator(markdownSyntaxFactory);

                var todoDueDateText = markdownSyntaxFactory.Text($"Due: {todo.dueDateArray[0]}/{todo.dueDateArray[1]}/{todo.dueDateArray[2]}");
                markdown.AppendChild(todoDueDateText);

                markdown.NewLineSeperator(markdownSyntaxFactory);
                var todoImportancyText = markdownSyntaxFactory.Text($"Importancy: { todo.importancy}");
                markdown.AppendChild(todoImportancyText);

                //var todoLineSeperator = new MarkdownSyntaxToken();
                //markdownSyntaxFactory.ThematicBreak()

                markdown.NewLineSeperator(markdownSyntaxFactory);

                index++;
            }

            // Get the directory as to where the file should be saved
            string workingDirectory = Environment.CurrentDirectory;
            string parentDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;

            try {
                markdown.Save(Path.Combine(parentDirectory, "Todo_Export.md"));
            } catch (Exception exception) {
                MainProgram.MainProgram.PrintLog("Failed to create file. Exception: " + exception, ConsoleColor.DarkYellow);
            }

            MainProgram.MainProgram.PrintLog("Succesfully exported todos to file!", ConsoleColor.Green);
        }

        private static List<Todo> GetTodosListSortInOrder() {
            if (todosList.Count <= 0) {
                MainProgram.MainProgram.PrintLog("Unable to sort notes since there aren't any notes saved!", ConsoleColor.DarkYellow);

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
        public static void NewLineSeperator(this MarkdownSyntaxTree md, MarkdownSyntaxFactory mdf) {
            md.AppendChild(mdf.NewLineTrivia());
        }

        public static void NewLine(this MarkdownSyntaxTree md, MarkdownSyntaxFactory mdf) {
            md.AppendChild(mdf.EmptyLine());
        }
    }
}