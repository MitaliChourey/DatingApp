using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.Models;
using DatingApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace DatingApp.Dtos
{
public class UserForRegisterDto
{

[Required]
public string UserName { get; set; }

[Required]
[StringLength(8,MinimumLength=4,ErrorMessage="You must specify password between 4 to 8 ")]
public string Password { get; set; }

}
}