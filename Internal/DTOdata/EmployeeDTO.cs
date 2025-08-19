using System.ComponentModel;

namespace FlightAPIs.Internal.DTOdata
{
    public class EmployeeDTO
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string Email { get; set; } = null!;

        public string? Cccd { get; set; }

        public string? Address { get; set; }

        public string? PhoneNumber { get; set; }

        public string Password { get; set; } = null!;

        private int UserType = 2;
        public int getUserType()
        {
            return this.UserType;
        }
        public void setUserType( int i)
        {
            if(i!= 0 && i!= 3)
            {
                this.UserType = 3;
            }
            this.UserType = i;
        }
    }
}
