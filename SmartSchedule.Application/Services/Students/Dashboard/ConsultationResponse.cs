namespace SmartSchedule.Application.Services.Students.Dashboard
{
    public sealed record ConsultationResponse
    {
        public required string TeacherFullName { get; set; }
        public required string DepartmentName { get; set; }
        public DateTime ConsultationStartTime { get; set; }
        public DateTime ConsultationEndTime { get; set; }
        public DateTime BookingDate { get; set; }
    }
}
