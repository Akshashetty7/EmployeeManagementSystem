namespace EmployeeManagementSystem.Services
{
    public interface INotificationService
    {
        Task SendLeaveStatusUpdateAsync(string toEmail, string toName, string leaveType, string status, string remarks);
        Task SendWelcomeEmailAsync(string toEmail, string employeeName, string tempPassword);
        Task SendLeaveReminderAsync(string managerEmail, string managerName, int pendingCount);
    }

    // Mock email service — replace with SendGrid/SMTP in production
    public class NotificationService : INotificationService
    {
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(ILogger<NotificationService> logger) => _logger = logger;

        public Task SendLeaveStatusUpdateAsync(string toEmail, string toName, string leaveType, string status, string remarks)
        {
            _logger.LogInformation("[EMAIL] To: {email} | Subject: Leave Request {status} | Body: Dear {name}, your {type} leave has been {status}. Remarks: {remarks}",
                toEmail, status, toName, leaveType, status, remarks);
            return Task.CompletedTask;
        }

        public Task SendWelcomeEmailAsync(string toEmail, string employeeName, string tempPassword)
        {
            _logger.LogInformation("[EMAIL] To: {email} | Welcome to NexaCorp, {name}! Temp password: {pwd}",
                toEmail, employeeName, tempPassword);
            return Task.CompletedTask;
        }

        public Task SendLeaveReminderAsync(string managerEmail, string managerName, int pendingCount)
        {
            _logger.LogInformation("[EMAIL] To: {email} | Reminder: You have {count} pending leave approvals, {name}.",
                managerEmail, pendingCount, managerName);
            return Task.CompletedTask;
        }
    }
}
