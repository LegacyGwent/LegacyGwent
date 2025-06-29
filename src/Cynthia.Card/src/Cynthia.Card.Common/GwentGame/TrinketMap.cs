using System;
using System.Collections.Generic;
using System.Linq;
using Alsein.Extensions;

namespace Cynthia.Card
{
    public static class TrinketMap
    {
        //
        public static Version TrinketMapVersion { get; } = new Version(1, 0, 0, 4);
        public static IEnumerable<TrinketAvatar> GetAvatars()
        {
            return AvatarMap
            .Select(x => x.Value);
        }
        public static IEnumerable<string> GetAvatarsId()
        {
            return AvatarMap
            .Select(x => x.Key);
        }
        public static IEnumerable<Border> GetBorders()
        {
            return BorderMap
            .Select(x => x.Value);
        }
        public static IEnumerable<string> GetBordersId()
        {
            return BorderMap
            .Select(x => x.Key);
        }
        public static IEnumerable<Title> GetTitles()
        {
            return TitleMap
            .Select(x => x.Value);
        }
        public static IEnumerable<string> GetTitlesId()
        {
            return TitleMap
            .Select(x => x.Key);
        }


        public static IDictionary<string, TrinketAvatar> AvatarMap { get; set; } = new Dictionary<string, TrinketAvatar>
        {
            //=========================================================================================================================================================================
            //Avatars
            //=========================================================================================================================================================================
            {
                "NoAvatar",
                new TrinketAvatar()
                {
                    ID = "NoAvatar",
                    Taunt1 = "",
                    Taunt2 = "",
                    Taunt3 = "",
                    Taunt4 = "",
                    Taunt5 = "",
                    Taunt6 = "",
                    IsReleased = true,
                }
            },
            {
                "GeraltOfRivia",
                new TrinketAvatar()
                {
                    ID = "GeraltOfRivia",
                    Taunt1 = "Geralt1",
                    Taunt2 = "Geralt2",
                    Taunt3 = "Geralt3",
                    Taunt4 = "Geralt4",
                    Taunt5 = "Geralt5",
                    Taunt6 = "Geralt6",
                    IsReleased = true,
                }
            },
            {
                "TrissMerigold",
                new TrinketAvatar()
                {
                    ID = "TrissMerigold",
                    Taunt1 = "Triss1",
                    Taunt2 = "Triss2",
                    Taunt3 = "Triss3",
                    Taunt4 = "Triss4",
                    Taunt5 = "Triss5",
                    Taunt6 = "Triss6",
                    IsReleased = true,
                }
            },
            {
                "Yennefer",
                new TrinketAvatar()
                {
                    ID = "Yennefer",
                    Taunt1 = "Yennefer1",
                    Taunt2 = "Yennefer2",
                    Taunt3 = "Yennefer3",
                    Taunt4 = "Yennefer4",
                    Taunt5 = "Yennefer5",
                    Taunt6 = "Yennefer6",
                    IsReleased = true,
                }
            },
            {
                "Ciri",
                new TrinketAvatar()
                {
                    ID = "Ciri",
                    Taunt1 = "Ciri1",
                    Taunt2 = "Ciri2",
                    Taunt3 = "Ciri3",
                    Taunt4 = "Ciri4",
                    Taunt5 = "Ciri5",
                    Taunt6 = "Ciri6",
                    IsReleased = false,
                }
            },
            {
                "Dandelion",
                new TrinketAvatar()
                {
                    ID = "Dandelion",
                    Taunt1 = "Dandelion1",
                    Taunt2 = "Dandelion2",
                    Taunt3 = "Dandelion3",
                    Taunt4 = "Dandelion4",
                    Taunt5 = "Dandelion5",
                    Taunt6 = "Dandelion6",
                    IsReleased = false,
                }
            },
            {
                "Zoltan_Animal_Tamer",
                new TrinketAvatar()
                {
                    ID = "Zoltan_Animal_Tamer",
                    Taunt1 = "Zoltan1",
                    Taunt2 = "Zoltan2",
                    Taunt3 = "Zoltan3",
                    Taunt4 = "Zoltan4",
                    Taunt5 = "Zoltan5",
                    Taunt6 = "Zoltan6",
                    IsReleased = false,
                }
            },
            {
                "YenneferFury",
                new TrinketAvatar()
                {
                    ID = "YenneferFury",
                    Taunt1 = "Yennefer1",
                    Taunt2 = "Yennefer2",
                    Taunt3 = "Yennefer3",
                    Taunt4 = "Yennefer4",
                    Taunt5 = "Yennefer5",
                    Taunt6 = "Yennefer6",
                    IsReleased = true,
                }
            },
            {
                "Vernon_Roche",
                new TrinketAvatar()
                {
                    ID = "Vernon_Roche",
                    Taunt1 = "Roche1",
                    Taunt2 = "Roche2",
                    Taunt3 = "Roche3",
                    Taunt4 = "Roche4",
                    Taunt5 = "Roche5",
                    Taunt6 = "Roche6",
                    IsReleased = false,
                }
            },
            {
                "TrissSorceress",
                new TrinketAvatar()
                {
                    ID = "TrissSorceress",
                    Taunt1 = "Triss1",
                    Taunt2 = "Triss2",
                    Taunt3 = "Triss3",
                    Taunt4 = "Triss4",
                    Taunt5 = "Triss5",
                    Taunt6 = "Triss6",
                    IsReleased = true,
                }
            },
            {
                "Regis",
                new TrinketAvatar()
                {
                    ID = "Regis",
                    Taunt1 = "Regis1",
                    Taunt2 = "Regis2",
                    Taunt3 = "Regis3",
                    Taunt4 = "Regis4",
                    Taunt5 = "Regis5",
                    Taunt6 = "Regis6",
                    IsReleased = false,
                }
            },
            {
                "Radovid",
                new TrinketAvatar()
                {
                    ID = "Radovid",
                    Taunt1 = "Radovid1",
                    Taunt2 = "Radovid2",
                    Taunt3 = "Radovid3",
                    Taunt4 = "Radovid4",
                    Taunt5 = "Radovid5",
                    Taunt6 = "Radovid6",
                    IsReleased = false,
                }
            },
            {
                "Phoenix",
                new TrinketAvatar()
                {
                    ID = "Phoenix",
                    Taunt1 = "Phoenix1",
                    Taunt2 = "Phoenix2",
                    Taunt3 = "Phoenix3",
                    Taunt4 = "Phoenix4",
                    Taunt5 = "Phoenix5",
                    Taunt6 = "Phoenix6",
                    IsReleased = true,
                    UnlockCounter = 200,
                    UnlockStat = "GGsReceived",
                }
            },
            {
                "Odrin",
                new TrinketAvatar()
                {
                    ID = "Odrin",
                    Taunt1 = "Odrin1",
                    Taunt2 = "Odrin2",
                    Taunt3 = "Odrin3",
                    Taunt4 = "Odrin4",
                    Taunt5 = "Odrin5",
                    Taunt6 = "Odrin6",
                    IsReleased = false,
                }
            },
            {
                "Letho",
                new TrinketAvatar()
                {
                    ID = "Letho",
                    Taunt1 = "Letho1",
                    Taunt2 = "Letho2",
                    Taunt3 = "Letho3",
                    Taunt4 = "Letho4",
                    Taunt5 = "Letho5",
                    Taunt6 = "Letho6",
                    IsReleased = false,
                }
            },
            {
                "King_Bran",
                new TrinketAvatar()
                {
                    ID = "King_Bran",
                    Taunt1 = "Bran1",
                    Taunt2 = "Bran2",
                    Taunt3 = "Bran3",
                    Taunt4 = "Bran4",
                    Taunt5 = "Bran5",
                    Taunt6 = "Bran6",
                    IsReleased = false,
                }
            },
            {
                "IorvethScarf",
                new TrinketAvatar()
                {
                    ID = "IorvethScarf",
                    Taunt1 = "Iorveth1",
                    Taunt2 = "Iorveth2",
                    Taunt3 = "Iorveth3",
                    Taunt4 = "Iorveth4",
                    Taunt5 = "Iorveth5",
                    Taunt6 = "Iorveth6",
                    IsReleased = false,
                }
            },
            {
                "Iorveth",
                new TrinketAvatar()
                {
                    ID = "Iorveth",
                    Taunt1 = "Iorveth1",
                    Taunt2 = "Iorveth2",
                    Taunt3 = "Iorveth3",
                    Taunt4 = "Iorveth4",
                    Taunt5 = "Iorveth5",
                    Taunt6 = "Iorveth6",
                    IsReleased = true,
                }
            },
            {
                "Imlerith_Unmasked",
                new TrinketAvatar()
                {
                    ID = "Imlerith_Unmasked",
                    Taunt1 = "Imlerith1",
                    Taunt2 = "Imlerith2",
                    Taunt3 = "Imlerith3",
                    Taunt4 = "Imlerith4",
                    Taunt5 = "Imlerith5",
                    Taunt6 = "Imlerith6",
                    IsReleased = true,
                }
            },
            {
                "Imlerith",
                new TrinketAvatar()
                {
                    ID = "Imlerith",
                    Taunt1 = "Imlerith1",
                    Taunt2 = "Imlerith2",
                    Taunt3 = "Imlerith3",
                    Taunt4 = "Imlerith4",
                    Taunt5 = "Imlerith5",
                    Taunt6 = "Imlerith6",
                    IsReleased = false,
                }
            },
            {
                "Geralt_Intoxicated",
                new TrinketAvatar()
                {
                    ID = "Geralt_Intoxicated",
                    Taunt1 = "Geralt1",
                    Taunt2 = "Geralt2",
                    Taunt3 = "Geralt3",
                    Taunt4 = "Geralt4",
                    Taunt5 = "Geralt5",
                    Taunt6 = "Geralt6",
                    IsReleased = false,
                }
            },
            {
                "Francesca",
                new TrinketAvatar()
                {
                    ID = "Francesca",
                    Taunt1 = "Francesca1",
                    Taunt2 = "Francesca2",
                    Taunt3 = "Francesca3",
                    Taunt4 = "Francesca4",
                    Taunt5 = "Francesca5",
                    Taunt6 = "Francesca6",
                    IsReleased = false,
                }
            },
            {
                "Eredin_Unmasked",
                new TrinketAvatar()
                {
                    ID = "Eredin_Unmasked",
                    Taunt1 = "Eredin1",
                    Taunt2 = "Eredin2",
                    Taunt3 = "Eredin3",
                    Taunt4 = "Eredin4",
                    Taunt5 = "Eredin5",
                    Taunt6 = "Eredin6",
                    IsReleased = false,
                }
            },
            {
                "Dandelionthewitcher2",
                new TrinketAvatar()
                {
                    ID = "Dandelionthewitcher2",
                    Taunt1 = "Dandelion1",
                    Taunt2 = "Dandelion2",
                    Taunt3 = "Dandelion3",
                    Taunt4 = "Dandelion4",
                    Taunt5 = "Dandelion5",
                    Taunt6 = "Dandelion6",
                    IsReleased = true,
                }
            },            
            {
                "Dagon",
                new TrinketAvatar()
                {
                    ID = "Dagon",
                    Taunt1 = "Dagon1",
                    Taunt2 = "Dagon2",
                    Taunt3 = "Dagon3",
                    Taunt4 = "Dagon4",
                    Taunt5 = "Dagon5",
                    Taunt6 = "Dagon6",
                    IsReleased = false,
                }
            },
            {
                "ClassicGeralt",
                new TrinketAvatar()
                {
                    ID = "ClassicGeralt",
                    Taunt1 = "Geralt1",
                    Taunt2 = "Geralt2",
                    Taunt3 = "Geralt3",
                    Taunt4 = "Geralt4",
                    Taunt5 = "Geralt5",
                    Taunt6 = "Geralt6",
                    IsReleased = true,
                }
            },
            {
                "CirALt",
                new TrinketAvatar()
                {
                    ID = "CirALt",
                    Taunt1 = "Ciri1",
                    Taunt2 = "Ciri2",
                    Taunt3 = "Ciri3",
                    Taunt4 = "Ciri4",
                    Taunt5 = "Ciri5",
                    Taunt6 = "Ciri6",
                    IsReleased = true,
                }
            },
            {
                "Maerolorn",
                new TrinketAvatar()
                {
                    ID = "Maerolorn",
                    Taunt1 = "Maerolorn1",
                    Taunt2 = "Maerolorn2",
                    Taunt3 = "Maerolorn3",
                    Taunt4 = "Maerolorn4",
                    Taunt5 = "Maerolorn5",
                    Taunt6 = "Maerolorn6",
                    IsReleased = false,
                }
            },            
        };

