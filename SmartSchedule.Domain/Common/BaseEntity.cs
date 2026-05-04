namespace SmartSchedule.Domain.Common
{
    public abstract class BaseEntity
    {
        public virtual Guid Id { get; set; }
        public virtual DateTime CreatedAtUtc { get; set; }
        public virtual DateTime UpdatedAtUtc { get; set; }
        public virtual DateTime? DeletedAtUtc { get; set; } = null;
    }
}