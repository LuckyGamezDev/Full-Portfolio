import csv

shouldRunProgram = True

def read_and_format_csv_file_as_dict(file) -> dict:
    with open(file, "r") as csv_file:
        csv_file_dict = csv.DictReader(csv_file)

        # Show the user what the file contents are
        print("CSV file contents:\n")

        for record in csv_file_dict:
            print(record)  

        # Convert the file contents into a dictionary

        validated_csv_file_dict = validate_csv_file_contents(csv_file_dict)

        


invalid_records_list_in_csv: list = []
def validate_csv_file_contents(file_dict: dict) -> dict:
    invalid_records_list = []

    for file in file_dict:
        if any(char.isdigit() for char in file.name):
            invalid_records_list.append(file)
            print("Invalid record: ", file)

            continue

    if len(invalid_records_list) > 0:
        print(
            "This CSV file contains one or more invalid records, this could be because of several reasons." +
            "To find out which records were found, you can use the '--invalid-record' command to see all the invalid ones." +
            "These records won't be included in the summary(s), nor in the final export file."
        )

        global invalid_records_list_in_csv
        invalid_records_list_in_csv = invalid_records_list

    return file_dict

def reset_values():
    global invalid_records_list_in_csv
    invalid_records_list_in_csv.clear()

def main():
    print("Welcome to CSV Parser.\n")

    while shouldRunProgram:
        # Reset all values
        reset_values()

        filePath = input("To get started please specify the path of the file you want to parse: ")

        csv_file = read_and_format_csv_file_as_dict("random_invalid.csv")

        with open("exported_file", "w") as final_csv_file:
            writer = csv.DictWriter(final_csv_file, fieldnames=["name", "gender", "age", "school_name", "city", "pet"])
            writer.writeheader()
            writer.writerows(csv_file)

if __name__ == "__main__":
    main()

# What I've learned this session:
# It's my first time working with CSV, so my intial idea of working with this concept wasn't working. I was planning to directly read the csv file like any ordinary file, and then
# just format it myself. Problem with that is that it doesn't work on all CSV files since CSV stupidely doesn't have a strict syntax. Turns out this assignment specifically
# wants me to only read this specific CSV file, so that didn't matter after all, but I used the official CSV parser implementation library anyway. I'm facing a bit of a confusing
# problem with the DictWriter, though. Apperantly I can't assign a variable to the output of the dict without it throwing an error after I'm trying to access that assigned value
# when the DictWriter is closed. I'm not really happy with this entire code anyway, so I might need to refactor it quite a bit tommorrow.

# Time worked: 1.3 hours