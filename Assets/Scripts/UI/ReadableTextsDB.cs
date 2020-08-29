public enum ReadableKey
{
    GuardsNote,
    StrangeNote,
    ConcerningNecromancy,
    Portrait,
    PrisonnersNote,
    IntroText,
}

public class ReadableTextsDB
{
    public static string getTextByKey(ReadableKey key)
    {
        switch(key)
        {
            case (ReadableKey.GuardsNote):
                return
                    "The 12th of Enerya, 537 :\n\n\tI just can't remember where this damn key is, " +
                    "Igstad hid it before he passed away because of this bloody plague... There are a lot of valuables in this chest " +
                    "and the lord is becoming more and more compelling about it! If only I could simply destroy it, but the chest is a " +
                    "very ancient one and protected by elder's magic, they say it belonged to Baldwin himself! Who knows what lies in there? " +
                    "One thing for sure, if I manage to find this key, the chest's content will be mine, the lord can go to hell! " +
                    "I've been searching for it in all the damn rooms in this floor! The only thing that remains is that strange cupboard in the library, " +
                    "I am pretty sure that there was light behind it and I was not that drunk, was I? Anyway it is way to heavy for me and I " +
                    "just can't make it move... I'll be investigating it tommorow.";
            
            case (ReadableKey.StrangeNote):
                return 
                    "Aethel...\nThat name is the only thing I know about you. " +
                    "I managed to neutralize you after you tried to kill me... Yet I need you... It seems that you have " +
                    "a stronger will than the others, I weakened the curse that is upon you, " +
                    "but it will not take long until you become mindless again. I found a portrait of a wife and a child " +
                    "in your cloths. I took it, just to make sure that you will go after me. " +
                    "But worry not, I am not a thief and I will return it to you when you find me. " +
                    "I have locked myself in another room nearby. Come and I shall answer all your questions. But for now you " +
                    "must feel weak, drink this potion or it will not take long until you join them for real.\n" +
                    "You shall need the dagger.";
            case (ReadableKey.IntroText):
                return 
                    "You are Aethel, a peasant who died during one of the many battles" +
                    " that plague the kingdom of Valdburg. The last thing you remember is when a shiny knight of Denloft Castle put" +
                    " his blade, deep into your throat. Then came a few moments of agony followed by a dark, neverending void." +
                    " Yet here you are, waking up in this dark room, you have a terrible headache and you feel weak. " +
                    " But more than anything else, your throat hurts as you are still bleeding cold, black blood. " +
                    " What the hell is going on here?!\nPerhaps you should first try to figure out how to get out of here...";

            case (ReadableKey.PrisonnersNote):
                return 
                    "The 47th of Mayen, 138 : \n\n " +
                    "It seems that after all, I am going to die in this gloomy cell, I guess I diserved it, but still... It feels weird. " +
                    "When I came here, I thought the castle was abandonned. Ancient tales say it was once King Baldwyn's, and many other adventurers " +
                    "like me told me it was filled with treasures, knowledge and lengendary weapons and armors. I have obviously been fooled, and this place " +
                    "is filled with monsters, I should have never come!\n" +
                    "I guess that's all a bandit like me diserves, it seems that after all, it is time to pay for all my sins...\n\n" +
                    "Lord, deliver me.";

            case (ReadableKey.Portrait):
                return
                    "(You see the portrait of your wife, she is holding your 8 years old son in her arms. This, added to Mordred's " +
                    "revelations, makes you realize that you will never see them again...\n\n" +
                    "Tears start to run along your cheeks as your throat is tightening.)";

            case (ReadableKey.ConcerningNecromancy):
                return 
                    "The Lord has started. He is finally trying to bring the dead back to life. Most of his experiments " +
                    "have been stunning successes, but I have noticed a side effect of the magic that has started to impregnate these walls. " +
                    "It seems that the old prisonners from Baldwyn's age are affected by the Lord's powers. I have seen some of them, walking " +
                    "meaninglessly in their cells, although they seem harmless, they will not hesitate to bite if you get to close to them! " +
                    "They also seem to be out of the lord's control and act without any sense of logic. Very curious indeed, I need to study that more " +
                    "in depth...";
            default:
                return "";
        }
    }

    public static string getTitleByKey(ReadableKey key)
    {
        switch (key)
        {
            case (ReadableKey.GuardsNote):
                return
                    "A Guard's note";
            case (ReadableKey.StrangeNote):
                return 
                    "A strange note";
            case (ReadableKey.IntroText):
                return 
                    "Introduction";
            case (ReadableKey.PrisonnersNote):
                return 
                    "A prisonner's note";
            case (ReadableKey.Portrait):
                return 
                    "A portrait";
            case (ReadableKey.ConcerningNecromancy):
                return 
                    "Concerning Necromancy";
            default:
                return "";
        }
    }
}
