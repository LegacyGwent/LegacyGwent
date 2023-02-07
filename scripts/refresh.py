import os
import shutil
import subprocess

def build_dll(path: str):
    print("run dotnet build")
    project_path = os.path.join(path, "src", "Cynthia.Card", "src", "Cynthia.Card.Server", "Cynthia.Card.Server.csproj")
    print(project_path)
    subprocess.run(["dotnet", "build", project_path])

def copy_dll(path: str):
    print("Copy dll")
    dll_path = os.path.join(path, "src", "Cynthia.Card", "src", "Cynthia.Card.Common", "bin", "Debug", "netstandard2.0", "Cynthia.Card.Common.dll")
    to_path = os.path.join(path, "src", "Cynthia.Card.Unity", "src", "Cynthia.Unity.Card", "Assets", "Assemblies", "Cynthia.Card.Common.dll")
    print("From: " + dll_path)
    print("To: " + to_path)
    shutil.copyfile(dll_path, to_path)

def main():
    path = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
    build_dll(path)
    copy_dll(path)
    print("Done!")



if __name__ == "__main__":
    main()
