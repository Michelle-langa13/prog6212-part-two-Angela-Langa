using Microsoft.AspNetCore.Http;
namespace EventManagementAPI.Helpers
{
    public static class SessionHelper
    {
        public static void SetUserSession(ISession session, int userId, string role, string email)
        {
            session.SetInt32("UserId", userId);
            session.SetString("UserRole", role);
            session.SetString("UserEmail", email);
        }
        public static int? GetUserId(ISession session) => session.GetInt32("UserId");
        public static string? GetUserRole(ISession session) => session.GetString("UserRole");
        public static bool IsAuthenticated(ISession session) => GetUserId(session) != null;
        public static bool IsOrganiser(ISession session) => GetUserRole(session) == "Organiser" || GetUserRole(session) == "Admin";
        public static bool IsParticipant(ISession session) => GetUserRole(session) == "Participant";
    }
}