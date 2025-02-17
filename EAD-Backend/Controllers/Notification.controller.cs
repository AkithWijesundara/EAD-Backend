/*
File: Notification.controller.cs
Author: Rathnayaka M.R.T.N
Description: Notification controller class for Notification management in the EAD project
created:  02/10/2024
*/



using EAD_Backend.Models;
using EAD_Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EAD_Backend.Controllers

{
    [Route("api/[controller]")]
    [ApiController]

    public class NotificationController : ControllerBase

    {
        private readonly NotificationService _notificationService;

        // Constructor
        public NotificationController(NotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        //! ======================================================== Define API Endpoints ============================================================>


        //create a notification
        [HttpPost("create")]
        public async Task<IActionResult> CreateNotification(Notification notification)
        {
            await _notificationService.CreateNotificationAsync(notification);
            return Ok(new { Message = "Notification created successfully." });
        }


        // Get all notifications for the logged-in user with status IsRead is false
        [HttpGet]
        public async Task<IActionResult> GetNotifications()
        {
            var userId = User.FindFirst("UserId")?.Value;
            var notifications = await _notificationService.GetNotificationsByUserIdAsync(userId);
            return Ok(notifications);
        }

        // Mark a notification as read
        [HttpPut("{id}")]
        public async Task<IActionResult> MarkNotificationAsRead(string id)
        {
            var userId = User.FindFirst("UserId")?.Value;
            var notification = await _notificationService.GetNotificationByIdAsync(id);

            if (notification == null)
            {
                return NotFound(new { Message = "Notification not found." });
            }

            if (notification.UserId != userId)
            {
                return Unauthorized(new { Message = "You are not authorized to mark this notification as read." });
            }

            notification.IsRead = true;
            await _notificationService.UpdateNotificationAsync(notification);

            return Ok(new { Message = "Notification marked as read." });
        }

        // Delete a notification
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotification(string id)
        {
            var userId = User.FindFirst("UserId")?.Value;
            var notification = await _notificationService.GetNotificationByIdAsync(id);

            if (notification == null)
            {
                return NotFound(new { Message = "Notification not found." });
            }

            if (notification.UserId != userId)
            {
                return Unauthorized(new { Message = "You are not authorized to delete this notification." });
            }

            await _notificationService.DeleteNotificationAsync(id);

            return Ok(new { Message = "Notification deleted successfully." });
        }
    }
}