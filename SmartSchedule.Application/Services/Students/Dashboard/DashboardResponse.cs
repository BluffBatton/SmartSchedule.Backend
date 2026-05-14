namespace SmartSchedule.Application.Services.Students.Dashboard
{
    public sealed record DashboardResponse
    {
        public int ActiveBookings { get; set; }
        public int AvailableTeachers { get; set; }
        public int UpcomingMeetings { get; set; }
        public List<ConsultationResponse> Consultations { get; set; } = new();
    }
}
