/*
File: Notification.service.cs
Author: Rathnayaka M.R.T.N
Description: Notification service class for Notification management in the EAD project
created:  02/10/2024
*/


using EAD_Backend.Models;
using MongoDB.Driver;
using EAD_Backend.Data;
using EAD_Backend.DTOs;

namespace EAD_Backend.Services
{
    public class NotificationService

    {
        private readonly IMongoCollection<Notification> _notificationModel;

        // Constructor
        public NotificationService(MongoDBService mongoDbService)
        {
            _notificationModel = mongoDbService.Database?.GetCollection<Notification>("notifications");
        }

        //! =======================================================  Define Business | DB Operations for Notifications ===================================>



        // Create a notification
        public async Task CreateNotificationAsync(Notification notification)
        {
            await _notificationModel.InsertOneAsync(notification);
        }


        // Retrieve all notifications for a user with status IsRead is false
        public async Task<List<Notification>> GetNotificationsByUserIdAsync(string userId)
        {
            return await _notificationModel.Find(notification => notification.UserId == userId && notification.IsRead == false).ToListAsync();
        }



        // Retrieve a notification by ID
        public async Task<Notification> GetNotificationByIdAsync(string id)
        {
            return await _notificationModel.Find(notification => notification.Id == id).FirstOrDefaultAsync();
        }



        // Update a notification
        public async Task UpdateNotificationAsync(Notification notification)
        {
            await _notificationModel.ReplaceOneAsync(n => n.Id == notification.Id, notification);
        }



        // Delete a notification
        public async Task DeleteNotificationAsync(string id)
        {
            await _notificationModel.DeleteOneAsync(notification => notification.Id == id);
        }
    }
}