using System;

namespace WebApp.Models
{
    public class GameInvitationModel
    {
        public Guid Id { get; set; } 
        public string EmailTo { get; set; } 
        public string InvitedBy { get; set; } 
        public bool IsConfirmed { get; set; } 
        public DateTime ConfirmationDate { get; set; }
    }
}