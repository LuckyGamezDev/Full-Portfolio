from pathlib import Path
from re import L
import AccountSystem;
import dearpygui.dearpygui as dpg;
import os;
import socket;
import threading;
from cryptography.fernet import Fernet;

NOTES_KEY_FILE_NAME = "NotesKey.key";
APPLICATION_PATH = r"Z:\Programming\VisualStudio Projects\PY\UserNotesSystem_Online";

HOST = "127.0.0.1";
PORT = 65432;

applicationLogin = None;
mainApplication = None;

class ApplicationLogin:
    def __init__(self):
        self.LOGIN_WINDOW_TAG = "LoginWindow";
        self.RESULT_WINDOW_TAG = "ResultWindow";

        self.currentUsernameInput: str = "";
        self.currentPasswordInput: str = "";

        self.BuildLoginWindowGUI();

    def BuildLoginWindowGUI(self):
        with (dpg.window(label="Login", tag=self.LOGIN_WINDOW_TAG, width=1920, height=1080)):
            dpg.add_input_text(label="username", callback=self.USERNAME_INPUT_FIELD_CALLBACK);
            dpg.add_input_text(label="password", callback=self.PASSOWRD_INPUT_FIELD_CALLBACK, password=True);

            dpg.add_button(label="Login", callback=self.LOGIN_BUTTON_CALLBACK);
            dpg.add_button(label="Logout", callback=self.LOGOUT_BUTTON_CALLBACK);
            dpg.add_button(label="Create New Account", callback=self.CREATE_USER_BUTTON_CALLBACK);

        with (dpg.window(label="Result Window",tag=self.RESULT_WINDOW_TAG ,width=500, height=500, show=False)):
            dpg.add_text(default_value="", tag="ResultText");

            dpg.add_button(label="OK", callback=lambda: dpg.configure_item(self.RESULT_WINDOW_TAG, show=False));


    # Login Window related callbacks/functions.
    def USERNAME_INPUT_FIELD_CALLBACK(self, sender, appData):
        self.currentUsernameInput = appData;

    def PASSOWRD_INPUT_FIELD_CALLBACK(self, sender, appData):
        self.currentPasswordInput = appData;

    def ShowLoginResultGuiWindow(self, resultText: str):
        dpg.set_value("ResultText", resultText);
        dpg.configure_item(self.RESULT_WINDOW_TAG, show=True);

    def LOGIN_BUTTON_CALLBACK(self, sender, appData):
        result, message = AccountSystem.LoginUser(self.currentUsernameInput, self.currentPasswordInput);

        self.ShowLoginResultGuiWindow(message);

        if (result):
            global mainApplication;

            # If the login is succesful, set the main window as the primary, and make it vissible.
            dpg.configure_item(mainApplication.MAIN_APPLICATION_WINDOW_TAG, show=True);
            dpg.set_primary_window(mainApplication.MAIN_APPLICATION_WINDOW_TAG, True);

            mainApplication.allNotes = mainApplication.FindOwnerNotes(mainApplication.allNotes);
            mainApplication.UpdateNoteSelectionGroup();

            # Disable the login screen
            dpg.configure_item(self.LOGIN_WINDOW_TAG, show=False);
        

    def LOGOUT_BUTTON_CALLBACK(self):
        result = AccountSystem.LogoutUser();

        self.ShowLoginResultGuiWindow("Succesfully Logged Out!" if result else "An Error Occured Logging The User Out, Please Try Again Later!");


    def CREATE_USER_BUTTON_CALLBACK(self, sender, appData):
        userAuthenticationKey = Fernet.generate_key();

        result, message = AccountSystem.CreateUser(self.currentUsernameInput, self.currentPasswordInput, userAuthenticationKey);

        self.ShowLoginResultGuiWindow(message);

