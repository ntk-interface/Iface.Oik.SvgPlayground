using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

namespace Iface.Oik.SvgPlayground.Util
{
  public static class BitmapUtil
  {
    public static BitmapImage ConvertBitmap(Bitmap bitmap)
    {
      if (bitmap == null) return null;
      
      var ms = new MemoryStream();
      bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
      
      var image = new BitmapImage();
      image.BeginInit();
      ms.Seek(0, SeekOrigin.Begin);
      image.StreamSource = ms;
      image.EndInit();

      return image;
    }
  }
}