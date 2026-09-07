import AccountSystem;
import dearpygui.dearpygui as dpg;


currentUsernameInput: str = None;
currentPasswordInput: str = None;

def USERNAME_INPUT_FIELD_CALLBACK(sender, appData):
    global currentUsernameInput;
    currentUsernameInput = appData;

def PASSOWRD_INPUT_FIELD_CALLBACK(sender, appData):
    global currentPasswordInput;
    currentPasswordInput = appData;

def ShowLoginResultGuiWindow(resultText: str):
    dpg.set_value("ResultText", resultText);
    dpg.configure_item("ResultWindow", show=True);

def LOGIN_BUTTON_CALLBACK(sender, appData):
    global currentUsernameInput;
    global currentPasswordInput;

    result, message = AccountSystem.LoginUser(currentUsernameInput, currentPasswordInput);

    ShowLoginResultGuiWindow(message);

def LOGOUT_BUTTON_CALLBACK():
    result = AccountSystem.LogoutUser();

    ShowLoginResultGuiWindow("Succesfully Logged Out!" if result else "An Error Occured Logging The User Out, Please Try Again Later!");


def CREATE_USER_BUTTON_CALLBACK(sender, appData):
    global currentUsernameInput;
    global currentPasswordInput;
    
    result, message = AccountSystem.CreateUser(currentUsernameInput, currentPasswordInput);

    ShowLoginResultGuiWindow(message);

def InitDearPyGui():
    dpg.create_context();
    dpg.create_viewport(title="DefaultViewport", width=1920, height=1080);

    with (dpg.window(tag="MainWindow", width=1920, height=1080)):
        dpg.add_input_text(label="username", callback=USERNAME_INPUT_FIELD_CALLBACK);
        dpg.add_input_text(label="password", callback=PASSOWRD_INPUT_FIELD_CALLBACK);

        dpg.add_button(label="Login", callback=LOGIN_BUTTON_CALLBACK);
        dpg.add_button(label="Logout", callback=LOGOUT_BUTTON_CALLBACK);
        dpg.add_button(label="Create New Account", callback=CREATE_USER_BUTTON_CALLBACK);

    with (dpg.window(label="Result Window",tag="ResultWindow" ,width=500, height=500, show=False)):
        dpg.add_text(default_value="", tag="ResultText");

        dpg.add_button(label="OK", callback=lambda: dpg.configure_item("ResultWindow", show=False));

    dpg.setup_dearpygui();
    dpg.show_viewport();
    dpg.set_primary_window("MainWindow", True);
    dpg.start_dearpygui();
    dpg.destroy_context();


def Main():
    InitDearPyGui();

if (__name__ == "__main__"):
    Main();