using MongoDB.Bson;
using System;
using System.Collections.Generic;
using Cynthia.Card;

public class DiyCardInfo
{
    public ObjectId _id;
    public int uid;
    public GwentCard cardInfo;
    public string Fraction;
    public string CardType;
    public string Group;
    public List<string> Categories;
    public string imageLink;
    public string author;
    public DateTime lastEditedDate;
    public List<string> likeList;
    public List<string> dislikeList;
    public List<Commit> commits = new List<Commit>();
    public class Commit
    {
        public string user;
        public string text;
        public List<string> likeList;
        public List<string> dislikeList;
        public DateTime commitDate;
    }
}