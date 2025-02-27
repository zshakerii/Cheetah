﻿using Cheetah_Business.Data;
using Cheetah_Business.Dimentions;
using Cheetah_Business.Links;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
namespace Cheetah_Business.Facts;

[Table(nameof(F_Endorsement), Schema = nameof(TableType.Facts))]
[Index(nameof(Name), IsUnique = true, AllDescending = true)]
[Index(nameof(CreateTimeRecord), IsUnique = true, AllDescending = true)]
[Index(nameof(LastUpdatedRecord), IsUnique = true, AllDescending = true)]
[Index(nameof(ERPCode), IsUnique = false, AllDescending = true)]
[Index(nameof(EnableRecord), IsUnique = false, AllDescending = true)]
//[Index(nameof(Parent_Id), IsUnique = false, AllDescending = true)]
public partial class F_Endorsement : SimpleClass
{
    [Column(Order = 100)]
    public long? RoleId { get; set; }
    public virtual D_Role? Role { get; set; }

    [Column(Order = 101)]
    public long? ScenarioId { get; set; }
    public virtual F_Scenario? Scenario { get; set; }

    [InverseProperty(nameof(F_Condition.Endorsement))]
    public virtual ICollection<F_Condition>? Conditions { get; set; } = new HashSet<F_Condition>();

    public virtual ICollection<L_TagEndorsement>? ValidUserActions { get; set; } = new HashSet<L_TagEndorsement>();

    public override void SetName()
    {
        DisplayName = Scenario?.DisplayName + "," + SortIndex + "," + Role?.DisplayName;
        Name = Scenario?.Name + "," + SortIndex + "," + Role?.Name;
    }

    #region Functions
    public F_Endorsement ShallowCopy()
    {
        return (F_Endorsement)this.MemberwiseClone();
    }
    #region Getter
    public Boolean IsRequestor()
    {
        return (RoleId == 100);
    }
    public Boolean IsRequestorManager()
    {
        return (RoleId == 101);
    }
    #endregion

    #endregion

}