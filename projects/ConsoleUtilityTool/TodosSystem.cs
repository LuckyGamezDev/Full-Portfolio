using System;
using MainProgram;

namespace Todos {

    public static class TodosSystem {

        public static Todo CreateNewTodoFromUserInput() {
            Console.WriteLine("To create a new todo, please fill in the following information: ");

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
            todosList[todoIndex] = replacementTodo; // Overwrite the old todo with the newly, edited todo
        }

        public static void RemoveTodo(int todoIndex) {
            todosList.RemoveAt(todoIndex);
        }

        public static List<Todo> GetTodosList() {
            return new List<Todo>(todosList);
        }

        public static void ExportTodosToFile() {
            List<Todo> sortedTodosList = GetTodosListSortInOrder();

            // Export this as an organized file
        }

        private static List<Todo> GetTodosListSortInOrder() {
            // Do a loop to order all the todos in the right order
            List<Todo> todosInOrderList = new List<Todo>();
            Todo? mostPriorityTodo = null;
            int todosAddedIndex = 0;
            while (todosAddedIndex <= todosList.Count) {
                foreach (Todo todo in todosList) {
                    if (mostPriorityTodo == null) mostPriorityTodo = todo;

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
                new Todo("first todo", "first todo YAY", ["27", "09", "2010"], Todo.Importancy.High),
                new Todo("second todo", "second todo", ["28", "09", "2010"], Todo.Importancy.High),
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
}
