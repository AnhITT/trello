
namespace DataAccess_Layer.Interfaces
{
    public interface ITrack
    {
        Guid? CreatedBy { get; set; }
        DateTime CreatedDate { get; set; }
        Guid? ModifiedBy { get; set; }
        DateTime? ModifiedDate { get; set; }
        Guid? DeletedBy { get; set; }
        DateTime? DeletedDate { get; set; }
        bool IsDeleted { get; set; }
    }
}
