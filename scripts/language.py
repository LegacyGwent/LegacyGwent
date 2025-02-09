# -*- coding: utf-8 -*-
r"""
@author: Kajtek

1) LegacyGwent\src\Cynthia.Card.Unity\src\Cynthia.Unity.Card\Assets\Resources\Locales
2) LegacyGwent\src\Cynthia.Card.Unity\src\Cynthia.Unity.Card\Assets\StreamingFile\Locales
3) LegacyGwent\src\Cynthia.Card\src\Cynthia.Card.Server\Locales
4) LegacyGwent\src\Cynthia.Card\src\Cynthia.Card.Server\bin\Debug\netcoreapp3.0\Locales

"""
import pathlib
import shutil
import json


def unify_language(path,languages):
    for language in languages:
        from_path = pathlib.PurePath(path, "src", "Cynthia.Card", "src", "Cynthia.Card.Server","Locales", language+'.json')
        
        to_path = pathlib.PurePath(path, "src", "Cynthia.Card.Unity", "src", "Cynthia.Unity.Card", "Assets", "Resources", "Locales", language+'.json')
        shutil.copyfile(from_path, to_path)
        
        to_path = pathlib.PurePath(path, "src", "Cynthia.Card.Unity", "src", "Cynthia.Unity.Card", "Assets", "StreamingFile", "Locales", language+'.json')
        shutil.copyfile(from_path, to_path)
        try:
            to_path = pathlib.PurePath(path, "src", "Cynthia.Card", "src", "Cynthia.Card.Server", "bin", "Debug", "netcoreapp3.0","locales", language+'.json')
            shutil.copyfile(from_path, to_path)
            print(language +' unified')
        except:
            print('unity not build '+language)
    
def sort(path,languages):
    for language in languages:
        full_path= pathlib.PurePath(path, "src", "Cynthia.Card", "src", "Cynthia.Card.Server","Locales", language+'.json')
        with open(full_path,encoding="utf-8") as file:
            source=json.load(file)
        cards=source['CardLocales']
        ids = [int(x) for x in list(cards.keys())]
        ids.sort()
        ids = [x for x in ids if x<130000]
        ids = [ str(x) for x in ids]
        sorted_cards = {i: cards[i] for i in ids}
        source['CardLocales']=sorted_cards
        with open(full_path, 'w',encoding="utf-8") as outfile:
            json.dump(source, outfile,indent=4, ensure_ascii=False)
        print(language +' sorted')
        
def main():
    languages=['cn','pl','ru','en']
    
    path = pathlib.Path(__file__).parents[1]
    print('location: '+str(path))
    
    sort(path,languages)
    unify_language(path,languages)


if __name__ == "__main__":
    main()
