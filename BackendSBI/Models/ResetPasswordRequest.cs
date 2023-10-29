namespace BackendSBI.Models
{
    public class ResetPasswordRequest
    {
        public string UserId { get; set; }
        public string Otp { get; set; }
        public string NewPassword { get; set; }
    }
}
