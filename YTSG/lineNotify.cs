using System.Text;
using System.Net;
using System.Web;

namespace YTSG
{
    static class lineNotify
    {
        public static void doNotify(string str)
        {
            var token = "wZsL5bz2aFChKF5Gv6dxcot8R3SXbo7nmCjBVWa6U56";

            var url = "https://notify-api.line.me/api/notify";
            var enc = Encoding.UTF8;
            var payload = "message=" + HttpUtility.UrlEncode(str, enc);

            using (var wc = new WebClient())
            {
                wc.Encoding = enc;
                wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                wc.Headers.Add("Authorization", "Bearer " + token);
                var response = wc.UploadString(url, payload);
            }
        }

    }
}
