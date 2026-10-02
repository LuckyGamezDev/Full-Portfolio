import csv

import os

program_commands = ["--help", "--invalid-records"]

shouldRunProgram = True    

invalid_records_list = []

def run_command(command: str):
    global program_commands
    global invalid_records_list

    if command == program_commands[0]: # Help
        print(
            """
            Description: Parses a specific-formatted CSV file and exports a cleaned up version of it with an optional summary
            Usage: Type '--' + the command name to run a command.
            Commands: '--help', '--invalid-records'
            """
        )
    elif command == program_commands[1]: # Invalid records
        if len(invalid_records_list) == 0:
            print("You haven't yet parsed any file(s) that had invalid records.")

            return

        print("Here are the invalid records of the most recently parsed CSV file that had invalid records: ")
        for invalid_record in invalid_records_list:
            print(invalid_record)
    else:
        print("This is not a valid command!")

def export_clean_csv(file_name, records_list: dict):
    with open(file_name, "w") as csv_file:
        print("Writing to file...")

        try:
            writer = csv.DictWriter(csv_file, fieldnames=["name", "gender", "age", "school_name", "country", "pet"], restval="unknown")

            writer.writeheader()
            writer.writerows(records_list)

            for record_dict in records_list:
                writer.writerow(record_dict)

        except Exception as ex:
            print("The following exception occured: ", ex)

        print("Wrote records successfully to file!")

def write_csv_summary(file_name) -> tuple:
    with open(file_name, "r") as csv_file:
        reader = csv.DictReader(csv_file)

        same_school_occurrences = 0
        same_age_occurrences = 0
        male_gender_occurrences = 0
        female_gender_occurrences = 0
        same_country_occurrences = 0

        # Save all the dictionaries to a list
        record_dicts_list = []
        for record in reader:
            record_dicts_list.append(record)

        for index, record in enumerate(record_dicts_list):
            if len(record_dicts_list) >= index + 2: # + 2 because you need to access one in front and make the index equal to the length
                record_to_compare = record_dicts_list[index + 1]
            else:
                continue

            if record["gender"] == "male":
                male_gender_occurrences += 1
            elif record["gender"] == "female":
                female_gender_occurrences += 1

            if record["age"] == record_to_compare["age"]:
                same_age_occurrences += 2 # Count for two since it's a matching pair

            if record["school_name"] == record_to_compare["school_name"]:
                same_school_occurrences += 2 # Count for two since it's a matching pair

            if record["country"] == record_to_compare["country"]:
                same_country_occurrences += 2 # Count for two since it's a matching pair

        summaries_list = []
        summaries_list.append(same_school_occurrences)
        summaries_list.append(same_age_occurrences)
        summaries_list.append(male_gender_occurrences)
        summaries_list.append(female_gender_occurrences)
        summaries_list.append(same_country_occurrences)

        summary_string = ( 
            f"There were/was a total of {male_gender_occurrences} male(s) and {female_gender_occurrences} female(s). There were {same_age_occurrences} people of the same age, "
            f"{same_school_occurrences} people who go to the same school, and {same_country_occurrences} people living in the same country."
        )
        

        return (summary_string, summaries_list)
     

def validate_csv_file_contents(file_name: str) -> list:
    records_list = []
    found_invalid_records_list = []

    with open(file_name, "r") as csv_file:
        reader = csv.DictReader(csv_file)

        for record in reader:
            print(record)

            if any(char.isdigit() for char in record["name"]):
                # Save the invalid record in a seperate list so that the user can see which record were faulty later
                found_invalid_records_list.append(record)

                # Continue so that the record doesn't get added to the list and thus gets excluded
                continue

            records_list.append(record)

    print("All the added records:")
    for record_dict in records_list:
        print(record_dict)

    if len(found_invalid_records_list) > 0:
        print(
            "This CSV file contains one or more invalid records, this could be because of several reasons. " +
            "To find out which records were found, you can use the '--invalid-records' command to see all the invalid ones. " +
            "These records won't be included in the summary(s), nor in the final export file."
        )

        # Assign the invalid records list to the global variable for possible later inspection by the user
        global invalid_records_list
        invalid_records_list = found_invalid_records_list

    return records_list

def main():
    print("Welcome to CSV Parser.\n")

    while shouldRunProgram:
        user_input = input("To get started please specify a program command or input the path of the file you want to parse: ")

        if user_input.startswith('--'):
            run_command(user_input)

            continue # Skip to the next loop to avoid file handling
        else:
            filePath = user_input

        if filePath is None or filePath == "" or not os.path.exists(filePath):
            print("This is not a valid file path, please specify an other one!")

            continue

        validated_csv_list = validate_csv_file_contents(filePath)

        export_clean_csv("exported_file.csv", validated_csv_list)
        summaries_tuple = write_csv_summary(filePath)

        while True:
            user_responds = input("Would you like to view a short summary of the exported CSV file? (y/n)")
            user_responds = user_responds.lower()

            if user_responds == "y":
                print(summaries_tuple[0])
                break
            elif user_responds == "n":
                break
            else:
                print("This is not a valid user respondse, please answer with 'y' or 'n'.")

if __name__ == "__main__":
    main()

# TODO:
# Fix error with the last record in a file 👍

# Add a summary of the CSV record(s) 👍

# Add a help command line command and just some other command line commands in general 👍

# ...


# What I've learned this session:
# Not really anything important

# Time worked: 4.5 hours