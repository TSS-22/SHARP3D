import os
import subprocess

# Path to the script you want to run in each subfolder
basic_data_script = "BasicTestData.py"  # Replace with your script name

# Path to the parent folder
parent_folder = "./../SampleFiles"  # Replace with your folder path

# Create a file to store the exceptions and the reason
with open("basic_test_exceptions.txt", "w"):
    pass

for root, dirs, files in os.walk(parent_folder):
    for subfolder in dirs:
        subfolder_path = os.path.join(root, subfolder)
        print(f"Running basic data script in: {subfolder_path}")

        # Run the script in the subfolder
        subprocess.run(["python", basic_data_script, subfolder_path])