        public static IDictionary<string, Border> BorderMap { get; set; } = new Dictionary<string, Border>
        {
            //=========================================================================================================================================================================
            //Borders
            //=========================================================================================================================================================================
            {
                "NoBorder",
                new Border()
                {
                    ID = "NoBorder",
                    IsReleased = true,
                }
            },
            {
                "Rank3border",
                new Border()
                {
                    ID = "Rank3border",
                    IsReleased = true,
                }
            },
            {
                "Rank6border",
                new Border()
                {
                    ID = "Rank6border",
                    IsReleased = true,
                }
            },
            {
                "Rank9border",
                new Border()
                {
                    ID = "Rank9border",
                    IsReleased = true,
                }
            },
            {
                "Rank12border",
                new Border()
                {
                    ID = "Rank12border",
                    IsReleased = true,
                }
            },
            {
                "Rank15border",
                new Border()
                {
                    ID = "Rank15border",
                    IsReleased = true,
                }
            },
            {
                "Rank18border",
                new Border()
                {
                    ID = "Rank18border",
                    IsReleased = true,
                }
            },
            {
                "Rank21border",
                new Border()
                {
                    ID = "Rank21border",
                    IsReleased = true,
                }
            },
            {
                "Rank21Border2",
                new Border()
                {
                    ID = "Rank21Border2",
                    IsReleased = false,
                }
            },
            {
                "FactionMO",
                new Border()
                {
                    ID = "FactionMO",
                    IsReleased = false,
                }
            },
            {
                "FactionNG",
                new Border()
                {
                    ID = "FactionNG",
                    IsReleased = false,
                }
            },
            {
                "FactionNR",
                new Border()
                {
                    ID = "FactionNR",
                    IsReleased = false,
                }
            },
            {
                "FactionSK",
                new Border()
                {
                    ID = "FactionSK",
                    IsReleased = false,
                }
            },
            {
                "FactionST",
                new Border()
                {
                    ID = "FactionST",
                    IsReleased = false,
                }
            },
            {
                "G_Bat",
                new Border()
                {
                    ID = "G_Bat",
                    IsReleased = false,
                }
            },
            {
                "G_Beer",
                new Border()
                {
                    ID = "G_Beer",
                    IsReleased = false,
                }
            },
            {
                "G_Phoenix",
                new Border()
                {
                    ID = "G_Phoenix",
                    IsReleased = true,
                    UnlockCounter = 100,
                    UnlockStat = "GGsReceived",
                }
            },
            {
                "Season1Border1",
                new Border()
                {
                    ID = "Season1Border1",
                    IsReleased = true,
                }
            },
            {
                "Season1Border2",
                new Border()
                {
                    ID = "Season1Border2",
                    IsReleased = true,
                }
            },
            {
                "Season1Border3",
                new Border()
                {
                    ID = "Season1Border3",
                    IsReleased = true,
                }
            },
            {
                "Season1Border4",
                new Border()
                {
                    ID = "Season1Border4",
                    IsReleased = true,
                }
            },
            {
                "Season1Border5",
                new Border()
                {
                    ID = "Season1Border5",
                    IsReleased = true,
                }
            },
            {
                "Season1Border6",
                new Border()
                {
                    ID = "Season1Border6",
                    IsReleased = true,
                }
            },
            {
                "Season2Border1",
                new Border()
                {
                    ID = "Season2Border1",
                    IsReleased = true,
                }
            },
            {
                "Season2Border2",
                new Border()
                {
                    ID = "Season2Border2",
                    IsReleased = true,
                }
            },
            {
                "Season2Border3",
                new Border()
                {
                    ID = "Season2Border3",
                    IsReleased = true,
                }
            },
            {
                "Season2Border4",
                new Border()
                {
                    ID = "Season2Border4",
                    IsReleased = true,
                }
            },
            {
                "Season2Border5",
                new Border()
                {
                    ID = "Season2Border5",
                    IsReleased = true,
                }
            },
            {
                "Season2Border6",
                new Border()
                {
                    ID = "Season2Border6",
                    IsReleased = true,
                }
            },
            {
                "Season3Border1",
                new Border()
                {
                    ID = "Season3Border1",
                    IsReleased = false,
                }
            },
            {
                "Season3Border2",
                new Border()
                {
                    ID = "Season3Border2",
                    IsReleased = false,
                }
            },
            {
                "Season3Border3",
                new Border()
                {
                    ID = "Season3Border3",
                    IsReleased = false,
                }
            },
            {
                "Season3Border4",
                new Border()
                {
                    ID = "Season3Border4",
                    IsReleased = false,
                }
            },
            {
                "Season3Border5",
                new Border()
                {
                    ID = "Season3Border5",
                    IsReleased = false,
                }
            },
            {
                "Season3Border6",
                new Border()
                {
                    ID = "Season3Border6",
                    IsReleased = false,
                }
            },
            {
                "Season4Border1",
                new Border()
                {
                    ID = "Season4Border1",
                    IsReleased = false,
                }
            },
            {
                "Season4Border2",
                new Border()
                {
                    ID = "Season4Border2",
                    IsReleased = false,
                }
            },
            {
                "Season4Border3",
                new Border()
                {
                    ID = "Season4Border3",
                    IsReleased = false,
                }
            },
            {
                "Season4Border4",
                new Border()
                {
                    ID = "Season4Border4",
                    IsReleased = false,
                }
            },
            {
                "Season4Border5",
                new Border()
                {
                    ID = "Season4Border5",
                    IsReleased = false,
                }
            },
            {
                "Season4Border6",
                new Border()
                {
                    ID = "Season4Border6",
                    IsReleased = false,
                }
            },
            {
                "Season5Border1",
                new Border()
                {
                    ID = "Season5Border1",
                    IsReleased = false,
                }
            },
            {
                "Season5Border2",
                new Border()
                {
                    ID = "Season5Border2",
                    IsReleased = false,
                }
            },
            {
                "Season5Border3",
                new Border()
                {
                    ID = "Season5Border3",
                    IsReleased = false,
                }
            },
            {
                "Season5Border4",
                new Border()
                {
                    ID = "Season5Border4",
                    IsReleased = false,
                }
            },
            {
                "Season5Border5",
                new Border()
                {
                    ID = "Season5Border5",
                    IsReleased = false,
                }
            },
            {
                "Season5Border6",
                new Border()
                {
                    ID = "Season5Border6",
                    IsReleased = false,
                }
            },
            {
                "Season6Border1",
                new Border()
                {
                    ID = "Season6Border1",
                    IsReleased = false,
                }
            },
            {
                "Season6Border2",
                new Border()
                {
                    ID = "Season6Border2",
                    IsReleased = false,
                }
            },
            {
                "Season6Border3",
                new Border()
                {
                    ID = "Season6Border3",
                    IsReleased = false,
                }
            },
            {
                "Season6Border4",
                new Border()
                {
                    ID = "Season6Border4",
                    IsReleased = false,
                }
            },
            {
                "Season6Border5",
                new Border()
                {
                    ID = "Season6Border5",
                    IsReleased = false,
                }
            },
            {
                "Season6Border6",
                new Border()
                {
                    ID = "Season6Border6",
                    IsReleased = false,
                }
            },
            {
                "Season7Border1",
                new Border()
                {
                    ID = "Season7Border1",
                    IsReleased = false,
                }
            },
            {
                "Season7Border2",
                new Border()
                {
                    ID = "Season7Border2",
                    IsReleased = false,
                }
            },
            {
                "Season7Border3",
                new Border()
                {
                    ID = "Season7Border3",
                    IsReleased = false,
                }
            },
            {
                "Season7Border4",
                new Border()
                {
                    ID = "Season7Border4",
                    IsReleased = false,
                }
            },
            {
                "Season7Border5",
                new Border()
                {
                    ID = "Season7Border5",
                    IsReleased = false,
                }
            },
            {
                "Season7Border6",
                new Border()
                {
                    ID = "Season7Border6",
                    IsReleased = false,
                }
            },
        };

