using FlightAPIs.Internal.DTOdata;
using FlightAPIs.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Text.RegularExpressions;

namespace FlightAPIs.Helper
{
    public class EmployeeValidation
    {
        public async Task<Dictionary<string,string>> validation( DbAbb296Kuphe1980Context db , bool isCreated , EmployeeDTO employeeDTO)
        {
            Dictionary<string, string> errorInformation = new Dictionary<string, string>();
            //take employee exist email 
            bool emailExist = await db.Admins.Where(u=>u.Email == employeeDTO.Email.Trim()).AnyAsync();
            if (isCreated && employeeDTO.Id != 0 )
            {
                emailExist = await db.Admins.Where(u => u.Email == employeeDTO.Email.Trim() && u.Id != employeeDTO.Id).AnyAsync();
            }
            if (employeeDTO == null)
            {
                errorInformation.Add("Error", "employee show not be empty");
                return errorInformation;
            }
            if (!checkCCCD(employeeDTO?.Cccd!))
            {
                errorInformation.Add("Error", "wrong CCCD pattern");
                return errorInformation;
            }
            if (!checkPhoneNumber(employeeDTO?.PhoneNumber!))
            {
                errorInformation.Add("Error", "wrong PhoneNumber pattern");
                return errorInformation;
            }
            if (!checkGmail(employeeDTO?.Email!))
            {
                errorInformation.Add("Error", "wrong Email pattern");
                return errorInformation;
            }
            if (emailExist)
            {
                errorInformation.Add("Error", "Email has been used");
                return errorInformation;
            }
            if (!IsPasswordStrong(employeeDTO?.Password!))
            {
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
