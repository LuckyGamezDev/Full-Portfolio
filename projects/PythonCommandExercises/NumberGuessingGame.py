import random

import time

user_numbers_set = set()
opponent_numbers_set = set()

max_valid_number_range = 50

is_user_turn = True

def set_game_valid_number_range():
    global max_valid_number_range

    user_number: int

    while True:
        user_response = input("please specify the max range you want to play the game in (eg. inputting '50' means the range will be 0-50) ")

        try:
            user_number = int(user_response)

            break
        except:
            print("This is not a valid number, please input a numeral!")
        
            continue

    max_valid_number_range = user_number

def wants_rematch() -> bool:
    while True:
        # Add a play again functionality
        user_response = input("Play again? (y/n) ")                
    
        user_response = user_response.lower()
        if user_response == "y":
            return True
        elif user_response == "n":
            return False
        else:
            print("This is not a valid response, please respond by either inputting 'y' or 'n'!")
    

def reset_game():
    global user_numbers_set
    global opponent_numbers_set
    global is_user_turn

    user_numbers_set.clear()
    opponent_numbers_set.clear()

    is_user_turn = True

    while True:
        user_response = input("Do you want to specify a custom range of where the game should take place in (eg. 0-50)? If not, the default will be 0-50. (y/n) ")

        user_response = user_response.lower()
        if user_response == "y":
            set_game_valid_number_range()           

            break
        elif user_response == "n":
            break
        else:
            print("This is not a valid response, please respond by either inputting 'y' or 'n'!")

def main():
    global user_numbers_set
    global opponent_numbers_set
    global is_user_turn

    global max_valid_number_range

    # Make the user able to choose from various options as to how big the range of numbers are
    while True:
        user_response = input("Do you want to specify a custom range of where the game should take place in (eg. 0-50)? If not, the default will be 0-50. (y/n) ")

        user_response = user_response.lower()
        if user_response == "y":
            set_game_valid_number_range()           

            break
        elif user_response == "n":
            break
        else:
            print("This is not a valid response, please respond by either inputting 'y' or 'n'!")

    while True:
        if is_user_turn:
            if len(user_numbers_set) > 0:
                print("Aready chosen user numbers: ", user_numbers_set)

            # Make the user able to input a number
            user_input_number = input(f"Type a number from 0-{max_valid_number_range} ")

            user_number: int
            try:
                user_number = int(user_input_number)
            except:
                print("This is not a valid number, please input a numeral!")

                continue

            if user_number > max_valid_number_range:
                print("This value is too big, please specify a smaller number that is within the specified range!")

                continue
            elif user_number < 0:
                print("A value smaller than '0' is not valid. Please input a number that is greater/equal to '0'!")

                continue

            if user_number in user_numbers_set:
                print("You already picked this number in a previous turn, please try an other one!")

                continue

            user_numbers_set.add(int(user_number))

            if set(user_numbers_set) & set(opponent_numbers_set):
                print("You lost")

                if wants_rematch():
                    reset_game()
                else:
                    return

            # Reset the turn
            is_user_turn = False
        else:
            while True:
                print("Opponent picking number...")

                random_number = random.randrange(0, max_valid_number_range + 1) # +1 since the max value is not inclusive

                if random_number in opponent_numbers_set:
                    continue
                
                break
            
            opponent_numbers_set.add(random_number)

            time.sleep(0.3)

            print("Opponent picked number.")

            time.sleep(0.2) # Add a second delay to make it feel a little more tactical instead

            # Check if the opponent number is available
            if set(user_numbers_set) & set(opponent_numbers_set):
 
                print("You won!")

                if wants_rematch():
                    reset_game()
                else:
                    return

            is_user_turn = True

if __name__ == "__main__":
    main()

# What I learned:
# What exactly sets are. By reading what sets really are, I was able to figure out what I could make with them. I came up with a simple back and forth game that makes you try
# to not pick the number the opponent has picked. I can't really think of a use case where sets would be used elsewhere, but I'm sure ther are plenty!

# Working time: 1 hour