        public static IDictionary<string, Title> TitleMap { get; set; } = new Dictionary<string, Title>
        {
            //=========================================================================================================================================================================
            //Titles
            //=========================================================================================================================================================================
            { 
                "CARDSMITH",
                new Title()
                {
                    ID = "CARDSMITH",
                    IsReleased = true,
                    TitleColor = "white",
                }
            },
            { 
                "NOVICE",
                new Title()
                {
                    ID = "NOVICE",
                    IsReleased = true,
                    TitleColor = "white",
                }
            },
            { 
                "APPRENTICE",
                new Title()
                {
                    ID = "APPRENTICE",
                    IsReleased = true,
                    TitleColor = "white",
                }
            },
            { 
                "JOURNEYMAN",
                new Title()
                {
                    ID = "JOURNEYMAN",
                    IsReleased = true,
                    TitleColor = "white",
                }
            },
            { 
                "ADEPT",
                new Title()
                {
                    ID = "ADEPT",
                    IsReleased = true,
                    TitleColor = "white",
                }
            },
            { 
                "CARDSHARP",
                new Title()
                {
                    ID = "CARDSHARP",
                    IsReleased = true,
                    TitleColor = "white",
                }
            },
            { 
                "MASTER",
                new Title()
                {
                    ID = "MASTER",
                    IsReleased = true,
                    TitleColor = "white",
                }
            },
            { 
                "GRANDMASTER",
                new Title()
                {
                    ID = "GRANDMASTER",
                    IsReleased = true,
                    TitleColor = "white",
                }
            },
            { 
                "MAN-AT-ARMS",
                new Title()
                {
                    ID = "MAN-AT-ARMS",
                    IsReleased = true,
                    TitleColor = "darkyellow",
                }
            },
            { 
                "MERCENARY",
                new Title()
                {
                    ID = "MERCENARY",
                    IsReleased = true,
                    TitleColor = "darkyellow",
                }
            },
            { 
                "BOUNTYHUNTER",
                new Title()
                {
                    ID = "BOUNTYHUNTER",
                    IsReleased = true,
                    TitleColor = "darkyellow",
                }
            },
            { 
                "VETERAN",
                new Title()
                {
                    ID = "VETERAN",
                    IsReleased = true,
                    TitleColor = "darkyellow",
                }
            },
                        { 
                "CHAMPION",
                new Title()
                {
                    ID = "CHAMPION",
                    IsReleased = true,
                    TitleColor = "darkyellow",
                }
            },
            { 
                "HERO",
                new Title()
                {
                    ID = "HERO",
                    IsReleased = true,
                    TitleColor = "darkyellow",
                }
            },
            { 
                "PIONEER",
                new Title()
                {
                    ID = "PIONEER",
                    IsReleased = true,
                    TitleColor = "darkgreen",
                }
            },
            { 
                "GOODGAMER",
                new Title()
                {
                    ID = "GOODGAMER",
                    IsReleased = true,
                    TitleColor = "blue",
                    UnlockCounter = 500,
                    UnlockStat = "GGsReceived",
                }
            },
            { 
                "RANGER",
                new Title()
                {
                    ID = "RANGER",
                    IsReleased = true,
                    TitleColor = "emerald",
                }
            },
            { 
                "TRAPPER",
                new Title()
                {
                    ID = "TRAPPER",
                    IsReleased = true,
                    TitleColor = "emerald",
                }
            },
            { 
                "HUNTER",
                new Title()
                {
                    ID = "HUNTER",
                    IsReleased = true,
                    TitleColor = "emerald",
                }
            },
            { 
                "REBEL",
                new Title()
                {
                    ID = "REBEL",
                    IsReleased = true,
                    TitleColor = "emerald",
                }
            },
                        { 
                "DEFENDER",
                new Title()
                {
                    ID = "DEFENDER",
                    IsReleased = true,
                    TitleColor = "emerald",
                }
            },
            { 
                "PROTECTOR",
                new Title()
                {
                    ID = "PROTECTOR",
                    IsReleased = true,
                    TitleColor = "emerald",
                }
            },
        };
    }
}