class Note:
    def __init__(self, name: str, contents: str, directory: str):
        self.name = name;
        self.contents = contents;
        self.directory = directory;
        self.path = self.directory + "\\" + self.name + ".enc";

        with open(self.path, "wb") as file:
            if (isinstance(self.contents, str)):
                self.contents = self.contents.encode();

            file.write(self.contents);

    def DecryptNoteContents(self) -> str:
        fernet = Fernet(AccountSystem.GetCurrentUserAuthenticationKey());

        with open(self.path, "rb") as file:
            encryptedContents = file.read();

        decryptedContents = fernet.decrypt(encryptedContents.decode());

        print(f"Decrypted note succesfully!");

        with open (self.path, "w") as file:
            self.contents = decryptedContents;
            self.contents = self.contents.decode();

            file.write(self.contents);

        return self.contents;

    def SaveNoteAndEncryptContents(self, noteContents):
        fernet = Fernet(AccountSystem.GetCurrentUserAuthenticationKey());

        self.contents = noteContents;

        # Encode the string so that the string is bytes, and not a string.
        encryptedContents = fernet.encrypt(self.contents.encode());

        with open(self.path, "wb") as file:
            file.write(encryptedContents);
            self.contents = encryptedContents;

class MainApplication:
    def __init__(self):
        self.MAIN_APPLICATION_WINDOW_TAG = "MainApplication";
        self.MAIN_APPLICATION_WARNING_WINDOW_TAG = "MainApplicationWarning";
        self.MAIN_APPLICATION_WARNING_WINDOW_MESSAGE_TAG = "MainApplicationWarningMessageText";
        self.NOTE_CREATION_WINDOW_TAG = "NoteCreation";
        self.NOTE_EDITING_WINDOW_TAG = "NoteEditing";

        self.noteNameInputField = None;
        self.noteContentsInputField = None;
        self.currentOpenedNote: Note = None;

        self.allNotes: list = None;
        self.notesGroup = None;

        self.allNotes = self.SearchForExistingNotes();
        self.BuildMainApplicationWindowGUI();
       
    def BuildMainApplicationWindowGUI(self):
        with (dpg.window(label="Main Application", tag=self.MAIN_APPLICATION_WINDOW_TAG, width=1920, height=1080, show=False)):
            dpg.add_button(label="Create Note", callback=self.ShowNoteCreationGuiWindow);

            # Create a group for all existing notes.
            self.notesGroup = dpg.add_group(label="Notes", width=200, height=100);

        with (dpg.window(label="Note Creation", tag=self.NOTE_CREATION_WINDOW_TAG, width=500, height=500, show=False)):
            self.noteNameInputField = dpg.add_input_text(label="Name");
            dpg.add_button(label="Create Note", callback=self.CreateNewNote);

        with (dpg.window(label="Note", tag=self.NOTE_EDITING_WINDOW_TAG, width=1920, height=1080, show=False)):
            self.noteContentsInputField = dpg.add_input_text(width=1900, height=900, multiline=True);
            dpg.add_button(label="Close", callback=self.CloseAndSaveCurrentlyOpenedNote);
            dpg.add_button(label="Delete Note", callback=self.DeleteCurrentlyOpenedNote);

        with (dpg.window(label="Warning", tag=self.MAIN_APPLICATION_WARNING_WINDOW_TAG, width=400, height=500, show=False)):
            dpg.add_text(default_value="Warning message", tag=self.MAIN_APPLICATION_WARNING_WINDOW_MESSAGE_TAG);

            dpg.add_button(label="Ok", callback=lambda: dpg.configure_item(self.MAIN_APPLICATION_WARNING_WINDOW_TAG, show=False));


    def CreateNewNote(self, sender, appData, userData: str):
        noteName = dpg.get_value(self.noteNameInputField);

        if (noteName == ""):
            self.ShowWarningGuiWindow("The note name must be declared before creation!");
            return;

        print("Note name:" + noteName);
        note = Note(noteName, "", APPLICATION_PATH);

        # Open the note editing window
        self.ShowNoteEditingGuiWindow();

        # Close the note creation window
        dpg.configure_item(self.NOTE_CREATION_WINDOW_TAG, show=False);

        # Assign the current opened note to this newly created one.
        self.currentOpenedNote = note;

        self.AddNoteToNotesList(note);

        return note;     

    def AddNoteToNotesList(self, note: Note):
        self.allNotes.append(note);
        dpg.add_button(label=note.name, parent=self.notesGroup, user_data=note, callback=self.OpenNote);
    
    def OpenNote(self, sender, appData, userData):
        if (isinstance(userData, Note)):
            note = userData;
        else:
            print(f"The passed in userData wasn't of type 'Note'!, it was {type(userData)}");
            
            return;

        self.currentOpenedNote = note;

        note.DecryptNoteContents();

        self.ShowNoteEditingGuiWindow(note.contents);

    def CloseAndSaveCurrentlyOpenedNote(self, sender, appData):
        self.currentOpenedNote.SaveNoteAndEncryptContents(dpg.get_value(self.noteContentsInputField));

        # Reset the current opened note to be none.
        self.currentOpenedNote = None;

        # Handle the UI
        dpg.configure_item(self.NOTE_EDITING_WINDOW_TAG, show=False);

    def CloseCurrentlyOpenedNote(self):
        # Reset the current opened note to be none.
        self.currentOpenedNote = None;

        # Handle the UI
        dpg.configure_item(self.NOTE_EDITING_WINDOW_TAG, show=False);

    def DeleteCurrentlyOpenedNote(self, sender, appData):
        if (self.currentOpenedNote is not None):
            os.remove(self.currentOpenedNote.path);
        
            # Remove the note from the notes list.
            self.allNotes.remove(self.currentOpenedNote);

            self.UpdateNoteSelectionGroup();

            self.CloseCurrentlyOpenedNote();

            print("Note succesfully deleted!");
        else:
            print("No note to delete, there is no note opened!");

            return;

    def SearchForExistingNotes(self) -> list:
        notes = [];
        for name in os.listdir(APPLICATION_PATH):
            if (name.lower().endswith(".enc")):
                with open(name, "rb") as file:
                    contents = file.read();

                note = Note(os.path.splitext(name) [0], contents, APPLICATION_PATH);
                notes.append(note);

        return notes;

    def FindOwnerNotes(self, notesList: list[Note]) -> list[Note]:
        ownerNotes = [];

        for note in notesList:
            try:
                decryptedContents = note.DecryptNoteContents();
                note.SaveNoteAndEncryptContents(decryptedContents);

                ownerNotes.append(note);

                print("Found 1 owner note, note added to list!");
            except Exception as e:
                print(f"Found a note not from the owner, didn't add it to the list!, the exception was: {e}");

        return ownerNotes;

    def UpdateNoteSelectionGroup(self):
        dpg.delete_item(self.notesGroup, children_only=True);

        # Re-add all notes
        for note in self.allNotes:
            dpg.add_button(label=note.name, parent=self.notesGroup, user_data=note, callback=self.OpenNote);

    def ShowNoteCreationGuiWindow(self):
        dpg.configure_item(self.NOTE_CREATION_WINDOW_TAG, show=True);

    def ShowNoteEditingGuiWindow(self, noteContents: str = ""):
        dpg.configure_item(self.NOTE_EDITING_WINDOW_TAG, show=True);
        dpg.set_value(self.noteContentsInputField, noteContents);

    def ShowWarningGuiWindow(self, message: str):
        dpg.configure_item(self.MAIN_APPLICATION_WARNING_WINDOW_TAG, show=True);
        dpg.set_value(self.MAIN_APPLICATION_WARNING_WINDOW_MESSAGE_TAG, message);

def ON_APPLICATION_EXITED_CALLBACK():
    # If a note is still opened, avoid corruption by closing it safely.
    if (mainApplication.currentOpenedNote is not None):
        mainApplication.CloseAndSaveCurrentlyOpenedNote(None, None);

def InitDearPyGui():
    dpg.create_context();
    dpg.create_viewport(title="User Notes System", width=1920, height=1080);

    dpg.setup_dearpygui();
    dpg.show_viewport();

    global applicationLogin;
    applicationLogin = ApplicationLogin();

    global mainApplication;
    mainApplication = MainApplication();

    dpg.set_primary_window(applicationLogin.LOGIN_WINDOW_TAG, True);
    dpg.set_exit_callback(callback=ON_APPLICATION_EXITED_CALLBACK);
    dpg.start_dearpygui();
    dpg.destroy_context();
  
def InitNetworking():
    # Init the server
    with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
        s.bind((HOST, PORT));
        s.listen();
        connection, address = s.accept();

        with connection:
            print(f"Host is connected with {connection}!");

    # Init the client
    with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
        s.connect((HOST, PORT));

def Main():
    networkingThread = threading.Thread(target=InitNetworking);
    networkingThread.start();
    InitDearPyGui();

if (__name__ == "__main__"):
    Main();