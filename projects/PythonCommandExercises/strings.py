from pathlib import Path

from multimethod import multimethod

users_dict = {}

def save():
    global users_dict

    with open("saved_users", "w") as save_file:
        save_file.write(str(users_dict))

def load():
    global users_dict
    dic = ''

    if Path("saved_users").exists(): # Only load the save file if it exists
        with open("saved_users", "r") as save_file:
            for i in save_file.readlines():
                dic = i

        dic = eval(dic)
        users_dict = dic
    else:
        print("NO SAVED USERS YET") # Temporary for testing

class UserInformation:
    user_name: str
    user_password: str
    user_money: float

    def __init__(self, name: str, password: str, money: float):
        self.user_name = name
        self.user_password = password
        self.user_money = money

    def deposite_money(self, money_amount: float):
        self.user_money += money_amount

        print("New money amount: ", self.user_money)

    def deplete_money(self, money_amount: float):
        self.user_money -= money_amount

        print("Remaining money: ", self.user_money)
    
    def get_current_money_amount(self) -> float:
        return self.user_money

current_logged_in_user: UserInformation = None

@multimethod
def log_into_user():
    global users_dict
    if len(users_dict) == 0:
        print("There aren't any accounts to log into. Please create a new account(s) to procceed.")

        return

    global current_logged_in_user

    user_name = ""
    user_password = ""

    user_name = input("Please specify the user name: ")
    user_password = input("Please specify the password: ")

    if ((user_name, user_password) in users_dict):
        current_logged_in_user = users_dict[(user_name, user_password)]

        print("User logged in!")
    else:
        print("This user does not exist! Please make a new account or log into an other one.")

@multimethod
def log_into_user(user_name: str, user_password: str):
    if ((user_name, user_password) in users_dict):
            current_logged_in_user = users_dict[(user_name, user_password)]
    
            print("User logged in!")
    else:
        raise Exception("User wasn't created properly so can't login!")

def create_new_user():
    global users_dict
    
    user_information = None
    
    user_name = ""
    user_password = ""
    user_money = ""
        
    user_name = input("user name: ")
    
    user_password = input("user password: ")
    
    user_money = float(input("amount of money: "))
    
    user_information = UserInformation(user_name, user_password, user_money)

    users_dict[(user_name, user_password)] = user_information

    save()
    
    log_into_user(user_name, user_password)

def command_line():
    user_input = ""

    while True:
        user_input = input(
            "Please specify which action you would like to do: \n" +

            "1. Create a new user\n" +
            "2. Log into an existing user\n" +
            "3. Deposite money to a user\n" + 
            "4. Deplete money from a user\n" +
            "5. Check money amount\n\n"  
        )

        user_input = user_input.replace(" ", "")
        user_input = user_input.replace(".", "")
        user_input = user_input.lower()

        if user_input == "1" or user_input == "create a new user":
            create_new_user()
        elif user_input == "2" or user_input == "log into an existing user":
            log_into_user()
        elif user_input == "3" or user_input == "deposite money to a user":
            if (current_logged_in_user):
                current_logged_in_user.deposite_money(float(input("amount: ")))
            else:
                print("You are not logged into an account yet. Please login to unlock this action!")
        elif user_input == "4" or user_input == "deplete money from a user":
            if (current_logged_in_user):
                current_logged_in_user.deplete_money(float(input("amount: ")))
            else:
                print("You are not logged into an account yet. Please login to unlock this action!")
        elif user_input == "5" or user_input == "check money amount":
            if (current_logged_in_user):
                print(current_logged_in_user.get_current_money_amount())
            else:
                print("You are not logged into an account yet. Please login to unlock this action!")

        save()

if __name__ == "__main__":
    load()
    command_line()

# What I learned during this session:

# Honestly not much, this is a little too easy. I already made it harder than necesarry for the assignment, but it's still easy.
# It is a nice little refresher on how the syntax of python works though, there was a lot I had forgoten, so that's at least something (important)

# That you can't easily serialize a dictionary with a tuple as key to a json file (yay)

# TODO:
# Make the accounts save and load properly. (somehow)
# ...

# Working Time: 2 hours