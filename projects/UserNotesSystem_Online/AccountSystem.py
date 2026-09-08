import _sqlite3
import sqlite3
import bcrypt;

import dearpygui.dearpygui as dpg;


DATABASE_FILE = "users.db";

currentLoggedInUsername: str = "";
isUserLoggedIn = False;

# Create the context for dearpygui
dpg.create_context();

# Initialize sql
connection = _sqlite3.connect(DATABASE_FILE);
cursor = connection.cursor();
cursor.execute("CREATE TABLE IF NOT EXISTS user(username TEXT UNIQUE, password TEXT, authenticationKey TEXT)");
connection.close();

def CreateUser(username: str, password: str, uniqueUserAuthenticationKey) -> tuple[bool, str]:
    # Create a new connection to avoid having the connection over different threads
    connection = _sqlite3.connect(DATABASE_FILE);
    cursor = connection.cursor();

    passwordInBytes = password.encode("utf-8");
    passwordSalt = bcrypt.gensalt();
    hashCode = bcrypt.hashpw(passwordInBytes, passwordSalt);

    # Add the user to the sql table
    accountDetails = (username, hashCode, uniqueUserAuthenticationKey);

    try:
        cursor.execute("INSERT INTO user VALUES (?, ?, ?)", accountDetails);
        connection.commit();
    except:
        return (False, "Unable To Login, The Username Already Taken!");

    # Close the connection to avoid having this connection used on a different thread
    connection.close();

    return (True, "Succesfully Created Account!");

def LoginUser(username: str, password: str) -> tuple[bool, str]:
    global isUserLoggedIn
    if (isUserLoggedIn):
        return (False, "User Is Already Logged In!");

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

        global currentLoggedInUsername;
        currentLoggedInUsername = username;

        return (True, "Login Succesful!");
    else:
        print ("Invalid log in!");

        isUserLoggedIn = False;

        return (False, "Invalid Credentials!");

def LogoutUser() -> bool:
    global isUserLoggedIn;

    if (not isUserLoggedIn): return False;

    isUserLoggedIn = False

    # Remove the username from the current logged in user variable
    currentLoggedInUsername = "";

    return True;

def GetCurrentUserPasswordHash() -> str:
    if (isUserLoggedIn):
        connection = _sqlite3.connect(DATABASE_FILE);
        cursor = connection.cursor();

        global currentLoggedInUsername;
        cursor.execute("SELECT password FROM user WHERE username = ?", (currentLoggedInUsername, ));
        hashedPassword = cursor.fetchone();
        
        connection.close();

        print(f"got the password: {hashedPassword[0]}");

        return hashedPassword[0];
    else:
        print("Can't return password! User isn't logged in.");

        return "";

def GetCurrentUserAuthenticationKey() -> str:
    connection = _sqlite3.connect(DATABASE_FILE);
    cursor = connection.cursor();

    global currentLoggedInUsername;
    cursor.execute("SELECT authenticationKey FROM user WHERE username = ?", (currentLoggedInUsername,));
    authenticationKey = cursor.fetchone();

    connection.close();

    authenticationKeyToString = authenticationKey [0];
    print(f"Succesfully got the authentication key: " + authenticationKeyToString.decode());

    return authenticationKey[0];