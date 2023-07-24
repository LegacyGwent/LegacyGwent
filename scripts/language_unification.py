# -*- coding: utf-8 -*-
"""
@author: Kajtek

1) LegacyGwent\src\Cynthia.Card.Unity\src\Cynthia.Unity.Card\Assets\Resources\Locales
2) LegacyGwent\src\Cynthia.Card.Unity\src\Cynthia.Unity.Card\Assets\StreamingFile\Locales
3) LegacyGwent\src\Cynthia.Card\src\Cynthia.Card.Server\Locales
4) LegacyGwent\src\Cynthia.Card\src\Cynthia.Card.Server\bin\Debug\netcoreapp3.0\Locales

"""
import os
import shutil



path = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
languages=['cn','en','pl','ru']
for language in languages:
    print(language)
    dll_path = os.path.join(path, "src", "Cynthia.Card", "src", "Cynthia.Card.Server","Locales", language+'.json')
    
    to_path = os.path.join(path, "src", "Cynthia.Card.Unity", "src", "Cynthia.Unity.Card", "Assets", "Resources", "Locales", language+'.json')
    shutil.copyfile(dll_path, to_path)
    
    to_path = os.path.join(path, "src", "Cynthia.Card.Unity", "src", "Cynthia.Unity.Card", "Assets", "StreamingFile", "Locales", language+'.json')
    shutil.copyfile(dll_path, to_path)
    
    to_path = os.path.join(path, "src", "Cynthia.Card", "src", "Cynthia.Card.Server", "bin", "Debug", "netcoreapp3.0","locales", language+'.json')
    shutil.copyfile(dll_path, to_path)