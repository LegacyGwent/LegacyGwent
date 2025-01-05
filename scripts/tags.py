import re
import json
import pandas as pd
import pathlib
import shutil

def filtr(good_cards,key,values):
    for line in content:
        if 'CardId' in line:
            currentID=line.split('"')[1]
        if key.lower() in line.lower():
            isthere=False
            for el in values:
                if el.lower() in line.lower():
                    isthere=True
                    break
            if not isthere:
                good_cards[currentID]=False
    return good_cards

def filtrmany(conditions):
    good_cards={}
    for line in content:
        if 'CardId' in line:
            ID=line.split('"')[1]
            if ID in ['33004',"63002","23001","43001","53001"]:#Remove agents
                good_cards[ID]=False
            else:
                good_cards[ID]=True
    for condition in conditions:
        good_cards=filtr(good_cards, condition[0], condition[1])
    return good_cards
def dict_to_lst(dictionary):
    lst=[]
    for key in dictionary:
        if dictionary[key]:
            lst.append(key)
    return lst
def lst_to_string(lst):
    result='"'
    for el in lst:
        result=result+el+'","'
    return result[:-2]
def linked(ID):
    read=False
    for line in content:
        if ID in line and "CardId =" in line:
            read=True
        if read:
            if 'LinkedCards' in line:
                return line.split('{')[1].split('}')[0]
        

path = pathlib.Path(__file__).parents[1]
path=pathlib.PurePath(path, "src", "Cynthia.Card", "src", "Cynthia.Card.Common","GwentGame","GwentMap.cs")
with open(path, 'r', encoding='utf-8') as file:
     content = file.readlines()
     
for idx in range(len(content)):
    if '以下是自动导入的代码' in content[idx]:
        cutpoint=idx
        break
    
        
content=content[cutpoint:]


        
        
cards=[\
#Aguara: True Form
['12030',[['isderive',['false']],['group',['Copper','silver']],['categorie',['spell']]]],\
#Black Blood
['13023',[['isderive',['false']],['group',['Copper']],['categorie',['necrophage','vampire']]]],\
#Whispering Hillock
['21003',[['isderive',['false']],['group',['Copper','silver']],['categorie',['organic']]]],\
#Eredin Bréacc Glas
['21004',[['isderive',['false']],['group',['Copper',]],['categorie',['WildHunt']]]],\
#Monster Nest
['23021',[['isderive',['false']],['group',['Copper']],['categorie',['Necrophage','Insectoid']]]],\
#Usurper
['31004',[['isderive',['false']],['group',['leader']]]],\
#Vreemde
['33016',[['isderive',['false']],['group',['Copper',]],['categorie',['soldier']],['faction',['nilfgaard']]]],\
#Princess Adda
['41002',[['isderive',['false']],['group',['Copper']],['categorie',['cursed']],['faction',['NorthernRealms']]]],\
#Kiyan
['42010',[['isderive',['false']],['group',['Copper','silver']],['categorie',['alchemy']]]],\
#Filavandrel
['51003',[['isderive',['false']],['group',['silver']],['cardtype',['special']]]],\
#Isengrim: Outlaw
['52013',[['isderive',['false']],['group',['silver']],['categorie',['elf']]]],\
#Mahakam Horn
['53021',[['isderive',['false']],['group',['Copper','silver']],['categorie',['dwarf']]]],\
#Eist Tuirseach
['61003',[['isderive',['false']],['group',['Copper']],['categorie',['Tuirseach']]]],\
#Hammond
['70003',[['isderive',['false']],['group',['Copper']],['categorie',['machine']],['faction',['Skellige']]]],\
#Queen Adalia
['70141',[['isderive',['false']],['group',['Copper']],['categorie',['cintra']]]],\
#Philippa: Mistress of the Lodge
['70086',[['isderive',['false']],['group',['Copper']],['categorie',['spell']]]],\
#MOrune
['23020',[['isderive',['false']],['group',['Copper','silver']],['faction',['monster']]]],\
#NGrune
['33019',[['isderive',['false']],['group',['Copper','silver']],['faction',['nilfgaard']]]],\
#NRrune
['43019',[['isderive',['false']],['group',['Copper','silver']],['faction',['NorthernRealms']]]],\
#SCrune
['53018',[['isderive',['false']],['group',['Copper','silver']],['faction',['ScoiaTael']]]],\
#SKrune
['63018',[['isderive',['false']],['group',['Copper','silver']],['faction',['Skellige']]]],\
#Uma
['12039',[['isderive',['false']],['group',['gold']]]]\
]
        
for el in cards:
     should=lst_to_string(dict_to_lst(filtrmany(el[1])))
     inmap=linked(el[0])
     if should != inmap:
        print('Card: '+el[0])
        print("In map: "+inmap)
        print("should: "+should)
        
        