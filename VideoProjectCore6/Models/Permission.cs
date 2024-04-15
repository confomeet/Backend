using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace VideoProjectCore6.Models;

[PrimaryKey("Name")]
public class Permission
{
    [Required]
    public string Name { get; set; } = null!;

    [Required]
    public int RoleId { get; set; }

    public Role Role { get; set; } = null!;
}
