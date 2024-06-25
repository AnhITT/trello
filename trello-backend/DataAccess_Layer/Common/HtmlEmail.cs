using System.Text;

namespace DataAccess_Layer.Common
{
    public static class HtmlEmail
    {
        public static string GetHtmlVerify(string generateLink)
        {
            string Response = "<!DOCTYPE html>";
            Response += "<html>";
            Response += "<head>";
            Response += "  <title>Xác thực đăng ký</title>";
            Response += "  <style>";
            Response += "    body { font-family: Arial, sans-serif; }";
            Response += "    .container { width: 600px; margin: 0 auto; background-color: #f2f2f2; padding: 20px; border-radius: 10px; }";
            Response += "    .header { background-color: #34d5d1; color: white; padding: 10px; text-align: center; }";
            Response += "    .content { padding: 20px; text-align: center; }";
            Response += "    .button { background-color: #34d5d1; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px; }";
            Response += "  </style>";
            Response += "</head>";
            Response += "<body>";
            Response += "  <div class='container'>";
            Response += "    <div class='header'>";
            Response += "      <h2>Xác thực đăng ký</h2>";
            Response += "    </div>";
            Response += "    <div class='content'>";
            Response += "      <p>Nhấn vào liên kết bên dưới để xác nhận đăng ký tài khoản:</p>";
            Response += $"      <a href='{generateLink}' class='button'>Xác nhận đăng ký</a>";
            Response += "    </div>";
            Response += "  </div>";
            Response += "</body>";
            Response += "</html>";

            return Response;
        }
        public static string GetHtmlSendPassword(string password)
        {
            string Response = "<!DOCTYPE html>";
            Response += "<html>";
            Response += "<head>";
            Response += "  <title>Hoàn tất đăng ký</title>";
            Response += "  <style>";
            Response += "    body { font-family: Arial, sans-serif; }";
            Response += "    .container { width: 600px; margin: 0 auto; background-color: #f2f2f2; padding: 20px; border-radius: 10px; }";
            Response += "    .header { background-color: #34d5d1; color: white; padding: 10px; text-align: center; }";
            Response += "    .content { padding: 20px; text-align: center; }";
            Response += "    .button { background-color: #34d5d1; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px; }";
            Response += "  </style>";
            Response += "</head>";
            Response += "<body>";
            Response += "  <div class='container'>";
            Response += "    <div class='header'>";
            Response += "      <h2>Hoàn tất đăng ký</h2>";
            Response += "    </div>";
            Response += "    <div class='content'>";
            Response += "      <p>Chúc mừng bạn đã đăng ký thành công tài khoản!</p>";
            Response += "      <p>Vui lòng nhấn vào nút bên dưới để đăng nhập và sử dụng ứng dụng.</p>";
            Response += $"      <p>Mật khẩu đăng nhập của bạn là: <strong>{password}</strong></p>";
            Response += "      <a href='#' class='button'>Đăng nhập</a>";
            Response += "    </div>";
            Response += "  </div>";
            Response += "</body>";
            Response += "</html>";

            return Response;
        }
        public static string GetHtmlResetPassword(string otp)
        {
            string Response = "<!DOCTYPE html>";
            Response += "<html>";
            Response += "<head>";
            Response += "  <title>OTP khôi phục mật khẩu</title>";
            Response += "  <style>";
            Response += "    body { font-family: Arial, sans-serif; }";
            Response += "    .container { width: 600px; margin: 0 auto; background-color: #f2f2f2; padding: 20px; border-radius: 10px; }";
            Response += "    .header { background-color: #34d5d1; color: white; padding: 10px; text-align: center; }";
            Response += "    .content { padding: 20px; text-align: center; }";
            Response += "    .button { background-color: #34d5d1; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px; }";
            Response += "  </style>";
            Response += "</head>";
            Response += "<body>";
            Response += "  <div class='container'>";
            Response += "    <div class='header'>";
            Response += "      <h2>OTP khôi phục mật khẩu</h2>";
            Response += "    </div>";
            Response += "    <div class='content'>";
            Response += $"      <p>OTP của bạn là: <strong>{otp}</strong></p>";
            Response += "    </div>";
            Response += "  </div>";
            Response += "</body>";
            Response += "</html>";

            return Response;
        }
        public static string EncodeToBase64(string id, string tokenEmail)
        {
            try
            {
                string encodeString = $"{id}:{tokenEmail}";
                byte[] encodeBytes = Encoding.UTF8.GetBytes(encodeString);
                return Convert.ToBase64String(encodeBytes);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
