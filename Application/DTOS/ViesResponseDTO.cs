using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOS;

public class ViesResponseDTO
{
    public bool IsValid { get; set; }
    public string Name { get; set; }
    public string CountryCode { get; set; }
    public string VatNumber { get; set; }
}
