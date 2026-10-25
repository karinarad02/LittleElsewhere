import zipfile

zip_path = r"C:\Users\radit\Downloads\LittleElsewhere_Unity_Project.zip"
with zipfile.ZipFile(zip_path, 'r') as zip_ref:
    for name in zip_ref.namelist():
        print(name)
