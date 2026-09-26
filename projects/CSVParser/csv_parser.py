import csv

import os

shouldRunProgram = True     

invalid_records_list = []

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

def write_csv_summary(file_name):
    with open(file_name, "r") as csv_file:
        reader = csv.DictReader(csv_file)

        same_school_occurrences = -1
        same_age_occurrences = -1
        male_gender_occurrences = -1
        female_gender_occurrences = -1
        same_country_occurrences = -1

        # Save all the dictionaries to a list
        record_dict_list = []
        for record in reader:
            record_dict_list.append(record)

        

def validate_csv_file_contents(file_name: str) -> list:
    global invalid_records_list

    records_list = []

    with open(file_name, "r") as csv_file:
        reader = csv.DictReader(csv_file)

        for record in reader:
            print(record)

            if any(char.isdigit() for char in record["name"]):
                # Save the invalid record in a seperate list so that the user can see which record were faulty later
                invalid_records_list.append(record)

                # Continue so that the record doesn't get added to the list and thus gets excluded
                continue

            records_list.append(record)

    print("All the added records:")
    for record_dict in records_list:
        print(record_dict)

    if len(invalid_records_list) > 0:
        print(
            "This CSV file contains one or more invalid records, this could be because of several reasons. " +
            "To find out which records were found, you can use the '--invalid-record' command to see all the invalid ones. " +
            "These records won't be included in the summary(s), nor in the final export file."
        )

    return records_list

def reset_values():
    global invalid_records_list
    invalid_records_list.clear()

def main():
    print("Welcome to CSV Parser.\n")

    while shouldRunProgram:
        # Reset all values
        reset_values()

        filePath = input("To get started please specify the path of the file you want to parse: ")

        if filePath is None or filePath == "" or not os.path.exists(filePath):
            print("This is not a valid file path, please specify an other one!")

            continue

        validated_csv_list = validate_csv_file_contents(filePath)

        export_clean_csv("exported_file.csv", validated_csv_list)

if __name__ == "__main__":
    main()

# TODO:
# Fix error with the last record in a file 👍

# Add a summary of the CSV record(s)

# ...


# What I've learned this session:
# How the actual reader/write from CSV works and finally getting that I should be saving each record to a list since one record is an entire dictionary. I got to know
# with the help of AI. Anyway, I basically didn't do anything, just fixed the problem with saving and such. The main system works, now only the summary is left. For the record
# so you know how bad I genuinely currently am: this was 1 hour of time, I basically just was staring at the code an hoped it'd fix it by itself. Motivation currently is basically non-existent.

# Time worked: 3.5 hours