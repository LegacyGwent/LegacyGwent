using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Drawing.Imaging;

public class DiyCardInfo
{
    public ObjectId _id;
    public int uid;
    public string cardName;
    public string describe;
    public List<Commit> commits = new List<Commit>();
    public class Commit
    {
        public string user;
        public string text;
        public List<string> likeList;
        public List<string> dislikeList;
    }
}
public class DefaultTexture
{
    public ObjectId _id;
    public byte[] cardFramesImage;
    public byte[] cardUploadImage;
    public byte[] cardLoadImage;
}
public class DiyCardTextureInfo
{

    public ObjectId _id;
    public int uid;
    public byte[] imagedata;

    public DiyCardTextureInfo(int uid, Image image)
    {
        this.uid = uid;
        MemoryStream mstream = new MemoryStream();
        image.Save(mstream, ImageFormat.Gif);
        imagedata = new Byte[mstream.Length];
        mstream.Position = 0;
        mstream.Read(imagedata, 0, imagedata.Length);
        mstream.Close();
        image.Dispose();

    }

    public Image GetImage()
    {
        MemoryStream ms = new MemoryStream(imagedata);
        return Image.FromStream(ms);
    }

}
public class ResponseModel
{
    public string name { get; set; }

    public string status { get; set; }

    public string url { get; set; }

    public string thumbUrl { get; set; }
}