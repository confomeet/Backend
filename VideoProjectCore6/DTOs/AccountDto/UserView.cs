﻿namespace VideoProjectCore6.DTOs.AccountDto
#nullable disable
{
    public class UserView
    {
        public virtual string PhoneNumber { get; set; }
        public virtual string Email { get; set; }
        public virtual string UserName { get; set; }
        public virtual int Id { get; set; }
        public string FullName { get; set; }
        public bool Locked { get; set; }
        public bool Confirmed { get; set; }
        public List<int> Roles { get; set; }
        public List<int> Groups { get; set; }
    }
}
