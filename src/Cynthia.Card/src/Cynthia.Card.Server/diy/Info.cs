using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class Info
{
    internal static List<DiyCardInfo> diyCardInfo;
    internal static DefaultTexture defaultTexture;

    internal static byte[] uploadImageData = new byte[0];
    bool isPageFirstInitialized = true;//页面在刷新时会两次调用OnInitialized
    
}
