using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entites
{
    public class Supplier:BaseEntity
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string ContactEmail { get; private set; }
        public string? PhoneNumber { get; private set; }

        public string Vatstatus { get;  set; }
        public bool IsDeleted { get; set; }

        public Supplier(
                string name,
                string contactEmail,
                string? phoneNumber = null,
                string vatstatus = "Verified")
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Supplier name cannot be empty.");

            if (!contactEmail.Contains("@"))
                throw new ArgumentException("Invalid email format.");
            Name = name;
            ContactEmail = contactEmail;
            PhoneNumber = phoneNumber;
            Vatstatus = vatstatus;
        }
        // Optional: Methods to update supplier info
        public void UpdateInfo(string name, string contactEmail, string? phoneNumber = null)
        {
            Name = name;
            ContactEmail = contactEmail;
            PhoneNumber = phoneNumber;
        }
    }
}
