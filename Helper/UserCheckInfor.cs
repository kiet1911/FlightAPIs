using FlightAPIs.Models;
using System.Text.RegularExpressions;

namespace FlightAPIs.Helper
{
    internal class UserCheckInfor
    {
        public Dictionary<string,string> userCheckInformation(User user)
        {
            Dictionary<string, string> errorInformation = new Dictionary<string, string>();
            if(user == null)
            {
                errorInformation.Add("Error", "user show not be empty");
                return errorInformation;
            }
            if (!checkCCCD(user?.Cccd!))
            {
                errorInformation.Add("Error", "wrong CCCD pattern");
                return errorInformation;
            }
            if (!checkPhoneNumber(user?.PhoneNumber!)){
                errorInformation.Add("Error", "wrong PhoneNumber pattern");
                return errorInformation;
            }
            if (!checkGmail(user?.Email!)){
                errorInformation.Add("Error", "wrong Email pattern");
                return errorInformation;
            }
            if (!IsPasswordStrong(user?.Password!)){
                errorInformation.Add("Error", "password not strong enough ");
                return errorInformation;
            }
            return errorInformation;
        }

        public bool checkCCCD(String cccd)
        {
            String regex = "^\\d{12}$";
            if (Regex.IsMatch(cccd, regex))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool checkPhoneNumber(String phoneNumber)
        {
            String regex = @"^\+84\d{9,10}$";
            if (Regex.IsMatch(phoneNumber, regex))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool checkGmail(String gmail)
        {
            string pattern = @"(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|""(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*"")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])";
            if (Regex.IsMatch(gmail, pattern, RegexOptions.IgnoreCase))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool IsPasswordStrong(string password)
        {
            // Thêm logic kiểm tra độ mạnh của mật khẩu ở đây
            return password.Length >= 8 &&
                   password.Any(char.IsUpper) &&
                   password.Any(char.IsDigit);
        }
    }
}
