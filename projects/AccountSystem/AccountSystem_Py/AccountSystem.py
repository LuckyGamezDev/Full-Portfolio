import _sqlite3
import bcrypt;

import dearpygui.dearpygui as dpg;


DATABASE_FILE = "users.db";

currentUsernameInput: str = None;
currentPasswordInput: str = None;

isUserLoggedIn = False;

# Create the context for dearpygui
dpg.create_context();

# Initialize sql
connection = _sqlite3.connect(DATABASE_FILE);
cursor = connection.cursor();
cursor.execute("CREATE TABLE IF NOT EXISTS user(username TEXT UNIQUE, password TEXT)");
connection.close();


def USERNAME_INPUT_FIELD_CALLBACK(sender, appData, userData=None):
    global currentUsernameInput;
    currentUsernameInput = appData;

def PASSWORD_INPUT_FIELD_CALLBACK(sender, appData, userData=None):
    global currentPasswordInput
    currentPasswordInput = appData;


def LOGIN_BUTTON_CALLBACK(sender, appData, userData=None):
    result = LoginUser(currentUsernameInput, currentPasswordInput);

    with (dpg.window(label="LoginResult", width=500, height=500)):
        textString = "Login succesful!" if result else "Login failed!";
        dpg.add_text(label="LoginResultText", default_value=textString);

def LOGOUT_BUTTON_CALLBACK(sender, appData, userData=None):
    LogoutUser();

def CREATE_USER_BUTTON_CALLBACK(sender, appData, userData=None):
    CreateUser(currentUsernameInput, currentPasswordInput);


def CreateUser(username: str, password: str):
    # Create a new connection to avoid having the connection over different threads
    connection = _sqlite3.connect(DATABASE_FILE);
    cursor = connection.cursor();

    passwordInBytes = password.encode("utf-8");
    passwordSalt = bcrypt.gensalt();
    hashCode = bcrypt.hashpw(passwordInBytes, passwordSalt);

    # Add the user to the sql table
    accountDetails = (username, hashCode);
    cursor.execute("INSERT OR IGNORE INTO user VALUES (?, ?)", accountDetails);
    connection.commit();

    # Close the connection to avoid having this connection used on a different thread
    connection.close();

def LoginUser(username: str, password: str) -> bool:
    global isUserLoggedIn
    if (isUserLoggedIn):
        with (dpg.window(label="LoginResult", width=500, height=500)):
            dpg.add_text(label="LoginResultText", default_value="User is already logged in!");

        return False;

    # Create a new connection to avoid having the connection over different threads
    connection = _sqlite3.connect(DATABASE_FILE);
    cursor = connection.cursor();

    hashCursor = cursor.execute("SELECT password FROM user WHERE username = ?", (username,));
    hashCodesList = hashCursor.fetchall();

    userPasswordToBytes = password.encode("utf-8");

    result = False;
    for hashCode in hashCodesList:
        result = bcrypt.checkpw(userPasswordToBytes, *hashCode);

        if (result == True):
            break;

    # Close the connection to avoid having this connection used on a different thread
    connection.close();

    if result:
        print("User logged in!");

        isUserLoggedIn = True;

        return True;
    else:
        print ("Invalid log in!");

        isUserLoggedIn = False;

        return False;

def LogoutUser():
    global isUserLoggedIn;

    if (not isUserLoggedIn): return;

    isUserLoggedIn = False

    with (dpg.window(label="LoginResult", width=500, height=500)):
        dpg.add_text(label="LoginResultText", default_value="Logout succesful!");

def InitializeDPG():
    with dpg.window(tag="PrimaryWindow"):
        dpg.add_input_text(label="Username", default_value="Input username...", callback=USERNAME_INPUT_FIELD_CALLBACK);
        dpg.add_input_text(label="Password", default_value="Input password...", callback=PASSWORD_INPUT_FIELD_CALLBACK);

        dpg.add_button(label="Login", callback=LOGIN_BUTTON_CALLBACK, user_data="Some data");
        dpg.add_button(label="Logout", callback=LOGOUT_BUTTON_CALLBACK);
        dpg.add_button(label="Create account", callback=CREATE_USER_BUTTON_CALLBACK);

    dpg.create_viewport(title="AccountSystem", width=1920, height=1080);
    dpg.setup_dearpygui();
    dpg.show_viewport();
    dpg.set_primary_window("PrimaryWindow", True);
    dpg.start_dearpygui();
    dpg.destroy_context();

def Main():
    InitializeDPG();

if (__name__ == "__main__"):
    Main();